// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.AnchorablePaneControlOverlayArea
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class AnchorablePaneControlOverlayArea : OverlayArea
{
  private LayoutAnchorablePaneControl _anchorablePaneControl;

  internal AnchorablePaneControlOverlayArea(
    IOverlayWindow overlayWindow,
    LayoutAnchorablePaneControl anchorablePaneControl)
    : base(overlayWindow)
  {
    this._anchorablePaneControl = anchorablePaneControl;
    this.SetScreenDetectionArea(new Rect(this._anchorablePaneControl.PointToScreenDPI(new Point()), this._anchorablePaneControl.TransformActualSizeToAncestor()));
  }
}
