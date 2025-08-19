// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlParamElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlParamElement(Document owner, string prefix = null) : 
  HtmlElement(owner, TagNames.Param, prefix, NodeFlags.SelfClosing | NodeFlags.Special),
  IHtmlParamElement,
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
  public string Value
  {
    get => this.GetOwnAttribute(AttributeNames.Value);
    set => this.SetOwnAttribute(AttributeNames.Value, value);
  }

  public string Name
  {
    get => this.GetOwnAttribute(AttributeNames.Name);
    set => this.SetOwnAttribute(AttributeNames.Name, value);
  }
}
