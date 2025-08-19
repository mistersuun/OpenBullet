// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.CompilerServices.DebugLambdaInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Debugging.CompilerServices;

public sealed class DebugLambdaInfo
{
  public DebugLambdaInfo(
    IDebugCompilerSupport compilerSupport,
    string lambdaAlias,
    bool optimizeForLeafFrames,
    IList<ParameterExpression> hiddenVariables,
    IDictionary<ParameterExpression, string> variableAliases,
    object customPayload)
  {
    this.CompilerSupport = compilerSupport;
    this.LambdaAlias = lambdaAlias;
    this.HiddenVariables = hiddenVariables;
    this.VariableAliases = variableAliases;
    this.CustomPayload = customPayload;
    this.OptimizeForLeafFrames = optimizeForLeafFrames;
  }

  public IDebugCompilerSupport CompilerSupport { get; }

  public string LambdaAlias { get; }

  public IList<ParameterExpression> HiddenVariables { get; }

  public IDictionary<ParameterExpression, string> VariableAliases { get; }

  public object CustomPayload { get; }

  public bool OptimizeForLeafFrames { get; }
}
