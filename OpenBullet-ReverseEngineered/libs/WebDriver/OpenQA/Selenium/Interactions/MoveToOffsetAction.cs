// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.MoveToOffsetAction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Interactions.Internal;

#nullable disable
namespace OpenQA.Selenium.Interactions;

internal class MoveToOffsetAction : MouseAction, IAction
{
  private int offsetX;
  private int offsetY;

  public MoveToOffsetAction(IMouse mouse, ILocatable actionTarget, int offsetX, int offsetY)
    : base(mouse, actionTarget)
  {
    this.offsetX = offsetX;
    this.offsetY = offsetY;
  }

  public void Perform() => this.Mouse.MouseMove(this.ActionLocation, this.offsetX, this.offsetY);
}
