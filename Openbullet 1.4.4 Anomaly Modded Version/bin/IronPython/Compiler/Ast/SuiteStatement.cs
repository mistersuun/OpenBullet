// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.SuiteStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Ast;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

public sealed class SuiteStatement : Statement
{
  private readonly Statement[] _statements;

  public SuiteStatement(Statement[] statements) => this._statements = statements;

  public IList<Statement> Statements => (IList<Statement>) this._statements;

  public override System.Linq.Expressions.Expression Reduce()
  {
    if (this._statements.Length == 0)
      return this.GlobalParent.AddDebugInfoAndVoid((System.Linq.Expressions.Expression) Utils.Empty(), this.Span);
    ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression> collectionBuilder = new ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression>();
    int line1 = -1;
    foreach (Statement statement in this._statements)
    {
      int line2 = this.GlobalParent.IndexToLocation(statement.StartIndex).Line;
      if (line2 == line1)
      {
        collectionBuilder.Add((System.Linq.Expressions.Expression) new SuiteStatement.DebugInfoRemovalExpression((System.Linq.Expressions.Expression) statement, line1));
      }
      else
      {
        if (statement.CanThrow && line2 != -1)
          collectionBuilder.Add(Node.UpdateLineNumber(line2));
        collectionBuilder.Add((System.Linq.Expressions.Expression) statement);
      }
      line1 = line2;
    }
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<System.Linq.Expressions.Expression>) collectionBuilder.ToReadOnlyCollection());
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this._statements != null)
    {
      foreach (Node statement in this._statements)
        statement.Walk(walker);
    }
    walker.PostWalk(this);
  }

  public override string Documentation
  {
    get => this._statements.Length != 0 ? this._statements[0].Documentation : (string) null;
  }

  internal override bool CanThrow
  {
    get
    {
      foreach (Node statement in this._statements)
      {
        if (statement.CanThrow)
          return true;
      }
      return false;
    }
  }

  internal class DebugInfoRemovalExpression : System.Linq.Expressions.Expression
  {
    private System.Linq.Expressions.Expression _inner;
    private int _start;

    public DebugInfoRemovalExpression(System.Linq.Expressions.Expression expression, int line)
    {
      this._inner = expression;
      this._start = line;
    }

    public override System.Linq.Expressions.Expression Reduce()
    {
      return Node.RemoveDebugInfo(this._start, this._inner.Reduce());
    }

    public override ExpressionType NodeType => ExpressionType.Extension;

    public override Type Type => this._inner.Type;

    public override bool CanReduce => true;
  }
}
