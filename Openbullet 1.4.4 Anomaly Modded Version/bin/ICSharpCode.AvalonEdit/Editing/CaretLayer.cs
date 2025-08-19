// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.CaretLayer
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

internal sealed class CaretLayer : Layer
{
  private TextArea textArea;
  private bool isVisible;
  private Rect caretRectangle;
  private DispatcherTimer caretBlinkTimer = new DispatcherTimer();
  private bool blink;
  internal Brush CaretBrush;

  public CaretLayer(TextArea textArea)
    : base(textArea.TextView, KnownLayer.Caret)
  {
    this.textArea = textArea;
    this.IsHitTestVisible = false;
    this.caretBlinkTimer.Tick += new EventHandler(this.caretBlinkTimer_Tick);
  }

  private void caretBlinkTimer_Tick(object sender, EventArgs e)
  {
    this.blink = !this.blink;
    this.InvalidateVisual();
  }

  public void Show(Rect caretRectangle)
  {
    this.caretRectangle = caretRectangle;
    this.isVisible = true;
    this.StartBlinkAnimation();
    this.InvalidateVisual();
  }

  public void Hide()
  {
    if (!this.isVisible)
      return;
    this.isVisible = false;
    this.StopBlinkAnimation();
    this.InvalidateVisual();
  }

  private void StartBlinkAnimation()
  {
    TimeSpan caretBlinkTime = Win32.CaretBlinkTime;
    this.blink = true;
    if (caretBlinkTime.TotalMilliseconds <= 0.0)
      return;
    this.caretBlinkTimer.Interval = caretBlinkTime;
    this.caretBlinkTimer.Start();
  }

  private void StopBlinkAnimation() => this.caretBlinkTimer.Stop();

  protected override void OnRender(DrawingContext drawingContext)
  {
    base.OnRender(drawingContext);
    if (!this.isVisible || !this.blink)
      return;
    Brush brush = this.CaretBrush ?? (Brush) this.textView.GetValue(TextBlock.ForegroundProperty);
    if (this.textArea.OverstrikeMode && brush is SolidColorBrush solidColorBrush)
    {
      Color color = solidColorBrush.Color;
      brush = (Brush) new SolidColorBrush(Color.FromArgb((byte) 100, color.R, color.G, color.B));
      brush.Freeze();
    }
    Rect rect = new Rect(this.caretRectangle.X - this.textView.HorizontalOffset, this.caretRectangle.Y - this.textView.VerticalOffset, this.caretRectangle.Width, this.caretRectangle.Height);
    drawingContext.DrawRectangle(brush, (Pen) null, PixelSnapHelpers.Round(rect, PixelSnapHelpers.GetPixelSize((Visual) this)));
  }
}
