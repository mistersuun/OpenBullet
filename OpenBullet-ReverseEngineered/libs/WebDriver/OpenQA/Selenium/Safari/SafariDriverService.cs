// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Safari.SafariDriverService
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using System;
using System.Net;
using System.Text;

#nullable disable
namespace OpenQA.Selenium.Safari;

public sealed class SafariDriverService : DriverService
{
  private const string DefaultSafariDriverServiceExecutableName = "safaridriver";
  private const string DefaultSafariDriverServiceExecutablePath = "/usr/bin";
  private static readonly Uri SafariDriverDownloadUrl = new Uri("http://apple.com");
  private bool useLegacyProtocol;

  private SafariDriverService(string executablePath, string executableFileName, int port)
    : base(executablePath, port, executableFileName, SafariDriverService.SafariDriverDownloadUrl)
  {
  }

  public bool UseLegacyProtocol
  {
    get => this.useLegacyProtocol;
    set => this.useLegacyProtocol = value;
  }

  protected override string CommandLineArguments
  {
    get
    {
      StringBuilder stringBuilder = new StringBuilder(base.CommandLineArguments);
      if (this.useLegacyProtocol)
        stringBuilder.Append(" --legacy");
      return stringBuilder.ToString();
    }
  }

  protected override TimeSpan TerminationTimeout => TimeSpan.FromMilliseconds(100.0);

  protected override bool HasShutdown => false;

  protected override bool IsInitialized
  {
    get
    {
      bool isInitialized = false;
      try
      {
        HttpWebRequest httpWebRequest = WebRequest.Create(new Uri(this.ServiceUrl, new Uri("/session/FakeSessionIdForPollingPurposes", UriKind.Relative))) as HttpWebRequest;
        httpWebRequest.KeepAlive = false;
        httpWebRequest.Timeout = 5000;
        httpWebRequest.Method = "DELETE";
        HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse;
        isInitialized = response.StatusCode == HttpStatusCode.OK && response.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase);
        response.Close();
      }
      catch (WebException ex)
      {
        if (ex.Response is HttpWebResponse response)
          isInitialized = response.StatusCode == HttpStatusCode.InternalServerError && response.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) || response.StatusCode == HttpStatusCode.NotFound;
        else
          Console.WriteLine(ex.Message);
      }
      return isInitialized;
    }
  }

  public static SafariDriverService CreateDefaultService()
  {
    return SafariDriverService.CreateDefaultService("/usr/bin");
  }

  public static SafariDriverService CreateDefaultService(string driverPath)
  {
    return SafariDriverService.CreateDefaultService(driverPath, "safaridriver");
  }

  public static SafariDriverService CreateDefaultService(
    string driverPath,
    string driverExecutableFileName)
  {
    return new SafariDriverService(driverPath, driverExecutableFileName, PortUtilities.FindFreePort());
  }
}
