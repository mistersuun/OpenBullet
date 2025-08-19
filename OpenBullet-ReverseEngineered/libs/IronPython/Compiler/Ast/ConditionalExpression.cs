// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ConditionalExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Ast;

#nullable disable
namespace IronPython.Compiler.Ast;

public class ConditionalExpression : Expression
{
  private readonly Expression _testExpr;
  private readonly Expression _trueExpr;
  private readonly Expression _falseExpr;

  public ConditionalExpression(
    Expression testExpression,
    Expression trueExpression,
    Expression falseExpression)
  {
    this._testExpr = testExpression;
    this._trueExpr = trueExpression;
    this._falseExpr = falseExpression;
  }

  public Expression FalseExpression => this._falseExpr;

  public Expression Test => this._testExpr;

  public Expression TrueExpression => this._trueExpr;

  public override string NodeName => "conditional expression";

  public override System.Linq.Expressions.Expression Reduce()
  {
    System.Linq.Expressions.Expression ifTrue = Utils.Convert((System.Linq.Expressions.Expression) this._trueExpr, typeof (object));
    System.Linq.Expressions.Expression ifFalse = Utils.Convert((System.Linq.Expressions.Expression) this._falseExpr, typeof (object));
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Condition(this.GlobalParent.Convert(typeof (bool), ConversionResultKind.ExplicitCast, (System.Linq.Expressions.Expression) this._testExpr), ifTrue, ifFalse);
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._testExpr != null)
        this._testExpr.Walk(walker);
      if (this._trueExpr != null)
        this._trueExpr.Walk(walker);
      if (this._falseExpr != null)
        this._falseExpr.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
