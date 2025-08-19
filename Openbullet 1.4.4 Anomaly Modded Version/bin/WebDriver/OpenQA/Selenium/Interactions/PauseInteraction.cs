// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.PauseInteraction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Interactions;

internal class PauseInteraction : Interaction
{
  private TimeSpan duration = TimeSpan.Zero;

  public PauseInteraction(InputDevice sourceDevice)
    : this(sourceDevice, TimeSpan.Zero)
  {
  }

  public PauseInteraction(InputDevice sourceDevice, TimeSpan duration)
    : base(sourceDevice)
  {
    this.duration = !(duration < TimeSpan.Zero) ? duration : throw new ArgumentException("Duration must be greater than or equal to zero", nameof (duration));
  }

  public override Dictionary<string, object> ToDictionary()
  {
    return new Dictionary<string, object>()
    {
      ["type"] = (object) "pause",
      ["duration"] = (object) Convert.ToInt64(this.duration.TotalMilliseconds)
    };
  }

  public override bool IsValidFor(InputDeviceKind sourceDeviceKind) => true;
}
