// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Folding.FoldingMargin
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Folding;

public class FoldingMargin : AbstractMargin
{
  internal const double SizeFactor = 1.3333333333333333;
  public static readonly DependencyProperty FoldingMarkerBrushProperty = DependencyProperty.RegisterAttached(nameof (FoldingMarkerBrush), typeof (Brush), typeof (FoldingMargin), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Gray, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(FoldingMargin.OnUpdateBrushes)));
  public static readonly DependencyProperty FoldingMarkerBackgroundBrushProperty = DependencyProperty.RegisterAttached(nameof (FoldingMarkerBackgroundBrush), typeof (Brush), typeof (FoldingMargin), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.White, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(FoldingMargin.OnUpdateBrushes)));
  public static readonly DependencyProperty SelectedFoldingMarkerBrushProperty = DependencyProperty.RegisterAttached(nameof (SelectedFoldingMarkerBrush), typeof (Brush), typeof (FoldingMargin), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Black, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(FoldingMargin.OnUpdateBrushes)));
  public static readonly DependencyProperty SelectedFoldingMarkerBackgroundBrushProperty = DependencyProperty.RegisterAttached(nameof (SelectedFoldingMarkerBackgroundBrush), typeof (Brush), typeof (FoldingMargin), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.White, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(FoldingMargin.OnUpdateBrushes)));
  private List<FoldingMarginMarker> markers = new List<FoldingMarginMarker>();
  private Pen foldingControlPen = FoldingMargin.MakeFrozenPen((Brush) FoldingMargin.FoldingMarkerBrushProperty.DefaultMetadata.DefaultValue);
  private Pen selectedFoldingControlPen = FoldingMargin.MakeFrozenPen((Brush) FoldingMargin.SelectedFoldingMarkerBrushProperty.DefaultMetadata.DefaultValue);

  public FoldingManager FoldingManager { get; set; }

  public Brush FoldingMarkerBrush
  {
    get => (Brush) this.GetValue(FoldingMargin.FoldingMarkerBrushProperty);
    set => this.SetValue(FoldingMargin.FoldingMarkerBrushProperty, (object) value);
  }

  public Brush FoldingMarkerBackgroundBrush
  {
    get => (Brush) this.GetValue(FoldingMargin.FoldingMarkerBackgroundBrushProperty);
    set => this.SetValue(FoldingMargin.FoldingMarkerBackgroundBrushProperty, (object) value);
  }

  public Brush SelectedFoldingMarkerBrush
  {
    get => (Brush) this.GetValue(FoldingMargin.SelectedFoldingMarkerBrushProperty);
    set => this.SetValue(FoldingMargin.SelectedFoldingMarkerBrushProperty, (object) value);
  }

  public Brush SelectedFoldingMarkerBackgroundBrush
  {
    get => (Brush) this.GetValue(FoldingMargin.SelectedFoldingMarkerBackgroundBrushProperty);
    set
    {
      this.SetValue(FoldingMargin.SelectedFoldingMarkerBackgroundBrushProperty, (object) value);
    }
  }

  private static void OnUpdateBrushes(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    FoldingMargin foldingMargin = (FoldingMargin) null;
    if (d is FoldingMargin)
      foldingMargin = (FoldingMargin) d;
    else if (d is TextEditor)
      foldingMargin = ((TextEditor) d).TextArea.LeftMargins.FirstOrDefault<UIElement>((Func<UIElement, bool>) (c => c is FoldingMargin)) as FoldingMargin;
    if (foldingMargin == null)
      return;
    if (e.Property.Name == FoldingMargin.FoldingMarkerBrushProperty.Name)
      foldingMargin.foldingControlPen = FoldingMargin.MakeFrozenPen((Brush) e.NewValue);
    if (!(e.Property.Name == FoldingMargin.SelectedFoldingMarkerBrushProperty.Name))
      return;
    foldingMargin.selectedFoldingControlPen = FoldingMargin.MakeFrozenPen((Brush) e.NewValue);
  }

  protected override Size MeasureOverride(Size availableSize)
  {
    foreach (UIElement marker in this.markers)
      marker.Measure(availableSize);
    return new Size(PixelSnapHelpers.RoundToOdd(4.0 / 3.0 * (double) this.GetValue(TextBlock.FontSizeProperty), PixelSnapHelpers.GetPixelSize((Visual) this).Width), 0.0);
  }

  protected override Size ArrangeOverride(Size finalSize)
  {
    Size pixelSize = PixelSnapHelpers.GetPixelSize((Visual) this);
    foreach (FoldingMarginMarker marker in this.markers)
    {
      int visualColumn = marker.VisualLine.GetVisualColumn(marker.FoldingSection.StartOffset - marker.VisualLine.FirstDocumentLine.Offset);
      TextLine textLine = marker.VisualLine.GetTextLine(visualColumn);
      double y = marker.VisualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextMiddle) - this.TextView.VerticalOffset - marker.DesiredSize.Height / 2.0;
      double x = (finalSize.Width - marker.DesiredSize.Width) / 2.0;
      marker.Arrange(new Rect(PixelSnapHelpers.Round(new Point(x, y), pixelSize), marker.DesiredSize));
    }
    return base.ArrangeOverride(finalSize);
  }

  protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
  {
    if (oldTextView != null)
      oldTextView.VisualLinesChanged -= new EventHandler(this.TextViewVisualLinesChanged);
    base.OnTextViewChanged(oldTextView, newTextView);
    if (newTextView != null)
      newTextView.VisualLinesChanged += new EventHandler(this.TextViewVisualLinesChanged);
    this.TextViewVisualLinesChanged((object) null, (EventArgs) null);
  }

  private void TextViewVisualLinesChanged(object sender, EventArgs e)
  {
    foreach (Visual marker in this.markers)
      this.RemoveVisualChild(marker);
    this.markers.Clear();
    this.InvalidateVisual();
    if (this.TextView == null || this.FoldingManager == null || !this.TextView.VisualLinesValid)
      return;
    foreach (VisualLine visualLine in this.TextView.VisualLines)
    {
      FoldingSection nextFolding = this.FoldingManager.GetNextFolding(visualLine.FirstDocumentLine.Offset);
      if (nextFolding != null && nextFolding.StartOffset <= visualLine.LastDocumentLine.Offset + visualLine.LastDocumentLine.Length)
      {
        FoldingMarginMarker child = new FoldingMarginMarker()
        {
          IsExpanded = !nextFolding.IsFolded,
          VisualLine = visualLine,
          FoldingSection = nextFolding
        };
        this.markers.Add(child);
        this.AddVisualChild((Visual) child);
        child.IsMouseDirectlyOverChanged += (DependencyPropertyChangedEventHandler) ((param0, param1) => this.InvalidateVisual());
        this.InvalidateMeasure();
      }
    }
  }

  protected override int VisualChildrenCount => this.markers.Count;

  protected override Visual GetVisualChild(int index) => (Visual) this.markers[index];

  private static Pen MakeFrozenPen(Brush brush)
  {
    Pen pen = new Pen(brush, 1.0);
    pen.Freeze();
    return pen;
  }

  protected override void OnRender(DrawingContext drawingContext)
  {
    if (this.TextView == null || !this.TextView.VisualLinesValid || this.TextView.VisualLines.Count == 0 || this.FoldingManager == null)
      return;
    List<TextLine> list = this.TextView.VisualLines.SelectMany<VisualLine, TextLine>((Func<VisualLine, IEnumerable<TextLine>>) (vl => (IEnumerable<TextLine>) vl.TextLines)).ToList<TextLine>();
    Pen[] colors = new Pen[list.Count + 1];
    Pen[] endMarker = new Pen[list.Count];
    this.CalculateFoldLinesForFoldingsActiveAtStart(list, colors, endMarker);
    this.CalculateFoldLinesForMarkers(list, colors, endMarker);
    this.DrawFoldLines(drawingContext, colors, endMarker);
    base.OnRender(drawingContext);
  }

  private void CalculateFoldLinesForFoldingsActiveAtStart(
    List<TextLine> allTextLines,
    Pen[] colors,
    Pen[] endMarker)
  {
    int offset1 = this.TextView.VisualLines[0].FirstDocumentLine.Offset;
    int endOffset1 = this.TextView.VisualLines.Last<VisualLine>().LastDocumentLine.EndOffset;
    ReadOnlyCollection<FoldingSection> foldingsContaining = this.FoldingManager.GetFoldingsContaining(offset1);
    int offset2 = 0;
    foreach (FoldingSection foldingSection in foldingsContaining)
    {
      int endOffset2 = foldingSection.EndOffset;
      if (endOffset2 <= endOffset1 && !foldingSection.IsFolded)
      {
        int lineIndexFromOffset = this.GetTextLineIndexFromOffset(allTextLines, endOffset2);
        if (lineIndexFromOffset >= 0)
          endMarker[lineIndexFromOffset] = this.foldingControlPen;
      }
      if (endOffset2 > offset2 && foldingSection.StartOffset < offset1)
        offset2 = endOffset2;
    }
    if (offset2 <= 0)
      return;
    if (offset2 > endOffset1)
    {
      for (int index = 0; index < colors.Length; ++index)
        colors[index] = this.foldingControlPen;
    }
    else
    {
      int lineIndexFromOffset = this.GetTextLineIndexFromOffset(allTextLines, offset2);
      for (int index = 0; index <= lineIndexFromOffset; ++index)
        colors[index] = this.foldingControlPen;
    }
  }

  private void CalculateFoldLinesForMarkers(
    List<TextLine> allTextLines,
    Pen[] colors,
    Pen[] endMarker)
  {
    using (List<FoldingMarginMarker>.Enumerator enumerator = this.markers.GetEnumerator())
    {
label_15:
      while (enumerator.MoveNext())
      {
        FoldingMarginMarker current = enumerator.Current;
        int endOffset = current.FoldingSection.EndOffset;
        int lineIndexFromOffset1 = this.GetTextLineIndexFromOffset(allTextLines, endOffset);
        if (!current.FoldingSection.IsFolded && lineIndexFromOffset1 >= 0)
        {
          if (current.IsMouseDirectlyOver)
            endMarker[lineIndexFromOffset1] = this.selectedFoldingControlPen;
          else if (endMarker[lineIndexFromOffset1] == null)
            endMarker[lineIndexFromOffset1] = this.foldingControlPen;
        }
        int lineIndexFromOffset2 = this.GetTextLineIndexFromOffset(allTextLines, current.FoldingSection.StartOffset);
        if (lineIndexFromOffset2 >= 0)
        {
          int index = lineIndexFromOffset2 + 1;
          while (true)
          {
            if (index < colors.Length && index - 1 != lineIndexFromOffset1)
            {
              if (current.IsMouseDirectlyOver)
                colors[index] = this.selectedFoldingControlPen;
              else if (colors[index] == null)
                colors[index] = this.foldingControlPen;
              ++index;
            }
            else
              goto label_15;
          }
        }
      }
    }
  }

  private void DrawFoldLines(DrawingContext drawingContext, Pen[] colors, Pen[] endMarker)
  {
    Size pixelSize = PixelSnapHelpers.GetPixelSize((Visual) this);
    double x = PixelSnapHelpers.PixelAlign(this.RenderSize.Width / 2.0, pixelSize.Width);
    double num = 0.0;
    Pen color = colors[0];
    int index = 0;
    foreach (VisualLine visualLine in this.TextView.VisualLines)
    {
      foreach (TextLine textLine in visualLine.TextLines)
      {
        if (endMarker[index] != null)
        {
          double visualPos = this.GetVisualPos(visualLine, textLine, pixelSize.Height);
          drawingContext.DrawLine(endMarker[index], new Point(x - pixelSize.Width / 2.0, visualPos), new Point(this.RenderSize.Width, visualPos));
        }
        if (colors[index + 1] != color)
        {
          double visualPos = this.GetVisualPos(visualLine, textLine, pixelSize.Height);
          if (color != null)
            drawingContext.DrawLine(color, new Point(x, num + pixelSize.Height / 2.0), new Point(x, visualPos - pixelSize.Height / 2.0));
          color = colors[index + 1];
          num = visualPos;
        }
        ++index;
      }
    }
    if (color == null)
      return;
    drawingContext.DrawLine(color, new Point(x, num + pixelSize.Height / 2.0), new Point(x, this.RenderSize.Height));
  }

  private double GetVisualPos(VisualLine vl, TextLine tl, double pixelHeight)
  {
    return PixelSnapHelpers.PixelAlign(vl.GetTextLineVisualYPosition(tl, VisualYPosition.TextMiddle) - this.TextView.VerticalOffset, pixelHeight);
  }

  private int GetTextLineIndexFromOffset(List<TextLine> textLines, int offset)
  {
    VisualLine visualLine = this.TextView.GetVisualLine(this.TextView.Document.GetLineByOffset(offset).LineNumber);
    if (visualLine == null)
      return -1;
    int relativeTextOffset = offset - visualLine.FirstDocumentLine.Offset;
    TextLine textLine = visualLine.GetTextLine(visualLine.GetVisualColumn(relativeTextOffset));
    return textLines.IndexOf(textLine);
  }
}
