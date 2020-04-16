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
using System.Data;

namespace Microsoft.VisualStudio.Academic.AssignmentManager
{
	/// <summary>
	/// Summary description for StudentAssignmentM.
	/// </summary>
	public class StudentAssignmentM
	{
		private int _userID;
		private int _assignmentID;

		private int _userAssignmentID; 
		private DateTime _lastSubmitDate;
		private DateTime _lastUpdatedDate;
		private string _overallGrade;
		private string _gradeComments;
		private int _autoCompileStatus;
		private int _autoGradeStatus;
		private string _userLastName;
		private string _userFirstName;
		private string _userUniversityIdentifier;
		private string _assignmentName;		

		private string _buildDetails;
		private string _checkDetails;
		private string _buildResultCode;
		private string _checkResultCode;


		public StudentAssignmentM()
		{
			this._userAssignmentID = 0;
		}

		private enum StoredProcType
		{
			New = 0,
			Update = 1
		}

		public static StudentAssignmentM Load(int userID, int assignmentID)
		{
			System.Data.DataSet ds = new System.Data.DataSet();

			DatabaseCall dbc = new DatabaseCall("StudentAssignments_LoadAssignment", DBCallType.Select);
			dbc.AddParameter("@UserID", userID);
			dbc.AddParameter("@AssignmentID", assignmentID);
			
			dbc.Fill(ds);
						
			if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
			{
				return null;
			}
			return PopulateNewAssignment(ds);
		}

		public static StudentAssignmentM Load(int userAssignmentID)
		{
			System.Data.DataSet ds = new System.Data.DataSet();

			DatabaseCall dbc = new DatabaseCall("StudentAssignments_LoadAssignmentByUserAssignmentID", DBCallType.Select);
			dbc.AddParameter("@UserAssignmentID", userAssignmentID);
			
			dbc.Fill(ds);
						
			if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
			{
				return null;
			}
			return PopulateNewAssignment(ds);
		}

		private static StudentAssignmentM PopulateNewAssignment(System.Data.DataSet ds)
		{
			StudentAssignmentM retVal = new StudentAssignmentM();
			// Populate the return value.	
			retVal._userID = Convert.ToInt32( ds.Tables[0].Rows[0]["UserID"] );
			retVal._assignmentID = Convert.ToInt32( ds.Tables[0].Rows[0]["AssignmentID"] );
			retVal._userAssignmentID = Convert.ToInt32( ds.Tables[0].Rows[0]["UserAssignmentID"] );
			retVal._lastSubmitDate = Convert.ToDateTime( ds.Tables[0].Rows[0]["LastSubmitDate"] );
			retVal._lastUpdatedDate = Convert.ToDateTime( ds.Tables[0].Rows[0]["LastUpdatedDate"] );
			retVal._overallGrade = ds.Tables[0].Rows[0]["OverallGrade"].ToString();
			retVal._gradeComments = ds.Tables[0].Rows[0]["GradeComments"].ToString();
			retVal._autoCompileStatus = Convert.ToInt32( ds.Tables[0].Rows[0]["AutoCompileStatus"] );
			retVal._autoGradeStatus = Convert.ToInt32( ds.Tables[0].Rows[0]["AutoGradeStatus"] );
			retVal._userLastName = ds.Tables[0].Rows[0]["LastName"].ToString();
			retVal._userFirstName = ds.Tables[0].Rows[0]["FirstName"].ToString();
			retVal._assignmentName = ds.Tables[0].Rows[0]["ShortName"].ToString();
			retVal._userUniversityIdentifier = ds.Tables[0].Rows[0]["UniversityIdentifier"].ToString();

			retVal._buildDetails = ds.Tables[0].Rows[0]["BuildDetails"].ToString();
			retVal._checkDetails = ds.Tables[0].Rows[0]["CheckDetails"].ToString();
			retVal._buildResultCode = ds.Tables[0].Rows[0]["BuildResultCode"].ToString();
			retVal._checkResultCode = ds.Tables[0].Rows[0]["CheckResultCode"].ToString();
			return retVal;
		}

		public void Submit(int assignmentID, int courseID, string xmlFileList, string pathGUID)
		{
			try
			{ 
				bool bBuildAssignment = false;	// indicates if build indicated by faculty for this assignment
				bool bCheckAssignment = false;	// indicates if check indicated by faculty for this assignment

				//For each file uploaded - move to secure directory
				this._assignmentID = assignmentID;
				this._userID = SharedSupport.GetUserIdentity();		
	
				//Check to see if the user has already submitted an assignment and if the assignment allows for multiple submissions.
				//If not allowed, and the user has already submitted an assignment for this assignment then throw error.
				AssignmentM assign = AssignmentM.Load(assignmentID);
				if(assign.IsValid)
				{
					// build / check indicated for this assignment? 
					
					bBuildAssignment = assign.AutoCompileFlag;
					bCheckAssignment = assign.AutoGradeFlag;

					if(!assign.MultipleSubmitsFlag)
					{
						if( this.HasSubmitted )
						{
							throw new ApplicationException(SharedSupport.GetLocalizedString("StudentAssignment_NoMultipleSubmits"));
						}	
					}
				}
				else
				{
					throw new ApplicationException(SharedSupport.GetLocalizedString("StudentAssignment_NoCourseOfferingAssignment_Error"));
				}

				// delete any previous submissions
				this.ClearSubmissions();

				if(bBuildAssignment)
				{
					this._autoCompileStatus = Constants.AUTOCOMPILE_PENDING_STATUS;
				}
				else
				{
					this._autoCompileStatus  = Constants.AUTOCOMPILE_NOTAPPLICABLE_STATUS;
				}
				if(bCheckAssignment)
				{
					this._autoGradeStatus = Constants.AUTOGRADE_PENDING_STATUS;
				}
				else
				{
					this._autoGradeStatus = Constants.AUTOGRADE_NOTAPPLICABLE_STATUS;
				}
				this._lastSubmitDate = DateTime.Now;
				this._lastUpdatedDate = DateTime.Now;

				this.SaveToDatabase(StoredProcType.New);

				this.saveFileList(xmlFileList, pathGUID);

				// queue action requests
				if(bBuildAssignment || bCheckAssignment) 
				{
					try 
					{
						SendActionToQueue(this._userAssignmentID, bBuildAssignment, bCheckAssignment);
					}
					catch (Exception ex)
					{
						// this is the student submitting, so log it and continue
						SharedSupport.LogMessage(ex.Message, this.ToString(), System.Diagnostics.EventLogEntryType.Warning);
					}
				}
			}
			catch(Exception ex)
			{
				SharedSupport.HandleError(ex);
			}
		}

		private void saveFileList(string xmlFileList, string guid)
		{
			System.Data.DataSet ds = new System.Data.DataSet();
			System.IO.StringReader reader = new System.IO.StringReader(xmlFileList);
			System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(reader);
			ds.ReadXml(xmlReader);

			AssignmentM assign = AssignmentM.Load(_assignmentID);
			string targetdir = SharedSupport.AddBackSlashToDirectory( assign.StorageDirectory + _userID );
			string uploadDir = SharedSupport.AddBackSlashToDirectory(System.Web.HttpContext.Current.Request.MapPath(String.Empty,Constants.ASSIGNMENTMANAGER_UPLOAD_DIRECTORY,true)) + 
								SharedSupport.AddBackSlashToDirectory(guid);
			try
			{
				if(Directory.Exists(targetdir))
				{
					// If the directory exists, remove it so we are not left with extra files around.
					Directory.Delete(targetdir, true);
				}
				Directory.CreateDirectory(targetdir);
			}
			catch
			{
			}

			//get max project size from settings table (in megabytes)
			Int64 maxProjectSize = new Int64();
			maxProjectSize = Convert.ToInt32(SharedSupport.GetSetting(Constants.MAX_PROJECT_SETTING)) * Constants.BytesInMegaByte;
			//create variable to track project size
			Int64 currentProjectSize = new Int64();
			currentProjectSize = 0;

			//Cycle through all uploaded files and save a record for each in the user assignment files table
			for(int i=0;i<ds.Tables[0].Rows.Count;i++)
			{
				string filename = ds.Tables[0].Rows[i]["Filename"].ToString();
				//make sure the file name is not blank
				if(filename != null && filename != String.Empty)
				{
                    filename = filename.Replace("/", @"\");
					if (filename.StartsWith(@"\"))
					{
						filename = filename.Remove(0,1);
					}
					this.AddFile(filename);
				}
				else
				{
					//Throw an error because the file name for the given record is blank
					throw new ApplicationException(SharedSupport.GetLocalizedString("StudentAssignment_BlankFileName_Error"));
				}

				string uploadFilePath = uploadDir + filename;
				string targetFilePath = targetdir + filename;
                string fullTargetDir = targetFilePath.Substring(0, targetFilePath.LastIndexOf(@"\"));
                if (!Directory.Exists(fullTargetDir))
                {
                    Directory.CreateDirectory(fullTargetDir);
                }
				if(!File.Exists(uploadFilePath))
				{ 
					throw new ApplicationException(SharedSupport.GetLocalizedString("SharedSupport_InvalidFileLocation_Error")); 
				}
				else
				{
					//Get the size of the file and add it to other's to see if project exceeds
					//    the maximum size limit held in the settings table
					currentProjectSize += new FileInfo(uploadFilePath).Length;
						
					if(currentProjectSize > maxProjectSize)
					{
						//delete all files
						Directory.Delete(uploadDir, true);
						Directory.Delete(targetdir, true);
						throw new ApplicationException(SharedSupport.GetLocalizedString("StudentAssignment_ProjectTooLarge") + maxProjectSize.ToString() + SharedSupport.GetLocalizedString("StudentAssignment_Megabytes"));
					}
				}
				File.Copy(uploadFilePath, targetFilePath, true);				
			}
		}

		private void AddFile(string filename)
		{
			DatabaseCall dbc = new DatabaseCall("StudentAssignments_AddFile", DBCallType.Execute);
			dbc.AddParameter("@UserAssignmentID", _userAssignmentID);
			dbc.AddParameter("@FileName", filename);
			dbc.Execute();
		}

		public void Update()
		{
			this.SaveToDatabase(StoredProcType.Update);
		}
		private void SaveToDatabase( StoredProcType procType )
		{

			DatabaseCall dbc;

			switch( procType )
			{
				case StoredProcType.New:
				
					dbc = new DatabaseCall("StudentAssignments_AddNewAssignment", DBCallType.Execute);
					dbc.AddOutputParameter("@UserAssignmentID");
					break;
				case StoredProcType.Update:
					dbc = new DatabaseCall("StudentAssignments_UpdateExistingAssignment", DBCallType.Execute);
					break;
				default:
					throw new Exception("Invalid Stored Procedure");
			}

			dbc.AddParameter("@UserID", this._userID);
			dbc.AddParameter("@AssignmentID", this._assignmentID);
			dbc.AddParameter("@LastSubmitDate", this._lastSubmitDate);
			dbc.AddParameter("@LastUpdatedDate", this._lastUpdatedDate);
			dbc.AddParameter("@OverallGrade",  this._overallGrade );
			dbc.AddNTextParameter("@GradeComments", this._gradeComments);
			dbc.AddParameter("@AutoCompileStatus", this._autoCompileStatus);
			dbc.AddParameter("@AutoGradeStatus", this._autoGradeStatus);
			dbc.AddNTextParameter("@BuildDetails", this._buildDetails);
			dbc.AddNTextParameter("@CheckDetails", this._checkDetails);
			dbc.AddParameter("@BuildResultCode", this._buildResultCode);
			dbc.AddParameter("@CheckResultCode", this._checkResultCode);
			
			dbc.Execute();

			if (procType == StoredProcType.New)
			{
				this._userAssignmentID = Convert.ToInt32(dbc.GetOutputParam("@UserAssignmentID"));
			}
		}

		public void ClearSubmissions()
		{
			DatabaseCall dbc = new DatabaseCall("StudentAssignments_ClearSubmissions", DBCallType.Execute);
			dbc.AddParameter("@UserID", _userID);
			dbc.AddParameter("@AssignmentID", _assignmentID);
			
			dbc.Execute();
		}

		public int UserAssignmentID
		{
			get { return _userAssignmentID; }
			set { _userAssignmentID = value; }
		}
		
		public DateTime LastSubmitDate
		{
			get { return _lastSubmitDate; }
			set { _lastSubmitDate = value; }
		}
		
		public DateTime LastUpdatedDate
		{
			get { return _lastUpdatedDate; }
			set { _lastUpdatedDate = value; }
		}
		
		public string OverallGrade
		{
			get { return _overallGrade; }
			set { _overallGrade = value; }
		}
		
		public string GradeComments
		{
			get { return _gradeComments; }
			set { _gradeComments = value; }
		}
		
		public int AutoCompileStatus
		{
			get { return _autoCompileStatus; }
			set { _autoCompileStatus = value; }
		}
		
		public int AutoGradeStatus
		{
			get { return _autoGradeStatus; }
			set { _autoGradeStatus = value; }
		}
		
		public string UserLastName
		{
			get { return _userLastName; }
			set { _userLastName = value; }
		}
		
		public string UserFirstName
		{
			get { return _userFirstName; }
			set { _userFirstName = value; }
		}
		
		public string AssignmentName
		{
			get { return _assignmentName; }
			set { _assignmentName = value; }
		}

		public string UserUniversityIdentifier
		{
			get { return _userUniversityIdentifier; }
			set { _userUniversityIdentifier = value; }
		}

		public string BuildDetails
		{
			get { return _buildDetails; }
			set { _buildDetails = value; }
		}

		public string CheckDetails
		{
			get { return _checkDetails; }
			set { _checkDetails = value; }
		}

		public string BuildResultCode
		{
			get { return _buildResultCode; }
			set { _buildResultCode = value; }
		}

		public string CheckResultCode
		{
			get { return _checkResultCode; }
			set { _checkResultCode = value; }
		}


		public bool HasSubmitted
		{
			get
			{
				try
				{

					return (_userAssignmentID > 0);
				}
				catch
				{
					return false;
				}
			}
	
		}
	
		private object ConvertToDBValue(string str)
		{
			if ( (str != null) && (str != "") )
			{
				return str;
			}
			return DBNull.Value;
		}

		internal void SendActionToQueue(bool build, bool check)
		{
			try
			{
				// at least one action must be true
				if (!build && !check)
				{
					throw new ApplicationException(SharedSupport.GetLocalizedString("StudentAssignment_Must_Choose_Action"));
				}

				// validate userAssignmentId
				if(_userAssignmentID <= 0)
				{
					throw new ApplicationException(SharedSupport.GetLocalizedString("StudentAssignment_InvalidUserAssignmentId"));
				}

				// generate the xml
				System.IO.MemoryStream ms = new System.IO.MemoryStream();
				System.Xml.XmlTextWriter xmlwriter = new System.Xml.XmlTextWriter(ms, System.Text.Encoding.ASCII);
				xmlwriter.Formatting = System.Xml.Formatting.Indented;
				xmlwriter.WriteStartDocument(false);
				xmlwriter.WriteStartElement("serverActions");

				// build requested? 
				if(build)
				{
					// update auto build status - set to pending
					this._autoCompileStatus = Constants.AUTOCOMPILE_PENDING_STATUS;
				}

				// include serverAction element for auto build
				xmlwriter.WriteStartElement("serverAction");
				xmlwriter.WriteAttributeString("name", "AutoBuild");
				xmlwriter.WriteElementString("userAssignmentID", this.UserAssignmentID.ToString());
				xmlwriter.WriteEndElement();

				// check requested? 
				if(check)
				{
					// update auto check status - set to pending
					this._autoGradeStatus = Constants.AUTOGRADE_PENDING_STATUS;
					this._autoCompileStatus = Constants.AUTOCOMPILE_PENDING_STATUS;

					// include serverAction element for auto build
					xmlwriter.WriteStartElement("serverAction");
					xmlwriter.WriteAttributeString("name", "AutoCheck");
					xmlwriter.WriteElementString("userAssignmentID", this.UserAssignmentID.ToString());
					xmlwriter.WriteEndElement();
				}

				xmlwriter.WriteEndElement();
				xmlwriter.Flush();
				
				//read all of the stream and convert to a string
				string msg = System.Text.Encoding.ASCII.GetString(ms.GetBuffer());
				
				// close
				xmlwriter.Close();
				ms.Close();

				try
				{
					SharedSupport.SendMessageToQueue(Constants.ACTION_QUEUE_PATH, Constants.ACTION_QUEUE_NAME, Constants.AM_SUBMIT_ACTION, msg);
				}
				catch (Exception e)
				{
					SharedSupport.HandleError(e, "ServerAction_InvalidQueue");
				}

				// update the status of the userAssignments	
				this.LastUpdatedDate = DateTime.Now;
				this.Update();
			}
			catch(Exception ex)
			{
				SharedSupport.HandleError(ex);
			}
		}

		internal static void SendActionToQueue(int userAssignmentID, bool build, bool check)
		{
			StudentAssignmentM student = StudentAssignmentM.Load(userAssignmentID);
			student.SendActionToQueue(build, check);
		}

		internal string AssignmentFilesXML(string guid, AssignmentM assign)
		{
			try
			{
				DataSet ds = new DataSet();
				string downloadRoot = SharedSupport.AddBackSlashToDirectory(System.Web.HttpContext.Current.Request.MapPath(String.Empty,Constants.ASSIGNMENTMANAGER_DOWNLOAD_DIRECTORY,true));
				string storageLocation = String.Empty;
			
				storageLocation = SharedSupport.AddBackSlashToDirectory(assign.StorageDirectory + _userID);

				//add unique guid to download path
				downloadRoot = downloadRoot + SharedSupport.AddBackSlashToDirectory(guid.ToString());

                if(!Directory.Exists(downloadRoot))
                {
                    Directory.CreateDirectory(downloadRoot);
                }

				DatabaseCall dbc = new DatabaseCall("StudentAssignments_GetFiles", DBCallType.Select);
				dbc.AddParameter("@UserAssignmentID", _userAssignmentID);
				dbc.Fill(ds);
				
				for(int i=0;i<ds.Tables[0].Rows.Count;i++)
				{
					string source = String.Empty;
					string destination = String.Empty;

					string filename = ds.Tables[0].Rows[i]["FileName"].ToString().Trim();
					filename = filename.Replace(@"\", @"/");
					source = SharedSupport.RemoveIllegalFilePathCharacters(storageLocation + filename);
					destination =  SharedSupport.RemoveIllegalFilePathCharacters(downloadRoot + filename);

                    //check to see if destination directory exists
                    string destinationDir = destination.Substring(0, destination.LastIndexOf(@"\"));
                    if(!Directory.Exists(destinationDir))
                    {
                        Directory.CreateDirectory(destinationDir);
                    }

					//check if file is there
					if(System.IO.File.Exists(source))
					{
						System.IO.File.Copy(source,destination,true);
					}
					else
					{
						throw new System.IO.FileNotFoundException(SharedSupport.GetLocalizedString("Assignment_UploadedFileNotFound"));
					}

					if(filename.StartsWith(@"/"))
					{
						ds.Tables[0].Rows[i]["FileName"] =  guid + filename;
					}
					else
					{
						ds.Tables[0].Rows[i]["FileName"] =  guid + @"/" + filename;
					}		
				}
				return ds.GetXml();
			}
			catch
			{
				return "";
			}
		}

	}
}
