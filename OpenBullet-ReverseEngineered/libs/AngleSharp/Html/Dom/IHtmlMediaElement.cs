// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.IHtmlMediaElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;
using AngleSharp.Media.Dom;
using System;

#nullable disable
namespace AngleSharp.Html.Dom;

[DomName("HTMLMediaElement")]
public interface IHtmlMediaElement : 
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
  [DomName("src")]
  string Source { get; set; }

  [DomName("crossOrigin")]
  string CrossOrigin { get; set; }

  [DomName("preload")]
  string Preload { get; set; }

  [DomName("mediaGroup")]
  string MediaGroup { get; set; }

  [DomName("networkState")]
  MediaNetworkState NetworkState { get; }

  [DomName("seeking")]
  bool IsSeeking { get; }

  [DomName("currentSrc")]
  string CurrentSource { get; }

  [DomName("error")]
  IMediaError MediaError { get; }

  [DomName("controller")]
  IMediaController Controller { get; }

  [DomName("ended")]
  bool IsEnded { get; }

  [DomName("autoplay")]
  bool IsAutoplay { get; set; }

  [DomName("loop")]
  bool IsLoop { get; set; }

  [DomName("controls")]
  bool IsShowingControls { get; set; }

  [DomName("defaultMuted")]
  bool IsDefaultMuted { get; set; }

  [DomName("load")]
  void Load();

  [DomName("canPlayType")]
  string CanPlayType(string type);

  [DomName("startDate")]
  DateTime StartDate { get; }

  [DomName("audioTracks")]
  IAudioTrackList AudioTracks { get; }

  [DomName("videoTracks")]
  IVideoTrackList VideoTracks { get; }

  [DomName("textTracks")]
  ITextTrackList TextTracks { get; }

  [DomName("addTextTrack")]
  ITextTrack AddTextTrack(string kind, string label = null, string language = null);
}
