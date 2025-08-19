// Decompiled with JetBrains decompiler
// Type: InteropDotNet.LibraryLoader
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Tesseract.Internal;

#nullable disable
namespace InteropDotNet;

public sealed class LibraryLoader
{
  private readonly ILibraryLoaderLogic logic;
  private readonly object syncLock = new object();
  private readonly Dictionary<string, IntPtr> loadedAssemblies = new Dictionary<string, IntPtr>();
  private string customSearchPath;
  private static LibraryLoader instance;

  private LibraryLoader(ILibraryLoaderLogic logic) => this.logic = logic;

  public string CustomSearchPath
  {
    get => this.customSearchPath;
    set => this.customSearchPath = value;
  }

  public IntPtr LoadLibrary(string fileName, string platformName = null)
  {
    fileName = this.FixUpLibraryName(fileName);
    lock (this.syncLock)
    {
      if (!this.loadedAssemblies.ContainsKey(fileName))
      {
        if (platformName == null)
          platformName = SystemManager.GetPlatformName();
        Logger.TraceInformation("Current platform: " + platformName);
        IntPtr num = this.CheckCustomSearchPath(fileName, platformName);
        if (num == IntPtr.Zero)
          num = this.CheckExecutingAssemblyDomain(fileName, platformName);
        if (num == IntPtr.Zero)
          num = this.CheckCurrentAppDomain(fileName, platformName);
        if (num == IntPtr.Zero)
          num = this.CheckCurrentAppDomainBin(fileName, platformName);
        if (num == IntPtr.Zero)
          num = this.CheckWorkingDirecotry(fileName, platformName);
        this.loadedAssemblies[fileName] = num != IntPtr.Zero ? num : throw new DllNotFoundException($"Failed to find library \"{fileName}\" for platform {platformName}.");
      }
      return this.loadedAssemblies[fileName];
    }
  }

  private IntPtr CheckCustomSearchPath(string fileName, string platformName)
  {
    string customSearchPath = this.CustomSearchPath;
    if (!string.IsNullOrEmpty(customSearchPath))
    {
      Logger.TraceInformation("Checking custom search location '{0}' for '{1}' on platform {2}.", (object) customSearchPath, (object) fileName, (object) platformName);
      return this.InternalLoadLibrary(customSearchPath, platformName, fileName);
    }
    Logger.TraceInformation("Custom search path is not defined, skipping.");
    return IntPtr.Zero;
  }

  private IntPtr CheckExecutingAssemblyDomain(string fileName, string platformName)
  {
    string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    Logger.TraceInformation("Checking executing application domain location '{0}' for '{1}' on platform {2}.", (object) directoryName, (object) fileName, (object) platformName);
    return this.InternalLoadLibrary(directoryName, platformName, fileName);
  }

  private IntPtr CheckCurrentAppDomain(string fileName, string platformName)
  {
    string fullPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
    Logger.TraceInformation("Checking current application domain location '{0}' for '{1}' on platform {2}.", (object) fullPath, (object) fileName, (object) platformName);
    return this.InternalLoadLibrary(fullPath, platformName, fileName);
  }

  private IntPtr CheckCurrentAppDomainBin(string fileName, string platformName)
  {
    string str = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), "bin");
    if (Directory.Exists(str))
    {
      Logger.TraceInformation("Checking current application domain's bin location '{0}' for '{1}' on platform {2}.", (object) str, (object) fileName, (object) platformName);
      return this.InternalLoadLibrary(str, platformName, fileName);
    }
    Logger.TraceInformation("No bin directory exists under the current application domain's location, skipping.");
    return IntPtr.Zero;
  }

  private IntPtr CheckWorkingDirecotry(string fileName, string platformName)
  {
    string fullPath = Path.GetFullPath(Environment.CurrentDirectory);
    Logger.TraceInformation("Checking working directory '{0}' for '{1}' on platform {2}.", (object) fullPath, (object) fileName, (object) platformName);
    return this.InternalLoadLibrary(fullPath, platformName, fileName);
  }

  private IntPtr InternalLoadLibrary(string baseDirectory, string platformName, string fileName)
  {
    string str = Path.Combine(baseDirectory, Path.Combine(platformName, fileName));
    return !File.Exists(str) ? IntPtr.Zero : this.logic.LoadLibrary(str);
  }

  public bool FreeLibrary(string fileName)
  {
    fileName = this.FixUpLibraryName(fileName);
    lock (this.syncLock)
    {
      if (!this.IsLibraryLoaded(fileName))
      {
        Logger.TraceWarning("Failed to free library \"{0}\" because it is not loaded", (object) fileName);
        return false;
      }
      if (!this.logic.FreeLibrary(this.loadedAssemblies[fileName]))
        return false;
      this.loadedAssemblies.Remove(fileName);
      return true;
    }
  }

  public IntPtr GetProcAddress(IntPtr dllHandle, string name)
  {
    return this.logic.GetProcAddress(dllHandle, name);
  }

  public bool IsLibraryLoaded(string fileName)
  {
    fileName = this.FixUpLibraryName(fileName);
    lock (this.syncLock)
      return this.loadedAssemblies.ContainsKey(fileName);
  }

  private string FixUpLibraryName(string fileName) => this.logic.FixUpLibraryName(fileName);

  public static LibraryLoader Instance
  {
    get
    {
      if (LibraryLoader.instance == null)
      {
        switch (SystemManager.GetOperatingSystem())
        {
          case OperatingSystem.Windows:
            Logger.TraceInformation("Current OS: Windows");
            LibraryLoader.instance = new LibraryLoader((ILibraryLoaderLogic) new WindowsLibraryLoaderLogic());
            break;
          case OperatingSystem.Unix:
            Logger.TraceInformation("Current OS: Unix");
            LibraryLoader.instance = new LibraryLoader((ILibraryLoaderLogic) new UnixLibraryLoaderLogic());
            break;
          case OperatingSystem.MacOSX:
            Logger.TraceInformation("Current OS: MacOsX");
            LibraryLoader.instance = new LibraryLoader((ILibraryLoaderLogic) new UnixLibraryLoaderLogic());
            break;
          default:
            throw new Exception("Unsupported operation system");
        }
      }
      return LibraryLoader.instance;
    }
  }
}
