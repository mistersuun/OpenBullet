// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.Processors.ObjectRequestProcessor
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Media;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Io.Processors;

internal sealed class ObjectRequestProcessor(IBrowsingContext context) : 
  ResourceRequestProcessor<IObjectInfo>(context)
{
  public int Width
  {
    get
    {
      IObjectInfo resource = this.Resource;
      return resource == null ? 0 : resource.Width;
    }
  }

  public int Height
  {
    get
    {
      IObjectInfo resource = this.Resource;
      return resource == null ? 0 : resource.Height;
    }
  }

  protected override async Task ProcessResponseAsync(IResponse response)
  {
    ObjectRequestProcessor requestProcessor = this;
    IResourceService<IObjectInfo> service = requestProcessor.GetService(response);
    if (service == null)
      return;
    CancellationToken none = CancellationToken.None;
    IObjectInfo objectInfo = await service.CreateAsync(response, none).ConfigureAwait(false);
    requestProcessor.Resource = objectInfo;
  }
}
