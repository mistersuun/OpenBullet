// Decompiled with JetBrains decompiler
// Type: Standard.WIN32_FIND_DATAW
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System.IO;
using System.Runtime.InteropServices;

#nullable disable
namespace Standard;

[BestFitMapping(false)]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal class WIN32_FIND_DATAW
{
  public FileAttributes dwFileAttributes;
  public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
  public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
  public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
  public int nFileSizeHigh;
  public int nFileSizeLow;
  public int dwReserved0;
  public int dwReserved1;
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
  public string cFileName;
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
  public string cAlternateFileName;
}
