// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlFrameElementBase
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Browser;
using AngleSharp.Dom;
using AngleSharp.Io;
using AngleSharp.Io.Processors;

#nullable disable
namespace AngleSharp.Html.Dom;

internal abstract class HtmlFrameElementBase : HtmlFrameOwnerElement
{
  private IBrowsingContext _context;
  private FrameRequestProcessor _request;

  public HtmlFrameElementBase(Document owner, string name, string prefix, NodeFlags flags = NodeFlags.None)
    : base(owner, name, prefix, flags | NodeFlags.Special)
  {
    this._request = new FrameRequestProcessor(owner.Context, this);
  }

  public IDownload CurrentDownload => this._request?.Download;

  public string Name
  {
    get => this.GetOwnAttribute(AttributeNames.Name);
    set => this.SetOwnAttribute(AttributeNames.Name, value);
  }

  public string Source
  {
    get => this.GetUrlAttribute(AttributeNames.Src);
    set => this.SetOwnAttribute(AttributeNames.Src, value);
  }

  public string Scrolling
  {
    get => this.GetOwnAttribute(AttributeNames.Scrolling);
    set => this.SetOwnAttribute(AttributeNames.Scrolling, value);
  }

  public IDocument ContentDocument => this._request?.Document;

  public string LongDesc
  {
    get => this.GetOwnAttribute(AttributeNames.LongDesc);
    set => this.SetOwnAttribute(AttributeNames.LongDesc, value);
  }

  public string FrameBorder
  {
    get => this.GetOwnAttribute(AttributeNames.FrameBorder);
    set => this.SetOwnAttribute(AttributeNames.FrameBorder, value);
  }

  public IBrowsingContext NestedContext
  {
    get => this._context ?? (this._context = this.NewChildContext());
  }

  internal virtual string GetContentHtml() => (string) null;

  internal override void SetupElement()
  {
    base.SetupElement();
    if (this.GetOwnAttribute(AttributeNames.Src) == null)
      return;
    this.UpdateSource();
  }

  internal void UpdateSource()
  {
    string contentHtml = this.GetContentHtml();
    string source = this.Source;
    if ((source == null || !(source != this.Owner.DocumentUri)) && contentHtml == null)
      return;
    this.Process((IRequestProcessor) this._request, this.HyperReference(source));
  }

  private IBrowsingContext NewChildContext()
  {
    IBrowsingContext child = this.Context.CreateChild((string) null, Sandboxes.None);
    this.Owner.AttachReference((object) child);
    return child;
  }
}
