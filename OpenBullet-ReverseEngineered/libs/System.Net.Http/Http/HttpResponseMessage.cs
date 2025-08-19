// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpResponseMessage
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

#nullable disable
namespace System.Net.Http;

public class HttpResponseMessage : IDisposable
{
  private const HttpStatusCode defaultStatusCode = HttpStatusCode.OK;
  private HttpStatusCode _statusCode;
  private HttpResponseHeaders _headers;
  private string _reasonPhrase;
  private HttpRequestMessage _requestMessage;
  private Version _version;
  private HttpContent _content;
  private bool _disposed;

  public Version Version
  {
    get => this._version;
    set
    {
      if (value == (Version) null)
        throw new ArgumentNullException(nameof (value));
      this.CheckDisposed();
      this._version = value;
    }
  }

  public HttpContent Content
  {
    get => this._content;
    set
    {
      this.CheckDisposed();
      if (HttpEventSource.Log.IsEnabled())
      {
        if (value == null)
          HttpEventSource.ContentNull((object) this);
        else
          HttpEventSource.Associate((object) this, (object) value);
      }
      this._content = value;
    }
  }

  public HttpStatusCode StatusCode
  {
    get => this._statusCode;
    set
    {
      if (value < (HttpStatusCode) 0 || value > (HttpStatusCode) 999)
        throw new ArgumentOutOfRangeException(nameof (value));
      this.CheckDisposed();
      this._statusCode = value;
    }
  }

  public string ReasonPhrase
  {
    get
    {
      return this._reasonPhrase != null ? this._reasonPhrase : HttpStatusDescription.Get(this.StatusCode);
    }
    set
    {
      if (value != null && this.ContainsNewLineCharacter(value))
        throw new FormatException(SR.net_http_reasonphrase_format_error);
      this.CheckDisposed();
      this._reasonPhrase = value;
    }
  }

  public HttpResponseHeaders Headers
  {
    get
    {
      if (this._headers == null)
        this._headers = new HttpResponseHeaders();
      return this._headers;
    }
  }

  public HttpRequestMessage RequestMessage
  {
    get => this._requestMessage;
    set
    {
      this.CheckDisposed();
      if (HttpEventSource.Log.IsEnabled() && value != null)
        HttpEventSource.Associate((object) this, (object) value);
      this._requestMessage = value;
    }
  }

  public bool IsSuccessStatusCode
  {
    get => this._statusCode >= HttpStatusCode.OK && this._statusCode <= (HttpStatusCode) 299;
  }

  public HttpResponseMessage()
    : this(HttpStatusCode.OK)
  {
  }

  public HttpResponseMessage(HttpStatusCode statusCode)
  {
    if (NetEventSource.Log.IsEnabled())
      NetEventSource.Enter(NetEventSource.ComponentType.Http, (object) this, ".ctor", (object) $"StatusCode: {(object) (int) statusCode}, ReasonPhrase: '{this._reasonPhrase}'");
    this._statusCode = statusCode >= (HttpStatusCode) 0 && statusCode <= (HttpStatusCode) 999 ? statusCode : throw new ArgumentOutOfRangeException(nameof (statusCode));
    this._version = HttpUtilities.DefaultResponseVersion;
    if (!NetEventSource.Log.IsEnabled())
      return;
    NetEventSource.Exit(NetEventSource.ComponentType.Http, (object) this, ".ctor", (object) null);
  }

  public HttpResponseMessage EnsureSuccessStatusCode()
  {
    if (!this.IsSuccessStatusCode)
    {
      if (this._content != null)
        this._content.Dispose();
      throw new HttpRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_message_not_success_statuscode, (object) (int) this._statusCode, (object) this.ReasonPhrase));
    }
    return this;
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("StatusCode: ");
    stringBuilder.Append((int) this._statusCode);
    stringBuilder.Append(", ReasonPhrase: '");
    stringBuilder.Append(this.ReasonPhrase ?? "<null>");
    stringBuilder.Append("', Version: ");
    stringBuilder.Append((object) this._version);
    stringBuilder.Append(", Content: ");
    stringBuilder.Append(this._content == null ? "<null>" : this._content.GetType().ToString());
    stringBuilder.Append(", Headers:\r\n");
    stringBuilder.Append(HeaderUtilities.DumpHeaders((HttpHeaders) this._headers, this._content == null ? (HttpHeaders) null : (HttpHeaders) this._content.Headers));
    return stringBuilder.ToString();
  }

  private bool ContainsNewLineCharacter(string value)
  {
    foreach (char ch in value)
    {
      switch (ch)
      {
        case '\n':
        case '\r':
          return true;
        default:
          continue;
      }
    }
    return false;
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!disposing || this._disposed)
      return;
    this._disposed = true;
    if (this._content == null)
      return;
    this._content.Dispose();
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  private void CheckDisposed()
  {
    if (this._disposed)
      throw new ObjectDisposedException(this.GetType().ToString());
  }
}
