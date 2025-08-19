// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.ThrowInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class ThrowInstruction : Instruction
{
  internal static readonly ThrowInstruction Throw = new ThrowInstruction(true, false);
  internal static readonly ThrowInstruction VoidThrow = new ThrowInstruction(false, false);
  internal static readonly ThrowInstruction Rethrow = new ThrowInstruction(true, true);
  internal static readonly ThrowInstruction VoidRethrow = new ThrowInstruction(false, true);
  private readonly bool _hasResult;
  private readonly bool _rethrow;

  private ThrowInstruction(bool hasResult, bool isRethrow)
  {
    this._hasResult = hasResult;
    this._rethrow = isRethrow;
  }

  public override int ProducedStack => !this._hasResult ? 0 : 1;

  public override int ConsumedStack => 1;

  public override int Run(InterpretedFrame frame)
  {
    Exception exception = (Exception) frame.Pop();
    if (this._rethrow)
      return frame.Interpreter.GotoHandler(frame, (object) exception, out ExceptionHandler _);
    throw exception;
  }
}
