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

namespace AMInstall
{
	/// <summary>
	/// Summary description for LSAUtil.
	/// </summary>
	internal class LSAUtil
	{
		/// <summary>
		/// Name of the private key to store the LSA secret into
		/// </summary>
		private readonly string PrivateKeyString;

		/// <summary>
		/// Following are a series of access permission standard constants, 
		/// ported from winnt.h and ntsecapi.h.
		/// </summary>
		private static readonly System.UInt32 DELETE                           = (0x00010000);
		private static readonly System.UInt32 READ_CONTROL                     = (0x00020000);
		private static readonly System.UInt32 WRITE_DAC                        = (0x00040000);
		private static readonly System.UInt32 WRITE_OWNER                      = (0x00080000);
		private static readonly System.UInt32 SYNCHRONIZE                      = (0x00100000);
		private static readonly System.UInt32 STANDARD_RIGHTS_REQUIRED         = (0x000F0000);
		private static readonly System.UInt32 STANDARD_RIGHTS_READ             = (READ_CONTROL);
		private static readonly System.UInt32 STANDARD_RIGHTS_WRITE            = (READ_CONTROL);
		private static readonly System.UInt32 STANDARD_RIGHTS_EXECUTE          = (READ_CONTROL);
		private static readonly System.UInt32 STANDARD_RIGHTS_ALL              = (0x001F0000);
		private static readonly System.UInt32 SPECIFIC_RIGHTS_ALL              = (0x0000FFFF);

		private static readonly System.UInt32 POLICY_VIEW_LOCAL_INFORMATION              = 0x00000001;
		private static readonly System.UInt32 POLICY_VIEW_AUDIT_INFORMATION              = 0x00000002;
		private static readonly System.UInt32 POLICY_GET_PRIVATE_INFORMATION             = 0x00000004;
		private static readonly System.UInt32 POLICY_TRUST_ADMIN                         = 0x00000008;
		private static readonly System.UInt32 POLICY_CREATE_ACCOUNT                      = 0x00000010;
		private static readonly System.UInt32 POLICY_CREATE_SECRET                       = 0x00000020;
		private static readonly System.UInt32 POLICY_CREATE_PRIVILEGE                    = 0x00000040;
		private static readonly System.UInt32 POLICY_SET_DEFAULT_QUOTA_LIMITS            = 0x00000080;
		private static readonly System.UInt32 POLICY_SET_AUDIT_REQUIREMENTS              = 0x00000100;
		private static readonly System.UInt32 POLICY_AUDIT_LOG_ADMIN                     = 0x00000200;
		private static readonly System.UInt32 POLICY_SERVER_ADMIN                        = 0x00000400;
		private static readonly System.UInt32 POLICY_LOOKUP_NAMES                        = 0x00000800;
		private static readonly System.UInt32 POLICY_NOTIFICATION                        = 0x00001000;

		[StructLayout(LayoutKind.Sequential)]
		internal struct LSA_UNICODE_STRING 
		{
			internal ushort Length;
			internal ushort MaximumLength;
			[MarshalAs(UnmanagedType.LPWStr)]
			internal string Buffer;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal class LSA_OBJECT_ATTRIBUTES 
		{
			internal System.UInt32 Length;
			internal System.IntPtr RootDirectory;
			[MarshalAs(UnmanagedType.LPWStr)]
			internal System.String ObjectName;
			internal System.UInt32 Attributes;
			internal System.IntPtr SecurityDescriptor;
			internal System.IntPtr SecurityQualityOfService;
		}

		private static readonly System.UInt32 POLICY_ALL_ACCESS     = (STANDARD_RIGHTS_REQUIRED         |
			POLICY_VIEW_LOCAL_INFORMATION    |
			POLICY_VIEW_AUDIT_INFORMATION    |
			POLICY_GET_PRIVATE_INFORMATION   |
			POLICY_TRUST_ADMIN               |
			POLICY_CREATE_ACCOUNT            |
			POLICY_CREATE_SECRET             |
			POLICY_CREATE_PRIVILEGE          |
			POLICY_SET_DEFAULT_QUOTA_LIMITS  |
			POLICY_SET_AUDIT_REQUIREMENTS    |
			POLICY_AUDIT_LOG_ADMIN           |
			POLICY_SERVER_ADMIN              |
			POLICY_LOOKUP_NAMES);

		private static readonly System.UInt32 POLICY_READ           = (STANDARD_RIGHTS_READ             |
			POLICY_VIEW_AUDIT_INFORMATION    |
			POLICY_GET_PRIVATE_INFORMATION);
    
		private static readonly System.UInt32 POLICY_WRITE          = (STANDARD_RIGHTS_WRITE            |
			POLICY_TRUST_ADMIN               |
			POLICY_CREATE_ACCOUNT            |
			POLICY_CREATE_SECRET             |
			POLICY_CREATE_PRIVILEGE          |
			POLICY_SET_DEFAULT_QUOTA_LIMITS  |
			POLICY_SET_AUDIT_REQUIREMENTS    |
			POLICY_AUDIT_LOG_ADMIN           |
			POLICY_SERVER_ADMIN);
    
		private static readonly System.UInt32 POLICY_EXECUTE        = (STANDARD_RIGHTS_EXECUTE          |
			POLICY_VIEW_LOCAL_INFORMATION    |
			POLICY_LOOKUP_NAMES);
    
		[DllImport("advapi32.dll")]
		private unsafe static extern int LsaOpenPolicy(
			[In]
			ref LSA_UNICODE_STRING SystemName,
			[In, MarshalAs(UnmanagedType.LPStruct)]
			LSA_OBJECT_ATTRIBUTES Attributes,
			[In]
			System.UInt32 DesiredAccess,
			[Out]
			out System.IntPtr PolicyHandle
			);

		[DllImport("advapi32.dll")]
		private static extern int LsaClose(
			[In]
			System.IntPtr ObjectHandle
			);

		[DllImport("advapi32.dll")]
		private unsafe static extern int LsaRetrievePrivateData(
			[In]
			System.IntPtr PolicyHandle,
			[In]
			ref LSA_UNICODE_STRING KeyName,
			[Out]
			out void *PrivateData
			);       

		[DllImport("advapi32.dll")]
		private unsafe static extern int LsaStorePrivateData(
			[In]
			System.IntPtr PolicyHandle,
			[In]
			ref LSA_UNICODE_STRING KeyName,
			[In]
			ref LSA_UNICODE_STRING PrivateData
			);

		[DllImport("advapi32.dll")]
		private static extern ulong LsaNtStatusToWinError(int Status);

		[DllImport("advapi32.dll")]
		private unsafe static extern ulong LsaFreeMemory(void *Buffer);

		internal LSAUtil()
		{
			PrivateKeyString = "L$AssnMgrConnectString";
		}

		internal LSAUtil(string KeyString)
		{
			PrivateKeyString = KeyString;
		}

		internal unsafe void StoreEncryptedString(string s) 
		{
			System.IntPtr PolicyHandle;
			LSA_UNICODE_STRING Secret = new LSA_UNICODE_STRING();
			LSA_UNICODE_STRING PrivateKeyName = new LSA_UNICODE_STRING();
			LSA_UNICODE_STRING NullString = new LSA_UNICODE_STRING();
			int ReturnValue = 0;

			NullString.Buffer = String.Empty;
			NullString.Length = NullString.MaximumLength = 0;

			ReturnValue = LsaOpenPolicy(ref NullString, new LSA_OBJECT_ATTRIBUTES(), POLICY_WRITE, out PolicyHandle);

			if (ReturnValue != 0) 
			{
				ulong error = LsaNtStatusToWinError(ReturnValue);
				throw new System.Security.SecurityException("Unable to open the local LSA policy. " + error.ToString());
			}

			Secret.Buffer = s;
			Secret.Length = (ushort)(Secret.Buffer.Length * 2);
			Secret.MaximumLength = (ushort)((Secret.Buffer.Length + 1) * 2);
			PrivateKeyName.Buffer = PrivateKeyString;
			PrivateKeyName.Length = (ushort)(PrivateKeyName.Buffer.Length * 2);
			PrivateKeyName.MaximumLength = (ushort)((PrivateKeyName.Buffer.Length + 1) * 2);

			ReturnValue = LsaStorePrivateData(PolicyHandle, ref PrivateKeyName, ref Secret);

			LsaClose(PolicyHandle);

			if (ReturnValue != 0) 
			{
				ulong error = LsaNtStatusToWinError(ReturnValue);
				throw new System.Security.SecurityException("Unable to store private data. " + error.ToString());
			}
		}

		internal unsafe void DeletePrivateKey()
		{
			System.IntPtr PolicyHandle;
			LSA_UNICODE_STRING PrivateKeyName = new LSA_UNICODE_STRING();
			LSA_UNICODE_STRING NullString = new LSA_UNICODE_STRING();
			int ReturnValue = 0;

			NullString.Buffer = String.Empty;
			NullString.Length = NullString.MaximumLength = 0;

			ReturnValue = LsaOpenPolicy(ref NullString, new LSA_OBJECT_ATTRIBUTES(), POLICY_WRITE, out PolicyHandle);

			if (ReturnValue != 0) 
			{
				ulong error = LsaNtStatusToWinError(ReturnValue);
				throw new System.Security.SecurityException("Unable to open the local LSA policy. " + error.ToString());
			}

			PrivateKeyName.Buffer = PrivateKeyString;
			PrivateKeyName.Length = (ushort)(PrivateKeyName.Buffer.Length * 2);
			PrivateKeyName.MaximumLength = (ushort)((PrivateKeyName.Buffer.Length + 1) * 2);

			ReturnValue = LsaStorePrivateData(PolicyHandle, ref PrivateKeyName, ref NullString);

			if (ReturnValue != 0) 
			{
				ulong error = LsaNtStatusToWinError(ReturnValue);
				throw new System.Security.SecurityException("Unable to store private data. " + error.ToString());
			}
		}

		internal unsafe string RetrieveEncryptedString() 
		{
			System.IntPtr PolicyHandle;
			LSA_UNICODE_STRING PrivateKeyName = new LSA_UNICODE_STRING();
			LSA_UNICODE_STRING NullString = new LSA_UNICODE_STRING();
			int ReturnValue = 0;
			void *Secret = null;
			ushort *Data = null;
			string Result = String.Empty;

			NullString.Buffer = String.Empty;
			NullString.Length = NullString.MaximumLength = 0;

			ReturnValue = LsaOpenPolicy(ref NullString, new LSA_OBJECT_ATTRIBUTES(), POLICY_READ, out PolicyHandle);

			if (ReturnValue != 0) 
			{
				ulong error = LsaNtStatusToWinError(ReturnValue);
				throw new System.Security.SecurityException("Unable to open the local LSA policy. " + error.ToString());
			}

			PrivateKeyName.Buffer = PrivateKeyString;
			PrivateKeyName.Length = (ushort)(PrivateKeyName.Buffer.Length * 2);
			PrivateKeyName.MaximumLength = (ushort)((PrivateKeyName.Buffer.Length + 1) * 2);

			ReturnValue = LsaRetrievePrivateData(PolicyHandle, ref PrivateKeyName, out Secret);

			LsaClose(PolicyHandle);

			if (ReturnValue != 0) 
			{
				ulong error = LsaNtStatusToWinError(ReturnValue);
				throw new System.Security.SecurityException("Unable to retrieve private data. " + error.ToString());
			}

			Data = (ushort *)Secret;
			if (Data != null) 
			{        
				// The first ushort is the length; the second is the maxlength (buffer size). The
				// third is the pointer to the unicode string data.
				ushort *StringData = *((ushort**)(&(Data[2])));

				int Length = (Data[0] / 2) - 1; // Length is in bytes, not unicode characters
				for (int i = 0; i <= Length; i++) 
				{
					Result += Convert.ToChar(StringData[i]).ToString();
				}
			}

			LsaFreeMemory(Secret);

			return Result;
		}
	}
}
