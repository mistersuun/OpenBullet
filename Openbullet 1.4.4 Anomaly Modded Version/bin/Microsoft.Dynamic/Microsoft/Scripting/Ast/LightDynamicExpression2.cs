// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LightDynamicExpression2
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Ast;

public class LightDynamicExpression2 : LightDynamicExpression, ILightExceptionAwareExpression
{
  internal readonly Expression _arg0;
  internal readonly Expression _arg1;

  protected internal LightDynamicExpression2(
    CallSiteBinder binder,
    Expression arg0,
    Expression arg1)
    : base(binder)
  {
    ContractUtils.RequiresNotNull((object) arg0, nameof (arg0));
    ContractUtils.RequiresNotNull((object) arg1, nameof (arg1));
    this._arg0 = arg0;
    this._arg1 = arg1;
  }

  public override Expression Reduce()
  {
    return (Expression) Expression.Dynamic(this.Binder, this.Type, this._arg0, this._arg1);
  }

  protected override int ArgumentCount => 2;

  public Expression Argument0 => this._arg0;

  public Expression Argument1 => this._arg1;

  protected override Expression GetArgument(int index)
  {
    if (index == 0)
      return this._arg0;
    if (index == 1)
      return this._arg1;
    throw Assert.Unreachable;
  }

  protected override Expression VisitChildren(ExpressionVisitor visitor)
  {
    Expression expression1 = visitor.Visit(this._arg0);
    Expression expression2 = visitor.Visit(this._arg1);
    return expression1 == this._arg0 && this._arg1 == expression2 ? (Expression) this : this.Rewrite(this.Binder, expression1, expression2);
  }

  protected virtual Expression Rewrite(CallSiteBinder binder, Expression arg0, Expression arg1)
  {
    return (Expression) new LightDynamicExpression2(binder, arg0, arg1);
  }

  Expression ILightExceptionAwareExpression.ReduceForLightExceptions()
  {
    CallSiteBinder lightBinder = this.GetLightBinder();
    return lightBinder != this.Binder ? this.Rewrite(lightBinder, this._arg0, this._arg1) : (Expression) this;
  }
}
