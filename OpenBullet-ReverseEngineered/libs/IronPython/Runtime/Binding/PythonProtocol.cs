// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonProtocol
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.ComInterop;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace IronPython.Runtime.Binding;

internal static class PythonProtocol
{
  private const string DisallowCoerce = "DisallowCoerce";

  internal static DynamicMetaObject ConvertToBool(
    DynamicMetaObjectBinder conversion,
    DynamicMetaObject self)
  {
    SlotOrFunction slotOrFunction1 = SlotOrFunction.GetSlotOrFunction(PythonContext.GetPythonContext(conversion), "__nonzero__", self);
    if (slotOrFunction1.Success)
      return slotOrFunction1.Target.Expression.Type != typeof (bool) ? new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("ThrowingConvertToNonZero"), slotOrFunction1.Target.Expression), slotOrFunction1.Target.Restrictions) : slotOrFunction1.Target;
    SlotOrFunction slotOrFunction2 = SlotOrFunction.GetSlotOrFunction(PythonContext.GetPythonContext(conversion), "__len__", self);
    return slotOrFunction2.Success ? new DynamicMetaObject(PythonProtocol.GetConvertByLengthBody(PythonContext.GetPythonContext(conversion), slotOrFunction2.Target.Expression), slotOrFunction2.Target.Restrictions) : (DynamicMetaObject) null;
  }

  private static Expression GetConvertByLengthBody(PythonContext state, Expression call)
  {
    Expression left = call;
    if (call.Type != typeof (int))
      left = (Expression) Expression.Dynamic((CallSiteBinder) state.Convert(typeof (int), ConversionResultKind.ExplicitCast), typeof (int), call);
    return (Expression) Expression.NotEqual(left, Microsoft.Scripting.Ast.Utils.Constant((object) 0));
  }

  internal static DynamicMetaObject Call(
    DynamicMetaObjectBinder call,
    DynamicMetaObject target,
    DynamicMetaObject[] args)
  {
    if (target.NeedsDeferral())
      return call.Defer(ArrayUtils.Insert<DynamicMetaObject>(target, args));
    foreach (DynamicMetaObject self in args)
    {
      if (self.NeedsDeferral())
      {
        PythonProtocol.RestrictTypes(args);
        return call.Defer(ArrayUtils.Insert<DynamicMetaObject>(target, args));
      }
    }
    DynamicMetaObject dynamicMetaObject = target.Restrict(target.GetLimitType());
    ValidationInfo validationInfo = BindingHelpers.GetValidationInfo(target);
    PythonType pythonType = DynamicHelpers.GetPythonType(target.Value);
    PythonContext pythonContext = PythonContext.GetPythonContext(call);
    PythonTypeSlot slot;
    if (typeof (Delegate).IsAssignableFrom(target.GetLimitType()) || !pythonType.TryResolveSlot(pythonContext.SharedContext, "__call__", out slot))
      return (DynamicMetaObject) null;
    ConditionalBuilder builder = new ConditionalBuilder(call);
    slot.MakeGetExpression(pythonContext.Binder, PythonContext.GetCodeContext(call), dynamicMetaObject, PythonProtocol.GetPythonType(dynamicMetaObject), builder);
    if (!builder.IsFinal)
      builder.FinishCondition(PythonProtocol.GetCallError(call, dynamicMetaObject));
    Expression[] expressionArray = ArrayUtils.Insert<Expression>(PythonContext.GetCodeContext(call), builder.GetMetaObject().Expression, DynamicUtils.GetExpressions(args));
    Expression expression1 = (Expression) Expression.Dynamic((CallSiteBinder) PythonContext.GetPythonContext(call).Invoke(BindingHelpers.GetCallSignature(call)), typeof (object), expressionArray);
    Expression expression2 = (Expression) Expression.TryFinally((Expression) Expression.Block((Expression) Expression.Call(typeof (PythonOps).GetMethod("FunctionPushFrame"), (Expression) Expression.Constant((object) pythonContext)), expression1), (Expression) Expression.Call(typeof (PythonOps).GetMethod("FunctionPopFrame")));
    return BindingHelpers.AddDynamicTestAndDefer(call, new DynamicMetaObject(expression2, dynamicMetaObject.Restrictions.Merge(BindingRestrictions.Combine((IList<DynamicMetaObject>) args))), args, validationInfo);
  }

  private static DynamicMetaObject GetPythonType(DynamicMetaObject self)
  {
    PythonType pythonType = DynamicHelpers.GetPythonType(self.Value);
    return pythonType.IsSystemType ? new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) pythonType), BindingRestrictions.Empty, (object) pythonType) : new DynamicMetaObject((Expression) Expression.Property((Expression) Expression.Convert(self.Expression, typeof (IPythonObject)), PythonTypeInfo._IPythonObject.PythonType), BindingRestrictions.Empty, (object) pythonType);
  }

  private static Expression GetCallError(DynamicMetaObjectBinder binder, DynamicMetaObject self)
  {
    return binder.Throw((Expression) Expression.Call(typeof (PythonOps).GetMethod("UncallableError"), Microsoft.Scripting.Ast.Utils.Convert(self.Expression, typeof (object))));
  }

  public static DynamicMetaObject Operation(
    BinaryOperationBinder operation,
    DynamicMetaObject target,
    DynamicMetaObject arg,
    DynamicMetaObject errorSuggestion)
  {
    DynamicMetaObject[] args = new DynamicMetaObject[2]
    {
      target,
      arg
    };
    if (BindingHelpers.NeedsDeferral(args))
      return operation.Defer(target, arg);
    ValidationInfo validationInfo = BindingHelpers.GetValidationInfo(args);
    PythonOperationKind? nullable = new PythonOperationKind?();
    switch (operation.Operation)
    {
      case ExpressionType.Add:
        nullable = new PythonOperationKind?(PythonOperationKind.Add);
        break;
      case ExpressionType.And:
        nullable = new PythonOperationKind?(PythonOperationKind.BitwiseAnd);
        break;
      case ExpressionType.Divide:
        nullable = new PythonOperationKind?(PythonOperationKind.Divide);
        break;
      case ExpressionType.Equal:
        nullable = new PythonOperationKind?(PythonOperationKind.Equal);
        break;
      case ExpressionType.ExclusiveOr:
        nullable = new PythonOperationKind?(PythonOperationKind.ExclusiveOr);
        break;
      case ExpressionType.GreaterThan:
        nullable = new PythonOperationKind?(PythonOperationKind.GreaterThan);
        break;
      case ExpressionType.GreaterThanOrEqual:
        nullable = new PythonOperationKind?(PythonOperationKind.GreaterThanOrEqual);
        break;
      case ExpressionType.LeftShift:
        nullable = new PythonOperationKind?(PythonOperationKind.LeftShift);
        break;
      case ExpressionType.LessThan:
        nullable = new PythonOperationKind?(PythonOperationKind.LessThan);
        break;
      case ExpressionType.LessThanOrEqual:
        nullable = new PythonOperationKind?(PythonOperationKind.LessThanOrEqual);
        break;
      case ExpressionType.Modulo:
        nullable = new PythonOperationKind?(PythonOperationKind.Mod);
        break;
      case ExpressionType.Multiply:
        nullable = new PythonOperationKind?(PythonOperationKind.Multiply);
        break;
      case ExpressionType.NotEqual:
        nullable = new PythonOperationKind?(PythonOperationKind.NotEqual);
        break;
      case ExpressionType.Or:
        nullable = new PythonOperationKind?(PythonOperationKind.BitwiseOr);
        break;
      case ExpressionType.Power:
        nullable = new PythonOperationKind?(PythonOperationKind.Power);
        break;
      case ExpressionType.RightShift:
        nullable = new PythonOperationKind?(PythonOperationKind.RightShift);
        break;
      case ExpressionType.Subtract:
        nullable = new PythonOperationKind?(PythonOperationKind.Subtract);
        break;
      case ExpressionType.AddAssign:
        nullable = new PythonOperationKind?(PythonOperationKind.InPlaceAdd);
        break;
      case ExpressionType.AndAssign:
        nullable = new PythonOperationKind?(PythonOperationKind.InPlaceBitwiseAnd);
        break;
      case ExpressionType.DivideAssign:
        nullable = new PythonOperationKind?(PythonOperationKind.InPlaceDivide);
        break;
      case ExpressionType.ExclusiveOrAssign:
        nullable = new PythonOperationKind?(PythonOperationKind.InPlaceExclusiveOr);
        break;
      case ExpressionType.LeftShiftAssign:
        nullable = new PythonOperationKind?(PythonOperationKind.InPlaceLeftShift);
        break;
      case ExpressionType.ModuloAssign:
        nullable = new PythonOperationKind?(PythonOperationKind.InPlaceMod);
        break;
      case ExpressionType.MultiplyAssign:
        nullable = new PythonOperationKind?(PythonOperationKind.InPlaceMultiply);
        break;
      case ExpressionType.OrAssign:
        nullable = new PythonOperationKind?(PythonOperationKind.InPlaceBitwiseOr);
        break;
      case ExpressionType.PowerAssign:
        nullable = new PythonOperationKind?(PythonOperationKind.InPlacePower);
        break;
      case ExpressionType.RightShiftAssign:
        nullable = new PythonOperationKind?(PythonOperationKind.InPlaceRightShift);
        break;
      case ExpressionType.SubtractAssign:
        nullable = new PythonOperationKind?(PythonOperationKind.InPlaceSubtract);
        break;
    }
    DynamicMetaObject res = !nullable.HasValue ? operation.FallbackBinaryOperation(target, arg) : PythonProtocol.MakeBinaryOperation((DynamicMetaObjectBinder) operation, args, nullable.Value, errorSuggestion);
    return BindingHelpers.AddDynamicTestAndDefer((DynamicMetaObjectBinder) operation, BindingHelpers.AddPythonBoxing(res), args, validationInfo);
  }

  public static DynamicMetaObject Operation(
    UnaryOperationBinder operation,
    DynamicMetaObject arg,
    DynamicMetaObject errorSuggestion)
  {
    DynamicMetaObject[] args = new DynamicMetaObject[1]
    {
      arg
    };
    if (arg.NeedsDeferral())
      return operation.Defer(arg);
    ValidationInfo validationInfo = BindingHelpers.GetValidationInfo(args);
    Type deferType = typeof (object);
    DynamicMetaObject res;
    switch (operation.Operation)
    {
      case ExpressionType.Negate:
        res = BindingHelpers.AddPythonBoxing(PythonProtocol.MakeUnaryOperation((DynamicMetaObjectBinder) operation, arg, "__neg__", errorSuggestion));
        break;
      case ExpressionType.UnaryPlus:
        res = BindingHelpers.AddPythonBoxing(PythonProtocol.MakeUnaryOperation((DynamicMetaObjectBinder) operation, arg, "__pos__", errorSuggestion));
        break;
      case ExpressionType.Not:
        res = PythonProtocol.MakeUnaryNotOperation((DynamicMetaObjectBinder) operation, arg, typeof (object), errorSuggestion);
        break;
      case ExpressionType.OnesComplement:
        res = BindingHelpers.AddPythonBoxing(PythonProtocol.MakeUnaryOperation((DynamicMetaObjectBinder) operation, arg, "__invert__", errorSuggestion));
        break;
      case ExpressionType.IsTrue:
        res = PythonProtocol.ConvertToBool((DynamicMetaObjectBinder) operation, arg);
        deferType = typeof (bool);
        break;
      case ExpressionType.IsFalse:
        res = PythonProtocol.MakeUnaryNotOperation((DynamicMetaObjectBinder) operation, arg, typeof (bool), errorSuggestion);
        deferType = typeof (bool);
        break;
      default:
        res = PythonProtocol.TypeError((DynamicMetaObjectBinder) operation, "unknown operation: " + operation.ToString(), args);
        break;
    }
    return BindingHelpers.AddDynamicTestAndDefer((DynamicMetaObjectBinder) operation, res, args, validationInfo, deferType);
  }

  public static DynamicMetaObject Index(
    DynamicMetaObjectBinder operation,
    PythonIndexType index,
    DynamicMetaObject[] args)
  {
    return PythonProtocol.Index(operation, index, args, (DynamicMetaObject) null);
  }

  public static DynamicMetaObject Index(
    DynamicMetaObjectBinder operation,
    PythonIndexType index,
    DynamicMetaObject[] args,
    DynamicMetaObject errorSuggestion)
  {
    int length = args.Length;
    if (BindingHelpers.NeedsDeferral(args))
      return operation.Defer(args);
    ValidationInfo validationInfo = BindingHelpers.GetValidationInfo(args[0]);
    DynamicMetaObject res = BindingHelpers.AddPythonBoxing(PythonProtocol.MakeIndexerOperation(operation, index, args, errorSuggestion));
    return BindingHelpers.AddDynamicTestAndDefer(operation, res, args, validationInfo);
  }

  public static DynamicMetaObject Operation(
    PythonOperationBinder operation,
    params DynamicMetaObject[] args)
  {
    int length = args.Length;
    return BindingHelpers.NeedsDeferral(args) ? operation.Defer(args) : PythonProtocol.MakeOperationRule(operation, args);
  }

  private static DynamicMetaObject MakeOperationRule(
    PythonOperationBinder operation,
    DynamicMetaObject[] args)
  {
    ValidationInfo validationInfo = BindingHelpers.GetValidationInfo(args);
    Type deferType = typeof (object);
    DynamicMetaObject res;
    switch (PythonProtocol.NormalizeOperator(operation.Operation))
    {
      case PythonOperationKind.Documentation:
        res = BindingHelpers.AddPythonBoxing(PythonProtocol.MakeDocumentationOperation(operation, args));
        break;
      case PythonOperationKind.CallSignatures:
        res = BindingHelpers.AddPythonBoxing(PythonProtocol.MakeCallSignatureOperation(args[0], (IList<MethodBase>) CompilerHelpers.GetMethodTargets(args[0].Value)));
        break;
      case PythonOperationKind.IsCallable:
        res = PythonProtocol.MakeIscallableOperation(operation, args);
        break;
      case PythonOperationKind.Hash:
        res = PythonProtocol.MakeHashOperation(operation, args[0]);
        break;
      case PythonOperationKind.Contains:
        res = PythonProtocol.MakeContainsOperation(operation, args);
        break;
      case PythonOperationKind.Compare:
        res = PythonProtocol.MakeSortComparisonRule(args, (DynamicMetaObjectBinder) operation, operation.Operation);
        break;
      case PythonOperationKind.AbsoluteValue:
        res = BindingHelpers.AddPythonBoxing(PythonProtocol.MakeUnaryOperation((DynamicMetaObjectBinder) operation, args[0], "__abs__", (DynamicMetaObject) null));
        break;
      case PythonOperationKind.GetEnumeratorForIteration:
        res = PythonProtocol.MakeEnumeratorOperation(operation, args[0]);
        break;
      default:
        res = BindingHelpers.AddPythonBoxing(PythonProtocol.MakeBinaryOperation((DynamicMetaObjectBinder) operation, args, operation.Operation, (DynamicMetaObject) null));
        break;
    }
    return BindingHelpers.AddDynamicTestAndDefer((DynamicMetaObjectBinder) operation, res, args, validationInfo, deferType);
  }

  private static DynamicMetaObject MakeBinaryOperation(
    DynamicMetaObjectBinder operation,
    DynamicMetaObject[] args,
    PythonOperationKind opStr,
    DynamicMetaObject errorSuggestion)
  {
    return PythonProtocol.IsComparison(opStr) ? PythonProtocol.MakeComparisonOperation(args, operation, opStr, errorSuggestion) : PythonProtocol.MakeSimpleOperation(args, operation, opStr, errorSuggestion);
  }

  private static DynamicMetaObject MakeContainsOperation(
    PythonOperationBinder operation,
    DynamicMetaObject[] types)
  {
    ArrayUtils.SwapLastTwo<DynamicMetaObject>(types);
    PythonContext pythonContext = PythonContext.GetPythonContext((DynamicMetaObjectBinder) operation);
    SlotOrFunction slotOrFunction1 = SlotOrFunction.GetSlotOrFunction(pythonContext, "__contains__", types);
    DynamicMetaObject self;
    if (slotOrFunction1.Success)
    {
      self = slotOrFunction1.Target;
    }
    else
    {
      PythonProtocol.RestrictTypes(types);
      SlotOrFunction slotOrFunction2 = SlotOrFunction.GetSlotOrFunction(pythonContext, "__iter__", types[0]);
      if (slotOrFunction2.Success)
      {
        self = new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("ContainsFromEnumerable"), Microsoft.Scripting.Ast.Utils.Constant((object) pythonContext.SharedContext), slotOrFunction2.Target.Expression, Microsoft.Scripting.Ast.Utils.Convert(types[1].Expression, typeof (object))), BindingRestrictions.Combine((IList<DynamicMetaObject>) types));
      }
      else
      {
        ParameterExpression left1 = Expression.Variable(typeof (int), "count");
        SlotOrFunction slotOrFunction3 = SlotOrFunction.GetSlotOrFunction(pythonContext, "__getitem__", types[0], new DynamicMetaObject((Expression) left1, BindingRestrictions.Empty));
        if (slotOrFunction3.Success)
        {
          ParameterExpression left2 = Expression.Variable(slotOrFunction3.ReturnType, "getItemRes");
          ParameterExpression left3 = Expression.Variable(typeof (bool), "containsRes");
          LabelTarget labelTarget = Expression.Label();
          self = new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[3]
          {
            left1,
            left2,
            left3
          }, (Expression) Microsoft.Scripting.Ast.Utils.Loop((Expression) null, (Expression) Expression.Assign((Expression) left1, (Expression) Expression.Add((Expression) left1, Microsoft.Scripting.Ast.Utils.Constant((object) 1))), (Expression) Expression.Block((Expression) Microsoft.Scripting.Ast.Utils.Try((Expression) Expression.Block((Expression) Expression.Assign((Expression) left2, slotOrFunction3.Target.Expression), (Expression) Expression.Empty())).Catch(typeof (IndexOutOfRangeException), (Expression) Expression.Break(labelTarget)), (Expression) Microsoft.Scripting.Ast.Utils.If((Expression) Expression.Dynamic((CallSiteBinder) pythonContext.BinaryOperationRetType(pythonContext.BinaryOperation(ExpressionType.Equal), pythonContext.Convert(typeof (bool), ConversionResultKind.ExplicitCast)), typeof (bool), types[1].Expression, (Expression) left2), (Expression) Expression.Assign((Expression) left3, Microsoft.Scripting.Ast.Utils.Constant((object) true)), (Expression) Expression.Break(labelTarget)), (Expression) Microsoft.Scripting.Ast.Utils.Empty()), (Expression) null, labelTarget, (LabelTarget) null), (Expression) left3), BindingRestrictions.Combine((IList<DynamicMetaObject>) types));
        }
        else
          self = new DynamicMetaObject(operation.Throw((Expression) Expression.Call(typeof (PythonOps).GetMethod("TypeErrorForNonIterableObject"), Microsoft.Scripting.Ast.Utils.Convert(types[0].Expression, typeof (object))), typeof (bool)), BindingRestrictions.Combine((IList<DynamicMetaObject>) types));
      }
    }
    if (self.GetLimitType() != typeof (bool) && self.GetLimitType() != typeof (void))
      self = new DynamicMetaObject((Expression) Expression.Dynamic((CallSiteBinder) pythonContext.Convert(typeof (bool), ConversionResultKind.ExplicitCast), typeof (bool), self.Expression), self.Restrictions);
    return self;
  }

  private static void RestrictTypes(DynamicMetaObject[] types)
  {
    for (int index = 0; index < types.Length; ++index)
      types[index] = types[index].Restrict(types[index].GetLimitType());
  }

  private static DynamicMetaObject MakeHashOperation(
    PythonOperationBinder operation,
    DynamicMetaObject self)
  {
    self = self.Restrict(self.GetLimitType());
    PythonContext pythonContext = PythonContext.GetPythonContext((DynamicMetaObjectBinder) operation);
    SlotOrFunction slotOrFunction = SlotOrFunction.GetSlotOrFunction(pythonContext, "__hash__", self);
    DynamicMetaObject dynamicMetaObject = slotOrFunction.Target;
    if (slotOrFunction.IsNull)
      dynamicMetaObject = new DynamicMetaObject(operation.Throw((Expression) Expression.Call(typeof (PythonOps).GetMethod("TypeErrorForUnhashableObject"), self.Expression), typeof (int)), dynamicMetaObject.Restrictions);
    else if (slotOrFunction.ReturnType != typeof (int))
    {
      if (slotOrFunction.ReturnType == typeof (BigInteger))
        dynamicMetaObject = new DynamicMetaObject((Expression) PythonProtocol.HashBigInt(operation, dynamicMetaObject.Expression), dynamicMetaObject.Restrictions);
      else if (slotOrFunction.ReturnType == typeof (object))
      {
        ParameterExpression left = Expression.Parameter(typeof (object), "hashTemp");
        dynamicMetaObject = new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
        {
          left
        }, (Expression) Expression.Assign((Expression) left, dynamicMetaObject.Expression), (Expression) Expression.Condition((Expression) Expression.TypeIs((Expression) left, typeof (int)), (Expression) Expression.Convert((Expression) left, typeof (int)), (Expression) Expression.Condition((Expression) Expression.TypeIs((Expression) left, typeof (BigInteger)), (Expression) PythonProtocol.HashBigInt(operation, (Expression) left), (Expression) PythonProtocol.HashConvertToInt(pythonContext, (Expression) left)))), dynamicMetaObject.Restrictions);
      }
      else
        dynamicMetaObject = new DynamicMetaObject((Expression) PythonProtocol.HashConvertToInt(pythonContext, dynamicMetaObject.Expression), dynamicMetaObject.Restrictions);
    }
    return dynamicMetaObject;
  }

  private static DynamicExpression HashBigInt(
    PythonOperationBinder operation,
    Expression expression)
  {
    return Expression.Dynamic((CallSiteBinder) operation, typeof (int), expression);
  }

  private static DynamicExpression HashConvertToInt(PythonContext state, Expression expression)
  {
    return Expression.Dynamic((CallSiteBinder) state.Convert(typeof (int), ConversionResultKind.ExplicitCast), typeof (int), expression);
  }

  private static DynamicMetaObject MakeUnaryOperation(
    DynamicMetaObjectBinder binder,
    DynamicMetaObject self,
    string symbol,
    DynamicMetaObject errorSuggestion)
  {
    self = self.Restrict(self.GetLimitType());
    SlotOrFunction slotOrFunction = SlotOrFunction.GetSlotOrFunction(PythonContext.GetPythonContext(binder), symbol, self);
    if (slotOrFunction.Success)
      return slotOrFunction.Target;
    DynamicMetaObject dynamicMetaObject = errorSuggestion;
    if (dynamicMetaObject != null)
      return dynamicMetaObject;
    return PythonProtocol.TypeError(binder, PythonProtocol.MakeUnaryOpErrorMessage(symbol, "{0}"), self);
  }

  private static DynamicMetaObject MakeEnumeratorOperation(
    PythonOperationBinder operation,
    DynamicMetaObject self)
  {
    if (self.GetLimitType() == typeof (string))
    {
      self = self.Restrict(self.GetLimitType());
      return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("StringEnumerator"), self.Expression), self.Restrictions);
    }
    if (self.GetLimitType() == typeof (Bytes))
    {
      self = self.Restrict(self.GetLimitType());
      return operation.Context.PythonOptions.Python30 ? new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("BytesIntEnumerator"), self.Expression), self.Restrictions) : new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("BytesEnumerator"), self.Expression), self.Restrictions);
    }
    if ((self.Value is IEnumerable || typeof (IEnumerable).IsAssignableFrom(self.GetLimitType())) && !(self.Value is PythonGenerator))
    {
      self = self.Restrict(self.GetLimitType());
      return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("GetEnumeratorFromEnumerable"), (Expression) Expression.Convert(self.Expression, typeof (IEnumerable))), self.Restrictions);
    }
    if (self.Value is IEnumerator || typeof (IEnumerator).IsAssignableFrom(self.GetLimitType()))
    {
      DynamicMetaObject dynamicMetaObject = new DynamicMetaObject((Expression) PythonProtocol.MakeEnumeratorResult((Expression) Expression.Convert(self.Expression, typeof (IEnumerator))), self.Restrict(self.GetLimitType()).Restrictions);
      if (ComBinder.IsComObject(self.Value))
        dynamicMetaObject = new DynamicMetaObject((Expression) PythonProtocol.MakeEnumeratorResult((Expression) Expression.Convert(self.Expression, typeof (IEnumerator))), dynamicMetaObject.Restrictions.Merge(BindingRestrictions.GetExpressionRestriction((Expression) Expression.TypeIs(self.Expression, typeof (IEnumerator)))));
      return dynamicMetaObject;
    }
    ParameterExpression parameterExpression = Expression.Parameter(typeof (IEnumerator), "enum");
    IPythonConvertible pythonConvertible = self as IPythonConvertible;
    PythonConversionBinder binder = PythonContext.GetPythonContext((DynamicMetaObjectBinder) operation).Convert(typeof (IEnumerator), ConversionResultKind.ExplicitTry);
    DynamicMetaObject dynamicMetaObject1 = pythonConvertible == null ? binder.Bind(self, new DynamicMetaObject[0]) : pythonConvertible.BindConvert(binder);
    return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      parameterExpression
    }, (Expression) Expression.Condition((Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) parameterExpression, dynamicMetaObject1.Expression), Microsoft.Scripting.Ast.Utils.Constant((object) null)), (Expression) PythonProtocol.MakeEnumeratorResult((Expression) parameterExpression), (Expression) Expression.Call(typeof (PythonOps).GetMethod("ThrowTypeErrorForBadIteration"), PythonContext.GetCodeContext((DynamicMetaObjectBinder) operation), self.Expression))), dynamicMetaObject1.Restrictions);
  }

  private static NewExpression MakeEnumeratorResult(Expression tmp)
  {
    return Expression.New(typeof (KeyValuePair<IEnumerator, IDisposable>).GetConstructor(new Type[2]
    {
      typeof (IEnumerator),
      typeof (IDisposable)
    }), tmp, (Expression) Expression.Constant((object) null, typeof (IDisposable)));
  }

  private static DynamicMetaObject MakeUnaryNotOperation(
    DynamicMetaObjectBinder operation,
    DynamicMetaObject self,
    Type retType,
    DynamicMetaObject errorSuggestion)
  {
    self = self.Restrict(self.GetLimitType());
    SlotOrFunction slotOrFunction1 = SlotOrFunction.GetSlotOrFunction(PythonContext.GetPythonContext(operation), "__nonzero__", self);
    SlotOrFunction slotOrFunction2 = SlotOrFunction.GetSlotOrFunction(PythonContext.GetPythonContext(operation), "__len__", self);
    Expression expression1;
    if (!slotOrFunction1.Success && !slotOrFunction2.Success)
    {
      expression1 = errorSuggestion != null ? errorSuggestion.Expression : (self.GetLimitType() == typeof (DynamicNull) ? Microsoft.Scripting.Ast.Utils.Constant((object) true) : Microsoft.Scripting.Ast.Utils.Constant((object) false));
    }
    else
    {
      Expression expression2 = (slotOrFunction1.Success ? slotOrFunction1 : slotOrFunction2).Target.Expression;
      expression1 = !slotOrFunction1.Success ? (!(expression2.Type == typeof (int)) ? (Expression) Expression.Equal((Expression) Expression.Dynamic((CallSiteBinder) PythonContext.GetPythonContext(operation).Operation(PythonOperationKind.Compare), typeof (int), expression2, Microsoft.Scripting.Ast.Utils.Constant((object) 0)), Microsoft.Scripting.Ast.Utils.Constant((object) 0)) : (Expression) Expression.Equal(expression2, Microsoft.Scripting.Ast.Utils.Constant((object) 0))) : (!(expression2.Type == typeof (bool)) ? (Expression) Expression.Call(typeof (PythonOps).GetMethod("Not"), Microsoft.Scripting.Ast.Utils.Convert(expression2, typeof (object))) : (Expression) Expression.Equal(expression2, Microsoft.Scripting.Ast.Utils.Constant((object) false)));
    }
    if (retType == typeof (object) && expression1.Type == typeof (bool))
      expression1 = BindingHelpers.AddPythonBoxing(expression1);
    return new DynamicMetaObject(expression1, self.Restrictions.Merge(slotOrFunction1.Target.Restrictions.Merge(slotOrFunction2.Target.Restrictions)));
  }

  private static DynamicMetaObject MakeDocumentationOperation(
    PythonOperationBinder operation,
    DynamicMetaObject[] args)
  {
    PythonContext pythonContext = PythonContext.GetPythonContext((DynamicMetaObjectBinder) operation);
    return new DynamicMetaObject(Binders.Convert(PythonContext.GetCodeContext((DynamicMetaObjectBinder) operation), pythonContext, typeof (string), ConversionResultKind.ExplicitCast, Binders.Get(PythonContext.GetCodeContext((DynamicMetaObjectBinder) operation), pythonContext, typeof (object), "__doc__", args[0].Expression)), args[0].Restrictions);
  }

  internal static DynamicMetaObject MakeCallSignatureOperation(
    DynamicMetaObject self,
    IList<MethodBase> targets)
  {
    List<string> stringList = new List<string>();
    foreach (MethodBase target in (IEnumerable<MethodBase>) targets)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str = "";
      Type returnType = target.GetReturnType();
      if (returnType != typeof (void))
      {
        stringBuilder.Append(DynamicHelpers.GetPythonTypeFromType(returnType).Name);
        stringBuilder.Append(" ");
      }
      MethodInfo mi = target as MethodInfo;
      if (mi != (MethodInfo) null)
      {
        string name1;
        int name2 = (int) NameConverter.TryGetName(DynamicHelpers.GetPythonTypeFromType(target.DeclaringType), mi, out name1);
        stringBuilder.Append(name1);
      }
      else
        stringBuilder.Append(DynamicHelpers.GetPythonTypeFromType(target.DeclaringType).Name);
      stringBuilder.Append("(");
      if (!CompilerHelpers.IsStatic(target))
      {
        stringBuilder.Append(nameof (self));
        str = ", ";
      }
      foreach (ParameterInfo parameter in target.GetParameters())
      {
        if (!(parameter.ParameterType == typeof (CodeContext)))
        {
          stringBuilder.Append(str);
          stringBuilder.Append($"{DynamicHelpers.GetPythonTypeFromType(parameter.ParameterType).Name} {parameter.Name}");
          str = ", ";
        }
      }
      stringBuilder.Append(")");
      stringList.Add(stringBuilder.ToString());
    }
    return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) stringList.ToArray()), self.Restrictions.Merge(BindingRestrictions.GetInstanceRestriction(self.Expression, self.Value)));
  }

  private static DynamicMetaObject MakeIscallableOperation(
    PythonOperationBinder operation,
    DynamicMetaObject[] args)
  {
    DynamicMetaObject self = args[0];
    if (typeof (Delegate).IsAssignableFrom(self.GetLimitType()) || typeof (MethodGroup).IsAssignableFrom(self.GetLimitType()))
      return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) true), self.Restrict(self.GetLimitType()).Restrictions);
    PythonContext pythonContext = PythonContext.GetPythonContext((DynamicMetaObjectBinder) operation);
    return new DynamicMetaObject((Expression) Expression.NotEqual(Binders.TryGet(PythonContext.GetCodeContext((DynamicMetaObjectBinder) operation), pythonContext, typeof (object), "__call__", self.Expression), Microsoft.Scripting.Ast.Utils.Constant((object) OperationFailed.Value)), self.Restrict(self.GetLimitType()).Restrictions);
  }

  private static DynamicMetaObject MakeSimpleOperation(
    DynamicMetaObject[] types,
    DynamicMetaObjectBinder binder,
    PythonOperationKind operation,
    DynamicMetaObject errorSuggestion)
  {
    PythonProtocol.RestrictTypes(types);
    SlotOrFunction fbinder;
    SlotOrFunction rbinder;
    PythonTypeSlot fSlot;
    PythonTypeSlot rSlot;
    PythonProtocol.GetOperatorMethods(types, operation, PythonContext.GetPythonContext(binder), out fbinder, out rbinder, out fSlot, out rSlot);
    return PythonProtocol.MakeBinaryOperatorResult(types, binder, operation, fbinder, rbinder, fSlot, rSlot, errorSuggestion);
  }

  private static void GetOperatorMethods(
    DynamicMetaObject[] types,
    PythonOperationKind oper,
    PythonContext state,
    out SlotOrFunction fbinder,
    out SlotOrFunction rbinder,
    out PythonTypeSlot fSlot,
    out PythonTypeSlot rSlot)
  {
    oper = PythonProtocol.NormalizeOperator(oper);
    oper &= ~PythonOperationKind.InPlace;
    string str1;
    string str2;
    if (!PythonProtocol.IsReverseOperator(oper))
    {
      str1 = Symbols.OperatorToSymbol(oper);
      str2 = Symbols.OperatorToReversedSymbol(oper);
    }
    else
    {
      str2 = Symbols.OperatorToSymbol(oper);
      str1 = Symbols.OperatorToReversedSymbol(oper);
    }
    fSlot = (PythonTypeSlot) null;
    rSlot = (PythonTypeSlot) null;
    if (oper == PythonOperationKind.Multiply && PythonProtocol.IsSequence(types[0]) && !PythonOps.IsNonExtensibleNumericType(types[1].GetLimitType()))
      types = new DynamicMetaObject[2]
      {
        types[0],
        new DynamicMetaObject((Expression) Expression.New(typeof (IronPython.Runtime.Index).GetConstructor(new Type[1]
        {
          typeof (object)
        }), Microsoft.Scripting.Ast.Utils.Convert(types[1].Expression, typeof (object))), BindingRestrictions.Empty)
      };
    PythonType declaringType1;
    if (!SlotOrFunction.TryGetBinder(state, types, str1, (string) null, out fbinder, out declaringType1))
    {
      foreach (PythonType pythonType in (IEnumerable<PythonType>) MetaPythonObject.GetPythonType(types[0]).ResolutionOrder)
      {
        if (pythonType.TryLookupSlot(state.SharedContext, str1, out fSlot))
        {
          declaringType1 = pythonType;
          break;
        }
      }
    }
    PythonType declaringType2;
    if (!SlotOrFunction.TryGetBinder(state, types, (string) null, str2, out rbinder, out declaringType2))
    {
      foreach (PythonType pythonType in (IEnumerable<PythonType>) MetaPythonObject.GetPythonType(types[1]).ResolutionOrder)
      {
        if (pythonType.TryLookupSlot(state.SharedContext, str2, out rSlot))
        {
          declaringType2 = pythonType;
          break;
        }
      }
    }
    if (declaringType1 != null && (rbinder.Success || rSlot != null) && declaringType2 != declaringType1 && declaringType2.IsSubclassOf(declaringType1))
    {
      fbinder = SlotOrFunction.Empty;
      fSlot = (PythonTypeSlot) null;
    }
    if (fbinder.Success || rbinder.Success || fSlot != null || rSlot != null || !(str1 == "__truediv__") && !(str1 == "__rtruediv__"))
      return;
    PythonOperationKind oper1 = str1 == "__truediv__" ? PythonOperationKind.Divide : PythonOperationKind.ReverseDivide;
    PythonProtocol.GetOperatorMethods(types, oper1, state, out fbinder, out rbinder, out fSlot, out rSlot);
  }

  private static bool IsReverseOperator(PythonOperationKind oper)
  {
    return (oper & PythonOperationKind.Reversed) != 0;
  }

  private static bool IsSequence(DynamicMetaObject metaObject)
  {
    return typeof (List).IsAssignableFrom(metaObject.GetLimitType()) || typeof (PythonTuple).IsAssignableFrom(metaObject.GetLimitType()) || typeof (string).IsAssignableFrom(metaObject.GetLimitType());
  }

  private static DynamicMetaObject MakeBinaryOperatorResult(
    DynamicMetaObject[] types,
    DynamicMetaObjectBinder operation,
    PythonOperationKind op,
    SlotOrFunction fCand,
    SlotOrFunction rCand,
    PythonTypeSlot fSlot,
    PythonTypeSlot rSlot,
    DynamicMetaObject errorSuggestion)
  {
    PythonContext pythonContext = PythonContext.GetPythonContext(operation);
    ConditionalBuilder bodyBuilder = new ConditionalBuilder(operation);
    if ((op & PythonOperationKind.InPlace) != PythonOperationKind.None && !PythonProtocol.MakeOneCompareGeneric(SlotOrFunction.GetSlotOrFunction(PythonContext.GetPythonContext(operation), Symbols.OperatorToSymbol(op), types), false, types, new PythonProtocol.ComparisonHelper(PythonProtocol.MakeCompareReturn), bodyBuilder, typeof (object)))
      return bodyBuilder.GetMetaObject(types);
    SlotOrFunction fTarget;
    SlotOrFunction rTarget;
    if (!SlotOrFunction.GetCombinedTargets(fCand, rCand, out fTarget, out rTarget) && fSlot == null && rSlot == null && !PythonProtocol.ShouldCoerce(pythonContext, op, types[0], types[1], false) && !PythonProtocol.ShouldCoerce(pythonContext, op, types[1], types[0], false) && bodyBuilder.NoConditions)
      return PythonProtocol.MakeRuleForNoMatch(operation, op, errorSuggestion, types);
    if (PythonProtocol.ShouldCoerce(pythonContext, op, types[0], types[1], false) && (op != PythonOperationKind.Mod || !MetaPythonObject.GetPythonType(types[0]).IsSubclassOf(TypeCache.String)))
      PythonProtocol.DoCoerce(pythonContext, bodyBuilder, op, types, false);
    if (PythonProtocol.MakeOneTarget(PythonContext.GetPythonContext(operation), fTarget, fSlot, bodyBuilder, false, types))
    {
      if (PythonProtocol.ShouldCoerce(pythonContext, op, types[1], types[0], false))
        PythonProtocol.DoCoerce(pythonContext, bodyBuilder, op, new DynamicMetaObject[2]
        {
          types[1],
          types[0]
        }, true);
      if (rSlot != null)
      {
        PythonProtocol.MakeSlotCall(PythonContext.GetPythonContext(operation), types, bodyBuilder, rSlot, true);
        bodyBuilder.FinishCondition(PythonProtocol.MakeBinaryThrow(operation, op, types).Expression, typeof (object));
      }
      else if (PythonProtocol.MakeOneTarget(PythonContext.GetPythonContext(operation), rTarget, rSlot, bodyBuilder, false, types))
        bodyBuilder.FinishCondition(PythonProtocol.MakeBinaryThrow(operation, op, types).Expression, typeof (object));
    }
    return bodyBuilder.GetMetaObject(types);
  }

  private static void MakeCompareReturn(
    ConditionalBuilder bodyBuilder,
    Expression retCondition,
    Expression retValue,
    bool isReverse,
    Type retType)
  {
    if (retCondition != null)
      bodyBuilder.AddCondition(retCondition, retValue);
    else
      bodyBuilder.FinishCondition(retValue, retType);
  }

  private static bool MakeOneCompareGeneric(
    SlotOrFunction target,
    bool reverse,
    DynamicMetaObject[] types,
    PythonProtocol.ComparisonHelper returner,
    ConditionalBuilder bodyBuilder,
    Type retType)
  {
    if (target == SlotOrFunction.Empty || !target.Success)
      return true;
    ParameterExpression parameterExpression;
    if (target.ReturnType == typeof (bool))
    {
      parameterExpression = bodyBuilder.CompareRetBool;
    }
    else
    {
      parameterExpression = Expression.Variable(target.ReturnType, "compareRetValue");
      bodyBuilder.AddVariable(parameterExpression);
    }
    if (target.MaybeNotImplemented)
    {
      Expression expression = target.Target.Expression;
      Expression left = (Expression) Expression.Assign((Expression) parameterExpression, expression);
      returner(bodyBuilder, (Expression) Expression.NotEqual(left, Microsoft.Scripting.Ast.Utils.Constant((object) PythonOps.NotImplemented)), (Expression) parameterExpression, reverse, retType);
      return true;
    }
    returner(bodyBuilder, (Expression) null, target.Target.Expression, reverse, retType);
    return false;
  }

  private static bool MakeOneTarget(
    PythonContext state,
    SlotOrFunction target,
    PythonTypeSlot slotTarget,
    ConditionalBuilder bodyBuilder,
    bool reverse,
    DynamicMetaObject[] types)
  {
    if (target == SlotOrFunction.Empty && slotTarget == null)
      return true;
    if (slotTarget != null)
    {
      PythonProtocol.MakeSlotCall(state, types, bodyBuilder, slotTarget, reverse);
      return true;
    }
    if (target.MaybeNotImplemented)
    {
      ParameterExpression parameterExpression = Expression.Variable(typeof (object), "slot");
      bodyBuilder.AddVariable(parameterExpression);
      bodyBuilder.AddCondition((Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) parameterExpression, target.Target.Expression), (Expression) Expression.Property((Expression) null, typeof (PythonOps).GetProperty("NotImplemented"))), (Expression) parameterExpression);
      return true;
    }
    bodyBuilder.FinishCondition(target.Target.Expression, typeof (object));
    return false;
  }

  private static void MakeSlotCall(
    PythonContext state,
    DynamicMetaObject[] types,
    ConditionalBuilder bodyBuilder,
    PythonTypeSlot slotTarget,
    bool reverse)
  {
    Expression expression1;
    Expression expression2;
    if (reverse)
    {
      expression1 = types[1].Expression;
      expression2 = types[0].Expression;
    }
    else
    {
      expression1 = types[0].Expression;
      expression2 = types[1].Expression;
    }
    PythonProtocol.MakeSlotCallWorker(state, slotTarget, expression1, bodyBuilder, expression2);
  }

  private static void MakeSlotCallWorker(
    PythonContext state,
    PythonTypeSlot slotTarget,
    Expression self,
    ConditionalBuilder bodyBuilder,
    params Expression[] args)
  {
    ParameterExpression var = Expression.Variable(typeof (object), "slot");
    ParameterExpression parameterExpression = Expression.Variable(typeof (object), "slot");
    bodyBuilder.AddCondition((Expression) Expression.AndAlso((Expression) Expression.Call(typeof (PythonOps).GetMethod("SlotTryGetValue"), Microsoft.Scripting.Ast.Utils.Constant((object) state.SharedContext), Microsoft.Scripting.Ast.Utils.Convert((Expression) Microsoft.Scripting.Ast.Utils.WeakConstant((object) slotTarget), typeof (PythonTypeSlot)), Microsoft.Scripting.Ast.Utils.Convert(self, typeof (object)), (Expression) Expression.Call(typeof (DynamicHelpers).GetMethod("GetPythonType"), Microsoft.Scripting.Ast.Utils.Convert(self, typeof (object))), (Expression) var), (Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) parameterExpression, (Expression) Expression.Dynamic((CallSiteBinder) state.Invoke(new CallSignature(args.Length)), typeof (object), ArrayUtils.Insert<Expression>(Microsoft.Scripting.Ast.Utils.Constant((object) state.SharedContext), (Expression) var, args))), (Expression) Expression.Property((Expression) null, typeof (PythonOps).GetProperty("NotImplemented")))), (Expression) parameterExpression);
    bodyBuilder.AddVariable(var);
    bodyBuilder.AddVariable(parameterExpression);
  }

  private static void DoCoerce(
    PythonContext state,
    ConditionalBuilder bodyBuilder,
    PythonOperationKind op,
    DynamicMetaObject[] types,
    bool reverse)
  {
    PythonProtocol.DoCoerce(state, bodyBuilder, op, types, reverse, (Func<Expression, Expression>) (e => e));
  }

  private static void DoCoerce(
    PythonContext pyContext,
    ConditionalBuilder bodyBuilder,
    PythonOperationKind op,
    DynamicMetaObject[] types,
    bool reverse,
    Func<Expression, Expression> returnTransform)
  {
    ParameterExpression parameterExpression1 = Expression.Variable(typeof (object), "coerceResult");
    ParameterExpression parameterExpression2 = Expression.Variable(typeof (PythonTuple), "coerceTuple");
    SlotOrFunction slotOrFunction = SlotOrFunction.GetSlotOrFunction(pyContext, "__coerce__", types);
    if (!slotOrFunction.Success)
      return;
    bodyBuilder.AddCondition((Expression) Expression.AndAlso((Expression) Expression.Not((Expression) Expression.TypeIs((Expression) Expression.Assign((Expression) parameterExpression1, slotOrFunction.Target.Expression), typeof (OldInstance))), (Expression) Expression.NotEqual((Expression) Expression.Assign((Expression) parameterExpression2, (Expression) Expression.Call(typeof (PythonOps).GetMethod("ValidateCoerceResult"), (Expression) parameterExpression1)), Microsoft.Scripting.Ast.Utils.Constant((object) null))), BindingHelpers.AddRecursionCheck(pyContext, returnTransform((Expression) Expression.Dynamic((CallSiteBinder) pyContext.Operation(op | PythonOperationKind.DisableCoerce), op == PythonOperationKind.Compare ? typeof (int) : typeof (object), reverse ? (Expression) PythonProtocol.CoerceTwo(parameterExpression2) : (Expression) PythonProtocol.CoerceOne(parameterExpression2), reverse ? (Expression) PythonProtocol.CoerceOne(parameterExpression2) : (Expression) PythonProtocol.CoerceTwo(parameterExpression2)))));
    bodyBuilder.AddVariable(parameterExpression1);
    bodyBuilder.AddVariable(parameterExpression2);
  }

  private static MethodCallExpression CoerceTwo(ParameterExpression coerceTuple)
  {
    return Expression.Call(typeof (PythonOps).GetMethod("GetCoerceResultTwo"), (Expression) coerceTuple);
  }

  private static MethodCallExpression CoerceOne(ParameterExpression coerceTuple)
  {
    return Expression.Call(typeof (PythonOps).GetMethod("GetCoerceResultOne"), (Expression) coerceTuple);
  }

  private static DynamicMetaObject MakeComparisonOperation(
    DynamicMetaObject[] types,
    DynamicMetaObjectBinder operation,
    PythonOperationKind opString,
    DynamicMetaObject errorSuggestion)
  {
    PythonProtocol.RestrictTypes(types);
    PythonOperationKind op = PythonProtocol.NormalizeOperator(opString);
    PythonContext pythonContext = PythonContext.GetPythonContext(operation);
    DynamicMetaObject type1 = types[0];
    DynamicMetaObject type2 = types[1];
    string symbol = Symbols.OperatorToSymbol(op);
    string reversedSymbol = Symbols.OperatorToReversedSymbol(op);
    DynamicMetaObject[] types1 = new DynamicMetaObject[2]
    {
      types[1],
      types[0]
    };
    SlotOrFunction fTarget1 = SlotOrFunction.GetSlotOrFunction(pythonContext, symbol, types);
    SlotOrFunction rTarget1 = SlotOrFunction.GetSlotOrFunction(pythonContext, reversedSymbol, types1);
    SlotOrFunction fTarget2 = SlotOrFunction.GetSlotOrFunction(pythonContext, "__cmp__", types);
    SlotOrFunction rTarget2 = SlotOrFunction.GetSlotOrFunction(pythonContext, "__cmp__", types1);
    ConditionalBuilder bodyBuilder = new ConditionalBuilder(operation);
    SlotOrFunction.GetCombinedTargets(fTarget1, rTarget1, out fTarget1, out rTarget1);
    SlotOrFunction.GetCombinedTargets(fTarget2, rTarget2, out fTarget2, out rTarget2);
    PythonType pythonType1 = MetaPythonObject.GetPythonType(type1);
    PythonType pythonType2 = MetaPythonObject.GetPythonType(type2);
    bool reverse1 = false;
    if (rTarget1 != null && (pythonType1.IsSystemType || !pythonType1.IsOldClass) && pythonType2 != pythonType1 && BindingHelpers.IsSubclassOf(type2, type1))
    {
      SlotOrFunction slotOrFunction = rTarget1;
      rTarget1 = fTarget1;
      fTarget1 = slotOrFunction;
      reverse1 = true;
    }
    WarningInfo info;
    bool flag = fTarget1.ShouldWarn(pythonContext, out info);
    if (PythonProtocol.MakeOneCompareGeneric(fTarget1, reverse1, types, new PythonProtocol.ComparisonHelper(PythonProtocol.MakeCompareReturn), bodyBuilder, typeof (object)))
    {
      flag = flag || rTarget1.ShouldWarn(pythonContext, out info);
      if (PythonProtocol.MakeOneCompareGeneric(rTarget1, !reverse1, types, new PythonProtocol.ComparisonHelper(PythonProtocol.MakeCompareReturn), bodyBuilder, typeof (object)))
      {
        flag = flag || fTarget2.ShouldWarn(pythonContext, out info);
        if (PythonProtocol.ShouldCoerce(pythonContext, opString, type1, type2, true))
          PythonProtocol.DoCoerce(pythonContext, bodyBuilder, PythonOperationKind.Compare, types, false, (Func<Expression, Expression>) (e => PythonProtocol.GetCompareTest(op, e, false)));
        if (PythonProtocol.MakeOneCompareGeneric(fTarget2, false, types, (PythonProtocol.ComparisonHelper) ((builder, retCond, expr, reverse, retType) => PythonProtocol.MakeCompareTest(op, builder, retCond, expr, reverse, retType)), bodyBuilder, typeof (object)))
        {
          flag = flag || rTarget2.ShouldWarn(pythonContext, out info);
          if (PythonProtocol.ShouldCoerce(pythonContext, opString, type2, type1, true))
            PythonProtocol.DoCoerce(pythonContext, bodyBuilder, PythonOperationKind.Compare, types1, true, (Func<Expression, Expression>) (e => PythonProtocol.GetCompareTest(op, e, true)));
          if (PythonProtocol.MakeOneCompareGeneric(rTarget2, true, types, (PythonProtocol.ComparisonHelper) ((builder, retCond, expr, reverse, retType) => PythonProtocol.MakeCompareTest(op, builder, retCond, expr, reverse, retType)), bodyBuilder, typeof (object)))
          {
            if (errorSuggestion != null)
              bodyBuilder.FinishCondition(errorSuggestion.Expression, typeof (object));
            else
              bodyBuilder.FinishCondition(BindingHelpers.AddPythonBoxing(PythonProtocol.MakeFallbackCompare(operation, op, types)), typeof (object));
          }
        }
      }
    }
    DynamicMetaObject metaObject = bodyBuilder.GetMetaObject(types);
    return !flag || metaObject == null ? metaObject : info.AddWarning((Expression) Expression.Constant((object) pythonContext.SharedContext), metaObject);
  }

  private static DynamicMetaObject MakeSortComparisonRule(
    DynamicMetaObject[] types,
    DynamicMetaObjectBinder operation,
    PythonOperationKind op)
  {
    PythonProtocol.RestrictTypes(types);
    DynamicMetaObject dynamicMetaObject = PythonProtocol.FastPathCompare(types);
    if (dynamicMetaObject != null)
      return dynamicMetaObject;
    DynamicMetaObject[] types1 = new DynamicMetaObject[2]
    {
      types[1],
      types[0]
    };
    PythonContext pythonContext = PythonContext.GetPythonContext(operation);
    SlotOrFunction slotOrFunction1 = SlotOrFunction.GetSlotOrFunction(pythonContext, "__cmp__", types);
    SlotOrFunction slotOrFunction2 = SlotOrFunction.GetSlotOrFunction(pythonContext, "__cmp__", types1);
    SlotOrFunction slotOrFunction3 = SlotOrFunction.GetSlotOrFunction(pythonContext, "__eq__", types);
    SlotOrFunction slotOrFunction4 = SlotOrFunction.GetSlotOrFunction(pythonContext, "__eq__", types1);
    SlotOrFunction slotOrFunction5 = SlotOrFunction.GetSlotOrFunction(pythonContext, "__lt__", types);
    SlotOrFunction slotOrFunction6 = SlotOrFunction.GetSlotOrFunction(pythonContext, "__gt__", types);
    SlotOrFunction slotOrFunction7 = SlotOrFunction.GetSlotOrFunction(pythonContext, "__lt__", types1);
    SlotOrFunction slotOrFunction8 = SlotOrFunction.GetSlotOrFunction(pythonContext, "__gt__", types1);
    SlotOrFunction fTarget1;
    SlotOrFunction rTarget1;
    SlotOrFunction.GetCombinedTargets(slotOrFunction1, slotOrFunction2, out fTarget1, out rTarget1);
    SlotOrFunction fTarget2;
    SlotOrFunction rTarget2;
    SlotOrFunction.GetCombinedTargets(slotOrFunction3, slotOrFunction4, out fTarget2, out rTarget2);
    SlotOrFunction fTarget3;
    SlotOrFunction rTarget3;
    SlotOrFunction.GetCombinedTargets(slotOrFunction5, slotOrFunction8, out fTarget3, out rTarget3);
    SlotOrFunction fTarget4;
    SlotOrFunction rTarget4;
    SlotOrFunction.GetCombinedTargets(slotOrFunction6, slotOrFunction7, out fTarget4, out rTarget4);
    PythonType pythonType1 = MetaPythonObject.GetPythonType(types[0]);
    PythonType pythonType2 = MetaPythonObject.GetPythonType(types[1]);
    if (pythonType1.IsNull)
    {
      if (pythonType2.IsNull)
        return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) 0), BindingRestrictions.Combine((IList<DynamicMetaObject>) types));
      if (pythonType2.UnderlyingSystemType.IsPrimitive() || pythonType2.UnderlyingSystemType == typeof (BigInteger))
        return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) -1), BindingRestrictions.Combine((IList<DynamicMetaObject>) types));
    }
    ConditionalBuilder bodyBuilder = new ConditionalBuilder(operation);
    bool flag1 = true;
    bool flag2 = true;
    if (pythonType1 == pythonType2 && fTarget1 != SlotOrFunction.Empty)
    {
      if (PythonProtocol.ShouldCoerce(pythonContext, op, types[0], types[1], true))
        PythonProtocol.DoCoerce(pythonContext, bodyBuilder, PythonOperationKind.Compare, types, false);
      flag2 = flag2 && PythonProtocol.MakeOneCompareGeneric(fTarget1, false, types, new PythonProtocol.ComparisonHelper(PythonProtocol.MakeCompareReverse), bodyBuilder, typeof (int));
      if (pythonType1 != TypeCache.OldInstance)
      {
        flag2 = flag2 && PythonProtocol.MakeOneCompareGeneric(rTarget1, true, types, new PythonProtocol.ComparisonHelper(PythonProtocol.MakeCompareReverse), bodyBuilder, typeof (int));
        flag1 = false;
      }
    }
    if (flag1 & flag2)
    {
      PythonProtocol.MakeOneCompareGeneric(fTarget2, false, types, new PythonProtocol.ComparisonHelper(PythonProtocol.MakeCompareToZero), bodyBuilder, typeof (int));
      PythonProtocol.MakeOneCompareGeneric(rTarget2, true, types, new PythonProtocol.ComparisonHelper(PythonProtocol.MakeCompareToZero), bodyBuilder, typeof (int));
      PythonProtocol.MakeOneCompareGeneric(fTarget3, false, types, new PythonProtocol.ComparisonHelper(PythonProtocol.MakeCompareToNegativeOne), bodyBuilder, typeof (int));
      PythonProtocol.MakeOneCompareGeneric(rTarget3, true, types, new PythonProtocol.ComparisonHelper(PythonProtocol.MakeCompareToNegativeOne), bodyBuilder, typeof (int));
      PythonProtocol.MakeOneCompareGeneric(fTarget4, false, types, new PythonProtocol.ComparisonHelper(PythonProtocol.MakeCompareToOne), bodyBuilder, typeof (int));
      PythonProtocol.MakeOneCompareGeneric(rTarget4, true, types, new PythonProtocol.ComparisonHelper(PythonProtocol.MakeCompareToOne), bodyBuilder, typeof (int));
    }
    if (pythonType1 != pythonType2)
    {
      if (flag2 && PythonProtocol.ShouldCoerce(pythonContext, op, types[0], types[1], true))
        PythonProtocol.DoCoerce(pythonContext, bodyBuilder, PythonOperationKind.Compare, types, false);
      bool flag3 = flag2 && PythonProtocol.MakeOneCompareGeneric(fTarget1, false, types, new PythonProtocol.ComparisonHelper(PythonProtocol.MakeCompareReverse), bodyBuilder, typeof (int));
      if (flag3 && PythonProtocol.ShouldCoerce(pythonContext, op, types[1], types[0], true))
        PythonProtocol.DoCoerce(pythonContext, bodyBuilder, PythonOperationKind.Compare, types1, true, (Func<Expression, Expression>) (e => (Expression) PythonProtocol.ReverseCompareValue(e)));
      flag2 = flag3 && PythonProtocol.MakeOneCompareGeneric(rTarget1, true, types, new PythonProtocol.ComparisonHelper(PythonProtocol.MakeCompareReverse), bodyBuilder, typeof (int));
    }
    if (flag2)
      bodyBuilder.FinishCondition(PythonProtocol.MakeFallbackCompare(operation, op, types), typeof (int));
    return bodyBuilder.GetMetaObject(types);
  }

  private static DynamicMetaObject FastPathCompare(DynamicMetaObject[] types)
  {
    if (types[0].GetLimitType() == types[1].GetLimitType())
    {
      if (types[0].GetLimitType() == typeof (List))
      {
        types[0] = types[0].Restrict(typeof (List));
        types[1] = types[1].Restrict(typeof (List));
        return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("CompareLists"), types[0].Expression, types[1].Expression), BindingRestrictions.Combine((IList<DynamicMetaObject>) types));
      }
      if (types[0].GetLimitType() == typeof (PythonTuple))
      {
        types[0] = types[0].Restrict(typeof (PythonTuple));
        types[1] = types[1].Restrict(typeof (PythonTuple));
        return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("CompareTuples"), types[0].Expression, types[1].Expression), BindingRestrictions.Combine((IList<DynamicMetaObject>) types));
      }
      if (types[0].GetLimitType() == typeof (double))
      {
        types[0] = types[0].Restrict(typeof (double));
        types[1] = types[1].Restrict(typeof (double));
        return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("CompareFloats"), types[0].Expression, types[1].Expression), BindingRestrictions.Combine((IList<DynamicMetaObject>) types));
      }
    }
    return (DynamicMetaObject) null;
  }

  private static void MakeCompareToZero(
    ConditionalBuilder bodyBuilder,
    Expression retCondition,
    Expression expr,
    bool reverse,
    Type retType)
  {
    PythonProtocol.MakeValueCheck(0, expr, bodyBuilder, retCondition);
  }

  private static void MakeCompareToOne(
    ConditionalBuilder bodyBuilder,
    Expression retCondition,
    Expression expr,
    bool reverse,
    Type retType)
  {
    PythonProtocol.MakeValueCheck(1, expr, bodyBuilder, retCondition);
  }

  private static void MakeCompareToNegativeOne(
    ConditionalBuilder bodyBuilder,
    Expression retCondition,
    Expression expr,
    bool reverse,
    Type retType)
  {
    PythonProtocol.MakeValueCheck(-1, expr, bodyBuilder, retCondition);
  }

  private static void MakeValueCheck(
    int val,
    Expression retValue,
    ConditionalBuilder bodyBuilder,
    Expression retCondition)
  {
    if (retValue.Type != typeof (bool))
      retValue = (Expression) Expression.Dynamic((CallSiteBinder) PythonContext.GetPythonContext(bodyBuilder.Action).Convert(typeof (bool), ConversionResultKind.ExplicitCast), typeof (bool), retValue);
    if (retCondition != null)
      retValue = (Expression) Expression.AndAlso(retCondition, retValue);
    bodyBuilder.AddCondition(retValue, Microsoft.Scripting.Ast.Utils.Constant((object) val));
  }

  private static BinaryExpression ReverseCompareValue(Expression retVal)
  {
    return Expression.Multiply(Microsoft.Scripting.Ast.Utils.Convert(retVal, typeof (int)), Microsoft.Scripting.Ast.Utils.Constant((object) -1));
  }

  private static void MakeCompareReverse(
    ConditionalBuilder bodyBuilder,
    Expression retCondition,
    Expression expr,
    bool reverse,
    Type retType)
  {
    Expression retValue = expr;
    if (reverse)
      retValue = (Expression) PythonProtocol.ReverseCompareValue(expr);
    PythonProtocol.MakeCompareReturn(bodyBuilder, retCondition, retValue, reverse, retType);
  }

  private static void MakeCompareTest(
    PythonOperationKind op,
    ConditionalBuilder bodyBuilder,
    Expression retCond,
    Expression expr,
    bool reverse,
    Type retType)
  {
    PythonProtocol.MakeCompareReturn(bodyBuilder, retCond, PythonProtocol.GetCompareTest(op, expr, reverse), reverse, retType);
  }

  private static Expression MakeFallbackCompare(
    DynamicMetaObjectBinder binder,
    PythonOperationKind op,
    DynamicMetaObject[] types)
  {
    return (Expression) Expression.Call(PythonProtocol.GetComparisonFallbackMethod(op), PythonContext.GetCodeContext(binder), Microsoft.Scripting.Ast.Utils.Convert(types[0].Expression, typeof (object)), Microsoft.Scripting.Ast.Utils.Convert(types[1].Expression, typeof (object)));
  }

  private static Expression GetCompareTest(PythonOperationKind op, Expression expr, bool reverse)
  {
    return expr.Type == typeof (int) ? PythonProtocol.GetCompareNode(op, reverse, expr) : PythonProtocol.GetCompareExpression(op, reverse, (Expression) Expression.Call(typeof (PythonOps).GetMethod("CompareToZero"), Microsoft.Scripting.Ast.Utils.Convert(expr, typeof (object))));
  }

  private static DynamicMetaObject MakeIndexerOperation(
    DynamicMetaObjectBinder operation,
    PythonIndexType op,
    DynamicMetaObject[] types,
    DynamicMetaObject errorSuggestion)
  {
    DynamicMetaObject dynamicMetaObject = types[0].Restrict(types[0].GetLimitType());
    PythonContext pythonContext = PythonContext.GetPythonContext(operation);
    BuiltinFunction function = (BuiltinFunction) null;
    PythonTypeSlot slot = (PythonTypeSlot) null;
    bool flag = false;
    string str;
    string slice;
    int mandatoryArgs;
    PythonProtocol.GetIndexOperators(op, out str, out slice, out mandatoryArgs);
    if (types.Length == mandatoryArgs + 1 && PythonProtocol.IsSlice(op) && PythonProtocol.HasOnlyNumericTypes(operation, types, op == PythonIndexType.SetSlice))
    {
      flag = BindingHelpers.TryGetStaticFunction(pythonContext, slice, dynamicMetaObject, out function);
      if (function == null || !flag)
        flag = MetaPythonObject.GetPythonType(dynamicMetaObject).TryResolveSlot(pythonContext.SharedContext, slice, out slot);
    }
    if (!flag && !BindingHelpers.TryGetStaticFunction(pythonContext, str, dynamicMetaObject, out function))
      MetaPythonObject.GetPythonType(dynamicMetaObject).TryResolveSlot(pythonContext.SharedContext, str, out slot);
    PythonProtocol.Callable callable = PythonProtocol.Callable.MakeCallable(pythonContext, op, function, slot);
    if (callable == null)
      return errorSuggestion ?? PythonProtocol.MakeUnindexableError(operation, op, types, dynamicMetaObject, pythonContext);
    PythonProtocol.IndexBuilder indexBuilder;
    DynamicMetaObject[] args;
    if (flag)
    {
      indexBuilder = (PythonProtocol.IndexBuilder) new PythonProtocol.SliceBuilder(types, callable);
      args = PythonProtocol.ConvertArgs(types);
    }
    else
    {
      indexBuilder = (PythonProtocol.IndexBuilder) new PythonProtocol.ItemBuilder(types, callable);
      if (PythonProtocol.IsSlice(op))
      {
        args = PythonProtocol.GetItemSliceArguments(pythonContext, op, types);
      }
      else
      {
        args = (DynamicMetaObject[]) types.Clone();
        args[0] = types[0].Restrict(types[0].GetLimitType());
      }
    }
    return indexBuilder.MakeRule(operation, pythonContext, args);
  }

  private static DynamicMetaObject MakeUnindexableError(
    DynamicMetaObjectBinder operation,
    PythonIndexType op,
    DynamicMetaObject[] types,
    DynamicMetaObject indexedType,
    PythonContext state)
  {
    DynamicMetaObject[] dynamicMetaObjectArray = (DynamicMetaObject[]) types.Clone();
    dynamicMetaObjectArray[0] = indexedType;
    if (op == PythonIndexType.GetItem || op == PythonIndexType.GetSlice || !DynamicHelpers.GetPythonType(indexedType.Value).TryResolveSlot(state.SharedContext, "__getitem__", out PythonTypeSlot _))
      return PythonProtocol.TypeError(operation, "'{0}' object is not subscriptable", dynamicMetaObjectArray);
    return op == PythonIndexType.SetItem || op == PythonIndexType.SetSlice ? PythonProtocol.TypeError(operation, "'{0}' object does not support item assignment", dynamicMetaObjectArray) : PythonProtocol.TypeError(operation, "'{0}' object doesn't support item deletion", dynamicMetaObjectArray);
  }

  private static DynamicMetaObject[] ConvertArgs(DynamicMetaObject[] types)
  {
    DynamicMetaObject[] dynamicMetaObjectArray = new DynamicMetaObject[types.Length];
    for (int index = 0; index < types.Length; ++index)
      dynamicMetaObjectArray[index] = types[index].Restrict(types[index].GetLimitType());
    return dynamicMetaObjectArray;
  }

  private static DynamicMetaObject[] GetItemSliceArguments(
    PythonContext state,
    PythonIndexType op,
    DynamicMetaObject[] types)
  {
    DynamicMetaObject[] itemSliceArguments;
    if (op == PythonIndexType.SetSlice)
      itemSliceArguments = new DynamicMetaObject[3]
      {
        types[0].Restrict(types[0].GetLimitType()),
        PythonProtocol.GetSetSlice(state, types),
        types[types.Length - 1].Restrict(types[types.Length - 1].GetLimitType())
      };
    else
      itemSliceArguments = new DynamicMetaObject[2]
      {
        types[0].Restrict(types[0].GetLimitType()),
        PythonProtocol.GetGetOrDeleteSlice(state, types)
      };
    return itemSliceArguments;
  }

  private static bool HasOnlyNumericTypes(
    DynamicMetaObjectBinder action,
    DynamicMetaObject[] types,
    bool skipLast)
  {
    bool flag = true;
    PythonContext pythonContext = PythonContext.GetPythonContext(action);
    for (int index = 1; index < (skipLast ? types.Length - 1 : types.Length); ++index)
    {
      DynamicMetaObject type = types[index];
      if (!PythonProtocol.IsIndexType(pythonContext, type))
      {
        flag = false;
        break;
      }
    }
    return flag;
  }

  private static bool IsIndexType(PythonContext state, DynamicMetaObject obj)
  {
    bool flag = true;
    if (obj.GetLimitType() != typeof (MissingParameter) && !PythonOps.IsNumericType(obj.GetLimitType()) && !MetaPythonObject.GetPythonType(obj).TryResolveSlot(state.SharedContext, "__index__", out PythonTypeSlot _))
      flag = false;
    return flag;
  }

  private static bool IsSlice(PythonIndexType op) => op >= PythonIndexType.GetSlice;

  private static void GetIndexOperators(
    PythonIndexType op,
    out string item,
    out string slice,
    out int mandatoryArgs)
  {
    switch (op)
    {
      case PythonIndexType.GetItem:
      case PythonIndexType.GetSlice:
        item = "__getitem__";
        slice = "__getslice__";
        mandatoryArgs = 2;
        break;
      case PythonIndexType.SetItem:
      case PythonIndexType.SetSlice:
        item = "__setitem__";
        slice = "__setslice__";
        mandatoryArgs = 3;
        break;
      case PythonIndexType.DeleteItem:
      case PythonIndexType.DeleteSlice:
        item = "__delitem__";
        slice = "__delslice__";
        mandatoryArgs = 2;
        break;
      default:
        throw new InvalidOperationException();
    }
  }

  private static DynamicMetaObject GetSetSlice(PythonContext state, DynamicMetaObject[] args)
  {
    DynamicMetaObject[] dynamicMetaObjectArray = (DynamicMetaObject[]) args.Clone();
    for (int index = 1; index < dynamicMetaObjectArray.Length; ++index)
    {
      if (!PythonProtocol.IsIndexType(state, dynamicMetaObjectArray[index]))
        dynamicMetaObjectArray[index] = dynamicMetaObjectArray[index].Restrict(dynamicMetaObjectArray[index].GetLimitType());
    }
    return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeSlice"), Microsoft.Scripting.Ast.Utils.Convert(PythonProtocol.GetSetParameter(dynamicMetaObjectArray, 1), typeof (object)), Microsoft.Scripting.Ast.Utils.Convert(PythonProtocol.GetSetParameter(dynamicMetaObjectArray, 2), typeof (object)), Microsoft.Scripting.Ast.Utils.Convert(PythonProtocol.GetSetParameter(dynamicMetaObjectArray, 3), typeof (object))), BindingRestrictions.Combine((IList<DynamicMetaObject>) dynamicMetaObjectArray));
  }

  private static DynamicMetaObject GetGetOrDeleteSlice(
    PythonContext state,
    DynamicMetaObject[] args)
  {
    DynamicMetaObject[] dynamicMetaObjectArray = (DynamicMetaObject[]) args.Clone();
    for (int index = 1; index < dynamicMetaObjectArray.Length; ++index)
    {
      if (!PythonProtocol.IsIndexType(state, dynamicMetaObjectArray[index]))
        dynamicMetaObjectArray[index] = dynamicMetaObjectArray[index].Restrict(dynamicMetaObjectArray[index].GetLimitType());
    }
    return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeSlice"), Microsoft.Scripting.Ast.Utils.Convert(PythonProtocol.GetGetOrDeleteParameter(dynamicMetaObjectArray, 1), typeof (object)), Microsoft.Scripting.Ast.Utils.Convert(PythonProtocol.GetGetOrDeleteParameter(dynamicMetaObjectArray, 2), typeof (object)), Microsoft.Scripting.Ast.Utils.Convert(PythonProtocol.GetGetOrDeleteParameter(dynamicMetaObjectArray, 3), typeof (object))), BindingRestrictions.Combine((IList<DynamicMetaObject>) dynamicMetaObjectArray));
  }

  private static Expression GetGetOrDeleteParameter(DynamicMetaObject[] args, int index)
  {
    return args.Length > index ? PythonProtocol.CheckMissing(args[index].Expression) : Microsoft.Scripting.Ast.Utils.Constant((object) null);
  }

  private static Expression GetSetParameter(DynamicMetaObject[] args, int index)
  {
    return args.Length > index + 1 ? PythonProtocol.CheckMissing(args[index].Expression) : Microsoft.Scripting.Ast.Utils.Constant((object) null);
  }

  private static bool ShouldCoerce(
    PythonContext state,
    PythonOperationKind operation,
    DynamicMetaObject x,
    DynamicMetaObject y,
    bool isCompare)
  {
    if ((operation & PythonOperationKind.DisableCoerce) != PythonOperationKind.None)
      return false;
    PythonType pythonType1 = MetaPythonObject.GetPythonType(x);
    PythonType pythonType2 = MetaPythonObject.GetPythonType(y);
    if (pythonType1 == TypeCache.OldInstance)
      return true;
    PythonTypeSlot slot;
    if (!isCompare || pythonType1.IsSystemType || !pythonType2.IsSystemType || pythonType2 != TypeCache.Int32 && pythonType2 != TypeCache.BigInteger && pythonType2 != TypeCache.Double && pythonType2 != TypeCache.Complex || !pythonType1.TryResolveSlot(state.SharedContext, "__coerce__", out slot))
      return false;
    if (!(slot is BuiltinMethodDescriptor methodDescriptor) || methodDescriptor.__name__ != "__coerce__" && methodDescriptor.DeclaringType != typeof (int) && methodDescriptor.DeclaringType != typeof (BigInteger) && methodDescriptor.DeclaringType != typeof (double) && methodDescriptor.DeclaringType != typeof (Complex))
      return true;
    foreach (PythonType pythonType3 in (IEnumerable<PythonType>) pythonType1.ResolutionOrder)
    {
      if (pythonType3.UnderlyingSystemType == methodDescriptor.DeclaringType)
        return false;
    }
    return true;
  }

  public static PythonOperationKind DirectOperation(PythonOperationKind op)
  {
    if ((op & PythonOperationKind.InPlace) == PythonOperationKind.None)
      throw new InvalidOperationException();
    return op & ~PythonOperationKind.InPlace;
  }

  private static PythonOperationKind NormalizeOperator(PythonOperationKind op)
  {
    if ((op & PythonOperationKind.DisableCoerce) != PythonOperationKind.None)
      op &= ~PythonOperationKind.DisableCoerce;
    return op;
  }

  private static bool IsComparisonOperator(PythonOperationKind op)
  {
    return (op & PythonOperationKind.Comparison) != 0;
  }

  private static bool IsComparison(PythonOperationKind op)
  {
    return PythonProtocol.IsComparisonOperator(PythonProtocol.NormalizeOperator(op));
  }

  private static Expression GetCompareNode(PythonOperationKind op, bool reverse, Expression expr)
  {
    op = PythonProtocol.NormalizeOperator(op);
    switch (reverse ? (int) PythonProtocol.OperatorToReverseOperator(op) : (int) op)
    {
      case 134217760 /*0x08000020*/:
        return (Expression) Expression.LessThan(expr, Microsoft.Scripting.Ast.Utils.Constant((object) 0));
      case 134217761 /*0x08000021*/:
        return (Expression) Expression.GreaterThan(expr, Microsoft.Scripting.Ast.Utils.Constant((object) 0));
      case 134217762 /*0x08000022*/:
        return (Expression) Expression.LessThanOrEqual(expr, Microsoft.Scripting.Ast.Utils.Constant((object) 0));
      case 134217763 /*0x08000023*/:
        return (Expression) Expression.GreaterThanOrEqual(expr, Microsoft.Scripting.Ast.Utils.Constant((object) 0));
      case 134217764 /*0x08000024*/:
        return (Expression) Expression.Equal(expr, Microsoft.Scripting.Ast.Utils.Constant((object) 0));
      case 134217765 /*0x08000025*/:
        return (Expression) Expression.NotEqual(expr, Microsoft.Scripting.Ast.Utils.Constant((object) 0));
      default:
        throw new InvalidOperationException();
    }
  }

  public static PythonOperationKind OperatorToReverseOperator(PythonOperationKind op)
  {
    switch (op)
    {
      case PythonOperationKind.DivMod:
        return PythonOperationKind.ReverseDivMod;
      case PythonOperationKind.LessThan:
        return PythonOperationKind.GreaterThan;
      case PythonOperationKind.GreaterThan:
        return PythonOperationKind.LessThan;
      case PythonOperationKind.LessThanOrEqual:
        return PythonOperationKind.GreaterThanOrEqual;
      case PythonOperationKind.GreaterThanOrEqual:
        return PythonOperationKind.LessThanOrEqual;
      case PythonOperationKind.Equal:
        return PythonOperationKind.Equal;
      case PythonOperationKind.NotEqual:
        return PythonOperationKind.NotEqual;
      case PythonOperationKind.ReverseDivMod:
        return PythonOperationKind.DivMod;
      default:
        return op & ~PythonOperationKind.Reversed;
    }
  }

  private static Expression GetCompareExpression(
    PythonOperationKind op,
    bool reverse,
    Expression value)
  {
    op = PythonProtocol.NormalizeOperator(op);
    Expression right = Microsoft.Scripting.Ast.Utils.Constant((object) 0);
    Expression res;
    switch (reverse ? (int) PythonProtocol.OperatorToReverseOperator(op) : (int) op)
    {
      case 134217760 /*0x08000020*/:
        res = (Expression) Expression.LessThan(value, right);
        break;
      case 134217761 /*0x08000021*/:
        res = (Expression) Expression.GreaterThan(value, right);
        break;
      case 134217762 /*0x08000022*/:
        res = (Expression) Expression.LessThanOrEqual(value, right);
        break;
      case 134217763 /*0x08000023*/:
        res = (Expression) Expression.GreaterThanOrEqual(value, right);
        break;
      case 134217764 /*0x08000024*/:
        res = (Expression) Expression.Equal(value, right);
        break;
      case 134217765 /*0x08000025*/:
        res = (Expression) Expression.NotEqual(value, right);
        break;
      default:
        throw new InvalidOperationException();
    }
    return BindingHelpers.AddPythonBoxing(res);
  }

  private static MethodInfo GetComparisonFallbackMethod(PythonOperationKind op)
  {
    op = PythonProtocol.NormalizeOperator(op);
    string name;
    switch (op)
    {
      case PythonOperationKind.Compare:
        name = "CompareTypes";
        break;
      case PythonOperationKind.LessThan:
        name = "CompareTypesLessThan";
        break;
      case PythonOperationKind.GreaterThan:
        name = "CompareTypesGreaterThan";
        break;
      case PythonOperationKind.LessThanOrEqual:
        name = "CompareTypesLessThanOrEqual";
        break;
      case PythonOperationKind.GreaterThanOrEqual:
        name = "CompareTypesGreaterThanOrEqual";
        break;
      case PythonOperationKind.Equal:
        name = "CompareTypesEqual";
        break;
      case PythonOperationKind.NotEqual:
        name = "CompareTypesNotEqual";
        break;
      default:
        throw new InvalidOperationException();
    }
    return typeof (PythonOps).GetMethod(name);
  }

  internal static Expression CheckMissing(Expression toCheck)
  {
    if (toCheck.Type == typeof (MissingParameter))
      return Microsoft.Scripting.Ast.Utils.Constant((object) null);
    return toCheck.Type != typeof (object) ? toCheck : (Expression) Expression.Condition((Expression) Expression.TypeIs(toCheck, typeof (MissingParameter)), Microsoft.Scripting.Ast.Utils.Constant((object) null), toCheck);
  }

  private static DynamicMetaObject MakeRuleForNoMatch(
    DynamicMetaObjectBinder operation,
    PythonOperationKind op,
    DynamicMetaObject errorSuggestion,
    params DynamicMetaObject[] types)
  {
    return errorSuggestion ?? PythonProtocol.TypeError(operation, PythonProtocol.MakeBinaryOpErrorMessage(op, "{0}", "{1}"), types);
  }

  internal static string MakeUnaryOpErrorMessage(string op, string xType)
  {
    switch (op)
    {
      case "__invert__":
        return $"bad operand type for unary ~: '{xType}'";
      case "__abs__":
        return $"bad operand type for abs(): '{xType}'";
      case "__pos__":
        return $"bad operand type for unary +: '{xType}'";
      case "__neg__":
        return $"bad operand type for unary -: '{xType}'";
      default:
        throw new InvalidOperationException();
    }
  }

  internal static string MakeBinaryOpErrorMessage(
    PythonOperationKind op,
    string xType,
    string yType)
  {
    return string.Format("unsupported operand type(s) for {2}: '{0}' and '{1}'", (object) xType, (object) yType, (object) PythonProtocol.GetOperatorDisplay(op));
  }

  private static string GetOperatorDisplay(PythonOperationKind op)
  {
    op = PythonProtocol.NormalizeOperator(op);
    switch (op)
    {
      case PythonOperationKind.Add:
        return "+";
      case PythonOperationKind.Subtract:
        return "-";
      case PythonOperationKind.Power:
        return "**";
      case PythonOperationKind.Multiply:
        return "*";
      case PythonOperationKind.FloorDivide:
        return "//";
      case PythonOperationKind.Divide:
        return "/";
      case PythonOperationKind.TrueDivide:
        return "//";
      case PythonOperationKind.Mod:
        return "%";
      case PythonOperationKind.LeftShift:
        return "<<";
      case PythonOperationKind.RightShift:
        return ">>";
      case PythonOperationKind.BitwiseAnd:
        return "&";
      case PythonOperationKind.BitwiseOr:
        return "|";
      case PythonOperationKind.ExclusiveOr:
        return "^";
      case PythonOperationKind.LessThan:
        return "<";
      case PythonOperationKind.GreaterThan:
        return ">";
      case PythonOperationKind.LessThanOrEqual:
        return "<=";
      case PythonOperationKind.GreaterThanOrEqual:
        return ">=";
      case PythonOperationKind.Equal:
        return "==";
      case PythonOperationKind.NotEqual:
        return "!=";
      case PythonOperationKind.LessThanGreaterThan:
        return "<>";
      case PythonOperationKind.ReverseAdd:
        return "+";
      case PythonOperationKind.ReverseSubtract:
        return "-";
      case PythonOperationKind.ReversePower:
        return "**";
      case PythonOperationKind.ReverseMultiply:
        return "*";
      case PythonOperationKind.ReverseFloorDivide:
        return "/";
      case PythonOperationKind.ReverseDivide:
        return "/";
      case PythonOperationKind.ReverseTrueDivide:
        return "//";
      case PythonOperationKind.ReverseMod:
        return "%";
      case PythonOperationKind.ReverseLeftShift:
        return "<<";
      case PythonOperationKind.ReverseRightShift:
        return ">>";
      case PythonOperationKind.ReverseBitwiseAnd:
        return "&";
      case PythonOperationKind.ReverseBitwiseOr:
        return "|";
      case PythonOperationKind.ReverseExclusiveOr:
        return "^";
      case PythonOperationKind.InPlaceAdd:
        return "+=";
      case PythonOperationKind.InPlaceSubtract:
        return "-=";
      case PythonOperationKind.InPlacePower:
        return "**=";
      case PythonOperationKind.InPlaceMultiply:
        return "*=";
      case PythonOperationKind.InPlaceFloorDivide:
        return "/=";
      case PythonOperationKind.InPlaceDivide:
        return "/=";
      case PythonOperationKind.InPlaceTrueDivide:
        return "//=";
      case PythonOperationKind.InPlaceMod:
        return "%=";
      case PythonOperationKind.InPlaceLeftShift:
        return "<<=";
      case PythonOperationKind.InPlaceRightShift:
        return ">>=";
      case PythonOperationKind.InPlaceBitwiseAnd:
        return "&=";
      case PythonOperationKind.InPlaceBitwiseOr:
        return "|=";
      case PythonOperationKind.InPlaceExclusiveOr:
        return "^=";
      default:
        return op.ToString();
    }
  }

  private static DynamicMetaObject MakeBinaryThrow(
    DynamicMetaObjectBinder action,
    PythonOperationKind op,
    DynamicMetaObject[] args)
  {
    return action is IPythonSite ? new DynamicMetaObject(action.Throw((Expression) Expression.Call(typeof (PythonOps).GetMethod("TypeErrorForBinaryOp"), Microsoft.Scripting.Ast.Utils.Constant((object) Symbols.OperatorToSymbol(PythonProtocol.NormalizeOperator(op))), Microsoft.Scripting.Ast.Utils.Convert(args[0].Expression, typeof (object)), Microsoft.Scripting.Ast.Utils.Convert(args[1].Expression, typeof (object))), typeof (object)), BindingRestrictions.Combine((IList<DynamicMetaObject>) args)) : PythonProtocol.GenericFallback(action, args);
  }

  public static DynamicMetaObject TypeError(
    DynamicMetaObjectBinder action,
    string message,
    params DynamicMetaObject[] types)
  {
    if (!(action is IPythonSite))
      return PythonProtocol.GenericFallback(action, types);
    message = string.Format(message, (object[]) ArrayUtils.ConvertAll<DynamicMetaObject, string>(types, (Func<DynamicMetaObject, string>) (x => MetaPythonObject.GetPythonType(x).Name)));
    return new DynamicMetaObject(action.Throw((Expression) Expression.Call(typeof (PythonOps).GetMethod("SimpleTypeError"), (Expression) Expression.Constant((object) message)), typeof (object)), BindingRestrictions.Combine((IList<DynamicMetaObject>) types));
  }

  private static DynamicMetaObject GenericFallback(
    DynamicMetaObjectBinder action,
    DynamicMetaObject[] types)
  {
    switch (action)
    {
      case GetIndexBinder _:
        return ((GetIndexBinder) action).FallbackGetIndex(types[0], ArrayUtils.RemoveFirst<DynamicMetaObject>(types));
      case SetIndexBinder _:
        return ((SetIndexBinder) action).FallbackSetIndex(types[0], ArrayUtils.RemoveLast<DynamicMetaObject>(ArrayUtils.RemoveFirst<DynamicMetaObject>(types)), types[types.Length - 1]);
      case DeleteIndexBinder _:
        return ((DeleteIndexBinder) action).FallbackDeleteIndex(types[0], ArrayUtils.RemoveFirst<DynamicMetaObject>(types));
      case UnaryOperationBinder _:
        return ((UnaryOperationBinder) action).FallbackUnaryOperation(types[0]);
      case BinaryOperationBinder _:
        return ((BinaryOperationBinder) action).FallbackBinaryOperation(types[0], types[1]);
      default:
        throw new NotImplementedException();
    }
  }

  private delegate void ComparisonHelper(
    ConditionalBuilder bodyBuilder,
    Expression retCondition,
    Expression retValue,
    bool isReverse,
    Type retType);

  private abstract class Callable
  {
    private readonly PythonContext _binder;
    private readonly PythonIndexType _op;

    protected Callable(PythonContext binder, PythonIndexType op)
    {
      this._binder = binder;
      this._op = op;
    }

    public static PythonProtocol.Callable MakeCallable(
      PythonContext binder,
      PythonIndexType op,
      BuiltinFunction itemFunc,
      PythonTypeSlot itemSlot)
    {
      if (itemFunc != null)
        return (PythonProtocol.Callable) new PythonProtocol.BuiltinCallable(binder, op, itemFunc);
      return itemSlot != null ? (PythonProtocol.Callable) new PythonProtocol.SlotCallable(binder, op, itemSlot) : (PythonProtocol.Callable) null;
    }

    public virtual DynamicMetaObject[] GetTupleArguments(DynamicMetaObject[] arguments)
    {
      if (this.IsSetter)
      {
        if (arguments.Length == 3)
          return arguments;
        Expression[] expressionArray = new Expression[arguments.Length - 2];
        BindingRestrictions restrictions = BindingRestrictions.Empty;
        for (int index = 1; index < arguments.Length - 1; ++index)
        {
          expressionArray[index - 1] = Microsoft.Scripting.Ast.Utils.Convert(arguments[index].Expression, typeof (object));
          restrictions = restrictions.Merge(arguments[index].Restrictions);
        }
        return new DynamicMetaObject[3]
        {
          arguments[0],
          new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeTuple"), (Expression) Expression.NewArrayInit(typeof (object), expressionArray)), restrictions),
          arguments[arguments.Length - 1]
        };
      }
      if (arguments.Length == 2)
        return arguments;
      Expression[] expressionArray1 = new Expression[arguments.Length - 1];
      for (int index = 1; index < arguments.Length; ++index)
        expressionArray1[index - 1] = Microsoft.Scripting.Ast.Utils.Convert(arguments[index].Expression, typeof (object));
      return new DynamicMetaObject[2]
      {
        arguments[0],
        new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeTuple"), (Expression) Expression.NewArrayInit(typeof (object), expressionArray1)), BindingRestrictions.Combine((IList<DynamicMetaObject>) ArrayUtils.RemoveFirst<DynamicMetaObject>(arguments)))
      };
    }

    public abstract DynamicMetaObject CompleteRuleTarget(
      DynamicMetaObjectBinder metaBinder,
      DynamicMetaObject[] args,
      Func<DynamicMetaObject> customFailure);

    protected PythonBinder Binder => this._binder.Binder;

    protected PythonContext PythonContext => this._binder;

    protected bool IsSetter
    {
      get => this._op == PythonIndexType.SetItem || this._op == PythonIndexType.SetSlice;
    }
  }

  private class BuiltinCallable : PythonProtocol.Callable
  {
    private readonly BuiltinFunction _bf;

    public BuiltinCallable(PythonContext binder, PythonIndexType op, BuiltinFunction func)
      : base(binder, op)
    {
      this._bf = func;
    }

    public override DynamicMetaObject[] GetTupleArguments(DynamicMetaObject[] arguments)
    {
      return arguments[0].GetLimitType() == typeof (OldInstance) ? base.GetTupleArguments(arguments) : arguments;
    }

    public override DynamicMetaObject CompleteRuleTarget(
      DynamicMetaObjectBinder metaBinder,
      DynamicMetaObject[] args,
      Func<DynamicMetaObject> customFailure)
    {
      PythonOverloadResolver resolver = new PythonOverloadResolver(this.Binder, args[0], (IList<DynamicMetaObject>) ArrayUtils.RemoveFirst<DynamicMetaObject>(args), new CallSignature(args.Length - 1), Microsoft.Scripting.Ast.Utils.Constant((object) this.PythonContext.SharedContext));
      BindingTarget target;
      DynamicMetaObject res = this.Binder.CallMethod((DefaultOverloadResolver) resolver, this._bf.Targets, BindingRestrictions.Combine((IList<DynamicMetaObject>) args), this._bf.Name, NarrowingLevel.None, NarrowingLevel.Three, out target);
      DynamicMetaObject result = BindingHelpers.CheckLightThrowMO(metaBinder, res, target);
      if (target.Success)
      {
        if (this.IsSetter)
          result = new DynamicMetaObject((Expression) Expression.Block(result.Expression, args[args.Length - 1].Expression), result.Restrictions);
        WarningInfo info;
        if (BindingWarnings.ShouldWarn(this.Binder.Context, target.Overload, out info))
          result = info.AddWarning((Expression) Expression.Constant((object) this.PythonContext.SharedContext), result);
      }
      else if (customFailure == null || (result = customFailure()) == null)
        result = Microsoft.Scripting.Actions.DefaultBinder.MakeError(resolver.MakeInvalidParametersError(target), BindingRestrictions.Combine((IList<DynamicMetaObject>) PythonProtocol.ConvertArgs(args)), typeof (object));
      return result;
    }
  }

  private class SlotCallable : PythonProtocol.Callable
  {
    private PythonTypeSlot _slot;

    public SlotCallable(PythonContext binder, PythonIndexType op, PythonTypeSlot slot)
      : base(binder, op)
    {
      this._slot = slot;
    }

    public override DynamicMetaObject CompleteRuleTarget(
      DynamicMetaObjectBinder metaBinder,
      DynamicMetaObject[] args,
      Func<DynamicMetaObject> customFailure)
    {
      ConditionalBuilder builder = new ConditionalBuilder();
      this._slot.MakeGetExpression(this.Binder, Microsoft.Scripting.Ast.Utils.Constant((object) this.PythonContext.SharedContext), args[0], new DynamicMetaObject((Expression) Expression.Call(typeof (DynamicHelpers).GetMethod("GetPythonType"), Microsoft.Scripting.Ast.Utils.Convert(args[0].Expression, typeof (object))), BindingRestrictions.Empty, (object) DynamicHelpers.GetPythonType(args[0].Value)), builder);
      if (!builder.IsFinal)
        builder.FinishCondition(metaBinder.Throw((Expression) Expression.New(typeof (InvalidOperationException))));
      Expression expression1 = builder.GetMetaObject().Expression;
      Expression[] array = new Expression[args.Length - 1];
      for (int index = 1; index < args.Length; ++index)
        array[index - 1] = args[index].Expression;
      Expression expression2 = (Expression) Expression.Dynamic((CallSiteBinder) this.PythonContext.Invoke(new CallSignature(array.Length)), typeof (object), ArrayUtils.Insert<Expression>(Microsoft.Scripting.Ast.Utils.Constant((object) this.PythonContext.SharedContext), expression1, array));
      if (this.IsSetter)
        expression2 = (Expression) Expression.Block(expression2, args[args.Length - 1].Expression);
      return new DynamicMetaObject(expression2, BindingRestrictions.Combine((IList<DynamicMetaObject>) args));
    }
  }

  private abstract class IndexBuilder
  {
    private readonly PythonProtocol.Callable _callable;
    private readonly DynamicMetaObject[] _types;

    public IndexBuilder(DynamicMetaObject[] types, PythonProtocol.Callable callable)
    {
      this._callable = callable;
      this._types = types;
    }

    public abstract DynamicMetaObject MakeRule(
      DynamicMetaObjectBinder metaBinder,
      PythonContext binder,
      DynamicMetaObject[] args);

    protected PythonProtocol.Callable Callable => this._callable;

    protected PythonType GetTypeAt(int index) => MetaPythonObject.GetPythonType(this._types[index]);
  }

  private class SliceBuilder(DynamicMetaObject[] types, PythonProtocol.Callable callable) : 
    PythonProtocol.IndexBuilder(types, callable)
  {
    private ParameterExpression _lengthVar;

    public override DynamicMetaObject MakeRule(
      DynamicMetaObjectBinder metaBinder,
      PythonContext binder,
      DynamicMetaObject[] args)
    {
      args = ArrayUtils.Copy<DynamicMetaObject>(args);
      for (int index = 1; index < 3; ++index)
      {
        args[index] = args[index].Restrict(args[index].GetLimitType());
        if (args[index].GetLimitType() == typeof (MissingParameter))
        {
          switch (index)
          {
            case 1:
              args[index] = new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) 0), args[index].Restrictions);
              continue;
            case 2:
              args[index] = new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) int.MaxValue), args[index].Restrictions);
              continue;
            default:
              continue;
          }
        }
        else
          args[index] = !(args[index].GetLimitType() == typeof (int)) ? (!args[index].GetLimitType().IsSubclassOf(typeof (Extensible<int>)) ? (!(args[index].GetLimitType() == typeof (BigInteger)) ? (!args[index].GetLimitType().IsSubclassOf(typeof (Extensible<BigInteger>)) ? (!(args[index].GetLimitType() == typeof (bool)) ? this.MakeIntTest(args[0], new DynamicMetaObject((Expression) Expression.Dynamic((CallSiteBinder) binder.Convert(typeof (int), ConversionResultKind.ExplicitCast), typeof (int), (Expression) Expression.Dynamic((CallSiteBinder) binder.InvokeNone, typeof (object), Microsoft.Scripting.Ast.Utils.Constant((object) binder.SharedContext), Binders.Get(Microsoft.Scripting.Ast.Utils.Constant((object) binder.SharedContext), binder, typeof (object), "__index__", (Expression) Expression.Convert(args[index].Expression, typeof (object))))), args[index].Restrictions)) : new DynamicMetaObject((Expression) Expression.Condition(args[index].Expression, Microsoft.Scripting.Ast.Utils.Constant((object) 1), Microsoft.Scripting.Ast.Utils.Constant((object) 0)), args[index].Restrictions)) : this.MakeBigIntTest(args[0], new DynamicMetaObject((Expression) Expression.Property(args[index].Expression, args[index].GetLimitType().GetProperty("Value")), args[index].Restrictions))) : this.MakeBigIntTest(args[0], args[index])) : this.MakeIntTest(args[0], new DynamicMetaObject((Expression) Expression.Property(args[index].Expression, args[index].GetLimitType().GetProperty("Value")), args[index].Restrictions))) : this.MakeIntTest(args[0], args[index]);
      }
      if (this._lengthVar == null)
        return this.Callable.CompleteRuleTarget(metaBinder, args, (Func<DynamicMetaObject>) null);
      DynamicMetaObject dynamicMetaObject = this.Callable.CompleteRuleTarget(metaBinder, args, (Func<DynamicMetaObject>) null);
      return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        this._lengthVar
      }, (Expression) Expression.Assign((Expression) this._lengthVar, (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, this._lengthVar.Type)), dynamicMetaObject.Expression), dynamicMetaObject.Restrictions);
    }

    private DynamicMetaObject MakeBigIntTest(DynamicMetaObject self, DynamicMetaObject bigInt)
    {
      this.EnsureLengthVariable();
      return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("NormalizeBigInteger"), self.Expression, bigInt.Expression, (Expression) this._lengthVar), self.Restrictions.Merge(bigInt.Restrictions));
    }

    private DynamicMetaObject MakeIntTest(DynamicMetaObject self, DynamicMetaObject intVal)
    {
      return new DynamicMetaObject((Expression) Expression.Condition((Expression) Expression.LessThan(intVal.Expression, Microsoft.Scripting.Ast.Utils.Constant((object) 0)), (Expression) Expression.Add(intVal.Expression, this.MakeGetLength(self)), intVal.Expression), self.Restrictions.Merge(intVal.Restrictions));
    }

    private Expression MakeGetLength(DynamicMetaObject self)
    {
      this.EnsureLengthVariable();
      return (Expression) Expression.Call(typeof (PythonOps).GetMethod("GetLengthOnce"), self.Expression, (Expression) this._lengthVar);
    }

    private void EnsureLengthVariable()
    {
      if (this._lengthVar != null)
        return;
      this._lengthVar = Expression.Variable(typeof (int?), "objLength");
    }
  }

  private class ItemBuilder(DynamicMetaObject[] types, PythonProtocol.Callable callable) : 
    PythonProtocol.IndexBuilder(types, callable)
  {
    public override DynamicMetaObject MakeRule(
      DynamicMetaObjectBinder metaBinder,
      PythonContext binder,
      DynamicMetaObject[] args)
    {
      DynamicMetaObject[] tupleArgs = this.Callable.GetTupleArguments(args);
      return this.Callable.CompleteRuleTarget(metaBinder, tupleArgs, (Func<DynamicMetaObject>) (() =>
      {
        if (!(args[1].GetLimitType() != typeof (Slice)) || !this.GetTypeAt(1).TryResolveSlot(binder.SharedContext, "__index__", out PythonTypeSlot _))
          return (DynamicMetaObject) null;
        args[1] = new DynamicMetaObject((Expression) Expression.Dynamic((CallSiteBinder) binder.Convert(typeof (int), ConversionResultKind.ExplicitCast), typeof (int), (Expression) Expression.Dynamic((CallSiteBinder) binder.InvokeNone, typeof (object), Microsoft.Scripting.Ast.Utils.Constant((object) binder.SharedContext), Binders.Get(Microsoft.Scripting.Ast.Utils.Constant((object) binder.SharedContext), binder, typeof (object), "__index__", args[1].Expression))), BindingRestrictions.Empty);
        return this.Callable.CompleteRuleTarget(metaBinder, tupleArgs, (Func<DynamicMetaObject>) null);
      }));
    }
  }
}
