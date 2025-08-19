// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.ReferenceArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

internal sealed class ReferenceArgBuilder : SimpleArgBuilder
{
  private readonly Type _elementType;
  private ParameterExpression _tmp;

  public ReferenceArgBuilder(ParameterInfo info, Type elementType, Type strongBox, int index)
    : base(info, strongBox, index, false, false)
  {
    this._elementType = elementType;
  }

  protected override SimpleArgBuilder Copy(int newIndex)
  {
    return (SimpleArgBuilder) new ReferenceArgBuilder(this.ParameterInfo, this._elementType, this.Type, newIndex);
  }

  public override ArgBuilder Clone(ParameterInfo newType)
  {
    Type elementType = newType.ParameterType.GetElementType();
    return (ArgBuilder) new ReferenceArgBuilder(newType, elementType, typeof (StrongBox<>).MakeGenericType(elementType), this.Index);
  }

  public override int Priority => 5;

  protected internal override Expression ToExpression(
    OverloadResolver resolver,
    RestrictedArguments args,
    bool[] hasBeenUsed)
  {
    if (this._tmp == null)
      this._tmp = resolver.GetTemporary(this._elementType, "outParam");
    hasBeenUsed[this.Index] = true;
    Expression expression = args.GetObject(this.Index).Expression;
    return (Expression) Expression.Condition((Expression) Expression.TypeIs(expression, this.Type), (Expression) Expression.Assign((Expression) this._tmp, (Expression) Expression.Field(Microsoft.Scripting.Ast.Utils.Convert(expression, this.Type), this.Type.GetDeclaredField("Value"))), (Expression) Expression.Throw((Expression) Expression.Call(RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Func<Type, object, Exception>(ScriptingRuntimeHelpers.MakeIncorrectBoxTypeError)), Microsoft.Scripting.Ast.Utils.Constant((object) this._elementType), Microsoft.Scripting.Ast.Utils.Convert(expression, typeof (object))), this._elementType));
  }

  internal override Expression UpdateFromReturn(OverloadResolver resolver, RestrictedArguments args)
  {
    return (Expression) Expression.Assign((Expression) Expression.Field((Expression) Expression.Convert(args.GetObject(this.Index).Expression, this.Type), this.Type.GetDeclaredField("Value")), (Expression) this._tmp);
  }

  internal override Expression ByRefArgument => (Expression) this._tmp;
}
