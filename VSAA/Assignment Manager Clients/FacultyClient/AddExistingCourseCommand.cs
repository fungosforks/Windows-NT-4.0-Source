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

namespace FacultyClient
{
	/// <summary>
	/// Summary description for AddDeleteCourseCommand.
	/// </summary>
	public class AddExistingCourseCommand : IAddInCommand
	{
		private string m_strCommandName;
		private string m_strName;
		private string m_strItemText;
		private EnvDTE.AddIn m_addInInstance;
		private EnvDTE._DTE m_applicationObject;

		public AddExistingCourseCommand()
		{

		}
		public string addInName { get { return m_strName; } }
		public string commandName { get { return m_strCommandName; } }
		public string commandText { get { return m_strCommandName; } }

		/// <summary>
		/// Called when the AddIn is loaded. This method allows each of the commands to
		/// store member variables with the objects passed in and ensure that the menu
		/// items and commands have been properly added to the object model.
		/// </summary>
		/// <param name="application"> Root object in the application </param>
		/// <param name="connectMode"> 'Mode' in which the environment is starting up the addin </param>
		/// <param name="addIn"> Object representing this AddIn in the Object Model</param>
		public void OnConnection(EnvDTE._DTE application, Extensibility.ext_ConnectMode connectMode, EnvDTE.AddIn addIn)
		{
			m_applicationObject = (EnvDTE._DTE)application;
			m_addInInstance = (EnvDTE.AddIn)addIn;

			m_strCommandName = "AMAddExistingCourse";
			m_strName = AMResources.GetLocalizedString("AMAddExistingCourseName");
			m_strItemText= AMResources.GetLocalizedString("AMAddExistingCourseItemText");

			string description = AMResources.GetLocalizedString("AMAddExistingCourseDescription");
			EnvDTE.Commands commands = null;
			EnvDTE.Command command = null;
			_CommandBars officeCommandBars = null;
			CommandBar officeCommandBar = null;
			CommandBarControl officeCommandControl = null;
			CommandBar officeAcademic = null;
			
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
					officeAcademic = (CommandBar)m_applicationObject.Commands.AddCommandBar( amFacultyMenuItem, EnvDTE.vsCommandBarType.vsCommandBarTypeMenu, officeCommandBar,1);
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

		/// <summary>
		/// Called by the Object Model whenever this command is going to be displayed (in the menu
		/// or on a toolbar).
		/// </summary>
		/// <param name="status"> The current availability status of the command </param>
		public void QueryStatus(ref EnvDTE.vsCommandStatus status)
		{
			// This command is always supported.
			status = EnvDTE.vsCommandStatus.vsCommandStatusEnabled | EnvDTE.vsCommandStatus.vsCommandStatusSupported;
		}

		/// <summary>
		/// Called by the Object Model whenever the user invokes the command.
		/// </summary>
		/// <param name="executeOption"> Execution type </param>
		/// <param name="varIn"> Input paramaters (optional) </param>
		/// <param name="varOut"> Output paramaters (optional) </param>
		public void Exec(EnvDTE.vsCommandExecOption executeOption, ref object varIn, ref object varOut)
		{
			try 
			{
				AddExistingCourseDialog dialogAddExisting = new AddExistingCourseDialog(m_applicationObject);
				dialogAddExisting.ShowDialog();
			} 
			catch (System.Exception) 
			{
			}
		}
	}
}
