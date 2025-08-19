// Decompiled with JetBrains decompiler
// Type: Tesseract.Internal.TessConvert
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Globalization;

#nullable disable
namespace Tesseract.Internal;

internal static class TessConvert
{
  public static bool TryToString(object value, out string result)
  {
    switch (value)
    {
      case bool flag:
        result = TessConvert.ToString(flag);
        break;
      case Decimal num1:
        result = TessConvert.ToString(num1);
        break;
      case double num2:
        result = TessConvert.ToString(num2);
        break;
      case float num3:
        result = TessConvert.ToString(num3);
        break;
      case short num4:
        result = TessConvert.ToString(num4);
        break;
      case int num5:
        result = TessConvert.ToString(num5);
        break;
      case long num6:
        result = TessConvert.ToString(num6);
        break;
      case ushort num7:
        result = TessConvert.ToString(num7);
        break;
      case uint num8:
        result = TessConvert.ToString(num8);
        break;
      case ulong num9:
        result = TessConvert.ToString(num9);
        break;
      case string _:
        result = (string) value;
        break;
      default:
        result = (string) null;
        return false;
    }
    return true;
  }

  public static string ToString(bool value) => !value ? "FALSE" : "TRUE";

  public static string ToString(Decimal value)
  {
    return value.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
  }

  public static string ToString(double value)
  {
    return value.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
  }

  public static string ToString(float value)
  {
    return value.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
  }

  public static string ToString(short value)
  {
    return value.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
  }

  public static string ToString(int value)
  {
    return value.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
  }

  public static string ToString(long value)
  {
    return value.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
  }

  public static string ToString(ushort value)
  {
    return value.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
  }

  public static string ToString(uint value)
  {
    return value.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
  }

  public static string ToString(ulong value)
  {
    return value.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
  }
}
