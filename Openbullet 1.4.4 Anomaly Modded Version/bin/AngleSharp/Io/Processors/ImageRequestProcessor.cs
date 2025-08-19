// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.Processors.ImageRequestProcessor
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Media;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Io.Processors;

internal sealed class ImageRequestProcessor(IBrowsingContext context) : 
  ResourceRequestProcessor<IImageInfo>(context)
{
  public int Width => !this.IsReady ? 0 : this.Resource.Width;

  public int Height => !this.IsReady ? 0 : this.Resource.Height;

  protected override async Task ProcessResponseAsync(IResponse response)
  {
    ImageRequestProcessor requestProcessor = this;
    IResourceService<IImageInfo> service = requestProcessor.GetService(response);
    if (service == null)
      return;
    CancellationToken none = CancellationToken.None;
    IImageInfo imageInfo = await service.CreateAsync(response, none).ConfigureAwait(false);
    requestProcessor.Resource = imageInfo;
  }
}
