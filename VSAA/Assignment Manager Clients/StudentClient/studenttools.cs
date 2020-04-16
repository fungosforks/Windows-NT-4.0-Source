//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//
namespace StudentClient 
{
	using System;
	using System.Runtime.InteropServices;
	using Microsoft.Office;

	/// <summary>
	/// This object is set to the AddIn's .Object property so that any requests (from, say,
	/// JavaScript within the Start Page) will return this object and allow the user to
	/// call methods on it instead of the actual object that implements the AddIn interfaces.
	/// This separation is performed because the COM Interop layer only allows you to say
	/// either 'expose EVERY method through IDispatch, including .NET goop that you would never
	/// want to call from JScript' or 'just expose this interface'. Since some of the methods
	/// on the AddIn interface need to be exposed through IDispatch from two interfaces (but
	/// not System.Object!), this approach is used.
	/// </summary>
	// This attribute makes the runtime pick up IStudentCourseWork as its default interface 
	// for the class when registering through the COM interop layer.
	[System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.None)] 
	public class ClientTools : Object, IStudentCourseWork 
	{
		public ClientTools(EnvDTE._DTE dte) 
		{
			m_applicationObject = dte;
			m_storageTable = new System.Collections.Hashtable();
		}

		/// <summary>
		/// Public function that adds an entry for an Assignment Manager course to the 
		/// users's courses.xml file.
		/// </summary>
		/// <param name="courseName"> Short name of the course </param>
		/// <param name="url"> AM path to the course </param>
		/// <param name="guid"> Unique identifier for the course </param>
		/// <param name="xmlpath"> Path to the assignments.xml-compatible file </param>
		public bool RegisterAssignmentManagerCourse(string courseName, string url, string guid, string xmlpath) 
		{
			string strDestinationDirectory = "";
			string strCoursesFile = "";
			Microsoft.Win32.RegistryKey key = null;
			System.Xml.XmlDocument xmlDoc = null;
			System.Xml.XmlNode xmlNode = null;
			System.Xml.XmlElement xmlCourseElem = null;
			System.Xml.XmlElement xmlAssnMgrElem = null;

			System.Windows.Forms.Cursor old = System.Windows.Forms.Cursor.Current;

			try 
			{
				System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

				key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(s_strShellFolderHive);
				strDestinationDirectory = key.GetValue(s_strAppDataValueName) + s_strLocalTabPath;
				strCoursesFile = strDestinationDirectory + s_strCoursesFile;

				if (!System.IO.Directory.Exists(strDestinationDirectory)) 
				{
					System.IO.Directory.CreateDirectory(strDestinationDirectory);
				}

				xmlDoc = new System.Xml.XmlDocument();
				try 
				{
					xmlDoc.Load(strCoursesFile);
				} 
				catch (System.Exception) 
				{
					xmlDoc.LoadXml("<studentcourses></studentcourses>");
				}

				if ((xmlNode = xmlDoc.SelectSingleNode("/studentcourses/course[assnmgr/guid='" + EscapeStringForXPath(guid) + "']")) != null) 
				{
					// If it already exists, we're in an update and getting rid of the old data.
					xmlNode.ParentNode.RemoveChild(xmlNode);
				}

				// Create the course node and its name sub-element
				xmlCourseElem = xmlDoc.CreateElement("course");
				CreateCDATAElement(xmlDoc, xmlCourseElem, "name", courseName);

				// Create an populate the noassnmgr node, attaching it to the
				// course node
				xmlAssnMgrElem = xmlDoc.CreateElement("assnmgr");
				CreateCDATAElement(xmlDoc, xmlAssnMgrElem, "amurl", url);
				CreateCDATAElement(xmlDoc, xmlAssnMgrElem, "xmlpath", xmlpath);
				CreateCDATAElement(xmlDoc, xmlAssnMgrElem, "guid", guid);
				xmlCourseElem.AppendChild(xmlAssnMgrElem);
        
				// Add the completed course entry to the document
				xmlDoc.DocumentElement.AppendChild(xmlCourseElem);

				xmlDoc.PreserveWhitespace = true;
				xmlDoc.Save(strCoursesFile);
				RegenerateUserCustomTab(false);
        
				// Assignment manager courses cannot be browsed to without properly registering their URL
				// into the SafeDomains hive.
				SetupSafeDomain(url, true);
			} 
			catch (System.Exception) 
			{
				System.Windows.Forms.Cursor.Current = old;
				return false;
			}

			return true;
		}

		/// <summary>
		/// Public function that will remove an entry for an Assignment Manager course from
		/// the user's courses.xml file.
		/// </summary>
		/// <param name="guid"> The unique GUID denoting the course </param>
		public bool UnregisterAssignmentManagerCourse(string guid) 
		{
			Microsoft.Win32.RegistryKey key = null;
			string strDestinationDirectory = "";
			string strCoursesFile = "";
			string strURL = "";
			System.Xml.XmlDocument xmlDoc = null;
			System.Xml.XmlNode xmlNode = null, xmlPathNode = null;
			System.Xml.XmlNodeList xmlNodes = null;
			System.Windows.Forms.Cursor old = System.Windows.Forms.Cursor.Current;

			try 
			{
				System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

				key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(s_strShellFolderHive);
				strDestinationDirectory = key.GetValue(s_strAppDataValueName) + s_strLocalTabPath;
				strCoursesFile = strDestinationDirectory + s_strCoursesFile;

				xmlDoc = new System.Xml.XmlDocument();
				try 
				{
					xmlDoc.Load(strCoursesFile);
				} 
				catch (System.Exception) 
				{
					return false;
				}

				if ((xmlNode = xmlDoc.SelectSingleNode("/studentcourses/course[assnmgr/guid='" + EscapeStringForXPath(guid) + "']")) != null) 
				{
					if (((xmlPathNode = xmlDoc.SelectSingleNode("/studentcourses/course/assnmgr[guid='" + EscapeStringForXPath(guid) + "']")) != null) &&
						((xmlPathNode = xmlPathNode.SelectSingleNode("amurl")) != null)) 
					{
						strURL = xmlPathNode.InnerText;
					}
					xmlNode.ParentNode.RemoveChild(xmlNode);
					xmlDoc.PreserveWhitespace = true;
					xmlDoc.Save(strCoursesFile);
					if (((xmlNodes = xmlDoc.SelectNodes("/studentcourses/course")) == null) ||
						(xmlNodes.Count == 0)) 
					{
						RegenerateUserCustomTab(true);
					} 
					else 
					{
						RegenerateUserCustomTab(false);
					}
				}
			} 
			catch (System.Exception) 
			{
				System.Windows.Forms.Cursor.Current = old;
				return false;
			}

			System.Windows.Forms.Cursor.Current = old;
			return true;
		}

		/// <summary>
		/// Given a URL, this will either register or unregister a safe domain with the shell. It
		/// also sets the flag telling the shell to re-read the safedomains key.
		/// </summary>
		/// <param name="url"> String denoting the path to add to the safe domain.</param>
		/// <param name="register"> Boolean flag whether to register or unregister (true == register). </param>
		private void SetupSafeDomain(string url, bool register) 
		{
			Microsoft.Win32.RegistryKey key = null, newKey = null;
			string strRegistryRoot = "";

			try 
			{
				strRegistryRoot = m_applicationObject.RegistryRoot;
				key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(strRegistryRoot + s_strVSProtocolSafeHive);
				// Inform the shell that the set of safe domains has changed
				key.SetValue(s_strSafeDomainChangedValueName, 1);

				if (register) 
				{
					newKey = key.CreateSubKey(url);
				} 
				else 
				{
					key.DeleteSubKey(url, false);
				}
			} 
			catch (System.Exception) 
			{
				// Ignore exceptions, as they mean that permission error occurred creating / deleting a key, which
				// is a terminal error for VS anyways (you couldn't use the shell successfully!).
			} 
			finally 
			{
				if (key != null) 
				{
					key.Close();
				}
				if (newKey != null) 
				{
					newKey.Close();
				}
			}
		}

		/// <summary>
		/// Given an xml document, an element in that document, and a name/value pair, it will create a child element,
		/// properly CDATA-escaped, with the given name and value. Note that the name must be a valid XML tag name.
		/// </summary>
		private void CreateCDATAElement(System.Xml.XmlDocument xmlDoc, System.Xml.XmlElement parent, string elemName, string elemValue) 
		{
			System.Xml.XmlElement xmlTempElem = null;
			System.Xml.XmlNode xmlTempNode = null;

			// CONSIDER: According to docs, setting InnerText properties has changed to now do proper escaping of the
			// value. Encompassing every created item in a CDATA element may no longer be necessary through the framework.
			xmlTempElem = xmlDoc.CreateElement(elemName);
			xmlTempNode = xmlDoc.CreateNode(System.Xml.XmlNodeType.CDATA, "", "");
			xmlTempNode.InnerText = elemValue;
			xmlTempElem.AppendChild(xmlTempNode);
			parent.AppendChild(xmlTempElem);
		}

    
		/// <summary>
		/// The custom tab that displays the 'Add Course', 'Delete Course', etc. items also
		/// has an optional FEED tag, which will cause it to try to pick up an updated
		/// copy, if available, from the user's APPDATA\Microsoft\VisualStudio\7.1\Academic
		/// folder. This function updates that file, and should be called whenever either
		/// script or addin code update the list of courses on disk.
		/// </summary>
		public bool RegenerateUserCustomTab(bool noCoursesRemaining) 
		{
			try 
			{
				System.Xml.Xsl.XslTransform transformer;
				System.Xml.XmlTextReader xslReader;
				Microsoft.Win32.RegistryKey key;
				string coursesXMLFile, userTabFile, strDestinationDirectory, registryRoot, emptyTabFile;

				// Create a cached copy of assignments.xml and put it in the right spot
				key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(s_strShellFolderHive);
				strDestinationDirectory = key.GetValue(s_strAppDataValueName) + s_strLocalTabPath;

				coursesXMLFile = strDestinationDirectory + s_strCoursesFile;
				userTabFile = strDestinationDirectory + s_strUserTabFile;

				if (noCoursesRemaining) 
				{
					// Copy the default tab.
					registryRoot = m_applicationObject.RegistryRoot;
					key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryRoot);
					emptyTabFile = key.GetValue(s_strInstallationDirValueName) + s_strVisualStudioHTMLDir;
					emptyTabFile += m_applicationObject.LocaleID.ToString() + "\\";
					emptyTabFile += s_strVisualStudioMyCoursesTabDir;
          
					if (System.IO.File.Exists(userTabFile)) 
					{
						System.IO.File.Delete(userTabFile);
					}

					System.IO.File.Copy(emptyTabFile, userTabFile);
				} 
				else 
				{
					// Apply an XSL transform to the courses.xml file.
					xslReader = new System.Xml.XmlTextReader(AssignmentManager.ClientUI.AMResources.GetLocalizedString("XSLStudentCustomTab"), System.Xml.XmlNodeType.Document, null);
          
					transformer = new System.Xml.Xsl.XslTransform();
					transformer.Load(xslReader);
          
					if (System.IO.File.Exists(userTabFile)) 
					{
						System.IO.File.Delete(userTabFile);
					}

					transformer.Transform(coursesXMLFile, userTabFile);
				}
			} 
			catch (System.Exception) 
			{
				return false;
			}

			return true;
		}
    
		/// <summary>
		/// XPath queries require that the strings passed in for comparison are properly
		/// formed. That is, the following characters must be escaped:
		/// ' "
		/// </summary>
		public string EscapeStringForXPath(string s) 
		{
			char current;
			string strFinal = "";
			int i, nLength;

			nLength = s.Length;
			for (i = 0; i < nLength; i++) 
			{
				current = s[i];
				if ((current == '\'') || (current == '\"')) 
				{
					strFinal += '\\';
				}
				strFinal += current;
			}

			return strFinal;
		}
    
		/// <summary>
		/// In order to make sure that all of the slashes are in the
		/// same direction, this function makes the FTP & HTTP slashes
		/// go forward, and all others (assuming UNC or local path) go
		/// backwards.
		/// </summary>
		internal string NormalizeURL(string url) 
		{
			string upper = url.ToUpper();

			if (upper.StartsWith("HTTP") || upper.StartsWith("FTP")) 
			{
				return url.Replace('\\', '/');
			}

			return url.Replace('/', '\\');
		}

		public bool CheckVersion(string url)
		{
			try
			{
				System.Xml.XmlDocument doc = null;
				System.Net.WebRequest wr = System.Net.WebRequest.Create(url);
				System.Net.HttpWebResponse response = null;
				try 
				{
					response = (System.Net.HttpWebResponse)wr.GetResponse();
				} 
				catch (System.Net.WebException) 
				{
					// Could not resolve server name.
					string message = AssignmentManager.ClientUI.AMResources.GetLocalizedString("AddCourse_ErrorWrongServerName");
					string caption = AssignmentManager.ClientUI.AMResources.GetLocalizedString("AddCourse_ErrorWrongServerNameCaption");
					message = message.Replace("%1", url);
					System.Windows.Forms.MessageBox.Show(message,caption,System.Windows.Forms.MessageBoxButtons.OK,
						System.Windows.Forms.MessageBoxIcon.Warning);
					return false;
				}

				switch (response.StatusCode) 
				{
					case System.Net.HttpStatusCode.OK:
						System.IO.Stream file = response.GetResponseStream();
						System.IO.StreamReader fileReader = new System.IO.StreamReader(file);
          
						doc = new System.Xml.XmlDocument();
						doc.Load(fileReader);
						break;
					case System.Net.HttpStatusCode.NotFound:
						throw new System.IO.FileNotFoundException();
					case System.Net.HttpStatusCode.Unauthorized:
						throw new System.IO.FileNotFoundException();
					default:
						throw new Exception();
				}
				
				System.Xml.XmlNode node = doc.SelectSingleNode("/AssignmentManager/version");

				double version = Convert.ToDouble(node.InnerText, System.Globalization.CultureInfo.InvariantCulture);
				if (version >= 7.1)
				{
					return true;	
				}
				else
				{
					throw new Exception();
				}
			}
			catch( System.IO.FileNotFoundException )
			{
				// Unable to connect to server.
				string message = AssignmentManager.ClientUI.AMResources.GetLocalizedString("AddCourse_UnableToConnectToServer");
				string caption = AssignmentManager.ClientUI.AMResources.GetLocalizedString("AddCourse_DialogCaption");
				System.Windows.Forms.MessageBox.Show(message,caption,System.Windows.Forms.MessageBoxButtons.OK,
					System.Windows.Forms.MessageBoxIcon.Warning);
				return false;				
			}
			catch(Exception)
			{
				string message = AssignmentManager.ClientUI.AMResources.GetLocalizedString("AddCourse_WrongVersion");
				string caption = AssignmentManager.ClientUI.AMResources.GetLocalizedString("AddCourse_WrongVersionCaption");
				System.Windows.Forms.MessageBox.Show(message,caption,System.Windows.Forms.MessageBoxButtons.OK,
					System.Windows.Forms.MessageBoxIcon.Warning);
				return false;
			}
		}

		public System.Xml.XmlDocument GetCourseFile(string url)
		{

			try
			{
				System.Xml.XmlDocument doc = null;
				int nIndex = 0;
				string strServer;

				if (url.StartsWith("http://"))
				{
					strServer = url.Substring(7);
				}
				else if (url.StartsWith("https://"))
				{
					strServer = url.Substring(8);
				}
				else
				{
					strServer = url;
				}
				// Determine the first fore or back slash, other than those in the string "http://",
				// in order to trim to that point and retrieve the server name.
				nIndex = strServer.IndexOf('/');
				if (nIndex == -1) 
				{
					throw new System.ArgumentException();
				}
				strServer = strServer.Substring(0, nIndex);

				System.Net.WebRequest wr = System.Net.WebRequest.Create(url);
				System.Net.HttpWebResponse response = null;
				try 
				{
					response = (System.Net.HttpWebResponse)wr.GetResponse();
				} 
				catch (System.Net.WebException) 
				{
					// Translate NameResolution and ProtocolErrors into a file not found
					// exception for easier handling by the higher-level infrastructure.
					//				throw new ServerNotFoundException();
				}

				switch (response.StatusCode) 
				{
					case System.Net.HttpStatusCode.OK:
						System.IO.Stream file = response.GetResponseStream();
						System.IO.StreamReader fileReader = new System.IO.StreamReader(file);
          
						doc = new System.Xml.XmlDocument();
						doc.Load(fileReader);
						break;
					case System.Net.HttpStatusCode.NotFound:
						//					throw new FileNotFoundException();
					case System.Net.HttpStatusCode.Unauthorized:
						//					throw new SecurityException();
					default:
						return null;
				}
				return doc;	
			}
			catch
			{
				// Cannot load remote file.
				return null;
			}
		}
    
		internal System.Xml.XmlDocument LocalCoursesFile
		{
			get
			{
				string strDestinationDirectory = "";
				string strCoursesFile = "";
				Microsoft.Win32.RegistryKey key = null;
				System.Xml.XmlDocument xmlDoc = null;

				System.Windows.Forms.Cursor old = System.Windows.Forms.Cursor.Current;

				System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

				key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(s_strShellFolderHive);
				strDestinationDirectory = key.GetValue(s_strAppDataValueName) + s_strLocalTabPath;
				strCoursesFile = strDestinationDirectory + s_strCoursesFile;

				if (!System.IO.Directory.Exists(strDestinationDirectory)) 
				{
					System.IO.Directory.CreateDirectory(strDestinationDirectory);
				}

				xmlDoc = new System.Xml.XmlDocument();
				try 
				{
					xmlDoc.Load(strCoursesFile);
				} 
				catch (System.Exception) 
				{
					xmlDoc.LoadXml("<studentcourses></studentcourses>");
				}
				return xmlDoc;
			}
		}


		[DllImport("ole32.dll")]
		private static extern int StgCreateDocfile([MarshalAs(UnmanagedType.LPWStr)]string name, System.UInt32 mode, System.UInt32 reserved, [MarshalAs(UnmanagedType.Interface)]ref IStorage Storge);
		[System.Runtime.InteropServices.DllImport("ole32.dll")]
		private static extern int StgOpenStorage([MarshalAs(UnmanagedType.LPWStr)]string name, System.IntPtr priority, System.UInt32 mode,
			System.IntPtr exclude, System.UInt32 reserved, ref object Storge);
		private const System.UInt32 STGM_READ = 0x00000000;
		private const System.UInt32 STGM_READWRITE = 0x00000002;
		private const System.UInt32 STGM_SHARE_EXCLUSIVE = 0x00000010;
		private const System.UInt32 STGM_DIRECT = 0x00000000;
		private const System.UInt32 STGM_TRANSACTED = 0x00010000;
		private const System.UInt32 STGM_CONVERT = 0x00020000;
		private const System.UInt32 STGM_S_CONVERTED = 0x00030200;
		private const System.UInt32 S_OK = 0x00000000;

		/// <summary>
		/// The XMLHttp control requires a file stream to upload; this method will
		/// provide it with one, without all of the cross-domain restrictions of
		/// that provided by the ADO object's. 
		/// </summary>
		public object GetFileReadStream(string localFilePath) 
		{
			int retval;
			IStorage storage = null;
			object fileStream = null;

			try 
			{
				// Open the file, converting it into a COM Storage file, but opening transacted so that
				// we need never 'commit' the changes to disk. 
				retval = StgCreateDocfile(localFilePath, STGM_CONVERT | STGM_TRANSACTED | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,
					0, ref storage);
        
				if (retval == STGM_S_CONVERTED) 
				{
					// Now, get the stream to read from on the file.
					retval = storage.OpenStream("CONTENTS", (System.IntPtr)0, STGM_READ | STGM_SHARE_EXCLUSIVE | STGM_DIRECT,
						0, out fileStream);
					if (retval == S_OK) 
					{
						m_storageTable.Add(fileStream, storage);
						return fileStream;
					}
				}
			} 
			catch (System.Exception) 
			{
			}
			return null;
		}

		/// <summary>
		/// Since the storage cannot be disposed of until all of the streams are
		/// done being used, the caller must tell us when they are completed with
		/// the stream.
		/// </summary>
		public void DisposeFileReadStream(object stream) 
		{
			try 
			{
				IStorage storage = null;
        
				if (stream == null) 
				{
					return;
				}

				if (m_storageTable.Contains(stream)) 
				{
					storage = (IStorage)m_storageTable[stream];
          
					if (storage != null) 
					{
						m_storageTable.Remove(stream);
						System.Runtime.InteropServices.Marshal.ReleaseComObject(storage);
					}
				}
			} 
			catch (System.Exception) 
			{
				// Fall through and return no exception, as it probably means that
				// the user already closed the stream themselves (by calling .Release())
				// but we still had an entry in our table. 
			}
		}

		// This interface is a simple copy of the ISequentialStream interface provided
		// by COM. Since we will only ever be calling Read, that's the only
		// method that is actually correctly prototyped. Note that members of the
		// interface are still here as placeholders (and so that VTable order is
		// correct), but if they are to be called, all of the paramaters would
		// need to be correctly imported from the original IDL.
		[Guid("0c733a30-2a1c-11ce-ade5-00aa0044773d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			interface ISequentialStream 
		{
			//    HRESULT Read([out, size_is(cb), length_is(*pcbRead)] void *pv, [in] ULONG cb, [out] ULONG *pcbRead);
			[PreserveSig]
			int Read(
				[MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.U1, SizeParamIndex=1)] byte[] data,
				System.UInt32 numBytesAvailable,
				out System.UInt32 numBytesRead);
			void Write();
		}

		/// <summary>
		/// The XMLHttp control only gives back a file stream to save; this method
		/// save out that stream to disk, without all of the cross-domain restrictions
		/// of that provided by the ADO object's. 
		/// </summary>
		public bool SaveFileStream(object fileStream, string localFilePath) 
		{
			byte []data;
			System.UInt32 bytesAvailable = 4096, bytesRead = 0;
			System.IO.FileStream writeStream = null;
			ISequentialStream readStream = null;

			try 
			{
				if (System.IO.File.Exists(localFilePath)) 
				{
					// If this throws an exception (due to permissons or file sharing),
					// we catch and return false.
					System.IO.File.Delete(localFilePath);
				}

				writeStream = System.IO.File.Create(localFilePath);
				readStream = (ISequentialStream)fileStream;

				data = new byte[bytesAvailable];

				// ISequentialStream implementors are requested -- but not required --
				// to return S_OK and nRead == 0 on end-of-stream. Therefore, we
				// ignore the return value from .Read (since it's basically meaningless)
				// and simply check the value returned from the call. If it comes back
				// zero, we either reached the end of the stream or the caller errored
				// out and we didn't do anything.
				bytesRead = 0;
				readStream.Read(data, bytesAvailable, out bytesRead);
				while (bytesRead > 0) 
				{
					writeStream.Write(data, 0, (int)bytesRead);
          
					bytesRead = 0;
					readStream.Read(data, bytesAvailable, out bytesRead);
				}

				writeStream.Close();
			} 
			catch (System.Exception) 
			{
				if (writeStream != null) 
				{
					writeStream.Close();

					// Since the file started to be created, attempt to clean up
					// after the garbage that may have been left around if the
					// handed-in stream generated an exception.
					if (System.IO.File.Exists(localFilePath)) 
					{
						try 
						{
							System.IO.File.Delete(localFilePath);
						} 
						catch (System.Exception) 
						{
						}
					}
				}
        
				return false;
			}

			return true;
		}

		private EnvDTE._DTE m_applicationObject;
		private System.Collections.Hashtable m_storageTable;
		private static string s_strCoursesFile = "courses.xml";
		private static string s_strUserTabFile = "AMStudentStartPage.xml";
		private static string s_strShellFolderHive = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders";
		private static string s_strVSProtocolSafeHive = "\\VSProtocol\\SafeDomains";
		private static string s_strAppDataValueName = "AppData";
		private static string s_strLocalTabPath = "\\Microsoft\\VisualStudio\\7.1\\Academic\\";
		private static string s_strVisualStudioHTMLDir = "HTML\\";
		private static string s_strVisualStudioMyCoursesTabDir = "Custom\\mycourses.xml";
		private static string s_strSafeDomainChangedValueName = "SafeDomainsChanged";
		private static string s_strInstallationDirValueName = "InstallDir";
	}
}
