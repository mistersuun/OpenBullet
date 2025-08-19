// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.EnterTryFinallyInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class EnterTryFinallyInstruction : IndexedBranchInstruction
{
  private static readonly EnterTryFinallyInstruction[] Cache = new EnterTryFinallyInstruction[32 /*0x20*/];

  public override int ProducedContinuations => 1;

  private EnterTryFinallyInstruction(int targetIndex)
    : base(targetIndex)
  {
  }

  internal static EnterTryFinallyInstruction Create(int labelIndex)
  {
    return labelIndex < 32 /*0x20*/ ? EnterTryFinallyInstruction.Cache[labelIndex] ?? (EnterTryFinallyInstruction.Cache[labelIndex] = new EnterTryFinallyInstruction(labelIndex)) : new EnterTryFinallyInstruction(labelIndex);
  }

  public override int Run(InterpretedFrame frame)
  {
    frame.PushContinuation(this._labelIndex);
    return 1;
  }
}
