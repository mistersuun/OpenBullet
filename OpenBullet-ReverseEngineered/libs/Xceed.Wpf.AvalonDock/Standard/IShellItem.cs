// Decompiled with JetBrains decompiler
// Type: Standard.IShellItem
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

#nullable disable
namespace Standard;

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
[ComImport]
internal interface IShellItem
{
  [return: MarshalAs(UnmanagedType.Interface)]
  object BindToHandler(IBindCtx pbc, [In] ref Guid bhid, [In] ref Guid riid);

  IShellItem GetParent();

  [return: MarshalAs(UnmanagedType.LPWStr)]
  string GetDisplayName(SIGDN sigdnName);

  SFGAO GetAttributes(SFGAO sfgaoMask);

  int Compare(IShellItem psi, SICHINT hint);
}
