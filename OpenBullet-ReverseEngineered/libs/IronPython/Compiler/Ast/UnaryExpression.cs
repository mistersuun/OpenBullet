// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.UnaryExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;

#nullable disable
namespace IronPython.Compiler.Ast;

public class UnaryExpression : Expression
{
  private readonly Expression _expression;
  private readonly PythonOperator _op;

  public UnaryExpression(PythonOperator op, Expression expression)
  {
    this._op = op;
    this._expression = expression;
    this.EndIndex = expression.EndIndex;
  }

  public Expression Expression => this._expression;

  public PythonOperator Op => this._op;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this.GlobalParent.Operation(typeof (object), UnaryExpression.PythonOperatorToOperatorString(this._op), (System.Linq.Expressions.Expression) this._expression);
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this._expression != null)
      this._expression.Walk(walker);
    walker.PostWalk(this);
  }

  private static PythonOperationKind PythonOperatorToOperatorString(PythonOperator op)
  {
    switch (op)
    {
      case PythonOperator.Not:
        return PythonOperationKind.Not;
      case PythonOperator.Pos:
        return PythonOperationKind.Positive;
      case PythonOperator.Invert:
        return PythonOperationKind.OnesComplement;
      case PythonOperator.Negate:
        return PythonOperationKind.Negate;
      default:
        return PythonOperationKind.None;
    }
  }
}
