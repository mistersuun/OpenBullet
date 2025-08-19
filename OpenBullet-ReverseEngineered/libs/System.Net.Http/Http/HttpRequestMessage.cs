// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpRequestMessage
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

#nullable disable
namespace System.Net.Http;

public class HttpRequestMessage : IDisposable
{
  private const int MessageNotYetSent = 0;
  private const int MessageAlreadySent = 1;
  private int _sendStatus;
  private HttpMethod _method;
  private Uri _requestUri;
  private HttpRequestHeaders _headers;
  private Version _version;
  private HttpContent _content;
  private bool _disposed;
  private IDictionary<string, object> _properties;

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

  public HttpMethod Method
  {
    get => this._method;
    set
    {
      if (value == (HttpMethod) null)
        throw new ArgumentNullException(nameof (value));
      this.CheckDisposed();
      this._method = value;
    }
  }

  public Uri RequestUri
  {
    get => this._requestUri;
    set
    {
      if (value != (Uri) null && value.IsAbsoluteUri && !HttpUtilities.IsHttpUri(value))
        throw new ArgumentException(SR.net_http_client_http_baseaddress_required, nameof (value));
      this.CheckDisposed();
      this._requestUri = value;
    }
  }

  public HttpRequestHeaders Headers
  {
    get
    {
      if (this._headers == null)
        this._headers = new HttpRequestHeaders();
      return this._headers;
    }
  }

  public IDictionary<string, object> Properties
  {
    get
    {
      if (this._properties == null)
        this._properties = (IDictionary<string, object>) new Dictionary<string, object>();
      return this._properties;
    }
  }

  public HttpRequestMessage()
    : this(HttpMethod.Get, (Uri) null)
  {
  }

  public HttpRequestMessage(HttpMethod method, Uri requestUri)
  {
    if (NetEventSource.Log.IsEnabled())
      NetEventSource.Enter(NetEventSource.ComponentType.Http, (object) this, ".ctor", (object) $"Method: {(object) method}, Uri: '{(object) requestUri}'");
    this.InitializeValues(method, requestUri);
    if (!NetEventSource.Log.IsEnabled())
      return;
    NetEventSource.Exit(NetEventSource.ComponentType.Http, (object) this, ".ctor", (object) null);
  }

  public HttpRequestMessage(HttpMethod method, string requestUri)
  {
    if (NetEventSource.Log.IsEnabled())
      NetEventSource.Enter(NetEventSource.ComponentType.Http, (object) this, ".ctor", (object) $"Method: {(object) method}, Uri: '{requestUri}'");
    if (string.IsNullOrEmpty(requestUri))
      this.InitializeValues(method, (Uri) null);
    else
      this.InitializeValues(method, new Uri(requestUri, UriKind.RelativeOrAbsolute));
    if (!NetEventSource.Log.IsEnabled())
      return;
    NetEventSource.Exit(NetEventSource.ComponentType.Http, (object) this, ".ctor", (object) null);
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("Method: ");
    stringBuilder.Append((object) this._method);
    stringBuilder.Append(", RequestUri: '");
    stringBuilder.Append(this._requestUri == (Uri) null ? "<null>" : this._requestUri.ToString());
    stringBuilder.Append("', Version: ");
    stringBuilder.Append((object) this._version);
    stringBuilder.Append(", Content: ");
    stringBuilder.Append(this._content == null ? "<null>" : this._content.GetType().ToString());
    stringBuilder.Append(", Headers:\r\n");
    stringBuilder.Append(HeaderUtilities.DumpHeaders((HttpHeaders) this._headers, this._content == null ? (HttpHeaders) null : (HttpHeaders) this._content.Headers));
    return stringBuilder.ToString();
  }

  private void InitializeValues(HttpMethod method, Uri requestUri)
  {
    if (method == (HttpMethod) null)
      throw new ArgumentNullException(nameof (method));
    if (requestUri != (Uri) null && requestUri.IsAbsoluteUri && !HttpUtilities.IsHttpUri(requestUri))
      throw new ArgumentException(SR.net_http_client_http_baseaddress_required, nameof (requestUri));
    this._method = method;
    this._requestUri = requestUri;
    this._version = HttpUtilities.DefaultRequestVersion;
  }

  internal bool MarkAsSent() => Interlocked.Exchange(ref this._sendStatus, 1) == 0;

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
