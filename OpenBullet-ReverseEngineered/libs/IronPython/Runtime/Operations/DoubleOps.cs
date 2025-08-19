// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.DoubleOps
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
using System.Text.RegularExpressions;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class DoubleOps
{
  private static Regex _fromHexRegex;
  private static char[] _whitespace = new char[6]
  {
    ' ',
    '\t',
    '\n',
    '\f',
    '\v',
    '\r'
  };
  internal const double PositiveZero = 0.0;
  internal const double NegativeZero = -0.0;

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls)
  {
    return cls == TypeCache.Double ? (object) 0.0 : cls.CreateInstance(context);
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, object x)
  {
    object obj = (object) null;
    switch (x)
    {
      case string _:
        obj = (object) DoubleOps.ParseFloat((string) x);
        break;
      case Extensible<string> _:
        if (!PythonTypeOps.TryInvokeUnaryOperator(context, x, "__float__", out obj))
        {
          obj = (object) DoubleOps.ParseFloat(((Extensible<string>) x).Value);
          break;
        }
        break;
      case char ch:
        obj = (object) DoubleOps.ParseFloat(ScriptingRuntimeHelpers.CharToString(ch));
        break;
      case Complex _:
        throw PythonOps.TypeError("can't convert complex to float; use abs(z)");
      default:
        object o = PythonOps.CallWithContext(context, PythonOps.GetBoundAttr(context, x, "__float__"));
        switch (o)
        {
          case double _:
            obj = o;
            break;
          case Extensible<double> _:
            obj = (object) ((Extensible<double>) o).Value;
            break;
          default:
            throw PythonOps.TypeError("__float__ returned non-float (type {0})", (object) PythonTypeOps.GetName(o));
        }
        break;
    }
    return cls == TypeCache.Double ? obj : cls.CreateInstance(context, obj);
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, IList<byte> s)
  {
    object obj;
    if (!(s is IPythonObject o) || !PythonTypeOps.TryInvokeUnaryOperator(DefaultContext.Default, (object) o, "__float__", out obj))
      obj = (object) DoubleOps.ParseFloat(s.MakeString());
    return cls == TypeCache.Double ? obj : cls.CreateInstance(context, obj);
  }

  public static PythonTuple as_integer_ratio(double self)
  {
    if (double.IsInfinity(self))
      throw PythonOps.OverflowError("Cannot pass infinity to float.as_integer_ratio.");
    if (double.IsNaN(self))
      throw PythonOps.ValueError("Cannot pass nan to float.as_integer_ratio.");
    BigInteger bigInteger = (BigInteger) 1;
    while (self % 1.0 != 0.0)
    {
      self *= 2.0;
      bigInteger *= (BigInteger) 2;
    }
    return PythonTuple.MakeTuple((object) (BigInteger) self, (object) bigInteger);
  }

  [ClassMethod]
  [StaticExtensionMethod]
  public static object fromhex(CodeContext context, PythonType cls, string self)
  {
    self = !string.IsNullOrEmpty(self) ? self.Trim(DoubleOps._whitespace) : throw PythonOps.ValueError("expected non empty string");
    double? specialFloat = DoubleOps.TryParseSpecialFloat(self);
    if (specialFloat.HasValue)
      return (object) specialFloat.Value;
    if (DoubleOps._fromHexRegex == null)
      DoubleOps._fromHexRegex = new Regex("\\A\\s*(?<sign>[-+])?(?:0[xX])?(?<integer>[0-9a-fA-F]+)?(?<fraction>\\.[0-9a-fA-F]*)?(?<exponent>[pP][-+]?[0-9]+)?\\s*\\z");
    Match match = DoubleOps._fromHexRegex.Match(self);
    Group group1 = match.Success ? match.Groups["sign"] : throw DoubleOps.InvalidHexString();
    Group group2 = match.Groups["integer"];
    Group group3 = match.Groups["fraction"];
    Group group4 = match.Groups["exponent"];
    bool flag1 = group1.Success && group1.Value == "-";
    BigInteger bigInteger1 = !group2.Success ? BigInteger.Zero : LiteralParser.ParseBigInteger(group2.Value, 16 /*0x10*/);
    int num1 = 0;
    BigInteger self1;
    if (group3.Success)
    {
      BigInteger bigInteger2 = (BigInteger) 0;
      for (int index = 1; index < group3.Value.Length; ++index)
      {
        char ch = group3.Value[index];
        int num2;
        if (ch >= '0' && ch <= '9')
          num2 = (int) ch - 48 /*0x30*/;
        else if (ch >= 'a' && ch <= 'f')
        {
          num2 = 10 + (int) ch - 97;
        }
        else
        {
          if (ch < 'A' || ch > 'Z')
            throw new InvalidOperationException();
          num2 = 10 + (int) ch - 65;
        }
        bigInteger2 = bigInteger2 << 4 | (BigInteger) num2;
        num1 += 4;
      }
      self1 = bigInteger1 << num1 | bigInteger2;
    }
    else
      self1 = bigInteger1;
    if (group4.Success)
    {
      int result = 0;
      if (!int.TryParse(group4.Value.Substring(1), out result))
      {
        if (!group4.Value.ToLowerAsciiTriggered().StartsWith("p-") && !(self1 == BigInteger.Zero))
          throw DoubleOps.HexStringOverflow();
        double num3 = flag1 ? -0.0 : 0.0;
        if (cls == TypeCache.Double)
          return (object) num3;
        return PythonCalls.Call((object) cls, (object) num3);
      }
      if (result > 0)
        self1 <<= result;
      else if (result < 0)
        num1 -= result;
    }
    if (!group4.Success && !group3.Success && !group2.Success || !group2.Success && group3.Length == 1)
      throw PythonOps.ValueError("invalid hexidecimal floating point string '{0}'", (object) self);
    if (self1 == BigInteger.Zero)
      return flag1 ? (object) -0.0 : (object) 0.0;
    int bitCount = self1.GetBitCount();
    int num4;
    for (num4 = bitCount - num1 - 1; num4 < -1023; ++num4)
      ++bitCount;
    if (num4 == -1023)
      ++bitCount;
    int num5 = bitCount - 53;
    bool flag2 = false;
    if (num5 > 0)
    {
      BigInteger self2 = self1 >> num5 - 1;
      if ((self2 & BigInteger.One) != BigInteger.Zero && ((self1 & (BigInteger.One << num5 - 1) - (BigInteger) 1) != BigInteger.Zero || (self1 >> num5 & BigInteger.One) != BigInteger.Zero))
      {
        BigInteger self3 = self2 + (BigInteger) 1;
        self1 = self3 >> 1 & (BigInteger) 4503599627370495L /*0x0FFFFFFFFFFFFF*/;
        if (self3.GetBitCount() != self2.GetBitCount())
        {
          if (num4 != -1023)
          {
            self1 >>= 1;
            ++num4;
          }
          else if (self1 == BigInteger.Zero)
            ++num4;
        }
        flag2 = true;
      }
    }
    if (!flag2)
      self1 = self1 >> bitCount - 53 & (BigInteger) 4503599627370495L /*0x0FFFFFFFFFFFFF*/;
    if (num4 > 1023 /*0x03FF*/)
      throw DoubleOps.HexStringOverflow();
    long num6 = (long) self1 | (long) num4 + 1023L /*0x03FF*/ << 52;
    if (flag1)
      num6 |= long.MinValue;
    double num7 = BitConverter.Int64BitsToDouble(num6);
    if (cls == TypeCache.Double)
      return (object) num7;
    return PythonCalls.Call((object) cls, (object) num7);
  }

  private static double? TryParseSpecialFloat(string self)
  {
    switch (self.ToLower())
    {
      case "+inf":
      case "+infinity":
      case "inf":
      case "infinity":
        return new double?(double.PositiveInfinity);
      case "+nan":
      case "-nan":
      case "nan":
        return new double?(double.NaN);
      case "-inf":
      case "-infinity":
        return new double?(double.NegativeInfinity);
      default:
        return new double?();
    }
  }

  private static Exception HexStringOverflow()
  {
    return PythonOps.OverflowError("hexadecimal value too large to represent as a float");
  }

  private static Exception InvalidHexString()
  {
    return PythonOps.ValueError("invalid hexadecimal floating-point string");
  }

  public static string hex(double self)
  {
    if (double.IsPositiveInfinity(self))
      return "inf";
    if (double.IsNegativeInfinity(self))
      return "-inf";
    if (double.IsNaN(self))
      return "nan";
    long int64Bits = BitConverter.DoubleToInt64Bits(self);
    int num = (int) (int64Bits >>> 52 & 2047L /*0x07FF*/) - 1023 /*0x03FF*/;
    long val = int64Bits & 4503599627370495L /*0x0FFFFFFFFFFFFF*/;
    StringBuilder stringBuilder = new StringBuilder();
    if ((int64Bits & long.MinValue) != 0L)
      stringBuilder.Append('-');
    if (num == -1023)
    {
      stringBuilder.Append("0x0.");
      ++num;
    }
    else
      stringBuilder.Append("0x1.");
    stringBuilder.Append(StringFormatSpec.FromString("013").AlignNumericText(BigIntegerOps.AbsToHex((BigInteger) val, true), val == 0L, true));
    stringBuilder.Append("p");
    if (num >= 0)
      stringBuilder.Append('+');
    stringBuilder.Append(num.ToString());
    return stringBuilder.ToString();
  }

  public static bool is_integer(double self) => self % 1.0 == 0.0;

  private static double ParseFloat(string x)
  {
    try
    {
      double? specialFloat = DoubleOps.TryParseSpecialFloat(x);
      return specialFloat.HasValue ? specialFloat.Value : LiteralParser.ParseFloat(x);
    }
    catch (FormatException ex)
    {
      throw PythonOps.ValueError("invalid literal for float(): {0}", (object) x);
    }
  }

  [SpecialName]
  public static object DivMod(double x, double y)
  {
    double num1 = y != 0.0 ? Math.IEEERemainder(x, y) : throw PythonOps.ZeroDivisionError();
    double d = (x - num1) / y;
    if (num1 != 0.0)
    {
      if (y < 0.0 != num1 < 0.0)
      {
        num1 += y;
        --d;
      }
    }
    else
      num1 = DoubleOps.CopySign(0.0, y);
    double num2;
    if (d != 0.0)
    {
      num2 = Math.Floor(d);
      if (d - num2 > 0.5)
        ++num2;
    }
    else
      num2 = DoubleOps.CopySign(0.0, x / y);
    return (object) PythonTuple.MakeTuple((object) num2, (object) num1);
  }

  [SpecialName]
  public static double Mod(double x, double y)
  {
    double num = y != 0.0 ? Math.IEEERemainder(x, y) : throw PythonOps.ZeroDivisionError();
    if (num != 0.0)
    {
      if (y < 0.0 != num < 0.0)
        num += y;
    }
    else
      num = DoubleOps.CopySign(0.0, y);
    return num;
  }

  [SpecialName]
  public static double Power(double x, double y)
  {
    if (x == 1.0 || y == 0.0)
      return 1.0;
    if (double.IsNaN(x) || double.IsNaN(y))
      return double.NaN;
    if (x == 0.0)
    {
      if (y > 0.0)
        return y % 2.0 == 1.0 ? x : 0.0;
      if (y == 0.0)
        return 1.0;
      if (double.IsNegativeInfinity(y))
        return double.PositiveInfinity;
      throw PythonOps.ZeroDivisionError("0.0 cannot be raised to a negative power");
    }
    if (double.IsPositiveInfinity(y))
    {
      if (x > 1.0 || x < -1.0)
        return double.PositiveInfinity;
      return x == -1.0 ? 1.0 : 0.0;
    }
    if (double.IsNegativeInfinity(y))
    {
      if (x > 1.0 || x < -1.0)
        return 0.0;
      return x == -1.0 ? 1.0 : double.PositiveInfinity;
    }
    if (double.IsNegativeInfinity(x))
      return Math.Abs(y % 2.0) == 1.0 ? (y <= 0.0 ? -0.0 : double.NegativeInfinity) : (y <= 0.0 ? 0.0 : double.PositiveInfinity);
    return x >= 0.0 || Math.Floor(y) == y ? PythonOps.CheckMath(x, y, Math.Pow(x, y)) : throw PythonOps.ValueError("negative number cannot be raised to fraction");
  }

  public static PythonTuple __coerce__(CodeContext context, double x, object o)
  {
    double d = (double) DoubleOps.__new__(context, TypeCache.Double, o);
    return !double.IsInfinity(d) ? PythonTuple.MakeTuple((object) x, (object) d) : throw PythonOps.OverflowError("number too big");
  }

  public static object __int__(double d)
  {
    if ((double) int.MinValue <= d && d <= (double) int.MaxValue)
      return (object) (int) d;
    if (double.IsInfinity(d))
      throw PythonOps.OverflowError("cannot convert float infinity to integer");
    return !double.IsNaN(d) ? (object) (BigInteger) d : throw PythonOps.ValueError("cannot convert float NaN to integer");
  }

  public static object __getnewargs__(CodeContext context, double self)
  {
    return (object) PythonTuple.MakeTuple(DoubleOps.__new__(context, TypeCache.Double, (object) self));
  }

  public static string __str__(CodeContext context, double x)
  {
    return new StringFormatter(context, "%.12g", (object) x)
    {
      _TrailingZeroAfterWholeFloat = true
    }.Format();
  }

  public static string __str__(double x, IFormatProvider provider) => x.ToString(provider);

  public static string __str__(double x, string format) => x.ToString(format);

  public static string __str__(double x, string format, IFormatProvider provider)
  {
    return x.ToString(format, provider);
  }

  public static int __hash__(double d)
  {
    return d % 1.0 == 0.0 ? ((double) int.MinValue <= d && d <= (double) int.MaxValue ? ((int) d).GetHashCode() : BigIntegerOps.__hash__((BigInteger) d)) : (double.IsInfinity(d) ? (d <= 0.0 ? -271828 : 314159) : (double.IsNaN(d) ? 0 : d.GetHashCode()));
  }

  [SpecialName]
  public static bool LessThan(double x, double y)
  {
    if (x >= y || double.IsInfinity(x) && double.IsNaN(y))
      return false;
    return !double.IsNaN(x) || !double.IsInfinity(y);
  }

  [SpecialName]
  public static bool LessThanOrEqual(double x, double y) => x == y ? !double.IsNaN(x) : x < y;

  [SpecialName]
  public static bool GreaterThan(double x, double y)
  {
    if (x <= y || double.IsInfinity(x) && double.IsNaN(y))
      return false;
    return !double.IsNaN(x) || !double.IsInfinity(y);
  }

  [SpecialName]
  public static bool GreaterThanOrEqual(double x, double y) => x == y ? !double.IsNaN(x) : x > y;

  [SpecialName]
  public static bool Equals(double x, double y) => x == y && !double.IsNaN(x);

  [SpecialName]
  public static bool NotEquals(double x, double y) => x != y || double.IsNaN(x);

  [SpecialName]
  public static bool LessThan(double x, BigInteger y) => DoubleOps.Compare(x, y) < 0;

  [SpecialName]
  public static bool LessThanOrEqual(double x, BigInteger y) => DoubleOps.Compare(x, y) <= 0;

  [SpecialName]
  public static bool GreaterThan(double x, BigInteger y) => DoubleOps.Compare(x, y) > 0;

  [SpecialName]
  public static bool GreaterThanOrEqual(double x, BigInteger y) => DoubleOps.Compare(x, y) >= 0;

  [SpecialName]
  public static bool Equals(double x, BigInteger y) => DoubleOps.Compare(x, y) == 0;

  [SpecialName]
  public static bool NotEquals(double x, BigInteger y) => DoubleOps.Compare(x, y) != 0;

  internal static bool IsPositiveZero(double value)
  {
    return value == 0.0 && double.IsPositiveInfinity(1.0 / value);
  }

  internal static bool IsNegativeZero(double value)
  {
    return value == 0.0 && double.IsNegativeInfinity(1.0 / value);
  }

  internal static int Sign(double value)
  {
    return value == 0.0 ? (!double.IsPositiveInfinity(1.0 / value) ? -1 : 1) : (value <= 0.0 ? -1 : 1);
  }

  internal static double CopySign(double value, double sign)
  {
    return (double) DoubleOps.Sign(sign) * Math.Abs(value);
  }

  internal static int Compare(double x, double y)
  {
    if (double.IsInfinity(x) && double.IsNaN(y))
      return 1;
    if (double.IsNaN(x) && double.IsInfinity(y))
      return -1;
    if (x > y)
      return 1;
    return x != y ? -1 : 0;
  }

  internal static int Compare(double x, BigInteger y) => -DoubleOps.Compare(y, x);

  internal static int Compare(BigInteger x, double y)
  {
    if (double.IsNaN(y) || double.IsPositiveInfinity(y))
      return -1;
    if (y == double.NegativeInfinity)
      return 1;
    BigInteger bigInteger = (BigInteger) y;
    if (bigInteger == x)
    {
      double num = y % 1.0;
      if (num == 0.0)
        return 0;
      return num > 0.0 ? -1 : 1;
    }
    return bigInteger > x ? -1 : 1;
  }

  [SpecialName]
  public static bool LessThan(double x, Decimal y) => DoubleOps.Compare(x, y) < 0;

  [SpecialName]
  public static bool LessThanOrEqual(double x, Decimal y) => DoubleOps.Compare(x, y) <= 0;

  [SpecialName]
  public static bool GreaterThan(double x, Decimal y) => DoubleOps.Compare(x, y) > 0;

  [SpecialName]
  public static bool GreaterThanOrEqual(double x, Decimal y) => DoubleOps.Compare(x, y) >= 0;

  [SpecialName]
  public static bool Equals(double x, Decimal y) => DoubleOps.Compare(x, y) == 0;

  [SpecialName]
  public static bool NotEquals(double x, Decimal y) => DoubleOps.Compare(x, y) != 0;

  internal static int Compare(double x, Decimal y)
  {
    if (x > 7.9228162514264338E+28)
      return 1;
    return x < -7.9228162514264338E+28 ? -1 : ((Decimal) x).CompareTo(y);
  }

  [SpecialName]
  public static bool LessThan(double x, int y) => x < (double) y;

  [SpecialName]
  public static bool LessThanOrEqual(double x, int y) => x <= (double) y;

  [SpecialName]
  public static bool GreaterThan(double x, int y) => x > (double) y;

  [SpecialName]
  public static bool GreaterThanOrEqual(double x, int y) => x >= (double) y;

  [SpecialName]
  public static bool Equals(double x, int y) => x == (double) y;

  [SpecialName]
  public static bool NotEquals(double x, int y) => x != (double) y;

  public static string __repr__(CodeContext context, double self)
  {
    if (double.IsNaN(self))
      return "nan";
    string text = new StringFormatter(context, "%.17g", (object) self)
    {
      _TrailingZeroAfterWholeFloat = true
    }.Format();
    return LiteralParser.ParseFloat(text) == self ? text : self.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public static BigInteger __long__(double self)
  {
    if (double.IsInfinity(self))
      throw PythonOps.OverflowError("cannot convert float infinity to integer");
    return !double.IsNaN(self) ? (BigInteger) self : throw PythonOps.ValueError("cannot convert float NaN to integer");
  }

  public static double __float__(double self) => self;

  public static string __getformat__(CodeContext context, string typestr)
  {
    FloatFormat floatFormat;
    switch (typestr)
    {
      case "float":
        floatFormat = context.LanguageContext.FloatFormat;
        break;
      case "double":
        floatFormat = context.LanguageContext.DoubleFormat;
        break;
      default:
        throw PythonOps.ValueError("__getformat__() argument 1 must be 'double' or 'float'");
    }
    switch (floatFormat)
    {
      case FloatFormat.Unknown:
        return "unknown";
      case FloatFormat.IEEE_LittleEndian:
        return "IEEE, little-endian";
      case FloatFormat.IEEE_BigEndian:
        return "IEEE, big-endian";
      default:
        return DoubleOps.DefaultFloatFormat();
    }
  }

  public static string __format__(CodeContext context, double self, [NotNull] string formatSpec)
  {
    if (formatSpec == string.Empty)
      return DoubleOps.__str__(context, self);
    StringFormatSpec spec = StringFormatSpec.FromString(formatSpec);
    string text = double.IsPositiveInfinity(self) || double.IsNegativeInfinity(self) ? (!spec.Type.HasValue || !char.IsUpper(spec.Type.Value) ? "inf" : "INF") : (!double.IsNaN(self) ? DoubleOps.DoubleToFormatString(context, self, spec) : (!spec.Type.HasValue || !char.IsUpper(spec.Type.Value) ? "nan" : "NAN"));
    return !spec.Sign.HasValue ? spec.AlignNumericText(text, false, double.IsNaN(self) || DoubleOps.Sign(self) > 0) : spec.AlignNumericText(text, false, double.IsNaN(self) || DoubleOps.Sign(self) > 0);
  }

  private static string DoubleToFormatString(
    CodeContext context,
    double self,
    StringFormatSpec spec)
  {
    self = Math.Abs(self);
    int? nullable = spec.Precision;
    int num1 = nullable ?? 6;
    char? type1 = spec.Type;
    if (type1.HasValue)
    {
      switch (type1.GetValueOrDefault())
      {
        case char.MinValue:
          break;
        case '%':
          string format1 = $"0.{new string('0', num1)}%";
          if (spec.ThousandsComma)
            format1 = "#," + format1;
          return self.ToString(format1, (IFormatProvider) CultureInfo.InvariantCulture);
        case 'E':
        case 'e':
          string format2 = $"0.{new string('0', num1)}{(object) spec.Type}+00";
          if (spec.ThousandsComma)
            format2 = "#," + format2;
          return self.ToString(format2, (IFormatProvider) CultureInfo.InvariantCulture);
        case 'F':
        case 'f':
          string format3 = "0." + new string('0', num1);
          if (spec.ThousandsComma)
            format3 = "#," + format3;
          return self.ToString(format3, (IFormatProvider) CultureInfo.InvariantCulture);
        case 'G':
        case 'g':
        case 'n':
          int num2 = 1;
          double num3 = self;
          while (num3 >= 10.0)
          {
            num3 /= 10.0;
            ++num2;
          }
          if (num2 > num1 && num2 != 1)
          {
            self = MathUtils.RoundAwayFromZero(self, 0);
            double num4 = Math.Pow(10.0, (double) (num2 - Math.Max(num1, 1)));
            double num5 = self / num4;
            self -= self % num4;
            if (num5 % 1.0 >= 0.5)
              self += num4;
            char? type2 = spec.Type;
            nullable = type2.HasValue ? new int?((int) type2.GetValueOrDefault()) : new int?();
            int num6 = 110;
            string str1;
            if (nullable.GetValueOrDefault() == num6 & nullable.HasValue && context.LanguageContext.NumericCulture != PythonContext.CCulture)
            {
              str1 = "0";
            }
            else
            {
              nullable = spec.Precision;
              int num7 = 1;
              str1 = nullable.GetValueOrDefault() > num7 & nullable.HasValue || num2 > 6 ? "0.#" + new string('#', num1) : "0";
            }
            if (spec.ThousandsComma)
              str1 = "#," + str1;
            ref double local = ref self;
            string str2 = str1;
            char? type3 = spec.Type;
            nullable = type3.HasValue ? new int?((int) type3.GetValueOrDefault()) : new int?();
            int num8 = 71;
            string str3 = nullable.GetValueOrDefault() == num8 & nullable.HasValue ? "E+00" : "e+00";
            string format4 = str2 + str3;
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            return local.ToString(format4, (IFormatProvider) invariantCulture);
          }
          if (self < 1.0)
            --num2;
          int num9 = Math.Max(num1 - num2, 0);
          self = MathUtils.RoundAwayFromZero(self, num9);
          char? type4 = spec.Type;
          nullable = type4.HasValue ? new int?((int) type4.GetValueOrDefault()) : new int?();
          int num10 = 110;
          return nullable.GetValueOrDefault() == num10 & nullable.HasValue && context.LanguageContext.NumericCulture != PythonContext.CCulture ? (num2 != num1 && self % 1.0 != 0.0 ? self.ToString("#,0.0" + new string('#', num9)) : self.ToString("#,0")) : (num2 != num1 && self % 1.0 != 0.0 ? self.ToString("0.0" + new string('#', num9)) : self.ToString("0"));
        default:
          throw PythonOps.ValueError("Unknown format code '{0}' for object of type 'float'", (object) spec.Type.ToString());
      }
    }
    if (spec.Precision.HasValue)
    {
      int num11 = 1;
      double num12 = self;
      while (num12 >= 10.0)
      {
        num12 /= 10.0;
        ++num11;
      }
      if (num11 > spec.Precision.Value && num11 != 1)
      {
        self = MathUtils.RoundAwayFromZero(self, 0);
        double num13 = Math.Pow(10.0, (double) (num11 - Math.Max(spec.Precision.Value, 1)));
        self -= self % num13;
        string str = "0.0" + new string('#', spec.Precision.Value);
        return self.ToString(str + "e+00", (IFormatProvider) CultureInfo.InvariantCulture);
      }
      int num14 = Math.Max(spec.Precision.Value - num11, 0);
      self = MathUtils.RoundAwayFromZero(self, num14);
      return self.ToString("0.0" + new string('#', num14));
    }
    if (DoubleOps.IncludeExponent(self))
      return self.ToString("0.#e+00", (IFormatProvider) CultureInfo.InvariantCulture);
    return spec.ThousandsComma ? self.ToString("#,0.0###", (IFormatProvider) CultureInfo.InvariantCulture) : self.ToString("0.0###", (IFormatProvider) CultureInfo.InvariantCulture);
  }

  private static bool IncludeExponent(double self)
  {
    if (self >= 1000000000000.0)
      return true;
    return self != 0.0 && self <= 9E-05;
  }

  private static string DefaultFloatFormat()
  {
    return BitConverter.IsLittleEndian ? "IEEE, little-endian" : "IEEE, big-endian";
  }

  public static void __setformat__(CodeContext context, string typestr, string fmt)
  {
    FloatFormat floatFormat;
    switch (fmt)
    {
      case "unknown":
        floatFormat = FloatFormat.Unknown;
        break;
      case "IEEE, little-endian":
        if (!BitConverter.IsLittleEndian)
          throw PythonOps.ValueError("can only set double format to 'unknown' or the detected platform value");
        floatFormat = FloatFormat.IEEE_LittleEndian;
        break;
      case "IEEE, big-endian":
        if (BitConverter.IsLittleEndian)
          throw PythonOps.ValueError("can only set double format to 'unknown' or the detected platform value");
        floatFormat = FloatFormat.IEEE_BigEndian;
        break;
      default:
        throw PythonOps.ValueError(" __setformat__() argument 2 must be 'unknown', 'IEEE, little-endian' or 'IEEE, big-endian'");
    }
    switch (typestr)
    {
      case "float":
        context.LanguageContext.FloatFormat = floatFormat;
        break;
      case "double":
        context.LanguageContext.DoubleFormat = floatFormat;
        break;
      default:
        throw PythonOps.ValueError("__setformat__() argument 1 must be 'double' or 'float'");
    }
  }

  [SpecialName]
  public static double Plus(double x) => x;

  [SpecialName]
  public static double Negate(double x) => -x;

  [SpecialName]
  public static double Abs(double x) => Math.Abs(x);

  public static bool __nonzero__(double x) => x != 0.0;

  public static object __trunc__(double x)
  {
    return x >= (double) int.MaxValue || x <= (double) int.MinValue ? (object) (BigInteger) x : (object) (int) x;
  }

  [SpecialName]
  public static double Add(double x, double y) => x + y;

  [SpecialName]
  public static double Subtract(double x, double y) => x - y;

  [SpecialName]
  public static double Multiply(double x, double y) => x * y;

  [SpecialName]
  public static double Divide(double x, double y) => DoubleOps.TrueDivide(x, y);

  [SpecialName]
  public static double TrueDivide(double x, double y)
  {
    if (y == 0.0)
      throw PythonOps.ZeroDivisionError();
    return x / y;
  }

  [SpecialName]
  public static double FloorDivide(double x, double y)
  {
    return y != 0.0 ? Math.Floor(x / y) : throw PythonOps.ZeroDivisionError();
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static sbyte ConvertToSByte(double x)
  {
    return (double) sbyte.MinValue <= x && x <= (double) sbyte.MaxValue ? (sbyte) x : throw Converter.CannotConvertOverflow("SByte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static byte ConvertToByte(double x)
  {
    return 0.0 <= x && x <= (double) byte.MaxValue ? (byte) x : throw Converter.CannotConvertOverflow("Byte", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static short ConvertToInt16(double x)
  {
    return (double) short.MinValue <= x && x <= (double) short.MaxValue ? (short) x : throw Converter.CannotConvertOverflow("Int16", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static ushort ConvertToUInt16(double x)
  {
    return 0.0 <= x && x <= (double) ushort.MaxValue ? (ushort) x : throw Converter.CannotConvertOverflow("UInt16", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static int ConvertToInt32(double x)
  {
    return (double) int.MinValue <= x && x <= (double) int.MaxValue ? (int) x : throw Converter.CannotConvertOverflow("Int32", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static uint ConvertToUInt32(double x)
  {
    return 0.0 <= x && x <= (double) uint.MaxValue ? (uint) x : throw Converter.CannotConvertOverflow("UInt32", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static long ConvertToInt64(double x)
  {
    return (double) long.MinValue <= x && x <= (double) long.MaxValue ? (long) x : throw Converter.CannotConvertOverflow("Int64", (object) x);
  }

  [ExplicitConversionMethod]
  [SpecialName]
  public static ulong ConvertToUInt64(double x)
  {
    return 0.0 <= x && x <= 1.8446744073709552E+19 ? (ulong) x : throw Converter.CannotConvertOverflow("UInt64", (object) x);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static float ConvertToSingle(double x) => (float) x;

  [PropertyMethod]
  [SpecialName]
  public static double Getreal(double x) => x;

  [PropertyMethod]
  [SpecialName]
  public static double Getimag(double x) => 0.0;

  public static double conjugate(double x) => x;
}
