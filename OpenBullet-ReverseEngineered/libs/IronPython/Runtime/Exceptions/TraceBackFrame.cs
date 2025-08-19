// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Exceptions.TraceBackFrame
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Exceptions;

[PythonType("frame")]
[DebuggerDisplay("Code = {f_code.co_name}, Line = {f_lineno}")]
[Serializable]
public class TraceBackFrame
{
  private readonly PythonTracebackListener _traceAdapter;
  private TracebackDelegate _trace;
  private object _traceObject;
  internal int _lineNo;
  private readonly PythonDebuggingPayload _debugProperties;
  private readonly Func<IDictionary<object, object>> _scopeCallback;
  private readonly PythonDictionary _globals;
  private readonly object _locals;
  private readonly FunctionCode _code;
  private readonly CodeContext _context;
  private readonly TraceBackFrame _back;

  internal TraceBackFrame(
    CodeContext context,
    PythonDictionary globals,
    object locals,
    FunctionCode code)
  {
    this._globals = globals;
    this._locals = locals;
    this._code = code;
    this._context = context;
  }

  internal TraceBackFrame(
    CodeContext context,
    PythonDictionary globals,
    object locals,
    FunctionCode code,
    TraceBackFrame back)
  {
    this._globals = globals;
    this._locals = locals;
    this._code = code;
    this._context = context;
    this._back = back;
  }

  internal TraceBackFrame(
    PythonTracebackListener traceAdapter,
    FunctionCode code,
    TraceBackFrame back,
    PythonDebuggingPayload debugProperties,
    Func<IDictionary<object, object>> scopeCallback)
  {
    this._traceAdapter = traceAdapter;
    this._code = code;
    this._back = back;
    this._debugProperties = debugProperties;
    this._scopeCallback = scopeCallback;
  }

  [PropertyMethod]
  [SpecialName]
  public object Getf_trace() => this._traceAdapter != null ? this._traceObject : (object) null;

  [PropertyMethod]
  [SpecialName]
  public void Setf_trace(object value)
  {
    this._traceObject = value;
    this._trace = (TracebackDelegate) Converter.ConvertToDelegate(value, typeof (TracebackDelegate));
  }

  [PropertyMethod]
  [SpecialName]
  public void Deletef_trace() => this.Setf_trace((object) null);

  internal CodeContext Context => this._context;

  internal TracebackDelegate TraceDelegate
  {
    get => this._traceAdapter != null ? this._trace : (TracebackDelegate) null;
  }

  public PythonDictionary f_globals
  {
    get
    {
      object obj;
      return this._scopeCallback != null && this._scopeCallback().TryGetValue((object) "$globalContext", out obj) && obj != null ? ((CodeContext) obj).GlobalDict : this._globals;
    }
  }

  public object f_locals
  {
    get
    {
      if (this._traceAdapter == null || this._scopeCallback == null)
        return this._locals;
      return this._code.IsModule ? (object) this.f_globals : (object) new PythonDictionary((DictionaryStorage) new DebuggerDictionaryStorage(this._scopeCallback()));
    }
  }

  public FunctionCode f_code => this._code;

  public object f_builtins => (object) this._context.LanguageContext.BuiltinModuleDict;

  public TraceBackFrame f_back => this._back;

  public object f_exc_traceback
  {
    [Python3Warning("f_exc_traceback has been removed in 3.x")] get => (object) null;
  }

  public object f_exc_type
  {
    [Python3Warning("f_exc_type has been removed in 3.x")] get => (object) null;
  }

  public bool f_restricted => false;

  public object f_lineno
  {
    get => this._traceAdapter != null ? (object) this._lineNo : (object) 1;
    set
    {
      if (!(value is int newLineNum))
        throw PythonOps.ValueError("lineno must be an integer");
      this.SetLineNumber(newLineNum);
    }
  }

  private void SetLineNumber(int newLineNum)
  {
    List<FunctionStack> functionStackNoCreate = PythonOps.GetFunctionStackNoCreate();
    if (this._traceAdapter == null || !this.IsTopMostFrame(functionStackNoCreate))
      throw PythonOps.ValueError("f_lineno can only be set by a trace function on the topmost frame");
    FunctionCode code = this._debugProperties.Code;
    Dictionary<int, Dictionary<int, bool>> finallyLocations = this._debugProperties.LoopAndFinallyLocations;
    Dictionary<int, bool> handlerLocations = this._debugProperties.HandlerLocations;
    Dictionary<int, bool> jumpIntoLoopIds1 = (Dictionary<int, bool>) null;
    bool flag1 = finallyLocations != null && finallyLocations.TryGetValue(this._lineNo, out jumpIntoLoopIds1);
    int num1 = newLineNum;
    if (newLineNum < code.Span.Start.Line)
      throw PythonOps.ValueError("line {0} comes before the current code block", (object) newLineNum);
    int num2 = newLineNum;
    SourceSpan span = code.Span;
    int line1 = span.End.Line;
    if (num2 > line1)
      throw PythonOps.ValueError("line {0} comes after the current code block", (object) newLineNum);
    Dictionary<int, bool> jumpIntoLoopIds2;
    while (true)
    {
      int num3 = newLineNum;
      span = code.Span;
      int line2 = span.End.Line;
      if (num3 <= line2)
      {
        SourceSpan sourceSpan = new SourceSpan(new SourceLocation(0, newLineNum, 1), new SourceLocation(0, newLineNum, int.MaxValue));
        if (handlerLocations == null || !handlerLocations.TryGetValue(newLineNum, out bool _))
        {
          if (finallyLocations != null && finallyLocations.TryGetValue(newLineNum, out jumpIntoLoopIds2))
          {
            if (flag1)
            {
              foreach (int key in jumpIntoLoopIds2.Keys)
              {
                if (!jumpIntoLoopIds1.ContainsKey(key))
                  throw TraceBackFrame.BadForOrFinallyJump(newLineNum, jumpIntoLoopIds1);
              }
            }
            else
              goto label_10;
          }
          else if (jumpIntoLoopIds1 != null)
          {
            foreach (bool flag2 in jumpIntoLoopIds1.Values)
            {
              if (flag2)
                throw PythonOps.ValueError("can't jump out of 'finally block'");
            }
          }
          if (!this._traceAdapter.PythonContext.TracePipeline.TrySetNextStatement(this._code.co_filename, sourceSpan))
            ++newLineNum;
          else
            goto label_25;
        }
        else
          break;
      }
      else
        goto label_28;
    }
    throw PythonOps.ValueError("can't jump to 'except' line");
label_10:
    throw TraceBackFrame.BadForOrFinallyJump(newLineNum, jumpIntoLoopIds2);
label_25:
    this._lineNo = newLineNum;
    return;
label_28:
    object[] objArray = new object[3]
    {
      (object) num1,
      null,
      null
    };
    span = code.Span;
    objArray[1] = (object) span.Start.Line;
    span = code.Span;
    objArray[2] = (object) span.End.Line;
    throw PythonOps.ValueError("line {0} is invalid jump location ({1} - {2} are valid)", objArray);
  }

  private bool IsTopMostFrame(List<FunctionStack> pyThread)
  {
    return pyThread != null && pyThread.Count != 0 && pyThread[pyThread.Count - 1].Frame == this;
  }

  private static Exception BadForOrFinallyJump(
    int newLineNum,
    Dictionary<int, bool> jumpIntoLoopIds)
  {
    foreach (bool flag in jumpIntoLoopIds.Values)
    {
      if (flag)
        return PythonOps.ValueError("can't jump into 'finally block'", (object) newLineNum);
    }
    return PythonOps.ValueError("can't jump into 'for loop'", (object) newLineNum);
  }
}
