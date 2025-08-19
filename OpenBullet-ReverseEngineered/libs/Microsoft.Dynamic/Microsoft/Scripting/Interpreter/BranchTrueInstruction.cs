// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.BranchTrueInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class BranchTrueInstruction : OffsetInstruction
{
  private static Instruction[] _cache;

  public override Instruction[] Cache
  {
    get
    {
      return BranchTrueInstruction._cache ?? (BranchTrueInstruction._cache = new Instruction[32 /*0x20*/]);
    }
  }

  public override int ConsumedStack => 1;

  public override int Run(InterpretedFrame frame) => (bool) frame.Pop() ? this._offset : 1;
}
