// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.TimeSpanHeaderParser
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers;

internal class TimeSpanHeaderParser : BaseHeaderParser
{
  internal static readonly TimeSpanHeaderParser Parser = new TimeSpanHeaderParser();

  private TimeSpanHeaderParser()
    : base(false)
  {
  }

  public override string ToString(object value)
  {
    return ((int) ((TimeSpan) value).TotalSeconds).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
  }

  protected override int GetParsedValueLength(
    string value,
    int startIndex,
    object storeValue,
    out object parsedValue)
  {
    parsedValue = (object) null;
    int numberLength = HttpRuleParser.GetNumberLength(value, startIndex, false);
    if (numberLength == 0 || numberLength > 10)
      return 0;
    int result = 0;
    if (!HeaderUtilities.TryParseInt32(value.Substring(startIndex, numberLength), out result))
      return 0;
    parsedValue = (object) new TimeSpan(0, 0, result);
    return numberLength;
  }
}
