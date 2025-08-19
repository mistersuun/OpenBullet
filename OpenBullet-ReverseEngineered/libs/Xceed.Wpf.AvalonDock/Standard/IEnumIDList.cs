// Decompiled with JetBrains decompiler
// Type: Standard.IEnumIDList
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("000214F2-0000-0000-C000-000000000046")]
[ComImport]
internal interface IEnumIDList
{
  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT Next(uint celt, out IntPtr rgelt, out int pceltFetched);

  [MethodImpl(MethodImplOptions.PreserveSig)]
  HRESULT Skip(uint celt);

  void Reset();

  void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenum);
}
