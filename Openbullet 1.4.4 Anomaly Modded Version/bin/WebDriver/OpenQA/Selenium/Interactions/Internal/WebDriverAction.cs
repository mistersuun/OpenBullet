// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.Internal.WebDriverAction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace OpenQA.Selenium.Interactions.Internal;

internal abstract class WebDriverAction
{
  private ILocatable where;

  protected WebDriverAction(ILocatable actionLocation) => this.where = actionLocation;

  protected WebDriverAction()
  {
  }

  protected ILocatable ActionTarget => this.where;
}
