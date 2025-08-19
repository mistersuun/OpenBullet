// Decompiled with JetBrains decompiler
// Type: Standard.THUMBBUTTON
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Unicode)]
internal struct THUMBBUTTON
{
  public const int THBN_CLICKED = 6144;
  public THB dwMask;
  public uint iId;
  public uint iBitmap;
  public IntPtr hIcon;
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
  public string szTip;
  public THBF dwFlags;
}
