// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.KeyInputDevice
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Interactions;

public class KeyInputDevice(string deviceName) : InputDevice(deviceName)
{
  public KeyInputDevice()
    : this(Guid.NewGuid().ToString())
  {
  }

  public override InputDeviceKind DeviceKind => InputDeviceKind.Key;

  public override Dictionary<string, object> ToDictionary()
  {
    return new Dictionary<string, object>()
    {
      ["type"] = (object) "key",
      ["id"] = (object) this.DeviceName
    };
  }

  public Interaction CreateKeyDown(char codePoint)
  {
    return (Interaction) new KeyInputDevice.KeyDownInteraction((InputDevice) this, codePoint);
  }

  public Interaction CreateKeyUp(char codePoint)
  {
    return (Interaction) new KeyInputDevice.KeyUpInteraction((InputDevice) this, codePoint);
  }

  private class KeyDownInteraction(InputDevice sourceDevice, char codePoint) : 
    KeyInputDevice.TypingInteraction(sourceDevice, "keyDown", codePoint)
  {
    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Key down [key: {0}]", Keys.GetDescription(this.Value));
    }
  }

  private class KeyUpInteraction(InputDevice sourceDevice, char codePoint) : 
    KeyInputDevice.TypingInteraction(sourceDevice, "keyUp", codePoint)
  {
    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Key up [key: {0}]", Keys.GetDescription(this.Value));
    }
  }

  private class TypingInteraction : Interaction
  {
    private string type;
    private string value;

    public TypingInteraction(InputDevice sourceDevice, string type, char codePoint)
      : base(sourceDevice)
    {
      this.type = type;
      this.value = codePoint.ToString();
    }

    protected string Value => this.value;

    public override Dictionary<string, object> ToDictionary()
    {
      return new Dictionary<string, object>()
      {
        ["type"] = (object) this.type,
        ["value"] = (object) this.value
      };
    }
  }
}
