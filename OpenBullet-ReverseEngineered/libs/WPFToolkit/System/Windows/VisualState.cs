// Decompiled with JetBrains decompiler
// Type: System.Windows.VisualState
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System.Windows.Markup;
using System.Windows.Media.Animation;

#nullable disable
namespace System.Windows;

[ContentProperty("Storyboard")]
[RuntimeNameProperty("Name")]
public class VisualState : DependencyObject
{
  private static readonly DependencyProperty StoryboardProperty = DependencyProperty.Register(nameof (Storyboard), typeof (Storyboard), typeof (VisualState));

  public string Name { get; set; }

  public Storyboard Storyboard
  {
    get => (Storyboard) this.GetValue(VisualState.StoryboardProperty);
    set => this.SetValue(VisualState.StoryboardProperty, (object) value);
  }
}
