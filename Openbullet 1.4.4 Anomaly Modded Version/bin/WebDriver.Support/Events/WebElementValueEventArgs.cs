// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.Events.WebElementValueEventArgs
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

#nullable disable
namespace OpenQA.Selenium.Support.Events;

public class WebElementValueEventArgs : WebElementEventArgs
{
  public WebElementValueEventArgs(IWebDriver driver, IWebElement element, string value)
    : base(driver, element)
  {
    this.Value = value;
  }

  public string Value { get; private set; }
}
