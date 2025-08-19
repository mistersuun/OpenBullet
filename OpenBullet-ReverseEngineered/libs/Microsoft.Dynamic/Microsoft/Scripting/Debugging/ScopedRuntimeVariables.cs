// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.ScopedRuntimeVariables
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Debugging;

internal class ScopedRuntimeVariables : IRuntimeVariables
{
  private readonly IList<VariableInfo> _variableInfos;
  private readonly IRuntimeVariables _variables;

  internal ScopedRuntimeVariables(IList<VariableInfo> variableInfos, IRuntimeVariables variables)
  {
    this._variableInfos = variableInfos;
    this._variables = variables;
  }

  public int Count => this._variableInfos.Count;

  public object this[int index]
  {
    get => this._variables[this._variableInfos[index].GlobalIndex];
    set => this._variables[this._variableInfos[index].GlobalIndex] = value;
  }
}
