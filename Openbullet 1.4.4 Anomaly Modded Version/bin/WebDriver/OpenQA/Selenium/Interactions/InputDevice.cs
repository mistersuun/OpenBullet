// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.InputDevice
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Interactions;

public abstract class InputDevice
{
  private string deviceName;

  protected InputDevice(string deviceName)
  {
    this.deviceName = !string.IsNullOrEmpty(deviceName) ? deviceName : throw new ArgumentException("Device name must not be null or empty", nameof (deviceName));
  }

  public string DeviceName => this.deviceName;

  public abstract InputDeviceKind DeviceKind { get; }

  public abstract Dictionary<string, object> ToDictionary();

  public Interaction CreatePause() => this.CreatePause(TimeSpan.Zero);

  public Interaction CreatePause(TimeSpan duration)
  {
    return (Interaction) new PauseInteraction(this, duration);
  }

  public override int GetHashCode() => this.deviceName.GetHashCode();

  public override string ToString()
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} input device [name: {1}]", (object) this.DeviceKind, (object) this.deviceName);
  }
}
