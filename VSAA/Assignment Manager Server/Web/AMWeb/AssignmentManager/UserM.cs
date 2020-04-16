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
using System.Security.Cryptography;
namespace Microsoft.VisualStudio.Academic.AssignmentManager
{
    /// <summary>
    /// Summary description for UserM.
    /// </summary>
    public class UserM
    {
        private int _userID = 0;

        //property variables
        private string _lastName = String.Empty;
        private string _middleName = String.Empty;
        private string _firstName = String.Empty;
        private string _emailAddress = String.Empty;
        private string _universityID = String.Empty;
        private string _username = String.Empty;
        private string _password = String.Empty;
        private System.DateTime _lastUpdatedDate;
        private int _lastUpdatedUserID = 0;
        private bool _changedPassword = false;

        private enum StoredProcType
        {
            Create = 0,
            Update = 1
        }

        public UserM()
        {
        }

        public static void RemoveFromCourse(int userID, int courseID)
        {
            DatabaseCall dbc = new DatabaseCall("Users_Purge", DBCallType.Execute);
            dbc.AddParameter("@UserID", userID);
            dbc.AddParameter("@CourseID", courseID);
            dbc.Execute();
        }

        public static UserM Load(int userID)
        {
            DatabaseCall dbc = new DatabaseCall("Users_LoadUserByUserID", DBCallType.Select);
            dbc.AddParameter("@UserID", userID);

            return LoadUserFromDatabase(dbc);
        }

        public static UserM LoadByUserName(string username)
        {
            DatabaseCall dbc = new DatabaseCall("Users_LoadUserByUsername", DBCallType.Select);
            dbc.AddParameter("@UserName", username);

            return LoadUserFromDatabase(dbc);
        }

        public static UserM LoadByUniversityID(string universityID)
        {
            DatabaseCall dbc = new DatabaseCall("Users_LoadUserByUniversityID", DBCallType.Select);
            dbc.AddParameter("@UniversityID", universityID);

            return LoadUserFromDatabase(dbc);
        }

        public static UserM LoadByEmail(string email)
        {
            DatabaseCall dbc = new DatabaseCall("Users_LoadUserByEmail", DBCallType.Select);
            dbc.AddParameter("@Email", email);

            return LoadUserFromDatabase(dbc);
        }


        public void UpdatePassword()
        {
            this.SetPassword(this._password, true);
        }
        public void SetPassword(string password, bool hasChanged)
        {
            Byte[] passwd = SharedSupport.ConvertStringToByteArray(password.Trim());
            byte[] hashValue = ((HashAlgorithm) CryptoConfig.CreateFromName(Constants.HashMethod)).ComputeHash(passwd);
            string hashedPassword = BitConverter.ToString(hashValue);


            DatabaseCall dbc = new DatabaseCall("Users_ChangePassword", DBCallType.Execute);
            dbc.AddParameter("@UserID", _userID);
            dbc.AddParameter("@Password", hashedPassword);
            dbc.AddParameter("@ChangedPassword", hasChanged);
            dbc.Execute();

            // This is only true when a faculty member updated someone else's password
            if(!hasChanged)
            {
                SendPasswordToUser(password);
            }
        }

        private static UserM LoadUserFromDatabase(DatabaseCall dbc)
        {
            UserM newUser = new UserM();
            System.Data.DataSet ds = new System.Data.DataSet();
            dbc.Fill(ds);
            if ((ds.Tables.Count <= 0) || (ds.Tables[0].Rows.Count <= 0) )
            {
                return newUser;
            }

            newUser._userID = Convert.ToInt32( ds.Tables[0].Rows[0]["UserID"] );
            newUser._lastName = ds.Tables[0].Rows[0]["LastName"].ToString();
            newUser._middleName = ds.Tables[0].Rows[0]["MiddleName"].ToString();
            newUser._firstName = ds.Tables[0].Rows[0]["FirstName"].ToString();
            newUser._emailAddress = ds.Tables[0].Rows[0]["Email"].ToString();
            newUser._universityID = ds.Tables[0].Rows[0]["UniversityIdentifier"].ToString();
            newUser._username = ds.Tables[0].Rows[0]["UserName"].ToString();
            newUser._password = ds.Tables[0].Rows[0]["Password"].ToString();
            newUser._lastUpdatedDate = Convert.ToDateTime( ds.Tables[0].Rows[0]["LastUpdatedDate"].ToString() );
            newUser._lastUpdatedUserID = Convert.ToInt32( ds.Tables[0].Rows[0]["LastUpdatedUserID"] );
            newUser._changedPassword = Convert.ToBoolean( ds.Tables[0].Rows[0]["ChangedPassword"] );

            return newUser;
        }

        public int Update()
        {
            return saveUserToDatabase(StoredProcType.Update);
        }

        public int Create()
        {
            return Create(true);
        }

        public int Create(bool mailPassword)
        {
            generatePassword(mailPassword);
            return saveUserToDatabase(StoredProcType.Create);
        }

        private int saveUserToDatabase(StoredProcType sprocType)
        {

            DatabaseCall dbc;
            int retID=0;
            if (sprocType == StoredProcType.Create)
            {
                dbc = new DatabaseCall("Users_CreateNew", DBCallType.Execute);
                dbc.AddOutputParameter("@UserID");
            }
            else if (sprocType == StoredProcType.Update)
            {
                dbc = new DatabaseCall("Users_UpdateExisting", DBCallType.Execute);
                dbc.AddParameter("@UserID", _userID);
            }
            else
            {
                throw new Exception("Unknown Stored Procedure Type");
            }

            dbc.AddParameter("@LastName", _lastName);
            dbc.AddParameter("FirstName", _firstName);
            dbc.AddParameter("@MiddleName", _middleName);
            dbc.AddParameter("@Email", _emailAddress);
            dbc.AddParameter("@UniversityIdentifier", _universityID);
            dbc.AddParameter("@UserName", _username);
            dbc.AddParameter("@Password", _password);
            dbc.AddParameter("@LastUpdatedDate", _lastUpdatedDate);
            dbc.AddParameter("@LastUpdatedUserID", _lastUpdatedUserID);
            dbc.AddParameter("@ChangedPassword", _changedPassword);
            dbc.Execute();
            if( sprocType == StoredProcType.Create )
            {
                retID = Convert.ToInt32( dbc.GetOutputParam("@UserID") );
                this._userID = retID;
            }

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

        internal int UserID
        {
            get{ return _userID; }
            set{ _userID = value; }
        }

        internal string LastName
        {
            get{ return _lastName; }
            set{ _lastName = value; }
        }

        internal string MiddleName
        {
            get{ return _middleName; }
            set{ _middleName = value; }
        }

        internal string FirstName
        {
            get{ return _firstName; }
            set{ _firstName = value; }
        }

        internal string EmailAddress
        {
            get{ return _emailAddress; }
            set{ _emailAddress = value; }
        }

        internal string UniversityID
        {
            get{ return _universityID; }
            set{ _universityID = value; }
        }

        internal string UserName
        {
            get{ return _username; }
            set{ _username = value; }
        }

        internal string Password
        {
            set{ _password = value;}
            get{ return _password; }
        }

        internal System.DateTime LastUpdatedDate
        {
            get{ return _lastUpdatedDate; }
            set{ _lastUpdatedDate = value; }
        }

        internal int LastUpdatedUserID
        {
            get{ return _lastUpdatedUserID; }
            set{ _lastUpdatedUserID = value; }
        }

        internal bool ChangedPassword
        {
            get{ return _changedPassword; }
            set{ _changedPassword = value; }
        }

        public bool IsValid
        {
            get{ return this._userID != 0; }
        }

        public bool IsInCourse(int courseId)
        {
            UserList userList = UserList.GetListFromCourse(courseId);
            int[] users = userList.UserIDList;
            for(int i=0;i<users.Length;i++)
            {
                if(users[i] == _userID)
                {
                    return true;
                }
            }
            return false;
        }

        internal void GenerateNewPassword()
        {
            generatePassword(true);
            UpdatePassword();
        }

        private void generatePassword(bool mailPassword)
        {
            const int PASSWORD_LENGTH = 10;
            string initialPassword = String.Empty;
            bool usingSMTP = SharedSupport.UsingSmtp;
            // If the user has SMTP enabled, then generate a more secure password and send it to them. It doesn't
            // matter if it's terribly secure, as they're going to be forced to change it at first logon. If they don't
            // have SMTP enabled, default the password to their username.
            if (usingSMTP) 
            {
                // generate the password
                initialPassword = getCharStringFromName(PASSWORD_LENGTH, this._emailAddress + this._username + this._firstName + this._lastName);
                if(mailPassword)
                {
                    SendPasswordToUser(initialPassword);
                }
            }
            else 
            {
                initialPassword = this._username;
            }

            Byte[] passwd = SharedSupport.ConvertStringToByteArray(initialPassword);
            byte[] hashValue = ((HashAlgorithm) CryptoConfig.CreateFromName(Constants.HashMethod)).ComputeHash(passwd);
            string hashedPassword = BitConverter.ToString(hashValue);

            //Set password
            this._password = hashedPassword;
            // Make the user change it when they first log in.
            this._changedPassword = false;
        }

        private string getCharStringFromName(int length, string name)
        {
            System.Random random = new System.Random();
            string pwd = "";
            int charCode = 0;
            for (int i = 0; i < 12; i++)
            {
                charCode = random.Next(35, 126);
                pwd += Convert.ToChar(charCode);
            }
            return pwd;
        }

        public void SendPasswordToUser()
        {
            // use Assignment Manager sysadmin email 
            UserM amsaUser = UserM.Load(Constants.ASSIGNMENTMANAGER_SYSTEM_ADMIN_USERID);
            string sentByEmail = amsaUser.EmailAddress;
            string emailSubject = SharedSupport.GetLocalizedString("User_EmailSubject");
            string[] replacements = new string[2] {this._username, this._password};
            string emailBody = SharedSupport.GetLocalizedString("User_EmailBody", replacements);

            MessageM.SendMessage(sentByEmail, this._emailAddress, emailSubject, emailBody);
        }

        public void AddToCourse(int courseID)
        {
            AddToCourse(courseID, PermissionsID.Student);
        }

        internal void AddToCourse(int courseID, PermissionsID roleID)
        {
            DatabaseCall dbc = new DatabaseCall("Users_AddToCourse", DBCallType.Execute);
            dbc.AddParameter("@UserID", _userID);
            dbc.AddParameter("@CourseID", courseID);
            dbc.AddParameter("RoleID", (int)roleID);
            dbc.Execute();
        }

        public void ImportToCourse(int courseID, string importID)
        {
            DatabaseCall dbc = new DatabaseCall("Users_ImportToCourse", DBCallType.Execute);
            dbc.AddParameter("@UserID", _userID);
            dbc.AddParameter("@CourseID", courseID);
            dbc.AddParameter("@ImportID", importID);
            dbc.Execute();
        }

        public static UserM AuthenticateUser(string username, string password)
        {
            UserM user = UserM.LoadByUserName(username);
            //Compare the hashed version of the password stored in the db to the hashed version of the password entered.
            Byte[] passwd = SharedSupport.ConvertStringToByteArray(password.Trim());
            byte[] hashValue = ((HashAlgorithm) CryptoConfig.CreateFromName(Constants.HashMethod)).ComputeHash(passwd);

            if (user.Password != BitConverter.ToString(hashValue))
            {
                return new UserM();
            }
            else
            {
                return user;
            }
        }

        public RoleM GetRoleInCourse(int courseID)
        {
            return RoleM.GetUsersRoleInCourse(_userID, courseID);
        }

        public void SetRoleInCourse(int courseID, int roleID)
        {
            RoleM.SetRoleInCourse(_userID, courseID, roleID);
        }

        private void SendPasswordToUser(string password)
        {
            // email if new user
            if (SharedSupport.UsingSmtp)
            {
                string subject = SharedSupport.GetLocalizedString("ChangePassword_NewPasswordEmailSubject");
                string body = SharedSupport.GetLocalizedString("ChangePassword_NewPassword_UsernameMessage") + " " + this._username + "\n";
                body += SharedSupport.GetLocalizedString("ChangePassword_NewPassword_Message") + " " + password;
                UserM amsaUser = UserM.Load(Constants.ASSIGNMENTMANAGER_SYSTEM_ADMIN_USERID);
                MessageM.SendMessage(amsaUser.EmailAddress, this._emailAddress, subject, body);

            }
        }
    }
}
