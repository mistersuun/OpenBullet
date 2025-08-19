// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.CodeCompletion.InsightWindow
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.CodeCompletion;

public partial class InsightWindow : CompletionWindowBase
{
  static InsightWindow()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (InsightWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (InsightWindow)));
    Window.AllowsTransparencyProperty.OverrideMetadata(typeof (InsightWindow), (PropertyMetadata) new FrameworkPropertyMetadata(Boxes.True));
  }

  public InsightWindow(TextArea textArea)
    : base(textArea)
  {
    this.CloseAutomatically = true;
    this.AttachEvents();
  }

  protected override void OnSourceInitialized(EventArgs e)
  {
    Rect rect = Screen.FromPoint(this.TextArea.TextView.PointToScreen(this.TextArea.Caret.CalculateCaretRectangle().Location - this.TextArea.TextView.ScrollOffset).ToSystemDrawing()).WorkingArea.ToWpf().TransformFromDevice((Visual) this);
    this.MaxHeight = rect.Height;
    this.MaxWidth = Math.Min(rect.Width, Math.Max(1000.0, rect.Width * 0.6));
    base.OnSourceInitialized(e);
  }

  public bool CloseAutomatically { get; set; }

  protected override bool CloseOnFocusLost => this.CloseAutomatically;

  private void AttachEvents()
  {
    this.TextArea.Caret.PositionChanged += new EventHandler(this.CaretPositionChanged);
  }

  protected override void DetachEvents()
  {
    this.TextArea.Caret.PositionChanged -= new EventHandler(this.CaretPositionChanged);
    base.DetachEvents();
  }

  private void CaretPositionChanged(object sender, EventArgs e)
  {
    if (!this.CloseAutomatically)
      return;
    int offset = this.TextArea.Caret.Offset;
    if (offset >= this.StartOffset && offset <= this.EndOffset)
      return;
    this.Close();
  }
}
