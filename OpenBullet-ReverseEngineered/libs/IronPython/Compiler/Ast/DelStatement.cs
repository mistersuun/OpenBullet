// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.DelStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Ast;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

public class DelStatement : Statement
{
  private readonly Expression[] _expressions;

  public DelStatement(Expression[] expressions) => this._expressions = expressions;

  public IList<Expression> Expressions => (IList<Expression>) this._expressions;

  public override System.Linq.Expressions.Expression Reduce()
  {
    ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression> collectionBuilder = new ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression>(this._expressions.Length + 1);
    for (int index = 0; index < this._expressions.Length; ++index)
      collectionBuilder.Add(this._expressions[index].TransformDelete());
    collectionBuilder.Add((System.Linq.Expressions.Expression) Utils.Empty());
    return this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<System.Linq.Expressions.Expression>) collectionBuilder), this.Span);
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this._expressions != null)
    {
      foreach (Node expression in this._expressions)
        expression.Walk(walker);
    }
    walker.PostWalk(this);
  }

  internal override bool CanThrow
  {
    get
    {
      foreach (Node expression in this._expressions)
      {
        if (expression.CanThrow)
          return true;
      }
      return false;
    }
  }
}
