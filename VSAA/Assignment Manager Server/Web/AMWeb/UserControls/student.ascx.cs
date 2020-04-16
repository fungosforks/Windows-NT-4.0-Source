//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//

namespace Microsoft.VisualStudio.Academic.AssignmentManager.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Microsoft.VisualStudio.Academic.AssignmentManager;

	/// <summary>
	///		Summary description for faculty.
	/// </summary>
	public class student : System.Web.UI.UserControl
	{
		public string Title;		// title string; expected as a token that needs to be localized
		public string SubTitle;		// subtitle string; token
		//public string Feedback;		// feedback/error string; token
		public string relativeURL;
		protected int CourseId;		// CourseID
		public int SideTabId;		// the id of the side tab to display; use constants class
		public int TopTabId;		// the id of the top tab to display; use constants class
		public int userId;

		protected System.Web.UI.WebControls.Label lblSubTitle;
		protected System.Web.UI.WebControls.Label lblTitle;
		public System.Web.UI.WebControls.Label Feedback;
		protected System.Web.UI.WebControls.LinkButton LinkButton1;
		protected System.Web.UI.WebControls.LinkButton btnCourseManagement ;
		protected System.Web.UI.HtmlControls.HtmlGenericControl copyText;
		protected string loginLink = "/login.aspx";
		protected string baseUrl = String.Empty;
		protected string rootURL;

		public readonly string UserControl_Student_CapA = SharedSupport.GetLocalizedString("NavBar_CapA");
		public readonly string UserControl_Student_Assignment = SharedSupport.GetLocalizedString("NavBar_ssignment");
		public readonly string UserControl_Student_CapM = SharedSupport.GetLocalizedString("NavBar_CapM");
		public readonly string UserControl_Student_Manager = SharedSupport.GetLocalizedString("NavBar_anager");
		public readonly string UserControl_Student_LogOff = SharedSupport.GetLocalizedString("UserControl_Student_LogOff");
		public readonly string UserControl_Student_BackToStartPage = SharedSupport.GetLocalizedString("UserControl_Student_BackToStartPage");
		public readonly string UserControl_Student_Password = SharedSupport.GetLocalizedString("UserControl_Student_Password");
		public readonly string UserControl_Student_CourseInfo = SharedSupport.GetLocalizedString("UserControl_Student_CourseInfo");
		public readonly string UserControl_Student_CourseAssignments = SharedSupport.GetLocalizedString("UserControl_Student_CourseAssignments");
		public readonly string UserControl_Student_Read = SharedSupport.GetLocalizedString("UserControl_Student_Read");
		public readonly string UserControl_Student_Password2 = SharedSupport.GetLocalizedString("UserControl_Student_Password2");
		public readonly string UserControl_Student_Course = SharedSupport.GetLocalizedString("UserControl_Student_Course");
		protected System.Web.UI.HtmlControls.HtmlGenericControl Div1;
		public readonly string StartPageLocation = AssignmentManager.Common.constants.BACK_TO_CORETOOLS_LINK;


		/// <summary>
		public student()
		{
			this.Init += new System.EventHandler(Page_Init);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				// Initialize labels
				initializeLabels();
				// set the baseUrl variable
				baseUrl = AssignmentManager.SharedSupport.BaseUrl.ToString();
		
				// set logoff link dynamically based on if server is using SSL
				if(Convert.ToBoolean(SharedSupport.UsingSsl) == true)
				{
					rootURL = "https://" + baseUrl + @"/";
					
				}
				else
				{	rootURL = "http://" + baseUrl + @"/";
				}
		
				// logs user off if actionLogoff hidden input tag = "logoff" (e.g. click on Logoff link)
				if(Page.Request["actionLogoff"] == "logoff") 
				{
					// Note that Redirect ends page execution.
					Response.Redirect(rootURL + "logoff.aspx");
				}
				// Put user code to initialize the page here
				userId = SharedSupport.GetUserIdentity();
				AssignmentManager.Common.Functions fun = new AssignmentManager.Common.Functions();
				CourseId = fun.ValidateNumericQueryStringParameter(Request,"CourseID");
			}
			catch(Exception ex)
			{
				Feedback.Text = ex.Message;
			}
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			Page.Response.BufferOutput = true;		
			InitializeComponent();	
		}

		private void initializeLabels()
		{
			this.lblTitle.Text = Title;
			this.lblSubTitle.Text = SubTitle;
		}

		#region Web Form Designer generated code
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
