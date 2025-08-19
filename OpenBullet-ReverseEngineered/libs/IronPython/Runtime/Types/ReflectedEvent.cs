// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.ReflectedEvent
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Generation;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("event#")]
public sealed class ReflectedEvent : PythonTypeDataSlot, ICodeFormattable
{
  private readonly bool _clsOnly;
  private readonly EventTracker _tracker;

  internal ReflectedEvent(EventTracker tracker, bool clsOnly)
  {
    this._clsOnly = clsOnly;
    this._tracker = tracker;
  }

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    value = (object) new ReflectedEvent.BoundEvent(this, instance, owner);
    return true;
  }

  internal override bool GetAlwaysSucceeds => true;

  internal override bool TrySetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    object value)
  {
    switch (value)
    {
      case ReflectedEvent.BoundEvent et when !this.EventInfosDiffer(et):
        return true;
      case ReflectedEvent.BadEventChange badEventChange:
        PythonType owner1 = badEventChange.Owner;
        if (owner1 != null)
        {
          if (badEventChange.Instance == null)
            throw new MissingMemberException(string.Format("attribute '{1}' of '{0}' object is read-only", (object) owner1.Name, (object) this._tracker.Name));
          throw new MissingMemberException($"'{owner1.Name}' object has no attribute '{this._tracker.Name}'");
        }
        break;
    }
    throw this.ReadOnlyException(DynamicHelpers.GetPythonTypeFromType(this.Info.DeclaringType));
  }

  private bool EventInfosDiffer(ReflectedEvent.BoundEvent et)
  {
    return !(et.Event.Info == this.Info) && (et.Event.Info.DeclaringType != this.Info.DeclaringType || et.Event.Info.MetadataToken != this.Info.MetadataToken);
  }

  internal override bool TryDeleteValue(CodeContext context, object instance, PythonType owner)
  {
    throw this.ReadOnlyException(DynamicHelpers.GetPythonTypeFromType(this.Info.DeclaringType));
  }

  internal override bool IsAlwaysVisible => !this._clsOnly;

  public string __doc__ => DocBuilder.CreateAutoDoc(this._tracker.Event);

  public EventInfo Info
  {
    [PythonHidden(new PlatformID[] {})] get => this._tracker.Event;
  }

  public EventTracker Tracker
  {
    [PythonHidden(new PlatformID[] {})] get => this._tracker;
  }

  private MissingMemberException ReadOnlyException(PythonType dt)
  {
    return new MissingMemberException(string.Format("attribute '{1}' of '{0}' object is read-only", (object) dt.Name, (object) this._tracker.Name));
  }

  public string __repr__(CodeContext context)
  {
    return $"<event# {this.Info.Name} on {this.Info.DeclaringType.Name}>";
  }

  public class BoundEvent
  {
    private readonly ReflectedEvent _event;
    private readonly PythonType _ownerType;
    private readonly object _instance;

    public ReflectedEvent Event => this._event;

    public BoundEvent(ReflectedEvent reflectedEvent, object instance, PythonType ownerType)
    {
      this._event = reflectedEvent;
      this._instance = instance;
      this._ownerType = ownerType;
    }

    [SpecialName]
    public object operator op_AdditionAssignment(CodeContext context, object func)
    {
      return this.InPlaceAdd(context, func);
    }

    [SpecialName]
    public object InPlaceAdd(CodeContext context, object func)
    {
      if (func == null || !PythonOps.IsCallable(context, func))
        throw PythonOps.TypeError("event addition expected callable object, got {0}", (object) PythonTypeOps.GetName(func));
      if (this._event.Tracker.IsStatic && this._ownerType != DynamicHelpers.GetPythonTypeFromType(this._event.Tracker.DeclaringType))
        return (object) new ReflectedEvent.BadEventChange(this._ownerType, this._instance);
      MethodInfo methodInfo = this._event.Tracker.GetCallableAddMethod();
      if (this._instance != null)
        methodInfo = CompilerHelpers.TryGetCallableMethod(this._instance.GetType(), methodInfo);
      if (!CompilerHelpers.IsVisible((MethodBase) methodInfo) && !methodInfo.IsProtected() && !context.LanguageContext.DomainManager.Configuration.PrivateBinding)
        throw new TypeErrorException("Cannot add handler to a private event.");
      this._event.Tracker.AddHandler(this._instance, func, context.LanguageContext.DelegateCreator);
      return (object) this;
    }

    [SpecialName]
    public object InPlaceSubtract(CodeContext context, object func)
    {
      if (func == null)
        throw PythonOps.TypeError("event subtraction expected callable object, got None");
      if (this._event.Tracker.IsStatic && this._ownerType != DynamicHelpers.GetPythonTypeFromType(this._event.Tracker.DeclaringType))
        return (object) new ReflectedEvent.BadEventChange(this._ownerType, this._instance);
      MethodInfo callableRemoveMethod = this._event.Tracker.GetCallableRemoveMethod();
      if (!CompilerHelpers.IsVisible((MethodBase) callableRemoveMethod) && !callableRemoveMethod.IsProtected() && !context.LanguageContext.DomainManager.Configuration.PrivateBinding)
        throw new TypeErrorException("Cannot remove handler from a private event.");
      this._event.Tracker.RemoveHandler(this._instance, func, context.LanguageContext.EqualityComparer);
      return (object) this;
    }
  }

  private class BadEventChange
  {
    private readonly PythonType _ownerType;
    private readonly object _instance;

    public BadEventChange(PythonType ownerType, object instance)
    {
      this._ownerType = ownerType;
      this._instance = instance;
    }

    public PythonType Owner => this._ownerType;

    public object Instance => this._instance;
  }
}
