// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.EnumUtils
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Utils;

public static class EnumUtils
{
  public static object BitwiseOr(object self, object other)
  {
    if (self is Enum && other is Enum)
    {
      Type type1 = self.GetType();
      Type type2 = other.GetType();
      if (type1 == type2)
      {
        Type underlyingType = Enum.GetUnderlyingType(type1);
        if (underlyingType == typeof (int))
          return Enum.ToObject(type1, (int) self | (int) other);
        if (underlyingType == typeof (long))
          return Enum.ToObject(type1, (long) self | (long) other);
        if (underlyingType == typeof (short))
          return Enum.ToObject(type1, (int) (short) self | (int) (short) other);
        if (underlyingType == typeof (byte))
          return Enum.ToObject(type1, (int) (byte) self | (int) (byte) other);
        if (underlyingType == typeof (sbyte))
          return Enum.ToObject(type1, (int) (sbyte) self | (int) (sbyte) other);
        if (underlyingType == typeof (uint))
          return Enum.ToObject(type1, (uint) self | (uint) other);
        if (underlyingType == typeof (ulong))
          return Enum.ToObject(type1, (ulong) self | (ulong) other);
        if (underlyingType == typeof (ushort))
          return Enum.ToObject(type1, (int) (ushort) self | (int) (ushort) other);
        throw Assert.Unreachable;
      }
    }
    return (object) null;
  }

  public static object BitwiseAnd(object self, object other)
  {
    if (self is Enum && other is Enum)
    {
      Type type1 = self.GetType();
      Type type2 = other.GetType();
      if (type1 == type2)
      {
        Type underlyingType = Enum.GetUnderlyingType(type1);
        if (underlyingType == typeof (int))
          return Enum.ToObject(type1, (int) self & (int) other);
        if (underlyingType == typeof (long))
          return Enum.ToObject(type1, (long) self & (long) other);
        if (underlyingType == typeof (short))
          return Enum.ToObject(type1, (int) (short) self & (int) (short) other);
        if (underlyingType == typeof (byte))
          return Enum.ToObject(type1, (int) (byte) self & (int) (byte) other);
        if (underlyingType == typeof (sbyte))
          return Enum.ToObject(type1, (int) (sbyte) self & (int) (sbyte) other);
        if (underlyingType == typeof (uint))
          return Enum.ToObject(type1, (uint) self & (uint) other);
        if (underlyingType == typeof (ulong))
          return Enum.ToObject(type1, (ulong) self & (ulong) other);
        if (underlyingType == typeof (ushort))
          return Enum.ToObject(type1, (int) (ushort) self & (int) (ushort) other);
        throw Assert.Unreachable;
      }
    }
    return (object) null;
  }

  public static object ExclusiveOr(object self, object other)
  {
    if (self is Enum && other is Enum)
    {
      Type type1 = self.GetType();
      Type type2 = other.GetType();
      if (type1 == type2)
      {
        Type underlyingType = Enum.GetUnderlyingType(type1);
        if (underlyingType == typeof (int))
          return Enum.ToObject(type1, (int) self ^ (int) other);
        if (underlyingType == typeof (long))
          return Enum.ToObject(type1, (long) self ^ (long) other);
        if (underlyingType == typeof (short))
          return Enum.ToObject(type1, (int) (short) self ^ (int) (short) other);
        if (underlyingType == typeof (byte))
          return Enum.ToObject(type1, (int) (byte) self ^ (int) (byte) other);
        if (underlyingType == typeof (sbyte))
          return Enum.ToObject(type1, (int) (sbyte) self ^ (int) (sbyte) other);
        if (underlyingType == typeof (uint))
          return Enum.ToObject(type1, (uint) self ^ (uint) other);
        if (underlyingType == typeof (ulong))
          return Enum.ToObject(type1, (ulong) self ^ (ulong) other);
        if (underlyingType == typeof (ushort))
          return Enum.ToObject(type1, (int) (ushort) self ^ (int) (ushort) other);
        throw Assert.Unreachable;
      }
    }
    return (object) null;
  }

  public static object OnesComplement(object self)
  {
    if (!(self is Enum))
      return (object) null;
    Type type = self.GetType();
    Type underlyingType = Enum.GetUnderlyingType(type);
    if (underlyingType == typeof (int))
      return Enum.ToObject(type, ~(int) self);
    if (underlyingType == typeof (long))
      return Enum.ToObject(type, ~(long) self);
    if (underlyingType == typeof (short))
      return Enum.ToObject(type, (int) ~(short) self);
    if (underlyingType == typeof (byte))
      return Enum.ToObject(type, (int) ~(byte) self);
    if (underlyingType == typeof (sbyte))
      return Enum.ToObject(type, (int) ~(sbyte) self);
    if (underlyingType == typeof (uint))
      return Enum.ToObject(type, ~(uint) self);
    if (underlyingType == typeof (ulong))
      return Enum.ToObject(type, ~(ulong) self);
    if (underlyingType == typeof (ushort))
      return Enum.ToObject(type, (int) ~(ushort) self);
    throw Assert.Unreachable;
  }
}
