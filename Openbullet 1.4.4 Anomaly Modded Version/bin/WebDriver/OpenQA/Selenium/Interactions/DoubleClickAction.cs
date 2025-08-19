// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.DoubleClickAction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Interactions.Internal;

#nullable disable
namespace OpenQA.Selenium.Interactions;

internal class DoubleClickAction(IMouse mouse, ILocatable actionTarget) : MouseAction(mouse, actionTarget), IAction
{
  public void Perform()
  {
    this.MoveToLocation();
    this.Mouse.DoubleClick(this.ActionLocation);
  }
}
