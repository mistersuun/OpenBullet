// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Primitives.WindowContainer
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit.Primitives;

public class WindowContainer : Canvas
{
  private Brush _defaultBackgroundBrush;
  private bool _isModalBackgroundApplied;
  public static readonly DependencyProperty ModalBackgroundBrushProperty;

  static WindowContainer()
  {
    SolidColorBrush defaultValue = new SolidColorBrush(Colors.Transparent);
    defaultValue.Freeze();
    WindowContainer.ModalBackgroundBrushProperty = DependencyProperty.Register(nameof (ModalBackgroundBrush), typeof (Brush), typeof (WindowContainer), (PropertyMetadata) new UIPropertyMetadata((object) defaultValue, new PropertyChangedCallback(WindowContainer.OnModalBackgroundBrushChanged)));
  }

  public WindowContainer()
  {
    this.SizeChanged += new SizeChangedEventHandler(this.WindowContainer_SizeChanged);
    this.LayoutUpdated += new EventHandler(this.WindowContainer_LayoutUpdated);
    this.Loaded += new RoutedEventHandler(this.WindowContainer_Loaded);
    this.ClipToBounds = true;
  }

  private void WindowContainer_Loaded(object sender, RoutedEventArgs e)
  {
    foreach (WindowControl child in this.Children)
      child.SetIsActiveInternal(false);
    this.SetNextActiveWindow((WindowControl) null);
  }

  public Brush ModalBackgroundBrush
  {
    get => (Brush) this.GetValue(WindowContainer.ModalBackgroundBrushProperty);
    set => this.SetValue(WindowContainer.ModalBackgroundBrushProperty, (object) value);
  }

  private static void OnModalBackgroundBrushChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((WindowContainer) d)?.OnModalBackgroundBrushChanged((Brush) e.OldValue, (Brush) e.NewValue);
  }

  protected virtual void OnModalBackgroundBrushChanged(Brush oldValue, Brush newValue)
  {
    this.SetModalBackground();
  }

  protected override Size MeasureOverride(Size constraint)
  {
    Size size = base.MeasureOverride(constraint);
    if (this.Children.Count <= 0)
      return size;
    double val1_1 = double.IsNaN(this.Width) ? this.Children.OfType<WindowControl>().Max<WindowControl>((Func<WindowControl, double>) (w => w.Left + w.DesiredSize.Width)) : this.Width;
    double val1_2 = double.IsNaN(this.Height) ? this.Children.OfType<WindowControl>().Max<WindowControl>((Func<WindowControl, double>) (w => w.Top + w.DesiredSize.Height)) : this.Height;
    double width = constraint.Width;
    return new Size(Math.Min(val1_1, width), Math.Min(val1_2, constraint.Height));
  }

  protected override void OnVisualChildrenChanged(
    DependencyObject visualAdded,
    DependencyObject visualRemoved)
  {
    base.OnVisualChildrenChanged(visualAdded, visualRemoved);
    switch (visualAdded)
    {
      case null:
      case WindowControl _:
        if (visualRemoved != null)
        {
          WindowControl windowControl = (WindowControl) visualRemoved;
          windowControl.LeftChanged -= new EventHandler<EventArgs>(this.Child_LeftChanged);
          windowControl.TopChanged -= new EventHandler<EventArgs>(this.Child_TopChanged);
          windowControl.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(this.Child_PreviewMouseLeftButtonDown);
          windowControl.IsVisibleChanged -= new DependencyPropertyChangedEventHandler(this.Child_IsVisibleChanged);
          windowControl.IsKeyboardFocusWithinChanged -= new DependencyPropertyChangedEventHandler(this.Child_IsKeyboardFocusWithinChanged);
          if (windowControl is ChildWindow)
            ((ChildWindow) windowControl).IsModalChanged -= new EventHandler<EventArgs>(this.Child_IsModalChanged);
        }
        if (visualAdded == null)
          break;
        WindowControl windowControl1 = (WindowControl) visualAdded;
        windowControl1.LeftChanged += new EventHandler<EventArgs>(this.Child_LeftChanged);
        windowControl1.TopChanged += new EventHandler<EventArgs>(this.Child_TopChanged);
        windowControl1.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.Child_PreviewMouseLeftButtonDown);
        windowControl1.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.Child_IsVisibleChanged);
        windowControl1.IsKeyboardFocusWithinChanged += new DependencyPropertyChangedEventHandler(this.Child_IsKeyboardFocusWithinChanged);
        if (!(windowControl1 is ChildWindow))
          break;
        ((ChildWindow) windowControl1).IsModalChanged += new EventHandler<EventArgs>(this.Child_IsModalChanged);
        break;
      default:
        throw new InvalidOperationException("WindowContainer can only contain WindowControl types.");
    }
  }

  private void Child_LeftChanged(object sender, EventArgs e)
  {
    WindowControl windowControl = (WindowControl) sender;
    if (windowControl != null)
      windowControl.Left = this.GetRestrictedLeft(windowControl);
    Canvas.SetLeft((UIElement) windowControl, windowControl.Left);
  }

  private void Child_TopChanged(object sender, EventArgs e)
  {
    WindowControl windowControl = (WindowControl) sender;
    if (windowControl != null)
      windowControl.Top = this.GetRestrictedTop(windowControl);
    Canvas.SetTop((UIElement) windowControl, windowControl.Top);
  }

  private void Child_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
  {
    WindowControl windowControl = (WindowControl) sender;
    if (this.GetModalWindow() != null)
      return;
    this.SetNextActiveWindow(windowControl);
  }

  private void Child_IsModalChanged(object sender, EventArgs e) => this.SetModalBackground();

  private void Child_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
  {
    WindowControl windowControl = (WindowControl) sender;
    this.IsHitTestVisible = this.Children.OfType<WindowControl>().FirstOrDefault<WindowControl>((Func<WindowControl, bool>) (x => x.Visibility == Visibility.Visible)) != null;
    if ((bool) e.NewValue)
    {
      this.SetChildPos(windowControl);
      this.SetNextActiveWindow(windowControl);
    }
    else
      this.SetNextActiveWindow((WindowControl) null);
    WindowControl modalWindow = this.GetModalWindow();
    foreach (WindowControl child in this.Children)
      child.IsBlockMouseInputsPanelActive = modalWindow != null && !object.Equals((object) modalWindow, (object) child);
    this.SetModalBackground();
  }

  private void Child_IsKeyboardFocusWithinChanged(
    object sender,
    DependencyPropertyChangedEventArgs e)
  {
    WindowControl windowControl = (WindowControl) sender;
    if (!(bool) e.NewValue)
      return;
    this.SetNextActiveWindow(windowControl);
  }

  private void WindowContainer_LayoutUpdated(object sender, EventArgs e)
  {
    foreach (WindowControl child in this.Children)
    {
      if (!child.IsStartupPositionInitialized && child.ActualWidth != 0.0 && child.ActualHeight != 0.0)
      {
        this.SetChildPos(child);
        child.IsStartupPositionInitialized = true;
      }
    }
  }

  private void WindowContainer_SizeChanged(object sender, SizeChangedEventArgs e)
  {
    foreach (WindowControl child in this.Children)
    {
      child.Left = this.GetRestrictedLeft(child);
      child.Top = this.GetRestrictedTop(child);
    }
  }

  private void ExpandWindowControl(WindowControl windowControl)
  {
    if (windowControl == null)
      return;
    windowControl.Left = 0.0;
    windowControl.Top = 0.0;
    windowControl.Width = Math.Min(this.ActualWidth, windowControl.MaxWidth);
    windowControl.Height = Math.Min(this.ActualHeight, windowControl.MaxHeight);
  }

  private void SetChildPos(WindowControl windowControl)
  {
    if (windowControl is Xceed.Wpf.Toolkit.MessageBox && windowControl.Left == 0.0 && windowControl.Top == 0.0 || windowControl is ChildWindow && ((ChildWindow) windowControl).WindowStartupLocation == Xceed.Wpf.Toolkit.WindowStartupLocation.Center)
    {
      this.CenterChild(windowControl);
    }
    else
    {
      Canvas.SetLeft((UIElement) windowControl, windowControl.Left);
      Canvas.SetTop((UIElement) windowControl, windowControl.Top);
    }
  }

  private void CenterChild(WindowControl windowControl)
  {
    windowControl.UpdateLayout();
    if (windowControl.ActualWidth == 0.0 || windowControl.ActualHeight == 0.0)
      return;
    windowControl.Left = (this.ActualWidth - windowControl.ActualWidth) / 2.0;
    windowControl.Left += windowControl.Margin.Left - windowControl.Margin.Right;
    windowControl.Top = (this.ActualHeight - windowControl.ActualHeight) / 2.0;
    windowControl.Top += windowControl.Margin.Top - windowControl.Margin.Bottom;
  }

  private void SetNextActiveWindow(WindowControl windowControl)
  {
    if (!this.IsLoaded)
      return;
    if (this.IsModalWindow(windowControl))
    {
      this.BringToFront(windowControl);
    }
    else
    {
      WindowControl modalWindow = this.GetModalWindow();
      if (modalWindow != null)
        this.BringToFront(modalWindow);
      else if (windowControl != null)
        this.BringToFront(windowControl);
      else
        this.BringToFront(this.Children.OfType<WindowControl>().OrderByDescending<WindowControl, int>((Func<WindowControl, int>) (x => Panel.GetZIndex((UIElement) x))).FirstOrDefault<WindowControl>((Func<WindowControl, bool>) (x => x.Visibility == Visibility.Visible)));
    }
  }

  private void BringToFront(WindowControl windowControl)
  {
    if (windowControl == null)
      return;
    int num = this.Children.OfType<WindowControl>().Max<WindowControl>((Func<WindowControl, int>) (x => Panel.GetZIndex((UIElement) x)));
    Panel.SetZIndex((UIElement) windowControl, num + 1);
    this.SetActiveWindow(windowControl);
  }

  private void SetActiveWindow(WindowControl windowControl)
  {
    if (windowControl.IsActive)
      return;
    foreach (WindowControl child in this.Children)
      child.SetIsActiveInternal(false);
    windowControl.SetIsActiveInternal(true);
  }

  private bool IsModalWindow(WindowControl windowControl)
  {
    switch (windowControl)
    {
      case Xceed.Wpf.Toolkit.MessageBox _ when windowControl.Visibility == Visibility.Visible:
        return true;
      case ChildWindow _ when ((ChildWindow) windowControl).IsModal:
        return ((ChildWindow) windowControl).WindowState == Xceed.Wpf.Toolkit.WindowState.Open;
      default:
        return false;
    }
  }

  private WindowControl GetModalWindow()
  {
    return this.Children.OfType<WindowControl>().OrderByDescending<WindowControl, int>((Func<WindowControl, int>) (x => Panel.GetZIndex((UIElement) x))).FirstOrDefault<WindowControl>((Func<WindowControl, bool>) (x => this.IsModalWindow(x) && x.Visibility == Visibility.Visible));
  }

  private double GetRestrictedLeft(WindowControl windowControl)
  {
    if (windowControl.Left < 0.0)
      return 0.0;
    if (windowControl.Left + windowControl.ActualWidth <= this.ActualWidth || this.ActualWidth == 0.0)
      return windowControl.Left;
    double num = this.ActualWidth - windowControl.ActualWidth;
    return num >= 0.0 ? num : 0.0;
  }

  private double GetRestrictedTop(WindowControl windowControl)
  {
    if (windowControl.Top < 0.0)
      return 0.0;
    if (windowControl.Top + windowControl.ActualHeight <= this.ActualHeight || this.ActualHeight == 0.0)
      return windowControl.Top;
    double num = this.ActualHeight - windowControl.ActualHeight;
    return num >= 0.0 ? num : 0.0;
  }

  private void SetModalBackground()
  {
    if (this.GetModalWindow() != null && this.ModalBackgroundBrush != null)
    {
      if (!this._isModalBackgroundApplied)
      {
        this._defaultBackgroundBrush = this.Background;
        this._isModalBackgroundApplied = true;
      }
      this.Background = this.ModalBackgroundBrush;
    }
    else
    {
      if (!this._isModalBackgroundApplied)
        return;
      this.Background = this._defaultBackgroundBrush;
      this._defaultBackgroundBrush = (Brush) null;
      this._isModalBackgroundApplied = false;
    }
  }
}
