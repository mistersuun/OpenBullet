// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.DocumentPaneControlOverlayArea
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class DocumentPaneControlOverlayArea : OverlayArea
{
  private LayoutDocumentPaneControl _documentPaneControl;

  internal DocumentPaneControlOverlayArea(
    IOverlayWindow overlayWindow,
    LayoutDocumentPaneControl documentPaneControl)
    : base(overlayWindow)
  {
    this._documentPaneControl = documentPaneControl;
    this.SetScreenDetectionArea(new Rect(this._documentPaneControl.PointToScreenDPI(new Point()), this._documentPaneControl.TransformActualSizeToAncestor()));
  }
}
