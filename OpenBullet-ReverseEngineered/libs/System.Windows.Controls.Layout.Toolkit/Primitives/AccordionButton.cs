// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.Primitives.AccordionButton
// Assembly: System.Windows.Controls.Layout.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 2878816D-F7B3-441D-96A5-F68332B17866
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Layout.Toolkit.dll

#nullable disable
namespace System.Windows.Controls.Primitives;

[TemplateVisualState(Name = "ExpandDown", GroupName = "ExpandDirectionStates")]
[TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplateVisualState(Name = "ExpandRight", GroupName = "ExpandDirectionStates")]
[TemplateVisualState(Name = "Expanded", GroupName = "ExpansionStates")]
[TemplateVisualState(Name = "Collapsed", GroupName = "ExpansionStates")]
[TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
[TemplateVisualState(Name = "ExpandUp", GroupName = "ExpandDirectionStates")]
[TemplateVisualState(Name = "ExpandLeft", GroupName = "ExpandDirectionStates")]
public class AccordionButton : ToggleButton
{
  internal AccordionItem ParentAccordionItem { get; set; }

  static AccordionButton()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (AccordionButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (AccordionButton)));
  }

  internal new virtual void UpdateVisualState(bool useTransitions)
  {
    if (this.ParentAccordionItem == null)
      return;
    if (this.ParentAccordionItem.IsSelected)
      VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Expanded");
    else
      VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Collapsed");
    switch (this.ParentAccordionItem.ExpandDirection)
    {
      case ExpandDirection.Down:
        VisualStates.GoToState((Control) this, false, "ExpandDown");
        break;
      case ExpandDirection.Up:
        VisualStates.GoToState((Control) this, false, "ExpandUp");
        break;
      case ExpandDirection.Left:
        VisualStates.GoToState((Control) this, false, "ExpandLeft");
        break;
      default:
        VisualStates.GoToState((Control) this, false, "ExpandRight");
        break;
    }
  }
}
