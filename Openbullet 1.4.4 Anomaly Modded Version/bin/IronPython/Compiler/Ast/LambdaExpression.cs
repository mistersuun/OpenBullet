// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.LambdaExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler.Ast;

public class LambdaExpression : Expression
{
  private readonly FunctionDefinition _function;

  public LambdaExpression(FunctionDefinition function) => this._function = function;

  public FunctionDefinition Function => this._function;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this._function.MakeFunctionExpression();
  }

  public override string NodeName => "lambda";

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this._function != null)
      this._function.Walk(walker);
    walker.PostWalk(this);
  }
}
