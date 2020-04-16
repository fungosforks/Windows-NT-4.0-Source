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
						
			if (ds.Tables.Count <= 0)
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
						
			if (ds.Tables.Count <= 0)
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

		public int UserID
		{
			get { return _userID; }
			set { _userID = value; }
		}

		public int AssignmentID
		{
			get { return _assignmentID; }
			set { _assignmentID = value; }
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

	}
}
