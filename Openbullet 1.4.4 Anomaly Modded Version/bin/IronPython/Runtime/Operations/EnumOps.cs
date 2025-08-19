// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.EnumOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Utils;
using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class EnumOps
{
  [SpecialName]
  public static object BitwiseOr(object self, object other)
  {
    return EnumUtils.BitwiseOr(self, other) ?? throw PythonOps.ValueError("bitwise or cannot be applied to {0} and {1}", (object) self.GetType(), (object) other.GetType());
  }

  [SpecialName]
  public static object BitwiseAnd(object self, object other)
  {
    return EnumUtils.BitwiseAnd(self, other) ?? throw PythonOps.ValueError("bitwise and cannot be applied to {0} and {1}", (object) self.GetType(), (object) other.GetType());
  }

  [SpecialName]
  public static object ExclusiveOr(object self, object other)
  {
    return EnumUtils.ExclusiveOr(self, other) ?? throw PythonOps.ValueError("bitwise xor cannot be applied to {0} and {1}", (object) self.GetType(), (object) other.GetType());
  }

  [SpecialName]
  public static object OnesComplement(object self)
  {
    return EnumUtils.OnesComplement(self) ?? throw PythonOps.ValueError("one's complement cannot be applied to {0}", (object) self.GetType());
  }

  public static bool __nonzero__(object self)
  {
    if (self is Enum)
    {
      switch (Enum.GetUnderlyingType(self.GetType()).GetTypeCode())
      {
        case TypeCode.SByte:
          return (sbyte) self != (sbyte) 0;
        case TypeCode.Byte:
          return (byte) self > (byte) 0;
        case TypeCode.Int16:
          return (short) self != (short) 0;
        case TypeCode.UInt16:
          return (ushort) self > (ushort) 0;
        case TypeCode.Int32:
          return (int) self != 0;
        case TypeCode.UInt32:
          return (uint) self > 0U;
        case TypeCode.Int64:
          return (long) self != 0L;
        case TypeCode.UInt64:
          return ~(ulong) self > 0UL;
      }
    }
    throw PythonOps.ValueError("__nonzero__ cannot be applied to {0}", (object) self.GetType());
  }

  public static string __repr__(object self)
  {
    if (!Enum.IsDefined(self.GetType(), self))
      return $"<enum {self.GetType().FullName}: {self.ToString()}>";
    string name = Enum.GetName(self.GetType(), self);
    return $"{self.GetType().FullName}.{name}";
  }
}
