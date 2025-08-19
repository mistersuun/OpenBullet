// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Events.RequestEvent
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Io;

#nullable disable
namespace AngleSharp.Dom.Events;

public class RequestEvent : Event
{
  public RequestEvent(Request request, IResponse response)
    : base(response != null ? EventNames.Requested : EventNames.Requesting)
  {
    this.Response = response;
    this.Request = request;
  }

  public Request Request { get; private set; }

  public IResponse Response { get; private set; }
}
