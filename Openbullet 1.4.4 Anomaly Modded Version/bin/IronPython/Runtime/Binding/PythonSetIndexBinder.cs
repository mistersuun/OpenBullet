// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonSetIndexBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.ComInterop;
using Microsoft.Scripting.Runtime;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class PythonSetIndexBinder : SetIndexBinder, IPythonSite, IExpressionSerializable
{
  private readonly PythonContext _context;

  public PythonSetIndexBinder(PythonContext context, int argCount)
    : base(new CallInfo(argCount, new string[0]))
  {
    this._context = context;
  }

  public override DynamicMetaObject FallbackSetIndex(
    DynamicMetaObject target,
    DynamicMetaObject[] indexes,
    DynamicMetaObject value,
    DynamicMetaObject errorSuggestion)
  {
    DynamicMetaObject result;
    if (ComBinder.TryBindSetIndex((SetIndexBinder) this, target, BindingHelpers.GetComArguments(indexes), BindingHelpers.GetComArgument(value), out result))
      return result;
    DynamicMetaObject[] args = new DynamicMetaObject[indexes.Length + 2];
    args[0] = target;
    for (int index = 0; index < indexes.Length; ++index)
      args[index + 1] = indexes[index];
    args[args.Length - 1] = value;
    return PythonProtocol.Index((DynamicMetaObjectBinder) this, PythonIndexType.SetItem, args, errorSuggestion);
  }

  public override T BindDelegate<T>(CallSite<T> site, object[] args)
  {
    return args[0] != null && args[0].GetType() == typeof (PythonDictionary) && typeof (T) == typeof (Func<CallSite, object, object, object, object>) ? (T) new Func<CallSite, object, object, object, object>(this.DictAssign) : base.BindDelegate<T>(site, args);
  }

  private object DictAssign(CallSite site, object dict, object key, object value)
  {
    if (dict == null || !(dict.GetType() == typeof (PythonDictionary)))
      return ((CallSite<Func<CallSite, object, object, object, object>>) site).Update(site, dict, key, value);
    ((PythonDictionary) dict)[key] = value;
    return value;
  }

  public override int GetHashCode() => base.GetHashCode() ^ this._context.Binder.GetHashCode();

  public override bool Equals(object obj)
  {
    return obj is PythonSetIndexBinder pythonSetIndexBinder && pythonSetIndexBinder._context.Binder == this._context.Binder && base.Equals(obj);
  }

  public PythonContext Context => this._context;

  public Expression CreateExpression()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeSetIndexAction"), BindingHelpers.CreateBinderStateExpression(), Utils.Constant((object) this.CallInfo.ArgumentCount));
  }
}
