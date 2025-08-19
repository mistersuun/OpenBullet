// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.InputTypes.TimeInputType
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using System;
using System.Globalization;

#nullable disable
namespace AngleSharp.Html.InputTypes;

internal class TimeInputType(IHtmlInputElement input, string name) : BaseInputType(input, name, true)
{
  public override ValidationErrors Check(IValidityState current)
  {
    string str = this.Input.Value;
    DateTime? date = TimeInputType.ConvertFromTime(str);
    DateTime? min = TimeInputType.ConvertFromTime(this.Input.Minimum);
    DateTime? max = TimeInputType.ConvertFromTime(this.Input.Maximum);
    return this.CheckTime(current, str, date, min, max);
  }

  public override double? ConvertToNumber(string value)
  {
    DateTime? nullable = TimeInputType.ConvertFromTime(value);
    return nullable.HasValue ? new double?(nullable.Value.Subtract(new DateTime()).TotalMilliseconds) : new double?();
  }

  public override string ConvertFromNumber(double value)
  {
    return this.ConvertFromDate(new DateTime().AddMilliseconds(value));
  }

  public override DateTime? ConvertToDate(string value)
  {
    DateTime? nullable = TimeInputType.ConvertFromTime(value);
    return nullable.HasValue ? new DateTime?(BaseInputType.UnixEpoch.Add(nullable.Value.Subtract(new DateTime()))) : new DateTime?();
  }

  public override string ConvertFromDate(DateTime value)
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00},{3:000}", (object) value.Hour, (object) value.Minute, (object) value.Second, (object) value.Millisecond);
  }

  public override void DoStep(int n)
  {
    DateTime? nullable1 = TimeInputType.ConvertFromTime(this.Input.Value);
    if (!nullable1.HasValue)
      return;
    DateTime dateTime = nullable1.Value.AddMilliseconds(this.GetStep() * (double) n);
    DateTime? nullable2 = TimeInputType.ConvertFromTime(this.Input.Minimum);
    DateTime? nullable3 = TimeInputType.ConvertFromTime(this.Input.Maximum);
    if (nullable2.HasValue && !(nullable2.Value <= dateTime) || nullable3.HasValue && !(nullable3.Value >= dateTime))
      return;
    this.Input.ValueAsDate = new DateTime?(dateTime);
  }

  protected override double GetDefaultStepBase() => 0.0;

  protected override double GetDefaultStep() => 60.0;

  protected override double GetStepScaleFactor() => 1000.0;

  protected static DateTime? ConvertFromTime(string value)
  {
    if (!string.IsNullOrEmpty(value))
    {
      int position = 0;
      TimeSpan? time = BaseInputType.ToTime(value, ref position);
      if (time.HasValue && position == value.Length)
        return new DateTime?(new DateTime(0L, DateTimeKind.Utc).Add(time.Value));
    }
    return new DateTime?();
  }
}
