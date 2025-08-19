// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Remote.RemoteKeyboard
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Remote;

internal class RemoteKeyboard : IKeyboard
{
  private RemoteWebDriver driver;

  public RemoteKeyboard(RemoteWebDriver driver) => this.driver = driver;

  public void SendKeys(string keySequence)
  {
    if (keySequence == null)
      throw new ArgumentException("key sequence to send must not be null", nameof (keySequence));
    this.driver.InternalExecute(DriverCommand.SendKeysToActiveElement, new Dictionary<string, object>()
    {
      {
        "value",
        (object) keySequence.ToCharArray()
      }
    });
  }

  public void PressKey(string keyToPress)
  {
    if (keyToPress == null)
      throw new ArgumentException("key to press must not be null", nameof (keyToPress));
    this.driver.InternalExecute(DriverCommand.SendKeysToActiveElement, new Dictionary<string, object>()
    {
      {
        "value",
        (object) keyToPress.ToCharArray()
      }
    });
  }

  public void ReleaseKey(string keyToRelease)
  {
    if (keyToRelease == null)
      throw new ArgumentException("key to release must not be null", nameof (keyToRelease));
    this.driver.InternalExecute(DriverCommand.SendKeysToActiveElement, new Dictionary<string, object>()
    {
      {
        "value",
        (object) keyToRelease.ToCharArray()
      }
    });
  }
}
