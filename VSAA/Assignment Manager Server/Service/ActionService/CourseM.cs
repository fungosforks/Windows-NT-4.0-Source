//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//


using System;
using System.IO;

namespace Microsoft.VisualStudio.Academic.AssignmentManager.ActionService
{
	/// <summary>
	/// Summary description for CourseM.
	/// </summary>
	public class CourseM
	{
		private int _courseID = 0;
		private string _description = null;
		private System.Guid _courseGUID;
		private string _courseName = null;
		private System.DateTime _lastUpdatedDate;
		private int _lastUpdatedUserID = 0;
		private string _homepageURL = null;
		private bool _sendEmailRemindersFlag = false;
		private System.DateTime _startDate;
		private System.DateTime _endDate;
		private string _rootStoragePath = null;


		private enum CourseStoredProcType
		{
			Add = 0,
			Update = 1
		}

		public CourseM()
		{
			_courseGUID = System.Guid.NewGuid();
			_endDate = _startDate = System.DateTime.Now;
		}

		public static CourseM Load(int courseID)
		{
			DatabaseCall dbc = new DatabaseCall("Courses_LoadCourse", DBCallType.Select);
			dbc.AddParameter("@CourseID", courseID);
			return LoadCourseFromDatabase(dbc);
		}

		public static CourseM Load(System.Guid courseGUID)
		{
			DatabaseCall dbc = new DatabaseCall("Courses_LoadCourseByGUID", DBCallType.Select);
			dbc.AddParameter("@CourseGUID", courseGUID);
			return LoadCourseFromDatabase(dbc);
		}

		public static CourseM Load(string courseName)
		{	
			DatabaseCall dbc = new DatabaseCall("Courses_LoadCourseByName", DBCallType.Select);
			dbc.AddParameter("@ShortName", courseName);
			return LoadCourseFromDatabase(dbc);
		}

		private static CourseM LoadCourseFromDatabase(DatabaseCall dbc)
		{
			CourseM newCourse = new CourseM();
			System.Data.DataSet ds = new System.Data.DataSet();
			dbc.Fill(ds);
						
			if ((ds.Tables.Count <= 0) || (ds.Tables[0].Rows.Count <= 0) )
			{
				return newCourse;
			}
			
			newCourse._courseID = Convert.ToInt32( ds.Tables[0].Rows[0]["CourseID"] );
			newCourse._courseGUID = new System.Guid( ds.Tables[0].Rows[0]["CourseGUID"].ToString() );
			newCourse.Name = ds.Tables[0].Rows[0]["ShortName"].ToString();
			newCourse.Description = ds.Tables[0].Rows[0]["Description"].ToString();
			newCourse.LastUpdatedDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["LastUpdatedDate"]);
			newCourse.LastUpdatedUserID = Convert.ToInt32( ds.Tables[0].Rows[0]["LastUpdatedUserID"] );
			newCourse.HomepageURL = ds.Tables[0].Rows[0]["HomepageURL"].ToString();
			newCourse.SendEmailRemindersFlag = Convert.ToBoolean( ds.Tables[0].Rows[0]["SendEmailRemindersFlag"] );
			newCourse.StartDate = Convert.ToDateTime( ds.Tables[0].Rows[0]["StartDate"] );
			newCourse.EndDate = Convert.ToDateTime( ds.Tables[0].Rows[0]["EndDate"] );
			newCourse.RootStoragePath = ds.Tables[0].Rows[0]["RootStoragePath"].ToString();

			return newCourse;
		}

		public int Update()
		{
			return saveCourseToDatabase(CourseStoredProcType.Update);
		}

		public int Add()
		{
			return saveCourseToDatabase(CourseStoredProcType.Add);
		}

		private int saveCourseToDatabase(CourseStoredProcType sprocType)
		{

			DatabaseCall dbc;
			int retID=0;

			if (sprocType == CourseStoredProcType.Add)
			{
				dbc = new DatabaseCall("Courses_AddNewCourse", DBCallType.Execute);
				dbc.AddOutputParameter("@CourseID");
			}
			else if (sprocType == CourseStoredProcType.Update)
			{
				dbc = new DatabaseCall("Courses_UpdateExistingCourse", DBCallType.Execute);
				dbc.AddParameter("@CourseID", this._courseID);
			}
			else
			{
				throw new Exception("Unknown Stored Procedure Type");
			}

			dbc.AddParameter("@CourseGUID", _courseGUID);
			dbc.AddParameter("@ShortName", _courseName);
			dbc.AddNTextParameter("@Description", _description);
			dbc.AddParameter("@LastUpdatedDate", System.DateTime.Now);
			dbc.AddParameter("@LastUpdatedUserID", _lastUpdatedUserID);
			dbc.AddParameter("@HomepageURL", _homepageURL);
			dbc.AddParameter("@MultipleSubmitsFlag", false);
			dbc.AddParameter("@SendEmailRemindersFlag", _sendEmailRemindersFlag);
			dbc.AddParameter("@StartDate", _startDate);
			dbc.AddParameter("@EndDate", _endDate);
			dbc.AddParameter("@RootStoragePath", _rootStoragePath);
			
			dbc.Execute();

			if(sprocType == CourseStoredProcType.Add)
			{
				try
				{

					retID = Convert.ToInt32( dbc.GetOutputParam("@CourseID") );
					this._courseID = retID;
				}
				catch
				{
				}
			}
			//Write course data to file
			saveCourseXML();
			return retID;
		}

		private object validDBValue(string param)
		{
			if( (param == null) || (param.Equals(String.Empty)) )
			{
				return DBNull.Value;
			}
			return param;
		}

		public int CourseID 
		{
			get{ return _courseID; }
			set{ _courseID = value; }
		}

		public string Description 
		{
			get{ return _description; }
			set{ _description = value; }
		}

		public string Name
		{
			get{ return _courseName; }
			set{ _courseName = value; }
		}

		public System.DateTime LastUpdatedDate 
		{
			get{ return _lastUpdatedDate; }
			set{ _lastUpdatedDate = value; }
		}
		
		public int LastUpdatedUserID 
		{
			get{ return _lastUpdatedUserID; }
			set{ _lastUpdatedUserID = value; }
		}

		public string HomepageURL 
		{
			get{ return _homepageURL; }
			set{ _homepageURL = value; }
		}

		public bool SendEmailRemindersFlag 
		{
			get{ return _sendEmailRemindersFlag; }
			set{ _sendEmailRemindersFlag = value; }
		}

		public System.DateTime StartDate 
		{
			get{ return _startDate; }
			set{ _startDate = value; }
		}
		
		public System.DateTime EndDate 
		{
			get{ return _endDate; }
			set{ _endDate = value; }
		}
		
		public string RootStoragePath 
		{
			get{ return _rootStoragePath; }
			set{ _rootStoragePath = value; }
		}

		public System.Guid CourseGuid
		{
			set{ _courseGUID = value; }
		}
		public bool IsValid
		{
			get{ return this._courseID != 0; }
		}

		private void saveCourseXML()
		{
			string dir = System.Web.HttpContext.Current.Server.MapPath("..\\") + "\\Courses\\";

			try
			{
				//Check to see if folder and file exist
				if(Directory.Exists(dir))
				{
					// Use the CourseID to create a unique xml filename for the course.
					string filename = this.CourseID + ".xml";
					
					//Create CourseID.xml file
					System.Xml.XmlTextWriter xmlwriter = new System.Xml.XmlTextWriter(dir + filename, null);
					xmlwriter.Formatting = System.Xml.Formatting.Indented;
					xmlwriter.WriteStartDocument(false);
					// xmlwriter.WriteDocType("Course", null, null, null);
					xmlwriter.WriteStartElement("course");
					xmlwriter.WriteStartElement("name");
					if(this.Name != null && this.Name != "")
					{ 
						xmlwriter.WriteCData(this.Name);
					}
					xmlwriter.WriteEndElement();
					xmlwriter.WriteStartElement("assnmgr");
					xmlwriter.WriteStartElement("amurl");
					
					if(SharedSupport.UsingSsl == true)
					{
						xmlwriter.WriteCData(@"https://" + SharedSupport.BaseUrl);
					}			
					else
					{
						xmlwriter.WriteCData(@"http://" + SharedSupport.BaseUrl);
					}
					
					xmlwriter.WriteEndElement();
					xmlwriter.WriteStartElement("guid");
					xmlwriter.WriteCData(this._courseGUID.ToString());
					xmlwriter.WriteEndElement();
					xmlwriter.WriteEndElement();
					xmlwriter.WriteEndElement();

					//write the xml to the file and close
					xmlwriter.Flush();
					xmlwriter.Close();
				}
				else
				{
					throw new Exception(SharedSupport.GetLocalizedString("Course_DirectoryDoesNotExist")); //"Directory could not be found. " + dir);	
				}
			}
			catch(System.Exception ex)
			{
				SharedSupport.HandleError(ex);
			}
		}

		public void AddResource(string name, string resourceValue)
		{
			DatabaseCall dbc = new DatabaseCall("Courses_AddResource", DBCallType.Execute);
			dbc.AddParameter("@CourseID", _courseID);
			dbc.AddParameter("@Name", name);
			dbc.AddParameter("@ResourceValue", resourceValue);
			dbc.Execute();
		}

		public void DeleteResource(int resourceID)
		{
			DatabaseCall dbc = new DatabaseCall("Courses_DeleteResource", DBCallType.Execute);
			dbc.AddParameter("@CourseResourceID", resourceID);
			dbc.Execute();
		}

		public System.Data.DataSet ResourceList
		{
			get
			{
				DatabaseCall dbc = new DatabaseCall("Courses_GetResourceList", DBCallType.Select);
				dbc.AddParameter("@CourseID", _courseID);
				System.Data.DataSet ds = new System.Data.DataSet();
				dbc.Fill(ds);
				return ds;
			}
		}
	}
}
