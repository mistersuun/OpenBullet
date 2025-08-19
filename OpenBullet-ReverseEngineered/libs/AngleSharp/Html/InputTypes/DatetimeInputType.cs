// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.InputTypes.DatetimeInputType
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using AngleSharp.Text;
using System;
using System.Globalization;

#nullable disable
namespace AngleSharp.Html.InputTypes;

internal class DatetimeInputType(IHtmlInputElement input, string name) : BaseInputType(input, name, true)
{
  public override ValidationErrors Check(IValidityState current)
  {
    string str = this.Input.Value;
    DateTime? date = DatetimeInputType.ConvertFromDateTime(str);
    DateTime? min = DatetimeInputType.ConvertFromDateTime(this.Input.Minimum);
    DateTime? max = DatetimeInputType.ConvertFromDateTime(this.Input.Maximum);
    return this.CheckTime(current, str, date, min, max);
  }

  public override double? ConvertToNumber(string value)
  {
    DateTime? nullable = DatetimeInputType.ConvertFromDateTime(value);
    return nullable.HasValue ? new double?(nullable.Value.Subtract(BaseInputType.UnixEpoch).TotalMilliseconds) : new double?();
  }

  public override string ConvertFromNumber(double value)
  {
    return this.ConvertFromDate(BaseInputType.UnixEpoch.AddMilliseconds(value));
  }

  public override DateTime? ConvertToDate(string value)
  {
    return DatetimeInputType.ConvertFromDateTime(value);
  }

  public override string ConvertFromDate(DateTime value)
  {
    return $"{string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:0000}-{1:00}-{2:00}", (object) value.Year, (object) value.Month, (object) value.Day)}T{string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00},{3:000}", (object) value.Hour, (object) value.Minute, (object) value.Second, (object) value.Millisecond)}Z";
  }

  public override void DoStep(int n)
  {
    DateTime? nullable1 = DatetimeInputType.ConvertFromDateTime(this.Input.Value);
    if (!nullable1.HasValue)
      return;
    DateTime dateTime = nullable1.Value.AddMilliseconds(this.GetStep() * (double) n);
    DateTime? nullable2 = DatetimeInputType.ConvertFromDateTime(this.Input.Minimum);
    DateTime? nullable3 = DatetimeInputType.ConvertFromDateTime(this.Input.Maximum);
    if (nullable2.HasValue && !(nullable2.Value <= dateTime) || nullable3.HasValue && !(nullable3.Value >= dateTime))
      return;
    this.Input.ValueAsDate = new DateTime?(dateTime);
  }

  protected override double GetDefaultStepBase() => 0.0;

  protected override double GetDefaultStep() => 60.0;

  protected override double GetStepScaleFactor() => 1000.0;

  protected static DateTime? ConvertFromDateTime(string value)
  {
    if (!string.IsNullOrEmpty(value))
    {
      int num = BaseInputType.FetchDigits(value);
      if (BaseInputType.PositionIsValidForDateTime(value, num))
      {
        int year = int.Parse(value.Substring(0, num), (IFormatProvider) CultureInfo.InvariantCulture);
        int month = int.Parse(value.Substring(num + 1, 2), (IFormatProvider) CultureInfo.InvariantCulture);
        int day = int.Parse(value.Substring(num + 4, 2), (IFormatProvider) CultureInfo.InvariantCulture);
        int index1 = num + 6;
        if (BaseInputType.IsLegalDay(day, month, year) && BaseInputType.IsTimeSeparator(value[index1]))
        {
          string str = value;
          int index2 = index1;
          int position = index2 + 1;
          bool flag = str[index2] == ' ';
          TimeSpan? time = BaseInputType.ToTime(value, ref position);
          DateTime dateTime1 = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
          if (time.HasValue)
          {
            DateTime dateTime2 = dateTime1.Add(time.Value);
            if (position == value.Length)
              return flag ? new DateTime?() : new DateTime?(dateTime2);
            if (value[position] != 'Z')
            {
              if (!DatetimeInputType.IsLegalPosition(value, position))
                return new DateTime?();
              TimeSpan timeSpan = new TimeSpan(int.Parse(value.Substring(position + 1, 2), (IFormatProvider) CultureInfo.InvariantCulture), int.Parse(value.Substring(position + 4, 2), (IFormatProvider) CultureInfo.InvariantCulture), 0);
              if (value[position] == '+')
              {
                dateTime2 = dateTime2.Add(timeSpan);
              }
              else
              {
                if (value[position] != '-')
                  return new DateTime?();
                dateTime2 = dateTime2.Subtract(timeSpan);
              }
            }
            else
            {
              if (position + 1 != value.Length)
                return new DateTime?();
              dateTime2 = dateTime2.ToUniversalTime();
            }
            return new DateTime?(dateTime2);
          }
        }
      }
    }
    return new DateTime?();
  }

  private static bool IsLegalPosition(string value, int position)
  {
    return position == value.Length - 6 && value[position + 1].IsDigit() && value[position + 2].IsDigit() && value[position + 3] == ':' && value[position + 4].IsDigit() && value[position + 5].IsDigit();
  }
}
