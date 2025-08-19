// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Panels.SwitchPanel
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Media.Animation;

#nullable disable
namespace Xceed.Wpf.Toolkit.Panels;

public class SwitchPanel : PanelBase, IScrollInfo
{
  private static readonly Vector ZeroVector = new Vector();
  public static readonly DependencyProperty AreLayoutSwitchesAnimatedProperty = DependencyProperty.Register(nameof (AreLayoutSwitchesAnimated), typeof (bool), typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) true));
  private static readonly DependencyPropertyKey ActiveLayoutPropertyKey = DependencyProperty.RegisterReadOnly(nameof (ActiveLayout), typeof (AnimationPanel), typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(SwitchPanel.OnActiveLayoutChanged)));
  public static readonly DependencyProperty ActiveLayoutProperty = SwitchPanel.ActiveLayoutPropertyKey.DependencyProperty;
  public static readonly DependencyProperty ActiveLayoutIndexProperty = DependencyProperty.Register(nameof (ActiveLayoutIndex), typeof (int), typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) -1, new PropertyChangedCallback(SwitchPanel.OnActiveLayoutIndexChanged), new CoerceValueCallback(SwitchPanel.CoerceActiveLayoutIndexValue)));
  private static readonly DependencyPropertyKey ActiveSwitchTemplatePropertyKey = DependencyProperty.RegisterReadOnly(nameof (ActiveSwitchTemplate), typeof (DataTemplate), typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(SwitchPanel.OnActiveSwitchTemplateChanged)));
  public static readonly DependencyProperty ActiveSwitchTemplateProperty = SwitchPanel.ActiveSwitchTemplatePropertyKey.DependencyProperty;
  public static readonly DependencyProperty DefaultAnimationRateProperty = AnimationPanel.DefaultAnimationRateProperty.AddOwner(typeof (SwitchPanel));
  public static readonly DependencyProperty DefaultAnimatorProperty = AnimationPanel.DefaultAnimatorProperty.AddOwner(typeof (SwitchPanel));
  public static readonly DependencyProperty EnterAnimationRateProperty = AnimationPanel.EnterAnimationRateProperty.AddOwner(typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) AnimationRate.Default));
  public static readonly DependencyProperty EnterAnimatorProperty = AnimationPanel.EnterAnimatorProperty.AddOwner(typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) IterativeAnimator.Default));
  public static readonly DependencyProperty ExitAnimationRateProperty = AnimationPanel.ExitAnimationRateProperty.AddOwner(typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) AnimationRate.Default));
  public static readonly DependencyProperty ExitAnimatorProperty = AnimationPanel.ExitAnimatorProperty.AddOwner(typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) IterativeAnimator.Default));
  public static readonly DependencyProperty LayoutAnimationRateProperty = AnimationPanel.LayoutAnimationRateProperty.AddOwner(typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) AnimationRate.Default));
  public static readonly DependencyProperty LayoutAnimatorProperty = AnimationPanel.LayoutAnimatorProperty.AddOwner(typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) IterativeAnimator.Default));
  private static readonly DependencyPropertyKey LayoutsPropertyKey = DependencyProperty.RegisterReadOnly(nameof (Layouts), typeof (ObservableCollection<AnimationPanel>), typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(SwitchPanel.OnLayoutsChanged)));
  public static readonly DependencyProperty LayoutsProperty = SwitchPanel.LayoutsPropertyKey.DependencyProperty;
  public static readonly DependencyProperty SwitchAnimationRateProperty = AnimationPanel.SwitchAnimationRateProperty.AddOwner(typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) AnimationRate.Default));
  public static readonly DependencyProperty SwitchAnimatorProperty = AnimationPanel.SwitchAnimatorProperty.AddOwner(typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) IterativeAnimator.Default));
  public static readonly DependencyProperty SwitchTemplateProperty = DependencyProperty.Register(nameof (SwitchTemplate), typeof (DataTemplate), typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(SwitchPanel.OnSwitchTemplateChanged)));
  public static readonly DependencyProperty TemplateAnimationRateProperty = AnimationPanel.TemplateAnimationRateProperty.AddOwner(typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) AnimationRate.Default));
  public static readonly DependencyProperty TemplateAnimatorProperty = AnimationPanel.TemplateAnimatorProperty.AddOwner(typeof (SwitchPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) IterativeAnimator.Default));
  private readonly List<UIElement> _exitingChildren = new List<UIElement>();
  public static readonly RoutedEvent ActiveLayoutChangedEvent = EventManager.RegisterRoutedEvent("ActiveLayoutChanged", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (SwitchPanel));
  public static readonly RoutedEvent SwitchAnimationBegunEvent = EventManager.RegisterRoutedEvent("SwitchAnimationBegun", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (SwitchPanel));
  public static readonly RoutedEvent SwitchAnimationCompletedEvent = EventManager.RegisterRoutedEvent("SwitchAnimationCompleted", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (SwitchPanel));
  internal AnimationPanel _currentLayoutPanel;
  private readonly AnimationPanel _defaultLayoutCanvas = (AnimationPanel) new WrapPanel();
  private readonly Collection<SwitchPresenter> _presenters = new Collection<SwitchPresenter>();
  private BitVector32 _cacheBits = new BitVector32(0);
  private bool _allowHorizontal;
  private bool _allowVertical;
  private Vector _computedOffset = new Vector(0.0, 0.0);
  private Size _extent = new Size(0.0, 0.0);
  private Vector _offset = new Vector(0.0, 0.0);
  private ScrollViewer _scrollOwner;
  private Size _viewport;

  public SwitchPanel()
  {
    this.SetLayouts(new ObservableCollection<AnimationPanel>());
    this.Loaded += new RoutedEventHandler(this.OnLoaded);
  }

  public bool AreLayoutSwitchesAnimated
  {
    get => (bool) this.GetValue(SwitchPanel.AreLayoutSwitchesAnimatedProperty);
    set => this.SetValue(SwitchPanel.AreLayoutSwitchesAnimatedProperty, (object) value);
  }

  public AnimationPanel ActiveLayout
  {
    get => (AnimationPanel) this.GetValue(SwitchPanel.ActiveLayoutProperty);
  }

  protected void SetActiveLayout(AnimationPanel value)
  {
    this.SetValue(SwitchPanel.ActiveLayoutPropertyKey, (object) value);
  }

  private static void OnActiveLayoutChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((SwitchPanel) d).OnActiveLayoutChanged(e);
  }

  protected virtual void OnActiveLayoutChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this._currentLayoutPanel != null)
      this._currentLayoutPanel.DeactivateLayout();
    this._currentLayoutPanel = e.NewValue as AnimationPanel;
    if (this._currentLayoutPanel != null)
    {
      if (this._currentLayoutPanel is IScrollInfo currentLayoutPanel && currentLayoutPanel.ScrollOwner != null)
        currentLayoutPanel.ScrollOwner.InvalidateScrollInfo();
      this._currentLayoutPanel.ActivateLayout();
    }
    this.RaiseActiveLayoutChangedEvent();
    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate) (() => this.UpdateSwitchTemplate()));
  }

  public int ActiveLayoutIndex
  {
    get => (int) this.GetValue(SwitchPanel.ActiveLayoutIndexProperty);
    set => this.SetValue(SwitchPanel.ActiveLayoutIndexProperty, (object) value);
  }

  private static void OnActiveLayoutIndexChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((SwitchPanel) d).OnActiveLayoutIndexChanged(e);
  }

  protected virtual void OnActiveLayoutIndexChanged(DependencyPropertyChangedEventArgs e)
  {
    this.SetActiveLayout(this.Layouts.Count == 0 ? (AnimationPanel) null : this.Layouts[this.ActiveLayoutIndex]);
  }

  private static object CoerceActiveLayoutIndexValue(DependencyObject d, object value)
  {
    int count = (d as SwitchPanel).Layouts.Count;
    int num = (int) value;
    if (num < 0 && count > 0)
      num = 0;
    else if (num >= count)
      num = count - 1;
    return (object) num;
  }

  public DataTemplate ActiveSwitchTemplate
  {
    get => (DataTemplate) this.GetValue(SwitchPanel.ActiveSwitchTemplateProperty);
  }

  protected void SetActiveSwitchTemplate(DataTemplate value)
  {
    this.SetValue(SwitchPanel.ActiveSwitchTemplatePropertyKey, (object) value);
  }

  private static void OnActiveSwitchTemplateChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((SwitchPanel) d).OnActiveSwitchTemplateChanged(e);
  }

  protected virtual void OnActiveSwitchTemplateChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this._presenters.Count <= 0)
      return;
    DataTemplate newValue = e.NewValue as DataTemplate;
    List<UIElement> uiElementList = new List<UIElement>(this.InternalChildren.Count);
    foreach (UIElement internalChild in this.InternalChildren)
    {
      if (internalChild != null)
        uiElementList.Add(internalChild);
    }
    foreach (SwitchPresenter presenter in this._presenters)
    {
      if (presenter._switchRoot != null && uiElementList.Contains(presenter._switchRoot))
        presenter.SwapTheTemplate(newValue, this.AreLayoutSwitchesAnimated);
    }
  }

  public AnimationRate DefaultAnimationRate
  {
    get => (AnimationRate) this.GetValue(SwitchPanel.DefaultAnimationRateProperty);
    set => this.SetValue(SwitchPanel.DefaultAnimationRateProperty, (object) value);
  }

  public IterativeAnimator DefaultAnimator
  {
    get => (IterativeAnimator) this.GetValue(SwitchPanel.DefaultAnimatorProperty);
    set => this.SetValue(SwitchPanel.DefaultAnimatorProperty, (object) value);
  }

  public AnimationRate EnterAnimationRate
  {
    get => (AnimationRate) this.GetValue(SwitchPanel.EnterAnimationRateProperty);
    set => this.SetValue(SwitchPanel.EnterAnimationRateProperty, (object) value);
  }

  public IterativeAnimator EnterAnimator
  {
    get => (IterativeAnimator) this.GetValue(SwitchPanel.EnterAnimatorProperty);
    set => this.SetValue(SwitchPanel.EnterAnimatorProperty, (object) value);
  }

  public AnimationRate ExitAnimationRate
  {
    get => (AnimationRate) this.GetValue(SwitchPanel.ExitAnimationRateProperty);
    set => this.SetValue(SwitchPanel.ExitAnimationRateProperty, (object) value);
  }

  public IterativeAnimator ExitAnimator
  {
    get => (IterativeAnimator) this.GetValue(SwitchPanel.ExitAnimatorProperty);
    set => this.SetValue(SwitchPanel.ExitAnimatorProperty, (object) value);
  }

  public AnimationRate LayoutAnimationRate
  {
    get => (AnimationRate) this.GetValue(SwitchPanel.LayoutAnimationRateProperty);
    set => this.SetValue(SwitchPanel.LayoutAnimationRateProperty, (object) value);
  }

  public IterativeAnimator LayoutAnimator
  {
    get => (IterativeAnimator) this.GetValue(SwitchPanel.LayoutAnimatorProperty);
    set => this.SetValue(SwitchPanel.LayoutAnimatorProperty, (object) value);
  }

  public ObservableCollection<AnimationPanel> Layouts
  {
    get => (ObservableCollection<AnimationPanel>) this.GetValue(SwitchPanel.LayoutsProperty);
  }

  protected void SetLayouts(ObservableCollection<AnimationPanel> value)
  {
    this.SetValue(SwitchPanel.LayoutsPropertyKey, (object) value);
  }

  private static void OnLayoutsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    ((SwitchPanel) d).OnLayoutsChanged(e);
  }

  protected virtual void OnLayoutsChanged(DependencyPropertyChangedEventArgs e)
  {
    if (e.NewValue != null)
      (e.NewValue as ObservableCollection<AnimationPanel>).CollectionChanged += new NotifyCollectionChangedEventHandler(this.LayoutsCollectionChanged);
    if (e.OldValue == null)
      return;
    (e.OldValue as ObservableCollection<AnimationPanel>).CollectionChanged -= new NotifyCollectionChangedEventHandler(this.LayoutsCollectionChanged);
  }

  public AnimationRate SwitchAnimationRate
  {
    get => (AnimationRate) this.GetValue(SwitchPanel.SwitchAnimationRateProperty);
    set => this.SetValue(SwitchPanel.SwitchAnimationRateProperty, (object) value);
  }

  public IterativeAnimator SwitchAnimator
  {
    get => (IterativeAnimator) this.GetValue(SwitchPanel.SwitchAnimatorProperty);
    set => this.SetValue(SwitchPanel.SwitchAnimatorProperty, (object) value);
  }

  public DataTemplate SwitchTemplate
  {
    get => (DataTemplate) this.GetValue(SwitchPanel.SwitchTemplateProperty);
    set => this.SetValue(SwitchPanel.SwitchTemplateProperty, (object) value);
  }

  private static void OnSwitchTemplateChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((SwitchPanel) d).OnSwitchTemplateChanged(e);
  }

  protected virtual void OnSwitchTemplateChanged(DependencyPropertyChangedEventArgs e)
  {
    this.UpdateSwitchTemplate();
  }

  public AnimationRate TemplateAnimationRate
  {
    get => (AnimationRate) this.GetValue(SwitchPanel.TemplateAnimationRateProperty);
    set => this.SetValue(SwitchPanel.TemplateAnimationRateProperty, (object) value);
  }

  public IterativeAnimator TemplateAnimator
  {
    get => (IterativeAnimator) this.GetValue(SwitchPanel.TemplateAnimatorProperty);
    set => this.SetValue(SwitchPanel.TemplateAnimatorProperty, (object) value);
  }

  protected override int VisualChildrenCount
  {
    get
    {
      return !this.HasLoaded || this._currentLayoutPanel == null ? base.VisualChildrenCount : this._currentLayoutPanel.VisualChildrenCountInternal;
    }
  }

  internal List<UIElement> ExitingChildren => this._exitingChildren;

  internal UIElementCollection ChildrenInternal => this.InternalChildren;

  internal bool HasLoaded
  {
    get => this._cacheBits[1];
    set => this._cacheBits[1] = value;
  }

  private bool IsScrollingPhysically
  {
    get
    {
      bool scrollingPhysically = false;
      if (this._scrollOwner != null)
      {
        scrollingPhysically = true;
        if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
          scrollingPhysically = ((IScrollInfo) this.ActiveLayout).ScrollOwner == null;
      }
      return scrollingPhysically;
    }
  }

  public event RoutedEventHandler ActiveLayoutChanged
  {
    add => this.AddHandler(SwitchPanel.ActiveLayoutChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(SwitchPanel.ActiveLayoutChangedEvent, (Delegate) value);
  }

  protected RoutedEventArgs RaiseActiveLayoutChangedEvent()
  {
    return SwitchPanel.RaiseActiveLayoutChangedEvent((UIElement) this);
  }

  internal static RoutedEventArgs RaiseActiveLayoutChangedEvent(UIElement target)
  {
    if (target == null)
      return (RoutedEventArgs) null;
    RoutedEventArgs args = new RoutedEventArgs();
    args.RoutedEvent = SwitchPanel.ActiveLayoutChangedEvent;
    RoutedEventHelper.RaiseEvent((DependencyObject) target, args);
    return args;
  }

  public event RoutedEventHandler SwitchAnimationBegun
  {
    add => this.AddHandler(SwitchPanel.SwitchAnimationBegunEvent, (Delegate) value);
    remove => this.RemoveHandler(SwitchPanel.SwitchAnimationBegunEvent, (Delegate) value);
  }

  protected RoutedEventArgs RaiseSwitchAnimationBegunEvent()
  {
    return SwitchPanel.RaiseSwitchAnimationBegunEvent((UIElement) this);
  }

  private static RoutedEventArgs RaiseSwitchAnimationBegunEvent(UIElement target)
  {
    if (target == null)
      return (RoutedEventArgs) null;
    RoutedEventArgs args = new RoutedEventArgs();
    args.RoutedEvent = SwitchPanel.SwitchAnimationBegunEvent;
    RoutedEventHelper.RaiseEvent((DependencyObject) target, args);
    return args;
  }

  public event RoutedEventHandler SwitchAnimationCompleted
  {
    add => this.AddHandler(SwitchPanel.SwitchAnimationCompletedEvent, (Delegate) value);
    remove => this.RemoveHandler(SwitchPanel.SwitchAnimationCompletedEvent, (Delegate) value);
  }

  protected RoutedEventArgs RaiseSwitchAnimationCompletedEvent()
  {
    return SwitchPanel.RaiseSwitchAnimationCompletedEvent((UIElement) this);
  }

  private static RoutedEventArgs RaiseSwitchAnimationCompletedEvent(UIElement target)
  {
    if (target == null)
      return (RoutedEventArgs) null;
    RoutedEventArgs args = new RoutedEventArgs();
    args.RoutedEvent = SwitchPanel.SwitchAnimationCompletedEvent;
    RoutedEventHelper.RaiseEvent((DependencyObject) target, args);
    return args;
  }

  protected override Size MeasureOverride(Size availableSize)
  {
    Size size = (this.Layouts.Count == 0 ? this._defaultLayoutCanvas : this.ActiveLayout).MeasureChildrenCore(this.InternalChildren, availableSize);
    if (this.IsScrollingPhysically)
    {
      Size viewport = availableSize;
      Size extent = size;
      Vector offset = new Vector(Math.Max(0.0, Math.Min(this._offset.X, extent.Width - viewport.Width)), Math.Max(0.0, Math.Min(this._offset.Y, extent.Height - viewport.Height)));
      this.SetScrollingData(viewport, extent, offset);
    }
    return size;
  }

  protected override Size ArrangeOverride(Size finalSize)
  {
    AnimationPanel animationPanel = this.Layouts.Count == 0 ? this._defaultLayoutCanvas : this.ActiveLayout;
    animationPanel.PhysicalScrollOffset = !this.IsScrollingPhysically ? SwitchPanel.ZeroVector : this._offset;
    return animationPanel.ArrangeChildrenCore(this.InternalChildren, finalSize);
  }

  protected override Visual GetVisualChild(int index)
  {
    return this.HasLoaded && this._currentLayoutPanel != null ? this._currentLayoutPanel.GetVisualChildInternal(index) : base.GetVisualChild(index);
  }

  protected override void OnVisualChildrenChanged(
    DependencyObject visualAdded,
    DependencyObject visualRemoved)
  {
    if (visualAdded is UIElement)
    {
      if (this._currentLayoutPanel == null || !this._currentLayoutPanel.IsRemovingInternalChild)
      {
        foreach (AnimationPanel layout in (Collection<AnimationPanel>) this.Layouts)
          layout.OnNotifyVisualChildAddedInternal(visualAdded as UIElement);
      }
    }
    else if (visualRemoved is UIElement)
    {
      foreach (AnimationPanel layout in (Collection<AnimationPanel>) this.Layouts)
        layout.OnNotifyVisualChildRemovedInternal(visualRemoved as UIElement);
    }
    if (this._currentLayoutPanel != null)
      this._currentLayoutPanel.OnSwitchParentVisualChildrenChanged(visualAdded, visualRemoved);
    else
      base.OnVisualChildrenChanged(visualAdded, visualRemoved);
  }

  internal void AddVisualChildInternal(Visual child) => this.AddVisualChild(child);

  internal void BeginLayoutSwitch() => this.RaiseSwitchAnimationBegunEvent();

  internal void EndLayoutSwitch() => this.RaiseSwitchAnimationCompletedEvent();

  internal Visual GetVisualChildInternal(int index) => base.GetVisualChild(index);

  internal void OnVisualChildrenChangedInternal(
    DependencyObject visualAdded,
    DependencyObject visualRemoved)
  {
    base.OnVisualChildrenChanged(visualAdded, visualRemoved);
  }

  internal UIElement RegisterPresenter(SwitchPresenter presenter)
  {
    UIElement ofAnimationPanel = AnimationPanel.FindAncestorChildOfAnimationPanel((DependencyObject) presenter, out AnimationPanel _);
    if (ofAnimationPanel == null)
      return ofAnimationPanel;
    this._presenters.Add(presenter);
    presenter.SwapTheTemplate(this.ActiveSwitchTemplate, false);
    return ofAnimationPanel;
  }

  internal void RemoveVisualChildInternal(Visual child) => this.RemoveVisualChild(child);

  internal void UnregisterPresenter(SwitchPresenter presenter, DependencyObject container)
  {
    if (container == null)
      return;
    this._presenters.Remove(presenter);
    presenter.SwapTheTemplate((DataTemplate) null, false);
  }

  internal void UpdateSwitchTemplate()
  {
    this.SetActiveSwitchTemplate(this.ActiveLayout == null || this.ActiveLayout.SwitchTemplate == null ? this.SwitchTemplate : this.ActiveLayout.SwitchTemplate);
  }

  private void OnLoaded(object sender, RoutedEventArgs e)
  {
    this.HasLoaded = true;
    this.InvalidateArrange();
  }

  private void LayoutsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    if (e.Action != NotifyCollectionChangedAction.Move)
    {
      if (e.NewItems != null)
      {
        foreach (AnimationPanel newItem in (IEnumerable) e.NewItems)
        {
          this.AddLogicalChild((object) newItem);
          newItem.SetSwitchParent(this);
          if (newItem is IScrollInfo)
            ((IScrollInfo) newItem).ScrollOwner = this.ScrollOwner;
          if (this.IsLoaded)
          {
            foreach (UIElement internalChild in this.InternalChildren)
            {
              if (internalChild != null)
                newItem.OnNotifyVisualChildAddedInternal(internalChild);
            }
          }
        }
      }
      if (e.OldItems != null)
      {
        foreach (AnimationPanel oldItem in (IEnumerable) e.OldItems)
        {
          if (this.IsLoaded)
          {
            foreach (UIElement internalChild in this.InternalChildren)
            {
              if (internalChild != null)
                oldItem.OnNotifyVisualChildRemovedInternal(internalChild);
            }
          }
          this.RemoveLogicalChild((object) oldItem);
          oldItem.SetSwitchParent((SwitchPanel) null);
          if (oldItem is IScrollInfo)
            ((IScrollInfo) oldItem).ScrollOwner = (ScrollViewer) null;
        }
      }
    }
    this.CoerceValue(SwitchPanel.ActiveLayoutIndexProperty);
    this.SetActiveLayout(this.Layouts.Count == 0 ? (AnimationPanel) null : this.Layouts[this.ActiveLayoutIndex]);
  }

  private void ResetScrollInfo()
  {
    this._offset = new Vector();
    this._viewport = this._extent = new Size(0.0, 0.0);
  }

  private void OnScrollChange()
  {
    if (this.ScrollOwner == null)
      return;
    this.ScrollOwner.InvalidateScrollInfo();
  }

  private void SetScrollingData(Size viewport, Size extent, Vector offset)
  {
    this._offset = offset;
    if (DoubleHelper.AreVirtuallyEqual(viewport, this._viewport) && DoubleHelper.AreVirtuallyEqual(extent, this._extent) && DoubleHelper.AreVirtuallyEqual(offset, this._computedOffset))
      return;
    this._viewport = viewport;
    this._extent = extent;
    this._computedOffset = offset;
    this.OnScrollChange();
  }

  private double ValidateInputOffset(double offset, string parameterName)
  {
    return !double.IsNaN(offset) ? Math.Max(0.0, offset) : throw new ArgumentOutOfRangeException(parameterName);
  }

  private int FindChildFromVisual(Visual vis)
  {
    int childFromVisual = -1;
    DependencyObject dependencyObject1 = (DependencyObject) vis;
    DependencyObject dependencyObject2;
    do
    {
      dependencyObject2 = dependencyObject1;
      dependencyObject1 = VisualTreeHelper.GetParent(dependencyObject2);
    }
    while (dependencyObject1 != null && dependencyObject1 != this);
    if (dependencyObject1 == this)
      childFromVisual = this.Children.IndexOf((UIElement) dependencyObject2);
    return childFromVisual;
  }

  public bool CanHorizontallyScroll
  {
    get
    {
      return this.ActiveLayout != null && this.ActiveLayout is IScrollInfo ? ((IScrollInfo) this.ActiveLayout).CanHorizontallyScroll : this._allowHorizontal;
    }
    set
    {
      if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
        ((IScrollInfo) this.ActiveLayout).CanHorizontallyScroll = value;
      else
        this._allowHorizontal = value;
    }
  }

  public bool CanVerticallyScroll
  {
    get
    {
      return this.ActiveLayout != null && this.ActiveLayout is IScrollInfo ? ((IScrollInfo) this.ActiveLayout).CanVerticallyScroll : this._allowVertical;
    }
    set
    {
      if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
        ((IScrollInfo) this.ActiveLayout).CanVerticallyScroll = value;
      else
        this._allowVertical = value;
    }
  }

  public double ExtentHeight
  {
    get
    {
      return this.ActiveLayout != null && this.ActiveLayout is IScrollInfo ? ((IScrollInfo) this.ActiveLayout).ExtentHeight : this._extent.Height;
    }
  }

  public double ExtentWidth
  {
    get
    {
      return this.ActiveLayout != null && this.ActiveLayout is IScrollInfo ? ((IScrollInfo) this.ActiveLayout).ExtentWidth : this._extent.Width;
    }
  }

  public double HorizontalOffset
  {
    get
    {
      return this.ActiveLayout != null && this.ActiveLayout is IScrollInfo ? ((IScrollInfo) this.ActiveLayout).HorizontalOffset : this._offset.X;
    }
  }

  public void LineDown()
  {
    if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
      ((IScrollInfo) this.ActiveLayout).LineDown();
    else
      this.SetVerticalOffset(this.VerticalOffset + 1.0);
  }

  public void LineLeft()
  {
    if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
      ((IScrollInfo) this.ActiveLayout).LineLeft();
    else
      this.SetHorizontalOffset(this.VerticalOffset - 1.0);
  }

  public void LineRight()
  {
    if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
      ((IScrollInfo) this.ActiveLayout).LineRight();
    else
      this.SetHorizontalOffset(this.VerticalOffset + 1.0);
  }

  public void LineUp()
  {
    if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
      ((IScrollInfo) this.ActiveLayout).LineUp();
    else
      this.SetVerticalOffset(this.VerticalOffset + 1.0);
  }

  public Rect MakeVisible(Visual visual, Rect rectangle)
  {
    if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
      return ((IScrollInfo) this.ActiveLayout).MakeVisible(visual, rectangle);
    if (rectangle.IsEmpty || visual == null || visual == this || !this.IsAncestorOf((DependencyObject) visual))
      return Rect.Empty;
    rectangle = visual.TransformToAncestor((Visual) this).TransformBounds(rectangle);
    if (!this.IsScrollingPhysically)
      return rectangle;
    if (this.FindChildFromVisual(visual) == -1)
      throw new ArgumentException(nameof (visual));
    Rect itemRect = rectangle;
    itemRect.Offset(this._offset);
    Vector newPhysOffset;
    if (ScrollHelper.ScrollLeastAmount(new Rect(new Point(this._offset.X, this._offset.Y), this._viewport), itemRect, out newPhysOffset))
    {
      this.SetHorizontalOffset(newPhysOffset.X);
      this.SetVerticalOffset(newPhysOffset.Y);
    }
    return rectangle;
  }

  public void MouseWheelDown()
  {
    if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
      ((IScrollInfo) this.ActiveLayout).MouseWheelDown();
    else
      this.SetVerticalOffset(this.VerticalOffset + (double) SystemParameters.WheelScrollLines);
  }

  public void MouseWheelLeft()
  {
    if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
      ((IScrollInfo) this.ActiveLayout).MouseWheelLeft();
    else
      this.SetVerticalOffset(this.VerticalOffset - 3.0);
  }

  public void MouseWheelRight()
  {
    if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
      ((IScrollInfo) this.ActiveLayout).MouseWheelRight();
    else
      this.SetVerticalOffset(this.VerticalOffset + 3.0);
  }

  public void MouseWheelUp()
  {
    if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
      ((IScrollInfo) this.ActiveLayout).MouseWheelUp();
    else
      this.SetVerticalOffset(this.VerticalOffset - (double) SystemParameters.WheelScrollLines);
  }

  public void PageDown()
  {
    if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
      ((IScrollInfo) this.ActiveLayout).PageDown();
    else
      this.SetVerticalOffset(this.VerticalOffset + this.ViewportHeight);
  }

  public void PageLeft()
  {
    if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
      ((IScrollInfo) this.ActiveLayout).PageLeft();
    else
      this.SetHorizontalOffset(this.HorizontalOffset - this.ViewportWidth);
  }

  public void PageRight()
  {
    if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
      ((IScrollInfo) this.ActiveLayout).PageRight();
    else
      this.SetHorizontalOffset(this.HorizontalOffset + this.ViewportWidth);
  }

  public void PageUp()
  {
    if (this.ActiveLayout != null && this.ActiveLayout is IScrollInfo)
      ((IScrollInfo) this.ActiveLayout).PageUp();
    else
      this.SetVerticalOffset(this.VerticalOffset - this.ViewportHeight);
  }

  public ScrollViewer ScrollOwner
  {
    get => this._scrollOwner;
    set
    {
      foreach (AnimationPanel layout in (Collection<AnimationPanel>) this.Layouts)
      {
        if (layout != null && layout is IScrollInfo)
          ((IScrollInfo) layout).ScrollOwner = value;
      }
      if (this._scrollOwner == value)
        return;
      this._scrollOwner = value;
      this.ResetScrollInfo();
    }
  }

  public void SetHorizontalOffset(double offset)
  {
    offset = this.ValidateInputOffset(offset, "HorizontalOffset");
    offset = Math.Min(offset, this.ExtentWidth - this.ViewportWidth);
    if (DoubleHelper.AreVirtuallyEqual(offset, this._offset.X))
      return;
    this._offset.X = offset;
    this.InvalidateMeasure();
  }

  public void SetVerticalOffset(double offset)
  {
    offset = this.ValidateInputOffset(offset, "VerticalOffset");
    offset = Math.Min(offset, this.ExtentHeight - this.ViewportHeight);
    if (DoubleHelper.AreVirtuallyEqual(offset, this._offset.Y))
      return;
    this._offset.Y = offset;
    this.InvalidateMeasure();
  }

  public double VerticalOffset
  {
    get
    {
      return this.ActiveLayout != null && this.ActiveLayout is IScrollInfo ? ((IScrollInfo) this.ActiveLayout).VerticalOffset : this._offset.Y;
    }
  }

  public double ViewportHeight
  {
    get
    {
      return this.ActiveLayout != null && this.ActiveLayout is IScrollInfo ? ((IScrollInfo) this.ActiveLayout).ViewportHeight : this._viewport.Height;
    }
  }

  public double ViewportWidth
  {
    get
    {
      return this.ActiveLayout != null && this.ActiveLayout is IScrollInfo ? ((IScrollInfo) this.ActiveLayout).ViewportWidth : this._viewport.Width;
    }
  }

  private enum CacheBits
  {
    HasLoaded = 1,
  }
}
