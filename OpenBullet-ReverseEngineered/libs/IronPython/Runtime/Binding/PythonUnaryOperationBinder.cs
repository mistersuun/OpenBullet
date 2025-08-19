// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonUnaryOperationBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class PythonUnaryOperationBinder : 
  UnaryOperationBinder,
  IPythonSite,
  IExpressionSerializable
{
  private readonly PythonContext _context;

  public PythonUnaryOperationBinder(PythonContext context, ExpressionType operation)
    : base(operation)
  {
    this._context = context;
  }

  public override DynamicMetaObject FallbackUnaryOperation(
    DynamicMetaObject target,
    DynamicMetaObject errorSuggestion)
  {
    return PythonProtocol.Operation((UnaryOperationBinder) this, target, errorSuggestion);
  }

  public override int GetHashCode() => base.GetHashCode() ^ this._context.Binder.GetHashCode();

  public override bool Equals(object obj)
  {
    return obj is PythonUnaryOperationBinder unaryOperationBinder && unaryOperationBinder._context.Binder == this._context.Binder && base.Equals(obj);
  }

  public override T BindDelegate<T>(CallSite<T> site, object[] args)
  {
    switch (this.Operation)
    {
      case ExpressionType.Negate:
        if (CompilerHelpers.GetType(args[0]) == typeof (int) && typeof (T) == typeof (Func<CallSite, object, object>))
          return (T) new Func<CallSite, object, object>(this.IntNegate);
        break;
      case ExpressionType.Not:
        if (args[0] == null)
        {
          if (typeof (T) == typeof (Func<CallSite, object, object>))
            return (T) new Func<CallSite, object, object>(this.NoneNot);
          break;
        }
        if (args[0].GetType() == typeof (string))
        {
          if (typeof (T) == typeof (Func<CallSite, object, object>))
            return (T) new Func<CallSite, object, object>(this.StringNot);
          break;
        }
        if (args[0].GetType() == typeof (bool))
        {
          if (typeof (T) == typeof (Func<CallSite, object, object>))
            return (T) new Func<CallSite, object, object>(this.BoolNot);
          break;
        }
        if (args[0].GetType() == typeof (List))
        {
          if (typeof (T) == typeof (Func<CallSite, object, object>))
            return (T) new Func<CallSite, object, object>(this.ListNot);
          break;
        }
        if (args[0].GetType() == typeof (PythonTuple))
        {
          if (typeof (T) == typeof (Func<CallSite, object, object>))
            return (T) new Func<CallSite, object, object>(this.TupleNot);
          break;
        }
        if (args[0].GetType() == typeof (int) && typeof (T) == typeof (Func<CallSite, object, object>))
          return (T) new Func<CallSite, object, object>(this.IntNot);
        break;
      case ExpressionType.IsFalse:
        if (args[0] == null)
        {
          if (typeof (T) == typeof (Func<CallSite, object, bool>))
            return (T) new Func<CallSite, object, bool>(this.NoneIsFalse);
          break;
        }
        if (args[0].GetType() == typeof (string))
        {
          if (typeof (T) == typeof (Func<CallSite, object, bool>))
            return (T) new Func<CallSite, object, bool>(this.StringIsFalse);
          break;
        }
        if (args[0].GetType() == typeof (bool))
        {
          if (typeof (T) == typeof (Func<CallSite, object, bool>))
            return (T) new Func<CallSite, object, bool>(this.BoolIsFalse);
          break;
        }
        if (args[0].GetType() == typeof (List))
        {
          if (typeof (T) == typeof (Func<CallSite, object, bool>))
            return (T) new Func<CallSite, object, bool>(this.ListIsFalse);
          break;
        }
        if (args[0].GetType() == typeof (PythonTuple))
        {
          if (typeof (T) == typeof (Func<CallSite, object, bool>))
            return (T) new Func<CallSite, object, bool>(this.TupleIsFalse);
          break;
        }
        if (args[0].GetType() == typeof (int) && typeof (T) == typeof (Func<CallSite, object, bool>))
          return (T) new Func<CallSite, object, bool>(this.IntIsFalse);
        break;
    }
    return base.BindDelegate<T>(site, args);
  }

  private object IntNegate(CallSite site, object value)
  {
    return value is int x ? Int32Ops.Negate(x) : ((CallSite<Func<CallSite, object, object>>) site).Update(site, value);
  }

  private bool StringIsFalse(CallSite site, object value)
  {
    if (value is string str)
      return str.Length == 0;
    return value == null || ((CallSite<Func<CallSite, object, bool>>) site).Update(site, value);
  }

  private bool ListIsFalse(CallSite site, object value)
  {
    if (value != null && value.GetType() == typeof (List))
      return ((List) value).Count == 0;
    return value == null || ((CallSite<Func<CallSite, object, bool>>) site).Update(site, value);
  }

  private bool NoneIsFalse(CallSite site, object value)
  {
    return value == null || ((CallSite<Func<CallSite, object, bool>>) site).Update(site, value);
  }

  private bool IntIsFalse(CallSite site, object value)
  {
    if (value is int num)
      return num == 0;
    return value == null || ((CallSite<Func<CallSite, object, bool>>) site).Update(site, value);
  }

  private bool TupleIsFalse(CallSite site, object value)
  {
    if (value != null && value.GetType() == typeof (PythonTuple))
      return ((PythonTuple) value).Count == 0;
    return value == null || ((CallSite<Func<CallSite, object, bool>>) site).Update(site, value);
  }

  private bool BoolIsFalse(CallSite site, object value)
  {
    if (value is bool flag)
      return !flag;
    return value == null || ((CallSite<Func<CallSite, object, bool>>) site).Update(site, value);
  }

  private object StringNot(CallSite site, object value)
  {
    return value is string str ? (str.Length != 0 ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True) : (value == null ? (object) true : ((CallSite<Func<CallSite, object, object>>) site).Update(site, value));
  }

  private object ListNot(CallSite site, object value)
  {
    return value != null && value.GetType() == typeof (List) ? (((List) value).Count != 0 ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True) : (value == null ? (object) true : ((CallSite<Func<CallSite, object, object>>) site).Update(site, value));
  }

  private object NoneNot(CallSite site, object value)
  {
    return value == null ? ScriptingRuntimeHelpers.True : ((CallSite<Func<CallSite, object, object>>) site).Update(site, value);
  }

  private object TupleNot(CallSite site, object value)
  {
    return value != null && value.GetType() == typeof (PythonTuple) ? (((PythonTuple) value).Count != 0 ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True) : (value == null ? (object) true : ((CallSite<Func<CallSite, object, object>>) site).Update(site, value));
  }

  private object BoolNot(CallSite site, object value)
  {
    return value is bool flag ? (flag ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True) : (value == null ? (object) true : ((CallSite<Func<CallSite, object, object>>) site).Update(site, value));
  }

  private object IntNot(CallSite site, object value)
  {
    return value is int num ? (num != 0 ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True) : (value == null ? (object) true : ((CallSite<Func<CallSite, object, object>>) site).Update(site, value));
  }

  public PythonContext Context => this._context;

  public override string ToString() => "PythonUnary " + (object) this.Operation;

  public Expression CreateExpression()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeUnaryOperationAction"), BindingHelpers.CreateBinderStateExpression(), Utils.Constant((object) this.Operation));
  }
}
