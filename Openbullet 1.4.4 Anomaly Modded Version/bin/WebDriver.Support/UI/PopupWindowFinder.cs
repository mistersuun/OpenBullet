// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.UI.PopupWindowFinder
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace OpenQA.Selenium.Support.UI;

public class PopupWindowFinder
{
  private readonly IWebDriver driver;
  private readonly TimeSpan timeout;
  private readonly TimeSpan sleepInterval;

  public PopupWindowFinder(IWebDriver driver)
    : this(driver, PopupWindowFinder.DefaultTimeout, PopupWindowFinder.DefaultSleepInterval)
  {
  }

  public PopupWindowFinder(IWebDriver driver, TimeSpan timeout)
    : this(driver, timeout, PopupWindowFinder.DefaultSleepInterval)
  {
  }

  public PopupWindowFinder(IWebDriver driver, TimeSpan timeout, TimeSpan sleepInterval)
  {
    this.driver = driver;
    this.timeout = timeout;
    this.sleepInterval = sleepInterval;
  }

  private static TimeSpan DefaultTimeout => TimeSpan.FromSeconds(5.0);

  private static TimeSpan DefaultSleepInterval => TimeSpan.FromMilliseconds(250.0);

  public string Click(IWebElement element)
  {
    return element != null ? this.Invoke((Action) (() => element.Click())) : throw new ArgumentNullException(nameof (element), "element cannot be null");
  }

  public string Invoke(Action popupMethod)
  {
    if (popupMethod == null)
      throw new ArgumentNullException(nameof (popupMethod), "popupMethod cannot be null");
    IList<string> existingHandles = (IList<string>) this.driver.WindowHandles;
    popupMethod();
    return new WebDriverWait((IClock) new SystemClock(), this.driver, this.timeout, this.sleepInterval).Until<string>((Func<IWebDriver, string>) (d =>
    {
      string str = (string) null;
      IList<string> difference = PopupWindowFinder.GetDifference(existingHandles, (IList<string>) this.driver.WindowHandles);
      if (difference.Count > 0)
        str = difference[0];
      return str;
    }));
  }

  private static IList<string> GetDifference(
    IList<string> existingHandles,
    IList<string> currentHandles)
  {
    return (IList<string>) currentHandles.Except<string>((IEnumerable<string>) existingHandles).ToList<string>();
  }
}
