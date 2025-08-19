// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.TextDocument
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public sealed class TextDocument : IDocument, ITextSource, IServiceProvider, INotifyPropertyChanged
{
  private readonly object lockObject = new object();
  private Thread owner = Thread.CurrentThread;
  private readonly Rope<char> rope;
  private readonly DocumentLineTree lineTree;
  private readonly LineManager lineManager;
  private readonly TextAnchorTree anchorTree;
  private readonly TextSourceVersionProvider versionProvider = new TextSourceVersionProvider();
  private WeakReference cachedText;
  private int beginUpdateCount;
  private int oldTextLength;
  private int oldLineCount;
  private bool fireTextChanged;
  internal bool inDocumentChanging;
  private readonly ObservableCollection<ILineTracker> lineTrackers = new ObservableCollection<ILineTracker>();
  private UndoStack undoStack;
  private IServiceProvider serviceProvider;
  private string fileName;

  public void VerifyAccess()
  {
    if (Thread.CurrentThread != this.owner)
      throw new InvalidOperationException("TextDocument can be accessed only from the thread that owns it.");
  }

  public void SetOwnerThread(Thread newOwner)
  {
    lock (this.lockObject)
    {
      if (this.owner != null)
        this.VerifyAccess();
      this.owner = newOwner;
    }
  }

  public TextDocument()
    : this((IEnumerable<char>) string.Empty)
  {
  }

  public TextDocument(IEnumerable<char> initialText)
  {
    this.rope = initialText != null ? new Rope<char>(initialText) : throw new ArgumentNullException(nameof (initialText));
    this.lineTree = new DocumentLineTree(this);
    this.lineManager = new LineManager(this.lineTree, this);
    this.lineTrackers.CollectionChanged += (NotifyCollectionChangedEventHandler) ((param0, param1) => this.lineManager.UpdateListOfLineTrackers());
    this.anchorTree = new TextAnchorTree(this);
    this.undoStack = new UndoStack();
    this.FireChangeEvents();
  }

  public TextDocument(ITextSource initialText)
    : this(TextDocument.GetTextFromTextSource(initialText))
  {
  }

  private static IEnumerable<char> GetTextFromTextSource(ITextSource textSource)
  {
    switch (textSource)
    {
      case null:
        throw new ArgumentNullException(nameof (textSource));
      case RopeTextSource ropeTextSource:
        return (IEnumerable<char>) ropeTextSource.GetRope();
      case TextDocument textDocument:
        return (IEnumerable<char>) textDocument.rope;
      default:
        return (IEnumerable<char>) textSource.Text;
    }
  }

  private void ThrowIfRangeInvalid(int offset, int length)
  {
    if (offset < 0 || offset > this.rope.Length)
      throw new ArgumentOutOfRangeException(nameof (offset), (object) offset, "0 <= offset <= " + this.rope.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (length < 0 || offset + length > this.rope.Length)
      throw new ArgumentOutOfRangeException(nameof (length), (object) length, $"0 <= length, offset({(object) offset})+length <= {this.rope.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture)}");
  }

  public string GetText(int offset, int length)
  {
    this.VerifyAccess();
    return this.rope.ToString(offset, length);
  }

  public string GetText(ISegment segment)
  {
    if (segment == null)
      throw new ArgumentNullException(nameof (segment));
    return this.GetText(segment.Offset, segment.Length);
  }

  public int IndexOf(char c, int startIndex, int count) => this.rope.IndexOf(c, startIndex, count);

  public int LastIndexOf(char c, int startIndex, int count)
  {
    return this.rope.LastIndexOf(c, startIndex, count);
  }

  public int IndexOfAny(char[] anyOf, int startIndex, int count)
  {
    return this.rope.IndexOfAny(anyOf, startIndex, count);
  }

  public int IndexOf(
    string searchText,
    int startIndex,
    int count,
    StringComparison comparisonType)
  {
    return this.rope.IndexOf(searchText, startIndex, count, comparisonType);
  }

  public int LastIndexOf(
    string searchText,
    int startIndex,
    int count,
    StringComparison comparisonType)
  {
    return this.rope.LastIndexOf(searchText, startIndex, count, comparisonType);
  }

  public char GetCharAt(int offset) => this.rope[offset];

  public string Text
  {
    get
    {
      this.VerifyAccess();
      string target = this.cachedText != null ? this.cachedText.Target as string : (string) null;
      if (target == null)
      {
        target = this.rope.ToString();
        this.cachedText = new WeakReference((object) target);
      }
      return target;
    }
    set
    {
      this.VerifyAccess();
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      this.Replace(0, this.rope.Length, value);
    }
  }

  public event EventHandler TextChanged;

  event EventHandler IDocument.ChangeCompleted
  {
    add => this.TextChanged += value;
    remove => this.TextChanged -= value;
  }

  public int TextLength
  {
    get
    {
      this.VerifyAccess();
      return this.rope.Length;
    }
  }

  [Obsolete("This event will be removed in a future version; use the PropertyChanged event instead")]
  public event EventHandler TextLengthChanged;

  public event PropertyChangedEventHandler PropertyChanged;

  public event EventHandler<DocumentChangeEventArgs> Changing;

  private event EventHandler<TextChangeEventArgs> textChanging;

  event EventHandler<TextChangeEventArgs> IDocument.TextChanging
  {
    add => this.textChanging += value;
    remove => this.textChanging -= value;
  }

  public event EventHandler<DocumentChangeEventArgs> Changed;

  private event EventHandler<TextChangeEventArgs> textChanged;

  event EventHandler<TextChangeEventArgs> IDocument.TextChanged
  {
    add => this.textChanged += value;
    remove => this.textChanged -= value;
  }

  public ITextSource CreateSnapshot()
  {
    lock (this.lockObject)
      return (ITextSource) new RopeTextSource(this.rope, this.versionProvider.CurrentVersion);
  }

  public ITextSource CreateSnapshot(int offset, int length)
  {
    lock (this.lockObject)
      return (ITextSource) new RopeTextSource(this.rope.GetRange(offset, length));
  }

  public ITextSourceVersion Version => this.versionProvider.CurrentVersion;

  public TextReader CreateReader()
  {
    lock (this.lockObject)
      return (TextReader) new RopeTextReader(this.rope);
  }

  public TextReader CreateReader(int offset, int length)
  {
    lock (this.lockObject)
      return (TextReader) new RopeTextReader(this.rope.GetRange(offset, length));
  }

  public void WriteTextTo(TextWriter writer)
  {
    this.VerifyAccess();
    this.rope.WriteTo(writer, 0, this.rope.Length);
  }

  public void WriteTextTo(TextWriter writer, int offset, int length)
  {
    this.VerifyAccess();
    this.rope.WriteTo(writer, offset, length);
  }

  public bool IsInUpdate
  {
    get
    {
      this.VerifyAccess();
      return this.beginUpdateCount > 0;
    }
  }

  public IDisposable RunUpdate()
  {
    this.BeginUpdate();
    return (IDisposable) new CallbackOnDispose(new Action(this.EndUpdate));
  }

  public void BeginUpdate()
  {
    this.VerifyAccess();
    if (this.inDocumentChanging)
      throw new InvalidOperationException("Cannot change document within another document change.");
    ++this.beginUpdateCount;
    if (this.beginUpdateCount != 1)
      return;
    this.undoStack.StartUndoGroup();
    if (this.UpdateStarted == null)
      return;
    this.UpdateStarted((object) this, EventArgs.Empty);
  }

  public void EndUpdate()
  {
    this.VerifyAccess();
    if (this.inDocumentChanging)
      throw new InvalidOperationException("Cannot end update within document change.");
    if (this.beginUpdateCount == 0)
      throw new InvalidOperationException("No update is active.");
    if (this.beginUpdateCount == 1)
    {
      this.FireChangeEvents();
      this.undoStack.EndUndoGroup();
      this.beginUpdateCount = 0;
      if (this.UpdateFinished == null)
        return;
      this.UpdateFinished((object) this, EventArgs.Empty);
    }
    else
      --this.beginUpdateCount;
  }

  public event EventHandler UpdateStarted;

  public event EventHandler UpdateFinished;

  void IDocument.StartUndoableAction() => this.BeginUpdate();

  void IDocument.EndUndoableAction() => this.EndUpdate();

  IDisposable IDocument.OpenUndoGroup() => this.RunUpdate();

  internal void FireChangeEvents()
  {
    while (this.fireTextChanged)
    {
      this.fireTextChanged = false;
      if (this.TextChanged != null)
        this.TextChanged((object) this, EventArgs.Empty);
      this.OnPropertyChanged("Text");
      int length = this.rope.Length;
      if (length != this.oldTextLength)
      {
        this.oldTextLength = length;
        if (this.TextLengthChanged != null)
          this.TextLengthChanged((object) this, EventArgs.Empty);
        this.OnPropertyChanged("TextLength");
      }
      int lineCount = this.lineTree.LineCount;
      if (lineCount != this.oldLineCount)
      {
        this.oldLineCount = lineCount;
        if (this.LineCountChanged != null)
          this.LineCountChanged((object) this, EventArgs.Empty);
        this.OnPropertyChanged("LineCount");
      }
    }
  }

  private void OnPropertyChanged(string propertyName)
  {
    if (this.PropertyChanged == null)
      return;
    this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
  }

  public void Insert(int offset, string text)
  {
    this.Replace(offset, 0, (ITextSource) new StringTextSource(text), (OffsetChangeMap) null);
  }

  public void Insert(int offset, ITextSource text)
  {
    this.Replace(offset, 0, text, (OffsetChangeMap) null);
  }

  public void Insert(int offset, string text, AnchorMovementType defaultAnchorMovementType)
  {
    if (defaultAnchorMovementType == AnchorMovementType.BeforeInsertion)
      this.Replace(offset, 0, (ITextSource) new StringTextSource(text), OffsetChangeMappingType.KeepAnchorBeforeInsertion);
    else
      this.Replace(offset, 0, (ITextSource) new StringTextSource(text), (OffsetChangeMap) null);
  }

  public void Insert(int offset, ITextSource text, AnchorMovementType defaultAnchorMovementType)
  {
    if (defaultAnchorMovementType == AnchorMovementType.BeforeInsertion)
      this.Replace(offset, 0, text, OffsetChangeMappingType.KeepAnchorBeforeInsertion);
    else
      this.Replace(offset, 0, text, (OffsetChangeMap) null);
  }

  public void Remove(ISegment segment) => this.Replace(segment, string.Empty);

  public void Remove(int offset, int length)
  {
    this.Replace(offset, length, (ITextSource) StringTextSource.Empty);
  }

  public void Replace(ISegment segment, string text)
  {
    if (segment == null)
      throw new ArgumentNullException(nameof (segment));
    this.Replace(segment.Offset, segment.Length, (ITextSource) new StringTextSource(text), (OffsetChangeMap) null);
  }

  public void Replace(ISegment segment, ITextSource text)
  {
    if (segment == null)
      throw new ArgumentNullException(nameof (segment));
    this.Replace(segment.Offset, segment.Length, text, (OffsetChangeMap) null);
  }

  public void Replace(int offset, int length, string text)
  {
    this.Replace(offset, length, (ITextSource) new StringTextSource(text), (OffsetChangeMap) null);
  }

  public void Replace(int offset, int length, ITextSource text)
  {
    this.Replace(offset, length, text, (OffsetChangeMap) null);
  }

  public void Replace(
    int offset,
    int length,
    string text,
    OffsetChangeMappingType offsetChangeMappingType)
  {
    this.Replace(offset, length, (ITextSource) new StringTextSource(text), offsetChangeMappingType);
  }

  public void Replace(
    int offset,
    int length,
    ITextSource text,
    OffsetChangeMappingType offsetChangeMappingType)
  {
    if (text == null)
      throw new ArgumentNullException(nameof (text));
    switch (offsetChangeMappingType)
    {
      case OffsetChangeMappingType.Normal:
        this.Replace(offset, length, text, (OffsetChangeMap) null);
        break;
      case OffsetChangeMappingType.RemoveAndInsert:
        if (length == 0 || text.TextLength == 0)
        {
          this.Replace(offset, length, text, (OffsetChangeMap) null);
          break;
        }
        OffsetChangeMap offsetChangeMap = new OffsetChangeMap(2);
        offsetChangeMap.Add(new OffsetChangeMapEntry(offset, length, 0));
        offsetChangeMap.Add(new OffsetChangeMapEntry(offset, 0, text.TextLength));
        offsetChangeMap.Freeze();
        this.Replace(offset, length, text, offsetChangeMap);
        break;
      case OffsetChangeMappingType.CharacterReplace:
        if (length == 0 || text.TextLength == 0)
        {
          this.Replace(offset, length, text, (OffsetChangeMap) null);
          break;
        }
        if (text.TextLength > length)
        {
          OffsetChangeMapEntry entry = new OffsetChangeMapEntry(offset + length - 1, 1, 1 + text.TextLength - length);
          this.Replace(offset, length, text, OffsetChangeMap.FromSingleElement(entry));
          break;
        }
        if (text.TextLength < length)
        {
          OffsetChangeMapEntry entry = new OffsetChangeMapEntry(offset + text.TextLength, length - text.TextLength, 0, true, false);
          this.Replace(offset, length, text, OffsetChangeMap.FromSingleElement(entry));
          break;
        }
        this.Replace(offset, length, text, OffsetChangeMap.Empty);
        break;
      case OffsetChangeMappingType.KeepAnchorBeforeInsertion:
        this.Replace(offset, length, text, OffsetChangeMap.FromSingleElement(new OffsetChangeMapEntry(offset, length, text.TextLength, false, true)));
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof (offsetChangeMappingType), (object) offsetChangeMappingType, "Invalid enum value");
    }
  }

  public void Replace(int offset, int length, string text, OffsetChangeMap offsetChangeMap)
  {
    this.Replace(offset, length, (ITextSource) new StringTextSource(text), offsetChangeMap);
  }

  public void Replace(int offset, int length, ITextSource text, OffsetChangeMap offsetChangeMap)
  {
    text = text != null ? text.CreateSnapshot() : throw new ArgumentNullException(nameof (text));
    offsetChangeMap?.Freeze();
    this.BeginUpdate();
    try
    {
      this.inDocumentChanging = true;
      try
      {
        this.ThrowIfRangeInvalid(offset, length);
        this.DoReplace(offset, length, text, offsetChangeMap);
      }
      finally
      {
        this.inDocumentChanging = false;
      }
    }
    finally
    {
      this.EndUpdate();
    }
  }

  private void DoReplace(
    int offset,
    int length,
    ITextSource newText,
    OffsetChangeMap offsetChangeMap)
  {
    if (length == 0 && newText.TextLength == 0)
      return;
    if (length == 1 && newText.TextLength == 1 && offsetChangeMap == null)
      offsetChangeMap = OffsetChangeMap.Empty;
    ITextSource removedText = length != 0 ? (length >= 100 ? (ITextSource) new RopeTextSource(this.rope.GetRange(offset, length)) : (ITextSource) new StringTextSource(this.rope.ToString(offset, length))) : (ITextSource) StringTextSource.Empty;
    DocumentChangeEventArgs documentChangeEventArgs = new DocumentChangeEventArgs(offset, removedText, newText, offsetChangeMap);
    if (this.Changing != null)
      this.Changing((object) this, documentChangeEventArgs);
    if (this.textChanging != null)
      this.textChanging((object) this, (TextChangeEventArgs) documentChangeEventArgs);
    this.undoStack.Push(this, documentChangeEventArgs);
    this.cachedText = (WeakReference) null;
    this.fireTextChanged = true;
    DelayedEvents delayedEvents = new DelayedEvents();
    lock (this.lockObject)
    {
      this.versionProvider.AppendChange((TextChangeEventArgs) documentChangeEventArgs);
      if (offset == 0 && length == this.rope.Length)
      {
        this.rope.Clear();
        if (newText is RopeTextSource ropeTextSource)
          this.rope.InsertRange(0, ropeTextSource.GetRope());
        else
          this.rope.InsertText(0, newText.Text);
        this.lineManager.Rebuild();
      }
      else
      {
        this.rope.RemoveRange(offset, length);
        this.lineManager.Remove(offset, length);
        if (newText is RopeTextSource ropeTextSource)
          this.rope.InsertRange(offset, ropeTextSource.GetRope());
        else
          this.rope.InsertText(offset, newText.Text);
        this.lineManager.Insert(offset, newText);
      }
    }
    if (offsetChangeMap == null)
    {
      this.anchorTree.HandleTextChange(documentChangeEventArgs.CreateSingleChangeMapEntry(), delayedEvents);
    }
    else
    {
      foreach (OffsetChangeMapEntry offsetChange in (Collection<OffsetChangeMapEntry>) offsetChangeMap)
        this.anchorTree.HandleTextChange(offsetChange, delayedEvents);
    }
    this.lineManager.ChangeComplete(documentChangeEventArgs);
    delayedEvents.RaiseEvents();
    if (this.Changed != null)
      this.Changed((object) this, documentChangeEventArgs);
    if (this.textChanged == null)
      return;
    this.textChanged((object) this, (TextChangeEventArgs) documentChangeEventArgs);
  }

  public IList<DocumentLine> Lines => (IList<DocumentLine>) this.lineTree;

  public DocumentLine GetLineByNumber(int number)
  {
    this.VerifyAccess();
    if (number < 1 || number > this.lineTree.LineCount)
      throw new ArgumentOutOfRangeException(nameof (number), (object) number, "Value must be between 1 and " + (object) this.lineTree.LineCount);
    return this.lineTree.GetByNumber(number);
  }

  IDocumentLine IDocument.GetLineByNumber(int lineNumber)
  {
    return (IDocumentLine) this.GetLineByNumber(lineNumber);
  }

  public DocumentLine GetLineByOffset(int offset)
  {
    this.VerifyAccess();
    if (offset < 0 || offset > this.rope.Length)
      throw new ArgumentOutOfRangeException(nameof (offset), (object) offset, "0 <= offset <= " + this.rope.Length.ToString());
    return this.lineTree.GetByOffset(offset);
  }

  IDocumentLine IDocument.GetLineByOffset(int offset)
  {
    return (IDocumentLine) this.GetLineByOffset(offset);
  }

  public int GetOffset(TextLocation location) => this.GetOffset(location.Line, location.Column);

  public int GetOffset(int line, int column)
  {
    DocumentLine lineByNumber = this.GetLineByNumber(line);
    if (column <= 0)
      return lineByNumber.Offset;
    return column > lineByNumber.Length ? lineByNumber.EndOffset : lineByNumber.Offset + column - 1;
  }

  public TextLocation GetLocation(int offset)
  {
    DocumentLine lineByOffset = this.GetLineByOffset(offset);
    return new TextLocation(lineByOffset.LineNumber, offset - lineByOffset.Offset + 1);
  }

  public IList<ILineTracker> LineTrackers
  {
    get
    {
      this.VerifyAccess();
      return (IList<ILineTracker>) this.lineTrackers;
    }
  }

  public UndoStack UndoStack
  {
    get => this.undoStack;
    set
    {
      if (value == null)
        throw new ArgumentNullException();
      if (value == this.undoStack)
        return;
      this.undoStack.ClearAll();
      this.undoStack = value;
      this.OnPropertyChanged(nameof (UndoStack));
    }
  }

  public TextAnchor CreateAnchor(int offset)
  {
    this.VerifyAccess();
    if (offset < 0 || offset > this.rope.Length)
      throw new ArgumentOutOfRangeException(nameof (offset), (object) offset, "0 <= offset <= " + this.rope.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    return this.anchorTree.CreateAnchor(offset);
  }

  ITextAnchor IDocument.CreateAnchor(int offset) => (ITextAnchor) this.CreateAnchor(offset);

  public int LineCount
  {
    get
    {
      this.VerifyAccess();
      return this.lineTree.LineCount;
    }
  }

  [Obsolete("This event will be removed in a future version; use the PropertyChanged event instead")]
  public event EventHandler LineCountChanged;

  [Conditional("DEBUG")]
  internal void DebugVerifyAccess() => this.VerifyAccess();

  internal string GetLineTreeAsString() => "Not available in release build.";

  internal string GetTextAnchorTreeAsString() => "Not available in release build.";

  public IServiceProvider ServiceProvider
  {
    get
    {
      this.VerifyAccess();
      if (this.serviceProvider == null)
      {
        ServiceContainer serviceContainer = new ServiceContainer();
        serviceContainer.AddService(typeof (IDocument), (object) this);
        serviceContainer.AddService(typeof (TextDocument), (object) this);
        this.serviceProvider = (IServiceProvider) serviceContainer;
      }
      return this.serviceProvider;
    }
    set
    {
      this.VerifyAccess();
      this.serviceProvider = value != null ? value : throw new ArgumentNullException();
    }
  }

  object IServiceProvider.GetService(Type serviceType)
  {
    return this.ServiceProvider.GetService(serviceType);
  }

  public event EventHandler FileNameChanged;

  private void OnFileNameChanged(EventArgs e)
  {
    EventHandler fileNameChanged = this.FileNameChanged;
    if (fileNameChanged == null)
      return;
    fileNameChanged((object) this, e);
  }

  public string FileName
  {
    get => this.fileName;
    set
    {
      if (!(this.fileName != value))
        return;
      this.fileName = value;
      this.OnFileNameChanged(EventArgs.Empty);
    }
  }
}
