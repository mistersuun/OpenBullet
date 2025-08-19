// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.IE.InternetExplorerDriverService
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using System;
using System.Globalization;
using System.Text;

#nullable disable
namespace OpenQA.Selenium.IE;

public sealed class InternetExplorerDriverService : DriverService
{
  private const string InternetExplorerDriverServiceFileName = "IEDriverServer.exe";
  private static readonly Uri InternetExplorerDriverDownloadUrl = new Uri("http://selenium-release.storage.googleapis.com/index.html");
  private InternetExplorerDriverLogLevel loggingLevel = InternetExplorerDriverLogLevel.Fatal;
  private string host = string.Empty;
  private string logFile = string.Empty;
  private string libraryExtractionPath = string.Empty;
  private string whitelistedIpAddresses = string.Empty;

  private InternetExplorerDriverService(string executablePath, string executableFileName, int port)
    : base(executablePath, port, executableFileName, InternetExplorerDriverService.InternetExplorerDriverDownloadUrl)
  {
  }

  public string Host
  {
    get => this.host;
    set => this.host = value;
  }

  public string LogFile
  {
    get => this.logFile;
    set => this.logFile = value;
  }

  public InternetExplorerDriverLogLevel LoggingLevel
  {
    get => this.loggingLevel;
    set => this.loggingLevel = value;
  }

  public string LibraryExtractionPath
  {
    get => this.libraryExtractionPath;
    set => this.libraryExtractionPath = value;
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
      if (!string.IsNullOrEmpty(this.host))
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " -host={0}", (object) this.host));
      if (!string.IsNullOrEmpty(this.logFile))
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " -log-file={0}", (object) this.logFile));
      if (!string.IsNullOrEmpty(this.libraryExtractionPath))
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " -extract-path={0}", (object) this.libraryExtractionPath));
      if (this.loggingLevel != InternetExplorerDriverLogLevel.Fatal)
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " -log-level={0}", (object) this.loggingLevel.ToString().ToUpperInvariant()));
      if (!string.IsNullOrEmpty(this.whitelistedIpAddresses))
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " -whitelisted-ips={0}", (object) this.whitelistedIpAddresses));
      if (this.SuppressInitialDiagnosticInformation)
        stringBuilder.Append(" -silent");
      return stringBuilder.ToString();
    }
  }

  public static InternetExplorerDriverService CreateDefaultService()
  {
    return InternetExplorerDriverService.CreateDefaultService(DriverService.FindDriverServiceExecutable("IEDriverServer.exe", InternetExplorerDriverService.InternetExplorerDriverDownloadUrl));
  }

  public static InternetExplorerDriverService CreateDefaultService(string driverPath)
  {
    return InternetExplorerDriverService.CreateDefaultService(driverPath, "IEDriverServer.exe");
  }

  public static InternetExplorerDriverService CreateDefaultService(
    string driverPath,
    string driverExecutableFileName)
  {
    return new InternetExplorerDriverService(driverPath, driverExecutableFileName, PortUtilities.FindFreePort());
  }
}
