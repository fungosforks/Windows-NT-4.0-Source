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
	using System.Security;
    /// <summary>
    ///    Summary description for Users.
    /// </summary>
    public class Users : System.Web.UI.Page
    {
		protected System.Web.UI.WebControls.DataList dlUsers;
		protected System.Web.UI.WebControls.HyperLink hlAddUser;
		protected System.Web.UI.WebControls.HyperLink hlImportUsers;
		private string NO_COURSE_ERROR = SharedSupport.GetLocalizedString("FacultyUsers_NO_COURSE_ERROR"); //"There were no users for the corresponding CourseID.";
		private string NO_COURSEID_ERROR = SharedSupport.GetLocalizedString("FacultyUsers_NO_COURSEID_ERROR"); //"There was no CourseID passed on the query string.";
		private string INVALID_COURSEID_ERROR = SharedSupport.GetLocalizedString("FacultyUsers_INVALID_COURSEID_ERROR"); //"There was no Course corresponding to the passed CourseID.";
		
		// persist querystring parameters instead of referencing Request object every time needed
		private int courseId;
		protected string Users_Text_String_Name = SharedSupport.GetLocalizedString("Users_Text_String_Name");
		protected string Users_Text_String_Email = SharedSupport.GetLocalizedString("Users_Text_String_Email");
		protected string Users_Text_String_UniversityID =SharedSupport.GetLocalizedString("Users_Text_String_UniversityID");
		protected string Users_Text_String_UserName = SharedSupport.GetLocalizedString("Users_Text_String_UserName");
		protected string Users_Text_String_LastUpdated = SharedSupport.GetLocalizedString("Users_Text_String_LastUpdated");
		protected string Users_Text_String_Title = SharedSupport.GetLocalizedString("Users_Text_String_Title");
		protected string Title = SharedSupport.GetLocalizedString("AM_Title");
		protected string subTitle = SharedSupport.GetLocalizedString("UserControl_Faculty_CourseUsers");

		protected AssignmentManager.UserControls.faculty Nav1;
		protected AssignmentManager.UserControls.goBack GoBack1;

		public Users()
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
				Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_COURSE_USERS;
				Nav1.SubTitle = subTitle;

				Nav1.relativeURL = @"../";

				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vstskAddingCourseUsers");
				GoBack1.GoBackIncludeBack = false;
				GoBack1.GoBack_top = "-5px";
				GoBack1.GoBack_left = "10px";

				// grab CourseID parameter from the querystring
				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
				courseId = func.ValidateNumericQueryStringParameter(this.Request, "CourseID");

				if(courseId <= 0)
				{
					throw(new ArgumentException(SharedSupport.GetLocalizedString("Global_MissingParameter")));					
				}
				
				//Check Security Permissions
				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USER_VIEW))
				{
					// Note that Redirect ends page execution.
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
				}

				if (!IsPostBack)
				{
				    //
				    // Evals true first time browser hits the page
				    //

					LocalizeLabels();
					
					this.hlAddUser.Text = SharedSupport.GetLocalizedString("FacultyUsers_AddUser"); //"Add User";
					this.hlImportUsers.Text = SharedSupport.GetLocalizedString("AddEditUser_ImportUsers"); //bulk import users
					this.hlAddUser.NavigateUrl = "AddEditUser.aspx?CourseID=" + courseId.ToString();
					this.hlImportUsers.NavigateUrl = "ImportForm.aspx?CourseID=" + courseId.ToString();

					
					UserList userlist = UserList.GetListFromCourse(courseId);
					DataView dv = userlist.GetDataView(Server);
					if (dv != null)
					{
						dlUsers.DataSource = dv;
						dlUsers.DataBind();
						dlUsers.Visible = true;
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
			this.Load += new System.EventHandler (this.Page_Load);
        }
		private void LocalizeLabels()
		{
			
		}
    }
}
