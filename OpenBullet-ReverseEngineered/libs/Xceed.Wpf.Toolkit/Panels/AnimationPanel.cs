// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Panels.AnimationPanel
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Media.Animation;

#nullable disable
namespace Xceed.Wpf.Toolkit.Panels;

public abstract class AnimationPanel : PanelBase
{
  private static readonly DependencyPropertyKey ChildStatePropertyKey = DependencyProperty.RegisterAttachedReadOnly("ChildState", typeof (AnimationPanel.ChildState), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty DefaultAnimationRateProperty = DependencyProperty.Register(nameof (DefaultAnimationRate), typeof (AnimationRate), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) (AnimationRate) 1.0), new ValidateValueCallback(AnimationPanel.ValidateDefaultAnimationRate));
  public static readonly DependencyProperty DefaultAnimatorProperty = DependencyProperty.Register(nameof (DefaultAnimator), typeof (IterativeAnimator), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) Animators.Linear), new ValidateValueCallback(AnimationPanel.ValidateDefaultAnimator));
  public static readonly DependencyProperty EnterAnimationRateProperty = DependencyProperty.Register(nameof (EnterAnimationRate), typeof (AnimationRate), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) AnimationRate.Default));
  public static readonly DependencyProperty EnterAnimatorProperty = DependencyProperty.Register(nameof (EnterAnimator), typeof (IterativeAnimator), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) IterativeAnimator.Default));
  public static readonly DependencyProperty EnterFromProperty = DependencyProperty.RegisterAttached("EnterFrom", typeof (Rect?), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.Inherits));
  public static readonly DependencyProperty ExitAnimationRateProperty = DependencyProperty.Register(nameof (ExitAnimationRate), typeof (AnimationRate), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) AnimationRate.Default));
  public static readonly DependencyProperty ExitAnimatorProperty = DependencyProperty.Register(nameof (ExitAnimator), typeof (IterativeAnimator), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) IterativeAnimator.Default));
  public static readonly DependencyProperty ExitToProperty = DependencyProperty.RegisterAttached("ExitTo", typeof (Rect?), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.Inherits));
  public static readonly DependencyProperty LayoutAnimationRateProperty = DependencyProperty.Register(nameof (LayoutAnimationRate), typeof (AnimationRate), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) AnimationRate.Default));
  public static readonly DependencyProperty LayoutAnimatorProperty = DependencyProperty.Register(nameof (LayoutAnimator), typeof (IterativeAnimator), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) IterativeAnimator.Default));
  public static readonly DependencyProperty SwitchAnimationRateProperty = DependencyProperty.Register(nameof (SwitchAnimationRate), typeof (AnimationRate), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) AnimationRate.Default));
  public static readonly DependencyProperty SwitchAnimatorProperty = DependencyProperty.Register(nameof (SwitchAnimator), typeof (IterativeAnimator), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) IterativeAnimator.Default));
  private static readonly DependencyPropertyKey SwitchParentPropertyKey = DependencyProperty.RegisterReadOnly(nameof (SwitchParent), typeof (SwitchPanel), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(AnimationPanel.OnSwitchParentChanged)));
  public static readonly DependencyProperty SwitchParentProperty = AnimationPanel.SwitchParentPropertyKey.DependencyProperty;
  public static readonly DependencyProperty SwitchTemplateProperty = DependencyProperty.Register(nameof (SwitchTemplate), typeof (DataTemplate), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(AnimationPanel.OnSwitchTemplateChanged)));
  public static readonly DependencyProperty TemplateAnimationRateProperty = DependencyProperty.Register(nameof (TemplateAnimationRate), typeof (AnimationRate), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) AnimationRate.Default));
  public static readonly DependencyProperty TemplateAnimatorProperty = DependencyProperty.Register(nameof (TemplateAnimator), typeof (IterativeAnimator), typeof (AnimationPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) IterativeAnimator.Default));
  private Vector _physicalScrollOffset;
  private int _animatingChildCount;
  public static readonly RoutedEvent AnimationBegunEvent = EventManager.RegisterRoutedEvent("AnimationBegun", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (AnimationPanel));
  public static readonly RoutedEvent AnimationCompletedEvent = EventManager.RegisterRoutedEvent("AnimationCompleted", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (AnimationPanel));
  public static readonly RoutedEvent ChildEnteredEvent = EventManager.RegisterRoutedEvent("ChildEntered", RoutingStrategy.Bubble, typeof (ChildEnteredEventHandler), typeof (AnimationPanel));
  public static readonly RoutedEvent ChildEnteringEvent = EventManager.RegisterRoutedEvent("ChildEntering", RoutingStrategy.Bubble, typeof (ChildEnteringEventHandler), typeof (AnimationPanel));
  public static readonly RoutedEvent ChildExitedEvent = EventManager.RegisterRoutedEvent("ChildExited", RoutingStrategy.Bubble, typeof (ChildExitedEventHandler), typeof (AnimationPanel));
  public static readonly RoutedEvent ChildExitingEvent = EventManager.RegisterRoutedEvent("ChildExiting", RoutingStrategy.Bubble, typeof (ChildExitingEventHandler), typeof (AnimationPanel));
  public static readonly RoutedEvent SwitchLayoutActivatedEvent = EventManager.RegisterRoutedEvent("SwitchLayoutActivated", RoutingStrategy.Direct, typeof (RoutedEventHandler), typeof (AnimationPanel));
  public static readonly RoutedEvent SwitchLayoutDeactivatedEvent = EventManager.RegisterRoutedEvent("SwitchLayoutDeactivated", RoutingStrategy.Direct, typeof (RoutedEventHandler), typeof (AnimationPanel));
  private UIElementCollection _currentChildren;
  private readonly Collection<FrameworkElement> _animatingGrandchildren = new Collection<FrameworkElement>();
  private SwitchPanel _switchParent;
  private List<UIElement> _exitingChildren;
  private BitVector32 _cacheBits = new BitVector32(1);

  public AnimationPanel() => this.Loaded += new RoutedEventHandler(this.OnLoaded);

  private static AnimationPanel.ChildState GetChildState(DependencyObject d)
  {
    return (AnimationPanel.ChildState) d.GetValue(AnimationPanel.ChildStatePropertyKey.DependencyProperty);
  }

  private static void SetChildState(DependencyObject d, AnimationPanel.ChildState value)
  {
    d.SetValue(AnimationPanel.ChildStatePropertyKey, (object) value);
  }

  public AnimationRate DefaultAnimationRate
  {
    get => (AnimationRate) this.GetValue(AnimationPanel.DefaultAnimationRateProperty);
    set => this.SetValue(AnimationPanel.DefaultAnimationRateProperty, (object) value);
  }

  private static bool ValidateDefaultAnimationRate(object value)
  {
    if ((AnimationRate) value == AnimationRate.Default)
      throw new ArgumentException(ErrorMessages.GetMessage("DefaultAnimationRateAnimationRateDefault"));
    return true;
  }

  public IterativeAnimator DefaultAnimator
  {
    get => (IterativeAnimator) this.GetValue(AnimationPanel.DefaultAnimatorProperty);
    set => this.SetValue(AnimationPanel.DefaultAnimatorProperty, (object) value);
  }

  private static bool ValidateDefaultAnimator(object value)
  {
    if (value == IterativeAnimator.Default)
      throw new ArgumentException(ErrorMessages.GetMessage("DefaultAnimatorIterativeAnimationDefault"));
    return true;
  }

  public AnimationRate EnterAnimationRate
  {
    get => (AnimationRate) this.GetValue(AnimationPanel.EnterAnimationRateProperty);
    set => this.SetValue(AnimationPanel.EnterAnimationRateProperty, (object) value);
  }

  public IterativeAnimator EnterAnimator
  {
    get => (IterativeAnimator) this.GetValue(AnimationPanel.EnterAnimatorProperty);
    set => this.SetValue(AnimationPanel.EnterAnimatorProperty, (object) value);
  }

  public static Rect? GetEnterFrom(DependencyObject d)
  {
    return (Rect?) d.GetValue(AnimationPanel.EnterFromProperty);
  }

  public static void SetEnterFrom(DependencyObject d, Rect? value)
  {
    d.SetValue(AnimationPanel.EnterFromProperty, (object) value);
  }

  public AnimationRate ExitAnimationRate
  {
    get => (AnimationRate) this.GetValue(AnimationPanel.ExitAnimationRateProperty);
    set => this.SetValue(AnimationPanel.ExitAnimationRateProperty, (object) value);
  }

  public IterativeAnimator ExitAnimator
  {
    get => (IterativeAnimator) this.GetValue(AnimationPanel.ExitAnimatorProperty);
    set => this.SetValue(AnimationPanel.ExitAnimatorProperty, (object) value);
  }

  public static Rect? GetExitTo(DependencyObject d)
  {
    return (Rect?) d.GetValue(AnimationPanel.ExitToProperty);
  }

  public static void SetExitTo(DependencyObject d, Rect? value)
  {
    d.SetValue(AnimationPanel.ExitToProperty, (object) value);
  }

  public AnimationRate LayoutAnimationRate
  {
    get => (AnimationRate) this.GetValue(AnimationPanel.LayoutAnimationRateProperty);
    set => this.SetValue(AnimationPanel.LayoutAnimationRateProperty, (object) value);
  }

  public IterativeAnimator LayoutAnimator
  {
    get => (IterativeAnimator) this.GetValue(AnimationPanel.LayoutAnimatorProperty);
    set => this.SetValue(AnimationPanel.LayoutAnimatorProperty, (object) value);
  }

  public AnimationRate SwitchAnimationRate
  {
    get => (AnimationRate) this.GetValue(AnimationPanel.SwitchAnimationRateProperty);
    set => this.SetValue(AnimationPanel.SwitchAnimationRateProperty, (object) value);
  }

  public IterativeAnimator SwitchAnimator
  {
    get => (IterativeAnimator) this.GetValue(AnimationPanel.SwitchAnimatorProperty);
    set => this.SetValue(AnimationPanel.SwitchAnimatorProperty, (object) value);
  }

  public SwitchPanel SwitchParent
  {
    get => (SwitchPanel) this.GetValue(AnimationPanel.SwitchParentProperty);
  }

  protected internal void SetSwitchParent(SwitchPanel value)
  {
    this.SetValue(AnimationPanel.SwitchParentPropertyKey, (object) value);
  }

  private static void OnSwitchParentChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((AnimationPanel) d).OnSwitchParentChanged(e);
  }

  protected virtual void OnSwitchParentChanged(DependencyPropertyChangedEventArgs e)
  {
    this._switchParent = e.NewValue as SwitchPanel;
  }

  public DataTemplate SwitchTemplate
  {
    get => (DataTemplate) this.GetValue(AnimationPanel.SwitchTemplateProperty);
    set => this.SetValue(AnimationPanel.SwitchTemplateProperty, (object) value);
  }

  private static void OnSwitchTemplateChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((AnimationPanel) d).OnSwitchTemplateChanged(e);
  }

  protected virtual void OnSwitchTemplateChanged(DependencyPropertyChangedEventArgs e)
  {
    if (this._switchParent == null || this._switchParent.ActiveLayout != this)
      return;
    this._switchParent.UpdateSwitchTemplate();
  }

  public AnimationRate TemplateAnimationRate
  {
    get => (AnimationRate) this.GetValue(AnimationPanel.TemplateAnimationRateProperty);
    set => this.SetValue(AnimationPanel.TemplateAnimationRateProperty, (object) value);
  }

  public IterativeAnimator TemplateAnimator
  {
    get => (IterativeAnimator) this.GetValue(AnimationPanel.TemplateAnimatorProperty);
    set => this.SetValue(AnimationPanel.TemplateAnimatorProperty, (object) value);
  }

  public new Size DesiredSize
  {
    get => this._switchParent == null ? base.DesiredSize : this._switchParent.DesiredSize;
  }

  public new Size RenderSize
  {
    get => this._switchParent == null ? base.RenderSize : this._switchParent.RenderSize;
    set => base.RenderSize = value;
  }

  public bool IsActiveLayout
  {
    get => this._cacheBits[1];
    private set => this._cacheBits[1] = value;
  }

  protected internal new UIElementCollection InternalChildren
  {
    get => this._switchParent != null ? this._switchParent.ChildrenInternal : base.InternalChildren;
  }

  protected override int VisualChildrenCount
  {
    get
    {
      return !this.HasLoaded ? base.VisualChildrenCount : this.InternalChildren.Count + this.ExitingChildren.Count;
    }
  }

  protected PanelBase ChildrensParent
  {
    get => this._switchParent == null ? (PanelBase) this : (PanelBase) this._switchParent;
  }

  internal int VisualChildrenCountInternal => this.VisualChildrenCount;

  internal Vector PhysicalScrollOffset
  {
    get => this._physicalScrollOffset;
    set => this._physicalScrollOffset = value;
  }

  internal bool IsRemovingInternalChild
  {
    get => this._cacheBits[32 /*0x20*/];
    private set => this._cacheBits[32 /*0x20*/] = value;
  }

  private int AnimatingChildCount
  {
    get => this._animatingChildCount;
    set
    {
      if (this._animatingChildCount == 0 && value > 0)
      {
        CompositionTarget.Rendering += new EventHandler(this.OnRendering);
        this.RaiseAnimationBegunEvent();
      }
      if (this._animatingChildCount != 0 && value == 0)
      {
        if (this.EndSwitchOnAnimationCompleted && this._switchParent != null)
        {
          this.EndSwitchOnAnimationCompleted = false;
          this._switchParent.EndLayoutSwitch();
        }
        CompositionTarget.Rendering -= new EventHandler(this.OnRendering);
        this.RaiseAnimationCompletedEvent();
      }
      this._animatingChildCount = value;
    }
  }

  private bool EndSwitchOnAnimationCompleted
  {
    get => this._cacheBits[16 /*0x10*/];
    set => this._cacheBits[16 /*0x10*/] = value;
  }

  private bool HasArranged
  {
    get => this._cacheBits[128 /*0x80*/];
    set => this._cacheBits[128 /*0x80*/] = value;
  }

  protected bool HasLoaded
  {
    get => this._switchParent != null ? this._switchParent.HasLoaded : this._cacheBits[64 /*0x40*/];
    private set => this._cacheBits[64 /*0x40*/] = value;
  }

  private bool IsSwitchInProgress
  {
    get => this._cacheBits[2];
    set => this._cacheBits[2] = value;
  }

  private ItemsControl ItemsOwner
  {
    get
    {
      return ItemsControl.GetItemsOwner(this._switchParent == null ? (DependencyObject) this : (DependencyObject) this._switchParent);
    }
  }

  private List<UIElement> ExitingChildren
  {
    get
    {
      if (this._switchParent != null)
        return this._switchParent.ExitingChildren;
      if (this._exitingChildren == null)
        this._exitingChildren = new List<UIElement>();
      return this._exitingChildren;
    }
  }

  public event RoutedEventHandler AnimationBegun
  {
    add => this.AddHandler(AnimationPanel.AnimationBegunEvent, (Delegate) value);
    remove => this.RemoveHandler(AnimationPanel.AnimationBegunEvent, (Delegate) value);
  }

  protected RoutedEventArgs RaiseAnimationBegunEvent()
  {
    return AnimationPanel.RaiseAnimationBegunEvent(this._switchParent != null ? (UIElement) this._switchParent : (UIElement) this);
  }

  private static RoutedEventArgs RaiseAnimationBegunEvent(UIElement target)
  {
    if (target == null)
      return (RoutedEventArgs) null;
    RoutedEventArgs args = new RoutedEventArgs();
    args.RoutedEvent = AnimationPanel.AnimationBegunEvent;
    RoutedEventHelper.RaiseEvent((DependencyObject) target, args);
    return args;
  }

  public event RoutedEventHandler AnimationCompleted
  {
    add => this.AddHandler(AnimationPanel.AnimationCompletedEvent, (Delegate) value);
    remove => this.RemoveHandler(AnimationPanel.AnimationCompletedEvent, (Delegate) value);
  }

  protected RoutedEventArgs RaiseAnimationCompletedEvent()
  {
    return AnimationPanel.RaiseAnimationCompletedEvent(this._switchParent != null ? (UIElement) this._switchParent : (UIElement) this);
  }

  private static RoutedEventArgs RaiseAnimationCompletedEvent(UIElement target)
  {
    if (target == null)
      return (RoutedEventArgs) null;
    RoutedEventArgs args = new RoutedEventArgs();
    args.RoutedEvent = AnimationPanel.AnimationCompletedEvent;
    RoutedEventHelper.RaiseEvent((DependencyObject) target, args);
    return args;
  }

  public event ChildEnteredEventHandler ChildEntered
  {
    add => this.AddHandler(AnimationPanel.ChildEnteredEvent, (Delegate) value);
    remove => this.RemoveHandler(AnimationPanel.ChildEnteredEvent, (Delegate) value);
  }

  protected ChildEnteredEventArgs RaiseChildEnteredEvent(UIElement child, Rect arrangeRect)
  {
    return AnimationPanel.RaiseChildEnteredEvent((UIElement) this, child, arrangeRect);
  }

  internal static ChildEnteredEventArgs RaiseChildEnteredEvent(
    UIElement target,
    UIElement child,
    Rect arrangeRect)
  {
    if (target == null)
      return (ChildEnteredEventArgs) null;
    ChildEnteredEventArgs args = new ChildEnteredEventArgs(child, arrangeRect);
    args.RoutedEvent = AnimationPanel.ChildEnteredEvent;
    RoutedEventHelper.RaiseEvent((DependencyObject) target, (RoutedEventArgs) args);
    return args;
  }

  public event ChildEnteringEventHandler ChildEntering
  {
    add => this.AddHandler(AnimationPanel.ChildEnteringEvent, (Delegate) value);
    remove => this.RemoveHandler(AnimationPanel.ChildEnteringEvent, (Delegate) value);
  }

  protected ChildEnteringEventArgs RaiseChildEnteringEvent(
    UIElement child,
    Rect? EnterFrom,
    Rect ArrangeRect)
  {
    return AnimationPanel.RaiseChildEnteringEvent((UIElement) this, child, EnterFrom, ArrangeRect);
  }

  private static ChildEnteringEventArgs RaiseChildEnteringEvent(
    UIElement target,
    UIElement child,
    Rect? EnterFrom,
    Rect ArrangeRect)
  {
    if (target == null)
      return (ChildEnteringEventArgs) null;
    ChildEnteringEventArgs args = new ChildEnteringEventArgs(child, EnterFrom, ArrangeRect);
    args.RoutedEvent = AnimationPanel.ChildEnteringEvent;
    RoutedEventHelper.RaiseEvent((DependencyObject) target, (RoutedEventArgs) args);
    return args;
  }

  public event ChildExitedEventHandler ChildExited
  {
    add => this.AddHandler(AnimationPanel.ChildExitedEvent, (Delegate) value);
    remove => this.RemoveHandler(AnimationPanel.ChildExitedEvent, (Delegate) value);
  }

  protected ChildExitedEventArgs RaiseChildExitedEvent(UIElement child)
  {
    return AnimationPanel.RaiseChildExitedEvent((UIElement) this, child);
  }

  private static ChildExitedEventArgs RaiseChildExitedEvent(UIElement target, UIElement child)
  {
    if (target == null)
      return (ChildExitedEventArgs) null;
    ChildExitedEventArgs args = new ChildExitedEventArgs(child);
    args.RoutedEvent = AnimationPanel.ChildExitedEvent;
    RoutedEventHelper.RaiseEvent((DependencyObject) target, (RoutedEventArgs) args);
    return args;
  }

  public event ChildExitingEventHandler ChildExiting
  {
    add => this.AddHandler(AnimationPanel.ChildExitingEvent, (Delegate) value);
    remove => this.RemoveHandler(AnimationPanel.ChildExitingEvent, (Delegate) value);
  }

  protected ChildExitingEventArgs RaiseChildExitingEvent(
    UIElement child,
    Rect? exitTo,
    Rect arrangeRect)
  {
    return AnimationPanel.RaiseChildExitingEvent((UIElement) this, child, exitTo, arrangeRect);
  }

  private static ChildExitingEventArgs RaiseChildExitingEvent(
    UIElement target,
    UIElement child,
    Rect? exitTo,
    Rect arrangeRect)
  {
    if (target == null)
      return (ChildExitingEventArgs) null;
    ChildExitingEventArgs args = new ChildExitingEventArgs(child, exitTo, arrangeRect);
    args.RoutedEvent = AnimationPanel.ChildExitingEvent;
    RoutedEventHelper.RaiseEvent((DependencyObject) target, (RoutedEventArgs) args);
    return args;
  }

  public event RoutedEventHandler SwitchLayoutActivated
  {
    add => this.AddHandler(AnimationPanel.SwitchLayoutActivatedEvent, (Delegate) value);
    remove => this.RemoveHandler(AnimationPanel.SwitchLayoutActivatedEvent, (Delegate) value);
  }

  protected RoutedEventArgs RaiseSwitchLayoutActivatedEvent()
  {
    return AnimationPanel.RaiseSwitchLayoutActivatedEvent((UIElement) this);
  }

  internal static RoutedEventArgs RaiseSwitchLayoutActivatedEvent(UIElement target)
  {
    if (target == null)
      return (RoutedEventArgs) null;
    RoutedEventArgs args = new RoutedEventArgs();
    args.RoutedEvent = AnimationPanel.SwitchLayoutActivatedEvent;
    RoutedEventHelper.RaiseEvent((DependencyObject) target, args);
    return args;
  }

  public event RoutedEventHandler SwitchLayoutDeactivated
  {
    add => this.AddHandler(AnimationPanel.SwitchLayoutDeactivatedEvent, (Delegate) value);
    remove => this.RemoveHandler(AnimationPanel.SwitchLayoutDeactivatedEvent, (Delegate) value);
  }

  protected RoutedEventArgs RaiseSwitchLayoutDeactivatedEvent()
  {
    return AnimationPanel.RaiseSwitchLayoutDeactivatedEvent((UIElement) this);
  }

  internal static RoutedEventArgs RaiseSwitchLayoutDeactivatedEvent(UIElement target)
  {
    if (target == null)
      return (RoutedEventArgs) null;
    RoutedEventArgs args = new RoutedEventArgs();
    args.RoutedEvent = AnimationPanel.SwitchLayoutDeactivatedEvent;
    RoutedEventHelper.RaiseEvent((DependencyObject) target, args);
    return args;
  }

  public new void InvalidateArrange()
  {
    if (this._switchParent == null)
      base.InvalidateArrange();
    else
      this._switchParent.InvalidateArrange();
  }

  public new void InvalidateMeasure()
  {
    if (this._switchParent == null)
      base.InvalidateMeasure();
    else
      this._switchParent.InvalidateMeasure();
  }

  public new void InvalidateVisual()
  {
    if (this._switchParent == null)
      base.InvalidateVisual();
    else
      this._switchParent.InvalidateVisual();
  }

  internal void ActivateLayout()
  {
    this.HasArranged = false;
    this.IsActiveLayout = true;
    this.OnSwitchLayoutActivated();
    this.RaiseSwitchLayoutActivatedEvent();
  }

  internal void BeginChildExit(UIElement child)
  {
    AnimationPanel.ChildState childState1 = AnimationPanel.GetChildState((DependencyObject) child);
    if (childState1 == null)
      return;
    childState1.Type = AnimationPanel.AnimationType.Exit;
    childState1.HasExitBegun = true;
    this.ExitingChildren.Add(child);
    if (this._switchParent != null)
      this._switchParent.AddVisualChildInternal((Visual) child);
    else
      this.AddVisualChild((Visual) child);
    ChildExitingEventArgs exitingEventArgs = AnimationPanel.RaiseChildExitingEvent(child, child, AnimationPanel.GetExitTo((DependencyObject) child), childState1.CurrentPlacement);
    childState1.Animator = this.GetEffectiveAnimator(AnimationPanel.AnimationType.Exit);
    if (childState1.Animator != null)
    {
      AnimationPanel.ChildState childState2 = childState1;
      Rect? exitTo = exitingEventArgs.ExitTo;
      Rect empty;
      if (!exitTo.HasValue)
      {
        empty = Rect.Empty;
      }
      else
      {
        exitTo = exitingEventArgs.ExitTo;
        empty = exitTo.Value;
      }
      childState2.TargetPlacement = empty;
      childState1.BeginTimeStamp = DateTime.Now;
      if (childState1.IsAnimating)
        --this.AnimatingChildCount;
      this.ArrangeChild(child, childState1.TargetPlacement);
    }
    else
      this.EndChildExit(child, childState1);
  }

  internal void BeginGrandchildAnimation(
    FrameworkElement grandchild,
    Rect currentRect,
    Rect placementRect)
  {
    bool isDone = true;
    AnimationPanel.ChildState childState = new AnimationPanel.ChildState(currentRect);
    AnimationPanel.SetChildState((DependencyObject) grandchild, childState);
    childState.Type = AnimationPanel.AnimationType.Switch;
    childState.BeginTimeStamp = DateTime.Now;
    childState.TargetPlacement = placementRect;
    childState.Animator = this.GetEffectiveAnimator(AnimationPanel.AnimationType.Template);
    if (childState.Animator != null && !childState.TargetPlacement.IsEmpty)
    {
      AnimationRate effectiveAnimationRate = this.GetEffectiveAnimationRate(AnimationPanel.AnimationType.Template);
      object placementArgs;
      childState.CurrentPlacement = childState.Animator.GetInitialChildPlacement((UIElement) grandchild, childState.CurrentPlacement, childState.TargetPlacement, this, ref effectiveAnimationRate, out placementArgs, out isDone);
      childState.AnimationRate = effectiveAnimationRate;
      childState.PlacementArgs = placementArgs;
    }
    childState.IsAnimating = !isDone;
    grandchild.Arrange(childState.IsAnimating ? childState.CurrentPlacement : childState.TargetPlacement);
    if (childState.IsAnimating)
    {
      this._animatingGrandchildren.Add(grandchild);
      ++this.AnimatingChildCount;
    }
    else
      childState.CurrentPlacement = childState.TargetPlacement;
  }

  internal void DeactivateLayout()
  {
    this.IsActiveLayout = false;
    this.AnimatingChildCount = 0;
    this.OnSwitchLayoutDeactivated();
    this.RaiseSwitchLayoutDeactivatedEvent();
  }

  internal static UIElement FindAncestorChildOfAnimationPanel(
    DependencyObject element,
    out AnimationPanel panel)
  {
    panel = (AnimationPanel) null;
    if (element == null)
      return (UIElement) null;
    DependencyObject parent = VisualTreeHelper.GetParent(element);
    switch (parent)
    {
      case null:
        return (UIElement) null;
      case AnimationPanel _:
      case SwitchPanel _:
        panel = parent is SwitchPanel ? (parent as SwitchPanel)._currentLayoutPanel : parent as AnimationPanel;
        return element as UIElement;
      default:
        return AnimationPanel.FindAncestorChildOfAnimationPanel(parent, out panel);
    }
  }

  internal Dictionary<string, Rect> GetNewLocationsBasedOnTargetPlacement(
    SwitchPresenter presenter,
    UIElement parent)
  {
    AnimationPanel.ChildState childState = AnimationPanel.GetChildState((DependencyObject) parent);
    bool flag = childState.CurrentPlacement != childState.TargetPlacement && childState.IsAnimating;
    if (flag)
      parent.Arrange(childState.TargetPlacement);
    Dictionary<string, Rect> onTargetPlacement = new Dictionary<string, Rect>();
    foreach (KeyValuePair<string, FrameworkElement> knownId in presenter._knownIDs)
    {
      Size renderSize = knownId.Value.RenderSize;
      Point[] pointArray = new Point[2];
      pointArray[1] = new Point(renderSize.Width, renderSize.Height);
      Point[] points = pointArray;
      (knownId.Value.TransformToAncestor(VisualTreeHelper.GetParent((DependencyObject) knownId.Value) as Visual) as MatrixTransform).Matrix.Transform(points);
      onTargetPlacement[knownId.Key] = new Rect(points[0], points[1]);
    }
    if (flag)
      parent.Arrange(childState.CurrentPlacement);
    return onTargetPlacement;
  }

  internal Visual GetVisualChildInternal(int index) => this.GetVisualChild(index);

  internal void OnNotifyVisualChildAddedInternal(UIElement child)
  {
    this.OnNotifyVisualChildAdded(child);
  }

  internal void OnNotifyVisualChildRemovedInternal(UIElement child)
  {
    this.OnNotifyVisualChildRemoved(child);
  }

  internal Size MeasureChildrenCore(UIElementCollection children, Size constraint)
  {
    this._currentChildren = children;
    return this.MeasureChildrenOverride(this._currentChildren, constraint);
  }

  internal Size ArrangeChildrenCore(UIElementCollection children, Size finalSize)
  {
    if (this._currentChildren != children)
      this._currentChildren = children;
    this.AnimatingChildCount = 0;
    this._animatingGrandchildren.Clear();
    Size size;
    try
    {
      if (!this.HasArranged && this._switchParent != null)
      {
        this.IsSwitchInProgress = true;
        this._switchParent.BeginLayoutSwitch();
      }
      size = this.ArrangeChildrenOverride(this._currentChildren, finalSize);
      if (this.ExitingChildren.Count > 0)
      {
        this.AnimatingChildCount += this.ExitingChildren.Count;
        this.UpdateExitingChildren();
      }
      if (this.IsSwitchInProgress)
      {
        if (this.AnimatingChildCount == 0)
          this._switchParent.EndLayoutSwitch();
        else
          this.EndSwitchOnAnimationCompleted = true;
      }
    }
    finally
    {
      this.HasArranged = true;
      this.IsSwitchInProgress = false;
    }
    return size;
  }

  internal void OnSwitchParentVisualChildrenChanged(
    DependencyObject visualAdded,
    DependencyObject visualRemoved)
  {
    this.OnVisualChildrenChanged(visualAdded, visualRemoved);
  }

  protected sealed override Size MeasureOverride(Size constraint)
  {
    return this.MeasureChildrenCore(this.InternalChildren, constraint);
  }

  protected abstract Size MeasureChildrenOverride(UIElementCollection children, Size constraint);

  protected sealed override Size ArrangeOverride(Size finalSize)
  {
    return this.ArrangeChildrenCore(this._currentChildren, finalSize);
  }

  protected abstract Size ArrangeChildrenOverride(UIElementCollection children, Size finalSize);

  protected void ArrangeChild(UIElement child, Rect placementRect)
  {
    if (!placementRect.IsEmpty && this.PhysicalScrollOffset.Length > 0.0)
      placementRect.Offset(-this.PhysicalScrollOffset);
    if (this.HasLoaded)
    {
      if (!this.BeginChildAnimation(child, placementRect))
        return;
      ++this.AnimatingChildCount;
    }
    else
      child.Arrange(placementRect);
  }

  protected new void AddVisualChild(Visual child)
  {
    if (this._switchParent == null)
      base.AddVisualChild(child);
    else
      this._switchParent.AddVisualChildInternal(child);
  }

  protected override Visual GetVisualChild(int index)
  {
    if (index < 0)
      throw new IndexOutOfRangeException();
    if (index >= this.InternalChildren.Count)
    {
      int index1 = index - this.InternalChildren.Count;
      if (index1 < 0 || index1 >= this.ExitingChildren.Count)
        throw new IndexOutOfRangeException();
      return (Visual) this.ExitingChildren[index1];
    }
    return this._switchParent != null ? this._switchParent.GetVisualChildInternal(index) : base.GetVisualChild(index);
  }

  protected virtual void OnNotifyVisualChildAdded(UIElement child)
  {
  }

  protected virtual void OnNotifyVisualChildRemoved(UIElement child)
  {
  }

  protected virtual void OnSwitchLayoutActivated()
  {
  }

  protected virtual void OnSwitchLayoutDeactivated()
  {
  }

  protected override void OnVisualChildrenChanged(
    DependencyObject visualAdded,
    DependencyObject visualRemoved)
  {
    if (!this.IsRemovingInternalChild && visualRemoved is UIElement && visualRemoved != null)
    {
      this.IsRemovingInternalChild = true;
      try
      {
        this.BeginChildExit(visualRemoved as UIElement);
      }
      finally
      {
        this.IsRemovingInternalChild = false;
      }
    }
    if (this._switchParent == null)
    {
      if (visualAdded is UIElement)
        this.OnNotifyVisualChildAdded(visualAdded as UIElement);
      else if (visualRemoved is UIElement)
        this.OnNotifyVisualChildRemoved(visualRemoved as UIElement);
      base.OnVisualChildrenChanged(visualAdded, visualRemoved);
    }
    else
      this._switchParent.OnVisualChildrenChangedInternal(visualAdded, visualRemoved);
  }

  protected new void RemoveVisualChild(Visual child)
  {
    if (this._switchParent == null)
      base.RemoveVisualChild(child);
    else
      this._switchParent.RemoveVisualChildInternal(child);
  }

  protected int FindChildFromVisual(Visual vis)
  {
    int childFromVisual = -1;
    DependencyObject dependencyObject1 = (DependencyObject) vis;
    DependencyObject dependencyObject2;
    do
    {
      dependencyObject2 = dependencyObject1;
      dependencyObject1 = VisualTreeHelper.GetParent(dependencyObject2);
    }
    while (dependencyObject1 != null && dependencyObject1 != this.ChildrensParent);
    if (dependencyObject1 == this.ChildrensParent)
      childFromVisual = this.ChildrensParent.Children.IndexOf((UIElement) dependencyObject2);
    return childFromVisual;
  }

  private bool BeginChildAnimation(UIElement child, Rect placementRect)
  {
    AnimationPanel.ChildState state = this.EnsureChildState(child, placementRect, out bool _);
    if (state.HasEnterCompleted)
    {
      if (state.Type != AnimationPanel.AnimationType.Exit)
      {
        state.BeginTimeStamp = DateTime.Now;
        state.Type = this.IsSwitchInProgress ? AnimationPanel.AnimationType.Switch : AnimationPanel.AnimationType.Layout;
        state.TargetPlacement = placementRect;
      }
    }
    else
    {
      state.BeginTimeStamp = DateTime.Now;
      state.TargetPlacement = placementRect;
    }
    if (!state.HasExitCompleted)
    {
      bool isDone = true;
      if (state.Type != AnimationPanel.AnimationType.Enter)
        state.Animator = this.GetEffectiveAnimator(state.Type);
      if (state.Animator != null && !state.TargetPlacement.IsEmpty)
      {
        AnimationRate effectiveAnimationRate = this.GetEffectiveAnimationRate(state.Type);
        object placementArgs;
        state.CurrentPlacement = state.Animator.GetInitialChildPlacement(child, state.CurrentPlacement, state.TargetPlacement, this, ref effectiveAnimationRate, out placementArgs, out isDone);
        state.AnimationRate = effectiveAnimationRate;
        state.PlacementArgs = placementArgs;
      }
      state.IsAnimating = !isDone;
      if (!state.IsAnimating)
        state.CurrentPlacement = state.TargetPlacement;
    }
    if (!state.IsAnimating)
      this.UpdateTrueArrange(child, state);
    return state.IsAnimating;
  }

  private void BeginChildEnter(UIElement child, AnimationPanel.ChildState state)
  {
    state.Type = AnimationPanel.AnimationType.Enter;
    ChildEnteringEventArgs enteringEventArgs = AnimationPanel.RaiseChildEnteringEvent(child, child, AnimationPanel.GetEnterFrom((DependencyObject) child), state.CurrentPlacement);
    state.Animator = this.GetEffectiveAnimator(AnimationPanel.AnimationType.Enter);
    if (state.Animator == null)
      return;
    Rect? enterFrom = enteringEventArgs.EnterFrom;
    if (!enterFrom.HasValue)
      return;
    AnimationPanel.ChildState childState = state;
    enterFrom = enteringEventArgs.EnterFrom;
    Rect rect = enterFrom.Value;
    childState.CurrentPlacement = rect;
    state.BeginTimeStamp = DateTime.Now;
  }

  private void EndChildEnter(UIElement child, AnimationPanel.ChildState state)
  {
    state.HasEnterCompleted = true;
    AnimationPanel.RaiseChildEnteredEvent(child, child, state.TargetPlacement);
  }

  private void EndChildExit(UIElement child, AnimationPanel.ChildState state)
  {
    state.HasExitCompleted = true;
    AnimationPanel.RaiseChildExitedEvent(child, child);
    if (this.ExitingChildren.Contains(child))
    {
      this.IsRemovingInternalChild = true;
      try
      {
        if (this._switchParent != null)
          this._switchParent.RemoveVisualChildInternal((Visual) child);
        else
          this.RemoveVisualChild((Visual) child);
      }
      finally
      {
        this.IsRemovingInternalChild = false;
      }
      this.ExitingChildren.Remove(child);
    }
    child.ClearValue(AnimationPanel.ChildStatePropertyKey);
  }

  private AnimationPanel.ChildState EnsureChildState(
    UIElement child,
    Rect placementRect,
    out bool newStateCreated)
  {
    newStateCreated = false;
    AnimationPanel.ChildState state = AnimationPanel.GetChildState((DependencyObject) child);
    if (state == null)
    {
      state = new AnimationPanel.ChildState(placementRect);
      AnimationPanel.SetChildState((DependencyObject) child, state);
      this.BeginChildEnter(child, state);
      newStateCreated = true;
    }
    return state;
  }

  internal AnimationRate GetEffectiveAnimationRate(AnimationPanel.AnimationType animationType)
  {
    AnimationRate effectiveAnimationRate = this._switchParent == null ? this.DefaultAnimationRate : this._switchParent.DefaultAnimationRate;
    switch (animationType)
    {
      case AnimationPanel.AnimationType.Enter:
        if (this.EnterAnimationRate != AnimationRate.Default)
        {
          effectiveAnimationRate = this.EnterAnimationRate;
          break;
        }
        if (this._switchParent != null && this._switchParent.EnterAnimationRate != AnimationRate.Default)
        {
          effectiveAnimationRate = this._switchParent.EnterAnimationRate;
          break;
        }
        break;
      case AnimationPanel.AnimationType.Exit:
        if (this.ExitAnimationRate != AnimationRate.Default)
        {
          effectiveAnimationRate = this.ExitAnimationRate;
          break;
        }
        if (this._switchParent != null && this._switchParent.ExitAnimationRate != AnimationRate.Default)
        {
          effectiveAnimationRate = this._switchParent.ExitAnimationRate;
          break;
        }
        break;
      case AnimationPanel.AnimationType.Layout:
        if (this.LayoutAnimationRate != AnimationRate.Default)
        {
          effectiveAnimationRate = this.LayoutAnimationRate;
          break;
        }
        if (this._switchParent != null && this._switchParent.LayoutAnimationRate != AnimationRate.Default)
        {
          effectiveAnimationRate = this._switchParent.LayoutAnimationRate;
          break;
        }
        break;
      case AnimationPanel.AnimationType.Switch:
        if (this.SwitchAnimationRate != AnimationRate.Default)
        {
          effectiveAnimationRate = this.SwitchAnimationRate;
          break;
        }
        if (this._switchParent != null && this._switchParent.SwitchAnimationRate != AnimationRate.Default)
        {
          effectiveAnimationRate = this._switchParent.SwitchAnimationRate;
          break;
        }
        break;
      case AnimationPanel.AnimationType.Template:
        if (this.TemplateAnimationRate != AnimationRate.Default)
        {
          effectiveAnimationRate = this.TemplateAnimationRate;
          break;
        }
        if (this._switchParent != null && this._switchParent.TemplateAnimationRate != AnimationRate.Default)
        {
          effectiveAnimationRate = this._switchParent.TemplateAnimationRate;
          break;
        }
        break;
    }
    return effectiveAnimationRate;
  }

  private IterativeAnimator GetEffectiveAnimator(AnimationPanel.AnimationType animationType)
  {
    IterativeAnimator effectiveAnimator = this._switchParent == null ? this.DefaultAnimator : this._switchParent.DefaultAnimator;
    switch (animationType)
    {
      case AnimationPanel.AnimationType.Enter:
        if (this.EnterAnimator != IterativeAnimator.Default || this._switchParent != null && this._switchParent.EnterAnimator != IterativeAnimator.Default)
        {
          effectiveAnimator = this.EnterAnimator == IterativeAnimator.Default ? this._switchParent.EnterAnimator : this.EnterAnimator;
          break;
        }
        break;
      case AnimationPanel.AnimationType.Exit:
        if (this.ExitAnimator != IterativeAnimator.Default || this._switchParent != null && this._switchParent.ExitAnimator != IterativeAnimator.Default)
        {
          effectiveAnimator = this.ExitAnimator == IterativeAnimator.Default ? this._switchParent.ExitAnimator : this.ExitAnimator;
          break;
        }
        break;
      case AnimationPanel.AnimationType.Layout:
        if (this.LayoutAnimator != IterativeAnimator.Default || this._switchParent != null && this._switchParent.LayoutAnimator != IterativeAnimator.Default)
        {
          effectiveAnimator = this.LayoutAnimator == IterativeAnimator.Default ? this._switchParent.LayoutAnimator : this.LayoutAnimator;
          break;
        }
        break;
      case AnimationPanel.AnimationType.Switch:
        if (this._switchParent != null && !this._switchParent.AreLayoutSwitchesAnimated)
        {
          effectiveAnimator = (IterativeAnimator) null;
          break;
        }
        if (this.SwitchAnimator != IterativeAnimator.Default || this._switchParent.SwitchAnimator != IterativeAnimator.Default)
        {
          effectiveAnimator = this.SwitchAnimator == IterativeAnimator.Default ? this._switchParent.SwitchAnimator : this.SwitchAnimator;
          break;
        }
        break;
      case AnimationPanel.AnimationType.Template:
        if (this.TemplateAnimator != IterativeAnimator.Default || this._switchParent != null && this._switchParent.TemplateAnimator != IterativeAnimator.Default)
        {
          effectiveAnimator = this.TemplateAnimator == IterativeAnimator.Default ? this._switchParent.TemplateAnimator : this.TemplateAnimator;
          break;
        }
        break;
    }
    return effectiveAnimator;
  }

  private void OnLoaded(object sender, RoutedEventArgs e)
  {
    this.HasLoaded = true;
    this.InvalidateArrange();
  }

  private void OnRendering(object sender, EventArgs e)
  {
    if (!this.IsActiveLayout)
      return;
    if (this._currentChildren != null)
    {
      foreach (UIElement currentChild in this._currentChildren)
      {
        if (currentChild != null)
        {
          AnimationPanel.ChildState childState = AnimationPanel.GetChildState((DependencyObject) currentChild);
          if (childState != null)
          {
            TimeSpan currentTime = DateTime.Now.Subtract(childState.BeginTimeStamp);
            if (childState.IsAnimating)
            {
              bool isDone;
              childState.CurrentPlacement = childState.Animator.GetNextChildPlacement(currentChild, currentTime, childState.CurrentPlacement, childState.TargetPlacement, this, childState.AnimationRate, ref childState.PlacementArgs, out isDone);
              childState.IsAnimating = !isDone;
              this.UpdateTrueArrange(currentChild, childState);
              if (!childState.IsAnimating)
                --this.AnimatingChildCount;
            }
          }
        }
      }
    }
    foreach (FrameworkElement animatingGrandchild in this._animatingGrandchildren)
    {
      AnimationPanel.ChildState childState = AnimationPanel.GetChildState((DependencyObject) animatingGrandchild);
      if (childState != null && childState.IsAnimating)
      {
        TimeSpan currentTime = DateTime.Now.Subtract(childState.BeginTimeStamp);
        bool isDone;
        childState.CurrentPlacement = childState.Animator.GetNextChildPlacement((UIElement) animatingGrandchild, currentTime, childState.CurrentPlacement, childState.TargetPlacement, this, childState.AnimationRate, ref childState.PlacementArgs, out isDone);
        childState.IsAnimating = !isDone;
        Rect finalRect = childState.IsAnimating ? childState.CurrentPlacement : childState.TargetPlacement;
        animatingGrandchild.Arrange(finalRect);
        if (!childState.IsAnimating)
          --this.AnimatingChildCount;
      }
    }
    this.UpdateExitingChildren();
    if (this.AnimatingChildCount != 0)
      return;
    this._animatingGrandchildren.Clear();
  }

  private void UpdateExitingChildren()
  {
    if (this.ExitingChildren.Count <= 0)
      return;
    foreach (UIElement uiElement in new List<UIElement>((IEnumerable<UIElement>) this.ExitingChildren))
    {
      if (uiElement != null)
      {
        AnimationPanel.ChildState childState = AnimationPanel.GetChildState((DependencyObject) uiElement);
        if (childState != null)
        {
          TimeSpan currentTime = DateTime.Now.Subtract(childState.BeginTimeStamp);
          if (childState.IsAnimating)
          {
            bool isDone;
            childState.CurrentPlacement = childState.Animator.GetNextChildPlacement(uiElement, currentTime, childState.CurrentPlacement, childState.TargetPlacement, this, childState.AnimationRate, ref childState.PlacementArgs, out isDone);
            childState.IsAnimating = !isDone;
            this.UpdateTrueArrange(uiElement, childState);
            if (!childState.IsAnimating)
              --this.AnimatingChildCount;
          }
        }
      }
    }
  }

  private void UpdateTrueArrange(UIElement child, AnimationPanel.ChildState state)
  {
    if (!state.TargetPlacement.IsEmpty)
      child.Arrange(!state.IsAnimating || state.Animator == null ? state.TargetPlacement : state.CurrentPlacement);
    if (!state.IsAnimating && !state.HasEnterCompleted)
      this.EndChildEnter(child, state);
    if (state.IsAnimating || !state.HasExitBegun)
      return;
    this.EndChildExit(child, state);
  }

  private sealed class ChildState
  {
    public AnimationPanel.AnimationType Type;
    public DateTime BeginTimeStamp;
    public IterativeAnimator Animator;
    public Rect CurrentPlacement;
    public Rect TargetPlacement;
    public AnimationRate AnimationRate;
    public object PlacementArgs;
    private BitVector32 _cacheBits = new BitVector32(0);

    public ChildState(Rect currentRect)
    {
      this.CurrentPlacement = currentRect;
      this.TargetPlacement = currentRect;
      this.BeginTimeStamp = DateTime.Now;
    }

    public bool HasEnterCompleted
    {
      get => this._cacheBits[2];
      set => this._cacheBits[2] = value;
    }

    public bool HasExitBegun
    {
      get => this._cacheBits[4];
      set => this._cacheBits[4] = value;
    }

    public bool HasExitCompleted
    {
      get => this._cacheBits[8];
      set => this._cacheBits[8] = value;
    }

    public bool IsAnimating
    {
      get => this._cacheBits[1];
      set => this._cacheBits[1] = value;
    }

    private enum CacheBits
    {
      IsAnimating = 1,
      HasEnterCompleted = 2,
      HasExitBegun = 4,
      HasExitCompleted = 8,
    }
  }

  internal enum AnimationType
  {
    Enter,
    Exit,
    Layout,
    Switch,
    Template,
  }

  private enum CacheBits
  {
    IsActiveLayout = 1,
    IsSwitchInProgress = 2,
    EndSwitchOnAnimationCompleted = 16, // 0x00000010
    IsRemovingInternalChild = 32, // 0x00000020
    HasLoaded = 64, // 0x00000040
    HasArranged = 128, // 0x00000080
  }
}
