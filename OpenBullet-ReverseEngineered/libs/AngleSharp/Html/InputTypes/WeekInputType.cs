// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.InputTypes.WeekInputType
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using AngleSharp.Text;
using System;
using System.Globalization;

#nullable disable
namespace AngleSharp.Html.InputTypes;

internal class WeekInputType(IHtmlInputElement input, string name) : BaseInputType(input, name, true)
{
  public override ValidationErrors Check(IValidityState current)
  {
    string str = this.Input.Value;
    DateTime? date = WeekInputType.ConvertFromWeek(str);
    DateTime? min = WeekInputType.ConvertFromWeek(this.Input.Minimum);
    DateTime? max = WeekInputType.ConvertFromWeek(this.Input.Maximum);
    return this.CheckTime(current, str, date, min, max);
  }

  public override double? ConvertToNumber(string value)
  {
    DateTime? nullable = WeekInputType.ConvertFromWeek(value);
    return nullable.HasValue ? new double?(nullable.Value.Subtract(BaseInputType.UnixEpoch).TotalMilliseconds) : new double?();
  }

  public override string ConvertFromNumber(double value)
  {
    return this.ConvertFromDate(BaseInputType.UnixEpoch.AddMilliseconds(value));
  }

  public override DateTime? ConvertToDate(string value) => WeekInputType.ConvertFromWeek(value);

  public override string ConvertFromDate(DateTime value)
  {
    int weekOfYear = BaseInputType.GetWeekOfYear(value);
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:0000}-W{1:00}", (object) value.Year, (object) weekOfYear);
  }

  public override void DoStep(int n)
  {
    DateTime? nullable1 = WeekInputType.ConvertFromWeek(this.Input.Value);
    if (!nullable1.HasValue)
      return;
    DateTime dateTime = nullable1.Value.AddMilliseconds(this.GetStep() * (double) n);
    DateTime? nullable2 = WeekInputType.ConvertFromWeek(this.Input.Minimum);
    DateTime? nullable3 = WeekInputType.ConvertFromWeek(this.Input.Maximum);
    if (nullable2.HasValue && !(nullable2.Value <= dateTime) || nullable3.HasValue && !(nullable3.Value >= dateTime))
      return;
    this.Input.ValueAsDate = new DateTime?(dateTime);
  }

  protected override double GetDefaultStepBase() => -259200000.0;

  protected override double GetDefaultStep() => 1.0;

  protected override double GetStepScaleFactor() => 604800000.0;

  protected static DateTime? ConvertFromWeek(string value)
  {
    if (!string.IsNullOrEmpty(value))
    {
      int num = BaseInputType.FetchDigits(value);
      if (WeekInputType.IsLegalPosition(value, num))
      {
        int year = int.Parse(value.Substring(0, num));
        int week = int.Parse(value.Substring(num + 2)) - 1;
        if (BaseInputType.IsLegalWeek(week, year))
        {
          DateTime dateTime = new DateTime(year, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
          DayOfWeek dayOfWeek = dateTime.DayOfWeek;
          if (dayOfWeek == DayOfWeek.Sunday)
            dateTime = dateTime.AddDays(-6.0);
          else if (dayOfWeek > DayOfWeek.Monday)
            dateTime = dateTime.AddDays((double) (1 - dayOfWeek));
          return new DateTime?(dateTime.AddDays((double) (7 * week)));
        }
      }
    }
    return new DateTime?();
  }

  private static bool IsLegalPosition(string value, int position)
  {
    return position >= 4 && position == value.Length - 4 && value[position] == '-' && value[position + 1] == 'W' && value[position + 2].IsDigit() && value[position + 3].IsDigit();
  }
}
