// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.YieldExpression
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Ast;

public sealed class YieldExpression : Expression
{
  private readonly Expression _value;

  internal YieldExpression(LabelTarget target, Expression value, int yieldMarker)
  {
    this.Target = target;
    this._value = value;
    this.YieldMarker = yieldMarker;
  }

  public override bool CanReduce => false;

  public sealed override Type Type => typeof (void);

  public sealed override ExpressionType NodeType => ExpressionType.Extension;

  public Expression Value => this._value;

  public LabelTarget Target { get; }

  public int YieldMarker { get; }

  protected override Expression VisitChildren(ExpressionVisitor visitor)
  {
    Expression expression = visitor.Visit(this._value);
    return expression == this._value ? (Expression) this : (Expression) Utils.MakeYield(this.Target, expression, this.YieldMarker);
  }
}
