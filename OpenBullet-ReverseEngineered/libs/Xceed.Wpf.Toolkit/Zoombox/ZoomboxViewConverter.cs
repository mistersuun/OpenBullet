// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Zoombox.ZoomboxViewConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Zoombox;

public sealed class ZoomboxViewConverter : TypeConverter
{
  private static ZoomboxViewConverter _converter;

  internal static ZoomboxViewConverter Converter
  {
    get
    {
      if (ZoomboxViewConverter._converter == null)
        ZoomboxViewConverter._converter = new ZoomboxViewConverter();
      return ZoomboxViewConverter._converter;
    }
  }

  public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type type)
  {
    return type == typeof (string) || type == typeof (double) || type == typeof (Point) || type == typeof (Rect) || base.CanConvertFrom(typeDescriptorContext, type);
  }

  public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type type)
  {
    return type == typeof (string) || base.CanConvertTo(typeDescriptorContext, type);
  }

  public override object ConvertFrom(
    ITypeDescriptorContext typeDescriptorContext,
    CultureInfo cultureInfo,
    object value)
  {
    ZoomboxView zoomboxView = (ZoomboxView) null;
    switch (value)
    {
      case double scale:
        zoomboxView = new ZoomboxView(scale);
        break;
      case Point position:
        zoomboxView = new ZoomboxView(position);
        break;
      case Rect region:
        zoomboxView = new ZoomboxView(region);
        break;
      case string _:
        if (string.IsNullOrEmpty((value as string).Trim()))
        {
          zoomboxView = ZoomboxView.Empty;
          break;
        }
        switch ((value as string).Trim().ToLower())
        {
          case "center":
            zoomboxView = ZoomboxView.Center;
            break;
          case "empty":
            zoomboxView = ZoomboxView.Empty;
            break;
          case "fill":
            zoomboxView = ZoomboxView.Fill;
            break;
          case "fit":
            zoomboxView = ZoomboxView.Fit;
            break;
          default:
            List<double> doubleList = new List<double>();
            string str = value as string;
            char[] separator = new char[3]{ ' ', ';', ',' };
            foreach (string s in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            {
              double result;
              if (double.TryParse(s, out result))
                doubleList.Add(result);
              if (doubleList.Count >= 4)
                break;
            }
            switch (doubleList.Count)
            {
              case 1:
                zoomboxView = new ZoomboxView(doubleList[0]);
                break;
              case 2:
                zoomboxView = new ZoomboxView(doubleList[0], doubleList[1]);
                break;
              case 3:
                zoomboxView = new ZoomboxView(doubleList[0], doubleList[1], doubleList[2]);
                break;
              case 4:
                zoomboxView = new ZoomboxView(doubleList[0], doubleList[1], doubleList[2], doubleList[3]);
                break;
            }
            break;
        }
        break;
    }
    return !(zoomboxView == (ZoomboxView) null) ? (object) zoomboxView : base.ConvertFrom(typeDescriptorContext, cultureInfo, value);
  }

  public override object ConvertTo(
    ITypeDescriptorContext typeDescriptorContext,
    CultureInfo cultureInfo,
    object value,
    Type destinationType)
  {
    object obj = (object) null;
    ZoomboxView zoomboxView = value as ZoomboxView;
    if (zoomboxView != (ZoomboxView) null && destinationType == typeof (string))
    {
      obj = (object) "Empty";
      switch (zoomboxView.ViewKind)
      {
        case ZoomboxViewKind.Absolute:
          if (PointHelper.IsEmpty(zoomboxView.Position))
          {
            if (!DoubleHelper.IsNaN(zoomboxView.Scale))
            {
              obj = (object) zoomboxView.Scale.ToString();
              break;
            }
            break;
          }
          if (DoubleHelper.IsNaN(zoomboxView.Scale))
          {
            obj = (object) $"{zoomboxView.Position.X.ToString()},{zoomboxView.Position.Y.ToString()}";
            break;
          }
          string[] strArray1 = new string[5]
          {
            zoomboxView.Scale.ToString(),
            ",",
            null,
            null,
            null
          };
          double num = zoomboxView.Position.X;
          strArray1[2] = num.ToString();
          strArray1[3] = ",";
          num = zoomboxView.Position.Y;
          strArray1[4] = num.ToString();
          obj = (object) string.Concat(strArray1);
          break;
        case ZoomboxViewKind.Fit:
          obj = (object) "Fit";
          break;
        case ZoomboxViewKind.Fill:
          obj = (object) "Fill";
          break;
        case ZoomboxViewKind.Center:
          obj = (object) "Center";
          break;
        case ZoomboxViewKind.Region:
          string[] strArray2 = new string[7];
          strArray2[0] = zoomboxView.Region.X.ToString();
          strArray2[1] = ",";
          Rect region = zoomboxView.Region;
          strArray2[2] = region.Y.ToString();
          strArray2[3] = ",";
          region = zoomboxView.Region;
          strArray2[4] = region.Width.ToString();
          strArray2[5] = ",";
          region = zoomboxView.Region;
          strArray2[6] = region.Height.ToString();
          obj = (object) string.Concat(strArray2);
          break;
      }
    }
    return obj ?? base.ConvertTo(typeDescriptorContext, cultureInfo, value, destinationType);
  }
}
