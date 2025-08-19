// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Opera.OperaDriver
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Remote;
using System;

#nullable disable
namespace OpenQA.Selenium.Opera;

public class OperaDriver : RemoteWebDriver
{
  public static readonly bool AcceptUntrustedCertificates = true;

  public OperaDriver()
    : this(new OperaOptions())
  {
  }

  public OperaDriver(OperaOptions options)
    : this(OperaDriverService.CreateDefaultService(), options, RemoteWebDriver.DefaultCommandTimeout)
  {
  }

  public OperaDriver(string operaDriverDirectory)
    : this(operaDriverDirectory, new OperaOptions())
  {
  }

  public OperaDriver(string operaDriverDirectory, OperaOptions options)
    : this(operaDriverDirectory, options, RemoteWebDriver.DefaultCommandTimeout)
  {
  }

  public OperaDriver(string operaDriverDirectory, OperaOptions options, TimeSpan commandTimeout)
    : this(OperaDriverService.CreateDefaultService(operaDriverDirectory), options, commandTimeout)
  {
  }

  public OperaDriver(OperaDriverService service, OperaOptions options)
    : this(service, options, RemoteWebDriver.DefaultCommandTimeout)
  {
  }

  public OperaDriver(OperaDriverService service, OperaOptions options, TimeSpan commandTimeout)
    : base((ICommandExecutor) new DriverServiceCommandExecutor((DriverService) service, commandTimeout), OperaDriver.ConvertOptionsToCapabilities(options))
  {
  }

  public override IFileDetector FileDetector
  {
    get => base.FileDetector;
    set
    {
    }
  }

  private static ICapabilities ConvertOptionsToCapabilities(OperaOptions options)
  {
    return options != null ? options.ToCapabilities() : throw new ArgumentNullException(nameof (options), "options must not be null");
  }
}
