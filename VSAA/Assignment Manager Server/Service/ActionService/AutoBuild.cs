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

namespace Microsoft.VisualStudio.Academic.AssignmentManager.ActionService
{
	/// <summary>
	/// Summary description for AutoBuild.
	/// </summary>
	public class AutoBuild : IActionService
	{
		// constants
		private const string BACKSLASH = " / ";
		private const string BACKSLASH_BUILD = " /build ";
		private const string BACKSLASH_PROJECT = " /project ";
		private const string BACKSLASH_OUT = " /out ";
		private const string EMPTY_SPACE = " ";
		private const string DEVENV = "devenv.com";
		private const string OUTPUT_FILE = "output.log";

		private string workingDirectory = "";
		private string buildResults = "";

		private StudentAssignmentM studentAssignment = null;
		private string projectFileLocation = string.Empty; // location of the project file to build the project
		private string sourceLocation = string.Empty;	// location of source files for this assignment
		private int processTime = 0;		// amount of time before timing ref a server action; user-configurable
		private bool buildSuccessful = false;		// if the build was successful or not
		private string projectName = string.Empty;		// project name
		private string buildType = string.Empty;			// build type debug, release
		private string buildPath = string.Empty;			// path to the output from the buil

		private int outputExitCode;

		public void Init(int userAssignmentID, string tempDir)
		{
			workingDirectory = tempDir;
			studentAssignment = StudentAssignmentM.Load(userAssignmentID);
		}

		public void RetrieveElements()
		{
			try
			{
				sourceLocation = string.Empty;
				projectFileLocation = string.Empty;
				projectName = String.Empty;
				processTime = 0;
				buildType = String.Empty;
				buildPath = String.Empty;

				AssignmentM assign = AssignmentM.Load(studentAssignment.AssignmentID);
				
				sourceLocation = assign.StorageDirectory + studentAssignment.UserID;
				sourceLocation = SharedSupport.AddBackSlashToDirectory(sourceLocation.Trim());

				//Get the allowable time to compile
				processTime = Convert.ToInt32(SharedSupport.GetSetting(Constants.MAX_PROCESS_SETTING)); 

				// get project file location and project name
				ProjectInfo objPI = new ProjectInfo(sourceLocation);
				projectFileLocation = objPI.ProjectFile.Trim();
				projectName = objPI.ProjectName.Trim();
				buildType = objPI.ConfigName.Trim();
				buildPath = objPI.BuildPath.Trim();		
	
				//Copy UserAssignment files to temp location
				ServerAction.CopyDirectories(sourceLocation, workingDirectory, true, true);	
				
				// get the projectFile from the path
				string projectFile = Path.GetFileName(projectFileLocation);
	
				// change the projectFileLocation because we copied to the working directory
				projectFileLocation = SharedSupport.AddBackSlashToDirectory(workingDirectory) + projectFile;

			}
			catch(Exception ex)
			{
				SharedSupport.HandleError(ex);
			}				
		}

		public void RunService()
		{
			//Create a new Process	
			UserProcess compile = new UserProcess();	
			try
			{				
				System.Text.ASCIIEncoding AE = new System.Text.ASCIIEncoding();
				byte[] ByteArray = {34};	// "
				string singleQuote = AE.GetString(ByteArray);
				
				// path to output file
				string outputFilePath = SharedSupport.AddBackSlashToDirectory(workingDirectory) + OUTPUT_FILE;
				if (File.Exists(outputFilePath)) File.Delete(outputFilePath);

				//put quotes around command line arguments to avoid problems with spaces
				projectFileLocation = "\"" + projectFileLocation +"\"";
				buildType = "\"" + buildType + "\"";
				projectName = "\"" + projectName + "\"";

				//populate the process information	
				compile.StartInfo.WorkingDirectory = workingDirectory;
				compile.StartInfo.FileName = getDevEnvPath() + DEVENV;
				compile.StartInfo.Arguments = projectFileLocation + BACKSLASH_BUILD + buildType + BACKSLASH_PROJECT + projectName + BACKSLASH_OUT + OUTPUT_FILE;
				compile.OutputFile = outputFilePath;
				compile.StartInfo.RedirectStandardOutput = true;
				compile.StartInfo.RedirectStandardError = false;
				compile.StartInfo.UseShellExecute = false;

				
				//start the compile process
				if (!compile.Run(processTime))
				{
					throw new System.Exception(SharedSupport.GetLocalizedString("ServerAction_FailedCreateBuildProcess"));
				}

				// if the process exceeds allotted time, kill 
				if(!compile.HasExited)
				{
					outputExitCode = Constants.AUTOCOMPILE_RETURN_CODE_FAILURE;
					buildResults = SharedSupport.GetLocalizedString("StudentAssignment_Killed");
				}
				else
				{
					// ok: exited before allotted time
					outputExitCode = (int) compile.ExitCode;

					// retrieve outcome from output.log file
					StreamReader sr = new StreamReader(new FileStream(outputFilePath, FileMode.Open, FileAccess.Read));
					buildResults = sr.ReadToEnd();
					sr.Close();
				}

				// return compile results (true/false)
				if (outputExitCode == Constants.AUTOCOMPILE_RETURN_CODE_SUCCESS) 
				{
					buildSuccessful = true;
				}
			}
			catch(System.Exception ex)
			{
					buildResults = ex.Message;
					buildSuccessful = false;
			}
		}

		public void StoreResult()
		{
			// copy the resulting exe file(s) to the root of the working directory for use in checking if build successful

			if(buildSuccessful)
			{
				string binDirectory = SharedSupport.AddBackSlashToDirectory(workingDirectory) + buildPath;
				ServerAction.CopyDirectories(binDirectory, sourceLocation, true, false);

				//store the failure
				studentAssignment.BuildDetails = buildResults;
				studentAssignment.LastUpdatedDate = System.DateTime.Now;
				studentAssignment.AutoCompileStatus = Constants.AUTOCOMPILE_SUCCESSFUL_STATUS;
				studentAssignment.BuildResultCode = Constants.AUTOCOMPILE_RETURN_CODE_SUCCESS.ToString();
				studentAssignment.Update();
			}
			else
			{
				//store the failure
				studentAssignment.BuildDetails = buildResults;
				studentAssignment.LastUpdatedDate = System.DateTime.Now;
				studentAssignment.AutoCompileStatus = Constants.AUTOCOMPILE_FAILURE_STATUS;
				studentAssignment.BuildResultCode = Constants.AUTOCOMPILE_RETURN_CODE_FAILURE.ToString();
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

		/// <summary>
		///		Create an Xml document represnting the output from the compile.
		/// </summary>
		/// <param name="exitCode"> </param>
		/// <param name="outputFileLocation"> </param>
		private string createBuildXml(int exitCode, string output)
		{
			try
			{
				string xml = "";
				xml += "<?xml version='1.0'?>";
				xml += "<CompileResult>";
				xml += "<ResultCode>" + exitCode + "</ResultCode>";
				xml += "<Details><![CDATA[" + output + "]]></Details>";
				xml += "</CompileResult>";

				return xml;
			}
			catch(System.Exception ex)
			{
				SharedSupport.HandleError(ex);
				return String.Empty;
			}
		}

		public static bool Run(int buildUserAssnId, string workingDirectory)
		{
			AutoBuild autoBuild = new AutoBuild();
			autoBuild.Init(buildUserAssnId, workingDirectory);
			autoBuild.RetrieveElements();
			autoBuild.RunService();
			autoBuild.StoreResult();
			autoBuild.Cleanup();
			return autoBuild.buildSuccessful;
		}

		private string getDevEnvPath()
		{
			// retrieve the install location for devenve.exe - used to build
			string path = "";
			Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\VisualStudio\\7.1");
			if (regKey != null) 
			{
				path = regKey.GetValue(@"InstallDir", @"").ToString();		
			}
			return path;
		}

		public AutoBuild()
		{
		}
	}
}
