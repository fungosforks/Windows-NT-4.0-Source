//
// Copyright (c) Microsoft Corporation.  All rights reserved.
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
using System.Security;

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
		protected System.Web.UI.WebControls.Button btnCancel;
		protected System.Web.UI.WebControls.Button btnSave;
		protected System.Web.UI.WebControls.Label lblRequired;
		protected AssignmentManager.UserControls.faculty Nav1;
		protected AssignmentManager.UserControls.goBack GoBack1;
		
		public readonly string ChangePassword_AMTitle = SharedSupport.GetLocalizedString("ChangePassword_AMTitle");
		public readonly string Title = SharedSupport.GetLocalizedString("AM_Title");
		protected System.Web.UI.HtmlControls.HtmlImage Img4;
		protected System.Web.UI.HtmlControls.HtmlImage Img1;
		protected System.Web.UI.HtmlControls.HtmlImage Img3;

		// persist querystring parameters instead of referencing Request object every time needed
		private int courseId;
	
		public ChangePassword()
		{
			Page.Init += new System.EventHandler(Page_Init);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{	
				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
				// grab CourseID parameter from the querystring

				int UserID = func.ValidateNumericQueryStringParameter(this.Request, "UserID");
				if( UserID == SharedSupport.GetUserIdentity() )
				{
					Nav1.Feedback.Text =  String.Empty;
					Nav1.relativeURL = @"../";
					Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_SERVER_ADMIN;
					Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_SERVER_MYACCOUNT;
					Nav1.Title = SharedSupport.GetLocalizedString("MyAccountChangePassword_Title"); 
					Nav1.SubTitle = SharedSupport.GetLocalizedString("MyAccountChangePassword_SubTitle"); 
				}
				else
				{
					Nav1.Feedback.Text = String.Empty;
					Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT;
					Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_COURSE_USERS;
					Nav1.relativeURL = @"../";
					Nav1.Title = " ";
					Nav1.SubTitle = SharedSupport.GetLocalizedString("ChangePassword_SubTitle2"); 
				}
				GoBack1.GoBackIncludeBack = true;
				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vstskAccessingYourAccount");
				GoBack1.GoBack_BackURL = "AddEditUser.aspx?" + Request.QueryString.ToString();

				courseId = func.ValidateNumericQueryStringParameter(this.Request, "CourseID");
					
				if(courseId <= 0)
				{
					throw(new ArgumentException(SharedSupport.GetLocalizedString("Global_MissingParameter")));					
				}

				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USER_EDIT))
				{
					// Note that Redirect ends page execution.
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
				}

				// if using SSL and the page isn't using a secure connection, redirect to https
				if(SharedSupport.UsingSsl == true && Request.IsSecureConnection == false)
				{
					// Note that Redirect ends page execution.
					Response.Redirect("https://" + SharedSupport.BaseUrl + "/faculty/ChangePassword.aspx?CourseID=" + courseId.ToString());	
				}

				//get Course Short Name to display as title of page
				CourseM course = CourseM.Load(courseId);
				if(course.IsValid)
				{
					Nav1.Title = course.Name;
				}
					
				if (!IsPostBack)
				{
					// Evals true first time browser hits the page
					LocalizeLabels();
				}

				Response.Cache.SetNoStore();
				if(!UserID.Equals(null))
				{
					UserM user = UserM.Load(UserID);
					if (user.IsValid)
					{
						this.lblUserName.Text = Server.HtmlEncode(user.FirstName + " " + user.LastName);
					}
				}
			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text = ex.Message;
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
			this.lblConfirmPwd.Text = SharedSupport.GetLocalizedString("ChangePassword_ConfirmPwdHeader");
			this.lblNewPwd.Text = SharedSupport.GetLocalizedString("ChangePassword_NewPwdHeader1");
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
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			try
			{
				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
				
				if(this.txtConfirmPwd.Text == "")
				{
					throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_ConfirmPassword_RequiredField"));
				}
				else if (this.txtNewPwd.Text == "")
				{
					throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_NewPassword_RequiredField"));
				}
				else if( (this.txtNewPwd.Text.Trim().Length < 4) || (this.txtNewPwd.Text.Trim().Length > 50))
				{
					throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_PwdLengthError"));
				}
				if(this.txtNewPwd.Text != this.txtConfirmPwd.Text)
				{
					this.txtNewPwd.Text = "";
					this.txtConfirmPwd.Text = "";
					throw new Exception(SharedSupport.GetLocalizedString("ChangePassword_ConfirmationError"));								
				}

				int UserID = func.ValidateNumericQueryStringParameter(this.Request, "UserID");
				int courseId = func.ValidateNumericQueryStringParameter(this.Request, "CourseID");

				if(UserID != 0)
				{
					int currentUserID = SharedSupport.GetUserIdentity();
					if (currentUserID == 0)
					{
						throw new Exception(SharedSupport.GetLocalizedString("Global_Unauthorized"));
					}

					if (currentUserID == UserID)
					{
						// you are always allowed to change your own password.
						setNewPassword(currentUserID);
					}
					else 
					{
						if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USER_EDIT))
						{
							// Note that Redirect ends page execution.
							Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
						}
						try
						{
							RoleM currentUsersRole = RoleM.GetUsersRoleInCourse(currentUserID, courseId);
							RoleM targetUsersRole = RoleM.GetUsersRoleInCourse(UserID, courseId);
							
							//Lower ID = more permissions
							if( currentUsersRole.ID <= targetUsersRole.ID )
							{
								setNewPassword(UserID);
							}
							else
							{
								throw new Exception();
							}
						}
						catch(Exception)
						{
							throw new Exception(SharedSupport.GetLocalizedString("Global_Unauthorized"));
						}
					}
					

					Response.Redirect(@"AddEditUser.aspx?UserID=" + UserID + "&CourseID=" + courseId);
				}
			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text = ex.Message;
			}
		}

		private void setNewPassword(int userID)
		{
			UserM user = UserM.Load(userID);
			if (user.IsValid)
			{
				// If user is changing their own password, then set HasChanged flag.
				bool hasChanged = (user.UserID == SharedSupport.GetUserIdentity());
				user.SetPassword(txtNewPwd.Text.Trim(), hasChanged);
				Nav1.Feedback.Text = SharedSupport.GetLocalizedString("MyAccountChangePassword_Successful");
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
			int courseId = func.ValidateNumericQueryStringParameter(this.Request, "CourseID");
			int userId = func.ValidateNumericQueryStringParameter(this.Request, "UserID");

			Response.Redirect(@"AddEditUser.aspx?UserID=" + userId + "&CourseID=" + courseId, false);
		}
	}
}
