// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Events.MessageEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;

#nullable disable
namespace AngleSharp.Dom.Events;

[DomName("MessageEvent")]
public class MessageEvent : Event
{
  public MessageEvent()
  {
  }

  [DomConstructor]
  [DomInitDict(1, true)]
  public MessageEvent(
    string type,
    bool bubbles = false,
    bool cancelable = false,
    object data = null,
    string origin = null,
    string lastEventId = null,
    IWindow source = null,
    params IMessagePort[] ports)
  {
    this.Init(type, bubbles, cancelable, data, origin ?? string.Empty, lastEventId ?? string.Empty, source, ports);
  }

  [DomName("data")]
  public object Data { get; private set; }

  [DomName("origin")]
  public string Origin { get; private set; }

  [DomName("lastEventId")]
  public string LastEventId { get; private set; }

  [DomName("source")]
  public IWindow Source { get; private set; }

  [DomName("ports")]
  public IMessagePort[] Ports { get; private set; }

  [DomName("initMessageEvent")]
  public void Init(
    string type,
    bool bubbles,
    bool cancelable,
    object data,
    string origin,
    string lastEventId,
    IWindow source,
    params IMessagePort[] ports)
  {
    this.Init(type, bubbles, cancelable);
    this.Data = data;
    this.Origin = origin;
    this.LastEventId = lastEventId;
    this.Source = source;
    this.Ports = ports;
  }
}
