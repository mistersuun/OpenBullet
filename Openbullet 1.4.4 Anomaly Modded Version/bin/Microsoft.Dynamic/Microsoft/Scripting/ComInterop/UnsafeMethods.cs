// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.UnsafeMethods
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Security.Permissions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal static class UnsafeMethods
{
  private static readonly MethodInfo _ConvertByrefToPtr = UnsafeMethods.Create_ConvertByrefToPtr();
  private static readonly UnsafeMethods.ConvertByrefToPtrDelegate<Variant> _ConvertVariantByrefToPtr = (UnsafeMethods.ConvertByrefToPtrDelegate<Variant>) Delegate.CreateDelegate(typeof (UnsafeMethods.ConvertByrefToPtrDelegate<Variant>), UnsafeMethods._ConvertByrefToPtr.MakeGenericMethod(typeof (Variant)));
  private static readonly UnsafeMethods.ConvertByrefToPtrDelegate<sbyte> _ConvertSByteByrefToPtr = (UnsafeMethods.ConvertByrefToPtrDelegate<sbyte>) Delegate.CreateDelegate(typeof (UnsafeMethods.ConvertByrefToPtrDelegate<sbyte>), UnsafeMethods._ConvertByrefToPtr.MakeGenericMethod(typeof (sbyte)));
  private static readonly UnsafeMethods.ConvertByrefToPtrDelegate<short> _ConvertInt16ByrefToPtr = (UnsafeMethods.ConvertByrefToPtrDelegate<short>) Delegate.CreateDelegate(typeof (UnsafeMethods.ConvertByrefToPtrDelegate<short>), UnsafeMethods._ConvertByrefToPtr.MakeGenericMethod(typeof (short)));
  private static readonly UnsafeMethods.ConvertByrefToPtrDelegate<int> _ConvertInt32ByrefToPtr = (UnsafeMethods.ConvertByrefToPtrDelegate<int>) Delegate.CreateDelegate(typeof (UnsafeMethods.ConvertByrefToPtrDelegate<int>), UnsafeMethods._ConvertByrefToPtr.MakeGenericMethod(typeof (int)));
  private static readonly UnsafeMethods.ConvertByrefToPtrDelegate<long> _ConvertInt64ByrefToPtr = (UnsafeMethods.ConvertByrefToPtrDelegate<long>) Delegate.CreateDelegate(typeof (UnsafeMethods.ConvertByrefToPtrDelegate<long>), UnsafeMethods._ConvertByrefToPtr.MakeGenericMethod(typeof (long)));
  private static readonly UnsafeMethods.ConvertByrefToPtrDelegate<byte> _ConvertByteByrefToPtr = (UnsafeMethods.ConvertByrefToPtrDelegate<byte>) Delegate.CreateDelegate(typeof (UnsafeMethods.ConvertByrefToPtrDelegate<byte>), UnsafeMethods._ConvertByrefToPtr.MakeGenericMethod(typeof (byte)));
  private static readonly UnsafeMethods.ConvertByrefToPtrDelegate<ushort> _ConvertUInt16ByrefToPtr = (UnsafeMethods.ConvertByrefToPtrDelegate<ushort>) Delegate.CreateDelegate(typeof (UnsafeMethods.ConvertByrefToPtrDelegate<ushort>), UnsafeMethods._ConvertByrefToPtr.MakeGenericMethod(typeof (ushort)));
  private static readonly UnsafeMethods.ConvertByrefToPtrDelegate<uint> _ConvertUInt32ByrefToPtr = (UnsafeMethods.ConvertByrefToPtrDelegate<uint>) Delegate.CreateDelegate(typeof (UnsafeMethods.ConvertByrefToPtrDelegate<uint>), UnsafeMethods._ConvertByrefToPtr.MakeGenericMethod(typeof (uint)));
  private static readonly UnsafeMethods.ConvertByrefToPtrDelegate<ulong> _ConvertUInt64ByrefToPtr = (UnsafeMethods.ConvertByrefToPtrDelegate<ulong>) Delegate.CreateDelegate(typeof (UnsafeMethods.ConvertByrefToPtrDelegate<ulong>), UnsafeMethods._ConvertByrefToPtr.MakeGenericMethod(typeof (ulong)));
  private static readonly UnsafeMethods.ConvertByrefToPtrDelegate<IntPtr> _ConvertIntPtrByrefToPtr = (UnsafeMethods.ConvertByrefToPtrDelegate<IntPtr>) Delegate.CreateDelegate(typeof (UnsafeMethods.ConvertByrefToPtrDelegate<IntPtr>), UnsafeMethods._ConvertByrefToPtr.MakeGenericMethod(typeof (IntPtr)));
  private static readonly UnsafeMethods.ConvertByrefToPtrDelegate<UIntPtr> _ConvertUIntPtrByrefToPtr = (UnsafeMethods.ConvertByrefToPtrDelegate<UIntPtr>) Delegate.CreateDelegate(typeof (UnsafeMethods.ConvertByrefToPtrDelegate<UIntPtr>), UnsafeMethods._ConvertByrefToPtr.MakeGenericMethod(typeof (UIntPtr)));
  private static readonly UnsafeMethods.ConvertByrefToPtrDelegate<float> _ConvertSingleByrefToPtr = (UnsafeMethods.ConvertByrefToPtrDelegate<float>) Delegate.CreateDelegate(typeof (UnsafeMethods.ConvertByrefToPtrDelegate<float>), UnsafeMethods._ConvertByrefToPtr.MakeGenericMethod(typeof (float)));
  private static readonly UnsafeMethods.ConvertByrefToPtrDelegate<double> _ConvertDoubleByrefToPtr = (UnsafeMethods.ConvertByrefToPtrDelegate<double>) Delegate.CreateDelegate(typeof (UnsafeMethods.ConvertByrefToPtrDelegate<double>), UnsafeMethods._ConvertByrefToPtr.MakeGenericMethod(typeof (double)));
  private static readonly UnsafeMethods.ConvertByrefToPtrDelegate<Decimal> _ConvertDecimalByrefToPtr = (UnsafeMethods.ConvertByrefToPtrDelegate<Decimal>) Delegate.CreateDelegate(typeof (UnsafeMethods.ConvertByrefToPtrDelegate<Decimal>), UnsafeMethods._ConvertByrefToPtr.MakeGenericMethod(typeof (Decimal)));
  private static readonly object _lock = new object();
  private static ModuleBuilder _dynamicModule;
  private const int _dummyMarker = 269488144 /*0x10101010*/;
  private static readonly UnsafeMethods.IUnknownReleaseDelegate _IUnknownRelease = UnsafeMethods.Create_IUnknownRelease();
  internal static readonly IntPtr NullInterfaceId = UnsafeMethods.GetNullInterfaceId();
  private static readonly UnsafeMethods.IDispatchInvokeDelegate _IDispatchInvoke = UnsafeMethods.Create_IDispatchInvoke(true);
  private static UnsafeMethods.IDispatchInvokeDelegate _IDispatchInvokeNoResultImpl;

  [DllImport("oleaut32.dll", PreserveSig = false)]
  internal static extern void VariantClear(IntPtr variant);

  [DllImport("oleaut32.dll", PreserveSig = false)]
  internal static extern ITypeLib LoadRegTypeLib(
    ref Guid clsid,
    short majorVersion,
    short minorVersion,
    int lcid);

  private static MethodInfo Create_ConvertByrefToPtr()
  {
    TypeBuilder typeBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("ComSnippets"), AssemblyBuilderAccess.Run).DefineDynamicModule("ComSnippets").DefineType("Type$ConvertByrefToPtr", TypeAttributes.Public);
    Type[] parameterTypes = new Type[1]
    {
      typeof (Variant).MakeByRefType()
    };
    MethodBuilder methodBuilder = typeBuilder.DefineMethod("ConvertByrefToPtr", MethodAttributes.Public | MethodAttributes.Static, typeof (IntPtr), parameterTypes);
    GenericTypeParameterBuilder[] parameterBuilderArray = methodBuilder.DefineGenericParameters("T");
    parameterBuilderArray[0].SetGenericParameterAttributes(GenericParameterAttributes.NotNullableValueTypeConstraint);
    methodBuilder.SetSignature(typeof (IntPtr), (Type[]) null, (Type[]) null, new Type[1]
    {
      parameterBuilderArray[0].MakeByRefType()
    }, (Type[][]) null, (Type[][]) null);
    ILGenerator ilGenerator = methodBuilder.GetILGenerator();
    ilGenerator.Emit(OpCodes.Ldarg_0);
    ilGenerator.Emit(OpCodes.Conv_I);
    ilGenerator.Emit(OpCodes.Ret);
    return typeBuilder.CreateType().GetMethod("ConvertByrefToPtr");
  }

  public static IntPtr ConvertSByteByrefToPtr(ref sbyte value)
  {
    return UnsafeMethods._ConvertSByteByrefToPtr(ref value);
  }

  public static IntPtr ConvertInt16ByrefToPtr(ref short value)
  {
    return UnsafeMethods._ConvertInt16ByrefToPtr(ref value);
  }

  public static IntPtr ConvertInt32ByrefToPtr(ref int value)
  {
    return UnsafeMethods._ConvertInt32ByrefToPtr(ref value);
  }

  public static IntPtr ConvertInt64ByrefToPtr(ref long value)
  {
    return UnsafeMethods._ConvertInt64ByrefToPtr(ref value);
  }

  public static IntPtr ConvertByteByrefToPtr(ref byte value)
  {
    return UnsafeMethods._ConvertByteByrefToPtr(ref value);
  }

  public static IntPtr ConvertUInt16ByrefToPtr(ref ushort value)
  {
    return UnsafeMethods._ConvertUInt16ByrefToPtr(ref value);
  }

  public static IntPtr ConvertUInt32ByrefToPtr(ref uint value)
  {
    return UnsafeMethods._ConvertUInt32ByrefToPtr(ref value);
  }

  public static IntPtr ConvertUInt64ByrefToPtr(ref ulong value)
  {
    return UnsafeMethods._ConvertUInt64ByrefToPtr(ref value);
  }

  public static IntPtr ConvertIntPtrByrefToPtr(ref IntPtr value)
  {
    return UnsafeMethods._ConvertIntPtrByrefToPtr(ref value);
  }

  public static IntPtr ConvertUIntPtrByrefToPtr(ref UIntPtr value)
  {
    return UnsafeMethods._ConvertUIntPtrByrefToPtr(ref value);
  }

  public static IntPtr ConvertSingleByrefToPtr(ref float value)
  {
    return UnsafeMethods._ConvertSingleByrefToPtr(ref value);
  }

  public static IntPtr ConvertDoubleByrefToPtr(ref double value)
  {
    return UnsafeMethods._ConvertDoubleByrefToPtr(ref value);
  }

  public static IntPtr ConvertDecimalByrefToPtr(ref Decimal value)
  {
    return UnsafeMethods._ConvertDecimalByrefToPtr(ref value);
  }

  public static IntPtr ConvertVariantByrefToPtr(ref Variant value)
  {
    return UnsafeMethods._ConvertVariantByrefToPtr(ref value);
  }

  internal static Variant GetVariantForObject(object obj)
  {
    Variant variant = new Variant();
    if (obj == null)
      return variant;
    UnsafeMethods.InitVariantForObject(obj, ref variant);
    return variant;
  }

  internal static void InitVariantForObject(object obj, ref Variant variant)
  {
    if (obj is IDispatch)
      variant.AsDispatch = obj;
    else
      Marshal.GetNativeVariantForObject(obj, UnsafeMethods.ConvertVariantByrefToPtr(ref variant));
  }

  [Obsolete("do not use this method", true)]
  public static object GetObjectForVariant(Variant variant)
  {
    return Marshal.GetObjectForNativeVariant(UnsafeMethods.ConvertVariantByrefToPtr(ref variant));
  }

  [Obsolete("do not use this method", true)]
  public static int IUnknownRelease(IntPtr interfacePointer)
  {
    return UnsafeMethods._IUnknownRelease(interfacePointer);
  }

  [Obsolete("do not use this method", true)]
  public static void IUnknownReleaseNotZero(IntPtr interfacePointer)
  {
    if (!(interfacePointer != IntPtr.Zero))
      return;
    UnsafeMethods.IUnknownRelease(interfacePointer);
  }

  [Obsolete("do not use this method", true)]
  public static int IDispatchInvoke(
    IntPtr dispatchPointer,
    int memberDispId,
    System.Runtime.InteropServices.ComTypes.INVOKEKIND flags,
    ref System.Runtime.InteropServices.ComTypes.DISPPARAMS dispParams,
    out Variant result,
    out ExcepInfo excepInfo,
    out uint argErr)
  {
    int num = UnsafeMethods._IDispatchInvoke(dispatchPointer, memberDispId, flags, ref dispParams, out result, out excepInfo, out argErr);
    if (num == -2147352573 /*0x80020003*/ && (flags & System.Runtime.InteropServices.ComTypes.INVOKEKIND.INVOKE_FUNC) != (System.Runtime.InteropServices.ComTypes.INVOKEKIND) 0 && (flags & (System.Runtime.InteropServices.ComTypes.INVOKEKIND.INVOKE_PROPERTYPUT | System.Runtime.InteropServices.ComTypes.INVOKEKIND.INVOKE_PROPERTYPUTREF)) == (System.Runtime.InteropServices.ComTypes.INVOKEKIND) 0)
      num = UnsafeMethods._IDispatchInvokeNoResult(dispatchPointer, memberDispId, System.Runtime.InteropServices.ComTypes.INVOKEKIND.INVOKE_FUNC, ref dispParams, out result, out excepInfo, out argErr);
    return num;
  }

  [Obsolete("do not use this method", true)]
  public static IntPtr GetIdsOfNamedParameters(
    IDispatch dispatch,
    string[] names,
    int methodDispId,
    out GCHandle pinningHandle)
  {
    pinningHandle = GCHandle.Alloc((object) null, GCHandleType.Pinned);
    int[] numArray = new int[names.Length];
    Guid empty = Guid.Empty;
    int idsOfNames = dispatch.TryGetIDsOfNames(ref empty, names, (uint) names.Length, 0, numArray);
    if (idsOfNames < 0)
      Marshal.ThrowExceptionForHR(idsOfNames);
    int[] arr = methodDispId == numArray[0] ? numArray.RemoveFirst<int>() : throw Error.GetIDsOfNamesInvalid((object) names[0]);
    pinningHandle.Target = (object) arr;
    return Marshal.UnsafeAddrOfPinnedArrayElement((Array) arr, 0);
  }

  private static void EmitLoadArg(ILGenerator il, int index)
  {
    ContractUtils.Requires(index >= 0, nameof (index));
    switch (index)
    {
      case 0:
        il.Emit(OpCodes.Ldarg_0);
        break;
      case 1:
        il.Emit(OpCodes.Ldarg_1);
        break;
      case 2:
        il.Emit(OpCodes.Ldarg_2);
        break;
      case 3:
        il.Emit(OpCodes.Ldarg_3);
        break;
      default:
        if (index <= (int) byte.MaxValue)
        {
          il.Emit(OpCodes.Ldarg_S, (byte) index);
          break;
        }
        il.Emit(OpCodes.Ldarg, index);
        break;
    }
  }

  [Conditional("DEBUG")]
  public static void AssertByrefPointsToStack(IntPtr ptr)
  {
    if (Marshal.ReadInt32(ptr) == 269488144 /*0x10101010*/)
      return;
    int num = 269488144 /*0x10101010*/;
    UnsafeMethods.ConvertInt32ByrefToPtr(ref num);
  }

  internal static ModuleBuilder DynamicModule
  {
    get
    {
      if ((Module) UnsafeMethods._dynamicModule != (Module) null)
        return UnsafeMethods._dynamicModule;
      lock (UnsafeMethods._lock)
      {
        if ((Module) UnsafeMethods._dynamicModule == (Module) null)
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
          string str = typeof (VariantArray).Namespace + ".DynamicAssembly";
          AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(str), AssemblyBuilderAccess.Run, (IEnumerable<CustomAttributeBuilder>) assemblyAttributes);
          assemblyBuilder.DefineVersionInfoResource();
          UnsafeMethods._dynamicModule = assemblyBuilder.DefineDynamicModule(str);
        }
        return UnsafeMethods._dynamicModule;
      }
    }
  }

  private static UnsafeMethods.IUnknownReleaseDelegate Create_IUnknownRelease()
  {
    DynamicMethod dynamicMethod = new DynamicMethod("IUnknownRelease", typeof (int), new Type[1]
    {
      typeof (IntPtr)
    }, (Module) UnsafeMethods.DynamicModule);
    ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
    ilGenerator.Emit(OpCodes.Ldarg_0);
    int num = 2 * Marshal.SizeOf(typeof (IntPtr));
    ilGenerator.Emit(OpCodes.Ldarg_0);
    ilGenerator.Emit(OpCodes.Ldind_I);
    ilGenerator.Emit(OpCodes.Ldc_I4, num);
    ilGenerator.Emit(OpCodes.Add);
    ilGenerator.Emit(OpCodes.Ldind_I);
    SignatureHelper methodSigHelper = SignatureHelper.GetMethodSigHelper(CallingConvention.Winapi, typeof (int));
    methodSigHelper.AddArgument(typeof (IntPtr));
    ilGenerator.Emit(OpCodes.Calli, methodSigHelper);
    ilGenerator.Emit(OpCodes.Ret);
    return (UnsafeMethods.IUnknownReleaseDelegate) ((MethodInfo) dynamicMethod).CreateDelegate(typeof (UnsafeMethods.IUnknownReleaseDelegate));
  }

  private static IntPtr GetNullInterfaceId()
  {
    int cb = Marshal.SizeOf((object) Guid.Empty);
    IntPtr ptr = Marshal.AllocHGlobal(cb);
    for (int ofs = 0; ofs < cb; ++ofs)
      Marshal.WriteByte(ptr, ofs, (byte) 0);
    return ptr;
  }

  private static UnsafeMethods.IDispatchInvokeDelegate _IDispatchInvokeNoResult
  {
    get
    {
      if (UnsafeMethods._IDispatchInvokeNoResultImpl == null)
      {
        lock (UnsafeMethods._IDispatchInvoke)
        {
          if (UnsafeMethods._IDispatchInvokeNoResultImpl == null)
            UnsafeMethods._IDispatchInvokeNoResultImpl = UnsafeMethods.Create_IDispatchInvoke(false);
        }
      }
      return UnsafeMethods._IDispatchInvokeNoResultImpl;
    }
  }

  private static UnsafeMethods.IDispatchInvokeDelegate Create_IDispatchInvoke(bool returnResult)
  {
    DynamicMethod dynamicMethod = new DynamicMethod("IDispatchInvoke", typeof (int), new Type[7]
    {
      typeof (IntPtr),
      typeof (int),
      typeof (System.Runtime.InteropServices.ComTypes.INVOKEKIND),
      typeof (System.Runtime.InteropServices.ComTypes.DISPPARAMS).MakeByRefType(),
      typeof (Variant).MakeByRefType(),
      typeof (ExcepInfo).MakeByRefType(),
      typeof (uint).MakeByRefType()
    }, (Module) UnsafeMethods.DynamicModule);
    ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
    UnsafeMethods.EmitLoadArg(ilGenerator, 0);
    UnsafeMethods.EmitLoadArg(ilGenerator, 1);
    if (IntPtr.Size == 4)
      ilGenerator.Emit(OpCodes.Ldc_I4, UnsafeMethods.NullInterfaceId.ToInt32());
    else
      ilGenerator.Emit(OpCodes.Ldc_I8, UnsafeMethods.NullInterfaceId.ToInt64());
    ilGenerator.Emit(OpCodes.Conv_I);
    ilGenerator.Emit(OpCodes.Ldc_I4_0);
    UnsafeMethods.EmitLoadArg(ilGenerator, 2);
    UnsafeMethods.EmitLoadArg(ilGenerator, 3);
    if (returnResult)
      UnsafeMethods.EmitLoadArg(ilGenerator, 4);
    else
      ilGenerator.Emit(OpCodes.Ldsfld, typeof (IntPtr).GetField("Zero"));
    UnsafeMethods.EmitLoadArg(ilGenerator, 5);
    UnsafeMethods.EmitLoadArg(ilGenerator, 6);
    int num = 6 * Marshal.SizeOf(typeof (IntPtr));
    UnsafeMethods.EmitLoadArg(ilGenerator, 0);
    ilGenerator.Emit(OpCodes.Ldind_I);
    ilGenerator.Emit(OpCodes.Ldc_I4, num);
    ilGenerator.Emit(OpCodes.Add);
    ilGenerator.Emit(OpCodes.Ldind_I);
    SignatureHelper methodSigHelper = SignatureHelper.GetMethodSigHelper(CallingConvention.Winapi, typeof (int));
    Type[] arguments = new Type[9]
    {
      typeof (IntPtr),
      typeof (int),
      typeof (IntPtr),
      typeof (int),
      typeof (ushort),
      typeof (IntPtr),
      typeof (IntPtr),
      typeof (IntPtr),
      typeof (IntPtr)
    };
    methodSigHelper.AddArguments(arguments, (Type[][]) null, (Type[][]) null);
    ilGenerator.Emit(OpCodes.Calli, methodSigHelper);
    ilGenerator.Emit(OpCodes.Ret);
    return (UnsafeMethods.IDispatchInvokeDelegate) ((MethodInfo) dynamicMethod).CreateDelegate(typeof (UnsafeMethods.IDispatchInvokeDelegate));
  }

  public delegate IntPtr ConvertByrefToPtrDelegate<T>(ref T value);

  private delegate int IUnknownReleaseDelegate(IntPtr interfacePointer);

  private delegate int IDispatchInvokeDelegate(
    IntPtr dispatchPointer,
    int memberDispId,
    System.Runtime.InteropServices.ComTypes.INVOKEKIND flags,
    ref System.Runtime.InteropServices.ComTypes.DISPPARAMS dispParams,
    out Variant result,
    out ExcepInfo excepInfo,
    out uint argErr);
}
