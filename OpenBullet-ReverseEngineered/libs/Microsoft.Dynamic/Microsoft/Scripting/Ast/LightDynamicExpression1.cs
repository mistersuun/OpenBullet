// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LightDynamicExpression1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Ast;

public class LightDynamicExpression1 : LightDynamicExpression, ILightExceptionAwareExpression
{
  internal readonly Expression _arg0;

  protected internal LightDynamicExpression1(CallSiteBinder binder, Expression arg0)
    : base(binder)
  {
    ContractUtils.RequiresNotNull((object) arg0, nameof (arg0));
    this._arg0 = arg0;
  }

  public override Expression Reduce()
  {
    return (Expression) Expression.Dynamic(this.Binder, this.Type, this._arg0);
  }

  protected sealed override int ArgumentCount => 1;

  public Expression Argument0 => this._arg0;

  protected sealed override Expression GetArgument(int index)
  {
    if (index == 0)
      return this._arg0;
    throw Assert.Unreachable;
  }

  protected override Expression VisitChildren(ExpressionVisitor visitor)
  {
    Expression expression = visitor.Visit(this._arg0);
    return expression == this._arg0 ? (Expression) this : this.Rewrite(this.Binder, expression);
  }

  protected virtual Expression Rewrite(CallSiteBinder binder, Expression arg0)
  {
    return (Expression) new LightDynamicExpression1(binder, arg0);
  }

  Expression ILightExceptionAwareExpression.ReduceForLightExceptions()
  {
    CallSiteBinder lightBinder = this.GetLightBinder();
    return lightBinder != this.Binder ? this.Rewrite(lightBinder, this._arg0) : (Expression) this;
  }
}
