// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.InlineObjectRun
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public class InlineObjectRun : TextEmbeddedObject
{
  private UIElement element;
  private int length;
  private TextRunProperties properties;
  internal Size desiredSize;

  public InlineObjectRun(int length, TextRunProperties properties, UIElement element)
  {
    if (length <= 0)
      throw new ArgumentOutOfRangeException(nameof (length), (object) length, "Value must be positive");
    if (properties == null)
      throw new ArgumentNullException(nameof (properties));
    if (element == null)
      throw new ArgumentNullException(nameof (element));
    this.length = length;
    this.properties = properties;
    this.element = element;
  }

  public UIElement Element => this.element;

  public VisualLine VisualLine { get; internal set; }

  public override LineBreakCondition BreakBefore => LineBreakCondition.BreakDesired;

  public override LineBreakCondition BreakAfter => LineBreakCondition.BreakDesired;

  public override bool HasFixedSize => true;

  public override CharacterBufferReference CharacterBufferReference
  {
    get => new CharacterBufferReference();
  }

  public override int Length => this.length;

  public override TextRunProperties Properties => this.properties;

  public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
  {
    double num = TextBlock.GetBaselineOffset((DependencyObject) this.element);
    if (double.IsNaN(num))
      num = this.desiredSize.Height;
    return new TextEmbeddedObjectMetrics(this.desiredSize.Width, this.desiredSize.Height, num);
  }

  public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
  {
    if (!this.element.IsArrangeValid)
      return Rect.Empty;
    double d = TextBlock.GetBaselineOffset((DependencyObject) this.element);
    if (double.IsNaN(d))
      d = this.desiredSize.Height;
    return new Rect(new Point(0.0, -d), this.desiredSize);
  }

  public override void Draw(
    DrawingContext drawingContext,
    Point origin,
    bool rightToLeft,
    bool sideways)
  {
  }
}
