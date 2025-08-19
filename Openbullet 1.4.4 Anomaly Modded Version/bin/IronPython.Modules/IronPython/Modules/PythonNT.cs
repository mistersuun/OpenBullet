// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonNT
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using Mono.Unix.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

#nullable disable
namespace IronPython.Modules;

public static class PythonNT
{
  public const string __doc__ = "Provides low-level operationg system access for files, the environment, etc...";
  private static Dictionary<int, Process> _processToIdMapping = new Dictionary<int, Process>();
  private static List<int> _freeProcessIds = new List<int>();
  private static int _processCount;
  public static readonly object environ = (object) new PythonDictionary((DictionaryStorage) new EnvironmentDictionaryStorage());
  public static readonly PythonType error = Builtin.OSError;
  private const int DefaultBufferSize = 4096 /*0x1000*/;
  private static readonly object _umaskKey = new object();
  public const int O_APPEND = 8;
  public const int O_CREAT = 256 /*0x0100*/;
  public const int O_TRUNC = 512 /*0x0200*/;
  public const int O_EXCL = 1024 /*0x0400*/;
  public const int O_NOINHERIT = 128 /*0x80*/;
  public const int O_RANDOM = 16 /*0x10*/;
  public const int O_SEQUENTIAL = 32 /*0x20*/;
  public const int O_SHORT_LIVED = 4096 /*0x1000*/;
  public const int O_TEMPORARY = 64 /*0x40*/;
  public const int O_WRONLY = 1;
  public const int O_RDONLY = 0;
  public const int O_RDWR = 2;
  public const int O_BINARY = 32768 /*0x8000*/;
  public const int O_TEXT = 16384 /*0x4000*/;
  public const int P_WAIT = 0;
  public const int P_NOWAIT = 1;
  public const int P_NOWAITO = 3;
  public const int P_OVERLAY = 2;
  public const int P_DETACH = 4;
  public const int TMP_MAX = 32767 /*0x7FFF*/;
  private const int S_IWRITE = 146;
  private const int S_IREAD = 292;
  private const int S_IEXEC = 73;
  public const int F_OK = 0;
  public const int X_OK = 1;
  public const int W_OK = 2;
  public const int R_OK = 4;

  public static void abort() => Environment.FailFast("IronPython os.abort");

  public static bool access(CodeContext context, string path, int mode)
  {
    if (path == null)
      throw PythonOps.TypeError("expected string, got None");
    try
    {
      FileAttributes attributes = File.GetAttributes(path);
      return mode == 0 || (attributes & FileAttributes.Directory) != (FileAttributes) 0 || (attributes & FileAttributes.ReadOnly) == (FileAttributes) 0 || (mode & 2) == 0;
    }
    catch (ArgumentException ex)
    {
    }
    catch (PathTooLongException ex)
    {
    }
    catch (NotSupportedException ex)
    {
    }
    catch (FileNotFoundException ex)
    {
    }
    catch (DirectoryNotFoundException ex)
    {
    }
    catch (IOException ex)
    {
    }
    catch (UnauthorizedAccessException ex)
    {
    }
    return false;
  }

  public static void chdir([NotNull] string path)
  {
    if (string.IsNullOrEmpty(path))
      throw PythonExceptions.CreateThrowable(PythonNT.WindowsError, (object) 123, (object) "Path cannot be an empty string");
    try
    {
      Directory.SetCurrentDirectory(path);
    }
    catch (Exception ex)
    {
      string filename = path;
      throw PythonNT.ToPythonException(ex, filename);
    }
  }

  public static void chmod(string path, int mode)
  {
    try
    {
      FileInfo fileInfo = new FileInfo(path);
      if ((mode & 146) != 0)
        fileInfo.Attributes &= ~FileAttributes.ReadOnly;
      else
        fileInfo.Attributes |= FileAttributes.ReadOnly;
    }
    catch (Exception ex)
    {
      string filename = path;
      throw PythonNT.ToPythonException(ex, filename);
    }
  }

  public static void close(CodeContext context, int fd)
  {
    PythonContext languageContext = context.LanguageContext;
    PythonFileManager fileManager = languageContext.FileManager;
    PythonFile pf;
    if (fileManager.TryGetFileFromId(languageContext, fd, out pf))
    {
      fileManager.CloseIfLast(fd, pf);
    }
    else
    {
      if (!(fileManager.GetObjectFromId(fd) is Stream objectFromId))
        throw PythonExceptions.CreateThrowable(PythonExceptions.OSError, (object) 9, (object) "Bad file descriptor");
      fileManager.CloseIfLast(fd, objectFromId);
    }
  }

  public static void closerange(CodeContext context, int fd_low, int fd_high)
  {
    for (int fd = fd_low; fd <= fd_high; ++fd)
    {
      try
      {
        PythonNT.close(context, fd);
      }
      catch (OSException ex)
      {
      }
    }
  }

  private static bool IsValidFd(CodeContext context, int fd)
  {
    PythonContext languageContext = context.LanguageContext;
    object o;
    return languageContext.FileManager.TryGetFileFromId(languageContext, fd, out PythonFile _) || languageContext.FileManager.TryGetObjectFromId(languageContext, fd, out o) && o is Stream;
  }

  public static int dup(CodeContext context, int fd)
  {
    PythonContext languageContext = context.LanguageContext;
    PythonFile pf;
    if (languageContext.FileManager.TryGetFileFromId(languageContext, fd, out pf))
      return languageContext.FileManager.AddToStrongMapping(pf);
    if (!(languageContext.FileManager.GetObjectFromId(fd) is Stream objectFromId))
      throw PythonExceptions.CreateThrowable(PythonExceptions.OSError, (object) 9, (object) "Bad file descriptor");
    return languageContext.FileManager.AddToStrongMapping((object) objectFromId);
  }

  public static int dup2(CodeContext context, int fd, int fd2)
  {
    PythonContext languageContext = context.LanguageContext;
    if (!PythonNT.IsValidFd(context, fd))
      throw PythonExceptions.CreateThrowable(PythonExceptions.OSError, (object) 9, (object) "Bad file descriptor");
    if (!languageContext.FileManager.ValidateFdRange(fd2))
      throw PythonExceptions.CreateThrowable(PythonExceptions.OSError, (object) 9, (object) "Bad file descriptor");
    bool flag = PythonNT.IsValidFd(context, fd2);
    if (fd == fd2)
    {
      if (flag)
        return fd2;
      throw PythonExceptions.CreateThrowable(PythonExceptions.OSError, (object) 9, (object) "Bad file descriptor");
    }
    if (flag)
      PythonNT.close(context, fd2);
    PythonFile pf;
    if (languageContext.FileManager.TryGetFileFromId(languageContext, fd, out pf))
      return languageContext.FileManager.AddToStrongMapping(pf, fd2);
    if (!(languageContext.FileManager.GetObjectFromId(fd) is Stream objectFromId))
      throw PythonExceptions.CreateThrowable(PythonExceptions.OSError, (object) 9, (object) "Bad file descriptor");
    return languageContext.FileManager.AddToStrongMapping((object) objectFromId, fd2);
  }

  public static void _exit(CodeContext context, int code)
  {
    context.LanguageContext.DomainManager.Platform.TerminateScriptExecution(code);
  }

  public static object fdopen(CodeContext context, int fd) => PythonNT.fdopen(context, fd, "r");

  public static object fdopen(CodeContext context, int fd, string mode)
  {
    return PythonNT.fdopen(context, fd, mode, 0);
  }

  public static object fdopen(CodeContext context, int fd, string mode, int bufsize)
  {
    PythonFile.ValidateMode(mode);
    PythonContext languageContext = context.LanguageContext;
    PythonFile pf;
    if (languageContext.FileManager.TryGetFileFromId(languageContext, fd, out pf))
      return (object) pf;
    if (!(languageContext.FileManager.GetObjectFromId(fd) is Stream objectFromId))
      throw PythonExceptions.CreateThrowable(PythonExceptions.OSError, (object) 9, (object) "Bad file descriptor");
    return (object) PythonFile.Create(context, objectFromId, objectFromId.ToString(), mode);
  }

  [LightThrowing]
  public static object fstat(CodeContext context, int fd)
  {
    PythonContext languageContext = context.LanguageContext;
    PythonFile fileFromId = languageContext.FileManager.GetFileFromId(languageContext, fd);
    return fileFromId.IsConsole ? (object) new PythonNT.stat_result(8192 /*0x2000*/) : PythonNT.lstat(fileFromId.name);
  }

  public static void fsync(CodeContext context, int fd)
  {
    PythonContext languageContext = context.LanguageContext;
    PythonFile fileFromId = languageContext.FileManager.GetFileFromId(languageContext, fd);
    if (!fileFromId.IsOutput)
      throw PythonExceptions.CreateThrowable(PythonExceptions.OSError, (object) 9, (object) "Bad file descriptor");
    try
    {
      fileFromId.FlushToDisk();
    }
    catch (Exception ex)
    {
      switch (ex)
      {
        case ValueErrorException _:
        case IOException _:
          throw PythonExceptions.CreateThrowable(PythonExceptions.OSError, (object) 9, (object) "Bad file descriptor");
        default:
          throw;
      }
    }
  }

  public static string getcwd(CodeContext context)
  {
    return context.LanguageContext.DomainManager.Platform.CurrentDirectory;
  }

  public static string getcwdu(CodeContext context)
  {
    return context.LanguageContext.DomainManager.Platform.CurrentDirectory;
  }

  public static string _getfullpathname(CodeContext context, [NotNull] string dir)
  {
    PlatformAdaptationLayer platform = context.LanguageContext.DomainManager.Platform;
    try
    {
      return platform.GetFullPath(dir);
    }
    catch (ArgumentException ex)
    {
      string path = dir;
      if (PythonNT.IsWindows())
      {
        if (path.Length >= 2 && path[1] == ':' && (path[0] < 'a' || path[0] > 'z') && (path[0] < 'A' || path[0] > 'Z'))
        {
          if (path.Length == 2)
            return path + Path.DirectorySeparatorChar.ToString();
          return (int) path[2] == (int) Path.DirectorySeparatorChar ? path : path.Substring(0, 2) + Path.DirectorySeparatorChar.ToString() + path.Substring(2);
        }
        if (path.Length > 2 && path.IndexOf(':', 2) != -1)
          path = path.Substring(0, 2) + path.Substring(2).Replace(':', char.MaxValue);
        if (path.Length > 0 && path[0] == ':')
          path = "\uFFFF" + path.Substring(1);
      }
      foreach (char invalidPathChar in Path.GetInvalidPathChars())
        path = path.Replace(invalidPathChar, char.MaxValue);
      foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
      {
        if ((int) invalidFileNameChar != (int) Path.VolumeSeparatorChar && (int) invalidFileNameChar != (int) Path.DirectorySeparatorChar)
          path = path.Replace(invalidFileNameChar, char.MaxValue);
      }
      string str = platform.GetFullPath(path);
      int length = dir.Length;
      for (int index = str.Length - 1; index >= 0; --index)
      {
        if (str[index] == char.MaxValue)
        {
          for (--length; length >= 0; --length)
          {
            if (path[length] == char.MaxValue)
            {
              str = str.Substring(0, index) + dir[length].ToString() + str.Substring(index + 1);
              break;
            }
          }
        }
      }
      return str;
    }
  }

  private static bool IsWindows()
  {
    return Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32S || Environment.OSVersion.Platform == PlatformID.Win32Windows;
  }

  public static int getpid() => Process.GetCurrentProcess().Id;

  public static IronPython.Runtime.List listdir(CodeContext context, [NotNull] string path)
  {
    if (path == string.Empty)
      throw PythonOps.WindowsError("The system cannot find the path specified: '{0}'", (object) path);
    IronPython.Runtime.List ret = PythonOps.MakeList();
    try
    {
      PythonNT.addBase(context.LanguageContext.DomainManager.Platform.GetFileSystemEntries(path, "*"), ret);
      return ret;
    }
    catch (Exception ex)
    {
      string filename = path;
      throw PythonNT.ToPythonException(ex, filename);
    }
  }

  public static void lseek(CodeContext context, int filedes, long offset, int whence)
  {
    context.LanguageContext.FileManager.GetFileFromId(context.LanguageContext, filedes).seek(offset, whence);
  }

  [LightThrowing]
  public static object lstat([BytesConversion] string path) => PythonNT.stat(path);

  [PythonHidden(PlatformsAttribute.PlatformFamily.Windows)]
  public static void symlink(string source, string link_name)
  {
    if (Syscall.symlink(source, link_name) != 0)
      throw PythonExceptions.CreateThrowable(PythonExceptions.OSError, (object) 0, (object) source, (object) link_name);
  }

  [PythonHidden(PlatformsAttribute.PlatformFamily.Windows)]
  public static PythonTuple uname()
  {
    Utsname utsname;
    Syscall.uname(ref utsname);
    return PythonTuple.MakeTuple((object) utsname.sysname, (object) utsname.nodename, (object) utsname.release, (object) utsname.version, (object) utsname.machine);
  }

  [PythonHidden(PlatformsAttribute.PlatformFamily.Windows)]
  public static uint geteuid() => Syscall.geteuid();

  public static void mkdir(string path)
  {
    if (Directory.Exists(path))
      throw PythonNT.DirectoryExists();
    try
    {
      Directory.CreateDirectory(path);
    }
    catch (Exception ex)
    {
      string filename = path;
      throw PythonNT.ToPythonException(ex, filename);
    }
  }

  public static void mkdir(string path, int mode)
  {
    if (Directory.Exists(path))
      throw PythonNT.DirectoryExists();
    try
    {
      Directory.CreateDirectory(path);
    }
    catch (Exception ex)
    {
      string filename = path;
      throw PythonNT.ToPythonException(ex, filename);
    }
  }

  public static object open(CodeContext context, string filename, int flag)
  {
    return PythonNT.open(context, filename, flag, 777);
  }

  public static object open(CodeContext context, string filename, int flag, int mode)
  {
    try
    {
      FileMode mode1 = PythonNT.FileModeFromFlags(flag);
      FileAccess access = PythonNT.FileAccessFromFlags(flag);
      FileOptions options = PythonNT.FileOptionsFromFlags(flag);
      Stream stream;
      if (Environment.OSVersion.Platform == PlatformID.Win32NT && string.Compare(filename, "nul", true) == 0)
        stream = Stream.Null;
      else if (access == FileAccess.Read && (mode1 == FileMode.CreateNew || mode1 == FileMode.Create || mode1 == FileMode.Append))
      {
        new FileStream(filename, mode1, FileAccess.Write, FileShare.None).Dispose();
        stream = (Stream) new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096 /*0x1000*/, options);
      }
      else
        stream = access != FileAccess.ReadWrite || mode1 != FileMode.Append ? (Stream) new FileStream(filename, mode1, access, FileShare.ReadWrite, 4096 /*0x1000*/, options) : (Stream) new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096 /*0x1000*/, options);
      string mode2 = !stream.CanRead || !stream.CanWrite ? (!stream.CanWrite ? "r" : "w") : "w+";
      if ((flag & 32768 /*0x8000*/) != 0)
        mode2 += "b";
      return (object) context.LanguageContext.FileManager.AddToStrongMapping(PythonFile.Create(context, stream, filename, mode2));
    }
    catch (Exception ex)
    {
      string filename1 = filename;
      throw PythonNT.ToPythonException(ex, filename1);
    }
  }

  private static FileOptions FileOptionsFromFlags(int flag)
  {
    FileOptions fileOptions = FileOptions.None;
    if ((flag & 64 /*0x40*/) != 0)
      fileOptions |= FileOptions.DeleteOnClose;
    if ((flag & 16 /*0x10*/) != 0)
      fileOptions |= FileOptions.RandomAccess;
    if ((flag & 32 /*0x20*/) != 0)
      fileOptions |= FileOptions.SequentialScan;
    return fileOptions;
  }

  public static PythonTuple pipe(CodeContext context) => PythonFile.CreatePipeAsFd(context);

  public static PythonFile popen(CodeContext context, string command)
  {
    return PythonNT.popen(context, command, "r");
  }

  public static PythonFile popen(CodeContext context, string command, string mode)
  {
    return PythonNT.popen(context, command, mode, 4096 /*0x1000*/);
  }

  public static PythonFile popen(CodeContext context, string command, string mode, int bufsize)
  {
    if (string.IsNullOrEmpty(mode))
      mode = "r";
    ProcessStartInfo processInfo = PythonNT.GetProcessInfo(command, true);
    processInfo.CreateNoWindow = true;
    try
    {
      switch (mode)
      {
        case "r":
          processInfo.RedirectStandardOutput = true;
          Process process1 = Process.Start(processInfo);
          return (PythonFile) new PythonNT.POpenFile(context, command, process1, process1.StandardOutput.BaseStream, "r");
        case "w":
          processInfo.RedirectStandardInput = true;
          Process process2 = Process.Start(processInfo);
          return (PythonFile) new PythonNT.POpenFile(context, command, process2, process2.StandardInput.BaseStream, "w");
        default:
          throw PythonOps.ValueError("expected 'r' or 'w' for mode, got {0}", (object) mode);
      }
    }
    catch (Exception ex)
    {
      throw PythonNT.ToPythonException(ex);
    }
  }

  public static PythonTuple popen2(CodeContext context, string command)
  {
    return PythonNT.popen2(context, command, "t");
  }

  public static PythonTuple popen2(CodeContext context, string command, string mode)
  {
    return PythonNT.popen2(context, command, "t", 4096 /*0x1000*/);
  }

  public static PythonTuple popen2(CodeContext context, string command, string mode, int bufsize)
  {
    if (string.IsNullOrEmpty(mode))
      mode = "t";
    if (mode != "t" && mode != "b")
      throw PythonOps.ValueError("mode must be 't' or 'b' (default is t)");
    if (mode == "t")
      mode = string.Empty;
    try
    {
      ProcessStartInfo processInfo = PythonNT.GetProcessInfo(command, true);
      processInfo.RedirectStandardInput = true;
      processInfo.RedirectStandardOutput = true;
      processInfo.CreateNoWindow = true;
      Process process = Process.Start(processInfo);
      return PythonTuple.MakeTuple((object) new PythonNT.POpenFile(context, command, process, process.StandardInput.BaseStream, "w" + mode), (object) new PythonNT.POpenFile(context, command, process, process.StandardOutput.BaseStream, "r" + mode));
    }
    catch (Exception ex)
    {
      throw PythonNT.ToPythonException(ex);
    }
  }

  public static PythonTuple popen3(CodeContext context, string command)
  {
    return PythonNT.popen3(context, command, "t");
  }

  public static PythonTuple popen3(CodeContext context, string command, string mode)
  {
    return PythonNT.popen3(context, command, "t", 4096 /*0x1000*/);
  }

  public static PythonTuple popen3(CodeContext context, string command, string mode, int bufsize)
  {
    if (string.IsNullOrEmpty(mode))
      mode = "t";
    if (mode != "t" && mode != "b")
      throw PythonOps.ValueError("mode must be 't' or 'b' (default is t)");
    if (mode == "t")
      mode = string.Empty;
    try
    {
      ProcessStartInfo processInfo = PythonNT.GetProcessInfo(command, true);
      processInfo.RedirectStandardInput = true;
      processInfo.RedirectStandardOutput = true;
      processInfo.RedirectStandardError = true;
      processInfo.CreateNoWindow = true;
      Process process = Process.Start(processInfo);
      return PythonTuple.MakeTuple((object) new PythonNT.POpenFile(context, command, process, process.StandardInput.BaseStream, "w" + mode), (object) new PythonNT.POpenFile(context, command, process, process.StandardOutput.BaseStream, "r" + mode), (object) new PythonNT.POpenFile(context, command, process, process.StandardError.BaseStream, "r+" + mode));
    }
    catch (Exception ex)
    {
      throw PythonNT.ToPythonException(ex);
    }
  }

  public static void putenv(string varname, string value)
  {
    try
    {
      Environment.SetEnvironmentVariable(varname, value);
    }
    catch (Exception ex)
    {
      throw PythonNT.ToPythonException(ex);
    }
  }

  public static string read(CodeContext context, int fd, int buffersize)
  {
    if (buffersize < 0)
      throw PythonExceptions.CreateThrowable(PythonExceptions.OSError, (object) 22, (object) "Invalid argument");
    try
    {
      PythonContext languageContext = context.LanguageContext;
      return languageContext.FileManager.GetFileFromId(languageContext, fd).read(buffersize);
    }
    catch (Exception ex)
    {
      throw PythonNT.ToPythonException(ex);
    }
  }

  public static void rename(string src, string dst)
  {
    try
    {
      Directory.Move(src, dst);
    }
    catch (Exception ex)
    {
      throw PythonNT.ToPythonException(ex);
    }
  }

  public static void rmdir(string path)
  {
    try
    {
      Directory.Delete(path);
    }
    catch (Exception ex)
    {
      string filename = path;
      throw PythonNT.ToPythonException(ex, filename);
    }
  }

  public static object spawnl(CodeContext context, int mode, string path, params object[] args)
  {
    return PythonNT.SpawnProcessImpl(context, PythonNT.MakeProcess(), mode, path, (object) args);
  }

  public static object spawnle(CodeContext context, int mode, string path, params object[] args)
  {
    object newEnvironment = args.Length >= 1 ? args[args.Length - 1] : throw PythonOps.TypeError("spawnle() takes at least three arguments ({0} given)", (object) (2 + args.Length));
    object[] args1 = ArrayUtils.RemoveFirst<object>(args);
    Process process = PythonNT.MakeProcess();
    PythonNT.SetEnvironment(process.StartInfo.EnvironmentVariables, newEnvironment);
    return PythonNT.SpawnProcessImpl(context, process, mode, path, (object) args1);
  }

  public static object spawnv(CodeContext context, int mode, string path, object args)
  {
    return PythonNT.SpawnProcessImpl(context, PythonNT.MakeProcess(), mode, path, args);
  }

  public static object spawnve(
    CodeContext context,
    int mode,
    string path,
    object args,
    object env)
  {
    Process process = PythonNT.MakeProcess();
    PythonNT.SetEnvironment(process.StartInfo.EnvironmentVariables, env);
    return PythonNT.SpawnProcessImpl(context, process, mode, path, args);
  }

  private static Process MakeProcess()
  {
    try
    {
      return new Process();
    }
    catch (Exception ex)
    {
      throw PythonNT.ToPythonException(ex);
    }
  }

  private static object SpawnProcessImpl(
    CodeContext context,
    Process process,
    int mode,
    string path,
    object args)
  {
    try
    {
      process.StartInfo.Arguments = PythonNT.ArgumentsToString(context, args);
      process.StartInfo.FileName = path;
      process.StartInfo.UseShellExecute = false;
    }
    catch (Exception ex)
    {
      string filename = path;
      throw PythonNT.ToPythonException(ex, filename);
    }
    if (!process.Start())
      throw PythonOps.OSError("Cannot start process: {0}", (object) path);
    if (mode == 0)
    {
      process.WaitForExit();
      int exitCode = process.ExitCode;
      process.Dispose();
      return (object) exitCode;
    }
    lock (PythonNT._processToIdMapping)
    {
      int key;
      if (PythonNT._freeProcessIds.Count > 0)
      {
        key = PythonNT._freeProcessIds[PythonNT._freeProcessIds.Count - 1];
        PythonNT._freeProcessIds.RemoveAt(PythonNT._freeProcessIds.Count - 1);
      }
      else
      {
        PythonNT._processCount += 4;
        key = PythonNT._processCount;
      }
      PythonNT._processToIdMapping[key] = process;
      return ScriptingRuntimeHelpers.Int32ToObject(key);
    }
  }

  private static void SetEnvironment(StringDictionary currentEnvironment, object newEnvironment)
  {
    if (!(newEnvironment is PythonDictionary pythonDictionary))
      throw PythonOps.TypeError("env argument must be a dict");
    currentEnvironment.Clear();
    foreach (object key in pythonDictionary.keys())
    {
      string result1;
      if (!Converter.TryConvertToString(key, out result1))
        throw PythonOps.TypeError("env dict contains a non-string key");
      string result2;
      if (!Converter.TryConvertToString(pythonDictionary[key], out result2))
        throw PythonOps.TypeError("env dict contains a non-string value");
      currentEnvironment[result1] = result2;
    }
  }

  private static string ArgumentsToString(CodeContext context, object args)
  {
    StringBuilder stringBuilder = (StringBuilder) null;
    IEnumerator enumerator;
    if (!PythonOps.TryGetEnumerator(context, args, out enumerator))
      throw PythonOps.TypeError("args parameter must be sequence, not {0}", (object) DynamicHelpers.GetPythonType(args));
    bool flag = false;
    try
    {
      enumerator.MoveNext();
      while (enumerator.MoveNext())
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder();
        string str = PythonOps.ToString(enumerator.Current);
        if (flag)
          stringBuilder.Append(' ');
        if (str.IndexOf(' ') != -1)
        {
          stringBuilder.Append('"');
          stringBuilder.Append(str.Replace("\"", "\"\""));
          stringBuilder.Append('"');
        }
        else
          stringBuilder.Append(str);
        flag = true;
      }
    }
    finally
    {
      if (enumerator is IDisposable disposable)
        disposable.Dispose();
    }
    if (stringBuilder == null)
      return "";
    return stringBuilder.ToString();
  }

  [PythonHidden(PlatformsAttribute.PlatformFamily.Unix)]
  public static void startfile(string filename, string operation = "open")
  {
    Process process = new Process();
    process.StartInfo.FileName = filename;
    process.StartInfo.UseShellExecute = true;
    process.StartInfo.Verb = operation;
    try
    {
      process.Start();
    }
    catch (Exception ex)
    {
      string filename1 = filename;
      throw PythonNT.ToPythonException(ex, filename1);
    }
  }

  private static bool HasExecutableExtension(string path)
  {
    string lowerInvariant = Path.GetExtension(path).ToLowerInvariant();
    return lowerInvariant == ".exe" || lowerInvariant == ".dll" || lowerInvariant == ".com" || lowerInvariant == ".bat";
  }

  [Documentation("stat(path) -> stat result\nGathers statistics about the specified file or directory")]
  [LightThrowing]
  public static object stat([BytesConversion] string path)
  {
    if (path == null)
      return LightExceptions.Throw(PythonOps.TypeError("expected string, got NoneType"));
    try
    {
      FileInfo fileInfo = new FileInfo(path);
      long size;
      int num;
      if (Directory.Exists(path))
      {
        size = 0L;
        num = 16457;
      }
      else if (File.Exists(path))
      {
        size = fileInfo.Length;
        num = 32768 /*0x8000*/;
        if (PythonNT.HasExecutableExtension(path))
          num |= 73;
      }
      else
        return LightExceptions.Throw(PythonExceptions.CreateThrowable(PythonNT.WindowsError, (object) 3, (object) ("file does not exist: " + path)));
      int mode = num | 292;
      if ((fileInfo.Attributes & FileAttributes.ReadOnly) == (FileAttributes) 0)
        mode |= 146;
      if (Environment.OSVersion.Platform == PlatformID.Unix)
      {
        PythonNT.stat_result statResult = statUnix(mode);
        if (statResult != null)
          return (object) statResult;
      }
      DateTime dateTime1 = fileInfo.LastAccessTime;
      dateTime1 = dateTime1.ToUniversalTime();
      long timestamp1 = (long) PythonTime.TicksToTimestamp(dateTime1.Ticks);
      DateTime dateTime2 = fileInfo.CreationTime;
      dateTime2 = dateTime2.ToUniversalTime();
      long timestamp2 = (long) PythonTime.TicksToTimestamp(dateTime2.Ticks);
      DateTime dateTime3 = fileInfo.LastWriteTime;
      dateTime3 = dateTime3.ToUniversalTime();
      long timestamp3 = (long) PythonTime.TicksToTimestamp(dateTime3.Ticks);
      return (object) new PythonNT.stat_result(mode, (BigInteger) size, (BigInteger) timestamp1, (BigInteger) timestamp3, (BigInteger) timestamp2);
    }
    catch (ArgumentException ex)
    {
      return LightExceptions.Throw(PythonExceptions.CreateThrowable(PythonNT.WindowsError, (object) 123, (object) ("The path is invalid: " + path)));
    }
    catch (Exception ex)
    {
      string filename = path;
      return LightExceptions.Throw(PythonNT.ToPythonException(ex, filename));
    }

    PythonNT.stat_result statUnix(int mode)
    {
      Stat stat;
      return Syscall.stat(path, ref stat) == 0 ? new PythonNT.stat_result(stat, new int?(mode)) : (PythonNT.stat_result) null;
    }
  }

  public static string strerror(int code)
  {
    switch (code)
    {
      case 0:
        return "No error";
      case 1:
        return "Operation not permitted";
      case 2:
        return "No such file or directory";
      case 3:
        return "No such process";
      case 4:
        return "Interrupted function call";
      case 5:
        return "Input/output error";
      case 6:
        return "No such device or address";
      case 7:
        return "Arg list too long";
      case 8:
        return "Exec format error";
      case 9:
        return "Bad file descriptor";
      case 10:
        return "No child processes";
      case 11:
        return "Resource temporarily unavailable";
      case 12:
        return "Not enough space";
      case 13:
        return "Permission denied";
      case 14:
        return "Bad address";
      case 16 /*0x10*/:
        return "Resource device";
      case 17:
        return "File exists";
      case 18:
        return "Improper link";
      case 19:
        return "No such device";
      case 20:
        return "Not a directory";
      case 21:
        return "Is a directory";
      case 22:
        return "Invalid argument";
      case 23:
        return "Too many open files in system";
      case 24:
        return "Too many open files";
      case 25:
        return "Inappropriate I/O control operation";
      case 27:
        return "File too large";
      case 28:
        return "No space left on device";
      case 29:
        return "Invalid seek";
      case 30:
        return "Read-only file system";
      case 31 /*0x1F*/:
        return "Too many links";
      case 32 /*0x20*/:
        return "Broken pipe";
      case 33:
        return "Domain error";
      case 34:
        return "Result too large";
      case 36:
        return "Resource deadlock avoided";
      case 38:
        return "Filename too long";
      case 39:
        return "No locks available";
      case 40:
        return "Function not implemented";
      case 41:
        return "Directory not empty";
      case 42:
        return "Illegal byte sequence";
      case 10038:
        return "Unknown error";
      case 10056:
        return "Unknown error";
      case 10069:
        return "Unknown error";
      default:
        return "Unknown error " + (object) code;
    }
  }

  private static PythonType WindowsError => PythonExceptions.WindowsError;

  [Documentation("system(command) -> int\nExecute the command (a string) in a subshell.")]
  public static int system(string command)
  {
    ProcessStartInfo processInfo = PythonNT.GetProcessInfo(command, false);
    if (processInfo == null)
      return -1;
    processInfo.CreateNoWindow = false;
    try
    {
      Process process = Process.Start(processInfo);
      if (process == null)
        return -1;
      process.WaitForExit();
      return process.ExitCode;
    }
    catch (Win32Exception ex)
    {
      return 1;
    }
  }

  public static string tempnam(CodeContext context) => PythonNT.tempnam(context, (string) null);

  public static string tempnam(CodeContext context, string dir)
  {
    return PythonNT.tempnam(context, (string) null, (string) null);
  }

  public static string tempnam(CodeContext context, string dir, string prefix)
  {
    PythonOps.Warn(context, PythonExceptions.RuntimeWarning, "tempnam is a potential security risk to your program");
    try
    {
      dir = Path.GetTempPath();
      return Path.GetFullPath(Path.Combine(dir, prefix ?? string.Empty) + Path.GetRandomFileName());
    }
    catch (Exception ex)
    {
      string filename = dir;
      throw PythonNT.ToPythonException(ex, filename);
    }
  }

  public static object times()
  {
    Process currentProcess = Process.GetCurrentProcess();
    return (object) PythonTuple.MakeTuple((object) currentProcess.UserProcessorTime.TotalSeconds, (object) currentProcess.PrivilegedProcessorTime.TotalSeconds, (object) 0, (object) 0, (object) DateTime.Now.Subtract(currentProcess.StartTime).TotalSeconds);
  }

  public static PythonFile tmpfile(CodeContext context)
  {
    try
    {
      FileStream fileStream = new FileStream(Path.GetTempFileName(), FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096 /*0x1000*/, FileOptions.DeleteOnClose);
      return PythonFile.Create(context, (Stream) fileStream, fileStream.Name, "w+b");
    }
    catch (Exception ex)
    {
      throw PythonNT.ToPythonException(ex);
    }
  }

  public static string tmpnam(CodeContext context)
  {
    PythonOps.Warn(context, PythonExceptions.RuntimeWarning, "tmpnam is a potential security risk to your program");
    return Path.GetFullPath(Path.GetTempPath() + Path.GetRandomFileName());
  }

  public static void remove(string path) => PythonNT.UnlinkWorker(path);

  public static void unlink(string path) => PythonNT.UnlinkWorker(path);

  private static void UnlinkWorker(string path)
  {
    if (path == null)
      throw new ArgumentNullException(nameof (path));
    if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1 || Path.GetFileName(path).IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
      throw PythonExceptions.CreateThrowable(PythonNT.WindowsError, (object) 123, (object) ("The file could not be found for deletion: " + path));
    if (!File.Exists(path))
      throw PythonExceptions.CreateThrowable(PythonNT.WindowsError, (object) 2, (object) ("The file could not be found for deletion: " + path));
    try
    {
      File.Delete(path);
    }
    catch (Exception ex)
    {
      string filename = path;
      throw PythonNT.ToPythonException(ex, filename);
    }
  }

  public static void unsetenv(string varname)
  {
    Environment.SetEnvironmentVariable(varname, (string) null);
  }

  public static object urandom(int n)
  {
    RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
    byte[] data1 = new byte[n];
    byte[] data2 = data1;
    randomNumberGenerator.GetBytes(data2);
    return (object) PythonBinaryReader.PackDataIntoString(data1, n);
  }

  public static object urandom(BigInteger n) => PythonNT.urandom((int) n);

  public static object urandom(double n)
  {
    throw PythonOps.TypeError("integer argument expected, got float");
  }

  public static int umask(CodeContext context, int mask)
  {
    mask &= 384;
    object setModuleState = context.LanguageContext.GetSetModuleState(PythonNT._umaskKey, (object) mask);
    return setModuleState == null ? 0 : (int) setModuleState;
  }

  public static int umask(CodeContext context, BigInteger mask)
  {
    return PythonNT.umask(context, (int) mask);
  }

  public static int umask(double mask)
  {
    throw PythonOps.TypeError("integer argument expected, got float");
  }

  public static void utime(string path, PythonTuple times)
  {
    try
    {
      FileSystemInfo fileSystemInfo = Directory.Exists(path) ? (FileSystemInfo) new DirectoryInfo(path) : (FileSystemInfo) new FileInfo(path);
      if (times == null)
      {
        fileSystemInfo.LastAccessTime = DateTime.Now;
        fileSystemInfo.LastWriteTime = DateTime.Now;
      }
      else
      {
        DateTime dateTime1 = times.__len__() == 2 ? new DateTime(PythonTime.TimestampToTicks(Converter.ConvertToDouble(times[0])), DateTimeKind.Utc) : throw PythonOps.TypeError("times value must be a 2-value tuple (atime, mtime)");
        DateTime dateTime2 = new DateTime(PythonTime.TimestampToTicks(Converter.ConvertToDouble(times[1])), DateTimeKind.Utc);
        fileSystemInfo.LastAccessTime = dateTime1;
        fileSystemInfo.LastWriteTime = dateTime2;
      }
    }
    catch (Exception ex)
    {
      string filename = path;
      throw PythonNT.ToPythonException(ex, filename);
    }
  }

  public static PythonTuple waitpid(int pid, object options)
  {
    Process process;
    lock (PythonNT._processToIdMapping)
    {
      if (!PythonNT._processToIdMapping.TryGetValue(pid, out process))
        throw PythonExceptions.CreateThrowable(PythonExceptions.OSError, (object) 10, (object) "No child processes");
    }
    process.WaitForExit();
    PythonTuple pythonTuple = PythonTuple.MakeTuple((object) pid, (object) process.ExitCode);
    lock (PythonNT._processToIdMapping)
    {
      PythonNT._processToIdMapping.Remove(pid & -4);
      PythonNT._freeProcessIds.Add(pid & -4);
    }
    return pythonTuple;
  }

  public static int write(CodeContext context, int fd, [BytesConversion] string text)
  {
    try
    {
      PythonContext languageContext = context.LanguageContext;
      languageContext.FileManager.GetFileFromId(languageContext, fd).write(text);
      return text.Length;
    }
    catch (Exception ex)
    {
      throw PythonNT.ToPythonException(ex);
    }
  }

  [Documentation("Send signal sig to the process pid. Constants for the specific signals available on the host platform \r\nare defined in the signal module.")]
  public static void kill(CodeContext context, int pid, int sig)
  {
    if (PythonSignal.NativeSignal.GenerateConsoleCtrlEvent((uint) sig, (uint) pid))
      return;
    Process.GetProcessById(pid).Kill();
  }

  private static Exception ToPythonException(Exception e)
  {
    return PythonNT.ToPythonException(e, (string) null);
  }

  private static Exception ToPythonException(Exception e, string filename)
  {
    switch (e)
    {
      case IPythonAwareException _:
        return e;
      case ArgumentException _:
      case ArgumentNullException _:
      case ArgumentTypeException _:
        return ExceptionHelpers.UpdateForRethrow(e);
      default:
        int lastWin32Error = Marshal.GetLastWin32Error();
        string str = e.Message;
        int num = 0;
        bool flag = false;
        int hr;
        if (e is Win32Exception win32Exception)
        {
          hr = win32Exception.NativeErrorCode;
          str = PythonNT.GetFormattedException(e, hr);
          flag = true;
        }
        else
        {
          if (e is UnauthorizedAccessException)
          {
            flag = true;
            num = 5;
            str = filename == null ? "Access is denied" : $"Access is denied: '{filename}'";
          }
          if (e is IOException)
          {
            switch (lastWin32Error)
            {
              case 5:
                throw PythonExceptions.CreateThrowable(PythonNT.WindowsError, (object) lastWin32Error, (object) "Access is denied");
              case 32 /*0x20*/:
                throw PythonExceptions.CreateThrowable(PythonNT.WindowsError, (object) lastWin32Error, (object) "The process cannot access the file because it is being used by another process");
              case 145:
                throw PythonExceptions.CreateThrowable(PythonNT.WindowsError, (object) lastWin32Error, (object) "The directory is not empty");
            }
          }
          hr = Marshal.GetHRForException(e);
          if ((hr & -4096) == -2147024896 /*0x80070000*/)
          {
            hr &= 4095 /*0x0FFF*/;
            str = PythonNT.GetFormattedException(e, hr);
            flag = true;
          }
        }
        return flag ? PythonExceptions.CreateThrowable(PythonNT.WindowsError, (object) hr, (object) str) : PythonExceptions.CreateThrowable(PythonExceptions.OSError, (object) hr, (object) str);
    }
  }

  private static string GetFormattedException(Exception e, int hr)
  {
    return $"[Errno {hr.ToString()}] {e.Message}";
  }

  private static void addBase(string[] files, IronPython.Runtime.List ret)
  {
    foreach (string file in files)
      ret.AddNoLock((object) Path.GetFileName(file));
  }

  private static FileMode FileModeFromFlags(int flags)
  {
    if ((flags & 8) != 0)
      return FileMode.Append;
    if ((flags & 1024 /*0x0400*/) != 0)
      return (flags & 256 /*0x0100*/) != 0 ? FileMode.CreateNew : FileMode.Open;
    if ((flags & 256 /*0x0100*/) != 0)
      return FileMode.Create;
    return (flags & 512 /*0x0200*/) != 0 ? FileMode.Truncate : FileMode.Open;
  }

  private static FileAccess FileAccessFromFlags(int flags)
  {
    if ((flags & 2) != 0)
      return FileAccess.ReadWrite;
    return (flags & 1) != 0 ? FileAccess.Write : FileAccess.Read;
  }

  private static ProcessStartInfo GetProcessInfo(string command, bool throwException)
  {
    command = command.Trim();
    string baseCommand;
    string args;
    if (!PythonNT.TryGetExecutableCommand(command, out baseCommand, out args) && !PythonNT.TryGetShellCommand(command, out baseCommand, out args))
    {
      if (throwException)
        throw PythonOps.WindowsError("The system can not find command '{0}'", (object) command);
      return (ProcessStartInfo) null;
    }
    return new ProcessStartInfo(baseCommand, args)
    {
      UseShellExecute = false
    };
  }

  private static bool TryGetExecutableCommand(
    string command,
    out string baseCommand,
    out string args)
  {
    baseCommand = command;
    args = string.Empty;
    if (command[0] == '"')
    {
      int index;
      for (index = 1; index < command.Length; ++index)
      {
        if (command[index] == '"')
        {
          baseCommand = command.Substring(1, index - 1).Trim();
          if (index + 1 < command.Length)
          {
            args = command.Substring(index + 1);
            break;
          }
          break;
        }
      }
      if (index == command.Length)
      {
        baseCommand = command.Substring(1).Trim();
        command += "\"";
      }
    }
    else
    {
      int length = command.IndexOf(' ');
      if (length != -1)
      {
        baseCommand = command.Substring(0, length);
        args = command.Substring(length + 1);
      }
    }
    string fullPath = Path.GetFullPath(baseCommand);
    if (File.Exists(fullPath))
    {
      baseCommand = fullPath;
      return true;
    }
    string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
    string[] strArray = new string[5]
    {
      string.Empty,
      ".com",
      ".exe",
      "cmd",
      ".bat"
    };
    foreach (string str in strArray)
    {
      string path = Path.Combine(folderPath, baseCommand + str);
      if (File.Exists(path))
      {
        baseCommand = path;
        return true;
      }
    }
    return false;
  }

  private static bool TryGetShellCommand(string command, out string baseCommand, out string args)
  {
    baseCommand = Environment.GetEnvironmentVariable("COMSPEC");
    args = string.Empty;
    if (baseCommand == null)
    {
      baseCommand = Environment.GetEnvironmentVariable("SHELL");
      if (baseCommand == null)
        return false;
      args = $"-c \"{command}\"";
    }
    else
      args = $"/c {command}";
    return true;
  }

  private static Exception DirectoryExists()
  {
    return PythonExceptions.CreateThrowable(PythonNT.WindowsError, (object) 183, (object) "directory already exists");
  }

  [PythonType]
  [DontMapIEnumerableToIter]
  public class stat_result : 
    IList,
    ICollection,
    IEnumerable,
    IList<object>,
    ICollection<object>,
    IEnumerable<object>
  {
    private readonly object _mode;
    private readonly object _atime;
    private readonly object _mtime;
    private readonly object _ctime;
    private readonly object _dev;
    private readonly object _nlink;
    public const int n_fields = 13;
    public const int n_sequence_fields = 10;
    public const int n_unnamed_fields = 3;

    internal stat_result(int mode)
      : this(mode, BigInteger.Zero, BigInteger.Zero, BigInteger.Zero, BigInteger.Zero)
    {
      this._mode = (object) mode;
    }

    internal stat_result(Stat stat, int? mode = null)
    {
      this._mode = (object) (int) ((object) mode ?? (object) stat.st_mode);
      this.st_ino = (object) stat.st_ino;
      this._dev = (object) stat.st_dev;
      this._nlink = (object) stat.st_nlink;
      this.st_uid = (object) stat.st_uid;
      this.st_gid = (object) stat.st_gid;
      this.st_size = (object) stat.st_size;
      this.st_atime = this._atime = (object) stat.st_atime;
      this.st_mtime = this._mtime = (object) stat.st_mtime;
      this.st_ctime = this._ctime = (object) stat.st_ctime;
    }

    internal stat_result(
      int mode,
      BigInteger size,
      BigInteger st_atime,
      BigInteger st_mtime,
      BigInteger st_ctime)
    {
      this._mode = (object) mode;
      this.st_size = (object) size;
      this.st_atime = this._atime = PythonNT.stat_result.TryShrinkToInt((object) st_atime);
      this.st_mtime = this._mtime = PythonNT.stat_result.TryShrinkToInt((object) st_mtime);
      this.st_ctime = this._ctime = PythonNT.stat_result.TryShrinkToInt((object) st_ctime);
      this.st_ino = this._dev = this._nlink = this.st_uid = this.st_gid = ScriptingRuntimeHelpers.Int32ToObject(0);
    }

    public stat_result(CodeContext context, IList statResult, PythonDictionary dict = null)
    {
      this._mode = statResult.Count >= 10 ? statResult[0] : throw PythonOps.TypeError("stat_result() takes an at least 10-sequence ({0}-sequence given)", (object) statResult.Count);
      this.st_ino = statResult[1];
      this._dev = statResult[2];
      this._nlink = statResult[3];
      this.st_uid = statResult[4];
      this.st_gid = statResult[5];
      this.st_size = statResult[6];
      this._atime = statResult[7];
      this._mtime = statResult[8];
      this._ctime = statResult[9];
      object dictTime;
      this.st_atime = statResult.Count < 11 ? (!PythonNT.stat_result.TryGetDictValue(dict, nameof (st_atime), out dictTime) ? PythonNT.stat_result.TryShrinkToInt(this._atime) : dictTime) : PythonNT.stat_result.TryShrinkToInt(statResult[10]);
      this.st_mtime = statResult.Count < 12 ? (!PythonNT.stat_result.TryGetDictValue(dict, nameof (st_mtime), out dictTime) ? PythonNT.stat_result.TryShrinkToInt(this._mtime) : dictTime) : PythonNT.stat_result.TryShrinkToInt(statResult[11]);
      if (statResult.Count >= 13)
        this.st_ctime = PythonNT.stat_result.TryShrinkToInt(statResult[12]);
      else if (PythonNT.stat_result.TryGetDictValue(dict, nameof (st_ctime), out dictTime))
        this.st_ctime = dictTime;
      else
        this.st_ctime = PythonNT.stat_result.TryShrinkToInt(this._ctime);
    }

    private static bool TryGetDictValue(PythonDictionary dict, string name, out object dictTime)
    {
      if (dict != null && dict.TryGetValue((object) name, out dictTime))
      {
        dictTime = PythonNT.stat_result.TryShrinkToInt(dictTime);
        return true;
      }
      dictTime = (object) null;
      return false;
    }

    private static object TryShrinkToInt(object value)
    {
      return !(value is BigInteger x) ? value : BigIntegerOps.__int__(x);
    }

    public object st_atime { get; }

    public object st_ctime { get; }

    public object st_mtime { get; }

    public object st_dev => PythonNT.stat_result.TryShrinkToInt(this._dev);

    public object st_gid { get; }

    public object st_ino { get; }

    public object st_mode => PythonNT.stat_result.TryShrinkToInt(this._mode);

    public object st_nlink => PythonNT.stat_result.TryShrinkToInt(this._nlink);

    public object st_size { get; }

    public object st_uid { get; }

    public static PythonTuple operator +(PythonNT.stat_result stat, object tuple)
    {
      if (!(tuple is PythonTuple pythonTuple))
        throw PythonOps.TypeError("can only concatenate tuple (not \"{0}\") to tuple", (object) PythonTypeOps.GetName(tuple));
      return stat.MakeTuple() + pythonTuple;
    }

    public static bool operator >(PythonNT.stat_result stat, [NotNull] PythonNT.stat_result o)
    {
      return stat.MakeTuple() > PythonTuple.Make((object) o);
    }

    public static bool operator <(PythonNT.stat_result stat, [NotNull] PythonNT.stat_result o)
    {
      return stat.MakeTuple() < PythonTuple.Make((object) o);
    }

    public static bool operator >=(PythonNT.stat_result stat, [NotNull] PythonNT.stat_result o)
    {
      return stat.MakeTuple() >= PythonTuple.Make((object) o);
    }

    public static bool operator <=(PythonNT.stat_result stat, [NotNull] PythonNT.stat_result o)
    {
      return stat.MakeTuple() <= PythonTuple.Make((object) o);
    }

    public static bool operator >(PythonNT.stat_result stat, object o) => true;

    public static bool operator <(PythonNT.stat_result stat, object o) => false;

    public static bool operator >=(PythonNT.stat_result stat, object o) => true;

    public static bool operator <=(PythonNT.stat_result stat, object o) => false;

    public static PythonTuple operator *(PythonNT.stat_result stat, int size)
    {
      return stat.MakeTuple() * size;
    }

    public static PythonTuple operator *(int size, PythonNT.stat_result stat)
    {
      return stat.MakeTuple() * size;
    }

    public override string ToString()
    {
      return string.Format("nt.stat_result(st_mode={0}, st_ino={1}, st_dev={2}, st_nlink={3}, st_uid={4}, st_gid={5}, st_size={6}, st_atime={7}, st_mtime={8}, st_ctime={9})", this.MakeTuple().ToArray());
    }

    public string __repr__() => this.ToString();

    public PythonTuple __reduce__()
    {
      return PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonTypeFromType(typeof (PythonNT.stat_result)), (object) PythonTuple.MakeTuple((object) this.MakeTuple(), (object) new PythonDictionary(3)
      {
        [(object) "st_atime"] = this.st_atime,
        [(object) "st_ctime"] = this.st_ctime,
        [(object) "st_mtime"] = this.st_mtime
      }));
    }

    public object this[int index] => this.MakeTuple()[index];

    public object this[IronPython.Runtime.Slice slice] => this.MakeTuple()[slice];

    public object __getslice__(int start, int stop) => this.MakeTuple().__getslice__(start, stop);

    public int __len__() => this.MakeTuple().__len__();

    public bool __contains__(object item) => this.MakeTuple().Contains(item);

    private PythonTuple MakeTuple()
    {
      return PythonTuple.MakeTuple(this.st_mode, this.st_ino, this.st_dev, this.st_nlink, this.st_uid, this.st_gid, this.st_size, this._atime, this._mtime, this._ctime);
    }

    public override bool Equals(object obj)
    {
      return obj is PythonNT.stat_result ? this.MakeTuple().Equals((object) ((PythonNT.stat_result) obj).MakeTuple()) : this.MakeTuple().Equals(obj);
    }

    public override int GetHashCode() => this.MakeTuple().GetHashCode();

    int IList<object>.IndexOf(object item) => this.MakeTuple().IndexOf(item);

    void IList<object>.Insert(int index, object item) => throw new InvalidOperationException();

    void IList<object>.RemoveAt(int index) => throw new InvalidOperationException();

    object IList<object>.this[int index]
    {
      get => this.MakeTuple()[index];
      set => throw new InvalidOperationException();
    }

    void ICollection<object>.Add(object item) => throw new InvalidOperationException();

    void ICollection<object>.Clear() => throw new InvalidOperationException();

    bool ICollection<object>.Contains(object item) => this.__contains__(item);

    void ICollection<object>.CopyTo(object[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    int ICollection<object>.Count => this.__len__();

    bool ICollection<object>.IsReadOnly => true;

    bool ICollection<object>.Remove(object item) => throw new InvalidOperationException();

    IEnumerator<object> IEnumerable<object>.GetEnumerator()
    {
      foreach (object obj in this.MakeTuple())
        yield return obj;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      foreach (object obj in this.MakeTuple())
        yield return obj;
    }

    int IList.Add(object value) => throw new InvalidOperationException();

    void IList.Clear() => throw new InvalidOperationException();

    bool IList.Contains(object value) => this.__contains__(value);

    int IList.IndexOf(object value) => this.MakeTuple().IndexOf(value);

    void IList.Insert(int index, object value) => throw new InvalidOperationException();

    bool IList.IsFixedSize => true;

    bool IList.IsReadOnly => true;

    void IList.Remove(object value) => throw new InvalidOperationException();

    void IList.RemoveAt(int index) => throw new InvalidOperationException();

    object IList.this[int index]
    {
      get => this.MakeTuple()[index];
      set => throw new InvalidOperationException();
    }

    void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();

    int ICollection.Count => this.__len__();

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => (object) this;
  }

  [PythonType]
  private class POpenFile : PythonFile
  {
    private Process _process;

    internal POpenFile(
      CodeContext context,
      string command,
      Process process,
      Stream stream,
      string mode)
      : base(context.LanguageContext)
    {
      this.__init__(stream, context.LanguageContext.DefaultEncoding, command, mode);
      this._process = process;
    }

    public override object close()
    {
      base.close();
      return this._process.HasExited && this._process.ExitCode != 0 ? (object) this._process.ExitCode : (object) null;
    }
  }
}
