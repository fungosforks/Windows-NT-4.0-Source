//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//
using System;

namespace FacultyClient 
{
	[System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
	public interface ICourseManagement 
	{
		bool CreateNewProject(string sourceUniqueProjectName, string path, string destProjectName, bool performExtraction);
		bool RegisterAssignmentManagerCourse(string courseName, string url, string guid);
		bool UnregisterAssignmentManagerCourse(string guid);
		bool RegenerateUserCustomTab(bool noCoursesRemaining);
		object GetFileReadStream(string localFilePath);
		void DisposeFileReadStream(object stream);
		bool SaveFileStream(object fileStream, string localFilePath);
	}
}
