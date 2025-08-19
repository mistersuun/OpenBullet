// Decompiled with JetBrains decompiler
// Type: AngleSharp.Media.Dom.ITextTrack
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Media.Dom;

[DomName("TextTrack")]
public interface ITextTrack : IEventTarget
{
  [DomName("kind")]
  string Kind { get; }

  [DomName("label")]
  string Label { get; }

  [DomName("language")]
  string Language { get; }

  [DomName("mode")]
  TextTrackMode Mode { get; set; }

  [DomName("cues")]
  ITextTrackCueList Cues { get; }

  [DomName("activeCues")]
  ITextTrackCueList ActiveCues { get; }

  [DomName("addCue")]
  void Add(ITextTrackCue cue);

  [DomName("removeCue")]
  void Remove(ITextTrackCue cue);

  [DomName("oncuechange")]
  event DomEventHandler CueChanged;
}
