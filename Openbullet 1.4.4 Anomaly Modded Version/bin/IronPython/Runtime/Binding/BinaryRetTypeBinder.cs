// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.BinaryRetTypeBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class BinaryRetTypeBinder : ComboBinder, IExpressionSerializable
{
  private readonly DynamicMetaObjectBinder _opBinder;
  private readonly PythonConversionBinder _convBinder;

  public BinaryRetTypeBinder(
    DynamicMetaObjectBinder operationBinder,
    PythonConversionBinder conversionBinder)
    : base(new BinderMappingInfo(operationBinder, new ParameterMappingInfo[2]
    {
      ParameterMappingInfo.Parameter(0),
      ParameterMappingInfo.Parameter(1)
    }), new BinderMappingInfo((DynamicMetaObjectBinder) conversionBinder, new ParameterMappingInfo[1]
    {
      ParameterMappingInfo.Action(0)
    }))
  {
    this._opBinder = operationBinder;
    this._convBinder = conversionBinder;
  }

  public override Type ReturnType => this._convBinder.Type;

  public Expression CreateExpression()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeComboAction"), BindingHelpers.CreateBinderStateExpression(), ((IExpressionSerializable) this._opBinder).CreateExpression(), this._convBinder.CreateExpression());
  }

  public override T BindDelegate<T>(CallSite<T> site, object[] args)
  {
    if (this._convBinder.Type == typeof (bool) && this._opBinder is PythonBinaryOperationBinder)
    {
      T target = default (T);
      BinaryOperationBinder opBinder = (BinaryOperationBinder) this._opBinder;
      if (CompilerHelpers.GetType(args[0]) == typeof (int) && CompilerHelpers.GetType(args[1]) == typeof (int))
      {
        if (typeof (T) == typeof (Func<CallSite, int, int, bool>))
          target = (T) this.GetIntIntIntDelegate(opBinder);
        else if (typeof (T) == typeof (Func<CallSite, int, object, bool>))
          target = (T) this.GetIntIntObjectDelegate(opBinder);
        else if (typeof (T) == typeof (Func<CallSite, object, int, bool>))
          target = (T) this.GetIntObjectIntDelegate(opBinder);
        else if (typeof (T) == typeof (Func<CallSite, object, object, bool>))
          target = (T) this.GetIntObjectObjectDelegate(opBinder);
      }
      if ((object) target != null)
      {
        this.CacheTarget<T>(target);
        return target;
      }
    }
    return base.BindDelegate<T>(site, args);
  }

  private Func<CallSite, object, object, bool> GetIntObjectObjectDelegate(
    BinaryOperationBinder opBinder)
  {
    switch (opBinder.Operation)
    {
      case ExpressionType.Equal:
        return new Func<CallSite, object, object, bool>(this.IntEqualRetBool);
      case ExpressionType.GreaterThan:
        return new Func<CallSite, object, object, bool>(this.IntGreaterThanRetBool);
      case ExpressionType.GreaterThanOrEqual:
        return new Func<CallSite, object, object, bool>(this.IntGreaterThanOrEqualRetBool);
      case ExpressionType.LessThan:
        return new Func<CallSite, object, object, bool>(this.IntLessThanRetBool);
      case ExpressionType.LessThanOrEqual:
        return new Func<CallSite, object, object, bool>(this.IntLessThanOrEqualRetBool);
      case ExpressionType.NotEqual:
        return new Func<CallSite, object, object, bool>(this.IntNotEqualRetBool);
      default:
        return (Func<CallSite, object, object, bool>) null;
    }
  }

  public bool IntEqualRetBool(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (int) && other != null && other.GetType() == typeof (int) ? (int) self == (int) other : ((CallSite<Func<CallSite, object, object, bool>>) site).Update(site, self, other);
  }

  public bool IntNotEqualRetBool(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (int) && other != null && other.GetType() == typeof (int) ? (int) self != (int) other : ((CallSite<Func<CallSite, object, object, bool>>) site).Update(site, self, other);
  }

  public bool IntGreaterThanRetBool(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (int) && other != null && other.GetType() == typeof (int) ? (int) self > (int) other : ((CallSite<Func<CallSite, object, object, bool>>) site).Update(site, self, other);
  }

  public bool IntLessThanRetBool(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (int) && other != null && other.GetType() == typeof (int) ? (int) self < (int) other : ((CallSite<Func<CallSite, object, object, bool>>) site).Update(site, self, other);
  }

  public bool IntGreaterThanOrEqualRetBool(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (int) && other != null && other.GetType() == typeof (int) ? (int) self >= (int) other : ((CallSite<Func<CallSite, object, object, bool>>) site).Update(site, self, other);
  }

  public bool IntLessThanOrEqualRetBool(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (int) && other != null && other.GetType() == typeof (int) ? (int) self <= (int) other : ((CallSite<Func<CallSite, object, object, bool>>) site).Update(site, self, other);
  }

  private Func<CallSite, object, int, bool> GetIntObjectIntDelegate(BinaryOperationBinder opBinder)
  {
    switch (opBinder.Operation)
    {
      case ExpressionType.Equal:
        return new Func<CallSite, object, int, bool>(this.IntEqualRetBool);
      case ExpressionType.GreaterThan:
        return new Func<CallSite, object, int, bool>(this.IntGreaterThanRetBool);
      case ExpressionType.GreaterThanOrEqual:
        return new Func<CallSite, object, int, bool>(this.IntGreaterThanOrEqualRetBool);
      case ExpressionType.LessThan:
        return new Func<CallSite, object, int, bool>(this.IntLessThanRetBool);
      case ExpressionType.LessThanOrEqual:
        return new Func<CallSite, object, int, bool>(this.IntLessThanOrEqualRetBool);
      case ExpressionType.NotEqual:
        return new Func<CallSite, object, int, bool>(this.IntNotEqualRetBool);
      default:
        return (Func<CallSite, object, int, bool>) null;
    }
  }

  public bool IntEqualRetBool(CallSite site, object self, int other)
  {
    return self != null && self.GetType() == typeof (int) ? (int) self == other : ((CallSite<Func<CallSite, object, int, bool>>) site).Update(site, self, other);
  }

  public bool IntNotEqualRetBool(CallSite site, object self, int other)
  {
    return self != null && self.GetType() == typeof (int) ? (int) self != other : ((CallSite<Func<CallSite, object, int, bool>>) site).Update(site, self, other);
  }

  public bool IntGreaterThanRetBool(CallSite site, object self, int other)
  {
    return self != null && self.GetType() == typeof (int) ? (int) self > other : ((CallSite<Func<CallSite, object, int, bool>>) site).Update(site, self, other);
  }

  public bool IntLessThanRetBool(CallSite site, object self, int other)
  {
    return self != null && self.GetType() == typeof (int) ? (int) self < other : ((CallSite<Func<CallSite, object, int, bool>>) site).Update(site, self, other);
  }

  public bool IntGreaterThanOrEqualRetBool(CallSite site, object self, int other)
  {
    return self != null && self.GetType() == typeof (int) ? (int) self >= other : ((CallSite<Func<CallSite, object, int, bool>>) site).Update(site, self, other);
  }

  public bool IntLessThanOrEqualRetBool(CallSite site, object self, int other)
  {
    return self != null && self.GetType() == typeof (int) ? (int) self <= other : ((CallSite<Func<CallSite, object, int, bool>>) site).Update(site, self, other);
  }

  private Func<CallSite, int, object, bool> GetIntIntObjectDelegate(BinaryOperationBinder opBinder)
  {
    switch (opBinder.Operation)
    {
      case ExpressionType.Equal:
        return new Func<CallSite, int, object, bool>(this.IntEqualRetBool);
      case ExpressionType.GreaterThan:
        return new Func<CallSite, int, object, bool>(this.IntGreaterThanRetBool);
      case ExpressionType.GreaterThanOrEqual:
        return new Func<CallSite, int, object, bool>(this.IntGreaterThanOrEqualRetBool);
      case ExpressionType.LessThan:
        return new Func<CallSite, int, object, bool>(this.IntLessThanRetBool);
      case ExpressionType.LessThanOrEqual:
        return new Func<CallSite, int, object, bool>(this.IntLessThanOrEqualRetBool);
      case ExpressionType.NotEqual:
        return new Func<CallSite, int, object, bool>(this.IntNotEqualRetBool);
      default:
        return (Func<CallSite, int, object, bool>) null;
    }
  }

  public bool IntEqualRetBool(CallSite site, int self, object other)
  {
    return other != null && other.GetType() == typeof (int) ? self == (int) other : ((CallSite<Func<CallSite, int, object, bool>>) site).Update(site, self, other);
  }

  public bool IntNotEqualRetBool(CallSite site, int self, object other)
  {
    return other != null && other.GetType() == typeof (int) ? self != (int) other : ((CallSite<Func<CallSite, int, object, bool>>) site).Update(site, self, other);
  }

  public bool IntGreaterThanRetBool(CallSite site, int self, object other)
  {
    return other != null && other.GetType() == typeof (int) ? self > (int) other : ((CallSite<Func<CallSite, int, object, bool>>) site).Update(site, self, other);
  }

  public bool IntLessThanRetBool(CallSite site, int self, object other)
  {
    return other != null && other.GetType() == typeof (int) ? self < (int) other : ((CallSite<Func<CallSite, int, object, bool>>) site).Update(site, self, other);
  }

  public bool IntGreaterThanOrEqualRetBool(CallSite site, int self, object other)
  {
    return other != null && other.GetType() == typeof (int) ? self >= (int) other : ((CallSite<Func<CallSite, int, object, bool>>) site).Update(site, self, other);
  }

  public bool IntLessThanOrEqualRetBool(CallSite site, int self, object other)
  {
    return other != null && other.GetType() == typeof (int) ? self <= (int) other : ((CallSite<Func<CallSite, int, object, bool>>) site).Update(site, self, other);
  }

  private Func<CallSite, int, int, bool> GetIntIntIntDelegate(BinaryOperationBinder opBinder)
  {
    switch (opBinder.Operation)
    {
      case ExpressionType.Equal:
        return new Func<CallSite, int, int, bool>(this.IntEqualRetBool);
      case ExpressionType.GreaterThan:
        return new Func<CallSite, int, int, bool>(this.IntGreaterThanRetBool);
      case ExpressionType.GreaterThanOrEqual:
        return new Func<CallSite, int, int, bool>(this.IntGreaterThanOrEqualRetBool);
      case ExpressionType.LessThan:
        return new Func<CallSite, int, int, bool>(this.IntLessThanRetBool);
      case ExpressionType.LessThanOrEqual:
        return new Func<CallSite, int, int, bool>(this.IntLessThanOrEqualRetBool);
      case ExpressionType.NotEqual:
        return new Func<CallSite, int, int, bool>(this.IntNotEqualRetBool);
      default:
        return (Func<CallSite, int, int, bool>) null;
    }
  }

  public bool IntEqualRetBool(CallSite site, int self, int other) => self == other;

  public bool IntNotEqualRetBool(CallSite site, int self, int other) => self != other;

  public bool IntGreaterThanRetBool(CallSite site, int self, int other) => self > other;

  public bool IntLessThanRetBool(CallSite site, int self, int other) => self < other;

  public bool IntGreaterThanOrEqualRetBool(CallSite site, int self, int other) => self >= other;

  public bool IntLessThanOrEqualRetBool(CallSite site, int self, int other) => self <= other;
}
