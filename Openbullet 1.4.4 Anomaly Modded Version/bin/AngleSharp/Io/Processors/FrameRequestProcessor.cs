// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.Processors.FrameRequestProcessor
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Io.Processors;

internal sealed class FrameRequestProcessor : BaseRequestProcessor
{
  private readonly HtmlFrameElementBase _element;

  public FrameRequestProcessor(IBrowsingContext context, HtmlFrameElementBase element)
    : base(context?.GetService<IResourceLoader>())
  {
    this._element = element;
  }

  public IDocument Document { get; private set; }

  public override Task ProcessAsync(ResourceRequest request)
  {
    string contentHtml = this._element.GetContentHtml();
    if (contentHtml == null)
      return base.ProcessAsync(request);
    string documentUri = this._element.Owner.DocumentUri;
    return this.ProcessResponse(contentHtml, documentUri);
  }

  protected override Task ProcessResponseAsync(IResponse response)
  {
    CancellationToken none = CancellationToken.None;
    return this.WaitResponse(this._element.NestedContext.OpenAsync(response, none));
  }

  private Task ProcessResponse(string response, string referer)
  {
    return this.WaitResponse(this._element.NestedContext.OpenAsync((Action<VirtualResponse>) (m => m.Content(response).Address(referer)), CancellationToken.None));
  }

  private async Task WaitResponse(Task<IDocument> task)
  {
    this.Document = await task.ConfigureAwait(false);
  }
}
