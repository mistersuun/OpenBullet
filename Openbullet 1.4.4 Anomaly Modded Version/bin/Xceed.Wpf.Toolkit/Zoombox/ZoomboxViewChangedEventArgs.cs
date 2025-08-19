// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Zoombox.ZoomboxViewChangedEventArgs
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using Xceed.Wpf.Toolkit.Core;

#nullable disable
namespace Xceed.Wpf.Toolkit.Zoombox;

public class ZoomboxViewChangedEventArgs : PropertyChangedEventArgs<ZoomboxView>
{
  private readonly int _newViewStackIndex = -1;
  private readonly int _oldViewStackIndex = -1;

  public ZoomboxViewChangedEventArgs(
    ZoomboxView oldView,
    ZoomboxView newView,
    int oldViewStackIndex,
    int newViewStackIndex)
    : base(Xceed.Wpf.Toolkit.Zoombox.Zoombox.CurrentViewChangedEvent, oldView, newView)
  {
    this._newViewStackIndex = newViewStackIndex;
    this._oldViewStackIndex = oldViewStackIndex;
  }

  public int NewViewStackIndex => this._newViewStackIndex;

  public int OldViewStackIndex => this._oldViewStackIndex;

  public bool IsNewViewFromStack => this._newViewStackIndex >= 0;

  public bool IsOldViewFromStack => this._oldViewStackIndex >= 0;

  protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
  {
    ((ZoomboxViewChangedEventHandler) genericHandler)(genericTarget, this);
  }
}
