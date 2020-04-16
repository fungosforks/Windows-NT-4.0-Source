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
	using Microsoft.VisualStudio.Academic.AssignmentManager;

    /// <summary>
    ///    Summary description for Assignments.
    /// </summary>
    public class Submissions : System.Web.UI.Page
    {
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected System.Web.UI.WebControls.DataList dlUserAssignments;
		protected System.Web.UI.WebControls.CheckBox chkItemRerun;
		protected System.Web.UI.WebControls.TextBox txtUserAssignmentID;
		protected System.Web.UI.WebControls.Button btnAutoCompile;
		protected System.Web.UI.WebControls.Button btnAutoGrade;
		protected System.Web.UI.WebControls.Label lblCompleted;
		protected System.Web.UI.WebControls.Label lblPending;
		protected System.Web.UI.WebControls.Label lblFailed;
		protected System.Web.UI.WebControls.Label lblSubmissions;

		protected AssignmentManager.UserControls.faculty Nav1;
		protected AssignmentManager.UserControls.goBack GoBack1;

		private string _INVALID_COURSEID_ERROR = SharedSupport.GetLocalizedString("FacultySubmissions_INVALID_COURSEID_ERROR"); //"There was no record corresponding to the passed CourseID.";
		private string _INVALID_ASSIGNMENT_ID_ERROR = SharedSupport.GetLocalizedString("FacultySubmissions_INVALID_ASSIGNMENT_ID_ERROR"); //"Invalid AssignmentID.  Assignment does not exist.";
		public string Title = SharedSupport.GetLocalizedString("AM_Title");
		// persist querystring parameters instead of referencing Request object every time needed
		public int courseId;
		public int assignmentId;
		
		//DataList Header Localization
		public string Student_Text_String = SharedSupport.GetLocalizedString("FacultySubmissions_Student_Text_String");
		public string Date_Last_Submitted_Text_String = SharedSupport.GetLocalizedString("FacultySubmissions_Date_Last_Submitted_Text_String");
		public string Auto_Compile_Text_String = SharedSupport.GetLocalizedString("FacultySubmissions_Auto_Compile_Text_String");
		public string Auto_Grade_Text_String = SharedSupport.GetLocalizedString("FacultySubmissions_Auto_Grade_Text_String");
		public string Grade_Text_String = SharedSupport.GetLocalizedString("FacultySubmissions_Grade_Text_String");
		public string Get_Submission_Text_String = SharedSupport.GetLocalizedString("FacultySubmissions_Get_Submission_Text_String");
		public readonly string Submissions_AMTitle = SharedSupport.GetLocalizedString("Submissions_AMTitle");
		public Submissions()
		{
			Page.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			try
			{
				Response.Cache.SetNoStore();

				Nav1.Feedback.Text =  String.Empty;
				Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT;
				Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_COURSE_ASSIGNMENTS;
				Nav1.relativeURL = @"../";

				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vsoriUsingAssignmentManager");
				GoBack1.GoBackIncludeBack = true;
				GoBack1.GoBack_left = "400px";
				GoBack1.GoBack_top = "-5px";
				GoBack1.GoBack_BackURL = "Assignments.aspx?" + Request.QueryString.ToString();

				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
				// grab CourseID parameter from the querystring
				courseId = func.ValidateNumericQueryStringParameter(this.Request, "CourseID");
				
				if(courseId <= 0)
				{
					throw(new ArgumentException(SharedSupport.GetLocalizedString("Global_MissingParameter")));					
				}
				
				// grab assignmentID from querystring
				assignmentId = func.ValidateNumericQueryStringParameter(this.Request, "AssignmentID");

				//Check Security Permissions
				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USERASSIGNMENT_VIEW))
				{
					// Note that Redirect ends page execution.
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
				}
				
				Nav1.Feedback.Text =  String.Empty;

				if (!IsPostBack)
				{
					//
					// Evals true first time browser hits the page
					//

					//Get localization string for all text displayed on the page
					LocalizeLabels();				

					// refresh user assignments datalist
					userAssignmentsRefresh();
				}

			}	
			catch(Exception ex)
			{
				Nav1.Feedback.Text =  ex.Message;
			}
            
		}
		
        protected void Page_Init(object sender, EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP+ Windows Form Designer.
            //
            InitializeComponent();
        }

		public void btnAutoGrade_Click (object sender, System.EventArgs e)
		{
			try
			{
				//make sure the user is allowed to do this
				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USERASSIGNMENT_EDIT))
				{
					// Note that Redirect ends page execution.
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
				}

				int[] userAssignmentIds = getCheckedUserAssignmentIdsFromDataList();

				if(userAssignmentIds.Length<1)
				{
					Nav1.Feedback.Text =  SharedSupport.GetLocalizedString("Submissions_NoSelectionMade");
					return;
				}

				//call Auto Check for each UserAssignmentID
				for(int i=0;i < userAssignmentIds.Length;i++)
				{
					StudentAssignmentM.SendActionToQueue(userAssignmentIds[i], false, true);
				}

				// refresh user assignments data list
				userAssignmentsRefresh();

				Nav1.Feedback.Text = SharedSupport.GetLocalizedString("FacultySubmissions_CheckSubmitSuccessful");

			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text =  ex.Message;
			}
		}
		public void btnAutoCompile_Click (object sender, System.EventArgs e)
		{
			try
			{
				//make sure the user is allowed to do this
				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USERASSIGNMENT_EDIT))
				{
					// Note that Redirect ends page execution.
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
				}

				int[] userAssignmentIds = getCheckedUserAssignmentIdsFromDataList();

				if(userAssignmentIds.Length<1)
				{
					Nav1.Feedback.Text =  SharedSupport.GetLocalizedString("Submissions_NoSelectionMade");
					return;
				}

				//call Auto Compile for each UserAssignmentID
				for(int i=0;i < userAssignmentIds.Length;i++)
				{
					StudentAssignmentM.SendActionToQueue(userAssignmentIds[i], true, false);
				}

				// refresh user assignments data list
				userAssignmentsRefresh();

				Nav1.Feedback.Text = SharedSupport.GetLocalizedString("FacultySubmissions_BuildSubmitSuccessful");
				
			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text =  ex.Message;
			}
		}

		private void blankForm()
		{
			this.btnAutoCompile.Visible = false;
			this.btnAutoGrade.Visible = false;
			// CJH 2/19/2001 removed from scope this.btnCheatDetect.Visible = false;
		}
		private int[] getCheckedUserAssignmentIdsFromDataList()
		{
			// instantiate textbox, checkbox objects; array, loop variables
			System.Web.UI.WebControls.CheckBox chk = new System.Web.UI.WebControls.CheckBox();
			System.Web.UI.WebControls.TextBox txtbx = new System.Web.UI.WebControls.TextBox();
			int[] workArray = new int[this.dlUserAssignments.Items.Count];
			int i = 0;

			foreach(System.Web.UI.Control ctrl in this.dlUserAssignments.Items)
			{
				// create references to checkbox, textbox objects
				chk = (System.Web.UI.WebControls.CheckBox) ctrl.Controls[0];
				txtbx = (System.Web.UI.WebControls.TextBox) ctrl.Controls[1];

				// add UserAssignmentID to array if checked
				if (chk.Checked && txtbx.Text != String.Empty){workArray[i++] = Convert.ToInt32(txtbx.Text);}
			}		
	
			// re-dimension array to correct size
			int[] returnArray = new int[i];
			Array.Copy(workArray, 0, returnArray, 0, i);

			return returnArray;

		}
        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
		{
			btnAutoGrade.Click += new System.EventHandler (this.btnAutoGrade_Click);
			btnAutoCompile.Click += new System.EventHandler (this.btnAutoCompile_Click);
			// CJH 2/19/2001 removed from scope btnCheatDetect.Click += new System.EventHandler (this.btnCheatDetect_Click);
			this.Load += new System.EventHandler (this.Page_Load);
		}

		protected void LocalizeLabels()
		{
			Nav1.Feedback.Text =  String.Empty;
			if(!assignmentId.Equals(null))
			{
				AssignmentM assign = AssignmentM.Load(assignmentId);
				if(assign.IsValid)
				{
					Nav1.SubTitle = SharedSupport.GetLocalizedString("FacultySubmissions_SubTitle") + assign.ShortName;
				}
			}
			this.btnAutoCompile.Text = SharedSupport.GetLocalizedString("FacultySubmissions_btnAutoBuild"); //"Re-run Auto Compile";
			this.btnAutoGrade.Text = SharedSupport.GetLocalizedString("FacultySubmissions_btnAutoGrade"); //"Re-run Auto Grade";
			this.lblCompleted.Text = SharedSupport.GetLocalizedString("FacultySubmissions_lblCompleted");
			this.lblFailed.Text = SharedSupport.GetLocalizedString("FacultySubmissions_lblFailed");
			this.lblPending.Text = SharedSupport.GetLocalizedString("FacultySubmissions_lblPending");
			this.lblSubmissions.Text = SharedSupport.GetLocalizedString("FacultySubmissions_lblSubmissions");
		}

		private void userAssignmentsRefresh()
		{

			//Check to make sure that there was a Assignment ID on the query string
			if(!assignmentId.Equals(0))
			{
				//Load all user assignments for the given assignmentID
				AssignmentList assignList = AssignmentList.GetSubmissionsForAssignment(assignmentId);
				//Make sure that you got at least one row back
				if(assignList.Count > 0)
				{
					//cycle through all the rows and set n/a
					for(int i=0;i<assignList.Count; i++)
					{
						if(assignList.GetOverallGradeForItem(i) == "")		
						{
							assignList.SetOverallGradeForItem( i, SharedSupport.GetLocalizedString("FacultySubmissions_n/a") );
						}
					}
					//Populate DataList
					dlUserAssignments.DataSource = assignList.GetDataSource(Server);
					dlUserAssignments.DataBind();
					dlUserAssignments.Visible = true;
				}
				else
				{
					//The assignment for the given AssignmentID did not exist.
					blankForm();
					throw new Exception(_INVALID_ASSIGNMENT_ID_ERROR);
				}
			}
			else
			{
				//The assignment for the given AssignmentID did not exist.
				blankForm();
				throw new Exception(_INVALID_ASSIGNMENT_ID_ERROR);
			}
		}
    }
}
