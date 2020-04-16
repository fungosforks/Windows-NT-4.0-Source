// Localized strings
var L_ErrNoScriptingObjectMsg_Text = "The Scripting.FileSystemObject was not available. Please re-install.";
var L_ErrMsg_Text = "A critical error occurred during the creation process. Removing all generated files...";
var L_ErrNoTemplatesInfMsg_Text = "Cannot create TemplatesInf.txt file in temp directory: ";
var L_ErrNoVC_Text = "The Visual C++ product is not correctly installed. Please re-install.";
var L_ErrNoAPFiles_Text = "The AP files have not been installed on this system. Please place them into the VC Include directory.";
var L_ErrNoStartupFile_Text = "Error in loading the startup file: ";
var L_ErrCreateProject_Text = "Error in creating project: ";

// The VC DHTML-based Wizard infrastructure calls the OnFinish function. We
// follow the following steps:
// - Close the current solution and create a new one
// - Add and configure the two output modes (Debug & Release)
// - Add the filters (folders which 'hold' source & header files)
// - Add all of the files into the project (both templated AND AP files!)
// - Set the currently-active document to be the main file
// - Save & exit
function OnFinish(selProj, selObj)
{
	try
	{
		var strProjectPath = wizard.FindSymbol("PROJECT_PATH");
		var strProjectName = wizard.FindSymbol("PROJECT_NAME");

		selProj = CreateProject(strProjectName, strProjectPath);

		AddDebugConfig(selProj, strProjectName);
		
		AddReleaseConfig(selProj, strProjectName);
		
		AddFilters(selProj);

		var InfFile = CreateInfFile();
		
		AddFilesToProj(selProj, strProjectName, strProjectPath, InfFile);
		
		AddAPFilesToProj(selProj, strProjectPath);

		Cleanup(InfFile);
		var projName = strProjectPath + "\\" + strProjectName + ".vcproj";
		
		selProj.Object.Save();
	}
	catch(e)
	{
		wizard.ReportError(L_ErrMsg_Text);
		CloseAndDeleteSolution(selProj, strProjectPath);
	}
}

// If the project has been created, close the solution and such. Then,
// delete any files *if* they've been created
function CloseAndDeleteSolution(selProj, strProjectPath)
{
	var Solution = dte.solution;
	if (Solution) {
		Solution.Close();
	}
	
	try {
		var fso, folder, file, enumFiles, strFilePath, strFileName;
		fso = new ActiveXObject("Scripting.FileSystemObject");
		fso.DeleteFolder(strProjectPath);	
	} catch (e) {
		//Ignore any further errors at this point.
	}
}

function AddDebugConfig(proj, strProjectName)
{
	var config = proj.Object.Configurations("Debug|Win32");
	
	config.IntermediateDirectory = "Debug";
	config.OutputDirectory = "Debug";

	var CLTool = config.Tools("VCCLCompilerTool");

	CLTool.SuppressStartupBanner = true;
	CLTool.MinimalRebuild = true;
	CLTool.DebugInformationFormat = debugEditAndContinue;
	CLTool.Optimization = optimizeDisabled;
	CLTool.WarningLevel = warningLevel_4;
	CLTool.CompileOnly = true;
	CLTool.UsePrecompiledHeader = pchNone;
	CLTool.PreprocessorDefinitions = "WIN32;_DEBUG;_CONSOLE";
	CLTool.RuntimeLibrary = rtSingleThreadedDebug;
	CLTool.BrowseInformation = brAllInfo;
	CLTool.DisableSpecificWarnings = "4995";
	
	var LinkTool = config.Tools("VCLinkerTool");

	LinkTool.SuppressStartupBanner = true;
	LinkTool.ProgramDatabaseFile = "$(outdir)/" + strProjectName + ".pdb";
	LinkTool.AdditionalDependencies = "kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib";
	LinkTool.GenerateDebugInformation = true;
	LinkTool.LinkIncremental = linkIncrementalYes;
	LinkTool.SubSystem = subSystemConsole;
	LinkTool.OutputFile = "$(outdir)/" + strProjectName + ".exe";
}

function AddReleaseConfig(proj, strProjectName)
{
	var config = proj.Object.Configurations("Release|Win32");
	config.IntermediateDirectory = "Release";
	config.OutputDirectory = "Release";

	var CLTool = config.Tools("VCCLCompilerTool");
	
	CLTool.SuppressStartupBanner = true;
	CLTool.WarningLevel = warningLevel_4;
	CLTool.UsePrecompiledHeader = pchNone;
	CLTool.CompileOnly = true;
	CLTool.Optimization = optimizeMaxSpeed;
	CLTool.OmitFramePointers = true;
	CLTool.InlineFunctionExpansion = expandOnlyInline;
	CLTool.DebugInformationFormat = debugEnabled;
	CLTool.MinimalRebuild = false;
	CLTool.PreprocessorDefinitions = "WIN32;NDEBUG;_CONSOLE";
	CLTool.RuntimeLibrary = rtSingleThreaded;
	CLTool.DisableSpecificWarnings = "4995";

	var LinkTool = config.Tools("VCLinkerTool");
	
	LinkTool.SuppressStartupBanner = true; 
	LinkTool.SubSystem = subSystemConsole;
	LinkTool.GenerateDebugInformation = true;
	LinkTool.ProgramDatabaseFile = "$(outdir)/" + strProjectName + ".pdb";
	LinkTool.OutputFile = "$(outdir)/" + strProjectName + ".exe";
	LinkTool.AdditionalDependencies = "kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib";
	LinkTool.LinkIncremental = linkIncrementalNo;
}

function AddFilesToProj(proj, strProjectName, strProjectPath, InfFile)
{
	var strTemplatePath = wizard.FindSymbol('TEMPLATES_PATH');

	var strTpl = '';

	var strTextStream = InfFile.OpenAsTextStream(1, -2);
	while (!strTextStream.AtEndOfStream)
	{
		strTpl = strTextStream.ReadLine();
		if (strTpl != '')
		{
			strName = strTpl;
			var strTemplate = strTemplatePath + '\\' + strTpl;
			var strFile = strProjectPath + '\\' + strTpl;
			wizard.RenderTemplate(strTemplate, strFile, false);

			var projfile = proj.Object.AddFile(strFile);
		}
	}
	
	strTextStream.Close();
}

// To generate the VC Include path, we iterate through all
// of the possible include directories exposed by the
// VC product, looking for an AP subdirectory. Note that
// we can't hard-code a path from devenv, as VC can be
// installed anywhere.
function GenerateIncludePath(proj)
{
	var fso, IncludePath;
	
	try
	{
		fso = new ActiveXObject("Scripting.FileSystemObject");
	} 
	catch (e) 
	{
		wizard.ReportError(L_ErrNoScriptingObjectMsg_Text);
		throw false;
	}
			
	try 
	{
		var oVCPlatforms = dte.Properties("Projects", "VCDirectories").Item("Platforms").Object;
                var oVCConfiguration = proj.Object.Configurations("Release|Win32");
                var oVCPlatform, nPlatformCount, IncludePaths, IncludePath;
		var nPaths, PathsArray;
		
		nPlatformCount = oVCPlatforms.Count;
			
		for (i = 1; i <= nPlatformCount; i++) {
			oVCPlatform = oVCPlatforms.Item(i);
			IncludePaths = oVCPlatform.IncludeDirectories;

                        // The VC object model stores include paths in terms of system-neutral
                        // directory variables. The Evaluate method expands out the variables
                        // into their full file paths.
                        IncludePaths = oVCConfiguration.Evaluate(IncludePaths);

			// Include paths are semicolon-separated. Tokenize, and
			// test each.
			PathsArray = IncludePaths.split(';');
			if (PathsArray != null) {
				nPaths = PathsArray.length;

				for (i = 0; i < nPaths; i++) {
					IncludePath = PathsArray[i];
					var c = IncludePath.charAt(IncludePath.length - 1);
					
					if ((c == '/') || (c == '\\')) {
						IncludePath += "AP\\";
					} else {
						IncludePath += "\\AP\\";
					}
					
					if (fso.FolderExists(IncludePath)) {
						return IncludePath;
					}
				}
			}
		}
	}
	catch (e)
	{
		wizard.ReportError(L_ErrNoVC_Text);
		throw false;
	}

	wizard.ReportError(L_ErrNoAPFiles_Text);
	throw false;
}

function AddAPFilesToProj(proj, strProjectPath)
{
	var strAPPath = GenerateIncludePath(proj);
	var fso, folder, file, enumFiles, strFilePath, strFileName;
	
	try
	{
		fso = new ActiveXObject("Scripting.FileSystemObject");
	} 
	catch (e) 
	{
		wizard.ReportError(L_ErrNoScriptingObjectMsg_Text);
		throw false;
	}
		
	folder = fso.GetFolder(strAPPath);
	enumFiles = new Enumerator(folder.Files);

	for (; !enumFiles.atEnd(); enumFiles.moveNext())
	{
		file = enumFiles.item();
		strFilePath = file.Path;
		strFileName = file.Name;

		var strDest = strProjectPath + '\\' + strFileName;
		var newProjItem = null;

                // Even if the user has provided them in the AP/ directory,
                // do not copy files named main.cpp or readme.txt, since they
                // are provided by the template.
		if ((strFileName != "readme.txt") && (strFileName != "main.cpp")) {
                  fso.CopyFile(strFilePath, strDest);
                  newProjItem = proj.ProjectItems.AddFromFile(strDest);
                  if (APFileSetNonBuildable(strFileName)) {
                    var fileConfigurations;
                    var i;
                    var count;
                    
                    fileConfigurations = newProjItem.Object.FileConfigurations;
                    count = fileConfigurations.Count;
                    for (i = 1; i <= count; i++) {
                      fileConfigurations.Item(i).ExcludedFromBuild = true;
                    }
                  }
                }
	}
}

// The AP file contain two types of CPP files (in addition to the
// other files):  those that are intended to be compiled and
// those that are not. All header files should be added to the
// project normally, as should apstring.cpp. All of the other
// CPP files, however, should not. 
function APFileSetNonBuildable(strFileName)
{
	var regexp = /\.cpp$/i;
	if (!regexp.test(strFileName)) {
		return false;
	} else {
		if (strFileName == 'apstring.cpp') {
			return false;
		} else {
			return true;
		}
	}
}


// These are the identical filters to the ones created by 
// the standard VC Wizards.
function AddFilters(proj)
{
	var group = proj.Object.AddFilter('Source Files');
	group.Filter = 'cpp;c;cxx;rc;def;odl;idl;hpj;bat;asm';
	
	group = proj.Object.AddFilter('Header Files');
	group.Filter = 'h;hpp;hxx;hm;inl;inc';
	
	group = proj.Object.AddFilter('Resource Files');
	group.Filter = 'ico;cur;bmp;dlg;rc2;rct;bin;rgs;gif;jpg;jpeg;jpe';
}

function CreateProject(strProjectName, strProjectPath)
{
	try
	{
		var strProjTemplatePath = wizard.FindSymbol("PROJECT_TEMPLATE_PATH");
		var strProjTemplate = strProjTemplatePath + "\\default.vcproj"; 

		var Solution = dte.Solution;
		if (wizard.FindSymbol("CLOSE_SOLUTION"))
			Solution.Close();

		var strProjectNameWithExt = strProjectName + ".vcproj";
		var oTarget = wizard.FindSymbol("TARGET");
		var prj;
		if (wizard.FindSymbol("WIZARD_TYPE") == vsWizardAddSubProject)  // vsWizardAddSubProject
		{
			var prjItem = oTarget.AddFromTemplate(strProjTemplate, strProjectNameWithExt);
			prj = prjItem.SubProject;
		}
		else
		{
			prj = oTarget.AddFromTemplate(strProjTemplate, strProjectPath, strProjectNameWithExt);
		}
		return prj;
	}
	catch(e)
	{
		wizard.ReportError(L_ErrCreateProject_Text + e.description);
		throw false;
	}
}

function CreateInfFile()
{
	try
	{
		var fso, TemplatesFolder, TemplateFiles, strTemplate;
		fso = new ActiveXObject("Scripting.FileSystemObject");

		var TemporaryFolder = 2;
		var tfolder = fso.GetSpecialFolder(TemporaryFolder);
		var strTempFolder = tfolder.Drive + "\\" + tfolder.Name;

		var strWizTempFile = strTempFolder + "\\TemplatesInf.txt";

		CleanupTempFiles(fso, strWizTempFile);

		var strTemplatePath = wizard.FindSymbol("TEMPLATES_PATH");
		var strInfFile = strTemplatePath + "\\Templates.inf";
		wizard.RenderTemplate(strInfFile, strWizTempFile, false);

		var WizTempFile = fso.GetFile(strWizTempFile);
		return WizTempFile;

	}
	catch(e)
	{
		wizard.ReportError(L_ErrNoTemplatesInfMsg_Text + e.description);
		throw false;
	}
}

function Cleanup(InfFile)
{
	InfFile.Delete();
}
