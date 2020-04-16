//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//


using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

namespace Microsoft.VisualStudio.Academic.AssignmentManager.ActionService
{
	/// <summary>
	/// Summary description for ServiceInstaller.
	/// </summary>
	[RunInstaller(true)]
	public class ServiceInstaller : System.Configuration.Install.Installer
	{
		private System.ServiceProcess.ServiceInstaller serviceInstaller;
		private System.ServiceProcess.ServiceProcessInstaller processInstaller;

		public const string AM_SERVICE_NAME = "Assignment Manager Services";

		public ServiceInstaller()
		{
			processInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			serviceInstaller = new System.ServiceProcess.ServiceInstaller();

			// Service will run under system account
			processInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			
			// Service will have Start Type of Automatic
			serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

			// Add a dependency on MSMQ so we start after it, or force it to start
			serviceInstaller.ServicesDependedOn = new string[] {"MSMQ"};
    		
			serviceInstaller.ServiceName = @AM_SERVICE_NAME;
	
			Installers.Add(serviceInstaller);
			Installers.Add(processInstaller);

		}

		public override void Install(System.Collections.IDictionary savedState)
		{
			// run the base install first
			base.Install(savedState);

			// now try to start the service
			try
			{
				System.ServiceProcess.ServiceController amService = new System.ServiceProcess.ServiceController(@AM_SERVICE_NAME);
				amService.Start();
			}
			catch (Exception e)
			{
				System.Diagnostics.EventLog.WriteEntry(this.ToString(), e.ToString());
			}
		}
	}
}
