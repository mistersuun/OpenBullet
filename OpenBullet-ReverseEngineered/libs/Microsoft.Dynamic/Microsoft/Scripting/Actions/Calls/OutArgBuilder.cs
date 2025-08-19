// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.OutArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

internal sealed class OutArgBuilder : ArgBuilder
{
  private readonly Type _parameterType;
  private readonly bool _isRef;
  private ParameterExpression _tmp;

  public OutArgBuilder(ParameterInfo info)
    : base(info)
  {
    this._parameterType = info.ParameterType.IsByRef ? info.ParameterType.GetElementType() : info.ParameterType;
    this._isRef = info.ParameterType.IsByRef;
  }

  public override int ConsumedArgumentCount => 0;

  public override int Priority => 5;

  protected internal override Expression ToExpression(
    OverloadResolver resolver,
    RestrictedArguments args,
    bool[] hasBeenUsed)
  {
    return this._isRef ? (Expression) this._tmp ?? (Expression) (this._tmp = resolver.GetTemporary(this._parameterType, "outParam")) : this.GetDefaultValue();
  }

  internal override Expression ToReturnExpression(OverloadResolver resolver)
  {
    return this._isRef ? (Expression) this._tmp : this.GetDefaultValue();
  }

  internal override Expression ByRefArgument
  {
    get => !this._isRef ? (Expression) null : (Expression) this._tmp;
  }

  private Expression GetDefaultValue()
  {
    return this._parameterType.IsValueType() ? Microsoft.Scripting.Ast.Utils.Constant(Activator.CreateInstance(this._parameterType)) : Microsoft.Scripting.Ast.Utils.Constant((object) null);
  }
}
