// Decompiled with JetBrains decompiler
// Type: Standard.ErrorModes
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;

#nullable disable
namespace Standard;

[Flags]
internal enum ErrorModes
{
  Default = 0,
  FailCriticalErrors = 1,
  NoGpFaultErrorBox = 2,
  NoAlignmentFaultExcept = 4,
  NoOpenFileErrorBox = 32768, // 0x00008000
}
