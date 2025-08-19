// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.SimpleArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal class SimpleArgBuilder : ArgBuilder
{
  private readonly Type _parameterType;

  internal SimpleArgBuilder(Type parameterType) => this._parameterType = parameterType;

  internal Type ParameterType => this._parameterType;

  internal override Expression Marshal(Expression parameter)
  {
    return Helpers.Convert(parameter, this._parameterType);
  }

  internal override Expression UnmarshalFromRef(Expression newValue)
  {
    return base.UnmarshalFromRef(newValue);
  }
}
