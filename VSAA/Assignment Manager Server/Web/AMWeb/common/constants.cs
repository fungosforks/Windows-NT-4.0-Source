//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//

using System;

namespace Microsoft.VisualStudio.Academic.AssignmentManager.Common
{
	/// <summary>
	/// Summary description for constants.
	/// </summary>
	public class constants
	{
		private constants()
		{
			// Make Class uncreateable
		}

		/// <summary>
		/// Faculty Navigation Constants
		/// </summary>
		// side nav
		public const int SIDE_NAV_COURSE_MANAGEMENT = 100;
		public const int SIDE_NAV_SERVER_ADMIN = 200;
		public const int SIDE_NAV_MESSAGES = 300;

		// top nav Course Management
		public const int TOP_NAV_COURSE_INFO = 105;
		public const int TOP_NAV_COURSE_ASSIGNMENTS = 110;
		public const int TOP_NAV_COURSE_USERS = 115;
		// top nav Settings
		public const int TOP_NAV_SERVER_SETTINGS = 205;
		public const int TOP_NAV_SERVER_SECURITY = 210;
		public const int TOP_NAV_SERVER_MYACCOUNT = 220;
		// top nav Messages
		public const int TOP_NAV_MESSAGES_COMPOSE = 305;
		public const int TOP_NAV_MESSAGES_READ = 310;

		/// <summary>
		/// Student Navigation Constants
		/// </summary>
		// side nav
		public const int SIDE_NAV_STUDENT_COURSE = 400;
		public const int SIDE_NAV_STUDENT_MESSAGES = 500;
		public const int SIDE_NAV_STUDENT_CHANGE_PASSWORD = 600;

		//top nav Course
		public const int TOP_NAV_STUDENT_COURSE_INFO = 405;
		public const int TOP_NAV_STUDENT_COURSE_ASSIGNMENTS = 410;

		//top nav Messages
		public const int TOP_NAV_STUDENT_MESSAGES_READ = 505;

		//top nav Change Password
		public const int TOP_NAV_STUDENT_CHANGE_PASSWORD = 605;

		public const string BACK_TO_CORETOOLS_LINK = "vs:/default.htm";

	}
}
