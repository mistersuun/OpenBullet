// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.WatermarkComboBox
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class WatermarkComboBox : ComboBox
{
  public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(nameof (Watermark), typeof (object), typeof (WatermarkComboBox), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register(nameof (WatermarkTemplate), typeof (DataTemplate), typeof (WatermarkComboBox), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));

  public object Watermark
  {
    get => this.GetValue(WatermarkComboBox.WatermarkProperty);
    set => this.SetValue(WatermarkComboBox.WatermarkProperty, value);
  }

  public DataTemplate WatermarkTemplate
  {
    get => (DataTemplate) this.GetValue(WatermarkComboBox.WatermarkTemplateProperty);
    set => this.SetValue(WatermarkComboBox.WatermarkTemplateProperty, (object) value);
  }

  static WatermarkComboBox()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (WatermarkComboBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (WatermarkComboBox)));
  }
}
