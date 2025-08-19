// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Safari.SafariDriver
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Remote;
using System;

#nullable disable
namespace OpenQA.Selenium.Safari;

public class SafariDriver : RemoteWebDriver
{
  public SafariDriver()
    : this(new SafariOptions())
  {
  }

  public SafariDriver(SafariOptions options)
    : this(SafariDriverService.CreateDefaultService(), options)
  {
  }

  public SafariDriver(SafariDriverService service)
    : this(service, new SafariOptions())
  {
  }

  public SafariDriver(string safariDriverDirectory)
    : this(safariDriverDirectory, new SafariOptions())
  {
  }

  public SafariDriver(string safariDriverDirectory, SafariOptions options)
    : this(safariDriverDirectory, options, RemoteWebDriver.DefaultCommandTimeout)
  {
  }

  public SafariDriver(string safariDriverDirectory, SafariOptions options, TimeSpan commandTimeout)
    : this(SafariDriverService.CreateDefaultService(safariDriverDirectory), options, commandTimeout)
  {
  }

  public SafariDriver(SafariDriverService service, SafariOptions options)
    : this(service, options, RemoteWebDriver.DefaultCommandTimeout)
  {
  }

  public SafariDriver(SafariDriverService service, SafariOptions options, TimeSpan commandTimeout)
    : base((ICommandExecutor) new DriverServiceCommandExecutor((DriverService) service, commandTimeout), SafariDriver.ConvertOptionsToCapabilities(options))
  {
  }

  public override IFileDetector FileDetector
  {
    get => base.FileDetector;
    set
    {
    }
  }

  private static ICapabilities ConvertOptionsToCapabilities(SafariOptions options)
  {
    return options != null ? options.ToCapabilities() : throw new ArgumentNullException(nameof (options), "options must not be null");
  }
}
