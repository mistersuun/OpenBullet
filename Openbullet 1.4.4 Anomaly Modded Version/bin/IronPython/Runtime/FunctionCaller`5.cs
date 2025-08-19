// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.FunctionCaller`5
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

public sealed class FunctionCaller<T0, T1, T2, T3, T4>(int compat) : FunctionCaller(compat)
{
  public object Call5(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4)
  {
    return func is PythonFunction pythonFunction && pythonFunction._compat == this._compat ? ((Func<PythonFunction, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4) : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3, arg4);
  }

  public object Default1Call5(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3, arg4);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 5;
    return ((Func<PythonFunction, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, pythonFunction.Defaults[index]);
  }

  public object Default2Call5(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3, arg4);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 5;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1]);
  }

  public object Default3Call5(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3, arg4);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 5;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2]);
  }

  public object Default4Call5(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3, arg4);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 5;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3]);
  }

  public object Default5Call5(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3, arg4);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 5;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4]);
  }

  public object Default6Call5(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3, arg4);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 5;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4], pythonFunction.Defaults[index + 5]);
  }

  public object Default7Call5(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3, arg4);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 5;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4], pythonFunction.Defaults[index + 5], pythonFunction.Defaults[index + 6]);
  }

  public object Default8Call5(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3,
    T4 arg4)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, T4, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3, arg4);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 5;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, (object) arg4, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4], pythonFunction.Defaults[index + 5], pythonFunction.Defaults[index + 6], pythonFunction.Defaults[index + 7]);
  }
}
