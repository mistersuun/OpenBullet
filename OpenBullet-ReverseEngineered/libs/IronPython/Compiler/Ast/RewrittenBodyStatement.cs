// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.RewrittenBodyStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Compiler.Ast;

internal class RewrittenBodyStatement : Statement
{
  private readonly System.Linq.Expressions.Expression _body;
  private readonly string _doc;
  private readonly Statement _originalBody;

  public RewrittenBodyStatement(Statement originalBody, System.Linq.Expressions.Expression body)
  {
    this._body = body;
    this._doc = originalBody.Documentation;
    this._originalBody = originalBody;
    this.SetLoc(originalBody.GlobalParent, originalBody.IndexSpan);
  }

  public override System.Linq.Expressions.Expression Reduce() => this._body;

  public override string Documentation => this._doc;

  public override void Walk(PythonWalker walker) => this._originalBody.Walk(walker);
}
