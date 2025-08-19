// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Core.Utilities.ColorUtilities
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Primitives;

#nullable disable
namespace Xceed.Wpf.Toolkit.Core.Utilities;

internal static class ColorUtilities
{
  public static readonly Dictionary<string, Color> KnownColors = ColorUtilities.GetKnownColors();

  public static string GetColorName(this Color color)
  {
    string colorName = ColorUtilities.KnownColors.Where<KeyValuePair<string, Color>>((Func<KeyValuePair<string, Color>, bool>) (kvp => kvp.Value.Equals(color))).Select<KeyValuePair<string, Color>, string>((Func<KeyValuePair<string, Color>, string>) (kvp => kvp.Key)).FirstOrDefault<string>();
    if (string.IsNullOrEmpty(colorName))
      colorName = color.ToString();
    return colorName;
  }

  public static string FormatColorString(string stringToFormat, bool isUsingAlphaChannel)
  {
    return !isUsingAlphaChannel && stringToFormat.Length == 9 ? stringToFormat.Remove(1, 2) : stringToFormat;
  }

  private static Dictionary<string, Color> GetKnownColors()
  {
    return ((IEnumerable<PropertyInfo>) typeof (Colors).GetProperties(BindingFlags.Static | BindingFlags.Public)).ToDictionary<PropertyInfo, string, Color>((Func<PropertyInfo, string>) (p => p.Name), (Func<PropertyInfo, Color>) (p => (Color) p.GetValue((object) null, (object[]) null)));
  }

  public static HsvColor ConvertRgbToHsv(int r, int g, int b)
  {
    double num1 = 0.0;
    double num2 = (double) Math.Min(Math.Min(r, g), b);
    double num3 = (double) Math.Max(Math.Max(r, g), b);
    double num4 = num3 - num2;
    double num5 = num3 != 0.0 ? num4 / num3 : 0.0;
    double num6;
    if (num5 == 0.0)
    {
      num6 = 0.0;
    }
    else
    {
      if ((double) r == num3)
        num1 = (double) (g - b) / num4;
      else if ((double) g == num3)
        num1 = 2.0 + (double) (b - r) / num4;
      else if ((double) b == num3)
        num1 = 4.0 + (double) (r - g) / num4;
      num6 = num1 * 60.0;
      if (num6 < 0.0)
        num6 += 360.0;
    }
    return new HsvColor()
    {
      H = num6,
      S = num5,
      V = num3 / (double) byte.MaxValue
    };
  }

  public static Color ConvertHsvToRgb(double h, double s, double v)
  {
    double num1;
    double num2;
    double num3;
    if (s == 0.0)
    {
      num1 = v;
      num2 = v;
      num3 = v;
    }
    else
    {
      if (h == 360.0)
        h = 0.0;
      else
        h /= 60.0;
      int num4 = (int) Math.Truncate(h);
      double num5 = h - (double) num4;
      double num6 = v * (1.0 - s);
      double num7 = v * (1.0 - s * num5);
      double num8 = v * (1.0 - s * (1.0 - num5));
      switch (num4)
      {
        case 0:
          num1 = v;
          num2 = num8;
          num3 = num6;
          break;
        case 1:
          num1 = num7;
          num2 = v;
          num3 = num6;
          break;
        case 2:
          num1 = num6;
          num2 = v;
          num3 = num8;
          break;
        case 3:
          num1 = num6;
          num2 = num7;
          num3 = v;
          break;
        case 4:
          num1 = num8;
          num2 = num6;
          num3 = v;
          break;
        default:
          num1 = v;
          num2 = num6;
          num3 = num7;
          break;
      }
    }
    return Color.FromArgb(byte.MaxValue, (byte) Math.Round(num1 * (double) byte.MaxValue), (byte) Math.Round(num2 * (double) byte.MaxValue), (byte) Math.Round(num3 * (double) byte.MaxValue));
  }

  public static List<Color> GenerateHsvSpectrum()
  {
    List<Color> hsvSpectrum = new List<Color>();
    int num = 60;
    for (int h = 0; h < 360; h += num)
      hsvSpectrum.Add(ColorUtilities.ConvertHsvToRgb((double) h, 1.0, 1.0));
    hsvSpectrum.Add(ColorUtilities.ConvertHsvToRgb(0.0, 1.0, 1.0));
    return hsvSpectrum;
  }
}
