// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.YieldExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;

#nullable disable
namespace IronPython.Compiler.Ast;

public class YieldExpression : Expression
{
  private readonly Expression _expression;

  public YieldExpression(Expression expression) => this._expression = expression;

  public Expression Expression => this._expression;

  internal static System.Linq.Expressions.Expression CreateCheckThrowExpression(SourceSpan span)
  {
    System.Linq.Expressions.Expression generatorParam = (System.Linq.Expressions.Expression) IronPython.Compiler.GeneratorRewriter._generatorParam;
    return LightExceptions.CheckAndThrow((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.GeneratorCheckThrowableAndReturnSendValue, generatorParam));
  }

  public override System.Linq.Expressions.Expression Reduce()
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((System.Linq.Expressions.Expression) Utils.YieldReturn(Node.GeneratorLabel, Utils.Convert((System.Linq.Expressions.Expression) this._expression, typeof (object))), YieldExpression.CreateCheckThrowExpression(this.Span));
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this._expression != null)
      this._expression.Walk(walker);
    walker.PostWalk(this);
  }

  public override string NodeName => "yield expression";
}
