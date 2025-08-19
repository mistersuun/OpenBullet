// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.StringWithQualityHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers;

public class StringWithQualityHeaderValue : ICloneable
{
  private string _value;
  private double? _quality;

  public string Value => this._value;

  public double? Quality => this._quality;

  public StringWithQualityHeaderValue(string value)
  {
    HeaderUtilities.CheckValidToken(value, nameof (value));
    this._value = value;
  }

  public StringWithQualityHeaderValue(string value, double quality)
  {
    HeaderUtilities.CheckValidToken(value, nameof (value));
    if (quality < 0.0 || quality > 1.0)
      throw new ArgumentOutOfRangeException(nameof (quality));
    this._value = value;
    this._quality = new double?(quality);
  }

  private StringWithQualityHeaderValue(StringWithQualityHeaderValue source)
  {
    this._value = source._value;
    this._quality = source._quality;
  }

  private StringWithQualityHeaderValue()
  {
  }

  public override string ToString()
  {
    return this._quality.HasValue ? $"{this._value}; q={this._quality.Value.ToString("0.0##", (IFormatProvider) NumberFormatInfo.InvariantInfo)}" : this._value;
  }

  public override bool Equals(object obj)
  {
    if (!(obj is StringWithQualityHeaderValue qualityHeaderValue) || !string.Equals(this._value, qualityHeaderValue._value, StringComparison.OrdinalIgnoreCase))
      return false;
    if (!this._quality.HasValue)
      return !qualityHeaderValue._quality.HasValue;
    return qualityHeaderValue._quality.HasValue && this._quality.Value == qualityHeaderValue._quality.Value;
  }

  public override int GetHashCode()
  {
    int hashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(this._value);
    if (this._quality.HasValue)
      hashCode ^= this._quality.Value.GetHashCode();
    return hashCode;
  }

  public static StringWithQualityHeaderValue Parse(string input)
  {
    int index = 0;
    return (StringWithQualityHeaderValue) GenericHeaderParser.SingleValueStringWithQualityParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out StringWithQualityHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (StringWithQualityHeaderValue) null;
    object parsedValue1;
    if (!GenericHeaderParser.SingleValueStringWithQualityParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (StringWithQualityHeaderValue) parsedValue1;
    return true;
  }

  internal static int GetStringWithQualityLength(
    string input,
    int startIndex,
    out object parsedValue)
  {
    parsedValue = (object) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
    if (tokenLength == 0)
      return 0;
    StringWithQualityHeaderValue result = new StringWithQualityHeaderValue();
    result._value = input.Substring(startIndex, tokenLength);
    int startIndex1 = startIndex + tokenLength;
    int index1 = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
    if (index1 == input.Length || input[index1] != ';')
    {
      parsedValue = (object) result;
      return index1 - startIndex;
    }
    int startIndex2 = index1 + 1;
    int index2 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
    if (!StringWithQualityHeaderValue.TryReadQuality(input, result, ref index2))
      return 0;
    parsedValue = (object) result;
    return index2 - startIndex;
  }

  private static bool TryReadQuality(
    string input,
    StringWithQualityHeaderValue result,
    ref int index)
  {
    int index1 = index;
    if (index1 == input.Length || input[index1] != 'q' && input[index1] != 'Q')
      return false;
    int startIndex1 = index1 + 1;
    int index2 = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
    if (index2 == input.Length || input[index2] != '=')
      return false;
    int startIndex2 = index2 + 1;
    int startIndex3 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
    if (startIndex3 == input.Length)
      return false;
    int numberLength = HttpRuleParser.GetNumberLength(input, startIndex3, true);
    if (numberLength == 0)
      return false;
    double result1 = 0.0;
    if (!double.TryParse(input.Substring(startIndex3, numberLength), NumberStyles.AllowDecimalPoint, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result1) || result1 < 0.0 || result1 > 1.0)
      return false;
    result._quality = new double?(result1);
    int startIndex4 = startIndex3 + numberLength;
    int num = startIndex4 + HttpRuleParser.GetWhitespaceLength(input, startIndex4);
    index = num;
    return true;
  }

  object ICloneable.Clone() => (object) new StringWithQualityHeaderValue(this);
}
