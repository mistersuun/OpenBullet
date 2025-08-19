// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.IDispatch
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("00020400-0000-0000-C000-000000000046")]
[ComImport]
internal interface IDispatch
{
  [MethodImpl(MethodImplOptions.PreserveSig)]
  int TryGetTypeInfoCount(out uint pctinfo);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  int TryGetTypeInfo(uint iTInfo, int lcid, out IntPtr info);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  int TryGetIDsOfNames(ref Guid iid, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2, ArraySubType = UnmanagedType.LPWStr)] string[] names, uint cNames, int lcid, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2, ArraySubType = UnmanagedType.I4), Out] int[] rgDispId);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  int TryInvoke(
    int dispIdMember,
    ref Guid riid,
    int lcid,
    System.Runtime.InteropServices.ComTypes.INVOKEKIND wFlags,
    ref System.Runtime.InteropServices.ComTypes.DISPPARAMS pDispParams,
    out object VarResult,
    out System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo,
    out uint puArgErr);
}
