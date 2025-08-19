// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteTimeouts
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Remote;

internal class RemoteTimeouts : ITimeouts
{
  private const string ImplicitTimeoutName = "implicit";
  private const string AsyncScriptTimeoutName = "script";
  private const string PageLoadTimeoutName = "pageLoad";
  private const string LegacyPageLoadTimeoutName = "page load";
  private readonly TimeSpan DefaultImplicitWaitTimeout = TimeSpan.FromSeconds(0.0);
  private readonly TimeSpan DefaultAsyncScriptTimeout = TimeSpan.FromSeconds(30.0);
  private readonly TimeSpan DefaultPageLoadTimeout = TimeSpan.FromSeconds(300.0);
  private RemoteWebDriver driver;

  public RemoteTimeouts(RemoteWebDriver driver) => this.driver = driver;

  public TimeSpan ImplicitWait
  {
    get => this.ExecuteGetTimeout("implicit");
    set => this.ExecuteSetTimeout("implicit", value);
  }

  public TimeSpan AsynchronousJavaScript
  {
    get => this.ExecuteGetTimeout("script");
    set => this.ExecuteSetTimeout("script", value);
  }

  public TimeSpan PageLoad
  {
    get
    {
      string timeoutType = "page load";
      if (this.driver.IsSpecificationCompliant)
        timeoutType = "pageLoad";
      return this.ExecuteGetTimeout(timeoutType);
    }
    set
    {
      string timeoutType = "page load";
      if (this.driver.IsSpecificationCompliant)
        timeoutType = "pageLoad";
      this.ExecuteSetTimeout(timeoutType, value);
    }
  }

  private TimeSpan ExecuteGetTimeout(string timeoutType)
  {
    if (!this.driver.IsSpecificationCompliant)
      throw new NotImplementedException("Driver instance must comply with the W3C specification to support getting timeout values.");
    Dictionary<string, object> dictionary = (Dictionary<string, object>) this.driver.InternalExecute(DriverCommand.GetTimeouts, (Dictionary<string, object>) null).Value;
    if (!dictionary.ContainsKey(timeoutType))
      throw new WebDriverException("Specified timeout type not defined");
    return TimeSpan.FromMilliseconds(Convert.ToDouble(dictionary[timeoutType], (IFormatProvider) CultureInfo.InvariantCulture));
  }

  private void ExecuteSetTimeout(string timeoutType, TimeSpan timeToWait)
  {
    double num = timeToWait.TotalMilliseconds;
    if (timeToWait == TimeSpan.MinValue)
    {
      if (this.driver.IsSpecificationCompliant)
      {
        switch (timeoutType)
        {
          case "implicit":
            num = this.DefaultImplicitWaitTimeout.TotalMilliseconds;
            break;
          case "script":
            num = this.DefaultAsyncScriptTimeout.TotalMilliseconds;
            break;
          default:
            num = this.DefaultPageLoadTimeout.TotalMilliseconds;
            break;
        }
      }
      else
        num = -1.0;
    }
    Dictionary<string, object> parameters = new Dictionary<string, object>();
    if (this.driver.IsSpecificationCompliant)
    {
      parameters.Add(timeoutType, (object) Convert.ToInt64(num));
    }
    else
    {
      parameters.Add("type", (object) timeoutType);
      parameters.Add("ms", (object) num);
    }
    this.driver.InternalExecute(DriverCommand.SetTimeouts, parameters);
  }
}
