// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.InputTypes.MonthInputType
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using AngleSharp.Text;
using System;
using System.Globalization;

#nullable disable
namespace AngleSharp.Html.InputTypes;

internal class MonthInputType(IHtmlInputElement input, string name) : BaseInputType(input, name, true)
{
  public override ValidationErrors Check(IValidityState current)
  {
    string str = this.Input.Value;
    DateTime? date = MonthInputType.ConvertFromMonth(str);
    DateTime? min = MonthInputType.ConvertFromMonth(this.Input.Minimum);
    DateTime? max = MonthInputType.ConvertFromMonth(this.Input.Maximum);
    return this.CheckTime(current, str, date, min, max);
  }

  public override double? ConvertToNumber(string value)
  {
    DateTime? nullable = MonthInputType.ConvertFromMonth(value);
    return nullable.HasValue ? new double?((double) ((nullable.Value.Year - 1970) * 12 + nullable.Value.Month - 1)) : new double?();
  }

  public override string ConvertFromNumber(double value)
  {
    return this.ConvertFromDate(BaseInputType.UnixEpoch.AddMonths((int) value));
  }

  public override DateTime? ConvertToDate(string value) => MonthInputType.ConvertFromMonth(value);

  public override string ConvertFromDate(DateTime value)
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:0000}-{1:00}", (object) value.Year, (object) value.Month);
  }

  public override void DoStep(int n)
  {
    DateTime? nullable1 = MonthInputType.ConvertFromMonth(this.Input.Value);
    if (!nullable1.HasValue)
      return;
    DateTime dateTime = nullable1.Value.AddMilliseconds(this.GetStep() * (double) n);
    DateTime? nullable2 = MonthInputType.ConvertFromMonth(this.Input.Minimum);
    DateTime? nullable3 = MonthInputType.ConvertFromMonth(this.Input.Maximum);
    if (nullable2.HasValue && !(nullable2.Value <= dateTime) || nullable3.HasValue && !(nullable3.Value >= dateTime))
      return;
    this.Input.ValueAsDate = new DateTime?(dateTime);
  }

  protected override double GetDefaultStepBase() => 0.0;

  protected override double GetDefaultStep() => 1.0;

  protected override double GetStepScaleFactor() => 1.0;

  protected static DateTime? ConvertFromMonth(string value)
  {
    if (!string.IsNullOrEmpty(value))
    {
      int num = BaseInputType.FetchDigits(value);
      if (MonthInputType.IsLegalPosition(value, num))
      {
        int year = int.Parse(value.Substring(0, num), (IFormatProvider) CultureInfo.InvariantCulture);
        int month = int.Parse(value.Substring(num + 1), (IFormatProvider) CultureInfo.InvariantCulture);
        if (BaseInputType.IsLegalDay(1, month, year))
          return new DateTime?(new DateTime(year, month, 1, 0, 0, 0, 0, DateTimeKind.Utc));
      }
    }
    return new DateTime?();
  }

  private static bool IsLegalPosition(string value, int position)
  {
    return position >= 4 && position == value.Length - 3 && value[position] == '-' && value[position + 1].IsDigit() && value[position + 2].IsDigit();
  }
}
