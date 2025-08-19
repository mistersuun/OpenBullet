// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.DelayedTupleAssign
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler;

internal sealed class DelayedTupleAssign : Expression
{
  private readonly Expression _lhs;
  private readonly Expression _rhs;

  public DelayedTupleAssign(Expression lhs, Expression rhs)
  {
    this._lhs = lhs;
    this._rhs = rhs;
  }

  public override Expression Reduce()
  {
    return (Expression) Expression.Assign(this._lhs.Reduce(), this._rhs);
  }

  public sealed override ExpressionType NodeType => ExpressionType.Extension;

  public sealed override Type Type => this._lhs.Type;

  public override bool CanReduce => true;

  protected override Expression VisitChildren(ExpressionVisitor visitor)
  {
    Expression rhs = visitor.Visit(this._rhs);
    return rhs != this._rhs ? (Expression) new DelayedTupleAssign(this._lhs, rhs) : (Expression) this;
  }
}
