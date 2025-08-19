// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Dom.Events.InputEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Dom.Events;

#nullable disable
namespace AngleSharp.Html.Dom.Events;

[DomName("InputEvent")]
public class InputEvent : Event
{
  public InputEvent()
  {
  }

  [DomConstructor]
  [DomInitDict(1, true)]
  public InputEvent(string type, bool bubbles = false, bool cancelable = false, string data = null)
  {
    this.Init(type, bubbles, cancelable, data ?? string.Empty);
  }

  [DomName("data")]
  public string Data { get; private set; }

  [DomName("initInputEvent")]
  public void Init(string type, bool bubbles, bool cancelable, string data)
  {
    this.Init(type, bubbles, cancelable);
    this.Data = data;
  }
}
