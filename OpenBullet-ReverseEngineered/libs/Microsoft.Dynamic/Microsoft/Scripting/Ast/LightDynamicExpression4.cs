// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LightDynamicExpression4
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Ast;

public class LightDynamicExpression4 : LightDynamicExpression, ILightExceptionAwareExpression
{
  internal readonly Expression _arg0;
  internal readonly Expression _arg1;
  internal readonly Expression _arg2;
  internal readonly Expression _arg3;

  protected internal LightDynamicExpression4(
    CallSiteBinder binder,
    Expression arg0,
    Expression arg1,
    Expression arg2,
    Expression arg3)
    : base(binder)
  {
    ContractUtils.RequiresNotNull((object) arg0, nameof (arg0));
    ContractUtils.RequiresNotNull((object) arg1, nameof (arg1));
    ContractUtils.RequiresNotNull((object) arg2, nameof (arg2));
    ContractUtils.RequiresNotNull((object) arg3, nameof (arg3));
    this._arg0 = arg0;
    this._arg1 = arg1;
    this._arg2 = arg2;
    this._arg3 = arg3;
  }

  public override Expression Reduce()
  {
    return (Expression) Expression.Dynamic(this.Binder, this.Type, this._arg0, this._arg1, this._arg2, this._arg3);
  }

  protected sealed override int ArgumentCount => 4;

  public Expression Argument0 => this._arg0;

  public Expression Argument1 => this._arg1;

  public Expression Argument2 => this._arg2;

  public Expression Argument3 => this._arg3;

  protected sealed override Expression GetArgument(int index)
  {
    switch (index)
    {
      case 0:
        return this._arg0;
      case 1:
        return this._arg1;
      case 2:
        return this._arg2;
      case 3:
        return this._arg3;
      default:
        throw Assert.Unreachable;
    }
  }

  protected override Expression VisitChildren(ExpressionVisitor visitor)
  {
    Expression expression1 = visitor.Visit(this._arg0);
    Expression expression2 = visitor.Visit(this._arg1);
    Expression expression3 = visitor.Visit(this._arg2);
    Expression expression4 = visitor.Visit(this._arg3);
    return expression1 == this._arg0 && this._arg1 == expression2 && this._arg2 == expression3 && this._arg3 == expression4 ? (Expression) this : this.Rewrite(this.Binder, expression1, expression2, expression3, expression4);
  }

  protected virtual Expression Rewrite(
    CallSiteBinder binder,
    Expression arg0,
    Expression arg1,
    Expression arg2,
    Expression arg3)
  {
    return (Expression) new LightDynamicExpression4(binder, arg0, arg1, arg2, arg3);
  }

  Expression ILightExceptionAwareExpression.ReduceForLightExceptions()
  {
    CallSiteBinder lightBinder = this.GetLightBinder();
    return lightBinder != this.Binder ? this.Rewrite(lightBinder, this._arg0, this._arg1, this._arg2, this._arg3) : (Expression) this;
  }
}
