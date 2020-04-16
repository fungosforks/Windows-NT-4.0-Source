//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
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

	namespace Microsoft.VisualStudio.Academic.AssignmentManager.Faculty
	{
		/// <summary>
    ///    Summary description for Assignments.
    /// </summary>
    public class Assignments : System.Web.UI.Page
    {
		protected System.Web.UI.WebControls.Label lblCourseName;
		protected System.Web.UI.WebControls.Label lblFeedback;
		protected System.Web.UI.WebControls.DataList dlAssignments;
		protected System.Web.UI.WebControls.HyperLink hlAddAssignment;
		
		protected AssignmentManager.UserControls.faculty Nav1;
		protected AssignmentManager.UserControls.goBack GoBack1;

		private string INVALID_SECTION_ID_ERROR = SharedSupport.GetLocalizedString("FacultyAssignments_INVALID_SECTION_ID_ERROR"); //"The SectionID did not correspond to a SectionAssignments record.";
		private string NO_SECTIONID_ERROR = SharedSupport.GetLocalizedString("FacultyAssignments_NO_SECTIONID_ERROR"); //"There was no SectionID included on the querystring.  There is no way to load the corresponding assignments.";
		private string INVALID_COURSEID_ERROR = SharedSupport.GetLocalizedString("FacultyAssignments_INVALID_COURSEID_ERROR"); //"There was no Course record corresponding to the passed CourseID.";
		private string NO_COURSEID_ERROR = SharedSupport.GetLocalizedString("FacultyAssignments_NO_COURSEID_ERROR"); //"There was no CourseID passed on the query string.";
		
		//Localization of DataList Headers
		public string Assignments_Text_String = SharedSupport.GetLocalizedString("FacultyAssignments_Assignments_Text_String");
		public string DueDate_Text_String = SharedSupport.GetLocalizedString("FacultyAssignments_DueDate_Text_String");
		public string NumberSubmitting_Text_String = SharedSupport.GetLocalizedString("FacultyAssignments_NumberSubmitting_Text_String");
		public string UploadStarter_Text_String = SharedSupport.GetLocalizedString("FacultyAssignments_UploadStarter_Text_String");
		public readonly string Title = SharedSupport.GetLocalizedString("AM_Title");

		// persist querystring parameters instead of referencing Request object every time needed
		private int courseId;

		public Assignments()
		{
			Page.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			try
			{
				Nav1.Feedback.Text =  String.Empty;
				Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT;
				Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_COURSE_ASSIGNMENTS;
				Nav1.relativeURL = @"../";

				GoBack1.GoBack_left = "575px";
				GoBack1.GoBack_top = "-5px";
				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vsoriUsingAssignmentManager");
				GoBack1.GoBackIncludeBack = false;

				// grab CourseID parameter
				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
				courseId= func.ValidateNumericQueryStringParameter(this.Request, "CourseID");

				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.ASSIGNMENT_VIEW))
				{
					// Note that Redirect ends page execution.
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
				}
				
				if (!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.ASSIGNMENT_ADD))
				{
					hlAddAssignment.Enabled = false;
					hlAddAssignment.Visible = false;
				}
				else
				{
					hlAddAssignment.Enabled = true;
					hlAddAssignment.Visible = true;
				}

				// get the courseId based on courseOfferingId			
				CourseM course = CourseM.Load(courseId);
				if(course.IsValid)
				{
					Nav1.SubTitle = SharedSupport.GetLocalizedString("Assignments_Subtitle") + " " + course.Name;
				}
				
				if (!IsPostBack)
				{
				    //
				    // Evals true first time browser hits the page
				    //

					//Get localization string for all text displayed on the page
					LocalizeLabels();
					//Initialize the feedback label to nothing.
					Nav1.Feedback.Text =  String.Empty;

					//Populate the "Add Assignment" link with the courseID from the query string
					hlAddAssignment.NavigateUrl += "?CourseID=" + course.CourseID;

					//Grab the assignment information for the given CourseOffering
					
					AssignmentList assignList = course.AssignmentList;
					//If there was at least one assignment, build the table.
					if(assignList.Count> 0)
					{
						dlAssignments.DataSource = assignList.GetDefaultView(Server);
						dlAssignments.DataBind();
						dlAssignments.Visible = true;
					}
					else
					{
						//The assignment for the given SectionID did not exist.
						this.hlAddAssignment.Visible = true;
						throw new Exception(INVALID_SECTION_ID_ERROR);
					}
				}

			}
			catch (Exception ex)
			{
				//catch and add all exception errors to the lblFeedback label and display.
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
			this.Load += new System.EventHandler(this.Page_Load);

		}

		protected void LocalizeLabels()
		{
			this.hlAddAssignment.Text = SharedSupport.GetLocalizedString("Assignments_AddNewLink1");
		}
    }
}