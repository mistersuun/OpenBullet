// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlAudioElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;
using AngleSharp.Media;
using AngleSharp.Media.Dom;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlAudioElement : 
  HtmlMediaElement<IAudioInfo>,
  IHtmlAudioElement,
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
  private IAudioTrackList _audios;

  public HtmlAudioElement(Document owner, string prefix = null)
    : base(owner, TagNames.Audio, prefix)
  {
    this._audios = (IAudioTrackList) null;
  }

  public override IAudioTrackList AudioTracks => this._audios;
}
