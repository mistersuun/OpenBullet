// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.CallInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

public abstract class CallInstruction : Instruction
{
  private static readonly Dictionary<MethodInfo, CallInstruction> _cache = new Dictionary<MethodInfo, CallInstruction>();
  private const int MaxHelpers = 10;
  private const int MaxArgs = 3;

  public abstract MethodInfo Info { get; }

  public abstract int ArgumentCount { get; }

  internal CallInstruction()
  {
  }

  public static CallInstruction Create(MethodInfo info)
  {
    return CallInstruction.Create(info, info.GetParameters());
  }

  public static CallInstruction Create(MethodInfo info, ParameterInfo[] parameters)
  {
    int length = parameters.Length;
    if (!info.IsStatic)
      ++length;
    if (info.DeclaringType != (Type) null && info.DeclaringType.IsArray && (info.Name == "Get" || info.Name == "Set"))
      return CallInstruction.GetArrayAccessor(info, length);
    if (ReflectionUtils.IsDynamicMethod((MethodBase) info) || !info.IsStatic && info.DeclaringType.IsValueType())
      return (CallInstruction) new MethodInfoCallInstruction(info, length);
    if (length >= 10)
      return (CallInstruction) new MethodInfoCallInstruction(info, length);
    foreach (ParameterInfo parameter in parameters)
    {
      if (parameter.ParameterType.IsByRef)
        return (CallInstruction) new MethodInfoCallInstruction(info, length);
    }
    if (CallInstruction.ShouldCache(info))
    {
      lock (CallInstruction._cache)
      {
        CallInstruction callInstruction;
        if (CallInstruction._cache.TryGetValue(info, out callInstruction))
          return callInstruction;
      }
    }
    CallInstruction callInstruction1;
    try
    {
      callInstruction1 = length >= 3 ? CallInstruction.SlowCreate(info, parameters) : CallInstruction.FastCreate(info, parameters);
    }
    catch (TargetInvocationException ex)
    {
      if (!(ex.InnerException is NotSupportedException))
        throw ex.InnerException;
      callInstruction1 = (CallInstruction) new MethodInfoCallInstruction(info, length);
    }
    catch (NotSupportedException ex)
    {
      callInstruction1 = (CallInstruction) new MethodInfoCallInstruction(info, length);
    }
    if (CallInstruction.ShouldCache(info))
    {
      lock (CallInstruction._cache)
        CallInstruction._cache[info] = callInstruction1;
    }
    return callInstruction1;
  }

  private static CallInstruction GetArrayAccessor(MethodInfo info, int argumentCount)
  {
    Type declaringType = info.DeclaringType;
    bool flag = info.Name == "Get";
    switch (declaringType.GetArrayRank())
    {
      case 1:
        MethodInfo info1;
        if (!flag)
          info1 = RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Action<Array, int, object>(CallInstruction.ArrayItemSetter1));
        else
          info1 = declaringType.GetMethod("GetValue", new Type[1]
          {
            typeof (int)
          });
        return CallInstruction.Create(info1);
      case 2:
        MethodInfo info2;
        if (!flag)
          info2 = RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Action<Array, int, int, object>(CallInstruction.ArrayItemSetter2));
        else
          info2 = declaringType.GetMethod("GetValue", new Type[2]
          {
            typeof (int),
            typeof (int)
          });
        return CallInstruction.Create(info2);
      case 3:
        MethodInfo info3;
        if (!flag)
          info3 = RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Action<Array, int, int, int, object>(CallInstruction.ArrayItemSetter3));
        else
          info3 = declaringType.GetMethod("GetValue", new Type[3]
          {
            typeof (int),
            typeof (int),
            typeof (int)
          });
        return CallInstruction.Create(info3);
      default:
        return (CallInstruction) new MethodInfoCallInstruction(info, argumentCount);
    }
  }

  public static void ArrayItemSetter1(Array array, int index0, object value)
  {
    array.SetValue(value, index0);
  }

  public static void ArrayItemSetter2(Array array, int index0, int index1, object value)
  {
    array.SetValue(value, index0, index1);
  }

  public static void ArrayItemSetter3(
    Array array,
    int index0,
    int index1,
    int index2,
    object value)
  {
    array.SetValue(value, index0, index1, index2);
  }

  private static bool ShouldCache(MethodInfo info)
  {
    return !ReflectionUtils.IsDynamicMethod((MethodBase) info);
  }

  private static Type TryGetParameterOrReturnType(MethodInfo target, ParameterInfo[] pi, int index)
  {
    if (!target.IsStatic)
    {
      --index;
      if (index < 0)
        return target.DeclaringType;
    }
    if (index < pi.Length)
      return pi[index].ParameterType;
    return target.ReturnType == typeof (void) || index > pi.Length ? (Type) null : target.ReturnType;
  }

  private static bool IndexIsNotReturnType(int index, MethodInfo target, ParameterInfo[] pi)
  {
    if (pi.Length != index)
      return true;
    return pi.Length == index && !target.IsStatic;
  }

  private static CallInstruction SlowCreate(MethodInfo info, ParameterInfo[] pis)
  {
    List<Type> typeList = new List<Type>();
    if (!info.IsStatic)
      typeList.Add(info.DeclaringType);
    foreach (ParameterInfo pi in pis)
      typeList.Add(pi.ParameterType);
    if (info.ReturnType != typeof (void))
      typeList.Add(info.ReturnType);
    Type[] array = typeList.ToArray();
    return (CallInstruction) Activator.CreateInstance(CallInstruction.GetHelperType(info, array), (object) info);
  }

  public sealed override int ProducedStack => !(this.Info.ReturnType == typeof (void)) ? 1 : 0;

  public sealed override int ConsumedStack => this.ArgumentCount;

  public sealed override string InstructionName => "Call";

  public override string ToString() => $"Call({(object) this.Info})";

  public virtual object InvokeInstance(object instance, params object[] args)
  {
    switch (args.Length)
    {
      case 0:
        return this.Invoke(instance);
      case 1:
        return this.Invoke(instance, args[0]);
      case 2:
        return this.Invoke(instance, args[0], args[1]);
      case 3:
        return this.Invoke(instance, args[0], args[1], args[2]);
      case 4:
        return this.Invoke(instance, args[0], args[1], args[2], args[3]);
      case 5:
        return this.Invoke(instance, args[0], args[1], args[2], args[3], args[4]);
      case 6:
        return this.Invoke(instance, args[0], args[1], args[2], args[3], args[4], args[5]);
      case 7:
        return this.Invoke(instance, args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
      case 8:
        return this.Invoke(instance, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
      default:
        throw new InvalidOperationException();
    }
  }

  public virtual object Invoke(params object[] args)
  {
    switch (args.Length)
    {
      case 0:
        return this.Invoke();
      case 1:
        return this.Invoke(args[0]);
      case 2:
        return this.Invoke(args[0], args[1]);
      case 3:
        return this.Invoke(args[0], args[1], args[2]);
      case 4:
        return this.Invoke(args[0], args[1], args[2], args[3]);
      case 5:
        return this.Invoke(args[0], args[1], args[2], args[3], args[4]);
      case 6:
        return this.Invoke(args[0], args[1], args[2], args[3], args[4], args[5]);
      case 7:
        return this.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
      case 8:
        return this.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
      case 9:
        return this.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);
      default:
        throw new InvalidOperationException();
    }
  }

  public virtual object Invoke() => throw new InvalidOperationException();

  public virtual object Invoke(object arg0) => throw new InvalidOperationException();

  public virtual object Invoke(object arg0, object arg1) => throw new InvalidOperationException();

  public virtual object Invoke(object arg0, object arg1, object arg2)
  {
    throw new InvalidOperationException();
  }

  public virtual object Invoke(object arg0, object arg1, object arg2, object arg3)
  {
    throw new InvalidOperationException();
  }

  public virtual object Invoke(object arg0, object arg1, object arg2, object arg3, object arg4)
  {
    throw new InvalidOperationException();
  }

  public virtual object Invoke(
    object arg0,
    object arg1,
    object arg2,
    object arg3,
    object arg4,
    object arg5)
  {
    throw new InvalidOperationException();
  }

  public virtual object Invoke(
    object arg0,
    object arg1,
    object arg2,
    object arg3,
    object arg4,
    object arg5,
    object arg6)
  {
    throw new InvalidOperationException();
  }

  public virtual object Invoke(
    object arg0,
    object arg1,
    object arg2,
    object arg3,
    object arg4,
    object arg5,
    object arg6,
    object arg7)
  {
    throw new InvalidOperationException();
  }

  public virtual object Invoke(
    object arg0,
    object arg1,
    object arg2,
    object arg3,
    object arg4,
    object arg5,
    object arg6,
    object arg7,
    object arg8)
  {
    throw new InvalidOperationException();
  }

  private static CallInstruction FastCreate(MethodInfo target, ParameterInfo[] pi)
  {
    Type parameterOrReturnType = CallInstruction.TryGetParameterOrReturnType(target, pi, 0);
    if (parameterOrReturnType == (Type) null)
      return (CallInstruction) new ActionCallInstruction(target);
    if (parameterOrReturnType.IsEnum())
      return CallInstruction.SlowCreate(target, pi);
    switch (parameterOrReturnType.GetTypeCode())
    {
      case TypeCode.Object:
        if (!(parameterOrReturnType != typeof (object)) || !CallInstruction.IndexIsNotReturnType(0, target, pi) && !parameterOrReturnType.IsValueType())
          return CallInstruction.FastCreate<object>(target, pi);
        break;
      case TypeCode.Boolean:
        return CallInstruction.FastCreate<bool>(target, pi);
      case TypeCode.Char:
        return CallInstruction.FastCreate<char>(target, pi);
      case TypeCode.SByte:
        return CallInstruction.FastCreate<sbyte>(target, pi);
      case TypeCode.Byte:
        return CallInstruction.FastCreate<byte>(target, pi);
      case TypeCode.Int16:
        return CallInstruction.FastCreate<short>(target, pi);
      case TypeCode.UInt16:
        return CallInstruction.FastCreate<ushort>(target, pi);
      case TypeCode.Int32:
        return CallInstruction.FastCreate<int>(target, pi);
      case TypeCode.UInt32:
        return CallInstruction.FastCreate<uint>(target, pi);
      case TypeCode.Int64:
        return CallInstruction.FastCreate<long>(target, pi);
      case TypeCode.UInt64:
        return CallInstruction.FastCreate<ulong>(target, pi);
      case TypeCode.Single:
        return CallInstruction.FastCreate<float>(target, pi);
      case TypeCode.Double:
        return CallInstruction.FastCreate<double>(target, pi);
      case TypeCode.Decimal:
        return CallInstruction.FastCreate<Decimal>(target, pi);
      case TypeCode.DateTime:
        return CallInstruction.FastCreate<DateTime>(target, pi);
      case TypeCode.String:
        return CallInstruction.FastCreate<string>(target, pi);
    }
    return CallInstruction.SlowCreate(target, pi);
  }

  private static CallInstruction FastCreate<T0>(MethodInfo target, ParameterInfo[] pi)
  {
    Type parameterOrReturnType = CallInstruction.TryGetParameterOrReturnType(target, pi, 1);
    if (parameterOrReturnType == (Type) null)
      return target.ReturnType == typeof (void) ? (CallInstruction) new ActionCallInstruction<T0>(target) : (CallInstruction) new FuncCallInstruction<T0>(target);
    if (parameterOrReturnType.IsEnum())
      return CallInstruction.SlowCreate(target, pi);
    switch (parameterOrReturnType.GetTypeCode())
    {
      case TypeCode.Object:
        if (!(parameterOrReturnType != typeof (object)) || !CallInstruction.IndexIsNotReturnType(1, target, pi) && !parameterOrReturnType.IsValueType())
          return CallInstruction.FastCreate<T0, object>(target, pi);
        break;
      case TypeCode.Boolean:
        return CallInstruction.FastCreate<T0, bool>(target, pi);
      case TypeCode.Char:
        return CallInstruction.FastCreate<T0, char>(target, pi);
      case TypeCode.SByte:
        return CallInstruction.FastCreate<T0, sbyte>(target, pi);
      case TypeCode.Byte:
        return CallInstruction.FastCreate<T0, byte>(target, pi);
      case TypeCode.Int16:
        return CallInstruction.FastCreate<T0, short>(target, pi);
      case TypeCode.UInt16:
        return CallInstruction.FastCreate<T0, ushort>(target, pi);
      case TypeCode.Int32:
        return CallInstruction.FastCreate<T0, int>(target, pi);
      case TypeCode.UInt32:
        return CallInstruction.FastCreate<T0, uint>(target, pi);
      case TypeCode.Int64:
        return CallInstruction.FastCreate<T0, long>(target, pi);
      case TypeCode.UInt64:
        return CallInstruction.FastCreate<T0, ulong>(target, pi);
      case TypeCode.Single:
        return CallInstruction.FastCreate<T0, float>(target, pi);
      case TypeCode.Double:
        return CallInstruction.FastCreate<T0, double>(target, pi);
      case TypeCode.Decimal:
        return CallInstruction.FastCreate<T0, Decimal>(target, pi);
      case TypeCode.DateTime:
        return CallInstruction.FastCreate<T0, DateTime>(target, pi);
      case TypeCode.String:
        return CallInstruction.FastCreate<T0, string>(target, pi);
    }
    return CallInstruction.SlowCreate(target, pi);
  }

  private static CallInstruction FastCreate<T0, T1>(MethodInfo target, ParameterInfo[] pi)
  {
    Type parameterOrReturnType = CallInstruction.TryGetParameterOrReturnType(target, pi, 2);
    if (parameterOrReturnType == (Type) null)
      return target.ReturnType == typeof (void) ? (CallInstruction) new ActionCallInstruction<T0, T1>(target) : (CallInstruction) new FuncCallInstruction<T0, T1>(target);
    if (parameterOrReturnType.IsEnum())
      return CallInstruction.SlowCreate(target, pi);
    switch (parameterOrReturnType.GetTypeCode())
    {
      case TypeCode.Object:
        if (!parameterOrReturnType.IsValueType())
          return (CallInstruction) new FuncCallInstruction<T0, T1, object>(target);
        break;
      case TypeCode.Boolean:
        return (CallInstruction) new FuncCallInstruction<T0, T1, bool>(target);
      case TypeCode.Char:
        return (CallInstruction) new FuncCallInstruction<T0, T1, char>(target);
      case TypeCode.SByte:
        return (CallInstruction) new FuncCallInstruction<T0, T1, sbyte>(target);
      case TypeCode.Byte:
        return (CallInstruction) new FuncCallInstruction<T0, T1, byte>(target);
      case TypeCode.Int16:
        return (CallInstruction) new FuncCallInstruction<T0, T1, short>(target);
      case TypeCode.UInt16:
        return (CallInstruction) new FuncCallInstruction<T0, T1, ushort>(target);
      case TypeCode.Int32:
        return (CallInstruction) new FuncCallInstruction<T0, T1, int>(target);
      case TypeCode.UInt32:
        return (CallInstruction) new FuncCallInstruction<T0, T1, uint>(target);
      case TypeCode.Int64:
        return (CallInstruction) new FuncCallInstruction<T0, T1, long>(target);
      case TypeCode.UInt64:
        return (CallInstruction) new FuncCallInstruction<T0, T1, ulong>(target);
      case TypeCode.Single:
        return (CallInstruction) new FuncCallInstruction<T0, T1, float>(target);
      case TypeCode.Double:
        return (CallInstruction) new FuncCallInstruction<T0, T1, double>(target);
      case TypeCode.Decimal:
        return (CallInstruction) new FuncCallInstruction<T0, T1, Decimal>(target);
      case TypeCode.DateTime:
        return (CallInstruction) new FuncCallInstruction<T0, T1, DateTime>(target);
      case TypeCode.String:
        return (CallInstruction) new FuncCallInstruction<T0, T1, string>(target);
    }
    return CallInstruction.SlowCreate(target, pi);
  }

  private static Type GetHelperType(MethodInfo info, Type[] arrTypes)
  {
    if (info.ReturnType == typeof (void))
    {
      switch (arrTypes.Length)
      {
        case 0:
          return typeof (ActionCallInstruction);
        case 1:
          return typeof (ActionCallInstruction<>).MakeGenericType(arrTypes);
        case 2:
          return typeof (ActionCallInstruction<,>).MakeGenericType(arrTypes);
        case 3:
          return typeof (ActionCallInstruction<,,>).MakeGenericType(arrTypes);
        case 4:
          return typeof (ActionCallInstruction<,,,>).MakeGenericType(arrTypes);
        case 5:
          return typeof (ActionCallInstruction<,,,,>).MakeGenericType(arrTypes);
        case 6:
          return typeof (ActionCallInstruction<,,,,,>).MakeGenericType(arrTypes);
        case 7:
          return typeof (ActionCallInstruction<,,,,,,>).MakeGenericType(arrTypes);
        case 8:
          return typeof (ActionCallInstruction<,,,,,,,>).MakeGenericType(arrTypes);
        case 9:
          return typeof (ActionCallInstruction<,,,,,,,,>).MakeGenericType(arrTypes);
        default:
          throw new InvalidOperationException();
      }
    }
    else
    {
      switch (arrTypes.Length)
      {
        case 1:
          return typeof (FuncCallInstruction<>).MakeGenericType(arrTypes);
        case 2:
          return typeof (FuncCallInstruction<,>).MakeGenericType(arrTypes);
        case 3:
          return typeof (FuncCallInstruction<,,>).MakeGenericType(arrTypes);
        case 4:
          return typeof (FuncCallInstruction<,,,>).MakeGenericType(arrTypes);
        case 5:
          return typeof (FuncCallInstruction<,,,,>).MakeGenericType(arrTypes);
        case 6:
          return typeof (FuncCallInstruction<,,,,,>).MakeGenericType(arrTypes);
        case 7:
          return typeof (FuncCallInstruction<,,,,,,>).MakeGenericType(arrTypes);
        case 8:
          return typeof (FuncCallInstruction<,,,,,,,>).MakeGenericType(arrTypes);
        case 9:
          return typeof (FuncCallInstruction<,,,,,,,,>).MakeGenericType(arrTypes);
        case 10:
          return typeof (FuncCallInstruction<,,,,,,,,,>).MakeGenericType(arrTypes);
        default:
          throw new InvalidOperationException();
      }
    }
  }

  public static MethodInfo CacheFunc<TRet>(Func<TRet> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new FuncCallInstruction<TRet>(method);
    return methodInfo;
  }

  public static MethodInfo CacheFunc<T0, TRet>(Func<T0, TRet> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new FuncCallInstruction<T0, TRet>(method);
    return methodInfo;
  }

  public static MethodInfo CacheFunc<T0, T1, TRet>(Func<T0, T1, TRet> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new FuncCallInstruction<T0, T1, TRet>(method);
    return methodInfo;
  }

  public static MethodInfo CacheFunc<T0, T1, T2, TRet>(Func<T0, T1, T2, TRet> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new FuncCallInstruction<T0, T1, T2, TRet>(method);
    return methodInfo;
  }

  public static MethodInfo CacheFunc<T0, T1, T2, T3, TRet>(Func<T0, T1, T2, T3, TRet> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new FuncCallInstruction<T0, T1, T2, T3, TRet>(method);
    return methodInfo;
  }

  public static MethodInfo CacheFunc<T0, T1, T2, T3, T4, TRet>(Func<T0, T1, T2, T3, T4, TRet> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new FuncCallInstruction<T0, T1, T2, T3, T4, TRet>(method);
    return methodInfo;
  }

  public static MethodInfo CacheFunc<T0, T1, T2, T3, T4, T5, TRet>(
    Func<T0, T1, T2, T3, T4, T5, TRet> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new FuncCallInstruction<T0, T1, T2, T3, T4, T5, TRet>(method);
    return methodInfo;
  }

  public static MethodInfo CacheFunc<T0, T1, T2, T3, T4, T5, T6, TRet>(
    Func<T0, T1, T2, T3, T4, T5, T6, TRet> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new FuncCallInstruction<T0, T1, T2, T3, T4, T5, T6, TRet>(method);
    return methodInfo;
  }

  public static MethodInfo CacheFunc<T0, T1, T2, T3, T4, T5, T6, T7, TRet>(
    Func<T0, T1, T2, T3, T4, T5, T6, T7, TRet> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new FuncCallInstruction<T0, T1, T2, T3, T4, T5, T6, T7, TRet>(method);
    return methodInfo;
  }

  public static MethodInfo CacheFunc<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>(
    Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new FuncCallInstruction<T0, T1, T2, T3, T4, T5, T6, T7, T8, TRet>(method);
    return methodInfo;
  }

  public static MethodInfo CacheAction(Action method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new ActionCallInstruction(method);
    return methodInfo;
  }

  public static MethodInfo CacheAction<T0>(Action<T0> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new ActionCallInstruction<T0>(method);
    return methodInfo;
  }

  public static MethodInfo CacheAction<T0, T1>(Action<T0, T1> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new ActionCallInstruction<T0, T1>(method);
    return methodInfo;
  }

  public static MethodInfo CacheAction<T0, T1, T2>(Action<T0, T1, T2> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new ActionCallInstruction<T0, T1, T2>(method);
    return methodInfo;
  }

  public static MethodInfo CacheAction<T0, T1, T2, T3>(Action<T0, T1, T2, T3> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new ActionCallInstruction<T0, T1, T2, T3>(method);
    return methodInfo;
  }

  public static MethodInfo CacheAction<T0, T1, T2, T3, T4>(Action<T0, T1, T2, T3, T4> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new ActionCallInstruction<T0, T1, T2, T3, T4>(method);
    return methodInfo;
  }

  public static MethodInfo CacheAction<T0, T1, T2, T3, T4, T5>(Action<T0, T1, T2, T3, T4, T5> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new ActionCallInstruction<T0, T1, T2, T3, T4, T5>(method);
    return methodInfo;
  }

  public static MethodInfo CacheAction<T0, T1, T2, T3, T4, T5, T6>(
    Action<T0, T1, T2, T3, T4, T5, T6> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new ActionCallInstruction<T0, T1, T2, T3, T4, T5, T6>(method);
    return methodInfo;
  }

  public static MethodInfo CacheAction<T0, T1, T2, T3, T4, T5, T6, T7>(
    Action<T0, T1, T2, T3, T4, T5, T6, T7> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new ActionCallInstruction<T0, T1, T2, T3, T4, T5, T6, T7>(method);
    return methodInfo;
  }

  public static MethodInfo CacheAction<T0, T1, T2, T3, T4, T5, T6, T7, T8>(
    Action<T0, T1, T2, T3, T4, T5, T6, T7, T8> method)
  {
    MethodInfo methodInfo = RuntimeReflectionExtensions.GetMethodInfo((Delegate) method);
    lock (CallInstruction._cache)
      CallInstruction._cache[methodInfo] = (CallInstruction) new ActionCallInstruction<T0, T1, T2, T3, T4, T5, T6, T7, T8>(method);
    return methodInfo;
  }
}
