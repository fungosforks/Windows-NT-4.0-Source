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
	using Microsoft.Office;

	[System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual)]
	public interface IStudentCourseWork 
	{
		bool RegisterAssignmentManagerCourse(string courseName, string url, string guid, string xmlpath);
		bool UnregisterAssignmentManagerCourse(string guid);
		string EscapeStringForXPath(string s);
		object GetFileReadStream(string localFilePath);
		bool RegenerateUserCustomTab(bool noCoursesRemaining);
		void DisposeFileReadStream(object stream);
		bool SaveFileStream(object fileStream, string localFilePath);

	}
}
