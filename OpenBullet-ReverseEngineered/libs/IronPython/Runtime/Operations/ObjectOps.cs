// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.ObjectOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Operations;

public static class ObjectOps
{
  private static Dictionary<PythonType, object> _nativelyPickleableTypes;
  [SlotField]
  public static PythonTypeSlot __class__ = (PythonTypeSlot) new PythonTypeTypeSlot();

  public static void __delattr__(CodeContext context, object self, string name)
  {
    if (self is PythonType)
      throw PythonOps.TypeError("can't apply this __delattr__ to type object");
    PythonOps.ObjectDeleteAttribute(context, self, name);
  }

  public static int __hash__(object self) => self == null ? 505032256 : self.GetHashCode();

  public static object __getattribute__(CodeContext context, object self, string name)
  {
    return PythonOps.ObjectGetAttribute(context, self, name);
  }

  public static void __init__(CodeContext context, object self)
  {
  }

  public static void __init__(CodeContext context, object self, [NotNull] params object[] argsø)
  {
    InstanceOps.CheckInitArgs(context, (IDictionary<object, object>) null, argsø, self);
  }

  public static void __init__(
    CodeContext context,
    object self,
    [ParamDictionary] IDictionary<object, object> kwargs,
    params object[] argsø)
  {
    InstanceOps.CheckInitArgs(context, kwargs, argsø, self);
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls)
  {
    return cls != null ? cls.CreateInstance(context) : throw PythonOps.TypeError("__new__ expected type object, got {0}", (object) PythonOps.Repr(context, (object) DynamicHelpers.GetPythonType((object) cls)));
  }

  [StaticExtensionMethod]
  public static object __new__(CodeContext context, PythonType cls, [NotNull] params object[] argsø)
  {
    if (cls == null)
      throw PythonOps.TypeError("__new__ expected type object, got {0}", (object) PythonOps.Repr(context, (object) DynamicHelpers.GetPythonType((object) cls)));
    InstanceOps.CheckNewArgs(context, (IDictionary<object, object>) null, argsø, cls);
    return cls.CreateInstance(context);
  }

  [StaticExtensionMethod]
  public static object __new__(
    CodeContext context,
    PythonType cls,
    [ParamDictionary] IDictionary<object, object> kwargsø,
    [NotNull] params object[] argsø)
  {
    if (cls == null)
      throw PythonOps.TypeError("__new__ expected type object, got {0}", (object) PythonOps.Repr(context, (object) DynamicHelpers.GetPythonType((object) cls)));
    InstanceOps.CheckNewArgs(context, kwargsø, argsø, cls);
    return cls.CreateInstance(context);
  }

  public static object __reduce__(CodeContext context, object self)
  {
    return ObjectOps.__reduce_ex__(context, self, (object) 0);
  }

  public static object __reduce_ex__(CodeContext context, object self)
  {
    return ObjectOps.__reduce_ex__(context, self, (object) 0);
  }

  public static object __reduce_ex__(CodeContext context, object self, object protocol)
  {
    object boundAttr = PythonOps.GetBoundAttr(context, (object) DynamicHelpers.GetPythonTypeFromType(typeof (object)), "__reduce__");
    object ret;
    return PythonOps.TryGetBoundAttr(context, (object) DynamicHelpers.GetPythonType(self), "__reduce__", out ret) && !PythonOps.IsRetBool(ret, boundAttr) ? PythonOps.CallWithContext(context, ret, self) : (context.LanguageContext.ConvertToInt32(protocol) < 2 ? (object) ObjectOps.ReduceProtocol0(context, self) : (object) ObjectOps.ReduceProtocol2(context, self));
  }

  public static string __repr__(object self)
  {
    return $"<{DynamicHelpers.GetPythonType(self).Name} object at {PythonOps.HexId(self)}>";
  }

  public static void __setattr__(CodeContext context, object self, string name, object value)
  {
    if (self is PythonType)
      throw PythonOps.TypeError("can't apply this __setattr__ to type object");
    PythonOps.ObjectSetAttribute(context, self, name, value);
  }

  private static int AdjustPointerSize(int size) => IntPtr.Size == 4 ? size : size * 2;

  public static int __sizeof__(object self)
  {
    IPythonObject pythonObject = self as IPythonObject;
    int num = ObjectOps.AdjustPointerSize(8);
    if (pythonObject != null)
      num += ObjectOps.AdjustPointerSize(12);
    Type finalSystemType = DynamicHelpers.GetPythonType(self).FinalSystemType;
    return num + ObjectOps.GetTypeSize(finalSystemType);
  }

  private static int GetTypeSize(Type t)
  {
    FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
    int typeSize = 0;
    foreach (FieldInfo fieldInfo in fields)
    {
      if (fieldInfo.FieldType.IsClass || fieldInfo.FieldType.IsInterface)
      {
        typeSize += ObjectOps.AdjustPointerSize(4);
      }
      else
      {
        if (fieldInfo.FieldType.IsPrimitive)
          return Marshal.SizeOf(fieldInfo.FieldType);
        typeSize += ObjectOps.GetTypeSize(fieldInfo.FieldType);
      }
    }
    return typeSize;
  }

  public static string __str__(CodeContext context, object o) => PythonOps.Repr(context, o);

  public static NotImplementedType __subclasshook__(params object[] args)
  {
    return NotImplementedType.Value;
  }

  public static string __format__(CodeContext context, object self, [NotNull] string formatSpec)
  {
    string text = PythonOps.ToString(context, self);
    StringFormatSpec stringFormatSpec = StringFormatSpec.FromString(formatSpec);
    char? nullable1;
    int? nullable2;
    if (stringFormatSpec.Type.HasValue)
    {
      nullable1 = stringFormatSpec.Type;
      nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
      int num = 115;
      if (!(nullable2.GetValueOrDefault() == num & nullable2.HasValue))
        throw PythonOps.ValueError("Unknown format code '{0}' for object of type 'str'", (object) stringFormatSpec.Type.Value.ToString());
    }
    if (stringFormatSpec.Sign.HasValue)
      throw PythonOps.ValueError("Sign not allowed in string format specifier");
    nullable1 = stringFormatSpec.Alignment;
    nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
    int num1 = 61;
    if (nullable2.GetValueOrDefault() == num1 & nullable2.HasValue)
      throw PythonOps.ValueError("'=' alignment not allowed in string format specifier");
    if (stringFormatSpec.ThousandsComma)
      throw PythonOps.ValueError("Cannot specify ',' with 's'.");
    if (stringFormatSpec.IncludeType)
      throw PythonOps.ValueError("Alternate form (#) not allowed in string format specifier");
    if (stringFormatSpec.Precision.HasValue)
    {
      int length = stringFormatSpec.Precision.Value;
      if (text.Length > length)
        text = text.Substring(0, length);
    }
    return stringFormatSpec.AlignText(text);
  }

  private static Dictionary<PythonType, object> NativelyPickleableTypes
  {
    get
    {
      if (ObjectOps._nativelyPickleableTypes == null)
      {
        Dictionary<PythonType, object> dictionary = new Dictionary<PythonType, object>();
        dictionary.Add(TypeCache.Null, (object) null);
        dictionary.Add(DynamicHelpers.GetPythonTypeFromType(typeof (bool)), (object) null);
        dictionary.Add(DynamicHelpers.GetPythonTypeFromType(typeof (int)), (object) null);
        dictionary.Add(DynamicHelpers.GetPythonTypeFromType(typeof (double)), (object) null);
        dictionary.Add(DynamicHelpers.GetPythonTypeFromType(typeof (Complex)), (object) null);
        dictionary.Add(DynamicHelpers.GetPythonTypeFromType(typeof (string)), (object) null);
        dictionary.Add(DynamicHelpers.GetPythonTypeFromType(typeof (PythonTuple)), (object) null);
        dictionary.Add(DynamicHelpers.GetPythonTypeFromType(typeof (List)), (object) null);
        dictionary.Add(DynamicHelpers.GetPythonTypeFromType(typeof (PythonDictionary)), (object) null);
        dictionary.Add(DynamicHelpers.GetPythonTypeFromType(typeof (OldInstance)), (object) null);
        dictionary.Add(DynamicHelpers.GetPythonTypeFromType(typeof (OldClass)), (object) null);
        dictionary.Add(DynamicHelpers.GetPythonTypeFromType(typeof (PythonFunction)), (object) null);
        dictionary.Add(DynamicHelpers.GetPythonTypeFromType(typeof (BuiltinFunction)), (object) null);
        Thread.MemoryBarrier();
        ObjectOps._nativelyPickleableTypes = dictionary;
      }
      return ObjectOps._nativelyPickleableTypes;
    }
  }

  private static PythonDictionary GetInitializedSlotValues(object obj)
  {
    PythonDictionary pythonDictionary = new PythonDictionary();
    foreach (object o in (IEnumerable<PythonType>) DynamicHelpers.GetPythonType(obj).ResolutionOrder)
    {
      object ret1;
      if (PythonOps.TryGetBoundAttr(o, "__slots__", out ret1))
      {
        foreach (string str in PythonType.SlotsToList(ret1))
        {
          object ret2;
          if (!(str == "__dict__") && !pythonDictionary.__contains__((object) str) && PythonOps.TryGetBoundAttr(obj, str, out ret2))
            pythonDictionary[(object) str] = ret2;
        }
      }
    }
    return pythonDictionary.Count == 0 ? (PythonDictionary) null : pythonDictionary;
  }

  internal static PythonTuple ReduceProtocol0(CodeContext context, object self)
  {
    PythonType pythonType = DynamicHelpers.GetPythonType(self);
    ObjectOps.ThrowIfNativelyPickable(pythonType);
    object ret1;
    bool boundAttr = PythonOps.TryGetBoundAttr(context, self, "__getstate__", out ret1);
    object ret2;
    if (PythonOps.TryGetBoundAttr(context, (object) pythonType, "__slots__", out ret2) && PythonOps.Length(ret2) > 0 && !boundAttr)
      throw PythonOps.TypeError("a class that defines __slots__ without defining __getstate__ cannot be pickled with protocols 0 or 1");
    PythonType closestNonPythonBase = ObjectOps.FindClosestNonPythonBase(pythonType);
    object pythonReconstructor = (object) context.LanguageContext.PythonReconstructor;
    object obj = (object) PythonTuple.MakeTuple((object) pythonType, (object) closestNonPythonBase, TypeCache.Object == closestNonPythonBase ? (object) null : PythonCalls.Call(context, (object) closestNonPythonBase, self));
    object ret3;
    if (boundAttr)
      ret3 = PythonOps.CallWithContext(context, ret1);
    else if (self is IPythonObject pythonObject)
      ret3 = (object) pythonObject.Dict;
    else if (!PythonOps.TryGetBoundAttr(context, self, "__dict__", out ret3))
      ret3 = (object) null;
    if (!PythonOps.IsTrue(ret3))
      ret3 = (object) null;
    return PythonTuple.MakeTuple(pythonReconstructor, obj, ret3);
  }

  private static void ThrowIfNativelyPickable(PythonType type)
  {
    if (ObjectOps.NativelyPickleableTypes.ContainsKey(type))
      throw PythonOps.TypeError("can't pickle {0} objects", (object) type.Name);
  }

  private static PythonType FindClosestNonPythonBase(PythonType type)
  {
    foreach (PythonType closestNonPythonBase in (IEnumerable<PythonType>) type.ResolutionOrder)
    {
      if (closestNonPythonBase.IsSystemType)
        return closestNonPythonBase;
    }
    throw PythonOps.TypeError("can't pickle {0} instance: no non-Python bases found", (object) type.Name);
  }

  private static PythonTuple ReduceProtocol2(CodeContext context, object self)
  {
    PythonType pythonType = DynamicHelpers.GetPythonType(self);
    object newObject = (object) context.LanguageContext.NewObject;
    object ret1;
    object[] objArray;
    if (PythonOps.TryGetBoundAttr(context, (object) pythonType, "__getnewargs__", out ret1))
    {
      if (!(PythonOps.CallWithContext(context, ret1, self) is PythonTuple pythonTuple))
        throw PythonOps.TypeError("__getnewargs__ should return a tuple");
      objArray = new object[1 + pythonTuple.Count];
      objArray[0] = (object) pythonType;
      for (int index = 0; index < pythonTuple.Count; ++index)
        objArray[index + 1] = pythonTuple[index];
    }
    else
      objArray = new object[1]{ (object) pythonType };
    object obj1;
    if (!PythonTypeOps.TryInvokeUnaryOperator(context, self, "__getstate__", out obj1))
    {
      object ret2;
      if (self is IPythonObject pythonObject)
        ret2 = (object) pythonObject.Dict;
      else if (!PythonOps.TryGetBoundAttr(context, self, "__dict__", out ret2))
        ret2 = (object) null;
      PythonDictionary pythonDictionary = ObjectOps.GetInitializedSlotValues(self);
      if (pythonDictionary != null && pythonDictionary.Count == 0)
        pythonDictionary = (PythonDictionary) null;
      if (ret2 == null && pythonDictionary == null)
        obj1 = (object) null;
      else if (ret2 != null && pythonDictionary == null)
        obj1 = ret2;
      else if (ret2 != null && pythonDictionary != null)
        obj1 = (object) PythonTuple.MakeTuple(ret2, (object) pythonDictionary);
      else
        obj1 = (object) PythonTuple.MakeTuple(null, (object) pythonDictionary);
    }
    object obj2 = (object) null;
    if (self is List)
      obj2 = (object) PythonOps.GetEnumerator(self);
    object obj3 = (object) null;
    if (self is PythonDictionary)
      obj3 = PythonOps.Invoke(context, self, "iteritems", ArrayUtils.EmptyObjects);
    return PythonTuple.MakeTuple(newObject, (object) PythonTuple.MakeTuple(objArray), obj1, obj2, obj3);
  }
}
