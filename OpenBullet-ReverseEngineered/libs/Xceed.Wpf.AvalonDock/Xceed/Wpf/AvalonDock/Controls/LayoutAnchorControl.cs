// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutAnchorControl
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class LayoutAnchorControl : Control, ILayoutControl
{
  private LayoutAnchorable _model;
  private DispatcherTimer _openUpTimer;
  private static readonly DependencyPropertyKey SidePropertyKey = DependencyProperty.RegisterReadOnly(nameof (Side), typeof (AnchorSide), typeof (LayoutAnchorControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) AnchorSide.Left));
  public static readonly DependencyProperty SideProperty = LayoutAnchorControl.SidePropertyKey.DependencyProperty;

  static LayoutAnchorControl()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutAnchorControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (LayoutAnchorControl)));
    UIElement.IsHitTestVisibleProperty.AddOwner(typeof (LayoutAnchorControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
  }

  internal LayoutAnchorControl(LayoutAnchorable model)
  {
    this._model = model;
    this._model.IsActiveChanged += new EventHandler(this._model_IsActiveChanged);
    this._model.IsSelectedChanged += new EventHandler(this._model_IsSelectedChanged);
    this.SetSide(this._model.FindParent<LayoutAnchorSide>().Side);
  }

  public ILayoutElement Model => (ILayoutElement) this._model;

  public AnchorSide Side => (AnchorSide) this.GetValue(LayoutAnchorControl.SideProperty);

  protected void SetSide(AnchorSide value)
  {
    this.SetValue(LayoutAnchorControl.SidePropertyKey, (object) value);
  }

  private void _model_IsSelectedChanged(object sender, EventArgs e)
  {
    if (!this._model.IsAutoHidden)
    {
      this._model.IsSelectedChanged -= new EventHandler(this._model_IsSelectedChanged);
    }
    else
    {
      if (!this._model.IsSelected)
        return;
      this._model.Root.Manager.ShowAutoHideWindow(this);
      this._model.IsSelected = false;
    }
  }

  private void _model_IsActiveChanged(object sender, EventArgs e)
  {
    if (!this._model.IsAutoHidden)
    {
      this._model.IsActiveChanged -= new EventHandler(this._model_IsActiveChanged);
    }
    else
    {
      if (!this._model.IsActive)
        return;
      this._model.Root.Manager.ShowAutoHideWindow(this);
    }
  }

  private void _openUpTimer_Tick(object sender, EventArgs e)
  {
    this._openUpTimer.Tick -= new EventHandler(this._openUpTimer_Tick);
    this._openUpTimer.Stop();
    this._openUpTimer = (DispatcherTimer) null;
    this._model.Root.Manager.ShowAutoHideWindow(this);
  }

  protected override void OnMouseDown(MouseButtonEventArgs e)
  {
    base.OnMouseDown(e);
    if (e.Handled)
      return;
    this._model.Root.Manager.ShowAutoHideWindow(this);
    this._model.IsActive = true;
  }

  protected override void OnMouseEnter(MouseEventArgs e)
  {
    base.OnMouseEnter(e);
    if (e.Handled)
      return;
    this._openUpTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
    this._openUpTimer.Interval = TimeSpan.FromMilliseconds(400.0);
    this._openUpTimer.Tick += new EventHandler(this._openUpTimer_Tick);
    this._openUpTimer.Start();
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    if (this._openUpTimer != null)
    {
      this._openUpTimer.Tick -= new EventHandler(this._openUpTimer_Tick);
      this._openUpTimer.Stop();
      this._openUpTimer = (DispatcherTimer) null;
    }
    base.OnMouseLeave(e);
  }
}
