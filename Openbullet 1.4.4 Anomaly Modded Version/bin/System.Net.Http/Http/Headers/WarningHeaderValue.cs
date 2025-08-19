// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.WarningHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Globalization;
using System.Text;

#nullable disable
namespace System.Net.Http.Headers;

public class WarningHeaderValue : ICloneable
{
  private int _code;
  private string _agent;
  private string _text;
  private DateTimeOffset? _date;

  public int Code => this._code;

  public string Agent => this._agent;

  public string Text => this._text;

  public DateTimeOffset? Date => this._date;

  public WarningHeaderValue(int code, string agent, string text)
  {
    WarningHeaderValue.CheckCode(code);
    WarningHeaderValue.CheckAgent(agent);
    HeaderUtilities.CheckValidQuotedString(text, nameof (text));
    this._code = code;
    this._agent = agent;
    this._text = text;
  }

  public WarningHeaderValue(int code, string agent, string text, DateTimeOffset date)
  {
    WarningHeaderValue.CheckCode(code);
    WarningHeaderValue.CheckAgent(agent);
    HeaderUtilities.CheckValidQuotedString(text, nameof (text));
    this._code = code;
    this._agent = agent;
    this._text = text;
    this._date = new DateTimeOffset?(date);
  }

  private WarningHeaderValue()
  {
  }

  private WarningHeaderValue(WarningHeaderValue source)
  {
    this._code = source._code;
    this._agent = source._agent;
    this._text = source._text;
    this._date = source._date;
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(this._code.ToString("000", (IFormatProvider) NumberFormatInfo.InvariantInfo));
    stringBuilder.Append(' ');
    stringBuilder.Append(this._agent);
    stringBuilder.Append(' ');
    stringBuilder.Append(this._text);
    if (this._date.HasValue)
    {
      stringBuilder.Append(" \"");
      stringBuilder.Append(HttpRuleParser.DateToString(this._date.Value));
      stringBuilder.Append('"');
    }
    return stringBuilder.ToString();
  }

  public override bool Equals(object obj)
  {
    if (!(obj is WarningHeaderValue warningHeaderValue) || this._code != warningHeaderValue._code || !string.Equals(this._agent, warningHeaderValue._agent, StringComparison.OrdinalIgnoreCase) || !string.Equals(this._text, warningHeaderValue._text, StringComparison.Ordinal))
      return false;
    if (!this._date.HasValue)
      return !warningHeaderValue._date.HasValue;
    return warningHeaderValue._date.HasValue && this._date.Value == warningHeaderValue._date.Value;
  }

  public override int GetHashCode()
  {
    int hashCode = this._code.GetHashCode() ^ StringComparer.OrdinalIgnoreCase.GetHashCode(this._agent) ^ this._text.GetHashCode();
    if (this._date.HasValue)
      hashCode ^= this._date.Value.GetHashCode();
    return hashCode;
  }

  public static WarningHeaderValue Parse(string input)
  {
    int index = 0;
    return (WarningHeaderValue) GenericHeaderParser.SingleValueWarningParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out WarningHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (WarningHeaderValue) null;
    object parsedValue1;
    if (!GenericHeaderParser.SingleValueWarningParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (WarningHeaderValue) parsedValue1;
    return true;
  }

  internal static int GetWarningLength(string input, int startIndex, out object parsedValue)
  {
    parsedValue = (object) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    int current = startIndex;
    int code;
    string agent;
    if (!WarningHeaderValue.TryReadCode(input, ref current, out code) || !WarningHeaderValue.TryReadAgent(input, current, ref current, out agent))
      return 0;
    int length = 0;
    int startIndex1 = current;
    if (HttpRuleParser.GetQuotedStringLength(input, current, out length) != HttpParseResult.Parsed)
      return 0;
    current += length;
    DateTimeOffset? date = new DateTimeOffset?();
    if (!WarningHeaderValue.TryReadDate(input, ref current, out date))
      return 0;
    parsedValue = (object) new WarningHeaderValue()
    {
      _code = code,
      _agent = agent,
      _text = input.Substring(startIndex1, length),
      _date = date
    };
    return current - startIndex;
  }

  private static bool TryReadAgent(
    string input,
    int startIndex,
    ref int current,
    out string agent)
  {
    agent = (string) null;
    int hostLength = HttpRuleParser.GetHostLength(input, startIndex, true, out agent);
    if (hostLength == 0)
      return false;
    current += hostLength;
    int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, current);
    current += whitespaceLength;
    return whitespaceLength != 0 && current != input.Length;
  }

  private static bool TryReadCode(string input, ref int current, out int code)
  {
    code = 0;
    int numberLength = HttpRuleParser.GetNumberLength(input, current, false);
    if (numberLength == 0 || numberLength > 3 || !HeaderUtilities.TryParseInt32(input.Substring(current, numberLength), out code))
      return false;
    current += numberLength;
    int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, current);
    current += whitespaceLength;
    return whitespaceLength != 0 && current != input.Length;
  }

  private static bool TryReadDate(string input, ref int current, out DateTimeOffset? date)
  {
    date = new DateTimeOffset?();
    int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, current);
    current += whitespaceLength;
    if (current < input.Length && input[current] == '"')
    {
      if (whitespaceLength == 0)
        return false;
      ++current;
      int startIndex = current;
      while (current < input.Length && input[current] != '"')
        ++current;
      DateTimeOffset result;
      if (current == input.Length || current == startIndex || !HttpRuleParser.TryStringToDate(input.Substring(startIndex, current - startIndex), out result))
        return false;
      date = new DateTimeOffset?(result);
      ++current;
      current += HttpRuleParser.GetWhitespaceLength(input, current);
    }
    return true;
  }

  object ICloneable.Clone() => (object) new WarningHeaderValue(this);

  private static void CheckCode(int code)
  {
    if (code < 0 || code > 999)
      throw new ArgumentOutOfRangeException(nameof (code));
  }

  private static void CheckAgent(string agent)
  {
    if (string.IsNullOrEmpty(agent))
      throw new ArgumentException(SR.net_http_argument_empty_string, nameof (agent));
    string host = (string) null;
    if (HttpRuleParser.GetHostLength(agent, 0, true, out host) != agent.Length)
      throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) agent));
  }
}
