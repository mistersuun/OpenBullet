// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.ColorSpectrumSlider
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit;

[TemplatePart(Name = "PART_SpectrumDisplay", Type = typeof (Rectangle))]
public class ColorSpectrumSlider : Slider
{
  private const string PART_SpectrumDisplay = "PART_SpectrumDisplay";
  private Rectangle _spectrumDisplay;
  private LinearGradientBrush _pickerBrush;
  public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof (SelectedColor), typeof (Color), typeof (ColorSpectrumSlider), new PropertyMetadata((object) Colors.Transparent));

  static ColorSpectrumSlider()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (ColorSpectrumSlider), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (ColorSpectrumSlider)));
  }

  public Color SelectedColor
  {
    get => (Color) this.GetValue(ColorSpectrumSlider.SelectedColorProperty);
    set => this.SetValue(ColorSpectrumSlider.SelectedColorProperty, (object) value);
  }

  public override void OnApplyTemplate()
  {
    base.OnApplyTemplate();
    this._spectrumDisplay = (Rectangle) this.GetTemplateChild("PART_SpectrumDisplay");
    this.CreateSpectrum();
    this.OnValueChanged(double.NaN, this.Value);
  }

  protected override void OnValueChanged(double oldValue, double newValue)
  {
    base.OnValueChanged(oldValue, newValue);
    this.SelectedColor = ColorUtilities.ConvertHsvToRgb(360.0 - newValue, 1.0, 1.0);
  }

  private void CreateSpectrum()
  {
    this._pickerBrush = new LinearGradientBrush();
    this._pickerBrush.StartPoint = new Point(0.5, 0.0);
    this._pickerBrush.EndPoint = new Point(0.5, 1.0);
    this._pickerBrush.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;
    List<Color> hsvSpectrum = ColorUtilities.GenerateHsvSpectrum();
    double num = 1.0 / (double) (hsvSpectrum.Count - 1);
    int index;
    for (index = 0; index < hsvSpectrum.Count; ++index)
      this._pickerBrush.GradientStops.Add(new GradientStop(hsvSpectrum[index], (double) index * num));
    this._pickerBrush.GradientStops[index - 1].Offset = 1.0;
    if (this._spectrumDisplay == null)
      return;
    this._spectrumDisplay.Fill = (Brush) this._pickerBrush;
  }
}
