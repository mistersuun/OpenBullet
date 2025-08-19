// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.MediaTypeHeaderValue
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers;

public class MediaTypeHeaderValue : ICloneable
{
  private const string charSet = "charset";
  private ObjectCollection<NameValueHeaderValue> _parameters;
  private string _mediaType;

  public string CharSet
  {
    get => NameValueHeaderValue.Find(this._parameters, "charset")?.Value;
    set
    {
      NameValueHeaderValue valueHeaderValue = NameValueHeaderValue.Find(this._parameters, "charset");
      if (string.IsNullOrEmpty(value))
      {
        if (valueHeaderValue == null)
          return;
        this._parameters.Remove(valueHeaderValue);
      }
      else if (valueHeaderValue != null)
        valueHeaderValue.Value = value;
      else
        this.Parameters.Add(new NameValueHeaderValue("charset", value));
    }
  }

  public ICollection<NameValueHeaderValue> Parameters
  {
    get
    {
      if (this._parameters == null)
        this._parameters = new ObjectCollection<NameValueHeaderValue>();
      return (ICollection<NameValueHeaderValue>) this._parameters;
    }
  }

  public string MediaType
  {
    get => this._mediaType;
    set
    {
      MediaTypeHeaderValue.CheckMediaTypeFormat(value, nameof (value));
      this._mediaType = value;
    }
  }

  internal MediaTypeHeaderValue()
  {
  }

  protected MediaTypeHeaderValue(MediaTypeHeaderValue source)
  {
    this._mediaType = source._mediaType;
    if (source._parameters == null)
      return;
    foreach (ICloneable parameter in source._parameters)
      this.Parameters.Add((NameValueHeaderValue) parameter.Clone());
  }

  public MediaTypeHeaderValue(string mediaType)
  {
    MediaTypeHeaderValue.CheckMediaTypeFormat(mediaType, nameof (mediaType));
    this._mediaType = mediaType;
  }

  public override string ToString()
  {
    return this._mediaType + NameValueHeaderValue.ToString(this._parameters, ';', true);
  }

  public override bool Equals(object obj)
  {
    return obj is MediaTypeHeaderValue mediaTypeHeaderValue && string.Equals(this._mediaType, mediaTypeHeaderValue._mediaType, StringComparison.OrdinalIgnoreCase) && HeaderUtilities.AreEqualCollections<NameValueHeaderValue>(this._parameters, mediaTypeHeaderValue._parameters);
  }

  public override int GetHashCode()
  {
    return StringComparer.OrdinalIgnoreCase.GetHashCode(this._mediaType) ^ NameValueHeaderValue.GetHashCode(this._parameters);
  }

  public static MediaTypeHeaderValue Parse(string input)
  {
    int index = 0;
    return (MediaTypeHeaderValue) MediaTypeHeaderParser.SingleValueParser.ParseValue(input, (object) null, ref index);
  }

  public static bool TryParse(string input, out MediaTypeHeaderValue parsedValue)
  {
    int index = 0;
    parsedValue = (MediaTypeHeaderValue) null;
    object parsedValue1;
    if (!MediaTypeHeaderParser.SingleValueParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
      return false;
    parsedValue = (MediaTypeHeaderValue) parsedValue1;
    return true;
  }

  internal static int GetMediaTypeLength(
    string input,
    int startIndex,
    Func<MediaTypeHeaderValue> mediaTypeCreator,
    out MediaTypeHeaderValue parsedValue)
  {
    parsedValue = (MediaTypeHeaderValue) null;
    if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
      return 0;
    string mediaType = (string) null;
    int expressionLength = MediaTypeHeaderValue.GetMediaTypeExpressionLength(input, startIndex, out mediaType);
    if (expressionLength == 0)
      return 0;
    int startIndex1 = startIndex + expressionLength;
    int index = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
    if (index < input.Length && input[index] == ';')
    {
      MediaTypeHeaderValue mediaTypeHeaderValue = mediaTypeCreator();
      mediaTypeHeaderValue._mediaType = mediaType;
      int startIndex2 = index + 1;
      int nameValueListLength = NameValueHeaderValue.GetNameValueListLength(input, startIndex2, ';', (ObjectCollection<NameValueHeaderValue>) mediaTypeHeaderValue.Parameters);
      if (nameValueListLength == 0)
        return 0;
      parsedValue = mediaTypeHeaderValue;
      return startIndex2 + nameValueListLength - startIndex;
    }
    MediaTypeHeaderValue mediaTypeHeaderValue1 = mediaTypeCreator();
    mediaTypeHeaderValue1._mediaType = mediaType;
    parsedValue = mediaTypeHeaderValue1;
    return index - startIndex;
  }

  private static int GetMediaTypeExpressionLength(
    string input,
    int startIndex,
    out string mediaType)
  {
    mediaType = (string) null;
    int tokenLength1 = HttpRuleParser.GetTokenLength(input, startIndex);
    if (tokenLength1 == 0)
      return 0;
    int startIndex1 = startIndex + tokenLength1;
    int index = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
    if (index >= input.Length || input[index] != '/')
      return 0;
    int startIndex2 = index + 1;
    int startIndex3 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
    int tokenLength2 = HttpRuleParser.GetTokenLength(input, startIndex3);
    if (tokenLength2 == 0)
      return 0;
    int length = startIndex3 + tokenLength2 - startIndex;
    mediaType = tokenLength1 + tokenLength2 + 1 != length ? $"{input.Substring(startIndex, tokenLength1)}/{input.Substring(startIndex3, tokenLength2)}" : input.Substring(startIndex, length);
    return length;
  }

  private static void CheckMediaTypeFormat(string mediaType, string parameterName)
  {
    if (string.IsNullOrEmpty(mediaType))
      throw new ArgumentException(SR.net_http_argument_empty_string, parameterName);
    string mediaType1;
    if (MediaTypeHeaderValue.GetMediaTypeExpressionLength(mediaType, 0, out mediaType1) == 0 || mediaType1.Length != mediaType.Length)
      throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) mediaType));
  }

  object ICloneable.Clone() => (object) new MediaTypeHeaderValue(this);
}
