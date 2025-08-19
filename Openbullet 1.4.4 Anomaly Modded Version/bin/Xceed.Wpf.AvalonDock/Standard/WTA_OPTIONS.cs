// Decompiled with JetBrains decompiler
// Type: Standard.WTA_OPTIONS
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[StructLayout(LayoutKind.Explicit)]
internal struct WTA_OPTIONS
{
  public const uint Size = 8;
  [FieldOffset(0)]
  public WTNCA dwFlags;
  [FieldOffset(4)]
  public WTNCA dwMask;
}
