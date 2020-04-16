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
	/// Summary description for AddCourse.
	/// </summary>
	public class AddCourseDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtCourseName;
		private System.Windows.Forms.TextBox txtServer;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnCancel;

		EnvDTE._DTE m_applicationObject = null;
		private System.Windows.Forms.Label lblCourseName;
		private System.Windows.Forms.Label lblAMUrl;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AddCourseDialog(EnvDTE._DTE dte)
		{
			m_applicationObject = dte;
			InitializeComponent();
			LocalizeLabels();
			txtCourseName.Focus();
		}

		private void AddCourse()
		{
			string strPath = txtServer.Text;
			string strCourseName = txtCourseName.Text;
			string strGuid = System.Guid.NewGuid().ToString();

			if( (strCourseName == "") || (strPath == "") || (strPath=="http://") )
			{
				//User hasn't entered the information, so display error dialog
				MessageBox.Show( AMResources.GetLocalizedString("AddCourse_ErrorBlankField"),
					this.Text,
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
				return;
			}

			FacultyTools fac = new FacultyTools(m_applicationObject);
			if (!fac.CheckVersion(strPath + "/amversion.xml"))
			{
				//Error message has already been displayed
				return;
			}

			Utilities.RegisterAssignmentManagerCourse(m_applicationObject, strCourseName, strPath, strGuid);
			fac.RegenerateUserCustomTab(false);
			strPath += "/Faculty/AddCourse.aspx?CourseID=" + strGuid + "&CourseName=" + System.Web.HttpUtility.UrlEncode(strCourseName);
			m_applicationObject.ItemOperations.Navigate(strPath, vsNavigateOptions.vsNavigateOptionsDefault);	
			this.Close();
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
			this.txtCourseName = new System.Windows.Forms.TextBox();
			this.lblCourseName = new System.Windows.Forms.Label();
			this.lblAMUrl = new System.Windows.Forms.Label();
			this.txtServer = new System.Windows.Forms.TextBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtCourseName
			// 
			this.txtCourseName.Location = new System.Drawing.Point(8, 24);
			this.txtCourseName.Name = "txtCourseName";
			this.txtCourseName.Size = new System.Drawing.Size(296, 20);
			this.txtCourseName.TabIndex = 1;
			this.txtCourseName.Text = "";
			// 
			// lblCourseName
			// 
			this.lblCourseName.Location = new System.Drawing.Point(8, 8);
			this.lblCourseName.Name = "lblCourseName";
			this.lblCourseName.Size = new System.Drawing.Size(280, 16);
			this.lblCourseName.TabIndex = 4;
			this.lblCourseName.Text = "Course name";
			// 
			// lblAMUrl
			// 
			this.lblAMUrl.Location = new System.Drawing.Point(8, 56);
			this.lblAMUrl.Name = "lblAMUrl";
			this.lblAMUrl.Size = new System.Drawing.Size(288, 16);
			this.lblAMUrl.TabIndex = 7;
			this.lblAMUrl.Text = "Assignment Manager URL (http:// or https://)";
			// 
			// txtServer
			// 
			this.txtServer.Location = new System.Drawing.Point(8, 72);
			this.txtServer.Name = "txtServer";
			this.txtServer.Size = new System.Drawing.Size(296, 20);
			this.txtServer.TabIndex = 2;
			this.txtServer.Text = "http://";
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(120, 104);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(88, 24);
			this.btnAdd.TabIndex = 3;
			this.btnAdd.Text = "&Add";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(216, 104);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(88, 24);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// AddCourseDialog
			// 
			this.AcceptButton = this.btnAdd;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(312, 133);
			this.ControlBox = false;
			this.Controls.Add(this.txtCourseName);
			this.Controls.Add(this.txtServer);
			this.Controls.Add(this.lblCourseName);
			this.Controls.Add(this.lblAMUrl);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "AddCourseDialog";
			this.ShowInTaskbar = false;
			this.Text = "Add Course";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AddCourseDialog_HelpRequested);
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
			lblCourseName.Text = AMResources.GetLocalizedString("AddCourse_LblCourseName");
			lblAMUrl.Text = AMResources.GetLocalizedString("AddCourse_LblAMUrl");
			btnAdd.Text = AMResources.GetLocalizedString("BtnAdd");
			btnCancel.Text = AMResources.GetLocalizedString("BtnCancel");
			txtServer.Text = AMResources.GetLocalizedString("HttpText");
			this.Text = AMResources.GetLocalizedString("AddCourse_DialogCaption");
		}

		private void AddCourseDialog_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
		{
			Microsoft.VisualStudio.VSHelp.Help help = (Microsoft.VisualStudio.VSHelp.Help)m_applicationObject.GetObject("Help");
			help.DisplayTopicFromF1Keyword("vs.AMAddCourseFaculty");
		}
	}
}
