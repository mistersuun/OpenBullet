// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.FlickAction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Interactions.Internal;
using System;

#nullable disable
namespace OpenQA.Selenium.Interactions;

internal class FlickAction : TouchAction, IAction
{
  private int offsetX;
  private int offsetY;
  private int speed;
  private int speedX;
  private int speedY;

  public FlickAction(ITouchScreen touchScreen, int speedX, int speedY)
    : base(touchScreen, (ILocatable) null)
  {
    this.speedX = speedX;
    this.speedY = speedY;
  }

  public FlickAction(
    ITouchScreen touchScreen,
    ILocatable actionTarget,
    int offsetX,
    int offsetY,
    int speed)
    : base(touchScreen, actionTarget)
  {
    if (actionTarget == null)
      throw new ArgumentException("Must provide a location for a single tap action.", nameof (actionTarget));
    this.offsetX = offsetX;
    this.offsetY = offsetY;
    this.speed = speed;
  }

  public void Perform()
  {
    if (this.ActionLocation != null)
      this.TouchScreen.Flick(this.ActionLocation, this.offsetX, this.offsetY, this.speed);
    else
      this.TouchScreen.Flick(this.speedX, this.speedY);
  }
}
