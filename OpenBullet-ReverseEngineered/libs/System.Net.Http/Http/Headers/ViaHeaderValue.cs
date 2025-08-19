// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.ViaHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Globalization;
using System.Text;

#nullable disable
namespace System.Net.Http.Headers;

public class ViaHeaderValue : ICloneable
{
  private string _protocolName;
  private string _protocolVersion;
  private string _receivedBy;
  private string _comment;

  public string ProtocolName => this._protocolName;

  public string ProtocolVersion => this._protocolVersion;

  public string ReceivedBy => this._receivedBy;

  public string Comment => this._comment;

  public ViaHeaderValue(string protocolVersion, string receivedBy)
    : this(protocolVersion, receivedBy, (string) null, (string) null)
  {
  }

  public ViaHeaderValue(string protocolVersion, string receivedBy, string protocolName)
    : this(protocolVersion, receivedBy, protocolName, (string) null)
  {
  }

  public ViaHeaderValue(
    string protocolVersion,
    string receivedBy,
    string protocolName,
    string comment)
  {
    HeaderUtilities.CheckValidToken(protocolVersion, nameof (protocolVersion));
    ViaHeaderValue.CheckReceivedBy(receivedBy);
    if (!string.IsNullOrEmpty(protocolName))
    {
      HeaderUtilities.CheckValidToken(protocolName, nameof (protocolName));
      this._protocolName = protocolName;
    }
    if (!string.IsNullOrEmpty(comment))
    {
      HeaderUtilities.CheckValidComment(comment, nameof (comment));
      this._comment = comment;
    }
    this._protocolVersion = protocolVersion;
    this._receivedBy = receivedBy;
  }

  private ViaHeaderValue()
  {
  }

  private ViaHeaderValue(ViaHeaderValue source)
  {
    this._protocolName = source._protocolName;
    this._protocolVersion = source._protocolVersion;
    this._receivedBy = source._receivedBy;
    this._comment = source._comment;
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (!string.IsNullOrEmpty(this._protocolName))
    {
      stringBuilder.Append(this._protocolName);
      stringBuilder.Append('/');
    }
    stringBuilder.Append(this._protocolVersion);
    stringBuilder.Append(' ');
    stringBuilder.Append(this._receivedBy);
    if (!string.IsNullOrEmpty(this._comment))
    {
      stringBuilder.Append(' ');
      stringBuilder.Append(this._comment);
    }
    return stringBuilder.ToString();
  }

  public override bool Equals(object obj)
  {
    return obj is ViaHeaderValue viaHeaderValue && string.Equals(this._protocolVersion, viaHeaderValue._protocolVersion, StringComparison.OrdinalIgnoreCase) && string.Equals(this._receivedBy, viaHeaderValue._receivedBy, StringComparison.OrdinalIgnoreCase) && string.Equals(this._protocolName, viaHeaderValue._protocolName, StringComparison.OrdinalIgnoreCase) && string.Equals(this._comment, viaHeaderValue._comment, StringComparison.Ordinal);
  }

  public override int GetHashCode()
  {
    int hashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(this._protocolVersion) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(this._receivedBy);
    if (!string.IsNullOrEmpty(this._protocolName))
      hashCode ^= StringComparer.OrdinalIgnoreCase.GetHashCode(this._protocolName);
    if (!string.IsNullOrEmpty(this._comment))
      hashCode ^= this._comment.GetHashCode();
    return hashCode;
  }

  public static ViaHeaderValue Parse(string input)
  {
    int index = 0;
    return (ViaHeaderValue) GenericHeaderParser.SingleValueViaParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out ViaHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (ViaHeaderValue) null;
    object parsedValue1;
    if (!GenericHeaderParser.SingleValueViaParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (ViaHeaderValue) parsedValue1;
    return true;
  }

  internal static int GetViaLength(string input, int startIndex, out object parsedValue)
  {
    parsedValue = (object) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    string protocolName = (string) null;
    string protocolVersion = (string) null;
    int protocolEndIndex = ViaHeaderValue.GetProtocolEndIndex(input, startIndex, out protocolName, out protocolVersion);
    if (protocolEndIndex == startIndex || protocolEndIndex == input.Length)
      return 0;
    string host = (string) null;
    int hostLength = HttpRuleParser.GetHostLength(input, protocolEndIndex, true, out host);
    if (hostLength == 0)
      return 0;
    int startIndex1 = protocolEndIndex + hostLength;
    int num = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
    string str = (string) null;
    if (num < input.Length && input[num] == '(')
    {
      int length = 0;
      if (HttpRuleParser.GetCommentLength(input, num, out length) != HttpParseResult.Parsed)
        return 0;
      str = input.Substring(num, length);
      int startIndex2 = num + length;
      num = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
    }
    parsedValue = (object) new ViaHeaderValue()
    {
      _protocolVersion = protocolVersion,
      _protocolName = protocolName,
      _receivedBy = host,
      _comment = str
    };
    return num - startIndex;
  }

  private static int GetProtocolEndIndex(
    string input,
    int startIndex,
    out string protocolName,
    out string protocolVersion)
  {
    protocolName = (string) null;
    protocolVersion = (string) null;
    int startIndex1 = startIndex;
    int tokenLength1 = HttpRuleParser.GetTokenLength(input, startIndex1);
    if (tokenLength1 == 0)
      return 0;
    int startIndex2 = startIndex + tokenLength1;
    int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, startIndex2);
    int index = startIndex2 + whitespaceLength;
    if (index == input.Length)
      return 0;
    if (input[index] == '/')
    {
      protocolName = input.Substring(startIndex, tokenLength1);
      int startIndex3 = index + 1;
      int startIndex4 = startIndex3 + HttpRuleParser.GetWhitespaceLength(input, startIndex3);
      int tokenLength2 = HttpRuleParser.GetTokenLength(input, startIndex4);
      if (tokenLength2 == 0)
        return 0;
      protocolVersion = input.Substring(startIndex4, tokenLength2);
      int startIndex5 = startIndex4 + tokenLength2;
      whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, startIndex5);
      index = startIndex5 + whitespaceLength;
    }
    else
      protocolVersion = input.Substring(startIndex, tokenLength1);
    return whitespaceLength == 0 ? 0 : index;
  }

  object ICloneable.Clone() => (object) new ViaHeaderValue(this);

  private static void CheckReceivedBy(string receivedBy)
  {
    if (string.IsNullOrEmpty(receivedBy))
      throw new ArgumentException(SR.net_http_argument_empty_string, nameof (receivedBy));
    string host = (string) null;
    if (HttpRuleParser.GetHostLength(receivedBy, 0, true, out host) != receivedBy.Length)
      throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) receivedBy));
  }
}
