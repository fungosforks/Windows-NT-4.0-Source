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
using EnvDTE;
using AssignmentManager.ClientUI;

namespace StudentClient
{
	/// <summary>
	/// Summary description for AddCourseCommand.
	/// </summary>
	public class AddCourseCommand : IAddInCommand
	{
		private string m_strCommandName;
		private string m_strName;
		private string m_strItemText;
		private EnvDTE.AddIn m_addInInstance;
		private EnvDTE._DTE m_applicationObject;

		public AddCourseCommand()
		{
		}
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

			m_strCommandName = "AMStudentAddCourse";
			m_strName = AMResources.GetLocalizedString("AMStudentAddCourseName");
			m_strItemText= AMResources.GetLocalizedString("AMStudentAddCourseItemText");

			string strDescription = AMResources.GetLocalizedString("AddRemoveCourseDescription");
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
				command = commands.AddNamedCommand(m_addInInstance, m_strCommandName, m_strName, strDescription,
					false, 108, ref contextGuids, (int) (EnvDTE.vsCommandStatus.vsCommandStatusEnabled | EnvDTE.vsCommandStatus.vsCommandStatusSupported));
          
				// Add the new command to the tools menu
				officeCommandBars = m_applicationObject.CommandBars;
				string amStudentMenuItem = AMResources.GetLocalizedString("AMStudentMenuItem");
				try
				{
					officeAcademic = (CommandBar)officeCommandBars[amStudentMenuItem];
				}
				catch
				{
				}
				if( officeAcademic == null )
				{
					officeCommandBar= (CommandBar)officeCommandBars["Tools"];
					officeAcademic = (CommandBar)m_applicationObject.Commands.AddCommandBar(amStudentMenuItem, EnvDTE.vsCommandBarType.vsCommandBarTypeMenu, officeCommandBar,1);
				}
				officeCommandControl = command.AddControl((object)officeAcademic, 1);
				officeCommandControl.Caption = m_strItemText;
			} 
			catch (System.Exception /*e*/) 
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
			application.Commands.Item("StudentClient.Connect." + m_strCommandName, 0).Delete();
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
				AddCourseDialog dialogAddCourse = new AddCourseDialog(m_applicationObject);
				dialogAddCourse.ShowDialog();
			} 
			catch (System.Exception) 
			{
			}
		}

	}
}
