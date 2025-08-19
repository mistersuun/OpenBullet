// Decompiled with JetBrains decompiler
// Type: Extreme.Net.HttpRequest
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Extreme.Net;

public class HttpRequest : IDisposable
{
  public static readonly Version ProtocolVersion = new Version(1, 1);
  private static readonly List<string> _closedHeaders = new List<string>()
  {
    "Content-Length",
    "Proxy-Connection",
    "Host"
  };
  private HttpResponse _response;
  private TcpClient _connection;
  private Stream _connectionCommonStream;
  private NetworkStream _connectionNetworkStream;
  private ProxyClient _currentProxy;
  private int _redirectionCount = 0;
  private int _maximumAutomaticRedirections = 5;
  private int _connectTimeout = 60000;
  private int _readWriteTimeout = 60000;
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
  private RequestParams _temporaryParams;
  private RequestParams _temporaryUrlParams;
  private Dictionary<string, string> _temporaryHeaders;
  private MultipartContent _temporaryMultipartContent;
  private long _bytesSent;
  private long _totalBytesSent;
  private long _bytesReceived;
  private long _totalBytesReceived;
  private bool _canReportBytesReceived;
  private EventHandler<UploadProgressChangedEventArgs> _uploadProgressChangedHandler;
  private EventHandler<DownloadProgressChangedEventArgs> _downloadProgressChangedHandler;
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

  public HttpResponse Response => this._response;

  public ProxyClient Proxy { get; set; }

  public bool AllowAutoRedirect { get; set; }

  public int MaximumAutomaticRedirections
  {
    get => this._maximumAutomaticRedirections;
    set
    {
      this._maximumAutomaticRedirections = value >= 1 ? value : throw ExceptionHelper.CanNotBeLess<int>(nameof (MaximumAutomaticRedirections), 1);
    }
  }

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

  public CookieDictionary Cookies { get; set; }

  internal TcpClient TcpClient => this._connection;

  internal Stream ClientStream => this._connectionCommonStream;

  internal NetworkStream ClientNetworkStream => this._connectionNetworkStream;

  private MultipartContent AddedMultipartData
  {
    get
    {
      if (this._temporaryMultipartContent == null)
        this._temporaryMultipartContent = new MultipartContent();
      return this._temporaryMultipartContent;
    }
  }

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
          if (this.IsClosedHeader(headerName))
            throw new ArgumentException(string.Format(Resources.ArgumentException_HttpRequest_SetNotAvailableHeader, (object) headerName), nameof (headerName));
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
      this._temporaryUrlParams = urlParams;
    return this.Raw(HttpMethod.GET, address);
  }

  public HttpResponse Get(Uri address, RequestParams urlParams = null)
  {
    if (urlParams != null)
      this._temporaryUrlParams = urlParams;
    return this.Raw(HttpMethod.GET, address);
  }

  public async Task<HttpResponse> GetAsync(string address, RequestParams urlParams = null)
  {
    HttpResponse async = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Get(address, urlParams)));
    return async;
  }

  public async Task<HttpResponse> GetAsync(Uri address, RequestParams urlParams = null)
  {
    HttpResponse async = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Get(address, urlParams)));
    return async;
  }

  public HttpResponse Post(string address) => this.Raw(HttpMethod.POST, address);

  public async Task<HttpResponse> PostAsync(string address)
  {
    HttpResponse httpResponse = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Post(address)));
    return httpResponse;
  }

  public HttpResponse Post(Uri address) => this.Raw(HttpMethod.POST, address);

  public async Task<HttpResponse> PostAsync(Uri address)
  {
    HttpResponse httpResponse = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Post(address)));
    return httpResponse;
  }

  public HttpResponse Post(string address, RequestParams reqParams, bool dontEscape = false)
  {
    if (reqParams == null)
      throw new ArgumentNullException(nameof (reqParams));
    return this.Raw(HttpMethod.POST, address, (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) reqParams, dontEscape, this.CharacterSet));
  }

  public async Task<HttpResponse> PostAsync(
    string address,
    RequestParams reqParams,
    bool dontEscape = false)
  {
    HttpResponse httpResponse = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Post(address, reqParams, dontEscape)));
    return httpResponse;
  }

  public HttpResponse Post(Uri address, RequestParams reqParams, bool dontEscape = false)
  {
    if (reqParams == null)
      throw new ArgumentNullException(nameof (reqParams));
    return this.Raw(HttpMethod.POST, address, (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) reqParams, dontEscape, this.CharacterSet));
  }

  public async Task<HttpResponse> PostAsync(Uri address, RequestParams reqParams, bool dontEscape = false)
  {
    HttpResponse httpResponse = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Post(address, reqParams, dontEscape)));
    return httpResponse;
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

  public async Task<HttpResponse> PostAsync(string address, string str, string contentType)
  {
    HttpResponse httpResponse = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Post(address, str, contentType)));
    return httpResponse;
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

  public async Task<HttpResponse> PostAsync(Uri address, string str, string contentType)
  {
    HttpResponse httpResponse = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Post(address, str, contentType)));
    return httpResponse;
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

  public async Task<HttpResponse> PostAsync(string address, byte[] bytes, string contentType = "application/octet-stream")
  {
    HttpResponse httpResponse = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Post(address, bytes, contentType)));
    return httpResponse;
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

  public async Task<HttpResponse> PostAsync(Uri address, byte[] bytes, string contentType = "application/octet-stream")
  {
    HttpResponse httpResponse = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Post(address, bytes, contentType)));
    return httpResponse;
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

  public async Task<HttpResponse> PostAsync(string address, Stream stream, string contentType = "application/octet-stream")
  {
    HttpResponse httpResponse = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Post(address, stream, contentType)));
    return httpResponse;
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

  public async Task<HttpResponse> PostAsync(Uri address, Stream stream, string contentType = "application/octet-stream")
  {
    HttpResponse httpResponse = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Post(address, stream, contentType)));
    return httpResponse;
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

  public async Task<HttpResponse> PostAsync(string address, string path)
  {
    HttpResponse httpResponse = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Post(address, path)));
    return httpResponse;
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

  public async Task<HttpResponse> PostAsync(Uri address, string path)
  {
    HttpResponse httpResponse = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Post(address, path)));
    return httpResponse;
  }

  public HttpResponse Post(string address, HttpContent content)
  {
    return content != null ? this.Raw(HttpMethod.POST, address, content) : throw new ArgumentNullException(nameof (content));
  }

  public async Task<HttpResponse> PostAsync(string address, HttpContent content)
  {
    HttpResponse httpResponse = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Post(address, content)));
    return httpResponse;
  }

  public HttpResponse Post(Uri address, HttpContent content)
  {
    return content != null ? this.Raw(HttpMethod.POST, address, content) : throw new ArgumentNullException(nameof (content));
  }

  public async Task<HttpResponse> PostAsync(Uri address, HttpContent content)
  {
    HttpResponse httpResponse = await Task.Run<HttpResponse>((Func<HttpResponse>) (() => this.Post(address, content)));
    return httpResponse;
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
      address = this.GetRequestAddress(this.BaseAddress, address);
    if (this._temporaryUrlParams != null)
      address = new UriBuilder(address)
      {
        Query = Http.ToQueryString((IEnumerable<KeyValuePair<string, string>>) this._temporaryUrlParams, true)
      }.Uri;
    if (content == null)
    {
      if (this._temporaryParams != null)
        content = (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) this._temporaryParams, encoding: this.CharacterSet);
      else if (this._temporaryMultipartContent != null)
        content = (HttpContent) this._temporaryMultipartContent;
    }
    try
    {
      return this.Request(method, address, content);
    }
    finally
    {
      content?.Dispose();
      this.ClearRequestData();
    }
  }

  public HttpRequest AddUrlParam(string name, object value = null)
  {
    switch (name)
    {
      case null:
        throw new ArgumentNullException(nameof (name));
      case "":
        throw ExceptionHelper.EmptyString(nameof (name));
      default:
        if (this._temporaryUrlParams == null)
          this._temporaryUrlParams = new RequestParams();
        this._temporaryUrlParams[name] = value;
        return this;
    }
  }

  public HttpRequest AddParam(string name, object value = null)
  {
    switch (name)
    {
      case null:
        throw new ArgumentNullException(nameof (name));
      case "":
        throw ExceptionHelper.EmptyString(nameof (name));
      default:
        if (this._temporaryParams == null)
          this._temporaryParams = new RequestParams();
        this._temporaryParams[name] = value;
        return this;
    }
  }

  public HttpRequest AddField(string name, object value = null)
  {
    return this.AddField(name, value, this.CharacterSet ?? Encoding.UTF8);
  }

  public HttpRequest AddField(string name, object value, Encoding encoding)
  {
    switch (name)
    {
      case null:
        throw new ArgumentNullException(nameof (name));
      case "":
        throw ExceptionHelper.EmptyString(nameof (name));
      default:
        if (encoding == null)
          throw new ArgumentNullException(nameof (encoding));
        this.AddedMultipartData.Add((HttpContent) new StringContent(value == null ? string.Empty : value.ToString(), encoding), name);
        return this;
    }
  }

  public HttpRequest AddField(string name, byte[] value)
  {
    switch (name)
    {
      case null:
        throw new ArgumentNullException(nameof (name));
      case "":
        throw ExceptionHelper.EmptyString(nameof (name));
      default:
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        this.AddedMultipartData.Add((HttpContent) new BytesContent(value), name);
        return this;
    }
  }

  public HttpRequest AddFile(string name, string fileName, byte[] value)
  {
    switch (name)
    {
      case null:
        throw new ArgumentNullException(nameof (name));
      case "":
        throw ExceptionHelper.EmptyString(nameof (name));
      default:
        if (fileName == null)
          throw new ArgumentNullException(nameof (fileName));
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        this.AddedMultipartData.Add((HttpContent) new BytesContent(value), name, fileName);
        return this;
    }
  }

  public HttpRequest AddFile(string name, string fileName, string contentType, byte[] value)
  {
    switch (name)
    {
      case null:
        throw new ArgumentNullException(nameof (name));
      case "":
        throw ExceptionHelper.EmptyString(nameof (name));
      default:
        if (fileName == null)
          throw new ArgumentNullException(nameof (fileName));
        if (contentType == null)
          throw new ArgumentNullException(nameof (contentType));
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        this.AddedMultipartData.Add((HttpContent) new BytesContent(value), name, fileName, contentType);
        return this;
    }
  }

  public HttpRequest AddFile(string name, string fileName, Stream stream)
  {
    switch (name)
    {
      case null:
        throw new ArgumentNullException(nameof (name));
      case "":
        throw ExceptionHelper.EmptyString(nameof (name));
      default:
        if (fileName == null)
          throw new ArgumentNullException(nameof (fileName));
        if (stream == null)
          throw new ArgumentNullException(nameof (stream));
        this.AddedMultipartData.Add((HttpContent) new StreamContent(stream), name, fileName);
        return this;
    }
  }

  public HttpRequest AddFile(string name, string fileName, string path)
  {
    switch (name)
    {
      case null:
        throw new ArgumentNullException(nameof (name));
      case "":
        throw ExceptionHelper.EmptyString(nameof (name));
      default:
        if (fileName == null)
          throw new ArgumentNullException(nameof (fileName));
        switch (path)
        {
          case null:
            throw new ArgumentNullException(nameof (path));
          case "":
            throw ExceptionHelper.EmptyString(nameof (path));
          default:
            this.AddedMultipartData.Add((HttpContent) new FileContent(path), name, fileName);
            return this;
        }
    }
  }

  public HttpRequest AddFile(string name, string path)
  {
    switch (name)
    {
      case null:
        throw new ArgumentNullException(nameof (name));
      case "":
        throw ExceptionHelper.EmptyString(nameof (name));
      default:
        switch (path)
        {
          case null:
            throw new ArgumentNullException(nameof (path));
          case "":
            throw ExceptionHelper.EmptyString(nameof (path));
          default:
            this.AddedMultipartData.Add((HttpContent) new FileContent(path), name, Path.GetFileName(path));
            return this;
        }
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
            throw ExceptionHelper.EmptyString(nameof (value));
          default:
            if (this.IsClosedHeader(name))
              throw new ArgumentException(string.Format(Resources.ArgumentException_HttpRequest_SetNotAvailableHeader, (object) name), nameof (name));
            if (this._temporaryHeaders == null)
              this._temporaryHeaders = new Dictionary<string, string>();
            this._temporaryHeaders[name] = value;
            return this;
        }
    }
  }

  public HttpRequest AddHeader(HttpHeader header, string value)
  {
    this.AddHeader(Http.Headers[header], value);
    return this;
  }

  public void Close() => this.Dispose();

  public void Dispose() => this.Dispose(true);

  public bool ContainsCookie(string name) => this.Cookies != null && this.Cookies.ContainsKey(name);

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

  protected virtual void Dispose(bool disposing)
  {
    if (!disposing || this._connection == null)
      return;
    this._connection.Close();
    this._connection = (TcpClient) null;
    this._connectionCommonStream = (Stream) null;
    this._connectionNetworkStream = (NetworkStream) null;
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
    this.EnableEncodingContent = true;
    this._response = new HttpResponse(this);
  }

  private Uri GetRequestAddress(Uri baseAddress, Uri address)
  {
    Uri result = address;
    if (baseAddress == (Uri) null)
      result = new UriBuilder(address.OriginalString).Uri;
    else
      Uri.TryCreate(baseAddress, address, out result);
    return result;
  }

  private HttpResponse Request(HttpMethod method, Uri address, HttpContent content)
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
      if (this.CanReconnect())
        return this.ReconnectAfterFail();
      throw;
    }
    if (connectionOrUseExisting)
      this._keepAliveRequestCount = 1;
    else
      ++this._keepAliveRequestCount;
    try
    {
      this.SendRequestData(method);
    }
    catch (SecurityException ex)
    {
      throw this.NewHttpException(Resources.HttpException_FailedSendRequest, (Exception) ex, HttpExceptionStatus.SendFailure);
    }
    catch (IOException ex)
    {
      if (this.CanReconnect())
        return this.ReconnectAfterFail();
      throw this.NewHttpException(Resources.HttpException_FailedSendRequest, (Exception) ex, HttpExceptionStatus.SendFailure);
    }
    try
    {
      this.ReceiveResponseHeaders(method);
    }
    catch (HttpException ex)
    {
      if (this.CanReconnect())
        return this.ReconnectAfterFail();
      if (this.KeepAlive && !this._keepAliveReconnected && !connectionOrUseExisting && ex.EmptyMessageBody)
        return this.KeepAliveReconect();
      throw;
    }
    this._response.ReconnectCount = this._reconnectCount;
    this._reconnectCount = 0;
    this._keepAliveReconnected = false;
    this._whenConnectionIdle = DateTime.Now;
    if (!this.IgnoreProtocolErrors)
      this.CheckStatusCode(this._response.StatusCode);
    if (this.AllowAutoRedirect && this._response.HasRedirect)
    {
      if (++this._redirectionCount > this._maximumAutomaticRedirections)
        throw this.NewHttpException(Resources.HttpException_LimitRedirections);
      this.ClearRequestData(false);
      return this.Request(HttpMethod.GET, this._response.RedirectAddress, (HttpContent) null);
    }
    this._redirectionCount = 0;
    return this._response;
  }

  private void CloseConnectionIfNeeded()
  {
    if (this._connection == null || this._response.HasError || this._response.MessageBodyLoaded)
      return;
    try
    {
      this._response.None();
    }
    catch (HttpException ex)
    {
      this.Dispose();
    }
  }

  private bool TryCreateConnectionOrUseExisting(Uri address, Uri previousAddress)
  {
    ProxyClient proxy = this.GetProxy();
    if (!(this._connection == null | this._currentProxy != proxy | (previousAddress == (Uri) null || previousAddress.Port != address.Port || previousAddress.Host != address.Host || previousAddress.Scheme != address.Scheme)) && !this._response.HasError && !this.KeepAliveLimitIsReached())
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
    int? nullable = this._response.MaximumKeepAliveRequests;
    if (this._keepAliveRequestCount >= (nullable ?? this._maximumKeepAliveRequests))
      return true;
    nullable = this._response.KeepAliveTimeout;
    return this._whenConnectionIdle.AddMilliseconds((double) (nullable ?? this._keepAliveTimeout)) < DateTime.Now;
  }

  private void SendRequestData(HttpMethod method)
  {
    long contentLength = 0;
    string contentType = string.Empty;
    if (this.CanContainsRequestBody(method) && this._content != null)
    {
      contentType = this._content.ContentType;
      contentLength = this._content.CalculateContentLength();
    }
    string startingLine = this.GenerateStartingLine(method);
    string headers = this.GenerateHeaders(method, contentLength, contentType);
    byte[] bytes1 = Encoding.ASCII.GetBytes(startingLine);
    byte[] bytes2 = Encoding.ASCII.GetBytes(headers);
    this._bytesSent = 0L;
    this._totalBytesSent = (long) (bytes1.Length + bytes2.Length) + contentLength;
    this._connectionCommonStream.Write(bytes1, 0, bytes1.Length);
    this._connectionCommonStream.Write(bytes2, 0, bytes2.Length);
    if (this._content == null || contentLength <= 0L)
      return;
    this._content.WriteTo(this._connectionCommonStream);
  }

  private void ReceiveResponseHeaders(HttpMethod method)
  {
    this._canReportBytesReceived = false;
    this._bytesReceived = 0L;
    this._totalBytesReceived = this._response.LoadResponse(method);
    this._canReportBytesReceived = true;
  }

  private bool CanReconnect() => this.Reconnect && this._reconnectCount < this._reconnectLimit;

  private HttpResponse ReconnectAfterFail()
  {
    this.Dispose();
    Thread.Sleep(this._reconnectDelay);
    ++this._reconnectCount;
    return this.Request(this._method, this.Address, this._content);
  }

  private HttpResponse KeepAliveReconect()
  {
    this.Dispose();
    this._keepAliveReconnected = true;
    return this.Request(this._method, this.Address, this._content);
  }

  private void CheckStatusCode(HttpStatusCode statusCode)
  {
    int num = (int) statusCode;
    if (num >= 400 && num < 500)
      throw new HttpException(string.Format(Resources.HttpException_ClientError, (object) num), HttpExceptionStatus.ProtocolError, this._response.StatusCode);
    if (num >= 500)
      throw new HttpException(string.Format(Resources.HttpException_SeverError, (object) num), HttpExceptionStatus.ProtocolError, this._response.StatusCode);
  }

  private bool CanContainsRequestBody(HttpMethod method)
  {
    return method == HttpMethod.PUT || method == HttpMethod.POST || method == HttpMethod.DELETE;
  }

  private ProxyClient GetProxy()
  {
    if (HttpRequest.DisableProxyForLocalAddress)
    {
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
        if (ex is SocketException || ex is ArgumentException)
          throw this.NewHttpException(Resources.HttpException_FailedGetHostAddresses, ex);
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
        if (ex is SocketException || ex is SecurityException)
          throw this.NewHttpException(Resources.HttpException_FailedConnect, ex, HttpExceptionStatus.ConnectFailure);
        throw;
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
    this._connection = this.CreateTcpConnection(address.Host, address.Port);
    this._connectionNetworkStream = this._connection.GetStream();
    if (address.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
    {
      try
      {
        SslStream sslStream = this.SslCertificateValidatorCallback != null ? new SslStream((Stream) this._connectionNetworkStream, false, this.SslCertificateValidatorCallback) : new SslStream((Stream) this._connectionNetworkStream, false, Http.AcceptAllCertificationsCallback);
        sslStream.AuthenticateAsClient(address.Host);
        this._connectionCommonStream = (Stream) sslStream;
      }
      catch (Exception ex)
      {
        if (ex is IOException || ex is AuthenticationException)
          throw this.NewHttpException(Resources.HttpException_FailedSslConnect, ex, HttpExceptionStatus.ConnectFailure);
        throw;
      }
    }
    else
      this._connectionCommonStream = (Stream) this._connectionNetworkStream;
    if (this._uploadProgressChangedHandler == null && this._downloadProgressChangedHandler == null)
      return;
    HttpRequest.HttpWraperStream httpWraperStream = new HttpRequest.HttpWraperStream(this._connectionCommonStream, this._connection.SendBufferSize);
    if (this._uploadProgressChangedHandler != null)
      httpWraperStream.BytesWriteCallback = new Action<int>(this.ReportBytesSent);
    if (this._downloadProgressChangedHandler != null)
      httpWraperStream.BytesReadCallback = new Action<int>(this.ReportBytesReceived);
    this._connectionCommonStream = (Stream) httpWraperStream;
  }

  private string GenerateStartingLine(HttpMethod method)
  {
    string str = this._currentProxy == null || this._currentProxy.Type != ProxyType.Http && this._currentProxy.Type != ProxyType.Chain ? this.Address.PathAndQuery : this.Address.AbsoluteUri;
    return $"{method} {str} HTTP/{HttpRequest.ProtocolVersion}\r\n";
  }

  private string GenerateHeaders(HttpMethod method, long contentLength = 0, string contentType = null)
  {
    Dictionary<string, string> commonHeaders = this.GenerateCommonHeaders(method, contentLength, contentType);
    this.MergeHeaders(commonHeaders, this._permanentHeaders);
    if (this._temporaryHeaders != null && this._temporaryHeaders.Count > 0)
      this.MergeHeaders(commonHeaders, this._temporaryHeaders);
    if (this.Cookies != null && this.Cookies.Count != 0 && !commonHeaders.ContainsKey("Cookie"))
      commonHeaders["Cookie"] = this.Cookies.ToString();
    return this.ToHeadersString(commonHeaders);
  }

  private Dictionary<string, string> GenerateCommonHeaders(
    HttpMethod method,
    long contentLength = 0,
    string contentType = null)
  {
    Dictionary<string, string> commonHeaders = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    commonHeaders["Host"] = !this.Address.IsDefaultPort ? $"{this.Address.Host}:{this.Address.Port}" : this.Address.Host;
    HttpProxyClient httpProxy = (HttpProxyClient) null;
    if (this._currentProxy != null && this._currentProxy.Type == ProxyType.Http)
      httpProxy = this._currentProxy as HttpProxyClient;
    else if (this._currentProxy != null && this._currentProxy.Type == ProxyType.Chain)
      httpProxy = this.FindHttpProxyInChain(this._currentProxy as ChainProxyClient);
    if (httpProxy != null)
    {
      commonHeaders["Proxy-Connection"] = !this.KeepAlive ? "close" : "keep-alive";
      if (!string.IsNullOrEmpty(httpProxy.Username) || !string.IsNullOrEmpty(httpProxy.Password))
        commonHeaders["Proxy-Authorization"] = this.GetProxyAuthorizationHeader(httpProxy);
    }
    else
      commonHeaders["Connection"] = !this.KeepAlive ? "close" : "keep-alive";
    if (!string.IsNullOrEmpty(this.Username) || !string.IsNullOrEmpty(this.Password))
      commonHeaders["Authorization"] = this.GetAuthorizationHeader();
    if (this.EnableEncodingContent)
      commonHeaders["Accept-Encoding"] = "gzip,deflate";
    if (this.Culture != null)
      commonHeaders["Accept-Language"] = this.GetLanguageHeader();
    if (this.CharacterSet != null)
      commonHeaders["Accept-Charset"] = this.GetCharsetHeader();
    if (this.CanContainsRequestBody(method))
    {
      commonHeaders["Content-Type"] = contentType;
      commonHeaders["Content-Length"] = contentLength.ToString();
    }
    return commonHeaders;
  }

  private string GetAuthorizationHeader()
  {
    return $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{this.Username}:{this.Password}"))}";
  }

  private string GetProxyAuthorizationHeader(HttpProxyClient httpProxy)
  {
    return $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{httpProxy.Username}:{httpProxy.Password}"))}";
  }

  private string GetLanguageHeader()
  {
    string str = this.Culture == null ? CultureInfo.CurrentCulture.Name : this.Culture.Name;
    return str.StartsWith("en") ? str : $"{str},{str.Substring(0, 2)};q=0.8,en-US;q=0.6,en;q=0.4";
  }

  private string GetCharsetHeader()
  {
    return this.CharacterSet == Encoding.UTF8 ? "utf-8;q=0.7,*;q=0.3" : $"{(this.CharacterSet != null ? (object) this.CharacterSet.WebName : (object) Encoding.Default.WebName)},utf-8;q=0.7,*;q=0.3";
  }

  private void MergeHeaders(
    Dictionary<string, string> destination,
    Dictionary<string, string> source)
  {
    foreach (KeyValuePair<string, string> keyValuePair in source)
      destination[keyValuePair.Key] = keyValuePair.Value;
  }

  private HttpProxyClient FindHttpProxyInChain(ChainProxyClient chainProxy)
  {
    HttpProxyClient httpProxyInChain1 = (HttpProxyClient) null;
    foreach (ProxyClient proxy in chainProxy.Proxies)
    {
      if (proxy.Type == ProxyType.Http)
      {
        httpProxyInChain1 = proxy as HttpProxyClient;
        if (!string.IsNullOrEmpty(httpProxyInChain1.Username) || !string.IsNullOrEmpty(httpProxyInChain1.Password))
          return httpProxyInChain1;
      }
      else if (proxy.Type == ProxyType.Chain)
      {
        HttpProxyClient httpProxyInChain2 = this.FindHttpProxyInChain(proxy as ChainProxyClient);
        if (httpProxyInChain2 != null && (!string.IsNullOrEmpty(httpProxyInChain2.Username) || !string.IsNullOrEmpty(httpProxyInChain2.Password)))
          return httpProxyInChain2;
      }
    }
    return httpProxyInChain1;
  }

  private string ToHeadersString(Dictionary<string, string> headers)
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (KeyValuePair<string, string> header in headers)
      stringBuilder.AppendFormat("{0}: {1}\r\n", (object) header.Key, (object) header.Value);
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

  private bool IsClosedHeader(string name)
  {
    return HttpRequest._closedHeaders.Contains<string>(name, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  }

  private void ClearRequestData(bool clearHeaders = true)
  {
    this._content = (HttpContent) null;
    this._temporaryUrlParams = (RequestParams) null;
    this._temporaryParams = (RequestParams) null;
    this._temporaryMultipartContent = (MultipartContent) null;
    if (!clearHeaders)
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

  private sealed class HttpWraperStream : Stream
  {
    private Stream _baseStream;
    private int _sendBufferSize;

    public Action<int> BytesReadCallback { get; set; }

    public Action<int> BytesWriteCallback { get; set; }

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

    public HttpWraperStream(Stream baseStream, int sendBufferSize)
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
      if (this.BytesReadCallback != null)
        this.BytesReadCallback(num);
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
