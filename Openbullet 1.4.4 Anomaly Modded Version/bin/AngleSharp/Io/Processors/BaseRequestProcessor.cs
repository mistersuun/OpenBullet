// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.Processors.BaseRequestProcessor
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using System;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Io.Processors;

public abstract class BaseRequestProcessor : IRequestProcessor
{
  private readonly IResourceLoader _loader;

  public BaseRequestProcessor(IResourceLoader loader) => this._loader = loader;

  public bool IsAvailable => this._loader != null;

  public IDownload Download { get; protected set; }

  public virtual Task ProcessAsync(ResourceRequest request)
  {
    if (!this.IsAvailable || !this.IsDifferentToCurrentDownloadUrl(request.Target))
      return Task.CompletedTask;
    this.CancelDownload();
    this.Download = this._loader.FetchAsync(request);
    return this.FinishDownloadAsync();
  }

  protected abstract Task ProcessResponseAsync(IResponse response);

  protected async Task FinishDownloadAsync()
  {
    IDownload download = this.Download;
    IResponse response = await download.Task.ConfigureAwait(false);
    string eventName = EventNames.Error;
    if (response != null)
    {
      try
      {
        await this.ProcessResponseAsync(response).ConfigureAwait(false);
        eventName = EventNames.Load;
      }
      catch (Exception ex)
      {
      }
      finally
      {
        response.Dispose();
      }
    }
    EventTarget eventTarget = download.Source as EventTarget;
    if (eventTarget == null)
      return;
    if (eventTarget is Element element)
      element.Owner.QueueTask((Action) (() => eventTarget.FireSimpleEvent(eventName)));
    else
      eventTarget.FireSimpleEvent(eventName);
  }

  protected IDownload DownloadWithCors(CorsRequest request)
  {
    return this._loader.FetchWithCorsAsync(request);
  }

  protected void CancelDownload()
  {
    IDownload download = this.Download;
    if (download == null || download.IsCompleted)
      return;
    download.Cancel();
  }

  protected bool IsDifferentToCurrentDownloadUrl(Url target)
  {
    IDownload download = this.Download;
    return download == null || !target.Equals(download.Target);
  }
}
