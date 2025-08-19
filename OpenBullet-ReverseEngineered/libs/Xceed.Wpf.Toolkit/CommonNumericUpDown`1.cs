// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.CommonNumericUpDown`1
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public abstract class CommonNumericUpDown<T> : NumericUpDown<T?> where T : struct, IFormattable, IComparable<T>
{
  private CommonNumericUpDown<T>.FromText _fromText;
  private CommonNumericUpDown<T>.FromDecimal _fromDecimal;
  private Func<T, T, bool> _fromLowerThan;
  private Func<T, T, bool> _fromGreaterThan;
  internal static readonly DependencyProperty IsInvalidProperty = DependencyProperty.Register(nameof (IsInvalid), typeof (bool), typeof (CommonNumericUpDown<T>), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty ParsingNumberStyleProperty = DependencyProperty.Register(nameof (ParsingNumberStyle), typeof (NumberStyles), typeof (CommonNumericUpDown<T>), (PropertyMetadata) new UIPropertyMetadata((object) NumberStyles.Any));

  internal bool IsInvalid
  {
    get => (bool) this.GetValue(CommonNumericUpDown<T>.IsInvalidProperty);
    private set => this.SetValue(CommonNumericUpDown<T>.IsInvalidProperty, (object) value);
  }

  public NumberStyles ParsingNumberStyle
  {
    get => (NumberStyles) this.GetValue(CommonNumericUpDown<T>.ParsingNumberStyleProperty);
    set => this.SetValue(CommonNumericUpDown<T>.ParsingNumberStyleProperty, (object) value);
  }

  protected CommonNumericUpDown(
    CommonNumericUpDown<T>.FromText fromText,
    CommonNumericUpDown<T>.FromDecimal fromDecimal,
    Func<T, T, bool> fromLowerThan,
    Func<T, T, bool> fromGreaterThan)
  {
    if (fromText == null)
      throw new ArgumentNullException("tryParseMethod");
    if (fromDecimal == null)
      throw new ArgumentNullException(nameof (fromDecimal));
    if (fromLowerThan == null)
      throw new ArgumentNullException(nameof (fromLowerThan));
    if (fromGreaterThan == null)
      throw new ArgumentNullException(nameof (fromGreaterThan));
    this._fromText = fromText;
    this._fromDecimal = fromDecimal;
    this._fromLowerThan = fromLowerThan;
    this._fromGreaterThan = fromGreaterThan;
  }

  protected static void UpdateMetadata(Type type, T? increment, T? minValue, T? maxValue)
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) type));
    CommonNumericUpDown<T>.UpdateMetadataCommon(type, increment, minValue, maxValue);
  }

  protected void TestInputSpecialValue(
    AllowedSpecialValues allowedValues,
    AllowedSpecialValues valueToCompare)
  {
    if ((allowedValues & valueToCompare) == valueToCompare)
      return;
    switch (valueToCompare)
    {
      case AllowedSpecialValues.NaN:
        throw new InvalidDataException("Value to parse shouldn't be NaN.");
      case AllowedSpecialValues.PositiveInfinity:
        throw new InvalidDataException("Value to parse shouldn't be Positive Infinity.");
      case AllowedSpecialValues.NegativeInfinity:
        throw new InvalidDataException("Value to parse shouldn't be Negative Infinity.");
    }
  }

  internal bool IsBetweenMinMax(T? value)
  {
    return !this.IsLowerThan(value, this.Minimum) && !this.IsGreaterThan(value, this.Maximum);
  }

  private static void UpdateMetadataCommon(Type type, T? increment, T? minValue, T? maxValue)
  {
    NumericUpDown<T?>.IncrementProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) increment));
    UpDownBase<T?>.MaximumProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) maxValue));
    UpDownBase<T?>.MinimumProperty.OverrideMetadata(type, (PropertyMetadata) new FrameworkPropertyMetadata((object) minValue));
  }

  private bool IsLowerThan(T? value1, T? value2)
  {
    return value1.HasValue && value2.HasValue && this._fromLowerThan(value1.Value, value2.Value);
  }

  private bool IsGreaterThan(T? value1, T? value2)
  {
    return value1.HasValue && value2.HasValue && this._fromGreaterThan(value1.Value, value2.Value);
  }

  private bool HandleNullSpin()
  {
    if (!this.Value.HasValue)
    {
      this.Value = this.CoerceValueMinMax(this.DefaultValue.HasValue ? this.DefaultValue.Value : default (T));
      return true;
    }
    return !this.Increment.HasValue;
  }

  private T? CoerceValueMinMax(T value)
  {
    if (this.IsLowerThan(new T?(value), this.Minimum))
      return this.Minimum;
    return this.IsGreaterThan(new T?(value), this.Maximum) ? this.Maximum : new T?(value);
  }

  protected override void OnIncrement()
  {
    if (this.HandleNullSpin())
      return;
    if (this.UpdateValueOnEnterKey)
      this.TextBox.Text = this.CoerceValueMinMax(this.IncrementValue(this.ConvertTextToValue(this.TextBox.Text).Value, this.Increment.Value)).Value.ToString(this.FormatString, (IFormatProvider) this.CultureInfo);
    else
      this.Value = this.CoerceValueMinMax(this.IncrementValue(this.Value.Value, this.Increment.Value));
  }

  protected override void OnDecrement()
  {
    if (this.HandleNullSpin())
      return;
    if (this.UpdateValueOnEnterKey)
      this.TextBox.Text = this.CoerceValueMinMax(this.DecrementValue(this.ConvertTextToValue(this.TextBox.Text).Value, this.Increment.Value)).Value.ToString(this.FormatString, (IFormatProvider) this.CultureInfo);
    else
      this.Value = this.CoerceValueMinMax(this.DecrementValue(this.Value.Value, this.Increment.Value));
  }

  protected override void OnMinimumChanged(T? oldValue, T? newValue)
  {
    base.OnMinimumChanged(oldValue, newValue);
    if (!this.Value.HasValue || !this.ClipValueToMinMax)
      return;
    this.Value = this.CoerceValueMinMax(this.Value.Value);
  }

  protected override void OnMaximumChanged(T? oldValue, T? newValue)
  {
    base.OnMaximumChanged(oldValue, newValue);
    if (!this.Value.HasValue || !this.ClipValueToMinMax)
      return;
    this.Value = this.CoerceValueMinMax(this.Value.Value);
  }

  protected override T? ConvertTextToValue(string text)
  {
    T? nullable = new T?();
    if (string.IsNullOrEmpty(text))
      return nullable;
    string text1 = this.ConvertValueToText();
    if (object.Equals((object) text1, (object) text))
    {
      this.IsInvalid = false;
      return this.Value;
    }
    T? valueCore = this.ConvertTextToValueCore(text1, text);
    if (this.ClipValueToMinMax)
      return this.GetClippedMinMaxValue(valueCore);
    this.ValidateDefaultMinMax(valueCore);
    return valueCore;
  }

  protected override string ConvertValueToText()
  {
    if (!this.Value.HasValue)
      return string.Empty;
    this.IsInvalid = false;
    if (!this.FormatString.Contains("{0"))
      return this.Value.Value.ToString(this.FormatString, (IFormatProvider) this.CultureInfo);
    return string.Format((IFormatProvider) this.CultureInfo, this.FormatString, (object) this.Value.Value);
  }

  protected override void SetValidSpinDirection()
  {
    ValidSpinDirections validSpinDirections = ValidSpinDirections.None;
    if (this.Increment.HasValue && !this.IsReadOnly)
    {
      if (this.IsLowerThan(this.Value, this.Maximum) || !this.Value.HasValue || !this.Maximum.HasValue)
        validSpinDirections |= ValidSpinDirections.Increase;
      if (this.IsGreaterThan(this.Value, this.Minimum) || !this.Value.HasValue || !this.Minimum.HasValue)
        validSpinDirections |= ValidSpinDirections.Decrease;
    }
    if (this.Spinner == null)
      return;
    this.Spinner.ValidSpinDirection = validSpinDirections;
  }

  private bool IsPercent(string stringToTest)
  {
    int num = stringToTest.IndexOf("P");
    return num >= 0 && (!stringToTest.Substring(0, num).Contains("'") ? 0 : (stringToTest.Substring(num, this.FormatString.Length - num).Contains("'") ? 1 : 0)) == 0;
  }

  private T? ConvertTextToValueCore(string currentValueText, string text)
  {
    T? valueCore;
    if (this.IsPercent(this.FormatString))
    {
      valueCore = new T?(this._fromDecimal(NumericUpDown<T?>.ParsePercent(text, (IFormatProvider) this.CultureInfo)));
    }
    else
    {
      T result1 = new T();
      if (!this._fromText(text, this.ParsingNumberStyle, (IFormatProvider) this.CultureInfo, out result1))
      {
        bool flag = true;
        T result2;
        if (!this._fromText(currentValueText, this.ParsingNumberStyle, (IFormatProvider) this.CultureInfo, out result2))
        {
          IEnumerable<char> chars = currentValueText.Where<char>((Func<char, bool>) (c => !char.IsDigit(c)));
          if (chars.Count<char>() > 0)
          {
            IEnumerable<char> second = text.Where<char>((Func<char, bool>) (c => !char.IsDigit(c)));
            if (chars.Except<char>(second).ToList<char>().Count == 0)
            {
              foreach (char ch in second)
                text = text.Replace(ch.ToString(), string.Empty);
              if (this._fromText(text, this.ParsingNumberStyle, (IFormatProvider) this.CultureInfo, out result1))
                flag = false;
            }
          }
        }
        if (flag)
        {
          this.IsInvalid = true;
          throw new InvalidDataException("Input string was not in a correct format.");
        }
      }
      valueCore = new T?(result1);
    }
    return valueCore;
  }

  private T? GetClippedMinMaxValue(T? result)
  {
    if (this.IsGreaterThan(result, this.Maximum))
      return this.Maximum;
    return this.IsLowerThan(result, this.Minimum) ? this.Minimum : result;
  }

  private void ValidateDefaultMinMax(T? value)
  {
    if (object.Equals((object) value, (object) this.DefaultValue))
      return;
    if (this.IsLowerThan(value, this.Minimum))
      throw new ArgumentOutOfRangeException("Minimum", $"Value must be greater than MinValue of {this.Minimum}");
    if (this.IsGreaterThan(value, this.Maximum))
      throw new ArgumentOutOfRangeException("Maximum", $"Value must be less than MaxValue of {this.Maximum}");
  }

  protected abstract T IncrementValue(T value, T increment);

  protected abstract T DecrementValue(T value, T increment);

  protected delegate bool FromText(
    string s,
    NumberStyles style,
    IFormatProvider provider,
    out T result)
    where T : struct, IFormattable, IComparable<T>;

  protected delegate T FromDecimal(Decimal d) where T : struct, IFormattable, IComparable<T>;
}
