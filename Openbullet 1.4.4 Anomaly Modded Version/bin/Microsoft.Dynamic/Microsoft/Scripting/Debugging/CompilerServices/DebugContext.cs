// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.CompilerServices.DebugContext
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

#nullable disable
namespace Microsoft.Scripting.Debugging.CompilerServices;

public sealed class DebugContext
{
  private IDebugCallback _traceHook;
  private DebugMode _debugMode;
  private readonly Microsoft.Scripting.Debugging.ThreadLocal<DebugThread> _thread;
  private DebugThread _cachedThread;
  private readonly Dictionary<string, DebugSourceFile> _sourceFiles;
  private readonly IDebugThreadFactory _threadFactory;
  private static object _debugYieldValue;

  private DebugContext(IDebugThreadFactory runtimeThreadFactory)
  {
    this._thread = new Microsoft.Scripting.Debugging.ThreadLocal<DebugThread>();
    this._sourceFiles = new Dictionary<string, DebugSourceFile>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    this._threadFactory = runtimeThreadFactory;
  }

  public static DebugContext CreateInstance()
  {
    return new DebugContext((IDebugThreadFactory) new DefaultDebugThreadFactory());
  }

  internal static DebugContext CreateInstance(IDebugThreadFactory runtimeThreadFactory)
  {
    return new DebugContext(runtimeThreadFactory);
  }

  public LambdaExpression TransformLambda(LambdaExpression lambda, DebugLambdaInfo lambdaInfo)
  {
    ContractUtils.RequiresNotNull((object) lambda, nameof (lambda));
    ContractUtils.RequiresNotNull((object) lambdaInfo, nameof (lambdaInfo));
    return new DebuggableLambdaBuilder(this, lambdaInfo).Transform(lambda);
  }

  public LambdaExpression TransformLambda(LambdaExpression lambda)
  {
    ContractUtils.RequiresNotNull((object) lambda, nameof (lambda));
    return new DebuggableLambdaBuilder(this, new DebugLambdaInfo((IDebugCompilerSupport) null, (string) null, false, (IList<ParameterExpression>) null, (IDictionary<ParameterExpression, string>) null, (object) null)).Transform(lambda);
  }

  public void ResetSourceFile(string sourceFileName)
  {
    ContractUtils.RequiresNotNull((object) sourceFileName, nameof (sourceFileName));
    this._sourceFiles.Remove(sourceFileName);
  }

  [Obsolete("do not call this property", true)]
  public int Mode => (int) this._debugMode;

  internal DebugMode DebugMode
  {
    get => this._debugMode;
    set
    {
      this._debugMode = value;
      foreach (DebugSourceFile debugSourceFile in this._sourceFiles.Values)
        debugSourceFile.DebugMode = value;
    }
  }

  internal DebugSourceFile Lookup(string sourceFile)
  {
    DebugSourceFile debugSourceFile;
    return this._sourceFiles.TryGetValue(sourceFile, out debugSourceFile) ? debugSourceFile : (DebugSourceFile) null;
  }

  internal IEnumerable<DebugThread> Threads
  {
    get
    {
      DebugThread[] debugThreadArray = this._thread.Values;
      for (int index = 0; index < debugThreadArray.Length; ++index)
      {
        DebugThread thread = debugThreadArray[index];
        if (thread != null && thread.FrameCount > 0)
          yield return thread;
      }
      debugThreadArray = (DebugThread[]) null;
    }
  }

  internal IDebugCallback DebugCallback
  {
    get => this._traceHook;
    set => this._traceHook = value;
  }

  internal DebugSourceFile GetDebugSourceFile(string sourceFile)
  {
    DebugSourceFile debugSourceFile;
    lock (((ICollection) this._sourceFiles).SyncRoot)
    {
      if (!this._sourceFiles.TryGetValue(sourceFile, out debugSourceFile))
      {
        debugSourceFile = new DebugSourceFile(sourceFile, this._debugMode);
        this._sourceFiles.Add(sourceFile, debugSourceFile);
      }
    }
    return debugSourceFile;
  }

  internal static FunctionInfo CreateFunctionInfo(
    Delegate generatorFactory,
    string name,
    DebugSourceSpan[] locationSpanMap,
    IList<VariableInfo>[] scopedVariables,
    IList<VariableInfo> variables,
    object customPayload)
  {
    FunctionInfo functionInfo = new FunctionInfo(generatorFactory, name, locationSpanMap, scopedVariables, variables, customPayload);
    foreach (DebugSourceSpan locationSpan in locationSpanMap)
    {
      lock (locationSpan.SourceFile.FunctionInfoMap)
        locationSpan.SourceFile.FunctionInfoMap[locationSpan] = functionInfo;
    }
    return functionInfo;
  }

  internal DebugFrame CreateFrameForGenerator(FunctionInfo func)
  {
    return new DebugFrame(this.GetCurrentThread(), func);
  }

  internal void DispatchDebugEvent(
    DebugThread thread,
    int debugMarker,
    TraceEventKind eventKind,
    object payload)
  {
    DebugFrame frame = (DebugFrame) null;
    bool flag = false;
    int stackDepth;
    FunctionInfo functionInfo;
    if (eventKind != TraceEventKind.ThreadExit)
    {
      functionInfo = thread.GetLeafFrameFunctionInfo(out stackDepth);
    }
    else
    {
      stackDepth = int.MaxValue;
      functionInfo = (FunctionInfo) null;
    }
    if (eventKind == TraceEventKind.Exception || eventKind == TraceEventKind.ExceptionUnwind)
      thread.ThrownException = (Exception) payload;
    thread.IsInTraceback = true;
    try
    {
      this._traceHook?.OnDebugEvent(eventKind, thread, functionInfo, debugMarker, stackDepth, payload);
      flag = thread.TryGetLeafFrame(ref frame);
      if (flag && frame.ForceSwitchToGeneratorLoop && !frame.InGeneratorLoop)
        throw new ForceToGeneratorLoopException();
    }
    finally
    {
      if (flag)
        frame.IsInTraceback = false;
      thread.IsInTraceback = false;
      thread.ThrownException = (Exception) null;
    }
  }

  internal IDebugThreadFactory ThreadFactory => this._threadFactory;

  internal DebugThread GetCurrentThread()
  {
    DebugThread currentThread = this._cachedThread;
    if (currentThread == null || !currentThread.IsCurrentThread)
    {
      currentThread = this._thread.Value;
      if (currentThread == null)
      {
        currentThread = this._threadFactory.CreateDebugThread(this);
        this._thread.Value = currentThread;
      }
      Interlocked.Exchange<DebugThread>(ref this._cachedThread, currentThread);
    }
    return currentThread;
  }

  internal static object DebugYieldValue
  {
    get
    {
      if (DebugContext._debugYieldValue == null)
        DebugContext._debugYieldValue = new object();
      return DebugContext._debugYieldValue;
    }
  }

  internal object GeneratorLoopProc(DebugFrame frame, out bool moveNext)
  {
    moveNext = true;
    bool flag = true;
    if (frame.ForceSwitchToGeneratorLoop)
      frame.ForceSwitchToGeneratorLoop = false;
    object obj;
    while (true)
    {
      if (!flag)
      {
        if (frame.FunctionInfo.SequencePoints[frame.CurrentLocationCookie].SourceFile.DebugMode == DebugMode.FullyEnabled || frame.FunctionInfo.SequencePoints[frame.CurrentLocationCookie].SourceFile.DebugMode == DebugMode.TracePoints && frame.FunctionInfo.GetTraceLocations()[frame.CurrentLocationCookie])
        {
          frame.InGeneratorLoop = true;
          try
          {
            this.DispatchDebugEvent(frame.Thread, frame.CurrentLocationCookie, TraceEventKind.TracePoint, (object) null);
          }
          finally
          {
            frame.InGeneratorLoop = false;
          }
        }
      }
      else
        flag = false;
      try
      {
        moveNext = ((IEnumerator) frame.Generator).MoveNext();
        object current = ((IEnumerator) frame.Generator).Current;
        if (frame.Generator.YieldMarkerLocation != int.MaxValue)
          frame.LastKnownGeneratorYieldMarker = frame.Generator.YieldMarkerLocation;
        if (current == DebugContext.DebugYieldValue)
        {
          if (moveNext)
            continue;
        }
        obj = !moveNext ? (object) null : current;
        break;
      }
      catch (ForceToGeneratorLoopException ex)
      {
        flag = true;
      }
      catch (Exception ex)
      {
        if (frame.DebugContext.DebugMode != DebugMode.Disabled)
        {
          try
          {
            frame.InGeneratorLoop = true;
            this.DispatchDebugEvent(frame.Thread, frame.CurrentLocationCookie, TraceEventKind.ExceptionUnwind, (object) ex);
          }
          finally
          {
            frame.InGeneratorLoop = false;
          }
          if (frame.ThrownException != null)
            throw;
          flag = true;
        }
        else
          throw;
      }
    }
    return obj;
  }
}
