//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//

namespace Microsoft.VisualStudio.Academic.AssignmentManager.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Microsoft.VisualStudio.Academic.AssignmentManager;

	/// <summary>
	///		Summary description for goBack.
	/// </summary>
	public class goBack : System.Web.UI.UserControl
	{

		public static readonly string GoBack_Help = SharedSupport.GetLocalizedString("GoBack_Help");
		public static readonly string GoBack_Back = SharedSupport.GetLocalizedString("GoBack_Back");
		public string GoBack_top;
		public string GoBack_left;
		public bool GoBackIncludeBack = true;
		public string GoBack_HelpUrl = "http://";
		public string GoBack_BackURL;

		/// <summary>
		public goBack()
		{
			this.Init += new System.EventHandler(Page_Init);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if((GoBack_BackURL == "" || GoBack_BackURL == null) && (Request.ServerVariables.Get("HTTP_REFERER") != "" || Request.ServerVariables.Get("HTTP_REFERER") != null))
			{
				GoBack_BackURL = Request.ServerVariables.Get("HTTP_REFERER");
			}
			
			if(GoBack_top == null)
			{
				GoBack_top = "0px";
			}
			if(GoBack_left == null)
			{
				GoBack_left = "0px";
			}
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
		}

		#region Web Form Designer generated code
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
