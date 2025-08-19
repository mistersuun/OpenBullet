// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.RuntimeOps
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Debugging.CompilerServices;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Debugging;

public static class RuntimeOps
{
  [Obsolete("do not call this method", true)]
  public static DebugFrame CreateFrameForGenerator(DebugContext debugContext, FunctionInfo func)
  {
    return debugContext.CreateFrameForGenerator(func);
  }

  [Obsolete("do not call this method", true)]
  public static bool PopFrame(DebugThread thread) => thread.PopFrame();

  [Obsolete("do not call this method", true)]
  public static void OnTraceEvent(DebugThread thread, int debugMarker, Exception exception)
  {
    thread.DebugContext.DispatchDebugEvent(thread, debugMarker, exception != null ? TraceEventKind.Exception : TraceEventKind.TracePoint, (object) exception);
  }

  [Obsolete("do not call this method", true)]
  public static void OnTraceEventUnwind(DebugThread thread, int debugMarker, Exception exception)
  {
    thread.DebugContext.DispatchDebugEvent(thread, debugMarker, TraceEventKind.ExceptionUnwind, (object) exception);
  }

  [Obsolete("do not call this method", true)]
  public static void OnFrameEnterTraceEvent(DebugThread thread)
  {
    thread.DebugContext.DispatchDebugEvent(thread, 0, TraceEventKind.FrameEnter, (object) null);
  }

  [Obsolete("do not call this method", true)]
  public static void OnFrameExitTraceEvent(DebugThread thread, int debugMarker, object retVal)
  {
    thread.DebugContext.DispatchDebugEvent(thread, debugMarker, TraceEventKind.FrameExit, retVal);
  }

  [Obsolete("do not call this method", true)]
  public static void OnThreadExitEvent(DebugThread thread)
  {
    thread.DebugContext.DispatchDebugEvent(thread, int.MaxValue, TraceEventKind.ThreadExit, (object) null);
  }

  [Obsolete("do not call this method", true)]
  public static void ReplaceLiftedLocals(DebugFrame frame, IRuntimeVariables liftedLocals)
  {
    frame.ReplaceLiftedLocals(liftedLocals);
  }

  [Obsolete("do not call this method", true)]
  public static object GeneratorLoopProc(DebugThread thread)
  {
    return thread.DebugContext.GeneratorLoopProc(thread.GetLeafFrame(), out bool _);
  }

  [Obsolete("do not call this method", true)]
  public static IEnumerator<T> CreateDebugGenerator<T>(DebugFrame frame)
  {
    return (IEnumerator<T>) new DebugGenerator<T>(frame);
  }

  [Obsolete("do not call this method", true)]
  public static int GetCurrentSequencePointForGeneratorFrame(DebugFrame frame)
  {
    return frame.CurrentLocationCookie;
  }

  [Obsolete("do not call this method", true)]
  public static int GetCurrentSequencePointForLeafGeneratorFrame(DebugThread thread)
  {
    return thread.GetLeafFrame().CurrentLocationCookie;
  }

  [Obsolete("do not call this method", true)]
  public static bool IsCurrentLeafFrameRemappingToGenerator(DebugThread thread)
  {
    DebugFrame frame = (DebugFrame) null;
    return thread.TryGetLeafFrame(ref frame) && frame.ForceSwitchToGeneratorLoop;
  }

  [Obsolete("do not call this method", true)]
  public static FunctionInfo CreateFunctionInfo(
    Delegate generatorFactory,
    string name,
    object locationSpanMap,
    object scopedVariables,
    object variables,
    object customPayload)
  {
    return DebugContext.CreateFunctionInfo(generatorFactory, name, (DebugSourceSpan[]) locationSpanMap, (IList<VariableInfo>[]) scopedVariables, (IList<VariableInfo>) variables, customPayload);
  }

  [Obsolete("do not call this method", true)]
  public static DebugThread GetCurrentThread(DebugContext debugContext)
  {
    return debugContext.GetCurrentThread();
  }

  [Obsolete("do not call this method", true)]
  public static DebugThread GetThread(DebugFrame frame) => frame.Thread;

  [Obsolete("do not call this method", true)]
  public static bool[] GetTraceLocations(FunctionInfo functionInfo)
  {
    return functionInfo.GetTraceLocations();
  }

  [Obsolete("do not call this method", true)]
  public static void LiftVariables(DebugThread thread, IRuntimeVariables runtimeVariables)
  {
    ((DefaultDebugThread) thread).LiftVariables(runtimeVariables);
  }
}
