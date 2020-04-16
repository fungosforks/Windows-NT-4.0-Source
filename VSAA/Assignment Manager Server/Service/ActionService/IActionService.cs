//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//


using System;

namespace Microsoft.VisualStudio.Academic.AssignmentManager.ActionService
{
	/// <summary>
	/// Summary description for IActionService.
	/// </summary>
	public interface IActionService
	{
		void Init(int AssignmentID, string tempDirectory);
		void RetrieveElements();
		void RunService();
		void StoreResult();
		void Cleanup();
	}
}
