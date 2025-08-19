// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Events.FocusEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom.Events;

[DomName("FocusEvent")]
public class FocusEvent : UiEvent
{
  public FocusEvent()
  {
  }

  [DomConstructor]
  [DomInitDict(1, true)]
  public FocusEvent(
    string type,
    bool bubbles = false,
    bool cancelable = false,
    IWindow view = null,
    int detail = 0,
    IEventTarget target = null)
  {
    this.Init(type, bubbles, cancelable, view, detail, target);
  }

  [DomName("relatedTarget")]
  public IEventTarget Target { get; private set; }

  [DomName("initFocusEvent")]
  public void Init(
    string type,
    bool bubbles,
    bool cancelable,
    IWindow view,
    int detail,
    IEventTarget target)
  {
    this.Init(type, bubbles, cancelable, view, detail);
    this.Target = target;
  }
}
