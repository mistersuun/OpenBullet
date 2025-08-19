// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutAnchorSideControl
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

public class LayoutAnchorSideControl : Control, ILayoutControl
{
  private LayoutAnchorSide _model;
  private ObservableCollection<LayoutAnchorGroupControl> _childViews = new ObservableCollection<LayoutAnchorGroupControl>();
  private static readonly DependencyPropertyKey IsLeftSidePropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsLeftSide), typeof (bool), typeof (LayoutAnchorSideControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty IsLeftSideProperty = LayoutAnchorSideControl.IsLeftSidePropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey IsTopSidePropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsTopSide), typeof (bool), typeof (LayoutAnchorSideControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty IsTopSideProperty = LayoutAnchorSideControl.IsTopSidePropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey IsRightSidePropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsRightSide), typeof (bool), typeof (LayoutAnchorSideControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty IsRightSideProperty = LayoutAnchorSideControl.IsRightSidePropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey IsBottomSidePropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsBottomSide), typeof (bool), typeof (LayoutAnchorSideControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
  public static readonly DependencyProperty IsBottomSideProperty = LayoutAnchorSideControl.IsBottomSidePropertyKey.DependencyProperty;

  static LayoutAnchorSideControl()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutAnchorSideControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (LayoutAnchorSideControl)));
  }

  internal LayoutAnchorSideControl(LayoutAnchorSide model)
  {
    this._model = model != null ? model : throw new ArgumentNullException(nameof (model));
    this.CreateChildrenViews();
    this._model.Children.CollectionChanged += (NotifyCollectionChangedEventHandler) ((s, e) => this.OnModelChildrenCollectionChanged(e));
    this.UpdateSide();
  }

  public ILayoutElement Model => (ILayoutElement) this._model;

  public ObservableCollection<LayoutAnchorGroupControl> Children => this._childViews;

  public bool IsLeftSide => (bool) this.GetValue(LayoutAnchorSideControl.IsLeftSideProperty);

  protected void SetIsLeftSide(bool value)
  {
    this.SetValue(LayoutAnchorSideControl.IsLeftSidePropertyKey, (object) value);
  }

  public bool IsTopSide => (bool) this.GetValue(LayoutAnchorSideControl.IsTopSideProperty);

  protected void SetIsTopSide(bool value)
  {
    this.SetValue(LayoutAnchorSideControl.IsTopSidePropertyKey, (object) value);
  }

  public bool IsRightSide => (bool) this.GetValue(LayoutAnchorSideControl.IsRightSideProperty);

  protected void SetIsRightSide(bool value)
  {
    this.SetValue(LayoutAnchorSideControl.IsRightSidePropertyKey, (object) value);
  }

  public bool IsBottomSide => (bool) this.GetValue(LayoutAnchorSideControl.IsBottomSideProperty);

  protected void SetIsBottomSide(bool value)
  {
    this.SetValue(LayoutAnchorSideControl.IsBottomSidePropertyKey, (object) value);
  }

  private void CreateChildrenViews()
  {
    DockingManager manager = this._model.Root.Manager;
    foreach (LayoutAnchorGroup child in (Collection<LayoutAnchorGroup>) this._model.Children)
      this._childViews.Add(manager.CreateUIElementForModel((ILayoutElement) child) as LayoutAnchorGroupControl);
  }

  private void OnModelChildrenCollectionChanged(NotifyCollectionChangedEventArgs e)
  {
    if (e.OldItems != null && (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace))
    {
      foreach (object oldItem in (IEnumerable) e.OldItems)
      {
        object childModel = oldItem;
        this._childViews.Remove(this._childViews.First<LayoutAnchorGroupControl>((Func<LayoutAnchorGroupControl, bool>) (cv => cv.Model == childModel)));
      }
    }
    if (e.Action == NotifyCollectionChangedAction.Reset)
      this._childViews.Clear();
    if (e.NewItems == null || e.Action != NotifyCollectionChangedAction.Add && e.Action != NotifyCollectionChangedAction.Replace)
      return;
    DockingManager manager = this._model.Root.Manager;
    int newStartingIndex = e.NewStartingIndex;
    foreach (LayoutAnchorGroup newItem in (IEnumerable) e.NewItems)
      this._childViews.Insert(newStartingIndex++, manager.CreateUIElementForModel((ILayoutElement) newItem) as LayoutAnchorGroupControl);
  }

  private void UpdateSide()
  {
    switch (this._model.Side)
    {
      case AnchorSide.Left:
        this.SetIsLeftSide(true);
        break;
      case AnchorSide.Top:
        this.SetIsTopSide(true);
        break;
      case AnchorSide.Right:
        this.SetIsRightSide(true);
        break;
      case AnchorSide.Bottom:
        this.SetIsBottomSide(true);
        break;
    }
  }
}
