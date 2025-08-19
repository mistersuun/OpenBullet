// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.DebugSourceFile
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Debugging;

public sealed class DebugSourceFile
{
  private DebugMode _debugMode;
  private readonly Dictionary<DebugSourceSpan, FunctionInfo> _functionInfoMap;

  internal DebugSourceFile(string fileName, DebugMode debugMode)
  {
    this.Name = fileName;
    this._debugMode = debugMode;
    this._functionInfoMap = new Dictionary<DebugSourceSpan, FunctionInfo>();
  }

  internal Dictionary<DebugSourceSpan, FunctionInfo> FunctionInfoMap => this._functionInfoMap;

  internal string Name { get; }

  internal DebugMode DebugMode
  {
    get => this._debugMode;
    set => this._debugMode = value;
  }

  internal FunctionInfo LookupFunctionInfo(DebugSourceSpan span)
  {
    foreach (KeyValuePair<DebugSourceSpan, FunctionInfo> functionInfo in this._functionInfoMap)
    {
      if (functionInfo.Key.Intersects(span))
        return functionInfo.Value;
    }
    return (FunctionInfo) null;
  }

  [Obsolete("do not call this property", true)]
  public int Mode => (int) this._debugMode;
}
