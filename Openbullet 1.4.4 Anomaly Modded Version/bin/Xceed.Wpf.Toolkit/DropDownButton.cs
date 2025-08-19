// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.DropDownButton
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_DropDownButton", Type = typeof (ToggleButton))]
[TemplatePart(Name = "PART_ContentPresenter", Type = typeof (ContentPresenter))]
[TemplatePart(Name = "PART_Popup", Type = typeof (Popup))]
public class DropDownButton : ContentControl, ICommandSource
{
  private const string PART_DropDownButton = "PART_DropDownButton";
  private const string PART_ContentPresenter = "PART_ContentPresenter";
  private const string PART_Popup = "PART_Popup";
  private ContentPresenter _contentPresenter;
  private Popup _popup;
  private ButtonBase _button;
  public static readonly DependencyProperty DropDownContentProperty = DependencyProperty.Register(nameof (DropDownContent), typeof (object), typeof (DropDownButton), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(DropDownButton.OnDropDownContentChanged)));
  public static readonly DependencyProperty DropDownContentBackgroundProperty = DependencyProperty.Register(nameof (DropDownContentBackground), typeof (Brush), typeof (DropDownButton), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty DropDownPositionProperty = DependencyProperty.Register(nameof (DropDownPosition), typeof (PlacementMode), typeof (DropDownButton), (PropertyMetadata) new UIPropertyMetadata((object) PlacementMode.Bottom));
  public static readonly DependencyProperty IsDefaultProperty = DependencyProperty.Register(nameof (IsDefault), typeof (bool), typeof (DropDownButton), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(DropDownButton.OnIsDefaultChanged)));
  public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof (IsOpen), typeof (bool), typeof (DropDownButton), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(DropDownButton.OnIsOpenChanged)));
  public static readonly DependencyProperty MaxDropDownHeightProperty = DependencyProperty.Register(nameof (MaxDropDownHeight), typeof (double), typeof (DropDownButton), (PropertyMetadata) new UIPropertyMetadata((object) (SystemParameters.PrimaryScreenHeight / 2.0), new PropertyChangedCallback(DropDownButton.OnMaxDropDownHeightChanged)));
  public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (DropDownButton));
  public static readonly RoutedEvent OpenedEvent = EventManager.RegisterRoutedEvent("Opened", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (DropDownButton));
  public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (DropDownButton));
  private EventHandler canExecuteChangedHandler;
  public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof (Command), typeof (ICommand), typeof (DropDownButton), new PropertyMetadata((object) null, new PropertyChangedCallback(DropDownButton.OnCommandChanged)));
  public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof (CommandParameter), typeof (object), typeof (DropDownButton), new PropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(nameof (CommandTarget), typeof (IInputElement), typeof (DropDownButton), new PropertyMetadata((PropertyChangedCallback) null));

  static DropDownButton()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DropDownButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DropDownButton)));
    EventManager.RegisterClassHandler(typeof (DropDownButton), AccessKeyManager.AccessKeyPressedEvent, (Delegate) new AccessKeyPressedEventHandler(DropDownButton.OnAccessKeyPressed));
  }

  public DropDownButton()
  {
    Keyboard.AddKeyDownHandler((DependencyObject) this, new KeyEventHandler(this.OnKeyDown));
    Mouse.AddPreviewMouseDownOutsideCapturedElementHandler((DependencyObject) this, new MouseButtonEventHandler(this.OnMouseDownOutsideCapturedElement));
  }

  protected ButtonBase Button
  {
    get => this._button;
    set
    {
      if (this._button != null)
        this._button.Click -= new RoutedEventHandler(this.DropDownButton_Click);
      this._button = value;
      if (this._button == null)
        return;
      this._button.Click += new RoutedEventHandler(this.DropDownButton_Click);
    }
  }

  public object DropDownContent
  {
    get => this.GetValue(DropDownButton.DropDownContentProperty);
    set => this.SetValue(DropDownButton.DropDownContentProperty, value);
  }

  private static void OnDropDownContentChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is DropDownButton dropDownButton))
      return;
    dropDownButton.OnDropDownContentChanged(e.OldValue, e.NewValue);
  }

  protected virtual void OnDropDownContentChanged(object oldValue, object newValue)
  {
  }

  public Brush DropDownContentBackground
  {
    get => (Brush) this.GetValue(DropDownButton.DropDownContentBackgroundProperty);
    set => this.SetValue(DropDownButton.DropDownContentBackgroundProperty, (object) value);
  }

  public PlacementMode DropDownPosition
  {
    get => (PlacementMode) this.GetValue(DropDownButton.DropDownPositionProperty);
    set => this.SetValue(DropDownButton.DropDownPositionProperty, (object) value);
  }

  public bool IsDefault
  {
    get => (bool) this.GetValue(DropDownButton.IsDefaultProperty);
    set => this.SetValue(DropDownButton.IsDefaultProperty, (object) value);
  }

  private static void OnIsDefaultChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is DropDownButton dropDownButton))
      return;
    dropDownButton.OnIsDefaultChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsDefaultChanged(bool oldValue, bool newValue)
  {
    if (newValue)
      AccessKeyManager.Register("\r", (IInputElement) this);
    else
      AccessKeyManager.Unregister("\r", (IInputElement) this);
  }

  public bool IsOpen
  {
    get => (bool) this.GetValue(DropDownButton.IsOpenProperty);
    set => this.SetValue(DropDownButton.IsOpenProperty, (object) value);
  }

  private static void OnIsOpenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is DropDownButton dropDownButton))
      return;
    dropDownButton.OnIsOpenChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsOpenChanged(bool oldValue, bool newValue)
  {
    if (newValue)
      this.RaiseRoutedEvent(DropDownButton.OpenedEvent);
    else
      this.RaiseRoutedEvent(DropDownButton.ClosedEvent);
  }

  public double MaxDropDownHeight
  {
    get => (double) this.GetValue(DropDownButton.MaxDropDownHeightProperty);
    set => this.SetValue(DropDownButton.MaxDropDownHeightProperty, (object) value);
  }

  private static void OnMaxDropDownHeightChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is DropDownButton dropDownButton))
      return;
    dropDownButton.OnMaxDropDownHeightChanged((double) e.OldValue, (double) e.NewValue);
  }

  protected virtual void OnMaxDropDownHeightChanged(double oldValue, double newValue)
  {
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this.Button = (ButtonBase) (this.GetTemplateChild("PART_DropDownButton") as ToggleButton);
    this._contentPresenter = this.GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
    if (this._popup != null)
      this._popup.Opened -= new EventHandler(this.Popup_Opened);
    this._popup = this.GetTemplateChild("PART_Popup") as Popup;
    if (this._popup == null)
      return;
    this._popup.Opened += new EventHandler(this.Popup_Opened);
  }

  protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
  {
    base.OnIsKeyboardFocusWithinChanged(e);
    if ((bool) e.NewValue)
      return;
    this.CloseDropDown(false);
  }

  protected override void OnGotFocus(RoutedEventArgs e)
  {
    base.OnGotFocus(e);
    if (this.Button == null)
      return;
    this.Button.Focus();
  }

  protected override void OnAccessKey(AccessKeyEventArgs e)
  {
    if (e.IsMultiple)
      base.OnAccessKey(e);
    else
      this.OnClick();
  }

  public event RoutedEventHandler Click
  {
    add => this.AddHandler(DropDownButton.ClickEvent, (Delegate) value);
    remove => this.RemoveHandler(DropDownButton.ClickEvent, (Delegate) value);
  }

  public event RoutedEventHandler Opened
  {
    add => this.AddHandler(DropDownButton.OpenedEvent, (Delegate) value);
    remove => this.RemoveHandler(DropDownButton.OpenedEvent, (Delegate) value);
  }

  public event RoutedEventHandler Closed
  {
    add => this.AddHandler(DropDownButton.ClosedEvent, (Delegate) value);
    remove => this.RemoveHandler(DropDownButton.ClosedEvent, (Delegate) value);
  }

  private static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
  {
    if (e.Handled || e.Scope != null || e.Target != null)
      return;
    e.Target = (UIElement) (sender as DropDownButton);
  }

  private void OnKeyDown(object sender, KeyEventArgs e)
  {
    if (!this.IsOpen)
    {
      if (!KeyboardUtilities.IsKeyModifyingPopupState(e))
        return;
      this.IsOpen = true;
      e.Handled = true;
    }
    else if (KeyboardUtilities.IsKeyModifyingPopupState(e))
    {
      this.CloseDropDown(true);
      e.Handled = true;
    }
    else
    {
      if (e.Key != Key.Escape)
        return;
      this.CloseDropDown(true);
      e.Handled = true;
    }
  }

  private void OnMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
  {
    this.CloseDropDown(true);
  }

  private void DropDownButton_Click(object sender, RoutedEventArgs e) => this.OnClick();

  private void CanExecuteChanged(object sender, EventArgs e) => this.CanExecuteChanged();

  private void Popup_Opened(object sender, EventArgs e)
  {
    if (this._contentPresenter == null)
      return;
    this._contentPresenter.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
  }

  private void CanExecuteChanged()
  {
    if (this.Command == null)
      return;
    if (this.Command is RoutedCommand command)
      this.IsEnabled = command.CanExecute(this.CommandParameter, this.CommandTarget);
    else
      this.IsEnabled = this.Command.CanExecute(this.CommandParameter);
  }

  private void CloseDropDown(bool isFocusOnButton)
  {
    if (this.IsOpen)
      this.IsOpen = false;
    this.ReleaseMouseCapture();
    if (!isFocusOnButton || this.Button == null)
      return;
    this.Button.Focus();
  }

  protected virtual void OnClick()
  {
    this.RaiseRoutedEvent(DropDownButton.ClickEvent);
    this.RaiseCommand();
  }

  private void RaiseRoutedEvent(RoutedEvent routedEvent)
  {
    this.RaiseEvent(new RoutedEventArgs(routedEvent, (object) this));
  }

  private void RaiseCommand()
  {
    if (this.Command == null)
      return;
    if (!(this.Command is RoutedCommand command))
      this.Command.Execute(this.CommandParameter);
    else
      command.Execute(this.CommandParameter, this.CommandTarget);
  }

  private void UnhookCommand(ICommand oldCommand, ICommand newCommand)
  {
    EventHandler eventHandler = new EventHandler(this.CanExecuteChanged);
    oldCommand.CanExecuteChanged -= eventHandler;
  }

  private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
  {
    this.canExecuteChangedHandler = new EventHandler(this.CanExecuteChanged);
    if (newCommand == null)
      return;
    newCommand.CanExecuteChanged += this.canExecuteChangedHandler;
  }

  [TypeConverter(typeof (CommandConverter))]
  public ICommand Command
  {
    get => (ICommand) this.GetValue(DropDownButton.CommandProperty);
    set => this.SetValue(DropDownButton.CommandProperty, (object) value);
  }

  private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (!(d is DropDownButton dropDownButton))
      return;
    dropDownButton.OnCommandChanged((ICommand) e.OldValue, (ICommand) e.NewValue);
  }

  protected virtual void OnCommandChanged(ICommand oldValue, ICommand newValue)
  {
    if (oldValue != null)
      this.UnhookCommand(oldValue, newValue);
    this.HookUpCommand(oldValue, newValue);
    this.CanExecuteChanged();
  }

  public object CommandParameter
  {
    get => this.GetValue(DropDownButton.CommandParameterProperty);
    set => this.SetValue(DropDownButton.CommandParameterProperty, value);
  }

  public IInputElement CommandTarget
  {
    get => (IInputElement) this.GetValue(DropDownButton.CommandTargetProperty);
    set => this.SetValue(DropDownButton.CommandTargetProperty, (object) value);
  }
}
