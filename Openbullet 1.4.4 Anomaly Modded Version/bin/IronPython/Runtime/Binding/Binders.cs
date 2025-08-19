// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.Binders
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Actions;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal static class Binders
{
  public static Expression Convert(
    Expression codeContext,
    PythonContext binder,
    Type type,
    ConversionResultKind resultKind,
    Expression target)
  {
    return (Expression) Expression.Dynamic((CallSiteBinder) binder.Convert(type, resultKind), type, target);
  }

  public static Expression Get(
    Expression codeContext,
    PythonContext binder,
    Type resultType,
    string name,
    Expression target)
  {
    return (Expression) Expression.Dynamic((CallSiteBinder) binder.GetMember(name), resultType, target, codeContext);
  }

  public static Expression TryGet(
    Expression codeContext,
    PythonContext binder,
    Type resultType,
    string name,
    Expression target)
  {
    return (Expression) Expression.Dynamic((CallSiteBinder) binder.GetMember(name, true), resultType, target, codeContext);
  }

  public static DynamicMetaObjectBinder UnaryOperationBinder(
    PythonContext state,
    PythonOperationKind operatorName)
  {
    ExpressionType? fromUnaryOperator = Binders.GetExpressionTypeFromUnaryOperator(operatorName);
    return !fromUnaryOperator.HasValue ? (DynamicMetaObjectBinder) state.Operation(operatorName) : (DynamicMetaObjectBinder) state.UnaryOperation(fromUnaryOperator.Value);
  }

  private static ExpressionType? GetExpressionTypeFromUnaryOperator(PythonOperationKind operatorName)
  {
    switch (operatorName)
    {
      case PythonOperationKind.Positive:
        return new ExpressionType?(ExpressionType.UnaryPlus);
      case PythonOperationKind.Negate:
        return new ExpressionType?(ExpressionType.Negate);
      case PythonOperationKind.OnesComplement:
        return new ExpressionType?(ExpressionType.OnesComplement);
      case PythonOperationKind.IsFalse:
        return new ExpressionType?(ExpressionType.IsFalse);
      case PythonOperationKind.Not:
        return new ExpressionType?(ExpressionType.Not);
      default:
        return new ExpressionType?();
    }
  }

  public static DynamicMetaObjectBinder BinaryOperationBinder(
    PythonContext state,
    PythonOperationKind operatorName)
  {
    ExpressionType? fromBinaryOperator = Binders.GetExpressionTypeFromBinaryOperator(operatorName);
    return !fromBinaryOperator.HasValue ? (DynamicMetaObjectBinder) state.Operation(operatorName) : (DynamicMetaObjectBinder) state.BinaryOperation(fromBinaryOperator.Value);
  }

  private static ExpressionType? GetExpressionTypeFromBinaryOperator(
    PythonOperationKind operatorName)
  {
    switch (operatorName)
    {
      case PythonOperationKind.Add:
        return new ExpressionType?(ExpressionType.Add);
      case PythonOperationKind.Subtract:
        return new ExpressionType?(ExpressionType.Subtract);
      case PythonOperationKind.Power:
        return new ExpressionType?(ExpressionType.Power);
      case PythonOperationKind.Multiply:
        return new ExpressionType?(ExpressionType.Multiply);
      case PythonOperationKind.Divide:
        return new ExpressionType?(ExpressionType.Divide);
      case PythonOperationKind.Mod:
        return new ExpressionType?(ExpressionType.Modulo);
      case PythonOperationKind.LeftShift:
        return new ExpressionType?(ExpressionType.LeftShift);
      case PythonOperationKind.RightShift:
        return new ExpressionType?(ExpressionType.RightShift);
      case PythonOperationKind.BitwiseAnd:
        return new ExpressionType?(ExpressionType.And);
      case PythonOperationKind.BitwiseOr:
        return new ExpressionType?(ExpressionType.Or);
      case PythonOperationKind.ExclusiveOr:
        return new ExpressionType?(ExpressionType.ExclusiveOr);
      case PythonOperationKind.LessThan:
        return new ExpressionType?(ExpressionType.LessThan);
      case PythonOperationKind.GreaterThan:
        return new ExpressionType?(ExpressionType.GreaterThan);
      case PythonOperationKind.LessThanOrEqual:
        return new ExpressionType?(ExpressionType.LessThanOrEqual);
      case PythonOperationKind.GreaterThanOrEqual:
        return new ExpressionType?(ExpressionType.GreaterThanOrEqual);
      case PythonOperationKind.Equal:
        return new ExpressionType?(ExpressionType.Equal);
      case PythonOperationKind.NotEqual:
        return new ExpressionType?(ExpressionType.NotEqual);
      case PythonOperationKind.InPlaceAdd:
        return new ExpressionType?(ExpressionType.AddAssign);
      case PythonOperationKind.InPlaceSubtract:
        return new ExpressionType?(ExpressionType.SubtractAssign);
      case PythonOperationKind.InPlacePower:
        return new ExpressionType?(ExpressionType.PowerAssign);
      case PythonOperationKind.InPlaceMultiply:
        return new ExpressionType?(ExpressionType.MultiplyAssign);
      case PythonOperationKind.InPlaceDivide:
        return new ExpressionType?(ExpressionType.DivideAssign);
      case PythonOperationKind.InPlaceMod:
        return new ExpressionType?(ExpressionType.ModuloAssign);
      case PythonOperationKind.InPlaceLeftShift:
        return new ExpressionType?(ExpressionType.LeftShiftAssign);
      case PythonOperationKind.InPlaceRightShift:
        return new ExpressionType?(ExpressionType.RightShiftAssign);
      case PythonOperationKind.InPlaceBitwiseAnd:
        return new ExpressionType?(ExpressionType.AndAssign);
      case PythonOperationKind.InPlaceBitwiseOr:
        return new ExpressionType?(ExpressionType.OrAssign);
      case PythonOperationKind.InPlaceExclusiveOr:
        return new ExpressionType?(ExpressionType.ExclusiveOrAssign);
      default:
        return new ExpressionType?();
    }
  }

  public static PythonInvokeBinder InvokeSplat(PythonContext state)
  {
    return state.Invoke(new CallSignature(new Argument[1]
    {
      new Argument(ArgumentType.List)
    }));
  }

  public static PythonInvokeBinder InvokeKeywords(PythonContext state)
  {
    return state.Invoke(new CallSignature(new Argument[2]
    {
      new Argument(ArgumentType.List),
      new Argument(ArgumentType.Dictionary)
    }));
  }
}
