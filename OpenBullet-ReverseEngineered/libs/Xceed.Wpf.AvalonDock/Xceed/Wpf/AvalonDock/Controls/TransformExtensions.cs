// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Controls.TransformExtensions
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.Windows;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Controls;

internal static class TransformExtensions
{
  public static Point PointToScreenDPI(this Visual visual, Point pt)
  {
    Point screen = visual.PointToScreen(pt);
    return visual.TransformToDeviceDPI(screen);
  }

  public static Point PointToScreenDPIWithoutFlowDirection(
    this FrameworkElement element,
    Point point)
  {
    if (FrameworkElement.GetFlowDirection((DependencyObject) element) != FlowDirection.RightToLeft)
      return element.PointToScreenDPI(point);
    Point pt = new Point(element.TransformActualSizeToAncestor().Width - point.X, point.Y);
    return element.PointToScreenDPI(pt);
  }

  public static Rect GetScreenArea(this FrameworkElement element)
  {
    Point screenDpi = element.PointToScreenDPI(new Point());
    if (FrameworkElement.GetFlowDirection((DependencyObject) element) != FlowDirection.RightToLeft)
      return new Rect(screenDpi, element.TransformActualSizeToAncestor());
    Size ancestor = element.TransformActualSizeToAncestor();
    return new Rect(new Point(ancestor.Width - screenDpi.X, screenDpi.Y), ancestor);
  }

  public static Point TransformToDeviceDPI(this Visual visual, Point pt)
  {
    Matrix transformToDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
    return new Point(pt.X / transformToDevice.M11, pt.Y / transformToDevice.M22);
  }

  public static Size TransformFromDeviceDPI(this Visual visual, Size size)
  {
    Matrix transformToDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
    return new Size(size.Width * transformToDevice.M11, size.Height * transformToDevice.M22);
  }

  public static Point TransformFromDeviceDPI(this Visual visual, Point pt)
  {
    Matrix transformToDevice = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
    return new Point(pt.X * transformToDevice.M11, pt.Y * transformToDevice.M22);
  }

  public static bool CanTransform(this Visual visual)
  {
    return PresentationSource.FromVisual(visual) != null;
  }

  public static Size TransformActualSizeToAncestor(this FrameworkElement element)
  {
    if (PresentationSource.FromVisual((Visual) element) == null)
      return new Size(element.ActualWidth, element.ActualHeight);
    Visual rootVisual = PresentationSource.FromVisual((Visual) element).RootVisual;
    return element.TransformToAncestor(rootVisual).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight)).Size;
  }

  public static Size TransformSizeToAncestor(this FrameworkElement element, Size sizeToTransform)
  {
    if (PresentationSource.FromVisual((Visual) element) == null)
      return sizeToTransform;
    Visual rootVisual = PresentationSource.FromVisual((Visual) element).RootVisual;
    return element.TransformToAncestor(rootVisual).TransformBounds(new Rect(0.0, 0.0, sizeToTransform.Width, sizeToTransform.Height)).Size;
  }

  public static GeneralTransform TansformToAncestor(this FrameworkElement element)
  {
    if (PresentationSource.FromVisual((Visual) element) == null)
      return (GeneralTransform) new MatrixTransform(Matrix.Identity);
    Visual rootVisual = PresentationSource.FromVisual((Visual) element).RootVisual;
    return element.TransformToAncestor(rootVisual);
  }
}
