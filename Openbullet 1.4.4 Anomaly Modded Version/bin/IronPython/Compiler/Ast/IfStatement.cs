// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.IfStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Interpreter;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler.Ast;

public class IfStatement : Statement, IInstructionProvider
{
  private readonly IfStatementTest[] _tests;
  private readonly Statement _else;

  public IfStatement(IfStatementTest[] tests, Statement else_)
  {
    this._tests = tests;
    this._else = else_;
  }

  public IList<IfStatementTest> Tests => (IList<IfStatementTest>) this._tests;

  public Statement ElseStatement => this._else;

  public override System.Linq.Expressions.Expression Reduce() => this.ReduceWorker(true);

  void IInstructionProvider.AddInstructions(LightCompiler compiler)
  {
    compiler.Compile(this.ReduceWorker(false));
  }

  private System.Linq.Expressions.Expression ReduceWorker(bool optimizeDynamicConvert)
  {
    System.Linq.Expressions.Expression ifFalse;
    if (this._tests.Length > 100)
    {
      BlockBuilder blockBuilder = new BlockBuilder();
      LabelTarget target = System.Linq.Expressions.Expression.Label();
      for (int index = 0; index < this._tests.Length; ++index)
      {
        IfStatementTest test = this._tests[index];
        blockBuilder.Add((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Condition(optimizeDynamicConvert ? this.TransformAndDynamicConvert(test.Test, typeof (bool)) : this.GlobalParent.Convert(typeof (bool), ConversionResultKind.ExplicitCast, (System.Linq.Expressions.Expression) test.Test), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(Node.TransformMaybeSingleLineSuite(test.Body, this.GlobalParent.IndexToLocation(test.Test.StartIndex)), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Goto(target)), (System.Linq.Expressions.Expression) Utils.Empty()));
      }
      if (this._else != null)
        blockBuilder.Add((System.Linq.Expressions.Expression) this._else);
      blockBuilder.Add((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Label(target));
      ifFalse = blockBuilder.ToExpression();
    }
    else
    {
      ifFalse = this._else == null ? (System.Linq.Expressions.Expression) Utils.Empty() : (System.Linq.Expressions.Expression) this._else;
      int length = this._tests.Length;
      while (length-- > 0)
      {
        IfStatementTest test = this._tests[length];
        ifFalse = this.GlobalParent.AddDebugInfoAndVoid((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Condition(optimizeDynamicConvert ? this.TransformAndDynamicConvert(test.Test, typeof (bool)) : this.GlobalParent.Convert(typeof (bool), ConversionResultKind.ExplicitCast, (System.Linq.Expressions.Expression) test.Test), Node.TransformMaybeSingleLineSuite(test.Body, this.GlobalParent.IndexToLocation(test.Test.StartIndex)), ifFalse), new SourceSpan(this.GlobalParent.IndexToLocation(test.StartIndex), this.GlobalParent.IndexToLocation(test.HeaderIndex)));
      }
    }
    return ifFalse;
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._tests != null)
      {
        foreach (Node test in this._tests)
          test.Walk(walker);
      }
      if (this._else != null)
        this._else.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
