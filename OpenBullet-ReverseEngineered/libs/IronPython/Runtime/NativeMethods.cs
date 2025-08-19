// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.NativeMethods
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace IronPython.Runtime;

internal class NativeMethods
{
  public static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

  [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
  internal static extern IntPtr FindFirstFile(
    string fileName,
    out NativeMethods.WIN32_FIND_DATA data);

  [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false)]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool FindNextFile(
    IntPtr hndFindFile,
    out NativeMethods.WIN32_FIND_DATA lpFindFileData);

  [DllImport("kernel32")]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool FindClose(IntPtr handle);

  [DllImport("kernel32.dll", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool FlushFileBuffers(SafeFileHandle hFile);

  [BestFitMapping(false)]
  [Serializable]
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  internal struct WIN32_FIND_DATA
  {
    internal int dwFileAttributes;
    internal uint ftCreationTime_dwLowDateTime;
    internal uint ftCreationTime_dwHighDateTime;
    internal uint ftLastAccessTime_dwLowDateTime;
    internal uint ftLastAccessTime_dwHighDateTime;
    internal uint ftLastWriteTime_dwLowDateTime;
    internal uint ftLastWriteTime_dwHighDateTime;
    internal int nFileSizeHigh;
    internal int nFileSizeLow;
    internal int dwReserved0;
    internal int dwReserved1;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    internal string cFileName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
    internal string cAlternateFileName;
  }

  internal struct SECURITY_ATTRIBUTES
  {
    public int nLength;
    public IntPtr lpSecurityDescriptor;
    public int bInheritHandle;
  }
}
