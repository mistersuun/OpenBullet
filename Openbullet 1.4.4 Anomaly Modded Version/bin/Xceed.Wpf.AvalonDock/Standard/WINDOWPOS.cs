// Decompiled with JetBrains decompiler
// Type: Standard.WINDOWPOS
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;

#nullable disable
namespace Standard;

internal struct WINDOWPOS
{
  public IntPtr hwnd;
  public IntPtr hwndInsertAfter;
  public int x;
  public int y;
  public int cx;
  public int cy;
  public int flags;
}
