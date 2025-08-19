// Decompiled with JetBrains decompiler
// Type: AngleSharp.Media.Dom.IVideoTrackList
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Media.Dom;

[DomName("VideoTrackList")]
public interface IVideoTrackList : IEventTarget, IEnumerable<IVideoTrack>, IEnumerable
{
  [DomName("length")]
  int Length { get; }

  [DomName("selectedIndex")]
  int SelectedIndex { get; }

  [DomAccessor(Accessors.Getter)]
  IVideoTrack this[int index] { get; }

  [DomName("getTrackById")]
  IVideoTrack GetTrackById(string id);

  [DomName("onchange")]
  event DomEventHandler Changed;

  [DomName("onaddtrack")]
  event DomEventHandler TrackAdded;

  [DomName("onremovetrack")]
  event DomEventHandler TrackRemoved;
}
