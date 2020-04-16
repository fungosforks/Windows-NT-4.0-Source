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
	using System.Security.Principal;

	// Class to wrap impersonation of users.
	internal class ImpersonateUser : IDisposable
	{
		protected IntPtr hUserHandle = IntPtr.Zero;					
		protected WindowsImpersonationContext wiContext = null;		
		protected string strUsername = "";
		protected string strDomain = null;
		protected string strPassword = "";

		internal ImpersonateUser()
		{
			strUsername = Constants.AMUserName;
			strDomain = System.Environment.MachineName;
			LSAUtil lsaUtil = new LSAUtil(Constants.AMUserLSAPasswordKey);
			strPassword = lsaUtil.RetrieveEncryptedString();
		}

		~ImpersonateUser()
		{
			Dispose(false);
		}

		//Implement IDisposable.
		public void Dispose() 
		{
			Dispose(true);
			GC.SuppressFinalize(this); 
		}

		protected virtual void Dispose(bool disposing) 
		{
			if (hUserHandle != IntPtr.Zero)		
			{
				Win32.CloseHandle(hUserHandle);
			}
		}		

		// LogonUser
		internal bool Logon( )
		{
			return Win32.LogonUser( strUsername, strDomain, strPassword, Win32.LOGON32_LOGON_NETWORK, Win32.LOGON32_PROVIDER_DEFAULT, out hUserHandle );
		}

		// Start Impersonating
		internal void Start()
		{
			if (hUserHandle == IntPtr.Zero)
			{
				throw new InvalidOperationException();
			}

			WindowsIdentity wiIdentity = new WindowsIdentity(hUserHandle);
			// Impersonate the user.
			wiContext = wiIdentity.Impersonate();   
		}

		// Stop Impersonating
		internal void Stop()
		{
			if (wiContext == null)
			{
				throw new InvalidOperationException();
			}
			wiContext.Undo();
			wiContext = null;
		}
		
		// private class for P/Invoke functions
		private class Win32
		{
			internal const int LOGON32_LOGON_INTERACTIVE = 2;
			internal const int LOGON32_PROVIDER_DEFAULT = 0;
			internal const int LOGON32_LOGON_NETWORK = 3;
			internal const int LOGON32_LOGON_NETWORK_CLEARTEXT = 8;

			private Win32() 
			{
				// Make class non-creatable.
			}

			// Stub Function used for P/Invoke
			[DllImport("Advapi32.dll")]
			internal static extern bool LogonUser
			(
				string lpszUsername,
				string lpszDomain,
				string lpszPassword,
				uint dwLogonType,
				uint dwLogonProvider,
				out System.IntPtr phToken
			);

			// Stub Function used for P/Invoke
			[DllImport("kernel32.dll")]
			internal static extern bool CloseHandle( IntPtr hObject );
		}
	}
}