// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlMapElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLMapElement")]
public interface IHtmlMapElement : 
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
  [DomName("name")]
  string Name { get; set; }

  [DomName("areas")]
  IHtmlCollection<IHtmlAreaElement> Areas { get; }

  [DomName("images")]
  IHtmlCollection<IHtmlImageElement> Images { get; }
}
