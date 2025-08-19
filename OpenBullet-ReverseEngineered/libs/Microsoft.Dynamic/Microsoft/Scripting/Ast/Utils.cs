// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.Utils
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Ast;

public static class Utils
{
  private static readonly ConstantExpression TrueLiteral = Expression.Constant((object) true, typeof (bool));
  private static readonly ConstantExpression FalseLiteral = Expression.Constant((object) false, typeof (bool));
  private static readonly ConstantExpression NullLiteral = Expression.Constant((object) null, typeof (object));
  private static readonly ConstantExpression EmptyStringLiteral = Expression.Constant((object) string.Empty, typeof (string));
  private static readonly ConstantExpression[] IntCache = new ConstantExpression[100];
  private static readonly DefaultExpression VoidInstance = Expression.Empty();

  public static Expression Coalesce(
    Expression left,
    Expression right,
    out ParameterExpression temp)
  {
    return Microsoft.Scripting.Ast.Utils.CoalesceInternal(left, right, (MethodInfo) null, false, out temp);
  }

  public static Expression CoalesceTrue(
    Expression left,
    Expression right,
    MethodInfo isTrue,
    out ParameterExpression temp)
  {
    ContractUtils.RequiresNotNull((object) isTrue, nameof (isTrue));
    return Microsoft.Scripting.Ast.Utils.CoalesceInternal(left, right, isTrue, false, out temp);
  }

  public static Expression CoalesceFalse(
    Expression left,
    Expression right,
    MethodInfo isTrue,
    out ParameterExpression temp)
  {
    ContractUtils.RequiresNotNull((object) isTrue, nameof (isTrue));
    return Microsoft.Scripting.Ast.Utils.CoalesceInternal(left, right, isTrue, true, out temp);
  }

  private static Expression CoalesceInternal(
    Expression left,
    Expression right,
    MethodInfo isTrue,
    bool isReverse,
    out ParameterExpression temp)
  {
    ContractUtils.RequiresNotNull((object) left, nameof (left));
    ContractUtils.RequiresNotNull((object) right, nameof (right));
    ContractUtils.Requires(left.Type == right.Type, "Expression types must match");
    temp = Expression.Variable(left.Type, "tmp_left");
    Expression test;
    if (isTrue != (MethodInfo) null)
    {
      ContractUtils.Requires(isTrue.ReturnType == typeof (bool), nameof (isTrue), "Predicate must return bool.");
      ParameterInfo[] parameters = isTrue.GetParameters();
      ContractUtils.Requires(parameters.Length == 1, nameof (isTrue), "Predicate must take one parameter.");
      ContractUtils.Requires(isTrue.IsStatic && isTrue.IsPublic, nameof (isTrue), "Predicate must be public and static.");
      ContractUtils.Requires(TypeUtils.CanAssign(parameters[0].ParameterType, left.Type), nameof (left), "Incorrect left expression type");
      test = (Expression) Expression.Call(isTrue, (Expression) Expression.Assign((Expression) temp, left));
    }
    else
    {
      ContractUtils.Requires(TypeUtils.CanCompareToNull(left.Type), nameof (left), "Incorrect left expression type");
      test = (Expression) Expression.Equal((Expression) Expression.Assign((Expression) temp, left), (Expression) Microsoft.Scripting.Ast.Utils.Constant((object) null, left.Type));
    }
    Expression ifTrue;
    Expression ifFalse;
    if (isReverse)
    {
      ifTrue = (Expression) temp;
      ifFalse = right;
    }
    else
    {
      ifTrue = right;
      ifFalse = (Expression) temp;
    }
    return (Expression) Expression.Condition(test, ifTrue, ifFalse);
  }

  public static Expression Coalesce(LambdaBuilder builder, Expression left, Expression right)
  {
    ParameterExpression temp;
    Expression expression = Microsoft.Scripting.Ast.Utils.Coalesce(left, right, out temp);
    builder.AddHiddenVariable(temp);
    return expression;
  }

  public static Expression CoalesceTrue(
    LambdaBuilder builder,
    Expression left,
    Expression right,
    MethodInfo isTrue)
  {
    ContractUtils.RequiresNotNull((object) isTrue, nameof (isTrue));
    ParameterExpression temp;
    Expression expression = Microsoft.Scripting.Ast.Utils.CoalesceTrue(left, right, isTrue, out temp);
    builder.AddHiddenVariable(temp);
    return expression;
  }

  public static Expression CoalesceFalse(
    LambdaBuilder builder,
    Expression left,
    Expression right,
    MethodInfo isTrue)
  {
    ContractUtils.RequiresNotNull((object) isTrue, nameof (isTrue));
    ParameterExpression temp;
    Expression expression = Microsoft.Scripting.Ast.Utils.CoalesceFalse(left, right, isTrue, out temp);
    builder.AddHiddenVariable(temp);
    return expression;
  }

  public static BinaryExpression Update(
    this BinaryExpression expression,
    Expression left,
    Expression right)
  {
    return expression.Update(left, expression.Conversion, right);
  }

  internal static Expression AddScopedVariable(
    Expression body,
    ParameterExpression variable,
    Expression variableInit)
  {
    List<ParameterExpression> variables = new List<ParameterExpression>();
    List<Expression> expressionList = new List<Expression>();
    ReadOnlyCollection<Expression> collection = new ReadOnlyCollection<Expression>((IList<Expression>) new Expression[1]
    {
      body
    });
    BlockExpression blockExpression;
    for (Expression expression = body; collection.Count == 1 && collection[0].NodeType == ExpressionType.Block && expression.Type == collection[0].Type; collection = blockExpression.Expressions)
    {
      blockExpression = (BlockExpression) collection[0];
      variables.AddRange((IEnumerable<ParameterExpression>) blockExpression.Variables);
      expression = (Expression) blockExpression;
    }
    expressionList.Add((Expression) Expression.Assign((Expression) variable, variableInit));
    expressionList.AddRange((IEnumerable<Expression>) collection);
    variables.Add(variable);
    return (Expression) Expression.Block((IEnumerable<ParameterExpression>) variables, expressionList.ToArray());
  }

  internal static BlockExpression BlockVoid(Expression[] expressions)
  {
    if (expressions.Length == 0 || expressions[expressions.Length - 1].Type != typeof (void))
      expressions = ((IList<Expression>) expressions).AddLast<Expression>((Expression) Microsoft.Scripting.Ast.Utils.Empty());
    return Expression.Block(expressions);
  }

  internal static BlockExpression Block(Expression[] expressions)
  {
    if (expressions.Length == 0)
      expressions = ((IList<Expression>) expressions).AddLast<Expression>((Expression) Microsoft.Scripting.Ast.Utils.Empty());
    return Expression.Block(expressions);
  }

  public static MemberExpression WeakConstant(object value)
  {
    return Expression.Property(Microsoft.Scripting.Ast.Utils.Constant((object) new WeakReference(value)), typeof (WeakReference).GetDeclaredProperty("Target"));
  }

  public static ConstantExpression Constant(object value, Type type)
  {
    return Expression.Constant(value, type);
  }

  public static Expression Constant(object value)
  {
    switch (value)
    {
      case null:
        return (Expression) Microsoft.Scripting.Ast.Utils.NullLiteral;
      case BigInteger bigInteger:
        return Microsoft.Scripting.Ast.Utils.BigIntegerConstant(bigInteger);
      case Complex complex:
        return Microsoft.Scripting.Ast.Utils.ComplexConstant(complex);
      case Type _:
        return (Expression) Expression.Constant(value, typeof (Type));
      case ConstructorInfo _:
        return (Expression) Expression.Constant(value, typeof (ConstructorInfo));
      case EventInfo _:
        return (Expression) Expression.Constant(value, typeof (EventInfo));
      case FieldInfo _:
        return (Expression) Expression.Constant(value, typeof (FieldInfo));
      case MethodInfo _:
        return (Expression) Expression.Constant(value, typeof (MethodInfo));
      case PropertyInfo _:
        return (Expression) Expression.Constant(value, typeof (PropertyInfo));
      default:
        Type type = value.GetType();
        if (!type.IsEnum)
        {
          switch (type.GetTypeCode())
          {
            case TypeCode.Boolean:
              return !(bool) value ? (Expression) Microsoft.Scripting.Ast.Utils.FalseLiteral : (Expression) Microsoft.Scripting.Ast.Utils.TrueLiteral;
            case TypeCode.Int32:
              int num = (int) value;
              int index = num + 2;
              if (index >= 0 && index < Microsoft.Scripting.Ast.Utils.IntCache.Length)
              {
                ConstantExpression constantExpression;
                if ((constantExpression = Microsoft.Scripting.Ast.Utils.IntCache[index]) == null)
                  Microsoft.Scripting.Ast.Utils.IntCache[index] = constantExpression = Microsoft.Scripting.Ast.Utils.Constant((object) num, typeof (int));
                return (Expression) constantExpression;
              }
              break;
            case TypeCode.String:
              if (string.IsNullOrEmpty((string) value))
                return (Expression) Microsoft.Scripting.Ast.Utils.EmptyStringLiteral;
              break;
          }
        }
        return (Expression) Expression.Constant(value);
    }
  }

  private static Expression BigIntegerConstant(BigInteger value)
  {
    int ret1;
    if (value.AsInt32(out ret1))
      return (Expression) Expression.Call(RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Func<int, BigInteger>(CompilerHelpers.CreateBigInteger)), Microsoft.Scripting.Ast.Utils.Constant((object) ret1));
    long ret2;
    return value.AsInt64(out ret2) ? (Expression) Expression.Call(RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Func<long, BigInteger>(CompilerHelpers.CreateBigInteger)), Microsoft.Scripting.Ast.Utils.Constant((object) ret2)) : (Expression) Expression.Call(RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Func<bool, byte[], BigInteger>(CompilerHelpers.CreateBigInteger)), Microsoft.Scripting.Ast.Utils.Constant((object) (value.Sign < 0)), Microsoft.Scripting.Ast.Utils.CreateArray<byte>(value.Abs().ToByteArray()));
  }

  private static Expression CreateArray<T>(T[] array)
  {
    Expression[] expressionArray = new Expression[array.Length];
    for (int index = 0; index < expressionArray.Length; ++index)
      expressionArray[index] = Microsoft.Scripting.Ast.Utils.Constant((object) array[index]);
    return (Expression) Expression.NewArrayInit(typeof (T), expressionArray);
  }

  private static Expression ComplexConstant(Complex value)
  {
    if (value.Real == 0.0)
      return (Expression) Expression.Call(RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Func<double, Complex>(MathUtils.MakeImaginary)), Microsoft.Scripting.Ast.Utils.Constant((object) value.Imaginary()));
    return value.Imaginary() != 0.0 ? (Expression) Expression.Call(RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Func<double, double, Complex>(MathUtils.MakeComplex)), Microsoft.Scripting.Ast.Utils.Constant((object) value.Real), Microsoft.Scripting.Ast.Utils.Constant((object) value.Imaginary())) : (Expression) Expression.Call(RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Func<double, Complex>(MathUtils.MakeReal)), Microsoft.Scripting.Ast.Utils.Constant((object) value.Real));
  }

  public static Expression DebugMarker(string marker)
  {
    ContractUtils.RequiresNotNull((object) marker, nameof (marker));
    return (Expression) Microsoft.Scripting.Ast.Utils.Empty();
  }

  public static Expression DebugMark(Expression expression, string marker)
  {
    ContractUtils.RequiresNotNull((object) expression, nameof (expression));
    ContractUtils.RequiresNotNull((object) marker, nameof (marker));
    return expression;
  }

  public static Expression AddDebugInfo(
    Expression expression,
    SymbolDocumentInfo document,
    SourceLocation start,
    SourceLocation end)
  {
    return document == null || !start.IsValid || !end.IsValid ? expression : Microsoft.Scripting.Ast.Utils.AddDebugInfo(expression, document, start.Line, start.Column, end.Line, end.Column);
  }

  public static Expression AddDebugInfo(
    Expression expression,
    SymbolDocumentInfo document,
    int startLine,
    int startColumn,
    int endLine,
    int endColumn)
  {
    if (expression == null)
      throw new ArgumentNullException(nameof (expression));
    DebugInfoExpression debugInfoExpression1 = Expression.DebugInfo(document, startLine, startColumn, endLine, endColumn);
    DebugInfoExpression debugInfoExpression2 = Expression.ClearDebugInfo(document);
    if (expression.Type == typeof (void))
      return (Expression) Expression.Block((Expression) debugInfoExpression1, expression, (Expression) debugInfoExpression2);
    ParameterExpression left = Expression.Parameter(expression.Type, (string) null);
    return (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      left
    }, (Expression) debugInfoExpression1, (Expression) Expression.Assign((Expression) left, expression), (Expression) debugInfoExpression2, (Expression) left);
  }

  public static DefaultExpression Empty() => Microsoft.Scripting.Ast.Utils.VoidInstance;

  public static DefaultExpression Default(Type type)
  {
    return type == typeof (void) ? Microsoft.Scripting.Ast.Utils.Empty() : Expression.Default(type);
  }

  public static Expression FinallyFlowControl(Expression body)
  {
    return (Expression) new FinallyFlowControlExpression(body);
  }

  public static GeneratorExpression Generator(LabelTarget label, Expression body)
  {
    ContractUtils.RequiresNotNull((object) label, nameof (label));
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    ContractUtils.Requires(label.Type != typeof (void), nameof (label), "label must have a non-void type");
    return new GeneratorExpression("generator", typeof (IEnumerable<>).MakeGenericType(label.Type), label, body, true);
  }

  public static GeneratorExpression Generator(LabelTarget label, Expression body, Type type)
  {
    return Microsoft.Scripting.Ast.Utils.Generator("generator", label, body, type);
  }

  public static GeneratorExpression Generator(
    string name,
    LabelTarget label,
    Expression body,
    Type type)
  {
    return Microsoft.Scripting.Ast.Utils.Generator(name, label, body, type, true);
  }

  public static GeneratorExpression Generator(
    string name,
    LabelTarget label,
    Expression body,
    Type type,
    bool rewriteAssignments)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    ContractUtils.RequiresNotNull((object) label, nameof (label));
    ContractUtils.Requires(label.Type != typeof (void), nameof (label), "label must have a non-void type");
    ContractUtils.Requires(body.Type == typeof (void), nameof (body), "generator body must have a void type");
    if (type.IsGenericType())
    {
      Type genericTypeDefinition = type.GetGenericTypeDefinition();
      if (genericTypeDefinition != typeof (IEnumerable<>) && genericTypeDefinition != typeof (IEnumerator<>) || type.GetGenericArguments()[0] != label.Type)
        throw Microsoft.Scripting.Ast.Utils.GeneratorTypeMustBeEnumerableOfT(label.Type);
    }
    else if (type != typeof (IEnumerable) && type != typeof (IEnumerator))
      throw Microsoft.Scripting.Ast.Utils.GeneratorTypeMustBeEnumerableOfT(label.Type);
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    return new GeneratorExpression(name, type, label, body, rewriteAssignments);
  }

  private static ArgumentException GeneratorTypeMustBeEnumerableOfT(Type type)
  {
    return new ArgumentException($"Generator must be of type IEnumerable<T>, IEnumerator<T>, IEnumerable, or IEnumerator, where T is '{type}'");
  }

  internal static bool IsEnumerableType(Type type)
  {
    if (type == typeof (IEnumerable))
      return true;
    return type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (IEnumerable<>);
  }

  public static Expression<T> GeneratorLambda<T>(
    LabelTarget label,
    Expression body,
    params ParameterExpression[] parameters)
  {
    return (Expression<T>) Microsoft.Scripting.Ast.Utils.GeneratorLambda(typeof (T), label, body, (string) null, parameters);
  }

  public static Expression<T> GeneratorLambda<T>(
    LabelTarget label,
    Expression body,
    string name,
    params ParameterExpression[] parameters)
  {
    return (Expression<T>) Microsoft.Scripting.Ast.Utils.GeneratorLambda(typeof (T), label, body, name, parameters);
  }

  public static Expression<T> GeneratorLambda<T>(
    LabelTarget label,
    Expression body,
    string name,
    IEnumerable<ParameterExpression> parameters)
  {
    return (Expression<T>) Microsoft.Scripting.Ast.Utils.GeneratorLambda(typeof (T), label, body, name, parameters);
  }

  public static LambdaExpression GeneratorLambda(
    Type delegateType,
    LabelTarget label,
    Expression body,
    params ParameterExpression[] parameters)
  {
    return Microsoft.Scripting.Ast.Utils.GeneratorLambda(delegateType, label, body, (string) null, parameters);
  }

  public static LambdaExpression GeneratorLambda(
    Type delegateType,
    LabelTarget label,
    Expression body,
    string name,
    params ParameterExpression[] parameters)
  {
    return Microsoft.Scripting.Ast.Utils.GeneratorLambda(delegateType, label, body, name, (IEnumerable<ParameterExpression>) parameters);
  }

  public static LambdaExpression GeneratorLambda(
    Type delegateType,
    LabelTarget label,
    Expression body,
    string name,
    IEnumerable<ParameterExpression> parameters)
  {
    return Microsoft.Scripting.Ast.Utils.GeneratorLambda(delegateType, label, body, name, true, parameters);
  }

  public static LambdaExpression GeneratorLambda(
    Type delegateType,
    LabelTarget label,
    Expression body,
    string name,
    bool rewriteAssignments,
    IEnumerable<ParameterExpression> parameters)
  {
    ContractUtils.RequiresNotNull((object) delegateType, nameof (delegateType));
    ContractUtils.Requires(delegateType.IsSubclassOf(typeof (MulticastDelegate)), "Lambda type parameter must be derived from System.Delegate");
    Type returnType = delegateType.GetMethod("Invoke").GetReturnType();
    ReadOnlyCollection<ParameterExpression> readOnlyCollection = parameters.ToReadOnly<ParameterExpression>();
    if (Microsoft.Scripting.Ast.Utils.IsEnumerableType(returnType))
      body = Microsoft.Scripting.Ast.Utils.TransformEnumerable(body, readOnlyCollection);
    return Expression.Lambda(delegateType, (Expression) Microsoft.Scripting.Ast.Utils.Generator(name, label, body, returnType, rewriteAssignments), name, (IEnumerable<ParameterExpression>) readOnlyCollection);
  }

  private static Expression TransformEnumerable(
    Expression body,
    ReadOnlyCollection<ParameterExpression> paramList)
  {
    if (paramList.Count == 0)
      return body;
    int count = paramList.Count;
    ParameterExpression[] list1 = new ParameterExpression[count];
    Dictionary<ParameterExpression, ParameterExpression> map = new Dictionary<ParameterExpression, ParameterExpression>(count);
    Expression[] list2 = new Expression[count + 1];
    for (int index = 0; index < count; ++index)
    {
      ParameterExpression parameterExpression = paramList[index];
      list1[index] = Expression.Variable(parameterExpression.Type, parameterExpression.Name);
      map.Add(parameterExpression, list1[index]);
      list2[index] = (Expression) Expression.Assign((Expression) list1[index], (Expression) parameterExpression);
    }
    list2[count] = new LambdaParameterRewriter(map).Visit(body);
    return (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ReadOnlyCollection<ParameterExpression>((IList<ParameterExpression>) list1), (IEnumerable<Expression>) new ReadOnlyCollection<Expression>((IList<Expression>) list2));
  }

  public static IfStatementBuilder If() => new IfStatementBuilder();

  public static IfStatementBuilder If(Expression test, params Expression[] body)
  {
    return Microsoft.Scripting.Ast.Utils.If().ElseIf(test, body);
  }

  public static IfStatementBuilder If(Expression test, Expression body)
  {
    return Microsoft.Scripting.Ast.Utils.If().ElseIf(test, body);
  }

  public static Expression If(IfStatementTest[] tests, Expression @else)
  {
    ContractUtils.RequiresNotNullItems<IfStatementTest>((IList<IfStatementTest>) tests, nameof (tests));
    return IfStatementBuilder.BuildConditions((IList<IfStatementTest>) tests, @else);
  }

  public static Expression IfThen(Expression test, Expression body)
  {
    return Microsoft.Scripting.Ast.Utils.IfThenElse(test, body, (Expression) null);
  }

  public static Expression IfThen(Expression test, params Expression[] body)
  {
    return Microsoft.Scripting.Ast.Utils.IfThenElse(test, (Expression) Microsoft.Scripting.Ast.Utils.BlockVoid(body), (Expression) null);
  }

  public static Expression IfThenElse(Expression test, Expression body, Expression @else)
  {
    return Microsoft.Scripting.Ast.Utils.If(new IfStatementTest[1]
    {
      Microsoft.Scripting.Ast.Utils.IfCondition(test, body)
    }, @else);
  }

  public static Expression Unless(Expression test, Expression body)
  {
    return Microsoft.Scripting.Ast.Utils.IfThenElse(test, (Expression) Microsoft.Scripting.Ast.Utils.Empty(), body);
  }

  public static IfStatementTest IfCondition(Expression test, Expression body)
  {
    ContractUtils.RequiresNotNull((object) test, nameof (test));
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    ContractUtils.Requires(test.Type == typeof (bool), nameof (test), "Test must be boolean");
    return new IfStatementTest(test, body);
  }

  public static LambdaBuilder Lambda(Type returnType, string name)
  {
    return new LambdaBuilder(name, returnType);
  }

  public static LightDynamicExpression LightDynamic(CallSiteBinder binder, Expression arg0)
  {
    return Microsoft.Scripting.Ast.Utils.LightDynamic(binder, typeof (object), arg0);
  }

  public static LightDynamicExpression LightDynamic(
    CallSiteBinder binder,
    Type returnType,
    Expression arg0)
  {
    return !(returnType == typeof (object)) ? (LightDynamicExpression) new LightTypedDynamicExpression1(binder, returnType, arg0) : (LightDynamicExpression) new LightDynamicExpression1(binder, arg0);
  }

  public static LightDynamicExpression LightDynamic(
    CallSiteBinder binder,
    Expression arg0,
    Expression arg1)
  {
    return Microsoft.Scripting.Ast.Utils.LightDynamic(binder, typeof (object), arg0, arg1);
  }

  public static LightDynamicExpression LightDynamic(
    CallSiteBinder binder,
    Type returnType,
    Expression arg0,
    Expression arg1)
  {
    return !(returnType == typeof (object)) ? (LightDynamicExpression) new LightTypedDynamicExpression2(binder, returnType, arg0, arg1) : (LightDynamicExpression) new LightDynamicExpression2(binder, arg0, arg1);
  }

  public static LightDynamicExpression LightDynamic(
    CallSiteBinder binder,
    Expression arg0,
    Expression arg1,
    Expression arg2)
  {
    return Microsoft.Scripting.Ast.Utils.LightDynamic(binder, typeof (object), arg0, arg1, arg2);
  }

  public static LightDynamicExpression LightDynamic(
    CallSiteBinder binder,
    Type returnType,
    Expression arg0,
    Expression arg1,
    Expression arg2)
  {
    return !(returnType == typeof (object)) ? (LightDynamicExpression) new LightTypedDynamicExpression3(binder, returnType, arg0, arg1, arg2) : (LightDynamicExpression) new LightDynamicExpression3(binder, arg0, arg1, arg2);
  }

  public static LightDynamicExpression LightDynamic(
    CallSiteBinder binder,
    Expression arg0,
    Expression arg1,
    Expression arg2,
    Expression arg3)
  {
    return Microsoft.Scripting.Ast.Utils.LightDynamic(binder, typeof (object), arg0, arg1, arg2, arg3);
  }

  public static LightDynamicExpression LightDynamic(
    CallSiteBinder binder,
    Type returnType,
    Expression arg0,
    Expression arg1,
    Expression arg2,
    Expression arg3)
  {
    return !(returnType == typeof (object)) ? (LightDynamicExpression) new LightTypedDynamicExpression4(binder, returnType, arg0, arg1, arg2, arg3) : (LightDynamicExpression) new LightDynamicExpression4(binder, arg0, arg1, arg2, arg3);
  }

  public static LightDynamicExpression LightDynamic(
    CallSiteBinder binder,
    IList<Expression> arguments)
  {
    return Microsoft.Scripting.Ast.Utils.LightDynamic(binder, typeof (object), arguments);
  }

  public static LightDynamicExpression LightDynamic(
    CallSiteBinder binder,
    Type returnType,
    IList<Expression> arguments)
  {
    ContractUtils.RequiresNotNull((object) arguments, nameof (arguments));
    return (LightDynamicExpression) new LightTypedDynamicExpressionN(binder, returnType, arguments);
  }

  public static LightDynamicExpression LightDynamic(
    CallSiteBinder binder,
    ExpressionCollectionBuilder<Expression> arguments)
  {
    return Microsoft.Scripting.Ast.Utils.LightDynamic(binder, typeof (object), arguments);
  }

  public static LightDynamicExpression LightDynamic(
    CallSiteBinder binder,
    Type returnType,
    ExpressionCollectionBuilder<Expression> arguments)
  {
    ContractUtils.RequiresNotNull((object) arguments, nameof (arguments));
    switch (arguments.Count)
    {
      case 1:
        return Microsoft.Scripting.Ast.Utils.LightDynamic(binder, returnType, arguments.Expression0);
      case 2:
        return Microsoft.Scripting.Ast.Utils.LightDynamic(binder, returnType, arguments.Expression0, arguments.Expression1);
      case 3:
        return Microsoft.Scripting.Ast.Utils.LightDynamic(binder, returnType, arguments.Expression0, arguments.Expression1, arguments.Expression2);
      case 4:
        return Microsoft.Scripting.Ast.Utils.LightDynamic(binder, returnType, arguments.Expression0, arguments.Expression1, arguments.Expression2, arguments.Expression3);
      default:
        return Microsoft.Scripting.Ast.Utils.LightDynamic(binder, returnType, (IList<Expression>) arguments.Expressions);
    }
  }

  public static LightExpression<T> LightLambda<T>(
    Type retType,
    Expression body,
    string name,
    IList<ParameterExpression> args)
  {
    return new LightExpression<T>(retType, body, name, args);
  }

  public static LightLambdaExpression LightLambda(
    Type retType,
    Type delegateType,
    Expression body,
    string name,
    IList<ParameterExpression> args)
  {
    return (LightLambdaExpression) new TypedLightLambdaExpression(retType, delegateType, body, name, args);
  }

  public static LoopExpression While(Expression test, Expression body, Expression @else)
  {
    return Microsoft.Scripting.Ast.Utils.Loop(test, (Expression) null, body, @else, (LabelTarget) null, (LabelTarget) null);
  }

  public static LoopExpression While(
    Expression test,
    Expression body,
    Expression @else,
    LabelTarget @break,
    LabelTarget @continue)
  {
    return Microsoft.Scripting.Ast.Utils.Loop(test, (Expression) null, body, @else, @break, @continue);
  }

  public static LoopExpression Infinite(Expression body)
  {
    return Expression.Loop(body, (LabelTarget) null, (LabelTarget) null);
  }

  public static LoopExpression Infinite(Expression body, LabelTarget @break, LabelTarget @continue)
  {
    return Expression.Loop(body, @break, @continue);
  }

  public static LoopExpression Loop(
    Expression test,
    Expression increment,
    Expression body,
    Expression @else)
  {
    return Microsoft.Scripting.Ast.Utils.Loop(test, increment, body, @else, (LabelTarget) null, (LabelTarget) null);
  }

  public static LoopExpression Loop(
    Expression test,
    Expression increment,
    Expression body,
    Expression @else,
    LabelTarget @break,
    LabelTarget @continue)
  {
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    if (test != null)
    {
      ContractUtils.Requires(test.Type == typeof (bool), nameof (test), "Test must be boolean");
      if (@break == null)
        @break = Expression.Label();
    }
    return Expression.Loop((Expression) Expression.Block(test != null ? (Expression) Expression.Condition(test, (Expression) Microsoft.Scripting.Ast.Utils.Empty(), (Expression) Expression.Block(@else ?? (Expression) Microsoft.Scripting.Ast.Utils.Empty(), (Expression) Expression.Break(@break))) : (Expression) Microsoft.Scripting.Ast.Utils.Empty(), body, @continue != null ? (Expression) Expression.Label(@continue) : (Expression) Microsoft.Scripting.Ast.Utils.Empty(), increment ?? (Expression) Microsoft.Scripting.Ast.Utils.Empty()), @break, (LabelTarget) null);
  }

  public static MethodCallExpression SimpleCallHelper(
    MethodInfo method,
    params Expression[] arguments)
  {
    ContractUtils.RequiresNotNull((object) method, nameof (method));
    ContractUtils.Requires(method.IsStatic, nameof (method), "Method must be static");
    return Microsoft.Scripting.Ast.Utils.SimpleCallHelper((Expression) null, method, arguments);
  }

  public static MethodCallExpression SimpleCallHelper(
    Expression instance,
    MethodInfo method,
    params Expression[] arguments)
  {
    ContractUtils.RequiresNotNull((object) method, nameof (method));
    ContractUtils.Requires(instance != null ^ method.IsStatic, nameof (instance));
    ContractUtils.RequiresNotNullItems<Expression>((IList<Expression>) arguments, nameof (arguments));
    ParameterInfo[] parameters = method.GetParameters();
    ContractUtils.Requires(arguments.Length == parameters.Length, nameof (arguments), "Incorrect number of arguments");
    if (instance != null)
      instance = Microsoft.Scripting.Ast.Utils.Convert(instance, method.DeclaringType);
    Expression[] expressionArray = Microsoft.Scripting.Ast.Utils.ArgumentConvertHelper(arguments, parameters);
    ReadOnlyCollection<Expression> arguments1 = expressionArray != arguments ? new ReadOnlyCollection<Expression>((IList<Expression>) expressionArray) : ((IEnumerable<Expression>) expressionArray).ToReadOnly<Expression>();
    return Expression.Call(instance, method, (IEnumerable<Expression>) arguments1);
  }

  private static Expression[] ArgumentConvertHelper(
    Expression[] arguments,
    ParameterInfo[] parameters)
  {
    Expression[] expressionArray = (Expression[]) null;
    for (int index1 = 0; index1 < arguments.Length; ++index1)
    {
      Expression expression = arguments[index1];
      if (!Microsoft.Scripting.Ast.Utils.CompatibleParameterTypes(parameters[index1].ParameterType, expression.Type))
      {
        if (expressionArray == null)
        {
          expressionArray = new Expression[arguments.Length];
          for (int index2 = 0; index2 < index1; ++index2)
            expressionArray[index2] = arguments[index2];
        }
        expression = Microsoft.Scripting.Ast.Utils.ArgumentConvertHelper(expression, parameters[index1].ParameterType);
      }
      if (expressionArray != null)
        expressionArray[index1] = expression;
    }
    return expressionArray ?? arguments;
  }

  private static Expression ArgumentConvertHelper(Expression argument, Type type)
  {
    if (argument.Type != type)
    {
      if (type.IsByRef)
        type = type.GetElementType();
      if (argument.Type != type)
        argument = Microsoft.Scripting.Ast.Utils.Convert(argument, type);
    }
    return argument;
  }

  private static bool CompatibleParameterTypes(Type parameter, Type argument)
  {
    return parameter == argument || !parameter.IsValueType() && !argument.IsValueType() && parameter.IsAssignableFrom(argument) || parameter.IsByRef && parameter.GetElementType() == argument;
  }

  public static Expression ComplexCallHelper(MethodInfo method, params Expression[] arguments)
  {
    ContractUtils.RequiresNotNull((object) method, nameof (method));
    ContractUtils.Requires(method.IsStatic, nameof (method), "Method must be static");
    return Microsoft.Scripting.Ast.Utils.ComplexCallHelper((Expression) null, method, arguments);
  }

  public static Expression ComplexCallHelper(
    Expression instance,
    MethodInfo method,
    params Expression[] arguments)
  {
    ContractUtils.RequiresNotNull((object) method, nameof (method));
    ContractUtils.RequiresNotNullItems<Expression>((IList<Expression>) arguments, nameof (arguments));
    ContractUtils.Requires(instance != null ^ method.IsStatic, nameof (instance));
    ParameterInfo[] parameters = method.GetParameters();
    bool flag = parameters.Length != 0 && parameters[parameters.Length - 1].IsParamArray();
    if (instance != null)
      instance = Microsoft.Scripting.Ast.Utils.Convert(instance, method.DeclaringType);
    Expression[] expressionArray1 = (Expression[]) null;
    int index1 = 0;
    int index2 = 0;
    for (; index1 < parameters.Length; ++index1)
    {
      ParameterInfo parameterInfo = parameters[index1];
      Expression expression1;
      if (index1 == parameters.Length - 1 & flag)
      {
        if (index2 < arguments.Length)
        {
          if (index2 == arguments.Length - 1 && Microsoft.Scripting.Ast.Utils.CompatibleParameterTypes(parameterInfo.ParameterType, arguments[index2].Type))
          {
            expression1 = arguments[index2++];
          }
          else
          {
            Type elementType = parameterInfo.ParameterType.GetElementType();
            Expression[] expressionArray2 = new Expression[arguments.Length - index2];
            int num = 0;
            while (index2 < arguments.Length)
              expressionArray2[num++] = Microsoft.Scripting.Ast.Utils.Convert(arguments[index2++], elementType);
            expression1 = (Expression) Expression.NewArrayInit(elementType, expressionArray2);
          }
        }
        else
          expression1 = (Expression) Expression.NewArrayInit(parameterInfo.ParameterType.GetElementType());
      }
      else if (index2 < arguments.Length)
      {
        expression1 = arguments[index2++];
      }
      else
      {
        ContractUtils.Requires(!parameterInfo.IsMandatory(), nameof (arguments), "Argument not provided for a mandatory parameter");
        expression1 = Microsoft.Scripting.Ast.Utils.CreateDefaultValueExpression(parameterInfo);
      }
      Expression expression2 = Microsoft.Scripting.Ast.Utils.ArgumentConvertHelper(expression1, parameterInfo.ParameterType);
      if (expressionArray1 == null && (index1 >= arguments.Length || expression2 != arguments[index1]))
      {
        expressionArray1 = new Expression[parameters.Length];
        for (int index3 = 0; index3 < index1; ++index3)
          expressionArray1[index3] = arguments[index3];
      }
      if (expressionArray1 != null)
        expressionArray1[index1] = expression2;
    }
    ContractUtils.Requires(index2 == arguments.Length, nameof (arguments), "Incorrect number of arguments");
    return (Expression) Expression.Call(instance, method, expressionArray1 ?? arguments);
  }

  private static Expression CreateDefaultValueExpression(ParameterInfo parameter)
  {
    return parameter.HasDefaultValue() ? (Expression) Microsoft.Scripting.Ast.Utils.Constant(parameter.GetDefaultValue(), parameter.ParameterType) : throw new NotSupportedException("missing parameter value not supported");
  }

  public static NewArrayExpression NewArrayHelper(Type type, IEnumerable<Expression> initializers)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    ContractUtils.RequiresNotNull((object) initializers, nameof (initializers));
    if (type.Equals(typeof (void)))
      throw new ArgumentException("Argument type cannot be System.Void.");
    ReadOnlyCollection<Expression> initializers1 = initializers.ToReadOnly<Expression>();
    Expression[] list = (Expression[]) null;
    for (int index1 = 0; index1 < initializers1.Count; ++index1)
    {
      Expression expression = initializers1[index1];
      ContractUtils.RequiresNotNull((object) expression, nameof (initializers));
      if (!TypeUtils.AreReferenceAssignable(type, expression.Type))
      {
        if (list == null)
        {
          list = new Expression[initializers1.Count];
          for (int index2 = 0; index2 < index1; ++index2)
            list[index2] = initializers1[index2];
        }
        expression = !type.IsSubclassOf(typeof (Expression)) || !TypeUtils.AreAssignable(type, expression.GetType()) ? Microsoft.Scripting.Ast.Utils.Convert(expression, type) : (Expression) Expression.Quote(expression);
      }
      if (list != null)
        list[index1] = expression;
    }
    if (list != null)
      initializers1 = new ReadOnlyCollection<Expression>((IList<Expression>) list);
    return Expression.NewArrayInit(type, (IEnumerable<Expression>) initializers1);
  }

  public static NewExpression SimpleNewHelper(
    ConstructorInfo constructor,
    params Expression[] arguments)
  {
    ContractUtils.RequiresNotNull((object) constructor, nameof (constructor));
    ContractUtils.RequiresNotNullItems<Expression>((IList<Expression>) arguments, nameof (arguments));
    ParameterInfo[] parameters = constructor.GetParameters();
    ContractUtils.Requires(arguments.Length == parameters.Length, nameof (arguments), "Incorrect number of arguments");
    return Expression.New(constructor, Microsoft.Scripting.Ast.Utils.ArgumentConvertHelper(arguments, parameters));
  }

  public static TryStatementBuilder Try(Expression body)
  {
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    return new TryStatementBuilder(body);
  }

  public static TryStatementBuilder Try(Expression expr0, Expression expr1)
  {
    return new TryStatementBuilder((Expression) Expression.Block(expr0, expr1));
  }

  public static TryStatementBuilder Try(Expression expr0, Expression expr1, Expression expr2)
  {
    return new TryStatementBuilder((Expression) Expression.Block(expr0, expr1, expr2));
  }

  public static TryStatementBuilder Try(
    Expression expr0,
    Expression expr1,
    Expression expr2,
    Expression expr3)
  {
    return new TryStatementBuilder((Expression) Expression.Block(expr0, expr1, expr2, expr3));
  }

  public static TryStatementBuilder Try(params Expression[] body)
  {
    return new TryStatementBuilder((Expression) Expression.Block(body));
  }

  public static Expression Void(Expression expression)
  {
    ContractUtils.RequiresNotNull((object) expression, nameof (expression));
    return expression.Type == typeof (void) ? expression : (Expression) Expression.Block(expression, (Expression) Microsoft.Scripting.Ast.Utils.Empty());
  }

  public static Expression Convert(Expression expression, Type type)
  {
    ContractUtils.RequiresNotNull((object) expression, nameof (expression));
    if (expression.Type == type)
      return expression;
    if (expression.Type == typeof (void))
      return (Expression) Expression.Block(expression, (Expression) Microsoft.Scripting.Ast.Utils.Default(type));
    if (type == typeof (void))
      return Microsoft.Scripting.Ast.Utils.Void(expression);
    return type == typeof (object) ? Microsoft.Scripting.Ast.Utils.Box(expression) : (Expression) Expression.Convert(expression, type);
  }

  public static Expression Box(Expression expression)
  {
    MethodInfo method = !(expression.Type == typeof (int)) ? (!(expression.Type == typeof (bool)) ? (MethodInfo) null : ScriptingRuntimeHelpers.BooleanToObjectMethod) : ScriptingRuntimeHelpers.Int32ToObjectMethod;
    return (Expression) Expression.Convert(expression, typeof (object), method);
  }

  public static bool IsAssignment(this ExpressionType type)
  {
    return type.IsWriteOnlyAssignment() || type.IsReadWriteAssignment();
  }

  public static bool IsWriteOnlyAssignment(this ExpressionType type)
  {
    return type == ExpressionType.Assign;
  }

  public static bool IsReadWriteAssignment(this ExpressionType type)
  {
    return (uint) (type - 63 /*0x3F*/) <= 17U;
  }

  public static ExpressionAccess GetLValueAccess(this ExpressionType type)
  {
    if (type.IsReadWriteAssignment())
      return ExpressionAccess.ReadWrite;
    return type.IsWriteOnlyAssignment() ? ExpressionAccess.Write : ExpressionAccess.Read;
  }

  public static bool IsLValue(this ExpressionType type)
  {
    return type == ExpressionType.MemberAccess || type == ExpressionType.Parameter || type == ExpressionType.Index;
  }

  public static YieldExpression YieldBreak(LabelTarget target)
  {
    return Microsoft.Scripting.Ast.Utils.MakeYield(target, (Expression) null, -1);
  }

  public static YieldExpression YieldReturn(LabelTarget target, Expression value)
  {
    return Microsoft.Scripting.Ast.Utils.MakeYield(target, value, -1);
  }

  public static YieldExpression YieldReturn(LabelTarget target, Expression value, int yieldMarker)
  {
    ContractUtils.RequiresNotNull((object) value, nameof (value));
    return Microsoft.Scripting.Ast.Utils.MakeYield(target, value, yieldMarker);
  }

  public static YieldExpression MakeYield(LabelTarget target, Expression value, int yieldMarker)
  {
    ContractUtils.RequiresNotNull((object) target, nameof (target));
    ContractUtils.Requires(target.Type != typeof (void), nameof (target), "generator label must have a non-void type");
    if (value != null && !TypeUtils.AreReferenceAssignable(target.Type, value.Type))
    {
      if (target.Type.IsSubclassOf(typeof (Expression)) && TypeUtils.AreAssignable(target.Type, value.GetType()))
        value = (Expression) Expression.Quote(value);
      throw new ArgumentException($"Expression of type '{value.Type}' cannot be yielded to a generator label of type '{target.Type}'");
    }
    return new YieldExpression(target, value, yieldMarker);
  }
}
