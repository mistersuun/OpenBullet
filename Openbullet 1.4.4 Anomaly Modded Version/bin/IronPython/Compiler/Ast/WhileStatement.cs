// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.WhileStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Interpreter;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler.Ast;

public class WhileStatement : Statement, ILoopStatement, IInstructionProvider
{
  private int _indexHeader;
  private readonly Expression _test;
  private readonly Statement _body;
  private readonly Statement _else;
  private LabelTarget _break;
  private LabelTarget _continue;

  public WhileStatement(Expression test, Statement body, Statement else_)
  {
    this._test = test;
    this._body = body;
    this._else = else_;
  }

  public Expression Test => this._test;

  public Statement Body => this._body;

  public Statement ElseStatement => this._else;

  private SourceSpan Header
  {
    get
    {
      return new SourceSpan(this.GlobalParent.IndexToLocation(this.StartIndex), this.GlobalParent.IndexToLocation(this._indexHeader));
    }
  }

  public void SetLoc(PythonAst globalParent, int start, int header, int end)
  {
    this.SetLoc(globalParent, start, end);
    this._indexHeader = header;
  }

  LabelTarget ILoopStatement.BreakLabel
  {
    get => this._break;
    set => this._break = value;
  }

  LabelTarget ILoopStatement.ContinueLabel
  {
    get => this._continue;
    set => this._continue = value;
  }

  public override System.Linq.Expressions.Expression Reduce() => this.ReduceWorker(true);

  void IInstructionProvider.AddInstructions(LightCompiler compiler)
  {
    compiler.Compile(this.ReduceWorker(false));
  }

  private System.Linq.Expressions.Expression ReduceWorker(bool optimizeDynamicConvert)
  {
    if (!(this._test is ConstantExpression test) || !(test.Value is int))
      return (System.Linq.Expressions.Expression) Utils.While(this.GlobalParent.AddDebugInfo(optimizeDynamicConvert ? this.TransformAndDynamicConvert(this._test, typeof (bool)) : this.GlobalParent.Convert(typeof (bool), ConversionResultKind.ExplicitCast, (System.Linq.Expressions.Expression) this._test), this.Header), (System.Linq.Expressions.Expression) this._body, (System.Linq.Expressions.Expression) this._else, this._break, this._continue);
    if ((int) test.Value == 0)
      return this._else == null ? (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Empty() : (System.Linq.Expressions.Expression) this._else;
    System.Linq.Expressions.Expression expression = (System.Linq.Expressions.Expression) Utils.While((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) true), (System.Linq.Expressions.Expression) this._body, (System.Linq.Expressions.Expression) this._else, this._break, this._continue);
    if (this.GlobalParent.IndexToLocation(this._test.StartIndex).Line != this.GlobalParent.IndexToLocation(this._body.StartIndex).Line)
      expression = this.GlobalParent.AddDebugInfoAndVoid(expression, this._test.Span);
    return expression;
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._test != null)
        this._test.Walk(walker);
      if (this._body != null)
        this._body.Walk(walker);
      if (this._else != null)
        this._else.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
