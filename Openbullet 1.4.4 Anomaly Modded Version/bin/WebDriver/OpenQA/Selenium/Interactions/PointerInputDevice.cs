// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.PointerInputDevice
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace OpenQA.Selenium.Interactions;

public class PointerInputDevice : InputDevice
{
  private PointerKind pointerKind;

  public PointerInputDevice()
    : this(PointerKind.Mouse)
  {
  }

  public PointerInputDevice(PointerKind pointerKind)
    : this(pointerKind, Guid.NewGuid().ToString())
  {
  }

  public PointerInputDevice(PointerKind pointerKind, string deviceName)
    : base(deviceName)
  {
    this.pointerKind = pointerKind;
  }

  public override InputDeviceKind DeviceKind => InputDeviceKind.Pointer;

  public override Dictionary<string, object> ToDictionary()
  {
    return new Dictionary<string, object>()
    {
      ["type"] = (object) "pointer",
      ["id"] = (object) this.DeviceName,
      ["parameters"] = (object) new Dictionary<string, object>()
      {
        ["pointerType"] = (object) this.pointerKind.ToString().ToLowerInvariant()
      }
    };
  }

  public Interaction CreatePointerDown(MouseButton button)
  {
    return (Interaction) new PointerInputDevice.PointerDownInteraction((InputDevice) this, button);
  }

  public Interaction CreatePointerUp(MouseButton button)
  {
    return (Interaction) new PointerInputDevice.PointerUpInteraction((InputDevice) this, button);
  }

  public Interaction CreatePointerMove(
    IWebElement target,
    int xOffset,
    int yOffset,
    TimeSpan duration)
  {
    return (Interaction) new PointerInputDevice.PointerMoveInteraction((InputDevice) this, target, CoordinateOrigin.Element, xOffset, yOffset, duration);
  }

  public Interaction CreatePointerMove(
    CoordinateOrigin origin,
    int xOffset,
    int yOffset,
    TimeSpan duration)
  {
    if (origin == CoordinateOrigin.Element)
      throw new ArgumentException("Using a value of CoordinateOrigin.Element without an element is not supported.", nameof (origin));
    return (Interaction) new PointerInputDevice.PointerMoveInteraction((InputDevice) this, (IWebElement) null, origin, xOffset, yOffset, duration);
  }

  public Interaction CreatePointerCancel()
  {
    return (Interaction) new PointerInputDevice.PointerCancelInteraction((InputDevice) this);
  }

  private class PointerDownInteraction : Interaction
  {
    private MouseButton button;

    public PointerDownInteraction(InputDevice sourceDevice, MouseButton button)
      : base(sourceDevice)
    {
      this.button = button;
    }

    public override Dictionary<string, object> ToDictionary()
    {
      return new Dictionary<string, object>()
      {
        ["type"] = (object) "pointerDown",
        ["button"] = (object) Convert.ToInt32((object) this.button, (IFormatProvider) CultureInfo.InvariantCulture)
      };
    }

    public override string ToString() => "Pointer down";
  }

  private class PointerUpInteraction : Interaction
  {
    private MouseButton button;

    public PointerUpInteraction(InputDevice sourceDevice, MouseButton button)
      : base(sourceDevice)
    {
      this.button = button;
    }

    public override Dictionary<string, object> ToDictionary()
    {
      return new Dictionary<string, object>()
      {
        ["type"] = (object) "pointerUp",
        ["button"] = (object) Convert.ToInt32((object) this.button, (IFormatProvider) CultureInfo.InvariantCulture)
      };
    }

    public override string ToString() => "Pointer up";
  }

  private class PointerCancelInteraction(InputDevice sourceDevice) : Interaction(sourceDevice)
  {
    public override Dictionary<string, object> ToDictionary()
    {
      return new Dictionary<string, object>()
      {
        ["type"] = (object) "pointerCancel"
      };
    }

    public override string ToString() => "Pointer cancel";
  }

  private class PointerMoveInteraction : Interaction
  {
    private IWebElement target;
    private int x;
    private int y;
    private TimeSpan duration = TimeSpan.MinValue;
    private CoordinateOrigin origin = CoordinateOrigin.Pointer;

    public PointerMoveInteraction(
      InputDevice sourceDevice,
      IWebElement target,
      CoordinateOrigin origin,
      int x,
      int y,
      TimeSpan duration)
      : base(sourceDevice)
    {
      if (target != null)
      {
        this.target = target;
        this.origin = CoordinateOrigin.Element;
      }
      else if (this.origin != CoordinateOrigin.Element)
        this.origin = origin;
      if (duration != TimeSpan.MinValue)
        this.duration = duration;
      this.x = x;
      this.y = y;
    }

    public override Dictionary<string, object> ToDictionary()
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      dictionary["type"] = (object) "pointerMove";
      if (this.duration != TimeSpan.MinValue)
        dictionary["duration"] = (object) Convert.ToInt64(this.duration.TotalMilliseconds);
      dictionary["origin"] = this.target == null ? (object) this.origin.ToString().ToLowerInvariant() : (object) this.ConvertElement();
      dictionary["x"] = (object) this.x;
      dictionary["y"] = (object) this.y;
      return dictionary;
    }

    public override string ToString()
    {
      string str = this.origin.ToString();
      if (this.origin == CoordinateOrigin.Element)
        str = this.target.ToString();
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Pointer move [origin: {0}, x offset: {1}, y offset: {2}, duration: {3}ms]", (object) str, (object) this.x, (object) this.y, (object) this.duration.TotalMilliseconds);
    }

    private Dictionary<string, object> ConvertElement()
    {
      if (!(this.target is IWebElementReference elementReference) && this.target is IWrapsElement target)
        elementReference = target.WrappedElement as IWebElementReference;
      return elementReference != null ? elementReference.ToDictionary() : throw new ArgumentException("Target element cannot be converted to IWebElementReference");
    }
  }
}
