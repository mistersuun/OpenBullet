// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.SlotOrFunction
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal sealed class SlotOrFunction
{
  private readonly BindingTarget _function;
  private readonly DynamicMetaObject _target;
  private readonly PythonTypeSlot _slot;
  public static readonly SlotOrFunction Empty = new SlotOrFunction(new DynamicMetaObject((Expression) Microsoft.Scripting.Ast.Utils.Empty(), BindingRestrictions.Empty));

  private SlotOrFunction()
  {
  }

  public SlotOrFunction(BindingTarget function, DynamicMetaObject target)
  {
    this._target = target;
    this._function = function;
  }

  public SlotOrFunction(DynamicMetaObject target) => this._target = target;

  public SlotOrFunction(DynamicMetaObject target, PythonTypeSlot slot)
  {
    this._target = target;
    this._slot = slot;
  }

  public NarrowingLevel NarrowingLevel
  {
    get => this._function != null ? this._function.NarrowingLevel : NarrowingLevel.None;
  }

  public Type ReturnType => this._target.GetLimitType();

  public bool MaybeNotImplemented
  {
    get
    {
      if (this._function == null)
        return true;
      MethodInfo reflectionInfo = this._function.Overload.ReflectionInfo as MethodInfo;
      return reflectionInfo != (MethodInfo) null && reflectionInfo.ReturnTypeCustomAttributes.IsDefined(typeof (MaybeNotImplementedAttribute), false);
    }
  }

  public bool Success
  {
    get => this._function != null ? this._function.Success : this != SlotOrFunction.Empty;
  }

  public bool IsNull
  {
    get
    {
      return this._slot is PythonTypeUserDescriptorSlot && ((PythonTypeUserDescriptorSlot) this._slot).Value == null;
    }
  }

  public DynamicMetaObject Target => this._target;

  public static bool GetCombinedTargets(
    SlotOrFunction fCand,
    SlotOrFunction rCand,
    out SlotOrFunction fTarget,
    out SlotOrFunction rTarget)
  {
    fTarget = rTarget = SlotOrFunction.Empty;
    if (fCand.Success)
    {
      if (rCand.Success)
      {
        if (fCand.NarrowingLevel <= rCand.NarrowingLevel)
        {
          fTarget = fCand;
          rTarget = rCand;
        }
        else
        {
          fTarget = SlotOrFunction.Empty;
          rTarget = rCand;
        }
      }
      else
        fTarget = fCand;
    }
    else
    {
      if (!rCand.Success)
        return false;
      rTarget = rCand;
    }
    return true;
  }

  public bool ShouldWarn(PythonContext context, out WarningInfo info)
  {
    if (this._function != null)
      return BindingWarnings.ShouldWarn(context, this._function.Overload, out info);
    info = (WarningInfo) null;
    return false;
  }

  public static SlotOrFunction GetSlotOrFunction(
    PythonContext state,
    string op,
    params DynamicMetaObject[] types)
  {
    SlotOrFunction res;
    if (SlotOrFunction.TryGetBinder(state, types, op, (string) null, out res))
    {
      if (res != SlotOrFunction.Empty)
        return res;
    }
    else
    {
      PythonTypeSlot slot;
      if (MetaPythonObject.GetPythonType(types[0]).TryResolveSlot(state.SharedContext, op, out slot))
      {
        ParameterExpression tmp = Expression.Variable(typeof (object), "slotVal");
        Expression[] array = new Expression[types.Length - 1];
        for (int index = 1; index < types.Length; ++index)
          array[index - 1] = types[index].Expression;
        return new SlotOrFunction(new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
        {
          tmp
        }, (Expression) MetaPythonObject.MakeTryGetTypeMember(state, slot, tmp, types[0].Expression, (Expression) Expression.Call(typeof (DynamicHelpers).GetMethod("GetPythonType"), types[0].Expression)), (Expression) Expression.Dynamic((CallSiteBinder) state.Invoke(new CallSignature(array.Length)), typeof (object), ArrayUtils.Insert<Expression>(Microsoft.Scripting.Ast.Utils.Constant((object) state.SharedContext), (Expression) tmp, array))), BindingRestrictions.Combine((IList<DynamicMetaObject>) types).Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(types[0].Expression, types[0].GetLimitType()))), slot);
      }
    }
    return SlotOrFunction.Empty;
  }

  internal static bool TryGetBinder(
    PythonContext state,
    DynamicMetaObject[] types,
    string op,
    string rop,
    out SlotOrFunction res)
  {
    return SlotOrFunction.TryGetBinder(state, types, op, rop, out res, out PythonType _);
  }

  internal static bool TryGetBinder(
    PythonContext state,
    DynamicMetaObject[] types,
    string op,
    string rop,
    out SlotOrFunction res,
    out PythonType declaringType)
  {
    declaringType = (PythonType) null;
    DynamicMetaObject type = types[0];
    BuiltinFunction function1;
    if (!BindingHelpers.TryGetStaticFunction(state, op, type, out function1))
    {
      res = SlotOrFunction.Empty;
      return false;
    }
    BuiltinFunction builtinFunction = SlotOrFunction.CheckAlwaysNotImplemented(function1);
    DynamicMetaObject dynamicMetaObject = (DynamicMetaObject) null;
    BuiltinFunction function2 = (BuiltinFunction) null;
    if (types.Length > 1)
    {
      dynamicMetaObject = types[1];
      if (!BindingHelpers.IsSubclassOf(type, dynamicMetaObject) && !BindingHelpers.TryGetStaticFunction(state, rop, dynamicMetaObject, out function2))
      {
        res = SlotOrFunction.Empty;
        return false;
      }
      function2 = SlotOrFunction.CheckAlwaysNotImplemented(function2);
    }
    if (function2 == builtinFunction)
      function2 = (BuiltinFunction) null;
    else if (function2 != null && BindingHelpers.IsSubclassOf(dynamicMetaObject, type))
      builtinFunction = (BuiltinFunction) null;
    PythonOverloadResolver resolver = new PythonOverloadResolver(state.Binder, (IList<DynamicMetaObject>) types, new CallSignature(types.Length), Microsoft.Scripting.Ast.Utils.Constant((object) state.SharedContext));
    DynamicMetaObject target1;
    BindingTarget target2;
    if (builtinFunction == null)
    {
      if (function2 == null)
      {
        target1 = (DynamicMetaObject) null;
        target2 = (BindingTarget) null;
      }
      else
      {
        declaringType = DynamicHelpers.GetPythonTypeFromType(function2.DeclaringType);
        target1 = state.Binder.CallMethod((DefaultOverloadResolver) resolver, function2.Targets, BindingRestrictions.Empty, (string) null, NarrowingLevel.None, NarrowingLevel.Two, out target2);
      }
    }
    else if (function2 == null)
    {
      declaringType = DynamicHelpers.GetPythonTypeFromType(builtinFunction.DeclaringType);
      target1 = state.Binder.CallMethod((DefaultOverloadResolver) resolver, builtinFunction.Targets, BindingRestrictions.Empty, (string) null, NarrowingLevel.None, NarrowingLevel.Two, out target2);
    }
    else
    {
      List<MethodBase> existing = new List<MethodBase>();
      existing.AddRange((IEnumerable<MethodBase>) builtinFunction.Targets);
      foreach (MethodBase target3 in (IEnumerable<MethodBase>) function2.Targets)
      {
        if (!SlotOrFunction.ContainsMethodSignature((IList<MethodBase>) existing, target3))
          existing.Add(target3);
      }
      target1 = state.Binder.CallMethod((DefaultOverloadResolver) resolver, (IList<MethodBase>) existing.ToArray(), BindingRestrictions.Empty, (string) null, NarrowingLevel.None, NarrowingLevel.Two, out target2);
      foreach (MethodBase target4 in (IEnumerable<MethodBase>) function2.Targets)
      {
        if (target2.Overload.ReflectionInfo == target4)
        {
          declaringType = DynamicHelpers.GetPythonTypeFromType(function2.DeclaringType);
          break;
        }
      }
      if (declaringType == null)
        declaringType = DynamicHelpers.GetPythonTypeFromType(builtinFunction.DeclaringType);
    }
    res = target1 == null ? SlotOrFunction.Empty : new SlotOrFunction(target2, target1);
    return true;
  }

  private static BuiltinFunction CheckAlwaysNotImplemented(BuiltinFunction xBf)
  {
    if (xBf != null)
    {
      bool flag = false;
      foreach (MethodBase target in (IEnumerable<MethodBase>) xBf.Targets)
      {
        if (target.GetReturnType() != typeof (NotImplementedType) || target.IsDefined(typeof (Python3WarningAttribute), true))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        xBf = (BuiltinFunction) null;
    }
    return xBf;
  }

  private static bool ContainsMethodSignature(IList<MethodBase> existing, MethodBase check)
  {
    ParameterInfo[] parameters = check.GetParameters();
    foreach (MethodBase mb in (IEnumerable<MethodBase>) existing)
    {
      if (SlotOrFunction.MatchesMethodSignature(parameters, mb))
        return true;
    }
    return false;
  }

  private static bool MatchesMethodSignature(ParameterInfo[] pis, MethodBase mb)
  {
    ParameterInfo[] parameters = mb.GetParameters();
    if (pis.Length != parameters.Length)
      return false;
    for (int index = 0; index < pis.Length; ++index)
    {
      if (pis[index].ParameterType != parameters[index].ParameterType)
        return false;
    }
    return true;
  }
}
