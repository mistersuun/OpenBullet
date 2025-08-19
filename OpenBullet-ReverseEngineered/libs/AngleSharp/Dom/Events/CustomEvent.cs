// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Events.CustomEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom.Events;

[DomName("CustomEvent")]
public class CustomEvent : Event
{
  public CustomEvent()
  {
  }

  [DomConstructor]
  [DomInitDict(1, true)]
  public CustomEvent(string type, bool bubbles = false, bool cancelable = false, object details = null)
  {
    this.Init(type, bubbles, cancelable, details);
  }

  [DomName("detail")]
  public object Details { get; private set; }

  [DomName("initCustomEvent")]
  public void Init(string type, bool bubbles, bool cancelable, object details)
  {
    this.Init(type, bubbles, cancelable);
    this.Details = details;
  }
}
