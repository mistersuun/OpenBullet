// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.Events.TouchEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom.Events;

[DomName("TouchEvent")]
public class TouchEvent : UiEvent
{
  public TouchEvent()
  {
  }

  [DomConstructor]
  [DomInitDict(1, true)]
  public TouchEvent(
    string type,
    bool bubbles = false,
    bool cancelable = false,
    IWindow view = null,
    int detail = 0,
    ITouchList touches = null,
    ITouchList targetTouches = null,
    ITouchList changedTouches = null,
    bool ctrlKey = false,
    bool altKey = false,
    bool shiftKey = false,
    bool metaKey = false)
  {
    this.Init(type, bubbles, cancelable, view, detail);
  }

  [DomName("touches")]
  public ITouchList Touches { get; private set; }

  [DomName("targetTouches")]
  public ITouchList TargetTouches { get; private set; }

  [DomName("changedTouches")]
  public ITouchList ChangedTouches { get; private set; }

  [DomName("altKey")]
  public bool IsAltPressed { get; private set; }

  [DomName("metaKey")]
  public bool IsMetaPressed { get; private set; }

  [DomName("ctrlKey")]
  public bool IsCtrlPressed { get; private set; }

  [DomName("shiftKey")]
  public bool IsShiftPressed { get; private set; }

  [DomName("initTouchEvent")]
  public void Init(
    string type,
    bool bubbles,
    bool cancelable,
    IWindow view,
    int detail,
    ITouchList touches,
    ITouchList targetTouches,
    ITouchList changedTouches,
    bool ctrlKey,
    bool altKey,
    bool shiftKey,
    bool metaKey)
  {
    this.Init(type, bubbles, cancelable, view, detail);
    this.Touches = touches;
    this.TargetTouches = targetTouches;
    this.ChangedTouches = changedTouches;
    this.IsCtrlPressed = ctrlKey;
    this.IsShiftPressed = shiftKey;
    this.IsMetaPressed = metaKey;
    this.IsAltPressed = altKey;
  }
}
