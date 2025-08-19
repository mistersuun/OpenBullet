// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.PythonOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using IronPython.Compiler.Ast;
using IronPython.Hosting;
using IronPython.Modules;
using IronPython.Runtime.Binding;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.ComInterop;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Hosting.Providers;
using Microsoft.Scripting.Hosting.Shell;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class PythonOps
{
  [ThreadStatic]
  private static List<object> InfiniteRepr;
  [ThreadStatic]
  internal static Exception RawException;
  public static readonly PythonTuple EmptyTuple = PythonTuple.EMPTY;
  private static readonly Type[] _DelegateCtorSignature = new Type[2]
  {
    typeof (object),
    typeof (IntPtr)
  };
  [ThreadStatic]
  private static List<FunctionStack> _funcStack;

  public static BigInteger MakeIntegerFromHex(string s)
  {
    return LiteralParser.ParseBigInteger(s, 16 /*0x10*/);
  }

  public static PythonDictionary MakeDict(int size) => new PythonDictionary(size);

  public static PythonDictionary MakeEmptyDict()
  {
    return new PythonDictionary((DictionaryStorage) EmptyDictionaryStorage.Instance);
  }

  public static PythonDictionary MakeDictFromItems(params object[] data)
  {
    return new PythonDictionary((DictionaryStorage) new CommonDictionaryStorage(data, false));
  }

  public static PythonDictionary MakeConstantDict(object items)
  {
    return new PythonDictionary((DictionaryStorage) items);
  }

  public static object MakeConstantDictStorage(params object[] data)
  {
    return (object) new ConstantDictionaryStorage(new CommonDictionaryStorage(data, false));
  }

  public static SetCollection MakeSet(params object[] items) => new SetCollection(items);

  public static SetCollection MakeEmptySet() => new SetCollection();

  public static PythonDictionary MakeHomogeneousDictFromItems(object[] data)
  {
    return new PythonDictionary((DictionaryStorage) new CommonDictionaryStorage(data, true));
  }

  public static bool IsCallable(CodeContext context, object o)
  {
    return context.LanguageContext.IsCallable(o);
  }

  public static bool UserObjectIsCallable(CodeContext context, object o)
  {
    object ret;
    return PythonOps.TryGetBoundAttr(context, o, "__call__", out ret) && ret != null;
  }

  public static bool IsTrue(object o) => Converter.ConvertToBoolean(o);

  public static List<object> GetReprInfinite()
  {
    if (PythonOps.InfiniteRepr == null)
      PythonOps.InfiniteRepr = new List<object>();
    return PythonOps.InfiniteRepr;
  }

  [LightThrowing]
  internal static object LookupEncodingError(CodeContext context, string name)
  {
    Dictionary<string, object> errorHandlers = context.LanguageContext.ErrorHandlers;
    lock (errorHandlers)
    {
      if (errorHandlers.ContainsKey(name))
        return errorHandlers[name];
      return LightExceptions.Throw(PythonOps.LookupError("unknown error handler name '{0}'", (object) name));
    }
  }

  internal static void RegisterEncodingError(CodeContext context, string name, object handler)
  {
    Dictionary<string, object> errorHandlers = context.LanguageContext.ErrorHandlers;
    lock (errorHandlers)
      errorHandlers[name] = PythonOps.IsCallable(context, handler) ? handler : throw PythonOps.TypeError("handler must be callable");
  }

  internal static PythonTuple LookupEncoding(CodeContext context, string encoding)
  {
    context.LanguageContext.EnsureEncodings();
    List<object> searchFunctions = context.LanguageContext.SearchFunctions;
    string str = encoding.ToLower().Replace(' ', '-');
    if (str.IndexOf(char.MinValue) != -1)
      throw PythonOps.TypeError("lookup string cannot contain null character");
    lock (searchFunctions)
    {
      for (int index = 0; index < searchFunctions.Count; ++index)
      {
        object obj = PythonCalls.Call(context, searchFunctions[index], (object) str);
        if (obj != null)
          return (PythonTuple) obj;
      }
    }
    throw PythonOps.LookupError("unknown encoding: {0}", (object) encoding);
  }

  internal static void RegisterEncoding(CodeContext context, object search_function)
  {
    if (!PythonOps.IsCallable(context, search_function))
      throw PythonOps.TypeError("search_function must be callable");
    List<object> searchFunctions = context.LanguageContext.SearchFunctions;
    lock (searchFunctions)
      searchFunctions.Add(search_function);
  }

  internal static string GetPythonTypeName(object obj)
  {
    return obj is OldInstance oldInstance ? oldInstance._class._name.ToString() : PythonTypeOps.GetName(obj);
  }

  public static string Repr(CodeContext context, object o)
  {
    switch (o)
    {
      case null:
        return "None";
      case string self:
        return StringOps.__repr__(self);
      case int x:
        return Int32Ops.__repr__(x);
      case long num:
        return num.ToString() + "L";
      case ICodeFormattable codeFormattable:
        return codeFormattable.__repr__(context);
      default:
        return PythonContext.InvokeUnaryOperator(context, UnaryOperators.Repr, o) as string;
    }
  }

  public static List<object> GetAndCheckInfinite(object o)
  {
    List<object> reprInfinite = PythonOps.GetReprInfinite();
    foreach (object obj in reprInfinite)
    {
      if (o == obj)
        return (List<object>) null;
    }
    return reprInfinite;
  }

  public static string ToString(object o) => PythonOps.ToString(DefaultContext.Default, o);

  public static string ToString(CodeContext context, object o)
  {
    switch (o)
    {
      case string str2:
        return str2;
      case null:
        return "None";
      case double x:
        return DoubleOps.__str__(context, x);
      case PythonType pythonType:
        return pythonType.__repr__(DefaultContext.Default);
      case OldClass oldClass:
        return oldClass.ToString();
      default:
        if (o.GetType() == typeof (object).Assembly.GetType("System.__ComObject"))
          return ComOps.__repr__(o);
        object o1 = PythonContext.InvokeUnaryOperator(context, UnaryOperators.String, o);
        switch (o1)
        {
          case string str1:
label_11:
            return str1;
          case Extensible<string> extensible:
            str1 = extensible.Value;
            goto label_11;
          default:
            throw PythonOps.TypeError("expected str, got {0} from __str__", (object) PythonTypeOps.GetName(o1));
        }
    }
  }

  public static string FormatString(CodeContext context, string str, object data)
  {
    return new StringFormatter(context, str, data).Format();
  }

  public static string FormatUnicode(CodeContext context, string str, object data)
  {
    return new StringFormatter(context, str, data, true).Format();
  }

  public static object Plus(object o)
  {
    switch (o)
    {
      case int _:
        return o;
      case double _:
        return o;
      case BigInteger _:
        return o;
      case Complex _:
        return o;
      case long _:
        return o;
      case float _:
        return o;
      case bool flag:
        return ScriptingRuntimeHelpers.Int32ToObject(flag ? 1 : 0);
      default:
        object obj;
        if (PythonTypeOps.TryInvokeUnaryOperator(DefaultContext.Default, o, "__pos__", out obj) && obj != NotImplementedType.Value)
          return obj;
        throw PythonOps.TypeError("bad operand type for unary +");
    }
  }

  public static object Negate(object o)
  {
    switch (o)
    {
      case int x1:
        return Int32Ops.Negate(x1);
      case double x2:
        return (object) DoubleOps.Negate(x2);
      case long x3:
        return Int64Ops.Negate(x3);
      case BigInteger x4:
        return BigIntegerOps.Negate(x4);
      case Complex complex:
        return (object) -complex;
      case float x5:
        return (object) DoubleOps.Negate((double) x5);
      case bool flag:
        return ScriptingRuntimeHelpers.Int32ToObject(flag ? -1 : 0);
      default:
        object obj;
        if (PythonTypeOps.TryInvokeUnaryOperator(DefaultContext.Default, o, "__neg__", out obj) && obj != NotImplementedType.Value)
          return obj;
        throw PythonOps.TypeError("bad operand type for unary -");
    }
  }

  public static bool IsSubClass(PythonType c, PythonType typeinfo)
  {
    if (typeinfo == Builtin.basestring && (PythonOps.IsSubClass(c, DynamicHelpers.GetPythonTypeFromType(typeof (string))) || PythonOps.IsSubClass(c, DynamicHelpers.GetPythonTypeFromType(typeof (Bytes)))))
      return true;
    return c.OldClass != null ? typeinfo.__subclasscheck__(c.OldClass) : typeinfo.__subclasscheck__(c);
  }

  public static bool IsSubClass(CodeContext context, PythonType c, object typeinfo)
  {
    if (c == null)
      throw PythonOps.TypeError("issubclass: arg 1 must be a class");
    PythonTuple pythonTuple = typeinfo != null ? typeinfo as PythonTuple : throw PythonOps.TypeError("issubclass: arg 2 must be a class");
    PythonContext languageContext = context.LanguageContext;
    if (pythonTuple != null)
    {
      foreach (object typeinfo1 in pythonTuple)
      {
        try
        {
          PythonOps.FunctionPushFrame(languageContext);
          if (PythonOps.IsSubClass(context, c, typeinfo1))
            return true;
        }
        finally
        {
          PythonOps.FunctionPopFrame();
        }
      }
      return false;
    }
    if (typeinfo is OldClass oldClass)
      return c.IsSubclassOf(oldClass.TypeObject);
    Type type = typeinfo as Type;
    if (type != (Type) null)
      typeinfo = (object) DynamicHelpers.GetPythonTypeFromType(type);
    if (typeinfo is PythonType typeinfo2)
      return PythonOps.IsSubClass(c, typeinfo2);
    object ret;
    if (!PythonOps.TryGetBoundAttr(typeinfo, "__bases__", out ret))
      throw PythonOps.TypeErrorForBadInstance("issubclass(): {0} is not a class nor a tuple of classes", typeinfo);
    IEnumerator enumerator = PythonOps.GetEnumerator(ret);
    while (enumerator.MoveNext())
    {
      if (!(enumerator.Current is PythonType other))
      {
        if (enumerator.Current is OldClass current)
          other = current.TypeObject;
        else
          continue;
      }
      if (c.IsSubclassOf(other))
        return true;
    }
    return false;
  }

  public static bool IsInstance(object o, PythonType typeinfo)
  {
    return typeinfo == Builtin.basestring ? PythonOps.IsInstance(o, DynamicHelpers.GetPythonTypeFromType(typeof (string))) || PythonOps.IsInstance(o, DynamicHelpers.GetPythonTypeFromType(typeof (Bytes))) : typeinfo.__instancecheck__(o) || PythonOps.IsInstanceDynamic(o, (object) typeinfo, DynamicHelpers.GetPythonType(o));
  }

  public static bool IsInstance(CodeContext context, object o, PythonTuple typeinfo)
  {
    PythonContext languageContext = context.LanguageContext;
    foreach (object typeinfo1 in typeinfo)
    {
      try
      {
        PythonOps.FunctionPushFrame(languageContext);
        if (typeinfo1 is PythonType)
        {
          if (PythonOps.IsInstance(o, (PythonType) typeinfo1))
            return true;
        }
        else if (typeinfo1 is PythonTuple)
        {
          if (PythonOps.IsInstance(context, o, (PythonTuple) typeinfo1))
            return true;
        }
        else if (PythonOps.IsInstance(context, o, typeinfo1))
          return true;
      }
      finally
      {
        PythonOps.FunctionPopFrame();
      }
    }
    return false;
  }

  public static bool IsInstance(CodeContext context, object o, object typeinfo)
  {
    switch (typeinfo)
    {
      case null:
        throw PythonOps.TypeError("isinstance: arg 2 must be a class, type, or tuple of classes and types");
      case PythonTuple typeinfo1:
        return PythonOps.IsInstance(context, o, typeinfo1);
      case OldClass _:
        if (o is OldInstance oldInstance)
          return oldInstance._class.IsSubclassOf(typeinfo);
        break;
    }
    PythonType pythonType = DynamicHelpers.GetPythonType(o);
    return PythonOps.IsSubClass(context, pythonType, typeinfo) || PythonOps.IsInstanceDynamic(o, typeinfo);
  }

  private static bool IsInstanceDynamic(object o, object typeinfo)
  {
    return PythonOps.IsInstanceDynamic(o, typeinfo, DynamicHelpers.GetPythonType(o));
  }

  private static bool IsInstanceDynamic(object o, object typeinfo, PythonType odt)
  {
    switch (o)
    {
      case IPythonObject _:
      case OldInstance _:
        object ret;
        if (PythonOps.TryGetBoundAttr(o, "__class__", out ret) && odt != ret)
          return PythonOps.IsSubclassSlow(ret, typeinfo);
        break;
    }
    return false;
  }

  private static bool IsSubclassSlow(object cls, object typeinfo)
  {
    if (cls == null)
      return false;
    if (cls.Equals(typeinfo))
      return true;
    object ret;
    if (!PythonOps.TryGetBoundAttr(cls, "__bases__", out ret) || !(ret is PythonTuple pythonTuple))
      return false;
    foreach (object cls1 in pythonTuple)
    {
      if (PythonOps.IsSubclassSlow(cls1, typeinfo))
        return true;
    }
    return false;
  }

  public static object OnesComplement(object o)
  {
    switch (o)
    {
      case int num1:
        return (object) ~num1;
      case long num2:
        return (object) ~num2;
      case BigInteger bigInteger:
        return (object) ~bigInteger;
      case bool flag:
        return ScriptingRuntimeHelpers.Int32ToObject(flag ? -2 : -1);
      default:
        object obj;
        if (PythonTypeOps.TryInvokeUnaryOperator(DefaultContext.Default, o, "__invert__", out obj) && obj != NotImplementedType.Value)
          return obj;
        throw PythonOps.TypeError("bad operand type for unary ~");
    }
  }

  public static bool Not(object o) => !PythonOps.IsTrue(o);

  public static bool IsRetBool(object x, object y)
  {
    if (x == y)
      return true;
    bool? nullable1 = x as bool?;
    if (!nullable1.HasValue)
      return false;
    bool? nullable2 = nullable1;
    bool? nullable3 = y as bool?;
    return nullable2.GetValueOrDefault() == nullable3.GetValueOrDefault() & nullable2.HasValue == nullable3.HasValue;
  }

  public static object Is(object x, object y)
  {
    return !PythonOps.IsRetBool(x, y) ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  public static object IsNot(object x, object y)
  {
    return !PythonOps.IsRetBool(x, y) ? ScriptingRuntimeHelpers.True : ScriptingRuntimeHelpers.False;
  }

  internal static object MultiplySequence<T>(
    PythonOps.MultiplySequenceWorker<T> multiplier,
    T sequence,
    IronPython.Runtime.Index count,
    bool isForward)
  {
    object obj;
    if (isForward && count != null && PythonTypeOps.TryInvokeBinaryOperator(DefaultContext.Default, count.Value, (object) sequence, "__rmul__", out obj) && obj != NotImplementedType.Value)
      return obj;
    int count1 = PythonOps.GetSequenceMultiplier((object) sequence, count.Value);
    if (count1 < 0)
      count1 = 0;
    return (object) multiplier(sequence, count1);
  }

  internal static int GetSequenceMultiplier(object sequence, object count)
  {
    int index;
    if (!Converter.TryConvertToIndex(count, out index))
    {
      PythonTuple pythonTuple = (PythonTuple) null;
      if (count is OldInstance || !DynamicHelpers.GetPythonType(count).IsSystemType)
        pythonTuple = Builtin.TryCoerce(DefaultContext.Default, count, sequence) as PythonTuple;
      if (pythonTuple == null || !Converter.TryConvertToIndex(pythonTuple[0], out index))
        throw PythonOps.TypeError("can't multiply sequence by non-int of type '{0}'", (object) PythonTypeOps.GetName(count));
    }
    return index;
  }

  public static object Equal(CodeContext context, object x, object y)
  {
    PythonContext languageContext = context.LanguageContext;
    return languageContext.EqualSite.Target((CallSite) languageContext.EqualSite, x, y);
  }

  public static bool EqualRetBool(object x, object y)
  {
    switch (x)
    {
      case int _ when y is int num:
        return (int) x == num;
      case string _ when y is string:
        return ((string) x).Equals((string) y);
      default:
        return DynamicHelpers.GetPythonType(x).EqualRetBool(x, y);
    }
  }

  public static bool EqualRetBool(CodeContext context, object x, object y)
  {
    switch (x)
    {
      case int _ when y is int num:
        return (int) x == num;
      case string _ when y is string:
        return ((string) x).Equals((string) y);
      default:
        return DynamicHelpers.GetPythonType(x).EqualRetBool(x, y);
    }
  }

  public static int Compare(object x, object y) => PythonOps.Compare(DefaultContext.Default, x, y);

  public static int Compare(CodeContext context, object x, object y)
  {
    return x == y ? 0 : DynamicHelpers.GetPythonType(x).Compare(x, y);
  }

  public static object CompareEqual(int res)
  {
    return res != 0 ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  public static object CompareNotEqual(int res)
  {
    return res != 0 ? ScriptingRuntimeHelpers.True : ScriptingRuntimeHelpers.False;
  }

  public static object CompareGreaterThan(int res)
  {
    return res <= 0 ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  public static object CompareGreaterThanOrEqual(int res)
  {
    return res < 0 ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  public static object CompareLessThan(int res)
  {
    return res >= 0 ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  public static object CompareLessThanOrEqual(int res)
  {
    return res > 0 ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True;
  }

  public static bool CompareTypesEqual(CodeContext context, object x, object y)
  {
    if (x == null && y == null)
      return true;
    if (x == null || y == null)
      return false;
    return DynamicHelpers.GetPythonType(x) == DynamicHelpers.GetPythonType(y) ? x == y : PythonOps.CompareTypesWorker(context, false, x, y) == 0;
  }

  public static bool CompareTypesNotEqual(CodeContext context, object x, object y)
  {
    return PythonOps.CompareTypesWorker(context, false, x, y) != 0;
  }

  public static bool CompareTypesGreaterThan(CodeContext context, object x, object y)
  {
    return PythonOps.CompareTypes(context, x, y) > 0;
  }

  public static bool CompareTypesLessThan(CodeContext context, object x, object y)
  {
    return PythonOps.CompareTypes(context, x, y) < 0;
  }

  public static bool CompareTypesGreaterThanOrEqual(CodeContext context, object x, object y)
  {
    return PythonOps.CompareTypes(context, x, y) >= 0;
  }

  public static bool CompareTypesLessThanOrEqual(CodeContext context, object x, object y)
  {
    return PythonOps.CompareTypes(context, x, y) <= 0;
  }

  public static int CompareTypesWorker(CodeContext context, bool shouldWarn, object x, object y)
  {
    if (x == null && y == null)
      return 0;
    if (x == null)
      return -1;
    if (y == null)
      return 1;
    int num;
    if (DynamicHelpers.GetPythonType(x) != DynamicHelpers.GetPythonType(y))
    {
      if (shouldWarn && context.LanguageContext.PythonOptions.WarnPython30)
        PythonOps.Warn(context, PythonExceptions.DeprecationWarning, "comparing unequal types not supported in 3.x");
      string name1;
      string name2;
      if (x.GetType() == typeof (OldInstance))
      {
        name1 = ((OldInstance) x)._class.Name;
        if (!(y.GetType() == typeof (OldInstance)))
          return -1;
        name2 = ((OldInstance) y)._class.Name;
      }
      else
      {
        if (y.GetType() == typeof (OldInstance))
          return 1;
        name1 = PythonTypeOps.GetName(x);
        name2 = PythonTypeOps.GetName(y);
      }
      num = string.CompareOrdinal(name1, name2);
      if (num == 0)
        num = (int) (IdDispenser.GetId((object) DynamicHelpers.GetPythonType(x)) - IdDispenser.GetId((object) DynamicHelpers.GetPythonType(y)));
    }
    else
      num = (int) (IdDispenser.GetId(x) - IdDispenser.GetId(y));
    if (num < 0)
      return -1;
    return num == 0 ? 0 : 1;
  }

  public static int CompareTypes(CodeContext context, object x, object y)
  {
    return PythonOps.CompareTypesWorker(context, true, x, y);
  }

  public static object GreaterThanHelper(CodeContext context, object self, object other)
  {
    return PythonOps.InternalCompare(context, PythonOperationKind.GreaterThan, self, other);
  }

  public static object LessThanHelper(CodeContext context, object self, object other)
  {
    return PythonOps.InternalCompare(context, PythonOperationKind.LessThan, self, other);
  }

  public static object GreaterThanOrEqualHelper(CodeContext context, object self, object other)
  {
    return PythonOps.InternalCompare(context, PythonOperationKind.GreaterThanOrEqual, self, other);
  }

  public static object LessThanOrEqualHelper(CodeContext context, object self, object other)
  {
    return PythonOps.InternalCompare(context, PythonOperationKind.LessThanOrEqual, self, other);
  }

  internal static object InternalCompare(
    CodeContext context,
    PythonOperationKind op,
    object self,
    object other)
  {
    object obj;
    return PythonTypeOps.TryInvokeBinaryOperator(context, self, other, Symbols.OperatorToSymbol(op), out obj) ? obj : (object) NotImplementedType.Value;
  }

  public static int CompareToZero(object value)
  {
    double result;
    if (!Converter.TryConvertToDouble(value, out result))
      throw PythonOps.TypeErrorForBadInstance("an integer is required (got {0})", value);
    if (result > 0.0)
      return 1;
    return result < 0.0 ? -1 : 0;
  }

  public static int CompareArrays(object[] data0, int size0, object[] data1, int size1)
  {
    int num1 = Math.Min(size0, size1);
    for (int index = 0; index < num1; ++index)
    {
      int num2 = PythonOps.Compare(data0[index], data1[index]);
      if (num2 != 0)
        return num2;
    }
    if (size0 == size1)
      return 0;
    return size0 <= size1 ? -1 : 1;
  }

  public static int CompareArrays(
    object[] data0,
    int size0,
    object[] data1,
    int size1,
    IComparer comparer)
  {
    int num1 = Math.Min(size0, size1);
    for (int index = 0; index < num1; ++index)
    {
      int num2 = comparer.Compare(data0[index], data1[index]);
      if (num2 != 0)
        return num2;
    }
    if (size0 == size1)
      return 0;
    return size0 <= size1 ? -1 : 1;
  }

  public static bool ArraysEqual(object[] data0, int size0, object[] data1, int size1)
  {
    if (size0 != size1)
      return false;
    for (int index = 0; index < size0; ++index)
    {
      if (data0[index] != null)
      {
        if (!PythonOps.EqualRetBool(data0[index], data1[index]))
          return false;
      }
      else if (data1[index] != null)
        return false;
    }
    return true;
  }

  public static bool ArraysEqual(
    object[] data0,
    int size0,
    object[] data1,
    int size1,
    IEqualityComparer comparer)
  {
    if (size0 != size1)
      return false;
    for (int index = 0; index < size0; ++index)
    {
      if (data0[index] != null)
      {
        if (!comparer.Equals(data0[index], data1[index]))
          return false;
      }
      else if (data1[index] != null)
        return false;
    }
    return true;
  }

  public static object PowerMod(CodeContext context, object x, object y, object z)
  {
    if (z == null)
      return context.LanguageContext.Operation(PythonOperationKind.Power, x, y);
    object obj;
    if (x is int && y is int && z is int num)
    {
      obj = Int32Ops.Power((int) x, (int) y, new int?(num));
      if (obj != NotImplementedType.Value)
        return obj;
    }
    else if (x is BigInteger x1)
    {
      obj = BigIntegerOps.Power(x1, y, z);
      if (obj != NotImplementedType.Value)
        return obj;
    }
    if (x is Complex || y is Complex || z is Complex)
      throw PythonOps.ValueError("complex modulo");
    if (PythonTypeOps.TryInvokeTernaryOperator(context, x, y, z, "__pow__", out obj))
    {
      if (obj != NotImplementedType.Value)
        return obj;
      if (!PythonOps.IsNumericObject(y) || !PythonOps.IsNumericObject(z))
        throw PythonOps.TypeError("pow() 3rd argument not allowed unless all arguments are integers");
    }
    throw PythonOps.TypeErrorForBinaryOp("power with modulus", x, y);
  }

  public static long Id(object o) => IdDispenser.GetId(o);

  public static string HexId(object o) => $"0x{PythonOps.Id(o):X16}";

  public static int Hash(CodeContext context, object o) => PythonContext.Hash(o);

  public static object Hex(object o)
  {
    if (o is int x1)
      return (object) Int32Ops.__hex__(x1);
    if (o is BigInteger x2)
      return (object) BigIntegerOps.__hex__(x2);
    object o1;
    if (!PythonTypeOps.TryInvokeUnaryOperator(DefaultContext.Default, o, "__hex__", out o1))
      throw PythonOps.TypeError("hex() argument cannot be converted to hex");
    switch (o1)
    {
      case string _:
      case ExtensibleString _:
        return o1;
      default:
        throw PythonOps.TypeError("hex expected string type as return, got '{0}'", (object) PythonTypeOps.GetName(o1));
    }
  }

  public static object Oct(object o)
  {
    if (o is int x1)
      return (object) Int32Ops.__oct__(x1);
    if (o is BigInteger x2)
      return (object) BigIntegerOps.__oct__(x2);
    object o1;
    if (!PythonTypeOps.TryInvokeUnaryOperator(DefaultContext.Default, o, "__oct__", out o1))
      throw PythonOps.TypeError("oct() argument cannot be converted to octal");
    switch (o1)
    {
      case string _:
      case ExtensibleString _:
        return o1;
      default:
        throw PythonOps.TypeError("hex expected string type as return, got '{0}'", (object) PythonTypeOps.GetName(o1));
    }
  }

  public static object Index(object o)
  {
    switch (o)
    {
      case int x1:
        return (object) Int32Ops.__index__(x1);
      case uint x2:
        return (object) UInt32Ops.__index__(x2);
      case ushort x3:
        return (object) UInt16Ops.__index__(x3);
      case short x4:
        return (object) Int16Ops.__index__(x4);
      case byte x5:
        return (object) ByteOps.__index__(x5);
      case sbyte x6:
        return (object) SByteOps.__index__(x6);
      case long x7:
        return (object) Int64Ops.__index__(x7);
      case ulong x8:
        return (object) UInt64Ops.__index__(x8);
      case BigInteger self:
        return (object) BigIntegerOps.__index__(self);
      default:
        object o1;
        if (PythonTypeOps.TryInvokeUnaryOperator(DefaultContext.Default, o, "__index__", out o1))
        {
          switch (o1)
          {
            case int _:
            case double _:
              return o1;
            default:
              throw PythonOps.TypeError("__index__ returned non-(int,long) (type {0})", (object) PythonTypeOps.GetName(o1));
          }
        }
        else
          throw PythonOps.TypeError("'{0}' object cannot be interpreted as an index", (object) PythonTypeOps.GetName(o));
    }
  }

  public static int Length(object o)
  {
    switch (o)
    {
      case string str:
        return str.Length;
      case object[] objArray:
        return objArray.Length;
      default:
        object obj = PythonContext.InvokeUnaryOperator(DefaultContext.Default, UnaryOperators.Length, o, $"object of type '{PythonOps.GetPythonTypeName(o)}' has no len()");
        if (!(obj is int num))
          num = Converter.ConvertToInt32(obj);
        return num >= 0 ? num : throw PythonOps.ValueError("__len__ should return >= 0, got {0}", (object) num);
    }
  }

  public static object CallWithContext(CodeContext context, object func, params object[] args)
  {
    return PythonCalls.Call(context, func, args);
  }

  public static object CallWithContextAndThis(
    CodeContext context,
    object func,
    object instance,
    params object[] args)
  {
    return PythonOps.CallWithContext(context, func, args);
  }

  public static object ToPythonType(PythonType dt)
  {
    return dt != null ? (object) dt.OldClass ?? (object) dt : (object) null;
  }

  public static object CallWithArgsTupleAndContext(
    CodeContext context,
    object func,
    object[] args,
    object argsTuple)
  {
    if (argsTuple is PythonTuple pythonTuple)
    {
      object[] objArray = new object[args.Length + pythonTuple.__len__()];
      for (int index = 0; index < args.Length; ++index)
        objArray[index] = args[index];
      for (int index = 0; index < pythonTuple.__len__(); ++index)
        objArray[index + args.Length] = pythonTuple[index];
      return PythonOps.CallWithContext(context, func, objArray);
    }
    List list = PythonOps.MakeEmptyList(args.Length + 10);
    list.AddRange<object>((ICollection<object>) args);
    IEnumerator enumerator = PythonOps.GetEnumerator(argsTuple);
    while (enumerator.MoveNext())
      list.AddNoLock(enumerator.Current);
    return PythonOps.CallWithContext(context, func, list.GetObjectArray());
  }

  [Obsolete("Use ObjectOpertaions instead")]
  public static object CallWithArgsTupleAndKeywordDictAndContext(
    CodeContext context,
    object func,
    object[] args,
    string[] names,
    object argsTuple,
    object kwDict)
  {
    switch (kwDict)
    {
      case IDictionary dictionary:
      case null:
        if ((dictionary == null || dictionary.Count == 0) && names.Length == 0)
        {
          List<object> objectList = new List<object>((IEnumerable<object>) args);
          if (argsTuple != null)
          {
            foreach (object obj in (IEnumerable) PythonOps.GetCollection(argsTuple))
              objectList.Add(obj);
          }
          return PythonOps.CallWithContext(context, func, objectList.ToArray());
        }
        List<object> objectList1;
        if (argsTuple != null && args.Length == names.Length)
        {
          if (!(argsTuple is PythonTuple collection))
            collection = new PythonTuple(argsTuple);
          objectList1 = new List<object>((IEnumerable<object>) collection);
          objectList1.AddRange((IEnumerable<object>) args);
        }
        else
        {
          objectList1 = new List<object>((IEnumerable<object>) args);
          if (argsTuple != null)
            objectList1.InsertRange(args.Length - names.Length, (IEnumerable<object>) PythonTuple.Make(argsTuple));
        }
        List<string> stringList = new List<string>((IEnumerable<string>) names);
        if (dictionary != null)
        {
          IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
          while (enumerator.MoveNext())
          {
            stringList.Add((string) enumerator.Key);
            objectList1.Add(enumerator.Value);
          }
        }
        return PythonCalls.CallWithKeywordArgs(context, func, objectList1.ToArray(), stringList.ToArray());
      default:
        throw PythonOps.TypeError("argument after ** must be a dictionary");
    }
  }

  public static object CallWithKeywordArgs(
    CodeContext context,
    object func,
    object[] args,
    string[] names)
  {
    return PythonCalls.CallWithKeywordArgs(context, func, args, names);
  }

  public static object CallWithArgsTuple(object func, object[] args, object argsTuple)
  {
    if (argsTuple is PythonTuple pythonTuple)
    {
      object[] objArray = new object[args.Length + pythonTuple.__len__()];
      for (int index = 0; index < args.Length; ++index)
        objArray[index] = args[index];
      for (int index = 0; index < pythonTuple.__len__(); ++index)
        objArray[index + args.Length] = pythonTuple[index];
      return PythonCalls.Call(func, objArray);
    }
    List list = PythonOps.MakeEmptyList(args.Length + 10);
    list.AddRange<object>((ICollection<object>) args);
    IEnumerator enumerator = PythonOps.GetEnumerator(argsTuple);
    while (enumerator.MoveNext())
      list.AddNoLock(enumerator.Current);
    return PythonCalls.Call(func, list.GetObjectArray());
  }

  public static object GetIndex(CodeContext context, object o, object index)
  {
    PythonContext languageContext = context.LanguageContext;
    return languageContext.GetIndexSite.Target((CallSite) languageContext.GetIndexSite, o, index);
  }

  public static bool TryGetBoundAttr(object o, string name, out object ret)
  {
    return PythonOps.TryGetBoundAttr(DefaultContext.Default, o, name, out ret);
  }

  public static void SetAttr(CodeContext context, object o, string name, object value)
  {
    context.LanguageContext.SetAttr(context, o, name, value);
  }

  public static bool TryGetBoundAttr(CodeContext context, object o, string name, out object ret)
  {
    return DynamicHelpers.GetPythonType(o).TryGetBoundAttr(context, o, name, out ret);
  }

  public static void DeleteAttr(CodeContext context, object o, string name)
  {
    context.LanguageContext.DeleteAttr(context, o, name);
  }

  public static bool HasAttr(CodeContext context, object o, string name)
  {
    object ret;
    if (context.LanguageContext.PythonOptions.Python30)
      return PythonOps.TryGetBoundAttr(context, o, name, out ret);
    try
    {
      return PythonOps.TryGetBoundAttr(context, o, name, out ret);
    }
    catch (SystemExitException ex)
    {
      throw;
    }
    catch (KeyboardInterruptException ex)
    {
      throw;
    }
    catch
    {
      return false;
    }
  }

  public static object GetBoundAttr(CodeContext context, object o, string name)
  {
    object ret;
    if (DynamicHelpers.GetPythonType(o).TryGetBoundAttr(context, o, name, out ret))
      return ret;
    if (o is OldClass)
      throw PythonOps.AttributeError("type object '{0}' has no attribute '{1}'", (object) ((OldClass) o).Name, (object) name);
    throw PythonOps.AttributeError("'{0}' object has no attribute '{1}'", (object) PythonTypeOps.GetName(o), (object) name);
  }

  public static void ObjectSetAttribute(CodeContext context, object o, string name, object value)
  {
    if (!DynamicHelpers.GetPythonType(o).TrySetNonCustomMember(context, o, name, value))
      throw PythonOps.AttributeErrorForMissingOrReadonly(context, DynamicHelpers.GetPythonType(o), name);
  }

  public static void ObjectDeleteAttribute(CodeContext context, object o, string name)
  {
    if (!DynamicHelpers.GetPythonType(o).TryDeleteNonCustomMember(context, o, name))
      throw PythonOps.AttributeErrorForMissingOrReadonly(context, DynamicHelpers.GetPythonType(o), name);
  }

  public static object ObjectGetAttribute(CodeContext context, object o, string name)
  {
    if (o is OldClass oldClass)
      return oldClass.GetMember(context, name);
    object attribute;
    if (DynamicHelpers.GetPythonType(o).TryGetNonCustomMember(context, o, name, out attribute))
      return attribute;
    throw PythonOps.AttributeErrorForObjectMissingAttribute(o, name);
  }

  internal static IList<string> GetStringMemberList(IPythonMembersList pyMemList)
  {
    List<string> stringMemberList = new List<string>();
    foreach (object memberName in (IEnumerable<object>) pyMemList.GetMemberNames(DefaultContext.Default))
    {
      if (memberName is string)
        stringMemberList.Add((string) memberName);
    }
    return (IList<string>) stringMemberList;
  }

  public static IList<object> GetAttrNames(CodeContext context, object o)
  {
    switch (o)
    {
      case IPythonMembersList pythonMembersList:
        return pythonMembersList.GetMemberNames(context);
      case IMembersList membersList:
        return (IList<object>) new List((object) membersList.GetMemberNames());
      case IPythonObject pythonObject:
        return (IList<object>) pythonObject.PythonType.GetMemberNames(context, o);
      default:
        List memberNames = DynamicHelpers.GetPythonType(o).GetMemberNames(context, o);
        if (o != null && ComBinder.IsComObject(o))
        {
          foreach (string dynamicMemberName in ComBinder.GetDynamicMemberNames(o))
          {
            if (!memberNames.Contains((object) dynamicMemberName))
              memberNames.AddNoLock((object) dynamicMemberName);
          }
        }
        return (IList<object>) memberNames;
    }
  }

  public static void CheckInitializedAttribute(object o, object self, string name)
  {
    if (o == Uninitialized.Instance)
      throw PythonOps.AttributeError("'{0}' object has no attribute '{1}'", (object) PythonTypeOps.GetName(self), (object) name);
  }

  public static object GetUserSlotValue(
    CodeContext context,
    PythonTypeUserDescriptorSlot slot,
    object instance,
    PythonType type)
  {
    return slot.GetValue(context, instance, type);
  }

  public static object GetUserDescriptor(object o, object instance, object context)
  {
    object res;
    return o is IPythonObject && PythonContext.TryInvokeTernaryOperator(DefaultContext.Default, TernaryOperators.GetDescriptor, o, instance, context, out res) ? res : o;
  }

  public static bool TrySetUserDescriptor(object o, object instance, object value)
  {
    return (o == null || !(o.GetType() == typeof (OldInstance))) && PythonContext.TryInvokeTernaryOperator(DefaultContext.Default, TernaryOperators.SetDescriptor, o, instance, value, out object _);
  }

  public static bool TryDeleteUserDescriptor(object o, object instance)
  {
    return (o == null || !(o.GetType() == typeof (OldInstance))) && PythonTypeOps.TryInvokeBinaryOperator(DefaultContext.Default, o, instance, "__delete__", out object _);
  }

  public static object Invoke(
    CodeContext context,
    object target,
    string name,
    params object[] args)
  {
    return PythonCalls.Call(context, PythonOps.GetBoundAttr(context, target, name), args);
  }

  public static Delegate CreateDynamicDelegate(
    DynamicMethod meth,
    Type delegateType,
    object target)
  {
    return ((MethodInfo) meth).CreateDelegate(delegateType, target);
  }

  public static double CheckMath(double v)
  {
    if (double.IsInfinity(v))
      throw PythonOps.OverflowError("math range error");
    return !double.IsNaN(v) ? v : throw PythonOps.ValueError("math domain error");
  }

  public static double CheckMath(double input, double output)
  {
    return double.IsInfinity(input) && double.IsInfinity(output) || double.IsNaN(input) && double.IsNaN(output) ? output : PythonOps.CheckMath(output);
  }

  public static double CheckMath(double in0, double in1, double output)
  {
    return (double.IsInfinity(in0) || double.IsInfinity(in1)) && double.IsInfinity(output) || (double.IsNaN(in0) || double.IsNaN(in1)) && double.IsNaN(output) ? output : PythonOps.CheckMath(output);
  }

  public static object IsMappingType(CodeContext context, object o)
  {
    switch (o)
    {
      case IDictionary _:
      case PythonDictionary _:
      case IDictionary<object, object> _:
        return ScriptingRuntimeHelpers.True;
      case IPythonObject _:
      case OldInstance _:
        if (PythonOps.TryGetBoundAttr(context, o, "__getitem__", out object _) && (PythonOps.IsClsVisible(context) || !(o is BuiltinFunction)))
          return ScriptingRuntimeHelpers.True;
        break;
    }
    return ScriptingRuntimeHelpers.False;
  }

  public static int FixSliceIndex(int v, int len)
  {
    if (v < 0)
      v = len + v;
    if (v < 0)
      return 0;
    return v > len ? len : v;
  }

  public static long FixSliceIndex(long v, long len)
  {
    if (v < 0L)
      v = len + v;
    if (v < 0L)
      return 0;
    return v > len ? len : v;
  }

  public static void FixSlice(
    int length,
    object start,
    object stop,
    object step,
    out int ostart,
    out int ostop,
    out int ostep)
  {
    if (step == null)
    {
      ostep = 1;
    }
    else
    {
      ostep = Converter.ConvertToIndex(step);
      if (ostep == 0)
        throw PythonOps.ValueError("step cannot be zero");
    }
    if (start == null)
    {
      ostart = ostep > 0 ? 0 : length - 1;
    }
    else
    {
      ostart = Converter.ConvertToIndex(start);
      if (ostart < 0)
      {
        ostart += length;
        if (ostart < 0)
          ostart = ostep > 0 ? Math.Min(length, 0) : Math.Min(length - 1, -1);
      }
      else if (ostart >= length)
        ostart = ostep > 0 ? length : length - 1;
    }
    if (stop == null)
    {
      ostop = ostep > 0 ? length : -1;
    }
    else
    {
      ostop = Converter.ConvertToIndex(stop);
      if (ostop < 0)
      {
        ostop += length;
        if (ostop >= 0)
          return;
        ostop = ostep > 0 ? Math.Min(length, 0) : Math.Min(length - 1, -1);
      }
      else
      {
        if (ostop < length)
          return;
        ostop = ostep > 0 ? length : length - 1;
      }
    }
  }

  public static void FixSlice(
    long length,
    long? start,
    long? stop,
    long? step,
    out long ostart,
    out long ostop,
    out long ostep,
    out long ocount)
  {
    if (!step.HasValue)
    {
      ostep = 1L;
    }
    else
    {
      long? nullable = step;
      long num = 0;
      if (nullable.GetValueOrDefault() == num & nullable.HasValue)
        throw PythonOps.ValueError("step cannot be zero");
      ostep = step.Value;
    }
    if (!start.HasValue)
    {
      ostart = ostep > 0L ? 0L : length - 1L;
    }
    else
    {
      ostart = start.Value;
      if (ostart < 0L)
      {
        ostart += length;
        if (ostart < 0L)
          ostart = ostep > 0L ? Math.Min(length, 0L) : Math.Min(length - 1L, -1L);
      }
      else if (ostart >= length)
        ostart = ostep > 0L ? length : length - 1L;
    }
    if (!stop.HasValue)
    {
      ostop = ostep > 0L ? length : -1L;
    }
    else
    {
      ostop = stop.Value;
      if (ostop < 0L)
      {
        ostop += length;
        if (ostop < 0L)
          ostop = ostep > 0L ? Math.Min(length, 0L) : Math.Min(length - 1L, -1L);
      }
      else if (ostop >= length)
        ostop = ostep > 0L ? length : length - 1L;
    }
    ocount = Math.Max(0L, ostep > 0L ? (ostop - ostart + ostep - 1L) / ostep : (ostop - ostart + ostep + 1L) / ostep);
  }

  public static int FixIndex(int v, int len)
  {
    if (v < 0)
    {
      v += len;
      if (v < 0)
        throw PythonOps.IndexError("index out of range: {0}", (object) (v - len));
    }
    else if (v >= len)
      throw PythonOps.IndexError("index out of range: {0}", (object) v);
    return v;
  }

  public static void InitializeForFinalization(CodeContext context, object newObject)
  {
    IWeakReferenceable weakReferenceable = context.LanguageContext.ConvertToWeakReferenceable(newObject);
    InstanceFinalizer instanceFinalizer = new InstanceFinalizer(context, newObject);
    weakReferenceable.SetFinalizer(new WeakRefTracker(weakReferenceable, (object) instanceFinalizer, (object) instanceFinalizer));
  }

  private static object FindMetaclass(
    CodeContext context,
    PythonTuple bases,
    PythonDictionary dict)
  {
    object res;
    if (dict.TryGetValue((object) "__metaclass__", out res) && res != null)
      return res;
    for (int index = 0; index < bases.__len__(); ++index)
    {
      if (!(bases[index] is OldClass))
        return (object) DynamicHelpers.GetPythonType(bases[index]);
    }
    return context.TryGetGlobalVariable("__metaclass__", out res) && res != null ? res : (object) TypeCache.OldInstance;
  }

  public static object MakeClass(
    FunctionCode funcCode,
    Func<CodeContext, CodeContext> body,
    CodeContext parentContext,
    string name,
    object[] bases,
    string selfNames)
  {
    Func<CodeContext, CodeContext> classCode = PythonOps.GetClassCode(parentContext, funcCode, body);
    return PythonOps.MakeClass(parentContext, name, bases, selfNames, classCode(parentContext).Dict);
  }

  private static Func<CodeContext, CodeContext> GetClassCode(
    CodeContext context,
    FunctionCode funcCode,
    Func<CodeContext, CodeContext> body)
  {
    if (body == null)
    {
      if ((object) funcCode.Target == null)
        funcCode.UpdateDelegate(context.LanguageContext, true);
      return (Func<CodeContext, CodeContext>) funcCode.Target;
    }
    if ((object) funcCode.Target == null)
    {
      funcCode.SetTarget((Delegate) body);
      funcCode._normalDelegate = (Delegate) body;
    }
    return body;
  }

  internal static object MakeClass(
    CodeContext context,
    string name,
    object[] bases,
    string selfNames,
    PythonDictionary vars)
  {
    foreach (object obj in bases)
    {
      switch (obj)
      {
        case TypeGroup _:
          object[] objArray = new object[bases.Length];
          for (int index = 0; index < bases.Length; ++index)
          {
            if (bases[index] is TypeGroup o)
            {
              Type nonGenericType;
              if (!o.TryGetNonGenericType(out nonGenericType))
                throw PythonOps.TypeError("cannot derive from open generic types " + Builtin.repr(context, (object) o).ToString());
              objArray[index] = (object) DynamicHelpers.GetPythonTypeFromType(nonGenericType);
            }
            else
              objArray[index] = bases[index];
          }
          bases = objArray;
          goto label_15;
        case PythonType _:
          PythonType pythonType = obj as PythonType;
          if (pythonType.Equals((object) PythonType.GetPythonType(typeof (Enum))) || pythonType.Equals((object) PythonType.GetPythonType(typeof (Array))) || pythonType.Equals((object) PythonType.GetPythonType(typeof (Delegate))) || pythonType.Equals((object) PythonType.GetPythonType(typeof (ValueType))))
            throw PythonOps.TypeError("cannot derive from special class '{0}'", (object) pythonType.FinalSystemType.FullName);
          break;
      }
    }
label_15:
    PythonTuple bases1 = PythonTuple.MakeTuple(bases);
    object metaclass = PythonOps.FindMetaclass(context, bases1, vars);
    if (metaclass == TypeCache.OldInstance)
      return (object) new OldClass(name, bases1, vars, selfNames);
    if (metaclass == TypeCache.PythonType)
      return PythonType.__new__(context, TypeCache.PythonType, name, bases1, vars, selfNames);
    PythonContext languageContext = context.LanguageContext;
    return languageContext.MetaClassCallSite.Target((CallSite) languageContext.MetaClassCallSite, context, metaclass, name, bases1, vars);
  }

  public static void RaiseAssertionError(CodeContext context)
  {
    object o;
    if (!(context.GetBuiltinsDict() ?? context.LanguageContext.BuiltinModuleDict)._storage.TryGetValue((object) "AssertionError", out o))
      throw PythonOps.AssertionError("");
    if (o is PythonType type)
      throw PythonOps.CreateThrowable(type);
    throw PythonOps.CreateThrowable(DynamicHelpers.GetPythonType(o));
  }

  public static void RaiseAssertionError(CodeContext context, object msg)
  {
    object o;
    if ((context.GetBuiltinsDict() ?? context.LanguageContext.BuiltinModuleDict)._storage.TryGetValue((object) "AssertionError", out o))
    {
      if (o is PythonType type)
      {
        object[] objArray = new object[1]{ msg };
        throw PythonOps.CreateThrowable(type, objArray);
      }
      throw PythonOps.CreateThrowable(DynamicHelpers.GetPythonType(o), msg);
    }
    throw PythonOps.AssertionError("{0}", msg);
  }

  public static List MakeList() => new List();

  public static List MakeList(params object[] items) => new List((ICollection) items);

  [NoSideEffects]
  public static List MakeListNoCopy(params object[] items) => List.FromArrayNoCopy(items);

  public static List MakeListFromSequence(object sequence) => new List(sequence);

  [NoSideEffects]
  public static List MakeEmptyList(int capacity) => new List(capacity);

  [NoSideEffects]
  public static List MakeEmptyListFromCode() => List.FromArrayNoCopy(ArrayUtils.EmptyObjects);

  [NoSideEffects]
  public static PythonTuple MakeTuple(params object[] items) => PythonTuple.MakeTuple(items);

  [NoSideEffects]
  public static PythonTuple MakeTupleFromSequence(object items) => PythonTuple.Make(items);

  [LightThrowing]
  public static object GetEnumeratorValues(CodeContext context, object e, int expected)
  {
    if (e != null && e.GetType() == typeof (PythonTuple))
      return PythonOps.GetEnumeratorValuesFromTuple((PythonTuple) e, expected);
    IEnumerator enumeratorForUnpack = PythonOps.GetEnumeratorForUnpack(context, e);
    int right = 0;
    object[] objArray = new object[expected];
    for (; right < expected; ++right)
    {
      if (!enumeratorForUnpack.MoveNext())
        return LightExceptions.Throw(PythonOps.ValueErrorForUnpackMismatch(expected, right));
      objArray[right] = enumeratorForUnpack.Current;
    }
    return enumeratorForUnpack.MoveNext() ? LightExceptions.Throw(PythonOps.ValueErrorForUnpackMismatch(expected, right + 1)) : (object) objArray;
  }

  [LightThrowing]
  public static object GetEnumeratorValuesNoComplexSets(
    CodeContext context,
    object e,
    int expected)
  {
    return e != null && e.GetType() == typeof (List) ? (object) PythonOps.GetEnumeratorValuesFromList((List) e, expected) : PythonOps.GetEnumeratorValues(context, e, expected);
  }

  [LightThrowing]
  private static object GetEnumeratorValuesFromTuple(PythonTuple pythonTuple, int expected)
  {
    return pythonTuple.Count == expected ? (object) pythonTuple._data : LightExceptions.Throw(PythonOps.ValueErrorForUnpackMismatch(expected, pythonTuple.Count));
  }

  private static object[] GetEnumeratorValuesFromList(List list, int expected)
  {
    if (list._size == expected)
      return list._data;
    throw PythonOps.ValueErrorForUnpackMismatch(expected, list._size);
  }

  public static Slice MakeSlice(object start, object stop, object step)
  {
    return new Slice(start, stop, step);
  }

  public static void Write(CodeContext context, object f, string text)
  {
    PythonContext languageContext = context.LanguageContext;
    if (f == null)
      f = languageContext.SystemStandardOut;
    if (f == null || f == Uninitialized.Instance)
      throw PythonOps.RuntimeError("lost sys.stdout");
    if (f is PythonFile pythonFile)
    {
      pythonFile.write(text);
    }
    else
    {
      object obj = languageContext.WriteCallSite.Target((CallSite) languageContext.WriteCallSite, context, PythonOps.GetBoundAttr(context, f, "write"), text);
    }
  }

  private static object ReadLine(CodeContext context, object f)
  {
    return f != null && f != Uninitialized.Instance ? PythonOps.Invoke(context, f, "readline") : throw PythonOps.RuntimeError("lost sys.std_in");
  }

  public static void WriteSoftspace(CodeContext context, object f)
  {
    if (!PythonOps.CheckSoftspace(f))
      return;
    PythonOps.SetSoftspace(f, ScriptingRuntimeHelpers.False);
    PythonOps.Write(context, f, " ");
  }

  public static void SetSoftspace(object f, object value)
  {
    PythonOps.SetAttr(DefaultContext.Default, f, "softspace", value);
  }

  public static bool CheckSoftspace(object f)
  {
    if (f is PythonFile pythonFile)
      return pythonFile.softspace;
    object ret;
    return PythonOps.TryGetBoundAttr(f, "softspace", out ret) && PythonOps.IsTrue(ret);
  }

  public static void Print(CodeContext context, object o)
  {
    PythonOps.PrintWithDest(context, context.LanguageContext.SystemStandardOut, o);
  }

  public static void PrintNoNewline(CodeContext context, object o)
  {
    PythonOps.PrintWithDestNoNewline(context, context.LanguageContext.SystemStandardOut, o);
  }

  public static void PrintWithDest(CodeContext context, object dest, object o)
  {
    PythonOps.PrintWithDestNoNewline(context, dest, o);
    PythonOps.Write(context, dest, "\n");
  }

  public static void PrintWithDestNoNewline(CodeContext context, object dest, object o)
  {
    PythonOps.WriteSoftspace(context, dest);
    PythonOps.Write(context, dest, o == null ? "None" : PythonOps.ToString(o));
  }

  public static object ReadLineFromSrc(CodeContext context, object src)
  {
    return PythonOps.ReadLine(context, src);
  }

  public static void PrintNewline(CodeContext context)
  {
    PythonOps.PrintNewlineWithDest(context, context.LanguageContext.SystemStandardOut);
  }

  public static void PrintNewlineWithDest(CodeContext context, object dest)
  {
    PythonOps.Write(context, dest, "\n");
    PythonOps.SetSoftspace(dest, ScriptingRuntimeHelpers.False);
  }

  public static void PrintComma(CodeContext context, object o)
  {
    PythonOps.PrintCommaWithDest(context, context.LanguageContext.SystemStandardOut, o);
  }

  public static void PrintCommaWithDest(CodeContext context, object dest, object o)
  {
    PythonOps.WriteSoftspace(context, dest);
    string text = o == null ? "None" : PythonOps.ToString(o);
    PythonOps.Write(context, dest, text);
    PythonOps.SetSoftspace(dest, (object) (bool) (text.EndsWith("\n") ? 0 : (!text.EndsWith("\t") ? 1 : 0)));
  }

  public static void PrintExpressionValue(CodeContext context, object value)
  {
    PythonContext languageContext = context.LanguageContext;
    object systemStateValue = languageContext.GetSystemStateValue("displayhook");
    languageContext.CallWithContext(context, systemStateValue, value);
  }

  public static void PrintException(CodeContext context, Exception exception, IConsole console)
  {
    PythonContext languageContext = context.LanguageContext;
    PythonTuple exceptionInfoLocal = PythonOps.GetExceptionInfoLocal(context, exception);
    languageContext.SetSystemStateValue("last_type", exceptionInfoLocal[0]);
    languageContext.SetSystemStateValue("last_value", exceptionInfoLocal[1]);
    languageContext.SetSystemStateValue("last_traceback", exceptionInfoLocal[2]);
    object systemStateValue = languageContext.GetSystemStateValue("excepthook");
    BuiltinFunction builtinFunction = systemStateValue as BuiltinFunction;
    if (console != null && builtinFunction != null && builtinFunction.DeclaringType == typeof (SysModule) && builtinFunction.Name == "excepthook")
    {
      console.WriteLine(languageContext.FormatException(exception), Style.Error);
    }
    else
    {
      try
      {
        PythonCalls.Call(context, systemStateValue, exceptionInfoLocal[0], exceptionInfoLocal[1], exceptionInfoLocal[2]);
      }
      catch (Exception ex)
      {
        PythonOps.PrintWithDest(context, languageContext.SystemStandardError, (object) "Error in sys.excepthook:");
        PythonOps.PrintWithDest(context, languageContext.SystemStandardError, (object) languageContext.FormatException(ex));
        PythonOps.PrintNewlineWithDest(context, languageContext.SystemStandardError);
        PythonOps.PrintWithDest(context, languageContext.SystemStandardError, (object) "Original exception was:");
        PythonOps.PrintWithDest(context, languageContext.SystemStandardError, (object) languageContext.FormatException(exception));
      }
    }
  }

  [ProfilerTreatsAsExternal]
  [LightThrowing]
  public static object ImportTop(CodeContext context, string fullName, int level)
  {
    return Importer.ImportLightThrow(context, fullName, (PythonTuple) null, level);
  }

  [ProfilerTreatsAsExternal]
  [LightThrowing]
  public static object ImportBottom(CodeContext context, string fullName, int level)
  {
    object o = Importer.ImportLightThrow(context, fullName, (PythonTuple) null, level);
    if (!LightExceptions.IsLightException(o) && fullName.IndexOf('.') >= 0)
    {
      string[] strArray = fullName.Split('.');
      for (int index = 1; index < strArray.Length; ++index)
        o = PythonOps.GetBoundAttr(context, o, strArray[index]);
    }
    return o;
  }

  [ProfilerTreatsAsExternal]
  [LightThrowing]
  public static object ImportWithNames(
    CodeContext context,
    string fullName,
    string[] names,
    int level)
  {
    return Importer.ImportLightThrow(context, fullName, PythonTuple.MakeTuple((object[]) names), level);
  }

  public static object ImportFrom(CodeContext context, object module, string name)
  {
    return Importer.ImportFrom(context, module, name);
  }

  [ProfilerTreatsAsExternal]
  public static void ImportStar(CodeContext context, string fullName, int level)
  {
    object o = Importer.Import(context, fullName, PythonTuple.MakeTuple((object) "*"), level);
    PythonModule pythonModule = o as PythonModule;
    NamespaceTracker self = o as NamespaceTracker;
    if (o is PythonType owner && !owner.UnderlyingSystemType.IsEnum() && (!owner.UnderlyingSystemType.IsAbstract() || !owner.UnderlyingSystemType.IsSealed()))
      throw PythonOps.ImportError("no module named {0}", (object) owner.Name);
    bool flag = false;
    object ret;
    IEnumerator enumerator;
    if (PythonOps.TryGetBoundAttr(context, o, "__all__", out ret))
    {
      enumerator = PythonOps.GetEnumerator(ret);
    }
    else
    {
      enumerator = (IEnumerator) PythonOps.GetAttrNames(context, o).GetEnumerator();
      flag = true;
    }
    while (enumerator.MoveNext())
    {
      if (!(enumerator.Current is string current))
        throw PythonOps.TypeErrorForNonStringAttribute();
      if (!flag || current.Length <= 0 || current[0] != '_')
      {
        if (pythonModule != null)
          context.SetVariable(current, pythonModule.__dict__[(object) current]);
        else if (self != null)
        {
          object obj = NamespaceTrackerOps.GetCustomMember(context, self, current);
          if (obj != OperationFailed.Value)
            context.SetVariable(current, obj);
        }
        else if (owner != null)
        {
          PythonTypeSlot slot;
          object obj;
          if (owner.TryResolveSlot(context, current, out slot) && slot.TryGetValue(context, (object) null, owner, out obj))
            context.SetVariable(current, obj);
        }
        else
          context.SetVariable(current, PythonOps.GetBoundAttr(context, o, current));
      }
    }
  }

  [ProfilerTreatsAsExternal]
  public static void UnqualifiedExec(CodeContext context, object code)
  {
    locals = (PythonDictionary) null;
    globals = (PythonDictionary) null;
    if (code is PythonTuple pythonTuple && pythonTuple.__len__() > 0 && pythonTuple.__len__() <= 3)
    {
      code = pythonTuple[0];
      if (pythonTuple.__len__() > 1 && pythonTuple[1] != null && !(pythonTuple[1] is PythonDictionary globals))
        throw PythonOps.TypeError("globals must be dictionary or none");
      if (pythonTuple.__len__() > 2 && pythonTuple[2] != null)
      {
        if (!(pythonTuple[2] is PythonDictionary locals))
          throw PythonOps.TypeError("locals must be dictionary or none");
      }
      else
        locals = globals;
    }
    PythonOps.QualifiedExec(context, code, globals, (object) locals);
  }

  [ProfilerTreatsAsExternal]
  public static void QualifiedExec(
    CodeContext context,
    object code,
    PythonDictionary globals,
    object locals)
  {
    PythonContext languageContext = context.LanguageContext;
    bool flag = true;
    if (code is PythonFile pythonFile)
    {
      List list = pythonFile.readlines();
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < list.__len__(); ++index)
        stringBuilder.Append(list[index]);
      code = (object) stringBuilder.ToString();
    }
    else if (code is Stream stream)
    {
      using (StreamReader streamReader = new StreamReader(stream))
        code = (object) streamReader.ReadToEnd();
      flag = false;
    }
    if (code is string code1)
      code = (object) FunctionCode.FromSourceUnit(!flag ? languageContext.CreateSnippet(code1, SourceCodeKind.Statements) : languageContext.CreateSourceUnit((TextContentProvider) new NoLineFeedSourceContentProvider(code1), "<string>", SourceCodeKind.Statements), Builtin.GetRuntimeGeneratedCodeCompilerOptions(context, true, (CompileFlags) 0), false);
    if (!(code is FunctionCode function))
      throw PythonOps.TypeError("exec: arg 1 must be a string, file, Stream, or code object");
    if (locals == null)
      locals = (object) globals;
    if (globals == null)
      globals = context.GlobalDict;
    if (locals != null && PythonOps.IsMappingType(context, locals) != ScriptingRuntimeHelpers.True)
      throw PythonOps.TypeError("exec: arg 3 must be mapping or None");
    CodeContext execEvalScope = Builtin.GetExecEvalScope(context, globals, Builtin.GetAttrLocals(context, locals), true, false);
    if (context.LanguageContext.PythonOptions.Frames)
    {
      List<FunctionStack> functionStackList = PythonOps.PushFrame(execEvalScope, function);
      try
      {
        function.Call(execEvalScope);
      }
      finally
      {
        functionStackList.RemoveAt(functionStackList.Count - 1);
      }
    }
    else
      function.Call(execEvalScope);
  }

  public static ICollection GetCollection(object o)
  {
    if (o is ICollection collection1)
      return collection1;
    List<object> collection2 = new List<object>();
    IEnumerator enumerator = PythonOps.GetEnumerator(o);
    while (enumerator.MoveNext())
      collection2.Add(enumerator.Current);
    return (ICollection) collection2;
  }

  public static IEnumerator GetEnumerator(object o)
  {
    return PythonOps.GetEnumerator(DefaultContext.Default, o);
  }

  public static IEnumerator GetEnumerator(CodeContext context, object o)
  {
    IEnumerator enumerator;
    if (!PythonOps.TryGetEnumerator(context, o, out enumerator))
      throw PythonOps.TypeError("{0} is not iterable", (object) PythonTypeOps.GetName(o));
    return enumerator;
  }

  public static object GetEnumeratorObject(CodeContext context, object o)
  {
    object ret;
    return PythonOps.TryGetBoundAttr(context, o, "__iter__", out ret) && ret != NotImplementedType.Value ? PythonOps.CallWithContext(context, ret) : (object) PythonOps.GetEnumerator(context, o);
  }

  public static IEnumerator GetEnumeratorForUnpack(CodeContext context, object enumerable)
  {
    IEnumerator enumerator;
    if (!PythonOps.TryGetEnumerator(context, enumerable, out enumerator))
      throw PythonOps.TypeErrorForNotIterable(enumerable);
    return enumerator;
  }

  public static Exception TypeErrorForNotIterable(object enumerable)
  {
    return PythonOps.TypeError("'{0}' object is not iterable", (object) PythonTypeOps.GetName(enumerable));
  }

  public static KeyValuePair<IEnumerator, IDisposable> ThrowTypeErrorForBadIteration(
    CodeContext context,
    object enumerable)
  {
    throw PythonOps.TypeError("iteration over non-sequence of type {0}", (object) PythonTypeOps.GetName(enumerable));
  }

  internal static bool TryGetEnumerator(
    CodeContext context,
    object enumerable,
    out IEnumerator enumerator)
  {
    enumerator = (IEnumerator) null;
    IEnumerable res;
    if (enumerable is PythonType && !((PythonType) enumerable).IsIterable(context) || !context.LanguageContext.TryConvertToIEnumerable(enumerable, out res))
      return false;
    enumerator = res.GetEnumerator();
    return true;
  }

  public static void ForLoopDispose(
    KeyValuePair<IEnumerator, IDisposable> iteratorInfo)
  {
    if (iteratorInfo.Value == null)
      return;
    iteratorInfo.Value.Dispose();
  }

  public static KeyValuePair<IEnumerator, IDisposable> StringEnumerator(string str)
  {
    return new KeyValuePair<IEnumerator, IDisposable>((IEnumerator) StringOps.StringEnumerator(str), (IDisposable) null);
  }

  public static KeyValuePair<IEnumerator, IDisposable> BytesEnumerator(IList<byte> bytes)
  {
    return new KeyValuePair<IEnumerator, IDisposable>((IEnumerator) IListOfByteOps.BytesEnumerator(bytes), (IDisposable) null);
  }

  public static KeyValuePair<IEnumerator, IDisposable> BytesIntEnumerator(IList<byte> bytes)
  {
    return new KeyValuePair<IEnumerator, IDisposable>((IEnumerator) IListOfByteOps.BytesIntEnumerator(bytes), (IDisposable) null);
  }

  public static KeyValuePair<IEnumerator, IDisposable> GetEnumeratorFromEnumerable(
    IEnumerable enumerable)
  {
    IEnumerator enumerator = enumerable.GetEnumerator();
    return new KeyValuePair<IEnumerator, IDisposable>(enumerator, enumerator as IDisposable);
  }

  public static IEnumerable StringEnumerable(string str) => StringOps.StringEnumerable(str);

  public static IEnumerable BytesEnumerable(IList<byte> bytes)
  {
    return IListOfByteOps.BytesEnumerable(bytes);
  }

  public static IEnumerable BytesIntEnumerable(IList<byte> bytes)
  {
    return IListOfByteOps.BytesIntEnumerable(bytes);
  }

  public static object SetCurrentException(CodeContext context, Exception clrException)
  {
    object python = PythonExceptions.ToPython(clrException);
    if (clrException is ThreadAbortException threadAbortException && threadAbortException.ExceptionState is KeyboardInterruptException)
      Thread.ResetAbort();
    PythonOps.RawException = clrException;
    return python;
  }

  public static void BuildExceptionInfo(CodeContext context, Exception clrException)
  {
    object python = PythonExceptions.ToPython(clrException);
    List<DynamicStackFrame> frameList = clrException.GetFrameList();
    object type = !(python is IPythonObject pythonObject) ? PythonOps.GetBoundAttr(context, python, "__class__") : (object) pythonObject.PythonType;
    context.LanguageContext.UpdateExceptionInfo(clrException, type, python, frameList);
  }

  public static void ClearCurrentException() => PythonOps.RestoreCurrentException((Exception) null);

  public static void ExceptionHandled(CodeContext context)
  {
    context.LanguageContext.ExceptionHandled();
  }

  public static Exception SaveCurrentException() => PythonOps.RawException;

  public static void RestoreCurrentException(Exception clrException)
  {
    PythonOps.RawException = clrException;
  }

  public static object CheckException(CodeContext context, object exception, object test)
  {
    if (exception is ObjectException objectException)
      return PythonOps.IsSubClass(context, objectException.Type, test) ? objectException.Instance : (object) null;
    switch (test)
    {
      case PythonType _:
        if (PythonOps.IsSubClass(test as PythonType, TypeCache.BaseException))
        {
          if (PythonOps.IsInstance(context, exception, test))
            return exception;
          break;
        }
        if (PythonOps.IsSubClass(test as PythonType, DynamicHelpers.GetPythonTypeFromType(typeof (Exception))))
        {
          Exception clr = PythonExceptions.ToClr(exception);
          if (PythonOps.IsInstance(context, (object) clr, test))
            return (object) clr;
          break;
        }
        break;
      case PythonTuple _:
        PythonTuple pythonTuple = test as PythonTuple;
        for (int index = 0; index < pythonTuple.__len__(); ++index)
        {
          object obj = PythonOps.CheckException(context, exception, pythonTuple[index]);
          if (obj != null)
            return obj;
        }
        break;
      case OldClass _:
        if (PythonOps.IsInstance(context, exception, test))
          return exception;
        break;
    }
    return (object) null;
  }

  private static TraceBack CreateTraceBack(PythonContext pyContext, Exception e)
  {
    TraceBack traceBack = e.GetTraceBack();
    if (traceBack != null)
      return traceBack;
    IList<DynamicStackFrame> frames = (IList<DynamicStackFrame>) ((object) e.GetFrameList() ?? (object) new DynamicStackFrame[0]);
    List<FunctionStack> functionStackNoCreate = PythonOps.GetFunctionStackNoCreate();
    return PythonOps.CreateTraceBack(e, frames, (IList<FunctionStack>) functionStackNoCreate, frames.Count);
  }

  internal static TraceBack CreateTraceBack(
    Exception e,
    IList<DynamicStackFrame> frames,
    IList<FunctionStack> stacks,
    int frameCount)
  {
    TraceBackFrame traceBackFrame = (TraceBackFrame) null;
    if (stacks != null)
    {
      foreach (FunctionStack stack in (IEnumerable<FunctionStack>) stacks)
        traceBackFrame = stack.Frame ?? new TraceBackFrame(stack.Context, stack.Context.GlobalDict, (object) stack.Context.Dict, stack.Code, traceBackFrame);
    }
    bool flag = true;
    foreach (DynamicStackFrame dynamicStackFrame1 in frames.Take<DynamicStackFrame>(frameCount).Reverse<DynamicStackFrame>())
    {
      if (dynamicStackFrame1 is PythonDynamicStackFrame dynamicStackFrame2 && dynamicStackFrame2.CodeContext != null && (!flag || traceBackFrame?.f_code != dynamicStackFrame2.Code))
      {
        flag = false;
        CodeContext codeContext = dynamicStackFrame2.CodeContext;
        traceBackFrame = new TraceBackFrame(codeContext, codeContext.GlobalDict, (object) codeContext.Dict, dynamicStackFrame2.Code, traceBackFrame);
      }
    }
    TraceBack traceBack = (TraceBack) null;
    foreach (DynamicStackFrame dynamicStackFrame3 in frames.Take<DynamicStackFrame>(frameCount))
    {
      if (dynamicStackFrame3 is PythonDynamicStackFrame dynamicStackFrame4 && dynamicStackFrame4.CodeContext != null)
      {
        traceBack = new TraceBack(traceBack, traceBackFrame);
        traceBack.SetLine(dynamicStackFrame3.GetFileLineNumber());
        traceBackFrame = traceBackFrame?.f_back;
      }
    }
    e.SetTraceBack(traceBack);
    return traceBack;
  }

  public static PythonTuple GetExceptionInfo(CodeContext context)
  {
    return PythonOps.GetExceptionInfoLocal(context, PythonOps.RawException);
  }

  public static PythonTuple GetExceptionInfoLocal(CodeContext context, Exception ex)
  {
    if (ex == null)
      return PythonTuple.MakeTuple(new object[3]);
    PythonContext languageContext = context.LanguageContext;
    object python = PythonExceptions.ToPython(ex);
    TraceBack traceBack = PythonOps.CreateTraceBack(languageContext, ex);
    object type = !(python is IPythonObject pythonObject) ? PythonOps.GetBoundAttr(context, python, "__class__") : (object) pythonObject.PythonType;
    languageContext.UpdateExceptionInfo(type, python, (object) traceBack);
    return PythonTuple.MakeTuple(type, python, (object) traceBack);
  }

  public static Exception MakeRethrownException(CodeContext context)
  {
    PythonTuple exceptionInfo = PythonOps.GetExceptionInfo(context);
    return PythonOps.MakeRethrowExceptionWorker(PythonOps.MakeExceptionWorker(context, exceptionInfo[0], exceptionInfo[1], exceptionInfo[2], true));
  }

  public static Exception MakeRethrowExceptionWorker(Exception e)
  {
    e.RemoveTraceBack();
    ExceptionHelpers.UpdateForRethrow(e);
    return e;
  }

  public static Exception MakeException(
    CodeContext context,
    object type,
    object value,
    object traceback)
  {
    Exception e = PythonOps.MakeExceptionWorker(context, type, value, traceback, false);
    e.RemoveFrameList();
    return e;
  }

  public static Exception MakeExceptionForGenerator(
    CodeContext context,
    object type,
    object value,
    object traceback)
  {
    Exception e = PythonOps.MakeExceptionWorker(context, type, value, traceback, false, true);
    e.RemoveFrameList();
    return e;
  }

  private static Exception MakeExceptionWorker(
    CodeContext context,
    object type,
    object value,
    object traceback,
    bool forRethrow,
    bool forGenerator = false)
  {
    while (type is PythonTuple && ((IEnumerable<object>) type).Any<object>())
      type = ((IEnumerable<object>) type).First<object>();
    Exception e = !(type is PythonExceptions.BaseException) ? (!(type is Exception) ? (!(type is PythonType type1) || !typeof (PythonExceptions.BaseException).IsAssignableFrom(type1.UnderlyingSystemType) ? (!(type is OldClass) ? (!(type is OldInstance) ? PythonOps.MakeExceptionTypeError(type, forGenerator) : (Exception) new OldInstanceException((OldInstance) type)) : (value != null ? PythonExceptions.CreateThrowableForRaise(context, (OldClass) type, value) : (Exception) new OldInstanceException((OldInstance) PythonCalls.Call(context, type)))) : PythonExceptions.CreateThrowableForRaise(context, type1, value)) : type as Exception) : PythonExceptions.ToClr(type);
    if (traceback != null)
    {
      if (!forRethrow)
      {
        if (!(traceback is TraceBack traceback1))
          throw PythonOps.TypeError("traceback argument must be a traceback object");
        e.SetTraceBack(traceback1);
      }
    }
    else
      e.RemoveTraceBack();
    return e;
  }

  public static Exception CreateThrowable(PythonType type, params object[] args)
  {
    return PythonExceptions.CreateThrowable(type, args);
  }

  public static string[] GetFunctionSignature(PythonFunction function)
  {
    return new string[1]{ function.GetSignatureString() };
  }

  public static PythonDictionary CopyAndVerifyDictionary(PythonFunction function, IDictionary dict)
  {
    foreach (object key in (IEnumerable) dict.Keys)
    {
      if (!(key is string))
        throw PythonOps.TypeError("{0}() keywords must be strings", (object) function.__name__);
    }
    return new PythonDictionary(dict);
  }

  public static PythonDictionary CopyAndVerifyUserMapping(PythonFunction function, object dict)
  {
    return PythonOps.UserMappingToPythonDictionary(function.Context, dict, function.func_name);
  }

  public static PythonDictionary UserMappingToPythonDictionary(
    CodeContext context,
    object dict,
    string funcName)
  {
    object o;
    if (!PythonTypeOps.TryInvokeUnaryOperator(context, dict, "keys", out o))
      throw PythonOps.TypeError("{0}() argument after ** must be a mapping, not {1}", (object) funcName, (object) PythonTypeOps.GetName(dict));
    PythonDictionary pythonDictionary = new PythonDictionary();
    IEnumerator enumerator = PythonOps.GetEnumerator(o);
    while (enumerator.MoveNext())
    {
      object current = enumerator.Current;
      switch (current)
      {
        case string _:
label_6:
          pythonDictionary[current] = PythonOps.GetIndex(context, dict, current);
          continue;
        case Extensible<string> extensible:
          string str = extensible.Value;
          goto label_6;
        default:
          throw PythonOps.TypeError("{0}() keywords must be strings, not {0}", (object) funcName, (object) PythonTypeOps.GetName(dict));
      }
    }
    return pythonDictionary;
  }

  public static PythonDictionary CopyAndVerifyPythonDictionary(
    PythonFunction function,
    PythonDictionary dict)
  {
    return !dict._storage.HasNonStringAttributes() ? new PythonDictionary(dict) : throw PythonOps.TypeError("{0}() keywords must be strings", (object) function.__name__);
  }

  public static object ExtractDictionaryArgument(
    PythonFunction function,
    string name,
    int argCnt,
    PythonDictionary dict)
  {
    object dictionaryArgument;
    if (dict.TryGetValue((object) name, out dictionaryArgument))
    {
      dict.Remove((object) name);
      return dictionaryArgument;
    }
    throw PythonOps.TypeError("{0}() takes exactly {1} arguments ({2} given)", (object) function.__name__, (object) function.NormalArgumentCount, (object) argCnt);
  }

  public static void AddDictionaryArgument(
    PythonFunction function,
    string name,
    object value,
    PythonDictionary dict)
  {
    if (dict.ContainsKey((object) name))
      throw PythonOps.MultipleKeywordArgumentError(function, name);
    dict[(object) name] = value;
  }

  public static void VerifyUnduplicatedByPosition(
    PythonFunction function,
    string name,
    int position,
    int listlen)
  {
    if (listlen > 0 && listlen > position)
      throw PythonOps.MultipleKeywordArgumentError(function, name);
  }

  public static List CopyAndVerifyParamsList(PythonFunction function, object list)
  {
    return new List(list);
  }

  public static PythonTuple GetOrCopyParamsTuple(PythonFunction function, object input)
  {
    if (input == null)
      throw PythonOps.TypeError("{0}() argument after * must be a sequence, not NoneType", (object) function.func_name);
    return input.GetType() == typeof (PythonTuple) ? (PythonTuple) input : PythonTuple.Make(input);
  }

  public static object ExtractParamsArgument(PythonFunction function, int argCnt, List list)
  {
    return list.__len__() != 0 ? list.pop(0) : throw function.BadArgumentError(argCnt);
  }

  public static void AddParamsArguments(List list, params object[] args)
  {
    for (int index = 0; index < args.Length; ++index)
      list.insert(index, args[index]);
  }

  public static object ExtractAnyArgument(
    PythonFunction function,
    string name,
    int argCnt,
    List list,
    IDictionary dict)
  {
    if (dict.Contains((object) name))
    {
      if (list.__len__() != 0)
        throw PythonOps.MultipleKeywordArgumentError(function, name);
      object anyArgument = dict[(object) name];
      dict.Remove((object) name);
      return anyArgument;
    }
    if (list.__len__() != 0)
      return list.pop(0);
    if (function.ExpandDictPosition == -1 && dict.Count > 0)
    {
      foreach (string key in (IEnumerable) dict.Keys)
      {
        bool flag = false;
        foreach (string argName in function.ArgNames)
        {
          if (key == argName)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          throw PythonOps.UnexpectedKeywordArgumentError(function, key);
      }
    }
    throw BinderOps.TypeErrorForIncorrectArgumentCount(function.__name__, function.NormalArgumentCount, function.Defaults.Length, argCnt, function.ExpandListPosition != -1, dict.Count > 0);
  }

  public static ArgumentTypeException SimpleTypeError(string message)
  {
    return (ArgumentTypeException) new TypeErrorException(message);
  }

  public static object GetParamsValueOrDefault(PythonFunction function, int index, List extraArgs)
  {
    return extraArgs.__len__() > 0 ? extraArgs.pop(0) : function.Defaults[index];
  }

  public static object GetFunctionParameterValue(
    PythonFunction function,
    int index,
    string name,
    List extraArgs,
    PythonDictionary dict)
  {
    if (extraArgs != null && extraArgs.__len__() > 0)
      return extraArgs.pop(0);
    object obj;
    return dict != null && dict.TryRemoveValue((object) name, out obj) ? obj : function.Defaults[index];
  }

  public static void CheckParamsZero(PythonFunction function, List extraArgs)
  {
    if (extraArgs.__len__() != 0)
      throw function.BadArgumentError(extraArgs.__len__() + function.NormalArgumentCount);
  }

  public static void CheckUserParamsZero(PythonFunction function, object sequence)
  {
    int num = PythonOps.Length(sequence);
    if (num != 0)
      throw function.BadArgumentError(num + function.NormalArgumentCount);
  }

  public static void CheckDictionaryZero(PythonFunction function, IDictionary dict)
  {
    if (dict.Count != 0)
    {
      IDictionaryEnumerator enumerator = dict.GetEnumerator();
      enumerator.MoveNext();
      throw PythonOps.UnexpectedKeywordArgumentError(function, (string) enumerator.Key);
    }
  }

  public static bool CheckDictionaryMembers(PythonDictionary dict, string[] names)
  {
    if (dict.Count != names.Length)
      return false;
    foreach (string name in names)
    {
      if (!dict.ContainsKey((object) name))
        return false;
    }
    return true;
  }

  public static object PythonFunctionGetMember(PythonFunction function, string name)
  {
    object obj;
    return function._dict != null && function._dict.TryGetValue((object) name, out obj) ? obj : (object) OperationFailed.Value;
  }

  public static object PythonFunctionSetMember(PythonFunction function, string name, object value)
  {
    return function.__dict__[(object) name] = value;
  }

  public static void PythonFunctionDeleteDict()
  {
    throw PythonOps.TypeError("function's dictionary may not be deleted");
  }

  public static void PythonFunctionDeleteDoc(PythonFunction function)
  {
    function.__doc__ = (object) null;
  }

  public static void PythonFunctionDeleteDefaults(PythonFunction function)
  {
    function.__defaults__ = (PythonTuple) null;
  }

  public static bool PythonFunctionDeleteMember(PythonFunction function, string name)
  {
    return function._dict != null && function._dict.Remove((object) name);
  }

  public static object[] InitializeUserTypeSlots(PythonType type)
  {
    if (type.SlotCount == 0)
      return (object[]) null;
    object[] objArray = new object[type.SlotCount + 1];
    for (int index = 0; index < objArray.Length - 1; ++index)
      objArray[index] = (object) Uninitialized.Instance;
    return objArray;
  }

  public static bool IsClsVisible(CodeContext context) => context.ModuleContext.ShowCls;

  public static object GetInitMember(CodeContext context, PythonType type, object instance)
  {
    object initMember;
    type.TryGetNonCustomBoundMember(context, instance, "__init__", out initMember);
    return initMember;
  }

  public static object GetInitSlotMember(
    CodeContext context,
    PythonType type,
    PythonTypeSlot slot,
    object instance)
  {
    object initSlotMember;
    if (!slot.TryGetValue(context, instance, type, out initSlotMember))
      throw PythonOps.TypeError("bad __init__");
    return initSlotMember;
  }

  public static object GetMixedMember(
    CodeContext context,
    PythonType type,
    object instance,
    string name)
  {
    foreach (PythonType dt in (IEnumerable<PythonType>) type.ResolutionOrder)
    {
      if (dt.IsOldClass)
      {
        OldClass pythonType = (OldClass) PythonOps.ToPythonType(dt);
        object self;
        if (pythonType._dict._storage.TryGetValue((object) name, out self))
          return instance != null ? pythonType.GetOldStyleDescriptor(context, self, instance, (object) pythonType) : self;
      }
      else
      {
        PythonTypeSlot slot;
        if (dt.TryLookupSlot(context, name, out slot))
        {
          object obj;
          return slot.TryGetValue(context, instance, type, out obj) ? obj : (object) slot;
        }
      }
    }
    throw PythonOps.AttributeErrorForMissingAttribute((object) type, name);
  }

  public static bool IsNumericObject(object value)
  {
    switch (value)
    {
      case int _:
      case Extensible<int> _:
      case BigInteger _:
      case Extensible<BigInteger> _:
        return true;
      default:
        return value is bool;
    }
  }

  internal static bool IsNumericType(Type t)
  {
    return PythonOps.IsNonExtensibleNumericType(t) || t.IsSubclassOf(typeof (Extensible<int>)) || t.IsSubclassOf(typeof (Extensible<BigInteger>));
  }

  internal static bool IsNonExtensibleNumericType(Type t)
  {
    return t == typeof (int) || t == typeof (bool) || t == typeof (BigInteger);
  }

  public static int NormalizeBigInteger(object self, BigInteger bi, ref int? length)
  {
    if (bi < BigInteger.Zero)
    {
      PythonOps.GetLengthOnce(self, ref length);
      int ret;
      return bi.AsInt32(out ret) ? ret + length.Value : -1;
    }
    int ret1;
    return bi.AsInt32(out ret1) ? ret1 : int.MaxValue;
  }

  public static int GetLengthOnce(object self, ref int? length)
  {
    if (length.HasValue)
      return length.Value;
    length = new int?(PythonOps.Length(self));
    return length.Value;
  }

  public static ReflectedEvent.BoundEvent MakeBoundEvent(
    ReflectedEvent eventObj,
    object instance,
    Type type)
  {
    return new ReflectedEvent.BoundEvent(eventObj, instance, DynamicHelpers.GetPythonTypeFromType(type));
  }

  public static bool CheckTypeVersion(object o, int version)
  {
    return o is IPythonObject pythonObject && pythonObject.PythonType.Version == version;
  }

  public static bool CheckSpecificTypeVersion(PythonType type, int version)
  {
    return type.Version == version;
  }

  internal static MethodInfo GetConversionHelper(string name, ConversionResultKind resultKind)
  {
    switch (resultKind)
    {
      case ConversionResultKind.ImplicitCast:
      case ConversionResultKind.ExplicitCast:
        return typeof (PythonOps).GetMethod("Throwing" + name);
      case ConversionResultKind.ImplicitTry:
      case ConversionResultKind.ExplicitTry:
        return typeof (PythonOps).GetMethod("NonThrowing" + name);
      default:
        throw new InvalidOperationException();
    }
  }

  public static IEnumerable OldInstanceConvertToIEnumerableNonThrowing(
    CodeContext context,
    OldInstance self)
  {
    object callable;
    if (self.TryGetBoundCustomMember(context, "__iter__", out callable))
      return PythonOps.CreatePythonEnumerable((object) self);
    return self.TryGetBoundCustomMember(context, "__getitem__", out callable) ? (IEnumerable) PythonOps.CreateItemEnumerable(callable, context.LanguageContext.GetItemCallSite) : (IEnumerable) null;
  }

  public static IEnumerable OldInstanceConvertToIEnumerableThrowing(
    CodeContext context,
    OldInstance self)
  {
    return PythonOps.OldInstanceConvertToIEnumerableNonThrowing(context, self) ?? throw PythonOps.TypeErrorForTypeMismatch("IEnumerable", (object) self);
  }

  public static IEnumerable<T> OldInstanceConvertToIEnumerableOfTNonThrowing<T>(
    CodeContext context,
    OldInstance self)
  {
    object callable;
    if (self.TryGetBoundCustomMember(context, "__iter__", out callable))
      return (IEnumerable<T>) new IEnumerableOfTWrapper<T>(PythonOps.CreatePythonEnumerable((object) self));
    return self.TryGetBoundCustomMember(context, "__getitem__", out callable) ? (IEnumerable<T>) new IEnumerableOfTWrapper<T>((IEnumerable) PythonOps.CreateItemEnumerable(callable, context.LanguageContext.GetItemCallSite)) : (IEnumerable<T>) null;
  }

  public static IEnumerable<T> OldInstanceConvertToIEnumerableOfTThrowing<T>(
    CodeContext context,
    OldInstance self)
  {
    return PythonOps.OldInstanceConvertToIEnumerableOfTNonThrowing<T>(context, self) ?? throw PythonOps.TypeErrorForTypeMismatch("IEnumerable[T]", (object) self);
  }

  public static IEnumerator OldInstanceConvertToIEnumeratorNonThrowing(
    CodeContext context,
    OldInstance self)
  {
    object callable;
    if (self.TryGetBoundCustomMember(context, "__iter__", out callable))
      return PythonOps.CreatePythonEnumerator((object) self);
    return self.TryGetBoundCustomMember(context, "__getitem__", out callable) ? PythonOps.CreateItemEnumerator(callable, context.LanguageContext.GetItemCallSite) : (IEnumerator) null;
  }

  public static IEnumerator OldInstanceConvertToIEnumeratorThrowing(
    CodeContext context,
    OldInstance self)
  {
    return PythonOps.OldInstanceConvertToIEnumeratorNonThrowing(context, self) ?? throw PythonOps.TypeErrorForTypeMismatch("IEnumerator", (object) self);
  }

  public static bool? OldInstanceConvertToBoolNonThrowing(CodeContext context, OldInstance oi)
  {
    object func;
    if (oi.TryGetBoundCustomMember(context, "__nonzero__", out func))
    {
      switch (PythonOps.NonThrowingConvertToNonZero(PythonCalls.Call(context, func)))
      {
        case int num:
          return new bool?(num != 0);
        case bool flag:
          return new bool?(flag);
      }
    }
    else
    {
      int result;
      if (oi.TryGetBoundCustomMember(context, "__len__", out func) && Converter.TryConvertToInt32(PythonCalls.Call(context, func), out result))
        return new bool?(result != 0);
    }
    return new bool?();
  }

  public static object OldInstanceConvertToBoolThrowing(CodeContext context, OldInstance oi)
  {
    object func;
    if (oi.TryGetBoundCustomMember(context, "__nonzero__", out func))
      return (object) PythonOps.ThrowingConvertToNonZero(PythonCalls.Call(context, func));
    return oi.TryGetBoundCustomMember(context, "__len__", out func) ? (object) (context.LanguageContext.ConvertToInt32(PythonCalls.Call(context, func)) != 0) : (object) null;
  }

  public static object OldInstanceConvertNonThrowing(
    CodeContext context,
    OldInstance oi,
    string conversion)
  {
    object func;
    if (oi.TryGetBoundCustomMember(context, conversion, out func))
    {
      switch (conversion)
      {
        case "__int__":
          return PythonOps.NonThrowingConvertToInt(PythonCalls.Call(context, func));
        case "__long__":
          return PythonOps.NonThrowingConvertToLong(PythonCalls.Call(context, func));
        case "__float__":
          return PythonOps.NonThrowingConvertToFloat(PythonCalls.Call(context, func));
        case "__complex__":
          return PythonOps.NonThrowingConvertToComplex(PythonCalls.Call(context, func));
        case "__str__":
          return PythonOps.NonThrowingConvertToString(PythonCalls.Call(context, func));
      }
    }
    else if (conversion == "__complex__")
    {
      object obj = PythonOps.OldInstanceConvertNonThrowing(context, oi, "__float__");
      return obj == null ? (object) null : (object) Converter.ConvertToComplex(obj);
    }
    return (object) null;
  }

  public static object OldInstanceConvertThrowing(
    CodeContext context,
    OldInstance oi,
    string conversion)
  {
    object func;
    if (oi.TryGetBoundCustomMember(context, conversion, out func))
    {
      switch (conversion)
      {
        case "__int__":
          return PythonOps.ThrowingConvertToInt(PythonCalls.Call(context, func));
        case "__long__":
          return PythonOps.ThrowingConvertToLong(PythonCalls.Call(context, func));
        case "__float__":
          return PythonOps.ThrowingConvertToFloat(PythonCalls.Call(context, func));
        case "__complex__":
          return PythonOps.ThrowingConvertToComplex(PythonCalls.Call(context, func));
        case "__str__":
          return PythonOps.ThrowingConvertToString(PythonCalls.Call(context, func));
      }
    }
    else if (conversion == "__complex__")
      return PythonOps.OldInstanceConvertThrowing(context, oi, "__float__");
    return (object) null;
  }

  public static object ConvertFloatToComplex(object value)
  {
    return value == null ? (object) null : (object) new Complex((double) value, 0.0);
  }

  internal static bool CheckingConvertToInt(object value)
  {
    switch (value)
    {
      case int _:
      case BigInteger _:
      case Extensible<int> _:
        return true;
      default:
        return value is Extensible<BigInteger>;
    }
  }

  internal static bool CheckingConvertToLong(object value) => PythonOps.CheckingConvertToInt(value);

  internal static bool CheckingConvertToFloat(object value)
  {
    if (value is double)
      return true;
    return value != null && value is Extensible<double>;
  }

  internal static bool CheckingConvertToComplex(object value)
  {
    return value is Complex || value is Extensible<Complex> || PythonOps.CheckingConvertToInt(value) || PythonOps.CheckingConvertToFloat(value);
  }

  internal static bool CheckingConvertToString(object value)
  {
    return value is string || value is Extensible<string>;
  }

  public static bool CheckingConvertToNonZero(object value) => value is bool || value is int;

  public static object NonThrowingConvertToInt(object value)
  {
    return !PythonOps.CheckingConvertToInt(value) ? (object) null : value;
  }

  public static object NonThrowingConvertToLong(object value)
  {
    return !PythonOps.CheckingConvertToInt(value) ? (object) null : value;
  }

  public static object NonThrowingConvertToFloat(object value)
  {
    return !PythonOps.CheckingConvertToFloat(value) ? (object) null : value;
  }

  public static object NonThrowingConvertToComplex(object value)
  {
    return !PythonOps.CheckingConvertToComplex(value) ? (object) null : value;
  }

  public static object NonThrowingConvertToString(object value)
  {
    return !PythonOps.CheckingConvertToString(value) ? (object) null : value;
  }

  public static object NonThrowingConvertToNonZero(object value)
  {
    return !PythonOps.CheckingConvertToNonZero(value) ? (object) null : value;
  }

  public static object ThrowingConvertToInt(object value)
  {
    return PythonOps.CheckingConvertToInt(value) ? value : throw PythonOps.TypeError(" __int__ returned non-int (type {0})", (object) PythonTypeOps.GetName(value));
  }

  public static object ThrowingConvertToFloat(object value)
  {
    return PythonOps.CheckingConvertToFloat(value) ? value : throw PythonOps.TypeError(" __float__ returned non-float (type {0})", (object) PythonTypeOps.GetName(value));
  }

  public static object ThrowingConvertToComplex(object value)
  {
    return PythonOps.CheckingConvertToComplex(value) ? value : throw PythonOps.TypeError(" __complex__ returned non-complex (type {0})", (object) PythonTypeOps.GetName(value));
  }

  public static object ThrowingConvertToLong(object value)
  {
    return PythonOps.CheckingConvertToComplex(value) ? value : throw PythonOps.TypeError(" __long__ returned non-long (type {0})", (object) PythonTypeOps.GetName(value));
  }

  public static object ThrowingConvertToString(object value)
  {
    return PythonOps.CheckingConvertToString(value) ? value : throw PythonOps.TypeError(" __str__ returned non-str (type {0})", (object) PythonTypeOps.GetName(value));
  }

  public static bool ThrowingConvertToNonZero(object value)
  {
    if (!PythonOps.CheckingConvertToNonZero(value))
      throw PythonOps.TypeError("__nonzero__ should return bool or int, returned {0}", (object) PythonTypeOps.GetName(value));
    return value is bool flag ? flag : (int) value != 0;
  }

  public static bool SlotTryGetBoundValue(
    CodeContext context,
    PythonTypeSlot slot,
    object instance,
    PythonType owner,
    out object value)
  {
    return slot.TryGetValue(context, instance, owner, out value);
  }

  public static bool SlotTryGetValue(
    CodeContext context,
    PythonTypeSlot slot,
    object instance,
    PythonType owner,
    out object value)
  {
    return slot.TryGetValue(context, instance, owner, out value);
  }

  public static object SlotGetValue(
    CodeContext context,
    PythonTypeSlot slot,
    object instance,
    PythonType owner)
  {
    object obj;
    if (!slot.TryGetValue(context, instance, owner, out obj))
      throw new InvalidOperationException();
    return obj;
  }

  public static bool SlotTrySetValue(
    CodeContext context,
    PythonTypeSlot slot,
    object instance,
    PythonType owner,
    object value)
  {
    return slot.TrySetValue(context, instance, owner, value);
  }

  public static object SlotSetValue(
    CodeContext context,
    PythonTypeSlot slot,
    object instance,
    PythonType owner,
    object value)
  {
    if (!slot.TrySetValue(context, instance, owner, value))
      throw new InvalidOperationException();
    return value;
  }

  public static bool SlotTryDeleteValue(
    CodeContext context,
    PythonTypeSlot slot,
    object instance,
    PythonType owner)
  {
    return slot.TryDeleteValue(context, instance, owner);
  }

  public static BuiltinFunction MakeBoundBuiltinFunction(BuiltinFunction function, object target)
  {
    return function.BindToInstance(target);
  }

  public static object GetBuiltinFunctionSelf(BuiltinFunction function) => function.BindingSelf;

  public static bool TestBoundBuiltinFunction(BuiltinFunction function, object data)
  {
    return !function.IsUnbound && function.TestData(data);
  }

  public static BuiltinFunction GetBuiltinMethodDescriptorTemplate(
    BuiltinMethodDescriptor descriptor)
  {
    return descriptor.Template;
  }

  public static int GetTypeVersion(PythonType type) => type.Version;

  public static bool TryResolveTypeSlot(
    CodeContext context,
    PythonType type,
    string name,
    out PythonTypeSlot slot)
  {
    return type.TryResolveSlot(context, name, out slot);
  }

  public static T[] ConvertTupleToArray<T>(PythonTuple tuple)
  {
    T[] array = new T[tuple.__len__()];
    for (int index = 0; index < tuple.__len__(); ++index)
    {
      try
      {
        array[index] = (T) tuple[index];
      }
      catch (InvalidCastException ex)
      {
        array[index] = Converter.Convert<T>(tuple[index]);
      }
    }
    return array;
  }

  public static PythonGenerator MakeGenerator(
    PythonFunction function,
    MutableTuple data,
    object generatorCode)
  {
    if (!(generatorCode is Func<MutableTuple, object> next))
      next = ((LazyCode<Func<MutableTuple, object>>) generatorCode).EnsureDelegate();
    return new PythonGenerator(function, next, data);
  }

  public static object MakeGeneratorExpression(object function, object input)
  {
    PythonFunction pythonFunction = (PythonFunction) function;
    return ((Func<PythonFunction, object, object>) pythonFunction.func_code.Target)(pythonFunction, input);
  }

  public static FunctionCode MakeFunctionCode(
    CodeContext context,
    string name,
    string documentation,
    string[] argNames,
    FunctionAttributes flags,
    int startIndex,
    int endIndex,
    string path,
    Delegate code,
    string[] freeVars,
    string[] names,
    string[] cellVars,
    string[] varNames,
    int localCount)
  {
    SerializedScopeStatement scope = new SerializedScopeStatement(name, argNames, flags, startIndex, endIndex, path, freeVars, names, cellVars, varNames);
    return new FunctionCode(context.LanguageContext, code, (ScopeStatement) scope, documentation, localCount);
  }

  [NoSideEffects]
  public static object MakeFunction(
    CodeContext context,
    FunctionCode funcInfo,
    object modName,
    object[] defaults)
  {
    return (object) new PythonFunction(context, funcInfo, modName, defaults, (MutableTuple) null);
  }

  [NoSideEffects]
  public static object MakeFunctionDebug(
    CodeContext context,
    FunctionCode funcInfo,
    object modName,
    object[] defaults,
    Delegate target)
  {
    funcInfo.SetDebugTarget(context.LanguageContext, target);
    return (object) new PythonFunction(context, funcInfo, modName, defaults, (MutableTuple) null);
  }

  public static CodeContext FunctionGetContext(PythonFunction func) => func.Context;

  public static object FunctionGetDefaultValue(PythonFunction func, int index)
  {
    return func.Defaults[index];
  }

  public static int FunctionGetCompatibility(PythonFunction func) => func.FunctionCompatibility;

  public static int FunctionGetID(PythonFunction func) => func.FunctionID;

  public static Delegate FunctionGetTarget(PythonFunction func) => func.func_code.Target;

  public static Delegate FunctionGetLightThrowTarget(PythonFunction func)
  {
    return func.func_code.LightThrowTarget;
  }

  public static void FunctionPushFrame(PythonContext context)
  {
    if (PythonFunction.AddRecursionDepth(1) > context.RecursionLimit)
      throw PythonOps.RuntimeError("maximum recursion depth exceeded");
  }

  public static void FunctionPushFrameCodeContext(CodeContext context)
  {
    PythonOps.FunctionPushFrame(context.LanguageContext);
  }

  public static void FunctionPopFrame() => PythonFunction.AddRecursionDepth(-1);

  public static object ReturnConversionResult(object value)
  {
    return value is PythonTuple pythonTuple ? pythonTuple[0] : (object) NotImplementedType.Value;
  }

  public static T ConvertFromObject<T>(object obj)
  {
    Type type = typeof (T);
    MethodInfo fastConvertMethod = PythonBinder.GetFastConvertMethod(type);
    object obj1;
    if (fastConvertMethod != (MethodInfo) null)
      obj1 = fastConvertMethod.Invoke((object) null, new object[1]
      {
        obj
      });
    else
      obj1 = !typeof (Delegate).IsAssignableFrom(type) ? obj : Converter.ConvertToDelegate(obj, type);
    return (T) obj1;
  }

  public static DynamicMetaObjectBinder MakeComplexCallAction(
    int count,
    bool list,
    string[] keywords)
  {
    Argument[] objArray = CompilerHelpers.MakeRepeatedArray<Argument>(Argument.Simple, count + keywords.Length);
    if (list)
      objArray[checked (count - 1)] = new Argument(ArgumentType.List);
    for (int index = 0; index < keywords.Length; ++index)
      objArray[count + index] = new Argument(keywords[index]);
    return (DynamicMetaObjectBinder) DefaultContext.DefaultPythonContext.Invoke(new CallSignature(objArray));
  }

  public static DynamicMetaObjectBinder MakeSimpleCallAction(int count)
  {
    return (DynamicMetaObjectBinder) DefaultContext.DefaultPythonContext.Invoke(new CallSignature(CompilerHelpers.MakeRepeatedArray<Argument>(Argument.Simple, count)));
  }

  public static PythonTuple ValidateCoerceResult(object coerceResult)
  {
    if (coerceResult == null || coerceResult == NotImplementedType.Value)
      return (PythonTuple) null;
    return coerceResult is PythonTuple pythonTuple ? pythonTuple : throw PythonOps.TypeError("coercion should return None, NotImplemented, or 2-tuple, got {0}", (object) PythonTypeOps.GetName(coerceResult));
  }

  public static object GetCoerceResultOne(PythonTuple coerceResult) => coerceResult._data[0];

  public static object GetCoerceResultTwo(PythonTuple coerceResult) => coerceResult._data[1];

  public static object MethodCheckSelf(CodeContext context, Method method, object self)
  {
    return method.CheckSelf(context, self);
  }

  [LightThrowing]
  public static object GeneratorCheckThrowableAndReturnSendValue(object self)
  {
    return ((PythonGenerator) self).CheckThrowableAndReturnSendValue();
  }

  public static ItemEnumerable CreateItemEnumerable(
    object callable,
    CallSite<Func<CallSite, CodeContext, object, int, object>> site)
  {
    return new ItemEnumerable(callable, site);
  }

  public static DictionaryKeyEnumerator MakeDictionaryKeyEnumerator(PythonDictionary dict)
  {
    return new DictionaryKeyEnumerator(dict._storage);
  }

  public static IEnumerable CreatePythonEnumerable(object baseObject)
  {
    return PythonEnumerable.Create(baseObject);
  }

  public static IEnumerator CreateItemEnumerator(
    object callable,
    CallSite<Func<CallSite, CodeContext, object, int, object>> site)
  {
    return (IEnumerator) new ItemEnumerator(callable, site);
  }

  public static IEnumerator CreatePythonEnumerator(object baseObject)
  {
    return PythonEnumerator.Create(baseObject);
  }

  public static bool ContainsFromEnumerable(CodeContext context, object enumerable, object value)
  {
    switch (enumerable)
    {
      case IEnumerator enumerator:
label_5:
        while (enumerator.MoveNext())
        {
          if (PythonOps.EqualRetBool(context, enumerator.Current, value))
            return true;
        }
        return false;
      case IEnumerable enumerable1:
        enumerator = enumerable1.GetEnumerator();
        goto label_5;
      default:
        enumerator = Converter.ConvertToIEnumerator(enumerable);
        goto label_5;
    }
  }

  public static object PythonTypeGetMember(
    CodeContext context,
    PythonType type,
    object instance,
    string name)
  {
    return type.GetMember(context, instance, name);
  }

  [NoSideEffects]
  public static object CheckUninitialized(object value, string name)
  {
    return value != Uninitialized.Instance ? value : throw new UnboundLocalException($"Local variable '{name}' referenced before assignment.");
  }

  public static PythonDictionary OldClassGetDictionary(OldClass klass) => klass._dict;

  public static string OldClassGetName(OldClass klass) => klass.Name;

  public static bool OldInstanceIsCallable(CodeContext context, OldInstance self)
  {
    return self.TryGetBoundCustomMember(context, "__call__", out object _);
  }

  public static object OldClassCheckCallError(OldClass self, object dictionary, object list)
  {
    return dictionary != null && PythonOps.Length(dictionary) != 0 || list != null && PythonOps.Length(list) != 0 ? OldClass.MakeCallError() : (object) null;
  }

  public static object OldClassSetBases(OldClass oc, object value)
  {
    oc.SetBases(value);
    return value;
  }

  public static object OldClassSetName(OldClass oc, object value)
  {
    oc.SetName(value);
    return value;
  }

  public static object OldClassSetDictionary(OldClass oc, object value)
  {
    oc.SetDictionary(value);
    return value;
  }

  public static object OldClassSetNameHelper(OldClass oc, string name, object value)
  {
    oc.SetNameHelper(name, value);
    return value;
  }

  public static object OldClassTryLookupInit(OldClass oc, object inst)
  {
    object ret;
    return oc.TryLookupInit(inst, out ret) ? ret : (object) OperationFailed.Value;
  }

  public static object OldClassMakeCallError(OldClass oc) => OldClass.MakeCallError();

  public static PythonTuple OldClassGetBaseClasses(OldClass oc)
  {
    return PythonTuple.MakeTuple((object[]) oc.BaseClasses.ToArray());
  }

  public static void OldClassDictionaryIsPublic(OldClass oc) => oc.DictionaryIsPublic();

  public static object OldClassTryLookupValue(CodeContext context, OldClass oc, string name)
  {
    object obj;
    return oc.TryLookupValue(context, name, out obj) ? obj : (object) OperationFailed.Value;
  }

  public static object OldClassLookupValue(CodeContext context, OldClass oc, string name)
  {
    return oc.LookupValue(context, name);
  }

  public static object OldInstanceGetOptimizedDictionary(OldInstance instance, int keyVersion)
  {
    return !(instance.Dictionary._storage is CustomInstanceDictionaryStorage storage) || instance._class.HasSetAttr || storage.KeyVersion != keyVersion ? (object) null : (object) storage;
  }

  public static object OldInstanceDictionaryGetValueHelper(
    object dict,
    int index,
    object oldInstance)
  {
    return ((CustomInstanceDictionaryStorage) dict).GetValueHelper(index, oldInstance);
  }

  public static bool TryOldInstanceDictionaryGetValueHelper(
    object dict,
    int index,
    object oldInstance,
    out object res)
  {
    return ((CustomInstanceDictionaryStorage) dict).TryGetValueHelper(index, oldInstance, out res);
  }

  public static object OldInstanceGetBoundMember(
    CodeContext context,
    OldInstance instance,
    string name)
  {
    return instance.GetBoundMember(context, name);
  }

  public static object OldInstanceDictionarySetExtraValue(object dict, int index, object value)
  {
    ((CustomInstanceDictionaryStorage) dict).SetExtraValue(index, value);
    return value;
  }

  public static object OldClassDeleteMember(CodeContext context, OldClass self, string name)
  {
    self.DeleteCustomMember(context, name);
    return (object) null;
  }

  public static bool OldClassTryLookupOneSlot(
    PythonType type,
    OldClass self,
    string name,
    out object value)
  {
    return self.TryLookupOneSlot(type, name, out value);
  }

  public static object OldInstanceTryGetBoundCustomMember(
    CodeContext context,
    OldInstance self,
    string name)
  {
    object obj;
    return self.TryGetBoundCustomMember(context, name, out obj) ? obj : (object) OperationFailed.Value;
  }

  public static object OldInstanceSetCustomMember(
    CodeContext context,
    OldInstance self,
    string name,
    object value)
  {
    self.SetCustomMember(context, name, value);
    return value;
  }

  public static object OldInstanceDeleteCustomMember(
    CodeContext context,
    OldInstance self,
    string name)
  {
    self.DeleteCustomMember(context, name);
    return (object) null;
  }

  public static object PythonTypeSetCustomMember(
    CodeContext context,
    PythonType self,
    string name,
    object value)
  {
    self.SetCustomMember(context, name, value);
    return value;
  }

  public static object PythonTypeDeleteCustomMember(
    CodeContext context,
    PythonType self,
    string name)
  {
    self.DeleteCustomMember(context, name);
    return (object) null;
  }

  public static bool IsPythonType(PythonType type) => type.IsPythonType;

  public static object PublishModule(CodeContext context, string name)
  {
    object obj = (object) null;
    context.LanguageContext.SystemStateModules.TryGetValue((object) name, out obj);
    PythonModule module = ((PythonScopeExtension) context.GlobalScope.GetExtension(context.LanguageContext.ContextId)).Module;
    context.LanguageContext.SystemStateModules[(object) name] = (object) module;
    return obj;
  }

  public static void RemoveModule(CodeContext context, string name, object oldValue)
  {
    if (oldValue != null)
      context.LanguageContext.SystemStateModules[(object) name] = oldValue;
    else
      context.LanguageContext.SystemStateModules.Remove((object) name);
  }

  public static Ellipsis Ellipsis => Ellipsis.Value;

  public static NotImplementedType NotImplemented => NotImplementedType.Value;

  public static void ListAddForComprehension(List l, object o) => l.AddNoLock(o);

  public static void SetAddForComprehension(SetCollection s, object o) => s._items.AddNoLock(o);

  public static void DictAddForComprehension(PythonDictionary d, object k, object v)
  {
    d._storage.AddNoLock(ref d._storage, k, v);
  }

  public static void ModuleStarted(CodeContext context, ModuleOptions features)
  {
    context.ModuleContext.Features |= features;
  }

  public static void Warn(
    CodeContext context,
    PythonType category,
    string message,
    params object[] args)
  {
    PythonContext languageContext = context.LanguageContext;
    object warningsModule = languageContext.GetWarningsModule();
    object func = (object) null;
    if (warningsModule != null)
      func = PythonOps.GetBoundAttr(context, warningsModule, "warn");
    message = PythonOps.FormatWarning(message, args);
    if (func == null)
      PythonOps.PrintWithDest(context, languageContext.SystemStandardError, (object) $"warning: {category.Name}: {message}");
    else
      PythonOps.CallWithContext(context, func, (object) message, (object) category);
  }

  public static void Warn3k(CodeContext context, string message, params object[] args)
  {
    if (!context.LanguageContext.PythonOptions.WarnPython30)
      return;
    PythonOps.Warn(context, PythonExceptions.DeprecationWarning, message, args);
  }

  public static void ShowWarning(
    CodeContext context,
    PythonType category,
    string message,
    string filename,
    int lineNo)
  {
    PythonContext languageContext = context.LanguageContext;
    object warningsModule = languageContext.GetWarningsModule();
    object func = (object) null;
    if (warningsModule != null)
      func = PythonOps.GetBoundAttr(context, warningsModule, "showwarning");
    if (func == null)
      PythonOps.PrintWithDestNoNewline(context, languageContext.SystemStandardError, (object) $"{filename}:{lineNo}: {category.Name}: {message}\n");
    else
      PythonOps.CallWithContext(context, func, (object) message, (object) category, (object) (filename ?? ""), (object) lineNo);
  }

  private static string FormatWarning(string message, object[] args)
  {
    for (int index = 0; index < args.Length; ++index)
      args[index] = (object) PythonOps.ToString(args[index]);
    message = string.Format(message, args);
    return message;
  }

  private static bool IsPrimitiveNumber(object o)
  {
    if (!PythonOps.IsNumericObject(o))
    {
      switch (o)
      {
        case Complex _:
        case double _:
        case Extensible<Complex> _:
          break;
        default:
          return o is Extensible<double>;
      }
    }
    return true;
  }

  public static void WarnDivision(
    CodeContext context,
    PythonDivisionOptions options,
    object self,
    object other)
  {
    if (options == PythonDivisionOptions.WarnAll)
    {
      if (!PythonOps.IsPrimitiveNumber(self) || !PythonOps.IsPrimitiveNumber(other))
        return;
      if (self is Complex || other is Complex || self is Extensible<Complex> || other is Extensible<Complex>)
        PythonOps.Warn(context, PythonExceptions.DeprecationWarning, "classic complex division");
      else if (self is double || other is double || self is Extensible<double> || other is Extensible<double>)
        PythonOps.Warn(context, PythonExceptions.DeprecationWarning, "classic float division");
      else
        PythonOps.WarnDivisionInts(context, self, other);
    }
    else
    {
      if (!PythonOps.IsNumericObject(self) || !PythonOps.IsNumericObject(other))
        return;
      PythonOps.WarnDivisionInts(context, self, other);
    }
  }

  private static void WarnDivisionInts(CodeContext context, object self, object other)
  {
    if (self is BigInteger || other is BigInteger || self is Extensible<BigInteger> || other is Extensible<BigInteger>)
      PythonOps.Warn(context, PythonExceptions.DeprecationWarning, "classic long division");
    else
      PythonOps.Warn(context, PythonExceptions.DeprecationWarning, "classic int division");
  }

  public static DynamicMetaObjectBinder MakeComboAction(
    CodeContext context,
    DynamicMetaObjectBinder opBinder,
    DynamicMetaObjectBinder convBinder)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.BinaryOperationRetType((PythonBinaryOperationBinder) opBinder, (PythonConversionBinder) convBinder);
  }

  public static DynamicMetaObjectBinder MakeInvokeAction(
    CodeContext context,
    CallSignature signature)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.Invoke(signature);
  }

  public static DynamicMetaObjectBinder MakeGetAction(
    CodeContext context,
    string name,
    bool isNoThrow)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.GetMember(name);
  }

  public static DynamicMetaObjectBinder MakeCompatGetAction(CodeContext context, string name)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.CompatGetMember(name, false);
  }

  public static DynamicMetaObjectBinder MakeCompatInvokeAction(
    CodeContext context,
    CallInfo callInfo)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.CompatInvoke(callInfo);
  }

  public static DynamicMetaObjectBinder MakeCompatConvertAction(
    CodeContext context,
    Type toType,
    bool isExplicit)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.Convert(toType, isExplicit ? ConversionResultKind.ExplicitCast : ConversionResultKind.ImplicitCast).CompatBinder;
  }

  public static DynamicMetaObjectBinder MakeSetAction(CodeContext context, string name)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.SetMember(name);
  }

  public static DynamicMetaObjectBinder MakeDeleteAction(CodeContext context, string name)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.DeleteMember(name);
  }

  public static DynamicMetaObjectBinder MakeConversionAction(
    CodeContext context,
    Type type,
    ConversionResultKind kind)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.Convert(type, kind);
  }

  public static DynamicMetaObjectBinder MakeTryConversionAction(
    CodeContext context,
    Type type,
    ConversionResultKind kind)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.Convert(type, kind);
  }

  public static DynamicMetaObjectBinder MakeOperationAction(CodeContext context, int operationName)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.Operation((PythonOperationKind) operationName);
  }

  public static DynamicMetaObjectBinder MakeUnaryOperationAction(
    CodeContext context,
    ExpressionType expressionType)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.UnaryOperation(expressionType);
  }

  public static DynamicMetaObjectBinder MakeBinaryOperationAction(
    CodeContext context,
    ExpressionType expressionType)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.BinaryOperation(expressionType);
  }

  public static DynamicMetaObjectBinder MakeGetIndexAction(CodeContext context, int argCount)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.GetIndex(argCount);
  }

  public static DynamicMetaObjectBinder MakeSetIndexAction(CodeContext context, int argCount)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.SetIndex(argCount);
  }

  public static DynamicMetaObjectBinder MakeDeleteIndexAction(CodeContext context, int argCount)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.DeleteIndex(argCount);
  }

  public static DynamicMetaObjectBinder MakeGetSliceBinder(CodeContext context)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.GetSlice;
  }

  public static DynamicMetaObjectBinder MakeSetSliceBinder(CodeContext context)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.SetSliceBinder;
  }

  public static DynamicMetaObjectBinder MakeDeleteSliceBinder(CodeContext context)
  {
    return (DynamicMetaObjectBinder) context.LanguageContext.DeleteSlice;
  }

  public static AssemblyBuilder DefineDynamicAssembly(
    AssemblyName name,
    AssemblyBuilderAccess access)
  {
    return AssemblyBuilder.DefineDynamicAssembly(name, access);
  }

  public static Type MakeNewCustomDelegate(Type[] types)
  {
    return PythonOps.MakeNewCustomDelegate(types, new CallingConvention?());
  }

  public static Type MakeNewCustomDelegate(Type[] types, CallingConvention? callingConvention)
  {
    Type type = types[types.Length - 1];
    Type[] parameterTypes = ArrayUtils.RemoveLast<Type>(types);
    TypeBuilder typeBuilder = Snippets.Shared.DefineDelegateType("Delegate" + (object) types.Length);
    typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName, CallingConventions.Standard, PythonOps._DelegateCtorSignature).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
    typeBuilder.DefineMethod("Invoke", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask, type, parameterTypes).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
    if (callingConvention.HasValue)
      typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof (UnmanagedFunctionPointerAttribute).GetConstructor(new Type[1]
      {
        typeof (CallingConvention)
      }), new object[1]{ (object) callingConvention }));
    return typeBuilder.CreateType();
  }

  public static int InitializeModule(Assembly precompiled, string main, string[] references)
  {
    return PythonOps.InitializeModuleEx(precompiled, main, references, false, (Dictionary<string, object>) null);
  }

  public static int InitializeModuleEx(
    Assembly precompiled,
    string main,
    string[] references,
    bool ignoreEnvVars)
  {
    return PythonOps.InitializeModuleEx(precompiled, main, references, ignoreEnvVars, (Dictionary<string, object>) null);
  }

  public static int InitializeModuleEx(
    Assembly precompiled,
    string main,
    string[] references,
    bool ignoreEnvVars,
    Dictionary<string, object> options)
  {
    ContractUtils.RequiresNotNull((object) precompiled, nameof (precompiled));
    ContractUtils.RequiresNotNull((object) main, nameof (main));
    if (options == null)
      options = new Dictionary<string, object>();
    options["Arguments"] = (object) Environment.GetCommandLineArgs();
    PythonContext languageContext = (PythonContext) HostingHelpers.GetLanguageContext(Python.CreateEngine((IDictionary<string, object>) options));
    if (!ignoreEnvVars)
    {
      int count = languageContext.PythonOptions.SearchPaths.Count;
      string environmentVariable = Environment.GetEnvironmentVariable("IRONPYTHONPATH");
      if (environmentVariable != null && environmentVariable.Length > 0)
      {
        string str = environmentVariable;
        char[] chArray = new char[1]{ Path.PathSeparator };
        foreach (string directory in str.Split(chArray))
          languageContext.InsertIntoPath(count++, directory);
      }
    }
    foreach (ScriptCode code in SavableScriptCode.LoadFromAssembly(languageContext.DomainManager, precompiled))
      languageContext.GetCompiledLoader().AddScriptCode(code);
    if (references != null)
    {
      foreach (string reference in references)
        languageContext.DomainManager.LoadAssembly(Assembly.Load(new AssemblyName(reference)));
    }
    ModuleContext moduleContext = new ModuleContext(new PythonDictionary(), languageContext);
    try
    {
      Importer.Import(moduleContext.GlobalContext, main, PythonTuple.EMPTY, 0);
    }
    catch (SystemExitException ex)
    {
      object obj;
      ref object local = ref obj;
      return ex.GetExitCode(out local);
    }
    return 0;
  }

  public static CodeContext GetPythonTypeContext(PythonType pt) => pt.PythonContext.SharedContext;

  public static Delegate GetDelegate(CodeContext context, object target, Type type)
  {
    return context.LanguageContext.DelegateCreator.GetDelegate(target, type);
  }

  public static int CompareLists(List self, List other) => self.CompareTo(other);

  public static int CompareTuples(PythonTuple self, PythonTuple other) => self.CompareTo(other);

  public static int CompareFloats(double self, double other) => DoubleOps.Compare(self, other);

  public static Bytes MakeBytes(byte[] bytes) => new Bytes((IList<byte>) bytes);

  public static byte[] MakeByteArray(this string s)
  {
    byte[] numArray = new byte[s.Length];
    for (int index = 0; index < s.Length; ++index)
      numArray[index] = s[index] < 'Ā' ? (byte) s[index] : throw PythonOps.UnicodeEncodeError("ascii", s[index], index, "'ascii' codec can't encode character '\\u{0:X}' in position {1}: ordinal not in range(128)", (object) (int) s[index], (object) index);
    return numArray;
  }

  public static bool TryMakeByteArray(this string s, out byte[] bytes)
  {
    bytes = new byte[s.Length];
    for (int index = 0; index < s.Length; ++index)
    {
      if (s[index] < 'Ā')
      {
        bytes[index] = (byte) s[index];
      }
      else
      {
        bytes = (byte[]) null;
        return false;
      }
    }
    return true;
  }

  public static string MakeString(this IList<byte> bytes) => bytes.MakeString(bytes.Count);

  internal static string MakeString(this byte[] preamble, IList<byte> bytes)
  {
    char[] chArray = new char[preamble.Length + bytes.Count];
    for (int index = 0; index < preamble.Length; ++index)
      chArray[index] = (char) preamble[index];
    for (int index = 0; index < bytes.Count; ++index)
      chArray[index + preamble.Length] = (char) bytes[index];
    return new string(chArray);
  }

  internal static string MakeString(this IList<byte> bytes, int maxBytes)
  {
    int capacity = Math.Min(bytes.Count, maxBytes);
    StringBuilder stringBuilder = new StringBuilder(capacity);
    for (int index = 0; index < capacity; ++index)
      stringBuilder.Append((char) bytes[index]);
    return stringBuilder.ToString();
  }

  public static void RemoveName(CodeContext context, string name)
  {
    if (!context.TryRemoveVariable(name))
      throw PythonOps.NameError(name);
  }

  public static object LookupName(CodeContext context, string name)
  {
    object obj;
    if (context.TryLookupName(name, out obj) || context.TryLookupBuiltin(name, out obj))
      return obj;
    throw PythonOps.NameError(name);
  }

  public static object SetName(CodeContext context, string name, object value)
  {
    context.SetVariable(name, value);
    return value;
  }

  public static object ToPython(this IntPtr handle)
  {
    long int64 = handle.ToInt64();
    return int64 >= (long) int.MinValue && int64 <= (long) int.MaxValue ? ScriptingRuntimeHelpers.Int32ToObject((int) int64) : (object) (BigInteger) int64;
  }

  public static CodeContext CreateLocalContext(
    CodeContext outerContext,
    MutableTuple boxes,
    string[] args)
  {
    return new CodeContext(new PythonDictionary((DictionaryStorage) new RuntimeVariablesDictionaryStorage(boxes, args)), outerContext.ModuleContext);
  }

  public static CodeContext GetGlobalContext(CodeContext context)
  {
    return context.ModuleContext.GlobalContext;
  }

  public static ClosureCell MakeClosureCell() => new ClosureCell((object) Uninitialized.Instance);

  public static ClosureCell MakeClosureCellWithValue(object initialValue)
  {
    return new ClosureCell(initialValue);
  }

  public static MutableTuple GetClosureTupleFromFunction(PythonFunction function)
  {
    return PythonOps.GetClosureTupleFromContext(function.Context);
  }

  public static MutableTuple GetClosureTupleFromGenerator(PythonGenerator generator)
  {
    return PythonOps.GetClosureTupleFromContext(generator.Context);
  }

  public static MutableTuple GetClosureTupleFromContext(CodeContext context)
  {
    return (context.Dict._storage as RuntimeVariablesDictionaryStorage).Tuple;
  }

  public static CodeContext GetParentContextFromFunction(PythonFunction function)
  {
    return function.Context;
  }

  public static CodeContext GetParentContextFromGenerator(PythonGenerator generator)
  {
    return generator.Context;
  }

  public static object GetGlobal(CodeContext context, string name)
  {
    return PythonOps.GetVariable(context, name, true, false);
  }

  public static object GetLocal(CodeContext context, string name)
  {
    return PythonOps.GetVariable(context, name, false, false);
  }

  internal static object GetVariable(
    CodeContext context,
    string name,
    bool isGlobal,
    bool lightThrow)
  {
    object res;
    if (isGlobal)
    {
      if (context.TryGetGlobalVariable(name, out res))
        return res;
    }
    else if (context.TryLookupName(name, out res))
      return res;
    PythonDictionary builtinsDict = context.GetBuiltinsDict();
    if (builtinsDict != null && builtinsDict.TryGetValue((object) name, out res))
      return res;
    Exception exceptionValue = !isGlobal ? PythonOps.NameError(name) : PythonOps.GlobalNameError(name);
    if (lightThrow)
      return LightExceptions.Throw(exceptionValue);
    throw exceptionValue;
  }

  public static object RawGetGlobal(CodeContext context, string name)
  {
    object res;
    return context.TryGetGlobalVariable(name, out res) ? res : (object) Uninitialized.Instance;
  }

  public static object RawGetLocal(CodeContext context, string name)
  {
    object obj;
    return context.TryLookupName(name, out obj) ? obj : (object) Uninitialized.Instance;
  }

  public static void SetGlobal(CodeContext context, string name, object value)
  {
    context.SetGlobalVariable(name, value);
  }

  public static void SetLocal(CodeContext context, string name, object value)
  {
    context.SetVariable(name, value);
  }

  public static void DeleteGlobal(CodeContext context, string name)
  {
    if (!context.TryRemoveGlobalVariable(name))
      throw PythonOps.NameError(name);
  }

  public static void DeleteLocal(CodeContext context, string name)
  {
    if (!context.TryRemoveVariable(name))
      throw PythonOps.NameError(name);
  }

  public static PythonGlobal[] GetGlobalArrayFromContext(CodeContext context)
  {
    return context.GetGlobalArray();
  }

  public static Exception MultipleKeywordArgumentError(PythonFunction function, string name)
  {
    return PythonOps.TypeError("{0}() got multiple values for keyword argument '{1}'", (object) function.__name__, (object) name);
  }

  public static Exception UnexpectedKeywordArgumentError(PythonFunction function, string name)
  {
    return PythonOps.TypeError("{0}() got an unexpected keyword argument '{1}'", (object) function.__name__, (object) name);
  }

  public static Exception StaticAssignmentFromInstanceError(
    PropertyTracker tracker,
    bool isAssignment)
  {
    if (!isAssignment)
      return (Exception) new MissingMemberException(string.Format(Resources.StaticAccessFromInstanceError, (object) tracker.Name, (object) NameConverter.GetTypeName(tracker.DeclaringType)));
    if (!DynamicHelpers.GetPythonTypeFromType(tracker.DeclaringType).IsPythonType)
      return (Exception) new MissingMemberException(string.Format(Resources.StaticAssignmentFromInstanceError, (object) tracker.Name, (object) NameConverter.GetTypeName(tracker.DeclaringType)));
    return PythonOps.TypeError("can't set attributes of built-in/extension type '{0}'", (object) NameConverter.GetTypeName(tracker.DeclaringType));
  }

  public static Exception FunctionBadArgumentError(PythonFunction func, int count)
  {
    return func.BadArgumentError(count);
  }

  public static Exception BadKeywordArgumentError(PythonFunction func, int count)
  {
    return func.BadKeywordArgumentError(count);
  }

  public static Exception AttributeErrorForMissingOrReadonly(
    CodeContext context,
    PythonType dt,
    string name)
  {
    if (dt.TryResolveSlot(context, name, out PythonTypeSlot _))
      throw PythonOps.AttributeErrorForReadonlyAttribute(dt.Name, name);
    throw PythonOps.AttributeErrorForMissingAttribute(dt.Name, name);
  }

  public static Exception AttributeErrorForMissingAttribute(object o, string name)
  {
    switch (o)
    {
      case PythonType pythonType:
        return PythonOps.AttributeErrorForMissingAttribute(pythonType.Name, name);
      case NamespaceTracker _:
        return PythonOps.AttributeErrorForMissingAttribute(PythonTypeOps.GetName(o), name);
      default:
        return PythonOps.AttributeErrorForReadonlyAttribute(PythonTypeOps.GetName(o), name);
    }
  }

  public static Exception ValueError(string format, params object[] args)
  {
    return (Exception) new ValueErrorException(string.Format(format, args));
  }

  public static Exception KeyError(object key)
  {
    return PythonExceptions.CreateThrowable(PythonExceptions.KeyError, key);
  }

  public static Exception KeyError(string format, params object[] args)
  {
    return (Exception) new KeyNotFoundException(string.Format(format, args));
  }

  public static Exception UnicodeDecodeError(string format, params object[] args)
  {
    return (Exception) new DecoderFallbackException(string.Format(format, args));
  }

  public static Exception UnicodeDecodeError(string message, byte[] bytesUnknown, int index)
  {
    return (Exception) new DecoderFallbackException(message, bytesUnknown, index);
  }

  public static Exception UnicodeEncodeError(string format, params object[] args)
  {
    return (Exception) new EncoderFallbackException(string.Format(format, args));
  }

  public static Exception UnicodeEncodeError(
    string encoding,
    char charUnkown,
    int index,
    string format,
    params object[] args)
  {
    EncoderFallbackException fallbackException = (EncoderFallbackException) typeof (EncoderFallbackException).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, (Binder) null, new Type[3]
    {
      typeof (string),
      typeof (char),
      typeof (int)
    }, (ParameterModifier[]) null).Invoke(new object[3]
    {
      (object) string.Format(format, args),
      (object) charUnkown,
      (object) index
    });
    fallbackException.Data[(object) nameof (encoding)] = (object) encoding;
    return (Exception) fallbackException;
  }

  public static Exception IOError(Exception inner)
  {
    return (Exception) new IOException(inner.Message, inner);
  }

  public static Exception IOError(string format, params object[] args)
  {
    return (Exception) new IOException(string.Format(format, args));
  }

  public static Exception EofError(string format, params object[] args)
  {
    return (Exception) new EndOfStreamException(string.Format(format, args));
  }

  public static Exception StandardError(string format, params object[] args)
  {
    return (Exception) new SystemException(string.Format(format, args));
  }

  public static Exception ZeroDivisionError(string format, params object[] args)
  {
    return (Exception) new DivideByZeroException(string.Format(format, args));
  }

  public static Exception SystemError(string format, params object[] args)
  {
    return (Exception) new SystemException(string.Format(format, args));
  }

  public static Exception TypeError(string format, params object[] args)
  {
    return (Exception) new TypeErrorException(string.Format(format, args));
  }

  public static Exception IndexError(string format, params object[] args)
  {
    return (Exception) new IndexOutOfRangeException(string.Format(format, args));
  }

  public static Exception MemoryError(string format, params object[] args)
  {
    return (Exception) new OutOfMemoryException(string.Format(format, args));
  }

  public static Exception ArithmeticError(string format, params object[] args)
  {
    return (Exception) new ArithmeticException(string.Format(format, args));
  }

  public static Exception NotImplementedError(string format, params object[] args)
  {
    return (Exception) new NotImplementedException(string.Format(format, args));
  }

  public static Exception AttributeError(string format, params object[] args)
  {
    return (Exception) new MissingMemberException(string.Format(format, args));
  }

  public static Exception OverflowError(string format, params object[] args)
  {
    return (Exception) new OverflowException(string.Format(format, args));
  }

  public static Exception WindowsError(string format, params object[] args)
  {
    return (Exception) new Win32Exception(string.Format(format, args));
  }

  public static Exception SystemExit() => (Exception) new SystemExitException();

  public static void SyntaxWarning(
    string message,
    SourceUnit sourceUnit,
    SourceSpan span,
    int errorCode)
  {
    PythonOps.ShowWarning(((PythonContext) sourceUnit.LanguageContext).SharedContext, PythonExceptions.SyntaxWarning, message, sourceUnit.Path, span.Start.Line);
  }

  public static SyntaxErrorException SyntaxError(
    string message,
    SourceUnit sourceUnit,
    SourceSpan span,
    int errorCode)
  {
    switch (errorCode & 2147483632)
    {
      case 32 /*0x20*/:
        return (SyntaxErrorException) new IndentationException(message, sourceUnit, span, errorCode, Severity.FatalError);
      case 48 /*0x30*/:
        return (SyntaxErrorException) new TabException(message, sourceUnit, span, errorCode, Severity.FatalError);
      default:
        SyntaxErrorException syntaxErrorException = new SyntaxErrorException(message, sourceUnit, span, errorCode, Severity.FatalError);
        if ((errorCode & 64 /*0x40*/) != 0)
          syntaxErrorException.Data[PythonContext._syntaxErrorNoCaret] = ScriptingRuntimeHelpers.True;
        return syntaxErrorException;
    }
  }

  public static SyntaxErrorException BadSourceError(byte badByte, SourceSpan span, string path)
  {
    SyntaxErrorException syntaxErrorException = new SyntaxErrorException(string.Format("Non-ASCII character '\\x{0:x2}' in file {2} on line {1}, but no encoding declared; see http://www.python.org/peps/pep-0263.html for details", (object) badByte, (object) span.Start.Line, (object) path), path, (string) null, (string) null, span, 16 /*0x10*/, Severity.FatalError);
    syntaxErrorException.Data[PythonContext._syntaxErrorNoCaret] = ScriptingRuntimeHelpers.True;
    return syntaxErrorException;
  }

  public static Exception StopIteration() => PythonOps.StopIteration("");

  public static Exception InvalidType(object o, RuntimeTypeHandle handle)
  {
    return PythonOps.TypeErrorForTypeMismatch(DynamicHelpers.GetPythonTypeFromType(Type.GetTypeFromHandle(handle)).Name, o);
  }

  public static Exception ZeroDivisionError()
  {
    return PythonOps.ZeroDivisionError("Attempted to divide by zero.");
  }

  public static Exception ValueErrorForUnpackMismatch(int left, int right)
  {
    if (left <= right)
      return PythonOps.ValueError("too many values to unpack");
    return PythonOps.ValueError("need more than {0} values to unpack", (object) right);
  }

  public static Exception NameError(string name)
  {
    return (Exception) new UnboundNameException($"name '{name}' is not defined");
  }

  public static Exception GlobalNameError(string name)
  {
    return (Exception) new UnboundNameException($"global name '{name}' is not defined");
  }

  public static Exception TypeErrorForUnboundMethodCall(
    string methodName,
    Type methodType,
    object instance)
  {
    return PythonOps.TypeErrorForUnboundMethodCall(methodName, DynamicHelpers.GetPythonTypeFromType(methodType), instance);
  }

  public static Exception TypeErrorForUnboundMethodCall(
    string methodName,
    PythonType methodType,
    object instance)
  {
    return PythonOps.TypeError($"unbound method {methodName}() must be called with {methodType.Name} instance as first argument (got {PythonTypeOps.GetName(instance)} instead)");
  }

  public static Exception TypeErrorForIllegalSend()
  {
    return PythonOps.TypeError("can't send non-None value to a just-started generator");
  }

  public static Exception TypeErrorForArgumentCountMismatch(
    string methodName,
    int expectedArgCount,
    int actualArgCount)
  {
    return PythonOps.TypeError("{0}() takes exactly {1} argument{2} ({3} given)", (object) methodName, (object) expectedArgCount, expectedArgCount == 1 ? (object) "" : (object) "s", (object) actualArgCount);
  }

  public static Exception TypeErrorForTypeMismatch(string expectedTypeName, object instance)
  {
    return PythonOps.TypeError("expected {0}, got {1}", (object) expectedTypeName, (object) PythonOps.GetPythonTypeName(instance));
  }

  public static Exception TypeErrorForUnhashableType(string typeName)
  {
    return PythonOps.TypeError(typeName + " objects are unhashable");
  }

  public static Exception TypeErrorForUnhashableObject(object obj)
  {
    return PythonOps.TypeErrorForUnhashableType(PythonTypeOps.GetName(obj));
  }

  internal static Exception TypeErrorForIncompatibleObjectLayout(
    string prefix,
    PythonType type,
    Type newType)
  {
    return PythonOps.TypeError("{0}: '{1}' object layout differs from '{2}'", (object) prefix, (object) type.Name, (object) newType);
  }

  public static Exception TypeErrorForNonStringAttribute()
  {
    return PythonOps.TypeError("attribute name must be string");
  }

  internal static Exception TypeErrorForBadInstance(string template, object instance)
  {
    return PythonOps.TypeError(template, (object) PythonOps.GetPythonTypeName(instance));
  }

  public static Exception TypeErrorForBinaryOp(string opSymbol, object x, object y)
  {
    throw PythonOps.TypeError("unsupported operand type(s) for {0}: '{1}' and '{2}'", (object) opSymbol, (object) PythonOps.GetPythonTypeName(x), (object) PythonOps.GetPythonTypeName(y));
  }

  public static Exception TypeErrorForUnaryOp(string opSymbol, object x)
  {
    throw PythonOps.TypeError("unsupported operand type for {0}: '{1}'", (object) opSymbol, (object) PythonOps.GetPythonTypeName(x));
  }

  public static Exception TypeErrorForNonIterableObject(object o)
  {
    return PythonOps.TypeError("argument of type '{0}' is not iterable", (object) PythonTypeOps.GetName(o));
  }

  public static Exception TypeErrorForDefaultArgument(string message)
  {
    return PythonOps.TypeError(message);
  }

  public static Exception AttributeErrorForReadonlyAttribute(string typeName, string attributeName)
  {
    if (attributeName == "__class__")
      return PythonOps.TypeError("can't delete __class__ attribute");
    return PythonOps.AttributeError("'{1}' object attribute '{0}' is read-only", (object) attributeName, (object) typeName);
  }

  public static Exception AttributeErrorForBuiltinAttributeDeletion(
    string typeName,
    string attributeName)
  {
    return PythonOps.AttributeError("cannot delete attribute '{0}' of builtin type '{1}'", (object) attributeName, (object) typeName);
  }

  public static Exception MissingInvokeMethodException(object o, string name)
  {
    if (o is OldClass)
      throw PythonOps.AttributeError("type object '{0}' has no attribute '{1}'", (object) ((OldClass) o).Name, (object) name);
    throw PythonOps.AttributeError("'{0}' object has no attribute '{1}'", (object) PythonOps.GetPythonTypeName(o), (object) name);
  }

  internal static Exception MakeExceptionTypeError(object type, bool forGenerator = false)
  {
    return PythonOps.TypeError(forGenerator ? "exceptions must be classes, or instances, not {0}" : "exceptions must be old-style classes or derived from BaseException, not {0}", (object) PythonTypeOps.GetName(type));
  }

  public static Exception AttributeErrorForObjectMissingAttribute(object obj, string attributeName)
  {
    switch (obj)
    {
      case OldInstance _:
        return PythonOps.AttributeErrorForOldInstanceMissingAttribute(((OldInstance) obj)._class.Name, attributeName);
      case OldClass _:
        return PythonOps.AttributeErrorForOldClassMissingAttribute(((OldClass) obj).Name, attributeName);
      default:
        return PythonOps.AttributeErrorForMissingAttribute(PythonTypeOps.GetName(obj), attributeName);
    }
  }

  public static Exception AttributeErrorForMissingAttribute(string typeName, string attributeName)
  {
    return PythonOps.AttributeError("'{0}' object has no attribute '{1}'", (object) typeName, (object) attributeName);
  }

  public static Exception AttributeErrorForOldInstanceMissingAttribute(
    string typeName,
    string attributeName)
  {
    return PythonOps.AttributeError("{0} instance has no attribute '{1}'", (object) typeName, (object) attributeName);
  }

  public static Exception AttributeErrorForOldClassMissingAttribute(
    string typeName,
    string attributeName)
  {
    return PythonOps.AttributeError("class {0} has no attribute '{1}'", (object) typeName, (object) attributeName);
  }

  public static Exception UncallableError(object func)
  {
    return PythonOps.TypeError("{0} is not callable", (object) PythonTypeOps.GetName(func));
  }

  public static Exception TypeErrorForProtectedMember(Type type, string name)
  {
    return PythonOps.TypeError("cannot access protected member {0} without a python subclass of {1}", (object) name, (object) NameConverter.GetTypeName(type));
  }

  public static Exception TypeErrorForGenericMethod(Type type, string name)
  {
    return PythonOps.TypeError("{0}.{1} is a generic method and must be indexed with types before calling", (object) NameConverter.GetTypeName(type), (object) name);
  }

  public static Exception TypeErrorForUnIndexableObject(object o)
  {
    if (o == null)
      return PythonOps.TypeError("'NoneType' object cannot be interpreted as an index");
    if (!(o is IPythonObject pythonObject))
      return PythonOps.TypeError("object cannot be interpreted as an index");
    return PythonOps.TypeError("'{0}' object cannot be interpreted as an index", (object) pythonObject.PythonType.Name);
  }

  [Obsolete("no longer used anywhere")]
  public static Exception TypeErrorForBadDictionaryArgument(PythonFunction f)
  {
    return PythonOps.TypeError("{0}() argument after ** must be a dictionary", (object) f.__name__);
  }

  public static T TypeErrorForBadEnumConversion<T>(object value)
  {
    throw PythonOps.TypeError("Cannot convert numeric value {0} to {1}.  The value must be zero.", value, (object) NameConverter.GetTypeName(typeof (T)));
  }

  public static Exception UnreadableProperty() => PythonOps.AttributeError("unreadable attribute");

  public static Exception UnsetableProperty() => PythonOps.AttributeError("readonly attribute");

  public static Exception UndeletableProperty()
  {
    return PythonOps.AttributeError("undeletable attribute");
  }

  public static Exception Warning(string format, params object[] args)
  {
    return (Exception) new WarningException(string.Format(format, args));
  }

  public static List<FunctionStack> GetFunctionStack()
  {
    return PythonOps._funcStack ?? (PythonOps._funcStack = new List<FunctionStack>());
  }

  public static List<FunctionStack> GetFunctionStackNoCreate() => PythonOps._funcStack;

  public static List<FunctionStack> PushFrame(CodeContext context, FunctionCode function)
  {
    List<FunctionStack> functionStack = PythonOps.GetFunctionStack();
    functionStack.Add(new FunctionStack(context, function));
    return functionStack;
  }

  internal static LightLambdaExpression ToGenerator(
    this LightLambdaExpression code,
    bool shouldInterpret,
    bool debuggable,
    int compilationThreshold)
  {
    return Microsoft.Scripting.Ast.Utils.LightLambda(typeof (object), code.Type, new GeneratorRewriter(code.Name, code.Body).Reduce(shouldInterpret, debuggable, compilationThreshold, code.Parameters, (Func<Expression<Func<MutableTuple, object>>, Expression<Func<MutableTuple, object>>>) (x => x)), code.Name, code.Parameters);
  }

  public static void UpdateStackTrace(
    Exception e,
    CodeContext context,
    FunctionCode funcCode,
    int line)
  {
    if (line == -1)
      return;
    List<DynamicStackFrame> dynamicStackFrameList = e.GetFrameList();
    if (dynamicStackFrameList == null)
      e.SetFrameList(dynamicStackFrameList = new List<DynamicStackFrame>());
    PythonDynamicStackFrame dynamicStackFrame = new PythonDynamicStackFrame(context, funcCode, line);
    funcCode.LightThrowCompile(context);
    dynamicStackFrameList.Add((DynamicStackFrame) dynamicStackFrame);
  }

  public static DynamicStackFrame[] GetDynamicStackFrames(Exception e)
  {
    return PythonExceptions.GetDynamicStackFrames(e);
  }

  public static byte[] ConvertBufferToByteArray(PythonBuffer buffer)
  {
    return buffer.ToString().MakeByteArray();
  }

  public static bool ModuleTryGetMember(
    CodeContext context,
    PythonModule module,
    string name,
    out object res)
  {
    object attributeNoThrow = module.GetAttributeNoThrow(context, name);
    if (attributeNoThrow != OperationFailed.Value)
    {
      res = attributeNoThrow;
      return true;
    }
    res = (object) null;
    return false;
  }

  internal static void ScopeSetMember(CodeContext context, Scope scope, string name, object value)
  {
    if (scope.Storage is ScopeStorage storage)
      storage.SetValue(name, false, value);
    else
      PythonOps.SetAttr(context, (object) scope, name, value);
  }

  internal static object ScopeGetMember(CodeContext context, Scope scope, string name)
  {
    return scope.Storage is ScopeStorage storage ? storage.GetValue(name, false) : PythonOps.GetBoundAttr(context, (object) scope, name);
  }

  internal static bool ScopeTryGetMember(
    CodeContext context,
    Scope scope,
    string name,
    out object value)
  {
    return scope.Storage is ScopeStorage storage ? storage.TryGetValue(name, false, out value) : PythonOps.TryGetBoundAttr(context, (object) scope, name, out value);
  }

  internal static bool ScopeContainsMember(CodeContext context, Scope scope, string name)
  {
    return scope.Storage is ScopeStorage storage ? storage.HasValue(name, false) : PythonOps.HasAttr(context, (object) scope, name);
  }

  internal static bool ScopeDeleteMember(CodeContext context, Scope scope, string name)
  {
    if (scope.Storage is ScopeStorage storage)
      return storage.DeleteValue(name, false);
    int num = PythonOps.HasAttr(context, (object) scope, name) ? 1 : 0;
    PythonOps.DeleteAttr(context, (object) scope, name);
    return num != 0;
  }

  internal static IList<object> ScopeGetMemberNames(CodeContext context, Scope scope)
  {
    if (!(scope.Storage is ScopeStorage storage))
      return PythonOps.GetAttrNames(context, (object) scope);
    List<object> memberNames = new List<object>();
    foreach (string memberName in (IEnumerable<string>) storage.GetMemberNames())
      memberNames.Add((object) memberName);
    Dictionary<object, object> objectKeys = ((PythonScopeExtension) context.LanguageContext.EnsureScopeExtension(scope)).ObjectKeys;
    if (objectKeys != null)
    {
      foreach (object key in objectKeys.Keys)
        memberNames.Add(key);
    }
    return (IList<object>) memberNames;
  }

  public static bool IsUnicode(object unicodeObj) => unicodeObj == TypeCache.String;

  public static BuiltinFunction GetUnicodeFuntion() => UnicodeHelper.Function;

  public static bool IsExtensionSet(CodeContext codeContext, int id)
  {
    return codeContext.ModuleContext.ExtensionMethods.Id == id;
  }

  public static object GetExtensionMethodSet(CodeContext context)
  {
    return (object) context.ModuleContext.ExtensionMethods;
  }

  public static Exception ImportError(string format, params object[] args)
  {
    return (Exception) new ImportException(string.Format(format, args));
  }

  public static Exception RuntimeError(string format, params object[] args)
  {
    return (Exception) new RuntimeException(string.Format(format, args));
  }

  public static Exception UnicodeTranslateError(string format, params object[] args)
  {
    return (Exception) new UnicodeTranslateException(string.Format(format, args));
  }

  public static Exception PendingDeprecationWarning(string format, params object[] args)
  {
    return (Exception) new PendingDeprecationWarningException(string.Format(format, args));
  }

  public static Exception EnvironmentError(string format, params object[] args)
  {
    return (Exception) new EnvironmentException(string.Format(format, args));
  }

  public static Exception LookupError(string format, params object[] args)
  {
    return (Exception) new LookupException(string.Format(format, args));
  }

  public static Exception OSError(string format, params object[] args)
  {
    return (Exception) new OSException(string.Format(format, args));
  }

  public static Exception DeprecationWarning(string format, params object[] args)
  {
    return (Exception) new DeprecationWarningException(string.Format(format, args));
  }

  public static Exception UnicodeError(string format, params object[] args)
  {
    return (Exception) new UnicodeException(string.Format(format, args));
  }

  public static Exception FloatingPointError(string format, params object[] args)
  {
    return (Exception) new FloatingPointException(string.Format(format, args));
  }

  public static Exception ReferenceError(string format, params object[] args)
  {
    return (Exception) new ReferenceException(string.Format(format, args));
  }

  public static Exception FutureWarning(string format, params object[] args)
  {
    return (Exception) new FutureWarningException(string.Format(format, args));
  }

  public static Exception AssertionError(string format, params object[] args)
  {
    return (Exception) new AssertionException(string.Format(format, args));
  }

  public static Exception RuntimeWarning(string format, params object[] args)
  {
    return (Exception) new RuntimeWarningException(string.Format(format, args));
  }

  public static Exception ImportWarning(string format, params object[] args)
  {
    return (Exception) new ImportWarningException(string.Format(format, args));
  }

  public static Exception UserWarning(string format, params object[] args)
  {
    return (Exception) new UserWarningException(string.Format(format, args));
  }

  public static Exception SyntaxWarning(string format, params object[] args)
  {
    return (Exception) new SyntaxWarningException(string.Format(format, args));
  }

  public static Exception UnicodeWarning(string format, params object[] args)
  {
    return (Exception) new UnicodeWarningException(string.Format(format, args));
  }

  public static Exception StopIteration(string format, params object[] args)
  {
    return (Exception) new StopIterationException(string.Format(format, args));
  }

  public static Exception BytesWarning(string format, params object[] args)
  {
    return (Exception) new BytesWarningException(string.Format(format, args));
  }

  public static Exception BufferError(string format, params object[] args)
  {
    return (Exception) new BufferException(string.Format(format, args));
  }

  internal delegate T MultiplySequenceWorker<T>(T self, int count);
}
