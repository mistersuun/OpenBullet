// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.ReentrantFlag
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal class ReentrantFlag
{
  private bool _flag;

  public bool CanEnter => !this._flag;

  public ReentrantFlag._ReentrantFlagHandler Enter()
  {
    if (this._flag)
      throw new InvalidOperationException();
    return new ReentrantFlag._ReentrantFlagHandler(this);
  }

  public class _ReentrantFlagHandler : IDisposable
  {
    private ReentrantFlag _owner;

    public _ReentrantFlagHandler(ReentrantFlag owner)
    {
      this._owner = owner;
      this._owner._flag = true;
    }

    public void Dispose() => this._owner._flag = false;
  }
}
