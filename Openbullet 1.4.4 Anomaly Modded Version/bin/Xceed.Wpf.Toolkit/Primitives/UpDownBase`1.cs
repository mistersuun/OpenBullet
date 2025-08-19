// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Primitives.UpDownBase`1
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit.Core.Input;

#nullable disable
namespace Xceed.Wpf.Toolkit.Primitives;

[TemplatePart(Name = "PART_TextBox", Type = typeof (TextBox))]
[TemplatePart(Name = "PART_Spinner", Type = typeof (Spinner))]
public abstract class UpDownBase<T> : InputBase, IValidateInput
{
  internal const string PART_TextBox = "PART_TextBox";
  internal const string PART_Spinner = "PART_Spinner";
  internal bool _isTextChangedFromUI;
  private bool _isSyncingTextAndValueProperties;
  private bool _internalValueSet;
  public static readonly DependencyProperty AllowSpinProperty = DependencyProperty.Register(nameof (AllowSpin), typeof (bool), typeof (UpDownBase<T>), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty ButtonSpinnerLocationProperty = DependencyProperty.Register(nameof (ButtonSpinnerLocation), typeof (Location), typeof (UpDownBase<T>), (PropertyMetadata) new UIPropertyMetadata((object) Location.Right));
  public static readonly DependencyProperty ClipValueToMinMaxProperty = DependencyProperty.Register(nameof (ClipValueToMinMax), typeof (bool), typeof (UpDownBase<T>), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty DisplayDefaultValueOnEmptyTextProperty = DependencyProperty.Register(nameof (DisplayDefaultValueOnEmptyText), typeof (bool), typeof (UpDownBase<T>), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(UpDownBase<T>.OnDisplayDefaultValueOnEmptyTextChanged)));
  public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register(nameof (DefaultValue), typeof (T), typeof (UpDownBase<T>), (PropertyMetadata) new UIPropertyMetadata((object) default (T), new PropertyChangedCallback(UpDownBase<T>.OnDefaultValueChanged)));
  public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof (Maximum), typeof (T), typeof (UpDownBase<T>), (PropertyMetadata) new UIPropertyMetadata((object) default (T), new PropertyChangedCallback(UpDownBase<T>.OnMaximumChanged), new CoerceValueCallback(UpDownBase<T>.OnCoerceMaximum)));
  public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof (Minimum), typeof (T), typeof (UpDownBase<T>), (PropertyMetadata) new UIPropertyMetadata((object) default (T), new PropertyChangedCallback(UpDownBase<T>.OnMinimumChanged), new CoerceValueCallback(UpDownBase<T>.OnCoerceMinimum)));
  public static readonly DependencyProperty MouseWheelActiveTriggerProperty = DependencyProperty.Register(nameof (MouseWheelActiveTrigger), typeof (MouseWheelActiveTrigger), typeof (UpDownBase<T>), (PropertyMetadata) new UIPropertyMetadata((object) MouseWheelActiveTrigger.FocusedMouseOver));
  [Obsolete("Use MouseWheelActiveTrigger property instead")]
  public static readonly DependencyProperty MouseWheelActiveOnFocusProperty = DependencyProperty.Register(nameof (MouseWheelActiveOnFocus), typeof (bool), typeof (UpDownBase<T>), (PropertyMetadata) new UIPropertyMetadata((object) true, new PropertyChangedCallback(UpDownBase<T>.OnMouseWheelActiveOnFocusChanged)));
  public static readonly DependencyProperty ShowButtonSpinnerProperty = DependencyProperty.Register(nameof (ShowButtonSpinner), typeof (bool), typeof (UpDownBase<T>), (PropertyMetadata) new UIPropertyMetadata((object) true));
  public static readonly DependencyProperty UpdateValueOnEnterKeyProperty = DependencyProperty.Register(nameof (UpdateValueOnEnterKey), typeof (bool), typeof (UpDownBase<T>), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(UpDownBase<T>.OnUpdateValueOnEnterKeyChanged)));
  public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (T), typeof (UpDownBase<T>), (PropertyMetadata) new FrameworkPropertyMetadata((object) default (T), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(UpDownBase<T>.OnValueChanged), new CoerceValueCallback(UpDownBase<T>.OnCoerceValue), false, UpdateSourceTrigger.PropertyChanged));
  public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof (RoutedPropertyChangedEventHandler<object>), typeof (UpDownBase<T>));

  protected Spinner Spinner { get; private set; }

  protected TextBox TextBox { get; private set; }

  public bool AllowSpin
  {
    get => (bool) this.GetValue(UpDownBase<T>.AllowSpinProperty);
    set => this.SetValue(UpDownBase<T>.AllowSpinProperty, (object) value);
  }

  public Location ButtonSpinnerLocation
  {
    get => (Location) this.GetValue(UpDownBase<T>.ButtonSpinnerLocationProperty);
    set => this.SetValue(UpDownBase<T>.ButtonSpinnerLocationProperty, (object) value);
  }

  public bool ClipValueToMinMax
  {
    get => (bool) this.GetValue(UpDownBase<T>.ClipValueToMinMaxProperty);
    set => this.SetValue(UpDownBase<T>.ClipValueToMinMaxProperty, (object) value);
  }

  public bool DisplayDefaultValueOnEmptyText
  {
    get => (bool) this.GetValue(UpDownBase<T>.DisplayDefaultValueOnEmptyTextProperty);
    set => this.SetValue(UpDownBase<T>.DisplayDefaultValueOnEmptyTextProperty, (object) value);
  }

  private static void OnDisplayDefaultValueOnEmptyTextChanged(
    DependencyObject source,
    DependencyPropertyChangedEventArgs args)
  {
    ((UpDownBase<T>) source).OnDisplayDefaultValueOnEmptyTextChanged((bool) args.OldValue, (bool) args.NewValue);
  }

  private void OnDisplayDefaultValueOnEmptyTextChanged(bool oldValue, bool newValue)
  {
    if (!this.IsInitialized || !string.IsNullOrEmpty(this.Text))
      return;
    this.SyncTextAndValueProperties(false, this.Text);
  }

  public T DefaultValue
  {
    get => (T) this.GetValue(UpDownBase<T>.DefaultValueProperty);
    set => this.SetValue(UpDownBase<T>.DefaultValueProperty, (object) value);
  }

  private static void OnDefaultValueChanged(
    DependencyObject source,
    DependencyPropertyChangedEventArgs args)
  {
    ((UpDownBase<T>) source).OnDefaultValueChanged((T) args.OldValue, (T) args.NewValue);
  }

  private void OnDefaultValueChanged(T oldValue, T newValue)
  {
    if (!this.IsInitialized || !string.IsNullOrEmpty(this.Text))
      return;
    this.SyncTextAndValueProperties(true, this.Text);
  }

  public T Maximum
  {
    get => (T) this.GetValue(UpDownBase<T>.MaximumProperty);
    set => this.SetValue(UpDownBase<T>.MaximumProperty, (object) value);
  }

  private static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is UpDownBase<T> upDownBase))
      return;
    upDownBase.OnMaximumChanged((T) e.OldValue, (T) e.NewValue);
  }

  protected virtual void OnMaximumChanged(T oldValue, T newValue)
  {
    if (!this.IsInitialized)
      return;
    this.SetValidSpinDirection();
  }

  private static object OnCoerceMaximum(DependencyObject d, object baseValue)
  {
    return d is UpDownBase<T> upDownBase ? (object) upDownBase.OnCoerceMaximum((T) baseValue) : baseValue;
  }

  protected virtual T OnCoerceMaximum(T baseValue) => baseValue;

  public T Minimum
  {
    get => (T) this.GetValue(UpDownBase<T>.MinimumProperty);
    set => this.SetValue(UpDownBase<T>.MinimumProperty, (object) value);
  }

  private static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is UpDownBase<T> upDownBase))
      return;
    upDownBase.OnMinimumChanged((T) e.OldValue, (T) e.NewValue);
  }

  protected virtual void OnMinimumChanged(T oldValue, T newValue)
  {
    if (!this.IsInitialized)
      return;
    this.SetValidSpinDirection();
  }

  private static object OnCoerceMinimum(DependencyObject d, object baseValue)
  {
    return d is UpDownBase<T> upDownBase ? (object) upDownBase.OnCoerceMinimum((T) baseValue) : baseValue;
  }

  protected virtual T OnCoerceMinimum(T baseValue) => baseValue;

  public MouseWheelActiveTrigger MouseWheelActiveTrigger
  {
    get => (MouseWheelActiveTrigger) this.GetValue(UpDownBase<T>.MouseWheelActiveTriggerProperty);
    set => this.SetValue(UpDownBase<T>.MouseWheelActiveTriggerProperty, (object) value);
  }

  [Obsolete("Use MouseWheelActiveTrigger property instead")]
  public bool MouseWheelActiveOnFocus
  {
    get => (bool) this.GetValue(UpDownBase<T>.MouseWheelActiveOnFocusProperty);
    set => this.SetValue(UpDownBase<T>.MouseWheelActiveOnFocusProperty, (object) value);
  }

  private static void OnMouseWheelActiveOnFocusChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is UpDownBase<T> upDownBase))
      return;
    int num = (bool) e.NewValue ? 1 : 2;
    upDownBase.MouseWheelActiveTrigger = (MouseWheelActiveTrigger) num;
  }

  public bool ShowButtonSpinner
  {
    get => (bool) this.GetValue(UpDownBase<T>.ShowButtonSpinnerProperty);
    set => this.SetValue(UpDownBase<T>.ShowButtonSpinnerProperty, (object) value);
  }

  public bool UpdateValueOnEnterKey
  {
    get => (bool) this.GetValue(UpDownBase<T>.UpdateValueOnEnterKeyProperty);
    set => this.SetValue(UpDownBase<T>.UpdateValueOnEnterKeyProperty, (object) value);
  }

  private static void OnUpdateValueOnEnterKeyChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is UpDownBase<T> upDownBase))
      return;
    upDownBase.OnUpdateValueOnEnterKeyChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnUpdateValueOnEnterKeyChanged(bool oldValue, bool newValue)
  {
  }

  public T Value
  {
    get => (T) this.GetValue(UpDownBase<T>.ValueProperty);
    set => this.SetValue(UpDownBase<T>.ValueProperty, (object) value);
  }

  private void SetValueInternal(T value)
  {
    this._internalValueSet = true;
    try
    {
      this.Value = value;
    }
    finally
    {
      this._internalValueSet = false;
    }
  }

  private static object OnCoerceValue(DependencyObject o, object basevalue)
  {
    return ((UpDownBase<T>) o).OnCoerceValue(basevalue);
  }

  protected virtual object OnCoerceValue(object newValue) => newValue;

  private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is UpDownBase<T> upDownBase))
      return;
    upDownBase.OnValueChanged((T) e.OldValue, (T) e.NewValue);
  }

  protected virtual void OnValueChanged(T oldValue, T newValue)
  {
    if (!this._internalValueSet && this.IsInitialized)
      this.SyncTextAndValueProperties(false, (string) null, true);
    this.SetValidSpinDirection();
    this.RaiseValueChangedEvent(oldValue, newValue);
  }

  internal UpDownBase()
  {
    this.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, (Delegate) new RoutedEventHandler(this.HandleClickOutsideOfControlWithMouseCapture), true);
    this.IsKeyboardFocusWithinChanged += new DependencyPropertyChangedEventHandler(this.UpDownBase_IsKeyboardFocusWithinChanged);
  }

  protected override void OnAccessKey(AccessKeyEventArgs e)
  {
    if (this.TextBox != null)
      this.TextBox.Focus();
    base.OnAccessKey(e);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (this.TextBox != null)
    {
      this.TextBox.TextChanged -= new TextChangedEventHandler(this.TextBox_TextChanged);
      this.TextBox.RemoveHandler(Mouse.PreviewMouseDownEvent, (Delegate) new MouseButtonEventHandler(this.TextBox_PreviewMouseDown));
    }
    this.TextBox = this.GetTemplateChild("PART_TextBox") as TextBox;
    if (this.TextBox != null)
    {
      this.TextBox.Text = this.Text;
      this.TextBox.TextChanged += new TextChangedEventHandler(this.TextBox_TextChanged);
      this.TextBox.AddHandler(Mouse.PreviewMouseDownEvent, (Delegate) new MouseButtonEventHandler(this.TextBox_PreviewMouseDown), true);
    }
    if (this.Spinner != null)
      this.Spinner.Spin -= new EventHandler<SpinEventArgs>(this.OnSpinnerSpin);
    this.Spinner = this.GetTemplateChild("PART_Spinner") as Spinner;
    if (this.Spinner != null)
      this.Spinner.Spin += new EventHandler<SpinEventArgs>(this.OnSpinnerSpin);
    this.SetValidSpinDirection();
  }

  protected override void OnKeyDown(KeyEventArgs e)
  {
    if (e.Key != Key.Return)
      return;
    bool flag = this.CommitInput();
    e.Handled = !flag;
  }

  protected override void OnTextChanged(string oldValue, string newValue)
  {
    if (!this.IsInitialized)
      return;
    if (this.UpdateValueOnEnterKey)
    {
      if (this._isTextChangedFromUI)
        return;
      this.SyncTextAndValueProperties(true, this.Text);
    }
    else
      this.SyncTextAndValueProperties(true, this.Text);
  }

  protected override void OnCultureInfoChanged(CultureInfo oldValue, CultureInfo newValue)
  {
    if (!this.IsInitialized)
      return;
    this.SyncTextAndValueProperties(false, (string) null);
  }

  protected override void OnReadOnlyChanged(bool oldValue, bool newValue)
  {
    this.SetValidSpinDirection();
  }

  private void TextBox_PreviewMouseDown(object sender, RoutedEventArgs e)
  {
    if (this.MouseWheelActiveTrigger != MouseWheelActiveTrigger.Focused || Mouse.Captured == this.Spinner)
      return;
    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Delegate) (() => Mouse.Capture((IInputElement) this.Spinner)));
  }

  private void HandleClickOutsideOfControlWithMouseCapture(object sender, RoutedEventArgs e)
  {
    if (!(Mouse.Captured is Spinner))
      return;
    this.Spinner.ReleaseMouseCapture();
  }

  private void OnSpinnerSpin(object sender, SpinEventArgs e)
  {
    if (!this.AllowSpin || this.IsReadOnly)
      return;
    MouseWheelActiveTrigger wheelActiveTrigger = this.MouseWheelActiveTrigger;
    if (((!e.UsingMouseWheel | wheelActiveTrigger == MouseWheelActiveTrigger.MouseOver ? 1 : 0) | (this.TextBox == null || !this.TextBox.IsFocused ? 0 : (wheelActiveTrigger == MouseWheelActiveTrigger.FocusedMouseOver ? 1 : 0)) | (this.TextBox == null || !this.TextBox.IsFocused || wheelActiveTrigger != MouseWheelActiveTrigger.Focused ? 0 : (Mouse.Captured is Spinner ? 1 : 0))) == 0)
      return;
    e.Handled = true;
    this.OnSpin(e);
  }

  public event InputValidationErrorEventHandler InputValidationError;

  public event EventHandler<SpinEventArgs> Spinned;

  public event RoutedPropertyChangedEventHandler<object> ValueChanged
  {
    add => this.AddHandler(UpDownBase<T>.ValueChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(UpDownBase<T>.ValueChangedEvent, (Delegate) value);
  }

  protected virtual void OnSpin(SpinEventArgs e)
  {
    if (e == null)
      throw new ArgumentNullException(nameof (e));
    EventHandler<SpinEventArgs> spinned = this.Spinned;
    if (spinned != null)
      spinned((object) this, e);
    if (e.Direction == SpinDirection.Increase)
      this.DoIncrement();
    else
      this.DoDecrement();
  }

  protected virtual void RaiseValueChangedEvent(T oldValue, T newValue)
  {
    RoutedPropertyChangedEventArgs<object> e = new RoutedPropertyChangedEventArgs<object>((object) oldValue, (object) newValue);
    e.RoutedEvent = UpDownBase<T>.ValueChangedEvent;
    this.RaiseEvent((RoutedEventArgs) e);
  }

  protected override void OnInitialized(EventArgs e)
  {
    base.OnInitialized(e);
    bool updateValueFromText = this.ReadLocalValue(UpDownBase<T>.ValueProperty) == DependencyProperty.UnsetValue && BindingOperations.GetBinding((DependencyObject) this, UpDownBase<T>.ValueProperty) == null && object.Equals((object) this.Value, UpDownBase<T>.ValueProperty.DefaultMetadata.DefaultValue);
    this.SyncTextAndValueProperties(updateValueFromText, this.Text, !updateValueFromText);
  }

  internal void DoDecrement()
  {
    if (this.Spinner != null && (this.Spinner.ValidSpinDirection & ValidSpinDirections.Decrease) != ValidSpinDirections.Decrease)
      return;
    this.OnDecrement();
  }

  internal void DoIncrement()
  {
    if (this.Spinner != null && (this.Spinner.ValidSpinDirection & ValidSpinDirections.Increase) != ValidSpinDirections.Increase)
      return;
    this.OnIncrement();
  }

  private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
  {
    if (!this.IsKeyboardFocusWithin)
      return;
    try
    {
      this._isTextChangedFromUI = true;
      this.Text = ((TextBox) sender).Text;
    }
    finally
    {
      this._isTextChangedFromUI = false;
    }
  }

  private void UpDownBase_IsKeyboardFocusWithinChanged(
    object sender,
    DependencyPropertyChangedEventArgs e)
  {
    if ((bool) e.NewValue)
      return;
    this.CommitInput();
  }

  private void RaiseInputValidationError(Exception e)
  {
    if (this.InputValidationError == null)
      return;
    InputValidationErrorEventArgs e1 = new InputValidationErrorEventArgs(e);
    this.InputValidationError((object) this, e1);
    if (e1.ThrowException)
      throw e1.Exception;
  }

  public virtual bool CommitInput() => this.SyncTextAndValueProperties(true, this.Text);

  protected bool SyncTextAndValueProperties(bool updateValueFromText, string text)
  {
    return this.SyncTextAndValueProperties(updateValueFromText, text, false);
  }

  private bool SyncTextAndValueProperties(
    bool updateValueFromText,
    string text,
    bool forceTextUpdate)
  {
    if (this._isSyncingTextAndValueProperties)
      return true;
    this._isSyncingTextAndValueProperties = true;
    bool flag = true;
    try
    {
      if (updateValueFromText)
      {
        if (string.IsNullOrEmpty(text))
        {
          this.SetValueInternal(this.DefaultValue);
        }
        else
        {
          try
          {
            T objA = this.ConvertTextToValue(text);
            if (!object.Equals((object) objA, (object) this.Value))
              this.SetValueInternal(objA);
          }
          catch (Exception ex)
          {
            flag = false;
            if (!this._isTextChangedFromUI)
              this.RaiseInputValidationError(ex);
          }
        }
      }
      if (!this._isTextChangedFromUI)
      {
        if ((forceTextUpdate || !string.IsNullOrEmpty(this.Text) || !object.Equals((object) this.Value, (object) this.DefaultValue) ? 0 : (!this.DisplayDefaultValueOnEmptyText ? 1 : 0)) == 0)
        {
          string text1 = this.ConvertValueToText();
          if (!object.Equals((object) this.Text, (object) text1))
            this.Text = text1;
        }
        if (this.TextBox != null)
          this.TextBox.Text = this.Text;
      }
      if (this._isTextChangedFromUI && !flag)
      {
        if (this.Spinner != null)
          this.Spinner.ValidSpinDirection = ValidSpinDirections.None;
      }
      else
        this.SetValidSpinDirection();
    }
    finally
    {
      this._isSyncingTextAndValueProperties = false;
    }
    return flag;
  }

  protected abstract T ConvertTextToValue(string text);

  protected abstract string ConvertValueToText();

  protected abstract void OnIncrement();

  protected abstract void OnDecrement();

  protected abstract void SetValidSpinDirection();
}
