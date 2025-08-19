// Decompiled with JetBrains decompiler
// Type: System.Windows.VisualTransition
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Media.Animation;

#nullable disable
namespace System.Windows;

[ContentProperty("Storyboard")]
public class VisualTransition : DependencyObject
{
  private Duration _generatedDuration = new Duration(new TimeSpan());

  public VisualTransition()
  {
    this.DynamicStoryboardCompleted = true;
    this.ExplicitStoryboardCompleted = true;
  }

  public string From { get; set; }

  public string To { get; set; }

  public Storyboard Storyboard { get; set; }

  [TypeConverter(typeof (DurationConverter))]
  public Duration GeneratedDuration
  {
    get => this._generatedDuration;
    set => this._generatedDuration = value;
  }

  internal bool IsDefault => this.From == null && this.To == null;

  internal bool DynamicStoryboardCompleted { get; set; }

  internal bool ExplicitStoryboardCompleted { get; set; }
}
