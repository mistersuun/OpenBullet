// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonRegex
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace IronPython.Modules;

public static class PythonRegex
{
  private static CacheDict<PythonRegex.PatternKey, PythonRegex.RE_Pattern> _cachedPatterns = new CacheDict<PythonRegex.PatternKey, PythonRegex.RE_Pattern>(100);
  private static readonly System.Random r = new System.Random(DateTime.Now.Millisecond);
  public const int I = 2;
  public const int L = 4;
  public const int M = 8;
  public const int S = 16 /*0x10*/;
  public const int U = 32 /*0x20*/;
  public const int X = 64 /*0x40*/;
  public const int IGNORECASE = 2;
  public const int LOCALE = 4;
  public const int MULTILINE = 8;
  public const int DOTALL = 16 /*0x10*/;
  public const int UNICODE = 32 /*0x20*/;
  public const int VERBOSE = 64 /*0x40*/;
  public const string engine = "cli reg ex";
  private static readonly char[] _preParsedChars = new char[4]
  {
    '(',
    '{',
    '[',
    ']'
  };
  private const string _mangledNamedGroup = "___PyRegexNameMangled";

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    context.EnsureModuleException((object) "reerror", dict, "error", "re");
    PythonCopyReg.GetDispatchTable(context.SharedContext)[(object) DynamicHelpers.GetPythonTypeFromType(typeof (PythonRegex.RE_Pattern))] = dict[(object) "_pickle"];
  }

  public static PythonRegex.RE_Pattern compile(CodeContext context, object pattern, int flags = 0)
  {
    try
    {
      return PythonRegex.GetPattern(context, pattern, flags, true);
    }
    catch (ArgumentException ex)
    {
      throw PythonExceptions.CreateThrowable(PythonRegex.error(context), (object) ex.Message);
    }
  }

  public static string escape(string text)
  {
    if (text == null)
      throw PythonOps.TypeError("text must not be None");
    for (int index = 0; index < text.Length; ++index)
    {
      if (!char.IsLetterOrDigit(text[index]))
      {
        StringBuilder stringBuilder = new StringBuilder(text, 0, index, text.Length);
        char c = text[index];
        do
        {
          stringBuilder.Append('\\');
          stringBuilder.Append(c);
          ++index;
          int startIndex = index;
          for (; index < text.Length; ++index)
          {
            c = text[index];
            if (!char.IsLetterOrDigit(c))
              break;
          }
          stringBuilder.Append(text, startIndex, index - startIndex);
        }
        while (index < text.Length);
        return stringBuilder.ToString();
      }
    }
    return text;
  }

  public static IronPython.Runtime.List findall(
    CodeContext context,
    object pattern,
    string @string,
    int flags = 0)
  {
    PythonRegex.RE_Pattern pattern1 = PythonRegex.GetPattern(context, PythonRegex.ValidatePattern(pattern), flags);
    PythonRegex.ValidateString((object) @string, nameof (@string));
    return PythonRegex.FixFindAllMatch(pattern1, pattern1.FindAllWorker(context, @string, 0, (object) @string.Length), (Func<string, object>) null);
  }

  public static IronPython.Runtime.List findall(
    CodeContext context,
    object pattern,
    IList<byte> @string,
    int flags = 0)
  {
    PythonRegex.RE_Pattern pattern1 = PythonRegex.GetPattern(context, PythonRegex.ValidatePattern(pattern), flags);
    PythonRegex.ValidateString((object) @string, nameof (@string));
    return PythonRegex.FixFindAllMatch(pattern1, pattern1.FindAllWorker(context, @string, 0, (object) @string.Count), PythonRegex.FindMaker((object) @string));
  }

  private static Func<string, object> FindMaker(object input)
  {
    Func<string, object> maker = (Func<string, object>) null;
    if (input is ByteArray)
      maker = (Func<string, object>) (x => (object) new ByteArray(x.MakeByteArray()));
    return maker;
  }

  private static IronPython.Runtime.List FixFindAllMatch(
    PythonRegex.RE_Pattern pat,
    MatchCollection mc,
    Func<string, object> maker)
  {
    object[] objArray = new object[mc.Count];
    int length = pat._re.GetGroupNumbers().Length;
    for (int i = 0; i < mc.Count; ++i)
    {
      if (length > 2)
      {
        int num = 0;
        List<object> o = new List<object>();
        foreach (Group group in mc[i].Groups)
        {
          if (num++ != 0)
            o.Add(maker != null ? maker(group.Value) : (object) group.Value);
        }
        objArray[i] = (object) PythonTuple.Make((object) o);
      }
      else
        objArray[i] = length != 2 ? (maker != null ? maker(mc[i].Value) : (object) mc[i].Value) : (maker != null ? maker(mc[i].Groups[1].Value) : (object) mc[i].Groups[1].Value);
    }
    return IronPython.Runtime.List.FromArrayNoCopy(objArray);
  }

  public static object finditer(CodeContext context, object pattern, object @string, int flags = 0)
  {
    PythonRegex.RE_Pattern pattern1 = PythonRegex.GetPattern(context, PythonRegex.ValidatePattern(pattern), flags);
    string str = PythonRegex.ValidateString(@string, nameof (@string));
    return (object) PythonRegex.MatchIterator(pattern1.FindAllWorker(context, str, 0, (object) str.Length), pattern1, str);
  }

  public static PythonRegex.RE_Match match(
    CodeContext context,
    object pattern,
    object @string,
    int flags = 0)
  {
    return PythonRegex.GetPattern(context, PythonRegex.ValidatePattern(pattern), flags).match((object) PythonRegex.ValidateString(@string, nameof (@string)));
  }

  public static PythonRegex.RE_Match search(
    CodeContext context,
    object pattern,
    object @string,
    int flags = 0)
  {
    return PythonRegex.GetPattern(context, PythonRegex.ValidatePattern(pattern), flags).search((object) PythonRegex.ValidateString(@string, nameof (@string)));
  }

  [return: SequenceTypeInfo(new Type[] {typeof (string)})]
  public static IronPython.Runtime.List split(
    CodeContext context,
    object pattern,
    object @string,
    int maxsplit = 0,
    int flags = 0)
  {
    return PythonRegex.GetPattern(context, PythonRegex.ValidatePattern(pattern), flags).split((object) PythonRegex.ValidateString(@string, nameof (@string)), maxsplit);
  }

  public static string sub(
    CodeContext context,
    object pattern,
    object repl,
    object @string,
    int count = 0,
    int flags = 0)
  {
    return PythonRegex.GetPattern(context, PythonRegex.ValidatePattern(pattern), flags).sub(context, repl, (object) PythonRegex.ValidateString(@string, nameof (@string)), count);
  }

  public static object subn(
    CodeContext context,
    object pattern,
    object repl,
    object @string,
    int count = 0,
    int flags = 0)
  {
    return PythonRegex.GetPattern(context, PythonRegex.ValidatePattern(pattern), flags).subn(context, repl, (object) PythonRegex.ValidateString(@string, nameof (@string)), count);
  }

  public static void purge()
  {
    PythonRegex._cachedPatterns = new CacheDict<PythonRegex.PatternKey, PythonRegex.RE_Pattern>(100);
  }

  public static PythonTuple _pickle(CodeContext context, PythonRegex.RE_Pattern pattern)
  {
    object obj1 = Importer.ImportModule(context, (object) new PythonDictionary(), "re", false, 0);
    object obj2;
    if (!(obj1 is PythonModule) || !((PythonModule) obj1).__dict__.TryGetValue((object) "compile", out obj2))
      throw new InvalidOperationException("couldn't find compile method");
    return PythonTuple.MakeTuple(obj2, (object) PythonTuple.MakeTuple((object) pattern.pattern, (object) pattern.flags));
  }

  private static PythonRegex.RE_Pattern GetPattern(CodeContext context, object pattern, int flags)
  {
    return PythonRegex.GetPattern(context, pattern, flags, false);
  }

  private static PythonRegex.RE_Pattern GetPattern(
    CodeContext context,
    object pattern,
    int flags,
    bool compiled)
  {
    if (pattern is PythonRegex.RE_Pattern pattern1)
      return pattern1;
    string pattern2 = PythonRegex.ValidatePatternAsString(pattern);
    PythonRegex.PatternKey key = new PythonRegex.PatternKey(pattern2, flags);
    lock (PythonRegex._cachedPatterns)
    {
      if (PythonRegex._cachedPatterns.TryGetValue(new PythonRegex.PatternKey(pattern2, flags), out pattern1) && (!compiled || (pattern1._re.Options & RegexOptions.Compiled) == RegexOptions.Compiled))
        return pattern1;
      PythonRegex.RE_Pattern pattern3 = new PythonRegex.RE_Pattern(context, (object) pattern2, flags, compiled);
      PythonRegex._cachedPatterns[key] = pattern3;
      return pattern3;
    }
  }

  private static IEnumerator MatchIterator(
    MatchCollection matches,
    PythonRegex.RE_Pattern pattern,
    string input)
  {
    for (int i = 0; i < matches.Count; ++i)
      yield return (object) PythonRegex.RE_Match.make(matches[i], pattern, input, 0, input.Length);
  }

  private static RegexOptions FlagsToOption(int flags)
  {
    RegexOptions option = RegexOptions.None;
    if ((flags & 2) != 0)
      option |= RegexOptions.IgnoreCase;
    if ((flags & 8) != 0)
      option |= RegexOptions.Multiline;
    if ((flags & 4) == 0)
      option &= ~RegexOptions.CultureInvariant;
    if ((flags & 16 /*0x10*/) != 0)
      option |= RegexOptions.Singleline;
    if ((flags & 64 /*0x40*/) != 0)
      option |= RegexOptions.IgnorePatternWhitespace;
    return option;
  }

  private static int OptionToFlags(RegexOptions options)
  {
    int flags = 0;
    if ((options & RegexOptions.IgnoreCase) != RegexOptions.None)
      flags |= 2;
    if ((options & RegexOptions.Multiline) != RegexOptions.None)
      flags |= 8;
    if ((options & RegexOptions.CultureInvariant) == RegexOptions.None)
      flags |= 4;
    if ((options & RegexOptions.Singleline) != RegexOptions.None)
      flags |= 16 /*0x10*/;
    if ((options & RegexOptions.IgnorePatternWhitespace) != RegexOptions.None)
      flags |= 64 /*0x40*/;
    return flags;
  }

  private static PythonRegex.ParsedRegex PreParseRegex(CodeContext context, string pattern)
  {
    PythonRegex.ParsedRegex regex = new PythonRegex.ParsedRegex(pattern);
    int startIndex1 = 0;
    int num1 = 0;
    bool flag1 = false;
    bool flag2 = false;
    int num2 = 0;
    Dictionary<string, int> dictionary = new Dictionary<string, int>();
    int nameIndex;
    string key;
    while (true)
    {
      nameIndex = pattern.IndexOfAny(PythonRegex._preParsedChars, startIndex1);
      if (nameIndex > 0 && pattern[nameIndex - 1] == '\\')
      {
        int index = nameIndex - 2;
        int num3 = 1;
        for (; index >= 0 && pattern[index] == '\\'; --index)
          ++num3;
        if ((num3 & 1) != 0)
        {
          ++startIndex1;
          continue;
        }
      }
      if (nameIndex != -1 && nameIndex != pattern.Length - 1)
      {
        switch (pattern[nameIndex])
        {
          case '(':
            if (!flag1)
            {
              ++num2;
              if (pattern[++nameIndex] == '?')
              {
                if (nameIndex != pattern.Length - 1)
                {
                  switch (pattern[++nameIndex])
                  {
                    case '!':
                    case '#':
                    case ':':
                    case '<':
                    case '=':
                      break;
                    case '(':
                      ++nameIndex;
                      break;
                    case 'L':
                      regex.Options &= ~RegexOptions.CultureInvariant;
                      PythonRegex.RemoveOption(ref pattern, ref nameIndex);
                      break;
                    case 'P':
                      if (nameIndex + 1 < pattern.Length && pattern[nameIndex + 1] == '=')
                      {
                        pattern = pattern.Remove(nameIndex - 2, 4);
                        pattern = pattern.Insert(nameIndex - 2, "\\k<");
                        int length = pattern.IndexOf(')', nameIndex);
                        if (length != -1)
                        {
                          pattern = $"{pattern.Substring(0, length)}>{pattern.Substring(length + 1)}";
                          break;
                        }
                        goto label_20;
                      }
                      flag2 = true;
                      if (nameIndex + 1 < pattern.Length && pattern[nameIndex + 1] == '<')
                      {
                        int num4 = pattern.IndexOf('>', nameIndex);
                        if (num4 != -1)
                        {
                          key = pattern.Substring(nameIndex + 2, num4 - (nameIndex + 2));
                          if (!dictionary.ContainsKey(key))
                            dictionary[key] = num2;
                          else
                            goto label_26;
                        }
                        else
                          goto label_24;
                      }
                      pattern = pattern.Remove(nameIndex, 1);
                      break;
                    case 'i':
                      regex.Options |= RegexOptions.IgnoreCase;
                      PythonRegex.RemoveOption(ref pattern, ref nameIndex);
                      break;
                    case 'm':
                      regex.Options |= RegexOptions.Multiline;
                      PythonRegex.RemoveOption(ref pattern, ref nameIndex);
                      break;
                    case 's':
                      regex.Options |= RegexOptions.Singleline;
                      PythonRegex.RemoveOption(ref pattern, ref nameIndex);
                      break;
                    case 'u':
                      PythonRegex.RemoveOption(ref pattern, ref nameIndex);
                      break;
                    case 'x':
                      regex.Options |= RegexOptions.IgnorePatternWhitespace;
                      PythonRegex.RemoveOption(ref pattern, ref nameIndex);
                      break;
                    default:
                      goto label_36;
                  }
                }
                else
                  goto label_16;
              }
              else
              {
                ++num1;
                if (flag2)
                {
                  pattern = pattern.Insert(nameIndex, $"?<___PyRegexNameMangled{PythonRegex.GetRandomString()}>");
                  break;
                }
                break;
              }
            }
            else
            {
              ++nameIndex;
              break;
            }
            break;
          case '[':
            ++nameIndex;
            flag1 = true;
            break;
          case ']':
            ++nameIndex;
            flag1 = false;
            break;
          case '{':
            if (pattern[++nameIndex] == ',')
            {
              pattern = pattern.Insert(nameIndex, "0");
              break;
            }
            break;
        }
        startIndex1 = nameIndex;
      }
      else
        goto label_41;
    }
label_16:
    throw PythonExceptions.CreateThrowable(PythonRegex.error(context), (object) "unexpected end of regex");
label_20:
    throw PythonExceptions.CreateThrowable(PythonRegex.error(context), (object) "unexpected end of regex");
label_24:
    throw PythonExceptions.CreateThrowable(PythonRegex.error(context), (object) "unexpected end of regex");
label_26:
    throw PythonExceptions.CreateThrowable(PythonRegex.error(context), (object) $"redefinition of group name '{key}' as group {num2}; was group {dictionary[key]}");
label_36:
    throw PythonExceptions.CreateThrowable(PythonRegex.error(context), (object) ("Unrecognized extension " + pattern[nameIndex].ToString()));
label_41:
    int startIndex2 = 0;
    int num5;
    do
    {
      int num6 = pattern.IndexOf('\\', startIndex2);
      if (num6 != -1 && num6 != pattern.Length - 1)
      {
        int num7;
        num5 = num7 = num6 + 1;
        char ch = pattern[num5];
        switch (ch)
        {
          case 'A':
          case 'B':
          case 'D':
          case 'P':
          case 'S':
          case 'W':
          case '\\':
          case 'a':
          case 'b':
          case 'c':
          case 'd':
          case 'e':
          case 'f':
          case 'k':
          case 'n':
          case 'p':
          case 'r':
          case 's':
          case 't':
          case 'u':
          case 'v':
          case 'w':
          case 'x':
label_47:
            continue;
          case 'Z':
            pattern = pattern.Remove(num5, 1).Insert(num5, "z");
            goto case 'A';
          default:
            switch (CharUnicodeInfo.GetUnicodeCategory(ch))
            {
              case UnicodeCategory.UppercaseLetter:
              case UnicodeCategory.LowercaseLetter:
              case UnicodeCategory.TitlecaseLetter:
              case UnicodeCategory.ModifierLetter:
              case UnicodeCategory.OtherLetter:
              case UnicodeCategory.LetterNumber:
              case UnicodeCategory.OtherNumber:
              case UnicodeCategory.ConnectorPunctuation:
                pattern = pattern.Remove(num7 - 1, 1);
                --num5;
                goto label_47;
              default:
                goto label_47;
            }
        }
      }
      else
        break;
    }
    while ((startIndex2 = num5 + 1) < pattern.Length);
    regex.Pattern = pattern;
    return regex;
  }

  private static void RemoveOption(ref string pattern, ref int nameIndex)
  {
    if (pattern[nameIndex - 1] == '?' && nameIndex < pattern.Length - 1 && pattern[nameIndex + 1] == ')')
    {
      pattern = pattern.Remove(nameIndex - 2, 4);
      nameIndex -= 2;
    }
    else
    {
      pattern = pattern.Remove(nameIndex, 1);
      nameIndex -= 2;
    }
  }

  private static string GetRandomString()
  {
    return PythonRegex.r.Next(1073741823 /*0x3FFFFFFF*/, int.MaxValue).ToString();
  }

  private static string UnescapeGroups(Match m, string text)
  {
    for (int index = 0; index < text.Length; ++index)
    {
      if (text[index] == '\\')
      {
        StringBuilder stringBuilder = new StringBuilder(text, 0, index, text.Length);
        do
        {
          if (text[index] == '\\')
          {
            ++index;
            if (index == text.Length)
            {
              stringBuilder.Append('\\');
              break;
            }
            switch (text[index])
            {
              case '\'':
                stringBuilder.Append('\'');
                break;
              case '\\':
                stringBuilder.Append('\\');
                break;
              case 'b':
                stringBuilder.Append('\b');
                break;
              case 'g':
                if (text[index + 1] == '<')
                {
                  int num1 = index + 1;
                  int num2 = text.IndexOf('>', index + 2);
                  if (num2 != -1)
                  {
                    int length = num2 - (num1 + 1);
                    string str = text.Substring(num1 + 1, length);
                    int result;
                    if (StringUtils.TryParseInt32(str, out result))
                    {
                      Group group = m.Groups[result];
                      if (string.IsNullOrEmpty(group.Value))
                        throw PythonOps.IndexError("unknown group reference");
                      stringBuilder.Append(group.Value);
                    }
                    else
                    {
                      Group group = m.Groups[str];
                      if (string.IsNullOrEmpty(group.Value))
                        throw PythonOps.IndexError("unknown group reference");
                      stringBuilder.Append(group.Value);
                    }
                    index = num2;
                    break;
                  }
                  break;
                }
                stringBuilder.Append('\\');
                stringBuilder.Append(text[index]);
                break;
              case 'n':
                stringBuilder.Append('\n');
                break;
              case 'r':
                stringBuilder.Append('\r');
                break;
              case 't':
                stringBuilder.Append('\t');
                break;
              default:
                if (char.IsDigit(text[index]) && text[index] <= '7')
                {
                  int groupnum = 0;
                  int num = 0;
                  for (; index < text.Length && char.IsDigit(text[index]) && text[index] <= '7'; ++index)
                  {
                    ++num;
                    groupnum += groupnum * 8 + ((int) text[index] - 48 /*0x30*/);
                  }
                  --index;
                  if (num == 1 && groupnum > 0 && groupnum < m.Groups.Count)
                  {
                    stringBuilder.Append(m.Groups[groupnum].Value);
                    break;
                  }
                  stringBuilder.Append((char) groupnum);
                  break;
                }
                stringBuilder.Append('\\');
                stringBuilder.Append(text[index]);
                break;
            }
          }
          else
            stringBuilder.Append(text[index]);
        }
        while (++index < text.Length);
        return stringBuilder.ToString();
      }
    }
    return text;
  }

  private static object ValidatePattern(object pattern)
  {
    switch (pattern)
    {
      case string _:
        return (object) (pattern as string);
      case ExtensibleString extensibleString:
        return (object) extensibleString.Value;
      case Bytes bytes:
        return (object) bytes.ToString();
      case PythonRegex.RE_Pattern rePattern:
        return (object) rePattern;
      default:
        throw PythonOps.TypeError("pattern must be a string or compiled pattern");
    }
  }

  private static string ValidatePatternAsString(object pattern)
  {
    switch (pattern)
    {
      case string _:
        return pattern as string;
      case ExtensibleString extensibleString:
        return extensibleString.Value;
      case Bytes bytes:
        return bytes.ToString();
      case PythonRegex.RE_Pattern rePattern:
        return rePattern._pre.UserPattern;
      default:
        throw PythonOps.TypeError("pattern must be a string or compiled pattern");
    }
  }

  private static string ValidateString(object str, string param)
  {
    switch (str)
    {
      case string _:
        return str as string;
      case ExtensibleString extensibleString:
        return extensibleString.Value;
      case PythonBuffer pythonBuffer:
        return pythonBuffer.ToString();
      case Bytes bytes1:
        return bytes1.ToString();
      case ByteArray bytes2:
        return bytes2.MakeString();
      case ArrayModule.array array:
        return Bytes.Make(array.ToByteArray()).ToString();
      case MmapModule.MmapDefault mmapDefault:
        return mmapDefault.GetSearchString();
      default:
        throw PythonOps.TypeError($"expected string for parameter '{param}' but got '{PythonOps.GetPythonTypeName(str)}'");
    }
  }

  private static PythonType error(CodeContext context)
  {
    return (PythonType) context.LanguageContext.GetModuleState((object) "reerror");
  }

  [PythonType]
  public class RE_Pattern : IWeakReferenceable
  {
    internal Regex _re;
    private PythonDictionary _groups;
    private readonly int _compileFlags;
    private WeakRefTracker _weakRefTracker;
    internal PythonRegex.ParsedRegex _pre;

    internal RE_Pattern(CodeContext context, object pattern, int flags = 0, bool compiled = false)
    {
      this._pre = PythonRegex.PreParseRegex(context, PythonRegex.ValidatePatternAsString(pattern));
      try
      {
        flags |= PythonRegex.OptionToFlags(this._pre.Options);
        this._re = new Regex(this._pre.Pattern, PythonRegex.FlagsToOption(flags) | (compiled ? RegexOptions.Compiled : RegexOptions.None));
      }
      catch (ArgumentException ex)
      {
        throw PythonExceptions.CreateThrowable(PythonRegex.error(context), (object) ex.Message);
      }
      this._compileFlags = flags;
    }

    public PythonRegex.RE_Match match(object text)
    {
      string input = PythonRegex.ValidateString(text, nameof (text));
      return PythonRegex.RE_Match.makeMatch(this._re.Match(input), this, input, 0, input.Length);
    }

    private static int FixPosition(string text, int position)
    {
      if (position < 0)
        return 0;
      return position > text.Length ? text.Length : position;
    }

    public PythonRegex.RE_Match match(object text, int pos)
    {
      string str = PythonRegex.ValidateString(text, nameof (text));
      pos = PythonRegex.RE_Pattern.FixPosition(str, pos);
      return PythonRegex.RE_Match.makeMatch(this._re.Match(str, pos), this, str, pos, str.Length);
    }

    public PythonRegex.RE_Match match(object text, int pos = 0, int endpos)
    {
      string str = PythonRegex.ValidateString(text, nameof (text));
      pos = PythonRegex.RE_Pattern.FixPosition(str, pos);
      endpos = PythonRegex.RE_Pattern.FixPosition(str, endpos);
      return PythonRegex.RE_Match.makeMatch(this._re.Match(str.Substring(0, endpos), pos), this, str, pos, endpos);
    }

    public PythonRegex.RE_Match search(object text)
    {
      string input = PythonRegex.ValidateString(text, nameof (text));
      return PythonRegex.RE_Match.make(this._re.Match(input), this, input);
    }

    public PythonRegex.RE_Match search(object text, int pos)
    {
      string input = PythonRegex.ValidateString(text, nameof (text));
      if (pos < 0)
        pos = 0;
      return PythonRegex.RE_Match.make(this._re.Match(input, pos), this, input);
    }

    public PythonRegex.RE_Match search(object text, int pos, int endpos)
    {
      string input = PythonRegex.ValidateString(text, nameof (text));
      if (pos < 0)
        pos = 0;
      if (endpos < pos)
        return (PythonRegex.RE_Match) null;
      if (endpos < input.Length)
        input = input.Substring(0, endpos);
      return PythonRegex.RE_Match.make(this._re.Match(input, pos), this, input);
    }

    public object findall(CodeContext context, string @string)
    {
      return this.findall(context, (object) @string, 0, (object) null);
    }

    public object findall(CodeContext context, string @string, int pos)
    {
      return this.findall(context, (object) @string, pos, (object) null);
    }

    public object findall(CodeContext context, object @string, int pos, object endpos)
    {
      return (object) PythonRegex.FixFindAllMatch(this, this.FindAllWorker(context, PythonRegex.ValidateString(@string, "text"), pos, endpos), PythonRegex.FindMaker(@string));
    }

    internal MatchCollection FindAllWorker(
      CodeContext context,
      string str,
      int pos,
      object endpos)
    {
      string input = str;
      if (endpos != null)
      {
        int int32 = context.LanguageContext.ConvertToInt32(endpos);
        input = input.Substring(0, Math.Max(int32, 0));
      }
      return this._re.Matches(input, pos);
    }

    internal MatchCollection FindAllWorker(
      CodeContext context,
      IList<byte> str,
      int pos,
      object endpos)
    {
      string input = str.MakeString();
      if (endpos != null)
      {
        int int32 = context.LanguageContext.ConvertToInt32(endpos);
        input = input.Substring(0, Math.Max(int32, 0));
      }
      return this._re.Matches(input, pos);
    }

    public object finditer(CodeContext context, object @string)
    {
      string str = PythonRegex.ValidateString(@string, nameof (@string));
      return (object) PythonRegex.MatchIterator(this.FindAllWorker(context, str, 0, (object) str.Length), this, str);
    }

    public object finditer(CodeContext context, object @string, int pos)
    {
      string str = PythonRegex.ValidateString(@string, nameof (@string));
      return (object) PythonRegex.MatchIterator(this.FindAllWorker(context, str, pos, (object) str.Length), this, str);
    }

    public object finditer(CodeContext context, object @string, int pos, int endpos)
    {
      string str = PythonRegex.ValidateString(@string, nameof (@string));
      return (object) PythonRegex.MatchIterator(this.FindAllWorker(context, str, pos, (object) endpos), this, str);
    }

    [return: SequenceTypeInfo(new Type[] {typeof (string)})]
    public IronPython.Runtime.List split(object @string, int maxsplit = 0)
    {
      IronPython.Runtime.List list = new IronPython.Runtime.List();
      if (maxsplit < 0)
      {
        list.AddNoLock((object) PythonRegex.ValidateString(@string, nameof (@string)));
      }
      else
      {
        string input = PythonRegex.ValidateString(@string, nameof (@string));
        MatchCollection matchCollection = this._re.Matches(input);
        int startIndex = 0;
        int num = 0;
        foreach (Match match in matchCollection)
        {
          if (match.Length > 0)
          {
            list.AddNoLock((object) input.Substring(startIndex, match.Index - startIndex));
            if (match.Groups.Count > 1)
            {
              for (int groupnum = 1; groupnum < match.Groups.Count; ++groupnum)
              {
                if (match.Groups[groupnum].Success)
                  list.AddNoLock((object) match.Groups[groupnum].Value);
                else
                  list.AddNoLock((object) null);
              }
            }
            startIndex = match.Index + match.Length;
            ++num;
            if (num == maxsplit)
              break;
          }
        }
        list.AddNoLock((object) input.Substring(startIndex));
      }
      return list;
    }

    public string sub(CodeContext context, object repl, object @string, int count = 0)
    {
      if (repl == null)
        throw PythonOps.TypeError("NoneType is not valid repl");
      if (count == 0)
        count = int.MaxValue;
      string replacement = repl as string;
      if (replacement == null)
      {
        if (repl is ExtensibleString)
          replacement = ((Extensible<string>) repl).Value;
        else if (repl is Bytes)
          replacement = ((Bytes) repl).ToString();
      }
      Match prev = (Match) null;
      string input = PythonRegex.ValidateString(@string, nameof (@string));
      return this._re.Replace(input, (MatchEvaluator) (match =>
      {
        if (string.IsNullOrEmpty(match.Value) && prev != null && prev.Index + prev.Length == match.Index)
          return "";
        prev = match;
        return replacement != null ? PythonRegex.UnescapeGroups(match, replacement) : PythonCalls.Call(context, repl, (object) PythonRegex.RE_Match.make(match, this, input)) as string;
      }), count);
    }

    public object subn(CodeContext context, object repl, object @string, int count = 0)
    {
      if (repl == null)
        throw PythonOps.TypeError("NoneType is not valid repl");
      if (count == 0)
        count = int.MaxValue;
      int totalCount = 0;
      string replacement = repl as string;
      if (replacement == null)
      {
        if (repl is ExtensibleString)
          replacement = ((Extensible<string>) repl).Value;
        else if (repl is Bytes)
          replacement = ((Bytes) repl).ToString();
      }
      Match prev = (Match) null;
      string input = PythonRegex.ValidateString(@string, nameof (@string));
      return (object) PythonTuple.MakeTuple((object) this._re.Replace(input, (MatchEvaluator) (match =>
      {
        if (string.IsNullOrEmpty(match.Value) && prev != null && prev.Index + prev.Length == match.Index)
          return "";
        prev = match;
        ++totalCount;
        return replacement != null ? PythonRegex.UnescapeGroups(match, replacement) : PythonCalls.Call(context, repl, (object) PythonRegex.RE_Match.make(match, this, input)) as string;
      }), count), (object) totalCount);
    }

    public int flags => this._compileFlags;

    public PythonDictionary groupindex
    {
      get
      {
        if (this._groups == null)
        {
          PythonDictionary pythonDictionary = new PythonDictionary();
          string[] groupNames = this._re.GetGroupNames();
          int[] groupNumbers = this._re.GetGroupNumbers();
          for (int index = 1; index < groupNames.Length; ++index)
          {
            if (!char.IsDigit(groupNames[index][0]) && !groupNames[index].StartsWith("___PyRegexNameMangled"))
              pythonDictionary[(object) groupNames[index]] = (object) groupNumbers[index];
          }
          this._groups = pythonDictionary;
        }
        return this._groups;
      }
    }

    public int groups => this._re.GetGroupNumbers().Length - 1;

    public string pattern => this._pre.UserPattern;

    public override bool Equals(object obj)
    {
      return obj is PythonRegex.RE_Pattern rePattern && rePattern.pattern == this.pattern && rePattern.flags == this.flags;
    }

    public override int GetHashCode() => this.pattern.GetHashCode() ^ this.flags;

    WeakRefTracker IWeakReferenceable.GetWeakRef() => this._weakRefTracker;

    bool IWeakReferenceable.SetWeakRef(WeakRefTracker value)
    {
      this._weakRefTracker = value;
      return true;
    }

    void IWeakReferenceable.SetFinalizer(WeakRefTracker value)
    {
      ((IWeakReferenceable) this).SetWeakRef(value);
    }
  }

  [PythonType]
  public class RE_Match
  {
    private PythonRegex.RE_Pattern _pattern;
    private Match _m;
    private readonly string _text;
    private int _lastindex = -1;
    private readonly int _pos;
    private readonly int _endPos;

    internal static PythonRegex.RE_Match make(
      Match m,
      PythonRegex.RE_Pattern pattern,
      string input)
    {
      return m.Success ? new PythonRegex.RE_Match(m, pattern, input, 0, input.Length) : (PythonRegex.RE_Match) null;
    }

    internal static PythonRegex.RE_Match make(
      Match m,
      PythonRegex.RE_Pattern pattern,
      string input,
      int offset,
      int endpos)
    {
      return m.Success ? new PythonRegex.RE_Match(m, pattern, input, offset, endpos) : (PythonRegex.RE_Match) null;
    }

    internal static PythonRegex.RE_Match makeMatch(
      Match m,
      PythonRegex.RE_Pattern pattern,
      string input,
      int offset,
      int endpos)
    {
      return m.Success && m.Index == offset ? new PythonRegex.RE_Match(m, pattern, input, offset, endpos) : (PythonRegex.RE_Match) null;
    }

    public RE_Match(Match m, PythonRegex.RE_Pattern pattern, string text)
    {
      this._m = m;
      this._pattern = pattern;
      this._text = text;
    }

    public RE_Match(Match m, PythonRegex.RE_Pattern pattern, string text, int pos, int endpos)
    {
      this._m = m;
      this._pattern = pattern;
      this._text = text;
      this._pos = pos;
      this._endPos = endpos;
    }

    public int end() => this._m.Index + this._m.Length;

    public int start() => this._m.Index;

    public int start(object group)
    {
      int groupIndex = this.GetGroupIndex(group);
      return !this._m.Groups[groupIndex].Success ? -1 : this._m.Groups[groupIndex].Index;
    }

    public int end(object group)
    {
      int groupIndex = this.GetGroupIndex(group);
      return !this._m.Groups[groupIndex].Success ? -1 : this._m.Groups[groupIndex].Index + this._m.Groups[groupIndex].Length;
    }

    public object group(object index, params object[] additional)
    {
      if (additional.Length == 0)
        return (object) this.group(index);
      object[] objArray = new object[additional.Length + 1];
      objArray[0] = this._m.Groups[this.GetGroupIndex(index)].Success ? (object) this._m.Groups[this.GetGroupIndex(index)].Value : (object) (string) null;
      for (int index1 = 1; index1 < objArray.Length; ++index1)
      {
        int groupIndex = this.GetGroupIndex(additional[index1 - 1]);
        objArray[index1] = this._m.Groups[groupIndex].Success ? (object) this._m.Groups[groupIndex].Value : (object) (string) null;
      }
      return (object) PythonTuple.MakeTuple(objArray);
    }

    public string group(object index)
    {
      Group group = this._m.Groups[this.GetGroupIndex(index)];
      return !group.Success ? (string) null : group.Value;
    }

    public string group() => this.group((object) 0);

    [return: SequenceTypeInfo(new Type[] {typeof (string)})]
    public PythonTuple groups() => this.groups((object) null);

    public PythonTuple groups(object @default)
    {
      object[] objArray = new object[this._m.Groups.Count - 1];
      for (int groupnum = 1; groupnum < this._m.Groups.Count; ++groupnum)
        objArray[groupnum - 1] = this._m.Groups[groupnum].Success ? (object) this._m.Groups[groupnum].Value : @default;
      return PythonTuple.MakeTuple(objArray);
    }

    public string expand(object template)
    {
      string str = PythonRegex.ValidateString(template, nameof (template));
      StringBuilder sb = new StringBuilder();
      for (int index = 0; index < str.Length; ++index)
      {
        if (str[index] != '\\')
          sb.Append(str[index]);
        else if (++index == str.Length)
          sb.Append(str[index - 1]);
        else if (char.IsDigit(str[index]))
          this.AppendGroup(sb, (int) str[index] - 48 /*0x30*/);
        else if (str[index] == 'g')
        {
          if (++index == str.Length)
          {
            sb.Append("\\g");
            return sb.ToString();
          }
          if (str[index] != '<')
          {
            sb.Append("\\g<");
          }
          else
          {
            StringBuilder stringBuilder = new StringBuilder();
            ++index;
            while (str[index] != '>' && index < str.Length)
              stringBuilder.Append(str[index++]);
            this.AppendGroup(sb, this._pattern._re.GroupNumberFromName(stringBuilder.ToString()));
          }
        }
        else
        {
          switch (str[index])
          {
            case '\\':
              sb.Append('\\');
              continue;
            case 'n':
              sb.Append('\n');
              continue;
            case 'r':
              sb.Append('\r');
              continue;
            case 't':
              sb.Append('\t');
              continue;
            default:
              continue;
          }
        }
      }
      return sb.ToString();
    }

    [return: DictionaryTypeInfo(typeof (string), typeof (string))]
    public PythonDictionary groupdict() => this.groupdict((string) null);

    private static bool IsGroupNumber(string name)
    {
      foreach (char c in name)
      {
        if (!char.IsNumber(c))
          return false;
      }
      return true;
    }

    [return: DictionaryTypeInfo(typeof (string), typeof (string))]
    public PythonDictionary groupdict([NotNull] string value) => this.groupdict((object) value);

    [return: DictionaryTypeInfo(typeof (string), typeof (object))]
    public PythonDictionary groupdict(object value)
    {
      string[] groupNames = this._pattern._re.GetGroupNames();
      PythonDictionary pythonDictionary = new PythonDictionary();
      for (int groupnum = 0; groupnum < groupNames.Length; ++groupnum)
      {
        if (!PythonRegex.RE_Match.IsGroupNumber(groupNames[groupnum]))
        {
          if (this._m.Groups[groupnum].Captures.Count != 0)
            pythonDictionary[(object) groupNames[groupnum]] = (object) this._m.Groups[groupnum].Value;
          else
            pythonDictionary[(object) groupNames[groupnum]] = value;
        }
      }
      return pythonDictionary;
    }

    [return: SequenceTypeInfo(new Type[] {typeof (int)})]
    public PythonTuple span() => PythonTuple.MakeTuple((object) this.start(), (object) this.end());

    [return: SequenceTypeInfo(new Type[] {typeof (int)})]
    public PythonTuple span(object group)
    {
      return PythonTuple.MakeTuple((object) this.start(group), (object) this.end(group));
    }

    public int pos => this._pos;

    public int endpos => this._endPos;

    public string @string => this._text;

    public PythonTuple regs
    {
      get
      {
        object[] objArray = new object[this._m.Groups.Count];
        for (int group = 0; group < objArray.Length; ++group)
          objArray[group] = (object) PythonTuple.MakeTuple((object) this.start((object) group), (object) this.end((object) group));
        return PythonTuple.MakeTuple(objArray);
      }
    }

    public PythonRegex.RE_Pattern re => this._pattern;

    public object lastindex
    {
      get
      {
        if (this._lastindex == -1)
        {
          int groupnum = 1;
label_7:
          while (groupnum < this._m.Groups.Count)
          {
            if (this._m.Groups[groupnum].Success)
            {
              this._lastindex = groupnum;
              int num = this._m.Groups[groupnum].Index + this._m.Groups[groupnum].Length;
              ++groupnum;
              while (true)
              {
                if (groupnum < this._m.Groups.Count && this._m.Groups[groupnum].Index < num)
                  ++groupnum;
                else
                  goto label_7;
              }
            }
            else
              ++groupnum;
          }
          if (this._lastindex == -1)
            this._lastindex = 0;
        }
        return this._lastindex == 0 ? (object) null : (object) this._lastindex;
      }
    }

    public string lastgroup
    {
      get
      {
        return this.lastindex == null ? (string) null : this._pattern._re.GroupNameFromNumber((int) this.lastindex);
      }
    }

    private void AppendGroup(StringBuilder sb, int index) => sb.Append(this._m.Groups[index].Value);

    private int GetGroupIndex(object group)
    {
      int result;
      if (!Converter.TryConvertToInt32(group, out result))
        result = this._pattern._re.GroupNumberFromName(PythonRegex.ValidateString(group, nameof (group)));
      if (result < 0 || result >= this._m.Groups.Count)
        throw PythonOps.IndexError("no such group");
      return result;
    }
  }

  internal class ParsedRegex
  {
    public string UserPattern;
    public string Pattern;
    public RegexOptions Options = RegexOptions.CultureInvariant;

    public ParsedRegex(string pattern) => this.UserPattern = pattern;
  }

  private class PatternKey : IEquatable<PythonRegex.PatternKey>
  {
    public string Pattern;
    public int Flags;

    public PatternKey(string pattern, int flags)
    {
      this.Pattern = pattern;
      this.Flags = flags;
    }

    public override bool Equals(object obj)
    {
      return obj is PythonRegex.PatternKey other && this.Equals(other);
    }

    public override int GetHashCode() => this.Pattern.GetHashCode() ^ this.Flags;

    public bool Equals(PythonRegex.PatternKey other)
    {
      return other.Pattern == this.Pattern && other.Flags == this.Flags;
    }
  }
}
