//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//
using System;
using Microsoft.Office.Core;
using AssignmentManager.ClientUI;

namespace FacultyClient 
{

	/// <summary>
	///   The object for implementing an Add-in.
	/// </summary>
	public class GotoHomePage : Object, IAddInCommand 
	{

		public string addInName { get { return m_strName; } }
		public string commandName { get { return m_strCommandName; } }
		public string commandText { get { return m_strCommandName; } }

		/// <summary>
		/// Registers a command and places it on the Tools menu.
		/// </summary>
		public void OnConnection(EnvDTE._DTE application, Extensibility.ext_ConnectMode connectMode, EnvDTE.AddIn addIn) 
		{
			m_applicationObject = (EnvDTE._DTE)application;
			m_addInInstance = (EnvDTE.AddIn)addIn;

			m_strCommandName = "GotoHomePage";
			m_strName = AMResources.GetLocalizedString("GotoHomePageName");
			m_strItemText= AMResources.GetLocalizedString("GotoHomePageItemText");
			m_strHomePageUrl = "vs:/default.htm?tab=" + AMResources.GetLocalizedString("GotoHomePagePageName");

			string description = AMResources.GetLocalizedString("GotoHomePageDescription");
			EnvDTE.Commands commands = null;
			EnvDTE.Command command = null;
			Microsoft.Office.Core._CommandBars officeCommandBars = null;
			Microsoft.Office.Core.CommandBar officeCommandBar = null;
			Microsoft.Office.Core.CommandBarControl officeCommandControl = null;
			Microsoft.Office.Core.CommandBar officeAcademic = null;
			object []contextGuids;
			contextGuids = new object[] { };
        
			commands = m_applicationObject.Commands;
			try 
			{
				command = commands.AddNamedCommand(m_addInInstance, m_strCommandName, m_strName, description,
					false, 108, ref contextGuids, (int) (EnvDTE.vsCommandStatus.vsCommandStatusEnabled | EnvDTE.vsCommandStatus.vsCommandStatusSupported));
          
				// Add the new command to the tools menu
				officeCommandBars = m_applicationObject.CommandBars;
				string amFacultyMenuItem = AMResources.GetLocalizedString("AMFacultyMenuItem");
				try
				{
					officeAcademic = (CommandBar)officeCommandBars[amFacultyMenuItem];
				}
				catch
				{
				}
				if( officeAcademic == null )
				{
					officeCommandBar= (CommandBar)officeCommandBars["Tools"];
					officeAcademic = (CommandBar)m_applicationObject.Commands.AddCommandBar(amFacultyMenuItem, EnvDTE.vsCommandBarType.vsCommandBarTypeMenu, officeCommandBar,1);
				}
				officeCommandControl = command.AddControl((object)officeAcademic, 1);
				officeCommandControl.Caption = m_strItemText;
			} 
			catch
			{
				// Falling into this simply means that the command was already registered.
			}
		}
		
		/// <summary>
		/// Called when the AddIn is discarded. This method allows each of the commands to
		/// to unregister and close down on exiting.
		/// </summary>
		/// <param name="application"> Root object in the application </param>
		/// <param name="connectMode"> 'Mode' in which the environment is closing the addin </param>
		/// <param name="addIn"> Object representing this AddIn in the Object Model</param>
		public void OnDisconnection(EnvDTE._DTE application, Extensibility.ext_DisconnectMode disconnectMode, EnvDTE.AddIn addIn)
		{
			application.Commands.Item("FacultyClient.Connect." + m_strCommandName, 0).Delete();
		}

		public void QueryStatus(ref EnvDTE.vsCommandStatus status) 
		{
			// This command is always supported.
			status = EnvDTE.vsCommandStatus.vsCommandStatusEnabled | EnvDTE.vsCommandStatus.vsCommandStatusSupported;
		}

		public void Exec(EnvDTE.vsCommandExecOption executeOption, ref object varIn, ref object varOut) 
		{
			try 
			{
				// Load the faculty Course Management homepage. Note that this will create a new browser if one
				// isn't already open, but will reuse the existing one, if one is.
				m_applicationObject.ItemOperations.Navigate(m_strHomePageUrl, EnvDTE.vsNavigateOptions.vsNavigateOptionsDefault);
			} 
			catch (System.Exception) 
			{
			}
		}
		
		private EnvDTE._DTE m_applicationObject;
		private EnvDTE.AddIn m_addInInstance;
		private string m_strCommandName;
		private string m_strName;
		private string m_strItemText;
		private string m_strHomePageUrl;
	}
}
