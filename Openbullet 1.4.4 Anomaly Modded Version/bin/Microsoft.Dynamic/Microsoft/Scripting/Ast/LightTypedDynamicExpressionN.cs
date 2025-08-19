// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Ast.LightTypedDynamicExpressionN
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Ast;

public class LightTypedDynamicExpressionN : LightDynamicExpression, ILightExceptionAwareExpression
{
  private readonly IList<Expression> _args;
  private readonly Type _returnType;

  protected internal LightTypedDynamicExpressionN(
    CallSiteBinder binder,
    Type returnType,
    IList<Expression> args)
    : base(binder)
  {
    ContractUtils.RequiresNotNull((object) returnType, nameof (returnType));
    ContractUtils.RequiresNotEmpty<Expression>((ICollection<Expression>) args, nameof (args));
    this._args = args;
    this._returnType = returnType;
  }

  public override Expression Reduce()
  {
    return (Expression) Expression.Dynamic(this.Binder, this.Type, (IEnumerable<Expression>) this._args.ToReadOnly<Expression>());
  }

  protected sealed override int ArgumentCount => this._args.Count;

  public sealed override Type Type => this._returnType;

  public IList<Expression> Arguments => this._args;

  protected virtual Expression Rewrite(CallSiteBinder binder, IList<Expression> args)
  {
    return (Expression) new LightTypedDynamicExpressionN(binder, this._returnType, args);
  }

  protected override Expression VisitChildren(ExpressionVisitor visitor)
  {
    Expression[] args = (Expression[]) null;
    for (int index1 = 0; index1 < this._args.Count; ++index1)
    {
      Expression expression = visitor.Visit(this._args[index1]);
      if (expression != this._args[index1])
      {
        if (args == null)
        {
          args = new Expression[this._args.Count];
          for (int index2 = 0; index2 < index1; ++index2)
            args[index2] = this._args[index2];
        }
        args[index1] = expression;
      }
      else if (args != null)
        args[index1] = expression;
    }
    return args != null ? this.Rewrite(this.Binder, (IList<Expression>) args) : (Expression) this;
  }

  protected sealed override Expression GetArgument(int index) => this._args[index];

  Expression ILightExceptionAwareExpression.ReduceForLightExceptions()
  {
    CallSiteBinder lightBinder = this.GetLightBinder();
    return lightBinder != this.Binder ? this.Rewrite(lightBinder, this._args) : (Expression) this;
  }
}
