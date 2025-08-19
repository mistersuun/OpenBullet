// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.Processors.MediaRequestProcessor`1
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Media;
using AngleSharp.Media.Dom;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Io.Processors;

internal sealed class MediaRequestProcessor<TMediaInfo>(IBrowsingContext context) : 
  ResourceRequestProcessor<TMediaInfo>(context)
  where TMediaInfo : IMediaInfo
{
  public TMediaInfo Media { get; private set; }

  public MediaNetworkState NetworkState
  {
    get
    {
      IDownload download = this.Download;
      if (download != null)
      {
        if (download.IsRunning)
          return MediaNetworkState.Loading;
        if ((object) this.Resource == null)
          return MediaNetworkState.NoSource;
      }
      return MediaNetworkState.Idle;
    }
  }

  protected override async Task ProcessResponseAsync(IResponse response)
  {
    MediaRequestProcessor<TMediaInfo> requestProcessor = this;
    IResourceService<TMediaInfo> service = requestProcessor.GetService(response);
    if (service == null)
      return;
    CancellationToken none = CancellationToken.None;
    TMediaInfo mediaInfo = await service.CreateAsync(response, none).ConfigureAwait(false);
    requestProcessor.Media = mediaInfo;
  }
}
