// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.WeakRefTracker
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#nullable disable
namespace IronPython.Runtime;

public class WeakRefTracker
{
  private readonly long _targetId;
  private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
  private readonly List<WeakRefTracker.CallbackInfo> _callbacks = new List<WeakRefTracker.CallbackInfo>(1);

  public WeakRefTracker(IWeakReferenceable target)
  {
    this._targetId = IdDispenser.GetId((object) target);
  }

  public WeakRefTracker(IWeakReferenceable target, object callback, object weakRef)
    : this(target)
  {
    this.ChainCallback(callback, weakRef);
  }

  public long TargetId => this._targetId;

  public void ChainCallback(object callback, object weakRef)
  {
    this._lock.EnterWriteLock();
    try
    {
      this._callbacks.Add(new WeakRefTracker.CallbackInfo(callback, weakRef));
    }
    finally
    {
      this._lock.ExitWriteLock();
    }
  }

  public int HandlerCount
  {
    get
    {
      this._lock.EnterReadLock();
      try
      {
        return this._callbacks.Count;
      }
      finally
      {
        this._lock.ExitReadLock();
      }
    }
  }

  public void RemoveHandlerAt(int index)
  {
    this._lock.EnterWriteLock();
    try
    {
      this._callbacks.RemoveAt(index);
    }
    finally
    {
      this._lock.ExitWriteLock();
    }
  }

  public void RemoveHandler(object o)
  {
    this._lock.EnterWriteLock();
    try
    {
      for (int index = 0; index < this._callbacks.Count; ++index)
      {
        if (this._callbacks[index].WeakRef == o)
        {
          this._callbacks.RemoveAt(index);
          break;
        }
      }
    }
    finally
    {
      this._lock.ExitWriteLock();
    }
  }

  internal bool Contains(object callback, object weakref)
  {
    this._lock.EnterReadLock();
    try
    {
      return this._callbacks.Any<WeakRefTracker.CallbackInfo>((Func<WeakRefTracker.CallbackInfo, bool>) (o => o.Callback == callback && o.WeakRef == weakref));
    }
    finally
    {
      this._lock.ExitReadLock();
    }
  }

  public object GetHandlerCallback(int index)
  {
    this._lock.EnterReadLock();
    try
    {
      return this._callbacks[index].Callback;
    }
    finally
    {
      this._lock.ExitReadLock();
    }
  }

  public object GetWeakRef(int index)
  {
    this._lock.EnterReadLock();
    try
    {
      return this._callbacks[index].WeakRef;
    }
    finally
    {
      this._lock.ExitReadLock();
    }
  }

  ~WeakRefTracker()
  {
    this._lock.EnterWriteLock();
    try
    {
      for (int index = this._callbacks.Count - 1; index >= 0; --index)
      {
        WeakRefTracker.CallbackInfo callback1 = this._callbacks[index];
        try
        {
          if (callback1.Callback != null)
          {
            if (callback1.Callback is InstanceFinalizer callback2)
            {
              callback2.CallDirect(DefaultContext.Default);
            }
            else
            {
              object weakRef = callback1.WeakRef;
              if (weakRef != null)
                PythonCalls.Call(callback1.Callback, weakRef);
            }
          }
        }
        catch (Exception ex)
        {
        }
        callback1.Free();
      }
    }
    finally
    {
      this._lock.ExitWriteLock();
    }
  }

  private readonly struct CallbackInfo(object callback, object weakRef)
  {
    private readonly object _callback = callback;
    private readonly WeakHandle _longRef = new WeakHandle(weakRef, true);
    private readonly WeakHandle _shortRef = new WeakHandle(weakRef, false);

    public object Callback => this._callback;

    public object WeakRef
    {
      get
      {
        object target = this._longRef.Target;
        return this._shortRef.Target != target ? (object) null : target;
      }
    }

    public void Free()
    {
      this._longRef.Free();
      this._shortRef.Free();
    }
  }
}
