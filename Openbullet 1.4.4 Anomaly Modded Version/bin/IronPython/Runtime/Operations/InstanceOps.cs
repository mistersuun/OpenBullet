// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.InstanceOps
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
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class InstanceOps
{
  private static BuiltinFunction _New;
  internal static readonly BuiltinFunction NewCls = InstanceOps.CreateFunction("__new__", "DefaultNew", "DefaultNewClsKW");
  internal static readonly BuiltinFunction OverloadedNew = InstanceOps.CreateFunction("__new__", "OverloadedNewBasic", "OverloadedNewKW", "OverloadedNewClsKW");
  internal static readonly BuiltinFunction NonDefaultNewInst = InstanceOps.CreateNonDefaultNew();
  internal static BuiltinMethodDescriptor _Init;
  internal const string ObjectNewNoParameters = "object.__new__() takes no parameters";

  internal static BuiltinMethodDescriptor Init
  {
    get
    {
      if (InstanceOps._Init == null)
        InstanceOps._Init = InstanceOps.GetInitMethod();
      return InstanceOps._Init;
    }
  }

  internal static BuiltinFunction New
  {
    get
    {
      if (InstanceOps._New == null)
        InstanceOps._New = (BuiltinFunction) PythonTypeOps.GetSlot(PythonTypeInfo.GetExtensionMemberGroup(typeof (object), typeof (ObjectOps).GetMember("__new__")), "__new__", false);
      return InstanceOps._New;
    }
  }

  internal static BuiltinFunction CreateNonDefaultNew()
  {
    return InstanceOps.CreateFunction("__new__", "NonDefaultNew", "NonDefaultNewKW", "NonDefaultNewKWNoParams");
  }

  public static object DefaultNew(CodeContext context, PythonType typeø, params object[] argsø)
  {
    if (typeø == null)
      throw PythonOps.TypeError("__new__ expected type object, got {0}", (object) PythonOps.Repr(context, (object) DynamicHelpers.GetPythonType((object) typeø)));
    InstanceOps.CheckNewArgs(context, (IDictionary<object, object>) null, argsø, typeø);
    return typeø.CreateInstance(context);
  }

  public static object DefaultNewClsKW(
    CodeContext context,
    PythonType typeø,
    [ParamDictionary] IDictionary<object, object> kwargsø,
    params object[] argsø)
  {
    object o = InstanceOps.DefaultNew(context, typeø, argsø);
    if (kwargsø.Count > 0)
    {
      foreach (KeyValuePair<object, object> keyValuePair in (IEnumerable<KeyValuePair<object, object>>) kwargsø)
        PythonOps.SetAttr(context, o, keyValuePair.Key.ToString(), keyValuePair.Value);
    }
    return o;
  }

  public static object OverloadedNewBasic(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], object>>> storage,
    BuiltinFunction overloadsø,
    PythonType typeø,
    params object[] argsø)
  {
    if (typeø == null)
      throw PythonOps.TypeError("__new__ expected type object, got {0}", (object) PythonOps.Repr(context, (object) DynamicHelpers.GetPythonType((object) typeø)));
    if (argsø == null)
      argsø = new object[1];
    return overloadsø.Call(context, storage, (object) null, argsø);
  }

  public static object OverloadedNewKW(
    CodeContext context,
    BuiltinFunction overloadsø,
    PythonType typeø,
    [ParamDictionary] IDictionary<object, object> kwargsø)
  {
    if (typeø == null)
      throw PythonOps.TypeError("__new__ expected type object, got {0}", (object) PythonOps.Repr(context, (object) DynamicHelpers.GetPythonType((object) typeø)));
    return overloadsø.Call(context, (SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>>>) null, (object) null, ArrayUtils.EmptyObjects, kwargsø);
  }

  public static object OverloadedNewClsKW(
    CodeContext context,
    BuiltinFunction overloadsø,
    PythonType typeø,
    [ParamDictionary] IDictionary<object, object> kwargsø,
    params object[] argsø)
  {
    if (typeø == null)
      throw PythonOps.TypeError("__new__ expected type object, got {0}", (object) PythonOps.Repr(context, (object) DynamicHelpers.GetPythonType((object) typeø)));
    if (argsø == null)
      argsø = new object[1];
    return overloadsø.Call(context, (SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>>>) null, (object) null, argsø, kwargsø);
  }

  public static void DefaultInit(CodeContext context, object self, params object[] argsø)
  {
  }

  public static void DefaultInitKW(
    CodeContext context,
    object self,
    [ParamDictionary] IDictionary<object, object> kwargsø,
    params object[] argsø)
  {
  }

  [StaticExtensionMethod]
  public static object NonDefaultNew(CodeContext context, PythonType typeø, params object[] argsø)
  {
    if (typeø == null)
      throw PythonOps.TypeError("__new__ expected type object, got {0}", (object) PythonOps.Repr(context, (object) DynamicHelpers.GetPythonType((object) typeø)));
    if (argsø == null)
      argsø = new object[1];
    return typeø.CreateInstance(context, argsø);
  }

  [StaticExtensionMethod]
  public static object NonDefaultNewKW(
    CodeContext context,
    PythonType typeø,
    [ParamDictionary] IDictionary<object, object> kwargsø,
    params object[] argsø)
  {
    if (typeø == null)
      throw PythonOps.TypeError("__new__ expected type object, got {0}", (object) PythonOps.Repr(context, (object) DynamicHelpers.GetPythonType((object) typeø)));
    if (argsø == null)
      argsø = new object[1];
    string[] names;
    InstanceOps.GetKeywordArgs(kwargsø, argsø, out argsø, out names);
    return typeø.CreateInstance(context, argsø, names);
  }

  [StaticExtensionMethod]
  public static object NonDefaultNewKWNoParams(
    CodeContext context,
    PythonType typeø,
    [ParamDictionary] IDictionary<object, object> kwargsø)
  {
    if (typeø == null)
      throw PythonOps.TypeError("__new__ expected type object, got {0}", (object) PythonOps.Repr(context, (object) DynamicHelpers.GetPythonType((object) typeø)));
    object[] finalArgs;
    string[] names;
    InstanceOps.GetKeywordArgs(kwargsø, ArrayUtils.EmptyObjects, out finalArgs, out names);
    return typeø.CreateInstance(context, finalArgs, names);
  }

  public static object IterMethodForString(string self)
  {
    return (object) StringOps.StringEnumerator(self);
  }

  public static object IterMethodForBytes(Bytes self)
  {
    return (object) IListOfByteOps.BytesIntEnumerator((IList<byte>) self);
  }

  public static object IterMethodForEnumerator(IEnumerator self) => (object) self;

  public static object IterMethodForEnumerable(IEnumerable self) => (object) self.GetEnumerator();

  public static object IterMethodForGenericEnumerator<T>(IEnumerator<T> self) => (object) self;

  public static object IterMethodForGenericEnumerable<T>(IEnumerable<T> self)
  {
    return (object) self.GetEnumerator();
  }

  public static object NextMethod(object self)
  {
    IEnumerator enumerator = (IEnumerator) self;
    lock (enumerator)
    {
      if (enumerator.MoveNext())
        return enumerator.Current;
    }
    throw PythonOps.StopIteration();
  }

  public static List DynamicDir(CodeContext context, IDynamicMetaObjectProvider self)
  {
    List list = new List((object) self.GetMetaObject((Expression) Expression.Parameter(typeof (object))).GetDynamicMemberNames());
    Type type = self.GetType();
    while (typeof (IDynamicMetaObjectProvider).IsAssignableFrom(type))
      type = type.BaseType;
    list.extend(DynamicHelpers.GetPythonTypeFromType(type).GetMemberNames(context));
    list.sort(context);
    return list;
  }

  public static int LengthMethod(ICollection self) => self.Count;

  public static int GenericLengthMethod<T>(ICollection<T> self) => self.Count;

  public static string SimpleRepr(object self)
  {
    return $"<{PythonTypeOps.GetName(self)} object at {PythonOps.HexId(self)}>";
  }

  public static string FancyRepr(object self)
  {
    PythonType pythonType = DynamicHelpers.GetPythonType(self);
    if (!pythonType.IsSystemType)
      return InstanceOps.SimpleRepr(self);
    string str1 = self.ToString() ?? string.Empty;
    string fullName = pythonType.UnderlyingSystemType.FullName;
    int num = 0;
    while (num < str1.Length && (str1[num] == '\r' || str1[num] == '\n'))
      ++num;
    int index1 = num;
    while (index1 < str1.Length && str1[index1] != '\r' && str1[index1] != '\n')
      ++index1;
    int index2 = index1;
    while (index2 < str1.Length && (str1[index2] == '\r' || str1[index2] == '\n'))
      ++index2;
    if (index1 <= num)
      return $"<{fullName} object at {PythonOps.HexId(self)}>";
    string str2 = str1.Substring(num, index1 - num);
    bool flag = index2 < str1.Length;
    return $"<{fullName} object at {PythonOps.HexId(self)} [{str2}{(flag ? (object) "..." : (object) string.Empty)}]>";
  }

  public static object ReprHelper(CodeContext context, object self)
  {
    return (object) ((ICodeFormattable) self).__repr__(context);
  }

  public static string ToStringMethod(object self) => self.ToString() ?? string.Empty;

  public static string Format(IFormattable formattable, string format)
  {
    return formattable.ToString(format, (IFormatProvider) null);
  }

  public static bool EqualsMethod(object x, object y) => x.Equals(y);

  public static bool NotEqualsMethod(object x, object y) => !x.Equals(y);

  public static int StructuralHashMethod(CodeContext context, IStructuralEquatable x)
  {
    return x.GetHashCode(context.LanguageContext.EqualityComparerNonGeneric);
  }

  public static bool StructuralEqualityMethod<T>(CodeContext context, T x, [NotNull] T y) where T : IStructuralEquatable
  {
    return x.Equals((object) y, context.LanguageContext.EqualityComparerNonGeneric);
  }

  public static bool StructuralInequalityMethod<T>(CodeContext context, T x, [NotNull] T y) where T : IStructuralEquatable
  {
    return !x.Equals((object) y, context.LanguageContext.EqualityComparerNonGeneric);
  }

  [return: MaybeNotImplemented]
  public static object StructuralEqualityMethod<T>(CodeContext context, [NotNull] T x, object y) where T : IStructuralEquatable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(x.Equals(y, context.LanguageContext.EqualityComparerNonGeneric));
  }

  [return: MaybeNotImplemented]
  public static object StructuralInequalityMethod<T>(CodeContext context, [NotNull] T x, object y) where T : IStructuralEquatable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(!x.Equals(y, context.LanguageContext.EqualityComparerNonGeneric));
  }

  [return: MaybeNotImplemented]
  public static object StructuralEqualityMethod<T>(CodeContext context, object y, [NotNull] T x) where T : IStructuralEquatable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(x.Equals(y, context.LanguageContext.EqualityComparerNonGeneric));
  }

  [return: MaybeNotImplemented]
  public static object StructuralInequalityMethod<T>(CodeContext context, object y, [NotNull] T x) where T : IStructuralEquatable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(!x.Equals(y, context.LanguageContext.EqualityComparerNonGeneric));
  }

  private static int StructuralCompare(CodeContext context, IStructuralComparable x, object y)
  {
    return x.CompareTo(y, context.LanguageContext.GetComparer((object) null, (Type) null));
  }

  public static bool StructuralComparableEquality<T>(CodeContext context, T x, [NotNull] T y) where T : IStructuralComparable
  {
    return InstanceOps.StructuralCompare(context, (IStructuralComparable) x, (object) y) == 0;
  }

  public static bool StructuralComparableInequality<T>(CodeContext context, T x, [NotNull] T y) where T : IStructuralComparable
  {
    return InstanceOps.StructuralCompare(context, (IStructuralComparable) x, (object) y) != 0;
  }

  public static bool StructuralComparableGreaterThan<T>(CodeContext context, T x, [NotNull] T y) where T : IStructuralComparable
  {
    return InstanceOps.StructuralCompare(context, (IStructuralComparable) x, (object) y) > 0;
  }

  public static bool StructuralComparableLessThan<T>(CodeContext context, T x, [NotNull] T y) where T : IStructuralComparable
  {
    return InstanceOps.StructuralCompare(context, (IStructuralComparable) x, (object) y) < 0;
  }

  public static bool StructuralComparableGreaterEqual<T>(CodeContext context, T x, [NotNull] T y) where T : IStructuralComparable
  {
    return InstanceOps.StructuralCompare(context, (IStructuralComparable) x, (object) y) >= 0;
  }

  public static bool StructuralComparableLessEqual<T>(CodeContext context, T x, [NotNull] T y) where T : IStructuralComparable
  {
    return InstanceOps.StructuralCompare(context, (IStructuralComparable) x, (object) y) <= 0;
  }

  [return: MaybeNotImplemented]
  public static object StructuralComparableEquality<T>(CodeContext context, [NotNull] T x, object y) where T : IStructuralComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(InstanceOps.StructuralCompare(context, (IStructuralComparable) x, y) == 0);
  }

  [return: MaybeNotImplemented]
  public static object StructuralComparableInequality<T>(CodeContext context, [NotNull] T x, object y) where T : IStructuralComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(InstanceOps.StructuralCompare(context, (IStructuralComparable) x, y) != 0);
  }

  [return: MaybeNotImplemented]
  public static object StructuralComparableGreaterThan<T>(CodeContext context, [NotNull] T x, object y) where T : IStructuralComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(InstanceOps.StructuralCompare(context, (IStructuralComparable) x, y) > 0);
  }

  [return: MaybeNotImplemented]
  public static object StructuralComparableLessThan<T>(CodeContext context, [NotNull] T x, object y) where T : IStructuralComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(InstanceOps.StructuralCompare(context, (IStructuralComparable) x, y) < 0);
  }

  [return: MaybeNotImplemented]
  public static object StructuralComparableGreaterEqual<T>(CodeContext context, [NotNull] T x, object y) where T : IStructuralComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(InstanceOps.StructuralCompare(context, (IStructuralComparable) x, y) >= 0);
  }

  [return: MaybeNotImplemented]
  public static object StructuralComparableLessEqual<T>(CodeContext context, [NotNull] T x, object y) where T : IStructuralComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(InstanceOps.StructuralCompare(context, (IStructuralComparable) x, y) <= 0);
  }

  [return: MaybeNotImplemented]
  public static object StructuralComparableEquality<T>(CodeContext context, object y, [NotNull] T x) where T : IStructuralComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(InstanceOps.StructuralCompare(context, (IStructuralComparable) x, y) == 0);
  }

  [return: MaybeNotImplemented]
  public static object StructuralComparableInequality<T>(CodeContext context, object y, [NotNull] T x) where T : IStructuralComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(InstanceOps.StructuralCompare(context, (IStructuralComparable) x, y) != 0);
  }

  [return: MaybeNotImplemented]
  public static object StructuralComparableGreaterThan<T>(CodeContext context, object y, [NotNull] T x) where T : IStructuralComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(InstanceOps.StructuralCompare(context, (IStructuralComparable) x, y) < 0);
  }

  [return: MaybeNotImplemented]
  public static object StructuralComparableLessThan<T>(CodeContext context, object y, [NotNull] T x) where T : IStructuralComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(InstanceOps.StructuralCompare(context, (IStructuralComparable) x, y) > 0);
  }

  [return: MaybeNotImplemented]
  public static object StructuralComparableGreaterEqual<T>(CodeContext context, object y, [NotNull] T x) where T : IStructuralComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(InstanceOps.StructuralCompare(context, (IStructuralComparable) x, y) <= 0);
  }

  [return: MaybeNotImplemented]
  public static object StructuralComparableLessEqual<T>(CodeContext context, object y, [NotNull] T x) where T : IStructuralComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(InstanceOps.StructuralCompare(context, (IStructuralComparable) x, y) >= 0);
  }

  public static bool ComparableEquality<T>(T x, [NotNull] T y) where T : IComparable
  {
    return x.CompareTo((object) y) == 0;
  }

  public static bool ComparableInequality<T>(T x, [NotNull] T y) where T : IComparable
  {
    return x.CompareTo((object) y) != 0;
  }

  public static bool ComparableGreaterThan<T>(T x, [NotNull] T y) where T : IComparable
  {
    return x.CompareTo((object) y) > 0;
  }

  public static bool ComparableLessThan<T>(T x, [NotNull] T y) where T : IComparable
  {
    return x.CompareTo((object) y) < 0;
  }

  public static bool ComparableGreaterEqual<T>(T x, [NotNull] T y) where T : IComparable
  {
    return x.CompareTo((object) y) >= 0;
  }

  public static bool ComparableLessEqual<T>(T x, [NotNull] T y) where T : IComparable
  {
    return x.CompareTo((object) y) <= 0;
  }

  [return: MaybeNotImplemented]
  public static object ComparableEquality<T>([NotNull] T x, object y) where T : IComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(x.CompareTo(y) == 0);
  }

  [return: MaybeNotImplemented]
  public static object ComparableInequality<T>([NotNull] T x, object y) where T : IComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(x.CompareTo(y) != 0);
  }

  [return: MaybeNotImplemented]
  public static object ComparableGreaterThan<T>([NotNull] T x, object y) where T : IComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(x.CompareTo(y) > 0);
  }

  [return: MaybeNotImplemented]
  public static object ComparableLessThan<T>([NotNull] T x, object y) where T : IComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(x.CompareTo(y) < 0);
  }

  [return: MaybeNotImplemented]
  public static object ComparableGreaterEqual<T>([NotNull] T x, object y) where T : IComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(x.CompareTo(y) >= 0);
  }

  [return: MaybeNotImplemented]
  public static object ComparableLessEqual<T>([NotNull] T x, object y) where T : IComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(x.CompareTo(y) <= 0);
  }

  [return: MaybeNotImplemented]
  public static object ComparableEquality<T>(object y, [NotNull] T x) where T : IComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(x.CompareTo(y) == 0);
  }

  [return: MaybeNotImplemented]
  public static object ComparableInequality<T>(object y, [NotNull] T x) where T : IComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(x.CompareTo(y) != 0);
  }

  [return: MaybeNotImplemented]
  public static object ComparableGreaterThan<T>(object y, [NotNull] T x) where T : IComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(x.CompareTo(y) < 0);
  }

  [return: MaybeNotImplemented]
  public static object ComparableLessThan<T>(object y, [NotNull] T x) where T : IComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(x.CompareTo(y) > 0);
  }

  [return: MaybeNotImplemented]
  public static object ComparableGreaterEqual<T>(object y, [NotNull] T x) where T : IComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(x.CompareTo(y) <= 0);
  }

  [return: MaybeNotImplemented]
  public static object ComparableLessEqual<T>(object y, [NotNull] T x) where T : IComparable
  {
    return !(y is T) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(x.CompareTo(y) >= 0);
  }

  public static object EnterMethod(IDisposable self) => (object) self;

  public static void ExitMethod(
    IDisposable self,
    object exc_type,
    object exc_value,
    object exc_back)
  {
    self.Dispose();
  }

  [PropertyMethod]
  [StaticExtensionMethod]
  public static List Get__all__<T>(CodeContext context)
  {
    PythonType pythonTypeFromType = DynamicHelpers.GetPythonTypeFromType(typeof (T));
    List all = new List();
    foreach (string memberName in pythonTypeFromType.GetMemberNames(context))
    {
      if (InstanceOps.IsStaticTypeMemberInAll(context, pythonTypeFromType, memberName, out object _))
        all.AddNoLock((object) memberName);
    }
    return all;
  }

  private static bool IsStaticTypeMemberInAll(
    CodeContext context,
    PythonType pt,
    string name,
    out object res)
  {
    res = (object) null;
    PythonTypeSlot slot;
    if (pt.TryResolveSlot(context, name, out slot))
    {
      if (name == "__doc__" || name == "__class__" || slot is ReflectedGetterSetter || slot is ReflectedField reflectedField && !reflectedField._info.IsInitOnly && !reflectedField._info.IsLiteral || slot is BuiltinMethodDescriptor methodDescriptor && (!methodDescriptor.DeclaringType.IsSealed() || !methodDescriptor.DeclaringType.IsAbstract()) || slot is BuiltinFunction builtinFunction && (!builtinFunction.DeclaringType.IsSealed() || !builtinFunction.DeclaringType.IsAbstract()))
        return false;
      if (slot.TryGetValue(context, (object) null, pt, out res))
        return true;
    }
    res = (object) null;
    return false;
  }

  public static bool ContainsGenericMethod<T>(
    CodeContext context,
    IEnumerable<T> enumerable,
    T value)
  {
    foreach (T x in enumerable)
    {
      if (PythonOps.EqualRetBool(context, (object) x, (object) value))
        return true;
    }
    return false;
  }

  public static bool ContainsMethod(CodeContext context, IEnumerable enumerable, object value)
  {
    foreach (object x in enumerable)
    {
      if (PythonOps.EqualRetBool(context, x, value))
        return true;
    }
    return false;
  }

  public static bool ContainsGenericMethodIEnumerator<T>(
    CodeContext context,
    IEnumerator<T> enumerator,
    T value)
  {
    while (enumerator.MoveNext())
    {
      if (PythonOps.EqualRetBool(context, (object) enumerator.Current, (object) value))
        return true;
    }
    return false;
  }

  public static bool ContainsMethodIEnumerator(
    CodeContext context,
    IEnumerator enumerator,
    object value)
  {
    while (enumerator.MoveNext())
    {
      if (PythonOps.EqualRetBool(context, enumerator.Current, value))
        return true;
    }
    return false;
  }

  public static PythonTuple SerializeReduce(CodeContext context, object self, int protocol)
  {
    PythonTuple pythonTuple = ClrModule.Serialize(self);
    object obj;
    context.LanguageContext.ClrModule.__dict__.TryGetValue((object) "Deserialize", out obj);
    return PythonTuple.MakeTuple(obj, (object) pythonTuple, null);
  }

  internal static void CheckNewArgs(
    CodeContext context,
    IDictionary<object, object> dict,
    object[] args,
    PythonType pt)
  {
    if ((args == null || args.Length == 0) && (dict == null || dict.Count <= 0))
      return;
    bool flag1 = pt.HasObjectInit(context);
    bool flag2 = pt.HasObjectNew(context);
    if (flag1)
      throw PythonOps.TypeError("object.__new__() takes no parameters");
    if (flag2 || flag1)
      return;
    PythonOps.Warn(context, PythonExceptions.DeprecationWarning, "object.__new__() takes no parameters");
  }

  internal static void CheckInitArgs(
    CodeContext context,
    IDictionary<object, object> dict,
    object[] args,
    object self)
  {
    if ((args == null || args.Length == 0) && (dict == null || dict.Count <= 0))
      return;
    PythonType pythonType = DynamicHelpers.GetPythonType(self);
    bool flag1 = pythonType.HasObjectInit(context);
    bool flag2 = pythonType.HasObjectNew(context);
    if (flag2 && self != null)
      throw PythonOps.TypeError("object.__init__() takes no parameters");
    if ((flag2 || flag1) && self != null)
      return;
    PythonOps.Warn(context, PythonExceptions.DeprecationWarning, "object.__init__() takes no parameters");
  }

  private static BuiltinMethodDescriptor GetInitMethod()
  {
    PythonTypeSlot slot;
    TypeCache.Object.TryResolveSlot(DefaultContext.Default, "__init__", out slot);
    return (BuiltinMethodDescriptor) slot;
  }

  private static BuiltinFunction CreateFunction(string name, params string[] methodNames)
  {
    MethodBase[] infos = new MethodBase[methodNames.Length];
    for (int index = 0; index < infos.Length; ++index)
      infos[index] = (MethodBase) typeof (InstanceOps).GetMethod(methodNames[index]);
    return BuiltinFunction.MakeFunction(name, infos, typeof (object));
  }

  private static void GetKeywordArgs(
    IDictionary<object, object> dict,
    object[] args,
    out object[] finalArgs,
    out string[] names)
  {
    finalArgs = new object[args.Length + dict.Count];
    Array.Copy((Array) args, (Array) finalArgs, args.Length);
    names = new string[dict.Count];
    int index = 0;
    foreach (KeyValuePair<object, object> keyValuePair in (IEnumerable<KeyValuePair<object, object>>) dict)
    {
      names[index] = (string) keyValuePair.Key;
      finalArgs[index + args.Length] = keyValuePair.Value;
      ++index;
    }
  }
}
