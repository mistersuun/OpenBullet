// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.StringFormatter
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal class StringFormatter
{
  private const int UnspecifiedPrecision = -1;
  private readonly CodeContext _context;
  private readonly bool _isUnicodeString;
  private object _data;
  private int _dataIndex;
  private string _str;
  private int _index;
  private char _curCh;
  internal StringFormatter.FormatSettings _opts;
  internal bool _TrailingZeroAfterWholeFloat;
  private StringBuilder _buf;
  [ThreadStatic]
  private static NumberFormatInfo NumberFormatInfoForThreadLower;
  [ThreadStatic]
  private static NumberFormatInfo NumberFormatInfoForThreadUpper;
  private NumberFormatInfo _nfi;
  private static readonly char[] zero = new char[1]{ '0' };
  private static readonly long NegativeZeroBits = BitConverter.DoubleToInt64Bits(-0.0);

  internal static NumberFormatInfo nfil
  {
    get
    {
      if (StringFormatter.NumberFormatInfoForThreadLower == null)
      {
        NumberFormatInfo numberFormat = ((CultureInfo) CultureInfo.InvariantCulture.Clone()).NumberFormat;
        numberFormat.PositiveInfinitySymbol = "inf";
        numberFormat.NegativeInfinitySymbol = "-inf";
        numberFormat.NaNSymbol = "nan";
        StringFormatter.NumberFormatInfoForThreadLower = numberFormat;
      }
      return StringFormatter.NumberFormatInfoForThreadLower;
    }
  }

  internal static NumberFormatInfo nfiu
  {
    get
    {
      if (StringFormatter.NumberFormatInfoForThreadUpper == null)
      {
        NumberFormatInfo numberFormat = ((CultureInfo) CultureInfo.InvariantCulture.Clone()).NumberFormat;
        numberFormat.PositiveInfinitySymbol = "INF";
        numberFormat.NegativeInfinitySymbol = "-INF";
        numberFormat.NaNSymbol = "NAN";
        StringFormatter.NumberFormatInfoForThreadUpper = numberFormat;
      }
      return StringFormatter.NumberFormatInfoForThreadUpper;
    }
  }

  public StringFormatter(CodeContext context, string str, object data)
    : this(context, str, data, false)
  {
  }

  public StringFormatter(CodeContext context, string str, object data, bool isUnicode)
  {
    this._str = str;
    this._data = data;
    this._context = context;
    this._isUnicodeString = isUnicode;
    this._nfi = StringFormatter.nfil;
  }

  public string Format()
  {
    this._index = 0;
    this._buf = new StringBuilder(this._str.Length * 2);
    int num;
    while ((num = this._str.IndexOf('%', this._index)) != -1)
    {
      this._buf.Append(this._str, this._index, num - this._index);
      this._index = num + 1;
      this.DoFormatCode();
    }
    this._buf.Append(this._str, this._index, this._str.Length - this._index);
    this.CheckDataUsed();
    return this._buf.ToString();
  }

  private void DoFormatCode()
  {
    if (this._index == this._str.Length)
      throw PythonOps.ValueError("incomplete format, expected format character at index {0}", (object) this._index);
    this._curCh = this._str[this._index++];
    if (this._curCh == '%')
    {
      this._buf.Append('%');
    }
    else
    {
      string key = this.ReadMappingKey();
      this._opts = new StringFormatter.FormatSettings();
      this.ReadConversionFlags();
      this.ReadMinimumFieldWidth();
      this.ReadPrecision();
      this.ReadLengthModifier();
      this._opts.Value = key != null ? this.GetKey(key) : this.GetData(this._dataIndex++);
      this.WriteConversion();
    }
  }

  private string ReadMappingKey()
  {
    if (this._curCh != '(')
      return (string) null;
    int num = 1;
    int index1 = this._index;
    for (int index2 = index1; index2 < this._str.Length; ++index2)
    {
      if (this._str[index2] == '(')
        ++num;
      else if (this._str[index2] == ')')
        --num;
      if (num == 0)
      {
        string str = this._str.Substring(this._index, index2 - index1);
        this._index = index2 + 1;
        if (this._index == this._str.Length)
          throw PythonOps.ValueError("incomplete format");
        this._curCh = this._str[this._index++];
        return str;
      }
    }
    throw PythonOps.ValueError("incomplete format key");
  }

  private void ReadConversionFlags()
  {
    bool flag;
    do
    {
      flag = true;
      switch (this._curCh)
      {
        case ' ':
          if (!this._opts.SignChar)
          {
            this._opts.Space = true;
            break;
          }
          break;
        case '#':
          this._opts.AltForm = true;
          break;
        case '+':
          this._opts.SignChar = true;
          this._opts.Space = false;
          break;
        case '-':
          this._opts.LeftAdj = true;
          this._opts.ZeroPad = false;
          break;
        case '0':
          if (!this._opts.LeftAdj)
          {
            this._opts.ZeroPad = true;
            break;
          }
          break;
        default:
          flag = false;
          break;
      }
      if (flag)
        this._curCh = this._str[this._index++];
    }
    while (flag);
  }

  private int ReadNumberOrStar() => this.ReadNumberOrStar(0);

  private int ReadNumberOrStar(int noValSpecified)
  {
    int num = noValSpecified;
    if (this._curCh == '*')
    {
      if (!(this._data is PythonTuple))
        throw PythonOps.TypeError("* requires a tuple for values");
      this._curCh = this._str[this._index++];
      num = this._context.LanguageContext.ConvertToInt32(this.GetData(this._dataIndex++));
    }
    else if (char.IsDigit(this._curCh))
    {
      num = 0;
      for (; char.IsDigit(this._curCh) && this._index < this._str.Length; this._curCh = this._str[this._index++])
        num = num * 10 + ((int) this._curCh - 48 /*0x30*/);
    }
    return num;
  }

  private void ReadMinimumFieldWidth()
  {
    int num = this.ReadNumberOrStar();
    if (num < 0)
    {
      this._opts.FieldWidth = num * -1;
      this._opts.LeftAdj = true;
    }
    else
      this._opts.FieldWidth = num;
    if (this._opts.FieldWidth == int.MaxValue)
      throw PythonOps.MemoryError("not enough memory for field width");
  }

  private void ReadPrecision()
  {
    if (this._curCh == '.')
    {
      this._curCh = this._str[this._index++];
      this._opts.Precision = this.ReadNumberOrStar();
      if (this._opts.Precision > 116)
        throw PythonOps.OverflowError("formatted integer is too long (precision too large?)");
    }
    else
      this._opts.Precision = -1;
  }

  private void ReadLengthModifier()
  {
    switch (this._curCh)
    {
      case 'L':
      case 'h':
      case 'l':
        this._curCh = this._str[this._index++];
        break;
    }
  }

  private void WriteConversion()
  {
    switch (this._curCh)
    {
      case 'E':
      case 'F':
      case 'G':
        this._nfi = StringFormatter.nfiu;
        this.AppendFloat(this._curCh);
        this._nfi = StringFormatter.nfil;
        break;
      case 'X':
        this.AppendHex(this._curCh);
        break;
      case 'c':
        this.AppendChar();
        break;
      case 'd':
      case 'i':
        this.AppendInt();
        break;
      case 'e':
      case 'f':
      case 'g':
        this.AppendFloat(this._curCh);
        break;
      case 'o':
        this.AppendOctal();
        break;
      case 'r':
        this.AppendRepr();
        break;
      case 's':
        this.AppendString();
        break;
      case 'u':
        this.AppendInt();
        break;
      case 'x':
        this.AppendHex(this._curCh);
        break;
      default:
        if (this._curCh > 'ÿ')
          throw PythonOps.ValueError("unsupported format character '{0}' (0x{1:X}) at index {2}", (object) '?', (object) (int) this._curCh, (object) (this._index - 1));
        throw PythonOps.ValueError("unsupported format character '{0}' (0x{1:X}) at index {2}", (object) this._curCh, (object) (int) this._curCh, (object) (this._index - 1));
    }
  }

  private object GetData(int index)
  {
    if (this._data is PythonTuple data)
    {
      if (index < data.__len__())
        return data[index];
    }
    else if (index == 0)
      return this._data;
    throw PythonOps.TypeError("not enough arguments for format string");
  }

  private object GetKey(string key)
  {
    if (!(this._data is IDictionary<object, object> data1))
    {
      if (!(this._data is PythonDictionary data))
      {
        if (PythonOps.IsMappingType(DefaultContext.Default, this._data) == ScriptingRuntimeHelpers.True)
          return PythonOps.GetIndex(this._context, this._data, (object) key);
        throw PythonOps.TypeError("format requires a mapping");
      }
      object key1;
      if (data.TryGetValue((object) key, out key1))
        return key1;
    }
    else
    {
      object key2;
      if (data1.TryGetValue((object) key, out key2))
        return key2;
    }
    throw PythonOps.KeyError(key);
  }

  private object GetIntegerValue(out bool fPos)
  {
    int res;
    object integerValue;
    if (this._context.LanguageContext.TryConvertToInt32(this._opts.Value, out res))
    {
      integerValue = (object) res;
      fPos = res >= 0;
    }
    else
    {
      BigInteger result;
      if (!Converter.TryConvertToBigInteger(this._opts.Value, out result))
        throw PythonOps.TypeError("int argument required");
      integerValue = (object) result;
      fPos = result >= BigInteger.Zero;
    }
    return integerValue;
  }

  private void AppendChar()
  {
    char ch = Converter.ExplicitConvertToChar(this._opts.Value);
    if (this._opts.FieldWidth > 1)
    {
      if (!this._opts.LeftAdj)
        this._buf.Append(' ', this._opts.FieldWidth - 1);
      this._buf.Append(ch);
      if (!this._opts.LeftAdj)
        return;
      this._buf.Append(' ', this._opts.FieldWidth - 1);
    }
    else
      this._buf.Append(ch);
  }

  private void CheckDataUsed()
  {
    if (PythonOps.IsMappingType(DefaultContext.Default, this._data) == ScriptingRuntimeHelpers.False && (!(this._data is PythonTuple) && this._dataIndex != 1 || this._data is PythonTuple && this._dataIndex != ((PythonTuple) this._data).__len__()))
      throw PythonOps.TypeError("not all arguments converted during string formatting");
  }

  private void AppendInt()
  {
    bool fPos;
    object integerValue = this.GetIntegerValue(out fPos);
    if (this._opts.LeftAdj)
      this.AppendLeftAdj(integerValue, fPos, 'D');
    else if (this._opts.Precision > 0)
    {
      this._opts.FieldWidth = this._opts.Space || this._opts.SignChar ? this._opts.Precision + 1 : this._opts.Precision;
      this.AppendZeroPad(integerValue, fPos, 'D');
    }
    else if (this._opts.ZeroPad)
      this.AppendZeroPad(integerValue, fPos, 'D');
    else
      this.AppendNumeric(integerValue, fPos, 'D');
  }

  private char AdjustForG(char type, double v)
  {
    if (type != 'G' && type != 'g' || double.IsNaN(v) || double.IsInfinity(v))
      return type;
    double d = Math.Abs(v);
    if (this._opts.Precision == 0)
      this._opts.Precision = 1;
    if (v != 0.0 && d < 0.0001 || d >= Math.Pow(10.0, (double) this._opts.Precision))
    {
      type = type == 'G' ? 'E' : 'e';
      int num = this._opts.Precision - 1;
      string str1 = d.ToString("E" + (object) num, (IFormatProvider) CultureInfo.InvariantCulture);
      string str2 = str1.Substring(0, str1.IndexOf('E')).TrimEnd(StringFormatter.zero);
      this._opts.Precision = str2.Length != 1 ? str2.Length - 2 : 0;
    }
    else
    {
      int precision = this._opts.Precision;
      if (d < 0.001)
        precision += 3;
      else if (d < 0.01)
        precision += 2;
      else if (d < 0.1)
        ++precision;
      string str3 = d.ToString("F" + (object) precision, (IFormatProvider) CultureInfo.InvariantCulture).TrimEnd(StringFormatter.zero);
      bool flag = true;
      if (precision > 15)
      {
        string str4 = d.ToString("G" + (object) precision, (IFormatProvider) CultureInfo.InvariantCulture).TrimEnd(StringFormatter.zero);
        if (str4.Length > str3.Length)
        {
          str3 = str4;
          flag = false;
        }
      }
      if (flag)
      {
        type = type == 'G' ? 'F' : 'f';
        string str5 = str3.Substring(str3.IndexOf('.') + 1);
        this._opts.Precision = d >= 1.0 ? Math.Min(this._opts.Precision - (1 + (int) Math.Log10(d)), str5.Length) : str5.Length;
      }
    }
    return type;
  }

  private void AppendFloat(char type)
  {
    double result;
    if (!Converter.TryConvertToDouble(this._opts.Value, out result))
      throw PythonOps.TypeError("float argument required");
    bool flag = false;
    if (this._opts.Precision != -1)
    {
      if (this._opts.Precision == 0 && this._opts.AltForm)
        flag = true;
      if (this._opts.Precision > 50)
        this._opts.Precision = 50;
    }
    else if (this._opts.AltForm)
    {
      this._opts.Precision = 0;
      flag = true;
    }
    else
      this._opts.Precision = 6;
    type = this.AdjustForG(type, result);
    this._nfi.NumberDecimalDigits = this._opts.Precision;
    if (this._opts.LeftAdj)
      this.AppendLeftAdj((object) result, DoubleOps.Sign(result) >= 0, type);
    else if (this._opts.ZeroPad)
      this.AppendZeroPadFloat(result, type);
    else
      this.AppendNumeric((object) result, DoubleOps.Sign(result) >= 0, type);
    if (DoubleOps.Sign(result) < 0 && result > -1.0 && this._buf[0] != '-')
      this.FixupFloatMinus();
    if (!flag)
      return;
    this.FixupAltFormDot(result);
  }

  private void FixupAltFormDot(double v)
  {
    if (!double.IsInfinity(v) && !double.IsNaN(v))
      this._buf.Append('.');
    if (this._opts.FieldWidth == 0)
      return;
    for (int index = 0; index < this._buf.Length; ++index)
    {
      if (this._buf[index] == ' ' || this._buf[index] == '0')
      {
        this._buf.Remove(index, 1);
        break;
      }
      if (this._buf[index] != '-' && this._buf[index] != '+')
        break;
    }
  }

  private void FixupFloatMinus()
  {
    bool flag = true;
    for (int index = 0; index < this._buf.Length; ++index)
    {
      if (this._buf[index] != '.' && this._buf[index] != '0' && this._buf[index] != ' ')
      {
        flag = false;
        break;
      }
    }
    if (!flag)
      return;
    if (this._opts.FieldWidth != 0)
    {
      if (this._buf[this._buf.Length - 1] == ' ')
      {
        this._buf.Insert(0, "-");
        this._buf.Remove(this._buf.Length - 1, 1);
      }
      else
      {
        int index = 0;
        while (this._buf[index] == ' ')
          ++index;
        if (index > 0)
          --index;
        this._buf[index] = '-';
      }
    }
    else
      this._buf.Insert(0, "-");
  }

  private void AppendZeroPad(object val, bool fPos, char format)
  {
    if (fPos && (this._opts.SignChar || this._opts.Space))
    {
      string str = string.Format((IFormatProvider) this._nfi, $"{{0:{format.ToString()}{this._opts.FieldWidth.ToString()}}}", val);
      char ch = this._opts.SignChar ? '+' : ' ';
      this._buf.Append(str[0] != '0' || str.Length <= 1 ? ch.ToString() + str : ch.ToString() + str.Substring(1));
    }
    else
    {
      string str = string.Format((IFormatProvider) this._nfi, $"{{0:{format.ToString()}{this._opts.FieldWidth.ToString()}}}", val);
      if (str[0] == '-')
      {
        this._buf.Append("-");
        if (str[1] != '0')
          this._buf.Append(str.Substring(1));
        else
          this._buf.Append(str.Substring(2));
      }
      else
        this._buf.Append(str);
    }
  }

  private void AppendZeroPadFloat(double val, char format)
  {
    if (val >= 0.0)
    {
      StringBuilder stringBuilder = new StringBuilder(val.ToString(format.ToString(), (IFormatProvider) this._nfi));
      if (stringBuilder.Length < this._opts.FieldWidth)
        stringBuilder.Insert(0, new string('0', this._opts.FieldWidth - stringBuilder.Length));
      if (this._opts.SignChar || this._opts.Space)
      {
        char ch = this._opts.SignChar ? '+' : ' ';
        if (stringBuilder[0] == '0' && stringBuilder[1] != '.')
          stringBuilder[0] = ch;
        else
          stringBuilder.Insert(0, ch.ToString());
      }
      this._buf.Append((object) stringBuilder);
    }
    else
    {
      StringBuilder stringBuilder = new StringBuilder(val.ToString(format.ToString(), (IFormatProvider) this._nfi));
      if (stringBuilder.Length < this._opts.FieldWidth)
        stringBuilder.Insert(1, new string('0', this._opts.FieldWidth - stringBuilder.Length));
      this._buf.Append((object) stringBuilder);
    }
  }

  private void AppendNumeric(object val, bool fPos, char format)
  {
    if (format == 'e' || format == 'E')
      this.AppendNumericExp(val, fPos, format);
    else
      this.AppendNumericDecimal(val, fPos, format);
  }

  private void AppendNumericExp(object val, bool fPos, char format)
  {
    bool flag = false;
    if (StringFormatter.IsNegativeZero((double) val))
      flag = true;
    if (fPos && (this._opts.SignChar || this._opts.Space))
    {
      string str = this.adjustExponent((this._opts.SignChar ? "+" : " ") + string.Format((IFormatProvider) this._nfi, $"{{0:{format.ToString()}{(object) this._opts.Precision}}}", val));
      if (str.Length < this._opts.FieldWidth)
        this._buf.Append(' ', this._opts.FieldWidth - str.Length);
      if (flag)
        this._buf.Append('-');
      this._buf.Append(str);
    }
    else if (this._opts.Precision < 100)
    {
      string str = this.adjustExponent(string.Format((IFormatProvider) this._nfi, $"{{0,{(object) this._opts.FieldWidth}:{format.ToString()}{(object) this._opts.Precision}}}", val));
      if (str.Length < this._opts.FieldWidth)
        this._buf.Append(' ', this._opts.FieldWidth - str.Length);
      if (flag)
        this._buf.Append('-');
      this._buf.Append(str);
    }
    else
      this.AppendNumericCommon(val, format);
  }

  private void AppendNumericDecimal(object val, bool fPos, char format)
  {
    if (fPos && (this._opts.SignChar || this._opts.Space))
    {
      string str = (this._opts.SignChar ? "+" : " ") + string.Format((IFormatProvider) this._nfi, $"{{0:{format.ToString()}}}", val);
      if (str.Length < this._opts.FieldWidth)
        this._buf.Append(' ', this._opts.FieldWidth - str.Length);
      this._buf.Append(str);
    }
    else if (this._opts.Precision == -1)
      this._buf.AppendFormat((IFormatProvider) this._nfi, $"{{0,{(object) this._opts.FieldWidth}:{format.ToString()}}}", val);
    else if (this._opts.Precision < 100)
      this._buf.Append(string.Format((IFormatProvider) this._nfi, $"{{0,{(object) this._opts.FieldWidth}:{format.ToString()}{(object) this._opts.Precision}}}", val));
    else
      this.AppendNumericCommon(val, format);
    if (!this._TrailingZeroAfterWholeFloat || format != 'f' && format != 'F' || this._opts.Precision != 0)
      return;
    this._buf.Append(".0");
  }

  private void AppendNumericCommon(object val, char format)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendFormat($"{{0:{format.ToString()}}}", val);
    if (stringBuilder.Length < this._opts.Precision)
      stringBuilder.Insert(0, new string('0', this._opts.Precision - stringBuilder.Length));
    if (stringBuilder.Length < this._opts.FieldWidth)
      stringBuilder.Insert(0, new string(' ', this._opts.FieldWidth - stringBuilder.Length));
    this._buf.Append(stringBuilder.ToString());
  }

  private string adjustExponent(string val)
  {
    return val[val.Length - 3] == '0' ? val.Substring(0, val.Length - 3) + val.Substring(val.Length - 2, 2) : val;
  }

  private void AppendLeftAdj(object val, bool fPos, char type)
  {
    string str1;
    if (type != 'e' && type != 'E')
      str1 = string.Format((IFormatProvider) this._nfi, $"{{0:{type.ToString()}}}", val);
    else
      str1 = this.adjustExponent(string.Format((IFormatProvider) this._nfi, $"{{0:{type.ToString()}{(object) this._opts.Precision}}}", val));
    string str2 = str1;
    if (fPos)
    {
      if (this._opts.SignChar)
        str2 = "+" + str2;
      else if (this._opts.Space)
        str2 = " " + str2;
    }
    this._buf.Append(str2);
    if (str2.Length >= this._opts.FieldWidth)
      return;
    this._buf.Append(' ', this._opts.FieldWidth - str2.Length);
  }

  private static bool NeedsAltForm(char format, char last)
  {
    return format == 'X' || format == 'x' || last != '0';
  }

  private static string GetAltFormPrefixForRadix(char format, int radix)
  {
    if (radix == 8)
      return "0";
    return radix == 16 /*0x10*/ ? format.ToString() + "0" : "";
  }

  private void AppendBase(char format, int radix)
  {
    object integerValue = this.GetIntegerValue(out bool _);
    if (integerValue is BigInteger origVal)
    {
      this.AppendBaseBigInt(origVal, format, radix);
    }
    else
    {
      int num1 = (int) integerValue;
      int num2 = num1;
      if (num2 < 0)
      {
        num2 *= -1;
        this._opts.Space = false;
      }
      StringBuilder stringBuilder = new StringBuilder();
      if (num2 == 0)
        stringBuilder.Append('0');
      for (; num2 != 0; num2 /= radix)
      {
        int num3 = num2 % radix;
        if (num3 < 10)
          stringBuilder.Append((char) (num3 + 48 /*0x30*/));
        else if (char.IsLower(format))
          stringBuilder.Append((char) (num3 - 10 + 97));
        else
          stringBuilder.Append((char) (num3 - 10 + 65));
      }
      if (stringBuilder.Length < this._opts.Precision)
      {
        int repeatCount = this._opts.Precision - stringBuilder.Length;
        stringBuilder.Append('0', repeatCount);
      }
      if (this._opts.FieldWidth != 0)
      {
        int num4 = num1 < 0 || this._opts.SignChar ? 1 : 0;
        int num5 = this._opts.Space ? 1 : 0;
        int num6 = this._opts.FieldWidth - (stringBuilder.Length + num4 + num5);
        if (num6 > 0)
        {
          if (this._opts.AltForm && StringFormatter.NeedsAltForm(format, this._opts.LeftAdj || !this._opts.ZeroPad ? stringBuilder[stringBuilder.Length - 1] : '0'))
            num6 -= StringFormatter.GetAltFormPrefixForRadix(format, radix).Length;
          if (num6 > 0)
          {
            if (this._opts.LeftAdj)
              stringBuilder.Insert(0, " ", num6);
            else if (this._opts.ZeroPad)
              stringBuilder.Append('0', num6);
            else
              this._buf.Append(' ', num6);
          }
        }
      }
      if (this._opts.AltForm && StringFormatter.NeedsAltForm(format, stringBuilder[stringBuilder.Length - 1]))
        stringBuilder.Append(StringFormatter.GetAltFormPrefixForRadix(format, radix));
      if (num1 < 0)
        this._buf.Append('-');
      else if (this._opts.SignChar)
        this._buf.Append('+');
      else if (this._opts.Space)
        this._buf.Append(' ');
      for (int index = stringBuilder.Length - 1; index >= 0; --index)
        this._buf.Append(stringBuilder[index]);
    }
  }

  private void AppendBaseBigInt(BigInteger origVal, char format, int radix)
  {
    BigInteger bigInteger = origVal;
    if (bigInteger < 0L)
      bigInteger *= (BigInteger) -1;
    StringBuilder str = new StringBuilder();
    switch (radix)
    {
      case 10:
        StringFormatter.AppendNumberReversed(str, bigInteger.ToString());
        break;
      case 16 /*0x10*/:
        StringFormatter.AppendNumberReversed(str, char.IsLower(format) ? bigInteger.ToString("x") : bigInteger.ToString("X"));
        break;
      default:
        if (bigInteger == 0L)
          str.Append('0');
        while (bigInteger != 0L)
        {
          int num = (int) (bigInteger % (BigInteger) radix);
          if (num < 10)
            str.Append((char) (num + 48 /*0x30*/));
          else if (char.IsLower(format))
            str.Append((char) (num - 10 + 97));
          else
            str.Append((char) (num - 10 + 65));
          bigInteger /= (BigInteger) radix;
        }
        break;
    }
    if (str.Length < this._opts.Precision)
    {
      int repeatCount = this._opts.Precision - str.Length;
      str.Append('0', repeatCount);
    }
    if (this._opts.FieldWidth != 0)
    {
      int num1 = origVal < 0L || this._opts.SignChar ? 1 : 0;
      int num2 = this._opts.FieldWidth - (str.Length + num1);
      if (num2 > 0)
      {
        if (this._opts.AltForm && StringFormatter.NeedsAltForm(format, this._opts.LeftAdj || !this._opts.ZeroPad ? str[str.Length - 1] : '0'))
          num2 -= StringFormatter.GetAltFormPrefixForRadix(format, radix).Length;
        if (num2 > 0)
        {
          if (this._opts.LeftAdj)
            str.Insert(0, " ", num2);
          else if (this._opts.ZeroPad)
            str.Append('0', num2);
          else
            this._buf.Append(' ', num2);
        }
      }
    }
    if (this._opts.AltForm && StringFormatter.NeedsAltForm(format, str[str.Length - 1]))
      str.Append(StringFormatter.GetAltFormPrefixForRadix(format, radix));
    if (origVal < 0L)
      this._buf.Append('-');
    else if (this._opts.SignChar)
      this._buf.Append('+');
    else if (this._opts.Space)
      this._buf.Append(' ');
    for (int index = str.Length - 1; index >= 0; --index)
      this._buf.Append(str[index]);
  }

  private static void AppendNumberReversed(StringBuilder str, string res)
  {
    int index1 = 0;
    while (index1 < res.Length - 1 && res[index1] == '0')
      ++index1;
    for (int index2 = res.Length - 1; index2 >= index1; --index2)
      str.Append(res[index2]);
  }

  private void AppendHex(char format) => this.AppendBase(format, 16 /*0x10*/);

  private void AppendOctal() => this.AppendBase('o', 8);

  private void AppendString()
  {
    if (!this._isUnicodeString)
    {
      s = PythonOps.ToString(this._context, this._opts.Value);
    }
    else
    {
      object obj = StringOps.FastNewUnicode(this._context, this._opts.Value);
      if (!(obj is string s))
        s = ((Extensible<string>) obj).Value;
    }
    if (s == null)
      s = "None";
    this.AppendString(s);
  }

  private void AppendRepr() => this.AppendString(PythonOps.Repr(this._context, this._opts.Value));

  private void AppendString(string s)
  {
    if (this._opts.Precision != -1 && s.Length > this._opts.Precision)
      s = s.Substring(0, this._opts.Precision);
    if (!this._opts.LeftAdj && this._opts.FieldWidth > s.Length)
      this._buf.Append(' ', this._opts.FieldWidth - s.Length);
    this._buf.Append(s);
    if (!this._opts.LeftAdj || this._opts.FieldWidth <= s.Length)
      return;
    this._buf.Append(' ', this._opts.FieldWidth - s.Length);
  }

  internal static bool IsNegativeZero(double x)
  {
    return BitConverter.DoubleToInt64Bits(x) == StringFormatter.NegativeZeroBits;
  }

  [Flags]
  internal enum FormatOptions
  {
    ZeroPad = 1,
    LeftAdj = 2,
    AltForm = 4,
    Space = 8,
    SignChar = 16, // 0x00000010
  }

  internal struct FormatSettings
  {
    internal StringFormatter.FormatOptions Options;
    internal int FieldWidth;
    internal int Precision;
    internal object Value;

    public bool ZeroPad
    {
      get => (this.Options & StringFormatter.FormatOptions.ZeroPad) != 0;
      set
      {
        if (value)
          this.Options |= StringFormatter.FormatOptions.ZeroPad;
        else
          this.Options &= ~StringFormatter.FormatOptions.ZeroPad;
      }
    }

    public bool LeftAdj
    {
      get => (this.Options & StringFormatter.FormatOptions.LeftAdj) != 0;
      set
      {
        if (value)
          this.Options |= StringFormatter.FormatOptions.LeftAdj;
        else
          this.Options &= ~StringFormatter.FormatOptions.LeftAdj;
      }
    }

    public bool AltForm
    {
      get => (this.Options & StringFormatter.FormatOptions.AltForm) != 0;
      set
      {
        if (value)
          this.Options |= StringFormatter.FormatOptions.AltForm;
        else
          this.Options &= ~StringFormatter.FormatOptions.AltForm;
      }
    }

    public bool Space
    {
      get => (this.Options & StringFormatter.FormatOptions.Space) != 0;
      set
      {
        if (value)
          this.Options |= StringFormatter.FormatOptions.Space;
        else
          this.Options &= ~StringFormatter.FormatOptions.Space;
      }
    }

    public bool SignChar
    {
      get => (this.Options & StringFormatter.FormatOptions.SignChar) != 0;
      set
      {
        if (value)
          this.Options |= StringFormatter.FormatOptions.SignChar;
        else
          this.Options &= ~StringFormatter.FormatOptions.SignChar;
      }
    }
  }
}
