// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.HttpRequest
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using Leaf.xNet.Services.Captcha;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

#nullable disable
namespace Leaf.xNet;

public class HttpRequest : IDisposable
{
  public static Version ProtocolVersion = new Version(1, 1);
  private ProxyClient _currentProxy;
  private int _redirectionCount;
  private int _maximumAutomaticRedirections = 5;
  private int _connectTimeout = 9000;
  private int _readWriteTimeout = 30000;
  private DateTime _whenConnectionIdle;
  private int _keepAliveTimeout = 30000;
  private int _maximumKeepAliveRequests = 100;
  private int _keepAliveRequestCount;
  private bool _keepAliveReconnected;
  private int _reconnectLimit = 3;
  private int _reconnectDelay = 100;
  private int _reconnectCount;
  private HttpMethod _method;
  private HttpContent _content;
  private readonly Dictionary<string, string> _permanentHeaders = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  private Dictionary<string, string> _temporaryHeaders;
  private MultipartContent _temporaryMultipartContent;
  private long _bytesSent;
  private long _totalBytesSent;
  private long _bytesReceived;
  private long _totalBytesReceived;
  private bool _canReportBytesReceived;
  private EventHandler<UploadProgressChangedEventArgs> _uploadProgressChangedHandler;
  private EventHandler<DownloadProgressChangedEventArgs> _downloadProgressChangedHandler;
  private bool _tempAllowAutoRedirect;
  private bool _tempIgnoreProtocolErrors;
  public RemoteCertificateValidationCallback SslCertificateValidatorCallback;

  public static bool DisableProxyForLocalAddress { get; set; }

  public static ProxyClient GlobalProxy { get; set; }

  public event EventHandler<UploadProgressChangedEventArgs> UploadProgressChanged
  {
    add => this._uploadProgressChangedHandler += value;
    remove => this._uploadProgressChangedHandler -= value;
  }

  public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged
  {
    add => this._downloadProgressChangedHandler += value;
    remove => this._downloadProgressChangedHandler -= value;
  }

  public Uri BaseAddress { get; set; }

  public Uri Address { get; private set; }

  public HttpResponse Response { get; private set; }

  public ProxyClient Proxy { get; set; }

  public SslProtocols SslProtocols { get; set; } = (SslProtocols) 4032;

  public bool AllowEmptyHeaderValues { get; set; }

  public bool KeepTemporaryHeadersOnRedirect { get; set; } = true;

  public bool EnableMiddleHeaders { get; set; }

  public string AcceptEncoding { get; set; } = "gzip,deflate";

  public bool AllowAutoRedirect { get; set; }

  public bool ManualMode
  {
    get => !this.AllowAutoRedirect && this.IgnoreProtocolErrors;
    set
    {
      if (value)
      {
        this._tempAllowAutoRedirect = this.AllowAutoRedirect;
        this._tempIgnoreProtocolErrors = this.IgnoreProtocolErrors;
        this.AllowAutoRedirect = false;
        this.IgnoreProtocolErrors = true;
      }
      else
      {
        this.AllowAutoRedirect = this._tempAllowAutoRedirect;
        this.IgnoreProtocolErrors = this._tempIgnoreProtocolErrors;
      }
    }
  }

  public int MaximumAutomaticRedirections
  {
    get => this._maximumAutomaticRedirections;
    set
    {
      this._maximumAutomaticRedirections = value >= 1 ? value : throw ExceptionHelper.CanNotBeLess<int>(nameof (MaximumAutomaticRedirections), 1);
    }
  }

  public bool CookieSingleHeader { get; set; } = true;

  public int ConnectTimeout
  {
    get => this._connectTimeout;
    set
    {
      this._connectTimeout = value >= 0 ? value : throw ExceptionHelper.CanNotBeLess<int>(nameof (ConnectTimeout), 0);
    }
  }

  public int ReadWriteTimeout
  {
    get => this._readWriteTimeout;
    set
    {
      this._readWriteTimeout = value >= 0 ? value : throw ExceptionHelper.CanNotBeLess<int>(nameof (ReadWriteTimeout), 0);
    }
  }

  public bool IgnoreProtocolErrors { get; set; }

  public bool KeepAlive { get; set; }

  public int KeepAliveTimeout
  {
    get => this._keepAliveTimeout;
    set
    {
      this._keepAliveTimeout = value >= 0 ? value : throw ExceptionHelper.CanNotBeLess<int>(nameof (KeepAliveTimeout), 0);
    }
  }

  public int MaximumKeepAliveRequests
  {
    get => this._maximumKeepAliveRequests;
    set
    {
      this._maximumKeepAliveRequests = value >= 1 ? value : throw ExceptionHelper.CanNotBeLess<int>(nameof (MaximumKeepAliveRequests), 1);
    }
  }

  public bool Reconnect { get; set; }

  public int ReconnectLimit
  {
    get => this._reconnectLimit;
    set
    {
      this._reconnectLimit = value >= 1 ? value : throw ExceptionHelper.CanNotBeLess<int>(nameof (ReconnectLimit), 1);
    }
  }

  public int ReconnectDelay
  {
    get => this._reconnectDelay;
    set
    {
      this._reconnectDelay = value >= 0 ? value : throw ExceptionHelper.CanNotBeLess<int>(nameof (ReconnectDelay), 0);
    }
  }

  public CultureInfo Culture { get; set; }

  public Encoding CharacterSet { get; set; }

  public bool EnableEncodingContent { get; set; }

  public string Username { get; set; }

  public string Password { get; set; }

  public string UserAgent
  {
    get => this["User-Agent"];
    set => this["User-Agent"] = value;
  }

  public void UserAgentRandomize() => this.UserAgent = Http.RandomUserAgent();

  public string Referer
  {
    get => this[nameof (Referer)];
    set => this[nameof (Referer)] = value;
  }

  public string Authorization
  {
    get => this[nameof (Authorization)];
    set => this[nameof (Authorization)] = value;
  }

  public CookieStorage Cookies { get; set; }

  public bool UseCookies { get; set; } = true;

  public ICaptchaSolver CaptchaSolver { get; set; }

  internal TcpClient TcpClient { get; private set; }

  internal Stream ClientStream { get; private set; }

  internal NetworkStream ClientNetworkStream { get; private set; }

  public string this[string headerName]
  {
    get
    {
      switch (headerName)
      {
        case null:
          throw new ArgumentNullException(nameof (headerName));
        case "":
          throw ExceptionHelper.EmptyString(nameof (headerName));
        default:
          string empty;
          if (!this._permanentHeaders.TryGetValue(headerName, out empty))
            empty = string.Empty;
          return empty;
      }
    }
    set
    {
      switch (headerName)
      {
        case null:
          throw new ArgumentNullException(nameof (headerName));
        case "":
          throw ExceptionHelper.EmptyString(nameof (headerName));
        default:
          if (string.IsNullOrEmpty(value))
          {
            this._permanentHeaders.Remove(headerName);
            break;
          }
          this._permanentHeaders[headerName] = value;
          break;
      }
    }
  }

  public string this[HttpHeader header]
  {
    get => this[Http.Headers[header]];
    set => this[Http.Headers[header]] = value;
  }

  public HttpRequest() => this.Init();

  public HttpRequest(string baseAddress)
  {
    switch (baseAddress)
    {
      case null:
        throw new ArgumentNullException(nameof (baseAddress));
      case "":
        throw ExceptionHelper.EmptyString(nameof (baseAddress));
      default:
        if (!baseAddress.StartsWith("http"))
          baseAddress = "http://" + baseAddress;
        Uri uri = new Uri(baseAddress);
        this.BaseAddress = uri.IsAbsoluteUri ? uri : throw new ArgumentException(Resources.ArgumentException_OnlyAbsoluteUri, nameof (baseAddress));
        this.Init();
        break;
    }
  }

  public HttpRequest(Uri baseAddress)
  {
    if (baseAddress == (Uri) null)
      throw new ArgumentNullException(nameof (baseAddress));
    this.BaseAddress = baseAddress.IsAbsoluteUri ? baseAddress : throw new ArgumentException(Resources.ArgumentException_OnlyAbsoluteUri, nameof (baseAddress));
    this.Init();
  }

  public HttpResponse Get(string address, RequestParams urlParams = null)
  {
    if (urlParams != null)
      address = new UriBuilder(address)
      {
        Query = urlParams.Query
      }.Uri.AbsoluteUri;
    return this.Raw(HttpMethod.GET, address);
  }

  public HttpResponse Get(Uri address, RequestParams urlParams = null)
  {
    if (urlParams != null)
      address = new UriBuilder(address)
      {
        Query = urlParams.Query
      }.Uri;
    return this.Raw(HttpMethod.GET, address);
  }

  public HttpResponse Options(string address, RequestParams urlParams = null)
  {
    if (urlParams != null)
      address = new UriBuilder(address)
      {
        Query = urlParams.Query
      }.Uri.AbsoluteUri;
    return this.Raw(HttpMethod.OPTIONS, address);
  }

  public HttpResponse Options(Uri address, RequestParams urlParams = null)
  {
    if (urlParams != null)
      address = new UriBuilder(address)
      {
        Query = urlParams.Query
      }.Uri;
    return this.Raw(HttpMethod.OPTIONS, address);
  }

  public HttpResponse Post(string address) => this.Raw(HttpMethod.POST, address);

  public HttpResponse Post(Uri address) => this.Raw(HttpMethod.POST, address);

  public HttpResponse Post(string address, RequestParams reqParams)
  {
    return reqParams != null ? this.Raw(HttpMethod.POST, address, (HttpContent) new FormUrlEncodedContent(reqParams)) : throw new ArgumentNullException(nameof (reqParams));
  }

  public HttpResponse Post(Uri address, RequestParams reqParams)
  {
    return reqParams != null ? this.Raw(HttpMethod.POST, address, (HttpContent) new FormUrlEncodedContent(reqParams)) : throw new ArgumentNullException(nameof (reqParams));
  }

  public HttpResponse Post(string address, string str, string contentType)
  {
    switch (str)
    {
      case null:
        throw new ArgumentNullException(nameof (str));
      case "":
        throw new ArgumentNullException(nameof (str));
      default:
        switch (contentType)
        {
          case null:
            throw new ArgumentNullException(nameof (contentType));
          case "":
            throw new ArgumentNullException(nameof (contentType));
          default:
            StringContent stringContent = new StringContent(str);
            stringContent.ContentType = contentType;
            StringContent content = stringContent;
            return this.Raw(HttpMethod.POST, address, (HttpContent) content);
        }
    }
  }

  public HttpResponse Post(Uri address, string str, string contentType)
  {
    switch (str)
    {
      case null:
        throw new ArgumentNullException(nameof (str));
      case "":
        throw new ArgumentNullException(nameof (str));
      default:
        switch (contentType)
        {
          case null:
            throw new ArgumentNullException(nameof (contentType));
          case "":
            throw new ArgumentNullException(nameof (contentType));
          default:
            StringContent stringContent = new StringContent(str);
            stringContent.ContentType = contentType;
            StringContent content = stringContent;
            return this.Raw(HttpMethod.POST, address, (HttpContent) content);
        }
    }
  }

  public HttpResponse Post(string address, byte[] bytes, string contentType = "application/octet-stream")
  {
    if (bytes == null)
      throw new ArgumentNullException(nameof (bytes));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        BytesContent bytesContent = new BytesContent(bytes);
        bytesContent.ContentType = contentType;
        BytesContent content = bytesContent;
        return this.Raw(HttpMethod.POST, address, (HttpContent) content);
    }
  }

  public HttpResponse Post(Uri address, byte[] bytes, string contentType = "application/octet-stream")
  {
    if (bytes == null)
      throw new ArgumentNullException(nameof (bytes));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        BytesContent bytesContent = new BytesContent(bytes);
        bytesContent.ContentType = contentType;
        BytesContent content = bytesContent;
        return this.Raw(HttpMethod.POST, address, (HttpContent) content);
    }
  }

  public HttpResponse Post(string address, Stream stream, string contentType = "application/octet-stream")
  {
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        StreamContent streamContent = new StreamContent(stream);
        streamContent.ContentType = contentType;
        StreamContent content = streamContent;
        return this.Raw(HttpMethod.POST, address, (HttpContent) content);
    }
  }

  public HttpResponse Post(Uri address, Stream stream, string contentType = "application/octet-stream")
  {
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        StreamContent streamContent = new StreamContent(stream);
        streamContent.ContentType = contentType;
        StreamContent content = streamContent;
        return this.Raw(HttpMethod.POST, address, (HttpContent) content);
    }
  }

  public HttpResponse Post(string address, string path)
  {
    switch (path)
    {
      case null:
        throw new ArgumentNullException(nameof (path));
      case "":
        throw new ArgumentNullException(nameof (path));
      default:
        return this.Raw(HttpMethod.POST, address, (HttpContent) new FileContent(path));
    }
  }

  public HttpResponse Post(Uri address, string path)
  {
    switch (path)
    {
      case null:
        throw new ArgumentNullException(nameof (path));
      case "":
        throw new ArgumentNullException(nameof (path));
      default:
        return this.Raw(HttpMethod.POST, address, (HttpContent) new FileContent(path));
    }
  }

  public HttpResponse Post(string address, HttpContent content)
  {
    return content != null ? this.Raw(HttpMethod.POST, address, content) : throw new ArgumentNullException(nameof (content));
  }

  public HttpResponse Post(Uri address, HttpContent content)
  {
    return content != null ? this.Raw(HttpMethod.POST, address, content) : throw new ArgumentNullException(nameof (content));
  }

  public HttpResponse Raw(HttpMethod method, string address, HttpContent content = null)
  {
    switch (address)
    {
      case null:
        throw new ArgumentNullException(nameof (address));
      case "":
        throw ExceptionHelper.EmptyString(nameof (address));
      default:
        Uri address1 = new Uri(address, UriKind.RelativeOrAbsolute);
        return this.Raw(method, address1, content);
    }
  }

  public HttpResponse Raw(HttpMethod method, Uri address, HttpContent content = null)
  {
    if (address == (Uri) null)
      throw new ArgumentNullException(nameof (address));
    if (!address.IsAbsoluteUri)
      address = HttpRequest.GetRequestAddress(this.BaseAddress, address);
    if (content == null && this._temporaryMultipartContent != null)
      content = (HttpContent) this._temporaryMultipartContent;
    try
    {
      return this.Request(method, address, content);
    }
    finally
    {
      content?.Dispose();
      this.ClearRequestData(false);
    }
  }

  public HttpRequest AddHeader(string name, string value)
  {
    switch (name)
    {
      case null:
        throw new ArgumentNullException(nameof (name));
      case "":
        throw ExceptionHelper.EmptyString(nameof (name));
      default:
        switch (value)
        {
          case null:
            throw new ArgumentNullException(nameof (value));
          case "":
            if (!this.AllowEmptyHeaderValues)
              throw ExceptionHelper.EmptyString(nameof (value));
            break;
        }
        if (this._temporaryHeaders == null)
          this._temporaryHeaders = new Dictionary<string, string>();
        this._temporaryHeaders[name] = value;
        return this;
    }
  }

  public HttpRequest AddXmlHttpRequestHeader()
  {
    return this.AddHeader("X-Requested-With", "XMLHttpRequest");
  }

  public HttpRequest AddHeader(HttpHeader header, string value)
  {
    this.AddHeader(Http.Headers[header], value);
    return this;
  }

  public void Close() => this.Dispose();

  public void Dispose() => this.Dispose(true);

  public bool ContainsCookie(string url, string name)
  {
    return this.UseCookies && this.Cookies != null && this.Cookies.Contains(url, name);
  }

  public bool ContainsHeader(string headerName)
  {
    switch (headerName)
    {
      case null:
        throw new ArgumentNullException(nameof (headerName));
      case "":
        throw ExceptionHelper.EmptyString(nameof (headerName));
      default:
        return this._permanentHeaders.ContainsKey(headerName);
    }
  }

  public bool ContainsHeader(HttpHeader header) => this.ContainsHeader(Http.Headers[header]);

  public Dictionary<string, string>.Enumerator EnumerateHeaders()
  {
    return this._permanentHeaders.GetEnumerator();
  }

  public void ClearAllHeaders() => this._permanentHeaders.Clear();

  public HttpResponse Patch(string address) => this.Raw(HttpMethod.PATCH, address);

  public HttpResponse Patch(Uri address) => this.Raw(HttpMethod.PATCH, address);

  public HttpResponse Patch(string address, RequestParams reqParams)
  {
    return reqParams != null ? this.Raw(HttpMethod.PATCH, address, (HttpContent) new FormUrlEncodedContent(reqParams)) : throw new ArgumentNullException(nameof (reqParams));
  }

  public HttpResponse Patch(Uri address, RequestParams reqParams)
  {
    return reqParams != null ? this.Raw(HttpMethod.PATCH, address, (HttpContent) new FormUrlEncodedContent(reqParams)) : throw new ArgumentNullException(nameof (reqParams));
  }

  public HttpResponse Patch(string address, string str, string contentType)
  {
    switch (str)
    {
      case null:
        throw new ArgumentNullException(nameof (str));
      case "":
        throw new ArgumentNullException(nameof (str));
      default:
        switch (contentType)
        {
          case null:
            throw new ArgumentNullException(nameof (contentType));
          case "":
            throw new ArgumentNullException(nameof (contentType));
          default:
            StringContent stringContent = new StringContent(str);
            stringContent.ContentType = contentType;
            StringContent content = stringContent;
            return this.Raw(HttpMethod.PATCH, address, (HttpContent) content);
        }
    }
  }

  public HttpResponse Patch(Uri address, string str, string contentType)
  {
    switch (str)
    {
      case null:
        throw new ArgumentNullException(nameof (str));
      case "":
        throw new ArgumentNullException(nameof (str));
      default:
        switch (contentType)
        {
          case null:
            throw new ArgumentNullException(nameof (contentType));
          case "":
            throw new ArgumentNullException(nameof (contentType));
          default:
            StringContent stringContent = new StringContent(str);
            stringContent.ContentType = contentType;
            StringContent content = stringContent;
            return this.Raw(HttpMethod.PATCH, address, (HttpContent) content);
        }
    }
  }

  public HttpResponse Patch(string address, byte[] bytes, string contentType = "application/octet-stream")
  {
    if (bytes == null)
      throw new ArgumentNullException(nameof (bytes));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        BytesContent bytesContent = new BytesContent(bytes);
        bytesContent.ContentType = contentType;
        BytesContent content = bytesContent;
        return this.Raw(HttpMethod.PATCH, address, (HttpContent) content);
    }
  }

  public HttpResponse Patch(Uri address, byte[] bytes, string contentType = "application/octet-stream")
  {
    if (bytes == null)
      throw new ArgumentNullException(nameof (bytes));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        BytesContent bytesContent = new BytesContent(bytes);
        bytesContent.ContentType = contentType;
        BytesContent content = bytesContent;
        return this.Raw(HttpMethod.PATCH, address, (HttpContent) content);
    }
  }

  public HttpResponse Patch(string address, Stream stream, string contentType = "application/octet-stream")
  {
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        StreamContent streamContent = new StreamContent(stream);
        streamContent.ContentType = contentType;
        StreamContent content = streamContent;
        return this.Raw(HttpMethod.PATCH, address, (HttpContent) content);
    }
  }

  public HttpResponse Patch(Uri address, Stream stream, string contentType = "application/octet-stream")
  {
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        StreamContent streamContent = new StreamContent(stream);
        streamContent.ContentType = contentType;
        StreamContent content = streamContent;
        return this.Raw(HttpMethod.PATCH, address, (HttpContent) content);
    }
  }

  public HttpResponse Patch(string address, string path)
  {
    switch (path)
    {
      case null:
        throw new ArgumentNullException(nameof (path));
      case "":
        throw new ArgumentNullException(nameof (path));
      default:
        return this.Raw(HttpMethod.PATCH, address, (HttpContent) new FileContent(path));
    }
  }

  public HttpResponse Patch(Uri address, string path)
  {
    switch (path)
    {
      case null:
        throw new ArgumentNullException(nameof (path));
      case "":
        throw new ArgumentNullException(nameof (path));
      default:
        return this.Raw(HttpMethod.PATCH, address, (HttpContent) new FileContent(path));
    }
  }

  public HttpResponse Patch(string address, HttpContent content)
  {
    return content != null ? this.Raw(HttpMethod.PATCH, address, content) : throw new ArgumentNullException(nameof (content));
  }

  public HttpResponse Patch(Uri address, HttpContent content)
  {
    return content != null ? this.Raw(HttpMethod.PATCH, address, content) : throw new ArgumentNullException(nameof (content));
  }

  public HttpResponse Put(string address) => this.Raw(HttpMethod.PUT, address);

  public HttpResponse Put(Uri address) => this.Raw(HttpMethod.PUT, address);

  public HttpResponse Put(string address, RequestParams reqParams)
  {
    return reqParams != null ? this.Raw(HttpMethod.PUT, address, (HttpContent) new FormUrlEncodedContent(reqParams)) : throw new ArgumentNullException(nameof (reqParams));
  }

  public HttpResponse Put(Uri address, RequestParams reqParams)
  {
    return reqParams != null ? this.Raw(HttpMethod.PUT, address, (HttpContent) new FormUrlEncodedContent(reqParams)) : throw new ArgumentNullException(nameof (reqParams));
  }

  public HttpResponse Put(string address, string str, string contentType)
  {
    switch (str)
    {
      case null:
        throw new ArgumentNullException(nameof (str));
      case "":
        throw new ArgumentNullException(nameof (str));
      default:
        switch (contentType)
        {
          case null:
            throw new ArgumentNullException(nameof (contentType));
          case "":
            throw new ArgumentNullException(nameof (contentType));
          default:
            StringContent stringContent = new StringContent(str);
            stringContent.ContentType = contentType;
            StringContent content = stringContent;
            return this.Raw(HttpMethod.PUT, address, (HttpContent) content);
        }
    }
  }

  public HttpResponse Put(Uri address, string str, string contentType)
  {
    switch (str)
    {
      case null:
        throw new ArgumentNullException(nameof (str));
      case "":
        throw new ArgumentNullException(nameof (str));
      default:
        switch (contentType)
        {
          case null:
            throw new ArgumentNullException(nameof (contentType));
          case "":
            throw new ArgumentNullException(nameof (contentType));
          default:
            StringContent stringContent = new StringContent(str);
            stringContent.ContentType = contentType;
            StringContent content = stringContent;
            return this.Raw(HttpMethod.PUT, address, (HttpContent) content);
        }
    }
  }

  public HttpResponse Put(string address, byte[] bytes, string contentType = "application/octet-stream")
  {
    if (bytes == null)
      throw new ArgumentNullException(nameof (bytes));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        BytesContent bytesContent = new BytesContent(bytes);
        bytesContent.ContentType = contentType;
        BytesContent content = bytesContent;
        return this.Raw(HttpMethod.PUT, address, (HttpContent) content);
    }
  }

  public HttpResponse Put(Uri address, byte[] bytes, string contentType = "application/octet-stream")
  {
    if (bytes == null)
      throw new ArgumentNullException(nameof (bytes));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        BytesContent bytesContent = new BytesContent(bytes);
        bytesContent.ContentType = contentType;
        BytesContent content = bytesContent;
        return this.Raw(HttpMethod.PUT, address, (HttpContent) content);
    }
  }

  public HttpResponse Put(string address, Stream stream, string contentType = "application/octet-stream")
  {
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        StreamContent streamContent = new StreamContent(stream);
        streamContent.ContentType = contentType;
        StreamContent content = streamContent;
        return this.Raw(HttpMethod.PUT, address, (HttpContent) content);
    }
  }

  public HttpResponse Put(Uri address, Stream stream, string contentType = "application/octet-stream")
  {
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        StreamContent streamContent = new StreamContent(stream);
        streamContent.ContentType = contentType;
        StreamContent content = streamContent;
        return this.Raw(HttpMethod.PUT, address, (HttpContent) content);
    }
  }

  public HttpResponse Put(string address, string path)
  {
    switch (path)
    {
      case null:
        throw new ArgumentNullException(nameof (path));
      case "":
        throw new ArgumentNullException(nameof (path));
      default:
        return this.Raw(HttpMethod.PUT, address, (HttpContent) new FileContent(path));
    }
  }

  public HttpResponse Put(Uri address, string path)
  {
    switch (path)
    {
      case null:
        throw new ArgumentNullException(nameof (path));
      case "":
        throw new ArgumentNullException(nameof (path));
      default:
        return this.Raw(HttpMethod.PUT, address, (HttpContent) new FileContent(path));
    }
  }

  public HttpResponse Put(string address, HttpContent content)
  {
    return content != null ? this.Raw(HttpMethod.PUT, address, content) : throw new ArgumentNullException(nameof (content));
  }

  public HttpResponse Put(Uri address, HttpContent content)
  {
    return content != null ? this.Raw(HttpMethod.PUT, address, content) : throw new ArgumentNullException(nameof (content));
  }

  public HttpResponse Delete(string address) => this.Raw(HttpMethod.DELETE, address);

  public HttpResponse Delete(Uri address) => this.Raw(HttpMethod.DELETE, address);

  public HttpResponse Delete(string address, RequestParams reqParams)
  {
    return reqParams != null ? this.Raw(HttpMethod.DELETE, address, (HttpContent) new FormUrlEncodedContent(reqParams)) : throw new ArgumentNullException(nameof (reqParams));
  }

  public HttpResponse Delete(Uri address, RequestParams reqParams)
  {
    return reqParams != null ? this.Raw(HttpMethod.DELETE, address, (HttpContent) new FormUrlEncodedContent(reqParams)) : throw new ArgumentNullException(nameof (reqParams));
  }

  public HttpResponse Delete(string address, string str, string contentType)
  {
    switch (str)
    {
      case null:
        throw new ArgumentNullException(nameof (str));
      case "":
        throw new ArgumentNullException(nameof (str));
      default:
        switch (contentType)
        {
          case null:
            throw new ArgumentNullException(nameof (contentType));
          case "":
            throw new ArgumentNullException(nameof (contentType));
          default:
            StringContent stringContent = new StringContent(str);
            stringContent.ContentType = contentType;
            StringContent content = stringContent;
            return this.Raw(HttpMethod.DELETE, address, (HttpContent) content);
        }
    }
  }

  public HttpResponse Delete(Uri address, string str, string contentType)
  {
    switch (str)
    {
      case null:
        throw new ArgumentNullException(nameof (str));
      case "":
        throw new ArgumentNullException(nameof (str));
      default:
        switch (contentType)
        {
          case null:
            throw new ArgumentNullException(nameof (contentType));
          case "":
            throw new ArgumentNullException(nameof (contentType));
          default:
            StringContent stringContent = new StringContent(str);
            stringContent.ContentType = contentType;
            StringContent content = stringContent;
            return this.Raw(HttpMethod.DELETE, address, (HttpContent) content);
        }
    }
  }

  public HttpResponse Delete(string address, byte[] bytes, string contentType = "application/octet-stream")
  {
    if (bytes == null)
      throw new ArgumentNullException(nameof (bytes));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        BytesContent bytesContent = new BytesContent(bytes);
        bytesContent.ContentType = contentType;
        BytesContent content = bytesContent;
        return this.Raw(HttpMethod.DELETE, address, (HttpContent) content);
    }
  }

  public HttpResponse Delete(Uri address, byte[] bytes, string contentType = "application/octet-stream")
  {
    if (bytes == null)
      throw new ArgumentNullException(nameof (bytes));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        BytesContent bytesContent = new BytesContent(bytes);
        bytesContent.ContentType = contentType;
        BytesContent content = bytesContent;
        return this.Raw(HttpMethod.DELETE, address, (HttpContent) content);
    }
  }

  public HttpResponse Delete(string address, Stream stream, string contentType = "application/octet-stream")
  {
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        StreamContent streamContent = new StreamContent(stream);
        streamContent.ContentType = contentType;
        StreamContent content = streamContent;
        return this.Raw(HttpMethod.DELETE, address, (HttpContent) content);
    }
  }

  public HttpResponse Delete(Uri address, Stream stream, string contentType = "application/octet-stream")
  {
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    switch (contentType)
    {
      case null:
        throw new ArgumentNullException(nameof (contentType));
      case "":
        throw new ArgumentNullException(nameof (contentType));
      default:
        StreamContent streamContent = new StreamContent(stream);
        streamContent.ContentType = contentType;
        StreamContent content = streamContent;
        return this.Raw(HttpMethod.DELETE, address, (HttpContent) content);
    }
  }

  public HttpResponse Delete(string address, string path)
  {
    switch (path)
    {
      case null:
        throw new ArgumentNullException(nameof (path));
      case "":
        throw new ArgumentNullException(nameof (path));
      default:
        return this.Raw(HttpMethod.DELETE, address, (HttpContent) new FileContent(path));
    }
  }

  public HttpResponse Delete(Uri address, string path)
  {
    switch (path)
    {
      case null:
        throw new ArgumentNullException(nameof (path));
      case "":
        throw new ArgumentNullException(nameof (path));
      default:
        return this.Raw(HttpMethod.DELETE, address, (HttpContent) new FileContent(path));
    }
  }

  public HttpResponse Delete(string address, HttpContent content)
  {
    return content != null ? this.Raw(HttpMethod.DELETE, address, content) : throw new ArgumentNullException(nameof (content));
  }

  public HttpResponse Delete(Uri address, HttpContent content)
  {
    return content != null ? this.Raw(HttpMethod.DELETE, address, content) : throw new ArgumentNullException(nameof (content));
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!disposing || this.TcpClient == null)
      return;
    this.TcpClient.Close();
    this.TcpClient = (TcpClient) null;
    this.ClientStream?.Dispose();
    this.ClientStream = (Stream) null;
    this.ClientNetworkStream?.Dispose();
    this.ClientNetworkStream = (NetworkStream) null;
    this._keepAliveRequestCount = 0;
  }

  protected virtual void OnUploadProgressChanged(UploadProgressChangedEventArgs e)
  {
    EventHandler<UploadProgressChangedEventArgs> progressChangedHandler = this._uploadProgressChangedHandler;
    if (progressChangedHandler == null)
      return;
    progressChangedHandler((object) this, e);
  }

  protected virtual void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
  {
    EventHandler<DownloadProgressChangedEventArgs> progressChangedHandler = this._downloadProgressChangedHandler;
    if (progressChangedHandler == null)
      return;
    progressChangedHandler((object) this, e);
  }

  private void Init()
  {
    this.KeepAlive = true;
    this.AllowAutoRedirect = true;
    this._tempAllowAutoRedirect = this.AllowAutoRedirect;
    this.EnableEncodingContent = true;
    this.Response = new HttpResponse(this);
  }

  private static Uri GetRequestAddress(Uri baseAddress, Uri address)
  {
    Uri result;
    if (baseAddress == (Uri) null)
      result = new UriBuilder(address.OriginalString).Uri;
    else
      Uri.TryCreate(baseAddress, address, out result);
    return result;
  }

  private HttpResponse Request(HttpMethod method, Uri address, HttpContent content)
  {
    while (true)
    {
      this._method = method;
      this._content = content;
      this.CloseConnectionIfNeeded();
      Uri address1 = this.Address;
      this.Address = address;
      bool connectionOrUseExisting;
      try
      {
        connectionOrUseExisting = this.TryCreateConnectionOrUseExisting(address, address1);
      }
      catch (HttpException ex)
      {
        if (this.CanReconnect)
          return this.ReconnectAfterFail();
        throw;
      }
      if (connectionOrUseExisting)
        this._keepAliveRequestCount = 1;
      else
        ++this._keepAliveRequestCount;
      try
      {
        this.SendRequestData(address, method);
      }
      catch (SecurityException ex)
      {
        throw this.NewHttpException(Resources.HttpException_FailedSendRequest, (Exception) ex, HttpExceptionStatus.SendFailure);
      }
      catch (IOException ex)
      {
        if (this.CanReconnect)
          return this.ReconnectAfterFail();
        throw this.NewHttpException(Resources.HttpException_FailedSendRequest, (Exception) ex, HttpExceptionStatus.SendFailure);
      }
      try
      {
        this.ReceiveResponseHeaders(method);
      }
      catch (HttpException ex)
      {
        if (this.CanReconnect)
          return this.ReconnectAfterFail();
        if (this.KeepAlive && !this._keepAliveReconnected && !connectionOrUseExisting && ex.EmptyMessageBody)
          return this.KeepAliveReconnect();
        throw;
      }
      this.Response.ReconnectCount = this._reconnectCount;
      this._reconnectCount = 0;
      this._keepAliveReconnected = false;
      this._whenConnectionIdle = DateTime.Now;
      if (!this.IgnoreProtocolErrors)
        this.CheckStatusCode(this.Response.StatusCode);
      if (this.AllowAutoRedirect && this.Response.HasRedirect)
      {
        if (++this._redirectionCount <= this._maximumAutomaticRedirections)
        {
          if (!this.Response.HasExternalRedirect)
          {
            this.ClearRequestData(true);
            method = HttpMethod.GET;
            address = this.Response.RedirectAddress;
            content = (HttpContent) null;
          }
          else
            goto label_26;
        }
        else
          break;
      }
      else
        goto label_28;
    }
    throw this.NewHttpException(Resources.HttpException_LimitRedirections);
label_26:
    return this.Response;
label_28:
    this._redirectionCount = 0;
    return this.Response;
  }

  private void CloseConnectionIfNeeded()
  {
    if ((this.TcpClient == null ? 0 : (this.ClientStream != null ? 1 : 0)) == 0 || this.Response.HasError)
      return;
    if (this.Response.MessageBodyLoaded)
      return;
    try
    {
      this.Response.None();
    }
    catch (HttpException ex)
    {
      this.Dispose();
    }
  }

  private bool TryCreateConnectionOrUseExisting(Uri address, Uri previousAddress)
  {
    ProxyClient proxy = this.GetProxy();
    int num = this.TcpClient != null ? 1 : 0;
    bool flag1 = !object.Equals((object) this._currentProxy, (object) proxy);
    bool flag2 = previousAddress == (Uri) null || previousAddress.Port != address.Port || previousAddress.Host != address.Host || previousAddress.Scheme != address.Scheme;
    bool flag3 = this.Response.ContainsHeader("Connection") && this.Response["Connection"] == "close";
    if (num != 0 && !flag1 && !flag2 && !this.Response.HasError && !this.KeepAliveLimitIsReached() && !flag3)
      return false;
    this._currentProxy = proxy;
    this.Dispose();
    this.CreateConnection(address);
    return true;
  }

  private bool KeepAliveLimitIsReached()
  {
    if (!this.KeepAlive)
      return false;
    int? nullable = this.Response.MaximumKeepAliveRequests;
    if (this._keepAliveRequestCount >= (nullable ?? this._maximumKeepAliveRequests))
      return true;
    nullable = this.Response.KeepAliveTimeout;
    return this._whenConnectionIdle.AddMilliseconds((double) (nullable ?? this._keepAliveTimeout)) < DateTime.Now;
  }

  private void SendRequestData(Uri uri, HttpMethod method)
  {
    long contentLength = 0;
    string contentType = string.Empty;
    if (HttpRequest.CanContainsRequestBody(method) && this._content != null)
    {
      contentType = this._content.ContentType;
      contentLength = this._content.CalculateContentLength();
    }
    string startingLine = this.GenerateStartingLine(method);
    string headers = this.GenerateHeaders(uri, method, contentLength, contentType);
    byte[] bytes1 = Encoding.ASCII.GetBytes(startingLine);
    byte[] bytes2 = Encoding.ASCII.GetBytes(headers);
    this._bytesSent = 0L;
    this._totalBytesSent = (long) (bytes1.Length + bytes2.Length) + contentLength;
    this.ClientStream.Write(bytes1, 0, bytes1.Length);
    this.ClientStream.Write(bytes2, 0, bytes2.Length);
    if ((this._content == null ? 0 : (contentLength > 0L ? 1 : 0)) == 0)
      return;
    this._content.WriteTo(this.ClientStream);
  }

  private void ReceiveResponseHeaders(HttpMethod method)
  {
    this._canReportBytesReceived = false;
    this._bytesReceived = 0L;
    this._totalBytesReceived = this.Response.LoadResponse(method, this.EnableMiddleHeaders);
    this._canReportBytesReceived = true;
  }

  private bool CanReconnect => this.Reconnect && this._reconnectCount < this._reconnectLimit;

  private HttpResponse ReconnectAfterFail()
  {
    this.Dispose();
    Thread.Sleep(this._reconnectDelay);
    ++this._reconnectCount;
    return this.Request(this._method, this.Address, this._content);
  }

  private HttpResponse KeepAliveReconnect()
  {
    this.Dispose();
    this._keepAliveReconnected = true;
    return this.Request(this._method, this.Address, this._content);
  }

  private void CheckStatusCode(HttpStatusCode statusCode)
  {
    int num = (int) statusCode;
    if (num >= 400 && num < 500)
      throw new HttpException(string.Format(Resources.HttpException_ClientError, (object) num), HttpExceptionStatus.ProtocolError, this.Response.StatusCode);
    if (num >= 500)
      throw new HttpException(string.Format(Resources.HttpException_SeverError, (object) num), HttpExceptionStatus.ProtocolError, this.Response.StatusCode);
  }

  private static bool CanContainsRequestBody(HttpMethod method)
  {
    return method == HttpMethod.POST || method == HttpMethod.PUT || method == HttpMethod.PATCH || method == HttpMethod.DELETE;
  }

  private ProxyClient GetProxy()
  {
    if (!HttpRequest.DisableProxyForLocalAddress)
      return this.Proxy ?? HttpRequest.GlobalProxy;
    try
    {
      IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
      foreach (object hostAddress in Dns.GetHostAddresses(this.Address.Host))
      {
        if (hostAddress.Equals((object) ipAddress))
          return (ProxyClient) null;
      }
    }
    catch (Exception ex)
    {
      switch (ex)
      {
        case SocketException _:
        case ArgumentException _:
          throw this.NewHttpException(Resources.HttpException_FailedGetHostAddresses, ex);
        default:
          throw;
      }
    }
    return this.Proxy ?? HttpRequest.GlobalProxy;
  }

  private TcpClient CreateTcpConnection(string host, int port)
  {
    TcpClient tcpClient;
    if (this._currentProxy == null)
    {
      tcpClient = new TcpClient();
      Exception connectException = (Exception) null;
      ManualResetEventSlim connectDoneEvent = new ManualResetEventSlim();
      try
      {
        tcpClient.BeginConnect(host, port, (AsyncCallback) (ar =>
        {
          try
          {
            tcpClient.EndConnect(ar);
          }
          catch (Exception ex)
          {
            connectException = ex;
          }
          connectDoneEvent.Set();
        }), (object) tcpClient);
      }
      catch (Exception ex)
      {
        tcpClient.Close();
        switch (ex)
        {
          case SocketException _:
          case SecurityException _:
            throw this.NewHttpException(Resources.HttpException_FailedConnect, ex, HttpExceptionStatus.ConnectFailure);
          default:
            throw;
        }
      }
      if (!connectDoneEvent.Wait(this._connectTimeout))
      {
        tcpClient.Close();
        throw this.NewHttpException(Resources.HttpException_ConnectTimeout, status: HttpExceptionStatus.ConnectFailure);
      }
      if (connectException != null)
      {
        tcpClient.Close();
        if (connectException is SocketException)
          throw this.NewHttpException(Resources.HttpException_FailedConnect, connectException, HttpExceptionStatus.ConnectFailure);
        throw connectException;
      }
      if (!tcpClient.Connected)
      {
        tcpClient.Close();
        throw this.NewHttpException(Resources.HttpException_FailedConnect, status: HttpExceptionStatus.ConnectFailure);
      }
      tcpClient.SendTimeout = this._readWriteTimeout;
      tcpClient.ReceiveTimeout = this._readWriteTimeout;
    }
    else
    {
      try
      {
        tcpClient = this._currentProxy.CreateConnection(host, port);
      }
      catch (ProxyException ex)
      {
        throw this.NewHttpException(Resources.HttpException_FailedConnect, (Exception) ex, HttpExceptionStatus.ConnectFailure);
      }
    }
    return tcpClient;
  }

  private void CreateConnection(Uri address)
  {
    this.TcpClient = this.CreateTcpConnection(address.Host, address.Port);
    this.ClientNetworkStream = this.TcpClient.GetStream();
    if (address.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
    {
      try
      {
        SslStream sslStream = this.SslCertificateValidatorCallback == null ? new SslStream((Stream) this.ClientNetworkStream, false, Http.AcceptAllCertificationsCallback) : new SslStream((Stream) this.ClientNetworkStream, false, this.SslCertificateValidatorCallback);
        sslStream.AuthenticateAsClient(address.Host, new X509CertificateCollection(), this.SslProtocols, false);
        this.ClientStream = (Stream) sslStream;
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case IOException _:
          case AuthenticationException _:
            throw this.NewHttpException(Resources.HttpException_FailedSslConnect, ex, HttpExceptionStatus.ConnectFailure);
          default:
            throw;
        }
      }
    }
    else
      this.ClientStream = (Stream) this.ClientNetworkStream;
    if (this._uploadProgressChangedHandler == null && this._downloadProgressChangedHandler == null)
      return;
    HttpRequest.HttpWrapperStream httpWrapperStream = new HttpRequest.HttpWrapperStream(this.ClientStream, this.TcpClient.SendBufferSize);
    if (this._uploadProgressChangedHandler != null)
      httpWrapperStream.BytesWriteCallback = new Action<int>(this.ReportBytesSent);
    if (this._downloadProgressChangedHandler != null)
      httpWrapperStream.BytesReadCallback = new Action<int>(this.ReportBytesReceived);
    this.ClientStream = (Stream) httpWrapperStream;
  }

  private string GenerateStartingLine(HttpMethod method)
  {
    string str = this._currentProxy == null || this._currentProxy.Type != ProxyType.HTTP || !this._currentProxy.AbsoluteUriInStartingLine ? this.Address.PathAndQuery : this.Address.AbsoluteUri;
    return $"{method} {str} HTTP/{HttpRequest.ProtocolVersion}\r\n";
  }

  private string GenerateHeaders(
    Uri uri,
    HttpMethod method,
    long contentLength = 0,
    string contentType = null)
  {
    Dictionary<string, string> commonHeaders = this.GenerateCommonHeaders(method, contentLength, contentType);
    HttpRequest.MergeHeaders((IDictionary<string, string>) commonHeaders, this._permanentHeaders);
    if (this._temporaryHeaders != null && this._temporaryHeaders.Count > 0)
      HttpRequest.MergeHeaders((IDictionary<string, string>) commonHeaders, this._temporaryHeaders);
    if (!this.UseCookies)
      return this.ToHeadersString(commonHeaders);
    if (this.Cookies == null)
    {
      this.Cookies = new CookieStorage();
      return this.ToHeadersString(commonHeaders);
    }
    if (this.Cookies.Count == 0 || commonHeaders.ContainsKey("Cookie"))
      return this.ToHeadersString(commonHeaders);
    string cookieHeader = this.Cookies.GetCookieHeader(uri);
    if (!string.IsNullOrEmpty(cookieHeader))
      commonHeaders["Cookie"] = cookieHeader;
    return this.ToHeadersString(commonHeaders);
  }

  private Dictionary<string, string> GenerateCommonHeaders(
    HttpMethod method,
    long contentLength = 0,
    string contentType = null)
  {
    Dictionary<string, string> commonHeaders = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      ["Host"] = this.Address.IsDefaultPort ? this.Address.Host : $"{this.Address.Host}:{this.Address.Port}"
    };
    HttpProxyClient httpProxy = (HttpProxyClient) null;
    if (this._currentProxy != null && this._currentProxy.Type == ProxyType.HTTP)
      httpProxy = this._currentProxy as HttpProxyClient;
    if (httpProxy != null)
    {
      commonHeaders["Proxy-Connection"] = this.KeepAlive ? "keep-alive" : "close";
      if (!string.IsNullOrEmpty(httpProxy.Username) || !string.IsNullOrEmpty(httpProxy.Password))
        commonHeaders["Proxy-Authorization"] = HttpRequest.GetProxyAuthorizationHeader((ProxyClient) httpProxy);
    }
    else
      commonHeaders["Connection"] = this.KeepAlive ? "keep-alive" : "close";
    if (!string.IsNullOrEmpty(this.Username) || !string.IsNullOrEmpty(this.Password))
      commonHeaders["Authorization"] = this.GetAuthorizationHeader();
    if (this.EnableEncodingContent)
      commonHeaders["Accept-Encoding"] = this.AcceptEncoding;
    if (this.Culture != null)
      commonHeaders["Accept-Language"] = this.GetLanguageHeader();
    if (this.CharacterSet != null)
      commonHeaders["Accept-Charset"] = this.GetCharsetHeader();
    if (!HttpRequest.CanContainsRequestBody(method))
      return commonHeaders;
    if (contentLength > 0L)
      commonHeaders["Content-Type"] = contentType;
    commonHeaders["Content-Length"] = contentLength.ToString();
    return commonHeaders;
  }

  private string GetAuthorizationHeader()
  {
    return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{this.Username}:{this.Password}"));
  }

  private static string GetProxyAuthorizationHeader(ProxyClient httpProxy)
  {
    return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{httpProxy.Username}:{httpProxy.Password}"));
  }

  private string GetLanguageHeader()
  {
    string str = this.Culture?.Name ?? CultureInfo.CurrentCulture.Name;
    return !str.StartsWith("en") ? $"{str},{str.Substring(0, 2)};q=0.8,en-US;q=0.6,en;q=0.4" : str;
  }

  private string GetCharsetHeader()
  {
    return object.Equals((object) this.CharacterSet, (object) Encoding.UTF8) ? "utf-8;q=0.7,*;q=0.3" : (this.CharacterSet?.WebName ?? Encoding.Default.WebName) + ",utf-8;q=0.7,*;q=0.3";
  }

  private static void MergeHeaders(
    IDictionary<string, string> destination,
    Dictionary<string, string> source)
  {
    foreach (KeyValuePair<string, string> keyValuePair in source)
      destination[keyValuePair.Key] = keyValuePair.Value;
  }

  private string ToHeadersString(Dictionary<string, string> headers)
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (KeyValuePair<string, string> header in headers)
    {
      if (header.Key != "Cookie" || this.CookieSingleHeader)
      {
        stringBuilder.AppendFormat("{0}: {1}\r\n", (object) header.Key, (object) header.Value);
      }
      else
      {
        string str1 = header.Value;
        string[] separator = new string[1]{ "; " };
        foreach (string str2 in str1.Split(separator, StringSplitOptions.None))
          stringBuilder.AppendFormat("Cookie: {0}\r\n", (object) str2);
      }
    }
    stringBuilder.AppendLine();
    return stringBuilder.ToString();
  }

  private void ReportBytesSent(int bytesSent)
  {
    this._bytesSent += (long) bytesSent;
    this.OnUploadProgressChanged(new UploadProgressChangedEventArgs(this._bytesSent, this._totalBytesSent));
  }

  private void ReportBytesReceived(int bytesReceived)
  {
    this._bytesReceived += (long) bytesReceived;
    if (!this._canReportBytesReceived)
      return;
    this.OnDownloadProgressChanged(new DownloadProgressChangedEventArgs(this._bytesReceived, this._totalBytesReceived));
  }

  private void ClearRequestData(bool redirect)
  {
    this._content = (HttpContent) null;
    this._temporaryMultipartContent = (MultipartContent) null;
    if (redirect && this.KeepTemporaryHeadersOnRedirect)
      return;
    this._temporaryHeaders = (Dictionary<string, string>) null;
  }

  private HttpException NewHttpException(
    string message,
    Exception innerException = null,
    HttpExceptionStatus status = HttpExceptionStatus.Other)
  {
    return new HttpException(string.Format(message, (object) this.Address.Host), status, innerException: innerException);
  }

  private sealed class HttpWrapperStream : Stream
  {
    private readonly Stream _baseStream;
    private readonly int _sendBufferSize;

    public Action<int> BytesReadCallback { private get; set; }

    public Action<int> BytesWriteCallback { private get; set; }

    public override bool CanRead => this._baseStream.CanRead;

    public override bool CanSeek => this._baseStream.CanSeek;

    public override bool CanTimeout => this._baseStream.CanTimeout;

    public override bool CanWrite => this._baseStream.CanWrite;

    public override long Length => this._baseStream.Length;

    public override long Position
    {
      get => this._baseStream.Position;
      set => this._baseStream.Position = value;
    }

    public HttpWrapperStream(Stream baseStream, int sendBufferSize)
    {
      this._baseStream = baseStream;
      this._sendBufferSize = sendBufferSize;
    }

    public override void Flush()
    {
    }

    public override void SetLength(long value) => this._baseStream.SetLength(value);

    public override long Seek(long offset, SeekOrigin origin)
    {
      return this._baseStream.Seek(offset, origin);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int num = this._baseStream.Read(buffer, offset, count);
      Action<int> bytesReadCallback = this.BytesReadCallback;
      if (bytesReadCallback != null)
        bytesReadCallback(num);
      return num;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this.BytesWriteCallback == null)
      {
        this._baseStream.Write(buffer, offset, count);
      }
      else
      {
        int offset1 = 0;
        while (count > 0)
        {
          int count1;
          if (count >= this._sendBufferSize)
          {
            count1 = this._sendBufferSize;
            this._baseStream.Write(buffer, offset1, count1);
            offset1 += this._sendBufferSize;
            count -= this._sendBufferSize;
          }
          else
          {
            count1 = count;
            this._baseStream.Write(buffer, offset1, count1);
            count = 0;
          }
          this.BytesWriteCallback(count1);
        }
      }
    }
  }
}
