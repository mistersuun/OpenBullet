// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.BindingHelpers
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler.Ast;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal static class BindingHelpers
{
  internal static bool TryGetStaticFunction(
    PythonContext state,
    string op,
    DynamicMetaObject mo,
    out BuiltinFunction function)
  {
    PythonType pythonType = MetaPythonObject.GetPythonType(mo);
    function = (BuiltinFunction) null;
    PythonTypeSlot slot;
    object o;
    if (!string.IsNullOrEmpty(op) && pythonType.TryResolveSlot(state.SharedContext, op, out slot) && slot.TryGetValue(state.SharedContext, (object) null, pythonType, out o))
    {
      function = BindingHelpers.TryConvertToBuiltinFunction(o);
      if (function == null)
        return false;
    }
    return true;
  }

  internal static bool IsNoThrow(DynamicMetaObjectBinder action)
  {
    return action is PythonGetMemberBinder pythonGetMemberBinder && pythonGetMemberBinder.IsNoThrow;
  }

  internal static DynamicMetaObject FilterShowCls(
    DynamicMetaObject codeContext,
    DynamicMetaObjectBinder action,
    DynamicMetaObject res,
    System.Linq.Expressions.Expression failure)
  {
    return action is IPythonSite ? new DynamicMetaObject((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Condition((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod("IsClsVisible"), codeContext.Expression), Microsoft.Scripting.Ast.Utils.Convert(res.Expression, typeof (object)), Microsoft.Scripting.Ast.Utils.Convert(failure, typeof (object))), res.Restrictions) : res;
  }

  internal static CallSignature GetCallSignature(DynamicMetaObjectBinder action)
  {
    switch (action)
    {
      case PythonInvokeBinder pythonInvokeBinder:
        return pythonInvokeBinder.Signature;
      case InvokeBinder invokeBinder:
        return BindingHelpers.CallInfoToSignature(invokeBinder.CallInfo);
      case InvokeMemberBinder invokeMemberBinder:
        return BindingHelpers.CallInfoToSignature(invokeMemberBinder.CallInfo);
      default:
        return BindingHelpers.CallInfoToSignature((action as CreateInstanceBinder).CallInfo);
    }
  }

  public static System.Linq.Expressions.Expression Invoke(
    System.Linq.Expressions.Expression codeContext,
    PythonContext binder,
    Type resultType,
    CallSignature signature,
    params System.Linq.Expressions.Expression[] args)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Dynamic((CallSiteBinder) binder.Invoke(signature), resultType, ArrayUtils.Insert<System.Linq.Expressions.Expression>(codeContext, args));
  }

  internal static DynamicMetaObject GenericInvokeMember(
    InvokeMemberBinder action,
    ValidationInfo valInfo,
    DynamicMetaObject target,
    DynamicMetaObject[] args)
  {
    return target.NeedsDeferral() ? action.Defer(args) : BindingHelpers.AddDynamicTestAndDefer((DynamicMetaObjectBinder) action, action.FallbackInvoke(new DynamicMetaObject(Binders.Get(PythonContext.GetCodeContext((DynamicMetaObjectBinder) action), PythonContext.GetPythonContext((DynamicMetaObjectBinder) action), typeof (object), action.Name, target.Expression), BindingRestrictionsHelpers.GetRuntimeTypeRestriction(target)), args, (DynamicMetaObject) null), args, valInfo);
  }

  internal static bool NeedsDeferral(DynamicMetaObject[] args)
  {
    foreach (DynamicMetaObject self in args)
    {
      if (self.NeedsDeferral())
        return true;
    }
    return false;
  }

  internal static CallSignature CallInfoToSignature(CallInfo callInfo)
  {
    Argument[] objArray = new Argument[callInfo.ArgumentCount];
    int num = callInfo.ArgumentCount - callInfo.ArgumentNames.Count;
    int index;
    for (index = 0; index < num; ++index)
      objArray[index] = new Argument(ArgumentType.Simple);
    foreach (string argumentName in callInfo.ArgumentNames)
      objArray[index++] = new Argument(ArgumentType.Named, argumentName);
    return new CallSignature(objArray);
  }

  internal static Type GetCompatibleType(Type t, Type otherType)
  {
    if (t != otherType)
    {
      if (t.IsAssignableFrom(otherType))
        t = otherType;
      else if (!otherType.IsAssignableFrom(t))
        t = typeof (object);
    }
    return t;
  }

  internal static bool IsSubclassOf(DynamicMetaObject xType, DynamicMetaObject yType)
  {
    return MetaPythonObject.GetPythonType(xType).IsSubclassOf(MetaPythonObject.GetPythonType(yType));
  }

  private static BuiltinFunction TryConvertToBuiltinFunction(object o)
  {
    return o is BuiltinMethodDescriptor methodDescriptor ? methodDescriptor.Template : o as BuiltinFunction;
  }

  internal static DynamicMetaObject AddDynamicTestAndDefer(
    DynamicMetaObjectBinder operation,
    DynamicMetaObject res,
    DynamicMetaObject[] args,
    ValidationInfo typeTest,
    params ParameterExpression[] temps)
  {
    return BindingHelpers.AddDynamicTestAndDefer(operation, res, args, typeTest, (Type) null, temps);
  }

  internal static DynamicMetaObject AddDynamicTestAndDefer(
    DynamicMetaObjectBinder operation,
    DynamicMetaObject res,
    DynamicMetaObject[] args,
    ValidationInfo typeTest,
    Type deferType,
    params ParameterExpression[] temps)
  {
    if (typeTest != null && typeTest.Test != null)
    {
      DynamicMetaObjectBinder metaObjectBinder = operation;
      Type type = deferType;
      if ((object) type == null)
        type = typeof (object);
      System.Linq.Expressions.Expression updateExpression = metaObjectBinder.GetUpdateExpression(type);
      Type compatibleType = BindingHelpers.GetCompatibleType(updateExpression.Type, res.Expression.Type);
      res = new DynamicMetaObject((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Condition(typeTest.Test, Microsoft.Scripting.Ast.Utils.Convert(res.Expression, compatibleType), Microsoft.Scripting.Ast.Utils.Convert(updateExpression, compatibleType)), res.Restrictions);
    }
    if (temps.Length != 0)
      res = new DynamicMetaObject((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) temps, res.Expression), res.Restrictions, (object) null);
    return res;
  }

  internal static ValidationInfo GetValidationInfo(DynamicMetaObject tested, PythonType type)
  {
    return new ValidationInfo((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.AndAlso((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.TypeEqual(tested.Expression, type.UnderlyingSystemType), (System.Linq.Expressions.Expression) BindingHelpers.CheckTypeVersion(Microsoft.Scripting.Ast.Utils.Convert(tested.Expression, type.UnderlyingSystemType), type.Version)));
  }

  internal static ValidationInfo GetValidationInfo(params DynamicMetaObject[] args)
  {
    System.Linq.Expressions.Expression expression = (System.Linq.Expressions.Expression) null;
    for (int index = 0; index < args.Length; ++index)
    {
      if (args[index].HasValue && args[index].Value is IPythonObject pythonObject)
      {
        System.Linq.Expressions.Expression right1 = (System.Linq.Expressions.Expression) BindingHelpers.CheckTypeVersion(Microsoft.Scripting.Ast.Utils.Convert(args[index].Expression, pythonObject.GetType()), pythonObject.PythonType.Version);
        System.Linq.Expressions.Expression right2 = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.AndAlso((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.TypeEqual(args[index].Expression, pythonObject.GetType()), right1);
        expression = expression == null ? right2 : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.AndAlso(expression, right2);
      }
    }
    return new ValidationInfo(expression);
  }

  internal static MethodCallExpression CheckTypeVersion(System.Linq.Expressions.Expression tested, int version)
  {
    FieldInfo field = tested.Type.GetField(".class");
    return field == (FieldInfo) null ? System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod(nameof (CheckTypeVersion)), Microsoft.Scripting.Ast.Utils.Convert(tested, typeof (object)), Microsoft.Scripting.Ast.Utils.Constant((object) version)) : System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod("CheckSpecificTypeVersion"), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field(tested, field), Microsoft.Scripting.Ast.Utils.Constant((object) version));
  }

  internal static System.Linq.Expressions.Expression AddRecursionCheck(
    PythonContext pyContext,
    System.Linq.Expressions.Expression expr)
  {
    ParameterExpression left = System.Linq.Expressions.Expression.Variable(expr.Type, "callres");
    expr = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      left
    }, (System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.Try((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod("FunctionPushFrame"), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) pyContext)), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) left, expr)).Finally((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod("FunctionPopFrame"))), (System.Linq.Expressions.Expression) left);
    return expr;
  }

  internal static System.Linq.Expressions.Expression CreateBinderStateExpression()
  {
    return (System.Linq.Expressions.Expression) PythonAst._globalContext;
  }

  internal static DynamicMetaObject InvokeFallback(
    DynamicMetaObjectBinder action,
    System.Linq.Expressions.Expression codeContext,
    DynamicMetaObject target,
    DynamicMetaObject[] args)
  {
    switch (action)
    {
      case InvokeBinder invokeBinder:
        return invokeBinder.FallbackInvoke(target, args);
      case PythonInvokeBinder pythonInvokeBinder:
        return pythonInvokeBinder.Fallback(codeContext, target, args);
      default:
        throw new InvalidOperationException();
    }
  }

  internal static System.Linq.Expressions.Expression TypeErrorForProtectedMember(
    Type type,
    string name)
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Throw((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod(nameof (TypeErrorForProtectedMember)), Microsoft.Scripting.Ast.Utils.Constant((object) type), Microsoft.Scripting.Ast.Utils.Constant((object) name)), typeof (object));
  }

  internal static DynamicMetaObject TypeErrorGenericMethod(
    Type type,
    string name,
    BindingRestrictions restrictions)
  {
    return new DynamicMetaObject((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Throw((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod("TypeErrorForGenericMethod"), Microsoft.Scripting.Ast.Utils.Constant((object) type), Microsoft.Scripting.Ast.Utils.Constant((object) name)), typeof (object)), restrictions);
  }

  internal static bool IsDataMember(object p)
  {
    switch (p)
    {
      case PythonFunction _:
      case BuiltinFunction _:
      case PythonType _:
      case BuiltinMethodDescriptor _:
      case OldClass _:
      case staticmethod _:
      case classmethod _:
      case Method _:
label_2:
        return false;
      default:
        if ((object) (p as Delegate) == null)
          return true;
        goto label_2;
    }
  }

  internal static DynamicMetaObject AddPythonBoxing(DynamicMetaObject res)
  {
    if (res.Expression.Type.IsValueType())
      res = new DynamicMetaObject(BindingHelpers.AddPythonBoxing(res.Expression), res.Restrictions);
    return res;
  }

  internal static System.Linq.Expressions.Expression AddPythonBoxing(System.Linq.Expressions.Expression res)
  {
    return Microsoft.Scripting.Ast.Utils.Convert(res, typeof (object));
  }

  internal static DynamicMetaObject[] GetComArguments(DynamicMetaObject[] args)
  {
    DynamicMetaObject[] dynamicMetaObjectArray = (DynamicMetaObject[]) null;
    for (int index1 = 0; index1 < args.Length; ++index1)
    {
      DynamicMetaObject comArgument = BindingHelpers.GetComArgument(args[index1]);
      if (comArgument != args[index1])
      {
        if (dynamicMetaObjectArray == null)
        {
          dynamicMetaObjectArray = new DynamicMetaObject[args.Length];
          for (int index2 = 0; index2 < index1; ++index2)
            dynamicMetaObjectArray[index2] = args[index2];
        }
        dynamicMetaObjectArray[index1] = comArgument;
      }
      else if (dynamicMetaObjectArray != null)
        dynamicMetaObjectArray[index1] = args[index1];
    }
    return dynamicMetaObjectArray ?? args;
  }

  internal static DynamicMetaObject GetComArgument(DynamicMetaObject arg)
  {
    if (arg is IComConvertible comConvertible)
      return comConvertible.GetComMetaObject();
    if (arg.Value != null)
    {
      Type type = arg.Value.GetType();
      if (type == typeof (BigInteger))
        return new DynamicMetaObject((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Convert(Microsoft.Scripting.Ast.Utils.Convert(arg.Expression, typeof (BigInteger)), typeof (double)), BindingRestrictions.GetTypeRestriction(arg.Expression, type));
    }
    return arg;
  }

  internal static BuiltinFunction.BindingResult CheckLightThrow(
    DynamicMetaObjectBinder call,
    DynamicMetaObject res,
    BindingTarget target)
  {
    return new BuiltinFunction.BindingResult(target, BindingHelpers.CheckLightThrowMO(call, res, target));
  }

  internal static DynamicMetaObject CheckLightThrowMO(
    DynamicMetaObjectBinder call,
    DynamicMetaObject res,
    BindingTarget target)
  {
    if (target.Success && target.Overload.ReflectionInfo.IsDefined(typeof (LightThrowingAttribute), false) && !call.SupportsLightThrow())
      res = new DynamicMetaObject(LightExceptions.CheckAndThrow(res.Expression), res.Restrictions);
    return res;
  }
}
