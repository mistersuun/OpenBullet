// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.HighlightingColorizer
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

public class HighlightingColorizer : DocumentColorizingTransformer
{
  private readonly IHighlightingDefinition definition;
  private TextView textView;
  private IHighlighter highlighter;
  private bool isFixedHighlighter;
  private bool isInHighlightingGroup;
  private DocumentLine lastColorizedLine;
  private int lineNumberBeingColorized;

  public HighlightingColorizer(IHighlightingDefinition definition)
  {
    this.definition = definition != null ? definition : throw new ArgumentNullException(nameof (definition));
  }

  public HighlightingColorizer(IHighlighter highlighter)
  {
    this.highlighter = highlighter != null ? highlighter : throw new ArgumentNullException(nameof (highlighter));
    this.isFixedHighlighter = true;
  }

  protected HighlightingColorizer()
  {
  }

  private void textView_DocumentChanged(object sender, EventArgs e)
  {
    TextView textView = (TextView) sender;
    this.DeregisterServices(textView);
    this.RegisterServices(textView);
  }

  protected virtual void DeregisterServices(TextView textView)
  {
    if (this.highlighter == null)
      return;
    if (this.isInHighlightingGroup)
    {
      this.highlighter.EndHighlighting();
      this.isInHighlightingGroup = false;
    }
    this.highlighter.HighlightingStateChanged -= new HighlightingStateChangedEventHandler(this.OnHighlightStateChanged);
    if (textView.Services.GetService(typeof (IHighlighter)) == this.highlighter)
      textView.Services.RemoveService(typeof (IHighlighter));
    if (this.isFixedHighlighter)
      return;
    if (this.highlighter != null)
      this.highlighter.Dispose();
    this.highlighter = (IHighlighter) null;
  }

  protected virtual void RegisterServices(TextView textView)
  {
    if (textView.Document == null)
      return;
    if (!this.isFixedHighlighter)
      this.highlighter = textView.Document != null ? this.CreateHighlighter(textView, textView.Document) : (IHighlighter) null;
    if (this.highlighter == null || this.highlighter.Document != textView.Document)
      return;
    if (textView.Services.GetService(typeof (IHighlighter)) == null)
      textView.Services.AddService(typeof (IHighlighter), (object) this.highlighter);
    this.highlighter.HighlightingStateChanged += new HighlightingStateChangedEventHandler(this.OnHighlightStateChanged);
  }

  protected virtual IHighlighter CreateHighlighter(TextView textView, TextDocument document)
  {
    return this.definition != null ? (IHighlighter) new DocumentHighlighter(document, this.definition) : throw new NotSupportedException("Cannot create a highlighter because no IHighlightingDefinition was specified, and the CreateHighlighter() method was not overridden.");
  }

  protected override void OnAddToTextView(TextView textView)
  {
    if (this.textView != null)
      throw new InvalidOperationException("Cannot use a HighlightingColorizer instance in multiple text views. Please create a separate instance for each text view.");
    base.OnAddToTextView(textView);
    this.textView = textView;
    textView.DocumentChanged += new EventHandler(this.textView_DocumentChanged);
    textView.VisualLineConstructionStarting += new EventHandler<VisualLineConstructionStartEventArgs>(this.textView_VisualLineConstructionStarting);
    textView.VisualLinesChanged += new EventHandler(this.textView_VisualLinesChanged);
    this.RegisterServices(textView);
  }

  protected override void OnRemoveFromTextView(TextView textView)
  {
    this.DeregisterServices(textView);
    textView.DocumentChanged -= new EventHandler(this.textView_DocumentChanged);
    textView.VisualLineConstructionStarting -= new EventHandler<VisualLineConstructionStartEventArgs>(this.textView_VisualLineConstructionStarting);
    textView.VisualLinesChanged -= new EventHandler(this.textView_VisualLinesChanged);
    base.OnRemoveFromTextView(textView);
    this.textView = (TextView) null;
  }

  private void textView_VisualLineConstructionStarting(
    object sender,
    VisualLineConstructionStartEventArgs e)
  {
    if (this.highlighter == null)
      return;
    this.lineNumberBeingColorized = e.FirstLineInView.LineNumber - 1;
    if (!this.isInHighlightingGroup)
    {
      this.highlighter.BeginHighlighting();
      this.isInHighlightingGroup = true;
    }
    this.highlighter.UpdateHighlightingState(this.lineNumberBeingColorized);
    this.lineNumberBeingColorized = 0;
  }

  private void textView_VisualLinesChanged(object sender, EventArgs e)
  {
    if (this.highlighter == null || !this.isInHighlightingGroup)
      return;
    this.highlighter.EndHighlighting();
    this.isInHighlightingGroup = false;
  }

  protected override void Colorize(ITextRunConstructionContext context)
  {
    this.lastColorizedLine = (DocumentLine) null;
    base.Colorize(context);
    if (this.lastColorizedLine != context.VisualLine.LastDocumentLine && this.highlighter != null)
    {
      this.lineNumberBeingColorized = context.VisualLine.LastDocumentLine.LineNumber;
      this.highlighter.UpdateHighlightingState(this.lineNumberBeingColorized);
      this.lineNumberBeingColorized = 0;
    }
    this.lastColorizedLine = (DocumentLine) null;
  }

  protected override void ColorizeLine(DocumentLine line)
  {
    if (this.highlighter != null)
    {
      this.lineNumberBeingColorized = line.LineNumber;
      HighlightedLine highlightedLine = this.highlighter.HighlightLine(this.lineNumberBeingColorized);
      this.lineNumberBeingColorized = 0;
      foreach (HighlightedSection section1 in (IEnumerable<HighlightedSection>) highlightedLine.Sections)
      {
        HighlightedSection section = section1;
        if (!HighlightingColorizer.IsEmptyColor(section.Color))
          this.ChangeLinePart(section.Offset, section.Offset + section.Length, (Action<VisualLineElement>) (visualLineElement => this.ApplyColorToElement(visualLineElement, section.Color)));
      }
    }
    this.lastColorizedLine = line;
  }

  internal static bool IsEmptyColor(HighlightingColor color)
  {
    if (color == null)
      return true;
    return color.Background == null && color.Foreground == null && !color.FontStyle.HasValue && !color.FontWeight.HasValue && !color.Underline.HasValue;
  }

  protected virtual void ApplyColorToElement(VisualLineElement element, HighlightingColor color)
  {
    HighlightingColorizer.ApplyColorToElement(element, color, this.CurrentContext);
  }

  internal static void ApplyColorToElement(
    VisualLineElement element,
    HighlightingColor color,
    ITextRunConstructionContext context)
  {
    if (color.Foreground != null)
    {
      Brush brush = color.Foreground.GetBrush(context);
      if (brush != null)
        element.TextRunProperties.SetForegroundBrush(brush);
    }
    if (color.Background != null)
    {
      Brush brush = color.Background.GetBrush(context);
      if (brush != null)
        element.BackgroundBrush = brush;
    }
    if (color.FontStyle.HasValue || color.FontWeight.HasValue)
    {
      Typeface typeface = element.TextRunProperties.Typeface;
      element.TextRunProperties.SetTypeface(new Typeface(typeface.FontFamily, color.FontStyle ?? typeface.Style, color.FontWeight ?? typeface.Weight, typeface.Stretch));
    }
    if (((int) color.Underline ?? 0) == 0)
      return;
    element.TextRunProperties.SetTextDecorations(TextDecorations.Underline);
  }

  private void OnHighlightStateChanged(int fromLineNumber, int toLineNumber)
  {
    if (this.lineNumberBeingColorized != 0 && toLineNumber <= this.lineNumberBeingColorized)
      return;
    if (fromLineNumber == toLineNumber)
    {
      this.textView.Redraw((ISegment) this.textView.Document.GetLineByNumber(fromLineNumber));
    }
    else
    {
      DocumentLine lineByNumber1 = this.textView.Document.GetLineByNumber(fromLineNumber);
      DocumentLine lineByNumber2 = this.textView.Document.GetLineByNumber(toLineNumber);
      int offset = lineByNumber1.Offset;
      this.textView.Redraw(offset, lineByNumber2.EndOffset - offset);
    }
  }
}
