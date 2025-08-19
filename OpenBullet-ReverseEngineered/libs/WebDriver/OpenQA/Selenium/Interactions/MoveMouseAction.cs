// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.MoveMouseAction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Interactions.Internal;
using System;

#nullable disable
namespace OpenQA.Selenium.Interactions;

internal class MoveMouseAction : MouseAction, IAction
{
  public MoveMouseAction(IMouse mouse, ILocatable actionTarget)
    : base(mouse, actionTarget)
  {
    if (actionTarget == null)
      throw new ArgumentException("Must provide a location for a move action.", nameof (actionTarget));
  }

  public void Perform() => this.Mouse.MouseMove(this.ActionLocation);
}
