// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LightTypedDynamicExpression4
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Ast;

public class LightTypedDynamicExpression4 : LightDynamicExpression4, ILightExceptionAwareExpression
{
  private readonly Type _returnType;

  internal LightTypedDynamicExpression4(
    CallSiteBinder binder,
    Type returnType,
    Expression arg0,
    Expression arg1,
    Expression arg2,
    Expression arg3)
    : base(binder, arg0, arg1, arg2, arg3)
  {
    ContractUtils.RequiresNotNull((object) returnType, nameof (returnType));
    this._returnType = returnType;
  }

  protected override Expression Rewrite(
    CallSiteBinder binder,
    Expression arg0,
    Expression arg1,
    Expression arg2,
    Expression arg3)
  {
    return (Expression) new LightTypedDynamicExpression4(binder, this._returnType, arg0, arg1, arg2, arg3);
  }

  public sealed override Type Type => this._returnType;

  Expression ILightExceptionAwareExpression.ReduceForLightExceptions()
  {
    CallSiteBinder lightBinder = this.GetLightBinder();
    return lightBinder != this.Binder ? this.Rewrite(lightBinder, this._arg0, this._arg1, this._arg2, this._arg3) : (Expression) this;
  }
}
