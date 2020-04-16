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
	/// Summary description for ConfirmUserDelete.
	/// </summary>
	public class ConfirmUserDelete : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lblConfirmDelete;
		protected System.Web.UI.WebControls.Button Button1;
		protected System.Web.UI.WebControls.Button Button2;
	
		public string Yes = SharedSupport.GetLocalizedString("ConfirmRoleDelete_Yes");
		public string No = SharedSupport.GetLocalizedString("ConfirmRoleDelete_No");
		public string Title = SharedSupport.GetLocalizedString("ConfirmRoleDelete_Title");

		public ConfirmUserDelete()
		{
			Page.Init += new System.EventHandler(Page_Init);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.lblConfirmDelete.Text = SharedSupport.GetLocalizedString("ConfirmUserDelete_lblConfirmDelete");
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
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
