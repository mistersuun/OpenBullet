// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Firefox.Internal.Executable
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Microsoft.Win32;
using OpenQA.Selenium.Internal;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;

#nullable disable
namespace OpenQA.Selenium.Firefox.Internal;

internal class Executable
{
  private readonly string binaryInDefaultLocationForPlatform;
  private string binaryLocation;

  public Executable(string userSpecifiedBinaryPath)
  {
    if (!string.IsNullOrEmpty(userSpecifiedBinaryPath))
    {
      this.binaryLocation = File.Exists(userSpecifiedBinaryPath) ? userSpecifiedBinaryPath : throw new WebDriverException("Specified firefox binary location does not exist or is not a real file: " + userSpecifiedBinaryPath);
    }
    else
    {
      this.binaryInDefaultLocationForPlatform = Executable.LocateFirefoxBinaryFromPlatform();
      this.binaryLocation = this.binaryInDefaultLocationForPlatform != null && File.Exists(this.binaryInDefaultLocationForPlatform) ? this.binaryInDefaultLocationForPlatform : throw new WebDriverException("Cannot find Firefox binary in PATH or default install locations. Make sure Firefox is installed. OS appears to be: " + Platform.CurrentPlatform.ToString());
    }
  }

  public string ExecutablePath => this.binaryLocation;

  [SecurityPermission(SecurityAction.Demand)]
  public void SetLibraryPath(Process builder)
  {
    string pathPropertyName = Executable.GetLibraryPathPropertyName();
    StringBuilder stringBuilder = new StringBuilder();
    string environmentVariable = Executable.GetEnvironmentVariable(pathPropertyName, (string) null);
    if (environmentVariable != null)
      stringBuilder.Append(environmentVariable).Append(Path.PathSeparator);
    if (builder.StartInfo.EnvironmentVariables.ContainsKey(pathPropertyName))
      stringBuilder.Append(builder.StartInfo.EnvironmentVariables[pathPropertyName]).Append(Path.PathSeparator);
    string fullPath = Path.GetFullPath(this.binaryLocation);
    if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Mac) && Platform.CurrentPlatform.MinorVersion > 5)
      stringBuilder.Append(Path.PathSeparator);
    else
      stringBuilder.Insert(0, Path.PathSeparator).Insert(0, fullPath);
    if (builder.StartInfo.EnvironmentVariables.ContainsKey(pathPropertyName))
      builder.StartInfo.EnvironmentVariables[pathPropertyName] = stringBuilder.ToString();
    else
      builder.StartInfo.EnvironmentVariables.Add(pathPropertyName, stringBuilder.ToString());
  }

  [SecurityPermission(SecurityAction.Demand)]
  private static string LocateFirefoxBinaryFromPlatform()
  {
    string empty = string.Empty;
    string path;
    if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows))
    {
      string name = "SOFTWARE\\Mozilla\\Mozilla Firefox";
      RegistryKey mozillaKey = Registry.LocalMachine.OpenSubKey(name) ?? Registry.CurrentUser.OpenSubKey(name);
      if (mozillaKey != null)
        path = Executable.GetExecutablePathUsingRegistry(mozillaKey);
      else
        path = Executable.GetExecutablePathUsingDefaultInstallLocations(new string[2]
        {
          Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Mozilla Firefox"),
          Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + " (x86)", "Mozilla Firefox")
        }, "Firefox.exe");
    }
    else
    {
      path = Executable.GetExecutablePathUsingDefaultInstallLocations(new string[2]
      {
        "/Applications/Firefox.app/Contents/MacOS",
        string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Users/{0}/Applications/Firefox.app/Contents/MacOS", (object) Environment.UserName)
      }, "firefox-bin");
      if (string.IsNullOrEmpty(path))
      {
        using (Process process = new Process())
        {
          process.StartInfo.FileName = "which";
          process.StartInfo.Arguments = "firefox";
          process.StartInfo.CreateNoWindow = true;
          process.StartInfo.RedirectStandardOutput = true;
          process.StartInfo.UseShellExecute = false;
          process.Start();
          process.WaitForExit();
          path = process.StandardOutput.ReadToEnd().Trim();
        }
      }
    }
    if (path != null && File.Exists(path))
      return path;
    return Executable.FindBinary(new string[2]
    {
      "firefox3",
      "firefox"
    });
  }

  private static string GetExecutablePathUsingRegistry(RegistryKey mozillaKey)
  {
    string str = (string) mozillaKey.GetValue("CurrentVersion");
    if (string.IsNullOrEmpty(str))
      throw new WebDriverException("Unable to determine the current version of FireFox using the registry, please make sure you have installed FireFox correctly");
    string path = (string) (mozillaKey.OpenSubKey(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\Main", (object) str)) ?? throw new WebDriverException("Unable to determine the current version of FireFox using the registry, please make sure you have installed FireFox correctly")).GetValue("PathToExe");
    return File.Exists(path) ? path : throw new WebDriverException("FireFox executable listed in the registry does not exist, please make sure you have installed FireFox correctly");
  }

  private static string GetExecutablePathUsingDefaultInstallLocations(
    string[] defaultInstallLocations,
    string exeName)
  {
    foreach (string defaultInstallLocation in defaultInstallLocations)
    {
      string path = Path.Combine(defaultInstallLocation, exeName);
      if (File.Exists(path))
        return path;
    }
    return (string) null;
  }

  private static string GetEnvironmentVariable(string name, string defaultValue)
  {
    string environmentVariable = Environment.GetEnvironmentVariable(name);
    if (string.IsNullOrEmpty(environmentVariable))
      environmentVariable = defaultValue;
    return environmentVariable;
  }

  private static string GetLibraryPathPropertyName()
  {
    string pathPropertyName = "LD_LIBRARY_PATH";
    if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows))
      pathPropertyName = "PATH";
    else if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Mac))
      pathPropertyName = "DYLD_LIBRARY_PATH";
    return pathPropertyName;
  }

  private static string FindBinary(string[] binaryNames)
  {
    foreach (string binaryName in binaryNames)
    {
      if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows))
        binaryName += ".exe";
      string file = FileUtilities.FindFile(binaryName);
      if (!string.IsNullOrEmpty(file))
        return Path.Combine(file, binaryName);
    }
    return (string) null;
  }
}
