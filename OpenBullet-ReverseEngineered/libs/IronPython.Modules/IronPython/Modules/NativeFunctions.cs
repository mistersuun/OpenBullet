// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.NativeFunctions
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

#nullable disable
namespace IronPython.Modules;

internal static class NativeFunctions
{
  private static NativeFunctions.SetMemoryDelegate _setMem = new NativeFunctions.SetMemoryDelegate(NativeFunctions.MemSet);
  private static NativeFunctions.MoveMemoryDelegate _moveMem = new NativeFunctions.MoveMemoryDelegate(NativeFunctions.MoveMemory);
  private const int RTLD_NOW = 2;
  private const int LMEM_ZEROINIT = 64 /*0x40*/;

  [DllImport("kernel32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool FreeLibrary(IntPtr hModule);

  [DllImport("kernel32.dll", SetLastError = true)]
  private static extern IntPtr LoadLibrary(string lpFileName);

  [DllImport("kernel32.dll")]
  public static extern void SetLastError(int errorCode);

  [DllImport("kernel32.dll")]
  public static extern int GetLastError();

  [DllImport("kernel32.dll")]
  private static extern IntPtr GetProcAddress(IntPtr module, string lpFileName);

  [DllImport("kernel32.dll")]
  private static extern IntPtr GetProcAddress(IntPtr module, IntPtr ordinal);

  [DllImport("kernel32.dll")]
  public static extern void CopyMemory(IntPtr destination, IntPtr source, IntPtr Length);

  [DllImport("libdl")]
  private static extern IntPtr dlopen(string filename, int flags);

  [DllImport("libdl")]
  private static extern IntPtr dlsym(IntPtr handle, string symbol);

  public static IntPtr LoadDLL(string filename, int flags)
  {
    if (Environment.OSVersion.Platform != PlatformID.Unix && Environment.OSVersion.Platform != PlatformID.MacOSX)
      return NativeFunctions.LoadLibrary(filename);
    if (flags == 0)
      flags = 2;
    return NativeFunctions.dlopen(filename, flags);
  }

  public static IntPtr LoadFunction(IntPtr module, string functionName)
  {
    return Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX ? NativeFunctions.dlsym(module, functionName) : NativeFunctions.GetProcAddress(module, functionName);
  }

  public static IntPtr LoadFunction(IntPtr module, IntPtr ordinal)
  {
    return Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX ? IntPtr.Zero : NativeFunctions.GetProcAddress(module, ordinal);
  }

  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
  public static IntPtr Calloc(IntPtr size)
  {
    return Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX ? NativeFunctions.calloc((IntPtr) 1, size) : NativeFunctions.LocalAlloc(64U /*0x40*/, size);
  }

  public static IntPtr GetMemMoveAddress()
  {
    return Marshal.GetFunctionPointerForDelegate((Delegate) NativeFunctions._moveMem);
  }

  public static IntPtr GetMemSetAddress()
  {
    return Marshal.GetFunctionPointerForDelegate((Delegate) NativeFunctions._setMem);
  }

  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
  [DllImport("kernel32.dll")]
  private static extern IntPtr LocalAlloc(uint flags, IntPtr size);

  [DllImport("libc")]
  private static extern IntPtr calloc(IntPtr num, IntPtr size);

  [DllImport("kernel32.dll")]
  private static extern void RtlMoveMemory(IntPtr Destination, IntPtr src, IntPtr length);

  [DllImport("libc")]
  private static extern IntPtr memmove(IntPtr dst, IntPtr src, IntPtr length);

  private static IntPtr MemSet(IntPtr dest, byte value, IntPtr length)
  {
    IntPtr num = dest.Add(length.ToInt32());
    for (IntPtr ptr = dest; ptr != num; ptr = new IntPtr(ptr.ToInt64() + 1L))
      Marshal.WriteByte(ptr, value);
    return dest;
  }

  private static IntPtr MoveMemory(IntPtr dest, IntPtr src, IntPtr length)
  {
    if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
      NativeFunctions.memmove(dest, src, length);
    else
      NativeFunctions.RtlMoveMemory(dest, src, length);
    return dest;
  }

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate IntPtr SetMemoryDelegate(IntPtr dest, byte value, IntPtr length);

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate IntPtr MoveMemoryDelegate(IntPtr dest, IntPtr src, IntPtr length);
}
