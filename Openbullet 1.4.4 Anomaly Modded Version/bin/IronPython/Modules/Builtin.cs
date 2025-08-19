// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.Builtin
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using IronPython.Compiler.Ast;
using IronPython.Runtime;
using IronPython.Runtime.Binding;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Modules;

[Documentation("")]
public static class Builtin
{
  public const string __doc__ = "Provides access to commonly used built-in functions, exception objects, etc...";
  public const object __package__ = null;
  public const string __name__ = "__builtin__";
  public static readonly object None;
  [ThreadStatic]
  private static List<PythonModule> _reloadStack;
  private static BigInteger MaxDouble = new BigInteger(double.MaxValue);
  private static BigInteger MinDouble = new BigInteger(double.MinValue);

  public static object True => ScriptingRuntimeHelpers.True;

  public static object False => ScriptingRuntimeHelpers.False;

  public static IronPython.Runtime.Types.Ellipsis Ellipsis => IronPython.Runtime.Types.Ellipsis.Value;

  public static NotImplementedType NotImplemented => NotImplementedType.Value;

  [Documentation("__import__(name) -> module\n\nImport a module.")]
  [LightThrowing]
  public static object __import__(CodeContext context, string name)
  {
    return Builtin.__import__(context, name, (object) null, (object) null, (object) null, -1);
  }

  [Documentation("__import__(name, globals, locals, fromlist, level) -> module\n\nImport a module.")]
  [LightThrowing]
  public static object __import__(
    CodeContext context,
    string name,
    object globals = null,
    object locals = null,
    object fromlist = null,
    int level = -1)
  {
    switch (fromlist)
    {
      case string _:
      case Extensible<string> _:
        fromlist = (object) new List<object>() { fromlist };
        break;
    }
    IList list = fromlist as IList;
    PythonContext languageContext = context.LanguageContext;
    object obj = Importer.ImportModule(context, globals, name, list != null && list.Count > 0, level);
    if (obj == null)
      return LightExceptions.Throw(PythonOps.ImportError("No module named {0}", (object) name));
    if (obj is PythonModule from && list != null)
    {
      for (int index = 0; index < list.Count; ++index)
      {
        object str = list[index];
        string res;
        if (languageContext.TryConvertToString(str, out res) && !string.IsNullOrEmpty(res))
        {
          if (res != "*")
          {
            try
            {
              Importer.ImportFrom(context, (object) from, res);
            }
            catch (ImportException ex)
            {
            }
          }
        }
      }
    }
    return obj;
  }

  [Documentation("abs(number) -> number\n\nReturn the absolute value of the argument.")]
  public static object abs(CodeContext context, object o)
  {
    switch (o)
    {
      case int x1:
        return Int32Ops.Abs(x1);
      case long x2:
        return Int64Ops.Abs(x2);
      case double x3:
        return (object) DoubleOps.Abs(x3);
      case bool flag:
        return (object) (flag ? 1 : 0);
      case BigInteger x4:
        return BigIntegerOps.__abs__(x4);
      case Complex x5:
        return (object) ComplexOps.Abs(x5);
      default:
        object obj;
        if (PythonTypeOps.TryInvokeUnaryOperator(context, o, "__abs__", out obj))
          return obj;
        throw PythonOps.TypeError("bad operand type for abs(): '{0}'", (object) PythonTypeOps.GetName(o));
    }
  }

  public static bool all(object x)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(x);
    while (enumerator.MoveNext())
    {
      if (!PythonOps.IsTrue(enumerator.Current))
        return false;
    }
    return true;
  }

  public static bool any(object x)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(x);
    while (enumerator.MoveNext())
    {
      if (PythonOps.IsTrue(enumerator.Current))
        return true;
    }
    return false;
  }

  [Documentation("apply(object[, args[, kwargs]]) -> value\n\nDeprecated.\nInstead, use:\n    function(*args, **keywords).")]
  [Python3Warning("apply() not supported in 3.x; use func(*args, **kwargs)")]
  public static object apply(CodeContext context, object func)
  {
    return PythonOps.CallWithContext(context, func);
  }

  [Python3Warning("apply() not supported in 3.x; use func(*args, **kwargs)")]
  public static object apply(CodeContext context, object func, object args)
  {
    return PythonOps.CallWithArgsTupleAndContext(context, func, ArrayUtils.EmptyObjects, args);
  }

  [Python3Warning("apply() not supported in 3.x; use func(*args, **kwargs)")]
  public static object apply(CodeContext context, object func, object args, object kws)
  {
    return context.LanguageContext.CallWithKeywords(func, args, kws);
  }

  public static PythonType basestring
  {
    get => DynamicHelpers.GetPythonTypeFromType(typeof (Builtin._basestring));
  }

  public static string bin(object obj)
  {
    switch (obj)
    {
      case int self2:
        return Int32Ops.ToBinary(self2);
      case IronPython.Runtime.Index _:
        return Int32Ops.ToBinary(Converter.ConvertToIndex((object) (IronPython.Runtime.Index) obj));
      case BigInteger val2:
        return BigIntegerOps.ToBinary(val2);
      default:
        object obj1 = PythonOps.Index(obj);
        switch (obj1)
        {
          case int self1:
            return Int32Ops.ToBinary(self1);
          case BigInteger val1:
            return BigIntegerOps.ToBinary(val1);
          default:
            throw PythonOps.TypeError("__index__ returned non-(int, long) (type {0})", (object) PythonOps.GetPythonTypeName(obj1));
        }
    }
  }

  public static PythonType @bool => DynamicHelpers.GetPythonTypeFromType(typeof (bool));

  public static PythonType buffer => DynamicHelpers.GetPythonTypeFromType(typeof (PythonBuffer));

  public static PythonType bytes => DynamicHelpers.GetPythonTypeFromType(typeof (Bytes));

  public static PythonType bytearray => DynamicHelpers.GetPythonTypeFromType(typeof (ByteArray));

  [Documentation("callable(object) -> bool\n\nReturn whether the object is callable (i.e., some kind of function).")]
  public static bool callable(CodeContext context, object o) => PythonOps.IsCallable(context, o);

  [Documentation("chr(i) -> character\n\nReturn a string of one character with ordinal i; 0 <= i< 256.")]
  [LightThrowing]
  public static object chr(int value)
  {
    if (value >= 0 && value <= (int) byte.MaxValue)
      return (object) ScriptingRuntimeHelpers.CharToString((char) value);
    return LightExceptions.Throw(PythonOps.ValueError("{0} is not in required range", (object) value));
  }

  internal static object TryCoerce(CodeContext context, object x, object y)
  {
    PythonType pythonType = DynamicHelpers.GetPythonType(x);
    PythonTypeSlot slot;
    object func;
    return pythonType.TryResolveSlot(context, "__coerce__", out slot) && slot.TryGetValue(context, x, pythonType, out func) ? PythonCalls.Call(context, func, y) : (object) NotImplementedType.Value;
  }

  [Documentation("coerce(x, y) -> (x1, y1)\n\nReturn a tuple consisting of the two numeric arguments converted to\na common type. If coercion is not possible, raise TypeError.")]
  [Python3Warning("coerce() not supported in 3.x")]
  public static object coerce(CodeContext context, object x, object y)
  {
    if (x == null && y == null)
      return (object) PythonTuple.MakeTuple(new object[2]);
    object obj1 = Builtin.TryCoerce(context, x, y);
    if (obj1 != null && obj1 != NotImplementedType.Value)
      return obj1;
    object obj2 = Builtin.TryCoerce(context, y, x);
    if (obj2 == null || obj2 == NotImplementedType.Value || !(obj2 is PythonTuple pythonTuple) || pythonTuple.Count != 2)
      throw PythonOps.TypeError("coercion failed");
    return (object) PythonTuple.MakeTuple(pythonTuple[1], pythonTuple[0]);
  }

  [Documentation("compile a unit of source code.\n\nThe source can be compiled either as exec, eval, or single.\nexec compiles the code as if it were a file\neval compiles the code as if were an expression\nsingle compiles a single statement\n\nsource can either be a string or an AST object")]
  public static object compile(
    CodeContext context,
    object source,
    string filename,
    string mode,
    object flags = null,
    object dont_inherit = null)
  {
    bool flag = false;
    int int32 = flags != null ? Converter.ConvertToInt32(flags) : 0;
    if ((int32 & 1024 /*0x0400*/) != 0)
    {
      flag = true;
      int32 &= -1025;
    }
    if (mode != "exec" && mode != "eval" && mode != "single")
      throw PythonOps.ValueError("compile() arg 3 must be 'exec' or 'eval' or 'single'");
    string source1;
    switch (source)
    {
      case _ast.AST _:
        if (flag)
          return source;
        PythonAst pythonAst = _ast.ConvertToPythonAst(context, (_ast.AST) source, filename);
        pythonAst.Bind();
        return (object) ((RunnableScriptCode) pythonAst.ToScriptCode()).GetFunctionCode(true);
      case string _:
        source1 = (string) source;
        break;
      case PythonBuffer _:
        source1 = ((PythonBuffer) source).ToString();
        break;
      case ByteArray _:
        source1 = ((ByteArray) source).ToString();
        break;
      case Bytes _:
        source1 = ((Bytes) source).ToString();
        break;
      default:
        throw PythonOps.TypeError("source can be either AST or string, actual argument: {0}", (object) source.GetType());
    }
    string code = source1.IndexOf(char.MinValue) == -1 ? Builtin.RemoveBom(source1) : throw PythonOps.TypeError("compile() expected string without null bytes");
    bool compilerInheritance = Builtin.GetCompilerInheritance(dont_inherit);
    CompileFlags compilerFlags = Builtin.GetCompilerFlags(int32);
    PythonCompilerOptions codeCompilerOptions = Builtin.GetRuntimeGeneratedCodeCompilerOptions(context, compilerInheritance, compilerFlags);
    if ((compilerFlags & CompileFlags.CO_DONT_IMPLY_DEDENT) != (CompileFlags) 0)
      codeCompilerOptions.DontImplyDedent = true;
    SourceUnit sourceUnit = (SourceUnit) null;
    switch (mode)
    {
      case "exec":
        sourceUnit = context.LanguageContext.CreateSnippet(code, filename, SourceCodeKind.Statements);
        break;
      case "eval":
        sourceUnit = context.LanguageContext.CreateSnippet(code, filename, SourceCodeKind.Expression);
        break;
      case "single":
        sourceUnit = context.LanguageContext.CreateSnippet(code, filename, SourceCodeKind.InteractiveCode);
        break;
    }
    return flag ? (object) _ast.BuildAst(context, sourceUnit, codeCompilerOptions, mode) : (object) FunctionCode.FromSourceUnit(sourceUnit, codeCompilerOptions, true);
  }

  private static string RemoveBom(string source)
  {
    if (source.StartsWith("ï»¿", StringComparison.Ordinal))
      source = source.Substring(3, source.Length - 3);
    return source;
  }

  public static PythonType classmethod
  {
    get => DynamicHelpers.GetPythonTypeFromType(typeof (IronPython.Runtime.classmethod));
  }

  public static int cmp(CodeContext context, object x, object y)
  {
    return PythonOps.Compare(context, x, y);
  }

  public static int cmp(CodeContext context, int x, int y) => Int32Ops.Compare(x, y);

  public static int cmp(CodeContext context, [NotNull] BigInteger x, [NotNull] BigInteger y)
  {
    return (ValueType) x == (ValueType) y ? 0 : BigIntegerOps.Compare(x, y);
  }

  public static int cmp(CodeContext context, double x, [NotNull] BigInteger y)
  {
    return -BigIntegerOps.Compare(y, x);
  }

  public static int cmp(CodeContext context, [NotNull] BigInteger x, double y)
  {
    return BigIntegerOps.Compare(x, y);
  }

  public static int cmp(CodeContext context, [NotNull] string x, [NotNull] string y)
  {
    if ((object) x != (object) y)
    {
      int num = string.CompareOrdinal(x, y);
      if (num >= 1)
        return 1;
      if (num <= -1)
        return -1;
    }
    return 0;
  }

  public static int cmp(CodeContext context, [NotNull] SortKey x, [NotNull] SortKey y)
  {
    return SortKey.Compare(x, y);
  }

  public static int cmp(CodeContext context, [NotNull] PythonTuple x, [NotNull] PythonTuple y)
  {
    return x == y ? 0 : x.CompareTo(y);
  }

  public static PythonType complex => DynamicHelpers.GetPythonTypeFromType(typeof (Complex));

  public static void delattr(CodeContext context, object o, string name)
  {
    PythonOps.DeleteAttr(context, o, name);
  }

  public static PythonType dict => DynamicHelpers.GetPythonTypeFromType(typeof (PythonDictionary));

  public static IronPython.Runtime.List dir(CodeContext context)
  {
    IronPython.Runtime.List list = PythonOps.MakeListFromSequence((object) context.Dict.Keys);
    list.sort(context);
    return list;
  }

  public static IronPython.Runtime.List dir(CodeContext context, object o)
  {
    IronPython.Runtime.List list = new IronPython.Runtime.List((object) PythonOps.GetAttrNames(context, o));
    list.sort(context);
    return list;
  }

  public static object divmod(CodeContext context, object x, object y)
  {
    return context.LanguageContext.DivMod(x, y);
  }

  public static PythonType enumerate => DynamicHelpers.GetPythonTypeFromType(typeof (Enumerate));

  public static object eval(CodeContext context, FunctionCode code)
  {
    return Builtin.eval(context, code, (PythonDictionary) null, (object) null);
  }

  public static object eval(CodeContext context, FunctionCode code, PythonDictionary globals)
  {
    return Builtin.eval(context, code, globals, (object) null);
  }

  public static object eval(
    CodeContext context,
    FunctionCode code,
    PythonDictionary globals,
    object locals)
  {
    if (code == null)
      throw PythonOps.TypeError("eval() argument 1 must be string or code object");
    return code.Call(Builtin.GetExecEvalScopeOptional(context, globals, locals, false));
  }

  internal static PythonDictionary GetAttrLocals(CodeContext context, object locals)
  {
    PythonDictionary attrLocals = (PythonDictionary) null;
    if (locals == null)
    {
      if (context.IsTopLevel)
        attrLocals = context.Dict;
    }
    else
    {
      if (!(locals is PythonDictionary pythonDictionary))
        pythonDictionary = new PythonDictionary((DictionaryStorage) new ObjectAttributesAdapter(context, locals));
      attrLocals = pythonDictionary;
    }
    return attrLocals;
  }

  [LightThrowing]
  public static object eval(CodeContext context, string expression)
  {
    return Builtin.eval(context, expression, (PythonDictionary) null, (object) null);
  }

  [LightThrowing]
  public static object eval(CodeContext context, string expression, PythonDictionary globals)
  {
    return Builtin.eval(context, expression, globals, (object) null);
  }

  [LightThrowing]
  public static object eval(
    CodeContext context,
    string expression,
    PythonDictionary globals,
    object locals)
  {
    if (expression == null)
      throw PythonOps.TypeError("eval() argument 1 must be string or code object");
    if (locals != null && PythonOps.IsMappingType(context, locals) == ScriptingRuntimeHelpers.False)
      throw PythonOps.TypeError("locals must be mapping");
    expression = Builtin.RemoveBom(expression);
    CodeContext evalScopeOptional = Builtin.GetExecEvalScopeOptional(context, globals, locals, false);
    SourceUnit snippet = context.LanguageContext.CreateSnippet(expression.TrimStart(' ', '\t'), SourceCodeKind.Expression);
    PythonCompilerOptions codeCompilerOptions = Builtin.GetRuntimeGeneratedCodeCompilerOptions(context, true, (CompileFlags) 0);
    codeCompilerOptions.Module |= ModuleOptions.LightThrow;
    codeCompilerOptions.Module &= ~ModuleOptions.ModuleBuiltins;
    PythonCompilerOptions options = codeCompilerOptions;
    return FunctionCode.FromSourceUnit(snippet, options, false).Call(evalScopeOptional);
  }

  [Python3Warning("execfile() not supported in 3.x; use exec()")]
  public static void execfile(CodeContext context, object filename)
  {
    Builtin.execfile(context, filename, (object) null, (object) null);
  }

  [Python3Warning("execfile() not supported in 3.x; use exec()")]
  public static void execfile(CodeContext context, object filename, object globals)
  {
    Builtin.execfile(context, filename, globals, (object) null);
  }

  [Python3Warning("execfile() not supported in 3.x; use exec()")]
  public static void execfile(CodeContext context, object filename, object globals, object locals)
  {
    if (filename == null)
      throw PythonOps.TypeError("execfile() argument 1 must be string, not None");
    switch (globals)
    {
      case PythonDictionary globals1:
      case null:
        switch (locals)
        {
          case PythonDictionary localsDict:
          case null:
            if (localsDict == null)
              localsDict = globals1;
            CodeContext evalScopeOptional = Builtin.GetExecEvalScopeOptional(context, globals1, (object) localsDict, true);
            string path = Converter.ConvertToString(filename);
            PythonContext languageContext = context.LanguageContext;
            if (!languageContext.DomainManager.Platform.FileExists(path))
              throw PythonOps.IOError("execfile: specified file doesn't exist");
            SourceUnit fileUnit = languageContext.CreateFileUnit(path, languageContext.DefaultEncoding, SourceCodeKind.Statements);
            PythonCompilerOptions codeCompilerOptions = Builtin.GetRuntimeGeneratedCodeCompilerOptions(context, true, (CompileFlags) 0);
            codeCompilerOptions.Module &= ~ModuleOptions.Optimized;
            FunctionCode functionCode;
            try
            {
              functionCode = FunctionCode.FromSourceUnit(fileUnit, codeCompilerOptions, false);
            }
            catch (UnauthorizedAccessException ex)
            {
              throw PythonOps.IOError((System.Exception) ex);
            }
            functionCode.Call(evalScopeOptional);
            return;
          default:
            throw PythonOps.TypeError("execfile() arg 3 must be dictionary");
        }
      default:
        throw PythonOps.TypeError("execfile() arg 2 must be dictionary");
    }
  }

  public static PythonType file => DynamicHelpers.GetPythonTypeFromType(typeof (PythonFile));

  public static string filter(CodeContext context, object function, [NotNull] string list)
  {
    if (function == null)
      return list;
    if (list == null)
      throw PythonOps.TypeError("NoneType is not iterable");
    StringBuilder stringBuilder = new StringBuilder();
    foreach (char ch in list)
    {
      if (PythonOps.IsTrue(PythonCalls.Call(context, function, (object) ScriptingRuntimeHelpers.CharToString(ch))))
        stringBuilder.Append(ch);
    }
    return stringBuilder.ToString();
  }

  public static string filter(CodeContext context, object function, [NotNull] ExtensibleString list)
  {
    StringBuilder stringBuilder = new StringBuilder();
    IEnumerator enumerator = PythonOps.GetEnumerator((object) list);
    while (enumerator.MoveNext())
    {
      object current = enumerator.Current;
      if (PythonOps.IsTrue(function != null ? PythonCalls.Call(context, function, current) : current))
        stringBuilder.Append(Converter.ConvertToString(current));
    }
    return stringBuilder.ToString();
  }

  public static PythonTuple filter(CodeContext context, object function, [NotNull] PythonTuple tuple)
  {
    List<object> objectList = new List<object>(tuple.__len__());
    for (int index = 0; index < tuple.__len__(); ++index)
    {
      object obj = tuple[index];
      if (PythonOps.IsTrue(function != null ? PythonCalls.Call(context, function, obj) : obj))
        objectList.Add(obj);
    }
    return PythonTuple.MakeTuple(objectList.ToArray());
  }

  public static IronPython.Runtime.List filter(CodeContext context, object function, object list)
  {
    if (list == null)
      throw PythonOps.TypeError("NoneType is not iterable");
    IronPython.Runtime.List list1 = new IronPython.Runtime.List();
    IEnumerator enumerator = PythonOps.GetEnumerator(list);
    while (enumerator.MoveNext())
    {
      if (function == null)
      {
        if (PythonOps.IsTrue(enumerator.Current))
          list1.AddNoLock(enumerator.Current);
      }
      else if (PythonOps.IsTrue(PythonCalls.Call(context, function, enumerator.Current)))
        list1.AddNoLock(enumerator.Current);
    }
    return list1;
  }

  public static PythonType @float => DynamicHelpers.GetPythonTypeFromType(typeof (double));

  public static string format(CodeContext context, object argValue, string formatSpec = "")
  {
    object func;
    object o;
    if (argValue is OldInstance oldInstance && oldInstance.TryGetBoundCustomMember(context, "__format__", out func))
      o = PythonOps.CallWithContext(context, func, (object) formatSpec);
    else
      PythonTypeOps.TryInvokeBinaryOperator(context, argValue, (object) formatSpec, "__format__", out o);
    return o is string str ? str : throw PythonOps.TypeError("{0}.__format__ must return string or unicode, not {1}", (object) PythonTypeOps.GetName(argValue), (object) PythonTypeOps.GetName(o));
  }

  public static object getattr(CodeContext context, object o, string name)
  {
    return PythonOps.GetBoundAttr(context, o, name);
  }

  public static object getattr(CodeContext context, object o, string name, object def)
  {
    object ret;
    return PythonOps.TryGetBoundAttr(context, o, name, out ret) ? ret : def;
  }

  public static PythonDictionary globals(CodeContext context) => context.ModuleContext.Globals;

  public static bool hasattr(CodeContext context, object o, string name)
  {
    return PythonOps.HasAttr(context, o, name);
  }

  public static int hash(CodeContext context, object o) => PythonContext.Hash(o);

  public static int hash(CodeContext context, [NotNull] PythonTuple o)
  {
    return ((IStructuralEquatable) o).GetHashCode(context.LanguageContext.EqualityComparerNonGeneric);
  }

  public static int hash(CodeContext context, char o) => PythonContext.Hash((object) o);

  public static int hash(CodeContext context, int o) => o;

  public static int hash(CodeContext context, [NotNull] string o) => o.GetHashCode();

  public static int hash(CodeContext context, [NotNull] ExtensibleString o)
  {
    return Builtin.hash(context, (object) o);
  }

  public static int hash(CodeContext context, [NotNull] BigInteger o) => BigIntegerOps.__hash__(o);

  public static int hash(CodeContext context, [NotNull] Extensible<BigInteger> o)
  {
    return Builtin.hash(context, (object) o);
  }

  public static int hash(CodeContext context, double o) => DoubleOps.__hash__(o);

  public static void help(CodeContext context, object o)
  {
    StringBuilder doc = new StringBuilder();
    List<object> doced = new List<object>();
    Builtin.help(context, doced, doc, 0, o);
    if (doc.Length == 0)
    {
      if (!(o is string))
      {
        Builtin.help(context, (object) DynamicHelpers.GetPythonType(o));
        return;
      }
      doc.Append("no documentation found for ");
      doc.Append(PythonOps.Repr(context, o));
    }
    string str = doc.ToString();
    char[] chArray = new char[1]{ '\n' };
    foreach (object o1 in str.Split(chArray))
      PythonOps.Print(context, o1);
  }

  private static void help(
    CodeContext context,
    List<object> doced,
    StringBuilder doc,
    int indent,
    object obj)
  {
    if (doced.Contains(obj))
      return;
    doced.Add(obj);
    switch (obj)
    {
      case string name:
        if (indent != 0)
          break;
        using (IEnumerator<object> enumerator = context.LanguageContext.SystemStateModules.Values.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            object current = enumerator.Current;
            IList<object> attrNames = PythonOps.GetAttrNames(context, current);
            IronPython.Runtime.List list = new IronPython.Runtime.List();
            foreach (string str in (IEnumerable<object>) attrNames)
            {
              object ret;
              if (str == name && PythonOps.TryGetBoundAttr(context, current, name, out ret))
                list.append(ret);
            }
            pythonType = (PythonType) null;
            builtinFunction = (BuiltinFunction) null;
            PythonFunction pythonFunction = (PythonFunction) null;
            for (int index = 0; index < list.__len__() && !(list[index] is PythonType pythonType); ++index)
            {
              if ((builtinFunction != null || !(list[index] is BuiltinFunction builtinFunction)) && pythonFunction == null)
                pythonFunction = list[index] as PythonFunction;
            }
            if (pythonType != null)
              Builtin.help(context, doced, doc, indent, (object) pythonType);
            else if (builtinFunction != null)
              Builtin.help(context, doced, doc, indent, (object) builtinFunction);
            else if (pythonFunction != null)
              Builtin.help(context, doced, doc, indent, (object) pythonFunction);
          }
          break;
        }
      case PythonType pythonType1:
        if (indent == 0)
          doc.AppendFormat("Help on {0} in module {1}\n\n", (object) pythonType1.Name, PythonOps.GetBoundAttr(context, (object) pythonType1, "__module__"));
        PythonTypeSlot slot1;
        if (pythonType1.TryResolveSlot(context, "__doc__", out slot1))
        {
          object obj1;
          if (slot1.TryGetValue(context, (object) null, pythonType1, out obj1) && obj1 != null)
            Builtin.AppendMultiLine(doc, obj1.ToString() + Environment.NewLine, indent);
          Builtin.AppendIndent(doc, indent);
          doc.AppendLine("Data and other attributes defined here:");
          Builtin.AppendIndent(doc, indent);
          doc.AppendLine();
        }
        IronPython.Runtime.List memberNames = pythonType1.GetMemberNames(context);
        memberNames.sort(context);
        IEnumerator enumerator1 = memberNames.GetEnumerator();
        try
        {
          while (enumerator1.MoveNext())
          {
            string current = (string) enumerator1.Current;
            PythonTypeSlot slot2;
            object obj2;
            if (!(current == "__class__") && pythonType1.TryLookupSlot(context, current, out slot2) && slot2.TryGetValue(context, (object) null, pythonType1, out obj2))
              Builtin.help(context, doced, doc, indent + 1, obj2);
          }
          break;
        }
        finally
        {
          if (enumerator1 is IDisposable disposable)
            disposable.Dispose();
        }
      case BuiltinMethodDescriptor methodDescriptor:
        if (indent == 0)
          doc.AppendFormat("Help on method-descriptor {0}\n\n", (object) methodDescriptor.__name__);
        Builtin.AppendIndent(doc, indent);
        doc.Append(methodDescriptor.__name__);
        doc.Append("(...)\n");
        Builtin.AppendMultiLine(doc, methodDescriptor.__doc__, indent + 1);
        break;
      case BuiltinFunction builtinFunction1:
        if (indent == 0)
          doc.AppendFormat("Help on built-in function {0}\n\n", (object) builtinFunction1.Name);
        Builtin.AppendIndent(doc, indent);
        doc.Append(builtinFunction1.Name);
        doc.Append("(...)\n");
        Builtin.AppendMultiLine(doc, builtinFunction1.__doc__, indent + 1);
        break;
      case PythonFunction pythonFunction1:
        if (indent == 0)
          doc.AppendFormat("Help on function {0} in module {1}:\n\n", (object) pythonFunction1.__name__, pythonFunction1.__module__);
        Builtin.AppendIndent(doc, indent);
        doc.Append(pythonFunction1.GetSignatureString());
        string multiline1 = Converter.ConvertToString(pythonFunction1.__doc__);
        if (string.IsNullOrEmpty(multiline1))
          break;
        Builtin.AppendMultiLine(doc, multiline1, indent);
        break;
      case Method method when method.im_func is PythonFunction imFunc:
        if (indent == 0)
          doc.AppendFormat("Help on method {0} in module {1}:\n\n", (object) imFunc.__name__, imFunc.__module__);
        Builtin.AppendIndent(doc, indent);
        doc.Append(imFunc.GetSignatureString());
        if (method.im_self == null)
          doc.AppendFormat(" unbound {0} method\n", (object) PythonOps.ToString(method.im_class));
        else
          doc.AppendFormat(" method of {0} instance\n", (object) PythonOps.ToString(method.im_class));
        string multiline2 = Converter.ConvertToString(imFunc.__doc__);
        if (string.IsNullOrEmpty(multiline2))
          break;
        Builtin.AppendMultiLine(doc, multiline2, indent);
        break;
      case PythonModule pythonModule:
        using (IEnumerator<object> enumerator2 = pythonModule.__dict__.Keys.GetEnumerator())
        {
          while (enumerator2.MoveNext())
          {
            string current = (string) enumerator2.Current;
            object obj3;
            if (!(current == "__class__") && !(current == "__builtins__") && pythonModule.__dict__.TryGetValue((object) current, out obj3))
              Builtin.help(context, doced, doc, indent + 1, obj3);
          }
          break;
        }
      case OldClass o:
        if (indent == 0)
          doc.AppendFormat("Help on {0} in module {1}\n\n", (object) o.Name, PythonOps.GetBoundAttr(context, (object) o, "__module__"));
        object ret1;
        if (o.TryLookupSlot("__doc__", out ret1) && ret1 != null)
        {
          Builtin.AppendMultiLine(doc, ret1.ToString() + Environment.NewLine, indent);
          Builtin.AppendIndent(doc, indent);
          doc.AppendLine("Data and other attributes defined here:");
          Builtin.AppendIndent(doc, indent);
          doc.AppendLine();
        }
        IronPython.Runtime.List list1 = new IronPython.Runtime.List((object) ((IPythonMembersList) o).GetMemberNames(context));
        list1.sort(context);
        using (IEnumerator<object> enumerator3 = ((IEnumerable<object>) list1).GetEnumerator())
        {
          while (enumerator3.MoveNext())
          {
            string current = (string) enumerator3.Current;
            object ret2;
            if (!(current == "__class__") && o.TryLookupSlot(current, out ret2))
              Builtin.help(context, doced, doc, indent + 1, ret2);
          }
          break;
        }
    }
  }

  private static void AppendMultiLine(StringBuilder doc, string multiline, int indent)
  {
    string str1 = multiline;
    char[] chArray = new char[1]{ '\n' };
    foreach (string str2 in str1.Split(chArray))
    {
      Builtin.AppendIndent(doc, indent + 1);
      doc.Append(str2);
      doc.Append('\n');
    }
  }

  private static void AppendIndent(StringBuilder doc, int indent)
  {
    doc.Append(" |  ");
    for (int index = 0; index < indent; ++index)
      doc.Append("    ");
  }

  public static object hex(object o) => PythonOps.Hex(o);

  public static object id(object o)
  {
    long num = PythonOps.Id(o);
    return PythonOps.Id(o) <= (long) int.MaxValue ? (object) (int) num : (object) (BigInteger) num;
  }

  [LightThrowing]
  public static object input(CodeContext context) => Builtin.input(context, (object) null);

  [LightThrowing]
  public static object input(CodeContext context, object prompt)
  {
    return Builtin.eval(context, Builtin.raw_input(context, prompt));
  }

  public static PythonType @int => DynamicHelpers.GetPythonTypeFromType(typeof (int));

  public static string intern(object o)
  {
    return o is string str ? string.Intern(str) : throw PythonOps.TypeError("intern: argument must be string");
  }

  public static bool isinstance(object o, [NotNull] PythonType typeinfo)
  {
    return PythonOps.IsInstance(o, typeinfo);
  }

  public static bool isinstance(CodeContext context, object o, [NotNull] PythonTuple typeinfo)
  {
    return PythonOps.IsInstance(context, o, typeinfo);
  }

  public static bool isinstance(CodeContext context, object o, object typeinfo)
  {
    return PythonOps.IsInstance(context, o, typeinfo);
  }

  public static bool issubclass(CodeContext context, [NotNull] OldClass c, object typeinfo)
  {
    return PythonOps.IsSubClass(context, c.TypeObject, typeinfo);
  }

  public static bool issubclass(CodeContext context, [NotNull] PythonType c, object typeinfo)
  {
    return PythonOps.IsSubClass(context, c, typeinfo);
  }

  public static bool issubclass(CodeContext context, [NotNull] PythonType c, [NotNull] PythonType typeinfo)
  {
    return PythonOps.IsSubClass(c, typeinfo);
  }

  [LightThrowing]
  public static object issubclass(CodeContext context, object o, object typeinfo)
  {
    if (typeinfo is PythonTuple pythonTuple1)
    {
      foreach (object typeinfo1 in pythonTuple1)
      {
        try
        {
          PythonOps.FunctionPushFrame(context.LanguageContext);
          object obj = Builtin.issubclass(context, o, typeinfo1);
          if (obj == ScriptingRuntimeHelpers.True)
            return ScriptingRuntimeHelpers.True;
          if (LightExceptions.IsLightException(obj))
            return obj;
        }
        finally
        {
          PythonOps.FunctionPopFrame();
        }
      }
      return ScriptingRuntimeHelpers.False;
    }
    object ret;
    if (!PythonOps.TryGetBoundAttr(o, "__bases__", out ret) || !(ret is PythonTuple pythonTuple2))
      return LightExceptions.Throw(PythonOps.TypeError("issubclass() arg 1 must be a class"));
    if (o == typeinfo)
      return ScriptingRuntimeHelpers.True;
    foreach (object o1 in pythonTuple2)
    {
      if (o1 == typeinfo)
        return ScriptingRuntimeHelpers.True;
      if (o1 is PythonType c2)
      {
        if (Builtin.issubclass(context, c2, typeinfo))
          return ScriptingRuntimeHelpers.True;
      }
      else if (o1 is OldClass c1)
      {
        if (Builtin.issubclass(context, c1, typeinfo))
          return ScriptingRuntimeHelpers.True;
      }
      else if (Builtin.hasattr(context, o1, "__bases__"))
      {
        object obj = Builtin.issubclass(context, o1, typeinfo);
        if (obj == ScriptingRuntimeHelpers.True)
          return ScriptingRuntimeHelpers.True;
        if (LightExceptions.IsLightException(obj))
          return obj;
      }
    }
    return ScriptingRuntimeHelpers.False;
  }

  public static object iter(CodeContext context, object o)
  {
    return PythonOps.GetEnumeratorObject(context, o);
  }

  public static object iter(CodeContext context, object func, object sentinel)
  {
    if (!PythonOps.IsCallable(context, func))
      throw PythonOps.TypeError("iter(v, w): v must be callable");
    return (object) new SentinelIterator(context, func, sentinel);
  }

  public static int len([NotNull] string str) => str.Length;

  public static int len([NotNull] ExtensibleString str) => str.__len__();

  public static int len([NotNull] IronPython.Runtime.List list) => list.__len__();

  public static int len([NotNull] PythonTuple tuple) => tuple.__len__();

  public static int len([NotNull] PythonDictionary dict) => dict.__len__();

  public static int len([NotNull] ICollection collection) => collection.Count;

  public static int len(object o) => PythonOps.Length(o);

  public static PythonType set => DynamicHelpers.GetPythonTypeFromType(typeof (SetCollection));

  public static PythonType frozenset
  {
    get => DynamicHelpers.GetPythonTypeFromType(typeof (FrozenSetCollection));
  }

  public static PythonType list => DynamicHelpers.GetPythonTypeFromType(typeof (IronPython.Runtime.List));

  public static object locals(CodeContext context)
  {
    return context.Dict._storage is ObjectAttributesAdapter storage ? storage.Backing : (object) context.Dict;
  }

  public static PythonType @long => TypeCache.BigInteger;

  public static PythonType memoryview => DynamicHelpers.GetPythonTypeFromType(typeof (MemoryView));

  private static CallSite<Func<CallSite, CodeContext, T, T1, object>> MakeMapSite<T, T1>(
    CodeContext context)
  {
    return CallSite<Func<CallSite, CodeContext, T, T1, object>>.Create((CallSiteBinder) context.LanguageContext.InvokeOne);
  }

  public static IronPython.Runtime.List map(
    CodeContext context,
    object func,
    [NotNull] IEnumerable enumerator)
  {
    IEnumerator enumerator1 = PythonOps.GetEnumerator((object) enumerator);
    IronPython.Runtime.List list = new IronPython.Runtime.List();
    CallSite<Func<CallSite, CodeContext, object, object, object>> callSite = (CallSite<Func<CallSite, CodeContext, object, object, object>>) null;
    if (func != null)
      callSite = Builtin.MakeMapSite<object, object>(context);
    else
      PythonOps.Warn3k(context, "map(None, ...) not supported in 3.x; use list(...)");
    while (enumerator1.MoveNext())
    {
      if (func == null)
        list.AddNoLock(enumerator1.Current);
      else
        list.AddNoLock(callSite.Target((CallSite) callSite, context, func, enumerator1.Current));
    }
    return list;
  }

  public static IronPython.Runtime.List map(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object, object>>> storage,
    object func,
    [NotNull] string enumerator)
  {
    if (storage.Data == null && func != null)
      storage.Data = Builtin.MakeMapSite<object, object>(context);
    CallSite<Func<CallSite, CodeContext, object, object, object>> data = storage.Data;
    IronPython.Runtime.List list = new IronPython.Runtime.List(enumerator.Length);
    foreach (char ch in enumerator)
    {
      if (func == null)
        list.AddNoLock((object) ScriptingRuntimeHelpers.CharToString(ch));
      else
        list.AddNoLock(data.Target((CallSite) data, context, func, (object) ScriptingRuntimeHelpers.CharToString(ch)));
    }
    return list;
  }

  public static IronPython.Runtime.List map(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, PythonType, object, object>>> storage,
    [NotNull] PythonType func,
    [NotNull] string enumerator)
  {
    CallSite<Func<CallSite, CodeContext, PythonType, string, object>> callSite = Builtin.MakeMapSite<PythonType, string>(context);
    IronPython.Runtime.List list = new IronPython.Runtime.List(enumerator.Length);
    foreach (char ch in enumerator)
      list.AddNoLock(callSite.Target((CallSite) callSite, context, func, ScriptingRuntimeHelpers.CharToString(ch)));
    return list;
  }

  public static IronPython.Runtime.List map(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, PythonType, object, object>>> storage,
    [NotNull] PythonType func,
    [NotNull] IEnumerable enumerator)
  {
    if (storage.Data == null)
      storage.Data = Builtin.MakeMapSite<PythonType, object>(context);
    CallSite<Func<CallSite, CodeContext, PythonType, object, object>> data = storage.Data;
    IEnumerator enumerator1 = PythonOps.GetEnumerator((object) enumerator);
    IronPython.Runtime.List list = new IronPython.Runtime.List();
    while (enumerator1.MoveNext())
      list.AddNoLock(data.Target((CallSite) data, context, func, enumerator1.Current));
    return list;
  }

  public static IronPython.Runtime.List map(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object>>> storage,
    [NotNull] BuiltinFunction func,
    [NotNull] string enumerator)
  {
    if (storage.Data == null)
      storage.Data = Builtin.MakeMapSite<BuiltinFunction, object>(context);
    CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object>> data = storage.Data;
    IronPython.Runtime.List list = new IronPython.Runtime.List(enumerator.Length);
    foreach (char ch in enumerator)
      list.AddNoLock(data.Target((CallSite) data, context, func, (object) ScriptingRuntimeHelpers.CharToString(ch)));
    return list;
  }

  public static IronPython.Runtime.List map(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object>>> storage,
    [NotNull] BuiltinFunction func,
    [NotNull] IEnumerable enumerator)
  {
    if (storage.Data == null)
      storage.Data = Builtin.MakeMapSite<BuiltinFunction, object>(context);
    CallSite<Func<CallSite, CodeContext, BuiltinFunction, object, object>> data = storage.Data;
    IEnumerator enumerator1 = PythonOps.GetEnumerator((object) enumerator);
    IronPython.Runtime.List list = new IronPython.Runtime.List();
    while (enumerator1.MoveNext())
      list.AddNoLock(data.Target((CallSite) data, context, func, enumerator1.Current));
    return list;
  }

  public static IronPython.Runtime.List map(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, PythonFunction, object, object>>> storage,
    [NotNull] PythonFunction func,
    [NotNull] IList enumerator)
  {
    if (storage.Data == null)
      storage.Data = Builtin.MakeMapSite<PythonFunction, object>(context);
    CallSite<Func<CallSite, CodeContext, PythonFunction, object, object>> data = storage.Data;
    IEnumerator enumerator1 = PythonOps.GetEnumerator((object) enumerator);
    IronPython.Runtime.List list = new IronPython.Runtime.List(enumerator.Count);
    while (enumerator1.MoveNext())
      list.AddNoLock(data.Target((CallSite) data, context, func, enumerator1.Current));
    return list;
  }

  public static IronPython.Runtime.List map(CodeContext context, params object[] param)
  {
    if (param == null || param.Length < 2)
      throw PythonOps.TypeError("at least 2 arguments required to map");
    IronPython.Runtime.List list = new IronPython.Runtime.List();
    object func = param[0];
    IEnumerator[] enumeratorArray = new IEnumerator[param.Length - 1];
    for (int index = 0; index < enumeratorArray.Length; ++index)
      enumeratorArray[index] = PythonOps.GetEnumerator(param[index + 1]);
    object[] objArray = new object[enumeratorArray.Length];
    while (true)
    {
      bool flag = true;
      for (int index = 0; index < enumeratorArray.Length; ++index)
      {
        if (enumeratorArray[index].MoveNext())
        {
          objArray[index] = enumeratorArray[index].Current;
          flag = false;
        }
        else
          objArray[index] = (object) null;
      }
      if (!flag)
      {
        if (func != null)
          list.AddNoLock(PythonCalls.Call(context, func, objArray));
        else if (objArray.Length == 1)
        {
          list.AddNoLock(objArray[0]);
        }
        else
        {
          list.AddNoLock((object) PythonTuple.MakeTuple(objArray));
          objArray = new object[enumeratorArray.Length];
        }
      }
      else
        break;
    }
    return list;
  }

  public static object max(CodeContext context, object x)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(x);
    object other = enumerator.MoveNext() ? enumerator.Current : throw PythonOps.ValueError("max() arg is an empty sequence");
    PythonContext languageContext = context.LanguageContext;
    while (enumerator.MoveNext())
    {
      if (languageContext.GreaterThan(enumerator.Current, other))
        other = enumerator.Current;
    }
    return other;
  }

  public static object max(CodeContext context, object x, object y)
  {
    return !context.LanguageContext.GreaterThan(x, y) ? y : x;
  }

  public static object max(CodeContext context, params object[] args)
  {
    object obj = args.Length != 0 ? args[0] : throw PythonOps.TypeError("max expecting 1 arguments, got 0");
    if (args.Length == 1)
      return Builtin.max(context, obj);
    PythonContext languageContext = context.LanguageContext;
    for (int index = 1; index < args.Length; ++index)
    {
      if (languageContext.GreaterThan(args[index], obj))
        obj = args[index];
    }
    return obj;
  }

  public static object max(CodeContext context, object x, [ParamDictionary] IDictionary<object, object> dict)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(x);
    if (!enumerator.MoveNext())
      throw PythonOps.ValueError(" max() arg is an empty sequence");
    object maxKwArg = Builtin.GetMaxKwArg(dict);
    object current = enumerator.Current;
    object other = PythonCalls.Call(context, maxKwArg, enumerator.Current);
    PythonContext languageContext = context.LanguageContext;
    while (enumerator.MoveNext())
    {
      object self = PythonCalls.Call(context, maxKwArg, enumerator.Current);
      if (languageContext.GreaterThan(self, other))
      {
        current = enumerator.Current;
        other = self;
      }
    }
    return current;
  }

  public static object max(
    CodeContext context,
    object x,
    object y,
    [ParamDictionary] IDictionary<object, object> dict)
  {
    object maxKwArg = Builtin.GetMaxKwArg(dict);
    return !context.LanguageContext.GreaterThan(PythonCalls.Call(context, maxKwArg, x), PythonCalls.Call(context, maxKwArg, y)) ? y : x;
  }

  public static object max(
    CodeContext context,
    [ParamDictionary] IDictionary<object, object> dict,
    params object[] args)
  {
    if (args.Length == 0)
      throw PythonOps.TypeError("max expecting 1 arguments, got 0");
    int index1 = 0;
    if (args.Length == 1)
      return Builtin.max(context, args[index1], dict);
    object maxKwArg = Builtin.GetMaxKwArg(dict);
    object other = PythonCalls.Call(context, maxKwArg, args[index1]);
    PythonContext languageContext = context.LanguageContext;
    for (int index2 = 1; index2 < args.Length; ++index2)
    {
      object self = PythonCalls.Call(context, maxKwArg, args[index2]);
      if (languageContext.GreaterThan(self, other))
      {
        index1 = index2;
        other = self;
      }
    }
    return args[index1];
  }

  private static object GetMaxKwArg(IDictionary<object, object> dict)
  {
    return dict.Count == 1 ? Builtin.VerifyKeys("max", dict) : throw PythonOps.TypeError(" max() should have only 1 keyword argument, but got {0} keyword arguments", (object) dict.Count);
  }

  public static object min(CodeContext context, object x)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(x);
    object other = enumerator.MoveNext() ? enumerator.Current : throw PythonOps.ValueError("empty sequence");
    PythonContext languageContext = context.LanguageContext;
    while (enumerator.MoveNext())
    {
      if (languageContext.LessThan(enumerator.Current, other))
        other = enumerator.Current;
    }
    return other;
  }

  public static object min(CodeContext context, object x, object y)
  {
    return !context.LanguageContext.LessThan(x, y) ? y : x;
  }

  public static object min(CodeContext context, params object[] args)
  {
    object obj = args.Length != 0 ? args[0] : throw PythonOps.TypeError("min expecting 1 arguments, got 0");
    if (args.Length == 1)
      return Builtin.min(context, obj);
    PythonContext languageContext = context.LanguageContext;
    for (int index = 1; index < args.Length; ++index)
    {
      if (languageContext.LessThan(args[index], obj))
        obj = args[index];
    }
    return obj;
  }

  public static object min(CodeContext context, object x, [ParamDictionary] IDictionary<object, object> dict)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(x);
    if (!enumerator.MoveNext())
      throw PythonOps.ValueError(" min() arg is an empty sequence");
    object minKwArg = Builtin.GetMinKwArg(dict);
    object current = enumerator.Current;
    object other = PythonCalls.Call(context, minKwArg, enumerator.Current);
    PythonContext languageContext = context.LanguageContext;
    while (enumerator.MoveNext())
    {
      object self = PythonCalls.Call(context, minKwArg, enumerator.Current);
      if (languageContext.LessThan(self, other))
      {
        current = enumerator.Current;
        other = self;
      }
    }
    return current;
  }

  public static object min(
    CodeContext context,
    object x,
    object y,
    [ParamDictionary] IDictionary<object, object> dict)
  {
    object minKwArg = Builtin.GetMinKwArg(dict);
    return !context.LanguageContext.LessThan(PythonCalls.Call(context, minKwArg, x), PythonCalls.Call(context, minKwArg, y)) ? y : x;
  }

  public static object min(
    CodeContext context,
    [ParamDictionary] IDictionary<object, object> dict,
    params object[] args)
  {
    if (args.Length == 0)
      throw PythonOps.TypeError("min expecting 1 arguments, got 0");
    int index1 = 0;
    if (args.Length == 1)
      return Builtin.min(context, args[index1], dict);
    object minKwArg = Builtin.GetMinKwArg(dict);
    object other = PythonCalls.Call(context, minKwArg, args[index1]);
    PythonContext languageContext = context.LanguageContext;
    for (int index2 = 1; index2 < args.Length; ++index2)
    {
      object self = PythonCalls.Call(context, minKwArg, args[index2]);
      if (languageContext.LessThan(self, other))
      {
        index1 = index2;
        other = self;
      }
    }
    return args[index1];
  }

  private static object GetMinKwArg([ParamDictionary] IDictionary<object, object> dict)
  {
    return dict.Count == 1 ? Builtin.VerifyKeys("min", dict) : throw PythonOps.TypeError(" min() should have only 1 keyword argument, but got {0} keyword arguments", (object) dict.Count);
  }

  private static object VerifyKeys(string name, IDictionary<object, object> dict)
  {
    object obj;
    if (!dict.TryGetValue((object) "key", out obj))
    {
      IEnumerator<object> enumerator = dict.Keys.GetEnumerator();
      if (enumerator.MoveNext())
        throw PythonOps.TypeError(" {1}() got an unexpected keyword argument ({0})", enumerator.Current, (object) name);
    }
    return obj;
  }

  public static object next(IEnumerator iter)
  {
    return iter.MoveNext() ? iter.Current : throw PythonOps.StopIteration();
  }

  public static object next(IEnumerator iter, object defaultVal)
  {
    return iter.MoveNext() ? iter.Current : defaultVal;
  }

  [LightThrowing]
  public static object next(PythonGenerator gen) => gen.next();

  [LightThrowing]
  public static object next(PythonGenerator gen, object defaultVal)
  {
    object exceptionValue = gen.next();
    System.Exception lightException = LightExceptions.GetLightException(exceptionValue);
    return lightException != null && lightException is StopIterationException ? defaultVal : exceptionValue;
  }

  public static object next(CodeContext context, object iter)
  {
    return PythonOps.Invoke(context, iter, nameof (next));
  }

  public static object next(CodeContext context, object iter, object defaultVal)
  {
    try
    {
      return PythonOps.Invoke(context, iter, nameof (next));
    }
    catch (StopIterationException ex)
    {
      return defaultVal;
    }
  }

  public static PythonType @object => DynamicHelpers.GetPythonTypeFromType(typeof (object));

  public static object oct(object o) => PythonOps.Oct(o);

  public static PythonFile open(CodeContext context, string name, string mode = "r", int buffering = -1)
  {
    PythonFile pythonFile = new PythonFile(context);
    pythonFile.__init__(context, name, mode, buffering);
    return pythonFile;
  }

  public static PythonFile open(
    CodeContext context,
    string name,
    string mode = "r",
    BigInteger buffering)
  {
    return Builtin.open(context, name, mode, (int) buffering);
  }

  public static PythonFile open(CodeContext context, string name, string mode = "r", double buffering)
  {
    throw PythonOps.TypeError("integer argument expected, got float");
  }

  public static PythonFile open(CodeContext context, [NotNull] Stream stream)
  {
    PythonFile pythonFile = new PythonFile(context);
    pythonFile.__init__(context, stream);
    return pythonFile;
  }

  public static int ord(object value)
  {
    switch (value)
    {
      case char ch:
        return (int) ch;
      case ExtensibleString extensibleString:
        pattern_0 = extensibleString.Value;
        break;
    }
    if (pattern_0 != null)
      return pattern_0.Length == 1 ? (int) pattern_0[0] : throw PythonOps.TypeError("expected a character, but string of length {0} found", (object) pattern_0.Length);
    if (value is IList<byte> byteList)
      return byteList.Count == 1 ? (int) byteList[0] : throw PythonOps.TypeError("expected a character, but string of length {0} found", (object) byteList.Count);
    throw PythonOps.TypeError("expected a character, but {0} found", (object) PythonTypeOps.GetName(value));
  }

  public static object pow(CodeContext context, object x, object y)
  {
    return context.LanguageContext.Operation(PythonOperationKind.Power, x, y);
  }

  public static object pow(CodeContext context, object x, object y, object z)
  {
    try
    {
      return PythonOps.PowerMod(context, x, y, z);
    }
    catch (DivideByZeroException ex)
    {
      throw PythonOps.ValueError("3rd argument cannot be 0");
    }
  }

  public static void print(CodeContext context, params object[] args)
  {
    Builtin.print(context, " ", "\n", (object) null, args);
  }

  public static void print(
    CodeContext context,
    [ParamDictionary] IDictionary<object, object> kwargs,
    params object[] args)
  {
    object o1 = Builtin.AttrCollectionPop(kwargs, "sep", (object) " ");
    switch (o1)
    {
      case null:
      case string _:
        object o2 = Builtin.AttrCollectionPop(kwargs, "end", (object) "\n");
        switch (o2)
        {
          case null:
          case string _:
            object file = Builtin.AttrCollectionPop(kwargs, "file", (object) null);
            if (kwargs.Count != 0)
              throw PythonOps.TypeError("'{0}' is an invalid keyword argument for this function", new List<object>((IEnumerable<object>) kwargs.Keys)[0]);
            Builtin.print(context, (string) o1 ?? " ", (string) o2 ?? "\n", file, args);
            return;
          default:
            throw PythonOps.TypeError("end must be None or str, not {0}", (object) PythonTypeOps.GetName(o2));
        }
      default:
        throw PythonOps.TypeError("sep must be None or str, not {0}", (object) PythonTypeOps.GetName(o1));
    }
  }

  private static object AttrCollectionPop(
    IDictionary<object, object> kwargs,
    string name,
    object defaultValue)
  {
    object obj;
    if (kwargs.TryGetValue((object) name, out obj))
      kwargs.Remove((object) name);
    else
      obj = defaultValue;
    return obj;
  }

  private static void print(
    CodeContext context,
    string sep,
    string end,
    object file,
    object[] args)
  {
    PythonContext languageContext = context.LanguageContext;
    if (file == null)
      file = languageContext.SystemStandardOut;
    if (file == null)
      throw PythonOps.RuntimeError("lost sys.std_out");
    if (args == null)
      args = new object[1];
    PythonFile pythonFile = file as PythonFile;
    for (int index = 0; index < args.Length; ++index)
    {
      string s = PythonOps.ToString(context, args[index]);
      if (pythonFile != null)
      {
        pythonFile.write(s);
      }
      else
      {
        object obj1 = languageContext.WriteCallSite.Target((CallSite) languageContext.WriteCallSite, context, PythonOps.GetBoundAttr(context, file, "write"), s);
      }
      if (index != args.Length - 1)
      {
        if (pythonFile != null)
        {
          pythonFile.write(sep);
        }
        else
        {
          object obj2 = languageContext.WriteCallSite.Target((CallSite) languageContext.WriteCallSite, context, PythonOps.GetBoundAttr(context, file, "write"), sep);
        }
      }
    }
    if (pythonFile != null)
    {
      pythonFile.write(end);
    }
    else
    {
      object obj = languageContext.WriteCallSite.Target((CallSite) languageContext.WriteCallSite, context, PythonOps.GetBoundAttr(context, file, "write"), end);
    }
  }

  public static PythonType property
  {
    get => DynamicHelpers.GetPythonTypeFromType(typeof (PythonProperty));
  }

  [return: SequenceTypeInfo(new Type[] {typeof (int)})]
  public static IronPython.Runtime.List range(int stop) => Builtin.rangeWorker(stop);

  [return: SequenceTypeInfo(new Type[] {typeof (int)})]
  public static IronPython.Runtime.List range(BigInteger stop) => Builtin.rangeWorker(stop);

  private static IronPython.Runtime.List rangeWorker(int stop)
  {
    if (stop < 0)
      stop = 0;
    IronPython.Runtime.List list = PythonOps.MakeEmptyList(stop);
    for (int index = 0; index < stop; ++index)
      list.AddNoLock(ScriptingRuntimeHelpers.Int32ToObject(index));
    return list;
  }

  private static IronPython.Runtime.List rangeWorker(BigInteger stop)
  {
    if (stop < BigInteger.Zero)
      return Builtin.range(0);
    int ret;
    if (stop.AsInt32(out ret))
      return Builtin.range(ret);
    throw PythonOps.OverflowError("too many items in the range");
  }

  [return: SequenceTypeInfo(new Type[] {typeof (int)})]
  public static IronPython.Runtime.List range(int start, int stop)
  {
    return Builtin.rangeWorker(start, stop);
  }

  [return: SequenceTypeInfo(new Type[] {typeof (int)})]
  public static IronPython.Runtime.List range(BigInteger start, BigInteger stop)
  {
    return Builtin.rangeWorker(start, stop);
  }

  private static IronPython.Runtime.List rangeWorker(int start, int stop)
  {
    if (start > stop)
      stop = start;
    long num = (long) stop - (long) start;
    if ((long) int.MinValue > num || num > (long) int.MaxValue)
      throw PythonOps.OverflowError("too many items in the list");
    IronPython.Runtime.List list = PythonOps.MakeEmptyList(stop - start);
    for (int index = start; index < stop; ++index)
      list.AddNoLock(ScriptingRuntimeHelpers.Int32ToObject(index));
    return list;
  }

  private static IronPython.Runtime.List rangeWorker(BigInteger start, BigInteger stop)
  {
    if (start > stop)
      stop = start;
    int ret;
    if (!(stop - start).AsInt32(out ret))
      throw PythonOps.OverflowError("too many items in the range");
    IronPython.Runtime.List list = PythonOps.MakeEmptyList(ret);
    for (int index = 0; index < ret; ++index)
      list.AddNoLock((object) (start + (BigInteger) index));
    return list;
  }

  [return: SequenceTypeInfo(new Type[] {typeof (int)})]
  public static IronPython.Runtime.List range(int start, int stop, int step)
  {
    return Builtin.rangeWorker(start, stop, step);
  }

  [return: SequenceTypeInfo(new Type[] {typeof (int)})]
  public static IronPython.Runtime.List range(BigInteger start, BigInteger stop, BigInteger step)
  {
    return Builtin.rangeWorker(start, stop, step);
  }

  private static IronPython.Runtime.List rangeWorker(int start, int stop, int step)
  {
    if (step == 0)
      throw PythonOps.ValueError("step of 0");
    IronPython.Runtime.List list;
    if (step > 0)
    {
      if (start > stop)
        stop = start;
      list = PythonOps.MakeEmptyList((stop - start) / step);
      for (int index = start; index < stop; index += step)
        list.AddNoLock(ScriptingRuntimeHelpers.Int32ToObject(index));
    }
    else
    {
      if (start < stop)
        stop = start;
      list = PythonOps.MakeEmptyList((stop - start) / step);
      for (int index = start; index > stop; index += step)
        list.AddNoLock(ScriptingRuntimeHelpers.Int32ToObject(index));
    }
    return list;
  }

  private static IronPython.Runtime.List rangeWorker(
    BigInteger start,
    BigInteger stop,
    BigInteger step)
  {
    if (step == BigInteger.Zero)
      throw PythonOps.ValueError("step of 0");
    BigInteger self;
    if (step > BigInteger.Zero)
    {
      if (start > stop)
        stop = start;
      self = (stop - start + step - (BigInteger) 1) / step;
    }
    else
    {
      if (start < stop)
        stop = start;
      self = (stop - start + step + (BigInteger) 1) / step;
    }
    int ret;
    if (!self.AsInt32(out ret))
      throw PythonOps.OverflowError("too many items for list");
    IronPython.Runtime.List list = PythonOps.MakeEmptyList(ret);
    for (int index = 0; index < ret; ++index)
    {
      list.AddNoLock((object) start);
      start += step;
    }
    return list;
  }

  [return: SequenceTypeInfo(new Type[] {typeof (int)})]
  public static IronPython.Runtime.List range(CodeContext context, object stop)
  {
    return Builtin.range(Builtin.GetRangeInt(context, stop, "end"));
  }

  [return: SequenceTypeInfo(new Type[] {typeof (int)})]
  public static IronPython.Runtime.List range(
    CodeContext context,
    object start,
    object stop,
    object step = 1)
  {
    BigInteger rangeInt1 = Builtin.GetRangeInt(context, stop, "end");
    BigInteger rangeInt2 = Builtin.GetRangeInt(context, start, nameof (start));
    BigInteger rangeInt3 = Builtin.GetRangeInt(context, step, nameof (step));
    BigInteger stop1 = rangeInt1;
    BigInteger step1 = rangeInt3;
    return Builtin.range(rangeInt2, stop1, step1);
  }

  private static bool FastGetRangeInt(object arg, out BigInteger res)
  {
    switch (arg)
    {
      case int num:
        res = (BigInteger) num;
        return true;
      case BigInteger bigInteger:
        res = bigInteger;
        return true;
      case Extensible<int> extensible1:
        res = (BigInteger) extensible1.Value;
        return true;
      case Extensible<BigInteger> extensible2:
        res = extensible2.Value;
        return true;
      default:
        res = BigInteger.Zero;
        return false;
    }
  }

  private static BigInteger GetRangeInt(CodeContext context, object arg, string pos)
  {
    BigInteger res;
    if (Builtin.FastGetRangeInt(arg, out res))
      return res;
    if (arg is double || arg is Extensible<double>)
      throw PythonOps.TypeError("range() integer {0} argument expected, got {1}.", (object) pos, (object) PythonTypeOps.GetName(arg));
    object ret;
    if (PythonOps.TryGetBoundAttr(context, arg, "__int__", out ret))
    {
      if (!Builtin.FastGetRangeInt(PythonOps.CallWithContext(context, ret), out res))
        throw PythonOps.TypeError("__int__ should return int object");
      return res;
    }
    if (arg is OldInstance)
    {
      if (PythonOps.TryGetBoundAttr(context, arg, "__trunc__", out ret))
      {
        if (!Builtin.FastGetRangeInt(PythonOps.CallWithContext(context, ret), out res))
          throw PythonOps.TypeError("__trunc__ returned non-Integral (type {0})", (object) PythonTypeOps.GetOldName(arg));
        return res;
      }
      throw PythonOps.AttributeError("{0} instance has no attribute __trunc__", (object) PythonTypeOps.GetOldName(arg));
    }
    throw PythonOps.TypeError("range() integer {0} argument expected, got {1}.", (object) pos, (object) PythonTypeOps.GetName(arg));
  }

  public static string raw_input(CodeContext context) => Builtin.raw_input(context, (object) null);

  public static string raw_input(CodeContext context, object prompt)
  {
    PythonModule moduleByName = context.LanguageContext.GetModuleByName("readline");
    string str;
    if (moduleByName != null)
    {
      object attributeNoThrow = moduleByName.GetAttributeNoThrow(context, "rl");
      str = PythonOps.Invoke(context, attributeNoThrow, "readline", prompt) as string;
    }
    else
    {
      if (prompt != null)
        PythonOps.PrintNoNewline(context, prompt);
      str = PythonOps.ReadLineFromSrc(context, context.LanguageContext.SystemStandardIn) as string;
    }
    return str != null && str.EndsWith("\n") ? str.Substring(0, str.Length - 1) : str;
  }

  [Python3Warning("reduce() not supported in 3.x; use functools.reduce()")]
  public static object reduce(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object, object, object>>> siteData,
    object func,
    object seq)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(seq);
    if (!enumerator.MoveNext())
      throw PythonOps.TypeError("reduce() of empty sequence with no initial value");
    Builtin.EnsureReduceData(context, siteData);
    CallSite<Func<CallSite, CodeContext, object, object, object, object>> data = siteData.Data;
    object obj = enumerator.Current;
    while (enumerator.MoveNext())
      obj = data.Target((CallSite) data, context, func, obj, enumerator.Current);
    return obj;
  }

  [Python3Warning("reduce() not supported in 3.x; use functools.reduce()")]
  public static object reduce(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object, object, object>>> siteData,
    object func,
    object seq,
    object initializer)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(seq);
    Builtin.EnsureReduceData(context, siteData);
    CallSite<Func<CallSite, CodeContext, object, object, object, object>> data = siteData.Data;
    object obj = initializer;
    while (enumerator.MoveNext())
      obj = data.Target((CallSite) data, context, func, obj, enumerator.Current);
    return obj;
  }

  private static void EnsureReduceData(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object, object, object>>> siteData)
  {
    if (siteData.Data != null)
      return;
    siteData.Data = CallSite<Func<CallSite, CodeContext, object, object, object, object>>.Create((CallSiteBinder) context.LanguageContext.Invoke(new CallSignature(2)));
  }

  [Python3Warning("In 3.x, reload() is renamed to imp.reload()")]
  public static object reload(CodeContext context, PythonModule module)
  {
    if (module == null)
      throw PythonOps.TypeError("unexpected type: NoneType");
    if (Builtin._reloadStack == null)
      Interlocked.CompareExchange<List<PythonModule>>(ref Builtin._reloadStack, new List<PythonModule>(), (List<PythonModule>) null);
    if (Builtin._reloadStack.Contains(module))
      return (object) module;
    Builtin._reloadStack.Add(module);
    try
    {
      return Importer.ReloadModule(context, module);
    }
    finally
    {
      Builtin._reloadStack.RemoveAt(Builtin._reloadStack.Count - 1);
    }
  }

  public static object repr(CodeContext context, object o)
  {
    object obj = (object) PythonOps.Repr(context, o);
    switch (obj)
    {
      case string _:
      case ExtensibleString _:
        return obj;
      default:
        throw PythonOps.TypeError("__repr__ returned non-string (type {0})", (object) PythonOps.GetPythonTypeName(o));
    }
  }

  public static PythonType reversed
  {
    get => DynamicHelpers.GetPythonTypeFromType(typeof (ReversedEnumerator));
  }

  public static double round(double number) => MathUtils.RoundAwayFromZero(number);

  public static double round(double number, int ndigits)
  {
    return PythonOps.CheckMath(number, MathUtils.RoundAwayFromZero(number, ndigits));
  }

  public static double round(double number, BigInteger ndigits)
  {
    int ret;
    if (ndigits.AsInt32(out ret))
      return Builtin.round(number, ret);
    return !(ndigits > 0L) ? 0.0 : number;
  }

  public static double round(double number, double ndigits)
  {
    throw PythonOps.TypeError("'float' object cannot be interpreted as an index");
  }

  public static void setattr(CodeContext context, object o, string name, object val)
  {
    PythonOps.SetAttr(context, o, name, val);
  }

  public static PythonType slice => DynamicHelpers.GetPythonTypeFromType(typeof (IronPython.Runtime.Slice));

  public static IronPython.Runtime.List sorted(CodeContext context, object iterable)
  {
    return Builtin.sorted(context, iterable, (object) null, (object) null, false);
  }

  public static IronPython.Runtime.List sorted(CodeContext context, object iterable, object cmp)
  {
    return Builtin.sorted(context, iterable, cmp, (object) null, false);
  }

  public static IronPython.Runtime.List sorted(
    CodeContext context,
    object iterable,
    object cmp,
    object key)
  {
    return Builtin.sorted(context, iterable, cmp, key, false);
  }

  public static IronPython.Runtime.List sorted(
    CodeContext context,
    object iterable = null,
    object cmp = null,
    object key = null,
    bool reverse = false)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(iterable);
    IronPython.Runtime.List list = PythonOps.MakeEmptyList(10);
    while (enumerator.MoveNext())
      list.AddNoLock(enumerator.Current);
    list.sort(context, cmp, key, reverse);
    return list;
  }

  public static PythonType staticmethod
  {
    get => DynamicHelpers.GetPythonTypeFromType(typeof (IronPython.Runtime.staticmethod));
  }

  public static object sum(CodeContext context, object sequence)
  {
    return Builtin.sum(context, sequence, (object) 0);
  }

  public static object sum(CodeContext context, [NotNull] IronPython.Runtime.List sequence)
  {
    return Builtin.sum(context, sequence, (object) 0);
  }

  public static object sum(CodeContext context, [NotNull] PythonTuple sequence)
  {
    return Builtin.sum(context, sequence, (object) 0);
  }

  public static object sum(CodeContext context, object sequence, object start)
  {
    IEnumerator enumerator = PythonOps.GetEnumerator(sequence);
    if (start is string)
      throw PythonOps.TypeError("Cannot sum strings, use '{0}'.join(seq)", start);
    Builtin.SumState state = new Builtin.SumState(context.LanguageContext, start);
    while (enumerator.MoveNext())
      Builtin.SumOne(ref state, enumerator.Current);
    return state.CurrentValue;
  }

  public static object sum(CodeContext context, [NotNull] IronPython.Runtime.List sequence, object start)
  {
    if (start is string)
      throw PythonOps.TypeError("Cannot sum strings, use '{0}'.join(seq)", start);
    Builtin.SumState state = new Builtin.SumState(context.LanguageContext, start);
    for (int index = 0; index < sequence._size; ++index)
      Builtin.SumOne(ref state, sequence._data[index]);
    return state.CurrentValue;
  }

  public static object sum(CodeContext context, [NotNull] PythonTuple sequence, object start)
  {
    if (start is string)
      throw PythonOps.TypeError("Cannot sum strings, use '{0}'.join(seq)", start);
    Builtin.SumState state = new Builtin.SumState(context.LanguageContext, start);
    foreach (object current in sequence._data)
      Builtin.SumOne(ref state, current);
    return state.CurrentValue;
  }

  private static void SumOne(ref Builtin.SumState state, object current)
  {
    if (current != null)
    {
      if (state.CurType == Builtin.SumVariantType.Int)
      {
        if (current.GetType() == typeof (int))
        {
          try
          {
            checked { state.IntVal += (int) current; }
          }
          catch (OverflowException ex)
          {
            state.BigIntVal = (BigInteger) state.IntVal + (BigInteger) (int) current;
            state.CurType = Builtin.SumVariantType.BigInt;
          }
        }
        else if (current.GetType() == typeof (double))
        {
          state.DoubleVal = (double) state.IntVal + (double) current;
          state.CurType = Builtin.SumVariantType.Double;
        }
        else if (current.GetType() == typeof (BigInteger))
        {
          state.BigIntVal = (BigInteger) state.IntVal + (BigInteger) current;
          state.CurType = Builtin.SumVariantType.BigInt;
        }
        else
          Builtin.SumObject(ref state, (object) state.IntVal, current);
      }
      else if (state.CurType == Builtin.SumVariantType.Double)
      {
        if (current.GetType() == typeof (double))
          state.DoubleVal += (double) current;
        else if (current.GetType() == typeof (int))
          state.DoubleVal += (double) (int) current;
        else if (current.GetType() == typeof (BigInteger))
          Builtin.SumBigIntAndDouble(ref state, (BigInteger) current, state.DoubleVal);
        else
          Builtin.SumObject(ref state, (object) state.DoubleVal, current);
      }
      else if (state.CurType == Builtin.SumVariantType.BigInt)
      {
        if (current.GetType() == typeof (BigInteger))
          state.BigIntVal += (BigInteger) current;
        else if (current.GetType() == typeof (int))
          state.BigIntVal += (BigInteger) (int) current;
        else if (current.GetType() == typeof (double))
          Builtin.SumBigIntAndDouble(ref state, state.BigIntVal, (double) current);
        else
          Builtin.SumObject(ref state, (object) state.BigIntVal, current);
      }
      else
      {
        if (state.CurType != Builtin.SumVariantType.Object)
          return;
        state.ObjectVal = state.AddSite.Target((CallSite) state.AddSite, state.ObjectVal, current);
      }
    }
    else
      Builtin.SumObject(ref state, (object) state.BigIntVal, current);
  }

  private static void SumBigIntAndDouble(ref Builtin.SumState state, BigInteger bigInt, double dbl)
  {
    if (bigInt <= Builtin.MaxDouble && bigInt >= Builtin.MinDouble)
    {
      state.DoubleVal = (double) bigInt + dbl;
      state.CurType = Builtin.SumVariantType.Double;
    }
    else
      Builtin.SumObject(ref state, (object) dbl, (object) bigInt);
  }

  private static void SumObject(ref Builtin.SumState state, object value, object current)
  {
    state.ObjectVal = state.AddSite.Target((CallSite) state.AddSite, value, current);
    state.CurType = Builtin.SumVariantType.Object;
  }

  public static PythonType super => DynamicHelpers.GetPythonTypeFromType(typeof (Super));

  public static PythonType str => DynamicHelpers.GetPythonTypeFromType(typeof (string));

  public static PythonType tuple => DynamicHelpers.GetPythonTypeFromType(typeof (PythonTuple));

  public static PythonType type => DynamicHelpers.GetPythonTypeFromType(typeof (PythonType));

  public static string unichr(int i)
  {
    return i >= 0 && i <= (int) ushort.MaxValue ? ScriptingRuntimeHelpers.CharToString((char) i) : throw PythonOps.ValueError("{0} is not in required range", (object) i);
  }

  public static PythonType unicode => DynamicHelpers.GetPythonTypeFromType(typeof (string));

  [Documentation("vars([object]) -> dictionary\n\nWithout arguments, equivalent to locals().\nWith an argument, equivalent to object.__dict__.")]
  public static object vars(CodeContext context) => Builtin.locals(context);

  public static object vars(CodeContext context, object @object)
  {
    object ret;
    if (!PythonOps.TryGetBoundAttr(@object, "__dict__", out ret))
      throw PythonOps.TypeError("vars() argument must have __dict__ attribute");
    return ret;
  }

  public static PythonType xrange => DynamicHelpers.GetPythonTypeFromType(typeof (XRange));

  public static IronPython.Runtime.List zip(object s0, object s1)
  {
    IEnumerator enumerator1 = PythonOps.GetEnumerator(s0);
    IEnumerator enumerator2 = PythonOps.GetEnumerator(s1);
    IronPython.Runtime.List list = new IronPython.Runtime.List();
    while (enumerator1.MoveNext())
    {
      object current1 = enumerator1.Current;
      if (enumerator2.MoveNext())
      {
        object current2 = enumerator2.Current;
        list.AddNoLock((object) PythonTuple.MakeTuple(current1, current2));
      }
      else
        break;
    }
    return list;
  }

  public static IronPython.Runtime.List zip(params object[] seqs)
  {
    int length = seqs != null ? seqs.Length : throw PythonOps.TypeError("zip argument must support iteration, got None");
    switch (length)
    {
      case 0:
        return PythonOps.MakeList();
      case 2:
        return Builtin.zip(seqs[0], seqs[1]);
      default:
        IEnumerator[] enumeratorArray = new IEnumerator[length];
        for (int index = 0; index < length; ++index)
          enumeratorArray[index] = PythonOps.GetEnumerator(seqs[index]);
        IronPython.Runtime.List list = new IronPython.Runtime.List();
        while (true)
        {
          object[] objArray = new object[length];
          for (int index = 0; index < length; ++index)
          {
            if (!enumeratorArray[index].MoveNext())
              return list;
            objArray[index] = enumeratorArray[index].Current;
          }
          list.AddNoLock((object) PythonTuple.MakeTuple(objArray));
        }
    }
  }

  public static PythonType BaseException
  {
    get => DynamicHelpers.GetPythonTypeFromType(typeof (PythonExceptions.BaseException));
  }

  internal static PythonCompilerOptions GetRuntimeGeneratedCodeCompilerOptions(
    CodeContext context,
    bool inheritContext,
    CompileFlags cflags)
  {
    PythonCompilerOptions codeCompilerOptions = !inheritContext ? DefaultContext.DefaultPythonContext.GetPythonCompilerOptions() : new PythonCompilerOptions(context.ModuleContext.Features);
    ModuleOptions moduleOptions = ModuleOptions.None;
    if ((cflags & CompileFlags.CO_FUTURE_DIVISION) != (CompileFlags) 0)
      moduleOptions |= ModuleOptions.TrueDivision;
    if ((cflags & CompileFlags.CO_FUTURE_WITH_STATEMENT) != (CompileFlags) 0)
      moduleOptions |= ModuleOptions.WithStatement;
    if ((cflags & CompileFlags.CO_FUTURE_ABSOLUTE_IMPORT) != (CompileFlags) 0)
      moduleOptions |= ModuleOptions.AbsoluteImports;
    if ((cflags & CompileFlags.CO_FUTURE_PRINT_FUNCTION) != (CompileFlags) 0)
      moduleOptions |= ModuleOptions.PrintFunction;
    if ((cflags & CompileFlags.CO_FUTURE_UNICODE_LITERALS) != (CompileFlags) 0)
      moduleOptions |= ModuleOptions.UnicodeLiterals;
    codeCompilerOptions.Module |= moduleOptions;
    codeCompilerOptions.Module &= ~(ModuleOptions.Optimized | ModuleOptions.Initialize);
    codeCompilerOptions.Module |= ModuleOptions.ExecOrEvalCode | ModuleOptions.Interpret;
    codeCompilerOptions.CompilationMode = CompilationMode.Lookup;
    return codeCompilerOptions;
  }

  private static bool GetCompilerInheritance(object dontInherit)
  {
    return dontInherit == null || Converter.ConvertToInt32(dontInherit) == 0;
  }

  private static CompileFlags GetCompilerFlags(int flags)
  {
    CompileFlags compileFlags = (CompileFlags) flags;
    return (compileFlags & ~(CompileFlags.CO_NESTED | CompileFlags.CO_DONT_IMPLY_DEDENT | CompileFlags.CO_GENERATOR_ALLOWED | CompileFlags.CO_FUTURE_DIVISION | CompileFlags.CO_FUTURE_ABSOLUTE_IMPORT | CompileFlags.CO_FUTURE_WITH_STATEMENT | CompileFlags.CO_FUTURE_PRINT_FUNCTION | CompileFlags.CO_FUTURE_UNICODE_LITERALS)) == (CompileFlags) 0 ? compileFlags : throw PythonOps.ValueError("unrecognized flags");
  }

  private static CodeContext GetExecEvalScopeOptional(
    CodeContext context,
    PythonDictionary globals,
    object localsDict,
    bool copyModule)
  {
    if (localsDict == null)
      localsDict = (object) globals;
    if (globals == null)
      globals = Builtin.globals(context);
    if (localsDict == null)
      localsDict = Builtin.locals(context);
    return Builtin.GetExecEvalScope(context, globals, Builtin.GetAttrLocals(context, localsDict), copyModule, true);
  }

  internal static CodeContext GetExecEvalScope(
    CodeContext context,
    PythonDictionary globals,
    PythonDictionary locals,
    bool copyModule,
    bool setBuiltinsToModule)
  {
    PythonContext languageContext = context.LanguageContext;
    ModuleContext moduleContext = new ModuleContext(PythonDictionary.FromIAC(context, globals), context.LanguageContext);
    CodeContext execEvalScope = locals != null ? new CodeContext(PythonDictionary.FromIAC(context, locals), moduleContext) : moduleContext.GlobalContext;
    if (!globals.ContainsKey((object) "__builtins__"))
    {
      if (setBuiltinsToModule)
        globals[(object) "__builtins__"] = languageContext.SystemStateModules[(object) "__builtin__"];
      else
        globals[(object) "__builtins__"] = (object) languageContext.BuiltinModuleDict;
    }
    return execEvalScope;
  }

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    dict[(object) "__debug__"] = ScriptingRuntimeHelpers.BooleanToObject(!context.PythonOptions.Optimize);
  }

  public static PythonType GeneratorExit => PythonExceptions.GeneratorExit;

  public static PythonType SystemExit => PythonExceptions.SystemExit;

  public static PythonType KeyboardInterrupt => PythonExceptions.KeyboardInterrupt;

  public static PythonType Exception => PythonExceptions.Exception;

  public static PythonType StopIteration => PythonExceptions.StopIteration;

  public static PythonType StandardError => PythonExceptions.StandardError;

  public static PythonType BufferError => PythonExceptions.BufferError;

  public static PythonType ArithmeticError => PythonExceptions.ArithmeticError;

  public static PythonType FloatingPointError => PythonExceptions.FloatingPointError;

  public static PythonType OverflowError => PythonExceptions.OverflowError;

  public static PythonType ZeroDivisionError => PythonExceptions.ZeroDivisionError;

  public static PythonType AssertionError => PythonExceptions.AssertionError;

  public static PythonType AttributeError => PythonExceptions.AttributeError;

  public static PythonType EnvironmentError => PythonExceptions.EnvironmentError;

  public static PythonType IOError => PythonExceptions.IOError;

  public static PythonType OSError => PythonExceptions.OSError;

  public static PythonType WindowsError => PythonExceptions.WindowsError;

  public static PythonType EOFError => PythonExceptions.EOFError;

  public static PythonType ImportError => PythonExceptions.ImportError;

  public static PythonType LookupError => PythonExceptions.LookupError;

  public static PythonType IndexError => PythonExceptions.IndexError;

  public static PythonType KeyError => PythonExceptions.KeyError;

  public static PythonType MemoryError => PythonExceptions.MemoryError;

  public static PythonType NameError => PythonExceptions.NameError;

  public static PythonType UnboundLocalError => PythonExceptions.UnboundLocalError;

  public static PythonType ReferenceError => PythonExceptions.ReferenceError;

  public static PythonType RuntimeError => PythonExceptions.RuntimeError;

  public static PythonType NotImplementedError => PythonExceptions.NotImplementedError;

  public static PythonType SyntaxError => PythonExceptions.SyntaxError;

  public static PythonType IndentationError => PythonExceptions.IndentationError;

  public static PythonType TabError => PythonExceptions.TabError;

  public static PythonType SystemError => PythonExceptions.SystemError;

  public static PythonType TypeError => PythonExceptions.TypeError;

  public static PythonType ValueError => PythonExceptions.ValueError;

  public static PythonType UnicodeError => PythonExceptions.UnicodeError;

  public static PythonType UnicodeDecodeError => PythonExceptions.UnicodeDecodeError;

  public static PythonType UnicodeEncodeError => PythonExceptions.UnicodeEncodeError;

  public static PythonType UnicodeTranslateError => PythonExceptions.UnicodeTranslateError;

  public static PythonType Warning => PythonExceptions.Warning;

  public static PythonType DeprecationWarning => PythonExceptions.DeprecationWarning;

  public static PythonType PendingDeprecationWarning => PythonExceptions.PendingDeprecationWarning;

  public static PythonType RuntimeWarning => PythonExceptions.RuntimeWarning;

  public static PythonType SyntaxWarning => PythonExceptions.SyntaxWarning;

  public static PythonType UserWarning => PythonExceptions.UserWarning;

  public static PythonType FutureWarning => PythonExceptions.FutureWarning;

  public static PythonType ImportWarning => PythonExceptions.ImportWarning;

  public static PythonType UnicodeWarning => PythonExceptions.UnicodeWarning;

  public static PythonType BytesWarning => PythonExceptions.BytesWarning;

  [PythonType("basestring")]
  [PythonHidden(new PlatformID[] {})]
  public class _basestring
  {
    public static void __new__(
      CodeContext context,
      object cls,
      [ParamDictionary] IDictionary<object, object> kwargs,
      params object[] args)
    {
      throw PythonOps.TypeError("The basestring type cannot be instantiated");
    }
  }

  private enum SumVariantType
  {
    Double,
    Int,
    BigInt,
    Object,
  }

  private struct SumState
  {
    public double DoubleVal;
    public int IntVal;
    public object ObjectVal;
    public BigInteger BigIntVal;
    public Builtin.SumVariantType CurType;
    public CallSite<Func<CallSite, object, object, object>> AddSite;

    public SumState(PythonContext context, object start)
    {
      this.DoubleVal = 0.0;
      this.IntVal = 0;
      this.ObjectVal = start;
      this.BigIntVal = BigInteger.Zero;
      this.AddSite = context.EnsureAddSite();
      if (start != null)
      {
        if (start.GetType() == typeof (int))
        {
          this.CurType = Builtin.SumVariantType.Int;
          this.IntVal = (int) start;
        }
        else if (start.GetType() == typeof (double))
        {
          this.CurType = Builtin.SumVariantType.Double;
          this.DoubleVal = (double) start;
        }
        else if (start.GetType() == typeof (BigInteger))
        {
          this.CurType = Builtin.SumVariantType.BigInt;
          this.BigIntVal = (BigInteger) start;
        }
        else
          this.CurType = Builtin.SumVariantType.Object;
      }
      else
        this.CurType = Builtin.SumVariantType.Object;
    }

    public object CurrentValue
    {
      get
      {
        switch (this.CurType)
        {
          case Builtin.SumVariantType.Double:
            return (object) this.DoubleVal;
          case Builtin.SumVariantType.Int:
            return (object) this.IntVal;
          case Builtin.SumVariantType.BigInt:
            return (object) this.BigIntVal;
          case Builtin.SumVariantType.Object:
            return this.ObjectVal;
          default:
            throw Microsoft.Scripting.Utils.Assert.Unreachable;
        }
      }
    }
  }
}
