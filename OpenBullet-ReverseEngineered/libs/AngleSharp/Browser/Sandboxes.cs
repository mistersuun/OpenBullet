// Decompiled with JetBrains decompiler
// Type: AngleSharp.Browser.Sandboxes
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using System;

#nullable disable
namespace AngleSharp.Browser;

[Flags]
public enum Sandboxes : ushort
{
  None = 0,
  Navigation = 1,
  AuxiliaryNavigation = 2,
  TopLevelNavigation = 4,
  Plugins = 8,
  Origin = 16, // 0x0010
  Forms = 32, // 0x0020
  PointerLock = 64, // 0x0040
  Scripts = 128, // 0x0080
  AutomaticFeatures = 256, // 0x0100
  Fullscreen = 512, // 0x0200
  DocumentDomain = 1024, // 0x0400
  Presentation = 2048, // 0x0800
}
