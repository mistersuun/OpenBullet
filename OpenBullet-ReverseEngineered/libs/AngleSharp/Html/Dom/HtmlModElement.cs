// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlModElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlModElement(Document owner, string name = null, string prefix = null) : 
  HtmlElement(owner, name ?? TagNames.Ins, prefix),
  IHtmlModElement,
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
  public string Citation
  {
    get => this.GetOwnAttribute(AttributeNames.Cite);
    set => this.SetOwnAttribute(AttributeNames.Cite, value);
  }

  public string DateTime
  {
    get => this.GetOwnAttribute(AttributeNames.Datetime);
    set => this.SetOwnAttribute(AttributeNames.Datetime, value);
  }
}
