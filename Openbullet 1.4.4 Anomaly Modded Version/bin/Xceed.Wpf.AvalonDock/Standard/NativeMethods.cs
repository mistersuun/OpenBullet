// Decompiled with JetBrains decompiler
// Type: Standard.NativeMethods
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

#nullable disable
namespace Standard;

internal static class NativeMethods
{
  [DllImport("user32.dll", EntryPoint = "AdjustWindowRectEx", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _AdjustWindowRectEx(
    ref RECT lpRect,
    WS dwStyle,
    [MarshalAs(UnmanagedType.Bool)] bool bMenu,
    WS_EX dwExStyle);

  public static RECT AdjustWindowRectEx(RECT lpRect, WS dwStyle, bool bMenu, WS_EX dwExStyle)
  {
    if (!NativeMethods._AdjustWindowRectEx(ref lpRect, dwStyle, bMenu, dwExStyle))
      HRESULT.ThrowLastError();
    return lpRect;
  }

  [DllImport("user32.dll", EntryPoint = "ChangeWindowMessageFilter", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _ChangeWindowMessageFilter(WM message, MSGFLT dwFlag);

  [DllImport("user32.dll", EntryPoint = "ChangeWindowMessageFilterEx", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _ChangeWindowMessageFilterEx(
    IntPtr hwnd,
    WM message,
    MSGFLT action,
    [In, Out, Optional] ref CHANGEFILTERSTRUCT pChangeFilterStruct);

  public static HRESULT ChangeWindowMessageFilterEx(
    IntPtr hwnd,
    WM message,
    MSGFLT action,
    out MSGFLTINFO filterInfo)
  {
    filterInfo = MSGFLTINFO.NONE;
    if (!Utility.IsOSVistaOrNewer)
      return HRESULT.S_FALSE;
    if (!Utility.IsOSWindows7OrNewer)
      return !NativeMethods._ChangeWindowMessageFilter(message, action) ? (HRESULT) Win32Error.GetLastError() : HRESULT.S_OK;
    CHANGEFILTERSTRUCT pChangeFilterStruct = new CHANGEFILTERSTRUCT()
    {
      cbSize = (uint) Marshal.SizeOf(typeof (CHANGEFILTERSTRUCT))
    };
    if (!NativeMethods._ChangeWindowMessageFilterEx(hwnd, message, action, ref pChangeFilterStruct))
      return (HRESULT) Win32Error.GetLastError();
    filterInfo = pChangeFilterStruct.ExtStatus;
    return HRESULT.S_OK;
  }

  [DllImport("gdi32.dll")]
  public static extern CombineRgnResult CombineRgn(
    IntPtr hrgnDest,
    IntPtr hrgnSrc1,
    IntPtr hrgnSrc2,
    RGN fnCombineMode);

  [DllImport("shell32.dll", EntryPoint = "CommandLineToArgvW", CharSet = CharSet.Unicode)]
  private static extern IntPtr _CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string cmdLine, out int numArgs);

  public static string[] CommandLineToArgvW(string cmdLine)
  {
    IntPtr num = IntPtr.Zero;
    try
    {
      int numArgs = 0;
      num = NativeMethods._CommandLineToArgvW(cmdLine, out numArgs);
      if (num == IntPtr.Zero)
        throw new Win32Exception();
      string[] argvW = new string[numArgs];
      for (int index = 0; index < numArgs; ++index)
      {
        IntPtr ptr = Marshal.ReadIntPtr(num, index * Marshal.SizeOf(typeof (IntPtr)));
        argvW[index] = Marshal.PtrToStringUni(ptr);
      }
      return argvW;
    }
    finally
    {
      NativeMethods._LocalFree(num);
    }
  }

  [DllImport("gdi32.dll", EntryPoint = "CreateDIBSection", SetLastError = true)]
  private static extern SafeHBITMAP _CreateDIBSection(
    SafeDC hdc,
    [In] ref BITMAPINFO bitmapInfo,
    int iUsage,
    out IntPtr ppvBits,
    IntPtr hSection,
    int dwOffset);

  [DllImport("gdi32.dll", EntryPoint = "CreateDIBSection", SetLastError = true)]
  private static extern SafeHBITMAP _CreateDIBSectionIntPtr(
    IntPtr hdc,
    [In] ref BITMAPINFO bitmapInfo,
    int iUsage,
    out IntPtr ppvBits,
    IntPtr hSection,
    int dwOffset);

  public static SafeHBITMAP CreateDIBSection(
    SafeDC hdc,
    ref BITMAPINFO bitmapInfo,
    out IntPtr ppvBits,
    IntPtr hSection,
    int dwOffset)
  {
    SafeHBITMAP dibSection = hdc != null ? NativeMethods._CreateDIBSection(hdc, ref bitmapInfo, 0, out ppvBits, hSection, dwOffset) : NativeMethods._CreateDIBSectionIntPtr(IntPtr.Zero, ref bitmapInfo, 0, out ppvBits, hSection, dwOffset);
    if (dibSection.IsInvalid)
      HRESULT.ThrowLastError();
    return dibSection;
  }

  [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn", SetLastError = true)]
  private static extern IntPtr _CreateRoundRectRgn(
    int nLeftRect,
    int nTopRect,
    int nRightRect,
    int nBottomRect,
    int nWidthEllipse,
    int nHeightEllipse);

  public static IntPtr CreateRoundRectRgn(
    int nLeftRect,
    int nTopRect,
    int nRightRect,
    int nBottomRect,
    int nWidthEllipse,
    int nHeightEllipse)
  {
    IntPtr roundRectRgn = NativeMethods._CreateRoundRectRgn(nLeftRect, nTopRect, nRightRect, nBottomRect, nWidthEllipse, nHeightEllipse);
    return !(IntPtr.Zero == roundRectRgn) ? roundRectRgn : throw new Win32Exception();
  }

  [DllImport("gdi32.dll", EntryPoint = "CreateRectRgn", SetLastError = true)]
  private static extern IntPtr _CreateRectRgn(
    int nLeftRect,
    int nTopRect,
    int nRightRect,
    int nBottomRect);

  public static IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect)
  {
    IntPtr rectRgn = NativeMethods._CreateRectRgn(nLeftRect, nTopRect, nRightRect, nBottomRect);
    return !(IntPtr.Zero == rectRgn) ? rectRgn : throw new Win32Exception();
  }

  [DllImport("gdi32.dll", EntryPoint = "CreateRectRgnIndirect", SetLastError = true)]
  private static extern IntPtr _CreateRectRgnIndirect([In] ref RECT lprc);

  public static IntPtr CreateRectRgnIndirect(RECT lprc)
  {
    IntPtr rectRgnIndirect = NativeMethods._CreateRectRgnIndirect(ref lprc);
    return !(IntPtr.Zero == rectRgnIndirect) ? rectRgnIndirect : throw new Win32Exception();
  }

  [DllImport("gdi32.dll")]
  public static extern IntPtr CreateSolidBrush(int crColor);

  [DllImport("user32.dll", EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode, SetLastError = true)]
  private static extern IntPtr _CreateWindowEx(
    WS_EX dwExStyle,
    [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
    [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
    WS dwStyle,
    int x,
    int y,
    int nWidth,
    int nHeight,
    IntPtr hWndParent,
    IntPtr hMenu,
    IntPtr hInstance,
    IntPtr lpParam);

  public static IntPtr CreateWindowEx(
    WS_EX dwExStyle,
    string lpClassName,
    string lpWindowName,
    WS dwStyle,
    int x,
    int y,
    int nWidth,
    int nHeight,
    IntPtr hWndParent,
    IntPtr hMenu,
    IntPtr hInstance,
    IntPtr lpParam)
  {
    IntPtr windowEx = NativeMethods._CreateWindowEx(dwExStyle, lpClassName, lpWindowName, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);
    if (IntPtr.Zero == windowEx)
      HRESULT.ThrowLastError();
    return windowEx;
  }

  [DllImport("user32.dll", EntryPoint = "DefWindowProcW", CharSet = CharSet.Unicode)]
  public static extern IntPtr DefWindowProc(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

  [DllImport("gdi32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool DeleteObject(IntPtr hObject);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool DestroyIcon(IntPtr handle);

  [DllImport("user32.dll", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool DestroyWindow(IntPtr hwnd);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool IsWindow(IntPtr hwnd);

  [DllImport("dwmapi.dll", PreserveSig = false)]
  public static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

  [DllImport("dwmapi.dll", EntryPoint = "DwmIsCompositionEnabled", PreserveSig = false)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _DwmIsCompositionEnabled();

  [DllImport("dwmapi.dll", EntryPoint = "DwmGetColorizationColor")]
  private static extern HRESULT _DwmGetColorizationColor(
    out uint pcrColorization,
    [MarshalAs(UnmanagedType.Bool)] out bool pfOpaqueBlend);

  public static bool DwmGetColorizationColor(out uint pcrColorization, out bool pfOpaqueBlend)
  {
    if (Utility.IsOSVistaOrNewer && NativeMethods.IsThemeActive() && NativeMethods._DwmGetColorizationColor(out pcrColorization, out pfOpaqueBlend).Succeeded)
      return true;
    pcrColorization = 4278190080U /*0xFF000000*/;
    pfOpaqueBlend = true;
    return false;
  }

  public static bool DwmIsCompositionEnabled()
  {
    return Utility.IsOSVistaOrNewer && NativeMethods._DwmIsCompositionEnabled();
  }

  [DllImport("dwmapi.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool DwmDefWindowProc(
    IntPtr hwnd,
    WM msg,
    IntPtr wParam,
    IntPtr lParam,
    out IntPtr plResult);

  [DllImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute")]
  private static extern void _DwmSetWindowAttribute(
    IntPtr hwnd,
    DWMWA dwAttribute,
    ref int pvAttribute,
    int cbAttribute);

  public static void DwmSetWindowAttributeFlip3DPolicy(IntPtr hwnd, DWMFLIP3D flip3dPolicy)
  {
    int pvAttribute = (int) flip3dPolicy;
    NativeMethods._DwmSetWindowAttribute(hwnd, DWMWA.FLIP3D_POLICY, ref pvAttribute, 4);
  }

  public static void DwmSetWindowAttributeDisallowPeek(IntPtr hwnd, bool disallowPeek)
  {
    int pvAttribute = disallowPeek ? 1 : 0;
    NativeMethods._DwmSetWindowAttribute(hwnd, DWMWA.DISALLOW_PEEK, ref pvAttribute, 4);
  }

  [DllImport("user32.dll", EntryPoint = "EnableMenuItem")]
  private static extern int _EnableMenuItem(IntPtr hMenu, SC uIDEnableItem, MF uEnable);

  public static MF EnableMenuItem(IntPtr hMenu, SC uIDEnableItem, MF uEnable)
  {
    return (MF) NativeMethods._EnableMenuItem(hMenu, uIDEnableItem, uEnable);
  }

  [DllImport("user32.dll", EntryPoint = "RemoveMenu", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

  public static void RemoveMenu(IntPtr hMenu, SC uPosition, MF uFlags)
  {
    if (!NativeMethods._RemoveMenu(hMenu, (uint) uPosition, (uint) uFlags))
      throw new Win32Exception();
  }

  [DllImport("user32.dll", EntryPoint = "DrawMenuBar", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _DrawMenuBar(IntPtr hWnd);

  public static void DrawMenuBar(IntPtr hWnd)
  {
    if (!NativeMethods._DrawMenuBar(hWnd))
      throw new Win32Exception();
  }

  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
  [DllImport("kernel32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool FindClose(IntPtr handle);

  [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
  public static extern SafeFindHandle FindFirstFileW(
    string lpFileName,
    [MarshalAs(UnmanagedType.LPStruct), In, Out] WIN32_FIND_DATAW lpFindFileData);

  [DllImport("kernel32.dll", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool FindNextFileW(
    SafeFindHandle hndFindFile,
    [MarshalAs(UnmanagedType.LPStruct), In, Out] WIN32_FIND_DATAW lpFindFileData);

  [DllImport("user32.dll", EntryPoint = "GetClientRect", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _GetClientRect(IntPtr hwnd, out RECT lpRect);

  public static RECT GetClientRect(IntPtr hwnd)
  {
    RECT lpRect;
    if (!NativeMethods._GetClientRect(hwnd, out lpRect))
      HRESULT.ThrowLastError();
    return lpRect;
  }

  [DllImport("uxtheme.dll", EntryPoint = "GetCurrentThemeName", CharSet = CharSet.Unicode)]
  private static extern HRESULT _GetCurrentThemeName(
    StringBuilder pszThemeFileName,
    int dwMaxNameChars,
    StringBuilder pszColorBuff,
    int cchMaxColorChars,
    StringBuilder pszSizeBuff,
    int cchMaxSizeChars);

  public static void GetCurrentThemeName(
    out string themeFileName,
    out string color,
    out string size)
  {
    StringBuilder pszThemeFileName = new StringBuilder(260);
    StringBuilder pszColorBuff = new StringBuilder(260);
    StringBuilder pszSizeBuff = new StringBuilder(260);
    NativeMethods._GetCurrentThemeName(pszThemeFileName, pszThemeFileName.Capacity, pszColorBuff, pszColorBuff.Capacity, pszSizeBuff, pszSizeBuff.Capacity).ThrowIfFailed();
    themeFileName = pszThemeFileName.ToString();
    color = pszColorBuff.ToString();
    size = pszSizeBuff.ToString();
  }

  [DllImport("uxtheme.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool IsThemeActive();

  [Obsolete("Use SafeDC.GetDC instead.", true)]
  public static void GetDC()
  {
  }

  [DllImport("gdi32.dll")]
  public static extern int GetDeviceCaps(SafeDC hdc, DeviceCap nIndex);

  [DllImport("kernel32.dll", EntryPoint = "GetModuleFileName", CharSet = CharSet.Unicode, SetLastError = true)]
  private static extern int _GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, int nSize);

  public static string GetModuleFileName(IntPtr hModule)
  {
    StringBuilder lpFilename = new StringBuilder(260);
    while (true)
    {
      int moduleFileName = NativeMethods._GetModuleFileName(hModule, lpFilename, lpFilename.Capacity);
      if (moduleFileName == 0)
        HRESULT.ThrowLastError();
      if (moduleFileName == lpFilename.Capacity)
        lpFilename.EnsureCapacity(lpFilename.Capacity * 2);
      else
        break;
    }
    return lpFilename.ToString();
  }

  [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", CharSet = CharSet.Unicode, SetLastError = true)]
  private static extern IntPtr _GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

  public static IntPtr GetModuleHandle(string lpModuleName)
  {
    IntPtr moduleHandle = NativeMethods._GetModuleHandle(lpModuleName);
    if (!(moduleHandle == IntPtr.Zero))
      return moduleHandle;
    HRESULT.ThrowLastError();
    return moduleHandle;
  }

  [DllImport("user32.dll", EntryPoint = "GetMonitorInfo", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _GetMonitorInfo(IntPtr hMonitor, [In, Out] MONITORINFO lpmi);

  public static MONITORINFO GetMonitorInfo(IntPtr hMonitor)
  {
    MONITORINFO lpmi = new MONITORINFO();
    return NativeMethods._GetMonitorInfo(hMonitor, lpmi) ? lpmi : throw new Win32Exception();
  }

  [DllImport("gdi32.dll", EntryPoint = "GetStockObject", SetLastError = true)]
  private static extern IntPtr _GetStockObject(StockObject fnObject);

  public static IntPtr GetStockObject(StockObject fnObject)
  {
    return NativeMethods._GetStockObject(fnObject);
  }

  [DllImport("user32.dll")]
  public static extern IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

  [DllImport("user32.dll")]
  public static extern int GetSystemMetrics(SM nIndex);

  public static IntPtr GetWindowLongPtr(IntPtr hwnd, GWL nIndex)
  {
    IntPtr num = IntPtr.Zero;
    num = 8 != IntPtr.Size ? new IntPtr(NativeMethods.GetWindowLongPtr32(hwnd, nIndex)) : NativeMethods.GetWindowLongPtr64(hwnd, nIndex);
    return !(IntPtr.Zero == num) ? num : throw new Win32Exception();
  }

  [DllImport("uxtheme.dll", PreserveSig = false)]
  public static extern void SetWindowThemeAttribute(
    [In] IntPtr hwnd,
    [In] WINDOWTHEMEATTRIBUTETYPE eAttribute,
    [In] ref WTA_OPTIONS pvAttribute,
    [In] uint cbAttribute);

  [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
  private static extern int GetWindowLongPtr32(IntPtr hWnd, GWL nIndex);

  [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
  private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, GWL nIndex);

  [DllImport("user32.dll", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool GetWindowPlacement(IntPtr hwnd, WINDOWPLACEMENT lpwndpl);

  public static WINDOWPLACEMENT GetWindowPlacement(IntPtr hwnd)
  {
    WINDOWPLACEMENT lpwndpl = new WINDOWPLACEMENT();
    return NativeMethods.GetWindowPlacement(hwnd, lpwndpl) ? lpwndpl : throw new Win32Exception();
  }

  [DllImport("user32.dll", EntryPoint = "GetWindowRect", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _GetWindowRect(IntPtr hWnd, out RECT lpRect);

  public static RECT GetWindowRect(IntPtr hwnd)
  {
    RECT lpRect;
    if (!NativeMethods._GetWindowRect(hwnd, out lpRect))
      HRESULT.ThrowLastError();
    return lpRect;
  }

  [DllImport("gdiplus.dll")]
  public static extern Status GdipCreateBitmapFromStream(IStream stream, out IntPtr bitmap);

  [DllImport("gdiplus.dll")]
  public static extern Status GdipCreateHBITMAPFromBitmap(
    IntPtr bitmap,
    out IntPtr hbmReturn,
    int background);

  [DllImport("gdiplus.dll")]
  public static extern Status GdipCreateHICONFromBitmap(IntPtr bitmap, out IntPtr hbmReturn);

  [DllImport("gdiplus.dll")]
  public static extern Status GdipDisposeImage(IntPtr image);

  [DllImport("gdiplus.dll")]
  public static extern Status GdipImageForceValidation(IntPtr image);

  [DllImport("gdiplus.dll")]
  public static extern Status GdiplusStartup(
    out IntPtr token,
    StartupInput input,
    out StartupOutput output);

  [DllImport("gdiplus.dll")]
  public static extern Status GdiplusShutdown(IntPtr token);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool IsWindowVisible(IntPtr hwnd);

  [DllImport("kernel32.dll", EntryPoint = "LocalFree", SetLastError = true)]
  private static extern IntPtr _LocalFree(IntPtr hMem);

  [DllImport("user32.dll")]
  public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

  [DllImport("user32.dll", EntryPoint = "PostMessage", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _PostMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

  public static void PostMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam)
  {
    if (!NativeMethods._PostMessage(hWnd, Msg, wParam, lParam))
      throw new Win32Exception();
  }

  [DllImport("user32.dll", EntryPoint = "RegisterClassExW", SetLastError = true)]
  private static extern short _RegisterClassEx([In] ref WNDCLASSEX lpwcx);

  public static short RegisterClassEx(ref WNDCLASSEX lpwcx)
  {
    int num = (int) NativeMethods._RegisterClassEx(ref lpwcx);
    if (num != 0)
      return (short) num;
    HRESULT.ThrowLastError();
    return (short) num;
  }

  [DllImport("user32.dll", EntryPoint = "RegisterWindowMessage", CharSet = CharSet.Unicode, SetLastError = true)]
  private static extern uint _RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

  public static WM RegisterWindowMessage(string lpString)
  {
    int num = (int) NativeMethods._RegisterWindowMessage(lpString);
    if (num != 0)
      return (WM) num;
    HRESULT.ThrowLastError();
    return (WM) num;
  }

  [DllImport("user32.dll", EntryPoint = "SetActiveWindow", SetLastError = true)]
  private static extern IntPtr _SetActiveWindow(IntPtr hWnd);

  public static IntPtr SetActiveWindow(IntPtr hwnd)
  {
    Verify.IsNotDefault<IntPtr>(hwnd, nameof (hwnd));
    IntPtr num = NativeMethods._SetActiveWindow(hwnd);
    if (!(num == IntPtr.Zero))
      return num;
    HRESULT.ThrowLastError();
    return num;
  }

  public static IntPtr SetClassLongPtr(IntPtr hwnd, GCLP nIndex, IntPtr dwNewLong)
  {
    return 8 == IntPtr.Size ? NativeMethods.SetClassLongPtr64(hwnd, nIndex, dwNewLong) : new IntPtr(NativeMethods.SetClassLongPtr32(hwnd, nIndex, dwNewLong.ToInt32()));
  }

  [DllImport("user32.dll", EntryPoint = "SetClassLong", SetLastError = true)]
  private static extern int SetClassLongPtr32(IntPtr hWnd, GCLP nIndex, int dwNewLong);

  [DllImport("user32.dll", EntryPoint = "SetClassLongPtr", SetLastError = true)]
  private static extern IntPtr SetClassLongPtr64(IntPtr hWnd, GCLP nIndex, IntPtr dwNewLong);

  [DllImport("kernel32.dll", SetLastError = true)]
  public static extern ErrorModes SetErrorMode(ErrorModes newMode);

  [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _SetProcessWorkingSetSize(
    IntPtr hProcess,
    IntPtr dwMinimiumWorkingSetSize,
    IntPtr dwMaximumWorkingSetSize);

  public static void SetProcessWorkingSetSize(
    IntPtr hProcess,
    int dwMinimumWorkingSetSize,
    int dwMaximumWorkingSetSize)
  {
    if (!NativeMethods._SetProcessWorkingSetSize(hProcess, new IntPtr(dwMinimumWorkingSetSize), new IntPtr(dwMaximumWorkingSetSize)))
      throw new Win32Exception();
  }

  public static IntPtr SetWindowLongPtr(IntPtr hwnd, GWL nIndex, IntPtr dwNewLong)
  {
    return 8 == IntPtr.Size ? NativeMethods.SetWindowLongPtr64(hwnd, nIndex, dwNewLong) : new IntPtr(NativeMethods.SetWindowLongPtr32(hwnd, nIndex, dwNewLong.ToInt32()));
  }

  [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
  private static extern int SetWindowLongPtr32(IntPtr hWnd, GWL nIndex, int dwNewLong);

  [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
  private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

  [DllImport("user32.dll", EntryPoint = "SetWindowRgn", SetLastError = true)]
  private static extern int _SetWindowRgn(IntPtr hWnd, IntPtr hRgn, [MarshalAs(UnmanagedType.Bool)] bool bRedraw);

  public static void SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw)
  {
    if (NativeMethods._SetWindowRgn(hWnd, hRgn, bRedraw) == 0)
      throw new Win32Exception();
  }

  [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _SetWindowPos(
    IntPtr hWnd,
    IntPtr hWndInsertAfter,
    int x,
    int y,
    int cx,
    int cy,
    SWP uFlags);

  public static bool SetWindowPos(
    IntPtr hWnd,
    IntPtr hWndInsertAfter,
    int x,
    int y,
    int cx,
    int cy,
    SWP uFlags)
  {
    return NativeMethods._SetWindowPos(hWnd, hWndInsertAfter, x, y, cx, cy, uFlags);
  }

  [DllImport("shell32.dll")]
  public static extern Win32Error SHFileOperation(ref SHFILEOPSTRUCT lpFileOp);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool ShowWindow(IntPtr hwnd, SW nCmdShow);

  [DllImport("user32.dll", EntryPoint = "SystemParametersInfoW", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _SystemParametersInfo_String(
    SPI uiAction,
    int uiParam,
    [MarshalAs(UnmanagedType.LPWStr)] string pvParam,
    SPIF fWinIni);

  [DllImport("user32.dll", EntryPoint = "SystemParametersInfoW", CharSet = CharSet.Unicode, SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _SystemParametersInfo_NONCLIENTMETRICS(
    SPI uiAction,
    int uiParam,
    [In, Out] ref NONCLIENTMETRICS pvParam,
    SPIF fWinIni);

  [DllImport("user32.dll", EntryPoint = "SystemParametersInfoW", CharSet = CharSet.Unicode, SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _SystemParametersInfo_HIGHCONTRAST(
    SPI uiAction,
    int uiParam,
    [In, Out] ref HIGHCONTRAST pvParam,
    SPIF fWinIni);

  public static void SystemParametersInfo(SPI uiAction, int uiParam, string pvParam, SPIF fWinIni)
  {
    if (NativeMethods._SystemParametersInfo_String(uiAction, uiParam, pvParam, fWinIni))
      return;
    HRESULT.ThrowLastError();
  }

  public static NONCLIENTMETRICS SystemParameterInfo_GetNONCLIENTMETRICS()
  {
    NONCLIENTMETRICS pvParam = Utility.IsOSVistaOrNewer ? NONCLIENTMETRICS.VistaMetricsStruct : NONCLIENTMETRICS.XPMetricsStruct;
    if (!NativeMethods._SystemParametersInfo_NONCLIENTMETRICS(SPI.GETNONCLIENTMETRICS, pvParam.cbSize, ref pvParam, SPIF.None))
      HRESULT.ThrowLastError();
    return pvParam;
  }

  public static HIGHCONTRAST SystemParameterInfo_GetHIGHCONTRAST()
  {
    HIGHCONTRAST pvParam = new HIGHCONTRAST()
    {
      cbSize = Marshal.SizeOf(typeof (HIGHCONTRAST))
    };
    if (!NativeMethods._SystemParametersInfo_HIGHCONTRAST(SPI.GETHIGHCONTRAST, pvParam.cbSize, ref pvParam, SPIF.None))
      HRESULT.ThrowLastError();
    return pvParam;
  }

  [DllImport("user32.dll")]
  public static extern uint TrackPopupMenuEx(
    IntPtr hmenu,
    uint fuFlags,
    int x,
    int y,
    IntPtr hwnd,
    IntPtr lptpm);

  [DllImport("gdi32.dll", EntryPoint = "SelectObject", SetLastError = true)]
  private static extern IntPtr _SelectObject(SafeDC hdc, IntPtr hgdiobj);

  public static IntPtr SelectObject(SafeDC hdc, IntPtr hgdiobj)
  {
    IntPtr num = NativeMethods._SelectObject(hdc, hgdiobj);
    if (!(num == IntPtr.Zero))
      return num;
    HRESULT.ThrowLastError();
    return num;
  }

  [DllImport("gdi32.dll", EntryPoint = "SelectObject", SetLastError = true)]
  private static extern IntPtr _SelectObjectSafeHBITMAP(SafeDC hdc, SafeHBITMAP hgdiobj);

  public static IntPtr SelectObject(SafeDC hdc, SafeHBITMAP hgdiobj)
  {
    IntPtr num = NativeMethods._SelectObjectSafeHBITMAP(hdc, hgdiobj);
    if (!(num == IntPtr.Zero))
      return num;
    HRESULT.ThrowLastError();
    return num;
  }

  [DllImport("user32.dll", SetLastError = true)]
  public static extern int SendInput(int nInputs, ref INPUT pInputs, int cbSize);

  [DllImport("user32.dll", SetLastError = true)]
  public static extern IntPtr SendMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

  [DllImport("user32.dll", EntryPoint = "UnregisterClass", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _UnregisterClassAtom(IntPtr lpClassName, IntPtr hInstance);

  [DllImport("user32.dll", EntryPoint = "UnregisterClass", CharSet = CharSet.Unicode, SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _UnregisterClassName(string lpClassName, IntPtr hInstance);

  public static void UnregisterClass(short atom, IntPtr hinstance)
  {
    if (NativeMethods._UnregisterClassAtom(new IntPtr((int) atom), hinstance))
      return;
    HRESULT.ThrowLastError();
  }

  public static void UnregisterClass(string lpClassName, IntPtr hInstance)
  {
    if (NativeMethods._UnregisterClassName(lpClassName, hInstance))
      return;
    HRESULT.ThrowLastError();
  }

  [DllImport("user32.dll", EntryPoint = "UpdateLayeredWindow", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _UpdateLayeredWindow(
    IntPtr hwnd,
    SafeDC hdcDst,
    [In] ref POINT pptDst,
    [In] ref SIZE psize,
    SafeDC hdcSrc,
    [In] ref POINT pptSrc,
    int crKey,
    ref BLENDFUNCTION pblend,
    ULW dwFlags);

  [DllImport("user32.dll", EntryPoint = "UpdateLayeredWindow", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool _UpdateLayeredWindowIntPtr(
    IntPtr hwnd,
    IntPtr hdcDst,
    IntPtr pptDst,
    IntPtr psize,
    IntPtr hdcSrc,
    IntPtr pptSrc,
    int crKey,
    ref BLENDFUNCTION pblend,
    ULW dwFlags);

  public static void UpdateLayeredWindow(
    IntPtr hwnd,
    SafeDC hdcDst,
    ref POINT pptDst,
    ref SIZE psize,
    SafeDC hdcSrc,
    ref POINT pptSrc,
    int crKey,
    ref BLENDFUNCTION pblend,
    ULW dwFlags)
  {
    if (NativeMethods._UpdateLayeredWindow(hwnd, hdcDst, ref pptDst, ref psize, hdcSrc, ref pptSrc, crKey, ref pblend, dwFlags))
      return;
    HRESULT.ThrowLastError();
  }

  public static void UpdateLayeredWindow(
    IntPtr hwnd,
    int crKey,
    ref BLENDFUNCTION pblend,
    ULW dwFlags)
  {
    if (NativeMethods._UpdateLayeredWindowIntPtr(hwnd, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, crKey, ref pblend, dwFlags))
      return;
    HRESULT.ThrowLastError();
  }

  [DllImport("shell32.dll", EntryPoint = "SHAddToRecentDocs")]
  private static extern void _SHAddToRecentDocs_String(SHARD uFlags, [MarshalAs(UnmanagedType.LPWStr)] string pv);

  [DllImport("shell32.dll", EntryPoint = "SHAddToRecentDocs")]
  private static extern void _SHAddToRecentDocs_ShellLink(SHARD uFlags, IShellLinkW pv);

  public static void SHAddToRecentDocs(string path)
  {
    NativeMethods._SHAddToRecentDocs_String(SHARD.PATHW, path);
  }

  public static void SHAddToRecentDocs(IShellLinkW shellLink)
  {
    NativeMethods._SHAddToRecentDocs_ShellLink(SHARD.LINK, shellLink);
  }

  [DllImport("dwmapi.dll", EntryPoint = "DwmGetCompositionTimingInfo")]
  private static extern HRESULT _DwmGetCompositionTimingInfo(
    IntPtr hwnd,
    ref DWM_TIMING_INFO pTimingInfo);

  public static DWM_TIMING_INFO? DwmGetCompositionTimingInfo(IntPtr hwnd)
  {
    if (!Utility.IsOSVistaOrNewer)
      return new DWM_TIMING_INFO?();
    DWM_TIMING_INFO pTimingInfo = new DWM_TIMING_INFO()
    {
      cbSize = Marshal.SizeOf(typeof (DWM_TIMING_INFO))
    };
    HRESULT compositionTimingInfo = NativeMethods._DwmGetCompositionTimingInfo(Utility.IsOSWindows8OrNewer ? IntPtr.Zero : hwnd, ref pTimingInfo);
    if (compositionTimingInfo == HRESULT.E_PENDING)
      return new DWM_TIMING_INFO?();
    compositionTimingInfo.ThrowIfFailed();
    return new DWM_TIMING_INFO?(pTimingInfo);
  }

  [DllImport("dwmapi.dll", PreserveSig = false)]
  public static extern void DwmInvalidateIconicBitmaps(IntPtr hwnd);

  [DllImport("dwmapi.dll", PreserveSig = false)]
  public static extern void DwmSetIconicThumbnail(IntPtr hwnd, IntPtr hbmp, DWM_SIT dwSITFlags);

  [DllImport("dwmapi.dll", PreserveSig = false)]
  public static extern void DwmSetIconicLivePreviewBitmap(
    IntPtr hwnd,
    IntPtr hbmp,
    RefPOINT pptClient,
    DWM_SIT dwSITFlags);

  [DllImport("shell32.dll", PreserveSig = false)]
  public static extern void SHGetItemFromDataObject(
    IDataObject pdtobj,
    DOGIF dwFlags,
    [In] ref Guid riid,
    [MarshalAs(UnmanagedType.Interface)] out object ppv);

  [DllImport("shell32.dll", PreserveSig = false)]
  public static extern HRESULT SHCreateItemFromParsingName(
    [MarshalAs(UnmanagedType.LPWStr)] string pszPath,
    IBindCtx pbc,
    [In] ref Guid riid,
    [MarshalAs(UnmanagedType.Interface)] out object ppv);

  [DllImport("shell32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool Shell_NotifyIcon(NIM dwMessage, [In] NOTIFYICONDATA lpdata);

  [DllImport("shell32.dll", PreserveSig = false)]
  public static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

  [DllImport("shell32.dll")]
  public static extern HRESULT GetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] out string AppID);
}
