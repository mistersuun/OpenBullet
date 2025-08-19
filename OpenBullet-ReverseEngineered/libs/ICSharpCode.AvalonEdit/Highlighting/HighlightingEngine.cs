// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.HighlightingEngine
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

public class HighlightingEngine
{
  private readonly HighlightingRuleSet mainRuleSet;
  private ImmutableStack<HighlightingSpan> spanStack = ImmutableStack<HighlightingSpan>.Empty;
  private string lineText;
  private int lineStartOffset;
  private int position;
  private HighlightedLine highlightedLine;
  private static readonly HighlightingRuleSet emptyRuleSet = new HighlightingRuleSet()
  {
    Name = "EmptyRuleSet"
  };
  private Stack<HighlightedSection> highlightedSectionStack;
  private HighlightedSection lastPoppedSection;

  public HighlightingEngine(HighlightingRuleSet mainRuleSet)
  {
    this.mainRuleSet = mainRuleSet != null ? mainRuleSet : throw new ArgumentNullException(nameof (mainRuleSet));
  }

  public ImmutableStack<HighlightingSpan> CurrentSpanStack
  {
    get => this.spanStack;
    set => this.spanStack = value ?? ImmutableStack<HighlightingSpan>.Empty;
  }

  public HighlightedLine HighlightLine(IDocument document, IDocumentLine line)
  {
    this.lineStartOffset = line.Offset;
    this.lineText = document.GetText((ISegment) line);
    try
    {
      this.highlightedLine = new HighlightedLine(document, line);
      this.HighlightLineInternal();
      return this.highlightedLine;
    }
    finally
    {
      this.highlightedLine = (HighlightedLine) null;
      this.lineText = (string) null;
      this.lineStartOffset = 0;
    }
  }

  public void ScanLine(IDocument document, IDocumentLine line)
  {
    this.lineText = document.GetText((ISegment) line);
    try
    {
      this.HighlightLineInternal();
    }
    finally
    {
      this.lineText = (string) null;
    }
  }

  private void HighlightLineInternal()
  {
    this.position = 0;
    this.ResetColorStack();
    HighlightingRuleSet currentRuleSet = this.CurrentRuleSet;
    Stack<Match[]> matchArrayStack = new Stack<Match[]>();
    Match[] matchArray = HighlightingEngine.AllocateMatchArray(currentRuleSet.Spans.Count);
    Match endSpanMatch = (Match) null;
    HighlightingSpan highlightingSpan;
    while (true)
    {
      for (int index = 0; index < matchArray.Length; ++index)
      {
        if (matchArray[index] == null || matchArray[index].Success && matchArray[index].Index < this.position)
          matchArray[index] = currentRuleSet.Spans[index].StartExpression.Match(this.lineText, this.position);
      }
      if (endSpanMatch == null && !this.spanStack.IsEmpty)
        endSpanMatch = this.spanStack.Peek().EndExpression.Match(this.lineText, this.position);
      Match match = HighlightingEngine.Minimum(matchArray, endSpanMatch);
      if (match != null)
      {
        this.HighlightNonSpans(match.Index);
        if (match == endSpanMatch)
        {
          highlightingSpan = this.spanStack.Peek();
          if (!highlightingSpan.SpanColorIncludesEnd)
            this.PopColor();
          this.PushColor(highlightingSpan.EndColor);
          this.position = match.Index + match.Length;
          this.PopColor();
          if (highlightingSpan.SpanColorIncludesEnd)
            this.PopColor();
          this.spanStack = this.spanStack.Pop();
          currentRuleSet = this.CurrentRuleSet;
          if (matchArrayStack.Count > 0)
          {
            matchArray = matchArrayStack.Pop();
            int index = currentRuleSet.Spans.IndexOf(highlightingSpan);
            if (matchArray[index].Index == this.position)
              break;
          }
          else
            matchArray = HighlightingEngine.AllocateMatchArray(currentRuleSet.Spans.Count);
        }
        else
        {
          int index = Array.IndexOf<Match>(matchArray, match);
          HighlightingSpan span = currentRuleSet.Spans[index];
          this.spanStack = this.spanStack.Push(span);
          currentRuleSet = this.CurrentRuleSet;
          matchArrayStack.Push(matchArray);
          matchArray = HighlightingEngine.AllocateMatchArray(currentRuleSet.Spans.Count);
          if (span.SpanColorIncludesStart)
            this.PushColor(span.SpanColor);
          this.PushColor(span.StartColor);
          this.position = match.Index + match.Length;
          this.PopColor();
          if (!span.SpanColorIncludesStart)
            this.PushColor(span.SpanColor);
        }
        endSpanMatch = (Match) null;
      }
      else
        goto label_23;
    }
    throw new InvalidOperationException($"A highlighting span matched 0 characters, which would cause an endless loop.\nChange the highlighting definition so that either the start or the end regex matches at least one character.\nStart regex: {(object) highlightingSpan.StartExpression}\nEnd regex: {(object) highlightingSpan.EndExpression}");
label_23:
    this.HighlightNonSpans(this.lineText.Length);
    this.PopAllColors();
  }

  private void HighlightNonSpans(int until)
  {
    if (this.position == until)
      return;
    if (this.highlightedLine != null)
    {
      IList<HighlightingRule> rules = this.CurrentRuleSet.Rules;
      Match[] matchArray = HighlightingEngine.AllocateMatchArray(rules.Count);
      int index1;
      while (true)
      {
        for (int index2 = 0; index2 < matchArray.Length; ++index2)
        {
          if (matchArray[index2] == null || matchArray[index2].Success && matchArray[index2].Index < this.position)
            matchArray[index2] = rules[index2].Regex.Match(this.lineText, this.position, until - this.position);
        }
        Match match = HighlightingEngine.Minimum(matchArray, (Match) null);
        if (match != null)
        {
          this.position = match.Index;
          index1 = Array.IndexOf<Match>(matchArray, match);
          if (match.Length != 0)
          {
            this.PushColor(rules[index1].Color);
            this.position = match.Index + match.Length;
            this.PopColor();
          }
          else
            break;
        }
        else
          goto label_12;
      }
      throw new InvalidOperationException("A highlighting rule matched 0 characters, which would cause an endless loop.\nChange the highlighting definition so that the rule matches at least one character.\nRegex: " + (object) rules[index1].Regex);
    }
label_12:
    this.position = until;
  }

  private HighlightingRuleSet CurrentRuleSet
  {
    get
    {
      return this.spanStack.IsEmpty ? this.mainRuleSet : this.spanStack.Peek().RuleSet ?? HighlightingEngine.emptyRuleSet;
    }
  }

  private void ResetColorStack()
  {
    this.lastPoppedSection = (HighlightedSection) null;
    if (this.highlightedLine == null)
    {
      this.highlightedSectionStack = (Stack<HighlightedSection>) null;
    }
    else
    {
      this.highlightedSectionStack = new Stack<HighlightedSection>();
      foreach (HighlightingSpan highlightingSpan in this.spanStack.Reverse<HighlightingSpan>())
        this.PushColor(highlightingSpan.SpanColor);
    }
  }

  private void PushColor(HighlightingColor color)
  {
    if (this.highlightedLine == null)
      return;
    if (color == null)
      this.highlightedSectionStack.Push((HighlightedSection) null);
    else if (this.lastPoppedSection != null && this.lastPoppedSection.Color == color && this.lastPoppedSection.Offset + this.lastPoppedSection.Length == this.position + this.lineStartOffset)
    {
      this.highlightedSectionStack.Push(this.lastPoppedSection);
      this.lastPoppedSection = (HighlightedSection) null;
    }
    else
    {
      HighlightedSection highlightedSection = new HighlightedSection()
      {
        Offset = this.position + this.lineStartOffset,
        Color = color
      };
      this.highlightedLine.Sections.Add(highlightedSection);
      this.highlightedSectionStack.Push(highlightedSection);
      this.lastPoppedSection = (HighlightedSection) null;
    }
  }

  private void PopColor()
  {
    if (this.highlightedLine == null)
      return;
    HighlightedSection highlightedSection = this.highlightedSectionStack.Pop();
    if (highlightedSection == null)
      return;
    highlightedSection.Length = this.position + this.lineStartOffset - highlightedSection.Offset;
    if (highlightedSection.Length == 0)
      this.highlightedLine.Sections.Remove(highlightedSection);
    else
      this.lastPoppedSection = highlightedSection;
  }

  private void PopAllColors()
  {
    if (this.highlightedSectionStack == null)
      return;
    while (this.highlightedSectionStack.Count > 0)
      this.PopColor();
  }

  private static Match Minimum(Match[] arr, Match endSpanMatch)
  {
    Match match1 = (Match) null;
    foreach (Match match2 in arr)
    {
      if (match2.Success && (match1 == null || match2.Index < match1.Index))
        match1 = match2;
    }
    return endSpanMatch != null && endSpanMatch.Success && (match1 == null || endSpanMatch.Index < match1.Index) ? endSpanMatch : match1;
  }

  private static Match[] AllocateMatchArray(int count)
  {
    return count == 0 ? Empty<Match>.Array : new Match[count];
  }
}
