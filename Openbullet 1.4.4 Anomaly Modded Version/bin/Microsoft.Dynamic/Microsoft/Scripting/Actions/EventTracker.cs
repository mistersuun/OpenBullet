// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.EventTracker
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class EventTracker : MemberTracker
{
  private WeakDictionary<object, EventTracker.NormalHandlerList> _handlerLists;
  private static readonly object _staticTarget = new object();
  private readonly EventInfo _eventInfo;
  private MethodInfo _addMethod;
  private MethodInfo _removeMethod;

  internal EventTracker(EventInfo eventInfo) => this._eventInfo = eventInfo;

  public override Type DeclaringType => this._eventInfo.DeclaringType;

  public override TrackerTypes MemberType => TrackerTypes.Event;

  public override string Name => this._eventInfo.Name;

  public EventInfo Event => this._eventInfo;

  public MethodInfo GetCallableAddMethod()
  {
    if (this._addMethod == (MethodInfo) null)
      this._addMethod = this._eventInfo.GetAddMethod(true);
    return this._addMethod;
  }

  public MethodInfo GetCallableRemoveMethod()
  {
    if (this._removeMethod == (MethodInfo) null)
      this._removeMethod = this._eventInfo.GetRemoveMethod(true);
    return this._removeMethod;
  }

  public bool IsStatic
  {
    get
    {
      MethodInfo methodInfo = this.Event.GetAddMethod(false);
      if ((object) methodInfo == null)
        methodInfo = this.Event.GetRemoveMethod(false) ?? this.Event.GetRaiseMethod(false) ?? this.Event.GetAddMethod(true) ?? this.Event.GetRemoveMethod(true) ?? this.Event.GetRaiseMethod(true);
      return methodInfo.IsStatic;
    }
  }

  protected internal override DynamicMetaObject GetBoundValue(
    OverloadResolverFactory resolverFactory,
    ActionBinder binder,
    Type type,
    DynamicMetaObject instance)
  {
    return binder.ReturnMemberTracker(type, (MemberTracker) new BoundMemberTracker((MemberTracker) this, instance));
  }

  public override MemberTracker BindToInstance(DynamicMetaObject instance)
  {
    return this.IsStatic ? (MemberTracker) this : (MemberTracker) new BoundMemberTracker((MemberTracker) this, instance);
  }

  public override string ToString() => this._eventInfo.ToString();

  public void AddHandler(object target, object handler, DynamicDelegateCreator delegateCreator)
  {
    ContractUtils.RequiresNotNull(handler, nameof (handler));
    ContractUtils.RequiresNotNull((object) delegateCreator, nameof (delegateCreator));
    Delegate handler1;
    EventTracker.HandlerList handlerList;
    if (this._eventInfo.EventHandlerType.IsAssignableFrom(handler.GetType()))
    {
      handler1 = (Delegate) handler;
      handlerList = (EventTracker.HandlerList) null;
    }
    else
    {
      handler1 = delegateCreator.GetDelegate(handler, this._eventInfo.EventHandlerType);
      handlerList = this.GetHandlerList(target);
    }
    MethodInfo method = this.GetCallableAddMethod();
    if (target != null)
      method = CompilerHelpers.TryGetCallableMethod(target.GetType(), method);
    method.Invoke(target, new object[1]{ (object) handler1 });
    handlerList?.AddHandler(handler, handler1);
  }

  public void RemoveHandler(
    object target,
    object handler,
    IEqualityComparer<object> objectComparer)
  {
    ContractUtils.RequiresNotNull(handler, nameof (handler));
    ContractUtils.RequiresNotNull((object) objectComparer, nameof (objectComparer));
    Delegate @delegate = !this._eventInfo.EventHandlerType.IsAssignableFrom(handler.GetType()) ? this.GetHandlerList(target).RemoveHandler(handler, objectComparer) : (Delegate) handler;
    if ((object) @delegate == null)
      return;
    this.GetCallableRemoveMethod().Invoke(target, new object[1]
    {
      (object) @delegate
    });
  }

  private EventTracker.HandlerList GetHandlerList(object instance)
  {
    if (TypeUtils.IsComObject(instance))
      return this.GetComHandlerList(instance);
    if (this._handlerLists == null)
      Interlocked.CompareExchange<WeakDictionary<object, EventTracker.NormalHandlerList>>(ref this._handlerLists, new WeakDictionary<object, EventTracker.NormalHandlerList>(), (WeakDictionary<object, EventTracker.NormalHandlerList>) null);
    if (instance == null)
      instance = EventTracker._staticTarget;
    lock (this._handlerLists)
    {
      EventTracker.NormalHandlerList handlerList;
      if (this._handlerLists.TryGetValue(instance, out handlerList))
        return (EventTracker.HandlerList) handlerList;
      handlerList = new EventTracker.NormalHandlerList();
      this._handlerLists[instance] = handlerList;
      return (EventTracker.HandlerList) handlerList;
    }
  }

  private EventTracker.HandlerList GetComHandlerList(object instance)
  {
    EventTracker.HandlerList data = (EventTracker.HandlerList) Marshal.GetComObjectData(instance, (object) this);
    if (data == null)
    {
      lock (EventTracker._staticTarget)
      {
        data = (EventTracker.HandlerList) Marshal.GetComObjectData(instance, (object) this);
        if (data == null)
        {
          data = (EventTracker.HandlerList) new EventTracker.ComHandlerList();
          if (!Marshal.SetComObjectData(instance, (object) this, (object) data))
            throw new COMException("Failed to set COM Object Data");
        }
      }
    }
    return data;
  }

  private abstract class HandlerList
  {
    public abstract void AddHandler(object callableObject, Delegate handler);

    public abstract Delegate RemoveHandler(
      object callableObject,
      IEqualityComparer<object> comparer);
  }

  private sealed class ComHandlerList : EventTracker.HandlerList
  {
    private readonly CopyOnWriteList<KeyValuePair<object, object>> _handlers = new CopyOnWriteList<KeyValuePair<object, object>>();

    public override void AddHandler(object callableObject, Delegate handler)
    {
      this._handlers.Add(new KeyValuePair<object, object>(callableObject, (object) handler));
    }

    public override Delegate RemoveHandler(
      object callableObject,
      IEqualityComparer<object> comparer)
    {
      List<KeyValuePair<object, object>> copyForRead = this._handlers.GetCopyForRead();
      for (int index = copyForRead.Count - 1; index >= 0; --index)
      {
        object key = copyForRead[index].Key;
        object obj = copyForRead[index].Value;
        if (comparer.Equals(key, callableObject))
        {
          Delegate @delegate = (Delegate) obj;
          this._handlers.RemoveAt(index);
          return @delegate;
        }
      }
      return (Delegate) null;
    }
  }

  private sealed class NormalHandlerList : EventTracker.HandlerList
  {
    private readonly CopyOnWriteList<KeyValuePair<WeakReference, WeakReference>> _handlers = new CopyOnWriteList<KeyValuePair<WeakReference, WeakReference>>();

    public override void AddHandler(object callableObject, Delegate handler)
    {
      this._handlers.Add(new KeyValuePair<WeakReference, WeakReference>(new WeakReference(callableObject), new WeakReference((object) handler)));
    }

    public override Delegate RemoveHandler(
      object callableObject,
      IEqualityComparer<object> comparer)
    {
      List<KeyValuePair<WeakReference, WeakReference>> copyForRead = this._handlers.GetCopyForRead();
      for (int index = copyForRead.Count - 1; index >= 0; --index)
      {
        object target1 = copyForRead[index].Key.Target;
        object target2 = copyForRead[index].Value.Target;
        if (target1 != null && target2 != null && comparer.Equals(target1, callableObject))
        {
          Delegate @delegate = (Delegate) target2;
          this._handlers.RemoveAt(index);
          return @delegate;
        }
      }
      return (Delegate) null;
    }
  }
}
