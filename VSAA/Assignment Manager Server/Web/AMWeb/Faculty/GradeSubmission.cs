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
	using System.Security;

	using Microsoft.VisualStudio.Academic.AssignmentManager;

    /// <summary>
    ///    Summary description for GradeDetail.
    /// </summary>
    public class GradeSubmission : System.Web.UI.Page
    {
		protected System.Web.UI.WebControls.TextBox txtAutoGradeDetails;
		protected System.Web.UI.WebControls.Label lblAutoGradeDetailLabel;
		protected System.Web.UI.WebControls.TextBox txtAutoGradeResult;
		protected System.Web.UI.WebControls.Label lblAutoGradeResultLabel;
		protected System.Web.UI.WebControls.TextBox txtAutoGradeDate;
		protected System.Web.UI.WebControls.Label lblAutoGradeDateLabel;
		protected System.Web.UI.WebControls.Label lblAutoGradeDetailsLabel;
		protected System.Web.UI.WebControls.TextBox txtCompileDetails;
		protected System.Web.UI.WebControls.Label lblCompileDetailLabel;
		protected System.Web.UI.WebControls.Label lblCompileResult;
		protected System.Web.UI.WebControls.Label lblCompileResultLabel;
		protected System.Web.UI.WebControls.Label lblCompileDate;
		protected System.Web.UI.WebControls.Label lblCompileDateLabel;
		protected System.Web.UI.WebControls.Label lblCompileDetailsLabel;
		protected System.Web.UI.WebControls.Button btnSaveGrade;
		protected System.Web.UI.WebControls.TextBox txtComments;
		protected System.Web.UI.WebControls.Label lblCommentsLabel;
		protected System.Web.UI.WebControls.TextBox txtGrade;
		protected System.Web.UI.WebControls.Label lblGradeLabel;
		protected System.Web.UI.WebControls.Label lblDateSubmitted;
		protected System.Web.UI.WebControls.Label lblDateSubmittedLabel;
		protected System.Web.UI.WebControls.Label lblAssignment;
		protected System.Web.UI.WebControls.Label lblAssignmentLabel;
		protected System.Web.UI.WebControls.Label lblStudent;
		protected System.Web.UI.WebControls.Label lblStudentLabel;
		protected System.Web.UI.HtmlControls.HtmlForm frmGrade;

		protected AssignmentManager.UserControls.faculty Nav1;
		protected AssignmentManager.UserControls.goBack GoBack1;

		// persist querystring parameters instead of referencing Request object every time needed
		private int courseId;
		private int studentID;
		private int assignmentId;
		protected System.Web.UI.HtmlControls.HtmlForm formGrade;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divAutoGradeResults;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divCompileResults;
		protected System.Web.UI.WebControls.Button btnCancel;
		protected System.Web.UI.WebControls.Button btnAutoCompile;
		protected System.Web.UI.WebControls.Button btnAutoGrade;

		protected string Title = SharedSupport.GetLocalizedString("AM_Title");
		
		public GradeSubmission()
		{
			Page.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			try
			{
				// Do not cache this page
				Response.Cache.SetNoStore();

				Nav1.Feedback.Text = String.Empty;
				Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT;
				Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_COURSE_ASSIGNMENTS;
				Nav1.relativeURL = @"../";
				
				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vsoriUsingAssignmentManager");
				GoBack1.GoBackIncludeBack = true;
				GoBack1.GoBack_BackURL = "Submissions.aspx?" + "CourseID=" + Request.QueryString["CourseID"].ToString() + "&" + "AssignmentID=" + Request.QueryString["AssignmentID"].ToString();
				
				// grab CourseID parameter from the querystring
				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
				courseId = func.ValidateNumericQueryStringParameter(this.Request, "CourseID");
				
				if(courseId.Equals(null))
				{
					// Note that Redirect ends page execution.
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_MissingParameter");
				}

				// get UserAssignmentID, AssignmentID querystring param
				studentID = func.ValidateNumericQueryStringParameter(this.Request, "UserID");
				assignmentId = func.ValidateNumericQueryStringParameter(this.Request, "AssignmentID");

				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USERASSIGNMENT_VIEW))
				{
					// Note that Redirect ends page execution.
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
				}

				if(studentID == 0)
				{
					//If userassignmentId missing then doesn't make sense to show page at all?
					throw(new ArgumentException(SharedSupport.GetLocalizedString("Global_MissingParameter")));	
				}
				
				LocalizeLabels();

				if (!Page.IsPostBack) 
				{
					// load the details
					userAssignmentDetailRefresh();
				}

			}
			catch(System.Exception ex)
			{
				Nav1.Feedback.Text =  ex.Message.ToString();
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
			this.btnAutoCompile.Click += new System.EventHandler(this.btnAutoCompile_Click);
			this.btnAutoGrade.Click += new System.EventHandler(this.btnAutoGrade_Click);
			this.btnSaveGrade.Click += new System.EventHandler(this.btnSaveGrade_Click);
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}

		public void btnSaveGrade_Click (object sender, System.EventArgs e)
		{
			try
			{
				// Check permissions.
				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USERASSIGNMENT_EDIT))
				{
					throw new Exception(SharedSupport.GetLocalizedString("Global_Unauthorized"));
				}
				
				Nav1.Feedback.Text = String.Empty;

				if(studentID != 0)
				{
					StudentAssignmentM assignment = StudentAssignmentM.Load(studentID, assignmentId);
					if(assignment != null)
					{
						assignment.GradeComments = txtComments.Text.ToString();
						assignment.OverallGrade = txtGrade.Text.ToString();
						assignment.LastUpdatedDate = DateTime.Now;

						//Save the updated User Assignment
						assignment.Update();
						Response.Redirect("Submissions.aspx?CourseID=" + Request.QueryString.Get("CourseID") + "&AssignmentID=" + Request.QueryString.Get("AssignmentID"), false);
					}
				}
				else
				{
					throw new Exception(SharedSupport.GetLocalizedString("FacultyGradeSubmission_InvalidUserAssignmentID") + Request.QueryString.Get("UserAssignmentID").ToString()); 
				}
			}
			catch(System.Exception ex)
			{
				Nav1.Feedback.Text =  ex.Message.ToString();
			}
		}

		public void btnAutoGrade_Click (object sender, System.EventArgs e)
		{
			try
			{

				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USERASSIGNMENT_EDIT))
				{
					throw new Exception(SharedSupport.GetLocalizedString("Global_Unauthorized"));
				}

				StudentAssignmentM sa = StudentAssignmentM.Load(studentID, assignmentId);
				sa.SendActionToQueue(false, true);
				
				userAssignmentDetailRefresh();
				Nav1.Feedback.Text = SharedSupport.GetLocalizedString("FacultySubmissions_CheckSubmitSuccessful");
			}
			catch
			{
				Nav1.Feedback.Text =  SharedSupport.GetLocalizedString("FacultyGradeSubmission_InvalidUserAssignmentID");
			}
		}
		public void btnAutoCompile_Click (object sender, System.EventArgs e)
		{
			try
			{
				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USERASSIGNMENT_EDIT))
				{
					throw new Exception(SharedSupport.GetLocalizedString("Global_Unauthorized"));
				}

				StudentAssignmentM sa = StudentAssignmentM.Load(studentID, assignmentId);
				sa.SendActionToQueue(true, false);
				
				userAssignmentDetailRefresh();
				Nav1.Feedback.Text = SharedSupport.GetLocalizedString("FacultySubmissions_BuildSubmitSuccessful");
			}
			catch
			{
				Nav1.Feedback.Text =  SharedSupport.GetLocalizedString("FacultyGradeSubmission_InvalidUserAssignmentID");
			}
		}


		protected void LocalizeLabels()
		{
			//general
			Nav1.Feedback.Text =  String.Empty;
			this.lblAssignmentLabel.Text = SharedSupport.GetLocalizedString("FacultyGradeSubmission_lblAssignmentLabel "); //"Assignment: ";
			this.lblAssignment.Text = String.Empty;
			this.lblDateSubmittedLabel.Text = SharedSupport.GetLocalizedString("FacultyGradeSubmission_lblDateSubmittedLabel"); //"Date Submitted: ";
			this.lblDateSubmitted.Text = String.Empty;
			this.lblGradeLabel.Text = SharedSupport.GetLocalizedString("FacultyGradeSubmission_lblGradeLabel"); //"Grade: ";
			this.lblCommentsLabel.Text = SharedSupport.GetLocalizedString("FacultyGradeSubmission_lblCommentsLabel"); //"Comments: ";

			//compile
			this.lblCompileDetailsLabel.Text = SharedSupport.GetLocalizedString("FacultyGradeSubmission_lblBuildDetailsLabel1"); //"Compile Details";
			this.lblCompileResultLabel.Text	= SharedSupport.GetLocalizedString("FacultyGradeSubmission_lblCompileResultLabel"); //"Result: ";
			this.lblCompileResult.Text = String.Empty;
			this.lblCompileDateLabel.Text = SharedSupport.GetLocalizedString("FacultyGradeSubmission_lblBuildDateLabel"); //"Compile Date: ";
			this.lblCompileDate.Text = String.Empty;
			this.lblCompileDetailLabel.Text = SharedSupport.GetLocalizedString("FacultyGradeSubmission_lblCompileDetailLabel"); //"Detail: ";
			this.txtCompileDetails.Text = String.Empty;
			this.txtCompileDetails.ReadOnly = true;
			this.txtCompileDetails.Enabled = true;
			this.btnAutoCompile.Text = SharedSupport.GetLocalizedString("FacultySubmissions_btnAutoCompile"); //"Re-run Auto Compile";
			
			//auto grade
			this.lblAutoGradeDetailsLabel.Text = SharedSupport.GetLocalizedString("FacultyGradeSubmission_lblAutoGradeDetailsLabel"); //"Auto Grade Details";
			this.lblAutoGradeResultLabel.Text = SharedSupport.GetLocalizedString("FacultyGradeSubmission_lblAutoGradeResultLabel"); //"Result: ";
			this.txtAutoGradeResult.Text = String.Empty;
			this.txtAutoGradeResult.ReadOnly = true;
			this.txtAutoGradeResult.Enabled = true;
			this.lblAutoGradeDateLabel.Text = SharedSupport.GetLocalizedString("FacultyGradeSubmission_lblAutoGradeDateLabel"); //"AutoGrade Date: ";
			this.txtAutoGradeDate.Text = String.Empty;
			this.lblAutoGradeDetailLabel.Text = SharedSupport.GetLocalizedString("FacultyGradeSubmission_lblAutoGradeDetailLabel"); //"Detail: ";
			this.txtAutoGradeDetails.Text = String.Empty;
			this.txtAutoGradeDetails.ReadOnly = true;
			this.txtAutoGradeDetails.Enabled = true;
			this.btnAutoGrade.Text = SharedSupport.GetLocalizedString("FacultySubmissions_btnAutoGrade"); //"Re-run Auto Grade";
			
			//user details
			this.lblStudentLabel.Text = SharedSupport.GetLocalizedString("FacultyGradeSubmission_lblStudentLabel"); //"Student Name: ";
			this.lblStudent.Text = String.Empty;

            this.btnSaveGrade.Text = SharedSupport.GetLocalizedString("FacultyGradeSubmission_btnSaveGrade");
            this.btnCancel.Text = SharedSupport.GetLocalizedString("FacultyGradeSubmission_btnCancel");
		}

		private void userAssignmentDetailRefresh()
		{
			StudentAssignmentM sa = StudentAssignmentM.Load(studentID,assignmentId);
			if(sa != null)
			{
				txtGrade.Text = sa.OverallGrade;
				txtComments.Text = sa.GradeComments;
				lblDateSubmitted.Text = Server.HtmlEncode(sa.LastSubmitDate.ToShortDateString() + " " + sa.LastSubmitDate.ToShortTimeString());
				
				//Populate Auto-Compile Section
				lblCompileDate.Text = sa.LastUpdatedDate.ToShortDateString() + " " + sa.LastUpdatedDate.ToShortTimeString();
				txtCompileDetails.Text = sa.BuildDetails;
				lblCompileResult.Text = Server.HtmlEncode(sa.BuildResultCode);

				//Populate Auto-Grade Section
				txtAutoGradeDate.Text = sa.LastUpdatedDate.ToShortDateString() + " " + sa.LastUpdatedDate.ToShortTimeString();
				txtAutoGradeDetails.Text = sa.CheckDetails;
				txtAutoGradeResult.Text = sa.CheckResultCode;
				
				//Grab assignment information based on loaded assignmentID				
				AssignmentM assign = AssignmentM.Load(assignmentId);
				if(assign.IsValid)
				{
					lblAssignment.Text = Server.HtmlEncode(assign.ShortName);
				}
				else
				{
					string[] AssignmentID = new string[] {assignmentId.ToString()};
					throw new Exception(SharedSupport.GetLocalizedString("FacultyGradeSubmission_InvalidAssignmentID", AssignmentID));
				}

				//Grab user information based on loaded userID							
				UserM user = UserM.Load(studentID);
				if(user.IsValid)
				{
					this.lblStudent.Text = Server.HtmlEncode( user.LastName + SharedSupport.GetLocalizedString("FacultyGradeSubmission_Comma") + user.FirstName);
				}
				else
				{
					Nav1.Feedback.Text = SharedSupport.GetLocalizedString("FacultyGradeSubmission_NoUserIDFound"); //"No UserID found. ";
				}
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			// Go Back to Submissions page.
			Response.Redirect("Submissions.aspx?CourseID=" + Request.QueryString.Get("CourseID") + "&AssignmentID=" + Request.QueryString.Get("AssignmentID"), false);
		}
    }
}
