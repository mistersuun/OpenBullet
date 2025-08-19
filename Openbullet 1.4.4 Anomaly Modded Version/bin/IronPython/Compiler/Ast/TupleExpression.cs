// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.TupleExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Compiler.Ast;

public class TupleExpression : SequenceExpression
{
  private bool _expandable;

  public TupleExpression(bool expandable, params Expression[] items)
    : base(items)
  {
    this._expandable = expandable;
  }

  internal override string CheckAssign()
  {
    if (this.Items.Count == 0)
      return "can't assign to ()";
    for (int index = 0; index < this.Items.Count; ++index)
    {
      Expression expression = this.Items[index];
      if (expression.CheckAssign() != null)
        return "can't assign to " + expression.NodeName;
    }
    return (string) null;
  }

  internal override string CheckDelete()
  {
    return this.Items.Count == 0 ? "can't delete ()" : base.CheckDelete();
  }

  public override System.Linq.Expressions.Expression Reduce()
  {
    if (this._expandable)
      return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NewArrayInit(typeof (object), Node.ToObjectArray(this.Items));
    return this.Items.Count == 0 ? (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, typeof (PythonOps).GetField("EmptyTuple")) : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.MakeTuple, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NewArrayInit(typeof (object), Node.ToObjectArray(this.Items)));
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

  public bool IsExpandable => this._expandable;

  internal override bool IsConstant
  {
    get
    {
      foreach (Expression expression in (IEnumerable<Expression>) this.Items)
      {
        if (!expression.IsConstant)
          return false;
      }
      return true;
    }
  }

  internal override object GetConstantValue()
  {
    if (this.Items.Count == 0)
      return (object) PythonTuple.EMPTY;
    object[] objArray = new object[this.Items.Count];
    for (int index = 0; index < objArray.Length; ++index)
      objArray[index] = this.Items[index].GetConstantValue();
    return (object) PythonOps.MakeTuple(objArray);
  }
}
