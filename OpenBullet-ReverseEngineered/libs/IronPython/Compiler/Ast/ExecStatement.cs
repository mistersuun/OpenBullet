// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ExecStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using Microsoft.Scripting.Ast;

#nullable disable
namespace IronPython.Compiler.Ast;

public class ExecStatement : Statement
{
  private readonly Expression _code;
  private readonly Expression _locals;
  private readonly Expression _globals;

  public ExecStatement(Expression code, Expression locals, Expression globals)
  {
    this._code = code;
    this._locals = locals;
    this._globals = globals;
  }

  public Expression Code => this._code;

  public Expression Locals => this._locals;

  public Expression Globals => this._globals;

  public bool NeedsLocalsDictionary() => this._globals == null && this._locals == null;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this.GlobalParent.AddDebugInfo(this._locals != null || this._globals != null ? (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.QualifiedExec, this.Parent.LocalContext, Utils.Convert((System.Linq.Expressions.Expression) this._code, typeof (object)), this.TransformAndDynamicConvert(this._globals, typeof (PythonDictionary)), Node.TransformOrConstantNull(this._locals, typeof (object))) : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.UnqualifiedExec, this.Parent.LocalContext, Utils.Convert((System.Linq.Expressions.Expression) this._code, typeof (object))), this.Span);
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._code != null)
        this._code.Walk(walker);
      if (this._locals != null)
        this._locals.Walk(walker);
      if (this._globals != null)
        this._globals.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
