//
// Copyright © 2000-2003 Microsoft Corporation.  All rights reserved.
//
//
// This source code is licensed under Microsoft Shared Source License
// for the Visual Studio .NET Academic Tools Source Licensing Program
// For a copy of the license, see http://www.msdnaa.net/assignmentmanager/sourcelicense/
//
namespace StudentClient {
  using System;
  using System.Runtime.InteropServices;

  // This interface is a simple copy of the IStorage interface provided
  // by COM. Since we will only ever be calling OpenStream, that's the only
  // method that is actually correctly prototyped. Note that members of the
  // interface are still here as placeholders (and so that VTable order is
  // correct), but if they are to be called, all of the paramaters would
  // need to be correctly imported from the original IDL.
  [Guid("0000000b-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  internal interface IStorage {
    void CreateStream();
    [PreserveSig]
    int OpenStream([MarshalAs(UnmanagedType.LPWStr)] string name,
        System.IntPtr reserved, System.UInt32 grfMode,
        System.UInt32 reserved2, [MarshalAs(UnmanagedType.Interface)] out object ppstm);
    void CreateStorage();
    void OpenStorage();
    void CopyTo();
    void MoveElementTo();
    void Commit();
    void Revert();
    void EnumElements();
    void DestroyElement();
    void RenameElement();
    void SetElementTimes();
    void SetClass();
    void SetStateBits();
    void Stat();
  }
}
