// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ListComprehension
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using Microsoft.Scripting.Ast;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace IronPython.Compiler.Ast;

public sealed class ListComprehension : Comprehension
{
  private readonly ComprehensionIterator[] _iterators;
  private readonly Expression _item;

  public ListComprehension(Expression item, ComprehensionIterator[] iterators)
  {
    this._item = item;
    this._iterators = iterators;
  }

  public Expression Item => this._item;

  public override IList<ComprehensionIterator> Iterators
  {
    get => (IList<ComprehensionIterator>) this._iterators;
  }

  protected override ParameterExpression MakeParameter()
  {
    return System.Linq.Expressions.Expression.Parameter(typeof (List), "list_comprehension_list");
  }

  protected override MethodInfo Factory() => AstMethods.MakeList;

  protected override System.Linq.Expressions.Expression Body(ParameterExpression res)
  {
    return this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.ListAddForComprehension, (System.Linq.Expressions.Expression) res, Utils.Convert((System.Linq.Expressions.Expression) this._item, typeof (object))), this._item.Span);
  }

  public override string NodeName => "list comprehension";

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._item != null)
        this._item.Walk(walker);
      if (this._iterators != null)
      {
        foreach (Node iterator in this._iterators)
          iterator.Walk(walker);
      }
    }
    walker.PostWalk(this);
  }
}
