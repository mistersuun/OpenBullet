// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Firefox.FirefoxBinary
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Firefox.Internal;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading;

#nullable disable
namespace OpenQA.Selenium.Firefox;

public class FirefoxBinary : IDisposable
{
  private const string NoFocusLibraryName = "x_ignore_nofocus.so";
  private Dictionary<string, string> extraEnv = new Dictionary<string, string>();
  private Executable executable;
  private Process process;
  private TimeSpan timeout = TimeSpan.FromSeconds(45.0);
  private bool isDisposed;

  public FirefoxBinary()
    : this((string) null)
  {
  }

  public FirefoxBinary(string pathToFirefoxBinary)
  {
    this.executable = new Executable(pathToFirefoxBinary);
  }

  public TimeSpan Timeout
  {
    get => this.timeout;
    set => this.timeout = value;
  }

  internal Executable BinaryExecutable => this.executable;

  protected static bool IsOnLinux => Platform.CurrentPlatform.IsPlatformType(PlatformType.Linux);

  protected Dictionary<string, string> ExtraEnvironmentVariables => this.extraEnv;

  [SecurityPermission(SecurityAction.Demand)]
  public void StartProfile(FirefoxProfile profile, params string[] commandLineArguments)
  {
    if (profile == null)
      throw new ArgumentNullException(nameof (profile), "profile cannot be null");
    if (commandLineArguments == null)
      commandLineArguments = new string[0];
    this.SetEnvironmentProperty("XRE_PROFILE_PATH", profile.ProfileDirectory);
    this.SetEnvironmentProperty("MOZ_NO_REMOTE", "1");
    this.SetEnvironmentProperty("MOZ_CRASHREPORTER_DISABLE", "1");
    this.SetEnvironmentProperty("NO_EM_RESTART", "1");
    if (FirefoxBinary.IsOnLinux && (profile.EnableNativeEvents || profile.AlwaysLoadNoFocusLibrary))
      this.ModifyLinkLibraryPath(profile);
    StringBuilder stringBuilder = new StringBuilder();
    foreach (string commandLineArgument in commandLineArguments)
      stringBuilder.Append(" ").Append(commandLineArgument);
    this.process = new Process();
    this.process.StartInfo.FileName = this.BinaryExecutable.ExecutablePath;
    this.process.StartInfo.Arguments = stringBuilder.ToString();
    this.process.StartInfo.UseShellExecute = false;
    foreach (string key in this.extraEnv.Keys)
    {
      if (this.process.StartInfo.EnvironmentVariables.ContainsKey(key))
        this.process.StartInfo.EnvironmentVariables[key] = this.extraEnv[key];
      else
        this.process.StartInfo.EnvironmentVariables.Add(key, this.extraEnv[key]);
    }
    this.BinaryExecutable.SetLibraryPath(this.process);
    this.StartFirefoxProcess();
    this.CopeWithTheStrangenessOfTheMac();
  }

  public void SetEnvironmentProperty(string propertyName, string value)
  {
    if (string.IsNullOrEmpty(propertyName) || value == null)
      throw new WebDriverException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "You must set both the property name and value: {0}, {1}", (object) propertyName, (object) value));
    if (this.extraEnv.ContainsKey(propertyName))
      this.extraEnv[propertyName] = value;
    else
      this.extraEnv.Add(propertyName, value);
  }

  [SecurityPermission(SecurityAction.Demand)]
  public void WaitForProcessExit() => this.process.WaitForExit();

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }

  public override string ToString() => $"FirefoxBinary({this.executable.ExecutablePath})";

  [SecurityPermission(SecurityAction.Demand)]
  protected void StartFirefoxProcess() => this.process.Start();

  [SecurityPermission(SecurityAction.Demand)]
  protected virtual void Dispose(bool disposing)
  {
    if (this.isDisposed)
      return;
    if (disposing && this.process != null)
    {
      if (!this.process.HasExited)
        Thread.Sleep(1000);
      if (!this.process.HasExited)
        this.process.Kill();
      this.process.Dispose();
      this.process = (Process) null;
    }
    this.isDisposed = true;
  }

  private static void Sleep(int timeInMilliseconds)
  {
    try
    {
      Thread.Sleep(timeInMilliseconds);
    }
    catch (ThreadInterruptedException ex)
    {
      throw new WebDriverException("Thread was interrupted", (Exception) ex);
    }
  }

  private static string ExtractAndCheck(
    FirefoxProfile profile,
    string noFocusSoName,
    string libraryPath32Bit,
    string libraryPath64Bit)
  {
    List<string> stringList = new List<string>();
    stringList.Add(libraryPath32Bit);
    stringList.Add(libraryPath64Bit);
    StringBuilder stringBuilder = new StringBuilder();
    foreach (string path2 in stringList)
    {
      string str = Path.Combine(profile.ProfileDirectory, path2);
      string path = Path.Combine(str, noFocusSoName);
      string resourceId = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "WebDriver.FirefoxNoFocus.{0}.dll", (object) path2);
      if (ResourceUtilities.IsValidResourceName(resourceId))
      {
        using (Stream resourceStream = ResourceUtilities.GetResourceStream(noFocusSoName, resourceId))
        {
          Directory.CreateDirectory(str);
          using (FileStream fileStream = File.Create(path))
          {
            byte[] buffer = new byte[1000];
            for (int count = resourceStream.Read(buffer, 0, buffer.Length); count > 0; count = resourceStream.Read(buffer, 0, buffer.Length))
              fileStream.Write(buffer, 0, count);
          }
        }
      }
      if (!File.Exists(path))
        throw new WebDriverException($"Could not locate {path2}: native events will not work.");
      stringBuilder.Append(str).Append(Path.PathSeparator);
    }
    return stringBuilder.ToString();
  }

  private void ModifyLinkLibraryPath(FirefoxProfile profile)
  {
    string environmentVariable = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH");
    string andCheck = FirefoxBinary.ExtractAndCheck(profile, "x_ignore_nofocus.so", "x86", "x64");
    if (!string.IsNullOrEmpty(environmentVariable))
      andCheck += environmentVariable;
    this.SetEnvironmentProperty("LD_LIBRARY_PATH", andCheck);
    this.SetEnvironmentProperty("LD_PRELOAD", "x_ignore_nofocus.so");
  }

  [SecurityPermission(SecurityAction.Demand)]
  private void CopeWithTheStrangenessOfTheMac()
  {
    if (!Platform.CurrentPlatform.IsPlatformType(PlatformType.Mac))
      return;
    try
    {
      Thread.Sleep(300);
      if (this.process.ExitCode == 0)
        return;
      Thread.Sleep(10000);
      this.StartFirefoxProcess();
    }
    catch (ThreadStateException ex)
    {
    }
    try
    {
      FirefoxBinary.Sleep(300);
      if (this.process.ExitCode != 0)
      {
        StringBuilder stringBuilder = new StringBuilder("Unable to start firefox cleanly.\n");
        stringBuilder.Append("Exit value: ").Append(this.process.ExitCode.ToString((IFormatProvider) CultureInfo.InvariantCulture)).Append("\n");
        stringBuilder.Append("Ran from: ").Append(this.process.StartInfo.FileName).Append("\n");
        throw new WebDriverException(stringBuilder.ToString());
      }
    }
    catch (ThreadStateException ex)
    {
    }
  }
}
