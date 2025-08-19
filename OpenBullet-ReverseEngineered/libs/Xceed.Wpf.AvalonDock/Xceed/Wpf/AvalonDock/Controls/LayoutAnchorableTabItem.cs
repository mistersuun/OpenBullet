// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutAnchorableTabItem
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class LayoutAnchorableTabItem : Control
{
  private bool _isMouseDown;
  private static LayoutAnchorableTabItem _draggingItem = (LayoutAnchorableTabItem) null;
  public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(nameof (Model), typeof (LayoutContent), typeof (LayoutAnchorableTabItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutAnchorableTabItem.OnModelChanged)));
  private static readonly DependencyPropertyKey LayoutItemPropertyKey = DependencyProperty.RegisterReadOnly(nameof (LayoutItem), typeof (LayoutItem), typeof (LayoutAnchorableTabItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty LayoutItemProperty = LayoutAnchorableTabItem.LayoutItemPropertyKey.DependencyProperty;

  static LayoutAnchorableTabItem()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutAnchorableTabItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (LayoutAnchorableTabItem)));
  }

  public LayoutContent Model
  {
    get => (LayoutContent) this.GetValue(LayoutAnchorableTabItem.ModelProperty);
    set => this.SetValue(LayoutAnchorableTabItem.ModelProperty, (object) value);
  }

  private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutAnchorableTabItem) d).OnModelChanged(e);
  }

  protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this.Model != null)
      this.SetLayoutItem(this.Model.Root.Manager.GetLayoutItemFromModel(this.Model));
    else
      this.SetLayoutItem((LayoutItem) null);
  }

  public LayoutItem LayoutItem
  {
    get => (LayoutItem) this.GetValue(LayoutAnchorableTabItem.LayoutItemProperty);
  }

  protected void SetLayoutItem(LayoutItem value)
  {
    this.SetValue(LayoutAnchorableTabItem.LayoutItemPropertyKey, (object) value);
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    base.OnMouseLeftButtonDown(e);
    this._isMouseDown = true;
    LayoutAnchorableTabItem._draggingItem = this;
  }

  protected override void OnMouseMove(MouseEventArgs e)
  {
    base.OnMouseMove(e);
    if (e.LeftButton == MouseButtonState.Pressed)
      return;
    this._isMouseDown = false;
    LayoutAnchorableTabItem._draggingItem = (LayoutAnchorableTabItem) null;
  }

  protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    this._isMouseDown = false;
    base.OnMouseLeftButtonUp(e);
    this.Model.IsActive = true;
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    base.OnMouseLeave(e);
    if (this._isMouseDown && e.LeftButton == MouseButtonState.Pressed)
      LayoutAnchorableTabItem._draggingItem = this;
    this._isMouseDown = false;
  }

  protected override void OnMouseEnter(MouseEventArgs e)
  {
    base.OnMouseEnter(e);
    if (LayoutAnchorableTabItem._draggingItem == null || LayoutAnchorableTabItem._draggingItem == this || e.LeftButton != MouseButtonState.Pressed)
      return;
    LayoutContent model = this.Model;
    ILayoutContainer parent1 = model.Parent;
    ILayoutPane parent2 = model.Parent as ILayoutPane;
    if (parent2 is LayoutAnchorablePane && !((LayoutPositionableGroup<LayoutAnchorable>) parent2).CanRepositionItems || parent2.Parent != null && parent2.Parent is LayoutAnchorablePaneGroup && !((LayoutPositionableGroup<ILayoutAnchorablePane>) parent2.Parent).CanRepositionItems)
      return;
    List<ILayoutElement> list = parent1.Children.ToList<ILayoutElement>();
    parent2.MoveChild(list.IndexOf((ILayoutElement) LayoutAnchorableTabItem._draggingItem.Model), list.IndexOf((ILayoutElement) model));
  }

  protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
  {
    base.OnPreviewGotKeyboardFocus(e);
  }

  internal static bool IsDraggingItem() => LayoutAnchorableTabItem._draggingItem != null;

  internal static LayoutAnchorableTabItem GetDraggingItem()
  {
    return LayoutAnchorableTabItem._draggingItem;
  }

  internal static void ResetDraggingItem()
  {
    LayoutAnchorableTabItem._draggingItem = (LayoutAnchorableTabItem) null;
  }
}
