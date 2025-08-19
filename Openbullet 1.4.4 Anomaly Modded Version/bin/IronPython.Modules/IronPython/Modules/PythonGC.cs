// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonGC
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Modules;

public static class PythonGC
{
  public const string __doc__ = "Provides functions for inspecting, configuring, and forcing garbage collection.";
  public const int DEBUG_STATS = 1;
  public const int DEBUG_COLLECTABLE = 2;
  public const int DEBUG_UNCOLLECTABLE = 4;
  public const int DEBUG_INSTANCES = 8;
  public const int DEBUG_OBJECTS = 16 /*0x10*/;
  public const int DEBUG_SAVEALL = 32 /*0x20*/;
  public const int DEBUG_LEAK = 62;
  private static readonly object _threadholdKey = new object();

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    context.SetModuleState(PythonGC._threadholdKey, (object) PythonTuple.MakeTuple((object) 65536 /*0x010000*/, (object) 262144 /*0x040000*/, (object) 1048576 /*0x100000*/));
  }

  public static void enable()
  {
  }

  public static void disable(CodeContext context)
  {
    PythonOps.Warn(context, PythonExceptions.RuntimeWarning, "IronPython has no support for disabling the GC");
  }

  public static object isenabled() => ScriptingRuntimeHelpers.True;

  public static int collect(CodeContext context, int generation)
  {
    return context.LanguageContext.Collect(generation);
  }

  public static int collect(CodeContext context) => PythonGC.collect(context, GC.MaxGeneration);

  public static void set_debug(object o)
  {
    throw PythonOps.NotImplementedError("gc.set_debug isn't implemented");
  }

  public static object get_debug() => (object) 0;

  public static object[] get_objects()
  {
    throw PythonOps.NotImplementedError("gc.get_objects isn't implemented");
  }

  public static void set_threshold(CodeContext context, params object[] args)
  {
    if (args.Length == 0)
      throw PythonOps.TypeError("set_threshold() takes at least 1 argument (0 given)");
    if (args.Length > 3)
      throw PythonOps.TypeError("set_threshold() takes at most 3 arguments ({0} given)", (object) args.Length);
    if (((IEnumerable<object>) args).Any<object>((Func<object, bool>) (x => x is double)))
      throw PythonOps.TypeError("integer argument expected, got float");
    if (!((IEnumerable<object>) args).All<object>((Func<object, bool>) (x => x is int)))
      throw PythonOps.TypeError("an integer is required");
    PythonTuple threshold = PythonGC.get_threshold(context);
    object[] array = ((IEnumerable<object>) args).Take<object>(args.Length).Concat<object>(((IEnumerable<object>) threshold.ToArray()).Skip<object>(args.Length)).ToArray<object>();
    PythonGC.SetThresholds(context, PythonTuple.MakeTuple(array));
  }

  public static PythonTuple get_threshold(CodeContext context) => PythonGC.GetThresholds(context);

  public static object[] get_referrers(params object[] objs)
  {
    throw PythonOps.NotImplementedError("gc.get_referrers isn't implemented");
  }

  public static object[] get_referents(params object[] objs)
  {
    throw PythonOps.NotImplementedError("gc.get_referents isn't implemented");
  }

  public static IronPython.Runtime.List garbage => new IronPython.Runtime.List();

  private static PythonTuple GetThresholds(CodeContext context)
  {
    return (PythonTuple) context.LanguageContext.GetModuleState(PythonGC._threadholdKey);
  }

  private static void SetThresholds(CodeContext context, PythonTuple thresholds)
  {
    context.LanguageContext.SetModuleState(PythonGC._threadholdKey, (object) thresholds);
  }
}
