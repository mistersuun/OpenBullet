// Decompiled with JetBrains decompiler
// Type: Standard.GPS
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

#nullable disable
namespace Standard;

internal enum GPS
{
  DEFAULT = 0,
  HANDLERPROPERTIESONLY = 1,
  READWRITE = 2,
  TEMPORARY = 4,
  FASTPROPERTIESONLY = 8,
  OPENSLOWITEM = 16, // 0x00000010
  DELAYCREATION = 32, // 0x00000020
  BESTEFFORT = 64, // 0x00000040
  NO_OPLOCK = 128, // 0x00000080
  MASK_VALID = 255, // 0x000000FF
}
