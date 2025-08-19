// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.RetryConditionHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers;

public class RetryConditionHeaderValue : ICloneable
{
  private DateTimeOffset? _date;
  private TimeSpan? _delta;

  public DateTimeOffset? Date => this._date;

  public TimeSpan? Delta => this._delta;

  public RetryConditionHeaderValue(DateTimeOffset date) => this._date = new DateTimeOffset?(date);

  public RetryConditionHeaderValue(TimeSpan delta)
  {
    this._delta = delta.TotalSeconds <= (double) int.MaxValue ? new TimeSpan?(delta) : throw new ArgumentOutOfRangeException(nameof (delta));
  }

  private RetryConditionHeaderValue(RetryConditionHeaderValue source)
  {
    this._delta = source._delta;
    this._date = source._date;
  }

  private RetryConditionHeaderValue()
  {
  }

  public override string ToString()
  {
    return this._delta.HasValue ? ((int) this._delta.Value.TotalSeconds).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) : HttpRuleParser.DateToString(this._date.Value);
  }

  public override bool Equals(object obj)
  {
    if (!(obj is RetryConditionHeaderValue conditionHeaderValue))
      return false;
    return this._delta.HasValue ? conditionHeaderValue._delta.HasValue && this._delta.Value == conditionHeaderValue._delta.Value : conditionHeaderValue._date.HasValue && this._date.Value == conditionHeaderValue._date.Value;
  }

  public override int GetHashCode()
  {
    return !this._delta.HasValue ? this._date.Value.GetHashCode() : this._delta.Value.GetHashCode();
  }

  public static RetryConditionHeaderValue Parse(string input)
  {
    int index = 0;
    return (RetryConditionHeaderValue) GenericHeaderParser.RetryConditionParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out RetryConditionHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (RetryConditionHeaderValue) null;
    object parsedValue1;
    if (!GenericHeaderParser.RetryConditionParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (RetryConditionHeaderValue) parsedValue1;
    return true;
  }

  internal static int GetRetryConditionLength(string input, int startIndex, out object parsedValue)
  {
    parsedValue = (object) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    int num1 = startIndex;
    DateTimeOffset result1 = DateTimeOffset.MinValue;
    int result2 = -1;
    int num2;
    switch (input[num1])
    {
      case '0':
      case '1':
      case '2':
      case '3':
      case '4':
      case '5':
      case '6':
      case '7':
      case '8':
      case '9':
        int startIndex1 = num1;
        int numberLength = HttpRuleParser.GetNumberLength(input, num1, false);
        if (numberLength == 0 || numberLength > 10)
          return 0;
        int startIndex2 = num1 + numberLength;
        num2 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
        if (num2 != input.Length || !HeaderUtilities.TryParseInt32(input.Substring(startIndex1, numberLength), out result2))
          return 0;
        break;
      default:
        if (!HttpRuleParser.TryStringToDate(input.Substring(num1), out result1))
          return 0;
        num2 = input.Length;
        break;
    }
    RetryConditionHeaderValue conditionHeaderValue = new RetryConditionHeaderValue();
    if (result2 == -1)
      conditionHeaderValue._date = new DateTimeOffset?(result1);
    else
      conditionHeaderValue._delta = new TimeSpan?(new TimeSpan(0, 0, result2));
    parsedValue = (object) conditionHeaderValue;
    return num2 - startIndex;
  }

  object ICloneable.Clone() => (object) new RetryConditionHeaderValue(this);
}
