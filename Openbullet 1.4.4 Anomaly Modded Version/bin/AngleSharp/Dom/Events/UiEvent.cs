// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Events.UiEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom.Events;

[DomName("UIEvent")]
public class UiEvent : Event
{
  public UiEvent()
  {
  }

  [DomConstructor]
  [DomInitDict(1, true)]
  public UiEvent(string type, bool bubbles = false, bool cancelable = false, IWindow view = null, int detail = 0)
  {
    this.Init(type, bubbles, cancelable, view, detail);
  }

  [DomName("view")]
  public IWindow View { get; private set; }

  [DomName("detail")]
  public int Detail { get; private set; }

  [DomName("initUIEvent")]
  public void Init(string type, bool bubbles, bool cancelable, IWindow view, int detail)
  {
    this.Init(type, bubbles, cancelable);
    this.View = view;
    this.Detail = detail;
  }
}
