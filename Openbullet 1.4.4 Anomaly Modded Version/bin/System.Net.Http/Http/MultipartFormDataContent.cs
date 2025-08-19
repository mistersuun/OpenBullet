// Decompiled with JetBrains decompiler
// Type: System.Net.Http.MultipartFormDataContent
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Net.Http.Headers;

#nullable disable
namespace System.Net.Http;

public class MultipartFormDataContent : MultipartContent
{
  private const string formData = "form-data";

  public MultipartFormDataContent()
    : base("form-data")
  {
  }

  public MultipartFormDataContent(string boundary)
    : base("form-data", boundary)
  {
  }

  public override void Add(HttpContent content)
  {
    if (content == null)
      throw new ArgumentNullException(nameof (content));
    if (content.Headers.ContentDisposition == null)
      content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
    base.Add(content);
  }

  public void Add(HttpContent content, string name)
  {
    if (content == null)
      throw new ArgumentNullException(nameof (content));
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException(SR.net_http_argument_empty_string, nameof (name));
    this.AddInternal(content, name, (string) null);
  }

  public void Add(HttpContent content, string name, string fileName)
  {
    if (content == null)
      throw new ArgumentNullException(nameof (content));
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException(SR.net_http_argument_empty_string, nameof (name));
    if (string.IsNullOrWhiteSpace(fileName))
      throw new ArgumentException(SR.net_http_argument_empty_string, nameof (fileName));
    this.AddInternal(content, name, fileName);
  }

  private void AddInternal(HttpContent content, string name, string fileName)
  {
    if (content.Headers.ContentDisposition == null)
      content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
      {
        Name = name,
        FileName = fileName,
        FileNameStar = fileName
      };
    base.Add(content);
  }
}
