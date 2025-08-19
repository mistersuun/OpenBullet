// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteNavigator
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Remote;

internal class RemoteNavigator : INavigation
{
  private RemoteWebDriver driver;

  public RemoteNavigator(RemoteWebDriver driver) => this.driver = driver;

  public void Back()
  {
    this.driver.InternalExecute(DriverCommand.GoBack, (Dictionary<string, object>) null);
  }

  public void Forward()
  {
    this.driver.InternalExecute(DriverCommand.GoForward, (Dictionary<string, object>) null);
  }

  public void GoToUrl(string url) => this.driver.Url = url;

  public void GoToUrl(Uri url)
  {
    this.driver.Url = !(url == (Uri) null) ? url.ToString() : throw new ArgumentNullException(nameof (url), "URL cannot be null.");
  }

  public void Refresh()
  {
    this.driver.InternalExecute(DriverCommand.Refresh, (Dictionary<string, object>) null);
  }
}
