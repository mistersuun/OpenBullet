// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Converters.ColorBlendConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Converters;

public class ColorBlendConverter : IValueConverter
{
  private double _blendedColorRatio;

  public double BlendedColorRatio
  {
    get => this._blendedColorRatio;
    set
    {
      this._blendedColorRatio = value >= 0.0 && value <= 1.0 ? value : throw new ArgumentException("BlendedColorRatio must be greater than or equal to 0 and lower than or equal to 1 ");
    }
  }

  public Color BlendedColor { get; set; }

  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value == null || value.GetType() != typeof (Color))
      return (object) null;
    Color color1 = (Color) value;
    Color color2 = new Color();
    ref Color local1 = ref color2;
    int a1 = (int) color1.A;
    Color blendedColor = this.BlendedColor;
    int a2 = (int) blendedColor.A;
    int num1 = (int) this.BlendValue((byte) a1, (byte) a2);
    local1.A = (byte) num1;
    ref Color local2 = ref color2;
    int r1 = (int) color1.R;
    blendedColor = this.BlendedColor;
    int r2 = (int) blendedColor.R;
    int num2 = (int) this.BlendValue((byte) r1, (byte) r2);
    local2.R = (byte) num2;
    ref Color local3 = ref color2;
    int g1 = (int) color1.G;
    blendedColor = this.BlendedColor;
    int g2 = (int) blendedColor.G;
    int num3 = (int) this.BlendValue((byte) g1, (byte) g2);
    local3.G = (byte) num3;
    ref Color local4 = ref color2;
    int b1 = (int) color1.B;
    blendedColor = this.BlendedColor;
    int b2 = (int) blendedColor.B;
    int num4 = (int) this.BlendValue((byte) b1, (byte) b2);
    local4.B = (byte) num4;
    return (object) color2;
  }

  private byte BlendValue(byte original, byte blend)
  {
    double blendedColorRatio = this.BlendedColorRatio;
    double num = 1.0 - blendedColorRatio;
    return System.Convert.ToByte(Math.Min((double) byte.MaxValue, Math.Max(0.0, Math.Round((double) original * num + (double) blend * blendedColorRatio))));
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
