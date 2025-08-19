// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonOperationBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class PythonOperationBinder : DynamicMetaObjectBinder, IPythonSite, IExpressionSerializable
{
  private readonly PythonContext _context;
  private readonly PythonOperationKind _operation;

  public PythonOperationBinder(PythonContext context, PythonOperationKind operation)
  {
    this._context = context;
    this._operation = operation;
  }

  public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
  {
    if (target is IPythonOperable pythonOperable)
    {
      DynamicMetaObject dynamicMetaObject = pythonOperable.BindOperation(this, ArrayUtils.Insert<DynamicMetaObject>(target, args));
      if (dynamicMetaObject != null)
        return dynamicMetaObject;
    }
    return PythonProtocol.Operation(this, ArrayUtils.Insert<DynamicMetaObject>(target, args));
  }

  public override T BindDelegate<T>(CallSite<T> site, object[] args)
  {
    switch (this._operation)
    {
      case PythonOperationKind.Hash:
        if (CompilerHelpers.GetType(args[0]) == typeof (PythonType))
        {
          if (typeof (T) == typeof (Func<CallSite, object, int>))
            return (T) new Func<CallSite, object, int>(this.HashPythonType);
          break;
        }
        if (args[0] is OldClass && typeof (T) == typeof (Func<CallSite, object, int>))
          return (T) new Func<CallSite, object, int>(this.HashOldClass);
        break;
      case PythonOperationKind.Contains:
        if (CompilerHelpers.GetType(args[1]) == typeof (List))
        {
          Type type = typeof (T);
          if (type == typeof (Func<CallSite, object, object, bool>))
            return (T) new Func<CallSite, object, object, bool>(this.ListContains<object>);
          if (type == typeof (Func<CallSite, object, List, bool>))
            return (T) new Func<CallSite, object, List, bool>(this.ListContains);
          if (type == typeof (Func<CallSite, int, object, bool>))
            return (T) new Func<CallSite, int, object, bool>(this.ListContains<int>);
          if (type == typeof (Func<CallSite, string, object, bool>))
            return (T) new Func<CallSite, string, object, bool>(this.ListContains<string>);
          if (type == typeof (Func<CallSite, double, object, bool>))
            return (T) new Func<CallSite, double, object, bool>(this.ListContains<double>);
          if (type == typeof (Func<CallSite, PythonTuple, object, bool>))
            return (T) new Func<CallSite, PythonTuple, object, bool>(this.ListContains<PythonTuple>);
          break;
        }
        if (CompilerHelpers.GetType(args[1]) == typeof (PythonTuple))
        {
          Type type = typeof (T);
          if (type == typeof (Func<CallSite, object, object, bool>))
            return (T) new Func<CallSite, object, object, bool>(this.TupleContains<object>);
          if (type == typeof (Func<CallSite, object, PythonTuple, bool>))
            return (T) new Func<CallSite, object, PythonTuple, bool>(this.TupleContains);
          if (type == typeof (Func<CallSite, int, object, bool>))
            return (T) new Func<CallSite, int, object, bool>(this.TupleContains<int>);
          if (type == typeof (Func<CallSite, string, object, bool>))
            return (T) new Func<CallSite, string, object, bool>(this.TupleContains<string>);
          if (type == typeof (Func<CallSite, double, object, bool>))
            return (T) new Func<CallSite, double, object, bool>(this.TupleContains<double>);
          if (type == typeof (Func<CallSite, PythonTuple, object, bool>))
            return (T) new Func<CallSite, PythonTuple, object, bool>(this.TupleContains<PythonTuple>);
          break;
        }
        if (CompilerHelpers.GetType(args[0]) == typeof (string) && CompilerHelpers.GetType(args[1]) == typeof (string))
        {
          Type type = typeof (T);
          if (type == typeof (Func<CallSite, object, object, bool>))
            return (T) new Func<CallSite, object, object, bool>(this.StringContains);
          if (type == typeof (Func<CallSite, string, object, bool>))
            return (T) new Func<CallSite, string, object, bool>(this.StringContains);
          if (type == typeof (Func<CallSite, object, string, bool>))
            return (T) new Func<CallSite, object, string, bool>(this.StringContains);
          if (type == typeof (Func<CallSite, string, string, bool>))
            return (T) new Func<CallSite, string, string, bool>(this.StringContains);
          break;
        }
        if (CompilerHelpers.GetType(args[1]) == typeof (SetCollection) && typeof (T) == typeof (Func<CallSite, object, object, bool>))
          return (T) new Func<CallSite, object, object, bool>(this.SetContains);
        break;
      case PythonOperationKind.Compare:
        if (CompilerHelpers.GetType(args[0]) == typeof (string) && CompilerHelpers.GetType(args[1]) == typeof (string) && typeof (T) == typeof (Func<CallSite, object, object, int>))
          return (T) new Func<CallSite, object, object, int>(this.CompareStrings);
        break;
      case PythonOperationKind.GetEnumeratorForIteration:
        if (CompilerHelpers.GetType(args[0]) == typeof (List))
          return typeof (T) == typeof (Func<CallSite, List, KeyValuePair<IEnumerator, IDisposable>>) ? (T) new Func<CallSite, List, KeyValuePair<IEnumerator, IDisposable>>(this.GetListEnumerator) : (T) new Func<CallSite, object, KeyValuePair<IEnumerator, IDisposable>>(this.GetListEnumerator);
        if (CompilerHelpers.GetType(args[0]) == typeof (PythonTuple))
          return typeof (T) == typeof (Func<CallSite, PythonTuple, KeyValuePair<IEnumerator, IDisposable>>) ? (T) new Func<CallSite, PythonTuple, KeyValuePair<IEnumerator, IDisposable>>(this.GetTupleEnumerator) : (T) new Func<CallSite, object, KeyValuePair<IEnumerator, IDisposable>>(this.GetTupleEnumerator);
        break;
    }
    return base.BindDelegate<T>(site, args);
  }

  private KeyValuePair<IEnumerator, IDisposable> GetListEnumerator(CallSite site, List value)
  {
    return new KeyValuePair<IEnumerator, IDisposable>((IEnumerator) new ListIterator(value), (IDisposable) null);
  }

  private KeyValuePair<IEnumerator, IDisposable> GetListEnumerator(CallSite site, object value)
  {
    return value != null && value.GetType() == typeof (List) ? new KeyValuePair<IEnumerator, IDisposable>((IEnumerator) new ListIterator((List) value), (IDisposable) null) : ((CallSite<Func<CallSite, object, KeyValuePair<IEnumerator, IDisposable>>>) site).Update(site, value);
  }

  private KeyValuePair<IEnumerator, IDisposable> GetTupleEnumerator(
    CallSite site,
    PythonTuple value)
  {
    return new KeyValuePair<IEnumerator, IDisposable>((IEnumerator) new TupleEnumerator(value), (IDisposable) null);
  }

  private KeyValuePair<IEnumerator, IDisposable> GetTupleEnumerator(CallSite site, object value)
  {
    return value != null && value.GetType() == typeof (PythonTuple) ? new KeyValuePair<IEnumerator, IDisposable>((IEnumerator) new TupleEnumerator((PythonTuple) value), (IDisposable) null) : ((CallSite<Func<CallSite, object, KeyValuePair<IEnumerator, IDisposable>>>) site).Update(site, value);
  }

  private bool ListContains(CallSite site, object other, List value) => value.ContainsWorker(other);

  private bool ListContains<TOther>(CallSite site, TOther other, object value)
  {
    return value != null && value.GetType() == typeof (List) ? ((List) value).ContainsWorker((object) other) : ((CallSite<Func<CallSite, TOther, object, bool>>) site).Update(site, other, value);
  }

  private bool TupleContains(CallSite site, object other, PythonTuple value)
  {
    return value.Contains(other);
  }

  private bool TupleContains<TOther>(CallSite site, TOther other, object value)
  {
    return value != null && value.GetType() == typeof (PythonTuple) ? ((PythonTuple) value).Contains((object) other) : ((CallSite<Func<CallSite, TOther, object, bool>>) site).Update(site, other, value);
  }

  private bool StringContains(CallSite site, string other, string value)
  {
    return other != null && value != null ? StringOps.__contains__(value, other) : ((CallSite<Func<CallSite, string, string, bool>>) site).Update(site, other, value);
  }

  private bool StringContains(CallSite site, object other, string value)
  {
    return other is string && value != null ? StringOps.__contains__(value, (string) other) : ((CallSite<Func<CallSite, object, string, bool>>) site).Update(site, other, value);
  }

  private bool StringContains(CallSite site, string other, object value)
  {
    return value is string && other != null ? StringOps.__contains__((string) value, other) : ((CallSite<Func<CallSite, string, object, bool>>) site).Update(site, other, value);
  }

  private bool StringContains(CallSite site, object other, object value)
  {
    return value is string && other is string ? StringOps.__contains__((string) value, (string) other) : ((CallSite<Func<CallSite, object, object, bool>>) site).Update(site, other, value);
  }

  private bool SetContains(CallSite site, object other, object value)
  {
    return value != null && value.GetType() == typeof (SetCollection) ? ((SetCollection) value).__contains__(other) : ((CallSite<Func<CallSite, object, object, bool>>) site).Update(site, other, value);
  }

  private int HashPythonType(CallSite site, object value)
  {
    return value != null && value.GetType() == typeof (PythonType) ? value.GetHashCode() : ((CallSite<Func<CallSite, object, int>>) site).Update(site, value);
  }

  private int HashOldClass(CallSite site, object value)
  {
    return value is OldClass ? value.GetHashCode() : ((CallSite<Func<CallSite, object, int>>) site).Update(site, value);
  }

  private int CompareStrings(CallSite site, object arg0, object arg1)
  {
    return arg0 != null && arg0.GetType() == typeof (string) && arg1 != null && arg1.GetType() == typeof (string) ? StringOps.Compare((string) arg0, (string) arg1) : ((CallSite<Func<CallSite, object, object, int>>) site).Update(site, arg0, arg1);
  }

  public PythonOperationKind Operation => this._operation;

  public override Type ReturnType
  {
    get
    {
      switch (this.Operation & ~PythonOperationKind.DisableCoerce)
      {
        case PythonOperationKind.Documentation:
          return typeof (string);
        case PythonOperationKind.CallSignatures:
          return typeof (IList<string>);
        case PythonOperationKind.IsCallable:
          return typeof (bool);
        case PythonOperationKind.Hash:
          return typeof (int);
        case PythonOperationKind.Contains:
          return typeof (bool);
        case PythonOperationKind.Compare:
          return typeof (int);
        case PythonOperationKind.GetEnumeratorForIteration:
          return typeof (KeyValuePair<IEnumerator, IDisposable>);
        default:
          return typeof (object);
      }
    }
  }

  public override int GetHashCode()
  {
    return base.GetHashCode() ^ this._context.Binder.GetHashCode() ^ this._operation.GetHashCode();
  }

  public override bool Equals(object obj)
  {
    return obj is PythonOperationBinder pythonOperationBinder && pythonOperationBinder._context.Binder == this._context.Binder && base.Equals(obj);
  }

  public PythonContext Context => this._context;

  public override string ToString() => "Python " + (object) this.Operation;

  public Expression CreateExpression()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeOperationAction"), BindingHelpers.CreateBinderStateExpression(), Microsoft.Scripting.Ast.Utils.Constant((object) (int) this.Operation));
  }
}
