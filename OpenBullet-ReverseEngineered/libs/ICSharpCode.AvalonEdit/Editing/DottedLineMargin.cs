// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.DottedLineMargin
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

public static class DottedLineMargin
{
  private static readonly object tag = new object();

  public static UIElement Create()
  {
    Line line = new Line();
    line.X1 = 0.0;
    line.Y1 = 0.0;
    line.X2 = 0.0;
    line.Y2 = 1.0;
    line.StrokeDashArray.Add(0.0);
    line.StrokeDashArray.Add(2.0);
    line.Stretch = Stretch.Fill;
    line.StrokeThickness = 1.0;
    line.StrokeDashCap = PenLineCap.Round;
    line.Margin = new Thickness(2.0, 0.0, 2.0, 0.0);
    line.Tag = DottedLineMargin.tag;
    return (UIElement) line;
  }

  [Obsolete("This method got published accidentally; and will be removed again in a future version. Use the parameterless overload instead.")]
  public static UIElement Create(TextEditor editor)
  {
    Line line = (Line) DottedLineMargin.Create();
    line.SetBinding(Shape.StrokeProperty, (BindingBase) new Binding("LineNumbersForeground")
    {
      Source = (object) editor
    });
    return (UIElement) line;
  }

  public static bool IsDottedLineMargin(UIElement element)
  {
    return element is Line line && line.Tag == DottedLineMargin.tag;
  }
}
