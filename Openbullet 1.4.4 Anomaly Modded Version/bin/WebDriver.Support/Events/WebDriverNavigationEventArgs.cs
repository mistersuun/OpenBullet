// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.Events.WebDriverNavigationEventArgs
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;

#nullable disable
namespace OpenQA.Selenium.Support.Events;

public class WebDriverNavigationEventArgs : EventArgs
{
  private string url;
  private IWebDriver driver;

  public WebDriverNavigationEventArgs(IWebDriver driver)
    : this(driver, (string) null)
  {
  }

  public WebDriverNavigationEventArgs(IWebDriver driver, string url)
  {
    this.url = url;
    this.driver = driver;
  }

  public string Url => this.url;

  public IWebDriver Driver => this.driver;
}
