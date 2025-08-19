// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.AnchorablePaneTitle
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class AnchorablePaneTitle : Control
{
  private bool _isMouseDown;
  public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(nameof (Model), typeof (LayoutAnchorable), typeof (AnchorablePaneTitle), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(AnchorablePaneTitle._OnModelChanged)));
  private static readonly DependencyPropertyKey LayoutItemPropertyKey = DependencyProperty.RegisterReadOnly(nameof (LayoutItem), typeof (LayoutItem), typeof (AnchorablePaneTitle), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty LayoutItemProperty = AnchorablePaneTitle.LayoutItemPropertyKey.DependencyProperty;

  static AnchorablePaneTitle()
  {
    UIElement.IsHitTestVisibleProperty.OverrideMetadata(typeof (AnchorablePaneTitle), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
    UIElement.FocusableProperty.OverrideMetadata(typeof (AnchorablePaneTitle), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (AnchorablePaneTitle), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (AnchorablePaneTitle)));
  }

  public LayoutAnchorable Model
  {
    get => (LayoutAnchorable) this.GetValue(AnchorablePaneTitle.ModelProperty);
    set => this.SetValue(AnchorablePaneTitle.ModelProperty, (object) value);
  }

  private static void _OnModelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    ((AnchorablePaneTitle) sender).OnModelChanged(e);
  }

  protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.Model != null)
      this.SetLayoutItem(this.Model.Root.Manager.GetLayoutItemFromModel((LayoutContent) this.Model));
    else
      this.SetLayoutItem((LayoutItem) null);
  }

  public LayoutItem LayoutItem
  {
    get => (LayoutItem) this.GetValue(AnchorablePaneTitle.LayoutItemProperty);
  }

  protected void SetLayoutItem(LayoutItem value)
  {
    this.SetValue(AnchorablePaneTitle.LayoutItemPropertyKey, (object) value);
  }

  protected override void OnMouseMove(MouseEventArgs e)
  {
    if (e.LeftButton != MouseButtonState.Pressed)
      this._isMouseDown = false;
    base.OnMouseMove(e);
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    base.OnMouseLeave(e);
    if (this._isMouseDown && e.LeftButton == MouseButtonState.Pressed)
    {
      LayoutAnchorablePaneControl visualAncestor = this.FindVisualAncestor<LayoutAnchorablePaneControl>();
      if (visualAncestor != null)
      {
        LayoutAnchorablePane model = visualAncestor.Model as LayoutAnchorablePane;
        model.Root.Manager.StartDraggingFloatingWindowForPane(model);
      }
      else
      {
        LayoutAnchorable model = this.Model;
        model?.Root?.Manager?.StartDraggingFloatingWindowForContent((LayoutContent) model);
      }
    }
    this._isMouseDown = false;
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    base.OnMouseLeftButtonDown(e);
    if (e.Handled)
      return;
    bool flag = false;
    LayoutAnchorableFloatingWindow parentFloatingWindow = this.Model.FindParent<LayoutAnchorableFloatingWindow>();
    if (parentFloatingWindow != null)
      flag = parentFloatingWindow.Descendents().OfType<LayoutAnchorablePane>().Count<LayoutAnchorablePane>() == 1;
    if (flag)
      this.Model.Root.Manager.FloatingWindows.Single<LayoutFloatingWindowControl>((Func<LayoutFloatingWindowControl, bool>) (fwc => fwc.Model == parentFloatingWindow)).AttachDrag(false);
    else
      this._isMouseDown = true;
  }

  protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    this._isMouseDown = false;
    base.OnMouseLeftButtonUp(e);
    if (this.Model == null)
      return;
    this.Model.IsActive = true;
  }

  private void OnHide() => this.Model.Hide();

  private void OnToggleAutoHide() => this.Model.ToggleAutoHide();
}
