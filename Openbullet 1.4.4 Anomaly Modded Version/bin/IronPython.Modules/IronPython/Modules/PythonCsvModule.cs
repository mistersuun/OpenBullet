// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonCsvModule
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace IronPython.Modules;

public static class PythonCsvModule
{
  public const string __doc__ = "";
  public const string __version__ = "1.0";
  public const int QUOTE_MINIMAL = 0;
  public const int QUOTE_ALL = 1;
  public const int QUOTE_NONNUMERIC = 2;
  public const int QUOTE_NONE = 3;
  private static readonly object _fieldSizeLimitKey = new object();
  private static readonly object _dialectRegistryKey = new object();
  private const int FieldSizeLimit = 131072 /*0x020000*/;
  public static PythonType Error;

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    if (!context.HasModuleState(PythonCsvModule._fieldSizeLimitKey))
      context.SetModuleState(PythonCsvModule._fieldSizeLimitKey, (object) 131072 /*0x020000*/);
    if (!context.HasModuleState(PythonCsvModule._dialectRegistryKey))
      context.SetModuleState(PythonCsvModule._dialectRegistryKey, (object) new Dictionary<string, PythonCsvModule.Dialect>());
    PythonCsvModule.InitModuleExceptions(context, dict);
  }

  public static int field_size_limit(CodeContext context, int new_limit)
  {
    PythonContext languageContext = context.LanguageContext;
    int moduleState = (int) languageContext.GetModuleState(PythonCsvModule._fieldSizeLimitKey);
    languageContext.SetModuleState(PythonCsvModule._fieldSizeLimitKey, (object) new_limit);
    return moduleState;
  }

  public static int field_size_limit(CodeContext context)
  {
    return (int) context.LanguageContext.GetModuleState(PythonCsvModule._fieldSizeLimitKey);
  }

  [Documentation("Create a mapping from a string name to a dialect class.\r\ndialect = csv.register_dialect(name, dialect)")]
  public static void register_dialect(
    CodeContext context,
    [ParamDictionary] IDictionary<object, object> kwArgs,
    params object[] args)
  {
    object obj = (object) null;
    if (args.Length < 1)
      throw PythonOps.TypeError("expected at least 1 arguments, got {0}", (object) args.Length);
    if (args.Length > 2)
      throw PythonOps.TypeError("expected at most 2 arguments, got {0}", (object) args.Length);
    if (!(args[0] is string key))
      throw PythonOps.TypeError("dialect name must be a string or unicode");
    if (args.Length > 1)
      obj = args[1];
    PythonCsvModule.Dialect dialect1;
    if (obj == null)
      dialect1 = PythonCsvModule.Dialect.Create(context, kwArgs);
    else
      dialect1 = PythonCsvModule.Dialect.Create(context, kwArgs, obj);
    PythonCsvModule.Dialect dialect2 = dialect1;
    if (dialect2 == null)
      return;
    PythonCsvModule.GetDialects(context)[key] = dialect2;
  }

  private static Dictionary<string, PythonCsvModule.Dialect> GetDialects(CodeContext context)
  {
    PythonContext languageContext = context.LanguageContext;
    if (!languageContext.HasModuleState(PythonCsvModule._dialectRegistryKey))
      languageContext.SetModuleState(PythonCsvModule._dialectRegistryKey, (object) new Dictionary<string, PythonCsvModule.Dialect>());
    return (Dictionary<string, PythonCsvModule.Dialect>) languageContext.GetModuleState(PythonCsvModule._dialectRegistryKey);
  }

  private static int GetFieldSizeLimit(CodeContext context)
  {
    PythonContext languageContext = context.LanguageContext;
    if (!languageContext.HasModuleState(PythonCsvModule._fieldSizeLimitKey))
      languageContext.SetModuleState(PythonCsvModule._fieldSizeLimitKey, (object) 131072 /*0x020000*/);
    return (int) languageContext.GetModuleState(PythonCsvModule._fieldSizeLimitKey);
  }

  [Documentation("Delete the name/dialect mapping associated with a string name.\\n\r\n    csv.unregister_dialect(name)")]
  public static void unregister_dialect(CodeContext context, string name)
  {
    Dictionary<string, PythonCsvModule.Dialect> dialects = PythonCsvModule.GetDialects(context);
    if (name == null || !dialects.ContainsKey(name))
      throw PythonCsvModule.MakeError((object) "unknown dialect");
    if (!dialects.ContainsKey(name))
      return;
    dialects.Remove(name);
  }

  [Documentation("Return the dialect instance associated with name.\r\n    dialect = csv.get_dialect(name)")]
  public static object get_dialect(CodeContext context, string name)
  {
    Dictionary<string, PythonCsvModule.Dialect> dialects = PythonCsvModule.GetDialects(context);
    return name != null && dialects.ContainsKey(name) ? (object) dialects[name] : throw PythonCsvModule.MakeError((object) "unknown dialect");
  }

  [Documentation("Return a list of all know dialect names\r\n    names = csv.list_dialects()")]
  public static IronPython.Runtime.List list_dialects(CodeContext context)
  {
    return new IronPython.Runtime.List((ICollection) PythonCsvModule.GetDialects(context).Keys);
  }

  [Documentation("csv_reader = reader(iterable [, dialect='excel']\r\n                       [optional keyword args])\r\n    for row in csv_reader:\r\n        process(row)\r\n\r\n    The \"iterable\" argument can be any object that returns a line\r\n    of input for each iteration, such as a file object or a list.  The\r\n    optional \"dialect\" parameter is discussed below.  The function\r\n    also accepts optional keyword arguments which override settings\r\n    provided by the dialect.\r\n\r\n    The returned object is an iterator.  Each iteration returns a row\r\n    of the CSV file (which can span multiple input lines)")]
  public static object reader(
    CodeContext context,
    [ParamDictionary] IDictionary<object, object> kwArgs,
    params object[] args)
  {
    object key = (object) null;
    IEnumerator enumerator = (IEnumerator) null;
    Dictionary<string, PythonCsvModule.Dialect> dialects = PythonCsvModule.GetDialects(context);
    if (args.Length < 1)
      throw PythonOps.TypeError("expected at least 1 arguments, got {0}", (object) args.Length);
    if (args.Length > 2)
      throw PythonOps.TypeError("expected at most 2 arguments, got {0}", (object) args.Length);
    if (!PythonOps.TryGetEnumerator(context, args[0], out enumerator))
      throw PythonOps.TypeError("argument 1 must be an iterator");
    if (args.Length > 1)
      key = args[1];
    if (key is string && !dialects.ContainsKey((string) key))
      throw PythonCsvModule.MakeError((object) "unknown dialect");
    if (key is string)
      key = (object) dialects[(string) key];
    PythonCsvModule.Dialect dialect1;
    if (key == null)
      dialect1 = PythonCsvModule.Dialect.Create(context, kwArgs);
    else
      dialect1 = PythonCsvModule.Dialect.Create(context, kwArgs, key);
    PythonCsvModule.Dialect dialect2 = dialect1;
    return (object) new PythonCsvModule.Reader(context, enumerator, dialect2);
  }

  public static object writer(
    CodeContext context,
    [ParamDictionary] IDictionary<object, object> kwArgs,
    params object[] args)
  {
    object key = (object) null;
    Dictionary<string, PythonCsvModule.Dialect> dialects = PythonCsvModule.GetDialects(context);
    if (args.Length < 1)
      throw PythonOps.TypeError("expected at least 1 arguments, got {0}", (object) args.Length);
    object output_file = args.Length <= 2 ? args[0] : throw PythonOps.TypeError("expected at most 2 arguments, got {0}", (object) args.Length);
    if (args.Length > 1)
      key = args[1];
    if (key is string && !dialects.ContainsKey((string) key))
      throw PythonCsvModule.MakeError((object) "unknown dialect");
    if (key is string)
      key = (object) dialects[(string) key];
    PythonCsvModule.Dialect dialect1;
    if (key == null)
      dialect1 = PythonCsvModule.Dialect.Create(context, kwArgs);
    else
      dialect1 = PythonCsvModule.Dialect.Create(context, kwArgs, key);
    PythonCsvModule.Dialect dialect2 = dialect1;
    return (object) new PythonCsvModule.Writer(context, output_file, dialect2);
  }

  internal static Exception MakeError(params object[] args)
  {
    return PythonOps.CreateThrowable(PythonCsvModule.Error, args);
  }

  private static void InitModuleExceptions(PythonContext context, PythonDictionary dict)
  {
    PythonCsvModule.Error = context.EnsureModuleException((object) "csv.Error", PythonExceptions.StandardError, dict, "Error", "_csv");
  }

  [Documentation("CSV dialect\r\nThe Dialect type records CSV parsing and generation options.")]
  [PythonType]
  public class Dialect
  {
    private string _delimiter = ",";
    private string _escapechar;
    private bool _skipinitialspace;
    private bool _doublequote = true;
    private bool _strict;
    private int _quoting;
    private string _quotechar = "\"";
    private string _lineterminator = "\r\n";
    private static readonly string[] VALID_KWARGS = new string[9]
    {
      "dialect",
      nameof (delimiter),
      nameof (doublequote),
      nameof (escapechar),
      nameof (lineterminator),
      nameof (quotechar),
      nameof (quoting),
      nameof (skipinitialspace),
      nameof (strict)
    };

    private Dialect()
    {
    }

    public static PythonCsvModule.Dialect Create(
      CodeContext context,
      [ParamDictionary] IDictionary<object, object> kwArgs,
      params object[] args)
    {
      object obj1 = (object) null;
      object obj2 = (object) null;
      object obj3 = (object) null;
      object obj4 = (object) null;
      object obj5 = (object) null;
      object obj6 = (object) null;
      object obj7 = (object) null;
      object obj8 = (object) null;
      object obj9 = (object) null;
      Dictionary<string, PythonCsvModule.Dialect> dialects = PythonCsvModule.GetDialects(context);
      if (args.Length != 0 && args[0] != null)
        obj1 = args[0];
      if (obj1 == null)
        kwArgs.TryGetValue((object) "dialect", out obj1);
      kwArgs.TryGetValue((object) "delimiter", out obj2);
      kwArgs.TryGetValue((object) "doublequote", out obj3);
      kwArgs.TryGetValue((object) "escapechar", out obj4);
      kwArgs.TryGetValue((object) "lineterminator", out obj5);
      kwArgs.TryGetValue((object) "quotechar", out obj6);
      kwArgs.TryGetValue((object) "quoting", out obj7);
      kwArgs.TryGetValue((object) "skipinitialspace", out obj8);
      kwArgs.TryGetValue((object) "strict", out obj9);
      if (obj1 != null)
      {
        if (obj1 is string)
        {
          string key = (string) obj1;
          obj1 = dialects.ContainsKey(key) ? (object) dialects[key] : throw PythonCsvModule.MakeError((object) "unknown dialect");
        }
        if (obj1 is PythonCsvModule.Dialect && obj2 == null && obj3 == null && obj4 == null && obj5 == null && obj6 == null && obj7 == null && obj8 == null && obj9 == null)
          return obj1 as PythonCsvModule.Dialect;
      }
      if (obj1 == null)
        return new PythonCsvModule.Dialect(context, kwArgs, new object[0]);
      return new PythonCsvModule.Dialect(context, kwArgs, new object[1]
      {
        obj1
      });
    }

    [SpecialName]
    public void DeleteMember(CodeContext context, string name)
    {
      if (string.Compare(name, "delimiter") == 0 || string.Compare(name, "skipinitialspace") == 0 || string.Compare(name, "doublequote") == 0 || string.Compare(name, "strict") == 0)
        throw PythonOps.TypeError("readonly attribute");
      if (string.Compare(name, "escapechar") == 0 || string.Compare(name, "lineterminator") == 0 || string.Compare(name, "quotechar") == 0 || string.Compare(name, "quoting") == 0)
        throw PythonOps.AttributeError("attribute '{0}' of '_csv.Dialect' objects is not writable", (object) name);
      throw PythonOps.AttributeError("'_csv.Dialect' object has no attribute '{0}'", (object) name);
    }

    [SpecialName]
    public void SetMember(CodeContext context, string name, object value)
    {
      if (string.Compare(name, "delimiter") == 0 || string.Compare(name, "skipinitialspace") == 0 || string.Compare(name, "doublequote") == 0 || string.Compare(name, "strict") == 0)
        throw PythonOps.TypeError("readonly attribute");
      if (string.Compare(name, "escapechar") == 0 || string.Compare(name, "lineterminator") == 0 || string.Compare(name, "quotechar") == 0 || string.Compare(name, "quoting") == 0)
        throw PythonOps.AttributeError("attribute '{0}' of '_csv.Dialect' objects is not writable", (object) name);
      throw PythonOps.AttributeError("'_csv.Dialect' object has no attribute '{0}'", (object) name);
    }

    private static int SetInt(string name, object src, bool found, int @default)
    {
      num = @default;
      if (found && !(src is int num))
        throw PythonOps.TypeError("\"{0}\" must be an integer", (object) name);
      return num;
    }

    private static bool SetBool(string name, object src, bool found, bool @default)
    {
      bool flag = @default;
      if (found)
        flag = PythonOps.IsTrue(src);
      return flag;
    }

    private static string SetChar(string name, object src, bool found, string @default)
    {
      string str1 = @default;
      if (found)
      {
        if (src == null)
        {
          str1 = (string) null;
        }
        else
        {
          string str2 = src is string ? src as string : throw PythonOps.TypeError("\"{0}\" must be string, not {1}", (object) name, (object) PythonOps.GetPythonTypeName(src));
          if (str2.Length == 0)
            str1 = (string) null;
          else
            str1 = str2.Length == 1 ? str2.Substring(0, 1) : throw PythonOps.TypeError("\"{0}\" must be an 1-character string", (object) name);
        }
      }
      return str1;
    }

    private static string SetString(string name, object src, bool found, string @default)
    {
      string str = @default;
      if (found)
      {
        if (src == null)
          str = (string) null;
        else
          str = src is string ? src as string : throw PythonOps.TypeError("\"{0}\" must be a string", (object) name);
      }
      return str;
    }

    public Dialect(CodeContext context, [ParamDictionary] IDictionary<object, object> kwArgs, params object[] args)
    {
      object o = (object) null;
      object ret1 = (object) null;
      object ret2 = (object) null;
      object ret3 = (object) null;
      object ret4 = (object) null;
      object ret5 = (object) null;
      object ret6 = (object) null;
      object ret7 = (object) null;
      object ret8 = (object) null;
      Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
      foreach (object key in (IEnumerable<object>) kwArgs.Keys)
      {
        if (Array.IndexOf<object>((object[]) PythonCsvModule.Dialect.VALID_KWARGS, key) < 0)
          throw PythonOps.TypeError("'{0}' is an invalid keyword argument for this function", key);
      }
      if (args.Length != 0 && args[0] != null)
      {
        o = args[0];
        dictionary["dialect"] = true;
      }
      if (o == null)
        dictionary["dialect"] = kwArgs.TryGetValue((object) "dialect", out o);
      dictionary[nameof (delimiter)] = kwArgs.TryGetValue((object) nameof (delimiter), out ret1);
      dictionary[nameof (doublequote)] = kwArgs.TryGetValue((object) nameof (doublequote), out ret2);
      dictionary[nameof (escapechar)] = kwArgs.TryGetValue((object) nameof (escapechar), out ret3);
      dictionary[nameof (lineterminator)] = kwArgs.TryGetValue((object) nameof (lineterminator), out ret4);
      dictionary[nameof (quotechar)] = kwArgs.TryGetValue((object) nameof (quotechar), out ret5);
      dictionary[nameof (quoting)] = kwArgs.TryGetValue((object) nameof (quoting), out ret6);
      dictionary[nameof (skipinitialspace)] = kwArgs.TryGetValue((object) nameof (skipinitialspace), out ret7);
      dictionary[nameof (strict)] = kwArgs.TryGetValue((object) nameof (strict), out ret8);
      if (o != null)
      {
        if (!dictionary[nameof (delimiter)] && ret1 == null)
          dictionary[nameof (delimiter)] = PythonOps.TryGetBoundAttr(o, nameof (delimiter), out ret1);
        if (!dictionary[nameof (doublequote)] && ret2 == null)
          dictionary[nameof (doublequote)] = PythonOps.TryGetBoundAttr(o, nameof (doublequote), out ret2);
        if (!dictionary[nameof (escapechar)] && ret3 == null)
          dictionary[nameof (escapechar)] = PythonOps.TryGetBoundAttr(o, nameof (escapechar), out ret3);
        if (!dictionary[nameof (lineterminator)] && ret4 == null)
          dictionary[nameof (lineterminator)] = PythonOps.TryGetBoundAttr(o, nameof (lineterminator), out ret4);
        if (!dictionary[nameof (quotechar)] && ret5 == null)
          dictionary[nameof (quotechar)] = PythonOps.TryGetBoundAttr(o, nameof (quotechar), out ret5);
        if (!dictionary[nameof (quoting)] && ret6 == null)
          dictionary[nameof (quoting)] = PythonOps.TryGetBoundAttr(o, nameof (quoting), out ret6);
        if (!dictionary[nameof (skipinitialspace)] && ret7 == null)
          dictionary[nameof (skipinitialspace)] = PythonOps.TryGetBoundAttr(o, nameof (skipinitialspace), out ret7);
        if (!dictionary[nameof (strict)] && ret8 == null)
          dictionary[nameof (strict)] = PythonOps.TryGetBoundAttr(o, nameof (strict), out ret8);
      }
      this._delimiter = PythonCsvModule.Dialect.SetChar(nameof (delimiter), ret1, dictionary[nameof (delimiter)], ",");
      this._doublequote = PythonCsvModule.Dialect.SetBool(nameof (doublequote), ret2, dictionary[nameof (doublequote)], true);
      this._escapechar = PythonCsvModule.Dialect.SetString(nameof (escapechar), ret3, dictionary[nameof (escapechar)], (string) null);
      this._lineterminator = PythonCsvModule.Dialect.SetString(nameof (lineterminator), ret4, dictionary[nameof (lineterminator)], "\r\n");
      this._quotechar = PythonCsvModule.Dialect.SetChar(nameof (quotechar), ret5, dictionary[nameof (quotechar)], "\"");
      this._quoting = PythonCsvModule.Dialect.SetInt(nameof (quoting), ret6, dictionary[nameof (quoting)], 0);
      this._skipinitialspace = PythonCsvModule.Dialect.SetBool(nameof (skipinitialspace), ret7, dictionary[nameof (skipinitialspace)], false);
      this._strict = PythonCsvModule.Dialect.SetBool(nameof (strict), ret8, dictionary[nameof (strict)], false);
      if (this._quoting < 0 || this._quoting > 3)
        throw PythonOps.TypeError("bad \"quoting\" value");
      if (string.IsNullOrEmpty(this._delimiter))
        throw PythonOps.TypeError("\"delimiter\" must be an 1-character string");
      if (dictionary[nameof (quotechar)] && ret5 == null && ret6 == null)
        this._quoting = 3;
      if (this._quoting != 3 && string.IsNullOrEmpty(this._quotechar))
        throw PythonOps.TypeError("quotechar must be set if quoting enabled");
      if (this._lineterminator == null)
        throw PythonOps.TypeError("lineterminator must be set");
    }

    public string escapechar => this._escapechar;

    public string delimiter => this._delimiter;

    public bool skipinitialspace => this._skipinitialspace;

    public bool doublequote => this._doublequote;

    public string lineterminator => this._lineterminator;

    public bool strict => this._strict;

    public int quoting => this._quoting;

    public string quotechar => this._quotechar;
  }

  [Documentation("CSV reader\r\n\r\nReader objects are responsible for reading and parsing tabular data\r\nin CSV format.")]
  [PythonType]
  public class Reader : IEnumerable
  {
    private IEnumerator _input_iter;
    private PythonCsvModule.Dialect _dialect;
    private int _line_num;
    private PythonCsvModule.Reader.ReaderIterator _iterator;

    public Reader(CodeContext context, IEnumerator input_iter, PythonCsvModule.Dialect dialect)
    {
      this._input_iter = input_iter;
      this._dialect = dialect;
      this._iterator = new PythonCsvModule.Reader.ReaderIterator(context, this);
    }

    public object next()
    {
      return this._iterator.MoveNext() ? this._iterator.Current : throw PythonOps.StopIteration();
    }

    public IEnumerator GetEnumerator() => (IEnumerator) this._iterator;

    public object dialect => (object) this._dialect;

    public int line_num => this._line_num;

    private sealed class ReaderIterator : IEnumerator, IEnumerable
    {
      private CodeContext _context;
      private PythonCsvModule.Reader _reader;
      private IronPython.Runtime.List _fields = new IronPython.Runtime.List();
      private bool _is_numeric_field;
      private PythonCsvModule.Reader.ReaderIterator.State _state;
      private StringBuilder _field = new StringBuilder();
      private IEnumerator _iterator;

      public ReaderIterator(CodeContext context, PythonCsvModule.Reader reader)
      {
        this._context = context;
        this._reader = reader;
        this._iterator = this._reader._input_iter;
      }

      public object Current => (object) new IronPython.Runtime.List((ICollection) this._fields);

      public bool MoveNext()
      {
        this.Reset();
        bool flag;
        do
        {
          if (!this._iterator.MoveNext())
          {
            if (this._field.Length <= 0 && this._state != PythonCsvModule.Reader.ReaderIterator.State.InQuotedField)
              return false;
            if (this._reader._dialect.strict)
              throw PythonCsvModule.MakeError((object) "unexpected end of data");
            this.ParseSaveField();
            return true;
          }
          object current = this._iterator.Current;
          ++this._reader._line_num;
          if (current is char)
            current = (object) current.ToString();
          string str = current is string ? current as string : throw PythonOps.TypeError("expected string or Unicode object, {0} found", (object) DynamicHelpers.GetPythonType((object) current.GetType()));
          if (!string.IsNullOrEmpty(str))
          {
            for (int index = 0; index < str.Length; ++index)
            {
              char c = str[index];
              if (c == char.MinValue)
                throw PythonCsvModule.MakeError((object) "line contains NULL byte");
              this.ProcessChar(c);
            }
          }
          this.ProcessChar(char.MinValue);
          flag = true;
        }
        while (this._state != PythonCsvModule.Reader.ReaderIterator.State.StartRecord);
        return flag;
      }

      public void Reset()
      {
        this._state = PythonCsvModule.Reader.ReaderIterator.State.StartRecord;
        this._fields.Clear();
        this._is_numeric_field = false;
        this._field.Clear();
      }

      public IEnumerator GetEnumerator() => (IEnumerator) this;

      private void ProcessChar(char c)
      {
        PythonCsvModule.Dialect dialect = this._reader._dialect;
        switch (this._state)
        {
          case PythonCsvModule.Reader.ReaderIterator.State.StartRecord:
            switch (c)
            {
              case char.MinValue:
                return;
              case '\n':
              case '\r':
                this._state = PythonCsvModule.Reader.ReaderIterator.State.EatCrNl;
                return;
              default:
                this._state = PythonCsvModule.Reader.ReaderIterator.State.StartField;
                goto label_5;
            }
          case PythonCsvModule.Reader.ReaderIterator.State.StartField:
label_5:
            if (c == '\n' || c == '\r' || c == char.MinValue)
            {
              this.ParseSaveField();
              this._state = c == char.MinValue ? PythonCsvModule.Reader.ReaderIterator.State.StartRecord : PythonCsvModule.Reader.ReaderIterator.State.EatCrNl;
              break;
            }
            if (!string.IsNullOrEmpty(dialect.quotechar) && (int) c == (int) dialect.quotechar[0] && dialect.quoting != 3)
            {
              this._state = PythonCsvModule.Reader.ReaderIterator.State.InQuotedField;
              break;
            }
            if (!string.IsNullOrEmpty(dialect.escapechar) && (int) c == (int) dialect.escapechar[0])
            {
              this._state = PythonCsvModule.Reader.ReaderIterator.State.EscapedChar;
              break;
            }
            if (c == ' ' && dialect.skipinitialspace)
              break;
            if ((int) c == (int) dialect.delimiter[0])
            {
              this.ParseSaveField();
              break;
            }
            if (dialect.quoting == 2)
              this._is_numeric_field = true;
            this.ParseAddChar(c);
            this._state = PythonCsvModule.Reader.ReaderIterator.State.InField;
            break;
          case PythonCsvModule.Reader.ReaderIterator.State.EscapedChar:
            if (c == char.MinValue)
              c = '\n';
            this.ParseAddChar(c);
            this._state = PythonCsvModule.Reader.ReaderIterator.State.InField;
            break;
          case PythonCsvModule.Reader.ReaderIterator.State.InField:
            if (c == '\n' || c == '\r' || c == char.MinValue)
            {
              this.ParseSaveField();
              this._state = c == char.MinValue ? PythonCsvModule.Reader.ReaderIterator.State.StartRecord : PythonCsvModule.Reader.ReaderIterator.State.EatCrNl;
              break;
            }
            if (!string.IsNullOrEmpty(dialect.escapechar) && (int) c == (int) dialect.escapechar[0])
            {
              this._state = PythonCsvModule.Reader.ReaderIterator.State.EscapedChar;
              break;
            }
            if ((int) c == (int) dialect.delimiter[0])
            {
              this.ParseSaveField();
              this._state = PythonCsvModule.Reader.ReaderIterator.State.StartField;
              break;
            }
            this.ParseAddChar(c);
            break;
          case PythonCsvModule.Reader.ReaderIterator.State.InQuotedField:
            if (c == char.MinValue)
              break;
            if (!string.IsNullOrEmpty(dialect.escapechar) && (int) c == (int) dialect.escapechar[0])
            {
              this._state = PythonCsvModule.Reader.ReaderIterator.State.EscapeInQuotedField;
              break;
            }
            if (!string.IsNullOrEmpty(dialect.quotechar) && (int) c == (int) dialect.quotechar[0] && dialect.quoting != 3)
            {
              if (dialect.doublequote)
              {
                this._state = PythonCsvModule.Reader.ReaderIterator.State.QuoteInQuotedField;
                break;
              }
              this._state = PythonCsvModule.Reader.ReaderIterator.State.InField;
              break;
            }
            this.ParseAddChar(c);
            break;
          case PythonCsvModule.Reader.ReaderIterator.State.EscapeInQuotedField:
            if (c == char.MinValue)
              c = '\n';
            this.ParseAddChar(c);
            this._state = PythonCsvModule.Reader.ReaderIterator.State.InQuotedField;
            break;
          case PythonCsvModule.Reader.ReaderIterator.State.QuoteInQuotedField:
            if (dialect.quoting != 3 && (int) c == (int) dialect.quotechar[0])
            {
              this.ParseAddChar(c);
              this._state = PythonCsvModule.Reader.ReaderIterator.State.InQuotedField;
              break;
            }
            if ((int) c == (int) dialect.delimiter[0])
            {
              this.ParseSaveField();
              this._state = PythonCsvModule.Reader.ReaderIterator.State.StartField;
              break;
            }
            if (c == '\n' || c == '\r' || c == char.MinValue)
            {
              this.ParseSaveField();
              this._state = c == char.MinValue ? PythonCsvModule.Reader.ReaderIterator.State.StartRecord : PythonCsvModule.Reader.ReaderIterator.State.EatCrNl;
              break;
            }
            if (!dialect.strict)
            {
              this.ParseAddChar(c);
              this._state = PythonCsvModule.Reader.ReaderIterator.State.InField;
              break;
            }
            throw PythonCsvModule.MakeError((object) "'{0}' expected after '{1}'", (object) dialect.delimiter, (object) dialect.quotechar);
          case PythonCsvModule.Reader.ReaderIterator.State.EatCrNl:
            if (c == '\n' || c == '\r')
              break;
            if (c == char.MinValue)
            {
              this._state = PythonCsvModule.Reader.ReaderIterator.State.StartRecord;
              break;
            }
            throw PythonCsvModule.MakeError((object) "new-line character seen in unquoted field - do you need to open the file in universal-newline mode?");
        }
      }

      private void ParseAddChar(char c)
      {
        int fieldSizeLimit = PythonCsvModule.GetFieldSizeLimit(this._context);
        if (this._field.Length >= fieldSizeLimit)
          throw PythonCsvModule.MakeError((object) $"field larger than field limit ({fieldSizeLimit})");
        this._field.Append(c);
      }

      private void ParseSaveField()
      {
        string s = this._field.ToString();
        if (this._is_numeric_field)
        {
          this._is_numeric_field = false;
          double result;
          if (double.TryParse(s, out result))
          {
            if (s.Contains("."))
              this._fields.Add((object) result);
            else
              this._fields.Add((object) (int) result);
          }
          else
            throw PythonOps.ValueError("invalid literal for float(): {0}", (object) s);
        }
        else
          this._fields.Add((object) s);
        this._field.Clear();
      }

      private enum State
      {
        StartRecord,
        StartField,
        EscapedChar,
        InField,
        InQuotedField,
        EscapeInQuotedField,
        QuoteInQuotedField,
        EatCrNl,
      }
    }
  }

  [Documentation("CSV writer\r\n\r\nWriter objects are responsible for generating tabular data\r\nin CSV format from sequence input.")]
  [PythonType]
  public class Writer
  {
    private PythonCsvModule.Dialect _dialect;
    private object _writeline;
    private List<string> _rec = new List<string>();
    private int _num_fields;

    public Writer(CodeContext context, object output_file, PythonCsvModule.Dialect dialect)
    {
      this._dialect = dialect;
      if (!PythonOps.TryGetBoundAttr(output_file, "write", out this._writeline) || this._writeline == null || !PythonOps.IsCallable(context, this._writeline))
        throw PythonOps.TypeError("argument 1 must have a \"write\" method");
    }

    public object dialect => (object) this._dialect;

    [Documentation("writerow(sequence)\r\n\r\nConstruct and write a CSV record from a sequence of fields.  Non-string\r\nelements will be converted to string.")]
    public void writerow(CodeContext context, object sequence)
    {
      IEnumerator enumerator = (IEnumerator) null;
      int num = PythonOps.TryGetEnumerator(context, sequence, out enumerator) ? PythonOps.Length(sequence) : throw PythonCsvModule.MakeError((object) "sequence expected");
      this.JoinReset();
      while (enumerator.MoveNext())
      {
        object current = enumerator.Current;
        bool quoted = false;
        switch (this._dialect.quoting)
        {
          case 1:
            quoted = true;
            break;
          case 2:
            quoted = !PythonOps.CheckingConvertToFloat(current) && !PythonOps.CheckingConvertToInt(current) && !PythonOps.CheckingConvertToLong(current);
            break;
        }
        switch (current)
        {
          case string _:
            this.JoinAppend((string) current, quoted, num == 1);
            continue;
          case double self1:
            this.JoinAppend(DoubleOps.__repr__(context, self1), quoted, num == 1);
            continue;
          case float self2:
            this.JoinAppend(SingleOps.__repr__(context, self2), quoted, num == 1);
            continue;
          case null:
            this.JoinAppend(string.Empty, quoted, num == 1);
            continue;
          default:
            this.JoinAppend(current.ToString(), quoted, num == 1);
            continue;
        }
      }
      this._rec.Add(this._dialect.lineterminator);
      PythonOps.CallWithContext(context, this._writeline, (object) string.Join("", this._rec.ToArray()));
    }

    [Documentation("writerows(sequence of sequences)\r\n\r\nConstruct and write a series of sequences to a csv file.  Non-string \r\nelements will be converted to string.")]
    public void writerows(CodeContext context, object sequence)
    {
      IEnumerator enumerator = (IEnumerator) null;
      if (!PythonOps.TryGetEnumerator(context, sequence, out enumerator))
        throw PythonOps.TypeError("writerows() argument must be iterable");
      while (enumerator.MoveNext())
        this.writerow(context, enumerator.Current);
    }

    private void JoinReset()
    {
      this._num_fields = 0;
      this._rec.Clear();
    }

    private void JoinAppend(string field, bool quoted, bool quote_empty)
    {
      if (this._num_fields > 0)
        this._rec.Add(this._dialect.delimiter);
      List<char> charList1 = new List<char>();
      if (this._dialect.quoting == 3)
      {
        charList1.AddRange((IEnumerable<char>) this._dialect.lineterminator.ToCharArray());
        if (!string.IsNullOrEmpty(this._dialect.escapechar))
          charList1.Add(this._dialect.escapechar[0]);
        if (!string.IsNullOrEmpty(this._dialect.delimiter))
          charList1.Add(this._dialect.delimiter[0]);
        if (!string.IsNullOrEmpty(this._dialect.quotechar))
          charList1.Add(this._dialect.quotechar[0]);
      }
      else
      {
        List<char> charList2 = new List<char>();
        charList2.AddRange((IEnumerable<char>) this._dialect.lineterminator.ToCharArray());
        if (!string.IsNullOrEmpty(this._dialect.delimiter))
          charList2.Add(this._dialect.delimiter[0]);
        if (!string.IsNullOrEmpty(this._dialect.escapechar))
          charList2.Add(this._dialect.escapechar[0]);
        if (field.IndexOfAny(charList2.ToArray()) >= 0)
          quoted = true;
        charList1.Clear();
        if (!string.IsNullOrEmpty(this._dialect.quotechar) && field.Contains(this._dialect.quotechar))
        {
          if (this._dialect.doublequote)
          {
            field = field.Replace(this._dialect.quotechar, this._dialect.quotechar + this._dialect.quotechar);
            quoted = true;
          }
          else
            charList1.Add(this._dialect.quotechar[0]);
        }
      }
      foreach (char ch in charList1)
      {
        if (field.IndexOf(ch) >= 0)
        {
          if (string.IsNullOrEmpty(this._dialect.escapechar))
            throw PythonCsvModule.MakeError((object) "need to escape, but no escapechar set");
          field = field.Replace(ch.ToString(), this._dialect.escapechar + ch.ToString());
        }
      }
      if (string.IsNullOrEmpty(field) & quote_empty)
      {
        if (this._dialect.quoting == 3)
          throw PythonCsvModule.MakeError((object) "single empty field record must be quoted");
        quoted = true;
      }
      if (quoted)
        field = this._dialect.quotechar + field + this._dialect.quotechar;
      this._rec.Add(field);
      ++this._num_fields;
    }
  }
}
