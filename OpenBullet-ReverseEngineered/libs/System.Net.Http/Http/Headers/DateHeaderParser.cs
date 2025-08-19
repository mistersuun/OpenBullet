// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.DateHeaderParser
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

#nullable disable
namespace System.Net.Http.Headers;

internal class DateHeaderParser : HttpHeaderParser
{
  internal static readonly DateHeaderParser Parser = new DateHeaderParser();

  private DateHeaderParser()
    : base(false)
  {
  }

  public override string ToString(object value)
  {
    return HttpRuleParser.DateToString((DateTimeOffset) value);
  }

  public override bool TryParseValue(
    string value,
    object storeValue,
    ref int index,
    out object parsedValue)
  {
    parsedValue = (object) null;
    if (string.IsNullOrEmpty(value) || index == value.Length)
      return false;
    string input = value;
    if (index > 0)
      input = value.Substring(index);
    DateTimeOffset result;
    if (!HttpRuleParser.TryStringToDate(input, out result))
      return false;
    index = value.Length;
    parsedValue = (object) result;
    return true;
  }
}
