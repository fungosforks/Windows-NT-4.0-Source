//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//

namespace Microsoft.VisualStudio.Academic.AssignmentManager.Faculty
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.SessionState;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.IO;
    using System.Security;
    using Microsoft.VisualStudio.Academic.AssignmentManager;

    /// <summary>
    ///    Summary description for UploadDownload.
    /// </summary>
    public class UploadDownload : System.Web.UI.Page
    {
        protected System.Web.UI.HtmlControls.HtmlInputButton btnDownload;
        protected System.Web.UI.HtmlControls.HtmlInputText txtDirSize;
        protected System.Web.UI.HtmlControls.HtmlInputText txtMaxUploadSize;
        protected System.Web.UI.HtmlControls.HtmlInputText txtSolutionName;
        protected System.Web.UI.HtmlControls.HtmlInputText txtDownloadFilesXML;
        protected System.Web.UI.HtmlControls.HtmlInputText txtFilesUploadedXML;
        protected System.Web.UI.HtmlControls.HtmlInputText txtDownloadFolderLocation;
        protected System.Web.UI.HtmlControls.HtmlInputText txtUploadLocation;
        protected System.Web.UI.HtmlControls.HtmlInputText txtExistingStarterProject;
        protected System.Web.UI.HtmlControls.HtmlInputText txtNewGUID;
        protected System.Web.UI.WebControls.Label lblSelectProject;
        protected System.Web.UI.WebControls.Label lblAssignmentName;
        protected System.Web.UI.WebControls.TextBox txtAssignmentName;
        protected System.Web.UI.WebControls.Label lblUploadSubTitle;
        protected System.Web.UI.WebControls.Label lblUploadDescription;
        protected System.Web.UI.WebControls.Label lblDownloadFacultyTitle;
        protected System.Web.UI.WebControls.Label lblDownloadLocationForFiles;
        protected System.Web.UI.WebControls.Label lblStudentName;
        protected System.Web.UI.WebControls.Label lblAssignmentNameDownload;
        protected System.Web.UI.WebControls.TextBox txtStudentName;
        protected System.Web.UI.WebControls.TextBox txtAssignmentNameDownload;
        //Localization strings
        public readonly string UploadDownload_ProjectAlreadyOpenInSolution = SharedSupport.GetLocalizedString("UploadDownload_ProjectAlreadyOpenInSolution");
        public readonly string UploadDownload_NoSolutionOrProject = SharedSupport.GetLocalizedString("UploadDownload_NoSolutionOrProject");
        public readonly string UploadDownload_CloseCurrentSol = SharedSupport.GetLocalizedString("UploadDownload_CloseCurrentSol");
        public readonly string UploadDownload_PromptToSave = SharedSupport.GetLocalizedString("UploadDownload_PromptToSave");
        public readonly string UploadDownload_Download_Faculty_Title = SharedSupport.GetLocalizedString("UploadDownload_Download_Faculty_Title");
        public readonly string UploadDownload_StudentName = SharedSupport.GetLocalizedString("UploadDownload_StudentName");
        public readonly string UploadDownload_err_No_Action = SharedSupport.GetLocalizedString("UploadDownload_err_No_Action");
        public readonly string UploadDownload_err_No_Files_Downloaded =  SharedSupport.GetLocalizedString("UploadDownload_err_No_Files_Downloaded");
        public readonly string UploadDownload_err_No_Files_Uploaded = SharedSupport.GetLocalizedString("UploadDownload_err_No_Files_Uploaded");
        public readonly string UploadDownload_err_Server_Location_Not_Found = SharedSupport.GetLocalizedString("UploadDownload_err_Server_Location_Not_Found");
        public readonly string UploadDownload_err_Local_Download_Location_Not_Exist = SharedSupport.GetLocalizedString("UploadDownload_err_Local_Download_Location_Not_Exist");
        public readonly string UploadDownload_err_XMLFileList_Load_Failed = SharedSupport.GetLocalizedString("UploadDownload_err_XMLFileList_Load_Failed");
        public readonly string UploadDownload_err_File_Does_Not_Exist = SharedSupport.GetLocalizedString("UploadDownload_err_File_Does_Not_Exist");
        public readonly string UploadDownload_err_Attempt_To_Copy = SharedSupport.GetLocalizedString("UploadDownload_err_Attempt_To_Copy");
        public readonly string UploadDownload_err_Attempt_To_Copy_Failed = SharedSupport.GetLocalizedString("UploadDownload_err_Attempt_To_Copy_Failed");
        public readonly string UploadDownload_err_Upload_Location_Not_Available = SharedSupport.GetLocalizedString("UploadDownload_err_Upload_Location_Not_Available");
        public readonly string UploadDownload_err_Download_Location_Not_Available = SharedSupport.GetLocalizedString("UploadDownload_err_Download_Location_Not_Available");
        public readonly string UploadDownload_prob_Opening_Web_Project = SharedSupport.GetLocalizedString("UploadDownload_prob_Opening_Web_Project"); 
        public readonly string UploadDownload_err_Select_Proj_To_Upload = SharedSupport.GetLocalizedString("UploadDownload_err_Select_Proj_To_Upload");
        public readonly string UploadDownload_err_Terminate_And_Delete = SharedSupport.GetLocalizedString("UploadDownload_err_Terminate_And_Delete");
        public readonly string UploadDownload_err_The_Copy_of = SharedSupport.GetLocalizedString("UploadDownload_err_The_Copy_of"); 
        public readonly string UploadDownload_err_Failed = SharedSupport.GetLocalizedString("UploadDownload_err_Failed");
        public readonly string UploadDownload_err_Exceeded_Max_Size = SharedSupport.GetLocalizedString("UploadDownload_err_Exceeded_Max_Size"); 
        public readonly string UploadDownload_err_MB_No_More_Files_Uploaded = SharedSupport.GetLocalizedString("UploadDownload_err_MB_No_More_Files_Uploaded");
        public readonly string UploadDownload_err_Select_Location = SharedSupport.GetLocalizedString("UploadDownload_err_Select_Location");
        public readonly string UploadDownload_err_Opening_Project = SharedSupport.GetLocalizedString("UploadDownload_err_Opening_Project"); 
        public readonly string UploadDownload_err_Upload_Failed = SharedSupport.GetLocalizedString("UploadDownload_err_Upload_Failed");
        public readonly string UploadDownload_err_code_hiding_failed = SharedSupport.GetLocalizedString("UploadDownload_err_code_hiding_failed"); 
        public readonly string UploadDownload_err_Addin_Not_Loaded = SharedSupport.GetLocalizedString("UploadDownload_err_Addin_Not_Loaded");
        public readonly string UploadDownload_err_ProjType_Not_Supported = SharedSupport.GetLocalizedString("UploadDownload_err_ProjType_Not_Supported");
        public readonly string UploadDownload_msg_File_Copy_Complete = SharedSupport.GetLocalizedString("UploadDownload_msg_File_Copy_Complete");
        public readonly string UploadDownload_msg_Files_Were_Uploaded = SharedSupport.GetLocalizedString("UploadDownload_msg_Files_Were_Uploaded");
        public readonly string UploadDownload_dir_Code_Stipping_TempDir = SharedSupport.GetLocalizedString("UploadDownload_dir_Code_Stipping_TempDir");
        public readonly string UploadDownload_btn_Add_Text = SharedSupport.GetLocalizedString("UploadDownload_btn_Add_Text");
        public readonly string UploadDownload_btn_UploadProject_Text = SharedSupport.GetLocalizedString("UploadDownload_btn_UploadProject_Text");
        public readonly string UploadDownload_btnCancel_Text = SharedSupport.GetLocalizedString("UploadDownload_btnCancel_Text");
        public readonly string UploadDownload_Extra_Files_Text = SharedSupport.GetLocalizedString("UploadDownload_Extra_Files_Text");
        public readonly string UploadDownload_Location_For_Download_Files_Text = SharedSupport.GetLocalizedString("UploadDownload_Location_For_Download_Files_Text");
        public readonly string UploadDownload_Download_Text = SharedSupport.GetLocalizedString("UploadDownload_Download_Text");
        public readonly string UploadDownload_Download_Colon_Text = SharedSupport.GetLocalizedString("UploadDownload_Download_Colon_Text");
        public readonly string Title = SharedSupport.GetLocalizedString("AM_Title");
        public readonly string UploadDownload_RemoveCode_Text = SharedSupport.GetLocalizedString("UploadDownload_RemoveCode_Text");
        public readonly string UploadDownload_SelectProject = SharedSupport.GetLocalizedString("UploadDownload_SelectProject");
        public readonly string UploadDownload_StatusBarUploadingText = SharedSupport.GetLocalizedString("UploadDownload_StatusBarUploadingText");
        public readonly string UploadDownload_err_No_Starter_Files = SharedSupport.GetLocalizedString("UploadDownload_err_No_Starter_Files");
        public readonly string UploadDownload_err_No_Submitted_Files = SharedSupport.GetLocalizedString("UploadDownload_err_No_Submitted_Files");
        public readonly string UploadDownload_UploadSubTitle = SharedSupport.GetLocalizedString("UploadDownload_UploadSubTitle");
        public readonly string UploadDownload_UploadDescription = SharedSupport.GetLocalizedString("UploadDownload_UploadDescription");
        public readonly string UploadDownload_BrowseButtonText = SharedSupport.GetLocalizedString("UploadDownload_BrowseButtonText");
        public readonly string UploadDownload_DownloadPrompt = SharedSupport.GetLocalizedString("UploadDownload_DownloadPrompt");
        public readonly string UploadDownload_AlreadyStarterUploaded = SharedSupport.GetLocalizedString("UploadDownload_AlreadyStarterUploaded");
        public string UploadDownload_DownloadRedirectUrl = "";
        protected System.Web.UI.WebControls.Label Label1;
        protected System.Web.UI.WebControls.Label Label2;

        public readonly string UploadDownload_AssignmentName = SharedSupport.GetLocalizedString("UploadDownload_AssignmentName");

        protected AssignmentManager.UserControls.faculty Nav1;
        protected System.Web.UI.HtmlControls.HtmlInputCheckBox chkRemoveStudentCode;
        protected System.Web.UI.WebControls.Label lblRemoveCode;
        protected AssignmentManager.UserControls.goBack GoBack1;

        public UploadDownload()
        {
            Page.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
                int courseId = func.ValidateNumericQueryStringParameter(this.Request, "CourseID");

                if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.ASSIGNMENT_VIEW))
                {
                    // Note that Redirect ends page execution.
                    Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
                }

                //Cleanup temporary files after project downloads. Page does not need to load.
                if(Request.QueryString.Get("Action").ToLower() == "cleanupdirectory")
                {
                    CleanupTempDirectory();
                }

                Nav1.Feedback.Text =  String.Empty;
                Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT;
                Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_COURSE_ASSIGNMENTS;
                if(Request.QueryString.Get("Action").ToLower() == "uploadstarter")
                {
                    Nav1.Title = SharedSupport.GetLocalizedString("UploadDownload_Upload_Starter_Title");//"Upload Starter";
                }
                if(Request.QueryString.Get("Action").ToLower() == "downloadsubmission") 
                {
                    Nav1.Title = SharedSupport.GetLocalizedString("UploadDownload_Download_Faculty_Title");//"Download Student Submission";
                }
                Nav1.relativeURL = @"../";

                if (Request.QueryString.Get("Action").ToLower() == "uploadstarter")
                {
                    GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vstskUpdatingStarterProject");
                }
                else
                {
                    GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vsoriUsingAssignmentManager");
                }
                GoBack1.GoBackIncludeBack = true;
                GoBack1.GoBack_left = "295px";
                GoBack1.GoBack_top = "22px";
                if(Request.UrlReferrer.ToString() != "")
                {
                    GoBack1.GoBack_BackURL = Request.UrlReferrer.ToString();
                }
                else
                {
                    GoBack1.GoBack_BackURL = "Assignments.aspx?" + Request.QueryString.ToString();
                }
                //Give the client the upload and download locations
                if(SharedSupport.UsingSsl)
                {
                    txtUploadLocation.Value = "https://" + Request.ServerVariables.Get("HTTP_HOST") + Constants.ASSIGNMENTMANAGER_UPLOAD_DIRECTORY;
                    txtDownloadFolderLocation.Value = "https://" + Request.ServerVariables.Get("HTTP_HOST") + Constants.ASSIGNMENTMANAGER_DOWNLOAD_DIRECTORY;
                }
                else
                {
                    txtUploadLocation.Value = "http://" + Request.ServerVariables.Get("HTTP_HOST") + Constants.ASSIGNMENTMANAGER_UPLOAD_DIRECTORY;
                    txtDownloadFolderLocation.Value = "http://" + Request.ServerVariables.Get("HTTP_HOST") + Constants.ASSIGNMENTMANAGER_DOWNLOAD_DIRECTORY;
                }

                txtMaxUploadSize.Value = SharedSupport.GetSetting(AssignmentManager.Constants.MAX_PROJECT_SETTING).ToString();
                btnDownload.Value = UploadDownload_Download_Text;

                string dir = SharedSupport.AddBackSlashToDirectory(System.Web.HttpContext.Current.Request.MapPath(String.Empty,Constants.ASSIGNMENTMANAGER_DOWNLOAD_DIRECTORY,true));
                if(Directory.Exists(dir))
                {
                    txtDirSize.Value = SharedSupport.GetSetting(AssignmentManager.Constants.MAX_PROJECT_SETTING).ToString();
                }

                int studentID = func.ValidateNumericQueryStringParameter(this.Request, "StudentID");
                int assignmentId = func.ValidateNumericQueryStringParameter(this.Request, "AssignmentID");
                if (!IsPostBack)
                {
                    //
                    // Evals true first time browser hits the page
                    //
                    if(Request.QueryString.Get("Action").ToLower() == "uploadstarter")
                    {
                        chkRemoveStudentCode.Visible = true;
                        lblRemoveCode.Visible = true;
                    }
                    else
                    {
                        chkRemoveStudentCode.Visible = false;
                        lblRemoveCode.Visible = false;
                    }
                    txtExistingStarterProject.Value = "";
                    lblRemoveCode.Text = UploadDownload_RemoveCode_Text;
                    lblSelectProject.Text = UploadDownload_SelectProject;
                    lblAssignmentName.Text = UploadDownload_AssignmentName;
                    lblUploadSubTitle.Text = UploadDownload_UploadSubTitle;
                    lblUploadDescription.Text = UploadDownload_UploadDescription;
                    lblDownloadFacultyTitle.Text = UploadDownload_Download_Faculty_Title;
                    lblDownloadLocationForFiles.Text = UploadDownload_Location_For_Download_Files_Text;
                    lblStudentName.Text = UploadDownload_StudentName;
                    lblAssignmentNameDownload.Text = UploadDownload_AssignmentName;
                    if(!assignmentId.Equals(0))
                    {
                        AssignmentM assign = AssignmentM.Load(assignmentId);
                        if(assign.IsValid)
                        {
                            if(assign.StarterProjectFlag)
                            {
                                txtExistingStarterProject.Value = "1";
                            }
                            txtAssignmentName.ReadOnly = true;
                            txtAssignmentName.Enabled = false;
                            txtAssignmentName.Text = assign.ShortName;
                        }
                    }

                    //Download of student Submission
                    if(Request.QueryString.Get("Action").ToLower() == "downloadsubmission")
                    {
                        AssignmentM assignment = AssignmentM.Load(assignmentId);
                        if(assignment.IsValid)
                        {
                            if(assignment.StarterProjectFlag)
                            {
                                txtExistingStarterProject.Value = "1";
                            }
                            txtAssignmentName.ReadOnly = true;
                            txtAssignmentName.Enabled = false;
                            txtAssignmentName.Text = assignment.ShortName;
                            txtAssignmentNameDownload.ReadOnly = true;
                            txtAssignmentNameDownload.Enabled = false;
                            txtAssignmentNameDownload.Text = assignment.ShortName;
                        }
                        UserM user = UserM.Load(studentID);
                        if(user.IsValid)
                        {
                            txtStudentName.ReadOnly = true;
                            txtStudentName.Text = Server.HtmlEncode( user.LastName.Trim() + SharedSupport.GetLocalizedString("UploadDownload_comma") + " " + user.FirstName.Trim() + " " + user.MiddleName.Trim() );
                        }

                        txtSolutionName.Value = SharedSupport.AddBackSlashToDirectory(assignment.ShortName);
                        System.Guid guid = System.Guid.NewGuid();
                        StudentAssignmentM studentAssign = StudentAssignmentM.Load(studentID, assignmentId);
                        txtDownloadFilesXML.Value = studentAssign.AssignmentFilesXML(guid.ToString(), assignment);
                        txtNewGUID.Value = guid.ToString();
                    }
                    else
                    {
                        txtNewGUID.Value = System.Guid.NewGuid().ToString();
                    }

                }
                else
                {
                    //If this is coming back from the client see what the action 
                    //is on the query string and perform accordingly
                    //Faculty Submitting Starter Project
                    if(Request.QueryString.Get("Action").ToLower() == "uploadstarter")
                    {
                        string xmlFileList = txtFilesUploadedXML.Value.ToString();
                        AssignmentM assign = AssignmentM.Load(assignmentId);
                        assign.SubmitStarter(xmlFileList, txtNewGUID.Value.ToString());
                        Response.Redirect("Assignments.aspx?CourseID=" + Request.QueryString.Get("CourseID"), false);
                    }

                }
            }
            catch (Exception ex)
            {
                Nav1.Feedback.Text =  ex.Message.ToString();
            }
        }
        //Cleanup temporary files after project downloads.
        private void CleanupTempDirectory() 
        {

            // Grab the querystring parameters.
            string tempGUID = Request.QueryString.Get("GUID");
            bool addQuerystring = Convert.ToBoolean(Request.QueryString.Get("AddQS"));
            string targetUrl = Request.QueryString.Get("TargetURL");

            // Test whether the GUID that identifies the directory to be deleted exists.
            if (tempGUID == "" || tempGUID == string.Empty) 
            {
                // The GUID is missing, try and continue the redirect without deleting the directory.
            }
            else 
            {
                // Delete the temporary download directory from the AMWeb virtual directory.
                DeleteTempDirectory(tempGUID);
            }
            if (addQuerystring == true) 
            {
                // Remove the QueryString parameters used for this action then pass the leftovers on the redirect.
                string tempQuerystring = Request.Url.Query.ToString();
                tempQuerystring = tempQuerystring.Substring(0, tempQuerystring.IndexOf("&GUID"));
                tempQuerystring = tempQuerystring.Replace("StudentID", "UserID");
                Response.Redirect(targetUrl + tempQuerystring);
            }
            else 
            { 
                Response.Redirect(targetUrl);
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP+ Windows Form Designer.
            //
            InitializeComponent();
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);

        }

        // This function is used to delete the temporary directories that are created on the server when files are downloaded.
        internal void DeleteTempDirectory(string GUID) 
        {
            try 
            { 
                // Validate the GUID.
                System.Guid guid = new System.Guid(GUID);

                // Get the local path for the AMDownload directory.
                string filePath = System.Web.HttpContext.Current.Request.MapPath(Constants.ASSIGNMENTMANAGER_DOWNLOAD_DIRECTORY);
                // Add the GUID to complete the temporary filepath (i.e. 'C:\Inetpub\wwwroot\AMDownload\6c5fb16e-9332-46ce-9eab-94ab6b4f5cdb').
                filePath += guid.ToString();
                try 
                {
                    Directory.Delete(filePath, true);
                }
                catch(Exception ex) 
                {
                    SharedSupport.LogMessage(SharedSupport.GetLocalizedString("Assignment_DeleteStarterProjectFailed") + filePath + " " + ex.Message);
                }
            } 
            catch (Exception GUIDException) 
            {
                // Invalid GUID returned, abort delete action to prevent the deleting or files outside of AMDownload directory.
                SharedSupport.LogMessage(SharedSupport.GetLocalizedString("Assignment_DeleteStarterProjectFailed") + GUID + " " + GUIDException.Message);
            }
        }
    }
}
