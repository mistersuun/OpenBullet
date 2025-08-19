// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.WindowHelper
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

#nullable disable
namespace Xceed.Wpf.AvalonDock;

internal static class WindowHelper
{
  public static bool IsAttachedToPresentationSource(this Visual element)
  {
    return PresentationSource.FromVisual(element) != null;
  }

  public static void SetParentToMainWindowOf(this Window window, Visual element)
  {
    Window window1 = Window.GetWindow((DependencyObject) element);
    if (window1 != null)
    {
      window.Owner = window1;
    }
    else
    {
      IntPtr hwnd;
      if (!element.GetParentWindowHandle(out hwnd))
        return;
      Win32Helper.SetOwner(new WindowInteropHelper(window).Handle, hwnd);
    }
  }

  public static IntPtr GetParentWindowHandle(this Window window)
  {
    return window.Owner != null ? new WindowInteropHelper(window.Owner).Handle : Win32Helper.GetOwner(new WindowInteropHelper(window).Handle);
  }

  public static bool GetParentWindowHandle(this Visual element, out IntPtr hwnd)
  {
    hwnd = IntPtr.Zero;
    if (!(PresentationSource.FromVisual(element) is HwndSource hwndSource))
      return false;
    hwnd = Win32Helper.GetParent(hwndSource.Handle);
    if (hwnd == IntPtr.Zero)
      hwnd = hwndSource.Handle;
    return true;
  }

  public static void SetParentWindowToNull(this Window window)
  {
    if (window.Owner != null)
      window.Owner = (Window) null;
    else
      Win32Helper.SetOwner(new WindowInteropHelper(window).Handle, IntPtr.Zero);
  }
}
