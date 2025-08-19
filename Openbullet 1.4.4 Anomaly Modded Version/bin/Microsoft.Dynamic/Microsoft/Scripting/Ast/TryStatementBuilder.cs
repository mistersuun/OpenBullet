// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.TryStatementBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Ast;

public sealed class TryStatementBuilder
{
  private readonly List<CatchBlock> _catchBlocks = new List<CatchBlock>();
  private Expression _try;
  private Expression _finally;
  private Expression _fault;
  private bool _enableJumpsFromFinally;

  internal TryStatementBuilder(Expression body) => this._try = body;

  public TryStatementBuilder Catch(Type type, Expression body)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    if (this._finally != null)
      throw Microsoft.Scripting.Error.FinallyAlreadyDefined();
    this._catchBlocks.Add(Expression.Catch(type, body));
    return this;
  }

  public TryStatementBuilder Catch(Type type, Expression expr0, Expression expr1)
  {
    return this.Catch(type, (Expression) Expression.Block(expr0, expr1));
  }

  public TryStatementBuilder Catch(
    Type type,
    Expression expr0,
    Expression expr1,
    Expression expr2)
  {
    return this.Catch(type, (Expression) Expression.Block(expr0, expr1, expr2));
  }

  public TryStatementBuilder Catch(
    Type type,
    Expression expr0,
    Expression expr1,
    Expression expr2,
    Expression expr3)
  {
    return this.Catch(type, (Expression) Expression.Block(expr0, expr1, expr2, expr3));
  }

  public TryStatementBuilder Catch(Type type, params Expression[] body)
  {
    return this.Catch(type, (Expression) Expression.Block(body));
  }

  public TryStatementBuilder Catch(ParameterExpression holder, Expression expr0, Expression expr1)
  {
    return this.Catch(holder, (Expression) Expression.Block(expr0, expr1));
  }

  public TryStatementBuilder Catch(
    ParameterExpression holder,
    Expression expr0,
    Expression expr1,
    Expression expr2)
  {
    return this.Catch(holder, (Expression) Expression.Block(expr0, expr1, expr2));
  }

  public TryStatementBuilder Catch(
    ParameterExpression holder,
    Expression expr0,
    Expression expr1,
    Expression expr2,
    Expression expr3)
  {
    return this.Catch(holder, (Expression) Expression.Block(expr0, expr1, expr2, expr3));
  }

  public TryStatementBuilder Catch(ParameterExpression holder, params Expression[] body)
  {
    return this.Catch(holder, (Expression) Microsoft.Scripting.Ast.Utils.Block(body));
  }

  public TryStatementBuilder Catch(ParameterExpression holder, Expression body)
  {
    ContractUtils.RequiresNotNull((object) holder, nameof (holder));
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    if (this._finally != null)
      throw Microsoft.Scripting.Error.FinallyAlreadyDefined();
    this._catchBlocks.Add(Expression.Catch(holder, body));
    return this;
  }

  public TryStatementBuilder Filter(Type type, Expression condition, params Expression[] body)
  {
    return this.Filter(type, condition, (Expression) Microsoft.Scripting.Ast.Utils.Block(body));
  }

  public TryStatementBuilder Filter(Type type, Expression condition, Expression body)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    ContractUtils.RequiresNotNull((object) condition, nameof (condition));
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    this._catchBlocks.Add(Expression.Catch(type, body, condition));
    return this;
  }

  public TryStatementBuilder Filter(
    ParameterExpression holder,
    Expression condition,
    params Expression[] body)
  {
    return this.Filter(holder, condition, (Expression) Microsoft.Scripting.Ast.Utils.Block(body));
  }

  public TryStatementBuilder Filter(
    ParameterExpression holder,
    Expression condition,
    Expression body)
  {
    ContractUtils.RequiresNotNull((object) holder, nameof (holder));
    ContractUtils.RequiresNotNull((object) condition, nameof (condition));
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    this._catchBlocks.Add(Expression.Catch(holder, body, condition));
    return this;
  }

  public TryStatementBuilder Finally(params Expression[] body)
  {
    return this.Finally((Expression) Microsoft.Scripting.Ast.Utils.BlockVoid(body));
  }

  public TryStatementBuilder Finally(Expression body)
  {
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    if (this._finally != null)
      throw Microsoft.Scripting.Error.FinallyAlreadyDefined();
    if (this._fault != null)
      throw Microsoft.Scripting.Error.CannotHaveFaultAndFinally();
    this._finally = body;
    return this;
  }

  public TryStatementBuilder FinallyWithJumps(params Expression[] body)
  {
    this._enableJumpsFromFinally = true;
    return this.Finally(body);
  }

  public TryStatementBuilder FinallyWithJumps(Expression body)
  {
    this._enableJumpsFromFinally = true;
    return this.Finally(body);
  }

  public TryStatementBuilder Fault(params Expression[] body)
  {
    ContractUtils.RequiresNotNullItems<Expression>((IList<Expression>) body, nameof (body));
    if (this._finally != null)
      throw Microsoft.Scripting.Error.CannotHaveFaultAndFinally();
    if (this._fault != null)
      throw Microsoft.Scripting.Error.FaultAlreadyDefined();
    this._fault = body.Length == 1 ? body[0] : (Expression) Microsoft.Scripting.Ast.Utils.BlockVoid(body);
    return this;
  }

  public static implicit operator Expression(TryStatementBuilder builder)
  {
    ContractUtils.RequiresNotNull((object) builder, nameof (builder));
    return builder.ToExpression();
  }

  public Expression ToExpression()
  {
    List<CatchBlock> handlers = new List<CatchBlock>((IEnumerable<CatchBlock>) this._catchBlocks);
    int index = 0;
    for (int count = handlers.Count; index < count; ++index)
    {
      CatchBlock catchBlock = handlers[index];
      if (catchBlock.Filter != null)
        handlers[index] = Expression.MakeCatchBlock(catchBlock.Test, catchBlock.Variable, (Expression) Expression.Condition(catchBlock.Filter, catchBlock.Body, (Expression) Expression.Rethrow(catchBlock.Body.Type)), (Expression) null);
    }
    if (this._fault != null)
    {
      ContractUtils.Requires(handlers.Count == 0, "fault cannot be used with catch or finally clauses");
      handlers.Add(Expression.Catch(typeof (Exception), (Expression) Expression.Block(this._fault, (Expression) Expression.Rethrow(this._try.Type))));
    }
    TryExpression body = Expression.MakeTry((Type) null, this._try, this._finally, (Expression) null, (IEnumerable<CatchBlock>) handlers);
    return this._enableJumpsFromFinally ? Microsoft.Scripting.Ast.Utils.FinallyFlowControl((Expression) body) : (Expression) body;
  }
}
