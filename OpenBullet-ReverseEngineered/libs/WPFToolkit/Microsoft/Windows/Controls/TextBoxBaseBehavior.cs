// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.TextBoxBaseBehavior
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

#nullable disable
namespace Microsoft.Windows.Controls;

public class TextBoxBaseBehavior : ControlBehavior
{
  protected internal override Type TargetType => typeof (TextBoxBase);

  protected override void OnAttach(Control control)
  {
    base.OnAttach(control);
    TextBoxBase instance = (TextBoxBase) control;
    Type targetType = typeof (TextBoxBase);
    VisualStateBehavior.AddValueChanged(UIElement.IsMouseOverProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
    VisualStateBehavior.AddValueChanged(UIElement.IsEnabledProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
    VisualStateBehavior.AddValueChanged(TextBoxBase.IsReadOnlyProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
  }

  protected override void OnDetach(Control control)
  {
    base.OnDetach(control);
    TextBoxBase instance = (TextBoxBase) control;
    Type targetType = typeof (TextBoxBase);
    VisualStateBehavior.RemoveValueChanged(UIElement.IsMouseOverProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
    VisualStateBehavior.RemoveValueChanged(UIElement.IsEnabledProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
    VisualStateBehavior.RemoveValueChanged(TextBoxBase.IsReadOnlyProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
  }

  protected override void UpdateState(Control control, bool useTransitions)
  {
    TextBoxBase textBoxBase = (TextBoxBase) control;
    if (!textBoxBase.IsEnabled)
      VisualStateManager.GoToState((Control) textBoxBase, "Disabled", useTransitions);
    else if (textBoxBase.IsReadOnly)
      VisualStateManager.GoToState((Control) textBoxBase, "ReadOnly", useTransitions);
    else if (textBoxBase.IsMouseOver)
      VisualStateManager.GoToState((Control) textBoxBase, "MouseOver", useTransitions);
    else
      VisualStateManager.GoToState((Control) textBoxBase, "Normal", useTransitions);
    base.UpdateState(control, useTransitions);
  }
}
