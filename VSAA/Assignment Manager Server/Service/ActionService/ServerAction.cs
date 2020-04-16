//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//


namespace Microsoft.VisualStudio.Academic.AssignmentManager.ActionService
{
    using System;
	using System.Messaging;
	using System.Xml;
	using System.Xml.XPath;
	using System.IO;
    using System.Diagnostics;
	using System.ComponentModel;
	
    /// <summary>
    ///    Summary description for ServerAction.
    /// </summary>
    /// 
	internal class ServerAction
	{
		internal ServerAction()
		{
		}

		// reads message off the queue; calls ProcessMessage
		internal void PeekMessage()
		{
			//call PeekMessage with the default queue path
			PeekMessage(@Constants.ACTION_QUEUE_PATH);
		}

		// reads message off the queue; calls ProcessMessage
		internal void PeekMessage(string mqPath)
		{
			try
			{
				// grab a handle to the queue
				MessageQueue mq = new MessageQueue(mqPath);
				((XmlMessageFormatter)mq.Formatter).TargetTypeNames = new string[]{"System.String"};

				// receive the next message with a timeout of 30 seconds
				System.Messaging.Message msg = mq.Peek(new TimeSpan(0,0,0,Constants.QUEUE_TIMEOUT,0));

				// save message ID; will remove from queue later if all is successful
				string msgId = msg.Id;

				// log 
				SharedSupport.LogMessage(SharedSupport.GetLocalizedString("ServerAction_PeekMessage") + msg.Label + " " + msg.Body);

				// set boolean to remove from queue
				bool removeFromQueue = ProcessMessage(msg.Body.ToString(), msg.Label.ToString());

				// if ProcessMessage true, remove the message from the queue
				if(removeFromQueue == true)
				{
					mq.ReceiveById(msgId, new TimeSpan(0,0,0,Constants.QUEUE_TIMEOUT,0));

					// log
					SharedSupport.LogMessage(SharedSupport.GetLocalizedString("ServerAction_RemoveMessage") + msg.Label + " " + msg.Body);
				}

			}
			catch(System.Exception ex)
			{
				SharedSupport.HandleError(ex);
			}
			
		}

		// validates, processes directions inside XML Message; returns true if ok to remove msg from queue
		internal bool ProcessMessage(string message, string label)
		{

			// always return true because always want to remove message from queue in this implementation
			try
			{	
				// raise error if no label or message
				label = label.Trim();
				message = message.Trim();
				if (label.Equals(String.Empty))
				{					 
					SharedSupport.HandleError(new System.Exception(SharedSupport.GetLocalizedString("ServerAction_MissingLabel")));
				}
				if (message.Equals(String.Empty))
				{					 
					SharedSupport.HandleError(new System.Exception(SharedSupport.GetLocalizedString("ServerAction_MissingBody")));
				}

				// xml object declarations
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(message);	//load the string as xml
				XPathNavigator nav = ((IXPathNavigable)doc).CreateNavigator();
				
				// misc vars
				bool bCheckRequested = false;	// track if check requested
				bool bBuildSuccessful = false;	// track if build successful
				int buildUserAssnId = 0;	// build userAssignmentId
				int checkUserAssnId = 0;	// check userAssignmentId
				string buildResults = string.Empty;	// results of the build
				string checkResults = string.Empty;	// results of the check
				string workingDirectory = string.Empty;	// working directory

				//-------------------------------------------------------------------------------------------------------------
				// evaluate label, parse xml message; extract instructions
				//-------------------------------------------------------------------------------------------------------------

				switch(label)
				{
						////////////////////////////////////////////////////////////////
						// This is a submit action
						////////////////////////////////////////////////////////////////
					case Constants.AM_SUBMIT_ACTION:
						
						// select the serverActions
						XPathNodeIterator serverActionsIterator = nav.Select("serverActions/serverAction");

						// retrieve all serverAction actions from xml
						while (serverActionsIterator.MoveNext())
						{			
							// perform the appropriate action evaluating the serverAction attribute
							string actionCommand = serverActionsIterator.Current.GetAttribute("name", doc.NamespaceURI).ToString().Trim();
							switch(actionCommand)
							{
									//////////////////////////////////////////////////////////////
									// AutoBuild
									/////////////////////////////////////////////////////////////
								case Constants.AM_BUILD:
									// grab the userAssignmentId from the xml
									buildUserAssnId = Convert.ToInt32(serverActionsIterator.Current.Value.ToString());

									/////////////////////////////////////////////////
									// extensibility: add custom parameters here
									/////////////////////////////////////////////////
									break;

									/////////////////////////////////////////////////////////////
									// AutoCheck
									/////////////////////////////////////////////////////////////
								case Constants.AM_CHECK:
									// set check flag
									bCheckRequested = true;

									// grab the userAssignmentId from the xml
									checkUserAssnId = Convert.ToInt32(serverActionsIterator.Current.Value.ToString());

									/////////////////////////////////////////////////
									// extensibility: add custom parameters here
									/////////////////////////////////////////////////									
									break;

									/////////////////////////////////////////////////
									// extensibility: add future submit actions here
									/////////////////////////////////////////////////
									// 1. load all actions from ServerActions table
									// 2. loop actions; do any match xml serverAction element?
									// 3. if so, retrieve parameters from ServerActions
									// 4. make 'late bound' call to proper class, method
									/////////////////////////////////////////////////
					
								default:
									break;
							} 	
						} 						
		
						break;						

						/////////////////////////////////////////////////
						// extensibility: add future actions here
						/////////////////////////////////////////////////

					default:
						throw new System.NotImplementedException(SharedSupport.GetLocalizedString("ServerAction_ActionNotImplemented"));
						
				}
				
				//-------------------------------------------------------------------------------------------------------------
				// execute instructions from xml message in proper order
				//-------------------------------------------------------------------------------------------------------------
				// perform prep work, execute build, record results; returns true if successful
				// we always do the build: necessary for check and expected by custom actions

				// set the working directory
				workingDirectory = getUserWorkingDirectory();

				////////////////////////////////////////////
				// Perform AutoBuild
				////////////////////////////////////////////
				
				// check to make sure we have a valid userAssignmentId
				if(buildUserAssnId == 0)
				{
					if(checkUserAssnId == 0)
					{
						// raise error
						SharedSupport.HandleError(new System.Exception(SharedSupport.GetLocalizedString("ServerAction_MissingUserAssignmentElement")));
					}
					else
					{
						// set the buildUserAssnId = check
						buildUserAssnId = checkUserAssnId;
					}
				}

				// raise error if no buildUserAssnId
				if (buildUserAssnId <= 0)
				{
					SharedSupport.HandleError(new System.Exception(SharedSupport.GetLocalizedString("ServerAction_MissingUserAssignmentElement")));
				}

				// raise error if build disabled on the server
				if (!Convert.ToBoolean(SharedSupport.GetSetting(Constants.AUTOBUILD_SETTING)))
				{
					SharedSupport.HandleError(new System.Exception(SharedSupport.GetLocalizedString("ServerAction_BuildDisabled")));
				}

				// delete any previous user assignment detail records for this userassignment and detail type
				deleteUserAssignmentDetails(buildUserAssnId, Constants.AUTO_COMPILE_DETAILTYPE);

				bBuildSuccessful = AutoBuild.Run(buildUserAssnId, workingDirectory);
				
				// was check requested? 
				if(bCheckRequested)
				{
					////////////////////////////////////////////
					// Perform AutoCheck
					////////////////////////////////////////////

					// raise error if no checkUserAssnId
					if (checkUserAssnId <= 0)
					{
						SharedSupport.HandleError(new System.Exception(SharedSupport.GetLocalizedString("ServerAction_MissingUserAssignmentElement")));
					}

					// check that checkUserAssnId = buildUserAssnId;
					if(checkUserAssnId != buildUserAssnId)
					{
						// raise error
						SharedSupport.HandleError(new System.Exception(SharedSupport.GetLocalizedString("ServerAction_DifferentUserAssignmentIDs")));
					}	
					else
					{						
						// did the build execute ok if it was requested?
						if(bBuildSuccessful)
						{
							// raise error if check disabled on the server
							if (!Convert.ToBoolean(SharedSupport.GetSetting(Constants.AUTOCHECK_SETTING)))
							{
								SharedSupport.HandleError(new System.Exception(SharedSupport.GetLocalizedString("ServerAction_CheckDisabled")));
							}

							AutoCheck.Run(buildUserAssnId, workingDirectory);

						}			
					}
				}
				////////////////////////////////////////////
				// extensibility: CUSTOM ACTIONS
				////////////////////////////////////////////	
			}
			catch(System.Exception ex)
			{
				SharedSupport.LogMessage(ex.Message + " " + SharedSupport.GetLocalizedString("ServerAction_GeneralProcessError") + " " + label + " " + SharedSupport.GetLocalizedString("ServerAction_GeneralProcessErrorBody") + " " + message + " ", SharedSupport.GetLocalizedString("ServerAction_MethodName"), System.Diagnostics.EventLogEntryType.Warning);
			}

			// returning true here ensures message is removed from the queue
			return true;	

		}

		private string getUserWorkingDirectory()
		{
			string workingDirectory = string.Empty;

			//Get system temp location, add guid
			string newGuid = System.Guid.NewGuid().ToString();
			workingDirectory = SharedSupport.AddBackSlashToDirectory(System.Environment.GetEnvironmentVariable(Constants.TEMP_ENVIRON_VARIABLE));
			workingDirectory += SharedSupport.AddBackSlashToDirectory(Constants.ASSIGNMENT_MANAGER_DIRECTORY);
			workingDirectory += SharedSupport.AddBackSlashToDirectory(newGuid);
			workingDirectory = workingDirectory.Trim();		
			return workingDirectory;
		}

		private void deleteUserAssignmentDetails(int userAssignmentId, int detailType)
		{
			try
			{
				StudentAssignmentM stuAssign = StudentAssignmentM.Load(userAssignmentId);
				stuAssign.BuildDetails = "";
				stuAssign.BuildResultCode = "";
				stuAssign.AutoCompileStatus = Constants.AUTOCOMPILE_PENDING_STATUS;
			
				stuAssign.CheckDetails = "";
				stuAssign.CheckResultCode = "";
				stuAssign.Update();
			}
			catch(Exception ex)
			{
				SharedSupport.HandleError(ex);	
			}

		}

		/// <summary>
		///		Copies one directory, all files, and all sub directories to another directory.
		/// </summary>
		/// <param name="sourceDirectory">Directory to copy files and sub-directories from. </param>
		/// <param name="destinationDirectory">Directory to copy files and sub-directories to.  </param>
		/// <param name="overwriteFlag">Determines whether or not to overwrite a file if it already exists at the given location</param>
		internal static void CopyDirectories(string sourceDirectory, string destinationDirectory, bool overwriteFlag, bool useImpersonation) 
		{
			SecurityACL dacl = null;
			try
			{
				sourceDirectory = SharedSupport.AddBackSlashToDirectory(sourceDirectory);
				destinationDirectory = SharedSupport.AddBackSlashToDirectory(destinationDirectory);

				if(!Directory.Exists(sourceDirectory))
				{
					//If the source directory does not exist throw a new error.
					throw new DirectoryNotFoundException(SharedSupport.GetLocalizedString("StudentAssignment_DirectoryNotFound") + sourceDirectory);
				}
				if(!Directory.Exists(destinationDirectory))
				{				

					if (useImpersonation)
					{
						// Impersonate the AM User
						ImpersonateUser User = null;
						try
						{
							User = new ImpersonateUser();

							// Login
							User.Logon();

							// start impersonating
							User.Start();

							//if the destination does not exist, create it.
							Directory.CreateDirectory(destinationDirectory);

						}
						finally
						{
							if (User != null)
							{
                                // stop impersonating
                                User.Stop();

								User.Dispose();
							}
						}
					}
					else
					{
						Directory.CreateDirectory(destinationDirectory);
					}

				}
				//copy each file in the current directory
				string[] f = Directory.GetFiles(sourceDirectory);
				dacl = new SecurityACL(Constants.AMUserName);
				for(int i=0;i<f.Length;i++)
				{
					if(!File.Exists(f[i].ToString()))
					{
						throw new FileNotFoundException(sourceDirectory + f[i].ToString());
					}
					else
					{
						string sourceFile = sourceDirectory + Path.GetFileName(f[i].ToString());
						string destinationFile = destinationDirectory + Path.GetFileName(f[i].ToString());
						File.Copy(sourceFile,destinationFile,overwriteFlag);
						dacl.ApplyACLToFile(destinationFile);
					}
					
				}
				// recursively copy each subdirectory in the current directory
				string[] d = Directory.GetDirectories(sourceDirectory);
				if(d.Length > 0)
				{
					for (int i=0;i<d.Length;i++)
					{
						string sourceDir = d[i].ToString();
						string destDir = destinationDirectory + d[i].ToString().Replace(sourceDirectory, String.Empty);

						if(!Directory.Exists(destDir)) 
						{
							if (useImpersonation)
							{
								// Impersonate the AM User
								ImpersonateUser User = null;
								try
								{
									User = new ImpersonateUser();

									// Login
									User.Logon();

									// start impersonating
									User.Start();

									//if the destination does not exist, create it.
									Directory.CreateDirectory(destDir);
								}
								finally
								{
									if (User != null)
									{
                                        // stop impersonating
                                        User.Stop();
										User.Dispose();
									}
								}
							}
							else
							{
								Directory.CreateDirectory(destDir);
							}
						}
						ServerAction.CopyDirectories(sourceDir, destDir, overwriteFlag, useImpersonation);
					}
				}
			}
			catch(Exception ex)
			{
				SharedSupport.HandleError(ex);	
			}
			finally
			{
				if (dacl != null)
				{
					dacl.Dispose();
				}
			}
		}
	}
}
