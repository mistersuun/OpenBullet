// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.PixelSnapHelpers
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

public static class PixelSnapHelpers
{
  public static Size GetPixelSize(Visual visual)
  {
    PresentationSource presentationSource = visual != null ? PresentationSource.FromVisual(visual) : throw new ArgumentNullException(nameof (visual));
    if (presentationSource == null)
      return new Size(1.0, 1.0);
    Matrix transformFromDevice = presentationSource.CompositionTarget.TransformFromDevice;
    return new Size(transformFromDevice.M11, transformFromDevice.M22);
  }

  public static double PixelAlign(double value, double pixelSize)
  {
    return pixelSize * (Math.Round(value / pixelSize + 0.5, MidpointRounding.AwayFromZero) - 0.5);
  }

  public static Rect PixelAlign(Rect rect, Size pixelSize)
  {
    rect.X = PixelSnapHelpers.PixelAlign(rect.X, pixelSize.Width);
    rect.Y = PixelSnapHelpers.PixelAlign(rect.Y, pixelSize.Height);
    rect.Width = PixelSnapHelpers.Round(rect.Width, pixelSize.Width);
    rect.Height = PixelSnapHelpers.Round(rect.Height, pixelSize.Height);
    return rect;
  }

  public static Point Round(Point point, Size pixelSize)
  {
    return new Point(PixelSnapHelpers.Round(point.X, pixelSize.Width), PixelSnapHelpers.Round(point.Y, pixelSize.Height));
  }

  public static Rect Round(Rect rect, Size pixelSize)
  {
    return new Rect(PixelSnapHelpers.Round(rect.X, pixelSize.Width), PixelSnapHelpers.Round(rect.Y, pixelSize.Height), PixelSnapHelpers.Round(rect.Width, pixelSize.Width), PixelSnapHelpers.Round(rect.Height, pixelSize.Height));
  }

  public static double Round(double value, double pixelSize)
  {
    return pixelSize * Math.Round(value / pixelSize, MidpointRounding.AwayFromZero);
  }

  public static double RoundToOdd(double value, double pixelSize)
  {
    return PixelSnapHelpers.Round(value - pixelSize, pixelSize * 2.0) + pixelSize;
  }
}
