// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.Internal.SingleKeyAction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Interactions.Internal;

internal class SingleKeyAction : KeyboardAction
{
  private static readonly List<string> ModifierKeys = new List<string>()
  {
    Keys.Shift,
    Keys.Control,
    Keys.Alt,
    Keys.Meta,
    Keys.Command,
    Keys.LeftAlt,
    Keys.LeftControl,
    Keys.LeftShift
  };
  private string key;

  protected SingleKeyAction(IKeyboard keyboard, IMouse mouse, ILocatable actionTarget, string key)
    : base(keyboard, mouse, actionTarget)
  {
    this.key = SingleKeyAction.ModifierKeys.Contains(key) ? key : throw new ArgumentException("key must be a modifier key (Keys.Shift, Keys.Control, Keys.Alt, Keys.Meta, Keys.Command, Keys.LeftAlt, Keys.LeftControl, Keys.LeftShift)", nameof (key));
  }

  protected string Key => this.key;
}
