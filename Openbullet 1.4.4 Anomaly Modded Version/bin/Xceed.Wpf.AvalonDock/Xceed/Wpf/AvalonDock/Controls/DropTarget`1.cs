// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.DropTarget`1
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal abstract class DropTarget<T> : DropTargetBase, IDropTarget where T : FrameworkElement
{
  private Rect[] _detectionRect;
  private T _targetElement;
  private DropTargetType _type;

  protected DropTarget(T targetElement, Rect detectionRect, DropTargetType type)
  {
    this._targetElement = targetElement;
    this._detectionRect = new Rect[1]{ detectionRect };
    this._type = type;
  }

  protected DropTarget(T targetElement, IEnumerable<Rect> detectionRects, DropTargetType type)
  {
    this._targetElement = targetElement;
    this._detectionRect = detectionRects.ToArray<Rect>();
    this._type = type;
  }

  public Rect[] DetectionRects => this._detectionRect;

  public T TargetElement => this._targetElement;

  public DropTargetType Type => this._type;

  protected virtual void Drop(LayoutAnchorableFloatingWindow floatingWindow)
  {
  }

  protected virtual void Drop(LayoutDocumentFloatingWindow floatingWindow)
  {
  }

  public void Drop(LayoutFloatingWindow floatingWindow)
  {
    ILayoutRoot root = floatingWindow.Root;
    LayoutContent currentActiveContent = floatingWindow.Root.ActiveContent;
    if (floatingWindow is LayoutAnchorableFloatingWindow floatingWindow1)
      this.Drop(floatingWindow1);
    else
      this.Drop(floatingWindow as LayoutDocumentFloatingWindow);
    this.Dispatcher.BeginInvoke((Delegate) (() =>
    {
      currentActiveContent.IsSelected = false;
      currentActiveContent.IsActive = false;
      currentActiveContent.IsActive = true;
    }), DispatcherPriority.Background);
  }

  public virtual bool HitTest(Point dragPoint)
  {
    return ((IEnumerable<Rect>) this._detectionRect).Any<Rect>((Func<Rect, bool>) (dr => dr.Contains(dragPoint)));
  }

  public abstract Geometry GetPreviewPath(
    OverlayWindow overlayWindow,
    LayoutFloatingWindow floatingWindow);

  public void DragEnter()
  {
    DropTargetBase.SetIsDraggingOver((DependencyObject) this.TargetElement, true);
  }

  public void DragLeave()
  {
    DropTargetBase.SetIsDraggingOver((DependencyObject) this.TargetElement, false);
  }
}
