// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.AssignLocalInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class AssignLocalInstruction : LocalAccessInstruction, IBoxableInstruction
{
  internal AssignLocalInstruction(int index)
    : base(index)
  {
  }

  public override int ConsumedStack => 1;

  public override int ProducedStack => 1;

  public override int Run(InterpretedFrame frame)
  {
    frame.Data[this._index] = frame.Peek();
    return 1;
  }

  public Instruction BoxIfIndexMatches(int index)
  {
    return index != this._index ? (Instruction) null : InstructionList.AssignLocalBoxed(index);
  }
}
