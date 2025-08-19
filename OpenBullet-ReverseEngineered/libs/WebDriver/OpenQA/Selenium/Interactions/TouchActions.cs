// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.TouchActions
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using System;

#nullable disable
namespace OpenQA.Selenium.Interactions;

public class TouchActions : Actions
{
  private ITouchScreen touchScreen;

  public TouchActions(IWebDriver driver)
    : base(driver)
  {
    if (!(driver is IHasTouchScreen hasTouchScreen1))
    {
      IWrapsDriver wrapsDriver = driver as IWrapsDriver;
      while (wrapsDriver != null && !(wrapsDriver.WrappedDriver is IHasTouchScreen hasTouchScreen1))
        wrapsDriver = wrapsDriver.WrappedDriver as IWrapsDriver;
    }
    this.touchScreen = hasTouchScreen1 != null ? hasTouchScreen1.TouchScreen : throw new ArgumentException("The IWebDriver object must implement or wrap a driver that implements IHasTouchScreen.", nameof (driver));
  }

  public TouchActions SingleTap(IWebElement onElement)
  {
    this.AddAction((IAction) new SingleTapAction(this.touchScreen, Actions.GetLocatableFromElement(onElement)));
    return this;
  }

  public TouchActions Down(int locationX, int locationY)
  {
    this.AddAction((IAction) new ScreenPressAction(this.touchScreen, locationX, locationY));
    return this;
  }

  public TouchActions Up(int locationX, int locationY)
  {
    this.AddAction((IAction) new ScreenReleaseAction(this.touchScreen, locationX, locationY));
    return this;
  }

  public TouchActions Move(int locationX, int locationY)
  {
    this.AddAction((IAction) new ScreenMoveAction(this.touchScreen, locationX, locationY));
    return this;
  }

  public TouchActions Scroll(IWebElement onElement, int offsetX, int offsetY)
  {
    this.AddAction((IAction) new ScrollAction(this.touchScreen, Actions.GetLocatableFromElement(onElement), offsetX, offsetY));
    return this;
  }

  public TouchActions DoubleTap(IWebElement onElement)
  {
    this.AddAction((IAction) new DoubleTapAction(this.touchScreen, Actions.GetLocatableFromElement(onElement)));
    return this;
  }

  public TouchActions LongPress(IWebElement onElement)
  {
    this.AddAction((IAction) new LongPressAction(this.touchScreen, Actions.GetLocatableFromElement(onElement)));
    return this;
  }

  public TouchActions Scroll(int offsetX, int offsetY)
  {
    this.AddAction((IAction) new ScrollAction(this.touchScreen, offsetX, offsetY));
    return this;
  }

  public TouchActions Flick(int speedX, int speedY)
  {
    this.AddAction((IAction) new FlickAction(this.touchScreen, speedX, speedY));
    return this;
  }

  public TouchActions Flick(IWebElement onElement, int offsetX, int offsetY, int speed)
  {
    this.AddAction((IAction) new FlickAction(this.touchScreen, Actions.GetLocatableFromElement(onElement), offsetX, offsetY, speed));
    return this;
  }
}
