// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ReturnStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace IronPython.Compiler.Ast;

public class ReturnStatement : Statement
{
  private readonly Expression _expression;

  public ReturnStatement(Expression expression) => this._expression = expression;

  public Expression Expression => this._expression;

  public override System.Linq.Expressions.Expression Reduce()
  {
    if (!this.Parent.IsGeneratorMethod)
      return this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Return(FunctionDefinition._returnLabel, Node.TransformOrConstantNull(this._expression, typeof (object))), this.Span);
    return this._expression != null ? (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Throw((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.New(typeof (InvalidOperationException).GetConstructor(ReflectionUtils.EmptyTypes))) : this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) Microsoft.Scripting.Ast.Utils.YieldBreak(Node.GeneratorLabel), this.Span);
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this._expression != null)
      this._expression.Walk(walker);
    walker.PostWalk(this);
  }

  internal override bool CanThrow => this._expression != null && this._expression.CanThrow;
}
