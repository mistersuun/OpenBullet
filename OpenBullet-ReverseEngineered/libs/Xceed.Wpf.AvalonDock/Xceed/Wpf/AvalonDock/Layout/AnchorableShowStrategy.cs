// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.AnchorableShowStrategy
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

[Flags]
public enum AnchorableShowStrategy : byte
{
  Most = 1,
  Left = 2,
  Right = 4,
  Top = 16, // 0x10
  Bottom = 32, // 0x20
}
