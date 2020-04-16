//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//

using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Academic.AssignmentManager.ActionService
{
	/// <summary>
	/// Summary description for AutoBuild.
	/// </summary>
	public class AutoCheck : IActionService
	{
		private StudentAssignmentM studentAssignment;
		private string workingDirectory = "";

		private string compiledFileLocation = string.Empty;
		private string compiledFileName = string.Empty;
		private string commandLineParams = string.Empty;
		private string inputFileLocation = string.Empty;
		private string expectedOutputFileLocation = string.Empty;
		private int processTime = 0;
		private string sourceLocation = string.Empty;
		private string actualOutputFile = string.Empty;
		private string actualOutputFileLocation = "";

		private string errorString = "";
		internal bool checkSuccessful = true;

		public void Init(int userAssignmentID, string tempDir)
		{
			workingDirectory = tempDir;
			studentAssignment = StudentAssignmentM.Load(userAssignmentID);
		}

		public void RetrieveElements()
		{
			try
			{

				AssignmentM assign = AssignmentM.Load(studentAssignment.AssignmentID);
				processTime = Convert.ToInt32(SharedSupport.GetSetting(Constants.MAX_PROCESS_SETTING));

				// student source location
				sourceLocation = assign.StorageDirectory + studentAssignment.UserID;
				
				// get compiled file name (already built by build process)
				ProjectInfo objPI = new ProjectInfo(sourceLocation);
				compiledFileName = objPI.OutputFile;				
				
				inputFileLocation = assign.InputFile;
				expectedOutputFileLocation = assign.OutputFile;
				actualOutputFile = expectedOutputFileLocation;
				commandLineParams = assign.CommandLineArgs;

				compiledFileLocation = SharedSupport.AddBackSlashToDirectory(workingDirectory) + compiledFileName;
				expectedOutputFileLocation = assign.StorageDirectory + expectedOutputFileLocation;

				//change to use inputFileLocation in tempworkarea
				inputFileLocation = SharedSupport.AddBackSlashToDirectory(assign.StorageDirectory) + inputFileLocation;							

				//Get the user's submission folder location
				string userSubmissionRootFolder = SharedSupport.AddBackSlashToDirectory(sourceLocation);
				
				//Copy all files under the student's submission directory, to the working area.
				ServerAction.CopyDirectories(userSubmissionRootFolder,workingDirectory,true, true);
			}
			catch(Exception ex)
			{
				SharedSupport.HandleError(ex);	
			}			
		}

		public void RunService()
		{
			//Spawn new process - invoking the compiledFile
			UserProcess oProcess = new UserProcess();

			try
			{
				// set actual output file location
				actualOutputFileLocation = workingDirectory + actualOutputFile;
				
				//Set process start parameters
				ProcessStartInfo psi = new ProcessStartInfo();
				psi.Arguments = commandLineParams;
				psi.FileName = compiledFileName;
				psi.WorkingDirectory = workingDirectory;
				psi.RedirectStandardOutput = true;
				psi.RedirectStandardError = false;
				psi.UseShellExecute = false;
				
				if (File.Exists(inputFileLocation))
				{
					oProcess.InputFile = workingDirectory + Guid.NewGuid() + ".txt";
					File.Copy(inputFileLocation, oProcess.InputFile, true);
					psi.RedirectStandardInput = true;
					try
					{
						SecurityACL dacl = new SecurityACL(Constants.AMUserName);
						dacl.ApplyACLToFile(oProcess.InputFile);
					}
					catch(Exception)
					{
						// Continue on.  If we fail to apply the ACL, we may still be able
						// to autocheck (i.e. if user has set custom permissions.
					}
				}
				
				oProcess.StartInfo = psi;
				oProcess.OutputFile = actualOutputFileLocation;

				//Log start
				SharedSupport.LogMessage(SharedSupport.GetLocalizedString("StudentAssignment_CheckStart"));

				if (!oProcess.Run(processTime))
				{
					throw new System.Exception(SharedSupport.GetLocalizedString("ServerAction_FailedCreateTestProcess"));
				}

				//wait to see if process comes back in time.  Otherwise if it runs too long - kill it
				if(!oProcess.HasExited)
				{
					SharedSupport.LogMessage(SharedSupport.GetLocalizedString("StudentAssignment_CheckKilled"));
					throw new System.Exception(SharedSupport.GetLocalizedString("StudentAssignemtn_CheckExceededProcessTime"));
				}
				else
				{
					SharedSupport.LogMessage(SharedSupport.GetLocalizedString("StudentAssignment_CheckEnd"));
				}
			}
			catch(Exception ex)
			{	
				checkSuccessful = false;
				errorString = ex.Message;
				SharedSupport.HandleError(ex);
			}		
		}

		public void StoreResult()
		{
			if (checkSuccessful)
			{
				try
				{

					// return results of comparision
					string checkResults = compareResults();

					//Load checkResults into XMLDom 
					string element = string.Empty;
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(checkResults);	//load the string as xml
					XPathNavigator nav = ((IXPathNavigable)doc).CreateNavigator();
					XPathNodeIterator xpni;

					xpni = nav.Select("/GradingResults/ResultCode");
					
					xpni.MoveNext();
					element = xpni.Current.Value.ToString();
					studentAssignment.CheckResultCode = element;
					xpni = nav.Select("/GradingResults/Details/ActualResults");
					xpni.MoveNext();
					element = xpni.Current.Value.ToString();		
					if(element != "<![CDATA[]]>")
					{
						studentAssignment.CheckDetails = element;
					}
					else
					{
						studentAssignment.CheckDetails = String.Empty;
					}
			
					if (checkSuccessful)
					{
						studentAssignment.AutoGradeStatus = Constants.AUTOGRADE_SUCCESSFUL_STATUS;
						studentAssignment.CheckResultCode = Constants.GRADE_SUCCESSFUL_RESULT_CODE.ToString();
					}
					else
					{
						studentAssignment.AutoGradeStatus = Constants.AUTOGRADE_FAILURE_STATUS;
						studentAssignment.CheckResultCode= Constants.GRADE_FAILED_RESULT_CODE.ToString();
					}

					studentAssignment.Update();
				}
				catch(Exception ex)
				{
					SharedSupport.HandleError(ex);	
				}
			}
			else
			{
				studentAssignment.AutoGradeStatus = Constants.AUTOGRADE_FAILURE_STATUS;
				studentAssignment.CheckDetails = errorString;
				studentAssignment.LastUpdatedDate = System.DateTime.Now;
				studentAssignment.CheckResultCode = Constants.GRADE_FAILED_RESULT_CODE.ToString();
				studentAssignment.Update();
			}
		}

		public void Cleanup()
		{
			try
			{
				Directory.Delete(workingDirectory, true);
			}
			catch
			{
			}
		}


		public static bool Run(int buildUserAssnId, string workingDirectory)
		{
			AutoCheck autoCheck = new AutoCheck();
			autoCheck.Init(buildUserAssnId, workingDirectory);
			autoCheck.RetrieveElements();
			autoCheck.RunService();
			autoCheck.StoreResult();
			autoCheck.Cleanup();
			return autoCheck.checkSuccessful;
		}


		/// <summary>
		///		Open both files and read the bytes.
		///		Convert the bytes into strings into strings.
		///		Compare the strings.
		///		If the strings are equal, the solution passed.
		///		If the string are different, the solution failed.
		/// </summary>
		/// <param name="outputFile">Professor-defined output file - Student's resulting file should be exactly the same.</param>
		/// <param name="generatedOutputResultsFile">Student's results file</param>
		private string compareResults()
		{
			try 
			{
				string actualOutputFileString = String.Empty;
				//open pre-defined output file into a stream
				Stream oActualOutputFile = File.OpenRead(actualOutputFileLocation);

				StreamReader actualFileReader = new StreamReader(oActualOutputFile);
				actualOutputFileString = actualFileReader.ReadToEnd();
				actualFileReader.Close();

				string expectedOutputFileString = String.Empty;
				//open generated output file into a stream
				Stream oExpectedOutputFile = File.OpenRead(expectedOutputFileLocation);
				
				StreamReader expectedFileReader = new StreamReader(oExpectedOutputFile);
				expectedOutputFileString = expectedFileReader.ReadToEnd();
				expectedFileReader.Close();

				//build xml string with results
				string xml = "";
				xml += "<?xml version='1.0'?>";
				xml += "<GradingResults>";
				xml += "<ResultCode>";	
				if(actualOutputFileString == expectedOutputFileString)
				{
					xml += Constants.GRADE_SUCCESSFUL_RESULT_CODE.ToString();//Successful
					checkSuccessful = true;
				}
				else
				{
					xml += Constants.GRADE_FAILED_RESULT_CODE.ToString();//Failed
					checkSuccessful = false;
				}
				xml += "</ResultCode>";
				xml += "<Details>";
				xml += "<ExpectedResults><![CDATA[";
				xml += expectedOutputFileString;
				xml += "]]></ExpectedResults>";
				xml += "<ActualResults><![CDATA[";
				xml += actualOutputFileString;
				xml += "]]></ActualResults>";
				xml += "</Details>";
				xml += "</GradingResults>";

				return xml;	
			} 
			catch
			{
				//build xml string with results
				string xml = "";
				xml += "<?xml version='1.0'?>";
				xml += "<GradingResults>";
				xml += "<ResultCode>";	
				xml += Constants.GRADE_FAILED_RESULT_CODE.ToString();//Failed
				checkSuccessful = false;
				xml += "</ResultCode>";
				xml += "<Details>";
				xml += "<ActualResults><![CDATA[";
				xml += SharedSupport.GetLocalizedString("StudentAssignment_NoExpectedOutputFile_Error");
				xml += "]]></ActualResults>";
				xml += "</Details>";
				xml += "</GradingResults>";
			
				return xml;
			}
		}


		public AutoCheck()
		{
		}
	}
}
