// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlButtonElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLButtonElement")]
public interface IHtmlButtonElement : 
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
  [DomName("autofocus")]
  bool Autofocus { get; set; }

  [DomName("disabled")]
  bool IsDisabled { get; set; }

  [DomName("form")]
  IHtmlFormElement Form { get; }

  [DomName("labels")]
  INodeList Labels { get; }

  [DomName("name")]
  string Name { get; set; }

  [DomName("type")]
  string Type { get; set; }

  [DomName("value")]
  string Value { get; set; }

  [DomName("formAction")]
  string FormAction { get; set; }

  [DomName("formEncType")]
  string FormEncType { get; set; }

  [DomName("formMethod")]
  string FormMethod { get; set; }

  [DomName("formNoValidate")]
  bool FormNoValidate { get; set; }

  [DomName("formTarget")]
  string FormTarget { get; set; }
}
