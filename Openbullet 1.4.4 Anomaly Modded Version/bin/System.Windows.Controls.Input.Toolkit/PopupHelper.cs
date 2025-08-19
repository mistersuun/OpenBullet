// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.PopupHelper
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace System.Windows.Controls;

internal class PopupHelper
{
  public bool UsesClosingVisualState { get; private set; }

  private Control Parent { get; set; }

  public double MaxDropDownHeight { get; set; }

  public Popup Popup { get; private set; }

  public bool IsOpen
  {
    get => this.Popup.IsOpen;
    set => this.Popup.IsOpen = value;
  }

  private FrameworkElement PopupChild { get; set; }

  public event EventHandler Closed;

  public event EventHandler FocusChanged;

  public event EventHandler UpdateVisualStates;

  public PopupHelper(Control parent) => this.Parent = parent;

  public PopupHelper(Control parent, Popup popup)
    : this(parent)
  {
    this.Popup = popup;
  }

  public void Arrange()
  {
    if (this.Popup == null || this.PopupChild == null || Application.Current == null)
      return;
    UIElement reference = (UIElement) this.Parent;
    if (Application.Current.Windows.Count > 0)
      reference = (UIElement) Application.Current.Windows[0];
    while (true)
    {
      switch (reference)
      {
        case Window _:
        case null:
          goto label_6;
        default:
          reference = VisualTreeHelper.GetParent((DependencyObject) reference) as UIElement;
          continue;
      }
    }
label_6:
    if (!(reference is Window window))
      return;
    double actualWidth1 = window.ActualWidth;
    double actualHeight1 = window.ActualHeight;
    double actualWidth2 = this.PopupChild.ActualWidth;
    double actualHeight2 = this.PopupChild.ActualHeight;
    if (actualHeight1 == 0.0 || actualWidth1 == 0.0 || actualWidth2 == 0.0 || actualHeight2 == 0.0)
      return;
    double num1 = 0.0;
    double val1 = 0.0;
    double actualHeight3 = this.Parent.ActualHeight;
    double actualWidth3 = this.Parent.ActualWidth;
    double num2 = this.MaxDropDownHeight;
    if (double.IsInfinity(num2) || double.IsNaN(num2))
      num2 = (actualHeight1 - actualHeight3) * 3.0 / 5.0;
    double val2_1 = Math.Min(actualWidth2, actualWidth1);
    double num3 = Math.Min(actualHeight2, num2);
    double num4 = Math.Max(actualWidth3, val2_1);
    double num5 = num1;
    if (actualWidth1 < num5 + num4)
      num5 = Math.Max(0.0, actualWidth1 - num4);
    bool flag = true;
    double num6 = val1 + actualHeight3;
    if (actualHeight1 < num6 + num3)
    {
      flag = false;
      num6 = val1 - num3;
      if (num6 < 0.0)
      {
        if (val1 < (actualHeight1 - actualHeight3) / 2.0)
        {
          flag = true;
          num6 = val1 + actualHeight3;
        }
        else
        {
          flag = false;
          num6 = val1 - num3;
        }
      }
    }
    double val2_2 = flag ? Math.Min(actualHeight1 - num6, num2) : Math.Min(val1, num2);
    this.Popup.HorizontalOffset = 0.0;
    this.Popup.VerticalOffset = 0.0;
    this.PopupChild.MinWidth = actualWidth3;
    this.PopupChild.MaxWidth = actualWidth1;
    this.PopupChild.MinHeight = 0.0;
    this.PopupChild.MaxHeight = Math.Max(0.0, val2_2);
    this.PopupChild.Width = num4;
    this.PopupChild.HorizontalAlignment = HorizontalAlignment.Left;
    this.PopupChild.VerticalAlignment = VerticalAlignment.Top;
    Canvas.SetLeft((UIElement) this.PopupChild, num5 - num1);
    Canvas.SetTop((UIElement) this.PopupChild, num6 - val1);
  }

  private void OnClosed(EventArgs e)
  {
    EventHandler closed = this.Closed;
    if (closed == null)
      return;
    closed((object) this, e);
  }

  private void OnPopupClosedStateChanged(object sender, VisualStateChangedEventArgs e)
  {
    if (e == null || e.NewState == null || !(e.NewState.Name == "PopupClosed"))
      return;
    if (this.Popup != null)
      this.Popup.IsOpen = false;
    this.OnClosed(EventArgs.Empty);
  }

  public void BeforeOnApplyTemplate()
  {
    if (this.UsesClosingVisualState)
    {
      VisualStateGroup visualStateGroup = VisualStates.TryGetVisualStateGroup((DependencyObject) this.Parent, "PopupStates");
      if (visualStateGroup != null)
      {
        visualStateGroup.CurrentStateChanged -= new EventHandler<VisualStateChangedEventArgs>(this.OnPopupClosedStateChanged);
        this.UsesClosingVisualState = false;
      }
    }
    if (this.Popup == null)
      return;
    this.Popup.Closed -= new EventHandler(this.Popup_Closed);
  }

  public void AfterOnApplyTemplate()
  {
    if (this.Popup != null)
      this.Popup.Closed += new EventHandler(this.Popup_Closed);
    VisualStateGroup visualStateGroup = VisualStates.TryGetVisualStateGroup((DependencyObject) this.Parent, "PopupStates");
    if (visualStateGroup != null)
    {
      visualStateGroup.CurrentStateChanged += new EventHandler<VisualStateChangedEventArgs>(this.OnPopupClosedStateChanged);
      this.UsesClosingVisualState = true;
    }
    if (this.Popup == null)
      return;
    this.PopupChild = this.Popup.Child as FrameworkElement;
    if (this.PopupChild == null)
      return;
    this.PopupChild.GotFocus += new RoutedEventHandler(this.PopupChild_GotFocus);
    this.PopupChild.LostFocus += new RoutedEventHandler(this.PopupChild_LostFocus);
    this.PopupChild.MouseEnter += new MouseEventHandler(this.PopupChild_MouseEnter);
    this.PopupChild.MouseLeave += new MouseEventHandler(this.PopupChild_MouseLeave);
    this.PopupChild.SizeChanged += new SizeChangedEventHandler(this.PopupChild_SizeChanged);
  }

  private void PopupChild_SizeChanged(object sender, SizeChangedEventArgs e) => this.Arrange();

  private void OutsidePopup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    if (this.Popup == null)
      return;
    this.Popup.IsOpen = false;
  }

  private void Popup_Closed(object sender, EventArgs e) => this.OnClosed(EventArgs.Empty);

  private void OnFocusChanged(EventArgs e)
  {
    EventHandler focusChanged = this.FocusChanged;
    if (focusChanged == null)
      return;
    focusChanged((object) this, e);
  }

  private void OnUpdateVisualStates(EventArgs e)
  {
    EventHandler updateVisualStates = this.UpdateVisualStates;
    if (updateVisualStates == null)
      return;
    updateVisualStates((object) this, e);
  }

  private void PopupChild_GotFocus(object sender, RoutedEventArgs e)
  {
    this.OnFocusChanged(EventArgs.Empty);
  }

  private void PopupChild_LostFocus(object sender, RoutedEventArgs e)
  {
    this.OnFocusChanged(EventArgs.Empty);
  }

  private void PopupChild_MouseEnter(object sender, MouseEventArgs e)
  {
    this.OnUpdateVisualStates(EventArgs.Empty);
  }

  private void PopupChild_MouseLeave(object sender, MouseEventArgs e)
  {
    this.OnUpdateVisualStates(EventArgs.Empty);
  }
}
