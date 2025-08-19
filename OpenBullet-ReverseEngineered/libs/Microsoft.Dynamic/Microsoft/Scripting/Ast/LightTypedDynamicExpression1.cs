// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LightTypedDynamicExpression1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Ast;

public class LightTypedDynamicExpression1 : LightDynamicExpression1, ILightExceptionAwareExpression
{
  private readonly Type _returnType;

  protected internal LightTypedDynamicExpression1(
    CallSiteBinder binder,
    Type returnType,
    Expression arg0)
    : base(binder, arg0)
  {
    ContractUtils.RequiresNotNull((object) returnType, nameof (returnType));
    this._returnType = returnType;
  }

  public sealed override Type Type => this._returnType;

  protected override Expression Rewrite(CallSiteBinder binder, Expression arg0)
  {
    return (Expression) new LightTypedDynamicExpression1(binder, this._returnType, arg0);
  }

  Expression ILightExceptionAwareExpression.ReduceForLightExceptions()
  {
    CallSiteBinder lightBinder = this.GetLightBinder();
    return lightBinder != this.Binder ? this.Rewrite(lightBinder, this._arg0) : (Expression) this;
  }
}
