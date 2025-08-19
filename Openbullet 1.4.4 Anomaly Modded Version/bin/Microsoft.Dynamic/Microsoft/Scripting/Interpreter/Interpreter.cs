// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.Interpreter
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class Interpreter
{
  internal static readonly object NoValue = new object();
  internal const int RethrowOnReturn = 2147483647 /*0x7FFFFFFF*/;
  internal readonly int _compilationThreshold;
  internal readonly object[] _objects;
  internal readonly RuntimeLabel[] _labels;
  internal readonly string _name;
  private readonly ExceptionHandler[] _handlers;
  internal readonly DebugInfo[] _debugInfos;
  [ThreadStatic]
  private static ThreadAbortException _anyAbortException = (ThreadAbortException) null;

  internal Interpreter(
    string name,
    LocalVariables locals,
    HybridReferenceDictionary<LabelTarget, BranchLabel> labelMapping,
    InstructionArray instructions,
    ExceptionHandler[] handlers,
    DebugInfo[] debugInfos,
    int compilationThreshold)
  {
    this._name = name;
    this.LocalCount = locals.LocalCount;
    this.ClosureVariables = locals.ClosureVariables;
    this.Instructions = instructions;
    this._objects = instructions.Objects;
    this._labels = instructions.Labels;
    this.LabelMapping = labelMapping;
    this._handlers = handlers;
    this._debugInfos = debugInfos;
    this._compilationThreshold = compilationThreshold;
  }

  internal int ClosureSize => this.ClosureVariables == null ? 0 : this.ClosureVariables.Count;

  internal int LocalCount { get; }

  internal bool CompileSynchronously => this._compilationThreshold <= 1;

  internal InstructionArray Instructions { get; }

  internal Dictionary<ParameterExpression, LocalVariable> ClosureVariables { get; }

  internal HybridReferenceDictionary<LabelTarget, BranchLabel> LabelMapping { get; }

  [SpecialName]
  [MethodImpl(MethodImplOptions.NoInlining)]
  public void Run(InterpretedFrame frame)
  {
label_0:
    try
    {
      Instruction[] instructions = this.Instructions.Instructions;
      int instructionIndex = frame.InstructionIndex;
      while (instructionIndex < instructions.Length)
      {
        instructionIndex += instructions[instructionIndex].Run(frame);
        frame.InstructionIndex = instructionIndex;
      }
    }
    catch (Exception ex)
    {
      switch (this.HandleException(frame, ex))
      {
        case Microsoft.Scripting.Interpreter.Interpreter.ExceptionHandlingResult.Rethrow:
          throw;
        case Microsoft.Scripting.Interpreter.Interpreter.ExceptionHandlingResult.Return:
          break;
        default:
          goto label_0;
      }
    }
  }

  private Microsoft.Scripting.Interpreter.Interpreter.ExceptionHandlingResult HandleException(
    InterpretedFrame frame,
    Exception exception)
  {
    frame.SaveTraceToException(exception);
    ExceptionHandler handler;
    frame.InstructionIndex += this.GotoHandler(frame, (object) exception, out handler);
    if (handler == null || handler.IsFault)
    {
      this.Run(frame);
      return frame.InstructionIndex == int.MaxValue ? Microsoft.Scripting.Interpreter.Interpreter.ExceptionHandlingResult.Rethrow : Microsoft.Scripting.Interpreter.Interpreter.ExceptionHandlingResult.Return;
    }
    if (exception is ThreadAbortException threadAbortException)
    {
      Microsoft.Scripting.Interpreter.Interpreter._anyAbortException = threadAbortException;
      frame.CurrentAbortHandler = handler;
    }
label_6:
    try
    {
      Instruction[] instructions = this.Instructions.Instructions;
      int instructionIndex = frame.InstructionIndex;
      while (instructionIndex < instructions.Length)
      {
        Instruction instruction = instructions[instructionIndex];
        instructionIndex += instruction.Run(frame);
        frame.InstructionIndex = instructionIndex;
        if (instruction is LeaveExceptionHandlerInstruction)
          return Microsoft.Scripting.Interpreter.Interpreter.ExceptionHandlingResult.Continue;
      }
      return frame.InstructionIndex == int.MaxValue ? Microsoft.Scripting.Interpreter.Interpreter.ExceptionHandlingResult.Rethrow : Microsoft.Scripting.Interpreter.Interpreter.ExceptionHandlingResult.Return;
    }
    catch (Exception ex)
    {
      switch (this.HandleException(frame, ex))
      {
        case Microsoft.Scripting.Interpreter.Interpreter.ExceptionHandlingResult.Rethrow:
          throw;
        case Microsoft.Scripting.Interpreter.Interpreter.ExceptionHandlingResult.Continue:
          goto label_6;
        case Microsoft.Scripting.Interpreter.Interpreter.ExceptionHandlingResult.Return:
          return Microsoft.Scripting.Interpreter.Interpreter.ExceptionHandlingResult.Return;
        default:
          throw Assert.Unreachable;
      }
    }
  }

  internal static void AbortThreadIfRequested(InterpretedFrame frame, int targetLabelIndex)
  {
    ExceptionHandler currentAbortHandler = frame.CurrentAbortHandler;
    if (currentAbortHandler == null || currentAbortHandler.IsInside(frame.Interpreter._labels[targetLabelIndex].Index))
      return;
    frame.CurrentAbortHandler = (ExceptionHandler) null;
    Thread currentThread = Thread.CurrentThread;
    if ((currentThread.ThreadState & ThreadState.AbortRequested) == ThreadState.Running)
      return;
    currentThread.Abort(Microsoft.Scripting.Interpreter.Interpreter._anyAbortException.ExceptionState);
  }

  internal ExceptionHandler GetBestHandler(int instructionIndex, Type exceptionType)
  {
    ExceptionHandler other = (ExceptionHandler) null;
    foreach (ExceptionHandler handler in this._handlers)
    {
      if (handler.Matches(exceptionType, instructionIndex) && handler.IsBetterThan(other))
        other = handler;
    }
    return other;
  }

  internal int ReturnAndRethrowLabelIndex => this._labels.Length - 1;

  internal int GotoHandler(InterpretedFrame frame, object exception, out ExceptionHandler handler)
  {
    handler = this.GetBestHandler(frame.InstructionIndex, exception.GetType());
    return handler == null ? frame.Goto(this.ReturnAndRethrowLabelIndex, Microsoft.Scripting.Interpreter.Interpreter.NoValue) : frame.Goto(handler.LabelIndex, exception);
  }

  private enum ExceptionHandlingResult
  {
    Rethrow,
    Continue,
    Return,
  }
}
