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
using System.Xml;
using EnvDTE;
using AssignmentManager.ClientUI;

namespace StudentClient
{
	/// <summary>
	/// Summary description for AddCourseDialog.
	/// </summary>
	public class AddCourseDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.TextBox txtCourseURL;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label lblAMUrl;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.Button btnCancel;
		private EnvDTE._DTE m_applicationObject = null;
		
		public AddCourseDialog(EnvDTE._DTE dte)
		{
			m_applicationObject = dte;
			InitializeComponent();
			LocalizeLabels();
			txtCourseURL.Focus();
		}

		private void AddCourse()
		{
			if( (this.txtCourseURL.Text == String.Empty) || (this.txtCourseURL.Text == "http://") )
			{
				//User hasn't entered the information, so display error dialog
				MessageBox.Show( AMResources.GetLocalizedString("AMStudentAddCourse_ErrorBlankField"),
					this.Text,
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
				return;
			}

			ClientTools clientTools = new ClientTools(m_applicationObject);
			string courseGuid = Guid.NewGuid().ToString();
			string url = txtCourseURL.Text;
			
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

            if (versionURL == "")
            {
                string message = AssignmentManager.ClientUI.AMResources.GetLocalizedString("AddCourse_ErrorWrongServerName");
                string caption = AssignmentManager.ClientUI.AMResources.GetLocalizedString("AddCourse_ErrorWrongServerNameCaption");
                message = message.Replace("%1", url);
                System.Windows.Forms.MessageBox.Show(message,caption,System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Warning);
                return;
            }
            else if (!clientTools.CheckVersion(versionURL))
            {
                return;
            }

			XmlDocument xmlCourseDoc = clientTools.GetCourseFile(url);
			if( xmlCourseDoc == null)
			{
				//Not a valid course file
				string text = AMResources.GetLocalizedString("AddCourseFail");
				MessageBox.Show(text, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				XmlNode node = xmlCourseDoc.SelectSingleNode("/course/assnmgr");
				string strCourseName = xmlCourseDoc.SelectSingleNode("/course/name").InnerText;
				string strUrl = node.SelectSingleNode("amurl").InnerText;
				string strGUID = node.SelectSingleNode("guid").InnerText;
				clientTools.RegisterAssignmentManagerCourse(strCourseName, strUrl, strGUID, url);

				strUrl += "/Student/AddCourse.aspx?CourseID=" + strGUID;
				try
				{
					m_applicationObject.ItemOperations.Navigate(strUrl, vsNavigateOptions.vsNavigateOptionsNewWindow);	
				}
				catch
				{
				}
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
			this.lblAMUrl = new System.Windows.Forms.Label();
			this.btnAdd = new System.Windows.Forms.Button();
			this.txtCourseURL = new System.Windows.Forms.TextBox();
			this.lblDescription = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblAMUrl
			// 
			this.lblAMUrl.Location = new System.Drawing.Point(8, 56);
			this.lblAMUrl.Name = "lblAMUrl";
			this.lblAMUrl.Size = new System.Drawing.Size(296, 16);
			this.lblAMUrl.TabIndex = 7;
			this.lblAMUrl.Text = "URL:";
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(144, 104);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(80, 24);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "&Add";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// txtCourseURL
			// 
			this.txtCourseURL.AcceptsReturn = true;
			this.txtCourseURL.Location = new System.Drawing.Point(8, 72);
			this.txtCourseURL.Name = "txtCourseURL";
			this.txtCourseURL.Size = new System.Drawing.Size(304, 20);
			this.txtCourseURL.TabIndex = 1;
			this.txtCourseURL.Text = "http://";
			// 
			// lblDescription
			// 
			this.lblDescription.Location = new System.Drawing.Point(8, 8);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(296, 40);
			this.lblDescription.TabIndex = 4;
			this.lblDescription.Text = "To add a course enter the URL provided by your instructor and then click Add.";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(232, 104);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 24);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// AddCourseDialog
			// 
			this.AcceptButton = this.btnAdd;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(320, 133);
			this.ControlBox = false;
			this.Controls.Add(this.lblAMUrl);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.txtCourseURL);
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "AddCourseDialog";
			this.Text = "Add Course";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.AddCourseDialog_HelpRequested);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			AddCourse();
		}

		private void LocalizeLabels()
		{
			lblDescription.Text = AMResources.GetLocalizedString("AMStudentAddCourse_lblDescription");
			lblAMUrl.Text = AMResources.GetLocalizedString("AMStudentAddCourse_lblAMUrl");
			btnAdd.Text = AMResources.GetLocalizedString("BtnAdd");
			btnCancel.Text = AMResources.GetLocalizedString("BtnCancel");
			txtCourseURL.Text = AMResources.GetLocalizedString("HttpText");
			this.Text = AMResources.GetLocalizedString("AMStudentAddCourse_DialogCaption");
		}

		private void AddCourseDialog_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
		{
			Microsoft.VisualStudio.VSHelp.Help help = (Microsoft.VisualStudio.VSHelp.Help)m_applicationObject.GetObject("Help");
			help.DisplayTopicFromF1Keyword("vs.AMAddCourseStudent");
		}
	}
}
