// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.VariableInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Diagnostics;

#nullable disable
namespace Microsoft.Scripting.Debugging;

[DebuggerDisplay("{Name}")]
internal sealed class VariableInfo
{
  private readonly int _localIndex;
  private readonly int _globalIndex;

  internal VariableInfo(
    string name,
    Type type,
    bool parameter,
    bool hidden,
    bool strongBoxed,
    int localIndex,
    int globalIndex)
  {
    this.Name = name;
    this.VariableType = type;
    this.IsParameter = parameter;
    this.Hidden = hidden;
    this.IsStrongBoxed = strongBoxed;
    this._localIndex = localIndex;
    this._globalIndex = globalIndex;
  }

  internal VariableInfo(string name, Type type, bool parameter, bool hidden, bool strongBoxed)
    : this(name, type, parameter, hidden, strongBoxed, int.MaxValue, int.MaxValue)
  {
    this.Name = name;
    this.VariableType = type;
    this.IsParameter = parameter;
    this.Hidden = hidden;
    this.IsStrongBoxed = strongBoxed;
  }

  internal bool Hidden { get; }

  internal bool IsStrongBoxed { get; }

  internal int LocalIndex => this._localIndex;

  internal int GlobalIndex => this._globalIndex;

  internal Type VariableType { get; }

  internal string Name { get; }

  internal bool IsParameter { get; }
}
