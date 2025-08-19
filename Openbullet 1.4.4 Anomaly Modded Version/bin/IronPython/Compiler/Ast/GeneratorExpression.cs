// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.GeneratorExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler.Ast;

public class GeneratorExpression : Expression
{
  private readonly FunctionDefinition _function;
  private readonly Expression _iterable;

  public GeneratorExpression(FunctionDefinition function, Expression iterable)
  {
    this._function = function;
    this._iterable = iterable;
  }

  public override System.Linq.Expressions.Expression Reduce()
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeGeneratorExpression, this._function.MakeFunctionExpression(), (System.Linq.Expressions.Expression) this._iterable);
  }

  public FunctionDefinition Function => this._function;

  public Expression Iterable => this._iterable;

  public override string NodeName => "generator expression";

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      this._function.Walk(walker);
      this._iterable.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
