// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Win32Helper
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Runtime.InteropServices;
using System.Windows;

#nullable disable
namespace Xceed.Wpf.AvalonDock;

internal static class Win32Helper
{
  internal const int WS_CHILD = 1073741824 /*0x40000000*/;
  internal const int WS_VISIBLE = 268435456 /*0x10000000*/;
  internal const int WS_VSCROLL = 2097152 /*0x200000*/;
  internal const int WS_BORDER = 8388608 /*0x800000*/;
  internal const int WS_CLIPSIBLINGS = 67108864 /*0x04000000*/;
  internal const int WS_CLIPCHILDREN = 33554432 /*0x02000000*/;
  internal const int WS_TABSTOP = 65536 /*0x010000*/;
  internal const int WS_GROUP = 131072 /*0x020000*/;
  internal static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
  internal static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
  internal static readonly IntPtr HWND_TOP = new IntPtr(0);
  internal static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
  internal const int WM_WINDOWPOSCHANGED = 71;
  internal const int WM_WINDOWPOSCHANGING = 70;
  internal const int WM_NCMOUSEMOVE = 160 /*0xA0*/;
  internal const int WM_NCLBUTTONDOWN = 161;
  internal const int WM_NCLBUTTONUP = 162;
  internal const int WM_NCLBUTTONDBLCLK = 163;
  internal const int WM_NCRBUTTONDOWN = 164;
  internal const int WM_NCRBUTTONUP = 165;
  internal const int WM_CAPTURECHANGED = 533;
  internal const int WM_EXITSIZEMOVE = 562;
  internal const int WM_ENTERSIZEMOVE = 561;
  internal const int WM_MOVE = 3;
  internal const int WM_MOVING = 534;
  internal const int WM_KILLFOCUS = 8;
  internal const int WM_SETFOCUS = 7;
  internal const int WM_ACTIVATE = 6;
  internal const int WM_NCHITTEST = 132;
  internal const int WM_INITMENUPOPUP = 279;
  internal const int WM_KEYDOWN = 256 /*0x0100*/;
  internal const int WM_KEYUP = 257;
  internal const int WA_INACTIVE = 0;
  internal const int WM_SYSCOMMAND = 274;
  internal const int SC_MAXIMIZE = 61488;
  internal const int SC_RESTORE = 61728;
  internal const int WM_CREATE = 1;
  internal const int HT_CAPTION = 2;
  public const int HCBT_SETFOCUS = 9;
  public const int HCBT_ACTIVATE = 5;
  internal const uint GW_HWNDNEXT = 2;
  internal const uint GW_HWNDPREV = 3;
  internal const int WM_MOUSEMOVE = 512 /*0x0200*/;
  internal const int WM_LBUTTONDOWN = 513;
  internal const int WM_LBUTTONUP = 514;
  internal const int WM_LBUTTONDBLCLK = 515;
  internal const int WM_RBUTTONDOWN = 516;
  internal const int WM_RBUTTONUP = 517;
  internal const int WM_RBUTTONDBLCLK = 518;
  internal const int WM_MBUTTONDOWN = 519;
  internal const int WM_MBUTTONUP = 520;
  internal const int WM_MBUTTONDBLCLK = 521;
  internal const int WM_MOUSEWHEEL = 522;
  internal const int WM_MOUSEHWHEEL = 526;

  [DllImport("user32.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CreateWindowEx(
    int dwExStyle,
    string lpszClassName,
    string lpszWindowName,
    int style,
    int x,
    int y,
    int width,
    int height,
    IntPtr hwndParent,
    IntPtr hMenu,
    IntPtr hInst,
    [MarshalAs(UnmanagedType.AsAny)] object pvParam);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool SetWindowPos(
    IntPtr hWnd,
    IntPtr hWndInsertAfter,
    int X,
    int Y,
    int cx,
    int cy,
    Win32Helper.SetWindowPosFlags uFlags);

  [DllImport("user32.dll", CharSet = CharSet.Auto)]
  internal static extern bool IsChild(IntPtr hWndParent, IntPtr hwnd);

  [DllImport("user32.dll")]
  internal static extern IntPtr SetFocus(IntPtr hWnd);

  [DllImport("user32.dll", SetLastError = true)]
  public static extern IntPtr SetActiveWindow(IntPtr hWnd);

  [DllImport("user32.dll", CharSet = CharSet.Unicode)]
  internal static extern bool DestroyWindow(IntPtr hwnd);

  [DllImport("user32.dll")]
  internal static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

  [DllImport("user32.dll")]
  internal static extern int PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

  [DllImport("user32.dll")]
  private static extern bool GetClientRect(IntPtr hWnd, out Win32Helper.RECT lpRect);

  [DllImport("user32.dll")]
  internal static extern bool GetWindowRect(IntPtr hWnd, out Win32Helper.RECT lpRect);

  [DllImport("kernel32.dll")]
  public static extern uint GetCurrentThreadId();

  [DllImport("user32.dll")]
  public static extern IntPtr SetWindowsHookEx(
    Win32Helper.HookType code,
    Win32Helper.HookProc func,
    IntPtr hInstance,
    int threadID);

  [DllImport("user32.dll")]
  public static extern int UnhookWindowsHookEx(IntPtr hhook);

  [DllImport("user32.dll")]
  public static extern int CallNextHookEx(IntPtr hhook, int code, IntPtr wParam, IntPtr lParam);

  internal static Win32Helper.RECT GetClientRect(IntPtr hWnd)
  {
    Win32Helper.RECT lpRect = new Win32Helper.RECT();
    Win32Helper.GetClientRect(hWnd, out lpRect);
    return lpRect;
  }

  internal static Win32Helper.RECT GetWindowRect(IntPtr hWnd)
  {
    Win32Helper.RECT lpRect = new Win32Helper.RECT();
    Win32Helper.GetWindowRect(hWnd, out lpRect);
    return lpRect;
  }

  [DllImport("user32.dll")]
  internal static extern IntPtr GetTopWindow(IntPtr hWnd);

  [DllImport("user32.dll", SetLastError = true)]
  internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

  internal static int MakeLParam(int LoWord, int HiWord)
  {
    return HiWord << 16 /*0x10*/ | LoWord & (int) ushort.MaxValue;
  }

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool GetCursorPos(ref Win32Helper.Win32Point pt);

  internal static Point GetMousePosition()
  {
    Win32Helper.Win32Point pt = new Win32Helper.Win32Point();
    Win32Helper.GetCursorPos(ref pt);
    return new Point((double) pt.X, (double) pt.Y);
  }

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool IsWindowVisible(IntPtr hWnd);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool IsWindowEnabled(IntPtr hWnd);

  [DllImport("user32.dll")]
  internal static extern IntPtr GetFocus();

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool BringWindowToTop(IntPtr hWnd);

  [DllImport("user32.dll", SetLastError = true)]
  internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

  [DllImport("user32.dll", CharSet = CharSet.Auto)]
  internal static extern IntPtr GetParent(IntPtr hWnd);

  [DllImport("user32.dll")]
  private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

  [DllImport("user32.dll", SetLastError = true)]
  private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

  public static void SetOwner(IntPtr childHandle, IntPtr ownerHandle)
  {
    Win32Helper.SetWindowLong(childHandle, -8, ownerHandle.ToInt32());
  }

  public static IntPtr GetOwner(IntPtr childHandle)
  {
    return new IntPtr(Win32Helper.GetWindowLong(childHandle, -8));
  }

  [DllImport("user32.dll")]
  public static extern IntPtr MonitorFromRect([In] ref Win32Helper.RECT lprc, uint dwFlags);

  [DllImport("user32.dll")]
  public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool GetMonitorInfo(IntPtr hMonitor, [In, Out] Win32Helper.MonitorInfo lpmi);

  [Flags]
  internal enum SetWindowPosFlags : uint
  {
    SynchronousWindowPosition = 16384, // 0x00004000
    DeferErase = 8192, // 0x00002000
    DrawFrame = 32, // 0x00000020
    FrameChanged = DrawFrame, // 0x00000020
    HideWindow = 128, // 0x00000080
    DoNotActivate = 16, // 0x00000010
    DoNotCopyBits = 256, // 0x00000100
    IgnoreMove = 2,
    DoNotChangeOwnerZOrder = 512, // 0x00000200
    DoNotRedraw = 8,
    DoNotReposition = DoNotChangeOwnerZOrder, // 0x00000200
    DoNotSendChangingEvent = 1024, // 0x00000400
    IgnoreResize = 1,
    IgnoreZOrder = 4,
    ShowWindow = 64, // 0x00000040
  }

  [StructLayout(LayoutKind.Sequential)]
  internal class WINDOWPOS
  {
    public IntPtr hwnd;
    public IntPtr hwndInsertAfter;
    public int x;
    public int y;
    public int cx;
    public int cy;
    public int flags;
  }

  public enum HookType
  {
    WH_JOURNALRECORD,
    WH_JOURNALPLAYBACK,
    WH_KEYBOARD,
    WH_GETMESSAGE,
    WH_CALLWNDPROC,
    WH_CBT,
    WH_SYSMSGFILTER,
    WH_MOUSE,
    WH_HARDWARE,
    WH_DEBUG,
    WH_SHELL,
    WH_FOREGROUNDIDLE,
    WH_CALLWNDPROCRET,
    WH_KEYBOARD_LL,
    WH_MOUSE_LL,
  }

  public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

  [Serializable]
  internal struct RECT(int left_, int top_, int right_, int bottom_)
  {
    public int Left = left_;
    public int Top = top_;
    public int Right = right_;
    public int Bottom = bottom_;

    public int Height => this.Bottom - this.Top;

    public int Width => this.Right - this.Left;

    public Size Size => new Size((double) this.Width, (double) this.Height);

    public Point Location => new Point((double) this.Left, (double) this.Top);

    public Rect ToRectangle()
    {
      return new Rect((double) this.Left, (double) this.Top, (double) this.Right, (double) this.Bottom);
    }

    public static Win32Helper.RECT FromRectangle(Rect rectangle)
    {
      return (Win32Helper.RECT) new Rect(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
    }

    public override int GetHashCode()
    {
      return this.Left ^ (this.Top << 13 | this.Top >> 19) ^ (this.Width << 26 | this.Width >> 6) ^ (this.Height << 7 | this.Height >> 25);
    }

    public static implicit operator Rect(Win32Helper.RECT rect) => rect.ToRectangle();

    public static implicit operator Win32Helper.RECT(Rect rect)
    {
      return Win32Helper.RECT.FromRectangle(rect);
    }
  }

  internal enum GetWindow_Cmd : uint
  {
    GW_HWNDFIRST,
    GW_HWNDLAST,
    GW_HWNDNEXT,
    GW_HWNDPREV,
    GW_OWNER,
    GW_CHILD,
    GW_ENABLEDPOPUP,
  }

  internal struct Win32Point
  {
    public int X;
    public int Y;
  }

  [StructLayout(LayoutKind.Sequential)]
  public class MonitorInfo
  {
    public int Size = Marshal.SizeOf(typeof (Win32Helper.MonitorInfo));
    public Win32Helper.RECT Monitor;
    public Win32Helper.RECT Work;
    public uint Flags;
  }
}
