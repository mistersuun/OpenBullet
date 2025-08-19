// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.VisualLineTextParagraphProperties
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System.Windows;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class VisualLineTextParagraphProperties : TextParagraphProperties
{
  internal TextRunProperties defaultTextRunProperties;
  internal TextWrapping textWrapping;
  internal double tabSize;
  internal double indent;
  internal bool firstLineInParagraph;

  public override double DefaultIncrementalTab => this.tabSize;

  public override FlowDirection FlowDirection => FlowDirection.LeftToRight;

  public override TextAlignment TextAlignment => TextAlignment.Left;

  public override double LineHeight => double.NaN;

  public override bool FirstLineInParagraph => this.firstLineInParagraph;

  public override TextRunProperties DefaultTextRunProperties => this.defaultTextRunProperties;

  public override TextWrapping TextWrapping => this.textWrapping;

  public override TextMarkerProperties TextMarkerProperties => (TextMarkerProperties) null;

  public override double Indent => this.indent;
}
