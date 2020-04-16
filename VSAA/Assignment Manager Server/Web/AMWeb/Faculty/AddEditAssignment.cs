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
	using Microsoft.VisualStudio.Academic.AssignmentManager;

    /// <summary>
    ///    Summary description for AddEditAssignment.
    /// </summary>
    public class AddEditAssignment : System.Web.UI.Page
    {
		protected System.Web.UI.HtmlControls.HtmlTable tblNotification;
		protected System.Web.UI.HtmlControls.HtmlTable tblNotificationHeader;
		protected System.Web.UI.HtmlControls.HtmlTable tblGeneralAssignment;
		protected System.Web.UI.HtmlControls.HtmlTable tblBottomButtons;
		protected System.Web.UI.HtmlControls.HtmlInputButton btnUpdate2;
		protected System.Web.UI.HtmlControls.HtmlTable tblAutoCompile;
		protected System.Web.UI.WebControls.DropDownList cboAutoCompileAssignmentCompileType;
		protected System.Web.UI.WebControls.Label lblAutoCompileAssignmentCompileType;
		protected System.Web.UI.HtmlControls.HtmlTable tblAutoCompileHeader;
		protected System.Web.UI.WebControls.CheckBox chkAutoCompileAssignmentAutompileOn;
		protected System.Web.UI.WebControls.Label lblAutoCompileHeader;
		protected System.Web.UI.HtmlControls.HtmlTable tblAutoGrade;
		protected System.Web.UI.WebControls.DropDownList cboAutoGradeAssignmentGradeType;
		protected System.Web.UI.WebControls.Label lblAutoGradeAssignmentGradeType;
		protected System.Web.UI.HtmlControls.HtmlInputFile inAutoGradeAssignmentOutputFile;
		protected System.Web.UI.WebControls.Label lblAutoGradeAssignmentOutputFile;
		protected System.Web.UI.HtmlControls.HtmlInputFile inAutoGradeAssignmentInputFile;
		protected System.Web.UI.WebControls.Label lblAutoGradeAssignmentInputFile;
		protected System.Web.UI.HtmlControls.HtmlTable tblAutoGradeHeader;
		protected System.Web.UI.WebControls.CheckBox chkAutoGradeAssignmentAutoGradeOn;
		protected System.Web.UI.WebControls.Label lblAutoGradeHeader;
		protected System.Web.UI.HtmlControls.HtmlInputText txtDirSize;
		protected System.Web.UI.HtmlControls.HtmlInputText txtMaxUploadSize;
		protected System.Web.UI.HtmlControls.HtmlInputText txtNewGUID;
		protected System.Web.UI.HtmlControls.HtmlInputText txtUploadLocation;
		protected System.Web.UI.HtmlControls.HtmlInputText txtFilesUploadedXML;
		protected System.Web.UI.HtmlControls.HtmlInputText txtSolutionName;
		protected System.Web.UI.HtmlControls.HtmlInputText txtExistingStarterProject;
		protected System.Web.UI.WebControls.Label lblGeneralAssignmentStarterProject;
		protected System.Web.UI.WebControls.CheckBox chkGeneralAssignmentReminderEmails;
		protected System.Web.UI.WebControls.CheckBox chkGeneralAssignmentMultiSubmit;
		protected System.Web.UI.WebControls.TextBox txtGeneralAssignmentHomePageURL;
		protected System.Web.UI.WebControls.TextBox txtCalendarFlag;
		protected System.Web.UI.WebControls.Label lblGeneralAssignmentHomePageURL;
		protected System.Web.UI.WebControls.Label lblGeneralAssignmentDueDate;
		protected System.Web.UI.WebControls.TextBox txtGeneralAssignmentDescription;
		protected System.Web.UI.WebControls.Label lblGeneralAssignmentDescription;
		protected System.Web.UI.WebControls.TextBox txtGeneralAssignmentName;
		protected System.Web.UI.WebControls.Label lblGeneralAssignmentName;
		protected System.Web.UI.HtmlControls.HtmlTable tblGeneralAssignmentHeader;
		protected System.Web.UI.WebControls.Label lblGeneralHeader;
		protected System.Web.UI.WebControls.Label lblAssignment;
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected System.Web.UI.HtmlControls.HtmlTable Table2;
		protected System.Web.UI.HtmlControls.HtmlTable Table1;
		protected System.Web.UI.WebControls.TextBox AutoGradeAssignmentGradeType;
		protected System.Web.UI.WebControls.TextBox txtStarterProject;
		protected System.Web.UI.HtmlControls.HtmlInputFile File1;		
		protected System.Web.UI.WebControls.TextBox txtReminderDays;
		protected System.Web.UI.WebControls.TextBox txtPastDueDays;
		protected System.Web.UI.WebControls.Label lblPastDueNotice;
		protected System.Web.UI.WebControls.Label lblPastDueDays;
		protected System.Web.UI.WebControls.Label lblReminderNotice;
		protected System.Web.UI.WebControls.Label lblReminderDay;
		protected System.Web.UI.WebControls.Label lblUpdatedProject;		
		protected System.Web.UI.WebControls.Label lblNotificationHeader;
		protected System.Web.UI.WebControls.CheckBox chkSendReminder;
		protected System.Web.UI.WebControls.CheckBox chkPastDueNotice;
		protected System.Web.UI.WebControls.CheckBox chkUpdatedProjectNotice;
		protected System.Web.UI.WebControls.Label lblCommandLineArgs;
		protected System.Web.UI.WebControls.TextBox txtCommandLineArgs;
		protected System.Web.UI.WebControls.Label lblRequired;

		protected AssignmentManager.UserControls.faculty Nav1;
		protected AssignmentManager.UserControls.goBack GoBack1;
		protected System.Web.UI.HtmlControls.HtmlSelect cboTime;
		//Localization strings
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
		public readonly string UploadDownload_Location_For_Download_Files_Text = SharedSupport.GetLocalizedString("UploadDownload_Location_For_Download_Files_Text");
		public readonly string UploadDownload_Download_Text = SharedSupport.GetLocalizedString("UploadDownload_Download_Text");
		public readonly string UploadDownload_Download_Colon_Text = SharedSupport.GetLocalizedString("UploadDownload_Download_Colon_Text");
		public readonly string UploadDownload_AMTitle = SharedSupport.GetLocalizedString("UploadDownload_AMTitle");
		public readonly string UploadDownload_RemoveCode_Text = SharedSupport.GetLocalizedString("UploadDownload_RemoveCode_Text");
		public readonly string UploadDownload_SelectProject = SharedSupport.GetLocalizedString("UploadDownload_SelectProject");
		public readonly string UploadDownload_StatusBarUploadingText = SharedSupport.GetLocalizedString("UploadDownload_StatusBarUploadingText");
		public readonly string UploadDownload_DownloadPrompt = SharedSupport.GetLocalizedString("UploadDownload_DownloadPrompt");
		public readonly string UploadDownload_AlreadyStarterUploaded = SharedSupport.GetLocalizedString("UploadDownload_AlreadyStarterUploaded");
		public string UploadDownload_DownloadRedirectUrl = "";
		public readonly string UploadDownload_NoSolutionOrProject = SharedSupport.GetLocalizedString("UploadDownload_NoSolutionOrProject");
		public readonly string UploadDownload_CloseCurrentSol = SharedSupport.GetLocalizedString("UploadDownload_CloseCurrentSol");
		public readonly string UploadDownload_PromptToSave = SharedSupport.GetLocalizedString("UploadDownload_PromptToSave");
		
		public readonly string AddEditAssignment_WriteOverStarterProject = SharedSupport.GetLocalizedString("AddEditAssignment_WriteOverStarterProject");
		private string INVALID_ASSIGNMENT_ID_ERROR = SharedSupport.GetLocalizedString("AddEditAssignment_INVALID_ASSIGNMENT_ID_ERROR"); //"Invalid AssignmentID.  Assignment does not exist.";
		private string INSERT_BUTTON_TEXT = SharedSupport.GetLocalizedString("AddEditAssignment_INSERT_BUTTON_TEXT"); //"Insert";
		private string UPDATE_BUTTON_TEXT = SharedSupport.GetLocalizedString("AddEditAssignment_UPDATE_BUTTON_TEXT"); //"Update";
		private string INVALID_COURSEID_ERROR = SharedSupport.GetLocalizedString("AddEditAssignment_INVALID_COURSEID_ERROR"); //"There was no record in the Course table corresponding to the passed CourseID.";
		private string INVALID_SECTION_ERROR = SharedSupport.GetLocalizedString("AddEditAssignment_INVALID_SECTION_ERROR"); 
		public string Title = SharedSupport.GetLocalizedString("AM_Title");

		// persist querystring parameters instead of referencing Request object every time needed
		private int courseId;
		private int assignmentId;

		protected System.Web.UI.WebControls.CompareValidator validPastDueDays;
		protected System.Web.UI.WebControls.CompareValidator validateReminderDays;
		protected System.Web.UI.WebControls.DropDownList cboGeneralAssignmentStarterProject;
		protected System.Web.UI.WebControls.Label lblAutoCompileAssignmentMakeFile;
		protected System.Web.UI.HtmlControls.HtmlImage Img2;
		protected System.Web.UI.HtmlControls.HtmlImage Img3;
		protected System.Web.UI.HtmlControls.HtmlImage Img4;
		protected string cancelUrl;
		public readonly string AddEditAssignment_btnCancel2 = SharedSupport.GetLocalizedString("AddEditAssignment_btnCancel2");
		protected System.Web.UI.WebControls.Label lblGeneralAssignmentDueTime;
		protected System.Web.UI.WebControls.TextBox txtDueDate;
		protected System.Web.UI.WebControls.Calendar calDueDate;
		protected System.Web.UI.HtmlControls.HtmlInputButton btnDelete;
		protected System.Web.UI.HtmlControls.HtmlInputText txtAction;
		
		public AddEditAssignment()
		{
			Page.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			try
			{
				if(Page.Request.UrlReferrer.ToString() != "")
				{
					cancelUrl = "Assignments.aspx?" + Request.QueryString.ToString() + "&CancelFlag=1";
				}

				Nav1.Feedback.Text = String.Empty;
				Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT;
				Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_COURSE_ASSIGNMENTS;
				Nav1.relativeURL = @"../";
				
				GoBack1.GoBack_left = "375px";
				GoBack1.GoBack_top = "-15px";
				GoBack1.GoBackIncludeBack = true;
				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vstskAddingAssignmentManagerAssignment");
				if(Request.UrlReferrer.ToString() != "")
				{
					GoBack1.GoBack_BackURL = "Assignments.aspx?" + Request.QueryString.ToString();
				}

				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();		
				// grab CourseID parameter from the querystring
				courseId = func.ValidateNumericQueryStringParameter(this.Request, "CourseID");
	
				assignmentId = func.ValidateNumericQueryStringParameter(this.Request, "AssignmentID");

				chkGeneralAssignmentMultiSubmit.Checked = true;
				chkGeneralAssignmentMultiSubmit.Enabled = false;
				chkGeneralAssignmentMultiSubmit.Visible = false;


				if (assignmentId == 0)
				{
					//Adding an Assignment
					if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.ASSIGNMENT_ADD))
					{
						Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
					}
				}
				else
				{
					if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.ASSIGNMENT_VIEW))
					{
						Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
					}
				}
	
				if (!IsPostBack)
				{
					//
					// Evals true first time browser hits the page
					//
					//Get localization string for all text displayed on the page
					LocalizeLabels();
					//Populate all combo boxes with correct (localized) values 
					populateComboBoxes();
					//Initialize the feedback label to nothing.
					Nav1.Feedback.Text = String.Empty;

					calDueDate.SelectedDate = calDueDate.TodaysDate;
					txtDueDate.Text = calDueDate.TodaysDate.ToShortDateString();

					//Give the client the upload and download locations
					if(!SharedSupport.UsingSsl)
					{
						txtUploadLocation.Value = "http://" + Request.ServerVariables.Get("HTTP_HOST") + Constants.ASSIGNMENTMANAGER_UPLOAD_DIRECTORY;
					}
					else
					{
						txtUploadLocation.Value = "https://" + Request.ServerVariables.Get("HTTP_HOST") + Constants.ASSIGNMENTMANAGER_UPLOAD_DIRECTORY;
					}
					txtMaxUploadSize.Value = SharedSupport.GetSetting(AssignmentManager.Constants.MAX_PROJECT_SETTING).ToString();
				
					string dir = SharedSupport.AddBackSlashToDirectory(System.Web.HttpContext.Current.Request.MapPath(String.Empty,Constants.ASSIGNMENTMANAGER_DOWNLOAD_DIRECTORY,true));
					if(Directory.Exists(dir))
					{
						txtDirSize.Value = SharedSupport.GetSetting(AssignmentManager.Constants.MAX_PROJECT_SETTING).ToString();
					}
					txtNewGUID.Value = System.Guid.NewGuid().ToString();

					if( !SharedSupport.UsingSmtp )
					{
						hideNotifications();
					}
					//Check to make sure there is an assignment ID in the query string
					if(assignmentId != 0)
					{
						//Show the Delete button
						btnDelete.Visible = true;

						//Load the assignment corresponding to the given ID.
						AssignmentM am = AssignmentM.Load(assignmentId);

						//Check to make sure you got a row back
						if(am.IsValid)
						{
							//Populate all fields according to the information you get back given the passed assignment ID.
							populateFields(am);
						}
						else
						{
							//The assignment for the given AssignmentID
							throw new Exception(INVALID_ASSIGNMENT_ID_ERROR);
						}
					}
					else
					{
						//Change Labels on buttons from "Update" to "Insert"
						btnUpdate2.Value = INSERT_BUTTON_TEXT;
					}	
				}
				else
				{
					// Postback
					if (func.ValidateOptionNumericQueryStringParam(this.Request, "CancelFlag") == 1) 
					{
						Response.Redirect("Assignments.aspx?" + Request.QueryString, false);
					}
					if (txtAction.Value == "DeleteAssignment")
					{
						if(SharedSupport.SecurityIsAllowed(courseId, SecurityAction.ASSIGNMENT_EDIT))
						{
							AssignmentM.Delete(assignmentId);
							Response.Redirect("Assignments.aspx?" + Request.QueryString, false);
						}
						else
						{
							throw new Exception(SharedSupport.GetLocalizedString("Global_Unauthorized"));
						}
					}
					else if( txtAction.Value == "submit" )
					{
						if (txtGeneralAssignmentName.Text == String.Empty)
						{
							throw new Exception(SharedSupport.GetLocalizedString("AddEditAssignment_MissingAssignmentName"));
						}
						try
						{
							if (Convert.ToDateTime(txtDueDate.Text) < DateTime.Today)
							{
							}
						}
						catch
						{
							throw new Exception(SharedSupport.GetLocalizedString("AddEditAssignment_DateTime_Invalid"));
						}
						

						//make sure the user has permission create or update assignments
						if(assignmentId == 0) 
						{
							// adding an assignment make sure the user is allowed

							if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.ASSIGNMENT_ADD))
							{
								throw new Exception(SharedSupport.GetLocalizedString("Global_Unauthorized"));
							}
						}
						else
						{
							// updateing an assignment make sure the user is allowed
							if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.ASSIGNMENT_EDIT))
							{
								throw new Exception(SharedSupport.GetLocalizedString("Global_Unauthorized"));
							}
						}

						// perform the update
						assignmentId = saveUpload();
						if(!assignmentId.Equals(0))
						{
							if(!txtFilesUploadedXML.Value.Equals(String.Empty) && txtFilesUploadedXML.Value != "")
							{
								string xmlFileList = txtFilesUploadedXML.Value.ToString();
								AssignmentM assign = AssignmentM.Load(assignmentId);
								assign.SubmitStarter(xmlFileList, txtNewGUID.Value.ToString());
							}
							else
							{
								// see if the autocheck files were updated
								AssignmentM.UpdateAutoCheckFiles(assignmentId, courseId, new Guid(txtNewGUID.Value.ToString()));								
							}
							Response.Redirect("Assignments.aspx?" + Request.QueryString, false);
						}
					}
				}
			}
			catch (System.Exception ex)
			{
				//catch and add all exception errors to the lblFeedback label and display.
				Nav1.Feedback.Text = ex.Message;
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
			this.calDueDate.SelectionChanged += new System.EventHandler(this.calDueDate_SelectionChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}

		private int saveUpload()
		{
			//reset error feedback string
			Nav1.Feedback.Text = String.Empty;

			//validates that if either check box is selected for past due notification, 
			//a number has been entered for days 
			if(chkPastDueNotice.Checked)
			{
				if(this.txtPastDueDays.Text.Trim() == "")
				{	

					throw new Exception(SharedSupport.GetLocalizedString("AddEditAssignment_SendPastDueDayError"));
				}
				else
				{
					// Validate that it's an integer and within the valid range (1->365).
					int days = 0;
					try 
					{
						days = Int32.Parse(txtPastDueDays.Text);
					} 
					catch (System.Exception) 
					{
						throw new Exception(SharedSupport.GetLocalizedString("AddEditAssignment_SendPastDueDayError"));
					}
					if ((days < 1) || (days > 365)) 
					{
						throw new Exception(SharedSupport.GetLocalizedString("AddEditAssignment_SendPastDueDayError"));
					}
				}
			}
			//validates that if a number has been entered for past due notification, 
			//one or both of the check boxes has been selected
			if(chkPastDueNotice.Checked)
			{
				if(this.txtPastDueDays.Text.Trim() == "")
				{
					throw new Exception(SharedSupport.GetLocalizedString("AddEditAssignment_SendPastDueCheckboxError"));
				}
			}
			//validates that if either check box is selected for reminder notification, 
			//a number has been entered for days 
			if(chkSendReminder.Checked)
			{
				if(this.txtReminderDays.Text.Trim() == "")
				{	
					throw new Exception(SharedSupport.GetLocalizedString("AddEditAssignment_SendReminderDayError"));
				}
				else
				{
					// Validate that it's an integer and within the valid range (1->365).
					int days = 0;
					try 
					{
						days = Int32.Parse(txtReminderDays.Text);
					} 
					catch (System.Exception) 
					{
						throw new Exception(SharedSupport.GetLocalizedString("AddEditAssignment_SendReminderDayError"));
					}
					if ((days < 1) || (days > 365)) 
					{
						throw new Exception(SharedSupport.GetLocalizedString("AddEditAssignment_SendReminderDayError"));
					}
				}
			}

			//Validate that a user has added an output file if AutoCheck enabled.
			if (chkAutoGradeAssignmentAutoGradeOn.Checked && (inAutoGradeAssignmentOutputFile.PostedFile == null || inAutoGradeAssignmentOutputFile.PostedFile.ContentLength == 0 || inAutoGradeAssignmentOutputFile.PostedFile.FileName.Equals(String.Empty)))
			{
				throw new Exception(SharedSupport.GetLocalizedString("AddEditAssignment_AutoCheckNoOutputError"));
			}

			//Declare Assignment object and dataset		

			AssignmentM am = null;
			//Check to see if this is an "Insert" or "Update"
			string inputfile, inputfiledir, outputfile, outputfiledir;
			inputfile = inputfiledir = outputfile = outputfiledir = null;
			
			saveUploadedFilesToUploadDir(inAutoGradeAssignmentInputFile, ref inputfile, ref inputfiledir);
			saveUploadedFilesToUploadDir(inAutoGradeAssignmentOutputFile, ref outputfile, ref outputfiledir);
			if(assignmentId != 0)
			{
				//Update
				am = AssignmentM.Load(assignmentId);
				populateAssignmentDataSet(ref am, inputfile, outputfile);			
				am.Update();
			}
			else
			{
				//New Assignment
				am = new AssignmentM(courseId);
				populateAssignmentDataSet(ref am, inputfile, outputfile);
				am.Add();
			}

			copyFileToPermanentStorage(am, inputfile, inputfiledir);
			copyFileToPermanentStorage(am, outputfile, outputfiledir);
			
			if (am != null)
			{
				populateFields(am);
				return am.AssignmentID;
			}
			else
			{
				return 0;
			}
		}
		
		internal void saveUploadedFilesToUploadDir(HtmlInputFile fileUploadControl, ref string uploadFilename, ref string uploadFileDirectory)
		{
			uploadFileDirectory = SharedSupport.AddBackSlashToDirectory(Request.MapPath(String.Empty,Constants.ASSIGNMENTMANAGER_UPLOAD_DIRECTORY,true)) + SharedSupport.AddBackSlashToDirectory(txtNewGUID.Value);
			
			//Verify that Directory structure exists to place uploaded files in.
			if(!Directory.Exists(uploadFileDirectory)) 
			{
				Directory.CreateDirectory(uploadFileDirectory);
			}
			
			//Save uploaded file
			if(!fileUploadControl.PostedFile.FileName.Equals(String.Empty))
			{
				uploadFilename = Path.GetFileName(fileUploadControl.PostedFile.FileName);
				fileUploadControl.PostedFile.SaveAs(uploadFileDirectory + uploadFilename);
			}
			else
			{
				uploadFilename = null;
			}
		}

		internal void copyFileToPermanentStorage(AssignmentM assign, string uploadFilename, string uploadFileDirectory)
		{
			if ((uploadFilename != null) && (uploadFilename != String.Empty))
			{
				//Verify that the StorageDirectory exists before copying.
				if(!Directory.Exists(assign.StorageDirectory)) 
				{
					Directory.CreateDirectory(assign.StorageDirectory);
				}
				File.Copy(uploadFileDirectory + uploadFilename, assign.StorageDirectory + uploadFilename, true);
			}
		}

		internal void populateAssignmentDataSet(ref AssignmentM ds, string inputfile, string outputfile)
		{
			ds.Description = txtGeneralAssignmentDescription.Text;
			ds.ShortName = txtGeneralAssignmentName.Text;
			ds.Status = Constants.ACTIVE_STATUS;

			if(txtStarterProject.Text != String.Empty)
			{
				// set starter project flag 
				ds.StarterProjectFlag = true;
			}
			else
			{
				ds.StarterProjectFlag = false;
			}			
			//Combos
			ds.CompilerType = cboAutoCompileAssignmentCompileType.SelectedItem.Value.ToString();
			//Other column values
			ds.LastUpdatedDate = DateTime.Now;
			ds.LastUpdatedUserID = SharedSupport.GetUserIdentity();

			ds.DueDate = Convert.ToDateTime(txtDueDate.Text + " " + cboTime.Value);
		
			ds.AssignmentURL = txtGeneralAssignmentHomePageURL.Text;
			if((txtCommandLineArgs.Text != null) && !txtCommandLineArgs.Text.Equals(String.Empty))
			{
				ds.CommandLineArgs = txtCommandLineArgs.Text;
			}
			
			//File Input Boxes
			if(inputfile != null)
			{
				//Put original filename into dataset
				ds.InputFile = inputfile;
			}

			if(outputfile != null)
			{
				//Put original filename into dataset
				ds.OutputFile = outputfile;
			}
			
			//CheckBoxes
			ds.MultipleSubmitsFlag = chkGeneralAssignmentMultiSubmit.Checked;
			ds.AutoGradeFlag = chkAutoGradeAssignmentAutoGradeOn.Checked;
			ds.AutoCompileFlag = chkAutoCompileAssignmentAutompileOn.Checked;

			//Combo Boxes
			ds.GradeType = Convert.ToByte(cboAutoGradeAssignmentGradeType.SelectedItem.Value);
			
			//Notification Information - update
			//Send notification before assignment due date
			ds.SendReminders = chkSendReminder.Checked;

			//send notification when the assignment is past due
			ds.SendPastDue = chkPastDueNotice.Checked;

			//send notification when a new or updated starter project has been posted
			ds.SendNewProject = ds.SendUpdatedProject = chkUpdatedProjectNotice.Checked;

			if(chkSendReminder.Checked)
			{
				if(txtReminderDays.Text.Trim() != "")
				{
					ds.ReminderWarningDays = Convert.ToInt32(this.txtReminderDays.Text.Trim());
				}
				else
				{
					ds.ReminderWarningDays = 0;	
				}
			}
			if(chkPastDueNotice.Checked)
			{
				if(txtPastDueDays.Text.Trim() != "")
				{
					ds.PastDueWarningDays = Convert.ToInt32(this.txtPastDueDays.Text.Trim());
				}
				else
				{
					ds.PastDueWarningDays = 0;
				}
			}
			ds.LastUpdatedDate = System.DateTime.Now;
		}

		private void populateFields(AssignmentM assignment)
		{
			//Change Labels on buttons from "Add" to "Update"
			btnUpdate2.Value = UPDATE_BUTTON_TEXT;
			//Populate TextBoxes
			if(assignment.Description != null) 
			{
				txtGeneralAssignmentDescription.Text = assignment.Description;
			}
			if(assignment.StarterProjectFlag)
			{
				txtExistingStarterProject.Value = "1";
			}
			else
			{
				txtExistingStarterProject.Value = String.Empty;
			}

			if(!assignment.DueDate.ToString().Equals(string.Empty))
			{	
				this.cboTime.Value = assignment.DueDate.ToShortTimeString();
				calDueDate.SelectedDate = calDueDate.VisibleDate = assignment.DueDate.Date;
				if((assignment.AssignmentURL != null) && !assignment.AssignmentURL.Equals(String.Empty)) 
				{
					txtGeneralAssignmentHomePageURL.Text = assignment.AssignmentURL;
				}		
				if((assignment.CommandLineArgs != null) && !assignment.CommandLineArgs.Equals(String.Empty)) 
				{
					txtCommandLineArgs.Text = assignment.CommandLineArgs;
				}
											
				txtGeneralAssignmentName.Text = assignment.ShortName;
				
				//Populate CheckBoxes
				if(!assignment.MultipleSubmitsFlag)
				{
					chkGeneralAssignmentMultiSubmit.Checked = false;
				}
				else
				{
					chkGeneralAssignmentMultiSubmit.Checked = true;
				}
				if(!assignment.AutoGradeFlag)
				{
					chkAutoGradeAssignmentAutoGradeOn.Checked = false;
				}
				else
				{
					chkAutoGradeAssignmentAutoGradeOn.Checked = true;
				}
				if(!assignment.AutoCompileFlag)
				{
					chkAutoCompileAssignmentAutompileOn.Checked = false;
				}
				else
				{
					chkAutoCompileAssignmentAutompileOn.Checked = true;
				}
			
				this.chkSendReminder.Checked = assignment.SendReminders;
				this.chkPastDueNotice.Checked = assignment.SendPastDue;
				this.chkUpdatedProjectNotice.Checked = assignment.SendUpdatedProject;				

				if(assignment.StarterProjectFlag)
				{
					//a Starter project has been uploaded before, this ensures that the flag is set 
					//correctly even if the user does not upload another one.
					txtStarterProject.Text = assignment.StarterProjectFlag.ToString();
				}
				else
				{
					//Set the flag for the starterproject to inform that one has not be uploaded before.
					txtStarterProject.Text = "-1";
				}
				//populate text boxes that correspond with checkboxes
				this.txtReminderDays.Text = assignment.ReminderWarningDays.ToString();
				this.txtPastDueDays.Text = assignment.PastDueWarningDays.ToString();				

				//Set Combo Boxes - cycle through each until you find the saved one, set it as the selected one, and break out of the loop.
				for(int i=0;i<cboAutoGradeAssignmentGradeType.Items.Count;i++)
				{
					if(cboAutoGradeAssignmentGradeType.Items[i].Value.Equals(assignment.GradeType.ToString()))
					{cboAutoGradeAssignmentGradeType.SelectedIndex = i; break;}
				}
				for(int i=0;i<cboAutoCompileAssignmentCompileType.Items.Count;i++)
				{
					if(cboAutoCompileAssignmentCompileType.Items[i].Value == assignment.CompilerType)
					{cboAutoCompileAssignmentCompileType.SelectedIndex = i; break;}
				}
			}
		}

		/// <summary>
		///		Clear and then populate all combo boxes with initial values.
		///		Right now these values are not coming from the database.  Later, they should.
		/// </summary> 
		private void populateComboBoxes()
		{
			clearComboBoxes();

			System.Web.UI.WebControls.ListItem newItem;

			newItem= new System.Web.UI.WebControls.ListItem(SharedSupport.GetLocalizedString("AddEditAssignment_VS"),"1");
			cboAutoCompileAssignmentCompileType.Items.Add(newItem);
			newItem= new System.Web.UI.WebControls.ListItem(SharedSupport.GetLocalizedString("AddEditAssignment_Compare_E_A_output"),"1");
			cboAutoGradeAssignmentGradeType.Items.Add(newItem);


		}
		private void clearComboBoxes()
		{
			cboAutoCompileAssignmentCompileType.Items.Clear();
			cboAutoGradeAssignmentGradeType.Items.Clear();
		}

		protected void LocalizeLabels()
		{
			Nav1.Feedback.Text = String.Empty;
			if(assignmentId == 0)
			{
				lblAssignment.Text = SharedSupport.GetLocalizedString("AddEditAssignment_Add");
			}
			else
			{
				lblAssignment.Text = SharedSupport.GetLocalizedString("AddEditAssignment_Edit");
				AssignmentM assignment = AssignmentM.Load(assignmentId);
				if(assignment.IsValid)
				{
					Nav1.SubTitle = SharedSupport.GetLocalizedString("AddEditAssignment_SubTitle") + assignment.ShortName;
					// Add "(read-only)" label to SubTitle if they don't have edit permissions.
					if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.ASSIGNMENT_EDIT))
					{
						Nav1.SubTitle += " " + SharedSupport.GetLocalizedString("AddEditAssignment_SubTitle_ReadOnly");
					}
				}
			}
			this.lblGeneralHeader.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblGeneralHeader"); //"ASSIGNMENT";
			this.lblGeneralAssignmentName.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblGeneralAssignmentName");//"Name: ";
			this.txtGeneralAssignmentName.Text = String.Empty;
			this.lblGeneralAssignmentDescription.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblGeneralAssignmentDescription"); //"Description: ";
			this.txtGeneralAssignmentDescription.Text = String.Empty;
			this.lblGeneralAssignmentDueDate.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblGeneralAssignmentDueDate"); //"Due Date: ";
			this.lblGeneralAssignmentDueTime.Text = "Time of Day:";
			this.chkGeneralAssignmentMultiSubmit.Text = SharedSupport.GetLocalizedString("AddEditAssignment_chkGeneralAssignmentMultiSubmit"); //"Multiple Submits";
			
			this.lblGeneralAssignmentStarterProject.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblGeneralAssignmentStarterProject"); //"Starter Project: ";
			
			this.lblAutoGradeHeader.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblAutoCheckHeader"); //"AUTO GRADE";
			
			this.chkAutoGradeAssignmentAutoGradeOn.Text = SharedSupport.GetLocalizedString("AddEditAssignment_chkAutoGradeAssignmentAutoGradeOn"); //"Auto Grade On";
			this.lblAutoGradeAssignmentInputFile.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblAutoGradeAssignmentInputFile"); //"Input File: ";
			
			this.lblAutoGradeAssignmentOutputFile.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblAutoGradeAssignmentOutputFile"); //"Output File: ";
			this.lblAutoGradeAssignmentGradeType.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblAutoGradeAssignmentGradeType"); //"Grade Type: ";
			this.lblAutoCompileHeader.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblAutoBuildHeader"); //"AUTO COMPILE";
			this.chkAutoCompileAssignmentAutompileOn.Text = SharedSupport.GetLocalizedString("AddEditAssignment_chkAutoCompileAssignmentAutompileOn"); //"Auto Compile On";
			
			//this.lblAutoCompileAssignmentMakeFile.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblAutoCompileAssignmentMakeFile");//"Make File: ";
			this.lblAutoCompileAssignmentCompileType.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblAutoCompileAssignmentCompileType");//"Compile Type: ";
			this.lblUpdatedProject.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblUpdatedProject");
			this.lblPastDueDays.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblPastDueDays");
			this.lblPastDueNotice.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblPastDueNotice");
			this.lblReminderNotice.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblReminderNotice");
			this.lblReminderDay.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblReminderDays");
			this.lblNotificationHeader.Text = SharedSupport.GetLocalizedString("AddEditAssignment_NotificationHeader");
			//this.btnUpdate2.Text = SharedSupport.GetLocalizedString("AddEditAssignment_btnUpdate2"); //"Update";
			//this.btnCancel2.Text = SharedSupport.GetLocalizedString("AddEditAssignment_btnCancel2"); //"Cancel";
			this.txtGeneralAssignmentHomePageURL.Text = String.Empty;
			this.lblGeneralAssignmentHomePageURL.Text = SharedSupport.GetLocalizedString("AddEditAssignment_lblGeneralAssignmentHomePageURL"); 
			//this.lblCompiledFileName.Text = SharedSupport.GetLocalizedString("AddEditAssignment_CompiledFileNameLabel");//"lblCommandLineArgs2";//SharedSupport.GetLocalizedString("AddEditAssignment_CompiledFileNameLabel");
			this.lblCommandLineArgs.Text = SharedSupport.GetLocalizedString("AddEditAssignment_CommandLineArgsLabel");//"lblCommandLineArgs3";//SharedSupport.GetLocalizedString("AddEditAssignment_CommandLineArgsLabel");
			this.txtReminderDays.Text = SharedSupport.GetLocalizedString("AddEditAssignment_DefaultDaysBeforeDue");
			this.txtPastDueDays.Text = SharedSupport.GetLocalizedString("AddEditAssignment_DefaultDaysAfterDue");
			this.lblRequired.Text = SharedSupport.GetLocalizedString("Global_RequiredFieldIndicator");
/*			this.validateDate.ValueToCompare= Convert.ToString(System.DateTime.Today);
			this.validateDate.Operator = System.Web.UI.WebControls.ValidationCompareOperator.GreaterThan;
			this.validateDate.ErrorMessage =  SharedSupport.GetLocalizedString("AddEditAssignment_DateTime_Invalid");
			this.validateDate1.ErrorMessage = SharedSupport.GetLocalizedString("AddEditAssignment_DateTime_Exception");
*/		
			btnDelete.Value = SharedSupport.GetLocalizedString("AddEditAssignment_Delete");
			//Fill time combo
			//AM
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_12:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_12:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_1:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_1:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_2:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_2:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_3:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_3:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_4:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_4:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_5:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_5:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_6:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_6:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_7:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_7:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_8:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_8:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_9:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_9:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_10:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_10:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_11:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_11:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_AM"));
			//PM
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_12:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_12:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_1:00") + " "   + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_1:30") + " "   + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_2:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_2:30") + " "   + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_3:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_3:30") + " " + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_4:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_4:30") + " " + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_5:00") + " " + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_5:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_6:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_6:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_7:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_7:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_8:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_8:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_9:00") + " "  +  SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_9:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_10:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_10:30") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_11:00") + " "  + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
			this.cboTime.Items.Add(SharedSupport.GetLocalizedString("Calendar_11:30") + " " + SharedSupport.GetLocalizedString("Calendar_Time_PM"));
		}

		private void hideNotifications()
		{
			lblNotificationHeader.Visible = false;
			chkSendReminder.Visible = false;
			chkPastDueNotice.Visible = false;
			chkUpdatedProjectNotice.Visible = false;
			lblReminderNotice.Visible = false;
			lblReminderDay.Visible = false;
			lblPastDueNotice.Visible = false;
			lblPastDueDays.Visible = false;
			lblUpdatedProject.Visible = false;
			txtReminderDays.Visible = false;
			txtPastDueDays.Visible = false;
			validateReminderDays.Visible = false;
			validPastDueDays.Visible = false;


		}

		private void calDueDate_SelectionChanged(object sender, System.EventArgs e)
		{
			//update DueDate Textbox
			txtDueDate.Text = calDueDate.SelectedDate.ToShortDateString();
		}
	}
}