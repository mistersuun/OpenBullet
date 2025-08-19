// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.ExpressionCollectionBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Ast;

public class ExpressionCollectionBuilder : ExpressionCollectionBuilder<Expression>
{
  public Expression ToMethodCall(Expression instance, MethodInfo method)
  {
    switch (this.Count)
    {
      case 0:
        return (Expression) Expression.Call(instance, method);
      case 1:
        if (instance == null)
          return (Expression) Expression.Call(method, this.Expression0);
        return (Expression) Expression.Call(instance, method, (IEnumerable<Expression>) new ReadOnlyCollectionBuilder<Expression>()
        {
          this.Expression0
        });
      case 2:
        return (Expression) Expression.Call(instance, method, this.Expression0, this.Expression1);
      case 3:
        return (Expression) Expression.Call(instance, method, this.Expression0, this.Expression1, this.Expression2);
      case 4:
        if (instance == null)
          return (Expression) Expression.Call(method, this.Expression0, this.Expression1, this.Expression2, this.Expression3);
        return (Expression) Expression.Call(instance, method, (IEnumerable<Expression>) new ReadOnlyCollectionBuilder<Expression>()
        {
          this.Expression0,
          this.Expression1,
          this.Expression2,
          this.Expression3
        });
      default:
        return (Expression) Expression.Call(instance, method, (IEnumerable<Expression>) this.Expressions);
    }
  }
}
