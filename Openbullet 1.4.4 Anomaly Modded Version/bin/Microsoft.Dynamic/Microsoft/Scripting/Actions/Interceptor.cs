// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Interceptor
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Actions;

public static class Interceptor
{
  public static Expression Intercept(Expression expression)
  {
    return ((ExpressionVisitor) new Interceptor.InterceptorWalker()).Visit(expression);
  }

  public static LambdaExpression Intercept(LambdaExpression lambda)
  {
    return ((ExpressionVisitor) new Interceptor.InterceptorWalker()).Visit((Expression) lambda) as LambdaExpression;
  }

  internal class InterceptorSiteBinder : CallSiteBinder
  {
    private readonly CallSiteBinder _binder;

    internal InterceptorSiteBinder(CallSiteBinder binder) => this._binder = binder;

    public override int GetHashCode() => this._binder.GetHashCode();

    public override bool Equals(object obj) => obj != null && obj.Equals((object) this._binder);

    public override Expression Bind(
      object[] args,
      ReadOnlyCollection<ParameterExpression> parameters,
      LabelTarget returnLabel)
    {
      return Interceptor.Intercept(this._binder.Bind(args, parameters, returnLabel));
    }
  }

  internal class InterceptorWalker : DynamicExpressionVisitor
  {
    protected virtual Expression VisitDynamic(DynamicExpression node)
    {
      CallSiteBinder binder1 = node.Binder;
      if (binder1 is Interceptor.InterceptorSiteBinder)
        return (Expression) node;
      CallSiteBinder binder2 = (CallSiteBinder) new Interceptor.InterceptorSiteBinder(binder1);
      return (Expression) Expression.MakeDynamic(node.DelegateType, binder2, (IEnumerable<Expression>) node.Arguments);
    }
  }
}
