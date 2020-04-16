//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//
namespace FacultyClient
{
	using System;
	using Microsoft.Office.Core;
	using Extensibility;
	using System.Runtime.InteropServices;
	using EnvDTE;
	
	#region Read me for Add-in installation and setup information.
	// When run, the Add-in wizard prepared the registry for the Add-in.
	// At a later time, if the Add-in becomes unavailable for reasons such as:
	//   1) You moved this project to a computer other than which is was originally created on.
	//   2) You chose 'Yes' when presented with a message asking if you wish to remove the Add-in.
	//   3) Registry corruption.
	// you will need to re-register the Add-in by building the MyAddin21Setup project 
	// by right clicking the project in the Solution Explorer, then choosing install.
	#endregion
	
	/// <summary>
	///   The object for implementing an Add-in.
	/// </summary>
	/// <seealso class='IDTExtensibility2' />
	[GuidAttribute("51D61707-4750-442E-AE6E-D567CD31FC20"), ProgId("FacultyClient.Connect")]
	public class Connect : Object, Extensibility.IDTExtensibility2, IDTCommandTarget
	{
		/// <summary>
		///		Implements the constructor for the Add-in object.
		///		Place your initialization code within this method.
		/// </summary>
		public Connect()
		{
		}

		/// <summary>
		///      Implements the OnConnection method of the IDTExtensibility2 interface.
		///      Receives notification that the Add-in is being loaded.
		/// </summary>
		/// <param term='application'>
		///      Root object of the host application.
		/// </param>
		/// <param term='connectMode'>
		///      Describes how the Add-in is being loaded.
		/// </param>
		/// <param term='addInInst'>
		///      Object representing this Add-in.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, Extensibility.ext_ConnectMode connectMode, object addInInst, ref System.Array custom)
		{
      
			m_Commands = new System.Collections.ArrayList();
			m_Commands.Add(new CodeMarker());
			m_Commands.Add(new GotoHomePage());
			m_Commands.Add(new DeleteCourseCommand());
			m_Commands.Add(new AddCourseCommand());
			m_Commands.Add(new AddExistingCourseCommand());
			m_strProgID = "FacultyClient.Connect"; 

			m_applicationObject = (EnvDTE._DTE)application;
			m_addInInstance = (EnvDTE.AddIn)addInInst;

			// Instead of the default .Object property (which would be the IDispatch of this object), we need
			// to create an instance of an object that exposes the proper COM interfaces.
			m_applicationObject.AddIns.Item(m_strProgID).Object = new FacultyTools(m_applicationObject);

			// Allow all of the components to perform any registration steps they feel necessary.
			foreach (IAddInCommand command in m_Commands) 
			{ 
				try 
				{
					command.OnConnection(m_applicationObject, connectMode, m_addInInstance);
				} 
				catch (System.Exception /*e*/) 
				{
				}
			}
			
		}

		/// <summary>
		///     Implements the OnDisconnection method of the IDTExtensibility2 interface.
		///     Receives notification that the Add-in is being unloaded.
		/// </summary>
		/// <param term='disconnectMode'>
		///      Describes how the Add-in is being unloaded.
		/// </param>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(Extensibility.ext_DisconnectMode disconnectMode, ref System.Array custom)
		{
			// Allow each of the components to unregister.
			foreach (IAddInCommand command in m_Commands) 
			{ 
				try 
				{
					command.OnDisconnection(m_applicationObject, disconnectMode, m_addInInstance);
				} 
				catch (System.Exception e) 
				{
					System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
				}
			}
		
			//Remove the Folder
//			try
//			{
//				((CommandBar)officeCommandBars["Assignment Manager &Faculty"]).Delete();
//			}
//			catch
//			{
//			}
		}

		/// <summary>
		///      Implements the OnAddInsUpdate method of the IDTExtensibility2 interface.
		///      Receives notification that the collection of Add-ins has changed.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnAddInsUpdate(ref System.Array custom)
		{
		}

		/// <summary>
		///      Implements the OnStartupComplete method of the IDTExtensibility2 interface.
		///      Receives notification that the host application has completed loading.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref System.Array custom)
		{
		}

		/// <summary>
		///      Implements the OnBeginShutdown method of the IDTExtensibility2 interface.
		///      Receives notification that the host application is being unloaded.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref System.Array custom)
		{
		}
		
		/// <summary>
		///      Implements the QueryStatus method of the IDTCommandTarget interface.
		///      This is called when the command's availability is updated
		/// </summary>
		/// <param term='commandName'>
		///		The name of the command to determine state for.
		/// </param>
		/// <param term='neededText'>
		///		Text that is needed for the command.
		/// </param>
		/// <param term='status'>
		///		The state of the command in the user interface.
		/// </param>
		/// <param term='commandText'>
		///		Text requested by the neededText parameter.
		/// </param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, EnvDTE.vsCommandStatusTextWanted neededText, ref EnvDTE.vsCommandStatus status, ref object commandText)
		{
			IAddInCommand currentCommand = GetAddIn(commandName);

			if (currentCommand == null) 
			{
				return;
			}

			currentCommand.QueryStatus(ref status);

			switch (neededText) 
			{
				case EnvDTE.vsCommandStatusTextWanted.vsCommandStatusTextWantedName:
					commandText = currentCommand.commandName;
					break;
				case EnvDTE.vsCommandStatusTextWanted.vsCommandStatusTextWantedStatus:
					// CONSIDER: what does this do?
					break;
				default:
					break;
			}
		}

		/// <summary>
		///      Implements the Exec method of the IDTCommandTarget interface.
		///      This is called when the command is invoked.
		/// </summary>
		/// <param term='commandName'>
		///		The name of the command to execute.
		/// </param>
		/// <param term='executeOption'>
		///		Describes how the command should be run.
		/// </param>
		/// <param term='varIn'>
		///		Parameters passed from the caller to the command handler.
		/// </param>
		/// <param term='varOut'>
		///		Parameters passed from the command handler to the caller.
		/// </param>
		/// <param term='handled'>
		///		Informs the caller if the command was handled or not.
		/// </param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, EnvDTE.vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;

			IAddInCommand currentCommand = GetAddIn(commandName);
			if (currentCommand == null) 
			{
				return;
			}

			try 
			{
				currentCommand.Exec(executeOption, ref varIn, ref varOut);
				handled = true;
			} 
			catch (System.Exception /*e*/) 
			{
			}
		}

		/// <summary>
		/// If available, this command takes a fully-qualified AddIn command name and returns
		/// its implementation object.
		/// </summary>
		/// <param name="commandName"> A full ProgID.CommandName string, as passed by the Shell to QueryStatus & Exec </param>
		private IAddInCommand GetAddIn(string commandName) 
		{
			string strPrefix = m_strProgID + ".";
			string strCurrent = null;
			IAddInCommand command = null;
			
			foreach (IAddInCommand currCommand in m_Commands) 
			{
				strCurrent = strPrefix + currCommand.commandName;
				if (System.String.Compare(strCurrent, commandName, true) == 0) 
				{
					command = currCommand;
					break;
				}
			}

			return command;
		}
		private EnvDTE._DTE m_applicationObject;
		private EnvDTE.AddIn m_addInInstance;
		private string m_strProgID;
		private System.Collections.ArrayList m_Commands;		
	}
}