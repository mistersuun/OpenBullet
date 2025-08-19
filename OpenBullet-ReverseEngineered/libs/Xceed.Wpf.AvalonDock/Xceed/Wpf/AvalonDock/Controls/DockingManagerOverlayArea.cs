// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.DockingManagerOverlayArea
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class DockingManagerOverlayArea : OverlayArea
{
  private DockingManager _manager;

  internal DockingManagerOverlayArea(IOverlayWindow overlayWindow, DockingManager manager)
    : base(overlayWindow)
  {
    this._manager = manager;
    this.SetScreenDetectionArea(new Rect(this._manager.PointToScreenDPI(new Point()), this._manager.TransformActualSizeToAncestor()));
  }
}
