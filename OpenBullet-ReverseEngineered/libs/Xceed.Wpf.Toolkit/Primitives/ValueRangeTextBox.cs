// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Primitives.ValueRangeTextBox
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Primitives;

public class ValueRangeTextBox : AutoSelectTextBox
{
  public static readonly DependencyProperty BeepOnErrorProperty = DependencyProperty.Register(nameof (BeepOnError), typeof (bool), typeof (ValueRangeTextBox), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty FormatProviderProperty = DependencyProperty.Register(nameof (FormatProvider), typeof (IFormatProvider), typeof (ValueRangeTextBox), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(ValueRangeTextBox.FormatProviderPropertyChangedCallback)));
  public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof (MinValue), typeof (object), typeof (ValueRangeTextBox), (PropertyMetadata) new UIPropertyMetadata((object) null, (PropertyChangedCallback) null, new CoerceValueCallback(ValueRangeTextBox.MinValueCoerceValueCallback)));
  public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof (MaxValue), typeof (object), typeof (ValueRangeTextBox), (PropertyMetadata) new UIPropertyMetadata((object) null, (PropertyChangedCallback) null, new CoerceValueCallback(ValueRangeTextBox.MaxValueCoerceValueCallback)));
  public static readonly DependencyProperty NullValueProperty = DependencyProperty.Register(nameof (NullValue), typeof (object), typeof (ValueRangeTextBox), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(ValueRangeTextBox.NullValuePropertyChangedCallback), new CoerceValueCallback(ValueRangeTextBox.NullValueCoerceValueCallback)));
  public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof (Value), typeof (object), typeof (ValueRangeTextBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ValueRangeTextBox.ValuePropertyChangedCallback), new CoerceValueCallback(ValueRangeTextBox.ValueCoerceValueCallback)));
  public static readonly DependencyProperty ValueDataTypeProperty = DependencyProperty.Register(nameof (ValueDataType), typeof (Type), typeof (ValueRangeTextBox), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(ValueRangeTextBox.ValueDataTypePropertyChangedCallback), new CoerceValueCallback(ValueRangeTextBox.ValueDataTypeCoerceValueCallback)));
  private static readonly DependencyPropertyKey HasValidationErrorPropertyKey = DependencyProperty.RegisterReadOnly(nameof (HasValidationError), typeof (bool), typeof (ValueRangeTextBox), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty HasValidationErrorProperty = ValueRangeTextBox.HasValidationErrorPropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey HasParsingErrorPropertyKey = DependencyProperty.RegisterReadOnly(nameof (HasParsingError), typeof (bool), typeof (ValueRangeTextBox), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty HasParsingErrorProperty = ValueRangeTextBox.HasParsingErrorPropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey IsValueOutOfRangePropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsValueOutOfRange), typeof (bool), typeof (ValueRangeTextBox), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty IsValueOutOfRangeProperty = ValueRangeTextBox.IsValueOutOfRangePropertyKey.DependencyProperty;
  private BitVector32 m_flags;
  private CachedTextInfo m_imePreCompositionCachedTextInfo;

  static ValueRangeTextBox()
  {
    TextBox.TextProperty.OverrideMetadata(typeof (ValueRangeTextBox), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(ValueRangeTextBox.TextCoerceValueCallback)));
    TextBoxBase.AcceptsReturnProperty.OverrideMetadata(typeof (ValueRangeTextBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, (PropertyChangedCallback) null, new CoerceValueCallback(ValueRangeTextBox.AcceptsReturnCoerceValueCallback)));
    TextBoxBase.AcceptsTabProperty.OverrideMetadata(typeof (ValueRangeTextBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, (PropertyChangedCallback) null, new CoerceValueCallback(ValueRangeTextBox.AcceptsTabCoerceValueCallback)));
  }

  private static object AcceptsReturnCoerceValueCallback(DependencyObject sender, object value)
  {
    if ((bool) value)
      throw new NotSupportedException("The ValueRangeTextBox does not support the AcceptsReturn property.");
    return (object) false;
  }

  private static object AcceptsTabCoerceValueCallback(DependencyObject sender, object value)
  {
    if ((bool) value)
      throw new NotSupportedException("The ValueRangeTextBox does not support the AcceptsTab property.");
    return (object) false;
  }

  public bool BeepOnError
  {
    get => (bool) this.GetValue(ValueRangeTextBox.BeepOnErrorProperty);
    set => this.SetValue(ValueRangeTextBox.BeepOnErrorProperty, (object) value);
  }

  public IFormatProvider FormatProvider
  {
    get => (IFormatProvider) this.GetValue(ValueRangeTextBox.FormatProviderProperty);
    set => this.SetValue(ValueRangeTextBox.FormatProviderProperty, (object) value);
  }

  private static void FormatProviderPropertyChangedCallback(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs e)
  {
    ValueRangeTextBox valueRangeTextBox = (ValueRangeTextBox) sender;
    if (!valueRangeTextBox.IsInitialized)
      return;
    valueRangeTextBox.OnFormatProviderChanged();
  }

  internal virtual void OnFormatProviderChanged()
  {
    this.RefreshConversionHelpers();
    this.RefreshCurrentText(false);
    this.RefreshValue();
  }

  public object MinValue
  {
    get => this.GetValue(ValueRangeTextBox.MinValueProperty);
    set => this.SetValue(ValueRangeTextBox.MinValueProperty, value);
  }

  private static object MinValueCoerceValueCallback(DependencyObject sender, object value)
  {
    ValueRangeTextBox valueRangeTextBox = sender as ValueRangeTextBox;
    if (!valueRangeTextBox.IsInitialized)
      return DependencyProperty.UnsetValue;
    if (value == null)
      return value;
    Type valueDataType = valueRangeTextBox.ValueDataType;
    if (valueDataType == (Type) null)
      throw new InvalidOperationException("An attempt was made to set a minimum value when the ValueDataType property is null.");
    if (valueRangeTextBox.IsFinalizingInitialization)
      value = ValueRangeTextBox.ConvertValueToDataType(value, valueRangeTextBox.ValueDataType);
    if (value.GetType() != valueDataType)
      throw new ArgumentException($"The value is not of type {valueDataType.Name}.", "MinValue");
    if (!(value is IComparable))
      throw new InvalidOperationException("MinValue does not implement the IComparable interface.");
    object maxValue = valueRangeTextBox.MaxValue;
    valueRangeTextBox.ValidateValueInRange(value, maxValue, valueRangeTextBox.Value);
    return value;
  }

  public object MaxValue
  {
    get => this.GetValue(ValueRangeTextBox.MaxValueProperty);
    set => this.SetValue(ValueRangeTextBox.MaxValueProperty, value);
  }

  private static object MaxValueCoerceValueCallback(DependencyObject sender, object value)
  {
    ValueRangeTextBox valueRangeTextBox = sender as ValueRangeTextBox;
    if (!valueRangeTextBox.IsInitialized)
      return DependencyProperty.UnsetValue;
    if (value == null)
      return value;
    Type valueDataType = valueRangeTextBox.ValueDataType;
    if (valueDataType == (Type) null)
      throw new InvalidOperationException("An attempt was made to set a maximum value when the ValueDataType property is null.");
    if (valueRangeTextBox.IsFinalizingInitialization)
      value = ValueRangeTextBox.ConvertValueToDataType(value, valueRangeTextBox.ValueDataType);
    if (value.GetType() != valueDataType)
      throw new ArgumentException($"The value is not of type {valueDataType.Name}.", "MinValue");
    if (!(value is IComparable))
      throw new InvalidOperationException("MaxValue does not implement the IComparable interface.");
    object minValue = valueRangeTextBox.MinValue;
    valueRangeTextBox.ValidateValueInRange(minValue, value, valueRangeTextBox.Value);
    return value;
  }

  public object NullValue
  {
    get => this.GetValue(ValueRangeTextBox.NullValueProperty);
    set => this.SetValue(ValueRangeTextBox.NullValueProperty, value);
  }

  private static object NullValueCoerceValueCallback(DependencyObject sender, object value)
  {
    ValueRangeTextBox valueRangeTextBox = sender as ValueRangeTextBox;
    if (!valueRangeTextBox.IsInitialized)
      return DependencyProperty.UnsetValue;
    if (value == null || value == DBNull.Value)
      return value;
    Type valueDataType = valueRangeTextBox.ValueDataType;
    if (valueDataType == (Type) null)
      throw new InvalidOperationException("An attempt was made to set a null value when the ValueDataType property is null.");
    if (valueRangeTextBox.IsFinalizingInitialization)
      value = ValueRangeTextBox.ConvertValueToDataType(value, valueRangeTextBox.ValueDataType);
    return !(value.GetType() != valueDataType) ? value : throw new ArgumentException($"The value is not of type {valueDataType.Name}.", "NullValue");
  }

  private static void NullValuePropertyChangedCallback(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs e)
  {
    ValueRangeTextBox valueRangeTextBox = sender as ValueRangeTextBox;
    if (e.OldValue == null)
    {
      if (valueRangeTextBox.Value != null)
        return;
      valueRangeTextBox.RefreshValue();
    }
    else
    {
      if (!e.OldValue.Equals(valueRangeTextBox.Value))
        return;
      valueRangeTextBox.RefreshValue();
    }
  }

  public object Value
  {
    get => this.GetValue(ValueRangeTextBox.ValueProperty);
    set => this.SetValue(ValueRangeTextBox.ValueProperty, value);
  }

  private static object ValueCoerceValueCallback(object sender, object value)
  {
    ValueRangeTextBox valueRangeTextBox = sender as ValueRangeTextBox;
    if (!valueRangeTextBox.IsInitialized)
      return DependencyProperty.UnsetValue;
    if (valueRangeTextBox.IsFinalizingInitialization)
      value = ValueRangeTextBox.ConvertValueToDataType(value, valueRangeTextBox.ValueDataType);
    if (!valueRangeTextBox.IsForcingValue)
      valueRangeTextBox.ValidateValue(value);
    return value;
  }

  private static void ValuePropertyChangedCallback(
    object sender,
    DependencyPropertyChangedEventArgs e)
  {
    ValueRangeTextBox valueRangeTextBox = sender as ValueRangeTextBox;
    if (valueRangeTextBox.IsForcingValue || object.Equals(e.NewValue, e.OldValue))
      return;
    valueRangeTextBox.IsInValueChanged = true;
    try
    {
      valueRangeTextBox.Text = valueRangeTextBox.GetTextFromValue(e.NewValue);
    }
    finally
    {
      valueRangeTextBox.IsInValueChanged = false;
    }
  }

  public Type ValueDataType
  {
    get => (Type) this.GetValue(ValueRangeTextBox.ValueDataTypeProperty);
    set => this.SetValue(ValueRangeTextBox.ValueDataTypeProperty, (object) value);
  }

  private static object ValueDataTypeCoerceValueCallback(DependencyObject sender, object value)
  {
    ValueRangeTextBox valueRangeTextBox = sender as ValueRangeTextBox;
    if (!valueRangeTextBox.IsInitialized)
      return DependencyProperty.UnsetValue;
    Type type = value as Type;
    try
    {
      valueRangeTextBox.ValidateDataType(type);
    }
    catch (Exception ex)
    {
      throw new ArgumentException("An error occured while trying to change the ValueDataType.", ex);
    }
    return value;
  }

  private static void ValueDataTypePropertyChangedCallback(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs e)
  {
    ValueRangeTextBox valueRangeTextBox = sender as ValueRangeTextBox;
    Type newValue = e.NewValue as Type;
    valueRangeTextBox.IsNumericValueDataType = ValueRangeTextBox.IsNumericType(newValue);
    valueRangeTextBox.RefreshConversionHelpers();
    valueRangeTextBox.ConvertValuesToDataType(newValue);
  }

  internal virtual void ValidateDataType(Type type)
  {
    if (type == (Type) null)
      return;
    object obj1 = this.MinValue;
    if (obj1 != null && obj1.GetType() != type)
      obj1 = Convert.ChangeType(obj1, type, (IFormatProvider) CultureInfo.InvariantCulture);
    object obj2 = this.MaxValue;
    if (obj2 != null && obj2.GetType() != type)
      obj2 = Convert.ChangeType(obj2, type, (IFormatProvider) CultureInfo.InvariantCulture);
    object obj3 = this.NullValue;
    if (obj3 != null && obj3 != DBNull.Value && obj3.GetType() != type)
      obj3 = Convert.ChangeType(obj3, type, (IFormatProvider) CultureInfo.InvariantCulture);
    object obj4 = this.Value;
    if (obj4 != null && obj4 != DBNull.Value && obj4.GetType() != type)
      Convert.ChangeType(obj4, type, (IFormatProvider) CultureInfo.InvariantCulture);
    if ((obj1 != null || obj2 != null || obj3 != null && obj3 != DBNull.Value) && type.GetInterface("IComparable") == (Type) null)
      throw new InvalidOperationException("MinValue, MaxValue, and NullValue must implement the IComparable interface.");
  }

  private void ConvertValuesToDataType(Type type)
  {
    if (type == (Type) null)
    {
      this.MinValue = (object) null;
      this.MaxValue = (object) null;
      this.NullValue = (object) null;
      this.Value = (object) null;
    }
    else
    {
      object minValue = this.MinValue;
      if (minValue != null && minValue.GetType() != type)
        this.MinValue = ValueRangeTextBox.ConvertValueToDataType(minValue, type);
      object maxValue = this.MaxValue;
      if (maxValue != null && maxValue.GetType() != type)
        this.MaxValue = ValueRangeTextBox.ConvertValueToDataType(maxValue, type);
      object nullValue = this.NullValue;
      if (nullValue != null && nullValue != DBNull.Value && nullValue.GetType() != type)
        this.NullValue = ValueRangeTextBox.ConvertValueToDataType(nullValue, type);
      object obj = this.Value;
      if (obj == null || obj == DBNull.Value || !(obj.GetType() != type))
        return;
      this.Value = ValueRangeTextBox.ConvertValueToDataType(obj, type);
    }
  }

  private static object TextCoerceValueCallback(object sender, object value)
  {
    if (!(sender as ValueRangeTextBox).IsInitialized)
      return DependencyProperty.UnsetValue;
    return value == null ? (object) string.Empty : value;
  }

  protected override void OnTextChanged(TextChangedEventArgs e)
  {
    this.RefreshValue();
    base.OnTextChanged(e);
  }

  public bool HasValidationError
  {
    get => (bool) this.GetValue(ValueRangeTextBox.HasValidationErrorProperty);
  }

  private void SetHasValidationError(bool value)
  {
    this.SetValue(ValueRangeTextBox.HasValidationErrorPropertyKey, (object) value);
  }

  public bool HasParsingError => (bool) this.GetValue(ValueRangeTextBox.HasParsingErrorProperty);

  internal void SetHasParsingError(bool value)
  {
    this.SetValue(ValueRangeTextBox.HasParsingErrorPropertyKey, (object) value);
  }

  public bool IsValueOutOfRange
  {
    get => (bool) this.GetValue(ValueRangeTextBox.IsValueOutOfRangeProperty);
  }

  private void SetIsValueOutOfRange(bool value)
  {
    this.SetValue(ValueRangeTextBox.IsValueOutOfRangePropertyKey, (object) value);
  }

  internal bool IsInValueChanged
  {
    get => this.m_flags[8];
    private set => this.m_flags[8] = value;
  }

  internal bool IsForcingValue
  {
    get => this.m_flags[4];
    private set => this.m_flags[4] = value;
  }

  internal bool IsForcingText
  {
    get => this.m_flags[2];
    private set => this.m_flags[2] = value;
  }

  internal bool IsNumericValueDataType
  {
    get => this.m_flags[16 /*0x10*/];
    private set => this.m_flags[16 /*0x10*/] = value;
  }

  internal virtual bool IsTextReadyToBeParsed => true;

  internal bool IsInIMEComposition => this.m_imePreCompositionCachedTextInfo != null;

  private bool IsFinalizingInitialization
  {
    get => this.m_flags[1];
    set => this.m_flags[1] = value;
  }

  public event EventHandler<QueryTextFromValueEventArgs> QueryTextFromValue;

  internal string GetTextFromValue(object value)
  {
    string text = this.QueryTextFromValueCore(value);
    QueryTextFromValueEventArgs e = new QueryTextFromValueEventArgs(value, text);
    this.OnQueryTextFromValue(e);
    return e.Text;
  }

  protected virtual string QueryTextFromValueCore(object value)
  {
    if (value == null || value == DBNull.Value)
      return string.Empty;
    IFormatProvider activeFormatProvider = this.GetActiveFormatProvider();
    if (activeFormatProvider is CultureInfo culture)
    {
      TypeConverter converter = TypeDescriptor.GetConverter(value.GetType());
      if (converter.CanConvertTo(typeof (string)))
        return (string) converter.ConvertTo((ITypeDescriptorContext) null, culture, value, typeof (string));
    }
    try
    {
      return Convert.ToString(value, activeFormatProvider);
    }
    catch
    {
    }
    return value.ToString();
  }

  private void OnQueryTextFromValue(QueryTextFromValueEventArgs e)
  {
    if (this.QueryTextFromValue == null)
      return;
    this.QueryTextFromValue((object) this, e);
  }

  public event EventHandler<QueryValueFromTextEventArgs> QueryValueFromText;

  internal object GetValueFromText(string text, out bool hasParsingError)
  {
    object obj = (object) null;
    bool flag = this.QueryValueFromTextCore(text, out obj);
    QueryValueFromTextEventArgs e = new QueryValueFromTextEventArgs(text, obj);
    e.HasParsingError = !flag;
    this.OnQueryValueFromText(e);
    hasParsingError = e.HasParsingError;
    return e.Value;
  }

  protected virtual bool QueryValueFromTextCore(string text, out object value)
  {
    value = (object) null;
    Type valueDataType = this.ValueDataType;
    text = text.Trim();
    if (valueDataType == (Type) null)
      return true;
    if (!valueDataType.IsValueType && valueDataType != typeof (string))
      return false;
    try
    {
      value = ChangeTypeHelper.ChangeType((object) text, valueDataType, this.GetActiveFormatProvider());
    }
    catch
    {
      if (this.BeepOnError)
        SystemSounds.Beep.Play();
      return false;
    }
    return true;
  }

  private void OnQueryValueFromText(QueryValueFromTextEventArgs e)
  {
    if (this.QueryValueFromText == null)
      return;
    this.QueryValueFromText((object) this, e);
  }

  protected override void OnPreviewKeyDown(KeyEventArgs e)
  {
    if (e.ImeProcessedKey != Key.None && !this.IsInIMEComposition)
      this.StartIMEComposition();
    base.OnPreviewKeyDown(e);
  }

  protected override void OnGotFocus(RoutedEventArgs e)
  {
    base.OnGotFocus(e);
    this.RefreshCurrentText(true);
  }

  protected override void OnLostFocus(RoutedEventArgs e)
  {
    base.OnLostFocus(e);
    this.RefreshCurrentText(true);
  }

  protected override void OnTextInput(TextCompositionEventArgs e)
  {
    if (this.IsInIMEComposition)
      this.EndIMEComposition();
    base.OnTextInput(e);
  }

  protected virtual void ValidateValue(object value)
  {
    if (value == null)
      return;
    Type type = this.ValueDataType;
    if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof (Nullable<>)))
      type = new NullableConverter(type).UnderlyingType;
    if (type == (Type) null)
      throw new InvalidOperationException("An attempt was made to set a value when the ValueDataType property is null.");
    if (value != DBNull.Value && value.GetType() != type)
      throw new ArgumentException($"The value is not of type {type.Name}.", "Value");
    this.ValidateValueInRange(this.MinValue, this.MaxValue, value);
  }

  internal static bool IsNumericType(Type type)
  {
    return !(type == (Type) null) && type.IsValueType && (type == typeof (int) || type == typeof (double) || type == typeof (Decimal) || type == typeof (float) || type == typeof (short) || type == typeof (long) || type == typeof (ushort) || type == typeof (uint) || type == typeof (ulong) || type == typeof (byte));
  }

  internal void StartIMEComposition()
  {
    this.m_imePreCompositionCachedTextInfo = new CachedTextInfo((TextBox) this);
  }

  internal void EndIMEComposition()
  {
    CachedTextInfo cachedTextInfo = this.m_imePreCompositionCachedTextInfo.Clone() as CachedTextInfo;
    this.m_imePreCompositionCachedTextInfo = (CachedTextInfo) null;
    this.OnIMECompositionEnded(cachedTextInfo);
  }

  internal virtual void OnIMECompositionEnded(CachedTextInfo cachedTextInfo)
  {
  }

  internal virtual void RefreshConversionHelpers()
  {
  }

  internal IFormatProvider GetActiveFormatProvider()
  {
    return this.FormatProvider ?? (IFormatProvider) CultureInfo.CurrentCulture;
  }

  internal CultureInfo GetCultureInfo()
  {
    return this.GetActiveFormatProvider() is CultureInfo activeFormatProvider ? activeFormatProvider : CultureInfo.CurrentCulture;
  }

  internal virtual string GetCurrentText() => this.Text;

  internal virtual string GetParsableText() => this.Text;

  internal void ForceText(string text, bool preserveCaret)
  {
    this.IsForcingText = true;
    try
    {
      int caretIndex = this.CaretIndex;
      this.Text = text;
      if (!preserveCaret)
        return;
      if (!this.IsLoaded)
        return;
      try
      {
        this.SelectionStart = caretIndex;
      }
      catch (NullReferenceException ex)
      {
      }
    }
    finally
    {
      this.IsForcingText = false;
    }
  }

  internal bool IsValueNull(object value)
  {
    if (value == null || value == DBNull.Value)
      return true;
    Type type = this.ValueDataType;
    if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof (Nullable<>)))
      type = new NullableConverter(type).UnderlyingType;
    if (value.GetType() != type)
      value = Convert.ChangeType(value, type);
    object obj = this.NullValue;
    if (obj == null)
      return false;
    if (obj.GetType() != type)
      obj = Convert.ChangeType(obj, type);
    return obj.Equals(value);
  }

  internal void ForceValue(object value)
  {
    this.IsForcingValue = true;
    try
    {
      this.Value = value;
    }
    finally
    {
      this.IsForcingValue = false;
    }
  }

  internal void RefreshCurrentText(bool preserveCurrentCaretPosition)
  {
    string currentText = this.GetCurrentText();
    if (string.Equals(currentText, this.Text))
      return;
    this.ForceText(currentText, preserveCurrentCaretPosition);
  }

  internal void RefreshValue()
  {
    if (this.IsForcingValue || this.ValueDataType == (Type) null || this.IsInIMEComposition)
      return;
    bool hasParsingError;
    object objA;
    if (this.IsTextReadyToBeParsed)
    {
      objA = this.GetValueFromText(this.GetParsableText(), out hasParsingError);
      if (this.IsValueNull(objA))
        objA = this.NullValue;
    }
    else
    {
      hasParsingError = !this.GetIsEditTextEmpty();
      objA = this.NullValue;
    }
    this.SetHasParsingError(hasParsingError);
    bool flag = hasParsingError;
    try
    {
      this.ValidateValue(objA);
      this.SetIsValueOutOfRange(false);
    }
    catch (Exception ex)
    {
      flag = true;
      if (this.BeepOnError)
        SystemSounds.Beep.Play();
      if (ex is ArgumentOutOfRangeException)
        this.SetIsValueOutOfRange(true);
      objA = this.NullValue;
    }
    if (!object.Equals(objA, this.Value))
      this.ForceValue(objA);
    this.SetHasValidationError(flag);
  }

  internal virtual bool GetIsEditTextEmpty() => this.Text == string.Empty;

  private static object ConvertValueToDataType(object value, Type type)
  {
    if (type == (Type) null)
      return (object) null;
    return value != null && value != DBNull.Value && value.GetType() != type ? ChangeTypeHelper.ChangeType(value, type, (IFormatProvider) CultureInfo.InvariantCulture) : value;
  }

  private void CanEnterLineBreak(object sender, CanExecuteRoutedEventArgs e)
  {
    e.CanExecute = false;
    e.Handled = true;
  }

  private void CanEnterParagraphBreak(object sender, CanExecuteRoutedEventArgs e)
  {
    e.CanExecute = false;
    e.Handled = true;
  }

  private void ValidateValueInRange(object minValue, object maxValue, object value)
  {
    if (this.IsValueNull(value))
      return;
    Type type = this.ValueDataType;
    if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof (Nullable<>)))
      type = new NullableConverter(type).UnderlyingType;
    if (value.GetType() != type)
      value = Convert.ChangeType(value, type);
    if (minValue != null)
    {
      IComparable comparable = (IComparable) minValue;
      if (maxValue != null && comparable.CompareTo(maxValue) > 0)
        throw new ArgumentOutOfRangeException(nameof (minValue), "MaxValue must be greater than MinValue.");
      if (comparable.CompareTo(value) > 0)
        throw new ArgumentOutOfRangeException(nameof (minValue), "Value must be greater than MinValue.");
    }
    if (maxValue != null && ((IComparable) maxValue).CompareTo(value) < 0)
      throw new ArgumentOutOfRangeException(nameof (maxValue), "Value must be less than MaxValue.");
  }

  protected override void OnInitialized(EventArgs e)
  {
    this.IsFinalizingInitialization = true;
    try
    {
      this.CoerceValue(ValueRangeTextBox.ValueDataTypeProperty);
      this.IsNumericValueDataType = ValueRangeTextBox.IsNumericType(this.ValueDataType);
      this.RefreshConversionHelpers();
      this.CoerceValue(ValueRangeTextBox.MinValueProperty);
      this.CoerceValue(ValueRangeTextBox.MaxValueProperty);
      this.CoerceValue(ValueRangeTextBox.ValueProperty);
      this.CoerceValue(ValueRangeTextBox.NullValueProperty);
      this.CoerceValue(TextBox.TextProperty);
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Initialization of the ValueRangeTextBox failed.", ex);
    }
    finally
    {
      this.IsFinalizingInitialization = false;
    }
    base.OnInitialized(e);
  }

  [Flags]
  private enum ValueRangeTextBoxFlags
  {
    IsFinalizingInitialization = 1,
    IsForcingText = 2,
    IsForcingValue = 4,
    IsInValueChanged = 8,
    IsNumericValueDataType = 16, // 0x00000010
  }
}
