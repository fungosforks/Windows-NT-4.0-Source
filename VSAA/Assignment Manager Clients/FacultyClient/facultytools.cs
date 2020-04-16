//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//
using System;
using Microsoft.Office;
using StudentClient;
namespace FacultyClient 
{
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
	// This attribute makes the runtime pick up ICourseManagement as its default interface 
	// for the class when registering through the COM interop layer.
	[System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.None)] 
	public class FacultyTools : Object, ICourseManagement  
	{
		public FacultyTools(EnvDTE._DTE dte) 
		{
			m_applicationObject = dte;
			m_studentTools = new StudentClient.ClientTools(dte);
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
				key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Constants.KeyName);
				strDestinationDirectory = key.GetValue(Constants.ValueName) + Constants.ApplicationPath;

				coursesXMLFile = strDestinationDirectory + Constants.ManagedCoursesFileName;
				userTabFile = strDestinationDirectory + Constants.UserTabFile;

				if (noCoursesRemaining) 
				{
					// Copy the default tab.
					registryRoot = m_applicationObject.RegistryRoot;
					key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryRoot);
					emptyTabFile = key.GetValue(Constants.InstallationDirValueName) + Constants.VisualStudioHTMLDir;
					emptyTabFile += m_applicationObject.LocaleID.ToString() + "\\";
					emptyTabFile += Constants.VisualStudioCourseManagementTabDir;
          
					if (System.IO.File.Exists(userTabFile)) 
					{
						System.IO.File.Delete(userTabFile);
					}

					System.IO.File.Copy(emptyTabFile, userTabFile);
				} 
				else 
				{
					// Apply an XSL transform to the courses.xml file.
					xslReader = new System.Xml.XmlTextReader(AssignmentManager.ClientUI.AMResources.GetLocalizedString("XSLCustomTab"), System.Xml.XmlNodeType.Document, null);
          
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
		/// Copies a project currently open in the environment to a new location,
		/// optionally extracting the code that has been marked.
		/// </summary>
		public bool CreateNewProject(string sourceUniqueProjectName, string path,
			string destProjectName, bool performExtraction) 
		{
			try 
			{
				CodeExtractor e = new CodeExtractor(m_applicationObject);
				return e.CreateNewProject(sourceUniqueProjectName, path,
					destProjectName, performExtraction);
			} 
			catch (System.Exception) 
			{
				return false;
			}
		}
    
		/// <summary>
		/// Makes the required entries in managedcourses.xml so that the AM course will
		/// appear in the Start Pages associated with Course Management.
		/// </summary>
		public bool RegisterAssignmentManagerCourse(string courseName, string url, string guid) 
		{
			System.Windows.Forms.Cursor old = System.Windows.Forms.Cursor.Current;
			bool result;
	
			try 
			{
				System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
				url = NormalizeURL(url);
				result = Utilities.RegisterAssignmentManagerCourse(m_applicationObject, courseName, url, guid);
				RegenerateUserCustomTab(false);
        
				return result;
			} 
			catch (System.Exception) 
			{
				return false;
			} 
			finally 
			{
				System.Windows.Forms.Cursor.Current = old;
			}
		}

		/// <summary>
		/// Given a URL to a course, this function will attempt to access it,
		/// register it in the proper way, and then return the final URL used
		/// to access it. It will return NULL on failure. The URL can be 
		/// different from that passed in because this will attempt to append
		/// '/assignments.xml', if it is unable to get the first url presented
		/// and it doesn't already end in that.
		/// </summary>
		public string RegisterCourseFromUrl(string url) 
		{	
			try 
			{
				string strFinalURL = url;
				System.Xml.XmlDocument xmlDoc = null;
				System.Xml.XmlNode xmlNode = null;
				string strCourseName = null;
				string strCourseGUID = null;

				try 
				{
					xmlDoc = m_studentTools.GetCourseFile(strFinalURL);
				} 
				catch (System.Exception) 
				{
					// Do nothing -- xmlDoc will be NULL anyways.
				}

				if (xmlDoc != null) 
				{
					if ((xmlNode = xmlDoc.SelectSingleNode("/course/name")) != null) 
					{
						strCourseName = xmlNode.InnerText;
					}
					if (xmlDoc.SelectSingleNode("/course/assnmgr") != null) 
					{
						if ((xmlNode = xmlDoc.SelectSingleNode("/course/assnmgr/guid")) != null) 
						{
							strCourseGUID = xmlNode.InnerText;
						}
						if ((xmlNode = xmlDoc.SelectSingleNode("/course/assnmgr/amurl")) != null) 
						{
							strFinalURL = xmlNode.InnerText;
						}
            
						strFinalURL = NormalizeURL(strFinalURL);
						Utilities.RegisterAssignmentManagerCourse(m_applicationObject, strCourseName, strFinalURL, strCourseGUID);
					}
					RegenerateUserCustomTab(false);
					return strFinalURL + "/Faculty/WorkWithCourse.aspx?CourseID=" + strCourseGUID;;
				}         
				return null;
			} 
			catch (System.Exception) 
			{
				return null;
			}
		}

		/// <summary>
		/// Removes the entries from managedcourses.xml so that the AM course will
		/// no longer appear in the Start Pages associated with Course Management.
		/// </summary>
		public bool UnregisterAssignmentManagerCourse(string guid) 
		{
			System.Windows.Forms.Cursor old = System.Windows.Forms.Cursor.Current;
			bool fLastCourse;
	
			try 
			{
				System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
				fLastCourse = Utilities.UnregisterAssignmentManagerCourse(m_applicationObject, guid);
				RegenerateUserCustomTab(fLastCourse);
				return true;
			} 
			catch (System.Exception) 
			{
				return false;
			} 
			finally 
			{
				System.Windows.Forms.Cursor.Current = old;
			}
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

		/// <summary>
		/// Helper function for CreateFileList() to add entries to the stream for
		/// each of the nested directories and their files. Note that all entries
		/// are relative from the location of assignments.xml.
		/// </summary>
		private static  bool CreateFileListNestedEntries(string strRootOffset, System.IO.DirectoryInfo dir, System.IO.StreamWriter writer) 
		{
			try 
			{
				bool fSucceeded = true;
				string strRelativePath = strRootOffset + dir.Name + "\\";
	  
				foreach (System.IO.FileInfo f in dir.GetFiles()) 
				{
					writer.WriteLine(strRelativePath + f.Name);
				}

				foreach (System.IO.DirectoryInfo d in dir.GetDirectories()) 
				{
					fSucceeded &= CreateFileListNestedEntries(strRelativePath, d, writer);
				}

				return fSucceeded;
			} 
			catch (System.Exception) 
			{
				return false;
			}
		}

		/// <summary>
		/// This handles the low-level copying of files from a project into a destination path, flattening
		/// the file structure.
		/// </summary>
		private static bool CopyProjectFiles(EnvDTE.Project proj, string destPath, string listingFile) 
		{
			System.IO.StreamWriter writer = null;
			EnvDTE.ProjectItems Items = proj.ProjectItems;
			EnvDTE.ProjectItem Item = null;
			int i, j, nItemCount, nFileCount;
			string strFileName = null;
			string strFilePath = null;
			bool fSucceeded = true;

			try 
			{
				// Open for append
				writer = new System.IO.StreamWriter(listingFile, true);
	    
				nItemCount = Items.Count;
				for (i = 1; i <= nItemCount; i++) 
				{
					Item = Items.Item(i);
					nFileCount = Item.FileCount;
	  
					for (j = 1; j <= nFileCount; j++) 
					{
						strFilePath = Item.get_FileNames((short)j);
	    
						// NOTE: The item kind can be one of physical file, misc item, 
						// or solution item. However, since we're explicitly *in* the 
						// misc or solution items folders if we're here, it makes more 
						// sense just to copy any & all files that we find in the folder
						// and are able to locate on disk.
						// The only exception is HTTP-based URLs, which we explicitly
						// pick out and add to the listing file, which is deployed
						// with all submitted assignments.
						if ((new System.IO.FileInfo(strFilePath)).Exists) 
						{
							strFileName = strFilePath.Substring(System.Math.Max(strFilePath.LastIndexOf('/'), strFilePath.LastIndexOf('\\')));
							System.IO.File.Copy(strFilePath, destPath + strFileName);
							writer.WriteLine(strFileName);
						} 
						else 
						{
							if (strFilePath.ToUpper().StartsWith("HTTP://")) 
							{
								writer.WriteLine(strFilePath);
							}
	      
						}
					}
				}
			} 
			catch (System.Exception) 
			{
				fSucceeded = false;
			} 
			finally 
			{
				if (writer != null) 
				{
					writer.Close();
				}
			}
      
			return fSucceeded;
		}

		/// <summary>
		/// The XMLHttp control requires a file stream to upload; this method will
		/// provide it with one, without all of the cross-domain restrictions of
		/// that provided by the ADO object's. 
		/// </summary>
		public object GetFileReadStream(string localFilePath) 
		{
			return m_studentTools.GetFileReadStream(localFilePath);
		}

		/// <summary>
		/// Since the storage cannot be disposed of until all of the streams are
		/// done being used, the caller must tell us when they are completed with
		/// the stream.
		/// </summary>
		public void DisposeFileReadStream(object stream) 
		{
			m_studentTools.DisposeFileReadStream(stream);
		}

		/// <summary>
		/// The XMLHttp control only gives back a file stream to save; this method
		/// save out that stream to disk, without all of the cross-domain restrictions
		/// of that provided by the ADO object's. 
		/// </summary>
		public bool SaveFileStream(object fileStream, string localFilePath) 
		{
			return m_studentTools.SaveFileStream(fileStream, localFilePath);
		}

		public bool CheckVersion(string url)
		{
			return m_studentTools.CheckVersion(url);
		}

		private EnvDTE._DTE m_applicationObject;
		private StudentClient.ClientTools m_studentTools;
	}
}
