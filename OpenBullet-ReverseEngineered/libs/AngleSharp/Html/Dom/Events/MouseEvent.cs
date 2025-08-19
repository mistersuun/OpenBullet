// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.Events.MouseEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom.Events;

[DomName("MouseEvent")]
public class MouseEvent : UiEvent
{
  public MouseEvent()
  {
  }

  [DomConstructor]
  [DomInitDict(1, true)]
  public MouseEvent(
    string type,
    bool bubbles = false,
    bool cancelable = false,
    IWindow view = null,
    int detail = 0,
    int screenX = 0,
    int screenY = 0,
    int clientX = 0,
    int clientY = 0,
    bool ctrlKey = false,
    bool altKey = false,
    bool shiftKey = false,
    bool metaKey = false,
    MouseButton button = MouseButton.Primary,
    IEventTarget relatedTarget = null)
  {
    this.Init(type, bubbles, cancelable, view, detail, screenX, screenY, clientX, clientY, ctrlKey, altKey, shiftKey, metaKey, button, relatedTarget);
  }

  [DomName("screenX")]
  public int ScreenX { get; private set; }

  [DomName("screenY")]
  public int ScreenY { get; private set; }

  [DomName("clientX")]
  public int ClientX { get; private set; }

  [DomName("clientY")]
  public int ClientY { get; private set; }

  [DomName("ctrlKey")]
  public bool IsCtrlPressed { get; private set; }

  [DomName("shiftKey")]
  public bool IsShiftPressed { get; private set; }

  [DomName("altKey")]
  public bool IsAltPressed { get; private set; }

  [DomName("metaKey")]
  public bool IsMetaPressed { get; private set; }

  [DomName("button")]
  public MouseButton Button { get; private set; }

  [DomName("buttons")]
  public MouseButtons Buttons { get; private set; }

  [DomName("relatedTarget")]
  public IEventTarget Target { get; private set; }

  [DomName("getModifierState")]
  public bool GetModifierState(string key) => false;

  [DomName("initMouseEvent")]
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
    bool ctrlKey,
    bool altKey,
    bool shiftKey,
    bool metaKey,
    MouseButton button,
    IEventTarget target)
  {
    this.Init(type, bubbles, cancelable, view, detail);
    this.ScreenX = screenX;
    this.ScreenY = screenY;
    this.ClientX = clientX;
    this.ClientY = clientY;
    this.IsCtrlPressed = ctrlKey;
    this.IsMetaPressed = metaKey;
    this.IsShiftPressed = shiftKey;
    this.IsAltPressed = altKey;
    this.Button = button;
    this.Target = target;
  }
}
