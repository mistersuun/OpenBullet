// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.NameValueHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Globalization;
using System.Text;

#nullable disable
namespace System.Net.Http.Headers;

public class NameValueHeaderValue : ICloneable
{
  private static readonly Func<NameValueHeaderValue> s_defaultNameValueCreator = new Func<NameValueHeaderValue>(NameValueHeaderValue.CreateNameValue);
  private string _name;
  private string _value;

  public string Name => this._name;

  public string Value
  {
    get => this._value;
    set
    {
      NameValueHeaderValue.CheckValueFormat(value);
      this._value = value;
    }
  }

  internal NameValueHeaderValue()
  {
  }

  public NameValueHeaderValue(string name)
    : this(name, (string) null)
  {
  }

  public NameValueHeaderValue(string name, string value)
  {
    NameValueHeaderValue.CheckNameValueFormat(name, value);
    this._name = name;
    this._value = value;
  }

  protected NameValueHeaderValue(NameValueHeaderValue source)
  {
    this._name = source._name;
    this._value = source._value;
  }

  public override int GetHashCode()
  {
    int hashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(this._name);
    if (string.IsNullOrEmpty(this._value))
      return hashCode;
    return this._value[0] == '"' ? hashCode ^ this._value.GetHashCode() : hashCode ^ StringComparer.OrdinalIgnoreCase.GetHashCode(this._value);
  }

  public override bool Equals(object obj)
  {
    if (!(obj is NameValueHeaderValue valueHeaderValue) || !string.Equals(this._name, valueHeaderValue._name, StringComparison.OrdinalIgnoreCase))
      return false;
    if (string.IsNullOrEmpty(this._value))
      return string.IsNullOrEmpty(valueHeaderValue._value);
    return this._value[0] == '"' ? string.Equals(this._value, valueHeaderValue._value, StringComparison.Ordinal) : string.Equals(this._value, valueHeaderValue._value, StringComparison.OrdinalIgnoreCase);
  }

  public static NameValueHeaderValue Parse(string input)
  {
    int index = 0;
    return (NameValueHeaderValue) GenericHeaderParser.SingleValueNameValueParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out NameValueHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (NameValueHeaderValue) null;
    object parsedValue1;
    if (!GenericHeaderParser.SingleValueNameValueParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (NameValueHeaderValue) parsedValue1;
    return true;
  }

  public override string ToString()
  {
    return !string.IsNullOrEmpty(this._value) ? $"{this._name}={this._value}" : this._name;
  }

  internal static void ToString(
    ObjectCollection<NameValueHeaderValue> values,
    char separator,
    bool leadingSeparator,
    StringBuilder destination)
  {
    if (values == null || values.Count == 0)
      return;
    foreach (NameValueHeaderValue valueHeaderValue in values)
    {
      if (leadingSeparator || destination.Length > 0)
      {
        destination.Append(separator);
        destination.Append(' ');
      }
      destination.Append(valueHeaderValue.ToString());
    }
  }

  internal static string ToString(
    ObjectCollection<NameValueHeaderValue> values,
    char separator,
    bool leadingSeparator)
  {
    if (values == null || values.Count == 0)
      return (string) null;
    StringBuilder destination = new StringBuilder();
    NameValueHeaderValue.ToString(values, separator, leadingSeparator, destination);
    return destination.ToString();
  }

  internal static int GetHashCode(ObjectCollection<NameValueHeaderValue> values)
  {
    if (values == null || values.Count == 0)
      return 0;
    int hashCode = 0;
    foreach (NameValueHeaderValue valueHeaderValue in values)
      hashCode ^= valueHeaderValue.GetHashCode();
    return hashCode;
  }

  internal static int GetNameValueLength(
    string input,
    int startIndex,
    out NameValueHeaderValue parsedValue)
  {
    return NameValueHeaderValue.GetNameValueLength(input, startIndex, NameValueHeaderValue.s_defaultNameValueCreator, out parsedValue);
  }

  internal static int GetNameValueLength(
    string input,
    int startIndex,
    Func<NameValueHeaderValue> nameValueCreator,
    out NameValueHeaderValue parsedValue)
  {
    parsedValue = (NameValueHeaderValue) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
    if (tokenLength == 0)
      return 0;
    string str = input.Substring(startIndex, tokenLength);
    int startIndex1 = startIndex + tokenLength;
    int num = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
    if (num == input.Length || input[num] != '=')
    {
      parsedValue = nameValueCreator();
      parsedValue._name = str;
      return num + HttpRuleParser.GetWhitespaceLength(input, num) - startIndex;
    }
    int startIndex2 = num + 1;
    int startIndex3 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
    int valueLength = NameValueHeaderValue.GetValueLength(input, startIndex3);
    if (valueLength == 0)
      return 0;
    parsedValue = nameValueCreator();
    parsedValue._name = str;
    parsedValue._value = input.Substring(startIndex3, valueLength);
    int startIndex4 = startIndex3 + valueLength;
    return startIndex4 + HttpRuleParser.GetWhitespaceLength(input, startIndex4) - startIndex;
  }

  internal static int GetNameValueListLength(
    string input,
    int startIndex,
    char delimiter,
    ObjectCollection<NameValueHeaderValue> nameValueCollection)
  {
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    int startIndex1 = startIndex + HttpRuleParser.GetWhitespaceLength(input, startIndex);
    int index;
    while (true)
    {
      NameValueHeaderValue parsedValue = (NameValueHeaderValue) null;
      int nameValueLength = NameValueHeaderValue.GetNameValueLength(input, startIndex1, NameValueHeaderValue.s_defaultNameValueCreator, out parsedValue);
      if (nameValueLength != 0)
      {
        nameValueCollection.Add(parsedValue);
        int startIndex2 = startIndex1 + nameValueLength;
        index = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
        if (index != input.Length && (int) input[index] == (int) delimiter)
        {
          int startIndex3 = index + 1;
          startIndex1 = startIndex3 + HttpRuleParser.GetWhitespaceLength(input, startIndex3);
        }
        else
          goto label_6;
      }
      else
        break;
    }
    return 0;
label_6:
    return index - startIndex;
  }

  internal static NameValueHeaderValue Find(
    ObjectCollection<NameValueHeaderValue> values,
    string name)
  {
    if (values == null || values.Count == 0)
      return (NameValueHeaderValue) null;
    foreach (NameValueHeaderValue valueHeaderValue in values)
    {
      if (string.Equals(valueHeaderValue.Name, name, StringComparison.OrdinalIgnoreCase))
        return valueHeaderValue;
    }
    return (NameValueHeaderValue) null;
  }

  internal static int GetValueLength(string input, int startIndex)
  {
    if (startIndex >= input.Length)
      return 0;
    int length = HttpRuleParser.GetTokenLength(input, startIndex);
    return length == 0 && HttpRuleParser.GetQuotedStringLength(input, startIndex, out length) != HttpParseResult.Parsed ? 0 : length;
  }

  private static void CheckNameValueFormat(string name, string value)
  {
    HeaderUtilities.CheckValidToken(name, nameof (name));
    NameValueHeaderValue.CheckValueFormat(value);
  }

  private static void CheckValueFormat(string value)
  {
    if (!string.IsNullOrEmpty(value) && NameValueHeaderValue.GetValueLength(value, 0) != value.Length)
      throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) value));
  }

  private static NameValueHeaderValue CreateNameValue() => new NameValueHeaderValue();

  object ICloneable.Clone() => (object) new NameValueHeaderValue(this);
}
