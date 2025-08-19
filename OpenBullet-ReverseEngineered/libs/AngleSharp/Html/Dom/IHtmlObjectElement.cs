// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlObjectElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLObjectElement")]
public interface IHtmlObjectElement : 
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  IValidation,
  ILoadableElement
{
  [DomName("data")]
  string Source { get; set; }

  [DomName("type")]
  string Type { get; set; }

  [DomName("typeMustMatch")]
  bool TypeMustMatch { get; set; }

  [DomName("name")]
  string Name { get; set; }

  [DomName("useMap")]
  string UseMap { get; set; }

  [DomName("form")]
  IHtmlFormElement Form { get; }

  [DomName("width")]
  int DisplayWidth { get; set; }

  [DomName("height")]
  int DisplayHeight { get; set; }

  [DomName("contentDocument")]
  IDocument ContentDocument { get; }

  [DomName("contentWindow")]
  IWindow ContentWindow { get; }
}
