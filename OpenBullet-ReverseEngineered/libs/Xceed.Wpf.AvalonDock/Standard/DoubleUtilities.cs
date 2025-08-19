// Decompiled with JetBrains decompiler
// Type: Standard.DoubleUtilities
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

#nullable disable
namespace Standard;

internal static class DoubleUtilities
{
  private const double Epsilon = 1.53E-06;

  public static bool AreClose(double value1, double value2)
  {
    if (value1 == value2)
      return true;
    double num = value1 - value2;
    return num < 1.53E-06 && num > -1.53E-06;
  }

  public static bool LessThan(double value1, double value2)
  {
    return value1 < value2 && !DoubleUtilities.AreClose(value1, value2);
  }

  public static bool GreaterThan(double value1, double value2)
  {
    return value1 > value2 && !DoubleUtilities.AreClose(value1, value2);
  }

  public static bool LessThanOrClose(double value1, double value2)
  {
    return value1 < value2 || DoubleUtilities.AreClose(value1, value2);
  }

  public static bool GreaterThanOrClose(double value1, double value2)
  {
    return value1 > value2 || DoubleUtilities.AreClose(value1, value2);
  }

  public static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);

  public static bool IsValidSize(double value)
  {
    return DoubleUtilities.IsFinite(value) && DoubleUtilities.GreaterThanOrClose(value, 0.0);
  }
}
