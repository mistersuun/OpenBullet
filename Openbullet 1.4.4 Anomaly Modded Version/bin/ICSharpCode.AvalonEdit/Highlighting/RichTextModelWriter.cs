// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.RichTextModelWriter
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

internal class RichTextModelWriter : PlainRichTextWriter
{
  private readonly RichTextModel richTextModel;
  private readonly DocumentTextWriter documentTextWriter;
  private readonly Stack<HighlightingColor> colorStack = new Stack<HighlightingColor>();
  private HighlightingColor currentColor;
  private int currentColorBegin = -1;

  public RichTextModelWriter(RichTextModel richTextModel, IDocument document, int insertionOffset)
    : base((TextWriter) new DocumentTextWriter(document, insertionOffset))
  {
    this.richTextModel = richTextModel != null ? richTextModel : throw new ArgumentNullException(nameof (richTextModel));
    this.documentTextWriter = (DocumentTextWriter) this.textWriter;
    this.currentColor = richTextModel.GetHighlightingAt(Math.Max(0, insertionOffset - 1));
  }

  public int InsertionOffset
  {
    get => this.documentTextWriter.InsertionOffset;
    set => this.documentTextWriter.InsertionOffset = value;
  }

  protected override void BeginUnhandledSpan() => this.colorStack.Push(this.currentColor);

  private void BeginColorSpan()
  {
    this.WriteIndentationIfNecessary();
    this.colorStack.Push(this.currentColor);
    this.currentColor = this.currentColor.Clone();
    this.currentColorBegin = this.documentTextWriter.InsertionOffset;
  }

  public override void EndSpan()
  {
    this.currentColor = this.colorStack.Pop();
    this.currentColorBegin = this.documentTextWriter.InsertionOffset;
  }

  protected override void AfterWrite()
  {
    base.AfterWrite();
    this.richTextModel.SetHighlighting(this.currentColorBegin, this.documentTextWriter.InsertionOffset - this.currentColorBegin, this.currentColor);
  }

  public override void BeginSpan(Color foregroundColor)
  {
    this.BeginColorSpan();
    this.currentColor.Foreground = (HighlightingBrush) new SimpleHighlightingBrush(foregroundColor);
    this.currentColor.Freeze();
  }

  public override void BeginSpan(FontFamily fontFamily) => this.BeginUnhandledSpan();

  public override void BeginSpan(FontStyle fontStyle)
  {
    this.BeginColorSpan();
    this.currentColor.FontStyle = new FontStyle?(fontStyle);
    this.currentColor.Freeze();
  }

  public override void BeginSpan(FontWeight fontWeight)
  {
    this.BeginColorSpan();
    this.currentColor.FontWeight = new FontWeight?(fontWeight);
    this.currentColor.Freeze();
  }

  public override void BeginSpan(HighlightingColor highlightingColor)
  {
    this.BeginColorSpan();
    this.currentColor.MergeWith(highlightingColor);
    this.currentColor.Freeze();
  }
}
