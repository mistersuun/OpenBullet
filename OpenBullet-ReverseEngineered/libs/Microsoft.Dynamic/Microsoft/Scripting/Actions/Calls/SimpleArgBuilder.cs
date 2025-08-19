// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.SimpleArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public class SimpleArgBuilder : ArgBuilder
{
  private readonly Type _parameterType;

  public SimpleArgBuilder(Type parameterType, int index, bool isParams, bool isParamsDict)
    : this((ParameterInfo) null, parameterType, index, isParams, isParamsDict)
  {
  }

  [Obsolete("Use other overload")]
  public SimpleArgBuilder(ParameterInfo info, int index)
    : this(info, info.ParameterType, index, info.IsParamArray(), info.IsParamDictionary())
  {
  }

  public SimpleArgBuilder(
    ParameterInfo info,
    Type parameterType,
    int index,
    bool isParams,
    bool isParamsDict)
    : base(info)
  {
    ContractUtils.Requires(index >= 0);
    ContractUtils.RequiresNotNull((object) parameterType, nameof (parameterType));
    this.Index = index;
    this._parameterType = parameterType;
    this.IsParamsArray = isParams;
    this.IsParamsDict = isParamsDict;
  }

  internal SimpleArgBuilder MakeCopy(int newIndex) => this.Copy(newIndex);

  protected virtual SimpleArgBuilder Copy(int newIndex)
  {
    return new SimpleArgBuilder(this.ParameterInfo, this._parameterType, newIndex, this.IsParamsArray, this.IsParamsDict);
  }

  public override int ConsumedArgumentCount => 1;

  public override int Priority => 0;

  public bool IsParamsArray { get; }

  public bool IsParamsDict { get; }

  protected internal override Expression ToExpression(
    OverloadResolver resolver,
    RestrictedArguments args,
    bool[] hasBeenUsed)
  {
    hasBeenUsed[this.Index] = true;
    return resolver.Convert(args.GetObject(this.Index), args.GetType(this.Index), this.ParameterInfo, this._parameterType);
  }

  public int Index { get; }

  public override Type Type => this._parameterType;

  public override ArgBuilder Clone(ParameterInfo newType)
  {
    return (ArgBuilder) new SimpleArgBuilder(newType, newType.ParameterType, this.Index, this.IsParamsArray, this.IsParamsDict);
  }
}
