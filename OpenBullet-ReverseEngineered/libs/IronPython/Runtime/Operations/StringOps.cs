// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.StringOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class StringOps
{
  internal const int LowestUnicodeValue = 127 /*0x7F*/;
  private static CultureInfo CasingCultureInfo = new CultureInfo("en");
  private static DecoderFallback ReplacementFallback = (DecoderFallback) new DecoderReplacementFallback("�");

  internal static object FastNew(CodeContext context, object x)
  {
    if (x == null)
      return (object) "None";
    if (x is string str)
      return (object) str;
    object o = PythonContext.InvokeUnaryOperator(context, UnaryOperators.String, x);
    switch (o)
    {
      case string _:
      case Extensible<string> _:
        return o;
      default:
        throw PythonOps.TypeError("expected str, got {0} from __str__", (object) DynamicHelpers.GetPythonType(o).Name);
    }
  }

  internal static string FastNewUnicode(
    CodeContext context,
    object value,
    object encoding,
    object errors)
  {
    if (!(errors is string errors1))
      throw PythonOps.TypeError("unicode() argument 3 must be string, not {0}", (object) PythonTypeOps.GetName(errors));
    switch (value)
    {
      case string s:
        return StringOps.RawDecode(context, s, encoding, errors1);
      case Extensible<string> extensible:
        return StringOps.RawDecode(context, extensible.Value, encoding, errors1);
      case Bytes bytes:
        return StringOps.RawDecode(context, bytes.ToString(), encoding, errors1);
      case PythonBuffer pythonBuffer:
        return StringOps.RawDecode(context, pythonBuffer.ToString(), encoding, errors1);
      default:
        throw PythonOps.TypeError("coercing to Unicode: need string or buffer, {0} found", (object) PythonTypeOps.GetName(value));
    }
  }

  internal static object FastNewUnicode(CodeContext context, object value, object encoding)
  {
    return (object) StringOps.FastNewUnicode(context, value, encoding, (object) "strict");
  }

  internal static object FastNewUnicode(CodeContext context, object value)
  {
    if (value == null)
      return (object) "None";
    if (value is string)
      return value;
    object func;
    if (value is OldInstance oldInstance && (oldInstance.TryGetBoundCustomMember(context, "__unicode__", out func) || oldInstance.TryGetBoundCustomMember(context, "__str__", out func)))
    {
      func = context.LanguageContext.Call(context, func);
      switch (func)
      {
        case string _:
        case Extensible<string> _:
          return func;
        default:
          throw PythonOps.TypeError("coercing to Unicode: expected string, got {0}", (object) PythonTypeOps.GetName(value));
      }
    }
    else
    {
      if (!PythonTypeOps.TryInvokeUnaryOperator(context, value, "__unicode__", out func) && !PythonTypeOps.TryInvokeUnaryOperator(context, value, "__str__", out func))
        return (object) StringOps.FastNewUnicode(context, value, (object) context.LanguageContext.DefaultEncoding.WebName, (object) "strict");
      switch (func)
      {
        case string _:
        case Extensible<string> _:
          return func;
        default:
          throw PythonOps.TypeError("coercing to Unicode: expected string, got {0}", (object) PythonTypeOps.GetName(value));
      }
    }
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls)
  {
    return cls == TypeCache.String ? (object) "" : cls.CreateInstance(context);
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, object @object)
  {
    return cls == TypeCache.String ? StringOps.FastNew(context, @object) : cls.CreateInstance(context, StringOps.__new__(context, TypeCache.String, @object));
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, [NotNull] string @object)
  {
    return cls == TypeCache.String ? (object) @object : cls.CreateInstance(context, (object) @object);
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, [NotNull] ExtensibleString @object)
  {
    return cls == TypeCache.String ? StringOps.FastNew(context, (object) @object) : cls.CreateInstance(context, StringOps.__new__(context, TypeCache.String, @object));
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, char @object)
  {
    return cls == TypeCache.String ? (object) ScriptingRuntimeHelpers.CharToString(@object) : cls.CreateInstance(context, StringOps.__new__(context, TypeCache.String, @object));
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, [NotNull] BigInteger @object)
  {
    return cls == TypeCache.String ? (object) @object.ToString() : cls.CreateInstance(context, StringOps.__new__(context, TypeCache.String, @object));
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, [NotNull] Extensible<BigInteger> @object)
  {
    return cls == TypeCache.String ? StringOps.FastNew(context, (object) @object) : cls.CreateInstance(context, StringOps.__new__(context, TypeCache.String, @object));
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, int @object)
  {
    return cls == TypeCache.String ? (object) @object.ToString() : cls.CreateInstance(context, StringOps.__new__(context, TypeCache.String, @object));
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, bool @object)
  {
    return cls == TypeCache.String ? (object) @object.ToString() : cls.CreateInstance(context, StringOps.__new__(context, TypeCache.String, @object));
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, double @object)
  {
    return cls == TypeCache.String ? (object) DoubleOps.__str__(context, @object) : cls.CreateInstance(context, StringOps.__new__(context, TypeCache.String, @object));
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, Extensible<double> @object)
  {
    return cls == TypeCache.String ? StringOps.FastNew(context, (object) @object) : cls.CreateInstance(context, StringOps.__new__(context, TypeCache.String, @object));
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, float @object)
  {
    return cls == TypeCache.String ? (object) SingleOps.__str__(context, @object) : cls.CreateInstance(context, StringOps.__new__(context, TypeCache.String, @object));
  }

  [StaticExtensionMethod]
  public static object __new__(
    CodeContext context,
    PythonType cls,
    object @string,
    string encoding = null,
    string errors = "strict")
  {
    if (!(@string is string str))
      throw PythonOps.TypeError("converting to unicode: need string, got {0}", (object) DynamicHelpers.GetPythonType(@string).Name);
    return cls == TypeCache.String ? (object) StringOps.decode(context, str, (object) (encoding ?? context.LanguageContext.GetDefaultEncodingName()), errors) : cls.CreateInstance(context, StringOps.__new__(context, TypeCache.String, (object) str, encoding, errors));
  }

  public static bool __contains__(string s, [BytesConversion] string item) => s.Contains(item);

  public static bool __contains__(string s, char item) => s.IndexOf(item) != -1;

  public static string __format__(CodeContext context, string self, [BytesConversion] string formatSpec)
  {
    return ObjectOps.__format__(context, (object) self, formatSpec);
  }

  public static int __len__(string s) => s.Length;

  [SpecialName]
  public static string GetItem(string s, int index)
  {
    return ScriptingRuntimeHelpers.CharToString(s[PythonOps.FixIndex(index, s.Length)]);
  }

  [SpecialName]
  public static string GetItem(string s, object index)
  {
    return StringOps.GetItem(s, Converter.ConvertToIndex(index));
  }

  [SpecialName]
  public static string GetItem(string s, Slice slice)
  {
    if (slice == null)
      throw PythonOps.TypeError("string indices must be slices or integers");
    int ostart;
    int ostop;
    int ostep;
    slice.indices(s.Length, out ostart, out ostop, out ostep);
    if (ostep == 1)
      return ostop <= ostart ? string.Empty : s.Substring(ostart, ostop - ostart);
    int num = 0;
    char[] chArray;
    if (ostep > 0)
    {
      if (ostart > ostop)
        return string.Empty;
      chArray = new char[(ostop - ostart + ostep - 1) / ostep];
      for (int index = ostart; index < ostop; index += ostep)
        chArray[num++] = s[index];
    }
    else
    {
      if (ostart < ostop)
        return string.Empty;
      chArray = new char[(ostop - ostart + ostep + 1) / ostep];
      for (int index = ostart; index > ostop; index += ostep)
        chArray[num++] = s[index];
    }
    return new string(chArray);
  }

  public static string __getslice__(string self, int x, int y)
  {
    Slice.FixSliceArguments(self.Length, ref x, ref y);
    return x >= y ? string.Empty : self.Substring(x, y - x);
  }

  public static string capitalize(this string self)
  {
    return self.Length == 0 ? self : char.ToUpperInvariant(self[0]).ToString() + self.Substring(1).ToLowerInvariant();
  }

  public static string center(this string self, int width) => self.center(width, ' ');

  public static string center(this string self, int width, char fillchar)
  {
    int num = width - self.Length;
    if (num <= 0)
      return self;
    StringBuilder stringBuilder = new StringBuilder(width);
    stringBuilder.Append(fillchar, num / 2);
    stringBuilder.Append(self);
    stringBuilder.Append(fillchar, (num + 1) / 2);
    return stringBuilder.ToString();
  }

  public static int count(this string self, [BytesConversion] string sub)
  {
    return self.count(sub, 0, self.Length);
  }

  public static int count(this string self, [BytesConversion] string sub, int start)
  {
    return self.count(sub, start, self.Length);
  }

  public static int count(this string self, [BytesConversion] string ssub, int start, int end)
  {
    if (ssub == null)
      throw PythonOps.TypeError("expected string for 'sub' argument, got NoneType");
    if (start > self.Length)
      return 0;
    start = PythonOps.FixSliceIndex(start, self.Length);
    end = PythonOps.FixSliceIndex(end, self.Length);
    if (ssub.Length == 0)
      return Math.Max(end - start + 1, 0);
    int num1 = 0;
    CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
    int num2;
    for (; end > start; start = num2 + ssub.Length)
    {
      num2 = compareInfo.IndexOf(self, ssub, start, end - start, CompareOptions.Ordinal);
      if (num2 != -1)
        ++num1;
      else
        break;
    }
    return num1;
  }

  public static string decode(CodeContext context, string s)
  {
    return StringOps.decode(context, s, (object) Missing.Value, "strict");
  }

  public static string decode(CodeContext context, string s, [Optional] object encoding, string errors = "strict")
  {
    return StringOps.RawDecode(context, s, encoding, errors);
  }

  public static string encode(CodeContext context, string s, [Optional] object encoding, string errors = "strict")
  {
    return StringOps.RawEncode(context, s, encoding, errors);
  }

  private static string CastString(object o)
  {
    return o is string str ? str : ((Extensible<string>) o).Value;
  }

  internal static string AsString(object o)
  {
    switch (o)
    {
      case string str:
        return str;
      case Extensible<string> extensible:
        return extensible.Value;
      default:
        return (string) null;
    }
  }

  public static bool endswith(this string self, object suffix)
  {
    StringOps.TryStringOrTuple(suffix);
    return suffix is PythonTuple ? StringOps.endswith(self, (PythonTuple) suffix) : StringOps.endswith(self, StringOps.CastString(suffix));
  }

  public static bool endswith(this string self, object suffix, int start)
  {
    StringOps.TryStringOrTuple(suffix);
    return suffix is PythonTuple ? StringOps.endswith(self, (PythonTuple) suffix, start) : StringOps.endswith(self, StringOps.CastString(suffix), start);
  }

  public static bool endswith(this string self, object suffix, int start, int end)
  {
    StringOps.TryStringOrTuple(suffix);
    return suffix is PythonTuple ? StringOps.endswith(self, (PythonTuple) suffix, start, end) : StringOps.endswith(self, StringOps.CastString(suffix), start, end);
  }

  public static string expandtabs(string self) => self.expandtabs(8);

  public static string expandtabs(this string self, int tabsize)
  {
    StringBuilder stringBuilder = new StringBuilder(self.Length * 2);
    string str = self;
    int num = 0;
    for (int index = 0; index < str.Length; ++index)
    {
      char ch = str[index];
      switch (ch)
      {
        case '\t':
          if (tabsize > 0)
          {
            int repeatCount = tabsize - num % tabsize;
            int capacity = stringBuilder.Capacity;
            stringBuilder.Capacity = checked (capacity + repeatCount);
            stringBuilder.Append(' ', repeatCount);
            num = 0;
            break;
          }
          break;
        case '\n':
        case '\r':
          num = 0;
          stringBuilder.Append(ch);
          break;
        default:
          ++num;
          stringBuilder.Append(ch);
          break;
      }
    }
    return stringBuilder.ToString();
  }

  public static int find(this string self, [BytesConversion] string sub)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    return sub.Length == 1 ? self.IndexOf(sub[0]) : CultureInfo.InvariantCulture.CompareInfo.IndexOf(self, sub, CompareOptions.Ordinal);
  }

  public static int find(this string self, [BytesConversion] string sub, int start)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    if (start > self.Length)
      return -1;
    start = PythonOps.FixSliceIndex(start, self.Length);
    return CultureInfo.InvariantCulture.CompareInfo.IndexOf(self, sub, start, CompareOptions.Ordinal);
  }

  public static int find(this string self, [BytesConversion] string sub, BigInteger start)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    return start > (long) self.Length ? -1 : self.find(sub, (int) start);
  }

  public static int find(this string self, [BytesConversion] string sub, int start, int end)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    if (start > self.Length)
      return -1;
    start = PythonOps.FixSliceIndex(start, self.Length);
    end = PythonOps.FixSliceIndex(end, self.Length);
    return end < start ? -1 : CultureInfo.InvariantCulture.CompareInfo.IndexOf(self, sub, start, end - start, CompareOptions.Ordinal);
  }

  public static int find(this string self, [BytesConversion] string sub, BigInteger start, BigInteger end)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    return start > (long) self.Length ? -1 : StringOps.find(self, sub, (int) start, (int) end);
  }

  public static int find(this string self, [BytesConversion] string sub, object start, object end = null)
  {
    return StringOps.find(self, sub, StringOps.CheckIndex(start, 0), StringOps.CheckIndex(end, self.Length));
  }

  public static int index(this string self, [BytesConversion] string sub)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    return StringOps.index(self, sub, 0, self.Length);
  }

  public static int index(this string self, [BytesConversion] string sub, int start)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    return StringOps.index(self, sub, start, self.Length);
  }

  public static int index(this string self, [BytesConversion] string sub, int start, int end)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    int num = StringOps.find(self, sub, start, end);
    return num != -1 ? num : throw PythonOps.ValueError("substring {0} not found in {1}", (object) sub, (object) self);
  }

  public static int index(this string self, [BytesConversion] string sub, object start, object end = null)
  {
    return StringOps.index(self, sub, StringOps.CheckIndex(start, 0), StringOps.CheckIndex(end, self.Length));
  }

  public static bool isalnum(this string self)
  {
    if (self.Length == 0)
      return false;
    string s = self;
    for (int index = s.Length - 1; index >= 0; --index)
    {
      if (!char.IsLetterOrDigit(s, index))
        return false;
    }
    return true;
  }

  public static bool isalpha(this string self)
  {
    if (self.Length == 0)
      return false;
    string s = self;
    for (int index = s.Length - 1; index >= 0; --index)
    {
      if (!char.IsLetter(s, index))
        return false;
    }
    return true;
  }

  public static bool isdigit(this string self)
  {
    if (self.Length == 0)
      return false;
    string s = self;
    for (int index = s.Length - 1; index >= 0; --index)
    {
      if (!char.IsDigit(s, index) && (s[index] < '\u2460' || s[index] > '\u2468'))
        return false;
    }
    return true;
  }

  public static bool isspace(this string self)
  {
    if (self.Length == 0)
      return false;
    string s = self;
    for (int index = s.Length - 1; index >= 0; --index)
    {
      if (!char.IsWhiteSpace(s, index))
        return false;
    }
    return true;
  }

  public static bool isdecimal(this string self) => self.isnumeric();

  public static bool isnumeric(this string self)
  {
    if (string.IsNullOrEmpty(self))
      return false;
    foreach (char c in self)
    {
      if (!char.IsDigit(c))
        return false;
    }
    return true;
  }

  public static bool islower(this string self)
  {
    if (self.Length == 0)
      return false;
    string s = self;
    bool flag = false;
    for (int index = s.Length - 1; index >= 0; --index)
    {
      if (!flag && char.IsLower(s, index))
        flag = true;
      if (char.IsUpper(s, index))
        return false;
    }
    return flag;
  }

  public static bool isupper(this string self)
  {
    if (self.Length == 0)
      return false;
    string s = self;
    bool flag = false;
    for (int index = s.Length - 1; index >= 0; --index)
    {
      if (!flag && char.IsUpper(s, index))
        flag = true;
      if (char.IsLower(s, index))
        return false;
    }
    return flag;
  }

  public static bool istitle(this string self)
  {
    if (self == null || self.Length == 0)
      return false;
    string s = self;
    bool flag1 = false;
    bool flag2 = false;
    for (int index = 0; index < s.Length; ++index)
    {
      bool flag3;
      if (char.IsUpper(s, index) || CharUnicodeInfo.GetUnicodeCategory(s, index) == UnicodeCategory.TitlecaseLetter)
      {
        flag2 = true;
        if (flag1)
          return false;
        flag3 = true;
      }
      else if (char.IsLower(s, index))
      {
        if (!flag1)
          return false;
        flag3 = true;
      }
      else
        flag3 = false;
      flag1 = flag3;
    }
    return flag2;
  }

  public static bool isunicode(this string self)
  {
    return self.Any<char>((Func<char, bool>) (c => c >= '\u007F'));
  }

  public static string join(this string self, object sequence)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(sequence);
    if (!enumerator.MoveNext())
      return "";
    object current = enumerator.Current;
    if (!enumerator.MoveNext())
      return Converter.ConvertToString(current);
    StringBuilder sb = new StringBuilder();
    StringOps.AppendJoin(current, 0, sb);
    int index = 1;
    do
    {
      sb.Append(self);
      StringOps.AppendJoin(enumerator.Current, index, sb);
      ++index;
    }
    while (enumerator.MoveNext());
    return sb.ToString();
  }

  public static string join(this string self, [NotNull] List sequence)
  {
    if (sequence.__len__() == 0)
      return string.Empty;
    lock (sequence)
    {
      if (sequence.__len__() == 1)
        return Converter.ConvertToString(sequence[0]);
      StringBuilder sb = new StringBuilder();
      StringOps.AppendJoin(sequence._data[0], 0, sb);
      for (int index = 1; index < sequence._size; ++index)
      {
        if (!string.IsNullOrEmpty(self))
          sb.Append(self);
        StringOps.AppendJoin(sequence._data[index], index, sb);
      }
      return sb.ToString();
    }
  }

  public static string ljust(this string self, int width) => self.ljust(width, ' ');

  public static string ljust(this string self, int width, char fillchar)
  {
    if (width < 0)
      return self;
    int repeatCount = width - self.Length;
    if (repeatCount <= 0)
      return self;
    StringBuilder stringBuilder = new StringBuilder(width);
    stringBuilder.Append(self);
    stringBuilder.Append(fillchar, repeatCount);
    return stringBuilder.ToString();
  }

  public static string lower(this string self)
  {
    return StringOps.CasingCultureInfo.TextInfo.ToLower(self);
  }

  internal static string ToLowerAsciiTriggered(this string self)
  {
    for (int index = 0; index < self.Length; ++index)
    {
      if (self[index] >= 'A' && self[index] <= 'Z')
        return self.ToLowerInvariant();
    }
    return self;
  }

  public static string lstrip(this string self) => self.TrimStart();

  public static string lstrip(this string self, [BytesConversion] string chars)
  {
    return chars == null ? self.lstrip() : self.TrimStart(chars.ToCharArray());
  }

  [return: SequenceTypeInfo(new Type[] {typeof (string)})]
  public static PythonTuple partition(this string self, [BytesConversion] string sep)
  {
    switch (sep)
    {
      case null:
        throw PythonOps.TypeError("expected string, got NoneType");
      case "":
        throw PythonOps.ValueError("empty separator");
      default:
        object[] o = new object[3]
        {
          (object) "",
          (object) "",
          (object) ""
        };
        if (self.Length != 0)
        {
          int length = self.find(sep);
          if (length == -1)
          {
            o[0] = (object) self;
          }
          else
          {
            o[0] = (object) self.Substring(0, length);
            o[1] = (object) sep;
            o[2] = (object) self.Substring(length + sep.Length, self.Length - length - sep.Length);
          }
        }
        return new PythonTuple((object) o);
    }
  }

  public static string replace(this string self, [BytesConversion] string old, [BytesConversion] string @new, int count = -1)
  {
    switch (old)
    {
      case null:
        throw PythonOps.TypeError("expected a character buffer object");
      case "":
        return StringOps.ReplaceEmpty(self, @new, count);
      default:
        string self1 = self;
        int num1 = self1.count(old);
        int num2 = count < 0 || count > num1 ? num1 : count;
        StringBuilder stringBuilder = new StringBuilder(checked (unchecked (self1.Length - num2 * old.Length) + num2 * @new.Length));
        int startIndex;
        int num3;
        for (startIndex = 0; count != 0 && (num3 = self1.IndexOf(old, startIndex, StringComparison.Ordinal)) != -1; --count)
        {
          stringBuilder.Append(self1, startIndex, num3 - startIndex);
          stringBuilder.Append(@new);
          startIndex = num3 + old.Length;
        }
        stringBuilder.Append(self1.Substring(startIndex));
        return stringBuilder.ToString();
    }
  }

  public static int rfind(this string self, [BytesConversion] string sub)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    return StringOps.rfind(self, sub, 0, self.Length);
  }

  public static int rfind(this string self, [BytesConversion] string sub, int start)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    return start > self.Length ? -1 : StringOps.rfind(self, sub, start, self.Length);
  }

  public static int rfind(this string self, [BytesConversion] string sub, BigInteger start)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    return start > (long) self.Length ? -1 : StringOps.rfind(self, sub, (int) start, self.Length);
  }

  public static int rfind(this string self, [BytesConversion] string sub, int start, int end)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    if (start > self.Length)
      return -1;
    start = PythonOps.FixSliceIndex(start, self.Length);
    end = PythonOps.FixSliceIndex(end, self.Length);
    if (start > end)
      return -1;
    if (sub.Length == 0)
      return end;
    return end == 0 ? -1 : CultureInfo.InvariantCulture.CompareInfo.LastIndexOf(self, sub, end - 1, end - start, CompareOptions.Ordinal);
  }

  public static int rfind(this string self, [BytesConversion] string sub, BigInteger start, BigInteger end)
  {
    if (sub == null)
      throw PythonOps.TypeError("expected string, got NoneType");
    return start > (long) self.Length ? -1 : StringOps.rfind(self, sub, (int) start, (int) end);
  }

  public static int rfind(this string self, [BytesConversion] string sub, object start, object end = null)
  {
    return StringOps.rfind(self, sub, StringOps.CheckIndex(start, 0), StringOps.CheckIndex(end, self.Length));
  }

  public static int rindex(this string self, [BytesConversion] string sub)
  {
    return StringOps.rindex(self, sub, 0, self.Length);
  }

  public static int rindex(this string self, [BytesConversion] string sub, int start)
  {
    return StringOps.rindex(self, sub, start, self.Length);
  }

  public static int rindex(this string self, [BytesConversion] string sub, int start, int end)
  {
    int num = StringOps.rfind(self, sub, start, end);
    return num != -1 ? num : throw PythonOps.ValueError("substring {0} not found in {1}", (object) sub, (object) self);
  }

  public static int rindex(this string self, [BytesConversion] string sub, object start, object end = null)
  {
    return StringOps.rindex(self, sub, StringOps.CheckIndex(start, 0), StringOps.CheckIndex(end, self.Length));
  }

  public static string rjust(this string self, int width) => self.rjust(width, ' ');

  public static string rjust(this string self, int width, char fillchar)
  {
    int repeatCount = width - self.Length;
    if (repeatCount <= 0)
      return self;
    StringBuilder stringBuilder = new StringBuilder(width);
    stringBuilder.Append(fillchar, repeatCount);
    stringBuilder.Append(self);
    return stringBuilder.ToString();
  }

  [return: SequenceTypeInfo(new Type[] {typeof (string)})]
  public static PythonTuple rpartition(this string self, [BytesConversion] string sep)
  {
    switch (sep)
    {
      case null:
        throw PythonOps.TypeError("expected string, got NoneType");
      case "":
        throw PythonOps.ValueError("empty separator");
      default:
        object[] o = new object[3]
        {
          (object) "",
          (object) "",
          (object) ""
        };
        if (self.Length != 0)
        {
          int length = self.rfind(sep);
          if (length == -1)
          {
            o[2] = (object) self;
          }
          else
          {
            o[0] = (object) self.Substring(0, length);
            o[1] = (object) sep;
            o[2] = (object) self.Substring(length + sep.Length, self.Length - length - sep.Length);
          }
        }
        return new PythonTuple((object) o);
    }
  }

  public static List rsplit(this string self) => StringOps.SplitInternal(self, (char[]) null, -1);

  public static List rsplit(this string self, [BytesConversion] string sep) => self.rsplit(sep, -1);

  public static List rsplit(this string self, [BytesConversion] string sep, int maxsplit)
  {
    string self1 = StringOps.Reverse(self);
    if (sep != null)
      sep = StringOps.Reverse(sep);
    string sep1 = sep;
    int maxsplit1 = maxsplit;
    List list1 = self1.split(sep1, maxsplit1);
    list1.reverse();
    int capacity = list1.__len__();
    List list2;
    if (capacity != 0)
    {
      list2 = new List(capacity);
      foreach (string s in list1)
        list2.AddNoLock((object) StringOps.Reverse(s));
    }
    else
      list2 = list1;
    return list2;
  }

  public static string rstrip(this string self) => self.TrimEnd();

  public static string rstrip(this string self, [BytesConversion] string chars)
  {
    return chars == null ? self.rstrip() : self.TrimEnd(chars.ToCharArray());
  }

  public static List split(this string self) => StringOps.SplitInternal(self, (char[]) null, -1);

  public static List split(this string self, [BytesConversion] string sep) => self.split(sep, -1);

  public static List split(this string self, [BytesConversion] string sep, int maxsplit)
  {
    switch (sep)
    {
      case null:
        if (maxsplit != 0)
          return StringOps.SplitInternal(self, (char[]) null, maxsplit);
        List list = PythonOps.MakeEmptyList(1);
        list.AddNoLock((object) self.TrimStart());
        return list;
      case "":
        throw PythonOps.ValueError("empty separator");
      default:
        if (sep.Length != 1)
          return StringOps.SplitInternal(self, sep, maxsplit);
        return StringOps.SplitInternal(self, new char[1]
        {
          sep[0]
        }, maxsplit);
    }
  }

  public static List splitlines(this string self) => self.splitlines(false);

  public static List splitlines(this string self, bool keepends)
  {
    List list = new List();
    int index = 0;
    int startIndex = 0;
    for (; index < self.Length; ++index)
    {
      if (self[index] == '\n' || self[index] == '\r' || self[index] == '\u2028')
      {
        if (index < self.Length - 1 && self[index] == '\r' && self[index + 1] == '\n')
        {
          if (keepends)
            list.AddNoLock((object) self.Substring(startIndex, index - startIndex + 2));
          else
            list.AddNoLock((object) self.Substring(startIndex, index - startIndex));
          startIndex = index + 2;
          ++index;
        }
        else
        {
          if (keepends)
            list.AddNoLock((object) self.Substring(startIndex, index - startIndex + 1));
          else
            list.AddNoLock((object) self.Substring(startIndex, index - startIndex));
          startIndex = index + 1;
        }
      }
    }
    if (index - startIndex != 0)
      list.AddNoLock((object) self.Substring(startIndex, index - startIndex));
    return list;
  }

  public static bool startswith(this string self, object prefix)
  {
    StringOps.TryStringOrTuple(prefix);
    return prefix is PythonTuple ? StringOps.startswith(self, (PythonTuple) prefix) : StringOps.startswith(self, StringOps.CastString(prefix));
  }

  public static bool startswith(this string self, object prefix, int start)
  {
    StringOps.TryStringOrTuple(prefix);
    return prefix is PythonTuple ? StringOps.startswith(self, (PythonTuple) prefix, start) : StringOps.startswith(self, StringOps.CastString(prefix), start);
  }

  public static bool startswith(this string self, object prefix, int start, int end)
  {
    StringOps.TryStringOrTuple(prefix);
    return prefix is PythonTuple ? StringOps.startswith(self, (PythonTuple) prefix, start, end) : StringOps.startswith(self, StringOps.CastString(prefix), start, end);
  }

  public static string strip(this string self) => self.Trim();

  public static string strip(this string self, [BytesConversion] string chars)
  {
    return chars == null ? self.strip() : self.Trim(chars.ToCharArray());
  }

  public static string swapcase(this string self)
  {
    StringBuilder stringBuilder = new StringBuilder(self);
    for (int index = 0; index < stringBuilder.Length; ++index)
    {
      char c = stringBuilder[index];
      if (char.IsUpper(c))
        stringBuilder[index] = char.ToLowerInvariant(c);
      else if (char.IsLower(c))
        stringBuilder[index] = char.ToUpperInvariant(c);
    }
    return stringBuilder.ToString();
  }

  public static string title(this string self)
  {
    if (self == null || self.Length == 0)
      return self;
    char[] charArray = self.ToCharArray();
    bool flag1 = false;
    int index = 0;
    do
    {
      bool flag2;
      if (char.IsUpper(charArray[index]) || char.IsLower(charArray[index]))
      {
        charArray[index] = flag1 ? char.ToLowerInvariant(charArray[index]) : char.ToUpperInvariant(charArray[index]);
        flag2 = true;
      }
      else
        flag2 = false;
      ++index;
      flag1 = flag2;
    }
    while (index < charArray.Length);
    return new string(charArray);
  }

  public static string translate(this string self, [NotNull] PythonDictionary table)
  {
    if (table == null || self.Length == 0)
      return self;
    StringBuilder stringBuilder = new StringBuilder();
    int index = 0;
    for (; index < self.Length; ++index)
    {
      int key = (int) self[index];
      if (table.__contains__((object) key))
      {
        object obj = table[(object) key];
        switch (obj)
        {
          case null:
            continue;
          case int num:
            if (num > (int) ushort.MaxValue)
              throw PythonOps.TypeError("character mapping must be in range(0x%lx)");
            stringBuilder.Append((char) (int) obj);
            continue;
          case string _:
            stringBuilder.Append(obj);
            continue;
          default:
            throw PythonOps.TypeError("character mapping must return integer, None or unicode");
        }
      }
      else
        stringBuilder.Append(self[index]);
    }
    return stringBuilder.ToString();
  }

  public static string translate(this string self, [BytesConversion] string table)
  {
    return self.translate(table, (string) null);
  }

  public static string translate(this string self, [BytesConversion] string table, [BytesConversion] string deletechars)
  {
    if (table != null && table.Length != 256 /*0x0100*/)
      throw PythonOps.ValueError("translation table must be 256 characters long");
    if (self.Length == 0)
      return self;
    List<char> charList = new List<char>();
    for (int index1 = 0; index1 < self.Length; ++index1)
    {
      if (deletechars == null || !deletechars.Contains(char.ToString(self[index1])))
      {
        if (table != null)
        {
          int index2 = (int) self[index1];
          if (index2 >= 0 && index2 < 256 /*0x0100*/)
            charList.Add(table[index2]);
        }
        else
          charList.Add(self[index1]);
      }
    }
    return new string(charList.ToArray());
  }

  public static string upper(this string self)
  {
    return StringOps.CasingCultureInfo.TextInfo.ToUpper(self);
  }

  public static string zfill(this string self, int width)
  {
    int repeatCount = width - self.Length;
    if (repeatCount <= 0)
      return self;
    StringBuilder stringBuilder = new StringBuilder(width);
    if (self.Length > 0 && StringOps.IsSign(self[0]))
    {
      stringBuilder.Append(self[0]);
      stringBuilder.Append('0', repeatCount);
      stringBuilder.Append(self.Substring(1));
    }
    else
    {
      stringBuilder.Append('0', repeatCount);
      stringBuilder.Append(self);
    }
    return stringBuilder.ToString();
  }

  public static string format(CodeContext context, string format_string, [NotNull] params object[] args)
  {
    return NewStringFormatter.FormatString(context.LanguageContext, format_string, PythonTuple.MakeTuple(args), (IDictionary<object, object>) new PythonDictionary());
  }

  public static string format(
    CodeContext context,
    string format_stringø,
    [ParamDictionary] IDictionary<object, object> kwargsø,
    params object[] argsø)
  {
    return NewStringFormatter.FormatString(context.LanguageContext, format_stringø, PythonTuple.MakeTuple(argsø), kwargsø);
  }

  public static IEnumerable<PythonTuple> _formatter_parser(this string self)
  {
    return NewStringFormatter.GetFormatInfo(self);
  }

  public static PythonTuple _formatter_field_name_split(this string self)
  {
    return NewStringFormatter.GetFieldNameInfo(self);
  }

  [SpecialName]
  public static string Add([NotNull] string self, [NotNull] string other) => self + other;

  [SpecialName]
  public static string Add([NotNull] string self, char other) => self + other.ToString();

  [SpecialName]
  public static string Add(char self, [NotNull] string other) => self.ToString() + other;

  [SpecialName]
  public static string Mod(CodeContext context, string self, object other)
  {
    return new StringFormatter(context, self, other).Format();
  }

  [SpecialName]
  [return: MaybeNotImplemented]
  public static object Mod(CodeContext context, object other, string self)
  {
    switch (other)
    {
      case string str:
        return (object) new StringFormatter(context, str, (object) self).Format();
      case Extensible<string> extensible:
        return (object) new StringFormatter(context, extensible.Value, (object) self).Format();
      default:
        return (object) NotImplementedType.Value;
    }
  }

  [SpecialName]
  public static string Multiply(string s, int count)
  {
    if (count <= 0)
      return string.Empty;
    if (count == 1)
      return s;
    if ((long) s.Length * (long) count > (long) int.MaxValue)
      throw PythonOps.OverflowError("repeated string is too long");
    int length = s.Length;
    if (length == 1)
      return new string(s[0], count);
    StringBuilder stringBuilder = new StringBuilder(length * count);
    stringBuilder.Insert(0, s, count);
    return stringBuilder.ToString();
  }

  [SpecialName]
  public static string Multiply(int other, string self) => StringOps.Multiply(self, other);

  [SpecialName]
  public static object Multiply(string self, [NotNull] Index count)
  {
    return PythonOps.MultiplySequence<string>(new PythonOps.MultiplySequenceWorker<string>(StringOps.Multiply), self, count, true);
  }

  [SpecialName]
  public static object Multiply([NotNull] Index count, string self)
  {
    return PythonOps.MultiplySequence<string>(new PythonOps.MultiplySequenceWorker<string>(StringOps.Multiply), self, count, false);
  }

  [SpecialName]
  public static object Multiply(string self, object count)
  {
    int index;
    if (Converter.TryConvertToIndex(count, out index))
      return (object) StringOps.Multiply(self, index);
    throw PythonOps.TypeErrorForUnIndexableObject(count);
  }

  [SpecialName]
  public static object Multiply(object count, string self)
  {
    int index;
    if (Converter.TryConvertToIndex(count, out index))
      return (object) StringOps.Multiply(index, self);
    throw PythonOps.TypeErrorForUnIndexableObject(count);
  }

  [SpecialName]
  public static bool GreaterThan(string x, string y) => string.CompareOrdinal(x, y) > 0;

  [SpecialName]
  public static bool LessThan(string x, string y) => string.CompareOrdinal(x, y) < 0;

  [SpecialName]
  public static bool LessThanOrEqual(string x, string y) => string.CompareOrdinal(x, y) <= 0;

  [SpecialName]
  public static bool GreaterThanOrEqual(string x, string y) => string.CompareOrdinal(x, y) >= 0;

  [SpecialName]
  public static bool Equals(string x, string y) => string.Equals(x, y);

  [SpecialName]
  public static bool NotEquals(string x, string y) => !string.Equals(x, y);

  [ImplicitConversionMethod]
  [SpecialName]
  public static string ConvertFromChar(char c) => ScriptingRuntimeHelpers.CharToString(c);

  [ExplicitConversionMethod]
  [SpecialName]
  public static char ConvertToChar(string s)
  {
    return s.Length == 1 ? s[0] : throw PythonOps.TypeErrorForTypeMismatch("char", (object) s);
  }

  [ImplicitConversionMethod]
  [SpecialName]
  public static IEnumerable ConvertToIEnumerable(string s)
  {
    return (IEnumerable) new StringOps.PythonStringEnumerable(s);
  }

  internal static int Compare(string self, string obj)
  {
    int num = string.CompareOrdinal(self, obj);
    if (num == 0)
      return 0;
    return num >= 0 ? 1 : -1;
  }

  public static object __getnewargs__(CodeContext context, string self)
  {
    if (self == null)
      throw PythonOps.TypeErrorForBadInstance("__getnewargs__ requires a 'str' object but received a '{0}'", (object) self);
    return (object) PythonTuple.MakeTuple(StringOps.__new__(context, TypeCache.String, (object) self));
  }

  public static string __str__(string self) => self;

  public static Extensible<string> __str__(ExtensibleString self) => (Extensible<string>) self;

  internal static string Quote(string s)
  {
    bool isUnicode = false;
    StringBuilder stringBuilder = new StringBuilder(s.Length + 5);
    char quote = '\'';
    if (s.IndexOf('\'') != -1 && s.IndexOf('"') == -1)
      quote = '"';
    stringBuilder.Append(quote);
    stringBuilder.Append(StringOps.ReprEncode(s, quote, ref isUnicode));
    stringBuilder.Append(quote);
    return isUnicode ? "u" + stringBuilder.ToString() : stringBuilder.ToString();
  }

  internal static string ReprEncode(string s, ref bool isUnicode)
  {
    return StringOps.ReprEncode(s, char.MinValue, ref isUnicode);
  }

  internal static bool TryGetEncoding(string name, out Encoding encoding)
  {
    name = StringOps.NormalizeEncodingName(name);
    StringOps.EncodingInfoWrapper encodingInfoWrapper;
    if (StringOps.CodecsInfo.Codecs.TryGetValue(name, out encodingInfoWrapper))
    {
      encoding = (Encoding) encodingInfoWrapper.GetEncoding().Clone();
      return true;
    }
    encoding = (Encoding) null;
    return false;
  }

  internal static string RawUnicodeEscapeEncode(string s)
  {
    StringBuilder sb = (StringBuilder) null;
    for (int index = 0; index < s.Length; ++index)
    {
      char ch = s[index];
      if (ch > 'ÿ')
      {
        StringOps.ReprInit(ref sb, s, index);
        sb.AppendFormat("\\u{0:x4}", (object) (int) ch);
      }
      else
        sb?.Append(ch);
    }
    return sb == null ? s : sb.ToString();
  }

  private static int CheckIndex(object index, int defaultValue)
  {
    int index1;
    if (index == null)
      index1 = defaultValue;
    else if (!Converter.TryConvertToIndex(index, out index1))
      throw PythonOps.TypeError("slice indices must be integers or None or have an __index__ method");
    return index1;
  }

  private static void AppendJoin(object value, int index, StringBuilder sb)
  {
    if (value is string result)
      sb.Append(result);
    else if (Converter.TryConvertToString(value, out result) && result != null)
      sb.Append(result);
    else
      throw PythonOps.TypeError("sequence item {0}: expected string, {1} found", (object) index.ToString(), (object) PythonOps.GetPythonTypeName(value));
  }

  private static string ReplaceEmpty(string self, string @new, int count)
  {
    string str = self;
    if (count == 0)
      return str;
    if (count < 0)
      count = str.Length + 1;
    else if (count > str.Length + 1)
      count = checked (str.Length + 1);
    int capacity = checked (str.Length + @new.Length * count);
    int startIndex = Math.Min(str.Length, count);
    StringBuilder stringBuilder = new StringBuilder(capacity);
    for (int index = 0; index < startIndex; ++index)
    {
      stringBuilder.Append(@new);
      stringBuilder.Append(str[index]);
    }
    if (count > startIndex)
      stringBuilder.Append(@new);
    else
      stringBuilder.Append(str, startIndex, str.Length - startIndex);
    return stringBuilder.ToString();
  }

  private static string Reverse(string s)
  {
    if (s.Length == 0 || s.Length == 1)
      return s;
    char[] chArray = new char[s.Length];
    int index1 = s.Length - 1;
    int index2 = 0;
    while (index1 >= 0)
    {
      chArray[index2] = s[index1];
      --index1;
      ++index2;
    }
    return new string(chArray);
  }

  internal static string ReprEncode(string s, char quote, ref bool isUnicode)
  {
    StringBuilder sb = (StringBuilder) null;
    for (int index = 0; index < s.Length; ++index)
    {
      char ch = s[index];
      if (ch >= '\u007F')
        isUnicode = true;
      switch ((int) ch - 9)
      {
        case 0:
          StringOps.ReprInit(ref sb, s, index);
          sb.Append("\\t");
          break;
        case 1:
          StringOps.ReprInit(ref sb, s, index);
          sb.Append("\\n");
          break;
        case 2:
        case 3:
          if (quote != char.MinValue && (int) ch == (int) quote)
          {
            StringOps.ReprInit(ref sb, s, index);
            sb.Append('\\');
            sb.Append(ch);
            break;
          }
          if (ch < ' ' || ch >= '\u007F' && ch <= 'ÿ')
          {
            StringOps.ReprInit(ref sb, s, index);
            sb.AppendFormat("\\x{0:x2}", (object) (int) ch);
            break;
          }
          if (ch > 'ÿ')
          {
            StringOps.ReprInit(ref sb, s, index);
            sb.AppendFormat("\\u{0:x4}", (object) (int) ch);
            break;
          }
          if (sb != null)
          {
            sb.Append(ch);
            break;
          }
          break;
        case 4:
          StringOps.ReprInit(ref sb, s, index);
          sb.Append("\\r");
          break;
        default:
          if (ch == '\\')
          {
            StringOps.ReprInit(ref sb, s, index);
            sb.Append("\\\\");
            break;
          }
          goto case 2;
      }
    }
    return sb == null ? s : sb.ToString();
  }

  private static void ReprInit(ref StringBuilder sb, string s, int c)
  {
    if (sb != null)
      return;
    sb = new StringBuilder(s, 0, c, s.Length);
  }

  private static bool IsSign(char ch) => ch == '+' || ch == '-';

  internal static string GetEncodingName(Encoding encoding)
  {
    string name = (string) null;
    if (encoding.CodePage != 0)
    {
      if (encoding.IsBrowserDisplay)
        name = encoding.WebName;
      if (name == null && encoding.IsMailNewsDisplay)
        name = encoding.HeaderName;
      if (name == null)
        name = "cp" + (object) encoding.CodePage;
    }
    if (name == null)
      name = encoding.EncodingName;
    return StringOps.NormalizeEncodingName(name);
  }

  internal static string NormalizeEncodingName(string name)
  {
    return name?.ToLowerInvariant().Replace('-', '_').Replace(' ', '_');
  }

  private static string RawDecode(
    CodeContext context,
    string s,
    object encodingType,
    string errors)
  {
    PythonContext languageContext = context.LanguageContext;
    encoding = (Encoding) null;
    if (!(encodingType is string str1) && !(encodingType is Encoding encoding))
    {
      if (encodingType == Missing.Value)
        str1 = languageContext.GetDefaultEncodingName();
      else
        throw PythonOps.TypeError("decode() expected string, got '{0}'", (object) DynamicHelpers.GetPythonType(encodingType).Name);
    }
    if (encoding == null)
    {
      string str2 = StringOps.NormalizeEncodingName(str1);
      if ("raw_unicode_escape" == str2)
        return LiteralParser.ParseString(s, true, true);
      if ("unicode_escape" == str2)
        return LiteralParser.ParseString(s, false, true);
      if ("string_escape" == str2)
        return LiteralParser.ParseString(s, false, false);
    }
    if (encoding != null || StringOps.TryGetEncoding(str1, out encoding))
      return StringOps.DoDecode(context, s, errors, str1, encoding);
    return StringOps.UserDecodeOrEncode((PythonOps.LookupEncoding(context, str1) ?? throw PythonOps.LookupError("unknown encoding: {0}", (object) str1))[1], s, errors);
  }

  internal static string DoDecode(
    CodeContext context,
    string s,
    string errors,
    string encoding,
    Encoding e)
  {
    return StringOps.DoDecode(context, s, errors, encoding, e, true, out int _);
  }

  internal static string DoDecode(
    CodeContext context,
    string s,
    string errors,
    string encoding,
    Encoding e,
    bool final,
    out int numBytes)
  {
    byte[] bytes = s.MakeByteArray();
    int startingOffset = StringOps.GetStartingOffset(e, bytes);
    numBytes = bytes.Length - startingOffset;
    e = (Encoding) e.Clone();
    switch (errors)
    {
      case "backslashreplace":
      case "xmlcharrefreplace":
      case "strict":
        e.DecoderFallback = final ? DecoderFallback.ExceptionFallback : (DecoderFallback) new StringOps.ExceptionFallBack(numBytes, e is UTF8Encoding);
        break;
      case "replace":
        e.DecoderFallback = StringOps.ReplacementFallback;
        break;
      case "ignore":
        e.DecoderFallback = (DecoderFallback) new StringOps.PythonDecoderFallback(encoding, s, (object) null);
        break;
      default:
        e.DecoderFallback = (DecoderFallback) new StringOps.PythonDecoderFallback(encoding, s, LightExceptions.CheckAndThrow(PythonOps.LookupEncodingError(context, errors)));
        break;
    }
    string str = e.GetString(bytes, startingOffset, numBytes);
    if (!(e.DecoderFallback is StringOps.ExceptionFallBack decoderFallback))
      return str;
    byte[] badBytes = decoderFallback.buffer.badBytes;
    if (badBytes == null)
      return str;
    numBytes -= badBytes.Length;
    return str;
  }

  private static int GetStartingOffset(Encoding e, byte[] bytes)
  {
    byte[] preamble = e.GetPreamble();
    if (preamble.Length == 0 || bytes.Length < preamble.Length)
      return 0;
    for (int index = 0; index < preamble.Length; ++index)
    {
      if ((int) bytes[index] != (int) preamble[index])
        return 0;
    }
    return preamble.Length;
  }

  private static string RawEncode(
    CodeContext context,
    string s,
    object encodingType,
    string errors)
  {
    string str1 = encodingType as string;
    encoding = (Encoding) null;
    if (str1 == null && !(encodingType is Encoding encoding))
    {
      if (encodingType == Missing.Value)
        str1 = context.LanguageContext.GetDefaultEncodingName();
      else
        throw PythonOps.TypeError("encode() expected string, got '{0}'", (object) DynamicHelpers.GetPythonType(encodingType).Name);
    }
    if (encoding == null)
    {
      string str2 = StringOps.NormalizeEncodingName(str1);
      if ("raw_unicode_escape" == str2)
        return StringOps.RawUnicodeEscapeEncode(s);
      if ("unicode_escape" == str2 || "string_escape" == str2)
      {
        bool isUnicode = false;
        return StringOps.ReprEncode(s, '\'', ref isUnicode);
      }
    }
    if (encoding != null || StringOps.TryGetEncoding(str1, out encoding))
      return StringOps.DoEncode(context, s, errors, str1, encoding);
    return StringOps.UserDecodeOrEncode((PythonOps.LookupEncoding(context, str1) ?? throw PythonOps.LookupError("unknown encoding: {0}", (object) str1))[0], s, errors);
  }

  internal static string DoEncode(
    CodeContext context,
    string s,
    string errors,
    string encoding,
    Encoding e)
  {
    e = (Encoding) e.Clone();
    switch (errors)
    {
      case "strict":
        e.EncoderFallback = EncoderFallback.ExceptionFallback;
        break;
      case "replace":
        e.EncoderFallback = EncoderFallback.ReplacementFallback;
        break;
      case "backslashreplace":
        e.EncoderFallback = (EncoderFallback) new StringOps.BackslashEncoderReplaceFallback();
        break;
      case "xmlcharrefreplace":
        e.EncoderFallback = (EncoderFallback) new StringOps.XmlCharRefEncoderReplaceFallback();
        break;
      case "ignore":
        e.EncoderFallback = (EncoderFallback) new StringOps.PythonEncoderFallback(encoding, s, (object) null);
        break;
      default:
        e.EncoderFallback = (EncoderFallback) new StringOps.PythonEncoderFallback(encoding, s, LightExceptions.CheckAndThrow(PythonOps.LookupEncodingError(context, errors)));
        break;
    }
    return e.GetPreamble().MakeString((IList<byte>) e.GetBytes(s));
  }

  private static string UserDecodeOrEncode(object function, string data, string errors)
  {
    object obj;
    if (errors != null)
      obj = PythonCalls.Call(function, (object) data, (object) errors);
    else
      obj = PythonCalls.Call(function, (object) data);
    string str = StringOps.AsString(obj);
    if (str != null)
      return str;
    return obj is PythonTuple pythonTuple ? Converter.ConvertToString(pythonTuple[0]) : throw PythonOps.TypeErrorForBadInstance("expected tuple, but found {0}", obj);
  }

  private static List SplitEmptyString(bool separators)
  {
    List list = PythonOps.MakeEmptyList(1);
    if (separators)
      list.AddNoLock((object) string.Empty);
    return list;
  }

  private static List SplitInternal(string self, char[] seps, int maxsplit)
  {
    if (string.IsNullOrEmpty(self))
      return StringOps.SplitEmptyString(seps != null);
    string[] strArray = StringUtils.Split(self, seps, maxsplit < 0 ? int.MaxValue : maxsplit + 1, seps == null ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
    List list = PythonOps.MakeEmptyList(strArray.Length);
    foreach (string str in strArray)
      list.AddNoLock((object) str);
    return list;
  }

  private static List SplitInternal(string self, string separator, int maxsplit)
  {
    if (string.IsNullOrEmpty(self))
      return StringOps.SplitEmptyString(separator != null);
    string[] strArray = StringUtils.Split(self, separator, maxsplit < 0 ? int.MaxValue : maxsplit + 1, StringSplitOptions.None);
    List list = PythonOps.MakeEmptyList(strArray.Length);
    foreach (string str in strArray)
      list.AddNoLock((object) str);
    return list;
  }

  private static void TryStringOrTuple(object prefix)
  {
    switch (prefix)
    {
      case null:
        throw PythonOps.TypeError("expected string or Tuple, got NoneType");
      case string _:
        break;
      case PythonTuple _:
        break;
      case Extensible<string> _:
        break;
      default:
        throw PythonOps.TypeError("expected string or Tuple, got {0} Type", (object) prefix.GetType());
    }
  }

  private static string GetString(object obj)
  {
    return StringOps.AsString(obj) ?? throw PythonOps.TypeError("expected string, got {0}", (object) DynamicHelpers.GetPythonType(obj).Name);
  }

  public static bool endswith(string self, [BytesConversion] string suffix)
  {
    return self.EndsWith(suffix, StringComparison.Ordinal);
  }

  public static bool endswith(string self, [BytesConversion] string suffix, int start)
  {
    int length = self.Length;
    if (start > length)
      return false;
    if (start < 0)
    {
      start += length;
      if (start < 0)
        start = 0;
    }
    return self.Substring(start).EndsWith(suffix, StringComparison.Ordinal);
  }

  public static bool endswith(string self, [BytesConversion] string suffix, int start, int end)
  {
    int length = self.Length;
    if (start > length)
      return false;
    if (start < 0)
    {
      start += length;
      if (start < 0)
        start = 0;
    }
    if (end >= length)
      return self.Substring(start).EndsWith(suffix, StringComparison.Ordinal);
    if (end < 0)
    {
      end += length;
      if (end < 0)
        return false;
    }
    return end >= start && self.Substring(start, end - start).EndsWith(suffix, StringComparison.Ordinal);
  }

  private static bool endswith(string self, PythonTuple suffix)
  {
    foreach (object obj in suffix)
    {
      if (self.EndsWith(StringOps.GetString(obj), StringComparison.Ordinal))
        return true;
    }
    return false;
  }

  private static bool endswith(string self, PythonTuple suffix, int start)
  {
    foreach (object obj in suffix)
    {
      if (StringOps.endswith(self, StringOps.GetString(obj), start))
        return true;
    }
    return false;
  }

  private static bool endswith(string self, PythonTuple suffix, int start, int end)
  {
    foreach (object obj in suffix)
    {
      if (StringOps.endswith(self, StringOps.GetString(obj), start, end))
        return true;
    }
    return false;
  }

  public static bool startswith(string self, [BytesConversion] string prefix)
  {
    return self.StartsWith(prefix, StringComparison.Ordinal);
  }

  public static bool startswith(string self, [BytesConversion] string prefix, int start)
  {
    int length = self.Length;
    if (start > length)
      return false;
    if (start < 0)
    {
      start += length;
      if (start < 0)
        start = 0;
    }
    return self.Substring(start).StartsWith(prefix, StringComparison.Ordinal);
  }

  public static bool startswith(string self, [BytesConversion] string prefix, int start, int end)
  {
    int length = self.Length;
    if (start > length)
      return false;
    if (start < 0)
    {
      start += length;
      if (start < 0)
        start = 0;
    }
    if (end >= length)
      return self.Substring(start).StartsWith(prefix, StringComparison.Ordinal);
    if (end < 0)
    {
      end += length;
      if (end < 0)
        return false;
    }
    return end >= start && self.Substring(start, end - start).StartsWith(prefix, StringComparison.Ordinal);
  }

  private static bool startswith(string self, PythonTuple prefix)
  {
    foreach (object obj in prefix)
    {
      if (self.StartsWith(StringOps.GetString(obj), StringComparison.Ordinal))
        return true;
    }
    return false;
  }

  private static bool startswith(string self, PythonTuple prefix, int start)
  {
    foreach (object obj in prefix)
    {
      if (StringOps.startswith(self, StringOps.GetString(obj), start))
        return true;
    }
    return false;
  }

  private static bool startswith(string self, PythonTuple prefix, int start, int end)
  {
    foreach (object obj in prefix)
    {
      if (StringOps.startswith(self, StringOps.GetString(obj), start, end))
        return true;
    }
    return false;
  }

  internal static IEnumerable StringEnumerable(string str)
  {
    return (IEnumerable) new StringOps.PythonStringEnumerable(str);
  }

  internal static IEnumerator<string> StringEnumerator(string str)
  {
    return (IEnumerator<string>) new StringOps.PythonStringEnumerable(str);
  }

  public static string __repr__(string self) => StringOps.Quote(self);

  private static class CodecsInfo
  {
    public static readonly Dictionary<string, StringOps.EncodingInfoWrapper> Codecs = StringOps.CodecsInfo.MakeCodecsDict();

    private static Dictionary<string, StringOps.EncodingInfoWrapper> MakeCodecsDict()
    {
      Dictionary<string, StringOps.EncodingInfoWrapper> dictionary1 = new Dictionary<string, StringOps.EncodingInfoWrapper>();
      foreach (EncodingInfo encoding in Encoding.GetEncodings())
      {
        string key1 = StringOps.NormalizeEncodingName(encoding.Name);
        int num;
        switch (key1)
        {
          case "iso_8859_1":
            dictionary1["8859"] = dictionary1["latin_1"] = dictionary1["latin1"] = dictionary1["iso 8859_1"] = dictionary1["iso8859_1"] = dictionary1["cp819"] = dictionary1["819"] = dictionary1["latin"] = dictionary1["l1"] = (StringOps.EncodingInfoWrapper) encoding;
            goto default;
          case "us_ascii":
            Dictionary<string, StringOps.EncodingInfoWrapper> dictionary2 = dictionary1;
            num = encoding.CodePage;
            string key2 = "cp" + num.ToString();
            StringOps.EncodingInfoWrapper encodingInfoWrapper1 = dictionary1["us_ascii"] = dictionary1["us"] = dictionary1["ascii"] = dictionary1["646"] = (StringOps.EncodingInfoWrapper) new StringOps.AsciiEncodingInfoWrapper();
            dictionary2[key2] = encodingInfoWrapper1;
            break;
          case "utf_16":
            dictionary1["utf_16le"] = dictionary1["utf_16_le"] = new StringOps.EncodingInfoWrapper(encoding, new byte[0]);
            Dictionary<string, StringOps.EncodingInfoWrapper> dictionary3 = dictionary1;
            num = encoding.CodePage;
            string key3 = "cp" + num.ToString();
            StringOps.EncodingInfoWrapper encodingInfoWrapper2 = dictionary1["utf_16"] = dictionary1["utf16"] = dictionary1["u16"] = (StringOps.EncodingInfoWrapper) encoding;
            dictionary3[key3] = encodingInfoWrapper2;
            break;
          case "utf_16be":
            Dictionary<string, StringOps.EncodingInfoWrapper> dictionary4 = dictionary1;
            num = encoding.CodePage;
            string key4 = "cp" + num.ToString();
            StringOps.EncodingInfoWrapper encodingInfoWrapper3 = dictionary1["utf_16be"] = dictionary1["utf_16_be"] = new StringOps.EncodingInfoWrapper(encoding, new byte[0]);
            dictionary4[key4] = encodingInfoWrapper3;
            break;
          case "utf_32":
            dictionary1["utf_32le"] = dictionary1["utf_32_le"] = new StringOps.EncodingInfoWrapper(encoding, new byte[0]);
            Dictionary<string, StringOps.EncodingInfoWrapper> dictionary5 = dictionary1;
            num = encoding.CodePage;
            string key5 = "cp" + num.ToString();
            StringOps.EncodingInfoWrapper encodingInfoWrapper4 = dictionary1["utf_32"] = dictionary1["utf32"] = dictionary1["u32"] = (StringOps.EncodingInfoWrapper) encoding;
            dictionary5[key5] = encodingInfoWrapper4;
            break;
          case "utf_32be":
            Dictionary<string, StringOps.EncodingInfoWrapper> dictionary6 = dictionary1;
            num = encoding.CodePage;
            string key6 = "cp" + num.ToString();
            StringOps.EncodingInfoWrapper encodingInfoWrapper5 = dictionary1["utf_32be"] = dictionary1["utf_32_be"] = new StringOps.EncodingInfoWrapper(encoding, new byte[0]);
            dictionary6[key6] = encodingInfoWrapper5;
            break;
          case "utf_7":
            Dictionary<string, StringOps.EncodingInfoWrapper> dictionary7 = dictionary1;
            num = encoding.CodePage;
            string key7 = "cp" + num.ToString();
            StringOps.EncodingInfoWrapper encodingInfoWrapper6 = dictionary1["utf_7"] = dictionary1["u7"] = dictionary1["unicode-1-1-utf-7"] = (StringOps.EncodingInfoWrapper) encoding;
            dictionary7[key7] = encodingInfoWrapper6;
            goto default;
          case "utf_8":
            Dictionary<string, StringOps.EncodingInfoWrapper> dictionary8 = dictionary1;
            num = encoding.CodePage;
            string key8 = "cp" + num.ToString();
            StringOps.EncodingInfoWrapper encodingInfoWrapper7 = dictionary1["utf_8"] = dictionary1["utf8"] = dictionary1["u8"] = new StringOps.EncodingInfoWrapper(encoding, new byte[0]);
            dictionary8[key8] = encodingInfoWrapper7;
            dictionary1["utf_8_sig"] = (StringOps.EncodingInfoWrapper) encoding;
            break;
          default:
            dictionary1[key1] = (StringOps.EncodingInfoWrapper) encoding;
            Dictionary<string, StringOps.EncodingInfoWrapper> dictionary9 = dictionary1;
            num = encoding.GetEncoding().WindowsCodePage;
            string key9 = "windows-" + num.ToString();
            StringOps.EncodingInfoWrapper encodingInfoWrapper8 = (StringOps.EncodingInfoWrapper) encoding;
            dictionary9[key9] = encodingInfoWrapper8;
            Dictionary<string, StringOps.EncodingInfoWrapper> dictionary10 = dictionary1;
            num = encoding.CodePage;
            string key10 = "cp" + num.ToString();
            Dictionary<string, StringOps.EncodingInfoWrapper> dictionary11 = dictionary1;
            num = encoding.CodePage;
            string key11 = num.ToString();
            StringOps.EncodingInfoWrapper encodingInfoWrapper9;
            StringOps.EncodingInfoWrapper encodingInfoWrapper10 = encodingInfoWrapper9 = (StringOps.EncodingInfoWrapper) encoding;
            dictionary11[key11] = encodingInfoWrapper9;
            StringOps.EncodingInfoWrapper encodingInfoWrapper11 = encodingInfoWrapper10;
            dictionary10[key10] = encodingInfoWrapper11;
            break;
        }
      }
      dictionary1["raw_unicode_escape"] = new StringOps.EncodingInfoWrapper((Encoding) new StringOps.UnicodeEscapeEncoding(true));
      dictionary1["unicode_escape"] = new StringOps.EncodingInfoWrapper((Encoding) new StringOps.UnicodeEscapeEncoding(false));
      return dictionary1;
    }
  }

  private class EncodingInfoWrapper
  {
    private EncodingInfo _info;
    private Encoding _encoding;
    private byte[] _preamble;

    public EncodingInfoWrapper(Encoding enc) => this._encoding = enc;

    public EncodingInfoWrapper(EncodingInfo info, byte[] preamble = null)
    {
      this._info = info;
      this._preamble = preamble;
    }

    public virtual Encoding GetEncoding()
    {
      if (this._encoding != null)
        return this._encoding;
      return this._preamble == null ? this._info.GetEncoding() : (Encoding) new StringOps.EncodingWrapper(this._info.GetEncoding(), this._preamble);
    }

    public static implicit operator StringOps.EncodingInfoWrapper(EncodingInfo info)
    {
      return new StringOps.EncodingInfoWrapper(info);
    }
  }

  private class AsciiEncodingInfoWrapper : StringOps.EncodingInfoWrapper
  {
    public AsciiEncodingInfoWrapper()
      : base((EncodingInfo) null)
    {
    }

    public override Encoding GetEncoding() => PythonAsciiEncoding.Instance;
  }

  private class EncodingWrapper : Encoding
  {
    private byte[] _preamble;
    private Encoding _encoding;

    public EncodingWrapper(Encoding encoding, byte[] preamable)
    {
      this._preamble = preamable;
      this._encoding = encoding;
    }

    private void SetEncoderFallback() => this._encoding.EncoderFallback = this.EncoderFallback;

    private void SetDecoderFallback() => this._encoding.DecoderFallback = this.DecoderFallback;

    public override int GetByteCount(char[] chars, int index, int count)
    {
      this.SetEncoderFallback();
      return this._encoding.GetByteCount(chars, index, count);
    }

    public override int GetBytes(
      char[] chars,
      int charIndex,
      int charCount,
      byte[] bytes,
      int byteIndex)
    {
      this.SetEncoderFallback();
      return this._encoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
    }

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
      this.SetDecoderFallback();
      return this._encoding.GetCharCount(bytes, index, count);
    }

    public override int GetChars(
      byte[] bytes,
      int byteIndex,
      int byteCount,
      char[] chars,
      int charIndex)
    {
      this.SetDecoderFallback();
      return this._encoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
    }

    public override int GetMaxByteCount(int charCount)
    {
      this.SetEncoderFallback();
      return this._encoding.GetMaxByteCount(charCount);
    }

    public override int GetMaxCharCount(int byteCount)
    {
      this.SetDecoderFallback();
      return this._encoding.GetMaxCharCount(byteCount);
    }

    public override byte[] GetPreamble() => this._preamble;

    public override Encoder GetEncoder()
    {
      this.SetEncoderFallback();
      return this._encoding.GetEncoder();
    }

    public override Decoder GetDecoder()
    {
      this.SetDecoderFallback();
      return this._encoding.GetDecoder();
    }

    public override object Clone()
    {
      StringOps.EncodingWrapper encodingWrapper = (StringOps.EncodingWrapper) base.Clone();
      encodingWrapper._encoding = (Encoding) this._encoding.Clone();
      return (object) encodingWrapper;
    }
  }

  [PythonType("str_iterator")]
  private class PythonStringEnumerable : IEnumerable, IEnumerator<string>, IDisposable, IEnumerator
  {
    private readonly string _s;
    private int _index;

    public PythonStringEnumerable(string s)
    {
      this._index = -1;
      this._s = s;
    }

    public IEnumerator GetEnumerator() => (IEnumerator) this;

    public string Current
    {
      get
      {
        if (this._index < 0)
          throw PythonOps.SystemError("Enumeration has not started. Call MoveNext.");
        if (this._index >= this._s.Length)
          throw PythonOps.SystemError("Enumeration already finished.");
        return ScriptingRuntimeHelpers.CharToString(this._s[this._index]);
      }
    }

    public void Dispose()
    {
    }

    object IEnumerator.Current => (object) this.Current;

    public bool MoveNext()
    {
      if (this._index >= this._s.Length)
        return false;
      ++this._index;
      return this._index != this._s.Length;
    }

    public void Reset() => this._index = -1;
  }

  private class PythonEncoderFallbackBuffer : EncoderFallbackBuffer
  {
    private object _function;
    private string _encoding;
    private string _strData;
    private string _buffer;
    private int _bufferIndex;

    public PythonEncoderFallbackBuffer(string encoding, string str, object callable)
    {
      this._function = callable;
      this._strData = str;
      this._encoding = encoding;
    }

    public override bool Fallback(char charUnknown, int index) => this.DoPythonFallback(index, 1);

    public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
    {
      return this.DoPythonFallback(index, 2);
    }

    public override char GetNextChar()
    {
      return this._buffer == null || this._bufferIndex >= this._buffer.Length ? char.MinValue : this._buffer[this._bufferIndex++];
    }

    public override bool MovePrevious()
    {
      if (this._bufferIndex <= 0)
        return false;
      --this._bufferIndex;
      return true;
    }

    public override int Remaining
    {
      get => this._buffer == null ? 0 : this._buffer.Length - this._bufferIndex;
    }

    public override void Reset()
    {
      this._buffer = (string) null;
      this._bufferIndex = 0;
      base.Reset();
    }

    private bool DoPythonFallback(int index, int length)
    {
      if (this._function == null)
        return false;
      PythonExceptions._UnicodeEncodeError unicodeEncodeError = new PythonExceptions._UnicodeEncodeError();
      unicodeEncodeError.__init__((object) this._encoding, (object) this._strData, (object) index, (object) (index + length), (object) "unexpected code byte");
      this._buffer = StringOps.PythonDecoderFallbackBuffer.CheckReplacementTuple(PythonCalls.Call(this._function, (object) unicodeEncodeError), "encoding");
      this._bufferIndex = 0;
      return true;
    }
  }

  private class PythonEncoderFallback : EncoderFallback
  {
    private object _function;
    private string _str;
    private string _enc;

    public PythonEncoderFallback(string encoding, string data, object callable)
    {
      this._function = callable;
      this._str = data;
      this._enc = encoding;
    }

    public override EncoderFallbackBuffer CreateFallbackBuffer()
    {
      return (EncoderFallbackBuffer) new StringOps.PythonEncoderFallbackBuffer(this._enc, this._str, this._function);
    }

    public override int MaxCharCount => int.MaxValue;
  }

  private class PythonDecoderFallbackBuffer : DecoderFallbackBuffer
  {
    private object _function;
    private string _encoding;
    private string _strData;
    private string _buffer;
    private int _bufferIndex;

    public PythonDecoderFallbackBuffer(string encoding, string str, object callable)
    {
      this._encoding = encoding;
      this._strData = str;
      this._function = callable;
    }

    public override int Remaining
    {
      get => this._buffer == null ? 0 : this._buffer.Length - this._bufferIndex;
    }

    public override char GetNextChar()
    {
      return this._buffer == null || this._bufferIndex >= this._buffer.Length ? char.MinValue : this._buffer[this._bufferIndex++];
    }

    public override bool MovePrevious()
    {
      if (this._bufferIndex <= 0)
        return false;
      --this._bufferIndex;
      return true;
    }

    public override void Reset()
    {
      this._buffer = (string) null;
      this._bufferIndex = 0;
      base.Reset();
    }

    public override bool Fallback(byte[] bytesUnknown, int index)
    {
      if (this._function == null)
        return false;
      PythonExceptions._UnicodeDecodeError unicodeDecodeError = new PythonExceptions._UnicodeDecodeError();
      unicodeDecodeError.__init__((object) this._encoding, (object) this._strData, (object) index, (object) (index + bytesUnknown.Length), (object) "unexpected code byte");
      this._buffer = StringOps.PythonDecoderFallbackBuffer.CheckReplacementTuple(PythonCalls.Call(this._function, (object) unicodeDecodeError), "decoding");
      this._bufferIndex = 0;
      return true;
    }

    internal static string CheckReplacementTuple(object res, string encodeOrDecode)
    {
      bool flag = true;
      string result = (string) null;
      if (res is PythonTuple pythonTuple && pythonTuple.__len__() == 2)
      {
        if (!Converter.TryConvertToString(pythonTuple[0], out result))
          flag = false;
        if (flag && !Converter.TryConvertToInt32(pythonTuple[1], out int _))
          flag = false;
      }
      else
        flag = false;
      if (!flag)
        throw PythonOps.TypeError("{1} error handler must return tuple containing (str, int), got {0}", (object) PythonOps.GetPythonTypeName(res), (object) encodeOrDecode);
      return result;
    }
  }

  private class PythonDecoderFallback : DecoderFallback
  {
    private object function;
    private string str;
    private string enc;

    public PythonDecoderFallback(string encoding, string data, object callable)
    {
      this.function = callable;
      this.str = data;
      this.enc = encoding;
    }

    public override DecoderFallbackBuffer CreateFallbackBuffer()
    {
      return (DecoderFallbackBuffer) new StringOps.PythonDecoderFallbackBuffer(this.enc, this.str, this.function);
    }

    public override int MaxCharCount => throw new NotImplementedException();
  }

  private class BackslashEncoderReplaceFallback : EncoderFallback
  {
    public override EncoderFallbackBuffer CreateFallbackBuffer()
    {
      return (EncoderFallbackBuffer) new StringOps.BackslashEncoderReplaceFallback.BackslashReplaceFallbackBuffer();
    }

    public override int MaxCharCount => throw new NotImplementedException();

    private class BackslashReplaceFallbackBuffer : EncoderFallbackBuffer
    {
      private List<char> _buffer = new List<char>();
      private int _index;

      public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index) => false;

      public override bool Fallback(char charUnknown, int index)
      {
        this._buffer.Add('\\');
        int num = (int) charUnknown;
        if (num > (int) byte.MaxValue)
        {
          this._buffer.Add('u');
          this.AddCharacter(num >> 8);
          this.AddCharacter(num & (int) byte.MaxValue);
        }
        else
        {
          this._buffer.Add('x');
          this.AddCharacter((int) charUnknown);
        }
        return true;
      }

      private void AddCharacter(int val)
      {
        this.AddOneDigit((val & 240 /*0xF0*/) >> 4);
        this.AddOneDigit(val & 15);
      }

      private void AddOneDigit(int val)
      {
        if (val > 9)
          this._buffer.Add((char) (97 + val - 10));
        else
          this._buffer.Add((char) (48 /*0x30*/ + val));
      }

      public override char GetNextChar()
      {
        return this._index == this._buffer.Count ? char.MinValue : this._buffer[this._index++];
      }

      public override bool MovePrevious()
      {
        if (this._index <= 0)
          return false;
        --this._index;
        return true;
      }

      public override int Remaining => this._buffer.Count - this._index;
    }
  }

  private class XmlCharRefEncoderReplaceFallback : EncoderFallback
  {
    public override EncoderFallbackBuffer CreateFallbackBuffer()
    {
      return (EncoderFallbackBuffer) new StringOps.XmlCharRefEncoderReplaceFallback.XmlCharRefEncoderReplaceFallbackBuffer();
    }

    public override int MaxCharCount => throw new NotImplementedException();

    private class XmlCharRefEncoderReplaceFallbackBuffer : EncoderFallbackBuffer
    {
      private List<char> _buffer = new List<char>();
      private int _index;

      public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index) => false;

      public override bool Fallback(char charUnknown, int index)
      {
        this._buffer.Add('&');
        this._buffer.Add('#');
        foreach (char ch in ((int) charUnknown).ToString())
          this._buffer.Add(ch);
        this._buffer.Add(';');
        return true;
      }

      public override char GetNextChar()
      {
        return this._index == this._buffer.Count ? char.MinValue : this._buffer[this._index++];
      }

      public override bool MovePrevious()
      {
        if (this._index <= 0)
          return false;
        --this._index;
        return true;
      }

      public override int Remaining => this._buffer.Count - this._index;
    }
  }

  private class UnicodeEscapeEncoding : Encoding
  {
    private bool _raw;

    public UnicodeEscapeEncoding(bool raw) => this._raw = raw;

    public override int GetByteCount(char[] chars, int index, int count)
    {
      return this.EscapeEncode(chars, index, count).Length;
    }

    private string EscapeEncode(char[] chars, int index, int count)
    {
      if (this._raw)
        return StringOps.RawUnicodeEscapeEncode(new string(chars, index, count));
      bool isUnicode = false;
      return StringOps.ReprEncode(new string(chars, index, count), ref isUnicode);
    }

    public override int GetBytes(
      char[] chars,
      int charIndex,
      int charCount,
      byte[] bytes,
      int byteIndex)
    {
      if (this._raw)
      {
        string str = StringOps.RawUnicodeEscapeEncode(new string(chars, charIndex, charCount));
        for (int index = 0; index < str.Length; ++index)
          bytes[index + byteIndex] = this._raw ? (byte) str[index] : (byte) chars[index];
        return str.Length;
      }
      for (int index = 0; index < charCount; ++index)
        bytes[index + byteIndex] = (byte) chars[index + charIndex];
      return charCount;
    }

    public override int GetCharCount(byte[] bytes, int index, int count)
    {
      char[] text = new char[count];
      for (int index1 = 0; index1 < count; ++index1)
        text[index1] = (char) bytes[index1 + index];
      return LiteralParser.ParseString(text, 0, text.Length, this._raw, true, false).Length;
    }

    public override int GetChars(
      byte[] bytes,
      int byteIndex,
      int byteCount,
      char[] chars,
      int charIndex)
    {
      char[] text = new char[byteCount];
      for (int index = 0; index < byteCount; ++index)
        chars[index] = (char) bytes[index + byteIndex];
      string str = LiteralParser.ParseString(text, 0, text.Length, this._raw, true, false);
      for (int index = 0; index < str.Length; ++index)
        chars[index + charIndex] = str[index];
      return str.Length;
    }

    public override int GetMaxByteCount(int charCount) => charCount * 5;

    public override int GetMaxCharCount(int byteCount) => byteCount;
  }

  private class ExceptionFallBack : DecoderFallback
  {
    internal StringOps.ExceptionFallbackBuffer buffer;
    private static bool isUtf8Bugged;

    static ExceptionFallBack()
    {
      Encoding encoding = (Encoding) Encoding.UTF8.Clone();
      encoding.DecoderFallback = (DecoderFallback) new StringOps.ExceptionFallBack.TestUtf8DecoderFallBack();
      try
      {
        encoding.GetString(new byte[1]{ byte.MaxValue });
      }
      catch
      {
        StringOps.ExceptionFallBack.isUtf8Bugged = true;
      }
    }

    public ExceptionFallBack(int length, bool isUtf8 = false)
    {
      this.buffer = !isUtf8 || !StringOps.ExceptionFallBack.isUtf8Bugged ? new StringOps.ExceptionFallbackBuffer(length) : (StringOps.ExceptionFallbackBuffer) new StringOps.ExceptionFallbackBufferUtf8DotNet(length);
    }

    public override DecoderFallbackBuffer CreateFallbackBuffer()
    {
      return (DecoderFallbackBuffer) this.buffer;
    }

    public override int MaxCharCount => 0;

    private class TestUtf8DecoderFallBack : DecoderFallback
    {
      public override int MaxCharCount => 0;

      public override DecoderFallbackBuffer CreateFallbackBuffer()
      {
        return (DecoderFallbackBuffer) new StringOps.ExceptionFallBack.TestUtf8DecoderFallbackBuffer();
      }
    }

    private class TestUtf8DecoderFallbackBuffer : DecoderFallbackBuffer
    {
      public override int Remaining => 0;

      public override bool Fallback(byte[] bytesUnknown, int index)
      {
        if (index < 0)
          throw new Exception();
        return false;
      }

      public override char GetNextChar() => char.MinValue;

      public override bool MovePrevious() => false;
    }
  }

  private class ExceptionFallbackBuffer : DecoderFallbackBuffer
  {
    private readonly int length;
    internal byte[] badBytes;

    public ExceptionFallbackBuffer(int length) => this.length = length;

    public override bool Fallback(byte[] bytesUnknown, int index)
    {
      if (index > 0 && index + bytesUnknown.Length != this.length)
        throw PythonOps.UnicodeDecodeError($"failed to decode bytes at index: {index}", bytesUnknown, index);
      this.badBytes = bytesUnknown;
      return false;
    }

    public override char GetNextChar() => char.MinValue;

    public override bool MovePrevious() => false;

    public override int Remaining => 0;
  }

  private class ExceptionFallbackBufferUtf8DotNet(int length) : StringOps.ExceptionFallbackBuffer(length)
  {
    private bool ignoreNext;

    public override bool Fallback(byte[] bytesUnknown, int index)
    {
      if (this.ignoreNext)
        return false;
      index += bytesUnknown.Length;
      this.ignoreNext = true;
      return base.Fallback(bytesUnknown, index);
    }
  }
}
