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
	/// Summary description for Assignments.
	/// </summary>
	public class Assignments : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lblSubmitProject;	
		protected System.Web.UI.WebControls.Label lblLastSubmitDate;
		protected System.Web.UI.WebControls.Label lblGrade;
		protected System.Web.UI.WebControls.Label lblGetStarter;
		protected System.Web.UI.WebControls.Label lblDueDate;
		protected System.Web.UI.WebControls.Label lblAutoGradeStatus;
		protected System.Web.UI.WebControls.Label lblAutoCompileStatus;
		protected System.Web.UI.WebControls.Label lblAssignment;
		protected System.Web.UI.WebControls.DataList dlAssignments;
		protected System.Web.UI.WebControls.Table tblAssignments;
		protected System.Web.UI.WebControls.HyperLink linkAutoGrade;
		protected System.Web.UI.WebControls.HyperLink linkAutoCompile;
		protected System.Web.UI.WebControls.Label lblCompleted;
		protected System.Web.UI.WebControls.Label lblPending;
		protected System.Web.UI.WebControls.Label lblFailed;

		protected System.Web.UI.WebControls.HyperLink hlStarterAvailable;
		protected System.Web.UI.WebControls.Image imgStarterNotAvailable;

		protected AssignmentManager.UserControls.student Nav1;
		protected AssignmentManager.UserControls.goBack GoBack1;

		public string assignments = SharedSupport.GetLocalizedString("Assignments_AssignmentHeader");
		public string dueDate = SharedSupport.GetLocalizedString("Assignments_DueDateHeader");
		public string dateSubmitted = SharedSupport.GetLocalizedString("Assignments_SubmitDateHeader");
		public string autoCompile = SharedSupport.GetLocalizedString("Assignments_AutoBuildHeader");
		public string autoGrade = SharedSupport.GetLocalizedString("Assignments_AutoGradeHeader");
		// CJH 2/19/2001 removed from scope public string cheatDetect = SharedSupport.GetLocalizedString("Assignments_CheatDetectHeader");
		public string grade = SharedSupport.GetLocalizedString("Assignments_GradeHeader");
		public string getStarter = SharedSupport.GetLocalizedString("Assignments_GetStarterHeader");
		public string submitProject = SharedSupport.GetLocalizedString("Assignments_SubmitProjectHeader");
		public string Title = SharedSupport.GetLocalizedString("AM_Title");
		
		// persist querystring parameters instead of referencing Request object every time needed
		public int courseId;
		protected System.Web.UI.WebControls.Label lblCourseName;
	
		public Assignments()
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

				Nav1.Feedback.Text = String.Empty;
				Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_STUDENT_COURSE;
				Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_STUDENT_COURSE_ASSIGNMENTS;
				Nav1.relativeURL = @"../";
				
				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vstskCheckingAssignmentStatus");
				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("tskUsingAssignmentManagerToCheckAssignmentStatus");
				GoBack1.GoBack_top = "-5px";
				GoBack1.GoBack_left = "60px";
				GoBack1.GoBackIncludeBack = false;

				if(courseId <= 0)
				{
					Nav1.Feedback.Text = SharedSupport.GetLocalizedString("Global_MissingParameter");					
				}

				CourseM course = CourseM.Load(courseId);
				string title = course.Name.Trim();
				if(title.Length > 45)
				{
					title = title.Substring(0,45) + SharedSupport.GetLocalizedString("UserControl_Faculty_DotDotDot");
				}
				Nav1.Title = Server.HtmlEncode(title);

                if(course.IsValid)
				{
					Nav1.SubTitle = Server.HtmlEncode(SharedSupport.GetLocalizedString("Assignments_Subtitle") + " " + course.Name);
				}
				

				if (!IsPostBack)
				{
					// Evals true first time browser hits the page
				}

				Response.Cache.SetNoStore();			
				int userID = SharedSupport.GetUserIdentity();		
				//throws an error if either the courseID or userID are empty
				if (courseId.Equals(null) || userID.Equals(null))
				{
					Nav1.Feedback.Text =  SharedSupport.GetLocalizedString("Assignments_CourseIDError");
				}
				else
				{					
					try
					{
						//calls a function to return all the assignments for the course
						BrowseAssignments(course);
					}
					catch(System.Exception ex)
					{
						Nav1.Feedback.Text = ex.Message;
					}
				}

				localizeLabels();
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

		
		protected void localizeLabels()
		{
			this.lblCompleted.Text = SharedSupport.GetLocalizedString("Assignment_Completed");
			this.lblFailed.Text = SharedSupport.GetLocalizedString("Assignment_Failed");
			this.lblPending.Text = SharedSupport.GetLocalizedString("Assignment_Pending");
		 
		}

		//First, defines the SectionID for the User and Course
		//Then, grabs the info for the Assignments and status using the UserID and SectionID
		protected void BrowseAssignments(CourseM course)
		{			
			if(courseId.Equals(null))
			{
				Nav1.Feedback.Text = SharedSupport.GetLocalizedString("Assignments_SectionIDError");
			}
			else
			{
				int userID = SharedSupport.GetUserIdentity();
				AssignmentList assignList = course.GetStudentAssignmentList(userID);

				if(assignList.Count > 0)
				{
					//cycle through all the rows and set n/a
					for(int i=0;i<assignList.Count; i++)
					{
						if(assignList.GetOverallGradeForItem(i).Equals(String.Empty))
						{
							assignList.SetOverallGradeForItem(i, SharedSupport.GetLocalizedString("GradeDetail_Default"));
						}
					}
		
					//Populate DataList
					this.dlAssignments.DataSource = assignList.GetDefaultView(Server);
					this.dlAssignments.DataBind();
				}	
				else
				{
					Nav1.Feedback.Text = SharedSupport.GetLocalizedString("Assignments_NoAssignmentError");
				}
			}		
		}

		public void dlAssignments_ItemDataBound(Object sender, DataListItemEventArgs e) 
		{	
			// this event fires as individual items are bound to the datalist
			// we dynamically change the 'Get Starter' icon and hyperlink depending upon starter availability for the assignment

			// ensure item is type item
			ListItemType itemType = e.Item.ItemType;
			if(!(itemType.Equals(ListItemType.AlternatingItem) || itemType.Equals(ListItemType.Item))) return;

			// grab handle to data row
			DataRowView dr = (DataRowView)e.Item.DataItem;

			// evaluate the dataitem value and show applicable image
			if(Convert.ToBoolean(dr["StarterProjectFlag"]) == true) 
			{
				System.Web.UI.WebControls.HyperLink hl = (System.Web.UI.WebControls.HyperLink)e.Item.FindControl("hlStarterAvailable");
				hl.Visible = true;
				hl.NavigateUrl += "?Action=downloadstarter&AssignmentID=" + dr["AssignmentID"] + "&CourseID=" + courseId.ToString();
			}
			else
			{
				e.Item.FindControl("imgStarterNotAvailable").Visible = true;
			}
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
