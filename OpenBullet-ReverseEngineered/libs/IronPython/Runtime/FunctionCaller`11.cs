// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.FunctionCaller`11
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

public sealed class FunctionCaller<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(int compat) : 
  FunctionCaller(compat)
{
  public object Call11(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8,
    T9 arg9,
    T10 arg10)
  {
    return func is PythonFunction pythonFunction && pythonFunction._compat == this._compat ? ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, (object) arg5, (object) arg6, (object) arg7, (object) arg8, (object) arg9, (object) arg10) : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
  }

  public object Default1Call11(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8,
    T9 arg9,
    T10 arg10)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 11;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, (object) arg5, (object) arg6, (object) arg7, (object) arg8, (object) arg9, (object) arg10, pythonFunction.Defaults[index]);
  }

  public object Default2Call11(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4,
    T5 arg5,
    T6 arg6,
    T7 arg7,
    T8 arg8,
    T9 arg9,
    T10 arg10)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 11;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, (object) arg5, (object) arg6, (object) arg7, (object) arg8, (object) arg9, (object) arg10, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1]);
  }
}
