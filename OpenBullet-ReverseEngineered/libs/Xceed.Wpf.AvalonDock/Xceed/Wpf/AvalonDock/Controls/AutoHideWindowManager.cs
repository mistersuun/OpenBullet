// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.AutoHideWindowManager
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal class AutoHideWindowManager
{
  private DockingManager _manager;
  private WeakReference _currentAutohiddenAnchor;
  private DispatcherTimer _closeTimer;

  internal AutoHideWindowManager(DockingManager manager)
  {
    this._manager = manager;
    this.SetupCloseTimer();
  }

  public void ShowAutoHideWindow(LayoutAnchorControl anchor)
  {
    if (this._currentAutohiddenAnchor.GetValueOrDefault<LayoutAnchorControl>() == anchor)
      return;
    this.StopCloseTimer();
    this._currentAutohiddenAnchor = new WeakReference((object) anchor);
    this._manager.AutoHideWindow.Show(anchor);
    this.StartCloseTimer();
  }

  public void HideAutoWindow(LayoutAnchorControl anchor = null)
  {
    if (anchor != null && anchor != this._currentAutohiddenAnchor.GetValueOrDefault<LayoutAnchorControl>())
      return;
    this.StopCloseTimer();
  }

  private void SetupCloseTimer()
  {
    this._closeTimer = new DispatcherTimer(DispatcherPriority.Background);
    this._closeTimer.Interval = TimeSpan.FromMilliseconds(1500.0);
    this._closeTimer.Tick += (EventHandler) ((s, e) =>
    {
      if (this._manager.AutoHideWindow.IsWin32MouseOver || ((LayoutContent) this._manager.AutoHideWindow.Model).IsActive || this._manager.AutoHideWindow.IsResizing)
        return;
      this.StopCloseTimer();
    });
  }

  private void StartCloseTimer() => this._closeTimer.Start();

  private void StopCloseTimer()
  {
    this._closeTimer.Stop();
    this._manager.AutoHideWindow.Hide();
    this._currentAutohiddenAnchor = (WeakReference) null;
  }
}
