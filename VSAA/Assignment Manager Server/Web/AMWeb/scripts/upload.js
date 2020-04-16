
var oldCursor;
var clean = "";

// GUIDs used by the shell through the project automation support to allow
// scripts to determine the type of a particular ProjectItem.
var vsProjectItemKindPhysicalFile = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";
var vsProjectItemKindVirtualFolder = "{6BB5F8F0-4483-11D3-8BCF-00C04F8EC28C}";
var vsProjectItemKindPhysicalFolder = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";

//*** Upload the selected project
function UploadProject(cbo, checkBlank, starterSubmit) {
//set the cursor variable equal to the current cursor
oldCursor = document.body.style.cursor;
			
	try
	{
		if((window.external.Solution.IsOpen != true || window.external.Solution.Projects.Count == 0) && g_Action != ""){
			alert(UploadDownload_NoSolutionOrProject);
			return false;
		}
		
		// Save opened project to make sure to include all files.
		window.external.DTE.ExecuteCommand ("File.SaveAll");

		//Figure out the maximum allowable upload size
		g_TotalAllowedSize = frm.txtMaxUploadSize.value;
		if(frm.txtDirSize.value < frm.txtMaxUploadSize.value) { 
			g_TotalAllowedSize = frm.txtDirSize.value;
		}
		//reset the total uploaded size
		g_UploadSize = 0;
		if(cbo.options[0].selected == true && cbo.options[0].text == "" && checkBlank == true) {
			alert(err_Select_Proj_To_Upload);
			return false;
		}else
		{
			if(cbo.options[0].selected == true && cbo.options[0].text == "" && checkBlank == false) {
				return true;
			}
		}

		//figure out which project, if any, was selected
		var i = -1;
		for(var x=0;x<cbo.options.length;x++){
			if(cbo.options[x].selected){
				if(cbo.options[0].text == ""){
					i = x;
				}else{
					i = x+1;
				}
			}
		}
		//if no project was selected, error
		if(i == -1){
			alert(err_Select_Proj_To_Upload);
			return false;
		}
		
		
		var fso = new ActiveXObject("Scripting.FileSystemObject");
		if(window.external.Solution.Projects(i).Properties != null){
			
			if(frm.txtExistingStarterProject.value == "1") {
				if(window.confirm(UploadDownload_AlreadyStarterUploaded) == false){
					return false;
				}
			}
				
			dir_Code_Stipping_TempDir = frm.txtNewGUID.value;
			//set cursor equal to hourglass
			document.body.style.cursor = "wait";
			//set the status bar text equal to "uploading..please wait..."
			window.external.StatusBar.text = UploadDownload_StatusBarUploadingText;

			//grab the project's unique name
			var projName = window.external.Solution.Projects(i).UniqueName;
			//add "clean" to make directory different from current directory
			var tempLocation = window.external.Solution.Projects(i).FullName.substring(0,  window.external.Solution.Projects(i).FullName.lastIndexOf(dir_BackSlash)) + dir_Code_Stipping_TempDir + dir_BackSlash;
			//clean
			clean = dir_Code_Stipping_TempDir;
			//grab the project's unique name
			var outProjName = "";
			if(window.external.Solution.Projects(i).UniqueName.lastIndexOf(dir_ForwardSlash) == -1){
				outProjName = window.external.Solution.Projects(i).UniqueName.substring(window.external.Solution.Projects(i).UniqueName.lastIndexOf(dir_BackSlash)+1, window.external.Solution.Projects(i).UniqueName.length);
			}else{
				outProjName = window.external.Solution.Projects(i).UniqueName;
			}
			
			//Remove all code that has been flaged by the professor for starter projects
			var bRtn = removeFlaggedCode(projName, tempLocation, outProjName, frm.chkRemoveStudentCode.checked);
			
			//Check to see if the code removal was successful
			if(bRtn == false) {
				//set status bar text to nothing
				window.external.StatusBar.text = "";
				//set cursor back to original
				document.body.style.cursor = oldCursor;
				//display an alert that the code hiding failed
				alert(err_code_hiding_failed);
				//delete the temporary folder
				if(fso.FolderExists(tempLocation)){
					fso.GetFolder(tempLocation).Delete(true);
				}
				//exit the function
				return false;
			}
			//Upload all files associated with the project
			var rtn = uploadProject(i, tempLocation, starterSubmit);
			//Delete temp location
			try{
				if(fso.FolderExists(tempLocation)){
					fso.GetFolder(tempLocation).Delete(true);
				}
			}catch(ex2){
				//catch and do nothing if delete of temp folder fails.
			}
			//set status bar text to nothing
			window.external.StatusBar.text = "";
			//set cursor back to original
			document.body.style.cursor = oldCursor;
			if (rtn == false) {
				try {
					frm.txtCancel.value = "1";
				}
				catch(ex3) {
					//just eat it, this page doesn't have/need the txtCancel control
				}
			}
			//exit the function successfully
			return true;
		}
	}
	catch(ex)
	{
		//set status bar text to nothing
		window.external.StatusBar.text = "";
		//set cursor back to original
		document.body.style.cursor = oldCursor;
		//exit the function successfully
		alert(ex.message);
		return false;
	}
}

//This function handles the main body of uploading files and
//creating directories for a given project selected by the user.
function uploadProject(i, rootDir, starterSubmit){
	//Create Upload Folder
	var uploadFolder = frm.txtUploadLocation.value + frm.txtNewGUID.value + dir_ForwardSlash;
	//create the folder on the web server you are going to upload all of your files to.
	createFolderDAV(uploadFolder, "");
	//Start adding all project files
	initializeXML(frm.txtFilesUploadedXML);
	//Add file to total size
	addFileToTotalSize(rootDir + getFileFromDirectory(window.external.Solution.Projects(i).FullName));
	//ignore the upload limit for starter projects
	if (starterSubmit != true) {
		//Check total size
		if((g_UploadSize / g_BytesInMegaBytes) > (g_TotalAllowedSize)) {
			//set status bar text to nothing
			window.external.StatusBar.text = "";
			//set cursor back to original
			document.body.style.cursor = oldCursor;
			//display error
			alert(err_Exceeded_Max_Size + frm.txtMaxUploadSize.value + err_MB_No_More_Files_Uploaded);
			//Finalize the xml and make it a valid document
			closeXML(frm.txtFilesUploadedXML);
			//exit function with failed result
			return false;
		}
	}
	//copy project file from client to server
	copyLocalFileToServer(rootDir + getFileFromDirectory(window.external.Solution.Projects(i).FullName), uploadFolder + getFileFromDirectory(window.external.Solution.Projects(i).FullName));
	//add project file to xml.
	addFileToXml(frm.txtFilesUploadedXML, getFileFromDirectory(window.external.Solution.Projects(i).FileName), "");
	//recurse through all files and folders in the project and upload.
	var rtn = recurseFolders(window.external.Solution.Projects(i).ProjectItems, uploadFolder,  getRootPath(window.external.Solution.Projects(i).FullName), starterSubmit);
	if (rtn == false) {
		//set status bar text to nothing
		window.external.StatusBar.text = "";
		//set cursor back to original
		document.body.style.cursor = oldCursor;
		//Finalize the xml and make it a valid document
		closeXML(frm.txtFilesUploadedXML);
		//exit function with failed result
		return false;
	}
	//Finalize the xml and make it a valid document
	closeXML(frm.txtFilesUploadedXML);
}

//Retrieves the full path minus the filename and extension
function getRootPath(strFullPath){
	if(strFullPath != null && strFullPath != "" && strFullPath.length > 0){
		return strFullPath.substring(0, strFullPath.lastIndexOf(dir_BackSlash) + 1);
	}
}

//Retrieves the filename and extension from a full path
function getFileFromDirectory(strFullPath){
	if(strFullPath != null && strFullPath != "" && strFullPath.length > 0){
		return strFullPath.substring(strFullPath.lastIndexOf(dir_BackSlash) + 1,strFullPath.length);
	}
}

//This function adds the current file to be uploaded to the total size 
//of the project which is later checked against the max size determined
//by the server setting 
function addFileToTotalSize(strLocalPath) {
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	var file = fso.GetFile(strLocalPath);
	g_UploadSize += file.Size;
}

//This function handles recursing through folders of 
//a loaded project to get all files and folders it contains
function recurseFolders(ProjectItem, uploadFolder, localRootFolder, starterSubmit) {
	if(g_StopRecursing == true) {
		return false;
	}
	
	if(ProjectItem != null){
		//Check every projectitem
		for(var g=1;g<ProjectItem.Count+1;g++){
			//make sure that it has some properties (i.e. a name)
			if(ProjectItem(g).Properties != null){
				// If it's a file on disk.
				if (ProjectItem(g).Kind == vsProjectItemKindPhysicalFile) {
					//set up a try catch sequence in order to roll-back or delete any 
					//files that have already been uploaded
					try 
					{	
						var folders = getFoldersFromRoot(localRootFolder,ProjectItem(g).Properties("FullPath").Value);
						var foldersClean = "";
						if(clean != ""){
							foldersClean = clean + dir_BackSlash + folders;
							if(folders != ""){
								foldersClean += dir_BackSlash;
							}
						}else{
							foldersClean += dir_BackSlash + folders;
							if(folders.substring(folders.length-1,1) != dir_BackSlash){
								foldersClean += dir_BackSlash;
							}
						}
						//add a forward slash to the folder location
						folders += dir_ForwardSlash;

						//Add file to total size
						//OLD->alert(localRootFolder.substring(0,localRootFolder.length-1) + foldersClean + getFileFromDirectory(ProjectItem(g).Properties("FullPath").Value));
						addFileToTotalSize(localRootFolder.substring(0,localRootFolder.length-1) + foldersClean + getFileFromDirectory(ProjectItem(g).Properties("FullPath").Value));
						//ignore file size for starter projects
						if (starterSubmit != true) {
							//Check total size
							if((g_UploadSize / g_BytesInMegaBytes) > (g_TotalAllowedSize)) {
								alert(err_Exceeded_Max_Size + frm.txtMaxUploadSize.value + err_MB_No_More_Files_Uploaded);
								//Finalize the xml and make it a valid document
								closeXML(frm.txtFilesUploadedXML);
								return false;
							}
						}
						
						var rtn;
						try
						{
							//try and copy the file to the upload location
							rtn = copyLocalFileToServer(localRootFolder.substring(0,localRootFolder.length-1) + foldersClean + getFileFromDirectory(ProjectItem(g).Properties("FullPath").Value), uploadFolder + folders + getFileFromDirectory(ProjectItem(g).Properties("FullPath").Value));
						} catch (ex)
						{
							rtn = false;
						} 

						try
						{
						if(rtn == false) 
						{
							//check to see if it wasn't just that the folder is missing
							createFolderDAV(uploadFolder, folders);
							rtn = copyLocalFileToServer(localRootFolder.substring(0,localRootFolder.length-1) + foldersClean + getFileFromDirectory(ProjectItem(g).Properties("FullPath").Value), uploadFolder + folders + getFileFromDirectory(ProjectItem(g).Properties("FullPath").Value));						
						}
						} catch (ex) {}
							//check to see if the copy was successful							
						if(rtn == false) {
							//if the copy failed, throw an error describing which file
							throw err_The_Copy_of + folders + getFileFromDirectory(ProjectItem(g).Properties("FullPath").Value) + err_Failed;
						}
						//add the file to the xml listing
						addFileToXml(frm.txtFilesUploadedXML, getFileFromDirectory(ProjectItem(g).Properties("FullPath").Value), folders);

						// VB applications have some files types (like form files) whose associated resource files
						// aren't actually first-class ProjectItems, but are instead considered nested items. Try
						// to get them and recurse 
						try {
							var VBSubItems = ProjectItem(g).ProjectItems;
							if ((VBSubItems != null) && (VBSubItems != ProjectItem)) {
								if (recurseFolders(VBSubItems, uploadFolder, localRootFolder, starterSubmit) == false) {
									return false;
								}
							}
						} catch(ex) {
							// Ignore any errors getting sub-items for physical file types.
						}
					}
					catch(ex) {
						//display a messagebox that says that you are going to delete
						//any files that have already been uploaded because there was
						//an error when transfering one or more of the files
						alert(ex + err_Terminate_And_Delete);
						//delete the upload location
						deleteFolderDAV(uploadFolder);
						//set the flag to tell the function to stop recursing through 
						//the project file structure.
						g_StopRecursing = true;
						return false;
					}
				} else if ((ProjectItem(g).Kind == vsProjectItemKindVirtualFolder) ||
				           (ProjectItem(g).Kind == vsProjectItemKindPhysicalFolder))
				{
					//recurse one level deeper into the folder or code-behind
					if (recurseFolders(ProjectItem(g).ProjectItems, uploadFolder, localRootFolder, starterSubmit) == false) 
					{
						return false;
					}
				}
				
				// Fall-through and do nothing, if the project item is a type that is not one of the above types 
				// explicitly supported for copying.
			}
		}
		
		return true;
	}
}

//This function users the DAV MKCOL method to create a 
//directory in order to place uploaded file in it.
function createFolderDAV(strFolderRoot, strAdditionalFolders) {
	var Req = new ActiveXObject("Msxml2.ServerXMLHTTP");
	try
	{
		strFolderRoot = encodeURI(strFolderRoot);
		Req.open("MKCOL", strFolderRoot, false);
		Req.send();
	}
	catch(e)
	{
		// If we fail here, it is because the folder already exists.  We can continue and use this folder.
	}

	if (strAdditionalFolders != "")
	{
	  // Make all back-slashes forward-slashes to simplify the split.
	  strAdditionalFolders = strAdditionalFolders.replace(/\\/g, "/");
	
	  // Get all of the constituant directories. DAV cannot create multiple subfolders in one
	  // pass, so we have to explicitly create them ourselves.
	  var rgAllFolders = strAdditionalFolders.split("/");
	  var length = rgAllFolders.length;
	  var i = 0;
	 
	  for (i = 0; i < length; i++) {
	    if (rgAllFolders[i] != "") {
  	      strFolderRoot = strFolderRoot + rgAllFolders[i] + "/";
		  encodeURI(strFolderRoot);
              try
	      {
	          Req.open("MKCOL", strFolderRoot, false);
  	          Req.send();
              }
              catch (e)
              {
                  //folder already existed
              }
	  }
    }
  }

	return Req;
}

//This function uses the DAV DELETE method to delete a directory
//from a web location.  (used for clean-up of temp directories)
function deleteFolderDAV(strFolderLocation) {
	strFolderLocation = encodeURI(strFolderLocation);
	var Req = new ActiveXObject("Msxml2.ServerXMLHTTP");
	Req.open("DELETE", strFolderLocation, false);
	Req.send();
	return Req;
}

//This function grabs a file stream from a local file and uses the 
//PUT method to transfer it a save it to a web location via HTTP.
function copyLocalFileToServer(strFileToSend, strServerLocation) {
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	//check to make sure the file exists locally
	if(fso.FileExists(strFileToSend)) {
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
	
		var bytes = obj.GetFileReadStream(strFileToSend);
		//create a new XMLHTTP obj
		var xmlhttp = new ActiveXObject("Msxml2.ServerXMLHTTP");
		//open the connection using the PUT syntax
		strServerLocation = encodeURI(strServerLocation);
		xmlhttp.open("PUT", strServerLocation, false);
		//send all of the bytes (file)
		xmlhttp.send(bytes);


		//dispose of the opened stream to release file
		obj.DisposeFileReadStream(bytes);
		
		//check the return status
		if(xmlhttp.status >= 200 && xmlhttp.status < 300) {
			//upload was successful
			return true;
		}else
		{
			//upload failed
			return false;
		}
	}else{
		//the file did not exist locally - throw an error
		throw err_File_Does_Not_Exist;
	}
}

//This function interacts with the DTE to make a call to remove 
//code the professor has marked for removal before uploading a 
//starter project.
function removeFlaggedCode(projName, outLocation, outProjName, bRemoveFlag) {
	// Remove code flagged for removal by professor.
	var f = null;
	try { 
		f = window.external.AddIns.Item("FacultyClient.Connect").Object;
	}
	catch (e) {
		// Ignore this exception and test whether an instance of the faculty addin was created.
	}

	if( bRemoveFlag == true )
	{

		if(f == null)
		{
			// The Faculty Academic Core Tools Addin was unavailable and they requested code extraction,
			// which requires uses of the faculty toolset.
			alert(err_Addin_Not_Loaded);
			return null;
		}
		else
		{
			return f.CreateNewProject(projName, outLocation, outProjName, bRemoveFlag); 
		}
	}
	else
	{
		// Otherwise, they are a student that doesn't have the faculty toolset and are
		// submitting a project and don't need code extraction. We therefore have to
		// hand-copy the project and handle errors ourselves.
		return newDTECopyProject(projName, outLocation, outProjName);
	}
}

function newDTECopyProject(projName, outLocation, outProjName) {
  try {
    var DTE = null;
    var proj = null;

    DTE = new ActiveXObject("VisualStudio.DTE.7.1");
    proj = window.external.Solution.Projects.Item(projName);
    DTE.Solution.AddFromTemplate(proj.FileName, outLocation, outProjName, false);
    DTE.Solution.Close(false);
    DTE.Quit();
  
    return true;
  } catch (e) {
    if (DTE != null) {
      DTE.Quit();
    }
    alert(err_ProjType_Not_Supported);  // "Project type cannot be used to submit a project."
    return false;
  }
}

//***Use the following functions to build the xml***\\
function initializeXML(txtBox) {
	txtBox.value = "<NewDataSet>";
}

function addFileToXml(txtBox, FileName, PathFromRoot) {
	//Add xml node containing file path
	txtBox.value += "<UserAssignmentFiles>";
	txtBox.value += "<FileName>" + PathFromRoot + FileName + "</FileName>";
    txtBox.value += "</UserAssignmentFiles>";
}

function closeXML(txtBox) {
	//close the xml to make a valid xml doc
	txtBox.value += "</NewDataSet>";
}
