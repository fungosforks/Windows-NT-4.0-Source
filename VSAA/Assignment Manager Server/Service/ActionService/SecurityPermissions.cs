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

namespace Microsoft.VisualStudio.Academic.AssignmentManager.ActionService
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

		// ACL Struct for P/Invoke
		[StructLayout(LayoutKind.Explicit, Size=152)]
			public struct ACL
		{
			[FieldOffset(0)]
			public byte AclRevision;

			[FieldOffset(1)]
			public byte Sbz1;

			[FieldOffset(2)]
			public ushort AclSize;

			[FieldOffset(4)]
			public ushort AceCount;

			[FieldOffset(6)]
			public ushort Sbz2;
		}

		// SID for P/Invoke
		[StructLayout(LayoutKind.Explicit, Size=12)]
			public struct SID
		{
			[FieldOffset(0)]
			public byte Revision;

			[FieldOffset(1)]
			public byte SubAuthorityCount;

			[FieldOffset(2)]
			public SID_IDENTIFIER_AUTHORITY IdentifierAuthority;

			//Fixed-length Array "SubAuthority[1]". Members can be
			//accessed with (&my_SID.SubAuthority_1)[index]
			[FieldOffset(8)]
			public uint SubAuthority_1;
		}

		protected struct ACL_SIZE_INFORMATION
		{
			public uint AceCount;
			public uint AclBytesInUse;
			public uint AclBytesFree;
		}

		// P/Invoke struct
		[StructLayout(LayoutKind.Explicit, Size=6)]
			public struct SID_IDENTIFIER_AUTHORITY
		{
			//Fixed-length Array "Value[6]". Members can be
			//accessed with (&my_SID_IDENTIFIER_AUTHORITY.Value_6)[index]
			[FieldOffset(0)]
			public byte Value_6;
		}

		// P/Invoke function definition
		[DllImport("advapi32.dll")]
		protected unsafe static extern bool InitializeAcl(
			[In, Out] ref ACL pAcl,
			uint nAclLength,
			uint dwAclRevision
			);

		[DllImport("advapi32.dll")]
		protected unsafe static extern bool IsValidSid(SID* pSid);

		[DllImport("Kernel32.dll")]
		protected unsafe static extern void* LocalAlloc( uint uFlags, uint uBytes);

		[DllImport("kernel32.dll")]
		protected unsafe static extern void* LocalFree(void* hMem);

		[DllImport("advapi32.dll")]
		public static extern unsafe bool InitializeSecurityDescriptor(
			void *pSecurityDescriptor,
			uint revision
			);

		[DllImport("advapi32.dll")]
		public static extern unsafe bool SetSecurityDescriptorDacl(
			void *pSecurityDescriptor,
			bool bDaclPresent,
			ACL *pDacl,
			bool bDaclDefaulted);

		[DllImport("Mqrt.dll")]
		public static extern unsafe uint MQSetQueueSecurity(
			[MarshalAs(UnmanagedType.LPWStr)]
			string lpwcsFormatName,
			uint SecurityInformation,
			void *SecurityDescriptor
			);

		[DllImport("Mqrt.dll")]
		public static extern unsafe uint MQGetQueueSecurity(
			[MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
			string lpwcsFormatName,
			uint SecurityInformation,
			void *SecurityDescriptor,
			uint nLength,
			out uint lpnLengthNeeded);

		[DllImport("advapi32.dll")]
		protected unsafe static extern bool IsValidSecurityDescriptor(
			void *pSecurityDescriptor
			);

		[DllImport("advapi32.dll")]
		protected unsafe static extern bool AllocateAndInitializeSid(
			SID_IDENTIFIER_AUTHORITY *pIdentifierAuthority,
			byte nSubAuthorityCount,
			uint dwSubAuthority0,
			uint dwSubAuthority1,
			uint dwSubAuthority2,
			uint dwSubAuthority3,
			uint dwSubAuthority4,
			uint dwSubAuthority5,
			uint dwSubAuthority6,
			uint dwSubAuthority8,
			out SID *pSid
			);

		// P/Invoke function definition
		[DllImport("advapi32.dll")]
		protected unsafe static extern bool AddAccessAllowedAceEx(
			[In, Out] ACL *pAcl,
			uint dwAceRevision,
			uint AceFlags,
			uint AccessMask,
			SID* pSid
			);

		private SecurityPermissions()
		{
			// Make class non-createable
		}

		// ACLQueue:
		//	 - Local System - Full Control
		//   - Administrators - Full Control
		internal unsafe static bool ACLQueue(string messageQueue)
		{
			messageQueue = @messageQueue;
			ACL_SIZE_INFORMATION si = new ACL_SIZE_INFORMATION();
			uint size = (uint) sizeof(ACL_SIZE_INFORMATION);
			uint cb = si.AclBytesInUse + _maxVersion2AceSize;

			// Files and Folders inherit all ACE's
			uint grfInherit = OBJECT_INHERIT_ACE | CONTAINER_INHERIT_ACE;

			SID *pAdminSID = null;
			SID *pSystemSID = null;
			ACL *pdacl = null;
			void *pSD = null;
			try
			{
				SID_IDENTIFIER_AUTHORITY SIDAuthNT = new SID_IDENTIFIER_AUTHORITY();
				// Defined in winnt.h
				(&SIDAuthNT.Value_6)[0] = 0;
				(&SIDAuthNT.Value_6)[1] = 0;
				(&SIDAuthNT.Value_6)[2] = 0;
				(&SIDAuthNT.Value_6)[3] = 0;
				(&SIDAuthNT.Value_6)[4] = 0;
				(&SIDAuthNT.Value_6)[5] = 5;

				uint SECURITY_BUILTIN_DOMAIN_RID = 0x00000020;   //defined in winnt.h
				uint DOMAIN_ALIAS_RID_ADMINS = 0x00000220;  // defined in winnt.h
				uint SECURITY_LOCAL_SYSTEM_RID = 0x00000012; // defined in winnt.h

				ACL *pdaclNew = (ACL*)LocalAlloc(0,cb);
				InitializeAcl(ref (*pdaclNew), cb, ACL_REVISION);
				
				// Administrators Full Control
				if (AllocateAndInitializeSid(&SIDAuthNT, 2, SECURITY_BUILTIN_DOMAIN_RID, DOMAIN_ALIAS_RID_ADMINS, 0, 0, 0, 0, 0, 0, out pAdminSID))
				{
					if (IsValidSid(pAdminSID))
					{

						if (!AddAccessAllowedAceEx(pdaclNew, ACL_REVISION, grfInherit, MQSEC_QUEUE_GENERIC_ALL, pAdminSID))
						{
							throw new Exception();
						}
					}
				}

				// Local System Full Control
				if (AllocateAndInitializeSid(&SIDAuthNT, 1, SECURITY_LOCAL_SYSTEM_RID, 0, 0, 0, 0, 0, 0, 0, out pSystemSID))
				{
					if (IsValidSid(pSystemSID))
					{
						if (!AddAccessAllowedAceEx(pdaclNew, ACL_REVISION, grfInherit, MQSEC_QUEUE_GENERIC_ALL, pSystemSID))
						{
							throw new Exception();
						}
					}
				}
			
				pSD = (void *)LocalAlloc(0, 200);
				if (!InitializeSecurityDescriptor(pSD, 1))
				{
					throw new Exception();
				}
				if (!SetSecurityDescriptorDacl(pSD, true, pdaclNew, false))
				{
					throw new Exception();
				}
				if (!IsValidSecurityDescriptor(pSD))
				{
					throw new Exception();
				}			
	
				MessageQueue mq = new MessageQueue(messageQueue);
				if (MQSetQueueSecurity(mq.FormatName, DACL_SECURITY_INFORMATION, pSD) != 0)
				{
					throw new Exception();
				}
			}
			catch
			{
				return false;
			}
			finally
			{
				if (pSD != null)
				{
					LocalFree(pSD);
				}
				if (pdacl != null)
				{
					LocalFree(pdacl);
				}
				if (pAdminSID != null)
				{
					LocalFree(pAdminSID);
				}
				if (pSystemSID != null)
				{
					LocalFree(pSystemSID);
				}
			}
			return true;
		}
	}
}
