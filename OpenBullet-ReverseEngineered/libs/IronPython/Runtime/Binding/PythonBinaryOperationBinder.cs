// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonBinaryOperationBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class PythonBinaryOperationBinder : 
  BinaryOperationBinder,
  IPythonSite,
  IExpressionSerializable,
  ILightExceptionBinder
{
  private readonly PythonContext _context;
  private PythonBinaryOperationBinder _lightThrowBinder;

  public PythonBinaryOperationBinder(PythonContext context, ExpressionType operation)
    : base(operation)
  {
    this._context = context;
  }

  public override DynamicMetaObject FallbackBinaryOperation(
    DynamicMetaObject target,
    DynamicMetaObject arg,
    DynamicMetaObject errorSuggestion)
  {
    return PythonProtocol.Operation((BinaryOperationBinder) this, target, arg, errorSuggestion);
  }

  public override T BindDelegate<T>(CallSite<T> site, object[] args)
  {
    if (args[0] != null && CompilerHelpers.GetType(args[0]) == CompilerHelpers.GetType(args[1]))
    {
      switch (this.Operation)
      {
        case ExpressionType.Add:
        case ExpressionType.AddAssign:
          return this.BindAdd<T>(site, args);
        case ExpressionType.And:
        case ExpressionType.AndAssign:
          return this.BindAnd<T>(site, args);
        case ExpressionType.Divide:
        case ExpressionType.DivideAssign:
          return this.BindDivide<T>(site, args);
        case ExpressionType.Equal:
          return this.BindEqual<T>(site, args);
        case ExpressionType.GreaterThan:
          return this.BindGreaterThan<T>(site, args);
        case ExpressionType.GreaterThanOrEqual:
          return this.BindGreaterThanOrEqual<T>(site, args);
        case ExpressionType.LessThan:
          return this.BindLessThan<T>(site, args);
        case ExpressionType.LessThanOrEqual:
          return this.BindLessThanOrEqual<T>(site, args);
        case ExpressionType.Modulo:
          return this.BindModulo<T>(site, args);
        case ExpressionType.Multiply:
        case ExpressionType.MultiplyAssign:
          return this.BindMultiply<T>(site, args);
        case ExpressionType.NotEqual:
          return this.BindNotEqual<T>(site, args);
        case ExpressionType.Or:
        case ExpressionType.OrAssign:
          return this.BindOr<T>(site, args);
        case ExpressionType.Subtract:
        case ExpressionType.SubtractAssign:
          return this.BindSubtract<T>(site, args);
      }
    }
    else
    {
      switch (this.Operation)
      {
        case ExpressionType.Modulo:
          return this.BindModulo<T>(site, args);
        case ExpressionType.Multiply:
          return this.BindMultiplyDifferentTypes<T>(site, args);
      }
    }
    return base.BindDelegate<T>(site, args);
  }

  private T BindModulo<T>(CallSite<T> site, object[] args) where T : class
  {
    if (CompilerHelpers.GetType(args[0]) == typeof (string) && !(args[1] is Extensible<string>))
    {
      if (typeof (T) == typeof (Func<CallSite, string, PythonDictionary, object>))
        return (T) new Func<CallSite, string, PythonDictionary, object>(this.StringModulo);
      if (typeof (T) == typeof (Func<CallSite, string, PythonTuple, object>))
        return (T) new Func<CallSite, string, PythonTuple, object>(this.StringModulo);
      if (typeof (T) == typeof (Func<CallSite, string, object, object>))
        return (T) new Func<CallSite, string, object, object>(this.StringModulo);
      if (typeof (T) == typeof (Func<CallSite, object, PythonDictionary, object>))
        return (T) new Func<CallSite, object, PythonDictionary, object>(this.StringModulo);
      if (typeof (T) == typeof (Func<CallSite, object, PythonTuple, object>))
        return (T) new Func<CallSite, object, PythonTuple, object>(this.StringModulo);
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return (T) new Func<CallSite, object, object, object>(this.StringModulo);
    }
    return base.BindDelegate<T>(site, args);
  }

  private T BindMultiply<T>(CallSite<T> site, object[] args) where T : class
  {
    if (CompilerHelpers.GetType(args[0]) == typeof (int) && CompilerHelpers.GetType(args[1]) == typeof (int))
    {
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return (T) new Func<CallSite, object, object, object>(this.IntMultiply);
      if (typeof (T) == typeof (Func<CallSite, int, object, object>))
        return (T) new Func<CallSite, int, object, object>(this.IntMultiply);
      if (typeof (T) == typeof (Func<CallSite, object, int, object>))
        return (T) new Func<CallSite, object, int, object>(this.IntMultiply);
    }
    return base.BindDelegate<T>(site, args);
  }

  private T BindMultiplyDifferentTypes<T>(CallSite<T> site, object[] args) where T : class
  {
    if (CompilerHelpers.GetType(args[0]) == typeof (List) && CompilerHelpers.GetType(args[1]) == typeof (int))
    {
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return (T) new Func<CallSite, object, object, object>(this.ListIntMultiply);
    }
    else if (CompilerHelpers.GetType(args[0]) == typeof (string) && CompilerHelpers.GetType(args[1]) == typeof (int))
    {
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return (T) new Func<CallSite, object, object, object>(this.StringIntMultiply);
    }
    else if (CompilerHelpers.GetType(args[0]) == typeof (PythonTuple) && CompilerHelpers.GetType(args[1]) == typeof (int) && typeof (T) == typeof (Func<CallSite, object, object, object>))
      return (T) new Func<CallSite, object, object, object>(this.TupleIntMultiply);
    return base.BindDelegate<T>(site, args);
  }

  private T BindDivide<T>(CallSite<T> site, object[] args) where T : class
  {
    if (CompilerHelpers.GetType(args[0]) == typeof (int) && CompilerHelpers.GetType(args[1]) == typeof (int))
    {
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return (T) new Func<CallSite, object, object, object>(this.IntDivide);
      if (typeof (T) == typeof (Func<CallSite, int, object, object>))
        return (T) new Func<CallSite, int, object, object>(this.IntDivide);
      if (typeof (T) == typeof (Func<CallSite, object, int, object>))
        return (T) new Func<CallSite, object, int, object>(this.IntDivide);
    }
    return base.BindDelegate<T>(site, args);
  }

  private T BindLessThanOrEqual<T>(CallSite<T> site, object[] args) where T : class
  {
    if (CompilerHelpers.GetType(args[0]) == typeof (int) && CompilerHelpers.GetType(args[1]) == typeof (int))
    {
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return (T) new Func<CallSite, object, object, object>(this.IntLessThanOrEqual);
      if (typeof (T) == typeof (Func<CallSite, int, object, object>))
        return (T) new Func<CallSite, int, object, object>(this.IntLessThanOrEqual);
      if (typeof (T) == typeof (Func<CallSite, object, int, object>))
        return (T) new Func<CallSite, object, int, object>(this.IntLessThanOrEqual);
    }
    return base.BindDelegate<T>(site, args);
  }

  private T BindGreaterThanOrEqual<T>(CallSite<T> site, object[] args) where T : class
  {
    if (CompilerHelpers.GetType(args[0]) == typeof (int) && CompilerHelpers.GetType(args[1]) == typeof (int))
    {
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return (T) new Func<CallSite, object, object, object>(this.IntGreaterThanOrEqual);
      if (typeof (T) == typeof (Func<CallSite, int, object, object>))
        return (T) new Func<CallSite, int, object, object>(this.IntGreaterThanOrEqual);
      if (typeof (T) == typeof (Func<CallSite, object, int, object>))
        return (T) new Func<CallSite, object, int, object>(this.IntGreaterThanOrEqual);
    }
    return base.BindDelegate<T>(site, args);
  }

  private T BindGreaterThan<T>(CallSite<T> site, object[] args) where T : class
  {
    if (CompilerHelpers.GetType(args[0]) == typeof (int) && CompilerHelpers.GetType(args[1]) == typeof (int))
    {
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return (T) new Func<CallSite, object, object, object>(this.IntGreaterThan);
      if (typeof (T) == typeof (Func<CallSite, int, object, object>))
        return (T) new Func<CallSite, int, object, object>(this.IntGreaterThan);
      if (typeof (T) == typeof (Func<CallSite, object, int, object>))
        return (T) new Func<CallSite, object, int, object>(this.IntGreaterThan);
    }
    return base.BindDelegate<T>(site, args);
  }

  private T BindLessThan<T>(CallSite<T> site, object[] args) where T : class
  {
    if (CompilerHelpers.GetType(args[0]) == typeof (int) && CompilerHelpers.GetType(args[1]) == typeof (int))
    {
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return (T) new Func<CallSite, object, object, object>(this.IntLessThan);
      if (typeof (T) == typeof (Func<CallSite, int, object, object>))
        return (T) new Func<CallSite, int, object, object>(this.IntLessThan);
      if (typeof (T) == typeof (Func<CallSite, object, int, object>))
        return (T) new Func<CallSite, object, int, object>(this.IntLessThan);
    }
    return base.BindDelegate<T>(site, args);
  }

  private T BindAnd<T>(CallSite<T> site, object[] args) where T : class
  {
    if (CompilerHelpers.GetType(args[0]) == typeof (int) && CompilerHelpers.GetType(args[1]) == typeof (int))
    {
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return (T) new Func<CallSite, object, object, object>(this.IntAnd);
      if (typeof (T) == typeof (Func<CallSite, int, object, object>))
        return (T) new Func<CallSite, int, object, object>(this.IntAnd);
      if (typeof (T) == typeof (Func<CallSite, object, int, object>))
        return (T) new Func<CallSite, object, int, object>(this.IntAnd);
    }
    return base.BindDelegate<T>(site, args);
  }

  private T BindOr<T>(CallSite<T> site, object[] args) where T : class
  {
    if (CompilerHelpers.GetType(args[0]) == typeof (int) && CompilerHelpers.GetType(args[1]) == typeof (int))
    {
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return (T) new Func<CallSite, object, object, object>(this.IntOr);
      if (typeof (T) == typeof (Func<CallSite, int, object, object>))
        return (T) new Func<CallSite, int, object, object>(this.IntOr);
      if (typeof (T) == typeof (Func<CallSite, object, int, object>))
        return (T) new Func<CallSite, object, int, object>(this.IntOr);
    }
    return base.BindDelegate<T>(site, args);
  }

  private T BindAdd<T>(CallSite<T> site, object[] args) where T : class
  {
    Type type = args[0].GetType();
    if (type == typeof (string))
    {
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return (T) new Func<CallSite, object, object, object>(this.StringAdd);
      if (typeof (T) == typeof (Func<CallSite, object, string, object>))
        return (T) new Func<CallSite, object, string, object>(this.StringAdd);
      if (typeof (T) == typeof (Func<CallSite, string, object, object>))
        return (T) new Func<CallSite, string, object, object>(this.StringAdd);
    }
    else if (type == typeof (List))
    {
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return this.Operation == ExpressionType.Add ? (T) new Func<CallSite, object, object, object>(this.ListAdd) : (T) new Func<CallSite, object, object, object>(this.ListAddAssign);
    }
    else if (type == typeof (PythonTuple))
    {
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return (T) new Func<CallSite, object, object, object>(this.TupleAdd);
    }
    else if (!type.IsEnum())
    {
      switch (type.GetTypeCode())
      {
        case TypeCode.Int32:
          if (typeof (T) == typeof (Func<CallSite, object, object, object>))
            return (T) new Func<CallSite, object, object, object>(this.IntAdd);
          if (typeof (T) == typeof (Func<CallSite, object, int, object>))
            return (T) new Func<CallSite, object, int, object>(this.IntAdd);
          if (typeof (T) == typeof (Func<CallSite, int, object, object>))
            return (T) new Func<CallSite, int, object, object>(this.IntAdd);
          break;
        case TypeCode.Double:
          if (typeof (T) == typeof (Func<CallSite, object, object, object>))
            return (T) new Func<CallSite, object, object, object>(this.DoubleAdd);
          break;
      }
    }
    return base.BindDelegate<T>(site, args);
  }

  private T BindSubtract<T>(CallSite<T> site, object[] args) where T : class
  {
    Type type = args[0].GetType();
    if (!type.IsEnum())
    {
      switch (type.GetTypeCode())
      {
        case TypeCode.Int32:
          if (typeof (T) == typeof (Func<CallSite, object, object, object>))
            return (T) new Func<CallSite, object, object, object>(this.IntSubtract);
          if (typeof (T) == typeof (Func<CallSite, object, int, object>))
            return (T) new Func<CallSite, object, int, object>(this.IntSubtract);
          if (typeof (T) == typeof (Func<CallSite, int, object, object>))
            return (T) new Func<CallSite, int, object, object>(this.IntSubtract);
          break;
        case TypeCode.Double:
          if (typeof (T) == typeof (Func<CallSite, object, object, object>))
            return (T) new Func<CallSite, object, object, object>(this.DoubleSubtract);
          break;
      }
    }
    return base.BindDelegate<T>(site, args);
  }

  private T BindEqual<T>(CallSite<T> site, object[] args) where T : class
  {
    Type type = args[0].GetType();
    if (type == typeof (string))
    {
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return (T) new Func<CallSite, object, object, object>(this.StringEqual);
      if (typeof (T) == typeof (Func<CallSite, string, object, object>))
        return (T) new Func<CallSite, string, object, object>(this.StringEqual);
      if (typeof (T) == typeof (Func<CallSite, object, string, object>))
        return (T) new Func<CallSite, object, string, object>(this.StringEqual);
    }
    else if (!type.IsEnum() && typeof (T) == typeof (Func<CallSite, object, object, object>))
    {
      switch (type.GetTypeCode())
      {
        case TypeCode.Int32:
          return (T) new Func<CallSite, object, object, object>(this.IntEqual);
        case TypeCode.Double:
          return (T) new Func<CallSite, object, object, object>(this.DoubleEqual);
      }
    }
    return base.BindDelegate<T>(site, args);
  }

  private T BindNotEqual<T>(CallSite<T> site, object[] args) where T : class
  {
    Type type = args[0].GetType();
    if (type == typeof (string))
    {
      if (typeof (T) == typeof (Func<CallSite, object, object, object>))
        return (T) new Func<CallSite, object, object, object>(this.StringNotEqual);
      if (typeof (T) == typeof (Func<CallSite, object, string, object>))
        return (T) new Func<CallSite, object, string, object>(this.StringNotEqual);
      if (typeof (T) == typeof (Func<CallSite, string, object, object>))
        return (T) new Func<CallSite, string, object, object>(this.StringNotEqual);
    }
    else if (!type.IsEnum() && typeof (T) == typeof (Func<CallSite, object, object, object>))
    {
      switch (type.GetTypeCode())
      {
        case TypeCode.Int32:
          return (T) new Func<CallSite, object, object, object>(this.IntNotEqual);
        case TypeCode.Double:
          return (T) new Func<CallSite, object, object, object>(this.DoubleNotEqual);
      }
    }
    return base.BindDelegate<T>(site, args);
  }

  private object StringModulo(CallSite site, string self, PythonDictionary other)
  {
    return (object) StringOps.Mod(this.Context.SharedContext, self, (object) other);
  }

  private object StringModulo(CallSite site, string self, PythonTuple other)
  {
    return (object) StringOps.Mod(this.Context.SharedContext, self, (object) other);
  }

  private object StringModulo(CallSite site, string self, object other)
  {
    return (object) StringOps.Mod(this.Context.SharedContext, self, other);
  }

  private object StringModulo(CallSite site, object self, PythonDictionary other)
  {
    return self != null && self.GetType() == typeof (string) ? (object) StringOps.Mod(this.Context.SharedContext, (string) self, (object) other) : ((CallSite<Func<CallSite, object, PythonDictionary, object>>) site).Update(site, self, other);
  }

  private object StringModulo(CallSite site, object self, PythonTuple other)
  {
    return self != null && self.GetType() == typeof (string) ? (object) StringOps.Mod(this.Context.SharedContext, (string) self, (object) other) : ((CallSite<Func<CallSite, object, PythonTuple, object>>) site).Update(site, self, other);
  }

  private object StringModulo(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (string) ? (object) StringOps.Mod(this.Context.SharedContext, (string) self, other) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object DoubleAdd(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (double) && other != null && other.GetType() == typeof (double) ? (object) ((double) self + (double) other) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object IntAdd(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (int) && other != null && other.GetType() == typeof (int) ? Int32Ops.Add((int) self, (int) other) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object IntAdd(CallSite site, object self, int other)
  {
    return self != null && self.GetType() == typeof (int) ? Int32Ops.Add((int) self, other) : ((CallSite<Func<CallSite, object, int, object>>) site).Update(site, self, other);
  }

  private object IntAdd(CallSite site, int self, object other)
  {
    return other != null && other.GetType() == typeof (int) ? Int32Ops.Add(self, (int) other) : ((CallSite<Func<CallSite, int, object, object>>) site).Update(site, self, other);
  }

  private object ListIntMultiply(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (List) && other != null && other.GetType() == typeof (int) ? (object) ((List) self * (int) other) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object StringIntMultiply(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (string) && other != null && other.GetType() == typeof (int) ? (object) StringOps.Multiply((string) self, (int) other) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object TupleIntMultiply(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (PythonTuple) && other != null && other.GetType() == typeof (int) ? (object) ((PythonTuple) self * (int) other) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object IntMultiply(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (int) && other != null && other.GetType() == typeof (int) ? Int32Ops.Multiply((int) self, (int) other) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object IntMultiply(CallSite site, object self, int other)
  {
    return self != null && self.GetType() == typeof (int) ? Int32Ops.Multiply((int) self, other) : ((CallSite<Func<CallSite, object, int, object>>) site).Update(site, self, other);
  }

  private object IntMultiply(CallSite site, int self, object other)
  {
    return other != null && other.GetType() == typeof (int) ? Int32Ops.Multiply(self, (int) other) : ((CallSite<Func<CallSite, int, object, object>>) site).Update(site, self, other);
  }

  private object IntDivide(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (int) && other != null && other.GetType() == typeof (int) ? Int32Ops.Divide((int) self, (int) other) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object IntDivide(CallSite site, object self, int other)
  {
    return self != null && self.GetType() == typeof (int) ? Int32Ops.Divide((int) self, other) : ((CallSite<Func<CallSite, object, int, object>>) site).Update(site, self, other);
  }

  private object IntDivide(CallSite site, int self, object other)
  {
    return other != null && other.GetType() == typeof (int) ? Int32Ops.Divide(self, (int) other) : ((CallSite<Func<CallSite, int, object, object>>) site).Update(site, self, other);
  }

  private object IntAnd(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (int) && other != null && other.GetType() == typeof (int) ? ScriptingRuntimeHelpers.Int32ToObject(Int32Ops.BitwiseAnd((int) self, (int) other)) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object IntAnd(CallSite site, object self, int other)
  {
    return self != null && self.GetType() == typeof (int) ? ScriptingRuntimeHelpers.Int32ToObject(Int32Ops.BitwiseAnd((int) self, other)) : ((CallSite<Func<CallSite, object, int, object>>) site).Update(site, self, other);
  }

  private object IntAnd(CallSite site, int self, object other)
  {
    return other != null && other.GetType() == typeof (int) ? ScriptingRuntimeHelpers.Int32ToObject(Int32Ops.BitwiseAnd(self, (int) other)) : ((CallSite<Func<CallSite, int, object, object>>) site).Update(site, self, other);
  }

  private object IntOr(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (int) && other != null && other.GetType() == typeof (int) ? ScriptingRuntimeHelpers.Int32ToObject(Int32Ops.BitwiseOr((int) self, (int) other)) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object IntOr(CallSite site, object self, int other)
  {
    return self != null && self.GetType() == typeof (int) ? ScriptingRuntimeHelpers.Int32ToObject(Int32Ops.BitwiseOr((int) self, other)) : ((CallSite<Func<CallSite, object, int, object>>) site).Update(site, self, other);
  }

  private object IntOr(CallSite site, int self, object other)
  {
    return other != null && other.GetType() == typeof (int) ? ScriptingRuntimeHelpers.Int32ToObject(Int32Ops.BitwiseOr(self, (int) other)) : ((CallSite<Func<CallSite, int, object, object>>) site).Update(site, self, other);
  }

  private object ListAdd(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (List) && other != null && other.GetType() == typeof (List) ? (object) ((List) self + (List) other) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object ListAddAssign(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (List) && other != null && other.GetType() == typeof (List) ? ((List) self).InPlaceAdd(other) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object TupleAdd(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (PythonTuple) && other != null && other.GetType() == typeof (PythonTuple) ? (object) ((PythonTuple) self + (PythonTuple) other) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object StringAdd(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (string) && other != null && other.GetType() == typeof (string) ? (object) StringOps.Add((string) self, (string) other) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object StringAdd(CallSite site, string self, object other)
  {
    return self != null && other != null && other.GetType() == typeof (string) ? (object) StringOps.Add(self, (string) other) : ((CallSite<Func<CallSite, string, object, object>>) site).Update(site, self, other);
  }

  private object StringAdd(CallSite site, object self, string other)
  {
    return self != null && self.GetType() == typeof (string) && other != null ? (object) StringOps.Add((string) self, other) : ((CallSite<Func<CallSite, object, string, object>>) site).Update(site, self, other);
  }

  private object DoubleSubtract(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (double) && other != null && other.GetType() == typeof (double) ? (object) ((double) self - (double) other) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object IntSubtract(CallSite site, object self, object other)
  {
    return self != null && self.GetType() == typeof (int) && other != null && other.GetType() == typeof (int) ? Int32Ops.Subtract((int) self, (int) other) : ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
  }

  private object IntSubtract(CallSite site, object self, int other)
  {
    return self != null && self.GetType() == typeof (int) ? Int32Ops.Subtract((int) self, other) : ((CallSite<Func<CallSite, object, int, object>>) site).Update(site, self, other);
  }

  private object IntSubtract(CallSite site, int self, object other)
  {
    return other != null && other.GetType() == typeof (int) ? Int32Ops.Subtract(self, (int) other) : ((CallSite<Func<CallSite, int, object, object>>) site).Update(site, self, other);
  }

  private object DoubleEqual(CallSite site, object self, object other)
  {
    if (self == null || !(self.GetType() == typeof (double)) || other == null || !(other.GetType() == typeof (double)))
      return ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
    return !DoubleOps.Equals((double) self, (double) other) ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object IntEqual(CallSite site, object self, object other)
  {
    if (self == null || !(self.GetType() == typeof (int)) || other == null || !(other.GetType() == typeof (int)))
      return ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
    return (int) self != (int) other ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object StringEqual(CallSite site, object self, object other)
  {
    if (self == null || !(self.GetType() == typeof (string)) || other == null || !(other.GetType() == typeof (string)))
      return ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
    return !StringOps.Equals((string) self, (string) other) ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object StringEqual(CallSite site, string self, object other)
  {
    if (self == null || other == null || !(other.GetType() == typeof (string)))
      return ((CallSite<Func<CallSite, string, object, object>>) site).Update(site, self, other);
    return !StringOps.Equals(self, (string) other) ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object StringEqual(CallSite site, object self, string other)
  {
    if (self == null || !(self.GetType() == typeof (string)) || other == null)
      return ((CallSite<Func<CallSite, object, string, object>>) site).Update(site, self, other);
    return !StringOps.Equals((string) self, other) ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object DoubleNotEqual(CallSite site, object self, object other)
  {
    if (self == null || !(self.GetType() == typeof (double)) || other == null || !(other.GetType() == typeof (double)))
      return ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
    return !DoubleOps.NotEquals((double) self, (double) other) ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object IntNotEqual(CallSite site, object self, object other)
  {
    if (self == null || !(self.GetType() == typeof (int)) || other == null || !(other.GetType() == typeof (int)))
      return ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
    return (int) self == (int) other ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object StringNotEqual(CallSite site, object self, object other)
  {
    if (self == null || !(self.GetType() == typeof (string)) || other == null || !(other.GetType() == typeof (string)))
      return ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
    return !StringOps.NotEquals((string) self, (string) other) ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object StringNotEqual(CallSite site, string self, object other)
  {
    if (self == null || other == null || !(other.GetType() == typeof (string)))
      return ((CallSite<Func<CallSite, string, object, object>>) site).Update(site, self, other);
    return !StringOps.NotEquals(self, (string) other) ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object StringNotEqual(CallSite site, object self, string other)
  {
    if (self == null || !(self.GetType() == typeof (string)) || other == null)
      return ((CallSite<Func<CallSite, object, string, object>>) site).Update(site, self, other);
    return !StringOps.NotEquals((string) self, other) ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object IntGreaterThan(CallSite site, object self, object other)
  {
    if (self == null || !(self.GetType() == typeof (int)) || other == null || !(other.GetType() == typeof (int)))
      return ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
    return (int) self <= (int) other ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object IntGreaterThan(CallSite site, object self, int other)
  {
    if (self == null || !(self.GetType() == typeof (int)))
      return ((CallSite<Func<CallSite, object, int, object>>) site).Update(site, self, other);
    return (int) self <= other ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object IntGreaterThan(CallSite site, int self, object other)
  {
    return other != null && other.GetType() == typeof (int) ? (object) (self > (int) other) : ((CallSite<Func<CallSite, int, object, object>>) site).Update(site, self, other);
  }

  private object IntLessThan(CallSite site, object self, object other)
  {
    if (self == null || !(self.GetType() == typeof (int)) || other == null || !(other.GetType() == typeof (int)))
      return ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
    return (int) self >= (int) other ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object IntLessThan(CallSite site, object self, int other)
  {
    if (self == null || !(self.GetType() == typeof (int)))
      return ((CallSite<Func<CallSite, object, int, object>>) site).Update(site, self, other);
    return (int) self >= other ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object IntLessThan(CallSite site, int self, object other)
  {
    if (other == null || !(other.GetType() == typeof (int)))
      return ((CallSite<Func<CallSite, int, object, object>>) site).Update(site, self, other);
    return self >= (int) other ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object IntGreaterThanOrEqual(CallSite site, object self, object other)
  {
    if (self == null || !(self.GetType() == typeof (int)) || other == null || !(other.GetType() == typeof (int)))
      return ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
    return (int) self < (int) other ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object IntGreaterThanOrEqual(CallSite site, object self, int other)
  {
    if (self == null || !(self.GetType() == typeof (int)))
      return ((CallSite<Func<CallSite, object, int, object>>) site).Update(site, self, other);
    return (int) self < other ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object IntGreaterThanOrEqual(CallSite site, int self, object other)
  {
    if (other == null || !(other.GetType() == typeof (int)))
      return ((CallSite<Func<CallSite, int, object, object>>) site).Update(site, self, other);
    return self < (int) other ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object IntLessThanOrEqual(CallSite site, object self, object other)
  {
    if (self == null || !(self.GetType() == typeof (int)) || other == null || !(other.GetType() == typeof (int)))
      return ((CallSite<Func<CallSite, object, object, object>>) site).Update(site, self, other);
    return (int) self > (int) other ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object IntLessThanOrEqual(CallSite site, object self, int other)
  {
    if (self == null || !(self.GetType() == typeof (int)))
      return ((CallSite<Func<CallSite, object, int, object>>) site).Update(site, self, other);
    return (int) self > other ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  private object IntLessThanOrEqual(CallSite site, int self, object other)
  {
    if (other == null || !(other.GetType() == typeof (int)))
      return ((CallSite<Func<CallSite, int, object, object>>) site).Update(site, self, other);
    return self > (int) other ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  public override int GetHashCode() => base.GetHashCode() ^ this._context.Binder.GetHashCode();

  public override bool Equals(object obj)
  {
    return obj is PythonBinaryOperationBinder binaryOperationBinder && binaryOperationBinder._context.Binder == this._context.Binder && base.Equals(obj);
  }

  public PythonContext Context => this._context;

  public override string ToString() => "PythonBinary " + (object) this.Operation;

  public Expression CreateExpression()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeBinaryOperationAction"), BindingHelpers.CreateBinderStateExpression(), Microsoft.Scripting.Ast.Utils.Constant((object) this.Operation));
  }

  public virtual bool SupportsLightThrow => false;

  public virtual CallSiteBinder GetLightExceptionBinder()
  {
    if (this._lightThrowBinder == null)
      this._lightThrowBinder = (PythonBinaryOperationBinder) new PythonBinaryOperationBinder.LightThrowBinder(this._context, this.Operation);
    return (CallSiteBinder) this._lightThrowBinder;
  }

  private class LightThrowBinder(PythonContext context, ExpressionType operation) : 
    PythonBinaryOperationBinder(context, operation)
  {
    public override bool SupportsLightThrow => true;

    public override CallSiteBinder GetLightExceptionBinder() => (CallSiteBinder) this;
  }
}
