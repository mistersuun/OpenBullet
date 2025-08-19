// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.FocusChangeEventArgs
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal class FocusChangeEventArgs : EventArgs
{
  public FocusChangeEventArgs(IntPtr gotFocusWinHandle, IntPtr lostFocusWinHandle)
  {
    this.GotFocusWinHandle = gotFocusWinHandle;
    this.LostFocusWinHandle = lostFocusWinHandle;
  }

  public IntPtr GotFocusWinHandle { get; private set; }

  public IntPtr LostFocusWinHandle { get; private set; }
}
