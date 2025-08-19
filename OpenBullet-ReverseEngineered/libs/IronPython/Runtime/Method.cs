// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Method
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

[PythonType("instancemethod")]
[DontMapGetMemberNamesToDir]
public sealed class Method : 
  PythonTypeSlot,
  IWeakReferenceable,
  IPythonMembersList,
  IMembersList,
  IDynamicMetaObjectProvider,
  ICodeFormattable,
  IFastInvokable
{
  private readonly object _func;
  private readonly object _inst;
  private readonly object _declaringClass;
  private WeakRefTracker _weakref;

  public Method(object function, object instance, object @class)
  {
    this._func = function;
    this._inst = instance;
    this._declaringClass = @class;
  }

  public Method(object function, object instance)
  {
    if (instance == null)
      throw PythonOps.TypeError("unbound methods must have a class provided");
    this._func = function;
    this._inst = instance;
  }

  internal string Name
  {
    get => (string) PythonOps.GetBoundAttr(DefaultContext.Default, this._func, "__name__");
  }

  public string __doc__
  {
    get => PythonOps.GetBoundAttr(DefaultContext.Default, this._func, nameof (__doc__)) as string;
  }

  public object im_func => this._func;

  public object __func__ => this._func;

  public object im_self => this._inst;

  public object __self__ => this._inst;

  public object im_class
  {
    get => PythonOps.ToPythonType(this._declaringClass as PythonType) ?? this._declaringClass;
  }

  [SpecialName]
  public object Call(CodeContext context, params object[] args)
  {
    return context.LanguageContext.CallSplat((object) this, args);
  }

  [SpecialName]
  public object Call(CodeContext context, [ParamDictionary] IDictionary<object, object> kwArgs, params object[] args)
  {
    return context.LanguageContext.CallWithKeywords((object) this, args, kwArgs);
  }

  private Exception BadSelf(object got)
  {
    OldClass imClass1 = this.im_class as OldClass;
    string str = got != null ? PythonOps.GetPythonTypeName(got) + " instance" : "nothing";
    PythonType imClass2 = this.im_class as PythonType;
    return PythonOps.TypeError("unbound method {0}() must be called with {1} instance as first argument (got {2} instead)", (object) this.Name, imClass1 != null ? (object) imClass1.Name : (imClass2 != null ? (object) imClass2.Name : this.im_class), (object) str);
  }

  internal object CheckSelf(CodeContext context, object self)
  {
    if (!PythonOps.IsInstance(context, self, this.im_class))
      throw this.BadSelf(self);
    return self;
  }

  private string DeclaringClassAsString()
  {
    if (this.im_class == null)
      return "?";
    if (this.im_class is PythonType imClass1)
      return imClass1.Name;
    return this.im_class is OldClass imClass2 ? imClass2.Name : this.im_class.ToString();
  }

  public override bool Equals(object obj)
  {
    return obj is Method method && (this._inst == method._inst || PythonOps.EqualRetBool(this._inst, method._inst)) && PythonOps.EqualRetBool(this._func, method._func);
  }

  public override int GetHashCode()
  {
    return this._inst == null ? PythonOps.Hash(DefaultContext.Default, this._func) : PythonOps.Hash(DefaultContext.Default, this._inst) ^ PythonOps.Hash(DefaultContext.Default, this._func);
  }

  WeakRefTracker IWeakReferenceable.GetWeakRef() => this._weakref;

  bool IWeakReferenceable.SetWeakRef(WeakRefTracker value)
  {
    this._weakref = value;
    return true;
  }

  void IWeakReferenceable.SetFinalizer(WeakRefTracker value)
  {
    ((IWeakReferenceable) this).SetWeakRef(value);
  }

  [SpecialName]
  public object GetCustomMember(CodeContext context, string name)
  {
    switch (name)
    {
      case "__module__":
        return PythonOps.GetBoundAttr(context, this._func, "__module__");
      case "__name__":
        return PythonOps.GetBoundAttr(DefaultContext.Default, this._func, "__name__");
      default:
        string name1 = name;
        object ret;
        return TypeCache.Method.TryGetBoundMember(context, (object) this, name1, out ret) || PythonOps.TryGetBoundAttr(context, this._func, name1, out ret) ? ret : (object) OperationFailed.Value;
    }
  }

  [SpecialName]
  public void SetMemberAfter(CodeContext context, string name, object value)
  {
    TypeCache.Method.SetMember(context, (object) this, name, value);
  }

  [SpecialName]
  public void DeleteMember(CodeContext context, string name)
  {
    TypeCache.Method.DeleteMember(context, (object) this, name);
  }

  IList<string> IMembersList.GetMemberNames()
  {
    return PythonOps.GetStringMemberList((IPythonMembersList) this);
  }

  IList<object> IPythonMembersList.GetMemberNames(CodeContext context)
  {
    List memberNames = TypeCache.Method.GetMemberNames(context);
    memberNames.AddNoLockNoDups((object) "__module__");
    if (this._func is PythonFunction func)
    {
      foreach (KeyValuePair<object, object> keyValuePair in func.func_dict)
        memberNames.AddNoLockNoDups(keyValuePair.Key);
    }
    return (IList<object>) memberNames;
  }

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    if (this.im_self == null && (owner == null || owner == this.im_class || PythonOps.IsSubClass(context, owner, this.im_class)))
    {
      value = (object) new Method(this._func, instance, (object) owner);
      return true;
    }
    value = (object) this;
    return true;
  }

  internal override bool GetAlwaysSucceeds => true;

  public string __repr__(CodeContext context)
  {
    object ret;
    if (!PythonOps.TryGetBoundAttr(context, this._func, "__name__", out ret))
      ret = (object) "?";
    return this._inst != null ? $"<bound method {this.DeclaringClassAsString()}.{ret} of {PythonOps.Repr(context, this._inst)}>" : $"<unbound method {this.DeclaringClassAsString()}.{ret}>";
  }

  DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new MetaMethod(parameter, BindingRestrictions.Empty, this);
  }

  FastBindResult<T> IFastInvokable.MakeInvokeBinding<T>(
    CallSite<T> site,
    PythonInvokeBinder binder,
    CodeContext context,
    object[] args)
  {
    if (binder.Signature.IsSimple)
    {
      Method.BaseMethodBinding binding = (Method.BaseMethodBinding) null;
      if (this.__self__ == null)
      {
        if (args.Length != 0)
        {
          Method.BaseMethodBinding methodBinding = Method.GetMethodBinding<T>(binder, Method.GetTypeArgsSelfless<T>(), binding);
          if (methodBinding != null)
            return new FastBindResult<T>((T) methodBinding.GetSelflessTarget(), true);
        }
      }
      else
      {
        PythonInvokeBinder selfBinder = Method.GetSelfBinder(binder, context);
        Method.BaseMethodBinding baseMethodBinding = args.Length != 0 ? Method.GetMethodBinding<T>(selfBinder, Method.GetTypeArgs<T>(), binding) : (Method.BaseMethodBinding) new Method.MethodBinding(selfBinder);
        if (baseMethodBinding != null)
          return new FastBindResult<T>((T) baseMethodBinding.GetSelfTarget(), true);
      }
    }
    return new FastBindResult<T>();
  }

  private static Method.BaseMethodBinding GetMethodBinding<T>(
    PythonInvokeBinder binder,
    Type[] typeArgs,
    Method.BaseMethodBinding binding)
    where T : class
  {
    switch (typeArgs.Length)
    {
      case 1:
        binding = (Method.BaseMethodBinding) Activator.CreateInstance(typeof (Method.MethodBinding<>).MakeGenericType(typeArgs), (object) binder);
        break;
      case 2:
        binding = (Method.BaseMethodBinding) Activator.CreateInstance(typeof (Method.MethodBinding<,>).MakeGenericType(typeArgs), (object) binder);
        break;
      case 3:
        binding = (Method.BaseMethodBinding) Activator.CreateInstance(typeof (Method.MethodBinding<,,>).MakeGenericType(typeArgs), (object) binder);
        break;
      case 4:
        binding = (Method.BaseMethodBinding) Activator.CreateInstance(typeof (Method.MethodBinding<,,,>).MakeGenericType(typeArgs), (object) binder);
        break;
      case 5:
        binding = (Method.BaseMethodBinding) Activator.CreateInstance(typeof (Method.MethodBinding<,,,,>).MakeGenericType(typeArgs), (object) binder);
        break;
      case 6:
        binding = (Method.BaseMethodBinding) Activator.CreateInstance(typeof (Method.MethodBinding<,,,,,>).MakeGenericType(typeArgs), (object) binder);
        break;
      case 7:
        binding = (Method.BaseMethodBinding) Activator.CreateInstance(typeof (Method.MethodBinding<,,,,,,>).MakeGenericType(typeArgs), (object) binder);
        break;
      case 8:
        binding = (Method.BaseMethodBinding) Activator.CreateInstance(typeof (Method.MethodBinding<,,,,,,,>).MakeGenericType(typeArgs), (object) binder);
        break;
      case 9:
        binding = (Method.BaseMethodBinding) Activator.CreateInstance(typeof (Method.MethodBinding<,,,,,,,,>).MakeGenericType(typeArgs), (object) binder);
        break;
      case 10:
        binding = (Method.BaseMethodBinding) Activator.CreateInstance(typeof (Method.MethodBinding<,,,,,,,,,>).MakeGenericType(typeArgs), (object) binder);
        break;
      case 11:
        binding = (Method.BaseMethodBinding) Activator.CreateInstance(typeof (Method.MethodBinding<,,,,,,,,,,>).MakeGenericType(typeArgs), (object) binder);
        break;
      case 12:
        binding = (Method.BaseMethodBinding) Activator.CreateInstance(typeof (Method.MethodBinding<,,,,,,,,,,,>).MakeGenericType(typeArgs), (object) binder);
        break;
    }
    return binding;
  }

  private static Type[] GetTypeArgs<T>() where T : class
  {
    return ArrayUtils.ShiftLeft<Type>(ArrayUtils.ConvertAll<ParameterInfo, Type>(typeof (T).GetMethod("Invoke").GetParameters(), (Func<ParameterInfo, Type>) (pi => pi.ParameterType)), 3);
  }

  private static Type[] GetTypeArgsSelfless<T>() where T : class
  {
    return ArrayUtils.ShiftLeft<Type>(ArrayUtils.ConvertAll<ParameterInfo, Type>(typeof (T).GetMethod("Invoke").GetParameters(), (Func<ParameterInfo, Type>) (pi => pi.ParameterType)), 4);
  }

  private static PythonInvokeBinder GetSelfBinder(PythonInvokeBinder binder, CodeContext context)
  {
    return context.LanguageContext.Invoke(new CallSignature(ArrayUtils.Insert<Argument>(new Argument(ArgumentType.Simple), binder.Signature.GetArgumentInfos())));
  }

  private abstract class BaseMethodBinding
  {
    public abstract Delegate GetSelfTarget();

    public abstract Delegate GetSelflessTarget();
  }

  private class MethodBinding : Method.BaseMethodBinding
  {
    private CallSite<Func<CallSite, CodeContext, object, object, object>> _site;

    public MethodBinding(PythonInvokeBinder binder)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, object>>.Create((CallSiteBinder) binder);
    }

    public object SelfTarget(CallSite site, CodeContext context, object target)
    {
      return target is Method method && method._inst != null ? this._site.Target((CallSite) this._site, context, method._func, method._inst) : ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, target);
    }

    public override Delegate GetSelfTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, object>(this.SelfTarget);
    }

    public override Delegate GetSelflessTarget() => throw new InvalidOperationException();
  }

  private class MethodBinding<T0> : Method.BaseMethodBinding
  {
    private CallSite<Func<CallSite, CodeContext, object, object, T0, object>> _site;

    public MethodBinding(PythonInvokeBinder binder)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, object>>.Create((CallSiteBinder) binder);
    }

    public object SelfTarget(CallSite site, CodeContext context, object target, T0 arg0)
    {
      return target is Method method && method._inst != null ? this._site.Target((CallSite) this._site, context, method._func, method._inst, arg0) : ((CallSite<Func<CallSite, CodeContext, object, T0, object>>) site).Update(site, context, target, arg0);
    }

    public object SelflessTarget(
      CallSite site,
      CodeContext context,
      object target,
      object arg0,
      T0 arg1)
    {
      return target is Method method && method._inst == null ? this._site.Target((CallSite) this._site, context, method._func, PythonOps.MethodCheckSelf(context, method, arg0), arg1) : ((CallSite<Func<CallSite, CodeContext, object, object, T0, object>>) site).Update(site, context, target, arg0, arg1);
    }

    public override Delegate GetSelfTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, object>(this.SelfTarget);
    }

    public override Delegate GetSelflessTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, object, T0, object>(this.SelflessTarget);
    }
  }

  private class MethodBinding<T0, T1> : Method.BaseMethodBinding
  {
    private CallSite<Func<CallSite, CodeContext, object, object, T0, T1, object>> _site;

    public MethodBinding(PythonInvokeBinder binder)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, T1, object>>.Create((CallSiteBinder) binder);
    }

    public object SelfTarget(CallSite site, CodeContext context, object target, T0 arg0, T1 arg1)
    {
      return target is Method method && method._inst != null ? this._site.Target((CallSite) this._site, context, method._func, method._inst, arg0, arg1) : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, object>>) site).Update(site, context, target, arg0, arg1);
    }

    public object SelflessTarget(
      CallSite site,
      CodeContext context,
      object target,
      object arg0,
      T0 arg1,
      T1 arg2)
    {
      return target is Method method && method._inst == null ? this._site.Target((CallSite) this._site, context, method._func, PythonOps.MethodCheckSelf(context, method, arg0), arg1, arg2) : ((CallSite<Func<CallSite, CodeContext, object, object, T0, T1, object>>) site).Update(site, context, target, arg0, arg1, arg2);
    }

    public override Delegate GetSelfTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, T1, object>(this.SelfTarget);
    }

    public override Delegate GetSelflessTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, object, T0, T1, object>(this.SelflessTarget);
    }
  }

  private class MethodBinding<T0, T1, T2> : Method.BaseMethodBinding
  {
    private CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, object>> _site;

    public MethodBinding(PythonInvokeBinder binder)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, object>>.Create((CallSiteBinder) binder);
    }

    public object SelfTarget(
      CallSite site,
      CodeContext context,
      object target,
      T0 arg0,
      T1 arg1,
      T2 arg2)
    {
      return target is Method method && method._inst != null ? this._site.Target((CallSite) this._site, context, method._func, method._inst, arg0, arg1, arg2) : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, object>>) site).Update(site, context, target, arg0, arg1, arg2);
    }

    public object SelflessTarget(
      CallSite site,
      CodeContext context,
      object target,
      object arg0,
      T0 arg1,
      T1 arg2,
      T2 arg3)
    {
      return target is Method method && method._inst == null ? this._site.Target((CallSite) this._site, context, method._func, PythonOps.MethodCheckSelf(context, method, arg0), arg1, arg2, arg3) : ((CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3);
    }

    public override Delegate GetSelfTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, T1, T2, object>(this.SelfTarget);
    }

    public override Delegate GetSelflessTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, object, T0, T1, T2, object>(this.SelflessTarget);
    }
  }

  private class MethodBinding<T0, T1, T2, T3> : Method.BaseMethodBinding
  {
    private CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, object>> _site;

    public MethodBinding(PythonInvokeBinder binder)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, object>>.Create((CallSiteBinder) binder);
    }

    public object SelfTarget(
      CallSite site,
      CodeContext context,
      object target,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3)
    {
      return target is Method method && method._inst != null ? this._site.Target((CallSite) this._site, context, method._func, method._inst, arg0, arg1, arg2, arg3) : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3);
    }

    public object SelflessTarget(
      CallSite site,
      CodeContext context,
      object target,
      object arg0,
      T0 arg1,
      T1 arg2,
      T2 arg3,
      T3 arg4)
    {
      return target is Method method && method._inst == null ? this._site.Target((CallSite) this._site, context, method._func, PythonOps.MethodCheckSelf(context, method, arg0), arg1, arg2, arg3, arg4) : ((CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4);
    }

    public override Delegate GetSelfTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>(this.SelfTarget);
    }

    public override Delegate GetSelflessTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, object>(this.SelflessTarget);
    }
  }

  private class MethodBinding<T0, T1, T2, T3, T4> : Method.BaseMethodBinding
  {
    private CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, object>> _site;

    public MethodBinding(PythonInvokeBinder binder)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, object>>.Create((CallSiteBinder) binder);
    }

    public object SelfTarget(
      CallSite site,
      CodeContext context,
      object target,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3,
      T4 arg4)
    {
      return target is Method method && method._inst != null ? this._site.Target((CallSite) this._site, context, method._func, method._inst, arg0, arg1, arg2, arg3, arg4) : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4);
    }

    public object SelflessTarget(
      CallSite site,
      CodeContext context,
      object target,
      object arg0,
      T0 arg1,
      T1 arg2,
      T2 arg3,
      T3 arg4,
      T4 arg5)
    {
      return target is Method method && method._inst == null ? this._site.Target((CallSite) this._site, context, method._func, PythonOps.MethodCheckSelf(context, method, arg0), arg1, arg2, arg3, arg4, arg5) : ((CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4, arg5);
    }

    public override Delegate GetSelfTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>(this.SelfTarget);
    }

    public override Delegate GetSelflessTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, object>(this.SelflessTarget);
    }
  }

  private class MethodBinding<T0, T1, T2, T3, T4, T5> : Method.BaseMethodBinding
  {
    private CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, object>> _site;

    public MethodBinding(PythonInvokeBinder binder)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, object>>.Create((CallSiteBinder) binder);
    }

    public object SelfTarget(
      CallSite site,
      CodeContext context,
      object target,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3,
      T4 arg4,
      T5 arg5)
    {
      return target is Method method && method._inst != null ? this._site.Target((CallSite) this._site, context, method._func, method._inst, arg0, arg1, arg2, arg3, arg4, arg5) : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4, arg5);
    }

    public object SelflessTarget(
      CallSite site,
      CodeContext context,
      object target,
      object arg0,
      T0 arg1,
      T1 arg2,
      T2 arg3,
      T3 arg4,
      T4 arg5,
      T5 arg6)
    {
      return target is Method method && method._inst == null ? this._site.Target((CallSite) this._site, context, method._func, PythonOps.MethodCheckSelf(context, method, arg0), arg1, arg2, arg3, arg4, arg5, arg6) : ((CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
    }

    public override Delegate GetSelfTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, object>(this.SelfTarget);
    }

    public override Delegate GetSelflessTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, object>(this.SelflessTarget);
    }
  }

  private class MethodBinding<T0, T1, T2, T3, T4, T5, T6> : Method.BaseMethodBinding
  {
    private CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, object>> _site;

    public MethodBinding(PythonInvokeBinder binder)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, object>>.Create((CallSiteBinder) binder);
    }

    public object SelfTarget(
      CallSite site,
      CodeContext context,
      object target,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3,
      T4 arg4,
      T5 arg5,
      T6 arg6)
    {
      return target is Method method && method._inst != null ? this._site.Target((CallSite) this._site, context, method._func, method._inst, arg0, arg1, arg2, arg3, arg4, arg5, arg6) : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, T6, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
    }

    public object SelflessTarget(
      CallSite site,
      CodeContext context,
      object target,
      object arg0,
      T0 arg1,
      T1 arg2,
      T2 arg3,
      T3 arg4,
      T4 arg5,
      T5 arg6,
      T6 arg7)
    {
      return target is Method method && method._inst == null ? this._site.Target((CallSite) this._site, context, method._func, PythonOps.MethodCheckSelf(context, method, arg0), arg1, arg2, arg3, arg4, arg5, arg6, arg7) : ((CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    public override Delegate GetSelfTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, T6, object>(this.SelfTarget);
    }

    public override Delegate GetSelflessTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, object>(this.SelflessTarget);
    }
  }

  private class MethodBinding<T0, T1, T2, T3, T4, T5, T6, T7> : Method.BaseMethodBinding
  {
    private CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, object>> _site;

    public MethodBinding(PythonInvokeBinder binder)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, object>>.Create((CallSiteBinder) binder);
    }

    public object SelfTarget(
      CallSite site,
      CodeContext context,
      object target,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3,
      T4 arg4,
      T5 arg5,
      T6 arg6,
      T7 arg7)
    {
      return target is Method method && method._inst != null ? this._site.Target((CallSite) this._site, context, method._func, method._inst, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7) : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, T6, T7, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    public object SelflessTarget(
      CallSite site,
      CodeContext context,
      object target,
      object arg0,
      T0 arg1,
      T1 arg2,
      T2 arg3,
      T3 arg4,
      T4 arg5,
      T5 arg6,
      T6 arg7,
      T7 arg8)
    {
      return target is Method method && method._inst == null ? this._site.Target((CallSite) this._site, context, method._func, PythonOps.MethodCheckSelf(context, method, arg0), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) : ((CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }

    public override Delegate GetSelfTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, T6, T7, object>(this.SelfTarget);
    }

    public override Delegate GetSelflessTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, object>(this.SelflessTarget);
    }
  }

  private class MethodBinding<T0, T1, T2, T3, T4, T5, T6, T7, T8> : Method.BaseMethodBinding
  {
    private CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, object>> _site;

    public MethodBinding(PythonInvokeBinder binder)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, object>>.Create((CallSiteBinder) binder);
    }

    public object SelfTarget(
      CallSite site,
      CodeContext context,
      object target,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3,
      T4 arg4,
      T5 arg5,
      T6 arg6,
      T7 arg7,
      T8 arg8)
    {
      return target is Method method && method._inst != null ? this._site.Target((CallSite) this._site, context, method._func, method._inst, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }

    public object SelflessTarget(
      CallSite site,
      CodeContext context,
      object target,
      object arg0,
      T0 arg1,
      T1 arg2,
      T2 arg3,
      T3 arg4,
      T4 arg5,
      T5 arg6,
      T6 arg7,
      T7 arg8,
      T8 arg9)
    {
      return target is Method method && method._inst == null ? this._site.Target((CallSite) this._site, context, method._func, PythonOps.MethodCheckSelf(context, method, arg0), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) : ((CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
    }

    public override Delegate GetSelfTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, object>(this.SelfTarget);
    }

    public override Delegate GetSelflessTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, object>(this.SelflessTarget);
    }
  }

  private class MethodBinding<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> : Method.BaseMethodBinding
  {
    private CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> _site;

    public MethodBinding(PythonInvokeBinder binder)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>>.Create((CallSiteBinder) binder);
    }

    public object SelfTarget(
      CallSite site,
      CodeContext context,
      object target,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3,
      T4 arg4,
      T5 arg5,
      T6 arg6,
      T7 arg7,
      T8 arg8,
      T9 arg9)
    {
      return target is Method method && method._inst != null ? this._site.Target((CallSite) this._site, context, method._func, method._inst, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
    }

    public object SelflessTarget(
      CallSite site,
      CodeContext context,
      object target,
      object arg0,
      T0 arg1,
      T1 arg2,
      T2 arg3,
      T3 arg4,
      T4 arg5,
      T5 arg6,
      T6 arg7,
      T7 arg8,
      T8 arg9,
      T9 arg10)
    {
      return target is Method method && method._inst == null ? this._site.Target((CallSite) this._site, context, method._func, PythonOps.MethodCheckSelf(context, method, arg0), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) : ((CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    }

    public override Delegate GetSelfTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>(this.SelfTarget);
    }

    public override Delegate GetSelflessTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>(this.SelflessTarget);
    }
  }

  private class MethodBinding<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : Method.BaseMethodBinding
  {
    private CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> _site;

    public MethodBinding(PythonInvokeBinder binder)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>>.Create((CallSiteBinder) binder);
    }

    public object SelfTarget(
      CallSite site,
      CodeContext context,
      object target,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3,
      T4 arg4,
      T5 arg5,
      T6 arg6,
      T7 arg7,
      T8 arg8,
      T9 arg9,
      T10 arg10)
    {
      return target is Method method && method._inst != null ? this._site.Target((CallSite) this._site, context, method._func, method._inst, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    }

    public object SelflessTarget(
      CallSite site,
      CodeContext context,
      object target,
      object arg0,
      T0 arg1,
      T1 arg2,
      T2 arg3,
      T3 arg4,
      T4 arg5,
      T5 arg6,
      T6 arg7,
      T7 arg8,
      T8 arg9,
      T9 arg10,
      T10 arg11)
    {
      return target is Method method && method._inst == null ? this._site.Target((CallSite) this._site, context, method._func, PythonOps.MethodCheckSelf(context, method, arg0), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11) : ((CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
    }

    public override Delegate GetSelfTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>(this.SelfTarget);
    }

    public override Delegate GetSelflessTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>(this.SelflessTarget);
    }
  }

  private class MethodBinding<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : 
    Method.BaseMethodBinding
  {
    private CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> _site;

    public MethodBinding(PythonInvokeBinder binder)
    {
      this._site = CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>>.Create((CallSiteBinder) binder);
    }

    public object SelfTarget(
      CallSite site,
      CodeContext context,
      object target,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3,
      T4 arg4,
      T5 arg5,
      T6 arg6,
      T7 arg7,
      T8 arg8,
      T9 arg9,
      T10 arg10,
      T11 arg11)
    {
      return target is Method method && method._inst != null ? this._site.Target((CallSite) this._site, context, method._func, method._inst, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11) : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
    }

    public object SelflessTarget(
      CallSite site,
      CodeContext context,
      object target,
      object arg0,
      T0 arg1,
      T1 arg2,
      T2 arg3,
      T3 arg4,
      T4 arg5,
      T5 arg6,
      T6 arg7,
      T7 arg8,
      T8 arg9,
      T9 arg10,
      T10 arg11,
      T11 arg12)
    {
      return target is Method method && method._inst == null ? this._site.Target((CallSite) this._site, context, method._func, PythonOps.MethodCheckSelf(context, method, arg0), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12) : ((CallSite<Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>>) site).Update(site, context, target, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
    }

    public override Delegate GetSelfTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>(this.SelfTarget);
    }

    public override Delegate GetSelflessTarget()
    {
      return (Delegate) new Func<CallSite, CodeContext, object, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>(this.SelflessTarget);
    }
  }
}
