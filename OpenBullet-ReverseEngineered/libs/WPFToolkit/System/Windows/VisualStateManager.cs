// Decompiled with JetBrains decompiler
// Type: System.Windows.VisualStateManager
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using Microsoft.Windows.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

#nullable disable
namespace System.Windows;

public class VisualStateManager : DependencyObject
{
  public static readonly DependencyProperty CustomVisualStateManagerProperty = DependencyProperty.RegisterAttached("CustomVisualStateManager", typeof (VisualStateManager), typeof (VisualStateManager), (PropertyMetadata) null);
  internal static readonly DependencyProperty VisualStateGroupsProperty = DependencyProperty.RegisterAttached("InternalVisualStateGroups", typeof (Collection<VisualStateGroup>), typeof (VisualStateManager), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(VisualStateManager.OnVisualStateGroupsChanged)));
  private static readonly Duration DurationZero = new Duration(TimeSpan.Zero);

  public static bool GoToState(Control control, string stateName, bool useTransitions)
  {
    if (control == null)
      throw new ArgumentNullException(nameof (control));
    if (stateName == null)
      throw new ArgumentNullException(nameof (stateName));
    FrameworkElement templateRoot = VisualStateManager.GetTemplateRoot(control);
    if (templateRoot == null)
      return false;
    IList<VisualStateGroup> stateGroupsInternal = (IList<VisualStateGroup>) VisualStateManager.GetVisualStateGroupsInternal(templateRoot);
    if (stateGroupsInternal == null)
      return false;
    VisualStateGroup group;
    VisualState state;
    VisualStateManager.TryGetState(stateGroupsInternal, stateName, out group, out state);
    VisualStateManager visualStateManager = VisualStateManager.GetCustomVisualStateManager(templateRoot);
    if (visualStateManager != null)
      return visualStateManager.GoToStateCore(control, templateRoot, stateName, group, state, useTransitions);
    return state != null && VisualStateManager.GoToStateInternal(control, templateRoot, group, state, useTransitions);
  }

  protected virtual bool GoToStateCore(
    Control control,
    FrameworkElement templateRoot,
    string stateName,
    VisualStateGroup group,
    VisualState state,
    bool useTransitions)
  {
    return VisualStateManager.GoToStateInternal(control, templateRoot, group, state, useTransitions);
  }

  public static VisualStateManager GetCustomVisualStateManager(FrameworkElement obj)
  {
    return obj != null ? obj.GetValue(VisualStateManager.CustomVisualStateManagerProperty) as VisualStateManager : throw new ArgumentNullException(nameof (obj));
  }

  public static void SetCustomVisualStateManager(FrameworkElement obj, VisualStateManager value)
  {
    if (obj == null)
      throw new ArgumentNullException(nameof (obj));
    obj.SetValue(VisualStateManager.CustomVisualStateManagerProperty, (object) value);
  }

  internal static Collection<VisualStateGroup> GetVisualStateGroupsInternal(FrameworkElement obj)
  {
    if (obj == null)
      throw new ArgumentNullException(nameof (obj));
    if (!(obj.GetValue(VisualStateManager.VisualStateGroupsProperty) is Collection<VisualStateGroup> stateGroupsInternal))
    {
      stateGroupsInternal = new Collection<VisualStateGroup>();
      VisualStateManager.SetVisualStateGroups(obj, stateGroupsInternal);
    }
    return stateGroupsInternal;
  }

  public static IList GetVisualStateGroups(FrameworkElement obj)
  {
    return (IList) VisualStateManager.GetVisualStateGroupsInternal(obj);
  }

  internal static void SetVisualStateGroups(
    FrameworkElement obj,
    Collection<VisualStateGroup> value)
  {
    if (obj == null)
      throw new ArgumentNullException(nameof (obj));
    obj.SetValue(VisualStateManager.VisualStateGroupsProperty, (object) value);
  }

  private static void OnVisualStateGroupsChanged(
    DependencyObject obj,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(obj is FrameworkElement element))
      return;
    Control templatedParent = VisualStateManager.GetTemplatedParent(element);
    if (templatedParent == null)
      return;
    VisualStateBehaviorFactory.AttachBehavior(templatedParent);
  }

  internal static bool TryGetState(
    IList<VisualStateGroup> groups,
    string stateName,
    out VisualStateGroup group,
    out VisualState state)
  {
    for (int index = 0; index < groups.Count; ++index)
    {
      VisualStateGroup group1 = groups[index];
      VisualState state1 = group1.GetState(stateName);
      if (state1 != null)
      {
        group = group1;
        state = state1;
        return true;
      }
    }
    group = (VisualStateGroup) null;
    state = (VisualState) null;
    return false;
  }

  private static bool GoToStateInternal(
    Control control,
    FrameworkElement element,
    VisualStateGroup group,
    VisualState state,
    bool useTransitions)
  {
    if (element == null)
      throw new ArgumentNullException(nameof (element));
    if (state == null)
      throw new ArgumentNullException(nameof (state));
    VisualState lastState = group != null ? group.CurrentState : throw new InvalidOperationException();
    if (lastState == state)
      return true;
    VisualTransition transition = useTransitions ? VisualStateManager.GetTransition(element, group, lastState, state) : (VisualTransition) null;
    Storyboard transitionAnimations = VisualStateManager.GenerateDynamicTransitionAnimations(element, group, state, transition);
    if (transition == null || transition.GeneratedDuration == VisualStateManager.DurationZero && (transition.Storyboard == null || transition.Storyboard.Duration == VisualStateManager.DurationZero))
    {
      if (transition != null && transition.Storyboard != null)
        group.StartNewThenStopOld(element, transition.Storyboard, state.Storyboard);
      else
        group.StartNewThenStopOld(element, state.Storyboard);
      group.RaiseCurrentStateChanging(element, lastState, state, control);
      group.RaiseCurrentStateChanged(element, lastState, state, control);
    }
    else
    {
      transition.DynamicStoryboardCompleted = false;
      transitionAnimations.Completed += (EventHandler) ((sender, e) =>
      {
        if (transition.Storyboard == null || transition.ExplicitStoryboardCompleted)
        {
          if (VisualStateManager.ShouldRunStateStoryboard((FrameworkElement) control, element, state, group))
            group.StartNewThenStopOld(element, state.Storyboard);
          group.RaiseCurrentStateChanged(element, lastState, state, control);
        }
        transition.DynamicStoryboardCompleted = true;
      });
      if (transition.Storyboard != null && transition.ExplicitStoryboardCompleted)
      {
        EventHandler transitionCompleted = (EventHandler) null;
        transitionCompleted = (EventHandler) ((sender, e) =>
        {
          if (transition.DynamicStoryboardCompleted)
          {
            if (VisualStateManager.ShouldRunStateStoryboard((FrameworkElement) control, element, state, group))
              group.StartNewThenStopOld(element, state.Storyboard);
            group.RaiseCurrentStateChanged(element, lastState, state, control);
          }
          transition.Storyboard.Completed -= transitionCompleted;
          transition.ExplicitStoryboardCompleted = true;
        });
        transition.ExplicitStoryboardCompleted = false;
        transition.Storyboard.Completed += transitionCompleted;
      }
      group.StartNewThenStopOld(element, transition.Storyboard, transitionAnimations);
      group.RaiseCurrentStateChanging(element, lastState, state, control);
    }
    group.CurrentState = state;
    return true;
  }

  private static bool ShouldRunStateStoryboard(
    FrameworkElement control,
    FrameworkElement stateGroupsRoot,
    VisualState state,
    VisualStateGroup group)
  {
    bool flag1 = true;
    bool flag2 = true;
    if (control != null && !control.IsVisible)
      flag1 = PresentationSource.FromVisual((Visual) control) != null;
    if (stateGroupsRoot != null && !stateGroupsRoot.IsVisible)
      flag2 = PresentationSource.FromVisual((Visual) stateGroupsRoot) != null;
    return flag1 && flag2 && state == group.CurrentState;
  }

  protected void RaiseCurrentStateChanging(
    VisualStateGroup stateGroup,
    VisualState oldState,
    VisualState newState,
    Control control)
  {
    if (stateGroup == null)
      throw new ArgumentNullException(nameof (stateGroup));
    if (newState == null)
      throw new ArgumentNullException(nameof (newState));
    FrameworkElement element = control != null ? VisualStateManager.GetTemplateRoot(control) : throw new ArgumentNullException(nameof (control));
    if (element == null)
      return;
    stateGroup.RaiseCurrentStateChanging(element, oldState, newState, control);
  }

  protected void RaiseCurrentStateChanged(
    VisualStateGroup stateGroup,
    VisualState oldState,
    VisualState newState,
    Control control)
  {
    if (stateGroup == null)
      throw new ArgumentNullException(nameof (stateGroup));
    if (newState == null)
      throw new ArgumentNullException(nameof (newState));
    FrameworkElement element = control != null ? VisualStateManager.GetTemplateRoot(control) : throw new ArgumentNullException(nameof (control));
    if (element == null)
      return;
    stateGroup.RaiseCurrentStateChanged(element, oldState, newState, control);
  }

  private static Storyboard GenerateDynamicTransitionAnimations(
    FrameworkElement root,
    VisualStateGroup group,
    VisualState newState,
    VisualTransition transition)
  {
    Storyboard transitionAnimations = new Storyboard();
    if (transition != null)
    {
      Duration generatedDuration = transition.GeneratedDuration;
      transitionAnimations.Duration = transition.GeneratedDuration;
    }
    else
      transitionAnimations.Duration = new Duration(TimeSpan.Zero);
    Dictionary<VisualStateManager.TimelineDataToken, Timeline> dictionary1 = VisualStateManager.FlattenTimelines(group.CurrentStoryboards);
    Dictionary<VisualStateManager.TimelineDataToken, Timeline> dictionary2 = VisualStateManager.FlattenTimelines(transition?.Storyboard);
    Dictionary<VisualStateManager.TimelineDataToken, Timeline> dictionary3 = VisualStateManager.FlattenTimelines(newState.Storyboard);
    foreach (KeyValuePair<VisualStateManager.TimelineDataToken, Timeline> keyValuePair in dictionary2)
    {
      dictionary1.Remove(keyValuePair.Key);
      dictionary3.Remove(keyValuePair.Key);
    }
    foreach (KeyValuePair<VisualStateManager.TimelineDataToken, Timeline> keyValuePair in dictionary3)
    {
      Timeline toAnimation = VisualStateManager.GenerateToAnimation(root, keyValuePair.Value, true);
      if (toAnimation != null)
      {
        toAnimation.Duration = transitionAnimations.Duration;
        transitionAnimations.Children.Add(toAnimation);
      }
      dictionary1.Remove(keyValuePair.Key);
    }
    foreach (KeyValuePair<VisualStateManager.TimelineDataToken, Timeline> keyValuePair in dictionary1)
    {
      Timeline fromAnimation = VisualStateManager.GenerateFromAnimation(root, keyValuePair.Value);
      if (fromAnimation != null)
      {
        fromAnimation.Duration = transitionAnimations.Duration;
        transitionAnimations.Children.Add(fromAnimation);
      }
    }
    return transitionAnimations;
  }

  private static Timeline GenerateFromAnimation(FrameworkElement root, Timeline timeline)
  {
    Timeline destination = (Timeline) null;
    switch (timeline)
    {
      case ColorAnimation _:
      case ColorAnimationUsingKeyFrames _:
        destination = (Timeline) new ColorAnimation();
        break;
      case DoubleAnimation _:
      case DoubleAnimationUsingKeyFrames _:
        destination = (Timeline) new DoubleAnimation();
        break;
      case PointAnimation _:
      case PointAnimationUsingKeyFrames _:
        destination = (Timeline) new PointAnimation();
        break;
    }
    if (destination != null)
      VisualStateManager.CopyStoryboardTargetProperties(root, timeline, destination);
    return destination;
  }

  private static Timeline GenerateToAnimation(
    FrameworkElement root,
    Timeline timeline,
    bool isEntering)
  {
    Timeline destination = (Timeline) null;
    Color? targetColor = VisualStateManager.GetTargetColor(timeline, isEntering);
    if (targetColor.HasValue)
      destination = (Timeline) new ColorAnimation()
      {
        To = targetColor
      };
    if (destination == null)
    {
      double? targetDouble = VisualStateManager.GetTargetDouble(timeline, isEntering);
      if (targetDouble.HasValue)
        destination = (Timeline) new DoubleAnimation()
        {
          To = targetDouble
        };
    }
    if (destination == null)
    {
      Point? targetPoint = VisualStateManager.GetTargetPoint(timeline, isEntering);
      if (targetPoint.HasValue)
        destination = (Timeline) new PointAnimation()
        {
          To = targetPoint
        };
    }
    if (destination != null)
      VisualStateManager.CopyStoryboardTargetProperties(root, timeline, destination);
    return destination;
  }

  private static void CopyStoryboardTargetProperties(
    FrameworkElement root,
    Timeline source,
    Timeline destination)
  {
    if (source == null && destination == null)
      return;
    string targetName = Storyboard.GetTargetName((DependencyObject) source);
    DependencyObject dependencyObject = Storyboard.GetTarget((DependencyObject) source);
    PropertyPath targetProperty = Storyboard.GetTargetProperty((DependencyObject) source);
    if (dependencyObject == null && !string.IsNullOrEmpty(targetName))
      dependencyObject = root.FindName(targetName) as DependencyObject;
    if (targetName != null)
      Storyboard.SetTargetName((DependencyObject) destination, targetName);
    if (dependencyObject != null)
      Storyboard.SetTarget((DependencyObject) destination, dependencyObject);
    if (targetProperty == null)
      return;
    Storyboard.SetTargetProperty((DependencyObject) destination, targetProperty);
  }

  internal static VisualTransition GetTransition(
    FrameworkElement element,
    VisualStateGroup group,
    VisualState from,
    VisualState to)
  {
    if (element == null)
      throw new ArgumentNullException(nameof (element));
    if (group == null)
      throw new ArgumentNullException(nameof (group));
    if (to == null)
      throw new ArgumentNullException(nameof (to));
    VisualTransition visualTransition1 = (VisualTransition) null;
    VisualTransition visualTransition2 = (VisualTransition) null;
    int num1 = -1;
    IList<VisualTransition> transitions = (IList<VisualTransition>) group.Transitions;
    if (transitions != null)
    {
      foreach (VisualTransition visualTransition3 in (IEnumerable<VisualTransition>) transitions)
      {
        if (visualTransition2 == null && visualTransition3.IsDefault)
        {
          visualTransition2 = visualTransition3;
        }
        else
        {
          int num2 = -1;
          VisualState state1 = group.GetState(visualTransition3.From);
          VisualState state2 = group.GetState(visualTransition3.To);
          if (from == state1)
            ++num2;
          else if (state1 != null)
            continue;
          if (to == state2)
            num2 += 2;
          else if (state2 != null)
            continue;
          if (num2 > num1)
          {
            num1 = num2;
            visualTransition1 = visualTransition3;
          }
        }
      }
    }
    return visualTransition1 ?? visualTransition2;
  }

  private static Color? GetTargetColor(Timeline timeline, bool isEntering)
  {
    switch (timeline)
    {
      case ColorAnimation colorAnimation:
        return !colorAnimation.From.HasValue ? colorAnimation.To : colorAnimation.From;
      case ColorAnimationUsingKeyFrames animationUsingKeyFrames:
        return animationUsingKeyFrames.KeyFrames.Count == 0 ? new Color?() : new Color?(animationUsingKeyFrames.KeyFrames[isEntering ? 0 : animationUsingKeyFrames.KeyFrames.Count - 1].Value);
      default:
        return new Color?();
    }
  }

  private static double? GetTargetDouble(Timeline timeline, bool isEntering)
  {
    switch (timeline)
    {
      case DoubleAnimation doubleAnimation:
        return !doubleAnimation.From.HasValue ? doubleAnimation.To : doubleAnimation.From;
      case DoubleAnimationUsingKeyFrames animationUsingKeyFrames:
        return animationUsingKeyFrames.KeyFrames.Count == 0 ? new double?() : new double?(animationUsingKeyFrames.KeyFrames[isEntering ? 0 : animationUsingKeyFrames.KeyFrames.Count - 1].Value);
      default:
        return new double?();
    }
  }

  private static Point? GetTargetPoint(Timeline timeline, bool isEntering)
  {
    switch (timeline)
    {
      case PointAnimation pointAnimation:
        return !pointAnimation.From.HasValue ? pointAnimation.To : pointAnimation.From;
      case PointAnimationUsingKeyFrames animationUsingKeyFrames:
        return animationUsingKeyFrames.KeyFrames.Count == 0 ? new Point?() : new Point?(animationUsingKeyFrames.KeyFrames[isEntering ? 0 : animationUsingKeyFrames.KeyFrames.Count - 1].Value);
      default:
        return new Point?();
    }
  }

  private static Dictionary<VisualStateManager.TimelineDataToken, Timeline> FlattenTimelines(
    Storyboard storyboard)
  {
    Dictionary<VisualStateManager.TimelineDataToken, Timeline> result = new Dictionary<VisualStateManager.TimelineDataToken, Timeline>();
    VisualStateManager.FlattenTimelines(storyboard, result);
    return result;
  }

  private static Dictionary<VisualStateManager.TimelineDataToken, Timeline> FlattenTimelines(
    Collection<Storyboard> storyboards)
  {
    Dictionary<VisualStateManager.TimelineDataToken, Timeline> result = new Dictionary<VisualStateManager.TimelineDataToken, Timeline>();
    for (int index = 0; index < storyboards.Count; ++index)
      VisualStateManager.FlattenTimelines(storyboards[index], result);
    return result;
  }

  private static void FlattenTimelines(
    Storyboard storyboard,
    Dictionary<VisualStateManager.TimelineDataToken, Timeline> result)
  {
    if (storyboard == null)
      return;
    for (int index = 0; index < storyboard.Children.Count; ++index)
    {
      Timeline child = storyboard.Children[index];
      if (child is Storyboard storyboard1)
        VisualStateManager.FlattenTimelines(storyboard1, result);
      else
        result[new VisualStateManager.TimelineDataToken(child)] = child;
    }
  }

  private static FrameworkElement GetTemplateRoot(Control control)
  {
    if (control is UserControl userControl)
      return userControl.Content as FrameworkElement;
    return VisualTreeHelper.GetChildrenCount((DependencyObject) control) > 0 ? VisualTreeHelper.GetChild((DependencyObject) control, 0) as FrameworkElement : (FrameworkElement) null;
  }

  private static Control GetTemplatedParent(FrameworkElement element)
  {
    DependencyObject templatedParent = element.TemplatedParent;
    if (templatedParent != null)
      return templatedParent as Control;
    return element.Parent is UserControl parent ? (Control) parent : (Control) null;
  }

  private struct TimelineDataToken(Timeline timeline) : 
    IEquatable<VisualStateManager.TimelineDataToken>
  {
    private DependencyObject _target = Storyboard.GetTarget((DependencyObject) timeline);
    private string _targetName = Storyboard.GetTargetName((DependencyObject) timeline);
    private PropertyPath _targetProperty = Storyboard.GetTargetProperty((DependencyObject) timeline);

    public bool Equals(VisualStateManager.TimelineDataToken other)
    {
      if (other._target != this._target || !(other._targetName == this._targetName) || !(other._targetProperty.Path == this._targetProperty.Path) || other._targetProperty.PathParameters.Count != this._targetProperty.PathParameters.Count)
        return false;
      bool flag = true;
      int index = 0;
      for (int count = this._targetProperty.PathParameters.Count; index < count; ++index)
      {
        if (other._targetProperty.PathParameters[index] != this._targetProperty.PathParameters[index])
        {
          flag = false;
          break;
        }
      }
      return flag;
    }

    public override int GetHashCode()
    {
      return (this._target != null ? this._target.GetHashCode() : 0) ^ (this._targetName != null ? this._targetName.GetHashCode() : 0) ^ (this._targetProperty != null ? this._targetProperty.GetHashCode() : 0);
    }
  }
}
