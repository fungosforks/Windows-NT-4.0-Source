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
	/// Summary description for CourseInfo.
	/// </summary>
	public class CourseInfo : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lblContainsSections;
		protected System.Web.UI.WebControls.Label lblDescription;
		protected System.Web.UI.WebControls.Label lblShortName;
		protected System.Web.UI.WebControls.TextBox txtDescriptionText;
		protected System.Web.UI.HtmlControls.HtmlTable Table1;
		protected System.Web.UI.WebControls.Label lblSendEMailReminders;
		protected System.Web.UI.WebControls.Label lblHomePageURL;		
		protected System.Web.UI.WebControls.HyperLink linkHomePageURLText;
		protected System.Web.UI.WebControls.Label lblShortNameValue;
		protected System.Web.UI.WebControls.Label lblAllowMultipleSubmits;
		protected System.Web.UI.WebControls.DataList dlCourseResources;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divCourseResources;

		public string Title = SharedSupport.GetLocalizedString("AM_Title");
		public string ResourcesTitle = SharedSupport.GetLocalizedString("AddDeleteResources_Title");
		protected AssignmentManager.UserControls.student Nav1;
		protected AssignmentManager.UserControls.goBack GoBack1;

		// persist querystring parameters instead of referencing Request object every time needed
		private int courseId;
		protected System.Web.UI.HtmlControls.HtmlTable tblGeneralAssignment;
		protected System.Web.UI.WebControls.TextBox txtResourceValue;
		protected System.Web.UI.WebControls.TextBox txtResourceName;
		protected System.Web.UI.HtmlControls.HtmlTable Table2;
		protected System.Web.UI.WebControls.Label lblCourseResources;
	
		public CourseInfo()
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

				//display nav bar 
				Nav1.Feedback.Text = String.Empty;
				Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_STUDENT_COURSE;
				Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_STUDENT_COURSE_INFO;
				Nav1.relativeURL = @"../";

				GoBack1.GoBack_left = "450px";
				GoBack1.GoBack_top = "5px";
				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("tskViewingCourseInformationWithAssignmentManager");
				GoBack1.GoBackIncludeBack = false;
				
				if(courseId.Equals(null))
				{
					throw(new ArgumentException(SharedSupport.GetLocalizedString("Global_MissingParameter")));					
				}

				CourseM course = CourseM.Load(courseId);
				if(course.IsValid)
				{
					Nav1.SubTitle = SharedSupport.GetLocalizedString("Assignments_Subtitle") + " " + Server.HtmlEncode(course.Name);
				}
				

				if (!IsPostBack)
				{
					//
					// Evals true first time browser hits the page
					//
			
					LocalizeLabels();
					
					if(course.IsValid)
					{
						this.txtDescriptionText.Text = course.Description;
						this.lblShortNameValue.Text = Server.HtmlEncode( course.Name );
						if(course.HomepageURL.Trim() != "")
						{
							this.linkHomePageURLText.Text = Server.HtmlEncode(course.HomepageURL);
							this.linkHomePageURLText.NavigateUrl = course.HomepageURL;
						}
						else
						{
							this.lblHomePageURL.Text = "";
						}
					
						string title = course.Name.Trim();
						if(title.Length > 45)
						{
							title = title.Substring(0,45) + SharedSupport.GetLocalizedString("UserControl_Faculty_DotDotDot");
						}
						Nav1.Title = Server.HtmlEncode(title);
						CourseResources(course);
					}
					else
					{
						// throw error - can't use this page without CourseID int passed in
						Nav1.Feedback.Text = SharedSupport.GetLocalizedString("AddEditCourse_MissingCourseID");
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
		private void LocalizeLabels()
		{
			txtDescriptionText.Text = String.Empty;
			lblDescription.Text = SharedSupport.GetLocalizedString("AddEditCourse_lblDescription"); //"Description: ";
			linkHomePageURLText.Text = String.Empty;
			lblHomePageURL.Text = SharedSupport.GetLocalizedString("AddEditCourse_lblHomePageURL1"); //"Home Page URL: ";
			Nav1.Feedback.Text = String.Empty;
			lblShortName.Text = SharedSupport.GetLocalizedString("AddEditCourse_lblShortName"); //"Course Name: ";
			lblShortNameValue.Text = String.Empty;
		}

		//Browses the CourseResources table by CourseID
		//If records are returned, fills labels w/ information; else labels are blank.
		private void CourseResources(CourseM course)
		{
			DataSet resourceList = course.ResourceList;
			if (resourceList.Tables.Count > 0)
			{
				for (int i=0;i<resourceList.Tables[0].Rows.Count;i++)
				{
					for (int j=0;j<resourceList.Tables[0].Columns.Count;j++)
					{
						try
						{
							resourceList.Tables[0].Rows[i][j] = Server.HtmlEncode( resourceList.Tables[0].Rows[i][j].ToString() );
						}
						catch
						{
						}
					}
				}
			}
			this.dlCourseResources.DataSource = resourceList;
			this.dlCourseResources.DataBind();
			this.dlCourseResources.Visible = true;
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
