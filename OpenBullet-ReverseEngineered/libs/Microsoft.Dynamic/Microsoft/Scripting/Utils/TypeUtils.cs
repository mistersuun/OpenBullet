// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.TypeUtils
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Utils;

public static class TypeUtils
{
  public static readonly Type ComObjectType = typeof (object).Assembly.GetType("System.__ComObject");

  public static bool IsNested(this Type t) => t.DeclaringType != (Type) null;

  internal static Type GetNonNullableType(this Type type)
  {
    return !type.IsNullableType() ? type : type.GetGenericArguments()[0];
  }

  internal static Type GetNullableType(this Type type)
  {
    if (!type.IsValueType || type.IsNullableType())
      return type;
    return typeof (Nullable<>).MakeGenericType(type);
  }

  internal static bool IsNullableType(this Type type)
  {
    return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
  }

  internal static bool IsBool(this Type type) => type.GetNonNullableType() == typeof (bool);

  internal static bool IsNumeric(this Type type)
  {
    type = type.GetNonNullableType();
    return !type.IsEnum && TypeUtils.IsNumeric(type.GetTypeCode());
  }

  internal static bool IsNumeric(TypeCode typeCode)
  {
    switch (typeCode)
    {
      case TypeCode.Char:
      case TypeCode.SByte:
      case TypeCode.Byte:
      case TypeCode.Int16:
      case TypeCode.UInt16:
      case TypeCode.Int32:
      case TypeCode.UInt32:
      case TypeCode.Int64:
      case TypeCode.UInt64:
      case TypeCode.Single:
      case TypeCode.Double:
        return true;
      default:
        return false;
    }
  }

  internal static bool IsArithmetic(this Type type)
  {
    type = type.GetNonNullableType();
    if (!type.IsEnum)
    {
      switch (type.GetTypeCode())
      {
        case TypeCode.Int16:
        case TypeCode.UInt16:
        case TypeCode.Int32:
        case TypeCode.UInt32:
        case TypeCode.Int64:
        case TypeCode.UInt64:
        case TypeCode.Single:
        case TypeCode.Double:
          return true;
      }
    }
    return false;
  }

  internal static bool IsUnsignedInt(this Type type)
  {
    type = type.GetNonNullableType();
    if (!type.IsEnum)
    {
      switch (type.GetTypeCode())
      {
        case TypeCode.UInt16:
        case TypeCode.UInt32:
        case TypeCode.UInt64:
          return true;
      }
    }
    return false;
  }

  internal static bool IsIntegerOrBool(this Type type)
  {
    type = type.GetNonNullableType();
    if (!type.IsEnum)
    {
      switch (type.GetTypeCode())
      {
        case TypeCode.Boolean:
        case TypeCode.SByte:
        case TypeCode.Byte:
        case TypeCode.Int16:
        case TypeCode.UInt16:
        case TypeCode.Int32:
        case TypeCode.UInt32:
        case TypeCode.Int64:
        case TypeCode.UInt64:
          return true;
      }
    }
    return false;
  }

  internal static bool CanAssign(Type to, Expression from)
  {
    return TypeUtils.CanAssign(to, from.Type) || to.IsValueType() && to.IsGenericType() && to.GetGenericTypeDefinition() == typeof (Nullable<>) && Microsoft.Scripting.Generation.ConstantCheck.Check(from, (object) null);
  }

  internal static bool CanAssign(Type to, Type from)
  {
    return to == from || !to.IsValueType() && !from.IsValueType() && (to.IsAssignableFrom(from) || to.IsArray && from.IsArray && to.GetArrayRank() == from.GetArrayRank() && TypeUtils.CanAssign(to.GetElementType(), from.GetElementType()));
  }

  internal static bool IsGeneric(Type type)
  {
    return type.ContainsGenericParameters() || type.IsGenericTypeDefinition();
  }

  internal static bool CanCompareToNull(Type type) => !type.IsValueType();

  internal static bool GetNumericConversionOrder(TypeCode code, out int x, out int y)
  {
    switch (code)
    {
      case TypeCode.SByte:
        x = 0;
        y = 1;
        break;
      case TypeCode.Byte:
        x = 0;
        y = 0;
        break;
      case TypeCode.Int16:
        x = 1;
        y = 1;
        break;
      case TypeCode.UInt16:
        x = 1;
        y = 0;
        break;
      case TypeCode.Int32:
        x = 2;
        y = 1;
        break;
      case TypeCode.UInt32:
        x = 2;
        y = 0;
        break;
      case TypeCode.Int64:
        x = 3;
        y = 1;
        break;
      case TypeCode.UInt64:
        x = 3;
        y = 0;
        break;
      case TypeCode.Single:
        x = 1;
        y = 2;
        break;
      case TypeCode.Double:
        x = 2;
        y = 2;
        break;
      default:
        x = y = 0;
        return false;
    }
    return true;
  }

  internal static bool IsImplicitlyConvertible(int fromX, int fromY, int toX, int toY)
  {
    return fromX <= toX && fromY <= toY;
  }

  internal static bool HasBuiltinEquality(Type left, Type right)
  {
    return left.IsInterface() && !right.IsValueType() || right.IsInterface() && !left.IsValueType() || !left.IsValueType() && !right.IsValueType() && (TypeUtils.CanAssign(left, right) || TypeUtils.CanAssign(right, left)) || TypeUtils.NullVsNullable(left, right) || TypeUtils.NullVsNullable(right, left) || !(left != right) && (left == typeof (bool) || left.IsNumeric() || left.IsEnum());
  }

  private static bool NullVsNullable(Type left, Type right)
  {
    return left.IsNullableType() && right == typeof (DynamicNull);
  }

  internal static bool AreEquivalent(Type t1, Type t2) => t1 == t2 || t1.IsEquivalentTo(t2);

  internal static bool AreReferenceAssignable(Type dest, Type src)
  {
    return dest == src || !dest.IsValueType() && !src.IsValueType() && TypeUtils.AreAssignable(dest, src);
  }

  internal static bool AreAssignable(Type dest, Type src)
  {
    return dest == src || dest.IsAssignableFrom(src) || dest.IsArray && src.IsArray && dest.GetArrayRank() == src.GetArrayRank() && TypeUtils.AreReferenceAssignable(dest.GetElementType(), src.GetElementType()) || src.IsArray && dest.IsGenericType() && (dest.GetGenericTypeDefinition() == typeof (IEnumerable<>) || dest.GetGenericTypeDefinition() == typeof (IList<>) || dest.GetGenericTypeDefinition() == typeof (ICollection<>)) && dest.GetGenericArguments()[0] == src.GetElementType();
  }

  internal static Type GetConstantType(Type type)
  {
    if (type.IsVisible())
      return type;
    Type type1 = type;
    do
    {
      type1 = type1.GetBaseType();
    }
    while (!type1.IsVisible());
    return type1 == typeof (Type) || type1 == typeof (ConstructorInfo) || type1 == typeof (EventInfo) || type1 == typeof (FieldInfo) || type1 == typeof (MethodInfo) || type1 == typeof (PropertyInfo) ? type1 : type;
  }

  internal static bool IsConvertible(Type type)
  {
    type = type.GetNonNullableType();
    return type.IsEnum() || (uint) (type.GetTypeCode() - 3) <= 11U;
  }

  internal static bool IsFloatingPoint(Type type)
  {
    type = type.GetNonNullableType();
    switch (type.GetTypeCode())
    {
      case TypeCode.Single:
      case TypeCode.Double:
        return true;
      default:
        return false;
    }
  }

  public static bool IsComObjectType(Type type) => TypeUtils.ComObjectType.IsAssignableFrom(type);

  public static bool IsComObject(object obj)
  {
    return obj != null && TypeUtils.IsComObjectType(obj.GetType());
  }
}
