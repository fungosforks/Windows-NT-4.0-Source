//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//

using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Academic.AssignmentManager
{
	/// <summary>
	/// Summary description for ServiceControl.
	/// </summary>
	internal class ServiceControl
	{
		///<summary>
		///Series of contants ported from winnt.h and winsvc.h
		///</summary>
		private static readonly System.UInt32 SERVICE_NO_CHANGE		= (0xffffffff);

		//StartTypes 
		private static readonly System.UInt32 SERVICE_AUTO_START	= (0x00000002);
		private static readonly System.UInt32 SERVICE_DISABLED      = (0x00000004);

		//SCM Access Permissions
		private static readonly System.UInt32 SC_MANAGER_CONNECT			= (0x0001);
		private static readonly System.UInt32 SC_MANAGER_CREATE_SERVICE		= (0x0002);
		private static readonly System.UInt32 SC_MANAGER_ENUMERATE_SERVICE	= (0x0004);
		private static readonly System.UInt32 SC_MANAGER_LOCK				= (0x0008);
		private static readonly System.UInt32 SC_MANAGER_QUERY_LOCK_STATUS	= (0x0010);
		private static readonly System.UInt32 SC_MANAGER_MODIFY_BOOT_CONFIG = (0x0020);
		private static readonly System.UInt32 SC_MANAGER_ALL_ACCESS			= (SC_MANAGER_CONNECT            |
			SC_MANAGER_CREATE_SERVICE     |
			SC_MANAGER_ENUMERATE_SERVICE  |
			SC_MANAGER_LOCK               |
			SC_MANAGER_QUERY_LOCK_STATUS  |
			SC_MANAGER_MODIFY_BOOT_CONFIG);

		//
		// Service object specific access type
		//
		private static readonly System.UInt32 SERVICE_QUERY_CONFIG			= (0x0001);
		private static readonly System.UInt32 SERVICE_CHANGE_CONFIG			= (0x0002);
		private static readonly System.UInt32 SERVICE_QUERY_STATUS          = (0x0004);
		private static readonly System.UInt32 SERVICE_ENUMERATE_DEPENDENTS	= (0x0008);
		private static readonly System.UInt32 SERVICE_START                 = (0x0010);
		private static readonly System.UInt32 SERVICE_STOP                  = (0x0020);
		private static readonly System.UInt32 SERVICE_PAUSE_CONTINUE        = (0x0040);
		private static readonly System.UInt32 SERVICE_INTERROGATE           = (0x0080);
		private static readonly System.UInt32 SERVICE_USER_DEFINED_CONTROL  = (0x0100);
		private static readonly System.UInt32 SERVICE_ALL_ACCESS            = (SERVICE_QUERY_CONFIG         |
			SERVICE_CHANGE_CONFIG        |
			SERVICE_QUERY_STATUS         |
			SERVICE_ENUMERATE_DEPENDENTS |
			SERVICE_START                |
			SERVICE_STOP                 |
			SERVICE_PAUSE_CONTINUE       |
			SERVICE_INTERROGATE          | 
			SERVICE_USER_DEFINED_CONTROL);
		
		[DllImport("advapi32.dll")]
		private unsafe static extern System.IntPtr OpenService(
			[In]
			System.IntPtr SCManager,
			[In]
			System.String ServiceName,
			[In]
			System.UInt32 DesiredAccess
			);

		[DllImport("advapi32.dll")]
		private unsafe static extern bool CloseServiceHandle(
			[In]
			System.IntPtr SCObject
			);

		[DllImport("advapi32.dll")]
		private unsafe static extern System.IntPtr OpenSCManager(
			[In]
			System.String MachineName,
			[In]
			System.String DatabaseName,
			[In]
			System.UInt32 DesiredAccess
			);

		[DllImport("advapi32.dll")]
		private unsafe static extern bool ChangeServiceConfig(
			[In]
			System.IntPtr Service,
			[In]
			System.UInt32 ServiceType,
			[In]
			System.UInt32 StartType,
			[In]
			System.UInt32 ErrorControl,
			[In]
			System.String BinaryPathName,
			[In]
			System.String LoadOrderGroup,
			[In]
			System.String TagId,
			[In]
			System.String Dependencies,
			[In]
			System.String ServiceStartName,
			[In]
			System.String Password,
			[In]
			System.String DisplayName
			);

		internal static void EnableService(string serviceName)
		{
			// enable the service
			updateService(serviceName, SERVICE_AUTO_START);
		}

		internal static void DisableService(string serviceName)
		{
			// disable the service
			updateService(serviceName, SERVICE_DISABLED);
		}

		private static void updateService(string serviceName, System.UInt32 startType)
		{

			bool updateStatus = false;

			// open the service db
			System.IntPtr scm = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);

			// get a handle to the service
			System.IntPtr service = OpenService(scm, serviceName, SERVICE_CHANGE_CONFIG);

			// update the service
			updateStatus = ChangeServiceConfig(service, SERVICE_NO_CHANGE, startType, SERVICE_NO_CHANGE, null, null, null, null, null, null, null);

			// release the handle to the service
			CloseServiceHandle(service);

			// close the service db
			CloseServiceHandle(scm);

			if (!updateStatus)
			{
				// if the config change failed, throw an exception
				throw new Exception(SharedSupport.GetLocalizedString("Setting_UnableToUpdateService_Error"));
			}

		}
		

	}
}
