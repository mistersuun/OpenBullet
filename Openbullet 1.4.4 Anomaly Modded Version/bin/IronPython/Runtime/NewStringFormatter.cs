// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.NewStringFormatter
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Modules;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal sealed class NewStringFormatter
{
  private static readonly char[] _brackets = new char[2]
  {
    '{',
    '}'
  };
  private static readonly char[] _fieldNameEnd = new char[4]
  {
    '{',
    '}',
    '!',
    ':'
  };

  public static string FormatString(
    PythonContext context,
    string format,
    PythonTuple args,
    IDictionary<object, object> kwArgs)
  {
    ContractUtils.RequiresNotNull((object) context, nameof (context));
    ContractUtils.RequiresNotNull((object) format, nameof (format));
    ContractUtils.RequiresNotNull((object) args, nameof (args));
    ContractUtils.RequiresNotNull((object) kwArgs, nameof (kwArgs));
    return NewStringFormatter.Formatter.FormatString(context, format, args, kwArgs);
  }

  public static IEnumerable<PythonTuple> GetFormatInfo(string format)
  {
    ContractUtils.RequiresNotNull((object) format, nameof (format));
    return NewStringFormatter.StringFormatParser.Parse(format);
  }

  public static PythonTuple GetFieldNameInfo(string name)
  {
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    NewStringFormatter.FieldName fieldName = NewStringFormatter.ParseFieldName(name, false);
    object obj = !string.IsNullOrEmpty(fieldName.ArgumentName) ? (object) fieldName.ArgumentName : throw PythonOps.ValueError("empty field name");
    int result;
    if (int.TryParse(fieldName.ArgumentName, out result))
      obj = ScriptingRuntimeHelpers.Int32ToObject(result);
    return PythonTuple.MakeTuple(obj, (object) NewStringFormatter.AccessorsToPython(fieldName.Accessors));
  }

  private static NewStringFormatter.FieldName ParseFieldName(string str, bool reportErrors)
  {
    int index = 0;
    return new NewStringFormatter.FieldName(NewStringFormatter.ParseIdentifier(str, false, ref index), NewStringFormatter.ParseFieldAccessors(str, index, reportErrors));
  }

  private static IEnumerable<NewStringFormatter.FieldAccessor> ParseFieldAccessors(
    string str,
    int index,
    bool reportErrors)
  {
    while (index != str.Length && str[index] != '}')
    {
      char ch = str[index];
      switch (ch)
      {
        case '.':
        case '[':
          ++index;
          bool isIndex = ch == '[';
          string identifier = NewStringFormatter.ParseIdentifier(str, isIndex, ref index);
          if (isIndex)
          {
            if (index == str.Length || str[index] != ']')
              throw PythonOps.ValueError("Missing ']' in format string");
            ++index;
          }
          if (identifier.Length == 0)
            throw PythonOps.ValueError("Empty attribute in format string");
          yield return new NewStringFormatter.FieldAccessor(identifier, !isIndex);
          continue;
        default:
          if (!reportErrors)
            yield break;
          throw PythonOps.ValueError("Only '.' and '[' are valid in format field specifier, got {0}", (object) ch);
      }
    }
  }

  private static IEnumerable<PythonTuple> AccessorsToPython(
    IEnumerable<NewStringFormatter.FieldAccessor> accessors)
  {
    foreach (NewStringFormatter.FieldAccessor accessor in accessors)
    {
      object attributeName = (object) accessor.AttributeName;
      int result;
      if (int.TryParse(accessor.AttributeName, out result))
        attributeName = ScriptingRuntimeHelpers.Int32ToObject(result);
      yield return PythonTuple.MakeTuple(ScriptingRuntimeHelpers.BooleanToObject(accessor.IsField), attributeName);
    }
  }

  private static string ParseIdentifier(string str, bool isIndex, ref int index)
  {
    int startIndex = index;
    while (index < str.Length && str[index] != '.' && (isIndex || str[index] != '[') && (!isIndex || str[index] != ']'))
      ++index;
    return str.Substring(startIndex, index - startIndex);
  }

  private struct StringFormatParser
  {
    private readonly string _str;
    private int _index;

    private StringFormatParser(string text)
    {
      this._str = text;
      this._index = 0;
    }

    public static IEnumerable<PythonTuple> Parse(string text)
    {
      return new NewStringFormatter.StringFormatParser(text).Parse();
    }

    private IEnumerable<PythonTuple> Parse()
    {
      while (this._index != this._str.Length)
      {
        int index = this._index;
        this._index = this._str.IndexOfAny(NewStringFormatter._brackets, this._index);
        if (this._index == -1)
        {
          yield return PythonTuple.MakeTuple((object) this._str.Substring(index, this._str.Length - index), null, null, null);
          break;
        }
        yield return this.ParseFormat(index);
      }
    }

    private PythonTuple ParseFormat(int lastTextStart)
    {
      string text;
      if (this.ParseDoubleBracket(lastTextStart, out text))
        return PythonTuple.MakeTuple((object) text, null, null, null);
      int depth = 1;
      char? nullable = new char?();
      string str = string.Empty;
      string fieldName = this.ParseFieldName(ref depth);
      int num1 = this.CheckEnd() ? 1 : 0;
      if (num1 == 0 && this._str[this._index] == '!')
        nullable = new char?(this.ParseConversion());
      int num2 = num1 != 0 ? 1 : (this.CheckEnd() ? 1 : 0);
      if (num2 == 0 && this._str[this._index] == ':')
        str = this.ParseFormatSpec(ref depth);
      if ((num2 != 0 ? 1 : (this.CheckEnd() ? 1 : 0)) == 0)
        throw PythonOps.ValueError("expected ':' after format specifier");
      return PythonTuple.MakeTuple((object) text, (object) fieldName, (object) str, nullable.HasValue ? (object) nullable.ToString() : (object) (string) null);
    }

    private bool ParseDoubleBracket(int lastTextStart, out string text)
    {
      if (this._str[this._index] == '}')
      {
        ++this._index;
        if (this._index == this._str.Length || this._str[this._index] != '}')
          throw PythonOps.ValueError("Single '}}' encountered in format string");
        text = this._str.Substring(lastTextStart, this._index - lastTextStart);
        ++this._index;
        return true;
      }
      if (this._index == this._str.Length - 1)
        throw PythonOps.ValueError("Single '{{' encountered in format string");
      if (this._str[this._index + 1] == '{')
      {
        text = this._str.Substring(lastTextStart, ++this._index - lastTextStart);
        ++this._index;
        return true;
      }
      text = this._str.Substring(lastTextStart, this._index++ - lastTextStart);
      return false;
    }

    private char ParseConversion()
    {
      ++this._index;
      if (this.CheckEnd())
        throw PythonOps.ValueError("end of format while looking for conversion specifier");
      return this._str[this._index++];
    }

    private bool CheckEnd()
    {
      if (this._index == this._str.Length)
        throw PythonOps.ValueError("unmatched '{{' in format");
      if (this._str[this._index] != '}')
        return false;
      ++this._index;
      return true;
    }

    private string ParseFormatSpec(ref int depth)
    {
      ++this._index;
      return this.ParseFieldOrSpecWorker(NewStringFormatter._brackets, ref depth);
    }

    private string ParseFieldName(ref int depth)
    {
      return this.ParseFieldOrSpecWorker(NewStringFormatter._fieldNameEnd, ref depth);
    }

    private string ParseFieldOrSpecWorker(char[] ends, ref int depth)
    {
      int index = this._index - 1;
      bool flag = false;
      do
      {
        index = this._str.IndexOfAny(ends, index + 1);
        if (index == -1)
          throw PythonOps.ValueError("unmatched '{{' in format");
        switch (this._str[index])
        {
          case '{':
            ++depth;
            break;
          case '}':
            --depth;
            break;
          default:
            flag = true;
            break;
        }
      }
      while (!flag && depth != 0);
      string fieldOrSpecWorker = this._str.Substring(this._index, index - this._index);
      this._index = index;
      return fieldOrSpecWorker;
    }
  }

  private class Formatter
  {
    private readonly PythonContext _context;
    private readonly PythonTuple _args;
    private readonly IDictionary<object, object> _kwArgs;
    private int _depth;
    private int _autoNumberedIndex;

    private Formatter(PythonContext context, PythonTuple args, IDictionary<object, object> kwArgs)
    {
      this._context = context;
      this._args = args;
      this._kwArgs = kwArgs;
    }

    public static string FormatString(
      PythonContext context,
      string format,
      PythonTuple args,
      IDictionary<object, object> kwArgs)
    {
      return new NewStringFormatter.Formatter(context, args, kwArgs).ReplaceText(format);
    }

    private string ReplaceText(string format)
    {
      if (this._depth == 2)
        throw PythonOps.ValueError("Max string recursion exceeded");
      StringBuilder stringBuilder = new StringBuilder();
      foreach (PythonTuple pythonTuple in NewStringFormatter.StringFormatParser.Parse(format))
      {
        string str1 = (string) pythonTuple[0];
        string str2 = (string) pythonTuple[1];
        string formatSpec1 = (string) pythonTuple[2];
        string str3 = (string) pythonTuple[3];
        char? conversion = str3 == null || str3.Length <= 0 ? new char?() : new char?(str3[0]);
        stringBuilder.Append(str1);
        if (str2 != null)
        {
          object argumentValue = this.GetArgumentValue(NewStringFormatter.ParseFieldName(str2, true));
          object argValue = this.ApplyConversion(conversion, argumentValue);
          string formatSpec2 = this.ReplaceComputedFormats(formatSpec1);
          stringBuilder.Append(Builtin.format(this._context.SharedContext, argValue, formatSpec2));
        }
      }
      return stringBuilder.ToString();
    }

    private string ReplaceComputedFormats(string formatSpec)
    {
      if (formatSpec.IndexOf('{') != -1)
      {
        ++this._depth;
        formatSpec = this.ReplaceText(formatSpec);
        --this._depth;
      }
      return formatSpec;
    }

    private object GetArgumentValue(NewStringFormatter.FieldName fieldName)
    {
      return this.DoAccessors(fieldName, this.GetUnaccessedObject(fieldName));
    }

    private object ApplyConversion(char? conversion, object argValue)
    {
      char? nullable = conversion;
      if (nullable.HasValue)
      {
        switch (nullable.GetValueOrDefault())
        {
          case 'r':
            argValue = (object) PythonOps.Repr(this._context.SharedContext, argValue);
            break;
          case 's':
            argValue = (object) PythonOps.ToString(this._context.SharedContext, argValue);
            break;
          default:
            throw PythonOps.ValueError("Unknown conversion specifier {0}", (object) conversion.Value);
        }
      }
      return argValue;
    }

    private object GetUnaccessedObject(NewStringFormatter.FieldName fieldName)
    {
      object unaccessedObject;
      if (fieldName.ArgumentName.Length == 0)
      {
        unaccessedObject = this._autoNumberedIndex != -1 ? this._args[this._autoNumberedIndex++] : throw PythonOps.ValueError("cannot switch from manual field specification to automatic field numbering");
      }
      else
      {
        int result;
        if (int.TryParse(fieldName.ArgumentName, out result))
        {
          this._autoNumberedIndex = this._autoNumberedIndex <= 0 ? -1 : throw PythonOps.ValueError("cannot switch from automatic field numbering to manual field specification");
          unaccessedObject = this._args[result];
        }
        else
          unaccessedObject = this._kwArgs[(object) fieldName.ArgumentName];
      }
      return unaccessedObject;
    }

    private object DoAccessors(NewStringFormatter.FieldName fieldName, object argValue)
    {
      foreach (NewStringFormatter.FieldAccessor accessor in fieldName.Accessors)
      {
        int result;
        argValue = !accessor.IsField ? (!int.TryParse(accessor.AttributeName, out result) ? PythonOps.GetIndex(this._context.SharedContext, argValue, (object) accessor.AttributeName) : PythonOps.GetIndex(this._context.SharedContext, argValue, ScriptingRuntimeHelpers.Int32ToObject(result))) : PythonOps.GetBoundAttr(this._context.SharedContext, argValue, accessor.AttributeName);
      }
      return argValue;
    }
  }

  private struct FieldName(
    string argumentName,
    IEnumerable<NewStringFormatter.FieldAccessor> accessors)
  {
    public readonly string ArgumentName = argumentName;
    public readonly IEnumerable<NewStringFormatter.FieldAccessor> Accessors = accessors;
  }

  private struct FieldAccessor(string attributeName, bool isField)
  {
    public readonly string AttributeName = attributeName;
    public readonly bool IsField = isField;
  }
}
