// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonModule
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace IronPython.Runtime;

[PythonType("module")]
[DebuggerTypeProxy(typeof (PythonModule.DebugProxy))]
[DebuggerDisplay("module: {GetName()}")]
public class PythonModule : IDynamicMetaObjectProvider, IPythonMembersList, IMembersList
{
  private readonly PythonDictionary _dict;
  private Scope _scope;

  public PythonModule()
  {
    this._dict = new PythonDictionary();
    if (!(this.GetType() != typeof (PythonModule)) || !(this is IPythonObject))
      return;
    ((IPythonObject) this).ReplaceDict(this._dict);
  }

  internal PythonModule(PythonContext context, Scope scope)
  {
    this._dict = new PythonDictionary((DictionaryStorage) new ScopeDictionaryStorage(context, scope));
    this._scope = scope;
  }

  internal PythonModule(PythonDictionary dict) => this._dict = dict;

  public static PythonModule __new__(CodeContext context, PythonType cls, params object[] argsø)
  {
    if (cls == TypeCache.Module)
      return new PythonModule();
    return cls.IsSubclassOf(TypeCache.Module) ? (PythonModule) cls.CreateInstance(context) : throw PythonOps.TypeError("{0} is not a subtype of module", (object) cls.Name);
  }

  [StaticExtensionMethod]
  public static PythonModule __new__(
    CodeContext context,
    PythonType cls,
    [ParamDictionary] IDictionary<object, object> kwDict0,
    params object[] argsø)
  {
    return PythonModule.__new__(context, cls, argsø);
  }

  public void __init__(string name) => this.__init__(name, (string) null);

  public void __init__(string name, string doc)
  {
    this._dict[(object) "__name__"] = (object) name;
    this._dict[(object) "__doc__"] = (object) doc;
  }

  public object __getattribute__(CodeContext context, string name)
  {
    PythonTypeSlot slot;
    object obj;
    if (this.GetType() != typeof (PythonModule) && DynamicHelpers.GetPythonType((object) this).TryResolveMixedSlot(context, name, out slot) && slot.TryGetValue(context, (object) this, DynamicHelpers.GetPythonType((object) this), out obj))
      return obj;
    switch (name)
    {
      case "__dict__":
        return (object) this.__dict__;
      case "__class__":
        return (object) DynamicHelpers.GetPythonType((object) this);
      default:
        return this._dict.TryGetValue((object) name, out obj) ? obj : ObjectOps.__getattribute__(context, (object) this, name);
    }
  }

  internal object GetAttributeNoThrow(CodeContext context, string name)
  {
    PythonTypeSlot slot;
    object func;
    if (this.GetType() != typeof (PythonModule) && DynamicHelpers.GetPythonType((object) this).TryResolveMixedSlot(context, name, out slot) && slot.TryGetValue(context, (object) this, DynamicHelpers.GetPythonType((object) this), out func))
      return func;
    switch (name)
    {
      case "__dict__":
        return (object) this.__dict__;
      case "__class__":
        return (object) DynamicHelpers.GetPythonType((object) this);
      default:
        if (this._dict.TryGetValue((object) name, out func) || DynamicHelpers.GetPythonType((object) this).TryGetNonCustomMember(context, (object) this, name, out func))
          return func;
        return DynamicHelpers.GetPythonType((object) this).TryResolveMixedSlot(context, "__getattr__", out slot) && slot.TryGetValue(context, (object) this, DynamicHelpers.GetPythonType((object) this), out func) ? PythonCalls.Call(context, func, (object) name) : (object) OperationFailed.Value;
    }
  }

  public void __setattr__(CodeContext context, string name, object value)
  {
    PythonTypeSlot slot;
    if (this.GetType() != typeof (PythonModule) && DynamicHelpers.GetPythonType((object) this).TryResolveMixedSlot(context, name, out slot) && slot.TrySetValue(context, (object) this, DynamicHelpers.GetPythonType((object) this), value))
      return;
    switch (name)
    {
      case "__dict__":
        throw PythonOps.TypeError("readonly attribute");
      case "__class__":
        throw PythonOps.TypeError("__class__ assignment: only for heap types");
      default:
        this._dict[(object) name] = value;
        break;
    }
  }

  public void __delattr__(CodeContext context, string name)
  {
    PythonTypeSlot slot;
    if (this.GetType() != typeof (PythonModule) && DynamicHelpers.GetPythonType((object) this).TryResolveMixedSlot(context, name, out slot) && slot.TryDeleteValue(context, (object) this, DynamicHelpers.GetPythonType((object) this)))
      return;
    switch (name)
    {
      case "__dict__":
        throw PythonOps.TypeError("readonly attribute");
      case "__class__":
        throw PythonOps.TypeError("can't delete __class__ attribute");
      default:
        if (this._dict.TryRemoveValue((object) name, out object _))
          break;
        throw PythonOps.AttributeErrorForMissingAttribute("module", name);
    }
  }

  public string __repr__() => this.__str__();

  public string __str__()
  {
    object obj1;
    if (!this._dict.TryGetValue((object) "__file__", out obj1))
      obj1 = (object) null;
    object obj2;
    if (!this._dict._storage.TryGetName(out obj2))
      obj2 = (object) null;
    string str1 = obj1 as string;
    if (!(obj2 is string str2))
      str2 = "?";
    string str3 = str2;
    return str1 == null ? $"<module '{str3}' (built-in)>" : $"<module '{str3}' from '{str1}'>";
  }

  internal PythonDictionary __dict__ => this._dict;

  [PropertyMethod]
  [SpecialName]
  public PythonDictionary Get__dict__() => this._dict;

  [PropertyMethod]
  [SpecialName]
  public void Set__dict__(object value) => throw PythonOps.TypeError("readonly attribute");

  [PropertyMethod]
  [SpecialName]
  public void Delete__dict__() => throw PythonOps.TypeError("readonly attribute");

  public Scope Scope
  {
    get
    {
      if (this._scope == null)
        Interlocked.CompareExchange<Scope>(ref this._scope, new Scope((IDynamicMetaObjectProvider) new ObjectDictionaryExpando((IDictionary<object, object>) this._dict)), (Scope) null);
      return this._scope;
    }
  }

  [PythonHidden(new PlatformID[] {})]
  public DynamicMetaObject GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new PythonModule.MetaModule(this, parameter);
  }

  internal string GetFile()
  {
    object obj;
    return this._dict.TryGetValue((object) "__file__", out obj) ? obj as string : (string) null;
  }

  internal string GetName()
  {
    object obj;
    return this._dict._storage.TryGetName(out obj) ? obj as string : (string) null;
  }

  IList<object> IPythonMembersList.GetMemberNames(CodeContext context)
  {
    return (IList<object>) new List<object>((IEnumerable<object>) this.__dict__.Keys);
  }

  IList<string> IMembersList.GetMemberNames()
  {
    List<string> memberNames = new List<string>(this.__dict__.Keys.Count);
    foreach (object key in (IEnumerable<object>) this.__dict__.Keys)
    {
      if (key is string str)
        memberNames.Add(str);
    }
    return (IList<string>) memberNames;
  }

  private class MetaModule(PythonModule module, Expression self) : MetaPythonObject(self, BindingRestrictions.Empty, (object) module), IPythonGetable
  {
    public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
    {
      return this.GetMemberWorker((DynamicMetaObjectBinder) binder, PythonContext.GetCodeContextMO((DynamicMetaObjectBinder) binder));
    }

    public DynamicMetaObject GetMember(PythonGetMemberBinder member, DynamicMetaObject codeContext)
    {
      return this.GetMemberWorker((DynamicMetaObjectBinder) member, codeContext);
    }

    private DynamicMetaObject GetMemberWorker(
      DynamicMetaObjectBinder binder,
      DynamicMetaObject codeContext)
    {
      string getMemberName = MetaPythonObject.GetGetMemberName(binder);
      ParameterExpression ifTrue = Expression.Variable(typeof (object), "res");
      return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        ifTrue
      }, (Expression) Expression.Condition((Expression) Expression.Call(typeof (PythonOps).GetMethod("ModuleTryGetMember"), PythonContext.GetCodeContext(binder), Utils.Convert(this.Expression, typeof (PythonModule)), (Expression) Expression.Constant((object) getMemberName), (Expression) ifTrue), (Expression) ifTrue, (Expression) Expression.Convert(MetaPythonObject.GetMemberFallback((DynamicMetaObject) this, binder, codeContext).Expression, typeof (object)))), BindingRestrictions.GetTypeRestriction(this.Expression, this.Value.GetType()));
    }

    public override DynamicMetaObject BindInvokeMember(
      InvokeMemberBinder action,
      DynamicMetaObject[] args)
    {
      return BindingHelpers.GenericInvokeMember(action, (ValidationInfo) null, (DynamicMetaObject) this, args);
    }

    public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
    {
      return new DynamicMetaObject((Expression) Expression.Block((Expression) Expression.Call(Utils.Convert(this.Expression, typeof (PythonModule)), typeof (PythonModule).GetMethod("__setattr__"), PythonContext.GetCodeContext((DynamicMetaObjectBinder) binder), (Expression) Expression.Constant((object) binder.Name), (Expression) Expression.Convert(value.Expression, typeof (object))), (Expression) Expression.Convert(value.Expression, typeof (object))), BindingRestrictions.GetTypeRestriction(this.Expression, this.Value.GetType()));
    }

    public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
    {
      return new DynamicMetaObject((Expression) Expression.Call(Utils.Convert(this.Expression, typeof (PythonModule)), typeof (PythonModule).GetMethod("__delattr__"), PythonContext.GetCodeContext((DynamicMetaObjectBinder) binder), (Expression) Expression.Constant((object) binder.Name)), BindingRestrictions.GetTypeRestriction(this.Expression, this.Value.GetType()));
    }

    public override IEnumerable<string> GetDynamicMemberNames()
    {
      foreach (object key in (IEnumerable<object>) ((PythonModule) this.Value).__dict__.Keys)
      {
        if (key is string dynamicMemberName)
          yield return dynamicMemberName;
      }
    }
  }

  internal class DebugProxy
  {
    private readonly PythonModule _module;

    public DebugProxy(PythonModule module) => this._module = module;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public List<ObjectDebugView> Members
    {
      get
      {
        List<ObjectDebugView> members = new List<ObjectDebugView>();
        foreach (KeyValuePair<object, object> keyValuePair in this._module._dict)
          members.Add(new ObjectDebugView(keyValuePair.Key, keyValuePair.Value));
        return members;
      }
    }
  }
}
