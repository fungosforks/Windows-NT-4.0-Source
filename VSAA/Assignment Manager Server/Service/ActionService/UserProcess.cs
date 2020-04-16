//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//


namespace Microsoft.VisualStudio.Academic.AssignmentManager.ActionService
{
	using System;
	using System.Runtime.InteropServices;
	using System.Diagnostics;
	using System.Security.Principal;

	// class holding all the P/Invoke stub functions
	internal class PInvokeProcess
	{
		protected const uint WAIT_FAILED = 0xFFFFFFFF;
		protected const uint WAIT_TIMEOUT = 258;

		[StructLayout(LayoutKind.Sequential)]
		protected struct PROFILEINFOW 
		{
			internal uint       dwSize;                 // Set to sizeof(PROFILEINFO) before calling
			internal uint       dwFlags;                // See flags above
			[MarshalAs(UnmanagedType.LPWStr)] internal string       lpUserName;             // User name (required)
			[MarshalAs(UnmanagedType.LPWStr)] internal string       lpProfilePath;          // Roaming profile path (optional, can be NULL)
			[MarshalAs(UnmanagedType.LPWStr)] internal string       lpDefaultPath;          // Default user profile path (optional, can be NULL)
			[MarshalAs(UnmanagedType.LPWStr)] internal string       lpServerName;           // Validating domain controller name in netbios format (optional, can be NULL but group NT4 style policy won't be applied)
			[MarshalAs(UnmanagedType.LPWStr)] internal string       lpPolicyPath;           // Path to the NT4 style policy file (optional, can be NULL)
			internal IntPtr hProfile;               // Filled in by the function.  Registry key handle open to the root.
		}

		[DllImport("Userenv.dll")]
		protected static extern bool LoadUserProfileW(IntPtr hToken, ref PROFILEINFOW lpProfileInfo);

		[DllImport("Userenv.dll")]
		protected static extern bool UnloadUserProfile(IntPtr hToken, IntPtr hProfile);

		[DllImport("Userenv.dll")]
		protected static extern bool CreateEnvironmentBlock(out IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

		[DllImport("Userenv.dll")]
		protected static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);

		//Struct stub for P/Invoking
		[StructLayout(LayoutKind.Sequential)]
		protected struct PROCESS_INFORMATION
		{
			internal IntPtr hProcess;
			internal IntPtr hThread;
			internal uint dwProcessID;
			internal uint dwThreadID;
		}

		//Struct stub for P/Invoking
		[StructLayout(LayoutKind.Sequential)]
		protected struct STARTUPINFO
		{
			internal uint cb;
			[MarshalAs(UnmanagedType.LPWStr)] internal string lpReserved;
			[MarshalAs(UnmanagedType.LPWStr)] internal string lpDesktop;
			[MarshalAs(UnmanagedType.LPWStr)] internal string lpTitle;
			internal uint dwX;
			internal uint dwY;
			internal uint dwXSize;
			internal uint dwYSize;
			internal uint dwXCountChars;
			internal uint dwYCountChars;
			internal uint dwFillAttribute;
			internal uint dwFlags;
			internal ushort wShowWindow;
			internal ushort cbReserved2;
			internal byte lpReserved2;
			internal IntPtr hStdInput;
			internal IntPtr hStdOutput;
			internal IntPtr hStdError;
		}

		protected const int CREATE_SUSPENDED = 0x00000004;
		protected const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;

		[DllImport("Advapi32.dll")]
		protected static extern bool CreateProcessAsUserW(
			IntPtr hToken,													// handle to user token
			[MarshalAs(UnmanagedType.LPWStr)] string lpApplicationName,		// name of executable module
			[MarshalAs(UnmanagedType.LPWStr)] string lpCommandLine,			// command-line string
			IntPtr lpProcessAttributes,										// SD
			IntPtr lpThreadAttributes,										// SD
			bool bInheritHandles,											// inheritance option
			uint dwCreationFlags,											// creation flags
			IntPtr lpEnvironment,											// new environment block
			[MarshalAs(UnmanagedType.LPWStr)] string lpCurrentDirectory,    // current directory name
			ref STARTUPINFO lpStartupInfo,									// startup information
			out PROCESS_INFORMATION lpProcessInformation					// process information
		);

		// Stub Function used for P/Invoke
		[DllImport("kernel32.dll")]
		protected static extern IntPtr CreateJobObject( IntPtr lpJobAttributes, [MarshalAs(UnmanagedType.LPWStr)] string lpName );
	
		// Stub Function used for P/Invoke
		[DllImport("kernel32.dll")]
		protected static extern bool AssignProcessToJobObject( IntPtr hJob, IntPtr hProcess );
	
		// Stub Function used for P/Invoke
		[DllImport("kernel32.dll")]
		protected static extern bool TerminateJobObject( IntPtr hJob, uint uExitCode);

		// Stub Function used for P/Invoke
		[DllImport("kernel32.dll")]
		protected static extern bool CloseHandle( IntPtr hObject );

		// Stub Function used for P/Invoke
		[DllImport("kernel32.dll")]
		protected static extern uint WaitForMultipleObjects
			(
			uint nCount,
			IntPtr[] lpHandles,
			bool bWaitAll,
			uint dwMilliseconds
			);

		// Stub function used for P/Invoke
		[DllImport("kernel32.dll")]
		protected static extern int ResumeThread( IntPtr hTread );

		// Stub function used for P/Invoke
		[DllImport("kernel32.dll")]
		protected static extern bool GetExitCodeProcess( IntPtr hProcess, out uint lpExitCode );
	
		protected const int LOGON32_LOGON_INTERACTIVE = 2;
		protected const int LOGON32_PROVIDER_DEFAULT = 0;

		// Stub Function used for P/Invoke
		[DllImport("Advapi32.dll")]
		protected static extern bool LogonUserW
		(
			[MarshalAs(UnmanagedType.LPWStr)] string lpszUsername,
			[MarshalAs(UnmanagedType.LPWStr)] string lpszDomain,
			[MarshalAs(UnmanagedType.LPWStr)] string lpszPassword,
			uint dwLogonType,
			uint dwLogonProvider,
			out System.IntPtr phToken
		);
	}

	internal class UserProcess : PInvokeProcess
	{
		internal ProcessStartInfo StartInfo = new ProcessStartInfo();
		internal bool HasExited = false;
		internal uint ExitCode = 0;	
		internal string OutputFile = "";
		internal string InputFile = "";
		protected IntPtr hJobHandle = IntPtr.Zero;
		protected STARTUPINFO si = new STARTUPINFO();

		internal bool Run(int maxTime)
		{
			IntPtr hJob = IntPtr.Zero;
			IntPtr hUser = IntPtr.Zero;
			IntPtr hEnvironment = IntPtr.Zero;
			STARTUPINFO si = new STARTUPINFO();
			PROCESS_INFORMATION pi = new PROCESS_INFORMATION();
			PROFILEINFOW profInfo = new PROFILEINFOW();
			profInfo.lpUserName = null;
			pi.hProcess = IntPtr.Zero;
			pi.hThread = IntPtr.Zero;

			try 
			{
				hJob = CreateJobObject( IntPtr.Zero, null );

				if (hJob == IntPtr.Zero) 
				{
					SharedSupport.LogMessage("Unable to create the Job object.");
					return false;
				}


				string commandLine = "\"" + StartInfo.FileName + "\" " + StartInfo.Arguments;

				if (StartInfo.RedirectStandardInput && System.IO.File.Exists(InputFile))
				{
					commandLine += " < \"" + InputFile + "\"";
				}

				if (StartInfo.RedirectStandardOutput)
				{
					commandLine += " > \"" + OutputFile + "\"";
				}

				si.cb = (uint)Marshal.SizeOf(si);

				LSAUtil lsaUtil = new LSAUtil(Constants.AMUserLSAPasswordKey);
				string strPassword = lsaUtil.RetrieveEncryptedString();
				string strDomain = System.Environment.MachineName;

				if (!LogonUserW(Constants.AMUserName, strDomain, strPassword, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out hUser))
				{
					return false;
				}
			
				profInfo.dwSize = (uint)Marshal.SizeOf(profInfo);
				profInfo.lpUserName = Constants.AMUserName;
				
				if (!LoadUserProfileW(hUser, ref profInfo)) 
				{
					return false;
				}

				if (!CreateEnvironmentBlock(out hEnvironment, hUser, false)) 
				{
					return false;
				}

				// Create process suspended
				commandLine = System.Environment.SystemDirectory+"\\cmd.exe /c \"" + commandLine + "\"";
				if (!CreateProcessAsUserW( hUser, null, commandLine, IntPtr.Zero, IntPtr.Zero, false, 
					CREATE_SUSPENDED | CREATE_UNICODE_ENVIRONMENT, hEnvironment, 
					StartInfo.WorkingDirectory, ref si, out pi ))
				{
					return false;
				}
			
				if (!AssignProcessToJobObject(hJob, pi.hProcess)) 
				{
					return false;
				}

				if (ResumeThread(pi.hThread) < 0)
				{
					return false;
				}

				IntPtr[] h = {pi.hProcess, hJob};
				const int WAIT_OBJECT_0 = 0;
				uint dw = WaitForMultipleObjects( 2, h, false, (uint) maxTime ); 			
				switch( dw )
				{
					case WAIT_OBJECT_0:
						// Process exited normally
						HasExited = true;
						GetExitCodeProcess( pi.hProcess , out ExitCode );
						break;

					case WAIT_OBJECT_0 + 1:
						// If the job object is signaled, it means that it has 
						// terminated because it reached a resource limit.

					case WAIT_TIMEOUT:
						// We ran out of time for the process being run by the user.
						// It will be killed in the 'finally' block when the Job
						// object is terminated.
				
					default:
						HasExited = false;
						break;
				}
			} 
			finally 
			{
				if (hEnvironment != IntPtr.Zero) 
				{
					DestroyEnvironmentBlock(hEnvironment);
				}

				if (hUser != IntPtr.Zero && profInfo.lpUserName != String.Empty) 
				{
					UnloadUserProfile(hUser, profInfo.hProfile);
				}

				if (hUser != IntPtr.Zero) 
				{
					CloseHandle(hUser);
					hUser = IntPtr.Zero;
				}

				if (hJob != IntPtr.Zero) 
				{
					TerminateJobObject(hJob, 1);
					CloseHandle(hJob);	
				}

				if (pi.hThread != IntPtr.Zero) 
				{
					CloseHandle(pi.hProcess);
					CloseHandle(pi.hThread);
				}
			}

			return true;
		}
	}
}