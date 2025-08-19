// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.ColorSorter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;

#nullable disable
namespace Xceed.Wpf.Toolkit;

internal class ColorSorter : IComparer
{
  public int Compare(object firstItem, object secondItem)
  {
    if (firstItem == null || secondItem == null)
      return -1;
    ColorItem colorItem1 = (ColorItem) firstItem;
    ColorItem colorItem2 = (ColorItem) secondItem;
    if (colorItem1.Color.HasValue)
    {
      System.Windows.Media.Color? color1 = colorItem1.Color;
      if (color1.HasValue)
      {
        color1 = colorItem2.Color;
        if (color1.HasValue)
        {
          color1 = colorItem2.Color;
          if (color1.HasValue)
          {
            color1 = colorItem1.Color;
            int a1 = (int) color1.Value.A;
            color1 = colorItem1.Color;
            System.Windows.Media.Color color2 = color1.Value;
            int r1 = (int) color2.R;
            color1 = colorItem1.Color;
            color2 = color1.Value;
            int g1 = (int) color2.G;
            color1 = colorItem1.Color;
            color2 = color1.Value;
            int b1 = (int) color2.B;
            System.Drawing.Color color3 = System.Drawing.Color.FromArgb(a1, r1, g1, b1);
            color1 = colorItem2.Color;
            color2 = color1.Value;
            int a2 = (int) color2.A;
            color1 = colorItem2.Color;
            color2 = color1.Value;
            int r2 = (int) color2.R;
            color1 = colorItem2.Color;
            color2 = color1.Value;
            int g2 = (int) color2.G;
            color1 = colorItem2.Color;
            color2 = color1.Value;
            int b2 = (int) color2.B;
            System.Drawing.Color color4 = System.Drawing.Color.FromArgb(a2, r2, g2, b2);
            double num1 = Math.Round((double) color3.GetHue(), 3);
            double num2 = Math.Round((double) color4.GetHue(), 3);
            if (num1 > num2)
              return 1;
            if (num1 < num2)
              return -1;
            double num3 = Math.Round((double) color3.GetSaturation(), 3);
            double num4 = Math.Round((double) color4.GetSaturation(), 3);
            if (num3 > num4)
              return 1;
            if (num3 < num4)
              return -1;
            double num5 = Math.Round((double) color3.GetBrightness(), 3);
            double num6 = Math.Round((double) color4.GetBrightness(), 3);
            if (num5 > num6)
              return 1;
            return num5 < num6 ? -1 : 0;
          }
        }
      }
    }
    return -1;
  }
}
