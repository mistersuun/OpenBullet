// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.IndexExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Binding;
using Microsoft.Scripting;
using Microsoft.Scripting.Utils;

#nullable disable
namespace IronPython.Compiler.Ast;

public class IndexExpression : Expression
{
  private readonly Expression _target;
  private readonly Expression _index;

  public IndexExpression(Expression target, Expression index)
  {
    this._target = target;
    this._index = index;
  }

  public Expression Target => this._target;

  public Expression Index => this._index;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this.IsSlice ? this.GlobalParent.GetSlice(this.GetActionArgumentsForGetOrDelete()) : this.GlobalParent.GetIndex(this.GetActionArgumentsForGetOrDelete());
  }

  private System.Linq.Expressions.Expression[] GetActionArgumentsForGetOrDelete()
  {
    if (this._index is TupleExpression index1 && index1.IsExpandable)
      return (System.Linq.Expressions.Expression[]) ArrayUtils.Insert<Expression>(this._target, index1.Items);
    return this._index is SliceExpression index2 ? (index2.StepProvided ? new System.Linq.Expressions.Expression[4]
    {
      (System.Linq.Expressions.Expression) this._target,
      IndexExpression.GetSliceValue(index2.SliceStart),
      IndexExpression.GetSliceValue(index2.SliceStop),
      IndexExpression.GetSliceValue(index2.SliceStep)
    } : new System.Linq.Expressions.Expression[3]
    {
      (System.Linq.Expressions.Expression) this._target,
      IndexExpression.GetSliceValue(index2.SliceStart),
      IndexExpression.GetSliceValue(index2.SliceStop)
    }) : (System.Linq.Expressions.Expression[]) new Expression[2]
    {
      this._target,
      this._index
    };
  }

  private static System.Linq.Expressions.Expression GetSliceValue(Expression expr)
  {
    return expr != null ? (System.Linq.Expressions.Expression) expr : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Field((System.Linq.Expressions.Expression) null, typeof (MissingParameter).GetField("Value"));
  }

  private System.Linq.Expressions.Expression[] GetActionArgumentsForSet(System.Linq.Expressions.Expression right)
  {
    return ArrayUtils.Append<System.Linq.Expressions.Expression>(this.GetActionArgumentsForGetOrDelete(), right);
  }

  internal override System.Linq.Expressions.Expression TransformSet(
    SourceSpan span,
    System.Linq.Expressions.Expression right,
    PythonOperationKind op)
  {
    if (op != PythonOperationKind.None)
      right = this.GlobalParent.Operation(typeof (object), op, (System.Linq.Expressions.Expression) this, right);
    return this.GlobalParent.AddDebugInfoAndVoid(!this.IsSlice ? this.GlobalParent.SetIndex(this.GetActionArgumentsForSet(right)) : this.GlobalParent.SetSlice(this.GetActionArgumentsForSet(right)), this.Span);
  }

  internal override System.Linq.Expressions.Expression TransformDelete()
  {
    return this.GlobalParent.AddDebugInfoAndVoid(!this.IsSlice ? this.GlobalParent.DeleteIndex(this.GetActionArgumentsForGetOrDelete()) : this.GlobalParent.DeleteSlice(this.GetActionArgumentsForGetOrDelete()), this.Span);
  }

  internal override string CheckAssign() => (string) null;

  internal override string CheckDelete() => (string) null;

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._target != null)
        this._target.Walk(walker);
      if (this._index != null)
        this._index.Walk(walker);
    }
    walker.PostWalk(this);
  }

  private bool IsSlice => this._index is SliceExpression;
}
