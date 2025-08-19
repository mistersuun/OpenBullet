// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.FunctionCaller`4
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

public sealed class FunctionCaller<T0, T1, T2, T3>(int compat) : FunctionCaller(compat)
{
  public object Call4(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3)
  {
    return func is PythonFunction pythonFunction && pythonFunction._compat == this._compat ? ((Func<PythonFunction, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3) : ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3);
  }

  public object Default1Call4(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 4;
    return ((Func<PythonFunction, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, pythonFunction.Defaults[index]);
  }

  public object Default2Call4(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 4;
    return ((Func<PythonFunction, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1]);
  }

  public object Default3Call4(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 4;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2]);
  }

  public object Default4Call4(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 4;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3]);
  }

  public object Default5Call4(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 4;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4]);
  }

  public object Default6Call4(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 4;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4], pythonFunction.Defaults[index + 5]);
  }

  public object Default7Call4(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 4;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4], pythonFunction.Defaults[index + 5], pythonFunction.Defaults[index + 6]);
  }

  public object Default8Call4(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 4;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4], pythonFunction.Defaults[index + 5], pythonFunction.Defaults[index + 6], pythonFunction.Defaults[index + 7]);
  }

  public object Default9Call4(
    CallSite site,
    CodeContext context,
    object func,
    T0 arg0,
    T1 arg1,
    T2 arg2,
    T3 arg3)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, T0, T1, T2, T3, object>>) site).Update(site, context, func, arg0, arg1, arg2, arg3);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount + 4;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, (object) arg0, (object) arg1, (object) arg2, (object) arg3, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4], pythonFunction.Defaults[index + 5], pythonFunction.Defaults[index + 6], pythonFunction.Defaults[index + 7], pythonFunction.Defaults[index + 8]);
  }
}
