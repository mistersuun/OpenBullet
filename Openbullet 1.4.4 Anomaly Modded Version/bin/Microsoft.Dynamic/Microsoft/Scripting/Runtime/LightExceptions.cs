// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.LightExceptions
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public static class LightExceptions
{
  internal static MethodInfo _checkAndThrow = RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Func<object, object>(LightExceptions.CheckAndThrow));

  public static Expression Rewrite(Expression expression)
  {
    ContractUtils.RequiresNotNull((object) expression, nameof (expression));
    return new LightExceptionRewriter().Rewrite(expression);
  }

  public static Expression RewriteLazy(Expression expression)
  {
    ContractUtils.RequiresNotNull((object) expression, nameof (expression));
    return (Expression) new LightExceptionConvertingExpression(expression, false);
  }

  public static Expression RewriteExternal(Expression expression)
  {
    ContractUtils.RequiresNotNull((object) expression, nameof (expression));
    return LightExceptions.CheckAndThrow((Expression) new LightExceptionConvertingExpression(expression, true));
  }

  public static object Throw(Exception exceptionValue)
  {
    ContractUtils.RequiresNotNull((object) exceptionValue, nameof (exceptionValue));
    return (object) new LightExceptions.LightException(exceptionValue);
  }

  public static Expression Throw(Expression exceptionValue)
  {
    ContractUtils.RequiresNotNull((object) exceptionValue, nameof (exceptionValue));
    return (Expression) new LightThrowExpression(exceptionValue);
  }

  public static Expression Throw(Expression exceptionValue, Type retType)
  {
    ContractUtils.RequiresNotNull((object) exceptionValue, nameof (exceptionValue));
    ContractUtils.RequiresNotNull((object) retType, nameof (retType));
    return (Expression) Expression.Convert((Expression) new LightThrowExpression(exceptionValue), retType);
  }

  public static Expression Throw(this DynamicMetaObjectBinder binder, Expression exceptionValue)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    ContractUtils.RequiresNotNull((object) exceptionValue, nameof (exceptionValue));
    return binder.SupportsLightThrow() ? LightExceptions.Throw(exceptionValue) : (Expression) Expression.Throw(exceptionValue);
  }

  public static Expression Throw(
    this DynamicMetaObjectBinder binder,
    Expression exceptionValue,
    Type retType)
  {
    ContractUtils.RequiresNotNull((object) binder, nameof (binder));
    ContractUtils.RequiresNotNull((object) exceptionValue, nameof (exceptionValue));
    ContractUtils.RequiresNotNull((object) retType, nameof (retType));
    return binder.SupportsLightThrow() ? LightExceptions.Throw(exceptionValue, retType) : (Expression) Expression.Throw(exceptionValue, retType);
  }

  public static object CheckAndThrow(object value)
  {
    if (value is LightExceptions.LightException lightEx)
      LightExceptions.ThrowException(lightEx);
    return value;
  }

  private static void ThrowException(LightExceptions.LightException lightEx)
  {
    throw lightEx.Exception;
  }

  public static Expression CheckAndThrow(Expression expr)
  {
    ContractUtils.RequiresNotNull((object) expr, nameof (expr));
    ContractUtils.Requires(expr.Type == typeof (object), "checked expression must be type of object");
    return (Expression) new LightCheckAndThrowExpression(expr);
  }

  public static bool IsLightException(object value) => value is LightExceptions.LightException;

  public static Exception GetLightException(object exceptionValue)
  {
    return !(exceptionValue is LightExceptions.LightException lightException) ? (Exception) null : lightException.Exception;
  }

  public static bool SupportsLightThrow(this CallSiteBinder binder)
  {
    return binder is ILightExceptionBinder lightExceptionBinder && lightExceptionBinder.SupportsLightThrow;
  }

  private static ReadOnlyCollection<Expression> ToReadOnly(Expression[] args)
  {
    return new ReadOnlyCollectionBuilder<Expression>((IEnumerable<Expression>) args).ToReadOnlyCollection();
  }

  private sealed class LightException
  {
    public readonly Exception Exception;

    public LightException(Exception exception) => this.Exception = exception;
  }
}
