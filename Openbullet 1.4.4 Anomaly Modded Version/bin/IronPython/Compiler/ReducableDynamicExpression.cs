// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.ReducableDynamicExpression
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Ast;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Compiler;

internal class ReducableDynamicExpression : Expression, ILightExceptionAwareExpression
{
  private readonly Expression _reduction;
  private readonly DynamicMetaObjectBinder _binder;
  private readonly IList<Expression> _args;

  public ReducableDynamicExpression(
    Expression reduction,
    DynamicMetaObjectBinder binder,
    IList<Expression> args)
  {
    this._reduction = reduction;
    this._binder = binder;
    this._args = args;
  }

  public DynamicMetaObjectBinder Binder => this._binder;

  public IList<Expression> Args => this._args;

  public override bool CanReduce => true;

  public sealed override ExpressionType NodeType => ExpressionType.Extension;

  public sealed override Type Type => this._reduction.Type;

  public override Expression Reduce() => this._reduction;

  Expression ILightExceptionAwareExpression.ReduceForLightExceptions()
  {
    if (this.Binder is ILightExceptionBinder binder)
    {
      DynamicMetaObjectBinder lightExceptionBinder = binder.GetLightExceptionBinder() as DynamicMetaObjectBinder;
      if (lightExceptionBinder != binder)
        return (Expression) Expression.Dynamic((CallSiteBinder) lightExceptionBinder, this.Type, (IEnumerable<Expression>) this._args);
    }
    return (Expression) this;
  }
}
