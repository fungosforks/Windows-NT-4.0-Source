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

namespace Microsoft.VisualStudio.Academic.AssignmentManager.Student
{
	/// <summary>
	/// Summary description for AddCourse.
	/// </summary>
	public class AddCourse : System.Web.UI.Page
	{
		protected AssignmentManager.UserControls.student Nav1;		
		public string Title = SharedSupport.GetLocalizedString("AM_Title");

		public AddCourse()
		{
			Page.Init += new System.EventHandler(Page_Init);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				Nav1.relativeURL = "../";
				
                AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();		
				int vsversion = func.ValidateNumericQueryStringParameter(Request, "VSVersion");
				if (vsversion < 7.1)
				{
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "AddCourse_WrongVSVersion", false);
				}

				//Check Security Permissions
				if (!IsPostBack)
				{
					if(Request["CourseID"] != null && Request["CourseID"] != String.Empty)
					{				
						
						System.Guid courseGuid = new System.Guid( Request.QueryString.Get("CourseID").ToString() );
						CourseM course = CourseM.Load(courseGuid);
						if(course.IsValid)
						{
							Response.Redirect("Assignments.aspx?CourseID=" + course.CourseID, false);
						}
					}
					else
					{
						//Throw error, there was no CourseID on the query string
						Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_MissingParameter", false);
					}					
				}
			}
			catch
			{
				Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized", false);
			}
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Windows Form Designer.
			//
			InitializeComponent();
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
