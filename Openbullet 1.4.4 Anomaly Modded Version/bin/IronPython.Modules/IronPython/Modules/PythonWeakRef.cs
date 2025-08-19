// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonWeakRef
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Modules;

public static class PythonWeakRef
{
  public const string __doc__ = "Provides support for creating weak references and proxies to objects";
  public static readonly PythonType CallableProxyType = DynamicHelpers.GetPythonTypeFromType(typeof (PythonWeakRef.weakcallableproxy));
  public static readonly PythonType ProxyType = DynamicHelpers.GetPythonTypeFromType(typeof (PythonWeakRef.weakproxy));
  public static readonly PythonType ReferenceType = DynamicHelpers.GetPythonTypeFromType(typeof (PythonWeakRef.@ref));

  internal static IWeakReferenceable ConvertToWeakReferenceable(PythonContext context, object obj)
  {
    return context.ConvertToWeakReferenceable(obj);
  }

  public static int getweakrefcount(CodeContext context, object @object)
  {
    return PythonWeakRef.@ref.GetWeakRefCount(context.LanguageContext, @object);
  }

  public static IronPython.Runtime.List getweakrefs(CodeContext context, object @object)
  {
    return PythonWeakRef.@ref.GetWeakRefs(context.LanguageContext, @object);
  }

  public static object proxy(CodeContext context, object @object)
  {
    return PythonWeakRef.proxy(context, @object, (object) null);
  }

  public static object proxy(CodeContext context, object @object, object callback)
  {
    return PythonOps.IsCallable(context, @object) ? PythonWeakRef.weakcallableproxy.MakeNew(context, @object, callback) : PythonWeakRef.weakproxy.MakeNew(context, @object, callback);
  }

  public static void _remove_dead_weakref(CodeContext context, PythonDictionary dict, object key)
  {
    dict.TryRemoveValue(key, out object _);
  }

  [PythonType]
  public class @ref : IStructuralEquatable
  {
    private CodeContext _context;
    private WeakHandle _target;
    private long _targetId;
    private int _hashVal;
    private bool _fHasHash;

    public static object __new__(CodeContext context, PythonType cls, object @object)
    {
      IWeakReferenceable weakReferenceable = PythonWeakRef.ConvertToWeakReferenceable(context.LanguageContext, @object);
      if (cls != DynamicHelpers.GetPythonTypeFromType(typeof (PythonWeakRef.@ref)))
        return cls.CreateInstance(context, @object);
      WeakRefTracker weakRef = weakReferenceable.GetWeakRef();
      if (weakRef != null)
      {
        for (int index = 0; index < weakRef.HandlerCount; ++index)
        {
          if (weakRef.GetHandlerCallback(index) == null && weakRef.GetWeakRef(index) is PythonWeakRef.@ref)
            return weakRef.GetWeakRef(index);
        }
      }
      return (object) new PythonWeakRef.@ref(context, @object);
    }

    public static object __new__(
      CodeContext context,
      PythonType cls,
      object @object,
      object callback)
    {
      if (callback == null)
        return PythonWeakRef.@ref.__new__(context, cls, @object);
      return cls == DynamicHelpers.GetPythonTypeFromType(typeof (PythonWeakRef.@ref)) ? (object) new PythonWeakRef.@ref(context, @object, callback) : cls.CreateInstance(context, @object, callback);
    }

    public void __init__(CodeContext context, object ob, object callback = null)
    {
      this._context = context;
      WeakRefTracker weakRefTracker = PythonWeakRef.WeakRefHelpers.InitializeWeakRef(this._context.LanguageContext, (object) this, ob, callback);
      this._target = new WeakHandle(ob, false);
      this._targetId = weakRefTracker.TargetId;
    }

    public @ref(CodeContext context, object @object, object callback = null)
    {
    }

    ~@ref()
    {
      IWeakReferenceable weakref;
      if (this._context.LanguageContext.TryConvertToWeakReferenceable(this._target.Target, out weakref))
        weakref.GetWeakRef()?.RemoveHandler((object) this);
      this._target.Free();
    }

    internal static int GetWeakRefCount(PythonContext context, object o)
    {
      IWeakReferenceable weakref;
      if (context.TryConvertToWeakReferenceable(o, out weakref))
      {
        WeakRefTracker weakRef = weakref.GetWeakRef();
        if (weakRef != null)
          return weakRef.HandlerCount;
      }
      return 0;
    }

    internal static IronPython.Runtime.List GetWeakRefs(PythonContext context, object o)
    {
      IronPython.Runtime.List weakRefs = new IronPython.Runtime.List();
      IWeakReferenceable weakref;
      if (context.TryConvertToWeakReferenceable(o, out weakref))
      {
        WeakRefTracker weakRef = weakref.GetWeakRef();
        if (weakRef != null)
        {
          for (int index = 0; index < weakRef.HandlerCount; ++index)
            weakRefs.AddNoLock(weakRef.GetWeakRef(index));
        }
      }
      return weakRefs;
    }

    [SpecialName]
    public object Call(CodeContext context)
    {
      object target = this._target.Target;
      GC.KeepAlive((object) this);
      return target;
    }

    public static NotImplementedType operator >(PythonWeakRef.@ref self, object other)
    {
      return PythonOps.NotImplemented;
    }

    public static NotImplementedType operator <(PythonWeakRef.@ref self, object other)
    {
      return PythonOps.NotImplemented;
    }

    public static NotImplementedType operator <=(PythonWeakRef.@ref self, object other)
    {
      return PythonOps.NotImplemented;
    }

    public static NotImplementedType operator >=(PythonWeakRef.@ref self, object other)
    {
      return PythonOps.NotImplemented;
    }

    public int __hash__(CodeContext context)
    {
      if (!this._fHasHash)
      {
        this._hashVal = context.LanguageContext.EqualityComparerNonGeneric.GetHashCode(this._target.Target ?? throw PythonOps.TypeError("weak object has gone away"));
        this._fHasHash = true;
      }
      GC.KeepAlive((object) this);
      return this._hashVal;
    }

    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    {
      if (!this._fHasHash)
      {
        object target = this._target.Target;
        this._hashVal = comparer.GetHashCode(target);
        this._fHasHash = true;
      }
      GC.KeepAlive((object) this);
      return this._hashVal;
    }

    bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
    {
      return this.EqualsWorker(other, comparer);
    }

    private bool EqualsWorker(object other, IEqualityComparer comparer)
    {
      if (this == other)
        return true;
      bool flag = false;
      if (other is PythonWeakRef.@ref @ref)
      {
        object target1 = this._target.Target;
        object target2 = @ref._target.Target;
        flag = target1 == null || target2 == null ? this._targetId == @ref._targetId : PythonWeakRef.@ref.RefEquals(target1, target2, comparer);
      }
      GC.KeepAlive((object) this);
      return flag;
    }

    private static bool RefEquals(object x, object y, IEqualityComparer comparer)
    {
      CodeContext context = comparer == null || !(comparer is PythonContext.PythonEqualityComparer) ? DefaultContext.Default : ((PythonContext.PythonEqualityComparer) comparer).Context.SharedContext;
      object obj;
      if (PythonTypeOps.TryInvokeBinaryOperator(context, x, y, "__eq__", out obj) && obj != NotImplementedType.Value)
        return (bool) obj;
      if (PythonTypeOps.TryInvokeBinaryOperator(context, y, x, "__eq__", out obj) && obj != NotImplementedType.Value)
        return (bool) obj;
      return comparer != null ? comparer.Equals(x, y) : x.Equals(y);
    }
  }

  [PythonType]
  [DynamicBaseType]
  [PythonHidden(new PlatformID[] {})]
  public sealed class weakproxy : 
    IPythonObject,
    ICodeFormattable,
    IProxyObject,
    IPythonMembersList,
    IMembersList,
    IStructuralEquatable
  {
    private readonly WeakHandle _target;
    private readonly CodeContext _context;
    public const object __hash__ = null;
    [SlotField]
    public static PythonTypeSlot __add__ = (PythonTypeSlot) new SlotWrapper(nameof (__add__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __radd__ = (PythonTypeSlot) new SlotWrapper(nameof (__radd__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __iadd__ = (PythonTypeSlot) new SlotWrapper(nameof (__iadd__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __sub__ = (PythonTypeSlot) new SlotWrapper(nameof (__sub__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __rsub__ = (PythonTypeSlot) new SlotWrapper(nameof (__rsub__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __isub__ = (PythonTypeSlot) new SlotWrapper(nameof (__isub__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __pow__ = (PythonTypeSlot) new SlotWrapper(nameof (__pow__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __rpow__ = (PythonTypeSlot) new SlotWrapper(nameof (__rpow__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __ipow__ = (PythonTypeSlot) new SlotWrapper(nameof (__ipow__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __mul__ = (PythonTypeSlot) new SlotWrapper(nameof (__mul__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __rmul__ = (PythonTypeSlot) new SlotWrapper(nameof (__rmul__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __imul__ = (PythonTypeSlot) new SlotWrapper(nameof (__imul__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __floordiv__ = (PythonTypeSlot) new SlotWrapper(nameof (__floordiv__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __rfloordiv__ = (PythonTypeSlot) new SlotWrapper(nameof (__rfloordiv__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __ifloordiv__ = (PythonTypeSlot) new SlotWrapper(nameof (__ifloordiv__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __div__ = (PythonTypeSlot) new SlotWrapper(nameof (__div__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __rdiv__ = (PythonTypeSlot) new SlotWrapper(nameof (__rdiv__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __idiv__ = (PythonTypeSlot) new SlotWrapper(nameof (__idiv__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __truediv__ = (PythonTypeSlot) new SlotWrapper(nameof (__truediv__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __rtruediv__ = (PythonTypeSlot) new SlotWrapper(nameof (__rtruediv__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __itruediv__ = (PythonTypeSlot) new SlotWrapper(nameof (__itruediv__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __mod__ = (PythonTypeSlot) new SlotWrapper(nameof (__mod__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __rmod__ = (PythonTypeSlot) new SlotWrapper(nameof (__rmod__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __imod__ = (PythonTypeSlot) new SlotWrapper(nameof (__imod__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __lshift__ = (PythonTypeSlot) new SlotWrapper(nameof (__lshift__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __rlshift__ = (PythonTypeSlot) new SlotWrapper(nameof (__rlshift__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __ilshift__ = (PythonTypeSlot) new SlotWrapper(nameof (__ilshift__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __rshift__ = (PythonTypeSlot) new SlotWrapper(nameof (__rshift__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __rrshift__ = (PythonTypeSlot) new SlotWrapper(nameof (__rrshift__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __irshift__ = (PythonTypeSlot) new SlotWrapper(nameof (__irshift__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __and__ = (PythonTypeSlot) new SlotWrapper(nameof (__and__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __rand__ = (PythonTypeSlot) new SlotWrapper(nameof (__rand__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __iand__ = (PythonTypeSlot) new SlotWrapper(nameof (__iand__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __or__ = (PythonTypeSlot) new SlotWrapper(nameof (__or__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __ror__ = (PythonTypeSlot) new SlotWrapper(nameof (__ror__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __ior__ = (PythonTypeSlot) new SlotWrapper(nameof (__ior__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __xor__ = (PythonTypeSlot) new SlotWrapper(nameof (__xor__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __rxor__ = (PythonTypeSlot) new SlotWrapper(nameof (__rxor__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __ixor__ = (PythonTypeSlot) new SlotWrapper(nameof (__ixor__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __delslice__ = (PythonTypeSlot) new SlotWrapper(nameof (__delslice__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __divmod__ = (PythonTypeSlot) new SlotWrapper(nameof (__divmod__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __float__ = (PythonTypeSlot) new SlotWrapper(nameof (__float__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __getslice__ = (PythonTypeSlot) new SlotWrapper(nameof (__getslice__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __index__ = (PythonTypeSlot) new SlotWrapper(nameof (__index__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __int__ = (PythonTypeSlot) new SlotWrapper(nameof (__int__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __iter__ = (PythonTypeSlot) new SlotWrapper(nameof (__iter__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __long__ = (PythonTypeSlot) new SlotWrapper(nameof (__long__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __rdivmod__ = (PythonTypeSlot) new SlotWrapper(nameof (__rdivmod__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __setslice__ = (PythonTypeSlot) new SlotWrapper(nameof (__setslice__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot next = (PythonTypeSlot) new SlotWrapper(nameof (next), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __getitem__ = (PythonTypeSlot) new SlotWrapper(nameof (__getitem__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __setitem__ = (PythonTypeSlot) new SlotWrapper(nameof (__setitem__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __delitem__ = (PythonTypeSlot) new SlotWrapper(nameof (__delitem__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __len__ = (PythonTypeSlot) new SlotWrapper(nameof (__len__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __pos__ = (PythonTypeSlot) new SlotWrapper(nameof (__pos__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __neg__ = (PythonTypeSlot) new SlotWrapper(nameof (__neg__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __invert__ = (PythonTypeSlot) new SlotWrapper(nameof (__invert__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __contains__ = (PythonTypeSlot) new SlotWrapper(nameof (__contains__), PythonWeakRef.ProxyType);
    [SlotField]
    public static PythonTypeSlot __abs__ = (PythonTypeSlot) new SlotWrapper(nameof (__abs__), PythonWeakRef.ProxyType);

    internal static object MakeNew(CodeContext context, object @object, object callback)
    {
      IWeakReferenceable weakReferenceable = PythonWeakRef.ConvertToWeakReferenceable(context.LanguageContext, @object);
      if (callback == null)
      {
        WeakRefTracker weakRef = weakReferenceable.GetWeakRef();
        if (weakRef != null)
        {
          for (int index = 0; index < weakRef.HandlerCount; ++index)
          {
            if (weakRef.GetHandlerCallback(index) == null && weakRef.GetWeakRef(index) is PythonWeakRef.weakproxy)
              return weakRef.GetWeakRef(index);
          }
        }
      }
      return (object) new PythonWeakRef.weakproxy(context, @object, callback);
    }

    private weakproxy(CodeContext context, object target, object callback)
    {
      PythonWeakRef.WeakRefHelpers.InitializeWeakRef(context.LanguageContext, (object) this, target, callback);
      this._target = new WeakHandle(target, false);
      this._context = context;
    }

    ~weakproxy()
    {
      IWeakReferenceable weakref;
      if (this._context.LanguageContext.TryConvertToWeakReferenceable(this._target.Target, out weakref))
        weakref.GetWeakRef().RemoveHandler((object) this);
      this._target.Free();
    }

    private object GetObject()
    {
      object result;
      if (!this.TryGetObject(out result))
        throw PythonOps.ReferenceError("weakly referenced object no longer exists");
      return result;
    }

    private bool TryGetObject(out object result)
    {
      result = this._target.Target;
      if (result == null)
        return false;
      GC.KeepAlive((object) this);
      return true;
    }

    PythonDictionary IPythonObject.Dict
    {
      get
      {
        return this.GetObject() is IPythonObject pythonObject ? pythonObject.Dict : (PythonDictionary) null;
      }
    }

    PythonDictionary IPythonObject.SetDict(PythonDictionary dict)
    {
      return (this.GetObject() as IPythonObject).SetDict(dict);
    }

    bool IPythonObject.ReplaceDict(PythonDictionary dict)
    {
      return (this.GetObject() as IPythonObject).ReplaceDict(dict);
    }

    void IPythonObject.SetPythonType(PythonType newType)
    {
      (this.GetObject() as IPythonObject).SetPythonType(newType);
    }

    PythonType IPythonObject.PythonType
    {
      get => DynamicHelpers.GetPythonTypeFromType(typeof (PythonWeakRef.weakproxy));
    }

    object[] IPythonObject.GetSlots() => (object[]) null;

    object[] IPythonObject.GetSlotsCreate() => (object[]) null;

    public override string ToString() => PythonOps.ToString(this.GetObject());

    public string __repr__(CodeContext context)
    {
      object target = this._target.Target;
      GC.KeepAlive((object) this);
      return $"<weakproxy at {IdDispenser.GetId((object) this)} to {PythonOps.GetPythonTypeName(target)} at {IdDispenser.GetId(target)}>";
    }

    [SpecialName]
    public object GetCustomMember(CodeContext context, string name)
    {
      object o = this.GetObject();
      object ret;
      return PythonOps.TryGetBoundAttr(context, o, name, out ret) ? ret : (object) OperationFailed.Value;
    }

    [SpecialName]
    public void SetMember(CodeContext context, string name, object value)
    {
      object o = this.GetObject();
      PythonOps.SetAttr(context, o, name, value);
    }

    [SpecialName]
    public void DeleteMember(CodeContext context, string name)
    {
      object o = this.GetObject();
      PythonOps.DeleteAttr(context, o, name);
    }

    IList<string> IMembersList.GetMemberNames()
    {
      return PythonOps.GetStringMemberList((IPythonMembersList) this);
    }

    IList<object> IPythonMembersList.GetMemberNames(CodeContext context)
    {
      object result;
      return !this.TryGetObject(out result) ? (IList<object>) new IronPython.Runtime.List() : PythonOps.GetAttrNames(context, result);
    }

    object IProxyObject.Target => this.GetObject();

    private bool EqualsWorker(PythonWeakRef.weakproxy other)
    {
      return PythonOps.EqualRetBool(this._context, this.GetObject(), other.GetObject());
    }

    [return: MaybeNotImplemented]
    public object __eq__(object other)
    {
      return !(other is PythonWeakRef.weakproxy) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(this.EqualsWorker((PythonWeakRef.weakproxy) other));
    }

    [return: MaybeNotImplemented]
    public object __ne__(object other)
    {
      return !(other is PythonWeakRef.weakproxy) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(!this.EqualsWorker((PythonWeakRef.weakproxy) other));
    }

    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    {
      object result;
      return this.TryGetObject(out result) ? comparer.GetHashCode(result) : comparer.GetHashCode((object) null);
    }

    bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
    {
      object result1;
      if (!this.TryGetObject(out result1))
        result1 = (object) null;
      if (!(other is PythonWeakRef.weakproxy))
        return comparer.Equals(result1, other);
      object result2;
      if (!this.TryGetObject(out result2))
        result2 = (object) null;
      return comparer.Equals(result1, result2);
    }

    public object __nonzero__() => (object) Converter.ConvertToBoolean(this.GetObject());

    public static explicit operator bool(PythonWeakRef.weakproxy self)
    {
      return Converter.ConvertToBoolean(self.GetObject());
    }
  }

  [PythonType]
  [DynamicBaseType]
  [PythonHidden(new PlatformID[] {})]
  public sealed class weakcallableproxy : 
    IPythonObject,
    ICodeFormattable,
    IProxyObject,
    IStructuralEquatable,
    IPythonMembersList,
    IMembersList
  {
    private WeakHandle _target;
    private readonly CodeContext _context;
    public const object __hash__ = null;
    [SlotField]
    public static PythonTypeSlot __add__ = (PythonTypeSlot) new SlotWrapper(nameof (__add__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __radd__ = (PythonTypeSlot) new SlotWrapper(nameof (__radd__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __iadd__ = (PythonTypeSlot) new SlotWrapper(nameof (__iadd__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __sub__ = (PythonTypeSlot) new SlotWrapper(nameof (__sub__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __rsub__ = (PythonTypeSlot) new SlotWrapper(nameof (__rsub__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __isub__ = (PythonTypeSlot) new SlotWrapper(nameof (__isub__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __pow__ = (PythonTypeSlot) new SlotWrapper(nameof (__pow__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __rpow__ = (PythonTypeSlot) new SlotWrapper(nameof (__rpow__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __ipow__ = (PythonTypeSlot) new SlotWrapper(nameof (__ipow__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __mul__ = (PythonTypeSlot) new SlotWrapper(nameof (__mul__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __rmul__ = (PythonTypeSlot) new SlotWrapper(nameof (__rmul__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __imul__ = (PythonTypeSlot) new SlotWrapper(nameof (__imul__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __floordiv__ = (PythonTypeSlot) new SlotWrapper(nameof (__floordiv__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __rfloordiv__ = (PythonTypeSlot) new SlotWrapper(nameof (__rfloordiv__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __ifloordiv__ = (PythonTypeSlot) new SlotWrapper(nameof (__ifloordiv__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __div__ = (PythonTypeSlot) new SlotWrapper(nameof (__div__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __rdiv__ = (PythonTypeSlot) new SlotWrapper(nameof (__rdiv__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __idiv__ = (PythonTypeSlot) new SlotWrapper(nameof (__idiv__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __truediv__ = (PythonTypeSlot) new SlotWrapper(nameof (__truediv__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __rtruediv__ = (PythonTypeSlot) new SlotWrapper(nameof (__rtruediv__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __itruediv__ = (PythonTypeSlot) new SlotWrapper(nameof (__itruediv__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __mod__ = (PythonTypeSlot) new SlotWrapper(nameof (__mod__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __rmod__ = (PythonTypeSlot) new SlotWrapper(nameof (__rmod__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __imod__ = (PythonTypeSlot) new SlotWrapper(nameof (__imod__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __lshift__ = (PythonTypeSlot) new SlotWrapper(nameof (__lshift__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __rlshift__ = (PythonTypeSlot) new SlotWrapper(nameof (__rlshift__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __ilshift__ = (PythonTypeSlot) new SlotWrapper(nameof (__ilshift__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __rshift__ = (PythonTypeSlot) new SlotWrapper(nameof (__rshift__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __rrshift__ = (PythonTypeSlot) new SlotWrapper(nameof (__rrshift__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __irshift__ = (PythonTypeSlot) new SlotWrapper(nameof (__irshift__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __and__ = (PythonTypeSlot) new SlotWrapper(nameof (__and__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __rand__ = (PythonTypeSlot) new SlotWrapper(nameof (__rand__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __iand__ = (PythonTypeSlot) new SlotWrapper(nameof (__iand__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __or__ = (PythonTypeSlot) new SlotWrapper(nameof (__or__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __ror__ = (PythonTypeSlot) new SlotWrapper(nameof (__ror__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __ior__ = (PythonTypeSlot) new SlotWrapper(nameof (__ior__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __xor__ = (PythonTypeSlot) new SlotWrapper(nameof (__xor__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __rxor__ = (PythonTypeSlot) new SlotWrapper(nameof (__rxor__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __ixor__ = (PythonTypeSlot) new SlotWrapper(nameof (__ixor__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __delslice__ = (PythonTypeSlot) new SlotWrapper(nameof (__delslice__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __divmod__ = (PythonTypeSlot) new SlotWrapper(nameof (__divmod__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __float__ = (PythonTypeSlot) new SlotWrapper(nameof (__float__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __getslice__ = (PythonTypeSlot) new SlotWrapper(nameof (__getslice__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __index__ = (PythonTypeSlot) new SlotWrapper(nameof (__index__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __int__ = (PythonTypeSlot) new SlotWrapper(nameof (__int__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __iter__ = (PythonTypeSlot) new SlotWrapper(nameof (__iter__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __long__ = (PythonTypeSlot) new SlotWrapper(nameof (__long__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __rdivmod__ = (PythonTypeSlot) new SlotWrapper(nameof (__rdivmod__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __setslice__ = (PythonTypeSlot) new SlotWrapper(nameof (__setslice__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot next = (PythonTypeSlot) new SlotWrapper(nameof (next), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __getitem__ = (PythonTypeSlot) new SlotWrapper(nameof (__getitem__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __setitem__ = (PythonTypeSlot) new SlotWrapper(nameof (__setitem__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __delitem__ = (PythonTypeSlot) new SlotWrapper(nameof (__delitem__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __len__ = (PythonTypeSlot) new SlotWrapper(nameof (__len__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __pos__ = (PythonTypeSlot) new SlotWrapper(nameof (__pos__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __neg__ = (PythonTypeSlot) new SlotWrapper(nameof (__neg__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __invert__ = (PythonTypeSlot) new SlotWrapper(nameof (__invert__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __contains__ = (PythonTypeSlot) new SlotWrapper(nameof (__contains__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __abs__ = (PythonTypeSlot) new SlotWrapper(nameof (__abs__), PythonWeakRef.CallableProxyType);
    [SlotField]
    public static PythonTypeSlot __call__ = (PythonTypeSlot) new SlotWrapper(nameof (__call__), PythonWeakRef.CallableProxyType);

    internal static object MakeNew(CodeContext context, object @object, object callback)
    {
      IWeakReferenceable weakReferenceable = PythonWeakRef.ConvertToWeakReferenceable(context.LanguageContext, @object);
      if (callback == null)
      {
        WeakRefTracker weakRef = weakReferenceable.GetWeakRef();
        if (weakRef != null)
        {
          for (int index = 0; index < weakRef.HandlerCount; ++index)
          {
            if (weakRef.GetHandlerCallback(index) == null && weakRef.GetWeakRef(index) is PythonWeakRef.weakcallableproxy)
              return weakRef.GetWeakRef(index);
          }
        }
      }
      return (object) new PythonWeakRef.weakcallableproxy(context, @object, callback);
    }

    private weakcallableproxy(CodeContext context, object target, object callback)
    {
      PythonWeakRef.WeakRefHelpers.InitializeWeakRef(context.LanguageContext, (object) this, target, callback);
      this._target = new WeakHandle(target, false);
      this._context = context;
    }

    ~weakcallableproxy()
    {
      IWeakReferenceable weakref;
      if (this._context.LanguageContext.TryConvertToWeakReferenceable(this._target.Target, out weakref))
        weakref.GetWeakRef().RemoveHandler((object) this);
      this._target.Free();
    }

    private object GetObject()
    {
      object result;
      if (!this.TryGetObject(out result))
        throw PythonOps.ReferenceError("weakly referenced object no longer exists");
      return result;
    }

    private bool TryGetObject(out object result)
    {
      try
      {
        result = this._target.Target;
        if (result == null)
          return false;
        GC.KeepAlive((object) this);
        return true;
      }
      catch (InvalidOperationException ex)
      {
        result = (object) null;
        return false;
      }
    }

    PythonDictionary IPythonObject.Dict => (this.GetObject() as IPythonObject).Dict;

    PythonDictionary IPythonObject.SetDict(PythonDictionary dict)
    {
      return (this.GetObject() as IPythonObject).SetDict(dict);
    }

    bool IPythonObject.ReplaceDict(PythonDictionary dict)
    {
      return (this.GetObject() as IPythonObject).ReplaceDict(dict);
    }

    void IPythonObject.SetPythonType(PythonType newType)
    {
      (this.GetObject() as IPythonObject).SetPythonType(newType);
    }

    PythonType IPythonObject.PythonType
    {
      get => DynamicHelpers.GetPythonTypeFromType(typeof (PythonWeakRef.weakcallableproxy));
    }

    object[] IPythonObject.GetSlots() => (object[]) null;

    object[] IPythonObject.GetSlotsCreate() => (object[]) null;

    public override string ToString() => PythonOps.ToString(this.GetObject());

    public string __repr__(CodeContext context)
    {
      object target = this._target.Target;
      GC.KeepAlive((object) this);
      return $"<weakproxy at {IdDispenser.GetId((object) this)} to {PythonOps.GetPythonTypeName(target)} at {IdDispenser.GetId(target)}>";
    }

    [SpecialName]
    public object Call(CodeContext context, params object[] args)
    {
      return context.LanguageContext.CallSplat(this.GetObject(), args);
    }

    [SpecialName]
    public object Call(CodeContext context, [ParamDictionary] IDictionary<object, object> dict, params object[] args)
    {
      return PythonCalls.CallWithKeywordArgs(context, this.GetObject(), args, dict);
    }

    [SpecialName]
    public object GetCustomMember(CodeContext context, string name)
    {
      object o = this.GetObject();
      object ret;
      return PythonOps.TryGetBoundAttr(context, o, name, out ret) ? ret : (object) OperationFailed.Value;
    }

    [SpecialName]
    public void SetMember(CodeContext context, string name, object value)
    {
      object o = this.GetObject();
      PythonOps.SetAttr(context, o, name, value);
    }

    [SpecialName]
    public void DeleteMember(CodeContext context, string name)
    {
      object o = this.GetObject();
      PythonOps.DeleteAttr(context, o, name);
    }

    IList<string> IMembersList.GetMemberNames()
    {
      return PythonOps.GetStringMemberList((IPythonMembersList) this);
    }

    IList<object> IPythonMembersList.GetMemberNames(CodeContext context)
    {
      object result;
      return !this.TryGetObject(out result) ? (IList<object>) new IronPython.Runtime.List() : PythonOps.GetAttrNames(context, result);
    }

    object IProxyObject.Target => this.GetObject();

    public bool __eq__(object other)
    {
      return other is PythonWeakRef.weakcallableproxy weakcallableproxy ? this.GetObject().Equals(weakcallableproxy.GetObject()) : PythonOps.EqualRetBool(this._context, this.GetObject(), other);
    }

    public bool __ne__(object other) => !this.__eq__(other);

    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    {
      object result;
      return this.TryGetObject(out result) ? comparer.GetHashCode(result) : comparer.GetHashCode((object) null);
    }

    bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
    {
      object result1;
      if (!this.TryGetObject(out result1))
        result1 = (object) null;
      if (!(other is PythonWeakRef.weakcallableproxy))
        return comparer.Equals(result1, other);
      object result2;
      if (!this.TryGetObject(out result2))
        result2 = (object) null;
      return comparer.Equals(result1, result2);
    }

    public object __nonzero__() => (object) Converter.ConvertToBoolean(this.GetObject());
  }

  private static class WeakRefHelpers
  {
    public static WeakRefTracker InitializeWeakRef(
      PythonContext context,
      object self,
      object target,
      object callback)
    {
      IWeakReferenceable weakReferenceable = PythonWeakRef.ConvertToWeakReferenceable(context, target);
      WeakRefTracker weakRefTracker = weakReferenceable.GetWeakRef();
      if (weakRefTracker == null && !weakReferenceable.SetWeakRef(weakRefTracker = new WeakRefTracker(weakReferenceable)))
        throw PythonOps.TypeError("cannot create weak reference to '{0}' object", (object) PythonOps.GetPythonTypeName(target));
      if (callback != null || !weakRefTracker.Contains(callback, self))
        weakRefTracker.ChainCallback(callback, self);
      return weakRefTracker;
    }
  }
}
