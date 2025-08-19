// Decompiled with JetBrains decompiler
// Type: System.Net.Http.FormUrlEncodedContent
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

#nullable disable
namespace System.Net.Http;

public class FormUrlEncodedContent : ByteArrayContent
{
  public FormUrlEncodedContent(
    IEnumerable<KeyValuePair<string, string>> nameValueCollection)
    : base(FormUrlEncodedContent.GetContentByteArray(nameValueCollection))
  {
    this.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
  }

  private static byte[] GetContentByteArray(
    IEnumerable<KeyValuePair<string, string>> nameValueCollection)
  {
    if (nameValueCollection == null)
      throw new ArgumentNullException(nameof (nameValueCollection));
    StringBuilder stringBuilder = new StringBuilder();
    foreach (KeyValuePair<string, string> nameValue in nameValueCollection)
    {
      if (stringBuilder.Length > 0)
        stringBuilder.Append('&');
      stringBuilder.Append(FormUrlEncodedContent.Encode(nameValue.Key));
      stringBuilder.Append('=');
      stringBuilder.Append(FormUrlEncodedContent.Encode(nameValue.Value));
    }
    return HttpRuleParser.DefaultHttpEncoding.GetBytes(stringBuilder.ToString());
  }

  private static string Encode(string data)
  {
    return string.IsNullOrEmpty(data) ? string.Empty : Uri.EscapeDataString(data).Replace("%20", "+");
  }
}
