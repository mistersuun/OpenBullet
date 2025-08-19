// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.FunctionInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace Microsoft.Scripting.Debugging;

[DebuggerDisplay("{Name}")]
public sealed class FunctionInfo
{
  private readonly IList<VariableInfo>[] _variableScopeMap;
  private readonly bool[] _traceLocations;

  internal FunctionInfo(
    Delegate generatorFactory,
    string name,
    DebugSourceSpan[] sequencePoints,
    IList<VariableInfo>[] scopedVariables,
    IList<VariableInfo> variables,
    object customPayload)
  {
    this.GeneratorFactory = generatorFactory;
    this.Name = name;
    this.SequencePoints = sequencePoints;
    this._variableScopeMap = scopedVariables;
    this.Variables = variables;
    this.CustomPayload = customPayload;
    this._traceLocations = new bool[sequencePoints.Length];
  }

  internal Delegate GeneratorFactory { get; }

  internal IList<VariableInfo> Variables { get; }

  internal IList<VariableInfo>[] VariableScopeMap => this._variableScopeMap;

  internal FunctionInfo PreviousVersion { get; set; }

  internal FunctionInfo NextVersion { get; set; }

  internal int Version { get; set; }

  internal DebugSourceSpan[] SequencePoints { get; }

  internal string Name { get; }

  internal object CustomPayload { get; }

  internal bool[] GetTraceLocations() => this._traceLocations;
}
