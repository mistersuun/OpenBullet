// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.CodeCompletion.CompletionWindowBase
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

#nullable disable
namespace ICSharpCode.AvalonEdit.CodeCompletion;

public class CompletionWindowBase : Window
{
  private Window parentWindow;
  private TextDocument document;
  private CompletionWindowBase.InputHandler myInputHandler;
  private bool sourceIsInitialized;
  private Point visualLocation;
  private Point visualLocationTop;

  static CompletionWindowBase()
  {
    Window.WindowStyleProperty.OverrideMetadata(typeof (CompletionWindowBase), (PropertyMetadata) new FrameworkPropertyMetadata((object) WindowStyle.None));
    Window.ShowActivatedProperty.OverrideMetadata(typeof (CompletionWindowBase), (PropertyMetadata) new FrameworkPropertyMetadata(Boxes.False));
    Window.ShowInTaskbarProperty.OverrideMetadata(typeof (CompletionWindowBase), (PropertyMetadata) new FrameworkPropertyMetadata(Boxes.False));
  }

  public TextArea TextArea { get; private set; }

  public int StartOffset { get; set; }

  public int EndOffset { get; set; }

  protected bool IsUp { get; private set; }

  public CompletionWindowBase(TextArea textArea)
  {
    this.TextArea = textArea != null ? textArea : throw new ArgumentNullException(nameof (textArea));
    this.parentWindow = Window.GetWindow((DependencyObject) textArea);
    this.Owner = this.parentWindow;
    this.AddHandler(UIElement.MouseUpEvent, (Delegate) new MouseButtonEventHandler(this.OnMouseUp), true);
    this.StartOffset = this.EndOffset = this.TextArea.Caret.Offset;
    this.AttachEvents();
  }

  private void AttachEvents()
  {
    this.document = this.TextArea.Document;
    if (this.document != null)
      this.document.Changing += new EventHandler<DocumentChangeEventArgs>(this.textArea_Document_Changing);
    this.TextArea.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(this.TextAreaLostFocus);
    this.TextArea.TextView.ScrollOffsetChanged += new EventHandler(this.TextViewScrollOffsetChanged);
    this.TextArea.DocumentChanged += new EventHandler(this.TextAreaDocumentChanged);
    if (this.parentWindow != null)
      this.parentWindow.LocationChanged += new EventHandler(this.parentWindow_LocationChanged);
    foreach (CompletionWindowBase.InputHandler inputHandler in this.TextArea.StackedInputHandlers.OfType<CompletionWindowBase.InputHandler>())
    {
      if (inputHandler.window.GetType() == this.GetType())
        this.TextArea.PopStackedInputHandler((TextAreaStackedInputHandler) inputHandler);
    }
    this.myInputHandler = new CompletionWindowBase.InputHandler(this);
    this.TextArea.PushStackedInputHandler((TextAreaStackedInputHandler) this.myInputHandler);
  }

  protected virtual void DetachEvents()
  {
    if (this.document != null)
      this.document.Changing -= new EventHandler<DocumentChangeEventArgs>(this.textArea_Document_Changing);
    this.TextArea.LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(this.TextAreaLostFocus);
    this.TextArea.TextView.ScrollOffsetChanged -= new EventHandler(this.TextViewScrollOffsetChanged);
    this.TextArea.DocumentChanged -= new EventHandler(this.TextAreaDocumentChanged);
    if (this.parentWindow != null)
      this.parentWindow.LocationChanged -= new EventHandler(this.parentWindow_LocationChanged);
    this.TextArea.PopStackedInputHandler((TextAreaStackedInputHandler) this.myInputHandler);
  }

  private void TextViewScrollOffsetChanged(object sender, EventArgs e)
  {
    if (!this.sourceIsInitialized)
      return;
    IScrollInfo textView = (IScrollInfo) this.TextArea.TextView;
    Rect rect = new Rect(textView.HorizontalOffset, textView.VerticalOffset, textView.ViewportWidth, textView.ViewportHeight);
    if (rect.Contains(this.visualLocation) || rect.Contains(this.visualLocationTop))
      this.UpdatePosition();
    else
      this.Close();
  }

  private void TextAreaDocumentChanged(object sender, EventArgs e) => this.Close();

  private void TextAreaLostFocus(object sender, RoutedEventArgs e)
  {
    this.Dispatcher.BeginInvoke((Delegate) new Action(this.CloseIfFocusLost), DispatcherPriority.Background);
  }

  private void parentWindow_LocationChanged(object sender, EventArgs e) => this.UpdatePosition();

  protected override void OnDeactivated(EventArgs e)
  {
    base.OnDeactivated(e);
    this.Dispatcher.BeginInvoke((Delegate) new Action(this.CloseIfFocusLost), DispatcherPriority.Background);
  }

  protected static bool RaiseEventPair(
    UIElement target,
    RoutedEvent previewEvent,
    RoutedEvent @event,
    RoutedEventArgs args)
  {
    if (target == null)
      throw new ArgumentNullException(nameof (target));
    if (previewEvent == null)
      throw new ArgumentNullException(nameof (previewEvent));
    if (@event == null)
      throw new ArgumentNullException(nameof (@event));
    if (args == null)
      throw new ArgumentNullException(nameof (args));
    args.RoutedEvent = previewEvent;
    target.RaiseEvent(args);
    args.RoutedEvent = @event;
    target.RaiseEvent(args);
    return args.Handled;
  }

  private void OnMouseUp(object sender, MouseButtonEventArgs e) => this.ActivateParentWindow();

  protected virtual void ActivateParentWindow()
  {
    if (this.parentWindow == null)
      return;
    this.parentWindow.Activate();
  }

  private void CloseIfFocusLost()
  {
    if (!this.CloseOnFocusLost || this.IsActive || this.IsTextAreaFocused)
      return;
    this.Close();
  }

  protected virtual bool CloseOnFocusLost => true;

  private bool IsTextAreaFocused
  {
    get
    {
      return (this.parentWindow == null || this.parentWindow.IsActive) && this.TextArea.IsKeyboardFocused;
    }
  }

  protected override void OnSourceInitialized(EventArgs e)
  {
    base.OnSourceInitialized(e);
    if (this.document != null && this.StartOffset != this.TextArea.Caret.Offset)
      this.SetPosition(new TextViewPosition(this.document.GetLocation(this.StartOffset)));
    else
      this.SetPosition(this.TextArea.Caret.Position);
    this.sourceIsInitialized = true;
  }

  protected override void OnClosed(EventArgs e)
  {
    base.OnClosed(e);
    this.DetachEvents();
  }

  protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
  {
    base.OnKeyDown(e);
    if (e.Handled || e.Key != Key.Escape)
      return;
    e.Handled = true;
    this.Close();
  }

  protected void SetPosition(TextViewPosition position)
  {
    TextView textView = this.TextArea.TextView;
    this.visualLocation = textView.GetVisualPosition(position, VisualYPosition.LineBottom);
    this.visualLocationTop = textView.GetVisualPosition(position, VisualYPosition.LineTop);
    this.UpdatePosition();
  }

  protected void UpdatePosition()
  {
    TextView textView = this.TextArea.TextView;
    Point screen1 = textView.PointToScreen(this.visualLocation - textView.ScrollOffset);
    Point screen2 = textView.PointToScreen(this.visualLocationTop - textView.ScrollOffset);
    Size device = new Size(this.ActualWidth, this.ActualHeight).TransformToDevice((Visual) textView);
    Rect rect = new Rect(screen1, device);
    Rect wpf = Screen.GetWorkingArea(screen1.ToSystemDrawing()).ToWpf();
    if (!wpf.Contains(rect))
    {
      if (rect.Left < wpf.Left)
        rect.X = wpf.Left;
      else if (rect.Right > wpf.Right)
        rect.X = wpf.Right - rect.Width;
      if (rect.Bottom > wpf.Bottom)
      {
        rect.Y = screen2.Y - rect.Height;
        this.IsUp = true;
      }
      else
        this.IsUp = false;
      if (rect.Y < wpf.Top)
        rect.Y = wpf.Top;
    }
    rect = rect.TransformFromDevice((Visual) textView);
    this.Left = rect.X;
    this.Top = rect.Y;
  }

  protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
  {
    base.OnRenderSizeChanged(sizeInfo);
    if (!sizeInfo.HeightChanged || !this.IsUp)
      return;
    this.Top += sizeInfo.PreviousSize.Height - sizeInfo.NewSize.Height;
  }

  public bool ExpectInsertionBeforeStart { get; set; }

  private void textArea_Document_Changing(object sender, DocumentChangeEventArgs e)
  {
    if (e.Offset + e.RemovalLength == this.StartOffset && e.RemovalLength > 0)
      this.Close();
    if (e.Offset == this.StartOffset && e.RemovalLength == 0 && this.ExpectInsertionBeforeStart)
    {
      this.StartOffset = e.GetNewOffset(this.StartOffset, AnchorMovementType.AfterInsertion);
      this.ExpectInsertionBeforeStart = false;
    }
    else
      this.StartOffset = e.GetNewOffset(this.StartOffset, AnchorMovementType.BeforeInsertion);
    this.EndOffset = e.GetNewOffset(this.EndOffset, AnchorMovementType.AfterInsertion);
  }

  private sealed class InputHandler : TextAreaStackedInputHandler
  {
    private const Key KeyDeadCharProcessed = Key.DeadCharProcessed;
    internal readonly CompletionWindowBase window;

    public InputHandler(CompletionWindowBase window)
      : base(window.TextArea)
    {
      this.window = window;
    }

    public override void Detach()
    {
      base.Detach();
      this.window.Close();
    }

    public override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == Key.DeadCharProcessed)
        return;
      e.Handled = CompletionWindowBase.RaiseEventPair((UIElement) this.window, UIElement.PreviewKeyDownEvent, UIElement.KeyDownEvent, (RoutedEventArgs) new System.Windows.Input.KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
    }

    public override void OnPreviewKeyUp(System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == Key.DeadCharProcessed)
        return;
      e.Handled = CompletionWindowBase.RaiseEventPair((UIElement) this.window, UIElement.PreviewKeyUpEvent, UIElement.KeyUpEvent, (RoutedEventArgs) new System.Windows.Input.KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
    }
  }
}
