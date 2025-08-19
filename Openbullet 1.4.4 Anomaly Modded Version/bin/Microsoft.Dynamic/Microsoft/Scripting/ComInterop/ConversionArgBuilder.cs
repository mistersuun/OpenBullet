// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ConversionArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal class ConversionArgBuilder : ArgBuilder
{
  private readonly SimpleArgBuilder _innerBuilder;
  private readonly Type _parameterType;

  internal ConversionArgBuilder(Type parameterType, SimpleArgBuilder innerBuilder)
  {
    this._parameterType = parameterType;
    this._innerBuilder = innerBuilder;
  }

  internal override Expression Marshal(Expression parameter)
  {
    return this._innerBuilder.Marshal(Helpers.Convert(parameter, this._parameterType));
  }

  internal override Expression MarshalToRef(Expression parameter) => throw Assert.Unreachable;
}
