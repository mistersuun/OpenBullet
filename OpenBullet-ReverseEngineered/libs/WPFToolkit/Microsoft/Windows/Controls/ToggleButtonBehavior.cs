// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.ToggleButtonBehavior
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

#nullable disable
namespace Microsoft.Windows.Controls;

public class ToggleButtonBehavior : ButtonBaseBehavior
{
  protected internal override Type TargetType => typeof (ToggleButton);

  protected override void OnAttach(Control control)
  {
    base.OnAttach(control);
    ToggleButton instance = (ToggleButton) control;
    Type targetType = typeof (ToggleButton);
    VisualStateBehavior.AddValueChanged(ToggleButton.IsCheckedProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
  }

  protected override void OnDetach(Control control)
  {
    base.OnDetach(control);
    ToggleButton instance = (ToggleButton) control;
    Type targetType = typeof (ToggleButton);
    VisualStateBehavior.RemoveValueChanged(ToggleButton.IsCheckedProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
  }

  protected override void UpdateState(Control control, bool useTransitions)
  {
    ToggleButton toggleButton = (ToggleButton) control;
    if (!toggleButton.IsChecked.HasValue)
      VisualStateManager.GoToState((Control) toggleButton, "Indeterminate", useTransitions);
    else if (toggleButton.IsChecked.Value)
      VisualStateManager.GoToState((Control) toggleButton, "Checked", useTransitions);
    else
      VisualStateManager.GoToState((Control) toggleButton, "Unchecked", useTransitions);
    base.UpdateState(control, useTransitions);
  }
}
