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
	/// Summary description for Logoff.
	/// </summary>
	public class Logoff : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lblTitle;
	
		protected System.Web.UI.HtmlControls.HtmlGenericControl copyText;
		protected System.Web.UI.WebControls.HyperLink HyperLink1;
		protected System.Web.UI.WebControls.Label lblLogoffMsg;
	
		protected System.Web.UI.HtmlControls.HtmlForm loginform;

		public readonly string Title = SharedSupport.GetLocalizedString("LOGOFF_TITLE");
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here

			// load the captions
			lblTitle.Text = "";
			lblLogoffMsg.Text = SharedSupport.GetLocalizedString("LOGOFF_MESSAGE"); 
			HyperLink1.Text = SharedSupport.GetLocalizedString("LOGOFF_RETURN_TO_START_PAGE");
			HyperLink1.NavigateUrl = AssignmentManager.Common.constants.BACK_TO_CORETOOLS_LINK;

			// log the user off
			System.Web.Security.FormsAuthentication.SignOut();

		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
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
