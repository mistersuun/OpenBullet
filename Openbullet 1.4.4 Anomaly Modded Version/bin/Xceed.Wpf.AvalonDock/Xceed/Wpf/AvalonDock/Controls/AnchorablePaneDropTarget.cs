// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.AnchorablePaneDropTarget
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal class AnchorablePaneDropTarget : DropTarget<LayoutAnchorablePaneControl>
{
  private LayoutAnchorablePaneControl _targetPane;
  private int _tabIndex = -1;

  internal AnchorablePaneDropTarget(
    LayoutAnchorablePaneControl paneControl,
    Rect detectionRect,
    DropTargetType type)
    : base(paneControl, detectionRect, type)
  {
    this._targetPane = paneControl;
  }

  internal AnchorablePaneDropTarget(
    LayoutAnchorablePaneControl paneControl,
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
    ILayoutAnchorablePane model = this._targetPane.Model as ILayoutAnchorablePane;
    LayoutAnchorable layoutAnchorable1 = floatingWindow.Descendents().OfType<LayoutAnchorable>().FirstOrDefault<LayoutAnchorable>();
    switch (this.Type)
    {
      case DropTargetType.AnchorablePaneDockLeft:
        ILayoutGroup parent1 = model.Parent as ILayoutGroup;
        ILayoutOrientableGroup parent2 = model.Parent as ILayoutOrientableGroup;
        int index1 = parent1.IndexOfChild((ILayoutElement) model);
        if (parent2.Orientation != Orientation.Horizontal && parent1.ChildrenCount == 1)
          parent2.Orientation = Orientation.Horizontal;
        if (parent2.Orientation == Orientation.Horizontal)
        {
          LayoutAnchorablePaneGroup rootPanel = floatingWindow.RootPanel;
          if (rootPanel != null && (rootPanel.Children.Count == 1 || rootPanel.Orientation == Orientation.Horizontal))
          {
            ILayoutAnchorablePane[] array = rootPanel.Children.ToArray<ILayoutAnchorablePane>();
            for (int index2 = 0; index2 < array.Length; ++index2)
              parent1.InsertChildAt(index1 + index2, (ILayoutElement) array[index2]);
            break;
          }
          parent1.InsertChildAt(index1, (ILayoutElement) floatingWindow.RootPanel);
          break;
        }
        ILayoutPositionableElement positionableElement1 = model as ILayoutPositionableElement;
        LayoutAnchorablePaneGroup anchorablePaneGroup1 = new LayoutAnchorablePaneGroup();
        anchorablePaneGroup1.Orientation = Orientation.Horizontal;
        anchorablePaneGroup1.DockWidth = positionableElement1.DockWidth;
        anchorablePaneGroup1.DockHeight = positionableElement1.DockHeight;
        LayoutAnchorablePaneGroup element1 = anchorablePaneGroup1;
        parent1.InsertChildAt(index1, (ILayoutElement) element1);
        element1.Children.Add(model);
        element1.Children.Insert(0, (ILayoutAnchorablePane) floatingWindow.RootPanel);
        break;
      case DropTargetType.AnchorablePaneDockTop:
        ILayoutGroup parent3 = model.Parent as ILayoutGroup;
        ILayoutOrientableGroup parent4 = model.Parent as ILayoutOrientableGroup;
        int index3 = parent3.IndexOfChild((ILayoutElement) model);
        if (parent4.Orientation != Orientation.Vertical && parent3.ChildrenCount == 1)
          parent4.Orientation = Orientation.Vertical;
        if (parent4.Orientation == Orientation.Vertical)
        {
          LayoutAnchorablePaneGroup rootPanel = floatingWindow.RootPanel;
          if (rootPanel != null && (rootPanel.Children.Count == 1 || rootPanel.Orientation == Orientation.Vertical))
          {
            ILayoutAnchorablePane[] array = rootPanel.Children.ToArray<ILayoutAnchorablePane>();
            for (int index4 = 0; index4 < array.Length; ++index4)
              parent3.InsertChildAt(index3 + index4, (ILayoutElement) array[index4]);
            break;
          }
          parent3.InsertChildAt(index3, (ILayoutElement) floatingWindow.RootPanel);
          break;
        }
        ILayoutPositionableElement positionableElement2 = model as ILayoutPositionableElement;
        LayoutAnchorablePaneGroup anchorablePaneGroup2 = new LayoutAnchorablePaneGroup();
        anchorablePaneGroup2.Orientation = Orientation.Vertical;
        anchorablePaneGroup2.DockWidth = positionableElement2.DockWidth;
        anchorablePaneGroup2.DockHeight = positionableElement2.DockHeight;
        LayoutAnchorablePaneGroup element2 = anchorablePaneGroup2;
        parent3.InsertChildAt(index3, (ILayoutElement) element2);
        element2.Children.Add(model);
        element2.Children.Insert(0, (ILayoutAnchorablePane) floatingWindow.RootPanel);
        break;
      case DropTargetType.AnchorablePaneDockRight:
        ILayoutGroup parent5 = model.Parent as ILayoutGroup;
        ILayoutOrientableGroup parent6 = model.Parent as ILayoutOrientableGroup;
        int index5 = parent5.IndexOfChild((ILayoutElement) model);
        if (parent6.Orientation != Orientation.Horizontal && parent5.ChildrenCount == 1)
          parent6.Orientation = Orientation.Horizontal;
        if (parent6.Orientation == Orientation.Horizontal)
        {
          LayoutAnchorablePaneGroup rootPanel = floatingWindow.RootPanel;
          if (rootPanel != null && (rootPanel.Children.Count == 1 || rootPanel.Orientation == Orientation.Horizontal))
          {
            ILayoutAnchorablePane[] array = rootPanel.Children.ToArray<ILayoutAnchorablePane>();
            for (int index6 = 0; index6 < array.Length; ++index6)
              parent5.InsertChildAt(index5 + 1 + index6, (ILayoutElement) array[index6]);
            break;
          }
          parent5.InsertChildAt(index5 + 1, (ILayoutElement) floatingWindow.RootPanel);
          break;
        }
        ILayoutPositionableElement positionableElement3 = model as ILayoutPositionableElement;
        LayoutAnchorablePaneGroup anchorablePaneGroup3 = new LayoutAnchorablePaneGroup();
        anchorablePaneGroup3.Orientation = Orientation.Horizontal;
        anchorablePaneGroup3.DockWidth = positionableElement3.DockWidth;
        anchorablePaneGroup3.DockHeight = positionableElement3.DockHeight;
        LayoutAnchorablePaneGroup element3 = anchorablePaneGroup3;
        parent5.InsertChildAt(index5, (ILayoutElement) element3);
        element3.Children.Add(model);
        element3.Children.Add((ILayoutAnchorablePane) floatingWindow.RootPanel);
        break;
      case DropTargetType.AnchorablePaneDockBottom:
        ILayoutGroup parent7 = model.Parent as ILayoutGroup;
        ILayoutOrientableGroup parent8 = model.Parent as ILayoutOrientableGroup;
        int index7 = parent7.IndexOfChild((ILayoutElement) model);
        if (parent8.Orientation != Orientation.Vertical && parent7.ChildrenCount == 1)
          parent8.Orientation = Orientation.Vertical;
        if (parent8.Orientation == Orientation.Vertical)
        {
          LayoutAnchorablePaneGroup rootPanel = floatingWindow.RootPanel;
          if (rootPanel != null && (rootPanel.Children.Count == 1 || rootPanel.Orientation == Orientation.Vertical))
          {
            ILayoutAnchorablePane[] array = rootPanel.Children.ToArray<ILayoutAnchorablePane>();
            for (int index8 = 0; index8 < array.Length; ++index8)
              parent7.InsertChildAt(index7 + 1 + index8, (ILayoutElement) array[index8]);
            break;
          }
          parent7.InsertChildAt(index7 + 1, (ILayoutElement) floatingWindow.RootPanel);
          break;
        }
        ILayoutPositionableElement positionableElement4 = model as ILayoutPositionableElement;
        LayoutAnchorablePaneGroup anchorablePaneGroup4 = new LayoutAnchorablePaneGroup();
        anchorablePaneGroup4.Orientation = Orientation.Vertical;
        anchorablePaneGroup4.DockWidth = positionableElement4.DockWidth;
        anchorablePaneGroup4.DockHeight = positionableElement4.DockHeight;
        LayoutAnchorablePaneGroup element4 = anchorablePaneGroup4;
        parent7.InsertChildAt(index7, (ILayoutElement) element4);
        element4.Children.Add(model);
        element4.Children.Add((ILayoutAnchorablePane) floatingWindow.RootPanel);
        break;
      case DropTargetType.AnchorablePaneDockInside:
        LayoutAnchorablePane layoutAnchorablePane = model as LayoutAnchorablePane;
        LayoutAnchorablePaneGroup rootPanel1 = floatingWindow.RootPanel;
        int tabIndex = this._tabIndex == -1 ? 0 : this._tabIndex;
        foreach (LayoutAnchorable layoutAnchorable2 in rootPanel1.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>())
        {
          layoutAnchorablePane.Children.Insert(tabIndex, layoutAnchorable2);
          ++tabIndex;
        }
        break;
    }
    layoutAnchorable1.IsActive = true;
    base.Drop(floatingWindow);
  }

  public override Geometry GetPreviewPath(
    OverlayWindow overlayWindow,
    LayoutFloatingWindow floatingWindowModel)
  {
    LayoutAnchorableFloatingWindow anchorableFloatingWindow = floatingWindowModel as LayoutAnchorableFloatingWindow;
    LayoutAnchorablePaneGroup rootPanel1 = anchorableFloatingWindow.RootPanel;
    LayoutAnchorablePaneGroup rootPanel2 = anchorableFloatingWindow.RootPanel;
    switch (this.Type)
    {
      case DropTargetType.AnchorablePaneDockLeft:
        Rect screenArea1 = this.TargetElement.GetScreenArea();
        screenArea1.Offset(-overlayWindow.Left, -overlayWindow.Top);
        screenArea1.Width /= 2.0;
        return (Geometry) new RectangleGeometry(screenArea1);
      case DropTargetType.AnchorablePaneDockTop:
        Rect screenArea2 = this.TargetElement.GetScreenArea();
        screenArea2.Offset(-overlayWindow.Left, -overlayWindow.Top);
        screenArea2.Height /= 2.0;
        return (Geometry) new RectangleGeometry(screenArea2);
      case DropTargetType.AnchorablePaneDockRight:
        Rect screenArea3 = this.TargetElement.GetScreenArea();
        screenArea3.Offset(-overlayWindow.Left, -overlayWindow.Top);
        screenArea3.Offset(screenArea3.Width / 2.0, 0.0);
        screenArea3.Width /= 2.0;
        return (Geometry) new RectangleGeometry(screenArea3);
      case DropTargetType.AnchorablePaneDockBottom:
        Rect screenArea4 = this.TargetElement.GetScreenArea();
        screenArea4.Offset(-overlayWindow.Left, -overlayWindow.Top);
        screenArea4.Offset(0.0, screenArea4.Height / 2.0);
        screenArea4.Height /= 2.0;
        return (Geometry) new RectangleGeometry(screenArea4);
      case DropTargetType.AnchorablePaneDockInside:
        Rect screenArea5 = this.TargetElement.GetScreenArea();
        screenArea5.Offset(-overlayWindow.Left, -overlayWindow.Top);
        if (this._tabIndex == -1)
          return (Geometry) new RectangleGeometry(screenArea5);
        Rect rect = new Rect(this.DetectionRects[0].TopLeft, this.DetectionRects[0].BottomRight);
        rect.Offset(-overlayWindow.Left, -overlayWindow.Top);
        PathFigure pathFigure = new PathFigure();
        pathFigure.StartPoint = screenArea5.TopLeft;
        pathFigure.Segments.Add((PathSegment) new LineSegment()
        {
          Point = new Point(screenArea5.Left, rect.Top)
        });
        pathFigure.Segments.Add((PathSegment) new LineSegment()
        {
          Point = rect.TopLeft
        });
        pathFigure.Segments.Add((PathSegment) new LineSegment()
        {
          Point = rect.BottomLeft
        });
        pathFigure.Segments.Add((PathSegment) new LineSegment()
        {
          Point = rect.BottomRight
        });
        pathFigure.Segments.Add((PathSegment) new LineSegment()
        {
          Point = rect.TopRight
        });
        pathFigure.Segments.Add((PathSegment) new LineSegment()
        {
          Point = new Point(screenArea5.Right, rect.Top)
        });
        pathFigure.Segments.Add((PathSegment) new LineSegment()
        {
          Point = screenArea5.TopRight
        });
        pathFigure.IsClosed = true;
        pathFigure.IsFilled = true;
        pathFigure.Freeze();
        return (Geometry) new PathGeometry((IEnumerable<PathFigure>) new PathFigure[1]
        {
          pathFigure
        });
      default:
        return (Geometry) null;
    }
  }
}
