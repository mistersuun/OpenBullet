// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LoadCachedObjectInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class LoadCachedObjectInstruction : Instruction
{
  private readonly uint _index;

  internal LoadCachedObjectInstruction(uint index) => this._index = index;

  public override int ProducedStack => 1;

  public override int Run(InterpretedFrame frame)
  {
    frame.Data[frame.StackIndex++] = frame.Interpreter._objects[(int) this._index];
    return 1;
  }

  public override string ToDebugString(
    int instructionIndex,
    object cookie,
    Func<int, int> labelIndexer,
    IList<object> objects)
  {
    return $"LoadCached({this._index}: {objects[(int) this._index]})";
  }

  public override string ToString() => $"LoadCached({(object) this._index})";
}
