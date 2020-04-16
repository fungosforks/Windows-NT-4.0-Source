//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//

using System;

namespace Microsoft.VisualStudio.Academic.AssignmentManager.ActionService
{
	/// <summary>
	/// Summary description for Constants.
	/// </summary>
	internal class Constants
	{
		private Constants()
		{
			//make class non-createable.
		}

		internal const int ASSIGNMENTMANAGER_SYSTEM_ADMIN_USERID = 1;

		internal const int AUTOCOMPILE_NOTAPPLICABLE_STATUS = 0;
		internal const int AUTOCOMPILE_PENDING_STATUS = 1;
		internal const int AUTOCOMPILE_SUCCESSFUL_STATUS = 2;
		internal const int AUTOCOMPILE_FAILURE_STATUS = 3;
		internal const int AUTOCOMPILE_MAKETYPE_NMAKE = 1;
		internal const int AUTOCOMPILE_RETURN_CODE_SUCCESS = 0;
		internal const int AUTOCOMPILE_RETURN_CODE_FAILURE = 2;
		
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

		//UserAssignmentDetail_DetailTypes
		internal const int AUTO_COMPILE_DETAILTYPE = 1;
		internal const int AUTO_GRADE_DETAILTYPE = 2;
		internal const int CHEAT_DETECT_DETAILTYPE = 3;

		internal const int AUTOGRADE_NOTAPPLICABLE_STATUS = 0;
		internal const int AUTOGRADE_PENDING_STATUS = 1;
		internal const int AUTOGRADE_SUCCESSFUL_STATUS = 2;
		internal const int AUTOGRADE_FAILURE_STATUS = 3;
        
		//Grading Result Codes
		internal const int GRADE_FAILED_RESULT_CODE = -1;
		internal const int GRADE_SUCCESSFUL_RESULT_CODE = 0;

		// AM User constants
		internal const string AMUserName = "AssignmentManager";
		internal const string AMUserLSAPasswordKey = "L$AssnMgrPassword";

		internal const int USERASSIGNMENT_VIEW_ACTION = 21;
		internal const int USERASSIGNMENT_EDIT_ACTION = 22;

		// action constants: please use subject_verb_action syntax
		internal const int NAVBAR_VIEW_ADVANCED_ACTION = 1;

		internal const int COURSE_VIEW_ACTION = 2;
		internal const int COURSE_ADD_ACTION = 3;
		internal const int COURSE_EDIT_ACTION = 4;
		internal const int COURSE_DELETE_ACTION = 5;

		internal const int SETTING_EDIT_ACTION = 13;
		internal const int SETTING_VIEW_ACTION = 14;
		
		internal const int MESSAGE_VIEW_ACTION = 15;
		internal const int MESSAGE_ADD_ACTION = 16;
		
		internal const int SECURITY_VIEW_ACTION = 17;
		internal const int SECURITY_ADD_ACTION = 18;
		internal const int SECURITY_EDIT_ACTION = 19;
		internal const int SECURITY_DELETE_ACTION = 20;

		internal const int ASSIGNMENT_ADD_ACTION = 6;
		internal const int ASSIGNMENT_VIEW_ACTION = 7;
		internal const int ASSIGNMENT_EDIT_ACTION = 9;

		internal const int USER_VIEW_ACTION = 10;
		internal const int USER_ADD_ACTION = 11;
		internal const int USER_EDIT_ACTION = 12;

		//Notification Sending Method Types
		internal const int SEND_INTERNAL_NOTIFICATION = 1;
		internal const int SEND_EMAIL_NOTIFICATION = 2;
		internal const int SEND_BOTH_NOTIFICATION = 3;

		// status constants - global
		internal const int INACTIVE_STATUS = 2;
		internal const int ACTIVE_STATUS = 1;
		//nText parameter size limit - to address OleDbDataAdapter size checking issue
		internal const int NTEXT_SIZE = 2000000000;

		// Hashing algorithm
		internal const string HashMethod = "SHA512";

	}
}
