// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.Internal.MouseAction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace OpenQA.Selenium.Interactions.Internal;

internal class MouseAction : WebDriverAction
{
  private IMouse mouse;

  public MouseAction(IMouse mouse, ILocatable target)
    : base(target)
  {
    this.mouse = mouse;
  }

  protected ICoordinates ActionLocation
  {
    get => this.ActionTarget == null ? (ICoordinates) null : this.ActionTarget.Coordinates;
  }

  protected IMouse Mouse => this.mouse;

  protected void MoveToLocation()
  {
    if (this.ActionLocation == null)
      return;
    this.mouse.MouseMove(this.ActionLocation);
  }
}
