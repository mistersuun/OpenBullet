// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.BackgroundGeometryBuilder
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public sealed class BackgroundGeometryBuilder
{
  private double cornerRadius;
  private bool alignToMiddleOfPixels;
  private PathFigureCollection figures = new PathFigureCollection();
  private PathFigure figure;
  private int insertionIndex;
  private double lastTop;
  private double lastBottom;
  private double lastLeft;
  private double lastRight;

  public double CornerRadius
  {
    get => this.cornerRadius;
    set => this.cornerRadius = value;
  }

  public bool AlignToWholePixels { get; set; }

  public double BorderThickness { get; set; }

  [Obsolete("Use the AlignToWholePixels and BorderThickness properties instead. Setting AlignToWholePixels=true and setting the BorderThickness to the pixel size is equivalent to aligning the geometry to the middle of pixels.")]
  public bool AlignToMiddleOfPixels
  {
    get => this.alignToMiddleOfPixels;
    set => this.alignToMiddleOfPixels = value;
  }

  public bool ExtendToFullWidthAtLineEnd { get; set; }

  public void AddSegment(TextView textView, ISegment segment)
  {
    Size pixelSize = textView != null ? PixelSnapHelpers.GetPixelSize((Visual) textView) : throw new ArgumentNullException(nameof (textView));
    foreach (Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment, this.ExtendToFullWidthAtLineEnd))
      this.AddRectangle(pixelSize, r);
  }

  public void AddRectangle(TextView textView, Rect rectangle)
  {
    this.AddRectangle(PixelSnapHelpers.GetPixelSize((Visual) textView), rectangle);
  }

  private void AddRectangle(Size pixelSize, Rect r)
  {
    if (this.AlignToWholePixels)
    {
      double num = 0.5 * this.BorderThickness;
      this.AddRectangle(PixelSnapHelpers.Round(r.Left - num, pixelSize.Width) + num, PixelSnapHelpers.Round(r.Top - num, pixelSize.Height) + num, PixelSnapHelpers.Round(r.Right + num, pixelSize.Width) - num, PixelSnapHelpers.Round(r.Bottom + num, pixelSize.Height) - num);
    }
    else if (this.alignToMiddleOfPixels)
      this.AddRectangle(PixelSnapHelpers.PixelAlign(r.Left, pixelSize.Width), PixelSnapHelpers.PixelAlign(r.Top, pixelSize.Height), PixelSnapHelpers.PixelAlign(r.Right, pixelSize.Width), PixelSnapHelpers.PixelAlign(r.Bottom, pixelSize.Height));
    else
      this.AddRectangle(r.Left, r.Top, r.Right, r.Bottom);
  }

  public static IEnumerable<Rect> GetRectsForSegment(
    TextView textView,
    ISegment segment,
    bool extendToFullWidthAtLineEnd = false)
  {
    if (textView == null)
      throw new ArgumentNullException(nameof (textView));
    if (segment == null)
      throw new ArgumentNullException(nameof (segment));
    return BackgroundGeometryBuilder.GetRectsForSegmentImpl(textView, segment, extendToFullWidthAtLineEnd);
  }

  private static IEnumerable<Rect> GetRectsForSegmentImpl(
    TextView textView,
    ISegment segment,
    bool extendToFullWidthAtLineEnd)
  {
    int segmentStart = segment.Offset;
    int segmentEnd = segment.Offset + segment.Length;
    segmentStart = segmentStart.CoerceValue(0, textView.Document.TextLength);
    segmentEnd = segmentEnd.CoerceValue(0, textView.Document.TextLength);
    TextViewPosition start;
    TextViewPosition end;
    if (segment is SelectionSegment)
    {
      SelectionSegment selectionSegment = (SelectionSegment) segment;
      start = new TextViewPosition(textView.Document.GetLocation(selectionSegment.StartOffset), selectionSegment.StartVisualColumn);
      end = new TextViewPosition(textView.Document.GetLocation(selectionSegment.EndOffset), selectionSegment.EndVisualColumn);
    }
    else
    {
      start = new TextViewPosition(textView.Document.GetLocation(segmentStart));
      end = new TextViewPosition(textView.Document.GetLocation(segmentEnd));
    }
    foreach (VisualLine vl in textView.VisualLines)
    {
      int vlStartOffset = vl.FirstDocumentLine.Offset;
      if (vlStartOffset <= segmentEnd)
      {
        int vlEndOffset = vl.LastDocumentLine.Offset + vl.LastDocumentLine.Length;
        if (vlEndOffset >= segmentStart)
        {
          int segmentStartVC = segmentStart >= vlStartOffset ? vl.ValidateVisualColumn(start, extendToFullWidthAtLineEnd) : 0;
          int segmentEndVC = segmentEnd <= vlEndOffset ? vl.ValidateVisualColumn(end, extendToFullWidthAtLineEnd) : (extendToFullWidthAtLineEnd ? int.MaxValue : vl.VisualLengthWithEndOfLineMarker);
          foreach (Rect rect in BackgroundGeometryBuilder.ProcessTextLines(textView, vl, segmentStartVC, segmentEndVC))
            yield return rect;
        }
      }
      else
        break;
    }
  }

  public static IEnumerable<Rect> GetRectsFromVisualSegment(
    TextView textView,
    VisualLine line,
    int startVC,
    int endVC)
  {
    if (textView == null)
      throw new ArgumentNullException(nameof (textView));
    if (line == null)
      throw new ArgumentNullException(nameof (line));
    return BackgroundGeometryBuilder.ProcessTextLines(textView, line, startVC, endVC);
  }

  private static IEnumerable<Rect> ProcessTextLines(
    TextView textView,
    VisualLine visualLine,
    int segmentStartVC,
    int segmentEndVC)
  {
    TextLine lastTextLine = visualLine.TextLines.Last<TextLine>();
    Vector scrollOffset = textView.ScrollOffset;
    for (int i = 0; i < visualLine.TextLines.Count; ++i)
    {
      TextLine line = visualLine.TextLines[i];
      double y = visualLine.GetTextLineVisualYPosition(line, VisualYPosition.LineTop);
      int visualStartCol = visualLine.GetTextLineVisualStartColumn(line);
      int visualEndCol = visualStartCol + line.Length;
      if (line == lastTextLine)
        --visualEndCol;
      else
        visualEndCol -= line.TrailingWhitespaceLength;
      if (segmentEndVC < visualStartCol)
        break;
      if (lastTextLine == line || segmentStartVC <= visualEndCol)
      {
        int segmentStartVCInLine = Math.Max(segmentStartVC, visualStartCol);
        int segmentEndVCInLine = Math.Min(segmentEndVC, visualEndCol);
        y -= scrollOffset.Y;
        Rect lastRect = Rect.Empty;
        if (segmentStartVCInLine == segmentEndVCInLine)
        {
          double x = visualLine.GetTextLineVisualXPosition(line, segmentStartVCInLine) - scrollOffset.X;
          if ((segmentEndVCInLine != visualEndCol || i >= visualLine.TextLines.Count - 1 || segmentEndVC <= segmentEndVCInLine || line.TrailingWhitespaceLength != 0) && (segmentStartVCInLine != visualStartCol || i <= 0 || segmentStartVC >= segmentStartVCInLine || visualLine.TextLines[i - 1].TrailingWhitespaceLength != 0))
            lastRect = new Rect(x, y, textView.EmptyLineSelectionWidth, line.Height);
          else
            continue;
        }
        else if (segmentStartVCInLine <= visualEndCol)
        {
          foreach (TextBounds b in (IEnumerable<TextBounds>) line.GetTextBounds(segmentStartVCInLine, segmentEndVCInLine - segmentStartVCInLine))
          {
            double left = b.Rectangle.Left - scrollOffset.X;
            double right = b.Rectangle.Right - scrollOffset.X;
            if (!lastRect.IsEmpty)
              yield return lastRect;
            lastRect = new Rect(Math.Min(left, right), y, Math.Abs(right - left), line.Height);
          }
        }
        if (segmentEndVC > visualEndCol)
        {
          double left = segmentStartVC <= visualLine.VisualLengthWithEndOfLineMarker ? (line == lastTextLine ? line.WidthIncludingTrailingWhitespace : line.Width) : visualLine.GetTextLineVisualXPosition(lastTextLine, segmentStartVC);
          double right = line != lastTextLine || segmentEndVC == int.MaxValue ? Math.Max(((IScrollInfo) textView).ExtentWidth, ((IScrollInfo) textView).ViewportWidth) : visualLine.GetTextLineVisualXPosition(lastTextLine, segmentEndVC);
          Rect extendSelection = new Rect(Math.Min(left, right), y, Math.Abs(right - left), line.Height);
          if (!lastRect.IsEmpty)
          {
            if (extendSelection.IntersectsWith(lastRect))
            {
              lastRect.Union(extendSelection);
              yield return lastRect;
            }
            else
            {
              yield return lastRect;
              yield return extendSelection;
            }
          }
          else
            yield return extendSelection;
        }
        else
          yield return lastRect;
      }
    }
  }

  public void AddRectangle(double left, double top, double right, double bottom)
  {
    if (!top.IsClose(this.lastBottom))
      this.CloseFigure();
    if (this.figure == null)
    {
      this.figure = new PathFigure();
      this.figure.StartPoint = new Point(left, top + this.cornerRadius);
      if (Math.Abs(left - right) > this.cornerRadius)
      {
        this.figure.Segments.Add((PathSegment) this.MakeArc(left + this.cornerRadius, top, SweepDirection.Clockwise));
        this.figure.Segments.Add((PathSegment) BackgroundGeometryBuilder.MakeLineSegment(right - this.cornerRadius, top));
        this.figure.Segments.Add((PathSegment) this.MakeArc(right, top + this.cornerRadius, SweepDirection.Clockwise));
      }
      this.figure.Segments.Add((PathSegment) BackgroundGeometryBuilder.MakeLineSegment(right, bottom - this.cornerRadius));
      this.insertionIndex = this.figure.Segments.Count;
    }
    else
    {
      if (!this.lastRight.IsClose(right))
      {
        double num = right < this.lastRight ? -this.cornerRadius : this.cornerRadius;
        SweepDirection dir1 = right < this.lastRight ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
        SweepDirection dir2 = right < this.lastRight ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
        this.figure.Segments.Insert(this.insertionIndex++, (PathSegment) this.MakeArc(this.lastRight + num, this.lastBottom, dir1));
        this.figure.Segments.Insert(this.insertionIndex++, (PathSegment) BackgroundGeometryBuilder.MakeLineSegment(right - num, top));
        this.figure.Segments.Insert(this.insertionIndex++, (PathSegment) this.MakeArc(right, top + this.cornerRadius, dir2));
      }
      this.figure.Segments.Insert(this.insertionIndex++, (PathSegment) BackgroundGeometryBuilder.MakeLineSegment(right, bottom - this.cornerRadius));
      this.figure.Segments.Insert(this.insertionIndex, (PathSegment) BackgroundGeometryBuilder.MakeLineSegment(this.lastLeft, this.lastTop + this.cornerRadius));
      if (!this.lastLeft.IsClose(left))
      {
        double num = left < this.lastLeft ? this.cornerRadius : -this.cornerRadius;
        SweepDirection dir3 = left < this.lastLeft ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
        SweepDirection dir4 = left < this.lastLeft ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
        this.figure.Segments.Insert(this.insertionIndex, (PathSegment) this.MakeArc(this.lastLeft, this.lastBottom - this.cornerRadius, dir3));
        this.figure.Segments.Insert(this.insertionIndex, (PathSegment) BackgroundGeometryBuilder.MakeLineSegment(this.lastLeft - num, this.lastBottom));
        this.figure.Segments.Insert(this.insertionIndex, (PathSegment) this.MakeArc(left + num, this.lastBottom, dir4));
      }
    }
    this.lastTop = top;
    this.lastBottom = bottom;
    this.lastLeft = left;
    this.lastRight = right;
  }

  private ArcSegment MakeArc(double x, double y, SweepDirection dir)
  {
    ArcSegment arcSegment = new ArcSegment(new Point(x, y), new Size(this.cornerRadius, this.cornerRadius), 0.0, false, dir, true);
    arcSegment.Freeze();
    return arcSegment;
  }

  private static LineSegment MakeLineSegment(double x, double y)
  {
    LineSegment lineSegment = new LineSegment(new Point(x, y), true);
    lineSegment.Freeze();
    return lineSegment;
  }

  public void CloseFigure()
  {
    if (this.figure == null)
      return;
    this.figure.Segments.Insert(this.insertionIndex, (PathSegment) BackgroundGeometryBuilder.MakeLineSegment(this.lastLeft, this.lastTop + this.cornerRadius));
    if (Math.Abs(this.lastLeft - this.lastRight) > this.cornerRadius)
    {
      this.figure.Segments.Insert(this.insertionIndex, (PathSegment) this.MakeArc(this.lastLeft, this.lastBottom - this.cornerRadius, SweepDirection.Clockwise));
      this.figure.Segments.Insert(this.insertionIndex, (PathSegment) BackgroundGeometryBuilder.MakeLineSegment(this.lastLeft + this.cornerRadius, this.lastBottom));
      this.figure.Segments.Insert(this.insertionIndex, (PathSegment) this.MakeArc(this.lastRight - this.cornerRadius, this.lastBottom, SweepDirection.Clockwise));
    }
    this.figure.IsClosed = true;
    this.figures.Add(this.figure);
    this.figure = (PathFigure) null;
  }

  public Geometry CreateGeometry()
  {
    this.CloseFigure();
    if (this.figures.Count == 0)
      return (Geometry) null;
    PathGeometry geometry = new PathGeometry((IEnumerable<PathFigure>) this.figures);
    geometry.Freeze();
    return (Geometry) geometry;
  }
}
