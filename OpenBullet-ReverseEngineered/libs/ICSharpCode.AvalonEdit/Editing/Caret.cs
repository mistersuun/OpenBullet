// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.Caret
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

public sealed class Caret
{
  internal const double MinimumDistanceToViewBorder = 30.0;
  private readonly TextArea textArea;
  private readonly TextView textView;
  private readonly CaretLayer caretAdorner;
  private bool visible;
  private double desiredXPos = double.NaN;
  private TextViewPosition position;
  private bool isInVirtualSpace;
  private int storedCaretOffset;
  private bool raisePositionChangedOnUpdateFinished;
  private bool visualColumnValid;
  private bool showScheduled;
  private bool hasWin32Caret;

  internal Caret(TextArea textArea)
  {
    this.textArea = textArea;
    this.textView = textArea.TextView;
    this.position = new TextViewPosition(1, 1, 0);
    this.caretAdorner = new CaretLayer(textArea);
    this.textView.InsertLayer((UIElement) this.caretAdorner, KnownLayer.Caret, LayerInsertionPosition.Replace);
    this.textView.VisualLinesChanged += new EventHandler(this.TextView_VisualLinesChanged);
    this.textView.ScrollOffsetChanged += new EventHandler(this.TextView_ScrollOffsetChanged);
  }

  internal void UpdateIfVisible()
  {
    if (!this.visible)
      return;
    this.Show();
  }

  private void TextView_VisualLinesChanged(object sender, EventArgs e)
  {
    if (this.visible)
      this.Show();
    this.InvalidateVisualColumn();
  }

  private void TextView_ScrollOffsetChanged(object sender, EventArgs e)
  {
    if (this.caretAdorner == null)
      return;
    this.caretAdorner.InvalidateVisual();
  }

  public TextViewPosition Position
  {
    get
    {
      this.ValidateVisualColumn();
      return this.position;
    }
    set
    {
      if (!(this.position != value))
        return;
      this.position = value;
      this.storedCaretOffset = -1;
      this.ValidatePosition();
      this.InvalidateVisualColumn();
      this.RaisePositionChanged();
      if (!this.visible)
        return;
      this.Show();
    }
  }

  internal TextViewPosition NonValidatedPosition => this.position;

  public TextLocation Location
  {
    get => this.position.Location;
    set => this.Position = new TextViewPosition(value);
  }

  public int Line
  {
    get => this.position.Line;
    set => this.Position = new TextViewPosition(value, this.position.Column);
  }

  public int Column
  {
    get => this.position.Column;
    set => this.Position = new TextViewPosition(this.position.Line, value);
  }

  public int VisualColumn
  {
    get
    {
      this.ValidateVisualColumn();
      return this.position.VisualColumn;
    }
    set => this.Position = new TextViewPosition(this.position.Line, this.position.Column, value);
  }

  public bool IsInVirtualSpace
  {
    get
    {
      this.ValidateVisualColumn();
      return this.isInVirtualSpace;
    }
  }

  internal void OnDocumentChanging()
  {
    this.storedCaretOffset = this.Offset;
    this.InvalidateVisualColumn();
  }

  internal void OnDocumentChanged(DocumentChangeEventArgs e)
  {
    this.InvalidateVisualColumn();
    if (this.storedCaretOffset >= 0)
    {
      AnchorMovementType movementType = this.textArea.Selection.IsEmpty || this.storedCaretOffset != this.textArea.Selection.SurroundingSegment.EndOffset ? AnchorMovementType.Default : AnchorMovementType.BeforeInsertion;
      int newOffset = e.GetNewOffset(this.storedCaretOffset, movementType);
      TextDocument document = this.textArea.Document;
      if (document != null)
        this.Position = new TextViewPosition(document.GetLocation(newOffset), this.position.VisualColumn);
    }
    this.storedCaretOffset = -1;
  }

  public int Offset
  {
    get
    {
      TextDocument document = this.textArea.Document;
      return document == null ? 0 : document.GetOffset(this.position.Location);
    }
    set
    {
      TextDocument document = this.textArea.Document;
      if (document == null)
        return;
      this.Position = new TextViewPosition(document.GetLocation(value));
      this.DesiredXPos = double.NaN;
    }
  }

  public double DesiredXPos
  {
    get => this.desiredXPos;
    set => this.desiredXPos = value;
  }

  private void ValidatePosition()
  {
    if (this.position.Line < 1)
      this.position.Line = 1;
    if (this.position.Column < 1)
      this.position.Column = 1;
    if (this.position.VisualColumn < -1)
      this.position.VisualColumn = -1;
    TextDocument document = this.textArea.Document;
    if (document == null)
      return;
    if (this.position.Line > document.LineCount)
    {
      this.position.Line = document.LineCount;
      this.position.Column = document.GetLineByNumber(this.position.Line).Length + 1;
      this.position.VisualColumn = -1;
    }
    else
    {
      DocumentLine lineByNumber = document.GetLineByNumber(this.position.Line);
      if (this.position.Column <= lineByNumber.Length + 1)
        return;
      this.position.Column = lineByNumber.Length + 1;
      this.position.VisualColumn = -1;
    }
  }

  public event EventHandler PositionChanged;

  private void RaisePositionChanged()
  {
    if (this.textArea.Document != null && this.textArea.Document.IsInUpdate)
    {
      this.raisePositionChangedOnUpdateFinished = true;
    }
    else
    {
      if (this.PositionChanged == null)
        return;
      this.PositionChanged((object) this, EventArgs.Empty);
    }
  }

  internal void OnDocumentUpdateFinished()
  {
    if (!this.raisePositionChangedOnUpdateFinished || this.PositionChanged == null)
      return;
    this.PositionChanged((object) this, EventArgs.Empty);
  }

  private void ValidateVisualColumn()
  {
    if (this.visualColumnValid)
      return;
    TextDocument document = this.textArea.Document;
    if (document == null)
      return;
    this.RevalidateVisualColumn(this.textView.GetOrConstructVisualLine(document.GetLineByNumber(this.position.Line)));
  }

  private void InvalidateVisualColumn() => this.visualColumnValid = false;

  private void RevalidateVisualColumn(VisualLine visualLine)
  {
    if (visualLine == null)
      throw new ArgumentNullException(nameof (visualLine));
    this.visualColumnValid = true;
    int offset1 = this.textView.Document.GetOffset(this.position.Location);
    int offset2 = visualLine.FirstDocumentLine.Offset;
    this.position.VisualColumn = visualLine.ValidateVisualColumn(this.position, this.textArea.Selection.EnableVirtualSpace);
    int nextCaretPosition1 = visualLine.GetNextCaretPosition(this.position.VisualColumn - 1, LogicalDirection.Forward, CaretPositioningMode.Normal, this.textArea.Selection.EnableVirtualSpace);
    if (nextCaretPosition1 != this.position.VisualColumn)
    {
      int nextCaretPosition2 = visualLine.GetNextCaretPosition(this.position.VisualColumn + 1, LogicalDirection.Backward, CaretPositioningMode.Normal, this.textArea.Selection.EnableVirtualSpace);
      if (nextCaretPosition1 < 0 && nextCaretPosition2 < 0)
        throw ThrowUtil.NoValidCaretPosition();
      int num1 = nextCaretPosition1 < 0 ? -1 : visualLine.GetRelativeOffset(nextCaretPosition1) + offset2;
      int num2 = nextCaretPosition2 < 0 ? -1 : visualLine.GetRelativeOffset(nextCaretPosition2) + offset2;
      int visualColumn;
      int offset3;
      if (nextCaretPosition1 < 0)
      {
        visualColumn = nextCaretPosition2;
        offset3 = num2;
      }
      else if (nextCaretPosition2 < 0)
      {
        visualColumn = nextCaretPosition1;
        offset3 = num1;
      }
      else if (Math.Abs(num2 - offset1) < Math.Abs(num1 - offset1))
      {
        visualColumn = nextCaretPosition2;
        offset3 = num2;
      }
      else
      {
        visualColumn = nextCaretPosition1;
        offset3 = num1;
      }
      this.Position = new TextViewPosition(this.textView.Document.GetLocation(offset3), visualColumn);
    }
    this.isInVirtualSpace = this.position.VisualColumn > visualLine.VisualLength;
  }

  private Rect CalcCaretRectangle(VisualLine visualLine)
  {
    if (!this.visualColumnValid)
      this.RevalidateVisualColumn(visualLine);
    TextLine textLine = visualLine.GetTextLine(this.position.VisualColumn, this.position.IsAtEndOfLine);
    double lineVisualXposition = visualLine.GetTextLineVisualXPosition(textLine, this.position.VisualColumn);
    double lineVisualYposition1 = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextTop);
    double lineVisualYposition2 = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextBottom);
    return new Rect(lineVisualXposition, lineVisualYposition1, SystemParameters.CaretWidth, lineVisualYposition2 - lineVisualYposition1);
  }

  private Rect CalcCaretOverstrikeRectangle(VisualLine visualLine)
  {
    if (!this.visualColumnValid)
      this.RevalidateVisualColumn(visualLine);
    int visualColumn = this.position.VisualColumn;
    int nextCaretPosition = visualLine.GetNextCaretPosition(visualColumn, LogicalDirection.Forward, CaretPositioningMode.Normal, true);
    TextLine textLine = visualLine.GetTextLine(visualColumn);
    Rect rect;
    if (visualColumn < visualLine.VisualLength)
    {
      rect = textLine.GetTextBounds(visualColumn, nextCaretPosition - visualColumn)[0].Rectangle;
      rect.Y += visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.LineTop);
    }
    else
    {
      double lineVisualXposition1 = visualLine.GetTextLineVisualXPosition(textLine, visualColumn);
      double lineVisualXposition2 = visualLine.GetTextLineVisualXPosition(textLine, nextCaretPosition);
      double lineVisualYposition1 = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextTop);
      double lineVisualYposition2 = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextBottom);
      rect = new Rect(lineVisualXposition1, lineVisualYposition1, lineVisualXposition2 - lineVisualXposition1, lineVisualYposition2 - lineVisualYposition1);
    }
    if (rect.Width < SystemParameters.CaretWidth)
      rect.Width = SystemParameters.CaretWidth;
    return rect;
  }

  public Rect CalculateCaretRectangle()
  {
    if (this.textView == null || this.textView.Document == null)
      return Rect.Empty;
    VisualLine constructVisualLine = this.textView.GetOrConstructVisualLine(this.textView.Document.GetLineByNumber(this.position.Line));
    return !this.textArea.OverstrikeMode ? this.CalcCaretRectangle(constructVisualLine) : this.CalcCaretOverstrikeRectangle(constructVisualLine);
  }

  public void BringCaretToView() => this.BringCaretToView(30.0);

  internal void BringCaretToView(double border)
  {
    Rect caretRectangle = this.CalculateCaretRectangle();
    if (caretRectangle.IsEmpty)
      return;
    caretRectangle.Inflate(border, border);
    this.textView.MakeVisible(caretRectangle);
  }

  public void Show()
  {
    this.visible = true;
    if (this.showScheduled)
      return;
    this.showScheduled = true;
    this.textArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate) new Action(this.ShowInternal));
  }

  private void ShowInternal()
  {
    this.showScheduled = false;
    if (!this.visible || this.caretAdorner == null || this.textView == null)
      return;
    VisualLine visualLine = this.textView.GetVisualLine(this.position.Line);
    if (visualLine != null)
    {
      Rect caretRectangle = this.textArea.OverstrikeMode ? this.CalcCaretOverstrikeRectangle(visualLine) : this.CalcCaretRectangle(visualLine);
      if (!this.hasWin32Caret)
        this.hasWin32Caret = Win32.CreateCaret((Visual) this.textView, caretRectangle.Size);
      if (this.hasWin32Caret)
        Win32.SetCaretPosition((Visual) this.textView, caretRectangle.Location - this.textView.ScrollOffset);
      this.caretAdorner.Show(caretRectangle);
      this.textArea.ime.UpdateCompositionWindow();
    }
    else
      this.caretAdorner.Hide();
  }

  public void Hide()
  {
    this.visible = false;
    if (this.hasWin32Caret)
    {
      Win32.DestroyCaret();
      this.hasWin32Caret = false;
    }
    if (this.caretAdorner == null)
      return;
    this.caretAdorner.Hide();
  }

  [Conditional("DEBUG")]
  private static void Log(string text)
  {
  }

  public Brush CaretBrush
  {
    get => this.caretAdorner.CaretBrush;
    set => this.caretAdorner.CaretBrush = value;
  }
}
