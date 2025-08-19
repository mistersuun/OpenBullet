// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.ListBoxItemBehavior
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Windows.Controls;

public class ListBoxItemBehavior : ControlBehavior
{
  protected internal override Type TargetType => typeof (ListBoxItem);

  protected override void OnAttach(Control control)
  {
    base.OnAttach(control);
    ListBoxItem instance = (ListBoxItem) control;
    Type targetType = typeof (ListBoxItem);
    VisualStateBehavior.AddValueChanged(UIElement.IsMouseOverProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
    VisualStateBehavior.AddValueChanged(ListBoxItem.IsSelectedProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
  }

  protected override void OnDetach(Control control)
  {
    base.OnDetach(control);
    ListBoxItem instance = (ListBoxItem) control;
    Type targetType = typeof (ListBoxItem);
    VisualStateBehavior.RemoveValueChanged(UIElement.IsMouseOverProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
    VisualStateBehavior.RemoveValueChanged(ListBoxItem.IsSelectedProperty, targetType, (object) instance, new EventHandler(((VisualStateBehavior) this).UpdateStateHandler));
  }

  protected override void UpdateState(Control control, bool useTransitions)
  {
    ListBoxItem listBoxItem = (ListBoxItem) control;
    if (listBoxItem.IsMouseOver)
      VisualStateManager.GoToState((Control) listBoxItem, "MouseOver", useTransitions);
    else
      VisualStateManager.GoToState((Control) listBoxItem, "Normal", useTransitions);
    if (listBoxItem.IsSelected)
      VisualStateManager.GoToState((Control) listBoxItem, "Selected", useTransitions);
    else
      VisualStateManager.GoToState((Control) listBoxItem, "Unselected", useTransitions);
    base.UpdateState(control, useTransitions);
  }
}
