// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.BackQuoteExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Ast;

#nullable disable
namespace IronPython.Compiler.Ast;

public class BackQuoteExpression : Expression
{
  private readonly Expression _expression;

  public BackQuoteExpression(Expression expression) => this._expression = expression;

  public Expression Expression => this._expression;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.Repr, this.Parent.LocalContext, Utils.Convert((System.Linq.Expressions.Expression) this._expression, typeof (object)));
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this._expression != null)
      this._expression.Walk(walker);
    walker.PostWalk(this);
  }
}
