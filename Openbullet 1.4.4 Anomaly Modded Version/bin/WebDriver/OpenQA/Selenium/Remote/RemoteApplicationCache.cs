// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteApplicationCache
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Html5;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Remote;

public class RemoteApplicationCache : IApplicationCache
{
  private RemoteWebDriver driver;

  public RemoteApplicationCache(RemoteWebDriver driver) => this.driver = driver;

  public AppCacheStatus Status
  {
    get
    {
      Response response = this.driver.InternalExecute(DriverCommand.GetAppCacheStatus, (Dictionary<string, object>) null);
      Type enumType = typeof (AppCacheStatus);
      if (!Enum.IsDefined(enumType, (object) Convert.ToInt32(response.Value, (IFormatProvider) CultureInfo.InvariantCulture)))
        throw new InvalidOperationException("Value returned from remote end is not a number or is not in the specified range of values. Actual value was " + response.Value.ToString());
      return (AppCacheStatus) Enum.ToObject(enumType, response.Value);
    }
  }
}
