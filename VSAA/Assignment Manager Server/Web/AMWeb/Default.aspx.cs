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

namespace Microsoft.VisualStudio.Academic.AssignmentManager
{
	/// <summary>
	/// Summary description for CDefault.
	/// </summary>
	internal class CDefault : System.Web.UI.Page
	{
		internal CDefault()
		{
			Page.Init += new System.EventHandler(Page_Init);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Nothing on this page so redirect based on rights to view advanced nav bar
//			AssignmentManager.Security security = new AssignmentManager.Security();
//			if(security.IsAllowed(Constants.NAVBAR_VIEW_ADVANCED_ACTION))
//			{ 
//				Response.Redirect("faculty/Assignments.aspx?" + Request.QueryString.ToString()); 
//			} 
//			else
//			{
//				Response.Redirect("student/Assignments.aspx?" + Request.QueryString.ToString()); 
//			}
			

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
