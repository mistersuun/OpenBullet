// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Primitives.WindowControl
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit.Primitives;

[TemplatePart(Name = "PART_HeaderThumb", Type = typeof (Thumb))]
[TemplatePart(Name = "PART_Icon", Type = typeof (Image))]
[TemplatePart(Name = "PART_CloseButton", Type = typeof (Button))]
[TemplatePart(Name = "PART_ToolWindowCloseButton", Type = typeof (Button))]
[TemplatePart(Name = "PART_BlockMouseInputsBorder", Type = typeof (Border))]
[TemplatePart(Name = "PART_HeaderGrid", Type = typeof (Grid))]
public class WindowControl : ContentControl
{
  public static readonly ComponentResourceKey DefaultCloseButtonStyleKey = new ComponentResourceKey(typeof (WindowControl), (object) "DefaultCloseButtonStyle");
  private const string PART_HeaderThumb = "PART_HeaderThumb";
  private const string PART_Icon = "PART_Icon";
  private const string PART_CloseButton = "PART_CloseButton";
  private const string PART_ToolWindowCloseButton = "PART_ToolWindowCloseButton";
  private const string PART_BlockMouseInputsBorder = "PART_BlockMouseInputsBorder";
  private const string PART_HeaderGrid = "PART_HeaderGrid";
  private Thumb _headerThumb;
  private Image _icon;
  private Button _closeButton;
  private Button _windowToolboxCloseButton;
  private bool _setIsActiveInternal;
  internal Border _windowBlockMouseInputsPanel;
  public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(nameof (Caption), typeof (string), typeof (WindowControl), (PropertyMetadata) new UIPropertyMetadata((object) string.Empty));
  public static readonly DependencyProperty CaptionFontSizeProperty = DependencyProperty.Register(nameof (CaptionFontSize), typeof (double), typeof (WindowControl), (PropertyMetadata) new UIPropertyMetadata((object) 15.0));
  public static readonly DependencyProperty CaptionForegroundProperty = DependencyProperty.Register(nameof (CaptionForeground), typeof (Brush), typeof (WindowControl), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty CaptionShadowBrushProperty = DependencyProperty.Register(nameof (CaptionShadowBrush), typeof (Brush), typeof (WindowControl), (PropertyMetadata) new UIPropertyMetadata((object) new SolidColorBrush(Color.FromArgb((byte) 179, byte.MaxValue, byte.MaxValue, byte.MaxValue))));
  public static readonly DependencyProperty CaptionIconProperty = DependencyProperty.Register(nameof (CaptionIcon), typeof (ImageSource), typeof (WindowControl), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty CloseButtonStyleProperty = DependencyProperty.Register(nameof (CloseButtonStyle), typeof (Style), typeof (WindowControl), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty CloseButtonVisibilityProperty = DependencyProperty.Register(nameof (CloseButtonVisibility), typeof (Visibility), typeof (WindowControl), new PropertyMetadata((object) Visibility.Visible, (PropertyChangedCallback) null, new CoerceValueCallback(WindowControl.OnCoerceCloseButtonVisibility)));
  public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof (IsActive), typeof (bool), typeof (WindowControl), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(WindowControl.OnIsActiveChanged), new CoerceValueCallback(WindowControl.OnCoerceIsActive)));
  public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(nameof (Left), typeof (double), typeof (WindowControl), new PropertyMetadata((object) 0.0, new PropertyChangedCallback(WindowControl.OnLeftPropertyChanged), new CoerceValueCallback(WindowControl.OnCoerceLeft)));
  public static readonly DependencyProperty TopProperty = DependencyProperty.Register(nameof (Top), typeof (double), typeof (WindowControl), new PropertyMetadata((object) 0.0, new PropertyChangedCallback(WindowControl.OnTopPropertyChanged), new CoerceValueCallback(WindowControl.OnCoerceTop)));
  public static readonly DependencyProperty WindowBackgroundProperty = DependencyProperty.Register(nameof (WindowBackground), typeof (Brush), typeof (WindowControl), new PropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty WindowBorderBrushProperty = DependencyProperty.Register(nameof (WindowBorderBrush), typeof (Brush), typeof (WindowControl), new PropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty WindowBorderThicknessProperty = DependencyProperty.Register(nameof (WindowBorderThickness), typeof (Thickness), typeof (WindowControl), new PropertyMetadata((object) new Thickness(0.0)));
  public static readonly DependencyProperty WindowInactiveBackgroundProperty = DependencyProperty.Register(nameof (WindowInactiveBackground), typeof (Brush), typeof (WindowControl), new PropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty WindowOpacityProperty = DependencyProperty.Register(nameof (WindowOpacity), typeof (double), typeof (WindowControl), new PropertyMetadata((object) 1.0));
  public static readonly DependencyProperty WindowStyleProperty = DependencyProperty.Register(nameof (WindowStyle), typeof (WindowStyle), typeof (WindowControl), new PropertyMetadata((object) WindowStyle.SingleBorderWindow, (PropertyChangedCallback) null, new CoerceValueCallback(WindowControl.OnCoerceWindowStyle)));
  public static readonly DependencyProperty WindowThicknessProperty = DependencyProperty.Register(nameof (WindowThickness), typeof (Thickness), typeof (WindowControl), new PropertyMetadata((object) new Thickness(SystemParameters.ResizeFrameVerticalBorderWidth - 3.0, SystemParameters.ResizeFrameHorizontalBorderHeight - 3.0, SystemParameters.ResizeFrameVerticalBorderWidth - 3.0, SystemParameters.ResizeFrameHorizontalBorderHeight - 3.0)));
  private bool _IsBlockMouseInputsPanelActive;
  public static readonly RoutedEvent ActivatedEvent = EventManager.RegisterRoutedEvent("Activated", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (WindowControl));
  public static readonly RoutedEvent HeaderMouseLeftButtonClickedEvent = EventManager.RegisterRoutedEvent("HeaderMouseLeftButtonClicked", RoutingStrategy.Bubble, typeof (MouseButtonEventHandler), typeof (WindowControl));
  public static readonly RoutedEvent HeaderMouseRightButtonClickedEvent = EventManager.RegisterRoutedEvent("HeaderMouseRightButtonClicked", RoutingStrategy.Bubble, typeof (MouseButtonEventHandler), typeof (WindowControl));
  public static readonly RoutedEvent HeaderMouseLeftButtonDoubleClickedEvent = EventManager.RegisterRoutedEvent("HeaderMouseLeftButtonDoubleClicked", RoutingStrategy.Bubble, typeof (MouseButtonEventHandler), typeof (WindowControl));
  public static readonly RoutedEvent HeaderDragDeltaEvent = EventManager.RegisterRoutedEvent("HeaderDragDelta", RoutingStrategy.Bubble, typeof (DragDeltaEventHandler), typeof (WindowControl));
  public static readonly RoutedEvent HeaderIconClickedEvent = EventManager.RegisterRoutedEvent("HeaderIconClicked", RoutingStrategy.Bubble, typeof (MouseButtonEventHandler), typeof (WindowControl));
  public static readonly RoutedEvent HeaderIconDoubleClickedEvent = EventManager.RegisterRoutedEvent("HeaderIconDoubleClicked", RoutingStrategy.Bubble, typeof (MouseButtonEventHandler), typeof (WindowControl));
  public static readonly RoutedEvent CloseButtonClickedEvent = EventManager.RegisterRoutedEvent("CloseButtonClicked", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (WindowControl));

  static WindowControl()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (WindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (WindowControl)));
  }

  public string Caption
  {
    get => (string) this.GetValue(WindowControl.CaptionProperty);
    set => this.SetValue(WindowControl.CaptionProperty, (object) value);
  }

  public double CaptionFontSize
  {
    get => (double) this.GetValue(WindowControl.CaptionFontSizeProperty);
    set => this.SetValue(WindowControl.CaptionFontSizeProperty, (object) value);
  }

  public Brush CaptionForeground
  {
    get => (Brush) this.GetValue(WindowControl.CaptionForegroundProperty);
    set => this.SetValue(WindowControl.CaptionForegroundProperty, (object) value);
  }

  public Brush CaptionShadowBrush
  {
    get => (Brush) this.GetValue(WindowControl.CaptionShadowBrushProperty);
    set => this.SetValue(WindowControl.CaptionShadowBrushProperty, (object) value);
  }

  public ImageSource CaptionIcon
  {
    get => (ImageSource) this.GetValue(WindowControl.CaptionIconProperty);
    set => this.SetValue(WindowControl.CaptionIconProperty, (object) value);
  }

  public Style CloseButtonStyle
  {
    get => (Style) this.GetValue(WindowControl.CloseButtonStyleProperty);
    set => this.SetValue(WindowControl.CloseButtonStyleProperty, (object) value);
  }

  public Visibility CloseButtonVisibility
  {
    get => (Visibility) this.GetValue(WindowControl.CloseButtonVisibilityProperty);
    set => this.SetValue(WindowControl.CloseButtonVisibilityProperty, (object) value);
  }

  private static object OnCoerceCloseButtonVisibility(DependencyObject d, object basevalue)
  {
    return basevalue == DependencyProperty.UnsetValue || !(d is WindowControl windowControl) ? basevalue : windowControl.OnCoerceCloseButtonVisibility((Visibility) basevalue);
  }

  protected virtual object OnCoerceCloseButtonVisibility(Visibility newValue) => (object) newValue;

  public bool IsActive
  {
    get => (bool) this.GetValue(WindowControl.IsActiveProperty);
    set => this.SetValue(WindowControl.IsActiveProperty, (object) value);
  }

  private static object OnCoerceIsActive(DependencyObject d, object basevalue)
  {
    if (d is WindowControl windowControl && !windowControl._setIsActiveInternal && !windowControl.AllowPublicIsActiveChange)
      throw new InvalidOperationException("Cannot set IsActive directly. This is handled by the underlying system");
    return basevalue;
  }

  private static void OnIsActiveChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
  {
    if (!(obj is WindowControl windowControl))
      return;
    windowControl.OnIsActiveChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsActiveChanged(bool oldValue, bool newValue)
  {
    if (!newValue || !(this.GetType() == typeof (WindowControl)))
      return;
    this.RaiseEvent(new RoutedEventArgs(WindowControl.ActivatedEvent, (object) this));
  }

  internal void SetIsActiveInternal(bool isActive)
  {
    this._setIsActiveInternal = true;
    this.IsActive = isActive;
    this._setIsActiveInternal = false;
  }

  public double Left
  {
    get => (double) this.GetValue(WindowControl.LeftProperty);
    set => this.SetValue(WindowControl.LeftProperty, (object) value);
  }

  private static object OnCoerceLeft(DependencyObject d, object basevalue)
  {
    if (basevalue == DependencyProperty.UnsetValue)
      return basevalue;
    WindowControl windowControl = (WindowControl) d;
    return windowControl == null ? basevalue : windowControl.OnCoerceLeft(basevalue);
  }

  private object OnCoerceLeft(object newValue)
  {
    return object.Equals((object) (double) newValue, (object) double.NaN) ? (object) 0.0 : newValue;
  }

  private static void OnLeftPropertyChanged(
    DependencyObject obj,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(obj is WindowControl windowControl))
      return;
    windowControl.OnLeftPropertyChanged((double) e.OldValue, (double) e.NewValue);
  }

  internal event EventHandler<EventArgs> LeftChanged;

  protected virtual void OnLeftPropertyChanged(double oldValue, double newValue)
  {
    EventHandler<EventArgs> leftChanged = this.LeftChanged;
    if (leftChanged == null)
      return;
    leftChanged((object) this, EventArgs.Empty);
  }

  public double Top
  {
    get => (double) this.GetValue(WindowControl.TopProperty);
    set => this.SetValue(WindowControl.TopProperty, (object) value);
  }

  private static object OnCoerceTop(DependencyObject d, object basevalue)
  {
    if (basevalue == DependencyProperty.UnsetValue)
      return basevalue;
    WindowControl windowControl = (WindowControl) d;
    return windowControl == null ? basevalue : windowControl.OnCoerceTop(basevalue);
  }

  private object OnCoerceTop(object newValue)
  {
    return object.Equals((object) (double) newValue, (object) double.NaN) ? (object) 0.0 : newValue;
  }

  private static void OnTopPropertyChanged(
    DependencyObject obj,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(obj is WindowControl windowControl))
      return;
    windowControl.OnTopPropertyChanged((double) e.OldValue, (double) e.NewValue);
  }

  internal event EventHandler<EventArgs> TopChanged;

  protected virtual void OnTopPropertyChanged(double oldValue, double newValue)
  {
    EventHandler<EventArgs> topChanged = this.TopChanged;
    if (topChanged == null)
      return;
    topChanged((object) this, EventArgs.Empty);
  }

  public Brush WindowBackground
  {
    get => (Brush) this.GetValue(WindowControl.WindowBackgroundProperty);
    set => this.SetValue(WindowControl.WindowBackgroundProperty, (object) value);
  }

  public Brush WindowBorderBrush
  {
    get => (Brush) this.GetValue(WindowControl.WindowBorderBrushProperty);
    set => this.SetValue(WindowControl.WindowBorderBrushProperty, (object) value);
  }

  public Thickness WindowBorderThickness
  {
    get => (Thickness) this.GetValue(WindowControl.WindowBorderThicknessProperty);
    set => this.SetValue(WindowControl.WindowBorderThicknessProperty, (object) value);
  }

  public Brush WindowInactiveBackground
  {
    get => (Brush) this.GetValue(WindowControl.WindowInactiveBackgroundProperty);
    set => this.SetValue(WindowControl.WindowInactiveBackgroundProperty, (object) value);
  }

  public double WindowOpacity
  {
    get => (double) this.GetValue(WindowControl.WindowOpacityProperty);
    set => this.SetValue(WindowControl.WindowOpacityProperty, (object) value);
  }

  public WindowStyle WindowStyle
  {
    get => (WindowStyle) this.GetValue(WindowControl.WindowStyleProperty);
    set => this.SetValue(WindowControl.WindowStyleProperty, (object) value);
  }

  private static object OnCoerceWindowStyle(DependencyObject d, object basevalue)
  {
    return basevalue == DependencyProperty.UnsetValue || !(d is WindowControl windowControl) ? basevalue : windowControl.OnCoerceWindowStyle((WindowStyle) basevalue);
  }

  protected virtual object OnCoerceWindowStyle(WindowStyle newValue) => (object) newValue;

  private static void OnWindowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((WindowControl) d)?.OnWindowStyleChanged((WindowStyle) e.OldValue, (WindowStyle) e.NewValue);
  }

  protected virtual void OnWindowStyleChanged(WindowStyle oldValue, WindowStyle newValue)
  {
  }

  public Thickness WindowThickness
  {
    get => (Thickness) this.GetValue(WindowControl.WindowThicknessProperty);
    set => this.SetValue(WindowControl.WindowThicknessProperty, (object) value);
  }

  internal bool IsStartupPositionInitialized { get; set; }

  internal bool IsBlockMouseInputsPanelActive
  {
    get => this._IsBlockMouseInputsPanelActive;
    set
    {
      if (value == this._IsBlockMouseInputsPanelActive)
        return;
      this._IsBlockMouseInputsPanelActive = value;
      this.UpdateBlockMouseInputsPanel();
    }
  }

  internal virtual bool AllowPublicIsActiveChange => true;

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (this._headerThumb != null)
    {
      this._headerThumb.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(this.HeaderPreviewMouseLeftButtonDown);
      this._headerThumb.PreviewMouseRightButtonDown -= new MouseButtonEventHandler(this.HeaderPreviewMouseRightButtonDown);
      this._headerThumb.DragDelta -= new DragDeltaEventHandler(this.HeaderThumbDragDelta);
    }
    this._headerThumb = this.Template.FindName("PART_HeaderThumb", (FrameworkElement) this) as Thumb;
    if (this._headerThumb != null)
    {
      this._headerThumb.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.HeaderPreviewMouseLeftButtonDown);
      this._headerThumb.PreviewMouseRightButtonDown += new MouseButtonEventHandler(this.HeaderPreviewMouseRightButtonDown);
      this._headerThumb.DragDelta += new DragDeltaEventHandler(this.HeaderThumbDragDelta);
    }
    if (this._icon != null)
      this._icon.MouseLeftButtonDown -= new MouseButtonEventHandler(this.IconMouseLeftButtonDown);
    this._icon = this.Template.FindName("PART_Icon", (FrameworkElement) this) as Image;
    if (this._icon != null)
      this._icon.MouseLeftButtonDown += new MouseButtonEventHandler(this.IconMouseLeftButtonDown);
    if (this._closeButton != null)
      this._closeButton.Click -= new RoutedEventHandler(this.Close);
    this._closeButton = this.Template.FindName("PART_CloseButton", (FrameworkElement) this) as Button;
    if (this._closeButton != null)
      this._closeButton.Click += new RoutedEventHandler(this.Close);
    if (this._windowToolboxCloseButton != null)
      this._windowToolboxCloseButton.Click -= new RoutedEventHandler(this.Close);
    this._windowToolboxCloseButton = this.Template.FindName("PART_ToolWindowCloseButton", (FrameworkElement) this) as Button;
    if (this._windowToolboxCloseButton != null)
      this._windowToolboxCloseButton.Click += new RoutedEventHandler(this.Close);
    this._windowBlockMouseInputsPanel = this.Template.FindName("PART_BlockMouseInputsBorder", (FrameworkElement) this) as Border;
    this.UpdateBlockMouseInputsPanel();
  }

  public event RoutedEventHandler Activated
  {
    add => this.AddHandler(WindowControl.ActivatedEvent, (Delegate) value);
    remove => this.RemoveHandler(WindowControl.ActivatedEvent, (Delegate) value);
  }

  public event MouseButtonEventHandler HeaderMouseLeftButtonClicked
  {
    add => this.AddHandler(WindowControl.HeaderMouseLeftButtonClickedEvent, (Delegate) value);
    remove => this.RemoveHandler(WindowControl.HeaderMouseLeftButtonClickedEvent, (Delegate) value);
  }

  public event MouseButtonEventHandler HeaderMouseRightButtonClicked
  {
    add => this.AddHandler(WindowControl.HeaderMouseRightButtonClickedEvent, (Delegate) value);
    remove
    {
      this.RemoveHandler(WindowControl.HeaderMouseRightButtonClickedEvent, (Delegate) value);
    }
  }

  public event MouseButtonEventHandler HeaderMouseLeftButtonDoubleClicked
  {
    add => this.AddHandler(WindowControl.HeaderMouseLeftButtonDoubleClickedEvent, (Delegate) value);
    remove
    {
      this.RemoveHandler(WindowControl.HeaderMouseLeftButtonDoubleClickedEvent, (Delegate) value);
    }
  }

  public event DragDeltaEventHandler HeaderDragDelta
  {
    add => this.AddHandler(WindowControl.HeaderDragDeltaEvent, (Delegate) value);
    remove => this.RemoveHandler(WindowControl.HeaderDragDeltaEvent, (Delegate) value);
  }

  public event MouseButtonEventHandler HeaderIconClicked
  {
    add => this.AddHandler(WindowControl.HeaderIconClickedEvent, (Delegate) value);
    remove => this.RemoveHandler(WindowControl.HeaderIconClickedEvent, (Delegate) value);
  }

  public event MouseButtonEventHandler HeaderIconDoubleClicked
  {
    add => this.AddHandler(WindowControl.HeaderIconDoubleClickedEvent, (Delegate) value);
    remove => this.RemoveHandler(WindowControl.HeaderIconDoubleClickedEvent, (Delegate) value);
  }

  public event RoutedEventHandler CloseButtonClicked
  {
    add => this.AddHandler(WindowControl.CloseButtonClickedEvent, (Delegate) value);
    remove => this.RemoveHandler(WindowControl.CloseButtonClickedEvent, (Delegate) value);
  }

  private void HeaderPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    MouseButtonEventArgs e1 = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
    e1.RoutedEvent = e.ClickCount == 2 ? WindowControl.HeaderMouseLeftButtonDoubleClickedEvent : WindowControl.HeaderMouseLeftButtonClickedEvent;
    e1.Source = (object) this;
    this.RaiseEvent((RoutedEventArgs) e1);
  }

  private void HeaderPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
  {
    MouseButtonEventArgs e1 = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Right);
    e1.RoutedEvent = WindowControl.HeaderMouseRightButtonClickedEvent;
    e1.Source = (object) this;
    this.RaiseEvent((RoutedEventArgs) e1);
  }

  private void HeaderThumbDragDelta(object sender, DragDeltaEventArgs e)
  {
    DragDeltaEventArgs e1 = new DragDeltaEventArgs(e.HorizontalChange, e.VerticalChange);
    e1.RoutedEvent = WindowControl.HeaderDragDeltaEvent;
    e1.Source = (object) this;
    this.RaiseEvent((RoutedEventArgs) e1);
  }

  private void IconMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    MouseButtonEventArgs e1 = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left);
    e1.RoutedEvent = e.ClickCount == 2 ? WindowControl.HeaderIconDoubleClickedEvent : WindowControl.HeaderIconClickedEvent;
    e1.Source = (object) this;
    this.RaiseEvent((RoutedEventArgs) e1);
  }

  private void Close(object sender, RoutedEventArgs e)
  {
    this.RaiseEvent(new RoutedEventArgs(WindowControl.CloseButtonClickedEvent, (object) this));
  }

  internal virtual void UpdateBlockMouseInputsPanel()
  {
    if (this._windowBlockMouseInputsPanel == null)
      return;
    this._windowBlockMouseInputsPanel.Visibility = this.IsBlockMouseInputsPanelActive ? Visibility.Visible : Visibility.Collapsed;
  }

  internal double GetHeaderHeight()
  {
    return this.Template.FindName("PART_HeaderGrid", (FrameworkElement) this) is Grid name ? name.ActualHeight : 0.0;
  }
}
