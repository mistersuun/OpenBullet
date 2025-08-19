// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.UI.WebDriverWait
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;

#nullable disable
namespace OpenQA.Selenium.Support.UI;

public class WebDriverWait : DefaultWait<IWebDriver>
{
  public WebDriverWait(IWebDriver driver, TimeSpan timeout)
    : this((IClock) new SystemClock(), driver, timeout, WebDriverWait.DefaultSleepTimeout)
  {
  }

  public WebDriverWait(IClock clock, IWebDriver driver, TimeSpan timeout, TimeSpan sleepInterval)
    : base(driver, clock)
  {
    this.Timeout = timeout;
    this.PollingInterval = sleepInterval;
    this.IgnoreExceptionTypes(new Type[1]
    {
      typeof (NotFoundException)
    });
  }

  private static TimeSpan DefaultSleepTimeout => TimeSpan.FromMilliseconds(500.0);
}
