// Decompiled with JetBrains decompiler
// Type: AngleSharp.Media.Dom.IMediaController
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Media.Dom;

[DomName("MediaController")]
public interface IMediaController
{
  [DomName("buffered")]
  ITimeRanges BufferedTime { get; }

  [DomName("seekable")]
  ITimeRanges SeekableTime { get; }

  [DomName("played")]
  ITimeRanges PlayedTime { get; }

  [DomName("duration")]
  double Duration { get; }

  [DomName("currentTime")]
  double CurrentTime { get; set; }

  [DomName("defaultPlaybackRate")]
  double DefaultPlaybackRate { get; set; }

  [DomName("playbackRate")]
  double PlaybackRate { get; set; }

  [DomName("volume")]
  double Volume { get; set; }

  [DomName("muted")]
  bool IsMuted { get; set; }

  [DomName("paused")]
  bool IsPaused { get; }

  [DomName("play")]
  void Play();

  [DomName("pause")]
  void Pause();

  [DomName("readyState")]
  MediaReadyState ReadyState { get; }

  [DomName("playbackState")]
  MediaControllerPlaybackState PlaybackState { get; }

  [DomName("onemptied")]
  event DomEventHandler Emptied;

  [DomName("onloadedmetadata")]
  event DomEventHandler LoadedMetadata;

  [DomName("onloadeddata")]
  event DomEventHandler LoadedData;

  [DomName("oncanplay")]
  event DomEventHandler CanPlay;

  [DomName("oncanplaythrough")]
  event DomEventHandler CanPlayThrough;

  [DomName("onended")]
  event DomEventHandler Ended;

  [DomName("onwaiting")]
  event DomEventHandler Waiting;

  [DomName("ondurationchange")]
  event DomEventHandler DurationChanged;

  [DomName("ontimeupdate")]
  event DomEventHandler TimeUpdated;

  [DomName("onpause")]
  event DomEventHandler Paused;

  [DomName("onplay")]
  event DomEventHandler Played;

  [DomName("onplaying")]
  event DomEventHandler Playing;

  [DomName("onratechange")]
  event DomEventHandler RateChanged;

  [DomName("onvolumechange")]
  event DomEventHandler VolumeChanged;
}
