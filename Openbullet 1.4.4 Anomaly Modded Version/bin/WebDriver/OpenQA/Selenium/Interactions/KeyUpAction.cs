// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.KeyUpAction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Interactions.Internal;

#nullable disable
namespace OpenQA.Selenium.Interactions;

internal class KeyUpAction(IKeyboard keyboard, IMouse mouse, ILocatable actionTarget, string key) : 
  SingleKeyAction(keyboard, mouse, actionTarget, key),
  IAction
{
  public void Perform()
  {
    this.FocusOnElement();
    this.Keyboard.ReleaseKey(this.Key);
  }
}
