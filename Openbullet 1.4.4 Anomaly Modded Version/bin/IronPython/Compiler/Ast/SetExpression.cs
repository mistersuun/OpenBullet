// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.SetExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Compiler.Ast;

public class SetExpression : Expression
{
  private readonly Expression[] _items;

  public SetExpression(params Expression[] items)
  {
    ContractUtils.RequiresNotNull((object) items, nameof (items));
    this._items = items;
  }

  public IList<Expression> Items => (IList<Expression>) this._items;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeSet, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NewArrayInit(typeof (object), ArrayUtils.ConvertAll<Expression, System.Linq.Expressions.Expression>(this._items, (Func<Expression, System.Linq.Expressions.Expression>) (x => Microsoft.Scripting.Ast.Utils.Convert((System.Linq.Expressions.Expression) x, typeof (object))))));
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      foreach (Node node in this._items)
        node.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
