// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Folding.FoldingElementGenerator
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Folding;

public sealed class FoldingElementGenerator : VisualLineElementGenerator, ITextViewConnect
{
  private readonly List<TextView> textViews = new List<TextView>();
  private FoldingManager foldingManager;
  public static readonly Brush DefaultTextBrush = (Brush) Brushes.Gray;
  private static Brush textBrush = FoldingElementGenerator.DefaultTextBrush;

  public FoldingManager FoldingManager
  {
    get => this.foldingManager;
    set
    {
      if (this.foldingManager == value)
        return;
      if (this.foldingManager != null)
      {
        foreach (TextView textView in this.textViews)
          this.foldingManager.RemoveFromTextView(textView);
      }
      this.foldingManager = value;
      if (this.foldingManager == null)
        return;
      foreach (TextView textView in this.textViews)
        this.foldingManager.AddToTextView(textView);
    }
  }

  void ITextViewConnect.AddToTextView(TextView textView)
  {
    this.textViews.Add(textView);
    if (this.foldingManager == null)
      return;
    this.foldingManager.AddToTextView(textView);
  }

  void ITextViewConnect.RemoveFromTextView(TextView textView)
  {
    this.textViews.Remove(textView);
    if (this.foldingManager == null)
      return;
    this.foldingManager.RemoveFromTextView(textView);
  }

  public override void StartGeneration(ITextRunConstructionContext context)
  {
    base.StartGeneration(context);
    if (this.foldingManager == null)
      return;
    if (!this.foldingManager.textViews.Contains(context.TextView))
      throw new ArgumentException("Invalid TextView");
    if (context.Document != this.foldingManager.document)
      throw new ArgumentException("Invalid document");
  }

  public override int GetFirstInterestedOffset(int startOffset)
  {
    if (this.foldingManager == null)
      return -1;
    foreach (FoldingSection foldingSection in this.foldingManager.GetFoldingsContaining(startOffset))
    {
      if (foldingSection.IsFolded)
      {
        int endOffset = foldingSection.EndOffset;
      }
    }
    return this.foldingManager.GetNextFoldedFoldingStart(startOffset);
  }

  public override VisualLineElement ConstructElement(int offset)
  {
    if (this.foldingManager == null)
      return (VisualLineElement) null;
    int offset1 = -1;
    FoldingSection fs = (FoldingSection) null;
    foreach (FoldingSection foldingSection in this.foldingManager.GetFoldingsContaining(offset))
    {
      if (foldingSection.IsFolded && foldingSection.EndOffset > offset1)
      {
        offset1 = foldingSection.EndOffset;
        fs = foldingSection;
      }
    }
    if (offset1 <= offset || fs == null)
      return (VisualLineElement) null;
    bool flag;
    do
    {
      flag = false;
      foreach (FoldingSection foldingSection in this.FoldingManager.GetFoldingsContaining(offset1))
      {
        if (foldingSection.IsFolded && foldingSection.EndOffset > offset1)
        {
          offset1 = foldingSection.EndOffset;
          flag = true;
        }
      }
    }
    while (flag);
    string text1 = fs.Title;
    if (string.IsNullOrEmpty(text1))
      text1 = "...";
    VisualLineElementTextRunProperties properties = new VisualLineElementTextRunProperties(this.CurrentContext.GlobalTextRunProperties);
    properties.SetForegroundBrush(FoldingElementGenerator.textBrush);
    TextLine text2 = FormattedTextElement.PrepareText(TextFormatterFactory.Create((DependencyObject) this.CurrentContext.TextView), text1, (TextRunProperties) properties);
    return (VisualLineElement) new FoldingElementGenerator.FoldingLineElement(fs, text2, offset1 - offset)
    {
      textBrush = FoldingElementGenerator.textBrush
    };
  }

  public static Brush TextBrush
  {
    get => FoldingElementGenerator.textBrush;
    set => FoldingElementGenerator.textBrush = value;
  }

  private sealed class FoldingLineElement : FormattedTextElement
  {
    private readonly FoldingSection fs;
    internal Brush textBrush;

    public FoldingLineElement(FoldingSection fs, TextLine text, int documentLength)
      : base(text, documentLength)
    {
      this.fs = fs;
    }

    public override TextRun CreateTextRun(
      int startVisualColumn,
      ITextRunConstructionContext context)
    {
      return (TextRun) new FoldingElementGenerator.FoldingLineTextRun((FormattedTextElement) this, (TextRunProperties) this.TextRunProperties)
      {
        textBrush = this.textBrush
      };
    }

    protected internal override void OnMouseDown(MouseButtonEventArgs e)
    {
      if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
      {
        this.fs.IsFolded = false;
        e.Handled = true;
      }
      else
        base.OnMouseDown(e);
    }
  }

  private sealed class FoldingLineTextRun(
    FormattedTextElement element,
    TextRunProperties properties) : FormattedTextRun(element, properties)
  {
    internal Brush textBrush;

    public override void Draw(
      DrawingContext drawingContext,
      Point origin,
      bool rightToLeft,
      bool sideways)
    {
      TextEmbeddedObjectMetrics embeddedObjectMetrics = this.Format(double.PositiveInfinity);
      Rect rectangle = new Rect(origin.X, origin.Y - embeddedObjectMetrics.Baseline, embeddedObjectMetrics.Width, embeddedObjectMetrics.Height);
      drawingContext.DrawRectangle((Brush) null, new Pen(this.textBrush, 1.0), rectangle);
      base.Draw(drawingContext, origin, rightToLeft, sideways);
    }
  }
}
