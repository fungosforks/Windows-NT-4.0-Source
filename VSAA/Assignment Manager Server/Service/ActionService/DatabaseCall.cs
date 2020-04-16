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
using System.Data.OleDb;

namespace Microsoft.VisualStudio.Academic.AssignmentManager.ActionService
{
	public enum DBCallType
	{
		Select,
		Execute
	}
	/// <summary>
	/// Summary description for DatabaseCall.
	/// </summary>
	public class DatabaseCall
	{
		private DBCallType dbtype;
		private OleDbConnection con;
		private OleDbCommand cmd;
		private OleDbDataAdapter adap;

		public DatabaseCall(string sproc, DBCallType type)
		{
			dbtype = type;
			con = new OleDbConnection(SharedSupport.ConnectionString);
			if(type == DBCallType.Select)
			{
				adap = new OleDbDataAdapter(sproc, con);
				cmd = adap.SelectCommand;
			}
			else
			{
				cmd = new OleDbCommand(sproc,con);
			}
			cmd.CommandType = CommandType.StoredProcedure;
			System.Data.DataSet ds = new System.Data.DataSet();			
		}

		public void AddParameter(string param, string val)
		{
			OleDbParameter dbparam = new OleDbParameter(param, OleDbType.WChar);
			dbparam.Direction = System.Data.ParameterDirection.Input;
			dbparam.Value = validDBValue(val);
			cmd.Parameters.Add(dbparam);
		}

		public void AddParameter(string param, int val)
		{
			OleDbParameter dbparam = new OleDbParameter(param, OleDbType.Integer);
			dbparam.Direction = System.Data.ParameterDirection.Input;
			dbparam.Value = val;
			cmd.Parameters.Add(dbparam);
		}

		public void AddParameter(string param, System.Guid val)
		{
			OleDbParameter dbparam = new OleDbParameter(param, OleDbType.Guid);
			dbparam.Direction = System.Data.ParameterDirection.Input;
			dbparam.Value = val;
			cmd.Parameters.Add(dbparam);
		}

		public void AddParameter(string param, DateTime val)
		{
			OleDbParameter dbparam = new OleDbParameter(param, OleDbType.DBTimeStamp);
			dbparam.Direction = System.Data.ParameterDirection.Input;
			dbparam.Value = val;
			cmd.Parameters.Add(dbparam);
		}

		public void AddParameter(string param, bool val)
		{
			OleDbParameter dbparam = new OleDbParameter(param, OleDbType.Boolean);
			dbparam.Direction = System.Data.ParameterDirection.Input;
			dbparam.Value = val;
			cmd.Parameters.Add(dbparam);
		}

		public void AddParameter(string param, byte val)
		{
			OleDbParameter dbparam = new OleDbParameter(param, OleDbType.TinyInt);
			dbparam.Direction = System.Data.ParameterDirection.Input;
			dbparam.Value = val;
			cmd.Parameters.Add(dbparam);
		}

		public void AddOutputParameter(string param)
		{
			OleDbParameter dbparam = new OleDbParameter(param, OleDbType.Integer);
			dbparam.Direction = System.Data.ParameterDirection.Output;
			cmd.Parameters.Add(dbparam);		
		}

		public void AddNTextParameter(string param, string val)
		{
			OleDbParameter dbparam = new OleDbParameter(param, OleDbType.VarWChar, Constants.NTEXT_SIZE);
			dbparam.Direction = System.Data.ParameterDirection.Input;
			dbparam.Value = validDBValue(val);
			cmd.Parameters.Add(dbparam);
		}

		public void Fill(DataSet ds)
		{
			adap.SelectCommand = cmd;
			try
			{
				adap.Fill(ds);
			}
			catch (System.Exception e)
			{
				SharedSupport.HandleError(e);
			}
		}
		
		public void Execute()
		{
			try
			{
				con.Open();
				cmd.ExecuteNonQuery();
			}
			catch (System.Exception e)
			{
				SharedSupport.HandleError(e);
			}
			finally
			{
				con.Close();
			}
		}

		public object GetOutputParam(string param)
		{
			return cmd.Parameters[param].Value;
		}

		private Object validDBValue(string paramString)
		{
			if( (paramString != null) && (!paramString.Equals(String.Empty)) )
			{
				return paramString;
			}
			else
			{
				return DBNull.Value;
			}
		}

	}
}
