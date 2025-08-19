// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Converter
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

public static class Converter
{
  private static readonly CallSite<Func<CallSite, object, int>> _intSite = Converter.MakeExplicitConvertSite<int>();
  private static readonly CallSite<Func<CallSite, object, double>> _doubleSite = Converter.MakeExplicitConvertSite<double>();
  private static readonly CallSite<Func<CallSite, object, Complex>> _complexSite = Converter.MakeExplicitConvertSite<Complex>();
  private static readonly CallSite<Func<CallSite, object, BigInteger>> _bigIntSite = Converter.MakeExplicitConvertSite<BigInteger>();
  private static readonly CallSite<Func<CallSite, object, string>> _stringSite = Converter.MakeExplicitConvertSite<string>();
  private static readonly CallSite<Func<CallSite, object, bool>> _boolSite = Converter.MakeExplicitConvertSite<bool>();
  private static readonly CallSite<Func<CallSite, object, char>> _charSite = Converter.MakeImplicitConvertSite<char>();
  private static readonly CallSite<Func<CallSite, object, char>> _explicitCharSite = Converter.MakeExplicitConvertSite<char>();
  private static readonly CallSite<Func<CallSite, object, IEnumerable>> _ienumerableSite = Converter.MakeImplicitConvertSite<IEnumerable>();
  private static readonly CallSite<Func<CallSite, object, IEnumerator>> _ienumeratorSite = Converter.MakeImplicitConvertSite<IEnumerator>();
  private static readonly Dictionary<Type, CallSite<Func<CallSite, object, object>>> _siteDict = new Dictionary<Type, CallSite<Func<CallSite, object, object>>>();
  private static readonly CallSite<Func<CallSite, object, byte>> _byteSite = Converter.MakeExplicitConvertSite<byte>();
  private static readonly CallSite<Func<CallSite, object, sbyte>> _sbyteSite = Converter.MakeExplicitConvertSite<sbyte>();
  private static readonly CallSite<Func<CallSite, object, short>> _int16Site = Converter.MakeExplicitConvertSite<short>();
  private static readonly CallSite<Func<CallSite, object, ushort>> _uint16Site = Converter.MakeExplicitConvertSite<ushort>();
  private static readonly CallSite<Func<CallSite, object, uint>> _uint32Site = Converter.MakeExplicitConvertSite<uint>();
  private static readonly CallSite<Func<CallSite, object, long>> _int64Site = Converter.MakeExplicitConvertSite<long>();
  private static readonly CallSite<Func<CallSite, object, ulong>> _uint64Site = Converter.MakeExplicitConvertSite<ulong>();
  private static readonly CallSite<Func<CallSite, object, Decimal>> _decimalSite = Converter.MakeExplicitConvertSite<Decimal>();
  private static readonly CallSite<Func<CallSite, object, float>> _floatSite = Converter.MakeExplicitConvertSite<float>();
  private static readonly CallSite<Func<CallSite, object, object>> _tryByteSite = Converter.MakeExplicitTrySite<byte>();
  private static readonly CallSite<Func<CallSite, object, object>> _trySByteSite = Converter.MakeExplicitTrySite<sbyte>();
  private static readonly CallSite<Func<CallSite, object, object>> _tryInt16Site = Converter.MakeExplicitTrySite<short>();
  private static readonly CallSite<Func<CallSite, object, object>> _tryInt32Site = Converter.MakeExplicitTrySite<int>();
  private static readonly CallSite<Func<CallSite, object, object>> _tryInt64Site = Converter.MakeExplicitTrySite<long>();
  private static readonly CallSite<Func<CallSite, object, object>> _tryUInt16Site = Converter.MakeExplicitTrySite<ushort>();
  private static readonly CallSite<Func<CallSite, object, object>> _tryUInt32Site = Converter.MakeExplicitTrySite<uint>();
  private static readonly CallSite<Func<CallSite, object, object>> _tryUInt64Site = Converter.MakeExplicitTrySite<ulong>();
  private static readonly CallSite<Func<CallSite, object, object>> _tryDoubleSite = Converter.MakeExplicitTrySite<double>();
  private static readonly CallSite<Func<CallSite, object, object>> _tryCharSite = Converter.MakeExplicitTrySite<char>();
  private static readonly CallSite<Func<CallSite, object, object>> _tryBigIntegerSite = Converter.MakeExplicitTrySite<BigInteger>();
  private static readonly CallSite<Func<CallSite, object, object>> _tryComplexSite = Converter.MakeExplicitTrySite<Complex>();
  private static readonly CallSite<Func<CallSite, object, object>> _tryStringSite = Converter.MakeExplicitTrySite<string>();
  private static readonly Type StringType = typeof (string);
  private static readonly Type Int32Type = typeof (int);
  private static readonly Type DoubleType = typeof (double);
  private static readonly Type DecimalType = typeof (Decimal);
  private static readonly Type Int64Type = typeof (long);
  private static readonly Type CharType = typeof (char);
  private static readonly Type SingleType = typeof (float);
  private static readonly Type BooleanType = typeof (bool);
  private static readonly Type BigIntegerType = typeof (BigInteger);
  private static readonly Type ComplexType = typeof (Complex);
  private static readonly Type DelegateType = typeof (Delegate);
  private static readonly Type IEnumerableType = typeof (IEnumerable);
  private static readonly Type TypeType = typeof (Type);
  private static readonly Type NullableOfTType = typeof (Nullable<>);
  private static readonly Type IListOfTType = typeof (IList<>);
  private static readonly Type IDictOfTType = typeof (IDictionary<,>);
  private static readonly Type IEnumerableOfTType = typeof (IEnumerable<>);
  private static readonly Type IListOfObjectType = typeof (IList<object>);
  private static readonly Type IEnumerableOfObjectType = typeof (IEnumerable<object>);
  private static readonly Type IDictionaryOfObjectType = typeof (IDictionary<object, object>);

  private static CallSite<Func<CallSite, object, T>> MakeImplicitConvertSite<T>()
  {
    return Converter.MakeConvertSite<T>(ConversionResultKind.ImplicitCast);
  }

  private static CallSite<Func<CallSite, object, T>> MakeExplicitConvertSite<T>()
  {
    return Converter.MakeConvertSite<T>(ConversionResultKind.ExplicitCast);
  }

  private static CallSite<Func<CallSite, object, T>> MakeConvertSite<T>(ConversionResultKind kind)
  {
    return CallSite<Func<CallSite, object, T>>.Create((CallSiteBinder) DefaultContext.DefaultPythonContext.Convert(typeof (T), kind));
  }

  private static CallSite<Func<CallSite, object, object>> MakeExplicitTrySite<T>()
  {
    return Converter.MakeTrySite<T>(ConversionResultKind.ExplicitTry);
  }

  private static CallSite<Func<CallSite, object, object>> MakeTrySite<T>(ConversionResultKind kind)
  {
    return CallSite<Func<CallSite, object, object>>.Create((CallSiteBinder) DefaultContext.DefaultPythonContext.Convert(typeof (T), kind));
  }

  public static int ConvertToInt32(object value)
  {
    return Converter._intSite.Target((CallSite) Converter._intSite, value);
  }

  public static string ConvertToString(object value)
  {
    return Converter._stringSite.Target((CallSite) Converter._stringSite, value);
  }

  public static BigInteger ConvertToBigInteger(object value)
  {
    return Converter._bigIntSite.Target((CallSite) Converter._bigIntSite, value);
  }

  public static double ConvertToDouble(object value)
  {
    return Converter._doubleSite.Target((CallSite) Converter._doubleSite, value);
  }

  public static Complex ConvertToComplex(object value)
  {
    return Converter._complexSite.Target((CallSite) Converter._complexSite, value);
  }

  public static bool ConvertToBoolean(object value)
  {
    return Converter._boolSite.Target((CallSite) Converter._boolSite, value);
  }

  public static long ConvertToInt64(object value)
  {
    return Converter._int64Site.Target((CallSite) Converter._int64Site, value);
  }

  public static byte ConvertToByte(object value)
  {
    return Converter._byteSite.Target((CallSite) Converter._byteSite, value);
  }

  public static sbyte ConvertToSByte(object value)
  {
    return Converter._sbyteSite.Target((CallSite) Converter._sbyteSite, value);
  }

  public static short ConvertToInt16(object value)
  {
    return Converter._int16Site.Target((CallSite) Converter._int16Site, value);
  }

  public static ushort ConvertToUInt16(object value)
  {
    return Converter._uint16Site.Target((CallSite) Converter._uint16Site, value);
  }

  public static uint ConvertToUInt32(object value)
  {
    return Converter._uint32Site.Target((CallSite) Converter._uint32Site, value);
  }

  public static ulong ConvertToUInt64(object value)
  {
    return Converter._uint64Site.Target((CallSite) Converter._uint64Site, value);
  }

  public static float ConvertToSingle(object value)
  {
    return Converter._floatSite.Target((CallSite) Converter._floatSite, value);
  }

  public static Decimal ConvertToDecimal(object value)
  {
    return Converter._decimalSite.Target((CallSite) Converter._decimalSite, value);
  }

  public static char ConvertToChar(object value)
  {
    return Converter._charSite.Target((CallSite) Converter._charSite, value);
  }

  internal static bool TryConvertToByte(object value, out byte result)
  {
    object obj = Converter._tryByteSite.Target((CallSite) Converter._tryByteSite, value);
    if (obj != null)
    {
      result = (byte) obj;
      return true;
    }
    result = (byte) 0;
    return false;
  }

  internal static bool TryConvertToSByte(object value, out sbyte result)
  {
    object obj = Converter._trySByteSite.Target((CallSite) Converter._trySByteSite, value);
    if (obj != null)
    {
      result = (sbyte) obj;
      return true;
    }
    result = (sbyte) 0;
    return false;
  }

  internal static bool TryConvertToInt16(object value, out short result)
  {
    object obj = Converter._tryInt16Site.Target((CallSite) Converter._tryInt16Site, value);
    if (obj != null)
    {
      result = (short) obj;
      return true;
    }
    result = (short) 0;
    return false;
  }

  internal static bool TryConvertToInt32(object value, out int result)
  {
    object obj = Converter._tryInt32Site.Target((CallSite) Converter._tryInt32Site, value);
    if (obj != null)
    {
      result = (int) obj;
      return true;
    }
    result = 0;
    return false;
  }

  internal static bool TryConvertToInt64(object value, out long result)
  {
    object obj = Converter._tryInt64Site.Target((CallSite) Converter._tryInt64Site, value);
    if (obj != null)
    {
      result = (long) obj;
      return true;
    }
    result = 0L;
    return false;
  }

  internal static bool TryConvertToUInt16(object value, out ushort result)
  {
    object obj = Converter._tryUInt16Site.Target((CallSite) Converter._tryUInt16Site, value);
    if (obj != null)
    {
      result = (ushort) obj;
      return true;
    }
    result = (ushort) 0;
    return false;
  }

  internal static bool TryConvertToUInt32(object value, out uint result)
  {
    object obj = Converter._tryUInt32Site.Target((CallSite) Converter._tryUInt32Site, value);
    if (obj != null)
    {
      result = (uint) obj;
      return true;
    }
    result = 0U;
    return false;
  }

  internal static bool TryConvertToUInt64(object value, out ulong result)
  {
    object obj = Converter._tryUInt64Site.Target((CallSite) Converter._tryUInt64Site, value);
    if (obj != null)
    {
      result = (ulong) obj;
      return true;
    }
    result = 0UL;
    return false;
  }

  internal static bool TryConvertToDouble(object value, out double result)
  {
    object obj = Converter._tryDoubleSite.Target((CallSite) Converter._tryDoubleSite, value);
    if (obj != null)
    {
      result = (double) obj;
      return true;
    }
    result = 0.0;
    return false;
  }

  internal static bool TryConvertToBigInteger(object value, out BigInteger result)
  {
    object obj = Converter._tryBigIntegerSite.Target((CallSite) Converter._tryBigIntegerSite, value);
    if (obj != null)
    {
      result = (BigInteger) obj;
      return true;
    }
    result = new BigInteger();
    return false;
  }

  internal static bool TryConvertToComplex(object value, out Complex result)
  {
    object obj = Converter._tryComplexSite.Target((CallSite) Converter._tryComplexSite, value);
    if (obj != null)
    {
      result = (Complex) obj;
      return true;
    }
    result = new Complex();
    return false;
  }

  internal static bool TryConvertToString(object value, out string result)
  {
    object obj = Converter._tryStringSite.Target((CallSite) Converter._tryStringSite, value);
    if (obj != null)
    {
      result = (string) obj;
      return true;
    }
    result = (string) null;
    return false;
  }

  internal static bool TryConvertToChar(object value, out char result)
  {
    object obj = Converter._tryCharSite.Target((CallSite) Converter._tryCharSite, value);
    if (obj != null)
    {
      result = (char) obj;
      return true;
    }
    result = char.MinValue;
    return false;
  }

  internal static char ExplicitConvertToChar(object value)
  {
    return Converter._explicitCharSite.Target((CallSite) Converter._explicitCharSite, value);
  }

  public static T Convert<T>(object value) => (T) Converter.Convert(value, typeof (T));

  internal static bool TryConvert(object value, Type to, out object result)
  {
    try
    {
      result = Converter.Convert(value, to);
      return true;
    }
    catch
    {
      result = (object) null;
      return false;
    }
  }

  internal static object Convert(object value, Type to)
  {
    CallSite<Func<CallSite, object, object>> callSite;
    lock (Converter._siteDict)
    {
      if (!Converter._siteDict.TryGetValue(to, out callSite))
        Converter._siteDict[to] = callSite = CallSite<Func<CallSite, object, object>>.Create((CallSiteBinder) DefaultContext.DefaultPythonContext.ConvertRetObject(to, ConversionResultKind.ExplicitCast));
    }
    object obj = callSite.Target((CallSite) callSite, value);
    return !to.IsValueType() || obj != null || to.IsGenericType() && !(to.GetGenericTypeDefinition() != typeof (Nullable<>)) ? obj : throw Converter.MakeTypeError(to, value);
  }

  internal static bool TryConvertToIEnumerator(object o, out IEnumerator e)
  {
    try
    {
      e = Converter._ienumeratorSite.Target((CallSite) Converter._ienumeratorSite, o);
      return e != null;
    }
    catch
    {
      e = (IEnumerator) null;
      return false;
    }
  }

  internal static IEnumerator ConvertToIEnumerator(object o)
  {
    return Converter._ienumeratorSite.Target((CallSite) Converter._ienumeratorSite, o);
  }

  public static IEnumerable ConvertToIEnumerable(object o)
  {
    return Converter._ienumerableSite.Target((CallSite) Converter._ienumerableSite, o);
  }

  internal static bool TryConvertToIndex(object value, out int index)
  {
    return Converter.TryConvertToIndex(value, true, out index);
  }

  internal static bool TryConvertToIndex(object value, bool throwOverflowError, out int index)
  {
    int? sliceIndexHelper = Converter.ConvertToSliceIndexHelper(value, throwOverflowError);
    object ret;
    if (!sliceIndexHelper.HasValue && PythonOps.TryGetBoundAttr(value, "__index__", out ret))
      sliceIndexHelper = Converter.ConvertToSliceIndexHelper(PythonCalls.Call(ret), throwOverflowError);
    index = sliceIndexHelper.HasValue ? sliceIndexHelper.Value : 0;
    return sliceIndexHelper.HasValue;
  }

  internal static bool TryConvertToIndex(object value, out object index)
  {
    return Converter.TryConvertToIndex(value, true, out index);
  }

  internal static bool TryConvertToIndex(object value, bool throwOverflowError, out object index)
  {
    index = Converter.ConvertToSliceIndexHelper(value);
    object ret;
    if (index == null && PythonOps.TryGetBoundAttr(value, "__index__", out ret))
      index = Converter.ConvertToSliceIndexHelper(PythonCalls.Call(ret));
    return index != null;
  }

  public static int ConvertToIndex(object value)
  {
    int? sliceIndexHelper = Converter.ConvertToSliceIndexHelper(value, false);
    if (sliceIndexHelper.HasValue)
      return sliceIndexHelper.Value;
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "__index__", out ret))
    {
      object o = PythonCalls.Call(ret);
      return (Converter.ConvertToSliceIndexHelper(o, false) ?? throw PythonOps.TypeError("__index__ returned bad value: {0}", (object) DynamicHelpers.GetPythonType(o).Name)).Value;
    }
    throw PythonOps.TypeError("expected index value, got {0}", (object) DynamicHelpers.GetPythonType(value).Name);
  }

  private static int? ConvertToSliceIndexHelper(object value, bool throwOverflowError)
  {
    switch (value)
    {
      case int num:
        return new int?(num);
      case Extensible<int> _:
        return new int?(((Extensible<int>) value).Value);
      case BigInteger self:
label_5:
        int ret;
        if (self.AsInt32(out ret))
          return new int?(ret);
        if (throwOverflowError)
          throw PythonOps.OverflowError("can't fit long into index");
        return new int?(self == BigInteger.Zero ? 0 : (self > 0L ? int.MaxValue : int.MinValue));
      case Extensible<BigInteger> extensible:
        self = extensible.Value;
        goto label_5;
      default:
        return new int?();
    }
  }

  private static object ConvertToSliceIndexHelper(object value)
  {
    switch (value)
    {
      case int _:
        return value;
      case Extensible<int> _:
        return ScriptingRuntimeHelpers.Int32ToObject(((Extensible<int>) value).Value);
      case BigInteger _:
        return value;
      case Extensible<BigInteger> _:
        return (object) ((Extensible<BigInteger>) value).Value;
      default:
        return (object) null;
    }
  }

  internal static Exception CannotConvertOverflow(string name, object value)
  {
    return PythonOps.OverflowError("Cannot convert {0}({1}) to {2}", (object) PythonTypeOps.GetName(value), value, (object) name);
  }

  private static Exception MakeTypeError(Type expectedType, object o)
  {
    return Converter.MakeTypeError(DynamicHelpers.GetPythonTypeFromType(expectedType).Name.ToString(), o);
  }

  private static Exception MakeTypeError(string expectedType, object o)
  {
    return PythonOps.TypeErrorForTypeMismatch(expectedType, o);
  }

  public static object ConvertToReferenceType(object fromObject, RuntimeTypeHandle typeHandle)
  {
    return fromObject == null ? (object) null : Converter.Convert(fromObject, Type.GetTypeFromHandle(typeHandle));
  }

  public static object ConvertToNullableType(object fromObject, RuntimeTypeHandle typeHandle)
  {
    return fromObject == null ? (object) null : Converter.Convert(fromObject, Type.GetTypeFromHandle(typeHandle));
  }

  public static object ConvertToValueType(object fromObject, RuntimeTypeHandle typeHandle)
  {
    return fromObject != null ? Converter.Convert(fromObject, Type.GetTypeFromHandle(typeHandle)) : throw PythonOps.InvalidType(fromObject, typeHandle);
  }

  public static Type ConvertToType(object value)
  {
    if (value == null)
      return (Type) null;
    Type type = value as Type;
    if (type != (Type) null)
      return type;
    switch (value)
    {
      case PythonType pythonType:
        return pythonType.UnderlyingSystemType;
      case TypeGroup typeGroup:
        Type nonGenericType;
        if (typeGroup.TryGetNonGenericType(out nonGenericType))
          return nonGenericType;
        break;
    }
    throw Converter.MakeTypeError("Type", value);
  }

  public static object ConvertToDelegate(object value, Type to)
  {
    return value == null ? (object) null : (object) DefaultContext.DefaultCLS.LanguageContext.DelegateCreator.GetDelegate(value, to);
  }

  public static bool CanConvertFrom(Type fromType, Type toType, NarrowingLevel allowNarrowing)
  {
    ContractUtils.RequiresNotNull((object) fromType, nameof (fromType));
    ContractUtils.RequiresNotNull((object) toType, nameof (toType));
    if (toType == fromType || toType.IsAssignableFrom(fromType) || fromType.IsCOMObject && toType.IsInterface || Converter.HasImplicitNumericConversion(fromType, toType) || toType == Converter.TypeType && (typeof (PythonType).IsAssignableFrom(fromType) || typeof (TypeGroup).IsAssignableFrom(fromType)) || typeof (Extensible<int>).IsAssignableFrom(fromType) && Converter.CanConvertFrom(Converter.Int32Type, toType, allowNarrowing) || typeof (Extensible<BigInteger>).IsAssignableFrom(fromType) && Converter.CanConvertFrom(Converter.BigIntegerType, toType, allowNarrowing) || typeof (ExtensibleString).IsAssignableFrom(fromType) && Converter.CanConvertFrom(Converter.StringType, toType, allowNarrowing) || typeof (Extensible<double>).IsAssignableFrom(fromType) && Converter.CanConvertFrom(Converter.DoubleType, toType, allowNarrowing) || typeof (Extensible<Complex>).IsAssignableFrom(fromType) && Converter.CanConvertFrom(Converter.ComplexType, toType, allowNarrowing))
      return true;
    foreach (TypeConverterAttribute customAttribute in toType.GetCustomAttributes(typeof (TypeConverterAttribute), true))
    {
      TypeConverter typeConverter = Converter.GetTypeConverter(customAttribute);
      if (typeConverter != null && typeConverter.CanConvertFrom(fromType))
        return true;
    }
    return allowNarrowing != NarrowingLevel.None && Converter.HasNarrowingConversion(fromType, toType, allowNarrowing);
  }

  private static TypeConverter GetTypeConverter(TypeConverterAttribute tca)
  {
    try
    {
      ConstructorInfo constructor = Type.GetType(tca.ConverterTypeName).GetConstructor(ReflectionUtils.EmptyTypes);
      if (constructor != (ConstructorInfo) null)
        return constructor.Invoke(ArrayUtils.EmptyObjects) as TypeConverter;
    }
    catch (TargetInvocationException ex)
    {
    }
    return (TypeConverter) null;
  }

  private static bool HasImplicitNumericConversion(Type fromType, Type toType)
  {
    if (fromType.IsEnum())
      return false;
    if (fromType == typeof (BigInteger))
      return toType == typeof (double) || toType == typeof (float) || toType == typeof (Complex);
    if (fromType == typeof (bool))
      return toType == typeof (int) || Converter.HasImplicitNumericConversion(typeof (int), toType);
    switch (fromType.GetTypeCode())
    {
      case TypeCode.Char:
        switch (toType.GetTypeCode())
        {
          case TypeCode.UInt16:
          case TypeCode.Int32:
          case TypeCode.UInt32:
          case TypeCode.Int64:
          case TypeCode.UInt64:
          case TypeCode.Single:
          case TypeCode.Double:
          case TypeCode.Decimal:
            return true;
          default:
            return toType == Converter.BigIntegerType || toType == Converter.ComplexType;
        }
      case TypeCode.SByte:
        switch (toType.GetTypeCode())
        {
          case TypeCode.Int16:
          case TypeCode.Int32:
          case TypeCode.Int64:
          case TypeCode.Single:
          case TypeCode.Double:
          case TypeCode.Decimal:
            return true;
          default:
            return toType == Converter.BigIntegerType || toType == Converter.ComplexType;
        }
      case TypeCode.Byte:
        switch (toType.GetTypeCode())
        {
          case TypeCode.Int16:
          case TypeCode.UInt16:
          case TypeCode.Int32:
          case TypeCode.UInt32:
          case TypeCode.Int64:
          case TypeCode.UInt64:
          case TypeCode.Single:
          case TypeCode.Double:
          case TypeCode.Decimal:
            return true;
          default:
            return toType == Converter.BigIntegerType || toType == Converter.ComplexType;
        }
      case TypeCode.Int16:
        switch (toType.GetTypeCode())
        {
          case TypeCode.Int32:
          case TypeCode.Int64:
          case TypeCode.Single:
          case TypeCode.Double:
          case TypeCode.Decimal:
            return true;
          default:
            return toType == Converter.BigIntegerType || toType == Converter.ComplexType;
        }
      case TypeCode.UInt16:
        switch (toType.GetTypeCode())
        {
          case TypeCode.Int32:
          case TypeCode.UInt32:
          case TypeCode.Int64:
          case TypeCode.UInt64:
          case TypeCode.Single:
          case TypeCode.Double:
          case TypeCode.Decimal:
            return true;
          default:
            return toType == Converter.BigIntegerType || toType == Converter.ComplexType;
        }
      case TypeCode.Int32:
        switch (toType.GetTypeCode())
        {
          case TypeCode.Int64:
          case TypeCode.Single:
          case TypeCode.Double:
          case TypeCode.Decimal:
            return true;
          default:
            return toType == Converter.BigIntegerType || toType == Converter.ComplexType;
        }
      case TypeCode.UInt32:
        switch (toType.GetTypeCode())
        {
          case TypeCode.Int64:
          case TypeCode.UInt64:
          case TypeCode.Single:
          case TypeCode.Double:
          case TypeCode.Decimal:
            return true;
          default:
            return toType == Converter.BigIntegerType || toType == Converter.ComplexType;
        }
      case TypeCode.Int64:
        switch (toType.GetTypeCode())
        {
          case TypeCode.Single:
          case TypeCode.Double:
          case TypeCode.Decimal:
            return true;
          default:
            return toType == Converter.BigIntegerType || toType == Converter.ComplexType;
        }
      case TypeCode.UInt64:
        switch (toType.GetTypeCode())
        {
          case TypeCode.Single:
          case TypeCode.Double:
          case TypeCode.Decimal:
            return true;
          default:
            return toType == Converter.BigIntegerType || toType == Converter.ComplexType;
        }
      case TypeCode.Single:
        return toType.GetTypeCode() == TypeCode.Double || toType == Converter.ComplexType;
      case TypeCode.Double:
        int typeCode = (int) toType.GetTypeCode();
        return toType == Converter.ComplexType;
      default:
        return false;
    }
  }

  public static Candidate PreferConvert(Type t1, Type t2)
  {
    if (t1 == typeof (bool) && t2 == typeof (int) || t1 == typeof (Decimal) && t2 == typeof (BigInteger))
      return Candidate.Two;
    switch (t1.GetTypeCode())
    {
      case TypeCode.SByte:
        switch (t2.GetTypeCode())
        {
          case TypeCode.Byte:
          case TypeCode.UInt16:
          case TypeCode.UInt32:
          case TypeCode.UInt64:
            return Candidate.Two;
          default:
            return Candidate.Equivalent;
        }
      case TypeCode.Int16:
        switch (t2.GetTypeCode())
        {
          case TypeCode.UInt16:
          case TypeCode.UInt32:
          case TypeCode.UInt64:
            return Candidate.Two;
          default:
            return Candidate.Equivalent;
        }
      case TypeCode.Int32:
        switch (t2.GetTypeCode())
        {
          case TypeCode.UInt32:
          case TypeCode.UInt64:
            return Candidate.Two;
          default:
            return Candidate.Equivalent;
        }
      case TypeCode.Int64:
        return t2.GetTypeCode() == TypeCode.UInt64 ? Candidate.Two : Candidate.Equivalent;
      default:
        return Candidate.Equivalent;
    }
  }

  private static bool HasNarrowingConversion(
    Type fromType,
    Type toType,
    NarrowingLevel allowNarrowing)
  {
    if (allowNarrowing == NarrowingLevel.Three && (toType == Converter.CharType && fromType == Converter.StringType || toType == Converter.StringType && fromType == Converter.CharType || Converter.HasImplicitConversion(fromType, toType)) || toType == Converter.DoubleType && fromType == Converter.DecimalType || toType == Converter.SingleType && fromType == Converter.DecimalType)
      return true;
    if (toType.IsArray)
      return typeof (PythonTuple).IsAssignableFrom(fromType);
    if (allowNarrowing == NarrowingLevel.Three)
    {
      if (Converter.IsNumeric(fromType) && Converter.IsNumeric(toType) && fromType != typeof (float) && fromType != typeof (double) && fromType != typeof (Decimal) && fromType != typeof (Complex) || fromType == typeof (bool) && Converter.IsNumeric(toType) || toType == Converter.CharType && fromType == Converter.StringType || toType == Converter.Int32Type && fromType == Converter.BooleanType || toType == Converter.BooleanType || Converter.DelegateType.IsAssignableFrom(toType) && Converter.IsPythonType(fromType) || Converter.IEnumerableType == toType && Converter.IsPythonType(fromType))
        return true;
      if (toType == typeof (IEnumerator))
      {
        if (Converter.IsPythonType(fromType))
          return true;
      }
      else if (toType.IsGenericType())
      {
        Type genericTypeDefinition = toType.GetGenericTypeDefinition();
        if (genericTypeDefinition == Converter.IEnumerableOfTType)
          return Converter.IEnumerableOfObjectType.IsAssignableFrom(fromType) || Converter.IEnumerableType.IsAssignableFrom(fromType) || fromType == typeof (OldInstance);
        if (genericTypeDefinition == typeof (IEnumerator<>) && Converter.IsPythonType(fromType))
          return true;
      }
    }
    if (allowNarrowing == NarrowingLevel.All && (Converter.IsNumeric(fromType) && Converter.IsNumeric(toType) || toType == Converter.Int32Type && Converter.HasPythonProtocol(fromType, "__int__") || toType == Converter.DoubleType && Converter.HasPythonProtocol(fromType, "__float__") || toType == Converter.BigIntegerType && Converter.HasPythonProtocol(fromType, "__long__")))
      return true;
    if (toType.IsGenericType())
    {
      Type genericTypeDefinition = toType.GetGenericTypeDefinition();
      if (genericTypeDefinition == Converter.IListOfTType)
        return Converter.IListOfObjectType.IsAssignableFrom(fromType);
      if (genericTypeDefinition == Converter.NullableOfTType)
      {
        if (fromType == typeof (DynamicNull) || Converter.CanConvertFrom(fromType, toType.GetGenericArguments()[0], allowNarrowing))
          return true;
      }
      else if (genericTypeDefinition == Converter.IDictOfTType)
        return Converter.IDictionaryOfObjectType.IsAssignableFrom(fromType);
    }
    return fromType == Converter.BigIntegerType && toType == Converter.Int64Type || toType.IsEnum() && fromType == Enum.GetUnderlyingType(toType);
  }

  private static bool HasImplicitConversion(Type fromType, Type toType)
  {
    return Converter.HasImplicitConversionWorker(fromType, fromType, toType) || Converter.HasImplicitConversionWorker(toType, fromType, toType);
  }

  private static bool HasImplicitConversionWorker(Type lookupType, Type fromType, Type toType)
  {
    for (; lookupType != (Type) null; lookupType = lookupType.GetBaseType())
    {
      foreach (MethodInfo method in lookupType.GetMethods())
      {
        if (method.Name == "op_Implicit" && method.GetParameters()[0].ParameterType.IsAssignableFrom(fromType) && toType.IsAssignableFrom(method.ReturnType))
          return true;
      }
    }
    return false;
  }

  public static int? ImplicitConvertToInt32(object o)
  {
    int result;
    switch (o)
    {
      case int num1:
        return new int?(num1);
      case BigInteger self:
        int ret;
        if (self.AsInt32(out ret))
          return new int?(ret);
        break;
      case Extensible<int> _:
        return new int?(Converter.ConvertToInt32(o));
      case Extensible<BigInteger> _ when Converter.TryConvertToInt32(o, out result):
        return new int?(result);
    }
    object obj;
    return !(o is double) && !(o is float) && !(o is Extensible<double>) && PythonTypeOps.TryInvokeUnaryOperator(DefaultContext.Default, o, "__int__", out obj) && obj is int num2 ? new int?(num2) : new int?();
  }

  internal static bool IsNumeric(Type t)
  {
    if (t.IsEnum())
      return false;
    switch (t.GetTypeCode())
    {
      case TypeCode.Empty:
      case TypeCode.DBNull:
      case TypeCode.Boolean:
      case TypeCode.Char:
      case TypeCode.DateTime:
      case TypeCode.String:
        return false;
      case TypeCode.Object:
        return t == Converter.BigIntegerType || t == Converter.ComplexType;
      default:
        return true;
    }
  }

  private static bool IsPythonType(Type t) => t.FullName.StartsWith("IronPython.");

  private static bool HasPythonProtocol(Type t, string name)
  {
    if (t.FullName.StartsWith("IronPython.NewTypes.") || t == typeof (OldInstance))
      return true;
    PythonType pythonTypeFromType = DynamicHelpers.GetPythonTypeFromType(t);
    return pythonTypeFromType != null && pythonTypeFromType.TryResolveSlot(DefaultContext.Default, name, out PythonTypeSlot _);
  }
}
