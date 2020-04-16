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

	/// <summary>
	/// Because of the large number of methods and 'simply boilerplate code' that goes into
	/// implementing a command, the most common operations are factored out into this
	/// interface, which is more in the spirit of .NET.
	/// </summary>	
	internal interface IAddInCommand 
	{
		/// <summary>
		/// Name of the addin.
		/// </summary>
		string addInName { get; }
		
		/// <summary>
		/// Name of the command. This will be concatenated to the ProgID to form
		/// the unique name that VS will use to identify the AddIn and allow indexing
		/// through the object model.
		/// </summary>
		string commandName { get; }

		/// <summary>
		/// The text that will appear on the display of the menu or toolbar
		/// item.
		/// </summary>
		string commandText { get; }

		/// <summary>
		/// Called when the AddIn is loaded. This method allows each of the commands to
		/// store member variables with the objects passed in and ensure that the menu
		/// items and commands have been properly added to the object model.
		/// </summary>
		/// <param name="application"> Root object in the application </param>
		/// <param name="connectMode"> 'Mode' in which the environment is starting up the addin </param>
		/// <param name="addIn"> Object representing this AddIn in the Object Model</param>
		void OnConnection(EnvDTE._DTE application, Extensibility.ext_ConnectMode connectMode, EnvDTE.AddIn addIn);

		/// <summary>
		/// Called when the AddIn is discarded. This method allows each of the commands to
		/// to unregister and close down on exiting.
		/// </summary>
		/// <param name="application"> Root object in the application </param>
		/// <param name="connectMode"> 'Mode' in which the environment is closing the addin </param>
		/// <param name="addIn"> Object representing this AddIn in the Object Model</param>
		void OnDisconnection(EnvDTE._DTE application, Extensibility.ext_DisconnectMode disconnectMode, EnvDTE.AddIn addIn);
	  
		/// <summary>
		/// Called by the Object Model whenever this command is going to be displayed (in the menu
		/// or on a toolbar).
		/// </summary>
		/// <param name="status"> The current availability status of the command </param>
		void QueryStatus(ref EnvDTE.vsCommandStatus status);
		
		/// <summary>
		/// Called by the Object Model whenever the user invokes the command.
		/// </summary>
		/// <param name="executeOption"> Execution type </param>
		/// <param name="varIn"> Input paramaters (optional) </param>
		/// <param name="varOut"> Output paramaters (optional) </param>
		void Exec(EnvDTE.vsCommandExecOption executeOption, ref object varIn, ref object varOut);
	}
}
