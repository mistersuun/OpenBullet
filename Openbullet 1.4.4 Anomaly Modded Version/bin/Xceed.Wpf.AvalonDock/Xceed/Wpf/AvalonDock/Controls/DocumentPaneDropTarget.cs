// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.DocumentPaneDropTarget
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

internal class DocumentPaneDropTarget : DropTarget<LayoutDocumentPaneControl>
{
  private LayoutDocumentPaneControl _targetPane;
  private int _tabIndex = -1;

  internal DocumentPaneDropTarget(
    LayoutDocumentPaneControl paneControl,
    Rect detectionRect,
    DropTargetType type)
    : base(paneControl, detectionRect, type)
  {
    this._targetPane = paneControl;
  }

  internal DocumentPaneDropTarget(
    LayoutDocumentPaneControl paneControl,
    Rect detectionRect,
    DropTargetType type,
    int tabIndex)
    : base(paneControl, detectionRect, type)
  {
    this._targetPane = paneControl;
    this._tabIndex = tabIndex;
  }

  protected override void Drop(LayoutDocumentFloatingWindow floatingWindow)
  {
    ILayoutDocumentPane model = this._targetPane.Model as ILayoutDocumentPane;
    switch (this.Type)
    {
      case DropTargetType.DocumentPaneDockLeft:
        LayoutDocumentPane layoutDocumentPane1 = new LayoutDocumentPane((LayoutContent) floatingWindow.RootDocument);
        if (!(model.Parent is LayoutDocumentPaneGroup parent1))
        {
          ILayoutContainer parent = model.Parent;
          LayoutDocumentPaneGroup documentPaneGroup = new LayoutDocumentPaneGroup()
          {
            Orientation = Orientation.Horizontal
          };
          ILayoutDocumentPane oldElement = model;
          LayoutDocumentPaneGroup newElement = documentPaneGroup;
          parent.ReplaceChild((ILayoutElement) oldElement, (ILayoutElement) newElement);
          documentPaneGroup.Children.Add(model);
          documentPaneGroup.Children.Insert(0, (ILayoutDocumentPane) layoutDocumentPane1);
          break;
        }
        if (!parent1.Root.Manager.AllowMixedOrientation || parent1.Orientation == Orientation.Horizontal)
        {
          parent1.Orientation = Orientation.Horizontal;
          int index = parent1.IndexOfChild((ILayoutElement) model);
          parent1.Children.Insert(index, (ILayoutDocumentPane) layoutDocumentPane1);
          break;
        }
        LayoutDocumentPaneGroup newElement1 = new LayoutDocumentPaneGroup();
        newElement1.Orientation = Orientation.Horizontal;
        parent1.ReplaceChild((ILayoutElement) model, (ILayoutElement) newElement1);
        newElement1.Children.Add((ILayoutDocumentPane) layoutDocumentPane1);
        newElement1.Children.Add(model);
        break;
      case DropTargetType.DocumentPaneDockTop:
        LayoutDocumentPane layoutDocumentPane2 = new LayoutDocumentPane((LayoutContent) floatingWindow.RootDocument);
        if (!(model.Parent is LayoutDocumentPaneGroup parent3))
        {
          ILayoutContainer parent2 = model.Parent;
          LayoutDocumentPaneGroup documentPaneGroup = new LayoutDocumentPaneGroup()
          {
            Orientation = Orientation.Vertical
          };
          ILayoutDocumentPane oldElement = model;
          LayoutDocumentPaneGroup newElement2 = documentPaneGroup;
          parent2.ReplaceChild((ILayoutElement) oldElement, (ILayoutElement) newElement2);
          documentPaneGroup.Children.Add((ILayoutDocumentPane) (model as LayoutDocumentPane));
          documentPaneGroup.Children.Insert(0, (ILayoutDocumentPane) layoutDocumentPane2);
          break;
        }
        if (!parent3.Root.Manager.AllowMixedOrientation || parent3.Orientation == Orientation.Vertical)
        {
          parent3.Orientation = Orientation.Vertical;
          int index = parent3.IndexOfChild((ILayoutElement) model);
          parent3.Children.Insert(index, (ILayoutDocumentPane) layoutDocumentPane2);
          break;
        }
        LayoutDocumentPaneGroup newElement3 = new LayoutDocumentPaneGroup();
        newElement3.Orientation = Orientation.Vertical;
        parent3.ReplaceChild((ILayoutElement) model, (ILayoutElement) newElement3);
        newElement3.Children.Add((ILayoutDocumentPane) layoutDocumentPane2);
        newElement3.Children.Add(model);
        break;
      case DropTargetType.DocumentPaneDockRight:
        LayoutDocumentPane layoutDocumentPane3 = new LayoutDocumentPane((LayoutContent) floatingWindow.RootDocument);
        if (!(model.Parent is LayoutDocumentPaneGroup parent5))
        {
          ILayoutContainer parent4 = model.Parent;
          LayoutDocumentPaneGroup documentPaneGroup = new LayoutDocumentPaneGroup()
          {
            Orientation = Orientation.Horizontal
          };
          ILayoutDocumentPane oldElement = model;
          LayoutDocumentPaneGroup newElement4 = documentPaneGroup;
          parent4.ReplaceChild((ILayoutElement) oldElement, (ILayoutElement) newElement4);
          documentPaneGroup.Children.Add((ILayoutDocumentPane) (model as LayoutDocumentPane));
          documentPaneGroup.Children.Add((ILayoutDocumentPane) layoutDocumentPane3);
          break;
        }
        if (!parent5.Root.Manager.AllowMixedOrientation || parent5.Orientation == Orientation.Horizontal)
        {
          parent5.Orientation = Orientation.Horizontal;
          int num = parent5.IndexOfChild((ILayoutElement) model);
          parent5.Children.Insert(num + 1, (ILayoutDocumentPane) layoutDocumentPane3);
          break;
        }
        LayoutDocumentPaneGroup newElement5 = new LayoutDocumentPaneGroup();
        newElement5.Orientation = Orientation.Horizontal;
        parent5.ReplaceChild((ILayoutElement) model, (ILayoutElement) newElement5);
        newElement5.Children.Add(model);
        newElement5.Children.Add((ILayoutDocumentPane) layoutDocumentPane3);
        break;
      case DropTargetType.DocumentPaneDockBottom:
        LayoutDocumentPane layoutDocumentPane4 = new LayoutDocumentPane((LayoutContent) floatingWindow.RootDocument);
        if (!(model.Parent is LayoutDocumentPaneGroup parent7))
        {
          ILayoutContainer parent6 = model.Parent;
          LayoutDocumentPaneGroup documentPaneGroup = new LayoutDocumentPaneGroup()
          {
            Orientation = Orientation.Vertical
          };
          ILayoutDocumentPane oldElement = model;
          LayoutDocumentPaneGroup newElement6 = documentPaneGroup;
          parent6.ReplaceChild((ILayoutElement) oldElement, (ILayoutElement) newElement6);
          documentPaneGroup.Children.Add((ILayoutDocumentPane) (model as LayoutDocumentPane));
          documentPaneGroup.Children.Add((ILayoutDocumentPane) layoutDocumentPane4);
          break;
        }
        if (!parent7.Root.Manager.AllowMixedOrientation || parent7.Orientation == Orientation.Vertical)
        {
          parent7.Orientation = Orientation.Vertical;
          int num = parent7.IndexOfChild((ILayoutElement) model);
          parent7.Children.Insert(num + 1, (ILayoutDocumentPane) layoutDocumentPane4);
          break;
        }
        LayoutDocumentPaneGroup newElement7 = new LayoutDocumentPaneGroup();
        newElement7.Orientation = Orientation.Vertical;
        parent7.ReplaceChild((ILayoutElement) model, (ILayoutElement) newElement7);
        newElement7.Children.Add(model);
        newElement7.Children.Add((ILayoutDocumentPane) layoutDocumentPane4);
        break;
      case DropTargetType.DocumentPaneDockInside:
        LayoutDocumentPane layoutDocumentPane5 = model as LayoutDocumentPane;
        LayoutDocument rootDocument = floatingWindow.RootDocument;
        int index1;
        if (this._tabIndex != -1)
        {
          index1 = this._tabIndex;
        }
        else
        {
          int num = 0;
          if (((ILayoutPreviousContainer) rootDocument).PreviousContainer == model && rootDocument.PreviousContainerIndex != -1)
            num = rootDocument.PreviousContainerIndex;
          index1 = num;
        }
        rootDocument.IsActive = false;
        layoutDocumentPane5.Children.Insert(index1, (LayoutContent) rootDocument);
        rootDocument.IsActive = true;
        break;
    }
    base.Drop(floatingWindow);
  }

  protected override void Drop(LayoutAnchorableFloatingWindow floatingWindow)
  {
    ILayoutDocumentPane model = this._targetPane.Model as ILayoutDocumentPane;
    switch (this.Type)
    {
      case DropTargetType.DocumentPaneDockLeft:
        LayoutDocumentPaneGroup parent1 = model.Parent as LayoutDocumentPaneGroup;
        LayoutDocumentPane layoutDocumentPane1 = new LayoutDocumentPane();
        if (parent1 == null)
        {
          ILayoutContainer parent2 = model.Parent;
          LayoutDocumentPaneGroup documentPaneGroup = new LayoutDocumentPaneGroup()
          {
            Orientation = Orientation.Horizontal
          };
          ILayoutDocumentPane oldElement = model;
          LayoutDocumentPaneGroup newElement = documentPaneGroup;
          parent2.ReplaceChild((ILayoutElement) oldElement, (ILayoutElement) newElement);
          documentPaneGroup.Children.Add((ILayoutDocumentPane) layoutDocumentPane1);
          documentPaneGroup.Children.Add((ILayoutDocumentPane) (model as LayoutDocumentPane));
        }
        else if (!parent1.Root.Manager.AllowMixedOrientation || parent1.Orientation == Orientation.Horizontal)
        {
          parent1.Orientation = Orientation.Horizontal;
          int index = parent1.IndexOfChild((ILayoutElement) model);
          parent1.Children.Insert(index, (ILayoutDocumentPane) layoutDocumentPane1);
        }
        else
        {
          LayoutDocumentPaneGroup newElement = new LayoutDocumentPaneGroup();
          newElement.Orientation = Orientation.Horizontal;
          parent1.ReplaceChild((ILayoutElement) model, (ILayoutElement) newElement);
          newElement.Children.Add((ILayoutDocumentPane) layoutDocumentPane1);
          newElement.Children.Add(model);
        }
        foreach (LayoutAnchorable layoutAnchorable in floatingWindow.RootPanel.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>())
          layoutDocumentPane1.Children.Add((LayoutContent) layoutAnchorable);
        break;
      case DropTargetType.DocumentPaneDockTop:
        LayoutDocumentPaneGroup parent3 = model.Parent as LayoutDocumentPaneGroup;
        LayoutDocumentPane layoutDocumentPane2 = new LayoutDocumentPane();
        if (parent3 == null)
        {
          ILayoutContainer parent4 = model.Parent;
          LayoutDocumentPaneGroup documentPaneGroup = new LayoutDocumentPaneGroup()
          {
            Orientation = Orientation.Vertical
          };
          ILayoutDocumentPane oldElement = model;
          LayoutDocumentPaneGroup newElement = documentPaneGroup;
          parent4.ReplaceChild((ILayoutElement) oldElement, (ILayoutElement) newElement);
          documentPaneGroup.Children.Add((ILayoutDocumentPane) layoutDocumentPane2);
          documentPaneGroup.Children.Add((ILayoutDocumentPane) (model as LayoutDocumentPane));
        }
        else if (!parent3.Root.Manager.AllowMixedOrientation || parent3.Orientation == Orientation.Vertical)
        {
          parent3.Orientation = Orientation.Vertical;
          int index = parent3.IndexOfChild((ILayoutElement) model);
          parent3.Children.Insert(index, (ILayoutDocumentPane) layoutDocumentPane2);
        }
        else
        {
          LayoutDocumentPaneGroup newElement = new LayoutDocumentPaneGroup();
          newElement.Orientation = Orientation.Vertical;
          parent3.ReplaceChild((ILayoutElement) model, (ILayoutElement) newElement);
          newElement.Children.Add((ILayoutDocumentPane) layoutDocumentPane2);
          newElement.Children.Add(model);
        }
        foreach (LayoutAnchorable layoutAnchorable in floatingWindow.RootPanel.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>())
          layoutDocumentPane2.Children.Add((LayoutContent) layoutAnchorable);
        break;
      case DropTargetType.DocumentPaneDockRight:
        LayoutDocumentPaneGroup parent5 = model.Parent as LayoutDocumentPaneGroup;
        LayoutDocumentPane layoutDocumentPane3 = new LayoutDocumentPane();
        if (parent5 == null)
        {
          ILayoutContainer parent6 = model.Parent;
          LayoutDocumentPaneGroup documentPaneGroup = new LayoutDocumentPaneGroup()
          {
            Orientation = Orientation.Horizontal
          };
          ILayoutDocumentPane oldElement = model;
          LayoutDocumentPaneGroup newElement = documentPaneGroup;
          parent6.ReplaceChild((ILayoutElement) oldElement, (ILayoutElement) newElement);
          documentPaneGroup.Children.Add((ILayoutDocumentPane) (model as LayoutDocumentPane));
          documentPaneGroup.Children.Add((ILayoutDocumentPane) layoutDocumentPane3);
        }
        else if (!parent5.Root.Manager.AllowMixedOrientation || parent5.Orientation == Orientation.Horizontal)
        {
          parent5.Orientation = Orientation.Horizontal;
          int num = parent5.IndexOfChild((ILayoutElement) model);
          parent5.Children.Insert(num + 1, (ILayoutDocumentPane) layoutDocumentPane3);
        }
        else
        {
          LayoutDocumentPaneGroup newElement = new LayoutDocumentPaneGroup();
          newElement.Orientation = Orientation.Horizontal;
          parent5.ReplaceChild((ILayoutElement) model, (ILayoutElement) newElement);
          newElement.Children.Add(model);
          newElement.Children.Add((ILayoutDocumentPane) layoutDocumentPane3);
        }
        foreach (LayoutAnchorable layoutAnchorable in floatingWindow.RootPanel.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>())
          layoutDocumentPane3.Children.Add((LayoutContent) layoutAnchorable);
        break;
      case DropTargetType.DocumentPaneDockBottom:
        LayoutDocumentPaneGroup parent7 = model.Parent as LayoutDocumentPaneGroup;
        LayoutDocumentPane layoutDocumentPane4 = new LayoutDocumentPane();
        if (parent7 == null)
        {
          ILayoutContainer parent8 = model.Parent;
          LayoutDocumentPaneGroup documentPaneGroup = new LayoutDocumentPaneGroup()
          {
            Orientation = Orientation.Vertical
          };
          ILayoutDocumentPane oldElement = model;
          LayoutDocumentPaneGroup newElement = documentPaneGroup;
          parent8.ReplaceChild((ILayoutElement) oldElement, (ILayoutElement) newElement);
          documentPaneGroup.Children.Add((ILayoutDocumentPane) (model as LayoutDocumentPane));
          documentPaneGroup.Children.Add((ILayoutDocumentPane) layoutDocumentPane4);
        }
        else if (!parent7.Root.Manager.AllowMixedOrientation || parent7.Orientation == Orientation.Vertical)
        {
          parent7.Orientation = Orientation.Vertical;
          int num = parent7.IndexOfChild((ILayoutElement) model);
          parent7.Children.Insert(num + 1, (ILayoutDocumentPane) layoutDocumentPane4);
        }
        else
        {
          LayoutDocumentPaneGroup newElement = new LayoutDocumentPaneGroup();
          newElement.Orientation = Orientation.Vertical;
          parent7.ReplaceChild((ILayoutElement) model, (ILayoutElement) newElement);
          newElement.Children.Add(model);
          newElement.Children.Add((ILayoutDocumentPane) layoutDocumentPane4);
        }
        foreach (LayoutAnchorable layoutAnchorable in floatingWindow.RootPanel.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>())
          layoutDocumentPane4.Children.Add((LayoutContent) layoutAnchorable);
        break;
      case DropTargetType.DocumentPaneDockInside:
        LayoutDocumentPane layoutDocumentPane5 = model as LayoutDocumentPane;
        LayoutAnchorablePaneGroup rootPanel = floatingWindow.RootPanel;
        bool flag = true;
        int index1 = 0;
        if (this._tabIndex != -1)
        {
          index1 = this._tabIndex;
          flag = false;
        }
        LayoutAnchorable layoutAnchorable1 = (LayoutAnchorable) null;
        foreach (LayoutAnchorable layoutAnchorable2 in rootPanel.Descendents().OfType<LayoutAnchorable>().ToArray<LayoutAnchorable>())
        {
          if (flag)
          {
            if (((ILayoutPreviousContainer) layoutAnchorable2).PreviousContainer == model && layoutAnchorable2.PreviousContainerIndex != -1)
              index1 = layoutAnchorable2.PreviousContainerIndex;
            flag = false;
          }
          layoutAnchorable2.SetCanCloseInternal(true);
          layoutDocumentPane5.Children.Insert(index1, (LayoutContent) layoutAnchorable2);
          ++index1;
          layoutAnchorable1 = layoutAnchorable2;
        }
        layoutAnchorable1.IsActive = true;
        break;
    }
    base.Drop(floatingWindow);
  }

  public override Geometry GetPreviewPath(
    OverlayWindow overlayWindow,
    LayoutFloatingWindow floatingWindowModel)
  {
    switch (this.Type)
    {
      case DropTargetType.DocumentPaneDockLeft:
        Rect screenArea1 = this.TargetElement.GetScreenArea();
        screenArea1.Offset(-overlayWindow.Left, -overlayWindow.Top);
        screenArea1.Width /= 2.0;
        return (Geometry) new RectangleGeometry(screenArea1);
      case DropTargetType.DocumentPaneDockTop:
        Rect screenArea2 = this.TargetElement.GetScreenArea();
        screenArea2.Offset(-overlayWindow.Left, -overlayWindow.Top);
        screenArea2.Height /= 2.0;
        return (Geometry) new RectangleGeometry(screenArea2);
      case DropTargetType.DocumentPaneDockRight:
        Rect screenArea3 = this.TargetElement.GetScreenArea();
        screenArea3.Offset(-overlayWindow.Left, -overlayWindow.Top);
        screenArea3.Offset(screenArea3.Width / 2.0, 0.0);
        screenArea3.Width /= 2.0;
        return (Geometry) new RectangleGeometry(screenArea3);
      case DropTargetType.DocumentPaneDockBottom:
        Rect screenArea4 = this.TargetElement.GetScreenArea();
        screenArea4.Offset(-overlayWindow.Left, -overlayWindow.Top);
        screenArea4.Offset(0.0, screenArea4.Height / 2.0);
        screenArea4.Height /= 2.0;
        return (Geometry) new RectangleGeometry(screenArea4);
      case DropTargetType.DocumentPaneDockInside:
        Rect screenArea5 = this.TargetElement.GetScreenArea();
        screenArea5.Offset(-overlayWindow.Left, -overlayWindow.Top);
        if (this._tabIndex == -1)
          return (Geometry) new RectangleGeometry(screenArea5);
        Rect rect = new Rect(this.DetectionRects[0].TopLeft, this.DetectionRects[0].BottomRight);
        rect.Offset(-overlayWindow.Left, -overlayWindow.Top);
        PathFigure pathFigure = new PathFigure();
        pathFigure.StartPoint = screenArea5.BottomRight;
        pathFigure.Segments.Add((PathSegment) new LineSegment()
        {
          Point = new Point(screenArea5.Right, rect.Bottom)
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
          Point = rect.TopLeft
        });
        pathFigure.Segments.Add((PathSegment) new LineSegment()
        {
          Point = rect.BottomLeft
        });
        pathFigure.Segments.Add((PathSegment) new LineSegment()
        {
          Point = new Point(screenArea5.Left, rect.Bottom)
        });
        pathFigure.Segments.Add((PathSegment) new LineSegment()
        {
          Point = screenArea5.BottomLeft
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
