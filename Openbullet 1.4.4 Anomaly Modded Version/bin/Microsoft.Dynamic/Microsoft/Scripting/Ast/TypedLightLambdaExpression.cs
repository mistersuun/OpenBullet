// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.TypedLightLambdaExpression
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Ast;

internal class TypedLightLambdaExpression : LightLambdaExpression
{
  private readonly Type _delegateType;

  internal TypedLightLambdaExpression(
    Type retType,
    Type delegateType,
    Expression body,
    string name,
    IList<ParameterExpression> args)
    : base(retType, body, name, args)
  {
    this._delegateType = delegateType;
  }

  internal override LambdaExpression ReduceToLambdaWorker()
  {
    return Expression.Lambda(this._delegateType, this.Body, this.Name, (IEnumerable<ParameterExpression>) this.Parameters);
  }

  public override Type Type => this._delegateType;
}
