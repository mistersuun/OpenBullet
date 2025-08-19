// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.GotoInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class GotoInstruction : IndexedBranchInstruction
{
  private const int Variants = 4;
  private static readonly GotoInstruction[] Cache = new GotoInstruction[128 /*0x80*/];
  private readonly bool _hasResult;
  private readonly bool _hasValue;

  public override int ConsumedContinuations => 0;

  public override int ProducedContinuations => 0;

  public override int ConsumedStack => !this._hasValue ? 0 : 1;

  public override int ProducedStack => !this._hasResult ? 0 : 1;

  private GotoInstruction(int targetIndex, bool hasResult, bool hasValue)
    : base(targetIndex)
  {
    this._hasResult = hasResult;
    this._hasValue = hasValue;
  }

  internal static GotoInstruction Create(int labelIndex, bool hasResult, bool hasValue)
  {
    if (labelIndex >= 32 /*0x20*/)
      return new GotoInstruction(labelIndex, hasResult, hasValue);
    int index = 4 * labelIndex | (hasResult ? 2 : 0) | (hasValue ? 1 : 0);
    return GotoInstruction.Cache[index] ?? (GotoInstruction.Cache[index] = new GotoInstruction(labelIndex, hasResult, hasValue));
  }

  public override int Run(InterpretedFrame frame)
  {
    Microsoft.Scripting.Interpreter.Interpreter.AbortThreadIfRequested(frame, this._labelIndex);
    return frame.Goto(this._labelIndex, this._hasValue ? frame.Pop() : Microsoft.Scripting.Interpreter.Interpreter.NoValue);
  }
}
