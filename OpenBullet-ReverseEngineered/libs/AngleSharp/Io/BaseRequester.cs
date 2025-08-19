// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.BaseRequester
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Io;

public abstract class BaseRequester : EventTarget, IRequester, IEventTarget
{
  public event DomEventHandler Requesting
  {
    add => this.AddEventListener(EventNames.Requesting, value, false);
    remove => this.RemoveEventListener(EventNames.Requesting, value, false);
  }

  public event DomEventHandler Requested
  {
    add => this.AddEventListener(EventNames.Requested, value, false);
    remove => this.RemoveEventListener(EventNames.Requested, value, false);
  }

  public async Task<IResponse> RequestAsync(Request request, CancellationToken cancel)
  {
    BaseRequester baseRequester = this;
    RequestEvent ev1 = new RequestEvent(request, (IResponse) null);
    // ISSUE: explicit non-virtual call
    __nonvirtual (baseRequester.InvokeEventListener((Event) ev1));
    IResponse response = await baseRequester.PerformRequestAsync(request, cancel);
    RequestEvent ev2 = new RequestEvent(request, response);
    // ISSUE: explicit non-virtual call
    __nonvirtual (baseRequester.InvokeEventListener((Event) ev2));
    return response;
  }

  public abstract bool SupportsProtocol(string protocol);

  protected abstract Task<IResponse> PerformRequestAsync(Request request, CancellationToken cancel);
}
