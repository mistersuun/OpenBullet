// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.LayoutAnchorGroupControl
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

public class LayoutAnchorGroupControl : Control, ILayoutControl
{
  private ObservableCollection<LayoutAnchorControl> _childViews = new ObservableCollection<LayoutAnchorControl>();
  private LayoutAnchorGroup _model;

  static LayoutAnchorGroupControl()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutAnchorGroupControl), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (LayoutAnchorGroupControl)));
  }

  internal LayoutAnchorGroupControl(LayoutAnchorGroup model)
  {
    this._model = model;
    this.CreateChildrenViews();
    this._model.Children.CollectionChanged += (NotifyCollectionChangedEventHandler) ((s, e) => this.OnModelChildrenCollectionChanged(e));
  }

  public ObservableCollection<LayoutAnchorControl> Children => this._childViews;

  public ILayoutElement Model => (ILayoutElement) this._model;

  private void CreateChildrenViews()
  {
    DockingManager manager = this._model.Root.Manager;
    foreach (LayoutAnchorable child in (Collection<LayoutAnchorable>) this._model.Children)
    {
      ObservableCollection<LayoutAnchorControl> childViews = this._childViews;
      LayoutAnchorControl layoutAnchorControl = new LayoutAnchorControl(child);
      layoutAnchorControl.Template = manager.AnchorTemplate;
      childViews.Add(layoutAnchorControl);
    }
  }

  private void OnModelChildrenCollectionChanged(NotifyCollectionChangedEventArgs e)
  {
    if ((e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace) && e.OldItems != null)
    {
      foreach (object oldItem in (IEnumerable) e.OldItems)
      {
        object childModel = oldItem;
        this._childViews.Remove(this._childViews.First<LayoutAnchorControl>((Func<LayoutAnchorControl, bool>) (cv => cv.Model == childModel)));
      }
    }
    if (e.Action == NotifyCollectionChangedAction.Reset)
      this._childViews.Clear();
    if (e.Action != NotifyCollectionChangedAction.Add && e.Action != NotifyCollectionChangedAction.Replace || e.NewItems == null)
      return;
    DockingManager manager = this._model.Root.Manager;
    int newStartingIndex = e.NewStartingIndex;
    foreach (LayoutAnchorable newItem in (IEnumerable) e.NewItems)
    {
      ObservableCollection<LayoutAnchorControl> childViews = this._childViews;
      int index = newStartingIndex++;
      LayoutAnchorControl layoutAnchorControl = new LayoutAnchorControl(newItem);
      layoutAnchorControl.Template = manager.AnchorTemplate;
      childViews.Insert(index, layoutAnchorControl);
    }
  }
}
