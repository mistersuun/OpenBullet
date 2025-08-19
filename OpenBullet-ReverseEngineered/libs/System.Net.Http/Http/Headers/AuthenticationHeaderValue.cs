// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.AuthenticationHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

#nullable disable
namespace System.Net.Http.Headers;

public class AuthenticationHeaderValue : ICloneable
{
  private string _scheme;
  private string _parameter;

  public string Scheme => this._scheme;

  public string Parameter => this._parameter;

  public AuthenticationHeaderValue(string scheme)
    : this(scheme, (string) null)
  {
  }

  public AuthenticationHeaderValue(string scheme, string parameter)
  {
    HeaderUtilities.CheckValidToken(scheme, nameof (scheme));
    this._scheme = scheme;
    this._parameter = parameter;
  }

  private AuthenticationHeaderValue(AuthenticationHeaderValue source)
  {
    this._scheme = source._scheme;
    this._parameter = source._parameter;
  }

  private AuthenticationHeaderValue()
  {
  }

  public override string ToString()
  {
    return string.IsNullOrEmpty(this._parameter) ? this._scheme : $"{this._scheme} {this._parameter}";
  }

  public override bool Equals(object obj)
  {
    if (!(obj is AuthenticationHeaderValue authenticationHeaderValue))
      return false;
    if (string.IsNullOrEmpty(this._parameter) && string.IsNullOrEmpty(authenticationHeaderValue._parameter))
      return string.Equals(this._scheme, authenticationHeaderValue._scheme, StringComparison.OrdinalIgnoreCase);
    return string.Equals(this._scheme, authenticationHeaderValue._scheme, StringComparison.OrdinalIgnoreCase) && string.Equals(this._parameter, authenticationHeaderValue._parameter, StringComparison.Ordinal);
  }

  public override int GetHashCode()
  {
    int hashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(this._scheme);
    if (!string.IsNullOrEmpty(this._parameter))
      hashCode ^= this._parameter.GetHashCode();
    return hashCode;
  }

  public static AuthenticationHeaderValue Parse(string input)
  {
    int index = 0;
    return (AuthenticationHeaderValue) GenericHeaderParser.SingleValueAuthenticationParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out AuthenticationHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (AuthenticationHeaderValue) null;
    object parsedValue1;
    if (!GenericHeaderParser.SingleValueAuthenticationParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (AuthenticationHeaderValue) parsedValue1;
    return true;
  }

  internal static int GetAuthenticationLength(string input, int startIndex, out object parsedValue)
  {
    parsedValue = (object) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
    if (tokenLength == 0)
      return 0;
    AuthenticationHeaderValue authenticationHeaderValue = new AuthenticationHeaderValue();
    authenticationHeaderValue._scheme = input.Substring(startIndex, tokenLength);
    int startIndex1 = startIndex + tokenLength;
    int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, startIndex1);
    int index = startIndex1 + whitespaceLength;
    if (index == input.Length || input[index] == ',')
    {
      parsedValue = (object) authenticationHeaderValue;
      return index - startIndex;
    }
    if (whitespaceLength == 0)
      return 0;
    int startIndex2 = index;
    int parameterEndIndex = index;
    if (!AuthenticationHeaderValue.TrySkipFirstBlob(input, ref index, ref parameterEndIndex) || index < input.Length && !AuthenticationHeaderValue.TryGetParametersEndIndex(input, ref index, ref parameterEndIndex))
      return 0;
    authenticationHeaderValue._parameter = input.Substring(startIndex2, parameterEndIndex - startIndex2 + 1);
    parsedValue = (object) authenticationHeaderValue;
    return index - startIndex;
  }

  private static bool TrySkipFirstBlob(string input, ref int current, ref int parameterEndIndex)
  {
    while (current < input.Length && input[current] != ',')
    {
      if (input[current] == '"')
      {
        int length = 0;
        if (HttpRuleParser.GetQuotedStringLength(input, current, out length) != HttpParseResult.Parsed)
          return false;
        current += length;
        parameterEndIndex = current - 1;
      }
      else
      {
        int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, current);
        if (whitespaceLength == 0)
        {
          parameterEndIndex = current;
          ++current;
        }
        else
          current += whitespaceLength;
      }
    }
    return true;
  }

  private static bool TryGetParametersEndIndex(
    string input,
    ref int parseEndIndex,
    ref int parameterEndIndex)
  {
    int index1 = parseEndIndex;
    do
    {
      int startIndex1 = index1 + 1;
      bool separatorFound = false;
      int orWhitespaceIndex = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, startIndex1, true, out separatorFound);
      if (orWhitespaceIndex == input.Length)
        return true;
      int tokenLength = HttpRuleParser.GetTokenLength(input, orWhitespaceIndex);
      if (tokenLength == 0)
        return false;
      int startIndex2 = orWhitespaceIndex + tokenLength;
      int index2 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
      if (index2 == input.Length || input[index2] != '=')
        return true;
      int startIndex3 = index2 + 1;
      int startIndex4 = startIndex3 + HttpRuleParser.GetWhitespaceLength(input, startIndex3);
      int valueLength = NameValueHeaderValue.GetValueLength(input, startIndex4);
      if (valueLength == 0)
        return false;
      int startIndex5 = startIndex4 + valueLength;
      parameterEndIndex = startIndex5 - 1;
      index1 = startIndex5 + HttpRuleParser.GetWhitespaceLength(input, startIndex5);
      parseEndIndex = index1;
    }
    while (index1 < input.Length && input[index1] == ',');
    return true;
  }

  object ICloneable.Clone() => (object) new AuthenticationHeaderValue(this);
}
