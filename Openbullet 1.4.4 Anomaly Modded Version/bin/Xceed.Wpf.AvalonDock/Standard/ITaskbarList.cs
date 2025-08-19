// Decompiled with JetBrains decompiler
// Type: Standard.ITaskbarList
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("56FDF342-FD6D-11d0-958A-006097C9A090")]
[ComImport]
internal interface ITaskbarList
{
  void HrInit();

  void AddTab(IntPtr hwnd);

  void DeleteTab(IntPtr hwnd);

  void ActivateTab(IntPtr hwnd);

  void SetActiveAlt(IntPtr hwnd);
}
