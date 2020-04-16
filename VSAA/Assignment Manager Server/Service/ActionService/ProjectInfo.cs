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
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Academic.AssignmentManager.ActionService
{
	
	// stub class used to CoCreateInitialize VisualStudio.DTE.7.1
	[ComImport, Guid("8CD2DD97-4EC1-4bc4-9359-89A3EEDD57A6")]
	internal class VisualStudio_DTE_7_1
	{
	}

	/// <summary>
	/// Summary description for ProjectInfo.
	/// </summary>
	internal class ProjectInfo
	{

		internal ProjectInfo(string projectPath)
		{
			//assign string constants values

			// constructor logic 
			ProjectFile = getProjectFile(projectPath);
			getProjectFileInfo(ProjectFile, out ProjectName, out OutputFile, out ConfigName, out BuildPath);
		}
		
		internal readonly string ProjectFile;
		internal readonly string ProjectName;
		internal readonly string OutputFile;
		internal readonly string ConfigName;
		internal readonly string BuildPath;

		private string getProjectFile(string projPath)
		{
			//grab the project file from the directory passed in
			if(System.IO.Directory.Exists(projPath))
			{
				string SEARCH_PATTERN = "*.*proj";
				string[] projFile = System.IO.Directory.GetFiles(projPath,SEARCH_PATTERN);
				//return the first project file found.
				return projFile[0];
			}
			else
			{
				throw new System.IO.DirectoryNotFoundException(SharedSupport.GetLocalizedString("ProjectInfo_DirNotFound") + projPath);
			}
		}

		private string getPath(string path)
		{
			return path.Substring(0,path.LastIndexOf(@"\")+1);
		}

		internal void getProjectFileInfo(string fileName, out string name, out string outputFile, out string configurationName, out string buildPath)
		{
			EnvDTE._DTE dte = null;
			EnvDTE.Configuration config = null;
			string Name, OutputFile, ConfigurationName, BuildPath;

			buildPath = String.Empty;
			configurationName = String.Empty;
			outputFile = String.Empty;
			name = String.Empty;

			try 
			{
				EnvDTE.Project proj = null;
				EnvDTE.OutputGroup group = null;
				string outputURLDir, projectFileDir;
				Object[] OutputFiles = null;
				Object[] OutputURLs = null;
				int nIndex;

				dte = (EnvDTE._DTE) new VisualStudio_DTE_7_1();

				dte.Solution.AddFromFile(fileName, false);
				proj = dte.Solution.Projects.Item(1);
				Name = proj.Name;
				config = proj.ConfigurationManager.ActiveConfiguration;
				ConfigurationName = config.ConfigurationName;

				// Loop through the possibly-many set of output groups, looking for the
				// one that has the build output. If we don't can't locate it, we will 
				// attempt to use the first one in the list.
				nIndex = config.OutputGroups.Count;
				do 
				{
					group = config.OutputGroups.Item(nIndex);
					nIndex--;
				} while ((nIndex > 0) && (group.CanonicalName != "Built"));

				OutputFiles = (Object[])group.FileNames;
				OutputFile = (string)OutputFiles[0];

				OutputURLs = (Object[])group.FileURLs;
				outputURLDir = (string)OutputURLs[0];

				// Given a full URL to the file path (file://c:\....) and the base path
				// to the project file (c:\...), determine the set of additional directories
				// into which the build is being performed.
				projectFileDir = getPath(fileName);
				projectFileDir = projectFileDir.ToUpper();
				outputURLDir = outputURLDir.ToUpper();
				nIndex = outputURLDir.LastIndexOf(projectFileDir);
				BuildPath = outputURLDir.Substring(nIndex + projectFileDir.Length);
				BuildPath = getPath(BuildPath);

				name = Name;
				outputFile = OutputFile;
				configurationName = ConfigurationName;
				buildPath = BuildPath;

			} 
			catch (System.Exception) 
			{
				throw new System.Exception(SharedSupport.GetLocalizedString("ProjectInfo_ProjFileInvalid"));
			} 
			finally 
			{
				if (dte != null) 
				{
					try 
					{
						dte.Solution.Close(false);
						dte.Quit();
					} 
					catch (System.Exception) 
					{
						// Ignore errors when shutting down out-of-process IDE.
					}
				}
			}

		}

	}
}
