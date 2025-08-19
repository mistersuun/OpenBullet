// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.VirtualResponse
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

#nullable disable
namespace AngleSharp.Io;

public class VirtualResponse : IResponse, IDisposable
{
  private Url _address;
  private HttpStatusCode _status;
  private Dictionary<string, string> _headers;
  private Stream _content;
  private bool _dispose;

  private VirtualResponse()
  {
    this._address = Url.Create("http://localhost/");
    this._status = HttpStatusCode.OK;
    this._headers = new Dictionary<string, string>();
    this._content = Stream.Null;
    this._dispose = false;
  }

  public static IResponse Create(Action<VirtualResponse> request)
  {
    VirtualResponse virtualResponse = new VirtualResponse();
    request(virtualResponse);
    return (IResponse) virtualResponse;
  }

  Url IResponse.Address => this._address;

  Stream IResponse.Content => this._content;

  IDictionary<string, string> IResponse.Headers => (IDictionary<string, string>) this._headers;

  HttpStatusCode IResponse.StatusCode => this._status;

  public VirtualResponse Address(Url url)
  {
    this._address = url;
    return this;
  }

  public VirtualResponse Address(string address)
  {
    return this.Address(Url.Create(address ?? string.Empty));
  }

  public VirtualResponse Address(Uri url) => this.Address(Url.Convert(url));

  public VirtualResponse Cookie(string value) => this.Header(HeaderNames.SetCookie, value);

  public VirtualResponse Status(HttpStatusCode code)
  {
    this._status = code;
    return this;
  }

  public VirtualResponse Status(int code) => this.Status((HttpStatusCode) code);

  public VirtualResponse Header(string name, string value)
  {
    this._headers[name] = value;
    return this;
  }

  public VirtualResponse Headers(object obj)
  {
    return this.Headers((IDictionary<string, string>) obj.ToDictionary());
  }

  public VirtualResponse Headers(IDictionary<string, string> headers)
  {
    foreach (KeyValuePair<string, string> header in (IEnumerable<KeyValuePair<string, string>>) headers)
      this.Header(header.Key, header.Value);
    return this;
  }

  public VirtualResponse Content(string text)
  {
    this.Release();
    this._content = (Stream) new MemoryStream(TextEncoding.Utf8.GetBytes(text));
    this._dispose = true;
    return this;
  }

  public VirtualResponse Content(Stream stream, bool shouldDispose = false)
  {
    this.Release();
    this._content = stream;
    this._dispose = shouldDispose;
    return this;
  }

  private void Release()
  {
    if (this._dispose)
      this._content?.Dispose();
    this._dispose = false;
    this._content = (Stream) null;
  }

  void IDisposable.Dispose() => this.Release();
}
