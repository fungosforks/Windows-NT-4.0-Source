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
    /// Summary description for AssignmentM.
    /// </summary>
    internal class AssignmentM
    {
        private int _assignmentID = 0;

        //property variables
        string _description = null;
        string _shortName = null;
        bool _starterProjectFlag;
        string _compilerType = null;
        System.DateTime _lastUpdatedDate;
        int _lastUpdatedUserID = 0;
        byte _status = 0;
        int _courseId;

        //Course Assignment property values
        System.DateTime _dueDate;
        string _assignmentURL = null;
        string _commandLineArgs = null;
        string _inputFile = null;
        string _outputFile = null;
        bool _multipleSubmitsFlag = false;
        bool _autoGradeFlag = false;
        bool _autoCompileFlag = false;
        byte _gradeType = 0;
        bool _sendReminders = false;
        bool  _sendNewProject = true;
        bool  _sendPastDue = false;
        bool _sendUpdatedProject = false;
        int _courseAssignmentID = 0;
        string _makeFile = String.Empty;

        //Notification property values
        int _reminderWarningDays = 0;
        int _pastDueWarningDays = 0;

        private enum StoredProcType
        {
            New = 0,
            Update = 1
        }

        internal AssignmentM(int courseID)
        {
            _courseId = courseID;
        }

        private string _INVALID_SHORT_NAME_ERROR = SharedSupport.GetLocalizedString("Assignments_INVALID_SHORT_NAME_ERROR"); //"The Short Name cannot be blank.";
        private string _INVALID_SHORT_NAME_LENGTH_ERROR = SharedSupport.GetLocalizedString("Assignments_INVALID_SHORT_NAME_LENGTH_ERROR"); //"The Short Name cannot be more than 100 characters.";
        private string _INVALID_DESCRIPTION_ERROR = SharedSupport.GetLocalizedString("Assignments_INVALID_DESCRIPTION_ERROR"); //"The Description cannot be blank.";
        private string INVALID_ASSIGNMENT_ID_ERROR = SharedSupport.GetLocalizedString("AddEditAssignment_INVALID_ASSIGNMENT_ID_ERROR"); //"Invalid AssignmentID.  Assignment does not exist.";

        internal int Add()
        {
            return _assignmentID = this.SaveAssignmentToDatabase(StoredProcType.New);
        }

        internal static AssignmentM Load(int assignmentID)
        {
            //set courseID = 0, it will get overwritten when we load.
            AssignmentM retVal = new AssignmentM(0);

            DatabaseCall dbc = new DatabaseCall("Assignments_LoadAssignment", DBCallType.Select);
            dbc.AddParameter("@AssignmentID", assignmentID);
            DataSet ds = new DataSet();
            dbc.Fill(ds);
            if (ds.Tables.Count <= 0)
            {
                return null;
            }

            // Populate the return value.
            retVal._assignmentID = assignmentID;
            retVal.Description = ds.Tables[0].Rows[0]["Description"].ToString();
            retVal.StarterProjectFlag = Convert.ToBoolean( ds.Tables[0].Rows[0]["StarterProjectFlag"] );
            retVal.ShortName = ds.Tables[0].Rows[0]["ShortName"].ToString();
            retVal.CompilerType = ds.Tables[0].Rows[0]["CompilerType"].ToString();
            retVal.LastUpdatedDate = Convert.ToDateTime( ds.Tables[0].Rows[0]["LastUpdatedDate"].ToString() );
            retVal.LastUpdatedUserID = Convert.ToInt32( ds.Tables[0].Rows[0]["LastUpdatedUserID"] );
            retVal.DueDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["DueDate"].ToString());
            retVal.AssignmentURL = ds.Tables[0].Rows[0]["AssignmentURL"].ToString();
            retVal.CommandLineArgs = ds.Tables[0].Rows[0]["CommandLineArgs"].ToString();
            retVal.InputFile = ds.Tables[0].Rows[0]["InputFile"].ToString();
            retVal.OutputFile = ds.Tables[0].Rows[0]["OutputFile"].ToString();
            retVal.MultipleSubmitsFlag = Convert.ToBoolean( ds.Tables[0].Rows[0]["MultipleSubmitsFlag"] );
            retVal.AutoGradeFlag = Convert.ToBoolean( ds.Tables[0].Rows[0]["AutoGradeFlag"] );
            retVal.AutoCompileFlag = Convert.ToBoolean( ds.Tables[0].Rows[0]["AutoCompileFlag"] );
            retVal.SendReminders = Convert.ToBoolean( ds.Tables[0].Rows[0]["SendReminders"] );
            retVal.SendPastDue = Convert.ToBoolean( ds.Tables[0].Rows[0]["SendPastDue"] );
            retVal.SendNewProject = Convert.ToBoolean( ds.Tables[0].Rows[0]["SendNewProject"] );
            retVal.SendUpdatedProject = Convert.ToBoolean( ds.Tables[0].Rows[0]["SendUpdatedProject"] );
            retVal.GradeType = Convert.ToByte( ds.Tables[0].Rows[0]["GradeType"] );
            retVal.CourseAssignmentID = Convert.ToInt32(ds.Tables[0].Rows[0]["CourseAssignmentID"]);
            retVal.ReminderWarningDays = Convert.ToInt32( ds.Tables[0].Rows[0]["ReminderWarningDays"] );
            retVal.PastDueWarningDays = Convert.ToInt32( ds.Tables[0].Rows[0]["PastDueWarningDays"] );

            retVal.CourseID = Convert.ToInt32( ds.Tables[0].Rows[0]["CourseID"] );
            return retVal;
        }

        internal void Update()
        {
            this.SaveAssignmentToDatabase(StoredProcType.Update);
        }

        internal static void Delete(int assignid)
        {
            DatabaseCall dbc = new DatabaseCall("Assignments_Purge", DBCallType.Execute);
            dbc.AddParameter("@AssignmentID", assignid);
            dbc.Execute();			
        }

        internal string Description
        {
            get{ return _description; }
            set{ _description = value; }
        }

        internal string ShortName
        {
            set{ _shortName = value; }
            get{ return _shortName; }
        }

        internal bool StarterProjectFlag
        {
            set{ _starterProjectFlag = value; }
            get{ return _starterProjectFlag; }
        }

        //Combo Boxes
        internal string CompilerType 
        {
            set{ _compilerType = value; }
            get{ return _compilerType; }
        }

        //Other column values
        internal System.DateTime LastUpdatedDate 
        {
            set{ _lastUpdatedDate = value; }
            get{ return _lastUpdatedDate; }
        }
        internal int LastUpdatedUserID 
        {
            set{ _lastUpdatedUserID = value; }
            get{ return _lastUpdatedUserID; }
        }

        internal byte Status 
        {
            set{ _status = value; }
            get{ return _status; }
        }

        internal int AssignmentID
        {
            get{ return _assignmentID; }
            set{ _assignmentID = value; }
        }


        internal System.DateTime DueDate
        {
            set{ _dueDate = value; }
            get{ return _dueDate; }
        }

        internal string AssignmentURL
        {
            set{ _assignmentURL = value; }
            get{ return _assignmentURL; }
        }

        internal string CommandLineArgs
        {
            set{ _commandLineArgs = value; }
            get{ return _commandLineArgs; }
        }

        internal string InputFile
        {
            set{ _inputFile = value; }
            get{ return _inputFile; }
        }

        internal string OutputFile
        {
            set{ _outputFile = value; }
            get{ return _outputFile; }
        }

        internal bool MultipleSubmitsFlag
        {
            set{ _multipleSubmitsFlag = value; }
            get{ return _multipleSubmitsFlag; }
        }

        internal bool AutoGradeFlag
        {
            set{ _autoGradeFlag = value; }
            get{ return _autoGradeFlag; }
        }

        internal bool AutoCompileFlag
        {
            set{ _autoCompileFlag = value; }
            get{ return _autoCompileFlag; }
        }

        internal byte GradeType
        {
            set{ _gradeType = value; }
            get{ return _gradeType; }
        }

        internal bool SendReminders
        {
            set{ _sendReminders = value; }
            get{ return _sendReminders; }
        }

        internal bool SendNewProject
        {
            set{ _sendNewProject = value; }
            get{ return _sendNewProject; }
        }

        internal bool SendPastDue
        {
            set{ _sendPastDue = value; }
            get{ return _sendPastDue; }
        }

        internal bool SendUpdatedProject
        {
            set{ _sendUpdatedProject = value; }
            get{ return _sendUpdatedProject; }
        }

        internal int CourseAssignmentID
        {
            set{ _courseAssignmentID = value; }
            get{ return _courseAssignmentID; }
        }

        internal string MakeFile
        {
            set{ _makeFile = value; }
            get{ return _makeFile; }
        }

        internal int ReminderWarningDays
        {
            set{ _reminderWarningDays = value; }
            get{ return _reminderWarningDays; }
        }

        internal int PastDueWarningDays
        {
            set{ _pastDueWarningDays = value; }
            get{ return _pastDueWarningDays; }
        }

        internal int CourseID
        {
            set{ _courseId = value; }
            get{ return _courseId; }
        }
        internal bool IsValid
        {
            get{ return (_assignmentID != 0); }
        }

        private int SaveAssignmentToDatabase(StoredProcType procType)
        {
            int retID = 0;
            DatabaseCall dbc = null;
            switch( procType )
            {
                case StoredProcType.New:
                    dbc = new DatabaseCall("Assignments_AddNewAssignment", DBCallType.Execute);
                    dbc.AddOutputParameter("@AssignmentID");
                    dbc.AddOutputParameter("@CourseAssignmentID");
                    break;

                case StoredProcType.Update:
                    dbc = new DatabaseCall("Assignments_UpdateExistingAssignment", DBCallType.Execute);
                    dbc.AddParameter("@AssignmentID", this.AssignmentID);
                    break;

                default:
                    throw new Exception("Invalid Stored Procedure");
            }

            dbc.AddParameter("@ShortName", this.ShortName);
            dbc.AddNTextParameter("@Description", this.Description);
            dbc.AddParameter("@LastUpdatedDate", this.LastUpdatedDate);
            dbc.AddParameter("@LastUpdatedUserID", this.LastUpdatedUserID);
            dbc.AddParameter("@StarterProjectFlag", this.StarterProjectFlag);
            dbc.AddParameter("@MakeFile", this.MakeFile);
            dbc.AddParameter("@CompilerType", this.CompilerType);
            dbc.AddParameter("@CourseID", this.CourseID);
            dbc.AddParameter("@DueDate", this.DueDate);
            dbc.AddParameter("@MultipleSubmitsFlag", this.MultipleSubmitsFlag);
            dbc.AddParameter("@SendReminders", this.SendReminders);
            dbc.AddParameter("@AutoGradeFlag", this.AutoGradeFlag);
            dbc.AddParameter("@InputFile", this.InputFile);  // set to either DBNull or String Valu);
            dbc.AddParameter("@OutputFile", this.OutputFile);  // set to either DBNull or String Valu);
            dbc.AddParameter("@GradeType", this.GradeType);
            dbc.AddParameter("@AutoCompileFlag", this.AutoCompileFlag);
            dbc.AddParameter("@AssignmentURL", this.AssignmentURL);   // set to either DBNull or String Valu);
            dbc.AddParameter("@CommandLineArgs", this.CommandLineArgs);
            dbc.AddParameter("@SendPastDue", this.SendPastDue);
            dbc.AddParameter("@SendNewProject", this.SendNewProject);
            dbc.AddParameter("@SendUpdatedProject", this.SendUpdatedProject);
            dbc.AddParameter("@PastDueWarningDays", this.PastDueWarningDays);
            dbc.AddParameter("@ReminderWarningDays", this.ReminderWarningDays);

            dbc.Execute();

            if(procType == StoredProcType.New)
            {
                retID = Convert.ToInt32( dbc.GetOutputParam("@AssignmentID") );
                _courseAssignmentID = Convert.ToInt32( dbc.GetOutputParam("@CourseAssignmentID") );
            }

            return retID;
        }

        public bool SubmitStarter(string xmlFileListing, string pathGuid)
        {
            if (this.IsValid)
            {
                bool submitSuccess = true;
                try
                {
                    System.Data.DataSet dsXmlFileListing = new System.Data.DataSet();
                    System.IO.StringReader reader = new System.IO.StringReader(xmlFileListing);
                    try 
                    {
                        System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(reader);
                        dsXmlFileListing.ReadXml(xmlReader);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(SharedSupport.GetLocalizedString("UploadDownload_UnableToCopyToServer"), ex);
                    }

                    this.ClearStarter();

                    string uploadPath = SharedSupport.AddBackSlashToDirectory(System.Web.HttpContext.Current.Request.MapPath(String.Empty,Constants.ASSIGNMENTMANAGER_UPLOAD_DIRECTORY,true));
                    uploadPath += SharedSupport.AddBackSlashToDirectory(pathGuid);

                    string destinationPath = SharedSupport.AddBackSlashToDirectory(this.StorageDirectory + Constants.STARTER_PROJECT_PATH);

                    try
                    {
                        //Clear old directory
                        if(Directory.Exists(destinationPath))
                        {
                            Directory.Delete(destinationPath, true);
                        }
                        Directory.CreateDirectory(destinationPath);
                    }
                    catch
                    {
                    }

                    //Save all files and relative paths to assignmentfile table
                    for(int i=0;i<dsXmlFileListing.Tables[0].Rows.Count;i++)
                    {
                        string filename = dsXmlFileListing.Tables[0].Rows[i]["FileName"].ToString();
                        if(filename.StartsWith(@"\") || filename.StartsWith(@"/"))
                        {
                            filename = filename.Remove(0,1);
                        }
                        string sourceFile = String.Empty;
                        string destinationFile = String.Empty;
                        sourceFile = SharedSupport.RemoveIllegalFilePathCharacters(uploadPath + filename);
                        destinationFile = SharedSupport.RemoveIllegalFilePathCharacters(destinationPath + filename);
                        //check to make sure the target directory exists
                        string targetDirectory = destinationFile.Substring(0, destinationFile.LastIndexOf("\\"));
                        if (!Directory.Exists(targetDirectory))
                        {
                            Directory.CreateDirectory(targetDirectory);
                        }

                        //check if file is there
                        if(System.IO.File.Exists(sourceFile))
                        {
                            System.IO.File.Copy(sourceFile, destinationFile, true);
                        }
                        else
                        {
                            throw new System.IO.FileNotFoundException(SharedSupport.GetLocalizedString("Assignment_UploadedFileNotFound"));
                        }

                        this.AddFile(filename);
                    }
                    try
                    {
                        Directory.Delete(uploadPath,true);
                    }
                    catch
                    {
                    }

                    // Send new Starter project notice.
                    if(_sendNewProject && SharedSupport.UsingSmtp)
                    {
                        string[] AssignmentName = new string[]{_shortName};
                        string subject = SharedSupport.GetLocalizedString("Notification_UpdatedProjectSubject", AssignmentName);
                        string body = SharedSupport.GetLocalizedString("Notification_UpdatedProjectBody", AssignmentName);
                        MessageM.sendEmailMessageToCourse(subject, body,String.Empty, _courseId);
                    }
                }
                catch(Exception ex)
                {
                    SharedSupport.HandleError(ex);
                    submitSuccess = false;
                }

                if (submitSuccess)
                {
                    this.StarterProjectFlag = true;
                    this.Update();
                }
                return submitSuccess;
            }
            else
            {
                return false;
            }
        }

        public string StorageDirectory
        {
            get
            {
                string rootDestinationPath = String.Empty;
                CourseM course = CourseM.Load(this._courseId);

                // Build folder structure 
                // <CourseRootStoragePath>\<CourseID>\<AssignmentID>
                rootDestinationPath += SharedSupport.AddBackSlashToDirectory(course.RootStoragePath);
                rootDestinationPath += SharedSupport.AddBackSlashToDirectory(course.CourseID.ToString());
                rootDestinationPath += SharedSupport.AddBackSlashToDirectory(_assignmentID.ToString());
                return rootDestinationPath;
            }
        }

        public static void UpdateAutoCheckFiles(int assignmentID, int courseID, Guid rootGuid)
        {
            //AssignmentFiles.UpdateAutoCheckFiles(assignmentID, courseID, rootGuid);
        }

        public string StarterFilesXML(System.Guid guid)
        {
            try
            {
                DataSet ds = new DataSet();
                string downloadRoot = SharedSupport.AddBackSlashToDirectory(System.Web.HttpContext.Current.Request.MapPath(String.Empty,Constants.ASSIGNMENTMANAGER_DOWNLOAD_DIRECTORY,true));
                string storageLocation = String.Empty;
                AssignmentM assign = AssignmentM.Load(_assignmentID);
                storageLocation = SharedSupport.AddBackSlashToDirectory(assign.StorageDirectory + Constants.STARTER_PROJECT_PATH);

                //add unique guid to download path
                downloadRoot = downloadRoot + SharedSupport.AddBackSlashToDirectory(guid.ToString());

                DatabaseCall dbc = new DatabaseCall("Assignments_GetFiles", DBCallType.Select);
                dbc.AddParameter("@AssignmentID", _assignmentID);
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
                    string destinationDir = destination.Substring(0, destination.LastIndexOf("\\")+1);
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
            catch( Exception e )
            {

                return e.Message;
            }
        }

        private void AddFile(string filename)
        {
            DatabaseCall dbc = new DatabaseCall("Assignments_AddFile", DBCallType.Execute);
            dbc.AddParameter("@AssignmentID", _assignmentID);
            dbc.AddParameter("@FileName", filename);
            dbc.Execute();
        }

        public void ClearStarter()
        {
            DatabaseCall dbc = new DatabaseCall("Assignments_ClearStarter", DBCallType.Execute);
            dbc.AddParameter("@AssignmentID", _assignmentID);
            dbc.Execute();
        }
    }
}
