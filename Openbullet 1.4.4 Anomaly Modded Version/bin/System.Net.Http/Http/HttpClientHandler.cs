// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpClientHandler
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http;

public class HttpClientHandler : HttpMessageHandler
{
  private static readonly Action<object> s_onCancel = new Action<object>(HttpClientHandler.OnCancel);
  private readonly Action<object> _startRequest;
  private readonly AsyncCallback _getRequestStreamCallback;
  private readonly AsyncCallback _getResponseCallback;
  private volatile bool _operationStarted;
  private volatile bool _disposed;
  private long _maxRequestContentBufferSize;
  private int _maxResponseHeadersLength;
  private CookieContainer _cookieContainer;
  private bool _useCookies;
  private DecompressionMethods _automaticDecompression;
  private IWebProxy _proxy;
  private bool _useProxy;
  private ICredentials _defaultProxyCredentials;
  private bool _preAuthenticate;
  private bool _useDefaultCredentials;
  private ICredentials _credentials;
  private bool _allowAutoRedirect;
  private int _maxAutomaticRedirections;
  private string _connectionGroupName;
  private ClientCertificateOption _clientCertOptions;
  private X509Certificate2Collection _clientCertificates;
  private IDictionary<string, object> _properties;
  private int _maxConnectionsPerServer;
  private Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> _serverCertificateCustomValidationCallback;

  public bool AllowAutoRedirect
  {
    get => this._allowAutoRedirect;
    set
    {
      this.CheckDisposedOrStarted();
      this._allowAutoRedirect = value;
    }
  }

  public DecompressionMethods AutomaticDecompression
  {
    get => this._automaticDecompression;
    set
    {
      this.CheckDisposedOrStarted();
      this._automaticDecompression = value;
    }
  }

  public bool CheckCertificateRevocationList
  {
    get => throw new PlatformNotSupportedException();
    set
    {
      this.CheckDisposedOrStarted();
      throw new PlatformNotSupportedException();
    }
  }

  public ClientCertificateOption ClientCertificateOptions
  {
    get => this._clientCertOptions;
    set
    {
      if (value != ClientCertificateOption.Manual && value != ClientCertificateOption.Automatic)
        throw new ArgumentOutOfRangeException(nameof (value));
      this.CheckDisposedOrStarted();
      this._clientCertOptions = value;
    }
  }

  public X509CertificateCollection ClientCertificates
  {
    get
    {
      if (this._clientCertificates == null)
        this._clientCertificates = new X509Certificate2Collection();
      return (X509CertificateCollection) this._clientCertificates;
    }
  }

  public CookieContainer CookieContainer
  {
    get => this._cookieContainer;
    set
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (!this.UseCookies)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.SR.net_http_invalid_enable_first, (object) "UseCookies", (object) "true"));
      this.CheckDisposedOrStarted();
      this._cookieContainer = value;
    }
  }

  public ICredentials Credentials
  {
    get => this._credentials;
    set
    {
      this.CheckDisposedOrStarted();
      this._credentials = value;
    }
  }

  public ICredentials DefaultProxyCredentials
  {
    get => this._defaultProxyCredentials;
    set
    {
      this.CheckDisposedOrStarted();
      this._defaultProxyCredentials = value;
    }
  }

  public int MaxAutomaticRedirections
  {
    get => this._maxAutomaticRedirections;
    set
    {
      if (value <= 0)
        throw new ArgumentOutOfRangeException(nameof (value));
      this.CheckDisposedOrStarted();
      this._maxAutomaticRedirections = value;
    }
  }

  public int MaxConnectionsPerServer
  {
    get => this._maxConnectionsPerServer;
    set
    {
      this.CheckDisposedOrStarted();
      this._maxConnectionsPerServer = value;
    }
  }

  public long MaxRequestContentBufferSize
  {
    get => this._maxRequestContentBufferSize;
    set
    {
      if (value < 0L)
        throw new ArgumentOutOfRangeException(nameof (value));
      if (value > (long) int.MaxValue)
        throw new ArgumentOutOfRangeException(nameof (value), (object) value, string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.SR.net_http_content_buffersize_limit, (object) (long) int.MaxValue));
      this.CheckDisposedOrStarted();
      this._maxRequestContentBufferSize = value;
    }
  }

  public int MaxResponseHeadersLength
  {
    get => this._maxResponseHeadersLength;
    set
    {
      if (value <= 0)
        throw new ArgumentOutOfRangeException(nameof (value));
      this.CheckDisposedOrStarted();
      this._maxResponseHeadersLength = value;
    }
  }

  public bool PreAuthenticate
  {
    get => this._preAuthenticate;
    set
    {
      this.CheckDisposedOrStarted();
      this._preAuthenticate = value;
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

  public IWebProxy Proxy
  {
    get => this._proxy;
    [SecuritySafeCritical] set
    {
      if (!this.UseProxy && value != null)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, System.SR.net_http_invalid_enable_first, (object) "UseProxy", (object) "true"));
      this.CheckDisposedOrStarted();
      ExceptionHelper.WebPermissionUnrestricted.Demand();
      this._proxy = value;
    }
  }

  public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback
  {
    get => this._serverCertificateCustomValidationCallback;
    set
    {
      this.CheckDisposedOrStarted();
      this._serverCertificateCustomValidationCallback = value;
    }
  }

  public SslProtocols SslProtocols
  {
    get => throw new PlatformNotSupportedException();
    set
    {
      this.CheckDisposedOrStarted();
      throw new PlatformNotSupportedException();
    }
  }

  public virtual bool SupportsAutomaticDecompression => true;

  public virtual bool SupportsProxy => true;

  public virtual bool SupportsRedirectConfiguration => true;

  public bool UseCookies
  {
    get => this._useCookies;
    set
    {
      this.CheckDisposedOrStarted();
      this._useCookies = value;
    }
  }

  public bool UseDefaultCredentials
  {
    get => this._useDefaultCredentials;
    set
    {
      this.CheckDisposedOrStarted();
      this._useDefaultCredentials = value;
    }
  }

  public bool UseProxy
  {
    get => this._useProxy;
    set
    {
      this.CheckDisposedOrStarted();
      this._useProxy = value;
    }
  }

  public HttpClientHandler()
  {
    this._startRequest = new Action<object>(this.StartRequest);
    this._getRequestStreamCallback = new AsyncCallback(this.GetRequestStreamCallback);
    this._getResponseCallback = new AsyncCallback(this.GetResponseCallback);
    this._connectionGroupName = RuntimeHelpers.GetHashCode((object) this).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
    this._allowAutoRedirect = true;
    this._maxRequestContentBufferSize = (long) int.MaxValue;
    this._automaticDecompression = DecompressionMethods.None;
    this._cookieContainer = new CookieContainer();
    this._credentials = (ICredentials) null;
    this._maxAutomaticRedirections = 50;
    this._preAuthenticate = false;
    this._proxy = (IWebProxy) null;
    this._useProxy = true;
    this._useCookies = true;
    this._useDefaultCredentials = false;
    this._clientCertOptions = ClientCertificateOption.Manual;
    this._maxResponseHeadersLength = HttpWebRequest.DefaultMaximumResponseHeadersLength;
    this._defaultProxyCredentials = (ICredentials) null;
    this._clientCertificates = (X509Certificate2Collection) null;
    this._properties = (IDictionary<string, object>) null;
    this._maxConnectionsPerServer = ServicePointManager.DefaultConnectionLimit;
    this._serverCertificateCustomValidationCallback = (Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>) null;
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && !this._disposed)
    {
      this._disposed = true;
      ServicePointManager.CloseConnectionGroups(this._connectionGroupName);
    }
    base.Dispose(disposing);
  }

  private HttpWebRequest CreateAndPrepareWebRequest(HttpRequestMessage request)
  {
    HttpWebRequest webRequest = request.Content == null ? new HttpWebRequest(request.RequestUri, true, this._connectionGroupName, (Action<Stream>) null) : new HttpWebRequest(request.RequestUri, true, this._connectionGroupName, new Action<Stream>(request.Content.CopyTo));
    if (Logging.On)
      Logging.Associate(Logging.Http, (object) request, (object) webRequest);
    webRequest.Method = request.Method.Method;
    webRequest.ProtocolVersion = request.Version;
    this.SetDefaultOptions(webRequest);
    HttpClientHandler.SetConnectionOptions(webRequest, request);
    this.SetServicePointOptions(webRequest, request);
    HttpClientHandler.SetRequestHeaders(webRequest, request);
    HttpClientHandler.SetContentHeaders(webRequest, request);
    webRequest.ServicePoint.ConnectionLimit = this._maxConnectionsPerServer;
    webRequest.MaximumResponseHeadersLength = this._maxResponseHeadersLength;
    if (this.ClientCertificateOptions == ClientCertificateOption.Manual && this._clientCertificates != null && this._clientCertificates.Count > 0)
      webRequest.ClientCertificates = (X509CertificateCollection) this._clientCertificates;
    if (this._serverCertificateCustomValidationCallback != null)
      webRequest.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(this.ServerCertificateValidationCallback);
    if (this._defaultProxyCredentials != null && webRequest.Proxy != null)
      webRequest.Proxy.Credentials = this._defaultProxyCredentials;
    this.InitializeWebRequest(request, webRequest);
    return webRequest;
  }

  private bool ServerCertificateValidationCallback(
    object sender,
    X509Certificate certificate,
    X509Chain chain,
    SslPolicyErrors sslPolicyErrors)
  {
    return this._serverCertificateCustomValidationCallback((HttpRequestMessage) null, (X509Certificate2) certificate, chain, sslPolicyErrors);
  }

  internal virtual void InitializeWebRequest(HttpRequestMessage request, HttpWebRequest webRequest)
  {
  }

  private void SetDefaultOptions(HttpWebRequest webRequest)
  {
    webRequest.Timeout = -1;
    webRequest.AllowAutoRedirect = this._allowAutoRedirect;
    webRequest.AutomaticDecompression = this._automaticDecompression;
    webRequest.PreAuthenticate = this._preAuthenticate;
    if (this._useDefaultCredentials)
      webRequest.UseDefaultCredentials = true;
    else
      webRequest.Credentials = this._credentials;
    if (this._allowAutoRedirect)
      webRequest.MaximumAutomaticRedirections = this._maxAutomaticRedirections;
    if (this._useProxy)
    {
      if (this._proxy != null)
        webRequest.Proxy = this._proxy;
    }
    else
      webRequest.Proxy = (IWebProxy) null;
    if (this._useCookies)
      webRequest.CookieContainer = this._cookieContainer;
    if (this._clientCertOptions != ClientCertificateOption.Automatic || !ComNetOS.IsWin7orLater)
      return;
    X509CertificateCollection clientCertificates = UnsafeNclNativeMethods.NativePKI.FindClientCertificates();
    if (clientCertificates.Count <= 0)
      return;
    webRequest.ClientCertificates = clientCertificates;
  }

  private static void SetConnectionOptions(HttpWebRequest webRequest, HttpRequestMessage request)
  {
    if (request.Version <= HttpVersion.Version10)
    {
      bool flag = false;
      foreach (string strA in request.Headers.Connection)
      {
        if (string.Compare(strA, "Keep-Alive", StringComparison.OrdinalIgnoreCase) == 0)
        {
          flag = true;
          break;
        }
      }
      webRequest.KeepAlive = flag;
    }
    else
    {
      bool? connectionClose = request.Headers.ConnectionClose;
      bool flag = true;
      if ((connectionClose.GetValueOrDefault() == flag ? (connectionClose.HasValue ? 1 : 0) : 0) == 0)
        return;
      webRequest.KeepAlive = false;
    }
  }

  private void SetServicePointOptions(HttpWebRequest webRequest, HttpRequestMessage request)
  {
    bool? expectContinue = request.Headers.ExpectContinue;
    if (!expectContinue.HasValue)
      return;
    webRequest.ServicePoint.Expect100Continue = expectContinue.Value;
  }

  private static void SetRequestHeaders(HttpWebRequest webRequest, HttpRequestMessage request)
  {
    WebHeaderCollection headers1 = webRequest.Headers;
    HttpRequestHeaders headers2 = request.Headers;
    bool flag1 = headers2.Contains("Host");
    bool flag2 = headers2.Contains("Expect");
    bool flag3 = headers2.Contains("Transfer-Encoding");
    bool flag4 = headers2.Contains("Connection");
    if (flag1)
    {
      string host = headers2.Host;
      if (host != null)
        webRequest.Host = host;
    }
    if (flag2)
    {
      string stringWithoutSpecial = headers2.Expect.GetHeaderStringWithoutSpecial();
      if (!string.IsNullOrEmpty(stringWithoutSpecial) || !headers2.Expect.IsSpecialValueSet)
        headers1.AddInternal("Expect", stringWithoutSpecial);
    }
    if (flag3)
    {
      string stringWithoutSpecial = headers2.TransferEncoding.GetHeaderStringWithoutSpecial();
      if (!string.IsNullOrEmpty(stringWithoutSpecial) || !headers2.TransferEncoding.IsSpecialValueSet)
        headers1.AddInternal("Transfer-Encoding", stringWithoutSpecial);
    }
    if (flag4)
    {
      string stringWithoutSpecial = headers2.Connection.GetHeaderStringWithoutSpecial();
      if (!string.IsNullOrEmpty(stringWithoutSpecial) || !headers2.Connection.IsSpecialValueSet)
        headers1.AddInternal("Connection", stringWithoutSpecial);
    }
    foreach (KeyValuePair<string, string> headerString in request.Headers.GetHeaderStrings())
    {
      string key = headerString.Key;
      if ((!flag1 || !HttpClientHandler.AreEqual("Host", key)) && (!flag2 || !HttpClientHandler.AreEqual("Expect", key)) && (!flag3 || !HttpClientHandler.AreEqual("Transfer-Encoding", key)) && (!flag4 || !HttpClientHandler.AreEqual("Connection", key)))
        headers1.AddInternal(headerString.Key, headerString.Value);
    }
  }

  private static void SetContentHeaders(HttpWebRequest webRequest, HttpRequestMessage request)
  {
    if (request.Content == null)
      return;
    if (request.Content.Headers.Contains("Content-Length"))
    {
      foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) request.Content.Headers)
      {
        if (string.Compare("Content-Length", header.Key, StringComparison.OrdinalIgnoreCase) != 0)
          webRequest.Headers.AddInternal(header.Key, string.Join(", ", header.Value));
      }
    }
    else
    {
      foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) request.Content.Headers)
        webRequest.Headers.AddInternal(header.Key, string.Join(", ", header.Value));
    }
  }

  protected internal override Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
  {
    if (request == null)
      throw new ArgumentNullException(nameof (request), System.SR.net_http_handler_norequest);
    this.CheckDisposed();
    if (Logging.On)
      Logging.Enter(Logging.Http, (object) this, nameof (SendAsync), (object) request);
    this.SetOperationStarted();
    TaskCompletionSource<HttpResponseMessage> completionSource = new TaskCompletionSource<HttpResponseMessage>();
    HttpClientHandler.RequestState state = new HttpClientHandler.RequestState();
    state.tcs = completionSource;
    state.cancellationToken = cancellationToken;
    state.requestMessage = request;
    try
    {
      HttpWebRequest prepareWebRequest = this.CreateAndPrepareWebRequest(request);
      state.webRequest = prepareWebRequest;
      cancellationToken.Register(HttpClientHandler.s_onCancel, (object) prepareWebRequest);
      if (ExecutionContext.IsFlowSuppressed())
      {
        IWebProxy webProxy = (IWebProxy) null;
        if (this._useProxy)
          webProxy = this._proxy ?? WebRequest.DefaultWebProxy;
        if (this.UseDefaultCredentials || this.Credentials != null || webProxy != null && webProxy.Credentials != null)
          this.SafeCaptureIdenity(state);
      }
      Task.Factory.StartNew(this._startRequest, (object) state);
    }
    catch (Exception ex)
    {
      this.HandleAsyncException(state, ex);
    }
    if (Logging.On)
      Logging.Exit(Logging.Http, (object) this, nameof (SendAsync), (object) completionSource.Task);
    return completionSource.Task;
  }

  private void StartRequest(object obj)
  {
    HttpClientHandler.RequestState state = obj as HttpClientHandler.RequestState;
    try
    {
      if (state.requestMessage.Content != null)
      {
        this.PrepareAndStartContentUpload(state);
      }
      else
      {
        state.webRequest.ContentLength = 0L;
        this.StartGettingResponse(state);
      }
    }
    catch (Exception ex)
    {
      this.HandleAsyncException(state, ex);
    }
  }

  private void PrepareAndStartContentUpload(HttpClientHandler.RequestState state)
  {
    HttpContent requestContent = state.requestMessage.Content;
    try
    {
      bool? transferEncodingChunked = state.requestMessage.Headers.TransferEncodingChunked;
      bool flag = true;
      if ((transferEncodingChunked.GetValueOrDefault() == flag ? (transferEncodingChunked.HasValue ? 1 : 0) : 0) != 0)
      {
        state.webRequest.SendChunked = true;
        this.StartGettingRequestStream(state);
      }
      else
      {
        long? contentLength = requestContent.Headers.ContentLength;
        if (contentLength.HasValue)
        {
          state.webRequest.ContentLength = contentLength.Value;
          this.StartGettingRequestStream(state);
        }
        else
        {
          if (this._maxRequestContentBufferSize == 0L)
            throw new HttpRequestException(System.SR.net_http_handler_nocontentlength);
          requestContent.LoadIntoBufferAsync(this._maxRequestContentBufferSize).ContinueWithStandard((Action<Task>) (task =>
          {
            if (task.IsFaulted)
            {
              this.HandleAsyncException(state, task.Exception.GetBaseException());
            }
            else
            {
              try
              {
                contentLength = requestContent.Headers.ContentLength;
                state.webRequest.ContentLength = contentLength.Value;
                this.StartGettingRequestStream(state);
              }
              catch (Exception ex)
              {
                this.HandleAsyncException(state, ex);
              }
            }
          }));
        }
      }
    }
    catch (Exception ex)
    {
      this.HandleAsyncException(state, ex);
    }
  }

  private void StartGettingRequestStream(HttpClientHandler.RequestState state)
  {
    if (state.identity != null)
    {
      using (state.identity.Impersonate())
        state.webRequest.BeginGetRequestStream(this._getRequestStreamCallback, (object) state);
    }
    else
      state.webRequest.BeginGetRequestStream(this._getRequestStreamCallback, (object) state);
  }

  private void GetRequestStreamCallback(IAsyncResult ar)
  {
    HttpClientHandler.RequestState state = ar.AsyncState as HttpClientHandler.RequestState;
    try
    {
      TransportContext context = (TransportContext) null;
      Stream requestStream = state.webRequest.EndGetRequestStream(ar, out context);
      state.requestStream = requestStream;
      state.requestMessage.Content.CopyToAsync(requestStream, context).ContinueWithStandard((Action<Task>) (task =>
      {
        try
        {
          if (task.IsFaulted)
            this.HandleAsyncException(state, task.Exception.GetBaseException());
          else if (task.IsCanceled)
          {
            state.tcs.TrySetCanceled();
          }
          else
          {
            state.requestStream.Close();
            this.StartGettingResponse(state);
          }
        }
        catch (Exception ex)
        {
          this.HandleAsyncException(state, ex);
        }
      }));
    }
    catch (Exception ex)
    {
      this.HandleAsyncException(state, ex);
    }
  }

  private void StartGettingResponse(HttpClientHandler.RequestState state)
  {
    if (state.identity != null)
    {
      using (state.identity.Impersonate())
        state.webRequest.BeginGetResponse(this._getResponseCallback, (object) state);
    }
    else
      state.webRequest.BeginGetResponse(this._getResponseCallback, (object) state);
  }

  private void GetResponseCallback(IAsyncResult ar)
  {
    HttpClientHandler.RequestState asyncState = ar.AsyncState as HttpClientHandler.RequestState;
    try
    {
      HttpWebResponse response = asyncState.webRequest.EndGetResponse(ar) as HttpWebResponse;
      asyncState.tcs.TrySetResult(this.CreateResponseMessage(response, asyncState.requestMessage));
    }
    catch (Exception ex)
    {
      this.HandleAsyncException(asyncState, ex);
    }
  }

  private HttpResponseMessage CreateResponseMessage(
    HttpWebResponse webResponse,
    HttpRequestMessage request)
  {
    HttpResponseMessage responseMessage = new HttpResponseMessage(webResponse.StatusCode);
    responseMessage.ReasonPhrase = webResponse.StatusDescription;
    responseMessage.Version = webResponse.ProtocolVersion;
    responseMessage.RequestMessage = request;
    responseMessage.Content = (HttpContent) new StreamContent((Stream) new HttpClientHandler.WebExceptionWrapperStream(webResponse.GetResponseStream()));
    request.RequestUri = webResponse.ResponseUri;
    WebHeaderCollection headers1 = webResponse.Headers;
    HttpContentHeaders headers2 = responseMessage.Content.Headers;
    HttpResponseHeaders headers3 = responseMessage.Headers;
    if (webResponse.ContentLength >= 0L)
      headers2.ContentLength = new long?(webResponse.ContentLength);
    for (int index = 0; index < headers1.Count; ++index)
    {
      string key = headers1.GetKey(index);
      if (string.Compare(key, "Content-Length", StringComparison.OrdinalIgnoreCase) != 0)
      {
        string[] values = headers1.GetValues(index);
        if (!headers3.TryAddWithoutValidation(key, (IEnumerable<string>) values))
          headers2.TryAddWithoutValidation(key, (IEnumerable<string>) values);
      }
    }
    return responseMessage;
  }

  private void HandleAsyncException(HttpClientHandler.RequestState state, Exception e)
  {
    if (Logging.On)
      Logging.Exception(Logging.Http, (object) this, "SendAsync", e);
    if (state.cancellationToken.IsCancellationRequested)
    {
      state.tcs.TrySetCanceled();
    }
    else
    {
      switch (e)
      {
        case WebException _:
        case IOException _:
          state.tcs.TrySetException((Exception) new HttpRequestException(System.SR.net_http_client_execution_error, e));
          break;
        default:
          state.tcs.TrySetException(e);
          break;
      }
    }
  }

  private static void OnCancel(object state) => (state as HttpWebRequest).Abort();

  private void SetOperationStarted()
  {
    if (this._operationStarted)
      return;
    this._operationStarted = true;
  }

  private void CheckDisposed()
  {
    if (this._disposed)
      throw new ObjectDisposedException(this.GetType().FullName);
  }

  internal void CheckDisposedOrStarted()
  {
    this.CheckDisposed();
    if (this._operationStarted)
      throw new InvalidOperationException(System.SR.net_http_operation_started);
  }

  private static bool AreEqual(string x, string y)
  {
    return string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;
  }

  [SecuritySafeCritical]
  [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.ControlPrincipal)]
  private void SafeCaptureIdenity(HttpClientHandler.RequestState state)
  {
    state.identity = WindowsIdentity.GetCurrent();
  }

  private class RequestState
  {
    internal HttpWebRequest webRequest;
    internal TaskCompletionSource<HttpResponseMessage> tcs;
    internal CancellationToken cancellationToken;
    internal HttpRequestMessage requestMessage;
    internal Stream requestStream;
    internal WindowsIdentity identity;
  }

  private class WebExceptionWrapperStream : DelegatingStream
  {
    internal WebExceptionWrapperStream(Stream innerStream)
      : base(innerStream)
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      try
      {
        return base.Read(buffer, offset, count);
      }
      catch (WebException ex)
      {
        throw new IOException(System.SR.net_http_io_read, (Exception) ex);
      }
    }

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      try
      {
        return base.BeginRead(buffer, offset, count, callback, state);
      }
      catch (WebException ex)
      {
        throw new IOException(System.SR.net_http_io_read, (Exception) ex);
      }
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
      try
      {
        return base.EndRead(asyncResult);
      }
      catch (WebException ex)
      {
        throw new IOException(System.SR.net_http_io_read, (Exception) ex);
      }
    }

    public override async Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      int num;
      try
      {
        num = await base.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
      }
      catch (WebException ex)
      {
        throw new IOException(System.SR.net_http_io_read, (Exception) ex);
      }
      return num;
    }

    public override int ReadByte()
    {
      try
      {
        return base.ReadByte();
      }
      catch (WebException ex)
      {
        throw new IOException(System.SR.net_http_io_read, (Exception) ex);
      }
    }
  }
}
