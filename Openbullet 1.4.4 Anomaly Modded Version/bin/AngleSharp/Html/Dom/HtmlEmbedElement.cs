// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlEmbedElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;
using AngleSharp.Io.Processors;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlEmbedElement : 
  HtmlElement,
  IHtmlEmbedElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  ILoadableElement
{
  private readonly ObjectRequestProcessor _request;

  public HtmlEmbedElement(Document owner, string prefix = null)
    : base(owner, TagNames.Embed, prefix, NodeFlags.SelfClosing | NodeFlags.Special)
  {
    this._request = new ObjectRequestProcessor(owner.Context);
  }

  public IDownload CurrentDownload => this._request?.Download;

  public string Source
  {
    get => this.GetOwnAttribute(AttributeNames.Src);
    set => this.SetOwnAttribute(AttributeNames.Src, value);
  }

  public string Type
  {
    get => this.GetOwnAttribute(AttributeNames.Type);
    set => this.SetOwnAttribute(AttributeNames.Type, value);
  }

  public string DisplayWidth
  {
    get => this.GetOwnAttribute(AttributeNames.Width);
    set => this.SetOwnAttribute(AttributeNames.Width, value);
  }

  public string DisplayHeight
  {
    get => this.GetOwnAttribute(AttributeNames.Height);
    set => this.SetOwnAttribute(AttributeNames.Height, value);
  }

  internal override void SetupElement()
  {
    base.SetupElement();
    string ownAttribute = this.GetOwnAttribute(AttributeNames.Src);
    if (ownAttribute == null)
      return;
    this.UpdateSource(ownAttribute);
  }

  internal void UpdateSource(string value)
  {
    this.Process((IRequestProcessor) this._request, new Url(this.Source));
  }
}
