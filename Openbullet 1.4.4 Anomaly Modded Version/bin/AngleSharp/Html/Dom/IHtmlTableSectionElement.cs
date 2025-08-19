// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlTableSectionElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLTableSectionElement")]
public interface IHtmlTableSectionElement : 
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
  [DomName("rows")]
  IHtmlCollection<IHtmlTableRowElement> Rows { get; }

  [DomName("insertRow")]
  IHtmlTableRowElement InsertRowAt(int index = -1);

  [DomName("deleteRow")]
  void RemoveRowAt(int index);
}
