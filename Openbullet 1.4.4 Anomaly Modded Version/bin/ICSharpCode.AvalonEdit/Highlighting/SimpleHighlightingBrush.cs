// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.SimpleHighlightingBrush
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

[Serializable]
public sealed class SimpleHighlightingBrush : HighlightingBrush, ISerializable
{
  private readonly SolidColorBrush brush;

  internal SimpleHighlightingBrush(SolidColorBrush brush)
  {
    brush.Freeze();
    this.brush = brush;
  }

  public SimpleHighlightingBrush(Color color)
    : this(new SolidColorBrush(color))
  {
  }

  public override Brush GetBrush(ITextRunConstructionContext context) => (Brush) this.brush;

  public override string ToString() => this.brush.ToString();

  private SimpleHighlightingBrush(SerializationInfo info, StreamingContext context)
  {
    this.brush = new SolidColorBrush((Color) ColorConverter.ConvertFromString(info.GetString("color")));
    this.brush.Freeze();
  }

  void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
  {
    info.AddValue("color", (object) this.brush.Color.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }

  public override bool Equals(object obj)
  {
    return obj is SimpleHighlightingBrush highlightingBrush && this.brush.Color.Equals(highlightingBrush.brush.Color);
  }

  public override int GetHashCode() => this.brush.Color.GetHashCode();
}
