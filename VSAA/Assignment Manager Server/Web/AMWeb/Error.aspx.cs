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
	/// Summary description for Error.
	/// </summary>
	public class Error : System.Web.UI.Page
	{

		protected System.Web.UI.WebControls.Label lblErrorEncountered;
		protected System.Web.UI.WebControls.Label lblErrorDetail;
		protected System.Web.UI.WebControls.HyperLink hlCoreTools;
		protected System.Web.UI.WebControls.Label lblTitle;
		protected System.Web.UI.WebControls.Label lblSubTitle;
		protected System.Web.UI.WebControls.Label lblFeedback;
		protected System.Web.UI.HtmlControls.HtmlGenericControl copyText;
		protected System.Web.UI.HtmlControls.HtmlForm loginform;

		public string Title = SharedSupport.GetLocalizedString("AM_Title");

		public Error()
		{
			Page.Init += new System.EventHandler(Page_Init);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				// standard error label
				this.lblErrorEncountered.Text = SharedSupport.GetLocalizedString("ErrorPage_ErrorEncountered");

				// localize token on querystring
				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
				string errorDetail = func.ValidateStringQueryStringParameter(this.Request, "ErrorDetail");
				if (!errorDetail.Equals(null) && !errorDetail.Equals(String.Empty))
				{	
					this.lblErrorDetail.Text = SharedSupport.GetLocalizedString(errorDetail);
				}
				else
				{
					this.lblErrorDetail.Text = String.Empty;
				}

				// link to Core Tools
				this.hlCoreTools.Text = SharedSupport.GetLocalizedString("ErrorPage_BackToStartPage");
				this.hlCoreTools.NavigateUrl = AssignmentManager.Common.constants.BACK_TO_CORETOOLS_LINK;				

				// log off ...otherwise, after returning to core tools, previous identity persists
				System.Web.Security.FormsAuthentication.SignOut();
		
			}
			catch (System.Exception ex)
			{
				this.lblErrorDetail.Text = ex.Message;
			}
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
