//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//

namespace Microsoft.VisualStudio.Academic.AssignmentManager
{
    using System;
	using System.Web;
	using System.IO;
	using System.Data;
	using System.Security.Cryptography;
	using System.Text;

    /// <summary>
    ///    Summary description for SharedSupport.
    /// </summary>
    public class SharedSupport
    {
		private static string _connectionString = null;
		private static string _appName = SharedSupport.GetLocalizedString("Default_Title"); //"Assignment Manager";
		private static string _logMachine = System.Environment.GetEnvironmentVariable("COMPUTERNAME").ToString();
		private static System.Diagnostics.EventLogEntryType _logLevel = System.Diagnostics.EventLogEntryType.Information;
		private static string _baseUrl;
		private static bool _usingSsl;
		private static bool _usingSmtp = Convert.ToBoolean(System.Configuration.ConfigurationSettings.AppSettings["usingSmtp"]);

		// security type enumeration
		internal enum SecurityType : int {Cookie = 0, Windows = 1, Passport = 3, Generic = 4}

		/// <summary>
		/// persist database connection string 
		/// </summary>
		internal static string ConnectionString
		{
			get
			{
				try
				{
					// if Connection string is null and we have HttpContext, grab the connection string from Config.web
					if (_connectionString == null)
					{
						// Microsoft.VisualStudio.Academic.AssignmentManager.config file must be in bin directory!!
						LSAUtil lsa = new LSAUtil();
						_connectionString = lsa.RetrieveEncryptedString();
					}
					return _connectionString;
				}
				catch(System.Exception ex)
				{
					HandleError(ex);
				}

				return String.Empty;
			}
		}

		/// <summary>
		/// persist using SSL indicator
		/// </summary>
		internal static bool UsingSsl
		{
			get
			{
				try
				{
					if(System.Web.HttpContext.Current != null)
					{
						_usingSsl = Convert.ToBoolean(System.Configuration.ConfigurationSettings.AppSettings["usingSsl"]);						
					}
					else
					{
						// no HttpContext; set false
						_usingSsl = false;
					}
						
					return _usingSsl;
				}
				catch//(System.Exception ex)
				{
					//HandleError(ex);
					return false;
				}
				
				//return false;
			}
		}

		/// <summary>
		/// persist using Smtp indicator
		/// </summary>
		internal static bool UsingSmtp
		{
			get
			{
				try
				{
					return _usingSmtp;
				}
				catch//(System.Exception ex)
				{
					//HandleError(ex);
					return false;
				}
			}
		}

		/// <summary>
		///  persists BaseUrl: http://MachineName/VirtualDirectory
		/// </summary>
		internal static string BaseUrl
		{
			get
			{
				try
				{
					if (_baseUrl == null)
					{
						if(System.Web.HttpContext.Current != null)
						{
							_baseUrl = System.Web.HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST").ToString() + System.Web.HttpContext.Current.Request.ApplicationPath.ToString();
							
						}
						else
						{
							throw(new NullReferenceException(GetLocalizedString("General_Error_MissingHttpContext_BaseUrl_Exception")));
						}
						
					}
					return _baseUrl;
				}
				catch(System.Exception ex)
				{
					HandleError(ex);
				}

				return String.Empty;
			}
		}

		/// <summary>
		/// Logs message to appropriate source
		/// </summary>
		/// <param name="message"> </param>
		internal static void LogMessage(string message)
		{
			// assume info / debug message
			LogMessage(message, GetLocalizedString("General_Error_Logging_Debug"), System.Diagnostics.EventLogEntryType.Information);
		}

		/// <summary>
		/// Logs message to appropriate source
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="procedureName"> </param>
		/// <param name="msgType"> </param>
		internal static void LogMessage(string message, string procedureName, System.Diagnostics.EventLogEntryType msgType)
		{
			// log the message into the local event log
			System.Diagnostics.EventLog log = new System.Diagnostics.EventLog("Application", _logMachine, _appName);
			string msg = "";
			
			// build the log msg
			msg += System.DateTime.Now.ToString() + "\r\n";
			msg += GetLocalizedString("General_Error_Logging_ProcedureName") + "=" + procedureName + "\r\n"; 
			msg += GetLocalizedString("General_Error_Logging_Details") + "=" + message;

			// make sure we are still logging
			if (_logLevel >= msgType)
			{
				log.WriteEntry(msg, msgType);
			}
		}

		/// <summary>
		/// Logs, localizes, and re-throws errors
		/// </summary>
		/// <param name="e"> </param>
		internal static void HandleError(System.Exception e)
		{
			// handleErrorCore(e, GetLocalizedString("General_Error_Logging_InternalError") + ": " + e.Message);	
			handleErrorCore(e, e.Message);	

		}
		internal static void HandleError(System.Exception e, string customMessageToken)
		{
			// get localized error string
			string locMsg = GetLocalizedString(customMessageToken);

			// call core method
			handleErrorCore(e, locMsg);
			
		}

		/// <summary>
		/// Logs, localizes, and re-throws error
		/// </summary>
		/// <param name="e"> </param>
		/// <param name="customMessageToken"> </param>
		/// <param name="replacements"> </param>
		internal static void HandleError(System.Exception e, string customMessageToken, string[] replacements)
		{
			// get localized error string w/ replacements
			string locMsg = GetLocalizedString(customMessageToken, replacements);

			// call core method
			handleErrorCore(e, locMsg);
		}

		/// <summary>
		/// Core HandleError implementation
		/// </summary>
		/// <param name="e"> </param>
		/// <param name="errorMessage"> </param>
		private static void handleErrorCore(System.Exception e, string errorMessage)
		{
			// actual log and throw the error
			LogMessage(errorMessage, e.StackTrace, System.Diagnostics.EventLogEntryType.Error);

			// now throw the error back out
			throw new System.Exception(errorMessage, e);
		}

		/// <summary>
		/// Localizes string
		/// </summary>
		/// <param name="stringToken"> </param>
		internal static string GetLocalizedString(string stringToken)
		{
			// call core method
			return getLocalizedStringCore(stringToken);
		}

		/// <summary>
		/// Localizes string
		/// </summary>
		/// <param name="stringToken"> </param>
		/// <param name="replacements"> </param>
		internal static string GetLocalizedString(string stringToken, string[] replacements)
		{
			// call core method
			string locString = getLocalizedStringCore(stringToken);
			//make the replacements
			for (int i=0;i < replacements.Length;i++)
			{
				// replace all % inside localized string with passed-in replacement values
				locString = replaceString(locString, "%" + i.ToString(), replacements[i]);
			}

			// return the new string
			return locString;
		}

		/// <summary>
		/// Core GetLocalized implementation
		/// </summary>
		/// <param name="stringToken"> </param>
		private static string getLocalizedStringCore(string stringToken)
		{
			try
			{
				string culture = System.Globalization.CultureInfo.CurrentCulture.ToString();
				Microsoft.VisualStudio.Academic.AssignmentManager.Localization.SharedSupport ss = new Microsoft.VisualStudio.Academic.AssignmentManager.Localization.SharedSupport();			

				// calls to separate DLL which returns localized string
				return ss.GetLocalizedString(stringToken, culture);


			}
			catch(System.Exception ex)
			{
				HandleError(ex);
			}

			return String.Empty;
		}

		/// <summary>
		/// Returns user identity using cookie authentication
		/// </summary>
		internal static int GetUserIdentity()
		{
			try
			{
				if (System.Web.HttpContext.Current.User.Identity != null && System.Web.HttpContext.Current.User.Identity.Name.ToString() != "" && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
				{
					return Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.Name);
				}
				else
				{
					return 0;
				}
			}
			catch(System.Exception ex)
			{
				HandleError(ex);
			}
					
			return 0;
		}

		/// <summary>
		/// Used to replace variables inside GetLocalizedString.
		/// </summary>
		/// <param name="sourceString"> </param>
		/// <param name="oldString"> </param>
		/// <param name="newString"> </param>
		private static string replaceString(string sourceString, string oldString, string newString)
		{
			// replace the occurances of the oldString with the newString
			sourceString = sourceString.Replace(oldString,newString);
			return sourceString;
		}

		/// <summary>
		/// Retrieves value from Settings table
		/// </summary>
		/// <param name="settingName"> </param>
		internal static string GetSetting(string settingName)
		{
			try
			{
				// validate parameter
				if(settingName == null|| settingName == String.Empty) 
				{
					throw new ArgumentNullException(SharedSupport.GetLocalizedString("SharedSupport_SettingNameField"), 
						SharedSupport.GetLocalizedString("SharedSupport_Missing_SettingName"));
				}

				if (settingName.Equals(Constants.AUTOBUILD_SETTING) || settingName.Equals(Constants.AUTOCHECK_SETTING))
				{
					// query the status of the actual service
					return getServiceStatus(Constants.ACTION_SERVICE_NAME);
				}
				else
				{
					DatabaseCall dbc = new DatabaseCall("Settings_GetSetting", DBCallType.Select);
					dbc.AddParameter("@Setting", settingName);
					System.Data.DataSet ds = new System.Data.DataSet();
					dbc.Fill(ds);						
					try
					{
                        string returnval = ds.Tables[0].Rows[0]["Value"].ToString().Trim();
						return returnval;
					}
					catch
					{
						return String.Empty;
					}			
				}

			}
			catch (System.Exception e)
			{
				SharedSupport.HandleError(e);
				return null;
			}	

		}


		/// <summary>
		/// Sets value in Settings table
		/// </summary>
		/// <param name="settingName"> </param>
		/// <param name="settingValue"> </param>
		internal static void SetSetting(string settingName, string settingValue)
		{
			try
			{
				// validate parameters
				if(settingName.Equals(null) || settingName == String.Empty) throw new ArgumentNullException(SharedSupport.GetLocalizedString("SharedSupport_SettingNameField"), SharedSupport.GetLocalizedString("SharedSupport_Missing_SettingName"));
				if(settingValue.Equals(null) || settingValue == String.Empty) throw new ArgumentNullException(SharedSupport.GetLocalizedString("SharedSupport_SettingValueField"), SharedSupport.GetLocalizedString("SharedSupport_Missing_SettingValue"));

				// if they are changing the AutoBuild/AutoCheck update the Service Status
				if ((settingName.Equals(Constants.AUTOBUILD_SETTING)) || (settingName.Equals(Constants.AUTOCHECK_SETTING)))
				{
					// change the service status
					if (!changeServiceStatus(settingValue, Constants.ACTION_SERVICE_NAME))
					{
						// throw an exception b/c we couldn't update the service
						throw new System.Exception(GetLocalizedString("Setting_UnableToUpdateService_Error"));
					}
					// no database interaction for the Actions Service
				}
				else
				{
					//We're changing a setting stored in the database
					DatabaseCall dbc = new DatabaseCall("Settings_UpdateSetting", DBCallType.Execute);
					dbc.AddParameter("@Setting", settingName);
					dbc.AddParameter("@Value", settingValue);
					dbc.Execute();
				}
			}
			catch (System.Exception e)
			{
				 SharedSupport.HandleError(e);
			}			

		}
		/// <summary>
		/// Checks the string passed in and concatenates a backslash to a directory path if not already there.
		/// </summary>
		/// <param name="directoryLocation"> </param>
		internal static string AddBackSlashToDirectory(string directoryLocation)
		{
			if(!directoryLocation.EndsWith("\\") && !directoryLocation.Equals(String.Empty))
			{
				string s = directoryLocation.Trim() + "\\";
				return s;
			}
			else
			{
				return directoryLocation.Trim();
			}
		}
		/// <summary>
		/// Removes any illegal charaters from a file or directory path.  These illegal characters include ? : < > | /
		/// </summary>
		/// <param name="path"> </param>
		internal static string RemoveIllegalFilePathCharacters(string path)
		{
			//Cycle through each ? from start to finish
			while(path.IndexOf("?") != -1)
			{
				path = path.Remove(path.IndexOf("?"),1);
			}
			//Cycle through each < from start to finish
			while(path.IndexOf("<") != -1)
			{
				path = path.Remove(path.IndexOf("<"),1);
			}
			//Cycle through each > from start to finish
			while(path.IndexOf(">") != -1)
			{
				path = path.Remove(path.IndexOf(">"),1);
			}
			//Cycle through each | from start to finish
			while(path.IndexOf("|") != -1)
			{
				path = path.Remove(path.IndexOf("|"),1);
			}
			//Cycle through each / from start to finish
			while(path.IndexOf(@"/") != -1)
			{
				path = path.Replace(@"/",@"\");
			}
			//Cycle through each : from the last one in the string and keep looking 
			//until you get to the 2nd char position in the string.  If it 
			//is the second position it is part of the path such as 
			//in "C:\" where the : is in the 2nd position.
			while(path.LastIndexOf(":") > 1)
			{
				path = path.Remove(path.LastIndexOf(":"),1);
			}
			//Return the string free from illegal chars.
			return path;
		}

		internal bool ContainsIllegalCharactersUrl(string url)
		{
			// check for illegal characters
			System.Text.ASCIIEncoding AE = new System.Text.ASCIIEncoding();
			byte[] ByteArray = { 34, 39, 60, 61, 62, 91, 93, 123, 124, 125 };	// ", ', <, =, >, [, ], {, |, }
			char[] CharArray = AE.GetChars(ByteArray);

			return containsIllegalCharactersCore(CharArray, url);

		}
		internal bool ContainsIllegalCharactersFilePath(string path)
		{
			// check for illegal characters
			System.Text.ASCIIEncoding AE = new System.Text.ASCIIEncoding();
			byte[] ByteArray = { 34, 39, 60, 61, 62, 91, 92, 93, 123, 124, 125 };	// ", ', <, =, >, [, \, ], {, |, }
			char[] CharArray = AE.GetChars(ByteArray);

			return containsIllegalCharactersCore(CharArray, path);

		}

		private bool containsIllegalCharactersCore (char[] illegalChars, string field)
		{
			int numIllegals = field.IndexOfAny(illegalChars);
			if (numIllegals != -1) 
			{
				return true;
			}
			else
			{	
				return false;
			}
		}


		internal static void SendMessageToQueue(string mqPath, string mqName, string mqLabel, string mqMessage)
		{
			try
			{
				mqPath = @mqPath;
				mqName = @mqName;
				mqMessage = @mqMessage;				

				queueCreate(mqPath, mqName);

				System.Messaging.MessageQueue mq = new System.Messaging.MessageQueue(mqPath);
				mq.DefaultPropertiesToSend.Recoverable = true;
				mq.Send(mqMessage, mqLabel);

			}
			catch(System.Exception ex)
			{
				HandleError(ex);
			}
		}

		private static void queueCreate(string mqPath, string mqName)
		{
			try
			{
				mqPath = @mqPath;
				mqName = @mqName;

				// create if doesn't exist
				if(!System.Messaging.MessageQueue.Exists(mqPath))
				{
					System.Messaging.MessageQueue.Create(mqPath, false);

					// set other queue parameters
					System.Messaging.MessageQueue mq = new System.Messaging.MessageQueue(mqPath);
					mq.Label = mqName;
					//assumption Queue is on this box
					mq.MachineName = _logMachine;
					mq.QueueName = mqName;
				
				}	
			}
			catch(System.Exception ex)
			{
				HandleError(ex);
			}
		}

		private static string getServiceStatus(string serviceName)
		{

			// get an instance of the service
			System.ServiceProcess.ServiceController actionControl = new System.ServiceProcess.ServiceController(serviceName);
			if (actionControl.Status == System.ServiceProcess.ServiceControllerStatus.Stopped ||
				actionControl.Status == System.ServiceProcess.ServiceControllerStatus.StopPending ||
				actionControl.Status == System.ServiceProcess.ServiceControllerStatus.Paused ||
				actionControl.Status == System.ServiceProcess.ServiceControllerStatus.PausePending)
			{
				return "False";
			}
			else
			{
				return "True";
			}
		}
		private static bool changeServiceStatus(string settingValue, string serviceName)
		{
			// start/stops the Action Service
			bool newStatus = Convert.ToBoolean(settingValue);
			bool updated = false;
			
			// get an instance of the service
			System.ServiceProcess.ServiceController actionControl = new System.ServiceProcess.ServiceController(serviceName);
			if (newStatus)
			{
				// set the StartType is set to AutoStart so we restart when the machine boots
				ServiceControl.EnableService(serviceName);

				// attempt to start it
				if (actionControl.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
				{
					
					actionControl.Start();
					updated = true;
				}
				else if (actionControl.Status == System.ServiceProcess.ServiceControllerStatus.Running ||
					     actionControl.Status == System.ServiceProcess.ServiceControllerStatus.StartPending)
				{
					// already running or starting up
					updated = true;
				}
			}
			else
			{
				// attempt to stop it
				if (actionControl.Status == System.ServiceProcess.ServiceControllerStatus.Running && actionControl.CanStop)
				{
					actionControl.Stop();
					updated = true;
				}
				else if (actionControl.Status == System.ServiceProcess.ServiceControllerStatus.Stopped ||
					     actionControl.Status == System.ServiceProcess.ServiceControllerStatus.StopPending)
				{
					// already stopped or stopping
					updated = true;
				}
				// set the StartType to Disabled so we don't restart when the machine boots
				ServiceControl.DisableService(serviceName);
			}
			// we couldn't change the service status
			return updated;
		}

		/// <summary>
		/// Takes a string representing the date/time and attempts to turn it
		/// into a DateTime object. Note that if the current culture is not able
		/// to parse the DateTime object, then it falls back to the en-US culture,
		/// for globalization support (i.e. on platforms where we couldn't install
		/// resources).
		/// </summary>
		/// <param name="s">String representing the date</param>
		/// <returns>The DateTime format of the passed-in string</returns>
		internal static DateTime ParseDateTime(string s) 
					 		
		{
			DateTime t;
			try 	
			{
				t = Convert.ToDateTime(s);
			} 
			catch (System.Exception e) 
			{
				// If it fails to parse in the current culture, then fall back to the
				// en-US culture. Note that this, too, may throw, if an invalid date
				// was provided.
				try 
				{
					System.IFormatProvider format =	new System.Globalization.CultureInfo("en-US", true);
					t = Convert.ToDateTime(s, format);
				} 
				catch (System.Exception) 
				{
					// Rethrow the original exception so that the language and details of
					// the exception are for the default culture, not the fallback!
					throw e;
				}
			}
			return t;
		}

		// Genearate a secure password:
		// password must contain at least one each of: 
		// uppercase, lowercase, punctuation and numbers
		internal static string CreatePassword(int pwLength) 
		{
			const int STRONG_PWD_UPPER = 0;
			const int STRONG_PWD_LOWER = 1;
			const int STRONG_PWD_NUM = 2;
			const int STRONG_PWD_PUNC = 3;

			const int STRONG_PWD_CATS = (STRONG_PWD_PUNC + 1);
			const int NUM_LETTERS = 26;
			const int NUM_NUMBERS = 10;
			const int MIN_PWD_LEN = 8;

			if (pwLength-1 < MIN_PWD_LEN)
				return "";

			byte[] szPwd = new byte[pwLength];

			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			// generate a pwd pattern, each byte is in the range 
			// (0..255) mod STRONG_PWD_CATS
			// this indicates which character pool to take a char from
			byte[] pPwdPattern = new byte[pwLength];
			bool[] fFound = new bool[STRONG_PWD_CATS];
			do 
			{
				rng.GetBytes(pPwdPattern);

				fFound[STRONG_PWD_UPPER] = 
					fFound[STRONG_PWD_LOWER] =
					fFound[STRONG_PWD_PUNC] =
					fFound[STRONG_PWD_NUM] = false;

				for (int i=0; i < pwLength; i++) 
					fFound[pPwdPattern[i] % STRONG_PWD_CATS] = true;

				// check that each character category is in the pattern
			} while (!fFound[STRONG_PWD_UPPER] || 
				!fFound[STRONG_PWD_LOWER] || 
				!fFound[STRONG_PWD_PUNC] || 
				!fFound[STRONG_PWD_NUM]);

			// populate password with random data 
			// this, in conjunction with pPwdPattern, is
			// used to determine the actual data
			rng.GetBytes(szPwd);

			for (int i=0; i < pwLength; i++) 
			{ 
				byte bChar = 0;

				// there is a bias in each character pool because of the % function
				switch (pPwdPattern[i] % STRONG_PWD_CATS) 
				{

					case STRONG_PWD_UPPER : bChar = (byte)('A' + szPwd[i] % NUM_LETTERS);
						break;

					case STRONG_PWD_LOWER : bChar = (byte)('a' + szPwd[i] % NUM_LETTERS);
						break;

					case STRONG_PWD_NUM :   bChar = (byte)('0' + szPwd[i] % NUM_NUMBERS);
						break;

					case STRONG_PWD_PUNC :
					default:                
						string szPunc="!@#$%^&*()_-+=[{]};:\'\"<>,./?\\|~`";
						int dwLenPunc = szPunc.Length;
						bChar = (byte)(szPunc[szPwd[i] % dwLenPunc]);
						break;
				}

				szPwd[i] = bChar;
			}
			
			char[] pass = new char[szPwd.Length];
			for (int i=0;i<szPwd.Length; i++)
			{
				pass[i] = (char)szPwd[i];
			}
			return new string(pass);
		}

		internal static Byte[] ConvertStringToByteArray(String s)
		{
			return (new UnicodeEncoding()).GetBytes(s);
		}

		internal static System.Data.DataSet ParseDelimitedFile(string textFileName, string delimiter)
		{
			return SharedSupport.ParseDelimitedFile(textFileName, delimiter, -1);
		}

		internal static System.Data.DataSet ParseDelimitedFile(string textFileName, string delimiter, int numRows)
		{
			//verify that file is there.
			if(!System.IO.File.Exists(textFileName))
			{
				throw new System.IO.FileNotFoundException();
			}
			
			//Create a new dataset
			DataSet ds = new DataSet();
			//Create a new DataTable inside the dataset 
			DataTable dt = ds.Tables.Add();
			//Open text file and read into stream.
			System.IO.StreamReader sReader = System.IO.File.OpenText(textFileName);
			
			//Read the first line into the string
			string strline = sReader.ReadLine();
			
			// make sure we have a valid file w/data
			if (strline == null) 
			{
				throw new System.IO.FileNotFoundException(SharedSupport.GetLocalizedString("User_UploadFileNotFound"));
			}				
			string[] strArray = strline.Split(delimiter.ToCharArray());
			for(int c=0;c<strArray.Length;c++)
			{
				//add a column to the data table
				dt.Columns.Add();
			}
			//Only grab the number of rows from the parameter passed in.
			//If numRows = -1 then the function has been overloaded and loop through entire file.
			//initialize counter
			int i=0;
			while (strline != null && (i<numRows || numRows == -1))
			{
				//Split string into array
				strArray = strline.Split(delimiter.ToCharArray());
				//Create the row
				System.Data.DataRow dr = dt.NewRow();
				for(int c=0;c<strArray.Length;c++)
				{
					dr[c] = strArray[c].Trim();
				}
				//add the row to the table.
				dt.Rows.Add(dr);
				//Increment the counter
				i++;
				//Read the next line in the file
				strline = sReader.ReadLine();
				strArray = null;
			}	

			//Close the stream reader for the text file
			sReader.Close();
			//return the created dataset
			return ds;
		}

		//Course independant
		internal static bool SecurityIsAllowed(SecurityAction action)
		{
			PermissionsID temp;
			return SharedSupport.SecurityIsAllowed(action, out temp);
		}

		internal static bool SecurityIsAllowed(SecurityAction action, out PermissionsID maxPermID)
		{
			int userID = SharedSupport.GetUserIdentity();
			System.Data.OleDb.OleDbConnection con = new System.Data.OleDb.OleDbConnection(SharedSupport.ConnectionString);
			System.Data.OleDb.OleDbDataAdapter cmd = new System.Data.OleDb.OleDbDataAdapter("Security_GlobalIsAllowed", con);
			System.Data.DataSet ds = new System.Data.DataSet();
			System.Data.OleDb.OleDbParameter param;

			cmd.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;

			param = new System.Data.OleDb.OleDbParameter("@UserID", System.Data.OleDb.OleDbType.Integer);
			param.Value = userID;
			cmd.SelectCommand.Parameters.Add(param);

			param = new System.Data.OleDb.OleDbParameter("@ActionID", System.Data.OleDb.OleDbType.Integer);
			param.Value = (int)action;
			cmd.SelectCommand.Parameters.Add(param);

			maxPermID = PermissionsID.Student;
			try
			{
				cmd.Fill(ds);
			}
			catch (System.Exception e)
			{
				SharedSupport.HandleError(e);
			}
						
			try
			{
				if(Convert.ToInt32(ds.Tables[0].Rows[0]["UserID"]) == userID)
				{
					maxPermID = (PermissionsID)Convert.ToInt32(ds.Tables[0].Rows[0]["RoleID"]);
					return true;
				}
			}
			catch
			{
				return false;
			}			
			return false;			
		}

		internal static bool SecurityIsAllowed(int courseID, SecurityAction action)
		{
			return SharedSupport.SecurityIsAllowed(SharedSupport.GetUserIdentity(), courseID, action);
		}

		internal static bool SecurityIsAllowed(int userID, int courseID, SecurityAction action)
		{
			DatabaseCall dbc = new DatabaseCall("Security_IsAllowed", DBCallType.Select);
			dbc.AddParameter("@UserID", userID);
			dbc.AddParameter("@CourseID", courseID);
			dbc.AddParameter("@ActionID", (int)action);
			System.Data.DataSet ds = new System.Data.DataSet();
			dbc.Fill(ds);						
			try
			{
				if(Convert.ToInt32(ds.Tables[0].Rows[0]["UserID"]) == userID)
				{
					return true;
				}
			}
			catch
			{
				return false;
			}			
			return false;
		}

		internal static string HelpRedirect(string keyword)
		{
			return "ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword=\"" + keyword + "\"";
		}

        private SharedSupport()
        {

        }
    }
}
