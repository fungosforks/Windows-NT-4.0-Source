//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//
namespace FacultyClient {
  using System;
  using Microsoft.Office;

  internal class Utilities : Object {
    /// <summary>
    /// This class is not creatable, as it provides all of its functionality through
    /// static functions and member variables.
    /// </summary>
    private Utilities() {}

    /// <summary>
    /// XPath queries require that the strings passed in for comparison are properly
    /// formed. That is, the following characters must be escaped:
    /// ' "
    /// </summary>
    public static string EscapeStringForXPath(string s) {
      char current;
      string sFinal = "";
      int i, nLength;

      nLength = s.Length;
      for (i = 0; i < nLength; i++) {
        current = s[i];
        if ((current == '\'') || (current == '\"')) {
          sFinal += '\\';
        }
        sFinal += current;
      }

      return sFinal;
    }
    
    /// <summary>
    /// Given an xml document, an element in that document, and a name/value pair, it will create a child element,
    /// properly CDATA-escaped, with the given name and value. Note that the name must be a valid XML tag name.
    /// </summary>
    public static void CreateCDATAElement(System.Xml.XmlDocument xmlDoc, System.Xml.XmlElement parent, string elemName, string elemValue) {
      System.Xml.XmlElement xmlTempElem = null;
      System.Xml.XmlNode xmlTempNode = null;

      xmlTempElem = xmlDoc.CreateElement(elemName);
      xmlTempNode = xmlDoc.CreateNode(System.Xml.XmlNodeType.CDATA, "", "");
      xmlTempNode.InnerText = elemValue;
      xmlTempElem.AppendChild(xmlTempNode);
      parent.AppendChild(xmlTempElem);
    }

    /// <summary>
    /// Adds the necessary entries to managedcourses.xml so that the assignment manager-based
    /// course will correctly appear in the environment and the links to 'work with' the course
    /// will point at the correct server.
    /// </summary>
    public static bool RegisterAssignmentManagerCourse(EnvDTE._DTE dte, string courseName, string url, string guid) {
      Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Constants.KeyName);
      string sAppDataPath = (string)key.GetValue(Constants.ValueName);
      string sFolderName = sAppDataPath + Constants.ApplicationPath;
      string sManagedFile = sFolderName + Constants.ManagedCoursesFileName;
      System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(sFolderName);
      System.Xml.XmlDocument xmlDoc = null;
      System.Xml.XmlNode xmlNode = null;
      System.Xml.XmlElement xmlCourseElem = null;
      System.Xml.XmlElement xmlAssnMgrElem = null;

      if (!di.Exists) {
        System.IO.Directory.CreateDirectory(sFolderName);
      }

      xmlDoc = new System.Xml.XmlDocument();
      try {
        xmlDoc.Load(sManagedFile);
      } catch (System.Exception) {
        xmlDoc.LoadXml("<managedcourses></managedcourses>");
      }

      if ((xmlNode = xmlDoc.SelectSingleNode("/managedcourses/course[assnmgr/guid='" + EscapeStringForXPath(guid) + "']")) != null) {
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
      CreateCDATAElement(xmlDoc, xmlAssnMgrElem, "guid", guid);
      xmlCourseElem.AppendChild(xmlAssnMgrElem);

      // Finally, add the completed course entry to the document
      xmlDoc.DocumentElement.AppendChild(xmlCourseElem);

      xmlDoc.PreserveWhitespace = true;
      xmlDoc.Save(sManagedFile);

      // Assignment manager courses cannot be browsed to without properly registering their URL
      // into the SafeDomains hive.
      SetupSafeDomain(dte, url, true);
      
      return true;
    }
    
    /// <summary>
    /// Removes all of the entries from managedcourses.xml for the given
    /// assignment manager course.
    /// </summary>
    public static bool UnregisterAssignmentManagerCourse(EnvDTE._DTE dte, string guid) {
      Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Constants.KeyName);
      string sAppDataPath = (string)key.GetValue(Constants.ValueName);
      string sFolderName = sAppDataPath + Constants.ApplicationPath;
      string sManagedFile = sFolderName + Constants.ManagedCoursesFileName;
      string strURL = "";
      System.Xml.XmlDocument xmlDoc = null;
      System.Xml.XmlNode xmlNode = null, xmlPathNode = null;
      System.Xml.XmlNodeList xmlNodes = null;

      xmlDoc = new System.Xml.XmlDocument();
      xmlDoc.Load(sManagedFile);

      if ((xmlNode = xmlDoc.SelectSingleNode("/managedcourses/course[assnmgr/guid='" + EscapeStringForXPath(guid) + "']")) != null) {
        if (((xmlPathNode = xmlDoc.SelectSingleNode("/managedcourses/course/assnmgr[guid='" + EscapeStringForXPath(guid) + "']")) != null) &&
            ((xmlPathNode = xmlPathNode.SelectSingleNode("amurl")) != null)) {
          strURL = xmlPathNode.InnerText;
        }
        
        xmlNode.ParentNode.RemoveChild(xmlNode);
        xmlDoc.PreserveWhitespace = true;
        xmlDoc.Save(sManagedFile);
      }

      // Return true if there are now no more courses.
      return (((xmlNodes = xmlDoc.SelectNodes("/managedcourses/course")) == null) || (xmlNodes.Count == 0));
    }

    /// <summary>
    /// Given a URL, this will either register or unregister a safe domain with the shell. It
    /// also sets the flag telling the shell to re-read the safedomains key.
    /// </summary>
    /// <param name="url"> String denoting the path to add to the safe domain.</param>
    /// <param name="register"> Boolean flag whether to register or unregister (true == register). </param>
    public static void SetupSafeDomain(EnvDTE._DTE dte, string url, bool register) {
      Microsoft.Win32.RegistryKey key = null, newKey = null;
      string strRegistryRoot = "";

      try {
        strRegistryRoot = dte.RegistryRoot;
        key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(strRegistryRoot + Constants.VSProtocolSafeHive);
        // Inform the shell that the set of safe domains has changed
        key.SetValue(Constants.SafeDomainChangedValueName, 1);

        if (register) {
          newKey = key.CreateSubKey(url);
        } else {
          key.DeleteSubKey(url, false);
        }
      } catch (System.Exception) {
        // Ignore exceptions, as they mean that permission error occurred creating / deleting a key, which
        // is a terminal error for VS anyways (you couldn't use the shell successfully!).
      } finally {
        if (key != null) {
          key.Close();
        }
        if (newKey != null) {
          newKey.Close();
        }
      }
    }
  }
}
