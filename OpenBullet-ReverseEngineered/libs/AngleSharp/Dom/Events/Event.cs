// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.Events.Event
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Attributes;
using AngleSharp.Html;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Dom.Events;

[DomName("Event")]
public class Event : EventArgs
{
  private EventFlags _flags;
  private EventPhase _phase;
  private IEventTarget _current;
  private IEventTarget _target;
  private bool _bubbles;
  private bool _cancelable;
  private string _type;
  private DateTime _time;

  public Event()
  {
    this._flags = EventFlags.None;
    this._phase = EventPhase.None;
    this._time = DateTime.Now;
  }

  [DomConstructor]
  [DomInitDict(1, true)]
  public Event(string type, bool bubbles = false, bool cancelable = false)
    : this()
  {
    this.Init(type, bubbles, cancelable);
  }

  internal EventFlags Flags => this._flags;

  [DomName("type")]
  public string Type => this._type;

  [DomName("target")]
  public IEventTarget OriginalTarget => this._target;

  [DomName("currentTarget")]
  public IEventTarget CurrentTarget => this._current;

  [DomName("eventPhase")]
  public EventPhase Phase => this._phase;

  [DomName("bubbles")]
  public bool IsBubbling => this._bubbles;

  [DomName("cancelable")]
  public bool IsCancelable => this._cancelable;

  [DomName("defaultPrevented")]
  public bool IsDefaultPrevented => (this._flags & EventFlags.Canceled) == EventFlags.Canceled;

  [DomName("isTrusted")]
  public bool IsTrusted { get; internal set; }

  [DomName("timeStamp")]
  public DateTime Time => this._time;

  [DomName("stopPropagation")]
  public void Stop() => this._flags |= EventFlags.StopPropagation;

  [DomName("stopImmediatePropagation")]
  public void StopImmediately() => this._flags |= EventFlags.StopImmediatePropagation;

  [DomName("preventDefault")]
  public void Cancel()
  {
    if (!this._cancelable)
      return;
    this._flags |= EventFlags.Canceled;
  }

  [DomName("initEvent")]
  public void Init(string type, bool bubbles, bool cancelable)
  {
    this._flags |= EventFlags.Initialized;
    if ((this._flags & EventFlags.Dispatch) == EventFlags.Dispatch)
      return;
    this._flags &= ~(EventFlags.StopPropagation | EventFlags.StopImmediatePropagation | EventFlags.Canceled);
    this.IsTrusted = false;
    this._target = (IEventTarget) null;
    this._type = type;
    this._bubbles = bubbles;
    this._cancelable = cancelable;
  }

  internal bool Dispatch(IEventTarget target)
  {
    this._flags |= EventFlags.Dispatch;
    this._target = target;
    List<IEventTarget> eventTargetList = new List<IEventTarget>();
    if (target is AngleSharp.Dom.Node node)
    {
      while ((node = node.Parent) != null)
        eventTargetList.Add((IEventTarget) node);
    }
    this._phase = EventPhase.Capturing;
    this.DispatchAt(eventTargetList.Reverse<IEventTarget>());
    this._phase = EventPhase.AtTarget;
    if ((this._flags & EventFlags.StopPropagation) != EventFlags.StopPropagation)
      this.CallListeners(target);
    if (this._bubbles)
    {
      this._phase = EventPhase.Bubbling;
      this.DispatchAt((IEnumerable<IEventTarget>) eventTargetList);
    }
    this._flags &= ~EventFlags.Dispatch;
    this._phase = EventPhase.None;
    this._current = (IEventTarget) null;
    return (this._flags & EventFlags.Canceled) == EventFlags.Canceled;
  }

  private void CallListeners(IEventTarget target)
  {
    this._current = target;
    target.InvokeEventListener(this);
  }

  private void DispatchAt(IEnumerable<IEventTarget> targets)
  {
    foreach (IEventTarget target in targets)
    {
      this.CallListeners(target);
      if ((this._flags & EventFlags.StopPropagation) == EventFlags.StopPropagation)
        break;
    }
  }
}
