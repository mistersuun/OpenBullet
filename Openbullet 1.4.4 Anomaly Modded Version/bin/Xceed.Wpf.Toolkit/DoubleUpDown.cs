// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.DoubleUpDown
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class DoubleUpDown : CommonNumericUpDown<double>
{
  public static readonly DependencyProperty AllowInputSpecialValuesProperty = DependencyProperty.Register(nameof (AllowInputSpecialValues), typeof (AllowedSpecialValues), typeof (DoubleUpDown), (PropertyMetadata) new UIPropertyMetadata((object) AllowedSpecialValues.None));

  static DoubleUpDown()
  {
    CommonNumericUpDown<double>.UpdateMetadata(typeof (DoubleUpDown), new double?(1.0), new double?(double.NegativeInfinity), new double?(double.PositiveInfinity));
  }

  public DoubleUpDown()
    : base(new CommonNumericUpDown<double>.FromText(double.TryParse), new CommonNumericUpDown<double>.FromDecimal(Decimal.ToDouble), (Func<double, double, bool>) ((v1, v2) => v1 < v2), (Func<double, double, bool>) ((v1, v2) => v1 > v2))
  {
  }

  public AllowedSpecialValues AllowInputSpecialValues
  {
    get => (AllowedSpecialValues) this.GetValue(DoubleUpDown.AllowInputSpecialValuesProperty);
    set => this.SetValue(DoubleUpDown.AllowInputSpecialValuesProperty, (object) value);
  }

  protected override double? OnCoerceIncrement(double? baseValue)
  {
    if (baseValue.HasValue && double.IsNaN(baseValue.Value))
      throw new ArgumentException("NaN is invalid for Increment.");
    return base.OnCoerceIncrement(baseValue);
  }

  protected override double? OnCoerceMaximum(double? baseValue)
  {
    if (baseValue.HasValue && double.IsNaN(baseValue.Value))
      throw new ArgumentException("NaN is invalid for Maximum.");
    return base.OnCoerceMaximum(baseValue);
  }

  protected override double? OnCoerceMinimum(double? baseValue)
  {
    if (baseValue.HasValue && double.IsNaN(baseValue.Value))
      throw new ArgumentException("NaN is invalid for Minimum.");
    return base.OnCoerceMinimum(baseValue);
  }

  protected override double IncrementValue(double value, double increment) => value + increment;

  protected override double DecrementValue(double value, double increment) => value - increment;

  protected override void SetValidSpinDirection()
  {
    if (this.Value.HasValue && double.IsInfinity(this.Value.Value) && this.Spinner != null)
      this.Spinner.ValidSpinDirection = ValidSpinDirections.None;
    else
      base.SetValidSpinDirection();
  }

  protected override double? ConvertTextToValue(string text)
  {
    double? nullable = base.ConvertTextToValue(text);
    if (nullable.HasValue)
    {
      if (double.IsNaN(nullable.Value))
        this.TestInputSpecialValue(this.AllowInputSpecialValues, AllowedSpecialValues.NaN);
      else if (double.IsPositiveInfinity(nullable.Value))
        this.TestInputSpecialValue(this.AllowInputSpecialValues, AllowedSpecialValues.PositiveInfinity);
      else if (double.IsNegativeInfinity(nullable.Value))
        this.TestInputSpecialValue(this.AllowInputSpecialValues, AllowedSpecialValues.NegativeInfinity);
    }
    return nullable;
  }
}
