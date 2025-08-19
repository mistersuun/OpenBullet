// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.Processors.DocumentRequestProcessor
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Browser;
using AngleSharp.Dom;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Io.Processors;

internal sealed class DocumentRequestProcessor : BaseRequestProcessor
{
  private readonly IDocument _parentDocument;
  private readonly IBrowsingContext _context;

  public DocumentRequestProcessor(IBrowsingContext context)
    : base(context?.GetService<IResourceLoader>())
  {
    this._parentDocument = context.Active;
    this._context = context;
  }

  public IDocument ChildDocument { get; private set; }

  protected override async Task ProcessResponseAsync(IResponse response)
  {
    BrowsingContext context = new BrowsingContext(this._context, Sandboxes.None);
    CreateDocumentOptions options = new CreateDocumentOptions(response, this._context.GetDefaultEncoding(), this._parentDocument);
    this.ChildDocument = await this._context.GetFactory<IDocumentFactory>().CreateAsync((IBrowsingContext) context, options, CancellationToken.None).ConfigureAwait(false);
  }
}
