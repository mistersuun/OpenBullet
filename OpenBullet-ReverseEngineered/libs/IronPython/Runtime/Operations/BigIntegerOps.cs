// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.BigIntegerOps
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

public static class BigIntegerOps
{
  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, string s, int radix)
  {
    int start = 0;
    if (radix == 16 /*0x10*/ || radix == 8 || radix == 2)
      start = s.Length - Int32Ops.TrimRadix(s, radix).Length;
    if (cls == TypeCache.BigInteger)
      return (object) BigIntegerOps.ParseBigIntegerSign(s, radix, start);
    BigInteger bigIntegerSign = BigIntegerOps.ParseBigIntegerSign(s, radix);
    return cls.CreateInstance(context, (object) bigIntegerSign);
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, IList<byte> s)
  {
    return BigIntegerOps.__new__(context, cls, s, 10);
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, IList<byte> s, int redix)
  {
    object bigIntegerSign;
    if (!(s is IPythonObject o) || !PythonTypeOps.TryInvokeUnaryOperator(DefaultContext.Default, (object) o, "__long__", out bigIntegerSign))
      bigIntegerSign = (object) BigIntegerOps.ParseBigIntegerSign(s.MakeString(), redix);
    return cls == TypeCache.BigInteger ? bigIntegerSign : cls.CreateInstance(context, bigIntegerSign);
  }

  private static BigInteger ParseBigIntegerSign(string s, int radix, int start = 0)
  {
    try
    {
      return LiteralParser.ParseBigIntegerSign(s, radix, start);
    }
    catch (ArgumentException ex)
    {
      throw PythonOps.ValueError(ex.Message);
    }
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, object x)
  {
    switch (x)
    {
      case string _:
        return BigIntegerOps.ReturnObject(context, cls, (object) BigIntegerOps.ParseBigIntegerSign((string) x, 10));
      case Extensible<string> extensible:
        object obj;
        return PythonTypeOps.TryInvokeUnaryOperator(context, x, "__long__", out obj) ? BigIntegerOps.ReturnObject(context, cls, (object) (BigInteger) obj) : BigIntegerOps.ReturnObject(context, cls, (object) BigIntegerOps.ParseBigIntegerSign(extensible.Value, 10));
      case double self:
        return BigIntegerOps.ReturnObject(context, cls, (object) DoubleOps.__long__(self));
      case int num1:
        return BigIntegerOps.ReturnObject(context, cls, (object) (BigInteger) num1);
      case BigInteger _:
        return BigIntegerOps.ReturnObject(context, cls, x);
      case Complex _:
        throw PythonOps.TypeError("can't convert complex to long; use long(abs(z))");
      case Decimal num2:
        return BigIntegerOps.ReturnObject(context, cls, (object) (BigInteger) num2);
      default:
        object ret;
        if (PythonTypeOps.TryInvokeUnaryOperator(context, x, "__long__", out ret) && ret != NotImplementedType.Value || x is OldInstance && PythonTypeOps.TryInvokeUnaryOperator(context, x, "__int__", out ret) && ret != NotImplementedType.Value)
        {
          switch (ret)
          {
            case int _:
            case BigInteger _:
            case Extensible<int> _:
            case Extensible<BigInteger> _:
              return BigIntegerOps.ReturnObject(context, cls, ret);
            default:
              throw PythonOps.TypeError("__long__ returned non-long (type {0})", (object) PythonTypeOps.GetOldName(ret));
          }
        }
        else
        {
          if (PythonOps.TryGetBoundAttr(context, x, "__trunc__", out ret))
          {
            ret = PythonOps.CallWithContext(context, ret);
            int result1;
            if (Converter.TryConvertToInt32(ret, out result1))
              return BigIntegerOps.ReturnObject(context, cls, (object) (BigInteger) result1);
            BigInteger result2;
            if (Converter.TryConvertToBigInteger(ret, out result2))
              return BigIntegerOps.ReturnObject(context, cls, (object) result2);
            throw PythonOps.TypeError("__trunc__ returned non-Integral (type {0})", (object) PythonTypeOps.GetOldName(ret));
          }
          if (x is OldInstance)
            throw PythonOps.AttributeError("{0} instance has no attribute '__trunc__'", (object) ((OldInstance) x)._class.Name);
          throw PythonOps.TypeError("long() argument must be a string or a number, not '{0}'", (object) DynamicHelpers.GetPythonType(x).Name);
        }
    }
  }

  private static object ReturnObject(CodeContext context, PythonType cls, object value)
  {
    return cls == TypeCache.BigInteger ? value : cls.CreateInstance(context, value);
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls)
  {
    return cls == TypeCache.BigInteger ? (object) BigInteger.Zero : cls.CreateInstance(context, (object) BigInteger.Zero);
  }

  [SpecialName]
  public static object Power(BigInteger x, object y, object z)
  {
    switch (y)
    {
      case int y1:
        return BigIntegerOps.Power(x, y1, z);
      case long y2:
        return BigIntegerOps.Power(x, (BigInteger) y2, z);
      case BigInteger y3:
        return BigIntegerOps.Power(x, y3, z);
      default:
        return (object) NotImplementedType.Value;
    }
  }

  [SpecialName]
  public static object Power(BigInteger x, int y, object z)
  {
    switch (z)
    {
      case int z1:
        return BigIntegerOps.Power(x, y, (BigInteger) z1);
      case long z2:
        return BigIntegerOps.Power(x, y, (BigInteger) z2);
      case BigInteger z3:
        return BigIntegerOps.Power(x, y, z3);
      case null:
        return BigIntegerOps.Power(x, y);
      default:
        return (object) NotImplementedType.Value;
    }
  }

  [SpecialName]
  public static object Power(BigInteger x, BigInteger y, object z)
  {
    switch (z)
    {
      case int z1:
        return BigIntegerOps.Power(x, y, (BigInteger) z1);
      case long z2:
        return BigIntegerOps.Power(x, y, (BigInteger) z2);
      case BigInteger z3:
        return BigIntegerOps.Power(x, y, z3);
      case null:
        return BigIntegerOps.Power(x, y);
      default:
        return (object) NotImplementedType.Value;
    }
  }

  [SpecialName]
  public static object Power(BigInteger x, int y, BigInteger z)
  {
    if (y < 0)
      throw PythonOps.TypeError("power", (object) y, (object) "power must be >= 0");
    if (z == BigInteger.Zero)
      throw PythonOps.ZeroDivisionError();
    BigInteger bigInteger = x.ModPow(y, z);
    if (z < BigInteger.Zero && bigInteger > BigInteger.Zero || z > BigInteger.Zero && bigInteger < BigInteger.Zero)
      bigInteger += z;
    return (object) bigInteger;
  }

  [SpecialName]
  public static object Power(BigInteger x, BigInteger y, BigInteger z)
  {
    if (y < BigInteger.Zero)
      throw PythonOps.TypeError("power", (object) y, (object) "power must be >= 0");
    if (z == BigInteger.Zero)
      throw PythonOps.ZeroDivisionError();
    BigInteger bigInteger = x.ModPow(y, z);
    if (z < BigInteger.Zero && bigInteger > BigInteger.Zero || z > BigInteger.Zero && bigInteger < BigInteger.Zero)
      bigInteger += z;
    return (object) bigInteger;
  }

  [SpecialName]
  public static object Power([NotNull] BigInteger x, int y)
  {
    return y < 0 ? (object) DoubleOps.Power(x.ToFloat64(), (double) y) : (object) x.Power(y);
  }

  [SpecialName]
  public static object Power([NotNull] BigInteger x, long y)
  {
    return y < 0L ? (object) DoubleOps.Power(x.ToFloat64(), (double) y) : (object) x.Power(y);
  }

  [SpecialName]
  public static object Power([NotNull] BigInteger x, [NotNull] BigInteger y)
  {
    int ret1;
    if (y.AsInt32(out ret1))
      return BigIntegerOps.Power(x, ret1);
    long ret2;
    if (y.AsInt64(out ret2))
      return BigIntegerOps.Power(x, ret2);
    if (x == BigInteger.Zero)
    {
      if (y.Sign < 0)
        throw PythonOps.ZeroDivisionError("0.0 cannot be raised to a negative power");
      return (object) BigInteger.Zero;
    }
    return x == BigInteger.One ? (object) BigInteger.One : throw PythonOps.ValueError("Number too big");
  }

  private static BigInteger DivMod(BigInteger x, BigInteger y, out BigInteger r)
  {
    BigInteger remainder;
    BigInteger bigInteger = BigInteger.DivRem(x, y, out remainder);
    if (x >= BigInteger.Zero)
    {
      if (y > BigInteger.Zero)
      {
        r = remainder;
        return bigInteger;
      }
      if (remainder == BigInteger.Zero)
      {
        r = remainder;
        return bigInteger;
      }
      r = remainder + y;
      return bigInteger - BigInteger.One;
    }
    if (y > BigInteger.Zero)
    {
      if (remainder == BigInteger.Zero)
      {
        r = remainder;
        return bigInteger;
      }
      r = remainder + y;
      return bigInteger - BigInteger.One;
    }
    r = remainder;
    return bigInteger;
  }

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger Add(BigInteger x, BigInteger y) => x + y;

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger Subtract(BigInteger x, BigInteger y) => x - y;

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger Multiply(BigInteger x, BigInteger y) => x * y;

  [SpecialName]
  public static BigInteger FloorDivide([NotNull] BigInteger x, [NotNull] BigInteger y)
  {
    return BigIntegerOps.Divide(x, y);
  }

  [SpecialName]
  public static double TrueDivide([NotNull] BigInteger x, [NotNull] BigInteger y)
  {
    if (y == BigInteger.Zero)
      throw new DivideByZeroException();
    double result1;
    double result2;
    if (x.TryToFloat64(out result1) && y.TryToFloat64(out result2))
      return result1 / result2;
    BigInteger remainder;
    if (!BigInteger.DivRem(x, y, out remainder).TryToFloat64(out result1))
      throw PythonOps.OverflowError("long/long too large for a float");
    if (remainder != BigInteger.Zero && (y / remainder).TryToFloat64(out result2) && result2 != 0.0)
      result1 += 1.0 / result2;
    return result1;
  }

  public static BigInteger operator /(BigInteger x, BigInteger y)
  {
    return BigIntegerOps.DivMod(x, y, out BigInteger _);
  }

  public static BigInteger operator %(BigInteger x, BigInteger y)
  {
    BigInteger r;
    BigIntegerOps.DivMod(x, y, out r);
    return r;
  }

  public static BigInteger operator <<(BigInteger x, int y)
  {
    if (y < 0)
      throw PythonOps.ValueError("negative shift count");
    return x << y;
  }

  public static BigInteger operator >>(BigInteger x, int y)
  {
    if (y < 0)
      throw PythonOps.ValueError("negative shift count");
    return x >> y;
  }

  public static BigInteger operator <<(BigInteger x, BigInteger y) => x << (int) y;

  public static BigInteger operator >>(BigInteger x, BigInteger y) => x >> (int) y;

  [SpecialName]
  public static PythonTuple DivMod(BigInteger x, BigInteger y)
  {
    BigInteger r;
    return PythonTuple.MakeTuple((object) BigIntegerOps.DivMod(x, y, out r), (object) r);
  }

  public static object __abs__(BigInteger x) => (object) x.Abs();

  public static bool __nonzero__(BigInteger x) => !x.IsZero();

  [SpecialName]
  public static object Negate(BigInteger x) => (object) -x;

  public static object __pos__(BigInteger x) => (object) x;

  public static object __int__(BigInteger x)
  {
    int ret;
    return x.AsInt32(out ret) ? ScriptingRuntimeHelpers.Int32ToObject(ret) : (object) x;
  }

  public static object __float__(BigInteger self) => (object) self.ToFloat64();

  public static string __oct__(BigInteger x)
  {
    if (x == BigInteger.Zero)
      return "0L";
    return x > 0L ? $"0{x.ToString(8)}L" : $"-0{(-x).ToString(8)}L";
  }

  public static string __hex__(BigInteger x)
  {
    return x < 0L ? $"-0x{(-x).ToString(16 /*0x10*/).ToLower()}L" : $"0x{x.ToString(16 /*0x10*/).ToLower()}L";
  }

  public static object __getnewargs__(CodeContext context, BigInteger self)
  {
    return (object) PythonTuple.MakeTuple(BigIntegerOps.__new__(context, TypeCache.BigInteger, (object) self));
  }

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger OnesComplement(BigInteger x) => ~x;

  internal static BigInteger FloorDivideImpl(BigInteger x, BigInteger y)
  {
    return BigIntegerOps.FloorDivide(x, y);
  }

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger BitwiseAnd(BigInteger x, BigInteger y) => x & y;

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger BitwiseOr(BigInteger x, BigInteger y) => x | y;

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger ExclusiveOr(BigInteger x, BigInteger y) => x ^ y;

  [PropertyMethod]
  [SpecialName]
  public static BigInteger Getreal(BigInteger self) => self;

  [PropertyMethod]
  [SpecialName]
  public static BigInteger Getimag(BigInteger self) => (BigInteger) 0;

  public static BigInteger conjugate(BigInteger self) => self;

  [PropertyMethod]
  [SpecialName]
  public static BigInteger Getnumerator(BigInteger self) => self;

  [PropertyMethod]
  [SpecialName]
  public static BigInteger Getdenominator(BigInteger self) => (BigInteger) 1;

  public static int bit_length(BigInteger self) => MathUtils.BitLength(self);

  public static BigInteger __trunc__(BigInteger self) => self;

  [ImplicitConversionMethod]
  [SpecialName]
  public static double ConvertToDouble(BigInteger self) => self.ToFloat64();

  [ExplicitConversionMethod]
  [SpecialName]
  public static int ConvertToInt32(BigInteger self)
  {
    int ret;
    if (self.AsInt32(out ret))
      return ret;
    throw Converter.CannotConvertOverflow("int", (object) self);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static Complex ConvertToComplex(BigInteger self)
  {
    return MathUtils.MakeReal(BigIntegerOps.ConvertToDouble(self));
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static BigInteger ConvertToBigInteger(bool self)
  {
    return !self ? BigInteger.Zero : BigInteger.One;
  }

  [SpecialName]
  public static int Compare(BigInteger x, BigInteger y) => x.CompareTo(y);

  [SpecialName]
  public static int Compare(BigInteger x, int y)
  {
    int ret;
    if (!x.AsInt32(out ret))
      return BigInteger.Compare(x, (BigInteger) y);
    if (ret == y)
      return 0;
    return ret <= y ? -1 : 1;
  }

  [SpecialName]
  public static int Compare(BigInteger x, uint y)
  {
    uint ret;
    if (!x.AsUInt32(out ret))
      return BigInteger.Compare(x, (BigInteger) y);
    if ((int) ret == (int) y)
      return 0;
    return ret <= y ? -1 : 1;
  }

  [SpecialName]
  public static int Compare(BigInteger x, double y) => -DoubleOps.Compare(y, x);

  [SpecialName]
  public static int Compare(BigInteger x, [NotNull] Extensible<double> y)
  {
    return -DoubleOps.Compare(y.Value, x);
  }

  [SpecialName]
  public static int Compare(BigInteger x, Decimal y) => DecimalOps.__cmp__(x, y);

  [SpecialName]
  public static int Compare(BigInteger x, bool y) => BigIntegerOps.Compare(x, y ? 1 : 0);

  public static BigInteger __long__(BigInteger self) => self;

  public static BigInteger __index__(BigInteger self) => self;

  public static int __hash__(BigInteger self)
  {
    if (self == (long) int.MinValue)
      return int.MinValue;
    long ret1;
    if (self.AsInt64(out ret1))
      return Int64Ops.__hash__(ret1);
    ulong ret2;
    return self.AsUInt64(out ret2) ? UInt64Ops.__hash__(ret2) : self.GetHashCode();
  }

  public static string __repr__([NotNull] BigInteger self) => self.ToString() + "L";

  public static object __coerce__(CodeContext context, BigInteger self, object o)
  {
    BigInteger result;
    if (!Converter.TryConvertToBigInteger(o, out result))
      return (object) NotImplementedType.Value;
    return (object) PythonTuple.MakeTuple((object) self, (object) result);
  }

  [PythonHidden(new PlatformID[] {})]
  public static float ToFloat(BigInteger self) => (float) self.ToFloat64();

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger Xor(BigInteger x, BigInteger y) => x ^ y;

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger Divide(BigInteger x, BigInteger y) => x / y;

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger Mod(BigInteger x, BigInteger y) => x % y;

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger LeftShift(BigInteger x, int y) => x << y;

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger RightShift(BigInteger x, int y) => x >> y;

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger LeftShift(BigInteger x, BigInteger y) => x << y;

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger RightShift(BigInteger x, BigInteger y) => x >> y;

  [PythonHidden(new PlatformID[] {})]
  public static bool AsDecimal(BigInteger self, out Decimal res)
  {
    if (self <= (BigInteger) Decimal.MaxValue && self >= (BigInteger) Decimal.MinValue)
    {
      res = (Decimal) self;
      return true;
    }
    res = 0M;
    return false;
  }

  [PythonHidden(new PlatformID[] {})]
  public static bool AsInt32(BigInteger self, out int res) => self.AsInt32(out res);

  [PythonHidden(new PlatformID[] {})]
  public static bool AsInt64(BigInteger self, out long res) => self.AsInt64(out res);

  [CLSCompliant(false)]
  [PythonHidden(new PlatformID[] {})]
  public static bool AsUInt32(BigInteger self, out uint res) => self.AsUInt32(out res);

  [CLSCompliant(false)]
  [PythonHidden(new PlatformID[] {})]
  public static bool AsUInt64(BigInteger self, out ulong res) => self.AsUInt64(out res);

  [PythonHidden(new PlatformID[] {})]
  public static int ToInt32(BigInteger self) => (int) self;

  [PythonHidden(new PlatformID[] {})]
  public static long ToInt64(BigInteger self) => (long) self;

  [CLSCompliant(false)]
  [PythonHidden(new PlatformID[] {})]
  public static uint ToUInt32(BigInteger self) => (uint) self;

  [CLSCompliant(false)]
  [PythonHidden(new PlatformID[] {})]
  public static ulong ToUInt64(BigInteger self) => (ulong) self;

  [PythonHidden(new PlatformID[] {})]
  public static bool ToBoolean(BigInteger self, IFormatProvider provider) => !self.IsZero;

  [PythonHidden(new PlatformID[] {})]
  public static byte ToByte(BigInteger self, IFormatProvider provider) => (byte) self;

  [CLSCompliant(false)]
  [PythonHidden(new PlatformID[] {})]
  public static sbyte ToSByte(BigInteger self, IFormatProvider provider) => (sbyte) self;

  [PythonHidden(new PlatformID[] {})]
  public static char ToChar(BigInteger self, IFormatProvider provider)
  {
    int ret;
    return self.AsInt32(out ret) && ret <= (int) ushort.MaxValue && ret >= 0 ? (char) ret : throw new OverflowException("big integer won't fit into char");
  }

  [PythonHidden(new PlatformID[] {})]
  public static Decimal ToDecimal(BigInteger self, IFormatProvider provider) => (Decimal) self;

  [PythonHidden(new PlatformID[] {})]
  public static double ToDouble(BigInteger self, IFormatProvider provider)
  {
    return BigIntegerOps.ConvertToDouble(self);
  }

  [PythonHidden(new PlatformID[] {})]
  public static float ToSingle(BigInteger self, IFormatProvider provider)
  {
    return BigIntegerOps.ToFloat(self);
  }

  [PythonHidden(new PlatformID[] {})]
  public static short ToInt16(BigInteger self, IFormatProvider provider) => (short) self;

  [PythonHidden(new PlatformID[] {})]
  public static int ToInt32(BigInteger self, IFormatProvider provider) => (int) self;

  [PythonHidden(new PlatformID[] {})]
  public static long ToInt64(BigInteger self, IFormatProvider provider) => (long) self;

  [CLSCompliant(false)]
  [PythonHidden(new PlatformID[] {})]
  public static ushort ToUInt16(BigInteger self, IFormatProvider provider) => (ushort) self;

  [CLSCompliant(false)]
  [PythonHidden(new PlatformID[] {})]
  public static uint ToUInt32(BigInteger self, IFormatProvider provider) => (uint) self;

  [CLSCompliant(false)]
  [PythonHidden(new PlatformID[] {})]
  public static ulong ToUInt64(BigInteger self, IFormatProvider provider) => (ulong) self;

  [PythonHidden(new PlatformID[] {})]
  public static object ToType(BigInteger self, Type conversionType, IFormatProvider provider)
  {
    if (conversionType == typeof (BigInteger))
      return (object) self;
    throw new NotImplementedException();
  }

  [PythonHidden(new PlatformID[] {})]
  public static TypeCode GetTypeCode(BigInteger self) => TypeCode.Object;

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger Square(BigInteger self) => self * self;

  [PythonHidden(new PlatformID[] {})]
  public static bool IsNegative(BigInteger self) => self.Sign < 0;

  [PythonHidden(new PlatformID[] {})]
  public static bool IsPositive(BigInteger self) => self.Sign > 0;

  [PythonHidden(new PlatformID[] {})]
  public static int GetBitCount(BigInteger self) => self.GetBitCount();

  [PythonHidden(new PlatformID[] {})]
  public static int GetByteCount(BigInteger self) => self.GetByteCount();

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger Create(byte[] v) => new BigInteger(v);

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger Create(int v) => new BigInteger(v);

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger Create(long v) => new BigInteger(v);

  [CLSCompliant(false)]
  [PythonHidden(new PlatformID[] {})]
  public static BigInteger Create(uint v) => new BigInteger(v);

  [CLSCompliant(false)]
  [PythonHidden(new PlatformID[] {})]
  public static BigInteger Create(ulong v) => (BigInteger) v;

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger Create(Decimal v) => new BigInteger(v);

  [PythonHidden(new PlatformID[] {})]
  public static BigInteger Create(double v) => new BigInteger(v);

  [CLSCompliant(false)]
  [PythonHidden(new PlatformID[] {})]
  public static uint[] GetWords(BigInteger self) => self.GetWords();

  [CLSCompliant(false)]
  [PythonHidden(new PlatformID[] {})]
  public static uint GetWord(BigInteger self, int index) => self.GetWord(index);

  [PythonHidden(new PlatformID[] {})]
  public static int GetWordCount(BigInteger self) => self.GetWordCount();

  public static string __format__(CodeContext context, BigInteger self, [NotNull] string formatSpec)
  {
    StringFormatSpec spec = StringFormatSpec.FromString(formatSpec);
    if (spec.Precision.HasValue)
      throw PythonOps.ValueError("Precision not allowed in integer format specifier");
    BigInteger val = self;
    if (self < 0L)
      val = -self;
    char? type = spec.Type;
    string text;
    if (type.HasValue)
    {
      switch (type.GetValueOrDefault())
      {
        case '%':
          text = !spec.ThousandsComma ? val.ToString("0.000000%", (IFormatProvider) CultureInfo.InvariantCulture) : val.ToString("#,0.000000%", (IFormatProvider) CultureInfo.InvariantCulture);
          goto label_35;
        case 'E':
          text = !spec.ThousandsComma ? val.ToString("0.000000E+00", (IFormatProvider) CultureInfo.InvariantCulture) : val.ToString("#,0.000000E+00", (IFormatProvider) CultureInfo.InvariantCulture);
          goto label_35;
        case 'F':
        case 'f':
          text = !spec.ThousandsComma ? val.ToString("#########0.000000", (IFormatProvider) CultureInfo.InvariantCulture) : val.ToString("#,########0.000000", (IFormatProvider) CultureInfo.InvariantCulture);
          goto label_35;
        case 'G':
          if (val >= 1000000L)
          {
            text = val.ToString("0.#####E+00", (IFormatProvider) CultureInfo.InvariantCulture);
            goto label_35;
          }
          if (!spec.ThousandsComma)
          {
            text = val.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            goto label_35;
          }
          break;
        case 'X':
          text = BigIntegerOps.AbsToHex(val, false);
          goto label_35;
        case 'b':
          text = BigIntegerOps.ToBinary(val, false, true);
          goto label_35;
        case 'c':
          if (spec.Sign.HasValue)
            throw PythonOps.ValueError("Sign not allowed with integer format specifier 'c'");
          int ret;
          if (!self.AsInt32(out ret))
            throw PythonOps.OverflowError("long int too large to convert to int");
          text = ret >= 0 && ret <= (int) byte.MaxValue ? ScriptingRuntimeHelpers.CharToString((char) ret) : throw PythonOps.OverflowError("%c arg not in range(0x10000)");
          goto label_35;
        case 'd':
          break;
        case 'e':
          text = !spec.ThousandsComma ? val.ToString("0.000000e+00", (IFormatProvider) CultureInfo.InvariantCulture) : val.ToString("#,0.000000e+00", (IFormatProvider) CultureInfo.InvariantCulture);
          goto label_35;
        case 'g':
          if (val >= 1000000L)
          {
            text = val.ToString("0.#####e+00", (IFormatProvider) CultureInfo.InvariantCulture);
            goto label_35;
          }
          if (!spec.ThousandsComma)
          {
            text = val.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            goto label_35;
          }
          break;
        case 'n':
          if (context.LanguageContext.NumericCulture != CultureInfo.InvariantCulture)
          {
            text = FormattingHelper.ToCultureString<BigInteger>(val, context.LanguageContext.NumericCulture.NumberFormat, spec);
            goto label_35;
          }
          break;
        case 'o':
          text = BigIntegerOps.ToOctal(val, true);
          goto label_35;
        case 'x':
          text = BigIntegerOps.AbsToHex(val, true);
          goto label_35;
        default:
          throw PythonOps.ValueError("Unknown format code '{0}'", (object) spec.Type.ToString());
      }
    }
    if (spec.ThousandsComma)
    {
      int num = spec.Width ?? 0;
      text = !spec.Fill.HasValue || spec.Fill.Value != '0' || num <= 1 ? val.ToString("#,0", (IFormatProvider) CultureInfo.InvariantCulture) : val.ToString(FormattingHelper.ToCultureString<BigInteger>(self, FormattingHelper.InvariantCommaNumberInfo, spec));
    }
    else
      text = val.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
label_35:
    return spec.AlignNumericText(text, self.IsZero(), self.IsPositive());
  }

  internal static string AbsToHex(BigInteger val, bool lowercase)
  {
    return BigIntegerOps.ToDigits(val, 16 /*0x10*/, lowercase);
  }

  private static string ToOctal(BigInteger val, bool lowercase)
  {
    return BigIntegerOps.ToDigits(val, 8, lowercase);
  }

  internal static string ToBinary(BigInteger val)
  {
    string binary = BigIntegerOps.ToBinary(val.Abs(), true, true);
    if (val.IsNegative())
      binary = "-" + binary;
    return binary;
  }

  private static string ToBinary(BigInteger val, bool includeType, bool lowercase)
  {
    string binary = BigIntegerOps.ToDigits(val, 2, lowercase);
    if (includeType)
      binary = (lowercase ? "0b" : "0B") + binary;
    return binary;
  }

  private static string ToExponent(
    BigInteger self,
    bool lower,
    int minPrecision,
    int maxPrecision)
  {
    string str = self.ToString();
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(str[0]);
    for (int index = 1; index < maxPrecision && index < str.Length; ++index)
    {
      if (str[index] != '0' || index <= minPrecision)
      {
        if (stringBuilder.Length == 1)
          stringBuilder.Append('.');
        while (index > stringBuilder.Length - 1)
          stringBuilder.Append('0');
        if (index == maxPrecision - 1 && index != str.Length - 1 && str[index + 1] >= '5')
          stringBuilder.Append((char) ((uint) str[index] + 1U));
        else
          stringBuilder.Append(str[index]);
      }
    }
    if (str.Length <= minPrecision)
    {
      if (stringBuilder.Length == 1)
        stringBuilder.Append('.');
      while (minPrecision >= stringBuilder.Length - 1)
        stringBuilder.Append('0');
    }
    stringBuilder.Append(lower ? "e+" : "E+");
    int num = str.Length - 1;
    if (num < 10)
    {
      stringBuilder.Append('0');
      stringBuilder.Append((char) (48 /*0x30*/ + num));
    }
    else
      stringBuilder.Append(num.ToString());
    return stringBuilder.ToString();
  }

  private static string ToDigits(BigInteger val, int radix, bool lower)
  {
    if (val.IsZero())
      return "0";
    StringBuilder stringBuilder1 = new StringBuilder();
    while (val != 0L)
    {
      int num = (int) (val % (BigInteger) radix);
      if (num < 10)
        stringBuilder1.Append((char) (num + 48 /*0x30*/));
      else if (lower)
        stringBuilder1.Append((char) (num - 10 + 97));
      else
        stringBuilder1.Append((char) (num - 10 + 65));
      val /= (BigInteger) radix;
    }
    StringBuilder stringBuilder2 = new StringBuilder(stringBuilder1.Length);
    for (int index = stringBuilder1.Length - 1; index >= 0; --index)
      stringBuilder2.Append(stringBuilder1[index]);
    return stringBuilder2.ToString();
  }
}
