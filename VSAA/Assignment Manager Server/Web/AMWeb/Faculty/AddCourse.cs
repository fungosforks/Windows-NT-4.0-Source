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
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
	using System.IO;
	using Microsoft.VisualStudio.Academic.AssignmentManager;
	using System.Security;

    /// <summary>
    ///    Summary description for AddCourse.
    /// </summary>
    public class AddCourse : System.Web.UI.Page
    {
		protected System.Web.UI.WebControls.HyperLink hlWorkWithCourse;
		protected System.Web.UI.WebControls.Label lblPrompt;
		protected System.Web.UI.WebControls.HyperLink hlCopyCourse;
		protected AssignmentManager.UserControls.faculty Nav1;
		protected System.Web.UI.HtmlControls.HtmlInputText txtCourseOfferingID;
		protected System.Web.UI.HtmlControls.HtmlInputText txtCourseID;
		protected System.Web.UI.HtmlControls.HtmlInputText txtCourseGUID;

		public readonly string Title = SharedSupport.GetLocalizedString("AM_Title");
		protected System.Web.UI.WebControls.Label lblInvalidLink;
		public System.Guid courseGuid;

		public AddCourse()
		{
			Page.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			try
			{
				Nav1.Feedback.Text =  String.Empty;
				Nav1.SideTabId = AssignmentManager.Common.constants.SIDE_NAV_COURSE_MANAGEMENT;
				Nav1.TopTabId = AssignmentManager.Common.constants.TOP_NAV_COURSE_INFO;
				Nav1.SubTitle = " ";
				Nav1.Title = " ";
				Nav1.relativeURL = @"../";
				
				PermissionsID maxUserPermission;
                AssignmentManager.Common.Functions func = new AssignmentManager.Common.Functions();		
				if(!SharedSupport.SecurityIsAllowed(SecurityAction.COURSE_ADD, out maxUserPermission))
				{
					// Note that Redirect ends page execution.
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_Unauthorized");
				}
			
				
				int vsversion = func.ValidateNumericQueryStringParameter(Request, "VSVersion");
				if (vsversion < 7.1)
				{
					Response.Redirect(@"../Error.aspx?ErrorDetail=" + "AddCourse_WrongVSVersion", false);
				}

				if (!IsPostBack)
				{
					//
					// Evals true first time browser hits the page
					//
					if(Request["CourseID"] != null && Request["CourseID"] != String.Empty)
					{
						if(Request["CourseName"] != null && Request["CourseName"] != String.Empty)
						{							
							// Load by GUID
							string CourseID = Request.QueryString.Get("CourseID").ToString();
							courseGuid = new System.Guid(CourseID);
							txtCourseGUID.Value = courseGuid.ToString();

							CourseM course = CourseM.Load(courseGuid);

							if(course.IsValid)
							{
								// course already exists
								courseExists(course);
								return;
							}
							else
							{
								// check the short name for uniqueness
								string courseShortName = Request.QueryString.Get("CourseName").ToString().Trim();
								if (courseShortName == null || courseShortName == String.Empty)
								{
									Response.Redirect(@"../Error.aspx?ErrorDetail=" + "AddCourse_MissingCourseShortName", false);
									return;
								}
								else
								{
									course = CourseM.Load(courseShortName);
									if(course.IsValid)
									{
										// course short name already exists; prompt: work with? copy?
										courseExists(course);
										return;
									}
								}			

								// insert bare min. course information
								course = new CourseM();
								course.Name = courseShortName;
								course.CourseGuid = courseGuid;
								course.SendEmailRemindersFlag = false;
								course.LastUpdatedUserID = SharedSupport.GetUserIdentity();
								course.StartDate = DateTime.Now.AddMonths(1);
								course.EndDate = DateTime.Now.AddMonths(1);

								string fileDir = SharedSupport.RemoveIllegalFilePathCharacters(course.Name).Replace(" ",String.Empty);
								fileDir = SharedSupport.AddBackSlashToDirectory(Constants.DEFAULT_COURSE_OFFERINGS_ROOT_STORAGE_PATH) + SharedSupport.AddBackSlashToDirectory(fileDir);
								if(!System.IO.Directory.Exists(fileDir))
								{
									//create directory
									System.IO.Directory.CreateDirectory(fileDir);
								}

								course.RootStoragePath = SharedSupport.AddBackSlashToDirectory(Constants.DEFAULT_COURSE_OFFERINGS_ROOT_STORAGE_PATH) + SharedSupport.AddBackSlashToDirectory(SharedSupport.RemoveIllegalFilePathCharacters(course.Name).Replace(" ",String.Empty));
								course.Add();

								if(!Directory.Exists(course.RootStoragePath))
								{
									Directory.CreateDirectory(course.RootStoragePath);
								}

								UserM user = UserM.Load(SharedSupport.GetUserIdentity());
								user.AddToCourse(course.CourseID, maxUserPermission);
								// redirect for additional maintenance
								Response.Redirect("AddEditCourse.aspx?CourseID=" + course.CourseID, false);
							}
						}
						else
						{
							//Throw error, there was no ShortName on the query string
							Response.Redirect(@"../Error.aspx?ErrorDetail=" + "AddCourse_MissingCourseShortName", false);
						}
					}
					else
					{
						//Throw error, there was no CourseID on the query string
						Response.Redirect(@"../Error.aspx?ErrorDetail=" + "AddCourse_MissingCourseID", false);
					}
				}
			}
			catch(Exception ex)
			{
				Nav1.Feedback.Text =  ex.Message.ToString();
			}
		}

		private void courseExists(CourseM course)
		{
			// build the connect to URL
			string connectURL;
			string filename = course.CourseID.ToString() + ".xml";
			if(SharedSupport.UsingSsl == true)
			{
				connectURL = @"https://" + 
					SharedSupport.BaseUrl + 
					AssignmentManager.Constants.ASSIGNMENTMANAGER_COURSES_DIRECTORY + 
					Server.UrlEncode(filename);
			}			
			else
			{
				connectURL = @"http://" + 
					SharedSupport.BaseUrl + 
					AssignmentManager.Constants.ASSIGNMENTMANAGER_COURSES_DIRECTORY + 
					Server.UrlEncode(filename);
			}

			
			Nav1.Feedback.Text =  SharedSupport.GetLocalizedString("AddCourse_CourseExists", new string[]{course.Name});	
			this.lblPrompt.Text = SharedSupport.GetLocalizedString("AddCourse_WhatDoPrompt", new string[]{connectURL});
			this.lblInvalidLink.Text = SharedSupport.GetLocalizedString("AddCourse_InvalidLink");
			// retasking the CopyCourse hyperlink for Delete Course
			this.hlCopyCourse.Text = SharedSupport.GetLocalizedString("AddCourse_DeleteLink");
			this.hlWorkWithCourse.Text = SharedSupport.GetLocalizedString("AddCourse_WorkWithCourse");

			this.hlWorkWithCourse.NavigateUrl += "vs://custom/faculty/addcourse.html";
			this.hlCopyCourse.NavigateUrl += "vs://custom/faculty/deletecourse.html";

			this.lblPrompt.Visible = true;
			this.hlCopyCourse.Visible = true;
			this.hlWorkWithCourse.Visible = true;
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
			this.Load += new System.EventHandler(this.Page_Load);

		}

    }
}
