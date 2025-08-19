// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlSelectElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLSelectElement")]
public interface IHtmlSelectElement : 
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

  [DomName("value")]
  string Value { get; set; }

  [DomName("type")]
  string Type { get; }

  [DomName("required")]
  bool IsRequired { get; set; }

  [DomName("selectedOptions")]
  IHtmlCollection<IHtmlOptionElement> SelectedOptions { get; }

  [DomName("size")]
  int Size { get; set; }

  [DomName("options")]
  IHtmlOptionsCollection Options { get; }

  [DomName("length")]
  int Length { get; }

  [DomName("multiple")]
  bool IsMultiple { get; set; }

  [DomName("selectedIndex")]
  int SelectedIndex { get; }

  [DomAccessor(Accessors.Getter | Accessors.Setter)]
  IHtmlOptionElement this[int index] { get; set; }

  [DomName("add")]
  void AddOption(IHtmlOptionElement element, IHtmlElement before = null);

  [DomName("add")]
  void AddOption(IHtmlOptionsGroupElement element, IHtmlElement before = null);

  [DomName("remove")]
  void RemoveOptionAt(int index);
}
