// Decompiled with JetBrains decompiler
// Type: Standard.CREATESTRUCT
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct CREATESTRUCT
{
  public IntPtr lpCreateParams;
  public IntPtr hInstance;
  public IntPtr hMenu;
  public IntPtr hwndParent;
  public int cy;
  public int cx;
  public int y;
  public int x;
  public WS style;
  [MarshalAs(UnmanagedType.LPWStr)]
  public string lpszName;
  [MarshalAs(UnmanagedType.LPWStr)]
  public string lpszClass;
  public WS_EX dwExStyle;
}
