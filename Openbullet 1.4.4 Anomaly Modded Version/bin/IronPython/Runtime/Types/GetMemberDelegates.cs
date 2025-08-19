// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.GetMemberDelegates
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Types;

internal class GetMemberDelegates : UserGetBase
{
  private readonly string _name;
  private readonly bool _isNoThrow;
  private readonly PythonTypeSlot _slot;
  private readonly PythonTypeSlot _getattrSlot;
  private readonly SlotGetValue _slotFunc;
  private readonly Func<CallSite, object, CodeContext, object> _fallback;
  private readonly int _dictVersion;
  private readonly int _dictIndex;
  private readonly ExtensionMethodSet _extMethods;

  public GetMemberDelegates(
    OptimizedGetKind getKind,
    PythonType type,
    PythonGetMemberBinder binder,
    string name,
    int version,
    PythonTypeSlot slot,
    PythonTypeSlot getattrSlot,
    SlotGetValue slotFunc,
    Func<CallSite, object, CodeContext, object> fallback,
    ExtensionMethodSet extMethods)
    : base(binder, version)
  {
    this._slot = slot;
    this._name = name;
    this._getattrSlot = getattrSlot;
    this._slotFunc = slotFunc;
    this._fallback = fallback;
    this._isNoThrow = binder.IsNoThrow;
    this._extMethods = extMethods;
    IList<string> optimizedInstanceNames = type.GetOptimizedInstanceNames();
    switch (getKind)
    {
      case OptimizedGetKind.SlotDict:
        if (optimizedInstanceNames != null)
          this._dictIndex = optimizedInstanceNames.IndexOf(name);
        if (optimizedInstanceNames != null && this._dictIndex != -1)
        {
          this._func = new Func<CallSite, object, CodeContext, object>(this.SlotDictOptimized);
          this._dictVersion = type.GetOptimizedInstanceVersion();
          break;
        }
        this._func = new Func<CallSite, object, CodeContext, object>(this.SlotDict);
        break;
      case OptimizedGetKind.SlotOnly:
        this._func = new Func<CallSite, object, CodeContext, object>(this.SlotOnly);
        break;
      case OptimizedGetKind.PropertySlot:
        this._func = new Func<CallSite, object, CodeContext, object>(this.UserSlot);
        break;
      case OptimizedGetKind.UserSlotDict:
        if (optimizedInstanceNames != null)
          this._dictIndex = optimizedInstanceNames.IndexOf(name);
        if (optimizedInstanceNames != null && this._dictIndex != -1)
        {
          this._dictVersion = type.GetOptimizedInstanceVersion();
          if (this._getattrSlot != null)
          {
            this._func = new Func<CallSite, object, CodeContext, object>(this.UserSlotDictGetAttrOptimized);
            break;
          }
          this._func = new Func<CallSite, object, CodeContext, object>(this.UserSlotDictOptimized);
          break;
        }
        if (this._getattrSlot != null)
        {
          this._func = new Func<CallSite, object, CodeContext, object>(this.UserSlotDictGetAttr);
          break;
        }
        this._func = new Func<CallSite, object, CodeContext, object>(this.UserSlotDict);
        break;
      case OptimizedGetKind.UserSlotOnly:
        if (this._getattrSlot != null)
        {
          this._func = new Func<CallSite, object, CodeContext, object>(this.UserSlotOnlyGetAttr);
          break;
        }
        this._func = new Func<CallSite, object, CodeContext, object>(this.UserSlotOnly);
        break;
      default:
        throw new InvalidOperationException();
    }
  }

  public object SlotDict(CallSite site, object self, CodeContext context)
  {
    if (!(self is IPythonObject ipo) || ipo.PythonType.Version != this._version || !this.ShouldUseNonOptimizedSite || (object) context.ModuleContext.ExtensionMethods != (object) this._extMethods)
      return FastGetBase.Update(site, self, context);
    ++this._hitCount;
    object res;
    if (ipo.Dict != null && ipo.Dict.TryGetValue((object) this._name, out res) || this._slot != null && this._slot.TryGetValue(context, self, ipo.PythonType, out res))
      return res;
    return this._getattrSlot != null && this._getattrSlot.TryGetValue(context, self, ipo.PythonType, out res) ? this.GetAttr(context, res) : this.TypeError(site, ipo, context);
  }

  public object SlotDictOptimized(CallSite site, object self, CodeContext context)
  {
    if (!(self is IPythonObject ipo) || ipo.PythonType.Version != this._version || !this.ShouldUseNonOptimizedSite || (object) context.ModuleContext.ExtensionMethods != (object) this._extMethods)
      return FastGetBase.Update(site, self, context);
    ++this._hitCount;
    object res;
    if (UserTypeOps.TryGetDictionaryValue(ipo.Dict, this._name, this._dictVersion, this._dictIndex, out res) || this._slot != null && this._slot.TryGetValue(context, self, ipo.PythonType, out res))
      return res;
    return this._getattrSlot != null && this._getattrSlot.TryGetValue(context, self, ipo.PythonType, out res) ? this.GetAttr(context, res) : this.TypeError(site, ipo, context);
  }

  public object SlotOnly(CallSite site, object self, CodeContext context)
  {
    if (!(self is IPythonObject ipo) || ipo.PythonType.Version != this._version || !this.ShouldUseNonOptimizedSite || (object) context.ModuleContext.ExtensionMethods != (object) this._extMethods)
      return FastGetBase.Update(site, self, context);
    ++this._hitCount;
    object res;
    if (this._slot != null && this._slot.TryGetValue(context, self, ipo.PythonType, out res))
      return res;
    return this._getattrSlot != null && this._getattrSlot.TryGetValue(context, self, ipo.PythonType, out res) ? this.GetAttr(context, res) : this.TypeError(site, ipo, context);
  }

  public object UserSlotDict(CallSite site, object self, CodeContext context)
  {
    if (!(self is IPythonObject pythonObject) || pythonObject.PythonType.Version != this._version || (object) context.ModuleContext.ExtensionMethods != (object) this._extMethods)
      return FastGetBase.Update(site, self, context);
    object obj;
    return pythonObject.Dict != null && pythonObject.Dict.TryGetValue((object) this._name, out obj) ? obj : ((PythonTypeUserDescriptorSlot) this._slot).GetValue(context, self, pythonObject.PythonType);
  }

  public object UserSlotDictOptimized(CallSite site, object self, CodeContext context)
  {
    if (!(self is IPythonObject pythonObject) || pythonObject.PythonType.Version != this._version || (object) context.ModuleContext.ExtensionMethods != (object) this._extMethods)
      return FastGetBase.Update(site, self, context);
    object res;
    return UserTypeOps.TryGetDictionaryValue(pythonObject.Dict, this._name, this._dictVersion, this._dictIndex, out res) ? res : ((PythonTypeUserDescriptorSlot) this._slot).GetValue(context, self, pythonObject.PythonType);
  }

  public object UserSlotOnly(CallSite site, object self, CodeContext context)
  {
    return self is IPythonObject pythonObject && pythonObject.PythonType.Version == this._version && (object) context.ModuleContext.ExtensionMethods == (object) this._extMethods ? ((PythonTypeUserDescriptorSlot) this._slot).GetValue(context, self, pythonObject.PythonType) : FastGetBase.Update(site, self, context);
  }

  public object UserSlotDictGetAttr(CallSite site, object self, CodeContext context)
  {
    if (!(self is IPythonObject ipo) || ipo.PythonType.Version != this._version || (object) context.ModuleContext.ExtensionMethods != (object) this._extMethods)
      return FastGetBase.Update(site, self, context);
    object res;
    if (ipo.Dict != null && ipo.Dict.TryGetValue((object) this._name, out res))
      return res;
    try
    {
      return ((PythonTypeUserDescriptorSlot) this._slot).GetValue(context, self, ipo.PythonType);
    }
    catch (MissingMemberException ex)
    {
    }
    return this._getattrSlot.TryGetValue(context, self, ipo.PythonType, out res) ? this.GetAttr(context, res) : this.TypeError(site, ipo, context);
  }

  public object UserSlotDictGetAttrOptimized(CallSite site, object self, CodeContext context)
  {
    if (!(self is IPythonObject ipo) || ipo.PythonType.Version != this._version || (object) context.ModuleContext.ExtensionMethods != (object) this._extMethods)
      return FastGetBase.Update(site, self, context);
    object res;
    if (UserTypeOps.TryGetDictionaryValue(ipo.Dict, this._name, this._dictVersion, this._dictIndex, out res))
      return res;
    try
    {
      return ((PythonTypeUserDescriptorSlot) this._slot).GetValue(context, self, ipo.PythonType);
    }
    catch (MissingMemberException ex)
    {
    }
    return this._getattrSlot.TryGetValue(context, self, ipo.PythonType, out res) ? this.GetAttr(context, res) : this.TypeError(site, ipo, context);
  }

  public object UserSlotOnlyGetAttr(CallSite site, object self, CodeContext context)
  {
    if (self is IPythonObject ipo && ipo.PythonType.Version == this._version)
    {
      if ((object) context.ModuleContext.ExtensionMethods == (object) this._extMethods)
      {
        try
        {
          return ((PythonTypeUserDescriptorSlot) this._slot).GetValue(context, self, ipo.PythonType);
        }
        catch (MissingMemberException ex)
        {
        }
        object res;
        return this._getattrSlot.TryGetValue(context, self, ipo.PythonType, out res) ? this.GetAttr(context, res) : this.TypeError(site, ipo, context);
      }
    }
    return FastGetBase.Update(site, self, context);
  }

  public object UserSlot(CallSite site, object self, CodeContext context)
  {
    if (!(self is IPythonObject ipo) || ipo.PythonType.Version != this._version || !this.ShouldUseNonOptimizedSite || (object) context.ModuleContext.ExtensionMethods != (object) this._extMethods)
      return FastGetBase.Update(site, self, context);
    object res = this._slotFunc(self);
    if (res != Uninitialized.Instance)
      return res;
    return this._getattrSlot != null && this._getattrSlot.TryGetValue(context, self, ipo.PythonType, out res) ? this.GetAttr(context, res) : this.TypeError(site, ipo, context);
  }

  private object GetAttr(CodeContext context, object res)
  {
    if (!this._isNoThrow)
      return context.LanguageContext.Call(context, res, (object) this._name);
    try
    {
      return context.LanguageContext.Call(context, res, (object) this._name);
    }
    catch (MissingMemberException ex)
    {
      return (object) OperationFailed.Value;
    }
  }

  private object TypeError(CallSite site, IPythonObject ipo, CodeContext context)
  {
    return this._fallback(site, (object) ipo, context);
  }
}
