// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.ByteArrayHeaderParser
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers;

internal class ByteArrayHeaderParser : HttpHeaderParser
{
  internal static readonly ByteArrayHeaderParser Parser = new ByteArrayHeaderParser();

  private ByteArrayHeaderParser()
    : base(false)
  {
  }

  public override string ToString(object value) => Convert.ToBase64String((byte[]) value);

  public override bool TryParseValue(
    string value,
    object storeValue,
    ref int index,
    out object parsedValue)
  {
    parsedValue = (object) null;
    if (string.IsNullOrEmpty(value) || index == value.Length)
      return false;
    string s = value;
    if (index > 0)
      s = value.Substring(index);
    try
    {
      parsedValue = (object) Convert.FromBase64String(s);
      index = value.Length;
      return true;
    }
    catch (FormatException ex)
    {
      if (NetEventSource.Log.IsEnabled())
        NetEventSource.PrintError(NetEventSource.ComponentType.Http, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_parser_invalid_base64_string, (object) s, (object) ex.Message));
    }
    return false;
  }
}
