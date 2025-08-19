// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.AssignmentStatement
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

public class AssignmentStatement : Statement
{
  private readonly Expression[] _left;
  private readonly Expression _right;

  public AssignmentStatement(Expression[] left, Expression right)
  {
    this._left = left;
    this._right = right;
  }

  public IList<Expression> Left => (IList<Expression>) this._left;

  public Expression Right => this._right;

  public override System.Linq.Expressions.Expression Reduce()
  {
    return this._left.Length == 1 ? this.AssignOne() : this.AssignComplex((System.Linq.Expressions.Expression) this._right);
  }

  private System.Linq.Expressions.Expression AssignComplex(System.Linq.Expressions.Expression right)
  {
    List<System.Linq.Expressions.Expression> expressionList = new List<System.Linq.Expressions.Expression>();
    ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Variable(typeof (object), "assignment");
    expressionList.Add(Node.MakeAssignment(parameterExpression, right));
    foreach (Expression expression1 in this._left)
    {
      if (expression1 != null)
      {
        System.Linq.Expressions.Expression expression2 = expression1.TransformSet(this.Span, (System.Linq.Expressions.Expression) parameterExpression, PythonOperationKind.None);
        expressionList.Add(expression2);
      }
    }
    expressionList.Add((System.Linq.Expressions.Expression) Utils.Empty());
    return this.GlobalParent.AddDebugInfoAndVoid((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      parameterExpression
    }, expressionList.ToArray()), this.Span);
  }

  private System.Linq.Expressions.Expression AssignOne()
  {
    SequenceExpression sequenceExpression = this._left[0] as SequenceExpression;
    SequenceExpression right1 = this._right as SequenceExpression;
    if (sequenceExpression == null || right1 == null || sequenceExpression.Items.Count != right1.Items.Count)
      return this._left[0].TransformSet(this.Span, (System.Linq.Expressions.Expression) this._right, PythonOperationKind.None);
    int count = sequenceExpression.Items.Count;
    ParameterExpression[] variables = new ParameterExpression[count];
    System.Linq.Expressions.Expression[] expressionArray = new System.Linq.Expressions.Expression[count * 2 + 1];
    for (int index = 0; index < count; ++index)
    {
      System.Linq.Expressions.Expression right2 = (System.Linq.Expressions.Expression) right1.Items[index];
      variables[index] = System.Linq.Expressions.Expression.Variable(right2.Type, "parallelAssign");
      expressionArray[index] = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) variables[index], right2);
    }
    for (int index = 0; index < count; ++index)
      expressionArray[index + count] = sequenceExpression.Items[index].TransformSet(SourceSpan.None, (System.Linq.Expressions.Expression) variables[index], PythonOperationKind.None);
    expressionArray[count * 2] = (System.Linq.Expressions.Expression) Utils.Empty();
    return this.GlobalParent.AddDebugInfoAndVoid((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) variables, expressionArray), this.Span);
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      foreach (Node node in this._left)
        node.Walk(walker);
      this._right.Walk(walker);
    }
    walker.PostWalk(this);
  }
}
