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
using AssignmentManager.ClientUI;

namespace FacultyClient 
{

	internal class TodoCommentForm : System.Windows.Forms.Form 
	{
		/// <summary>
		///    Required designer variable.
		/// </summary>
		protected internal System.Windows.Forms.Button bCancel;
		protected internal System.Windows.Forms.Button bOK;
		protected internal System.Windows.Forms.Label lblDescription;
		protected internal System.Windows.Forms.TextBox tbComment;

		public TodoCommentForm() 
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			TodoComment = "";
		}

		/// <summary>
		///    Clean up any resources being used
		/// </summary>
		protected override void Dispose(bool flag)
		{
			base.Dispose(flag);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() 
		{
			this.bOK = new System.Windows.Forms.Button();
			this.lblDescription = new System.Windows.Forms.Label();
			this.tbComment = new System.Windows.Forms.TextBox();
			this.bCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// bOK
			// 
			this.bOK.AccessibleName = "bOk";
			this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bOK.Location = new System.Drawing.Point(64, 88);
			this.bOK.Name = "bOK";
			this.bOK.TabIndex = 2;
			this.bOK.Text = "OK";
			this.bOK.Click += new System.EventHandler(this.bOK_Click);
			// 
			// lblDescription
			// 
			this.lblDescription.AccessibleName = "lblDescription";
			this.lblDescription.Location = new System.Drawing.Point(8, 8);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(288, 40);
			this.lblDescription.TabIndex = 0;
			this.lblDescription.Text = "Code has been marked for extraction. Enter a comment that will appear in the stud" +
				"ent\'s Task List as a TODO item";
			// 
			// tbComment
			// 
			this.tbComment.AccessibleName = "tbComment";
			this.tbComment.Location = new System.Drawing.Point(8, 56);
			this.tbComment.Name = "tbComment";
			this.tbComment.Size = new System.Drawing.Size(288, 20);
			this.tbComment.TabIndex = 1;
			this.tbComment.Text = "";
			// 
			// bCancel
			// 
			this.bCancel.AccessibleName = "bCancel";
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(160, 88);
			this.bCancel.Name = "bCancel";
			this.bCancel.TabIndex = 3;
			this.bCancel.Text = "Cancel";
			// 
			// TodoCommentForm
			// 
			this.AcceptButton = this.bOK;
			this.AccessibleName = "TodoCommentForm";
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.bCancel;
			this.ClientSize = new System.Drawing.Size(304, 117);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bOK);
			this.Controls.Add(this.tbComment);
			this.Controls.Add(this.lblDescription);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "TodoCommentForm";
			this.Text = "TODO Comment";
			this.ResumeLayout(false);

		}

		protected void bOK_Click(object sender, System.EventArgs e) 
		{
			this.TodoComment = this.tbComment.Text;
		}

		/// <summary>
		/// This method gets called every time that the user presses a command key. The
		/// override handles changing focus to the appropriate text control.
		/// </summary>
		protected override bool ProcessCmdKey (ref Message msg, Keys keyData) 
		{
			if (keyData  == (Keys.Alt | Keys.C)) 
			{
				tbComment.Focus();
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		public string TodoComment;
	}
}
