// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.DocumentPaneDropAsAnchorableTarget
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal class DocumentPaneDropAsAnchorableTarget : DropTarget<LayoutDocumentPaneControl>
{
  private LayoutDocumentPaneControl _targetPane;
  private int _tabIndex = -1;

  internal DocumentPaneDropAsAnchorableTarget(
    LayoutDocumentPaneControl paneControl,
    Rect detectionRect,
    DropTargetType type)
    : base(paneControl, detectionRect, type)
  {
    this._targetPane = paneControl;
  }

  internal DocumentPaneDropAsAnchorableTarget(
    LayoutDocumentPaneControl paneControl,
    Rect detectionRect,
    DropTargetType type,
    int tabIndex)
    : base(paneControl, detectionRect, type)
  {
    this._targetPane = paneControl;
    this._tabIndex = tabIndex;
  }

  protected override void Drop(LayoutAnchorableFloatingWindow floatingWindow)
  {
    ILayoutDocumentPane model = this._targetPane.Model as ILayoutDocumentPane;
    LayoutDocumentPaneGroup containerPaneGroup;
    LayoutPanel containerPanel;
    this.FindParentLayoutDocumentPane(model, out containerPaneGroup, out containerPanel);
    switch (this.Type)
    {
      case DropTargetType.DocumentPaneDockAsAnchorableLeft:
        if (containerPanel != null && containerPanel.ChildrenCount == 1)
          containerPanel.Orientation = Orientation.Horizontal;
        if (containerPanel != null && containerPanel.Orientation == Orientation.Horizontal)
        {
          containerPanel.Children.Insert(containerPanel.IndexOfChild(containerPaneGroup != null ? (ILayoutElement) containerPaneGroup : (ILayoutElement) model), (ILayoutPanelElement) floatingWindow.RootPanel);
          break;
        }
        if (containerPanel == null)
          throw new NotImplementedException();
        LayoutPanel newElement1 = new LayoutPanel()
        {
          Orientation = Orientation.Horizontal
        };
        containerPanel.ReplaceChild(containerPaneGroup != null ? (ILayoutElement) containerPaneGroup : (ILayoutElement) model, (ILayoutElement) newElement1);
        newElement1.Children.Add(containerPaneGroup != null ? (ILayoutPanelElement) containerPaneGroup : (ILayoutPanelElement) model);
        newElement1.Children.Insert(0, (ILayoutPanelElement) floatingWindow.RootPanel);
        break;
      case DropTargetType.DocumentPaneDockAsAnchorableTop:
        if (containerPanel != null && containerPanel.ChildrenCount == 1)
          containerPanel.Orientation = Orientation.Vertical;
        if (containerPanel != null && containerPanel.Orientation == Orientation.Vertical)
        {
          containerPanel.Children.Insert(containerPanel.IndexOfChild(containerPaneGroup != null ? (ILayoutElement) containerPaneGroup : (ILayoutElement) model), (ILayoutPanelElement) floatingWindow.RootPanel);
          break;
        }
        if (containerPanel == null)
          throw new NotImplementedException();
        LayoutPanel newElement2 = new LayoutPanel()
        {
          Orientation = Orientation.Vertical
        };
        containerPanel.ReplaceChild(containerPaneGroup != null ? (ILayoutElement) containerPaneGroup : (ILayoutElement) model, (ILayoutElement) newElement2);
        newElement2.Children.Add(containerPaneGroup != null ? (ILayoutPanelElement) containerPaneGroup : (ILayoutPanelElement) model);
        newElement2.Children.Insert(0, (ILayoutPanelElement) floatingWindow.RootPanel);
        break;
      case DropTargetType.DocumentPaneDockAsAnchorableRight:
        if (containerPanel != null && containerPanel.ChildrenCount == 1)
          containerPanel.Orientation = Orientation.Horizontal;
        if (containerPanel != null && containerPanel.Orientation == Orientation.Horizontal)
        {
          containerPanel.Children.Insert(containerPanel.IndexOfChild(containerPaneGroup != null ? (ILayoutElement) containerPaneGroup : (ILayoutElement) model) + 1, (ILayoutPanelElement) floatingWindow.RootPanel);
          break;
        }
        if (containerPanel == null)
          throw new NotImplementedException();
        LayoutPanel newElement3 = new LayoutPanel()
        {
          Orientation = Orientation.Horizontal
        };
        containerPanel.ReplaceChild(containerPaneGroup != null ? (ILayoutElement) containerPaneGroup : (ILayoutElement) model, (ILayoutElement) newElement3);
        newElement3.Children.Add(containerPaneGroup != null ? (ILayoutPanelElement) containerPaneGroup : (ILayoutPanelElement) model);
        newElement3.Children.Add((ILayoutPanelElement) floatingWindow.RootPanel);
        break;
      case DropTargetType.DocumentPaneDockAsAnchorableBottom:
        if (containerPanel != null && containerPanel.ChildrenCount == 1)
          containerPanel.Orientation = Orientation.Vertical;
        if (containerPanel != null && containerPanel.Orientation == Orientation.Vertical)
        {
          containerPanel.Children.Insert(containerPanel.IndexOfChild(containerPaneGroup != null ? (ILayoutElement) containerPaneGroup : (ILayoutElement) model) + 1, (ILayoutPanelElement) floatingWindow.RootPanel);
          break;
        }
        if (containerPanel == null)
          throw new NotImplementedException();
        LayoutPanel newElement4 = new LayoutPanel()
        {
          Orientation = Orientation.Vertical
        };
        containerPanel.ReplaceChild(containerPaneGroup != null ? (ILayoutElement) containerPaneGroup : (ILayoutElement) model, (ILayoutElement) newElement4);
        newElement4.Children.Add(containerPaneGroup != null ? (ILayoutPanelElement) containerPaneGroup : (ILayoutPanelElement) model);
        newElement4.Children.Add((ILayoutPanelElement) floatingWindow.RootPanel);
        break;
    }
    base.Drop(floatingWindow);
  }

  public override Geometry GetPreviewPath(
    OverlayWindow overlayWindow,
    LayoutFloatingWindow floatingWindowModel)
  {
    ILayoutDocumentPane model = this._targetPane.Model as ILayoutDocumentPane;
    DockingManager manager = model.Root.Manager;
    LayoutDocumentPaneGroup parentGroup;
    LayoutPanel parentGroupPanel;
    if (!this.FindParentLayoutDocumentPane(model, out parentGroup, out parentGroupPanel))
      return (Geometry) null;
    Rect screenArea = (manager.FindLogicalChildren<FrameworkElement>().OfType<ILayoutControl>().First<ILayoutControl>((Func<ILayoutControl, bool>) (d => parentGroup == null ? d.Model == parentGroupPanel : d.Model == parentGroup)) as FrameworkElement).GetScreenArea();
    switch (this.Type)
    {
      case DropTargetType.DocumentPaneDockAsAnchorableLeft:
        screenArea.Offset(-overlayWindow.Left, -overlayWindow.Top);
        screenArea.Width /= 3.0;
        return (Geometry) new RectangleGeometry(screenArea);
      case DropTargetType.DocumentPaneDockAsAnchorableTop:
        screenArea.Offset(-overlayWindow.Left, -overlayWindow.Top);
        screenArea.Height /= 3.0;
        return (Geometry) new RectangleGeometry(screenArea);
      case DropTargetType.DocumentPaneDockAsAnchorableRight:
        screenArea.Offset(-overlayWindow.Left, -overlayWindow.Top);
        screenArea.Offset(screenArea.Width - screenArea.Width / 3.0, 0.0);
        screenArea.Width /= 3.0;
        return (Geometry) new RectangleGeometry(screenArea);
      case DropTargetType.DocumentPaneDockAsAnchorableBottom:
        screenArea.Offset(-overlayWindow.Left, -overlayWindow.Top);
        screenArea.Offset(0.0, screenArea.Height - screenArea.Height / 3.0);
        screenArea.Height /= 3.0;
        return (Geometry) new RectangleGeometry(screenArea);
      default:
        return (Geometry) null;
    }
  }

  private bool FindParentLayoutDocumentPane(
    ILayoutDocumentPane documentPane,
    out LayoutDocumentPaneGroup containerPaneGroup,
    out LayoutPanel containerPanel)
  {
    containerPaneGroup = (LayoutDocumentPaneGroup) null;
    containerPanel = (LayoutPanel) null;
    if (documentPane.Parent is LayoutPanel)
    {
      containerPaneGroup = (LayoutDocumentPaneGroup) null;
      containerPanel = documentPane.Parent as LayoutPanel;
      return true;
    }
    if (!(documentPane.Parent is LayoutDocumentPaneGroup))
      return false;
    parent = documentPane.Parent as LayoutDocumentPaneGroup;
    do
      ;
    while (!(parent.Parent is LayoutPanel) && parent.Parent is LayoutDocumentPaneGroup parent);
    if (parent == null)
      return false;
    containerPaneGroup = parent;
    containerPanel = parent.Parent as LayoutPanel;
    return true;
  }
}
