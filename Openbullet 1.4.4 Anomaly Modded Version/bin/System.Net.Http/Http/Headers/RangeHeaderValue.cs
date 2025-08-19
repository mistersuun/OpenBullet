// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.RangeHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections.Generic;
using System.Text;

#nullable disable
namespace System.Net.Http.Headers;

public class RangeHeaderValue : ICloneable
{
  private string _unit;
  private ObjectCollection<RangeItemHeaderValue> _ranges;

  public string Unit
  {
    get => this._unit;
    set
    {
      HeaderUtilities.CheckValidToken(value, nameof (value));
      this._unit = value;
    }
  }

  public ICollection<RangeItemHeaderValue> Ranges
  {
    get
    {
      if (this._ranges == null)
        this._ranges = new ObjectCollection<RangeItemHeaderValue>();
      return (ICollection<RangeItemHeaderValue>) this._ranges;
    }
  }

  public RangeHeaderValue() => this._unit = "bytes";

  public RangeHeaderValue(long? from, long? to)
  {
    this._unit = "bytes";
    this.Ranges.Add(new RangeItemHeaderValue(from, to));
  }

  private RangeHeaderValue(RangeHeaderValue source)
  {
    this._unit = source._unit;
    if (source._ranges == null)
      return;
    foreach (ICloneable range in source._ranges)
      this.Ranges.Add((RangeItemHeaderValue) range.Clone());
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder(this._unit);
    stringBuilder.Append('=');
    if (this._ranges != null)
    {
      bool flag = true;
      foreach (RangeItemHeaderValue range in this._ranges)
      {
        if (flag)
          flag = false;
        else
          stringBuilder.Append(", ");
        stringBuilder.Append((object) range.From);
        stringBuilder.Append('-');
        stringBuilder.Append((object) range.To);
      }
    }
    return stringBuilder.ToString();
  }

  public override bool Equals(object obj)
  {
    return obj is RangeHeaderValue rangeHeaderValue && string.Equals(this._unit, rangeHeaderValue._unit, StringComparison.OrdinalIgnoreCase) && HeaderUtilities.AreEqualCollections<RangeItemHeaderValue>(this._ranges, rangeHeaderValue._ranges);
  }

  public override int GetHashCode()
  {
    int hashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(this._unit);
    if (this._ranges != null)
    {
      foreach (RangeItemHeaderValue range in this._ranges)
        hashCode ^= range.GetHashCode();
    }
    return hashCode;
  }

  public static RangeHeaderValue Parse(string input)
  {
    int index = 0;
    return (RangeHeaderValue) GenericHeaderParser.RangeParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out RangeHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (RangeHeaderValue) null;
    object parsedValue1;
    if (!GenericHeaderParser.RangeParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (RangeHeaderValue) parsedValue1;
    return true;
  }

  internal static int GetRangeLength(string input, int startIndex, out object parsedValue)
  {
    parsedValue = (object) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
    if (tokenLength == 0)
      return 0;
    RangeHeaderValue rangeHeaderValue = new RangeHeaderValue();
    rangeHeaderValue._unit = input.Substring(startIndex, tokenLength);
    int startIndex1 = startIndex + tokenLength;
    int index = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
    if (index == input.Length || input[index] != '=')
      return 0;
    int startIndex2 = index + 1;
    int startIndex3 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
    int rangeItemListLength = RangeItemHeaderValue.GetRangeItemListLength(input, startIndex3, rangeHeaderValue.Ranges);
    if (rangeItemListLength == 0)
      return 0;
    int num = startIndex3 + rangeItemListLength;
    parsedValue = (object) rangeHeaderValue;
    return num - startIndex;
  }

  object ICloneable.Clone() => (object) new RangeHeaderValue(this);
}
