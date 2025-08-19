// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Shell.SystemCommands
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using Standard;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

#nullable disable
namespace Microsoft.Windows.Shell;

public static class SystemCommands
{
  public static RoutedCommand CloseWindowCommand { get; private set; }

  public static RoutedCommand MaximizeWindowCommand { get; private set; }

  public static RoutedCommand MinimizeWindowCommand { get; private set; }

  public static RoutedCommand RestoreWindowCommand { get; private set; }

  public static RoutedCommand ShowSystemMenuCommand { get; private set; }

  static SystemCommands()
  {
    SystemCommands.CloseWindowCommand = new RoutedCommand("CloseWindow", typeof (SystemCommands));
    SystemCommands.MaximizeWindowCommand = new RoutedCommand("MaximizeWindow", typeof (SystemCommands));
    SystemCommands.MinimizeWindowCommand = new RoutedCommand("MinimizeWindow", typeof (SystemCommands));
    SystemCommands.RestoreWindowCommand = new RoutedCommand("RestoreWindow", typeof (SystemCommands));
    SystemCommands.ShowSystemMenuCommand = new RoutedCommand("ShowSystemMenu", typeof (SystemCommands));
  }

  private static void _PostSystemCommand(Window window, SC command)
  {
    IntPtr handle = new WindowInteropHelper(window).Handle;
    if (handle == IntPtr.Zero || !NativeMethods.IsWindow(handle))
      return;
    NativeMethods.PostMessage(handle, WM.SYSCOMMAND, new IntPtr((int) command), IntPtr.Zero);
  }

  public static void CloseWindow(Window window)
  {
    Verify.IsNotNull<Window>(window, nameof (window));
    SystemCommands._PostSystemCommand(window, SC.CLOSE);
  }

  public static void MaximizeWindow(Window window)
  {
    Verify.IsNotNull<Window>(window, nameof (window));
    SystemCommands._PostSystemCommand(window, SC.MAXIMIZE);
  }

  public static void MinimizeWindow(Window window)
  {
    Verify.IsNotNull<Window>(window, nameof (window));
    SystemCommands._PostSystemCommand(window, SC.MINIMIZE);
  }

  public static void RestoreWindow(Window window)
  {
    Verify.IsNotNull<Window>(window, nameof (window));
    SystemCommands._PostSystemCommand(window, SC.RESTORE);
  }

  public static void ShowSystemMenu(Window window, Point screenLocation)
  {
    Verify.IsNotNull<Window>(window, nameof (window));
    SystemCommands.ShowSystemMenuPhysicalCoordinates(window, DpiHelper.LogicalPixelsToDevice(screenLocation));
  }

  internal static void ShowSystemMenuPhysicalCoordinates(
    Window window,
    Point physicalScreenLocation)
  {
    Verify.IsNotNull<Window>(window, nameof (window));
    IntPtr handle = new WindowInteropHelper(window).Handle;
    if (handle == IntPtr.Zero || !NativeMethods.IsWindow(handle))
      return;
    uint num = NativeMethods.TrackPopupMenuEx(NativeMethods.GetSystemMenu(handle, false), 256U /*0x0100*/, (int) physicalScreenLocation.X, (int) physicalScreenLocation.Y, handle, IntPtr.Zero);
    if (num == 0U)
      return;
    NativeMethods.PostMessage(handle, WM.SYSCOMMAND, new IntPtr((long) num), IntPtr.Zero);
  }
}
