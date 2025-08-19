// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.ControlBehavior
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Windows.Controls;

public class ControlBehavior : VisualStateBehavior
{
  protected internal override Type TargetType => typeof (Control);

  protected override void OnAttach(Control control)
  {
    control.Loaded += (RoutedEventHandler) ((sender, e) => this.UpdateState(control, false));
    VisualStateBehavior.AddValueChanged(UIElement.IsKeyboardFocusWithinProperty, typeof (Control), (object) control, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
  }

  protected override void OnDetach(Control control)
  {
    VisualStateBehavior.RemoveValueChanged(UIElement.IsKeyboardFocusWithinProperty, typeof (Control), (object) control, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
  }

  protected override void UpdateStateHandler(object o, EventArgs e)
  {
    if (!(o is Control control))
      throw new InvalidOperationException("This should never be used on anything other than a control.");
    this.UpdateState(control, true);
  }

  protected override void UpdateState(Control control, bool useTransitions)
  {
    if (control.IsKeyboardFocusWithin)
      VisualStateManager.GoToState(control, "Focused", useTransitions);
    else
      VisualStateManager.GoToState(control, "Unfocused", useTransitions);
  }
}
