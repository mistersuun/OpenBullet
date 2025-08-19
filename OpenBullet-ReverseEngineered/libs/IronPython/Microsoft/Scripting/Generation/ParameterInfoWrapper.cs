// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.ParameterInfoWrapper
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Generation;

internal class ParameterInfoWrapper
{
  private readonly ParameterInfo parameterInfo;
  private readonly Type parameterType;
  private readonly string name;

  public ParameterInfoWrapper(ParameterInfo parameterInfo) => this.parameterInfo = parameterInfo;

  public ParameterInfoWrapper(Type parameterType, string name = null)
  {
    this.parameterType = parameterType;
    this.name = name;
  }

  public Type ParameterType
  {
    get => this.parameterInfo != null ? this.parameterInfo.ParameterType : this.parameterType;
  }

  public string Name => this.parameterInfo != null ? this.parameterInfo.Name : this.name;

  public ParameterAttributes Attributes
  {
    get => this.parameterInfo != null ? this.parameterInfo.Attributes : ParameterAttributes.None;
  }
}
