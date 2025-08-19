// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.SwitchInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class SwitchInstruction : Instruction
{
  private readonly Dictionary<int, int> _cases;

  internal SwitchInstruction(Dictionary<int, int> cases) => this._cases = cases;

  public override int ConsumedStack => 1;

  public override int ProducedStack => 0;

  public override int Run(InterpretedFrame frame)
  {
    int num;
    return !this._cases.TryGetValue((int) frame.Pop(), out num) ? 1 : num;
  }
}
