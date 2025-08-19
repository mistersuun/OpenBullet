// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ExpressionStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler.Ast;

public class ExpressionStatement : Statement
{
  private readonly Expression _expression;

  public ExpressionStatement(Expression expression) => this._expression = expression;

  public Expression Expression => this._expression;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this.ReduceWorker((System.Linq.Expressions.Expression) this._expression);
  }

  private System.Linq.Expressions.Expression ReduceWorker(System.Linq.Expressions.Expression expression)
  {
    if (this.Parent.PrintExpressions)
      expression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.PrintExpressionValue, this.Parent.LocalContext, Node.ConvertIfNeeded(expression, typeof (object)));
    return this.GlobalParent.AddDebugInfoAndVoid(expression, this._expression.Span);
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this._expression != null)
      this._expression.Walk(walker);
    walker.PostWalk(this);
  }

  public override string Documentation
  {
    get
    {
      return this._expression is ConstantExpression expression ? expression.Value as string : (string) null;
    }
  }

  internal override bool CanThrow => this._expression.CanThrow;
}
