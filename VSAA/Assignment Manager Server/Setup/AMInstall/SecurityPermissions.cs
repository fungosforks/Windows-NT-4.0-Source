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
using Microsoft.Win32;
using System.DirectoryServices;
using System.IO;
using System.Messaging;

namespace AMInstall
{
	/// <summary>
	/// SecurityPermissions class used to set the permissions on a file or folder.
	/// </summary>
	internal class SecurityPermissions
	{
		// consts for P/Invoke
		internal const int SE_FILE_OBJECT = 1;
		internal const uint DACL_SECURITY_INFORMATION = 0x00000004;
		internal const uint MQSEC_QUEUE_GENERIC_ALL = 0x000f003f;
		internal const uint _maxVersion2AceSize = 152;
		internal const uint OBJECT_INHERIT_ACE = 0x1;
		internal const uint CONTAINER_INHERIT_ACE = 0x2;
		internal const int ACL_REVISION = 2;

		// P/Invoke function definition
		[DllImport("advapi32.dll")]
		protected unsafe static extern uint GetNamedSecurityInfo(
			string pObjectName, 
			int ObjectType, 
			uint SecurityInfo, 
			IntPtr ppsidOwner, 
			IntPtr ppsidGroup, 
			IntPtr ppDacl, 
			IntPtr ppSacl, 
			void** ppSecurityDescriptor);

		[DllImport("kernel32.dll")]
		protected unsafe static extern void* LocalFree(void* hMem);

		[DllImport("advapi32.dll")]
		internal static extern unsafe bool ConvertStringSecurityDescriptorToSecurityDescriptor(
			string StringSecurityDescriptor,
			uint StringSDRevision,
			void *SecurityDescriptor,
			ref ulong SecurityDescriptorSize);

		[DllImport("advapi32.dll")]
		internal static extern unsafe bool SetFileSecurity(
			string lpFileName,
			uint SecurityInformation,
			void *SecurityDescriptor
			);

		private SecurityPermissions()
		{
			// Make class non-createable
		}

		internal unsafe static bool SetVDirSecurityDescriptor(string virtualRootSubdir, string SDString)
		{
			try
			{
				// First, get the full path to the directory
				string directoryName;
				DirectoryEntry entry = new DirectoryEntry("IIS://localhost/w3svc/1/root");
				directoryName = entry.Properties["Path"].Value.ToString();
				directoryName = directoryName + "\\" + virtualRootSubdir;
			
				return RecurseAndSetSecurity(directoryName, SDString);			
			}
			catch( Exception )
			{
				return false;
			}
		}

		internal unsafe static bool RecurseAndSetSecurity(string directory, string SDString)
		{
			bool retVal = true;
			// Set permissions for all subdirectories
			DirectoryInfo di = new DirectoryInfo(directory);		
			foreach ( DirectoryInfo subDI in di.GetDirectories())
			{
				retVal &= RecurseAndSetSecurity(subDI.FullName, SDString);
			}

			// Set permissions for all files in the directory
			foreach( FileInfo fi in di.GetFiles() )
			{
				retVal &= SetPathSecurityDescriptor(fi.FullName, SDString);
			}
			retVal &= SetPathSecurityDescriptor(directory, SDString);
			return retVal;
		}

		internal static unsafe bool SetPathSecurityDescriptor(string path, string SDString)
		{
			void *psd = null;
			try
			{
				// get valid SD
				GetNamedSecurityInfo(path, SE_FILE_OBJECT, DACL_SECURITY_INFORMATION, IntPtr.Zero,IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, &psd);
				
				ulong size = 0;
				// set SD to new SD
				if(!ConvertStringSecurityDescriptorToSecurityDescriptor(SDString, 1, &psd, ref size))
				{
					return false;
				}
				// apply SD to file / directory
				if (!SetFileSecurity(path, DACL_SECURITY_INFORMATION, psd))
				{
					return false;
				}
			}
			finally
			{
				LocalFree(psd);
			}
			return true;
		}

	}
}
