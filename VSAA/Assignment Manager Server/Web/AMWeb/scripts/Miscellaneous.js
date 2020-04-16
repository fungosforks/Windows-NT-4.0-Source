//****Global Directory Structure Strings
var dir_BackSlash = "\\";
var dir_ForwardSlash = "/";

//****Miscelaneous Global variables
//var Courses_File_Name = "Courses.xml";
var g_Action = "";											//The action from the querystring
var g_UploadSize = 0;	
var g_TotalAllowedSize = 0;								
var g_BytesInMegaBytes = 1048576;
var g_StopRecursing = false;
//********************************************//
//***   End Global Variables			   ***//
//********************************************//
//*** Fires on the Page_Load() event ***//
function InitializePage() {

		//*** Grab all query strings and place the known ones into variables
		if(self.document.URL.lastIndexOf("?") != -1) {
			
			//*** Splits out querystring into known global variables.
			findActionQuerystring();
			
			//***Determine what the function of the page should be based on the action
			switch (g_Action.toLowerCase()) {
				case "uploadsubmission":
					showProjectsDiv();
					getCurrentSolutionsProjects();
					break;
				case "uploadstarter":
					showProjectsDiv();
					getCurrentSolutionsProjects();
					break;
				case "downloadstarter":
					setSolutionDefaultLocation();
					showDownloadDiv();
					break;
				case "downloadsubmission":
					setSolutionDefaultLocation();
					showDownloadDiv();
					break;
				default:
					break;	
			}
		}
}
function showDownloadDiv() {
	divDownload.className = "";
}
function hideDownloadDiv() {
	divDownload.className = "invisible";
	
}
function showProjectsDiv() {
	divProjects.className = "";
}

function InitializeAssignmentPage() {
	frm.cboGeneralAssignmentStarterProject.options[frm.cboGeneralAssignmentStarterProject.options.length] = new Option("");
	for(var i=1;i<window.external.Solution.Projects.Count+1;i++){
		frm.cboGeneralAssignmentStarterProject.options[frm.cboGeneralAssignmentStarterProject.options.length] = new Option(window.external.Solution.Projects(i).Name);
	}
}
function getCurrentSolutionsProjects() {
	//frm.cboProject.options[frm.cboProject.options.length] = new Option("");
	for(var i=1;i<window.external.Solution.Projects.Count+1;i++){
		frm.cboProject.options[frm.cboProject.options.length] = new Option(window.external.Solution.Projects(i).Name);
	}

}

function getFileName(strPath){
	if(strPath != null && strPath != "" && strPath.length > 0){
		for(var i=strPath.length;i>=1; i--){
			if(strPath.substring(i-1,i) == dir_BackSlash){
				return strPath.substring(i,strPath.length);
			}
		}
	}
}

function getFoldersFromRoot(strFileRootFolder,strFileToSend) {
	strFileRootFolder = strFileRootFolder.toUpperCase();
	strFileToSend = strFileToSend.toUpperCase();
	if(strFileToSend.lastIndexOf(strFileRootFolder) >= 0) {
		var str = strFileToSend.substring(strFileToSend.lastIndexOf(strFileRootFolder) + strFileRootFolder.length, strFileToSend.length);
		var final = str.substring(0, str.lastIndexOf(dir_BackSlash));
		return final;
	}
	else{
		return "";
	}
}

function findActionQuerystring() {
	//*** Split querystring into array based on "&" separator
	var ar = self.document.URL.split("&");
	
	//***Trim the first param to only include the querystring and not the URL
	ar[0] = ar[0].substring(ar[0].lastIndexOf("?")+1,ar[0].length);
	
	//***Cycle through the array grabbing name / value pairs
	for(var i=0;i<ar.length;i++) {
		var name = ar[i].substring(0,ar[i].lastIndexOf("="));
		var value = ar[i].substring(ar[i].lastIndexOf("=")+1,ar[i].length);
		//***Assign known querystring params to variables
		if(name != "" && name != null) {
			switch (name.toLowerCase()) {
				case "action":
					g_Action = value;
					break;
				default:
					break;
			}
		}
	}
}

function replaceForwardSlash(str){
	return str.replace(dir_ForwardSlash,dir_BackSlash);
}
function removeGuid(filePath) {
	var index = filePath.indexOf(dir_ForwardSlash) + 1;
	var rtn = filePath.substring(index);
	return rtn;
}
function setSolutionDefaultLocation() {
	
	//*** Write the download location (folder location of the current solution) into the text box.
	document.frm.txtDownloadLocation.value =  window.external.Properties("Environment", "ProjectsAndSolution").Item("ProjectsLocation").Value + dir_BackSlash + document.frm.txtSolutionName.value + dir_BackSlash;
}