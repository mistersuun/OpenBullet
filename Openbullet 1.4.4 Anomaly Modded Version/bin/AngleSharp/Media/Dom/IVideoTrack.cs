// Decompiled with JetBrains decompiler
// Type: AngleSharp.Media.Dom.IVideoTrack
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Media.Dom;

[DomName("VideoTrack")]
public interface IVideoTrack
{
  [DomName("id")]
  string Id { get; }

  [DomName("kind")]
  string Kind { get; }

  [DomName("label")]
  string Label { get; }

  [DomName("language")]
  string Language { get; }

  [DomName("selected")]
  bool IsSelected { get; set; }
}
