// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutFloatingWindowControl
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using Microsoft.Windows.Shell;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public abstract class LayoutFloatingWindowControl : Window, ILayoutControl
{
  private ResourceDictionary currentThemeResourceDictionary;
  private bool _isInternalChange;
  private ILayoutElement _model;
  private bool _attachDrag;
  private HwndSource _hwndSrc;
  private HwndSourceHook _hwndSrcHook;
  private DragService _dragService;
  private bool _internalCloseFlag;
  private bool _isClosing;
  public static readonly DependencyProperty IsContentImmutableProperty = DependencyProperty.Register(nameof (IsContentImmutable), typeof (bool), typeof (LayoutFloatingWindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  private static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsDragging), typeof (bool), typeof (LayoutFloatingWindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(LayoutFloatingWindowControl.OnIsDraggingChanged)));
  public static readonly DependencyProperty IsDraggingProperty = LayoutFloatingWindowControl.IsDraggingPropertyKey.DependencyProperty;
  public static readonly DependencyProperty IsMaximizedProperty = DependencyProperty.Register(nameof (IsMaximized), typeof (bool), typeof (LayoutFloatingWindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));

  static LayoutFloatingWindowControl()
  {
    ContentControl.ContentProperty.OverrideMetadata(typeof (LayoutFloatingWindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, (PropertyChangedCallback) null, new CoerceValueCallback(LayoutFloatingWindowControl.CoerceContentValue)));
    Window.AllowsTransparencyProperty.OverrideMetadata(typeof (LayoutFloatingWindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    Window.ShowInTaskbarProperty.OverrideMetadata(typeof (LayoutFloatingWindowControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  }

  protected LayoutFloatingWindowControl(ILayoutElement model)
  {
    this.Loaded += new RoutedEventHandler(this.OnLoaded);
    this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
    this._model = model;
  }

  protected LayoutFloatingWindowControl(ILayoutElement model, bool isContentImmutable)
    : this(model)
  {
    this.IsContentImmutable = isContentImmutable;
  }

  public abstract ILayoutElement Model { get; }

  public bool IsContentImmutable
  {
    get => (bool) this.GetValue(LayoutFloatingWindowControl.IsContentImmutableProperty);
    private set
    {
      this.SetValue(LayoutFloatingWindowControl.IsContentImmutableProperty, (object) value);
    }
  }

  public bool IsDragging => (bool) this.GetValue(LayoutFloatingWindowControl.IsDraggingProperty);

  protected void SetIsDragging(bool value)
  {
    this.SetValue(LayoutFloatingWindowControl.IsDraggingPropertyKey, (object) value);
  }

  private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutFloatingWindowControl) d).OnIsDraggingChanged(e);
  }

  protected virtual void OnIsDraggingChanged(DependencyPropertyChangedEventArgs e)
  {
    if ((bool) e.NewValue)
      this.CaptureMouse();
    else
      this.ReleaseMouseCapture();
  }

  protected bool CloseInitiatedByUser => !this._internalCloseFlag;

  internal bool KeepContentVisibleOnClose { get; set; }

  public bool IsMaximized
  {
    get => (bool) this.GetValue(LayoutFloatingWindowControl.IsMaximizedProperty);
    private set
    {
      this.SetValue(LayoutFloatingWindowControl.IsMaximizedProperty, (object) value);
      this.UpdatePositionAndSizeOfPanes();
    }
  }

  protected override void OnStateChanged(EventArgs e)
  {
    if (!this._isInternalChange)
    {
      if (this.WindowState == WindowState.Maximized)
        this.UpdateMaximizedState(true);
      else
        this.WindowState = this.IsMaximized ? WindowState.Maximized : WindowState.Normal;
    }
    base.OnStateChanged(e);
  }

  protected override void OnClosed(EventArgs e)
  {
    if (this.Content != null)
    {
      (this.Content as LayoutFloatingWindowControl.FloatingWindowContentHost).Dispose();
      if (this._hwndSrc != null)
      {
        this._hwndSrc.RemoveHook(this._hwndSrcHook);
        this._hwndSrc.Dispose();
        this._hwndSrc = (HwndSource) null;
      }
    }
    base.OnClosed(e);
  }

  protected override void OnInitialized(EventArgs e)
  {
    this.CommandBindings.Add(new CommandBinding((ICommand) SystemCommands.CloseWindowCommand, (ExecutedRoutedEventHandler) ((s, args) => SystemCommands.CloseWindow((Window) args.Parameter))));
    this.CommandBindings.Add(new CommandBinding((ICommand) SystemCommands.MaximizeWindowCommand, (ExecutedRoutedEventHandler) ((s, args) => SystemCommands.MaximizeWindow((Window) args.Parameter))));
    this.CommandBindings.Add(new CommandBinding((ICommand) SystemCommands.MinimizeWindowCommand, (ExecutedRoutedEventHandler) ((s, args) => SystemCommands.MinimizeWindow((Window) args.Parameter))));
    this.CommandBindings.Add(new CommandBinding((ICommand) SystemCommands.RestoreWindowCommand, (ExecutedRoutedEventHandler) ((s, args) => SystemCommands.RestoreWindow((Window) args.Parameter))));
    base.OnInitialized(e);
  }

  internal virtual void UpdateThemeResources(Theme oldTheme = null)
  {
    if (oldTheme != null)
    {
      if (oldTheme is DictionaryTheme)
      {
        if (this.currentThemeResourceDictionary != null)
        {
          this.Resources.MergedDictionaries.Remove(this.currentThemeResourceDictionary);
          this.currentThemeResourceDictionary = (ResourceDictionary) null;
        }
      }
      else
      {
        ResourceDictionary resourceDictionary = this.Resources.MergedDictionaries.FirstOrDefault<ResourceDictionary>((Func<ResourceDictionary, bool>) (r => r.Source == oldTheme.GetResourceUri()));
        if (resourceDictionary != null)
          this.Resources.MergedDictionaries.Remove(resourceDictionary);
      }
    }
    DockingManager manager = this._model.Root.Manager;
    if (manager.Theme == null)
      return;
    if (manager.Theme is DictionaryTheme)
    {
      this.currentThemeResourceDictionary = ((DictionaryTheme) manager.Theme).ThemeResourceDictionary;
      this.Resources.MergedDictionaries.Add(this.currentThemeResourceDictionary);
    }
    else
      this.Resources.MergedDictionaries.Add(new ResourceDictionary()
      {
        Source = manager.Theme.GetResourceUri()
      });
  }

  internal void AttachDrag(bool onActivated = true)
  {
    if (onActivated)
    {
      this._attachDrag = true;
      this.Activated += new EventHandler(this.OnActivated);
    }
    else
    {
      IntPtr handle = new WindowInteropHelper((Window) this).Handle;
      IntPtr num = new IntPtr((int) this.Left & (int) ushort.MaxValue | (int) this.Top << 16 /*0x10*/);
      IntPtr wParam = new IntPtr(2);
      IntPtr lParam = num;
      Win32Helper.SendMessage(handle, 161, wParam, lParam);
    }
  }

  protected virtual IntPtr FilterMessage(
    IntPtr hwnd,
    int msg,
    IntPtr wParam,
    IntPtr lParam,
    ref bool handled)
  {
    handled = false;
    switch (msg)
    {
      case 6:
        if (((int) wParam & (int) ushort.MaxValue) == 0 && lParam == this.GetParentWindowHandle())
        {
          Win32Helper.SetActiveWindow(this._hwndSrc.Handle);
          handled = true;
          break;
        }
        break;
      case 274:
        int num = (int) wParam & 65520;
        switch (num)
        {
          case 61488:
          case 61728:
            this.UpdateMaximizedState(num == 61488);
            break;
        }
        break;
      case 514:
        if (this._dragService != null && Mouse.LeftButton == MouseButtonState.Released)
        {
          this._dragService.Abort();
          this._dragService = (DragService) null;
          this.SetIsDragging(false);
          break;
        }
        break;
      case 534:
        this.UpdateDragPosition();
        if (this.IsMaximized)
        {
          this.UpdateMaximizedState(false);
          break;
        }
        break;
      case 562:
        this.UpdatePositionAndSizeOfPanes();
        if (this._dragService != null)
        {
          bool dropHandled;
          this._dragService.Drop(this.TransformToDeviceDPI(Win32Helper.GetMousePosition()), out dropHandled);
          this._dragService = (DragService) null;
          this.SetIsDragging(false);
          if (dropHandled)
          {
            this.InternalClose();
            break;
          }
          break;
        }
        break;
    }
    return IntPtr.Zero;
  }

  internal void InternalClose()
  {
    this._internalCloseFlag = true;
    if (this._isClosing)
      return;
    this._isClosing = true;
    this.Close();
  }

  private static object CoerceContentValue(DependencyObject sender, object content)
  {
    if (!(sender is LayoutFloatingWindowControl floatingWindowControl))
      return (object) null;
    if (floatingWindowControl.IsLoaded && floatingWindowControl.IsContentImmutable)
      return floatingWindowControl.Content;
    return (object) new LayoutFloatingWindowControl.FloatingWindowContentHost(sender as LayoutFloatingWindowControl)
    {
      Content = (content as UIElement)
    };
  }

  private void OnLoaded(object sender, RoutedEventArgs e)
  {
    this.Loaded -= new RoutedEventHandler(this.OnLoaded);
    this.SetParentToMainWindowOf((Visual) this.Model.Root.Manager);
    this._hwndSrc = PresentationSource.FromDependencyObject((DependencyObject) this) as HwndSource;
    this._hwndSrcHook = new HwndSourceHook(this.FilterMessage);
    this._hwndSrc.AddHook(this._hwndSrcHook);
    this.UpdateMaximizedState(this.Model.Descendents().OfType<ILayoutElementForFloatingWindow>().Any<ILayoutElementForFloatingWindow>((Func<ILayoutElementForFloatingWindow, bool>) (l => l.IsMaximized)));
  }

  private void OnUnloaded(object sender, RoutedEventArgs e)
  {
    this.Unloaded -= new RoutedEventHandler(this.OnUnloaded);
    if (this._hwndSrc == null)
      return;
    this._hwndSrc.RemoveHook(this._hwndSrcHook);
    this.InternalClose();
  }

  private void OnActivated(object sender, EventArgs e)
  {
    this.Activated -= new EventHandler(this.OnActivated);
    if (!this._attachDrag || Mouse.LeftButton != MouseButtonState.Pressed)
      return;
    IntPtr handle = new WindowInteropHelper((Window) this).Handle;
    Point screenDpi = this.PointToScreenDPI(Mouse.GetPosition((IInputElement) this));
    Win32Helper.RECT clientRect = Win32Helper.GetClientRect(handle);
    Win32Helper.RECT windowRect = Win32Helper.GetWindowRect(handle);
    this.Left = screenDpi.X - (double) windowRect.Width / 2.0;
    this.Top = screenDpi.Y - (double) (windowRect.Height - clientRect.Height) / 2.0;
    this._attachDrag = false;
    Win32Helper.SendMessage(handle, 161, new IntPtr(2), new IntPtr((int) screenDpi.X & (int) ushort.MaxValue | (int) screenDpi.Y << 16 /*0x10*/));
  }

  private void UpdatePositionAndSizeOfPanes()
  {
    foreach (ILayoutElementForFloatingWindow forFloatingWindow in this.Model.Descendents().OfType<ILayoutElementForFloatingWindow>())
    {
      forFloatingWindow.FloatingLeft = this.Left;
      forFloatingWindow.FloatingTop = this.Top;
      forFloatingWindow.FloatingWidth = this.Width;
      forFloatingWindow.FloatingHeight = this.Height;
    }
  }

  private void UpdateMaximizedState(bool isMaximized)
  {
    foreach (ILayoutElementForFloatingWindow forFloatingWindow in this.Model.Descendents().OfType<ILayoutElementForFloatingWindow>())
      forFloatingWindow.IsMaximized = isMaximized;
    this.IsMaximized = isMaximized;
    this._isInternalChange = true;
    this.WindowState = isMaximized ? WindowState.Maximized : WindowState.Normal;
    this._isInternalChange = false;
  }

  private void UpdateDragPosition()
  {
    if (this._dragService == null)
    {
      this._dragService = new DragService(this);
      this.SetIsDragging(true);
    }
    this._dragService.UpdateMouseLocation(this.TransformToDeviceDPI(Win32Helper.GetMousePosition()));
  }

  protected internal class FloatingWindowContentHost : HwndHost
  {
    private LayoutFloatingWindowControl _owner;
    private HwndSource _wpfContentHost;
    private Border _rootPresenter;
    private DockingManager _manager;
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof (Content), typeof (UIElement), typeof (LayoutFloatingWindowControl.FloatingWindowContentHost), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutFloatingWindowControl.FloatingWindowContentHost.OnContentChanged)));
    public static readonly DependencyProperty SizeToContentProperty = DependencyProperty.Register(nameof (SizeToContent), typeof (SizeToContent), typeof (LayoutFloatingWindowControl.FloatingWindowContentHost), (PropertyMetadata) new FrameworkPropertyMetadata((object) SizeToContent.Manual, new PropertyChangedCallback(LayoutFloatingWindowControl.FloatingWindowContentHost.OnSizeToContentChanged)));

    public FloatingWindowContentHost(LayoutFloatingWindowControl owner)
    {
      this._owner = owner;
      DockingManager manager = this._owner.Model.Root.Manager;
      Binding binding = new Binding(nameof (SizeToContent))
      {
        Source = (object) this._owner
      };
      BindingOperations.SetBinding((DependencyObject) this, LayoutFloatingWindowControl.FloatingWindowContentHost.SizeToContentProperty, (BindingBase) binding);
    }

    public Visual RootVisual => (Visual) this._rootPresenter;

    public UIElement Content
    {
      get
      {
        return (UIElement) this.GetValue(LayoutFloatingWindowControl.FloatingWindowContentHost.ContentProperty);
      }
      set
      {
        this.SetValue(LayoutFloatingWindowControl.FloatingWindowContentHost.ContentProperty, (object) value);
      }
    }

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((LayoutFloatingWindowControl.FloatingWindowContentHost) d).OnContentChanged((UIElement) e.OldValue, (UIElement) e.NewValue);
    }

    protected virtual void OnContentChanged(UIElement oldValue, UIElement newValue)
    {
      if (this._rootPresenter != null)
        this._rootPresenter.Child = this.Content;
      if (oldValue is FrameworkElement frameworkElement1)
        frameworkElement1.SizeChanged -= new SizeChangedEventHandler(this.Content_SizeChanged);
      if (!(newValue is FrameworkElement frameworkElement2))
        return;
      frameworkElement2.SizeChanged += new SizeChangedEventHandler(this.Content_SizeChanged);
    }

    public SizeToContent SizeToContent
    {
      get
      {
        return (SizeToContent) this.GetValue(LayoutFloatingWindowControl.FloatingWindowContentHost.SizeToContentProperty);
      }
      set
      {
        this.SetValue(LayoutFloatingWindowControl.FloatingWindowContentHost.SizeToContentProperty, (object) value);
      }
    }

    private static void OnSizeToContentChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      ((LayoutFloatingWindowControl.FloatingWindowContentHost) d).OnSizeToContentChanged((SizeToContent) e.OldValue, (SizeToContent) e.NewValue);
    }

    protected virtual void OnSizeToContentChanged(SizeToContent oldValue, SizeToContent newValue)
    {
      if (this._wpfContentHost == null)
        return;
      this._wpfContentHost.SizeToContent = newValue;
    }

    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
      this._wpfContentHost = new HwndSource(new HwndSourceParameters()
      {
        ParentWindow = hwndParent.Handle,
        WindowStyle = 1442840576 /*0x56000000*/,
        Width = 1,
        Height = 1
      });
      Border border = new Border();
      AdornerDecorator adornerDecorator = new AdornerDecorator();
      adornerDecorator.Child = this.Content;
      border.Child = (UIElement) adornerDecorator;
      border.Focusable = true;
      this._rootPresenter = border;
      this._rootPresenter.SetBinding(Border.BackgroundProperty, (BindingBase) new Binding("Background")
      {
        Source = (object) this._owner
      });
      this._wpfContentHost.RootVisual = (Visual) this._rootPresenter;
      this._manager = this._owner.Model.Root.Manager;
      this._manager.InternalAddLogicalChild((object) this._rootPresenter);
      return new HandleRef((object) this, this._wpfContentHost.Handle);
    }

    protected override void DestroyWindowCore(HandleRef hwnd)
    {
      this._manager.InternalRemoveLogicalChild((object) this._rootPresenter);
      if (this._wpfContentHost == null)
        return;
      this._wpfContentHost.Dispose();
      this._wpfContentHost = (HwndSource) null;
    }

    protected override Size MeasureOverride(Size constraint)
    {
      if (this.Content == null)
        return base.MeasureOverride(constraint);
      this.Content.Measure(constraint);
      return this.Content.DesiredSize;
    }

    private void Content_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.InvalidateMeasure();
      this.InvalidateArrange();
    }
  }
}
