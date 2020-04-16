//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//

namespace Microsoft.VisualStudio.Academic.AssignmentManager.Common
{
    using System;
	using System.Web;
	using Microsoft.VisualStudio.Academic.AssignmentManager;

    /// <summary>
    ///    Summary description for Functions.
    /// </summary>
    internal class Functions
    {
        internal Functions()
        {
        }

		internal int ValidateNumericQueryStringParameter(System.Web.HttpRequest request, string param)
		{
			try
			{
				if(!request.QueryString[param].Equals(null) && !request.QueryString[param].Equals(""))
				{
					return Convert.ToInt32(request.QueryString[param]);
				}
				else
				{
					System.Web.HttpContext.Current.Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_MissingParameter&" + request.QueryString.ToString());
					return 0;
				}
			}
			catch
			{
				return 0;
			}
		}

		internal int ValidateOptionNumericQueryStringParam(System.Web.HttpRequest request, string param)
		{
			try
			{
				if(!request.QueryString[param].Equals(null) && !request.QueryString[param].Equals(""))
				{
					return Convert.ToInt32(request.QueryString[param]);
				}
				else
				{
					return 0;
				}
			}
			catch
			{
				return 0;
			}
		}

		internal string ValidateStringQueryStringParameter(System.Web.HttpRequest request, string param)
		{
			try
			{
				if(request.QueryString.Get(param) != null && request.QueryString.Get(param) != "")
				{
					return request.QueryString.Get(param).ToString();
				}
				else
				{
					System.Web.HttpContext.Current.Response.Redirect(@"../Error.aspx?ErrorDetail=" + "Global_MissingParameter&" + request.QueryString.ToString());
					return String.Empty;
				}
			}
			catch
			{
				return String.Empty;
			}
		}


		internal bool ValidateDate(string date)
		{
			try
			{
				System.DateTime date1 = System.Convert.ToDateTime(date);				
				if(date1 > System.DateTime.Today)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch
			{
				return true;
			}

		}
    }
}
