// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlMenuItemElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLMenuItemElement")]
public interface IHtmlMenuItemElement : 
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
  [DomName("command")]
  IHtmlElement Command { get; }

  [DomName("type")]
  string Type { get; set; }

  [DomName("label")]
  string Label { get; set; }

  [DomName("icon")]
  string Icon { get; set; }

  [DomName("disabled")]
  bool IsDisabled { get; set; }

  [DomName("checked")]
  bool IsChecked { get; set; }

  [DomName("default")]
  bool IsDefault { get; set; }

  [DomName("radiogroup")]
  string RadioGroup { get; set; }
}
