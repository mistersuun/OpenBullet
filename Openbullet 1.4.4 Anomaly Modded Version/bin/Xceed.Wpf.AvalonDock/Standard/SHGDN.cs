// Decompiled with JetBrains decompiler
// Type: Standard.SHGDN
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;

#nullable disable
namespace Standard;

[Flags]
internal enum SHGDN
{
  SHGDN_NORMAL = 0,
  SHGDN_INFOLDER = 1,
  SHGDN_FOREDITING = 4096, // 0x00001000
  SHGDN_FORADDRESSBAR = 16384, // 0x00004000
  SHGDN_FORPARSING = 32768, // 0x00008000
}
