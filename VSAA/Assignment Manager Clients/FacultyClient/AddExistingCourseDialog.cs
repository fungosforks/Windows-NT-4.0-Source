//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using EnvDTE;
using AssignmentManager.ClientUI;

namespace FacultyClient
{
	/// <summary>
	/// Summary description for AddExistingCourseDialog.
	/// </summary>
	public class AddExistingCourseDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtServer;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label lblAMUrl;

		private EnvDTE._DTE m_applicationObject = null;
		public AddExistingCourseDialog(EnvDTE._DTE dte)
		{
			//
			// Required for Windows Form Designer support
			//
			m_applicationObject = dte;
			InitializeComponent();
			txtServer.Focus();
			LocalizeLabels();
		}

		private void AddCourse()
		{

			if (this.txtServer.Text == String.Empty || this.txtServer.Text == "http://")
			{
				//User hasn't entered the information, so display error dialog
				MessageBox.Show( AMResources.GetLocalizedString("AddExistingCourse_ErrorBlankField"),
					this.Text,
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
				return;
			}

			FacultyTools fac = new FacultyTools(m_applicationObject);
			string url = txtServer.Text;
			string versionURL = "";
			try
			{
				string strServer;
				if (url.StartsWith("http://"))
				{
					versionURL = "http://";
					strServer = url.Substring(7);
				}
				else if (url.StartsWith("https://"))
				{
					versionURL = "https://";
					strServer = url.Substring(8);
				}
				else
				{
					versionURL = "http://";
					strServer = url;
				}

				int nIndex = strServer.IndexOf("/");
				if (nIndex == -1) 
				{
					throw new System.ArgumentException();
				}
				
				//find the second one
				nIndex = strServer.IndexOf("/", nIndex+1);
				
				strServer = strServer.Substring(0, nIndex);
				versionURL += strServer + "/amversion.xml";
			}
			catch
			{
				versionURL = "";
			}

			if (versionURL == "" || !fac.CheckVersion(versionURL))
			{
				return;
			}

			string courseUrl = fac.RegisterCourseFromUrl(url);
			if (courseUrl == null)
			{
				string errorMessage = AMResources.GetLocalizedString("AddExisting_Error_UnableToRegisterExistingCourse");
				if (errorMessage != String.Empty)
				{
					MessageBox.Show(errorMessage, "Add Existing Course", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
			else
			{
				m_applicationObject.ItemOperations.Navigate(courseUrl, vsNavigateOptions.vsNavigateOptionsDefault);
				this.Close();
			}
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtServer = new System.Windows.Forms.TextBox();
			this.lblAMUrl = new System.Windows.Forms.Label();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtServer
			// 
			this.txtServer.AccessibleName = "txtServer";
			this.txtServer.Location = new System.Drawing.Point(8, 24);
			this.txtServer.Name = "txtServer";
			this.txtServer.Size = new System.Drawing.Size(296, 20);
			this.txtServer.TabIndex = 1;
			this.txtServer.Text = "http://";
			// 
			// lblAMUrl
			// 
			this.lblAMUrl.AccessibleName = "lblAMUrl";
			this.lblAMUrl.Location = new System.Drawing.Point(8, 8);
			this.lblAMUrl.Name = "lblAMUrl";
			this.lblAMUrl.Size = new System.Drawing.Size(288, 16);
			this.lblAMUrl.TabIndex = 12;
			this.lblAMUrl.Text = "Assignment Manager URL (http:// or https://)";
			// 
			// btnAdd
			// 
			this.btnAdd.AccessibleName = "btnAdd";
			this.btnAdd.Location = new System.Drawing.Point(120, 56);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(88, 24);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "&Add";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.AccessibleName = "btnCancel";
			this.btnCancel.Location = new System.Drawing.Point(216, 56);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(88, 24);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// AddExistingCourseDialog
			// 
			this.AcceptButton = this.btnAdd;
			this.AccessibleName = "AddExistingCourseDialog";
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(312, 85);
			this.ControlBox = false;
			this.Controls.Add(this.txtServer);
			this.Controls.Add(this.lblAMUrl);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "AddExistingCourseDialog";
			this.Text = "Add Existing Course";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AddExistingCourseDialog_HelpRequested);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			AddCourse();
		}


		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void LocalizeLabels()
		{
			lblAMUrl.Text = AMResources.GetLocalizedString("AddExistingCourse_LblAMUrl");
			btnAdd.Text = AMResources.GetLocalizedString("BtnAdd");
			btnCancel.Text = AMResources.GetLocalizedString("BtnCancel");
			txtServer.Text = AMResources.GetLocalizedString("HttpText");
			this.Text = AMResources.GetLocalizedString("AddExistingCourse_DialogCaption");
		}

		private void AddExistingCourseDialog_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
		{
			Microsoft.VisualStudio.VSHelp.Help help = (Microsoft.VisualStudio.VSHelp.Help)m_applicationObject.GetObject("Help");
			help.DisplayTopicFromF1Keyword("vs.AMAddExistingCourse");
		}
	}
}
