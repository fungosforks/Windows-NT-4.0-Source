//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//
using System;
using Microsoft.Office;
using System.Runtime.InteropServices;
using AssignmentManager.ClientUI;

namespace FacultyClient 
{

	// stub class used to CoCreateInitialize VisualStudio.DTE.7.1
	[ComImport, Guid("8CD2DD97-4EC1-4bc4-9359-89A3EEDD57A6")]
	internal class VisualStudio_DTE_7_1
	{
	}

	internal class CodeExtractor : Object 
	{
		public CodeExtractor(EnvDTE._DTE dte) 
		{ 
			if (dte == null) 
			{
				throw new System.ArgumentNullException("dte");
			}
			m_application = dte; 
			//      m_taskWindow = null;
		}

		/// <summary>
		/// Given a source and a destination, this function creates a new project, optionally performing code
		/// extraction upon it. A return of false means that either the project could not be created or 
		/// there was an error with the extraction process.
		/// </summary>
		/// <param name="sourceUniqueProjectName"> The 'uniquename' property of the project to retrieve params from</param>
		/// <param name="path"> Output path, UNC / file paths only</param>
		/// <param name="destProjectName"> Name of the project to create </param>
		/// <param name="performExtraction"> Whether or not to extract properly-delimited sections of text from the source</param>
		public bool CreateNewProject(string sourceUniqueProjectName, string path, string destProjectName, bool performExtraction) 
		{
			bool fSucceeded = true;
			if (sourceUniqueProjectName == null) 
			{
				throw new System.ArgumentNullException("sourceUniqueProjectName");
			}
			if (path == null) 
			{
				throw new System.ArgumentNullException("path");
			}
			if (destProjectName == null) 
			{
				throw new System.ArgumentNullException("destProjectName");
			}

			string strProjDestUniqueName = null;
			EnvDTE.Project projDest = null;
			EnvDTE.Project projSource = null;
			EnvDTE._DTE dteOutOfProcess = null;

			try 
			{
				//        if (m_taskWindow == null) {
				//          m_taskWindow = new TaskWindow.DelayLoadTaskWindow(m_application);
				//        }

				projSource = GetValidProject(sourceUniqueProjectName);
				if (projSource == null) 
				{
					throw new Exception(AMResources.GetLocalizedString("CodeExtractorNoBaseProject"));
				}

				// NOTE: This is a method in the docs, but a property in implementation.
				object ignore = m_application.ItemOperations.PromptToSave;


				dteOutOfProcess = (EnvDTE._DTE) new VisualStudio_DTE_7_1();
				projDest = CopyProject(projSource, dteOutOfProcess, path, destProjectName);
				if (projDest == null) 
				{
					throw new Exception(AMResources.GetLocalizedString("CodeExtractorInvalidProjectType"));
				}

				strProjDestUniqueName = projDest.UniqueName;
				if (performExtraction) 
				{
					// Perform extraction - on first user error, open handle to task-list. Log all errors to it.
					if (!ExtractItems(projSource.ProjectItems, dteOutOfProcess, projDest.ProjectItems)) 
					{
						throw new Exception(AMResources.GetLocalizedString("CodeExtractorInvalidMarkedTags"));
					}
				}
			} 
			finally 
			{
				if ((dteOutOfProcess != null) &&
					(dteOutOfProcess != m_application)) 
				{
					// The solution has to be closed because if we're out of process, the
					// application might not close for a little while, leaving around 
					// extant 'file opened exclusively' problems.
					dteOutOfProcess.Solution.Close(false);

					dteOutOfProcess.Quit();
					dteOutOfProcess = null;
				}
			}

			return fSucceeded;
		}

		/// <summary>
		/// Loops through each of the items in the project, attempting to extract any sections of code 
		/// marked.
		/// </summary>
		/// <param name="dte"> Pointer to Object Model in which all actions should be performed </param>
		private bool ExtractItems(EnvDTE.ProjectItems projSourceItems, EnvDTE._DTE dte, EnvDTE.ProjectItems projDestItems) 
		{ 
			EnvDTE.ProjectItem projItem = null;
			EnvDTE.TextDocument textDoc = null;
			EnvDTE.Properties extractorProperties = null;
			Extensions extensions = null;
			CommentPair comments = null;
			EnvDTE.Window w = null;

			bool fSuccess = true;
			int i, nItems, nLastIndex;
			string strExtension;

			extractorProperties = m_application.get_Properties("Assignment Manager", "Code Extractor");
			if (extractorProperties == null) 
			{
				throw new Exception("The Academic Code Extractor is not properly installed and configured.");
			}
			extensions = extractorProperties.Item("Extensions").Object as Extensions;
			if (extensions == null)	
			{
				throw new Exception("The Academic Code Extractor is not properly installed and configured.");
			}
	
			nItems = projDestItems.Count;
			for (i = 1; i <= nItems; i++) 
			{
				projItem = projDestItems.Item(i);
				try 
				{
                    if (projItem.ProjectItems.Count > 0)
                    {
                        ExtractItems(projSourceItems.Item(i).ProjectItems, dte, projItem.ProjectItems);
                    }
					// Note that this will *actually* be happening in an invisible
					// out-of-process instance of VS, so the user will not be
					// subjected to appearing / disappearing windows.
					w = projItem.Open(EnvDTE.Constants.vsViewKindTextView);
					textDoc = w.Document.Object("TextDocument") as EnvDTE.TextDocument;

					strExtension = projItem.get_FileNames(1);
					
					nLastIndex = strExtension.LastIndexOf('.');
					if (nLastIndex == -1) 
					{
						w.Close(EnvDTE.vsSaveChanges.vsSaveChangesNo);
						continue; // We have no capacity for files with no extension.
					}
					strExtension = strExtension.Remove(0, nLastIndex + 1); // Trim off the 'name.' part of 'name.ext'

					comments = extensions[strExtension];
					if (comments == null) 
					{
						w.Close(EnvDTE.vsSaveChanges.vsSaveChangesNo);
						continue; // This file has no associated extension type. Ignore it.
					}

					fSuccess &= ExtractText(textDoc, comments.BeginComment, comments.EndComment, projSourceItems);
              
					w.Close(EnvDTE.vsSaveChanges.vsSaveChangesYes);
				} 
				catch (Exception /*e*/) 
				{
					// If we end up here, that simply means that the file
					// has no text. Since we obviously don't want to remove the
					// textual tags from a file with no comments...
					if (w != null) 
					{
						w.Close(EnvDTE.vsSaveChanges.vsSaveChangesNo);
					}
				}
			}
	
			return fSuccess;
		}

		/// <summary>
		/// Simple shell function to start off the recursion and handle any exceptions that 'bubble
		/// up' from recursive calls.
		/// </summary>
		private bool ExtractText(EnvDTE.TextDocument textDoc, string begin, string end, EnvDTE.ProjectItems projSourceItems) 
		{
			try 
			{
				return ExtractTextHelper(textDoc.Parent, textDoc.StartPoint.CreateEditPoint(), begin, end, projSourceItems, 0);
			} 
			catch (Exception /*e*/) 
			{
				return false;
			}
		}																					

		/// <summary>
		/// This actually removes the text or, if there are errors, logs them to the task list.
		/// 
		/// There are six cases to handle:
		/// <no tags> -> true
		/// <just a start> -> error
		/// <just an end> -> error
		/// <end> <start> -> error
		/// <start> <start> -> error
		/// <start> <end> -> success -- remove *and* proceed to loop again!
		/// </summary>
		/// <param name="TextDoc"> Document upon which to operate. </param>
		/// <param name="currentPos"> The current position from which execution should proceed. </param>
		/// <param name="begin"> String denoting the begin tag to find. </param>
		/// <param name="end"> String denoting the end tag to find. </param>
		/// <param name="linesRemoved"> Number of lines that have already been removed from the file. </param>
		private bool ExtractTextHelper(EnvDTE.Document doc, EnvDTE.EditPoint currentPos, string begin, string end, EnvDTE.ProjectItems projSourceItems, int linesRemoved) 
		{
			EnvDTE.EditPoint startTagPoint, endTagPoint, nextStartTagPoint;
			EnvDTE.TextRanges trIgnore = null;
			bool fGotStart, fGotEnd, fGotNextStart;
			
			startTagPoint = currentPos.CreateEditPoint();
			endTagPoint = currentPos.CreateEditPoint();

			fGotStart = startTagPoint.FindPattern(begin, (int)EnvDTE.vsFindOptions.vsFindOptionsNone, ref startTagPoint, ref trIgnore);
			fGotEnd = endTagPoint.FindPattern(end, (int)EnvDTE.vsFindOptions.vsFindOptionsNone, ref endTagPoint, ref trIgnore);
			
			if (!fGotStart && !fGotEnd) 
			{
				return true;
			}

			if (fGotStart && !fGotEnd) 
			{
				// Error, end tag expected after startTagPoint
				string strError = AMResources.GetLocalizedString("TagValidationMissingEndAfter");
				//        m_taskWindow.LogErrorToTaskWindow(System.String.Format(strError, end), 
				//          projSourceItems.Item(doc.Name).get_FileNames(1), startTagPoint.Line + linesRemoved);
				return false;
			}

			if (fGotEnd && !fGotStart) 
			{
				// Error, start tag expected before endTagPoint
				string strError = AMResources.GetLocalizedString("TagValidationMissingStartBefore");
				//        m_taskWindow.LogErrorToTaskWindow(System.String.Format(strError, begin),
				//          projSourceItems.Item(doc.Name).get_FileNames(1), endTagPoint.Line + linesRemoved);
				return false;
			}

			if (endTagPoint.AbsoluteCharOffset < startTagPoint.AbsoluteCharOffset) 
			{
				// Error, start tag expected before endTagPoint
				string strError = AMResources.GetLocalizedString("TagValidationMissingStartBefore");
				//        m_taskWindow.LogErrorToTaskWindow(System.String.Format(strError, begin),
				//          projSourceItems.Item(doc.Name).get_FileNames(1), endTagPoint.Line + linesRemoved);
				return false;
			}

			nextStartTagPoint = startTagPoint.CreateEditPoint();
			fGotNextStart = nextStartTagPoint.FindPattern(begin, (int)EnvDTE.vsFindOptions.vsFindOptionsNone, ref nextStartTagPoint, ref trIgnore);
			
			if (fGotNextStart && (endTagPoint.AbsoluteCharOffset > nextStartTagPoint.AbsoluteCharOffset)) 
			{
				// Error, end tag expected before endTagPoint
				string strError = AMResources.GetLocalizedString("TagValidationMissingEndBefore");
				//        m_taskWindow.LogErrorToTaskWindow(System.String.Format(strError, end),
				//          projSourceItems.Item(doc.Name).get_FileNames(1), nextStartTagPoint.Line + linesRemoved);
				return false;
			}

			if (startTagPoint.AbsoluteCharOffset < endTagPoint.AbsoluteCharOffset) 
			{
				linesRemoved += (endTagPoint.Line - startTagPoint.Line);
				startTagPoint.CharLeft(begin.Length);
				startTagPoint.Delete((object)endTagPoint);

				return ExtractTextHelper(doc, endTagPoint, begin, end, projSourceItems, linesRemoved);
			}

			System.Diagnostics.Debug.Assert(false, "Impossible code path followed in ExtractTextHelper()");
			return true;
		}

		/// <summary>
		/// Takes a source project in one instance of the environment and requests for another instances
		/// to create a copy of the project.
		/// </summary>
		/// <param name="projSource"> Original project to copy from </param>
		/// <param name="dteCreateIn"> Object Model to create it in </param>
		/// <param name="strDestFolder"> Output folder </param>
		/// <param name="strDestName"> Output project name </param>
		private EnvDTE.Project CopyProject(EnvDTE.Project projSource, EnvDTE._DTE dteCreateIn, string strDestFolder, string strDestName) 
		{
			EnvDTE.Project projDest = null;

			try 
			{
				projDest = dteCreateIn.Solution.AddFromTemplate(projSource.FileName, strDestFolder, strDestName, false);

				return projDest;
			} 
			catch (System.Exception e) 
			{
				// In the event of errors, make it seem as if 'nothing happened'.
				if (projDest != null) 
				{
					projDest.Delete();
				}
				return null;
			}
		}

		/// <summary>
		/// This function returns the currently-active project object, provided that it is a valid 
		/// project. Validity is defined as:
		/// - It's open
		/// - All items in the project are stored off of the root directory where the 
		/// project itself is located
		/// </summary>
		private EnvDTE.Project GetValidProject(string strName) 
		{
			EnvDTE.Project CurrentProj = null;

			if (strName == EnvDTE.Constants.vsMiscFilesProjectUniqueName) 
			{
				// Copying the Misc Files project doesn't make any sense...
				return null;
			}

			try 
			{
				EnvDTE.Projects CurrentProjs = null;
				EnvDTE.ProjectItems Items = null;
				int nLastIndex;
				string strProjectRootPath;

				CurrentProjs = m_application.Solution.Projects;
				CurrentProj = CurrentProjs.Item(strName);
				if (CurrentProj == null) 
				{
					return null;
				}
				strProjectRootPath = CurrentProj.FileName;
				
				nLastIndex = strProjectRootPath.LastIndexOf('\\');
				if (nLastIndex == -1) 
				{
					nLastIndex = strProjectRootPath.LastIndexOf('/');
					if (nLastIndex == -1) 
					{
						return null;
					}
				}
				strProjectRootPath = strProjectRootPath.Substring(0, nLastIndex + 1); // Just keep the 'path\' of 'path\name'
				strProjectRootPath = strProjectRootPath.ToUpper();

				Items = CurrentProj.ProjectItems;
				if (!ValidProjectItems(Items, strProjectRootPath)) 
				{
					return null;
				}

			} 
			catch (Exception /*e*/) 
			{
				return null;
			}

			return CurrentProj;
		}

		/// <summary>
		/// ValidProjectItems recursively travels down the tree of items provided by
		/// the projectitems, checking for validity. This check is here to ensure that
		/// users to not attempt to copy projects that contain file types that would 
		/// require hard path references or for us to otherwise have to perform
		/// complex transformations on the project structure on-disk to copy it.
		/// </summary>
		/// <param name="Items"> The ProjectItems to recursively check.</param>
		/// <param name="strProjectRootPath"> The upper-case version of the root of the project path.</param>
		private bool ValidProjectItems(EnvDTE.ProjectItems Items, string strProjectRootPath) 
		{
			EnvDTE.ProjectItem Item = null;
			int i, j, nItemCount, nFileCount;
			string strFilePath, strFileKind;

			nItemCount = Items.Count;
			for (i = 1; i <= nItemCount; i++) 
			{
				Item = Items.Item(i);
				nFileCount = Item.FileCount;
						
				for (j = 1; j <= nFileCount; j++) 
				{
					strFilePath = Item.get_FileNames((short)j);
					strFilePath = strFilePath.ToUpper();
					strFileKind = Item.Kind;

					if ((strFileKind == EnvDTE.Constants.vsProjectItemKindPhysicalFile) ||
						(strFileKind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder)) 
					{
						if (!strFilePath.StartsWith(strProjectRootPath)) 
						{
							return false;
						}
					} 
					else if ((strFileKind == EnvDTE.Constants.vsProjectItemKindSubProject) ||
						(strFileKind == EnvDTE.Constants.vsProjectItemKindVirtualFolder) ||
						(strFileKind == EnvDTE.Constants.vsProjectItemKindMisc) ||
						(strFileKind == EnvDTE.Constants.vsProjectItemKindSolutionItems)) 
					{
						// Do nothing. We leave these kinds of items alone.
					} 
					else if ((new System.IO.FileInfo(strFilePath)).Exists) 
					{ 
						// If we fall down to this branch or further, it means that the package-provider
						// did not follow the specifications for exposing items through the object model.
						// However, since we cannot guarantee compliance, we must support this behavior.
						if (!strFilePath.StartsWith(strProjectRootPath)) 
						{
							return false;
						}
					} 
					else if ((new System.IO.DirectoryInfo(strFilePath).Exists)) 
					{
						try 
						{
							if (!ValidProjectItems(Item.ProjectItems, strProjectRootPath)) 
							{
								return false;
							}
						} 
						catch (Exception /*e*/) 
						{
							// Falling into here means that though the directory exists
							// on disk, it isn't necessarily holding any files. Some
							// project types also enumerate all of the files + all of
							// the directories as ProjectItems, rather than nesting
							// them in the more intuitive way. We must support both.
						}
					}
					// Otherwise, it's some kind of 'virtual object' and will be ignored
					// during the copying process, since it's probably built into the 
					// settings in the project file anyway.
				}
			}

			return true;
		}

		private EnvDTE._DTE m_application;
		//    private TaskWindow.DelayLoadTaskWindow m_taskWindow;
	}
}
