// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Chrome.ChromeDriver
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Chrome;

public class ChromeDriver : RemoteWebDriver
{
  public static readonly bool AcceptUntrustedCertificates = true;
  private const string GetNetworkConditionsCommand = "getNetworkConditions";
  private const string SetNetworkConditionsCommand = "setNetworkConditions";
  private const string DeleteNetworkConditionsCommand = "deleteNetworkConditions";
  private const string SendChromeCommand = "sendChromeCommand";
  private const string SendChromeCommandWithResult = "sendChromeCommandWithResult";

  public ChromeDriver()
    : this(new ChromeOptions())
  {
  }

  public ChromeDriver(ChromeOptions options)
    : this(ChromeDriverService.CreateDefaultService(), options, RemoteWebDriver.DefaultCommandTimeout)
  {
  }

  public ChromeDriver(ChromeDriverService service)
    : this(service, new ChromeOptions())
  {
  }

  public ChromeDriver(string chromeDriverDirectory)
    : this(chromeDriverDirectory, new ChromeOptions())
  {
  }

  public ChromeDriver(string chromeDriverDirectory, ChromeOptions options)
    : this(chromeDriverDirectory, options, RemoteWebDriver.DefaultCommandTimeout)
  {
  }

  public ChromeDriver(string chromeDriverDirectory, ChromeOptions options, TimeSpan commandTimeout)
    : this(ChromeDriverService.CreateDefaultService(chromeDriverDirectory), options, commandTimeout)
  {
  }

  public ChromeDriver(ChromeDriverService service, ChromeOptions options)
    : this(service, options, RemoteWebDriver.DefaultCommandTimeout)
  {
  }

  public ChromeDriver(ChromeDriverService service, ChromeOptions options, TimeSpan commandTimeout)
    : base((ICommandExecutor) new DriverServiceCommandExecutor((DriverService) service, commandTimeout), ChromeDriver.ConvertOptionsToCapabilities(options))
  {
    this.AddCustomChromeCommand("getNetworkConditions", "GET", "/session/{sessionId}/chromium/network_conditions");
    this.AddCustomChromeCommand("setNetworkConditions", "POST", "/session/{sessionId}/chromium/network_conditions");
    this.AddCustomChromeCommand("deleteNetworkConditions", "DELETE", "/session/{sessionId}/chromium/network_conditions");
    this.AddCustomChromeCommand("sendChromeCommand", "POST", "/session/{sessionId}/chromium/send_command");
    this.AddCustomChromeCommand("sendChromeCommandWithResult", "POST", "/session/{sessionId}/chromium/send_command_and_get_result");
  }

  public override IFileDetector FileDetector
  {
    get => base.FileDetector;
    set
    {
    }
  }

  public ChromeNetworkConditions NetworkConditions
  {
    get
    {
      return ChromeNetworkConditions.FromDictionary(this.Execute("getNetworkConditions", (Dictionary<string, object>) null).Value as Dictionary<string, object>);
    }
    set
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value), "value must not be null");
      this.Execute("setNetworkConditions", new Dictionary<string, object>()
      {
        ["network_conditions"] = (object) value.ToDictionary()
      });
    }
  }

  public void ExecuteChromeCommand(string commandName, Dictionary<string, object> commandParameters)
  {
    if (commandName == null)
      throw new ArgumentNullException(nameof (commandName), "commandName must not be null");
    this.Execute("sendChromeCommand", new Dictionary<string, object>()
    {
      ["cmd"] = (object) commandName,
      ["params"] = (object) commandParameters
    });
  }

  public object ExecuteChromeCommandWithResult(
    string commandName,
    Dictionary<string, object> commandParameters)
  {
    if (commandName == null)
      throw new ArgumentNullException(nameof (commandName), "commandName must not be null");
    return this.Execute("sendChromeCommandWithResult", new Dictionary<string, object>()
    {
      ["cmd"] = (object) commandName,
      ["params"] = (object) commandParameters
    }).Value;
  }

  private static ICapabilities ConvertOptionsToCapabilities(ChromeOptions options)
  {
    return options != null ? options.ToCapabilities() : throw new ArgumentNullException(nameof (options), "options must not be null");
  }

  private void AddCustomChromeCommand(string commandName, string method, string resourcePath)
  {
    CommandInfo commandInfo = new CommandInfo(method, resourcePath);
    this.CommandExecutor.CommandInfoRepository.TryAddCommand(commandName, commandInfo);
  }
}
