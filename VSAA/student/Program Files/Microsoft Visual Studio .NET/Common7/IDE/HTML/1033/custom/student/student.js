////////////////////////////////////////////////////////////////
// Localizable strings
////////////////////////////////////////////////////////////////
var L_NoAvailableCourses_HTMLText = "<div class=\"bodyText\" style=\"font-size:70%\" id=\"MyCoursesTextDiv\">No courses available.</div>";
var L_CoursesAvailableHeader_HTMLText = "<div class=\"bodyText\" style=\"font-size:70%\" id=\"MyCoursesTextDiv\">You currently have the following courses installed on your system:</div><p>";
var L_CourseWeb_HTMLText = "Course Web Page: ";
var L_NoAssignmentsListed_HTMLText = "No assignments listed.";
var L_ErrorNeedToEnterURL_Text = "Enter a course URL.";
var L_ErrorPleaseSelectCourseToDelete_Text = "Select course(s) to delete.";
var L_AddinNotLoadedError_HTMLText = "Student Add-in is not loaded.";
var L_UnableToLoadInfo_HTMLText = "Unable to load course information from server or local copy.";
var L_CurrentlyInstalledCourses_HTMLText = "Currently Installed Courses";
var L_SelectAll_HTMLText = "Select All";
var L_ClearAll_HTMLText = "Clear All";
var L_DeleteSelected_HTMLText = "Delete Selected Courses";
var L_DeleteCoursesInstructions_HTMLText = "<div id=\"StudentDeleteCoursesInstructions\" class=\"body\" style=\"font-size:70%\">Select the courses you wish to delete and then click <b>Delete Selected Courses</b>.</div><P>";
var L_EnterURLPrompt_HTMLText = "<enter URL>";
var L_DownloadStarterProject_HTMLText = "Download Starter Project";
var L_TabNameHTMLQuoted_HTMLText = "Student%20Course%20Tools"; // Be VERY careful with this! It must be URL-escaped (%20 for space, etc), and it must exactly match the name that the tab in the ..\mycourses.xml is given or the function will fail. This match INCLUDES case-sensitivity!
var L_ConfirmDeleteSelected_HTMLText = "Are you sure you want to delete the selected courses?";
var L_FullCourseDescription_HTMLText = "Course Description";
var L_CourseResources_HTMLText = "Course Links";
var L_CourseAssignments_HTMLText = "Course Assignments";
var L_WorkWithAssignmentManagerCourse_Text = "Work with Assignment Manager course";
var L_ErrorMalformedCoursesXML_Text = "Course file is not valid.";
var L_ErrorMalformedAssignmentsXML_Text= "Course assignment file is not valid. Check with instructor.";
var L_ErrorUnableToBrowseToAMCourse_Text = "Unable to browse to Assignment Manager course page";
var L_ErrorNoCourseInformationAvailable_Text = "Course information is not available. Check with instructor.";
var L_ErrorUsingCachedInformation_Text = "(from local cache)";
var L_ErrorAddCourse_Text = "Unable to add course.";
var L_ErrorDeleteCourse_Text = "Unable to delete course(s).";

////////////////////////////////////////////////////////////////
// Globals
////////////////////////////////////////////////////////////////
// This is the context attribute that may have been pushed by any given
// page. Since it only truly has relevance on a per-web-page basis, it
// should be removed from the window's attributes whenever the page is
// left.
var g_Attrib = null; 
var g_strLocalCourseSubdir = "Courses\\";

////////////////////////////////////////////////////////////////
// Code
////////////////////////////////////////////////////////////////

// This is an exception-safe function used to retrieve the StudentTools'
// extensibility object from the IDE. Note that the cases in which it will
// return null include not only when the addin has failed to load, but also
// the case where the capitalization of the registry key that loads the
// AddIn has changed!
function GetStudentObject()
{
  var obj = null;
  try {
    obj = window.external.AddIns.Item("Microsoft.VisualStudio.Academic.StudentTools.VS7AddIn").Object;
  } catch(e) {
    throw new Error(0, L_AddinNotLoadedError_HTMLText);
  }

  if (obj == null) {
    throw new Error(0, L_AddinNotLoadedError_HTMLText);
  }
  
  return obj;
}

function studentMOver(oTgt){
	oTgt.style.color = "#FF0000";
}

function studentMOut(oTgt){
	oTgt.style.color = "";
}

// Helper function used internally to load the WorkWith page
// associated with the Assignment Manager version of a course.
// It is called either from the MyCourses tab or the
// WorkWith page.
function LoadAssignmentManagerCourse(strURL, strGUID) {
  strURL += "/Student/WorkWithCourse.aspx?CourseID=" + strGUID;
  try {
    window.external.ItemOperations.Navigate(strURL, 1); //EnvDTE.vsNavigateOptions.vsNavigateOptionsNewWindow
  } catch (e) {
    alert(L_ErrorUnableToBrowseToAMCourse_Text);
  }
}

// Called every time that the deletecourse.html page is loaded
// or a refresh is otherwise requested of it.
function RefreshDeleteCoursesPage() {
  var oXML = null;

  try {
    g_Attrib = window.external.ActiveWindow.ContextAttributes.Add("keyword","vs.vshomepage.get.mycourses.deletecourse",3); // Type '3' is an F1 keyword
  } catch (helpFailed) {
  }
  
  oXML = GetStudentCoursesFile();
  if (oXML == null) {
    // No courses.xml file; no courses.
    DeleteCoursesTab.innerHTML = L_NoAvailableCourses_HTMLText;
    return;
  }
     
  PopulateDeleteCoursesTab(oXML);
}

// Helper function that is used solely within the context of the
// deletecourse.html file. On that tab, there is a <div> tag
// with an ID of DeleteCoursesTab, which this function
// populates with information about the courses (both Assignment
// Manager and Standard).
function PopulateDeleteCoursesTab(oXML) {
  var oSelection = oXML.selectNodes("/studentcourses/course");
  var oItem;
  var i = 0;
  var strCourseName = "";
  var strInsert = "";
    
  while ((oItem = oSelection.nextNode()) != null) {
    i++;
     
    strCourseName = oItem.selectSingleNode("name").text;
  
    // The name of the checkbox, DeleteCourseCheckBox, is used by the DeleteSelectedCourses function. Do not change it
    // here unless you also change it there! The same is true of DeleteCourseLabel.
    strInsert += "<tr><td class=\"tableItem" + ((i-1)%2) + "\" id=\"DeleteTableItem" + i + "\" ><input tabindex=\"1\" type=\"checkbox\" id=\"DeleteCourseCheckBox" + i + "\" value=\"\"><label for=\"DeleteCourseCheckBox" + i + "\" id=\"DeleteCourseLabel" + i + "\">" + strCourseName + "</label></td></tr>";
  }
        
  if (strInsert != "") {
    strInsert = L_DeleteCoursesInstructions_HTMLText + "<table id=\"deleteTable\"  class=\"tableStyle\"  width=\"400\" cellspacing=\"0\" cellpadding=\"4\" border=\"0\" ><tr><td class=\"columnHead\">" + L_CurrentlyInstalledCourses_HTMLText + "</td></tr>" + strInsert + "<tr><td class=\"columnFoot\" align=\"right\"><button tabindex=\"1\" class=\"webButton\" onClick=\"SelectAllForDelete(" + i + 
  ")\" id=\"selectallBTN\">" + L_SelectAll_HTMLText + "</button>&nbsp;<button tabindex=\"1\" class=\"webButton\" onClick=\"ClearAllForDelete(" + i + 
  ")\" id=\"clearallBTN\">" + L_ClearAll_HTMLText + "</button>&nbsp;<button tabindex=\"1\" class=\"webButton\" onClick=\"DeleteSelectedCourses(" + i + 
  ")\" id=\"deleteBTN\">" + L_DeleteSelected_HTMLText + "</button></td></tr></table>";
    DeleteCoursesTab.innerHTML = strInsert;
  } else {
    // There was a courses file, but no courses were available.
    DeleteCoursesTab.innerHTML = L_NoAvailableCourses_HTMLText;
  }
}

// Called whenever the user clicks on the 'Go' button on the AddCourse
// page.
function GetNewCourse()
{
  var oStudentTools = null;
  var oCourseFile = null;
  var fAMCourse = false;
  var strURLInput = URLinput.value;
  var reEndsWithXML = /\.xml$/i;

  if ((strURLInput == "") || (strURLInput == L_EnterURLPrompt_HTMLText)) {
    alert(L_ErrorNeedToEnterURL_Text);
    return;
  }

  try {
    oStudentTools = GetStudentObject();
    
    // To handle the AM course case, our general procedure is as
    // follows: attempt to load the page, assuming nothing about it and
    // *not* allowing to attempt for authentication. If we get it, check
    // for AM-ness. If it is, add an entry for it and browse to the AM
    // page. If it isn't (or if we couldn't load the file), pass it through
    // to the NewCourse() method, which may attempt to query the user for
    // authentication.
    if (strURLInput.search(reEndsWithXML) != -1) {
      oCourseFile = oStudentTools.RetrieveRemoteXmlDocFile(strURLInput, true);
    }

    if (oCourseFile != null) {
      if (!oStudentTools.IsConformantXmlFile(oCourseFile)) {
        throw new Error(0, L_ErrorMalformedAssignmentsXML_Text);
      }

      // It's possibly an AM course -- return if it is, after loading it.
      var oXML = new ActiveXObject("MSXML2.DOMDocument.3.0");
      oXML.async = false;
      oXML.preserveWhiteSpace = true;
      oXML.setProperty("SelectionLanguage", "XPath");
      var oNode = null;
      var strCourseName = "";
      var strURL = "";
      var strGUID = "";

      if (oXML.load(oCourseFile)) {
        oNode = oXML.selectSingleNode("/course/assnmgr");
        
        if (oNode != null) { // Is it an Assignment Manager course?
          strCourseName = oXML.selectSingleNode("/course/name").text;
          strURL = oNode.selectSingleNode("amurl").text;
          strGUID = oNode.selectSingleNode("guid").text;

          if (oStudentTools.RegisterAssignmentManagerCourse(strCourseName, strURL, strGUID, URLinput.value)) {
            RefreshAddCoursesPage();

            strURL += "/Student/AddCourse.aspx?CourseID=" + strGUID;
            try {
              window.external.ItemOperations.Navigate(strURL, 1); //EnvDTE.vsNavigateOptions.vsNavigateOptionsNewWindow
            } catch (e) {
              alert(L_ErrorUnableToBrowseToAMCourse_Text);
            }
            
            return;
          }
        }
      }
    }
    if (oStudentTools.NewCourse(strURLInput)) {
      LoadWorkWithCourses(null)
    } else {
      alert(L_ErrorAddCourse_Text);
    }
  } catch (e) {
    window.alert(e.description);
  }
}

// Called every time that the addcourse.html page is loaded
// or a refresh is otherwise requested of it.
function RefreshAddCoursesPage() 
{
  try {
    g_Attrib = window.external.ActiveWindow.ContextAttributes.Add("keyword","vs.vshomepage.get.mycourses.addcourse",3); // Type '3' is an F1 keyword
  } catch (helpFailed) {
  }

  try {
  URLinput.value = L_EnterURLPrompt_HTMLText;
  } catch (e) {
    window.alert(e.description);
  }
}

// Called whenever the user left-mouse-button-clicks on the
// text field. If the default text is in there, we reset
// it to blank.
function AddCourseURLFieldOnClick()
{
  if (URLinput.value == L_EnterURLPrompt_HTMLText) {
    URLinput.value = "";
  }
}

// Returns an XML DOMDocument object; null on failure. Note that in places, 
// some of this functionality is duplicated because of the need for the code
// to again 'save' the file back out after making a change (which requires the
// path that we go to some length to dynamically choose in a safe way).
function GetStudentCoursesFile() {
  var oXML = new ActiveXObject("MSXML2.DOMDocument.3.0");
  // Try : HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\ShellFolders.AppData -> C:\Documents and Settings\larsberg\Application Data
  var oWSHShell = new ActiveXObject("WScript.Shell");
  var sAppDataPath = oWSHShell.RegRead("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders\\AppData");
  var sFolderName = sAppDataPath + "\\Microsoft\\VisualStudio\\7.0\\Academic\\"; // The directory containing the files.
  var sAssignmentsFile = sFolderName + "courses.xml";
  var oStudentTools = GetStudentObject();
  
  oXML.async = false;
  oXML.preserveWhiteSpace = true;
  oXML.setProperty("SelectionLanguage", "XPath");
  
  if (!oXML.load(sAssignmentsFile)) {
    return null;
  } else {
    if (!oStudentTools.IsConformantXmlFile(sAssignmentsFile)) {
      throw new Error(0, L_ErrorMalformedCoursesXML_Text);
    }
    return oXML;
  }
}

// Called every time that the workwith.html page is loaded
// or a refresh is otherwise requested of it.
function RefreshWorkWithCoursesPage() {
  var oXML = null;
  var oWSHShell = null;
  var sAppDataPath = null;
  var sFolderName = null;

  try {
    g_Attrib = window.external.ActiveWindow.ContextAttributes.Add("keyword","vs.vshomepage.get.mycourses.workwithcourse",3); // Type '3' is an F1 keyword
  } catch (helpFailed) {
  }

  oWSHShell = new ActiveXObject("WScript.Shell");
  sAppDataPath = oWSHShell.RegRead("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders\\AppData");
  sFolderName = sAppDataPath + "\\Microsoft\\VisualStudio\\7.0\\Academic\\"; // The directory containing the files.
  
  oXML = GetStudentCoursesFile();
  if (oXML == null) {
    // No courses.xml file; no courses.
    WorkWithCoursesTab.innerHTML = L_NoAvailableCourses_HTMLText;
    return;
  }
     
  try {
    PopulateWorkWithCoursesTab(oXML, sFolderName);
  } catch (e) {
    window.alert(e.description);
  }
}

// Helper function that is used solely within the context of the
// workwith.html file. On that tab, there is a <div> tag
// with an ID of WorkWithCoursesTab, which this function
// populates with information about the courses (both Assignment
// Manager and Standard).
function PopulateWorkWithCoursesTab(oXML, sLocalFilePath) {
  var oSelection = oXML.selectNodes("/studentcourses/course");
  var oAssignmentsXML = new ActiveXObject("MSXML2.DOMDocument.3.0");
  var oItem;
  var oAssignments;
  var oAssignment;
  var oResources;
  var oResource;
  var oAMNode = null;
  var bLoaded = false;
  var bFromCache = false;
  var i = 0;
  var j = 0;
  var strCourseName = "";
  var strCourseDescription = "";
  var strCourseLongDescription = "";
  var strCourseURL = "";
  var strCourseGUID = "";
  var strCourseWeb = "";
  var strAssignmentsXMLNetLocation = "";
  var strAssignmentsXMLLocalFile = "";
  var strAssignmentsXMLCacheFileName = "";
  var strAssignments = "";
  var strResources = "";
  var strPrintCourseName = "";
  var oStudentTools = GetStudentObject();
  var fUpdateCoursesXML = 0;
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

    // First, check the course type. If it's an Assignment Manager
    // course, we have to do simply add a small amount of text
    // denoting that fact and providing them with a link to it. Otherwise,
    // we have to show all of the assignments, etc.
    oAMNode = oItem.selectSingleNode("assnmgr");
    if (oAMNode != null) {
      strCourseName = oItem.selectSingleNode("name").text;
      strCourseURL = oAMNode.selectSingleNode("amurl").text;
      strCourseGUID = oAMNode.selectSingleNode("guid").text;
      var strAMLocalFile = "";

      // This block of code attempts to get a new version of the XML document
      // for the Assignment Manager course. If it can, then it will check for
      // any updated course information and save it back into the local file
      // (courses.xml).
      strAMLocalFile = oStudentTools.RetrieveRemoteXmlDocFile(oAMNode.selectSingleNode("xmlpath").text, true);
      if (strAMLocalFile != null) {
        var oAMXML = new ActiveXObject("MSXML2.DOMDocument.3.0");
        oAMXML.async = false;
        oAMXML.preserveWhiteSpace = true;
        oAMXML.setProperty("SelectionLanguage", "XPath");

        if (!oStudentTools.IsConformantXmlFile(strAMLocalFile)) {
          throw new Error(0, L_ErrorMalformedAssignmentsXML_Text);
        }
        
        if (oAMXML.load(strAMLocalFile)) {          
          strCourseName = oAMXML.selectSingleNode("course/name").text;
          strCourseURL = oAMXML.selectSingleNode("course/assnmgr/amurl").text;
          strGUID = oAMXML.selectSingleNode("course/assnmgr/guid").text;

          oItem.selectSingleNode("name").text = strCourseName;
          oAMNode.selectSingleNode("amurl").text = strCourseURL;
          oAMNode.selectSingleNode("guid").text = strGUID;
          fUpdateCoursesXML = 1;
        }
      }

      WorkWithCoursesTab.innerHTML +=
      "<br><table class=\"tableStyle\" width=\"100%\" cellspacing=\"0\" cellpadding=\"8\"><tr><td class=\"workWithCourseHead\"> <span class=\"courseName\" id='courseName" + i + "'>" + strCourseName + "</span></td></tr>" +
      "<tr><td class= \"workWithCourseBody\"><div class=\"AppHostDescriptionText\"><span tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" class='editLinkText' id='workwithcourse" + i + "' onFocus='studentMOver(this)' onBlur='studentMOut(this)' onmouseOver='studentMOver(this)' onmouseOut='studentMOut(this)' onclick='LoadAssignmentManagerCourse(\"" +
      strCourseURL + "\", \"" + strCourseGUID + "\")'>" + L_WorkWithAssignmentManagerCourse_Text +
      "</span>&nbsp;</div></td></tr></table><BR>";
    } else {
      // Get the network-path location of the assignment. We need
      // this later, whether we use it here or not.
      strAssignmentsXMLNetLocation = oItem.selectSingleNode("noassnmgr/netassns").text;
    
      // Then, load the assignments.xml file associated with the
      // particular file. If it's not available on-disk, then we've
      // had an error, as the previous step should've done it!
      strAssignmentsXMLCacheFileName = sLocalFilePath + g_strLocalCourseSubdir + oItem.selectSingleNode("noassnmgr/localassns").text;

      // If we can, try to get it from the network. If we can't, load it from cache. If that fails, too, assert.
      // If we get it from the network, then we want to updated the locally-cached copy.
      bFromCache = false;
      strAssignmentsXMLLocalFile = oStudentTools.RetrieveRemoteXmlDocFile(strAssignmentsXMLNetLocation, true);
      if ((strAssignmentsXMLLocalFile == null) ||
          (!oStudentTools.IsConformantXmlFile(strAssignmentsXMLLocalFile)) ||
          (!oAssignmentsXML.load(strAssignmentsXMLLocalFile))) {
        if ((bLoaded = oAssignmentsXML.load(strAssignmentsXMLCacheFileName)) &&
            (oStudentTools.IsConformantXmlFile(strAssignmentsXMLCacheFileName))) {
          bFromCache = true;
        } else {
          // The file both couldn't be loaded from the online source *and*
          // wasn't cached locally. This should never happen, unless the file
          // is malformed or the user was changing data in the files on disk.

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
      } else {
        // Save a new cached copy
        oAssignmentsXML.save(strAssignmentsXMLCacheFileName);
      }

      oAssignmentsXML.setProperty("SelectionLanguage", "XPath");

      // When retrieving the course's name and description, also update the local
      // copies in courses.xml, if possible. 
      strCourseName = oAssignmentsXML.selectSingleNode("course/name").text;
      oItem.selectSingleNode("name").text = strCourseName;
      strCourseDescription = oAssignmentsXML.selectSingleNode("course/noassnmgr/description").text;
      fUpdateCoursesXML = 1;

      strCourseWeb = oAssignmentsXML.selectSingleNode("course/noassnmgr/courseweb").text;
      strResources = "<span id='courseWeb" + i + "'>" + L_CourseWeb_HTMLText + "<a tabindex=\"1\" id='courseWebHref" + i + "' href=" +
      strCourseWeb + ">" + strCourseWeb + " </a></span>";

      oResources = oAssignmentsXML.selectNodes("/course/noassnmgr/resource");
      if (oResources != null) {
        while ((oResource = oResources.nextNode()) != null) {
          strResources += "<li><a href=\"" + oResource.selectSingleNode("url").text +
          "\">" + oResource.selectSingleNode("description").text + "</a></li>";
        }
      }

      strAssignments = "";
      oAssignments = oAssignmentsXML.selectNodes("/course/assignment");
      if (oAssignments != null) {
        while ((oAssignment = oAssignments.nextNode()) != null) {
          j++;
          strAssignments += 
          "<div><span class=\"courseName\" tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" onClick=\"showHideAssn(ass" + i + "A" + j + ", course" + i + "A" + j + "Arrow )\" style=\"cursor:hand;\"><img src=\"images/Arrow_open_Dark.gif\" alt=\"\" border=\"0\" id=\"course" + i + "A" + j + "Arrow\">&nbsp;" +
          oAssignment.selectSingleNode("name").text + 
          "</span><br><div class=\"body\" id=\"ass" + i + "A" + j + "\" style=\"position:relative; left:30; display:inline;\">" +
          oAssignment.selectSingleNode("description").text +
          "<br><span class=\"editLinkText\" onFocus='studentMOver(this)' onBlur='studentMOut(this)' onmouseOver='studentMOver(this)' onmouseOut='studentMOut(this)' id=\"downLoadStarter" + i + "A" + j + "\" tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" onClick=\"DownloadStarterProj( '" + EscapePathString(strAssignmentsXMLNetLocation) + "', '" + oAssignment.selectSingleNode("name").text
          + "');\" style=\"position:relative; top:0;left:0; font-size:100%\">" +
          L_DownloadStarterProject_HTMLText + "</span></div></div><p>";
        }
      }

      if (strAssignments == "") {
        strAssignments = L_NoAssignmentsListed_HTMLText;
      }

      strPrintCourseName = "<table class=\"tableStyle\" width=\"100%\" cellspacing=\"0\" cellpadding=\"8\"><tr><td class=\"workWithCourseHead\"><span class=\"courseName\">" + strCourseName + "</span>";
      if (bFromCache) {
        strPrintCourseName = strPrintCourseName + " <span class=\"head1\"><i>" +
        L_ErrorUsingCachedInformation_Text + "</i></span></td></tr>";
      }
      
      WorkWithCoursesTab.innerHTML += 
      strPrintCourseName + "<tr><td class=\"workWithCourseBody\"><div class=\"AppHostDescriptionText\"><div onClick=\"AppHostDescText(course" + i + "AHiddenText, short" + i + "A, course" + i +
      "DesArrow);\" style=\"cursor: hand;\"><img src=\"images/Arrow_closed_Dark.gif\" alt=\"\" border=\"0\" id=\"course" + i +
      "DesArrow\">&nbsp;<span  class=\"courseName\"  id=\"short" + i +
      "A\">" + L_FullCourseDescription_HTMLText +
      "</span></div><div class=\"hiddenText\" id=\"course" + i + "AHiddenText\" style=\"position:relative; left:30; display:none;\">" +
      strCourseDescription + "</div><p><div onClick=\"AppHostDescText(course" + i + "Resources, resources" + i + ", course" + i +
      "ResArrow);\" style=\"cursor: hand;\"><img src=\"images/Arrow_closed_Dark.gif\" alt=\"\" border=\"0\" id=\"course" + i +
      "ResArrow\">&nbsp;<span tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" class=\"courseName\" id=\"resources" + i + "\">" + L_CourseResources_HTMLText +
      "</span></div><div class=\"hiddenText\" id=\"course" + i + "Resources\" style=\"position:relative; left:30; display:none;\">" +
      strResources +
      "</div><p><Div><span tabindex=\"1\" onkeypress=\"AccKeyPressHandler(this)\" id=\"course" + i + "AssignmentsHeader\" onClick=\"AppHostCourseDesc(course" + i + "Assignments, course" + i +
      "AssignmentsHeader, course" + i + "AssignmentsArrow);\" style=\"cursor: hand;\"><img src=\"images/Arrow_closed_Dark.gif\" alt=\"\" border=\"0\" id=\"course" +
      i + "AssignmentsArrow\">&nbsp;<span class=\"courseName\">" + L_CourseAssignments_HTMLText +
      "</span></span></div><div class=\"hiddenText\" id=\"course" +
      i + "Assignments\" style=\"position:relative; left:30; display:none;\">" +
      strAssignments + "</div></div></div></td></tr></table><br>";
    }
  }

  try {
    // If the user passed an argument to go to a certain course, then expand
    // the course. Note that we do NOT scroll, as that could cause the menu
    // from the top to go off the screen.
    index = GetCourseIndex();
    if ((index != 0) && (index <= i)) {
      index;
      eval("AppHostDescText(course" + index + "AHiddenText, short" + index + "A, course" + index + "DesArrow);");
      eval("AppHostCourseDesc(course" + i + "Assignments, course" + i + "AssignmentsHeader, course" + i + "AssignmentsArrow);");
    }
  } catch (ignore) {
  }
  
  try {
    // If we have updated any of the data associated with the XML file,
    // save it now that we're done loading.
    if (fUpdateCoursesXML == 1) {
      oXML.save(sLocalFilePath + "courses.xml");
      oStudentTools.RegenerateUserCustomTab(false);
    }
  } catch(ignore) {
  }

  if (i == 0) {
    // There was a courses file, but no courses were available.
    WorkWithCoursesTab.innerHTML = L_NoAvailableCourses_HTMLText;
  }
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

// Simply browses back to the VS start page. This function is used
// as the target of an OnClick even of several <span> tags.
function LoadHomePage()
{
  window.navigate("vs://default.htm?tab=" + L_TabNameHTMLQuoted_HTMLText);
}

// Simply browses back to the addcourse page. This function is used
// as the target of an OnClick even of several <span> tags.
function LoadAddCourses()
{
  window.location = "vs://custom/student/addcourse.html";
}

// Simply browses back to the deletecourse page. This function is used
// as the target of an OnClick even of several <span> tags.
function LoadDeleteCourses()
{
  window.location = "vs://custom/student/deletecourse.html";
}

// Simply browses back to the workwithcourse page. This function is used
// as the target of an OnClick even of several <span> tags. Note that there
// is an optional paramater that specifies which course is being selected
// in order to draw the user directly to that course. This is the one-based
// index of the course in the local courses.xml file.
function LoadWorkWithCourses(courseName)
{
  var path = "vs://custom/student/workwith.html";

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
function DeleteSelectedCourses(n) {
  var DeleteBoxChecked = null;
  var CourseName = null;
  var oStudentTools = null;
  var oStudentCourses = null;
  var DeletedACourse = false;
  var x = null;
  var nCourses = 0;
  var oCourseNode = null;

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
        eval("CourseName = DeleteCourseLabel" + i + ".innerText");
        if (oStudentTools == null) {
          oStudentTools = GetStudentObject();
        }
        if (oStudentCourses == null) {
          oStudentCourses = GetStudentCoursesFile();
        }

        oCourseNode = oStudentCourses.selectSingleNode("/studentcourses/course[name='" + CourseName + "']");
        if (oCourseNode == null) {
          throw new Error(0, L_UnableToLoadInfo_HTMLText);
        }

        if (oCourseNode.selectSingleNode("assnmgr") != null) {
          var strGUID = oCourseNode.selectSingleNode("assnmgr/guid").text;
          var strPath = oCourseNode.selectSingleNode("assnmgr/amurl").text
          
          DeletedACourse = oStudentTools.UnregisterAssignmentManagerCourse(strGUID) || DeletedACourse;
          
          // Browse to the student-side deletion page associated with the course.
          strPath += "/Student/DeleteCourse.aspx?CourseID=" + strGUID;
          try {
            window.external.ItemOperations.Navigate(strPath, 1); //EnvDTE.vsNavigateOptions.vsNavigateOptionsNewWindow
          } catch (e) {
            alert(L_ErrorUnableToBrowseToAMCourse_Text);
          }
        } else {
          DeletedACourse = oStudentTools.DeleteCourse(CourseName) || DeletedACourse;
        }
      }
    }
 
    if (DeletedACourse) {
      RefreshDeleteCoursesPage();
    } else {
      alert(L_ErrorDeleteCourse_Text);
    }
  }
    } catch (e) {
    window.alert(e.description);
  }
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

// This simply replaces all single-back-slashes with
// double-slashes. It doesn't do anything intelligent about detecting
// double-back-slashes and ignoring them -- they'll turn into
// quadruples.
function EscapePathString(path) {
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

// This is tied to the <span> tags associated with assignment downloads,
// and will attempt to download a 'starter project', given an assignments.xml
// path and the name of the assignment to get.
function DownloadStarterProj(assignmentFile, assignmentName) {
  try {
    var oStudentTools = GetStudentObject();

    oStudentTools.DownloadStarterProject(assignmentFile, assignmentName);
  } catch (e) {
    window.alert(e.description);
  }
}

// Whenever the user hits enter, we want to trigger the
// Add button on the form.
function AddCourseHandleKeyDown() {
  var i = window.event.keyCode;

  switch (i) {
  case 13: // ENTER
    AddCourseButton.click();
    break;
  default:
    break;
  }
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

function showHideAssn(whichAssn, whichArrow ) {
  if (whichAssn.style.display == "inline") {
    whichAssn.style.display = "none";
    whichArrow.src = "images/Arrow_closed_Dark.gif";
  }else{
    whichAssn.style.display = "inline";
    whichArrow.src = "images/Arrow_open_Dark.gif";
  }
}
function showHideCourseDesc(whichTarget, obj, img) {
  var x = whichTarget.style.display;
  if (x == "none") {
    whichTarget.style.display = "inline";
    obj.innerText = "";
    obj.style.pixelLeft = "40";
    img.src = "images/Arrow_open_Dark.gif";
  }else{
    whichTarget.style.display = "none";
    obj.innerText = L_CourseAssignments_HTMLText;
    obj.style.pixelLeft = "10";
    img.src = "images/Arrow_closed_Dark.gif";
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
