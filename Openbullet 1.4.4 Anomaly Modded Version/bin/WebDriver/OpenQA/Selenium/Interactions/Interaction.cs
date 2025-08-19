// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.Interaction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Interactions;

public abstract class Interaction
{
  private InputDevice sourceDevice;

  protected Interaction(InputDevice sourceDevice)
  {
    this.sourceDevice = sourceDevice != null ? sourceDevice : throw new ArgumentNullException(nameof (sourceDevice), "Source device cannot be null");
  }

  public InputDevice SourceDevice => this.sourceDevice;

  public abstract Dictionary<string, object> ToDictionary();

  public virtual bool IsValidFor(InputDeviceKind sourceDeviceKind)
  {
    return this.sourceDevice.DeviceKind == sourceDeviceKind;
  }
}
