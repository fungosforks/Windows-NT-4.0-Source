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
using System.Security;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Academic.AssignmentManager.Student
{
	/// <summary>
	/// Summary description for ChangePassword.
	/// </summary>
	public class ChangePassword : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lblNewPwd;
		protected System.Web.UI.WebControls.Label lblConfirmPwd;
		protected System.Web.UI.WebControls.TextBox txtNewPwd;
		protected System.Web.UI.WebControls.TextBox txtConfirmPwd;
		protected System.Web.UI.WebControls.Label lblUser;
		protected System.Web.UI.WebControls.Label lblUserName;
		protected System.Web.UI.WebControls.TextBox txtCurrentPwd;
		protected System.Web.UI.WebControls.Label lblCurrentPwd;
		protected System.Web.UI.WebControls.Button btnCancel;
		protected System.Web.UI.WebControls.Button btnSave;
		protected AssignmentManager.UserControls.student Nav1;
		protected AssignmentManager.UserControls.goBack GoBack1;
		protected System.Web.UI.WebControls.Label lblRequired;

		// persist querystring parameters instead of referencing Request object every time needed
		private int courseId;
		
		public string Title = SharedSupport.GetLocalizedString("AM_Title");
		protected System.Web.UI.HtmlControls.HtmlImage Img1;
		protected System.Web.UI.HtmlControls.HtmlImage Img4;
		protected System.Web.UI.HtmlControls.HtmlImage Img2;
		public string PageTitle = SharedSupport.GetLocalizedString("ChangePassword_Title");
		public ChangePassword()
		{
			Page.Init += new System.EventHandler(Page_Init);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{	
				// grab CourseID parameter from the querystring
				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
				courseId = func.ValidateNumericQueryStringParameter(this.Request, "CourseID");

				UserM user = UserM.Load(SharedSupport.GetUserIdentity());
				if (!user.IsInCourse(courseId))
				{
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");	
				}

				Nav1.Feedback.Text = String.Empty;
				Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_STUDENT_CHANGE_PASSWORD;
				Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_STUDENT_CHANGE_PASSWORD;
				Nav1.Title = SharedSupport.GetLocalizedString("ChangePassword_Title1");
				Nav1.SubTitle = SharedSupport.GetLocalizedString("ChangePassword_SubTitle1");
				Nav1.relativeURL = @"../";

				//GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vstskChangingYourUserPassword");
				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("tskChangingYourUserPasswordForAssignmentManager");
				GoBack1.GoBack_left = "275px";
				GoBack1.GoBack_top = "-15px";
				GoBack1.GoBackIncludeBack = false;

				if(courseId <= 0)
				{
					throw(new ArgumentException(SharedSupport.GetLocalizedString("Global_MissingParameter")));					
				}

				// if using SSL and the page isn't using a secure connection, redirect to https
				if(SharedSupport.UsingSsl == true && Request.IsSecureConnection == false)
				{
					// Note that Redirect ends page execution.
					Response.Redirect("https://" + SharedSupport.BaseUrl + "/faculty/ChangePassword.aspx?CourseID=" + courseId.ToString());
				}
					
				if (!IsPostBack)
				{
					// Evals true first time browser hits the page
					LocalizeLabels();
				}

				Response.Cache.SetNoStore();
				if (user.IsValid)
				{
					this.lblUserName.Text = Server.HtmlEncode(user.FirstName + " " + user.LastName);
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
			// CODEGEN: This call is required by the ASP.NET Windows Form Designer.
			//
			InitializeComponent();
		}

		protected void LocalizeLabels()
		{
			this.lblCurrentPwd.Text = SharedSupport.GetLocalizedString("ChangePassword_CurrentPwdHeader") + " " + SharedSupport.GetLocalizedString("Global_RequiredField");
			this.lblConfirmPwd.Text = SharedSupport.GetLocalizedString("ChangePassword_ConfirmPwdHeader") + " " + SharedSupport.GetLocalizedString("Global_RequiredField");
			this.lblNewPwd.Text = SharedSupport.GetLocalizedString("ChangePassword_NewPwdHeader1")+ " " + SharedSupport.GetLocalizedString("Global_RequiredField");
			this.btnCancel.Text = SharedSupport.GetLocalizedString("ChangePassword_btnCancel");
			this.btnSave.Text = SharedSupport.GetLocalizedString("ChangePassword_btnSave");	
			this.lblUser.Text = SharedSupport.GetLocalizedString("ChangePassword_lblUser");
			this.lblRequired.Text = SharedSupport.GetLocalizedString("Global_RequiredFieldIndicator");
		}

		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnSave_Click(object sender, System.EventArgs e)
		{
		
			try
			{
				if (this.txtCurrentPwd.Text == "")
				{
					throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_CurrentPassword_RequiredField"));
				}	
				else
				{
					UserM user = UserM.Load(SharedSupport.GetUserIdentity());
					//Compare the hashed version of the password stored in the db to the hashed version of the password entered.
					Byte[] passwd = SharedSupport.ConvertStringToByteArray(this.txtCurrentPwd.Text.Trim());
					byte[] hashValue = ((HashAlgorithm) CryptoConfig.CreateFromName(Constants.HashMethod)).ComputeHash(passwd);
			
					if(user.Password != BitConverter.ToString(hashValue))
					{
						throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_CurrentPasswordError"));
					}
				}
				if(this.txtConfirmPwd.Text == "")
				{
					throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_ConfirmPassword_RequiredField"));
				}
				if (this.txtNewPwd.Text == "")
				{
					throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_NewPassword_RequiredField"));
				}

				if(this.txtNewPwd.Text != this.txtConfirmPwd.Text)
				{
					this.txtConfirmPwd.Text = "";
					this.txtNewPwd.Text = "";
					throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_ConfirmationError"));
				}
				else if( (this.txtNewPwd.Text.Length < 4) || (this.txtNewPwd.Text.Length > 50) )
				{
					this.txtNewPwd.Text = "";
					this.txtConfirmPwd.Text = "";
					throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_PwdLengthError"));
				}
					// New password can't be the same as the previous password																								 																  
				else if (this.txtNewPwd.Text == this.txtCurrentPwd.Text)
				{
					this.txtNewPwd.Text = "";
					this.txtConfirmPwd.Text = "";
					throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_PwdSameAsOld"));															
				}

				UserM userObj = UserM.Load(SharedSupport.GetUserIdentity());
				if (userObj.IsValid)
				{
					userObj.Password = txtNewPwd.Text.Trim();
					userObj.UpdatePassword();
					Nav1.Feedback.Text = SharedSupport.GetLocalizedString("ChangePassword_SuccessfulUpdateMessage");
				}
			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text = ex.Message;
			}
		}
	}
}
