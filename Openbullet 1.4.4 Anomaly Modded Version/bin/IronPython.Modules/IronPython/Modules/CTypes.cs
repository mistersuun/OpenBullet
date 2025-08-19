// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.CTypes
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Binding;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Generation;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;

#nullable disable
namespace IronPython.Modules;

public static class CTypes
{
  private static WeakDictionary<PythonType, Dictionary<int, CTypes.ArrayType>> _arrayTypes = new WeakDictionary<PythonType, Dictionary<int, CTypes.ArrayType>>();
  public static readonly PythonType _SimpleCData = CTypes.SimpleType.MakeSystemType(typeof (CTypes.SimpleCData));
  public static readonly PythonType CFuncPtr = CTypes.CFuncPtrType.MakeSystemType(typeof (CTypes._CFuncPtr));
  public static readonly PythonType Structure = CTypes.StructType.MakeSystemType(typeof (CTypes._Structure));
  public static readonly PythonType Union = CTypes.UnionType.MakeSystemType(typeof (CTypes._Union));
  public static readonly PythonType _Pointer = CTypes.PointerType.MakeSystemType(typeof (CTypes.Pointer));
  public static readonly PythonType Array = CTypes.ArrayType.MakeSystemType(typeof (CTypes._Array));
  private static readonly object _lock = new object();
  private static readonly object _pointerTypeCacheKey = new object();
  private static readonly object _conversion_mode = new object();
  private static Dictionary<object, CTypes.RefCountInfo> _refCountTable;
  private static ModuleBuilder _dynamicModule;
  private static Dictionary<int, Type> _nativeTypes = new Dictionary<int, Type>();
  private static CTypes.StringAtDelegate _stringAt = new CTypes.StringAtDelegate(CTypes.StringAt);
  private static CTypes.StringAtDelegate _wstringAt = new CTypes.StringAtDelegate(CTypes.WStringAt);
  private static CTypes.CastDelegate _cast = new CTypes.CastDelegate(CTypes.Cast);
  public const string __version__ = "1.1.0";
  public const int FUNCFLAG_STDCALL = 0;
  public const int FUNCFLAG_CDECL = 1;
  public const int FUNCFLAG_HRESULT = 2;
  public const int FUNCFLAG_PYTHONAPI = 4;
  public const int FUNCFLAG_USE_ERRNO = 8;
  public const int FUNCFLAG_USE_LASTERROR = 16 /*0x10*/;
  public const int RTLD_GLOBAL = 0;
  public const int RTLD_LOCAL = 0;

  private static CTypes.ArrayType MakeArrayType(PythonType type, int count)
  {
    if (count < 0)
      throw PythonOps.ValueError("cannot multiply ctype by negative number");
    lock (CTypes._arrayTypes)
    {
      Dictionary<int, CTypes.ArrayType> dictionary1;
      if (!CTypes._arrayTypes.TryGetValue(type, out dictionary1))
        CTypes._arrayTypes[type] = dictionary1 = new Dictionary<int, CTypes.ArrayType>();
      CTypes.ArrayType arrayType1;
      if (!dictionary1.TryGetValue(count, out arrayType1))
      {
        Dictionary<int, CTypes.ArrayType> dictionary2 = dictionary1;
        int key = count;
        CodeContext sharedContext = type.Context.SharedContext;
        string name = $"{type.Name}_Array_{(object) count}";
        PythonTuple bases = PythonTuple.MakeTuple((object) CTypes.Array);
        PythonDictionary dict = PythonOps.MakeDictFromItems((object) type, (object) "_type_", (object) count, (object) "_length_");
        CTypes.ArrayType arrayType2;
        CTypes.ArrayType arrayType3 = arrayType2 = new CTypes.ArrayType(sharedContext, name, bases, dict);
        dictionary2[key] = arrayType2;
        arrayType1 = arrayType3;
      }
      return arrayType1;
    }
  }

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    context.EnsureModuleException((object) "ArgumentError", dict, "ArgumentError", "_ctypes");
    context.SystemState.__dict__[(object) "getrefcount"] = (object) null;
    PythonDictionary pythonDictionary = new PythonDictionary();
    dict[(object) "_pointer_type_cache"] = (object) pythonDictionary;
    context.SetModuleState(CTypes._pointerTypeCacheKey, (object) pythonDictionary);
    if (Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32S || Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.WinCE)
    {
      context.EnsureModuleException((object) "COMError", PythonExceptions.Exception, typeof (CTypes._COMError), dict, "COMError", "_ctypes", "Raised when a COM method call failed.", (Func<string, Exception>) (msg => (Exception) new COMException(msg)));
      context.SetModuleState(CTypes._conversion_mode, (object) PythonTuple.MakeTuple((object) "mbcs", (object) "ignore"));
    }
    else
      context.SetModuleState(CTypes._conversion_mode, (object) PythonTuple.MakeTuple((object) "ascii", (object) "strict"));
  }

  public static object _cast_addr
  {
    get => Marshal.GetFunctionPointerForDelegate((Delegate) CTypes._cast).ToPython();
  }

  private static IntPtr Cast(IntPtr data, IntPtr obj, IntPtr type)
  {
    GCHandle gcHandle1 = GCHandle.FromIntPtr(obj);
    GCHandle gcHandle2 = GCHandle.FromIntPtr(type);
    try
    {
      CTypes.CData target1 = gcHandle1.Target as CTypes.CData;
      PythonType target2 = (PythonType) gcHandle2.Target;
      CTypes.CData instance = (CTypes.CData) target2.CreateInstance(target2.Context.SharedContext);
      if (CTypes.IsPointer(target2))
      {
        instance._memHolder = new MemoryHolder(IntPtr.Size);
        if (CTypes.IsPointer(DynamicHelpers.GetPythonType((object) target1)))
          instance._memHolder.WriteIntPtr(0, target1._memHolder.ReadIntPtr(0));
        else
          instance._memHolder.WriteIntPtr(0, data);
        if (target1 != null)
        {
          instance._memHolder.Objects = target1._memHolder.Objects;
          instance._memHolder.AddObject((object) IdDispenser.GetId((object) target1), (object) target1);
        }
      }
      else
        instance._memHolder = target1 == null ? new MemoryHolder(data, ((CTypes.INativeType) target2).Size) : new MemoryHolder(data, ((CTypes.INativeType) target2).Size, target1._memHolder);
      return GCHandle.ToIntPtr(GCHandle.Alloc((object) instance));
    }
    finally
    {
      gcHandle2.Free();
      gcHandle1.Free();
    }
  }

  private static bool IsPointer(PythonType pt)
  {
    switch (pt)
    {
      case CTypes.PointerType _:
        return true;
      case CTypes.SimpleType simpleType:
        return simpleType._type == CTypes.SimpleTypeKind.Pointer || simpleType._type == CTypes.SimpleTypeKind.CharPointer || simpleType._type == CTypes.SimpleTypeKind.WCharPointer;
      default:
        return false;
    }
  }

  public static object _memmove_addr => NativeFunctions.GetMemMoveAddress().ToPython();

  public static object _memset_addr => NativeFunctions.GetMemSetAddress().ToPython();

  public static object _string_at_addr
  {
    get => Marshal.GetFunctionPointerForDelegate((Delegate) CTypes._stringAt).ToPython();
  }

  public static object _wstring_at_addr
  {
    get => Marshal.GetFunctionPointerForDelegate((Delegate) CTypes._wstringAt).ToPython();
  }

  public static int CopyComPointer(object src, object dest)
  {
    throw new NotImplementedException(nameof (CopyComPointer));
  }

  public static string FormatError() => CTypes.FormatError(CTypes.get_last_error());

  public static string FormatError(int errorCode) => new Win32Exception(errorCode).Message;

  public static void FreeLibrary(int handle) => CTypes.FreeLibrary(new IntPtr(handle));

  public static void FreeLibrary(BigInteger handle)
  {
    CTypes.FreeLibrary(new IntPtr((long) handle));
  }

  public static void FreeLibrary(IntPtr handle) => NativeFunctions.FreeLibrary(handle);

  public static object LoadLibrary(string library, int mode = 0)
  {
    IntPtr handle = NativeFunctions.LoadDLL(library, mode);
    return !(handle == IntPtr.Zero) ? handle.ToPython() : throw PythonOps.OSError("cannot load library " + library);
  }

  public static object dlopen(string library, int mode = 0) => CTypes.LoadLibrary(library, mode);

  public static PythonType POINTER(CodeContext context, PythonType type)
  {
    PythonDictionary moduleState = (PythonDictionary) context.LanguageContext.GetModuleState(CTypes._pointerTypeCacheKey);
    lock (moduleState)
    {
      object obj;
      if (!moduleState.TryGetValue((object) type, out obj))
      {
        string str = type != null ? "LP_" + type.Name : "c_void_p";
        PythonDictionary pythonDictionary = moduleState;
        PythonType key = type;
        CodeContext context1 = context;
        string name = str;
        PythonDictionary dict = PythonOps.MakeDictFromItems((object) type, (object) "_type_");
        CTypes.PointerType pointerType;
        obj = (object) (pointerType = CTypes.MakePointer(context1, name, dict));
        pythonDictionary[(object) key] = (object) pointerType;
      }
      return obj as PythonType;
    }
  }

  private static CTypes.PointerType MakePointer(
    CodeContext context,
    string name,
    PythonDictionary dict)
  {
    return new CTypes.PointerType(context, name, PythonTuple.MakeTuple((object) CTypes._Pointer), dict);
  }

  public static PythonType POINTER(CodeContext context, [NotNull] string name)
  {
    PythonType o = (PythonType) CTypes.MakePointer(context, name, new PythonDictionary());
    PythonDictionary moduleState = (PythonDictionary) context.LanguageContext.GetModuleState(CTypes._pointerTypeCacheKey);
    lock (moduleState)
      moduleState[Builtin.id((object) o)] = (object) o;
    return o;
  }

  public static object PyObj_FromPtr(IntPtr address)
  {
    GCHandle gcHandle = GCHandle.FromIntPtr(address);
    object target = gcHandle.Target;
    gcHandle.Free();
    return target;
  }

  public static IntPtr PyObj_ToPtr(object obj) => GCHandle.ToIntPtr(GCHandle.Alloc(obj));

  public static void Py_DECREF(object key)
  {
    CTypes.EnsureRefCountTable();
    lock (CTypes._refCountTable)
    {
      CTypes.RefCountInfo refCountInfo;
      if (!CTypes._refCountTable.TryGetValue(key, out refCountInfo))
        throw new InvalidOperationException();
      --refCountInfo.RefCount;
      if (refCountInfo.RefCount != 0)
        return;
      refCountInfo.Handle.Free();
      CTypes._refCountTable.Remove(key);
    }
  }

  public static void Py_INCREF(object key)
  {
    CTypes.EnsureRefCountTable();
    lock (CTypes._refCountTable)
    {
      CTypes.RefCountInfo refCountInfo;
      if (!CTypes._refCountTable.TryGetValue(key, out refCountInfo))
      {
        CTypes._refCountTable[key] = refCountInfo = new CTypes.RefCountInfo();
        refCountInfo.Handle = GCHandle.Alloc(key, GCHandleType.Pinned);
      }
      ++refCountInfo.RefCount;
    }
  }

  public static PythonTuple _buffer_info(CTypes.CData data) => data.GetBufferInfo();

  public static void _check_HRESULT(int hresult)
  {
    if (hresult < 0)
      throw PythonOps.WindowsError("ctypes function returned failed HRESULT: {0}", PythonOps.Hex((object) (BigInteger) (uint) hresult));
  }

  public static void _unpickle()
  {
  }

  public static object addressof(CTypes.CData data) => data._memHolder.UnsafeAddress.ToPython();

  public static int alignment(PythonType type)
  {
    return type is CTypes.INativeType nativeType ? nativeType.Alignment : throw PythonOps.TypeError("this type has no size");
  }

  public static int alignment(object o) => CTypes.alignment(DynamicHelpers.GetPythonType(o));

  public static object byref(CTypes.CData instance, int offset = 0)
  {
    if (offset != 0)
      throw new NotImplementedException("byref w/ arg");
    return (object) new CTypes.NativeArgument(instance, "P");
  }

  public static object call_cdeclfunction(CodeContext context, int address, PythonTuple args)
  {
    return CTypes.call_cdeclfunction(context, new IntPtr(address), args);
  }

  public static object call_cdeclfunction(
    CodeContext context,
    BigInteger address,
    PythonTuple args)
  {
    return CTypes.call_cdeclfunction(context, new IntPtr((long) address), args);
  }

  public static object call_cdeclfunction(CodeContext context, IntPtr address, PythonTuple args)
  {
    return PythonOps.CallWithArgsTuple((object) (CTypes._CFuncPtr) CTypes.GetFunctionType(context, 1).CreateInstance(context, (object) address), new object[0], (object) args);
  }

  public static void call_commethod()
  {
  }

  public static object call_function(CodeContext context, int address, PythonTuple args)
  {
    return CTypes.call_function(context, new IntPtr(address), args);
  }

  public static object call_function(CodeContext context, BigInteger address, PythonTuple args)
  {
    return CTypes.call_function(context, new IntPtr((long) address), args);
  }

  public static object call_function(CodeContext context, IntPtr address, PythonTuple args)
  {
    return PythonOps.CallWithArgsTuple((object) (CTypes._CFuncPtr) CTypes.GetFunctionType(context, 0).CreateInstance(context, (object) address), new object[0], (object) args);
  }

  private static CTypes.CFuncPtrType GetFunctionType(CodeContext context, int flags)
  {
    CTypes.SimpleType simpleType = new CTypes.SimpleType(context, "int", PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonTypeFromType(typeof (CTypes.SimpleCData))), PythonOps.MakeHomogeneousDictFromItems(new object[2]
    {
      (object) "i",
      (object) "_type_"
    }));
    return new CTypes.CFuncPtrType(context, "func", PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonTypeFromType(typeof (CTypes._CFuncPtr))), PythonOps.MakeHomogeneousDictFromItems(new object[4]
    {
      (object) 0,
      (object) "_flags_",
      (object) simpleType,
      (object) "_restype_"
    }));
  }

  public static int get_errno() => 0;

  public static int get_last_error()
  {
    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
      return NativeFunctions.GetLastError();
    throw PythonOps.NameError(nameof (get_last_error));
  }

  public static CTypes.Pointer pointer(CodeContext context, CTypes.CData data)
  {
    return (CTypes.Pointer) CTypes.POINTER(context, DynamicHelpers.GetPythonType((object) data)).CreateInstance(context, (object) data);
  }

  public static void resize(CTypes.CData obj, int newSize)
  {
    if (newSize < obj.NativeType.Size)
      throw PythonOps.ValueError("minimum size is {0}", (object) newSize);
    MemoryHolder destAddress = new MemoryHolder(newSize);
    obj._memHolder.CopyTo(destAddress, 0, Math.Min(obj._memHolder.Size, newSize));
    obj._memHolder = destAddress;
  }

  public static PythonTuple set_conversion_mode(
    CodeContext context,
    string encoding,
    string errors)
  {
    PythonContext languageContext = context.LanguageContext;
    PythonTuple moduleState = (PythonTuple) languageContext.GetModuleState(CTypes._conversion_mode);
    languageContext.SetModuleState(CTypes._conversion_mode, (object) PythonTuple.MakeTuple((object) encoding, (object) errors));
    return moduleState;
  }

  public static void set_errno()
  {
  }

  public static int set_last_error(int errorCode)
  {
    if (Environment.OSVersion.Platform != PlatformID.Win32NT)
      throw PythonOps.NameError(nameof (set_last_error));
    int lastError = NativeFunctions.GetLastError();
    NativeFunctions.SetLastError(errorCode);
    return lastError;
  }

  public static int @sizeof(PythonType type)
  {
    return type is CTypes.INativeType nativeType ? nativeType.Size : throw PythonOps.TypeError("this type has no size");
  }

  public static int @sizeof(object instance)
  {
    return instance is CTypes.CData cdata && cdata._memHolder != null ? cdata._memHolder.Size : CTypes.@sizeof(DynamicHelpers.GetPythonType(instance));
  }

  private static ModuleBuilder DynamicModule
  {
    get
    {
      if ((System.Reflection.Module) CTypes._dynamicModule == (System.Reflection.Module) null)
      {
        lock (CTypes._lock)
        {
          if ((System.Reflection.Module) CTypes._dynamicModule == (System.Reflection.Module) null)
          {
            CustomAttributeBuilder[] assemblyAttributes = new CustomAttributeBuilder[2]
            {
              new CustomAttributeBuilder(typeof (UnverifiableCodeAttribute).GetConstructor(ReflectionUtils.EmptyTypes), new object[0]),
              new CustomAttributeBuilder(typeof (PermissionSetAttribute).GetConstructor(new Type[1]
              {
                typeof (SecurityAction)
              }), new object[1]
              {
                (object) SecurityAction.Demand
              }, new PropertyInfo[1]
              {
                typeof (PermissionSetAttribute).GetProperty("Unrestricted")
              }, new object[1]{ (object) true })
            };
            string str = typeof (CTypes).Namespace + ".DynamicAssembly";
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(str), AssemblyBuilderAccess.Run, (IEnumerable<CustomAttributeBuilder>) assemblyAttributes);
            assemblyBuilder.DefineVersionInfoResource();
            CTypes._dynamicModule = assemblyBuilder.DefineDynamicModule(str);
          }
        }
      }
      return CTypes._dynamicModule;
    }
  }

  private static Type GetMarshalTypeFromSize(int size)
  {
    lock (CTypes._nativeTypes)
    {
      Type type;
      if (!CTypes._nativeTypes.TryGetValue(size, out type))
      {
        int num = size;
        TypeBuilder typeBuilder = CTypes.DynamicModule.DefineType("interop_type_size_" + (object) size, TypeAttributes.Public | TypeAttributes.SequentialLayout | TypeAttributes.Sealed | TypeAttributes.Serializable, typeof (ValueType), size);
        for (; num > 8; num -= 8)
          typeBuilder.DefineField("field" + (object) num, typeof (long), FieldAttributes.Private);
        for (; num > 4; num -= 4)
          typeBuilder.DefineField("field" + (object) num, typeof (int), FieldAttributes.Private);
        for (; num > 0; --num)
          typeBuilder.DefineField("field" + (object) num, typeof (byte), FieldAttributes.Private);
        CTypes._nativeTypes[size] = type = typeBuilder.CreateType();
      }
      return type;
    }
  }

  private static void GetFieldInfo(
    CTypes.INativeType type,
    object o,
    out string fieldName,
    out CTypes.INativeType cdata,
    out int? bitCount)
  {
    PythonTuple pt = o as PythonTuple;
    fieldName = pt.Count == 2 || pt.Count == 3 ? pt[0] as string : throw PythonOps.AttributeError("'_fields_' must be a sequence of pairs");
    if (fieldName == null)
      throw PythonOps.TypeError("first item in _fields_ tuple must be a string, got", (object) PythonTypeOps.GetName(pt[0]));
    cdata = pt[1] as CTypes.INativeType;
    if (cdata == null)
      throw PythonOps.TypeError("second item in _fields_ tuple must be a C type, got {0}", (object) PythonTypeOps.GetName(pt[0]));
    if (cdata == type)
      throw CTypes.StructureCannotContainSelf();
    if (cdata is CTypes.StructType structType)
      structType.EnsureFinal();
    if (pt.Count != 3)
      bitCount = new int?();
    else
      bitCount = new int?(CTypes.CheckBits(cdata, pt));
  }

  private static int CheckBits(CTypes.INativeType cdata, PythonTuple pt)
  {
    int int32 = Converter.ConvertToInt32(pt[2]);
    if (!(cdata is CTypes.SimpleType simpleType))
      throw PythonOps.TypeError("bit fields not allowed for type {0}", (object) ((PythonType) cdata).Name);
    switch (simpleType._type)
    {
      case CTypes.SimpleTypeKind.Char:
      case CTypes.SimpleTypeKind.Single:
      case CTypes.SimpleTypeKind.Double:
      case CTypes.SimpleTypeKind.Object:
      case CTypes.SimpleTypeKind.Pointer:
      case CTypes.SimpleTypeKind.CharPointer:
      case CTypes.SimpleTypeKind.WCharPointer:
      case CTypes.SimpleTypeKind.WChar:
        throw PythonOps.TypeError("bit fields not allowed for type {0}", (object) ((PythonType) cdata).Name);
      default:
        if (int32 <= 0 || int32 > cdata.Size * 8)
          throw PythonOps.ValueError("number of bits invalid for bit field");
        return int32;
    }
  }

  private static IList<object> GetFieldsList(object fields)
  {
    return fields is IList<object> objectList ? objectList : throw PythonOps.TypeError("class must be a sequence of pairs");
  }

  private static Exception StructureCannotContainSelf()
  {
    return PythonOps.AttributeError("Structure or union cannot contain itself");
  }

  private static IntPtr StringAt(IntPtr src, int len)
  {
    return GCHandle.ToIntPtr(GCHandle.Alloc(len != -1 ? (object) MemoryHolder.ReadAnsiString(src, 0, len) : (object) MemoryHolder.ReadAnsiString(src, 0)));
  }

  private static IntPtr WStringAt(IntPtr src, int len)
  {
    return GCHandle.ToIntPtr(GCHandle.Alloc(len != -1 ? (object) Marshal.PtrToStringUni(src, len) : (object) Marshal.PtrToStringUni(src)));
  }

  private static IntPtr GetHandleFromObject(object dll, string errorMsg)
  {
    BigInteger result;
    if (!Converter.TryConvertToBigInteger(PythonOps.GetBoundAttr(DefaultContext.Default, dll, "_handle"), out result))
      throw PythonOps.TypeError(errorMsg);
    return new IntPtr((long) result);
  }

  private static void ValidateArraySizes(ArrayModule.array array, int offset, int size)
  {
    CTypes.ValidateArraySizes(array.__len__() * array.itemsize, offset, size);
  }

  private static void ValidateArraySizes(Bytes bytes, int offset, int size)
  {
    CTypes.ValidateArraySizes(bytes.Count, offset, size);
  }

  private static void ValidateArraySizes(string data, int offset, int size)
  {
    CTypes.ValidateArraySizes(data.Length, offset, size);
  }

  private static void ValidateArraySizes(int arraySize, int offset, int size)
  {
    if (offset < 0)
      throw PythonOps.ValueError("offset cannot be negative");
    if (arraySize < size + offset)
      throw PythonOps.ValueError($"Buffer size too small ({arraySize} instead of at least {size} bytes)");
  }

  private static void ValidateArraySizes(BigInteger arraySize, int offset, int size)
  {
    if (offset < 0)
      throw PythonOps.ValueError("offset cannot be negative");
    if (arraySize < (long) (size + offset))
      throw PythonOps.ValueError($"Buffer size too small ({arraySize} instead of at least {size} bytes)");
  }

  public static object GetCharArrayValue(CTypes._Array arr)
  {
    return arr.NativeType.GetValue(arr._memHolder, (object) arr, 0, false);
  }

  public static void SetCharArrayValue(CTypes._Array arr, object value)
  {
    if (value is PythonBuffer pythonBuffer && pythonBuffer._object is string)
      value = (object) pythonBuffer.ToString();
    arr.NativeType.SetValue(arr._memHolder, 0, value);
  }

  public static void DeleteCharArrayValue(CTypes._Array arr, object value)
  {
    throw PythonOps.TypeError("cannot delete char array value");
  }

  public static object GetWCharArrayValue(CTypes._Array arr)
  {
    return arr.NativeType.GetValue(arr._memHolder, (object) arr, 0, false);
  }

  public static void SetWCharArrayValue(CTypes._Array arr, object value)
  {
    arr.NativeType.SetValue(arr._memHolder, 0, value);
  }

  public static object DeleteWCharArrayValue(CTypes._Array arr)
  {
    throw PythonOps.TypeError("cannot delete wchar array value");
  }

  public static object GetWCharArrayRaw(CTypes._Array arr)
  {
    return (object) ((CTypes.ArrayType) arr.NativeType).GetRawValue(arr._memHolder, 0);
  }

  public static void SetWCharArrayRaw(CTypes._Array arr, object value)
  {
    if (value is PythonBuffer pythonBuffer && (pythonBuffer._object is string || pythonBuffer._object is Bytes))
      value = (object) pythonBuffer.ToString();
    MemoryView memoryView = value as MemoryView;
    if ((object) memoryView != null)
    {
      string str = memoryView.tobytes().ToString();
      if (str.Length > arr.__len__())
        throw PythonOps.ValueError("string too long");
      value = (object) str;
    }
    arr.NativeType.SetValue(arr._memHolder, 0, value);
  }

  public static object DeleteWCharArrayRaw(CTypes._Array arr)
  {
    throw PythonOps.TypeError("cannot delete wchar array raw");
  }

  private static void EmitCDataCreation(
    CTypes.INativeType type,
    ILGenerator method,
    List<object> constantPool,
    int constantPoolArgument)
  {
    LocalBuilder local = method.DeclareLocal(type.GetNativeType());
    method.Emit(OpCodes.Stloc, local);
    method.Emit(OpCodes.Ldloca, local);
    constantPool.Add((object) type);
    method.Emit(OpCodes.Ldarg, constantPoolArgument);
    method.Emit(OpCodes.Ldc_I4, constantPool.Count - 1);
    method.Emit(OpCodes.Ldelem_Ref);
    method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("CreateCData"));
  }

  private static void EnsureRefCountTable()
  {
    if (CTypes._refCountTable != null)
      return;
    Interlocked.CompareExchange<Dictionary<object, CTypes.RefCountInfo>>(ref CTypes._refCountTable, new Dictionary<object, CTypes.RefCountInfo>(), (Dictionary<object, CTypes.RefCountInfo>) null);
  }

  [PythonType("Array")]
  public abstract class _Array : CTypes.CData
  {
    public void __init__(params object[] args)
    {
      CTypes.INativeType nativeType = this.NativeType;
      this._memHolder = new MemoryHolder(nativeType.Size);
      if (args.Length > ((CTypes.ArrayType) nativeType).Length)
        throw PythonOps.IndexError("too many arguments");
      nativeType.SetValue(this._memHolder, 0, (object) args);
    }

    public object this[int index]
    {
      get
      {
        index = PythonOps.FixIndex(index, ((CTypes.ArrayType) this.NativeType).Length);
        CTypes.INativeType elementType = this.ElementType;
        return elementType.GetValue(this._memHolder, (object) this, checked (index * elementType.Size), false);
      }
      set
      {
        index = PythonOps.FixIndex(index, ((CTypes.ArrayType) this.NativeType).Length);
        CTypes.INativeType elementType = this.ElementType;
        object obj = elementType.SetValue(this._memHolder, checked (index * elementType.Size), value);
        if (obj == null)
          return;
        this._memHolder.AddObject((object) index.ToString(), obj);
      }
    }

    public object this[[NotNull] IronPython.Runtime.Slice slice]
    {
      get
      {
        int length = ((CTypes.ArrayType) this.NativeType).Length;
        CTypes.SimpleType elementType = ((CTypes.ArrayType) this.NativeType).ElementType as CTypes.SimpleType;
        int ostart;
        int ostop;
        int ostep;
        slice.indices(length, out ostart, out ostop, out ostep);
        if (ostep > 0 && ostart >= ostop || ostep < 0 && ostart <= ostop)
          return elementType != null && (elementType._type == CTypes.SimpleTypeKind.WChar || elementType._type == CTypes.SimpleTypeKind.Char) ? (object) string.Empty : (object) new IronPython.Runtime.List();
        int capacity = ostep > 0 ? (int) (((long) ostop - (long) ostart + (long) ostep - 1L) / (long) ostep) : (int) (((long) ostop - (long) ostart + (long) ostep + 1L) / (long) ostep);
        if (elementType != null && (elementType._type == CTypes.SimpleTypeKind.WChar || elementType._type == CTypes.SimpleTypeKind.Char))
        {
          int size = ((CTypes.INativeType) elementType).Size;
          StringBuilder stringBuilder = new StringBuilder(capacity);
          int num1 = 0;
          int num2 = ostart;
          while (num1 < capacity)
          {
            char ch = elementType.ReadChar(this._memHolder, checked (num2 * size));
            stringBuilder.Append(ch);
            ++num1;
            num2 += ostep;
          }
          return (object) stringBuilder.ToString();
        }
        object[] items = new object[capacity];
        int num3 = 0;
        int num4 = 0;
        int index = ostart;
        while (num4 < capacity)
        {
          items[num3++] = this[index];
          ++num4;
          index += ostep;
        }
        return (object) new IronPython.Runtime.List((ICollection) items);
      }
      set
      {
        int length = ((CTypes.ArrayType) this.NativeType).Length;
        int ostart;
        int ostop;
        int ostep;
        slice.indices(length, out ostart, out ostop, out ostep);
        int num1 = ostep > 0 ? (int) (((long) ostop - (long) ostart + (long) ostep - 1L) / (long) ostep) : (int) (((long) ostop - (long) ostart + (long) ostep + 1L) / (long) ostep);
        IEnumerator enumerator = PythonOps.GetEnumerator(value);
        int num2 = 0;
        int index = ostart;
        while (num2 < num1)
        {
          this[index] = enumerator.MoveNext() ? enumerator.Current : throw PythonOps.ValueError("sequence not long enough");
          ++num2;
          index += ostep;
        }
        if (enumerator.MoveNext())
          throw PythonOps.ValueError("not all values consumed while slicing");
      }
    }

    public int __len__() => ((CTypes.ArrayType) this.NativeType).Length;

    private CTypes.INativeType ElementType => ((CTypes.ArrayType) this.NativeType).ElementType;

    internal override PythonTuple GetBufferInfo()
    {
      CTypes.INativeType elementType = this.ElementType;
      int num = 1;
      List<object> o = new List<object>();
      o.Add((object) (BigInteger) this.__len__());
      for (; elementType is CTypes.ArrayType; elementType = ((CTypes.ArrayType) elementType).ElementType)
      {
        ++num;
        o.Add((object) (BigInteger) ((CTypes.ArrayType) elementType).Length);
      }
      return PythonTuple.MakeTuple((object) this.NativeType.TypeFormat, (object) num, (object) PythonTuple.Make((object) o));
    }

    public override int ItemCount
    {
      [PythonHidden(new PlatformID[] {})] get => this.__len__();
    }

    public override BigInteger ItemSize
    {
      [PythonHidden(new PlatformID[] {})] get
      {
        CTypes.INativeType nativeType = this.NativeType;
        while (nativeType is CTypes.ArrayType)
          nativeType = ((CTypes.ArrayType) nativeType).ElementType;
        return (BigInteger) nativeType.Size;
      }
    }

    [PythonHidden(new PlatformID[] {})]
    public override IList<BigInteger> GetShape(int start, int? end)
    {
      List<BigInteger> shape = new List<BigInteger>();
      for (CTypes.ArrayType arrayType = this.NativeType as CTypes.ArrayType; arrayType != null; arrayType = arrayType.ElementType as CTypes.ArrayType)
        shape.Add((BigInteger) arrayType.Length);
      return (IList<BigInteger>) shape;
    }
  }

  [PythonType]
  [PythonHidden(new PlatformID[] {})]
  public class ArrayType : PythonType, CTypes.INativeType
  {
    private int _length;
    private CTypes.INativeType _type;

    public ArrayType(CodeContext context, string name, PythonTuple bases, PythonDictionary dict)
      : base(context, name, bases, dict)
    {
      object obj1;
      int num1;
      if (!dict.TryGetValue((object) "_length_", out obj1) || !(obj1 is int num2) || (num1 = num2) < 0)
        throw PythonOps.AttributeError("arrays must have _length_ attribute and it must be a positive integer");
      object obj2;
      if (!dict.TryGetValue((object) "_type_", out obj2))
        throw PythonOps.AttributeError("class must define a '_type_' attribute");
      this._length = num1;
      this._type = (CTypes.INativeType) obj2;
      if (!(this._type is CTypes.SimpleType type))
        return;
      if (type._type == CTypes.SimpleTypeKind.Char)
      {
        this.SetCustomMember(context, "value", (object) new ReflectedExtensionProperty(new ExtensionPropertyInfo((Type) (PythonType) this, typeof (CTypes).GetMethod("GetCharArrayValue")), NameType.PythonProperty));
        this.SetCustomMember(context, "raw", (object) new ReflectedExtensionProperty(new ExtensionPropertyInfo((Type) (PythonType) this, typeof (CTypes).GetMethod("GetWCharArrayRaw")), NameType.PythonProperty));
      }
      else
      {
        if (type._type != CTypes.SimpleTypeKind.WChar)
          return;
        this.SetCustomMember(context, "value", (object) new ReflectedExtensionProperty(new ExtensionPropertyInfo((Type) (PythonType) this, typeof (CTypes).GetMethod("GetWCharArrayValue")), NameType.PythonProperty));
        this.SetCustomMember(context, "raw", (object) new ReflectedExtensionProperty(new ExtensionPropertyInfo((Type) (PythonType) this, typeof (CTypes).GetMethod("GetWCharArrayRaw")), NameType.PythonProperty));
      }
    }

    private ArrayType(Type underlyingSystemType)
      : base(underlyingSystemType)
    {
    }

    public CTypes._Array from_address(CodeContext context, int ptr)
    {
      CTypes._Array instance = (CTypes._Array) this.CreateInstance(context);
      instance.SetAddress(new IntPtr(ptr));
      return instance;
    }

    public CTypes._Array from_address(CodeContext context, BigInteger ptr)
    {
      CTypes._Array instance = (CTypes._Array) this.CreateInstance(context);
      instance.SetAddress(new IntPtr((long) ptr));
      return instance;
    }

    public CTypes._Array from_buffer(CodeContext context, ArrayModule.array array, int offset = 0)
    {
      CTypes.ValidateArraySizes(array, offset, ((CTypes.INativeType) this).Size);
      CTypes._Array instance = (CTypes._Array) this.CreateInstance(context);
      instance._memHolder = new MemoryHolder(array.GetArrayAddress().Add(offset), ((CTypes.INativeType) this).Size);
      instance._memHolder.AddObject((object) "ffffffff", (object) array);
      return instance;
    }

    public CTypes._Array from_buffer_copy(CodeContext context, ArrayModule.array array, int offset = 0)
    {
      CTypes.ValidateArraySizes(array, offset, ((CTypes.INativeType) this).Size);
      CTypes._Array instance = (CTypes._Array) this.CreateInstance(context);
      instance._memHolder = new MemoryHolder(((CTypes.INativeType) this).Size);
      instance._memHolder.CopyFrom(array.GetArrayAddress().Add(offset), new IntPtr(((CTypes.INativeType) this).Size));
      GC.KeepAlive((object) array);
      return instance;
    }

    public CTypes._Array from_buffer_copy(CodeContext context, Bytes array, int offset = 0)
    {
      CTypes.ValidateArraySizes(array, offset, ((CTypes.INativeType) this).Size);
      CTypes._Array instance = (CTypes._Array) this.CreateInstance(context);
      instance._memHolder = new MemoryHolder(((CTypes.INativeType) this).Size);
      for (int offset1 = 0; offset1 < ((CTypes.INativeType) this).Size; ++offset1)
        instance._memHolder.WriteByte(offset1, array._bytes[offset1]);
      return instance;
    }

    public CTypes._Array from_buffer_copy(CodeContext context, string data, int offset = 0)
    {
      CTypes.ValidateArraySizes(data, offset, ((CTypes.INativeType) this).Size);
      CTypes._Array instance = (CTypes._Array) this.CreateInstance(context);
      instance._memHolder = new MemoryHolder(((CTypes.INativeType) this).Size);
      for (int index = 0; index < ((CTypes.INativeType) this).Size; ++index)
        instance._memHolder.WriteByte(index, (byte) data[index]);
      return instance;
    }

    public object from_param(object obj) => (object) null;

    internal static PythonType MakeSystemType(Type underlyingSystemType)
    {
      return PythonType.SetPythonType(underlyingSystemType, (PythonType) new CTypes.ArrayType(underlyingSystemType));
    }

    public static CTypes.ArrayType operator *(CTypes.ArrayType type, int count)
    {
      return CTypes.MakeArrayType((PythonType) type, count);
    }

    public static CTypes.ArrayType operator *(int count, CTypes.ArrayType type)
    {
      return CTypes.MakeArrayType((PythonType) type, count);
    }

    int CTypes.INativeType.Size => this.GetSize();

    private int GetSize() => this._length * this._type.Size;

    int CTypes.INativeType.Alignment => this._type.Alignment;

    object CTypes.INativeType.GetValue(
      MemoryHolder owner,
      object readingFrom,
      int offset,
      bool raw)
    {
      if (this.IsStringType)
      {
        string str = ((CTypes.SimpleType) this._type)._type != CTypes.SimpleTypeKind.Char ? owner.ReadUnicodeString(offset, this._length) : owner.ReadAnsiString(offset, this._length);
        for (int index = 0; index < str.Length; ++index)
        {
          if (str[index] == char.MinValue)
            return (object) str.Substring(0, index);
        }
        return (object) str;
      }
      CTypes._Array instance = (CTypes._Array) this.CreateInstance(this.Context.SharedContext);
      instance._memHolder = new MemoryHolder(owner.UnsafeAddress.Add(offset), ((CTypes.INativeType) this).Size, owner);
      return (object) instance;
    }

    internal string GetRawValue(MemoryHolder owner, int offset)
    {
      return ((CTypes.SimpleType) this._type)._type != CTypes.SimpleTypeKind.Char ? owner.ReadUnicodeString(offset, this._length) : owner.ReadAnsiString(offset, this._length);
    }

    private bool IsStringType
    {
      get
      {
        if (!(this._type is CTypes.SimpleType type))
          return false;
        return type._type == CTypes.SimpleTypeKind.WChar || type._type == CTypes.SimpleTypeKind.Char;
      }
    }

    object CTypes.INativeType.SetValue(MemoryHolder address, int offset, object value)
    {
      if (value is string str)
      {
        if (!this.IsStringType)
          throw PythonOps.TypeError("expected {0} instance, got str", (object) this.Name);
        if (str.Length > this._length)
          throw PythonOps.ValueError("string too long ({0}, maximum length {1})", (object) str.Length, (object) this._length);
        this.WriteString(address, offset, str);
        return (object) null;
      }
      if (this.IsStringType)
      {
        StringBuilder stringBuilder = value is IList<object> objectList ? new StringBuilder(objectList.Count) : throw PythonOps.TypeError("expected string or Unicode object, {0} found", (object) DynamicHelpers.GetPythonType(value).Name);
        foreach (object obj in (IEnumerable<object>) objectList)
          stringBuilder.Append(Converter.ConvertToChar(obj));
        this.WriteString(address, offset, stringBuilder.ToString());
        return (object) null;
      }
      if (!(value is object[] objArray) && value is PythonTuple pythonTuple)
        objArray = pythonTuple._data;
      if (objArray != null)
      {
        if (objArray.Length > this._length)
          throw PythonOps.RuntimeError("invalid index");
        for (int index = 0; index < objArray.Length; ++index)
          this._type.SetValue(address, checked (offset + index * this._type.Size), objArray[index]);
        return (object) null;
      }
      if (value is CTypes._Array array && array.NativeType == this)
      {
        array._memHolder.CopyTo(address, offset, ((CTypes.INativeType) this).Size);
        return (object) array._memHolder.EnsureObjects();
      }
      throw PythonOps.TypeError("unexpected {0} instance, got {1}", (object) this.Name, (object) DynamicHelpers.GetPythonType(value).Name);
    }

    private void WriteString(MemoryHolder address, int offset, string str)
    {
      CTypes.SimpleType type = (CTypes.SimpleType) this._type;
      if (str.Length < this._length)
        str += "\0";
      if (type._type == CTypes.SimpleTypeKind.Char)
        address.WriteAnsiString(offset, str);
      else
        address.WriteUnicodeString(offset, str);
    }

    Type CTypes.INativeType.GetNativeType() => typeof (IntPtr);

    MarshalCleanup CTypes.INativeType.EmitMarshalling(
      ILGenerator method,
      LocalOrArg argIndex,
      List<object> constantPool,
      int constantPoolArgument)
    {
      Type type = argIndex.Type;
      Label label1 = method.DefineLabel();
      if (!type.IsValueType)
      {
        Label label2 = method.DefineLabel();
        argIndex.Emit(method);
        method.Emit(OpCodes.Ldnull);
        method.Emit(OpCodes.Bne_Un, label2);
        method.Emit(OpCodes.Ldc_I4_0);
        method.Emit(OpCodes.Conv_I);
        method.Emit(OpCodes.Br, label1);
        method.MarkLabel(label2);
      }
      argIndex.Emit(method);
      if (type.IsValueType)
        method.Emit(OpCodes.Box, type);
      constantPool.Add((object) this);
      method.Emit(OpCodes.Ldarg, constantPoolArgument);
      method.Emit(OpCodes.Ldc_I4, constantPool.Count - 1);
      method.Emit(OpCodes.Ldelem_Ref);
      method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("CheckCDataType"));
      method.Emit(OpCodes.Call, typeof (CTypes.CData).GetMethod("get_UnsafeAddress"));
      method.MarkLabel(label1);
      return (MarshalCleanup) null;
    }

    Type CTypes.INativeType.GetPythonType() => ((CTypes.INativeType) this).GetNativeType();

    void CTypes.INativeType.EmitReverseMarshalling(
      ILGenerator method,
      LocalOrArg value,
      List<object> constantPool,
      int constantPoolArgument)
    {
      value.Emit(method);
    }

    internal int Length => this._length;

    internal CTypes.INativeType ElementType => this._type;

    string CTypes.INativeType.TypeFormat
    {
      get
      {
        string str = "(" + (object) this.Length;
        CTypes.INativeType elementType;
        for (elementType = this.ElementType; elementType is CTypes.ArrayType; elementType = ((CTypes.ArrayType) elementType).ElementType)
          str = $"{str},{(object) ((CTypes.ArrayType) elementType).Length}";
        return str + ")" + elementType.TypeFormat;
      }
    }
  }

  [PythonType("_CData")]
  [PythonHidden(new PlatformID[] {})]
  public abstract class CData : IPythonBufferable, IBufferProtocol
  {
    internal MemoryHolder _memHolder;

    public int Size
    {
      [PythonHidden(new PlatformID[] {})] get => this.NativeType.Size;
    }

    public IntPtr UnsafeAddress
    {
      [PythonHidden(new PlatformID[] {})] get => this._memHolder.UnsafeAddress;
    }

    byte[] IPythonBufferable.GetBytes(int offset, int length)
    {
      int num = checked (offset + length);
      byte[] bytes = new byte[length];
      for (int offset1 = offset; offset1 < num; ++offset1)
        bytes[offset1 - offset] = this._memHolder.ReadByte(offset1);
      return bytes;
    }

    internal CTypes.INativeType NativeType
    {
      get => (CTypes.INativeType) DynamicHelpers.GetPythonType((object) this);
    }

    public virtual object _objects => (object) this._memHolder.Objects;

    internal void SetAddress(IntPtr address)
    {
      this._memHolder = new MemoryHolder(address, this.NativeType.Size);
    }

    internal virtual PythonTuple GetBufferInfo()
    {
      return PythonTuple.MakeTuple((object) this.NativeType.TypeFormat, (object) 0, (object) PythonTuple.EMPTY);
    }

    Bytes IBufferProtocol.GetItem(int index)
    {
      return new Bytes((IList<byte>) ((IPythonBufferable) this).GetBytes(index, this.NativeType.Size));
    }

    void IBufferProtocol.SetItem(int index, object value) => throw new NotImplementedException();

    void IBufferProtocol.SetSlice(IronPython.Runtime.Slice index, object value)
    {
      throw new NotImplementedException();
    }

    public virtual int ItemCount
    {
      [PythonHidden(new PlatformID[] {})] get => 1;
    }

    string IBufferProtocol.Format => this.NativeType.TypeFormat;

    public virtual BigInteger ItemSize
    {
      [PythonHidden(new PlatformID[] {})] get => (BigInteger) this.NativeType.Size;
    }

    BigInteger IBufferProtocol.NumberDimensions => (BigInteger) 0;

    bool IBufferProtocol.ReadOnly => false;

    [PythonHidden(new PlatformID[] {})]
    public virtual IList<BigInteger> GetShape(int start, int? end) => (IList<BigInteger>) null;

    PythonTuple IBufferProtocol.Strides => (PythonTuple) null;

    object IBufferProtocol.SubOffsets => (object) null;

    Bytes IBufferProtocol.ToBytes(int start, int? end)
    {
      return new Bytes((IList<byte>) ((IPythonBufferable) this).GetBytes(start, this.NativeType.Size));
    }

    IronPython.Runtime.List IBufferProtocol.ToList(int start, int? end)
    {
      return new IronPython.Runtime.List((object) ((IBufferProtocol) this).ToBytes(start, end));
    }
  }

  [PythonType("CFuncPtr")]
  public abstract class _CFuncPtr : CTypes.CData, IDynamicMetaObjectProvider, ICodeFormattable
  {
    private readonly Delegate _delegate;
    private readonly int _comInterfaceIndex = -1;
    private object _errcheck;
    private object _restype = CTypes._CFuncPtr._noResType;
    private IList<object> _argtypes;
    private int _id;
    private static int _curId = 0;
    internal static object _noResType = new object();

    public _CFuncPtr(PythonTuple args)
    {
      if (args == null)
        throw PythonOps.TypeError("expected sequence, got None");
      object obj = args.Count == 2 ? args[0] : throw PythonOps.TypeError($"argument 1 must be a sequence of length 2, not {args.Count}");
      IntPtr handleFromObject = CTypes.GetHandleFromObject(args[1], "the _handle attribute of the second element must be an integer");
      IntPtr num = !(args[0] is string functionName) ? NativeFunctions.LoadFunction(handleFromObject, new IntPtr((int) obj)) : NativeFunctions.LoadFunction(handleFromObject, functionName);
      if (num == IntPtr.Zero)
      {
        if (this.CallingConvention == CallingConvention.StdCall && functionName != null)
        {
          string str = $"_{functionName}@";
          for (int index = 0; index < 128 /*0x80*/ && num == IntPtr.Zero; index += 4)
            num = NativeFunctions.LoadFunction(handleFromObject, str + (object) index);
        }
        if (num == IntPtr.Zero)
          throw PythonOps.AttributeError($"function {args[0]} is not defined");
      }
      this._memHolder = new MemoryHolder(IntPtr.Size);
      this.addr = num;
      this._id = Interlocked.Increment(ref CTypes._CFuncPtr._curId);
    }

    public _CFuncPtr()
    {
      this._id = Interlocked.Increment(ref CTypes._CFuncPtr._curId);
      this._memHolder = new MemoryHolder(IntPtr.Size);
    }

    public _CFuncPtr(CodeContext context, object function)
    {
      this._memHolder = new MemoryHolder(IntPtr.Size);
      if (function != null)
      {
        this._delegate = PythonOps.IsCallable(context, function) ? ((CTypes.CFuncPtrType) DynamicHelpers.GetPythonType((object) this)).MakeReverseDelegate(context, function) : throw PythonOps.TypeError("argument must be called or address of function");
        this.addr = Marshal.GetFunctionPointerForDelegate(this._delegate);
        PythonType restype = ((CTypes.CFuncPtrType) this.NativeType)._restype;
        if (restype != null && (!(restype is CTypes.INativeType) || restype is CTypes.PointerType))
          throw PythonOps.TypeError($"invalid result type {restype.Name} for callback function");
      }
      this._id = Interlocked.Increment(ref CTypes._CFuncPtr._curId);
    }

    public _CFuncPtr(int index, string name)
    {
      this._memHolder = new MemoryHolder(IntPtr.Size);
      this._comInterfaceIndex = index;
      this._id = Interlocked.Increment(ref CTypes._CFuncPtr._curId);
    }

    public _CFuncPtr(int handle)
    {
      this._memHolder = new MemoryHolder(IntPtr.Size);
      this.addr = new IntPtr(handle);
      this._id = Interlocked.Increment(ref CTypes._CFuncPtr._curId);
    }

    public _CFuncPtr([NotNull] BigInteger handle)
    {
      this._memHolder = new MemoryHolder(IntPtr.Size);
      this.addr = new IntPtr((long) handle);
      this._id = Interlocked.Increment(ref CTypes._CFuncPtr._curId);
    }

    public _CFuncPtr(IntPtr handle)
    {
      this._memHolder = new MemoryHolder(IntPtr.Size);
      this.addr = handle;
      this._id = Interlocked.Increment(ref CTypes._CFuncPtr._curId);
    }

    public bool __nonzero__() => this.addr != IntPtr.Zero;

    [PropertyMethod]
    [SpecialName]
    public object Geterrcheck() => this._errcheck;

    [PropertyMethod]
    [SpecialName]
    public void Seterrcheck(object value) => this._errcheck = value;

    [PropertyMethod]
    [SpecialName]
    public void Deleteerrcheck()
    {
      this._errcheck = (object) null;
      this._id = Interlocked.Increment(ref CTypes._CFuncPtr._curId);
    }

    [PropertyMethod]
    [SpecialName]
    public object Getrestype()
    {
      return this._restype == CTypes._CFuncPtr._noResType ? (object) ((CTypes.CFuncPtrType) this.NativeType)._restype : this._restype;
    }

    [PropertyMethod]
    [SpecialName]
    public void Setrestype(object value)
    {
      if (!(value is CTypes.INativeType) && value != null && !PythonOps.IsCallable(((PythonType) this.NativeType).Context.SharedContext, value))
        throw PythonOps.TypeError("restype must be a type, a callable, or None");
      this._restype = value;
      this._id = Interlocked.Increment(ref CTypes._CFuncPtr._curId);
    }

    [PropertyMethod]
    [SpecialName]
    public void Deleterestype()
    {
      this._restype = CTypes._CFuncPtr._noResType;
      this._id = Interlocked.Increment(ref CTypes._CFuncPtr._curId);
    }

    public object argtypes
    {
      get
      {
        if (this._argtypes != null)
          return (object) this._argtypes;
        return ((CTypes.CFuncPtrType) this.NativeType)._argtypes != null ? (object) PythonTuple.MakeTuple((object[]) ((CTypes.CFuncPtrType) this.NativeType)._argtypes) : (object) null;
      }
      set
      {
        if (value != null)
        {
          if (!(value is IList<object> objectList))
            throw PythonOps.TypeErrorForTypeMismatch("sequence", value);
          foreach (object obj in (IEnumerable<object>) objectList)
          {
            if (!(obj is CTypes.INativeType) && !PythonOps.HasAttr(DefaultContext.Default, obj, "from_param"))
              throw PythonOps.TypeErrorForTypeMismatch("ctype or object with from_param", obj);
          }
          this._argtypes = objectList;
        }
        else
          this._argtypes = (IList<object>) null;
        this._id = Interlocked.Increment(ref CTypes._CFuncPtr._curId);
      }
    }

    internal CallingConvention CallingConvention
    {
      get => ((CTypes.CFuncPtrType) DynamicHelpers.GetPythonType((object) this)).CallingConvention;
    }

    internal int Flags
    {
      get => ((CTypes.CFuncPtrType) DynamicHelpers.GetPythonType((object) this))._flags;
    }

    public IntPtr addr
    {
      [PythonHidden(new PlatformID[] {})] get => this._memHolder.ReadIntPtr(0);
      [PythonHidden(new PlatformID[] {})] set => this._memHolder.WriteIntPtr(0, value);
    }

    internal int Id => this._id;

    [PythonHidden(new PlatformID[] {})]
    public DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
    {
      return (DynamicMetaObject) new CTypes._CFuncPtr.Meta(parameter, this);
    }

    public string __repr__(CodeContext context)
    {
      return this._comInterfaceIndex != -1 ? $"<COM method offset {this._comInterfaceIndex}: {DynamicHelpers.GetPythonType((object) this).Name} at {this._id}>" : ObjectOps.__repr__((object) this);
    }

    private class Meta(System.Linq.Expressions.Expression parameter, CTypes._CFuncPtr func) : 
      MetaPythonObject(parameter, BindingRestrictions.Empty, (object) func)
    {
      public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
      {
        CodeContext sharedContext = PythonContext.GetPythonContext((DynamicMetaObjectBinder) binder).SharedContext;
        CTypes._CFuncPtr.Meta.ArgumentMarshaller[] argumentMarshallers = this.GetArgumentMarshallers(args);
        BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(this.Expression, this.Value.GetType()).Merge(BindingRestrictions.GetExpressionRestriction((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (ModuleOps).GetMethod("CheckFunctionId"), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Convert(this.Expression, typeof (CTypes._CFuncPtr)), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) this.Value.Id))));
        foreach (CTypes._CFuncPtr.Meta.ArgumentMarshaller argumentMarshaller in argumentMarshallers)
          restrictions = restrictions.Merge(argumentMarshaller.GetRestrictions());
        int length = args.Length;
        if (this.Value._comInterfaceIndex != -1)
          --length;
        if (this.Value._argtypes != null)
        {
          if (length < this.Value._argtypes.Count || this.Value.CallingConvention != CallingConvention.Cdecl && length > this.Value._argtypes.Count)
            return CTypes._CFuncPtr.Meta.IncorrectArgCount((DynamicMetaObjectBinder) binder, restrictions, this.Value._argtypes.Count, length);
        }
        else
        {
          CTypes.CFuncPtrType nativeType = (CTypes.CFuncPtrType) this.Value.NativeType;
          if (nativeType._argtypes != null && (length < nativeType._argtypes.Length || this.Value.CallingConvention != CallingConvention.Cdecl && length > nativeType._argtypes.Length))
            return CTypes._CFuncPtr.Meta.IncorrectArgCount((DynamicMetaObjectBinder) binder, restrictions, nativeType._argtypes.Length, length);
        }
        if (this.Value._comInterfaceIndex != -1 && args.Length == 0)
          return CTypes._CFuncPtr.Meta.NoThisParam((DynamicMetaObjectBinder) binder, restrictions);
        System.Linq.Expressions.Expression right = this.MakeCall(argumentMarshallers, this.GetNativeReturnType(), this.Value.Getrestype() == null, this.GetFunctionAddress(args));
        List<System.Linq.Expressions.Expression> block = new List<System.Linq.Expressions.Expression>();
        System.Linq.Expressions.Expression res;
        if (right.Type != typeof (void))
        {
          ParameterExpression left = System.Linq.Expressions.Expression.Parameter(right.Type, "ret");
          block.Add((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Assign((System.Linq.Expressions.Expression) left, right));
          this.AddKeepAlives(argumentMarshallers, block);
          block.Add((System.Linq.Expressions.Expression) left);
          res = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
          {
            left
          }, (IEnumerable<System.Linq.Expressions.Expression>) block);
        }
        else
        {
          block.Add(right);
          this.AddKeepAlives(argumentMarshallers, block);
          res = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Block((IEnumerable<System.Linq.Expressions.Expression>) block);
        }
        return new DynamicMetaObject(Microsoft.Scripting.Ast.Utils.Convert(this.AddReturnChecks(sharedContext, args, res), typeof (object)), restrictions);
      }

      private System.Linq.Expressions.Expression AddReturnChecks(
        CodeContext context,
        DynamicMetaObject[] args,
        System.Linq.Expressions.Expression res)
      {
        PythonContext languageContext = context.LanguageContext;
        object obj1 = this.Value.Getrestype();
        if (obj1 != null)
        {
          CTypes.INativeType o = obj1 as CTypes.INativeType;
          object ret = (object) null;
          if (o == null)
            ret = obj1;
          else if (!PythonOps.TryGetBoundAttr(context, (object) o, "_check_retval_", out ret))
            ret = (object) null;
          if (ret != null)
            res = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Dynamic((CallSiteBinder) languageContext.CompatInvoke(new CallInfo(1, new string[0])), typeof (object), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant(ret), res);
        }
        object obj2 = this.Value.Geterrcheck();
        if (obj2 != null)
          res = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Dynamic((CallSiteBinder) languageContext.CompatInvoke(new CallInfo(3, new string[0])), typeof (object), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant(obj2), res, this.Expression, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod("MakeTuple"), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NewArrayInit(typeof (object), ArrayUtils.ConvertAll<DynamicMetaObject, System.Linq.Expressions.Expression>(args, (Func<DynamicMetaObject, System.Linq.Expressions.Expression>) (x => Microsoft.Scripting.Ast.Utils.Convert(x.Expression, typeof (object)))))));
        return res;
      }

      private static DynamicMetaObject IncorrectArgCount(
        DynamicMetaObjectBinder binder,
        BindingRestrictions restrictions,
        int expected,
        int got)
      {
        return new DynamicMetaObject(binder.Throw((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod("TypeError"), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) $"this function takes {expected} arguments ({got} given)"), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NewArrayInit(typeof (object))), typeof (object)), restrictions);
      }

      private static DynamicMetaObject NoThisParam(
        DynamicMetaObjectBinder binder,
        BindingRestrictions restrictions)
      {
        return new DynamicMetaObject(binder.Throw((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (PythonOps).GetMethod("ValueError"), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) "native com method call without 'this' parameter"), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.NewArrayInit(typeof (object))), typeof (object)), restrictions);
      }

      private void AddKeepAlives(
        CTypes._CFuncPtr.Meta.ArgumentMarshaller[] signature,
        List<System.Linq.Expressions.Expression> block)
      {
        foreach (CTypes._CFuncPtr.Meta.ArgumentMarshaller argumentMarshaller in signature)
        {
          System.Linq.Expressions.Expression keepAlive = argumentMarshaller.GetKeepAlive();
          if (keepAlive != null)
            block.Add(keepAlive);
        }
      }

      private System.Linq.Expressions.Expression MakeCall(
        CTypes._CFuncPtr.Meta.ArgumentMarshaller[] signature,
        CTypes.INativeType nativeRetType,
        bool retVoid,
        System.Linq.Expressions.Expression address)
      {
        List<object> constantPool = new List<object>();
        MethodInfo interopInvoker = CTypes._CFuncPtr.Meta.CreateInteropInvoker(this.GetCallingConvention(), signature, nativeRetType, retVoid, constantPool);
        System.Linq.Expressions.Expression[] expressionArray = new System.Linq.Expressions.Expression[signature.Length + 2];
        expressionArray[0] = address;
        for (int index = 0; index < signature.Length; ++index)
          expressionArray[index + 1] = signature[index].ArgumentExpression;
        expressionArray[expressionArray.Length - 1] = (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) constantPool.ToArray());
        return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(interopInvoker, expressionArray);
      }

      private System.Linq.Expressions.Expression GetFunctionAddress(DynamicMetaObject[] args)
      {
        return this.Value._comInterfaceIndex == -1 ? (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Property((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Convert(this.Expression, typeof (CTypes._CFuncPtr)), "addr") : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (ModuleOps).GetMethod("GetInterfacePointer"), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (ModuleOps).GetMethod("GetPointer"), args[0].Expression), (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) this.Value._comInterfaceIndex));
      }

      private CallingConvention GetCallingConvention() => this.Value.CallingConvention;

      private CTypes.INativeType GetNativeReturnType()
      {
        return this.Value.Getrestype() as CTypes.INativeType;
      }

      private CTypes._CFuncPtr.Meta.ArgumentMarshaller[] GetArgumentMarshallers(
        DynamicMetaObject[] args)
      {
        CTypes.CFuncPtrType nativeType1 = (CTypes.CFuncPtrType) this.Value.NativeType;
        CTypes._CFuncPtr.Meta.ArgumentMarshaller[] argumentMarshallers = new CTypes._CFuncPtr.Meta.ArgumentMarshaller[args.Length];
        for (int index1 = 0; index1 < args.Length; ++index1)
        {
          DynamicMetaObject dynamicMetaObject = args[index1];
          object nativeType2 = (object) null;
          if (this.Value._comInterfaceIndex == -1 || index1 != 0)
          {
            int index2 = this.Value._comInterfaceIndex == -1 ? index1 : index1 - 1;
            if (this.Value._argtypes != null && index2 < this.Value._argtypes.Count)
              nativeType2 = this.Value._argtypes[index2];
            else if (nativeType1._argtypes != null && index2 < nativeType1._argtypes.Length)
              nativeType2 = (object) nativeType1._argtypes[index2];
          }
          argumentMarshallers[index1] = this.GetMarshaller(dynamicMetaObject.Expression, dynamicMetaObject.Value, index1, nativeType2);
        }
        return argumentMarshallers;
      }

      private CTypes._CFuncPtr.Meta.ArgumentMarshaller GetMarshaller(
        System.Linq.Expressions.Expression expr,
        object value,
        int index,
        object nativeType)
      {
        if (nativeType != null)
          return nativeType is CTypes.INativeType cdataType ? (CTypes._CFuncPtr.Meta.ArgumentMarshaller) new CTypes._CFuncPtr.Meta.CDataMarshaller(expr, CompilerHelpers.GetType(value), cdataType) : (CTypes._CFuncPtr.Meta.ArgumentMarshaller) new CTypes._CFuncPtr.Meta.FromParamMarshaller(expr);
        if (value is CTypes.CData cdata)
          return (CTypes._CFuncPtr.Meta.ArgumentMarshaller) new CTypes._CFuncPtr.Meta.CDataMarshaller(expr, CompilerHelpers.GetType(value), cdata.NativeType);
        if (value is CTypes.NativeArgument)
          return (CTypes._CFuncPtr.Meta.ArgumentMarshaller) new CTypes._CFuncPtr.Meta.NativeArgumentMarshaller(expr);
        return !PythonOps.TryGetBoundAttr(value, "_as_parameter_", out object _) ? (CTypes._CFuncPtr.Meta.ArgumentMarshaller) new CTypes._CFuncPtr.Meta.PrimitiveMarshaller(expr, CompilerHelpers.GetType(value)) : throw new NotImplementedException("_as_parameter");
      }

      public CTypes._CFuncPtr Value => (CTypes._CFuncPtr) base.Value;

      private static MethodInfo CreateInteropInvoker(
        CallingConvention convention,
        CTypes._CFuncPtr.Meta.ArgumentMarshaller[] sig,
        CTypes.INativeType nativeRetType,
        bool retVoid,
        List<object> constantPool)
      {
        Type[] parameterTypes = new Type[sig.Length + 2];
        parameterTypes[0] = typeof (IntPtr);
        for (int index = 0; index < sig.Length; ++index)
          parameterTypes[index + 1] = sig[index].ArgumentExpression.Type;
        parameterTypes[parameterTypes.Length - 1] = typeof (object[]);
        Type returnType = retVoid ? typeof (void) : (nativeRetType != null ? nativeRetType.GetPythonType() : typeof (int));
        Type type = retVoid ? typeof (void) : (nativeRetType != null ? nativeRetType.GetNativeType() : typeof (int));
        DynamicMethod interopInvoker = new DynamicMethod("InteropInvoker", returnType, parameterTypes, (System.Reflection.Module) CTypes.DynamicModule);
        ILGenerator ilGenerator = interopInvoker.GetILGenerator();
        LocalBuilder local1 = (LocalBuilder) null;
        LocalBuilder local2 = (LocalBuilder) null;
        if (interopInvoker.ReturnType != typeof (void))
        {
          local1 = ilGenerator.DeclareLocal(type);
          local2 = ilGenerator.DeclareLocal(interopInvoker.ReturnType);
        }
        ilGenerator.BeginExceptionBlock();
        List<MarshalCleanup> marshalCleanupList = (List<MarshalCleanup>) null;
        for (int index = 0; index < sig.Length; ++index)
        {
          MarshalCleanup marshalCleanup = sig[index].EmitCallStubArgument(ilGenerator, index + 1, constantPool, parameterTypes.Length - 1);
          if (marshalCleanup != null)
          {
            if (marshalCleanupList == null)
              marshalCleanupList = new List<MarshalCleanup>();
            marshalCleanupList.Add(marshalCleanup);
          }
        }
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Calli, CTypes._CFuncPtr.Meta.GetCalliSignature(convention, sig, type));
        if (returnType != typeof (void))
        {
          if (nativeRetType != null)
          {
            ilGenerator.Emit(OpCodes.Stloc, local1);
            nativeRetType.EmitReverseMarshalling(ilGenerator, (LocalOrArg) new Local(local1), constantPool, sig.Length + 1);
            ilGenerator.Emit(OpCodes.Stloc, local2);
          }
          else
            ilGenerator.Emit(OpCodes.Stloc, local2);
        }
        ilGenerator.BeginFinallyBlock();
        if (marshalCleanupList != null)
        {
          foreach (MarshalCleanup marshalCleanup in marshalCleanupList)
            marshalCleanup.Cleanup(ilGenerator);
        }
        ilGenerator.EndExceptionBlock();
        if (returnType != typeof (void))
          ilGenerator.Emit(OpCodes.Ldloc, local2);
        ilGenerator.Emit(OpCodes.Ret);
        return (MethodInfo) interopInvoker;
      }

      private static SignatureHelper GetMethodSigHelper(
        CallingConvention convention,
        Type calliRetType)
      {
        return SignatureHelper.GetMethodSigHelper(convention, calliRetType);
      }

      private static SignatureHelper GetCalliSignature(
        CallingConvention convention,
        CTypes._CFuncPtr.Meta.ArgumentMarshaller[] sig,
        Type calliRetType)
      {
        SignatureHelper methodSigHelper = CTypes._CFuncPtr.Meta.GetMethodSigHelper(convention, calliRetType);
        foreach (CTypes._CFuncPtr.Meta.ArgumentMarshaller argumentMarshaller in sig)
          methodSigHelper.AddArgument(argumentMarshaller.NativeType);
        return methodSigHelper;
      }

      private abstract class ArgumentMarshaller
      {
        private readonly System.Linq.Expressions.Expression _argExpr;

        public ArgumentMarshaller(System.Linq.Expressions.Expression container)
        {
          this._argExpr = container;
        }

        public abstract MarshalCleanup EmitCallStubArgument(
          ILGenerator generator,
          int argIndex,
          List<object> constantPool,
          int constantPoolArgument);

        public abstract Type NativeType { get; }

        public System.Linq.Expressions.Expression ArgumentExpression => this._argExpr;

        public virtual System.Linq.Expressions.Expression GetKeepAlive() => (System.Linq.Expressions.Expression) null;

        public virtual BindingRestrictions GetRestrictions() => BindingRestrictions.Empty;
      }

      private class PrimitiveMarshaller : CTypes._CFuncPtr.Meta.ArgumentMarshaller
      {
        private readonly Type _type;
        private static MethodInfo _bigIntToInt32;

        private static MethodInfo BigIntToInt32
        {
          get
          {
            if (CTypes._CFuncPtr.Meta.PrimitiveMarshaller._bigIntToInt32 == (MethodInfo) null)
            {
              foreach (MethodInfo methodInfo in typeof (BigInteger).GetMember("op_Explicit", MemberTypes.Method, BindingFlags.Static | BindingFlags.Public))
              {
                if (methodInfo.ReturnType == typeof (int))
                {
                  CTypes._CFuncPtr.Meta.PrimitiveMarshaller._bigIntToInt32 = methodInfo;
                  break;
                }
              }
            }
            return CTypes._CFuncPtr.Meta.PrimitiveMarshaller._bigIntToInt32;
          }
        }

        public PrimitiveMarshaller(System.Linq.Expressions.Expression container, Type type)
          : base(container)
        {
          this._type = type;
        }

        public override MarshalCleanup EmitCallStubArgument(
          ILGenerator generator,
          int argIndex,
          List<object> constantPool,
          int constantPoolArgument)
        {
          if (this._type == typeof (DynamicNull))
          {
            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Conv_I);
            return (MarshalCleanup) null;
          }
          generator.Emit(OpCodes.Ldarg, argIndex);
          if (this.ArgumentExpression.Type != this._type)
            generator.Emit(OpCodes.Unbox_Any, this._type);
          if (this._type == typeof (string))
          {
            LocalBuilder local = generator.DeclareLocal(typeof (string), true);
            generator.Emit(OpCodes.Stloc, local);
            generator.Emit(OpCodes.Ldloc, local);
            generator.Emit(OpCodes.Conv_I);
            generator.Emit(OpCodes.Ldc_I4, RuntimeHelpers.OffsetToStringData);
            generator.Emit(OpCodes.Add);
          }
          else if (this._type == typeof (Bytes))
          {
            LocalBuilder local = generator.DeclareLocal(typeof (byte).MakeByRefType(), true);
            generator.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("GetBytes"));
            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Ldelema, typeof (byte));
            generator.Emit(OpCodes.Stloc, local);
            generator.Emit(OpCodes.Ldloc, local);
          }
          else if (this._type == typeof (BigInteger))
            generator.Emit(OpCodes.Call, CTypes._CFuncPtr.Meta.PrimitiveMarshaller.BigIntToInt32);
          else if (!this._type.IsValueType)
            generator.Emit(OpCodes.Call, typeof (CTypes).GetMethod("PyObj_ToPtr"));
          return (MarshalCleanup) null;
        }

        public override Type NativeType
        {
          get
          {
            if (this._type == typeof (BigInteger))
              return typeof (int);
            return !this._type.IsValueType ? typeof (IntPtr) : this._type;
          }
        }

        public override BindingRestrictions GetRestrictions()
        {
          return this._type == typeof (DynamicNull) ? BindingRestrictions.GetExpressionRestriction((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Equal(this.ArgumentExpression, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Constant((object) null))) : BindingRestrictions.GetTypeRestriction(this.ArgumentExpression, this._type);
        }
      }

      private class FromParamMarshaller(System.Linq.Expressions.Expression container) : 
        CTypes._CFuncPtr.Meta.ArgumentMarshaller(container)
      {
        public override MarshalCleanup EmitCallStubArgument(
          ILGenerator generator,
          int argIndex,
          List<object> constantPool,
          int constantPoolArgument)
        {
          throw new NotImplementedException();
        }

        public override Type NativeType => throw new NotImplementedException();
      }

      private class CDataMarshaller : CTypes._CFuncPtr.Meta.ArgumentMarshaller
      {
        private readonly Type _type;
        private readonly CTypes.INativeType _cdataType;

        public CDataMarshaller(System.Linq.Expressions.Expression container, Type type, CTypes.INativeType cdataType)
          : base(container)
        {
          this._type = type;
          this._cdataType = cdataType;
        }

        public override MarshalCleanup EmitCallStubArgument(
          ILGenerator generator,
          int argIndex,
          List<object> constantPool,
          int constantPoolArgument)
        {
          return this._cdataType.EmitMarshalling(generator, (LocalOrArg) new Arg(argIndex, this.ArgumentExpression.Type), constantPool, constantPoolArgument);
        }

        public override Type NativeType => this._cdataType.GetNativeType();

        public override System.Linq.Expressions.Expression GetKeepAlive()
        {
          return this._type.IsValueType ? (System.Linq.Expressions.Expression) null : (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (GC).GetMethod("KeepAlive"), this.ArgumentExpression);
        }

        public override BindingRestrictions GetRestrictions() => BindingRestrictions.Empty;
      }

      private class NativeArgumentMarshaller(System.Linq.Expressions.Expression container) : 
        CTypes._CFuncPtr.Meta.ArgumentMarshaller(container)
      {
        public override MarshalCleanup EmitCallStubArgument(
          ILGenerator generator,
          int argIndex,
          List<object> constantPool,
          int constantPoolArgument)
        {
          generator.Emit(OpCodes.Ldarg, argIndex);
          generator.Emit(OpCodes.Castclass, typeof (CTypes.NativeArgument));
          generator.Emit(OpCodes.Call, typeof (CTypes.NativeArgument).GetMethod("get__obj"));
          generator.Emit(OpCodes.Call, typeof (CTypes.CData).GetMethod("get_UnsafeAddress"));
          return (MarshalCleanup) null;
        }

        public override Type NativeType => typeof (IntPtr);

        public override System.Linq.Expressions.Expression GetKeepAlive()
        {
          return (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Call(typeof (GC).GetMethod("KeepAlive"), this.ArgumentExpression);
        }

        public override BindingRestrictions GetRestrictions()
        {
          return BindingRestrictions.GetTypeRestriction(this.ArgumentExpression, typeof (CTypes.NativeArgument));
        }
      }
    }
  }

  [PythonType]
  [PythonHidden(new PlatformID[] {})]
  public class CFuncPtrType : PythonType, CTypes.INativeType
  {
    internal readonly int _flags;
    internal readonly PythonType _restype;
    internal readonly CTypes.INativeType[] _argtypes;
    private DynamicMethod _reverseDelegate;
    private List<object> _reverseDelegateConstants;
    private Type _reverseDelegateType;
    private static Dictionary<CTypes.CFuncPtrType.DelegateCacheKey, Type> _reverseDelegates = new Dictionary<CTypes.CFuncPtrType.DelegateCacheKey, Type>();

    public CFuncPtrType(
      CodeContext context,
      string name,
      PythonTuple bases,
      PythonDictionary members)
      : base(context, name, bases, members)
    {
      object obj1;
      if (!members.TryGetValue((object) "_flags_", out obj1) || !(obj1 is int num))
        throw PythonOps.TypeError("class must define _flags_ which must be an integer");
      this._flags = num;
      object obj2;
      if (members.TryGetValue((object) "_restype_", out obj2) && obj2 is PythonType)
        this._restype = (PythonType) obj2;
      object obj3;
      if (!members.TryGetValue((object) "_argtypes_", out obj3) || !(obj3 is PythonTuple))
        return;
      PythonTuple pythonTuple = obj3 as PythonTuple;
      this._argtypes = new CTypes.INativeType[pythonTuple.Count];
      for (int index = 0; index < pythonTuple.Count; ++index)
        this._argtypes[index] = (CTypes.INativeType) pythonTuple[index];
    }

    private CFuncPtrType(Type underlyingSystemType)
      : base(underlyingSystemType)
    {
    }

    internal static PythonType MakeSystemType(Type underlyingSystemType)
    {
      return PythonType.SetPythonType(underlyingSystemType, (PythonType) new CTypes.CFuncPtrType(underlyingSystemType));
    }

    public object from_param(object obj) => (object) null;

    public object internal_restype => (object) this._restype;

    int CTypes.INativeType.Size => IntPtr.Size;

    int CTypes.INativeType.Alignment => IntPtr.Size;

    object CTypes.INativeType.GetValue(
      MemoryHolder owner,
      object readingFrom,
      int offset,
      bool raw)
    {
      IntPtr handle = owner.ReadIntPtr(offset);
      return raw ? handle.ToPython() : this.CreateInstance(this.Context.SharedContext, (object) handle);
    }

    object CTypes.INativeType.SetValue(MemoryHolder address, int offset, object value)
    {
      switch (value)
      {
        case int num:
          address.WriteIntPtr(offset, new IntPtr(num));
          break;
        case BigInteger bigInteger:
          address.WriteIntPtr(offset, new IntPtr((long) bigInteger));
          break;
        case CTypes._CFuncPtr _:
          address.WriteIntPtr(offset, ((CTypes._CFuncPtr) value).addr);
          return value;
        default:
          throw PythonOps.TypeErrorForTypeMismatch("func pointer", value);
      }
      return (object) null;
    }

    Type CTypes.INativeType.GetNativeType() => typeof (IntPtr);

    MarshalCleanup CTypes.INativeType.EmitMarshalling(
      ILGenerator method,
      LocalOrArg argIndex,
      List<object> constantPool,
      int constantPoolArgument)
    {
      Type type = argIndex.Type;
      argIndex.Emit(method);
      if (type.IsValueType)
        method.Emit(OpCodes.Box, type);
      constantPool.Add((object) this);
      method.Emit(OpCodes.Ldarg, constantPoolArgument);
      method.Emit(OpCodes.Ldc_I4, constantPool.Count - 1);
      method.Emit(OpCodes.Ldelem_Ref);
      method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("GetFunctionPointerValue"));
      return (MarshalCleanup) null;
    }

    Type CTypes.INativeType.GetPythonType() => typeof (CTypes._CFuncPtr);

    void CTypes.INativeType.EmitReverseMarshalling(
      ILGenerator method,
      LocalOrArg value,
      List<object> constantPool,
      int constantPoolArgument)
    {
      value.Emit(method);
      constantPool.Add((object) this);
      method.Emit(OpCodes.Ldarg, constantPoolArgument);
      method.Emit(OpCodes.Ldc_I4, constantPool.Count - 1);
      method.Emit(OpCodes.Ldelem_Ref);
      method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("CreateCFunction"));
    }

    string CTypes.INativeType.TypeFormat => "X{}";

    internal CallingConvention CallingConvention
    {
      get
      {
        switch (this._flags & 7)
        {
          case 0:
            return CallingConvention.StdCall;
          case 1:
            return CallingConvention.Cdecl;
          default:
            return CallingConvention.Cdecl;
        }
      }
    }

    internal Delegate MakeReverseDelegate(CodeContext context, object target)
    {
      if ((MethodInfo) this._reverseDelegate == (MethodInfo) null)
      {
        lock (this)
        {
          if ((MethodInfo) this._reverseDelegate == (MethodInfo) null)
            this.MakeReverseDelegateWorker(context);
        }
      }
      object[] array = this._reverseDelegateConstants.ToArray();
      array[0] = target;
      return ((MethodInfo) this._reverseDelegate).CreateDelegate(this._reverseDelegateType, (object) array);
    }

    private void MakeReverseDelegateWorker(CodeContext context)
    {
      Type[] sigTypes;
      Type[] callSiteType;
      Type retType;
      this.GetSignatureInfo(out sigTypes, out callSiteType, out retType);
      DynamicMethod dynamicMethod = new DynamicMethod("ReverseInteropInvoker", retType, ArrayUtils.RemoveLast<Type>(sigTypes), (System.Reflection.Module) CTypes.DynamicModule);
      ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
      PythonContext languageContext = context.LanguageContext;
      Type delegateType = CompilerHelpers.MakeCallSiteDelegateType(callSiteType);
      CallSite callSite = CallSite.Create(delegateType, (CallSiteBinder) languageContext.Invoke(new CallSignature(this._argtypes.Length)));
      List<object> constantPool = new List<object>();
      constantPool.Add((object) null);
      constantPool.Add((object) callSite);
      ilGenerator.BeginExceptionBlock();
      LocalBuilder local1 = ilGenerator.DeclareLocal(callSite.GetType());
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldc_I4, constantPool.Count - 1);
      ilGenerator.Emit(OpCodes.Ldelem_Ref);
      ilGenerator.Emit(OpCodes.Castclass, callSite.GetType());
      ilGenerator.Emit(OpCodes.Stloc, local1);
      ilGenerator.Emit(OpCodes.Ldloc, local1);
      ilGenerator.Emit(OpCodes.Ldfld, callSite.GetType().GetField("Target"));
      ilGenerator.Emit(OpCodes.Ldloc, local1);
      int count = constantPool.Count;
      constantPool.Add((object) languageContext.SharedContext);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldc_I4, count);
      ilGenerator.Emit(OpCodes.Ldelem_Ref);
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldc_I4_0);
      ilGenerator.Emit(OpCodes.Ldelem_Ref);
      for (int index = 0; index < this._argtypes.Length; ++index)
        this._argtypes[index].EmitReverseMarshalling(ilGenerator, (LocalOrArg) new Arg(index + 1, sigTypes[index + 1]), constantPool, 0);
      ilGenerator.Emit(OpCodes.Call, delegateType.GetMethod("Invoke"));
      LocalBuilder local2 = (LocalBuilder) null;
      if (this._restype != null)
      {
        LocalBuilder local3 = ilGenerator.DeclareLocal(typeof (object));
        ilGenerator.Emit(OpCodes.Stloc, local3);
        local2 = ilGenerator.DeclareLocal(retType);
        ((CTypes.INativeType) this._restype).EmitMarshalling(ilGenerator, (LocalOrArg) new Local(local3), constantPool, 0);
        ilGenerator.Emit(OpCodes.Stloc, local2);
      }
      else
        ilGenerator.Emit(OpCodes.Pop);
      ilGenerator.BeginCatchBlock(typeof (Exception));
      ilGenerator.Emit(OpCodes.Ldarg_0);
      ilGenerator.Emit(OpCodes.Ldc_I4, count);
      ilGenerator.Emit(OpCodes.Ldelem_Ref);
      ilGenerator.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("CallbackException"));
      ilGenerator.EndExceptionBlock();
      if (this._restype != null)
        ilGenerator.Emit(OpCodes.Ldloc, local2);
      ilGenerator.Emit(OpCodes.Ret);
      this._reverseDelegateConstants = constantPool;
      this._reverseDelegateType = CTypes.CFuncPtrType.GetReverseDelegateType(ArrayUtils.RemoveFirst<Type>(sigTypes), this.CallingConvention);
      this._reverseDelegate = dynamicMethod;
    }

    private void GetSignatureInfo(out Type[] sigTypes, out Type[] callSiteType, out Type retType)
    {
      sigTypes = new Type[this._argtypes.Length + 2];
      callSiteType = new Type[this._argtypes.Length + 4];
      sigTypes[0] = typeof (object[]);
      callSiteType[0] = typeof (CallSite);
      callSiteType[1] = typeof (CodeContext);
      callSiteType[2] = typeof (object);
      callSiteType[callSiteType.Length - 1] = typeof (object);
      for (int index = 0; index < this._argtypes.Length; ++index)
      {
        sigTypes[index + 1] = this._argtypes[index].GetNativeType();
        callSiteType[index + 3] = this._argtypes[index].GetPythonType();
      }
      if (this._restype != null)
        sigTypes[sigTypes.Length - 1] = retType = ((CTypes.INativeType) this._restype).GetNativeType();
      else
        sigTypes[sigTypes.Length - 1] = retType = typeof (void);
    }

    private static Type GetReverseDelegateType(
      Type[] nativeSig,
      CallingConvention callingConvention)
    {
      Type reverseDelegateType;
      lock (CTypes.CFuncPtrType._reverseDelegates)
      {
        CTypes.CFuncPtrType.DelegateCacheKey key = new CTypes.CFuncPtrType.DelegateCacheKey(nativeSig, callingConvention);
        if (!CTypes.CFuncPtrType._reverseDelegates.TryGetValue(key, out reverseDelegateType))
          reverseDelegateType = CTypes.CFuncPtrType._reverseDelegates[key] = PythonOps.MakeNewCustomDelegate(nativeSig, new CallingConvention?(callingConvention));
      }
      return reverseDelegateType;
    }

    private struct DelegateCacheKey(Type[] sig, CallingConvention callingConvention) : 
      IEquatable<CTypes.CFuncPtrType.DelegateCacheKey>
    {
      private readonly Type[] _types = sig;
      private readonly CallingConvention _callConv = callingConvention;

      public override int GetHashCode()
      {
        int hashCode = this._callConv.GetHashCode();
        for (int index = 0; index < this._types.Length; ++index)
          hashCode ^= this._types[index].GetHashCode();
        return hashCode;
      }

      public override bool Equals(object obj)
      {
        return obj is CTypes.CFuncPtrType.DelegateCacheKey other && this.Equals(other);
      }

      public bool Equals(CTypes.CFuncPtrType.DelegateCacheKey other)
      {
        if (other._types.Length != this._types.Length || other._callConv != this._callConv)
          return false;
        for (int index = 0; index < this._types.Length; ++index)
        {
          if (this._types[index] != other._types[index])
            return false;
        }
        return true;
      }
    }
  }

  [PythonType]
  [PythonHidden(new PlatformID[] {})]
  public sealed class Field : PythonTypeDataSlot, ICodeFormattable
  {
    private readonly CTypes.INativeType _fieldType;
    private readonly int _offset;
    private readonly int _index;
    private readonly int _bits = -1;
    private readonly int _bitsOffset;
    private readonly string _fieldName;

    internal Field(string fieldName, CTypes.INativeType fieldType, int offset, int index)
    {
      this._offset = offset;
      this._fieldType = fieldType;
      this._index = index;
      this._fieldName = fieldName;
    }

    internal Field(
      string fieldName,
      CTypes.INativeType fieldType,
      int offset,
      int index,
      int? bits,
      int? bitOffset)
    {
      this._offset = offset;
      this._fieldType = fieldType;
      this._index = index;
      this._fieldName = fieldName;
      if (!bits.HasValue || bits.Value == this._fieldType.Size * 8)
        return;
      this._bits = bits.Value;
      this._bitsOffset = bitOffset.Value;
    }

    public int offset => this._offset;

    public int size => this._fieldType.Size;

    internal override bool TryGetValue(
      CodeContext context,
      object instance,
      PythonType owner,
      out object value)
    {
      if (instance != null)
      {
        CTypes.CData readingFrom = (CTypes.CData) instance;
        value = this._fieldType.GetValue(readingFrom._memHolder, (object) readingFrom, this._offset, false);
        if (this._bits == -1)
          return true;
        value = this.ExtractBits(value);
        return true;
      }
      value = (object) this;
      return true;
    }

    internal override bool GetAlwaysSucceeds => true;

    internal override bool TrySetValue(
      CodeContext context,
      object instance,
      PythonType owner,
      object value)
    {
      if (instance == null)
        return base.TrySetValue(context, instance, owner, value);
      this.SetValue(((CTypes.CData) instance)._memHolder, 0, value);
      return true;
    }

    internal void SetValue(MemoryHolder address, int baseOffset, object value)
    {
      if (this._bits == -1)
      {
        object obj = this._fieldType.SetValue(address, baseOffset + this._offset, value);
        if (obj == null)
          return;
        address.AddObject((object) this._index.ToString(), obj);
      }
      else
        this.SetBitsValue(address, baseOffset, value);
    }

    internal override bool TryDeleteValue(CodeContext context, object instance, PythonType owner)
    {
      throw PythonOps.TypeError("cannot delete fields in ctypes structures/unions");
    }

    internal CTypes.INativeType NativeType => this._fieldType;

    internal int? BitCount => this._bits == -1 ? new int?() : new int?(this._bits);

    internal string FieldName => this._fieldName;

    public string __repr__(CodeContext context)
    {
      if (this._bits == -1)
        return $"<Field type={((PythonType) this._fieldType).Name}, ofs={this.offset}, size={this.size}>";
      return $"<Field type={((PythonType) this._fieldType).Name}, ofs={this.offset}:{this._bitsOffset}, bits={this._bits}>";
    }

    private object ExtractBits(object value)
    {
      if (value is int num)
      {
        int num1 = (1 << this._bits) - 1;
        int num2 = num >> this._bitsOffset & num1;
        if (this.IsSignedType && (num2 & 1 << this._bits - 1) != 0)
          num2 |= -1 ^ num1;
        value = ScriptingRuntimeHelpers.Int32ToObject(num2);
      }
      else
      {
        ulong num3 = (ulong) (1L << this._bits) - 1UL;
        BigInteger bigInteger = (BigInteger) value;
        ulong num4 = (!this.IsSignedType ? (ulong) bigInteger : (ulong) (long) bigInteger) >> this._bitsOffset & num3;
        if (this.IsSignedType)
        {
          if (((long) num4 & 1L << this._bits - 1) != 0L)
            num4 |= ulong.MaxValue ^ num3;
          value = (object) (BigInteger) (long) num4;
        }
        else
          value = (object) (BigInteger) num4;
      }
      return value;
    }

    private void SetBitsValue(MemoryHolder address, int baseOffset, object value)
    {
      ulong num1;
      switch (value)
      {
        case int num2:
          num1 = (ulong) num2;
          break;
        case BigInteger bigInteger:
          num1 = (ulong) (long) bigInteger;
          break;
        default:
          throw PythonOps.TypeErrorForTypeMismatch("int or long", value);
      }
      int offset = checked (this._offset + baseOffset);
      object obj = this._fieldType.GetValue(address, (object) null, offset, false);
      ulong num3 = !(obj is int num4) ? (ulong) (long) (BigInteger) obj : (ulong) num4;
      ulong num5 = (ulong) ((1L << this._bits) - 1L << this._bitsOffset);
      ulong num6 = num3 & ~num5 | num1 << this._bitsOffset & num5;
      if (this.IsSignedType)
      {
        if (this._fieldType.Size <= 4)
          this._fieldType.SetValue(address, offset, (object) (int) num6);
        else
          this._fieldType.SetValue(address, offset, (object) (BigInteger) (long) num6);
      }
      else if (this._fieldType.Size < 4)
        this._fieldType.SetValue(address, offset, (object) (int) num6);
      else
        this._fieldType.SetValue(address, offset, (object) (BigInteger) num6);
    }

    private bool IsSignedType
    {
      get
      {
        switch (((CTypes.SimpleType) this._fieldType)._type)
        {
          case CTypes.SimpleTypeKind.SignedByte:
          case CTypes.SimpleTypeKind.SignedShort:
          case CTypes.SimpleTypeKind.SignedInt:
          case CTypes.SimpleTypeKind.SignedLong:
          case CTypes.SimpleTypeKind.SignedLongLong:
            return true;
          default:
            return false;
        }
      }
    }
  }

  internal interface INativeType
  {
    int Size { get; }

    int Alignment { get; }

    object GetValue(MemoryHolder owner, object readingFrom, int offset, bool raw);

    object SetValue(MemoryHolder address, int offset, object value);

    Type GetNativeType();

    Type GetPythonType();

    MarshalCleanup EmitMarshalling(
      ILGenerator method,
      LocalOrArg argIndex,
      List<object> constantPool,
      int constantPoolArgument);

    void EmitReverseMarshalling(
      ILGenerator method,
      LocalOrArg value,
      List<object> constantPool,
      int constantPoolArgument);

    string TypeFormat { get; }
  }

  [PythonType]
  [PythonHidden(new PlatformID[] {})]
  public sealed class NativeArgument : ICodeFormattable
  {
    private readonly CTypes.CData __obj;
    private readonly string _type;

    internal NativeArgument(CTypes.CData value, string type)
    {
      this.__obj = value;
      this._type = type;
    }

    public CTypes.CData _obj => this.__obj;

    public string __repr__(CodeContext context)
    {
      return $"<cparam '{this._type}' ({IdDispenser.GetId((object) this.__obj)})>";
    }
  }

  [PythonType("_Pointer")]
  public abstract class Pointer : CTypes.CData
  {
    private readonly CTypes.CData _object;

    public Pointer() => this._memHolder = new MemoryHolder(IntPtr.Size);

    public Pointer(CTypes.CData value)
    {
      this._object = value;
      this._memHolder = new MemoryHolder(IntPtr.Size);
      this._memHolder.WriteIntPtr(0, value._memHolder);
      this._memHolder.AddObject((object) "1", (object) value);
      if (value._objects == null)
        return;
      this._memHolder.AddObject((object) "0", value._objects);
    }

    public object contents
    {
      get
      {
        PythonType type = (PythonType) ((CTypes.PointerType) this.NativeType)._type;
        CTypes.CData instance = (CTypes.CData) type.CreateInstance(type.Context.SharedContext);
        instance._memHolder = this._memHolder.ReadMemoryHolder(0);
        if (instance._memHolder.UnsafeAddress == IntPtr.Zero)
          throw PythonOps.ValueError("NULL value access");
        return (object) instance;
      }
      set
      {
      }
    }

    public object this[int index]
    {
      get
      {
        CTypes.INativeType type = ((CTypes.PointerType) this.NativeType)._type;
        MemoryHolder owner = this._memHolder.ReadMemoryHolder(0);
        return type.GetValue(owner, (object) this, checked (type.Size * index), false);
      }
      set
      {
        MemoryHolder address = this._memHolder.ReadMemoryHolder(0);
        CTypes.INativeType type = ((CTypes.PointerType) this.NativeType)._type;
        object obj = type.SetValue(address, checked (type.Size * index), value);
        if (obj == null)
          return;
        this._memHolder.AddObject((object) index.ToString(), obj);
      }
    }

    public bool __nonzero__() => this._memHolder.ReadIntPtr(0) != IntPtr.Zero;

    public object this[IronPython.Runtime.Slice index]
    {
      get
      {
        if (index.stop == null)
          throw PythonOps.ValueError("slice stop is required");
        int num1 = index.start != null ? (int) index.start : 0;
        int stop = index.stop != null ? (int) index.stop : 0;
        int num2 = index.step != null ? (int) index.step : 1;
        if (num2 < 0 && index.start == null)
          throw PythonOps.ValueError("slice start is required for step < 0");
        if (num1 < 0)
          num1 = 0;
        CTypes.INativeType type = ((CTypes.PointerType) this.NativeType)._type;
        CTypes.SimpleType simpleType = type as CTypes.SimpleType;
        if (stop < num1 && num2 > 0 || num1 < stop && num2 < 0)
          return simpleType != null && (simpleType._type == CTypes.SimpleTypeKind.WChar || simpleType._type == CTypes.SimpleTypeKind.Char) ? (object) string.Empty : (object) new IronPython.Runtime.List();
        MemoryHolder owner = this._memHolder.ReadMemoryHolder(0);
        if (simpleType != null && (simpleType._type == CTypes.SimpleTypeKind.WChar || simpleType._type == CTypes.SimpleTypeKind.Char))
        {
          int size = ((CTypes.INativeType) simpleType).Size;
          StringBuilder stringBuilder = new StringBuilder();
          for (int index1 = num1; (stop > num1 ? (index1 < stop ? 1 : 0) : (index1 > stop ? 1 : 0)) != 0; index1 += num2)
            stringBuilder.Append(simpleType.ReadChar(owner, checked (index1 * size)));
          return (object) stringBuilder.ToString();
        }
        IronPython.Runtime.List list = new IronPython.Runtime.List((stop - num1) / num2);
        for (int index2 = num1; (stop > num1 ? (index2 < stop ? 1 : 0) : (index2 > stop ? 1 : 0)) != 0; index2 += num2)
          list.AddNoLock(type.GetValue(owner, (object) this, checked (type.Size * index2), false));
        return (object) list;
      }
    }
  }

  [PythonType]
  [PythonHidden(new PlatformID[] {})]
  public class PointerType : PythonType, CTypes.INativeType
  {
    internal CTypes.INativeType _type;
    private readonly string _typeFormat;

    public PointerType(
      CodeContext context,
      string name,
      PythonTuple bases,
      PythonDictionary members)
      : base(context, name, bases, members)
    {
      object obj;
      this._type = !members.TryGetValue((object) "_type_", out obj) || obj is CTypes.INativeType ? (CTypes.INativeType) obj : throw PythonOps.TypeError("_type_ must be a type");
      if (this._type == null)
        return;
      this._typeFormat = this._type.TypeFormat;
    }

    private PointerType(Type underlyingSystemType)
      : base(underlyingSystemType)
    {
    }

    public object from_param([NotNull] CTypes.CData obj)
    {
      return (object) new CTypes.NativeArgument((CTypes.CData) PythonCalls.Call((object) this, (object) obj), "P");
    }

    public object from_param(CTypes.Pointer obj)
    {
      if (obj == null)
        return ScriptingRuntimeHelpers.Int32ToObject(0);
      if (obj.NativeType != this)
        throw PythonOps.TypeError("assign to pointer of type {0} from {1} is not valid", (object) this.Name, (object) ((PythonType) obj.NativeType).Name);
      CTypes.Pointer pointer = (CTypes.Pointer) PythonCalls.Call((object) this);
      pointer._memHolder.WriteIntPtr(0, obj._memHolder.ReadMemoryHolder(0));
      return (object) pointer;
    }

    public object from_param([NotNull] CTypes.NativeArgument obj)
    {
      return (object) (CTypes.CData) PythonCalls.Call((object) this, (object) obj._obj);
    }

    public object from_address(object obj)
    {
      throw new NotImplementedException("pointer from address");
    }

    public void set_type(PythonType type) => this._type = (CTypes.INativeType) type;

    internal static PythonType MakeSystemType(Type underlyingSystemType)
    {
      return PythonType.SetPythonType(underlyingSystemType, (PythonType) new CTypes.PointerType(underlyingSystemType));
    }

    public static CTypes.ArrayType operator *(CTypes.PointerType type, int count)
    {
      return CTypes.MakeArrayType((PythonType) type, count);
    }

    public static CTypes.ArrayType operator *(int count, CTypes.PointerType type)
    {
      return CTypes.MakeArrayType((PythonType) type, count);
    }

    int CTypes.INativeType.Size => IntPtr.Size;

    int CTypes.INativeType.Alignment => IntPtr.Size;

    object CTypes.INativeType.GetValue(
      MemoryHolder owner,
      object readingFrom,
      int offset,
      bool raw)
    {
      if (raw)
        return owner.ReadIntPtr(offset).ToPython();
      CTypes.Pointer pointer = (CTypes.Pointer) PythonCalls.Call(this.Context.SharedContext, (object) this);
      pointer._memHolder.WriteIntPtr(0, owner.ReadIntPtr(offset));
      pointer._memHolder.AddObject((object) offset, readingFrom);
      return (object) pointer;
    }

    object CTypes.INativeType.SetValue(MemoryHolder address, int offset, object value)
    {
      switch (value)
      {
        case null:
          address.WriteIntPtr(offset, IntPtr.Zero);
          break;
        case int num:
          address.WriteIntPtr(offset, new IntPtr(num));
          break;
        case BigInteger bigInteger:
          address.WriteIntPtr(offset, new IntPtr((long) bigInteger));
          break;
        case CTypes.Pointer pointer:
          address.WriteIntPtr(offset, pointer._memHolder.ReadMemoryHolder(0));
          return (object) PythonOps.MakeDictFromItems((object) pointer, (object) "0", pointer._objects, (object) "1");
        case CTypes._Array array:
          address.WriteIntPtr(offset, array._memHolder);
          return (object) array;
        default:
          throw PythonOps.TypeErrorForTypeMismatch(this.Name, value);
      }
      return (object) null;
    }

    Type CTypes.INativeType.GetNativeType() => typeof (IntPtr);

    MarshalCleanup CTypes.INativeType.EmitMarshalling(
      ILGenerator method,
      LocalOrArg argIndex,
      List<object> constantPool,
      int constantPoolArgument)
    {
      Type type1 = argIndex.Type;
      Label label1 = method.DefineLabel();
      Label label2 = method.DefineLabel();
      if (!type1.IsValueType)
      {
        argIndex.Emit(method);
        method.Emit(OpCodes.Ldnull);
        method.Emit(OpCodes.Bne_Un, label1);
        method.Emit(OpCodes.Ldc_I4_0);
        method.Emit(OpCodes.Conv_I);
        method.Emit(OpCodes.Br, label2);
      }
      method.MarkLabel(label1);
      Label label3 = method.DefineLabel();
      argIndex.Emit(method);
      if (type1.IsValueType)
        method.Emit(OpCodes.Box, type1);
      constantPool.Add((object) this);
      CTypes.SimpleType type2 = this._type as CTypes.SimpleType;
      MarshalCleanup marshalCleanup = (MarshalCleanup) null;
      if (type2 != null && !argIndex.Type.IsValueType && (type2._type == CTypes.SimpleTypeKind.Char || type2._type == CTypes.SimpleTypeKind.WChar))
      {
        if (type2._type == CTypes.SimpleTypeKind.Char)
          CTypes.SimpleType.TryToCharPtrConversion(method, argIndex, type1, label2);
        else
          CTypes.SimpleType.TryArrayToWCharPtrConversion(method, argIndex, type1, label2);
        Label label4 = method.DefineLabel();
        LocalOrArg argIndex1 = argIndex;
        if (type1 != typeof (string))
        {
          LocalBuilder local = method.DeclareLocal(typeof (string));
          method.Emit(OpCodes.Isinst, typeof (string));
          method.Emit(OpCodes.Brfalse, label4);
          argIndex.Emit(method);
          method.Emit(OpCodes.Castclass, typeof (string));
          method.Emit(OpCodes.Stloc, local);
          method.Emit(OpCodes.Ldloc, local);
          argIndex1 = (LocalOrArg) new Local(local);
        }
        if (type2._type == CTypes.SimpleTypeKind.Char)
          marshalCleanup = CTypes.SimpleType.MarshalCharPointer(method, argIndex1);
        else
          CTypes.SimpleType.MarshalWCharPointer(method, argIndex1);
        method.Emit(OpCodes.Br, label2);
        method.MarkLabel(label4);
        argIndex.Emit(method);
      }
      method.Emit(OpCodes.Ldarg, constantPoolArgument);
      method.Emit(OpCodes.Ldc_I4, constantPool.Count - 1);
      method.Emit(OpCodes.Ldelem_Ref);
      method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("CheckNativeArgument"));
      method.Emit(OpCodes.Dup);
      method.Emit(OpCodes.Brfalse, label3);
      method.Emit(OpCodes.Call, typeof (CTypes.CData).GetMethod("get_UnsafeAddress"));
      method.Emit(OpCodes.Br, label2);
      method.MarkLabel(label3);
      Label label5 = method.DefineLabel();
      method.Emit(OpCodes.Pop);
      argIndex.Emit(method);
      if (type1.IsValueType)
        method.Emit(OpCodes.Box, type1);
      method.Emit(OpCodes.Ldarg, constantPoolArgument);
      method.Emit(OpCodes.Ldc_I4, constantPool.Count - 1);
      method.Emit(OpCodes.Ldelem_Ref);
      method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("TryCheckCDataPointerType"));
      method.Emit(OpCodes.Dup);
      method.Emit(OpCodes.Brfalse, label5);
      method.Emit(OpCodes.Call, typeof (CTypes.CData).GetMethod("get_UnsafeAddress"));
      method.Emit(OpCodes.Br, label2);
      method.MarkLabel(label5);
      method.Emit(OpCodes.Pop);
      argIndex.Emit(method);
      if (type1.IsValueType)
        method.Emit(OpCodes.Box, type1);
      method.Emit(OpCodes.Ldarg, constantPoolArgument);
      method.Emit(OpCodes.Ldc_I4, constantPool.Count - 1);
      method.Emit(OpCodes.Ldelem_Ref);
      method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("CheckCDataType"));
      method.Emit(OpCodes.Call, typeof (CTypes.CData).GetMethod("get_UnsafeAddress"));
      method.Emit(OpCodes.Ldind_I);
      method.MarkLabel(label2);
      return marshalCleanup;
    }

    Type CTypes.INativeType.GetPythonType() => typeof (object);

    void CTypes.INativeType.EmitReverseMarshalling(
      ILGenerator method,
      LocalOrArg value,
      List<object> constantPool,
      int constantPoolArgument)
    {
      value.Emit(method);
      CTypes.EmitCDataCreation((CTypes.INativeType) this, method, constantPool, constantPoolArgument);
    }

    string CTypes.INativeType.TypeFormat => "&" + (this._typeFormat ?? this._type.TypeFormat);
  }

  [PythonType("_SimpleCData")]
  public abstract class SimpleCData : CTypes.CData, ICodeFormattable
  {
    protected SimpleCData()
    {
    }

    protected SimpleCData(params object[] args)
    {
    }

    public void __init__() => this._memHolder = new MemoryHolder(this.Size);

    public void __init__(CodeContext context, object value)
    {
      if (this.IsChar && (!(value is string str) || str.Length != 1))
        throw PythonOps.TypeError("one character string expected");
      if (this.IsIntegerType)
      {
        object ret = (object) null;
        switch (value)
        {
          case float _:
          case double _:
            throw PythonOps.TypeError("int expected instead of float");
          case int _:
          case BigInteger _:
label_7:
            if (ret != null)
            {
              value = PythonOps.CallWithContext(context, ret);
              break;
            }
            break;
          default:
            if (!PythonOps.TryGetBoundAttr(value, "__int__", out ret))
              throw PythonOps.TypeError("an integer is required");
            goto label_7;
        }
      }
      if (this.IsFloatType)
      {
        object ret = (object) null;
        switch (value)
        {
          case double _:
          case float _:
          case int _:
          case BigInteger _:
label_13:
            if (value is BigInteger bigInteger && bigInteger > (BigInteger) double.MaxValue)
              throw PythonOps.OverflowError("long int too large to convert to float");
            if (ret != null)
            {
              value = PythonOps.CallWithContext(context, ret);
              break;
            }
            break;
          default:
            if (!PythonOps.TryGetBoundAttr(value, "__float__", out ret))
              throw PythonOps.TypeError("a float is required");
            goto label_13;
        }
      }
      this._memHolder = new MemoryHolder(this.Size);
      this.NativeType.SetValue(this._memHolder, 0, value);
      if (!this.IsString)
        return;
      this._memHolder.AddObject((object) "str", value);
    }

    [PropertyMethod]
    [SpecialName]
    public void Setvalue(object value)
    {
      this.NativeType.SetValue(this._memHolder, 0, value);
      if (!this.IsString)
        return;
      this._memHolder.AddObject((object) "str", value);
    }

    [PropertyMethod]
    [SpecialName]
    public object Getvalue() => this.NativeType.GetValue(this._memHolder, (object) this, 0, true);

    [PropertyMethod]
    [SpecialName]
    public void Deletevalue()
    {
      throw PythonOps.TypeError("cannot delete value property from simple cdata");
    }

    public override object _objects
    {
      get
      {
        if (this.IsString)
        {
          PythonDictionary objects = this._memHolder.Objects;
          if (objects != null)
            return objects[(object) "str"];
        }
        return (object) this._memHolder.Objects;
      }
    }

    private bool IsString
    {
      get
      {
        CTypes.SimpleTypeKind type = ((CTypes.SimpleType) this.NativeType)._type;
        return type == CTypes.SimpleTypeKind.CharPointer || type == CTypes.SimpleTypeKind.WCharPointer;
      }
    }

    private bool IsChar
    {
      get
      {
        CTypes.SimpleTypeKind type = ((CTypes.SimpleType) this.NativeType)._type;
        return type == CTypes.SimpleTypeKind.Char || type == CTypes.SimpleTypeKind.WChar;
      }
    }

    private bool IsIntegerType
    {
      get
      {
        CTypes.SimpleTypeKind type = ((CTypes.SimpleType) this.NativeType)._type;
        switch (type)
        {
          case CTypes.SimpleTypeKind.SignedShort:
          case CTypes.SimpleTypeKind.SignedInt:
          case CTypes.SimpleTypeKind.UnsignedInt:
          case CTypes.SimpleTypeKind.SignedLong:
          case CTypes.SimpleTypeKind.UnsignedLong:
          case CTypes.SimpleTypeKind.SignedLongLong:
          case CTypes.SimpleTypeKind.UnsignedLongLong:
            return true;
          default:
            return type == CTypes.SimpleTypeKind.UnsignedShort;
        }
      }
    }

    private bool IsFloatType
    {
      get
      {
        CTypes.SimpleTypeKind type = ((CTypes.SimpleType) this.NativeType)._type;
        return type == CTypes.SimpleTypeKind.Double || type == CTypes.SimpleTypeKind.Single;
      }
    }

    public string __repr__(CodeContext context)
    {
      return DynamicHelpers.GetPythonType((object) this).BaseTypes[0] == CTypes._SimpleCData ? $"{DynamicHelpers.GetPythonType((object) this).Name}({this.GetDataRepr(context)})" : ObjectOps.__repr__((object) this);
    }

    private string GetDataRepr(CodeContext context)
    {
      return PythonOps.Repr(context, this.NativeType.GetValue(this._memHolder, (object) this, 0, false));
    }
  }

  [PythonType]
  [PythonHidden(new PlatformID[] {})]
  public class SimpleType : PythonType, CTypes.INativeType
  {
    internal readonly CTypes.SimpleTypeKind _type;
    private readonly char _charType;
    private readonly string _format;
    private readonly bool _swap;

    public SimpleType(CodeContext context, string name, PythonTuple bases, PythonDictionary dict)
      : base(context, name, bases, dict)
    {
      object o;
      string str;
      if (!this.TryGetBoundCustomMember(context, "_type_", out o) || (str = StringOps.AsString(o)) == null || str.Length != 1 || "?cbBghHiIlLdfuzZqQPXOv".IndexOf(str[0]) == -1)
        throw PythonOps.AttributeError("AttributeError: class must define a '_type_' attribute which must be a single character string containing one of '{0}'.", (object) "?cbBghHiIlLdfuzZqQPXOv");
      this._charType = str[0];
      switch (this._charType)
      {
        case '?':
          this._type = CTypes.SimpleTypeKind.Boolean;
          break;
        case 'B':
          this._type = CTypes.SimpleTypeKind.UnsignedByte;
          break;
        case 'H':
          this._type = CTypes.SimpleTypeKind.UnsignedShort;
          break;
        case 'I':
          this._type = CTypes.SimpleTypeKind.UnsignedInt;
          break;
        case 'L':
          this._type = CTypes.SimpleTypeKind.UnsignedLong;
          break;
        case 'O':
          this._type = CTypes.SimpleTypeKind.Object;
          break;
        case 'P':
          this._type = CTypes.SimpleTypeKind.Pointer;
          break;
        case 'Q':
          this._type = CTypes.SimpleTypeKind.UnsignedLongLong;
          break;
        case 'X':
          this._type = CTypes.SimpleTypeKind.BStr;
          break;
        case 'Z':
          this._type = CTypes.SimpleTypeKind.WCharPointer;
          break;
        case 'b':
          this._type = CTypes.SimpleTypeKind.SignedByte;
          break;
        case 'c':
          this._type = CTypes.SimpleTypeKind.Char;
          break;
        case 'd':
        case 'g':
          this._type = CTypes.SimpleTypeKind.Double;
          break;
        case 'f':
          this._type = CTypes.SimpleTypeKind.Single;
          break;
        case 'h':
          this._type = CTypes.SimpleTypeKind.SignedShort;
          break;
        case 'i':
          this._type = CTypes.SimpleTypeKind.SignedInt;
          break;
        case 'l':
          this._type = CTypes.SimpleTypeKind.SignedLong;
          break;
        case 'q':
          this._type = CTypes.SimpleTypeKind.SignedLongLong;
          break;
        case 'u':
          this._type = CTypes.SimpleTypeKind.WChar;
          break;
        case 'v':
          this._type = CTypes.SimpleTypeKind.VariantBool;
          break;
        case 'z':
          this._type = CTypes.SimpleTypeKind.CharPointer;
          break;
        default:
          throw new NotImplementedException("simple type " + str);
      }
      if (!name.EndsWith("_be") && !name.EndsWith("_le") && "fdhHiIlLqQ".IndexOf(this._charType) != -1)
        this.CreateSwappedType(context, name, bases, dict);
      this._format = (BitConverter.IsLittleEndian ? '<' : '>').ToString() + this._charType.ToString();
    }

    private SimpleType(Type underlyingSystemType)
      : base(underlyingSystemType)
    {
    }

    private SimpleType(
      CodeContext context,
      string name,
      PythonTuple bases,
      PythonDictionary dict,
      bool isLittleEndian)
      : this(context, name, bases, dict)
    {
      this._format = (isLittleEndian ? '<' : '>').ToString() + this._charType.ToString();
      this._swap = isLittleEndian != BitConverter.IsLittleEndian;
    }

    private void CreateSwappedType(
      CodeContext context,
      string name,
      PythonTuple bases,
      PythonDictionary dict)
    {
      CTypes.SimpleType simpleType = new CTypes.SimpleType(context, name + (BitConverter.IsLittleEndian ? "_be" : "_le"), bases, dict, !BitConverter.IsLittleEndian);
      if (BitConverter.IsLittleEndian)
      {
        this.AddSlot("__ctype_be__", (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) simpleType));
        this.AddSlot("__ctype_le__", (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) this));
        simpleType.AddSlot("__ctype_le__", (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) this));
        simpleType.AddSlot("__ctype_be__", (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) simpleType));
      }
      else
      {
        this.AddSlot("__ctype_le__", (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) simpleType));
        this.AddSlot("__ctype_be__", (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) this));
        simpleType.AddSlot("__ctype_le__", (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) simpleType));
        simpleType.AddSlot("__ctype_be__", (PythonTypeSlot) new PythonTypeUserDescriptorSlot((object) this));
      }
    }

    public static CTypes.ArrayType operator *(CTypes.SimpleType type, int count)
    {
      return CTypes.MakeArrayType((PythonType) type, count);
    }

    public static CTypes.ArrayType operator *(int count, CTypes.SimpleType type)
    {
      return CTypes.MakeArrayType((PythonType) type, count);
    }

    internal static PythonType MakeSystemType(Type underlyingSystemType)
    {
      return PythonType.SetPythonType(underlyingSystemType, (PythonType) new CTypes.SimpleType(underlyingSystemType));
    }

    public CTypes.SimpleCData from_address(CodeContext context, int address)
    {
      return this.from_address(context, new IntPtr(address));
    }

    public CTypes.SimpleCData from_address(CodeContext context, BigInteger address)
    {
      return this.from_address(context, new IntPtr((long) address));
    }

    public CTypes.SimpleCData from_address(CodeContext context, IntPtr ptr)
    {
      CTypes.SimpleCData instance = (CTypes.SimpleCData) this.CreateInstance(context);
      instance.SetAddress(ptr);
      return instance;
    }

    public CTypes.SimpleCData from_buffer(ArrayModule.array array, int offset = 0)
    {
      CTypes.ValidateArraySizes(array, offset, ((CTypes.INativeType) this).Size);
      CTypes.SimpleCData instance = (CTypes.SimpleCData) this.CreateInstance(this.Context.SharedContext);
      instance._memHolder = new MemoryHolder(array.GetArrayAddress().Add(offset), ((CTypes.INativeType) this).Size);
      instance._memHolder.AddObject((object) "ffffffff", (object) array);
      return instance;
    }

    public CTypes.SimpleCData from_buffer_copy(ArrayModule.array array, int offset = 0)
    {
      CTypes.ValidateArraySizes(array, offset, ((CTypes.INativeType) this).Size);
      CTypes.SimpleCData instance = (CTypes.SimpleCData) this.CreateInstance(this.Context.SharedContext);
      instance._memHolder = new MemoryHolder(((CTypes.INativeType) this).Size);
      instance._memHolder.CopyFrom(array.GetArrayAddress().Add(offset), new IntPtr(((CTypes.INativeType) this).Size));
      GC.KeepAlive((object) array);
      return instance;
    }

    public object from_param(object obj)
    {
      return (object) new CTypes.NativeArgument((CTypes.CData) PythonCalls.Call((object) this, obj), this._charType.ToString());
    }

    public CTypes.SimpleCData in_dll(CodeContext context, object library, string name)
    {
      IntPtr address = NativeFunctions.LoadFunction(CTypes.GetHandleFromObject(library, "in_dll expected object with _handle attribute"), name);
      if (address == IntPtr.Zero)
        throw PythonOps.ValueError("{0} not found when attempting to load {1} from dll", (object) name, (object) this.Name);
      CTypes.SimpleCData instance = (CTypes.SimpleCData) this.CreateInstance(context);
      instance.SetAddress(address);
      return instance;
    }

    int CTypes.INativeType.Size
    {
      get
      {
        switch (this._type)
        {
          case CTypes.SimpleTypeKind.Char:
          case CTypes.SimpleTypeKind.SignedByte:
          case CTypes.SimpleTypeKind.UnsignedByte:
          case CTypes.SimpleTypeKind.Boolean:
            return 1;
          case CTypes.SimpleTypeKind.SignedShort:
          case CTypes.SimpleTypeKind.UnsignedShort:
          case CTypes.SimpleTypeKind.WChar:
          case CTypes.SimpleTypeKind.VariantBool:
            return 2;
          case CTypes.SimpleTypeKind.SignedInt:
          case CTypes.SimpleTypeKind.UnsignedInt:
          case CTypes.SimpleTypeKind.SignedLong:
          case CTypes.SimpleTypeKind.UnsignedLong:
          case CTypes.SimpleTypeKind.Single:
            return 4;
          case CTypes.SimpleTypeKind.Double:
          case CTypes.SimpleTypeKind.SignedLongLong:
          case CTypes.SimpleTypeKind.UnsignedLongLong:
            return 8;
          case CTypes.SimpleTypeKind.Object:
          case CTypes.SimpleTypeKind.Pointer:
          case CTypes.SimpleTypeKind.CharPointer:
          case CTypes.SimpleTypeKind.WCharPointer:
          case CTypes.SimpleTypeKind.BStr:
            return IntPtr.Size;
          default:
            throw new InvalidOperationException(this._type.ToString());
        }
      }
    }

    int CTypes.INativeType.Alignment => ((CTypes.INativeType) this).Size;

    object CTypes.INativeType.GetValue(
      MemoryHolder owner,
      object readingFrom,
      int offset,
      bool raw)
    {
      object obj;
      switch (this._type)
      {
        case CTypes.SimpleTypeKind.Char:
          obj = (object) new string((char) owner.ReadByte(offset), 1);
          break;
        case CTypes.SimpleTypeKind.SignedByte:
          obj = CTypes.SimpleType.GetIntReturn((int) (sbyte) owner.ReadByte(offset));
          break;
        case CTypes.SimpleTypeKind.UnsignedByte:
          obj = CTypes.SimpleType.GetIntReturn((int) owner.ReadByte(offset));
          break;
        case CTypes.SimpleTypeKind.SignedShort:
          obj = CTypes.SimpleType.GetIntReturn((int) owner.ReadInt16(offset, this._swap));
          break;
        case CTypes.SimpleTypeKind.UnsignedShort:
          obj = CTypes.SimpleType.GetIntReturn((int) (ushort) owner.ReadInt16(offset, this._swap));
          break;
        case CTypes.SimpleTypeKind.SignedInt:
          obj = CTypes.SimpleType.GetIntReturn(owner.ReadInt32(offset, this._swap));
          break;
        case CTypes.SimpleTypeKind.UnsignedInt:
          obj = CTypes.SimpleType.GetIntReturn((uint) owner.ReadInt32(offset, this._swap));
          break;
        case CTypes.SimpleTypeKind.SignedLong:
          obj = CTypes.SimpleType.GetIntReturn(owner.ReadInt32(offset, this._swap));
          break;
        case CTypes.SimpleTypeKind.UnsignedLong:
          obj = CTypes.SimpleType.GetIntReturn((uint) owner.ReadInt32(offset, this._swap));
          break;
        case CTypes.SimpleTypeKind.Single:
          obj = this.GetSingleReturn(owner.ReadInt32(offset, this._swap));
          break;
        case CTypes.SimpleTypeKind.Double:
          obj = this.GetDoubleReturn(owner.ReadInt64(offset, this._swap));
          break;
        case CTypes.SimpleTypeKind.SignedLongLong:
          obj = CTypes.SimpleType.GetIntReturn(owner.ReadInt64(offset, this._swap));
          break;
        case CTypes.SimpleTypeKind.UnsignedLongLong:
          obj = CTypes.SimpleType.GetIntReturn((ulong) owner.ReadInt64(offset, this._swap));
          break;
        case CTypes.SimpleTypeKind.Object:
          obj = this.GetObjectReturn(owner.ReadIntPtr(offset));
          break;
        case CTypes.SimpleTypeKind.Pointer:
          obj = owner.ReadIntPtr(offset).ToPython();
          break;
        case CTypes.SimpleTypeKind.CharPointer:
          obj = (object) owner.ReadMemoryHolder(offset).ReadAnsiString(0);
          break;
        case CTypes.SimpleTypeKind.WCharPointer:
          obj = (object) owner.ReadMemoryHolder(offset).ReadUnicodeString(0);
          break;
        case CTypes.SimpleTypeKind.WChar:
          obj = (object) new string((char) owner.ReadInt16(offset), 1);
          break;
        case CTypes.SimpleTypeKind.Boolean:
          obj = owner.ReadByte(offset) != (byte) 0 ? ScriptingRuntimeHelpers.True : ScriptingRuntimeHelpers.False;
          break;
        case CTypes.SimpleTypeKind.VariantBool:
          obj = owner.ReadInt16(offset, this._swap) != (short) 0 ? ScriptingRuntimeHelpers.True : ScriptingRuntimeHelpers.False;
          break;
        case CTypes.SimpleTypeKind.BStr:
          obj = (object) Marshal.PtrToStringBSTR(owner.ReadIntPtr(offset));
          break;
        default:
          throw new InvalidOperationException();
      }
      if (!raw && this.IsSubClass)
        obj = PythonCalls.Call((object) this, obj);
      return obj;
    }

    internal char ReadChar(MemoryHolder owner, int offset)
    {
      switch (this._type)
      {
        case CTypes.SimpleTypeKind.Char:
          return (char) owner.ReadByte(offset);
        case CTypes.SimpleTypeKind.WChar:
          return (char) owner.ReadInt16(offset);
        default:
          throw new InvalidOperationException();
      }
    }

    object CTypes.INativeType.SetValue(MemoryHolder owner, int offset, object value)
    {
      if (value is CTypes.SimpleCData simpleCdata && simpleCdata.NativeType == this)
      {
        simpleCdata._memHolder.CopyTo(owner, offset, ((CTypes.INativeType) this).Size);
        return (object) null;
      }
      switch (this._type)
      {
        case CTypes.SimpleTypeKind.Char:
          owner.WriteByte(offset, ModuleOps.GetChar(value, (object) this));
          break;
        case CTypes.SimpleTypeKind.SignedByte:
          owner.WriteByte(offset, ModuleOps.GetSignedByte(value, (object) this));
          break;
        case CTypes.SimpleTypeKind.UnsignedByte:
          owner.WriteByte(offset, ModuleOps.GetUnsignedByte(value, (object) this));
          break;
        case CTypes.SimpleTypeKind.SignedShort:
          owner.WriteInt16(offset, ModuleOps.GetSignedShort(value, (object) this), this._swap);
          break;
        case CTypes.SimpleTypeKind.UnsignedShort:
          owner.WriteInt16(offset, ModuleOps.GetUnsignedShort(value, (object) this), this._swap);
          break;
        case CTypes.SimpleTypeKind.SignedInt:
          owner.WriteInt32(offset, ModuleOps.GetSignedInt(value, (object) this), this._swap);
          break;
        case CTypes.SimpleTypeKind.UnsignedInt:
          owner.WriteInt32(offset, ModuleOps.GetUnsignedInt(value, (object) this), this._swap);
          break;
        case CTypes.SimpleTypeKind.SignedLong:
          owner.WriteInt32(offset, ModuleOps.GetSignedLong(value, (object) this), this._swap);
          break;
        case CTypes.SimpleTypeKind.UnsignedLong:
          owner.WriteInt32(offset, ModuleOps.GetUnsignedLong(value, (object) this), this._swap);
          break;
        case CTypes.SimpleTypeKind.Single:
          owner.WriteInt32(offset, ModuleOps.GetSingleBits(value), this._swap);
          break;
        case CTypes.SimpleTypeKind.Double:
          owner.WriteInt64(offset, ModuleOps.GetDoubleBits(value), this._swap);
          break;
        case CTypes.SimpleTypeKind.SignedLongLong:
          owner.WriteInt64(offset, ModuleOps.GetSignedLongLong(value, (object) this), this._swap);
          break;
        case CTypes.SimpleTypeKind.UnsignedLongLong:
          owner.WriteInt64(offset, ModuleOps.GetUnsignedLongLong(value, (object) this), this._swap);
          break;
        case CTypes.SimpleTypeKind.Object:
          owner.WriteIntPtr(offset, ModuleOps.GetObject(value));
          break;
        case CTypes.SimpleTypeKind.Pointer:
          owner.WriteIntPtr(offset, ModuleOps.GetPointer(value));
          break;
        case CTypes.SimpleTypeKind.CharPointer:
          owner.WriteIntPtr(offset, ModuleOps.GetCharPointer(value));
          return value;
        case CTypes.SimpleTypeKind.WCharPointer:
          owner.WriteIntPtr(offset, ModuleOps.GetWCharPointer(value));
          return value;
        case CTypes.SimpleTypeKind.WChar:
          owner.WriteInt16(offset, (short) ModuleOps.GetWChar(value, (object) this));
          break;
        case CTypes.SimpleTypeKind.Boolean:
          owner.WriteByte(offset, ModuleOps.GetBoolean(value, (object) this));
          break;
        case CTypes.SimpleTypeKind.VariantBool:
          owner.WriteInt16(offset, (short) ModuleOps.GetVariantBool(value, (object) this), this._swap);
          break;
        case CTypes.SimpleTypeKind.BStr:
          owner.WriteIntPtr(offset, ModuleOps.GetBSTR(value));
          return value;
        default:
          throw new InvalidOperationException();
      }
      return (object) null;
    }

    Type CTypes.INativeType.GetNativeType()
    {
      switch (this._type)
      {
        case CTypes.SimpleTypeKind.Char:
          return typeof (byte);
        case CTypes.SimpleTypeKind.SignedByte:
          return typeof (sbyte);
        case CTypes.SimpleTypeKind.UnsignedByte:
          return typeof (byte);
        case CTypes.SimpleTypeKind.SignedShort:
        case CTypes.SimpleTypeKind.VariantBool:
          return typeof (short);
        case CTypes.SimpleTypeKind.UnsignedShort:
          return typeof (ushort);
        case CTypes.SimpleTypeKind.SignedInt:
        case CTypes.SimpleTypeKind.SignedLong:
          return typeof (int);
        case CTypes.SimpleTypeKind.UnsignedInt:
        case CTypes.SimpleTypeKind.UnsignedLong:
          return typeof (uint);
        case CTypes.SimpleTypeKind.Single:
          return typeof (float);
        case CTypes.SimpleTypeKind.Double:
          return typeof (double);
        case CTypes.SimpleTypeKind.SignedLongLong:
          return typeof (long);
        case CTypes.SimpleTypeKind.UnsignedLongLong:
          return typeof (ulong);
        case CTypes.SimpleTypeKind.Object:
          return typeof (IntPtr);
        case CTypes.SimpleTypeKind.Pointer:
        case CTypes.SimpleTypeKind.CharPointer:
        case CTypes.SimpleTypeKind.WCharPointer:
        case CTypes.SimpleTypeKind.BStr:
          return typeof (IntPtr);
        case CTypes.SimpleTypeKind.WChar:
          return typeof (char);
        case CTypes.SimpleTypeKind.Boolean:
          return typeof (bool);
        default:
          throw new InvalidOperationException();
      }
    }

    MarshalCleanup CTypes.INativeType.EmitMarshalling(
      ILGenerator method,
      LocalOrArg argIndex,
      List<object> constantPool,
      int constantPoolArgument)
    {
      MarshalCleanup marshalCleanup = (MarshalCleanup) null;
      Label label1 = method.DefineLabel();
      Type type = argIndex.Type;
      if (!type.IsValueType && this._type != CTypes.SimpleTypeKind.Object && this._type != CTypes.SimpleTypeKind.Pointer)
      {
        Label label2 = method.DefineLabel();
        argIndex.Emit(method);
        constantPool.Add((object) this);
        method.Emit(OpCodes.Ldarg, constantPoolArgument);
        method.Emit(OpCodes.Ldc_I4, constantPool.Count - 1);
        method.Emit(OpCodes.Ldelem_Ref);
        method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("CheckSimpleCDataType"));
        method.Emit(OpCodes.Brfalse, label2);
        argIndex.Emit(method);
        method.Emit(OpCodes.Castclass, typeof (CTypes.CData));
        method.Emit(OpCodes.Call, typeof (CTypes.CData).GetMethod("get_UnsafeAddress"));
        method.Emit(OpCodes.Ldobj, ((CTypes.INativeType) this).GetNativeType());
        method.Emit(OpCodes.Br, label1);
        method.MarkLabel(label2);
      }
      argIndex.Emit(method);
      if (type.IsValueType)
        method.Emit(OpCodes.Box, type);
      switch (this._type)
      {
        case CTypes.SimpleTypeKind.Char:
        case CTypes.SimpleTypeKind.SignedByte:
        case CTypes.SimpleTypeKind.UnsignedByte:
        case CTypes.SimpleTypeKind.SignedShort:
        case CTypes.SimpleTypeKind.UnsignedShort:
        case CTypes.SimpleTypeKind.SignedInt:
        case CTypes.SimpleTypeKind.UnsignedInt:
        case CTypes.SimpleTypeKind.SignedLong:
        case CTypes.SimpleTypeKind.UnsignedLong:
        case CTypes.SimpleTypeKind.Single:
        case CTypes.SimpleTypeKind.Double:
        case CTypes.SimpleTypeKind.SignedLongLong:
        case CTypes.SimpleTypeKind.UnsignedLongLong:
        case CTypes.SimpleTypeKind.WChar:
        case CTypes.SimpleTypeKind.Boolean:
        case CTypes.SimpleTypeKind.VariantBool:
          constantPool.Add((object) this);
          method.Emit(OpCodes.Ldarg, constantPoolArgument);
          method.Emit(OpCodes.Ldc_I4, constantPool.Count - 1);
          method.Emit(OpCodes.Ldelem_Ref);
          method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("Get" + (object) this._type));
          break;
        case CTypes.SimpleTypeKind.Object:
          method.Emit(OpCodes.Call, typeof (CTypes).GetMethod("PyObj_ToPtr"));
          break;
        case CTypes.SimpleTypeKind.Pointer:
          Label label3 = method.DefineLabel();
          CTypes.SimpleType.TryBytesConversion(method, label3);
          Label label4 = method.DefineLabel();
          argIndex.Emit(method);
          if (type.IsValueType)
            method.Emit(OpCodes.Box, type);
          method.Emit(OpCodes.Isinst, typeof (string));
          method.Emit(OpCodes.Dup);
          method.Emit(OpCodes.Brfalse, label4);
          LocalBuilder local = method.DeclareLocal(typeof (string), true);
          method.Emit(OpCodes.Stloc, local);
          method.Emit(OpCodes.Ldloc, local);
          method.Emit(OpCodes.Conv_I);
          method.Emit(OpCodes.Ldc_I4, RuntimeHelpers.OffsetToStringData);
          method.Emit(OpCodes.Add);
          method.Emit(OpCodes.Br, label3);
          method.MarkLabel(label4);
          method.Emit(OpCodes.Pop);
          argIndex.Emit(method);
          if (type.IsValueType)
            method.Emit(OpCodes.Box, type);
          method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("GetPointer"));
          method.MarkLabel(label3);
          break;
        case CTypes.SimpleTypeKind.CharPointer:
          Label label5 = method.DefineLabel();
          CTypes.SimpleType.TryToCharPtrConversion(method, argIndex, type, label5);
          marshalCleanup = CTypes.SimpleType.MarshalCharPointer(method, argIndex);
          method.MarkLabel(label5);
          break;
        case CTypes.SimpleTypeKind.WCharPointer:
          Label label6 = method.DefineLabel();
          CTypes.SimpleType.TryArrayToWCharPtrConversion(method, argIndex, type, label6);
          CTypes.SimpleType.MarshalWCharPointer(method, argIndex);
          method.MarkLabel(label6);
          break;
        case CTypes.SimpleTypeKind.BStr:
          throw new NotImplementedException("BSTR marshalling");
      }
      method.MarkLabel(label1);
      return marshalCleanup;
    }

    private static void TryBytesConversion(ILGenerator method, Label done)
    {
      Label label = method.DefineLabel();
      LocalBuilder local = method.DeclareLocal(typeof (byte).MakeByRefType(), true);
      method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("TryCheckBytes"));
      method.Emit(OpCodes.Dup);
      method.Emit(OpCodes.Brfalse, label);
      method.Emit(OpCodes.Ldc_I4_0);
      method.Emit(OpCodes.Ldelema, typeof (byte));
      method.Emit(OpCodes.Stloc, local);
      method.Emit(OpCodes.Ldloc, local);
      method.Emit(OpCodes.Br, done);
      method.MarkLabel(label);
      method.Emit(OpCodes.Pop);
    }

    internal static void TryArrayToWCharPtrConversion(
      ILGenerator method,
      LocalOrArg argIndex,
      Type argumentType,
      Label done)
    {
      Label label = method.DefineLabel();
      method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("TryCheckWCharArray"));
      method.Emit(OpCodes.Dup);
      method.Emit(OpCodes.Brfalse, label);
      method.Emit(OpCodes.Call, typeof (CTypes.CData).GetMethod("get_UnsafeAddress"));
      method.Emit(OpCodes.Br, done);
      method.MarkLabel(label);
      method.Emit(OpCodes.Pop);
      argIndex.Emit(method);
      if (!argumentType.IsValueType)
        return;
      method.Emit(OpCodes.Box, argumentType);
    }

    internal static void TryToCharPtrConversion(
      ILGenerator method,
      LocalOrArg argIndex,
      Type argumentType,
      Label done)
    {
      CTypes.SimpleType.TryBytesConversion(method, done);
      Label label = method.DefineLabel();
      argIndex.Emit(method);
      method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("TryCheckCharArray"));
      method.Emit(OpCodes.Dup);
      method.Emit(OpCodes.Brfalse, label);
      method.Emit(OpCodes.Call, typeof (CTypes.CData).GetMethod("get_UnsafeAddress"));
      method.Emit(OpCodes.Br, done);
      method.MarkLabel(label);
      method.Emit(OpCodes.Pop);
      argIndex.Emit(method);
      if (!argumentType.IsValueType)
        return;
      method.Emit(OpCodes.Box, argumentType);
    }

    internal static void MarshalWCharPointer(ILGenerator method, LocalOrArg argIndex)
    {
      Type type = argIndex.Type;
      Label label1 = method.DefineLabel();
      Label label2 = method.DefineLabel();
      method.Emit(OpCodes.Brfalse, label1);
      argIndex.Emit(method);
      if (type.IsValueType)
        method.Emit(OpCodes.Box, type);
      LocalBuilder local = method.DeclareLocal(typeof (string), true);
      method.Emit(OpCodes.Stloc, local);
      method.Emit(OpCodes.Ldloc, local);
      method.Emit(OpCodes.Conv_I);
      method.Emit(OpCodes.Ldc_I4, RuntimeHelpers.OffsetToStringData);
      method.Emit(OpCodes.Add);
      method.Emit(OpCodes.Br, label2);
      method.MarkLabel(label1);
      method.Emit(OpCodes.Ldc_I4_0);
      method.Emit(OpCodes.Conv_I);
      method.MarkLabel(label2);
    }

    internal static MarshalCleanup MarshalCharPointer(ILGenerator method, LocalOrArg argIndex)
    {
      Type type = argIndex.Type;
      Label label1 = method.DefineLabel();
      Label label2 = method.DefineLabel();
      method.Emit(OpCodes.Brfalse, label1);
      argIndex.Emit(method);
      if (type.IsValueType)
        method.Emit(OpCodes.Box, type);
      LocalBuilder local = method.DeclareLocal(typeof (IntPtr));
      method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("StringToHGlobalAnsi"));
      method.Emit(OpCodes.Stloc, local);
      method.Emit(OpCodes.Ldloc, local);
      method.Emit(OpCodes.Br, label2);
      method.MarkLabel(label1);
      method.Emit(OpCodes.Ldc_I4_0);
      method.Emit(OpCodes.Conv_I);
      method.MarkLabel(label2);
      return (MarshalCleanup) new StringCleanup(local);
    }

    Type CTypes.INativeType.GetPythonType()
    {
      return this.IsSubClass ? typeof (object) : this.GetPythonTypeWorker();
    }

    private Type GetPythonTypeWorker()
    {
      switch (this._type)
      {
        case CTypes.SimpleTypeKind.Char:
        case CTypes.SimpleTypeKind.CharPointer:
        case CTypes.SimpleTypeKind.WCharPointer:
        case CTypes.SimpleTypeKind.WChar:
        case CTypes.SimpleTypeKind.BStr:
          return typeof (string);
        case CTypes.SimpleTypeKind.SignedByte:
        case CTypes.SimpleTypeKind.UnsignedByte:
        case CTypes.SimpleTypeKind.SignedShort:
        case CTypes.SimpleTypeKind.UnsignedShort:
        case CTypes.SimpleTypeKind.SignedInt:
        case CTypes.SimpleTypeKind.SignedLong:
        case CTypes.SimpleTypeKind.VariantBool:
          return typeof (int);
        case CTypes.SimpleTypeKind.UnsignedInt:
        case CTypes.SimpleTypeKind.UnsignedLong:
        case CTypes.SimpleTypeKind.SignedLongLong:
        case CTypes.SimpleTypeKind.UnsignedLongLong:
        case CTypes.SimpleTypeKind.Object:
        case CTypes.SimpleTypeKind.Pointer:
          return typeof (object);
        case CTypes.SimpleTypeKind.Single:
        case CTypes.SimpleTypeKind.Double:
          return typeof (double);
        case CTypes.SimpleTypeKind.Boolean:
          return typeof (bool);
        default:
          throw new InvalidOperationException();
      }
    }

    void CTypes.INativeType.EmitReverseMarshalling(
      ILGenerator method,
      LocalOrArg value,
      List<object> constantPool,
      int constantPoolArgument)
    {
      value.Emit(method);
      switch (this._type)
      {
        case CTypes.SimpleTypeKind.Char:
          method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("CharToString"));
          break;
        case CTypes.SimpleTypeKind.SignedByte:
        case CTypes.SimpleTypeKind.UnsignedByte:
        case CTypes.SimpleTypeKind.SignedShort:
        case CTypes.SimpleTypeKind.UnsignedShort:
        case CTypes.SimpleTypeKind.VariantBool:
          method.Emit(OpCodes.Conv_I4);
          break;
        case CTypes.SimpleTypeKind.UnsignedInt:
        case CTypes.SimpleTypeKind.UnsignedLong:
          CTypes.SimpleType.EmitInt32ToObject(method, value);
          break;
        case CTypes.SimpleTypeKind.Single:
          method.Emit(OpCodes.Conv_R8);
          break;
        case CTypes.SimpleTypeKind.SignedLongLong:
        case CTypes.SimpleTypeKind.UnsignedLongLong:
          CTypes.SimpleType.EmitInt64ToObject(method, value);
          break;
        case CTypes.SimpleTypeKind.Object:
          method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("IntPtrToObject"));
          break;
        case CTypes.SimpleTypeKind.Pointer:
          Label label1 = method.DefineLabel();
          Label label2 = method.DefineLabel();
          if (IntPtr.Size == 4)
          {
            LocalBuilder local = method.DeclareLocal(typeof (uint));
            method.Emit(OpCodes.Conv_U4);
            method.Emit(OpCodes.Stloc, local);
            method.Emit(OpCodes.Ldloc, local);
            method.Emit(OpCodes.Ldc_I4_0);
            method.Emit(OpCodes.Conv_U4);
            method.Emit(OpCodes.Bne_Un, label2);
            method.Emit(OpCodes.Ldnull);
            method.Emit(OpCodes.Br, label1);
            method.MarkLabel(label2);
            method.Emit(OpCodes.Ldloc, local);
            CTypes.SimpleType.EmitInt32ToObject(method, (LocalOrArg) new Local(local));
          }
          else
          {
            LocalBuilder local = method.DeclareLocal(typeof (long));
            method.Emit(OpCodes.Conv_I8);
            method.Emit(OpCodes.Stloc, local);
            method.Emit(OpCodes.Ldloc, local);
            method.Emit(OpCodes.Ldc_I4_0);
            method.Emit(OpCodes.Conv_U8);
            method.Emit(OpCodes.Bne_Un, label2);
            method.Emit(OpCodes.Ldnull);
            method.Emit(OpCodes.Br, label1);
            method.MarkLabel(label2);
            method.Emit(OpCodes.Ldloc, local);
            CTypes.SimpleType.EmitInt64ToObject(method, (LocalOrArg) new Local(local));
          }
          method.MarkLabel(label1);
          break;
        case CTypes.SimpleTypeKind.CharPointer:
          method.Emit(OpCodes.Call, typeof (Marshal).GetMethod("PtrToStringAnsi", new Type[1]
          {
            typeof (IntPtr)
          }));
          break;
        case CTypes.SimpleTypeKind.WCharPointer:
          method.Emit(OpCodes.Call, typeof (Marshal).GetMethod("PtrToStringUni", new Type[1]
          {
            typeof (IntPtr)
          }));
          break;
        case CTypes.SimpleTypeKind.WChar:
          method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("WCharToString"));
          break;
        case CTypes.SimpleTypeKind.BStr:
          method.Emit(OpCodes.Call, typeof (Marshal).GetMethod("PtrToStringBSTR", new Type[1]
          {
            typeof (IntPtr)
          }));
          break;
      }
      if (!this.IsSubClass)
        return;
      LocalBuilder local1 = method.DeclareLocal(typeof (object));
      if (this.GetPythonTypeWorker().IsValueType)
        method.Emit(OpCodes.Box, this.GetPythonTypeWorker());
      method.Emit(OpCodes.Stloc, local1);
      constantPool.Add((object) this);
      method.Emit(OpCodes.Ldarg, constantPoolArgument);
      method.Emit(OpCodes.Ldc_I4, constantPool.Count - 1);
      method.Emit(OpCodes.Ldelem_Ref);
      method.Emit(OpCodes.Ldloc, local1);
      method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("CreateSubclassInstance"));
    }

    private static void EmitInt64ToObject(ILGenerator method, LocalOrArg value)
    {
      Label label1 = method.DefineLabel();
      Label label2 = method.DefineLabel();
      method.Emit(OpCodes.Ldc_I4, int.MaxValue);
      method.Emit(OpCodes.Conv_I8);
      method.Emit(OpCodes.Bgt, label1);
      value.Emit(method);
      method.Emit(OpCodes.Ldc_I4, int.MinValue);
      method.Emit(OpCodes.Conv_I8);
      method.Emit(OpCodes.Blt, label1);
      value.Emit(method);
      method.Emit(OpCodes.Conv_I4);
      method.Emit(OpCodes.Box, typeof (int));
      method.Emit(OpCodes.Br, label2);
      method.MarkLabel(label1);
      value.Emit(method);
      method.Emit(OpCodes.Call, typeof (BigInteger).GetMethod("op_Implicit", new Type[1]
      {
        value.Type
      }));
      method.Emit(OpCodes.Box, typeof (BigInteger));
      method.MarkLabel(label2);
    }

    private static void EmitInt32ToObject(ILGenerator method, LocalOrArg value)
    {
      Label label1 = method.DefineLabel();
      Label label2 = method.DefineLabel();
      method.Emit(OpCodes.Ldc_I4, int.MaxValue);
      method.Emit(value.Type == typeof (uint) ? OpCodes.Conv_U4 : OpCodes.Conv_U8);
      method.Emit(OpCodes.Ble, label1);
      value.Emit(method);
      method.Emit(OpCodes.Call, typeof (BigInteger).GetMethod("op_Implicit", new Type[1]
      {
        value.Type
      }));
      method.Emit(OpCodes.Box, typeof (BigInteger));
      method.Emit(OpCodes.Br, label2);
      method.MarkLabel(label1);
      value.Emit(method);
      method.Emit(OpCodes.Conv_I4);
      method.Emit(OpCodes.Box, typeof (int));
      method.MarkLabel(label2);
    }

    private bool IsSubClass
    {
      get => this.BaseTypes.Count != 1 || this.BaseTypes[0] != CTypes._SimpleCData;
    }

    private object GetObjectReturn(IntPtr intPtr) => GCHandle.FromIntPtr(intPtr).Target;

    private object GetDoubleReturn(long p)
    {
      return (object) BitConverter.ToDouble(BitConverter.GetBytes(p), 0);
    }

    private object GetSingleReturn(int p)
    {
      return (object) BitConverter.ToSingle(BitConverter.GetBytes(p), 0);
    }

    private static object GetIntReturn(int value) => ScriptingRuntimeHelpers.Int32ToObject(value);

    private static object GetIntReturn(uint value)
    {
      return value > (uint) int.MaxValue ? (object) (BigInteger) value : ScriptingRuntimeHelpers.Int32ToObject((int) value);
    }

    private static object GetIntReturn(long value)
    {
      return value <= (long) int.MaxValue && value >= (long) int.MinValue ? (object) (int) value : (object) (BigInteger) value;
    }

    private static object GetIntReturn(ulong value)
    {
      return value <= (ulong) int.MaxValue ? (object) (int) value : (object) (BigInteger) value;
    }

    string CTypes.INativeType.TypeFormat => this._format;
  }

  internal enum SimpleTypeKind
  {
    Char,
    SignedByte,
    UnsignedByte,
    SignedShort,
    UnsignedShort,
    SignedInt,
    UnsignedInt,
    SignedLong,
    UnsignedLong,
    Single,
    Double,
    SignedLongLong,
    UnsignedLongLong,
    Object,
    Pointer,
    CharPointer,
    WCharPointer,
    WChar,
    Boolean,
    VariantBool,
    BStr,
  }

  [PythonType]
  [PythonHidden(new PlatformID[] {})]
  public class StructType : PythonType, CTypes.INativeType
  {
    internal CTypes.Field[] _fields;
    private int? _size;
    private int? _alignment;
    private int? _pack;
    private static readonly CTypes.Field[] _emptyFields = new CTypes.Field[0];

    public StructType(
      CodeContext context,
      string name,
      PythonTuple bases,
      PythonDictionary members)
      : base(context, name, bases, members)
    {
      foreach (PythonType pythonType in (IEnumerable<PythonType>) this.ResolutionOrder)
      {
        CTypes.StructType structType = pythonType as CTypes.StructType;
        if (structType != this && structType != null)
          structType.EnsureFinal();
        if (pythonType is CTypes.UnionType unionType)
          unionType.EnsureFinal();
      }
      object obj1;
      if (members.TryGetValue((object) "_pack_", out obj1))
        this._pack = obj1 is int num && num >= 0 ? new int?((int) obj1) : throw PythonOps.ValueError("pack must be a non-negative integer");
      object obj2;
      if (!members.TryGetValue((object) "_fields_", out obj2))
        return;
      this.__setattr__(context, "_fields_", obj2);
    }

    private StructType(Type underlyingSystemType)
      : base(underlyingSystemType)
    {
    }

    public static CTypes.ArrayType operator *(CTypes.StructType type, int count)
    {
      return CTypes.MakeArrayType((PythonType) type, count);
    }

    public static CTypes.ArrayType operator *(int count, CTypes.StructType type)
    {
      return CTypes.MakeArrayType((PythonType) type, count);
    }

    public CTypes._Structure from_address(CodeContext context, int address)
    {
      return this.from_address(context, new IntPtr(address));
    }

    public CTypes._Structure from_address(CodeContext context, BigInteger address)
    {
      return this.from_address(context, new IntPtr((long) address));
    }

    public CTypes._Structure from_address(CodeContext context, IntPtr ptr)
    {
      CTypes._Structure instance = (CTypes._Structure) this.CreateInstance(context);
      instance.SetAddress(ptr);
      return instance;
    }

    public CTypes._Structure from_buffer(CodeContext context, ArrayModule.array array, int offset = 0)
    {
      CTypes.ValidateArraySizes(array, offset, ((CTypes.INativeType) this).Size);
      CTypes._Structure instance = (CTypes._Structure) this.CreateInstance(context);
      instance._memHolder = new MemoryHolder(array.GetArrayAddress().Add(offset), ((CTypes.INativeType) this).Size);
      instance._memHolder.AddObject((object) "ffffffff", (object) array);
      return instance;
    }

    public CTypes._Structure from_buffer_copy(
      CodeContext context,
      ArrayModule.array array,
      int offset = 0)
    {
      CTypes.ValidateArraySizes(array, offset, ((CTypes.INativeType) this).Size);
      CTypes._Structure instance = (CTypes._Structure) this.CreateInstance(this.Context.SharedContext);
      instance._memHolder = new MemoryHolder(((CTypes.INativeType) this).Size);
      instance._memHolder.CopyFrom(array.GetArrayAddress().Add(offset), new IntPtr(((CTypes.INativeType) this).Size));
      GC.KeepAlive((object) array);
      return instance;
    }

    public object from_param(object obj)
    {
      return Builtin.isinstance(obj, (PythonType) this) ? obj : throw PythonOps.TypeError("expected {0} instance got {1}", (object) this.Name, (object) PythonTypeOps.GetName(obj));
    }

    public object in_dll(object library, string name)
    {
      throw new NotImplementedException("in dll");
    }

    public new virtual void __setattr__(CodeContext context, string name, object value)
    {
      if (name == "_fields_")
      {
        lock (this)
        {
          if (this._fields != null)
            throw PythonOps.AttributeError("_fields_ is final");
          this.SetFields(value);
        }
      }
      base.__setattr__(context, name, value);
    }

    int CTypes.INativeType.Size
    {
      get
      {
        this.EnsureSizeAndAlignment();
        return this._size.Value;
      }
    }

    int CTypes.INativeType.Alignment
    {
      get
      {
        this.EnsureSizeAndAlignment();
        return this._alignment.Value;
      }
    }

    object CTypes.INativeType.GetValue(
      MemoryHolder owner,
      object readingFrom,
      int offset,
      bool raw)
    {
      CTypes._Structure instance = (CTypes._Structure) this.CreateInstance(this.Context.SharedContext);
      instance._memHolder = owner.GetSubBlock(offset);
      return (object) instance;
    }

    object CTypes.INativeType.SetValue(MemoryHolder address, int offset, object value)
    {
      try
      {
        return this.SetValueInternal(address, offset, value);
      }
      catch (ArgumentTypeException ex)
      {
        throw PythonOps.RuntimeError("({0}) <type 'exceptions.TypeError'>: {1}", (object) this.Name, (object) ex.Message);
      }
      catch (ArgumentException ex)
      {
        throw PythonOps.RuntimeError("({0}) <type 'exceptions.ValueError'>: {1}", (object) this.Name, (object) ex.Message);
      }
    }

    internal object SetValueInternal(MemoryHolder address, int offset, object value)
    {
      switch (value)
      {
        case IList<object> objectList:
          if (objectList.Count > this._fields.Length)
            throw PythonOps.TypeError("too many initializers");
          for (int index = 0; index < objectList.Count; ++index)
            this._fields[index].SetValue(address, offset, objectList[index]);
          return (object) null;
        case CTypes.CData cdata:
          cdata._memHolder.CopyTo(address, offset, cdata.Size);
          return (object) cdata._memHolder.EnsureObjects();
        default:
          throw new NotImplementedException("set value");
      }
    }

    Type CTypes.INativeType.GetNativeType()
    {
      this.EnsureFinal();
      return CTypes.GetMarshalTypeFromSize(this._size.Value);
    }

    MarshalCleanup CTypes.INativeType.EmitMarshalling(
      ILGenerator method,
      LocalOrArg argIndex,
      List<object> constantPool,
      int constantPoolArgument)
    {
      Type type = argIndex.Type;
      argIndex.Emit(method);
      if (type.IsValueType)
        method.Emit(OpCodes.Box, type);
      constantPool.Add((object) this);
      method.Emit(OpCodes.Ldarg, constantPoolArgument);
      method.Emit(OpCodes.Ldc_I4, constantPool.Count - 1);
      method.Emit(OpCodes.Ldelem_Ref);
      method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("CheckCDataType"));
      method.Emit(OpCodes.Call, typeof (CTypes.CData).GetMethod("get_UnsafeAddress"));
      method.Emit(OpCodes.Ldobj, ((CTypes.INativeType) this).GetNativeType());
      return (MarshalCleanup) null;
    }

    Type CTypes.INativeType.GetPythonType() => typeof (object);

    void CTypes.INativeType.EmitReverseMarshalling(
      ILGenerator method,
      LocalOrArg value,
      List<object> constantPool,
      int constantPoolArgument)
    {
      value.Emit(method);
      CTypes.EmitCDataCreation((CTypes.INativeType) this, method, constantPool, constantPoolArgument);
    }

    string CTypes.INativeType.TypeFormat
    {
      get
      {
        if (this._pack.HasValue || this._fields == CTypes.StructType._emptyFields || this._fields == null)
          return "B";
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("T{");
        foreach (CTypes.Field field in this._fields)
        {
          stringBuilder.Append(field.NativeType.TypeFormat);
          stringBuilder.Append(':');
          stringBuilder.Append(field.FieldName);
          stringBuilder.Append(':');
        }
        stringBuilder.Append('}');
        return stringBuilder.ToString();
      }
    }

    internal static PythonType MakeSystemType(Type underlyingSystemType)
    {
      return PythonType.SetPythonType(underlyingSystemType, (PythonType) new CTypes.StructType(underlyingSystemType));
    }

    private void SetFields(object fields)
    {
      lock (this)
      {
        IList<object> fieldsList = CTypes.GetFieldsList(fields);
        int? bitCount = new int?();
        int? totalBitCount = new int?();
        CTypes.INativeType lastType = (CTypes.INativeType) null;
        int size;
        int alignment;
        List<CTypes.Field> alignmentAndFields = this.GetBaseSizeAlignmentAndFields(out size, out alignment);
        IList<object> anonymousFields = CTypes.StructType.GetAnonymousFields((PythonType) this);
        for (int index = 0; index < fieldsList.Count; ++index)
        {
          string fieldName1;
          CTypes.INativeType cdata;
          CTypes.GetFieldInfo((CTypes.INativeType) this, fieldsList[index], out fieldName1, out cdata, out bitCount);
          int num = this.UpdateSizeAndAlignment(cdata, bitCount, lastType, ref size, ref alignment, ref totalBitCount);
          string fieldName2 = fieldName1;
          CTypes.INativeType fieldType = cdata;
          int offset = num;
          int count = alignmentAndFields.Count;
          int? bits = bitCount;
          int? nullable1 = totalBitCount;
          int? nullable2 = bitCount;
          int? bitOffset = nullable1.HasValue & nullable2.HasValue ? new int?(nullable1.GetValueOrDefault() - nullable2.GetValueOrDefault()) : new int?();
          CTypes.Field field = new CTypes.Field(fieldName2, fieldType, offset, count, bits, bitOffset);
          alignmentAndFields.Add(field);
          this.AddSlot(fieldName1, (PythonTypeSlot) field);
          if (anonymousFields != null && anonymousFields.Contains((object) fieldName1))
            CTypes.StructType.AddAnonymousFields((PythonType) this, alignmentAndFields, cdata, field);
          lastType = cdata;
        }
        CTypes.StructType.CheckAnonymousFields(alignmentAndFields, anonymousFields);
        if (bitCount.HasValue)
          size += lastType.Size;
        this._fields = alignmentAndFields.ToArray();
        this._size = new int?(PythonStruct.Align(size, alignment));
        this._alignment = new int?(alignment);
      }
    }

    internal static void CheckAnonymousFields(
      List<CTypes.Field> allFields,
      IList<object> anonFields)
    {
      if (anonFields == null)
        return;
      foreach (string anonField in (IEnumerable<object>) anonFields)
      {
        bool flag = false;
        foreach (CTypes.Field allField in allFields)
        {
          if (allField.FieldName == anonField)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          throw PythonOps.AttributeError("anonymous field {0} is not defined in this structure", (object) anonField);
      }
    }

    internal static IList<object> GetAnonymousFields(PythonType type)
    {
      anonymousFields = (IList<object>) null;
      object ret;
      if (type.TryGetBoundAttr(type.Context.SharedContext, (object) type, "_anonymous_", out ret) && !(ret is IList<object> anonymousFields))
        throw PythonOps.TypeError("_anonymous_ must be a sequence");
      return anonymousFields;
    }

    internal static void AddAnonymousFields(
      PythonType type,
      List<CTypes.Field> allFields,
      CTypes.INativeType cdata,
      CTypes.Field newField)
    {
      CTypes.Field[] fields;
      switch (cdata)
      {
        case CTypes.StructType _:
          fields = ((CTypes.StructType) cdata)._fields;
          break;
        case CTypes.UnionType _:
          fields = ((CTypes.UnionType) cdata)._fields;
          break;
        default:
          throw PythonOps.TypeError("anonymous field must be struct or union");
      }
      foreach (CTypes.Field field in fields)
      {
        CTypes.Field slot = new CTypes.Field(field.FieldName, field.NativeType, checked (field.offset + newField.offset), allFields.Count);
        type.AddSlot(field.FieldName, (PythonTypeSlot) slot);
        allFields.Add(slot);
      }
    }

    private List<CTypes.Field> GetBaseSizeAlignmentAndFields(out int size, out int alignment)
    {
      size = 0;
      alignment = 1;
      List<CTypes.Field> alignmentAndFields = new List<CTypes.Field>();
      CTypes.INativeType lastType = (CTypes.INativeType) null;
      int? totalBitCount = new int?();
      foreach (PythonType baseType in (IEnumerable<PythonType>) this.BaseTypes)
      {
        if (baseType is CTypes.StructType structType)
        {
          foreach (CTypes.Field field in structType._fields)
          {
            alignmentAndFields.Add(field);
            this.UpdateSizeAndAlignment(field.NativeType, field.BitCount, lastType, ref size, ref alignment, ref totalBitCount);
            lastType = field.NativeType != this ? field.NativeType : throw CTypes.StructureCannotContainSelf();
          }
        }
      }
      return alignmentAndFields;
    }

    private int UpdateSizeAndAlignment(
      CTypes.INativeType cdata,
      int? bitCount,
      CTypes.INativeType lastType,
      ref int size,
      ref int alignment,
      ref int? totalBitCount)
    {
      int num1 = size;
      if (bitCount.HasValue)
      {
        if (lastType != null && lastType.Size != cdata.Size)
        {
          totalBitCount = new int?();
          num1 = (size += lastType.Size);
        }
        size = PythonStruct.Align(size, cdata.Alignment);
        if (totalBitCount.HasValue)
        {
          int? nullable1 = bitCount;
          int? nullable2 = totalBitCount;
          int? nullable3 = nullable1.HasValue & nullable2.HasValue ? new int?((nullable1.GetValueOrDefault() + nullable2.GetValueOrDefault() + 7) / 8) : new int?();
          int size1 = cdata.Size;
          if (nullable3.GetValueOrDefault() <= size1 & nullable3.HasValue)
          {
            ref int? local = ref totalBitCount;
            int? nullable4 = bitCount;
            nullable2 = totalBitCount;
            int? nullable5;
            if (!(nullable4.HasValue & nullable2.HasValue))
            {
              nullable1 = new int?();
              nullable5 = nullable1;
            }
            else
              nullable5 = new int?(nullable4.GetValueOrDefault() + nullable2.GetValueOrDefault());
            local = nullable5;
          }
          else
          {
            size += lastType.Size;
            num1 = size;
            totalBitCount = bitCount;
          }
        }
        else
          totalBitCount = bitCount;
      }
      else
      {
        if (totalBitCount.HasValue)
        {
          size += lastType.Size;
          int num2 = size;
          totalBitCount = new int?();
        }
        if (this._pack.HasValue)
        {
          alignment = this._pack.Value;
          num1 = size = PythonStruct.Align(size, this._pack.Value);
          size += cdata.Size;
        }
        else
        {
          alignment = Math.Max(alignment, cdata.Alignment);
          num1 = size = PythonStruct.Align(size, cdata.Alignment);
          size += cdata.Size;
        }
      }
      return num1;
    }

    internal void EnsureFinal()
    {
      if (this._fields != null)
        return;
      this.SetFields((object) PythonTuple.EMPTY);
      if (this._fields.Length != 0)
        return;
      this._fields = CTypes.StructType._emptyFields;
    }

    private void EnsureSizeAndAlignment()
    {
      if (this._size.HasValue)
        return;
      lock (this)
      {
        if (this._size.HasValue)
          return;
        int size;
        int alignment;
        this.GetBaseSizeAlignmentAndFields(out size, out alignment);
        this._size = new int?(size);
        this._alignment = new int?(alignment);
      }
    }
  }

  [PythonType("Structure")]
  public abstract class _Structure : CTypes.CData
  {
    protected _Structure()
    {
      ((CTypes.StructType) this.NativeType).EnsureFinal();
      this._memHolder = new MemoryHolder(this.NativeType.Size);
    }

    public void __init__(params object[] args)
    {
      this.CheckAbstract();
      ((CTypes.StructType) this.NativeType).SetValueInternal(this._memHolder, 0, (object) args);
    }

    public void __init__(CodeContext context, [ParamDictionary] IDictionary<string, object> kwargs)
    {
      this.CheckAbstract();
      foreach (KeyValuePair<string, object> kwarg in (IEnumerable<KeyValuePair<string, object>>) kwargs)
        PythonOps.SetAttr(context, (object) this, kwarg.Key, kwarg.Value);
    }

    private void CheckAbstract()
    {
      if (((PythonType) this.NativeType).TryGetBoundAttr(((PythonType) this.NativeType).Context.SharedContext, (object) this, "_abstract_", out object _))
        throw PythonOps.TypeError("abstract class");
    }
  }

  [PythonType("Union")]
  public abstract class _Union : CTypes.CData
  {
  }

  [PythonType]
  [PythonHidden(new PlatformID[] {})]
  public class UnionType : PythonType, CTypes.INativeType
  {
    internal CTypes.Field[] _fields;
    private int _size;
    private int _alignment;

    public UnionType(
      CodeContext context,
      string name,
      PythonTuple bases,
      PythonDictionary members)
      : base(context, name, bases, members)
    {
      object fields;
      if (!members.TryGetValue((object) "_fields_", out fields))
        return;
      this.SetFields(fields);
    }

    public new void __setattr__(CodeContext context, string name, object value)
    {
      if (name == "_fields_")
      {
        lock (this)
        {
          if (this._fields != null)
            throw PythonOps.AttributeError("_fields_ is final");
          this.SetFields(value);
        }
      }
      base.__setattr__(context, name, value);
    }

    private UnionType(Type underlyingSystemType)
      : base(underlyingSystemType)
    {
    }

    public object from_param(object obj) => (object) null;

    internal static PythonType MakeSystemType(Type underlyingSystemType)
    {
      return PythonType.SetPythonType(underlyingSystemType, (PythonType) new CTypes.UnionType(underlyingSystemType));
    }

    public static CTypes.ArrayType operator *(CTypes.UnionType type, int count)
    {
      return CTypes.MakeArrayType((PythonType) type, count);
    }

    public static CTypes.ArrayType operator *(int count, CTypes.UnionType type)
    {
      return CTypes.MakeArrayType((PythonType) type, count);
    }

    public object from_buffer(CodeContext context, ArrayModule.array array, int offset = 0)
    {
      CTypes.ValidateArraySizes(array, offset, ((CTypes.INativeType) this).Size);
      CTypes._Union instance = (CTypes._Union) this.CreateInstance(context);
      instance._memHolder = new MemoryHolder(array.GetArrayAddress().Add(offset), ((CTypes.INativeType) this).Size);
      instance._memHolder.AddObject((object) "ffffffff", (object) array);
      return (object) instance;
    }

    int CTypes.INativeType.Size => this._size;

    int CTypes.INativeType.Alignment => this._alignment;

    object CTypes.INativeType.GetValue(
      MemoryHolder owner,
      object readingFrom,
      int offset,
      bool raw)
    {
      CTypes._Union instance = (CTypes._Union) this.CreateInstance(this.Context.SharedContext);
      instance._memHolder = owner.GetSubBlock(offset);
      return (object) instance;
    }

    object CTypes.INativeType.SetValue(MemoryHolder address, int offset, object value)
    {
      switch (value)
      {
        case IList<object> objectList:
          if (objectList.Count > this._fields.Length)
            throw PythonOps.TypeError("too many initializers");
          for (int index = 0; index < objectList.Count; ++index)
            this._fields[index].SetValue(address, offset, objectList[index]);
          return (object) null;
        case CTypes.CData cdata:
          cdata._memHolder.CopyTo(address, offset, cdata.Size);
          return (object) cdata._memHolder.EnsureObjects();
        default:
          throw new NotImplementedException("Union set value");
      }
    }

    Type CTypes.INativeType.GetNativeType() => CTypes.GetMarshalTypeFromSize(this._size);

    MarshalCleanup CTypes.INativeType.EmitMarshalling(
      ILGenerator method,
      LocalOrArg argIndex,
      List<object> constantPool,
      int constantPoolArgument)
    {
      Type type = argIndex.Type;
      argIndex.Emit(method);
      if (type.IsValueType)
        method.Emit(OpCodes.Box, type);
      constantPool.Add((object) this);
      method.Emit(OpCodes.Ldarg, constantPoolArgument);
      method.Emit(OpCodes.Ldc_I4, constantPool.Count - 1);
      method.Emit(OpCodes.Ldelem_Ref);
      method.Emit(OpCodes.Call, typeof (ModuleOps).GetMethod("CheckCDataType"));
      method.Emit(OpCodes.Call, typeof (CTypes.CData).GetMethod("get_UnsafeAddress"));
      method.Emit(OpCodes.Ldobj, ((CTypes.INativeType) this).GetNativeType());
      return (MarshalCleanup) null;
    }

    Type CTypes.INativeType.GetPythonType() => typeof (object);

    void CTypes.INativeType.EmitReverseMarshalling(
      ILGenerator method,
      LocalOrArg value,
      List<object> constantPool,
      int constantPoolArgument)
    {
      value.Emit(method);
      CTypes.EmitCDataCreation((CTypes.INativeType) this, method, constantPool, constantPoolArgument);
    }

    string CTypes.INativeType.TypeFormat => "B";

    private void SetFields(object fields)
    {
      lock (this)
      {
        IList<object> fieldsList = CTypes.GetFieldsList(fields);
        IList<object> anonymousFields = CTypes.StructType.GetAnonymousFields((PythonType) this);
        int num1 = 0;
        int num2 = 1;
        List<CTypes.Field> allFields = new List<CTypes.Field>();
        for (int index = 0; index < fieldsList.Count; ++index)
        {
          string fieldName;
          CTypes.INativeType cdata;
          CTypes.GetFieldInfo((CTypes.INativeType) this, fieldsList[index], out fieldName, out cdata, out int? _);
          num2 = Math.Max(num2, cdata.Alignment);
          num1 = Math.Max(num1, cdata.Size);
          CTypes.Field field = new CTypes.Field(fieldName, cdata, 0, allFields.Count);
          allFields.Add(field);
          this.AddSlot(fieldName, (PythonTypeSlot) field);
          if (anonymousFields != null && anonymousFields.Contains((object) fieldName))
            CTypes.StructType.AddAnonymousFields((PythonType) this, allFields, cdata, field);
        }
        CTypes.StructType.CheckAnonymousFields(allFields, anonymousFields);
        this._fields = allFields.ToArray();
        this._size = PythonStruct.Align(num1, num2);
        this._alignment = num2;
      }
    }

    internal void EnsureFinal()
    {
      if (this._fields != null)
        return;
      this.SetFields((object) PythonTuple.EMPTY);
    }
  }

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate IntPtr CastDelegate(IntPtr data, IntPtr obj, IntPtr type);

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  private delegate IntPtr StringAtDelegate(IntPtr addr, int length);

  private class RefCountInfo
  {
    public int RefCount;
    public GCHandle Handle;
  }

  [PythonHidden(new PlatformID[] {})]
  [PythonType("COMError")]
  [DynamicBaseType]
  public class _COMError(PythonType cls) : PythonExceptions.BaseException(cls)
  {
    public override void __init__(params object[] args)
    {
      base.__init__(args);
      this.hresult = args.Length >= 3 ? args[0] : throw PythonOps.TypeError($"COMError() takes exactly 4 arguments({args.Length} given)");
      this.text = args[1];
      this.details = args[2];
    }

    public object hresult { get; set; }

    public object text { get; set; }

    public object details { get; set; }
  }
}
