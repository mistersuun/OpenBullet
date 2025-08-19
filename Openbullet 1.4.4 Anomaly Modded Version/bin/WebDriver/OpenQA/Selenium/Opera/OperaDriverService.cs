// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Opera.OperaDriverService
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using System;
using System.Globalization;
using System.Text;

#nullable disable
namespace OpenQA.Selenium.Opera;

public sealed class OperaDriverService : DriverService
{
  private const string OperaDriverServiceFileName = "operadriver.exe";
  private static readonly Uri OperaDriverDownloadUrl = new Uri("https://github.com/operasoftware/operachromiumdriver/releases");
  private string logPath = string.Empty;
  private string urlPathPrefix = string.Empty;
  private string portServerAddress = string.Empty;
  private int adbPort = -1;
  private bool enableVerboseLogging;

  private OperaDriverService(string executablePath, string executableFileName, int port)
    : base(executablePath, port, executableFileName, OperaDriverService.OperaDriverDownloadUrl)
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
      return stringBuilder.ToString();
    }
  }

  public static OperaDriverService CreateDefaultService()
  {
    return OperaDriverService.CreateDefaultService(DriverService.FindDriverServiceExecutable("operadriver.exe", OperaDriverService.OperaDriverDownloadUrl));
  }

  public static OperaDriverService CreateDefaultService(string driverPath)
  {
    return OperaDriverService.CreateDefaultService(driverPath, "operadriver.exe");
  }

  public static OperaDriverService CreateDefaultService(
    string driverPath,
    string driverExecutableFileName)
  {
    return new OperaDriverService(driverPath, driverExecutableFileName, PortUtilities.FindFreePort());
  }
}
