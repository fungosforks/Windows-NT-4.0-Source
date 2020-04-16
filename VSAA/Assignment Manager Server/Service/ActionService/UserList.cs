//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//

using System;
using System.Data;

namespace Microsoft.VisualStudio.Academic.AssignmentManager.ActionService
{
	/// <summary>
	/// Summary description for UserList.
	/// </summary>
	public class UserList
	{
		private DataSet ds = null;
		public UserList()
		{
			ds = new DataSet();
		}

		public int[] UserIDList
		{
			get
			{
				if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
				{
					// return empty array.
					return new int[0];
				}

				int[] userList = new int[ds.Tables[0].Rows.Count];
				for (int i=0;i<userList.Length;i++)
				{
					userList[i] = Convert.ToInt32(ds.Tables[0].Rows[i]["UserID"]);
				}
				return userList;
			}
		}


		public static UserList GetListFromCourse(int courseID)
		{
			UserList userList = new UserList();
			DatabaseCall dbc = new DatabaseCall("Courses_GetUserList", DBCallType.Select);
			dbc.AddParameter("@CourseID", courseID);

			dbc.Fill(userList.ds);
			return userList;
		}
		public DataView DataView
		{
			get
			{
				if (ds != null && ds.Tables.Count > 0)
				{
					return new DataView(ds.Tables[0]);
				}
				else
				{
					return null;
				}
			}
		}
	}
}
