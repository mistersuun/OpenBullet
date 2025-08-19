// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.InputTypes.NumberInputType
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using System;
using System.Globalization;

#nullable disable
namespace AngleSharp.Html.InputTypes;

internal class NumberInputType(IHtmlInputElement input, string name) : BaseInputType(input, name, true)
{
  public override double? ConvertToNumber(string value) => BaseInputType.ToNumber(value);

  public override string ConvertFromNumber(double value)
  {
    return value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }

  public override ValidationErrors Check(IValidityState current)
  {
    double? number1 = this.ConvertToNumber(this.Input.Value);
    ValidationErrors validationErrors = current.IsCustomError ? ValidationErrors.Custom : ValidationErrors.None;
    if (number1.HasValue)
    {
      double? number2 = this.ConvertToNumber(this.Input.Minimum);
      double? number3 = this.ConvertToNumber(this.Input.Maximum);
      if (number2.HasValue)
      {
        double? nullable = number1;
        double num = number2.Value;
        if (nullable.GetValueOrDefault() < num & nullable.HasValue)
          validationErrors ^= ValidationErrors.RangeUnderflow;
      }
      if (number3.HasValue)
      {
        double? nullable = number1;
        double num = number3.Value;
        if (nullable.GetValueOrDefault() > num & nullable.HasValue)
          validationErrors ^= ValidationErrors.RangeOverflow;
      }
      if (this.IsStepMismatch())
        validationErrors ^= ValidationErrors.StepMismatch;
    }
    else if (this.Input.IsRequired)
      validationErrors ^= ValidationErrors.ValueMissing;
    return validationErrors;
  }

  public override void DoStep(int n)
  {
    double? number1 = BaseInputType.ToNumber(this.Input.Value);
    if (!number1.HasValue)
      return;
    double num = number1.Value + this.GetStep() * (double) n;
    double? number2 = BaseInputType.ToNumber(this.Input.Minimum);
    double? number3 = BaseInputType.ToNumber(this.Input.Maximum);
    if (number2.HasValue && number2.Value > num || number3.HasValue && number3.Value < num)
      return;
    this.Input.ValueAsNumber = num;
  }

  protected override double GetDefaultStepBase() => 0.0;

  protected override double GetDefaultStep() => 1.0;

  protected override double GetStepScaleFactor() => 1.0;
}
