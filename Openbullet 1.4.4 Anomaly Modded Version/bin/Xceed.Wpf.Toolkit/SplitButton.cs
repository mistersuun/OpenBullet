// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.SplitButton
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using System.Windows.Controls.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_ActionButton", Type = typeof (System.Windows.Controls.Button))]
public class SplitButton : DropDownButton
{
  private const string PART_ActionButton = "PART_ActionButton";

  static SplitButton()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (SplitButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (SplitButton)));
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this.Button = (ButtonBase) (this.GetTemplateChild("PART_ActionButton") as System.Windows.Controls.Button);
  }
}
