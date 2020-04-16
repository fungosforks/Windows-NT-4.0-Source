//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//

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
using System.Security;

namespace Microsoft.VisualStudio.Academic.AssignmentManager.Student
{
	/// <summary>
	/// Summary description for AssignmentGrade.
	/// </summary>
	public class AssignmentGrade : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.TextBox lblAutoGradeDetails;
		protected System.Web.UI.WebControls.Label lblAutoGradeDetailLabel;
		protected System.Web.UI.WebControls.Label lblAutoGradeResult;
		protected System.Web.UI.WebControls.Label lblAutoGradeResultLabel;
		protected System.Web.UI.WebControls.Label lblAutoGradeDate;
		protected System.Web.UI.WebControls.Label lblAutoGradeDateLabel;
		protected System.Web.UI.WebControls.Label lblAutoGradeDetailsLabel;
		protected System.Web.UI.WebControls.TextBox lblCompileDetails;
		protected System.Web.UI.WebControls.Label lblCompileDetailLabel;
		protected System.Web.UI.WebControls.Label lblCompileResult;
		protected System.Web.UI.WebControls.Label lblCompileResultLabel;
		protected System.Web.UI.WebControls.Label lblCompileDate;
		protected System.Web.UI.WebControls.Label lblCompileDateLabel;
		protected System.Web.UI.WebControls.Label lblCompileDetailsLabel;
		protected System.Web.UI.WebControls.TextBox lblComments;
		protected System.Web.UI.WebControls.Label lblCommentsLabel;
		protected System.Web.UI.WebControls.Label lblGrade;
		protected System.Web.UI.WebControls.Label lblGradeLabel;
		protected System.Web.UI.WebControls.Label lblDateSubmitted;
		protected System.Web.UI.WebControls.Label lblDateSubmittedLabel;
		protected System.Web.UI.WebControls.Label lblAssignment;
		protected System.Web.UI.WebControls.Label lblAssignmentLabel;
		protected System.Web.UI.WebControls.Label lblDueDate;
		protected System.Web.UI.WebControls.Label lblAssignmentWebPage;
		protected System.Web.UI.WebControls.Label lblDescription;
		protected System.Web.UI.WebControls.TextBox txtDescription;
		protected System.Web.UI.WebControls.Label lblDueDateValue;
		protected System.Web.UI.WebControls.HyperLink hlAssignmentWebPage;
		
		
		protected AssignmentManager.UserControls.student Nav1;
		protected AssignmentManager.UserControls.goBack GoBack1;
		
		// persist querystring parameters instead of referencing Request object every time needed
		private int courseId;
		private int assignmentId;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divCompileResults;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divAutoGradeResults;

		public string Title = SharedSupport.GetLocalizedString("AM_Title");
		
		public AssignmentGrade()
		{
			Page.Init += new System.EventHandler(Page_Init);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				// grab CourseID parameter from the querystring
				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
				courseId = func.ValidateNumericQueryStringParameter(this.Request, "CourseID");

				UserM user = UserM.Load(SharedSupport.GetUserIdentity());
				if (!user.IsInCourse(courseId))
				{
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");	
				}

				// Do not cache this page
				Response.Cache.SetNoStore();

				Nav1.Feedback.Text = String.Empty;
				Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_STUDENT_COURSE;
				Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_STUDENT_COURSE_ASSIGNMENTS;
				Nav1.relativeURL = @"../";
				
				//GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vstskCheckingAssignmentStatus");
				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("tskUsingAssignmentManagerToCheckAssignmentStatus");
				GoBack1.GoBackIncludeBack = true;
				GoBack1.GoBack_BackURL = "Assignments.aspx?" + Request.QueryString.ToString();
				GoBack1.GoBack_left = "-105px";
				if(Request.QueryString.Get("Exp") == "1")
				{
					txtDescription.CssClass = "infoTextDisabled";
				}
				else
				{
					txtDescription.CssClass = "invisible";
				}
			
				if(courseId > 0)
				{
				//returns the course name to be displayed in the Nav bar title
					CourseM course = CourseM.Load(courseId);
					Nav1.Title = Server.HtmlEncode(course.Name);
				}
				else
				{
					Nav1.Feedback.Text = SharedSupport.GetLocalizedString("Global_MissingParameter");					
				}		
				// grab assignmentId from querystring
				assignmentId = func.ValidateNumericQueryStringParameter(this.Request, "assignmentId");

				if (!IsPostBack)
				{
					//
					// Evals true first time browser hits the page
					//
				}
				int userID = SharedSupport.GetUserIdentity();			
		
				LocalizeLabels();

				//checks that assignmentId is not empty then loops through 
				if (assignmentId.Equals(null))
				{
					Nav1.Feedback.Text = SharedSupport.GetLocalizedString("GradeDetail_InvalidassignmentIdError");
				}
				else
				{	
					AssignmentM assign = AssignmentM.Load(assignmentId);
					if(assign.IsValid)
					{
						txtDescription.Text = assign.Description;
						lblAssignment.Text = Server.HtmlEncode(assign.ShortName);
						lblDueDateValue.Text = assign.DueDate.ToShortDateString();
						hlAssignmentWebPage.NavigateUrl = assign.AssignmentURL;
						hlAssignmentWebPage.Text = Server.HtmlEncode(assign.AssignmentURL);
					}

					StudentAssignmentM stuAssign = StudentAssignmentM.Load(userID, assignmentId);
					if(stuAssign == null)
					{
						this.lblAssignment.Text = SharedSupport.GetLocalizedString("GradeDetail_NoDetailsAvailable");
					}
					else
					{
						//if data is returned, regardless of the Detail Type the LocalizeGeneralLabels
						//generates text for the header labels.
						LocalizeGeneralLabels(stuAssign);					
						LocalizeAutoBuildLabels(stuAssign);
						LocalizeAutoGradeLabels(stuAssign);
					}
				}
			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text = ex.Message.ToString();
			}
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Windows Form Designer.
			//
			InitializeComponent();
		}

		protected void LocalizeGeneralLabels(StudentAssignmentM assignment)
		{			
			this.lblDateSubmitted.Text = Server.HtmlEncode( assignment.LastSubmitDate.ToShortDateString() );
			this.lblGrade.Text = Server.HtmlEncode(assignment.OverallGrade.ToString());
			this.lblComments.Text = assignment.GradeComments.ToString();
		}	
		
		private void LocalizeLabels()
		{
			this.Nav1.Feedback.Text = "";
			this.lblAssignmentLabel.Text = SharedSupport.GetLocalizedString("GradeDetail_Assignment");
			this.lblDateSubmittedLabel.Text = SharedSupport.GetLocalizedString("GradeDetail_DateSubmitted");
			this.lblGradeLabel.Text = SharedSupport.GetLocalizedString("GradeDetail_Grade");
			this.lblCommentsLabel.Text = SharedSupport.GetLocalizedString("GradeDetail_Comments");
			this.lblCompileDetailsLabel.Text = SharedSupport.GetLocalizedString("GradeDetail_BuildDetails1");
			this.lblAutoGradeDetailsLabel.Text = SharedSupport.GetLocalizedString("GradeDetail_AutoGradeDetails");			
			this.lblAssignmentWebPage.Text = SharedSupport.GetLocalizedString("AssignmentGrade_AssignmentWebPage");
			this.lblDescription.Text = SharedSupport.GetLocalizedString("AssignmentGrade_Description");
			this.lblDueDate.Text = SharedSupport.GetLocalizedString("AssignmentGrade_DueDate");
		}

		//AutoCompile related fields.  Only visible when AutoCompile details are present
		protected void LocalizeAutoBuildLabels(StudentAssignmentM assignment)
		{
			this.lblCompileResultLabel.Text	= SharedSupport.GetLocalizedString("GradeDetail_Result");
			this.lblCompileResult.Text = Server.HtmlEncode( assignment.BuildResultCode );
			this.lblCompileDateLabel.Text = SharedSupport.GetLocalizedString("GradeDetail_Date");
			this.lblCompileDate.Text = Server.HtmlEncode( assignment.LastUpdatedDate.ToShortDateString() );
			this.lblCompileDetailLabel.Text = SharedSupport.GetLocalizedString("GradeDetail_Detail");
			this.lblCompileDetails.Text = assignment.BuildDetails;
		}

		//AutoGrade related fields.  Only visible when AutoGrade details are present
		protected void LocalizeAutoGradeLabels(StudentAssignmentM assignment)
		{
			this.lblAutoGradeResultLabel.Text	= SharedSupport.GetLocalizedString("GradeDetail_Result");
			this.lblAutoGradeResult.Text = Server.HtmlEncode(assignment.CheckResultCode);
			this.lblAutoGradeDateLabel.Text = SharedSupport.GetLocalizedString("GradeDetail_Date");

			this.lblAutoGradeDate.Text = Server.HtmlEncode( assignment.LastUpdatedDate.ToShortDateString() );
			this.lblAutoGradeDetailLabel.Text = SharedSupport.GetLocalizedString("GradeDetail_Detail");
			this.lblAutoGradeDetails.Text = assignment.CheckDetails;
					
		}

		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
