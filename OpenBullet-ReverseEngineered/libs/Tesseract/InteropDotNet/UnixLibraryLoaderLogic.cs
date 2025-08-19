// Decompiled with JetBrains decompiler
// Type: InteropDotNet.UnixLibraryLoaderLogic
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Runtime.InteropServices;
using Tesseract.Internal;

#nullable disable
namespace InteropDotNet;

internal class UnixLibraryLoaderLogic : ILibraryLoaderLogic
{
  private const int RTLD_NOW = 2;

  public IntPtr LoadLibrary(string fileName)
  {
    IntPtr num = IntPtr.Zero;
    try
    {
      Logger.TraceInformation("Trying to load native library \"{0}\"...", (object) fileName);
      num = UnixLibraryLoaderLogic.UnixLoadLibrary(fileName, 2);
      if (num != IntPtr.Zero)
        Logger.TraceInformation("Successfully loaded native library \"{0}\", handle = {1}.", (object) fileName, (object) num);
      else
        Logger.TraceError("Failed to load native library \"{0}\".\r\nCheck windows event log.", (object) fileName);
    }
    catch (Exception ex)
    {
      IntPtr lastError = UnixLibraryLoaderLogic.UnixGetLastError();
      Logger.TraceError("Failed to load native library \"{0}\".\r\nLast Error:{1}\r\nCheck inner exception and\\or windows event log.\r\nInner Exception: {2}", (object) fileName, (object) lastError, (object) ex.ToString());
    }
    return num;
  }

  public bool FreeLibrary(IntPtr libraryHandle)
  {
    return UnixLibraryLoaderLogic.UnixFreeLibrary(libraryHandle) != 0;
  }

  public IntPtr GetProcAddress(IntPtr libraryHandle, string functionName)
  {
    UnixLibraryLoaderLogic.UnixGetLastError();
    Logger.TraceInformation("Trying to load native function \"{0}\" from the library with handle {1}...", (object) functionName, (object) libraryHandle);
    IntPtr procAddress = UnixLibraryLoaderLogic.UnixGetProcAddress(libraryHandle, functionName);
    IntPtr lastError = UnixLibraryLoaderLogic.UnixGetLastError();
    if (lastError != IntPtr.Zero)
      throw new Exception("dlsym: " + Marshal.PtrToStringAnsi(lastError));
    if (procAddress != IntPtr.Zero && lastError == IntPtr.Zero)
      Logger.TraceInformation("Successfully loaded native function \"{0}\", function handle = {1}.", (object) functionName, (object) procAddress);
    else
      Logger.TraceError("Failed to load native function \"{0}\", function handle = {1}, error pointer = {2}", (object) functionName, (object) procAddress, (object) lastError);
    return procAddress;
  }

  public string FixUpLibraryName(string fileName)
  {
    if (!string.IsNullOrEmpty(fileName))
    {
      if (!fileName.EndsWith(".so", StringComparison.OrdinalIgnoreCase))
        fileName += ".so";
      if (!fileName.StartsWith("lib", StringComparison.OrdinalIgnoreCase))
        fileName = "lib" + fileName;
    }
    return fileName;
  }

  [DllImport("libdl.so", EntryPoint = "dlopen")]
  private static extern IntPtr UnixLoadLibrary(string fileName, int flags);

  [DllImport("libdl.so", EntryPoint = "dlclose", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
  private static extern int UnixFreeLibrary(IntPtr handle);

  [DllImport("libdl.so", EntryPoint = "dlsym", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
  private static extern IntPtr UnixGetProcAddress(IntPtr handle, string symbol);

  [DllImport("libdl.so", EntryPoint = "dlerror", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
  private static extern IntPtr UnixGetLastError();
}
