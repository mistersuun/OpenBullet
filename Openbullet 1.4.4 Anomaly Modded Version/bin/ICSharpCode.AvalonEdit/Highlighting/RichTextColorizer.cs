// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.RichTextColorizer
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

public class RichTextColorizer : DocumentColorizingTransformer
{
  private readonly RichTextModel richTextModel;

  public RichTextColorizer(RichTextModel richTextModel)
  {
    this.richTextModel = richTextModel != null ? richTextModel : throw new ArgumentNullException(nameof (richTextModel));
  }

  protected override void ColorizeLine(DocumentLine line)
  {
    foreach (HighlightedSection highlightedSection in this.richTextModel.GetHighlightedSections(line.Offset, line.Length))
    {
      HighlightedSection section = highlightedSection;
      if (!HighlightingColorizer.IsEmptyColor(section.Color))
        this.ChangeLinePart(section.Offset, section.Offset + section.Length, (Action<VisualLineElement>) (visualLineElement => HighlightingColorizer.ApplyColorToElement(visualLineElement, section.Color, this.CurrentContext)));
    }
  }
}
