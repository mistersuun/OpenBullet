// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonGenerator
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace IronPython.Runtime;

[PythonType("generator")]
[DontMapIDisposableToContextManager]
[DontMapIEnumerableToContains]
public sealed class PythonGenerator : 
  IEnumerator,
  IEnumerator<object>,
  IDisposable,
  ICodeFormattable,
  IEnumerable,
  IWeakReferenceable
{
  private readonly Func<MutableTuple, object> _next;
  private readonly PythonFunction _function;
  private readonly MutableTuple _data;
  private readonly MutableTuple<int, object> _dataTuple;
  private PythonGenerator.GeneratorFlags _flags;
  private bool _active;
  private PythonGenerator.GeneratorFinalizer _finalizer;
  private static PythonGenerator.GeneratorFinalizer _LastFinalizer;
  private object[] _excInfo;
  private object _sendValue;
  private WeakRefTracker _tracker;
  private FunctionStack? fnStack;

  internal PythonGenerator(
    PythonFunction function,
    Func<MutableTuple, object> next,
    MutableTuple data)
  {
    this._function = function;
    this._next = next;
    this._data = data;
    this._dataTuple = this.GetDataTuple();
    this.State = -1;
    if (PythonGenerator._LastFinalizer == null || (this._finalizer = Interlocked.Exchange<PythonGenerator.GeneratorFinalizer>(ref PythonGenerator._LastFinalizer, (PythonGenerator.GeneratorFinalizer) null)) == null)
      this._finalizer = new PythonGenerator.GeneratorFinalizer(this);
    else
      this._finalizer.Generator = this;
  }

  [LightThrowing]
  public object next()
  {
    if (this.Closed)
      return LightExceptions.Throw(PythonOps.StopIteration());
    object obj = this.NextWorker();
    return obj == OperationFailed.Value ? LightExceptions.Throw(PythonOps.StopIteration()) : obj;
  }

  [LightThrowing]
  public object @throw(object type) => this.@throw(type, (object) null, (object) null, false);

  [LightThrowing]
  public object @throw(object type, object value) => this.@throw(type, value, (object) null, false);

  [LightThrowing]
  public object @throw(object type, object value, object traceback)
  {
    return this.@throw(type, value, traceback, false);
  }

  [LightThrowing]
  private object @throw(object type, object value, object traceback, bool finalizing)
  {
    if (type == null)
      throw PythonOps.MakeExceptionTypeError((object) null, true);
    this._excInfo = new object[3]{ type, value, traceback };
    if (this.Closed)
    {
      object obj = this.CheckThrowable();
      if (obj != null)
        return obj;
    }
    return finalizing || !((IEnumerator) this).MoveNext() ? LightExceptions.Throw(PythonOps.StopIteration()) : this.CurrentValue;
  }

  [LightThrowing]
  public object send(object value)
  {
    this._sendValue = value == null || this.State != -1 ? value : throw PythonOps.TypeErrorForIllegalSend();
    return this.next();
  }

  [LightThrowing]
  public object close() => this.close(false);

  [LightThrowing]
  private object close(bool finalizing)
  {
    if (this.Closed)
      return (object) null;
    try
    {
      Exception lightException = LightExceptions.GetLightException(this.@throw((object) new GeneratorExitException(), (object) null, (object) null, finalizing));
      switch (lightException)
      {
        case null:
          return LightExceptions.Throw((Exception) new RuntimeException("generator ignored GeneratorExit"));
        case StopIterationException _:
        case GeneratorExitException _:
          return (object) null;
        default:
          return (object) lightException;
      }
    }
    catch (StopIterationException ex)
    {
    }
    catch (GeneratorExitException ex)
    {
    }
    return (object) null;
  }

  public FunctionCode gi_code => this._function.func_code;

  public int gi_running => this.Active ? 1 : 0;

  public TraceBackFrame gi_frame
  {
    get
    {
      return new TraceBackFrame(this._function.Context, this._function.Context.GlobalDict, (object) new PythonDictionary(), this.gi_code);
    }
  }

  public string __name__ => this._function.__name__;

  private int State
  {
    get => this._dataTuple.Item000;
    set => this._dataTuple.Item000 = value;
  }

  private object CurrentValue
  {
    get => this._dataTuple.Item001;
    set => this._dataTuple.Item001 = value;
  }

  private MutableTuple<int, object> GetDataTuple()
  {
    if (!(this._data is MutableTuple<int, object> dataTuple))
      dataTuple = PythonGenerator.GetBigData(this._data);
    return dataTuple;
  }

  private static MutableTuple<int, object> GetBigData(MutableTuple data)
  {
    do
    {
      data = (MutableTuple) data.GetValue(0);
    }
    while (!(data is MutableTuple<int, object> bigData));
    return bigData;
  }

  internal CodeContext Context => this._function.Context;

  internal PythonFunction Function => this._function;

  private void Finalizer()
  {
    if (!this.CanSetSysExcInfo && !this.ContainsTryFinally)
      return;
    try
    {
      Exception lightException = LightExceptions.GetLightException(this.close(true));
      if (lightException == null)
        return;
      this.HandleFinalizerException(lightException);
    }
    catch (Exception ex)
    {
      this.HandleFinalizerException(ex);
    }
  }

  private void HandleFinalizerException(Exception e)
  {
    try
    {
      PythonOps.PrintWithDest(this.Context, this.Context.LanguageContext.SystemStandardError, (object) $"Exception in generator {this.__repr__(this.Context)} ignored");
      PythonOps.PrintWithDest(this.Context, this.Context.LanguageContext.SystemStandardError, (object) this.Context.LanguageContext.FormatException(e));
    }
    catch
    {
    }
  }

  bool IEnumerator.MoveNext()
  {
    if (this.Closed)
      return false;
    this.CheckSetActive();
    if (!this.CanSetSysExcInfo)
      return this.MoveNextWorker();
    Exception save = this.SaveCurrentException();
    try
    {
      return this.MoveNextWorker();
    }
    finally
    {
      this.RestoreCurrentException(save);
    }
  }

  private void SaveFunctionStack(bool done)
  {
    if (!this.Context.LanguageContext.PythonOptions.Frames || this.Context.LanguageContext.EnableTracing)
      return;
    if (!done)
    {
      List<FunctionStack> functionStack1 = PythonOps.GetFunctionStack();
      FunctionStack functionStack2 = functionStack1[functionStack1.Count - 1] with
      {
        Frame = (TraceBackFrame) null
      };
      functionStack1.RemoveAt(functionStack1.Count - 1);
      this.fnStack = new FunctionStack?(functionStack2);
    }
    else
      this.fnStack = new FunctionStack?();
  }

  private void RestoreFunctionStack()
  {
    if (!this.Context.LanguageContext.PythonOptions.Frames || !this.fnStack.HasValue)
      return;
    PythonOps.GetFunctionStack().Add(this.fnStack.Value);
  }

  private bool MoveNextWorker()
  {
    bool flag = false;
    try
    {
      this.RestoreFunctionStack();
      try
      {
        flag = this.GetNext();
        return flag;
      }
      finally
      {
        this.Active = false;
        this.SaveFunctionStack(!flag);
        if (!flag)
          this.Close();
      }
    }
    catch (StopIterationException ex)
    {
      return false;
    }
  }

  private object NextWorker()
  {
    this.CheckSetActive();
    Exception save = this.SaveCurrentException();
    this.RestoreFunctionStack();
    bool flag = false;
    try
    {
      if (!(flag = this.GetNext()))
        this.CurrentValue = (object) OperationFailed.Value;
    }
    finally
    {
      this.RestoreCurrentException(save);
      this.Active = false;
      this.SaveFunctionStack(!flag);
      if (!flag)
        this.Close();
    }
    return this.CurrentValue;
  }

  private void RestoreCurrentException(Exception save)
  {
    if (!this.CanSetSysExcInfo)
      return;
    PythonOps.RestoreCurrentException(save);
  }

  private Exception SaveCurrentException()
  {
    return this.CanSetSysExcInfo ? PythonOps.SaveCurrentException() : (Exception) null;
  }

  private void CheckSetActive()
  {
    if (this.Active)
      PythonGenerator.AlreadyExecuting();
    this.Active = true;
  }

  private static void AlreadyExecuting()
  {
    throw PythonOps.ValueError("generator already executing");
  }

  [LightThrowing]
  internal object CheckThrowableAndReturnSendValue()
  {
    return this._sendValue != null ? this.SwapValues() : this.CheckThrowable();
  }

  private object SwapValues()
  {
    object sendValue = this._sendValue;
    this._sendValue = (object) null;
    return sendValue;
  }

  [LightThrowing]
  private object CheckThrowable() => this._excInfo != null ? this.ThrowThrowable() : (object) null;

  [LightThrowing]
  private object ThrowThrowable()
  {
    object[] excInfo = this._excInfo;
    this._excInfo = (object[]) null;
    return LightExceptions.Throw(PythonOps.MakeExceptionForGenerator(this.Context, excInfo[0], excInfo[1], excInfo[2]));
  }

  private void Close()
  {
    this.Closed = true;
    this.SuppressFinalize();
  }

  private void SuppressFinalize()
  {
    if (this._finalizer == null)
      return;
    this._finalizer.Generator = (PythonGenerator) null;
    PythonGenerator._LastFinalizer = this._finalizer;
  }

  private bool Closed
  {
    get => (this._flags & PythonGenerator.GeneratorFlags.Closed) != 0;
    set
    {
      if (value)
        this._flags |= PythonGenerator.GeneratorFlags.Closed;
      else
        this._flags &= ~PythonGenerator.GeneratorFlags.Closed;
    }
  }

  private bool Active
  {
    get => this._active;
    set => this._active = value;
  }

  private bool GetNext()
  {
    object obj = this._next(this._data);
    return this.State != 0;
  }

  internal bool CanSetSysExcInfo
  {
    get => (this._function.Flags & FunctionAttributes.CanSetSysExcInfo) != 0;
  }

  internal bool ContainsTryFinally
  {
    get => (this._function.Flags & FunctionAttributes.ContainsTryFinally) != 0;
  }

  public string __repr__(CodeContext context)
  {
    return $"<generator object at {PythonOps.HexId((object) this)}>";
  }

  object IEnumerator.Current => this.CurrentValue;

  void IEnumerator.Reset() => throw new NotImplementedException();

  object IEnumerator<object>.Current => this.CurrentValue;

  void IDisposable.Dispose() => this.SuppressFinalize();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this;

  WeakRefTracker IWeakReferenceable.GetWeakRef() => this._tracker;

  bool IWeakReferenceable.SetWeakRef(WeakRefTracker value)
  {
    this._tracker = value;
    return true;
  }

  void IWeakReferenceable.SetFinalizer(WeakRefTracker value) => this._tracker = value;

  [Flags]
  private enum GeneratorFlags
  {
    None = 0,
    Closed = 1,
    CanSetSysExcInfo = 4,
  }

  private class GeneratorFinalizer
  {
    public PythonGenerator Generator;

    public GeneratorFinalizer(PythonGenerator generator) => this.Generator = generator;

    ~GeneratorFinalizer()
    {
      PythonGenerator generator = this.Generator;
      if (generator == null)
        return;
      generator._finalizer = (PythonGenerator.GeneratorFinalizer) null;
      generator.Finalizer();
    }
  }
}
