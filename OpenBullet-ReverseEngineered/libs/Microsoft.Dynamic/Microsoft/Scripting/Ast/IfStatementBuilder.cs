// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.IfStatementBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Ast;

public sealed class IfStatementBuilder
{
  private readonly List<IfStatementTest> _clauses = new List<IfStatementTest>();

  internal IfStatementBuilder()
  {
  }

  public IfStatementBuilder ElseIf(Expression test, params Expression[] body)
  {
    ContractUtils.RequiresNotNullItems<Expression>((IList<Expression>) body, nameof (body));
    return this.ElseIf(test, (Expression) Microsoft.Scripting.Ast.Utils.BlockVoid(body));
  }

  public IfStatementBuilder ElseIf(Expression test, Expression body)
  {
    ContractUtils.RequiresNotNull((object) test, nameof (test));
    ContractUtils.Requires(test.Type == typeof (bool), nameof (test));
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    this._clauses.Add(Microsoft.Scripting.Ast.Utils.IfCondition(test, body));
    return this;
  }

  public Expression Else(params Expression[] body)
  {
    ContractUtils.RequiresNotNullItems<Expression>((IList<Expression>) body, nameof (body));
    return this.Else((Expression) Microsoft.Scripting.Ast.Utils.BlockVoid(body));
  }

  public Expression Else(Expression body)
  {
    ContractUtils.RequiresNotNull((object) body, nameof (body));
    return IfStatementBuilder.BuildConditions((IList<IfStatementTest>) this._clauses, body);
  }

  internal static Expression BuildConditions(IList<IfStatementTest> clauses, Expression @else)
  {
    Expression ifFalse = @else ?? (Expression) Microsoft.Scripting.Ast.Utils.Empty();
    int count = clauses.Count;
    while (count-- > 0)
    {
      IfStatementTest clause = clauses[count];
      ifFalse = (Expression) Expression.IfThenElse(clause.Test, clause.Body, ifFalse);
    }
    return ifFalse;
  }

  public Expression ToStatement()
  {
    return IfStatementBuilder.BuildConditions((IList<IfStatementTest>) this._clauses, (Expression) null);
  }

  public static implicit operator Expression(IfStatementBuilder builder)
  {
    ContractUtils.RequiresNotNull((object) builder, nameof (builder));
    return builder.ToStatement();
  }
}
