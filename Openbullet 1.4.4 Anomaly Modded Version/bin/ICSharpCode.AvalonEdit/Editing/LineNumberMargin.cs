// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.LineNumberMargin
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

public class LineNumberMargin : AbstractMargin, IWeakEventListener
{
  private TextArea textArea;
  protected Typeface typeface;
  protected double emSize;
  protected int maxLineNumberLength = 1;
  private AnchorSegment selectionStart;
  private bool selecting;

  static LineNumberMargin()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (LineNumberMargin), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (LineNumberMargin)));
  }

  protected override Size MeasureOverride(Size availableSize)
  {
    this.typeface = this.CreateTypeface();
    this.emSize = (double) this.GetValue(TextBlock.FontSizeProperty);
    return new Size(TextFormatterFactory.CreateFormattedText((FrameworkElement) this, new string('9', this.maxLineNumberLength), this.typeface, new double?(this.emSize), (Brush) this.GetValue(Control.ForegroundProperty)).Width, 0.0);
  }

  protected override void OnRender(DrawingContext drawingContext)
  {
    TextView textView = this.TextView;
    Size renderSize = this.RenderSize;
    if (textView == null || !textView.VisualLinesValid)
      return;
    Brush foreground = (Brush) this.GetValue(Control.ForegroundProperty);
    foreach (VisualLine visualLine in textView.VisualLines)
    {
      FormattedText formattedText = TextFormatterFactory.CreateFormattedText((FrameworkElement) this, visualLine.FirstDocumentLine.LineNumber.ToString((IFormatProvider) CultureInfo.CurrentCulture), this.typeface, new double?(this.emSize), foreground);
      double lineVisualYposition = visualLine.GetTextLineVisualYPosition(visualLine.TextLines[0], VisualYPosition.TextTop);
      drawingContext.DrawText(formattedText, new Point(renderSize.Width - formattedText.Width, lineVisualYposition - textView.VerticalOffset));
    }
  }

  protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
  {
    if (oldTextView != null)
      oldTextView.VisualLinesChanged -= new EventHandler(this.TextViewVisualLinesChanged);
    base.OnTextViewChanged(oldTextView, newTextView);
    if (newTextView != null)
    {
      newTextView.VisualLinesChanged += new EventHandler(this.TextViewVisualLinesChanged);
      this.textArea = newTextView.GetService(typeof (TextArea)) as TextArea;
    }
    else
      this.textArea = (TextArea) null;
    this.InvalidateVisual();
  }

  protected override void OnDocumentChanged(TextDocument oldDocument, TextDocument newDocument)
  {
    if (oldDocument != null)
      PropertyChangedEventManager.RemoveListener((INotifyPropertyChanged) oldDocument, (IWeakEventListener) this, "LineCount");
    base.OnDocumentChanged(oldDocument, newDocument);
    if (newDocument != null)
      PropertyChangedEventManager.AddListener((INotifyPropertyChanged) newDocument, (IWeakEventListener) this, "LineCount");
    this.OnDocumentLineCountChanged();
  }

  protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    if (!(managerType == typeof (PropertyChangedEventManager)))
      return false;
    this.OnDocumentLineCountChanged();
    return true;
  }

  bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
  {
    return this.ReceiveWeakEvent(managerType, sender, e);
  }

  private void OnDocumentLineCountChanged()
  {
    int num = (this.Document != null ? this.Document.LineCount : 1).ToString((IFormatProvider) CultureInfo.CurrentCulture).Length;
    if (num < 2)
      num = 2;
    if (num == this.maxLineNumberLength)
      return;
    this.maxLineNumberLength = num;
    this.InvalidateMeasure();
  }

  private void TextViewVisualLinesChanged(object sender, EventArgs e) => this.InvalidateVisual();

  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    base.OnMouseLeftButtonDown(e);
    if (e.Handled || this.TextView == null || this.textArea == null)
      return;
    e.Handled = true;
    this.textArea.Focus();
    SimpleSegment textLineSegment = this.GetTextLineSegment((MouseEventArgs) e);
    if (textLineSegment == SimpleSegment.Invalid)
      return;
    this.textArea.Caret.Offset = textLineSegment.Offset + textLineSegment.Length;
    if (!this.CaptureMouse())
      return;
    this.selecting = true;
    this.selectionStart = new AnchorSegment(this.Document, textLineSegment.Offset, textLineSegment.Length);
    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift && this.textArea.Selection is SimpleSelection selection)
      this.selectionStart = new AnchorSegment(this.Document, selection.SurroundingSegment);
    this.textArea.Selection = Selection.Create(this.textArea, (ISegment) this.selectionStart);
    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
      this.ExtendSelection(textLineSegment);
    this.textArea.Caret.BringCaretToView(5.0);
  }

  private SimpleSegment GetTextLineSegment(MouseEventArgs e)
  {
    Point position = e.GetPosition((IInputElement) this.TextView) with
    {
      X = 0.0
    };
    position.Y = position.Y.CoerceValue(0.0, this.TextView.ActualHeight);
    position.Y += this.TextView.VerticalOffset;
    VisualLine lineFromVisualTop = this.TextView.GetVisualLineFromVisualTop(position.Y);
    if (lineFromVisualTop == null)
      return SimpleSegment.Invalid;
    TextLine byVisualYposition = lineFromVisualTop.GetTextLineByVisualYPosition(position.Y);
    int visualStartColumn = lineFromVisualTop.GetTextLineVisualStartColumn(byVisualYposition);
    int visualColumn = visualStartColumn + byVisualYposition.Length;
    int offset1 = lineFromVisualTop.FirstDocumentLine.Offset;
    int offset2 = lineFromVisualTop.GetRelativeOffset(visualStartColumn) + offset1;
    int num = lineFromVisualTop.GetRelativeOffset(visualColumn) + offset1;
    if (num == lineFromVisualTop.LastDocumentLine.Offset + lineFromVisualTop.LastDocumentLine.Length)
      num += lineFromVisualTop.LastDocumentLine.DelimiterLength;
    return new SimpleSegment(offset2, num - offset2);
  }

  private void ExtendSelection(SimpleSegment currentSeg)
  {
    if (currentSeg.Offset < this.selectionStart.Offset)
    {
      this.textArea.Caret.Offset = currentSeg.Offset;
      this.textArea.Selection = Selection.Create(this.textArea, currentSeg.Offset, this.selectionStart.Offset + this.selectionStart.Length);
    }
    else
    {
      this.textArea.Caret.Offset = currentSeg.Offset + currentSeg.Length;
      this.textArea.Selection = Selection.Create(this.textArea, this.selectionStart.Offset, currentSeg.Offset + currentSeg.Length);
    }
  }

  protected override void OnMouseMove(MouseEventArgs e)
  {
    if (this.selecting && this.textArea != null && this.TextView != null)
    {
      e.Handled = true;
      SimpleSegment textLineSegment = this.GetTextLineSegment(e);
      if (textLineSegment == SimpleSegment.Invalid)
        return;
      this.ExtendSelection(textLineSegment);
      this.textArea.Caret.BringCaretToView(5.0);
    }
    base.OnMouseMove(e);
  }

  protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
  {
    if (this.selecting)
    {
      this.selecting = false;
      this.selectionStart = (AnchorSegment) null;
      this.ReleaseMouseCapture();
      e.Handled = true;
    }
    base.OnMouseLeftButtonUp(e);
  }

  protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
  {
    return (HitTestResult) new PointHitTestResult((Visual) this, hitTestParameters.HitPoint);
  }
}
