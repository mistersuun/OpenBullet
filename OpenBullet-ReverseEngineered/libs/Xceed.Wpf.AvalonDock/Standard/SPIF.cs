// Decompiled with JetBrains decompiler
// Type: Standard.SPIF
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;

#nullable disable
namespace Standard;

[Flags]
internal enum SPIF
{
  None = 0,
  UPDATEINIFILE = 1,
  SENDCHANGE = 2,
  SENDWININICHANGE = SENDCHANGE, // 0x00000002
}
