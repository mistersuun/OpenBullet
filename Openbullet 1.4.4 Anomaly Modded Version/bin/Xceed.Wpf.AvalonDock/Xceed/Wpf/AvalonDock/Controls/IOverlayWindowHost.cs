// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.IOverlayWindowHost
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Collections.Generic;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal interface IOverlayWindowHost
{
  DockingManager Manager { get; }

  bool HitTest(Point dragPoint);

  IOverlayWindow ShowOverlayWindow(LayoutFloatingWindowControl draggingWindow);

  void HideOverlayWindow();

  IEnumerable<IDropArea> GetDropAreas(LayoutFloatingWindowControl draggingWindow);
}
