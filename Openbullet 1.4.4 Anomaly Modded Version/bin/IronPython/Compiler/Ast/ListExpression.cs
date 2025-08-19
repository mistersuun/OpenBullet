// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.ListExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections.Generic;

#nullable disable
namespace IronPython.Compiler.Ast;

public class ListExpression(params Expression[] items) : SequenceExpression(items)
{
  public override System.Linq.Expressions.Expression Reduce()
  {
    return this.Items.Count == 0 ? (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeEmptyListFromCode, Node.EmptyExpression) : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeListNoCopy, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NewArrayInit(typeof (object), Node.ToObjectArray(this.Items)));
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this.Items != null)
    {
      foreach (Node node in (IEnumerable<Expression>) this.Items)
        node.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
