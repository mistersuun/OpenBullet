// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.DragService
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal class DragService
{
  private DockingManager _manager;
  private LayoutFloatingWindowControl _floatingWindow;
  private List<IOverlayWindowHost> _overlayWindowHosts = new List<IOverlayWindowHost>();
  private IOverlayWindowHost _currentHost;
  private IOverlayWindow _currentWindow;
  private List<IDropArea> _currentWindowAreas = new List<IDropArea>();
  private IDropTarget _currentDropTarget;

  public DragService(LayoutFloatingWindowControl floatingWindow)
  {
    this._floatingWindow = floatingWindow;
    this._manager = floatingWindow.Model.Root.Manager;
    this.GetOverlayWindowHosts();
  }

  public void UpdateMouseLocation(Point dragPosition)
  {
    ILayoutElement model = this._floatingWindow.Model;
    IOverlayWindowHost overlayWindowHost = this._overlayWindowHosts.FirstOrDefault<IOverlayWindowHost>((Func<IOverlayWindowHost, bool>) (oh => oh.HitTest(dragPosition)));
    if (this._currentHost != null || this._currentHost != overlayWindowHost)
    {
      if (this._currentHost != null && !this._currentHost.HitTest(dragPosition) || this._currentHost != overlayWindowHost)
      {
        if (this._currentDropTarget != null)
          this._currentWindow.DragLeave(this._currentDropTarget);
        this._currentDropTarget = (IDropTarget) null;
        this._currentWindowAreas.ForEach((Action<IDropArea>) (a => this._currentWindow.DragLeave(a)));
        this._currentWindowAreas.Clear();
        if (this._currentWindow != null)
          this._currentWindow.DragLeave(this._floatingWindow);
        if (this._currentHost != null)
          this._currentHost.HideOverlayWindow();
        this._currentHost = (IOverlayWindowHost) null;
      }
      if (this._currentHost != overlayWindowHost)
      {
        this._currentHost = overlayWindowHost;
        this._currentWindow = this._currentHost.ShowOverlayWindow(this._floatingWindow);
        this._currentWindow.DragEnter(this._floatingWindow);
      }
    }
    if (this._currentHost == null)
      return;
    if (this._currentDropTarget != null && !this._currentDropTarget.HitTest(dragPosition))
    {
      this._currentWindow.DragLeave(this._currentDropTarget);
      this._currentDropTarget = (IDropTarget) null;
    }
    List<IDropArea> areasToRemove = new List<IDropArea>();
    this._currentWindowAreas.ForEach((Action<IDropArea>) (a =>
    {
      if (a.DetectionRect.Contains(dragPosition))
        return;
      this._currentWindow.DragLeave(a);
      areasToRemove.Add(a);
    }));
    areasToRemove.ForEach((Action<IDropArea>) (a => this._currentWindowAreas.Remove(a)));
    List<IDropArea> list = this._currentHost.GetDropAreas(this._floatingWindow).Where<IDropArea>((Func<IDropArea, bool>) (cw => !this._currentWindowAreas.Contains(cw) && cw.DetectionRect.Contains(dragPosition))).ToList<IDropArea>();
    this._currentWindowAreas.AddRange((IEnumerable<IDropArea>) list);
    list.ForEach((Action<IDropArea>) (a => this._currentWindow.DragEnter(a)));
    if (this._currentDropTarget != null)
      return;
    this._currentWindowAreas.ForEach((Action<IDropArea>) (wa =>
    {
      if (this._currentDropTarget != null)
        return;
      this._currentDropTarget = this._currentWindow.GetTargets().FirstOrDefault<IDropTarget>((Func<IDropTarget, bool>) (dt => dt.HitTest(dragPosition)));
      if (this._currentDropTarget == null)
        return;
      this._currentWindow.DragEnter(this._currentDropTarget);
    }));
  }

  public void Drop(Point dropLocation, out bool dropHandled)
  {
    dropHandled = false;
    this.UpdateMouseLocation(dropLocation);
    ILayoutRoot root = (this._floatingWindow.Model as LayoutFloatingWindow).Root;
    if (this._currentHost != null)
      this._currentHost.HideOverlayWindow();
    if (this._currentDropTarget != null)
    {
      this._currentWindow.DragDrop(this._currentDropTarget);
      root.CollectGarbage();
      dropHandled = true;
    }
    this._currentWindowAreas.ForEach((Action<IDropArea>) (a => this._currentWindow.DragLeave(a)));
    if (this._currentDropTarget != null)
      this._currentWindow.DragLeave(this._currentDropTarget);
    if (this._currentWindow != null)
      this._currentWindow.DragLeave(this._floatingWindow);
    this._currentWindow = (IOverlayWindow) null;
    this._currentHost = (IOverlayWindowHost) null;
  }

  internal void Abort()
  {
    ILayoutElement model = this._floatingWindow.Model;
    this._currentWindowAreas.ForEach((Action<IDropArea>) (a => this._currentWindow.DragLeave(a)));
    if (this._currentDropTarget != null)
      this._currentWindow.DragLeave(this._currentDropTarget);
    if (this._currentWindow != null)
      this._currentWindow.DragLeave(this._floatingWindow);
    this._currentWindow = (IOverlayWindow) null;
    if (this._currentHost != null)
      this._currentHost.HideOverlayWindow();
    this._currentHost = (IOverlayWindowHost) null;
  }

  private void GetOverlayWindowHosts()
  {
    this._overlayWindowHosts.AddRange((IEnumerable<IOverlayWindowHost>) this._manager.GetFloatingWindowsByZOrder().OfType<LayoutAnchorableFloatingWindowControl>().Where<LayoutAnchorableFloatingWindowControl>((Func<LayoutAnchorableFloatingWindowControl, bool>) (fw => fw != this._floatingWindow && fw.IsVisible)));
    this._overlayWindowHosts.Add((IOverlayWindowHost) this._manager);
  }
}
