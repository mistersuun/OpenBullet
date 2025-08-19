// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonConversionBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.ComInterop;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class PythonConversionBinder : DynamicMetaObjectBinder, IPythonSite, IExpressionSerializable
{
  private readonly PythonContext _context;
  private readonly ConversionResultKind _kind;
  private readonly Type _type;
  private readonly bool _retObject;
  private CompatConversionBinder _compatConvert;

  public PythonConversionBinder(PythonContext context, Type type, ConversionResultKind resultKind)
  {
    this._context = context;
    this._kind = resultKind;
    this._type = type;
  }

  public PythonConversionBinder(
    PythonContext context,
    Type type,
    ConversionResultKind resultKind,
    bool retObject)
  {
    this._context = context;
    this._kind = resultKind;
    this._type = type;
    this._retObject = retObject;
  }

  public Type Type => this._type;

  public ConversionResultKind ResultKind => this._kind;

  public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
  {
    DynamicMetaObject self = target;
    DynamicMetaObject dynamicMetaObject = (DynamicMetaObject) null;
    if (self.NeedsDeferral())
      return this.MyDefer(self);
    if (target is IPythonConvertible pythonConvertible)
      dynamicMetaObject = pythonConvertible.BindConvert(this);
    else if (dynamicMetaObject == null)
      dynamicMetaObject = this.BindConvert(self);
    if (this._retObject)
      dynamicMetaObject = new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Convert(dynamicMetaObject.Expression, typeof (object)), dynamicMetaObject.Restrictions);
    return dynamicMetaObject;
  }

  public override Type ReturnType
  {
    get
    {
      if (this._retObject)
        return typeof (object);
      if (this._kind == ConversionResultKind.ExplicitCast || this._kind == ConversionResultKind.ImplicitCast)
        return this.Type;
      return !this._type.IsValueType() ? this._type : typeof (object);
    }
  }

  private DynamicMetaObject MyDefer(DynamicMetaObject self)
  {
    return new DynamicMetaObject((Expression) Expression.Dynamic((CallSiteBinder) this, this.ReturnType, self.Expression), self.Restrictions);
  }

  private DynamicMetaObject BindConvert(DynamicMetaObject self)
  {
    DynamicMetaObject result;
    DynamicMetaObject dynamicMetaObject = !ComBinder.TryConvert((ConvertBinder) this.CompatBinder, self, out result) ? self.BindConvert((ConvertBinder) this.CompatBinder) : result;
    if (this.ReturnType == typeof (object) && dynamicMetaObject.Expression.Type != typeof (object) && dynamicMetaObject.Expression.NodeType == ExpressionType.Convert)
      dynamicMetaObject = new DynamicMetaObject(((UnaryExpression) dynamicMetaObject.Expression).Operand, dynamicMetaObject.Restrictions);
    return dynamicMetaObject;
  }

  internal CompatConversionBinder CompatBinder
  {
    get
    {
      if (this._compatConvert == null)
        this._compatConvert = new CompatConversionBinder(this, this.Type, this._kind == ConversionResultKind.ExplicitCast || this._kind == ConversionResultKind.ExplicitTry);
      return this._compatConvert;
    }
  }

  internal DynamicMetaObject FallbackConvert(
    Type returnType,
    DynamicMetaObject self,
    DynamicMetaObject errorSuggestion)
  {
    Type type = this.Type;
    DynamicMetaObject dynamicMetaObject = (DynamicMetaObject) null;
    switch (type.GetTypeCode())
    {
      case TypeCode.Object:
        if (type.IsArray && self.Value is PythonTuple && type.GetArrayRank() == 1)
        {
          dynamicMetaObject = PythonConversionBinder.MakeToArrayConversion(self, type);
          break;
        }
        if (type.IsGenericType && !type.IsAssignableFrom(CompilerHelpers.GetType(self.Value)))
        {
          Type genericTypeDefinition = type.GetGenericTypeDefinition();
          if (genericTypeDefinition == typeof (IList<>))
          {
            dynamicMetaObject = !(self.LimitType == typeof (string)) ? PythonConversionBinder.TryToGenericInterfaceConversion(self, type, typeof (IList<object>), typeof (ListGenericWrapper<>)) : new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeByteArray"), Microsoft.Scripting.Ast.Utils.Convert(self.Expression, typeof (string))), BindingRestrictions.GetTypeRestriction(self.Expression, typeof (string)));
            break;
          }
          if (genericTypeDefinition == typeof (IDictionary<,>))
          {
            dynamicMetaObject = PythonConversionBinder.TryToGenericInterfaceConversion(self, type, typeof (IDictionary<object, object>), typeof (DictionaryGenericWrapper<,>));
            break;
          }
          if (genericTypeDefinition == typeof (IEnumerable<>))
          {
            dynamicMetaObject = PythonConversionBinder.TryToGenericInterfaceConversion(self, type, typeof (IEnumerable), typeof (IEnumerableOfTWrapper<>));
            break;
          }
          break;
        }
        if (type == typeof (IEnumerable))
        {
          if (!typeof (IEnumerable).IsAssignableFrom(self.GetLimitType()) && PythonConversionBinder.IsIndexless(self))
          {
            dynamicMetaObject = PythonConversionBinder.ConvertToIEnumerable((DynamicMetaObjectBinder) this, self.Restrict(self.GetLimitType()));
            break;
          }
          break;
        }
        if (type == typeof (IEnumerator) && !typeof (IEnumerator).IsAssignableFrom(self.GetLimitType()) && !typeof (IEnumerable).IsAssignableFrom(self.GetLimitType()) && PythonConversionBinder.IsIndexless(self))
        {
          dynamicMetaObject = PythonConversionBinder.ConvertToIEnumerator((DynamicMetaObjectBinder) this, self.Restrict(self.GetLimitType()));
          break;
        }
        break;
      case TypeCode.Boolean:
        dynamicMetaObject = this.MakeToBoolConversion(self);
        break;
      case TypeCode.Char:
        dynamicMetaObject = this.TryToCharConversion(self);
        break;
      case TypeCode.String:
        Type limitType = self.GetLimitType();
        if ((limitType == typeof (Bytes) || limitType == typeof (PythonBuffer) || limitType == typeof (ByteArray)) && !this._context.PythonOptions.Python30)
        {
          dynamicMetaObject = new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeString"), Microsoft.Scripting.Ast.Utils.Convert(self.Expression, typeof (IList<byte>))), BindingRestrictionsHelpers.GetRuntimeTypeRestriction(self.Expression, limitType));
          break;
        }
        break;
    }
    if (!type.IsEnum() || !(Enum.GetUnderlyingType(type) == self.GetLimitType()))
      return dynamicMetaObject ?? PythonConversionBinder.EnsureReturnType(returnType, this.Context.Binder.ConvertTo(this.Type, this.ResultKind, self, (OverloadResolverFactory) this._context.SharedOverloadResolverFactory, errorSuggestion));
    object instance = Activator.CreateInstance(type);
    return new DynamicMetaObject((Expression) Expression.Condition((Expression) Expression.Equal(Microsoft.Scripting.Ast.Utils.Convert(self.Expression, Enum.GetUnderlyingType(type)), Microsoft.Scripting.Ast.Utils.Constant(Activator.CreateInstance(self.GetLimitType()))), Microsoft.Scripting.Ast.Utils.Constant(instance), (Expression) Expression.Call(typeof (PythonOps).GetMethod("TypeErrorForBadEnumConversion").MakeGenericMethod(type), Microsoft.Scripting.Ast.Utils.Convert(self.Expression, typeof (object)))), self.Restrictions.Merge(BindingRestrictionsHelpers.GetRuntimeTypeRestriction(self.Expression, self.GetLimitType())), instance);
  }

  private static DynamicMetaObject EnsureReturnType(
    Type returnType,
    DynamicMetaObject dynamicMetaObject)
  {
    if (dynamicMetaObject.Expression.Type != returnType)
      dynamicMetaObject = new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Convert(dynamicMetaObject.Expression, returnType), dynamicMetaObject.Restrictions);
    return dynamicMetaObject;
  }

  public override T BindDelegate<T>(CallSite<T> site, object[] args)
  {
    object obj = args[0];
    T target = default (T);
    if (typeof (T) == typeof (Func<CallSite, object, string>) && obj is string)
      target = (T) new Func<CallSite, object, string>(this.StringConversion);
    else if (typeof (T) == typeof (Func<CallSite, object, int>))
    {
      if (obj is int)
        target = (T) new Func<CallSite, object, int>(this.IntConversion);
      else if (obj is bool)
        target = (T) new Func<CallSite, object, int>(this.BoolToIntConversion);
    }
    else if (typeof (T) == typeof (Func<CallSite, bool, int>))
      target = (T) new Func<CallSite, bool, int>(this.BoolToIntConversion);
    else if (typeof (T) == typeof (Func<CallSite, object, bool>))
    {
      switch (obj)
      {
        case bool _:
          target = (T) new Func<CallSite, object, bool>(this.BoolConversion);
          break;
        case string _:
          target = (T) new Func<CallSite, object, bool>(this.StringToBoolConversion);
          break;
        case int _:
          target = (T) new Func<CallSite, object, bool>(this.IntToBoolConversion);
          break;
        case null:
          target = (T) new Func<CallSite, object, bool>(this.NullToBoolConversion);
          break;
        default:
          if (obj.GetType() == typeof (object))
          {
            target = (T) new Func<CallSite, object, bool>(this.ObjectToBoolConversion);
            break;
          }
          if (obj.GetType() == typeof (List))
          {
            target = (T) new Func<CallSite, object, bool>(this.ListToBoolConversion);
            break;
          }
          if (obj.GetType() == typeof (PythonTuple))
          {
            target = (T) new Func<CallSite, object, bool>(this.TupleToBoolConversion);
            break;
          }
          break;
      }
    }
    else if (obj != null)
    {
      if (obj is BigInteger)
      {
        if (typeof (T) == typeof (Func<CallSite, BigInteger, Complex>))
          target = (T) new Func<CallSite, BigInteger, Complex>(this.BigIntegerToComplexConversion);
        else if (typeof (T) == typeof (Func<CallSite, object, Complex>))
          target = (T) new Func<CallSite, object, Complex>(this.BigIntegerObjectToComplexConversion);
        else if (typeof (T) == typeof (Func<CallSite, BigInteger, object>))
          target = (T) new Func<CallSite, BigInteger, object>(this.BigIntegerToComplexObjectConversion);
      }
      else if (obj is string)
      {
        if (typeof (T) == typeof (Func<CallSite, string, IEnumerable>))
          target = (T) new Func<CallSite, string, IEnumerable>(this.StringToIEnumerableConversion);
        else if (typeof (T) == typeof (Func<CallSite, string, IEnumerator>))
          target = (T) new Func<CallSite, string, IEnumerator>(this.StringToIEnumeratorConversion);
        else if (typeof (T) == typeof (Func<CallSite, object, IEnumerable>))
          target = (T) new Func<CallSite, object, IEnumerable>(this.ObjectToIEnumerableConversion);
        else if (typeof (T) == typeof (Func<CallSite, object, IEnumerator>))
          target = (T) new Func<CallSite, object, IEnumerator>(this.ObjectToIEnumeratorConversion);
      }
      else if (obj.GetType() == typeof (Bytes))
      {
        if (typeof (T) == typeof (Func<CallSite, Bytes, IEnumerable>))
          target = (T) new Func<CallSite, Bytes, IEnumerable>(this.BytesToIEnumerableConversion);
        else if (typeof (T) == typeof (Func<CallSite, Bytes, IEnumerator>))
          target = (T) new Func<CallSite, Bytes, IEnumerator>(this.BytesToIEnumeratorConversion);
        else if (typeof (T) == typeof (Func<CallSite, object, IEnumerable>))
          target = (T) new Func<CallSite, object, IEnumerable>(this.ObjectToIEnumerableConversion);
        else if (typeof (T) == typeof (Func<CallSite, object, IEnumerator>))
          target = (T) new Func<CallSite, object, IEnumerator>(this.ObjectToIEnumeratorConversion);
      }
      if ((object) target == null && (obj.GetType() == this.Type || this.Type.IsAssignableFrom(obj.GetType())))
      {
        if (typeof (T) == typeof (Func<CallSite, object, object>))
          target = (T) new Func<CallSite, object, object>(new PythonConversionBinder.IdentityConversion(obj.GetType()).Convert);
        else if (typeof (T).GetMethod("Invoke").GetParameters()[1].ParameterType == typeof (object))
        {
          object instance = Activator.CreateInstance(typeof (PythonConversionBinder.IdentityConversion<>).MakeGenericType(this.Type), (object) obj.GetType());
          target = (T) instance.GetType().GetMethod("Convert").CreateDelegate(typeof (T), instance);
        }
      }
    }
    if ((object) target == null)
      return base.BindDelegate<T>(site, args);
    this.CacheTarget<T>(target);
    return target;
  }

  public string StringConversion(CallSite site, object value)
  {
    return value is string str ? str : ((CallSite<Func<CallSite, object, string>>) site).Update(site, value);
  }

  public int IntConversion(CallSite site, object value)
  {
    return value is int num ? num : ((CallSite<Func<CallSite, object, int>>) site).Update(site, value);
  }

  public int BoolToIntConversion(CallSite site, object value)
  {
    if (!(value is bool flag))
      return ((CallSite<Func<CallSite, object, int>>) site).Update(site, value);
    return !flag ? 0 : 1;
  }

  public int BoolToIntConversion(CallSite site, bool value) => !value ? 0 : 1;

  public bool BoolConversion(CallSite site, object value)
  {
    if (value is bool flag)
      return flag;
    return value != null && ((CallSite<Func<CallSite, object, bool>>) site).Update(site, value);
  }

  public bool IntToBoolConversion(CallSite site, object value)
  {
    if (value is int num)
      return num != 0;
    return value != null && ((CallSite<Func<CallSite, object, bool>>) site).Update(site, value);
  }

  public bool StringToBoolConversion(CallSite site, object value)
  {
    if (value is string)
      return ((string) value).Length > 0;
    return value != null && ((CallSite<Func<CallSite, object, bool>>) site).Update(site, value);
  }

  public bool NullToBoolConversion(CallSite site, object value)
  {
    return value != null && ((CallSite<Func<CallSite, object, bool>>) site).Update(site, value);
  }

  public bool ObjectToBoolConversion(CallSite site, object value)
  {
    if (value != null && value.GetType() == typeof (object))
      return true;
    return value != null && ((CallSite<Func<CallSite, object, bool>>) site).Update(site, value);
  }

  public bool ListToBoolConversion(CallSite site, object value)
  {
    if (value == null)
      return false;
    return value.GetType() == typeof (List) ? ((List) value).Count != 0 : ((CallSite<Func<CallSite, object, bool>>) site).Update(site, value);
  }

  public bool TupleToBoolConversion(CallSite site, object value)
  {
    if (value == null)
      return false;
    return value.GetType() == typeof (PythonTuple) ? ((PythonTuple) value).Count != 0 : ((CallSite<Func<CallSite, object, bool>>) site).Update(site, value);
  }

  public IEnumerable StringToIEnumerableConversion(CallSite site, string value)
  {
    return value == null ? ((CallSite<Func<CallSite, string, IEnumerable>>) site).Update(site, value) : PythonOps.StringEnumerable(value);
  }

  public IEnumerator StringToIEnumeratorConversion(CallSite site, string value)
  {
    return value == null ? ((CallSite<Func<CallSite, string, IEnumerator>>) site).Update(site, value) : PythonOps.StringEnumerator(value).Key;
  }

  public IEnumerable BytesToIEnumerableConversion(CallSite site, Bytes value)
  {
    if (value == null)
      return ((CallSite<Func<CallSite, Bytes, IEnumerable>>) site).Update(site, value);
    return !this._context.PythonOptions.Python30 ? PythonOps.BytesEnumerable((IList<byte>) value) : PythonOps.BytesIntEnumerable((IList<byte>) value);
  }

  public IEnumerator BytesToIEnumeratorConversion(CallSite site, Bytes value)
  {
    if (value == null)
      return ((CallSite<Func<CallSite, Bytes, IEnumerator>>) site).Update(site, value);
    return !this._context.PythonOptions.Python30 ? PythonOps.BytesEnumerator((IList<byte>) value).Key : PythonOps.BytesIntEnumerator((IList<byte>) value).Key;
  }

  public IEnumerable ObjectToIEnumerableConversion(CallSite site, object value)
  {
    if (value != null)
    {
      if (value is string)
        return PythonOps.StringEnumerable((string) value);
      if (value.GetType() == typeof (Bytes))
        return !this._context.PythonOptions.Python30 ? PythonOps.BytesEnumerable((IList<byte>) value) : PythonOps.BytesIntEnumerable((IList<byte>) value);
    }
    return ((CallSite<Func<CallSite, object, IEnumerable>>) site).Update(site, value);
  }

  public IEnumerator ObjectToIEnumeratorConversion(CallSite site, object value)
  {
    if (value != null)
    {
      if (value is string)
        return PythonOps.StringEnumerator((string) value).Key;
      if (value.GetType() == typeof (Bytes))
        return !this._context.PythonOptions.Python30 ? PythonOps.BytesEnumerator((IList<byte>) value).Key : PythonOps.BytesIntEnumerator((IList<byte>) value).Key;
    }
    return ((CallSite<Func<CallSite, object, IEnumerator>>) site).Update(site, value);
  }

  public Complex BigIntegerToComplexConversion(CallSite site, BigInteger value)
  {
    return BigIntegerOps.ConvertToComplex(value);
  }

  public Complex BigIntegerObjectToComplexConversion(CallSite site, object value)
  {
    return value is BigInteger self ? BigIntegerOps.ConvertToComplex(self) : ((CallSite<Func<CallSite, object, Complex>>) site).Update(site, value);
  }

  public object BigIntegerToComplexObjectConversion(CallSite site, BigInteger value)
  {
    return (object) BigIntegerOps.ConvertToComplex(value);
  }

  internal static bool IsIndexless(DynamicMetaObject arg)
  {
    return arg.GetLimitType() != typeof (OldInstance);
  }

  public override int GetHashCode()
  {
    return base.GetHashCode() ^ this._context.Binder.GetHashCode() ^ this._kind.GetHashCode();
  }

  public override bool Equals(object obj)
  {
    return obj is PythonConversionBinder conversionBinder && conversionBinder._context.Binder == this._context.Binder && this._kind == conversionBinder._kind && base.Equals(obj) && this._retObject == conversionBinder._retObject;
  }

  public PythonContext Context => this._context;

  private static DynamicMetaObject TryToGenericInterfaceConversion(
    DynamicMetaObject self,
    Type toType,
    Type fromType,
    Type wrapperType)
  {
    if (!fromType.IsAssignableFrom(CompilerHelpers.GetType(self.Value)))
      return (DynamicMetaObject) null;
    Type type = wrapperType.MakeGenericType(toType.GetGenericArguments());
    self = self.Restrict(CompilerHelpers.GetType(self.Value));
    Type[] types = new Type[1]{ fromType };
    return new DynamicMetaObject((Expression) Expression.New(type.GetConstructor(types), Microsoft.Scripting.Ast.Utils.Convert(self.Expression, fromType)), self.Restrictions);
  }

  private static DynamicMetaObject MakeToArrayConversion(DynamicMetaObject self, Type toType)
  {
    self = self.Restrict(typeof (PythonTuple));
    return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod("ConvertTupleToArray").MakeGenericMethod(toType.GetElementType()), self.Expression), self.Restrictions);
  }

  private DynamicMetaObject TryToCharConversion(DynamicMetaObject self)
  {
    string str = self.Value as string;
    Expression expression = self.Expression;
    if (str == null && self.Value is Extensible<string> extensible)
    {
      str = extensible.Value;
      expression = (Expression) Expression.Property(Microsoft.Scripting.Ast.Utils.Convert(expression, typeof (Extensible<string>)), typeof (Extensible<string>).GetProperty("Value"));
    }
    DynamicMetaObject charConversion;
    if (str != null)
    {
      self = self.Restrict(self.GetRuntimeType());
      Expression left = (Expression) Expression.Property(Microsoft.Scripting.Ast.Utils.Convert(expression, typeof (string)), typeof (string).GetProperty("Length"));
      if (str.Length == 1)
        charConversion = new DynamicMetaObject((Expression) Expression.Call(Microsoft.Scripting.Ast.Utils.Convert(expression, typeof (string)), typeof (string).GetMethod("get_Chars"), Microsoft.Scripting.Ast.Utils.Constant((object) 0)), self.Restrictions.Merge(BindingRestrictions.GetExpressionRestriction((Expression) Expression.Equal(left, Microsoft.Scripting.Ast.Utils.Constant((object) 1)))));
      else
        charConversion = new DynamicMetaObject(this.Throw((Expression) Expression.Call(typeof (PythonOps).GetMethod("TypeError"), Microsoft.Scripting.Ast.Utils.Constant((object) "expected string of length 1 when converting to char, got '{0}'"), (Expression) Expression.NewArrayInit(typeof (object), self.Expression)), this.ReturnType), self.Restrictions.Merge(BindingRestrictions.GetExpressionRestriction((Expression) Expression.NotEqual(left, Microsoft.Scripting.Ast.Utils.Constant((object) 1)))));
    }
    else
      charConversion = (DynamicMetaObject) null;
    return charConversion;
  }

  private DynamicMetaObject MakeToBoolConversion(DynamicMetaObject self)
  {
    if (self.HasValue)
      self = self.Restrict(self.GetRuntimeType());
    if (self.Expression.NodeType == ExpressionType.Convert && self.Expression.Type == typeof (object))
    {
      UnaryExpression expression = (UnaryExpression) self.Expression;
      if (expression.Operand.Type == typeof (bool))
        return new DynamicMetaObject(expression.Operand, self.Restrictions);
    }
    if (self.GetLimitType() == typeof (DynamicNull))
      return PythonConversionBinder.MakeNoneToBoolConversion(self);
    if (self.GetLimitType() == typeof (bool))
      return self;
    if (typeof (IStrongBox).IsAssignableFrom(self.GetLimitType()))
      return this.MakeStrongBoxToBoolConversionError(self);
    return self.GetLimitType().IsPrimitive() || self.GetLimitType().IsEnum() ? PythonConversionBinder.MakePrimitiveToBoolComparison(self) : PythonProtocol.ConvertToBool((DynamicMetaObjectBinder) this, self) ?? new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) true), self.Restrictions);
  }

  private static DynamicMetaObject MakeNoneToBoolConversion(DynamicMetaObject self)
  {
    return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Constant((object) false), self.Restrictions);
  }

  private static DynamicMetaObject MakePrimitiveToBoolComparison(DynamicMetaObject self)
  {
    return new DynamicMetaObject((Expression) Expression.NotEqual(Microsoft.Scripting.Ast.Utils.Constant(Activator.CreateInstance(self.GetLimitType())), self.Expression), self.Restrictions);
  }

  private DynamicMetaObject MakeStrongBoxToBoolConversionError(DynamicMetaObject self)
  {
    return new DynamicMetaObject(this.Throw((Expression) Expression.Call(typeof (ScriptingRuntimeHelpers).GetMethod("SimpleTypeError"), Microsoft.Scripting.Ast.Utils.Constant((object) "Can't convert a Reference<> instance to a bool")), this.ReturnType), self.Restrictions);
  }

  internal static DynamicMetaObject ConvertToIEnumerable(
    DynamicMetaObjectBinder conversion,
    DynamicMetaObject metaUserObject)
  {
    PythonType pythonType = MetaPythonObject.GetPythonType(metaUserObject);
    PythonContext pythonContext = PythonContext.GetPythonContext(conversion);
    CodeContext sharedContext = pythonContext.SharedContext;
    PythonTypeSlot slot;
    if (pythonType.TryResolveSlot(sharedContext, "__iter__", out slot))
      return PythonConversionBinder.MakeIterRule(metaUserObject, "CreatePythonEnumerable");
    return pythonType.TryResolveSlot(sharedContext, "__getitem__", out slot) ? PythonConversionBinder.MakeGetItemIterable(metaUserObject, pythonContext, slot, "CreateItemEnumerable") : (DynamicMetaObject) null;
  }

  internal static DynamicMetaObject ConvertToIEnumerator(
    DynamicMetaObjectBinder conversion,
    DynamicMetaObject metaUserObject)
  {
    PythonType pythonType = MetaPythonObject.GetPythonType(metaUserObject);
    PythonContext pythonContext = PythonContext.GetPythonContext(conversion);
    CodeContext sharedContext = pythonContext.SharedContext;
    PythonTypeSlot slot;
    if (pythonType.TryResolveSlot(sharedContext, "__iter__", out slot))
    {
      ParameterExpression tmp = Expression.Parameter(typeof (object), "iterVal");
      return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
      {
        tmp
      }, (Expression) Expression.Call(typeof (PythonOps).GetMethod("CreatePythonEnumerator"), (Expression) Expression.Block((Expression) MetaPythonObject.MakeTryGetTypeMember(pythonContext, slot, metaUserObject.Expression, tmp), (Expression) Expression.Dynamic((CallSiteBinder) new PythonInvokeBinder(pythonContext, new CallSignature(0)), typeof (object), Microsoft.Scripting.Ast.Utils.Constant((object) sharedContext), (Expression) tmp)))), metaUserObject.Restrictions);
    }
    return pythonType.TryResolveSlot(sharedContext, "__getitem__", out slot) ? PythonConversionBinder.MakeGetItemIterable(metaUserObject, pythonContext, slot, "CreateItemEnumerator") : (DynamicMetaObject) null;
  }

  private static DynamicMetaObject MakeGetItemIterable(
    DynamicMetaObject metaUserObject,
    PythonContext state,
    PythonTypeSlot pts,
    string method)
  {
    ParameterExpression tmp = Expression.Parameter(typeof (object), "getitemVal");
    return new DynamicMetaObject((Expression) Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      tmp
    }, (Expression) Expression.Call(typeof (PythonOps).GetMethod(method), (Expression) Expression.Block((Expression) MetaPythonObject.MakeTryGetTypeMember(state, pts, tmp, metaUserObject.Expression, (Expression) Expression.Call(typeof (DynamicHelpers).GetMethod("GetPythonType"), Microsoft.Scripting.Ast.Utils.Convert(metaUserObject.Expression, typeof (object)))), (Expression) tmp), Microsoft.Scripting.Ast.Utils.Constant((object) CallSite<Func<CallSite, CodeContext, object, int, object>>.Create((CallSiteBinder) new PythonInvokeBinder(state, new CallSignature(1)))))), metaUserObject.Restrictions);
  }

  private static DynamicMetaObject MakeIterRule(DynamicMetaObject self, string methodName)
  {
    return new DynamicMetaObject((Expression) Expression.Call(typeof (PythonOps).GetMethod(methodName), Microsoft.Scripting.Ast.Utils.Convert(self.Expression, typeof (object))), self.Restrictions);
  }

  public override string ToString() => $"Python Convert {this.Type} {this.ResultKind}";

  public Expression CreateExpression()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeConversionAction"), BindingHelpers.CreateBinderStateExpression(), Microsoft.Scripting.Ast.Utils.Constant((object) this.Type), Microsoft.Scripting.Ast.Utils.Constant((object) this.ResultKind));
  }

  private class IdentityConversion
  {
    private readonly Type _type;

    public IdentityConversion(Type type) => this._type = type;

    public object Convert(CallSite site, object value)
    {
      return value != null && value.GetType() == this._type ? value : ((CallSite<Func<CallSite, object, object>>) site).Update(site, value);
    }
  }

  private class IdentityConversion<T>
  {
    private readonly Type _type;

    public IdentityConversion(Type type) => this._type = type;

    public T Convert(CallSite site, object value)
    {
      return value != null && value.GetType() == this._type ? (T) value : ((CallSite<Func<CallSite, object, T>>) site).Update(site, value);
    }
  }
}
