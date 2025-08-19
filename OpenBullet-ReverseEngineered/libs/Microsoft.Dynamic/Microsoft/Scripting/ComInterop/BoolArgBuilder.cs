// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.BoolArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal sealed class BoolArgBuilder : SimpleArgBuilder
{
  internal BoolArgBuilder(Type parameterType)
    : base(parameterType)
  {
  }

  internal override Expression MarshalToRef(Expression parameter)
  {
    return (Expression) Expression.Condition(this.Marshal(parameter), (Expression) Expression.Constant((object) (short) -1), (Expression) Expression.Constant((object) (short) 0));
  }

  internal override Expression UnmarshalFromRef(Expression value)
  {
    return base.UnmarshalFromRef((Expression) Expression.NotEqual(value, (Expression) Expression.Constant((object) (short) 0)));
  }
}
