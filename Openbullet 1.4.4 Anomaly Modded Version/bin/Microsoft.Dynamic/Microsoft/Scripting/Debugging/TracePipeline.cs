// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.TracePipeline
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Debugging.CompilerServices;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Debugging;

public sealed class TracePipeline : ITracePipeline, IDebugCallback
{
  private readonly DebugContext _debugContext;
  private readonly ThreadLocal<DebugFrame> _traceFrame = new ThreadLocal<DebugFrame>();
  private ITraceCallback _traceCallback;
  private bool _closed;

  private TracePipeline(DebugContext debugContext)
  {
    this._debugContext = debugContext;
    debugContext.DebugCallback = (IDebugCallback) this;
    debugContext.DebugMode = DebugMode.FullyEnabled;
  }

  public static TracePipeline CreateInstance(DebugContext debugContext)
  {
    ContractUtils.RequiresNotNull((object) debugContext, nameof (debugContext));
    return debugContext.DebugCallback == null ? new TracePipeline(debugContext) : throw new InvalidOperationException(ErrorStrings.DebugContextAlreadyConnectedToTracePipeline);
  }

  public void Close()
  {
    this.VerifyNotClosed();
    this._debugContext.DebugCallback = (IDebugCallback) null;
    this._debugContext.DebugMode = DebugMode.Disabled;
    this._closed = true;
  }

  public bool TrySetNextStatement(string sourceFile, SourceSpan sourceSpan)
  {
    this.VerifyNotClosed();
    ContractUtils.RequiresNotNull((object) sourceFile, nameof (sourceFile));
    ContractUtils.Requires(sourceSpan != SourceSpan.Invalid && sourceSpan != SourceSpan.None, ErrorStrings.InvalidSourceSpan);
    DebugFrame frame = this._traceFrame.Value;
    if (frame == null)
      return false;
    int indexForSourceSpan = this.GetSequencePointIndexForSourceSpan(sourceFile, sourceSpan, frame);
    if (indexForSourceSpan == int.MaxValue)
      return false;
    frame.CurrentSequencePointIndex = indexForSourceSpan;
    return true;
  }

  public ITraceCallback TraceCallback
  {
    get
    {
      this.VerifyNotClosed();
      return this._traceCallback;
    }
    set
    {
      this.VerifyNotClosed();
      this._traceCallback = value;
    }
  }

  void IDebugCallback.OnDebugEvent(
    TraceEventKind kind,
    DebugThread thread,
    FunctionInfo functionInfo,
    int sequencePointIndex,
    int stackDepth,
    object payload)
  {
    ITraceCallback traceCallback = this._traceCallback;
    if (traceCallback == null)
      return;
    DebugFrame debugFrame = this._traceFrame.Value;
    try
    {
      if (kind == TraceEventKind.FrameExit || kind == TraceEventKind.ThreadExit)
      {
        traceCallback.OnTraceEvent(kind, kind == TraceEventKind.FrameExit ? functionInfo.Name : (string) null, (string) null, SourceSpan.None, (Func<IDictionary<object, object>>) null, payload, functionInfo?.CustomPayload);
      }
      else
      {
        DebugFrame leafFrame = thread.GetLeafFrame();
        this._traceFrame.Value = leafFrame;
        DebugSourceSpan sequencePoint = functionInfo.SequencePoints[sequencePointIndex];
        traceCallback.OnTraceEvent(kind, functionInfo.Name, sequencePoint.SourceFile.Name, sequencePoint.ToDlrSpan(), (Func<IDictionary<object, object>>) (() => leafFrame.GetLocalsScope()), payload, functionInfo.CustomPayload);
      }
    }
    finally
    {
      this._traceFrame.Value = debugFrame;
    }
  }

  private int GetSequencePointIndexForSourceSpan(
    string sourceFile,
    SourceSpan sourceSpan,
    DebugFrame frame)
  {
    DebugSourceFile sourceFile1 = this._debugContext.Lookup(sourceFile);
    if (sourceFile1 == null)
      return int.MaxValue;
    DebugSourceSpan span = new DebugSourceSpan(sourceFile1, sourceSpan);
    FunctionInfo functionInfo = frame.FunctionInfo;
    FunctionInfo funcInfo = sourceFile1.LookupFunctionInfo(span);
    return funcInfo != functionInfo ? int.MaxValue : span.GetSequencePointIndex(funcInfo);
  }

  private void VerifyNotClosed()
  {
    if (this._closed)
      throw new InvalidOperationException(ErrorStrings.ITracePipelineClosed);
  }
}
