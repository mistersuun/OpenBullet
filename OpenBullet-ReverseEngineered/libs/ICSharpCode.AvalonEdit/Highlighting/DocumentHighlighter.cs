// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.DocumentHighlighter
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

public class DocumentHighlighter : ILineTracker, IHighlighter, IDisposable
{
  private readonly CompressingTreeList<ImmutableStack<HighlightingSpan>> storedSpanStacks = new CompressingTreeList<ImmutableStack<HighlightingSpan>>(new Func<ImmutableStack<HighlightingSpan>, ImmutableStack<HighlightingSpan>, bool>(object.ReferenceEquals));
  private readonly CompressingTreeList<bool> isValid = new CompressingTreeList<bool>((Func<bool, bool, bool>) ((a, b) => a == b));
  private readonly IDocument document;
  private readonly IHighlightingDefinition definition;
  private readonly HighlightingEngine engine;
  private readonly WeakLineTracker weakLineTracker;
  private bool isHighlighting;
  private bool isInHighlightingGroup;
  private bool isDisposed;
  private ImmutableStack<HighlightingSpan> initialSpanStack = ImmutableStack<HighlightingSpan>.Empty;
  private int firstInvalidLine;

  public IDocument Document => this.document;

  public DocumentHighlighter(TextDocument document, IHighlightingDefinition definition)
  {
    if (document == null)
      throw new ArgumentNullException(nameof (document));
    if (definition == null)
      throw new ArgumentNullException(nameof (definition));
    this.document = (IDocument) document;
    this.definition = definition;
    this.engine = new HighlightingEngine(definition.MainRuleSet);
    document.VerifyAccess();
    this.weakLineTracker = WeakLineTracker.Register(document, (ILineTracker) this);
    this.InvalidateSpanStacks();
  }

  public void Dispose()
  {
    if (this.weakLineTracker != null)
      this.weakLineTracker.Deregister();
    this.isDisposed = true;
  }

  void ILineTracker.BeforeRemoveLine(DocumentLine line)
  {
    this.CheckIsHighlighting();
    int lineNumber = line.LineNumber;
    this.storedSpanStacks.RemoveAt(lineNumber);
    this.isValid.RemoveAt(lineNumber);
    if (lineNumber >= this.isValid.Count)
      return;
    this.isValid[lineNumber] = false;
    if (lineNumber >= this.firstInvalidLine)
      return;
    this.firstInvalidLine = lineNumber;
  }

  void ILineTracker.SetLineLength(DocumentLine line, int newTotalLength)
  {
    this.CheckIsHighlighting();
    int lineNumber = line.LineNumber;
    this.isValid[lineNumber] = false;
    if (lineNumber >= this.firstInvalidLine)
      return;
    this.firstInvalidLine = lineNumber;
  }

  void ILineTracker.LineInserted(DocumentLine insertionPos, DocumentLine newLine)
  {
    this.CheckIsHighlighting();
    int lineNumber = newLine.LineNumber;
    this.storedSpanStacks.Insert(lineNumber, (ImmutableStack<HighlightingSpan>) null);
    this.isValid.Insert(lineNumber, false);
    if (lineNumber >= this.firstInvalidLine)
      return;
    this.firstInvalidLine = lineNumber;
  }

  void ILineTracker.RebuildDocument() => this.InvalidateSpanStacks();

  void ILineTracker.ChangeComplete(DocumentChangeEventArgs e)
  {
  }

  public ImmutableStack<HighlightingSpan> InitialSpanStack
  {
    get => this.initialSpanStack;
    set
    {
      this.initialSpanStack = value ?? ImmutableStack<HighlightingSpan>.Empty;
      this.InvalidateHighlighting();
    }
  }

  public void InvalidateHighlighting()
  {
    this.InvalidateSpanStacks();
    this.OnHighlightStateChanged(1, this.document.LineCount);
  }

  private void InvalidateSpanStacks()
  {
    this.CheckIsHighlighting();
    this.storedSpanStacks.Clear();
    this.storedSpanStacks.Add(this.initialSpanStack);
    this.storedSpanStacks.InsertRange(1, this.document.LineCount, (ImmutableStack<HighlightingSpan>) null);
    this.isValid.Clear();
    this.isValid.Add(true);
    this.isValid.InsertRange(1, this.document.LineCount, false);
    this.firstInvalidLine = 1;
  }

  public HighlightedLine HighlightLine(int lineNumber)
  {
    ThrowUtil.CheckInRangeInclusive(lineNumber, nameof (lineNumber), 1, this.document.LineCount);
    this.CheckIsHighlighting();
    this.isHighlighting = true;
    try
    {
      this.HighlightUpTo(lineNumber - 1);
      HighlightedLine highlightedLine = this.engine.HighlightLine(this.document, this.document.GetLineByNumber(lineNumber));
      this.UpdateTreeList(lineNumber);
      return highlightedLine;
    }
    finally
    {
      this.isHighlighting = false;
    }
  }

  public ImmutableStack<HighlightingSpan> GetSpanStack(int lineNumber)
  {
    ThrowUtil.CheckInRangeInclusive(lineNumber, nameof (lineNumber), 0, this.document.LineCount);
    if (this.firstInvalidLine <= lineNumber)
      this.UpdateHighlightingState(lineNumber);
    return this.storedSpanStacks[lineNumber];
  }

  public IEnumerable<HighlightingColor> GetColorStack(int lineNumber)
  {
    return this.GetSpanStack(lineNumber).Select<HighlightingSpan, HighlightingColor>((Func<HighlightingSpan, HighlightingColor>) (s => s.SpanColor)).Where<HighlightingColor>((Func<HighlightingColor, bool>) (s => s != null));
  }

  private void CheckIsHighlighting()
  {
    if (this.isDisposed)
      throw new ObjectDisposedException(nameof (DocumentHighlighter));
    if (this.isHighlighting)
      throw new InvalidOperationException("Invalid call - a highlighting operation is currently running.");
  }

  public void UpdateHighlightingState(int lineNumber)
  {
    this.CheckIsHighlighting();
    this.isHighlighting = true;
    try
    {
      this.HighlightUpTo(lineNumber);
    }
    finally
    {
      this.isHighlighting = false;
    }
  }

  private void HighlightUpTo(int targetLineNumber)
  {
    for (int lineNumber = 0; lineNumber <= targetLineNumber; ++lineNumber)
    {
      if (this.firstInvalidLine > lineNumber)
      {
        if (this.firstInvalidLine <= targetLineNumber)
        {
          this.engine.CurrentSpanStack = this.storedSpanStacks[this.firstInvalidLine - 1];
          lineNumber = this.firstInvalidLine;
        }
        else
        {
          this.engine.CurrentSpanStack = this.storedSpanStacks[targetLineNumber];
          break;
        }
      }
      this.engine.ScanLine(this.document, this.document.GetLineByNumber(lineNumber));
      this.UpdateTreeList(lineNumber);
    }
  }

  private void UpdateTreeList(int lineNumber)
  {
    if (!DocumentHighlighter.EqualSpanStacks(this.engine.CurrentSpanStack, this.storedSpanStacks[lineNumber]))
    {
      this.isValid[lineNumber] = true;
      this.storedSpanStacks[lineNumber] = this.engine.CurrentSpanStack;
      if (lineNumber + 1 < this.isValid.Count)
      {
        this.isValid[lineNumber + 1] = false;
        this.firstInvalidLine = lineNumber + 1;
      }
      else
        this.firstInvalidLine = int.MaxValue;
      if (lineNumber + 1 >= this.document.LineCount)
        return;
      this.OnHighlightStateChanged(lineNumber + 1, lineNumber + 1);
    }
    else
    {
      if (this.firstInvalidLine != lineNumber)
        return;
      this.isValid[lineNumber] = true;
      this.firstInvalidLine = this.isValid.IndexOf(false);
      if (this.firstInvalidLine >= 0)
        return;
      this.firstInvalidLine = int.MaxValue;
    }
  }

  private static bool EqualSpanStacks(
    ImmutableStack<HighlightingSpan> a,
    ImmutableStack<HighlightingSpan> b)
  {
    if (a == b)
      return true;
    if (a == null || b == null)
      return false;
    while (!a.IsEmpty && !b.IsEmpty)
    {
      if (a.Peek() != b.Peek())
        return false;
      a = a.Pop();
      b = b.Pop();
      if (a == b)
        return true;
    }
    return a.IsEmpty && b.IsEmpty;
  }

  public event HighlightingStateChangedEventHandler HighlightingStateChanged;

  protected virtual void OnHighlightStateChanged(int fromLineNumber, int toLineNumber)
  {
    if (this.HighlightingStateChanged == null)
      return;
    this.HighlightingStateChanged(fromLineNumber, toLineNumber);
  }

  public HighlightingColor DefaultTextColor => (HighlightingColor) null;

  public void BeginHighlighting()
  {
    this.isInHighlightingGroup = !this.isInHighlightingGroup ? true : throw new InvalidOperationException("Highlighting group is already open");
  }

  public void EndHighlighting()
  {
    this.isInHighlightingGroup = this.isInHighlightingGroup ? false : throw new InvalidOperationException("Highlighting group is not open");
  }

  public HighlightingColor GetNamedColor(string name) => this.definition.GetNamedColor(name);
}
