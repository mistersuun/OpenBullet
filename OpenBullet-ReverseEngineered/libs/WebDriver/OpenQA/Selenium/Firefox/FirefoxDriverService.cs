// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Firefox.FirefoxDriverService
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using System;
using System.Globalization;
using System.Text;

#nullable disable
namespace OpenQA.Selenium.Firefox;

public sealed class FirefoxDriverService : DriverService
{
  private const string DefaultFirefoxDriverServiceFileName = "geckodriver";
  private static readonly Uri FirefoxDriverDownloadUrl = new Uri("https://github.com/mozilla/geckodriver/releases");
  private bool connectToRunningBrowser;
  private bool openBrowserToolbox;
  private int browserCommunicationPort = -1;
  private string browserBinaryPath = string.Empty;
  private string host = string.Empty;
  private FirefoxDriverLogLevel loggingLevel = FirefoxDriverLogLevel.Default;

  private FirefoxDriverService(string executablePath, string executableFileName, int port)
    : base(executablePath, port, executableFileName, FirefoxDriverService.FirefoxDriverDownloadUrl)
  {
  }

  public string FirefoxBinaryPath
  {
    get => this.browserBinaryPath;
    set => this.browserBinaryPath = value;
  }

  public int BrowserCommunicationPort
  {
    get => this.browserCommunicationPort;
    set => this.browserCommunicationPort = value;
  }

  public string Host
  {
    get => this.host;
    set => this.host = value;
  }

  public bool ConnectToRunningBrowser
  {
    get => this.connectToRunningBrowser;
    set => this.connectToRunningBrowser = value;
  }

  public bool OpenBrowserToolbox
  {
    get => this.openBrowserToolbox;
    set => this.openBrowserToolbox = value;
  }

  protected override TimeSpan InitializationTimeout => TimeSpan.FromSeconds(2.0);

  protected override TimeSpan TerminationTimeout => TimeSpan.FromMilliseconds(100.0);

  protected override bool HasShutdown => false;

  protected override string CommandLineArguments
  {
    get
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.connectToRunningBrowser)
        stringBuilder.Append(" --connect-existing");
      if (this.browserCommunicationPort > 0)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " --marionette-port {0}", (object) this.browserCommunicationPort);
      if (this.Port > 0)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " --port {0}", (object) this.Port);
      if (!string.IsNullOrEmpty(this.browserBinaryPath))
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " --binary \"{0}\"", (object) this.browserBinaryPath);
      if (!string.IsNullOrEmpty(this.host))
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " --host \"{0}\"", (object) this.host);
      if (this.loggingLevel != FirefoxDriverLogLevel.Default)
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " --log {0}", (object) this.loggingLevel.ToString().ToLowerInvariant()));
      if (this.openBrowserToolbox)
        stringBuilder.Append(" --jsdebugger");
      return stringBuilder.ToString().Trim();
    }
  }

  public static FirefoxDriverService CreateDefaultService()
  {
    return FirefoxDriverService.CreateDefaultService(DriverService.FindDriverServiceExecutable(FirefoxDriverService.FirefoxDriverServiceFileName(), FirefoxDriverService.FirefoxDriverDownloadUrl));
  }

  public static FirefoxDriverService CreateDefaultService(string driverPath)
  {
    return FirefoxDriverService.CreateDefaultService(driverPath, FirefoxDriverService.FirefoxDriverServiceFileName());
  }

  public static FirefoxDriverService CreateDefaultService(
    string driverPath,
    string driverExecutableFileName)
  {
    return new FirefoxDriverService(driverPath, driverExecutableFileName, PortUtilities.FindFreePort());
  }

  private static string FirefoxDriverServiceFileName()
  {
    string str = "geckodriver";
    switch (Environment.OSVersion.Platform)
    {
      case PlatformID.Win32S:
      case PlatformID.Win32Windows:
      case PlatformID.Win32NT:
      case PlatformID.WinCE:
        str += ".exe";
        goto case PlatformID.Unix;
      case PlatformID.Unix:
      case PlatformID.MacOSX:
        return str;
      default:
        if (Environment.OSVersion.Platform != (PlatformID) 128 /*0x80*/)
          throw new WebDriverException("Unsupported platform: " + (object) Environment.OSVersion.Platform);
        goto case PlatformID.Unix;
    }
  }
}
