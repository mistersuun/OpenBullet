// Decompiled with JetBrains decompiler
// Type: IronPython.Compiler.PythonCallTargets
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using System;

#nullable disable
namespace IronPython.Compiler;

internal static class PythonCallTargets
{
  public const int MaxArgs = 15;

  public static object OriginalCallTargetN(PythonFunction function, params object[] args)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object[], object>) function.func_code.Target)(function, args);
  }

  public static object OriginalCallTarget0(PythonFunction function)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object>) function.func_code.Target)(function);
  }

  public static object OriginalCallTarget1(PythonFunction function, object arg0)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object, object>) function.func_code.Target)(function, arg0);
  }

  public static object OriginalCallTarget2(PythonFunction function, object arg0, object arg1)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object, object, object>) function.func_code.Target)(function, arg0, arg1);
  }

  public static object OriginalCallTarget3(
    PythonFunction function,
    object arg0,
    object arg1,
    object arg2)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object, object, object, object>) function.func_code.Target)(function, arg0, arg1, arg2);
  }

  public static object OriginalCallTarget4(
    PythonFunction function,
    object arg0,
    object arg1,
    object arg2,
    object arg3)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object, object, object, object, object>) function.func_code.Target)(function, arg0, arg1, arg2, arg3);
  }

  public static object OriginalCallTarget5(
    PythonFunction function,
    object arg0,
    object arg1,
    object arg2,
    object arg3,
    object arg4)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object, object, object, object, object, object>) function.func_code.Target)(function, arg0, arg1, arg2, arg3, arg4);
  }

  public static object OriginalCallTarget6(
    PythonFunction function,
    object arg0,
    object arg1,
    object arg2,
    object arg3,
    object arg4,
    object arg5)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object, object, object, object, object, object, object>) function.func_code.Target)(function, arg0, arg1, arg2, arg3, arg4, arg5);
  }

  public static object OriginalCallTarget7(
    PythonFunction function,
    object arg0,
    object arg1,
    object arg2,
    object arg3,
    object arg4,
    object arg5,
    object arg6)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object>) function.func_code.Target)(function, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
  }

  public static object OriginalCallTarget8(
    PythonFunction function,
    object arg0,
    object arg1,
    object arg2,
    object arg3,
    object arg4,
    object arg5,
    object arg6,
    object arg7)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object>) function.func_code.Target)(function, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
  }

  public static object OriginalCallTarget9(
    PythonFunction function,
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
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object>) function.func_code.Target)(function, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
  }

  public static object OriginalCallTarget10(
    PythonFunction function,
    object arg0,
    object arg1,
    object arg2,
    object arg3,
    object arg4,
    object arg5,
    object arg6,
    object arg7,
    object arg8,
    object arg9)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object>) function.func_code.Target)(function, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
  }

  public static object OriginalCallTarget11(
    PythonFunction function,
    object arg0,
    object arg1,
    object arg2,
    object arg3,
    object arg4,
    object arg5,
    object arg6,
    object arg7,
    object arg8,
    object arg9,
    object arg10)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object>) function.func_code.Target)(function, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
  }

  public static object OriginalCallTarget12(
    PythonFunction function,
    object arg0,
    object arg1,
    object arg2,
    object arg3,
    object arg4,
    object arg5,
    object arg6,
    object arg7,
    object arg8,
    object arg9,
    object arg10,
    object arg11)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object>) function.func_code.Target)(function, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
  }

  public static object OriginalCallTarget13(
    PythonFunction function,
    object arg0,
    object arg1,
    object arg2,
    object arg3,
    object arg4,
    object arg5,
    object arg6,
    object arg7,
    object arg8,
    object arg9,
    object arg10,
    object arg11,
    object arg12)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) function.func_code.Target)(function, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
  }

  public static object OriginalCallTarget14(
    PythonFunction function,
    object arg0,
    object arg1,
    object arg2,
    object arg3,
    object arg4,
    object arg5,
    object arg6,
    object arg7,
    object arg8,
    object arg9,
    object arg10,
    object arg11,
    object arg12,
    object arg13)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) function.func_code.Target)(function, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
  }

  public static object OriginalCallTarget15(
    PythonFunction function,
    object arg0,
    object arg1,
    object arg2,
    object arg3,
    object arg4,
    object arg5,
    object arg6,
    object arg7,
    object arg8,
    object arg9,
    object arg10,
    object arg11,
    object arg12,
    object arg13,
    object arg14)
  {
    function.func_code.LazyCompileFirstTarget(function);
    return ((Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>) function.func_code.Target)(function, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
  }

  internal static Type GetPythonTargetType(
    bool wrapper,
    int parameters,
    out Delegate originalTarget)
  {
    if (!wrapper)
    {
      switch (parameters)
      {
        case 0:
          originalTarget = (Delegate) new Func<PythonFunction, object>(PythonCallTargets.OriginalCallTarget0);
          return typeof (Func<PythonFunction, object>);
        case 1:
          originalTarget = (Delegate) new Func<PythonFunction, object, object>(PythonCallTargets.OriginalCallTarget1);
          return typeof (Func<PythonFunction, object, object>);
        case 2:
          originalTarget = (Delegate) new Func<PythonFunction, object, object, object>(PythonCallTargets.OriginalCallTarget2);
          return typeof (Func<PythonFunction, object, object, object>);
        case 3:
          originalTarget = (Delegate) new Func<PythonFunction, object, object, object, object>(PythonCallTargets.OriginalCallTarget3);
          return typeof (Func<PythonFunction, object, object, object, object>);
        case 4:
          originalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object>(PythonCallTargets.OriginalCallTarget4);
          return typeof (Func<PythonFunction, object, object, object, object, object>);
        case 5:
          originalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object>(PythonCallTargets.OriginalCallTarget5);
          return typeof (Func<PythonFunction, object, object, object, object, object, object>);
        case 6:
          originalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object>(PythonCallTargets.OriginalCallTarget6);
          return typeof (Func<PythonFunction, object, object, object, object, object, object, object>);
        case 7:
          originalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object>(PythonCallTargets.OriginalCallTarget7);
          return typeof (Func<PythonFunction, object, object, object, object, object, object, object, object>);
        case 8:
          originalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object>(PythonCallTargets.OriginalCallTarget8);
          return typeof (Func<PythonFunction, object, object, object, object, object, object, object, object, object>);
        case 9:
          originalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object, object>(PythonCallTargets.OriginalCallTarget9);
          return typeof (Func<PythonFunction, object, object, object, object, object, object, object, object, object, object>);
        case 10:
          originalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object>(PythonCallTargets.OriginalCallTarget10);
          return typeof (Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object>);
        case 11:
          originalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object>(PythonCallTargets.OriginalCallTarget11);
          return typeof (Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object>);
        case 12:
          originalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object>(PythonCallTargets.OriginalCallTarget12);
          return typeof (Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object>);
        case 13:
          originalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object>(PythonCallTargets.OriginalCallTarget13);
          return typeof (Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object>);
        case 14:
          originalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>(PythonCallTargets.OriginalCallTarget14);
          return typeof (Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>);
        case 15:
          originalTarget = (Delegate) new Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>(PythonCallTargets.OriginalCallTarget15);
          return typeof (Func<PythonFunction, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>);
      }
    }
    originalTarget = (Delegate) new Func<PythonFunction, object[], object>(PythonCallTargets.OriginalCallTargetN);
    return typeof (Func<PythonFunction, object[], object>);
  }
}
