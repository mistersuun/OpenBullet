// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.FunctionCaller
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

public class FunctionCaller
{
  protected readonly int _compat;

  internal FunctionCaller(int compat) => this._compat = compat;

  public object Call0(CallSite site, CodeContext context, object func)
  {
    return func is PythonFunction pythonFunction && pythonFunction._compat == this._compat ? ((Func<PythonFunction, object>) pythonFunction.func_code.Target)(pythonFunction) : ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, func);
  }

  public object Default1Call0(CallSite site, CodeContext context, object func)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, func);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount;
    return ((Func<PythonFunction, object, object>) pythonFunction.func_code.Target)(pythonFunction, pythonFunction.Defaults[index]);
  }

  public object Default2Call0(CallSite site, CodeContext context, object func)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, func);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount;
    return ((Func<PythonFunction, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1]);
  }

  public object Default3Call0(CallSite site, CodeContext context, object func)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, func);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount;
    return ((Func<PythonFunction, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2]);
  }

  public object Default4Call0(CallSite site, CodeContext context, object func)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, func);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount;
    return ((Func<PythonFunction, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3]);
  }

  public object Default5Call0(CallSite site, CodeContext context, object func)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, func);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount;
    return ((Func<PythonFunction, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4]);
  }

  public object Default6Call0(CallSite site, CodeContext context, object func)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, func);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount;
    return ((Func<PythonFunction, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4], pythonFunction.Defaults[index + 5]);
  }

  public object Default7Call0(CallSite site, CodeContext context, object func)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, func);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4], pythonFunction.Defaults[index + 5], pythonFunction.Defaults[index + 6]);
  }

  public object Default8Call0(CallSite site, CodeContext context, object func)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, func);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4], pythonFunction.Defaults[index + 5], pythonFunction.Defaults[index + 6], pythonFunction.Defaults[index + 7]);
  }

  public object Default9Call0(CallSite site, CodeContext context, object func)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, func);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4], pythonFunction.Defaults[index + 5], pythonFunction.Defaults[index + 6], pythonFunction.Defaults[index + 7], pythonFunction.Defaults[index + 8]);
  }

  public object Default10Call0(CallSite site, CodeContext context, object func)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, func);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4], pythonFunction.Defaults[index + 5], pythonFunction.Defaults[index + 6], pythonFunction.Defaults[index + 7], pythonFunction.Defaults[index + 8], pythonFunction.Defaults[index + 9]);
  }

  public object Default11Call0(CallSite site, CodeContext context, object func)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, func);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4], pythonFunction.Defaults[index + 5], pythonFunction.Defaults[index + 6], pythonFunction.Defaults[index + 7], pythonFunction.Defaults[index + 8], pythonFunction.Defaults[index + 9], pythonFunction.Defaults[index + 10]);
  }

  public object Default12Call0(CallSite site, CodeContext context, object func)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, func);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4], pythonFunction.Defaults[index + 5], pythonFunction.Defaults[index + 6], pythonFunction.Defaults[index + 7], pythonFunction.Defaults[index + 8], pythonFunction.Defaults[index + 9], pythonFunction.Defaults[index + 10], pythonFunction.Defaults[index + 11]);
  }

  public object Default13Call0(CallSite site, CodeContext context, object func)
  {
    if (!(func is PythonFunction pythonFunction) || pythonFunction._compat != this._compat)
      return ((CallSite<Func<CallSite, CodeContext, object, object>>) site).Update(site, context, func);
    int index = pythonFunction.Defaults.Length - pythonFunction.NormalArgumentCount;
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) pythonFunction.func_code.Target)(pythonFunction, pythonFunction.Defaults[index], pythonFunction.Defaults[index + 1], pythonFunction.Defaults[index + 2], pythonFunction.Defaults[index + 3], pythonFunction.Defaults[index + 4], pythonFunction.Defaults[index + 5], pythonFunction.Defaults[index + 6], pythonFunction.Defaults[index + 7], pythonFunction.Defaults[index + 8], pythonFunction.Defaults[index + 9], pythonFunction.Defaults[index + 10], pythonFunction.Defaults[index + 11], pythonFunction.Defaults[index + 12]);
  }
}
