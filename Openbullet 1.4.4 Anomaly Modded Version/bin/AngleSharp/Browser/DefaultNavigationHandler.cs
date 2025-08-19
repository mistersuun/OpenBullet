// Decompiled with JetBrains decompiler
// Type: AngleSharp.Browser.DefaultNavigationHandler
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Browser;

internal sealed class DefaultNavigationHandler : INavigationHandler
{
  private readonly IBrowsingContext _context;

  public DefaultNavigationHandler(IBrowsingContext context) => this._context = context;

  public async Task<IDocument> NavigateAsync(DocumentRequest request, CancellationToken cancel)
  {
    IBrowsingContext context = this._context.ResolveTargetContext(request.Source is HtmlUrlBaseElement source ? source.Target : (string) null);
    IDocumentLoader service = context.GetService<IDocumentLoader>();
    if (service != null)
    {
      IDownload download = service.FetchAsync(request);
      cancel.Register(new Action(((ICancellable) download).Cancel));
      using (IResponse response = await download.Task.ConfigureAwait(false))
      {
        if (response != null)
          return await context.OpenAsync(response, cancel).ConfigureAwait(false);
      }
    }
    return await context.OpenNewAsync(request.Target.Href, cancel).ConfigureAwait(false);
  }

  public bool SupportsProtocol(string protocol)
  {
    return this._context.GetServices<IRequester>().Any<IRequester>((Func<IRequester, bool>) (m => m.SupportsProtocol(protocol)));
  }
}
