// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Chrome.ChromeDriverService
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using System;
using System.Globalization;
using System.Text;

#nullable disable
namespace OpenQA.Selenium.Chrome;

public sealed class ChromeDriverService : DriverService
{
  private const string DefaultChromeDriverServiceExecutableName = "chromedriver";
  private static readonly Uri ChromeDriverDownloadUrl = new Uri("http://chromedriver.storage.googleapis.com/index.html");
  private string logPath = string.Empty;
  private string urlPathPrefix = string.Empty;
  private string portServerAddress = string.Empty;
  private string whitelistedIpAddresses = string.Empty;
  private int adbPort = -1;
  private bool enableVerboseLogging;

  private ChromeDriverService(string executablePath, string executableFileName, int port)
    : base(executablePath, port, executableFileName, ChromeDriverService.ChromeDriverDownloadUrl)
  {
  }

  public string LogPath
  {
    get => this.logPath;
    set => this.logPath = value;
  }

  public string UrlPathPrefix
  {
    get => this.urlPathPrefix;
    set => this.urlPathPrefix = value;
  }

  public string PortServerAddress
  {
    get => this.portServerAddress;
    set => this.portServerAddress = value;
  }

  public int AndroidDebugBridgePort
  {
    get => this.adbPort;
    set => this.adbPort = value;
  }

  public bool EnableVerboseLogging
  {
    get => this.enableVerboseLogging;
    set => this.enableVerboseLogging = value;
  }

  public string WhitelistedIPAddresses
  {
    get => this.whitelistedIpAddresses;
    set => this.whitelistedIpAddresses = value;
  }

  protected override string CommandLineArguments
  {
    get
    {
      StringBuilder stringBuilder = new StringBuilder(base.CommandLineArguments);
      if (this.adbPort > 0)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " --adb-port={0}", (object) this.adbPort);
      if (this.SuppressInitialDiagnosticInformation)
        stringBuilder.Append(" --silent");
      if (this.enableVerboseLogging)
        stringBuilder.Append(" --verbose");
      if (!string.IsNullOrEmpty(this.logPath))
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " --log-path={0}", (object) this.logPath);
      if (!string.IsNullOrEmpty(this.urlPathPrefix))
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " --url-base={0}", (object) this.urlPathPrefix);
      if (!string.IsNullOrEmpty(this.portServerAddress))
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " --port-server={0}", (object) this.portServerAddress);
      if (!string.IsNullOrEmpty(this.whitelistedIpAddresses))
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " -whitelisted-ips={0}", (object) this.whitelistedIpAddresses));
      return stringBuilder.ToString();
    }
  }

  public static ChromeDriverService CreateDefaultService()
  {
    return ChromeDriverService.CreateDefaultService(DriverService.FindDriverServiceExecutable(ChromeDriverService.ChromeDriverServiceFileName(), ChromeDriverService.ChromeDriverDownloadUrl));
  }

  public static ChromeDriverService CreateDefaultService(string driverPath)
  {
    return ChromeDriverService.CreateDefaultService(driverPath, ChromeDriverService.ChromeDriverServiceFileName());
  }

  public static ChromeDriverService CreateDefaultService(
    string driverPath,
    string driverExecutableFileName)
  {
    return new ChromeDriverService(driverPath, driverExecutableFileName, PortUtilities.FindFreePort());
  }

  private static string ChromeDriverServiceFileName()
  {
    string str = "chromedriver";
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
