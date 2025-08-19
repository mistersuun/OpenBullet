// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.ProgressBarBehavior
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Windows.Controls;

public class ProgressBarBehavior : ControlBehavior
{
  protected internal override Type TargetType => typeof (ProgressBar);

  protected override void OnAttach(Control control)
  {
    base.OnAttach(control);
    ProgressBar instance = (ProgressBar) control;
    Type targetType = typeof (ProgressBar);
    VisualStateBehavior.AddValueChanged(ProgressBar.IsIndeterminateProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
  }

  protected override void OnDetach(Control control)
  {
    base.OnDetach(control);
    ProgressBar instance = (ProgressBar) control;
    Type targetType = typeof (ProgressBar);
    VisualStateBehavior.RemoveValueChanged(ProgressBar.IsIndeterminateProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
  }

  protected override void UpdateState(Control control, bool useTransitions)
  {
    ProgressBar progressBar = (ProgressBar) control;
    if (!progressBar.IsIndeterminate)
      VisualStateManager.GoToState((Control) progressBar, "Determinate", useTransitions);
    else
      VisualStateManager.GoToState((Control) progressBar, "Indeterminate", useTransitions);
    base.UpdateState(control, useTransitions);
  }
}
