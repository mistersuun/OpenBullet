// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.DocumentPaneGroupDropTarget
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Linq;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal class DocumentPaneGroupDropTarget : DropTarget<LayoutDocumentPaneGroupControl>
{
  private LayoutDocumentPaneGroupControl _targetPane;

  internal DocumentPaneGroupDropTarget(
    LayoutDocumentPaneGroupControl paneControl,
    Rect detectionRect,
    DropTargetType type)
    : base(paneControl, detectionRect, type)
  {
    this._targetPane = paneControl;
  }

  protected override void Drop(LayoutDocumentFloatingWindow floatingWindow)
  {
    ILayoutPane model = this._targetPane.Model as ILayoutPane;
    if (this.Type == DropTargetType.DocumentPaneGroupDockInside)
      ((model as LayoutDocumentPaneGroup).Children[0] as LayoutDocumentPane).Children.Insert(0, (LayoutContent) floatingWindow.RootDocument);
    base.Drop(floatingWindow);
  }

  protected override void Drop(LayoutAnchorableFloatingWindow floatingWindow)
  {
    ILayoutPane model = this._targetPane.Model as ILayoutPane;
    if (this.Type == DropTargetType.DocumentPaneGroupDockInside)
    {
      LayoutDocumentPane child = (model as LayoutDocumentPaneGroup).Children[0] as LayoutDocumentPane;
      LayoutAnchorablePaneGroup rootPanel = floatingWindow.RootPanel;
      int index = 0;
      foreach (LayoutAnchorable layoutAnchorable in rootPanel.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>())
      {
        layoutAnchorable.SetCanCloseInternal(true);
        child.Children.Insert(index, (LayoutContent) layoutAnchorable);
        ++index;
      }
    }
    base.Drop(floatingWindow);
  }

  public override Geometry GetPreviewPath(
    OverlayWindow overlayWindow,
    LayoutFloatingWindow floatingWindowModel)
  {
    if (this.Type != DropTargetType.DocumentPaneGroupDockInside)
      return (Geometry) null;
    Rect screenArea = this.TargetElement.GetScreenArea();
    screenArea.Offset(-overlayWindow.Left, -overlayWindow.Top);
    return (Geometry) new RectangleGeometry(screenArea);
  }
}
