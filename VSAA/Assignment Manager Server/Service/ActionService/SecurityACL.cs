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
using System.IO;

namespace Microsoft.VisualStudio.Academic.AssignmentManager
{
	/// <summary>
	/// Summary description for SecurityReadACL.
	/// </summary>
	internal unsafe class SecurityACL : IDisposable
	{
		SID *puserSID = null;
		internal const int SE_FILE_OBJECT = 1;
		internal const int ACL_REVISION = 2;
		internal const uint DACL_SECURITY_INFORMATION = 0x00000004;
		internal const uint OBJECT_INHERIT_ACE = 0x1;
		internal const uint CONTAINER_INHERIT_ACE = 0x2;
		internal const uint GENERIC_READ = 0x80000000;
		internal const uint GENERIC_WRITE = 0x40000000;
		internal const uint GENERIC_EXECUTE = 0x20000000;
		internal const uint PROTECTED_DACL_SECURITY_INFORMATION = 0x80000000;
		internal const uint MAXDWORD = 0xffffffff;
		internal const uint _maxVersion2AceSize = 152;

		internal SecurityACL(string username)
		{
			// Get SID for user
			puserSID = getUserSid(username);	
		}

		//Implement IDisposable.
		public void Dispose() 
		{
			Dispose(true);
			GC.SuppressFinalize(this); 
		}

		public virtual unsafe void Dispose(bool disposing) 
		{
			LocalFree((void*)puserSID);
		}		

		[DllImport("kernel32.dll")]
		protected unsafe static extern void* LocalFree(void* hMem);

		[DllImport("Kernel32.dll")]
		protected unsafe static extern void* LocalAlloc( uint uFlags, uint uBytes);

		[DllImport("advapi32.dll")]
		protected unsafe static extern int GetAce(
			ACL* pAcl, 
			uint dwAceIndex, 
			out ACE_HEADER* pAce);

		[DllImport("advapi32.dll")]
		protected unsafe static extern int AddAce(
			ACL* pAcl, 
			uint dwAceRevision, 
			uint dwStartingAceIndex, 
			ACE_HEADER* pAceList, 
			uint nAceListLength);

		[DllImport("advapi32.dll")]
		protected unsafe static extern int IsValidSid(SID* pSid);

		protected struct ACE_HEADER
		{
			internal byte AceType;
			internal byte AceFlags;
			internal short AceSize;
		}

		// ACL Struct for P/Invoke
		[StructLayout(LayoutKind.Explicit, Size=152)]
		internal struct ACL
		{
			[FieldOffset(0)]
			internal byte AclRevision;

			[FieldOffset(1)]
			internal byte Sbz1;

			[FieldOffset(2)]
			internal ushort AclSize;

			[FieldOffset(4)]
			internal ushort AceCount;

			[FieldOffset(6)]
			internal ushort Sbz2;
		}

		// SID for P/Invoke
		[StructLayout(LayoutKind.Explicit, Size=12)]
		protected struct SID
		{
			[FieldOffset(0)]
			internal byte Revision;

			[FieldOffset(1)]
			internal byte SubAuthorityCount;

			[FieldOffset(2)]
			internal SID_IDENTIFIER_AUTHORITY IdentifierAuthority;

			//Fixed-length Array "SubAuthority[1]". Members can be
			//accessed with (&my_SID.SubAuthority_1)[index]
			[FieldOffset(8)]
			internal uint SubAuthority_1;
		}

		// P/Invoke struct
		[StructLayout(LayoutKind.Explicit, Size=6)]
		internal struct SID_IDENTIFIER_AUTHORITY
		{
			//Fixed-length Array "Value[6]". Members can be
			//accessed with (&my_SID_IDENTIFIER_AUTHORITY.Value_6)[index]
			[FieldOffset(0)]
			internal byte Value_6;
		}

		protected struct ACL_SIZE_INFORMATION
		{
			internal uint AceCount;
			internal uint AclBytesInUse;
			internal uint AclBytesFree;
		}

		protected enum ACL_INFORMATION_CLASS
		{
			AclRevisionInformation = 1,
			AclSizeInformation = 2
		}

		// P/Invoke function definition
		[DllImport("advapi32.dll")]
		protected unsafe static extern uint GetNamedSecurityInfo(
			string pObjectName, 
			int ObjectType, 
			uint SecurityInfo, 
			IntPtr ppsidOwner, 
			IntPtr ppsidGroup, 
			ACL** ppDacl, 
			IntPtr ppSacl, 
			void** ppSecurityDescriptor);

		// P/Invoke function definition
		[DllImport("advapi32.dll")]
		protected unsafe static extern uint SetNamedSecurityInfoW(
			[MarshalAs(UnmanagedType.LPTStr)]
			string pObjectName,
			int ObjectType,
			uint SecurityInfo,
			IntPtr ppsidOwner,
			IntPtr ppsidGroup,
			[In] ref ACL pDacl,
			IntPtr ppSacl
			);

		// P/Invoke function definition
		[DllImport("advapi32.dll")]
		protected unsafe static extern int LookupAccountName(string lpSystemName, string lpAccountName, void* Sid, uint* cbSid, string ReferencedDomainName, uint* cbReferencedDomainName, SID_NAME_USE* peUse);

		// P/Invoke struct
		protected enum SID_NAME_USE
		{
			SidTypeUser = 1,
			SidTypeGroup = 2,
			SidTypeDomain = 3,
			SidTypeAlias = 4,
			SidTypeWellKnownGroup = 5,
			SidTypeDeletedAccount = 6,
			SidTypeInvalid = 7,
			SidTypeUnknown = 8,
			SidTypeComputer = 9
		}

		// P/Invoke function definition
		[DllImport("advapi32.dll")]
		protected unsafe static extern bool AddAccessAllowedAceEx(
			[In, Out] ACL *pAcl,
			uint dwAceRevision,
			uint AceFlags,
			uint AccessMask,
			[In] ref SID pSid
			);

		// P/Invoke function definition
		[DllImport("advapi32.dll")]
		protected unsafe static extern bool InitializeAcl(
			[In, Out] ref ACL pAcl,
			uint nAclLength,
			uint dwAclRevision
			);

		// P/Invoke function definition
		[DllImport("advapi32.dll")]
		protected unsafe static extern int GetAclInformation(
			ACL* pAcl, 
			out ACL_SIZE_INFORMATION pAclInformation, 
			uint nAclInformationLength, 
			ACL_INFORMATION_CLASS dwAclInformationClass);



		internal unsafe void ApplyACLToFile(string filename)
		{
			
			ACL* pdaclOld = null;
			void* psd = null;
			ACL *pdacl = null;
			try
			{								
				if (!File.Exists(filename))
				{
					// Drive does not exist (i.e. removeable media)
					throw new Exception();
				}
				
				//Get DACL for the current drive.
				GetNamedSecurityInfo(filename, SE_FILE_OBJECT, DACL_SECURITY_INFORMATION, IntPtr.Zero,IntPtr.Zero, &pdaclOld, IntPtr.Zero, &psd);
				if (pdaclOld == null)
				{
					// It is a FAT32 drive and does not have permissions.
					throw new Exception();
				}
	
				// Files and Folders inherit all ACE's
				uint grfInherit = OBJECT_INHERIT_ACE | CONTAINER_INHERIT_ACE;

				// Insert Access Denied for userSID
				pdacl = insertAccessAllowedAce(pdaclOld, GENERIC_READ | GENERIC_EXECUTE | GENERIC_WRITE, grfInherit, puserSID);

				// Set the final ACL for the drive.
				SetNamedSecurityInfoW(filename, SE_FILE_OBJECT, DACL_SECURITY_INFORMATION | PROTECTED_DACL_SECURITY_INFORMATION, IntPtr.Zero, IntPtr.Zero, ref (*pdacl), IntPtr.Zero);
			}
			catch(Exception)
			{
				// failure most likely indicates a FAT32 drive, so we can just continue.
			}
			finally
			{
					
				LocalFree((void*)pdacl);
				LocalFree(psd);
			}
		}

		// Get User's SID from UserName
		protected unsafe SID *getUserSid(string szUserName)
		{

			string szDomain = null;
			uint cbDomain = 0;
			uint cbUserSID = 0;
			SID *pUserSID = null;
			SID_NAME_USE snuType;

			int fAPISuccess = LookupAccountName(null, szUserName,
				pUserSID, &cbUserSID, szDomain, &cbDomain, &snuType);    
			if (fAPISuccess != 0)
			{
				// It worked.  There is no way this will happen.
				return pUserSID;
			}

			pUserSID = (SID *)LocalAlloc(0, cbUserSID);
			if (pUserSID == null) 
			{
				throw new Exception();
			}      

			szDomain = new String('a',(int)cbDomain);
			LookupAccountName(null, szUserName, pUserSID, &cbUserSID, szDomain, &cbDomain, &snuType);
			if (IsValidSid(pUserSID) == 0)
			{
				throw new Exception();
			}		

			return pUserSID;
		}

		protected unsafe ACL* insertAccessAllowedAce(ACL *pdaclOld, uint grfMask, uint grfInherit, SID *psid)
		{
			ACL_SIZE_INFORMATION si;
			uint size = (uint) sizeof(ACL_SIZE_INFORMATION);

			if (GetAclInformation(pdaclOld, out si, size, ACL_INFORMATION_CLASS.AclSizeInformation) == 0)
			{
				throw new Exception();
			}
	
			uint cb = si.AclBytesInUse + _maxVersion2AceSize;
			ACL *pdaclNew = (ACL*)LocalAlloc(0, cb);
			InitializeAcl(ref (*pdaclNew), cb, ACL_REVISION);

			if (!AddAccessAllowedAceEx(pdaclNew, ACL_REVISION, grfInherit, grfMask, ref *psid))
			{
				throw new Exception();
			}

			for (uint i=0; i< si.AceCount; ++i)
			{
				ACE_HEADER *pace;
				GetAce(pdaclOld, i, out pace);
				AddAce(pdaclNew, ACL_REVISION, MAXDWORD, pace, (uint)pace->AceSize);
			}
			return pdaclNew;
		}

	}
}
