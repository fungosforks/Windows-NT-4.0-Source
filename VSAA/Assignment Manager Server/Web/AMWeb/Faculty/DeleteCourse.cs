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
	using System.IO;

    /// <summary>
    ///    Summary description for DeleteCourse.
    /// </summary>
    public class DeleteCourse : System.Web.UI.Page
    {
		protected System.Web.UI.WebControls.HyperLink hlCoreTools;
		protected System.Web.UI.WebControls.Label lblFeedback;
		protected System.Web.UI.WebControls.Label lblSubTitle;
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.HtmlControls.HtmlGenericControl copyText;

		public readonly string Title = SharedSupport.GetLocalizedString("AM_Title");

		public DeleteCourse()
		{
			Page.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			try
			{
				// link to Core Tools
				this.hlCoreTools.Text = SharedSupport.GetLocalizedString("ErrorPage_BackToStartPage");
				this.hlCoreTools.NavigateUrl = AssignmentManager.Common.constants.BACK_TO_CORETOOLS_LINK;

				//Check Security Permissions


				if(!SharedSupport.SecurityIsAllowed(SecurityAction.COURSE_DELETE))
				{
					// Note that Redirect ends page execution.
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
				}
	            
				if (!IsPostBack)
				{
					string CourseID = Request.QueryString.Get("CourseID").ToString();
					System.Guid courseGuid = new System.Guid(CourseID);
					CourseM course = CourseM.Load(courseGuid);
					if(course.IsValid)
					{
						string dir = System.Web.HttpContext.Current.Server.MapPath("..\\") + "\\Courses\\";
						File.Delete(dir + CourseID + ".xml");
						course.Delete();
						lblFeedback.Text = SharedSupport.GetLocalizedString("DeleteCourse_FacultyDeleteVerification");

						// log off to avoid confusion
						System.Web.Security.FormsAuthentication.SignOut();
					}
					else
					{
						lblFeedback.Text = SharedSupport.GetLocalizedString("DeleteCourse_FacultyAlreadyDeleted");
					}

				}
			}
			catch(System.Exception ex)
			{
				lblFeedback.Text = ex.Message;
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
    }
}
