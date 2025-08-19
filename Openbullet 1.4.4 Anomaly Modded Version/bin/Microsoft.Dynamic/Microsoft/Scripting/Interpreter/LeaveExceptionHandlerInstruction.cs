// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LeaveExceptionHandlerInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class LeaveExceptionHandlerInstruction : IndexedBranchInstruction
{
  private static LeaveExceptionHandlerInstruction[] Cache = new LeaveExceptionHandlerInstruction[64 /*0x40*/];
  private readonly bool _hasValue;

  public override int ConsumedStack => !this._hasValue ? 0 : 1;

  public override int ProducedStack => !this._hasValue ? 0 : 1;

  private LeaveExceptionHandlerInstruction(int labelIndex, bool hasValue)
    : base(labelIndex)
  {
    this._hasValue = hasValue;
  }

  internal static LeaveExceptionHandlerInstruction Create(int labelIndex, bool hasValue)
  {
    if (labelIndex >= 32 /*0x20*/)
      return new LeaveExceptionHandlerInstruction(labelIndex, hasValue);
    int index = 2 * labelIndex | (hasValue ? 1 : 0);
    return LeaveExceptionHandlerInstruction.Cache[index] ?? (LeaveExceptionHandlerInstruction.Cache[index] = new LeaveExceptionHandlerInstruction(labelIndex, hasValue));
  }

  public override int Run(InterpretedFrame frame)
  {
    Microsoft.Scripting.Interpreter.Interpreter.AbortThreadIfRequested(frame, this._labelIndex);
    return this.GetLabel(frame).Index - frame.InstructionIndex;
  }
}
