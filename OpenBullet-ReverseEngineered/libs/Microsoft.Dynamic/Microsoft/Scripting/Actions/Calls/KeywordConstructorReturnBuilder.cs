// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.KeywordConstructorReturnBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

internal sealed class KeywordConstructorReturnBuilder : ReturnBuilder
{
  private readonly ReturnBuilder _builder;
  private readonly int _kwArgCount;
  private readonly int[] _indexesUsed;
  private readonly MemberInfo[] _membersSet;
  private readonly bool _privateBinding;

  public KeywordConstructorReturnBuilder(
    ReturnBuilder builder,
    int kwArgCount,
    int[] indexesUsed,
    MemberInfo[] membersSet,
    bool privateBinding)
    : base(builder.ReturnType)
  {
    this._builder = builder;
    this._kwArgCount = kwArgCount;
    this._indexesUsed = indexesUsed;
    this._membersSet = membersSet;
    this._privateBinding = privateBinding;
  }

  internal override Expression ToExpression(
    OverloadResolver resolver,
    IList<ArgBuilder> builders,
    RestrictedArguments args,
    Expression ret)
  {
    List<Expression> expressionList = new List<Expression>();
    ParameterExpression temporary = resolver.GetTemporary(ret.Type, "val");
    expressionList.Add((Expression) Expression.Assign((Expression) temporary, ret));
    for (int index = 0; index < this._indexesUsed.Length; ++index)
    {
      Expression expression = args.GetObject(args.Length - this._kwArgCount + this._indexesUsed[index]).Expression;
      FieldInfo members1;
      if ((members1 = this._membersSet[index] as FieldInfo) != (FieldInfo) null)
      {
        if (!members1.IsLiteral && !members1.IsInitOnly)
          expressionList.Add((Expression) Expression.Assign((Expression) Expression.Field((Expression) temporary, members1), KeywordConstructorReturnBuilder.ConvertToHelper(resolver, expression, members1.FieldType)));
        else
          expressionList.Add((Expression) Expression.Convert((Expression) Expression.Call(typeof (ScriptingRuntimeHelpers).GetMethod("ReadOnlyAssignError"), Utils.Constant((object) true), Utils.Constant((object) members1.Name)), members1.FieldType));
      }
      else
      {
        PropertyInfo members2;
        if ((members2 = this._membersSet[index] as PropertyInfo) != (PropertyInfo) null)
        {
          if (members2.GetSetMethod(this._privateBinding) != (MethodInfo) null)
            expressionList.Add((Expression) Expression.Assign((Expression) Expression.Property((Expression) temporary, members2), KeywordConstructorReturnBuilder.ConvertToHelper(resolver, expression, members2.PropertyType)));
          else
            expressionList.Add((Expression) Expression.Convert((Expression) Expression.Call(typeof (ScriptingRuntimeHelpers).GetMethod("ReadOnlyAssignError"), Utils.Constant((object) false), Utils.Constant((object) members2.Name)), members2.PropertyType));
        }
      }
    }
    expressionList.Add((Expression) temporary);
    Expression ret1 = (Expression) Expression.Block(expressionList.ToArray());
    return this._builder.ToExpression(resolver, builders, args, ret1);
  }

  private static Expression ConvertToHelper(OverloadResolver resolver, Expression value, Type type)
  {
    if (type == value.Type)
      return value;
    return type.IsAssignableFrom(value.Type) ? Utils.Convert(value, type) : resolver.GetDynamicConversion(value, type);
  }
}
