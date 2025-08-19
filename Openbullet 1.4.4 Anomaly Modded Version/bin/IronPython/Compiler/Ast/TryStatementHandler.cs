// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.TryStatementHandler
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;

#nullable disable
namespace IronPython.Compiler.Ast;

public class TryStatementHandler : Node
{
  private int _headerIndex;
  private readonly Expression _test;
  private readonly Expression _target;
  private readonly Statement _body;

  public TryStatementHandler(Expression test, Expression target, Statement body)
  {
    this._test = test;
    this._target = target;
    this._body = body;
  }

  public SourceLocation Header => this.GlobalParent.IndexToLocation(this._headerIndex);

  public int HeaderIndex
  {
    get => this._headerIndex;
    set => this._headerIndex = value;
  }

  public Expression Test => this._test;

  public Expression Target => this._target;

  public Statement Body => this._body;

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._test != null)
        this._test.Walk(walker);
      if (this._target != null)
        this._target.Walk(walker);
      if (this._body != null)
        this._body.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
