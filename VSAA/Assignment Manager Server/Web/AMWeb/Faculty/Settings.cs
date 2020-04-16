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
    ///    Summary description for Settings.
    /// </summary>
    public class Settings : System.Web.UI.Page
    {
		protected System.Web.UI.HtmlControls.HtmlTable tblBottomButtons;
		protected System.Web.UI.WebControls.Button btnCancel;
		protected System.Web.UI.WebControls.Button btnUpdate;
		protected System.Web.UI.HtmlControls.HtmlTable tblGeneralSetting;
		protected System.Web.UI.WebControls.RadioButton rbtnAutoGradeOff;
		protected System.Web.UI.WebControls.RadioButton rbtnAutoGradeOn;
		protected System.Web.UI.WebControls.RadioButton rbtnSMTPOff;
		protected System.Web.UI.WebControls.RadioButton rbtnSMTPOn;
		protected System.Web.UI.WebControls.RadioButton rbtnSSLOff;
		protected System.Web.UI.WebControls.RadioButton rbtnSSLOn;
		protected System.Web.UI.HtmlControls.HtmlTable tblGeneralSettingHeader;
		protected System.Web.UI.WebControls.HyperLink hlHelp1;
		protected System.Web.UI.WebControls.Label lblGeneralHeader;
		protected System.Web.UI.WebControls.Label lblDefaultPassword;
		protected System.Web.UI.WebControls.TextBox txtDefaultPassword;
		protected System.Web.UI.WebControls.Label lblUsingSMTP;
		protected System.Web.UI.WebControls.Label lblUsingSSL;
		protected System.Web.UI.WebControls.Label lblProcessingTime;
		protected System.Web.UI.WebControls.Label lblProjectSize;
		protected System.Web.UI.WebControls.TextBox txtProcessingTime;
		protected System.Web.UI.WebControls.TextBox txtProjectSize;
		
		public readonly string Title = SharedSupport.GetLocalizedString("AM_Title");

		// persist querystring parameters instead of referencing Request object every time needed
		private int courseId;
		protected AssignmentManager.UserControls.faculty Nav1;
		protected AssignmentManager.UserControls.goBack GoBack1;
		protected System.Web.UI.WebControls.Label lblAMTitle;
		protected System.Web.UI.WebControls.Label lblAutoGradeOnOff;

			
		public Settings()
		{
			Page.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			try
			{
				Nav1.Feedback.Text =  String.Empty;
				Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_SERVER_ADMIN;
				Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_SERVER_SETTINGS;
				Nav1.Title =SharedSupport.GetLocalizedString("Settings_Title1");
				Nav1.SubTitle = SharedSupport.GetLocalizedString("Settings_SubTitle1");
				Nav1.relativeURL = @"../";

				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vsurfServerAdministrationSettings");
				GoBack1.GoBackIncludeBack = false;
				GoBack1.GoBack_left = "400px";
				GoBack1.GoBack_top = "-15px";

				// grab CourseID parameter from the querystring
				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
				courseId = func.ValidateNumericQueryStringParameter(this.Request, "CourseID");
				
				if(courseId.Equals(null))
				{
					throw(new ArgumentException(SharedSupport.GetLocalizedString("Global_MissingParameter")));					
				}

				//Check Security Permissions

				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.SETTING_VIEW))
				{
					// Note that Redirect ends page execution.
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
				}
				
				if (!IsPostBack)
				{
				    //
				    // Evals true first time browser hits the page
				    //
					//Get localization string for all text displayed on the page
					LocalizeLabels();

					initializeFields();

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
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}

		public void btnCancel_Click (object sender, System.EventArgs e)
		{
				initializeFields();
		}

		public void btnUpdate_Click (object sender, System.EventArgs e)
		{try
		 {
			 if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.SETTING_EDIT))
			 {
				 throw new Exception(SharedSupport.GetLocalizedString("Global_Unauthorized"));
			 }

			 // AutoBuild/AutoCheck status are tied together and the SetSetting method
			 // will make sure the values remain in synch.  Changed after UI freeze.
			 // Determine if the user changed the setting
			 if (SharedSupport.GetSetting(Constants.AUTOBUILD_SETTING) == "True")
			 {
				 // did the user turn it off?
				 if (rbtnAutoGradeOff.Checked)
				 {
					 // turn if off
					 SharedSupport.SetSetting(Constants.AUTOBUILD_SETTING, "False");
					 // update the UI
					 //						 rbtnAutoGradeOff.Checked = true;
					 //						 rbtnAutoGradeOn.Checked = false;
				 }
			 }
			 else
			 {
				 // it was off, did the user turn it back on?
				 if (rbtnAutoGradeOn.Checked)
				 {
					 // turn it on
					 SharedSupport.SetSetting(Constants.AUTOBUILD_SETTING, "True");
					 // update the UI
					 //						 rbtnAutoGradeOn.Checked = true;
					 //						 rbtnAutoGradeOff.Checked = false;
				 }
			 }

			 if(this.txtProcessingTime.Text != "")
			 {
				 try
				 {
					 if(Convert.ToInt32(this.txtProcessingTime.Text) > 0)
					 {
						 if(Convert.ToInt32(txtProcessingTime.Text) <= Constants.MAX_PROCESS_LIMIT)
						 {
							 SharedSupport.SetSetting("MaxProcessTime", txtProcessingTime.Text);
						 }
						 else
						 {
							 string[] limit = new string[] {Constants.MAX_PROCESS_LIMIT.ToString()};
							 Nav1.Feedback.Text = SharedSupport.GetLocalizedString("Setting_MaxProcessingTime_Limit_Error", limit); 
						 }
					 }
				 }
				 catch
				 {					
					 Nav1.Feedback.Text = SharedSupport.GetLocalizedString("Setting_MaxProcessingTime_Error"); 
				 }
			 }
			 if(this.txtProjectSize.Text != "")
			 {
				 try
				 {
					 if(Convert.ToDouble(this.txtProjectSize.Text) > 0)
					 {
						 if (Convert.ToDouble(txtProjectSize.Text) <= Constants.MAX_PROJECT_SIZE)
						 {
							 SharedSupport.SetSetting("MaxUploadSize", txtProjectSize.Text);
						 }
						 else
						 {
							 string[] limit = new string[] {Constants.MAX_PROJECT_SIZE.ToString()};
							 Nav1.Feedback.Text = SharedSupport.GetLocalizedString("Setting_MaxProjectSize_Limit_Error", limit);
						 }
					 }
				 }
				 catch
				 {Nav1.Feedback.Text = SharedSupport.GetLocalizedString("Setting_MaxProjectSize_Error");}
			 }
		 }
		 catch(Exception ex)
		 {
			 Nav1.Feedback.Text =  ex.Message;
		 }
		}
		protected void initializeFields()
		{
			if(Convert.ToBoolean(SharedSupport.GetSetting(AssignmentManager.Constants.AUTOCHECK_SETTING)) == true)
					{	this.rbtnAutoGradeOff.Checked = false;
						this.rbtnAutoGradeOn.Checked = true;				
					}
					else
					{	this.rbtnAutoGradeOff.Checked = true;
						this.rbtnAutoGradeOn.Checked = false;
					}
					
					if(Convert.ToBoolean(SharedSupport.UsingSsl) == true)
					{	this.rbtnSSLOn.Checked = true;
						this.rbtnSSLOff.Checked = false;
					}
					else
					{	this.rbtnSSLOn.Checked = false;	
						this.rbtnSSLOff.Checked = true;
					}
								
					if(Convert.ToBoolean(SharedSupport.UsingSmtp) == true)
					{	this.rbtnSMTPOn.Checked = true;
						this.rbtnSMTPOff.Checked = false;			
					}
					else
					{ this.rbtnSMTPOn.Checked = false;	
					  this.rbtnSMTPOff.Checked = true;
					}
					
					this.txtProcessingTime.Text = Convert.ToString(SharedSupport.GetSetting(AssignmentManager.Constants.MAX_PROCESS_SETTING));
					this.txtProjectSize.Text = Convert.ToString(SharedSupport.GetSetting(AssignmentManager.Constants.MAX_PROJECT_SETTING));


		}
		protected void LocalizeLabels()
		{
			this.lblAutoGradeOnOff.Text = SharedSupport.GetLocalizedString("Setting_lblAutoGradeOnOff");//"Auto Grade: ";
			this.rbtnAutoGradeOff.Text = SharedSupport.GetLocalizedString("Setting_rbtnAutoGradeOff");//"Off";
			this.rbtnAutoGradeOn.Text = SharedSupport.GetLocalizedString("Setting_rbtnAutoGradeOn");//"On";
			this.rbtnSMTPOff.Text = SharedSupport.GetLocalizedString("Setting_rbtnSMTPdisabled");//"Disabled";
			this.rbtnSMTPOn.Text = SharedSupport.GetLocalizedString("Setting_rbtnSMTPenabled");//"Enabled";
			this.rbtnSSLOff.Text = SharedSupport.GetLocalizedString("Setting_rbtnSSLdisabled");//"Disabled";
			this.rbtnSSLOn.Text = SharedSupport.GetLocalizedString("Setting_rbtnSSLenabled");//"Enabled";
			this.btnUpdate.Text = SharedSupport.GetLocalizedString("Setting_btnUpdate"); //"Update";
			this.btnCancel.Text = SharedSupport.GetLocalizedString("Setting_btnCancel"); //"Cancel";
			this.lblUsingSMTP.Text = SharedSupport.GetLocalizedString("Setting_lblUsingSMTP");
			this.lblUsingSSL.Text = SharedSupport.GetLocalizedString("Setting_lblUsingSSL");
			this.lblProcessingTime.Text = SharedSupport.GetLocalizedString("Setting_MaxProcessingTime1");
			this.lblProjectSize.Text = SharedSupport.GetLocalizedString("Setting_MaxProjectSize");
			this.lblAMTitle.Text = SharedSupport.GetLocalizedString("Settings_AMTitle");
		}
	}
}

