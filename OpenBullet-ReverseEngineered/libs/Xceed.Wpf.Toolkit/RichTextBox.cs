// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.RichTextBox
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class RichTextBox : System.Windows.Controls.RichTextBox
{
  private bool _preventDocumentUpdate;
  private bool _preventTextUpdate;
  public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof (Text), typeof (string), typeof (RichTextBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(RichTextBox.OnTextPropertyChanged), new CoerceValueCallback(RichTextBox.CoerceTextProperty), true, UpdateSourceTrigger.LostFocus));
  public static readonly DependencyProperty TextFormatterProperty = DependencyProperty.Register(nameof (TextFormatter), typeof (ITextFormatter), typeof (RichTextBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) new RtfFormatter(), new PropertyChangedCallback(RichTextBox.OnTextFormatterPropertyChanged)));

  public RichTextBox()
  {
  }

  public RichTextBox(FlowDocument document)
    : base(document)
  {
  }

  public string Text
  {
    get => (string) this.GetValue(RichTextBox.TextProperty);
    set => this.SetValue(RichTextBox.TextProperty, (object) value);
  }

  private static void OnTextPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    ((RichTextBox) d).UpdateDocumentFromText();
  }

  private static object CoerceTextProperty(DependencyObject d, object value)
  {
    return value ?? (object) "";
  }

  public ITextFormatter TextFormatter
  {
    get => (ITextFormatter) this.GetValue(RichTextBox.TextFormatterProperty);
    set => this.SetValue(RichTextBox.TextFormatterProperty, (object) value);
  }

  private static void OnTextFormatterPropertyChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(d is RichTextBox richTextBox))
      return;
    richTextBox.OnTextFormatterPropertyChanged((ITextFormatter) e.OldValue, (ITextFormatter) e.NewValue);
  }

  protected virtual void OnTextFormatterPropertyChanged(
    ITextFormatter oldValue,
    ITextFormatter newValue)
  {
    this.UpdateTextFromDocument();
  }

  protected override void OnTextChanged(TextChangedEventArgs e)
  {
    base.OnTextChanged(e);
    this.UpdateTextFromDocument();
  }

  private void UpdateTextFromDocument()
  {
    if (this._preventTextUpdate)
      return;
    this._preventDocumentUpdate = true;
    this.SetCurrentValue(RichTextBox.TextProperty, (object) this.TextFormatter.GetText(this.Document));
    this._preventDocumentUpdate = false;
  }

  private void UpdateDocumentFromText()
  {
    if (this._preventDocumentUpdate)
      return;
    this._preventTextUpdate = true;
    this.TextFormatter.SetText(this.Document, this.Text);
    this._preventTextUpdate = false;
  }

  public void Clear() => this.Document.Blocks.Clear();

  public override void BeginInit()
  {
    base.BeginInit();
    this._preventTextUpdate = true;
    this._preventDocumentUpdate = true;
  }

  public override void EndInit()
  {
    base.EndInit();
    this._preventTextUpdate = false;
    this._preventDocumentUpdate = false;
    if (!string.IsNullOrEmpty(this.Text))
      this.UpdateDocumentFromText();
    else
      this.UpdateTextFromDocument();
  }
}
