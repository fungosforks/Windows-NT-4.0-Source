//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Xml;
using System.Data;
using System.IO;
using System.Reflection;
using System.DirectoryServices;
using System.Web;
using System.Messaging;

namespace AMInstall
{
	/// <summary>
	/// Summary description for AMInstall.
	/// </summary>
	[RunInstaller(true)]
	public class AMInstall : System.Configuration.Install.Installer
	{

		// Enumerator for the install parameters / arguments passed 
		// to the custom action
		private enum InstallParams {TargetWebDirectory=0, ProgramFilesFolder=1, DBServer=2, DBName=3, DBUserID=4, DBPassword=5, SMTPEnabled=6, SSLEnabled=7, TargetVDir=8};

		// Install savedState keys
		private const string webName = "VDIR";
		private const string SSLEnabled = "SSLENABLED";
		private const string installPath = "INSTALLPATH";
		
		private const string appPoolName = "AssignmentManager";

		// Queue constants
		private const string ACTION_QUEUE_NAME = "AMActions";
		private const string ACTION_QUEUE_PATH = ".\\Private$\\AMActions";

		//Course Root Storage path
		private string DEFAULT_COURSE_OFFERINGS_ROOT_STORAGE_PATH = System.Environment.GetEnvironmentVariable("ProgramFiles")+ @"\Assignment Manager\Data\";

		public AMInstall()
		{
			// This call is required by the Designer.
			InitializeComponent();
		}

		public override void Install(System.Collections.IDictionary savedState)
		{
			String args = this.Context.Parameters["Args"];
			String[] individualArgs;
			String filePath = "";
			string installUser = "";
			string installPwd = "";

			individualArgs = args.Split(Convert.ToChar(","));
			if (args == ""){
				// throw an exception, we're expecting parameters/argumen
				throw new InstallException("No arguments specified");
			}

			// store the setup options we need for uninstall
			if (!saveInstallState(savedState, individualArgs))
			{
				throw new InstallException("Can't persist install parameters");
			}

			// valid arguments, do the install work
			filePath = individualArgs[(int)InstallParams.ProgramFilesFolder] + "Assignment Manager\\Setup";
			// grab the db install user/pwd
			installUser = individualArgs[(int)InstallParams.DBUserID];
			installPwd = individualArgs[(int)InstallParams.DBPassword];

			if( individualArgs[(int)InstallParams.TargetVDir] == "" )
			{
				throw new InstallException("individualArgs[VDir] = " + individualArgs[(int)InstallParams.TargetVDir] + 
											"\nargs[web] = " + individualArgs[(int)InstallParams.TargetWebDirectory]);
			}

			// create the db
			if (! createDB(individualArgs))
			{
				throw new  InstallException("Error occured creating the database");
			}

			// do the web site config
			Process p2;
			string targetVDir = individualArgs[(int)InstallParams.TargetVDir];
			string sslValue = getBooleanValue(individualArgs[(int)InstallParams.SSLEnabled]);

			ProcessStartInfo si2 = new System.Diagnostics.ProcessStartInfo("\"" + filePath + "\\RemoveScriptMaps.vbs\"", "\"" + targetVDir + "\" \"" + sslValue + "\"");
			
			si2.WindowStyle = ProcessWindowStyle.Hidden;
			try
			{
				p2 = Process.Start(si2);
				p2.WaitForExit();
			}
			catch (Exception ex)
			{
				System.Diagnostics.EventLog.WriteEntry("AMInstall", ex.ToString());
				throw new InstallException(ex.Message);
			}


            try
            {
                createApplicationPool(appPoolName);
                if (DirectoryEntry.Exists("IIS://localhost/w3svc/AppPools/AssignmentManager"))
                {
                    // if Application Pools exist, then add AssignmentManager to our newly created pool
                    addVDirToAppPool(individualArgs[(int)InstallParams.TargetVDir], appPoolName);				
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("AMInstall", ex.ToString());
            }
			
			
			// Set the upload directory to Read/Write/Delete for Everyone, Full Control for System.
			if (!SecurityPermissions.SetVDirSecurityDescriptor("AMUpload","D:(A;OICI;GWGRSD;;;WD)(A;OICI;GA;;;SY)"))
			{
				System.Diagnostics.EventLog.WriteEntry("AMInstall", "Unable to set the AMUpload directory writeable for Everyone.");
			}
			// Set the download directory to Full Control for Local System, Read/Delete for Everyone.
			if (!SecurityPermissions.SetVDirSecurityDescriptor("AMDownload","D:(A;OICI;GA;;;SY)(A;OICI;GRSD;;;WD)"))
			{
				System.Diagnostics.EventLog.WriteEntry("AMInstall", "Unable to set permissions on the AMDownload directory.");
			}

			// update the web.config file
			if (!updateConfigFile(individualArgs[(int)InstallParams.TargetWebDirectory] + "web.config", individualArgs)) 
			{
				System.Diagnostics.EventLog.WriteEntry("AMInstall", "Error occured while updating configuration files", System.Diagnostics.EventLogEntryType.Error);
				throw new InstallException();
			}

			// update the ActionService.exe.config file
			if (!updateConfigFile(individualArgs[(int)InstallParams.ProgramFilesFolder] + "Assignment Manager\\Controller\\ActionService.exe.config", individualArgs))
			{
				System.Diagnostics.EventLog.WriteEntry("AMInstall", "Error occured while updating application configuration files", System.Diagnostics.EventLogEntryType.Error);
				throw new InstallException();
			}

			// update the Controller.exe.config file
			if (!updateConfigFile(individualArgs[(int)InstallParams.ProgramFilesFolder] + "Assignment Manager\\Controller\\Controller.exe.config", individualArgs))
			{
				System.Diagnostics.EventLog.WriteEntry("AMInstall", "Error occured while updating application configuration files", System.Diagnostics.EventLogEntryType.Error);
				throw new InstallException();
			}

			if (!AMUser.CreateUser())
			{
				System.Diagnostics.EventLog.WriteEntry("AMInstall", "Error occured while creating the AssignmentManager user", System.Diagnostics.EventLogEntryType.Error);
				throw new InstallException();
			}

			if (!AMUser.AddUserToGroup())
			{
				System.Diagnostics.EventLog.WriteEntry("AMInstall", "Error occured while addding the AssignmentManager user to the Users group", System.Diagnostics.EventLogEntryType.Error);
				throw new InstallException();
			}

			//Create data directory
			if (!CreateDataDirectory())
			{
				System.Diagnostics.EventLog.WriteEntry("AMInstall", "Cannot create c:\\data directory.", System.Diagnostics.EventLogEntryType.Error);
				throw new InstallException();
			}

			if (!setASPNETProcessInfinite()) 
			{
				// Do not abort in this case; they can still continue with installation.
				System.Diagnostics.EventLog.WriteEntry("AMInstall", "Unable to set the ASP .NET worker process timeout to Infinite.", System.Diagnostics.EventLogEntryType.Warning);
			}

			// Change process model to run as Local System
			if (!changeASPNETProcessModel())
			{
				// Do not abort on this case; the user may have custom set their machine.config file.
				System.Diagnostics.EventLog.WriteEntry("AMInstall", "Cannot change the ASP.NET Process model in machine.config", EventLogEntryType.Information);
			}

			string acl = "D:"				  // DACL
				+ "(A;OICI;GA;;;SY)"  // Local System full control
				+ "(A;OICI;GA;;;BA)"  // Administrators full control
				+ "(A;OICI;GR;;;WD)"; // Everyone Read
			if (!SecurityPermissions.RecurseAndSetSecurity( individualArgs[(int)InstallParams.TargetWebDirectory] , acl))
			{
				System.Diagnostics.EventLog.WriteEntry("AMInstall", "Cannot change permissions on " + individualArgs[(int)InstallParams.TargetWebDirectory] );
			}
	
			base.Install(savedState);
		}

		public override void Uninstall(IDictionary savedState)
		{
			System.Diagnostics.EventLog.WriteEntry("AMInstall", "Begin Uninstall");
	
			LSAUtil lsa = new LSAUtil();

			try
			{
				// remove the connection string stored in the LSA
				lsa.DeletePrivateKey();

				// remove the SSL Flag from the IIS Vdir if it was set
				if (Convert.ToBoolean(savedState[SSLEnabled]))
				{
					Process p;
					string filePath = savedState[installPath].ToString() + "Assignment Manager\\Setup";
					ProcessStartInfo si = new System.Diagnostics.ProcessStartInfo("\"" + filePath + "\\RemoveSSLFlag.vbs\"", "\"" + savedState[webName].ToString() + "\"");
					si.WindowStyle = ProcessWindowStyle.Hidden;
					try
					{
						p = Process.Start(si);
						p.WaitForExit();
					}
					catch (Exception e)
					{
						System.Diagnostics.EventLog.WriteEntry("AMInstall", "Process Exception", System.Diagnostics.EventLogEntryType.Information);
						throw new InstallException(e.Message);
					}

				}
				AMUser.DeleteUser();					//Also deletes the password stored in LSA				

				try
				{
					// if the Queue exists, then remove it.
					if (System.Messaging.MessageQueue.Exists(ACTION_QUEUE_PATH))
					{
						System.Messaging.MessageQueue.Delete(ACTION_QUEUE_PATH);
					}
				}
				catch( Exception )
				{
					// Eat the exception.  We should not fail if we cannot remove the queue.
				}
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.EventLog.WriteEntry("AMInstall", ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
			}

			// continue uninstall
			base.Uninstall(savedState);
		}

		public override void Rollback(IDictionary savedState)
		{
			// make sure the connection string isn't in the LSA
			LSAUtil lsa = new LSAUtil();
			try
			{
				lsa.DeletePrivateKey();
				AMUser.DeleteUser();
				// continue rollback
				base.Rollback(savedState);

			}
			catch (System.Exception ex)
			{
				System.Diagnostics.EventLog.WriteEntry("AMInstall", ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
			}

			try
			{
				// Remove the message queue, if it exists
				if (System.Messaging.MessageQueue.Exists(ACTION_QUEUE_PATH))
				{
					System.Messaging.MessageQueue.Delete(ACTION_QUEUE_PATH);
				}
			}
			catch( Exception )
			{
				// Eat the exception.  We should not fail if we cannot remove the queue.
			}
		}

		private bool setASPNETProcessInfinite() 
		{
			const string processModelNode = "/configuration/system.web/processModel";
			const string infinite = "Infinite";
			const string responseDeadlockInterval = "responseDeadlockInterval";

			try 
			{
				string sConfigFile = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + "Config\\machine.config";
				System.Xml.XmlNode node = null;
				System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();

				xmlDoc.PreserveWhitespace = true;
				xmlDoc.Load(sConfigFile);

				node = xmlDoc.SelectSingleNode(processModelNode);
				if (node == null) 
				{
					// The processModel attribute doesn't exist, which basically
					// means that they've either heavily modified their ASP.NET 
					// configuration or there is a very bad error with their system.
					return false;
				}

				// Check node value 
				System.Xml.XmlAttribute attrDeadlockInterval = node.Attributes[responseDeadlockInterval];
				if (attrDeadlockInterval != null) 
				{
					attrDeadlockInterval.InnerText = infinite;
				}
				else 
				{
					node.Attributes.Append(xmlDoc.CreateAttribute(String.Empty, responseDeadlockInterval, infinite));
				}

				// Overwrite the original file.
				xmlDoc.Save(sConfigFile);
			} 
			catch (System.Exception) 
			{
				// If an error occurred (such as the file being secured, etc.), just
				// return false. 
				return false;
			}

			return true;
		}

		private bool saveInstallState(System.Collections.IDictionary savedState, String[] individualArgs)
		{
			try
			{
				savedState.Add(webName, individualArgs[(int)InstallParams.TargetVDir]);
				savedState.Add(SSLEnabled, getBooleanValue(individualArgs[(int)InstallParams.SSLEnabled]));
				savedState.Add(installPath, individualArgs[(int)InstallParams.ProgramFilesFolder]);
			}
			catch
			{
				// any error occured return false
				return false;
			}
			return true;
		}

		private bool updateConfigFile(string configFile, String[] installArgs)
		{
			// update the application configuration file information
			XmlDocument doc = new XmlDocument();
			System.Xml.XmlNode configEntry;

			// load the web.config file
			doc.Load(configFile);

			// update the SSL value
			configEntry = doc.SelectSingleNode("configuration//appSettings/add[@key=\"usingSsl\"]");
			if (configEntry != null) 
			{
				configEntry.Attributes["value"].Value = getBooleanValue(installArgs[(int)InstallParams.SSLEnabled]);
			}

			// update the SMTP value
			configEntry = doc.SelectSingleNode("configuration//appSettings/add[@key=\"usingSmtp\"]");
			if (configEntry != null) 
			{
				configEntry.Attributes["value"].Value = getBooleanValue(installArgs[(int)InstallParams.SMTPEnabled]);
			}

			// update the <forms requireSSL=""> field
			// only will work in web.config
			configEntry = doc.SelectSingleNode("configuration/system.web/authentication/forms");
			if (configEntry != null)
			{
				configEntry.Attributes["requireSSL"].Value = getBooleanValue(installArgs[(int)InstallParams.SSLEnabled]);
			}

			doc.Save(configFile);
			return true;
		}

		private bool storeConnectString(string[] installArgs)
		{
			string connectString = buildConnectString(installArgs, false, true);
			LSAUtil lsa = new LSAUtil();
			try
			{
				lsa.StoreEncryptedString(connectString);
			}
			catch (System.Exception ex)
			{
				throw ex;
			}

			return true;
		}

		private string getBooleanValue(string installParam)
		{
			if (installParam.Equals(""))
			{
				return "false";
			}
			else
			{
				return "true";
			}
		}

		private bool createDB(string[] installArgs)
		{
			// create the AM Database
			System.Data.OleDb.OleDbConnection con = new System.Data.OleDb.OleDbConnection(buildConnectString(installArgs, true, true));
			System.Data.OleDb.OleDbConnection conAMDB = new System.Data.OleDb.OleDbConnection(buildConnectString(installArgs, false, true));
			System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand();
			string command;
			string pwd;
			string dbUserName;

			// generate a password for the AMUser account
			pwd = generateDBPwd();
			dbUserName = installArgs[(int)InstallParams.DBName] + "User";

			string dropCommand = "IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'" + installArgs[(int)InstallParams.DBName] + "')	DROP DATABASE [" + installArgs[(int)InstallParams.DBName] + "]";
			string createCommand = "CREATE DATABASE [" + installArgs[(int)InstallParams.DBName] + "] COLLATE SQL_Latin1_General_CP1_CI_AS";
			string dropUserCommand = "IF EXISTS (SELECT * from master.dbo.syslogins where loginname = N'" + dbUserName + "') EXEC sp_droplogin N'" + dbUserName + "'";
			string userCommand = "IF NOT EXISTS (SELECT * from master.dbo.syslogins where loginname = N'" + dbUserName + "')";
			userCommand += " BEGIN";
			userCommand += " declare @logindb nvarchar(132), @loginlang nvarchar(132) select @logindb = N'" + installArgs[(int)InstallParams.DBName] + "', @loginlang = N'us_english'";
			userCommand += " if @logindb is null or not exists (select * from master.dbo.sysdatabases where name = @logindb)";
			userCommand += " select @logindb = N'master'";
			userCommand += " if @loginlang is null or (not exists (select * from master.dbo.syslanguages where name = @loginlang) and @loginlang <> N'us_english')";
			userCommand += " select @loginlang = @@language";
			userCommand += " exec sp_addlogin N'" + dbUserName + "', N'" + pwd + "', @logindb, @loginlang";
			userCommand += " END";
			string grantCommand = "IF NOT EXISTS (select * from dbo.sysusers where name = N'" + dbUserName + "' and uid < 16382)";
			grantCommand += " EXEC sp_grantdbaccess N'"+ dbUserName + "', N'AMUser'";


			con.Open();

			// drop the database if it exists
			command = dropCommand;

			cmd.CommandText = command;
			cmd.Connection = con;
			cmd.ExecuteNonQuery();

			// create the new database
			command = createCommand;

			cmd.CommandText = command;
			cmd.Connection = con;
			cmd.ExecuteNonQuery();

			// drop the AMUser if it exists
			command = dropUserCommand;
			cmd.CommandText = command;
			cmd.Connection = con;
			cmd.ExecuteNonQuery();

			// close the connection to the master db
			con.Close();

			// open the connection to the new AMDB
			conAMDB.Open();
			// create the AMUser account
			cmd.CommandText = userCommand;
			cmd.Connection = conAMDB;
			cmd.ExecuteNonQuery();

			// grant AMUser access to the new db
			cmd.CommandText = grantCommand;
			cmd.Connection = conAMDB;
			cmd.ExecuteNonQuery();

			conAMDB.Close();

			// Add all of the data to the database. Note: This call must
			// be made before the installArgs are updated with different
			// username and passwords!
			if (!configureDB(installArgs)) 
			{
				System.Diagnostics.EventLog.WriteEntry("AMInstall", "Error occured while setting the initial tables and data into the database.", System.Diagnostics.EventLogEntryType.Error);
				return false;
			}

			// save the updated connection string
			installArgs[(int)InstallParams.DBUserID] = dbUserName;
			installArgs[(int)InstallParams.DBPassword] = pwd;
			if (!storeConnectString(installArgs))
			{
				System.Diagnostics.EventLog.WriteEntry("AMInstall", "Error occured while storing the database connectionstring.", System.Diagnostics.EventLogEntryType.Error);
				return false;
			}
			return true;
		}

		private string generateDBPwd()
		{
			string tempPwd = AMUser.CreatePassword(30); // Create a strong password of length 30
			string pwd = "";
			for (int i = 0; i < tempPwd.Length; i++)
			{
				// we need to skip characters that cause SQL problems when used in a pwd
				char charCode = tempPwd[i];
				// blocks: ';,`="
				if ((charCode == 39) || (charCode == 59) || (charCode == 44) || (charCode == 96) || (charCode == 61) || (charCode == 34))
				{
					continue;
				}
				else
				{
					pwd += tempPwd[i];
				}
			}
			return pwd;
		}

		private string buildConnectString(string[] installArgs, bool useMasterDB, bool specifyProvider)
		{
			// create the OLEDB connection string
			string connectString = "";

			if (specifyProvider) 
			{
				connectString += "Provider=SQLOLEDB.1;";
			}
			
			connectString += "User ID=" + installArgs[(int)InstallParams.DBUserID];
			connectString += ";Password=" + installArgs[(int)InstallParams.DBPassword];
			connectString += ";Persist Security Info=False;Initial Catalog=";
			if (!useMasterDB)
			{
				connectString += installArgs[(int)InstallParams.DBName];
			}
			else 
			{
				connectString += "master";
			}
			connectString += ";Data Source=" + installArgs[(int)InstallParams.DBServer] + ";";

			return connectString;
		}

		private bool configureDB(string[] installArgs)
		{
			// create the AM Database
			System.Data.SqlClient.SqlConnection conAMDB = new System.Data.SqlClient.SqlConnection(buildConnectString(installArgs, false, false));
			System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();

			try 
			{
				// Open connection to the server as the user provided in setup.
				conAMDB.Open();

				// Set up the tables.
				System.Text.RegularExpressions.Regex goRegExp = new System.Text.RegularExpressions.Regex("GO", System.Text.RegularExpressions.RegexOptions.Singleline);
				string queries = GetSql("sql.txt");
				string []individualQueries = goRegExp.Split(queries);

				foreach (string query in individualQueries) 
				{
					cmd.CommandText = query;
					cmd.Connection = conAMDB;
					cmd.ExecuteNonQuery();
				}

				// Set up the data.
				queries = GetSql("data.txt");
				individualQueries = goRegExp.Split(queries);

				foreach (string query in individualQueries) 
				{
					cmd.CommandText = query;
					cmd.Connection = conAMDB;
					cmd.ExecuteNonQuery();
				}
			} 
			catch (System.Exception) 
			{
				return false;
			}
			finally
			{
				conAMDB.Close();
			}

			return true;
		}

		private string GetSql(string Name) 
		{
			try 
			{
				// Get the current assembly.
				Assembly Asm = Assembly.GetExecutingAssembly();

				// Resources are named using a fully qualified name.
				Stream strm = Asm.GetManifestResourceStream(Asm.GetName().Name + "." + Name);

				// Read the contents of the embedded file.
				StreamReader reader = new StreamReader(strm);
				return reader.ReadToEnd();
			} 
			catch (System.Exception ex) 
			{
				throw ex;
			}
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Create a new Application Pool that will be run under the local System security context.
		/// </summary>
		private bool createApplicationPool(string appPoolName)
		{
			try
			{
				DirectoryEntry appPools = new DirectoryEntry("IIS://localhost/w3svc/AppPools");
				DirectoryEntry newAppPool = appPools.Children.Add(appPoolName, "IIsApplicationPool");
				newAppPool.Properties["WamUserName"][0] = "";
				newAppPool.Properties["WamUserPass"][0] = "";
				newAppPool.Properties["AppPoolIdentityType"][0] = 0;
				newAppPool.CommitChanges();
			}
			catch(Exception)
			{
				// Application Pool does not exist (i.e. OS < .NET Server)
				return false;
			}
			return true;
		}

		/// <summary>
		///	Creates the c:\data directory
		/// </summary>
		private bool CreateDataDirectory()
		{
			try
			{
				if(!Directory.Exists(DEFAULT_COURSE_OFFERINGS_ROOT_STORAGE_PATH))
				{
					Directory.CreateDirectory(DEFAULT_COURSE_OFFERINGS_ROOT_STORAGE_PATH);
				}
				SecurityPermissions.SetPathSecurityDescriptor(DEFAULT_COURSE_OFFERINGS_ROOT_STORAGE_PATH,
															  "D:(A;OICI;GA;;;BA)(A;OICI;GA;;;SY)" );
			}
			catch(Exception)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		///	Add virtual directory to an application pool
		/// </summary>
		private void addVDirToAppPool(string vdir, string appPoolName)
		{
			try
			{
				string webrootPath = "IIS://localhost/w3svc/1/root/";
				DirectoryEntry virtualDirectory = new DirectoryEntry(webrootPath + vdir);
				virtualDirectory.Properties["AppPoolId"][0] = appPoolName;
				virtualDirectory.CommitChanges();
			}
			catch(Exception)
			{
				System.Diagnostics.EventLog.WriteEntry("AMInstall", "Cannot add virtual directory to Application Pool", EventLogEntryType.Information);
			}
		}

		/// <summary>
		///	Change the ASP.NET process model to runas Local System
		/// </summary>
		private bool changeASPNETProcessModel()
		{
			try
			{
				String machineConfigPath = HttpRuntime.MachineConfigurationDirectory + "\\machine.config";
				System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
				xmlDoc.Load(machineConfigPath);
				System.Xml.XmlNode processModelNode = xmlDoc.SelectSingleNode(@"//configuration/system.web/processModel");
				processModelNode.Attributes["userName"].Value = "System";
				xmlDoc.Save(machineConfigPath);
			}
			catch(Exception)
			{
				return false;
			}
			return true;
		}
	}
}
