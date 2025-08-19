// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LightExceptionConvertingExpression
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Ast;

internal class LightExceptionConvertingExpression : Expression, ILightExceptionAwareExpression
{
  private readonly Expression _expr;
  private readonly bool _supportsLightEx;

  internal LightExceptionConvertingExpression(Expression expr, bool supportsLightEx)
  {
    this._expr = expr;
    this._supportsLightEx = supportsLightEx;
  }

  public override bool CanReduce => true;

  public override ExpressionType NodeType => ExpressionType.Extension;

  public override Type Type => this._expr.Type;

  public override Expression Reduce() => new LightExceptionRewriter().Rewrite(this._expr);

  Expression ILightExceptionAwareExpression.ReduceForLightExceptions()
  {
    return this._supportsLightEx ? this._expr : this.Reduce();
  }

  protected override Expression VisitChildren(ExpressionVisitor visitor)
  {
    Expression expr = visitor.Visit(this._expr);
    return expr != this._expr ? (Expression) new LightExceptionConvertingExpression(expr, this._supportsLightEx) : (Expression) this;
  }
}
