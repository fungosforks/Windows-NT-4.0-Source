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
	/// Summary description for WorkWithCourse.
	/// </summary>
	public class WorkWithCourse : System.Web.UI.Page
	{
		protected AssignmentManager.UserControls.student Nav1;
		protected string Title = SharedSupport.GetLocalizedString("AM_Title");

		public WorkWithCourse()
		{
			Page.Init += new System.EventHandler(Page_Init);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				Nav1.relativeURL = "../";

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
						else
						{Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized", false);}
					}
					else
					{
						//Throw error, there was no CourseID on the query string
						Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_MissingParameter", false);
					}					
				}
			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text =  ex.Message.ToString();
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
