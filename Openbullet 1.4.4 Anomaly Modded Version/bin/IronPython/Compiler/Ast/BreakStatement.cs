// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.BreakStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler.Ast;

public class BreakStatement : Statement
{
  private ILoopStatement _loop;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Break(this._loop.BreakLabel), this.Span);
  }

  public override void Walk(PythonWalker walker)
  {
    walker.Walk(this);
    walker.PostWalk(this);
  }

  internal override bool CanThrow => false;

  internal ILoopStatement LoopStatement
  {
    get => this._loop;
    set => this._loop = value;
  }
}
