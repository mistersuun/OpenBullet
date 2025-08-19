// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ConvertArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal class ConvertArgBuilder : SimpleArgBuilder
{
  private readonly Type _marshalType;

  internal ConvertArgBuilder(Type parameterType, Type marshalType)
    : base(parameterType)
  {
    this._marshalType = marshalType;
  }

  internal override Expression Marshal(Expression parameter)
  {
    parameter = base.Marshal(parameter);
    return (Expression) Expression.Convert(parameter, this._marshalType);
  }

  internal override Expression UnmarshalFromRef(Expression newValue)
  {
    return base.UnmarshalFromRef((Expression) Expression.Convert(newValue, this.ParameterType));
  }
}
