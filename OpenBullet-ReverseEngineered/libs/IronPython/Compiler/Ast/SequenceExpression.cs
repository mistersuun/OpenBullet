// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.SequenceExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using Microsoft.Scripting;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler.Ast;

public abstract class SequenceExpression : Expression
{
  private readonly Expression[] _items;

  protected SequenceExpression(Expression[] items) => this._items = items;

  public IList<Expression> Items => (IList<Expression>) this._items;

  internal override System.Linq.Expressions.Expression TransformSet(
    SourceSpan span,
    System.Linq.Expressions.Expression right,
    PythonOperationKind op)
  {
    bool flag = false;
    foreach (Expression expr in this._items)
    {
      if (SequenceExpression.IsComplexAssignment(expr))
      {
        flag = true;
        break;
      }
    }
    SourceSpan span1 = SourceSpan.None;
    SourceLocation start = this.Span.Start;
    SourceSpan location1 = !start.IsValid || !span.IsValid ? SourceSpan.None : new SourceSpan(this.Span.Start, span.End);
    SourceSpan location2 = SourceSpan.None;
    if (flag)
    {
      span1 = span;
      location1 = SourceSpan.None;
      SourceSpan span2 = this.Span;
      start = span2.Start;
      SourceSpan sourceSpan;
      if (!start.IsValid || !span.IsValid)
      {
        sourceSpan = SourceSpan.None;
      }
      else
      {
        span2 = this.Span;
        sourceSpan = new SourceSpan(span2.Start, span.End);
      }
      location2 = sourceSpan;
    }
    ParameterExpression variable = System.Linq.Expressions.Expression.Variable(typeof (object), "unpacking");
    System.Linq.Expressions.Expression expression1 = Node.MakeAssignment(variable, right);
    System.Linq.Expressions.Expression right1 = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Convert(LightExceptions.CheckAndThrow((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(flag ? AstMethods.GetEnumeratorValues : AstMethods.GetEnumeratorValuesNoComplexSets, this.Parent.LocalContext, (System.Linq.Expressions.Expression) variable, Utils.Constant((object) this._items.Length))), typeof (object[]));
    ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Variable(typeof (object[]), "array");
    System.Linq.Expressions.Expression expression2 = this.MakeAssignment(parameterExpression, right1, span1);
    ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression> collectionBuilder = new ReadOnlyCollectionBuilder<System.Linq.Expressions.Expression>(this._items.Length + 1);
    for (int index = 0; index < this._items.Length; ++index)
    {
      Expression expression3 = this._items[index];
      if (expression3 != null)
      {
        System.Linq.Expressions.Expression right2 = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.ArrayAccess((System.Linq.Expressions.Expression) parameterExpression, Utils.Constant((object) index));
        System.Linq.Expressions.Expression expression4 = expression3.TransformSet(flag ? expression3.Span : SourceSpan.None, right2, PythonOperationKind.None);
        collectionBuilder.Add(expression4);
      }
    }
    collectionBuilder.Add((System.Linq.Expressions.Expression) Utils.Empty());
    System.Linq.Expressions.Expression expression5 = this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<System.Linq.Expressions.Expression>) collectionBuilder.ToReadOnlyCollection()), location1);
    return this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[2]
    {
      parameterExpression,
      variable
    }, expression1, expression2, expression5, (System.Linq.Expressions.Expression) Utils.Empty()), location2);
  }

  internal override string CheckAssign()
  {
    foreach (Expression expression in this._items)
    {
      string str = expression.CheckAssign();
      if (str != null)
        return str;
    }
    return (string) null;
  }

  internal override string CheckDelete() => (string) null;

  internal override string CheckAugmentedAssign()
  {
    return this.CheckAssign() ?? "illegal expression for augmented assignment";
  }

  private static bool IsComplexAssignment(Expression expr) => !(expr is NameExpression);

  internal override System.Linq.Expressions.Expression TransformDelete()
  {
    System.Linq.Expressions.Expression[] expressionArray = new System.Linq.Expressions.Expression[this._items.Length + 1];
    for (int index = 0; index < this._items.Length; ++index)
      expressionArray[index] = this._items[index].TransformDelete();
    expressionArray[this._items.Length] = (System.Linq.Expressions.Expression) Utils.Empty();
    return this.GlobalParent.AddDebugInfo((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block(expressionArray), this.Span);
  }

  internal override bool CanThrow
  {
    get
    {
      foreach (Node node in this._items)
      {
        if (node.CanThrow)
          return true;
      }
      return false;
    }
  }
}
