// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutGridControl`1
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public abstract class LayoutGridControl<T> : Grid, ILayoutControl where T : class, ILayoutPanelElement
{
  private LayoutPositionableGroup<T> _model;
  private Orientation _orientation;
  private bool _initialized;
  private ChildrenTreeChange? _asyncRefreshCalled;
  private ReentrantFlag _fixingChildrenDockLengths = new ReentrantFlag();
  private Border _resizerGhost;
  private Window _resizerWindowHost;
  private Vector _initialStartPoint;

  internal LayoutGridControl(LayoutPositionableGroup<T> model, Orientation orientation)
  {
    this._model = model != null ? model : throw new ArgumentNullException(nameof (model));
    this._orientation = orientation;
    this.FlowDirection = FlowDirection.LeftToRight;
  }

  public ILayoutElement Model => (ILayoutElement) this._model;

  public Orientation Orientation => (this._model as ILayoutOrientableGroup).Orientation;

  private bool AsyncRefreshCalled => this._asyncRefreshCalled.HasValue;

  protected override void OnInitialized(EventArgs e)
  {
    base.OnInitialized(e);
    this._model.ChildrenTreeChanged += (EventHandler<ChildrenTreeChangedEventArgs>) ((s, args) =>
    {
      if (this._asyncRefreshCalled.HasValue && this._asyncRefreshCalled.Value == args.Change)
        return;
      this._asyncRefreshCalled = new ChildrenTreeChange?(args.Change);
      this.Dispatcher.BeginInvoke((Delegate) (() =>
      {
        this._asyncRefreshCalled = new ChildrenTreeChange?();
        this.UpdateChildren();
      }), DispatcherPriority.Normal, (object[]) null);
    });
    this.LayoutUpdated += new EventHandler(this.OnLayoutUpdated);
  }

  protected void FixChildrenDockLengths()
  {
    using (this._fixingChildrenDockLengths.Enter())
      this.OnFixChildrenDockLengths();
  }

  protected abstract void OnFixChildrenDockLengths();

  private void OnLayoutUpdated(object sender, EventArgs e)
  {
    LayoutPositionableGroup<T> model = this._model;
    ((ILayoutPositionableElementWithActualSize) model).ActualWidth = this.ActualWidth;
    ((ILayoutPositionableElementWithActualSize) model).ActualHeight = this.ActualHeight;
    if (this._initialized)
      return;
    this._initialized = true;
    this.UpdateChildren();
  }

  private void UpdateChildren()
  {
    ILayoutControl[] array = this.Children.OfType<ILayoutControl>().ToArray<ILayoutControl>();
    this.DetachOldSplitters();
    this.DetachPropertChangeHandler();
    this.Children.Clear();
    this.ColumnDefinitions.Clear();
    this.RowDefinitions.Clear();
    if (this._model == null || this._model.Root == null)
      return;
    DockingManager manager = this._model.Root.Manager;
    if (manager == null)
      return;
    foreach (T child1 in (Collection<T>) this._model.Children)
    {
      ILayoutElement child = (ILayoutElement) child1;
      ILayoutControl element = ((IEnumerable<ILayoutControl>) array).FirstOrDefault<ILayoutControl>((Func<ILayoutControl, bool>) (chVM => chVM.Model == child));
      if (element != null)
        this.Children.Add(element as UIElement);
      else
        this.Children.Add(manager.CreateUIElementForModel(child));
    }
    this.CreateSplitters();
    this.UpdateRowColDefinitions();
    this.AttachNewSplitters();
    this.AttachPropertyChangeHandler();
  }

  private void AttachPropertyChangeHandler()
  {
    foreach (ILayoutControl layoutControl in this.InternalChildren.OfType<ILayoutControl>())
      layoutControl.Model.PropertyChanged += new PropertyChangedEventHandler(this.OnChildModelPropertyChanged);
  }

  private void DetachPropertChangeHandler()
  {
    foreach (ILayoutControl layoutControl in this.InternalChildren.OfType<ILayoutControl>())
      layoutControl.Model.PropertyChanged -= new PropertyChangedEventHandler(this.OnChildModelPropertyChanged);
  }

  private void OnChildModelPropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    if (this.AsyncRefreshCalled)
      return;
    if (this._fixingChildrenDockLengths.CanEnter && e.PropertyName == "DockWidth" && this.Orientation == Orientation.Horizontal)
    {
      if (this.ColumnDefinitions.Count != this.InternalChildren.Count)
        return;
      ILayoutPositionableElement changedElement = sender as ILayoutPositionableElement;
      this.ColumnDefinitions[this.InternalChildren.IndexOf(this.InternalChildren.OfType<ILayoutControl>().First<ILayoutControl>((Func<ILayoutControl, bool>) (ch => ch.Model == changedElement)) as UIElement)].Width = changedElement.DockWidth;
    }
    else if (this._fixingChildrenDockLengths.CanEnter && e.PropertyName == "DockHeight" && this.Orientation == Orientation.Vertical)
    {
      if (this.RowDefinitions.Count != this.InternalChildren.Count)
        return;
      ILayoutPositionableElement changedElement = sender as ILayoutPositionableElement;
      this.RowDefinitions[this.InternalChildren.IndexOf(this.InternalChildren.OfType<ILayoutControl>().First<ILayoutControl>((Func<ILayoutControl, bool>) (ch => ch.Model == changedElement)) as UIElement)].Height = changedElement.DockHeight;
    }
    else
    {
      if (!(e.PropertyName == "IsVisible"))
        return;
      this.UpdateRowColDefinitions();
    }
  }

  private void UpdateRowColDefinitions()
  {
    ILayoutRoot root = this._model.Root;
    if (root == null)
      return;
    DockingManager manager = root.Manager;
    if (manager == null)
      return;
    this.FixChildrenDockLengths();
    this.RowDefinitions.Clear();
    this.ColumnDefinitions.Clear();
    if (this.Orientation == Orientation.Horizontal)
    {
      int num = 0;
      int index1 = 0;
      int index2 = 0;
      while (index2 < this._model.Children.Count)
      {
        ILayoutPositionableElement child = (object) this._model.Children[index2] as ILayoutPositionableElement;
        this.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = child.IsVisible ? child.DockWidth : new GridLength(0.0, GridUnitType.Pixel),
          MinWidth = child.IsVisible ? child.DockMinWidth : 0.0
        });
        Grid.SetColumn(this.InternalChildren[index1], num);
        if (index1 < this.InternalChildren.Count - 1)
        {
          ++index1;
          ++num;
          bool flag = false;
          for (int index3 = index2 + 1; index3 < this._model.Children.Count; ++index3)
          {
            if (((object) this._model.Children[index3] as ILayoutPositionableElement).IsVisible)
            {
              flag = true;
              break;
            }
          }
          this.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = child.IsVisible & flag ? new GridLength(manager.GridSplitterWidth) : new GridLength(0.0, GridUnitType.Pixel)
          });
          Grid.SetColumn(this.InternalChildren[index1], num);
        }
        ++index2;
        ++num;
        ++index1;
      }
    }
    else
    {
      int num = 0;
      int index4 = 0;
      int index5 = 0;
      while (index5 < this._model.Children.Count)
      {
        ILayoutPositionableElement child = (object) this._model.Children[index5] as ILayoutPositionableElement;
        this.RowDefinitions.Add(new RowDefinition()
        {
          Height = child.IsVisible ? child.DockHeight : new GridLength(0.0, GridUnitType.Pixel),
          MinHeight = child.IsVisible ? child.DockMinHeight : 0.0
        });
        Grid.SetRow(this.InternalChildren[index4], num);
        if (index4 < this.InternalChildren.Count - 1)
        {
          ++index4;
          ++num;
          bool flag = false;
          for (int index6 = index5 + 1; index6 < this._model.Children.Count; ++index6)
          {
            if (((object) this._model.Children[index6] as ILayoutPositionableElement).IsVisible)
            {
              flag = true;
              break;
            }
          }
          this.RowDefinitions.Add(new RowDefinition()
          {
            Height = child.IsVisible & flag ? new GridLength(manager.GridSplitterHeight) : new GridLength(0.0, GridUnitType.Pixel)
          });
          Grid.SetRow(this.InternalChildren[index4], num);
        }
        ++index5;
        ++num;
        ++index4;
      }
    }
  }

  private void CreateSplitters()
  {
    for (int index = 1; index < this.Children.Count; index = index + 1 + 1)
    {
      LayoutGridResizerControl element = new LayoutGridResizerControl();
      element.Cursor = this.Orientation == Orientation.Horizontal ? Cursors.SizeWE : Cursors.SizeNS;
      this.Children.Insert(index, (UIElement) element);
    }
  }

  private void DetachOldSplitters()
  {
    foreach (LayoutGridResizerControl gridResizerControl in this.Children.OfType<LayoutGridResizerControl>())
    {
      gridResizerControl.DragStarted -= new DragStartedEventHandler(this.OnSplitterDragStarted);
      gridResizerControl.DragDelta -= new DragDeltaEventHandler(this.OnSplitterDragDelta);
      gridResizerControl.DragCompleted -= new DragCompletedEventHandler(this.OnSplitterDragCompleted);
    }
  }

  private void AttachNewSplitters()
  {
    foreach (LayoutGridResizerControl gridResizerControl in this.Children.OfType<LayoutGridResizerControl>())
    {
      gridResizerControl.DragStarted += new DragStartedEventHandler(this.OnSplitterDragStarted);
      gridResizerControl.DragDelta += new DragDeltaEventHandler(this.OnSplitterDragDelta);
      gridResizerControl.DragCompleted += new DragCompletedEventHandler(this.OnSplitterDragCompleted);
    }
  }

  private void OnSplitterDragStarted(object sender, DragStartedEventArgs e)
  {
    this.ShowResizerOverlayWindow(sender as LayoutGridResizerControl);
  }

  private void OnSplitterDragDelta(object sender, DragDeltaEventArgs e)
  {
    GeneralTransform ancestor = this.TransformToAncestor(this.FindVisualTreeRoot() as Visual);
    Vector vector = ancestor.Transform(new Point(e.HorizontalChange, e.VerticalChange)) - ancestor.Transform(new Point());
    if (this.Orientation == Orientation.Horizontal)
      Canvas.SetLeft((UIElement) this._resizerGhost, MathHelper.MinMax(this._initialStartPoint.X + vector.X, 0.0, this._resizerWindowHost.Width - this._resizerGhost.Width));
    else
      Canvas.SetTop((UIElement) this._resizerGhost, MathHelper.MinMax(this._initialStartPoint.Y + vector.Y, 0.0, this._resizerWindowHost.Height - this._resizerGhost.Height));
  }

  private void OnSplitterDragCompleted(object sender, DragCompletedEventArgs e)
  {
    LayoutGridResizerControl element = sender as LayoutGridResizerControl;
    GeneralTransform ancestor1 = this.TransformToAncestor(this.FindVisualTreeRoot() as Visual);
    Vector vector = ancestor1.Transform(new Point(e.HorizontalChange, e.VerticalChange)) - ancestor1.Transform(new Point());
    double num = this.Orientation != Orientation.Horizontal ? Canvas.GetTop((UIElement) this._resizerGhost) - this._initialStartPoint.Y : Canvas.GetLeft((UIElement) this._resizerGhost) - this._initialStartPoint.X;
    int index = this.InternalChildren.IndexOf((UIElement) element);
    FrameworkElement internalChild = this.InternalChildren[index - 1] as FrameworkElement;
    FrameworkElement nextVisibleChild = this.GetNextVisibleChild(index);
    Size ancestor2 = internalChild.TransformActualSizeToAncestor();
    Size ancestor3 = nextVisibleChild.TransformActualSizeToAncestor();
    ILayoutPositionableElement model1 = (ILayoutPositionableElement) (internalChild as ILayoutControl).Model;
    ILayoutPositionableElement model2 = (ILayoutPositionableElement) (nextVisibleChild as ILayoutControl).Model;
    if (this.Orientation == Orientation.Horizontal)
    {
      GridLength dockWidth = model1.DockWidth;
      if (dockWidth.IsStar)
      {
        ILayoutPositionableElement positionableElement = model1;
        dockWidth = model1.DockWidth;
        GridLength gridLength = new GridLength(dockWidth.Value * (ancestor2.Width + num) / ancestor2.Width, GridUnitType.Star);
        positionableElement.DockWidth = gridLength;
      }
      else
      {
        ILayoutPositionableElement positionableElement = model1;
        dockWidth = model1.DockWidth;
        GridLength gridLength = new GridLength(dockWidth.Value + num, GridUnitType.Pixel);
        positionableElement.DockWidth = gridLength;
      }
      dockWidth = model2.DockWidth;
      if (dockWidth.IsStar)
      {
        ILayoutPositionableElement positionableElement = model2;
        dockWidth = model2.DockWidth;
        GridLength gridLength = new GridLength(dockWidth.Value * (ancestor3.Width - num) / ancestor3.Width, GridUnitType.Star);
        positionableElement.DockWidth = gridLength;
      }
      else
      {
        ILayoutPositionableElement positionableElement = model2;
        dockWidth = model2.DockWidth;
        GridLength gridLength = new GridLength(dockWidth.Value - num, GridUnitType.Pixel);
        positionableElement.DockWidth = gridLength;
      }
    }
    else
    {
      GridLength dockHeight = model1.DockHeight;
      if (dockHeight.IsStar)
      {
        ILayoutPositionableElement positionableElement = model1;
        dockHeight = model1.DockHeight;
        GridLength gridLength = new GridLength(dockHeight.Value * (ancestor2.Height + num) / ancestor2.Height, GridUnitType.Star);
        positionableElement.DockHeight = gridLength;
      }
      else
      {
        ILayoutPositionableElement positionableElement = model1;
        dockHeight = model1.DockHeight;
        GridLength gridLength = new GridLength(dockHeight.Value + num, GridUnitType.Pixel);
        positionableElement.DockHeight = gridLength;
      }
      dockHeight = model2.DockHeight;
      if (dockHeight.IsStar)
      {
        ILayoutPositionableElement positionableElement = model2;
        dockHeight = model2.DockHeight;
        GridLength gridLength = new GridLength(dockHeight.Value * (ancestor3.Height - num) / ancestor3.Height, GridUnitType.Star);
        positionableElement.DockHeight = gridLength;
      }
      else
      {
        ILayoutPositionableElement positionableElement = model2;
        dockHeight = model2.DockHeight;
        GridLength gridLength = new GridLength(dockHeight.Value - num, GridUnitType.Pixel);
        positionableElement.DockHeight = gridLength;
      }
    }
    this.HideResizerOverlayWindow();
  }

  private FrameworkElement GetNextVisibleChild(int index)
  {
    for (int index1 = index + 1; index1 < this.InternalChildren.Count; ++index1)
    {
      if (!(this.InternalChildren[index1] is LayoutGridResizerControl))
      {
        GridLength gridLength;
        if (this.Orientation == Orientation.Horizontal)
        {
          gridLength = this.ColumnDefinitions[index1].Width;
          if (!gridLength.IsStar)
          {
            gridLength = this.ColumnDefinitions[index1].Width;
            if (gridLength.Value <= 0.0)
              continue;
          }
          return this.InternalChildren[index1] as FrameworkElement;
        }
        gridLength = this.RowDefinitions[index1].Height;
        if (!gridLength.IsStar)
        {
          gridLength = this.RowDefinitions[index1].Height;
          if (gridLength.Value <= 0.0)
            continue;
        }
        return this.InternalChildren[index1] as FrameworkElement;
      }
    }
    return (FrameworkElement) null;
  }

  private void ShowResizerOverlayWindow(LayoutGridResizerControl splitter)
  {
    Border border = new Border();
    border.Background = splitter.BackgroundWhileDragging;
    border.Opacity = splitter.OpacityWhileDragging;
    this._resizerGhost = border;
    int index = this.InternalChildren.IndexOf((UIElement) splitter);
    FrameworkElement internalChild = this.InternalChildren[index - 1] as FrameworkElement;
    FrameworkElement nextVisibleChild = this.GetNextVisibleChild(index);
    Size ancestor1 = internalChild.TransformActualSizeToAncestor();
    Size ancestor2 = nextVisibleChild.TransformActualSizeToAncestor();
    ILayoutPositionableElement model1 = (ILayoutPositionableElement) (internalChild as ILayoutControl).Model;
    ILayoutPositionableElement model2 = (ILayoutPositionableElement) (nextVisibleChild as ILayoutControl).Model;
    Point withoutFlowDirection = internalChild.PointToScreenDPIWithoutFlowDirection(new Point());
    Size size;
    if (this.Orientation == Orientation.Horizontal)
    {
      size = new Size(ancestor1.Width - model1.DockMinWidth + splitter.ActualWidth + ancestor2.Width - model2.DockMinWidth, ancestor2.Height);
      this._resizerGhost.Width = splitter.ActualWidth;
      this._resizerGhost.Height = size.Height;
      withoutFlowDirection.Offset(model1.DockMinWidth, 0.0);
    }
    else
    {
      size = new Size(ancestor1.Width, ancestor1.Height - model1.DockMinHeight + splitter.ActualHeight + ancestor2.Height - model2.DockMinHeight);
      this._resizerGhost.Height = splitter.ActualHeight;
      this._resizerGhost.Width = size.Width;
      withoutFlowDirection.Offset(0.0, model1.DockMinHeight);
    }
    this._initialStartPoint = splitter.PointToScreenDPIWithoutFlowDirection(new Point()) - withoutFlowDirection;
    if (this.Orientation == Orientation.Horizontal)
      Canvas.SetLeft((UIElement) this._resizerGhost, this._initialStartPoint.X);
    else
      Canvas.SetTop((UIElement) this._resizerGhost, this._initialStartPoint.Y);
    Canvas canvas1 = new Canvas();
    canvas1.HorizontalAlignment = HorizontalAlignment.Stretch;
    canvas1.VerticalAlignment = VerticalAlignment.Stretch;
    Canvas canvas2 = canvas1;
    canvas2.Children.Add((UIElement) this._resizerGhost);
    Window window = new Window();
    window.SizeToContent = SizeToContent.Manual;
    window.ResizeMode = ResizeMode.NoResize;
    window.WindowStyle = WindowStyle.None;
    window.ShowInTaskbar = false;
    window.AllowsTransparency = true;
    window.Background = (Brush) null;
    window.Width = size.Width;
    window.Height = size.Height;
    window.Left = withoutFlowDirection.X;
    window.Top = withoutFlowDirection.Y;
    window.ShowActivated = false;
    window.Content = (object) canvas2;
    this._resizerWindowHost = window;
    this._resizerWindowHost.Loaded += (RoutedEventHandler) ((s, e) => this._resizerWindowHost.SetParentToMainWindowOf((Visual) this));
    this._resizerWindowHost.Show();
  }

  private void HideResizerOverlayWindow()
  {
    if (this._resizerWindowHost == null)
      return;
    this._resizerWindowHost.Close();
    this._resizerWindowHost = (Window) null;
  }
}
