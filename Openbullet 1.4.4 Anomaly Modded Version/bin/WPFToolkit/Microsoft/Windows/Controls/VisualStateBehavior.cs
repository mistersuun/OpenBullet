// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.VisualStateBehavior
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Windows.Controls;

public abstract class VisualStateBehavior
{
  public static readonly DependencyProperty VisualStateBehaviorProperty = DependencyProperty.RegisterAttached(nameof (VisualStateBehavior), typeof (VisualStateBehavior), typeof (VisualStateBehavior), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(VisualStateBehavior.OnVisualStateBehaviorChanged)));
  private static readonly DependencyProperty IsVisualStateBehaviorAttachedProperty = DependencyProperty.RegisterAttached("IsVisualStateBehaviorAttached", typeof (bool), typeof (VisualStateBehavior), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));

  public static VisualStateBehavior GetVisualStateBehavior(DependencyObject obj)
  {
    return (VisualStateBehavior) obj.GetValue(VisualStateBehavior.VisualStateBehaviorProperty);
  }

  public static void SetVisualStateBehavior(DependencyObject obj, VisualStateBehavior value)
  {
    obj.SetValue(VisualStateBehavior.VisualStateBehaviorProperty, (object) value);
  }

  private static void OnVisualStateBehaviorChanged(
    object sender,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(sender is Control control))
      return;
    ((VisualStateBehavior) e.NewValue)?.Attach(control);
  }

  private void Attach(Control control)
  {
    if (VisualStateBehavior.GetIsVisualStateBehaviorAttached((DependencyObject) control))
      throw new InvalidOperationException("VisualStateBehavior is already attached.");
    VisualStateBehavior.SetIsVisualStateBehaviorAttached((DependencyObject) control, true);
    this.OnAttach(control);
    control.Unloaded += new RoutedEventHandler(this.DetachHandler);
    control.Loaded -= new RoutedEventHandler(this.AttachHandler);
  }

  private void Detach(Control control)
  {
    if (!VisualStateBehavior.GetIsVisualStateBehaviorAttached((DependencyObject) control))
      throw new InvalidOperationException("VisualStateBehavior is not attached.");
    VisualStateBehavior.SetIsVisualStateBehaviorAttached((DependencyObject) control, false);
    this.OnDetach(control);
    control.Loaded += new RoutedEventHandler(this.AttachHandler);
    control.Unloaded -= new RoutedEventHandler(this.DetachHandler);
  }

  private static bool GetIsVisualStateBehaviorAttached(DependencyObject obj)
  {
    return (bool) obj.GetValue(VisualStateBehavior.IsVisualStateBehaviorAttachedProperty);
  }

  private static void SetIsVisualStateBehaviorAttached(DependencyObject obj, bool value)
  {
    obj.SetValue(VisualStateBehavior.IsVisualStateBehaviorAttachedProperty, (object) value);
  }

  public static void RegisterBehavior(VisualStateBehavior behavior)
  {
    VisualStateBehaviorFactory.RegisterControlBehavior(behavior);
  }

  protected internal abstract Type TargetType { get; }

  protected abstract void OnAttach(Control control);

  protected abstract void OnDetach(Control control);

  protected abstract void UpdateStateHandler(object o, EventArgs e);

  private void DetachHandler(object sender, RoutedEventArgs e)
  {
    if (!(sender is Control control))
      throw new InvalidOperationException("This Handler should only be on a control.");
    this.Detach(control);
  }

  private void AttachHandler(object sender, RoutedEventArgs e)
  {
    if (!(sender is Control control))
      throw new InvalidOperationException("This Handler should only be on a control.");
    this.Attach(control);
  }

  protected abstract void UpdateState(Control control, bool useTransitions);

  protected static bool AddValueChanged(
    DependencyProperty dp,
    Type targetType,
    object instance,
    EventHandler handler)
  {
    if (dp == null)
      throw new ArgumentNullException(nameof (dp));
    if ((object) targetType == null)
      throw new ArgumentNullException(nameof (targetType));
    if (instance == null)
      throw new ArgumentNullException(nameof (instance));
    if (handler == null)
      throw new ArgumentNullException(nameof (handler));
    DependencyPropertyDescriptor propertyDescriptor = DependencyPropertyDescriptor.FromProperty(dp, targetType);
    if (propertyDescriptor == null)
      return false;
    propertyDescriptor.AddValueChanged(instance, handler);
    return true;
  }

  protected static bool RemoveValueChanged(
    DependencyProperty dp,
    Type targetType,
    object instance,
    EventHandler handler)
  {
    if (dp == null)
      throw new ArgumentNullException(nameof (dp));
    if ((object) targetType == null)
      throw new ArgumentNullException(nameof (targetType));
    if (instance == null)
      throw new ArgumentNullException(nameof (instance));
    if (handler == null)
      throw new ArgumentNullException(nameof (handler));
    DependencyPropertyDescriptor propertyDescriptor = DependencyPropertyDescriptor.FromProperty(dp, targetType);
    if (propertyDescriptor == null)
      return false;
    propertyDescriptor.RemoveValueChanged(instance, handler);
    return true;
  }
}
