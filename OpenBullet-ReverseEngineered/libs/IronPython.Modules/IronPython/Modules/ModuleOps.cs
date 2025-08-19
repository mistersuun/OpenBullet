// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.ModuleOps
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Utils;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable disable
namespace IronPython.Modules;

public static class ModuleOps
{
  private static readonly byte[] FakeZeroLength = new byte[1]
  {
    (byte) 42
  };

  public static IntPtr StringToHGlobalAnsi(string str)
  {
    return str == null ? IntPtr.Zero : Marshal.StringToHGlobalAnsi(str);
  }

  public static IntPtr StringToHGlobalUni(string str)
  {
    return str == null ? IntPtr.Zero : Marshal.StringToHGlobalUni(str);
  }

  public static object DoErrorCheck(
    object errCheckFunc,
    object result,
    object func,
    object[] arguments)
  {
    return PythonCalls.Call(errCheckFunc, result, func, (object) PythonTuple.Make((object) arguments));
  }

  public static object CreateMemoryHolder(IntPtr data, int size)
  {
    MemoryHolder memoryHolder = new MemoryHolder(size);
    memoryHolder.CopyFrom(data, new IntPtr(size));
    return (object) memoryHolder;
  }

  public static object CreateNativeWrapper(PythonType type, object holder)
  {
    CTypes.CData instance = (CTypes.CData) type.CreateInstance(type.Context.SharedContext);
    instance._memHolder = (MemoryHolder) holder;
    return (object) instance;
  }

  public static object CreateCData(IntPtr dataAddress, PythonType type)
  {
    CTypes.INativeType nativeType = (CTypes.INativeType) type;
    CTypes.CData instance = (CTypes.CData) type.CreateInstance(type.Context.SharedContext);
    instance._memHolder = new MemoryHolder(nativeType.Size);
    instance._memHolder.CopyFrom(dataAddress, new IntPtr(nativeType.Size));
    return (object) instance;
  }

  public static object CreateCFunction(IntPtr address, PythonType type)
  {
    return type.CreateInstance(type.Context.SharedContext, (object) address);
  }

  public static CTypes.CData CheckSimpleCDataType(object o, object type)
  {
    object ret;
    if (!(o is CTypes.SimpleCData simpleCdata) && PythonOps.TryGetBoundAttr(o, "_as_parameter_", out ret))
      simpleCdata = ret as CTypes.SimpleCData;
    if (simpleCdata != null && simpleCdata.NativeType != type)
      throw PythonOps.TypeErrorForTypeMismatch(((PythonType) type).Name, o);
    return (CTypes.CData) simpleCdata;
  }

  public static CTypes.CData CheckCDataType(object o, object type)
  {
    object ret;
    if (!(o is CTypes.CData cdata) && PythonOps.TryGetBoundAttr(o, "_as_parameter_", out ret))
      cdata = ret as CTypes.CData;
    bool flag = true;
    if (cdata != null && cdata.NativeType is CTypes.ArrayType nativeType)
    {
      int num;
      switch (type)
      {
        case CTypes.ArrayType arrayType when arrayType.ElementType == nativeType.ElementType:
          num = 1;
          break;
        case CTypes.PointerType pointerType:
          num = pointerType._type == nativeType.ElementType ? 1 : 0;
          break;
        default:
          num = 0;
          break;
      }
      flag = num != 0;
    }
    return cdata != null && flag ? cdata : throw ModuleOps.ArgumentError(type, ((PythonType) type).Name, o);
  }

  public static IntPtr GetFunctionPointerValue(object o, object type)
  {
    object ret;
    if (!(o is CTypes._CFuncPtr cfuncPtr) && PythonOps.TryGetBoundAttr(o, "_as_parameter_", out ret))
      cfuncPtr = ret as CTypes._CFuncPtr;
    if (cfuncPtr == null || cfuncPtr.NativeType != type)
      throw ModuleOps.ArgumentError(type, ((PythonType) type).Name, o);
    return cfuncPtr.addr;
  }

  public static CTypes.CData TryCheckCDataPointerType(object o, object type)
  {
    object ret;
    if (!(o is CTypes.CData cdata) && PythonOps.TryGetBoundAttr(o, "_as_parameter_", out ret))
      cdata = ret as CTypes.CData;
    bool flag = true;
    if (cdata != null && cdata.NativeType is CTypes.ArrayType nativeType)
    {
      int num;
      switch (type)
      {
        case CTypes.ArrayType arrayType when arrayType.ElementType == nativeType.ElementType:
          num = 1;
          break;
        case CTypes.PointerType pointerType:
          num = pointerType._type == nativeType.ElementType ? 1 : 0;
          break;
        default:
          num = 0;
          break;
      }
      flag = num != 0;
    }
    return cdata == null || flag ? cdata : throw ModuleOps.ArgumentError(type, ((PythonType) ((CTypes.PointerType) type)._type).Name, o);
  }

  public static CTypes._Array TryCheckCharArray(object o)
  {
    return o is CTypes._Array array && ((CTypes.ArrayType) array.NativeType).ElementType is CTypes.SimpleType elementType && elementType._type == CTypes.SimpleTypeKind.Char ? array : (CTypes._Array) null;
  }

  public static byte[] TryCheckBytes(object o)
  {
    if (!(o is Bytes bytes))
      return (byte[]) null;
    return bytes._bytes.Length == 0 ? ModuleOps.FakeZeroLength : bytes._bytes;
  }

  public static byte[] GetBytes(Bytes bytes) => bytes._bytes;

  public static CTypes._Array TryCheckWCharArray(object o)
  {
    return o is CTypes._Array array && ((CTypes.ArrayType) array.NativeType).ElementType is CTypes.SimpleType elementType && elementType._type == CTypes.SimpleTypeKind.WChar ? array : (CTypes._Array) null;
  }

  public static object CreateSubclassInstance(object type, object instance)
  {
    return PythonCalls.Call(type, instance);
  }

  public static void CallbackException(Exception e, CodeContext context)
  {
    PythonContext languageContext = context.LanguageContext;
    object systemStandardError = languageContext.SystemStandardError;
    PythonOps.PrintWithDest(context, systemStandardError, (object) languageContext.FormatException(e));
  }

  private static Exception ArgumentError(object type, string expected, object got)
  {
    return PythonExceptions.CreateThrowable((PythonType) ((PythonType) type).Context.GetModuleState((object) nameof (ArgumentError)), (object) $"expected {expected}, got {DynamicHelpers.GetPythonType(got).Name}");
  }

  public static CTypes.CData CheckNativeArgument(object o, object type)
  {
    if (!(o is CTypes.NativeArgument nativeArgument))
      return (CTypes.CData) null;
    if (((CTypes.PointerType) type)._type != DynamicHelpers.GetPythonType((object) nativeArgument._obj))
      throw ModuleOps.ArgumentError(type, ((PythonType) type).Name, o);
    return nativeArgument._obj;
  }

  public static string CharToString(byte c) => new string((char) c, 1);

  public static string WCharToString(char c) => new string(c, 1);

  public static char StringToChar(string s) => s[0];

  public static string EnsureString(object o)
  {
    return o is string str ? str : throw PythonOps.TypeErrorForTypeMismatch("str", o);
  }

  public static bool CheckFunctionId(CTypes._CFuncPtr func, int id) => func.Id == id;

  public static IntPtr GetWCharPointer(object value)
  {
    if (value is string s)
      return Marshal.StringToCoTaskMemUni(s);
    if (value == null)
      return IntPtr.Zero;
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret))
      return ModuleOps.GetWCharPointer(ret);
    throw PythonOps.TypeErrorForTypeMismatch("wchar pointer", value);
  }

  public static IntPtr GetBSTR(object value)
  {
    if (value is string s)
      return Marshal.StringToBSTR(s);
    if (value == null)
      return IntPtr.Zero;
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret))
      return ModuleOps.GetBSTR(ret);
    throw PythonOps.TypeErrorForTypeMismatch("BSTR", value);
  }

  public static IntPtr GetCharPointer(object value)
  {
    if (value is string s)
      return Marshal.StringToCoTaskMemAnsi(s);
    if (value == null)
      return IntPtr.Zero;
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret))
      return ModuleOps.GetCharPointer(ret);
    throw PythonOps.TypeErrorForTypeMismatch("char pointer", value);
  }

  public static IntPtr GetPointer(object value)
  {
    switch (value)
    {
      case null:
        return IntPtr.Zero;
      case int num1:
        if (num1 > int.MaxValue)
          num1 = -1;
        return new IntPtr(num1);
      case BigInteger bigInteger:
        if (bigInteger > long.MaxValue)
          bigInteger = (BigInteger) -1;
        return new IntPtr((long) bigInteger);
      case long num2:
        return new IntPtr(num2);
      default:
        object ret;
        if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret))
          return ModuleOps.GetPointer(ret);
        if (value is CTypes.SimpleCData simpleCdata)
        {
          CTypes.SimpleType nativeType = (CTypes.SimpleType) simpleCdata.NativeType;
          if (nativeType._type == CTypes.SimpleTypeKind.WCharPointer || nativeType._type == CTypes.SimpleTypeKind.CharPointer)
            return simpleCdata.UnsafeAddress;
          if (nativeType._type == CTypes.SimpleTypeKind.Pointer)
            return simpleCdata._memHolder.ReadIntPtr(0);
        }
        if (value is CTypes._Array array)
          return array.UnsafeAddress;
        if (value is CTypes._CFuncPtr cfuncPtr)
          return cfuncPtr.UnsafeAddress;
        return value is CTypes.Pointer pointer ? pointer.UnsafeAddress : throw PythonOps.TypeErrorForTypeMismatch("pointer", value);
    }
  }

  public static IntPtr GetInterfacePointer(IntPtr self, int offset)
  {
    return Marshal.ReadIntPtr(Marshal.ReadIntPtr(self), offset * IntPtr.Size);
  }

  public static IntPtr GetObject(object value) => GCHandle.ToIntPtr(GCHandle.Alloc(value));

  public static long GetSignedLongLong(object value, object type)
  {
    int? int32 = Converter.ImplicitConvertToInt32(value);
    if (int32.HasValue)
      return (long) int32.Value;
    if (value is BigInteger signedLongLong)
      return (long) signedLongLong;
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret))
      return ModuleOps.GetSignedLongLong(ret, type);
    throw PythonOps.TypeErrorForTypeMismatch("signed long long ", value);
  }

  public static long GetUnsignedLongLong(object value, object type)
  {
    int? int32 = Converter.ImplicitConvertToInt32(value);
    if (int32.HasValue && int32.Value >= 0)
      return (long) int32.Value;
    if (value is BigInteger unsignedLongLong)
      return (long) (ulong) unsignedLongLong;
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret))
      return ModuleOps.GetUnsignedLongLong(ret, type);
    throw PythonOps.TypeErrorForTypeMismatch("unsigned long long", value);
  }

  public static double GetDouble(object value, object type)
  {
    switch (value)
    {
      case double num1:
        return num1;
      case float num2:
        return (double) num2;
      case int num3:
        return (double) num3;
      case BigInteger self:
        return self.ToFloat64();
      default:
        object ret;
        return PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret) ? ModuleOps.GetDouble(ret, type) : Converter.ConvertToDouble(value);
    }
  }

  public static float GetSingle(object value, object type)
  {
    switch (value)
    {
      case double single1:
        return (float) single1;
      case float single2:
        return single2;
      case int single3:
        return (float) single3;
      case BigInteger self:
        return (float) self.ToFloat64();
      default:
        object ret;
        return PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret) ? ModuleOps.GetSingle(ret, type) : (float) Converter.ConvertToDouble(value);
    }
  }

  public static long GetDoubleBits(object value)
  {
    switch (value)
    {
      case double num1:
        return BitConverter.ToInt64(BitConverter.GetBytes(num1), 0);
      case float num2:
        return BitConverter.ToInt64(BitConverter.GetBytes(num2), 0);
      case int num3:
        return BitConverter.ToInt64(BitConverter.GetBytes((double) num3), 0);
      case BigInteger self:
        return BitConverter.ToInt64(BitConverter.GetBytes(self.ToFloat64()), 0);
      default:
        object ret;
        return PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret) ? ModuleOps.GetDoubleBits(ret) : BitConverter.ToInt64(BitConverter.GetBytes(Converter.ConvertToDouble(value)), 0);
    }
  }

  public static int GetSingleBits(object value)
  {
    switch (value)
    {
      case double num1:
        return BitConverter.ToInt32(BitConverter.GetBytes((float) num1), 0);
      case float num2:
        return BitConverter.ToInt32(BitConverter.GetBytes(num2), 0);
      case int num3:
        return BitConverter.ToInt32(BitConverter.GetBytes((float) num3), 0);
      case BigInteger self:
        return BitConverter.ToInt32(BitConverter.GetBytes((float) self.ToFloat64()), 0);
      default:
        object ret;
        return PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret) ? ModuleOps.GetSingleBits(ret) : BitConverter.ToInt32(BitConverter.GetBytes((float) Converter.ConvertToDouble(value)), 0);
    }
  }

  public static int GetSignedLong(object value, object type)
  {
    if (value is int signedLong)
      return signedLong;
    int? int32 = Converter.ImplicitConvertToInt32(value);
    if (int32.HasValue)
      return int32.Value;
    uint ret1;
    if (value is BigInteger self && self.AsUInt32(out ret1))
      return (int) ret1;
    object ret2;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret2))
      return ModuleOps.GetSignedLong(ret2, type);
    throw PythonOps.TypeErrorForTypeMismatch("signed long", value);
  }

  public static int GetUnsignedLong(object value, object type)
  {
    int? int32 = Converter.ImplicitConvertToInt32(value);
    if (int32.HasValue)
      return int32.Value;
    uint ret1;
    if (value is BigInteger self && self.AsUInt32(out ret1))
      return (int) ret1;
    object ret2;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret2))
      return ModuleOps.GetUnsignedLong(ret2, type);
    throw PythonOps.TypeErrorForTypeMismatch("unsigned long", value);
  }

  public static int GetUnsignedInt(object value, object type)
  {
    int? int32 = Converter.ImplicitConvertToInt32(value);
    if (int32.HasValue && int32.Value >= 0)
      return int32.Value;
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret))
      return ModuleOps.GetUnsignedInt(type, ret);
    throw PythonOps.TypeErrorForTypeMismatch("unsigned int", value);
  }

  public static int GetSignedInt(object value, object type)
  {
    int? int32 = Converter.ImplicitConvertToInt32(value);
    if (int32.HasValue)
      return int32.Value;
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret))
      return ModuleOps.GetSignedInt(ret, type);
    throw PythonOps.TypeErrorForTypeMismatch("signed int", value);
  }

  public static short GetUnsignedShort(object value, object type)
  {
    int? int32 = Converter.ImplicitConvertToInt32(value);
    if (int32.HasValue)
    {
      int unsignedShort = int32.Value;
      if (unsignedShort >= 0 && unsignedShort <= (int) ushort.MaxValue)
        return (short) (ushort) unsignedShort;
    }
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret))
      return ModuleOps.GetUnsignedShort(ret, type);
    throw PythonOps.TypeErrorForTypeMismatch("unsigned short", value);
  }

  public static short GetSignedShort(object value, object type)
  {
    int? int32 = Converter.ImplicitConvertToInt32(value);
    if (int32.HasValue)
      return (short) int32.Value;
    if (value is BigInteger bigInteger)
      return (short) (int) (bigInteger & (BigInteger) (int) ushort.MaxValue);
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret))
      return ModuleOps.GetSignedShort(ret, type);
    throw PythonOps.TypeErrorForTypeMismatch("signed short", value);
  }

  public static int GetVariantBool(object value, object type)
  {
    return !Converter.ConvertToBoolean(value) ? 0 : 1;
  }

  public static byte GetUnsignedByte(object value, object type)
  {
    int? int32 = Converter.ImplicitConvertToInt32(value);
    if (int32.HasValue)
      return (byte) int32.Value;
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret))
      return ModuleOps.GetUnsignedByte(ret, type);
    throw PythonOps.TypeErrorForTypeMismatch("unsigned byte", value);
  }

  public static byte GetSignedByte(object value, object type)
  {
    int? int32 = Converter.ImplicitConvertToInt32(value);
    if (int32.HasValue)
    {
      int signedByte = int32.Value;
      if (signedByte >= (int) sbyte.MinValue && signedByte <= (int) sbyte.MaxValue)
        return (byte) signedByte;
    }
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret))
      return ModuleOps.GetSignedByte(ret, type);
    throw PythonOps.TypeErrorForTypeMismatch("signed byte", value);
  }

  public static byte GetBoolean(object value, object type)
  {
    if (value is bool flag)
      return !flag ? (byte) 0 : (byte) 1;
    if (value is int num)
      return num == 0 ? (byte) 0 : (byte) 1;
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret))
      return ModuleOps.GetBoolean(ret, type);
    throw PythonOps.TypeErrorForTypeMismatch("bool", value);
  }

  public static byte GetChar(object value, object type)
  {
    if (value is string str && str.Length == 1)
      return (byte) str[0];
    if (value is Bytes bytes && bytes.Count == 1)
      return bytes._bytes[0];
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret))
      return ModuleOps.GetChar(ret, type);
    throw ModuleOps.ArgumentError(type, "char", value);
  }

  public static char GetWChar(object value, object type)
  {
    if (value is string str && str.Length == 1)
      return str[0];
    object ret;
    if (PythonOps.TryGetBoundAttr(value, "_as_parameter_", out ret))
      return ModuleOps.GetWChar(ret, type);
    throw ModuleOps.ArgumentError(type, "wchar", value);
  }

  public static object IntPtrToObject(IntPtr address)
  {
    GCHandle gcHandle = GCHandle.FromIntPtr(address);
    object target = gcHandle.Target;
    gcHandle.Free();
    return target;
  }
}
