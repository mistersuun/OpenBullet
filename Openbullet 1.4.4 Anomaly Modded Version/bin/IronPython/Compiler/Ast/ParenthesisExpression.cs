// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ParenthesisExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using Microsoft.Scripting;
using System;

#nullable disable
namespace IronPython.Compiler.Ast;

public class ParenthesisExpression : Expression
{
  private readonly Expression _expression;

  public ParenthesisExpression(Expression expression) => this._expression = expression;

  public Expression Expression => this._expression;

  public override System.Linq.Expressions.Expression Reduce() => (System.Linq.Expressions.Expression) this._expression;

  internal override System.Linq.Expressions.Expression TransformSet(
    SourceSpan span,
    System.Linq.Expressions.Expression right,
    PythonOperationKind op)
  {
    return this._expression.TransformSet(span, right, op);
  }

  internal override string CheckAssign() => this._expression.CheckAssign();

  internal override string CheckAugmentedAssign() => this._expression.CheckAugmentedAssign();

  internal override string CheckDelete() => this._expression.CheckDelete();

  internal override System.Linq.Expressions.Expression TransformDelete()
  {
    return this._expression.TransformDelete();
  }

  public override Type Type => this._expression.Type;

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this._expression != null)
      this._expression.Walk(walker);
    walker.PostWalk(this);
  }

  internal override bool CanThrow => this._expression.CanThrow;
}
