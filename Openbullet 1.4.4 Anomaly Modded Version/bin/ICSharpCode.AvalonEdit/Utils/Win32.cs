// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.Win32
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

internal static class Win32
{
  public static TimeSpan CaretBlinkTime
  {
    get => TimeSpan.FromMilliseconds((double) Win32.SafeNativeMethods.GetCaretBlinkTime());
  }

  public static bool CreateCaret(Visual owner, Size size)
  {
    if (owner == null)
      throw new ArgumentNullException(nameof (owner));
    if (!(PresentationSource.FromVisual(owner) is HwndSource hwndSource))
      return false;
    Vector vector = owner.PointToScreen(new Point(size.Width, size.Height)) - owner.PointToScreen(new Point(0.0, 0.0));
    return Win32.SafeNativeMethods.CreateCaret(hwndSource.Handle, IntPtr.Zero, (int) Math.Ceiling(vector.X), (int) Math.Ceiling(vector.Y));
  }

  public static bool SetCaretPosition(Visual owner, Point position)
  {
    if (owner == null)
      throw new ArgumentNullException(nameof (owner));
    if (!(PresentationSource.FromVisual(owner) is HwndSource hwndSource))
      return false;
    Point device = owner.TransformToAncestor(hwndSource.RootVisual).Transform(position).TransformToDevice(hwndSource.RootVisual);
    return Win32.SafeNativeMethods.SetCaretPos((int) device.X, (int) device.Y);
  }

  public static bool DestroyCaret() => Win32.SafeNativeMethods.DestroyCaret();

  [SuppressUnmanagedCodeSecurity]
  private static class SafeNativeMethods
  {
    [DllImport("user32.dll")]
    public static extern int GetCaretBlinkTime();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetCaretPos(int x, int y);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyCaret();
  }
}
