// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlOutputElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLOutputElement")]
public interface IHtmlOutputElement : 
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  IValidation
{
  [DomName("htmlFor")]
  ISettableTokenList HtmlFor { get; }

  [DomName("defaultValue")]
  string DefaultValue { get; set; }

  [DomName("value")]
  string Value { get; set; }

  [DomName("labels")]
  INodeList Labels { get; }

  [DomName("type")]
  string Type { get; }

  [DomName("form")]
  IHtmlFormElement Form { get; }

  [DomName("name")]
  string Name { get; set; }
}
