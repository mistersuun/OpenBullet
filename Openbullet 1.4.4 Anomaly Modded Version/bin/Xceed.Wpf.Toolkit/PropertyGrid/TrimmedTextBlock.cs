// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.PropertyGrid.TrimmedTextBlock
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit.PropertyGrid;

public class TrimmedTextBlock : TextBlock
{
  public static readonly DependencyProperty IsTextTrimmedProperty = DependencyProperty.Register(nameof (IsTextTrimmed), typeof (bool), typeof (TrimmedTextBlock), new PropertyMetadata((object) false, new PropertyChangedCallback(TrimmedTextBlock.OnIsTextTrimmedChanged)));
  public static readonly DependencyProperty HighlightedBrushProperty = DependencyProperty.Register(nameof (HighlightedBrush), typeof (Brush), typeof (TrimmedTextBlock), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.Yellow));
  public static readonly DependencyProperty HighlightedTextProperty = DependencyProperty.Register(nameof (HighlightedText), typeof (string), typeof (TrimmedTextBlock), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(TrimmedTextBlock.HighlightedTextChanged)));

  public TrimmedTextBlock()
  {
    this.SizeChanged += new SizeChangedEventHandler(this.TrimmedTextBlock_SizeChanged);
  }

  public bool IsTextTrimmed
  {
    get => (bool) this.GetValue(TrimmedTextBlock.IsTextTrimmedProperty);
    private set => this.SetValue(TrimmedTextBlock.IsTextTrimmedProperty, (object) value);
  }

  private static void OnIsTextTrimmedChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(d is TrimmedTextBlock trimmedTextBlock))
      return;
    trimmedTextBlock.OnIsTextTrimmedChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  private void OnIsTextTrimmedChanged(bool oldValue, bool newValue)
  {
    this.ToolTip = newValue ? (object) this.Text : (object) (string) null;
  }

  public Brush HighlightedBrush
  {
    get => (Brush) this.GetValue(TrimmedTextBlock.HighlightedBrushProperty);
    set => this.SetValue(TrimmedTextBlock.HighlightedBrushProperty, (object) value);
  }

  public string HighlightedText
  {
    get => (string) this.GetValue(TrimmedTextBlock.HighlightedTextProperty);
    set => this.SetValue(TrimmedTextBlock.HighlightedTextProperty, (object) value);
  }

  private static void HighlightedTextChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(sender is TrimmedTextBlock trimmedTextBlock))
      return;
    trimmedTextBlock.HighlightedTextChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void HighlightedTextChanged(string oldValue, string newValue)
  {
    if (this.Text.Length == 0)
      return;
    if (newValue == null)
    {
      Run run = new Run(this.Text);
      this.Inlines.Clear();
      this.Inlines.Add((Inline) run);
    }
    else
    {
      int num = this.Text.IndexOf(newValue, StringComparison.InvariantCultureIgnoreCase);
      int startIndex = num + newValue.Length;
      string text1 = this.Text.Substring(0, num);
      string text2 = this.Text.Substring(num, newValue.Length);
      string text3 = this.Text.Substring(startIndex, this.Text.Length - startIndex);
      this.Inlines.Clear();
      this.Inlines.Add((Inline) new Run(text1));
      Run run = new Run(text2);
      run.Background = this.HighlightedBrush;
      this.Inlines.Add((Inline) run);
      this.Inlines.Add((Inline) new Run(text3));
    }
  }

  private void TrimmedTextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
  {
    if (!(sender is TextBlock textBlock))
      return;
    this.IsTextTrimmed = this.GetIsTextTrimmed(textBlock);
  }

  private bool GetIsTextTrimmed(TextBlock textBlock)
  {
    if (textBlock == null || textBlock.TextTrimming == TextTrimming.None || textBlock.TextWrapping != TextWrapping.NoWrap)
      return false;
    double actualWidth = textBlock.ActualWidth;
    textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
    double width = textBlock.DesiredSize.Width;
    return actualWidth < width;
  }
}
