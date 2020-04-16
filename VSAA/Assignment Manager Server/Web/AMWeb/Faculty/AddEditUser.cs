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
    ///    Summary description for AddEditUser.
    /// </summary>
    public class AddEditUser : System.Web.UI.Page
    {
		protected System.Web.UI.WebControls.HyperLink hlChangePassword;
		protected System.Web.UI.WebControls.Button btnCancel;
		protected System.Web.UI.WebControls.Button btnUpdate;
		protected System.Web.UI.WebControls.TextBox txtMiddleName;
		protected System.Web.UI.WebControls.Label lblMiddleName;
		protected System.Web.UI.WebControls.TextBox txtFirstName;
		protected System.Web.UI.WebControls.Label lblFirstName;
		protected System.Web.UI.WebControls.TextBox txtLastName;
		protected System.Web.UI.WebControls.Label lblLastName;
		protected System.Web.UI.WebControls.TextBox txtUserName;
		protected System.Web.UI.WebControls.Label lblUserName;
		protected System.Web.UI.WebControls.Button btnFind;
		protected System.Web.UI.WebControls.TextBox txtUniversityIdentifier;
		protected System.Web.UI.WebControls.Label lblUniversityIdentifier;
		protected System.Web.UI.WebControls.TextBox txtEMailAddress;
		protected System.Web.UI.WebControls.Label lblEmailAddress;
		protected System.Web.UI.HtmlControls.HtmlTable tblGeneralAssignment;
		protected AssignmentManager.UserControls.faculty Nav1;
		protected System.Web.UI.WebControls.Label lblUserDetails;
		protected System.Web.UI.WebControls.Label lblDescription;
		protected System.Web.UI.WebControls.Label lblPasswordText;
		protected System.Web.UI.WebControls.Label lblFindInstructions;
		protected System.Web.UI.WebControls.Label lblRequired;

		private string NO_COURSEID_ERROR = SharedSupport.GetLocalizedString("AddEditUser_NO_COURSEID_ERROR"); //"There was no CourseID passed on the query string.";
		private string INVALID_COURSEID_ERROR = SharedSupport.GetLocalizedString("AddEditUser_INVALID_COURSEID_ERROR"); //"There was no record corresponding to the passed CourseID.";
		private string NO_USER_FOR_USERID_ERROR = SharedSupport.GetLocalizedString("AddEditUser_NO_USER_FOR_USERID_ERROR"); //"There was no existing user for the UserID on the query string.  Can not update user.";
		
		public readonly string Title = SharedSupport.GetLocalizedString("AM_Title");

		protected AssignmentManager.UserControls.goBack GoBack1;
		// persist querystring parameters instead of referencing Request object every time needed
		private int courseId;
		protected System.Web.UI.HtmlControls.HtmlImage Img1;
		protected System.Web.UI.HtmlControls.HtmlImage Img2;
		protected System.Web.UI.HtmlControls.HtmlImage Img3;
		protected System.Web.UI.HtmlControls.HtmlImage Img4;
		private int userId;
		protected System.Web.UI.WebControls.RadioButtonList UserRolesList;
		protected System.Web.UI.HtmlControls.HtmlInputButton btnDelete;
		protected System.Web.UI.HtmlControls.HtmlInputText txtAction;
		protected System.Web.UI.HtmlControls.HtmlForm frm;
		protected System.Web.UI.WebControls.Label lblUserRoles;
		
		public AddEditUser()
		{
			Page.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			try
			{
				// Do not cache this page
				Response.Cache.SetNoStore();

				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
				// Look for UserId parameter in the query string.
				userId = func.ValidateNumericQueryStringParameter(this.Request, "UserID");

				if(userId == -1 || userId == SharedSupport.GetUserIdentity())
				{
					//Editing self
					Nav1.Feedback.Text =  String.Empty;
					Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_SERVER_ADMIN;
					Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_SERVER_MYACCOUNT;
					Nav1.Title = " ";
					Nav1.SubTitle = SharedSupport.GetLocalizedString("MyAccountEdit_SubTitle");
					Nav1.relativeURL = @"../";

					lblUserDetails.Visible = false;
					lblDescription.Visible = false;
					lblPasswordText.Visible = false;
					lblFindInstructions.Visible = false;
					btnFind.Enabled = false;
					btnFind.Visible = false;
				}
				else
				{
					Nav1.Feedback.Text =  "&nbsp;";
					Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT;
					Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_COURSE_USERS;
					Nav1.relativeURL = @"../";
				}

				GoBack1.GoBack_left = "400px";
				GoBack1.GoBack_top = "-2px";		
				if (userId == 0) 
				{
					// Set help link to AddUser help topic.
					GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vstskAddingCourseUsers");
				}
				else if (userId == -1)
				{
					userId = SharedSupport.GetUserIdentity();
					GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vstskAccessingYourAccount");
				}
				else
				{
					// Set help link to EditUser help topic.
					GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vstskAccessingYourAccount");
				}
				
				GoBack1.GoBackIncludeBack = true;
				if(Request.UrlReferrer != null && Request.UrlReferrer.ToString() != "")
				{
					if(Request.UrlReferrer.ToString().IndexOf("UserRoles") > 0)
					{
						GoBack1.GoBack_BackURL = "Users.aspx?" + Request.QueryString.ToString();
					}
					else
					{
						GoBack1.GoBack_BackURL = Request.UrlReferrer.ToString();
					}
				}
				else
				{
					GoBack1.GoBack_BackURL = "Users.aspx?" + Request.QueryString.ToString();
				}
				
				// grab CourseID parameter from the querystring
				courseId = func.ValidateNumericQueryStringParameter(this.Request, "CourseID");

				if(userId == 0 )
				{
					if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USER_ADD))
					{
						// Note that Redirect ends page execution.
						Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
					} 
				}
				else
				{
					if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USER_EDIT))
					{
						// Note that Redirect ends page execution.
						Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
					}
				}


				//Check Security to see if we should disable roles
				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.SECURITY_EDIT))
				{
					UserRolesList.Enabled = false;
				}

				//Check Security to see if we should hide roles
				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.SECURITY_VIEW))
				{
					lblUserRoles.Visible = false;
					UserRolesList.Visible = false;
				}

				showLinks();

				if (!IsPostBack)
				{
					//
					// Evals true first time browser hits the page
					//

					LocalizeLabels();

					SetupRoleButtons();

					btnUpdate.Visible = true;
					Nav1.Feedback.Text =  String.Empty;

					// was userId passed in on querystring?
					if(userId != 0)
					{
						this.lblDescription.Visible = false;
						this.lblPasswordText.Visible = false;
						//Load existing User
						UserM user = UserM.Load(userId);
						if(user.IsValid)
						{
							// is user a member of the course? change button to 'add' if not
							if (user.IsInCourse(courseId))
							{
								//User is already added to the course, so show delete button
								this.btnDelete.Visible = true;
								btnUpdate.Text = SharedSupport.GetLocalizedString("AddEditUser_Update");
							}
							else
							{
								btnUpdate.Text = SharedSupport.GetLocalizedString("AddEditUser_Insert");
							}

							populateFields(user);
							this.lblUserDetails.Text = SharedSupport.GetLocalizedString("AddEditUser_UserDetails");
						}
						else
						{
							clearFields();
						}
					}
					else
					{
						// Insert
						//Prep for inserting a new user						
						btnUpdate.Text = SharedSupport.GetLocalizedString("AddEditUser_Insert");						
						this.lblUserDetails.Text = SharedSupport.GetLocalizedString("AddEditUser_AddLookUpUsers");
					}
				}
				else
				{
					if(txtAction.Value == "DeleteUser")
					{
						if(SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USER_EDIT))
						{
							UserM.RemoveFromCourse(userId,courseId);
							Response.Redirect("Users.aspx?UserID=" + userId.ToString() + "&" + Request.QueryString.ToString(), false);
						}
						else
						{
							Nav1.Feedback.Text = SharedSupport.GetLocalizedString("Global_Unauthorized");
						}
					}
				}
			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text =  ex.Message.ToString();
				btnUpdate.Visible = false;
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
			this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}

		public void btnFind_Click (object sender, System.EventArgs e)
		{
			try
			{		
				Nav1.Feedback.Text =  String.Empty;
				UserM user = null;
				if(txtUserName.Text != null  && !txtUserName.Text.Trim().Equals(String.Empty))
				{
					user = UserM.LoadByUserName(txtUserName.Text.Trim());
				}
				else if(txtUniversityIdentifier.Text != null && !txtUniversityIdentifier.Text.Trim().Equals(String.Empty))
				{
					user = UserM.LoadByUniversityID(txtUniversityIdentifier.Text.Trim());
				}
				else if(txtEMailAddress != null && !txtEMailAddress.Text.Trim().Equals(String.Empty))
				{
					user = UserM.LoadByEmail(txtEMailAddress.Text.Trim());
				}

				if(user != null && user.IsValid)
				{
					//Populate result
					Response.Redirect("AddEditUser.aspx?CourseID=" + courseId + "&UserID=" + user.UserID, false);
				}
				else
				{
					Nav1.Feedback.Text =  SharedSupport.GetLocalizedString("AddEditUser_NoUserRecord") + Server.HtmlEncode(txtUserName.Text.ToString());
					clearFields();
				}
			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text =  ex.Message.ToString();
				btnUpdate.Visible = false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"> </param>
		/// <param name="e"> </param>
		public void btnUpdate_Click (object sender, System.EventArgs e)
		{				
			try
			{
				//reset error handling label
				Nav1.Feedback.Text =  String.Empty;
				checkErrorCases();

				UserM user = null;
				//Save Updated or New User - check for UserID on query string
				if(userId != 0)
				{
					if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USER_EDIT))
					{
						throw new Exception(SharedSupport.GetLocalizedString("Global_Unauthorized"));
					}

					//Update
					user = UserM.Load(userId);
					
					if(user.IsValid)
					{
						//Save updated user
						user.EmailAddress = txtEMailAddress.Text.ToString();
						user.FirstName = txtFirstName.Text.ToString();
						user.LastName = txtLastName.Text.ToString();
						user.LastUpdatedDate = DateTime.Now;
						user.LastUpdatedUserID = SharedSupport.GetUserIdentity();
						user.MiddleName = txtMiddleName.Text.ToString();
						user.UniversityID = txtUniversityIdentifier.Text.ToString();
						user.UserName = txtUserName.Text.ToString();

						user.Update();

						if(user.IsInCourse(courseId))
						{
							if(SharedSupport.SecurityIsAllowed(courseId, SecurityAction.SECURITY_EDIT))
							{
								int roleid = Convert.ToInt32(UserRolesList.SelectedItem.Value);
								RoleM currentUsersRole = RoleM.GetUsersRoleInCourse(SharedSupport.GetUserIdentity(), courseId);

								// The lower role => greater permissions
								if( (currentUsersRole.ID == (int)PermissionsID.Admin) || (currentUsersRole.ID < roleid) )
								{
									user.SetRoleInCourse(courseId, roleid);
								}
								else
								{
									throw new Exception(SharedSupport.GetLocalizedString("AddEditUser_ErrorRolePermissionDenied"));
								}
							}
						}
						else
						{
							// Add user to Course
							PermissionsID permission = PermissionsID.Student;
							if(SharedSupport.SecurityIsAllowed(courseId, SecurityAction.SECURITY_EDIT))
							{
								int roleid = Convert.ToInt32(UserRolesList.SelectedItem.Value);
								RoleM currentUsersRole = RoleM.GetUsersRoleInCourse(SharedSupport.GetUserIdentity(), courseId);

								// The lower role => greater permissions
								// Note: Cannot change the permission of someone at your level.
								if( (currentUsersRole.ID == (int)PermissionsID.Admin) || (currentUsersRole.ID < roleid) )
								{
									permission = (PermissionsID)roleid;
									user.AddToCourse(courseId, permission);
								}
								else
								{
									throw new Exception(SharedSupport.GetLocalizedString("AddEditUser_ErrorRolePermissionDenied"));
								}
							}
						}
						btnUpdate.Text = SharedSupport.GetLocalizedString("AddEditUser_Update");
						Nav1.Feedback.Text =  SharedSupport.GetLocalizedString("AddEditUser_UserUpdated"); //"User has been Updated.";
					}
					else
					{
						throw new Exception(NO_USER_FOR_USERID_ERROR);
					}
				}
				else
				{
					if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.USER_ADD))
					{
						throw new Exception(SharedSupport.GetLocalizedString("Global_Unauthorized"));
					}

					//Insert
					user = new UserM();

					user.EmailAddress = txtEMailAddress.Text.ToString();
					user.FirstName = txtFirstName.Text.ToString();
					user.LastName = txtLastName.Text.ToString();
					user.LastUpdatedDate = DateTime.Now;
					user.LastUpdatedUserID = SharedSupport.GetUserIdentity();
					user.MiddleName = txtMiddleName.Text.ToString();
					user.UniversityID = txtUniversityIdentifier.Text.ToString();
					user.UserName = txtUserName.Text.ToString();
					user.ChangedPassword = false;

					// Does the user already exist?
					UserM userByName = UserM.LoadByUserName(user.UserName);
					if (!userByName.IsValid)
					{					
						userId = user.Create();
	
						btnUpdate.Text = SharedSupport.GetLocalizedString("AddEditUser_Update");
						Nav1.Feedback.Text =  SharedSupport.GetLocalizedString("AddEditUser_UserInserted"); //"User has been inserted.";

						PermissionsID permission = PermissionsID.Student;
						if(SharedSupport.SecurityIsAllowed(courseId, SecurityAction.SECURITY_EDIT))
						{
							int roleid = Convert.ToInt32(UserRolesList.SelectedItem.Value);
							RoleM currentUsersRole = RoleM.GetUsersRoleInCourse(SharedSupport.GetUserIdentity(), courseId);

							// The lower role = greater permissions 
							// Note: Can't change permissions of someone equal in level to you.
							if( (currentUsersRole.ID == (int)PermissionsID.Admin) || (currentUsersRole.ID < roleid) )
							{
								permission = (PermissionsID)roleid;
							}
							else
							{
								throw new Exception(SharedSupport.GetLocalizedString("AddEditUser_ErrorRolePermissionDenied"));
							}
						}
						user.AddToCourse(courseId,permission);
					}
					else
					{
						throw new Exception(SharedSupport.GetLocalizedString("User_UserNameMustBeUnique"));
					}
				}				

				Response.Redirect("Users.aspx?UserID=" + userId.ToString() + "&" + Request.QueryString.ToString(), false);

			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text =  ex.Message.ToString();
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"> </param>
		/// <param name="e"> </param>
		public void btnCancel_Click (object sender, System.EventArgs e)
		{
			//Cancel the current update or insert
			//Redirect bact to the user page
			//Make sure to pass course ID back with you...
			
			Response.Redirect("Users.aspx?" + Request.QueryString.ToString(), false);
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ds"> </param>
		private void populateFields(UserM user)
		{
			if(user.EmailAddress != null && user.EmailAddress != String.Empty) 
			{	
				txtEMailAddress.Text = user.EmailAddress;
			}
			
			if(user.FirstName != null && user.FirstName != String.Empty) 
			{
				txtFirstName.Text = user.FirstName;
			}
			
			if(user.LastName != null && user.LastName != String.Empty) 
			{
				txtLastName.Text = user.LastName;
			}
			
			if(user.MiddleName != null && user.MiddleName != String.Empty) 
			{
				txtMiddleName.Text = user.MiddleName;
			}
			
			if(user.UniversityID != null && user.UniversityID != String.Empty)
			{
				txtUniversityIdentifier.Text = user.UniversityID;
			}
			
			if(user.UserName != null && user.UserName != "")
			{
				txtUserName.Text = user.UserName;
			}

			//Set current role
			RoleM role = user.GetRoleInCourse(courseId);
			for(int i=0;i<UserRolesList.Items.Count;i++)
			{
				if(UserRolesList.Items[i].Value == role.ID.ToString())
				{
					UserRolesList.SelectedIndex = i;
					break;
				}
			}

			if( role.ID > 0)
			{
				RoleM currentUsersRole = RoleM.GetUsersRoleInCourse(SharedSupport.GetUserIdentity(), courseId);
				//Note: Can't change the role of someone = in level to you.
				if( (currentUsersRole.ID > (int)PermissionsID.Admin) && (currentUsersRole.ID >= role.ID) )
				{
					UserRolesList.Enabled = false;
				}
			}
		}
			
		/// <summary>
		/// 
		/// </summary>
		private void clearFields()
		{
			btnUpdate.Text = SharedSupport.GetLocalizedString("AddEditUser_Insert"); //"Insert";
			this.lblUserDetails.Text = SharedSupport.GetLocalizedString("AddEditUser_AddLookUpUsers"); //"Add/Look Up Users";
			this.lblDescription.Text = SharedSupport.GetLocalizedString("AddEditUser_EnterCreateUserInfo");
			this.lblFindInstructions.Text = SharedSupport.GetLocalizedString("AddEditUser_EnterFindUserInfo");

			// The password generation mechanism is dependant upon the whether SMTP can be used to inform
			// the user of their new password.
			if (Convert.ToBoolean(SharedSupport.UsingSmtp)) 
			{
				lblPasswordText.Text =  SharedSupport.GetLocalizedString("AddEditUser_UserInsertedEmailSent");
			} 
			else 
			{
				lblPasswordText.Text =  SharedSupport.GetLocalizedString("AddEditUser_UserInsertedPasswordIsName");
			}
				
			txtEMailAddress.Text = String.Empty;
			txtFirstName.Text = String.Empty;
			txtLastName.Text = String.Empty;
			txtMiddleName.Text = String.Empty;
			txtUniversityIdentifier.Text = String.Empty;
			txtUserName.Text = String.Empty;
		}

		private void showLinks()
		{
			if(userId.Equals(null) || userId.Equals(0))
			{
				hlChangePassword.Visible = false;
			}
			else
			{
				hlChangePassword.Visible = true;
				hlChangePassword.NavigateUrl = "ChangePassword.aspx?CourseID=" + courseId.ToString() + "&UserID=" + userId.ToString();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected void LocalizeLabels()
		{
			clearFields();
			this.btnCancel.Text = SharedSupport.GetLocalizedString("AddEditUser_btnCancel"); //"Cancel";
			this.btnFind.Text = SharedSupport.GetLocalizedString("AddEditUser_Find_Text"); //"Find";
			this.lblEmailAddress.Text = SharedSupport.GetLocalizedString("AddEditUser_lblEmailAddress"); //"E-mail Address: ";
			this.lblFirstName.Text = SharedSupport.GetLocalizedString("AddEditUser_lblFirstName"); //"First Name: ";
			this.lblLastName.Text = SharedSupport.GetLocalizedString("AddEditUser_lblLastName"); //"Last Name: ";
			this.lblMiddleName.Text = SharedSupport.GetLocalizedString("AddEditUser_lblMiddleName"); //"Middle Name: ";
			this.lblUniversityIdentifier.Text = SharedSupport.GetLocalizedString("AddEditUser_lblUniversityIdentifier"); //"University Identifier: ";
			this.lblUserName.Text = SharedSupport.GetLocalizedString("AddEditUser_lblUserName"); //"User Name: ";
			this.lblUserRoles.Text = SharedSupport.GetLocalizedString("AddEditUser_lblRole");
			this.txtEMailAddress.Text = String.Empty;
			this.txtFirstName.Text = String.Empty;
			this.txtLastName.Text = String.Empty;
			this.txtMiddleName.Text = String.Empty;
			this.txtUniversityIdentifier.Text = String.Empty;
			this.txtUserName.Text = String.Empty;
			this.hlChangePassword.Text = SharedSupport.GetLocalizedString("AddEditUser_ChangePassword");
			this.lblRequired.Text = SharedSupport.GetLocalizedString("Global_RequiredFieldIndicator");
			this.btnDelete.Value = SharedSupport.GetLocalizedString("AddEditUser_btnDeleteUser");
		}

		private void SetupRoleButtons()
		{
			RoleM[] roles = RoleM.GetAllRoles();
			if(roles.Length == 0)
			{
				UserRolesList.Visible = false;
			}
			else
			{
				foreach(RoleM role in roles)
				{
					ListItem item = new ListItem();
					item.Text = Server.HtmlEncode(role.Name);
					item.Value = Server.HtmlEncode(role.ID.ToString());
					if(item.Value == ((int)PermissionsID.Student).ToString())
					{
						item.Selected = true;
					}
					UserRolesList.Items.Add(item);
				}
			}
		}

		private void checkErrorCases()
		{
			if( txtUserName.Text == String.Empty )
			{
				throw new Exception(SharedSupport.GetLocalizedString("User_UserNameEmpty"));
			}
			else if( txtUserName.Text.Length < 4 || txtUserName.Text.Length > 50 )
			{
				throw new Exception(SharedSupport.GetLocalizedString("User_UserNameFailedBR"));
			}
			else if( txtEMailAddress.Text == String.Empty )
			{
				throw new Exception(SharedSupport.GetLocalizedString("User_EmailEmpty"));
			}
			else if( txtEMailAddress.Text.Length > 100 )
			{
				throw new Exception(SharedSupport.GetLocalizedString("User_EmailMoreThan100Char"));
			}
			else if( txtUniversityIdentifier.Text == String.Empty )
			{
				throw new Exception(SharedSupport.GetLocalizedString("User_UniversityIdentifierEmpty"));
			}
			else if( txtUniversityIdentifier.Text.Length > 50 )
			{
				throw new Exception(SharedSupport.GetLocalizedString("User_UniversityIdentifierTooBig"));
			}
		}
    }
}
