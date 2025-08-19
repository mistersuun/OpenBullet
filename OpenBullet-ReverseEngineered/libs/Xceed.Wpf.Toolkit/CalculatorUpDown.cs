// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.CalculatorUpDown
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_CalculatorPopup", Type = typeof (Popup))]
[TemplatePart(Name = "PART_Calculator", Type = typeof (Calculator))]
public class CalculatorUpDown : DecimalUpDown
{
  private const string PART_CalculatorPopup = "PART_CalculatorPopup";
  private const string PART_Calculator = "PART_Calculator";
  private Popup _calculatorPopup;
  private Calculator _calculator;
  private Decimal? _initialValue;
  public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(nameof (DisplayText), typeof (string), typeof (CalculatorUpDown), (PropertyMetadata) new UIPropertyMetadata((object) "0"));
  public static readonly DependencyProperty EnterClosesCalculatorProperty = DependencyProperty.Register(nameof (EnterClosesCalculator), typeof (bool), typeof (CalculatorUpDown), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof (IsOpen), typeof (bool), typeof (CalculatorUpDown), (PropertyMetadata) new UIPropertyMetadata((object) false, new PropertyChangedCallback(CalculatorUpDown.OnIsOpenChanged)));
  public static readonly DependencyProperty MemoryProperty = DependencyProperty.Register(nameof (Memory), typeof (Decimal), typeof (CalculatorUpDown), (PropertyMetadata) new UIPropertyMetadata((object) 0M));
  public static readonly DependencyProperty PrecisionProperty = DependencyProperty.Register(nameof (Precision), typeof (int), typeof (CalculatorUpDown), (PropertyMetadata) new UIPropertyMetadata((object) 6));

  public string DisplayText
  {
    get => (string) this.GetValue(CalculatorUpDown.DisplayTextProperty);
    set => this.SetValue(CalculatorUpDown.DisplayTextProperty, (object) value);
  }

  public bool EnterClosesCalculator
  {
    get => (bool) this.GetValue(CalculatorUpDown.EnterClosesCalculatorProperty);
    set => this.SetValue(CalculatorUpDown.EnterClosesCalculatorProperty, (object) value);
  }

  public bool IsOpen
  {
    get => (bool) this.GetValue(CalculatorUpDown.IsOpenProperty);
    set => this.SetValue(CalculatorUpDown.IsOpenProperty, (object) value);
  }

  private static void OnIsOpenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is CalculatorUpDown calculatorUpDown))
      return;
    calculatorUpDown.OnIsOpenChanged((bool) e.OldValue, (bool) e.NewValue);
  }

  protected virtual void OnIsOpenChanged(bool oldValue, bool newValue)
  {
    if (!newValue)
      return;
    this._initialValue = this.UpdateValueOnEnterKey ? this.ConvertTextToValue(this.TextBox.Text) : this.Value;
  }

  public Decimal Memory
  {
    get => (Decimal) this.GetValue(CalculatorUpDown.MemoryProperty);
    set => this.SetValue(CalculatorUpDown.MemoryProperty, (object) value);
  }

  public int Precision
  {
    get => (int) this.GetValue(CalculatorUpDown.PrecisionProperty);
    set => this.SetValue(CalculatorUpDown.PrecisionProperty, (object) value);
  }

  static CalculatorUpDown()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (CalculatorUpDown), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (CalculatorUpDown)));
  }

  public CalculatorUpDown()
  {
    Keyboard.AddKeyDownHandler((DependencyObject) this, new KeyEventHandler(this.OnKeyDown));
    Mouse.AddPreviewMouseDownOutsideCapturedElementHandler((DependencyObject) this, new MouseButtonEventHandler(this.OnMouseDownOutsideCapturedElement));
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (this._calculatorPopup != null)
      this._calculatorPopup.Opened -= new EventHandler(this.CalculatorPopup_Opened);
    this._calculatorPopup = this.GetTemplateChild("PART_CalculatorPopup") as Popup;
    if (this._calculatorPopup != null)
      this._calculatorPopup.Opened += new EventHandler(this.CalculatorPopup_Opened);
    if (this._calculator != null)
      this._calculator.ValueChanged -= new RoutedPropertyChangedEventHandler<object>(this.OnCalculatorValueChanged);
    this._calculator = this.GetTemplateChild("PART_Calculator") as Calculator;
    if (this._calculator == null)
      return;
    this._calculator.ValueChanged += new RoutedPropertyChangedEventHandler<object>(this.OnCalculatorValueChanged);
  }

  private void OnCalculatorValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
  {
    if (this._calculator == null || !this.IsBetweenMinMax(this._calculator.Value))
      return;
    if (this.UpdateValueOnEnterKey)
      this.TextBox.Text = this._calculator.Value.HasValue ? this._calculator.Value.Value.ToString(this.FormatString, (IFormatProvider) this.CultureInfo) : (string) null;
    else
      this.Value = this._calculator.Value;
  }

  private void CalculatorPopup_Opened(object sender, EventArgs e)
  {
    if (this._calculator == null)
      return;
    this._calculator.InitializeToValue(this.UpdateValueOnEnterKey ? this.ConvertTextToValue(this.TextBox.Text) : this.Value);
    this._calculator.Focus();
  }

  protected override void OnTextInput(TextCompositionEventArgs e)
  {
    if (!this.IsOpen || !this.EnterClosesCalculator || CalculatorUtilities.GetCalculatorButtonTypeFromText(e.Text) != Calculator.CalculatorButtonType.Equal)
      return;
    this.CloseCalculatorUpDown(true);
  }

  private void OnKeyDown(object sender, KeyEventArgs e)
  {
    if (!this.IsOpen)
    {
      if (!KeyboardUtilities.IsKeyModifyingPopupState(e))
        return;
      this.IsOpen = true;
      e.Handled = true;
    }
    else if (KeyboardUtilities.IsKeyModifyingPopupState(e))
    {
      this.CloseCalculatorUpDown(true);
      e.Handled = true;
    }
    else
    {
      if (e.Key != Key.Escape)
        return;
      if (this.EnterClosesCalculator)
      {
        if (this.UpdateValueOnEnterKey)
          this.TextBox.Text = this._initialValue.HasValue ? this._initialValue.Value.ToString(this.FormatString, (IFormatProvider) this.CultureInfo) : (string) null;
        else
          this.Value = this._initialValue;
      }
      this.CloseCalculatorUpDown(true);
      e.Handled = true;
    }
  }

  private void OnMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
  {
    this.CloseCalculatorUpDown(true);
  }

  private void CloseCalculatorUpDown(bool isFocusOnTextBox)
  {
    if (this.IsOpen)
      this.IsOpen = false;
    this.ReleaseMouseCapture();
    if (!isFocusOnTextBox || this.TextBox == null)
      return;
    this.TextBox.Focus();
  }
}
