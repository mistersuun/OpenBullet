// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.ContentRangeHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Globalization;
using System.Text;

#nullable disable
namespace System.Net.Http.Headers;

public class ContentRangeHeaderValue : ICloneable
{
  private string _unit;
  private long? _from;
  private long? _to;
  private long? _length;

  public string Unit
  {
    get => this._unit;
    set
    {
      HeaderUtilities.CheckValidToken(value, nameof (value));
      this._unit = value;
    }
  }

  public long? From => this._from;

  public long? To => this._to;

  public long? Length => this._length;

  public bool HasLength => this._length.HasValue;

  public bool HasRange => this._from.HasValue;

  public ContentRangeHeaderValue(long from, long to, long length)
  {
    if (length < 0L)
      throw new ArgumentOutOfRangeException(nameof (length));
    if (to < 0L || to > length)
      throw new ArgumentOutOfRangeException(nameof (to));
    this._from = from >= 0L && from <= to ? new long?(from) : throw new ArgumentOutOfRangeException(nameof (from));
    this._to = new long?(to);
    this._length = new long?(length);
    this._unit = "bytes";
  }

  public ContentRangeHeaderValue(long length)
  {
    this._length = length >= 0L ? new long?(length) : throw new ArgumentOutOfRangeException(nameof (length));
    this._unit = "bytes";
  }

  public ContentRangeHeaderValue(long from, long to)
  {
    if (to < 0L)
      throw new ArgumentOutOfRangeException(nameof (to));
    this._from = from >= 0L && from <= to ? new long?(from) : throw new ArgumentOutOfRangeException(nameof (from));
    this._to = new long?(to);
    this._unit = "bytes";
  }

  private ContentRangeHeaderValue()
  {
  }

  private ContentRangeHeaderValue(ContentRangeHeaderValue source)
  {
    this._from = source._from;
    this._to = source._to;
    this._length = source._length;
    this._unit = source._unit;
  }

  public override bool Equals(object obj)
  {
    if (!(obj is ContentRangeHeaderValue rangeHeaderValue))
      return false;
    long? nullable1 = this._from;
    long? nullable2 = rangeHeaderValue._from;
    if ((nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? (nullable1.HasValue == nullable2.HasValue ? 1 : 0) : 0) != 0)
    {
      nullable2 = this._to;
      nullable1 = rangeHeaderValue._to;
      if ((nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() ? (nullable2.HasValue == nullable1.HasValue ? 1 : 0) : 0) != 0)
      {
        nullable1 = this._length;
        nullable2 = rangeHeaderValue._length;
        if ((nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? (nullable1.HasValue == nullable2.HasValue ? 1 : 0) : 0) != 0)
          return string.Equals(this._unit, rangeHeaderValue._unit, StringComparison.OrdinalIgnoreCase);
      }
    }
    return false;
  }

  public override int GetHashCode()
  {
    int hashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(this._unit);
    if (this.HasRange)
      hashCode = hashCode ^ this._from.GetHashCode() ^ this._to.GetHashCode();
    if (this.HasLength)
      hashCode ^= this._length.GetHashCode();
    return hashCode;
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder(this._unit);
    stringBuilder.Append(' ');
    if (this.HasRange)
    {
      stringBuilder.Append(this._from.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      stringBuilder.Append('-');
      stringBuilder.Append(this._to.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
    }
    else
      stringBuilder.Append('*');
    stringBuilder.Append('/');
    if (this.HasLength)
      stringBuilder.Append(this._length.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
    else
      stringBuilder.Append('*');
    return stringBuilder.ToString();
  }

  public static ContentRangeHeaderValue Parse(string input)
  {
    int index = 0;
    return (ContentRangeHeaderValue) GenericHeaderParser.ContentRangeParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out ContentRangeHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (ContentRangeHeaderValue) null;
    object parsedValue1;
    if (!GenericHeaderParser.ContentRangeParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (ContentRangeHeaderValue) parsedValue1;
    return true;
  }

  internal static int GetContentRangeLength(string input, int startIndex, out object parsedValue)
  {
    parsedValue = (object) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
    if (tokenLength == 0)
      return 0;
    string unit = input.Substring(startIndex, tokenLength);
    int startIndex1 = startIndex + tokenLength;
    int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, startIndex1);
    if (whitespaceLength == 0)
      return 0;
    int current = startIndex1 + whitespaceLength;
    if (current == input.Length)
      return 0;
    int fromStartIndex = current;
    int fromLength = 0;
    int toStartIndex = 0;
    int toLength = 0;
    if (!ContentRangeHeaderValue.TryGetRangeLength(input, ref current, out fromLength, out toStartIndex, out toLength) || current == input.Length || input[current] != '/')
      return 0;
    ++current;
    current += HttpRuleParser.GetWhitespaceLength(input, current);
    if (current == input.Length)
      return 0;
    int lengthStartIndex = current;
    int lengthLength = 0;
    return !ContentRangeHeaderValue.TryGetLengthLength(input, ref current, out lengthLength) || !ContentRangeHeaderValue.TryCreateContentRange(input, unit, fromStartIndex, fromLength, toStartIndex, toLength, lengthStartIndex, lengthLength, out parsedValue) ? 0 : current - startIndex;
  }

  private static bool TryGetLengthLength(string input, ref int current, out int lengthLength)
  {
    lengthLength = 0;
    if (input[current] == '*')
    {
      ++current;
    }
    else
    {
      lengthLength = HttpRuleParser.GetNumberLength(input, current, false);
      if (lengthLength == 0 || lengthLength > 19)
        return false;
      current += lengthLength;
    }
    current += HttpRuleParser.GetWhitespaceLength(input, current);
    return true;
  }

  private static bool TryGetRangeLength(
    string input,
    ref int current,
    out int fromLength,
    out int toStartIndex,
    out int toLength)
  {
    fromLength = 0;
    toStartIndex = 0;
    toLength = 0;
    if (input[current] == '*')
    {
      ++current;
    }
    else
    {
      fromLength = HttpRuleParser.GetNumberLength(input, current, false);
      if (fromLength == 0 || fromLength > 19)
        return false;
      current += fromLength;
      current += HttpRuleParser.GetWhitespaceLength(input, current);
      if (current == input.Length || input[current] != '-')
        return false;
      ++current;
      current += HttpRuleParser.GetWhitespaceLength(input, current);
      if (current == input.Length)
        return false;
      toStartIndex = current;
      toLength = HttpRuleParser.GetNumberLength(input, current, false);
      if (toLength == 0 || toLength > 19)
        return false;
      current += toLength;
    }
    current += HttpRuleParser.GetWhitespaceLength(input, current);
    return true;
  }

  private static bool TryCreateContentRange(
    string input,
    string unit,
    int fromStartIndex,
    int fromLength,
    int toStartIndex,
    int toLength,
    int lengthStartIndex,
    int lengthLength,
    out object parsedValue)
  {
    parsedValue = (object) null;
    long result1 = 0;
    if (fromLength > 0 && !HeaderUtilities.TryParseInt64(input.Substring(fromStartIndex, fromLength), out result1))
      return false;
    long result2 = 0;
    if (toLength > 0 && !HeaderUtilities.TryParseInt64(input.Substring(toStartIndex, toLength), out result2) || fromLength > 0 && toLength > 0 && result1 > result2)
      return false;
    long result3 = 0;
    if (lengthLength > 0 && !HeaderUtilities.TryParseInt64(input.Substring(lengthStartIndex, lengthLength), out result3) || toLength > 0 && lengthLength > 0 && result2 >= result3)
      return false;
    ContentRangeHeaderValue rangeHeaderValue = new ContentRangeHeaderValue();
    rangeHeaderValue._unit = unit;
    if (fromLength > 0)
    {
      rangeHeaderValue._from = new long?(result1);
      rangeHeaderValue._to = new long?(result2);
    }
    if (lengthLength > 0)
      rangeHeaderValue._length = new long?(result3);
    parsedValue = (object) rangeHeaderValue;
    return true;
  }

  object ICloneable.Clone() => (object) new ContentRangeHeaderValue(this);
}
