// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.ParamsDictArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

internal sealed class ParamsDictArgBuilder : ArgBuilder
{
  private readonly string[] _names;
  private readonly int[] _nameIndexes;
  private readonly int _argIndex;

  public ParamsDictArgBuilder(ParameterInfo info, int argIndex, string[] names, int[] nameIndexes)
    : base(info)
  {
    this._argIndex = argIndex;
    this._names = names;
    this._nameIndexes = nameIndexes;
  }

  public override int ConsumedArgumentCount => -1;

  public override int Priority => 3;

  protected internal override Expression ToExpression(
    OverloadResolver resolver,
    RestrictedArguments args,
    bool[] hasBeenUsed)
  {
    return (Expression) Expression.Call(RuntimeReflectionExtensions.GetMethodInfo((Delegate) this.GetCreationDelegate(this.ParameterInfo.ParameterType)), (Expression) Expression.NewArrayInit(typeof (string), this.ConstantNames()), (Expression) Microsoft.Scripting.Ast.Utils.NewArrayHelper(typeof (object), (IEnumerable<Expression>) this.GetParameters(args, hasBeenUsed)));
  }

  private static InvalidOperationException BadDictionaryType(Type dictType)
  {
    return new InvalidOperationException("Unsupported param dictionary type: " + dictType.FullName);
  }

  public override Type Type => this.ParameterInfo.ParameterType;

  private List<Expression> GetParameters(RestrictedArguments args, bool[] hasBeenUsed)
  {
    List<Expression> parameters = new List<Expression>(this._nameIndexes.Length);
    for (int index = 0; index < this._nameIndexes.Length; ++index)
    {
      int i = this._nameIndexes[index] + this._argIndex;
      if (!hasBeenUsed[i])
      {
        parameters.Add(args.GetObject(i).Expression);
        hasBeenUsed[i] = true;
      }
    }
    return parameters;
  }

  private int[] GetParameters(bool[] hasBeenUsed)
  {
    List<int> intList = new List<int>(this._nameIndexes.Length);
    for (int index1 = 0; index1 < this._nameIndexes.Length; ++index1)
    {
      int index2 = this._nameIndexes[index1] + this._argIndex;
      if (!hasBeenUsed[index2])
      {
        intList.Add(index2);
        hasBeenUsed[index2] = true;
      }
    }
    return intList.ToArray();
  }

  private Expression[] ConstantNames()
  {
    Expression[] expressionArray = new Expression[this._names.Length];
    for (int index = 0; index < this._names.Length; ++index)
      expressionArray[index] = Microsoft.Scripting.Ast.Utils.Constant((object) this._names[index]);
    return expressionArray;
  }

  private Func<string[], object[], object> GetCreationDelegate(Type dictType)
  {
    Func<string[], object[], object> func = (Func<string[], object[], object>) null;
    if (dictType == typeof (IDictionary))
      func = new Func<string[], object[], object>(BinderOps.MakeDictionary<object, object>);
    else if (dictType.IsGenericType())
    {
      Type[] genericTypeArguments = dictType.GetGenericTypeArguments();
      if ((dictType.GetGenericTypeDefinition() == typeof (IDictionary<,>) || dictType.GetGenericTypeDefinition() == typeof (Dictionary<,>)) && (genericTypeArguments[0] == typeof (string) || genericTypeArguments[0] == typeof (object)))
        func = (Func<string[], object[], object>) typeof (BinderOps).GetMethod("MakeDictionary").MakeGenericMethod(genericTypeArguments).CreateDelegate(typeof (Func<string[], object[], object>));
    }
    return func != null ? func : throw ParamsDictArgBuilder.BadDictionaryType(dictType);
  }
}
