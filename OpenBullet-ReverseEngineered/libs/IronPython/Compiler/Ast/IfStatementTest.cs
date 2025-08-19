// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.IfStatementTest
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;

#nullable disable
namespace IronPython.Compiler.Ast;

public class IfStatementTest : Node
{
  private int _headerIndex;
  private readonly Expression _test;
  private Statement _body;

  public IfStatementTest(Expression test, Statement body)
  {
    this._test = test;
    this._body = body;
  }

  public SourceLocation Header => this.GlobalParent.IndexToLocation(this._headerIndex);

  public int HeaderIndex
  {
    set => this._headerIndex = value;
    get => this._headerIndex;
  }

  public Expression Test => this._test;

  public Statement Body
  {
    get => this._body;
    set => this._body = value;
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._test != null)
        this._test.Walk(walker);
      if (this._body != null)
        this._body.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
