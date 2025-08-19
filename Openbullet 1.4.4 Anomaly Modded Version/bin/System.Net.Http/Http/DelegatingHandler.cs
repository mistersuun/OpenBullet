// Decompiled with JetBrains decompiler
// Type: System.Net.Http.DelegatingHandler
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http;

public abstract class DelegatingHandler : HttpMessageHandler
{
  private HttpMessageHandler _innerHandler;
  private volatile bool _operationStarted;
  private volatile bool _disposed;

  public HttpMessageHandler InnerHandler
  {
    get => this._innerHandler;
    set
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      this.CheckDisposedOrStarted();
      if (HttpEventSource.Log.IsEnabled())
        HttpEventSource.Associate((object) this, (object) value);
      this._innerHandler = value;
    }
  }

  protected DelegatingHandler()
  {
  }

  protected DelegatingHandler(HttpMessageHandler innerHandler) => this.InnerHandler = innerHandler;

  protected internal override Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
  {
    if (request == null)
      throw new ArgumentNullException(nameof (request), SR.net_http_handler_norequest);
    this.SetOperationStarted();
    return this._innerHandler.SendAsync(request, cancellationToken);
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && !this._disposed)
    {
      this._disposed = true;
      if (this._innerHandler != null)
        this._innerHandler.Dispose();
    }
    base.Dispose(disposing);
  }

  private void CheckDisposed()
  {
    if (this._disposed)
      throw new ObjectDisposedException(this.GetType().ToString());
  }

  private void CheckDisposedOrStarted()
  {
    this.CheckDisposed();
    if (this._operationStarted)
      throw new InvalidOperationException(SR.net_http_operation_started);
  }

  private void SetOperationStarted()
  {
    this.CheckDisposed();
    if (this._innerHandler == null)
      throw new InvalidOperationException(SR.net_http_handler_not_assigned);
    if (this._operationStarted)
      return;
    this._operationStarted = true;
  }
}
