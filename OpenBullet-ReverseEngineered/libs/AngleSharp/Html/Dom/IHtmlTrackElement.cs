// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlTrackElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Media.Dom;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLTrackElement")]
public interface IHtmlTrackElement : 
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
  [DomName("kind")]
  string Kind { get; set; }

  [DomName("src")]
  string Source { get; set; }

  [DomName("srclang")]
  string SourceLanguage { get; set; }

  [DomName("label")]
  string Label { get; set; }

  [DomName("default")]
  bool IsDefault { get; set; }

  [DomName("readyState")]
  TrackReadyState ReadyState { get; }

  [DomName("track")]
  ITextTrack Track { get; }
}
