//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//

using System;

namespace Microsoft.VisualStudio.Academic.AssignmentManager
{

	internal enum PermissionsID
	{
		Admin = 1,
		Faculty = 25,
		TA = 50,
		Grader = 75,
		Student = 100
	};
	/// <summary>
	/// Summary description for Roles.
	/// </summary>
	public class RoleM
	{
		private int _roleID;
		private string _roleName;

		public RoleM(int id, string name)
		{
			_roleID = id;
			_roleName = name;
		}

		public static RoleM[] GetAllRoles()
		{
			DatabaseCall dbc = new DatabaseCall("Roles_BrowseAll", DBCallType.Select);
			System.Data.DataSet ds = new System.Data.DataSet();
			dbc.Fill(ds);
			
			if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
			{
				return new RoleM[0];
			}
			else
			{
				RoleM[] roles = new RoleM[ (ds.Tables[0].Rows.Count) ];
				for( int i=0; i<ds.Tables[0].Rows.Count; i++ )
				{
					int roleID = Convert.ToInt32(ds.Tables[0].Rows[i]["RoleID"]);
					string roleName = ds.Tables[0].Rows[i]["Name"].ToString();
                    
                    // replace with localized name if available
                    string localizedName = SharedSupport.GetLocalizedString("UserRole_"+roleName);
                    if (localizedName != String.Empty)
                    {
                        roleName = localizedName;
                    }
					roles[i] = new RoleM(roleID, roleName);
				}
				return roles;
			}
		}

		public static RoleM GetUsersRoleInCourse(int userID, int courseID)
		{
			DatabaseCall dbc = new DatabaseCall("Roles_GetRoleForUserInCourse", DBCallType.Select);
			dbc.AddParameter("@UserID", userID);
			dbc.AddParameter("@CourseID", courseID);
			System.Data.DataSet ds = new System.Data.DataSet();
			
			dbc.Fill(ds);

			if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
			{
				return new RoleM(0,"");
			}
			else
			{
				int roleID = Convert.ToInt32(ds.Tables[0].Rows[0]["RoleID"]);
				string roleName = ds.Tables[0].Rows[0]["Name"].ToString();
				return new RoleM(roleID, roleName);
			}
		}

		public static void SetRoleInCourse(int userID, int courseID, int roleID)
		{
			DatabaseCall dbc = new DatabaseCall("Roles_UpdateRoleInCourse", DBCallType.Execute);
			dbc.AddParameter("@UserID", userID);
			dbc.AddParameter("@CourseID", courseID);
			dbc.AddParameter("@RoleID", roleID);
			dbc.Execute();
		}
		public string Name
		{
			get
			{
				return _roleName;
			}
		}

		public int ID
		{
			get
			{
				return _roleID;
			}
		}

	}
}
