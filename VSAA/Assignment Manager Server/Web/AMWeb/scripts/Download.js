
function copyFileFromServerToLocalFile(strFileToGet, strLocalLocation) {
	try
	{
		var xmlhttp = new ActiveXObject("Msxml2.ServerXMLHTTP");
		strFileToGet = encodeURI(strFileToGet);
		xmlhttp.open("GET", strFileToGet, false);
		xmlhttp.send();
	}
	catch(e)
	{
	}
	//check to make sure that the status is between 200 and 300.
	if(xmlhttp.status >= 200 && xmlhttp.status < 300) {
		//save response stream
		putFile(xmlhttp.responseStream,strLocalLocation)
		return true;
	}else
	{
		return false;
	}
}

function getDownloadDirectory(){
	var oDialog = new ActiveXObject("Shell.Application"); 
    var oFolderLocation;
    var sLoc = "";
    oFolderLocation = oDialog.BrowseForFolder(0,UploadDownload_DownloadPrompt,0);
    if(oFolderLocation != "" && oFolderLocation != null){
		sLoc = oFolderLocation.Items().Item().Path;
		if(sLoc.lastIndexOf(dir_BackSlash) != sLoc.length-1){
			frm.txtDownloadLocation.value = sLoc + dir_BackSlash;
		}else{
			frm.txtDownloadLocation.value = sLoc;
		}
    }
}

function putFile(FileStream, localFileLocation) {
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	//if the location file already exists...delete it.
	if(fso.FileExists(localFileLocation)){
		fso.DeleteFile(localFileLocation,true);
	}
	var obj = null;
	try { 
		obj = window.external.AddIns.Item("StudentClient.Connect").Object;
	}
	catch (e) {
		// Ignore this exception and try to reference the faculty addin.
	}
	if(obj == null){
		try { 
			obj = window.external.AddIns.Item("FacultyClient.Connect").Object;
		}
		catch (e) {
			// Ignore this exception and test whether an instance of either the faculty or student addin was created.
		}
	}
	if(obj == null){
		// Both the Faculty and the Student Academic Core Tools were unavailable.
		alert(err_Addin_Not_Loaded);
		return null;
	}
	
	return obj.SaveFileStream(FileStream, localFileLocation);
	
	//var Stream = new ActiveXObject("ADODB.Stream");
	//Stream.Type = 1; //Binary
	//Stream.Open();
	//write the response stream to the ado stream
	//Stream.Write(FileStream);
	//Save stream to local file
	//Stream.SaveToFile(localFileLocation)
	//Stream.Close();
}

function getFilesFromXML(xmlFileList) {
	var xmlDoc = new ActiveXObject("MSXML2.DOMDocument.3.0");
	xmlDoc.loadXML(xmlFileList);
	//Grab all xml "filename" nodes
	var oSelection = xmlDoc.selectNodes("//FileName");
	var oItem;
	if (oSelection == null) {
		alert(err_XMLFileList_Load_Failed);
		return;
	}
	if(frm.txtDownloadLocation.value.substring(frm.txtDownloadLocation.value.length-1,frm.txtDownloadLocation.value.length) != dir_BackSlash){
		frm.txtDownloadLocation.value += dir_BackSlash;
	}
	var sol_name = "";
	//traverse xml looking for files and their locations...
	while ((oItem = oSelection.nextNode()) != null) {
		if (oItem != null) {
			var file = oItem.text;
			var filenameminusguid = removeGuid(file);
			var localFileLocation = replaceForwardSlash(frm.txtDownloadLocation.value + filenameminusguid);
			createFolders(localFileLocation);
			copyFileFromServerToLocalFile(frm.txtDownloadFolderLocation.value + file,localFileLocation);
			
			//pickup project
			if(file.lastIndexOf("proj") == file.length-4){
				sol_name = filenameminusguid;
			}
		}
	}
	
	//once all files have been obtained from the server and copied locally, open the solution
	if(sol_name != "" && sol_name != null) {
		var fso = new ActiveXObject("Scripting.FileSystemObject");
		if(fso.FileExists(frm.txtDownloadLocation.value + sol_name))
		{
			try
			{
				if(window.external.Solution.IsOpen){
					if(window.confirm(UploadDownload_PromptToSave)){
						window.external.DTE.ExecuteCommand ("File.SaveAll");
					}
					if(window.confirm(UploadDownload_CloseCurrentSol)){
						window.external.Solution.AddFromFile(frm.txtDownloadLocation.value + sol_name, true);
					}else{
						window.external.Solution.AddFromFile(frm.txtDownloadLocation.value + sol_name, false);
					}
				}else{
					window.external.Solution.AddFromFile(frm.txtDownloadLocation.value + sol_name, false);
				}
			}
			catch(ex)
			{
				//Project is already open, you downloaded a project over a currently open project.
				alert(UploadDownload_ProjectAlreadyOpenInSolution);
				return;
			}
			
			// Download complete, redirect to UploadDownload.cs to delete temp files from server.
			var redirectUrl = self.document.URL
						
			/* Add GUID, AddQS, and TargetURL querystring parameters to the Url.
			   GUID is the directory to be deleted, AddQS is whether to include the original querystring 
			   on the second redirect, and TargetURL is the page to redirect to from Cleanup.aspx */
			fileGUID = file.substring(0, file.indexOf("/"));
			redirectUrl += "&GUID=" + escape(fileGUID);
			
			if(g_Action.toLowerCase() == "downloadsubmission"){
				// Change the Action querystring parameter to 'cleanupdirectory'.
				redirectUrl = redirectUrl.replace("downloadsubmission", "cleanupdirectory");
				redirectUrl += "&AddQS=true&TargetURL=GradeSubmission.aspx";
			}
			
			if(g_Action.toLowerCase() == "downloadstarter"){
				// Change the Action querystring parameter to 'cleanupdirectory'.
				redirectUrl = redirectUrl.replace("downloadstarter", "cleanupdirectory");
				
				if(UploadDownload_DownloadRedirectUrl == "" || UploadDownload_DownloadRedirectUrl == null){	
					redirectUrl += "&AddQS=true&TargetURL=Assignments.aspx";
				}
				else {
					// UrlEncode the UploadDownload_DownloadRedirectUrl to pass on the querystring.
					redirectUrl += "&AddQS=false&TargetURL=" + escape(UploadDownload_DownloadRedirectUrl);
				}
			}
			window.navigate(redirectUrl);
		}
		else
		{
			alert(err_Opening_Project);
		}
	}
}

function createFolders(strFilePath) {

    // Change all forward-slashes to back-slashes before creating directory structure.
    strFilePath = strFilePath.replace(/\//g, "\\");
    
    // Get all of the constituant directories.
    var rgAllFolders = strFilePath.split(dir_BackSlash);

    var length = rgAllFolders.length;
    var i = 0;
    var strFolderRoot = "";

    var fso = new ActiveXObject("Scripting.FileSystemObject");
    // Loop through the directories and create them if they don't exist
    //  until you reach the filename(length-1 stops us short of the file name).
    for (i = 0; i < length-1; i++) {
		if (rgAllFolders[i] != "") {
			strFolderRoot = strFolderRoot + rgAllFolders[i] + dir_BackSlash;
			// Test whether strFolderRoot already exists before creating folder.
			if(!fso.FolderExists(strFolderRoot)) {
				fso.CreateFolder(strFolderRoot);
			}
        }
    }
}

function beginDownloadofFiles() {
	if(frm.txtDownloadFilesXML.value != ""){
		if(frm.txtDownloadLocation.value != "") {
			getFilesFromXML(frm.txtDownloadFilesXML.value);
		}else{
			alert(err_Select_Location);
		}
	}else{
		if(g_Action == "downloadstarter"){
			alert(err_No_Starter_Files);
		}else{
			alert(err_No_Submitted_Files);
		}
	}
}
