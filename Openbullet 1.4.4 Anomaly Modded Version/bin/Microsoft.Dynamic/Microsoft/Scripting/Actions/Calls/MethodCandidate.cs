// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.MethodCandidate
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public sealed class MethodCandidate
{
  private readonly List<ParameterWrapper> _parameters;
  private readonly ParameterWrapper _paramsDict;
  private readonly InstanceBuilder _instanceBuilder;

  internal MethodCandidate(
    OverloadResolver resolver,
    OverloadInfo method,
    List<ParameterWrapper> parameters,
    ParameterWrapper paramsDict,
    ReturnBuilder returnBuilder,
    InstanceBuilder instanceBuilder,
    IList<ArgBuilder> argBuilders,
    Dictionary<DynamicMetaObject, BindingRestrictions> restrictions)
  {
    this.Resolver = resolver;
    this.Overload = method;
    this._instanceBuilder = instanceBuilder;
    this.ArgBuilders = argBuilders;
    this.ReturnBuilder = returnBuilder;
    this._parameters = parameters;
    this._paramsDict = paramsDict;
    this.Restrictions = restrictions;
    this.ParamsArrayIndex = ParameterWrapper.IndexOfParamsArray((IList<ParameterWrapper>) parameters);
    parameters.TrimExcess();
  }

  internal MethodCandidate ReplaceMethod(
    OverloadInfo newMethod,
    List<ParameterWrapper> parameters,
    IList<ArgBuilder> argBuilders,
    Dictionary<DynamicMetaObject, BindingRestrictions> restrictions)
  {
    return new MethodCandidate(this.Resolver, newMethod, parameters, this._paramsDict, this.ReturnBuilder, this._instanceBuilder, argBuilders, restrictions);
  }

  internal ReturnBuilder ReturnBuilder { get; }

  internal IList<ArgBuilder> ArgBuilders { get; }

  public OverloadResolver Resolver { get; }

  [Obsolete("Use Overload instead")]
  public MethodBase Method => this.Overload.ReflectionInfo;

  public OverloadInfo Overload { get; }

  internal Dictionary<DynamicMetaObject, BindingRestrictions> Restrictions { get; }

  public Type ReturnType => this.ReturnBuilder.ReturnType;

  public int ParamsArrayIndex { get; }

  public bool HasParamsArray => this.ParamsArrayIndex != -1;

  public bool HasParamsDictionary => this._paramsDict != null;

  public ActionBinder Binder => this.Resolver.Binder;

  internal ParameterWrapper GetParameter(int argumentIndex, ArgumentBinding namesBinding)
  {
    return this._parameters[namesBinding.ArgumentToParameter(argumentIndex)];
  }

  internal ParameterWrapper GetParameter(int parameterIndex) => this._parameters[parameterIndex];

  internal int ParameterCount => this._parameters.Count;

  internal int IndexOfParameter(string name)
  {
    for (int index = 0; index < this._parameters.Count; ++index)
    {
      if (this._parameters[index].Name == name)
        return index;
    }
    return -1;
  }

  public int GetVisibleParameterCount()
  {
    int visibleParameterCount = 0;
    foreach (ParameterWrapper parameter in this._parameters)
    {
      if (!parameter.IsHidden)
        ++visibleParameterCount;
    }
    return visibleParameterCount;
  }

  public IList<ParameterWrapper> GetParameters()
  {
    return (IList<ParameterWrapper>) new ReadOnlyCollection<ParameterWrapper>((IList<ParameterWrapper>) this._parameters);
  }

  internal MethodCandidate MakeParamsExtended(int count, IList<string> names)
  {
    List<ParameterWrapper> parameters = new List<ParameterWrapper>(count);
    List<string> stringList = new List<string>((IEnumerable<string>) names);
    List<int> intList = new List<int>();
    for (int index = 0; index < stringList.Count; ++index)
      intList.Add(index);
    ParameterWrapper parameterWrapper1 = (ParameterWrapper) null;
    int val1 = -1;
    for (int index1 = 0; index1 < this._parameters.Count; ++index1)
    {
      ParameterWrapper parameter = this._parameters[index1];
      if (parameter.IsParamsArray)
      {
        parameterWrapper1 = parameter;
        val1 = index1;
      }
      else
      {
        int index2 = stringList.IndexOf(parameter.Name);
        if (index2 != -1)
        {
          stringList.RemoveAt(index2);
          intList.RemoveAt(index2);
        }
        parameters.Add(parameter);
      }
    }
    if (val1 != -1)
    {
      ParameterWrapper parameterWrapper2 = parameterWrapper1.Expand();
      while (parameters.Count < count - stringList.Count)
        parameters.Insert(Math.Min(val1, parameters.Count), parameterWrapper2);
    }
    if (this._paramsDict != null)
    {
      ParameterBindingFlags flags = (ParameterBindingFlags) ((this.Overload.ProhibitsNullItems(this._paramsDict.ParameterInfo.Position) ? 1 : 0) | (this._paramsDict.IsHidden ? 16 /*0x10*/ : 0));
      foreach (string name in stringList)
        parameters.Add(new ParameterWrapper(this._paramsDict.ParameterInfo, typeof (object), name, flags));
    }
    else if (stringList.Count != 0)
      return (MethodCandidate) null;
    return count != parameters.Count ? (MethodCandidate) null : this.MakeParamsExtended(stringList.ToArray(), intList.ToArray(), parameters);
  }

  private MethodCandidate MakeParamsExtended(
    string[] names,
    int[] nameIndices,
    List<ParameterWrapper> parameters)
  {
    List<ArgBuilder> argBuilders = new List<ArgBuilder>(this.ArgBuilders.Count);
    int num = this.Overload.IsStatic ? 0 : 1;
    int index = -1;
    ArgBuilder argBuilder1 = (ArgBuilder) null;
    foreach (ArgBuilder argBuilder2 in (IEnumerable<ArgBuilder>) this.ArgBuilders)
    {
      switch (argBuilder2)
      {
        case SimpleArgBuilder simpleArgBuilder:
          if (simpleArgBuilder.IsParamsArray)
          {
            int expandedCount = parameters.Count - this.GetConsumedArguments() - names.Length + (this.Overload.IsStatic ? 1 : 0);
            argBuilders.Add((ArgBuilder) new ParamsArgBuilder(simpleArgBuilder.ParameterInfo, simpleArgBuilder.Type.GetElementType(), num, expandedCount));
            num += expandedCount;
            continue;
          }
          if (simpleArgBuilder.IsParamsDict)
          {
            index = argBuilders.Count;
            argBuilder1 = (ArgBuilder) simpleArgBuilder;
            continue;
          }
          argBuilders.Add((ArgBuilder) simpleArgBuilder.MakeCopy(num++));
          continue;
        case KeywordArgBuilder _:
          argBuilders.Add(argBuilder2);
          ++num;
          continue;
        default:
          argBuilders.Add(argBuilder2);
          continue;
      }
    }
    if (index != -1)
      argBuilders.Insert(index, (ArgBuilder) new ParamsDictArgBuilder(argBuilder1.ParameterInfo, num, names, nameIndices));
    return new MethodCandidate(this.Resolver, this.Overload, parameters, (ParameterWrapper) null, this.ReturnBuilder, this._instanceBuilder, (IList<ArgBuilder>) argBuilders, (Dictionary<DynamicMetaObject, BindingRestrictions>) null);
  }

  private int GetConsumedArguments()
  {
    int consumedArguments = 0;
    foreach (ArgBuilder argBuilder in (IEnumerable<ArgBuilder>) this.ArgBuilders)
    {
      if (argBuilder is SimpleArgBuilder simpleArgBuilder && !simpleArgBuilder.IsParamsDict || argBuilder is KeywordArgBuilder)
        ++consumedArguments;
    }
    return consumedArguments;
  }

  public Type[] GetParameterTypes()
  {
    List<Type> typeList = new List<Type>(this.ArgBuilders.Count);
    for (int index = 0; index < this.ArgBuilders.Count; ++index)
    {
      Type type = this.ArgBuilders[index].Type;
      if (type != (Type) null)
        typeList.Add(type);
    }
    return typeList.ToArray();
  }

  internal Expression MakeExpression(RestrictedArguments restrictedArgs)
  {
    bool[] usageMarkers;
    Expression[] spilledArgs;
    Expression[] argumentExpressions = this.GetArgumentExpressions(restrictedArgs, out usageMarkers, out spilledArgs);
    MethodBase reflectionInfo = this.Overload.ReflectionInfo;
    MethodInfo method = !(reflectionInfo == (MethodBase) null) ? reflectionInfo as MethodInfo : throw new InvalidOperationException("Cannot generate an expression for an overload w/o MethodBase");
    Expression ret;
    if (method != (MethodInfo) null)
    {
      Expression expression = !method.IsStatic ? this._instanceBuilder.ToExpression(ref method, this.Resolver, restrictedArgs, usageMarkers) : (Expression) null;
      ret = !CompilerHelpers.IsVisible((MethodBase) method) ? (Expression) Expression.Call(typeof (BinderOps).GetMethod("InvokeMethod"), Microsoft.Scripting.Ast.Utils.Constant((object) method), expression != null ? Microsoft.Scripting.Ast.Utils.Convert(expression, typeof (object)) : Microsoft.Scripting.Ast.Utils.Constant((object) null), (Expression) Microsoft.Scripting.Ast.Utils.NewArrayHelper(typeof (object), (IEnumerable<Expression>) argumentExpressions)) : (Expression) Microsoft.Scripting.Ast.Utils.SimpleCallHelper(expression, method, argumentExpressions);
    }
    else
    {
      ConstructorInfo constructorInfo = (ConstructorInfo) reflectionInfo;
      ret = !CompilerHelpers.IsVisible((MethodBase) constructorInfo) ? (Expression) Expression.Call(typeof (BinderOps).GetMethod("InvokeConstructor"), Microsoft.Scripting.Ast.Utils.Constant((object) constructorInfo), (Expression) Microsoft.Scripting.Ast.Utils.NewArrayHelper(typeof (object), (IEnumerable<Expression>) argumentExpressions)) : (Expression) Microsoft.Scripting.Ast.Utils.SimpleNewHelper(constructorInfo, argumentExpressions);
    }
    if (spilledArgs != null)
      ret = (Expression) Expression.Block(((IList<Expression>) spilledArgs).AddLast<Expression>(ret));
    Expression right = this.ReturnBuilder.ToExpression(this.Resolver, this.ArgBuilders, restrictedArgs, ret);
    List<Expression> expressionList = (List<Expression>) null;
    for (int index = 0; index < this.ArgBuilders.Count; ++index)
    {
      Expression expression = this.ArgBuilders[index].UpdateFromReturn(this.Resolver, restrictedArgs);
      if (expression != null)
      {
        if (expressionList == null)
          expressionList = new List<Expression>();
        expressionList.Add(expression);
      }
    }
    if (expressionList != null)
    {
      if (right.Type != typeof (void))
      {
        ParameterExpression left = Expression.Variable(right.Type, "$ret");
        expressionList.Insert(0, (Expression) Expression.Assign((Expression) left, right));
        expressionList.Add((Expression) left);
        right = (Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
        {
          left
        }, expressionList.ToArray());
      }
      else
      {
        expressionList.Insert(0, right);
        right = (Expression) Expression.Block(typeof (void), expressionList.ToArray());
      }
    }
    if (this.Resolver.Temps != null)
      right = (Expression) Expression.Block((IEnumerable<ParameterExpression>) this.Resolver.Temps, right);
    return right;
  }

  private Expression[] GetArgumentExpressions(
    RestrictedArguments restrictedArgs,
    out bool[] usageMarkers,
    out Expression[] spilledArgs)
  {
    int val1_1 = int.MaxValue;
    int val1_2 = int.MinValue;
    foreach (ArgBuilder argBuilder in (IEnumerable<ArgBuilder>) this.ArgBuilders)
    {
      val1_1 = Math.Min(val1_1, argBuilder.Priority);
      val1_2 = Math.Max(val1_2, argBuilder.Priority);
    }
    Expression[] args1 = new Expression[this.ArgBuilders.Count];
    Expression[] args2 = (Expression[]) null;
    usageMarkers = new bool[restrictedArgs.Length];
    for (int index1 = val1_1; index1 <= val1_2; ++index1)
    {
      for (int index2 = 0; index2 < this.ArgBuilders.Count; ++index2)
      {
        if (this.ArgBuilders[index2].Priority == index1)
        {
          args1[index2] = this.ArgBuilders[index2].ToExpression(this.Resolver, restrictedArgs, usageMarkers);
          Expression byRefArgument = this.ArgBuilders[index2].ByRefArgument;
          if (byRefArgument != null)
          {
            if (args2 == null)
              args2 = new Expression[this.ArgBuilders.Count];
            args2[index2] = byRefArgument;
          }
        }
      }
    }
    if (args2 != null)
    {
      for (int index = 0; index < args1.Length; ++index)
      {
        if (args1[index] != null && args2[index] == null)
        {
          args2[index] = (Expression) this.Resolver.GetTemporary(args1[index].Type, (string) null);
          args1[index] = (Expression) Expression.Assign(args2[index], args1[index]);
        }
      }
      spilledArgs = MethodCandidate.RemoveNulls(args1);
      return MethodCandidate.RemoveNulls(args2);
    }
    spilledArgs = (Expression[]) null;
    return MethodCandidate.RemoveNulls(args1);
  }

  private static Expression[] RemoveNulls(Expression[] args)
  {
    int length = args.Length;
    for (int index = 0; index < args.Length; ++index)
    {
      if (args[index] == null)
        --length;
    }
    Expression[] expressionArray = new Expression[length];
    int index1 = 0;
    int num = 0;
    for (; index1 < args.Length; ++index1)
    {
      if (args[index1] != null)
        expressionArray[num++] = args[index1];
    }
    return expressionArray;
  }

  public override string ToString()
  {
    return $"MethodCandidate({this.Overload.ReflectionInfo} on {this.Overload.DeclaringType.FullName})";
  }
}
