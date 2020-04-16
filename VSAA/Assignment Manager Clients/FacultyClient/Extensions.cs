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
  /// The Extensions class is a mapping from file extension to CommentPair type. It is 
  /// read-only and is a copy of the settings on disk or in memory -- meaning if that they
  /// are changed while a piece of code retains a link to this item, those changes will
  /// not be reflected in the retained item!
  /// </summary>
  // This makes the runtime generate a dual interface for this class automagically.
  [System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual)] 
  public class Extensions : Object {
    internal Extensions ( System.Collections.ArrayList l) {
      string []extensions;
      char []splitChars = {','};
      m_hash = new System.Collections.Hashtable();
      
      foreach (ExtensionComment ec in l) {
        extensions = ec.Extensions.Split(splitChars);

        foreach (string extension in extensions) {
          // Get rid of the trialing space, if any, and also
          // remove the '.' at the beginning of the extension.
          m_hash.Add(extension.Trim().Substring(1),
            new CommentPair(ec.BeginComment, ec.EndComment));
        }
      }
    }

    /// <summary>
    /// The Extensions object maps a particular file extension (.scm) to
    /// a particular pair of commments (<; begin-student-comment . ; end-student-comment>).
    /// If there is no registered comment pair for an extension, this returns null,
    /// as the behavior inherited from the hash table implementation does.
    /// Please note that there is no 'set' for this property, even though it is
    /// an indexer, as this object is considered read-only.
    /// </summary>
    public CommentPair this[object Index] {
      get {
        return (CommentPair)m_hash[(string)Index];
      }
    }

    private System.Collections.Hashtable m_hash;
  }
}
