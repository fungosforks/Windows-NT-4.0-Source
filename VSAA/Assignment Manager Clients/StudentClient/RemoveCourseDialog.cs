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
	/// Summary description for RemoveCourseDialog.
	/// </summary>
	public class RemoveCourseDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.CheckedListBox deleteCourseList;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private EnvDTE._DTE m_applicationObject = null;
		private System.Windows.Forms.Label lblDescription;
		private string[] courseGuids;
		public RemoveCourseDialog(EnvDTE._DTE dte)
		{
			m_applicationObject = dte;
			InitializeComponent();
			LocalizeLabels();
			populateDeleteCourseList();
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
			this.btnRemove = new System.Windows.Forms.Button();
			this.deleteCourseList = new System.Windows.Forms.CheckedListBox();
			this.lblDescription = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnRemove
			// 
			this.btnRemove.AccessibleName = "btnRemove";
			this.btnRemove.Location = new System.Drawing.Point(144, 144);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(80, 24);
			this.btnRemove.TabIndex = 2;
			this.btnRemove.Text = "&Remove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// deleteCourseList
			// 
			this.deleteCourseList.AccessibleName = "deleteCourseList";
			this.deleteCourseList.HorizontalScrollbar = true;
			this.deleteCourseList.Location = new System.Drawing.Point(8, 40);
			this.deleteCourseList.Name = "deleteCourseList";
			this.deleteCourseList.Size = new System.Drawing.Size(304, 94);
			this.deleteCourseList.TabIndex = 1;
			// 
			// lblDescription
			// 
			this.lblDescription.AccessibleName = "lblDescription";
			this.lblDescription.Location = new System.Drawing.Point(8, 8);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(304, 32);
			this.lblDescription.TabIndex = 5;
			this.lblDescription.Text = "Select the courses you want to delete and then choose Remove.";
			// 
			// btnCancel
			// 
			this.btnCancel.AccessibleName = "btnCancel";
			this.btnCancel.Location = new System.Drawing.Point(232, 144);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 24);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// RemoveCourseDialog
			// 
			this.AcceptButton = this.btnRemove;
			this.AccessibleName = "RemoveCourseDialog";
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(320, 173);
			this.ControlBox = false;
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.deleteCourseList);
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "RemoveCourseDialog";
			this.Text = "Remove Course";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.RemoveCourseDialog_HelpRequested);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
				populateDeleteCourseList();
				this.Close();
		}

		private void populateDeleteCourseList()
		{
			deleteCourseList.CheckOnClick = true;
			deleteCourseList.Items.Clear();
			ClientTools clientTools = new ClientTools(m_applicationObject);
			XmlDocument xmlDoc = clientTools.LocalCoursesFile;
			XmlNodeList xmlCourses = xmlDoc.SelectNodes("/studentcourses/course");
			courseGuids = new string[xmlCourses.Count];
			for(int i=0;i<xmlCourses.Count;i++)
			{
				XmlNode parentNode = xmlCourses.Item(i);
				XmlNode node = parentNode.SelectSingleNode("name");
				string courseName = node.InnerText;
				deleteCourseList.Items.Add(courseName, CheckState.Unchecked);

				// save Guid for delete
				XmlNode guid = parentNode.SelectSingleNode("assnmgr/guid");
				if(guid != null)
				{
					courseGuids[i] = guid.InnerText;
				}
			}
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			//Confirm
			string text = AMResources.GetLocalizedString("ConfirmRemoveCourse");
			DialogResult result = MessageBox.Show(text, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (result == DialogResult.Yes)
			{
				int nCount = deleteCourseList.Items.Count;
				ClientTools clientTools = new ClientTools(m_applicationObject);
				for(int i=0;i<nCount; i++)
				{
					if (deleteCourseList.GetSelected(i))
					{
						string courseGuid = courseGuids[i];
						if (courseGuid != null && courseGuid != String.Empty)
						{
							clientTools.UnregisterAssignmentManagerCourse(courseGuid);
						}
					}
				}
				this.Close();
			}		
		}

		private void LocalizeLabels()
		{
			lblDescription.Text = AMResources.GetLocalizedString("AMStudentRemoveCourse_lblDescription");
			btnRemove.Text = AMResources.GetLocalizedString("AMStudentRemoveCourse_btnRemove");
			btnCancel.Text = AMResources.GetLocalizedString("BtnCancel");
			this.Text = AMResources.GetLocalizedString("AMStudentRemoveCourse_DialogCaption");
			
		}

		private void RemoveCourseDialog_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
		{			
			Microsoft.VisualStudio.VSHelp.Help help = (Microsoft.VisualStudio.VSHelp.Help)m_applicationObject.GetObject("Help");
			help.DisplayTopicFromF1Keyword("vs.AMRemoveCourse");
		}

	}
}
