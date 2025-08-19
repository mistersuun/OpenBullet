// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.FinallyFlowControlExpression
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Interpreter;
using System;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Ast;

public sealed class FinallyFlowControlExpression : Expression, IInstructionProvider
{
  private readonly Expression _body;
  private Expression _reduced;

  internal FinallyFlowControlExpression(Expression body) => this._body = body;

  public override bool CanReduce => true;

  public sealed override Type Type => this.Body.Type;

  public sealed override ExpressionType NodeType => ExpressionType.Extension;

  public Expression Body => this._body;

  public override Expression Reduce()
  {
    if (this._reduced == null)
      this._reduced = new FlowControlRewriter().Reduce(this._body);
    return this._reduced;
  }

  protected override Expression VisitChildren(ExpressionVisitor visitor)
  {
    Expression body = visitor.Visit(this._body);
    return body == this._body ? (Expression) this : (Expression) new FinallyFlowControlExpression(body);
  }

  void IInstructionProvider.AddInstructions(LightCompiler compiler) => compiler.Compile(this._body);
}
