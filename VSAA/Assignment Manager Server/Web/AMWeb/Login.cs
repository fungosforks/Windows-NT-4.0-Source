//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//

namespace Microsoft.VisualStudio.Academic.AssignmentManager
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

	using System.Web.Security;
	using Microsoft.VisualStudio.Academic.AssignmentManager;


    /// <summary>
    ///    Summary description for Login.
    /// </summary>
    public class Login : System.Web.UI.Page
    {
		protected System.Web.UI.WebControls.Button btnLogin;
		protected System.Web.UI.WebControls.TextBox txtPassword;
		protected System.Web.UI.WebControls.Label lblPassword;
		protected System.Web.UI.WebControls.Label lblFeedback;
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.Label lblSubTitle;
		protected System.Web.UI.WebControls.Label lblConfirmPwd;
		protected System.Web.UI.WebControls.Label lblNewPwd;
		protected System.Web.UI.WebControls.TextBox txtUserName;
		protected System.Web.UI.WebControls.Label lblUserName;
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected System.Web.UI.HtmlControls.HtmlForm form1;
		protected System.Web.UI.HtmlControls.HtmlGenericControl copyText;
		protected System.Web.UI.HtmlControls.HtmlForm loginform;
		protected System.Web.UI.WebControls.TextBox txtNewPwd;
		protected System.Web.UI.WebControls.TextBox txtConfirmPwd;

		public string Title = SharedSupport.GetLocalizedString("AM_Title");

		public Login()
		{
			Page.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			try
			{
				if (!IsPostBack)
				{
				    //
				    // Evals true first time browser hits the page
				    //

					//initialize localized labels 
					LocalizeLabels();
				}
			}
			catch(Exception ex)
			{
				this.lblFeedback.Text = ex.Message;
			}
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            InitializeComponent();
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
		{
			this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}

		public void btnLogin_Click (object sender, System.EventArgs e)
		{
			try
			{
				// field validation
				lblFeedback.Text = String.Empty;
				if (this.txtUserName.Text.Trim() == String.Empty)
				{
					lblFeedback.Text = SharedSupport.GetLocalizedString("Login_InvalidCredentials");
					return;
				}
				if (this.txtPassword.Text.Trim() == String.Empty)
				{
					lblFeedback.Text = SharedSupport.GetLocalizedString("Login_InvalidCredentials");
					return;
				}
				
							
				//AuthenticateUser returns User
				UserM user = UserM.AuthenticateUser(this.txtUserName.Text.Trim(), this.txtPassword.Text.Trim());
				if (user.IsValid)
				{
					// Trigger off of field visibilty to determine whether we're updating with a new
					// password or are attempting a first-time login.
					if (!txtNewPwd.Visible)
					{
						// In this case, we've authenticated the user, and are attempting to log in
						// directly. Make sure that they're valid for direct login, though (i.e. that
						// they've already changed their password!).
						if(!user.ChangedPassword)
						{
							// Uncover the fields for the 'need to change password' funtionality
							// and update the text.
							lblConfirmPwd.Visible = true;
							lblNewPwd.Visible = true;
							txtNewPwd.Visible = true;
							txtConfirmPwd.Visible = true;
								
							this.lblConfirmPwd.Text = SharedSupport.GetLocalizedString("ChangePassword_ConfirmPwdHeader");
							this.lblNewPwd.Text = SharedSupport.GetLocalizedString("ChangePassword_NewPwdHeader1");
							this.lblPassword.Text = SharedSupport.GetLocalizedString("ChangePassword_CurrentPwdHeader");
							this.lblSubTitle.Text = SharedSupport.GetLocalizedString("Login_ChangePasswordSubTitle");
						}
						else 
						{
							// The user has already changed their password and can be logged directly
							// in, using forms based authentication and redirect; see config.web for 
							// authentication cookie config
							FormsAuthentication.RedirectFromLoginPage(user.UserID.ToString(),false);
						}
					}
					else
					{
						// They hadn't changed their password yet, so this is the second attempt to log
						// in. Since they authenticated, if the two entries for the new password match, 
						// update the entry in the database.
						if(this.txtConfirmPwd.Text == "")
						{
							throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_ConfirmPassword_RequiredField"));
						}
						else if (this.txtNewPwd.Text == "")
						{
							throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_NewPassword_RequiredField"));
						}
						if( (this.txtNewPwd.Text.Length < 4) || (this.txtNewPwd.Text.Length > 50) )
						{
							this.txtNewPwd.Text = "";
							this.txtConfirmPwd.Text = "";
							throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_PwdLengthError"));
						}

						// New password can't be the same as the previous password																								 																  
						if (this.txtNewPwd.Text == this.txtPassword.Text)
						{
							this.txtNewPwd.Text = "";
							this.txtConfirmPwd.Text = "";
							throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_PwdSameAsOld"));															
						}						
						
						if(this.txtNewPwd.Text != this.txtConfirmPwd.Text)
						{
							this.txtNewPwd.Text = "";
							this.txtConfirmPwd.Text = "";
							throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_ConfirmationError"));								
						}

						// Update the password in the server, setting the 'changed' flag.
						user.SetPassword(txtNewPwd.Text.Trim(), true);
						
						// Now, redirect the user back to what they were doing before we 
						// interrupted their login.
						FormsAuthentication.RedirectFromLoginPage(user.UserID.ToString(),false);
					}
				}
				else 
				{
					lblFeedback.Text = SharedSupport.GetLocalizedString("Login_InvalidCredentials");
				}				
			}
			catch(System.Exception ex)
			{
				lblFeedback.Text = ex.Message;
			}
		}
	
		protected void LocalizeLabels()
		{
			this.lblPassword.Text = SharedSupport.GetLocalizedString("Login_Password");
			this.lblUserName.Text = SharedSupport.GetLocalizedString("Login_Username1");
			this.btnLogin.Text = SharedSupport.GetLocalizedString("Login_Login1");
			this.lblSubTitle.Text = SharedSupport.GetLocalizedString("Login_Title1");
			lblFeedback.Text = "";
		 
		}
    }
}
