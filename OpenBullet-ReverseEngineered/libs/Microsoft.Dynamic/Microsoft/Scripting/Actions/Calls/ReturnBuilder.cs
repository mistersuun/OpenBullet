// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.ReturnBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

internal class ReturnBuilder
{
  public ReturnBuilder(Type returnType) => this.ReturnType = returnType;

  internal virtual Expression ToExpression(
    OverloadResolver resolver,
    IList<ArgBuilder> builders,
    RestrictedArguments args,
    Expression ret)
  {
    return ret;
  }

  public virtual int CountOutParams => 0;

  public Type ReturnType { get; }
}
