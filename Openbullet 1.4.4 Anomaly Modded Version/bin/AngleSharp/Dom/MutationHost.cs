// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.MutationHost
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Browser;
using System;
using System.Collections.Generic;

#nullable disable
namespace AngleSharp.Dom;

internal sealed class MutationHost
{
  private readonly List<MutationObserver> _observers;
  private readonly IEventLoop _loop;
  private bool _queued;

  public MutationHost(IEventLoop loop)
  {
    this._observers = new List<MutationObserver>();
    this._queued = false;
    this._loop = loop;
  }

  public IEnumerable<MutationObserver> Observers => (IEnumerable<MutationObserver>) this._observers;

  public void Register(MutationObserver observer)
  {
    if (this._observers.Contains(observer))
      return;
    this._observers.Add(observer);
  }

  public void Unregister(MutationObserver observer)
  {
    if (!this._observers.Contains(observer))
      return;
    this._observers.Remove(observer);
  }

  public void ScheduleCallback()
  {
    if (this._queued)
      return;
    this._queued = true;
    this._loop.Enqueue(new Action(this.DispatchCallback));
  }

  private void DispatchCallback()
  {
    MutationObserver[] array = this._observers.ToArray();
    this._queued = false;
    foreach (MutationObserver mutationObserver in array)
      this._loop.Enqueue(new Action(mutationObserver.Trigger), TaskPriority.Microtask);
  }
}
