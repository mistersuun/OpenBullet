// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlOptionElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLOptionElement")]
public interface IHtmlOptionElement : 
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
  [DomName("disabled")]
  bool IsDisabled { get; set; }

  [DomName("form")]
  IHtmlFormElement Form { get; }

  [DomName("label")]
  string Label { get; set; }

  [DomName("defaultSelected")]
  bool IsDefaultSelected { get; set; }

  [DomName("selected")]
  bool IsSelected { get; set; }

  [DomName("value")]
  string Value { get; set; }

  [DomName("text")]
  string Text { get; set; }

  [DomName("index")]
  int Index { get; }
}
