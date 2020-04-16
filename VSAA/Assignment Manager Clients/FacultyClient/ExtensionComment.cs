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
  /// The ExtensionComment structure is simply a reification of the data held within
  /// the registry entries on-disk. It holds the set of extensions (comma-delimited),
  /// along with the begin and end comments.
  /// </summary>
  internal struct ExtensionComment {
    public ExtensionComment(string ext, string begin, string end) {
      Extensions = ext; BeginComment = begin; EndComment = end;
    }
    
    public string Extensions;
    public string BeginComment;
    public string EndComment;
  }
}
