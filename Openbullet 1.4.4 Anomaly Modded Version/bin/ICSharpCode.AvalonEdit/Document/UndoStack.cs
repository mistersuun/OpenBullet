// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.UndoStack
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public sealed class UndoStack : INotifyPropertyChanged
{
  internal const int StateListen = 0;
  internal const int StatePlayback = 1;
  internal const int StatePlaybackModifyDocument = 2;
  internal int state;
  private Deque<IUndoableOperation> undostack = new Deque<IUndoableOperation>();
  private Deque<IUndoableOperation> redostack = new Deque<IUndoableOperation>();
  private int sizeLimit = int.MaxValue;
  private int undoGroupDepth;
  private int actionCountInUndoGroup;
  private int optionalActionCount;
  private object lastGroupDescriptor;
  private bool allowContinue;
  private int elementsOnUndoUntilOriginalFile;
  private bool isOriginalFile = true;
  private List<TextDocument> affectedDocuments;

  public bool IsOriginalFile => this.isOriginalFile;

  private void RecalcIsOriginalFile()
  {
    bool flag = this.elementsOnUndoUntilOriginalFile == 0;
    if (flag == this.isOriginalFile)
      return;
    this.isOriginalFile = flag;
    this.NotifyPropertyChanged("IsOriginalFile");
  }

  public void MarkAsOriginalFile()
  {
    this.elementsOnUndoUntilOriginalFile = 0;
    this.RecalcIsOriginalFile();
  }

  public void DiscardOriginalFileMarker()
  {
    this.elementsOnUndoUntilOriginalFile = int.MinValue;
    this.RecalcIsOriginalFile();
  }

  private void FileModified(int newElementsOnUndoStack)
  {
    if (this.elementsOnUndoUntilOriginalFile == int.MinValue)
      return;
    this.elementsOnUndoUntilOriginalFile += newElementsOnUndoStack;
    if (this.elementsOnUndoUntilOriginalFile <= this.undostack.Count)
      return;
    this.elementsOnUndoUntilOriginalFile = int.MinValue;
  }

  public bool AcceptChanges => this.state == 0;

  public bool CanUndo => this.undostack.Count > 0;

  public bool CanRedo => this.redostack.Count > 0;

  public int SizeLimit
  {
    get => this.sizeLimit;
    set
    {
      if (value < 0)
        ThrowUtil.CheckNotNegative(value, nameof (value));
      if (this.sizeLimit == value)
        return;
      this.sizeLimit = value;
      this.NotifyPropertyChanged(nameof (SizeLimit));
      if (this.undoGroupDepth != 0)
        return;
      this.EnforceSizeLimit();
    }
  }

  private void EnforceSizeLimit()
  {
    while (this.undostack.Count > this.sizeLimit)
      this.undostack.PopFront();
    while (this.redostack.Count > this.sizeLimit)
      this.redostack.PopFront();
  }

  public object LastGroupDescriptor => this.lastGroupDescriptor;

  public void StartUndoGroup() => this.StartUndoGroup((object) null);

  public void StartUndoGroup(object groupDescriptor)
  {
    if (this.undoGroupDepth == 0)
    {
      this.actionCountInUndoGroup = 0;
      this.optionalActionCount = 0;
      this.lastGroupDescriptor = groupDescriptor;
    }
    ++this.undoGroupDepth;
  }

  public void StartContinuedUndoGroup(object groupDescriptor = null)
  {
    if (this.undoGroupDepth == 0)
    {
      this.actionCountInUndoGroup = !this.allowContinue || this.undostack.Count <= 0 ? 0 : 1;
      this.optionalActionCount = 0;
      this.lastGroupDescriptor = groupDescriptor;
    }
    ++this.undoGroupDepth;
  }

  public void EndUndoGroup()
  {
    if (this.undoGroupDepth == 0)
      throw new InvalidOperationException("There are no open undo groups");
    --this.undoGroupDepth;
    if (this.undoGroupDepth != 0)
      return;
    this.allowContinue = true;
    if (this.actionCountInUndoGroup == this.optionalActionCount)
    {
      for (int index = 0; index < this.optionalActionCount; ++index)
        this.undostack.PopBack();
      this.allowContinue = false;
    }
    else if (this.actionCountInUndoGroup > 1)
    {
      this.undostack.PushBack((IUndoableOperation) new UndoOperationGroup(this.undostack, this.actionCountInUndoGroup));
      this.FileModified(-this.actionCountInUndoGroup + 1 + this.optionalActionCount);
    }
    this.EnforceSizeLimit();
    this.RecalcIsOriginalFile();
  }

  private void ThrowIfUndoGroupOpen()
  {
    if (this.undoGroupDepth != 0)
    {
      this.undoGroupDepth = 0;
      throw new InvalidOperationException("No undo group should be open at this point");
    }
    if (this.state != 0)
      throw new InvalidOperationException("This method cannot be called while an undo operation is being performed");
  }

  internal void RegisterAffectedDocument(TextDocument document)
  {
    if (this.affectedDocuments == null)
      this.affectedDocuments = new List<TextDocument>();
    if (this.affectedDocuments.Contains(document))
      return;
    this.affectedDocuments.Add(document);
    document.BeginUpdate();
  }

  private void CallEndUpdateOnAffectedDocuments()
  {
    if (this.affectedDocuments == null)
      return;
    foreach (TextDocument affectedDocument in this.affectedDocuments)
      affectedDocument.EndUpdate();
    this.affectedDocuments = (List<TextDocument>) null;
  }

  public void Undo()
  {
    this.ThrowIfUndoGroupOpen();
    if (this.undostack.Count <= 0)
      return;
    this.lastGroupDescriptor = (object) null;
    this.allowContinue = false;
    IUndoableOperation op = this.undostack.PopBack();
    this.redostack.PushBack(op);
    this.state = 1;
    try
    {
      this.RunUndo(op);
    }
    finally
    {
      this.state = 0;
      this.FileModified(-1);
      this.CallEndUpdateOnAffectedDocuments();
    }
    this.RecalcIsOriginalFile();
    if (this.undostack.Count == 0)
      this.NotifyPropertyChanged("CanUndo");
    if (this.redostack.Count != 1)
      return;
    this.NotifyPropertyChanged("CanRedo");
  }

  internal void RunUndo(IUndoableOperation op)
  {
    if (op is IUndoableOperationWithContext operationWithContext)
      operationWithContext.Undo(this);
    else
      op.Undo();
  }

  public void Redo()
  {
    this.ThrowIfUndoGroupOpen();
    if (this.redostack.Count <= 0)
      return;
    this.lastGroupDescriptor = (object) null;
    this.allowContinue = false;
    IUndoableOperation op = this.redostack.PopBack();
    this.undostack.PushBack(op);
    this.state = 1;
    try
    {
      this.RunRedo(op);
    }
    finally
    {
      this.state = 0;
      this.FileModified(1);
      this.CallEndUpdateOnAffectedDocuments();
    }
    this.RecalcIsOriginalFile();
    if (this.redostack.Count == 0)
      this.NotifyPropertyChanged("CanRedo");
    if (this.undostack.Count != 1)
      return;
    this.NotifyPropertyChanged("CanUndo");
  }

  internal void RunRedo(IUndoableOperation op)
  {
    if (op is IUndoableOperationWithContext operationWithContext)
      operationWithContext.Redo(this);
    else
      op.Redo();
  }

  public void Push(IUndoableOperation operation) => this.Push(operation, false);

  public void PushOptional(IUndoableOperation operation)
  {
    if (this.undoGroupDepth == 0)
      throw new InvalidOperationException("Cannot use PushOptional outside of undo group");
    this.Push(operation, true);
  }

  private void Push(IUndoableOperation operation, bool isOptional)
  {
    if (operation == null)
      throw new ArgumentNullException(nameof (operation));
    if (this.state != 0 || this.sizeLimit <= 0)
      return;
    bool flag1 = this.undostack.Count == 0;
    bool flag2 = this.undoGroupDepth == 0;
    if (flag2)
      this.StartUndoGroup();
    this.undostack.PushBack(operation);
    ++this.actionCountInUndoGroup;
    if (isOptional)
      ++this.optionalActionCount;
    else
      this.FileModified(1);
    if (flag2)
      this.EndUndoGroup();
    if (flag1)
      this.NotifyPropertyChanged("CanUndo");
    this.ClearRedoStack();
  }

  public void ClearRedoStack()
  {
    if (this.redostack.Count == 0)
      return;
    this.redostack.Clear();
    this.NotifyPropertyChanged("CanRedo");
    if (this.elementsOnUndoUntilOriginalFile >= 0)
      return;
    this.elementsOnUndoUntilOriginalFile = int.MinValue;
  }

  public void ClearAll()
  {
    this.ThrowIfUndoGroupOpen();
    this.actionCountInUndoGroup = 0;
    this.optionalActionCount = 0;
    if (this.undostack.Count != 0)
    {
      this.lastGroupDescriptor = (object) null;
      this.allowContinue = false;
      this.undostack.Clear();
      this.NotifyPropertyChanged("CanUndo");
    }
    this.ClearRedoStack();
  }

  internal void Push(TextDocument document, DocumentChangeEventArgs e)
  {
    if (this.state == 1)
      throw new InvalidOperationException("Document changes during undo/redo operations are not allowed.");
    if (this.state == 2)
      this.state = 1;
    else
      this.Push((IUndoableOperation) new DocumentChangeOperation(document, e));
  }

  public event PropertyChangedEventHandler PropertyChanged;

  private void NotifyPropertyChanged(string propertyName)
  {
    if (this.PropertyChanged == null)
      return;
    this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
  }
}
