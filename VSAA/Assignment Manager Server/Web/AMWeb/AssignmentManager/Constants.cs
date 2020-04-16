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


	internal enum SecurityAction
	{
		NAVBAR_VIEW_ADVANCED = 1,
		COURSE_VIEW = 2,
		COURSE_ADD = 3,
		COURSE_EDIT = 4,
		COURSE_DELETE = 5,
		ASSIGNMENT_ADD = 6,
		ASSIGNMENT_VIEW = 7,
		ASSIGNMENT_EDIT = 9,
		USER_VIEW = 10,
		USER_ADD = 11,
		USER_EDIT = 12,
		SETTING_EDIT = 13,
		SETTING_VIEW = 14,
		MESSAGE_VIEW = 15,
		MESSAGE_ADD = 16,
		SECURITY_VIEW = 17,
		SECURITY_ADD = 18,
		SECURITY_EDIT = 19,
		SECURITY_DELETE = 20,
		USERASSIGNMENT_VIEW = 21,
		USERASSIGNMENT_EDIT = 22,
	};

	/// <summary>
	///    Summary description for Constants.
	/// </summary>
	internal class Constants
	{
		internal const int ASSIGNMENTMANAGER_SYSTEM_ADMIN_USERID = 1;
		
		internal const int AUTOCOMPILE_NOTAPPLICABLE_STATUS = 0;
		internal const int AUTOCOMPILE_PENDING_STATUS = 1;
		internal const int AUTOCOMPILE_SUCCESSFUL_STATUS = 2;
		internal const int AUTOCOMPILE_FAILURE_STATUS = 3;
		internal const int AUTOCOMPILE_MAKETYPE_NMAKE = 1;
		internal const int AUTOCOMPILE_RETURN_CODE_SUCCESS = 0;
		internal const int AUTOCOMPILE_RETURN_CODE_FAILURE = 2;

		internal const int AUTOGRADE_NOTAPPLICABLE_STATUS = 0;
		internal const int AUTOGRADE_PENDING_STATUS = 1;
		internal const int AUTOGRADE_SUCCESSFUL_STATUS = 2;
		internal const int AUTOGRADE_FAILURE_STATUS = 3;
        
		//Grading Result Codes
		internal const int GRADE_FAILED_RESULT_CODE = -1;
		internal const int GRADE_SUCCESSFUL_RESULT_CODE = 0;

		//UserAssignmentDetail_DetailTypes
		internal const int AUTO_COMPILE_DETAILTYPE = 1;
		internal const int AUTO_GRADE_DETAILTYPE = 2;
		internal const int CHEAT_DETECT_DETAILTYPE = 3;

		// status constants - global
		internal const int INACTIVE_STATUS = 2;
		internal const int ACTIVE_STATUS = 1;

		//settings constants
		internal const string MAX_PROCESS_SETTING = "MaxProcessTime";
		internal const string MAX_PROJECT_SETTING = "MaxUploadSize";
		internal const string AUTOCHECK_SETTING = "AutoCheck";
		internal const string AUTOBUILD_SETTING = "AutoBuild";

		//MSMQ Queue, Build, Check Constants
		internal const string ACTION_QUEUE_NAME = "AMActions";
		internal const string ACTION_QUEUE_PATH = ".\\Private$\\AMActions";
		internal const string AM_SUBMIT_ACTION = "AM_SUBMIT_ACTION";
		internal const string AM_GROUP_ACTION = "AM_GROUP_ACTION";
		internal const string AM_BUILD = "AutoBuild";
		internal const string AM_CHECK = "AutoCheck";
		internal const string AM_SUBMIT_ACTION_USER_ASSIGNMENTID_ELEMENT = "userAssignmentID";
		internal const string AM_SUBMIT_ACTION_SERVER_ACTION_ATTRIBUTE = "name";
		internal const string ASSIGNMENT_MANAGER_DIRECTORY = "AssignmentManager";
		internal const string TEMP_ENVIRON_VARIABLE = "TEMP";

		internal const int QUEUE_TIMEOUT = 30;
		internal const string ACTION_SERVICE_NAME = "Assignment Manager Services";

		//Upload / Downloads
		internal const string ASSIGNMENTMANAGER_UPLOAD_DIRECTORY = "/AMUpload/";
		internal const string ASSIGNMENTMANAGER_DOWNLOAD_DIRECTORY = "/AMDownload/";
		internal const string ASSIGNMENTMANAGER_COURSES_DIRECTORY = "/Courses/";

		//Assignment
		internal const bool ASSIGNMENT_OVERWRITEPROMPT_FALSE = false;
		internal const bool ASSIGNMENT_OVERWRITEPROMPT_TRUE = true;

		//Course Root Storage path
		internal static string DEFAULT_COURSE_OFFERINGS_ROOT_STORAGE_PATH = System.Environment.GetEnvironmentVariable("ProgramFiles")+ @"\Assignment Manager\Data\";

		//StarterProject path
		internal const string STARTER_PROJECT_PATH = "StarterProject";

		//Settings limit constants
		internal const int MAX_PROCESS_LIMIT = 3600000;
		internal const int MAX_PROJECT_SIZE = 100;

		//nText parameter size limit - to address OleDbDataAdapter size checking issue
		internal const int NTEXT_SIZE = 2000000000;

		// Constants used to identify the value from the ServerRoleFlag from the Roles table.
		internal const int COURSE_ROLE = 0;
		internal const int SERVER_ROLE = 1;
	
		// AM User constants
		internal const string AMUserName = "AssignmentManager";
		internal const string AMUserLSAPasswordKey = "L$AssnMgrPassword";

		// Hashing algorithm
		internal const string HashMethod = "SHA512";

		internal const int BytesInMegaByte = 1048576;

		private Constants()
		{
		}
	}
}
