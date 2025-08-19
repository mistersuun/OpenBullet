// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.RuntimeVariablesInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class RuntimeVariablesInstruction : Instruction
{
  private readonly int _count;

  public RuntimeVariablesInstruction(int count) => this._count = count;

  public override int ProducedStack => 1;

  public override int ConsumedStack => this._count;

  public override int Run(InterpretedFrame frame)
  {
    IStrongBox[] boxes = new IStrongBox[this._count];
    for (int index = boxes.Length - 1; index >= 0; --index)
      boxes[index] = (IStrongBox) frame.Pop();
    frame.Push((object) RuntimeVariables.Create(boxes));
    return 1;
  }

  public override string ToString() => "GetRuntimeVariables()";
}
