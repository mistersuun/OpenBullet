// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlAreaElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlAreaElement(Document owner, string prefix = null) : 
  HtmlUrlBaseElement(owner, TagNames.Area, prefix, NodeFlags.SelfClosing | NodeFlags.Special),
  IHtmlAreaElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  IUrlUtilities
{
  public string AlternativeText
  {
    get => this.GetOwnAttribute(AttributeNames.Alt);
    set => this.SetOwnAttribute(AttributeNames.Alt, value);
  }

  public string Coordinates
  {
    get => this.GetOwnAttribute(AttributeNames.Coords);
    set => this.SetOwnAttribute(AttributeNames.Coords, value);
  }

  public string Shape
  {
    get => this.GetOwnAttribute(AttributeNames.Shape);
    set => this.SetOwnAttribute(AttributeNames.Shape, value);
  }
}
