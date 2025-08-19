// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlOptionsGroupElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlOptionsGroupElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.Optgroup, prefix, NodeFlags.ImplicitelyClosed | NodeFlags.ImpliedEnd | NodeFlags.HtmlSelectScoped),
  IHtmlOptionsGroupElement,
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
  public string Label
  {
    get => this.GetOwnAttribute(AttributeNames.Label);
    set => this.SetOwnAttribute(AttributeNames.Label, value);
  }

  public bool IsDisabled
  {
    get => this.GetBoolAttribute(AttributeNames.Disabled);
    set => this.SetBoolAttribute(AttributeNames.Disabled, value);
  }
}
