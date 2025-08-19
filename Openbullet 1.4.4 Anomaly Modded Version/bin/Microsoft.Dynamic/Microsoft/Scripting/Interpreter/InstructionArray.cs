// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.InstructionArray
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

[DebuggerTypeProxy(typeof (InstructionArray.DebugView))]
public struct InstructionArray
{
  internal readonly int MaxStackDepth;
  internal readonly int MaxContinuationDepth;
  internal readonly Instruction[] Instructions;
  internal readonly object[] Objects;
  internal readonly RuntimeLabel[] Labels;
  internal readonly List<KeyValuePair<int, object>> DebugCookies;

  internal InstructionArray(
    int maxStackDepth,
    int maxContinuationDepth,
    Instruction[] instructions,
    object[] objects,
    RuntimeLabel[] labels,
    List<KeyValuePair<int, object>> debugCookies)
  {
    this.MaxStackDepth = maxStackDepth;
    this.MaxContinuationDepth = maxContinuationDepth;
    this.Instructions = instructions;
    this.DebugCookies = debugCookies;
    this.Objects = objects;
    this.Labels = labels;
  }

  internal int Length => this.Instructions.Length;

  internal sealed class DebugView
  {
    private readonly InstructionArray _array;

    public DebugView(InstructionArray array) => this._array = array;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public InstructionList.DebugView.InstructionView[] A0
    {
      get
      {
        return InstructionList.DebugView.GetInstructionViews((IList<Instruction>) this._array.Instructions, (IList<object>) this._array.Objects, (Func<int, int>) (index => this._array.Labels[index].Index), (IList<KeyValuePair<int, object>>) this._array.DebugCookies);
      }
    }
  }
}
