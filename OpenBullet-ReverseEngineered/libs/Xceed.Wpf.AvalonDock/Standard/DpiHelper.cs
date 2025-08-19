// Decompiled with JetBrains decompiler
// Type: Standard.DpiHelper
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;
using System.Windows.Media;

#nullable disable
namespace Standard;

internal static class DpiHelper
{
  private static Matrix _transformToDevice;
  private static Matrix _transformToDip;

  static DpiHelper()
  {
    using (SafeDC desktop = SafeDC.GetDesktop())
    {
      int deviceCaps1 = NativeMethods.GetDeviceCaps(desktop, DeviceCap.LOGPIXELSX);
      int deviceCaps2 = NativeMethods.GetDeviceCaps(desktop, DeviceCap.LOGPIXELSY);
      DpiHelper._transformToDip = Matrix.Identity;
      DpiHelper._transformToDip.Scale(96.0 / (double) deviceCaps1, 96.0 / (double) deviceCaps2);
      DpiHelper._transformToDevice = Matrix.Identity;
      DpiHelper._transformToDevice.Scale((double) deviceCaps1 / 96.0, (double) deviceCaps2 / 96.0);
    }
  }

  public static Point LogicalPixelsToDevice(Point logicalPoint)
  {
    return DpiHelper._transformToDevice.Transform(logicalPoint);
  }

  public static Point DevicePixelsToLogical(Point devicePoint)
  {
    return DpiHelper._transformToDip.Transform(devicePoint);
  }

  public static Rect LogicalRectToDevice(Rect logicalRectangle)
  {
    return new Rect(DpiHelper.LogicalPixelsToDevice(new Point(logicalRectangle.Left, logicalRectangle.Top)), DpiHelper.LogicalPixelsToDevice(new Point(logicalRectangle.Right, logicalRectangle.Bottom)));
  }

  public static Rect DeviceRectToLogical(Rect deviceRectangle)
  {
    return new Rect(DpiHelper.DevicePixelsToLogical(new Point(deviceRectangle.Left, deviceRectangle.Top)), DpiHelper.DevicePixelsToLogical(new Point(deviceRectangle.Right, deviceRectangle.Bottom)));
  }

  public static Size LogicalSizeToDevice(Size logicalSize)
  {
    Point device = DpiHelper.LogicalPixelsToDevice(new Point(logicalSize.Width, logicalSize.Height));
    return new Size()
    {
      Width = device.X,
      Height = device.Y
    };
  }

  public static Size DeviceSizeToLogical(Size deviceSize)
  {
    Point logical = DpiHelper.DevicePixelsToLogical(new Point(deviceSize.Width, deviceSize.Height));
    return new Size(logical.X, logical.Y);
  }
}
