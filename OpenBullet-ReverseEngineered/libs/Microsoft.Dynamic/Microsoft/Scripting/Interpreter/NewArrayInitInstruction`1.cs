// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.NewArrayInitInstruction`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public sealed class NewArrayInitInstruction<TElement> : Instruction
{
  private readonly int _elementCount;

  internal NewArrayInitInstruction(int elementCount) => this._elementCount = elementCount;

  public override int ConsumedStack => this._elementCount;

  public override int ProducedStack => 1;

  public override int Run(InterpretedFrame frame)
  {
    TElement[] elementArray = new TElement[this._elementCount];
    for (int index = this._elementCount - 1; index >= 0; --index)
      elementArray[index] = (TElement) frame.Pop();
    frame.Push((object) elementArray);
    return 1;
  }
}
