// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonSubprocess
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace IronPython.Modules;

[PythonType("_subprocess")]
public static class PythonSubprocess
{
  public const string __doc__ = "_subprocess Module";
  public const int CREATE_NEW_CONSOLE = 16 /*0x10*/;
  public const int CREATE_NEW_PROCESS_GROUP = 512 /*0x0200*/;
  public const int DUPLICATE_SAME_ACCESS = 2;
  public const int INFINITE = -1;
  public const int STARTF_USESHOWWINDOW = 1;
  public const int STARTF_USESTDHANDLES = 256 /*0x0100*/;
  public const int STD_ERROR_HANDLE = -12;
  public const int STD_INPUT_HANDLE = -10;
  public const int STD_OUTPUT_HANDLE = -11;
  public const int SW_HIDE = 0;
  public const int WAIT_OBJECT_0 = 0;
  public const int PIPE = -1;
  public const int STDOUT = -2;

  public static PythonTuple CreatePipe(CodeContext context, object pSec, int bufferSize)
  {
    PythonSubprocess.SECURITY_ATTRIBUTES lpPipeAttributes = new PythonSubprocess.SECURITY_ATTRIBUTES();
    lpPipeAttributes.nLength = Marshal.SizeOf((object) lpPipeAttributes);
    IntPtr hReadPipe;
    IntPtr hWritePipe;
    PythonSubprocess.CreatePipePI(out hReadPipe, out hWritePipe, ref lpPipeAttributes, (uint) bufferSize);
    return PythonTuple.MakeTuple((object) new PythonSubprocessHandle(hReadPipe), (object) new PythonSubprocessHandle(hWritePipe));
  }

  private static string FormatError(int errorCode) => new Win32Exception(errorCode).Message;

  public static PythonTuple CreateProcess(
    CodeContext context,
    string applicationName,
    string commandLineArgs,
    object pSec,
    object tSec,
    int? bInheritHandles,
    uint? dwCreationFlags,
    object lpEnvironment,
    string lpCurrentDirectory,
    object lpStartupInfo)
  {
    object boundAttr1 = PythonOps.GetBoundAttr(context, lpStartupInfo, "dwFlags");
    object boundAttr2 = PythonOps.GetBoundAttr(context, lpStartupInfo, "hStdInput");
    object boundAttr3 = PythonOps.GetBoundAttr(context, lpStartupInfo, "hStdOutput");
    object boundAttr4 = PythonOps.GetBoundAttr(context, lpStartupInfo, "hStdError");
    object boundAttr5 = PythonOps.GetBoundAttr(context, lpStartupInfo, "wShowWindow");
    int int32 = boundAttr1 != null ? Converter.ConvertToInt32(boundAttr1) : 0;
    IntPtr num1 = boundAttr2 != null ? new IntPtr(Converter.ConvertToInt32(boundAttr2)) : IntPtr.Zero;
    IntPtr num2 = boundAttr3 != null ? new IntPtr(Converter.ConvertToInt32(boundAttr3)) : IntPtr.Zero;
    IntPtr num3 = boundAttr4 != null ? new IntPtr(Converter.ConvertToInt32(boundAttr4)) : IntPtr.Zero;
    short int16 = boundAttr5 != null ? Converter.ConvertToInt16(boundAttr5) : (short) 0;
    PythonSubprocess.STARTUPINFO lpStartupInfo1 = new PythonSubprocess.STARTUPINFO();
    lpStartupInfo1.dwFlags = int32;
    lpStartupInfo1.hStdInput = num1;
    lpStartupInfo1.hStdOutput = num2;
    lpStartupInfo1.hStdError = num3;
    lpStartupInfo1.wShowWindow = int16;
    PythonSubprocess.SECURITY_ATTRIBUTES lpProcessAttributes = new PythonSubprocess.SECURITY_ATTRIBUTES();
    lpProcessAttributes.nLength = Marshal.SizeOf((object) lpProcessAttributes);
    PythonSubprocess.SECURITY_ATTRIBUTES lpThreadAttributes = new PythonSubprocess.SECURITY_ATTRIBUTES();
    lpThreadAttributes.nLength = Marshal.SizeOf((object) lpThreadAttributes);
    string native = PythonSubprocess.EnvironmentToNative(context, lpEnvironment);
    PythonSubprocess.PROCESS_INFORMATION lpProcessInformation = new PythonSubprocess.PROCESS_INFORMATION();
    if (!PythonSubprocess.CreateProcessPI(string.IsNullOrEmpty(applicationName) ? (string) null : applicationName, string.IsNullOrEmpty(commandLineArgs) ? (string) null : commandLineArgs, ref lpProcessAttributes, ref lpThreadAttributes, bInheritHandles.HasValue && bInheritHandles.Value > 0, dwCreationFlags.HasValue ? dwCreationFlags.Value : 0U, native, lpCurrentDirectory, ref lpStartupInfo1, out lpProcessInformation))
    {
      int lastWin32Error = Marshal.GetLastWin32Error();
      throw PythonExceptions.CreateThrowable(PythonExceptions.WindowsError, (object) lastWin32Error, (object) PythonSubprocess.FormatError(lastWin32Error));
    }
    return PythonTuple.MakeTuple((object) new PythonSubprocessHandle(lpProcessInformation.hProcess, true), (object) new PythonSubprocessHandle(lpProcessInformation.hThread), (object) lpProcessInformation.dwProcessId, (object) lpProcessInformation.dwThreadId);
  }

  private static string EnvironmentToNative(CodeContext context, object environment)
  {
    if (environment == null)
      return (string) null;
    if (!(environment is PythonDictionary pythonDictionary))
      pythonDictionary = new PythonDictionary(context, environment);
    StringBuilder stringBuilder = new StringBuilder();
    foreach (KeyValuePair<object, object> keyValuePair in pythonDictionary)
    {
      stringBuilder.Append(keyValuePair.Key);
      stringBuilder.Append('=');
      stringBuilder.Append(keyValuePair.Value);
      stringBuilder.Append(char.MinValue);
    }
    return stringBuilder.ToString();
  }

  public static PythonSubprocessHandle DuplicateHandle(
    CodeContext context,
    BigInteger sourceProcess,
    PythonSubprocessHandle handle,
    BigInteger targetProcess,
    int desiredAccess,
    bool inherit_handle,
    object DUPLICATE_SAME_ACCESS)
  {
    if (handle._duplicated)
      return PythonSubprocess.DuplicateHandle(context, sourceProcess, (BigInteger) handle, targetProcess, desiredAccess, inherit_handle, DUPLICATE_SAME_ACCESS);
    PythonSubprocessHandle subprocessHandle = PythonSubprocess.DuplicateHandle(context, sourceProcess, (BigInteger) handle, targetProcess, desiredAccess, inherit_handle, DUPLICATE_SAME_ACCESS);
    subprocessHandle._duplicated = true;
    handle.Close();
    return subprocessHandle;
  }

  public static PythonSubprocessHandle DuplicateHandle(
    CodeContext context,
    BigInteger sourceProcess,
    BigInteger handle,
    BigInteger targetProcess,
    int desiredAccess,
    bool inherit_handle,
    object DUPLICATE_SAME_ACCESS)
  {
    IntPtr hSourceProcessHandle = new IntPtr((long) sourceProcess);
    IntPtr num1 = new IntPtr((long) handle);
    IntPtr num2 = new IntPtr((long) targetProcess);
    bool flag = DUPLICATE_SAME_ACCESS != null && Converter.ConvertToBoolean(DUPLICATE_SAME_ACCESS);
    IntPtr hSourceHandle = num1;
    IntPtr hTargetProcessHandle = num2;
    IntPtr handle1;
    ref IntPtr local = ref handle1;
    int uint32 = (int) Converter.ConvertToUInt32((object) desiredAccess);
    int num3 = inherit_handle ? 1 : 0;
    int dwOptions = flag ? 2 : 1;
    PythonSubprocess.DuplicateHandlePI(hSourceProcessHandle, hSourceHandle, hTargetProcessHandle, out local, (uint) uint32, num3 != 0, (uint) dwOptions);
    return new PythonSubprocessHandle(handle1);
  }

  public static PythonSubprocessHandle GetCurrentProcess()
  {
    return new PythonSubprocessHandle(PythonSubprocess.GetCurrentProcessPI());
  }

  public static int GetExitCodeProcess(PythonSubprocessHandle hProcess)
  {
    if (hProcess._isProcess && hProcess._closed)
      return hProcess._exitCode;
    IntPtr hProcess1 = new IntPtr(Converter.ConvertToInt32((object) hProcess));
    int minValue = int.MinValue;
    ref int local = ref minValue;
    PythonSubprocess.GetExitCodeProcessPI(hProcess1, out local);
    return minValue;
  }

  public static string GetModuleFileName(object ignored)
  {
    return Process.GetCurrentProcess().MainModule.FileName;
  }

  public static object GetStdHandle(int STD_OUTPUT_HANDLE)
  {
    return PythonSubprocess.GetStdHandlePI(STD_OUTPUT_HANDLE).ToPython();
  }

  public static int GetVersion() => PythonSubprocess.GetVersionPI();

  public static bool TerminateProcess(PythonSubprocessHandle handle, object uExitCode)
  {
    return PythonSubprocess.TerminateProcessPI(new IntPtr(Converter.ConvertToInt32((object) handle)), Converter.ConvertToUInt32(uExitCode));
  }

  public static int WaitForSingleObject(PythonSubprocessHandle handle, int dwMilliseconds)
  {
    return PythonSubprocess.WaitForSingleObjectPI((IntPtr) handle, dwMilliseconds);
  }

  [DllImport("kernel32.dll", EntryPoint = "CreateProcess", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool CreateProcessPI(
    string lpApplicationName,
    string lpCommandLine,
    ref PythonSubprocess.SECURITY_ATTRIBUTES lpProcessAttributes,
    ref PythonSubprocess.SECURITY_ATTRIBUTES lpThreadAttributes,
    [MarshalAs(UnmanagedType.Bool)] bool bInheritHandles,
    uint dwCreationFlags,
    string lpEnvironment,
    string lpCurrentDirectory,
    [In] ref PythonSubprocess.STARTUPINFO lpStartupInfo,
    out PythonSubprocess.PROCESS_INFORMATION lpProcessInformation);

  [DllImport("kernel32.dll", EntryPoint = "CreatePipe")]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool CreatePipePI(
    out IntPtr hReadPipe,
    out IntPtr hWritePipe,
    ref PythonSubprocess.SECURITY_ATTRIBUTES lpPipeAttributes,
    uint nSize);

  [DllImport("kernel32.dll", EntryPoint = "DuplicateHandle", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool DuplicateHandlePI(
    IntPtr hSourceProcessHandle,
    IntPtr hSourceHandle,
    IntPtr hTargetProcessHandle,
    out IntPtr lpTargetHandle,
    uint dwDesiredAccess,
    [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
    uint dwOptions);

  [DllImport("kernel32.dll", EntryPoint = "GetCurrentProcess")]
  private static extern IntPtr GetCurrentProcessPI();

  [DllImport("kernel32.dll", EntryPoint = "GetExitCodeProcess", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool GetExitCodeProcessPI(IntPtr hProcess, out int lpExitCode);

  [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true)]
  private static extern IntPtr GetStdHandlePI(int nStdHandle);

  [DllImport("kernel32.dll", EntryPoint = "GetVersion")]
  private static extern int GetVersionPI();

  [DllImport("kernel32.dll", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool CloseHandle(IntPtr hObject);

  [DllImport("kernel32.dll", EntryPoint = "TerminateProcess", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool TerminateProcessPI(IntPtr hProcess, uint uExitCode);

  [DllImport("kernel32", EntryPoint = "WaitForSingleObject", CharSet = CharSet.Ansi, SetLastError = true)]
  internal static extern int WaitForSingleObjectPI(IntPtr hHandle, int dwMilliseconds);

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  internal struct STARTUPINFO
  {
    public int cb;
    public string lpReserved;
    public string lpDesktop;
    public string lpTitle;
    public int dwX;
    public int dwY;
    public int dwXSize;
    public int dwYSize;
    public int dwXCountChars;
    public int dwYCountChars;
    public int dwFillAttribute;
    public int dwFlags;
    public short wShowWindow;
    public short cbReserved2;
    public IntPtr lpReserved2;
    public IntPtr hStdInput;
    public IntPtr hStdOutput;
    public IntPtr hStdError;
  }

  internal struct PROCESS_INFORMATION
  {
    public IntPtr hProcess;
    public IntPtr hThread;
    public int dwProcessId;
    public int dwThreadId;
  }

  internal struct SECURITY_ATTRIBUTES
  {
    public int nLength;
    public IntPtr lpSecurityDescriptor;
    public int bInheritHandle;
  }

  [Flags]
  internal enum DuplicateOptions : uint
  {
    DUPLICATE_CLOSE_SOURCE = 1,
    DUPLICATE_SAME_ACCESS = 2,
  }
}
