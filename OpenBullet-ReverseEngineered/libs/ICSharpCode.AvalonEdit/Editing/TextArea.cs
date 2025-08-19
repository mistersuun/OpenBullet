// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.TextArea
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

public class TextArea : 
  System.Windows.Controls.Control,
  IScrollInfo,
  IWeakEventListener,
  ITextEditorComponent,
  System.IServiceProvider
{
  internal readonly ImeSupport ime;
  private ITextAreaInputHandler activeInputHandler;
  private bool isChangingInputHandler;
  private ImmutableStack<TextAreaStackedInputHandler> stackedInputHandlers = ImmutableStack<TextAreaStackedInputHandler>.Empty;
  public static readonly DependencyProperty DocumentProperty = TextView.DocumentProperty.AddOwner(typeof (TextArea), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(TextArea.OnDocumentChanged)));
  public static readonly DependencyProperty OptionsProperty = TextView.OptionsProperty.AddOwner(typeof (TextArea), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(TextArea.OnOptionsChanged)));
  private readonly TextView textView;
  private IScrollInfo scrollInfo;
  internal readonly Selection emptySelection;
  private Selection selection;
  public static readonly DependencyProperty SelectionBrushProperty = DependencyProperty.Register(nameof (SelectionBrush), typeof (Brush), typeof (TextArea));
  public static readonly DependencyProperty SelectionForegroundProperty = DependencyProperty.Register(nameof (SelectionForeground), typeof (Brush), typeof (TextArea));
  public static readonly DependencyProperty SelectionBorderProperty = DependencyProperty.Register(nameof (SelectionBorder), typeof (Pen), typeof (TextArea));
  public static readonly DependencyProperty SelectionCornerRadiusProperty = DependencyProperty.Register(nameof (SelectionCornerRadius), typeof (double), typeof (TextArea), (PropertyMetadata) new FrameworkPropertyMetadata((object) 3.0));
  private bool ensureSelectionValidRequested;
  private int allowCaretOutsideSelection;
  private readonly Caret caret;
  private ObservableCollection<UIElement> leftMargins = new ObservableCollection<UIElement>();
  private IReadOnlySectionProvider readOnlySectionProvider = (IReadOnlySectionProvider) NoReadOnlySections.Instance;
  private ScrollViewer scrollOwner;
  private bool canVerticallyScroll;
  private bool canHorizontallyScroll;
  public static readonly DependencyProperty IndentationStrategyProperty = DependencyProperty.Register(nameof (IndentationStrategy), typeof (IIndentationStrategy), typeof (TextArea), (PropertyMetadata) new FrameworkPropertyMetadata((object) new DefaultIndentationStrategy()));
  private bool isMouseCursorHidden;
  public static readonly DependencyProperty OverstrikeModeProperty = DependencyProperty.Register(nameof (OverstrikeMode), typeof (bool), typeof (TextArea), (PropertyMetadata) new FrameworkPropertyMetadata(Boxes.False));

  static TextArea()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (TextArea), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (TextArea)));
    KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof (TextArea), (PropertyMetadata) new FrameworkPropertyMetadata(Boxes.True));
    KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof (TextArea), (PropertyMetadata) new FrameworkPropertyMetadata((object) KeyboardNavigationMode.None));
    UIElement.FocusableProperty.OverrideMetadata(typeof (TextArea), (PropertyMetadata) new FrameworkPropertyMetadata(Boxes.True));
  }

  public TextArea()
    : this(new TextView())
  {
  }

  protected TextArea(TextView textView)
  {
    this.textView = textView != null ? textView : throw new ArgumentNullException(nameof (textView));
    this.Options = textView.Options;
    this.selection = this.emptySelection = (Selection) new EmptySelection(this);
    textView.Services.AddService(typeof (TextArea), (object) this);
    textView.LineTransformers.Add((IVisualLineTransformer) new SelectionColorizer(this));
    textView.InsertLayer((UIElement) new SelectionLayer(this), KnownLayer.Selection, LayerInsertionPosition.Replace);
    this.caret = new Caret(this);
    this.caret.PositionChanged += (EventHandler) ((sender, e) => this.RequestSelectionValidation());
    this.caret.PositionChanged += new EventHandler(this.CaretPositionChanged);
    this.AttachTypingEvents();
    this.ime = new ImeSupport(this);
    this.leftMargins.CollectionChanged += new NotifyCollectionChangedEventHandler(this.leftMargins_CollectionChanged);
    this.DefaultInputHandler = new TextAreaDefaultInputHandler(this);
    this.ActiveInputHandler = (ITextAreaInputHandler) this.DefaultInputHandler;
  }

  public TextAreaDefaultInputHandler DefaultInputHandler { get; private set; }

  public ITextAreaInputHandler ActiveInputHandler
  {
    get => this.activeInputHandler;
    set
    {
      if (value != null && value.TextArea != this)
        throw new ArgumentException("The input handler was created for a different text area than this one.");
      if (this.isChangingInputHandler)
        throw new InvalidOperationException("Cannot set ActiveInputHandler recursively");
      if (this.activeInputHandler == value)
        return;
      this.isChangingInputHandler = true;
      try
      {
        this.PopStackedInputHandler(this.stackedInputHandlers.LastOrDefault<TextAreaStackedInputHandler>());
        if (this.activeInputHandler != null)
          this.activeInputHandler.Detach();
        this.activeInputHandler = value;
        value?.Attach();
      }
      finally
      {
        this.isChangingInputHandler = false;
      }
      if (this.ActiveInputHandlerChanged == null)
        return;
      this.ActiveInputHandlerChanged((object) this, EventArgs.Empty);
    }
  }

  public event EventHandler ActiveInputHandlerChanged;

  public ImmutableStack<TextAreaStackedInputHandler> StackedInputHandlers
  {
    get => this.stackedInputHandlers;
  }

  public void PushStackedInputHandler(TextAreaStackedInputHandler inputHandler)
  {
    this.stackedInputHandlers = inputHandler != null ? this.stackedInputHandlers.Push(inputHandler) : throw new ArgumentNullException(nameof (inputHandler));
    inputHandler.Attach();
  }

  public void PopStackedInputHandler(TextAreaStackedInputHandler inputHandler)
  {
    if (!this.stackedInputHandlers.Any<TextAreaStackedInputHandler>((Func<TextAreaStackedInputHandler, bool>) (i => i == inputHandler)))
      return;
    ITextAreaInputHandler areaInputHandler;
    do
    {
      areaInputHandler = (ITextAreaInputHandler) this.stackedInputHandlers.Peek();
      this.stackedInputHandlers = this.stackedInputHandlers.Pop();
      areaInputHandler.Detach();
    }
    while (areaInputHandler != inputHandler);
  }

  public TextDocument Document
  {
    get => (TextDocument) this.GetValue(TextArea.DocumentProperty);
    set => this.SetValue(TextArea.DocumentProperty, (object) value);
  }

  public event EventHandler DocumentChanged;

  private static void OnDocumentChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
  {
    ((TextArea) dp).OnDocumentChanged((TextDocument) e.OldValue, (TextDocument) e.NewValue);
  }

  private void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
  {
    if (oldValue != null)
    {
      WeakEventManagerBase<TextDocumentWeakEventManager.Changing, TextDocument>.RemoveListener(oldValue, (IWeakEventListener) this);
      WeakEventManagerBase<TextDocumentWeakEventManager.Changed, TextDocument>.RemoveListener(oldValue, (IWeakEventListener) this);
      WeakEventManagerBase<TextDocumentWeakEventManager.UpdateStarted, TextDocument>.RemoveListener(oldValue, (IWeakEventListener) this);
      WeakEventManagerBase<TextDocumentWeakEventManager.UpdateFinished, TextDocument>.RemoveListener(oldValue, (IWeakEventListener) this);
    }
    this.textView.Document = newValue;
    if (newValue != null)
    {
      WeakEventManagerBase<TextDocumentWeakEventManager.Changing, TextDocument>.AddListener(newValue, (IWeakEventListener) this);
      WeakEventManagerBase<TextDocumentWeakEventManager.Changed, TextDocument>.AddListener(newValue, (IWeakEventListener) this);
      WeakEventManagerBase<TextDocumentWeakEventManager.UpdateStarted, TextDocument>.AddListener(newValue, (IWeakEventListener) this);
      WeakEventManagerBase<TextDocumentWeakEventManager.UpdateFinished, TextDocument>.AddListener(newValue, (IWeakEventListener) this);
    }
    this.caret.Location = new TextLocation(1, 1);
    this.ClearSelection();
    if (this.DocumentChanged != null)
      this.DocumentChanged((object) this, EventArgs.Empty);
    CommandManager.InvalidateRequerySuggested();
  }

  public TextEditorOptions Options
  {
    get => (TextEditorOptions) this.GetValue(TextArea.OptionsProperty);
    set => this.SetValue(TextArea.OptionsProperty, (object) value);
  }

  public event PropertyChangedEventHandler OptionChanged;

  protected virtual void OnOptionChanged(PropertyChangedEventArgs e)
  {
    if (this.OptionChanged == null)
      return;
    this.OptionChanged((object) this, e);
  }

  private static void OnOptionsChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
  {
    ((TextArea) dp).OnOptionsChanged((TextEditorOptions) e.OldValue, (TextEditorOptions) e.NewValue);
  }

  private void OnOptionsChanged(TextEditorOptions oldValue, TextEditorOptions newValue)
  {
    if (oldValue != null)
      WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>.RemoveListener((INotifyPropertyChanged) oldValue, (IWeakEventListener) this);
    this.textView.Options = newValue;
    if (newValue != null)
      WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>.AddListener((INotifyPropertyChanged) newValue, (IWeakEventListener) this);
    this.OnOptionChanged(new PropertyChangedEventArgs((string) null));
  }

  protected virtual bool ReceiveWeakEvent(System.Type managerType, object sender, EventArgs e)
  {
    if (managerType == typeof (TextDocumentWeakEventManager.Changing))
    {
      this.OnDocumentChanging();
      return true;
    }
    if (managerType == typeof (TextDocumentWeakEventManager.Changed))
    {
      this.OnDocumentChanged((DocumentChangeEventArgs) e);
      return true;
    }
    if (managerType == typeof (TextDocumentWeakEventManager.UpdateStarted))
    {
      this.OnUpdateStarted();
      return true;
    }
    if (managerType == typeof (TextDocumentWeakEventManager.UpdateFinished))
    {
      this.OnUpdateFinished();
      return true;
    }
    if (!(managerType == typeof (PropertyChangedWeakEventManager)))
      return false;
    this.OnOptionChanged((PropertyChangedEventArgs) e);
    return true;
  }

  bool IWeakEventListener.ReceiveWeakEvent(System.Type managerType, object sender, EventArgs e)
  {
    return this.ReceiveWeakEvent(managerType, sender, e);
  }

  private void OnDocumentChanging() => this.caret.OnDocumentChanging();

  private void OnDocumentChanged(DocumentChangeEventArgs e)
  {
    this.caret.OnDocumentChanged(e);
    this.Selection = this.selection.UpdateOnDocumentChange(e);
  }

  private void OnUpdateStarted()
  {
    this.Document.UndoStack.PushOptional((IUndoableOperation) new TextArea.RestoreCaretAndSelectionUndoAction(this));
  }

  private void OnUpdateFinished() => this.caret.OnDocumentUpdateFinished();

  public TextView TextView => this.textView;

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this.scrollInfo = (IScrollInfo) this.textView;
    this.ApplyScrollInfo();
  }

  public event EventHandler SelectionChanged;

  public Selection Selection
  {
    get => this.selection;
    set
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (value.textArea != this)
        throw new ArgumentException("Cannot use a Selection instance that belongs to another text area.");
      if (object.Equals((object) this.selection, (object) value))
        return;
      if (this.textView != null)
      {
        ISegment surroundingSegment1 = this.selection.SurroundingSegment;
        ISegment surroundingSegment2 = value.SurroundingSegment;
        if (!this.Selection.EnableVirtualSpace && this.selection is SimpleSelection && value is SimpleSelection && surroundingSegment1 != null && surroundingSegment2 != null)
        {
          int offset1 = surroundingSegment1.Offset;
          int offset2 = surroundingSegment2.Offset;
          if (offset1 != offset2)
            this.textView.Redraw(Math.Min(offset1, offset2), Math.Abs(offset1 - offset2), DispatcherPriority.Render);
          int endOffset1 = surroundingSegment1.EndOffset;
          int endOffset2 = surroundingSegment2.EndOffset;
          if (endOffset1 != endOffset2)
            this.textView.Redraw(Math.Min(endOffset1, endOffset2), Math.Abs(endOffset1 - endOffset2), DispatcherPriority.Render);
        }
        else
        {
          this.textView.Redraw(surroundingSegment1, DispatcherPriority.Render);
          this.textView.Redraw(surroundingSegment2, DispatcherPriority.Render);
        }
      }
      this.selection = value;
      if (this.SelectionChanged != null)
        this.SelectionChanged((object) this, EventArgs.Empty);
      CommandManager.InvalidateRequerySuggested();
    }
  }

  public void ClearSelection() => this.Selection = this.emptySelection;

  public Brush SelectionBrush
  {
    get => (Brush) this.GetValue(TextArea.SelectionBrushProperty);
    set => this.SetValue(TextArea.SelectionBrushProperty, (object) value);
  }

  public Brush SelectionForeground
  {
    get => (Brush) this.GetValue(TextArea.SelectionForegroundProperty);
    set => this.SetValue(TextArea.SelectionForegroundProperty, (object) value);
  }

  public Pen SelectionBorder
  {
    get => (Pen) this.GetValue(TextArea.SelectionBorderProperty);
    set => this.SetValue(TextArea.SelectionBorderProperty, (object) value);
  }

  public double SelectionCornerRadius
  {
    get => (double) this.GetValue(TextArea.SelectionCornerRadiusProperty);
    set => this.SetValue(TextArea.SelectionCornerRadiusProperty, (object) value);
  }

  public MouseSelectionMode MouseSelectionMode
  {
    get
    {
      return this.DefaultInputHandler.MouseSelection is SelectionMouseHandler mouseSelection ? mouseSelection.MouseSelectionMode : MouseSelectionMode.None;
    }
    set
    {
      if (!(this.DefaultInputHandler.MouseSelection is SelectionMouseHandler mouseSelection))
        return;
      mouseSelection.MouseSelectionMode = value;
    }
  }

  private void RequestSelectionValidation()
  {
    if (this.ensureSelectionValidRequested || this.allowCaretOutsideSelection != 0)
      return;
    this.ensureSelectionValidRequested = true;
    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate) new Action(this.EnsureSelectionValid));
  }

  private void EnsureSelectionValid()
  {
    this.ensureSelectionValidRequested = false;
    if (this.allowCaretOutsideSelection != 0 || this.selection.IsEmpty || this.selection.Contains(this.caret.Offset))
      return;
    this.ClearSelection();
  }

  public IDisposable AllowCaretOutsideSelection()
  {
    this.VerifyAccess();
    ++this.allowCaretOutsideSelection;
    return (IDisposable) new CallbackOnDispose((Action) (() =>
    {
      this.VerifyAccess();
      --this.allowCaretOutsideSelection;
      this.RequestSelectionValidation();
    }));
  }

  public Caret Caret => this.caret;

  private void CaretPositionChanged(object sender, EventArgs e)
  {
    if (this.textView == null)
      return;
    this.textView.HighlightedLine = this.Caret.Line;
  }

  public ObservableCollection<UIElement> LeftMargins => this.leftMargins;

  private void leftMargins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
  {
    if (e.OldItems != null)
    {
      foreach (ITextViewConnect textViewConnect in e.OldItems.OfType<ITextViewConnect>())
        textViewConnect.RemoveFromTextView(this.textView);
    }
    if (e.NewItems == null)
      return;
    foreach (ITextViewConnect textViewConnect in e.NewItems.OfType<ITextViewConnect>())
      textViewConnect.AddToTextView(this.textView);
  }

  public IReadOnlySectionProvider ReadOnlySectionProvider
  {
    get => this.readOnlySectionProvider;
    set
    {
      this.readOnlySectionProvider = value != null ? value : throw new ArgumentNullException(nameof (value));
      CommandManager.InvalidateRequerySuggested();
    }
  }

  private void ApplyScrollInfo()
  {
    if (this.scrollInfo == null)
      return;
    this.scrollInfo.ScrollOwner = this.scrollOwner;
    this.scrollInfo.CanVerticallyScroll = this.canVerticallyScroll;
    this.scrollInfo.CanHorizontallyScroll = this.canHorizontallyScroll;
    this.scrollOwner = (ScrollViewer) null;
  }

  bool IScrollInfo.CanVerticallyScroll
  {
    get => this.scrollInfo != null && this.scrollInfo.CanVerticallyScroll;
    set
    {
      this.canVerticallyScroll = value;
      if (this.scrollInfo == null)
        return;
      this.scrollInfo.CanVerticallyScroll = value;
    }
  }

  bool IScrollInfo.CanHorizontallyScroll
  {
    get => this.scrollInfo != null && this.scrollInfo.CanHorizontallyScroll;
    set
    {
      this.canHorizontallyScroll = value;
      if (this.scrollInfo == null)
        return;
      this.scrollInfo.CanHorizontallyScroll = value;
    }
  }

  double IScrollInfo.ExtentWidth => this.scrollInfo == null ? 0.0 : this.scrollInfo.ExtentWidth;

  double IScrollInfo.ExtentHeight => this.scrollInfo == null ? 0.0 : this.scrollInfo.ExtentHeight;

  double IScrollInfo.ViewportWidth => this.scrollInfo == null ? 0.0 : this.scrollInfo.ViewportWidth;

  double IScrollInfo.ViewportHeight
  {
    get => this.scrollInfo == null ? 0.0 : this.scrollInfo.ViewportHeight;
  }

  double IScrollInfo.HorizontalOffset
  {
    get => this.scrollInfo == null ? 0.0 : this.scrollInfo.HorizontalOffset;
  }

  double IScrollInfo.VerticalOffset
  {
    get => this.scrollInfo == null ? 0.0 : this.scrollInfo.VerticalOffset;
  }

  ScrollViewer IScrollInfo.ScrollOwner
  {
    get => this.scrollInfo == null ? (ScrollViewer) null : this.scrollInfo.ScrollOwner;
    set
    {
      if (this.scrollInfo != null)
        this.scrollInfo.ScrollOwner = value;
      else
        this.scrollOwner = value;
    }
  }

  void IScrollInfo.LineUp()
  {
    if (this.scrollInfo == null)
      return;
    this.scrollInfo.LineUp();
  }

  void IScrollInfo.LineDown()
  {
    if (this.scrollInfo == null)
      return;
    this.scrollInfo.LineDown();
  }

  void IScrollInfo.LineLeft()
  {
    if (this.scrollInfo == null)
      return;
    this.scrollInfo.LineLeft();
  }

  void IScrollInfo.LineRight()
  {
    if (this.scrollInfo == null)
      return;
    this.scrollInfo.LineRight();
  }

  void IScrollInfo.PageUp()
  {
    if (this.scrollInfo == null)
      return;
    this.scrollInfo.PageUp();
  }

  void IScrollInfo.PageDown()
  {
    if (this.scrollInfo == null)
      return;
    this.scrollInfo.PageDown();
  }

  void IScrollInfo.PageLeft()
  {
    if (this.scrollInfo == null)
      return;
    this.scrollInfo.PageLeft();
  }

  void IScrollInfo.PageRight()
  {
    if (this.scrollInfo == null)
      return;
    this.scrollInfo.PageRight();
  }

  void IScrollInfo.MouseWheelUp()
  {
    if (this.scrollInfo == null)
      return;
    this.scrollInfo.MouseWheelUp();
  }

  void IScrollInfo.MouseWheelDown()
  {
    if (this.scrollInfo == null)
      return;
    this.scrollInfo.MouseWheelDown();
  }

  void IScrollInfo.MouseWheelLeft()
  {
    if (this.scrollInfo == null)
      return;
    this.scrollInfo.MouseWheelLeft();
  }

  void IScrollInfo.MouseWheelRight()
  {
    if (this.scrollInfo == null)
      return;
    this.scrollInfo.MouseWheelRight();
  }

  void IScrollInfo.SetHorizontalOffset(double offset)
  {
    if (this.scrollInfo == null)
      return;
    this.scrollInfo.SetHorizontalOffset(offset);
  }

  void IScrollInfo.SetVerticalOffset(double offset)
  {
    if (this.scrollInfo == null)
      return;
    this.scrollInfo.SetVerticalOffset(offset);
  }

  Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle)
  {
    return this.scrollInfo != null ? this.scrollInfo.MakeVisible(visual, rectangle) : Rect.Empty;
  }

  protected override void OnMouseDown(MouseButtonEventArgs e)
  {
    base.OnMouseDown(e);
    this.Focus();
  }

  protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
  {
    base.OnGotKeyboardFocus(e);
    this.ime.OnGotKeyboardFocus(e);
    this.caret.Show();
  }

  protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
  {
    base.OnLostKeyboardFocus(e);
    this.caret.Hide();
    this.ime.OnLostKeyboardFocus(e);
  }

  public event TextCompositionEventHandler TextEntering;

  public event TextCompositionEventHandler TextEntered;

  protected virtual void OnTextEntering(TextCompositionEventArgs e)
  {
    if (this.TextEntering == null)
      return;
    this.TextEntering((object) this, e);
  }

  protected virtual void OnTextEntered(TextCompositionEventArgs e)
  {
    if (this.TextEntered == null)
      return;
    this.TextEntered((object) this, e);
  }

  protected override void OnTextInput(TextCompositionEventArgs e)
  {
    base.OnTextInput(e);
    if (e.Handled || this.Document == null || string.IsNullOrEmpty(e.Text) || e.Text == "\u001B" || e.Text == "\b")
      return;
    this.HideMouseCursor();
    this.PerformTextInput(e);
    e.Handled = true;
  }

  public void PerformTextInput(string text)
  {
    TextCompositionEventArgs e = new TextCompositionEventArgs((InputDevice) Keyboard.PrimaryDevice, new TextComposition(InputManager.Current, (IInputElement) this, text));
    e.RoutedEvent = UIElement.TextInputEvent;
    this.PerformTextInput(e);
  }

  public void PerformTextInput(TextCompositionEventArgs e)
  {
    if (e == null)
      throw new ArgumentNullException(nameof (e));
    if (this.Document == null)
      throw ThrowUtil.NoDocumentAssigned();
    this.OnTextEntering(e);
    if (e.Handled)
      return;
    if (e.Text == "\n" || e.Text == "\r" || e.Text == "\r\n")
    {
      this.ReplaceSelectionWithNewLine();
    }
    else
    {
      if (this.OverstrikeMode && this.Selection.IsEmpty && this.Document.GetLineByNumber(this.Caret.Line).EndOffset > this.Caret.Offset)
        EditingCommands.SelectRightByCharacter.Execute((object) null, (IInputElement) this);
      this.ReplaceSelectionWithText(e.Text);
    }
    this.OnTextEntered(e);
    this.caret.BringCaretToView();
  }

  private void ReplaceSelectionWithNewLine()
  {
    string lineFromDocument = TextUtilities.GetNewLineFromDocument((IDocument) this.Document, this.Caret.Line);
    using (this.Document.RunUpdate())
    {
      this.ReplaceSelectionWithText(lineFromDocument);
      if (this.IndentationStrategy == null)
        return;
      DocumentLine lineByNumber = this.Document.GetLineByNumber(this.Caret.Line);
      ISegment[] deletableSegments = this.GetDeletableSegments((ISegment) lineByNumber);
      if (deletableSegments.Length != 1 || deletableSegments[0].Offset != lineByNumber.Offset || deletableSegments[0].Length != lineByNumber.Length)
        return;
      this.IndentationStrategy.IndentLine(this.Document, lineByNumber);
    }
  }

  internal void RemoveSelectedText()
  {
    if (this.Document == null)
      throw ThrowUtil.NoDocumentAssigned();
    this.selection.ReplaceSelectionWithText(string.Empty);
  }

  internal void ReplaceSelectionWithText(string newText)
  {
    if (newText == null)
      throw new ArgumentNullException(nameof (newText));
    if (this.Document == null)
      throw ThrowUtil.NoDocumentAssigned();
    this.selection.ReplaceSelectionWithText(newText);
  }

  internal ISegment[] GetDeletableSegments(ISegment segment)
  {
    ISegment[] array = (this.ReadOnlySectionProvider.GetDeletableSegments(segment) ?? throw new InvalidOperationException("ReadOnlySectionProvider.GetDeletableSegments returned null")).ToArray<ISegment>();
    int num = segment.Offset;
    for (int index = 0; index < array.Length; ++index)
    {
      if (array[index].Offset < num)
        throw new InvalidOperationException("ReadOnlySectionProvider returned incorrect segments (outside of input segment / wrong order)");
      num = array[index].EndOffset;
    }
    if (num > segment.EndOffset)
      throw new InvalidOperationException("ReadOnlySectionProvider returned incorrect segments (outside of input segment / wrong order)");
    return array;
  }

  public IIndentationStrategy IndentationStrategy
  {
    get => (IIndentationStrategy) this.GetValue(TextArea.IndentationStrategyProperty);
    set => this.SetValue(TextArea.IndentationStrategyProperty, (object) value);
  }

  protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
  {
    base.OnPreviewKeyDown(e);
    foreach (TextAreaStackedInputHandler stackedInputHandler in this.stackedInputHandlers)
    {
      if (e.Handled)
        break;
      stackedInputHandler.OnPreviewKeyDown(e);
    }
  }

  protected override void OnPreviewKeyUp(System.Windows.Input.KeyEventArgs e)
  {
    base.OnPreviewKeyUp(e);
    foreach (TextAreaStackedInputHandler stackedInputHandler in this.stackedInputHandlers)
    {
      if (e.Handled)
        break;
      stackedInputHandler.OnPreviewKeyUp(e);
    }
  }

  protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
  {
    base.OnKeyDown(e);
    this.TextView.InvalidateCursorIfMouseWithinTextView();
  }

  protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
  {
    base.OnKeyUp(e);
    this.TextView.InvalidateCursorIfMouseWithinTextView();
  }

  private void AttachTypingEvents()
  {
    this.MouseEnter += (System.Windows.Input.MouseEventHandler) ((param0, param1) => this.ShowMouseCursor());
    this.MouseLeave += (System.Windows.Input.MouseEventHandler) ((param0, param1) => this.ShowMouseCursor());
    this.PreviewMouseMove += (System.Windows.Input.MouseEventHandler) ((param0, param1) => this.ShowMouseCursor());
    this.TouchEnter += (EventHandler<TouchEventArgs>) ((param0, param1) => this.ShowMouseCursor());
    this.TouchLeave += (EventHandler<TouchEventArgs>) ((param0, param1) => this.ShowMouseCursor());
    this.PreviewTouchMove += (EventHandler<TouchEventArgs>) ((param0, param1) => this.ShowMouseCursor());
  }

  private void ShowMouseCursor()
  {
    if (!this.isMouseCursorHidden)
      return;
    System.Windows.Forms.Cursor.Show();
    this.isMouseCursorHidden = false;
  }

  private void HideMouseCursor()
  {
    if (!this.Options.HideCursorWhileTyping || this.isMouseCursorHidden || !this.IsMouseOver)
      return;
    this.isMouseCursorHidden = true;
    System.Windows.Forms.Cursor.Hide();
  }

  public bool OverstrikeMode
  {
    get => (bool) this.GetValue(TextArea.OverstrikeModeProperty);
    set => this.SetValue(TextArea.OverstrikeModeProperty, (object) value);
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new TextAreaAutomationPeer(this);
  }

  protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
  {
    return (HitTestResult) new PointHitTestResult((Visual) this, hitTestParameters.HitPoint);
  }

  protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
  {
    base.OnPropertyChanged(e);
    if (e.Property == TextArea.SelectionBrushProperty || e.Property == TextArea.SelectionBorderProperty || e.Property == TextArea.SelectionForegroundProperty || e.Property == TextArea.SelectionCornerRadiusProperty)
    {
      this.textView.Redraw();
    }
    else
    {
      if (e.Property != TextArea.OverstrikeModeProperty)
        return;
      this.caret.UpdateIfVisible();
    }
  }

  public virtual object GetService(System.Type serviceType)
  {
    return this.textView.GetService(serviceType);
  }

  public event EventHandler<TextEventArgs> TextCopied;

  internal void OnTextCopied(TextEventArgs e)
  {
    if (this.TextCopied == null)
      return;
    this.TextCopied((object) this, e);
  }

  private sealed class RestoreCaretAndSelectionUndoAction : IUndoableOperation
  {
    private WeakReference textAreaReference;
    private TextViewPosition caretPosition;
    private Selection selection;

    public RestoreCaretAndSelectionUndoAction(TextArea textArea)
    {
      this.textAreaReference = new WeakReference((object) textArea);
      this.caretPosition = textArea.Caret.NonValidatedPosition;
      this.selection = textArea.Selection;
    }

    public void Undo()
    {
      TextArea target = (TextArea) this.textAreaReference.Target;
      if (target == null)
        return;
      target.Caret.Position = this.caretPosition;
      target.Selection = this.selection;
    }

    public void Redo() => this.Undo();
  }
}
