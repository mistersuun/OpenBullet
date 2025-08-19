// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Firefox.FirefoxWebElement
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;

#nullable disable
namespace OpenQA.Selenium.Firefox;

public class FirefoxWebElement(FirefoxDriver parentDriver, string id) : RemoteWebElement((RemoteWebDriver) parentDriver, id)
{
  public override bool Equals(object obj)
  {
    if (!(obj is IWebElement webElement))
      return false;
    if (webElement is IWrapsElement)
      webElement = ((IWrapsElement) obj).WrappedElement;
    return webElement is FirefoxWebElement firefoxWebElement && this.Id == firefoxWebElement.Id;
  }

  public override int GetHashCode() => base.GetHashCode();
}
