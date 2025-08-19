// Decompiled with JetBrains decompiler
// Type: Standard.NOTIFYICONDATA
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[StructLayout(LayoutKind.Sequential)]
internal class NOTIFYICONDATA
{
  public int cbSize;
  public IntPtr hWnd;
  public int uID;
  public NIF uFlags;
  public int uCallbackMessage;
  public IntPtr hIcon;
  [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128 /*0x80*/)]
  public char[] szTip = new char[128 /*0x80*/];
  public uint dwState;
  public uint dwStateMask;
  [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256 /*0x0100*/)]
  public char[] szInfo = new char[256 /*0x0100*/];
  public uint uVersion;
  [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64 /*0x40*/)]
  public char[] szInfoTitle = new char[64 /*0x40*/];
  public uint dwInfoFlags;
  public Guid guidItem;
  private IntPtr hBalloonIcon;
}
