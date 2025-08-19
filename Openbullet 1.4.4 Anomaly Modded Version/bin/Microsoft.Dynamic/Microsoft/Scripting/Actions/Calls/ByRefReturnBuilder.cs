// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.ByRefReturnBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Ast;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

internal sealed class ByRefReturnBuilder : ReturnBuilder
{
  private readonly IList<int> _returnArgs;

  public ByRefReturnBuilder(IList<int> returnArgs)
    : base(typeof (object))
  {
    this._returnArgs = returnArgs;
  }

  internal override Expression ToExpression(
    OverloadResolver resolver,
    IList<ArgBuilder> builders,
    RestrictedArguments args,
    Expression ret)
  {
    if (this._returnArgs.Count == 1)
      return this._returnArgs[0] == -1 ? ret : (Expression) Expression.Block(ret, builders[this._returnArgs[0]].ToReturnExpression(resolver));
    Expression[] initializers = new Expression[this._returnArgs.Count];
    int num = 0;
    bool flag = false;
    foreach (int returnArg in (IEnumerable<int>) this._returnArgs)
    {
      if (returnArg == -1)
      {
        flag = true;
        initializers[num++] = ret;
      }
      else
        initializers[num++] = builders[returnArg].ToReturnExpression(resolver);
    }
    Expression argumentArrayExpression = (Expression) Utils.NewArrayHelper(typeof (object), (IEnumerable<Expression>) initializers);
    if (!flag)
      argumentArrayExpression = (Expression) Expression.Block(ret, argumentArrayExpression);
    return resolver.GetByRefArrayExpression(argumentArrayExpression);
  }

  public override int CountOutParams => this._returnArgs.Count;
}
