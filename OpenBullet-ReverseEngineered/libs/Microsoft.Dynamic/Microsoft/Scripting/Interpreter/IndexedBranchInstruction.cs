// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.IndexedBranchInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal abstract class IndexedBranchInstruction : Instruction
{
  protected const int CacheSize = 32 /*0x20*/;
  internal readonly int _labelIndex;

  public IndexedBranchInstruction(int labelIndex) => this._labelIndex = labelIndex;

  public RuntimeLabel GetLabel(InterpretedFrame frame)
  {
    return frame.Interpreter._labels[this._labelIndex];
  }

  public override string ToDebugString(
    int instructionIndex,
    object cookie,
    Func<int, int> labelIndexer,
    IList<object> objects)
  {
    int num = labelIndexer(this._labelIndex);
    return this.ToString() + (num != int.MinValue ? " -> " + num.ToString() : "");
  }

  public override string ToString() => $"{this.InstructionName}[{(object) this._labelIndex}]";
}
