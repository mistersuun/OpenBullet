// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.HtmlMediaElement`1
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Io;
using AngleSharp.Io.Processors;
using AngleSharp.Media;
using AngleSharp.Media.Dom;
using System;

#nullable disable
namespace AngleSharp.Html.Dom;

internal abstract class HtmlMediaElement<TResource> : 
  HtmlElement,
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
  where TResource : class, IMediaInfo
{
  private readonly MediaRequestProcessor<TResource> _request;
  private ITextTrackList _texts;

  public HtmlMediaElement(Document owner, string name, string prefix)
    : base(owner, name, prefix)
  {
    this._request = new MediaRequestProcessor<TResource>(owner.Context);
  }

  public IDownload CurrentDownload => this._request?.Download;

  public string Source
  {
    get => this.GetUrlAttribute(AttributeNames.Src);
    set => this.SetOwnAttribute(AttributeNames.Src, value);
  }

  public string CrossOrigin
  {
    get => this.GetOwnAttribute(AttributeNames.CrossOrigin);
    set => this.SetOwnAttribute(AttributeNames.CrossOrigin, value);
  }

  public string Preload
  {
    get => this.GetOwnAttribute(AttributeNames.Preload);
    set => this.SetOwnAttribute(AttributeNames.Preload, value);
  }

  public MediaNetworkState NetworkState
  {
    get
    {
      MediaRequestProcessor<TResource> request = this._request;
      return request == null ? MediaNetworkState.Empty : request.NetworkState;
    }
  }

  public TResource Media
  {
    get
    {
      MediaRequestProcessor<TResource> request = this._request;
      return request == null ? default (TResource) : request.Resource;
    }
  }

  public MediaReadyState ReadyState
  {
    get
    {
      IMediaController controller = this.Controller;
      return controller != null ? controller.ReadyState : MediaReadyState.Nothing;
    }
  }

  public bool IsSeeking { get; protected set; }

  public string CurrentSource => this.Source;

  public double Duration
  {
    get
    {
      IMediaController controller = this.Controller;
      return controller == null ? 0.0 : controller.Duration;
    }
  }

  public double CurrentTime
  {
    get
    {
      IMediaController controller = this.Controller;
      return controller == null ? 0.0 : controller.CurrentTime;
    }
    set
    {
      IMediaController controller = this.Controller;
      if (controller == null)
        return;
      controller.CurrentTime = value;
    }
  }

  public bool IsAutoplay
  {
    get => this.GetBoolAttribute(AttributeNames.Autoplay);
    set => this.SetBoolAttribute(AttributeNames.Autoplay, value);
  }

  public bool IsLoop
  {
    get => this.GetBoolAttribute(AttributeNames.Loop);
    set => this.SetBoolAttribute(AttributeNames.Loop, value);
  }

  public bool IsShowingControls
  {
    get => this.GetBoolAttribute(AttributeNames.Controls);
    set => this.SetBoolAttribute(AttributeNames.Controls, value);
  }

  public bool IsDefaultMuted
  {
    get => this.GetBoolAttribute(AttributeNames.Muted);
    set => this.SetBoolAttribute(AttributeNames.Muted, value);
  }

  public bool IsPaused
  {
    get
    {
      return this.PlaybackState == MediaControllerPlaybackState.Waiting && this.ReadyState >= MediaReadyState.CurrentData;
    }
  }

  public bool IsEnded => this.PlaybackState == MediaControllerPlaybackState.Ended;

  public DateTime StartDate => DateTime.Today;

  public ITimeRanges BufferedTime => this.Controller?.BufferedTime;

  public ITimeRanges SeekableTime => this.Controller?.SeekableTime;

  public ITimeRanges PlayedTime => this.Controller?.PlayedTime;

  public string MediaGroup
  {
    get => this.GetOwnAttribute(AttributeNames.MediaGroup);
    set => this.SetOwnAttribute(AttributeNames.MediaGroup, value);
  }

  public double Volume
  {
    get
    {
      IMediaController controller = this.Controller;
      return controller == null ? 1.0 : controller.Volume;
    }
    set
    {
      IMediaController controller = this.Controller;
      if (controller == null)
        return;
      controller.Volume = value;
    }
  }

  public bool IsMuted
  {
    get
    {
      IMediaController controller = this.Controller;
      return controller != null && controller.IsMuted;
    }
    set
    {
      IMediaController controller = this.Controller;
      if (controller == null)
        return;
      controller.IsMuted = value;
    }
  }

  public IMediaController Controller
  {
    get
    {
      MediaRequestProcessor<TResource> request = this._request;
      if (request == null)
        return (IMediaController) null;
      return request.Resource?.Controller;
    }
  }

  public double DefaultPlaybackRate
  {
    get
    {
      IMediaController controller = this.Controller;
      return controller == null ? 1.0 : controller.DefaultPlaybackRate;
    }
    set
    {
      IMediaController controller = this.Controller;
      if (controller == null)
        return;
      controller.DefaultPlaybackRate = value;
    }
  }

  public double PlaybackRate
  {
    get
    {
      IMediaController controller = this.Controller;
      return controller == null ? 1.0 : controller.PlaybackRate;
    }
    set
    {
      IMediaController controller = this.Controller;
      if (controller == null)
        return;
      controller.PlaybackRate = value;
    }
  }

  public MediaControllerPlaybackState PlaybackState
  {
    get
    {
      IMediaController controller = this.Controller;
      return controller == null ? MediaControllerPlaybackState.Waiting : controller.PlaybackState;
    }
  }

  public IMediaError MediaError { get; private set; }

  public virtual IAudioTrackList AudioTracks => (IAudioTrackList) null;

  public virtual IVideoTrackList VideoTracks => (IVideoTrackList) null;

  public ITextTrackList TextTracks
  {
    get => this._texts;
    protected set => this._texts = value;
  }

  public void Load() => this.UpdateSource(this.CurrentSource);

  public void Play() => this.Controller?.Play();

  public void Pause() => this.Controller?.Pause();

  public string CanPlayType(string type)
  {
    IBrowsingContext context = this.Context;
    return (context != null ? context.GetResourceService<TResource>(type) : (IResourceService<TResource>) null) == null ? string.Empty : "maybe";
  }

  public ITextTrack AddTextTrack(string kind, string label = null, string language = null)
  {
    return (ITextTrack) null;
  }

  internal override void SetupElement()
  {
    base.SetupElement();
    string ownAttribute = this.GetOwnAttribute(AttributeNames.Src);
    if (ownAttribute == null)
      return;
    this.UpdateSource(ownAttribute);
  }

  internal void UpdateSource(string value)
  {
    this.Process((IRequestProcessor) this._request, new Url(value));
  }
}
