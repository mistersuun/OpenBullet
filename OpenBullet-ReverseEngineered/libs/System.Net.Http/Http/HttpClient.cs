// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpClient
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http;

public class HttpClient : HttpMessageInvoker
{
  private static readonly TimeSpan s_defaultTimeout = TimeSpan.FromSeconds(100.0);
  private static readonly TimeSpan s_maxTimeout = TimeSpan.FromMilliseconds((double) int.MaxValue);
  private static readonly TimeSpan s_infiniteTimeout = System.Threading.Timeout.InfiniteTimeSpan;
  private const HttpCompletionOption defaultCompletionOption = HttpCompletionOption.ResponseContentRead;
  private static readonly Task<Stream> s_nullStreamTask = Task.FromResult<Stream>(Stream.Null);
  private volatile bool _operationStarted;
  private volatile bool _disposed;
  private CancellationTokenSource _pendingRequestsCts;
  private HttpRequestHeaders _defaultRequestHeaders;
  private Uri _baseAddress;
  private TimeSpan _timeout;
  private long _maxResponseContentBufferSize;

  public HttpRequestHeaders DefaultRequestHeaders
  {
    get
    {
      if (this._defaultRequestHeaders == null)
        this._defaultRequestHeaders = new HttpRequestHeaders();
      return this._defaultRequestHeaders;
    }
  }

  public Uri BaseAddress
  {
    get => this._baseAddress;
    set
    {
      HttpClient.CheckBaseAddress(value, nameof (value));
      this.CheckDisposedOrStarted();
      if (HttpEventSource.Log.IsEnabled())
        HttpEventSource.UriBaseAddress((object) this, value != (Uri) null ? value.ToString() : string.Empty);
      this._baseAddress = value;
    }
  }

  public TimeSpan Timeout
  {
    get => this._timeout;
    set
    {
      if (value != HttpClient.s_infiniteTimeout && (value <= TimeSpan.Zero || value > HttpClient.s_maxTimeout))
        throw new ArgumentOutOfRangeException(nameof (value));
      this.CheckDisposedOrStarted();
      this._timeout = value;
    }
  }

  public long MaxResponseContentBufferSize
  {
    get => this._maxResponseContentBufferSize;
    set
    {
      if (value <= 0L)
        throw new ArgumentOutOfRangeException(nameof (value));
      if (value > (long) int.MaxValue)
        throw new ArgumentOutOfRangeException(nameof (value), (object) value, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_content_buffersize_limit, (object) (long) int.MaxValue));
      this.CheckDisposedOrStarted();
      this._maxResponseContentBufferSize = value;
    }
  }

  public HttpClient()
    : this((HttpMessageHandler) new HttpClientHandler())
  {
  }

  public HttpClient(HttpMessageHandler handler)
    : this(handler, true)
  {
  }

  public HttpClient(HttpMessageHandler handler, bool disposeHandler)
    : base(handler, disposeHandler)
  {
    if (NetEventSource.Log.IsEnabled())
      NetEventSource.Enter(NetEventSource.ComponentType.Http, (object) this, ".ctor", (object) handler);
    this._timeout = HttpClient.s_defaultTimeout;
    this._maxResponseContentBufferSize = (long) int.MaxValue;
    this._pendingRequestsCts = new CancellationTokenSource();
    if (!NetEventSource.Log.IsEnabled())
      return;
    NetEventSource.Exit(NetEventSource.ComponentType.Http, (object) this, ".ctor", (object) null);
  }

  public Task<string> GetStringAsync(string requestUri)
  {
    return this.GetStringAsync(this.CreateUri(requestUri));
  }

  public Task<string> GetStringAsync(Uri requestUri)
  {
    return this.GetContentAsync<string>(this.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead), (Func<HttpContent, string>) (content => content == null ? string.Empty : content.ReadBufferedContentAsString()));
  }

  public Task<byte[]> GetByteArrayAsync(string requestUri)
  {
    return this.GetByteArrayAsync(this.CreateUri(requestUri));
  }

  public Task<byte[]> GetByteArrayAsync(Uri requestUri)
  {
    return this.GetContentAsync<byte[]>(this.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead), (Func<HttpContent, byte[]>) (content => content == null ? Array.Empty<byte>() : content.ReadBufferedContentAsByteArray()));
  }

  public Task<Stream> GetStreamAsync(string requestUri)
  {
    return this.GetStreamAsync(this.CreateUri(requestUri));
  }

  public Task<Stream> GetStreamAsync(Uri requestUri)
  {
    return this.GetContentAsync<Stream>(this.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead), (Func<HttpContent, Task<Stream>>) (content => content == null ? HttpClient.s_nullStreamTask : content.ReadAsStreamAsync()));
  }

  private async Task<T> GetContentAsync<T>(
    Task<HttpResponseMessage> getTask,
    Func<HttpContent, T> readAs)
  {
    HttpResponseMessage httpResponseMessage = await getTask.ConfigureAwait(false);
    httpResponseMessage.EnsureSuccessStatusCode();
    return readAs(httpResponseMessage.Content);
  }

  private async Task<T> GetContentAsync<T>(
    Task<HttpResponseMessage> getTask,
    Func<HttpContent, Task<T>> readAsAsync)
  {
    HttpResponseMessage httpResponseMessage = await getTask.ConfigureAwait(false);
    httpResponseMessage.EnsureSuccessStatusCode();
    return await readAsAsync(httpResponseMessage.Content).ConfigureAwait(false);
  }

  public Task<HttpResponseMessage> GetAsync(string requestUri)
  {
    return this.GetAsync(this.CreateUri(requestUri));
  }

  public Task<HttpResponseMessage> GetAsync(Uri requestUri)
  {
    return this.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead);
  }

  public Task<HttpResponseMessage> GetAsync(
    string requestUri,
    HttpCompletionOption completionOption)
  {
    return this.GetAsync(this.CreateUri(requestUri), completionOption);
  }

  public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption)
  {
    return this.GetAsync(requestUri, completionOption, CancellationToken.None);
  }

  public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken)
  {
    return this.GetAsync(this.CreateUri(requestUri), cancellationToken);
  }

  public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken)
  {
    return this.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead, cancellationToken);
  }

  public Task<HttpResponseMessage> GetAsync(
    string requestUri,
    HttpCompletionOption completionOption,
    CancellationToken cancellationToken)
  {
    return this.GetAsync(this.CreateUri(requestUri), completionOption, cancellationToken);
  }

  public Task<HttpResponseMessage> GetAsync(
    Uri requestUri,
    HttpCompletionOption completionOption,
    CancellationToken cancellationToken)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption, cancellationToken);
  }

  public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
  {
    return this.PostAsync(this.CreateUri(requestUri), content);
  }

  public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
  {
    return this.PostAsync(requestUri, content, CancellationToken.None);
  }

  public Task<HttpResponseMessage> PostAsync(
    string requestUri,
    HttpContent content,
    CancellationToken cancellationToken)
  {
    return this.PostAsync(this.CreateUri(requestUri), content, cancellationToken);
  }

  public Task<HttpResponseMessage> PostAsync(
    Uri requestUri,
    HttpContent content,
    CancellationToken cancellationToken)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
    {
      Content = content
    }, cancellationToken);
  }

  public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
  {
    return this.PutAsync(this.CreateUri(requestUri), content);
  }

  public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
  {
    return this.PutAsync(requestUri, content, CancellationToken.None);
  }

  public Task<HttpResponseMessage> PutAsync(
    string requestUri,
    HttpContent content,
    CancellationToken cancellationToken)
  {
    return this.PutAsync(this.CreateUri(requestUri), content, cancellationToken);
  }

  public Task<HttpResponseMessage> PutAsync(
    Uri requestUri,
    HttpContent content,
    CancellationToken cancellationToken)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri)
    {
      Content = content
    }, cancellationToken);
  }

  public Task<HttpResponseMessage> DeleteAsync(string requestUri)
  {
    return this.DeleteAsync(this.CreateUri(requestUri));
  }

  public Task<HttpResponseMessage> DeleteAsync(Uri requestUri)
  {
    return this.DeleteAsync(requestUri, CancellationToken.None);
  }

  public Task<HttpResponseMessage> DeleteAsync(
    string requestUri,
    CancellationToken cancellationToken)
  {
    return this.DeleteAsync(this.CreateUri(requestUri), cancellationToken);
  }

  public Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken)
  {
    return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);
  }

  public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
  {
    return this.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
  }

  public override Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
  {
    return this.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
  }

  public Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    HttpCompletionOption completionOption)
  {
    return this.SendAsync(request, completionOption, CancellationToken.None);
  }

  public Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    HttpCompletionOption completionOption,
    CancellationToken cancellationToken)
  {
    if (request == null)
      throw new ArgumentNullException(nameof (request));
    this.CheckDisposed();
    HttpClient.CheckRequestMessage(request);
    this.SetOperationStarted();
    this.PrepareRequestMessage(request);
    CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this._pendingRequestsCts.Token);
    this.SetTimeout(linkedTokenSource);
    return this.FinishSendAsync(base.SendAsync(request, linkedTokenSource.Token), request, linkedTokenSource, completionOption == HttpCompletionOption.ResponseContentRead);
  }

  private async Task<HttpResponseMessage> FinishSendAsync(
    Task<HttpResponseMessage> sendTask,
    HttpRequestMessage request,
    CancellationTokenSource linkedCts,
    bool bufferResponseContent)
  {
    HttpResponseMessage response = (HttpResponseMessage) null;
    HttpResponseMessage httpResponseMessage;
    try
    {
      response = await sendTask.ConfigureAwait(false);
      if (response == null)
        throw new InvalidOperationException(SR.net_http_handler_noresponse);
      if (bufferResponseContent && response.Content != null)
        await response.Content.LoadIntoBufferAsync(this._maxResponseContentBufferSize).ConfigureAwait(false);
      if (HttpEventSource.Log.IsEnabled())
        HttpEventSource.ClientSendCompleted(this, response, request);
      httpResponseMessage = response;
    }
    catch (Exception ex)
    {
      response?.Dispose();
      if (linkedCts.IsCancellationRequested && ex is HttpRequestException)
      {
        this.LogSendError(request, linkedCts, "SendAsync", (Exception) null);
        throw new OperationCanceledException(linkedCts.Token);
      }
      this.LogSendError(request, linkedCts, "SendAsync", ex);
      if (NetEventSource.Log.IsEnabled())
        NetEventSource.Exception(NetEventSource.ComponentType.Http, (object) this, "SendAsync", ex);
      throw;
    }
    finally
    {
      try
      {
        request.Content?.Dispose();
      }
      finally
      {
        linkedCts.Dispose();
      }
    }
    return httpResponseMessage;
  }

  public void CancelPendingRequests()
  {
    this.CheckDisposed();
    if (NetEventSource.Log.IsEnabled())
      NetEventSource.Enter(NetEventSource.ComponentType.Http, (object) this, nameof (CancelPendingRequests), (object) "");
    CancellationTokenSource cancellationTokenSource = Interlocked.Exchange<CancellationTokenSource>(ref this._pendingRequestsCts, new CancellationTokenSource());
    cancellationTokenSource.Cancel();
    cancellationTokenSource.Dispose();
    if (!NetEventSource.Log.IsEnabled())
      return;
    NetEventSource.Exit(NetEventSource.ComponentType.Http, (object) this, nameof (CancelPendingRequests), (object) "");
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && !this._disposed)
    {
      this._disposed = true;
      this._pendingRequestsCts.Cancel();
      this._pendingRequestsCts.Dispose();
    }
    base.Dispose(disposing);
  }

  private void SetOperationStarted()
  {
    if (this._operationStarted)
      return;
    this._operationStarted = true;
  }

  private void CheckDisposedOrStarted()
  {
    this.CheckDisposed();
    if (this._operationStarted)
      throw new InvalidOperationException(SR.net_http_operation_started);
  }

  private void CheckDisposed()
  {
    if (this._disposed)
      throw new ObjectDisposedException(this.GetType().ToString());
  }

  private static void CheckRequestMessage(HttpRequestMessage request)
  {
    if (!request.MarkAsSent())
      throw new InvalidOperationException(SR.net_http_client_request_already_sent);
  }

  private void PrepareRequestMessage(HttpRequestMessage request)
  {
    Uri uri = (Uri) null;
    if (request.RequestUri == (Uri) null && this._baseAddress == (Uri) null)
      throw new InvalidOperationException(SR.net_http_client_invalid_requesturi);
    if (request.RequestUri == (Uri) null)
      uri = this._baseAddress;
    else if (!request.RequestUri.IsAbsoluteUri)
    {
      if (this._baseAddress == (Uri) null)
        throw new InvalidOperationException(SR.net_http_client_invalid_requesturi);
      uri = new Uri(this._baseAddress, request.RequestUri);
    }
    if (uri != (Uri) null)
      request.RequestUri = uri;
    if (this._defaultRequestHeaders == null)
      return;
    request.Headers.AddHeaders((HttpHeaders) this._defaultRequestHeaders);
  }

  private static void CheckBaseAddress(Uri baseAddress, string parameterName)
  {
    if (baseAddress == (Uri) null)
      return;
    if (!baseAddress.IsAbsoluteUri)
      throw new ArgumentException(SR.net_http_client_absolute_baseaddress_required, parameterName);
    if (!HttpUtilities.IsHttpUri(baseAddress))
      throw new ArgumentException(SR.net_http_client_http_baseaddress_required, parameterName);
  }

  private void SetTimeout(CancellationTokenSource cancellationTokenSource)
  {
    if (!(this._timeout != HttpClient.s_infiniteTimeout))
      return;
    cancellationTokenSource.CancelAfter(this._timeout);
  }

  private void LogSendError(
    HttpRequestMessage request,
    CancellationTokenSource cancellationTokenSource,
    string method,
    Exception e)
  {
    if (cancellationTokenSource.IsCancellationRequested)
    {
      if (!NetEventSource.Log.IsEnabled())
        return;
      NetEventSource.PrintError(NetEventSource.ComponentType.Http, (object) this, method, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_client_send_canceled, LoggingHash.GetObjectLogHash((object) request)));
    }
    else
    {
      if (!NetEventSource.Log.IsEnabled())
        return;
      NetEventSource.PrintError(NetEventSource.ComponentType.Http, (object) this, method, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_client_send_error, (object) LoggingHash.GetObjectLogHash((object) request), (object) e));
    }
  }

  private Uri CreateUri(string uri)
  {
    return string.IsNullOrEmpty(uri) ? (Uri) null : new Uri(uri, UriKind.RelativeOrAbsolute);
  }
}
