// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ComprehensionFor
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler.Ast;

public class ComprehensionFor : ComprehensionIterator
{
  private readonly Expression _lhs;
  private readonly Expression _list;

  public ComprehensionFor(Expression lhs, Expression list)
  {
    this._lhs = lhs;
    this._list = list;
  }

  public Expression Left => this._lhs;

  public Expression List => this._list;

  internal override System.Linq.Expressions.Expression Transform(System.Linq.Expressions.Expression body)
  {
    ParameterExpression enumerator = System.Linq.Expressions.Expression.Parameter(typeof (KeyValuePair<IEnumerator, IDisposable>), "list_comprehension_for");
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      enumerator
    }, ForStatement.TransformFor(this.Parent, enumerator, this._list, this._lhs, body, (Statement) null, this.Span, this.GlobalParent.IndexToLocation(this._lhs.EndIndex), (LabelTarget) null, (LabelTarget) null, false));
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._lhs != null)
        this._lhs.Walk(walker);
      if (this._list != null)
        this._list.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
