// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonTracebackListener
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Modules;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Debugging;
using System;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

internal sealed class PythonTracebackListener : ITraceCallback
{
  private readonly PythonContext _pythonContext;
  private object _traceObject;
  private TracebackDelegate _traceDispatch;
  private bool _inTraceBack;
  private bool _exceptionThrown;

  internal PythonTracebackListener(PythonContext pythonContext, object traceObject)
  {
    this._pythonContext = pythonContext;
    if (traceObject == null)
      return;
    this._traceObject = traceObject;
    this._traceDispatch = (TracebackDelegate) Converter.ConvertToDelegate(traceObject, typeof (TracebackDelegate));
  }

  internal PythonContext PythonContext => this._pythonContext;

  internal object TraceObject => this._traceObject;

  internal bool InTraceBack
  {
    get => this._inTraceBack;
    set => this._inTraceBack = value;
  }

  internal bool ExceptionThrown => this._exceptionThrown;

  public void OnTraceEvent(
    TraceEventKind kind,
    string name,
    string sourceFileName,
    SourceSpan sourceSpan,
    Func<IDictionary<object, object>> scopeCallback,
    object payload,
    object customPayload)
  {
    if (kind == TraceEventKind.ThreadExit || kind == TraceEventKind.ExceptionUnwind)
      return;
    List<FunctionStack> functionStack = PythonOps.GetFunctionStack();
    if (this._inTraceBack)
      return;
    try
    {
      TracebackDelegate traceDispatch;
      object traceDispatchObject;
      TraceBackFrame traceBackFrame;
      if (kind == TraceEventKind.FrameEnter)
      {
        traceDispatch = this._traceDispatch;
        traceDispatchObject = this._traceObject;
        PythonDebuggingPayload debugProperties = (PythonDebuggingPayload) customPayload;
        traceBackFrame = new TraceBackFrame(this, debugProperties.Code, functionStack.Count == 0 ? (TraceBackFrame) null : functionStack[functionStack.Count - 1].Frame, debugProperties, scopeCallback);
        functionStack.Add(new FunctionStack(traceBackFrame));
        if (traceDispatchObject == null)
          return;
        traceBackFrame.Setf_trace(traceDispatchObject);
      }
      else
      {
        if (functionStack.Count == 0)
          return;
        traceBackFrame = functionStack[functionStack.Count - 1].Frame ?? SysModule._getframeImpl(functionStack[functionStack.Count - 1].Context, 0);
        traceDispatch = traceBackFrame.TraceDelegate;
        traceDispatchObject = traceBackFrame.Getf_trace();
      }
      if (kind != TraceEventKind.FrameExit)
        traceBackFrame._lineNo = sourceSpan.Start.Line;
      if (traceDispatchObject == null || this._exceptionThrown)
        return;
      this.DispatchTrace(functionStack, kind, payload, traceDispatch, traceDispatchObject, traceBackFrame);
    }
    finally
    {
      if (kind == TraceEventKind.FrameExit && functionStack.Count > 0 && functionStack[functionStack.Count - 1].Code == ((PythonDebuggingPayload) customPayload).Code)
        functionStack.RemoveAt(functionStack.Count - 1);
    }
  }

  private void DispatchTrace(
    List<FunctionStack> thread,
    TraceEventKind kind,
    object payload,
    TracebackDelegate traceDispatch,
    object traceDispatchObject,
    TraceBackFrame pyFrame)
  {
    object payload1 = (object) null;
    string result = string.Empty;
    switch (kind)
    {
      case TraceEventKind.FrameEnter:
        result = "call";
        break;
      case TraceEventKind.FrameExit:
        result = "return";
        payload1 = payload;
        break;
      case TraceEventKind.TracePoint:
        result = "line";
        break;
      case TraceEventKind.Exception:
        result = "exception";
        object python = PythonExceptions.ToPython((Exception) payload);
        payload1 = (object) PythonTuple.MakeTuple((object) ((IPythonObject) python).PythonType, python, (object) new TraceBack((TraceBack) null, pyFrame));
        break;
    }
    bool flag = true;
    this._inTraceBack = true;
    try
    {
      TracebackDelegate tracebackDelegate = traceDispatch(pyFrame, result, payload1);
      flag = false;
      pyFrame.Setf_trace((object) tracebackDelegate);
    }
    finally
    {
      this._inTraceBack = false;
      if (flag)
      {
        this._traceObject = (object) (this._traceDispatch = (TracebackDelegate) null);
        this._exceptionThrown = true;
      }
    }
  }
}
