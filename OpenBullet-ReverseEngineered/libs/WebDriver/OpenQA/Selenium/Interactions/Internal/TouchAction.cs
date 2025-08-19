// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.Internal.TouchAction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace OpenQA.Selenium.Interactions.Internal;

internal class TouchAction : WebDriverAction
{
  private ITouchScreen touchScreen;

  protected TouchAction(ITouchScreen touchScreen, ILocatable actionTarget)
    : base(actionTarget)
  {
    this.touchScreen = touchScreen;
  }

  protected ITouchScreen TouchScreen => this.touchScreen;

  protected ICoordinates ActionLocation
  {
    get => this.ActionTarget != null ? this.ActionTarget.Coordinates : (ICoordinates) null;
  }
}
