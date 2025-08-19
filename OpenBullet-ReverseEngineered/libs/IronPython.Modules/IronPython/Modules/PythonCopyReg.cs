// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonCopyReg
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Modules;

[Documentation("Provides global reduction-function registration for pickling and copying objects.")]
public static class PythonCopyReg
{
  private static readonly object _dispatchTableKey = new object();
  private static readonly object _extensionRegistryKey = new object();
  private static readonly object _invertedRegistryKey = new object();
  private static readonly object _extensionCacheKey = new object();

  internal static PythonDictionary GetDispatchTable(CodeContext context)
  {
    PythonCopyReg.EnsureModuleInitialized(context);
    return (PythonDictionary) context.LanguageContext.GetModuleState(PythonCopyReg._dispatchTableKey);
  }

  internal static PythonDictionary GetExtensionRegistry(CodeContext context)
  {
    PythonCopyReg.EnsureModuleInitialized(context);
    return (PythonDictionary) context.LanguageContext.GetModuleState(PythonCopyReg._extensionRegistryKey);
  }

  internal static PythonDictionary GetInvertedRegistry(CodeContext context)
  {
    PythonCopyReg.EnsureModuleInitialized(context);
    return (PythonDictionary) context.LanguageContext.GetModuleState(PythonCopyReg._invertedRegistryKey);
  }

  internal static PythonDictionary GetExtensionCache(CodeContext context)
  {
    PythonCopyReg.EnsureModuleInitialized(context);
    return (PythonDictionary) context.LanguageContext.GetModuleState(PythonCopyReg._extensionCacheKey);
  }

  [Documentation("pickle(type, function[, constructor]) -> None\n\nAssociate function with type, indicating that function should be used to\n\"reduce\" objects of the given type when pickling. function should behave as\nspecified by the \"Extended __reduce__ API\" section of PEP 307.\n\nReduction functions registered by calling pickle() can be retrieved later\nthrough copy_reg.dispatch_table[type].\n\nNote that calling pickle() will overwrite any previous association for the\ngiven type.\n\nThe constructor argument is ignored, and exists only for backwards\ncompatibility.")]
  public static void pickle(CodeContext context, object type, object function, object ctor = null)
  {
    PythonCopyReg.EnsureCallable(context, function, "reduction functions must be callable");
    if (ctor != null)
      PythonCopyReg.constructor(context, ctor);
    PythonCopyReg.GetDispatchTable(context)[type] = function;
  }

  [Documentation("constructor(object) -> None\n\nRaise TypeError if object isn't callable. This function exists only for\nbackwards compatibility; for details, see\nhttp://mail.python.org/pipermail/python-dev/2006-June/066831.html.")]
  public static void constructor(CodeContext context, object callable)
  {
    PythonCopyReg.EnsureCallable(context, callable, "constructors must be callable");
  }

  private static void EnsureCallable(CodeContext context, object @object, string message)
  {
    if (!PythonOps.IsCallable(context, @object))
      throw PythonOps.TypeError(message);
  }

  [Documentation("pickle_complex(complex_number) -> (<type 'complex'>, (real, imag))\n\nReduction function for pickling complex numbers.")]
  public static PythonTuple pickle_complex(CodeContext context, object complex)
  {
    return PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonTypeFromType(typeof (Complex)), (object) PythonTuple.MakeTuple(PythonOps.GetBoundAttr(context, complex, "real"), PythonOps.GetBoundAttr(context, complex, "imag")));
  }

  public static void clear_extension_cache(CodeContext context)
  {
    PythonCopyReg.GetExtensionCache(context).clear();
  }

  [Documentation("Register an extension code.")]
  public static void add_extension(
    CodeContext context,
    object moduleName,
    object objectName,
    object value)
  {
    PythonTuple pythonTuple = PythonTuple.MakeTuple(moduleName, objectName);
    int code = PythonCopyReg.GetCode(context, value);
    bool flag1 = PythonCopyReg.GetExtensionRegistry(context).__contains__((object) pythonTuple);
    bool flag2 = PythonCopyReg.GetInvertedRegistry(context).__contains__((object) code);
    if (!flag1 && !flag2)
    {
      PythonCopyReg.GetExtensionRegistry(context)[(object) pythonTuple] = (object) code;
      PythonCopyReg.GetInvertedRegistry(context)[(object) code] = (object) pythonTuple;
    }
    else
    {
      if (flag1 & flag2 && PythonOps.EqualRetBool(context, PythonCopyReg.GetExtensionRegistry(context)[(object) pythonTuple], (object) code) && PythonOps.EqualRetBool(context, PythonCopyReg.GetInvertedRegistry(context)[(object) code], (object) pythonTuple))
        return;
      if (flag1)
        throw PythonOps.ValueError("key {0} is already registered with code {1}", (object) PythonOps.Repr(context, (object) pythonTuple), (object) PythonOps.Repr(context, PythonCopyReg.GetExtensionRegistry(context)[(object) pythonTuple]));
      throw PythonOps.ValueError("code {0} is already in use for key {1}", (object) PythonOps.Repr(context, (object) code), (object) PythonOps.Repr(context, PythonCopyReg.GetInvertedRegistry(context)[(object) code]));
    }
  }

  [Documentation("Unregister an extension code. (only for testing)")]
  public static void remove_extension(
    CodeContext context,
    object moduleName,
    object objectName,
    object value)
  {
    PythonTuple pythonTuple = PythonTuple.MakeTuple(moduleName, objectName);
    int code = PythonCopyReg.GetCode(context, value);
    object x1;
    object x2;
    if (PythonCopyReg.GetExtensionRegistry(context).TryGetValue((object) pythonTuple, out x1) && PythonCopyReg.GetInvertedRegistry(context).TryGetValue((object) code, out x2) && PythonOps.EqualRetBool(context, x1, (object) code) && PythonOps.EqualRetBool(context, x2, (object) pythonTuple))
    {
      PythonCopyReg.GetExtensionRegistry(context).__delitem__((object) pythonTuple);
      PythonCopyReg.GetInvertedRegistry(context).__delitem__((object) code);
    }
    else
      throw PythonOps.ValueError("key {0} is not registered with code {1}", (object) PythonOps.Repr(context, (object) pythonTuple), (object) PythonOps.Repr(context, (object) code));
  }

  [Documentation("__newobj__(cls, *args) -> cls.__new__(cls, *args)\n\nHelper function for unpickling. Creates a new object of a given class.\nSee PEP 307 section \"The __newobj__ unpickling function\" for details.")]
  public static object __newobj__(CodeContext context, object cls, params object[] args)
  {
    object[] objArray = new object[1 + args.Length];
    objArray[0] = cls;
    for (int index = 0; index < args.Length; ++index)
      objArray[index + 1] = args[index];
    return PythonOps.Invoke(context, cls, "__new__", objArray);
  }

  [Documentation("_reconstructor(basetype, objtype, basestate) -> object\n\nHelper function for unpickling. Creates and initializes a new object of a given\nclass. See PEP 307 section \"Case 2: pickling new-style class instances using\nprotocols 0 or 1\" for details.")]
  public static object _reconstructor(
    CodeContext context,
    object objType,
    object baseType,
    object baseState)
  {
    object obj;
    if (baseState == null)
    {
      obj = PythonOps.Invoke(context, baseType, "__new__", objType);
      PythonOps.Invoke(context, baseType, "__init__", obj);
    }
    else
    {
      obj = PythonOps.Invoke(context, baseType, "__new__", objType, baseState);
      PythonOps.Invoke(context, baseType, "__init__", obj, baseState);
    }
    return obj;
  }

  private static int GetCode(CodeContext context, object value)
  {
    try
    {
      int int32 = context.LanguageContext.ConvertToInt32(value);
      if (int32 > 0)
        return int32;
    }
    catch (OverflowException ex)
    {
    }
    throw PythonOps.ValueError("code out of range");
  }

  private static void EnsureModuleInitialized(CodeContext context)
  {
    if (context.LanguageContext.HasModuleState(PythonCopyReg._dispatchTableKey))
      return;
    Importer.ImportBuiltin(context, "copy_reg");
  }

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    context.NewObject = (BuiltinFunction) dict[(object) "__newobj__"];
    context.PythonReconstructor = (BuiltinFunction) dict[(object) "_reconstructor"];
    context.SetModuleState(PythonCopyReg._dispatchTableKey, dict[(object) "dispatch_table"] = (object) new PythonDictionary()
    {
      [(object) TypeCache.Complex] = dict[(object) "pickle_complex"]
    });
    context.SetModuleState(PythonCopyReg._extensionRegistryKey, dict[(object) "_extension_registry"] = (object) new PythonDictionary());
    context.SetModuleState(PythonCopyReg._invertedRegistryKey, dict[(object) "_inverted_registry"] = (object) new PythonDictionary());
    context.SetModuleState(PythonCopyReg._extensionCacheKey, dict[(object) "_extension_cache"] = (object) new PythonDictionary());
  }
}
