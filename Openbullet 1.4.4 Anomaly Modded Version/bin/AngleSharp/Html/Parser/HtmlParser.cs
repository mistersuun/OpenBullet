// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.HtmlParser
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Browser;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Dom.Events;
using AngleSharp.Text;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Html.Parser;

public class HtmlParser : EventTarget, IHtmlParser, IParser, IEventTarget
{
  private readonly HtmlParserOptions _options;
  private readonly IBrowsingContext _context;

  public event DomEventHandler Parsing
  {
    add => this.AddEventListener(EventNames.Parsing, value, false);
    remove => this.RemoveEventListener(EventNames.Parsing, value, false);
  }

  public event DomEventHandler Parsed
  {
    add => this.AddEventListener(EventNames.Parsed, value, false);
    remove => this.RemoveEventListener(EventNames.Parsed, value, false);
  }

  public event DomEventHandler Error
  {
    add => this.AddEventListener(EventNames.Error, value, false);
    remove => this.RemoveEventListener(EventNames.Error, value, false);
  }

  public HtmlParser()
    : this((IBrowsingContext) null)
  {
  }

  public HtmlParser(HtmlParserOptions options)
    : this(options, (IBrowsingContext) null)
  {
  }

  internal HtmlParser(IBrowsingContext context)
    : this(new HtmlParserOptions()
    {
      IsScripting = context != null && context.IsScripting()
    }, context)
  {
  }

  public HtmlParser(HtmlParserOptions options, IBrowsingContext context)
  {
    this._options = options;
    this._context = context ?? BrowsingContext.NewFrom<IHtmlParser>((IHtmlParser) this);
  }

  public HtmlParserOptions Options => this._options;

  public IHtmlDocument ParseDocument(string source) => this.Parse(this.CreateDocument(source));

  public INodeList ParseFragment(string source, IElement contextElement)
  {
    HtmlDocument document = this.CreateDocument(source);
    HtmlDomBuilder htmlDomBuilder = new HtmlDomBuilder(document);
    if (!(contextElement is Element))
      return (INodeList) htmlDomBuilder.Parse(this._options).ChildNodes;
    IBrowsingContext context = document.Context;
    Element elementFrom = document.CreateElementFrom(contextElement.LocalName, contextElement.Prefix);
    IElement documentElement = htmlDomBuilder.ParseFragment(this._options, elementFrom).DocumentElement;
    elementFrom.AppendNodes(documentElement.ChildNodes.ToArray<INode>());
    return (INodeList) elementFrom.ChildNodes;
  }

  public IHtmlDocument ParseDocument(Stream source) => this.Parse(this.CreateDocument(source));

  public Task<IHtmlDocument> ParseDocumentAsync(string source, CancellationToken cancel)
  {
    return this.ParseAsync(this.CreateDocument(source), cancel);
  }

  public Task<IHtmlDocument> ParseDocumentAsync(Stream source, CancellationToken cancel)
  {
    return this.ParseAsync(this.CreateDocument(source), cancel);
  }

  async Task<IDocument> IHtmlParser.ParseDocumentAsync(IDocument document, CancellationToken cancel)
  {
    return (IDocument) await this.ParseAsync((HtmlDocument) document, cancel).ConfigureAwait(false);
  }

  private HtmlDocument CreateDocument(string source) => this.CreateDocument(new TextSource(source));

  private HtmlDocument CreateDocument(Stream source)
  {
    Encoding defaultEncoding = this._context.GetDefaultEncoding();
    return this.CreateDocument(new TextSource(source, defaultEncoding));
  }

  private HtmlDocument CreateDocument(TextSource textSource)
  {
    return new HtmlDocument(this._context, textSource);
  }

  private HtmlDomBuilder CreateBuilder(HtmlDocument document)
  {
    HtmlDomBuilder builder = new HtmlDomBuilder(document);
    if (this.HasEventListener(EventNames.Error))
      builder.Error += (EventHandler<HtmlErrorEvent>) ((s, ev) => this.InvokeEventListener((Event) ev));
    return builder;
  }

  private IHtmlDocument Parse(HtmlDocument document)
  {
    HtmlDomBuilder builder = this.CreateBuilder(document);
    this.InvokeEventListener((Event) new HtmlParseEvent((IHtmlDocument) document, false));
    HtmlParserOptions options = this._options;
    builder.Parse(options);
    this.InvokeEventListener((Event) new HtmlParseEvent((IHtmlDocument) document, true));
    return (IHtmlDocument) document;
  }

  private async Task<IHtmlDocument> ParseAsync(HtmlDocument document, CancellationToken cancel)
  {
    HtmlParser htmlParser = this;
    HtmlDomBuilder builder = htmlParser.CreateBuilder(document);
    // ISSUE: explicit non-virtual call
    __nonvirtual (htmlParser.InvokeEventListener((Event) new HtmlParseEvent((IHtmlDocument) document, false)));
    HtmlParserOptions options = htmlParser._options;
    CancellationToken cancelToken = cancel;
    HtmlDocument htmlDocument = await builder.ParseAsync(options, cancelToken).ConfigureAwait(false);
    // ISSUE: explicit non-virtual call
    __nonvirtual (htmlParser.InvokeEventListener((Event) new HtmlParseEvent((IHtmlDocument) document, true)));
    return (IHtmlDocument) document;
  }
}
