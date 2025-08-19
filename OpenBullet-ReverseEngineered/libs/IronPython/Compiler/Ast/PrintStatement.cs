// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.PrintStatement
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Ast;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

public class PrintStatement : Statement
{
  private readonly Expression _dest;
  private readonly Expression[] _expressions;
  private readonly bool _trailingComma;

  public PrintStatement(Expression destination, Expression[] expressions, bool trailingComma)
  {
    this._dest = destination;
    this._expressions = expressions;
    this._trailingComma = trailingComma;
  }

  public Expression Destination => this._dest;

  public IList<Expression> Expressions => (IList<Expression>) this._expressions;

  public bool TrailingComma => this._trailingComma;

  public override System.Linq.Expressions.Expression Reduce()
  {
    System.Linq.Expressions.Expression right = (System.Linq.Expressions.Expression) this._dest;
    if (this._expressions.Length == 0)
      return this.GlobalParent.AddDebugInfo(right == null ? (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.PrintNewline, this.Parent.LocalContext) : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(AstMethods.PrintNewlineWithDest, this.Parent.LocalContext, right), this.Span);
    ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression> collectionBuilder = new ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression>();
    ParameterExpression variable = (ParameterExpression) null;
    if (right != null)
    {
      variable = System.Linq.Expressions.Expression.Variable(typeof (object), "destination");
      collectionBuilder.Add(Node.MakeAssignment(variable, right));
      right = (System.Linq.Expressions.Expression) variable;
    }
    for (int index = 0; index < this._expressions.Length; ++index)
    {
      bool flag = index < this._expressions.Length - 1 || this._trailingComma;
      Expression expression = this._expressions[index];
      MethodCallExpression methodCallExpression = right == null ? System.Linq.Expressions.Expression.Call(flag ? AstMethods.PrintComma : AstMethods.Print, this.Parent.LocalContext, Utils.Convert((System.Linq.Expressions.Expression) expression, typeof (object))) : System.Linq.Expressions.Expression.Call(flag ? AstMethods.PrintCommaWithDest : AstMethods.PrintWithDest, this.Parent.LocalContext, right, Utils.Convert((System.Linq.Expressions.Expression) expression, typeof (object)));
      collectionBuilder.Add((System.Linq.Expressions.Expression) methodCallExpression);
    }
    collectionBuilder.Add((System.Linq.Expressions.Expression) Utils.Empty());
    System.Linq.Expressions.Expression expression1;
    if (variable != null)
      expression1 = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        variable
      }, (IEnumerable<System.Linq.Expressions.Expression>) collectionBuilder.ToReadOnlyCollection());
    else
      expression1 = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<System.Linq.Expressions.Expression>) collectionBuilder.ToReadOnlyCollection());
    return this.GlobalParent.AddDebugInfo(expression1, this.Span);
  }

  public override void Walk(PythonWalker walker)
  {
    if (walker.Walk(this))
    {
      if (this._dest != null)
        this._dest.Walk(walker);
      if (this._expressions != null)
      {
        foreach (Node expression in this._expressions)
          expression.Walk(walker);
      }
    }
    walker.PostWalk(this);
  }
}
