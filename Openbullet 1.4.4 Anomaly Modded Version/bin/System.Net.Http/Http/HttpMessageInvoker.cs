// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpMessageInvoker
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http;

public class HttpMessageInvoker : IDisposable
{
  private volatile bool _disposed;
  private bool _disposeHandler;
  private HttpMessageHandler _handler;

  public HttpMessageInvoker(HttpMessageHandler handler)
    : this(handler, true)
  {
  }

  public HttpMessageInvoker(HttpMessageHandler handler, bool disposeHandler)
  {
    if (NetEventSource.Log.IsEnabled())
      NetEventSource.Enter(NetEventSource.ComponentType.Http, (object) this, ".ctor", (object) handler);
    if (handler == null)
      throw new ArgumentNullException(nameof (handler));
    if (HttpEventSource.Log.IsEnabled())
      HttpEventSource.Associate((object) this, (object) handler);
    this._handler = handler;
    this._disposeHandler = disposeHandler;
    if (!NetEventSource.Log.IsEnabled())
      return;
    NetEventSource.Exit(NetEventSource.ComponentType.Http, (object) this, ".ctor", (object) null);
  }

  public virtual Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
  {
    if (request == null)
      throw new ArgumentNullException(nameof (request));
    this.CheckDisposed();
    if (NetEventSource.Log.IsEnabled())
      NetEventSource.Enter(NetEventSource.ComponentType.Http, (object) this, nameof (SendAsync), (object) $"{(object) LoggingHash.GetObjectLogHash((object) request)}: {(object) request}");
    Task<HttpResponseMessage> retObject = this._handler.SendAsync(request, cancellationToken);
    if (NetEventSource.Log.IsEnabled())
      NetEventSource.Exit(NetEventSource.ComponentType.Http, (object) this, nameof (SendAsync), (object) retObject);
    return retObject;
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!disposing || this._disposed)
      return;
    this._disposed = true;
    if (!this._disposeHandler)
      return;
    this._handler.Dispose();
  }

  private void CheckDisposed()
  {
    if (this._disposed)
      throw new ObjectDisposedException(this.GetType().ToString());
  }
}
