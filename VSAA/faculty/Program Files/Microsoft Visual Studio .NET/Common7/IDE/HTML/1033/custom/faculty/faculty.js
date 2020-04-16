////////////////////////////////////////////////////////////////
// Localizable strings
////////////////////////////////////////////////////////////////
var L_DefaultAllResourcesValue_HTMLText = "[Empty]";
var L_NoAvailableCourses_HTMLText = "<div style=\"font-size:70%\" id=\"CourseManagementTextDiv\">No courses available.</div>";
var L_NoResourcesListed_HTMLText = "No course links listed.";
var L_NoAssignmentsListed_HTMLText = "No assignments listed.";
var L_CurrentlyInstalledCourses_HTMLText = "Currently Installed Courses";
var L_SelectAll_HTMLText = "Select All";
var L_ClearAll_HTMLText = "Clear All";
var L_DeleteSelected_HTMLText = "Delete Selected Courses";
var L_DeleteCoursesInstructions_HTMLText = "<div id=\"FacultyDeleteCoursesInstructions\" class=\"body\" style=\"font-size:70%\">Select the courses you wish to delete and then click <b>Delete Selected Courses</b>.</div><P>";
var L_CourseAdded_HTMLText = "Course Successfully Added";
var L_ErrorNeedAddCourseFields_Text = "Enter course name and location.";
var L_ErrorPleaseSelectCourseToDelete_Text = "Select course(s) to delete.";
var L_FacultyAddinNotLoadedError_HTMLText = "Faculty Add-in is not loaded.";
var L_NoProjectOpenInWorkspaceError_HTMLText = "Open a project before adding to the assignment.";
var L_ConnectToExistingTitle_Text = "Connect to an Existing Course";
var L_ConnectToExisting_Text = "Enter existing course URL:";
var L_AssignmentWeb_HTMLText = "Assignment Start Page: ";
var L_EditCourseWebTitle_Text = "Edit Course Web Page";
var L_EditCourseWeb_Text = "Course Web Page:";
var L_CourseWeb_HTMLText = "Course Web Page: ";
var L_OpenProject_HTMLText = "Download and open the starter project for this assignment";
var L_UpdateProjectOnServer_HTMLText = "Upload a new or updated starter project for this assignment";
var L_SetStartPage_HTMLText = "Edit Assignment Start Page";
var L_CourseResources_HTMLText = "Course Links";
var L_CourseAssignments_HTMLText = "Course Assignments";
var L_AddAssignment_HTMLText = "Add Assignment to Course";
var L_AddResource_HTMLText = "Add";
var L_DeleteAssignment_HTMLText = "Delete Assignment from Course";
var L_TabNameHTMLQuoted_HTMLText = "Course%20Management"; // Be VERY careful with this! It must be URL-escaped (%20 for space, etc), and it must exactly match the name that the tab in the ..\faculty.xml is given or the function will fail. This match INCLUDES case-sensitivity!
var L_ConfirmDeleteSelected_HTMLText = "Delete the selected courses?";
var L_ConfirmDeleteAssignment_HTMLText = "Are you sure you want to delete the specified assignment?";
var L_ConfirmDeleteResource_HTMLText = "Are you sure you want to delete the specified course link?";
var L_AssignmentManagerUrl_HTMLText = "Enter the URL of your Assignment Manager Server:";
var L_ServerManagerUrl_HTMLText = "Enter the location where you store course files. This location must be accessible to students and can be either a network location, a local directory available through a Web site, or an FTP site. <span id='LocationHelp' onClick='OnLocationHelp();' class='editLinkText'>Help</span>";
var L_ExistingServerURL_HTMLText = "Enter the URL of the assignments.xml file of the course you want to manage:";
var L_ErrorMalformedManagedCoursesXML_Text = "Managedcourses.xml file is not valid.";
var L_ErrorMalformedAssignmentsXML_Text= "Course Assignments.xml file is not valid.";
var L_WorkWithAssignmentManagerCourse_Text = "Work with Assignment Manager course";
var L_ErrorUnableToRegisterAMCourse_Text = "Unable to register the Assignment Manager course. ManagedCourses.xml file is read-only or is missing.";
var L_ErrorNoCourseInformationAvailable_Text = "Course information not available.";
var L_ErrorInvalidStartupURL_Text = "Enter a valid HTTP-based URL";
var L_ErrorUnableToRegisterExistingCourse_Text = "Unable to connect to an existing course. Check URL, server status, and server permissions.";
var L_ErrorNonAlphaNumFieldName_Text = "The following characters are not valid for the name field:\n < > \" \' \\ \nPlease enter a valid name.";
var L_StatusBarAddingAssignment_Text = "Creating new assignment...";
var L_StatusBarAddingCourse_Text = "Creating new course...";
var L_StatusBarDeletingCourse_Text = "Deleting course...";
var L_StatusBarDeletingAssignment_Text = "Deleting assignment...";
var L_StatusBarUpdatingCourseAssignment_Text = "Updating course / assignment...";
var L_ErrorRequiredAddAssignmentFields_Text = "Enter an assignment name and description.";
var L_ErrorFileServerAccessError_Text = "File permissions or server access prevent updating course and assignment information.";
var L_ErrorDescriptionFieldTooLong_Text = "Enter a description of no more than 4000 characters.";


////////////////////////////////////////////////////////////////
// Globals
////////////////////////////////////////////////////////////////
// This is the count of the number of resources that have been added to a 'new course'.
var g_nAddCourseResources = 0;

// This is the context attribute that may have been pushed by any given
// page. Since it only truly has relevance on a per-web-page basis, it
// should be removed from the window's attributes whenever the page is
// left.
var g_Attrib = null; 

////////////////////////////////////////////////////////////////
// Code
////////////////////////////////////////////////////////////////

function facultyMOver(oTgt){
	oTgt.style.color = "#FF0000";
}

function facultyMOut(oTgt){
	oTgt.style.color = "";
}

// Helper function used internally to load the WorkWith page
// associated with the Assignment Manager version of a course.
// It is called either from the CourseManagement tab or the
// WorkWith page.
function FacultyLoadAssignmentManagerCourse(strURL, strGUID) {
  strURL += "/Faculty/WorkWithCourse.aspx?CourseID=" + strGUID;
  try {
    window.external.ItemOperations.Navigate(strURL, 1); //EnvDTE.vsNavigateOptions.vsNavigateOptionsNewWindow
  } catch (e) {
    // The error will display in the IE window.
  }
}

// Called every time that the deletecourse.html page is loaded
// or a refresh is otherwise requested of it.
function FacultyRefreshDeleteCoursesPage() {
  var oXML = null;
  
  try {
    g_Attrib = window.external.ActiveWindow.ContextAttributes.Add("keyword","vs.vshomepage.get.coursemanagement.deletecourse",3); // Type '3' is an F1 keyword
  } catch (helpFailed) {
  }

  try {
    oXML = GetManagedCoursesFile();
    
    if (oXML == null) {
      // No managedcourses.xml file; no courses.
      FacultyDeleteCoursesTab.innerHTML = L_NoAvailableCourses_HTMLText;
      return;
    }
  
    FacultyPopulateDeleteCoursesTab(oXML);
  } catch (e) {
    alert(e.description);
  }
}

// Returns an XML DOMDocument object; null on failure. Note that in places, 
// some of this functionality is duplicated because of the need for the code
// to again 'save' the file back out after making a change (which requires the
// path that we go to some length to dynamically choose in a safe way).
function GetManagedCoursesFile() {
  var oXML = new ActiveXObject("MSXML2.DOMDocument.3.0");
  var oWSHShell = new ActiveXObject("WScript.Shell");
  var sAppDataPath = oWSHShell.RegRead("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders\\AppData");
  var sFolderName = sAppDataPath + "\\Microsoft\\VisualStudio\\7.0\\Academic\\"; // The directory containing the files.
  var sAssignmentsFile = sFolderName + "managedcourses.xml";
  oXML.async = false;
  oXML.preserveWhiteSpace = true;
  oXML.setProperty("SelectionLanguage", "XPath");
  var oFacultyTools = GetFacultyObject();

  if (!oXML.load(sAssignmentsFile)) {
    return null;
  } else {
    if (!oFacultyTools.IsConformantXmlFile(sAssignmentsFile)) {
      throw new Error(0, L_ErrorMalformedManagedCoursesXML_Text);
    }
    return oXML;
  }
}

// Helper function that is used solely within the context of the
// deletecourse.html file. On that tab, there is a <div> tag
// with an ID of FacultyDeleteCoursesTab, which this function
// populates with information about the courses (both Assignment
// Manager and Standard).
function FacultyPopulateDeleteCoursesTab(oXML) {
  var oSelection = oXML.selectNodes("/managedcourses/course");
  var oItem;
  var oNode;
  var i = 0;
  var strCourseName = "";
  var strCourseDescription = "";
  var strCourseLocation = "";
  var strInsert = "";
  var strCourseGUID = "";

  while ((oItem = oSelection.nextNode()) != null) {
    i++;
     
    strCourseName = oItem.selectSingleNode("name").text;
    if ((oNode = oItem.selectSingleNode("noassnmgr")) != null) {
      // It's a normal academic tools course, and should be rendered to use
      // this stuff.
      strCourseLocation = oNode.selectSingleNode("netassns").text;
      strCourseGUID="";
    } else if ((oNode = oItem.selectSingleNode("assnmgr")) != null) {
      // It's an assignment manager-compatible course, and we should browse there
      // to continue working with the course.
      strCourseLocation = oNode.selectSingleNode("amurl").text;
      strCourseGUID = oNode.selectSingleNode("guid").text;
    }
  
    strInsert += "<tr><td class=\"tableItem" + ((i-1)%2) + "\" id=\"DeleteTableItem" + i + "\" ><input tabindex=\"1\" type=\"checkbox\" id=\"DeleteCourseCheckBox" + i + "\" value=\"\"><b><label for=\"DeleteCourseCheckBox" + i + "\" id=\"DeleteCourseLabel" + i + "\">" +
    strCourseName + "</label></b><br> <span style='position:relative; left:30;' id=\"DeleteCourseLocation" + i + "\"><i>" +
    strCourseLocation + "</i></span><span style='display = none' id='DeleteCourseGUID" + i + "'>" + strCourseGUID + "</span></td></tr>";
  }
  
  if (strInsert != "") {
    strInsert = L_DeleteCoursesInstructions_HTMLText + "<table id=\"deleteTable\"  class=\"tableStyle\"  width=\"400\" cellspacing=\"0\" cellpadding=\"4\" border=\"0\"><tr><td class=\"columnHead\">" + L_CurrentlyInstalledCourses_HTMLText + "</td></tr>" +
    strInsert +
    "<tr><td class=\"columnFoot\" align=\"right\" valign=\"bottom\"><button tabindex=\"1\" class=\"webButton\" onClick=\"SelectAllForDelete(" + i + 
    ")\" name=\"deleteBTN\" id='deleteBTN'>" + L_SelectAll_HTMLText +
    "</button>&nbsp;<button tabindex=\"1\" class=\"webButton\" onClick=\"ClearAllForDelete(" + i + 
    ")\" name=\"CancelButton\" id='clearBTN'>" + L_ClearAll_HTMLText +
    "</button>&nbsp;<button tabindex=\"1\" class=\"webButton\" onClick=\"FacultyDeleteSelectedCourses(" + i + 
    ")\" id=\"OKButton\" value=\"Delete Selected\">" + L_DeleteSelected_HTMLText +
    "</button></td></tr></table>";
   
    FacultyDeleteCoursesTab.innerHTML = strInsert;
  } else {
    // No course entries in managedcourses.xml.
    FacultyDeleteCoursesTab.innerHTML = L_NoAvailableCourses_HTMLText;
  }
}

// Called whenever the user hits OK to connect to an existing
// course, whether it is an Assignment Manager or Standard course.
function FacultyConnectToExisting()
{
  try {
    var oFacultyTools = GetFacultyObject();
    var oURL = showGenericDialog(L_ConnectToExistingTitle_Text, L_ConnectToExisting_Text, "", "vs.vshomepage.get.coursemanagement.addcourse.addexisting");
    
    if (oURL != null) {
      if (!oFacultyTools.RegisterCourseFromUrl(oURL)) {
        alert(L_ErrorUnableToRegisterExistingCourse_Text);
      } else {
        FacultyLoadWorkWithCourses(null);
      }
    }
  } catch (e) {
    alert(e.description);
  }
}

// Called whenever the user hits OK to add a new course, whether it is
// an Assignment Manager or Standard course.
function FacultyAddNewCourse()
{
  var oFacultyTools = null;
  var descriptions = null;
  var urls = null;
  var i = 0;

  // It is intentional that this code allocates an array of size zero when
  // there are no resources -- if we do not, the COM Interop marshaller will
  // not allow us to cleanly handle the situation.
  descriptions = new Array(g_nAddCourseResources);
  urls = new Array(g_nAddCourseResources);
    
  for (i = 0; i < g_nAddCourseResources; i++) {
    eval("descriptions[" + i + "] = ResourceLinkTag" + i + ".innerText;");
    eval("urls[" + i + "] = ResourceLinkTag" + i + ".href;");
  }
  
  if ((serverPath.value == "") ||
      (courseName.value == "")) {
    alert(L_ErrorNeedAddCourseFields_Text);
    return;
  }

  if (!ValidateEntry(courseName.value)) {
    alert(L_ErrorNonAlphaNumFieldName_Text);
    return;
  }
  
  if (newCourseDesc.value != null && newCourseDesc.value.length > 4000) {
    alert(L_ErrorDescriptionFieldTooLong_Text);
    return;
  }
  
  try {
    oFacultyTools = GetFacultyObject();
    if (useAssnMan.checked) {
      var strGUID = oFacultyTools.GenerateGUID();
      var strPath = serverPath.value;

      if (!oFacultyTools.RegisterAssignmentManagerCourse(courseName.value, strPath, strGUID)) {
        alert(L_ErrorUnableToRegisterAMCourse_Text);
        return;
      }

      // Modify the path for use by Assignment Manager
      strPath += "/Faculty/AddCourse.aspx?CourseID=" + strGUID + "&CourseName=" + URLEscape(courseName.value);
      try {
        window.external.ItemOperations.Navigate(strPath, 1); //EnvDTE.vsNavigateOptions.vsNavigateOptionsNewWindow
      } catch (e) {
        // The error will display in the IE window.
      }
      
      FacultyRefreshAddCoursesPage();
    } else {
      window.external.StatusBar.Text = L_StatusBarAddingCourse_Text;

      if (courseWeb.value == "http://") {
        // The user didn't change it, and it isn't very likely that they want
        // to actually keep it this way...
        courseWeb.value = "";
      }
      if (oFacultyTools.AddCourse(serverPath.value, courseName.value,
                                 (newCourseDesc.value == "")?"":newCourseDesc.value,
                                 (courseWeb.value == "")?"":courseWeb.value, 
                                 JScriptArrayToSafeArray(descriptions), JScriptArrayToSafeArray(urls))) {
        FacultyRefreshAddCoursesPage();
        alert(L_CourseAdded_HTMLText);
      } else {
        alert(L_ErrorFileServerAccessError_Text);
      }
    }
  } catch (e) {
    alert(e.description);
  }
  window.external.StatusBar.Text = "";
}

// Called every time that the addcourse.html page is loaded
// or a refresh is otherwise requested of it.
function FacultyRefreshAddCoursesPage() 
{
  try {
    g_Attrib = window.external.ActiveWindow.ContextAttributes.Add("keyword","vs.vshomepage.get.coursemanagement.addcourse",3); // Type '3' is an F1 keyword
  } catch (helpFailed) {
  }

  courseName.value = "";
  serverPath.value = "";
  newCourseDesc.value = "";
  courseWeb.value = "http://";
  allResources.innerHTML = L_DefaultAllResourcesValue_HTMLText;
  resourceFilesList.style.display = "none";
  addRescArrow.src = "images/Arrow_closed_Dark.gif";
  g_nAddCourseResources = 0;

  CourseNameField.style.display = "inline";
  UseAssignmentManagerField.style.display = "inline";

  useAssnMan.checked = false;
  toggleCheckAssnMan();

  OkButton.onclick = FacultyAddNewCourse;
}

// When the user unloads a page, we want to remove the F1 keywords that have been
// pushed.
function UnloadProvidedAttribute() {
  if (g_Attrib != null) {
    try {
      g_Attrib.Remove();
    } catch (helpFailed) {
    }
    g_Attrib= null;
  }
}

// This is an exception-safe function used to retrieve the FacultyTools'
// extensibility object from the IDE. Note that the cases in which it will
// return null include not only when the addin has failed to load, but also
// the case where the capitalization of the registry key that loads the
// AddIn has changed!
function GetFacultyObject()
{
  var obj = null;
  try {
    obj = window.external.AddIns.Item("Microsoft.VisualStudio.Academic.FacultyTools.VS7AddIn").Object;
  } catch(e) {
    throw new Error(0, L_FacultyAddinNotLoadedError_HTMLText);
  }

  if (obj == null) {
    throw new Error(0, L_FacultyAddinNotLoadedError_HTMLText);
  }
  
  return obj;
}

// Called every time that the workwith.html page is loaded
// or a refresh is otherwise requested of it.
function FacultyRefreshWorkWithCoursesPage() {
  var oXML = null;
  var oWSHShell = null;
  var sAppDataPath = null;
  var sFolderName = null;
  var sAssignmentsFile = null;
  
  try {
    g_Attrib = window.external.ActiveWindow.ContextAttributes.Add("keyword","vs.vshomepage.get.coursemanagement.workwithcourse",3); // Type '3' is an F1 keyword
  } catch (helpFailed) {
  }

  oXML = new ActiveXObject("MSXML2.DOMDocument.3.0");
  oWSHShell = new ActiveXObject("WScript.Shell");
  sAppDataPath = oWSHShell.RegRead("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders\\AppData");
  sFolderName = sAppDataPath + "\\Microsoft\\VisualStudio\\7.0\\Academic\\"; // The directory containing the files.
  sAssignmentsFile = sFolderName + "managedcourses.xml";
  oXML.async = false;
  oXML.preserveWhiteSpace = true;
     
  try {
    var oFacultyTools = GetFacultyObject();

    if (!oXML.load(sAssignmentsFile)) {
      // No managedcourses.xml file; no courses.
      WorkWithCoursesTab.innerHTML = L_NoAvailableCourses_HTMLText;
      return;
    }
     
    if (!oFacultyTools.IsConformantXmlFile(sAssignmentsFile)) {
      throw new Error(0, L_ErrorMalformedManagedCoursesXML_Text);
    }

    oXML.setProperty("SelectionLanguage", "XPath");

    FacultyPopulateWorkWithCoursesTab(oXML, sFolderName);
  } catch (e) {
    window.alert(e.description);
  }
}

// Helper function that is used solely within the context of the
// workwith.html file. On that tab, there is a <div> tag
// with an ID of WorkWithCoursesTab, which this function
// populates with information about the courses (both Assignment
// Manager and Standard).
function FacultyPopulateWorkWithCoursesTab(oXML, sLocalFilePath) {
  var oSelection = oXML.selectNodes("/managedcourses/course");
  var oAssignmentsXML = new ActiveXObject("MSXML2.DOMDocument.3.0");
  var oItem;
  var oNodeValue;
  var oAssignments;
  var oAssignment;
  var oResources;
  var oResource;
  var bLoaded;
  var i = 0;
  var j = 0;
  var r = 0;
  var strCourseName = "";
  var strCourseDescription = "";
  var strCourseLongDescription = "";
  var strCourseWeb = "";
  var strCourseLinks = "";
  var strAssignmentName = "";
  var strAssignmentDescription = "";
  var strAssignmentStartup = "";
  var strAssignmentsXMLNetLocation = "";
  var strAssignmentsXMLLocalFile = "";
  var strSafeAssignmentsXMLNetFileName = "";
  var strResourceURL = "";
  var strResourceDescription = "";
  var strAssignments = "";
  var strResources = "";
  var oFacultyTools = GetFacultyObject();
  var oAMNode;
  var strCourseURL = "";
  var strCourseGUID = "";
  var fUpdateManagedCoursesXML = 0;
  var index = 0;
     
  if (oSelection == null) {
    return;
  }

  oAssignmentsXML.async = false;
  oAssignmentsXML.preserveWhiteSpace = true;
    
  WorkWithCoursesTab.innerHTML = "";

  while ((oItem = oSelection.nextNode()) != null) {
    i++; // Number of courses.
    j = 0; // Number of assignments in each course.
    r = 0; // Number of resources in each course.

    // First, check the course type. If it's an Assignment Manager
    // course, we have to do simply add a small amount of text
    // denoting that fact and providing them with a link to it. Otherwise,
    // we have to show all of the assignments, etc.
    oAMNode = oItem.selectSingleNode("assnmgr");
    if (oAMNode != null) {
      strCourseName = oItem.selectSingleNode("name").text;
      strCourseURL = oAMNode.selectSingleNode("amurl").text;
      strCourseGUID = oAMNode.selectSingleNode("guid").text;

      WorkWithCoursesTab.innerHTML +=
      "<br><table class=\"tableStyle\" width=\"100%\" cellspacing=\"0\" cellpadding=\"8\"><tr><td class=\"workWithCourseHead\"> <span class=\"courseName\" id='courseName" + i + "'>" + strCourseName + "</span></td></tr>" +
      "<tr><td class= \"workWithCourseBody\"><div class=\"AppHostDescriptionText\"><span tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" class='editLinkText' id='navigateToAMCourseDiv" + i + "' onFocus='facultyMOver(this)' onBlur='facultyMOut(this)' onmouseOver='facultyMOver(this)' onmouseOut='facultyMOut(this)' onclick='FacultyLoadAssignmentManagerCourse(\"" +
      FacultyEscapePathString(strCourseURL) + "\", \"" + strCourseGUID + "\")'>" + L_WorkWithAssignmentManagerCourse_Text +
      "</span>&nbsp;</div></td></tr></table><BR>";
    } else {
      // Get the network-path location of the assignment. We need
      // this later, whether we use it here or not.
      oNodeValue = oItem.selectSingleNode("noassnmgr/netassns");
      if ((oNodeValue != null) && (oNodeValue.text != "")) {
        strAssignmentsXMLNetLocation = oNodeValue.text;
      }

      strAssignmentsXMLLocalFile = oFacultyTools.RetrieveRemoteXmlDocFile(strAssignmentsXMLNetLocation, true);
      if ((strAssignmentsXMLLocalFile == null) ||
          (!oFacultyTools.IsConformantXmlFile(strAssignmentsXMLLocalFile)) ||
          (!oAssignmentsXML.load(strAssignmentsXMLLocalFile))) {
        // Either the file couldn't be loaded from the online source, or
        // the file was invalid.
        strCourseName = oItem.selectSingleNode("name").text;
        
        WorkWithCoursesTab.innerHTML +=
        "<br><table class=\"tableStyle\" width=\"100%\" cellspacing=\"0\" cellpadding=\"8\"><tr><td class=\"workWithCourseHead\"> <span class=\"courseName\" id='courseName"
        + i + "'>" + strCourseName + "</span>" +
        "<div >" + strAssignmentsXMLNetLocation + "</div></td></tr>" +
        "<tr><td class=\"workWithCourseBody\"><div><span id='courseErrorMessage" + i + "'>" +
        L_ErrorNoCourseInformationAvailable_Text +
        "</span>&nbsp;</div></td></tr></table><BR>";
        continue;
      }

      strSafeAssignmentsXMLNetFileName = FacultyEscapePathString(strAssignmentsXMLNetLocation);

      oAssignmentsXML.setProperty("SelectionLanguage", "XPath");

      // When retrieving the course's name and description, also update the local
      // copies in managedcourses.xml, if possible.
      strCourseName = oAssignmentsXML.selectSingleNode("course/name").text;
      oItem.selectSingleNode("name").text = strCourseName;
      strCourseDescription = oAssignmentsXML.selectSingleNode("course/noassnmgr/description").text;
      fUpdateManagedCoursesXML = 1;

      // Put the default courseweb into the start of the resources section
      strCourseWeb = oAssignmentsXML.selectSingleNode("course/noassnmgr/courseweb").text;
      strResources = "<div class='intentText'><span id='courseWeb" + i + "'>" + L_CourseWeb_HTMLText + "<a tabindex=\"1\" id='courseWebHref" + i + "' href=" + strCourseWeb + ">" +
      strCourseWeb + " </a></span>&nbsp;<img src='images/edit_ICON.gif' style='cursor:hand;' tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" id='editCourseWebSpan" + i +
      "' onClick=\"editCourseWeb('" + strSafeAssignmentsXMLNetFileName + "', '" + EscapeSingleQuote(strCourseWeb) +
      "', " + i + ");\"></img><BR></div>";

      strCourseLinks = "";
      oResources = oAssignmentsXML.selectNodes("/course/noassnmgr/resource");
      if (oResources != null) {
        while ((oResource = oResources.nextNode()) != null) {
          r++;
          strResourceURL = oResource.selectSingleNode("url").text;
          strResourceDescription = oResource.selectSingleNode("description").text;
          
          strCourseLinks += "<li><a id='resourceHref" + i + "a" + r + "' href=\"" + strResourceURL +
          "\">" + strResourceDescription + "</a> " +
          "<img src='images/edit_ICON.gif' style='cursor:hand;' tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" id='editResourceSpan" + i + "a" + r + "' onclick=\"editResource('" + strSafeAssignmentsXMLNetFileName + "', '" +
          EscapeSingleQuote(strResourceDescription) + "', '" +
          EscapeSingleQuote(strResourceURL) + "', " + i + ")\"></img>" +
          " <img src='images/delete_ICON.gif' style='cursor:hand;' tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" id='deleteResourceSpan" + i + "a" + r + "' onclick=\"deleteResource('" + strSafeAssignmentsXMLNetFileName + "', '" +
          strResourceDescription + "', " + i + ")\"></img></li>";
        }
      }

      if (strCourseLinks != "") {
        strResources += "<div class='intentText'>" + strCourseLinks + "</div>";
      }

      strAssignments = "";
      oAssignments = oAssignmentsXML.selectNodes("/course/assignment");
      if (oAssignments != null) {
        while ((oAssignment = oAssignments.nextNode()) != null) {
          j++;
          strAssignmentName = oAssignment.selectSingleNode("name").text;
          strAssignmentDescription = oAssignment.selectSingleNode("description").text;
          strAssignmentStartup = oAssignment.selectSingleNode("startup").text;
          
          strAssignments += 
          "<div><span class=\"courseName\" style='cursor:hand;' tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" id='showHideAssignmentSpan" + i + "' onClick=\"showHideAssn(ass" + i + "A" + j + ", course" + i + "A" + j + "Arrow )\" style=\"cursor:hand;\"><img src=\"images/Arrow_open_Dark.gif\" style='cursor:hand;' alt=\"\" border=\"0\" id=\"course" + i + "A" + j + "Arrow\">&nbsp;" +
          strAssignmentName + 
          "</span>&nbsp;<img src='images/edit_ICON.gif' style='cursor:hand;' tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" id='EditAssignmentNameSpan"+ i + "A" + j + "' onClick=\"editAssnName('" + strSafeAssignmentsXMLNetFileName + "', '" + EscapeSingleQuote(strAssignmentName) +
          "', " + i + ");\"></img><BR><div id=\"ass" + i + "A" + j + "\" style=\"position:relative; left:30; display:inline;\">" +
          strAssignmentDescription +
          "&nbsp;&nbsp;<img src='images/edit_ICON.gif' style='cursor:hand;' tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" id='EditAssignmentDescSpan"+ i + "A" + j + "' onClick=\"editAssnDesc('" + strSafeAssignmentsXMLNetFileName + "', '" + FacultyEscapePathString(strAssignmentsXMLLocalFile) + "', '" + EscapeSingleQuote(strAssignmentName) + "', null, " + i + ");\"></img><BR>" +
          "<div id=\"assnStart" + i + "A" + j + "\" style=\"display:inline;\">" +
          L_AssignmentWeb_HTMLText + " <a id='assStartup" + i + "A" + j + "' href='" + strAssignmentStartup + "'>" + strAssignmentStartup + 
          "</a> <img src='images/edit_ICON.gif' style='cursor:hand;' tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" id='editAssignmentStartPageSpan"+ i + "A" + j + "' onClick=\"setStartPage('" +
          strSafeAssignmentsXMLNetFileName + "', '" +
          EscapeSingleQuote(strAssignmentName) + "', '" + EscapeSingleQuote(strAssignmentStartup) +
          "', " + i + ")\" </img></div><br><br>" +
          "<span tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" class=\"editLinkText\" onFocus='facultyMOver(this)' onBlur='facultyMOut(this)' onmouseOver='facultyMOver(this)' onmouseOut='facultyMOut(this)' id='openProjectInVSSpan"+ i + "A" + j +
          "' onClick=\"openProjectInVS('" + strSafeAssignmentsXMLNetFileName + "', '" +
          EscapeSingleQuote(strAssignmentName) + "');\">" + L_OpenProject_HTMLText + "</span>" +
          "<br><span tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" class=\"editLinkText\" onFocus='facultyMOver(this)' onBlur='facultyMOut(this)' onmouseOver='facultyMOver(this)' onmouseOut='facultyMOut(this)' id='updateProjectOnServerSpan"+ i + "A" + j +
          "' onClick=\"updateProjServer('" + strSafeAssignmentsXMLNetFileName + "', '" + 
          EscapeSingleQuote(strAssignmentName) + "', " + i + ")\">" + L_UpdateProjectOnServer_HTMLText + "</span>" +
          "<br><span tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" class=\"editLinkText\" onFocus='facultyMOver(this)' onBlur='facultyMOut(this)' onmouseOver='facultyMOver(this)' onmouseOut='facultyMOut(this)' id='DeleteAssignmentFromCourseSpan" + i + "A" + j + "'onClick=\"delAsnFromCourse('" + strSafeAssignmentsXMLNetFileName + "', '" +
          EscapeSingleQuote(strAssignmentName) + "', " + i + ")\">" + L_DeleteAssignment_HTMLText + "</span></div></div><p>";
        }
      }

      if (strAssignments == "") {
        strAssignments = L_NoAssignmentsListed_HTMLText;
      }

      try {

      WorkWithCoursesTab.innerHTML +=
      "<table class=\"tableStyle\" width=\"100%\" cellspacing=\"0\" cellpadding=\"8\"><tr><td class=\"workWithCourseHead\"> <span class=\"courseName\" id='courseName" + i + "'>" + strCourseName + "</span>&nbsp;<img src='images/edit_ICON.gif' style='cursor:hand;' tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" id='editCourseNameSpan" + i + "'onClick=\"editCourseName('" +
      strSafeAssignmentsXMLNetFileName + "', '" + EscapeSingleQuote(strCourseName) + "', " + i +
      ");\" style=\"font-size:70%;\"></img><br><span class=\"body\" id='courseAssignmentLocation" + i + "'>" + strAssignmentsXMLNetLocation + "</span></div></td></tr>" +
      "<tr><td class=\"workWithCourseBody\"><div class=\"AppHostDescriptionText\"><span id='courseDesc" + i + "'>" + strCourseDescription + "</span>&nbsp;&nbsp;<img src='images/edit_ICON.gif' style='cursor:hand;' tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" class=\"editDescLink\" id='editCourseDescSpan" + i +
      "'onClick=\"editCourseDesc('" + strSafeAssignmentsXMLNetFileName + "', '" + FacultyEscapePathString(strAssignmentsXMLLocalFile) +
      "', null, " + i + ");\"></img><BR><span tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" class=\"editLinkText\" onFocus='facultyMOver(this)' onBlur='facultyMOut(this)' onmouseOver='facultyMOver(this)' onmouseOut='facultyMOut(this)' id='AddAssignmentToCourseSpan" + i +
      "'onClick=\"addAsnToCourse('" + strSafeAssignmentsXMLNetFileName + "', " + i + ")\">" + L_AddAssignment_HTMLText + "</span><BR><br>" +
      "<span id='ShowHideResourcesDiv" + i + "'onClick=\"AppHostDescText(course" + i + "Resources, resources" + i + ", course" + i +
      "ResArrow);\"><img src=\"images/Arrow_closed_Dark.gif\" style='cursor:hand;' alt=\"\" border=\"0\" id=\"course" + i +
      "ResArrow\">&nbsp;<span tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" class=\"courseName\" style='cursor:hand;' id=\"resources" + i + "\">" + L_CourseResources_HTMLText +
      "</span></span>&nbsp;<span tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" class=\"editLinkText\" onFocus='facultyMOver(this)' onBlur='facultyMOut(this)' onmouseOver='facultyMOver(this)' onmouseOut='facultyMOut(this)' id='AddResourceToCourseSpan" + i +
      "'onClick=\"addResToCourse('" + strSafeAssignmentsXMLNetFileName +
      "', " + i + ")\">" + L_AddResource_HTMLText + "</span><div class=\"hiddenText\" id=\"course" +
      i + "Resources\" style=\"display:none;\">" +
      strResources +
      "</div><p><div id='ShowHideAssignmentsDiv" + i + "'onClick=\"AppHostDescText(course" + i + "AHiddenText, short" + i + "A, course" + i +
      "DesArrow);\"><img src=\"images/Arrow_closed_Dark.gif\" style='cursor:hand;' alt=\"strDescri\" border=\"0\" id=\"course" + i +
      "DesArrow\">&nbsp;<span tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" class=\"courseName\" style='cursor:hand;' id=\"short" + i +
      "A\">" + L_CourseAssignments_HTMLText +
      "</span></div><div class=\"hiddenText\" id=\"course" + i + "AHiddenText\" style=\"position:relative; left:30; display:none;\">" +
      strAssignments + "</div></div></div></td></tr></table><br>";
      } catch (e) {
        alert(strCourseDescription + e.description);
      }
    }
  }

  try {
    // If the user passed an argument to go to a certain course, then expand
    // the course. Note that we do NOT scroll, as that could cause the menu
    // from the top to go off the screen.
    index = GetCourseIndex();
    if ((index != 0) && (index <= i)) {
      eval("AppHostDescText(course" + index + "Resources, resources" + index + ", course" + index + "ResArrow);");
      eval("AppHostDescText(course" + index + "AHiddenText, short" + index + "A, course" + index + "DesArrow);");
    }
  } catch (ignore) {
  }
  
  try {
    // If we have updated any of the data associated with the XML file,
    // save it now that we're done loading.
    if (fUpdateManagedCoursesXML == 1) {
      oXML.save(sLocalFilePath + "managedcourses.xml");
      oFacultyTools.RegenerateUserCustomTab(false);
    }
  } catch(ignore) {
  }

  if (i == 0) {
    // There was a course file, but there were no courses.
    WorkWithCoursesTab.innerHTML = L_NoAvailableCourses_HTMLText;
  }    
}

function editAssnName(fileName, assignmentName, index) {
  var L_EditAssignmentNameTitle_Text = "Edit Assignment Name";
  var L_EditAssignmentName_Text = "Assignment Name:";
  var result = null;

  try {
    result = showGenericDialog(L_EditAssignmentNameTitle_Text, L_EditAssignmentName_Text, assignmentName, "vs.vshomepage.get.coursemanagement.workwithcourse.editassignmentname");

    if (result != null) {
      if (!ValidateEntry(result)) {
        alert(L_ErrorNonAlphaNumFieldName_Text);
        editAssnName(fileName, assignmentName, index);
        return;
      }
      
      var oFacultyTools = GetFacultyObject();
      window.external.StatusBar.Text = L_StatusBarUpdatingCourseAssignment_Text;
      if (oFacultyTools.ModifyAssignmentProperties(fileName, assignmentName,
                                                   result, "", "")) {
        FacultyLoadWorkWithCourses(index);
      } else {
        alert(L_ErrorFileServerAccessError_Text);
      }
    }
  } catch (e) {
    window.alert(e.description);
  }
  window.external.StatusBar.Text = "";
}

function editAssnDesc(fileName, localFileName, assignmentName, oldDescription, index) {
  var L_EditAssignmentDescriptionTitle_Text = "Edit Assignment Description";
  var L_EditAssignmentDescription_Text = "Assignment Description:";
  var oldDescription = null;
  var result = null;

  try {
    if (oldDescription == null) {
      var oXML = new ActiveXObject("MSXML2.DOMDocument.3.0");
      oXML.async = false;
      oXML.preserveWhiteSpace = true;
      // No need to check for schema-validity here, as the file was already checked during WorkWith.
      if (!oXML.load(localFileName)) {
        return;
      }
      oXML.setProperty("SelectionLanguage", "XPath");
      oldDescription = oXML.selectSingleNode("/course/assignment[name='" + assignmentName + "']/description").text;
    }
    
    result = showGenericBigEditDialog(L_EditAssignmentDescriptionTitle_Text, L_EditAssignmentDescription_Text, oldDescription, "vs.vshomepage.get.coursemanagement.workwithcourse.editassignmentdescription");

    if (result != null) {
      var oFacultyTools = GetFacultyObject();
      window.external.StatusBar.Text = L_StatusBarUpdatingCourseAssignment_Text;
      if (oFacultyTools.ModifyAssignmentProperties(fileName, assignmentName,
                                                   "", result, "")) {
        FacultyLoadWorkWithCourses(index);
      } else {
        alert(L_ErrorFileServerAccessError_Text);
      }
    }
  } catch (e) {
    window.alert(e.description);
  }
  window.external.StatusBar.Text = "";
}

function setStartPage(fileName, assignmentName, oldStartPage, index) {
  var L_EditAssignmentStartPageTitle_Text = "Edit Assignment Start Page";
  var L_EditAssignmentStartPage_Text = "Assignment Start Page:";
  var result = null;

  try {
    result = showGenericDialog(L_EditAssignmentStartPageTitle_Text, L_EditAssignmentStartPage_Text, oldStartPage, "vs.vshomepage.get.coursemanagement.workwithcourse.editassignmentweb");

    if (result != null) {
      // The startup file may only be either an HTTP:// or HTTPS:// file
      // URL, since those are what the environment knows how to easily
      // navigate to.
      if (result != "") {
        if (result.search(/^https?:\/\//i) == -1) {
          alert(L_ErrorInvalidStartupURL_Text);
          setStartPage(fileName, assignmentName, oldStartPage, index);
          return;
        }
      }
      
      var oFacultyTools = GetFacultyObject();
      window.external.StatusBar.Text = L_StatusBarUpdatingCourseAssignment_Text;
      if (oFacultyTools.ModifyAssignmentProperties(fileName, assignmentName,
                                                   "", "", result)) {
        FacultyLoadWorkWithCourses(index);
      } else {
        alert(L_ErrorFileServerAccessError_Text);
      }
    }
  } catch (e) {
    window.alert(e.description);
  }
  window.external.StatusBar.Text = "";
}

function editCourseName(fileName, oldCourseName, index) {
  var L_EditCourseNameTitle_Text = "Edit Course Name";
  var L_EditCourseName_Text = "Course Name:";
  var result = null;

  try {
    result = showGenericDialog(L_EditCourseNameTitle_Text, L_EditCourseName_Text, oldCourseName, "vs.vshomepage.get.coursemanagement.workwithcourse.editcoursename");

    if (result != null) {
      if (!ValidateEntry(result)) {
        alert(L_ErrorNonAlphaNumFieldName_Text);
        editCourseName(fileName, oldCourseName, index);
        return;
      }

      var oFacultyTools = GetFacultyObject();
      window.external.StatusBar.Text = L_StatusBarUpdatingCourseAssignment_Text;
      if (oFacultyTools.ModifyCourse(fileName, result, "", "")) {
        FacultyLoadWorkWithCourses(index);
      } else {
        alert(L_ErrorFileServerAccessError_Text);
      }
    }
  } catch (e) {
    window.alert(e.description);
  }
  window.external.StatusBar.Text = "";
}

function editCourseDesc(fileName, localFileName, oldCourseDescription, index) {
  var L_EditCourseDescriptionTitle_Text = "Edit Course Description";
  var L_EditCourseDescription_Text = "Course Description:";
  var result = null;

  try {
    if (oldCourseDescription == null) {
      var oXML = new ActiveXObject("MSXML2.DOMDocument.3.0");
      oXML.async = false;
      oXML.preserveWhiteSpace = true;
      // No need to check for schema-validity here, as the file was already checked during WorkWith.
      if (!oXML.load(localFileName)) {
        return;
      }
      oXML.setProperty("SelectionLanguage", "XPath");
      oldCourseDescription = oXML.selectSingleNode("/course/noassnmgr/description").text;
    }

    result = showGenericBigEditDialog(L_EditCourseDescriptionTitle_Text, L_EditCourseDescription_Text, oldCourseDescription, "vs.vshomepage.get.coursemanagement.workwithcourse.editcoursedescription");

    if (result != null) {
      var oFacultyTools = GetFacultyObject();
      window.external.StatusBar.Text = L_StatusBarUpdatingCourseAssignment_Text;
      if (oFacultyTools.ModifyCourse(fileName, "", result, "")) {
        FacultyLoadWorkWithCourses(index);
      } else {
        alert(L_ErrorFileServerAccessError_Text)
      }
    }
  } catch (e) {
    window.alert(e.description);
  }
  window.external.StatusBar.Text = "";
}

function editCourseWeb(fileName, oldCourseWeb, index) {
  var result = null;

  // If the user has not specified a course web page, then
  // we pre-populate the edit field with some reasonable text
  // for them.
  if ((oldCourseWeb == null) || (oldCourseWeb == "")) {
    oldCourseWeb = "http://";
  }
  
  try {
    result = showGenericDialog(L_EditCourseWebTitle_Text, L_EditCourseWeb_Text, oldCourseWeb, "vs.vshomepage.get.coursemanagement.workwithcourse.editcourseweb");

    if (result != null) {
      // The course web site may only be either an HTTP:// or HTTPS:// URL.
      if (result != "") {
        if (result.search(/^https?:\/\//i) == -1) {
          alert(L_ErrorInvalidStartupURL_Text);
          editCourseWeb(fileName, oldCourseWeb, index);
          return;
        }
        if (result == "http://") {
          // Since we pre-populate with http://, if the user didn't bother
          // to change it from that, then we don't bother to update the course.
          // Note that this does *not* properly handle the case where the user
          // changes the startup URL from "http://booga" to "http://", and will
          // instead leave the URL as "http://booga".
          return;
        }
      }

      var oFacultyTools = GetFacultyObject();
      window.external.StatusBar.Text = L_StatusBarUpdatingCourseAssignment_Text;
      if (oFacultyTools.ModifyCourse(fileName, "", "", result)) {
        FacultyLoadWorkWithCourses(index);
      } else {
        alert(L_ErrorFileServerAccessError_Text);
      }
    }
  } catch (e) {
    window.alert(e.description);
  }
  window.external.StatusBar.Text = "";
}

function openProjectInVS(fileName, assignmentName) {
  try {
      var oFacultyTools = GetFacultyObject();
      oFacultyTools.DownloadStarterProject(fileName, assignmentName);
  } catch (e) {
    window.alert(e.description);
  }
}

function updateProjServer(fileName, assignmentName, index) {
  var newAssignmentInfo;
  var nEntries;
  var strSolutionName;

  nEntries = GetRealProjectCount();

  // Find the number of projects in the solution. If it's none, then we don't want
  // to pop up the dialog.
  if (nEntries == 0) {
    alert(L_NoProjectOpenInWorkspaceError_HTMLText);
    return;
  }

  // Pop up the Edit Assignment form to get the user's input.
  newAssignmentInfo = showModalDialog("editAssignment.html", new Array(window.external, fileName), "dialogWidth:486px;dialogHeight:225px;center:yes;status:no;scroll:no");
  if (newAssignmentInfo != null) {
    try {
      // User hit OK; perform add.
      var oFacultyTools = GetFacultyObject();
      var courseFile = newAssignmentInfo[0];
      var uniqueProjectName = newAssignmentInfo[1];
      var performExtraction = newAssignmentInfo[2];

      window.external.StatusBar.Text = L_StatusBarUpdatingCourseAssignment_Text;
      if (oFacultyTools.ModifyAssignmentProject(courseFile, assignmentName,
                                                uniqueProjectName, (performExtraction?true:false))) {
        FacultyLoadWorkWithCourses(index);
      } else {
        alert(L_ErrorFileServerAccessError_Text);
      }
    } catch (e) {
      alert(e.description);
    }
    window.external.StatusBar.Text = "";
  }
}

// Handles the display of a simple DHTML dialog capable of displaying
// a custom title, field name, and old value for the field to edit.
function showGenericDialog(title, fieldName, oldFieldValue, helpTopic) {
  return showModalDialog("genericOneFieldEdit.html",
                         new Array(window.external, helpTopic, title, fieldName, oldFieldValue),
                         "dialogWidth:417px; dialogHeight:175px; center:yes; status:no; scroll:no");
}

// Handles the display of a simple DHTML dialog capable of displaying
// a custom title, field name, and old value for the field to edit. The
// field to edit in this one is a textarea, providing multi-line support.
function showGenericBigEditDialog(title, fieldName, oldFieldValue, helpTopic) {
  return showModalDialog("genericOneFieldEditBig.html",
                         new Array(window.external, helpTopic, title, fieldName, oldFieldValue),
                         "dialogWidth:580px; dialogHeight:455px; center:yes;status:no;scroll:no");
}

function showAddResourceDialog(argument) {
  return showModalDialog("addResources.html", argument, "dialogWidth:413px; dialogHeight:280px;  center:yes; status:no;scroll:no");
}

// Calls straight through to the FacultyTools object's method
// to remove an assignment.
function delAsnFromCourse(assignmentsXMLName, assignmentName, index) {
  try {
    var oFacultyTools = GetFacultyObject();

    if (confirm(L_ConfirmDeleteAssignment_HTMLText)) {
      window.external.StatusBar.Text = L_StatusBarDeletingAssignment_Text;
      if (oFacultyTools.RemoveAssignment(assignmentsXMLName, assignmentName)) {
        FacultyLoadWorkWithCourses(index);
      } else {
        alert(L_ErrorFileServerAccessError_Text);
      }
    }
  } catch (e) {
    alert(e.description);
  }
  window.external.StatusBar.Text = "";
}

// Simply browses back to the VS start page. This function is used
// as the target of an OnClick even of several <span> tags.
function FacultyLoadHomePage()
{
  window.navigate("vs://default.htm?tab=" + L_TabNameHTMLQuoted_HTMLText);
}

// Simply browses back to the addcourse page. This function is used
// as the target of an OnClick even of several <span> tags.
function FacultyLoadAddCourses()
{
  window.location = "vs://custom/faculty/addcourse.html";
}

// Simply browses back to the deletecourse page. This function is used
// as the target of an OnClick even of several <span> tags.
function FacultyLoadDeleteCourses()
{
  window.location = "vs://custom/faculty/deletecourse.html";
}

// Simply browses back to the workwithcourse page. This function is used
// as the target of an OnClick even of several <span> tags. Note that there
// is an optional paramater that specifies which course is being selected
// in order to draw the user directly to that course. This is the one-based
// index of the course in the local managedcourses.xml file.
function FacultyLoadWorkWithCourses(courseName)
{
  var path = "vs://custom/faculty/workwith.html";

  if (courseName != null) {
    path += "?course=" + courseName;
  }
  
  window.location = path;
}

// By looking at the arguments passed to the current page, this function
// determines the current, one-based course index. Zero indicates that no
// course was detected.
function GetCourseIndex() {
  try {
    var temp, length;
    var compoundArgs = document.location.toString().split("?"); // split it into { URL , ALL-ARGUMENTS }
    compoundArgs = compoundArgs[1].split("&"); // split ALL-ARGUMENTS into { ARG, ARG, ARG, ... }

    length = compoundArgs.length;
    for (var i = 0; i < length; i++) {
      temp = compoundArgs[i].split("="); // split each ARG into { VARIABLE, VALUE } pairs

      // If the variable is course, then the value is what should be returned
      // to the caller. 
      if (temp[0] == "course") {
        return temp[1];
      }
    }
  } catch(e) {
  }
  
  return 0;
}

// Handler function for the 'Delete selected courses' button. It
// simply iterates over all of the selected courses and performs
// the proper action depending on whether the course is an
// Assignment Manager course or a standard course.
function FacultyDeleteSelectedCourses(n) {
  var DeleteBoxChecked = null;
  var CourseLocation = null;
  var CourseGUID= null;
  var oFacultyTools = null;
  var oManagedCourses = null;
  var oCourseNode = null;
  var DeletedACourse = false;
  var query = "";
  var x = null;
  var nCourses = 0;

  for (var i = 1; i <= n; i++) {
    eval("DeleteBoxChecked = DeleteCourseCheckBox" + i + ".checked");
    if (DeleteBoxChecked) {
      ++nCourses;
    }
  }

  if (nCourses == 0) {
    alert(L_ErrorPleaseSelectCourseToDelete_Text);
    return;
  }

  x = confirm(L_ConfirmDeleteSelected_HTMLText);

  try {
    if (x) {
      for (var i = 1; i <= n; i++) {
        eval("DeleteBoxChecked = DeleteCourseCheckBox" + i + ".checked");
        if (DeleteBoxChecked) {
          eval("CourseLocation = DeleteCourseLocation" + i + ".innerText");
          eval("CourseGUID = DeleteCourseGUID" + i + ".innerText");
          if (oManagedCourses == null) {
            oManagedCourses = GetManagedCoursesFile();
          }
          if (oFacultyTools == null) {
            oFacultyTools = GetFacultyObject();
          }

          if ((CourseGUID != "") && (oCourseNode = oManagedCourses.selectSingleNode("/managedcourses/course/assnmgr[guid='" + CourseGUID + "']")) != null) {
            DeletedACourse = oFacultyTools.UnregisterAssignmentManagerCourse(CourseGUID) || DeletedACourse;

            // Modify the path for use by Assignment Manager
            CourseLocation += "/Faculty/DeleteCourse.aspx?CourseID=" + CourseGUID;
            
            try {
              window.external.ItemOperations.Navigate(CourseLocation, 1); //EnvDTE.vsNavigateOptions.vsNavigateOptionsNewWindow
            } catch (e) {
              // The error will display in the IE window.
            }
          } else {
            window.external.StatusBar.Text = L_StatusBarDeletingCourse_Text;
            DeletedACourse = oFacultyTools.RemoveCourse(CourseLocation) || DeletedACourse;
          }
        }
      }
 
      if (DeletedACourse) {
        FacultyRefreshDeleteCoursesPage();
      } else {
        alert(L_ErrorFileServerAccessError_Text);
      }
    }
  } catch (e) {
    alert(e.description);
  }
  window.external.StatusBar.Text = "";
}

// Tied to the 'select all' button's handler. Simply sets all
// of the checkboxes to 'checked'.
function SelectAllForDelete(n) {
  var i;
  for (i = 1; i <= n; i++) {
    eval("DeleteCourseCheckBox" + i + ".checked = true;");
  }
}
   
// Tied to the 'clear all' button's handler. Simply sets all
// of the checkboxes to 'unchecked'.
function ClearAllForDelete(n) {
  var i;
  for (i = 1; i <= n; i++) {
    eval("DeleteCourseCheckBox" + i + ".checked = false;");
  }
}

// This simply replaces all single-back-slashes with double-slashes. It doesn't do
// anything intelligent about detecting double-back-slashes and ignoring them -- they'll
// turn into quadruples.
function FacultyEscapePathString(path) {
  var i;
  var l = path.length;
  var s = "";
  var c;
 
  for (i = 0; i < l; i++ ) {
    c = path.charAt(i);
    if (c == '\\') {
      s += '\\';
    }
   
    s += c;
  }
 
  return s;
}

// When putting strings within function calls in JScript, they cannot contain any
// embedded quotes, or it will pre-terminate the string and cause parse-errors.
function EscapeSingleQuote(str) {
  var i;
  var l = str.length;
  var s = "";
  var c;
 
  for (i = 0; i < l; i++ ) {
    c = str.charAt(i);
    if (c == '\'') {
      s += '\\';
    }
   
    s += c;
  }
 
  return s;
}

// Takes a string as input and generates a valid output string for
// transmission within the query field of a URL as follows:
// - All characters with ASCII values less than 32 decimal (0x20 hex) 
// or greater than 127 decimal (0x7F hex) are escaped by converting 
// them into a hexadecimal ASCII code (%##)
// - Any characters from the set: !"#$%&'()+,/:;<=>?[\]^`{|}~
// - DELETE (don't need to worry about, as it can't appear in a text box!)
// - SPACE. Spaces are escaped by replacing them with pluses (+).
function URLEscape(input) {
  var i;
  var l = input.length;
  var s = "";
  var c;
 
  for (i = 0; i < l; i++ ) {
    c = input.charAt(i);

    switch (c) {
    case ' ':
      s += "+";
      break;
    case '!':
      s += "%21";
      break;
    case '\"': 
      s += "%22";
      break;
    case '#':
      s += "%23";
      break;
    case '$':
      s += "%24";
      break;
    case '%':
      s += "%25";
      break;
    case '&':
      s += "%26";
      break;
    case '\'':
      s += "%27";
      break;
    case '(':
      s += "%28";
      break;
    case ')':
      s += "%29";
      break;
    case '+':
      s += "%2B";
      break;
    case ',':
      s += "%2C";
      break;
    case '/':
      s += "%2D";
      break;
    case ':':
      s += "%3A";
      break;
    case ';':
      s += "%3B";
      break;
    case '<':
      s += "%3C";
      break;
    case '=':
      s += "%3D";
      break;
    case '>':
      s += "%3E";
      break;
    case '?':
      s += "%3F";
      break;
    case '[':
      s += "%5B";
      break;
    case '\\':
      s += "%5C";
      break;
    case ']':
      s += "%5D";
      break;
    case '^':
      s += "%5E";
      break;
    case '`':
      s += "%60";
      break;
    case '{':
      s += "%7B";
      break;
    case '|':
      s += "%7C";
      break;
    case '}':
      s += "%7D";
      break;
    case '~':
      s += "%7E";
      break;
    default:
      s += c;
    }
  }
 
  return s;
  
}

// This function adds an operator to the array class for use
// by the conversion code that's written in VB script. Note 
// that this hackery is necessary because JScript arrays
// aren't compatible with the COM idea of arrays, which
// makes them difficulty to pass through the .NET interop
// layer.
function Array.prototype.item(index)
{
  return this[index];
}

// Called when the user attempts to add an assignment to a course. Before displaying
// any UI, it makes sure that the user has a valid project open (where valid project
// is defined as any project that isn't the built-in Miscellanous Files or Solution
// Files projects).
function addAsnToCourse(courseFileName, index) {
  var newAssignmentInfo;
  var nEntries;
  var strSolutionName;

  nEntries = GetRealProjectCount();
  // Find the number of projects in the solution. If it's none, then we don't want
  // to pop up the dialog.
  if (nEntries == 0) {
    alert(L_NoProjectOpenInWorkspaceError_HTMLText);
    return;
  }

  // Pop up the Add Assignment form to get the user's input.
  newAssignmentInfo = showModalDialog("addAssignment.html", new Array(window.external, courseFileName), "dialogWidth:486px; dialogHeight:525px;  center:yes; status:no;scroll:no");
  if (newAssignmentInfo != null) {
    // User hit OK; perform add.
    var oFacultyTools = GetFacultyObject();
    
    try {
      var courseFile = newAssignmentInfo[0];
      var assignmentName = newAssignmentInfo[1];
      var assignmentDescription = newAssignmentInfo[2];
      var relativeOutPath = newAssignmentInfo[3];
      var startupFile = newAssignmentInfo[4];
      var uniqueProjectName = newAssignmentInfo[5];
      var performExtraction = newAssignmentInfo[6];

      window.external.StatusBar.Text = L_StatusBarAddingAssignment_Text;
      if (oFacultyTools.AddAssignment(newAssignmentInfo[0], newAssignmentInfo[1], newAssignmentInfo[2],
                                     newAssignmentInfo[3], newAssignmentInfo[4], newAssignmentInfo[5], 
                                     (newAssignmentInfo[6]?true:false))) {
        FacultyLoadWorkWithCourses(index);
      } else {
        alert(L_ErrorFileServerAccessError_Text);
      }
    } catch (e) {
      alert(e.description);
    }
    window.external.StatusBar.Text = "";
  }
}

function GetRealProjectCount() {
  var nEntries;
  var i;
  
  nEntries = window.external.Solution.Count;
  for (i = 1; i <= nEntries; i++) {
    strSolutionName = window.external.Solution.Item(i).Name;
    if ((strSolutionName == "Miscellaneous Files") || (strSolutionName == "Solution Items")) {
      nEntries--;
    }
  }

  return nEntries;
}

// Shows a the dialog to add resources to a new course. It simply adds elements
// directly to the HTML within the 'resources' tab and increments a global counter
// used by the handler for the Add button (so that it can loop through all of 
// the created resources).
function addResources()	{
  var newResNameAndLink = showAddResourceDialog(new Array(window.external));
  if (newResNameAndLink != null) {
    // User hit OK; add to list.
    if (!ValidateEntry(newResNameAndLink[0])) {
      alert(L_ErrorNonAlphaNumFieldName_Text);
      addResources();
      return;
    }
    
    if (allResources.innerHTML == L_DefaultAllResourcesValue_HTMLText) {
      allResources.innerHTML = "";
      resourceFilesList.style.display = "inline";
      addRescArrow.src = "images/Arrow_open_Dark.gif";
    }

    allResources.innerHTML += "<li><a tabindex=\"1\" id=\"ResourceLinkTag" +g_nAddCourseResources + "\" href=\"" + newResNameAndLink[1] + "\">" + newResNameAndLink[0] + "</a>";
    ++g_nAddCourseResources;
  }
}

// Shows a the dialog to add resources to a new course, then
// calls the FacultyTools addin to modify the course properties.
function addResToCourse(coursePath, index) {
  try {
    var newResNameAndLink = showAddResourceDialog(new Array(window.external));
    if (newResNameAndLink != null) {
      if (!ValidateEntry(newResNameAndLink[0])) {
        alert(L_ErrorNonAlphaNumFieldName_Text);
        addResToCourse(coursePath, index);
        return;
      }
      
      var oFacultyTools = GetFacultyObject();
      window.external.StatusBar.Text = L_StatusBarUpdatingCourseAssignment_Text;
      if (oFacultyTools.AddCourseResource(coursePath, newResNameAndLink[0], newResNameAndLink[1])) {
        FacultyLoadWorkWithCourses(index);
      } else {
        alert(L_ErrorFileServerAccessError_Text);
      }
    }
  } catch (e) {
    alert(e.description);
  }
  window.external.StatusBar.Text = "";
}

function editResource(coursePath, oldResourceDesc, oldResourceUrl, index) {
  try {
    var newResNameAndLink = showAddResourceDialog(new Array(window.external, oldResourceDesc, oldResourceUrl));
    if (newResNameAndLink != null) {
      if (!ValidateEntry(newResNameAndLink[0])) {
        alert(L_ErrorNonAlphaNumFieldName_Text);
        editResource(coursePath, oldResourceDesc, oldResourceUrl, index);
        return;
      }

      var oFacultyTools = GetFacultyObject();
      window.external.StatusBar.Text = L_StatusBarUpdatingCourseAssignment_Text;
      if (oFacultyTools.EditCourseResource(coursePath, oldResourceDesc, newResNameAndLink[0], newResNameAndLink[1])) {
        FacultyLoadWorkWithCourses(index);
      } else {
        alert(L_ErrorFileServerAccessError_Text);
      }
    }
  } catch (e) {
    alert(e.description);
  }
  window.external.StatusBar.Text = "";
}

function deleteResource(coursePath, resourceDesc, index) {
  try {
    var oFacultyTools = GetFacultyObject();
    if (confirm(L_ConfirmDeleteResource_HTMLText)) {
      window.external.StatusBar.Text = L_StatusBarUpdatingCourseAssignment_Text;
      if (oFacultyTools.RemoveCourseResource(coursePath, resourceDesc)) {
        FacultyLoadWorkWithCourses(index);
      } else {
        alert(L_ErrorFileServerAccessError_Text);
      }
    }
  } catch (e) {
    alert(e.description);
  }
  window.external.StatusBar.Text = "";
}

function OnLocationHelp() {
  try {
    var help = window.external.GetObject("Help");
    help.DisplayTopicFromURL("ms-help://ms.acadt/acafaculty/html/vstskCreatingNewCourse.htm");
  } catch(e) {
    // falling in here simply means that the topic did not exist -- the
    // VS shell will provide an appropriate error topic to the user.
  }
}

// Whenever the user hits enter, we want to trigger the OK button on the form;
// when they hit esc, we want to trigger the cancel button.
function HandleKeyDown() {
  var i = window.event.keyCode;

  switch (i) {
  case 13: // ENTER
    OkButton.click();
    break;
  case 27: // ESC
    CancelButton.click();
    break;
  default:
    break;
  }
}

// When they hit esc, we want to trigger the cancel button. Some controls
// already handle enter themselves, and we probably don't want to override
// their behavior (such as textareas).
function HandleKeyDownNoEnter() {
  var i = window.event.keyCode;

  switch (i) {
  case 27: // ESC
    CancelButton.click();
    break;
  default:
    break;
  }
}

// If there are any of the <, >, ", \, or '
// characters in the string, we mark it as an invalid entry.
function ValidateEntry(str) {
  return ((str == "") ||
          (str.search(/[\u0027\u0022<>\\]/) == -1));
}

// Because SPAN, IMG, and DIV tags are not normally accessible, we
// must handle the user's press of the enter key explicitly.
function AccKeyPressHandler(span) {
  if (window.event.keyCode == 13) {
    span.click();
  }
}

////////////////////////////////////////////////////////////////
// The following functions were provided by the design team and
// are used to modify visual effects based on user actions on
// the page.
////////////////////////////////////////////////////////////////
function overInnerTabBar(obj)	{
  obj.style.color="#FE9901";
}

function outInnerTabBar(obj)	{
  obj.style.color = "#FFFFFF";
}

function toggleCheckAssnMan() {
  var i = 0;
  if(useAssnMan.checked) {
    for (i = 3; i < 8; i++) {
      eval("hideMe" + i + ".style.display = \"none\";");
    }
    serLocText.innerText = L_AssignmentManagerUrl_HTMLText;
  }else{
    for (i = 3; i < 8; i++) {
      eval("hideMe" + i + ".style.display = \"inline\";");
    }
    serLocText.innerHTML = L_ServerManagerUrl_HTMLText;
  }
}

function showHideAssn(whichAssn, whichArrow ) {
  if (whichAssn.style.display == "inline") {
    whichAssn.style.display = "none";
    whichArrow.src = "images/Arrow_closed_Dark.gif";
  }else{
    whichAssn.style.display = "inline";
    whichArrow.src = "images/Arrow_open_Dark.gif";
  }
}

function AppHostDescText(whichTarget, obj, img) {
  var x = whichTarget.style.display;
  if (x == "none") {
    whichTarget.style.display = "inline";
    img.src = "images/Arrow_open_Dark.gif";
  }else{
    whichTarget.style.display = "none";
    img.src = "images/Arrow_closed_Dark.gif";
  }
}

function AppHostCourseDesc(whichTarget, obj, img) {
  var x = whichTarget.style.display;
  if (x == "none") {
    whichTarget.style.display = "inline";
    obj.style.pixelLeft = "40";
    img.src = "images/Arrow_open_Dark.gif";
  }else{
    whichTarget.style.display = "none";
    obj.style.pixelLeft = "10";
    img.src = "images/Arrow_closed_Dark.gif";
  }
}
