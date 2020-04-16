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

namespace Microsoft.VisualStudio.Academic.AssignmentManager.Faculty
{
	/// <summary>
	/// Summary description for ImportFormPreview.
	/// </summary>
	public class ImportFormPreview : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lblMiddleName;
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.Label lblDescription;
		protected System.Web.UI.WebControls.Label lblUserName;
		protected System.Web.UI.WebControls.Label lblLastName;
		protected System.Web.UI.WebControls.Button btnImportRecords;
		protected System.Web.UI.WebControls.Button btnCancel;
		protected System.Web.UI.WebControls.Label lblUniversityID;
		protected System.Web.UI.WebControls.Label lblFirstName;
		protected System.Web.UI.WebControls.TextBox txtImportedFileLocation;
		protected System.Web.UI.WebControls.Label lblEmailAddress;

		protected System.Web.UI.WebControls.DropDownList cboMiddleName;
		protected System.Web.UI.WebControls.DropDownList cboUniversityID;
		protected System.Web.UI.WebControls.DropDownList cboEmailAddress;
		protected System.Web.UI.WebControls.DropDownList cboFirstName;
		protected System.Web.UI.WebControls.DropDownList cboUserName;
		protected System.Web.UI.WebControls.DropDownList cboLastName;
		protected System.Web.UI.WebControls.TextBox txtFileName;
	
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
		private string uploadedFilePath;
		private string delimitingCharacter;

		//user control declarations
		protected AssignmentManager.UserControls.faculty Nav1;
		protected AssignmentManager.UserControls.goBack GoBack1;

		public ImportFormPreview()
		{
			Page.Init += new System.EventHandler(Page_Init);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				//display nav bar 
				Nav1.Feedback.Text = String.Empty;
				Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT;
				Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_COURSE_USERS;
				Nav1.Title = SharedSupport.GetLocalizedString("AdminImport_Title");
				Nav1.SubTitle = SharedSupport.GetLocalizedString("ImportForm_SubTitle");
				Nav1.relativeURL = @"../";

				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vstskAddingCourseUsers");
				GoBack1.GoBackIncludeBack = true;
				GoBack1.GoBack_left = "400px";
				GoBack1.GoBack_top = "-15px";
				GoBack1.GoBack_BackURL = "ImportForm.aspx?CourseID=" + Request.QueryString.Get("CourseID");

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
					//clear combos
					cboEmailAddress.Items.Clear();
					cboFirstName.Items.Clear();
					cboLastName.Items.Clear();
					cboMiddleName.Items.Clear();
					cboUniversityID.Items.Clear();
					cboUserName.Items.Clear();			
				}
				//Localize all items
				localizeLabels();
			
				//grab items off of the querystring - DECODE them
				uploadedFilePath = SharedSupport.AddBackSlashToDirectory(Server.MapPath(Constants.ASSIGNMENTMANAGER_UPLOAD_DIRECTORY)) + Server.UrlDecode(Request.QueryString.Get("File"));
				delimitingCharacter = Server.UrlDecode(Request.QueryString.Get("Char"));

				//Create an instance of the dataset
				System.Data.DataSet ds = new System.Data.DataSet();

				//populate the combo boxes with items from the delimited file
				if(delimitingCharacter != "" && delimitingCharacter.Length < 2)
				{
					//Parse first line of file into dataset using delimiting character specified.
					ds = SharedSupport.ParseDelimitedFile(uploadedFilePath, delimitingCharacter, 1);
					//throw new System.IO.FileNotFoundException(SharedSupport.GetLocalizedString("User_UploadFileNotFound"));
										
				}
				else
				{
					throw(new ArgumentException(SharedSupport.GetLocalizedString("Global_MissingParameter")));
				}
		
				System.Data.DataSet dsNew = new System.Data.DataSet();
				System.Data.DataTable dtNew = dsNew.Tables.Add();
				dtNew.Columns.Add();
				for(int i=0;i<ds.Tables[0].Columns.Count;i++)
				{
					System.Data.DataRow drNew = dtNew.NewRow();
					drNew[0] = ds.Tables[0].Rows[0][i].ToString();
					dtNew.Rows.Add(drNew);
				}

				//populate combos
				if (!Page.IsPostBack) 
				{
					for(int i=0;i<dsNew.Tables[0].Rows.Count;i++)
					{
						int dropdownIndex = i + 1;
						ListItem newListItem = new ListItem(dsNew.Tables[0].Rows[i][0].ToString(), dropdownIndex.ToString());
						cboEmailAddress.Items.Add(newListItem);
						cboFirstName.Items.Add(newListItem);
						cboLastName.Items.Add(newListItem);
						cboMiddleName.Items.Add(newListItem);
						cboUniversityID.Items.Add(newListItem);
						cboUserName.Items.Add(newListItem);
					}
				}
		
				txtImportedFileLocation.Text = uploadedFilePath;
				
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

		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnImportRecords.Click += new System.EventHandler(this.btnImportRecords_Click);
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnImportRecords_Click(object sender, System.EventArgs e)
		{
			try
			{
				Nav1.Feedback.Text = String.Empty;
				//Validate delimiting character not blank
				if(delimitingCharacter == String.Empty)
				{
					Nav1.Feedback.Text = SharedSupport.GetLocalizedString("AdminImport_ChooseDelimitingChar");
					return;
				}

				System.Data.DataSet dsuser = SharedSupport.ParseDelimitedFile(uploadedFilePath, delimitingCharacter);
				//Grab the column order from the drop downs and put into string array
				
				int[] columns = new int[6];

				if(!cboLastName.SelectedIndex.Equals(0) && !cboLastName.SelectedIndex.Equals(-1))
				{
					if(!checkMultipleColumn(columns, cboLastName.SelectedIndex))
					{
						columns[0] = cboLastName.SelectedIndex;
					}
					else
					{
						throw new ApplicationException(SharedSupport.GetLocalizedString("AdminImport_ColumnOnce"));
					}
				}
				else
				{
					//throw required field error.
					throw new ApplicationException(SharedSupport.GetLocalizedString("AdminImport_MissingLastName"));
				}
				if(!cboFirstName.SelectedIndex.Equals(0) && !cboFirstName.SelectedIndex.Equals(-1))
				{
					if(!checkMultipleColumn(columns, cboFirstName.SelectedIndex))
					{
						columns[1] = cboFirstName.SelectedIndex;
					}
					else
					{
						throw new ApplicationException(SharedSupport.GetLocalizedString("AdminImport_ColumnOnce"));
					}
				}
				else
				{
					//throw required field error.
					throw new ApplicationException(SharedSupport.GetLocalizedString("AdminImport_MissingFirstName"));
				}
				if(!cboMiddleName.SelectedIndex.Equals(0) && !cboMiddleName.SelectedIndex.Equals(-1))
				{
					if(!checkMultipleColumn(columns, cboMiddleName.SelectedIndex))
					{
						columns[2] = cboMiddleName.SelectedIndex;
					}
					else
					{
						throw new ApplicationException(SharedSupport.GetLocalizedString("AdminImport_ColumnOnce"));
					}	
				}
				else
				{
					columns[2] = -1;
				}
				if(!cboEmailAddress.SelectedIndex.Equals(0) && !cboEmailAddress.SelectedIndex.Equals(-1))
				{
					if(!checkMultipleColumn(columns, cboEmailAddress.SelectedIndex))
					{
						columns[3] = cboEmailAddress.SelectedIndex;
					}
					else
					{
						throw new ApplicationException(SharedSupport.GetLocalizedString("AdminImport_ColumnOnce"));
					}
				}
				else
				{
					//throw required field error.
					throw new ApplicationException(SharedSupport.GetLocalizedString("AdminImport_MissingEmail"));
				}
				if(!cboUniversityID.SelectedIndex.Equals(0) && !cboUniversityID.SelectedIndex.Equals(-1))
				{
					if(!checkMultipleColumn(columns, cboUniversityID.SelectedIndex))
					{
						columns[4] = cboUniversityID.SelectedIndex;
					}
					else
					{
						throw new ApplicationException(SharedSupport.GetLocalizedString("AdminImport_ColumnOnce"));
					}
				}
				else
				{
					//throw required field error.
					throw new ApplicationException(SharedSupport.GetLocalizedString("AdminImport_MissingID"));
				}
				if(!cboUserName.SelectedIndex.Equals(0) && !cboUserName.SelectedIndex.Equals(-1))
				{
					if(!checkMultipleColumn(columns, cboUserName.SelectedIndex))
					{
						columns[5] = cboUserName.SelectedIndex;
					}
					else
					{
						throw new ApplicationException(SharedSupport.GetLocalizedString("AdminImport_ColumnOnce"));
					}
				}
				else
				{
					//throw required field error.
					throw new ApplicationException(SharedSupport.GetLocalizedString("AdminImport_MissingUserName"));
				}

				//Make sure that each column is only choosen once.
				//Grab the userId from the cookie
				int UserID = SharedSupport.GetUserIdentity();
				int importErrors = 0;
				int importSuccess = 0;
				string importID = System.Guid.NewGuid().ToString();
				for(int i=0; i< dsuser.Tables[0].Rows.Count; i++)
				{
					try
					{
						string userName = dsuser.Tables[0].Rows[i][columns[5]-1].ToString();
						// Does the user already exist?
						UserM userByName = UserM.LoadByUserName(userName);
						if (userByName.IsValid)
						{
							throw new Exception(SharedSupport.GetLocalizedString("User_UserNameMustBeUnique"));
						}
						UserM user = new UserM();
						user.LastName = dsuser.Tables[0].Rows[i][columns[0]-1].ToString();
						user.FirstName = dsuser.Tables[0].Rows[i][columns[1]-1].ToString();
						if(!columns[2].Equals(-1))
						{
							user.MiddleName = dsuser.Tables[0].Rows[i][columns[2]-1].ToString();
						}
						user.EmailAddress = dsuser.Tables[0].Rows[i][columns[3]-1].ToString();
						user.UniversityID = dsuser.Tables[0].Rows[i][columns[4]-1].ToString();
						user.UserName = userName;
						user.LastUpdatedUserID = UserID;
						user.LastUpdatedDate = DateTime.Now;
						user.ChangedPassword = false;
                        // create but do not mail out password.
						user.Create(false);
						if(!user.IsInCourse(courseId))
						{
							user.ImportToCourse(courseId, importID);
						}
						importSuccess++;
					}
					catch
					{
						importErrors++;
					}
				}

				//Delete imported file
				System.IO.File.Delete(uploadedFilePath);
				//Redirect to Results page.
				Response.Redirect("Results.aspx?CourseID=" + courseId.ToString() + "&ImportID=" + importID + "&Success=" + importSuccess + "&Errors=" + importErrors + "&Expected="+dsuser.Tables[0].Rows.Count, false);
			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text = SharedSupport.GetLocalizedString("AdminImport_GenericError");
			}
		}
		
		private bool checkMultipleColumn(int[] columns, int index)
		{
			// Check the SelectedIndex of the dropdown boxes to make sure a column is not selected twice.		
			for(int i=0;i<columns.Length;i++)
			{
				if(columns[i] == index)
				{
					return true;
				}
			}
				
			// If the item is not selected in other dropdown boxes return false.
			return false;
		}

		
		private void localizeLabels()
		{
			//Labels
			lblEmailAddress.Text = SharedSupport.GetLocalizedString("AdminImport_Email");
			lblFirstName.Text = SharedSupport.GetLocalizedString("AdminImport_FirstName");
			//lblImportedFileLocation.Text = SharedSupport.GetLocalizedString("AdminImport_ImportedFileLocation");
			lblLastName.Text = SharedSupport.GetLocalizedString("AdminImport_LastName");
			lblMiddleName.Text = SharedSupport.GetLocalizedString("AdminImport_MiddleName");
			lblUniversityID.Text = SharedSupport.GetLocalizedString("AdminImport_UniversityID");
			lblUserName.Text = SharedSupport.GetLocalizedString("AdminImport_UserName");
			lblTitle.Text = SharedSupport.GetLocalizedString("AdminImport_Title");
			lblDescription.Text = SharedSupport.GetLocalizedString("AdminImport_ImportFormPreview_SubTitle");
			//Buttons
			btnImportRecords.Text = SharedSupport.GetLocalizedString("AdminImport_ImportRecords");
			btnCancel.Text = SharedSupport.GetLocalizedString("AdminImport_Cancel");

			//Set readonly properties
			txtImportedFileLocation.ReadOnly = true;
			
			//add items to combo boxes
			if (!Page.IsPostBack)
			{
				//Set the default value for each of the dropdown menus.
				int dropdownIndex = 0; 
				ListItem newListItem = new ListItem(SharedSupport.GetLocalizedString("AdminImport_None"), dropdownIndex.ToString());
				
				cboEmailAddress.Items.Add(newListItem);
				cboFirstName.Items.Add(newListItem);
				cboLastName.Items.Add(newListItem);
				cboMiddleName.Items.Add(newListItem);
				cboUniversityID.Items.Add(newListItem);
				cboUserName.Items.Add(newListItem);
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			try
			{
				System.IO.File.Delete(uploadedFilePath);
				Response.Redirect("ImportForm.aspx?CourseID=" + Request.QueryString.Get("CourseID").ToString(), false);
			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text = ex.Message;
			}
		}	
	}
}
