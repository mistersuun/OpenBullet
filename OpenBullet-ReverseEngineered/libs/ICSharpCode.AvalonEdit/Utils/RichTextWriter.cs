// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.RichTextWriter
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

internal abstract class RichTextWriter : TextWriter
{
  protected abstract void BeginUnhandledSpan();

  public void Write(RichText richText) => this.Write(richText, 0, richText.Length);

  public virtual void Write(RichText richText, int offset, int length)
  {
    foreach (HighlightedSection highlightedSection in richText.GetHighlightedSections(offset, length))
    {
      this.BeginSpan(highlightedSection.Color);
      this.Write((RichText) richText.Text.Substring(highlightedSection.Offset, highlightedSection.Length));
      this.EndSpan();
    }
  }

  public virtual void BeginSpan(Color foregroundColor) => this.BeginUnhandledSpan();

  public virtual void BeginSpan(FontWeight fontWeight) => this.BeginUnhandledSpan();

  public virtual void BeginSpan(FontStyle fontStyle) => this.BeginUnhandledSpan();

  public virtual void BeginSpan(FontFamily fontFamily) => this.BeginUnhandledSpan();

  public virtual void BeginSpan(HighlightingColor highlightingColor) => this.BeginUnhandledSpan();

  public virtual void BeginHyperlinkSpan(Uri uri) => this.BeginUnhandledSpan();

  public abstract void EndSpan();

  public abstract void Indent();

  public abstract void Unindent();
}
