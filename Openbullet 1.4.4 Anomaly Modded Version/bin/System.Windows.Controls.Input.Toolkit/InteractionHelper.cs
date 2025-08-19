// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.InteractionHelper
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

using System.Windows.Input;

#nullable disable
namespace System.Windows.Controls;

internal sealed class InteractionHelper
{
  private const double SequentialClickThresholdInMilliseconds = 500.0;
  private const double SequentialClickThresholdInPixelsSquared = 9.0;
  private IUpdateVisualState _updateVisualState;

  public Control Control { get; private set; }

  public bool IsFocused { get; private set; }

  public bool IsMouseOver { get; private set; }

  public bool IsReadOnly { get; private set; }

  public bool IsPressed { get; private set; }

  private DateTime LastClickTime { get; set; }

  private Point LastClickPosition { get; set; }

  public int ClickCount { get; private set; }

  public InteractionHelper(Control control)
  {
    this.Control = control;
    this._updateVisualState = control as IUpdateVisualState;
    control.Loaded += new RoutedEventHandler(this.OnLoaded);
    control.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.OnIsEnabledChanged);
  }

  private void UpdateVisualState(bool useTransitions)
  {
    if (this._updateVisualState == null)
      return;
    this._updateVisualState.UpdateVisualState(useTransitions);
  }

  public void UpdateVisualStateBase(bool useTransitions)
  {
    if (!this.Control.IsEnabled)
      VisualStates.GoToState(this.Control, (useTransitions ? 1 : 0) != 0, "Disabled", "Normal");
    else if (this.IsReadOnly)
      VisualStates.GoToState(this.Control, (useTransitions ? 1 : 0) != 0, "ReadOnly", "Normal");
    else if (this.IsPressed)
      VisualStates.GoToState(this.Control, (useTransitions ? 1 : 0) != 0, "Pressed", "MouseOver", "Normal");
    else if (this.IsMouseOver)
      VisualStates.GoToState(this.Control, (useTransitions ? 1 : 0) != 0, "MouseOver", "Normal");
    else
      VisualStates.GoToState(this.Control, (useTransitions ? 1 : 0) != 0, "Normal");
    if (this.IsFocused)
      VisualStates.GoToState(this.Control, (useTransitions ? 1 : 0) != 0, "Focused", "Unfocused");
    else
      VisualStates.GoToState(this.Control, (useTransitions ? 1 : 0) != 0, "Unfocused");
  }

  private void OnLoaded(object sender, RoutedEventArgs e) => this.UpdateVisualState(false);

  private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
  {
    if (!(bool) e.NewValue)
    {
      this.IsPressed = false;
      this.IsMouseOver = false;
      this.IsFocused = false;
    }
    this.UpdateVisualState(true);
  }

  public void OnIsReadOnlyChanged(bool value)
  {
    this.IsReadOnly = value;
    if (!value)
    {
      this.IsPressed = false;
      this.IsMouseOver = false;
      this.IsFocused = false;
    }
    this.UpdateVisualState(true);
  }

  public void OnApplyTemplateBase() => this.UpdateVisualState(false);

  public bool AllowGotFocus(RoutedEventArgs e)
  {
    if (e == null)
      throw new ArgumentNullException(nameof (e));
    bool isEnabled = this.Control.IsEnabled;
    if (isEnabled)
      this.IsFocused = true;
    return isEnabled;
  }

  public void OnGotFocusBase() => this.UpdateVisualState(true);

  public bool AllowLostFocus(RoutedEventArgs e)
  {
    if (e == null)
      throw new ArgumentNullException(nameof (e));
    bool isEnabled = this.Control.IsEnabled;
    if (isEnabled)
      this.IsFocused = false;
    return isEnabled;
  }

  public void OnLostFocusBase()
  {
    this.IsPressed = false;
    this.UpdateVisualState(true);
  }

  public bool AllowMouseEnter(MouseEventArgs e)
  {
    if (e == null)
      throw new ArgumentNullException(nameof (e));
    bool isEnabled = this.Control.IsEnabled;
    if (isEnabled)
      this.IsMouseOver = true;
    return isEnabled;
  }

  public void OnMouseEnterBase() => this.UpdateVisualState(true);

  public bool AllowMouseLeave(MouseEventArgs e)
  {
    if (e == null)
      throw new ArgumentNullException(nameof (e));
    bool isEnabled = this.Control.IsEnabled;
    if (isEnabled)
      this.IsMouseOver = false;
    return isEnabled;
  }

  public void OnMouseLeaveBase() => this.UpdateVisualState(true);

  public bool AllowMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    if (e == null)
      throw new ArgumentNullException(nameof (e));
    bool isEnabled = this.Control.IsEnabled;
    if (isEnabled)
    {
      DateTime utcNow = DateTime.UtcNow;
      Point position = e.GetPosition((IInputElement) this.Control);
      double totalMilliseconds = (utcNow - this.LastClickTime).TotalMilliseconds;
      Point lastClickPosition = this.LastClickPosition;
      double num1 = position.X - lastClickPosition.X;
      double num2 = position.Y - lastClickPosition.Y;
      double num3 = num1 * num1 + num2 * num2;
      if (totalMilliseconds < 500.0 && num3 < 9.0)
        ++this.ClickCount;
      else
        this.ClickCount = 1;
      this.LastClickTime = utcNow;
      this.LastClickPosition = position;
      this.IsPressed = true;
    }
    else
      this.ClickCount = 1;
    return isEnabled;
  }

  public void OnMouseLeftButtonDownBase() => this.UpdateVisualState(true);

  public bool AllowMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    if (e == null)
      throw new ArgumentNullException(nameof (e));
    bool isEnabled = this.Control.IsEnabled;
    if (isEnabled)
      this.IsPressed = false;
    return isEnabled;
  }

  public void OnMouseLeftButtonUpBase() => this.UpdateVisualState(true);

  public bool AllowKeyDown(KeyEventArgs e)
  {
    if (e == null)
      throw new ArgumentNullException(nameof (e));
    return this.Control.IsEnabled;
  }

  public bool AllowKeyUp(KeyEventArgs e)
  {
    if (e == null)
      throw new ArgumentNullException(nameof (e));
    return this.Control.IsEnabled;
  }
}
