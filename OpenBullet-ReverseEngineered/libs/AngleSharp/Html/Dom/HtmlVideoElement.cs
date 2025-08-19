// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlVideoElement
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;
using AngleSharp.Media;
using AngleSharp.Media.Dom;
using AngleSharp.Text;

#nullable disable
namespace AngleSharp.Html.Dom;

internal sealed class HtmlVideoElement : 
  HtmlMediaElement<IVideoInfo>,
  IHtmlVideoElement,
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
  private IVideoTrackList _videos;

  public HtmlVideoElement(Document owner, string prefix = null)
    : base(owner, TagNames.Video, prefix)
  {
    this._videos = (IVideoTrackList) null;
  }

  public override IVideoTrackList VideoTracks => this._videos;

  public int DisplayWidth
  {
    get => this.GetOwnAttribute(AttributeNames.Width).ToInteger(this.OriginalWidth);
    set => this.SetOwnAttribute(AttributeNames.Width, value.ToString());
  }

  public int DisplayHeight
  {
    get => this.GetOwnAttribute(AttributeNames.Height).ToInteger(this.OriginalHeight);
    set => this.SetOwnAttribute(AttributeNames.Height, value.ToString());
  }

  public int OriginalWidth
  {
    get
    {
      IVideoInfo media = this.Media;
      return media == null ? 0 : media.Width;
    }
  }

  public int OriginalHeight
  {
    get
    {
      IVideoInfo media = this.Media;
      return media == null ? 0 : media.Height;
    }
  }

  public string Poster
  {
    get => this.GetUrlAttribute(AttributeNames.Poster);
    set => this.SetOwnAttribute(AttributeNames.Poster, value);
  }
}
