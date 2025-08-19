// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ErrorArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal class ErrorArgBuilder : SimpleArgBuilder
{
  internal ErrorArgBuilder(Type parameterType)
    : base(parameterType)
  {
  }

  internal override Expression Marshal(Expression parameter)
  {
    return (Expression) Expression.Property(Helpers.Convert(base.Marshal(parameter), typeof (ErrorWrapper)), "ErrorCode");
  }

  internal override Expression UnmarshalFromRef(Expression value)
  {
    return base.UnmarshalFromRef((Expression) Expression.New(typeof (ErrorWrapper).GetConstructor(new Type[1]
    {
      typeof (int)
    }), value));
  }
}
