// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlMetaElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Browser;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlMetaElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.Meta, prefix, NodeFlags.SelfClosing | NodeFlags.Special),
  IHtmlMetaElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers
{
  public string Content
  {
    get => this.GetOwnAttribute(AttributeNames.Content);
    set => this.SetOwnAttribute(AttributeNames.Content, value);
  }

  public string Charset
  {
    get => this.GetOwnAttribute(AttributeNames.Charset);
    set => this.SetOwnAttribute(AttributeNames.Charset, value);
  }

  public string HttpEquivalent
  {
    get => this.GetOwnAttribute(AttributeNames.HttpEquiv);
    set => this.SetOwnAttribute(AttributeNames.HttpEquiv, value);
  }

  public string Scheme
  {
    get => this.GetOwnAttribute(AttributeNames.Scheme);
    set => this.SetOwnAttribute(AttributeNames.Scheme, value);
  }

  public string Name
  {
    get => this.GetOwnAttribute(AttributeNames.Name);
    set => this.SetOwnAttribute(AttributeNames.Name, value);
  }

  public void Handle()
  {
    foreach (IMetaHandler service in this.Owner.Context.GetServices<IMetaHandler>())
      service.HandleContent((IHtmlMetaElement) this);
  }
}
