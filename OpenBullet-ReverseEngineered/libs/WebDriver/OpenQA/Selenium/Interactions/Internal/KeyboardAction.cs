// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.Internal.KeyboardAction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

#nullable disable
namespace OpenQA.Selenium.Interactions.Internal;

internal class KeyboardAction : WebDriverAction
{
  private IKeyboard keyboard;
  private IMouse mouse;

  protected KeyboardAction(IKeyboard keyboard, IMouse mouse, ILocatable actionTarget)
    : base(actionTarget)
  {
    this.keyboard = keyboard;
    this.mouse = mouse;
  }

  protected IKeyboard Keyboard => this.keyboard;

  protected void FocusOnElement()
  {
    if (this.ActionTarget == null)
      return;
    this.mouse.Click(this.ActionTarget.Coordinates);
  }
}
