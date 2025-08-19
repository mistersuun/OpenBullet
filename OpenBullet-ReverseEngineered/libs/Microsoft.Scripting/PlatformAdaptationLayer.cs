// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.PlatformAdaptationLayer
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Scripting;

[Serializable]
public class PlatformAdaptationLayer
{
  public static readonly PlatformAdaptationLayer Default = new PlatformAdaptationLayer();
  [Obsolete]
  public static readonly bool IsCompactFramework = false;

  public virtual Assembly LoadAssembly(string name) => Assembly.Load(name);

  public virtual Assembly LoadAssemblyFromPath(string path) => Assembly.LoadFile(path);

  public virtual void TerminateScriptExecution(int exitCode) => Environment.Exit(exitCode);

  public virtual bool IsSingleRootFileSystem
  {
    get
    {
      return Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX;
    }
  }

  public virtual StringComparer PathComparer
  {
    get
    {
      return Environment.OSVersion.Platform != PlatformID.Unix ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
    }
  }

  public virtual bool FileExists(string path) => File.Exists(path);

  public virtual bool DirectoryExists(string path) => Directory.Exists(path);

  public virtual Stream OpenFileStream(
    string path,
    FileMode mode = FileMode.OpenOrCreate,
    FileAccess access = FileAccess.ReadWrite,
    FileShare share = FileShare.Read,
    int bufferSize = 8192 /*0x2000*/)
  {
    return string.Equals("nul", path, StringComparison.InvariantCultureIgnoreCase) ? Stream.Null : (Stream) new FileStream(path, mode, access, share, bufferSize);
  }

  public virtual Stream OpenInputFileStream(
    string path,
    FileMode mode = FileMode.Open,
    FileAccess access = FileAccess.Read,
    FileShare share = FileShare.Read,
    int bufferSize = 8192 /*0x2000*/)
  {
    return this.OpenFileStream(path, mode, access, share, bufferSize);
  }

  public virtual Stream OpenOutputFileStream(string path)
  {
    return this.OpenFileStream(path, FileMode.Create, FileAccess.Write);
  }

  public virtual void DeleteFile(string path, bool deleteReadOnly)
  {
    FileInfo fileInfo = new FileInfo(path);
    if (deleteReadOnly && fileInfo.IsReadOnly)
      fileInfo.IsReadOnly = false;
    fileInfo.Delete();
  }

  public string[] GetFiles(string path, string searchPattern)
  {
    return this.GetFileSystemEntries(path, searchPattern, true, false);
  }

  public string[] GetDirectories(string path, string searchPattern)
  {
    return this.GetFileSystemEntries(path, searchPattern, false, true);
  }

  public string[] GetFileSystemEntries(string path, string searchPattern)
  {
    return this.GetFileSystemEntries(path, searchPattern, true, true);
  }

  public virtual string[] GetFileSystemEntries(
    string path,
    string searchPattern,
    bool includeFiles,
    bool includeDirectories)
  {
    if (includeFiles & includeDirectories)
      return Directory.GetFileSystemEntries(path, searchPattern);
    if (includeFiles)
      return Directory.GetFiles(path, searchPattern);
    return includeDirectories ? Directory.GetDirectories(path, searchPattern) : ArrayUtils.EmptyStrings;
  }

  public virtual string GetFullPath(string path)
  {
    try
    {
      return Path.GetFullPath(path);
    }
    catch (Exception ex)
    {
      throw Error.InvalidPath();
    }
  }

  public virtual string CombinePaths(string path1, string path2) => Path.Combine(path1, path2);

  public virtual string GetFileName(string path) => Path.GetFileName(path);

  public virtual string GetDirectoryName(string path) => Path.GetDirectoryName(path);

  public virtual string GetExtension(string path) => Path.GetExtension(path);

  public virtual string GetFileNameWithoutExtension(string path)
  {
    return Path.GetFileNameWithoutExtension(path);
  }

  public virtual bool IsAbsolutePath(string path)
  {
    if (this.IsSingleRootFileSystem)
      return Path.IsPathRooted(path);
    string pathRoot = Path.GetPathRoot(path);
    return pathRoot.EndsWith(":\\") || pathRoot.EndsWith(":/");
  }

  public virtual string CurrentDirectory
  {
    get => Directory.GetCurrentDirectory();
    set => Directory.SetCurrentDirectory(value);
  }

  public virtual void CreateDirectory(string path) => Directory.CreateDirectory(path);

  public virtual void DeleteDirectory(string path, bool recursive)
  {
    Directory.Delete(path, recursive);
  }

  public virtual void MoveFileSystemEntry(string sourcePath, string destinationPath)
  {
    Directory.Move(sourcePath, destinationPath);
  }

  public virtual string GetEnvironmentVariable(string key)
  {
    return Environment.GetEnvironmentVariable(key);
  }

  public virtual void SetEnvironmentVariable(string key, string value)
  {
    if (value != null && value.Length == 0)
      PlatformAdaptationLayer.SetEmptyEnvironmentVariable(key);
    else
      Environment.SetEnvironmentVariable(key, value);
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static void SetEmptyEnvironmentVariable(string key)
  {
    if (!Microsoft.Scripting.Utils.NativeMethods.SetEnvironmentVariable(key, string.Empty))
      throw new ExternalException("SetEnvironmentVariable failed", Marshal.GetLastWin32Error());
  }

  public virtual Dictionary<string, string> GetEnvironmentVariables()
  {
    Dictionary<string, string> environmentVariables = new Dictionary<string, string>();
    foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
      environmentVariables.Add((string) environmentVariable.Key, (string) environmentVariable.Value);
    return environmentVariables;
  }
}
