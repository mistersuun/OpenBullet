// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.LateBoundInitBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Modules;
using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
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
namespace IronPython.Runtime.Types;

internal class LateBoundInitBinder : DynamicMetaObjectBinder
{
  private readonly PythonType _newType;
  private readonly CallSignature _signature;
  public const int MaxFastLateBoundInitArgs = 6;

  public LateBoundInitBinder(PythonType type, CallSignature signature)
  {
    this._newType = type;
    this._signature = signature;
  }

  public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
  {
    DynamicMetaObject dynamicMetaObject1 = target;
    CodeContext context = (CodeContext) dynamicMetaObject1.Value;
    target = args[0];
    args = ArrayUtils.RemoveFirst<DynamicMetaObject>(args);
    ValidationInfo validationInfo = BindingHelpers.GetValidationInfo(target);
    PythonType pythonType = DynamicHelpers.GetPythonType(target.Value);
    BindingRestrictions restrictions = BindingRestrictions.Empty;
    System.Linq.Expressions.Expression expression;
    if (Builtin.isinstance(target.Value, this._newType) && this.NeedsInitCall((CodeContext) dynamicMetaObject1.Value, pythonType, args.Length))
    {
      PythonTypeSlot slot;
      pythonType.TryResolveSlot(context, "__init__", out slot);
      if (pythonType.IsMixedNewStyleOldStyle())
      {
        expression = (System.Linq.Expressions.Expression) this.MakeDynamicInitInvoke(context, args, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod("GetMixedMember"), dynamicMetaObject1.Expression, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Convert((System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) pythonType), typeof (PythonType)), Microsoft.Scripting.Ast.Utils.Convert(target.Expression, typeof (object)), Microsoft.Scripting.Ast.Utils.Constant((object) "__init__")), dynamicMetaObject1.Expression);
      }
      else
      {
        switch (slot)
        {
          case PythonFunction _:
            System.Linq.Expressions.Expression[] expressionArray = new System.Linq.Expressions.Expression[args.Length + 3];
            expressionArray[0] = dynamicMetaObject1.Expression;
            expressionArray[1] = (System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) slot);
            expressionArray[2] = target.Expression;
            for (int index = 0; index < args.Length; ++index)
              expressionArray[3 + index] = args[index].Expression;
            expression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Dynamic((CallSiteBinder) context.LanguageContext.Invoke(this._signature.InsertArgument(Argument.Simple)), typeof (object), expressionArray);
            break;
          case BuiltinMethodDescriptor _:
          case BuiltinFunction _:
            IList<MethodBase> targets = !(slot is BuiltinMethodDescriptor) ? ((BuiltinFunction) slot).Targets : ((BuiltinMethodDescriptor) slot).Template.Targets;
            PythonBinder binder = context.LanguageContext.Binder;
            DynamicMetaObject dynamicMetaObject2 = binder.CallMethod((DefaultOverloadResolver) new PythonOverloadResolver(binder, target, (IList<DynamicMetaObject>) args, this._signature, dynamicMetaObject1.Expression), targets, BindingRestrictions.Empty);
            expression = dynamicMetaObject2.Expression;
            restrictions = dynamicMetaObject2.Restrictions;
            break;
          default:
            expression = (System.Linq.Expressions.Expression) this.MakeDynamicInitInvoke(context, args, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod("GetInitSlotMember"), dynamicMetaObject1.Expression, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Convert((System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) this._newType), typeof (PythonType)), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Convert((System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) slot), typeof (PythonTypeSlot)), Microsoft.Scripting.Ast.Utils.Convert(target.Expression, typeof (object))), dynamicMetaObject1.Expression);
            break;
        }
      }
    }
    else
      expression = (System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.Empty();
    if (pythonType.TryResolveSlot(context, "__del__", out PythonTypeSlot _))
      expression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(expression, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod("InitializeForFinalization"), dynamicMetaObject1.Expression, Microsoft.Scripting.Ast.Utils.Convert(target.Expression, typeof (object))));
    return BindingHelpers.AddDynamicTestAndDefer((DynamicMetaObjectBinder) this, new DynamicMetaObject((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(expression, target.Expression), target.Restrict(target.LimitType).Restrictions.Merge(restrictions)), args, validationInfo);
  }

  public override T BindDelegate<T>(CallSite<T> site, object[] args)
  {
    if (args.Length <= 6)
    {
      CodeContext context = (CodeContext) args[0];
      object o = args[1];
      PythonType pythonType = DynamicHelpers.GetPythonType(o);
      PythonFunction target = (PythonFunction) null;
      string name = (string) null;
      if (!pythonType.TryResolveSlot(context, "__del__", out PythonTypeSlot _) && !pythonType.IsMixedNewStyleOldStyle())
      {
        if (Builtin.isinstance(o, this._newType) && this.NeedsInitCall(context, pythonType, args.Length))
        {
          PythonTypeSlot slot;
          pythonType.TryResolveSlot(context, "__init__", out slot);
          if (slot is PythonFunction)
          {
            name = "CallTarget";
            target = (PythonFunction) slot;
          }
        }
        else
          name = "EmptyCallTarget";
      }
      if (name != null)
      {
        int num = args.Length - 2;
        PythonInvokeBinder binder = context.LanguageContext.Invoke(this._signature.InsertArgument(Argument.Simple));
        if (num == 0)
        {
          LateBoundInitBinder.FastInitSite fastInitSite = new LateBoundInitBinder.FastInitSite(pythonType.Version, binder, target);
          return name == "CallTarget" ? (T) new Func<CallSite, CodeContext, object, object>(fastInitSite.CallTarget) : (T) new Func<CallSite, CodeContext, object, object>(fastInitSite.EmptyCallTarget);
        }
        Type[] array = ArrayUtils.ConvertAll<ParameterInfo, Type>(typeof (T).GetMethod("Invoke").GetParameters(), (Func<ParameterInfo, Type>) (x => x.ParameterType));
        Type type1;
        switch (args.Length)
        {
          case 3:
            type1 = typeof (LateBoundInitBinder.FastInitSite<>);
            break;
          case 4:
            type1 = typeof (LateBoundInitBinder.FastInitSite<,>);
            break;
          case 5:
            type1 = typeof (LateBoundInitBinder.FastInitSite<,,>);
            break;
          case 6:
            type1 = typeof (LateBoundInitBinder.FastInitSite<,,,>);
            break;
          case 7:
            type1 = typeof (LateBoundInitBinder.FastInitSite<,,,,>);
            break;
          default:
            throw new InvalidOperationException();
        }
        Type type2 = type1.MakeGenericType(ArrayUtils.ShiftLeft<Type>(array, 3));
        object instance = Activator.CreateInstance(type2, (object) pythonType.Version, (object) binder, (object) target);
        return (T) type2.GetMethod(name).CreateDelegate(typeof (T), instance);
      }
    }
    return base.BindDelegate<T>(site, args);
  }

  private bool NeedsInitCall(CodeContext context, PythonType type, int argCount)
  {
    if (!type.HasObjectInit(context) || type.IsMixedNewStyleOldStyle())
      return true;
    return this._newType.HasObjectNew(context) && argCount > 0;
  }

  private DynamicExpression MakeDynamicInitInvoke(
    CodeContext context,
    DynamicMetaObject[] args,
    System.Linq.Expressions.Expression initFunc,
    System.Linq.Expressions.Expression codeContext)
  {
    return System.Linq.Expressions.Expression.Dynamic((CallSiteBinder) context.LanguageContext.Invoke(this._signature), typeof (object), ArrayUtils.Insert<System.Linq.Expressions.Expression>(codeContext, initFunc, DynamicUtils.GetExpressions(args)));
  }

  private class FastInitSite
  {
    private readonly int _version;
    private readonly PythonFunction _slot;
    private readonly CallSite<Func<CallSite, CodeContext, PythonFunction, object, object>> _initSite;

    public FastInitSite(int version, PythonInvokeBinder binder, PythonFunction target)
    {
      this._version = version;
      this._slot = target;
      this._initSite = CallSite<Func<CallSite, CodeContext, PythonFunction, object, object>>.Create((CallSiteBinder) binder);
    }

    public object CallTarget(CallSite site, CodeContext context, object inst)
    {
      if (!(inst is IPythonObject pythonObject) || pythonObject.PythonType.Version != this._version)
        return ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, inst);
      object obj = this._initSite.Target((CallSite) this._initSite, context, this._slot, inst);
      return inst;
    }

    public object EmptyCallTarget(CallSite site, CodeContext context, object inst)
    {
      return inst is IPythonObject pythonObject && pythonObject.PythonType.Version == this._version || DynamicHelpers.GetPythonType(inst).Version == this._version ? inst : ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, inst);
    }
  }

  private class FastInitSite<T0>
  {
    private readonly int _version;
    private readonly PythonFunction _slot;
    private readonly CallSite<Func<CallSite, CodeContext, PythonFunction, object, T0, object>> _initSite;

    public FastInitSite(int version, PythonInvokeBinder binder, PythonFunction target)
    {
      this._version = version;
      this._slot = target;
      this._initSite = CallSite<Func<CallSite, CodeContext, PythonFunction, object, T0, object>>.Create((CallSiteBinder) binder);
    }

    public object CallTarget(CallSite site, CodeContext context, object inst, T0 arg0)
    {
      if (!(inst is IPythonObject pythonObject) || pythonObject.PythonType.Version != this._version)
        return ((CallSite<Func<CallSite, CodeContext, object, T0, object>>) site).Update(site, context, inst, arg0);
      object obj = this._initSite.Target((CallSite) this._initSite, context, this._slot, inst, arg0);
      return inst;
    }

    public object EmptyCallTarget(CallSite site, CodeContext context, object inst, T0 arg0)
    {
      return inst is IPythonObject pythonObject && pythonObject.PythonType.Version == this._version || DynamicHelpers.GetPythonType(inst).Version == this._version ? inst : ((CallSite<Func<CallSite, CodeContext, object, T0, object>>) site).Update(site, context, inst, arg0);
    }
  }

  private class FastInitSite<T0, T1>
  {
    private readonly int _version;
    private readonly PythonFunction _slot;
    private readonly CallSite<Func<CallSite, CodeContext, PythonFunction, object, T0, T1, object>> _initSite;

    public FastInitSite(int version, PythonInvokeBinder binder, PythonFunction target)
    {
      this._version = version;
      this._slot = target;
      this._initSite = CallSite<Func<CallSite, CodeContext, PythonFunction, object, T0, T1, object>>.Create((CallSiteBinder) binder);
    }

    public object CallTarget(CallSite site, CodeContext context, object inst, T0 arg0, T1 arg1)
    {
      if (!(inst is IPythonObject pythonObject) || pythonObject.PythonType.Version != this._version)
        return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, object>>) site).Update(site, context, inst, arg0, arg1);
      object obj = this._initSite.Target((CallSite) this._initSite, context, this._slot, inst, arg0, arg1);
      return inst;
    }

    public object EmptyCallTarget(
      CallSite site,
      CodeContext context,
      object inst,
      T0 arg0,
      T1 arg1)
    {
      return inst is IPythonObject pythonObject && pythonObject.PythonType.Version == this._version || DynamicHelpers.GetPythonType(inst).Version == this._version ? inst : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, object>>) site).Update(site, context, inst, arg0, arg1);
    }
  }

  private class FastInitSite<T0, T1, T2>
  {
    private readonly int _version;
    private readonly PythonFunction _slot;
    private readonly CallSite<Func<CallSite, CodeContext, PythonFunction, object, T0, T1, T2, object>> _initSite;

    public FastInitSite(int version, PythonInvokeBinder binder, PythonFunction target)
    {
      this._version = version;
      this._slot = target;
      this._initSite = CallSite<Func<CallSite, CodeContext, PythonFunction, object, T0, T1, T2, object>>.Create((CallSiteBinder) binder);
    }

    public object CallTarget(
      CallSite site,
      CodeContext context,
      object inst,
      T0 arg0,
      T1 arg1,
      T2 arg2)
    {
      if (!(inst is IPythonObject pythonObject) || pythonObject.PythonType.Version != this._version)
        return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, object>>) site).Update(site, context, inst, arg0, arg1, arg2);
      object obj = this._initSite.Target((CallSite) this._initSite, context, this._slot, inst, arg0, arg1, arg2);
      return inst;
    }

    public object EmptyCallTarget(
      CallSite site,
      CodeContext context,
      object inst,
      T0 arg0,
      T1 arg1,
      T2 arg2)
    {
      return inst is IPythonObject pythonObject && pythonObject.PythonType.Version == this._version || DynamicHelpers.GetPythonType(inst).Version == this._version ? inst : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, object>>) site).Update(site, context, inst, arg0, arg1, arg2);
    }
  }

  private class FastInitSite<T0, T1, T2, T3>
  {
    private readonly int _version;
    private readonly PythonFunction _slot;
    private readonly CallSite<Func<CallSite, CodeContext, PythonFunction, object, T0, T1, T2, T3, object>> _initSite;

    public FastInitSite(int version, PythonInvokeBinder binder, PythonFunction target)
    {
      this._version = version;
      this._slot = target;
      this._initSite = CallSite<Func<CallSite, CodeContext, PythonFunction, object, T0, T1, T2, T3, object>>.Create((CallSiteBinder) binder);
    }

    public object CallTarget(
      CallSite site,
      CodeContext context,
      object inst,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3)
    {
      if (!(inst is IPythonObject pythonObject) || pythonObject.PythonType.Version != this._version)
        return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>>) site).Update(site, context, inst, arg0, arg1, arg2, arg3);
      object obj = this._initSite.Target((CallSite) this._initSite, context, this._slot, inst, arg0, arg1, arg2, arg3);
      return inst;
    }

    public object EmptyCallTarget(
      CallSite site,
      CodeContext context,
      object inst,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3)
    {
      return inst is IPythonObject pythonObject && pythonObject.PythonType.Version == this._version || DynamicHelpers.GetPythonType(inst).Version == this._version ? inst : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>>) site).Update(site, context, inst, arg0, arg1, arg2, arg3);
    }
  }

  private class FastInitSite<T0, T1, T2, T3, T4>
  {
    private readonly int _version;
    private readonly PythonFunction _slot;
    private readonly CallSite<Func<CallSite, CodeContext, PythonFunction, object, T0, T1, T2, T3, T4, object>> _initSite;

    public FastInitSite(int version, PythonInvokeBinder binder, PythonFunction target)
    {
      this._version = version;
      this._slot = target;
      this._initSite = CallSite<Func<CallSite, CodeContext, PythonFunction, object, T0, T1, T2, T3, T4, object>>.Create((CallSiteBinder) binder);
    }

    public object CallTarget(
      CallSite site,
      CodeContext context,
      object inst,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3,
      T4 arg4)
    {
      if (!(inst is IPythonObject pythonObject) || pythonObject.PythonType.Version != this._version)
        return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>>) site).Update(site, context, inst, arg0, arg1, arg2, arg3, arg4);
      object obj = this._initSite.Target((CallSite) this._initSite, context, this._slot, inst, arg0, arg1, arg2, arg3, arg4);
      return inst;
    }

    public object EmptyCallTarget(
      CallSite site,
      CodeContext context,
      object inst,
      T0 arg0,
      T1 arg1,
      T2 arg2,
      T3 arg3,
      T4 arg4)
    {
      return inst is IPythonObject pythonObject && pythonObject.PythonType.Version == this._version || DynamicHelpers.GetPythonType(inst).Version == this._version ? inst : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>>) site).Update(site, context, inst, arg0, arg1, arg2, arg3, arg4);
    }
  }
}
