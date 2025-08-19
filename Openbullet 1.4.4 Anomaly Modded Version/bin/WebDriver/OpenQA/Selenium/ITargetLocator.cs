// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.ITargetLocator
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace OpenQA.Selenium;

public interface ITargetLocator
{
  IWebDriver Frame(int frameIndex);

  IWebDriver Frame(string frameName);

  IWebDriver Frame(IWebElement frameElement);

  IWebDriver ParentFrame();

  IWebDriver Window(string windowName);

  IWebDriver DefaultContent();

  IWebElement ActiveElement();

  IAlert Alert();
}
