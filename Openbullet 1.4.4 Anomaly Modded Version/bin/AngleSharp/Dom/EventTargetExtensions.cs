// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.EventTargetExtensions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom.Events;
using System;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Dom;

public static class EventTargetExtensions
{
  public static bool FireSimpleEvent(
    this IEventTarget target,
    string eventName,
    bool bubble = false,
    bool cancelable = false)
  {
    Event @event = new Event();
    @event.IsTrusted = true;
    @event.Init(eventName, bubble, cancelable);
    return @event.Dispatch(target);
  }

  public static bool Fire(this IEventTarget target, Event eventData)
  {
    eventData.IsTrusted = true;
    return eventData.Dispatch(target);
  }

  public static bool Fire<T>(
    this IEventTarget target,
    Action<T> initializer,
    IEventTarget targetOverride = null)
    where T : Event, new()
  {
    T obj1 = new T();
    obj1.IsTrusted = true;
    T obj2 = obj1;
    initializer(obj2);
    return obj2.Dispatch(targetOverride ?? target);
  }

  public static async Task<Event> AwaitEventAsync<TEventTarget>(
    this TEventTarget node,
    string eventName)
    where TEventTarget : IEventTarget
  {
    if ((object) (TEventTarget) node == null)
      throw new ArgumentNullException(nameof (node));
    if (eventName == null)
      throw new ArgumentNullException(nameof (eventName));
    TaskCompletionSource<Event> completion = new TaskCompletionSource<Event>();
    node.AddEventListener(eventName, new DomEventHandler(handler), false);
    Event @event;
    try
    {
      @event = await completion.Task.ConfigureAwait(false);
    }
    finally
    {
      node.RemoveEventListener(eventName, new DomEventHandler(handler), false);
    }
    return @event;

    void handler(object s, Event ev) => completion.TrySetResult(ev);
  }
}
