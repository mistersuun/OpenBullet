// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.IE.InternetExplorerDriver
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.IE;

public class InternetExplorerDriver : RemoteWebDriver
{
  public InternetExplorerDriver()
    : this(new InternetExplorerOptions())
  {
  }

  public InternetExplorerDriver(InternetExplorerOptions options)
    : this(InternetExplorerDriverService.CreateDefaultService(), options)
  {
  }

  public InternetExplorerDriver(InternetExplorerDriverService service)
    : this(service, new InternetExplorerOptions())
  {
  }

  public InternetExplorerDriver(string internetExplorerDriverServerDirectory)
    : this(internetExplorerDriverServerDirectory, new InternetExplorerOptions())
  {
  }

  public InternetExplorerDriver(
    string internetExplorerDriverServerDirectory,
    InternetExplorerOptions options)
    : this(internetExplorerDriverServerDirectory, options, RemoteWebDriver.DefaultCommandTimeout)
  {
  }

  public InternetExplorerDriver(
    string internetExplorerDriverServerDirectory,
    InternetExplorerOptions options,
    TimeSpan commandTimeout)
    : this(InternetExplorerDriverService.CreateDefaultService(internetExplorerDriverServerDirectory), options, commandTimeout)
  {
  }

  public InternetExplorerDriver(
    InternetExplorerDriverService service,
    InternetExplorerOptions options)
    : this(service, options, RemoteWebDriver.DefaultCommandTimeout)
  {
  }

  public InternetExplorerDriver(
    InternetExplorerDriverService service,
    InternetExplorerOptions options,
    TimeSpan commandTimeout)
    : base((ICommandExecutor) new DriverServiceCommandExecutor((DriverService) service, commandTimeout), InternetExplorerDriver.ConvertOptionsToCapabilities(options))
  {
  }

  public override IFileDetector FileDetector
  {
    get => base.FileDetector;
    set
    {
    }
  }

  protected override Dictionary<string, object> GetLegacyCapabilitiesDictionary(
    ICapabilities legacyCapabilities)
  {
    Dictionary<string, object> capabilitiesDictionary = new Dictionary<string, object>();
    foreach (KeyValuePair<string, object> capabilities in (legacyCapabilities as IHasCapabilitiesDictionary).CapabilitiesDictionary)
    {
      if (capabilities.Key == InternetExplorerOptions.Capability)
      {
        foreach (KeyValuePair<string, object> keyValuePair in capabilities.Value as Dictionary<string, object>)
          capabilitiesDictionary.Add(keyValuePair.Key, keyValuePair.Value);
      }
      else
        capabilitiesDictionary.Add(capabilities.Key, capabilities.Value);
    }
    return capabilitiesDictionary;
  }

  private static ICapabilities ConvertOptionsToCapabilities(InternetExplorerOptions options)
  {
    return options != null ? options.ToCapabilities() : throw new ArgumentNullException(nameof (options), "options must not be null");
  }
}
