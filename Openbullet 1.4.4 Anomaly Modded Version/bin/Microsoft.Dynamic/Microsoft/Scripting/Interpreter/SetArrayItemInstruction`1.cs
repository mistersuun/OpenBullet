// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.SetArrayItemInstruction`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public sealed class SetArrayItemInstruction<TElement> : Instruction
{
  internal SetArrayItemInstruction()
  {
  }

  public override int ConsumedStack => 3;

  public override int ProducedStack => 0;

  public override int Run(InterpretedFrame frame)
  {
    TElement element = (TElement) frame.Pop();
    int index = (int) frame.Pop();
    ((TElement[]) frame.Pop())[index] = element;
    return 1;
  }

  public override string InstructionName => "SetArrayItem";
}
