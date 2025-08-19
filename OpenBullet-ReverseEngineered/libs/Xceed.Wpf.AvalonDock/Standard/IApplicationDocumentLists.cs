// Decompiled with JetBrains decompiler
// Type: Standard.IApplicationDocumentLists
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("3c594f9f-9f30-47a1-979a-c9e83d3d0a06")]
[ComImport]
internal interface IApplicationDocumentLists
{
  void SetAppID([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);

  [return: MarshalAs(UnmanagedType.IUnknown)]
  object GetList([In] APPDOCLISTTYPE listtype, [In] uint cItemsDesired, [In] ref Guid riid);
}
