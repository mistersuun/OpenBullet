// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.EntityTagHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

#nullable disable
namespace System.Net.Http.Headers;

public class EntityTagHeaderValue : ICloneable
{
  private static EntityTagHeaderValue s_any;
  private string _tag;
  private bool _isWeak;

  public string Tag => this._tag;

  public bool IsWeak => this._isWeak;

  public static EntityTagHeaderValue Any
  {
    get
    {
      if (EntityTagHeaderValue.s_any == null)
      {
        EntityTagHeaderValue.s_any = new EntityTagHeaderValue();
        EntityTagHeaderValue.s_any._tag = "*";
        EntityTagHeaderValue.s_any._isWeak = false;
      }
      return EntityTagHeaderValue.s_any;
    }
  }

  public EntityTagHeaderValue(string tag)
    : this(tag, false)
  {
  }

  public EntityTagHeaderValue(string tag, bool isWeak)
  {
    if (string.IsNullOrEmpty(tag))
      throw new ArgumentException(SR.net_http_argument_empty_string, nameof (tag));
    int length = 0;
    if (HttpRuleParser.GetQuotedStringLength(tag, 0, out length) != HttpParseResult.Parsed || length != tag.Length)
      throw new FormatException(SR.net_http_headers_invalid_etag_name);
    this._tag = tag;
    this._isWeak = isWeak;
  }

  private EntityTagHeaderValue(EntityTagHeaderValue source)
  {
    this._tag = source._tag;
    this._isWeak = source._isWeak;
  }

  private EntityTagHeaderValue()
  {
  }

  public override string ToString() => this._isWeak ? "W/" + this._tag : this._tag;

  public override bool Equals(object obj)
  {
    return obj is EntityTagHeaderValue entityTagHeaderValue && this._isWeak == entityTagHeaderValue._isWeak && string.Equals(this._tag, entityTagHeaderValue._tag, StringComparison.Ordinal);
  }

  public override int GetHashCode() => this._tag.GetHashCode() ^ this._isWeak.GetHashCode();

  public static EntityTagHeaderValue Parse(string input)
  {
    int index = 0;
    return (EntityTagHeaderValue) GenericHeaderParser.SingleValueEntityTagParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out EntityTagHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (EntityTagHeaderValue) null;
    object parsedValue1;
    if (!GenericHeaderParser.SingleValueEntityTagParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (EntityTagHeaderValue) parsedValue1;
    return true;
  }

  internal static int GetEntityTagLength(
    string input,
    int startIndex,
    out EntityTagHeaderValue parsedValue)
  {
    parsedValue = (EntityTagHeaderValue) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    bool flag = false;
    int startIndex1 = startIndex;
    int startIndex2;
    switch (input[startIndex])
    {
      case '*':
        parsedValue = EntityTagHeaderValue.Any;
        startIndex2 = startIndex1 + 1;
        break;
      case 'W':
      case 'w':
        int index = startIndex1 + 1;
        if (index + 2 >= input.Length || input[index] != '/')
          return 0;
        flag = true;
        int startIndex3 = index + 1;
        startIndex1 = startIndex3 + HttpRuleParser.GetWhitespaceLength(input, startIndex3);
        goto default;
      default:
        int startIndex4 = startIndex1;
        int length = 0;
        if (HttpRuleParser.GetQuotedStringLength(input, startIndex1, out length) != HttpParseResult.Parsed)
          return 0;
        parsedValue = new EntityTagHeaderValue();
        if (length == input.Length)
        {
          parsedValue._tag = input;
          parsedValue._isWeak = false;
        }
        else
        {
          parsedValue._tag = input.Substring(startIndex4, length);
          parsedValue._isWeak = flag;
        }
        startIndex2 = startIndex1 + length;
        break;
    }
    return startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2) - startIndex;
  }

  object ICloneable.Clone()
  {
    return this == EntityTagHeaderValue.s_any ? (object) EntityTagHeaderValue.s_any : (object) new EntityTagHeaderValue(this);
  }
}
