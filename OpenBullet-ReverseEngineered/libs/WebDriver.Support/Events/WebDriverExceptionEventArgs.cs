// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.Events.WebDriverExceptionEventArgs
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;

#nullable disable
namespace OpenQA.Selenium.Support.Events;

public class WebDriverExceptionEventArgs : EventArgs
{
  private Exception thrownException;
  private IWebDriver driver;

  public WebDriverExceptionEventArgs(IWebDriver driver, Exception thrownException)
  {
    this.driver = driver;
    this.thrownException = thrownException;
  }

  public Exception ThrownException => this.thrownException;

  public IWebDriver Driver => this.driver;
}
