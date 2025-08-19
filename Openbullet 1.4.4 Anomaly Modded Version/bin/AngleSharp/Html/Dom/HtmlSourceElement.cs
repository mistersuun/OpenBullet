// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlSourceElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlSourceElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.Source, prefix, NodeFlags.SelfClosing | NodeFlags.Special),
  IHtmlSourceElement,
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
  public string Source
  {
    get => this.GetUrlAttribute(AttributeNames.Src);
    set => this.SetOwnAttribute(AttributeNames.Src, value);
  }

  public string Media
  {
    get => this.GetOwnAttribute(AttributeNames.Media);
    set => this.SetOwnAttribute(AttributeNames.Media, value);
  }

  public string Type
  {
    get => this.GetOwnAttribute(AttributeNames.Type);
    set => this.SetOwnAttribute(AttributeNames.Type, value);
  }

  public string SourceSet
  {
    get => this.GetOwnAttribute(AttributeNames.SrcSet);
    set => this.SetOwnAttribute(AttributeNames.SrcSet, value);
  }

  public string Sizes
  {
    get => this.GetOwnAttribute(AttributeNames.Sizes);
    set => this.SetOwnAttribute(AttributeNames.Sizes, value);
  }
}
