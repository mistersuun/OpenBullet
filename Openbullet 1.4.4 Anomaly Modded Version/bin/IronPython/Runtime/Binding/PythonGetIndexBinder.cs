// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonGetIndexBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.ComInterop;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class PythonGetIndexBinder : GetIndexBinder, IPythonSite, IExpressionSerializable
{
  private readonly PythonContext _context;

  public PythonGetIndexBinder(PythonContext context, int argCount)
    : base(new CallInfo(argCount, new string[0]))
  {
    this._context = context;
  }

  public override DynamicMetaObject FallbackGetIndex(
    DynamicMetaObject target,
    DynamicMetaObject[] indexes,
    DynamicMetaObject errorSuggestion)
  {
    DynamicMetaObject result;
    return ComBinder.TryBindGetIndex((GetIndexBinder) this, target, BindingHelpers.GetComArguments(indexes), out result) ? result : PythonProtocol.Index((DynamicMetaObjectBinder) this, PythonIndexType.GetItem, ArrayUtils.Insert<DynamicMetaObject>(target, indexes), errorSuggestion);
  }

  public override T BindDelegate<T>(CallSite<T> site, object[] args)
  {
    if (CompilerHelpers.GetType(args[1]) == typeof (int))
    {
      if (CompilerHelpers.GetType(args[0]) == typeof (List))
      {
        if (typeof (T) == typeof (Func<CallSite, object, object, object>))
          return (T) new Func<CallSite, object, object, object>(this.ListIndex);
        if (typeof (T) == typeof (Func<CallSite, object, int, object>))
          return (T) new Func<CallSite, object, int, object>(this.ListIndex);
        if (typeof (T) == typeof (Func<CallSite, List, object, object>))
          return (T) new Func<CallSite, List, object, object>(this.ListIndex);
      }
      else if (CompilerHelpers.GetType(args[0]) == typeof (PythonTuple))
      {
        if (typeof (T) == typeof (Func<CallSite, object, object, object>))
          return (T) new Func<CallSite, object, object, object>(this.TupleIndex);
        if (typeof (T) == typeof (Func<CallSite, object, int, object>))
          return (T) new Func<CallSite, object, int, object>(this.TupleIndex);
        if (typeof (T) == typeof (Func<CallSite, PythonTuple, object, object>))
          return (T) new Func<CallSite, PythonTuple, object, object>(this.TupleIndex);
      }
      else if (CompilerHelpers.GetType(args[0]) == typeof (string))
      {
        if (typeof (T) == typeof (Func<CallSite, object, object, object>))
          return (T) new Func<CallSite, object, object, object>(this.StringIndex);
        if (typeof (T) == typeof (Func<CallSite, object, int, object>))
          return (T) new Func<CallSite, object, int, object>(this.StringIndex);
        if (typeof (T) == typeof (Func<CallSite, string, object, object>))
          return (T) new Func<CallSite, string, object, object>(this.StringIndex);
      }
    }
    return base.BindDelegate<T>(site, args);
  }

  private object ListIndex(CallSite site, List target, object index)
  {
    return target != null && index != null && index.GetType() == typeof (int) ? target[(int) index] : ((CallSite<Func<CallSite, List, object, object>>) site).Update(site, target, index);
  }

  private object ListIndex(CallSite site, object target, object index)
  {
    return target is List list && index != null && index.GetType() == typeof (int) ? list[(int) index] : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, target, index);
  }

  private object ListIndex(CallSite site, object target, int index)
  {
    return target is List list ? list[index] : ((CallSite<Func<CallSite, object, int, object>>) site).Update(site, target, index);
  }

  private object TupleIndex(CallSite site, PythonTuple target, object index)
  {
    return target != null && index != null && index.GetType() == typeof (int) ? target[(int) index] : ((CallSite<Func<CallSite, PythonTuple, object, object>>) site).Update(site, target, index);
  }

  private object TupleIndex(CallSite site, object target, object index)
  {
    return target is PythonTuple pythonTuple && index != null && index.GetType() == typeof (int) ? pythonTuple[(int) index] : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, target, index);
  }

  private object TupleIndex(CallSite site, object target, int index)
  {
    return target is PythonTuple pythonTuple ? pythonTuple[index] : ((CallSite<Func<CallSite, object, int, object>>) site).Update(site, target, index);
  }

  private object StringIndex(CallSite site, string target, object index)
  {
    return target != null && index != null && index.GetType() == typeof (int) ? (object) StringOps.GetItem(target, (int) index) : ((CallSite<Func<CallSite, string, object, object>>) site).Update(site, target, index);
  }

  private object StringIndex(CallSite site, object target, object index)
  {
    return target is string s && index != null && index.GetType() == typeof (int) ? (object) StringOps.GetItem(s, (int) index) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, target, index);
  }

  private object StringIndex(CallSite site, object target, int index)
  {
    return target is string s ? (object) StringOps.GetItem(s, index) : ((CallSite<Func<CallSite, object, int, object>>) site).Update(site, target, index);
  }

  public override int GetHashCode() => base.GetHashCode() ^ this._context.Binder.GetHashCode();

  public override bool Equals(object obj)
  {
    return obj is PythonGetIndexBinder pythonGetIndexBinder && pythonGetIndexBinder._context.Binder == this._context.Binder && base.Equals(obj);
  }

  public PythonContext Context => this._context;

  public Expression CreateExpression()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeGetIndexAction"), BindingHelpers.CreateBinderStateExpression(), Microsoft.Scripting.Ast.Utils.Constant((object) this.CallInfo.ArgumentCount));
  }
}
