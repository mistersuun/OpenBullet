// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.TextRangeProvider
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Automation.Text;
using System.Windows.Documents;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

internal class TextRangeProvider : ITextRangeProvider
{
  private readonly TextArea textArea;
  private readonly TextDocument doc;
  private ISegment segment;

  public TextRangeProvider(TextArea textArea, TextDocument doc, ISegment segment)
  {
    this.textArea = textArea;
    this.doc = doc;
    this.segment = segment;
  }

  public TextRangeProvider(TextArea textArea, TextDocument doc, int offset, int length)
  {
    this.textArea = textArea;
    this.doc = doc;
    this.segment = (ISegment) new AnchorSegment(doc, offset, length);
  }

  private string ID => $"({this.GetHashCode().ToString("x8")}: {this.segment})";

  [Conditional("DEBUG")]
  private static void Log(string format, params object[] args)
  {
  }

  public void AddToSelection()
  {
  }

  public ITextRangeProvider Clone()
  {
    return (ITextRangeProvider) new TextRangeProvider(this.textArea, this.doc, this.segment);
  }

  public bool Compare(ITextRangeProvider range)
  {
    TextRangeProvider textRangeProvider = (TextRangeProvider) range;
    return this.doc == textRangeProvider.doc && this.segment.Offset == textRangeProvider.segment.Offset && this.segment.EndOffset == textRangeProvider.segment.EndOffset;
  }

  private int GetEndpoint(TextPatternRangeEndpoint endpoint)
  {
    switch (endpoint)
    {
      case TextPatternRangeEndpoint.Start:
        return this.segment.Offset;
      case TextPatternRangeEndpoint.End:
        return this.segment.EndOffset;
      default:
        throw new ArgumentOutOfRangeException(nameof (endpoint));
    }
  }

  public int CompareEndpoints(
    TextPatternRangeEndpoint endpoint,
    ITextRangeProvider targetRange,
    TextPatternRangeEndpoint targetEndpoint)
  {
    TextRangeProvider textRangeProvider = (TextRangeProvider) targetRange;
    return this.GetEndpoint(endpoint).CompareTo(textRangeProvider.GetEndpoint(targetEndpoint));
  }

  public void ExpandToEnclosingUnit(TextUnit unit)
  {
    switch (unit)
    {
      case TextUnit.Character:
        this.ExpandToEnclosingUnit(CaretPositioningMode.Normal);
        break;
      case TextUnit.Format:
      case TextUnit.Word:
        this.ExpandToEnclosingUnit(CaretPositioningMode.WordStartOrSymbol);
        break;
      case TextUnit.Line:
      case TextUnit.Paragraph:
        this.segment = (ISegment) this.doc.GetLineByOffset(this.segment.Offset);
        break;
      case TextUnit.Document:
        this.segment = (ISegment) new AnchorSegment(this.doc, 0, this.doc.TextLength);
        break;
    }
  }

  private void ExpandToEnclosingUnit(CaretPositioningMode mode)
  {
    int nextCaretPosition1 = TextUtilities.GetNextCaretPosition((ITextSource) this.doc, this.segment.Offset + 1, LogicalDirection.Backward, mode);
    if (nextCaretPosition1 < 0)
      return;
    int nextCaretPosition2 = TextUtilities.GetNextCaretPosition((ITextSource) this.doc, nextCaretPosition1, LogicalDirection.Forward, mode);
    if (nextCaretPosition2 < 0)
      return;
    this.segment = (ISegment) new AnchorSegment(this.doc, nextCaretPosition1, nextCaretPosition2 - nextCaretPosition1);
  }

  public ITextRangeProvider FindAttribute(int attribute, object value, bool backward)
  {
    return (ITextRangeProvider) null;
  }

  public ITextRangeProvider FindText(string text, bool backward, bool ignoreCase)
  {
    string text1 = this.doc.GetText(this.segment);
    StringComparison comparisonType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
    int num = backward ? text1.LastIndexOf(text, comparisonType) : text1.IndexOf(text, comparisonType);
    return num >= 0 ? (ITextRangeProvider) new TextRangeProvider(this.textArea, this.doc, this.segment.Offset + num, text.Length) : (ITextRangeProvider) null;
  }

  public object GetAttributeValue(int attribute) => (object) null;

  public double[] GetBoundingRectangles() => (double[]) null;

  public IRawElementProviderSimple[] GetChildren() => new IRawElementProviderSimple[0];

  public IRawElementProviderSimple GetEnclosingElement()
  {
    return UIElementAutomationPeer.FromElement((UIElement) this.textArea) is TextAreaAutomationPeer areaAutomationPeer ? areaAutomationPeer.Provider : throw new NotSupportedException();
  }

  public string GetText(int maxLength)
  {
    return maxLength < 0 ? this.doc.GetText(this.segment) : this.doc.GetText(this.segment.Offset, Math.Min(this.segment.Length, maxLength));
  }

  public int Move(TextUnit unit, int count)
  {
    int num = this.MoveEndpointByUnit(TextPatternRangeEndpoint.Start, unit, count);
    this.segment = (ISegment) new SimpleSegment(this.segment.Offset, 0);
    this.ExpandToEnclosingUnit(unit);
    return num;
  }

  public void MoveEndpointByRange(
    TextPatternRangeEndpoint endpoint,
    ITextRangeProvider targetRange,
    TextPatternRangeEndpoint targetEndpoint)
  {
    TextRangeProvider textRangeProvider = (TextRangeProvider) targetRange;
    this.SetEndpoint(endpoint, textRangeProvider.GetEndpoint(targetEndpoint));
  }

  private void SetEndpoint(TextPatternRangeEndpoint endpoint, int targetOffset)
  {
    if (endpoint == TextPatternRangeEndpoint.Start)
    {
      this.segment = (ISegment) new AnchorSegment(this.doc, targetOffset, Math.Max(0, this.segment.EndOffset - targetOffset));
    }
    else
    {
      int offset = Math.Min(this.segment.Offset, targetOffset);
      this.segment = (ISegment) new AnchorSegment(this.doc, offset, targetOffset - offset);
    }
  }

  public int MoveEndpointByUnit(TextPatternRangeEndpoint endpoint, TextUnit unit, int count)
  {
    int num = this.GetEndpoint(endpoint);
    switch (unit)
    {
      case TextUnit.Character:
        num = this.MoveOffset(num, CaretPositioningMode.Normal, count);
        break;
      case TextUnit.Format:
      case TextUnit.Word:
        num = this.MoveOffset(num, CaretPositioningMode.WordStart, count);
        break;
      case TextUnit.Line:
      case TextUnit.Paragraph:
        num = this.doc.GetLineByNumber(Math.Max(1, Math.Min(this.doc.LineCount, this.doc.GetLineByOffset(num).LineNumber + count))).Offset;
        break;
      case TextUnit.Document:
        num = count < 0 ? 0 : this.doc.TextLength;
        break;
    }
    this.SetEndpoint(endpoint, num);
    return count;
  }

  private int MoveOffset(int offset, CaretPositioningMode mode, int count)
  {
    LogicalDirection direction = count < 0 ? LogicalDirection.Backward : LogicalDirection.Forward;
    count = Math.Abs(count);
    for (int index = 0; index < count; ++index)
    {
      int nextCaretPosition = TextUtilities.GetNextCaretPosition((ITextSource) this.doc, offset, direction, mode);
      if (nextCaretPosition != offset && nextCaretPosition >= 0)
        offset = nextCaretPosition;
      else
        break;
    }
    return offset;
  }

  public void RemoveFromSelection()
  {
  }

  public void ScrollIntoView(bool alignToTop)
  {
  }

  public void Select()
  {
    this.textArea.Selection = (Selection) new SimpleSelection(this.textArea, new TextViewPosition(this.doc.GetLocation(this.segment.Offset)), new TextViewPosition(this.doc.GetLocation(this.segment.EndOffset)));
  }
}
