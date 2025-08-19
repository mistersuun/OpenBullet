// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlDocument
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;
using AngleSharp.Mathml.Dom;
using AngleSharp.Svg.Dom;
using AngleSharp.Text;
using System;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlDocument : 
  Document,
  IHtmlDocument,
  IDocument,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IGlobalEventHandlers,
  IDocumentStyle,
  INonElementParentNode,
  IDisposable
{
  private readonly IElementFactory<Document, HtmlElement> _htmlFactory;
  private readonly IElementFactory<Document, MathElement> _mathFactory;
  private readonly IElementFactory<Document, SvgElement> _svgFactory;

  internal HtmlDocument(IBrowsingContext context, TextSource source)
    : base(context ?? BrowsingContext.New(), source)
  {
    this.ContentType = MimeTypeNames.Html;
    this._htmlFactory = this.Context.GetFactory<IElementFactory<Document, HtmlElement>>();
    this._mathFactory = this.Context.GetFactory<IElementFactory<Document, MathElement>>();
    this._svgFactory = this.Context.GetFactory<IElementFactory<Document, SvgElement>>();
  }

  internal HtmlDocument(IBrowsingContext context = null)
    : this(context, new TextSource(string.Empty))
  {
  }

  public override IElement DocumentElement => (IElement) this.FindChild<HtmlHtmlElement>();

  public override IEntityProvider Entities
  {
    get => this.Context.GetProvider<IEntityProvider>() ?? HtmlEntityProvider.Resolver;
  }

  public HtmlElement CreateHtmlElement(string name, string prefix = null)
  {
    return this._htmlFactory.Create((Document) this, name, prefix);
  }

  public MathElement CreateMathElement(string name, string prefix = null)
  {
    return this._mathFactory.Create((Document) this, name, prefix);
  }

  public SvgElement CreateSvgElement(string name, string prefix = null)
  {
    return this._svgFactory.Create((Document) this, name, prefix);
  }

  public override Element CreateElementFrom(string name, string prefix)
  {
    return (Element) this.CreateHtmlElement(name, prefix);
  }

  public override Node Clone(Document owner, bool deep)
  {
    HtmlDocument htmlDocument = new HtmlDocument(this.Context, new TextSource(this.Source.Text));
    this.CloneDocument((Document) htmlDocument, deep);
    return (Node) htmlDocument;
  }

  protected override string GetTitle()
  {
    IHtmlTitleElement descendant = this.DocumentElement.FindDescendant<IHtmlTitleElement>();
    return (descendant != null ? descendant.TextContent.CollapseAndStrip() : (string) null) ?? base.GetTitle();
  }

  protected override void SetTitle(string value)
  {
    IHtmlTitleElement child = this.DocumentElement.FindDescendant<IHtmlTitleElement>();
    if (child == null)
    {
      IHtmlHeadElement head = this.Head;
      if (head == null)
        return;
      child = (IHtmlTitleElement) new HtmlTitleElement((Document) this);
      head.AppendChild((INode) child);
    }
    child.TextContent = value;
  }
}
