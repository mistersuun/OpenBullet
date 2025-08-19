// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlVideoElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;
using AngleSharp.Media.Dom;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLVideoElement")]
public interface IHtmlVideoElement : 
  IHtmlMediaElement,
  IHtmlElement,
  IElement,
  INode,
  IEventTarget,
  IMarkupFormattable,
  IParentNode,
  IChildNode,
  INonDocumentTypeChildNode,
  IGlobalEventHandlers,
  IMediaController,
  ILoadableElement
{
  [DomName("width")]
  int DisplayWidth { get; set; }

  [DomName("height")]
  int DisplayHeight { get; set; }

  [DomName("videoWidth")]
  int OriginalWidth { get; }

  [DomName("videoHeight")]
  int OriginalHeight { get; }

  [DomName("poster")]
  string Poster { get; set; }
}
