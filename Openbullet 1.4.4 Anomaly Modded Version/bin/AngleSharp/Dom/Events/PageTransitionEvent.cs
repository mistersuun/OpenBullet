// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Events.PageTransitionEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom.Events;

[DomName("PageTransitionEvent")]
public class PageTransitionEvent : Event
{
  public PageTransitionEvent()
  {
  }

  [DomConstructor]
  [DomInitDict(1, true)]
  public PageTransitionEvent(string type, bool bubbles = false, bool cancelable = false, bool persisted = false)
  {
    this.Init(type, bubbles, cancelable, persisted);
  }

  [DomName("persisted")]
  public bool IsPersisted { get; private set; }

  [DomName("initPageTransitionEvent")]
  public void Init(string type, bool bubbles, bool cancelable, bool persisted)
  {
    this.Init(type, bubbles, cancelable);
    this.IsPersisted = persisted;
  }
}
