// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlImageElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLImageElement")]
public interface IHtmlImageElement : 
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  ILoadableElement
{
  [DomName("alt")]
  string AlternativeText { get; set; }

  [DomName("currentSrc")]
  string ActualSource { get; }

  [DomName("src")]
  string Source { get; set; }

  [DomName("srcset")]
  string SourceSet { get; set; }

  [DomName("sizes")]
  string Sizes { get; set; }

  [DomName("crossOrigin")]
  string CrossOrigin { get; set; }

  [DomName("useMap")]
  string UseMap { get; set; }

  [DomName("isMap")]
  bool IsMap { get; set; }

  [DomName("width")]
  int DisplayWidth { get; set; }

  [DomName("height")]
  int DisplayHeight { get; set; }

  [DomName("naturalWidth")]
  int OriginalWidth { get; }

  [DomName("naturalHeight")]
  int OriginalHeight { get; }

  [DomName("complete")]
  bool IsCompleted { get; }
}
