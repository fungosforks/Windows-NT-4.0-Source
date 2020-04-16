//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//
namespace FacultyClient {
  using System;

  /// <summary>
  /// The PropertiesDummy object is returned from the GetProperties() method of the
  /// ToolsOptions class. It encapsulates the extensions object.
  /// CONSIDER: should users be allowed to set the global collapse-setting through
  /// the automation model?
  /// </summary>
  // This makes the runtime generate a dual interface for this class automagically.
  [System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual)] 
    public class PropertiesDummy : Object {
    internal PropertiesDummy(System.Collections.ArrayList l, bool collapse, bool todo) { m_list = l; m_collapse = collapse; m_todo = todo;}
    public Extensions Extensions {
      get {
        Extensions e = new Extensions( m_list );
        return e;
      }
    }
    
    public bool Collapse {
      get { return m_collapse; }
    }

    public bool PromptForTodo {
      get { return m_todo; }
    }

    private System.Collections.ArrayList m_list;
    private bool m_collapse;
    private bool m_todo;
  }
}
