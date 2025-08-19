// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.KeywordArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

internal sealed class KeywordArgBuilder : ArgBuilder
{
  private readonly int _kwArgCount;
  private readonly int _kwArgIndex;
  private readonly ArgBuilder _builder;

  public KeywordArgBuilder(ArgBuilder builder, int kwArgCount, int kwArgIndex)
    : base(builder.ParameterInfo)
  {
    this._builder = builder;
    this._kwArgCount = kwArgCount;
    this._kwArgIndex = kwArgIndex;
  }

  public override int Priority => this._builder.Priority;

  public override int ConsumedArgumentCount => 1;

  internal static bool BuilderExpectsSingleParameter(ArgBuilder builder)
  {
    return ((SimpleArgBuilder) builder).Index == 0;
  }

  protected internal override Expression ToExpression(
    OverloadResolver resolver,
    RestrictedArguments args,
    bool[] hasBeenUsed)
  {
    int keywordIndex = this.GetKeywordIndex(args.Length);
    hasBeenUsed[keywordIndex] = true;
    return this._builder.ToExpression(resolver, KeywordArgBuilder.MakeRestrictedArg(args, keywordIndex), new bool[1]);
  }

  public override Type Type => this._builder.Type;

  internal override Expression ToReturnExpression(OverloadResolver resolver)
  {
    return this._builder.ToReturnExpression(resolver);
  }

  internal override Expression UpdateFromReturn(OverloadResolver resolver, RestrictedArguments args)
  {
    int keywordIndex = this.GetKeywordIndex(args.Length);
    return this._builder.UpdateFromReturn(resolver, KeywordArgBuilder.MakeRestrictedArg(args, keywordIndex));
  }

  private static RestrictedArguments MakeRestrictedArg(RestrictedArguments args, int index)
  {
    return new RestrictedArguments(new DynamicMetaObject[1]
    {
      args.GetObject(index)
    }, new Type[1]{ args.GetType(index) }, false);
  }

  private int GetKeywordIndex(int paramCount) => paramCount - this._kwArgCount + this._kwArgIndex;

  internal override Expression ByRefArgument => this._builder.ByRefArgument;

  public override ArgBuilder Clone(ParameterInfo newType)
  {
    return (ArgBuilder) new KeywordArgBuilder(this._builder.Clone(newType), this._kwArgCount, this._kwArgIndex);
  }
}
