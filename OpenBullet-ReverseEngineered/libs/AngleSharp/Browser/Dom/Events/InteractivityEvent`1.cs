// Decompiled with JetBrains decompiler
// Type: AngleSharp.Browser.Dom.Events.InteractivityEvent`1
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom.Events;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Browser.Dom.Events;

public class InteractivityEvent<T> : Event
{
  private Task _result;

  public InteractivityEvent(string eventName, T data)
    : base(eventName)
  {
    this.Data = data;
  }

  public Task Result => this._result;

  public void SetResult(Task value)
  {
    if (this._result != null)
      this._result = Task.WhenAll(new Task[2]
      {
        this._result,
        value
      });
    else
      this._result = value;
  }

  public T Data { get; private set; }
}
