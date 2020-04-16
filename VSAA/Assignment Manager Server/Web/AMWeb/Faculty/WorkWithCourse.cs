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
    ///    Summary description for WorkWithCourse.
    /// </summary>
    public class WorkWithCourse : System.Web.UI.Page
    {
		protected AssignmentManager.UserControls.faculty Nav1;
		protected string Title = SharedSupport.GetLocalizedString("AM_Title");

		public WorkWithCourse()
		{
			Page.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			try
			{
				Nav1.relativeURL = "../";

				if (!IsPostBack)
				{
				    //
				    // Evals true first time browser hits the page
				    //				
					
					if(Request["CourseID"] != null && Request["CourseID"] != String.Empty)
					{		
						System.Guid courseGuid = new System.Guid(Request.QueryString.Get("CourseID").ToString());

						CourseM course = CourseM.Load(courseGuid);
						
						if(course.IsValid)
						{						
							Response.Redirect(@"Assignments.aspx?CourseID=" + course.CourseID, false);
						}
						else
						{
//							if(!security.IsAllowed(Constants.COURSE_ADD_ACTION) )
//							{ 
//								// Note that Redirect ends page execution.
//								Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
//							} 
							//Course needs to be inserted, redirect to AddCourse.aspx
							Response.Redirect("AddCourse.aspx?" + Request.QueryString.ToString(), false);
						}				
					}
					else
					{
						//Throw error, there was no CourseID on the query string
						Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_MissingParameter&" + Request.QueryString.ToString(), false);
					}					
				}
			}
			catch(Exception)
			{
				Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_MissingParameter&" + Request.QueryString.ToString(), false);
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
    }
}
