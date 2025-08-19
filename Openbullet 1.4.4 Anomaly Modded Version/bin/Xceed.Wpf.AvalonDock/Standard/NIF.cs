// Decompiled with JetBrains decompiler
// Type: Standard.NIF
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;

#nullable disable
namespace Standard;

[Flags]
internal enum NIF : uint
{
  MESSAGE = 1,
  ICON = 2,
  TIP = 4,
  STATE = 8,
  INFO = 16, // 0x00000010
  GUID = 32, // 0x00000020
  REALTIME = 64, // 0x00000040
  SHOWTIP = 128, // 0x00000080
  XP_MASK = GUID | INFO | STATE | ICON | MESSAGE, // 0x0000003B
  VISTA_MASK = XP_MASK | SHOWTIP | REALTIME, // 0x000000FB
}
