// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.Int32Ops
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class Int32Ops
{
  private static object FastNew(CodeContext context, object o)
  {
    switch (o)
    {
      case string _:
        return Int32Ops.__new__((PythonType) null, (string) o, 10);
      case double d1:
        return DoubleOps.__int__(d1);
      case int _:
        return o;
      case bool flag:
        return (object) (flag ? 1 : 0);
      case BigInteger self:
        int ret1;
        return self.AsInt32(out ret1) ? ScriptingRuntimeHelpers.Int32ToObject(ret1) : o;
      case Extensible<BigInteger> extensible:
        int ret2;
        return extensible.Value.AsInt32(out ret2) ? ScriptingRuntimeHelpers.Int32ToObject(ret2) : (object) extensible.Value;
      case float d2:
        return DoubleOps.__int__((double) d2);
      case Complex _:
        throw PythonOps.TypeError("can't convert complex to int; use int(abs(z))");
      case long num1:
        return (long) int.MinValue <= num1 && num1 <= (long) int.MaxValue ? (object) (int) num1 : (object) (BigInteger) num1;
      case uint num2:
        return num2 <= (uint) int.MaxValue ? (object) (int) num2 : (object) (BigInteger) num2;
      case ulong num3:
        return num3 <= (ulong) int.MaxValue ? (object) (int) num3 : (object) (BigInteger) num3;
      case Decimal num4:
        return -2147483648M <= num4 && num4 <= 2147483647M ? (object) (int) num4 : (object) (BigInteger) num4;
      case Enum _:
        return (object) ((IConvertible) o).ToInt32((IFormatProvider) null);
      case Extensible<string> o1:
        object obj;
        return PythonTypeOps.TryInvokeUnaryOperator(DefaultContext.Default, (object) o1, "__int__", out obj) ? obj : Int32Ops.__new__((PythonType) null, o1.Value, 10);
      default:
        object ret3;
        if (PythonTypeOps.TryInvokeUnaryOperator(context, o, "__int__", out ret3) && ret3 != NotImplementedType.Value)
        {
          switch (ret3)
          {
            case int _:
            case BigInteger _:
            case Extensible<int> _:
            case Extensible<BigInteger> _:
              return ret3;
            default:
              throw PythonOps.TypeError("__int__ returned non-Integral (type {0})", (object) PythonTypeOps.GetOldName(ret3));
          }
        }
        else if (PythonOps.TryGetBoundAttr(context, o, "__trunc__", out ret3))
        {
          ret3 = PythonOps.CallWithContext(context, ret3);
          switch (ret3)
          {
            case int _:
            case BigInteger _:
            case Extensible<int> _:
            case Extensible<BigInteger> _:
              return ret3;
            default:
              int result1;
              if (Converter.TryConvertToInt32(ret3, out result1))
                return (object) result1;
              BigInteger result2;
              if (Converter.TryConvertToBigInteger(ret3, out result2))
                return (object) result2;
              throw PythonOps.TypeError("__trunc__ returned non-Integral (type {0})", (object) PythonTypeOps.GetOldName(ret3));
          }
        }
        else
        {
          if (o is OldInstance)
            throw PythonOps.AttributeError("{0} instance has no attribute '__trunc__'", (object) PythonTypeOps.GetOldName((OldInstance) o));
          throw PythonOps.TypeError("int() argument must be a string or a number, not '{0}'", (object) PythonTypeOps.GetName(o));
        }
    }
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, object o)
  {
    return Int32Ops.__new__(context, TypeCache.Int32, o);
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, Extensible<double> o)
  {
    object obj;
    PythonTypeOps.TryInvokeUnaryOperator(context, (object) o, "__int__", out obj);
    return cls == TypeCache.Int32 ? (object) (int) obj : cls.CreateInstance(context, obj);
  }

  private static void ValidateType(PythonType cls)
  {
    if (cls == TypeCache.Boolean)
      throw PythonOps.TypeError("int.__new__(bool) is not safe, use bool.__new__()");
  }

  [StaticExtensionMethod]
  public static object __new__(PythonType cls, string s, int @base)
  {
    Int32Ops.ValidateType(cls);
    int start = 0;
    if (@base == 16 /*0x10*/ || @base == 8 || @base == 2)
      start = s.Length - Int32Ops.TrimRadix(s, @base).Length;
    return LiteralParser.ParseIntegerSign(s, @base, start);
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, IList<byte> s)
  {
    object obj;
    if (!(s is IPythonObject o) || !PythonTypeOps.TryInvokeUnaryOperator(DefaultContext.Default, (object) o, "__int__", out obj))
      obj = Int32Ops.FastNew(context, (object) s.MakeString());
    if (cls == TypeCache.Int32)
      return obj;
    Int32Ops.ValidateType(cls);
    return cls.CreateInstance(context, obj);
  }

  internal static string TrimRadix(string s, int radix)
  {
    for (int index = 0; index < s.Length; ++index)
    {
      if (!char.IsWhiteSpace(s[index]))
      {
        if (s[index] == '0' && index < s.Length - 1)
        {
          switch (radix)
          {
            case 2:
              if (s[index + 1] == 'b' || s[index + 1] == 'B')
              {
                s = s.Substring(index + 2);
                goto label_12;
              }
              goto label_12;
            case 8:
              if (s[index + 1] == 'o' || s[index + 1] == 'O')
              {
                s = s.Substring(index + 2);
                goto label_12;
              }
              goto label_12;
            case 16 /*0x10*/:
              if (s[index + 1] == 'x' || s[index + 1] == 'X')
              {
                s = s.Substring(index + 2);
                goto label_12;
              }
              goto label_12;
            default:
              goto label_12;
          }
        }
        else
          break;
      }
    }
label_12:
    return s;
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, object x)
  {
    object obj = Int32Ops.FastNew(context, x);
    if (cls == TypeCache.Int32)
      return obj;
    Int32Ops.ValidateType(cls);
    return cls.CreateInstance(context, obj);
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls)
  {
    return cls == TypeCache.Int32 ? (object) 0 : cls.CreateInstance(context);
  }

  [SpecialName]
  public static object FloorDivide(int x, int y)
  {
    return y == -1 && x == int.MinValue ? (object) -(BigInteger) int.MinValue : ScriptingRuntimeHelpers.Int32ToObject(MathUtils.FloorDivideUnchecked(x, y));
  }

  [SpecialName]
  public static int Mod(int x, int y) => MathUtils.FloorRemainder(x, y);

  [SpecialName]
  public static object Power(int x, BigInteger power, BigInteger qmod)
  {
    return BigIntegerOps.Power((BigInteger) x, power, qmod);
  }

  [SpecialName]
  public static object Power(int x, double power, double qmod) => (object) NotImplementedType.Value;

  [SpecialName]
  public static object Power(int x, int power, int? qmod)
  {
    if (!qmod.HasValue)
      return Int32Ops.Power(x, power);
    int num1 = qmod.Value;
    if (power < 0)
      throw PythonOps.TypeError(nameof (power), (object) power, (object) "power must be >= 0");
    if (num1 == 0)
      throw PythonOps.ZeroDivisionError();
    long num2 = (long) (1 % num1);
    long num3 = (long) x;
    for (; power != 0; power >>= 1)
    {
      if ((power & 1) != 0)
        num2 = num2 * num3 % (long) num1;
      num3 = num3 * num3 % (long) num1;
    }
    if (num1 < 0 && num2 > 0L || num1 > 0 && num2 < 0L)
      num2 += (long) num1;
    return (object) (int) num2;
  }

  [SpecialName]
  public static object Power(int x, int power)
  {
    if (power == 0)
      return (object) 1;
    if (power < 0)
      return x != 0 ? (object) DoubleOps.Power((double) x, (double) power) : throw PythonOps.ZeroDivisionError("0.0 cannot be raised to a negative power");
    int num1 = x;
    int num2 = 1;
    int y = power;
    try
    {
      for (; power != 0; power >>= 1)
      {
        if ((power & 1) != 0)
          checked { num2 *= num1; }
        if (power != 1)
          checked { num1 *= num1; }
        else
          break;
      }
      return (object) num2;
    }
    catch (OverflowException ex)
    {
      return BigIntegerOps.Power((BigInteger) x, y);
    }
  }

  [SpecialName]
  public static object LeftShift(int x, int y)
  {
    if (y < 0)
      throw PythonOps.ValueError("negative shift count");
    return y > 31 /*0x1F*/ || x > 0 && x > int.MaxValue >> y || x < 0 && x < int.MinValue >> y ? Int64Ops.LeftShift((long) x, (BigInteger) y) : ScriptingRuntimeHelpers.Int32ToObject(x << y);
  }

  [SpecialName]
  public static int RightShift(int x, int y)
  {
    if (y < 0)
      throw PythonOps.ValueError("negative shift count");
    if (y > 31 /*0x1F*/)
      return x < 0 ? -1 : 0;
    int num;
    if (x >= 0)
    {
      num = x >> y;
    }
    else
    {
      num = x + ((1 << y) - 1) >> y;
      if (x - (num << y) != 0)
        --num;
    }
    return num;
  }

  public static PythonTuple __divmod__(int x, int y)
  {
    return PythonTuple.MakeTuple(Int32Ops.Divide(x, y), (object) Int32Ops.Mod(x, y));
  }

  [return: MaybeNotImplemented]
  public static object __divmod__(int x, object y) => (object) NotImplementedType.Value;

  public static string __oct__(int x)
  {
    if (x == 0)
      return "0";
    return x > 0 ? "0" + ((BigInteger) x).ToString(8) : "-0" + ((BigInteger) -x).ToString(8);
  }

  public static string __hex__(int x)
  {
    return x < 0 ? "-0x" + (-x).ToString(nameof (x)) : "0x" + x.ToString(nameof (x));
  }

  public static object __getnewargs__(CodeContext context, int self)
  {
    return (object) PythonTuple.MakeTuple(Int32Ops.__new__(context, TypeCache.Int32, (object) self));
  }

  public static object __rdivmod__(int x, int y) => (object) Int32Ops.__divmod__(y, x);

  public static int __int__(int self) => self;

  public static BigInteger __long__(int self) => (BigInteger) self;

  public static double __float__(int self) => (double) self;

  public static int __abs__(int self) => Math.Abs(self);

  public static object __coerce__(CodeContext context, int x, object o)
  {
    if (!(o is int))
      return (object) NotImplementedType.Value;
    return (object) PythonTuple.MakeTuple(ScriptingRuntimeHelpers.Int32ToObject(x), o);
  }

  public static string __format__(CodeContext context, int self, [NotNull] string formatSpec)
  {
    StringFormatSpec spec = StringFormatSpec.FromString(formatSpec);
    if (spec.Precision.HasValue)
      throw PythonOps.ValueError("Precision not allowed in integer format specifier");
    char? type = spec.Type;
    string text;
    if (type.HasValue)
    {
      switch (type.GetValueOrDefault())
      {
        case '%':
          text = !spec.ThousandsComma ? self.ToString("0.000000%", (IFormatProvider) CultureInfo.InvariantCulture) : self.ToString("#,0.000000%", (IFormatProvider) CultureInfo.InvariantCulture);
          goto label_31;
        case 'E':
          text = !spec.ThousandsComma ? self.ToString("0.000000E+00", (IFormatProvider) CultureInfo.InvariantCulture) : self.ToString("#,0.000000E+00", (IFormatProvider) CultureInfo.InvariantCulture);
          goto label_31;
        case 'F':
        case 'f':
          text = !spec.ThousandsComma ? self.ToString("#########0.000000", (IFormatProvider) CultureInfo.InvariantCulture) : self.ToString("#,########0.000000", (IFormatProvider) CultureInfo.InvariantCulture);
          goto label_31;
        case 'G':
          if (self >= 1000000 || self <= -1000000)
          {
            text = self.ToString("0.#####E+00", (IFormatProvider) CultureInfo.InvariantCulture);
            goto label_31;
          }
          if (!spec.ThousandsComma)
          {
            text = self.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            goto label_31;
          }
          break;
        case 'X':
          text = Int32Ops.ToHex(self, false);
          goto label_31;
        case 'b':
          text = Int32Ops.ToBinary(self, false);
          goto label_31;
        case 'c':
          if (spec.Sign.HasValue)
            throw PythonOps.ValueError("Sign not allowed with integer format specifier 'c'");
          text = self >= 0 && self <= (int) byte.MaxValue ? ScriptingRuntimeHelpers.CharToString((char) self) : throw PythonOps.OverflowError("%c arg not in range(0x10000)");
          goto label_31;
        case 'd':
          break;
        case 'e':
          text = !spec.ThousandsComma ? self.ToString("0.000000e+00", (IFormatProvider) CultureInfo.InvariantCulture) : self.ToString("#,0.000000e+00", (IFormatProvider) CultureInfo.InvariantCulture);
          goto label_31;
        case 'g':
          if (self >= 1000000 || self <= -1000000)
          {
            text = self.ToString("0.#####e+00", (IFormatProvider) CultureInfo.InvariantCulture);
            goto label_31;
          }
          if (!spec.ThousandsComma)
          {
            text = self.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            goto label_31;
          }
          break;
        case 'n':
          CultureInfo numericCulture = context.LanguageContext.NumericCulture;
          if (numericCulture != CultureInfo.InvariantCulture)
          {
            int num = spec.Width ?? 0;
            text = !spec.Fill.HasValue || spec.Fill.Value != '0' || num <= 1 ? self.ToString("N0", (IFormatProvider) numericCulture) : FormattingHelper.ToCultureString<int>(self, numericCulture.NumberFormat, spec);
            goto label_31;
          }
          break;
        case 'o':
          text = Int32Ops.ToOctal(self, true);
          goto label_31;
        case 'x':
          text = Int32Ops.ToHex(self, true);
          goto label_31;
        default:
          throw PythonOps.ValueError("Unknown format code '{0}'", (object) spec.Type.ToString());
      }
    }
    if (spec.ThousandsComma)
    {
      int num = spec.Width ?? 0;
      text = !spec.Fill.HasValue || spec.Fill.Value != '0' || num <= 1 ? self.ToString("#,0", (IFormatProvider) CultureInfo.InvariantCulture) : FormattingHelper.ToCultureString<int>(self, FormattingHelper.InvariantCommaNumberInfo, spec);
    }
    else
      text = self.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
label_31:
    if (self < 0 && text[0] == '-')
      text = text.Substring(1);
    return spec.AlignNumericText(text, self == 0, self > 0);
  }

  private static string ToHex(int self, bool lowercase)
  {
    string hex;
    if (self != int.MinValue)
    {
      int num = self;
      if (self < 0)
        num = -self;
      hex = num.ToString(lowercase ? "x" : "X", (IFormatProvider) CultureInfo.InvariantCulture);
    }
    else
      hex = "80000000";
    return hex;
  }

  private static string ToOctal(int self, bool lowercase)
  {
    string octal;
    switch (self)
    {
      case int.MinValue:
        octal = "20000000000";
        break;
      case 0:
        octal = "0";
        break;
      default:
        int num = self;
        if (self < 0)
          num = -self;
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = 30; index >= 0; index -= 3)
        {
          char ch = (char) (48 /*0x30*/ + (num >> index & 7));
          if (ch != '0' || stringBuilder.Length > 0)
            stringBuilder.Append(ch);
        }
        octal = stringBuilder.ToString();
        break;
    }
    return octal;
  }

  internal static string ToBinary(int self)
  {
    if (self == int.MinValue)
      return "-0b10000000000000000000000000000000";
    string binary = Int32Ops.ToBinary(self, true);
    if (self < 0)
      binary = "-" + binary;
    return binary;
  }

  private static string ToBinary(int self, bool includeType)
  {
    string binary;
    switch (self)
    {
      case int.MinValue:
        binary = "10000000000000000000000000000000";
        break;
      case 0:
        binary = "0";
        break;
      default:
        StringBuilder stringBuilder = new StringBuilder();
        int num = self;
        if (self < 0)
          num = -self;
        for (int index = 31 /*0x1F*/; index >= 0; --index)
        {
          if ((num & 1 << index) != 0)
            stringBuilder.Append('1');
          else if (stringBuilder.Length != 0)
            stringBuilder.Append('0');
        }
        binary = stringBuilder.ToString();
        break;
    }
    if (includeType)
      binary = "0b" + binary;
    return binary;
  }

  [SpecialName]
  public static int Plus(int x) => x;

  [SpecialName]
  public static object Negate(int x)
  {
    return x == int.MinValue ? (object) -(BigInteger) int.MinValue : (object) -x;
  }

  [SpecialName]
  public static object Abs(int x)
  {
    if (x >= 0)
      return (object) x;
    return x == int.MinValue ? (object) -(BigInteger) int.MinValue : (object) -x;
  }

  [SpecialName]
  public static int OnesComplement(int x) => ~x;

  public static bool __nonzero__(int x) => x != 0;

  public static string __repr__(int x)
  {
    return x.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static int __trunc__(int x) => x;

  public static int __hash__(int x) => x;

  public static int __index__(int x) => x;

  [SpecialName]
  public static object Add(int x, int y)
  {
    long num = (long) x + (long) y;
    return (long) int.MinValue <= num && num <= (long) int.MaxValue ? ScriptingRuntimeHelpers.Int32ToObject((int) num) : (object) BigIntegerOps.Add((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object Subtract(int x, int y)
  {
    long num = (long) x - (long) y;
    return (long) int.MinValue <= num && num <= (long) int.MaxValue ? ScriptingRuntimeHelpers.Int32ToObject((int) num) : (object) BigIntegerOps.Subtract((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object Multiply(int x, int y)
  {
    long num = (long) x * (long) y;
    return (long) int.MinValue <= num && num <= (long) int.MaxValue ? ScriptingRuntimeHelpers.Int32ToObject((int) num) : (object) BigIntegerOps.Multiply((BigInteger) x, (BigInteger) y);
  }

  [SpecialName]
  public static object Divide(int x, int y) => Int32Ops.FloorDivide(x, y);

  [SpecialName]
  public static double TrueDivide(int x, int y) => DoubleOps.TrueDivide((double) x, (double) y);

  [SpecialName]
  public static object LeftShift(int x, [NotNull] BigInteger y)
  {
    return (object) BigIntegerOps.LeftShift((BigInteger) x, y);
  }

  [SpecialName]
  public static int RightShift(int x, [NotNull] BigInteger y)
  {
    return (int) BigIntegerOps.RightShift((BigInteger) x, y);
  }

  [SpecialName]
  public static int BitwiseAnd(int x, int y) => x & y;

  [SpecialName]
  public static int BitwiseOr(int x, int y) => x | y;

  [SpecialName]
  public static int ExclusiveOr(int x, int y) => x ^ y;

  [SpecialName]
  public static int Compare(int x, int y)
  {
    if (x == y)
      return 0;
    return x <= y ? -1 : 1;
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static sbyte ConvertToSByte(int x)
  {
    return (int) sbyte.MinValue <= x && x <= (int) sbyte.MaxValue ? (sbyte) x : throw Converter.CannotConvertOverflow("SByte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static byte ConvertToByte(int x)
  {
    return 0 <= x && x <= (int) byte.MaxValue ? (byte) x : throw Converter.CannotConvertOverflow("Byte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static short ConvertToInt16(int x)
  {
    return (int) short.MinValue <= x && x <= (int) short.MaxValue ? (short) x : throw Converter.CannotConvertOverflow("Int16", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static ushort ConvertToUInt16(int x)
  {
    return 0 <= x && x <= (int) ushort.MaxValue ? (ushort) x : throw Converter.CannotConvertOverflow("UInt16", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static uint ConvertToUInt32(int x)
  {
    return x >= 0 ? (uint) x : throw Converter.CannotConvertOverflow("UInt32", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static long ConvertToInt64(int x) => (long) x;

  [ExplicitConversionMethod]
  [SpecialName]
  public static ulong ConvertToUInt64(int x)
  {
    return x >= 0 ? (ulong) x : throw Converter.CannotConvertOverflow("UInt64", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static float ConvertToSingle(int x) => (float) x;

  [ImplicitConversionMethod]
  [SpecialName]
  public static double ConvertToDouble(int x) => (double) x;

  [PropertyMethod]
  [SpecialName]
  public static int Getreal(int x) => x;

  [PropertyMethod]
  [SpecialName]
  public static int Getimag(int x) => 0;

  public static int conjugate(int x) => x;

  [PropertyMethod]
  [SpecialName]
  public static int Getnumerator(int x) => x;

  [PropertyMethod]
  [SpecialName]
  public static int Getdenominator(int x) => 1;

  public static int bit_length(int value) => MathUtils.BitLength(value);
}
