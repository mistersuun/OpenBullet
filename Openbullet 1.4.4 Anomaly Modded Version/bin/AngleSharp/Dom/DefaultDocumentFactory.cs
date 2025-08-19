// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.DefaultDocumentFactory
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Io;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Dom;

public class DefaultDocumentFactory : IDocumentFactory
{
  private readonly Dictionary<string, DefaultDocumentFactory.Creator> _creators = new Dictionary<string, DefaultDocumentFactory.Creator>()
  {
    {
      MimeTypeNames.Html,
      new DefaultDocumentFactory.Creator(DefaultDocumentFactory.LoadHtmlAsync)
    },
    {
      MimeTypeNames.ApplicationXHtml,
      new DefaultDocumentFactory.Creator(DefaultDocumentFactory.LoadHtmlAsync)
    },
    {
      MimeTypeNames.Plain,
      new DefaultDocumentFactory.Creator(DefaultDocumentFactory.LoadTextAsync)
    },
    {
      MimeTypeNames.ApplicationJson,
      new DefaultDocumentFactory.Creator(DefaultDocumentFactory.LoadTextAsync)
    },
    {
      MimeTypeNames.DefaultJavaScript,
      new DefaultDocumentFactory.Creator(DefaultDocumentFactory.LoadTextAsync)
    },
    {
      MimeTypeNames.Css,
      new DefaultDocumentFactory.Creator(DefaultDocumentFactory.LoadTextAsync)
    }
  };

  public virtual void Register(string contentType, DefaultDocumentFactory.Creator creator)
  {
    this._creators.Add(contentType, creator);
  }

  public virtual DefaultDocumentFactory.Creator Unregister(string contentType)
  {
    DefaultDocumentFactory.Creator creator;
    if (this._creators.TryGetValue(contentType, out creator))
      this._creators.Remove(contentType);
    return creator;
  }

  protected virtual Task<IDocument> CreateDefaultAsync(
    IBrowsingContext context,
    CreateDocumentOptions options,
    CancellationToken cancellationToken)
  {
    return DefaultDocumentFactory.LoadHtmlAsync(context, options, cancellationToken);
  }

  public virtual Task<IDocument> CreateAsync(
    IBrowsingContext context,
    CreateDocumentOptions options,
    CancellationToken cancellationToken)
  {
    MimeType contentType = options.ContentType;
    foreach (KeyValuePair<string, DefaultDocumentFactory.Creator> creator in this._creators)
    {
      if (contentType.Represents(creator.Key))
        return creator.Value(context, options, cancellationToken);
    }
    return this.CreateDefaultAsync(context, options, cancellationToken);
  }

  protected static Task<IDocument> LoadHtmlAsync(
    IBrowsingContext context,
    CreateDocumentOptions options,
    CancellationToken cancellationToken)
  {
    IHtmlParser service = context.GetService<IHtmlParser>();
    HtmlDocument htmlDocument1 = new HtmlDocument(context, options.Source);
    htmlDocument1.Setup(options.Response, options.ContentType, options.ImportAncestor);
    context.NavigateTo((IDocument) htmlDocument1);
    HtmlDocument htmlDocument2 = htmlDocument1;
    CancellationToken cancel = cancellationToken;
    return service.ParseDocumentAsync((IDocument) htmlDocument2, cancel);
  }

  protected static async Task<IDocument> LoadTextAsync(
    IBrowsingContext context,
    CreateDocumentOptions options,
    CancellationToken cancellationToken)
  {
    HtmlDocument document = new HtmlDocument(context, options.Source);
    document.Setup(options.Response, options.ContentType, options.ImportAncestor);
    context.NavigateTo((IDocument) document);
    IElement element1 = document.CreateElement(TagNames.Html);
    IElement element2 = document.CreateElement(TagNames.Head);
    IElement element3 = document.CreateElement(TagNames.Body);
    IElement pre = document.CreateElement(TagNames.Pre);
    document.AppendChild((INode) element1);
    element1.AppendChild((INode) element2);
    element1.AppendChild((INode) element3);
    element3.AppendChild((INode) pre);
    pre.SetAttribute(AttributeNames.Style, "word-wrap: break-word; white-space: pre-wrap;");
    await options.Source.PrefetchAllAsync(cancellationToken).ConfigureAwait(false);
    pre.TextContent = options.Source.Text;
    return (IDocument) document;
  }

  public delegate Task<IDocument> Creator(
    IBrowsingContext context,
    CreateDocumentOptions options,
    CancellationToken cancellationToken);
}
