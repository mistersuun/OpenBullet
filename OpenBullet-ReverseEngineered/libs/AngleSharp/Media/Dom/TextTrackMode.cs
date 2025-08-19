// Decompiled with JetBrains decompiler
// Type: AngleSharp.Media.Dom.TextTrackMode
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Media.Dom;

[DomName("TextTrackMode")]
public enum TextTrackMode : byte
{
  [DomName("disabled")] Disabled,
  [DomName("hidden")] Hidden,
  [DomName("showing")] Showing,
}
