// Localized strings
var L_ErrMsg_Text = "A critical error occurred during the creation process. Removing all generated files...";
var L_ErrNoFile_Text = "Error in loading the file: ";
var L_ErrCreateProject_Text = "Error in creating project: ";
var L_ErrNoTemplatesInfMsg_Text = "Cannot create TemplatesInf.txt file in temp directory: ";

// The VC DHTML-based Wizard infrastructure calls the OnFinish function. We
// follow the following steps:
// - Close the current solution and create a new one
// - Add and configure the two output modes (Debug & Release)
// - Add the filters (folders which 'hold' source & header files)
// - Add all of the files into the project
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

		OpenFile(selProj, strProjectPath + '\\' + strProjectName + '.cpp');
		
		Cleanup(InfFile);
		var projName = strProjectPath + "\\" + strProjectName + ".vcproj";
		
		selProj.Object.Save();
	}
	catch(e)
	{
		wizard.ReportError(L_ErrMsg_Text + e.description);
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

// This function handles all necessary name mangling of the files.
// For instance, the 'main.cpp' file needs to be changed to <projname>.cpp
function GetTargetName(strName, strProjectName)
{
	var strTarget = strName;

	if (strName.substr(0, 4) == "main")
	{
		var strlen = strName.length;
		strTarget = strProjectName + strName.substr(4, strlen - 4);
	}

	return strTarget; 
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
	var projItems = proj.ProjectItems
	var strTemplatePath = wizard.FindSymbol('TEMPLATES_PATH');

	var strTpl = '';
	var strName = '';

	var strTextStream = InfFile.OpenAsTextStream(1, -2);
	while (!strTextStream.AtEndOfStream)
	{
		strTpl = strTextStream.ReadLine();
		if (strTpl != '')
		{
			strName = strTpl;
			var strTarget = GetTargetName(strName, strProjectName);
			var strTemplate = strTemplatePath + '\\' + strTpl;
			var strFile = strProjectPath + '\\' + strTarget;
			wizard.RenderTemplate(strTemplate, strFile, false);

			var projfile = proj.Object.AddFile(strFile);
		}
	}
	
	strTextStream.Close();
}

function OpenFile(proj, itemName)
{
	try
	{
          var oItems = proj.ProjectItems;
          var projFile = oItems(itemName);
          if (projFile != null)
          {
            var window = projFile.Open("{00000000-0000-0000-0000-000000000000}");
            if(window)
            {
              window.visible = true;
            }
          }
	}
	catch(e)
	{
		wizard.ReportError(L_ErrNoFile_Text + e.description);
		throw false;
	}
}

// These are the identical filters to the ones created by the standard
// VC Wizards.
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
