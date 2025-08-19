// Decompiled with JetBrains decompiler
// Type: AngleSharp.Media.Dom.ITextTrackCue
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Media.Dom;

[DomName("TextTrackCue")]
public interface ITextTrackCue : IEventTarget
{
  [DomName("id")]
  string Id { get; set; }

  [DomName("track")]
  ITextTrack Track { get; }

  [DomName("startTime")]
  double StartTime { get; set; }

  [DomName("endTime")]
  double EndTime { get; set; }

  [DomName("pauseOnExit")]
  bool IsPausedOnExit { get; set; }

  [DomName("vertical")]
  string Vertical { get; set; }

  [DomName("snapToLines")]
  bool IsSnappedToLines { get; set; }

  [DomName("line")]
  int Line { get; set; }

  [DomName("position")]
  int Position { get; set; }

  [DomName("size")]
  int Size { get; set; }

  [DomName("align")]
  string Alignment { get; set; }

  [DomName("text")]
  string Text { get; set; }

  [DomName("getCueAsHTML")]
  IDocumentFragment AsHtml();

  [DomName("onenter")]
  DomEventHandler Entered { get; set; }

  [DomName("onexit")]
  DomEventHandler Exited { get; set; }
}
