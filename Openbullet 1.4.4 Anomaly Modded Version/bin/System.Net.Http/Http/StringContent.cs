// Decompiled with JetBrains decompiler
// Type: System.Net.Http.StringContent
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Net.Http.Headers;
using System.Text;

#nullable disable
namespace System.Net.Http;

public class StringContent : ByteArrayContent
{
  private const string defaultMediaType = "text/plain";

  public StringContent(string content)
    : this(content, (Encoding) null, (string) null)
  {
  }

  public StringContent(string content, Encoding encoding)
    : this(content, encoding, (string) null)
  {
  }

  public StringContent(string content, Encoding encoding, string mediaType)
    : base(StringContent.GetContentByteArray(content, encoding))
  {
    this.Headers.ContentType = new MediaTypeHeaderValue(mediaType == null ? "text/plain" : mediaType)
    {
      CharSet = encoding == null ? HttpContent.DefaultStringEncoding.WebName : encoding.WebName
    };
  }

  private static byte[] GetContentByteArray(string content, Encoding encoding)
  {
    if (content == null)
      throw new ArgumentNullException(nameof (content));
    if (encoding == null)
      encoding = HttpContent.DefaultStringEncoding;
    return encoding.GetBytes(content);
  }
}
