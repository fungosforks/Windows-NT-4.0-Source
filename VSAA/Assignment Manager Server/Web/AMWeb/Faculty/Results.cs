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
    ///    Summary description for Results.
    /// </summary>
    public class Results : System.Web.UI.Page
    {
		protected System.Web.UI.WebControls.Label lblNumberFailed;
		protected System.Web.UI.WebControls.Label lblNumberSuccessful;
		protected System.Web.UI.WebControls.Label lblTotalRecords;
		protected System.Web.UI.WebControls.Label lblUserInfo;
		protected System.Web.UI.WebControls.Button btnSave;
		protected System.Web.UI.WebControls.Button btnCancel;
		
		protected AssignmentManager.UserControls.faculty Nav1;
		protected AssignmentManager.UserControls.goBack GoBack1;

		public string Total_RecordsToBe_TextString = SharedSupport.GetLocalizedString("AdminResults_Total_RecordsToBe");
		public string Total_RecordsImported_TextString = SharedSupport.GetLocalizedString("AdminResults_Total_RecordsImported");
		public string Total_RecordsFailedImported_TextString = SharedSupport.GetLocalizedString("AdminResults_Total_RecordsFailedImported");
		public string Title = SharedSupport.GetLocalizedString("AM_Title");

		// persist querystring parameters instead of referencing Request object every time needed
		private int courseId;
		
		public Results()
		{
			Page.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			try
			{
				//display nav bar 
				Nav1.Feedback.Text =  "&nbsp;"; //Add non-breakable space as placeholder for Feedback label(prevents table from shifting as messages appear/disappear).
				Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT;
				Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_COURSE_USERS;
				Nav1.Title = SharedSupport.GetLocalizedString("Results_Title");//"User Bulk Import Results";
				Nav1.relativeURL = @"../";

				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vsoriUsingAssignmentManager");
				GoBack1.GoBack_left = "400px";
				GoBack1.GoBack_top = "15px";
				GoBack1.GoBackIncludeBack = false;
				GoBack1.GoBack_BackURL = "ImportForm.aspx?" + Request.QueryString.ToString();
				
				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
				courseId = func.ValidateNumericQueryStringParameter(this.Request, "CourseID");

				if(courseId == 0)
				{
					throw(new ArgumentException(SharedSupport.GetLocalizedString("Global_MissingParameter")));					
				}

				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USER_ADD))
				{
					// Note that Redirect ends page execution.
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
				}

				btnCancel.Enabled = true;
				btnSave.Enabled = true;
				btnSave.Visible = true;
				
				if (!IsPostBack)
				{
				    //
				    // Evals true first time browser hits the page
				    //

					LocalizeLabels();

					lblNumberFailed.Text = func.ValidateNumericQueryStringParameter(this.Request, "Errors").ToString();
					lblNumberSuccessful.Text = func.ValidateNumericQueryStringParameter(this.Request, "Success").ToString();
					lblTotalRecords.Text = func.ValidateNumericQueryStringParameter(this.Request, "Expected").ToString();

					if (Convert.ToBoolean(SharedSupport.UsingSmtp)) 
					{
						lblUserInfo.Text =  SharedSupport.GetLocalizedString("AdminResults_UsersInsertedEmailSent"); //"A random password was generated and sent in e-mail to the user."
					} 
					else 
					{
						lblUserInfo.Text =  SharedSupport.GetLocalizedString("AdminResults_UsersInsertedPasswordIsName"); //"The user's password is their user name."
					}
				}
			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text =  ex.Message.ToString();
				btnCancel.Enabled = true;
				btnSave.Enabled = false;
				btnSave.Visible = false;
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
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}

		public void btnSave_Click (object sender, System.EventArgs e)
		{
			try
			{
				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USER_ADD))
				{
					// Note that Redirect ends page execution.
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
				}
				btnCancel.Enabled = true;
				btnSave.Enabled = true;
				btnSave.Visible = true;
				Nav1.Feedback.Text =  String.Empty;

				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();				
				string importID = func.ValidateStringQueryStringParameter(this.Request, "ImportID");

				if(ImportUsers.CommitImport(importID))
				{	
					// Records saved successfully redirect to Users page.
					Response.Redirect("Users.aspx?CourseID=" + Request.QueryString.Get("CourseID").ToString(), false);
				}
				else
				{
					throw new ApplicationException();
				}
			}
			catch
			{
				Nav1.Feedback.Text =  SharedSupport.GetLocalizedString("AdminResults_UpdateFailed");
				btnCancel.Enabled = true;
				btnSave.Enabled = false;
				btnSave.Visible = false;
			}

		}

		public void btnCancel_Click (object sender, System.EventArgs e)
		{
			try
			{
				btnCancel.Enabled = true;
				btnSave.Enabled = true;
				btnSave.Visible = true;
				//Delete all imported users with status of pending and userid = loggedin user
				Nav1.Feedback.Text =  String.Empty;
				
				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();				
				string importID = func.ValidateStringQueryStringParameter(this.Request, "ImportID");
				ImportUsers.AbortImport(importID);
				Response.Redirect("Users.aspx?CourseID=" + Request.QueryString.Get("CourseID").ToString(), false);
			}
			catch
			{
				Nav1.Feedback.Text =  SharedSupport.GetLocalizedString("AdminResults_CancelFailed");
				btnCancel.Enabled = true;
				btnSave.Enabled = false;
				btnSave.Visible = false;
			}
		}

		private void LocalizeLabels()
		{
			this.btnCancel.Text = SharedSupport.GetLocalizedString("AdminResults_Cancel");
			this.btnSave.Text = SharedSupport.GetLocalizedString("AdminResults_Save");
		}
    }
}
