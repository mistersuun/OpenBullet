// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.BranchInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal class BranchInstruction : OffsetInstruction
{
  private static Instruction[][][] _caches;
  internal readonly bool _hasResult;
  internal readonly bool _hasValue;

  public override Instruction[] Cache
  {
    get
    {
      if (BranchInstruction._caches == null)
        BranchInstruction._caches = new Instruction[2][][]
        {
          new Instruction[2][],
          new Instruction[2][]
        };
      return BranchInstruction._caches[this.ConsumedStack][this.ProducedStack] ?? (BranchInstruction._caches[this.ConsumedStack][this.ProducedStack] = new Instruction[32 /*0x20*/]);
    }
  }

  internal BranchInstruction()
    : this(false, false)
  {
  }

  public BranchInstruction(bool hasResult, bool hasValue)
  {
    this._hasResult = hasResult;
    this._hasValue = hasValue;
  }

  public override int ConsumedStack => !this._hasValue ? 0 : 1;

  public override int ProducedStack => !this._hasResult ? 0 : 1;

  public override int Run(InterpretedFrame frame) => this._offset;
}
