// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.AndExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler.Ast;

public class AndExpression : Expression
{
  private readonly Expression _left;
  private readonly Expression _right;

  public AndExpression(Expression left, Expression right)
  {
    ContractUtils.RequiresNotNull((object) left, nameof (left));
    ContractUtils.RequiresNotNull((object) right, nameof (right));
    this._left = left;
    this._right = right;
    this.StartIndex = left.StartIndex;
    this.EndIndex = right.EndIndex;
  }

  public Expression Left => this._left;

  public Expression Right => this._right;

  public override System.Linq.Expressions.Expression Reduce()
  {
    System.Linq.Expressions.Expression left = (System.Linq.Expressions.Expression) this._left;
    System.Linq.Expressions.Expression right = (System.Linq.Expressions.Expression) this._right;
    Type type = this.Type;
    ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Variable(type, "__all__");
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      parameterExpression
    }, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Condition(this.GlobalParent.Convert(typeof (bool), ConversionResultKind.ExplicitCast, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) parameterExpression, Microsoft.Scripting.Ast.Utils.Convert(left, type))), Microsoft.Scripting.Ast.Utils.Convert(right, type), (System.Linq.Expressions.Expression) parameterExpression));
  }

  public override Type Type
  {
    get => !(this._left.Type == this._right.Type) ? typeof (object) : this._left.Type;
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._left != null)
        this._left.Walk(walker);
      if (this._right != null)
        this._right.Walk(walker);
    }
    walker.PostWalk(this);
  }

  internal override bool CanThrow => this._left.CanThrow || this._right.CanThrow;
}
