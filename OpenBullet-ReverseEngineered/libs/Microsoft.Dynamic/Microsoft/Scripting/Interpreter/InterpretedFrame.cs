// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.InterpretedFrame
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public sealed class InterpretedFrame
{
  public static readonly ThreadLocal<InterpretedFrame> CurrentFrame = new ThreadLocal<InterpretedFrame>();
  internal readonly Microsoft.Scripting.Interpreter.Interpreter Interpreter;
  private int[] _continuations;
  private int _continuationIndex;
  private int _pendingContinuation;
  private object _pendingValue;
  public readonly object[] Data;
  public readonly StrongBox<object>[] Closure;
  public int StackIndex;
  public int InstructionIndex;
  public ExceptionHandler CurrentAbortHandler;
  private static MethodInfo _Goto;
  private static MethodInfo _VoidGoto;

  internal InterpretedFrame(Microsoft.Scripting.Interpreter.Interpreter interpreter, StrongBox<object>[] closure)
  {
    this.Interpreter = interpreter;
    this.StackIndex = interpreter.LocalCount;
    this.Data = new object[this.StackIndex + interpreter.Instructions.MaxStackDepth];
    int continuationDepth = interpreter.Instructions.MaxContinuationDepth;
    if (continuationDepth > 0)
      this._continuations = new int[continuationDepth];
    this.Closure = closure;
  }

  public DebugInfo GetDebugInfo(int instructionIndex)
  {
    return DebugInfo.GetMatchingDebugInfo(this.Interpreter._debugInfos, instructionIndex);
  }

  public string Name => this.Interpreter._name;

  public void Push(object value) => this.Data[this.StackIndex++] = value;

  public void Push(bool value)
  {
    this.Data[this.StackIndex++] = value ? ScriptingRuntimeHelpers.True : ScriptingRuntimeHelpers.False;
  }

  public void Push(int value)
  {
    this.Data[this.StackIndex++] = ScriptingRuntimeHelpers.Int32ToObject(value);
  }

  public object Pop()
  {
    object obj = this.Data[--this.StackIndex];
    this.Data[this.StackIndex] = (object) null;
    return obj;
  }

  public object Pop(int n)
  {
    this.StackIndex -= n;
    object obj = this.Data[this.StackIndex];
    Array.Clear((Array) this.Data, this.StackIndex, n);
    return obj;
  }

  internal void SetStackDepth(int depth) => this.StackIndex = this.Interpreter.LocalCount + depth;

  public object Peek() => this.Data[this.StackIndex - 1];

  public void Dup()
  {
    int stackIndex = this.StackIndex;
    this.Data[stackIndex] = this.Data[stackIndex - 1];
    this.StackIndex = stackIndex + 1;
  }

  public InterpretedFrame Parent { get; private set; }

  public static bool IsInterpretedFrame(MethodBase method)
  {
    ContractUtils.RequiresNotNull((object) method, nameof (method));
    return method.DeclaringType == typeof (Microsoft.Scripting.Interpreter.Interpreter) && method.Name == "Run";
  }

  public static IEnumerable<StackFrame> GroupStackFrames(IEnumerable<StackFrame> stackTrace)
  {
    bool inInterpretedFrame = false;
    foreach (StackFrame stackFrame in stackTrace)
    {
      MethodBase method = stackFrame.GetMethod();
      if (!(method == (MethodBase) null))
      {
        if (InterpretedFrame.IsInterpretedFrame(method))
        {
          if (!inInterpretedFrame)
            inInterpretedFrame = true;
          else
            continue;
        }
        else
          inInterpretedFrame = false;
        yield return stackFrame;
      }
    }
  }

  public IEnumerable<InterpretedFrameInfo> GetStackTraceDebugInfo()
  {
    InterpretedFrame frame = this;
    do
    {
      yield return new InterpretedFrameInfo(frame.Name, frame.GetDebugInfo(frame.InstructionIndex));
      frame = frame.Parent;
    }
    while (frame != null);
  }

  internal void SaveTraceToException(Exception exception)
  {
    if (exception.GetData((object) typeof (InterpretedFrameInfo)) != null)
      return;
    exception.SetData((object) typeof (InterpretedFrameInfo), (object) new List<InterpretedFrameInfo>(this.GetStackTraceDebugInfo()).ToArray());
  }

  public static InterpretedFrameInfo[] GetExceptionStackTrace(Exception exception)
  {
    return exception.GetData((object) typeof (InterpretedFrameInfo)) as InterpretedFrameInfo[];
  }

  internal ThreadLocal<InterpretedFrame>.StorageInfo Enter()
  {
    ThreadLocal<InterpretedFrame>.StorageInfo storageInfo = InterpretedFrame.CurrentFrame.GetStorageInfo();
    this.Parent = storageInfo.Value;
    storageInfo.Value = this;
    return storageInfo;
  }

  internal void Leave(
    ThreadLocal<InterpretedFrame>.StorageInfo currentFrame)
  {
    currentFrame.Value = this.Parent;
  }

  public void RemoveContinuation() => --this._continuationIndex;

  public void PushContinuation(int continuation)
  {
    this._continuations[this._continuationIndex++] = continuation;
  }

  public int YieldToCurrentContinuation()
  {
    RuntimeLabel label = this.Interpreter._labels[this._continuations[this._continuationIndex - 1]];
    this.SetStackDepth(label.StackDepth);
    return label.Index - this.InstructionIndex;
  }

  public int YieldToPendingContinuation()
  {
    RuntimeLabel label1 = this.Interpreter._labels[this._pendingContinuation];
    if (label1.ContinuationStackDepth < this._continuationIndex)
    {
      RuntimeLabel label2 = this.Interpreter._labels[this._continuations[this._continuationIndex - 1]];
      this.SetStackDepth(label2.StackDepth);
      return label2.Index - this.InstructionIndex;
    }
    this.SetStackDepth(label1.StackDepth);
    if (this._pendingValue != Microsoft.Scripting.Interpreter.Interpreter.NoValue)
      this.Data[this.StackIndex - 1] = this._pendingValue;
    return label1.Index - this.InstructionIndex;
  }

  internal void PushPendingContinuation()
  {
    this.Push(this._pendingContinuation);
    this.Push(this._pendingValue);
  }

  internal void PopPendingContinuation()
  {
    this._pendingValue = this.Pop();
    this._pendingContinuation = (int) this.Pop();
  }

  internal static MethodInfo GotoMethod
  {
    get
    {
      MethodInfo methodInfo = InterpretedFrame._Goto;
      return (object) methodInfo != null ? methodInfo : (InterpretedFrame._Goto = typeof (InterpretedFrame).GetMethod("Goto"));
    }
  }

  internal static MethodInfo VoidGotoMethod
  {
    get
    {
      MethodInfo voidGoto = InterpretedFrame._VoidGoto;
      return (object) voidGoto != null ? voidGoto : (InterpretedFrame._VoidGoto = typeof (InterpretedFrame).GetMethod("VoidGoto"));
    }
  }

  public int VoidGoto(int labelIndex) => this.Goto(labelIndex, Microsoft.Scripting.Interpreter.Interpreter.NoValue);

  public int Goto(int labelIndex, object value)
  {
    RuntimeLabel label = this.Interpreter._labels[labelIndex];
    if (this._continuationIndex == label.ContinuationStackDepth)
    {
      this.SetStackDepth(label.StackDepth);
      if (value != Microsoft.Scripting.Interpreter.Interpreter.NoValue)
        this.Data[this.StackIndex - 1] = value;
      return label.Index - this.InstructionIndex;
    }
    this._pendingContinuation = labelIndex;
    this._pendingValue = value;
    return this.YieldToCurrentContinuation();
  }
}
