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
    ///    Summary description for WebForm1.
    /// </summary>
    public class ImportForm : System.Web.UI.Page
    {
		protected System.Web.UI.WebControls.LinkButton btnImportRecords;
		protected System.Web.UI.WebControls.Button btnPreview;
		protected System.Web.UI.WebControls.Button btnCancel;
		protected System.Web.UI.WebControls.Label lblCourseText;
		protected System.Web.UI.WebControls.DropDownList cboSection;
		protected System.Web.UI.HtmlControls.HtmlInputFile txtUploadFile;
		protected System.Web.UI.WebControls.TextBox txtFileName;
		protected System.Web.UI.WebControls.Label lblValidationResultImport;
		protected System.Web.UI.WebControls.DataGrid dgrdPreviewBody;
		protected System.Web.UI.WebControls.DataList dlstPreviewHeader;
		protected System.Web.UI.WebControls.DropDownList cboColumns;
		protected System.Web.UI.WebControls.Label lblValidationResult;
		protected System.Web.UI.WebControls.TextBox txtDelimitingCharacter;
		protected System.Web.UI.WebControls.DropDownList cboDelimitingCharacter;
		protected System.Web.UI.WebControls.Label lblDelimitedCharacter;
		protected System.Web.UI.WebControls.Label lblTypeOfFile;
		protected System.Web.UI.WebControls.Label lblSelectFile;
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected System.Web.UI.HtmlControls.HtmlTable TABLE2;
		
		public static string NONE_Text_String = SharedSupport.GetLocalizedString("AdminImport_None");
		public static string FirstName_Text_String = SharedSupport.GetLocalizedString("AdminImport_FirstName");
		public static string LastName_Text_String = SharedSupport.GetLocalizedString("AdminImport_LastName");
		public static string MiddleName_Text_String = SharedSupport.GetLocalizedString("AdminImport_MiddleName");
		public static string Email_Text_String = SharedSupport.GetLocalizedString("AdminImport_Email");
		public static string UniversityID_Text_String = SharedSupport.GetLocalizedString("AdminImport_UniversityID");
		public static string UserName_Text_String = SharedSupport.GetLocalizedString("AdminImport_UserName");
		public static string Title = SharedSupport.GetLocalizedString("AM_Title");

		// persist querystring parameters instead of referencing Request object every time needed
		private int courseId = 0;

		//user control declarations
		protected AssignmentManager.UserControls.faculty Nav1;
		protected AssignmentManager.UserControls.goBack GoBack1;

		public ImportForm()
		{	
			Page.Init += new System.EventHandler(Page_Init);
		}

        protected void Page_Load(object sender, EventArgs e)
        {
			try
			{	
				//display nav bar 
				Nav1.Feedback.Text = String.Empty;
				Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT;
				Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_COURSE_USERS;
				Nav1.Title = " ";
				Nav1.SubTitle = SharedSupport.GetLocalizedString("ImportForm_SubTitle");
				Nav1.relativeURL = @"../";

				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vstskAddingCourseUsers");
				GoBack1.GoBackIncludeBack = true;
				GoBack1.GoBack_BackURL = "Users.aspx?" + Request.QueryString.ToString();
				GoBack1.GoBack_left = "400px";
				GoBack1.GoBack_top = "-15px";

				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
				courseId = func.ValidateNumericQueryStringParameter(this.Request, "CourseID");
				if(courseId <= 0)
				{
					throw(new ArgumentException(SharedSupport.GetLocalizedString("Global_MissingParameter")));					
				}	

				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USER_ADD))
				{
					// Note that Redirect ends page execution.
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
				}
				
				if(!IsPostBack)
				{
					// Populate the DropDownList with the available delimiters.
					cboDelimitingCharacter.Items.Clear();
					cboDelimitingCharacter.Items.Add(",");
					cboDelimitingCharacter.Items.Add(";");
					cboDelimitingCharacter.Items.Add(SharedSupport.GetLocalizedString("AdminImport_Tab"));
				}

				LocalizeLabels();
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

			Page.Response.BufferOutput = true;

            InitializeComponent();
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
		private void InitializeComponent()
		{
			this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}

		public void btnPreview_Click (object sender, System.EventArgs e)
		{
			try
			{	 

				Nav1.Feedback.Text = String.Empty;

				// Delete any previous imports that have not been committed.
				//bool clearPreviousImportData = users.CancelPendingImport(courseId);

				DataSet ds = new DataSet();
				//Validate File not blank
				if(txtUploadFile.PostedFile.Equals(null))
				{
					Nav1.Feedback.Text = SharedSupport.GetLocalizedString("AdminImport_ChooseUploadFile");
					return;
				}
				//Validate File not blank
				if(txtUploadFile.PostedFile.FileName == String.Empty)
				{
					Nav1.Feedback.Text= SharedSupport.GetLocalizedString("AdminImport_ChooseUploadFile");
					return;
				}
				//Validate delimiting character not blank
				if(cboDelimitingCharacter.SelectedItem.Text == String.Empty)
				{
					Nav1.Feedback.Text = SharedSupport.GetLocalizedString("AdminImport_ChooseDelimitingChar");
					return;
				}
				string delimiterCharacter = "";
				if(cboDelimitingCharacter.SelectedItem.Text == SharedSupport.GetLocalizedString("AdminImport_Tab")) 
				{
					delimiterCharacter = "\t";
				}
				else {
					delimiterCharacter = cboDelimitingCharacter.SelectedItem.Text;			
				}
				
				string filename = System.Guid.NewGuid().ToString();
				txtUploadFile.PostedFile.SaveAs(SharedSupport.AddBackSlashToDirectory(Server.MapPath(Constants.ASSIGNMENTMANAGER_UPLOAD_DIRECTORY)) + filename);
				
				Response.Redirect("ImportFormPreview.aspx?" + Request.QueryString + "&File=" + Server.UrlEncode(filename) + "&Char=" + Server.UrlEncode(delimiterCharacter), false);

			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text = ex.Message;
			}
		}
					
		private void LocalizeLabels()
		{
			Nav1.Feedback.Text = String.Empty;
			lblDelimitedCharacter.Text = SharedSupport.GetLocalizedString("AdminImport_Delimited_Char");
			lblSelectFile.Text = SharedSupport.GetLocalizedString("AdminImport_SelectFile");
			btnPreview.Text = SharedSupport.GetLocalizedString("AdminImport_Preview"); //"Preview";
			btnCancel.Text = SharedSupport.GetLocalizedString("AdminImport_Cancel");
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			try
			{
				Response.Redirect("Users.aspx?CourseID=" + Request.QueryString.Get("CourseID").ToString(), false);
			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text = ex.Message;
			}
		}
	}
}