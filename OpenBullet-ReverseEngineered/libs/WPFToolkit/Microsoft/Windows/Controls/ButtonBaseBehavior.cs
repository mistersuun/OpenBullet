// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.ButtonBaseBehavior
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

#nullable disable
namespace Microsoft.Windows.Controls;

public class ButtonBaseBehavior : ControlBehavior
{
  protected internal override Type TargetType => typeof (ButtonBase);

  protected override void OnAttach(Control control)
  {
    base.OnAttach(control);
    ButtonBase instance = (ButtonBase) control;
    Type targetType = typeof (ButtonBase);
    VisualStateBehavior.AddValueChanged(UIElement.IsMouseOverProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
    VisualStateBehavior.AddValueChanged(UIElement.IsEnabledProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
    VisualStateBehavior.AddValueChanged(ButtonBase.IsPressedProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
  }

  protected override void OnDetach(Control control)
  {
    base.OnDetach(control);
    ButtonBase instance = (ButtonBase) control;
    Type targetType = typeof (ButtonBase);
    VisualStateBehavior.RemoveValueChanged(UIElement.IsMouseOverProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
    VisualStateBehavior.RemoveValueChanged(UIElement.IsEnabledProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
    VisualStateBehavior.RemoveValueChanged(ButtonBase.IsPressedProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
  }

  protected override void UpdateState(Control control, bool useTransitions)
  {
    ButtonBase buttonBase = (ButtonBase) control;
    if (!buttonBase.IsEnabled)
      VisualStateManager.GoToState((Control) buttonBase, "Disabled", useTransitions);
    else if (buttonBase.IsPressed)
      VisualStateManager.GoToState((Control) buttonBase, "Pressed", useTransitions);
    else if (buttonBase.IsMouseOver)
      VisualStateManager.GoToState((Control) buttonBase, "MouseOver", useTransitions);
    else
      VisualStateManager.GoToState((Control) buttonBase, "Normal", useTransitions);
    base.UpdateState(control, useTransitions);
  }
}
