// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.UriHeaderParser
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Text;

#nullable disable
namespace System.Net.Http.Headers;

internal class UriHeaderParser : HttpHeaderParser
{
  private UriKind _uriKind;
  internal static readonly UriHeaderParser RelativeOrAbsoluteUriParser = new UriHeaderParser(UriKind.RelativeOrAbsolute);

  private UriHeaderParser(UriKind uriKind)
    : base(false)
  {
    this._uriKind = uriKind;
  }

  public override bool TryParseValue(
    string value,
    object storeValue,
    ref int index,
    out object parsedValue)
  {
    parsedValue = (object) null;
    if (string.IsNullOrEmpty(value) || index == value.Length)
      return false;
    string str = value;
    if (index > 0)
      str = value.Substring(index);
    Uri result;
    if (!Uri.TryCreate(str, this._uriKind, out result) && !Uri.TryCreate(UriHeaderParser.DecodeUtf8FromString(str), this._uriKind, out result))
      return false;
    index = value.Length;
    parsedValue = (object) result;
    return true;
  }

  internal static string DecodeUtf8FromString(string input)
  {
    if (string.IsNullOrWhiteSpace(input))
      return input;
    bool flag = false;
    for (int index = 0; index < input.Length; ++index)
    {
      if (input[index] > 'ÿ')
        return input;
      if (input[index] > '\u007F')
      {
        flag = true;
        break;
      }
    }
    if (flag)
    {
      byte[] bytes = new byte[input.Length];
      for (int index = 0; index < input.Length; ++index)
      {
        if (input[index] > 'ÿ')
          return input;
        bytes[index] = (byte) input[index];
      }
      try
      {
        return Encoding.GetEncoding("utf-8", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback).GetString(bytes, 0, bytes.Length);
      }
      catch (ArgumentException ex)
      {
      }
    }
    return input;
  }

  public override string ToString(object value)
  {
    Uri uri = (Uri) value;
    return uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
  }
}
