// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.ArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public abstract class ArgBuilder
{
  internal const int AllArguments = -1;

  protected ArgBuilder(ParameterInfo info) => this.ParameterInfo = info;

  public abstract int Priority { get; }

  public ParameterInfo ParameterInfo { get; }

  public abstract int ConsumedArgumentCount { get; }

  protected internal abstract Expression ToExpression(
    OverloadResolver resolver,
    RestrictedArguments args,
    bool[] hasBeenUsed);

  public virtual Type Type => (Type) null;

  internal virtual Expression UpdateFromReturn(OverloadResolver resolver, RestrictedArguments args)
  {
    return (Expression) null;
  }

  internal virtual Expression ToReturnExpression(OverloadResolver resolver)
  {
    throw new InvalidOperationException();
  }

  internal virtual Expression ByRefArgument => (Expression) null;

  public virtual ArgBuilder Clone(ParameterInfo newType) => (ArgBuilder) null;
}
