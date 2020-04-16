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
  using System.Runtime.InteropServices;

  /// <summary>
  /// The CommentPair object encapsulates the begin and end comments for a particular
  /// file extension type.
  /// </summary>
  // This makes the runtime generate a dual interface for this class automagically.
  [System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual)] 
    public class CommentPair : Object {
    private  CommentPair() {}
    internal CommentPair(string begin, string end) {
      m_strBeginComment = begin;
      m_strEndComment = end;
    }

    public string BeginComment {
      get { return m_strBeginComment; }
    }
    public string EndComment {
      get { return m_strEndComment; }
    }

    private string m_strBeginComment;
    private string m_strEndComment;
  }
}
