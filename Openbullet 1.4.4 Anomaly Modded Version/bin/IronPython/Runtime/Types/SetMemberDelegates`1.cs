// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.SetMemberDelegates`1
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Types;

internal class SetMemberDelegates<TValue> : FastSetBase<TValue>
{
  private readonly string _name;
  private readonly PythonTypeSlot _slot;
  private readonly SlotSetValue _slotFunc;
  private readonly CodeContext _context;
  private readonly int _index;
  private readonly int _keysVersion;

  public SetMemberDelegates(
    CodeContext context,
    PythonType type,
    OptimizedSetKind kind,
    string name,
    int version,
    PythonTypeSlot slot,
    SlotSetValue slotFunc)
    : base(version)
  {
    this._slot = slot;
    this._name = name;
    this._slotFunc = slotFunc;
    this._context = context;
    switch (kind)
    {
      case OptimizedSetKind.SetAttr:
        this._func = (Delegate) new Func<CallSite, object, TValue, object>(this.SetAttr);
        break;
      case OptimizedSetKind.UserSlot:
        this._func = (Delegate) new Func<CallSite, object, TValue, object>(this.UserSlot);
        break;
      case OptimizedSetKind.SetDict:
        IList<string> optimizedInstanceNames = type.GetOptimizedInstanceNames();
        int num;
        if (optimizedInstanceNames != null && (num = optimizedInstanceNames.IndexOf(name)) != -1)
        {
          this._index = num;
          this._keysVersion = type.GetOptimizedInstanceVersion();
          this._func = (Delegate) new Func<CallSite, object, TValue, object>(this.SetDictOptimized);
          break;
        }
        this._func = (Delegate) new Func<CallSite, object, TValue, object>(this.SetDict);
        break;
      case OptimizedSetKind.Error:
        this._func = (Delegate) new Func<CallSite, object, TValue, object>(this.Error);
        break;
    }
  }

  public object SetAttr(CallSite site, object self, TValue value)
  {
    if (!(self is IPythonObject ipo) || ipo.PythonType.Version != this._version || !this.ShouldUseNonOptimizedSite)
      return FastSetBase<TValue>.Update(site, self, value);
    ++this._hitCount;
    object func;
    if (!this._slot.TryGetValue(this._context, self, ipo.PythonType, out func))
      return this.TypeError(ipo);
    return PythonOps.CallWithContext(this._context, func, (object) this._name, (object) value);
  }

  public object SetDictOptimized(CallSite site, object self, TValue value)
  {
    if (!(self is IPythonObject ipo) || ipo.PythonType.Version != this._version || !this.ShouldUseNonOptimizedSite)
      return FastSetBase<TValue>.Update(site, self, value);
    ++this._hitCount;
    return UserTypeOps.SetDictionaryValueOptimized(ipo, this._name, (object) value, this._keysVersion, this._index);
  }

  public object SetDict(CallSite site, object self, TValue value)
  {
    if (!(self is IPythonObject self1) || self1.PythonType.Version != this._version || !this.ShouldUseNonOptimizedSite)
      return FastSetBase<TValue>.Update(site, self, value);
    ++this._hitCount;
    UserTypeOps.SetDictionaryValue(self1, this._name, (object) value);
    return (object) null;
  }

  public object Error(CallSite site, object self, TValue value)
  {
    return self is IPythonObject ipo && ipo.PythonType.Version == this._version ? this.TypeError(ipo) : FastSetBase<TValue>.Update(site, self, value);
  }

  public object UserSlot(CallSite site, object self, TValue value)
  {
    if (!(self is IPythonObject pythonObject) || pythonObject.PythonType.Version != this._version || !this.ShouldUseNonOptimizedSite)
      return FastSetBase<TValue>.Update(site, self, value);
    ++this._hitCount;
    this._slotFunc(self, (object) value);
    return (object) null;
  }

  private object TypeError(IPythonObject ipo)
  {
    throw PythonOps.AttributeErrorForMissingAttribute(ipo.PythonType.Name, this._name);
  }
}
