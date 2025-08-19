// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.Cast
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.CodeDom.Compiler;

#nullable disable
namespace Microsoft.Scripting.Runtime;

[GeneratedCode("DLR", "2.0")]
public static class Cast
{
  internal static readonly Type BooleanType = typeof (bool);
  internal static readonly Type ByteType = typeof (byte);
  internal static readonly Type CharType = typeof (char);
  internal static readonly Type DecimalType = typeof (Decimal);
  internal static readonly Type DoubleType = typeof (double);
  internal static readonly Type Int16Type = typeof (short);
  internal static readonly Type Int32Type = typeof (int);
  internal static readonly Type Int64Type = typeof (long);
  internal static readonly Type ObjectType = typeof (object);
  internal static readonly Type SByteType = typeof (sbyte);
  internal static readonly Type SingleType = typeof (float);
  internal static readonly Type UInt16Type = typeof (ushort);
  internal static readonly Type UInt32Type = typeof (uint);
  internal static readonly Type UInt64Type = typeof (ulong);
  internal static readonly Type NullableBooleanType = typeof (bool?);
  internal static readonly Type NullableByteType = typeof (byte?);
  internal static readonly Type NullableCharType = typeof (char?);
  internal static readonly Type NullableDecimalType = typeof (Decimal?);
  internal static readonly Type NullableDoubleType = typeof (double?);
  internal static readonly Type NullableInt16Type = typeof (short?);
  internal static readonly Type NullableInt32Type = typeof (int?);
  internal static readonly Type NullableInt64Type = typeof (long?);
  internal static readonly Type NullableSByteType = typeof (sbyte?);
  internal static readonly Type NullableSingleType = typeof (float?);
  internal static readonly Type NullableUInt16Type = typeof (ushort?);
  internal static readonly Type NullableUInt32Type = typeof (uint?);
  internal static readonly Type NullableUInt64Type = typeof (ulong?);
  internal static readonly Type NullableType = typeof (Nullable<>);

  public static object Explicit(object o, Type to)
  {
    if (o == null)
    {
      if (!to.IsValueType())
        return (object) null;
      if (to.IsGenericType() && to.GetGenericTypeDefinition() == Cast.NullableType)
        return Cast.NewNullableInstance(to.GetGenericArguments()[0]);
      if (to == typeof (void))
        return (object) null;
      throw new InvalidCastException("Cannot cast null to a value type " + to.Name);
    }
    if (to.IsValueType())
      return Cast.ExplicitCastToValueType(o, to);
    Type type = o.GetType();
    if (to.IsAssignableFrom(type))
      return o;
    throw new InvalidCastException($"Cannot cast {type.Name} to {to.Name}");
  }

  public static T Explicit<T>(object o) => (T) Cast.Explicit(o, typeof (T));

  private static object ExplicitCastToValueType(object o, Type to)
  {
    if (to == Cast.Int32Type)
      return ScriptingRuntimeHelpers.Int32ToObject(Cast.ExplicitCastToInt32(o));
    if (to == Cast.DoubleType)
      return (object) Cast.ExplicitCastToDouble(o);
    if (to == Cast.BooleanType)
      return ScriptingRuntimeHelpers.BooleanToObject(Cast.ExplicitCastToBoolean(o));
    if (to == Cast.ByteType)
      return (object) Cast.ExplicitCastToByte(o);
    if (to == Cast.CharType)
      return (object) Cast.ExplicitCastToChar(o);
    if (to == Cast.DecimalType)
      return (object) Cast.ExplicitCastToDecimal(o);
    if (to == Cast.Int16Type)
      return (object) Cast.ExplicitCastToInt16(o);
    if (to == Cast.Int64Type)
      return (object) Cast.ExplicitCastToInt64(o);
    if (to == Cast.SByteType)
      return (object) Cast.ExplicitCastToSByte(o);
    if (to == Cast.SingleType)
      return (object) Cast.ExplicitCastToSingle(o);
    if (to == Cast.UInt16Type)
      return (object) Cast.ExplicitCastToUInt16(o);
    if (to == Cast.UInt32Type)
      return (object) Cast.ExplicitCastToUInt32(o);
    if (to == Cast.UInt64Type)
      return (object) Cast.ExplicitCastToUInt64(o);
    if (to == Cast.NullableBooleanType)
      return (object) Cast.ExplicitCastToNullableBoolean(o);
    if (to == Cast.NullableByteType)
      return (object) Cast.ExplicitCastToNullableByte(o);
    if (to == Cast.NullableCharType)
      return (object) Cast.ExplicitCastToNullableChar(o);
    if (to == Cast.NullableDecimalType)
      return (object) Cast.ExplicitCastToNullableDecimal(o);
    if (to == Cast.NullableDoubleType)
      return (object) Cast.ExplicitCastToNullableDouble(o);
    if (to == Cast.NullableInt16Type)
      return (object) Cast.ExplicitCastToNullableInt16(o);
    if (to == Cast.NullableInt32Type)
      return (object) Cast.ExplicitCastToNullableInt32(o);
    if (to == Cast.NullableInt64Type)
      return (object) Cast.ExplicitCastToNullableInt64(o);
    if (to == Cast.NullableSByteType)
      return (object) Cast.ExplicitCastToNullableSByte(o);
    if (to == Cast.NullableSingleType)
      return (object) Cast.ExplicitCastToNullableSingle(o);
    if (to == Cast.NullableUInt16Type)
      return (object) Cast.ExplicitCastToNullableUInt16(o);
    if (to == Cast.NullableUInt32Type)
      return (object) Cast.ExplicitCastToNullableUInt32(o);
    if (to == Cast.NullableUInt64Type)
      return (object) Cast.ExplicitCastToNullableUInt64(o);
    return to.IsAssignableFrom(o.GetType()) ? o : throw new InvalidCastException();
  }

  private static object NewNullableInstanceSlow(Type type)
  {
    return Activator.CreateInstance(Cast.NullableType.MakeGenericType(type));
  }

  private static InvalidCastException InvalidCast(object o, string typeName)
  {
    return new InvalidCastException($"Cannot cast {o?.GetType().Name ?? "(null)"} to {typeName}");
  }

  public static bool ExplicitCastToBoolean(object o)
  {
    if (o != null)
    {
      Type type = o.GetType();
      if (type == Cast.BooleanType)
        return (bool) o;
      if (type == Cast.NullableBooleanType)
        return ((bool?) o).Value;
    }
    throw Cast.InvalidCast(o, "Boolean");
  }

  public static byte ExplicitCastToByte(object o)
  {
    if (o != null)
    {
      Type type = o.GetType();
      if (type == Cast.Int32Type)
        return (byte) (int) o;
      if (type == Cast.DoubleType)
        return (byte) (double) o;
      if (type == Cast.Int64Type)
        return (byte) (long) o;
      if (type == Cast.Int16Type)
        return (byte) (short) o;
      if (type == Cast.UInt32Type)
        return (byte) (uint) o;
      if (type == Cast.UInt64Type)
        return (byte) (ulong) o;
      if (type == Cast.UInt16Type)
        return (byte) (ushort) o;
      if (type == Cast.SByteType)
        return (byte) (sbyte) o;
      if (type == Cast.ByteType)
        return (byte) o;
      if (type == Cast.SingleType)
        return (byte) (float) o;
      if (type == Cast.CharType)
        return (byte) (char) o;
      if (type == Cast.DecimalType)
        return (byte) (Decimal) o;
      if (type.IsEnum())
        return Cast.ExplicitCastEnumToByte(o);
      if (type == Cast.NullableInt32Type)
        return (byte) ((int?) o).Value;
      if (type == Cast.NullableDoubleType)
        return (byte) ((double?) o).Value;
      if (type == Cast.NullableInt64Type)
        return (byte) ((long?) o).Value;
      if (type == Cast.NullableInt16Type)
        return (byte) ((short?) o).Value;
      if (type == Cast.NullableUInt32Type)
        return (byte) ((uint?) o).Value;
      if (type == Cast.NullableUInt64Type)
        return (byte) ((ulong?) o).Value;
      if (type == Cast.NullableUInt16Type)
        return (byte) ((ushort?) o).Value;
      if (type == Cast.NullableSByteType)
        return (byte) ((sbyte?) o).Value;
      if (type == Cast.NullableByteType)
        return ((byte?) o).Value;
      if (type == Cast.NullableSingleType)
        return (byte) ((float?) o).Value;
      if (type == Cast.NullableCharType)
        return (byte) ((char?) o).Value;
      if (type == Cast.NullableDecimalType)
        return (byte) ((Decimal?) o).Value;
    }
    throw Cast.InvalidCast(o, "Byte");
  }

  public static char ExplicitCastToChar(object o)
  {
    if (o != null)
    {
      Type type = o.GetType();
      if (type == Cast.Int32Type)
        return (char) (int) o;
      if (type == Cast.DoubleType)
        return (char) (double) o;
      if (type == Cast.Int64Type)
        return (char) (long) o;
      if (type == Cast.Int16Type)
        return (char) (short) o;
      if (type == Cast.UInt32Type)
        return (char) (uint) o;
      if (type == Cast.UInt64Type)
        return (char) (ulong) o;
      if (type == Cast.UInt16Type)
        return (char) (ushort) o;
      if (type == Cast.SByteType)
        return (char) (sbyte) o;
      if (type == Cast.ByteType)
        return (char) (byte) o;
      if (type == Cast.SingleType)
        return (char) (float) o;
      if (type == Cast.CharType)
        return (char) o;
      if (type == Cast.DecimalType)
        return (char) (Decimal) o;
      if (type.IsEnum())
        return (char) Cast.ExplicitCastEnumToInt32(o);
      if (type == Cast.NullableInt32Type)
        return (char) ((int?) o).Value;
      if (type == Cast.NullableDoubleType)
        return (char) ((double?) o).Value;
      if (type == Cast.NullableInt64Type)
        return (char) ((long?) o).Value;
      if (type == Cast.NullableInt16Type)
        return (char) ((short?) o).Value;
      if (type == Cast.NullableUInt32Type)
        return (char) ((uint?) o).Value;
      if (type == Cast.NullableUInt64Type)
        return (char) ((ulong?) o).Value;
      if (type == Cast.NullableUInt16Type)
        return (char) ((ushort?) o).Value;
      if (type == Cast.NullableSByteType)
        return (char) ((sbyte?) o).Value;
      if (type == Cast.NullableByteType)
        return (char) ((byte?) o).Value;
      if (type == Cast.NullableSingleType)
        return (char) ((float?) o).Value;
      if (type == Cast.NullableCharType)
        return ((char?) o).Value;
      if (type == Cast.NullableDecimalType)
        return (char) ((Decimal?) o).Value;
    }
    throw Cast.InvalidCast(o, "Char");
  }

  public static Decimal ExplicitCastToDecimal(object o)
  {
    if (o != null)
    {
      Type type = o.GetType();
      if (type == Cast.Int32Type)
        return (Decimal) (int) o;
      if (type == Cast.DoubleType)
        return (Decimal) (double) o;
      if (type == Cast.Int64Type)
        return (Decimal) (long) o;
      if (type == Cast.Int16Type)
        return (Decimal) (short) o;
      if (type == Cast.UInt32Type)
        return (Decimal) (uint) o;
      if (type == Cast.UInt64Type)
        return (Decimal) (ulong) o;
      if (type == Cast.UInt16Type)
        return (Decimal) (ushort) o;
      if (type == Cast.SByteType)
        return (Decimal) (sbyte) o;
      if (type == Cast.ByteType)
        return (Decimal) (byte) o;
      if (type == Cast.SingleType)
        return (Decimal) (float) o;
      if (type == Cast.CharType)
        return (Decimal) (char) o;
      if (type == Cast.DecimalType)
        return (Decimal) o;
      if (type.IsEnum())
        return (Decimal) Cast.ExplicitCastEnumToInt64(o);
      if (type == Cast.NullableInt32Type)
        return (Decimal) ((int?) o).Value;
      if (type == Cast.NullableDoubleType)
        return (Decimal) ((double?) o).Value;
      if (type == Cast.NullableInt64Type)
        return (Decimal) ((long?) o).Value;
      if (type == Cast.NullableInt16Type)
        return (Decimal) ((short?) o).Value;
      if (type == Cast.NullableUInt32Type)
        return (Decimal) ((uint?) o).Value;
      if (type == Cast.NullableUInt64Type)
        return (Decimal) ((ulong?) o).Value;
      if (type == Cast.NullableUInt16Type)
        return (Decimal) ((ushort?) o).Value;
      if (type == Cast.NullableSByteType)
        return (Decimal) ((sbyte?) o).Value;
      if (type == Cast.NullableByteType)
        return (Decimal) ((byte?) o).Value;
      if (type == Cast.NullableSingleType)
        return (Decimal) ((float?) o).Value;
      if (type == Cast.NullableCharType)
        return (Decimal) ((char?) o).Value;
      if (type == Cast.NullableDecimalType)
        return ((Decimal?) o).Value;
    }
    throw Cast.InvalidCast(o, "Decimal");
  }

  public static double ExplicitCastToDouble(object o)
  {
    if (o != null)
    {
      Type type = o.GetType();
      if (type == Cast.Int32Type)
        return (double) (int) o;
      if (type == Cast.DoubleType)
        return (double) o;
      if (type == Cast.Int64Type)
        return (double) (long) o;
      if (type == Cast.Int16Type)
        return (double) (short) o;
      if (type == Cast.UInt32Type)
        return (double) (uint) o;
      if (type == Cast.UInt64Type)
        return (double) (ulong) o;
      if (type == Cast.UInt16Type)
        return (double) (ushort) o;
      if (type == Cast.SByteType)
        return (double) (sbyte) o;
      if (type == Cast.ByteType)
        return (double) (byte) o;
      if (type == Cast.SingleType)
        return (double) (float) o;
      if (type == Cast.CharType)
        return (double) (char) o;
      if (type == Cast.DecimalType)
        return (double) (Decimal) o;
      if (type.IsEnum())
        return (double) Cast.ExplicitCastEnumToInt64(o);
      if (type == Cast.NullableInt32Type)
        return (double) ((int?) o).Value;
      if (type == Cast.NullableDoubleType)
        return ((double?) o).Value;
      if (type == Cast.NullableInt64Type)
        return (double) ((long?) o).Value;
      if (type == Cast.NullableInt16Type)
        return (double) ((short?) o).Value;
      if (type == Cast.NullableUInt32Type)
        return (double) ((uint?) o).Value;
      if (type == Cast.NullableUInt64Type)
        return (double) ((ulong?) o).Value;
      if (type == Cast.NullableUInt16Type)
        return (double) ((ushort?) o).Value;
      if (type == Cast.NullableSByteType)
        return (double) ((sbyte?) o).Value;
      if (type == Cast.NullableByteType)
        return (double) ((byte?) o).Value;
      if (type == Cast.NullableSingleType)
        return (double) ((float?) o).Value;
      if (type == Cast.NullableCharType)
        return (double) ((char?) o).Value;
      if (type == Cast.NullableDecimalType)
        return (double) ((Decimal?) o).Value;
    }
    throw Cast.InvalidCast(o, "Double");
  }

  public static short ExplicitCastToInt16(object o)
  {
    if (o != null)
    {
      Type type = o.GetType();
      if (type == Cast.Int32Type)
        return (short) (int) o;
      if (type == Cast.DoubleType)
        return (short) (double) o;
      if (type == Cast.Int64Type)
        return (short) (long) o;
      if (type == Cast.Int16Type)
        return (short) o;
      if (type == Cast.UInt32Type)
        return (short) (uint) o;
      if (type == Cast.UInt64Type)
        return (short) (ulong) o;
      if (type == Cast.UInt16Type)
        return (short) (ushort) o;
      if (type == Cast.SByteType)
        return (short) (sbyte) o;
      if (type == Cast.ByteType)
        return (short) (byte) o;
      if (type == Cast.SingleType)
        return (short) (float) o;
      if (type == Cast.CharType)
        return (short) (char) o;
      if (type == Cast.DecimalType)
        return (short) (Decimal) o;
      if (type.IsEnum())
        return Cast.ExplicitCastEnumToInt16(o);
      if (type == Cast.NullableInt32Type)
        return (short) ((int?) o).Value;
      if (type == Cast.NullableDoubleType)
        return (short) ((double?) o).Value;
      if (type == Cast.NullableInt64Type)
        return (short) ((long?) o).Value;
      if (type == Cast.NullableInt16Type)
        return ((short?) o).Value;
      if (type == Cast.NullableUInt32Type)
        return (short) ((uint?) o).Value;
      if (type == Cast.NullableUInt64Type)
        return (short) ((ulong?) o).Value;
      if (type == Cast.NullableUInt16Type)
        return (short) ((ushort?) o).Value;
      if (type == Cast.NullableSByteType)
        return (short) ((sbyte?) o).Value;
      if (type == Cast.NullableByteType)
        return (short) ((byte?) o).Value;
      if (type == Cast.NullableSingleType)
        return (short) ((float?) o).Value;
      if (type == Cast.NullableCharType)
        return (short) ((char?) o).Value;
      if (type == Cast.NullableDecimalType)
        return (short) ((Decimal?) o).Value;
    }
    throw Cast.InvalidCast(o, "Int16");
  }

  public static int ExplicitCastToInt32(object o)
  {
    if (o != null)
    {
      Type type = o.GetType();
      if (type == Cast.Int32Type)
        return (int) o;
      if (type == Cast.DoubleType)
        return (int) (double) o;
      if (type == Cast.Int64Type)
        return (int) (long) o;
      if (type == Cast.Int16Type)
        return (int) (short) o;
      if (type == Cast.UInt32Type)
        return (int) (uint) o;
      if (type == Cast.UInt64Type)
        return (int) (ulong) o;
      if (type == Cast.UInt16Type)
        return (int) (ushort) o;
      if (type == Cast.SByteType)
        return (int) (sbyte) o;
      if (type == Cast.ByteType)
        return (int) (byte) o;
      if (type == Cast.SingleType)
        return (int) (float) o;
      if (type == Cast.CharType)
        return (int) (char) o;
      if (type == Cast.DecimalType)
        return (int) (Decimal) o;
      if (type.IsEnum())
        return Cast.ExplicitCastEnumToInt32(o);
      if (type == Cast.NullableInt32Type)
        return ((int?) o).Value;
      if (type == Cast.NullableDoubleType)
        return (int) ((double?) o).Value;
      if (type == Cast.NullableInt64Type)
        return (int) ((long?) o).Value;
      if (type == Cast.NullableInt16Type)
        return (int) ((short?) o).Value;
      if (type == Cast.NullableUInt32Type)
        return (int) ((uint?) o).Value;
      if (type == Cast.NullableUInt64Type)
        return (int) ((ulong?) o).Value;
      if (type == Cast.NullableUInt16Type)
        return (int) ((ushort?) o).Value;
      if (type == Cast.NullableSByteType)
        return (int) ((sbyte?) o).Value;
      if (type == Cast.NullableByteType)
        return (int) ((byte?) o).Value;
      if (type == Cast.NullableSingleType)
        return (int) ((float?) o).Value;
      if (type == Cast.NullableCharType)
        return (int) ((char?) o).Value;
      if (type == Cast.NullableDecimalType)
        return (int) ((Decimal?) o).Value;
    }
    throw Cast.InvalidCast(o, "Int32");
  }

  public static long ExplicitCastToInt64(object o)
  {
    if (o != null)
    {
      Type type = o.GetType();
      if (type == Cast.Int32Type)
        return (long) (int) o;
      if (type == Cast.DoubleType)
        return (long) (double) o;
      if (type == Cast.Int64Type)
        return (long) o;
      if (type == Cast.Int16Type)
        return (long) (short) o;
      if (type == Cast.UInt32Type)
        return (long) (uint) o;
      if (type == Cast.UInt64Type)
        return (long) (ulong) o;
      if (type == Cast.UInt16Type)
        return (long) (ushort) o;
      if (type == Cast.SByteType)
        return (long) (sbyte) o;
      if (type == Cast.ByteType)
        return (long) (byte) o;
      if (type == Cast.SingleType)
        return (long) (float) o;
      if (type == Cast.CharType)
        return (long) (char) o;
      if (type == Cast.DecimalType)
        return (long) (Decimal) o;
      if (type.IsEnum())
        return Cast.ExplicitCastEnumToInt64(o);
      if (type == Cast.NullableInt32Type)
        return (long) ((int?) o).Value;
      if (type == Cast.NullableDoubleType)
        return (long) ((double?) o).Value;
      if (type == Cast.NullableInt64Type)
        return ((long?) o).Value;
      if (type == Cast.NullableInt16Type)
        return (long) ((short?) o).Value;
      if (type == Cast.NullableUInt32Type)
        return (long) ((uint?) o).Value;
      if (type == Cast.NullableUInt64Type)
        return (long) ((ulong?) o).Value;
      if (type == Cast.NullableUInt16Type)
        return (long) ((ushort?) o).Value;
      if (type == Cast.NullableSByteType)
        return (long) ((sbyte?) o).Value;
      if (type == Cast.NullableByteType)
        return (long) ((byte?) o).Value;
      if (type == Cast.NullableSingleType)
        return (long) ((float?) o).Value;
      if (type == Cast.NullableCharType)
        return (long) ((char?) o).Value;
      if (type == Cast.NullableDecimalType)
        return (long) ((Decimal?) o).Value;
    }
    throw Cast.InvalidCast(o, "Int64");
  }

  [CLSCompliant(false)]
  public static sbyte ExplicitCastToSByte(object o)
  {
    if (o != null)
    {
      Type type = o.GetType();
      if (type == Cast.Int32Type)
        return (sbyte) (int) o;
      if (type == Cast.DoubleType)
        return (sbyte) (double) o;
      if (type == Cast.Int64Type)
        return (sbyte) (long) o;
      if (type == Cast.Int16Type)
        return (sbyte) (short) o;
      if (type == Cast.UInt32Type)
        return (sbyte) (uint) o;
      if (type == Cast.UInt64Type)
        return (sbyte) (ulong) o;
      if (type == Cast.UInt16Type)
        return (sbyte) (ushort) o;
      if (type == Cast.SByteType)
        return (sbyte) o;
      if (type == Cast.ByteType)
        return (sbyte) (byte) o;
      if (type == Cast.SingleType)
        return (sbyte) (float) o;
      if (type == Cast.CharType)
        return (sbyte) (char) o;
      if (type == Cast.DecimalType)
        return (sbyte) (Decimal) o;
      if (type.IsEnum())
        return Cast.ExplicitCastEnumToSByte(o);
      if (type == Cast.NullableInt32Type)
        return (sbyte) ((int?) o).Value;
      if (type == Cast.NullableDoubleType)
        return (sbyte) ((double?) o).Value;
      if (type == Cast.NullableInt64Type)
        return (sbyte) ((long?) o).Value;
      if (type == Cast.NullableInt16Type)
        return (sbyte) ((short?) o).Value;
      if (type == Cast.NullableUInt32Type)
        return (sbyte) ((uint?) o).Value;
      if (type == Cast.NullableUInt64Type)
        return (sbyte) ((ulong?) o).Value;
      if (type == Cast.NullableUInt16Type)
        return (sbyte) ((ushort?) o).Value;
      if (type == Cast.NullableSByteType)
        return ((sbyte?) o).Value;
      if (type == Cast.NullableByteType)
        return (sbyte) ((byte?) o).Value;
      if (type == Cast.NullableSingleType)
        return (sbyte) ((float?) o).Value;
      if (type == Cast.NullableCharType)
        return (sbyte) ((char?) o).Value;
      if (type == Cast.NullableDecimalType)
        return (sbyte) ((Decimal?) o).Value;
    }
    throw Cast.InvalidCast(o, "SByte");
  }

  public static float ExplicitCastToSingle(object o)
  {
    if (o != null)
    {
      Type type = o.GetType();
      if (type == Cast.Int32Type)
        return (float) (int) o;
      if (type == Cast.DoubleType)
        return (float) (double) o;
      if (type == Cast.Int64Type)
        return (float) (long) o;
      if (type == Cast.Int16Type)
        return (float) (short) o;
      if (type == Cast.UInt32Type)
        return (float) (uint) o;
      if (type == Cast.UInt64Type)
        return (float) (ulong) o;
      if (type == Cast.UInt16Type)
        return (float) (ushort) o;
      if (type == Cast.SByteType)
        return (float) (sbyte) o;
      if (type == Cast.ByteType)
        return (float) (byte) o;
      if (type == Cast.SingleType)
        return (float) o;
      if (type == Cast.CharType)
        return (float) (char) o;
      if (type == Cast.DecimalType)
        return (float) (Decimal) o;
      if (type.IsEnum())
        return (float) Cast.ExplicitCastEnumToInt64(o);
      if (type == Cast.NullableInt32Type)
        return (float) ((int?) o).Value;
      if (type == Cast.NullableDoubleType)
        return (float) ((double?) o).Value;
      if (type == Cast.NullableInt64Type)
        return (float) ((long?) o).Value;
      if (type == Cast.NullableInt16Type)
        return (float) ((short?) o).Value;
      if (type == Cast.NullableUInt32Type)
        return (float) ((uint?) o).Value;
      if (type == Cast.NullableUInt64Type)
        return (float) ((ulong?) o).Value;
      if (type == Cast.NullableUInt16Type)
        return (float) ((ushort?) o).Value;
      if (type == Cast.NullableSByteType)
        return (float) ((sbyte?) o).Value;
      if (type == Cast.NullableByteType)
        return (float) ((byte?) o).Value;
      if (type == Cast.NullableSingleType)
        return ((float?) o).Value;
      if (type == Cast.NullableCharType)
        return (float) ((char?) o).Value;
      if (type == Cast.NullableDecimalType)
        return (float) ((Decimal?) o).Value;
    }
    throw Cast.InvalidCast(o, "Single");
  }

  [CLSCompliant(false)]
  public static ushort ExplicitCastToUInt16(object o)
  {
    if (o != null)
    {
      Type type = o.GetType();
      if (type == Cast.Int32Type)
        return (ushort) (int) o;
      if (type == Cast.DoubleType)
        return (ushort) (double) o;
      if (type == Cast.Int64Type)
        return (ushort) (long) o;
      if (type == Cast.Int16Type)
        return (ushort) (short) o;
      if (type == Cast.UInt32Type)
        return (ushort) (uint) o;
      if (type == Cast.UInt64Type)
        return (ushort) (ulong) o;
      if (type == Cast.UInt16Type)
        return (ushort) o;
      if (type == Cast.SByteType)
        return (ushort) (sbyte) o;
      if (type == Cast.ByteType)
        return (ushort) (byte) o;
      if (type == Cast.SingleType)
        return (ushort) (float) o;
      if (type == Cast.CharType)
        return (ushort) (char) o;
      if (type == Cast.DecimalType)
        return (ushort) (Decimal) o;
      if (type.IsEnum())
        return Cast.ExplicitCastEnumToUInt16(o);
      if (type == Cast.NullableInt32Type)
        return (ushort) ((int?) o).Value;
      if (type == Cast.NullableDoubleType)
        return (ushort) ((double?) o).Value;
      if (type == Cast.NullableInt64Type)
        return (ushort) ((long?) o).Value;
      if (type == Cast.NullableInt16Type)
        return (ushort) ((short?) o).Value;
      if (type == Cast.NullableUInt32Type)
        return (ushort) ((uint?) o).Value;
      if (type == Cast.NullableUInt64Type)
        return (ushort) ((ulong?) o).Value;
      if (type == Cast.NullableUInt16Type)
        return ((ushort?) o).Value;
      if (type == Cast.NullableSByteType)
        return (ushort) ((sbyte?) o).Value;
      if (type == Cast.NullableByteType)
        return (ushort) ((byte?) o).Value;
      if (type == Cast.NullableSingleType)
        return (ushort) ((float?) o).Value;
      if (type == Cast.NullableCharType)
        return (ushort) ((char?) o).Value;
      if (type == Cast.NullableDecimalType)
        return (ushort) ((Decimal?) o).Value;
    }
    throw Cast.InvalidCast(o, "UInt16");
  }

  [CLSCompliant(false)]
  public static uint ExplicitCastToUInt32(object o)
  {
    if (o != null)
    {
      Type type = o.GetType();
      if (type == Cast.Int32Type)
        return (uint) (int) o;
      if (type == Cast.DoubleType)
        return (uint) (double) o;
      if (type == Cast.Int64Type)
        return (uint) (long) o;
      if (type == Cast.Int16Type)
        return (uint) (short) o;
      if (type == Cast.UInt32Type)
        return (uint) o;
      if (type == Cast.UInt64Type)
        return (uint) (ulong) o;
      if (type == Cast.UInt16Type)
        return (uint) (ushort) o;
      if (type == Cast.SByteType)
        return (uint) (sbyte) o;
      if (type == Cast.ByteType)
        return (uint) (byte) o;
      if (type == Cast.SingleType)
        return (uint) (float) o;
      if (type == Cast.CharType)
        return (uint) (char) o;
      if (type == Cast.DecimalType)
        return (uint) (Decimal) o;
      if (type.IsEnum())
        return Cast.ExplicitCastEnumToUInt32(o);
      if (type == Cast.NullableInt32Type)
        return (uint) ((int?) o).Value;
      if (type == Cast.NullableDoubleType)
        return (uint) ((double?) o).Value;
      if (type == Cast.NullableInt64Type)
        return (uint) ((long?) o).Value;
      if (type == Cast.NullableInt16Type)
        return (uint) ((short?) o).Value;
      if (type == Cast.NullableUInt32Type)
        return ((uint?) o).Value;
      if (type == Cast.NullableUInt64Type)
        return (uint) ((ulong?) o).Value;
      if (type == Cast.NullableUInt16Type)
        return (uint) ((ushort?) o).Value;
      if (type == Cast.NullableSByteType)
        return (uint) ((sbyte?) o).Value;
      if (type == Cast.NullableByteType)
        return (uint) ((byte?) o).Value;
      if (type == Cast.NullableSingleType)
        return (uint) ((float?) o).Value;
      if (type == Cast.NullableCharType)
        return (uint) ((char?) o).Value;
      if (type == Cast.NullableDecimalType)
        return (uint) ((Decimal?) o).Value;
    }
    throw Cast.InvalidCast(o, "UInt32");
  }

  [CLSCompliant(false)]
  public static ulong ExplicitCastToUInt64(object o)
  {
    if (o != null)
    {
      Type type = o.GetType();
      if (type == Cast.Int32Type)
        return (ulong) (int) o;
      if (type == Cast.DoubleType)
        return (ulong) (double) o;
      if (type == Cast.Int64Type)
        return (ulong) (long) o;
      if (type == Cast.Int16Type)
        return (ulong) (short) o;
      if (type == Cast.UInt32Type)
        return (ulong) (uint) o;
      if (type == Cast.UInt64Type)
        return (ulong) o;
      if (type == Cast.UInt16Type)
        return (ulong) (ushort) o;
      if (type == Cast.SByteType)
        return (ulong) (sbyte) o;
      if (type == Cast.ByteType)
        return (ulong) (byte) o;
      if (type == Cast.SingleType)
        return (ulong) (float) o;
      if (type == Cast.CharType)
        return (ulong) (char) o;
      if (type == Cast.DecimalType)
        return (ulong) (Decimal) o;
      if (type.IsEnum())
        return Cast.ExplicitCastEnumToUInt64(o);
      if (type == Cast.NullableInt32Type)
        return (ulong) ((int?) o).Value;
      if (type == Cast.NullableDoubleType)
        return (ulong) ((double?) o).Value;
      if (type == Cast.NullableInt64Type)
        return (ulong) ((long?) o).Value;
      if (type == Cast.NullableInt16Type)
        return (ulong) ((short?) o).Value;
      if (type == Cast.NullableUInt32Type)
        return (ulong) ((uint?) o).Value;
      if (type == Cast.NullableUInt64Type)
        return ((ulong?) o).Value;
      if (type == Cast.NullableUInt16Type)
        return (ulong) ((ushort?) o).Value;
      if (type == Cast.NullableSByteType)
        return (ulong) ((sbyte?) o).Value;
      if (type == Cast.NullableByteType)
        return (ulong) ((byte?) o).Value;
      if (type == Cast.NullableSingleType)
        return (ulong) ((float?) o).Value;
      if (type == Cast.NullableCharType)
        return (ulong) ((char?) o).Value;
      if (type == Cast.NullableDecimalType)
        return (ulong) ((Decimal?) o).Value;
    }
    throw Cast.InvalidCast(o, "UInt64");
  }

  public static bool? ExplicitCastToNullableBoolean(object o)
  {
    if (o == null)
      return new bool?();
    Type type = o.GetType();
    if (type == Cast.BooleanType)
      return new bool?((bool) o);
    if (type == Cast.NullableBooleanType)
      return (bool?) o;
    throw Cast.InvalidCast(o, "Boolean");
  }

  public static byte? ExplicitCastToNullableByte(object o)
  {
    if (o == null)
      return new byte?();
    Type type = o.GetType();
    if (type == Cast.Int32Type)
      return new byte?((byte) (int) o);
    if (type == Cast.DoubleType)
      return new byte?((byte) (double) o);
    if (type == Cast.Int64Type)
      return new byte?((byte) (long) o);
    if (type == Cast.Int16Type)
      return new byte?((byte) (short) o);
    if (type == Cast.UInt32Type)
      return new byte?((byte) (uint) o);
    if (type == Cast.UInt64Type)
      return new byte?((byte) (ulong) o);
    if (type == Cast.UInt16Type)
      return new byte?((byte) (ushort) o);
    if (type == Cast.SByteType)
      return new byte?((byte) (sbyte) o);
    if (type == Cast.ByteType)
      return new byte?((byte) o);
    if (type == Cast.SingleType)
      return new byte?((byte) (float) o);
    if (type == Cast.CharType)
      return new byte?((byte) (char) o);
    if (type == Cast.DecimalType)
      return new byte?((byte) (Decimal) o);
    if (type.IsEnum())
      return new byte?(Cast.ExplicitCastEnumToByte(o));
    if (type == Cast.NullableInt32Type)
    {
      int? nullable = (int?) o;
      return !nullable.HasValue ? new byte?() : new byte?((byte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableDoubleType)
    {
      double? nullable = (double?) o;
      return !nullable.HasValue ? new byte?() : new byte?((byte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt64Type)
    {
      long? nullable = (long?) o;
      return !nullable.HasValue ? new byte?() : new byte?((byte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt16Type)
    {
      short? nullable = (short?) o;
      return !nullable.HasValue ? new byte?() : new byte?((byte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt32Type)
    {
      uint? nullable = (uint?) o;
      return !nullable.HasValue ? new byte?() : new byte?((byte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt64Type)
    {
      ulong? nullable = (ulong?) o;
      return !nullable.HasValue ? new byte?() : new byte?((byte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt16Type)
    {
      ushort? nullable = (ushort?) o;
      return !nullable.HasValue ? new byte?() : new byte?((byte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSByteType)
    {
      sbyte? nullable = (sbyte?) o;
      return !nullable.HasValue ? new byte?() : new byte?((byte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableByteType)
      return (byte?) o;
    if (type == Cast.NullableSingleType)
    {
      float? nullable = (float?) o;
      return !nullable.HasValue ? new byte?() : new byte?((byte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableCharType)
    {
      char? nullable = (char?) o;
      return !nullable.HasValue ? new byte?() : new byte?((byte) nullable.GetValueOrDefault());
    }
    if (!(type == Cast.NullableDecimalType))
      throw Cast.InvalidCast(o, "Byte");
    Decimal? nullable1 = (Decimal?) o;
    return !nullable1.HasValue ? new byte?() : new byte?((byte) nullable1.GetValueOrDefault());
  }

  public static char? ExplicitCastToNullableChar(object o)
  {
    if (o == null)
      return new char?();
    Type type = o.GetType();
    if (type == Cast.Int32Type)
      return new char?((char) (int) o);
    if (type == Cast.DoubleType)
      return new char?((char) (double) o);
    if (type == Cast.Int64Type)
      return new char?((char) (long) o);
    if (type == Cast.Int16Type)
      return new char?((char) (short) o);
    if (type == Cast.UInt32Type)
      return new char?((char) (uint) o);
    if (type == Cast.UInt64Type)
      return new char?((char) (ulong) o);
    if (type == Cast.UInt16Type)
      return new char?((char) (ushort) o);
    if (type == Cast.SByteType)
      return new char?((char) (sbyte) o);
    if (type == Cast.ByteType)
      return new char?((char) (byte) o);
    if (type == Cast.SingleType)
      return new char?((char) (float) o);
    if (type == Cast.CharType)
      return new char?((char) o);
    if (type == Cast.DecimalType)
      return new char?((char) (Decimal) o);
    if (type.IsEnum())
      return new char?((char) Cast.ExplicitCastEnumToInt32(o));
    if (type == Cast.NullableInt32Type)
    {
      int? nullable = (int?) o;
      return !nullable.HasValue ? new char?() : new char?((char) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableDoubleType)
    {
      double? nullable = (double?) o;
      return !nullable.HasValue ? new char?() : new char?((char) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt64Type)
    {
      long? nullable = (long?) o;
      return !nullable.HasValue ? new char?() : new char?((char) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt16Type)
    {
      short? nullable = (short?) o;
      return !nullable.HasValue ? new char?() : new char?((char) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt32Type)
    {
      uint? nullable = (uint?) o;
      return !nullable.HasValue ? new char?() : new char?((char) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt64Type)
    {
      ulong? nullable = (ulong?) o;
      return !nullable.HasValue ? new char?() : new char?((char) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt16Type)
    {
      ushort? nullable = (ushort?) o;
      return !nullable.HasValue ? new char?() : new char?((char) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSByteType)
    {
      sbyte? nullable = (sbyte?) o;
      return !nullable.HasValue ? new char?() : new char?((char) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableByteType)
    {
      byte? nullable = (byte?) o;
      return !nullable.HasValue ? new char?() : new char?((char) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSingleType)
    {
      float? nullable = (float?) o;
      return !nullable.HasValue ? new char?() : new char?((char) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableCharType)
      return (char?) o;
    if (!(type == Cast.NullableDecimalType))
      throw Cast.InvalidCast(o, "Char");
    Decimal? nullable1 = (Decimal?) o;
    return !nullable1.HasValue ? new char?() : new char?((char) nullable1.GetValueOrDefault());
  }

  public static Decimal? ExplicitCastToNullableDecimal(object o)
  {
    if (o == null)
      return new Decimal?();
    Type type = o.GetType();
    if (type == Cast.Int32Type)
      return new Decimal?((Decimal) (int) o);
    if (type == Cast.DoubleType)
      return new Decimal?((Decimal) (double) o);
    if (type == Cast.Int64Type)
      return new Decimal?((Decimal) (long) o);
    if (type == Cast.Int16Type)
      return new Decimal?((Decimal) (short) o);
    if (type == Cast.UInt32Type)
      return new Decimal?((Decimal) (uint) o);
    if (type == Cast.UInt64Type)
      return new Decimal?((Decimal) (ulong) o);
    if (type == Cast.UInt16Type)
      return new Decimal?((Decimal) (ushort) o);
    if (type == Cast.SByteType)
      return new Decimal?((Decimal) (sbyte) o);
    if (type == Cast.ByteType)
      return new Decimal?((Decimal) (byte) o);
    if (type == Cast.SingleType)
      return new Decimal?((Decimal) (float) o);
    if (type == Cast.CharType)
      return new Decimal?((Decimal) (char) o);
    if (type == Cast.DecimalType)
      return new Decimal?((Decimal) o);
    if (type.IsEnum())
      return new Decimal?((Decimal) Cast.ExplicitCastEnumToInt64(o));
    if (type == Cast.NullableInt32Type)
    {
      int? nullable = (int?) o;
      return !nullable.HasValue ? new Decimal?() : new Decimal?((Decimal) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableDoubleType)
    {
      double? nullable = (double?) o;
      return !nullable.HasValue ? new Decimal?() : new Decimal?((Decimal) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt64Type)
    {
      long? nullable = (long?) o;
      return !nullable.HasValue ? new Decimal?() : new Decimal?((Decimal) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt16Type)
    {
      short? nullable = (short?) o;
      return !nullable.HasValue ? new Decimal?() : new Decimal?((Decimal) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt32Type)
    {
      uint? nullable = (uint?) o;
      return !nullable.HasValue ? new Decimal?() : new Decimal?((Decimal) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt64Type)
    {
      ulong? nullable = (ulong?) o;
      return !nullable.HasValue ? new Decimal?() : new Decimal?((Decimal) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt16Type)
    {
      ushort? nullable = (ushort?) o;
      return !nullable.HasValue ? new Decimal?() : new Decimal?((Decimal) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSByteType)
    {
      sbyte? nullable = (sbyte?) o;
      return !nullable.HasValue ? new Decimal?() : new Decimal?((Decimal) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableByteType)
    {
      byte? nullable = (byte?) o;
      return !nullable.HasValue ? new Decimal?() : new Decimal?((Decimal) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSingleType)
    {
      float? nullable = (float?) o;
      return !nullable.HasValue ? new Decimal?() : new Decimal?((Decimal) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableCharType)
    {
      char? nullable = (char?) o;
      return !nullable.HasValue ? new Decimal?() : new Decimal?((Decimal) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableDecimalType)
      return (Decimal?) o;
    throw Cast.InvalidCast(o, "Decimal");
  }

  public static double? ExplicitCastToNullableDouble(object o)
  {
    if (o == null)
      return new double?();
    Type type = o.GetType();
    if (type == Cast.Int32Type)
      return new double?((double) (int) o);
    if (type == Cast.DoubleType)
      return new double?((double) o);
    if (type == Cast.Int64Type)
      return new double?((double) (long) o);
    if (type == Cast.Int16Type)
      return new double?((double) (short) o);
    if (type == Cast.UInt32Type)
      return new double?((double) (uint) o);
    if (type == Cast.UInt64Type)
      return new double?((double) (ulong) o);
    if (type == Cast.UInt16Type)
      return new double?((double) (ushort) o);
    if (type == Cast.SByteType)
      return new double?((double) (sbyte) o);
    if (type == Cast.ByteType)
      return new double?((double) (byte) o);
    if (type == Cast.SingleType)
      return new double?((double) (float) o);
    if (type == Cast.CharType)
      return new double?((double) (char) o);
    if (type == Cast.DecimalType)
      return new double?((double) (Decimal) o);
    if (type.IsEnum())
      return new double?((double) Cast.ExplicitCastEnumToInt64(o));
    if (type == Cast.NullableInt32Type)
    {
      int? nullable = (int?) o;
      return !nullable.HasValue ? new double?() : new double?((double) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableDoubleType)
      return (double?) o;
    if (type == Cast.NullableInt64Type)
    {
      long? nullable = (long?) o;
      return !nullable.HasValue ? new double?() : new double?((double) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt16Type)
    {
      short? nullable = (short?) o;
      return !nullable.HasValue ? new double?() : new double?((double) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt32Type)
    {
      uint? nullable = (uint?) o;
      return !nullable.HasValue ? new double?() : new double?((double) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt64Type)
    {
      ulong? nullable = (ulong?) o;
      return !nullable.HasValue ? new double?() : new double?((double) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt16Type)
    {
      ushort? nullable = (ushort?) o;
      return !nullable.HasValue ? new double?() : new double?((double) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSByteType)
    {
      sbyte? nullable = (sbyte?) o;
      return !nullable.HasValue ? new double?() : new double?((double) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableByteType)
    {
      byte? nullable = (byte?) o;
      return !nullable.HasValue ? new double?() : new double?((double) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSingleType)
    {
      float? nullable = (float?) o;
      return !nullable.HasValue ? new double?() : new double?((double) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableCharType)
    {
      char? nullable = (char?) o;
      return !nullable.HasValue ? new double?() : new double?((double) nullable.GetValueOrDefault());
    }
    if (!(type == Cast.NullableDecimalType))
      throw Cast.InvalidCast(o, "Double");
    Decimal? nullable1 = (Decimal?) o;
    return !nullable1.HasValue ? new double?() : new double?((double) nullable1.GetValueOrDefault());
  }

  public static short? ExplicitCastToNullableInt16(object o)
  {
    if (o == null)
      return new short?();
    Type type = o.GetType();
    if (type == Cast.Int32Type)
      return new short?((short) (int) o);
    if (type == Cast.DoubleType)
      return new short?((short) (double) o);
    if (type == Cast.Int64Type)
      return new short?((short) (long) o);
    if (type == Cast.Int16Type)
      return new short?((short) o);
    if (type == Cast.UInt32Type)
      return new short?((short) (uint) o);
    if (type == Cast.UInt64Type)
      return new short?((short) (ulong) o);
    if (type == Cast.UInt16Type)
      return new short?((short) (ushort) o);
    if (type == Cast.SByteType)
      return new short?((short) (sbyte) o);
    if (type == Cast.ByteType)
      return new short?((short) (byte) o);
    if (type == Cast.SingleType)
      return new short?((short) (float) o);
    if (type == Cast.CharType)
      return new short?((short) (char) o);
    if (type == Cast.DecimalType)
      return new short?((short) (Decimal) o);
    if (type.IsEnum())
      return new short?(Cast.ExplicitCastEnumToInt16(o));
    if (type == Cast.NullableInt32Type)
    {
      int? nullable = (int?) o;
      return !nullable.HasValue ? new short?() : new short?((short) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableDoubleType)
    {
      double? nullable = (double?) o;
      return !nullable.HasValue ? new short?() : new short?((short) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt64Type)
    {
      long? nullable = (long?) o;
      return !nullable.HasValue ? new short?() : new short?((short) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt16Type)
      return (short?) o;
    if (type == Cast.NullableUInt32Type)
    {
      uint? nullable = (uint?) o;
      return !nullable.HasValue ? new short?() : new short?((short) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt64Type)
    {
      ulong? nullable = (ulong?) o;
      return !nullable.HasValue ? new short?() : new short?((short) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt16Type)
    {
      ushort? nullable = (ushort?) o;
      return !nullable.HasValue ? new short?() : new short?((short) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSByteType)
    {
      sbyte? nullable = (sbyte?) o;
      return !nullable.HasValue ? new short?() : new short?((short) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableByteType)
    {
      byte? nullable = (byte?) o;
      return !nullable.HasValue ? new short?() : new short?((short) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSingleType)
    {
      float? nullable = (float?) o;
      return !nullable.HasValue ? new short?() : new short?((short) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableCharType)
    {
      char? nullable = (char?) o;
      return !nullable.HasValue ? new short?() : new short?((short) nullable.GetValueOrDefault());
    }
    if (!(type == Cast.NullableDecimalType))
      throw Cast.InvalidCast(o, "Int16");
    Decimal? nullable1 = (Decimal?) o;
    return !nullable1.HasValue ? new short?() : new short?((short) nullable1.GetValueOrDefault());
  }

  public static int? ExplicitCastToNullableInt32(object o)
  {
    if (o == null)
      return new int?();
    Type type = o.GetType();
    if (type == Cast.Int32Type)
      return new int?((int) o);
    if (type == Cast.DoubleType)
      return new int?((int) (double) o);
    if (type == Cast.Int64Type)
      return new int?((int) (long) o);
    if (type == Cast.Int16Type)
      return new int?((int) (short) o);
    if (type == Cast.UInt32Type)
      return new int?((int) (uint) o);
    if (type == Cast.UInt64Type)
      return new int?((int) (ulong) o);
    if (type == Cast.UInt16Type)
      return new int?((int) (ushort) o);
    if (type == Cast.SByteType)
      return new int?((int) (sbyte) o);
    if (type == Cast.ByteType)
      return new int?((int) (byte) o);
    if (type == Cast.SingleType)
      return new int?((int) (float) o);
    if (type == Cast.CharType)
      return new int?((int) (char) o);
    if (type == Cast.DecimalType)
      return new int?((int) (Decimal) o);
    if (type.IsEnum())
      return new int?(Cast.ExplicitCastEnumToInt32(o));
    if (type == Cast.NullableInt32Type)
      return (int?) o;
    if (type == Cast.NullableDoubleType)
    {
      double? nullable = (double?) o;
      return !nullable.HasValue ? new int?() : new int?((int) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt64Type)
    {
      long? nullable = (long?) o;
      return !nullable.HasValue ? new int?() : new int?((int) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt16Type)
    {
      short? nullable = (short?) o;
      return !nullable.HasValue ? new int?() : new int?((int) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt32Type)
    {
      uint? nullable = (uint?) o;
      return !nullable.HasValue ? new int?() : new int?((int) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt64Type)
    {
      ulong? nullable = (ulong?) o;
      return !nullable.HasValue ? new int?() : new int?((int) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt16Type)
    {
      ushort? nullable = (ushort?) o;
      return !nullable.HasValue ? new int?() : new int?((int) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSByteType)
    {
      sbyte? nullable = (sbyte?) o;
      return !nullable.HasValue ? new int?() : new int?((int) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableByteType)
    {
      byte? nullable = (byte?) o;
      return !nullable.HasValue ? new int?() : new int?((int) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSingleType)
    {
      float? nullable = (float?) o;
      return !nullable.HasValue ? new int?() : new int?((int) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableCharType)
    {
      char? nullable = (char?) o;
      return !nullable.HasValue ? new int?() : new int?((int) nullable.GetValueOrDefault());
    }
    if (!(type == Cast.NullableDecimalType))
      throw Cast.InvalidCast(o, "Int32");
    Decimal? nullable1 = (Decimal?) o;
    return !nullable1.HasValue ? new int?() : new int?((int) nullable1.GetValueOrDefault());
  }

  public static long? ExplicitCastToNullableInt64(object o)
  {
    if (o == null)
      return new long?();
    Type type = o.GetType();
    if (type == Cast.Int32Type)
      return new long?((long) (int) o);
    if (type == Cast.DoubleType)
      return new long?((long) (double) o);
    if (type == Cast.Int64Type)
      return new long?((long) o);
    if (type == Cast.Int16Type)
      return new long?((long) (short) o);
    if (type == Cast.UInt32Type)
      return new long?((long) (uint) o);
    if (type == Cast.UInt64Type)
      return new long?((long) (ulong) o);
    if (type == Cast.UInt16Type)
      return new long?((long) (ushort) o);
    if (type == Cast.SByteType)
      return new long?((long) (sbyte) o);
    if (type == Cast.ByteType)
      return new long?((long) (byte) o);
    if (type == Cast.SingleType)
      return new long?((long) (float) o);
    if (type == Cast.CharType)
      return new long?((long) (char) o);
    if (type == Cast.DecimalType)
      return new long?((long) (Decimal) o);
    if (type.IsEnum())
      return new long?(Cast.ExplicitCastEnumToInt64(o));
    if (type == Cast.NullableInt32Type)
    {
      int? nullable = (int?) o;
      return !nullable.HasValue ? new long?() : new long?((long) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableDoubleType)
    {
      double? nullable = (double?) o;
      return !nullable.HasValue ? new long?() : new long?((long) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt64Type)
      return (long?) o;
    if (type == Cast.NullableInt16Type)
    {
      short? nullable = (short?) o;
      return !nullable.HasValue ? new long?() : new long?((long) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt32Type)
    {
      uint? nullable = (uint?) o;
      return !nullable.HasValue ? new long?() : new long?((long) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt64Type)
    {
      ulong? nullable = (ulong?) o;
      return !nullable.HasValue ? new long?() : new long?((long) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt16Type)
    {
      ushort? nullable = (ushort?) o;
      return !nullable.HasValue ? new long?() : new long?((long) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSByteType)
    {
      sbyte? nullable = (sbyte?) o;
      return !nullable.HasValue ? new long?() : new long?((long) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableByteType)
    {
      byte? nullable = (byte?) o;
      return !nullable.HasValue ? new long?() : new long?((long) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSingleType)
    {
      float? nullable = (float?) o;
      return !nullable.HasValue ? new long?() : new long?((long) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableCharType)
    {
      char? nullable = (char?) o;
      return !nullable.HasValue ? new long?() : new long?((long) nullable.GetValueOrDefault());
    }
    if (!(type == Cast.NullableDecimalType))
      throw Cast.InvalidCast(o, "Int64");
    Decimal? nullable1 = (Decimal?) o;
    return !nullable1.HasValue ? new long?() : new long?((long) nullable1.GetValueOrDefault());
  }

  [CLSCompliant(false)]
  public static sbyte? ExplicitCastToNullableSByte(object o)
  {
    if (o == null)
      return new sbyte?();
    Type type = o.GetType();
    if (type == Cast.Int32Type)
      return new sbyte?((sbyte) (int) o);
    if (type == Cast.DoubleType)
      return new sbyte?((sbyte) (double) o);
    if (type == Cast.Int64Type)
      return new sbyte?((sbyte) (long) o);
    if (type == Cast.Int16Type)
      return new sbyte?((sbyte) (short) o);
    if (type == Cast.UInt32Type)
      return new sbyte?((sbyte) (uint) o);
    if (type == Cast.UInt64Type)
      return new sbyte?((sbyte) (ulong) o);
    if (type == Cast.UInt16Type)
      return new sbyte?((sbyte) (ushort) o);
    if (type == Cast.SByteType)
      return new sbyte?((sbyte) o);
    if (type == Cast.ByteType)
      return new sbyte?((sbyte) (byte) o);
    if (type == Cast.SingleType)
      return new sbyte?((sbyte) (float) o);
    if (type == Cast.CharType)
      return new sbyte?((sbyte) (char) o);
    if (type == Cast.DecimalType)
      return new sbyte?((sbyte) (Decimal) o);
    if (type.IsEnum())
      return new sbyte?(Cast.ExplicitCastEnumToSByte(o));
    if (type == Cast.NullableInt32Type)
    {
      int? nullable = (int?) o;
      return !nullable.HasValue ? new sbyte?() : new sbyte?((sbyte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableDoubleType)
    {
      double? nullable = (double?) o;
      return !nullable.HasValue ? new sbyte?() : new sbyte?((sbyte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt64Type)
    {
      long? nullable = (long?) o;
      return !nullable.HasValue ? new sbyte?() : new sbyte?((sbyte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt16Type)
    {
      short? nullable = (short?) o;
      return !nullable.HasValue ? new sbyte?() : new sbyte?((sbyte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt32Type)
    {
      uint? nullable = (uint?) o;
      return !nullable.HasValue ? new sbyte?() : new sbyte?((sbyte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt64Type)
    {
      ulong? nullable = (ulong?) o;
      return !nullable.HasValue ? new sbyte?() : new sbyte?((sbyte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt16Type)
    {
      ushort? nullable = (ushort?) o;
      return !nullable.HasValue ? new sbyte?() : new sbyte?((sbyte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSByteType)
      return (sbyte?) o;
    if (type == Cast.NullableByteType)
    {
      byte? nullable = (byte?) o;
      return !nullable.HasValue ? new sbyte?() : new sbyte?((sbyte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSingleType)
    {
      float? nullable = (float?) o;
      return !nullable.HasValue ? new sbyte?() : new sbyte?((sbyte) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableCharType)
    {
      char? nullable = (char?) o;
      return !nullable.HasValue ? new sbyte?() : new sbyte?((sbyte) nullable.GetValueOrDefault());
    }
    if (!(type == Cast.NullableDecimalType))
      throw Cast.InvalidCast(o, "SByte");
    Decimal? nullable1 = (Decimal?) o;
    return !nullable1.HasValue ? new sbyte?() : new sbyte?((sbyte) nullable1.GetValueOrDefault());
  }

  public static float? ExplicitCastToNullableSingle(object o)
  {
    if (o == null)
      return new float?();
    Type type = o.GetType();
    if (type == Cast.Int32Type)
      return new float?((float) (int) o);
    if (type == Cast.DoubleType)
      return new float?((float) (double) o);
    if (type == Cast.Int64Type)
      return new float?((float) (long) o);
    if (type == Cast.Int16Type)
      return new float?((float) (short) o);
    if (type == Cast.UInt32Type)
      return new float?((float) (uint) o);
    if (type == Cast.UInt64Type)
      return new float?((float) (ulong) o);
    if (type == Cast.UInt16Type)
      return new float?((float) (ushort) o);
    if (type == Cast.SByteType)
      return new float?((float) (sbyte) o);
    if (type == Cast.ByteType)
      return new float?((float) (byte) o);
    if (type == Cast.SingleType)
      return new float?((float) o);
    if (type == Cast.CharType)
      return new float?((float) (char) o);
    if (type == Cast.DecimalType)
      return new float?((float) (Decimal) o);
    if (type.IsEnum())
      return new float?((float) Cast.ExplicitCastEnumToInt64(o));
    if (type == Cast.NullableInt32Type)
    {
      int? nullable = (int?) o;
      return !nullable.HasValue ? new float?() : new float?((float) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableDoubleType)
    {
      double? nullable = (double?) o;
      return !nullable.HasValue ? new float?() : new float?((float) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt64Type)
    {
      long? nullable = (long?) o;
      return !nullable.HasValue ? new float?() : new float?((float) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt16Type)
    {
      short? nullable = (short?) o;
      return !nullable.HasValue ? new float?() : new float?((float) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt32Type)
    {
      uint? nullable = (uint?) o;
      return !nullable.HasValue ? new float?() : new float?((float) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt64Type)
    {
      ulong? nullable = (ulong?) o;
      return !nullable.HasValue ? new float?() : new float?((float) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt16Type)
    {
      ushort? nullable = (ushort?) o;
      return !nullable.HasValue ? new float?() : new float?((float) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSByteType)
    {
      sbyte? nullable = (sbyte?) o;
      return !nullable.HasValue ? new float?() : new float?((float) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableByteType)
    {
      byte? nullable = (byte?) o;
      return !nullable.HasValue ? new float?() : new float?((float) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSingleType)
      return (float?) o;
    if (type == Cast.NullableCharType)
    {
      char? nullable = (char?) o;
      return !nullable.HasValue ? new float?() : new float?((float) nullable.GetValueOrDefault());
    }
    if (!(type == Cast.NullableDecimalType))
      throw Cast.InvalidCast(o, "Single");
    Decimal? nullable1 = (Decimal?) o;
    return !nullable1.HasValue ? new float?() : new float?((float) nullable1.GetValueOrDefault());
  }

  [CLSCompliant(false)]
  public static ushort? ExplicitCastToNullableUInt16(object o)
  {
    if (o == null)
      return new ushort?();
    Type type = o.GetType();
    if (type == Cast.Int32Type)
      return new ushort?((ushort) (int) o);
    if (type == Cast.DoubleType)
      return new ushort?((ushort) (double) o);
    if (type == Cast.Int64Type)
      return new ushort?((ushort) (long) o);
    if (type == Cast.Int16Type)
      return new ushort?((ushort) (short) o);
    if (type == Cast.UInt32Type)
      return new ushort?((ushort) (uint) o);
    if (type == Cast.UInt64Type)
      return new ushort?((ushort) (ulong) o);
    if (type == Cast.UInt16Type)
      return new ushort?((ushort) o);
    if (type == Cast.SByteType)
      return new ushort?((ushort) (sbyte) o);
    if (type == Cast.ByteType)
      return new ushort?((ushort) (byte) o);
    if (type == Cast.SingleType)
      return new ushort?((ushort) (float) o);
    if (type == Cast.CharType)
      return new ushort?((ushort) (char) o);
    if (type == Cast.DecimalType)
      return new ushort?((ushort) (Decimal) o);
    if (type.IsEnum())
      return new ushort?(Cast.ExplicitCastEnumToUInt16(o));
    if (type == Cast.NullableInt32Type)
    {
      int? nullable = (int?) o;
      return !nullable.HasValue ? new ushort?() : new ushort?((ushort) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableDoubleType)
    {
      double? nullable = (double?) o;
      return !nullable.HasValue ? new ushort?() : new ushort?((ushort) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt64Type)
    {
      long? nullable = (long?) o;
      return !nullable.HasValue ? new ushort?() : new ushort?((ushort) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt16Type)
    {
      short? nullable = (short?) o;
      return !nullable.HasValue ? new ushort?() : new ushort?((ushort) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt32Type)
    {
      uint? nullable = (uint?) o;
      return !nullable.HasValue ? new ushort?() : new ushort?((ushort) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt64Type)
    {
      ulong? nullable = (ulong?) o;
      return !nullable.HasValue ? new ushort?() : new ushort?((ushort) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt16Type)
      return (ushort?) o;
    if (type == Cast.NullableSByteType)
    {
      sbyte? nullable = (sbyte?) o;
      return !nullable.HasValue ? new ushort?() : new ushort?((ushort) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableByteType)
    {
      byte? nullable = (byte?) o;
      return !nullable.HasValue ? new ushort?() : new ushort?((ushort) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSingleType)
    {
      float? nullable = (float?) o;
      return !nullable.HasValue ? new ushort?() : new ushort?((ushort) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableCharType)
    {
      char? nullable = (char?) o;
      return !nullable.HasValue ? new ushort?() : new ushort?((ushort) nullable.GetValueOrDefault());
    }
    if (!(type == Cast.NullableDecimalType))
      throw Cast.InvalidCast(o, "UInt16");
    Decimal? nullable1 = (Decimal?) o;
    return !nullable1.HasValue ? new ushort?() : new ushort?((ushort) nullable1.GetValueOrDefault());
  }

  [CLSCompliant(false)]
  public static uint? ExplicitCastToNullableUInt32(object o)
  {
    if (o == null)
      return new uint?();
    Type type = o.GetType();
    if (type == Cast.Int32Type)
      return new uint?((uint) (int) o);
    if (type == Cast.DoubleType)
      return new uint?((uint) (double) o);
    if (type == Cast.Int64Type)
      return new uint?((uint) (long) o);
    if (type == Cast.Int16Type)
      return new uint?((uint) (short) o);
    if (type == Cast.UInt32Type)
      return new uint?((uint) o);
    if (type == Cast.UInt64Type)
      return new uint?((uint) (ulong) o);
    if (type == Cast.UInt16Type)
      return new uint?((uint) (ushort) o);
    if (type == Cast.SByteType)
      return new uint?((uint) (sbyte) o);
    if (type == Cast.ByteType)
      return new uint?((uint) (byte) o);
    if (type == Cast.SingleType)
      return new uint?((uint) (float) o);
    if (type == Cast.CharType)
      return new uint?((uint) (char) o);
    if (type == Cast.DecimalType)
      return new uint?((uint) (Decimal) o);
    if (type.IsEnum())
      return new uint?(Cast.ExplicitCastEnumToUInt32(o));
    if (type == Cast.NullableInt32Type)
    {
      int? nullable = (int?) o;
      return !nullable.HasValue ? new uint?() : new uint?((uint) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableDoubleType)
    {
      double? nullable = (double?) o;
      return !nullable.HasValue ? new uint?() : new uint?((uint) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt64Type)
    {
      long? nullable = (long?) o;
      return !nullable.HasValue ? new uint?() : new uint?((uint) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt16Type)
    {
      short? nullable = (short?) o;
      return !nullable.HasValue ? new uint?() : new uint?((uint) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt32Type)
      return (uint?) o;
    if (type == Cast.NullableUInt64Type)
    {
      ulong? nullable = (ulong?) o;
      return !nullable.HasValue ? new uint?() : new uint?((uint) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt16Type)
    {
      ushort? nullable = (ushort?) o;
      return !nullable.HasValue ? new uint?() : new uint?((uint) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSByteType)
    {
      sbyte? nullable = (sbyte?) o;
      return !nullable.HasValue ? new uint?() : new uint?((uint) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableByteType)
    {
      byte? nullable = (byte?) o;
      return !nullable.HasValue ? new uint?() : new uint?((uint) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSingleType)
    {
      float? nullable = (float?) o;
      return !nullable.HasValue ? new uint?() : new uint?((uint) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableCharType)
    {
      char? nullable = (char?) o;
      return !nullable.HasValue ? new uint?() : new uint?((uint) nullable.GetValueOrDefault());
    }
    if (!(type == Cast.NullableDecimalType))
      throw Cast.InvalidCast(o, "UInt32");
    Decimal? nullable1 = (Decimal?) o;
    return !nullable1.HasValue ? new uint?() : new uint?((uint) nullable1.GetValueOrDefault());
  }

  [CLSCompliant(false)]
  public static ulong? ExplicitCastToNullableUInt64(object o)
  {
    if (o == null)
      return new ulong?();
    Type type = o.GetType();
    if (type == Cast.Int32Type)
      return new ulong?((ulong) (int) o);
    if (type == Cast.DoubleType)
      return new ulong?((ulong) (double) o);
    if (type == Cast.Int64Type)
      return new ulong?((ulong) (long) o);
    if (type == Cast.Int16Type)
      return new ulong?((ulong) (short) o);
    if (type == Cast.UInt32Type)
      return new ulong?((ulong) (uint) o);
    if (type == Cast.UInt64Type)
      return new ulong?((ulong) o);
    if (type == Cast.UInt16Type)
      return new ulong?((ulong) (ushort) o);
    if (type == Cast.SByteType)
      return new ulong?((ulong) (sbyte) o);
    if (type == Cast.ByteType)
      return new ulong?((ulong) (byte) o);
    if (type == Cast.SingleType)
      return new ulong?((ulong) (float) o);
    if (type == Cast.CharType)
      return new ulong?((ulong) (char) o);
    if (type == Cast.DecimalType)
      return new ulong?((ulong) (Decimal) o);
    if (type.IsEnum())
      return new ulong?(Cast.ExplicitCastEnumToUInt64(o));
    if (type == Cast.NullableInt32Type)
    {
      int? nullable = (int?) o;
      return !nullable.HasValue ? new ulong?() : new ulong?((ulong) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableDoubleType)
    {
      double? nullable = (double?) o;
      return !nullable.HasValue ? new ulong?() : new ulong?((ulong) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt64Type)
    {
      long? nullable = (long?) o;
      return !nullable.HasValue ? new ulong?() : new ulong?((ulong) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableInt16Type)
    {
      short? nullable = (short?) o;
      return !nullable.HasValue ? new ulong?() : new ulong?((ulong) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt32Type)
    {
      uint? nullable = (uint?) o;
      return !nullable.HasValue ? new ulong?() : new ulong?((ulong) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableUInt64Type)
      return (ulong?) o;
    if (type == Cast.NullableUInt16Type)
    {
      ushort? nullable = (ushort?) o;
      return !nullable.HasValue ? new ulong?() : new ulong?((ulong) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSByteType)
    {
      sbyte? nullable = (sbyte?) o;
      return !nullable.HasValue ? new ulong?() : new ulong?((ulong) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableByteType)
    {
      byte? nullable = (byte?) o;
      return !nullable.HasValue ? new ulong?() : new ulong?((ulong) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableSingleType)
    {
      float? nullable = (float?) o;
      return !nullable.HasValue ? new ulong?() : new ulong?((ulong) nullable.GetValueOrDefault());
    }
    if (type == Cast.NullableCharType)
    {
      char? nullable = (char?) o;
      return !nullable.HasValue ? new ulong?() : new ulong?((ulong) nullable.GetValueOrDefault());
    }
    if (!(type == Cast.NullableDecimalType))
      throw Cast.InvalidCast(o, "UInt64");
    Decimal? nullable1 = (Decimal?) o;
    return !nullable1.HasValue ? new ulong?() : new ulong?((ulong) nullable1.GetValueOrDefault());
  }

  public static object NewNullableInstance(Type type)
  {
    if (type == Cast.Int32Type)
      return (object) null;
    if (type == Cast.DoubleType)
      return (object) null;
    if (type == Cast.BooleanType)
      return (object) null;
    if (type == Cast.Int64Type)
      return (object) null;
    if (type == Cast.Int16Type)
      return (object) null;
    if (type == Cast.UInt32Type)
      return (object) null;
    if (type == Cast.UInt64Type)
      return (object) null;
    if (type == Cast.UInt16Type)
      return (object) null;
    if (type == Cast.SByteType)
      return (object) null;
    if (type == Cast.ByteType)
      return (object) null;
    if (type == Cast.SingleType)
      return (object) null;
    if (type == Cast.CharType)
      return (object) null;
    return type == Cast.DecimalType ? (object) null : Cast.NewNullableInstanceSlow(type);
  }

  internal static byte ExplicitCastEnumToByte(object o)
  {
    switch (((Enum) o).GetTypeCode())
    {
      case TypeCode.SByte:
        return (byte) (sbyte) o;
      case TypeCode.Byte:
        return (byte) o;
      case TypeCode.Int16:
        return (byte) (short) o;
      case TypeCode.UInt16:
        return (byte) (ushort) o;
      case TypeCode.Int32:
        return (byte) (int) o;
      case TypeCode.UInt32:
        return (byte) (uint) o;
      case TypeCode.Int64:
        return (byte) (long) o;
      case TypeCode.UInt64:
        return (byte) (ulong) o;
      default:
        throw new InvalidOperationException("Invalid enum");
    }
  }

  internal static sbyte ExplicitCastEnumToSByte(object o)
  {
    switch (((Enum) o).GetTypeCode())
    {
      case TypeCode.SByte:
        return (sbyte) o;
      case TypeCode.Byte:
        return (sbyte) (byte) o;
      case TypeCode.Int16:
        return (sbyte) (short) o;
      case TypeCode.UInt16:
        return (sbyte) (ushort) o;
      case TypeCode.Int32:
        return (sbyte) (int) o;
      case TypeCode.UInt32:
        return (sbyte) (uint) o;
      case TypeCode.Int64:
        return (sbyte) (long) o;
      case TypeCode.UInt64:
        return (sbyte) (ulong) o;
      default:
        throw new InvalidOperationException("Invalid enum");
    }
  }

  internal static short ExplicitCastEnumToInt16(object o)
  {
    switch (((Enum) o).GetTypeCode())
    {
      case TypeCode.SByte:
        return (short) (sbyte) o;
      case TypeCode.Byte:
        return (short) (byte) o;
      case TypeCode.Int16:
        return (short) o;
      case TypeCode.UInt16:
        return (short) (ushort) o;
      case TypeCode.Int32:
        return (short) (int) o;
      case TypeCode.UInt32:
        return (short) (uint) o;
      case TypeCode.Int64:
        return (short) (long) o;
      case TypeCode.UInt64:
        return (short) (ulong) o;
      default:
        throw new InvalidOperationException("Invalid enum");
    }
  }

  internal static ushort ExplicitCastEnumToUInt16(object o)
  {
    switch (((Enum) o).GetTypeCode())
    {
      case TypeCode.SByte:
        return (ushort) (sbyte) o;
      case TypeCode.Byte:
        return (ushort) (byte) o;
      case TypeCode.Int16:
        return (ushort) (short) o;
      case TypeCode.UInt16:
        return (ushort) o;
      case TypeCode.Int32:
        return (ushort) (int) o;
      case TypeCode.UInt32:
        return (ushort) (uint) o;
      case TypeCode.Int64:
        return (ushort) (long) o;
      case TypeCode.UInt64:
        return (ushort) (ulong) o;
      default:
        throw new InvalidOperationException("Invalid enum");
    }
  }

  internal static int ExplicitCastEnumToInt32(object o)
  {
    switch (((Enum) o).GetTypeCode())
    {
      case TypeCode.SByte:
        return (int) (sbyte) o;
      case TypeCode.Byte:
        return (int) (byte) o;
      case TypeCode.Int16:
        return (int) (short) o;
      case TypeCode.UInt16:
        return (int) (ushort) o;
      case TypeCode.Int32:
        return (int) o;
      case TypeCode.UInt32:
        return (int) (uint) o;
      case TypeCode.Int64:
        return (int) (long) o;
      case TypeCode.UInt64:
        return (int) (ulong) o;
      default:
        throw new InvalidOperationException("Invalid enum");
    }
  }

  internal static uint ExplicitCastEnumToUInt32(object o)
  {
    switch (((Enum) o).GetTypeCode())
    {
      case TypeCode.SByte:
        return (uint) (sbyte) o;
      case TypeCode.Byte:
        return (uint) (byte) o;
      case TypeCode.Int16:
        return (uint) (short) o;
      case TypeCode.UInt16:
        return (uint) (ushort) o;
      case TypeCode.Int32:
        return (uint) (int) o;
      case TypeCode.UInt32:
        return (uint) o;
      case TypeCode.Int64:
        return (uint) (long) o;
      case TypeCode.UInt64:
        return (uint) (ulong) o;
      default:
        throw new InvalidOperationException("Invalid enum");
    }
  }

  internal static long ExplicitCastEnumToInt64(object o)
  {
    switch (((Enum) o).GetTypeCode())
    {
      case TypeCode.SByte:
        return (long) (sbyte) o;
      case TypeCode.Byte:
        return (long) (byte) o;
      case TypeCode.Int16:
        return (long) (short) o;
      case TypeCode.UInt16:
        return (long) (ushort) o;
      case TypeCode.Int32:
        return (long) (int) o;
      case TypeCode.UInt32:
        return (long) (uint) o;
      case TypeCode.Int64:
        return (long) o;
      case TypeCode.UInt64:
        return (long) (ulong) o;
      default:
        throw new InvalidOperationException("Invalid enum");
    }
  }

  internal static ulong ExplicitCastEnumToUInt64(object o)
  {
    switch (((Enum) o).GetTypeCode())
    {
      case TypeCode.SByte:
        return (ulong) (sbyte) o;
      case TypeCode.Byte:
        return (ulong) (byte) o;
      case TypeCode.Int16:
        return (ulong) (short) o;
      case TypeCode.UInt16:
        return (ulong) (ushort) o;
      case TypeCode.Int32:
        return (ulong) (int) o;
      case TypeCode.UInt32:
        return (ulong) (uint) o;
      case TypeCode.Int64:
        return (ulong) (long) o;
      case TypeCode.UInt64:
        return (ulong) o;
      default:
        throw new InvalidOperationException("Invalid enum");
    }
  }
}
