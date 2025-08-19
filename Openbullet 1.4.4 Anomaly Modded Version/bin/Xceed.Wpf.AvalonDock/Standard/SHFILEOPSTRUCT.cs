// Decompiled with JetBrains decompiler
// Type: Standard.SHFILEOPSTRUCT
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
internal struct SHFILEOPSTRUCT
{
  public IntPtr hwnd;
  [MarshalAs(UnmanagedType.U4)]
  public FO wFunc;
  public string pFrom;
  public string pTo;
  [MarshalAs(UnmanagedType.U2)]
  public FOF fFlags;
  [MarshalAs(UnmanagedType.Bool)]
  public int fAnyOperationsAborted;
  public IntPtr hNameMappings;
  public string lpszProgressTitle;
}
