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

namespace FacultyClient
{
	/// <summary>
	/// Summary description for DeleteCourseDialog.
	/// </summary>
	public class DeleteCourseDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnDeleteSelected;
		private System.Windows.Forms.CheckedListBox deleteCourseList;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private EnvDTE._DTE m_applicationObject = null;
		private string courseListFile;
		private string[] courseGuids;
		private System.Windows.Forms.Label lblDescription;
		private XmlDocument xmlDoc;

		public DeleteCourseDialog(EnvDTE._DTE dte)
		{
			m_applicationObject = dte;
			InitializeComponent();
			LocalizeLabels();

			Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Constants.KeyName);
			string sAppDataPath = (string)key.GetValue(Constants.ValueName);
			courseListFile = sAppDataPath + Constants.ApplicationPath + Constants.ManagedCoursesFileName;

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

		private void populateDeleteCourseList()
		{
			deleteCourseList.CheckOnClick = true;
			deleteCourseList.Items.Clear();
			if (System.IO.File.Exists(courseListFile))
			{
				xmlDoc = new XmlDocument();
				xmlDoc.Load(courseListFile);
				XmlNodeList xmlCourses = xmlDoc.SelectNodes("/managedcourses/course");
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
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnDeleteSelected = new System.Windows.Forms.Button();
			this.deleteCourseList = new System.Windows.Forms.CheckedListBox();
			this.lblDescription = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnDeleteSelected
			// 
			this.btnDeleteSelected.AccessibleName = "btnDeleteSelected";
			this.btnDeleteSelected.Location = new System.Drawing.Point(136, 176);
			this.btnDeleteSelected.Name = "btnDeleteSelected";
			this.btnDeleteSelected.Size = new System.Drawing.Size(80, 24);
			this.btnDeleteSelected.TabIndex = 2;
			this.btnDeleteSelected.Text = "&Delete";
			this.btnDeleteSelected.Click += new System.EventHandler(this.btnDeleteSelected_Click);
			// 
			// deleteCourseList
			// 
			this.deleteCourseList.AccessibleName = "deleteCourseList";
			this.deleteCourseList.HorizontalScrollbar = true;
			this.deleteCourseList.Location = new System.Drawing.Point(8, 40);
			this.deleteCourseList.Name = "deleteCourseList";
			this.deleteCourseList.Size = new System.Drawing.Size(296, 124);
			this.deleteCourseList.TabIndex = 1;
			// 
			// lblDescription
			// 
			this.lblDescription.AccessibleName = "lblDescription";
			this.lblDescription.Location = new System.Drawing.Point(8, 8);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(296, 32);
			this.lblDescription.TabIndex = 4;
			this.lblDescription.Text = "Select the courses you want to delete and then choose Delete";
			// 
			// btnCancel
			// 
			this.btnCancel.AccessibleName = "btnCancel";
			this.btnCancel.Location = new System.Drawing.Point(224, 176);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 24);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// DeleteCourseDialog
			// 
			this.AcceptButton = this.btnDeleteSelected;
			this.AccessibleName = "DeleteCourseDialog";
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(312, 205);
			this.ControlBox = false;
			this.Controls.Add(this.btnDeleteSelected);
			this.Controls.Add(this.deleteCourseList);
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "DeleteCourseDialog";
			this.ShowInTaskbar = false;
			this.Text = "DeleteCourseDialog";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.DeleteCourseDialog_HelpRequested);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnDeleteSelected_Click(object sender, System.EventArgs e)
		{
			int nCount = deleteCourseList.Items.Count;
			for(int i=0;i<nCount; i++)
			{
				if (deleteCourseList.GetItemChecked(i))
				{
					string courseGuid = courseGuids[i];
					if (courseGuid != null && courseGuid != String.Empty)
					{
						FacultyTools fac = new FacultyTools(m_applicationObject);
						fac.UnregisterAssignmentManagerCourse(courseGuid);

						try
						{
							//Get AM Server
							XmlNode xmlCourse = xmlDoc.SelectSingleNode("/managedcourses/course/assnmgr[guid='" + courseGuid + "']");
							XmlNode xmlServer = xmlCourse.SelectSingleNode("amurl");
							string deletePath = xmlServer.InnerText;
							if (deletePath != String.Empty)
							{
								deletePath += "/Faculty/DeleteCourse.aspx?CourseID=" + courseGuid;
								m_applicationObject.ItemOperations.Navigate(deletePath, vsNavigateOptions.vsNavigateOptionsNewWindow);
							}
						}
						catch
						{
							//ignore the error.  If something fails then we can't delete from the server but still removed from local machine.
						}
					}
				}
			}
			this.Close();		
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
		
		private void LocalizeLabels()
		{
			lblDescription.Text = AMResources.GetLocalizedString("DeleteCourse_lblDescription");
			btnDeleteSelected.Text = AMResources.GetLocalizedString("DeleteCourse_btnDelete");
			btnCancel.Text = AMResources.GetLocalizedString("BtnCancel");
			this.Text = AMResources.GetLocalizedString("DeleteCourse_DialogCaption");
		}

		private void DeleteCourseDialog_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs hlpevent)
		{
			Microsoft.VisualStudio.VSHelp.Help help = (Microsoft.VisualStudio.VSHelp.Help)m_applicationObject.GetObject("Help");
			help.DisplayTopicFromF1Keyword("vs.AMDeleteCourse");
		}

	}
}
