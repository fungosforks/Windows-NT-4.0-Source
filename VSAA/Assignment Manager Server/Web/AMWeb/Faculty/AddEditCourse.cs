//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//

namespace Microsoft.VisualStudio.Academic.AssignmentManager.Faculty
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.SessionState;
    using System.Web.UI;
	using System.IO;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
	using Microsoft.VisualStudio.Academic.AssignmentManager;
	using System.Security;

    /// <summary>
    ///    Summary description for AddEditCourse.
    /// </summary>
    public class AddEditCourse : System.Web.UI.Page
    {
		protected System.Web.UI.WebControls.Button btnUpdate;
		protected System.Web.UI.WebControls.Label lblAllowMultipleSubmits;
		protected System.Web.UI.WebControls.TextBox txtDescription;
		protected System.Web.UI.WebControls.Label lblDescription;
		protected System.Web.UI.WebControls.TextBox txtHomePageURL;
		protected System.Web.UI.WebControls.Label lblHomePageURL;
		protected System.Web.UI.WebControls.TextBox txtStudentURL;
		protected System.Web.UI.WebControls.Label lblStudentURL;
		protected System.Web.UI.WebControls.TextBox txtShortNameValue;
		protected System.Web.UI.WebControls.Label lblShortName;
		protected System.Web.UI.HtmlControls.HtmlTable tblGeneralAssignment;
		protected System.Web.UI.HtmlControls.HtmlTable tblGeneralAssignmentHeader;
		protected System.Web.UI.WebControls.HyperLink hlHelp1;
		protected System.Web.UI.WebControls.Label lblGeneralHeader;
		protected System.Web.UI.WebControls.Label lblFeedback;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divCourseResources;
		protected System.Web.UI.WebControls.DataList dlCourseResources;
		protected System.Web.UI.HtmlControls.HtmlInputHidden txtCourseResourceID;		
		protected System.Web.UI.WebControls.Label lblAddResource;
		protected System.Web.UI.WebControls.TextBox txtDelete;
		protected System.Web.UI.WebControls.TextBox txtAdd;
		protected System.Web.UI.WebControls.Label lblRequired;

		// persist querystring parameters instead of referencing Request object every time needed
		private int courseId;
		protected System.Web.UI.WebControls.TextBox txtResourceName;
		protected System.Web.UI.WebControls.TextBox txtResourceValue;
		protected AssignmentManager.UserControls.faculty Nav1;
		protected AssignmentManager.UserControls.goBack GoBack1;
		public string Text_String_Name = SharedSupport.GetLocalizedString("AddDeleteResources_Text_String_Name");
		public string Text_String_Value = SharedSupport.GetLocalizedString("AddDeleteResources_Text_String_Value");
		public string Text_String_Title = SharedSupport.GetLocalizedString("AddDeleteResources_Text_String_Title");
		protected System.Web.UI.WebControls.Label lblCourseResources;
		protected System.Web.UI.HtmlControls.HtmlTable Table1;
		protected System.Web.UI.HtmlControls.HtmlTable Table2;
		protected System.Web.UI.HtmlControls.HtmlImage Img3;
		protected System.Web.UI.HtmlControls.HtmlImage Img2;
		protected System.Web.UI.HtmlControls.HtmlInputHidden txtCourseResourseID;
		public string Title = SharedSupport.GetLocalizedString("AM_Title");
		public AddEditCourse()
		{
		    Page.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			try
			{	
				//display nav bar 
				Nav1.Feedback.Text =  String.Empty;
				Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT;
				Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_COURSE_INFO;
				Nav1.relativeURL = @"../";
				
				GoBack1.GoBack_left = "325px";
				GoBack1.GoBack_HelpUrl = SharedSupport.HelpRedirect("vstskModifyingCourseInformation");
				GoBack1.GoBackIncludeBack = false;

				Nav1.Feedback.Text =  String.Empty;

				AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();
				// grab CourseID parameter from the querystring
				courseId = func.ValidateNumericQueryStringParameter(this.Request, "CourseID");				

				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.COURSE_VIEW))
				{
					// Note that Redirect ends page execution.
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
				} 

				if (Request.QueryString.Get("Action") == "Update")
				{
					Nav1.Feedback.Text =  SharedSupport.GetLocalizedString("AddEditCourse_UpdateSuccessful"); //"This course has been updated successfully.";
				}
				CourseM course = CourseM.Load(courseId);
				if (!IsPostBack)
				{
					//
					// Evals true first time browser hits the page
					//
					LocalizeLabels();
					
					if(course.IsValid)
					{
						populateControls(course);
					}
					else
					{
						// throw error - can't use this page without CourseID int passed in
						// Nav1.Feedback.Text =  SharedSupport.GetLocalizedString("AddEditCourse_MissingCourseID");
						Response.Redirect(@"../Error.aspx?ErrorDetail=" + "AddEditCourse_MissingCourseID&" + Request.QueryString.ToString(), false);
					}
				}
				else
				{
					//Check the value of txtCourseResourceID.
					if(txtCourseResourceID.Value != "" && this.txtDelete.Text == "1")
					{
						course.DeleteResource(Convert.ToInt32(txtCourseResourceID.Value));
						refreshGrid(course.ResourceList);
					}

					//Take care of Course Resource Additions from the "pop-up" form.
					if(this.txtResourceName.Text != String.Empty && this.txtResourceValue.Text != String.Empty)
					{
						string name = txtResourceName.Text;
						string resourceValue = txtResourceValue.Text;
						course.AddResource(name, resourceValue);
						txtResourceName.Text = String.Empty;
						txtResourceValue.Text = String.Empty;
					}
				}

				//Populate div tag with resources
				refreshGrid(course.ResourceList);

			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text =  ex.Message.ToString();
			}
            
        }
		private void populateControls(CourseM course)
		{
			//Update - Load existing Course and populate fields
			txtDescription.Text = course.Description;
			txtHomePageURL.Text = course.HomepageURL;
			txtShortNameValue.Text = course.Name;
						
			// Use the CourseID to create a unique xml filename for the course.
			string filename = course.CourseID + ".xml";
						
			if(SharedSupport.UsingSsl == true)
			{
				txtStudentURL.Text = @"https://" + 
									SharedSupport.BaseUrl + 
									AssignmentManager.Constants.ASSIGNMENTMANAGER_COURSES_DIRECTORY + 
									Server.UrlEncode(filename);
			}			
			else
			{
				txtStudentURL.Text = @"http://" + 
									SharedSupport.BaseUrl + 
									AssignmentManager.Constants.ASSIGNMENTMANAGER_COURSES_DIRECTORY + 
									Server.UrlEncode(filename);
			}

			btnUpdate.Text = SharedSupport.GetLocalizedString("AddEditCourse_UpdateText");

		}
		private void refreshGrid(DataSet dsResources)
		{		
			for (int i=0;i < dsResources.Tables[0].Rows.Count;i++)
			{
				dsResources.Tables[0].Rows[i]["Name"] = Server.HtmlEncode( dsResources.Tables[0].Rows[i]["Name"].ToString() );
				dsResources.Tables[0].Rows[i]["ResourceValue"] = Server.HtmlEncode( dsResources.Tables[0].Rows[i]["ResourceValue"].ToString() );
			}
			dlCourseResources.DataSource = dsResources.Tables[0].DefaultView;
			dlCourseResources.DataBind();
			dlCourseResources.Visible = true;	
		
		}

        protected void Page_Init(object sender, EventArgs e)
        {
            InitializeComponent();
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
		private void InitializeComponent()
		{
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}

		public void btnUpdate_Click (object sender, System.EventArgs e)
		{
			try
			{
				//Check Security Permissions
				if(!SharedSupport.SecurityIsAllowed(courseId, SecurityAction.COURSE_EDIT))
				{
					throw new Exception(SharedSupport.GetLocalizedString("Global_Unauthorized"));
				}

				CourseM course = CourseM.Load(courseId);
				if(course.IsValid)
				{
					//Update - Load existing Course and populate fields
					course.Name = txtShortNameValue.Text.ToString();
					course.Description = txtDescription.Text.ToString();
					course.HomepageURL = txtHomePageURL.Text.ToString();
					course.LastUpdatedDate = DateTime.Now;
					course.LastUpdatedUserID = SharedSupport.GetUserIdentity();
							
					course.Update();
					populateControls(course);
					Response.Redirect(@"./AddEditCourse.aspx?CourseID=" + courseId + "&Action=Update");
				}					
				else
				{
					// throw error - can't use this page without CourseID int passed in
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "AddEditCourse_MissingCourseID&" + Request.QueryString.ToString(), false);
				}
			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text =  ex.Message.ToString();
			}
		}
		
		private void LocalizeLabels()
		{
			txtDescription.Text = String.Empty;
			lblDescription.Text = SharedSupport.GetLocalizedString("AddEditCourse_lblDescription"); //"Description: ";
			txtHomePageURL.Text = String.Empty;
			lblHomePageURL.Text = SharedSupport.GetLocalizedString("AddEditCourse_lblHomePageURL1"); //"Home Page URL: ";
			lblShortName.Text = SharedSupport.GetLocalizedString("AddEditCourse_lblShortName") + " " + SharedSupport.GetLocalizedString("Global_RequiredField"); //"Course Name: ";
			btnUpdate.Text = SharedSupport.GetLocalizedString("AddEditCourse_btnUpdate"); //"Insert";
			this.lblStudentURL.Text = SharedSupport.GetLocalizedString("AddEditCourse_lblStudentURL");
			this.lblAddResource.Text = SharedSupport.GetLocalizedString("AddEditCourse_lblAddResource1");
			this.lblRequired.Text = SharedSupport.GetLocalizedString("Global_RequiredFieldIndicator");
		}
    }
}
