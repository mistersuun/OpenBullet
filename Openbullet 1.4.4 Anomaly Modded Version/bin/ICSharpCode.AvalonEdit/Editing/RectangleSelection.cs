// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.RectangleSelection
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

public sealed class RectangleSelection : Selection
{
  public const string RectangularSelectionDataType = "AvalonEditRectangularSelection";
  public static readonly RoutedUICommand BoxSelectLeftByCharacter = RectangleSelection.Command(nameof (BoxSelectLeftByCharacter));
  public static readonly RoutedUICommand BoxSelectRightByCharacter = RectangleSelection.Command(nameof (BoxSelectRightByCharacter));
  public static readonly RoutedUICommand BoxSelectLeftByWord = RectangleSelection.Command(nameof (BoxSelectLeftByWord));
  public static readonly RoutedUICommand BoxSelectRightByWord = RectangleSelection.Command(nameof (BoxSelectRightByWord));
  public static readonly RoutedUICommand BoxSelectUpByLine = RectangleSelection.Command(nameof (BoxSelectUpByLine));
  public static readonly RoutedUICommand BoxSelectDownByLine = RectangleSelection.Command(nameof (BoxSelectDownByLine));
  public static readonly RoutedUICommand BoxSelectToLineStart = RectangleSelection.Command(nameof (BoxSelectToLineStart));
  public static readonly RoutedUICommand BoxSelectToLineEnd = RectangleSelection.Command(nameof (BoxSelectToLineEnd));
  private TextDocument document;
  private readonly int startLine;
  private readonly int endLine;
  private readonly double startXPos;
  private readonly double endXPos;
  private readonly int topLeftOffset;
  private readonly int bottomRightOffset;
  private readonly TextViewPosition start;
  private readonly TextViewPosition end;
  private readonly List<SelectionSegment> segments = new List<SelectionSegment>();

  private static RoutedUICommand Command(string name)
  {
    return new RoutedUICommand(name, name, typeof (RectangleSelection));
  }

  public RectangleSelection(TextArea textArea, TextViewPosition start, TextViewPosition end)
    : base(textArea)
  {
    this.InitDocument();
    this.startLine = start.Line;
    this.endLine = end.Line;
    this.startXPos = RectangleSelection.GetXPos(textArea, start);
    this.endXPos = RectangleSelection.GetXPos(textArea, end);
    this.CalculateSegments();
    this.topLeftOffset = this.segments.First<SelectionSegment>().StartOffset;
    this.bottomRightOffset = this.segments.Last<SelectionSegment>().EndOffset;
    this.start = start;
    this.end = end;
  }

  private RectangleSelection(
    TextArea textArea,
    int startLine,
    double startXPos,
    TextViewPosition end)
    : base(textArea)
  {
    this.InitDocument();
    this.startLine = startLine;
    this.endLine = end.Line;
    this.startXPos = startXPos;
    this.endXPos = RectangleSelection.GetXPos(textArea, end);
    this.CalculateSegments();
    this.topLeftOffset = this.segments.First<SelectionSegment>().StartOffset;
    this.bottomRightOffset = this.segments.Last<SelectionSegment>().EndOffset;
    this.start = this.GetStart();
    this.end = end;
  }

  private RectangleSelection(
    TextArea textArea,
    TextViewPosition start,
    int endLine,
    double endXPos)
    : base(textArea)
  {
    this.InitDocument();
    this.startLine = start.Line;
    this.endLine = endLine;
    this.startXPos = RectangleSelection.GetXPos(textArea, start);
    this.endXPos = endXPos;
    this.CalculateSegments();
    this.topLeftOffset = this.segments.First<SelectionSegment>().StartOffset;
    this.bottomRightOffset = this.segments.Last<SelectionSegment>().EndOffset;
    this.start = start;
    this.end = this.GetEnd();
  }

  private void InitDocument()
  {
    this.document = this.textArea.Document;
    if (this.document == null)
      throw ThrowUtil.NoDocumentAssigned();
  }

  private static double GetXPos(TextArea textArea, TextViewPosition pos)
  {
    DocumentLine lineByNumber = textArea.Document.GetLineByNumber(pos.Line);
    VisualLine constructVisualLine = textArea.TextView.GetOrConstructVisualLine(lineByNumber);
    int visualColumn = constructVisualLine.ValidateVisualColumn(pos, true);
    TextLine textLine = constructVisualLine.GetTextLine(visualColumn, pos.IsAtEndOfLine);
    return constructVisualLine.GetTextLineVisualXPosition(textLine, visualColumn);
  }

  private void CalculateSegments()
  {
    DocumentLine documentLine = this.document.GetLineByNumber(Math.Min(this.startLine, this.endLine));
    do
    {
      VisualLine constructVisualLine = this.textArea.TextView.GetOrConstructVisualLine(documentLine);
      int visualColumn1 = constructVisualLine.GetVisualColumn(new Point(this.startXPos, 0.0), true);
      int visualColumn2 = constructVisualLine.GetVisualColumn(new Point(this.endXPos, 0.0), true);
      int offset = constructVisualLine.FirstDocumentLine.Offset;
      int startOffset = offset + constructVisualLine.GetRelativeOffset(visualColumn1);
      int endOffset = offset + constructVisualLine.GetRelativeOffset(visualColumn2);
      this.segments.Add(new SelectionSegment(startOffset, visualColumn1, endOffset, visualColumn2));
      documentLine = constructVisualLine.LastDocumentLine.NextLine;
    }
    while (documentLine != null && documentLine.LineNumber <= Math.Max(this.startLine, this.endLine));
  }

  private TextViewPosition GetStart()
  {
    SelectionSegment selectionSegment = this.startLine < this.endLine ? this.segments.First<SelectionSegment>() : this.segments.Last<SelectionSegment>();
    return this.startXPos < this.endXPos ? new TextViewPosition(this.document.GetLocation(selectionSegment.StartOffset), selectionSegment.StartVisualColumn) : new TextViewPosition(this.document.GetLocation(selectionSegment.EndOffset), selectionSegment.EndVisualColumn);
  }

  private TextViewPosition GetEnd()
  {
    SelectionSegment selectionSegment = this.startLine < this.endLine ? this.segments.Last<SelectionSegment>() : this.segments.First<SelectionSegment>();
    return this.startXPos < this.endXPos ? new TextViewPosition(this.document.GetLocation(selectionSegment.EndOffset), selectionSegment.EndVisualColumn) : new TextViewPosition(this.document.GetLocation(selectionSegment.StartOffset), selectionSegment.StartVisualColumn);
  }

  public override string GetText()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (ISegment segment in this.Segments)
    {
      if (stringBuilder.Length > 0)
        stringBuilder.AppendLine();
      stringBuilder.Append(this.document.GetText(segment));
    }
    return stringBuilder.ToString();
  }

  public override Selection StartSelectionOrSetEndpoint(
    TextViewPosition startPosition,
    TextViewPosition endPosition)
  {
    return this.SetEndpoint(endPosition);
  }

  public override int Length
  {
    get => this.Segments.Sum<SelectionSegment>((Func<SelectionSegment, int>) (s => s.Length));
  }

  public override bool EnableVirtualSpace => true;

  public override ISegment SurroundingSegment
  {
    get
    {
      return (ISegment) new SimpleSegment(this.topLeftOffset, this.bottomRightOffset - this.topLeftOffset);
    }
  }

  public override IEnumerable<SelectionSegment> Segments
  {
    get => (IEnumerable<SelectionSegment>) this.segments;
  }

  public override TextViewPosition StartPosition => this.start;

  public override TextViewPosition EndPosition => this.end;

  public override bool Equals(object obj)
  {
    return obj is RectangleSelection rectangleSelection && rectangleSelection.textArea == this.textArea && rectangleSelection.topLeftOffset == this.topLeftOffset && rectangleSelection.bottomRightOffset == this.bottomRightOffset && rectangleSelection.startLine == this.startLine && rectangleSelection.endLine == this.endLine && rectangleSelection.startXPos == this.startXPos && rectangleSelection.endXPos == this.endXPos;
  }

  public override int GetHashCode() => this.topLeftOffset ^ this.bottomRightOffset;

  public override Selection SetEndpoint(TextViewPosition endPosition)
  {
    return (Selection) new RectangleSelection(this.textArea, this.startLine, this.startXPos, endPosition);
  }

  private int GetVisualColumnFromXPos(int line, double xPos)
  {
    return this.textArea.TextView.GetOrConstructVisualLine(this.textArea.Document.GetLineByNumber(line)).GetVisualColumn(new Point(xPos, 0.0), true);
  }

  public override Selection UpdateOnDocumentChange(DocumentChangeEventArgs e)
  {
    TextLocation location1 = this.textArea.Document.GetLocation(e.GetNewOffset(this.topLeftOffset, AnchorMovementType.AfterInsertion));
    TextLocation location2 = this.textArea.Document.GetLocation(e.GetNewOffset(this.bottomRightOffset, AnchorMovementType.BeforeInsertion));
    return (Selection) new RectangleSelection(this.textArea, new TextViewPosition(location1, this.GetVisualColumnFromXPos(location1.Line, this.startXPos)), new TextViewPosition(location2, this.GetVisualColumnFromXPos(location2.Line, this.endXPos)));
  }

  public override void ReplaceSelectionWithText(string newText)
  {
    if (newText == null)
      throw new ArgumentNullException(nameof (newText));
    using (this.textArea.Document.RunUpdate())
    {
      TextViewPosition textViewPosition1 = new TextViewPosition(this.document.GetLocation(this.topLeftOffset), this.GetVisualColumnFromXPos(this.startLine, this.startXPos));
      TextViewPosition textViewPosition2 = new TextViewPosition(this.document.GetLocation(this.bottomRightOffset), this.GetVisualColumnFromXPos(this.endLine, this.endXPos));
      int num1 = 0;
      int num2 = 0;
      int num3 = Math.Min(this.topLeftOffset, this.bottomRightOffset);
      TextViewPosition textViewPosition3;
      if (NewLineFinder.NextNewLine(newText, 0) == SimpleSegment.Invalid)
      {
        foreach (SelectionSegment lineSegment in this.Segments.Reverse<SelectionSegment>())
        {
          int insertionLength;
          this.ReplaceSingleLineText(this.textArea, lineSegment, newText, out insertionLength);
          num1 += insertionLength;
          num2 = insertionLength;
        }
        textViewPosition3 = new TextViewPosition(this.document.GetLocation(num3 + num2));
        this.textArea.Selection = (Selection) new RectangleSelection(this.textArea, textViewPosition3, Math.Max(this.startLine, this.endLine), RectangleSelection.GetXPos(this.textArea, textViewPosition3));
      }
      else
      {
        string[] strArray = newText.Split(NewLineFinder.NewlineStrings, this.segments.Count, StringSplitOptions.None);
        Math.Min(this.startLine, this.endLine);
        for (int index = strArray.Length - 1; index >= 0; --index)
        {
          int insertionLength;
          this.ReplaceSingleLineText(this.textArea, this.segments[index], strArray[index], out insertionLength);
          num2 = insertionLength;
        }
        textViewPosition3 = new TextViewPosition(this.document.GetLocation(num3 + num2));
        this.textArea.ClearSelection();
      }
      this.textArea.Caret.Position = this.textArea.TextView.GetPosition(new Point(RectangleSelection.GetXPos(this.textArea, textViewPosition3), this.textArea.TextView.GetVisualTopByDocumentLine(Math.Max(this.startLine, this.endLine)))).GetValueOrDefault();
    }
  }

  private void ReplaceSingleLineText(
    TextArea textArea,
    SelectionSegment lineSegment,
    string newText,
    out int insertionLength)
  {
    if (lineSegment.Length == 0)
    {
      if (newText.Length > 0 && textArea.ReadOnlySectionProvider.CanInsert(lineSegment.StartOffset))
      {
        newText = this.AddSpacesIfRequired(newText, new TextViewPosition(this.document.GetLocation(lineSegment.StartOffset), lineSegment.StartVisualColumn), new TextViewPosition(this.document.GetLocation(lineSegment.EndOffset), lineSegment.EndVisualColumn));
        textArea.Document.Insert(lineSegment.StartOffset, newText);
      }
    }
    else
    {
      ISegment[] deletableSegments = textArea.GetDeletableSegments((ISegment) lineSegment);
      for (int index = deletableSegments.Length - 1; index >= 0; --index)
      {
        if (index == deletableSegments.Length - 1)
        {
          if (deletableSegments[index].Offset == this.SurroundingSegment.Offset && deletableSegments[index].Length == this.SurroundingSegment.Length)
            newText = this.AddSpacesIfRequired(newText, new TextViewPosition(this.document.GetLocation(lineSegment.StartOffset), lineSegment.StartVisualColumn), new TextViewPosition(this.document.GetLocation(lineSegment.EndOffset), lineSegment.EndVisualColumn));
          textArea.Document.Replace(deletableSegments[index], newText);
        }
        else
          textArea.Document.Remove(deletableSegments[index]);
      }
    }
    insertionLength = newText.Length;
  }

  public static bool PerformRectangularPaste(
    TextArea textArea,
    TextViewPosition startPosition,
    string text,
    bool selectInsertedText)
  {
    if (textArea == null)
      throw new ArgumentNullException(nameof (textArea));
    int num = text != null ? text.Count<char>((Func<char, bool>) (c => c == '\n')) : throw new ArgumentNullException(nameof (text));
    TextLocation location = new TextLocation(startPosition.Line + num, startPosition.Column);
    if (location.Line <= textArea.Document.LineCount)
    {
      int offset = textArea.Document.GetOffset(location);
      if (textArea.Selection.EnableVirtualSpace || textArea.Document.GetLocation(offset) == location)
      {
        new RectangleSelection(textArea, startPosition, location.Line, RectangleSelection.GetXPos(textArea, startPosition)).ReplaceSelectionWithText(text);
        if (selectInsertedText && textArea.Selection is RectangleSelection)
        {
          RectangleSelection selection = (RectangleSelection) textArea.Selection;
          textArea.Selection = (Selection) new RectangleSelection(textArea, startPosition, selection.endLine, selection.endXPos);
        }
        return true;
      }
    }
    return false;
  }

  public override DataObject CreateDataObject(TextArea textArea)
  {
    DataObject dataObject = base.CreateDataObject(textArea);
    if (EditingCommandHandler.ConfirmDataFormat(textArea, dataObject, "AvalonEditRectangularSelection"))
    {
      MemoryStream data = new MemoryStream(1);
      data.WriteByte((byte) 1);
      dataObject.SetData("AvalonEditRectangularSelection", (object) data, false);
    }
    return dataObject;
  }

  public override string ToString()
  {
    return $"[RectangleSelection {this.startLine} {this.topLeftOffset} {this.startXPos} to {this.endLine} {this.bottomRightOffset} {this.endXPos}]";
  }
}
