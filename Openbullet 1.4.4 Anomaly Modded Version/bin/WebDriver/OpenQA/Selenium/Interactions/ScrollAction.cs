// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.ScrollAction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Interactions.Internal;
using System;

#nullable disable
namespace OpenQA.Selenium.Interactions;

internal class ScrollAction : TouchAction, IAction
{
  private int offsetX;
  private int offsetY;

  public ScrollAction(ITouchScreen touchScreen, int offsetX, int offsetY)
    : this(touchScreen, (ILocatable) null, offsetX, offsetY)
  {
  }

  public ScrollAction(ITouchScreen touchScreen, ILocatable actionTarget, int offsetX, int offsetY)
    : base(touchScreen, actionTarget)
  {
    if (actionTarget == null)
      throw new ArgumentException("Must provide a location for a single tap action.", nameof (actionTarget));
    this.offsetX = offsetX;
    this.offsetY = offsetY;
  }

  public void Perform() => this.TouchScreen.Scroll(this.ActionLocation, this.offsetX, this.offsetY);
}
