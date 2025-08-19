// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.ScreenPressAction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Interactions.Internal;

#nullable disable
namespace OpenQA.Selenium.Interactions;

internal class ScreenPressAction : TouchAction, IAction
{
  private int x;
  private int y;

  public ScreenPressAction(ITouchScreen touchScreen, int x, int y)
    : base(touchScreen, (ILocatable) null)
  {
    this.x = x;
    this.y = y;
  }

  public void Perform() => this.TouchScreen.Down(this.x, this.y);
}
