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

  internal class Constants {
    private Constants() { } // Disallow construction
    
    public static string KeyName = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders";
    public static string ValueName = "AppData";
    public static string ApplicationPath = "\\Microsoft\\VisualStudio\\7.1\\Academic\\";
    public static string CourseFileName = "assignments.xml";
    public static string XslFileName = "course.xsl";
    public static string ManagedCoursesFileName = "AMFacultyManagedCourses.xml";
    public static string VSProtocolSafeHive = "\\VSProtocol\\SafeDomains";
    public static string SafeDomainChangedValueName = "SafeDomainsChanged";
    public static string VisualStudioHTMLDir = "HTML\\";
    public static string VisualStudioCourseManagementTabDir = "Custom\\amfaculty.xml";
    public static string UserTabFile = "AMFacultyStartPage.xml";
    public static string InstallationDirValueName = "InstallDir";
  }
}
