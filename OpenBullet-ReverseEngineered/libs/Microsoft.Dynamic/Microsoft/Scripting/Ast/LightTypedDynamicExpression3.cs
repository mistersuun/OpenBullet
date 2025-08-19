// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LightTypedDynamicExpression3
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Ast;

internal class LightTypedDynamicExpression3 : LightDynamicExpression3, ILightExceptionAwareExpression
{
  private readonly Type _returnType;

  protected internal LightTypedDynamicExpression3(
    CallSiteBinder binder,
    Type returnType,
    Expression arg0,
    Expression arg1,
    Expression arg2)
    : base(binder, arg0, arg1, arg2)
  {
    ContractUtils.RequiresNotNull((object) returnType, nameof (returnType));
    this._returnType = returnType;
  }

  public sealed override Type Type => this._returnType;

  protected override Expression Rewrite(
    CallSiteBinder binder,
    Expression arg0,
    Expression arg1,
    Expression arg2)
  {
    return (Expression) new LightTypedDynamicExpression3(binder, this._returnType, arg0, arg1, arg2);
  }

  Expression ILightExceptionAwareExpression.ReduceForLightExceptions()
  {
    CallSiteBinder lightBinder = this.GetLightBinder();
    return lightBinder != this.Binder ? this.Rewrite(lightBinder, this._arg0, this._arg1, this._arg2) : (Expression) this;
  }
}
