// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpMessageHandler
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net.Http;

public abstract class HttpMessageHandler : IDisposable
{
  protected HttpMessageHandler()
  {
    if (NetEventSource.Log.IsEnabled())
      NetEventSource.Enter(NetEventSource.ComponentType.Http, (object) this, ".ctor", (object) null);
    if (!NetEventSource.Log.IsEnabled())
      return;
    NetEventSource.Exit(NetEventSource.ComponentType.Http, (object) this, ".ctor", (object) null);
  }

  protected internal abstract Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken);

  protected virtual void Dispose(bool disposing)
  {
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }
}
