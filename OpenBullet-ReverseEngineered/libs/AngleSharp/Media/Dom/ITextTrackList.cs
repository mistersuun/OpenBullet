// Decompiled with JetBrains decompiler
// Type: AngleSharp.Media.Dom.ITextTrackList
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Media.Dom;

[DomName("TextTrackList")]
public interface ITextTrackList : IEventTarget, IEnumerable<ITextTrack>, IEnumerable
{
  [DomName("length")]
  int Length { get; }

  [DomAccessor(Accessors.Getter)]
  ITextTrack this[int index] { get; }

  [DomName("onaddtrack")]
  event DomEventHandler TrackAdded;

  [DomName("onremovetrack")]
  event DomEventHandler TrackRemoved;
}
