// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.RangeItemHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers;

public class RangeItemHeaderValue : ICloneable
{
  private long? _from;
  private long? _to;

  public long? From => this._from;

  public long? To => this._to;

  public RangeItemHeaderValue(long? from, long? to)
  {
    if (!from.HasValue && !to.HasValue)
      throw new ArgumentException(SR.net_http_headers_invalid_range);
    if (from.HasValue && from.Value < 0L)
      throw new ArgumentOutOfRangeException(nameof (from));
    if (to.HasValue && to.Value < 0L)
      throw new ArgumentOutOfRangeException(nameof (to));
    if (from.HasValue && to.HasValue && from.Value > to.Value)
      throw new ArgumentOutOfRangeException(nameof (from));
    this._from = from;
    this._to = to;
  }

  private RangeItemHeaderValue(RangeItemHeaderValue source)
  {
    this._from = source._from;
    this._to = source._to;
  }

  public override string ToString()
  {
    if (!this._from.HasValue)
      return "-" + this._to.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
    if (!this._to.HasValue)
      return this._from.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) + "-";
    long num = this._from.Value;
    string str1 = num.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
    num = this._to.Value;
    string str2 = num.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
    return $"{str1}-{str2}";
  }

  public override bool Equals(object obj)
  {
    if (!(obj is RangeItemHeaderValue rangeItemHeaderValue))
      return false;
    long? nullable1 = this._from;
    long? nullable2 = rangeItemHeaderValue._from;
    if ((nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? (nullable1.HasValue == nullable2.HasValue ? 1 : 0) : 0) == 0)
      return false;
    nullable2 = this._to;
    nullable1 = rangeItemHeaderValue._to;
    return nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() && nullable2.HasValue == nullable1.HasValue;
  }

  public override int GetHashCode()
  {
    if (!this._from.HasValue)
      return this._to.GetHashCode();
    return !this._to.HasValue ? this._from.GetHashCode() : this._from.GetHashCode() ^ this._to.GetHashCode();
  }

  internal static int GetRangeItemListLength(
    string input,
    int startIndex,
    ICollection<RangeItemHeaderValue> rangeCollection)
  {
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    bool separatorFound = false;
    int orWhitespaceIndex = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, startIndex, true, out separatorFound);
    if (orWhitespaceIndex == input.Length)
      return 0;
    RangeItemHeaderValue parsedValue = (RangeItemHeaderValue) null;
    do
    {
      int rangeItemLength = RangeItemHeaderValue.GetRangeItemLength(input, orWhitespaceIndex, out parsedValue);
      if (rangeItemLength == 0)
        return 0;
      rangeCollection.Add(parsedValue);
      int startIndex1 = orWhitespaceIndex + rangeItemLength;
      orWhitespaceIndex = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, startIndex1, true, out separatorFound);
      if (orWhitespaceIndex < input.Length && !separatorFound)
        return 0;
    }
    while (orWhitespaceIndex != input.Length);
    return orWhitespaceIndex - startIndex;
  }

  internal static int GetRangeItemLength(
    string input,
    int startIndex,
    out RangeItemHeaderValue parsedValue)
  {
    parsedValue = (RangeItemHeaderValue) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    int startIndex1 = startIndex;
    int startIndex2 = startIndex1;
    int numberLength = HttpRuleParser.GetNumberLength(input, startIndex1, false);
    if (numberLength > 19)
      return 0;
    int startIndex3 = startIndex1 + numberLength;
    int index = startIndex3 + HttpRuleParser.GetWhitespaceLength(input, startIndex3);
    if (index == input.Length || input[index] != '-')
      return 0;
    int startIndex4 = index + 1;
    int startIndex5 = startIndex4 + HttpRuleParser.GetWhitespaceLength(input, startIndex4);
    int startIndex6 = startIndex5;
    int length = 0;
    if (startIndex5 < input.Length)
    {
      length = HttpRuleParser.GetNumberLength(input, startIndex5, false);
      if (length > 19)
        return 0;
      int startIndex7 = startIndex5 + length;
      startIndex5 = startIndex7 + HttpRuleParser.GetWhitespaceLength(input, startIndex7);
    }
    if (numberLength == 0 && length == 0)
      return 0;
    long result1 = 0;
    if (numberLength > 0 && !HeaderUtilities.TryParseInt64(input.Substring(startIndex2, numberLength), out result1))
      return 0;
    long result2 = 0;
    if (length > 0 && !HeaderUtilities.TryParseInt64(input.Substring(startIndex6, length), out result2) || numberLength > 0 && length > 0 && result1 > result2)
      return 0;
    parsedValue = new RangeItemHeaderValue(numberLength == 0 ? new long?() : new long?(result1), length == 0 ? new long?() : new long?(result2));
    return startIndex5 - startIndex;
  }

  object ICloneable.Clone() => (object) new RangeItemHeaderValue(this);
}
