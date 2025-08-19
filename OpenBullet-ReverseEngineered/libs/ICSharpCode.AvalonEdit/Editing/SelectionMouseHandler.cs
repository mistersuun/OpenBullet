// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.SelectionMouseHandler
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

internal sealed class SelectionMouseHandler : ITextAreaInputHandler
{
  private readonly TextArea textArea;
  private MouseSelectionMode mode;
  private AnchorSegment startWord;
  private Point possibleDragStartMousePos;
  private bool enableTextDragDrop;
  private object currentDragDescriptor;

  internal SelectionMouseHandler(TextArea textArea)
  {
    this.textArea = textArea != null ? textArea : throw new ArgumentNullException(nameof (textArea));
  }

  static SelectionMouseHandler()
  {
    EventManager.RegisterClassHandler(typeof (TextArea), Mouse.LostMouseCaptureEvent, (Delegate) new MouseEventHandler(SelectionMouseHandler.OnLostMouseCapture));
  }

  private static void OnLostMouseCapture(object sender, MouseEventArgs e)
  {
    TextArea textArea = (TextArea) sender;
    if (Mouse.Captured == textArea || !(textArea.DefaultInputHandler.MouseSelection is SelectionMouseHandler mouseSelection))
      return;
    mouseSelection.mode = MouseSelectionMode.None;
  }

  TextArea ITextAreaInputHandler.TextArea => this.textArea;

  void ITextAreaInputHandler.Attach()
  {
    this.textArea.MouseLeftButtonDown += new MouseButtonEventHandler(this.textArea_MouseLeftButtonDown);
    this.textArea.MouseMove += new MouseEventHandler(this.textArea_MouseMove);
    this.textArea.MouseLeftButtonUp += new MouseButtonEventHandler(this.textArea_MouseLeftButtonUp);
    this.textArea.QueryCursor += new QueryCursorEventHandler(this.textArea_QueryCursor);
    this.textArea.OptionChanged += new PropertyChangedEventHandler(this.textArea_OptionChanged);
    this.enableTextDragDrop = this.textArea.Options.EnableTextDragDrop;
    if (!this.enableTextDragDrop)
      return;
    this.AttachDragDrop();
  }

  void ITextAreaInputHandler.Detach()
  {
    this.mode = MouseSelectionMode.None;
    this.textArea.MouseLeftButtonDown -= new MouseButtonEventHandler(this.textArea_MouseLeftButtonDown);
    this.textArea.MouseMove -= new MouseEventHandler(this.textArea_MouseMove);
    this.textArea.MouseLeftButtonUp -= new MouseButtonEventHandler(this.textArea_MouseLeftButtonUp);
    this.textArea.QueryCursor -= new QueryCursorEventHandler(this.textArea_QueryCursor);
    this.textArea.OptionChanged -= new PropertyChangedEventHandler(this.textArea_OptionChanged);
    if (!this.enableTextDragDrop)
      return;
    this.DetachDragDrop();
  }

  private void AttachDragDrop()
  {
    this.textArea.AllowDrop = true;
    this.textArea.GiveFeedback += new GiveFeedbackEventHandler(this.textArea_GiveFeedback);
    this.textArea.QueryContinueDrag += new QueryContinueDragEventHandler(this.textArea_QueryContinueDrag);
    this.textArea.DragEnter += new DragEventHandler(this.textArea_DragEnter);
    this.textArea.DragOver += new DragEventHandler(this.textArea_DragOver);
    this.textArea.DragLeave += new DragEventHandler(this.textArea_DragLeave);
    this.textArea.Drop += new DragEventHandler(this.textArea_Drop);
  }

  private void DetachDragDrop()
  {
    this.textArea.AllowDrop = false;
    this.textArea.GiveFeedback -= new GiveFeedbackEventHandler(this.textArea_GiveFeedback);
    this.textArea.QueryContinueDrag -= new QueryContinueDragEventHandler(this.textArea_QueryContinueDrag);
    this.textArea.DragEnter -= new DragEventHandler(this.textArea_DragEnter);
    this.textArea.DragOver -= new DragEventHandler(this.textArea_DragOver);
    this.textArea.DragLeave -= new DragEventHandler(this.textArea_DragLeave);
    this.textArea.Drop -= new DragEventHandler(this.textArea_Drop);
  }

  private void textArea_OptionChanged(object sender, PropertyChangedEventArgs e)
  {
    bool enableTextDragDrop = this.textArea.Options.EnableTextDragDrop;
    if (enableTextDragDrop == this.enableTextDragDrop)
      return;
    this.enableTextDragDrop = enableTextDragDrop;
    if (enableTextDragDrop)
      this.AttachDragDrop();
    else
      this.DetachDragDrop();
  }

  private void textArea_DragEnter(object sender, DragEventArgs e)
  {
    try
    {
      e.Effects = this.GetEffect(e);
      this.textArea.Caret.Show();
    }
    catch (Exception ex)
    {
      this.OnDragException(ex);
    }
  }

  private void textArea_DragOver(object sender, DragEventArgs e)
  {
    try
    {
      e.Effects = this.GetEffect(e);
    }
    catch (Exception ex)
    {
      this.OnDragException(ex);
    }
  }

  private DragDropEffects GetEffect(DragEventArgs e)
  {
    if (e.Data.GetDataPresent(DataFormats.UnicodeText, true))
    {
      e.Handled = true;
      int visualColumn;
      bool isAtEndOfLine;
      int fromMousePosition = this.GetOffsetFromMousePosition(e.GetPosition((IInputElement) this.textArea.TextView), out visualColumn, out isAtEndOfLine);
      if (fromMousePosition >= 0)
      {
        this.textArea.Caret.Position = new TextViewPosition(this.textArea.Document.GetLocation(fromMousePosition), visualColumn)
        {
          IsAtEndOfLine = isAtEndOfLine
        };
        this.textArea.Caret.DesiredXPos = double.NaN;
        if (this.textArea.ReadOnlySectionProvider.CanInsert(fromMousePosition))
          return (e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move && (e.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.ControlKey ? DragDropEffects.Move : e.AllowedEffects & DragDropEffects.Copy;
      }
    }
    return DragDropEffects.None;
  }

  private void textArea_DragLeave(object sender, DragEventArgs e)
  {
    try
    {
      e.Handled = true;
      if (this.textArea.IsKeyboardFocusWithin)
        return;
      this.textArea.Caret.Hide();
    }
    catch (Exception ex)
    {
      this.OnDragException(ex);
    }
  }

  private void textArea_Drop(object sender, DragEventArgs e)
  {
    try
    {
      DragDropEffects effect = this.GetEffect(e);
      e.Effects = effect;
      if (effect == DragDropEffects.None)
        return;
      int offset = this.textArea.Caret.Offset;
      if (this.mode == MouseSelectionMode.Drag && this.textArea.Selection.Contains(offset))
      {
        e.Effects = DragDropEffects.None;
      }
      else
      {
        DataObjectPastingEventArgs pastingEventArgs = new DataObjectPastingEventArgs(e.Data, true, DataFormats.UnicodeText);
        this.textArea.RaiseEvent((RoutedEventArgs) pastingEventArgs);
        if (pastingEventArgs.CommandCancelled)
          return;
        string textToPaste = EditingCommandHandler.GetTextToPaste(pastingEventArgs, this.textArea);
        if (textToPaste == null)
          return;
        bool dataPresent = pastingEventArgs.DataObject.GetDataPresent("AvalonEditRectangularSelection");
        this.textArea.Document.UndoStack.StartUndoGroup(this.currentDragDescriptor);
        try
        {
          if (dataPresent)
          {
            if (RectangleSelection.PerformRectangularPaste(this.textArea, this.textArea.Caret.Position, textToPaste, true))
              goto label_13;
          }
          this.textArea.Document.Insert(offset, textToPaste);
          this.textArea.Selection = Selection.Create(this.textArea, offset, offset + textToPaste.Length);
        }
        finally
        {
          this.textArea.Document.UndoStack.EndUndoGroup();
        }
      }
label_13:
      e.Handled = true;
    }
    catch (Exception ex)
    {
      this.OnDragException(ex);
    }
  }

  private void OnDragException(Exception ex)
  {
    this.textArea.Dispatcher.BeginInvoke(DispatcherPriority.Send, (Delegate) (() =>
    {
      throw new DragDropException("Exception during drag'n'drop", ex);
    }));
  }

  private void textArea_GiveFeedback(object sender, GiveFeedbackEventArgs e)
  {
    try
    {
      e.UseDefaultCursors = true;
      e.Handled = true;
    }
    catch (Exception ex)
    {
      this.OnDragException(ex);
    }
  }

  private void textArea_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
  {
    try
    {
      e.Action = !e.EscapePressed ? ((e.KeyStates & DragDropKeyStates.LeftMouseButton) == DragDropKeyStates.LeftMouseButton ? DragAction.Continue : DragAction.Drop) : DragAction.Cancel;
      e.Handled = true;
    }
    catch (Exception ex)
    {
      this.OnDragException(ex);
    }
  }

  private void StartDrag()
  {
    this.mode = MouseSelectionMode.Drag;
    this.textArea.ReleaseMouseCapture();
    DataObject dataObject = this.textArea.Selection.CreateDataObject(this.textArea);
    DragDropEffects allowedEffects = DragDropEffects.All;
    List<AnchorSegment> list = this.textArea.Selection.Segments.Select<SelectionSegment, AnchorSegment>((Func<SelectionSegment, AnchorSegment>) (s => new AnchorSegment(this.textArea.Document, (ISegment) s))).ToList<AnchorSegment>();
    foreach (ISegment segment in list)
    {
      ISegment[] deletableSegments = this.textArea.GetDeletableSegments(segment);
      if (deletableSegments.Length != 1 || deletableSegments[0].Offset != segment.Offset || deletableSegments[0].EndOffset != segment.EndOffset)
        allowedEffects &= ~DragDropEffects.Move;
    }
    DataObjectCopyingEventArgs e = new DataObjectCopyingEventArgs((IDataObject) dataObject, true);
    this.textArea.RaiseEvent((RoutedEventArgs) e);
    if (e.CommandCancelled)
      return;
    object obj = new object();
    this.currentDragDescriptor = obj;
    DragDropEffects dragDropEffects;
    using (this.textArea.AllowCaretOutsideSelection())
    {
      TextViewPosition position = this.textArea.Caret.Position;
      try
      {
        dragDropEffects = DragDrop.DoDragDrop((DependencyObject) this.textArea, (object) dataObject, allowedEffects);
      }
      catch (COMException ex)
      {
        return;
      }
      if (dragDropEffects == DragDropEffects.None)
        this.textArea.Caret.Position = position;
    }
    this.currentDragDescriptor = (object) null;
    if (list == null || dragDropEffects != DragDropEffects.Move || (allowedEffects & DragDropEffects.Move) != DragDropEffects.Move)
      return;
    bool flag = obj == this.textArea.Document.UndoStack.LastGroupDescriptor;
    if (flag)
      this.textArea.Document.UndoStack.StartContinuedUndoGroup();
    this.textArea.Document.BeginUpdate();
    try
    {
      foreach (ISegment segment in list)
        this.textArea.Document.Remove(segment.Offset, segment.Length);
    }
    finally
    {
      this.textArea.Document.EndUpdate();
      if (flag)
        this.textArea.Document.UndoStack.EndUndoGroup();
    }
  }

  private void textArea_QueryCursor(object sender, QueryCursorEventArgs e)
  {
    if (e.Handled)
      return;
    if (this.mode != MouseSelectionMode.None)
    {
      e.Cursor = Cursors.IBeam;
      e.Handled = true;
    }
    else
    {
      if (!this.textArea.TextView.VisualLinesValid)
        return;
      Point position = e.GetPosition((IInputElement) this.textArea.TextView);
      if (position.X < 0.0 || position.Y < 0.0 || position.X > this.textArea.TextView.ActualWidth || position.Y > this.textArea.TextView.ActualHeight)
        return;
      int fromMousePosition = this.GetOffsetFromMousePosition((MouseEventArgs) e, out int _, out bool _);
      e.Cursor = !this.enableTextDragDrop || !this.textArea.Selection.Contains(fromMousePosition) ? Cursors.IBeam : Cursors.Arrow;
      e.Handled = true;
    }
  }

  private void textArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    this.mode = MouseSelectionMode.None;
    if (!e.Handled && e.ChangedButton == MouseButton.Left)
    {
      ModifierKeys modifiers = Keyboard.Modifiers;
      bool flag = (modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
      if (this.enableTextDragDrop && e.ClickCount == 1 && !flag && this.textArea.Selection.Contains(this.GetOffsetFromMousePosition((MouseEventArgs) e, out int _, out bool _)))
      {
        if (this.textArea.CaptureMouse())
        {
          this.mode = MouseSelectionMode.PossibleDragStart;
          this.possibleDragStartMousePos = e.GetPosition((IInputElement) this.textArea);
        }
        e.Handled = true;
        return;
      }
      TextViewPosition position = this.textArea.Caret.Position;
      this.SetCaretOffsetToMousePosition((MouseEventArgs) e);
      if (!flag)
        this.textArea.ClearSelection();
      if (this.textArea.CaptureMouse())
      {
        if ((modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && this.textArea.Options.EnableRectangularSelection)
        {
          this.mode = MouseSelectionMode.Rectangular;
          if (flag && this.textArea.Selection is RectangleSelection)
            this.textArea.Selection = this.textArea.Selection.StartSelectionOrSetEndpoint(position, this.textArea.Caret.Position);
        }
        else if (e.ClickCount == 1 && (modifiers & ModifierKeys.Control) == ModifierKeys.None)
        {
          this.mode = MouseSelectionMode.Normal;
          if (flag && !(this.textArea.Selection is RectangleSelection))
            this.textArea.Selection = this.textArea.Selection.StartSelectionOrSetEndpoint(position, this.textArea.Caret.Position);
        }
        else
        {
          SimpleSegment simpleSegment;
          if (e.ClickCount == 3)
          {
            this.mode = MouseSelectionMode.WholeLine;
            simpleSegment = this.GetLineAtMousePosition((MouseEventArgs) e);
          }
          else
          {
            this.mode = MouseSelectionMode.WholeWord;
            simpleSegment = this.GetWordAtMousePosition((MouseEventArgs) e);
          }
          if (simpleSegment == SimpleSegment.Invalid)
          {
            this.mode = MouseSelectionMode.None;
            this.textArea.ReleaseMouseCapture();
            return;
          }
          if (flag && !this.textArea.Selection.IsEmpty)
          {
            if (simpleSegment.Offset < this.textArea.Selection.SurroundingSegment.Offset)
              this.textArea.Selection = this.textArea.Selection.SetEndpoint(new TextViewPosition(this.textArea.Document.GetLocation(simpleSegment.Offset)));
            else if (simpleSegment.EndOffset > this.textArea.Selection.SurroundingSegment.EndOffset)
              this.textArea.Selection = this.textArea.Selection.SetEndpoint(new TextViewPosition(this.textArea.Document.GetLocation(simpleSegment.EndOffset)));
            this.startWord = new AnchorSegment(this.textArea.Document, this.textArea.Selection.SurroundingSegment);
          }
          else
          {
            this.textArea.Selection = Selection.Create(this.textArea, simpleSegment.Offset, simpleSegment.EndOffset);
            this.startWord = new AnchorSegment(this.textArea.Document, simpleSegment.Offset, simpleSegment.Length);
          }
        }
      }
    }
    e.Handled = true;
  }

  public MouseSelectionMode MouseSelectionMode
  {
    get => this.mode;
    set
    {
      if (this.mode == value)
        return;
      if (value == MouseSelectionMode.None)
      {
        this.mode = MouseSelectionMode.None;
        this.textArea.ReleaseMouseCapture();
      }
      else
      {
        if (!this.textArea.CaptureMouse())
          return;
        switch (value)
        {
          case MouseSelectionMode.Normal:
          case MouseSelectionMode.Rectangular:
            this.mode = value;
            break;
          default:
            throw new NotImplementedException("Programmatically starting mouse selection is only supported for normal and rectangular selections.");
        }
      }
    }
  }

  private SimpleSegment GetWordAtMousePosition(MouseEventArgs e)
  {
    TextView textView = this.textArea.TextView;
    if (textView == null)
      return SimpleSegment.Invalid;
    Point position = e.GetPosition((IInputElement) textView);
    if (position.Y < 0.0)
      position.Y = 0.0;
    if (position.Y > textView.ActualHeight)
      position.Y = textView.ActualHeight;
    position += textView.ScrollOffset;
    VisualLine lineFromVisualTop = textView.GetVisualLineFromVisualTop(position.Y);
    if (lineFromVisualTop == null)
      return SimpleSegment.Invalid;
    int visualColumn1 = lineFromVisualTop.GetVisualColumn(position, this.textArea.Selection.EnableVirtualSpace);
    int visualColumn2 = lineFromVisualTop.GetNextCaretPosition(visualColumn1 + 1, LogicalDirection.Backward, CaretPositioningMode.WordStartOrSymbol, this.textArea.Selection.EnableVirtualSpace);
    if (visualColumn2 == -1)
      visualColumn2 = 0;
    int visualColumn3 = lineFromVisualTop.GetNextCaretPosition(visualColumn2, LogicalDirection.Forward, CaretPositioningMode.WordBorderOrSymbol, this.textArea.Selection.EnableVirtualSpace);
    if (visualColumn3 == -1)
      visualColumn3 = lineFromVisualTop.VisualLength;
    int offset1 = lineFromVisualTop.FirstDocumentLine.Offset;
    int offset2 = lineFromVisualTop.GetRelativeOffset(visualColumn2) + offset1;
    int num = lineFromVisualTop.GetRelativeOffset(visualColumn3) + offset1;
    return new SimpleSegment(offset2, num - offset2);
  }

  private SimpleSegment GetLineAtMousePosition(MouseEventArgs e)
  {
    TextView textView = this.textArea.TextView;
    if (textView == null)
      return SimpleSegment.Invalid;
    Point position = e.GetPosition((IInputElement) textView);
    if (position.Y < 0.0)
      position.Y = 0.0;
    if (position.Y > textView.ActualHeight)
      position.Y = textView.ActualHeight;
    position += textView.ScrollOffset;
    VisualLine lineFromVisualTop = textView.GetVisualLineFromVisualTop(position.Y);
    return lineFromVisualTop != null ? new SimpleSegment(lineFromVisualTop.StartOffset, lineFromVisualTop.LastDocumentLine.EndOffset - lineFromVisualTop.StartOffset) : SimpleSegment.Invalid;
  }

  private int GetOffsetFromMousePosition(
    MouseEventArgs e,
    out int visualColumn,
    out bool isAtEndOfLine)
  {
    return this.GetOffsetFromMousePosition(e.GetPosition((IInputElement) this.textArea.TextView), out visualColumn, out isAtEndOfLine);
  }

  private int GetOffsetFromMousePosition(
    Point positionRelativeToTextView,
    out int visualColumn,
    out bool isAtEndOfLine)
  {
    visualColumn = 0;
    TextView textView = this.textArea.TextView;
    Point point1 = positionRelativeToTextView;
    if (point1.Y < 0.0)
      point1.Y = 0.0;
    if (point1.Y > textView.ActualHeight)
      point1.Y = textView.ActualHeight;
    Point point2 = point1 + textView.ScrollOffset;
    if (point2.Y >= textView.DocumentHeight)
      point2.Y = textView.DocumentHeight - 0.01;
    VisualLine lineFromVisualTop = textView.GetVisualLineFromVisualTop(point2.Y);
    if (lineFromVisualTop != null)
    {
      visualColumn = lineFromVisualTop.GetVisualColumn(point2, this.textArea.Selection.EnableVirtualSpace, out isAtEndOfLine);
      return lineFromVisualTop.GetRelativeOffset(visualColumn) + lineFromVisualTop.FirstDocumentLine.Offset;
    }
    isAtEndOfLine = false;
    return -1;
  }

  private int GetOffsetFromMousePositionFirstTextLineOnly(
    Point positionRelativeToTextView,
    out int visualColumn)
  {
    visualColumn = 0;
    TextView textView = this.textArea.TextView;
    Point point1 = positionRelativeToTextView;
    if (point1.Y < 0.0)
      point1.Y = 0.0;
    if (point1.Y > textView.ActualHeight)
      point1.Y = textView.ActualHeight;
    Point point2 = point1 + textView.ScrollOffset;
    if (point2.Y >= textView.DocumentHeight)
      point2.Y = textView.DocumentHeight - 0.01;
    VisualLine lineFromVisualTop = textView.GetVisualLineFromVisualTop(point2.Y);
    if (lineFromVisualTop == null)
      return -1;
    visualColumn = lineFromVisualTop.GetVisualColumn(lineFromVisualTop.TextLines.First<TextLine>(), point2.X, this.textArea.Selection.EnableVirtualSpace);
    return lineFromVisualTop.GetRelativeOffset(visualColumn) + lineFromVisualTop.FirstDocumentLine.Offset;
  }

  private void textArea_MouseMove(object sender, MouseEventArgs e)
  {
    if (e.Handled)
      return;
    if (this.mode == MouseSelectionMode.Normal || this.mode == MouseSelectionMode.WholeWord || this.mode == MouseSelectionMode.WholeLine || this.mode == MouseSelectionMode.Rectangular)
    {
      e.Handled = true;
      if (!this.textArea.TextView.VisualLinesValid)
        return;
      this.ExtendSelectionToMouse(e);
    }
    else
    {
      if (this.mode != MouseSelectionMode.PossibleDragStart)
        return;
      e.Handled = true;
      Vector vector = e.GetPosition((IInputElement) this.textArea) - this.possibleDragStartMousePos;
      if (Math.Abs(vector.X) <= SystemParameters.MinimumHorizontalDragDistance && Math.Abs(vector.Y) <= SystemParameters.MinimumVerticalDragDistance)
        return;
      this.StartDrag();
    }
  }

  private void SetCaretOffsetToMousePosition(MouseEventArgs e)
  {
    this.SetCaretOffsetToMousePosition(e, (ISegment) null);
  }

  private void SetCaretOffsetToMousePosition(MouseEventArgs e, ISegment allowedSegment)
  {
    int visualColumn;
    int offset;
    bool isAtEndOfLine;
    if (this.mode == MouseSelectionMode.Rectangular)
    {
      offset = this.GetOffsetFromMousePositionFirstTextLineOnly(e.GetPosition((IInputElement) this.textArea.TextView), out visualColumn);
      isAtEndOfLine = true;
    }
    else
      offset = this.GetOffsetFromMousePosition(e, out visualColumn, out isAtEndOfLine);
    if (allowedSegment != null)
      offset = offset.CoerceValue(allowedSegment.Offset, allowedSegment.EndOffset);
    if (offset < 0)
      return;
    this.textArea.Caret.Position = new TextViewPosition(this.textArea.Document.GetLocation(offset), visualColumn)
    {
      IsAtEndOfLine = isAtEndOfLine
    };
    this.textArea.Caret.DesiredXPos = double.NaN;
  }

  private void ExtendSelectionToMouse(MouseEventArgs e)
  {
    TextViewPosition position = this.textArea.Caret.Position;
    if (this.mode == MouseSelectionMode.Normal || this.mode == MouseSelectionMode.Rectangular)
    {
      this.SetCaretOffsetToMousePosition(e);
      this.textArea.Selection = this.mode != MouseSelectionMode.Normal || !(this.textArea.Selection is RectangleSelection) ? (this.mode != MouseSelectionMode.Rectangular || this.textArea.Selection is RectangleSelection ? this.textArea.Selection.StartSelectionOrSetEndpoint(position, this.textArea.Caret.Position) : (Selection) new RectangleSelection(this.textArea, position, this.textArea.Caret.Position)) : (Selection) new SimpleSelection(this.textArea, position, this.textArea.Caret.Position);
    }
    else if (this.mode == MouseSelectionMode.WholeWord || this.mode == MouseSelectionMode.WholeLine)
    {
      SimpleSegment simpleSegment = this.mode == MouseSelectionMode.WholeLine ? this.GetLineAtMousePosition(e) : this.GetWordAtMousePosition(e);
      if (simpleSegment != SimpleSegment.Invalid)
      {
        this.textArea.Selection = Selection.Create(this.textArea, Math.Min(simpleSegment.Offset, this.startWord.Offset), Math.Max(simpleSegment.EndOffset, this.startWord.EndOffset));
        this.textArea.Caret.Offset = simpleSegment.Offset >= this.startWord.Offset ? Math.Max(simpleSegment.EndOffset, this.startWord.EndOffset) : simpleSegment.Offset;
      }
    }
    this.textArea.Caret.BringCaretToView(5.0);
  }

  private void textArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
  {
    if (this.mode == MouseSelectionMode.None || e.Handled)
      return;
    e.Handled = true;
    if (this.mode == MouseSelectionMode.PossibleDragStart)
    {
      this.SetCaretOffsetToMousePosition((MouseEventArgs) e);
      this.textArea.ClearSelection();
    }
    else if (this.mode == MouseSelectionMode.Normal || this.mode == MouseSelectionMode.WholeWord || this.mode == MouseSelectionMode.WholeLine || this.mode == MouseSelectionMode.Rectangular)
      this.ExtendSelectionToMouse((MouseEventArgs) e);
    this.mode = MouseSelectionMode.None;
    this.textArea.ReleaseMouseCapture();
  }
}
