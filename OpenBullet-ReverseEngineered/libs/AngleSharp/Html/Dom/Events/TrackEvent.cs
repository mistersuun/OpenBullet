// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.Events.TrackEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom.Events;

[DomName("TrackEvent")]
public class TrackEvent : Event
{
  public TrackEvent()
  {
  }

  [DomConstructor]
  [DomInitDict(1, true)]
  public TrackEvent(string type, bool bubbles = false, bool cancelable = false, object track = null)
  {
    this.Init(type, bubbles, cancelable, track);
  }

  [DomName("track")]
  public object Track { get; private set; }

  [DomName("initTrackEvent")]
  public void Init(string type, bool bubbles, bool cancelable, object track)
  {
    this.Init(type, bubbles, cancelable);
    this.Track = track;
  }
}
