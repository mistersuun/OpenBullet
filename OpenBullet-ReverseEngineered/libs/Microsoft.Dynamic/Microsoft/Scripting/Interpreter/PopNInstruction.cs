// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.PopNInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class PopNInstruction : Instruction
{
  private readonly int _n;

  internal PopNInstruction(int n) => this._n = n;

  public override int ConsumedStack => this._n;

  public override int Run(InterpretedFrame frame)
  {
    frame.Pop(this._n);
    return 1;
  }

  public override string ToString() => $"Pop({(object) this._n})";
}
