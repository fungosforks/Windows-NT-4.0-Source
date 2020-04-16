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
	public class faculty : System.Web.UI.UserControl
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
		protected string loginLink = "/login.aspx";
		protected string baseUrl = String.Empty;
		protected string rootURL;
		public readonly string UserControl_Faculty_CapA = SharedSupport.GetLocalizedString("NavBar_CapA");
		public readonly string UserControl_Faculty_Assignment = SharedSupport.GetLocalizedString("NavBar_ssignment");
		public readonly string UserControl_Faculty_CapM = SharedSupport.GetLocalizedString("NavBar_CapM");
		public readonly string UserControl_Faculty_Manager = SharedSupport.GetLocalizedString("NavBar_anager");
		public readonly string UserControl_Faculty_LogOff = SharedSupport.GetLocalizedString("UserControl_Faculty_LogOff");
		public readonly string UserControl_Faculty_BackToStartPage = SharedSupport.GetLocalizedString("UserControl_Faculty_BackToStartPage");
		public readonly string UserControl_Faculty_CourseManagement = SharedSupport.GetLocalizedString("UserControl_Faculty_CourseManagement");
		public readonly string UserControl_Faculty_Server_Administration = SharedSupport.GetLocalizedString("UserControl_Faculty_Server_Administration");
		public readonly string UserControl_Faculty_CourseInfo = SharedSupport.GetLocalizedString("UserControl_Faculty_CourseInfo");
		public readonly string UserControl_Faculty_CourseAssignments = SharedSupport.GetLocalizedString("UserControl_Faculty_CourseAssignments");
		public readonly string UserControl_Faculty_CourseUsers = SharedSupport.GetLocalizedString("UserControl_Faculty_CourseUsers");
		public readonly string UserControl_Faculty_Settings = SharedSupport.GetLocalizedString("UserControl_Faculty_Settings");
		public readonly string UserControl_Faculty_Security = SharedSupport.GetLocalizedString("UserControl_Faculty_Security");
		public readonly string UserControl_Faculty_MyAccount = SharedSupport.GetLocalizedString("UserControl_Faculty_MyAccount");
		public readonly string UserControl_Faculty_Read = SharedSupport.GetLocalizedString("UserControl_Faculty_Read");
		public readonly string UserControl_Faculty_Compose = SharedSupport.GetLocalizedString("UserControl_Faculty_Compose");
		public readonly string UserControl_Faculty_DotDotDot = SharedSupport.GetLocalizedString("UserControl_Faculty_DotDotDot");
		public readonly string StartPageLocation = AssignmentManager.Common.constants.BACK_TO_CORETOOLS_LINK;
		/// <summary>
		public faculty()
		{
			this.Init += new System.EventHandler(Page_Init);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
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
					Response.Redirect(rootURL + "Logoff.aspx");
				}
				// Put user code to initialize the page here
				userId = SharedSupport.GetUserIdentity();
				//Verify that CourseID is present
				AssignmentManager.Common.Functions fun = new AssignmentManager.Common.Functions();
				CourseId = fun.ValidateNumericQueryStringParameter(Request,"CourseID");
				
				//If the title is not already set, set it to the course
				if(Title == String.Empty || Title == "")
				{
					CourseM course = CourseM.Load(CourseId);
					if(course.IsValid)
					{
						Title = course.Name.Trim();
						if(Title.Length > 30)
						{
							lblTitle.Text = Title = Title.Substring(0,30) + UserControl_Faculty_DotDotDot;
						}
						lblTitle.Text = Server.HtmlEncode(Title);
					}
				}
				else
				{
					this.lblTitle.Text = Server.HtmlEncode(this.Title);
				}
				//If the subtitle is not already set, set it to the course
				if(SubTitle == String.Empty || SubTitle == "")
				{
					CourseM course = CourseM.Load(CourseId);
					if(course.IsValid)
					{						
						this.lblSubTitle.Text = Server.HtmlEncode(SharedSupport.GetLocalizedString("NavBar_SubTitle1") + course.Name);			
					}
				}
				else
				{
					this.lblSubTitle.Text = Server.HtmlEncode(this.SubTitle);
				}
			}
			catch(Exception ex)
			{
				Feedback.Text = ex.Message;
			}
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			Title = String.Empty;
			SubTitle = String.Empty;
			Page.Response.BufferOutput = true;		
			InitializeComponent();
			
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
