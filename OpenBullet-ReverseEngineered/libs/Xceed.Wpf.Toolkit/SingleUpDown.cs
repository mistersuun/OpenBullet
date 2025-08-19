// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.SingleUpDown
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class SingleUpDown : CommonNumericUpDown<float>
{
  public static readonly DependencyProperty AllowInputSpecialValuesProperty = DependencyProperty.Register(nameof (AllowInputSpecialValues), typeof (AllowedSpecialValues), typeof (SingleUpDown), (PropertyMetadata) new UIPropertyMetadata((object) AllowedSpecialValues.None));

  static SingleUpDown()
  {
    CommonNumericUpDown<float>.UpdateMetadata(typeof (SingleUpDown), new float?(1f), new float?(float.NegativeInfinity), new float?(float.PositiveInfinity));
  }

  public SingleUpDown()
    : base(new CommonNumericUpDown<float>.FromText(float.TryParse), new CommonNumericUpDown<float>.FromDecimal(Decimal.ToSingle), (Func<float, float, bool>) ((v1, v2) => (double) v1 < (double) v2), (Func<float, float, bool>) ((v1, v2) => (double) v1 > (double) v2))
  {
  }

  public AllowedSpecialValues AllowInputSpecialValues
  {
    get => (AllowedSpecialValues) this.GetValue(SingleUpDown.AllowInputSpecialValuesProperty);
    set => this.SetValue(SingleUpDown.AllowInputSpecialValuesProperty, (object) value);
  }

  protected override float? OnCoerceIncrement(float? baseValue)
  {
    if (baseValue.HasValue && float.IsNaN(baseValue.Value))
      throw new ArgumentException("NaN is invalid for Increment.");
    return base.OnCoerceIncrement(baseValue);
  }

  protected override float? OnCoerceMaximum(float? baseValue)
  {
    if (baseValue.HasValue && float.IsNaN(baseValue.Value))
      throw new ArgumentException("NaN is invalid for Maximum.");
    return base.OnCoerceMaximum(baseValue);
  }

  protected override float? OnCoerceMinimum(float? baseValue)
  {
    if (baseValue.HasValue && float.IsNaN(baseValue.Value))
      throw new ArgumentException("NaN is invalid for Minimum.");
    return base.OnCoerceMinimum(baseValue);
  }

  protected override float IncrementValue(float value, float increment) => value + increment;

  protected override float DecrementValue(float value, float increment) => value - increment;

  protected override void SetValidSpinDirection()
  {
    if (this.Value.HasValue && float.IsInfinity(this.Value.Value) && this.Spinner != null)
      this.Spinner.ValidSpinDirection = ValidSpinDirections.None;
    else
      base.SetValidSpinDirection();
  }

  protected override float? ConvertTextToValue(string text)
  {
    float? nullable = base.ConvertTextToValue(text);
    if (nullable.HasValue)
    {
      if (float.IsNaN(nullable.Value))
        this.TestInputSpecialValue(this.AllowInputSpecialValues, AllowedSpecialValues.NaN);
      else if (float.IsPositiveInfinity(nullable.Value))
        this.TestInputSpecialValue(this.AllowInputSpecialValues, AllowedSpecialValues.PositiveInfinity);
      else if (float.IsNegativeInfinity(nullable.Value))
        this.TestInputSpecialValue(this.AllowInputSpecialValues, AllowedSpecialValues.NegativeInfinity);
    }
    return nullable;
  }
}
