// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.ChildWindow
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_WindowRoot", Type = typeof (Grid))]
[TemplatePart(Name = "PART_Root", Type = typeof (Grid))]
[TemplatePart(Name = "PART_WindowControl", Type = typeof (WindowControl))]
public class ChildWindow : WindowControl
{
  private const string PART_WindowRoot = "PART_WindowRoot";
  private const string PART_Root = "PART_Root";
  private const string PART_WindowControl = "PART_WindowControl";
  private const int _horizontalOffset = 3;
  private const int _verticalOffset = 3;
  private Grid _root;
  private TranslateTransform _moveTransform = new TranslateTransform();
  private bool _startupPositionInitialized;
  private FrameworkElement _parentContainer;
  private Rectangle _modalLayer = new Rectangle();
  private Canvas _modalLayerPanel = new Canvas();
  private Grid _windowRoot;
  private WindowControl _windowControl;
  private bool _ignorePropertyChanged;
  private bool _hasChildren;
  private bool _hasWindowContainer;
  private bool? _dialogResult;
  public static readonly DependencyProperty DesignerWindowStateProperty = DependencyProperty.Register(nameof (DesignerWindowState), typeof (WindowState), typeof (ChildWindow), new PropertyMetadata((object) WindowState.Closed, new PropertyChangedCallback(ChildWindow.OnDesignerWindowStatePropertyChanged)));
  public static readonly DependencyProperty FocusedElementProperty = DependencyProperty.Register(nameof (FocusedElement), typeof (FrameworkElement), typeof (ChildWindow), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty IsModalProperty = DependencyProperty.Register(nameof (IsModal), typeof (bool), typeof (ChildWindow), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(ChildWindow.OnIsModalPropertyChanged)));
  [Obsolete("This property is obsolete and should no longer be used. Use WindowContainer.ModalBackgroundBrushProperty instead.")]
  public static readonly DependencyProperty OverlayBrushProperty = DependencyProperty.Register(nameof (OverlayBrush), typeof (Brush), typeof (ChildWindow), new PropertyMetadata((object) Brushes.Gray, new PropertyChangedCallback(ChildWindow.OnOverlayBrushChanged)));
  [Obsolete("This property is obsolete and should no longer be used. Use WindowContainer.ModalBackgroundBrushProperty instead.")]
  public static readonly DependencyProperty OverlayOpacityProperty = DependencyProperty.Register(nameof (OverlayOpacity), typeof (double), typeof (ChildWindow), new PropertyMetadata((object) 0.5, new PropertyChangedCallback(ChildWindow.OnOverlayOpacityChanged)));
  public static readonly DependencyProperty WindowStartupLocationProperty = DependencyProperty.Register(nameof (WindowStartupLocation), typeof (WindowStartupLocation), typeof (ChildWindow), (PropertyMetadata) new UIPropertyMetadata((object) WindowStartupLocation.Manual, new PropertyChangedCallback(ChildWindow.OnWindowStartupLocationChanged)));
  public static readonly DependencyProperty WindowStateProperty = DependencyProperty.Register(nameof (WindowState), typeof (WindowState), typeof (ChildWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) WindowState.Closed, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ChildWindow.OnWindowStatePropertyChanged)));

  [TypeConverter(typeof (NullableBoolConverter))]
  public bool? DialogResult
  {
    get => this._dialogResult;
    set
    {
      bool? dialogResult = this._dialogResult;
      bool? nullable = value;
      if (dialogResult.GetValueOrDefault() == nullable.GetValueOrDefault() & dialogResult.HasValue == nullable.HasValue)
        return;
      this._dialogResult = value;
      this.Close();
    }
  }

  public WindowState DesignerWindowState
  {
    get => (WindowState) this.GetValue(ChildWindow.DesignerWindowStateProperty);
    set => this.SetValue(ChildWindow.DesignerWindowStateProperty, (object) value);
  }

  private static void OnDesignerWindowStatePropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(d is ChildWindow childWindow))
      return;
    childWindow.OnDesignerWindowStatePropertyChanged((WindowState) e.OldValue, (WindowState) e.NewValue);
  }

  protected virtual void OnDesignerWindowStatePropertyChanged(
    WindowState oldValue,
    WindowState newValue)
  {
    if (!DesignerProperties.GetIsInDesignMode((DependencyObject) this))
      return;
    this.Visibility = newValue == WindowState.Open ? Visibility.Visible : Visibility.Collapsed;
  }

  public FrameworkElement FocusedElement
  {
    get => (FrameworkElement) this.GetValue(ChildWindow.FocusedElementProperty);
    set => this.SetValue(ChildWindow.FocusedElementProperty, (object) value);
  }

  public bool IsModal
  {
    get => (bool) this.GetValue(ChildWindow.IsModalProperty);
    set => this.SetValue(ChildWindow.IsModalProperty, (object) value);
  }

  private static void OnIsModalPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(d is ChildWindow childWindow))
      return;
    childWindow.OnIsModalChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  internal event EventHandler<EventArgs> IsModalChanged;

  private void OnIsModalChanged(bool oldValue, bool newValue)
  {
    EventHandler<EventArgs> isModalChanged = this.IsModalChanged;
    if (isModalChanged != null)
      isModalChanged((object) this, EventArgs.Empty);
    if (this._hasWindowContainer)
      return;
    if (newValue)
    {
      KeyboardNavigation.SetTabNavigation((DependencyObject) this, KeyboardNavigationMode.Cycle);
      this.ShowModalLayer();
    }
    else
    {
      KeyboardNavigation.SetTabNavigation((DependencyObject) this, KeyboardNavigationMode.Continue);
      this.HideModalLayer();
    }
  }

  [Obsolete("This property is obsolete and should no longer be used. Use WindowContainer.ModalBackgroundBrushProperty instead.")]
  public Brush OverlayBrush
  {
    get => (Brush) this.GetValue(ChildWindow.OverlayBrushProperty);
    set => this.SetValue(ChildWindow.OverlayBrushProperty, (object) value);
  }

  private static void OnOverlayBrushChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(d is ChildWindow childWindow))
      return;
    childWindow.OnOverlayBrushChanged((Brush) e.OldValue, (Brush) e.NewValue);
  }

  [Obsolete("This method is obsolete and should no longer be used. Use WindowContainer.ModalBackgroundBrushProperty instead.")]
  protected virtual void OnOverlayBrushChanged(Brush oldValue, Brush newValue)
  {
    this._modalLayer.Fill = newValue;
  }

  [Obsolete("This property is obsolete and should no longer be used. Use WindowContainer.ModalBackgroundBrushProperty instead.")]
  public double OverlayOpacity
  {
    get => (double) this.GetValue(ChildWindow.OverlayOpacityProperty);
    set => this.SetValue(ChildWindow.OverlayOpacityProperty, (object) value);
  }

  private static void OnOverlayOpacityChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(d is ChildWindow childWindow))
      return;
    childWindow.OnOverlayOpacityChanged((double) e.OldValue, (double) e.NewValue);
  }

  [Obsolete("This method is obsolete and should no longer be used. Use WindowContainer.ModalBackgroundBrushProperty instead.")]
  protected virtual void OnOverlayOpacityChanged(double oldValue, double newValue)
  {
    this._modalLayer.Opacity = newValue;
  }

  public WindowStartupLocation WindowStartupLocation
  {
    get => (WindowStartupLocation) this.GetValue(ChildWindow.WindowStartupLocationProperty);
    set => this.SetValue(ChildWindow.WindowStartupLocationProperty, (object) value);
  }

  private static void OnWindowStartupLocationChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ChildWindow childWindow))
      return;
    childWindow.OnWindowStartupLocationChanged((WindowStartupLocation) e.OldValue, (WindowStartupLocation) e.NewValue);
  }

  protected virtual void OnWindowStartupLocationChanged(
    WindowStartupLocation oldValue,
    WindowStartupLocation newValue)
  {
  }

  public WindowState WindowState
  {
    get => (WindowState) this.GetValue(ChildWindow.WindowStateProperty);
    set => this.SetValue(ChildWindow.WindowStateProperty, (object) value);
  }

  private static void OnWindowStatePropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(d is ChildWindow childWindow))
      return;
    childWindow.OnWindowStatePropertyChanged((WindowState) e.OldValue, (WindowState) e.NewValue);
  }

  protected virtual void OnWindowStatePropertyChanged(WindowState oldValue, WindowState newValue)
  {
    if (!DesignerProperties.GetIsInDesignMode((DependencyObject) this))
    {
      if (this._ignorePropertyChanged)
        return;
      this.SetWindowState(newValue);
    }
    else
      this.Visibility = this.DesignerWindowState == WindowState.Open ? Visibility.Visible : Visibility.Collapsed;
  }

  static ChildWindow()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (ChildWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (ChildWindow)));
  }

  public ChildWindow()
  {
    this.DesignerWindowState = WindowState.Open;
    this._modalLayer.Fill = this.OverlayBrush;
    this._modalLayer.Opacity = this.OverlayOpacity;
    this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.ChildWindow_IsVisibleChanged);
  }

  internal override bool AllowPublicIsActiveChange => false;

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (this._windowControl != null)
    {
      this._windowControl.HeaderDragDelta -= (DragDeltaEventHandler) ((o, e) => this.OnHeaderDragDelta(e));
      this._windowControl.HeaderIconDoubleClicked -= (MouseButtonEventHandler) ((o, e) => this.OnHeaderIconDoubleClick(e));
      this._windowControl.CloseButtonClicked -= (RoutedEventHandler) ((o, e) => this.OnCloseButtonClicked(e));
    }
    this._windowControl = this.GetTemplateChild("PART_WindowControl") as WindowControl;
    if (this._windowControl != null)
    {
      this._windowControl.HeaderDragDelta += (DragDeltaEventHandler) ((o, e) => this.OnHeaderDragDelta(e));
      this._windowControl.HeaderIconDoubleClicked += (MouseButtonEventHandler) ((o, e) => this.OnHeaderIconDoubleClick(e));
      this._windowControl.CloseButtonClicked += (RoutedEventHandler) ((o, e) => this.OnCloseButtonClicked(e));
    }
    this.UpdateBlockMouseInputsPanel();
    this._windowRoot = this.GetTemplateChild("PART_WindowRoot") as Grid;
    if (this._windowRoot != null)
      this._windowRoot.RenderTransform = (Transform) this._moveTransform;
    this._hasWindowContainer = VisualTreeHelper.GetParent((DependencyObject) this) is WindowContainer;
    if (this._hasWindowContainer)
      return;
    this._parentContainer = VisualTreeHelper.GetParent((DependencyObject) this) as FrameworkElement;
    if (this._parentContainer != null)
    {
      this._parentContainer.LayoutUpdated += new EventHandler(this.ParentContainer_LayoutUpdated);
      this._parentContainer.SizeChanged += new SizeChangedEventHandler(this.ParentContainer_SizeChanged);
      if (BrowserInteropHelper.IsBrowserHosted)
        this._parentContainer.Loaded += (RoutedEventHandler) ((o, e) => this.ExecuteOpen());
    }
    this.Unloaded += new RoutedEventHandler(this.ChildWindow_Unloaded);
    this._modalLayer.Height = this._parentContainer.ActualHeight;
    this._modalLayer.Width = this._parentContainer.ActualWidth;
    this._root = this.GetTemplateChild("PART_Root") as Grid;
    Style resource = this._root != null ? this._root.Resources[(object) "FocusVisualStyle"] as Style : (Style) null;
    if (resource != null)
    {
      Setter setter = new Setter(FrameworkElement.DataContextProperty, (object) this);
      resource.Setters.Add((SetterBase) setter);
      this.FocusVisualStyle = resource;
    }
    if (this._root == null)
      return;
    this._root.Children.Add((UIElement) this._modalLayerPanel);
  }

  protected override void OnGotFocus(RoutedEventArgs e)
  {
    base.OnGotFocus(e);
    this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Delegate) (() =>
    {
      if (this.FocusedElement != null)
      {
        this._hasChildren = true;
        this.FocusedElement.Focus();
      }
      else
      {
        FrameworkElement child = TreeHelper.FindChild<FrameworkElement>(this.Content as DependencyObject, (Func<FrameworkElement, bool>) (x => x.Focusable));
        if (child != null)
        {
          this._hasChildren = true;
          child.Focus();
        }
        else
          this._hasChildren = false;
      }
    }));
  }

  protected override void OnPreviewKeyDown(KeyEventArgs e)
  {
    base.OnPreviewKeyDown(e);
    if (!this.IsModal)
      return;
    if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
    {
      e.Handled = true;
    }
    else
    {
      if (e.Key != Key.Tab || this._hasChildren)
        return;
      e.Handled = true;
    }
  }

  protected override void OnKeyDown(KeyEventArgs e)
  {
    base.OnKeyDown(e);
    if (this.WindowState != WindowState.Open)
      return;
    switch (e.Key)
    {
      case Key.Left:
        this.Left -= 3.0;
        e.Handled = true;
        break;
      case Key.Up:
        this.Top -= 3.0;
        e.Handled = true;
        break;
      case Key.Right:
        this.Left += 3.0;
        e.Handled = true;
        break;
      case Key.Down:
        this.Top += 3.0;
        e.Handled = true;
        break;
    }
  }

  protected override void OnLeftPropertyChanged(double oldValue, double newValue)
  {
    base.OnLeftPropertyChanged(oldValue, newValue);
    this._hasWindowContainer = VisualTreeHelper.GetParent((DependencyObject) this) is WindowContainer;
    if (this._hasWindowContainer)
      return;
    this.Left = this.GetRestrictedLeft();
    this.ProcessMove(newValue - oldValue, 0.0);
  }

  protected override void OnTopPropertyChanged(double oldValue, double newValue)
  {
    base.OnTopPropertyChanged(oldValue, newValue);
    this._hasWindowContainer = VisualTreeHelper.GetParent((DependencyObject) this) is WindowContainer;
    if (this._hasWindowContainer)
      return;
    this.Top = this.GetRestrictedTop();
    this.ProcessMove(0.0, newValue - oldValue);
  }

  internal override void UpdateBlockMouseInputsPanel()
  {
    if (this._windowControl == null)
      return;
    this._windowControl.IsBlockMouseInputsPanelActive = this.IsBlockMouseInputsPanelActive;
  }

  protected virtual void OnHeaderDragDelta(DragDeltaEventArgs e)
  {
    if (!this.IsCurrentWindow(e.OriginalSource))
      return;
    e.Handled = true;
    DragDeltaEventArgs e1 = new DragDeltaEventArgs(e.HorizontalChange, e.VerticalChange);
    e1.RoutedEvent = WindowControl.HeaderDragDeltaEvent;
    e1.Source = (object) this;
    this.RaiseEvent((RoutedEventArgs) e1);
    if (e1.Handled || !object.Equals(e.OriginalSource, (object) this._windowControl))
      return;
    this.Left = this.FlowDirection != FlowDirection.RightToLeft ? this.Left + e.HorizontalChange : this.Left - e.HorizontalChange;
    this.Top += e.VerticalChange;
  }

  protected virtual void OnHeaderIconDoubleClick(MouseButtonEventArgs e)
  {
    if (!this.IsCurrentWindow(e.OriginalSource))
      return;
    e.Handled = true;
    MouseButtonEventArgs e1 = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
    e1.RoutedEvent = WindowControl.HeaderIconDoubleClickedEvent;
    e1.Source = (object) this;
    this.RaiseEvent((RoutedEventArgs) e1);
    if (e1.Handled)
      return;
    this.Close();
  }

  protected virtual void OnCloseButtonClicked(RoutedEventArgs e)
  {
    if (!this.IsCurrentWindow(e.OriginalSource))
      return;
    e.Handled = true;
    RoutedEventArgs e1 = new RoutedEventArgs(WindowControl.CloseButtonClickedEvent, (object) this);
    this.RaiseEvent(e1);
    if (e1.Handled)
      return;
    this.Close();
  }

  [Obsolete("This method is obsolete and should no longer be used.")]
  private void ParentContainer_LayoutUpdated(object sender, EventArgs e)
  {
    if (DesignerProperties.GetIsInDesignMode((DependencyObject) this) || this._startupPositionInitialized)
      return;
    this.ExecuteOpen();
    this._startupPositionInitialized = true;
  }

  [Obsolete("This method is obsolete and should no longer be used.")]
  private void ChildWindow_Unloaded(object sender, RoutedEventArgs e)
  {
    if (this._parentContainer == null)
      return;
    this._parentContainer.LayoutUpdated -= new EventHandler(this.ParentContainer_LayoutUpdated);
    this._parentContainer.SizeChanged -= new SizeChangedEventHandler(this.ParentContainer_SizeChanged);
    if (!BrowserInteropHelper.IsBrowserHosted)
      return;
    this._parentContainer.Loaded -= (RoutedEventHandler) ((o, ev) => this.ExecuteOpen());
  }

  [Obsolete("This method is obsolete and should no longer be used.")]
  private void ParentContainer_SizeChanged(object sender, SizeChangedEventArgs e)
  {
    this._modalLayer.Height = e.NewSize.Height;
    this._modalLayer.Width = e.NewSize.Width;
    this.Left = this.GetRestrictedLeft();
    this.Top = this.GetRestrictedTop();
  }

  private void ChildWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
  {
    if (!(bool) e.NewValue || !this.IsModal)
      return;
    this.Focus();
  }

  [Obsolete("This method is obsolete and should no longer be used. Use WindowContainer.GetRestrictedLeft() instead.")]
  private double GetRestrictedLeft()
  {
    if (this.Left < 0.0)
      return 0.0;
    if (this._parentContainer == null || this._windowRoot == null || this.Left + this._windowRoot.ActualWidth <= this._parentContainer.ActualWidth || this._parentContainer.ActualWidth == 0.0)
      return this.Left;
    double num = this._parentContainer.ActualWidth - this._windowRoot.ActualWidth;
    return num >= 0.0 ? num : 0.0;
  }

  [Obsolete("This method is obsolete and should no longer be used. Use WindowContainer.GetRestrictedTop() instead.")]
  private double GetRestrictedTop()
  {
    if (this.Top < 0.0)
      return 0.0;
    if (this._parentContainer == null || this._windowRoot == null || this.Top + this._windowRoot.ActualHeight <= this._parentContainer.ActualHeight || this._parentContainer.ActualHeight == 0.0)
      return this.Top;
    double num = this._parentContainer.ActualHeight - this._windowRoot.ActualHeight;
    return num >= 0.0 ? num : 0.0;
  }

  private void SetWindowState(WindowState state)
  {
    if (state != WindowState.Closed)
    {
      if (state != WindowState.Open)
        return;
      this.ExecuteOpen();
    }
    else
      this.ExecuteClose();
  }

  private void ExecuteClose()
  {
    CancelEventArgs e = new CancelEventArgs();
    this.OnClosing(e);
    if (!e.Cancel)
    {
      if (!this._dialogResult.HasValue)
        this._dialogResult = new bool?(false);
      this.OnClosed(EventArgs.Empty);
    }
    else
      this.CancelClose();
  }

  private void CancelClose()
  {
    this._dialogResult = new bool?();
    this._ignorePropertyChanged = true;
    this.WindowState = WindowState.Open;
    this._ignorePropertyChanged = false;
  }

  private void ExecuteOpen()
  {
    this._dialogResult = new bool?();
    if (!this._hasWindowContainer && this.WindowStartupLocation == WindowStartupLocation.Center)
      this.CenterChildWindow();
    if (this._hasWindowContainer)
      return;
    this.BringToFront();
  }

  private bool IsCurrentWindow(object windowtoTest)
  {
    return object.Equals((object) this._windowControl, windowtoTest);
  }

  [Obsolete("This method is obsolete and should no longer be used. Use WindowContainer.BringToFront() instead.")]
  private void BringToFront()
  {
    int num1 = 0;
    if (this._parentContainer != null)
      num1 = (int) this._parentContainer.GetValue(Panel.ZIndexProperty);
    int num2;
    this.SetValue(Panel.ZIndexProperty, (object) (num2 = num1 + 1));
    if (!this.IsModal)
      return;
    Panel.SetZIndex((UIElement) this._modalLayerPanel, num2 - 2);
  }

  [Obsolete("This method is obsolete and should no longer be used. Use WindowContainer.CenterChild() instead.")]
  private void CenterChildWindow()
  {
    if (this._parentContainer == null || this._windowRoot == null)
      return;
    this._windowRoot.UpdateLayout();
    this.Left = (this._parentContainer.ActualWidth - this._windowRoot.ActualWidth) / 2.0;
    this.Top = (this._parentContainer.ActualHeight - this._windowRoot.ActualHeight) / 2.0;
  }

  [Obsolete("This method is obsolete and should no longer be used.")]
  private void ShowModalLayer()
  {
    if (DesignerProperties.GetIsInDesignMode((DependencyObject) this))
      return;
    if (!this._modalLayerPanel.Children.Contains((UIElement) this._modalLayer))
      this._modalLayerPanel.Children.Add((UIElement) this._modalLayer);
    this._modalLayer.Visibility = Visibility.Visible;
  }

  [Obsolete("This method is obsolete and should no longer be used.")]
  private void HideModalLayer() => this._modalLayer.Visibility = Visibility.Collapsed;

  [Obsolete("This method is obsolete and should no longer be used. Use the ChildWindow in a WindowContainer instead.")]
  private void ProcessMove(double x, double y)
  {
    this._moveTransform.X += x;
    this._moveTransform.Y += y;
    this.InvalidateArrange();
  }

  public void Show() => this.WindowState = WindowState.Open;

  public void Close() => this.WindowState = WindowState.Closed;

  public event EventHandler Closed;

  protected virtual void OnClosed(EventArgs e)
  {
    if (this.Closed == null)
      return;
    this.Closed((object) this, e);
  }

  public event EventHandler<CancelEventArgs> Closing;

  protected virtual void OnClosing(CancelEventArgs e)
  {
    if (this.Closing == null)
      return;
    this.Closing((object) this, e);
  }
}
