// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Events.HashChangedEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom.Events;

[DomName("HashChangeEvent")]
public class HashChangedEvent : Event
{
  public HashChangedEvent()
  {
  }

  [DomConstructor]
  [DomInitDict(1, true)]
  public HashChangedEvent(
    string type,
    bool bubbles = false,
    bool cancelable = false,
    string oldURL = null,
    string newURL = null)
  {
    this.Init(type, bubbles, cancelable, oldURL ?? string.Empty, newURL ?? string.Empty);
  }

  [DomName("oldURL")]
  public string PreviousUrl { get; private set; }

  [DomName("newURL")]
  public string CurrentUrl { get; private set; }

  [DomName("initHashChangedEvent")]
  public void Init(
    string type,
    bool bubbles,
    bool cancelable,
    string previousUrl,
    string currentUrl)
  {
    this.Init(type, bubbles, cancelable);
    this.Stop();
    this.PreviousUrl = previousUrl;
    this.CurrentUrl = currentUrl;
  }
}
