// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.ColorCanvas
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_ColorShadingCanvas", Type = typeof (Canvas))]
[TemplatePart(Name = "PART_ColorShadeSelector", Type = typeof (Canvas))]
[TemplatePart(Name = "PART_SpectrumSlider", Type = typeof (ColorSpectrumSlider))]
[TemplatePart(Name = "PART_HexadecimalTextBox", Type = typeof (TextBox))]
public class ColorCanvas : Control
{
  private const string PART_ColorShadingCanvas = "PART_ColorShadingCanvas";
  private const string PART_ColorShadeSelector = "PART_ColorShadeSelector";
  private const string PART_SpectrumSlider = "PART_SpectrumSlider";
  private const string PART_HexadecimalTextBox = "PART_HexadecimalTextBox";
  private TranslateTransform _colorShadeSelectorTransform = new TranslateTransform();
  private Canvas _colorShadingCanvas;
  private Canvas _colorShadeSelector;
  private ColorSpectrumSlider _spectrumSlider;
  private TextBox _hexadecimalTextBox;
  private Point? _currentColorPosition;
  private bool _surpressPropertyChanged;
  private bool _updateSpectrumSliderValue = true;
  public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof (SelectedColor), typeof (Color?), typeof (ColorCanvas), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ColorCanvas.OnSelectedColorChanged)));
  public static readonly DependencyProperty AProperty = DependencyProperty.Register(nameof (A), typeof (byte), typeof (ColorCanvas), (PropertyMetadata) new UIPropertyMetadata((object) byte.MaxValue, new PropertyChangedCallback(ColorCanvas.OnAChanged)));
  public static readonly DependencyProperty RProperty = DependencyProperty.Register(nameof (R), typeof (byte), typeof (ColorCanvas), (PropertyMetadata) new UIPropertyMetadata((object) (byte) 0, new PropertyChangedCallback(ColorCanvas.OnRChanged)));
  public static readonly DependencyProperty GProperty = DependencyProperty.Register(nameof (G), typeof (byte), typeof (ColorCanvas), (PropertyMetadata) new UIPropertyMetadata((object) (byte) 0, new PropertyChangedCallback(ColorCanvas.OnGChanged)));
  public static readonly DependencyProperty BProperty = DependencyProperty.Register(nameof (B), typeof (byte), typeof (ColorCanvas), (PropertyMetadata) new UIPropertyMetadata((object) (byte) 0, new PropertyChangedCallback(ColorCanvas.OnBChanged)));
  public static readonly DependencyProperty HexadecimalStringProperty = DependencyProperty.Register(nameof (HexadecimalString), typeof (string), typeof (ColorCanvas), (PropertyMetadata) new UIPropertyMetadata((object) "", new PropertyChangedCallback(ColorCanvas.OnHexadecimalStringChanged), new CoerceValueCallback(ColorCanvas.OnCoerceHexadecimalString)));
  public static readonly DependencyProperty UsingAlphaChannelProperty = DependencyProperty.Register(nameof (UsingAlphaChannel), typeof (bool), typeof (ColorCanvas), (PropertyMetadata) new FrameworkPropertyMetadata((object) true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ColorCanvas.OnUsingAlphaChannelPropertyChanged)));
  public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent("SelectedColorChanged", RoutingStrategy.Bubble, typeof (RoutedPropertyChangedEventHandler<Color?>), typeof (ColorCanvas));

  public Color? SelectedColor
  {
    get => (Color?) this.GetValue(ColorCanvas.SelectedColorProperty);
    set => this.SetValue(ColorCanvas.SelectedColorProperty, (object) value);
  }

  private static void OnSelectedColorChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ColorCanvas colorCanvas))
      return;
    colorCanvas.OnSelectedColorChanged((Color?) e.OldValue, (Color?) e.NewValue);
  }

  protected virtual void OnSelectedColorChanged(Color? oldValue, Color? newValue)
  {
    this.SetHexadecimalStringProperty(this.GetFormatedColorString(newValue), false);
    this.UpdateRGBValues(newValue);
    this.UpdateColorShadeSelectorPosition(newValue);
    RoutedPropertyChangedEventArgs<Color?> e = new RoutedPropertyChangedEventArgs<Color?>(oldValue, newValue);
    e.RoutedEvent = ColorCanvas.SelectedColorChangedEvent;
    this.RaiseEvent((RoutedEventArgs) e);
  }

  public byte A
  {
    get => (byte) this.GetValue(ColorCanvas.AProperty);
    set => this.SetValue(ColorCanvas.AProperty, (object) value);
  }

  private static void OnAChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ColorCanvas colorCanvas))
      return;
    colorCanvas.OnAChanged((byte) e.OldValue, (byte) e.NewValue);
  }

  protected virtual void OnAChanged(byte oldValue, byte newValue)
  {
    if (this._surpressPropertyChanged)
      return;
    this.UpdateSelectedColor();
  }

  public byte R
  {
    get => (byte) this.GetValue(ColorCanvas.RProperty);
    set => this.SetValue(ColorCanvas.RProperty, (object) value);
  }

  private static void OnRChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ColorCanvas colorCanvas))
      return;
    colorCanvas.OnRChanged((byte) e.OldValue, (byte) e.NewValue);
  }

  protected virtual void OnRChanged(byte oldValue, byte newValue)
  {
    if (this._surpressPropertyChanged)
      return;
    this.UpdateSelectedColor();
  }

  public byte G
  {
    get => (byte) this.GetValue(ColorCanvas.GProperty);
    set => this.SetValue(ColorCanvas.GProperty, (object) value);
  }

  private static void OnGChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ColorCanvas colorCanvas))
      return;
    colorCanvas.OnGChanged((byte) e.OldValue, (byte) e.NewValue);
  }

  protected virtual void OnGChanged(byte oldValue, byte newValue)
  {
    if (this._surpressPropertyChanged)
      return;
    this.UpdateSelectedColor();
  }

  public byte B
  {
    get => (byte) this.GetValue(ColorCanvas.BProperty);
    set => this.SetValue(ColorCanvas.BProperty, (object) value);
  }

  private static void OnBChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ColorCanvas colorCanvas))
      return;
    colorCanvas.OnBChanged((byte) e.OldValue, (byte) e.NewValue);
  }

  protected virtual void OnBChanged(byte oldValue, byte newValue)
  {
    if (this._surpressPropertyChanged)
      return;
    this.UpdateSelectedColor();
  }

  public string HexadecimalString
  {
    get => (string) this.GetValue(ColorCanvas.HexadecimalStringProperty);
    set => this.SetValue(ColorCanvas.HexadecimalStringProperty, (object) value);
  }

  private static void OnHexadecimalStringChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ColorCanvas colorCanvas))
      return;
    colorCanvas.OnHexadecimalStringChanged((string) e.OldValue, (string) e.NewValue);
  }

  protected virtual void OnHexadecimalStringChanged(string oldValue, string newValue)
  {
    string formatedColorString = this.GetFormatedColorString(newValue);
    if (!this.GetFormatedColorString(this.SelectedColor).Equals(formatedColorString))
    {
      Color? color = new Color?();
      if (!string.IsNullOrEmpty(formatedColorString))
        color = new Color?((Color) ColorConverter.ConvertFromString(formatedColorString));
      this.UpdateSelectedColor(color);
    }
    this.SetHexadecimalTextBoxTextProperty(newValue);
  }

  private static object OnCoerceHexadecimalString(DependencyObject d, object basevalue)
  {
    ColorCanvas colorCanvas = (ColorCanvas) d;
    return colorCanvas == null ? basevalue : colorCanvas.OnCoerceHexadecimalString(basevalue);
  }

  private object OnCoerceHexadecimalString(object newValue)
  {
    string s = newValue as string;
    try
    {
      if (!string.IsNullOrEmpty(s))
      {
        if (int.TryParse(s, NumberStyles.HexNumber, (IFormatProvider) null, out int _))
          s = "#" + s;
        ColorConverter.ConvertFromString(s);
      }
    }
    catch
    {
      throw new InvalidDataException("Color provided is not in the correct format.");
    }
    return (object) s;
  }

  public bool UsingAlphaChannel
  {
    get => (bool) this.GetValue(ColorCanvas.UsingAlphaChannelProperty);
    set => this.SetValue(ColorCanvas.UsingAlphaChannelProperty, (object) value);
  }

  private static void OnUsingAlphaChannelPropertyChanged(
    DependencyObject o,
    DependencyPropertyChangedEventArgs e)
  {
    if (!(o is ColorCanvas colorCanvas))
      return;
    colorCanvas.OnUsingAlphaChannelChanged();
  }

  protected virtual void OnUsingAlphaChannelChanged()
  {
    this.SetHexadecimalStringProperty(this.GetFormatedColorString(this.SelectedColor), false);
  }

  static ColorCanvas()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (ColorCanvas), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (ColorCanvas)));
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    if (this._colorShadingCanvas != null)
    {
      this._colorShadingCanvas.MouseLeftButtonDown -= new MouseButtonEventHandler(this.ColorShadingCanvas_MouseLeftButtonDown);
      this._colorShadingCanvas.MouseLeftButtonUp -= new MouseButtonEventHandler(this.ColorShadingCanvas_MouseLeftButtonUp);
      this._colorShadingCanvas.MouseMove -= new MouseEventHandler(this.ColorShadingCanvas_MouseMove);
      this._colorShadingCanvas.SizeChanged -= new SizeChangedEventHandler(this.ColorShadingCanvas_SizeChanged);
    }
    this._colorShadingCanvas = this.GetTemplateChild("PART_ColorShadingCanvas") as Canvas;
    if (this._colorShadingCanvas != null)
    {
      this._colorShadingCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(this.ColorShadingCanvas_MouseLeftButtonDown);
      this._colorShadingCanvas.MouseLeftButtonUp += new MouseButtonEventHandler(this.ColorShadingCanvas_MouseLeftButtonUp);
      this._colorShadingCanvas.MouseMove += new MouseEventHandler(this.ColorShadingCanvas_MouseMove);
      this._colorShadingCanvas.SizeChanged += new SizeChangedEventHandler(this.ColorShadingCanvas_SizeChanged);
    }
    this._colorShadeSelector = this.GetTemplateChild("PART_ColorShadeSelector") as Canvas;
    if (this._colorShadeSelector != null)
      this._colorShadeSelector.RenderTransform = (Transform) this._colorShadeSelectorTransform;
    if (this._spectrumSlider != null)
      this._spectrumSlider.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(this.SpectrumSlider_ValueChanged);
    this._spectrumSlider = this.GetTemplateChild("PART_SpectrumSlider") as ColorSpectrumSlider;
    if (this._spectrumSlider != null)
      this._spectrumSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.SpectrumSlider_ValueChanged);
    if (this._hexadecimalTextBox != null)
      this._hexadecimalTextBox.LostFocus -= new RoutedEventHandler(this.HexadecimalTextBox_LostFocus);
    this._hexadecimalTextBox = this.GetTemplateChild("PART_HexadecimalTextBox") as TextBox;
    if (this._hexadecimalTextBox != null)
      this._hexadecimalTextBox.LostFocus += new RoutedEventHandler(this.HexadecimalTextBox_LostFocus);
    this.UpdateRGBValues(this.SelectedColor);
    this.UpdateColorShadeSelectorPosition(this.SelectedColor);
    this.SetHexadecimalTextBoxTextProperty(this.GetFormatedColorString(this.SelectedColor));
  }

  protected override void OnKeyDown(KeyEventArgs e)
  {
    base.OnKeyDown(e);
    if (e.Key != Key.Return || !(e.OriginalSource is TextBox))
      return;
    TextBox originalSource = (TextBox) e.OriginalSource;
    if (!(originalSource.Name == "PART_HexadecimalTextBox"))
      return;
    this.SetHexadecimalStringProperty(originalSource.Text, true);
  }

  private void ColorShadingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    if (this._colorShadingCanvas == null)
      return;
    this.UpdateColorShadeSelectorPositionAndCalculateColor(e.GetPosition((IInputElement) this._colorShadingCanvas), true);
    this._colorShadingCanvas.CaptureMouse();
    e.Handled = true;
  }

  private void ColorShadingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
  {
    if (this._colorShadingCanvas == null)
      return;
    this._colorShadingCanvas.ReleaseMouseCapture();
  }

  private void ColorShadingCanvas_MouseMove(object sender, MouseEventArgs e)
  {
    if (this._colorShadingCanvas == null || e.LeftButton != MouseButtonState.Pressed)
      return;
    this.UpdateColorShadeSelectorPositionAndCalculateColor(e.GetPosition((IInputElement) this._colorShadingCanvas), true);
    Mouse.Synchronize();
  }

  private void ColorShadingCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
  {
    if (!this._currentColorPosition.HasValue)
      return;
    this.UpdateColorShadeSelectorPositionAndCalculateColor(new Point()
    {
      X = this._currentColorPosition.Value.X * e.NewSize.Width,
      Y = this._currentColorPosition.Value.Y * e.NewSize.Height
    }, false);
  }

  private void SpectrumSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
  {
    if (!this._currentColorPosition.HasValue || !this.SelectedColor.HasValue)
      return;
    this.CalculateColor(this._currentColorPosition.Value);
  }

  private void HexadecimalTextBox_LostFocus(object sender, RoutedEventArgs e)
  {
    this.SetHexadecimalStringProperty((sender as TextBox).Text, true);
  }

  public event RoutedPropertyChangedEventHandler<Color?> SelectedColorChanged
  {
    add => this.AddHandler(ColorCanvas.SelectedColorChangedEvent, (Delegate) value);
    remove => this.RemoveHandler(ColorCanvas.SelectedColorChangedEvent, (Delegate) value);
  }

  private void UpdateSelectedColor()
  {
    this.SelectedColor = new Color?(Color.FromArgb(this.A, this.R, this.G, this.B));
  }

  private void UpdateSelectedColor(Color? color)
  {
    Color? nullable;
    if (!color.HasValue || !color.HasValue)
    {
      nullable = new Color?();
    }
    else
    {
      Color color1 = color.Value;
      int a = (int) color1.A;
      color1 = color.Value;
      int r = (int) color1.R;
      color1 = color.Value;
      int g = (int) color1.G;
      color1 = color.Value;
      int b = (int) color1.B;
      nullable = new Color?(Color.FromArgb((byte) a, (byte) r, (byte) g, (byte) b));
    }
    this.SelectedColor = nullable;
  }

  private void UpdateRGBValues(Color? color)
  {
    if (!color.HasValue || !color.HasValue)
      return;
    this._surpressPropertyChanged = true;
    this.A = color.Value.A;
    this.R = color.Value.R;
    this.G = color.Value.G;
    this.B = color.Value.B;
    this._surpressPropertyChanged = false;
  }

  private void UpdateColorShadeSelectorPositionAndCalculateColor(Point p, bool calculateColor)
  {
    if (this._colorShadingCanvas == null || this._colorShadeSelector == null)
      return;
    if (p.Y < 0.0)
      p.Y = 0.0;
    if (p.X < 0.0)
      p.X = 0.0;
    if (p.X > this._colorShadingCanvas.ActualWidth)
      p.X = this._colorShadingCanvas.ActualWidth;
    if (p.Y > this._colorShadingCanvas.ActualHeight)
      p.Y = this._colorShadingCanvas.ActualHeight;
    this._colorShadeSelectorTransform.X = p.X - this._colorShadeSelector.Width / 2.0;
    this._colorShadeSelectorTransform.Y = p.Y - this._colorShadeSelector.Height / 2.0;
    p.X /= this._colorShadingCanvas.ActualWidth;
    p.Y /= this._colorShadingCanvas.ActualHeight;
    this._currentColorPosition = new Point?(p);
    if (!calculateColor)
      return;
    this.CalculateColor(p);
  }

  private void UpdateColorShadeSelectorPosition(Color? color)
  {
    if (this._spectrumSlider == null || this._colorShadingCanvas == null || !color.HasValue || !color.HasValue)
      return;
    this._currentColorPosition = new Point?();
    int r = (int) color.Value.R;
    Color color1 = color.Value;
    int g = (int) color1.G;
    color1 = color.Value;
    int b = (int) color1.B;
    HsvColor hsv = ColorUtilities.ConvertRgbToHsv(r, g, b);
    if (this._updateSpectrumSliderValue)
      this._spectrumSlider.Value = 360.0 - hsv.H;
    Point point = new Point(hsv.S, 1.0 - hsv.V);
    this._currentColorPosition = new Point?(point);
    this._colorShadeSelectorTransform.X = point.X * this._colorShadingCanvas.Width - 5.0;
    this._colorShadeSelectorTransform.Y = point.Y * this._colorShadingCanvas.Height - 5.0;
  }

  private void CalculateColor(Point p)
  {
    if (this._spectrumSlider == null)
      return;
    HsvColor hsvColor = new HsvColor(360.0 - this._spectrumSlider.Value, 1.0, 1.0)
    {
      S = p.X,
      V = 1.0 - p.Y
    };
    Color rgb = ColorUtilities.ConvertHsvToRgb(hsvColor.H, hsvColor.S, hsvColor.V) with
    {
      A = this.A
    };
    this._updateSpectrumSliderValue = false;
    this.SelectedColor = new Color?(rgb);
    this._updateSpectrumSliderValue = true;
    this.SetHexadecimalStringProperty(this.GetFormatedColorString(this.SelectedColor), false);
  }

  private string GetFormatedColorString(Color? colorToFormat)
  {
    return !colorToFormat.HasValue || !colorToFormat.HasValue ? string.Empty : ColorUtilities.FormatColorString(colorToFormat.ToString(), this.UsingAlphaChannel);
  }

  private string GetFormatedColorString(string stringToFormat)
  {
    return ColorUtilities.FormatColorString(stringToFormat, this.UsingAlphaChannel);
  }

  private void SetHexadecimalStringProperty(string newValue, bool modifyFromUI)
  {
    if (modifyFromUI)
    {
      try
      {
        if (!string.IsNullOrEmpty(newValue))
        {
          if (int.TryParse(newValue, NumberStyles.HexNumber, (IFormatProvider) null, out int _))
            newValue = "#" + newValue;
          ColorConverter.ConvertFromString(newValue);
        }
        this.HexadecimalString = newValue;
      }
      catch
      {
        this.SetHexadecimalTextBoxTextProperty(this.HexadecimalString);
      }
    }
    else
      this.HexadecimalString = newValue;
  }

  private void SetHexadecimalTextBoxTextProperty(string newValue)
  {
    if (this._hexadecimalTextBox == null)
      return;
    this._hexadecimalTextBox.Text = newValue;
  }
}
