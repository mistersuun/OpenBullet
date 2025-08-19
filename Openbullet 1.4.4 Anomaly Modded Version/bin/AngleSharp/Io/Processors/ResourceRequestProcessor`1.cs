// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.Processors.ResourceRequestProcessor`1
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Media;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Io.Processors;

internal abstract class ResourceRequestProcessor<TResource> : BaseRequestProcessor where TResource : IResourceInfo
{
  private readonly IBrowsingContext _context;

  public ResourceRequestProcessor(IBrowsingContext context)
    : base(context?.GetService<IResourceLoader>())
  {
    this._context = context;
  }

  public string Source
  {
    get
    {
      TResource resource1 = this.Resource;
      ref TResource local1 = ref resource1;
      string str;
      if ((object) default (TResource) == null)
      {
        TResource resource2 = local1;
        ref TResource local2 = ref resource2;
        if ((object) resource2 == null)
        {
          str = (string) null;
          goto label_4;
        }
        local1 = ref local2;
      }
      str = local1.Source.Href;
label_4:
      return str ?? string.Empty;
    }
  }

  public bool IsReady => (object) this.Resource != null;

  public TResource Resource { get; protected set; }

  public override Task ProcessAsync(ResourceRequest request)
  {
    return this.IsAvailable && this.IsDifferentToCurrentResourceUrl(request.Target) ? base.ProcessAsync(request) : Task.CompletedTask;
  }

  protected IResourceService<TResource> GetService(IResponse response)
  {
    return this._context.GetResourceService<TResource>(response.GetContentType().Content);
  }

  private bool IsDifferentToCurrentResourceUrl(Url target)
  {
    TResource resource = this.Resource;
    return (object) resource == null || !target.Equals(resource.Source);
  }
}
