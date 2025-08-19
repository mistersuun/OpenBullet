// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.MemberExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Compiler.Ast;

public class MemberExpression : Expression
{
  private readonly Expression _target;
  private readonly string _name;

  public MemberExpression(Expression target, string name)
  {
    this._target = target;
    this._name = name;
  }

  public Expression Target => this._target;

  public string Name => this._name;

  public override string ToString() => $"{base.ToString()}:{this._name}";

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this.GlobalParent.Get(this._name, (System.Linq.Expressions.Expression) this._target);
  }

  internal override System.Linq.Expressions.Expression TransformSet(
    SourceSpan span,
    System.Linq.Expressions.Expression right,
    PythonOperationKind op)
  {
    if (op == PythonOperationKind.None)
      return this.GlobalParent.AddDebugInfoAndVoid(this.GlobalParent.Set(this._name, (System.Linq.Expressions.Expression) this._target, right), span);
    ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Variable(typeof (object), "inplace");
    return this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      parameterExpression
    }, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) parameterExpression, (System.Linq.Expressions.Expression) this._target), this.SetMemberOperator(right, op, parameterExpression), (System.Linq.Expressions.Expression) Utils.Empty()), this.Span.Start, span.End);
  }

  internal override string CheckAssign()
  {
    return string.Compare(this._name, "None") == 0 ? "cannot assign to None" : (string) null;
  }

  internal override string CheckDelete() => (string) null;

  private System.Linq.Expressions.Expression SetMemberOperator(
    System.Linq.Expressions.Expression right,
    PythonOperationKind op,
    ParameterExpression temp)
  {
    return this.GlobalParent.Set(this._name, (System.Linq.Expressions.Expression) temp, this.GlobalParent.Operation(typeof (object), op, this.GlobalParent.Get(this._name, (System.Linq.Expressions.Expression) temp), right));
  }

  internal override System.Linq.Expressions.Expression TransformDelete()
  {
    return this.GlobalParent.Delete(typeof (void), this._name, (System.Linq.Expressions.Expression) this._target);
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this) && this._target != null)
      this._target.Walk(walker);
    walker.PostWalk(this);
  }
}
