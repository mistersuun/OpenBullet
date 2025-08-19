// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Snippets.InsertionContext
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Windows;

#nullable disable
namespace ICSharpCode.AvalonEdit.Snippets;

public class InsertionContext : IWeakEventListener
{
  private InsertionContext.Status currentStatus;
  private readonly int startPosition;
  private AnchorSegment wholeSnippetAnchor;
  private bool deactivateIfSnippetEmpty;
  private Dictionary<SnippetElement, IActiveElement> elementMap = new Dictionary<SnippetElement, IActiveElement>();
  private List<IActiveElement> registeredElements = new List<IActiveElement>();
  private SnippetInputHandler myInputHandler;

  public InsertionContext(TextArea textArea, int insertionPosition)
  {
    this.TextArea = textArea != null ? textArea : throw new ArgumentNullException(nameof (textArea));
    this.Document = textArea.Document;
    this.SelectedText = textArea.Selection.GetText();
    this.InsertionPosition = insertionPosition;
    this.startPosition = insertionPosition;
    DocumentLine lineByOffset = this.Document.GetLineByOffset(insertionPosition);
    ISegment whitespaceAfter = TextUtilities.GetWhitespaceAfter((ITextSource) this.Document, lineByOffset.Offset);
    this.Indentation = this.Document.GetText(whitespaceAfter.Offset, Math.Min(whitespaceAfter.EndOffset, insertionPosition) - whitespaceAfter.Offset);
    this.Tab = textArea.Options.IndentationString;
    this.LineTerminator = TextUtilities.GetNewLineFromDocument((IDocument) this.Document, lineByOffset.LineNumber);
  }

  public TextArea TextArea { get; private set; }

  public TextDocument Document { get; private set; }

  public string SelectedText { get; private set; }

  public string Indentation { get; private set; }

  public string Tab { get; private set; }

  public string LineTerminator { get; private set; }

  public int InsertionPosition { get; set; }

  public int StartPosition
  {
    get => this.wholeSnippetAnchor != null ? this.wholeSnippetAnchor.Offset : this.startPosition;
  }

  public void InsertText(string text)
  {
    if (text == null)
      throw new ArgumentNullException(nameof (text));
    if (this.currentStatus != InsertionContext.Status.Insertion)
      throw new InvalidOperationException();
    text = text.Replace("\t", this.Tab);
    using (this.Document.RunUpdate())
    {
      int num;
      SimpleSegment simpleSegment;
      for (num = 0; (simpleSegment = NewLineFinder.NextNewLine(text, num)) != SimpleSegment.Invalid; num = simpleSegment.EndOffset)
      {
        string text1 = text.Substring(num, simpleSegment.Offset - num) + this.LineTerminator + this.Indentation;
        this.Document.Insert(this.InsertionPosition, text1);
        this.InsertionPosition += text1.Length;
      }
      string text2 = text.Substring(num);
      this.Document.Insert(this.InsertionPosition, text2);
      this.InsertionPosition += text2.Length;
    }
  }

  public void RegisterActiveElement(SnippetElement owner, IActiveElement element)
  {
    if (owner == null)
      throw new ArgumentNullException(nameof (owner));
    if (element == null)
      throw new ArgumentNullException(nameof (element));
    if (this.currentStatus != InsertionContext.Status.Insertion)
      throw new InvalidOperationException();
    this.elementMap.Add(owner, element);
    this.registeredElements.Add(element);
  }

  public IActiveElement GetActiveElement(SnippetElement owner)
  {
    if (owner == null)
      throw new ArgumentNullException(nameof (owner));
    IActiveElement activeElement;
    return this.elementMap.TryGetValue(owner, out activeElement) ? activeElement : (IActiveElement) null;
  }

  public IEnumerable<IActiveElement> ActiveElements
  {
    get => (IEnumerable<IActiveElement>) this.registeredElements;
  }

  public void RaiseInsertionCompleted(EventArgs e)
  {
    if (this.currentStatus != InsertionContext.Status.Insertion)
      throw new InvalidOperationException();
    if (e == null)
      e = EventArgs.Empty;
    this.currentStatus = InsertionContext.Status.RaisingInsertionCompleted;
    int insertionPosition = this.InsertionPosition;
    this.wholeSnippetAnchor = new AnchorSegment(this.Document, this.startPosition, insertionPosition - this.startPosition);
    WeakEventManagerBase<TextDocumentWeakEventManager.UpdateFinished, TextDocument>.AddListener(this.Document, (IWeakEventListener) this);
    this.deactivateIfSnippetEmpty = insertionPosition != this.startPosition;
    foreach (IActiveElement registeredElement in this.registeredElements)
      registeredElement.OnInsertionCompleted();
    if (this.InsertionCompleted != null)
      this.InsertionCompleted((object) this, e);
    this.currentStatus = InsertionContext.Status.Interactive;
    if (this.registeredElements.Count == 0)
    {
      this.Deactivate(new SnippetEventArgs(DeactivateReason.NoActiveElements));
    }
    else
    {
      this.myInputHandler = new SnippetInputHandler(this);
      foreach (TextAreaStackedInputHandler stackedInputHandler in this.TextArea.StackedInputHandlers)
      {
        if (stackedInputHandler is SnippetInputHandler)
          this.TextArea.PopStackedInputHandler(stackedInputHandler);
      }
      this.TextArea.PushStackedInputHandler((TextAreaStackedInputHandler) this.myInputHandler);
    }
  }

  public event EventHandler InsertionCompleted;

  public void Deactivate(SnippetEventArgs e)
  {
    if (this.currentStatus == InsertionContext.Status.Deactivated || this.currentStatus == InsertionContext.Status.RaisingDeactivated)
      return;
    if (this.currentStatus != InsertionContext.Status.Interactive)
      throw new InvalidOperationException("Cannot call Deactivate() until RaiseInsertionCompleted() has finished.");
    if (e == null)
      e = new SnippetEventArgs(DeactivateReason.Unknown);
    WeakEventManagerBase<TextDocumentWeakEventManager.UpdateFinished, TextDocument>.RemoveListener(this.Document, (IWeakEventListener) this);
    this.currentStatus = InsertionContext.Status.RaisingDeactivated;
    this.TextArea.PopStackedInputHandler((TextAreaStackedInputHandler) this.myInputHandler);
    foreach (IActiveElement registeredElement in this.registeredElements)
      registeredElement.Deactivate(e);
    if (this.Deactivated != null)
      this.Deactivated((object) this, e);
    this.currentStatus = InsertionContext.Status.Deactivated;
  }

  public event EventHandler<SnippetEventArgs> Deactivated;

  bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    return this.ReceiveWeakEvent(managerType, sender, e);
  }

  protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    if (!(managerType == typeof (TextDocumentWeakEventManager.UpdateFinished)))
      return false;
    if (this.wholeSnippetAnchor.Length == 0 && this.deactivateIfSnippetEmpty)
      this.Deactivate(new SnippetEventArgs(DeactivateReason.Deleted));
    return true;
  }

  public void Link(ISegment mainElement, ISegment[] boundElements)
  {
    SnippetReplaceableTextElement replaceableTextElement1 = new SnippetReplaceableTextElement();
    replaceableTextElement1.Text = this.Document.GetText(mainElement);
    SnippetReplaceableTextElement replaceableTextElement2 = replaceableTextElement1;
    this.RegisterActiveElement((SnippetElement) replaceableTextElement2, (IActiveElement) new ReplaceableActiveElement(this, mainElement.Offset, mainElement.EndOffset));
    foreach (ISegment boundElement in boundElements)
    {
      SnippetBoundElement snippetBoundElement = new SnippetBoundElement()
      {
        TargetElement = replaceableTextElement2
      };
      TextAnchor anchor1 = this.Document.CreateAnchor(boundElement.Offset);
      anchor1.MovementType = AnchorMovementType.BeforeInsertion;
      anchor1.SurviveDeletion = true;
      TextAnchor anchor2 = this.Document.CreateAnchor(boundElement.EndOffset);
      anchor2.MovementType = AnchorMovementType.BeforeInsertion;
      anchor2.SurviveDeletion = true;
      this.RegisterActiveElement((SnippetElement) snippetBoundElement, (IActiveElement) new BoundActiveElement(this, replaceableTextElement2, snippetBoundElement, new AnchorSegment(anchor1, anchor2)));
    }
  }

  private enum Status
  {
    Insertion,
    RaisingInsertionCompleted,
    Interactive,
    RaisingDeactivated,
    Deactivated,
  }
}
