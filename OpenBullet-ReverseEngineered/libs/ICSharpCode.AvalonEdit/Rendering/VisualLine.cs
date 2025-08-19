// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.VisualLine
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

public sealed class VisualLine
{
  private TextView textView;
  private List<VisualLineElement> elements;
  internal bool hasInlineObjects;
  private VisualLine.LifetimePhase phase;
  private ReadOnlyCollection<TextLine> textLines;
  private VisualLineDrawingVisual visual;

  public TextDocument Document { get; private set; }

  public DocumentLine FirstDocumentLine { get; private set; }

  public DocumentLine LastDocumentLine { get; private set; }

  public ReadOnlyCollection<VisualLineElement> Elements { get; private set; }

  public ReadOnlyCollection<TextLine> TextLines
  {
    get
    {
      if (this.phase < VisualLine.LifetimePhase.Live)
        throw new InvalidOperationException();
      return this.textLines;
    }
  }

  public int StartOffset => this.FirstDocumentLine.Offset;

  public int VisualLength { get; private set; }

  public int VisualLengthWithEndOfLineMarker
  {
    get
    {
      int visualLength = this.VisualLength;
      if (this.textView.Options.ShowEndOfLine && this.LastDocumentLine.NextLine != null)
        ++visualLength;
      return visualLength;
    }
  }

  public double Height { get; private set; }

  public double VisualTop { get; internal set; }

  internal VisualLine(TextView textView, DocumentLine firstDocumentLine)
  {
    this.textView = textView;
    this.Document = textView.Document;
    this.FirstDocumentLine = firstDocumentLine;
  }

  internal void ConstructVisualElements(
    ITextRunConstructionContext context,
    VisualLineElementGenerator[] generators)
  {
    foreach (VisualLineElementGenerator generator in generators)
      generator.StartGeneration(context);
    this.elements = new List<VisualLineElement>();
    this.PerformVisualElementConstruction(generators);
    foreach (VisualLineElementGenerator generator in generators)
      generator.FinishGeneration();
    TextRunProperties textRunProperties = context.GlobalTextRunProperties;
    foreach (VisualLineElement element in this.elements)
      element.SetTextRunProperties(new VisualLineElementTextRunProperties(textRunProperties));
    this.Elements = this.elements.AsReadOnly();
    this.CalculateOffsets();
    this.phase = VisualLine.LifetimePhase.Transforming;
  }

  private void PerformVisualElementConstruction(VisualLineElementGenerator[] generators)
  {
    TextDocument document = this.Document;
    int offset = this.FirstDocumentLine.Offset;
    int num1 = offset + this.FirstDocumentLine.Length;
    this.LastDocumentLine = this.FirstDocumentLine;
    int num2 = 0;
    while (offset + num2 <= num1)
    {
      int num3 = num1;
      foreach (VisualLineElementGenerator generator in generators)
      {
        generator.cachedInterest = generator.GetFirstInterestedOffset(offset + num2);
        if (generator.cachedInterest != -1)
        {
          if (generator.cachedInterest < offset)
            throw new ArgumentOutOfRangeException(generator.GetType().Name + ".GetFirstInterestedOffset", (object) generator.cachedInterest, "GetFirstInterestedOffset must not return an offset less than startOffset. Return -1 to signal no interest.");
          if (generator.cachedInterest < num3)
            num3 = generator.cachedInterest;
        }
      }
      if (num3 > offset)
      {
        this.elements.Add((VisualLineElement) new VisualLineText(this, num3 - offset));
        offset = num3;
      }
      num2 = 1;
      foreach (VisualLineElementGenerator generator in generators)
      {
        if (generator.cachedInterest == offset)
        {
          VisualLineElement visualLineElement = generator.ConstructElement(offset);
          if (visualLineElement != null)
          {
            this.elements.Add(visualLineElement);
            if (visualLineElement.DocumentLength > 0)
            {
              num2 = 0;
              offset += visualLineElement.DocumentLength;
              if (offset > num1)
              {
                DocumentLine lineByOffset = document.GetLineByOffset(offset);
                num1 = lineByOffset.Offset + lineByOffset.Length;
                this.LastDocumentLine = lineByOffset;
                if (num1 < offset)
                  throw new InvalidOperationException($"The VisualLineElementGenerator {generator.GetType().Name} produced an element which ends within the line delimiter");
                break;
              }
              break;
            }
          }
        }
      }
    }
  }

  private void CalculateOffsets()
  {
    int num1 = 0;
    int num2 = 0;
    foreach (VisualLineElement element in this.elements)
    {
      element.VisualColumn = num1;
      element.RelativeTextOffset = num2;
      num1 += element.VisualLength;
      num2 += element.DocumentLength;
    }
    this.VisualLength = num1;
  }

  internal void RunTransformers(
    ITextRunConstructionContext context,
    IVisualLineTransformer[] transformers)
  {
    foreach (IVisualLineTransformer transformer in transformers)
      transformer.Transform(context, (IList<VisualLineElement>) this.elements);
    if (this.elements.Any<VisualLineElement>((Func<VisualLineElement, bool>) (e => e.TextRunProperties.TypographyProperties != null)))
    {
      foreach (VisualLineElement element in this.elements)
      {
        if (element.TextRunProperties.TypographyProperties == null)
          element.TextRunProperties.SetTypographyProperties((TextRunTypographyProperties) new DefaultTextRunTypographyProperties());
      }
    }
    this.phase = VisualLine.LifetimePhase.Live;
  }

  public void ReplaceElement(int elementIndex, params VisualLineElement[] newElements)
  {
    this.ReplaceElement(elementIndex, 1, newElements);
  }

  public void ReplaceElement(int elementIndex, int count, params VisualLineElement[] newElements)
  {
    if (this.phase != VisualLine.LifetimePhase.Transforming)
      throw new InvalidOperationException("This method may only be called by line transformers.");
    int num1 = 0;
    for (int index = elementIndex; index < elementIndex + count; ++index)
      num1 += this.elements[index].DocumentLength;
    int num2 = 0;
    foreach (VisualLineElement newElement in newElements)
      num2 += newElement.DocumentLength;
    if (num1 != num2)
      throw new InvalidOperationException($"Old elements have document length {(object) num1}, but new elements have length {(object) num2}");
    this.elements.RemoveRange(elementIndex, count);
    this.elements.InsertRange(elementIndex, (IEnumerable<VisualLineElement>) newElements);
    this.CalculateOffsets();
  }

  internal void SetTextLines(List<TextLine> textLines)
  {
    this.textLines = textLines.AsReadOnly();
    this.Height = 0.0;
    foreach (TextLine textLine in textLines)
      this.Height += textLine.Height;
  }

  public int GetVisualColumn(int relativeTextOffset)
  {
    ThrowUtil.CheckNotNegative(relativeTextOffset, nameof (relativeTextOffset));
    foreach (VisualLineElement element in this.elements)
    {
      if (element.RelativeTextOffset <= relativeTextOffset && element.RelativeTextOffset + element.DocumentLength >= relativeTextOffset)
        return element.GetVisualColumn(relativeTextOffset);
    }
    return this.VisualLength;
  }

  public int GetRelativeOffset(int visualColumn)
  {
    ThrowUtil.CheckNotNegative(visualColumn, nameof (visualColumn));
    int relativeOffset = 0;
    foreach (VisualLineElement element in this.elements)
    {
      if (element.VisualColumn <= visualColumn && element.VisualColumn + element.VisualLength > visualColumn)
        return element.GetRelativeOffset(visualColumn);
      relativeOffset += element.DocumentLength;
    }
    return relativeOffset;
  }

  public TextLine GetTextLine(int visualColumn) => this.GetTextLine(visualColumn, false);

  public TextLine GetTextLine(int visualColumn, bool isAtEndOfLine)
  {
    if (visualColumn < 0)
      throw new ArgumentOutOfRangeException(nameof (visualColumn));
    if (visualColumn >= this.VisualLengthWithEndOfLineMarker)
      return this.TextLines[this.TextLines.Count - 1];
    foreach (TextLine textLine in this.TextLines)
    {
      if ((isAtEndOfLine ? (visualColumn <= textLine.Length ? 1 : 0) : (visualColumn < textLine.Length ? 1 : 0)) != 0)
        return textLine;
      visualColumn -= textLine.Length;
    }
    throw new InvalidOperationException("Shouldn't happen (VisualLength incorrect?)");
  }

  public double GetTextLineVisualYPosition(TextLine textLine, VisualYPosition yPositionMode)
  {
    if (textLine == null)
      throw new ArgumentNullException(nameof (textLine));
    double visualTop = this.VisualTop;
    foreach (TextLine textLine1 in this.TextLines)
    {
      if (textLine1 == textLine)
      {
        switch (yPositionMode)
        {
          case VisualYPosition.LineTop:
            return visualTop;
          case VisualYPosition.TextTop:
            return visualTop + textLine1.Baseline - this.textView.DefaultBaseline;
          case VisualYPosition.LineBottom:
            return visualTop + textLine1.Height;
          case VisualYPosition.LineMiddle:
            return visualTop + textLine1.Height / 2.0;
          case VisualYPosition.TextBottom:
            return visualTop + textLine1.Baseline - this.textView.DefaultBaseline + this.textView.DefaultLineHeight;
          case VisualYPosition.TextMiddle:
            return visualTop + textLine1.Baseline - this.textView.DefaultBaseline + this.textView.DefaultLineHeight / 2.0;
          case VisualYPosition.Baseline:
            return visualTop + textLine1.Baseline;
          default:
            throw new ArgumentException("Invalid yPositionMode:" + (object) yPositionMode);
        }
      }
      else
        visualTop += textLine1.Height;
    }
    throw new ArgumentException("textLine is not a line in this VisualLine");
  }

  public int GetTextLineVisualStartColumn(TextLine textLine)
  {
    if (!this.TextLines.Contains(textLine))
      throw new ArgumentException("textLine is not a line in this VisualLine");
    int visualStartColumn = 0;
    foreach (TextLine textLine1 in this.TextLines)
    {
      if (textLine1 != textLine)
        visualStartColumn += textLine1.Length;
      else
        break;
    }
    return visualStartColumn;
  }

  public TextLine GetTextLineByVisualYPosition(double visualTop)
  {
    double visualTop1 = this.VisualTop;
    foreach (TextLine textLine in this.TextLines)
    {
      visualTop1 += textLine.Height;
      if (visualTop + 0.0001 < visualTop1)
        return textLine;
    }
    return this.TextLines[this.TextLines.Count - 1];
  }

  public Point GetVisualPosition(int visualColumn, VisualYPosition yPositionMode)
  {
    TextLine textLine = this.GetTextLine(visualColumn);
    return new Point(this.GetTextLineVisualXPosition(textLine, visualColumn), this.GetTextLineVisualYPosition(textLine, yPositionMode));
  }

  internal Point GetVisualPosition(
    int visualColumn,
    bool isAtEndOfLine,
    VisualYPosition yPositionMode)
  {
    TextLine textLine = this.GetTextLine(visualColumn, isAtEndOfLine);
    return new Point(this.GetTextLineVisualXPosition(textLine, visualColumn), this.GetTextLineVisualYPosition(textLine, yPositionMode));
  }

  public double GetTextLineVisualXPosition(TextLine textLine, int visualColumn)
  {
    if (textLine == null)
      throw new ArgumentNullException(nameof (textLine));
    double fromCharacterHit = textLine.GetDistanceFromCharacterHit(new CharacterHit(Math.Min(visualColumn, this.VisualLengthWithEndOfLineMarker), 0));
    if (visualColumn > this.VisualLengthWithEndOfLineMarker)
      fromCharacterHit += (double) (visualColumn - this.VisualLengthWithEndOfLineMarker) * this.textView.WideSpaceWidth;
    return fromCharacterHit;
  }

  public int GetVisualColumn(Point point)
  {
    return this.GetVisualColumn(point, this.textView.Options.EnableVirtualSpace);
  }

  public int GetVisualColumn(Point point, bool allowVirtualSpace)
  {
    return this.GetVisualColumn(this.GetTextLineByVisualYPosition(point.Y), point.X, allowVirtualSpace);
  }

  internal int GetVisualColumn(Point point, bool allowVirtualSpace, out bool isAtEndOfLine)
  {
    TextLine byVisualYposition = this.GetTextLineByVisualYPosition(point.Y);
    int visualColumn = this.GetVisualColumn(byVisualYposition, point.X, allowVirtualSpace);
    isAtEndOfLine = visualColumn >= this.GetTextLineVisualStartColumn(byVisualYposition) + byVisualYposition.Length;
    return visualColumn;
  }

  public int GetVisualColumn(TextLine textLine, double xPos, bool allowVirtualSpace)
  {
    if (xPos > textLine.WidthIncludingTrailingWhitespace && allowVirtualSpace && textLine == this.TextLines[this.TextLines.Count - 1])
      return this.VisualLengthWithEndOfLineMarker + (int) Math.Round((xPos - textLine.WidthIncludingTrailingWhitespace) / this.textView.WideSpaceWidth, MidpointRounding.AwayFromZero);
    CharacterHit characterHitFromDistance = textLine.GetCharacterHitFromDistance(xPos);
    return characterHitFromDistance.FirstCharacterIndex + characterHitFromDistance.TrailingLength;
  }

  public int ValidateVisualColumn(TextViewPosition position, bool allowVirtualSpace)
  {
    return this.ValidateVisualColumn(this.Document.GetOffset(position.Location), position.VisualColumn, allowVirtualSpace);
  }

  public int ValidateVisualColumn(int offset, int visualColumn, bool allowVirtualSpace)
  {
    int offset1 = this.FirstDocumentLine.Offset;
    if (visualColumn < 0 || this.GetRelativeOffset(visualColumn) + offset1 != offset)
      return this.GetVisualColumn(offset - offset1);
    return visualColumn > this.VisualLength && !allowVirtualSpace ? this.VisualLength : visualColumn;
  }

  public int GetVisualColumnFloor(Point point)
  {
    return this.GetVisualColumnFloor(point, this.textView.Options.EnableVirtualSpace);
  }

  public int GetVisualColumnFloor(Point point, bool allowVirtualSpace)
  {
    return this.GetVisualColumnFloor(point, allowVirtualSpace, out bool _);
  }

  internal int GetVisualColumnFloor(Point point, bool allowVirtualSpace, out bool isAtEndOfLine)
  {
    TextLine byVisualYposition = this.GetTextLineByVisualYPosition(point.Y);
    if (point.X > byVisualYposition.WidthIncludingTrailingWhitespace)
    {
      isAtEndOfLine = true;
      return allowVirtualSpace && byVisualYposition == this.TextLines[this.TextLines.Count - 1] ? this.VisualLengthWithEndOfLineMarker + (int) ((point.X - byVisualYposition.WidthIncludingTrailingWhitespace) / this.textView.WideSpaceWidth) : this.GetTextLineVisualStartColumn(byVisualYposition) + byVisualYposition.Length;
    }
    isAtEndOfLine = false;
    return byVisualYposition.GetCharacterHitFromDistance(point.X).FirstCharacterIndex;
  }

  public TextViewPosition GetTextViewPosition(int visualColumn)
  {
    return new TextViewPosition(this.Document.GetLocation(this.GetRelativeOffset(visualColumn) + this.FirstDocumentLine.Offset), visualColumn);
  }

  public TextViewPosition GetTextViewPosition(Point visualPosition, bool allowVirtualSpace)
  {
    bool isAtEndOfLine;
    int visualColumn = this.GetVisualColumn(visualPosition, allowVirtualSpace, out isAtEndOfLine);
    return new TextViewPosition(this.Document.GetLocation(this.GetRelativeOffset(visualColumn) + this.FirstDocumentLine.Offset), visualColumn)
    {
      IsAtEndOfLine = isAtEndOfLine
    };
  }

  public TextViewPosition GetTextViewPositionFloor(Point visualPosition, bool allowVirtualSpace)
  {
    bool isAtEndOfLine;
    int visualColumnFloor = this.GetVisualColumnFloor(visualPosition, allowVirtualSpace, out isAtEndOfLine);
    return new TextViewPosition(this.Document.GetLocation(this.GetRelativeOffset(visualColumnFloor) + this.FirstDocumentLine.Offset), visualColumnFloor)
    {
      IsAtEndOfLine = isAtEndOfLine
    };
  }

  public bool IsDisposed => this.phase == VisualLine.LifetimePhase.Disposed;

  internal void Dispose()
  {
    if (this.phase == VisualLine.LifetimePhase.Disposed)
      return;
    this.phase = VisualLine.LifetimePhase.Disposed;
    foreach (TextLine textLine in this.TextLines)
      textLine.Dispose();
  }

  public int GetNextCaretPosition(
    int visualColumn,
    LogicalDirection direction,
    CaretPositioningMode mode,
    bool allowVirtualSpace)
  {
    if (!VisualLine.HasStopsInVirtualSpace(mode))
      allowVirtualSpace = false;
    if (this.elements.Count == 0)
    {
      if (allowVirtualSpace)
      {
        if (direction == LogicalDirection.Forward)
          return Math.Max(0, visualColumn + 1);
        return visualColumn > 0 ? visualColumn - 1 : -1;
      }
      return visualColumn < 0 && direction == LogicalDirection.Forward || visualColumn > 0 && direction == LogicalDirection.Backward ? 0 : -1;
    }
    if (direction == LogicalDirection.Backward)
    {
      if (visualColumn > this.VisualLength && !this.elements[this.elements.Count - 1].HandlesLineBorders && VisualLine.HasImplicitStopAtLineEnd(mode))
        return allowVirtualSpace ? visualColumn - 1 : this.VisualLength;
      int index = this.elements.Count - 1;
      while (index >= 0 && this.elements[index].VisualColumn >= visualColumn)
        --index;
      for (; index >= 0; --index)
      {
        int nextCaretPosition = this.elements[index].GetNextCaretPosition(Math.Min(visualColumn, this.elements[index].VisualColumn + this.elements[index].VisualLength + 1), direction, mode);
        if (nextCaretPosition >= 0)
          return nextCaretPosition;
      }
      if (visualColumn > 0 && !this.elements[0].HandlesLineBorders && VisualLine.HasImplicitStopAtLineStart(mode))
        return 0;
    }
    else
    {
      if (visualColumn < 0 && !this.elements[0].HandlesLineBorders && VisualLine.HasImplicitStopAtLineStart(mode))
        return 0;
      int index = 0;
      while (index < this.elements.Count && this.elements[index].VisualColumn + this.elements[index].VisualLength <= visualColumn)
        ++index;
      for (; index < this.elements.Count; ++index)
      {
        int nextCaretPosition = this.elements[index].GetNextCaretPosition(Math.Max(visualColumn, this.elements[index].VisualColumn - 1), direction, mode);
        if (nextCaretPosition >= 0)
          return nextCaretPosition;
      }
      if ((allowVirtualSpace || !this.elements[this.elements.Count - 1].HandlesLineBorders) && VisualLine.HasImplicitStopAtLineEnd(mode))
      {
        if (visualColumn < this.VisualLength)
          return this.VisualLength;
        if (allowVirtualSpace)
          return visualColumn + 1;
      }
    }
    return -1;
  }

  private static bool HasStopsInVirtualSpace(CaretPositioningMode mode)
  {
    return mode == CaretPositioningMode.Normal || mode == CaretPositioningMode.EveryCodepoint;
  }

  private static bool HasImplicitStopAtLineStart(CaretPositioningMode mode)
  {
    return mode == CaretPositioningMode.Normal || mode == CaretPositioningMode.EveryCodepoint;
  }

  private static bool HasImplicitStopAtLineEnd(CaretPositioningMode mode) => true;

  internal VisualLineDrawingVisual Render()
  {
    if (this.visual == null)
      this.visual = new VisualLineDrawingVisual(this);
    return this.visual;
  }

  private enum LifetimePhase : byte
  {
    Generating,
    Transforming,
    Live,
    Disposed,
  }
}
