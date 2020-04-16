//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//

namespace AMInstall
{
	using System;
	using System.Security;
	using System.Runtime.InteropServices;
	using System.Security.Cryptography;

	internal class AMUser
	{
		public const string AM_USER_NAME = "AssignmentManager";
		public const string LSA_PASSWORD_KEY = "L$AssnMgrPassword";

		private AMUser() 
		{
			// Make class non-creatable.
		}

		public static unsafe bool CreateUser()
		{
			int returnCode = 0, errorCode = 0;
			string serverName = null; // local machine.
			Win32.USER_INFO_1 amUser = new Win32.USER_INFO_1();
			CreateUserLevel1(ref amUser);
			LSAUtil lsaUtil = new LSAUtil(LSA_PASSWORD_KEY);
			lsaUtil.StoreEncryptedString(amUser.usri1_password);
			returnCode = Win32.NetUserAdd(serverName, 1, ref amUser, errorCode);
			return ( returnCode == Win32.NERR_Success );
		}
		
		public static unsafe bool DeleteUser()
		{
			string serverName = null; // local machine.
			string userName = AM_USER_NAME;
			int returnCode = Win32.NetUserDel(serverName, userName);
			
			LSAUtil lsaUtil = new LSAUtil(LSA_PASSWORD_KEY);
			lsaUtil.DeletePrivateKey();

			return ( returnCode == Win32.NERR_Success );
		}
		
		public static unsafe bool AddUserToGroup()
		{
			try
			{
				Win32.LOCALGROUP_MEMBERS_INFO_3 grpMembers = new Win32.LOCALGROUP_MEMBERS_INFO_3();
				string groupName = GetUsersGroupName();
			
				if (groupName == String.Empty) 
				{
					return false;
				}

				// syntax is:  domain\username -- however leaving domain blank = localhost
				grpMembers.lgrmi3_domainandname = "\\"+AM_USER_NAME;
				int returnCode = Win32.NetLocalGroupAddMembers(null, groupName, 3, ref grpMembers, 1);
				return ( returnCode == Win32.NERR_Success );
			}
			catch(Exception)
			{
				return false;
			}
		}

		internal static unsafe string GetUsersGroupName()
		{
			System.IntPtr TheSID;
			System.UInt32 nameSize, domainSize;
			Win32.SID_NAME_USE sidNameUse;
			Win32.SID_IDENTIFIER_AUTHORITY SECURITY_NT_AUTHORITY;
			byte []name, domain;

			name = null;
			domain = null;
            TheSID = IntPtr.Zero;
			
			try 
			{
				SECURITY_NT_AUTHORITY = new Win32.SID_IDENTIFIER_AUTHORITY();
				// setting 6-byte value to 5
				(&SECURITY_NT_AUTHORITY.Value_6)[0] = 0;
				(&SECURITY_NT_AUTHORITY.Value_6)[1] = 0;
				(&SECURITY_NT_AUTHORITY.Value_6)[2] = 0;
				(&SECURITY_NT_AUTHORITY.Value_6)[3] = 0;
				(&SECURITY_NT_AUTHORITY.Value_6)[4] = 0;
				(&SECURITY_NT_AUTHORITY.Value_6)[5] = 5;

				if (!Win32.AllocateAndInitializeSid(ref SECURITY_NT_AUTHORITY,
					2, Win32.SECURITY_BUILTIN_DOMAIN_RID,
					Win32.DOMAIN_ALIAS_RID_USERS,
					0, 0, 0, 0, 0, 0, out TheSID))
				{
					return String.Empty;
				}

				// Get the size of the name / domain before attempting to retrieve them
				nameSize = domainSize = 0;
				Win32.LookupAccountSidW(null, TheSID, name, ref nameSize, domain, ref domainSize, out sidNameUse);
				if (nameSize == 0) 
				{
					return String.Empty;
				}

				name = new byte[nameSize*2];
				domain = new byte[domainSize*2];
				if (!Win32.LookupAccountSidW(null, TheSID, name, ref nameSize, domain, ref domainSize, out sidNameUse)) 
				{
					return String.Empty;
				}
			} 
			finally
			{
				if (TheSID != IntPtr.Zero) 
				{
					Win32.FreeSid(TheSID);
				}
			}
			
			if (name != null) 
			{
				return System.Text.UnicodeEncoding.Unicode.GetString(name);
			} 
			else
			{
				return String.Empty;
			}
		}


		private static void CreateUserLevel1(ref Win32.USER_INFO_1 AMUser) 
		{
			AMUser.usri1_name = AM_USER_NAME; //Get the UserName constant.
			AMUser.usri1_password = CreatePassword(30); // Create a strong password of length 30
			AMUser.usri1_home_dir = null;
			//CONSIDER:  Adding SharedSupport for localizing this string.
			AMUser.usri1_comment = "Used by the Assignment Manager server to run the Visual Studio .NET command line compiler.";
			AMUser.usri1_flags = Win32.UF_PASSWD_CANT_CHANGE + Win32.UF_DONT_EXPIRE_PASSWD + Win32.UF_SCRIPT + Win32.UF_NORMAL_ACCOUNT;
			AMUser.usri1_script_path = null;
			AMUser.usri1_priv = Win32.USER_PRIV_USER;

			return;
		}

		// Genearate a secure password:
		// password must contain at least one each of: 
		// uppercase, lowercase, punctuation and numbers
		internal static string CreatePassword(int pwLength) 
		{
			const int STRONG_PWD_UPPER = 0;
			const int STRONG_PWD_LOWER = 1;
			const int STRONG_PWD_NUM = 2;
			const int STRONG_PWD_PUNC = 3;


			const int STRONG_PWD_CATS = (STRONG_PWD_PUNC + 1);
			const int NUM_LETTERS = 26;
			const int NUM_NUMBERS = 10;
			const int MIN_PWD_LEN = 8;

			if (pwLength-1 < MIN_PWD_LEN)
				return "";

			byte[] szPwd = new byte[pwLength];

			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			// generate a pwd pattern, each byte is in the range 
			// (0..255) mod STRONG_PWD_CATS
			// this indicates which character pool to take a char from
			byte[] pPwdPattern = new byte[pwLength];
			bool[] fFound = new bool[STRONG_PWD_CATS];
			do 
			{
				rng.GetBytes(pPwdPattern);

				fFound[STRONG_PWD_UPPER] = 
					fFound[STRONG_PWD_LOWER] =
					fFound[STRONG_PWD_PUNC] =
					fFound[STRONG_PWD_NUM] = false;

				for (int i=0; i < pwLength; i++) 
					fFound[pPwdPattern[i] % STRONG_PWD_CATS] = true;

				// check that each character category is in the pattern
			} while (!fFound[STRONG_PWD_UPPER] || 
				!fFound[STRONG_PWD_LOWER] || 
				!fFound[STRONG_PWD_PUNC] || 
				!fFound[STRONG_PWD_NUM]);

			// populate password with random data 
			// this, in conjunction with pPwdPattern, is
			// used to determine the actual data
			rng.GetBytes(szPwd);

			for (int i=0; i < pwLength; i++) 
			{ 
				byte bChar = 0;

				// there is a bias in each character pool because of the % function
				switch (pPwdPattern[i] % STRONG_PWD_CATS) 
				{

					case STRONG_PWD_UPPER : bChar = (byte)('A' + szPwd[i] % NUM_LETTERS);
						break;

					case STRONG_PWD_LOWER : bChar = (byte)('a' + szPwd[i] % NUM_LETTERS);
						break;

					case STRONG_PWD_NUM :   bChar = (byte)('0' + szPwd[i] % NUM_NUMBERS);
						break;

					case STRONG_PWD_PUNC :
					default:                
						string szPunc="!@#$%^&*()_-+=[{]};:\'\"<>,./?\\|~`";
						int dwLenPunc = szPunc.Length;
						bChar = (byte)(szPunc[szPwd[i] % dwLenPunc]);
						break;
				}

				szPwd[i] = bChar;
			}
			
			char[] pass = new char[szPwd.Length];
			for (int i=0;i<szPwd.Length; i++)
			{
				pass[i] = (char)szPwd[i];
			}
			return new string(pass);
		}


		private class Win32 
		{

			public struct USER_INFO_1 
			{
				//UserInfo Level 1, supported by Win2K and XP.
				[MarshalAs(UnmanagedType.LPWStr)]
				public string usri1_name;	//LPWSTR    
				[MarshalAs(UnmanagedType.LPWStr)]
				public string usri1_password;	//LPWSTR    
				public int usri1_password_age;	//DWORD
				public int usri1_priv;	//DWORD
				[MarshalAs(UnmanagedType.LPWStr)]
				public string usri1_home_dir;	//LPWSTR
				[MarshalAs(UnmanagedType.LPWStr)]
				public string usri1_comment;	//LPWSTR
				public int usri1_flags;	//DWORD
				[MarshalAs(UnmanagedType.LPWStr)]
				public string usri1_script_path;	//LPWSTR
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct GROUP_INFO_3
			{
				[MarshalAs(UnmanagedType.LPWStr)]
				public string grpi3_name; // LPWSTR
				[MarshalAs(UnmanagedType.LPWStr)]
				public string grpi3_comment; // LPWSTR
				public int grpi3_group_sid; //PSID
				public int grpi3_attributes; //DWORD
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct LOCALGROUP_MEMBERS_INFO_3
			{
				[MarshalAs(UnmanagedType.LPWStr)]
				public string lgrmi3_domainandname;
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

			//NetUserAdd
			[DllImport("netapi32.dll")]
			public static unsafe extern int NetUserAdd([In, MarshalAs(UnmanagedType.LPWStr)]string lpServer, int Level, [In] ref USER_INFO_1 userInfo, int lpError);

			//NetUserDel
			[DllImport("netapi32.dll")]
			public static unsafe extern int NetUserDel([In, MarshalAs(UnmanagedType.LPWStr)]string servername, [In, MarshalAs(UnmanagedType.LPWStr)]string username);

			//NetGroupAddUser
			[DllImport("netapi32.dll")]
			public static unsafe extern int NetLocalGroupAddMembers(
				[In, MarshalAs(UnmanagedType.LPWStr)] string serverName, 
				[In, MarshalAs(UnmanagedType.LPWStr)] string groupName, 
				uint level,
				ref LOCALGROUP_MEMBERS_INFO_3 buf,
				uint totalentries
				);

			public enum SID_NAME_USE 
			{
				SidTypeUser = 1,
				SidTypeGroup,
				SidTypeDomain,
				SidTypeAlias,
				SidTypeWellKnownGroup,
				SidTypeDeletedAccount,
				SidTypeInvalid,
				SidTypeUnknown,
				SidTypeComputer
			}

			[DllImport("advapi32.dll")]
			internal unsafe static extern bool LookupAccountSidW(
				[MarshalAs(UnmanagedType.LPWStr), In]
				string lpSystemName,  // name of local or remote computer
				[In] System.IntPtr Sid,              // security identifier
				[In, Out] byte []Name,           // account name buffer
				[In, Out] ref System.UInt32 cbName,        // size of account name buffer
				[In, Out] byte []DomainName,     // domain name
				[In, Out] ref System.UInt32 cbDomainName,  // size of domain name buffer
				[Out] out SID_NAME_USE peUse    // SID type
				);

			[DllImport("advapi32.dll")]
			internal unsafe static extern bool AllocateAndInitializeSid(
				[In] ref SID_IDENTIFIER_AUTHORITY pIdentifierAuthority, // authority
				byte nSubAuthorityCount,                        // count of subauthorities
				System.UInt32 dwSubAuthority0,                          // subauthority 0
				System.UInt32 dwSubAuthority1,                          // subauthority 1
				System.UInt32 dwSubAuthority2,                          // subauthority 2
				System.UInt32 dwSubAuthority3,                          // subauthority 3
				System.UInt32 dwSubAuthority4,                          // subauthority 4
				System.UInt32 dwSubAuthority5,                          // subauthority 5
				System.UInt32 dwSubAuthority6,                          // subauthority 6
				System.UInt32 dwSubAuthority7,                          // subauthority 7
				[Out] out System.IntPtr pSid                                      // SID
				);
		
			[DllImport("advapi32.dll")]
			internal unsafe static extern System.IntPtr FreeSid(
				System.IntPtr mem // handle to SID
				);

			//WIN32 API Return Codes for NetUserAdd().
			internal const int NERR_Success = 0;
			internal const int NERR_BASE = 2100;
			internal const int NERR_InvalidComputer = (NERR_BASE + 251);	//The computer name is invalid. 
			internal const int NERR_NotPrimary = (NERR_BASE + 126);		//The operation is allowed only on the primary domain controller of the domain. 
			internal const int NERR_GroupNotFound = (NERR_BASE + 120);  /* The group name could not be found. */
			internal const int NERR_GroupExists = (NERR_BASE + 123);		//The group already exists. 
			internal const int NERR_UserExists = (NERR_BASE + 124);		//The user account already exists. 
			internal const int NERR_PasswordTooShort = (NERR_BASE + 145); //The password is shorter than required. (The password could also be too long, be too recent in its change history, not have enough unique characters, or not meet another password policy requirement.)
			internal const int ERROR_ACCESS_DENIED = 5;	//The user does not have access to the requested information. 

			internal const int UF_SCRIPT = 0x01;
			internal const int UF_ACCOUNTDISABLE = 0x2;
			internal const int UF_HOMEDIR_REQUIRED = 0x8;
			internal const int UF_LOCKOUT = 0x10;
			internal const int UF_PASSWD_NOTREQD = 0x20;
			internal const int UF_PASSWD_CANT_CHANGE = 0x40;
			internal const int UF_TEMP_DUPLICATE_ACCOUNT = 0x100;
			internal const int UF_NORMAL_ACCOUNT = 0x200;
			internal const int UF_INTERDOMAIN_TRUST_ACCOUNT = 0x800;
			internal const int UF_WORKSTATION_TRUST_ACCOUNT = 0x1000;
			internal const int UF_SERVER_TRUST_ACCOUNT = 0x2000;
			internal const int UF_DONT_EXPIRE_PASSWD = 0x10000;
			internal const int UF_MNS_LOGON_ACCOUNT = 0x20000;

			internal const int TIMEQ_FOREVER = -1;
			internal const int USER_MAXSTORAGE_UNLIMITED = -1;
			internal const int USER_PRIV_USER = 1;
			internal const int DOMAIN_GROUP_RID_USERS = 0x201;

			internal const int MAX_PREFERRED_LENGTH = -1; //Indicates the API should just allocate as much space as it takes.
			internal const System.UInt32 SECURITY_BUILTIN_DOMAIN_RID = 0x20;
			internal const System.UInt32 DOMAIN_ALIAS_RID_USERS = 0x221;
		}
	}
}