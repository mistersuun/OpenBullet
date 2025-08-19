// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.Events.WheelEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Html.Dom.Events;

[DomName("WheelEvent")]
public class WheelEvent : MouseEvent
{
  public WheelEvent()
  {
  }

  [DomConstructor]
  [DomInitDict(1, true)]
  public WheelEvent(
    string type,
    bool bubbles = false,
    bool cancelable = false,
    IWindow view = null,
    int detail = 0,
    int screenX = 0,
    int screenY = 0,
    int clientX = 0,
    int clientY = 0,
    MouseButton button = MouseButton.Primary,
    IEventTarget target = null,
    string modifiersList = null,
    double deltaX = 0.0,
    double deltaY = 0.0,
    double deltaZ = 0.0,
    WheelMode deltaMode = WheelMode.Pixel)
  {
    this.Init(type, bubbles, cancelable, view, detail, screenX, screenY, clientX, clientY, button, target, modifiersList ?? string.Empty, deltaX, deltaY, deltaZ, deltaMode);
  }

  [DomName("deltaX")]
  public double DeltaX { get; private set; }

  [DomName("deltaY")]
  public double DeltaY { get; private set; }

  [DomName("deltaZ")]
  public double DeltaZ { get; private set; }

  [DomName("deltaMode")]
  public WheelMode DeltaMode { get; private set; }

  [DomName("initWheelEvent")]
  public void Init(
    string type,
    bool bubbles,
    bool cancelable,
    IWindow view,
    int detail,
    int screenX,
    int screenY,
    int clientX,
    int clientY,
    MouseButton button,
    IEventTarget target,
    string modifiersList,
    double deltaX,
    double deltaY,
    double deltaZ,
    WheelMode deltaMode)
  {
    this.Init(type, bubbles, cancelable, view, detail, screenX, screenY, clientX, clientY, modifiersList.IsCtrlPressed(), modifiersList.IsAltPressed(), modifiersList.IsShiftPressed(), modifiersList.IsMetaPressed(), button, target);
    this.DeltaX = deltaX;
    this.DeltaY = deltaY;
    this.DeltaZ = deltaZ;
    this.DeltaMode = deltaMode;
  }
}
