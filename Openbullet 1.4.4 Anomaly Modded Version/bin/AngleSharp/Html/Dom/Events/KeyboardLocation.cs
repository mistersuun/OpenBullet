// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.Events.KeyboardLocation
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Html.Dom.Events;

[DomName("KeyboardEvent")]
public enum KeyboardLocation : byte
{
  [DomName("DOM_KEY_LOCATION_STANDARD")] Standard,
  [DomName("DOM_KEY_LOCATION_LEFT")] Left,
  [DomName("DOM_KEY_LOCATION_RIGHT")] Right,
  [DomName("DOM_KEY_LOCATION_NUMPAD")] NumPad,
}
