// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LocalAccessInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal abstract class LocalAccessInstruction : Instruction
{
  internal readonly int _index;

  protected LocalAccessInstruction(int index) => this._index = index;

  public override string ToDebugString(
    int instructionIndex,
    object cookie,
    Func<int, int> labelIndexer,
    IList<object> objects)
  {
    return cookie != null ? $"{this.InstructionName}({cookie}: {(object) this._index})" : $"{this.InstructionName}({(object) this._index})";
  }
}
