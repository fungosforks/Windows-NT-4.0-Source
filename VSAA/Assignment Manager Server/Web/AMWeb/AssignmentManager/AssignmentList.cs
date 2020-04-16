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
	/// <summary>
	/// Summary description for AssignmentList.
	/// </summary>
	public class AssignmentList
	{
		private System.Data.DataSet assignmentDS = null;
		public AssignmentList()
		{
			assignmentDS = new System.Data.DataSet();
		}

		public static AssignmentList GetStudentAssignmentListForCourse(int userID, int  CourseID)
		{
			//****

			DatabaseCall dbc = new DatabaseCall("Courses_GetStudentAssignmentList", DBCallType.Select);
			AssignmentList assignList = new AssignmentList();
			dbc.AddParameter("@UserID", userID);
			dbc.AddParameter("@CourseID", CourseID);

			dbc.Fill(assignList.assignmentDS);
			return assignList;
		}

		public static AssignmentList GetAssignmentListForCourse(int  CourseID)
		{
			//****
			DatabaseCall dbc = new DatabaseCall("Courses_GetAssignmentList", DBCallType.Select);
			AssignmentList assignList = new AssignmentList();
			dbc.AddParameter("@CourseID",CourseID);
			dbc.Fill(assignList.assignmentDS);
			return assignList;
		}

		public static AssignmentList GetAssignmentListNotInCourse(int  CourseID)
		{
			//****
			DatabaseCall dbc = new DatabaseCall("Courses_GetAssignmentListNotInCourse",DBCallType.Select);
			AssignmentList assignList = new AssignmentList();
			dbc.AddParameter("@CourseID", CourseID);
			dbc.Fill(assignList.assignmentDS);
			return assignList;
		}

		public int Count
		{
			get
			{
				if (assignmentDS == null)
				{
					return 0;
				}
				else
				{
					
					if (assignmentDS.Tables.Count > 0)
					{
						return assignmentDS.Tables[0].Rows.Count;
					}
					else
					{
						return 0;
					}
				}
			}
		}

		public string GetOverallGradeForItem(int itemNumber)
		{
			if (this.assignmentDS == null)
			{
				return "";
			}
			else
			{
				return assignmentDS.Tables[0].Rows[itemNumber]["OverallGrade"].ToString();
			}
		}

		public void SetOverallGradeForItem(int itemNumber, string overallGrade)
		{
			if (this.assignmentDS == null)
			{
				throw new Exception("Invalid Assignment");
			}
			else
			{
				assignmentDS.Tables[0].Rows[itemNumber]["OverallGrade"] = overallGrade;
			}
		}

		internal AssignmentM GetAssignmentAt(int index)
		{
			if (this.assignmentDS == null)
			{
				return null;
			}
			else
			{
				
				int assignmentID = Convert.ToInt32( assignmentDS.Tables[0].Rows[index]["AssignmentID"] );
				return AssignmentM.Load(assignmentID);
			}
		}

		public System.Data.DataView GetDefaultView(System.Web.HttpServerUtility encode)
		{
            for (int i=0;i< assignmentDS.Tables[0].Rows.Count; i++)
            {
                for (int j=0;j<assignmentDS.Tables[0].Columns.Count; j++)
                {
                    try
                    {
                        assignmentDS.Tables[0].Rows[i][j] = encode.HtmlEncode( assignmentDS.Tables[0].Rows[i][j].ToString() );
                    }
                    catch
                    {
                    }
                }
            }
            return assignmentDS.Tables[0].DefaultView; 
		}

		public static AssignmentList GetSubmissionsForAssignment(int AssignmentID)
		{
			DatabaseCall dbc = new DatabaseCall("Courses_GetSubmissionsList", DBCallType.Select);
			AssignmentList assignList = new AssignmentList();
			dbc.AddParameter("@AssignmentID", AssignmentID);
			dbc.Fill(assignList.assignmentDS);
			return assignList;
		}

		public System.Data.DataSet GetDataSource(System.Web.HttpServerUtility encode)
		{

            for (int i=0;i< assignmentDS.Tables[0].Rows.Count; i++)
            {
                for (int j=0;j<assignmentDS.Tables[0].Columns.Count; j++)
                {
                    try
                    {
                        assignmentDS.Tables[0].Rows[i][j] = encode.HtmlEncode( assignmentDS.Tables[0].Rows[i][j].ToString() );
                    }
                    catch {}
                }
            }
            return assignmentDS;
        }
	}
}
