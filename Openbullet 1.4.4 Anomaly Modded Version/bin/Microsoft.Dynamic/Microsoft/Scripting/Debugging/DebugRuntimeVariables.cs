// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.DebugRuntimeVariables
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Debugging;

[DebuggerDisplay("Count = {Count}")]
internal class DebugRuntimeVariables : IDebugRuntimeVariables, IRuntimeVariables
{
  private readonly IRuntimeVariables _runtimeVariables;

  internal DebugRuntimeVariables(IRuntimeVariables runtimeVariables)
  {
    this._runtimeVariables = runtimeVariables;
  }

  public int Count => this._runtimeVariables.Count - 2;

  public object this[int index]
  {
    get => this._runtimeVariables[2 + index];
    set => this._runtimeVariables[2 + index] = value;
  }

  public FunctionInfo FunctionInfo => (FunctionInfo) this._runtimeVariables[0];

  public int DebugMarker => (int) this._runtimeVariables[1];
}
