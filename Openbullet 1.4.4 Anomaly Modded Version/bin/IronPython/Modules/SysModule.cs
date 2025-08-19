// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.SysModule
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;

#nullable disable
namespace IronPython.Modules;

public static class SysModule
{
  public const string __doc__ = "Provides access to functions which query or manipulate the Python runtime.";
  public const int api_version = 0;
  public static readonly string byteorder = BitConverter.IsLittleEndian ? "little" : "big";
  public const string copyright = "Copyright (c) IronPython Team";
  public static BuiltinFunction displayhook = BuiltinFunction.MakeFunction(nameof (displayhook), ArrayUtils.ConvertAll<MemberInfo, MethodBase>(typeof (SysModule).GetMember("displayhookImpl"), (Func<MemberInfo, MethodBase>) (x => (MethodBase) x)), typeof (SysModule));
  public static readonly BuiltinFunction __displayhook__ = SysModule.displayhook;
  public const int dllhandle = 0;
  public static readonly BuiltinFunction excepthook = BuiltinFunction.MakeFunction(nameof (excepthook), ArrayUtils.ConvertAll<MemberInfo, MethodBase>(typeof (SysModule).GetMember("excepthookImpl"), (Func<MemberInfo, MethodBase>) (x => (MethodBase) x)), typeof (SysModule));
  public static readonly BuiltinFunction __excepthook__ = SysModule.excepthook;
  public const int maxint = 2147483647 /*0x7FFFFFFF*/;
  public const int maxsize = 2147483647 /*0x7FFFFFFF*/;
  public const int maxunicode = 65535 /*0xFFFF*/;
  public const string platform = "cli";
  public static readonly string prefix = SysModule.GetPrefix();
  public static PythonTuple subversion = PythonTuple.MakeTuple((object) "IronPython", (object) "", (object) "");
  public const string winver = "2.7";
  public static SysModule.longinfo long_info = new SysModule.longinfo(32 /*0x20*/, 4);
  public static SysModule.floatinfo float_info = new SysModule.floatinfo(double.MaxValue, 1024 /*0x0400*/, 308, BitConverter.Int64BitsToDouble(BitConverter.IsLittleEndian ? 4503599627370496L /*0x10000000000000*/ : 4096L /*0x1000*/), -1021, -307, 15, 53, BitConverter.Int64BitsToDouble(BitConverter.IsLittleEndian ? 4372995238176751616L /*0x3CB0000000000000*/ : 45116L), 2, 1);

  private static string GetPrefix()
  {
    try
    {
      return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
    catch (SecurityException ex)
    {
      return string.Empty;
    }
    catch (ArgumentException ex)
    {
      return string.Empty;
    }
    catch (MethodAccessException ex)
    {
      return string.Empty;
    }
  }

  public static object callstats() => (object) null;

  [PythonHidden(new PlatformID[] {})]
  [Documentation("displayhook(object) -> None\r\n\r\nPrint an object to sys.stdout and also save it in __builtin__._")]
  public static void displayhookImpl(CodeContext context, object value)
  {
    if (value == null)
      return;
    PythonOps.Print(context, (object) PythonOps.Repr(context, value));
    context.LanguageContext.BuiltinModuleDict[(object) "_"] = value;
  }

  [PythonHidden(new PlatformID[] {})]
  [Documentation("excepthook(exctype, value, traceback) -> None\r\n\r\nHandle an exception by displaying it with a traceback on sys.stderr._")]
  public static void excepthookImpl(
    CodeContext context,
    object exctype,
    object value,
    object traceback)
  {
    PythonContext languageContext = context.LanguageContext;
    PythonOps.PrintWithDest(context, languageContext.SystemStandardError, (object) languageContext.FormatException(PythonExceptions.ToClr(value)));
  }

  public static int getcheckinterval()
  {
    throw PythonOps.NotImplementedError("IronPython does not support sys.getcheckinterval");
  }

  public static void setcheckinterval(int value)
  {
    throw PythonOps.NotImplementedError("IronPython does not support sys.setcheckinterval");
  }

  public static int getrefcount(CodeContext context, object o)
  {
    PythonOps.Warn(context, PythonExceptions.RuntimeWarning, "IronPython does not support sys.getrefcount. A dummy result is returned.");
    return 1000000;
  }

  [Python3Warning("sys.exc_clear() not supported in 3.x; use except clauses")]
  public static void exc_clear() => PythonOps.ClearCurrentException();

  public static PythonTuple exc_info(CodeContext context) => PythonOps.GetExceptionInfo(context);

  public static void exit() => SysModule.exit((object) null);

  public static void exit(object code)
  {
    if (code == null)
      throw new PythonExceptions._SystemExit().InitAndGetClrException();
    throw new PythonExceptions._SystemExit().InitAndGetClrException(code);
  }

  public static string getdefaultencoding(CodeContext context)
  {
    return context.LanguageContext.GetDefaultEncodingName();
  }

  public static object getfilesystemencoding()
  {
    return Environment.OSVersion.Platform == PlatformID.Unix ? (object) "utf-8" : (object) "mbcs";
  }

  [PythonHidden(new PlatformID[] {})]
  public static TraceBackFrame _getframeImpl(CodeContext context)
  {
    return SysModule._getframeImpl(context, 0);
  }

  [PythonHidden(new PlatformID[] {})]
  public static TraceBackFrame _getframeImpl(CodeContext context, int depth)
  {
    return SysModule._getframeImpl(context, depth, PythonOps.GetFunctionStack());
  }

  internal static TraceBackFrame _getframeImpl(
    CodeContext context,
    int depth,
    List<FunctionStack> stack)
  {
    if (depth >= stack.Count)
      throw PythonOps.ValueError("call stack is not deep enough");
    TraceBackFrame traceBackFrame = (TraceBackFrame) null;
    for (int index = 0; index < stack.Count - depth; ++index)
    {
      FunctionStack functionStack = stack[index];
      if (functionStack.Frame != null)
      {
        traceBackFrame = functionStack.Frame;
      }
      else
      {
        traceBackFrame = new TraceBackFrame(context, Builtin.globals(functionStack.Context), Builtin.locals(functionStack.Context), functionStack.Code, traceBackFrame);
        stack[index] = new FunctionStack(functionStack.Context, functionStack.Code, traceBackFrame);
      }
    }
    return traceBackFrame;
  }

  public static int getsizeof(object o) => ObjectOps.__sizeof__(o);

  public static PythonTuple getwindowsversion()
  {
    OperatingSystem osVersion = Environment.OSVersion;
    return (PythonTuple) new SysModule.windows_version(osVersion.Version.Major, osVersion.Version.Minor, osVersion.Version.Build, (int) osVersion.Platform, osVersion.ServicePack);
  }

  public static void setdefaultencoding(CodeContext context, object name)
  {
    if (name == null)
      throw PythonOps.TypeError("name cannot be None");
    if (!(name is string name1))
      throw PythonOps.TypeError("name must be a string");
    PythonContext languageContext = context.LanguageContext;
    Encoding encoding;
    if (!StringOps.TryGetEncoding(name1, out encoding))
      throw PythonOps.LookupError("'{0}' does not match any available encodings", (object) name1);
    languageContext.DefaultEncoding = encoding;
  }

  public static void settrace(CodeContext context, object o) => context.LanguageContext.SetTrace(o);

  public static object call_tracing(CodeContext context, object func, PythonTuple args)
  {
    return context.LanguageContext.CallTracing(func, args);
  }

  public static object gettrace(CodeContext context) => context.LanguageContext.GetTrace();

  public static void setrecursionlimit(CodeContext context, int limit)
  {
    context.LanguageContext.RecursionLimit = limit;
  }

  public static int getrecursionlimit(CodeContext context)
  {
    return context.LanguageContext.RecursionLimit;
  }

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    dict[(object) "stdin"] = dict[(object) "__stdin__"];
    dict[(object) "stdout"] = dict[(object) "__stdout__"];
    dict[(object) "stderr"] = dict[(object) "__stderr__"];
    dict[(object) "warnoptions"] = (object) new IronPython.Runtime.List(0);
    SysModule.PublishBuiltinModuleNames(context, dict);
    context.SetHostVariables(dict);
    dict[(object) "meta_path"] = (object) new IronPython.Runtime.List(0);
    dict[(object) "path_hooks"] = (object) new IronPython.Runtime.List(0);
    try
    {
      if (Importer.ImportModule(context.SharedClsContext, (object) context.SharedClsContext.GlobalDict, "zipimport", false, -1) is PythonModule o)
      {
        object boundAttr = PythonOps.GetBoundAttr(context.SharedClsContext, (object) o, "zipimporter");
        if (dict[(object) "path_hooks"] is IronPython.Runtime.List list)
        {
          if (boundAttr != null)
            list.Add(boundAttr);
        }
      }
    }
    catch
    {
    }
    dict[(object) "path_importer_cache"] = (object) new PythonDictionary();
  }

  internal static void PublishBuiltinModuleNames(PythonContext context, PythonDictionary dict)
  {
    object[] objArray = new object[context.BuiltinModules.Keys.Count];
    int num = 0;
    foreach (object key in context.BuiltinModules.Keys)
      objArray[num++] = key;
    dict[(object) "builtin_module_names"] = (object) PythonTuple.MakeTuple(objArray);
  }

  [PythonType("sys.getwindowsversion")]
  [PythonHidden(new PlatformID[] {})]
  public class windows_version : PythonTuple
  {
    public readonly int major;
    public readonly int minor;
    public readonly int build;
    public readonly int platform;
    public readonly string service_pack;
    public const int n_fields = 5;
    public const int n_sequence_fields = 5;
    public const int n_unnamed_fields = 0;

    internal windows_version(int major, int minor, int build, int platform, string service_pack)
      : base(new object[5]
      {
        (object) major,
        (object) minor,
        (object) build,
        (object) platform,
        (object) service_pack
      })
    {
      this.major = major;
      this.minor = minor;
      this.build = build;
      this.platform = platform;
      this.service_pack = service_pack;
    }

    public override string __repr__(CodeContext context)
    {
      return $"sys.getwindowsversion(major={this.major}, minor={this.minor}, build={this.build}, platform={this.platform}, service_pack='{this.service_pack}')";
    }
  }

  [PythonHidden(new PlatformID[] {})]
  [PythonType("flags")]
  [DontMapIEnumerableToIter]
  public sealed class SysFlags : IList<object>, ICollection<object>, IEnumerable<object>, IEnumerable
  {
    private const string _className = "sys.flags";
    private const int INDEX_DEBUG = 0;
    private const int INDEX_PY3K_WARNING = 1;
    private const int INDEX_DIVISION_WARNING = 2;
    private const int INDEX_DIVISION_NEW = 3;
    private const int INDEX_INSPECT = 4;
    private const int INDEX_INTERACTIVE = 5;
    private const int INDEX_OPTIMIZE = 6;
    private const int INDEX_DONT_WRITE_BYTECODE = 7;
    private const int INDEX_NO_USER_SITE = 8;
    private const int INDEX_NO_SITE = 9;
    private const int INDEX_IGNORE_ENVIRONMENT = 10;
    private const int INDEX_TABCHECK = 11;
    private const int INDEX_VERBOSE = 12;
    private const int INDEX_UNICODE = 13;
    private const int INDEX_BYTES_WARNING = 14;
    public const int n_fields = 15;
    public const int n_sequence_fields = 15;
    public const int n_unnamed_fields = 0;
    private static readonly string[] _keys = new string[15]
    {
      nameof (debug),
      nameof (py3k_warning),
      nameof (division_warning),
      nameof (division_new),
      nameof (inspect),
      nameof (interactive),
      nameof (optimize),
      nameof (dont_write_bytecode),
      nameof (no_user_site),
      nameof (no_site),
      nameof (ignore_environment),
      nameof (tabcheck),
      nameof (verbose),
      nameof (unicode),
      nameof (bytes_warning)
    };
    private object[] _values = new object[15]
    {
      (object) 0,
      (object) 0,
      (object) 0,
      (object) 0,
      (object) 0,
      (object) 0,
      (object) 0,
      (object) 0,
      (object) 0,
      (object) 0,
      (object) 0,
      (object) 0,
      (object) 0,
      (object) 0,
      (object) 0
    };
    private PythonTuple __tuple;
    private string __string;
    private bool _modified = true;

    internal SysFlags()
    {
    }

    private PythonTuple _tuple
    {
      get
      {
        this._Refresh();
        return this.__tuple;
      }
    }

    private string _string
    {
      get
      {
        this._Refresh();
        return this.__string;
      }
    }

    public override string ToString() => this._string;

    public string __repr__() => this._string;

    private void _Refresh()
    {
      if (!this._modified)
        return;
      this.__tuple = PythonTuple.MakeTuple(this._values);
      StringBuilder stringBuilder = new StringBuilder("sys.flags(");
      for (int index = 0; index < 15; ++index)
      {
        if (SysModule.SysFlags._keys[index] == null)
          stringBuilder.Append(this._values[index]);
        else
          stringBuilder.AppendFormat("{0}={1}", (object) SysModule.SysFlags._keys[index], this._values[index]);
        if (index < 14)
          stringBuilder.Append(", ");
        else
          stringBuilder.Append(')');
      }
      this.__string = stringBuilder.ToString();
      this._modified = false;
    }

    private int _GetVal(int index) => (int) this._values[index];

    private void _SetVal(int index, int value)
    {
      if ((int) this._values[index] == value)
        return;
      this._modified = true;
      this._values[index] = (object) value;
    }

    void ICollection<object>.Add(object item)
    {
      throw new InvalidOperationException("sys.flags is readonly");
    }

    void ICollection<object>.Clear()
    {
      throw new InvalidOperationException("sys.flags is readonly");
    }

    [PythonHidden(new PlatformID[] {})]
    public bool Contains(object item) => this._tuple.Contains(item);

    [PythonHidden(new PlatformID[] {})]
    public void CopyTo(object[] array, int arrayIndex) => this._tuple.CopyTo(array, arrayIndex);

    public int Count
    {
      [PythonHidden(new PlatformID[] {})] get => 15;
    }

    bool ICollection<object>.IsReadOnly => true;

    bool ICollection<object>.Remove(object item)
    {
      throw new InvalidOperationException("sys.flags is readonly");
    }

    [PythonHidden(new PlatformID[] {})]
    public IEnumerator GetEnumerator() => this._tuple.GetEnumerator();

    IEnumerator<object> IEnumerable<object>.GetEnumerator()
    {
      return ((IEnumerable<object>) this._tuple).GetEnumerator();
    }

    public int __len__() => 15;

    public object this[int i] => this._tuple[i];

    public object this[BigInteger i] => this[(int) i];

    public object __getslice__(int start, int end) => this._tuple.__getslice__(start, end);

    public object this[IronPython.Runtime.Slice s] => this._tuple[s];

    public object this[object o] => this[Converter.ConvertToIndex(o)];

    [PythonHidden(new PlatformID[] {})]
    public int IndexOf(object item) => this._tuple.IndexOf(item);

    void IList<object>.Insert(int index, object item)
    {
      throw new InvalidOperationException("sys.flags is readonly");
    }

    void IList<object>.RemoveAt(int index)
    {
      throw new InvalidOperationException("sys.flags is readonly");
    }

    object IList<object>.this[int index]
    {
      get => this._tuple[index];
      set => throw new InvalidOperationException("sys.flags is readonly");
    }

    public static PythonTuple operator +([NotNull] SysModule.SysFlags f, [NotNull] PythonTuple t)
    {
      return f._tuple + t;
    }

    public static PythonTuple operator *([NotNull] SysModule.SysFlags f, int n) => f._tuple * n;

    public static PythonTuple operator *(int n, [NotNull] SysModule.SysFlags f) => f._tuple * n;

    public static object operator *([NotNull] SysModule.SysFlags f, [NotNull] IronPython.Runtime.Index n)
    {
      return f._tuple * n;
    }

    public static object operator *([NotNull] IronPython.Runtime.Index n, [NotNull] SysModule.SysFlags f)
    {
      return f._tuple * n;
    }

    public static object operator *([NotNull] SysModule.SysFlags f, object n) => f._tuple * n;

    public static object operator *(object n, [NotNull] SysModule.SysFlags f) => f._tuple * n;

    public static bool operator >(SysModule.SysFlags f, PythonTuple t) => f._tuple > t;

    public static bool operator <(SysModule.SysFlags f, PythonTuple t) => f._tuple < t;

    public static bool operator >=(SysModule.SysFlags f, PythonTuple t) => f._tuple >= t;

    public static bool operator <=(SysModule.SysFlags f, PythonTuple t) => f._tuple <= t;

    public override bool Equals(object obj)
    {
      return obj is SysModule.SysFlags ? this._tuple.Equals((object) ((SysModule.SysFlags) obj)._tuple) : this._tuple.Equals(obj);
    }

    public override int GetHashCode() => this._tuple.GetHashCode();

    public int debug
    {
      get => this._GetVal(0);
      internal set => this._SetVal(0, value);
    }

    public int py3k_warning
    {
      get => this._GetVal(1);
      internal set => this._SetVal(1, value);
    }

    public int division_warning
    {
      get => this._GetVal(2);
      internal set => this._SetVal(2, value);
    }

    public int division_new
    {
      get => this._GetVal(3);
      internal set => this._SetVal(3, value);
    }

    public int inspect
    {
      get => this._GetVal(4);
      internal set => this._SetVal(4, value);
    }

    public int interactive
    {
      get => this._GetVal(5);
      internal set => this._SetVal(5, value);
    }

    public int optimize
    {
      get => this._GetVal(6);
      internal set => this._SetVal(6, value);
    }

    public int dont_write_bytecode
    {
      get => this._GetVal(7);
      internal set => this._SetVal(7, value);
    }

    public int no_user_site
    {
      get => this._GetVal(8);
      internal set => this._SetVal(8, value);
    }

    public int no_site
    {
      get => this._GetVal(9);
      internal set => this._SetVal(9, value);
    }

    public int ignore_environment
    {
      get => this._GetVal(10);
      internal set => this._SetVal(10, value);
    }

    public int tabcheck
    {
      get => this._GetVal(11);
      internal set => this._SetVal(11, value);
    }

    public int verbose
    {
      get => this._GetVal(12);
      internal set => this._SetVal(12, value);
    }

    public int unicode
    {
      get => this._GetVal(13);
      internal set => this._SetVal(13, value);
    }

    public int bytes_warning
    {
      get => this._GetVal(14);
      internal set => this._SetVal(14, value);
    }
  }

  [PythonType("sys.long_info")]
  [PythonHidden(new PlatformID[] {})]
  public class longinfo : PythonTuple
  {
    public readonly int bits_per_digit;
    public readonly int sizeof_digit;
    public const int n_fields = 2;
    public const int n_sequence_fields = 2;
    public const int n_unnamed_fields = 0;

    internal longinfo(int bits_per_digit, int sizeof_digit)
      : base(new object[2]
      {
        (object) bits_per_digit,
        (object) sizeof_digit
      })
    {
      this.bits_per_digit = bits_per_digit;
      this.sizeof_digit = sizeof_digit;
    }

    public override string __repr__(CodeContext context)
    {
      return $"sys.long_info(bits_per_digit={this.bits_per_digit}, sizeof_digit={this.sizeof_digit})";
    }
  }

  [PythonType("sys.float_info")]
  [PythonHidden(new PlatformID[] {})]
  public class floatinfo : PythonTuple
  {
    public readonly double max;
    public readonly int max_exp;
    public readonly int max_10_exp;
    public readonly double min;
    public readonly int min_exp;
    public readonly int min_10_exp;
    public readonly int dig;
    public readonly int mant_dig;
    public readonly double epsilon;
    public readonly int radix;
    public readonly int rounds;
    public const int n_fields = 11;
    public const int n_sequence_fields = 11;
    public const int n_unnamed_fields = 0;

    internal floatinfo(
      double max,
      int max_exp,
      int max_10_exp,
      double min,
      int min_exp,
      int min_10_exp,
      int dig,
      int mant_dig,
      double epsilon,
      int radix,
      int rounds)
      : base(new object[11]
      {
        (object) max,
        (object) max_exp,
        (object) max_10_exp,
        (object) min,
        (object) min_exp,
        (object) min_10_exp,
        (object) dig,
        (object) mant_dig,
        (object) epsilon,
        (object) radix,
        (object) rounds
      })
    {
      this.max = max;
      this.max_exp = max_exp;
      this.max_10_exp = max_10_exp;
      this.min = min;
      this.min_exp = min_exp;
      this.min_10_exp = min_10_exp;
      this.dig = dig;
      this.mant_dig = mant_dig;
      this.epsilon = epsilon;
      this.radix = radix;
      this.rounds = rounds;
    }

    public override string __repr__(CodeContext context)
    {
      return $"sys.float_info(max={this.max}, max_exp={this.max_exp}, max_10_exp={this.max_10_exp}, min={this.min}, min_exp={this.min_exp}, min_10_exp={this.min_10_exp}, dig={this.dig}, mant_dig={this.mant_dig}, epsilon={this.epsilon}, radix={this.radix}, rounds={this.rounds})";
    }
  }
}
