// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.IDropTarget
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal interface IDropTarget
{
  DropTargetType Type { get; }

  Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindow);

  bool HitTest(Point dragPoint);

  void Drop(LayoutFloatingWindow floatingWindow);

  void DragEnter();

  void DragLeave();
}
