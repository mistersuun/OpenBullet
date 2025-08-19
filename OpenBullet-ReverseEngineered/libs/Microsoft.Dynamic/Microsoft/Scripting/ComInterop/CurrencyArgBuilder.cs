// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.CurrencyArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal sealed class CurrencyArgBuilder : SimpleArgBuilder
{
  internal CurrencyArgBuilder(Type parameterType)
    : base(parameterType)
  {
  }

  internal override Expression Marshal(Expression parameter)
  {
    return (Expression) Expression.Property(Helpers.Convert(base.Marshal(parameter), typeof (CurrencyWrapper)), "WrappedObject");
  }

  internal override Expression MarshalToRef(Expression parameter)
  {
    return (Expression) Expression.Call(typeof (Decimal).GetMethod("ToOACurrency"), this.Marshal(parameter));
  }

  internal override Expression UnmarshalFromRef(Expression value)
  {
    return base.UnmarshalFromRef((Expression) Expression.New(typeof (CurrencyWrapper).GetConstructor(new Type[1]
    {
      typeof (Decimal)
    }), (Expression) Expression.Call(typeof (Decimal).GetMethod("FromOACurrency"), value)));
  }
}
