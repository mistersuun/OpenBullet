// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.OverlayWindowDropTarget
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class OverlayWindowDropTarget : IOverlayWindowDropTarget
{
  private IOverlayWindowArea _overlayArea;
  private Rect _screenDetectionArea;
  private OverlayWindowDropTargetType _type;

  internal OverlayWindowDropTarget(
    IOverlayWindowArea overlayArea,
    OverlayWindowDropTargetType targetType,
    FrameworkElement element)
  {
    this._overlayArea = overlayArea;
    this._type = targetType;
    this._screenDetectionArea = new Rect(element.TransformToDeviceDPI(new Point()), element.TransformActualSizeToAncestor());
  }

  Rect IOverlayWindowDropTarget.ScreenDetectionArea => this._screenDetectionArea;

  OverlayWindowDropTargetType IOverlayWindowDropTarget.Type => this._type;
}
