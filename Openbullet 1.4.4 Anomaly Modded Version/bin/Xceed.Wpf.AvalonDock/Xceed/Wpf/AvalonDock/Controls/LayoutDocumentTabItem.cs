// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutDocumentTabItem
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class LayoutDocumentTabItem : Control
{
  private List<Rect> _otherTabsScreenArea;
  private List<TabItem> _otherTabs;
  private Rect _parentDocumentTabPanelScreenArea;
  private DocumentPaneTabPanel _parentDocumentTabPanel;
  private bool _isMouseDown;
  private Point _mouseDownPoint;
  public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(nameof (Model), typeof (LayoutContent), typeof (LayoutDocumentTabItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(LayoutDocumentTabItem.OnModelChanged)));
  private static readonly DependencyPropertyKey LayoutItemPropertyKey = DependencyProperty.RegisterReadOnly(nameof (LayoutItem), typeof (LayoutItem), typeof (LayoutDocumentTabItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) null));
  public static readonly DependencyProperty LayoutItemProperty = LayoutDocumentTabItem.LayoutItemPropertyKey.DependencyProperty;

  static LayoutDocumentTabItem()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutDocumentTabItem), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (LayoutDocumentTabItem)));
  }

  public LayoutContent Model
  {
    get => (LayoutContent) this.GetValue(LayoutDocumentTabItem.ModelProperty);
    set => this.SetValue(LayoutDocumentTabItem.ModelProperty, (object) value);
  }

  private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((LayoutDocumentTabItem) d).OnModelChanged(e);
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
    get => (LayoutItem) this.GetValue(LayoutDocumentTabItem.LayoutItemProperty);
  }

  protected void SetLayoutItem(LayoutItem value)
  {
    this.SetValue(LayoutDocumentTabItem.LayoutItemPropertyKey, (object) value);
  }

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    base.OnMouseLeftButtonDown(e);
    this.Model.IsActive = true;
    if (this.Model is LayoutDocument model && !model.CanMove || e.ClickCount != 1)
      return;
    this._mouseDownPoint = e.GetPosition((IInputElement) this);
    this._isMouseDown = true;
  }

  protected override void OnMouseMove(MouseEventArgs e)
  {
    base.OnMouseMove(e);
    if (this._isMouseDown)
    {
      Point position = e.GetPosition((IInputElement) this);
      if (Math.Abs(position.X - this._mouseDownPoint.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - this._mouseDownPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
      {
        this.UpdateDragDetails();
        this.CaptureMouse();
        this._isMouseDown = false;
      }
    }
    if (!this.IsMouseCaptured)
      return;
    Point mousePosInScreenCoord = this.PointToScreenDPI(e.GetPosition((IInputElement) this));
    if (!this._parentDocumentTabPanelScreenArea.Contains(mousePosInScreenCoord))
    {
      this.StartDraggingFloatingWindowForContent();
    }
    else
    {
      int index = this._otherTabsScreenArea.FindIndex((Predicate<Rect>) (r => r.Contains(mousePosInScreenCoord)));
      if (index < 0)
        return;
      LayoutContent content = this._otherTabs[index].Content as LayoutContent;
      ILayoutContainer parent1 = this.Model.Parent;
      ILayoutPane parent2 = this.Model.Parent as ILayoutPane;
      if (parent2 is LayoutDocumentPane && !((LayoutPositionableGroup<LayoutContent>) parent2).CanRepositionItems || parent2.Parent != null && parent2.Parent is LayoutDocumentPaneGroup && !((LayoutPositionableGroup<ILayoutDocumentPane>) parent2.Parent).CanRepositionItems)
        return;
      List<ILayoutElement> list = parent1.Children.ToList<ILayoutElement>();
      parent2.MoveChild(list.IndexOf((ILayoutElement) this.Model), list.IndexOf((ILayoutElement) content));
      this.Model.IsActive = true;
      this._parentDocumentTabPanel.UpdateLayout();
      this.UpdateDragDetails();
    }
  }

  protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    if (this.IsMouseCaptured)
      this.ReleaseMouseCapture();
    this._isMouseDown = false;
    base.OnMouseLeftButtonUp(e);
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    base.OnMouseLeave(e);
    this._isMouseDown = false;
  }

  protected override void OnMouseEnter(MouseEventArgs e)
  {
    base.OnMouseEnter(e);
    this._isMouseDown = false;
  }

  protected override void OnMouseDown(MouseButtonEventArgs e)
  {
    if (e.ChangedButton == MouseButton.Middle && this.LayoutItem.CloseCommand.CanExecute((object) null))
      this.LayoutItem.CloseCommand.Execute((object) null);
    base.OnMouseDown(e);
  }

  private void UpdateDragDetails()
  {
    this._parentDocumentTabPanel = this.FindLogicalAncestor<DocumentPaneTabPanel>();
    this._parentDocumentTabPanelScreenArea = this._parentDocumentTabPanel.GetScreenArea();
    this._otherTabs = this._parentDocumentTabPanel.Children.Cast<TabItem>().Where<TabItem>((Func<TabItem, bool>) (ch => ch.Visibility != Visibility.Collapsed)).ToList<TabItem>();
    Rect currentTabScreenArea = this.FindLogicalAncestor<TabItem>().GetScreenArea();
    this._otherTabsScreenArea = this._otherTabs.Select<TabItem, Rect>((Func<TabItem, Rect>) (ti =>
    {
      Rect screenArea = ti.GetScreenArea();
      return new Rect(screenArea.Left, screenArea.Top, currentTabScreenArea.Width, screenArea.Height);
    })).ToList<Rect>();
  }

  private void StartDraggingFloatingWindowForContent()
  {
    this.ReleaseMouseCapture();
    if (this.Model is LayoutAnchorable)
      ((LayoutAnchorable) this.Model).ResetCanCloseInternal();
    this.Model.Root.Manager.StartDraggingFloatingWindowForContent(this.Model);
  }
}
