// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.LeaveFaultInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class LeaveFaultInstruction : Instruction
{
  internal static readonly Instruction NonVoid = (Instruction) new LeaveFaultInstruction(true);
  internal static readonly Instruction Void = (Instruction) new LeaveFaultInstruction(false);
  private readonly bool _hasValue;

  public override int ConsumedStack => 1;

  public override int ProducedStack => !this._hasValue ? 0 : 1;

  private LeaveFaultInstruction(bool hasValue) => this._hasValue = hasValue;

  public override int Run(InterpretedFrame frame)
  {
    object exception = frame.Pop();
    return frame.Interpreter.GotoHandler(frame, exception, out ExceptionHandler _);
  }
}
