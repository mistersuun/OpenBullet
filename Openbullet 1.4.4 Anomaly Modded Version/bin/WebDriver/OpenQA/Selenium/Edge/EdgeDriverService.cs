// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Edge.EdgeDriverService
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using System;
using System.Globalization;
using System.Text;

#nullable disable
namespace OpenQA.Selenium.Edge;

public sealed class EdgeDriverService : DriverService
{
  private const string MicrosoftWebDriverServiceFileName = "MicrosoftWebDriver.exe";
  private static readonly Uri MicrosoftWebDriverDownloadUrl = new Uri("http://go.microsoft.com/fwlink/?LinkId=619687");
  private string host;
  private string package;
  private bool useVerboseLogging;
  private bool? useSpecCompliantProtocol;

  private EdgeDriverService(string executablePath, string executableFileName, int port)
    : base(executablePath, port, executableFileName, EdgeDriverService.MicrosoftWebDriverDownloadUrl)
  {
  }

  public string Host
  {
    get => this.host;
    set => this.host = value;
  }

  public string Package
  {
    get => this.package;
    set => this.package = value;
  }

  public bool UseVerboseLogging
  {
    get => this.useVerboseLogging;
    set => this.useVerboseLogging = value;
  }

  public bool? UseSpecCompliantProtocol
  {
    get => this.useSpecCompliantProtocol;
    set => this.useSpecCompliantProtocol = value;
  }

  protected override bool HasShutdown
  {
    get
    {
      return this.useSpecCompliantProtocol.HasValue && !this.useSpecCompliantProtocol.Value && base.HasShutdown;
    }
  }

  protected override TimeSpan TerminationTimeout
  {
    get
    {
      return this.useSpecCompliantProtocol.HasValue && !this.useSpecCompliantProtocol.Value ? base.TerminationTimeout : TimeSpan.FromMilliseconds(100.0);
    }
  }

  protected override string CommandLineArguments
  {
    get
    {
      StringBuilder stringBuilder = new StringBuilder(base.CommandLineArguments);
      if (!string.IsNullOrEmpty(this.host))
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " --host={0}", (object) this.host));
      if (!string.IsNullOrEmpty(this.package))
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " --package={0}", (object) this.package));
      if (this.useVerboseLogging)
        stringBuilder.Append(" --verbose");
      if (this.SuppressInitialDiagnosticInformation)
        stringBuilder.Append(" --silent");
      if (this.useSpecCompliantProtocol.HasValue)
      {
        if (this.useSpecCompliantProtocol.Value)
          stringBuilder.Append(" --w3c");
        else
          stringBuilder.Append(" --jwp");
      }
      return stringBuilder.ToString();
    }
  }

  public static EdgeDriverService CreateDefaultService()
  {
    return EdgeDriverService.CreateDefaultService(DriverService.FindDriverServiceExecutable("MicrosoftWebDriver.exe", EdgeDriverService.MicrosoftWebDriverDownloadUrl));
  }

  public static EdgeDriverService CreateDefaultService(string driverPath)
  {
    return EdgeDriverService.CreateDefaultService(driverPath, "MicrosoftWebDriver.exe");
  }

  public static EdgeDriverService CreateDefaultService(
    string driverPath,
    string driverExecutableFileName)
  {
    return EdgeDriverService.CreateDefaultService(driverPath, driverExecutableFileName, PortUtilities.FindFreePort());
  }

  public static EdgeDriverService CreateDefaultService(
    string driverPath,
    string driverExecutableFileName,
    int port)
  {
    return new EdgeDriverService(driverPath, driverExecutableFileName, port);
  }
}
