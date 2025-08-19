// Decompiled with JetBrains decompiler
// Type: Standard.HCF
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;

#nullable disable
namespace Standard;

[Flags]
internal enum HCF
{
  HIGHCONTRASTON = 1,
  AVAILABLE = 2,
  HOTKEYACTIVE = 4,
  CONFIRMHOTKEY = 8,
  HOTKEYSOUND = 16, // 0x00000010
  INDICATOR = 32, // 0x00000020
  HOTKEYAVAILABLE = 64, // 0x00000040
}
