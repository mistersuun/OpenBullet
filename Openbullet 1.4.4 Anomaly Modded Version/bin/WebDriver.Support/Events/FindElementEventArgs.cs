// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.Events.FindElementEventArgs
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;

#nullable disable
namespace OpenQA.Selenium.Support.Events;

public class FindElementEventArgs : EventArgs
{
  private IWebDriver driver;
  private IWebElement element;
  private By method;

  public FindElementEventArgs(IWebDriver driver, By method)
    : this(driver, (IWebElement) null, method)
  {
  }

  public FindElementEventArgs(IWebDriver driver, IWebElement element, By method)
  {
    this.driver = driver;
    this.element = element;
    this.method = method;
  }

  public IWebDriver Driver => this.driver;

  public IWebElement Element => this.element;

  public By FindMethod => this.method;
}
