// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.ComOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class ComOps
{
  public static string __str__(object self) => self.ToString();

  public static string __repr__(object self)
  {
    return $"<{self.ToString()}({TypeDescriptor.GetClassName(self)}) object at {PythonOps.HexId(self)}>";
  }

  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("00020400-0000-0000-C000-000000000046")]
  [ComImport]
  private interface IDispatch
  {
    int GetTypeInfoCount();

    [return: MarshalAs(UnmanagedType.Interface)]
    ITypeInfo GetTypeInfo([MarshalAs(UnmanagedType.U4), In] int iTInfo, [MarshalAs(UnmanagedType.U4), In] int lcid);

    void GetIDsOfNames([In] ref Guid riid, [MarshalAs(UnmanagedType.LPArray), In] string[] rgszNames, [MarshalAs(UnmanagedType.U4), In] int cNames, [MarshalAs(UnmanagedType.U4), In] int lcid, [MarshalAs(UnmanagedType.LPArray), Out] int[] rgDispId);
  }
}
