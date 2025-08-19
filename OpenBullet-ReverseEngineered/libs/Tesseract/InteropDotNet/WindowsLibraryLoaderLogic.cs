// Decompiled with JetBrains decompiler
// Type: InteropDotNet.WindowsLibraryLoaderLogic
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Runtime.InteropServices;
using Tesseract.Internal;

#nullable disable
namespace InteropDotNet;

internal class WindowsLibraryLoaderLogic : ILibraryLoaderLogic
{
  public IntPtr LoadLibrary(string fileName)
  {
    IntPtr num = IntPtr.Zero;
    try
    {
      Logger.TraceInformation("Trying to load native library \"{0}\"...", (object) fileName);
      num = WindowsLibraryLoaderLogic.WindowsLoadLibrary(fileName);
      if (num != IntPtr.Zero)
        Logger.TraceInformation("Successfully loaded native library \"{0}\", handle = {1}.", (object) fileName, (object) num);
      else
        Logger.TraceError("Failed to load native library \"{0}\".\r\nCheck windows event log.", (object) fileName);
    }
    catch (Exception ex)
    {
      int lastError = WindowsLibraryLoaderLogic.WindowsGetLastError();
      Logger.TraceError("Failed to load native library \"{0}\".\r\nLast Error:{1}\r\nCheck inner exception and\\or windows event log.\r\nInner Exception: {2}", (object) fileName, (object) lastError, (object) ex.ToString());
    }
    return num;
  }

  public bool FreeLibrary(IntPtr libraryHandle)
  {
    try
    {
      Logger.TraceInformation("Trying to free native library with handle {0} ...", (object) libraryHandle);
      bool flag = WindowsLibraryLoaderLogic.WindowsFreeLibrary(libraryHandle);
      if (flag)
        Logger.TraceInformation("Successfully freed native library with handle {0}.", (object) libraryHandle);
      else
        Logger.TraceError("Failed to free native library with handle {0}.\r\nCheck windows event log.", (object) libraryHandle);
      return flag;
    }
    catch (Exception ex)
    {
      int lastError = WindowsLibraryLoaderLogic.WindowsGetLastError();
      Logger.TraceError("Failed to free native library with handle {0}.\r\nLast Error:{1}\r\nCheck inner exception and\\or windows event log.\r\nInner Exception: {2}", (object) libraryHandle, (object) lastError, (object) ex.ToString());
      return false;
    }
  }

  public IntPtr GetProcAddress(IntPtr libraryHandle, string functionName)
  {
    try
    {
      Logger.TraceInformation("Trying to load native function \"{0}\" from the library with handle {1}...", (object) functionName, (object) libraryHandle);
      IntPtr procAddress = WindowsLibraryLoaderLogic.WindowsGetProcAddress(libraryHandle, functionName);
      if (procAddress != IntPtr.Zero)
        Logger.TraceInformation("Successfully loaded native function \"{0}\", function handle = {1}.", (object) functionName, (object) procAddress);
      else
        Logger.TraceError("Failed to load native function \"{0}\", function handle = {1}", (object) functionName, (object) procAddress);
      return procAddress;
    }
    catch (Exception ex)
    {
      int lastError = WindowsLibraryLoaderLogic.WindowsGetLastError();
      Logger.TraceError("Failed to free native library with handle {0}.\r\nLast Error:{1}\r\nCheck inner exception and\\or windows event log.\r\nInner Exception: {2}", (object) libraryHandle, (object) lastError, (object) ex.ToString());
      return IntPtr.Zero;
    }
  }

  public string FixUpLibraryName(string fileName)
  {
    return !string.IsNullOrEmpty(fileName) && !fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ? fileName + ".dll" : fileName;
  }

  [DllImport("kernel32", EntryPoint = "LoadLibrary", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
  private static extern IntPtr WindowsLoadLibrary(string dllPath);

  [DllImport("kernel32", EntryPoint = "FreeLibrary", CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
  private static extern bool WindowsFreeLibrary(IntPtr handle);

  [DllImport("kernel32", EntryPoint = "GetProcAddress", SetLastError = true)]
  private static extern IntPtr WindowsGetProcAddress(IntPtr handle, string procedureName);

  private static int WindowsGetLastError() => Marshal.GetLastWin32Error();
}
