// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.TextEditor
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable disable
namespace ICSharpCode.AvalonEdit;

[Localizability(LocalizationCategory.Text)]
[ContentProperty("Text")]
public partial class TextEditor : Control, ITextEditorComponent, IServiceProvider, IWeakEventListener
{
  public static readonly DependencyProperty DocumentProperty = TextView.DocumentProperty.AddOwner(typeof (TextEditor), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(TextEditor.OnDocumentChanged)));
  public static readonly DependencyProperty OptionsProperty = TextView.OptionsProperty.AddOwner(typeof (TextEditor), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(TextEditor.OnOptionsChanged)));
  private readonly TextArea textArea;
  private ScrollViewer scrollViewer;
  public static readonly DependencyProperty SyntaxHighlightingProperty = DependencyProperty.Register(nameof (SyntaxHighlighting), typeof (IHighlightingDefinition), typeof (TextEditor), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(TextEditor.OnSyntaxHighlightingChanged)));
  private IVisualLineTransformer colorizer;
  public static readonly DependencyProperty WordWrapProperty = DependencyProperty.Register(nameof (WordWrap), typeof (bool), typeof (TextEditor), (PropertyMetadata) new FrameworkPropertyMetadata(Boxes.False));
  public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof (IsReadOnly), typeof (bool), typeof (TextEditor), (PropertyMetadata) new FrameworkPropertyMetadata(Boxes.False, new PropertyChangedCallback(TextEditor.OnIsReadOnlyChanged)));
  public static readonly DependencyProperty IsModifiedProperty = DependencyProperty.Register(nameof (IsModified), typeof (bool), typeof (TextEditor), (PropertyMetadata) new FrameworkPropertyMetadata(Boxes.False, new PropertyChangedCallback(TextEditor.OnIsModifiedChanged)));
  public static readonly DependencyProperty ShowLineNumbersProperty = DependencyProperty.Register(nameof (ShowLineNumbers), typeof (bool), typeof (TextEditor), (PropertyMetadata) new FrameworkPropertyMetadata(Boxes.False, new PropertyChangedCallback(TextEditor.OnShowLineNumbersChanged)));
  public static readonly DependencyProperty LineNumbersForegroundProperty = DependencyProperty.Register(nameof (LineNumbersForeground), typeof (Brush), typeof (TextEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Gray, new PropertyChangedCallback(TextEditor.OnLineNumbersForegroundChanged)));
  public static readonly DependencyProperty EncodingProperty = DependencyProperty.Register(nameof (Encoding), typeof (Encoding), typeof (TextEditor));
  public static readonly RoutedEvent PreviewMouseHoverEvent = TextView.PreviewMouseHoverEvent.AddOwner(typeof (TextEditor));
  public static readonly RoutedEvent MouseHoverEvent = TextView.MouseHoverEvent.AddOwner(typeof (TextEditor));
  public static readonly RoutedEvent PreviewMouseHoverStoppedEvent = TextView.PreviewMouseHoverStoppedEvent.AddOwner(typeof (TextEditor));
  public static readonly RoutedEvent MouseHoverStoppedEvent = TextView.MouseHoverStoppedEvent.AddOwner(typeof (TextEditor));
  public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = ScrollViewer.HorizontalScrollBarVisibilityProperty.AddOwner(typeof (TextEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) ScrollBarVisibility.Visible));
  public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = ScrollViewer.VerticalScrollBarVisibilityProperty.AddOwner(typeof (TextEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) ScrollBarVisibility.Visible));

  static TextEditor()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (TextEditor), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (TextEditor)));
    UIElement.FocusableProperty.OverrideMetadata(typeof (TextEditor), (PropertyMetadata) new FrameworkPropertyMetadata(Boxes.True));
  }

  public TextEditor()
    : this(new TextArea())
  {
  }

  protected TextEditor(TextArea textArea)
  {
    this.textArea = textArea != null ? textArea : throw new ArgumentNullException(nameof (textArea));
    textArea.TextView.Services.AddService(typeof (TextEditor), (object) this);
    this.SetCurrentValue(TextEditor.OptionsProperty, (object) textArea.Options);
    this.SetCurrentValue(TextEditor.DocumentProperty, (object) new TextDocument());
  }

  protected override AutomationPeer OnCreateAutomationPeer()
  {
    return (AutomationPeer) new TextEditorAutomationPeer(this);
  }

  protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
  {
    base.OnGotKeyboardFocus(e);
    if (e.NewFocus != this)
      return;
    Keyboard.Focus((IInputElement) this.TextArea);
    e.Handled = true;
  }

  public TextDocument Document
  {
    get => (TextDocument) this.GetValue(TextEditor.DocumentProperty);
    set => this.SetValue(TextEditor.DocumentProperty, (object) value);
  }

  public event EventHandler DocumentChanged;

  protected virtual void OnDocumentChanged(EventArgs e)
  {
    if (this.DocumentChanged == null)
      return;
    this.DocumentChanged((object) this, e);
  }

  private static void OnDocumentChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
  {
    ((TextEditor) dp).OnDocumentChanged((TextDocument) e.OldValue, (TextDocument) e.NewValue);
  }

  private void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
  {
    if (oldValue != null)
    {
      WeakEventManagerBase<TextDocumentWeakEventManager.TextChanged, TextDocument>.RemoveListener(oldValue, (IWeakEventListener) this);
      PropertyChangedEventManager.RemoveListener((INotifyPropertyChanged) oldValue.UndoStack, (IWeakEventListener) this, "IsOriginalFile");
    }
    this.textArea.Document = newValue;
    if (newValue != null)
    {
      WeakEventManagerBase<TextDocumentWeakEventManager.TextChanged, TextDocument>.AddListener(newValue, (IWeakEventListener) this);
      PropertyChangedEventManager.AddListener((INotifyPropertyChanged) newValue.UndoStack, (IWeakEventListener) this, "IsOriginalFile");
    }
    this.OnDocumentChanged(EventArgs.Empty);
    this.OnTextChanged(EventArgs.Empty);
  }

  public TextEditorOptions Options
  {
    get => (TextEditorOptions) this.GetValue(TextEditor.OptionsProperty);
    set => this.SetValue(TextEditor.OptionsProperty, (object) value);
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
    ((TextEditor) dp).OnOptionsChanged((TextEditorOptions) e.OldValue, (TextEditorOptions) e.NewValue);
  }

  private void OnOptionsChanged(TextEditorOptions oldValue, TextEditorOptions newValue)
  {
    if (oldValue != null)
      WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>.RemoveListener((INotifyPropertyChanged) oldValue, (IWeakEventListener) this);
    this.textArea.Options = newValue;
    if (newValue != null)
      WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>.AddListener((INotifyPropertyChanged) newValue, (IWeakEventListener) this);
    this.OnOptionChanged(new PropertyChangedEventArgs((string) null));
  }

  protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    if (managerType == typeof (PropertyChangedWeakEventManager))
    {
      this.OnOptionChanged((PropertyChangedEventArgs) e);
      return true;
    }
    if (managerType == typeof (TextDocumentWeakEventManager.TextChanged))
    {
      this.OnTextChanged(e);
      return true;
    }
    return managerType == typeof (PropertyChangedEventManager) && this.HandleIsOriginalChanged((PropertyChangedEventArgs) e);
  }

  bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    return this.ReceiveWeakEvent(managerType, sender, e);
  }

  [DefaultValue("")]
  [Localizability(LocalizationCategory.Text)]
  public string Text
  {
    get
    {
      TextDocument document = this.Document;
      return document == null ? string.Empty : document.Text;
    }
    set
    {
      TextDocument document = this.GetDocument();
      document.Text = value ?? string.Empty;
      this.CaretOffset = 0;
      document.UndoStack.ClearAll();
    }
  }

  private TextDocument GetDocument() => this.Document ?? throw ThrowUtil.NoDocumentAssigned();

  public event EventHandler TextChanged;

  protected virtual void OnTextChanged(EventArgs e)
  {
    if (this.TextChanged == null)
      return;
    this.TextChanged((object) this, e);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this.scrollViewer = (ScrollViewer) this.Template.FindName("PART_ScrollViewer", (FrameworkElement) this);
  }

  public TextArea TextArea => this.textArea;

  internal ScrollViewer ScrollViewer => this.scrollViewer;

  private bool CanExecute(RoutedUICommand command)
  {
    TextArea textArea = this.TextArea;
    return textArea != null && command.CanExecute((object) null, (IInputElement) textArea);
  }

  private void Execute(RoutedUICommand command)
  {
    TextArea textArea = this.TextArea;
    if (textArea == null)
      return;
    command.Execute((object) null, (IInputElement) textArea);
  }

  public IHighlightingDefinition SyntaxHighlighting
  {
    get => (IHighlightingDefinition) this.GetValue(TextEditor.SyntaxHighlightingProperty);
    set => this.SetValue(TextEditor.SyntaxHighlightingProperty, (object) value);
  }

  private static void OnSyntaxHighlightingChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((TextEditor) d).OnSyntaxHighlightingChanged(e.NewValue as IHighlightingDefinition);
  }

  private void OnSyntaxHighlightingChanged(IHighlightingDefinition newValue)
  {
    if (this.colorizer != null)
    {
      this.TextArea.TextView.LineTransformers.Remove(this.colorizer);
      this.colorizer = (IVisualLineTransformer) null;
    }
    if (newValue == null)
      return;
    this.colorizer = this.CreateColorizer(newValue);
    if (this.colorizer == null)
      return;
    this.TextArea.TextView.LineTransformers.Insert(0, this.colorizer);
  }

  protected virtual IVisualLineTransformer CreateColorizer(
    IHighlightingDefinition highlightingDefinition)
  {
    return highlightingDefinition != null ? (IVisualLineTransformer) new HighlightingColorizer(highlightingDefinition) : throw new ArgumentNullException(nameof (highlightingDefinition));
  }

  public bool WordWrap
  {
    get => (bool) this.GetValue(TextEditor.WordWrapProperty);
    set => this.SetValue(TextEditor.WordWrapProperty, Boxes.Box(value));
  }

  public bool IsReadOnly
  {
    get => (bool) this.GetValue(TextEditor.IsReadOnlyProperty);
    set => this.SetValue(TextEditor.IsReadOnlyProperty, Boxes.Box(value));
  }

  private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (!(d is TextEditor element))
      return;
    element.TextArea.ReadOnlySectionProvider = !(bool) e.NewValue ? (IReadOnlySectionProvider) NoReadOnlySections.Instance : (IReadOnlySectionProvider) ReadOnlySectionDocument.Instance;
    if (!(UIElementAutomationPeer.FromElement((UIElement) element) is TextEditorAutomationPeer editorAutomationPeer))
      return;
    editorAutomationPeer.RaiseIsReadOnlyChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  public bool IsModified
  {
    get => (bool) this.GetValue(TextEditor.IsModifiedProperty);
    set => this.SetValue(TextEditor.IsModifiedProperty, Boxes.Box(value));
  }

  private static void OnIsModifiedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (!(d is TextEditor textEditor))
      return;
    TextDocument document = textEditor.Document;
    if (document == null)
      return;
    UndoStack undoStack = document.UndoStack;
    if ((bool) e.NewValue)
    {
      if (!undoStack.IsOriginalFile)
        return;
      undoStack.DiscardOriginalFileMarker();
    }
    else
      undoStack.MarkAsOriginalFile();
  }

  private bool HandleIsOriginalChanged(PropertyChangedEventArgs e)
  {
    if (!(e.PropertyName == "IsOriginalFile"))
      return false;
    TextDocument document = this.Document;
    if (document != null)
      this.SetCurrentValue(TextEditor.IsModifiedProperty, Boxes.Box(!document.UndoStack.IsOriginalFile));
    return true;
  }

  public bool ShowLineNumbers
  {
    get => (bool) this.GetValue(TextEditor.ShowLineNumbersProperty);
    set => this.SetValue(TextEditor.ShowLineNumbersProperty, Boxes.Box(value));
  }

  private static void OnShowLineNumbersChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    TextEditor textEditor = (TextEditor) d;
    ObservableCollection<UIElement> leftMargins = textEditor.TextArea.LeftMargins;
    if ((bool) e.NewValue)
    {
      LineNumberMargin lineNumberMargin = new LineNumberMargin();
      Line line = (Line) DottedLineMargin.Create();
      leftMargins.Insert(0, (UIElement) lineNumberMargin);
      leftMargins.Insert(1, (UIElement) line);
      Binding binding = new Binding("LineNumbersForeground")
      {
        Source = (object) textEditor
      };
      line.SetBinding(Shape.StrokeProperty, (BindingBase) binding);
      lineNumberMargin.SetBinding(Control.ForegroundProperty, (BindingBase) binding);
    }
    else
    {
      for (int index = 0; index < leftMargins.Count; ++index)
      {
        if (leftMargins[index] is LineNumberMargin)
        {
          leftMargins.RemoveAt(index);
          if (index >= leftMargins.Count || !DottedLineMargin.IsDottedLineMargin(leftMargins[index]))
            break;
          leftMargins.RemoveAt(index);
          break;
        }
      }
    }
  }

  public Brush LineNumbersForeground
  {
    get => (Brush) this.GetValue(TextEditor.LineNumbersForegroundProperty);
    set => this.SetValue(TextEditor.LineNumbersForegroundProperty, (object) value);
  }

  private static void OnLineNumbersForegroundChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(((TextEditor) d).TextArea.LeftMargins.FirstOrDefault<UIElement>((Func<UIElement, bool>) (margin => margin is LineNumberMargin)) is LineNumberMargin lineNumberMargin))
      return;
    lineNumberMargin.SetValue(Control.ForegroundProperty, e.NewValue);
  }

  public void AppendText(string textData)
  {
    TextDocument document = this.GetDocument();
    document.Insert(document.TextLength, textData);
  }

  public void BeginChange() => this.GetDocument().BeginUpdate();

  public void Copy() => this.Execute(ApplicationCommands.Copy);

  public void Cut() => this.Execute(ApplicationCommands.Cut);

  public IDisposable DeclareChangeBlock() => this.GetDocument().RunUpdate();

  public void Delete() => this.Execute(ApplicationCommands.Delete);

  public void EndChange() => this.GetDocument().EndUpdate();

  public void LineDown()
  {
    if (this.scrollViewer == null)
      return;
    this.scrollViewer.LineDown();
  }

  public void LineLeft()
  {
    if (this.scrollViewer == null)
      return;
    this.scrollViewer.LineLeft();
  }

  public void LineRight()
  {
    if (this.scrollViewer == null)
      return;
    this.scrollViewer.LineRight();
  }

  public void LineUp()
  {
    if (this.scrollViewer == null)
      return;
    this.scrollViewer.LineUp();
  }

  public void PageDown()
  {
    if (this.scrollViewer == null)
      return;
    this.scrollViewer.PageDown();
  }

  public void PageUp()
  {
    if (this.scrollViewer == null)
      return;
    this.scrollViewer.PageUp();
  }

  public void PageLeft()
  {
    if (this.scrollViewer == null)
      return;
    this.scrollViewer.PageLeft();
  }

  public void PageRight()
  {
    if (this.scrollViewer == null)
      return;
    this.scrollViewer.PageRight();
  }

  public void Paste() => this.Execute(ApplicationCommands.Paste);

  public bool Redo()
  {
    if (!this.CanExecute(ApplicationCommands.Redo))
      return false;
    this.Execute(ApplicationCommands.Redo);
    return true;
  }

  public void ScrollToEnd()
  {
    this.ApplyTemplate();
    if (this.scrollViewer == null)
      return;
    this.scrollViewer.ScrollToEnd();
  }

  public void ScrollToHome()
  {
    this.ApplyTemplate();
    if (this.scrollViewer == null)
      return;
    this.scrollViewer.ScrollToHome();
  }

  public void ScrollToHorizontalOffset(double offset)
  {
    this.ApplyTemplate();
    if (this.scrollViewer == null)
      return;
    this.scrollViewer.ScrollToHorizontalOffset(offset);
  }

  public void ScrollToVerticalOffset(double offset)
  {
    this.ApplyTemplate();
    if (this.scrollViewer == null)
      return;
    this.scrollViewer.ScrollToVerticalOffset(offset);
  }

  public void SelectAll() => this.Execute(ApplicationCommands.SelectAll);

  public bool Undo()
  {
    if (!this.CanExecute(ApplicationCommands.Undo))
      return false;
    this.Execute(ApplicationCommands.Undo);
    return true;
  }

  public bool CanRedo => this.CanExecute(ApplicationCommands.Redo);

  public bool CanUndo => this.CanExecute(ApplicationCommands.Undo);

  public double ExtentHeight => this.scrollViewer == null ? 0.0 : this.scrollViewer.ExtentHeight;

  public double ExtentWidth => this.scrollViewer == null ? 0.0 : this.scrollViewer.ExtentWidth;

  public double ViewportHeight
  {
    get => this.scrollViewer == null ? 0.0 : this.scrollViewer.ViewportHeight;
  }

  public double ViewportWidth => this.scrollViewer == null ? 0.0 : this.scrollViewer.ViewportWidth;

  public double VerticalOffset
  {
    get => this.scrollViewer == null ? 0.0 : this.scrollViewer.VerticalOffset;
  }

  public double HorizontalOffset
  {
    get => this.scrollViewer == null ? 0.0 : this.scrollViewer.HorizontalOffset;
  }

  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public string SelectedText
  {
    get
    {
      TextArea textArea = this.TextArea;
      return textArea != null && textArea.Document != null && !textArea.Selection.IsEmpty ? textArea.Document.GetText(textArea.Selection.SurroundingSegment) : string.Empty;
    }
    set
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      TextArea textArea = this.TextArea;
      if (textArea == null || textArea.Document == null)
        return;
      int selectionStart = this.SelectionStart;
      int selectionLength = this.SelectionLength;
      textArea.Document.Replace(selectionStart, selectionLength, value);
      textArea.Selection = Selection.Create(textArea, selectionStart, selectionStart + value.Length);
    }
  }

  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public int CaretOffset
  {
    get
    {
      TextArea textArea = this.TextArea;
      return textArea != null ? textArea.Caret.Offset : 0;
    }
    set
    {
      TextArea textArea = this.TextArea;
      if (textArea == null)
        return;
      textArea.Caret.Offset = value;
    }
  }

  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public int SelectionStart
  {
    get
    {
      TextArea textArea = this.TextArea;
      if (textArea == null)
        return 0;
      return textArea.Selection.IsEmpty ? textArea.Caret.Offset : textArea.Selection.SurroundingSegment.Offset;
    }
    set => this.Select(value, this.SelectionLength);
  }

  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public int SelectionLength
  {
    get
    {
      TextArea textArea = this.TextArea;
      return textArea != null && !textArea.Selection.IsEmpty ? textArea.Selection.SurroundingSegment.Length : 0;
    }
    set => this.Select(this.SelectionStart, value);
  }

  public void Select(int start, int length)
  {
    int textLength = this.Document != null ? this.Document.TextLength : 0;
    if (start < 0 || start > textLength)
      throw new ArgumentOutOfRangeException(nameof (start), (object) start, "Value must be between 0 and " + (object) textLength);
    if (length < 0 || start + length > textLength)
      throw new ArgumentOutOfRangeException(nameof (length), (object) length, "Value must be between 0 and " + (object) (textLength - start));
    this.textArea.Selection = Selection.Create(this.textArea, start, start + length);
    this.textArea.Caret.Offset = start + length;
  }

  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public int LineCount
  {
    get
    {
      TextDocument document = this.Document;
      return document != null ? document.LineCount : 1;
    }
  }

  public void Clear() => this.Text = string.Empty;

  public void Load(Stream stream)
  {
    using (StreamReader streamReader = FileReader.OpenStream(stream, this.Encoding ?? Encoding.UTF8))
    {
      this.Text = streamReader.ReadToEnd();
      this.SetCurrentValue(TextEditor.EncodingProperty, (object) streamReader.CurrentEncoding);
    }
    this.SetCurrentValue(TextEditor.IsModifiedProperty, Boxes.False);
  }

  public void Load(string fileName)
  {
    if (fileName == null)
      throw new ArgumentNullException(nameof (fileName));
    using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
      this.Load((Stream) fileStream);
  }

  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public Encoding Encoding
  {
    get => (Encoding) this.GetValue(TextEditor.EncodingProperty);
    set => this.SetValue(TextEditor.EncodingProperty, (object) value);
  }

  public void Save(Stream stream)
  {
    if (stream == null)
      throw new ArgumentNullException(nameof (stream));
    Encoding encoding = this.Encoding;
    TextDocument document = this.Document;
    StreamWriter writer = encoding != null ? new StreamWriter(stream, encoding) : new StreamWriter(stream);
    document?.WriteTextTo((TextWriter) writer);
    writer.Flush();
    this.SetCurrentValue(TextEditor.IsModifiedProperty, Boxes.False);
  }

  public void Save(string fileName)
  {
    if (fileName == null)
      throw new ArgumentNullException(nameof (fileName));
    using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
      this.Save((Stream) fileStream);
  }

  public event MouseEventHandler PreviewMouseHover
  {
    add => this.AddHandler(TextEditor.PreviewMouseHoverEvent, (Delegate) value);
    remove => this.RemoveHandler(TextEditor.PreviewMouseHoverEvent, (Delegate) value);
  }

  public event MouseEventHandler MouseHover
  {
    add => this.AddHandler(TextEditor.MouseHoverEvent, (Delegate) value);
    remove => this.RemoveHandler(TextEditor.MouseHoverEvent, (Delegate) value);
  }

  public event MouseEventHandler PreviewMouseHoverStopped
  {
    add => this.AddHandler(TextEditor.PreviewMouseHoverStoppedEvent, (Delegate) value);
    remove => this.RemoveHandler(TextEditor.PreviewMouseHoverStoppedEvent, (Delegate) value);
  }

  public event MouseEventHandler MouseHoverStopped
  {
    add => this.AddHandler(TextEditor.MouseHoverStoppedEvent, (Delegate) value);
    remove => this.RemoveHandler(TextEditor.MouseHoverStoppedEvent, (Delegate) value);
  }

  public ScrollBarVisibility HorizontalScrollBarVisibility
  {
    get => (ScrollBarVisibility) this.GetValue(TextEditor.HorizontalScrollBarVisibilityProperty);
    set => this.SetValue(TextEditor.HorizontalScrollBarVisibilityProperty, (object) value);
  }

  public ScrollBarVisibility VerticalScrollBarVisibility
  {
    get => (ScrollBarVisibility) this.GetValue(TextEditor.VerticalScrollBarVisibilityProperty);
    set => this.SetValue(TextEditor.VerticalScrollBarVisibilityProperty, (object) value);
  }

  object IServiceProvider.GetService(Type serviceType) => this.textArea.GetService(serviceType);

  public TextViewPosition? GetPositionFromPoint(Point point)
  {
    if (this.Document == null)
      return new TextViewPosition?();
    TextView textView = this.TextArea.TextView;
    return textView.GetPosition(this.TranslatePoint(point, (UIElement) textView) + textView.ScrollOffset);
  }

  public void ScrollToLine(int line) => this.ScrollTo(line, -1);

  public void ScrollTo(int line, int column)
  {
    TextView textView = this.textArea.TextView;
    TextDocument document = textView.Document;
    if (this.scrollViewer == null || document == null)
      return;
    if (line < 1)
      line = 1;
    if (line > document.LineCount)
      line = document.LineCount;
    if (!((IScrollInfo) textView).CanHorizontallyScroll)
    {
      VisualLine constructVisualLine = textView.GetOrConstructVisualLine(document.GetLineByNumber(line));
      for (double num = this.scrollViewer.ViewportHeight / 2.0; num > 0.0; num -= constructVisualLine.Height)
      {
        DocumentLine previousLine = constructVisualLine.FirstDocumentLine.PreviousLine;
        if (previousLine != null)
          constructVisualLine = textView.GetOrConstructVisualLine(previousLine);
        else
          break;
      }
    }
    Point visualPosition = this.textArea.TextView.GetVisualPosition(new TextViewPosition(line, Math.Max(1, column)), VisualYPosition.LineMiddle);
    double val2 = visualPosition.Y - this.scrollViewer.ViewportHeight / 2.0;
    if (Math.Abs(val2 - this.scrollViewer.VerticalOffset) > 0.3 * this.scrollViewer.ViewportHeight)
      this.scrollViewer.ScrollToVerticalOffset(Math.Max(0.0, val2));
    if (column <= 0)
      return;
    if (visualPosition.X > this.scrollViewer.ViewportWidth - 60.0)
    {
      double offset = Math.Max(0.0, visualPosition.X - this.scrollViewer.ViewportWidth / 2.0);
      if (Math.Abs(offset - this.scrollViewer.HorizontalOffset) <= 0.3 * this.scrollViewer.ViewportWidth)
        return;
      this.scrollViewer.ScrollToHorizontalOffset(offset);
    }
    else
      this.scrollViewer.ScrollToHorizontalOffset(0.0);
  }
}
