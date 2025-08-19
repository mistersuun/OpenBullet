// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.MathUtils
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.Utils;

public static class MathUtils
{
  private static readonly double[] _RoundPowersOfTens = new double[16 /*0x10*/]
  {
    1.0,
    10.0,
    100.0,
    1000.0,
    10000.0,
    100000.0,
    1000000.0,
    10000000.0,
    100000000.0,
    1000000000.0,
    10000000000.0,
    100000000000.0,
    1000000000000.0,
    10000000000000.0,
    100000000000000.0,
    1E+15
  };
  private static readonly double[] ErfNumerCoeffs = new double[4]
  {
    242.66795523053176,
    21.979261618294153,
    6.9963834886191352,
    -0.035609843701815386
  };
  private static readonly double[] ErfDenomCoeffs = new double[4]
  {
    215.0588758698612,
    91.1649054045149,
    15.082797630407788,
    1.0
  };
  private static readonly double[] ErfcNumerCoeffs = new double[8]
  {
    300.45926102016159,
    451.91895371187292,
    339.32081673434368,
    152.98928504694041,
    43.162227222056735,
    7.2117582508830935,
    0.564195517478974,
    -1.3686485738271671E-07
  };
  private static readonly double[] ErfcDenomCoeffs = new double[8]
  {
    300.45926095698331,
    790.95092532789806,
    931.35409485060961,
    638.98026446563119,
    277.58544474398764,
    77.00015293522948,
    12.782727319629423,
    1.0
  };
  private static readonly double[] GammaNumerCoeffs = new double[13]
  {
    44012138428004.609,
    41590453358593.2,
    18013842787117.996,
    4728736263475.3887,
    837910083628.40466,
    105583707273.42993,
    9701363618.4949989,
    654914397.54820526,
    32238322.942133565,
    1128514.2194970914,
    26665.793784598591,
    381.88012486329268,
    2.5066282746310007
  };
  private static readonly double[] GammaDenomCoeffs = new double[13]
  {
    0.0,
    39916800.0,
    120543840.0,
    150917976.0,
    105258076.0,
    45995730.0,
    13339535.0,
    2637558.0,
    357423.0,
    32670.0,
    1925.0,
    66.0,
    1.0
  };
  private static readonly uint[] maxCharsPerDigit = new uint[37]
  {
    0U,
    0U,
    31U /*0x1F*/,
    20U,
    15U,
    13U,
    12U,
    11U,
    10U,
    10U,
    9U,
    9U,
    8U,
    8U,
    8U,
    8U,
    7U,
    7U,
    7U,
    7U,
    7U,
    7U,
    7U,
    7U,
    6U,
    6U,
    6U,
    6U,
    6U,
    6U,
    6U,
    6U,
    6U,
    6U,
    6U,
    6U,
    6U
  };
  private static readonly uint[] groupRadixValues = new uint[37]
  {
    0U,
    0U,
    2147483648U /*0x80000000*/,
    3486784401U,
    1073741824U /*0x40000000*/,
    1220703125U,
    2176782336U,
    1977326743U,
    1073741824U /*0x40000000*/,
    3486784401U,
    1000000000U,
    2357947691U,
    429981696U,
    815730721U,
    1475789056U,
    2562890625U,
    268435456U /*0x10000000*/,
    410338673U,
    612220032U,
    893871739U,
    1280000000U,
    1801088541U,
    2494357888U,
    3404825447U,
    191102976U /*0x0B640000*/,
    244140625U,
    308915776U,
    387420489U,
    481890304U,
    594823321U,
    729000000U,
    887503681U,
    1073741824U /*0x40000000*/,
    1291467969U,
    1544804416U,
    1838265625U,
    2176782336U
  };
  private const int BitsPerDigit = 32 /*0x20*/;

  public static int FloorDivideUnchecked(int x, int y)
  {
    int num = x / y;
    return x >= 0 ? (y > 0 || x % y == 0 ? num : num - 1) : (y > 0 && x % y != 0 ? num - 1 : num);
  }

  public static long FloorDivideUnchecked(long x, long y)
  {
    long num = x / y;
    return x >= 0L ? (y > 0L || x % y == 0L ? num : num - 1L) : (y > 0L && x % y != 0L ? num - 1L : num);
  }

  public static int FloorRemainder(int x, int y)
  {
    if (y == -1)
      return 0;
    int num = x % y;
    if (x >= 0)
    {
      if (y > 0)
        return num;
      return num == 0 ? 0 : num + y;
    }
    if (y <= 0)
      return num;
    return num == 0 ? 0 : num + y;
  }

  public static long FloorRemainder(long x, long y)
  {
    if (y == -1L)
      return 0;
    long num = x % y;
    if (x >= 0L)
    {
      if (y > 0L)
        return num;
      return num == 0L ? 0L : num + y;
    }
    if (y <= 0L)
      return num;
    return num == 0L ? 0L : num + y;
  }

  public static double RoundAwayFromZero(double value)
  {
    return Math.Round(value, MidpointRounding.AwayFromZero);
  }

  private static double GetPowerOf10(int precision)
  {
    return precision >= 16 /*0x10*/ ? Math.Pow(10.0, (double) precision) : MathUtils._RoundPowersOfTens[precision];
  }

  public static double RoundAwayFromZero(double value, int precision)
  {
    if (double.IsInfinity(value) || double.IsNaN(value))
      return value;
    if (precision >= 0)
    {
      if (precision > 308)
        return value;
      double powerOf10 = MathUtils.GetPowerOf10(precision);
      return MathUtils.RoundAwayFromZero(value * powerOf10) / powerOf10;
    }
    if (precision >= -308)
    {
      double powerOf10 = MathUtils.GetPowerOf10(-precision);
      return MathUtils.RoundAwayFromZero(value / powerOf10) * powerOf10;
    }
    return value >= 0.0 && 1.0 / value >= 0.0 ? 0.0 : -0.0;
  }

  public static bool IsNegativeZero(double self) => self == 0.0 && 1.0 / self < 0.0;

  public static double Erf(double v0)
  {
    if (v0 >= 10.0)
      return 1.0;
    if (v0 <= -10.0)
      return -1.0;
    if (v0 > 0.47 || v0 < -0.47)
      return 1.0 - MathUtils.ErfComplement(v0);
    double v0_1 = v0 * v0;
    double num1 = MathUtils.EvalPolynomial(v0_1, MathUtils.ErfNumerCoeffs);
    double num2 = MathUtils.EvalPolynomial(v0_1, MathUtils.ErfDenomCoeffs);
    return v0 * num1 / num2;
  }

  public static double ErfComplement(double v0)
  {
    if (v0 >= 30.0)
      return 0.0;
    if (v0 <= -10.0)
      return 2.0;
    double v0_1 = Math.Abs(v0);
    if (v0_1 < 0.47)
      return 1.0 - MathUtils.Erf(v0);
    double num1;
    if (v0_1 <= 4.0)
    {
      double num2 = MathUtils.EvalPolynomial(v0_1, MathUtils.ErfcNumerCoeffs);
      double num3 = MathUtils.EvalPolynomial(v0_1, MathUtils.ErfcDenomCoeffs);
      num1 = Math.Exp(-v0_1 * v0_1) * num2 / num3;
    }
    else
    {
      double num4 = v0_1 * v0_1;
      double num5 = 0.0;
      for (int index = 10; index > 0; --index)
      {
        double num6 = (double) (index * index) * 0.25;
        num5 += Math.Exp(-num6) / (num6 + num4);
      }
      num1 = 0.5 * v0_1 * Math.Exp(-num4) / Math.PI * (num5 * 2.0 + 1.0 / num4);
    }
    if (v0 < 0.0)
      num1 = 2.0 - num1;
    return num1;
  }

  public static double Gamma(double v0)
  {
    if (double.IsNegativeInfinity(v0))
      return double.NaN;
    double num1 = Math.Abs(v0);
    if (num1 % 1.0 == 0.0)
    {
      if (v0 <= 0.0)
        return double.NaN;
      if (num1 <= 25.0)
      {
        if (num1 <= 2.0)
          return 1.0;
        double num2 = num1 - 1.0;
        --v0;
        while (--v0 > 1.0)
          num2 *= v0;
        return num2;
      }
    }
    if (num1 < 1E-50)
      return 1.0 / v0;
    double num3;
    if (v0 < -150.0)
    {
      double num4 = v0 / 2.0;
      num3 = Math.Pow(Math.PI, 1.5) / MathUtils.SinPi(v0) * Math.Pow(2.0, v0) / MathUtils.PositiveGamma(0.5 - num4) / MathUtils.PositiveGamma(1.0 - num4);
    }
    else if (v0 < 0.001)
    {
      double num5 = Math.PI / MathUtils.SinPi(v0);
      double v0_1 = 1.0 - v0;
      num3 = v0 != 1.0 - v0_1 ? num5 / (-v0 * MathUtils.PositiveGamma(-v0)) : num5 / MathUtils.PositiveGamma(v0_1);
    }
    else
      num3 = MathUtils.PositiveGamma(v0);
    return num3;
  }

  public static double LogGamma(double v0)
  {
    if (double.IsInfinity(v0))
      return double.PositiveInfinity;
    double d = Math.Abs(v0);
    if (v0 <= 0.0 && d % 1.0 == 0.0)
      return double.NaN;
    return d < 1E-50 ? -Math.Log(d) : (v0 >= 0.0 ? MathUtils.PositiveLGamma(v0) : Math.Log(Math.PI / MathUtils.AbsSinPi(v0)) - MathUtils.PositiveLGamma(1.0 - v0));
  }

  public static double Hypot(double x, double y)
  {
    if (double.IsInfinity(x) || double.IsInfinity(y))
      return double.PositiveInfinity;
    if (x < 0.0)
      x = -x;
    if (y < 0.0)
      y = -y;
    if (x == 0.0)
      return y;
    if (y == 0.0)
      return x;
    if (x < y)
    {
      double num = y;
      y = x;
      x = num;
    }
    y /= x;
    return x * Math.Sqrt(1.0 + y * y);
  }

  private static double EvalPolynomial(double v0, double[] coeffs)
  {
    double num = 0.0;
    for (int index = coeffs.Length - 1; index >= 0; --index)
      num = num * v0 + coeffs[index];
    return num;
  }

  private static double EvalPolynomial(double v0, double[] coeffs, bool reverse)
  {
    if (!reverse)
      return MathUtils.EvalPolynomial(v0, coeffs);
    double num = 0.0;
    for (int index = 0; index < coeffs.Length; ++index)
      num = num * v0 + coeffs[index];
    return num;
  }

  private static double SinPi(double v0)
  {
    double num1 = Math.Abs(v0) % 2.0;
    double num2 = num1 >= 0.25 ? (num1 >= 0.75 ? (num1 >= 1.25 ? (num1 >= 1.75 ? Math.Sin((num1 - 2.0) * Math.PI) : -Math.Cos((num1 - 1.5) * Math.PI)) : -Math.Sin((num1 - 1.0) * Math.PI)) : Math.Cos((num1 - 0.5) * Math.PI)) : Math.Sin(num1 * Math.PI);
    return v0 >= 0.0 ? num2 : -num2;
  }

  private static double AbsSinPi(double v0)
  {
    double num = Math.Abs(v0) % 1.0;
    return Math.Abs(num >= 0.25 ? (num >= 0.75 ? Math.Sin((num - 1.0) * Math.PI) : Math.Cos((num - 0.5) * Math.PI)) : Math.Sin(num * Math.PI));
  }

  private static double GammaRationalFunc(double v0)
  {
    double num1;
    double num2;
    if (v0 < 1E+15)
    {
      num1 = MathUtils.EvalPolynomial(v0, MathUtils.GammaNumerCoeffs);
      num2 = MathUtils.EvalPolynomial(v0, MathUtils.GammaDenomCoeffs);
    }
    else
    {
      double v0_1 = 1.0 / v0;
      num1 = MathUtils.EvalPolynomial(v0_1, MathUtils.GammaNumerCoeffs, true);
      num2 = MathUtils.EvalPolynomial(v0_1, MathUtils.GammaDenomCoeffs, true);
    }
    return num1 / num2;
  }

  private static double PositiveGamma(double v0)
  {
    if (v0 > 200.0)
      return double.PositiveInfinity;
    double num1 = v0 + 12.644565;
    double num2 = MathUtils.GammaRationalFunc(v0) / Math.Exp(num1);
    double num3;
    if (v0 < 120.0)
    {
      num3 = num2 * Math.Pow(num1, v0 - 0.5);
    }
    else
    {
      double num4 = Math.Pow(num1, v0 / 2.0 - 0.25);
      num3 = num2 * num4 * num4;
    }
    return num3;
  }

  private static double PositiveLGamma(double v0)
  {
    double d = v0 + 13.144565 - 0.5;
    return Math.Log(MathUtils.GammaRationalFunc(v0)) - 13.144565 + (v0 - 0.5) * (Math.Log(d) - 1.0);
  }

  internal static string BigIntegerToString(uint[] d, int sign, int radix, bool lowerCase)
  {
    if (radix < 2 || radix > 36)
      throw ExceptionUtils.MakeArgumentOutOfRangeException(nameof (radix), (object) radix, "radix must be between 2 and 36");
    int length = d.Length;
    if (length == 0)
      return "0";
    List<uint> uintList1 = new List<uint>();
    uint groupRadixValue = MathUtils.groupRadixValues[radix];
    while (length > 0)
    {
      uint num = MathUtils.div(d, ref length, groupRadixValue);
      uintList1.Add(num);
    }
    StringBuilder buf = new StringBuilder();
    if (sign == -1)
      buf.Append("-");
    int num1 = uintList1.Count - 1;
    char[] tmp = new char[(int) MathUtils.maxCharsPerDigit[radix]];
    List<uint> uintList2 = uintList1;
    int index = num1;
    int num2 = index - 1;
    MathUtils.AppendRadix(uintList2[index], (uint) radix, tmp, buf, false, lowerCase);
    while (num2 >= 0)
      MathUtils.AppendRadix(uintList1[num2--], (uint) radix, tmp, buf, true, lowerCase);
    return buf.Length != 0 ? buf.ToString() : "0";
  }

  private static uint div(uint[] n, ref int nl, uint d)
  {
    ulong num1 = 0;
    int index = nl;
    bool flag = false;
    while (--index >= 0)
    {
      ulong num2 = num1 << 32 /*0x20*/ | (ulong) n[index];
      uint num3 = (uint) (num2 / (ulong) d);
      n[index] = num3;
      if (num3 == 0U)
      {
        if (!flag)
          --nl;
      }
      else
        flag = true;
      num1 = num2 % (ulong) d;
    }
    return (uint) num1;
  }

  private static void AppendRadix(
    uint rem,
    uint radix,
    char[] tmp,
    StringBuilder buf,
    bool leadingZeros,
    bool lowerCase)
  {
    string str = lowerCase ? "0123456789abcdefghijklmnopqrstuvwxyz" : "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    int length = tmp.Length;
    int startIndex;
    uint index;
    for (startIndex = length; startIndex > 0 && (leadingZeros || rem != 0U); tmp[--startIndex] = str[(int) index])
    {
      index = rem % radix;
      rem /= radix;
    }
    if (leadingZeros)
      buf.Append(tmp);
    else
      buf.Append(tmp, startIndex, length - startIndex);
  }

  private static uint GetWord(byte[] bytes, int start, int end)
  {
    uint word = 0;
    int num1 = end - start;
    int num2 = 0;
    if (num1 > 32 /*0x20*/)
      num1 = 32 /*0x20*/;
    start /= 8;
    while (num1 > 0)
    {
      uint num3 = (uint) bytes[start];
      if (num1 < 8)
        num3 &= (uint) ((1 << num1) - 1);
      uint num4 = num3 << num2;
      word |= num4;
      num1 -= 8;
      num2 += 8;
      ++start;
    }
    return word;
  }

  public static BigInteger GetRandBits(Action<byte[]> NextBytes, int bits)
  {
    ContractUtils.Requires(bits > 0, nameof (bits));
    int num = bits % 8 == 0 ? bits / 8 : bits / 8 + 1;
    byte[] bytes = new byte[bits % 8 == 0 ? num + 1 : num];
    NextBytes(bytes);
    bytes[bytes.Length - 1] = bits % 8 != 0 ? (byte) ((uint) bytes[bytes.Length - 1] & (uint) ((1 << bits % 8) - 1)) : (byte) 0;
    if (bits <= 32 /*0x20*/)
      return (BigInteger) MathUtils.GetWord(bytes, 0, bits);
    return bits <= 64 /*0x40*/ ? (BigInteger) ((ulong) MathUtils.GetWord(bytes, 0, bits) | (ulong) MathUtils.GetWord(bytes, 32 /*0x20*/, bits) << 32 /*0x20*/) : new BigInteger(bytes);
  }

  public static BigInteger GetRandBits(this System.Random generator, int bits)
  {
    return MathUtils.GetRandBits(new Action<byte[]>(generator.NextBytes), bits);
  }

  public static BigInteger Random(this System.Random generator, BigInteger limit)
  {
    ContractUtils.Requires(limit.Sign > 0, nameof (limit));
    ContractUtils.RequiresNotNull((object) generator, nameof (generator));
    BigInteger bigInteger = BigInteger.Zero;
    byte[] byteArray;
    for (; !(limit == BigInteger.Zero); limit = new BigInteger(byteArray))
    {
      int ret;
      if (limit.AsInt32(out ret))
        return bigInteger + (BigInteger) generator.Next(ret);
      byteArray = limit.ToByteArray();
      int length = byteArray.Length;
      do
        ;
      while (byteArray[--length] == (byte) 0);
      int num1;
      if (byteArray[length] < (byte) 128 /*0x80*/)
      {
        num1 = (int) byteArray[length] << 24;
        byteArray[length--] = (byte) 0;
      }
      else
        num1 = 0;
      int num2 = num1 | (int) byteArray[length] << 16 /*0x10*/;
      byte[] numArray1 = byteArray;
      int index1 = length;
      int index2 = index1 - 1;
      numArray1[index1] = (byte) 0;
      int num3 = num2 | (int) byteArray[index2] << 8;
      byte[] numArray2 = byteArray;
      int index3 = index2;
      int index4 = index3 - 1;
      numArray2[index3] = (byte) 0;
      int maxValue = num3 | (int) byteArray[index4];
      byte[] numArray3 = byteArray;
      int index5 = index4;
      int num4 = index5 - 1;
      numArray3[index5] = (byte) 0;
      byte[] buffer = new byte[num4 + 2];
      generator.NextBytes(buffer);
      buffer[num4 + 1] = (byte) 0;
      bigInteger = bigInteger + new BigInteger(buffer) + ((BigInteger) generator.Next(maxValue) << (num4 + 1) * 8);
    }
    return bigInteger;
  }

  public static bool TryToFloat64(this BigInteger self, out double result)
  {
    return StringUtils.TryParseDouble(self.ToString(), NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out result);
  }

  public static double ToFloat64(this BigInteger self)
  {
    return double.Parse(self.ToString(), NumberStyles.Number, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat);
  }

  public static int BitLength(BigInteger x)
  {
    if (x.IsZero)
      return 0;
    byte[] byteArray = BigInteger.Abs(x).ToByteArray();
    int length = byteArray.Length;
    do
      ;
    while (byteArray[--length] == (byte) 0);
    return length * 8 + MathUtils.BitLength((int) byteArray[length]);
  }

  public static int BitLength(long x)
  {
    if (x == 0L)
      return 0;
    if (x == long.MinValue)
      return 64 /*0x40*/;
    x = Math.Abs(x);
    int num = 1;
    if (x >= 4294967296L /*0x0100000000*/)
    {
      x >>= 32 /*0x20*/;
      num += 32 /*0x20*/;
    }
    if (x >= 65536L /*0x010000*/)
    {
      x >>= 16 /*0x10*/;
      num += 16 /*0x10*/;
    }
    if (x >= 256L /*0x0100*/)
    {
      x >>= 8;
      num += 8;
    }
    if (x >= 16L /*0x10*/)
    {
      x >>= 4;
      num += 4;
    }
    if (x >= 4L)
    {
      x >>= 2;
      num += 2;
    }
    if (x >= 2L)
      ++num;
    return num;
  }

  [CLSCompliant(false)]
  public static int BitLengthUnsigned(ulong x)
  {
    return x >= 9223372036854775808UL /*0x8000000000000000*/ ? 64 /*0x40*/ : MathUtils.BitLength((long) x);
  }

  public static int BitLength(int x)
  {
    if (x == 0)
      return 0;
    if (x == int.MinValue)
      return 32 /*0x20*/;
    x = Math.Abs(x);
    int num = 1;
    if (x >= 65536 /*0x010000*/)
    {
      x >>= 16 /*0x10*/;
      num += 16 /*0x10*/;
    }
    if (x >= 256 /*0x0100*/)
    {
      x >>= 8;
      num += 8;
    }
    if (x >= 16 /*0x10*/)
    {
      x >>= 4;
      num += 4;
    }
    if (x >= 4)
    {
      x >>= 2;
      num += 2;
    }
    if (x >= 2)
      ++num;
    return num;
  }

  [CLSCompliant(false)]
  public static int BitLengthUnsigned(uint x)
  {
    return x >= 2147483648U /*0x80000000*/ ? 32 /*0x20*/ : MathUtils.BitLength((int) x);
  }

  public static bool AsInt32(this BigInteger self, out int ret)
  {
    if (self >= (long) int.MinValue && self <= (long) int.MaxValue)
    {
      ret = (int) self;
      return true;
    }
    ret = 0;
    return false;
  }

  public static bool AsInt64(this BigInteger self, out long ret)
  {
    if (self >= long.MinValue && self <= long.MaxValue)
    {
      ret = (long) self;
      return true;
    }
    ret = 0L;
    return false;
  }

  [CLSCompliant(false)]
  public static bool AsUInt32(this BigInteger self, out uint ret)
  {
    if (self >= 0L && self <= (long) uint.MaxValue)
    {
      ret = (uint) self;
      return true;
    }
    ret = 0U;
    return false;
  }

  [CLSCompliant(false)]
  public static bool AsUInt64(this BigInteger self, out ulong ret)
  {
    if (self >= 0UL && self <= ulong.MaxValue)
    {
      ret = (ulong) self;
      return true;
    }
    ret = 0UL;
    return false;
  }

  public static BigInteger Abs(this BigInteger self) => BigInteger.Abs(self);

  public static bool IsZero(this BigInteger self) => self.IsZero;

  public static bool IsPositive(this BigInteger self) => self.Sign > 0;

  public static bool IsNegative(this BigInteger self) => self.Sign < 0;

  public static double Log(this BigInteger self) => BigInteger.Log(self);

  public static double Log(this BigInteger self, double baseValue)
  {
    return BigInteger.Log(self, baseValue);
  }

  public static double Log10(this BigInteger self) => BigInteger.Log10(self);

  public static BigInteger Power(this BigInteger self, int exp) => BigInteger.Pow(self, exp);

  public static BigInteger Power(this BigInteger self, long exp)
  {
    if (exp < 0L)
      throw ExceptionUtils.MakeArgumentOutOfRangeException(nameof (exp), (object) exp, "Must be at least 0");
    if (exp <= (long) int.MaxValue)
      return BigInteger.Pow(self, (int) exp);
    if (self.IsOne)
      return BigInteger.One;
    if (self.IsZero)
      return BigInteger.Zero;
    if (self == BigInteger.MinusOne)
      return exp % 2L == 0L ? BigInteger.One : BigInteger.MinusOne;
    BigInteger one = BigInteger.One;
    while (exp != 0L)
    {
      if (exp % 2L != 0L)
        one *= self;
      exp >>= 1;
      self *= self;
    }
    return one;
  }

  public static BigInteger ModPow(this BigInteger self, int power, BigInteger mod)
  {
    return BigInteger.ModPow(self, (BigInteger) power, mod);
  }

  public static BigInteger ModPow(this BigInteger self, BigInteger power, BigInteger mod)
  {
    return BigInteger.ModPow(self, power, mod);
  }

  public static string ToString(this BigInteger self, int radix)
  {
    if (radix < 2 || radix > 36)
      throw ExceptionUtils.MakeArgumentOutOfRangeException(nameof (radix), (object) radix, "radix must be between 2 and 36");
    bool flag = false;
    if (self < BigInteger.Zero)
    {
      self = -self;
      flag = true;
    }
    else if (self == BigInteger.Zero)
      return "0";
    List<char> charList = new List<char>();
    while (self > 0L)
    {
      charList.Add("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"[(int) (self % (BigInteger) radix)]);
      self /= (BigInteger) radix;
    }
    StringBuilder stringBuilder = new StringBuilder();
    if (flag)
      stringBuilder.Append('-');
    for (int index = charList.Count - 1; index >= 0; --index)
      stringBuilder.Append(charList[index]);
    return stringBuilder.ToString();
  }

  [CLSCompliant(false)]
  public static uint[] GetWords(this BigInteger self)
  {
    if (self.IsZero)
      return new uint[1];
    int index1;
    byte[] byteArray;
    int highestByte = (int) MathUtils.GetHighestByte(self, out index1, out byteArray);
    uint[] words = new uint[(index1 + 1 + 3) / 4];
    int num1 = 0;
    int index2 = 0;
    uint num2 = 0;
    int num3 = 0;
    while (num1 < byteArray.Length)
    {
      num2 |= (uint) byteArray[num1++] << num3;
      if (num1 % 4 == 0)
      {
        words[index2++] = num2;
        num2 = 0U;
      }
      num3 += 8;
    }
    if (num2 != 0U)
      words[index2] = num2;
    return words;
  }

  [CLSCompliant(false)]
  public static uint GetWord(this BigInteger self, int index) => self.GetWords()[index];

  public static int GetWordCount(this BigInteger self)
  {
    int index;
    int highestByte = (int) MathUtils.GetHighestByte(self, out index, out byte[] _);
    return index / 4 + 1;
  }

  public static int GetByteCount(this BigInteger self)
  {
    int index;
    int highestByte = (int) MathUtils.GetHighestByte(self, out index, out byte[] _);
    return index + 1;
  }

  public static int GetBitCount(this BigInteger self)
  {
    if (self.IsZero)
      return 1;
    byte[] byteArray = BigInteger.Abs(self).ToByteArray();
    int length = byteArray.Length;
    do
      ;
    while (byteArray[--length] == (byte) 0);
    int bitCount = length * 8;
    for (int index = (int) byteArray[length]; index > 0; index >>= 1)
      ++bitCount;
    return bitCount;
  }

  private static byte GetHighestByte(BigInteger self, out int index, out byte[] byteArray)
  {
    byte[] byteArray1 = BigInteger.Abs(self).ToByteArray();
    if (self.IsZero)
    {
      byteArray = byteArray1;
      index = 0;
      return 1;
    }
    int length = byteArray1.Length;
    byte highestByte;
    do
    {
      highestByte = byteArray1[--length];
    }
    while (highestByte == (byte) 0);
    index = length;
    byteArray = byteArray1;
    return highestByte;
  }

  public static Complex MakeReal(double real) => new Complex(real, 0.0);

  public static Complex MakeImaginary(double imag) => new Complex(0.0, imag);

  public static Complex MakeComplex(double real, double imag) => new Complex(real, imag);

  public static double Imaginary(this Complex self) => self.Imaginary;

  public static bool IsZero(this Complex self) => self.Equals(Complex.Zero);

  public static Complex Conjugate(this Complex self) => new Complex(self.Real, -self.Imaginary);

  public static double Abs(this Complex self) => Complex.Abs(self);

  public static Complex Pow(this Complex self, Complex power) => Complex.Pow(self, power);
}
