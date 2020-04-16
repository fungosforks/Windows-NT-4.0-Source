//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//
using System;

namespace AssignmentManager.ClientUI
{
    public class AMResources
    {
		private static string _localizedStrings = "AMClientResources";

		public static string GetLocalizedString(string name)
		{		
			//retrieves localized string from satellite DLL
			System.Resources.ResourceManager rm;
			string resourceString = ""; 
			string culture = System.Globalization.CultureInfo.CurrentCulture.ToString();

			try
			{				
				rm = new System.Resources.ResourceManager(_localizedStrings, System.Reflection.Assembly.GetExecutingAssembly());

				// honor culture if passed in
				if (!culture.Equals(String.Empty))
				{
					System.Globalization.CultureInfo globalCulture = new System.Globalization.CultureInfo(culture);
					resourceString = rm.GetString(name, globalCulture);
				}
				else
				{
					resourceString = rm.GetString(name);	
				}
						
			}
			catch(System.Exception ex)
			{
				throw(new Exception(ex.Message));
			}

			return resourceString;			
			
		}
    }
}
