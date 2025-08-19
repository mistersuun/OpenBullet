// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.HtmlOptions
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.IO;
using System.Net;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

public class HtmlOptions
{
  public HtmlOptions() => this.TabSize = 4;

  public HtmlOptions(TextEditorOptions options)
    : this()
  {
    this.TabSize = options != null ? options.IndentationSize : throw new ArgumentNullException(nameof (options));
  }

  public int TabSize { get; set; }

  public virtual void WriteStyleAttributeForColor(TextWriter writer, HighlightingColor color)
  {
    if (writer == null)
      throw new ArgumentNullException(nameof (writer));
    if (color == null)
      throw new ArgumentNullException(nameof (color));
    writer.Write(" style=\"");
    WebUtility.HtmlEncode(color.ToCss(), writer);
    writer.Write('"');
  }

  public virtual bool ColorNeedsSpanForStyling(HighlightingColor color)
  {
    if (color == null)
      throw new ArgumentNullException(nameof (color));
    return !string.IsNullOrEmpty(color.ToCss());
  }
}
