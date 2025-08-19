// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.HighlightedSection
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

public class HighlightedSection : ISegment
{
  public int Offset { get; set; }

  public int Length { get; set; }

  int ISegment.EndOffset => this.Offset + this.Length;

  public HighlightingColor Color { get; set; }

  public override string ToString()
  {
    return $"[HighlightedSection ({this.Offset}-{this.Offset + this.Length})={this.Color}]";
  }
}
