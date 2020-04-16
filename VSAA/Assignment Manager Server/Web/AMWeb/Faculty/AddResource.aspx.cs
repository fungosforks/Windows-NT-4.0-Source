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
	/// Summary description for AddResource.
	/// </summary>
	public class AddResource : System.Web.UI.Page
	{
		public readonly string AddResource_Title = SharedSupport.GetLocalizedString("AddResource_Title");
		public readonly string AddResource_OK = SharedSupport.GetLocalizedString("AddResource_OK");
		public readonly string AddResource_Cancel = SharedSupport.GetLocalizedString("AddResource_Cancel");

		protected System.Web.UI.HtmlControls.HtmlSelect Select1;
		protected System.Web.UI.WebControls.Label lblAddResource;
		protected System.Web.UI.WebControls.Label lblResourceName;
		protected System.Web.UI.WebControls.Label lblNameExample;
		protected System.Web.UI.WebControls.Label lblLinkExample;
		protected System.Web.UI.WebControls.Label lblResourceLink;
		public readonly string AddResource_NotEnoughFields = SharedSupport.GetLocalizedString("AddResource_NotEnoughFields");

		public AddResource()
		{
			Page.Init += new System.EventHandler(Page_Init);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			LocalizeLabels();
			Select1.Multiple = false;
			Select1.Items.Add(SharedSupport.GetLocalizedString("AddResource_Http"));
			Select1.Items.Add(SharedSupport.GetLocalizedString("AddResource_MailTo"));
			Select1.Items.Add(SharedSupport.GetLocalizedString("AddResource_News"));
			Select1.Items.Add(SharedSupport.GetLocalizedString("AddResource_ftp"));
			Select1.Items.Add(SharedSupport.GetLocalizedString("AddResource_Gopher"));
			Select1.Items.Add(SharedSupport.GetLocalizedString("AddResource_Telnet"));
			// Select1.Items.Add(SharedSupport.GetLocalizedString("AddResource_Other"));
			// Put user code to initialize the page here
		}

		protected void LocalizeLabels()
		{
			lblAddResource.Text = SharedSupport.GetLocalizedString("AddResource_AddResourcesText");
			lblResourceName.Text = SharedSupport.GetLocalizedString("AddResource_DisplayName");
			lblNameExample.Text = SharedSupport.GetLocalizedString("AddResource_DisplayNameExample");
			lblResourceLink.Text = SharedSupport.GetLocalizedString("AddResource_LinkInfo");
			lblLinkExample.Text = SharedSupport.GetLocalizedString("AddResource_LinkInfoExample");
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
