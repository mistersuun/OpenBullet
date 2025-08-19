// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.CompiledLoopInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class CompiledLoopInstruction : Instruction
{
  private readonly Func<object[], StrongBox<object>[], InterpretedFrame, int> _compiledLoop;

  public CompiledLoopInstruction(
    Func<object[], StrongBox<object>[], InterpretedFrame, int> compiledLoop)
  {
    this._compiledLoop = compiledLoop;
  }

  public override int Run(InterpretedFrame frame)
  {
    return this._compiledLoop(frame.Data, frame.Closure, frame);
  }
}
