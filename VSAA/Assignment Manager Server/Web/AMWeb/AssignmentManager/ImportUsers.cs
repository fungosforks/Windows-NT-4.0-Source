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

namespace Microsoft.VisualStudio.Academic.AssignmentManager
{
	/// <summary>
	/// Summary description for ImportResults.
	/// </summary>
	public class ImportUsers
	{
		private ImportUsers()
		{
		}

		internal static bool CommitImport(string importID)
		{
			DatabaseCall dbc = new DatabaseCall("Import_LoadPendingImport", DBCallType.Select);
			dbc.AddParameter("@ImportID", importID);
			System.Data.DataSet ds = new System.Data.DataSet();
			dbc.Fill(ds);
						
			if ((ds.Tables.Count <= 0) || (ds.Tables[0].Rows.Count <= 0) )
			{
				return false;
			}

			for(int i=0;i<ds.Tables[0].Rows.Count; i++)
			{
				try
				{
					UserM user = UserM.Load(Convert.ToInt32(ds.Tables[0].Rows[i]["UserID"]));
                    user.GenerateNewPassword();
					user.AddToCourse(Convert.ToInt32(ds.Tables[0].Rows[i]["CourseID"]));
				}
				catch
				{
				}
			}
			return true;
		}

		internal static void AbortImport(string importID)
		{
			DatabaseCall dbc = new DatabaseCall("Import_AbortImport", DBCallType.Execute);
			dbc.AddParameter("@ImportID", importID);

			dbc.Execute();
		}
	}
}
