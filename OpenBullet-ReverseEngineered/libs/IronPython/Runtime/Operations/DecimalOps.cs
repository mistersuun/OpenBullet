// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.DecimalOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class DecimalOps
{
  public static int __cmp__(CodeContext context, Decimal x, Decimal other) => x.CompareTo(other);

  public static bool __nonzero__(Decimal x) => x != 0M;

  public static string __repr__(Decimal x)
  {
    return x.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }

  [SpecialName]
  public static bool LessThan(Decimal x, Decimal y) => x < y;

  [SpecialName]
  public static bool LessThanOrEqual(Decimal x, Decimal y) => x <= y;

  [SpecialName]
  public static bool GreaterThan(Decimal x, Decimal y) => x > y;

  [SpecialName]
  public static bool GreaterThanOrEqual(Decimal x, Decimal y) => x >= y;

  [SpecialName]
  public static bool Equals(Decimal x, Decimal y) => x == y;

  [SpecialName]
  public static bool NotEquals(Decimal x, Decimal y) => x != y;

  internal static int __cmp__(BigInteger x, Decimal y) => -DecimalOps.__cmp__(y, x);

  internal static int __cmp__(Decimal x, BigInteger y)
  {
    BigInteger bigInteger = (BigInteger) x;
    if (bigInteger == y)
    {
      Decimal num = x % 1M;
      if (num == 0M)
        return 0;
      return num > 0M ? 1 : -1;
    }
    return !(bigInteger > y) ? -1 : 1;
  }

  [return: MaybeNotImplemented]
  internal static object __cmp__(object x, Decimal y) => DecimalOps.__cmp__(y, x);

  [return: MaybeNotImplemented]
  internal static object __cmp__(Decimal x, object y)
  {
    return y == null ? ScriptingRuntimeHelpers.Int32ToObject(1) : (object) PythonOps.NotImplemented;
  }

  public static int __hash__(Decimal x) => ((BigInteger) x).GetHashCode();

  public static string __format__(CodeContext context, Decimal self, [NotNull] string formatSpec)
  {
    StringFormatSpec spec = StringFormatSpec.FromString(formatSpec);
    if (spec.IsEmpty)
      return self.ToString();
    string formatString = DecimalOps.DecimalToFormatString(context, self, spec);
    return spec.AlignNumericText(formatString, self != 0M, self > 0M);
  }

  private static string DecimalToFormatString(
    CodeContext context,
    Decimal self,
    StringFormatSpec spec)
  {
    self = Math.Abs(self);
    int? nullable = spec.Precision;
    int num1 = nullable ?? 12;
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
          Decimal num3 = self;
          while (num3 >= 10M)
          {
            num3 /= 10M;
            ++num2;
          }
          if (num2 > num1 && num2 != 1)
          {
            self = Decimal.Round(self, 0, MidpointRounding.AwayFromZero);
            double num4 = Math.Pow(10.0, (double) (num2 - Math.Max(num1, 1)));
            Decimal num5 = self / (Decimal) num4;
            self -= self % (Decimal) num4;
            if (num5 % 1M >= 0.5M)
              self += (Decimal) num4;
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
            ref Decimal local = ref self;
            string str2 = str1;
            char? type3 = spec.Type;
            nullable = type3.HasValue ? new int?((int) type3.GetValueOrDefault()) : new int?();
            int num8 = 71;
            string str3 = nullable.GetValueOrDefault() == num8 & nullable.HasValue ? "E+00" : "e+00";
            string format4 = str2 + str3;
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            return local.ToString(format4, (IFormatProvider) invariantCulture);
          }
          if (self < 1M)
            --num2;
          int num9 = Math.Max(num1 - num2, 0);
          self = Decimal.Round(self, num9, MidpointRounding.AwayFromZero);
          char? type4 = spec.Type;
          nullable = type4.HasValue ? new int?((int) type4.GetValueOrDefault()) : new int?();
          int num10 = 110;
          return nullable.GetValueOrDefault() == num10 & nullable.HasValue && context.LanguageContext.NumericCulture != PythonContext.CCulture ? (num2 != num1 && self % 1M != 0M ? self.ToString("#,0.0" + new string('#', num9)) : self.ToString("#,0")) : (num2 != num1 && self % 1M != 0M ? self.ToString("0.0" + new string('#', num9)) : self.ToString("0"));
        default:
          throw PythonOps.ValueError("Unknown format code '{0}' for object of type 'decimal'", (object) spec.Type.ToString());
      }
    }
    if (spec.Precision.HasValue)
    {
      int num11 = 1;
      Decimal num12 = self;
      while (num12 >= 10M)
      {
        num12 /= 10M;
        ++num11;
      }
      if (num11 > spec.Precision.Value && num11 != 1)
      {
        self = Decimal.Round(self, 0, MidpointRounding.AwayFromZero);
        double num13 = Math.Pow(10.0, (double) (num11 - Math.Max(spec.Precision.Value, 1)));
        self -= self % (Decimal) num13;
        string str = "0.0" + new string('#', spec.Precision.Value);
        return self.ToString(str + "e+00", (IFormatProvider) CultureInfo.InvariantCulture);
      }
      int num14 = Math.Max(spec.Precision.Value - num11, 0);
      self = Decimal.Round(self, num14, MidpointRounding.AwayFromZero);
      return self.ToString("0.0" + new string('#', num14));
    }
    if (self >= 1000000000000M || self != 0M && self <= 0.00009M)
      return self.ToString("0.#e+00", (IFormatProvider) CultureInfo.InvariantCulture);
    return spec.ThousandsComma ? self.ToString("#,0.0###", (IFormatProvider) CultureInfo.InvariantCulture) : self.ToString("0.0###", (IFormatProvider) CultureInfo.InvariantCulture);
  }
}
