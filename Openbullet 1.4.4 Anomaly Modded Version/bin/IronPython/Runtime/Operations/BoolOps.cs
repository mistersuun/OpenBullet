// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.BoolOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class BoolOps
{
  [StaticExtensionMethod]
  public static object __new__(object cls) => ScriptingRuntimeHelpers.False;

  [StaticExtensionMethod]
  public static bool __new__(object cls, object o) => PythonOps.IsTrue(o);

  [SpecialName]
  public static bool BitwiseAnd(bool x, bool y) => x & y;

  [SpecialName]
  public static bool BitwiseOr(bool x, bool y) => x | y;

  [SpecialName]
  public static bool ExclusiveOr(bool x, bool y) => x ^ y;

  [SpecialName]
  public static int BitwiseAnd(int x, bool y) => Int32Ops.BitwiseAnd(y ? 1 : 0, x);

  [SpecialName]
  public static int BitwiseAnd(bool x, int y) => Int32Ops.BitwiseAnd(x ? 1 : 0, y);

  [SpecialName]
  public static int BitwiseOr(int x, bool y) => Int32Ops.BitwiseOr(y ? 1 : 0, x);

  [SpecialName]
  public static int BitwiseOr(bool x, int y) => Int32Ops.BitwiseOr(x ? 1 : 0, y);

  [SpecialName]
  public static int ExclusiveOr(int x, bool y) => Int32Ops.ExclusiveOr(y ? 1 : 0, x);

  [SpecialName]
  public static int ExclusiveOr(bool x, int y) => Int32Ops.ExclusiveOr(x ? 1 : 0, y);

  public static string __repr__(bool self) => !self ? "False" : "True";

  public static string __format__(CodeContext context, bool self, [NotNull] string formatSpec)
  {
    StringFormatSpec stringFormatSpec = StringFormatSpec.FromString(formatSpec);
    return stringFormatSpec.IsEmpty ? BoolOps.__repr__(self) : stringFormatSpec.AlignNumericText(self ? "1" : "0", !self, true);
  }

  [SpecialName]
  public static bool Equals(bool x, bool y) => x == y;

  [SpecialName]
  public static bool NotEquals(bool x, bool y) => x != y;

  [SpecialName]
  public static bool Equals(bool x, int y) => (x ? 1 : 0) == y;

  [SpecialName]
  public static bool NotEquals(bool x, int y) => (x ? 1 : 0) != y;

  [SpecialName]
  public static bool Equals(int x, bool y) => BoolOps.Equals(y, x);

  [SpecialName]
  public static bool NotEquals(int x, bool y) => BoolOps.NotEquals(y, x);

  [ImplicitConversionMethod]
  [SpecialName]
  public static sbyte ConvertToSByte(bool x) => x ? (sbyte) 1 : (sbyte) 0;

  [ImplicitConversionMethod]
  [SpecialName]
  public static byte ConvertToByte(bool x) => x ? (byte) 1 : (byte) 0;

  [ImplicitConversionMethod]
  [SpecialName]
  public static short ConvertToInt16(bool x) => x ? (short) 1 : (short) 0;

  [ImplicitConversionMethod]
  [SpecialName]
  public static ushort ConvertToUInt16(bool x) => x ? (ushort) 1 : (ushort) 0;

  public static int __int__(bool x) => !x ? 0 : 1;

  [ImplicitConversionMethod]
  [SpecialName]
  public static int ConvertToInt32(bool x) => !x ? 0 : 1;

  [ImplicitConversionMethod]
  [SpecialName]
  public static uint ConvertToUInt32(bool x) => !x ? 0U : 1U;

  [ImplicitConversionMethod]
  [SpecialName]
  public static long ConvertToInt64(bool x) => x ? 1L : 0L;

  [ImplicitConversionMethod]
  [SpecialName]
  public static ulong ConvertToUInt64(bool x) => x ? 1UL : 0UL;

  [ImplicitConversionMethod]
  [SpecialName]
  public static float ConvertToSingle(bool x) => x ? 1f : 0.0f;

  [ImplicitConversionMethod]
  [SpecialName]
  public static double ConvertToDouble(bool x) => x ? 1.0 : 0.0;

  [ImplicitConversionMethod]
  [SpecialName]
  public static Complex ConvertToComplex(bool x) => !x ? Complex.Zero : Complex.One;

  [ImplicitConversionMethod]
  [SpecialName]
  public static Decimal ConvertToDecimal(bool x) => !x ? 0M : 1M;
}
