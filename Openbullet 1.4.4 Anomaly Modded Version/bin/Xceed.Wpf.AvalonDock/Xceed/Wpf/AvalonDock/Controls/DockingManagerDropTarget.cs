// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.DockingManagerDropTarget
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

internal class DockingManagerDropTarget : DropTarget<DockingManager>
{
  private DockingManager _manager;

  internal DockingManagerDropTarget(
    DockingManager manager,
    Rect detectionRect,
    DropTargetType type)
    : base(manager, detectionRect, type)
  {
    this._manager = manager;
  }

  protected override void Drop(LayoutAnchorableFloatingWindow floatingWindow)
  {
    switch (this.Type)
    {
      case DropTargetType.DockingManagerDockLeft:
        if (this._manager.Layout.RootPanel.Orientation != Orientation.Horizontal && this._manager.Layout.RootPanel.Children.Count == 1)
          this._manager.Layout.RootPanel.Orientation = Orientation.Horizontal;
        if (this._manager.Layout.RootPanel.Orientation == Orientation.Horizontal)
        {
          LayoutAnchorablePaneGroup rootPanel = floatingWindow.RootPanel;
          if (rootPanel != null && rootPanel.Orientation == Orientation.Horizontal)
          {
            ILayoutAnchorablePane[] array = rootPanel.Children.ToArray<ILayoutAnchorablePane>();
            for (int index = 0; index < array.Length; ++index)
              this._manager.Layout.RootPanel.Children.Insert(index, (ILayoutPanelElement) array[index]);
            break;
          }
          this._manager.Layout.RootPanel.Children.Insert(0, (ILayoutPanelElement) floatingWindow.RootPanel);
          break;
        }
        LayoutPanel layoutPanel1 = new LayoutPanel()
        {
          Orientation = Orientation.Horizontal
        };
        layoutPanel1.Children.Add((ILayoutPanelElement) floatingWindow.RootPanel);
        layoutPanel1.Children.Add((ILayoutPanelElement) this._manager.Layout.RootPanel);
        this._manager.Layout.RootPanel = layoutPanel1;
        break;
      case DropTargetType.DockingManagerDockTop:
        if (this._manager.Layout.RootPanel.Orientation != Orientation.Vertical && this._manager.Layout.RootPanel.Children.Count == 1)
          this._manager.Layout.RootPanel.Orientation = Orientation.Vertical;
        if (this._manager.Layout.RootPanel.Orientation == Orientation.Vertical)
        {
          LayoutAnchorablePaneGroup rootPanel = floatingWindow.RootPanel;
          if (rootPanel != null && rootPanel.Orientation == Orientation.Vertical)
          {
            ILayoutAnchorablePane[] array = rootPanel.Children.ToArray<ILayoutAnchorablePane>();
            for (int index = 0; index < array.Length; ++index)
              this._manager.Layout.RootPanel.Children.Insert(index, (ILayoutPanelElement) array[index]);
            break;
          }
          this._manager.Layout.RootPanel.Children.Insert(0, (ILayoutPanelElement) floatingWindow.RootPanel);
          break;
        }
        LayoutPanel layoutPanel2 = new LayoutPanel()
        {
          Orientation = Orientation.Vertical
        };
        layoutPanel2.Children.Add((ILayoutPanelElement) floatingWindow.RootPanel);
        layoutPanel2.Children.Add((ILayoutPanelElement) this._manager.Layout.RootPanel);
        this._manager.Layout.RootPanel = layoutPanel2;
        break;
      case DropTargetType.DockingManagerDockRight:
        if (this._manager.Layout.RootPanel.Orientation != Orientation.Horizontal && this._manager.Layout.RootPanel.Children.Count == 1)
          this._manager.Layout.RootPanel.Orientation = Orientation.Horizontal;
        if (this._manager.Layout.RootPanel.Orientation == Orientation.Horizontal)
        {
          LayoutAnchorablePaneGroup rootPanel = floatingWindow.RootPanel;
          if (rootPanel != null && rootPanel.Orientation == Orientation.Horizontal)
          {
            foreach (ILayoutPanelElement layoutPanelElement in rootPanel.Children.ToArray<ILayoutAnchorablePane>())
              this._manager.Layout.RootPanel.Children.Add(layoutPanelElement);
            break;
          }
          this._manager.Layout.RootPanel.Children.Add((ILayoutPanelElement) floatingWindow.RootPanel);
          break;
        }
        LayoutPanel layoutPanel3 = new LayoutPanel()
        {
          Orientation = Orientation.Horizontal
        };
        layoutPanel3.Children.Add((ILayoutPanelElement) floatingWindow.RootPanel);
        layoutPanel3.Children.Insert(0, (ILayoutPanelElement) this._manager.Layout.RootPanel);
        this._manager.Layout.RootPanel = layoutPanel3;
        break;
      case DropTargetType.DockingManagerDockBottom:
        if (this._manager.Layout.RootPanel.Orientation != Orientation.Vertical && this._manager.Layout.RootPanel.Children.Count == 1)
          this._manager.Layout.RootPanel.Orientation = Orientation.Vertical;
        if (this._manager.Layout.RootPanel.Orientation == Orientation.Vertical)
        {
          LayoutAnchorablePaneGroup rootPanel = floatingWindow.RootPanel;
          if (rootPanel != null && rootPanel.Orientation == Orientation.Vertical)
          {
            foreach (ILayoutPanelElement layoutPanelElement in rootPanel.Children.ToArray<ILayoutAnchorablePane>())
              this._manager.Layout.RootPanel.Children.Add(layoutPanelElement);
            break;
          }
          this._manager.Layout.RootPanel.Children.Add((ILayoutPanelElement) floatingWindow.RootPanel);
          break;
        }
        LayoutPanel layoutPanel4 = new LayoutPanel()
        {
          Orientation = Orientation.Vertical
        };
        layoutPanel4.Children.Add((ILayoutPanelElement) floatingWindow.RootPanel);
        layoutPanel4.Children.Insert(0, (ILayoutPanelElement) this._manager.Layout.RootPanel);
        this._manager.Layout.RootPanel = layoutPanel4;
        break;
    }
    base.Drop(floatingWindow);
  }

  public override Geometry GetPreviewPath(
    OverlayWindow overlayWindow,
    LayoutFloatingWindow floatingWindowModel)
  {
    LayoutAnchorableFloatingWindow anchorableFloatingWindow = floatingWindowModel as LayoutAnchorableFloatingWindow;
    ILayoutPositionableElement rootPanel1 = (ILayoutPositionableElement) anchorableFloatingWindow.RootPanel;
    ILayoutPositionableElementWithActualSize rootPanel2 = (ILayoutPositionableElementWithActualSize) anchorableFloatingWindow.RootPanel;
    Rect screenArea = this.TargetElement.GetScreenArea();
    switch (this.Type)
    {
      case DropTargetType.DockingManagerDockLeft:
        double val1_1 = rootPanel1.DockWidth.IsAbsolute ? rootPanel1.DockWidth.Value : rootPanel2.ActualWidth;
        return (Geometry) new RectangleGeometry(new Rect(screenArea.Left - overlayWindow.Left, screenArea.Top - overlayWindow.Top, Math.Min(val1_1, screenArea.Width / 2.0), screenArea.Height));
      case DropTargetType.DockingManagerDockTop:
        double val1_2 = rootPanel1.DockHeight.IsAbsolute ? rootPanel1.DockHeight.Value : rootPanel2.ActualHeight;
        return (Geometry) new RectangleGeometry(new Rect(screenArea.Left - overlayWindow.Left, screenArea.Top - overlayWindow.Top, screenArea.Width, Math.Min(val1_2, screenArea.Height / 2.0)));
      case DropTargetType.DockingManagerDockRight:
        double val1_3 = rootPanel1.DockWidth.IsAbsolute ? rootPanel1.DockWidth.Value : rootPanel2.ActualWidth;
        return (Geometry) new RectangleGeometry(new Rect(screenArea.Right - overlayWindow.Left - Math.Min(val1_3, screenArea.Width / 2.0), screenArea.Top - overlayWindow.Top, Math.Min(val1_3, screenArea.Width / 2.0), screenArea.Height));
      case DropTargetType.DockingManagerDockBottom:
        double val1_4 = rootPanel1.DockHeight.IsAbsolute ? rootPanel1.DockHeight.Value : rootPanel2.ActualHeight;
        return (Geometry) new RectangleGeometry(new Rect(screenArea.Left - overlayWindow.Left, screenArea.Bottom - overlayWindow.Top - Math.Min(val1_4, screenArea.Height / 2.0), screenArea.Width, Math.Min(val1_4, screenArea.Height / 2.0)));
      default:
        throw new InvalidOperationException();
    }
  }
}
