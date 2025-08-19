// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.DelayedTupleExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler;

internal sealed class DelayedTupleExpression : Expression
{
  public readonly int Index;
  private readonly StrongBox<Type> _tupleType;
  private readonly StrongBox<ParameterExpression> _tupleExpr;
  private readonly Type _type;

  public DelayedTupleExpression(
    int index,
    StrongBox<ParameterExpression> tupleExpr,
    StrongBox<Type> tupleType,
    Type type)
  {
    this.Index = index;
    this._tupleType = tupleType;
    this._tupleExpr = tupleExpr;
    this._type = type;
  }

  public override Expression Reduce()
  {
    Expression expression = (Expression) this._tupleExpr.Value;
    foreach (PropertyInfo property in MutableTuple.GetAccessPath(this._tupleType.Value, this.Index))
      expression = (Expression) Expression.Property(expression, property);
    return expression;
  }

  public sealed override ExpressionType NodeType => ExpressionType.Extension;

  public sealed override Type Type => this._type;

  public override bool CanReduce => true;

  protected override Expression VisitChildren(ExpressionVisitor visitor) => (Expression) this;
}
