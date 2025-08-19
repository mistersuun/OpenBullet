// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutAutoHideWindowControl
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class LayoutAutoHideWindowControl : HwndHost, ILayoutControl
{
  internal LayoutAnchorableControl _internalHost;
  private LayoutAnchorControl _anchor;
  private LayoutAnchorable _model;
  private HwndSource _internalHwndSource;
  private IntPtr parentWindowHandle;
  private bool _internalHost_ContentRendered;
  private ContentPresenter _internalHostPresenter = new ContentPresenter();
  private Grid _internalGrid;
  private AnchorSide _side;
  private LayoutGridResizerControl _resizer;
  private DockingManager _manager;
  private Border _resizerGhost;
  private Window _resizerWindowHost;
  private Vector _initialStartPoint;
  public static readonly DependencyProperty AnchorableStyleProperty = DependencyProperty.Register(nameof (AnchorableStyle), typeof (Style), typeof (LayoutAutoHideWindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof (Background), typeof (Brush), typeof (LayoutAutoHideWindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));

  static LayoutAutoHideWindowControl()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutAutoHideWindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (LayoutAutoHideWindowControl)));
    UIElement.FocusableProperty.OverrideMetadata(typeof (LayoutAutoHideWindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
    Control.IsTabStopProperty.OverrideMetadata(typeof (LayoutAutoHideWindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
    UIElement.VisibilityProperty.OverrideMetadata(typeof (LayoutAutoHideWindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) Visibility.Hidden));
  }

  internal LayoutAutoHideWindowControl()
  {
  }

  public Style AnchorableStyle
  {
    get => (Style) this.GetValue(LayoutAutoHideWindowControl.AnchorableStyleProperty);
    set => this.SetValue(LayoutAutoHideWindowControl.AnchorableStyleProperty, (object) value);
  }

  public Brush Background
  {
    get => (Brush) this.GetValue(LayoutAutoHideWindowControl.BackgroundProperty);
    set => this.SetValue(LayoutAutoHideWindowControl.BackgroundProperty, (object) value);
  }

  public ILayoutElement Model => (ILayoutElement) this._model;

  internal bool IsResizing { get; private set; }

  protected override HandleRef BuildWindowCore(HandleRef hwndParent)
  {
    this.parentWindowHandle = hwndParent.Handle;
    this._internalHwndSource = new HwndSource(new HwndSourceParameters()
    {
      ParentWindow = hwndParent.Handle,
      WindowStyle = 1442840576 /*0x56000000*/,
      Width = 0,
      Height = 0
    });
    this._internalHost_ContentRendered = false;
    this._internalHwndSource.ContentRendered += new EventHandler(this._internalHwndSource_ContentRendered);
    this._internalHwndSource.RootVisual = (Visual) this._internalHostPresenter;
    this.AddLogicalChild((object) this._internalHostPresenter);
    Win32Helper.BringWindowToTop(this._internalHwndSource.Handle);
    return new HandleRef((object) this, this._internalHwndSource.Handle);
  }

  protected override void DestroyWindowCore(HandleRef hwnd)
  {
    if (this._internalHwndSource == null)
      return;
    this._internalHwndSource.ContentRendered -= new EventHandler(this._internalHwndSource_ContentRendered);
    this._internalHwndSource.Dispose();
    this._internalHwndSource = (HwndSource) null;
  }

  protected override bool HasFocusWithinCore() => false;

  protected override IEnumerator LogicalChildren
  {
    get
    {
      if (this._internalHostPresenter == null)
        return new UIElement[0].GetEnumerator();
      return new UIElement[1]
      {
        (UIElement) this._internalHostPresenter
      }.GetEnumerator();
    }
  }

  protected override Size MeasureOverride(Size constraint)
  {
    if (this._internalHostPresenter == null)
      return base.MeasureOverride(constraint);
    this._internalHostPresenter.Measure(constraint);
    return this._internalHostPresenter.DesiredSize;
  }

  protected override Size ArrangeOverride(Size finalSize)
  {
    if (this._internalHostPresenter == null)
      return base.ArrangeOverride(finalSize);
    this._internalHostPresenter.Arrange(new Rect(finalSize));
    return base.ArrangeOverride(finalSize);
  }

  internal void Show(LayoutAnchorControl anchor)
  {
    if (this._model != null)
      throw new InvalidOperationException();
    this._anchor = anchor;
    this._model = anchor.Model as LayoutAnchorable;
    this._side = (anchor.Model.Parent.Parent as LayoutAnchorSide).Side;
    this._manager = this._model.Root.Manager;
    this.CreateInternalGrid();
    this._model.PropertyChanged += new PropertyChangedEventHandler(this._model_PropertyChanged);
    this.Visibility = Visibility.Visible;
    this.InvalidateMeasure();
    this.UpdateWindowPos();
    Win32Helper.BringWindowToTop(this._internalHwndSource.Handle);
  }

  internal void Hide()
  {
    if (this._model == null)
      return;
    this._model.PropertyChanged -= new PropertyChangedEventHandler(this._model_PropertyChanged);
    this.RemoveInternalGrid();
    this._anchor = (LayoutAnchorControl) null;
    this._model = (LayoutAnchorable) null;
    this._manager = (DockingManager) null;
    this.Visibility = Visibility.Hidden;
  }

  internal bool IsWin32MouseOver
  {
    get
    {
      Win32Helper.Win32Point pt = new Win32Helper.Win32Point();
      if (!Win32Helper.GetCursorPos(ref pt))
        return false;
      this.PointToScreenDPI(new Point());
      if (this.GetScreenArea().Contains(new Point((double) pt.X, (double) pt.Y)))
        return true;
      LayoutAnchorControl visual = this.Model.Root.Manager.FindVisualChildren<LayoutAnchorControl>().Where<LayoutAnchorControl>((Func<LayoutAnchorControl, bool>) (c => c.Model == this.Model)).FirstOrDefault<LayoutAnchorControl>();
      if (visual == null)
        return false;
      visual.PointToScreenDPI(new Point());
      return visual.IsMouseOver;
    }
  }

  private void _internalHwndSource_ContentRendered(object sender, EventArgs e)
  {
    this._internalHost_ContentRendered = true;
  }

  private void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    if (!(e.PropertyName == "IsAutoHidden") || this._model.IsAutoHidden)
      return;
    this._manager.HideAutoHideWindow(this._anchor);
  }

  private void CreateInternalGrid()
  {
    Grid grid = new Grid();
    grid.FlowDirection = FlowDirection.LeftToRight;
    this._internalGrid = grid;
    this._internalGrid.SetBinding(Panel.BackgroundProperty, (BindingBase) new Binding("Background")
    {
      Source = (object) this
    });
    LayoutAnchorableControl anchorableControl = new LayoutAnchorableControl();
    anchorableControl.Model = this._model;
    anchorableControl.Style = this.AnchorableStyle;
    this._internalHost = anchorableControl;
    this._internalHost.SetBinding(FrameworkElement.FlowDirectionProperty, (BindingBase) new Binding("Model.Root.Manager.FlowDirection")
    {
      Source = (object) this
    });
    KeyboardNavigation.SetTabNavigation((DependencyObject) this._internalGrid, KeyboardNavigationMode.Cycle);
    this._resizer = new LayoutGridResizerControl();
    this._resizer.DragStarted += new DragStartedEventHandler(this.OnResizerDragStarted);
    this._resizer.DragDelta += new DragDeltaEventHandler(this.OnResizerDragDelta);
    this._resizer.DragCompleted += new DragCompletedEventHandler(this.OnResizerDragCompleted);
    if (this._side == AnchorSide.Right)
    {
      this._internalGrid.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(this._manager.GridSplitterWidth)
      });
      this._internalGrid.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = this._model.AutoHideWidth == 0.0 ? new GridLength(this._model.AutoHideMinWidth) : new GridLength(this._model.AutoHideWidth, GridUnitType.Pixel)
      });
      Grid.SetColumn((UIElement) this._resizer, 0);
      Grid.SetColumn((UIElement) this._internalHost, 1);
      this._resizer.Cursor = Cursors.SizeWE;
      this.HorizontalAlignment = HorizontalAlignment.Right;
      this.VerticalAlignment = VerticalAlignment.Stretch;
    }
    else if (this._side == AnchorSide.Left)
    {
      this._internalGrid.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = this._model.AutoHideWidth == 0.0 ? new GridLength(this._model.AutoHideMinWidth) : new GridLength(this._model.AutoHideWidth, GridUnitType.Pixel)
      });
      this._internalGrid.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(this._manager.GridSplitterWidth)
      });
      Grid.SetColumn((UIElement) this._internalHost, 0);
      Grid.SetColumn((UIElement) this._resizer, 1);
      this._resizer.Cursor = Cursors.SizeWE;
      this.HorizontalAlignment = HorizontalAlignment.Left;
      this.VerticalAlignment = VerticalAlignment.Stretch;
    }
    else if (this._side == AnchorSide.Top)
    {
      this._internalGrid.RowDefinitions.Add(new RowDefinition()
      {
        Height = this._model.AutoHideHeight == 0.0 ? new GridLength(this._model.AutoHideMinHeight) : new GridLength(this._model.AutoHideHeight, GridUnitType.Pixel)
      });
      this._internalGrid.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(this._manager.GridSplitterHeight)
      });
      Grid.SetRow((UIElement) this._internalHost, 0);
      Grid.SetRow((UIElement) this._resizer, 1);
      this._resizer.Cursor = Cursors.SizeNS;
      this.VerticalAlignment = VerticalAlignment.Top;
      this.HorizontalAlignment = HorizontalAlignment.Stretch;
    }
    else if (this._side == AnchorSide.Bottom)
    {
      this._internalGrid.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(this._manager.GridSplitterHeight)
      });
      this._internalGrid.RowDefinitions.Add(new RowDefinition()
      {
        Height = this._model.AutoHideHeight == 0.0 ? new GridLength(this._model.AutoHideMinHeight) : new GridLength(this._model.AutoHideHeight, GridUnitType.Pixel)
      });
      Grid.SetRow((UIElement) this._resizer, 0);
      Grid.SetRow((UIElement) this._internalHost, 1);
      this._resizer.Cursor = Cursors.SizeNS;
      this.VerticalAlignment = VerticalAlignment.Bottom;
      this.HorizontalAlignment = HorizontalAlignment.Stretch;
    }
    this._internalGrid.Children.Add((UIElement) this._resizer);
    this._internalGrid.Children.Add((UIElement) this._internalHost);
    this._internalHostPresenter.Content = (object) this._internalGrid;
  }

  private void RemoveInternalGrid()
  {
    this._resizer.DragStarted -= new DragStartedEventHandler(this.OnResizerDragStarted);
    this._resizer.DragDelta -= new DragDeltaEventHandler(this.OnResizerDragDelta);
    this._resizer.DragCompleted -= new DragCompletedEventHandler(this.OnResizerDragCompleted);
    this._internalHostPresenter.Content = (object) null;
  }

  private void ShowResizerOverlayWindow(LayoutGridResizerControl splitter)
  {
    Border border = new Border();
    border.Background = splitter.BackgroundWhileDragging;
    border.Opacity = splitter.OpacityWhileDragging;
    this._resizerGhost = border;
    FrameworkElement autoHideAreaElement = this._manager.GetAutoHideAreaElement();
    this._internalHost.TransformActualSizeToAncestor();
    Point withoutFlowDirection = autoHideAreaElement.PointToScreenDPIWithoutFlowDirection(new Point());
    Size ancestor = autoHideAreaElement.TransformActualSizeToAncestor();
    Size size;
    if (this._side == AnchorSide.Right || this._side == AnchorSide.Left)
    {
      size = new Size(ancestor.Width - 25.0 + splitter.ActualWidth, ancestor.Height);
      this._resizerGhost.Width = splitter.ActualWidth;
      this._resizerGhost.Height = size.Height;
      withoutFlowDirection.Offset(25.0, 0.0);
    }
    else
    {
      size = new Size(ancestor.Width, ancestor.Height - this._model.AutoHideMinHeight - 25.0 + splitter.ActualHeight);
      this._resizerGhost.Height = splitter.ActualHeight;
      this._resizerGhost.Width = size.Width;
      withoutFlowDirection.Offset(0.0, 25.0);
    }
    this._initialStartPoint = splitter.PointToScreenDPIWithoutFlowDirection(new Point()) - withoutFlowDirection;
    if (this._side == AnchorSide.Right || this._side == AnchorSide.Left)
      Canvas.SetLeft((UIElement) this._resizerGhost, this._initialStartPoint.X);
    else
      Canvas.SetTop((UIElement) this._resizerGhost, this._initialStartPoint.Y);
    Canvas canvas1 = new Canvas();
    canvas1.HorizontalAlignment = HorizontalAlignment.Stretch;
    canvas1.VerticalAlignment = VerticalAlignment.Stretch;
    Canvas canvas2 = canvas1;
    canvas2.Children.Add((UIElement) this._resizerGhost);
    Window window = new Window();
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
    window.Owner = Window.GetWindow((DependencyObject) this);
    window.Content = (object) canvas2;
    this._resizerWindowHost = window;
    this._resizerWindowHost.Show();
  }

  private void HideResizerOverlayWindow()
  {
    if (this._resizerWindowHost == null)
      return;
    this._resizerWindowHost.Close();
    this._resizerWindowHost = (Window) null;
  }

  private void OnResizerDragCompleted(object sender, DragCompletedEventArgs e)
  {
    GeneralTransform ancestor = this.TransformToAncestor(this.FindVisualTreeRoot() as Visual);
    Vector vector = ancestor.Transform(new Point(e.HorizontalChange, e.VerticalChange)) - ancestor.Transform(new Point());
    double num = this._side == AnchorSide.Right || this._side == AnchorSide.Left ? Canvas.GetLeft((UIElement) this._resizerGhost) - this._initialStartPoint.X : Canvas.GetTop((UIElement) this._resizerGhost) - this._initialStartPoint.Y;
    if (this._side == AnchorSide.Right)
    {
      if (this._model.AutoHideWidth == 0.0)
        this._model.AutoHideWidth = this._internalHost.ActualWidth - num;
      else
        this._model.AutoHideWidth -= num;
      this._internalGrid.ColumnDefinitions[1].Width = new GridLength(this._model.AutoHideWidth, GridUnitType.Pixel);
    }
    else if (this._side == AnchorSide.Left)
    {
      if (this._model.AutoHideWidth == 0.0)
        this._model.AutoHideWidth = this._internalHost.ActualWidth + num;
      else
        this._model.AutoHideWidth += num;
      this._internalGrid.ColumnDefinitions[0].Width = new GridLength(this._model.AutoHideWidth, GridUnitType.Pixel);
    }
    else if (this._side == AnchorSide.Top)
    {
      if (this._model.AutoHideHeight == 0.0)
        this._model.AutoHideHeight = this._internalHost.ActualHeight + num;
      else
        this._model.AutoHideHeight += num;
      this._internalGrid.RowDefinitions[0].Height = new GridLength(this._model.AutoHideHeight, GridUnitType.Pixel);
    }
    else if (this._side == AnchorSide.Bottom)
    {
      if (this._model.AutoHideHeight == 0.0)
        this._model.AutoHideHeight = this._internalHost.ActualHeight - num;
      else
        this._model.AutoHideHeight -= num;
      this._internalGrid.RowDefinitions[1].Height = new GridLength(this._model.AutoHideHeight, GridUnitType.Pixel);
    }
    this.HideResizerOverlayWindow();
    this.IsResizing = false;
    this.InvalidateMeasure();
  }

  private void OnResizerDragDelta(object sender, DragDeltaEventArgs e)
  {
    GeneralTransform ancestor = this.TransformToAncestor(this.FindVisualTreeRoot() as Visual);
    Vector vector = ancestor.Transform(new Point(e.HorizontalChange, e.VerticalChange)) - ancestor.Transform(new Point());
    if (this._side == AnchorSide.Right || this._side == AnchorSide.Left)
    {
      if (FrameworkElement.GetFlowDirection((DependencyObject) this._internalHost) == FlowDirection.RightToLeft)
        vector.X = -vector.X;
      Canvas.SetLeft((UIElement) this._resizerGhost, MathHelper.MinMax(this._initialStartPoint.X + vector.X, 0.0, this._resizerWindowHost.Width - this._resizerGhost.Width));
    }
    else
      Canvas.SetTop((UIElement) this._resizerGhost, MathHelper.MinMax(this._initialStartPoint.Y + vector.Y, 0.0, this._resizerWindowHost.Height - this._resizerGhost.Height));
  }

  private void OnResizerDragStarted(object sender, DragStartedEventArgs e)
  {
    this.ShowResizerOverlayWindow(sender as LayoutGridResizerControl);
    this.IsResizing = true;
  }
}
