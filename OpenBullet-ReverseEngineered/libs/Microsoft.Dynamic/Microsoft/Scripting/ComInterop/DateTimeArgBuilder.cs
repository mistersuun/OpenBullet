// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.DateTimeArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal sealed class DateTimeArgBuilder : SimpleArgBuilder
{
  internal DateTimeArgBuilder(Type parameterType)
    : base(parameterType)
  {
  }

  internal override Expression MarshalToRef(Expression parameter)
  {
    return (Expression) Expression.Call(this.Marshal(parameter), typeof (DateTime).GetMethod("ToOADate"));
  }

  internal override Expression UnmarshalFromRef(Expression value)
  {
    return base.UnmarshalFromRef((Expression) Expression.Call(typeof (DateTime).GetMethod("FromOADate"), value));
  }
}
