// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.NewArrayBoundsInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public sealed class NewArrayBoundsInstruction : Instruction
{
  private readonly Type _elementType;
  private readonly int _rank;

  internal NewArrayBoundsInstruction(Type elementType, int rank)
  {
    this._elementType = elementType;
    this._rank = rank;
  }

  public override int ConsumedStack => this._rank;

  public override int ProducedStack => 1;

  public override int Run(InterpretedFrame frame)
  {
    int[] numArray = new int[this._rank];
    for (int index = this._rank - 1; index >= 0; --index)
      numArray[index] = (int) frame.Pop();
    Array instance = Array.CreateInstance(this._elementType, numArray);
    frame.Push((object) instance);
    return 1;
  }
}
