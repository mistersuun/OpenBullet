// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.EnterLoopInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class EnterLoopInstruction : Instruction
{
  private readonly int _instructionIndex;
  private Dictionary<ParameterExpression, LocalVariable> _variables;
  private Dictionary<ParameterExpression, LocalVariable> _closureVariables;
  private LoopExpression _loop;
  private int _loopEnd;
  private int _compilationThreshold;

  internal EnterLoopInstruction(
    LoopExpression loop,
    LocalVariables locals,
    int compilationThreshold,
    int instructionIndex)
  {
    this._loop = loop;
    this._variables = locals.CopyLocals();
    this._closureVariables = locals.ClosureVariables;
    this._compilationThreshold = compilationThreshold;
    this._instructionIndex = instructionIndex;
  }

  internal void FinishLoop(int loopEnd) => this._loopEnd = loopEnd;

  public override int Run(InterpretedFrame frame)
  {
    if (this._compilationThreshold-- == 0)
    {
      if (frame.Interpreter.CompileSynchronously)
        this.Compile((object) frame);
      else
        new Task(new Action<object>(this.Compile), (object) frame).Start();
    }
    return 1;
  }

  private bool Compiled => this._loop == null;

  private void Compile(object frameObj)
  {
    if (this.Compiled)
      return;
    lock (this)
    {
      if (this.Compiled)
        return;
      InterpretedFrame interpretedFrame = (InterpretedFrame) frameObj;
      LoopCompiler loopCompiler = new LoopCompiler(this._loop, interpretedFrame.Interpreter.LabelMapping, this._variables, this._closureVariables, this._instructionIndex, this._loopEnd);
      Interlocked.Exchange<Instruction>(ref interpretedFrame.Interpreter.Instructions.Instructions[this._instructionIndex], (Instruction) new CompiledLoopInstruction(loopCompiler.CreateDelegate()));
      this._loop = (LoopExpression) null;
      this._variables = (Dictionary<ParameterExpression, LocalVariable>) null;
      this._closureVariables = (Dictionary<ParameterExpression, LocalVariable>) null;
    }
  }
}
