// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.Ast.Comprehension
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace IronPython.Compiler.Ast;

public abstract class Comprehension : Expression
{
  public abstract IList<ComprehensionIterator> Iterators { get; }

  public abstract override string NodeName { get; }

  protected abstract ParameterExpression MakeParameter();

  protected abstract MethodInfo Factory();

  protected abstract System.Linq.Expressions.Expression Body(ParameterExpression res);

  public abstract override void Walk(PythonWalker walker);

  public override System.Linq.Expressions.Expression Reduce()
  {
    ParameterExpression parameterExpression = this.MakeParameter();
    System.Linq.Expressions.Expression expression = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) parameterExpression, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(this.Factory()));
    System.Linq.Expressions.Expression body = this.Body(parameterExpression);
    for (int index = this.Iterators.Count - 1; index >= 0; --index)
      body = this.Iterators[index].Transform(body);
    return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      parameterExpression
    }, expression, body, (System.Linq.Expressions.Expression) parameterExpression);
  }
}
