// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.WatermarkTextBox
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class WatermarkTextBox : AutoSelectTextBox
{
  public static readonly DependencyProperty KeepWatermarkOnGotFocusProperty = DependencyProperty.Register(nameof (KeepWatermarkOnGotFocus), typeof (bool), typeof (WatermarkTextBox), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(nameof (Watermark), typeof (object), typeof (WatermarkTextBox), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
  public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register(nameof (WatermarkTemplate), typeof (DataTemplate), typeof (WatermarkTextBox), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));

  public bool KeepWatermarkOnGotFocus
  {
    get => (bool) this.GetValue(WatermarkTextBox.KeepWatermarkOnGotFocusProperty);
    set => this.SetValue(WatermarkTextBox.KeepWatermarkOnGotFocusProperty, (object) value);
  }

  public object Watermark
  {
    get => this.GetValue(WatermarkTextBox.WatermarkProperty);
    set => this.SetValue(WatermarkTextBox.WatermarkProperty, value);
  }

  public DataTemplate WatermarkTemplate
  {
    get => (DataTemplate) this.GetValue(WatermarkTextBox.WatermarkTemplateProperty);
    set => this.SetValue(WatermarkTextBox.WatermarkTemplateProperty, (object) value);
  }

  static WatermarkTextBox()
  {
    FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (WatermarkTextBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (WatermarkTextBox)));
  }
}
