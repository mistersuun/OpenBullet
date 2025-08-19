// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.Primitives.DatePickerTextBox
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#nullable disable
namespace Microsoft.Windows.Controls.Primitives;

[TemplateVisualState(Name = "Watermarked", GroupName = "WatermarkStates")]
[TemplateVisualState(Name = "Unwatermarked", GroupName = "WatermarkStates")]
[TemplatePart(Name = "Watermark", Type = typeof (ContentControl))]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
[TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
public sealed class DatePickerTextBox : TextBox
{
  private const string ElementContentName = "Watermark";
  private ContentControl elementContent;
  private bool isHovered;
  internal static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(nameof (Watermark), typeof (object), typeof (DatePickerTextBox), new PropertyMetadata(new PropertyChangedCallback(DatePickerTextBox.OnWatermarkPropertyChanged)));

  static DatePickerTextBox()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (DatePickerTextBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (DatePickerTextBox)));
  }

  public DatePickerTextBox()
  {
    this.Watermark = (object) Microsoft.Windows.Controls.SR.Get(Microsoft.Windows.Controls.SRID.DatePickerTextBox_DefaultWatermarkText);
    this.Loaded += new RoutedEventHandler(this.OnLoaded);
    this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.OnDatePickerTextBoxIsEnabledChanged);
  }

  internal object Watermark
  {
    get => this.GetValue(DatePickerTextBox.WatermarkProperty);
    set => this.SetValue(DatePickerTextBox.WatermarkProperty, value);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this.elementContent = this.ExtractTemplatePart<ContentControl>("Watermark");
    this.OnWatermarkChanged();
    this.ChangeVisualState(false);
  }

  protected override void OnGotFocus(RoutedEventArgs e)
  {
    base.OnGotFocus(e);
    if (!this.IsEnabled)
      return;
    if (!string.IsNullOrEmpty(this.Text))
      this.Select(0, this.Text.Length);
    this.ChangeVisualState(true);
  }

  protected override void OnLostFocus(RoutedEventArgs e)
  {
    base.OnLostFocus(e);
    this.ChangeVisualState(true);
  }

  protected override void OnMouseEnter(MouseEventArgs e)
  {
    base.OnMouseEnter(e);
    this.isHovered = true;
    if (this.IsFocused)
      return;
    this.ChangeVisualState(true);
  }

  protected override void OnMouseLeave(MouseEventArgs e)
  {
    base.OnMouseLeave(e);
    this.isHovered = false;
    if (this.IsFocused)
      return;
    this.ChangeVisualState(true);
  }

  protected override void OnTextChanged(TextChangedEventArgs e)
  {
    base.OnTextChanged(e);
    this.ChangeVisualState(true);
  }

  private void OnLoaded(object sender, RoutedEventArgs e)
  {
    this.ApplyTemplate();
    this.ChangeVisualState(false);
  }

  private new void ChangeVisualState(bool useTransitions)
  {
    if (!this.IsEnabled)
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Disabled", "Normal");
    else if (this.isHovered)
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "MouseOver", "Normal");
    else
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Normal");
    if (this.IsFocused && this.IsEnabled)
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Focused", "Unfocused");
    else
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Unfocused");
    if (this.Watermark != null && string.IsNullOrEmpty(this.Text))
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Watermarked", "Unwatermarked");
    else
      Microsoft.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Unwatermarked");
  }

  private T ExtractTemplatePart<T>(string partName) where T : DependencyObject
  {
    DependencyObject templateChild = this.GetTemplateChild(partName);
    return DatePickerTextBox.ExtractTemplatePart<T>(partName, templateChild);
  }

  private static T ExtractTemplatePart<T>(string partName, DependencyObject obj) where T : DependencyObject
  {
    return obj as T;
  }

  private void OnDatePickerTextBoxIsEnabledChanged(
    object sender,
    DependencyPropertyChangedEventArgs e)
  {
    this.IsReadOnly = !(bool) e.NewValue;
    this.ChangeVisualState(true);
  }

  private void OnWatermarkChanged()
  {
    if (this.elementContent == null || !(this.Watermark is Control watermark))
      return;
    watermark.IsTabStop = false;
    watermark.IsHitTestVisible = false;
  }

  private static void OnWatermarkPropertyChanged(
    DependencyObject sender,
    DependencyPropertyChangedEventArgs args)
  {
    DatePickerTextBox datePickerTextBox = sender as DatePickerTextBox;
    datePickerTextBox.OnWatermarkChanged();
    datePickerTextBox.ChangeVisualState(true);
  }
}
