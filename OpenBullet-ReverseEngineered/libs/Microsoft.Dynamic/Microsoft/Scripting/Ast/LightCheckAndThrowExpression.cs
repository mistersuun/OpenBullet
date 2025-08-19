// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LightCheckAndThrowExpression
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Ast;

internal class LightCheckAndThrowExpression : Expression, ILightExceptionAwareExpression
{
  private readonly Expression _expr;

  internal LightCheckAndThrowExpression(Expression instance) => this._expr = instance;

  public override bool CanReduce => true;

  public override ExpressionType NodeType => ExpressionType.Extension;

  public override Type Type => this._expr.Type;

  public override Expression Reduce()
  {
    return Utils.Convert((Expression) Expression.Call(LightExceptions._checkAndThrow, this._expr), this._expr.Type);
  }

  Expression ILightExceptionAwareExpression.ReduceForLightExceptions() => this._expr;

  protected override Expression VisitChildren(ExpressionVisitor visitor)
  {
    Expression instance = visitor.Visit(this._expr);
    return instance != this._expr ? (Expression) new LightCheckAndThrowExpression(instance) : (Expression) this;
  }
}
